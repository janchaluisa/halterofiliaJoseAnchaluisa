DROP DATABASE DB_PRUEBAJOSEANCHALUISA
go
CREATE DATABASE DB_PRUEBAJOSEANCHALUISA
GO

USE DB_PRUEBAJOSEANCHALUISA
GO

CREATE TABLE dbo.Logs(
	Mensaje varchar (500) NULL,
	Fecha datetime NULL
)
GO

CREATE	TABLE dbo.tb_Paises
(
	idPais		int primary key not null identity(1,1),
	iniciales	varchar(5) not null UNIQUE,  --veo q solo es de 3 caracteres
	descripcion	varchar(20)  not null
)

INSERT	INTO dbo.tb_Paises(iniciales, descripcion) values ('COL', 'COLOMBIA');
INSERT	INTO dbo.tb_Paises(iniciales, descripcion) values ('CHN', 'CHINA');
INSERT	INTO dbo.tb_Paises(iniciales, descripcion) values ('AUS', 'AUSTRALIA');

CREATE	TABLE dbo.tb_Depostistas
(
	idDeportista	int primary key not null identity(1,1), --esta me imagino que reemplaz la cedula
	nombre_completo	varchar(200) not null,
	idPais			int not null,
	FOREIGN KEY (idPais) REFERENCES tb_Paises(idPais)
)

INSERT	INTO dbo.tb_Depostistas(nombre_completo, idPais) values ('Anthony Boral', 1);
INSERT	INTO dbo.tb_Depostistas(nombre_completo, idPais) values ('Marcela Lopez', 2);
INSERT	INTO dbo.tb_Depostistas(nombre_completo, idPais) values ('Alejandra Ortega', 3);

CREATE	TABLE dbo.tb_Intentos
(
	idIntentos		int primary key not null identity(1,1),
	idDeportista	int not null,
	Tipo VARCHAR(10) CHECK (Tipo IN ('Arranque', 'Envion')),
    Peso INT NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (idDeportista) REFERENCES tb_Depostistas(idDeportista)
)

INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (1, 'Arranque', 160);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (1, 'Arranque', 100);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (1, 'Envion', 90);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (1, 'Envion', 130);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (1, 'Envion', 150);

INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (2, 'Arranque', 180);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (2, 'Arranque', 150);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (2, 'Arranque', 130);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (2, 'Envion', 140);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (2, 'Envion', 100);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (2, 'Envion', 60);

INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (3, 'Arranque', 100);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (3, 'Arranque', 90);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (3, 'Arranque', 80);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (3, 'Envion', 100);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (3, 'Envion', 110);
INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (3, 'Envion', 120);

-- Tabla de usuarios (para autenticación JWT)
CREATE TABLE dbo.tb_Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    ClaveHash VARBINARY(64) NOT NULL,
    ClaveSalt VARBINARY(128) NOT NULL
);

-- Índices
CREATE INDEX IDX_Intentos_idDeportista ON tb_Intentos (idDeportista);
go

-- Procedimiento para obtener los mejores intentos por atleta
CREATE PROCEDURE dbo.ObtenerMejoresIntentos
AS
BEGIN
    SELECT a.nombre_completo, p.iniciales AS Pais, 
           (SELECT MAX(Peso) FROM tb_Intentos WHERE idDeportista = a.idDeportista AND Tipo = 'Arranque') AS MejorArranque,
           (SELECT MAX(Peso) FROM tb_Intentos WHERE idDeportista = a.idDeportista AND Tipo = 'Envion') AS MejorEnvion
    FROM tb_Depostistas a
    JOIN tb_Paises p ON a.idPais = p.idPais;
END;
GO

-- Procedimiento para registrar un país
CREATE	PROCEDURE dbo.RegistrarPais
(
	@iniciales	varchar(5),
	@descripcion	varchar(20)
)
AS
BEGIN
	INSERT	INTO dbo.tb_Paises(iniciales, descripcion) values (@iniciales, @descripcion)
END
go

-- Procedimiento para registrar un deportista
CREATE	PROCEDURE dbo.RegistrarDepostista
(
	@nombre_completo	varchar(200),
	@idPais			int
)
AS
BEGIN
	INSERT	INTO dbo.tb_Depostistas(nombre_completo, idPais) VALUES (@nombre_completo, @idPais);
END
GO

-- Procedimiento para registrar un intento
CREATE PROCEDURE dbo.RegistrarIntento
    @idDeportista INT,
    @Tipo VARCHAR(10),
    @Peso INT
AS
BEGIN
    INSERT INTO dbo.tb_Intentos (idDeportista, Tipo, Peso) VALUES (@idDeportista, @Tipo, @Peso);
END;
GO

-- Procedimiento para revisar los intentos de los deportista
CREATE PROCEDURE dbo.IntentoDeportista
    @idDeportista INT
AS
BEGIN
	DECLARE	@nombre_completo varchar(200),
			@TotalIntentos	int

	SELECT	@nombre_completo = nombre_completo 
	  FROM	dbo.tb_Depostistas where idDeportista = @idDeportista

    SELECT	@TotalIntentos = COUNT(*) 
	  FROM	dbo.tb_Intentos 
	 WHERE	idDeportista = @idDeportista

	SELECT	'Total de Intentos del deportista ' + ISNULL(@nombre_completo, '') + ' es de: ' + CONVERT(varchar, ISNULL(@TotalIntentos, 0)) AS TotalIntentos
END;
GO

CREATE PROCEDURE ObtenerResultadosPaginados
    @Pagina INT,
    @TamanioPagina INT
AS
BEGIN
    SET NOCOUNT ON;

	  WITH	Arranque AS 
		 (
			SELECT	i.idDeportista, 
					i.Tipo, MAX(i.Peso) AS Peso
			  FROM	dbo.tb_Intentos i
			 WHERE	i.Tipo = 'Arranque'
			 GROUP	BY i.idDeportista, i.Tipo
		 ),
			Envion AS
		 (
			SELECT	i.idDeportista, 
					i.Tipo, MAX(i.Peso) AS Peso
			  FROM	dbo.tb_Intentos i
			 WHERE	i.Tipo = 'Envion'
			 GROUP	BY i.idDeportista, i.Tipo
		 ),
			Resultado AS
		 (
	SELECT	p.iniciales AS Pais, 
			d.nombre_completo as Nombre,
			a.Peso AS Arranque,
			E.Peso AS Envion,
			a.Peso + E.Peso AS TotalPeso,
			ROW_NUMBER() OVER (ORDER BY (a.Peso + E.Peso) DESC) AS RowNum
	  FROM	dbo.tb_Depostistas d
      JOIN	dbo.tb_Paises p ON d.idPais = p.idPais
	  JOIN	Arranque a on (d.idDeportista = a.idDeportista)
	  JOIN	Envion E on (d.idDeportista = E.idDeportista)
		 )
    SELECT 
			Pais, 
			Nombre, 
			Arranque, 
			Envion, 
			TotalPeso
      FROM	Resultado
     WHERE	RowNum BETWEEN ((@Pagina - 1) * @TamanioPagina + 1) AND (@Pagina * @TamanioPagina);
	 ;
END;
GO

-- Para crear la base de datos
CREATE DATABASE TheGoodTasteDB;
GO

USE TheGoodTasteDB;
GO

-- Para crear la tabla de roles
CREATE TABLE Roles(
	IdRol INT IDENTITY(1,1) PRIMARY KEY,
	Nombre VARCHAR(50) NOT NULL UNIQUE
);
GO

-- Insertar los tres roles (admin, gerente, vendedor)
INSERT INTO Roles (Nombre)  VALUES ('Admin'), ('Gerente'), ('Vendedor');
GO

-- Para crear la tabla de usuarios
CREATE TABLE Usuarios(
	IdUsuario INT IDENTITY (1,1) PRIMARY KEY,
	Username VARCHAR(50) NOT NULL UNIQUE,
	PasswordHash VARCHAR(256) NOT NULL,
	NombreCompleto VARCHAR(100) NOT NULL,
	IdRol INT NOT NULL,
	Activo BIT NOT NULL DEFAULT 1,
	CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);
GO

-- agregamos usuario admin inical, password admin123
INSERT INTO Usuarios (Username, PasswordHash, NombreCompleto, IdRol, Activo)
VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Administrador General', 1, 1);
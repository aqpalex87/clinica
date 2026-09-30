IF OBJECT_ID(N'dbo.Pacientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pacientes
    (
        IdPaciente INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Pacientes PRIMARY KEY,
        IdEmpresa INT NOT NULL,
        IdSede INT NOT NULL,
        Codigo VARCHAR(20) NOT NULL,
        TipoDocumento VARCHAR(10) NOT NULL,
        NumeroDocumento VARCHAR(20) NOT NULL,
        ApellidoPaterno VARCHAR(80) NOT NULL,
        ApellidoMaterno VARCHAR(80) NULL,
        Nombres VARCHAR(120) NOT NULL,
        FechaNacimiento DATE NOT NULL,
        Sexo VARCHAR(1) NOT NULL,
        Celular VARCHAR(9) NOT NULL,
        Correo VARCHAR(120) NULL,
        Direccion VARCHAR(200) NULL,
        Observacion VARCHAR(300) NULL,
        Activo BIT NOT NULL CONSTRAINT DF_Pacientes_Activo DEFAULT (1),
        CONSTRAINT FK_Pacientes_Empresa FOREIGN KEY (IdEmpresa) REFERENCES dbo.Empresas (IdEmpresa),
        CONSTRAINT FK_Pacientes_Sede FOREIGN KEY (IdSede) REFERENCES dbo.Sedes (IdSede),
        CONSTRAINT CK_Pacientes_Sexo CHECK (Sexo IN ('M', 'F')),
        CONSTRAINT CK_Pacientes_TipoDocumento CHECK (TipoDocumento IN ('DNI', 'CE', 'PAS', 'CPP')),
        CONSTRAINT UQ_Pacientes_Documento UNIQUE (IdEmpresa, TipoDocumento, NumeroDocumento),
        CONSTRAINT UQ_Pacientes_Codigo UNIQUE (Codigo)
    );

    CREATE INDEX IX_Pacientes_Nombre
        ON dbo.Pacientes (IdEmpresa, ApellidoPaterno, ApellidoMaterno, Nombres);

    CREATE INDEX IX_Pacientes_Celular
        ON dbo.Pacientes (IdEmpresa, Celular);
END

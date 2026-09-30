/* Incluye menús padre (ancestros) aunque no estén en RolMenu, para que la jerarquía IdPadre funcione. */
ALTER PROCEDURE [dbo].[MC_SELECT_MENUS_POR_USUARIO]
    @Usuario VARCHAR(50),
    @IdEmpresa INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Asignados AS (
        SELECT DISTINCT rm.IdMenu
        FROM dbo.Usuarios u
        INNER JOIN dbo.RolMenu rm ON rm.IdRol = u.IdRol
        WHERE u.Usuario = @Usuario
          AND (@IdEmpresa IS NULL OR u.IdEmpresa = @IdEmpresa)
          AND u.Activo = 1
    ),
    Jerarquia AS (
        SELECT m.IdMenu, m.IdPadre
        FROM dbo.Menus m
        INNER JOIN Asignados a ON a.IdMenu = m.IdMenu
        WHERE m.Activo = 1

        UNION ALL

        SELECT p.IdMenu, p.IdPadre
        FROM dbo.Menus p
        INNER JOIN Jerarquia j ON p.IdMenu = j.IdPadre
        WHERE p.Activo = 1
    )
    SELECT DISTINCT
        m.IdMenu,
        m.Nombre,
        m.Ruta,
        m.Icono,
        m.IdPadre,
        m.Orden,
        m.Activo
    FROM dbo.Menus m
    INNER JOIN Jerarquia j ON j.IdMenu = m.IdMenu
    --ORDER BY ISNULL(m.IdPadre, 0), ISNULL(m.Orden, 0), m.IdMenu
    OPTION (MAXRECURSION 100);
END

select * from Universidad

INSERT INTO Universidad
VALUES
    (N'UNIVERSIDAD NACIONAL DE CAJAMARCA', 1),
    (N'UNIVERSIDAD NACIONAL DE CHOTA', 1),
    (N'UNIVERSIDAD NACIONAL DE INGENIERÍA', 1),
    (N'UNIVERSIDAD NACIONAL DE PIURA', 1),
    (N'UNIVERSIDAD NACIONAL DE SAN AGUSTÍN DE AREQUIPA', 1),
    (N'UNIVERSIDAD NACIONAL DE TRUJILLO', 1),
    (N'UNIVERSIDAD NACIONAL DEL ALTIPLANO', 1),
    (N'UNIVERSIDAD NACIONAL JORGE BASADRE GROHMANN', 1),
    (N'UNIVERSIDAD NACIONAL MAYOR DE SAN MARCOS', 1),
    ( N'UNIVERSIDAD NACIONAL PEDRO RUIZ GALLO', 1),
    ( N'UNIVERSIDAD NACIONAL SAN LUIS GONZAGA', 1),
    ( N'UNIVERSIDAD PRIVADA DE TACNA', 1);


insert into UniversidadDetalle 
values 
    (4,N'ADMINISTRACIÓN', 348.910, 201.576,2026),
    (4,N'AGRONOMÍA', 373.244, 255.672,2026),
    (4,N'BIOTECNOLOGÍA', 390.456, 298.564,2026),
    (4,N'CONTABILIDAD', 284.754, 210.094,2026),
    (4,N'DERECHO', 412.992, 293.800,2026),
    (4,N'ECONOMÍA', 330.584, 200.572,2026),
    (4,N'EDUCACION LENGUAJE Y LITERATURA', 341.842, 212.874,2026),
    (4,N'EDUCACION MATEMÁTICA Y FÍSICA', 314.178, 207.918,2026),
    (4,N'ENFERMERÍA', 373.244, 251.820,2026),
    (4,N'INGENIERÍA AMBIENTAL', 273.124, 190.425,2026),
    (4,N'INGENIERÍA CIVIL', 432.848, 346.576,2026),
    (4,N'INGENIERÍA DE MINAS', 356.508, 255.672,2026),
    (4,N'INGENIERÍA DE SISTEMAS', 363.294, 248.006,2026),
    (4,N'INGENIERÍA FORESTAL', 244.040, 150.446,2026),
    (4,N'INGENIERÍA GEOLÓGICA', 343.804, 237.828,2026),
    (4,N'INGENIERÍA HIDRÁULICA', 290.452, 200.100,2026),
    (4,N'MEDICINA HUMANA', 506.488, 424.004,2026),
    (4,N'MEDICINA VETERINARIA', 264.546, 179.492,2026),
    (4,N'OBSTETRICIA', 395.872, 266.496,2026),
    (4,N'SOCIOLOGÍA', 309.362, 193.364,2026),
    (4,N'TURISMO Y HOTELERÍA', 274.624, 168.906,2026);


	alter table apoderado add IdParamParentesco int not null


CREATE TABLE ParametroTipo (
    IdParametroTipo INT IDENTITY(1,1) PRIMARY KEY,
    Codigo VARCHAR(50) NOT NULL UNIQUE, -- SEXO, PARENTESCO
    Nombre VARCHAR(100) NOT NULL,
    Activo BIT DEFAULT 1
);


CREATE TABLE Parametro (
    IdParametro INT IDENTITY(1,1) PRIMARY KEY,
    IdParametroTipo INT NOT NULL,
    Codigo VARCHAR(50) NOT NULL, -- M, F, PADRE, MADRE
    Nombre VARCHAR(100) NOT NULL, -- Masculino, Femenino, Padre
    ValorExtra VARCHAR(100) NULL, -- opcional
    Orden INT NULL,
    Activo BIT DEFAULT 1,

    CONSTRAINT FK_Parametro_Tipo
        FOREIGN KEY (IdParametroTipo) REFERENCES ParametroTipo(IdParametroTipo)
);


INSERT INTO ParametroTipo (Codigo, Nombre) VALUES
('SEXO', 'Sexo'),
('PARENTESCO', 'Parentesco');

-- SEXO
INSERT INTO Parametro (IdParametroTipo, Codigo, Nombre, Orden)
SELECT IdParametroTipo, 'M', 'Masculino', 1 FROM ParametroTipo WHERE Codigo = 'SEXO';

INSERT INTO Parametro (IdParametroTipo, Codigo, Nombre, Orden)
SELECT IdParametroTipo, 'F', 'Femenino', 2 FROM ParametroTipo WHERE Codigo = 'SEXO';

-- PARENTESCO
INSERT INTO Parametro (IdParametroTipo, Codigo, Nombre, Orden)
SELECT IdParametroTipo, 'PADRE', 'Padre', 1 FROM ParametroTipo WHERE Codigo = 'PARENTESCO';

INSERT INTO Parametro (IdParametroTipo, Codigo, Nombre, Orden)
SELECT IdParametroTipo, 'MADRE', 'Madre', 2 FROM ParametroTipo WHERE Codigo = 'PARENTESCO';

select * from menus
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Menus AS x
    WHERE LTRIM(RTRIM(UPPER(x.Nombre))) = N'MANTENIMIENTO CICLOS'
       OR LTRIM(RTRIM(ISNULL(x.Ruta, N''))) IN (N'/mantenimiento-parametros/ciclos', N'mantenimiento-parametros/ciclos')
)
BEGIN
    INSERT INTO dbo.Menus
    (
        Nombre,
        Ruta,
        Icono,
        IdPadre,
        Orden,
        Activo
    )
    VALUES
    (
        N'MANTENIMIENTO CICLOS',
        N'/mantenimiento-parametros/ciclos',
        N'event_note',
        11,
        70,
        1
    );
END;
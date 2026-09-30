IF NOT EXISTS (
    SELECT 1
    FROM dbo.Menus
    WHERE Nombre = 'Procesos'
      AND IdPadre IS NULL
      AND (Ruta IS NULL OR LTRIM(RTRIM(Ruta)) = '')
)
BEGIN
    INSERT INTO dbo.Menus (Nombre, Ruta, Icono, IdPadre, Orden, Activo)
    VALUES ('Procesos', NULL, 'science', NULL, 4, 1);
END

DECLARE @IdProcesos INT;

SELECT TOP 1 @IdProcesos = IdMenu
FROM dbo.Menus
WHERE Nombre = 'Procesos'
  AND IdPadre IS NULL
  AND (Ruta IS NULL OR LTRIM(RTRIM(Ruta)) = '')
ORDER BY IdMenu;

IF EXISTS (
    SELECT 1
    FROM dbo.Menus
    WHERE Ruta IN (
        'mantenimiento-parametros/pacientes',
        '/mantenimiento-parametros/pacientes',
        'procesos/pacientes',
        '/procesos/pacientes'
    )
)
BEGIN
    UPDATE dbo.Menus
    SET
        Nombre = 'Pacientes',
        Ruta = '/procesos/pacientes',
        Icono = 'people',
        IdPadre = @IdProcesos,
        Orden = 1,
        Activo = 1
    WHERE Ruta IN (
        'mantenimiento-parametros/pacientes',
        '/mantenimiento-parametros/pacientes',
        'procesos/pacientes',
        '/procesos/pacientes'
    );
END
ELSE
BEGIN
    INSERT INTO dbo.Menus (Nombre, Ruta, Icono, IdPadre, Orden, Activo)
    VALUES ('Pacientes', '/procesos/pacientes', 'people', @IdProcesos, 1, 1);
END

DECLARE @IdMenuPacientes INT;

SELECT TOP 1 @IdMenuPacientes = IdMenu
FROM dbo.Menus
WHERE Ruta = '/procesos/pacientes'
ORDER BY IdMenu;

INSERT INTO dbo.RolMenu (IdRol, IdMenu)
SELECT r.IdRol, @IdMenuPacientes
FROM dbo.Roles r
WHERE r.Activo = 1
  AND NOT EXISTS (
      SELECT 1
      FROM dbo.RolMenu rm
      WHERE rm.IdRol = r.IdRol
        AND rm.IdMenu = @IdMenuPacientes
  );

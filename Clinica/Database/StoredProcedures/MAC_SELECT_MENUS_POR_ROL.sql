CREATE PROCEDURE [dbo].[MC_SELECT_MENUS_POR_ROL]
    @IdRol INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.IdMenu,
        m.Nombre,
        m.Ruta,
        m.Icono,
        m.IdPadre,
        m.Orden,
        m.Activo,
        CAST(CASE WHEN rm.IdMenu IS NULL THEN 0 ELSE 1 END AS BIT) AS Seleccionado
    FROM dbo.Menus m
    LEFT JOIN dbo.RolMenu rm
        ON rm.IdMenu = m.IdMenu
       AND rm.IdRol = @IdRol
    WHERE m.Activo = 1
    ORDER BY ISNULL(m.IdPadre, 0), ISNULL(m.Orden, 0), m.IdMenu;
END

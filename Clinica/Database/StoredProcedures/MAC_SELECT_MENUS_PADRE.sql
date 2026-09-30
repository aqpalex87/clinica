CREATE PROCEDURE [dbo].[MC_SELECT_MENUS_PADRE]
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
        m.Activo
    FROM dbo.Menus m
    WHERE m.Activo = 1
    ORDER BY ISNULL(m.Orden, 999999), m.IdMenu;
END

CREATE PROCEDURE [dbo].[MC_SELECT_MENUS_PAGINADO]
    @P_NOMBRE VARCHAR(100) = '',
    @P_PAGENUMBER INT = 1,
    @P_PAGESIZE INT = 10,
    @P_TOTALROWS INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NombreFiltro VARCHAR(100) = LTRIM(RTRIM(ISNULL(@P_NOMBRE, '')));
    DECLARE @Offset INT = (@P_PAGENUMBER - 1) * @P_PAGESIZE;

    SELECT @P_TOTALROWS = COUNT(1)
    FROM dbo.Menus m
    WHERE (@NombreFiltro = '' OR m.Nombre LIKE '%' + @NombreFiltro + '%');

    SELECT
        m.IdMenu,
        m.Nombre,
        m.Ruta,
        m.Icono,
        m.IdPadre,
        p.Nombre AS NombrePadre,
        m.Orden,
        m.Activo
    FROM dbo.Menus m
    LEFT JOIN dbo.Menus p ON p.IdMenu = m.IdPadre
    WHERE (@NombreFiltro = '' OR m.Nombre LIKE '%' + @NombreFiltro + '%')
    ORDER BY ISNULL(m.Orden, 999999), m.IdMenu
    OFFSET @Offset ROWS FETCH NEXT @P_PAGESIZE ROWS ONLY;
END

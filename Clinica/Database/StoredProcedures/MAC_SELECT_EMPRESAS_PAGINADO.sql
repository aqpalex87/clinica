CREATE PROCEDURE [dbo].[MC_SELECT_EMPRESAS_PAGINADO]
    @P_NOMBRE VARCHAR(150) = '',
    @P_PAGENUMBER INT = 1,
    @P_PAGESIZE INT = 10,
    @P_TOTALROWS INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NombreFiltro VARCHAR(150) = LTRIM(RTRIM(ISNULL(@P_NOMBRE, '')));
    DECLARE @Offset INT = (@P_PAGENUMBER - 1) * @P_PAGESIZE;

    SELECT @P_TOTALROWS = COUNT(1)
    FROM dbo.Empresas e
    WHERE (@NombreFiltro = '' OR e.Nombre LIKE '%' + @NombreFiltro + '%');

    SELECT
        e.IdEmpresa,
        e.Nombre,
        e.Ruc,
        e.Activo
    FROM dbo.Empresas e
    WHERE (@NombreFiltro = '' OR e.Nombre LIKE '%' + @NombreFiltro + '%')
    ORDER BY e.Nombre
    OFFSET @Offset ROWS FETCH NEXT @P_PAGESIZE ROWS ONLY;
END

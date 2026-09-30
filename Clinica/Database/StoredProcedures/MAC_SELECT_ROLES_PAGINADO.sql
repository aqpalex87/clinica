CREATE PROCEDURE [dbo].[MC_SELECT_ROLES_PAGINADO]
    @IdEmpresa INT = NULL,
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
    FROM dbo.Roles r
    INNER JOIN dbo.Empresas e ON e.IdEmpresa = r.IdEmpresa
    WHERE (@IdEmpresa IS NULL OR r.IdEmpresa = @IdEmpresa)
      AND (@NombreFiltro = '' OR r.Nombre LIKE '%' + @NombreFiltro + '%');

    SELECT
        r.IdRol,
        r.IdEmpresa,
        e.Nombre AS NombreEmpresa,
        r.Nombre,
        r.Descripcion,
        r.Activo
    FROM dbo.Roles r
    INNER JOIN dbo.Empresas e ON e.IdEmpresa = r.IdEmpresa
    WHERE (@IdEmpresa IS NULL OR r.IdEmpresa = @IdEmpresa)
      AND (@NombreFiltro = '' OR r.Nombre LIKE '%' + @NombreFiltro + '%')
    ORDER BY r.Nombre
    OFFSET @Offset ROWS FETCH NEXT @P_PAGESIZE ROWS ONLY;
END

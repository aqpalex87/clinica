CREATE OR ALTER PROCEDURE [dbo].[MC_SELECT_SEDES_PAGINADO]
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
    FROM dbo.Sedes s
    INNER JOIN dbo.Empresas e ON e.IdEmpresa = s.IdEmpresa
    WHERE (@IdEmpresa IS NULL OR s.IdEmpresa = @IdEmpresa)
      AND (@NombreFiltro = '' OR s.Nombre LIKE '%' + @NombreFiltro + '%');

    SELECT
        s.IdSede,
        s.IdEmpresa,
        e.Nombre AS NombreEmpresa,
        s.Nombre,
        s.Codigo,
        s.Direccion,
        s.Activo
    FROM dbo.Sedes s
    INNER JOIN dbo.Empresas e ON e.IdEmpresa = s.IdEmpresa
    WHERE (@IdEmpresa IS NULL OR s.IdEmpresa = @IdEmpresa)
      AND (@NombreFiltro = '' OR s.Nombre LIKE '%' + @NombreFiltro + '%')
    ORDER BY s.Nombre
    OFFSET @Offset ROWS FETCH NEXT @P_PAGESIZE ROWS ONLY;
END

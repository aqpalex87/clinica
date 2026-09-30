CREATE PROCEDURE [dbo].[MC_SELECT_USUARIOS_PAGINADO]
    @IdEmpresa INT = NULL,
    @P_USUARIO VARCHAR(50) = '',
    @P_PAGENUMBER INT = 1,
    @P_PAGESIZE INT = 10,
    @P_TOTALROWS INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UsuarioFiltro VARCHAR(50) = LTRIM(RTRIM(ISNULL(@P_USUARIO, '')));
    DECLARE @Offset INT = (@P_PAGENUMBER - 1) * @P_PAGESIZE;

    SELECT @P_TOTALROWS = COUNT(1)
    FROM dbo.Usuarios u
    INNER JOIN dbo.Empresas e ON e.IdEmpresa = u.IdEmpresa
    INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
    WHERE (@IdEmpresa IS NULL OR u.IdEmpresa = @IdEmpresa)
      AND (@UsuarioFiltro = '' OR u.Usuario LIKE '%' + @UsuarioFiltro + '%');

    SELECT
        u.IdUsuario,
        u.Usuario AS UsuarioLogin,
        u.IdEmpresa,
        e.Nombre AS NombreEmpresa,
        u.IdRol,
        r.Nombre AS NombreRol,
        u.Activo
    FROM dbo.Usuarios u
    INNER JOIN dbo.Empresas e ON e.IdEmpresa = u.IdEmpresa
    INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
    WHERE (@IdEmpresa IS NULL OR u.IdEmpresa = @IdEmpresa)
      AND (@UsuarioFiltro = '' OR u.Usuario LIKE '%' + @UsuarioFiltro + '%')
    ORDER BY u.Usuario
    OFFSET @Offset ROWS FETCH NEXT @P_PAGESIZE ROWS ONLY;
END

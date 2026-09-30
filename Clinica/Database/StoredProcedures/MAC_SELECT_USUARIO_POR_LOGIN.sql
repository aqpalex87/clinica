CREATE OR ALTER PROCEDURE [dbo].[MC_SELECT_USUARIO_POR_LOGIN]
    @Usuario VARCHAR(50),
    @IdEmpresa INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        u.IdUsuario,
        u.Usuario,
        u.PasswordHash,
        u.IdEmpresa,
        e.Nombre AS EmpresaNombre,
        CAST(NULL AS INT) AS IdSede,
        CAST(NULL AS VARCHAR(150)) AS SedeNombre,
        u.IdRol,
        r.Nombre AS RolNombre,
        u.Activo
    FROM dbo.Usuarios u
    INNER JOIN dbo.Empresas e ON e.IdEmpresa = u.IdEmpresa
    INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
    WHERE u.Usuario = @Usuario
      AND (@IdEmpresa IS NULL OR u.IdEmpresa = @IdEmpresa);
END

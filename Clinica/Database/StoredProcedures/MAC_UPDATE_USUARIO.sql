CREATE PROCEDURE [dbo].[MC_UPDATE_USUARIO]
    @IdUsuario INT,
    @Usuario VARCHAR(50),
    @PasswordHash VARCHAR(256) = NULL,
    @IdEmpresa INT,
    @IdRol INT,
    @Activo BIT = 1
AS
BEGIN


    UPDATE dbo.Usuarios
    SET
        Usuario = LTRIM(RTRIM(@Usuario)),
        PasswordHash = CASE
            WHEN @PasswordHash IS NULL OR LTRIM(RTRIM(@PasswordHash)) = '' THEN PasswordHash
            ELSE @PasswordHash
        END,
        IdEmpresa = @IdEmpresa,
        IdRol = @IdRol,
        Activo = @Activo
    WHERE IdUsuario = @IdUsuario;
END

CREATE PROCEDURE [dbo].[MC_INSERT_USUARIO]
    @Usuario VARCHAR(50),
    @PasswordHash VARCHAR(256),
    @IdEmpresa INT,
    @IdRol INT,
    @Activo BIT = 1,
    @IdUsuario INT OUTPUT
AS
BEGIN


    INSERT INTO dbo.Usuarios (Usuario, PasswordHash, IdEmpresa, IdRol, Activo)
    VALUES (
        LTRIM(RTRIM(@Usuario)),
        @PasswordHash,
        @IdEmpresa,
        @IdRol,
        @Activo
    );

    SET @IdUsuario = SCOPE_IDENTITY();
END

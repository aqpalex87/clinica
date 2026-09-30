CREATE PROCEDURE [dbo].[MC_DELETE_USUARIO]
    @IdUsuario INT
AS
BEGIN

    DELETE FROM dbo.Usuarios
    WHERE IdUsuario = @IdUsuario;
END

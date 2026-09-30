CREATE PROCEDURE [dbo].[MC_UPDATE_ROL]
    @IdRol INT,
    @IdEmpresa INT,
    @Nombre VARCHAR(150),
    @Descripcion VARCHAR(300) = NULL,
    @Activo BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Roles
    SET
        IdEmpresa = @IdEmpresa,
        Nombre = LTRIM(RTRIM(@Nombre)),
        Descripcion = NULLIF(LTRIM(RTRIM(ISNULL(@Descripcion, ''))), ''),
        Activo = @Activo
    WHERE IdRol = @IdRol;
END

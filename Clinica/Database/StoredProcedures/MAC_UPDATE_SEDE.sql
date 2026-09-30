CREATE PROCEDURE [dbo].[MC_UPDATE_SEDE]
    @IdSede INT,
    @IdEmpresa INT,
    @Nombre VARCHAR(150),
    @Codigo VARCHAR(30) = NULL,
    @Direccion VARCHAR(250) = NULL,
    @Activo BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Sedes
    SET
        IdEmpresa = @IdEmpresa,
        Nombre = LTRIM(RTRIM(@Nombre)),
        Codigo = NULLIF(LTRIM(RTRIM(ISNULL(@Codigo, ''))), ''),
        Direccion = NULLIF(LTRIM(RTRIM(ISNULL(@Direccion, ''))), ''),
        Activo = @Activo
    WHERE IdSede = @IdSede;
END

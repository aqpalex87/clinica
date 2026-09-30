CREATE OR ALTER PROCEDURE [dbo].[MC_UPDATE_EMPRESA]
    @IdEmpresa INT,
    @Nombre VARCHAR(150),
    @Ruc VARCHAR(20) = NULL,
    @Activo BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Empresas
    SET
        Nombre = LTRIM(RTRIM(@Nombre)),
        Ruc = NULLIF(LTRIM(RTRIM(ISNULL(@Ruc, ''))), ''),
        Activo = @Activo
    WHERE IdEmpresa = @IdEmpresa;
END

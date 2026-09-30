CREATE PROCEDURE [dbo].[MC_INSERT_EMPRESA]
    @Nombre VARCHAR(150),
    @Ruc VARCHAR(20) = NULL,
    @Activo BIT = 1,
    @IdEmpresa INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Empresas
    (
        Nombre,
        Ruc,
        Activo
    )
    VALUES
    (
        LTRIM(RTRIM(@Nombre)),
        NULLIF(LTRIM(RTRIM(ISNULL(@Ruc, ''))), ''),
        @Activo
    );

    SET @IdEmpresa = SCOPE_IDENTITY();
END

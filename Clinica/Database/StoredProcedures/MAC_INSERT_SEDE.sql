CREATE PROCEDURE [dbo].[MC_INSERT_SEDE]
    @IdEmpresa INT,
    @Nombre VARCHAR(150),
    @Codigo VARCHAR(30) = NULL,
    @Direccion VARCHAR(250) = NULL,
    @Activo BIT = 1,
    @IdSede INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Sedes
    (
        IdEmpresa,
        Nombre,
        Codigo,
        Direccion,
        Activo
    )
    VALUES
    (
        @IdEmpresa,
        LTRIM(RTRIM(@Nombre)),
        NULLIF(LTRIM(RTRIM(ISNULL(@Codigo, ''))), ''),
        NULLIF(LTRIM(RTRIM(ISNULL(@Direccion, ''))), ''),
        @Activo
    );

    SET @IdSede = SCOPE_IDENTITY();
END

CREATE PROCEDURE [dbo].[MC_INSERT_ROL]
    @IdEmpresa INT,
    @Nombre VARCHAR(150),
    @Descripcion VARCHAR(300) = NULL,
    @Activo BIT = 1,
    @IdRol INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Roles (IdEmpresa, Nombre, Descripcion, Activo)
    VALUES (
        @IdEmpresa,
        LTRIM(RTRIM(@Nombre)),
        NULLIF(LTRIM(RTRIM(ISNULL(@Descripcion, ''))), ''),
        @Activo
    );

    SET @IdRol = SCOPE_IDENTITY();
END

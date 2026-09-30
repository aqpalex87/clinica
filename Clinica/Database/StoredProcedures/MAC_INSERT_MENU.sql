CREATE PROCEDURE [dbo].[MC_INSERT_MENU]
    @P_NOMBRE VARCHAR(100),
    @P_RUTA VARCHAR(200) = NULL,
    @P_ICONO VARCHAR(50) = NULL,
    @P_IDPADRE INT = NULL,
    @P_ORDEN INT = NULL,
    @P_ACTIVO BIT = 1,
    @P_IDMENU INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Menus
    (
        Nombre,
        Ruta,
        Icono,
        IdPadre,
        Orden,
        Activo
    )
    VALUES
    (
        LTRIM(RTRIM(@P_NOMBRE)),
        NULLIF(LTRIM(RTRIM(ISNULL(@P_RUTA, ''))), ''),
        NULLIF(LTRIM(RTRIM(ISNULL(@P_ICONO, ''))), ''),
        @P_IDPADRE,
        @P_ORDEN,
        @P_ACTIVO
    );

    SET @P_IDMENU = SCOPE_IDENTITY();
END

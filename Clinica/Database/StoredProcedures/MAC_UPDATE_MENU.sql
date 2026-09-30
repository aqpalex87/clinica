CREATE PROCEDURE [dbo].[MC_UPDATE_MENU]
    @P_IDMENU INT,
    @P_NOMBRE VARCHAR(100),
    @P_RUTA VARCHAR(200) = NULL,
    @P_ICONO VARCHAR(50) = NULL,
    @P_IDPADRE INT = NULL,
    @P_ORDEN INT = NULL,
    @P_ACTIVO BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Menus
    SET
        Nombre = LTRIM(RTRIM(@P_NOMBRE)),
        Ruta = NULLIF(LTRIM(RTRIM(ISNULL(@P_RUTA, ''))), ''),
        Icono = NULLIF(LTRIM(RTRIM(ISNULL(@P_ICONO, ''))), ''),
        IdPadre = @P_IDPADRE,
        Orden = @P_ORDEN,
        Activo = @P_ACTIVO
    WHERE IdMenu = @P_IDMENU;
END

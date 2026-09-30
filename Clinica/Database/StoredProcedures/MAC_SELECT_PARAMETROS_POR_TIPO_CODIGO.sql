CREATE PROCEDURE [dbo].[MC_SELECT_PARAMETROS_POR_TIPO_CODIGO]
    @CodigoTipo VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.IdParametro,
        p.Codigo,
        p.Nombre,
        p.Orden
    FROM dbo.Parametro AS p
    INNER JOIN dbo.ParametroTipo AS t ON t.IdParametroTipo = p.IdParametroTipo
    WHERE UPPER(LTRIM(RTRIM(t.Codigo))) = UPPER(LTRIM(RTRIM(@CodigoTipo)))
      AND ISNULL(p.Activo, 1) = 1
      AND ISNULL(t.Activo, 1) = 1
    ORDER BY ISNULL(p.Orden, 999999), p.IdParametro;
END

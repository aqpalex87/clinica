CREATE PROCEDURE [dbo].[MC_SELECT_EMPRESAS_ACTIVAS]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.IdEmpresa,
        e.Nombre
    FROM dbo.Empresas e
    WHERE e.Activo = 1
    ORDER BY e.Nombre;
END

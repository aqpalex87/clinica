CREATE OR ALTER PROCEDURE [dbo].[MC_SELECT_SEDES_POR_EMPRESA]
    @IdEmpresa INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.IdSede,
        s.IdEmpresa,
        s.Nombre
    FROM dbo.Sedes s
    WHERE s.IdEmpresa = @IdEmpresa
      AND s.Activo = 1
    ORDER BY s.Nombre;
END


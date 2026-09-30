CREATE PROCEDURE [dbo].[MC_DELETE_EMPRESA]
    @IdEmpresa INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Empresas
    SET Activo = 0
    WHERE IdEmpresa = @IdEmpresa;
END

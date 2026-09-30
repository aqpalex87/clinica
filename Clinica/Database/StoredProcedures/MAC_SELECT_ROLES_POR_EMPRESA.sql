CREATE PROCEDURE [dbo].[MC_SELECT_ROLES_POR_EMPRESA]
    @IdEmpresa INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.IdRol,
        r.Nombre
    FROM dbo.Roles r
    WHERE r.IdEmpresa = @IdEmpresa
      AND r.Activo = 1
    ORDER BY r.Nombre;
END

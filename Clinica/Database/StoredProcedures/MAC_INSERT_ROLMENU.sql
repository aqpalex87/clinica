CREATE PROCEDURE [dbo].[MC_INSERT_ROLMENU]
    @P_IDROL INT,
    @P_IDMENU INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.RolMenu rm
        WHERE rm.IdRol = @P_IDROL
          AND rm.IdMenu = @P_IDMENU
    )
    BEGIN
        INSERT INTO dbo.RolMenu (IdRol, IdMenu)
        VALUES (@P_IDROL, @P_IDMENU);
    END
END

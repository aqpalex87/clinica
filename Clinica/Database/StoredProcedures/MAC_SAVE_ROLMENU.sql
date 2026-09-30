ALTER PROCEDURE [dbo].[MC_SAVE_ROLMENU]
    @IdRol INT,
    @IdsMenu VARCHAR(MAX) = ''
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRAN;
    BEGIN TRY
        DELETE FROM dbo.RolMenu
        WHERE IdRol = @IdRol;

        IF (ISNULL(LTRIM(RTRIM(@IdsMenu)), '') <> '')
        BEGIN
            DECLARE @Xml XML;
            SET @Xml = CAST('<i>' + REPLACE(@IdsMenu, ',', '</i><i>') + '</i>' AS XML);

            INSERT INTO dbo.RolMenu (IdRol, IdMenu)
            SELECT DISTINCT
                @IdRol,
                CAST(LTRIM(RTRIM(T.N.value('.', 'VARCHAR(20)'))) AS INT)
            FROM @Xml.nodes('/i') AS T(N)
            WHERE ISNUMERIC(LTRIM(RTRIM(T.N.value('.', 'VARCHAR(20)')))) = 1;
        END

        COMMIT TRAN;
        SELECT 1 AS Resultado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        SELECT 0 AS Resultado;
    END CATCH
END

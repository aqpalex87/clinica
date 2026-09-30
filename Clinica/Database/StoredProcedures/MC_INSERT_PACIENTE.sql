IF OBJECT_ID(N'dbo.MC_INSERT_PACIENTE', N'P') IS NOT NULL
    DROP PROCEDURE dbo.MC_INSERT_PACIENTE;
GO
CREATE PROCEDURE [dbo].[MC_INSERT_PACIENTE]
    @IdEmpresa INT,
    @IdSede INT,
    @TipoDocumento VARCHAR(10),
    @NumeroDocumento VARCHAR(20),
    @ApellidoPaterno VARCHAR(80),
    @ApellidoMaterno VARCHAR(80) = NULL,
    @Nombres VARCHAR(120),
    @FechaNacimiento DATE,
    @Sexo VARCHAR(1),
    @Celular VARCHAR(9),
    @Correo VARCHAR(120) = NULL,
    @Direccion VARCHAR(200) = NULL,
    @Observacion VARCHAR(300) = NULL,
    @Activo BIT = 1,
    @IdPaciente INT OUTPUT,
    @Codigo VARCHAR(20) OUTPUT,
    @Resultado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SET @IdPaciente = 0;
    SET @Codigo = NULL;
    SET @Resultado = 0;

    SET @TipoDocumento = UPPER(LTRIM(RTRIM(@TipoDocumento)));
    SET @NumeroDocumento = UPPER(LTRIM(RTRIM(@NumeroDocumento)));
    SET @ApellidoPaterno = LTRIM(RTRIM(@ApellidoPaterno));
    SET @ApellidoMaterno = NULLIF(LTRIM(RTRIM(ISNULL(@ApellidoMaterno, ''))), '');
    SET @Nombres = LTRIM(RTRIM(@Nombres));
    SET @Sexo = UPPER(LTRIM(RTRIM(@Sexo)));
    SET @Celular = LTRIM(RTRIM(@Celular));
    SET @Correo = NULLIF(LTRIM(RTRIM(ISNULL(@Correo, ''))), '');
    SET @Direccion = NULLIF(LTRIM(RTRIM(ISNULL(@Direccion, ''))), '');
    SET @Observacion = NULLIF(LTRIM(RTRIM(ISNULL(@Observacion, ''))), '');

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Sedes
        WHERE IdSede = @IdSede
          AND IdEmpresa = @IdEmpresa
    )
    BEGIN
        SET @Resultado = -2;
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.Pacientes
        WHERE IdEmpresa = @IdEmpresa
          AND TipoDocumento = @TipoDocumento
          AND NumeroDocumento = @NumeroDocumento
    )
    BEGIN
        SET @Resultado = -1;
        RETURN;
    END

    DECLARE @CodigoTemporal VARCHAR(20) = 'T' + LEFT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 12);

    INSERT INTO dbo.Pacientes
        (
            IdEmpresa,
            IdSede,
            Codigo,
            TipoDocumento,
            NumeroDocumento,
            ApellidoPaterno,
            ApellidoMaterno,
            Nombres,
            FechaNacimiento,
            Sexo,
            Celular,
            Correo,
            Direccion,
            Observacion,
            Activo
        )
        VALUES
        (
            @IdEmpresa,
            @IdSede,
            @CodigoTemporal,
            @TipoDocumento,
            @NumeroDocumento,
            @ApellidoPaterno,
            @ApellidoMaterno,
            @Nombres,
            @FechaNacimiento,
            @Sexo,
            @Celular,
            @Correo,
            @Direccion,
            @Observacion,
            @Activo
        );

        SET @IdPaciente = SCOPE_IDENTITY();
        SET @Codigo = 'P' + RIGHT('000000' + CAST(@IdPaciente AS VARCHAR(6)), 6);

        UPDATE dbo.Pacientes
        SET Codigo = @Codigo
        WHERE IdPaciente = @IdPaciente;

    SET @Resultado = 1;
END

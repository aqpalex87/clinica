IF OBJECT_ID(N'dbo.MC_UPDATE_PACIENTE', N'P') IS NOT NULL
    DROP PROCEDURE dbo.MC_UPDATE_PACIENTE;
GO
CREATE PROCEDURE [dbo].[MC_UPDATE_PACIENTE]
    @IdPaciente INT,
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
    @Resultado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

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

    IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE IdPaciente = @IdPaciente)
    BEGIN
        SET @Resultado = 0;
        RETURN;
    END

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
          AND IdPaciente <> @IdPaciente
    )
    BEGIN
        SET @Resultado = -1;
        RETURN;
    END

    UPDATE dbo.Pacientes
        SET
            IdEmpresa = @IdEmpresa,
            IdSede = @IdSede,
            TipoDocumento = @TipoDocumento,
            NumeroDocumento = @NumeroDocumento,
            ApellidoPaterno = @ApellidoPaterno,
            ApellidoMaterno = @ApellidoMaterno,
            Nombres = @Nombres,
            FechaNacimiento = @FechaNacimiento,
            Sexo = @Sexo,
            Celular = @Celular,
            Correo = @Correo,
            Direccion = @Direccion,
            Observacion = @Observacion,
            Activo = @Activo
        WHERE IdPaciente = @IdPaciente;

    SET @Resultado = 1;
END

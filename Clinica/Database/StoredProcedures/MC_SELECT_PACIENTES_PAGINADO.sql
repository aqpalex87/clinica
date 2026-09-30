IF OBJECT_ID(N'dbo.MC_SELECT_PACIENTES_PAGINADO', N'P') IS NOT NULL
    DROP PROCEDURE dbo.MC_SELECT_PACIENTES_PAGINADO;
GO
CREATE PROCEDURE [dbo].[MC_SELECT_PACIENTES_PAGINADO]
    @IdEmpresa INT = NULL,
    @P_NOMBRE VARCHAR(150) = '',
    @P_DOCUMENTO VARCHAR(20) = '',
    @P_CONTACTO VARCHAR(120) = '',
    @P_PAGENUMBER INT = 1,
    @P_PAGESIZE INT = 10,
    @P_TOTALROWS INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Nombre VARCHAR(150) = LTRIM(RTRIM(ISNULL(@P_NOMBRE, '')));
    DECLARE @Documento VARCHAR(20) = LTRIM(RTRIM(ISNULL(@P_DOCUMENTO, '')));
    DECLARE @Contacto VARCHAR(120) = LTRIM(RTRIM(ISNULL(@P_CONTACTO, '')));
    DECLARE @Offset INT = (@P_PAGENUMBER - 1) * @P_PAGESIZE;
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

    SELECT @P_TOTALROWS = COUNT(1)
    FROM dbo.Pacientes p
    WHERE (@IdEmpresa IS NULL OR p.IdEmpresa = @IdEmpresa)
      AND (
            @Nombre = ''
            OR p.Nombres LIKE '%' + @Nombre + '%'
            OR p.ApellidoPaterno LIKE '%' + @Nombre + '%'
            OR ISNULL(p.ApellidoMaterno, '') LIKE '%' + @Nombre + '%'
            OR (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno, '') + ' ' + p.Nombres) LIKE '%' + @Nombre + '%'
          )
      AND (@Documento = '' OR p.NumeroDocumento LIKE '%' + @Documento + '%')
      AND (
            @Contacto = ''
            OR p.Celular LIKE '%' + @Contacto + '%'
            OR ISNULL(p.Correo, '') LIKE '%' + @Contacto + '%'
          );

    SELECT
        p.IdPaciente,
        p.IdEmpresa,
        e.Nombre AS NombreEmpresa,
        p.IdSede,
        s.Nombre AS NombreSede,
        p.Codigo,
        p.TipoDocumento,
        p.NumeroDocumento,
        p.ApellidoPaterno,
        p.ApellidoMaterno,
        p.Nombres,
        LTRIM(RTRIM(
            p.ApellidoPaterno + ' ' +
            CASE
                WHEN NULLIF(LTRIM(RTRIM(ISNULL(p.ApellidoMaterno, ''))), '') IS NULL THEN ''
                ELSE LTRIM(RTRIM(p.ApellidoMaterno)) + ' '
            END +
            p.Nombres
        )) AS NombreCompleto,
        p.FechaNacimiento,
        (
            DATEDIFF(YEAR, p.FechaNacimiento, @Hoy)
            - CASE
                WHEN DATEADD(YEAR, DATEDIFF(YEAR, p.FechaNacimiento, @Hoy), p.FechaNacimiento) > @Hoy THEN 1
                ELSE 0
              END
        ) AS Edad,
        p.Sexo,
        p.Celular,
        p.Correo,
        p.Direccion,
        p.Observacion,
        p.Activo
    FROM dbo.Pacientes p
    INNER JOIN dbo.Empresas e ON e.IdEmpresa = p.IdEmpresa
    INNER JOIN dbo.Sedes s ON s.IdSede = p.IdSede
    WHERE (@IdEmpresa IS NULL OR p.IdEmpresa = @IdEmpresa)
      AND (
            @Nombre = ''
            OR p.Nombres LIKE '%' + @Nombre + '%'
            OR p.ApellidoPaterno LIKE '%' + @Nombre + '%'
            OR ISNULL(p.ApellidoMaterno, '') LIKE '%' + @Nombre + '%'
            OR (p.ApellidoPaterno + ' ' + ISNULL(p.ApellidoMaterno, '') + ' ' + p.Nombres) LIKE '%' + @Nombre + '%'
          )
      AND (@Documento = '' OR p.NumeroDocumento LIKE '%' + @Documento + '%')
      AND (
            @Contacto = ''
            OR p.Celular LIKE '%' + @Contacto + '%'
            OR ISNULL(p.Correo, '') LIKE '%' + @Contacto + '%'
          )
    ORDER BY p.ApellidoPaterno, p.ApellidoMaterno, p.Nombres, p.IdPaciente
    OFFSET @Offset ROWS FETCH NEXT @P_PAGESIZE ROWS ONLY;
END

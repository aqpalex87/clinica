using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static MC.Control.Util.Constants;


namespace MC.Control.Handlers
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration configuration;

        public ErrorHandlingMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // No envolver Swagger/Swashbuckle: si la generación falla, este middleware convertía
            // la excepción en JSON de negocio y Swagger UI mostraba "Fetch error" al no ser OpenAPI.
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                await next(context);
                return;
            }

            context.Request.EnableBuffering();
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            SetMesageException(ex, ex.Message.Split("|"), out string exceptionCodigo, 
                                                          out string exceptionMensaje);

            var (httpstatuscode, titulo) = ErroresControl.ManejarErrores(exceptionCodigo);
            string result = JsonResponseError(exceptionCodigo, exceptionMensaje, titulo);

            var mensajeexcepcion = $"Mensaje: {ex.Message}, Detalle: {ex}";
            string resultexcepcioncompleta = JsonResponseError(exceptionCodigo, mensajeexcepcion, titulo);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpstatuscode;

            var request = new { context.Request.Headers, Body = await GetBodyRequestAsync(context) };

            GrabarLogError(context.Request.Path, JsonConvert.SerializeObject(request), resultexcepcioncompleta);

            await context.Response.WriteAsync(result);
        }

        private static async Task<dynamic> GetBodyRequestAsync(HttpContext context)
        {
            dynamic jsonrquest = null;

            if (context.Request.Body.CanSeek)
            {
                using var body = new StreamReader(context.Request.Body);
                body.BaseStream.Seek(0, SeekOrigin.Begin);
                var requestBody = await body.ReadToEndAsync();
                jsonrquest = JsonConvert.DeserializeObject(requestBody);
            }

            return jsonrquest;
        }

        private static void SetMesageException(Exception ex, string[] arrexcepcion, out string exceptionCodigo, out string exceptionMensaje)
        {
            if (ex.Message.Contains(ConstantesToken.ExpiradoCodigoJWT))
            {
                exceptionCodigo = ConstantesError.ERROR_TOKEN_EXPIRADO_CODIGO;
                exceptionMensaje = ex.Message;
            }
            else
            {
                exceptionCodigo = arrexcepcion.Length == 1 ? ConstantesError.ERROR_NO_CONTROLADO_CODIGO : arrexcepcion[0];
                exceptionMensaje = arrexcepcion.Length == 1 ? ex.Message : arrexcepcion[1];
            }
        }

        private static string JsonResponseError(string exceptionCodigo, string exceptionMensaje, string titulo)
        {
            return JsonConvert.SerializeObject(new RespuestaError
            {
                Error = new RespuestaErrorDetalle
                {
                    Codigo = exceptionCodigo,
                    Mensaje = exceptionMensaje,
                    Titulo = titulo
                }
            });
        }

        private void GrabarLogError(string api, string request, string response)
        {
            StringBuilder sb = new();
            sb.AppendLine($"SPC.API {DateTime.Now}");
            sb.AppendLine($"REQUEST PATH: {api}");
            sb.AppendLine($"REQUEST: {request}");
            sb.AppendLine($"REPONSE: {response}");
            sb.AppendLine($"{new string('-', 120)}\n");

            GenerarArchivoLog(sb.ToString());
        }

        private void GenerarArchivoLog(string strLog)
        {
            string carpetaLogs = configuration["APIrutaLogsError"];
            string logFilePath = $"{carpetaLogs}Log_SPC_{DateTime.Today:dd-MM-yyyy}.txt";

            var logDirInfo = new DirectoryInfo(carpetaLogs);
            if (!logDirInfo.Exists)
            {
                logDirInfo.Create();
            }

            File.AppendAllText(logFilePath, strLog);
        }
    }
}

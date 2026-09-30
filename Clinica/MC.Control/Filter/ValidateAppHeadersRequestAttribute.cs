using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static MC.Control.Util.Constants;

namespace MC.Control.Filter
{
    public class ValidateAppHeadersRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            string appkey = context.HttpContext.Request.Headers["X-AppKey"];
            string appcode = context.HttpContext.Request.Headers["X-AppCode"];

            string appkeyAppSetting = configuration.GetSection("AppKey").Value;
            string appCodeAppSetting = configuration.GetSection("AppCode").Value;

            if (appkey != appkeyAppSetting)
            {
                var mensaje = $"{ConstantesError.ERROR_APPKEY_INCORRECCTO_CODIGO}|Cabecera X-AppKey no es correcta.";
                context.Result = new BadRequestObjectResult(GetProblemDetails(mensaje));
            }
            else if (appcode != appCodeAppSetting)
            {
                var mensaje = $"{ConstantesError.ERROR_APPCODE_INCORRECCTO_CODIGO}|Cabecera X-AppCode no es correcta.";
                context.Result = new BadRequestObjectResult(GetProblemDetails(mensaje));
            }

        }

        private static ProblemDetails GetProblemDetails(string mensaje)
        {
            return new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Validation Problem.",
                Detail = mensaje,
            };
        }
    }
}

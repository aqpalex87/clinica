using System.Net;
using static MC.Control.Util.Constants;

namespace MC.Control.Handlers
{
    public static class ErroresControl
    {
        public static (HttpStatusCode, string) ManejarErrores(string tipoerror)
        {
            HttpStatusCode httpstatuscode;
            string titulo;
            if (tipoerror == ConstantesError.ERROR_PARAMETRO_CABECERA_REQUIREDO_CODIGO)
            {
                httpstatuscode = HttpStatusCode.BadRequest;
                titulo = ConstantesError.ERROR_PARAMETRO_CABECERA_REQUIREDO_MENSAJE;
            }
            else
            {
                httpstatuscode = HttpStatusCode.InternalServerError;
                titulo = ConstantesError.ERROR_NO_CONTROLADO_MENSAJE;
            }
            return (httpstatuscode, titulo);
        }
    }
}

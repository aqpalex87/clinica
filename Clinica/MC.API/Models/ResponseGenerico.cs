using System;

namespace SAR.API.Models
{
    public class ResponseSuccess
    {
        public ResponseSuccessDetalle Success { get; set; }
    }

    public class ResponseSuccessDetalle
    {
        public string Titulo { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public bool Resultado { get; set; }
        public DateTime? FechaRegistro { get; set; }

    }

    public class ResponseError
    {
        public ResponseErrorDetalle Error { get; set; }
    }

    public class ResponseErrorDetalle
    {
        public string Titulo { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public bool Resultado { get; set; }
    }
}

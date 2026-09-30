namespace MC.Control.Handlers
{
    public class RespuestaError
    {
        public RespuestaErrorDetalle Error { get; set; }
    }

    public class RespuestaErrorDetalle
    {
        public string Titulo { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
    }
}

using System;

namespace MC.Control.Model
{
    public class ResponseContainerModel
    {
        public ResponseContainerModel() { }

        public string Token { get; set; }
        public DateTime FechaInicioVigencia { get; set; }
        public DateTime FechaFinVigencia { get; set; }
    }
}

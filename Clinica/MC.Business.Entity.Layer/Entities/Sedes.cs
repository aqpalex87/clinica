using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Business.Entity.Layer.Entities
{
    public class Sedes:Auditoria
    {
        public int IdSede { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Direccion { get; set; }
        public bool Activo { get; set; }
    }
}

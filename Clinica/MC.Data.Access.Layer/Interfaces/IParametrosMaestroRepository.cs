using MC.Business.Entity.Layer.Entities;
using System.Collections.Generic;

namespace MC.Data.Access.Layer.Interfaces
{
    public interface IParametrosMaestroRepository
    {
        List<ParametroListaItem> ObtenerParametrosPorTipoCodigo(string codigoTipo);
    }
}

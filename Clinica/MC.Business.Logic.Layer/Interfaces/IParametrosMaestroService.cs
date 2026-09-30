using MC.DTO;
using MC.DTO.Dtos;
using System.Collections.Generic;

namespace MC.Business.Logic.Layer.Interfaces
{
    public interface IParametrosMaestroService
    {
        Result<List<ParametroListaDto>> ObtenerPorTipoCodigo(string codigoTipo);
    }
}

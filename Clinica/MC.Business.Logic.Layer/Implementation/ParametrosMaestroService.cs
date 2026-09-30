using MC.Business.Logic.Layer.Interfaces;
using MC.Data.Access.Layer.Interfaces;
using MC.DTO;
using MC.DTO.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MC.Business.Logic.Layer.Implementation
{
    public class ParametrosMaestroService : IParametrosMaestroService
    {
        private readonly IParametrosMaestroRepository _repository;

        public ParametrosMaestroService(IParametrosMaestroRepository repository)
        {
            _repository = repository;
        }

        public Result<List<ParametroListaDto>> ObtenerPorTipoCodigo(string codigoTipo)
        {
            Result<List<ParametroListaDto>> result = new();
            if (string.IsNullOrWhiteSpace(codigoTipo))
            {
                return result.BadRequest("Código de tipo requerido.");
            }

            var items = _repository.ObtenerParametrosPorTipoCodigo(codigoTipo.Trim());
            result.Status = HttpStatusCode.OK;
            result.Resultado = items.Select(x => new ParametroListaDto
            {
                IdParametro = x.IdParametro,
                Codigo = x.Codigo,
                Nombre = x.Nombre,
                Orden = x.Orden
            }).ToList();
            return result;
        }
    }
}

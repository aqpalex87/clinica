using MC.Business.Entity.Layer.Entities;
using MC.Business.Logic.Layer.Interfaces;
using MC.Data.Access.Layer.Interfaces;
using MC.DTO;
using MC.DTO.Dtos;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace MC.Business.Logic.Layer.Implementation
{
    public class PacienteService : IPacienteService
    {
        private static readonly string[] TiposDocumento = { "DNI", "CE", "PAS", "CPP" };
        private readonly IPacienteRepository _pacienteRepository;

        public PacienteService(IPacienteRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository;
        }

        public Result<PacientePaginadoDto> ObtenerPacientesPaginado(int? idEmpresa, string nombre, string numeroDocumento, string contacto, int pageNumber, int pageSize)
        {
            Result<PacientePaginadoDto> result = new();
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return result.BadRequest("Parámetros de paginación inválidos.");
            }

            var (pacientes, totalRows) = _pacienteRepository.ObtenerPacientesPaginado(idEmpresa, nombre, numeroDocumento, contacto, pageNumber, pageSize);
            result.Status = HttpStatusCode.OK;
            result.Resultado = new PacientePaginadoDto
            {
                TotalRows = totalRows,
                Items = pacientes.Select(MapToDto).ToList()
            };
            return result;
        }

        public Result<PacienteDto> CrearPaciente(PacienteDto request)
        {
            Result<PacienteDto> result = new();
            if (!Validar(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Paciente entity = MapToEntity(request);
            int codigo = _pacienteRepository.CrearPaciente(entity);
            if (!InterpretarCodigo(result, codigo))
            {
                return result;
            }

            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(entity);
            return result;
        }

        public Result<PacienteDto> ActualizarPaciente(int idPaciente, PacienteDto request)
        {
            Result<PacienteDto> result = new();
            if (idPaciente <= 0)
            {
                return result.BadRequest("IdPaciente inválido.");
            }
            if (!Validar(request, out string mensaje))
            {
                return result.BadRequest(mensaje);
            }

            Paciente entity = MapToEntity(request);
            entity.IdPaciente = idPaciente;
            int codigo = _pacienteRepository.ActualizarPaciente(entity);
            if (!InterpretarCodigo(result, codigo))
            {
                return result;
            }

            result.Status = HttpStatusCode.OK;
            result.Resultado = MapToDto(entity);
            return result;
        }

        public Result<bool> EliminarPaciente(int idPaciente)
        {
            Result<bool> result = new();
            if (idPaciente <= 0)
            {
                return result.BadRequest("IdPaciente inválido.");
            }

            int codigo = _pacienteRepository.EliminarPaciente(idPaciente);
            if (codigo == 0)
            {
                return result.NotFound("No se encontró el paciente para eliminar.");
            }

            result.Status = HttpStatusCode.OK;
            result.Resultado = true;
            return result;
        }

        private static bool InterpretarCodigo<T>(Result<T> result, int codigo)
        {
            if (codigo == 1)
            {
                return true;
            }
            if (codigo == -1)
            {
                result.BadRequest("Ya existe un paciente con ese documento en la empresa.");
                return false;
            }
            if (codigo == -2)
            {
                result.BadRequest("La sede no pertenece a la empresa seleccionada.");
                return false;
            }

            result.NotFound("No se encontró el paciente.");
            return false;
        }

        private static bool Validar(PacienteDto request, out string mensaje)
        {
            mensaje = string.Empty;
            if (request == null)
            {
                mensaje = "Datos requeridos.";
                return false;
            }
            if (request.IdEmpresa <= 0)
            {
                mensaje = "Empresa es requerida.";
                return false;
            }
            if (request.IdSede <= 0)
            {
                mensaje = "Sede es requerida.";
                return false;
            }

            string tipoDocumento = (request.TipoDocumento ?? string.Empty).Trim().ToUpperInvariant();
            if (!TiposDocumento.Contains(tipoDocumento))
            {
                mensaje = "Tipo de documento no válido. Use DNI, carné de extranjería, pasaporte o CPP.";
                return false;
            }

            string numeroDocumento = (request.NumeroDocumento ?? string.Empty).Trim().ToUpperInvariant();
            if (!DocumentoValido(tipoDocumento, numeroDocumento))
            {
                mensaje = MensajeDocumento(tipoDocumento);
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.ApellidoPaterno))
            {
                mensaje = "Apellido paterno es requerido.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(request.Nombres))
            {
                mensaje = "Nombres son requeridos.";
                return false;
            }
            if (!request.FechaNacimiento.HasValue || request.FechaNacimiento.Value.Date > DateTime.Today || request.FechaNacimiento.Value.Year < 1900)
            {
                mensaje = "Fecha de nacimiento no válida.";
                return false;
            }

            string sexo = (request.Sexo ?? string.Empty).Trim().ToUpperInvariant();
            if (sexo != "M" && sexo != "F")
            {
                mensaje = "Sexo es requerido.";
                return false;
            }

            string celular = new string((request.Celular ?? string.Empty).Where(char.IsDigit).ToArray());
            if (!Regex.IsMatch(celular, "^9\\d{8}$"))
            {
                mensaje = "El celular debe tener 9 dígitos y empezar con 9.";
                return false;
            }

            string correo = (request.Correo ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(correo) && !Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                mensaje = "Correo no válido.";
                return false;
            }

            return true;
        }

        private static bool DocumentoValido(string tipoDocumento, string numeroDocumento)
        {
            if (tipoDocumento == "DNI")
            {
                return Regex.IsMatch(numeroDocumento, @"^\d{8}$");
            }
            if (tipoDocumento == "PAS")
            {
                return Regex.IsMatch(numeroDocumento, @"^[A-Z0-9]{5,12}$");
            }
            return Regex.IsMatch(numeroDocumento, @"^[A-Z0-9]{6,12}$");
        }

        private static string MensajeDocumento(string tipoDocumento)
        {
            if (tipoDocumento == "DNI")
            {
                return "El DNI debe tener 8 dígitos.";
            }
            if (tipoDocumento == "PAS")
            {
                return "El pasaporte debe tener entre 5 y 12 caracteres.";
            }
            return "El documento debe tener entre 6 y 12 caracteres.";
        }

        private static PacienteDto MapToDto(Paciente item)
        {
            string apellidoMaterno = string.IsNullOrWhiteSpace(item.ApellidoMaterno) ? string.Empty : item.ApellidoMaterno.Trim() + " ";
            return new PacienteDto
            {
                IdPaciente = item.IdPaciente,
                IdEmpresa = item.IdEmpresa,
                NombreEmpresa = item.NombreEmpresa,
                IdSede = item.IdSede,
                NombreSede = item.NombreSede,
                Codigo = item.Codigo,
                TipoDocumento = item.TipoDocumento,
                NumeroDocumento = item.NumeroDocumento,
                ApellidoPaterno = item.ApellidoPaterno,
                ApellidoMaterno = item.ApellidoMaterno,
                Nombres = item.Nombres,
                NombreCompleto = $"{item.ApellidoPaterno} {apellidoMaterno}{item.Nombres}".Trim(),
                FechaNacimiento = item.FechaNacimiento,
                Edad = CalcularEdad(item.FechaNacimiento),
                Sexo = item.Sexo,
                Celular = item.Celular,
                Correo = item.Correo,
                Direccion = item.Direccion,
                Observacion = item.Observacion,
                Activo = item.Activo
            };
        }

        private static Paciente MapToEntity(PacienteDto item)
        {
            string celular = new string((item.Celular ?? string.Empty).Where(char.IsDigit).ToArray());
            return new Paciente
            {
                IdPaciente = item.IdPaciente,
                IdEmpresa = item.IdEmpresa,
                IdSede = item.IdSede,
                TipoDocumento = item.TipoDocumento?.Trim().ToUpperInvariant(),
                NumeroDocumento = item.NumeroDocumento?.Trim().ToUpperInvariant(),
                ApellidoPaterno = item.ApellidoPaterno?.Trim(),
                ApellidoMaterno = string.IsNullOrWhiteSpace(item.ApellidoMaterno) ? null : item.ApellidoMaterno.Trim(),
                Nombres = item.Nombres?.Trim(),
                FechaNacimiento = item.FechaNacimiento.Value.Date,
                Sexo = item.Sexo?.Trim().ToUpperInvariant(),
                Celular = celular,
                Correo = string.IsNullOrWhiteSpace(item.Correo) ? null : item.Correo.Trim(),
                Direccion = string.IsNullOrWhiteSpace(item.Direccion) ? null : item.Direccion.Trim(),
                Observacion = string.IsNullOrWhiteSpace(item.Observacion) ? null : item.Observacion.Trim(),
                Activo = item.Activo
            };
        }

        private static int CalcularEdad(DateTime nacimiento)
        {
            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - nacimiento.Year;
            if (nacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }
            return edad < 0 ? 0 : edad;
        }
    }
}

export interface PacienteCrud {
  idPaciente: number;
  idEmpresa: number;
  nombreEmpresa?: string;
  idSede: number;
  nombreSede?: string;
  codigo?: string;
  tipoDocumento: string;
  numeroDocumento: string;
  apellidoPaterno: string;
  apellidoMaterno?: string;
  nombres: string;
  nombreCompleto?: string;
  fechaNacimiento?: string;
  edad?: number;
  sexo: string;
  celular: string;
  correo?: string;
  direccion?: string;
  observacion?: string;
  activo: boolean;
}

export interface PacientePaginadoResponse {
  totalRows: number;
  items: PacienteCrud[];
}

export interface SedeCrud {
  idSede: number;
  idEmpresa: number;
  nombreEmpresa?: string;
  nombre: string;
  codigo: string;
  direccion: string;
  activo: boolean;
}

export interface SedePaginadoResponse {
  totalRows: number;
  items: SedeCrud[];
}

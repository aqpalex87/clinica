export interface RolCrud {
  idRol: number;
  idEmpresa: number;
  nombreEmpresa?: string;
  nombre: string;
  descripcion: string;
  activo: boolean;
}

export interface RolPaginadoResponse {
  totalRows: number;
  items: RolCrud[];
}

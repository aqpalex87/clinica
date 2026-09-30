export interface EmpresaCrud {
  idEmpresa: number;
  nombre: string;
  ruc: string;
  activo: boolean;
}

export interface EmpresaPaginadoResponse {
  totalRows: number;
  items: EmpresaCrud[];
}

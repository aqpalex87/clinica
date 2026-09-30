export interface UsuarioCrud {
  idUsuario: number;
  usuarioLogin: string;
  password?: string;
  idEmpresa: number;
  nombreEmpresa?: string;
  idRol: number;
  nombreRol?: string;
  activo: boolean;
}

export interface UsuarioPaginadoResponse {
  totalRows: number;
  items: UsuarioCrud[];
}

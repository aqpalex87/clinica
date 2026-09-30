export interface MenuCrud {
  idMenu: number;
  nombre: string;
  ruta: string;
  icono: string;
  idPadre?: number | null;
  nombrePadre?: string;
  orden?: number | null;
  activo: boolean;
}

export interface MenuPaginadoResponse {
  totalRows: number;
  items: MenuCrud[];
}

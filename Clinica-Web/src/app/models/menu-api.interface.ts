export interface MenuApi {
  idMenu: number;
  nombre: string;
  ruta: string;
  icono: string;
  idPadre?: number | null;
  orden?: number | null;
  hijos: MenuApi[];
}


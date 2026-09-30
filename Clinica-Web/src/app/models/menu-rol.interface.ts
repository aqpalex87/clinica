export interface RolSimple {
  idRol: number;
  nombre: string;
}

export interface MenuRolTree {
  idMenu: number;
  nombre: string;
  idPadre?: number | null;
  seleccionado: boolean;
  hijos: MenuRolTree[];
}

export interface GuardarMenuRolRequest {
  idRol: number;
  idsMenu: number[];
}

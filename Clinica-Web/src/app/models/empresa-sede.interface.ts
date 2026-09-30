export interface EmpresaAuth {
  idEmpresa: number;
  nombre: string;
}

export interface SedeAuth {
  idSede: number;
  idEmpresa: number;
  nombre: string;
}

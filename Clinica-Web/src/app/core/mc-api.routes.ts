import { environment } from 'src/environments/environment';

/** Base MC (`env.js` → UrlBase_MCAPI, p.ej. `https://localhost:44351/api`). Sin barra final. */
function MC(): string {
  const b = environment.UrlBase_MCAPI;
  return typeof b === 'string' ? b.replace(/\/+$/, '') : '';
}

/**
 * Rutas de MC.API en un solo sitio.
 * Flujos públicos de login usan `auth/*` (AuthController); CRUD sigue en sus controladores.
 */
export const MCApiRoutes = {
  root: MC,

  acceso: () => `${MC()}/acceso`,

  auth: {
    login: () => `${MC()}/Auth/login`,
    empresas: () => `${MC()}/Auth/empresas`,
    sedesPorEmpresa: (idEmpresa: number) => `${MC()}/Auth/sedes/${idEmpresa}`,
    menus: () => `${MC()}/Auth/menus`,
    seleccionarSede: () => `${MC()}/Auth/seleccionar-sede`,
  },

  /** Segmentos con la misma capitalización que `[controller]` en MC.API. */
  sedes: {
    base: () => `${MC()}/Sedes`,
    byId: (idSede: number) => `${MC()}/Sedes/${idSede}`,
  },

  roles: () => `${MC()}/Roles`,
  menuroles: () => `${MC()}/MenuRoles`,
  usuarios: () => `${MC()}/Usuarios`,
  empresas: () => `${MC()}/Empresas`,
  menu: () => `${MC()}/Menu`,
  clientes: () => `${MC()}/Clientes`,
  parametrosCatalogo: {
    base: () => `${MC()}/ParametrosCatalogo`,
    porTipo: (codigoTipo: string) =>
      `${MC()}/ParametrosCatalogo/por-tipo/${encodeURIComponent(codigoTipo)}`,
  },
} as const;

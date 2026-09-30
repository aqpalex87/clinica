import { EmpresaAuth } from 'src/app/models/empresa-sede.interface';

/**
 * Extrae el listado de empresas del resultado de `SecurityService.obtenerEmpresasApi()` (rxjs/ajax).
 * Misma lógica que debe usar roles, sedes, ciclos, etc.: `ajax` pone el JSON en `response`, a veces envuelto.
 */
export function parseEmpresasMCAjax(res: unknown): EmpresaAuth[] {
  if (res == null || typeof res !== 'object') {
    return [];
  }
  const root = res as Record<string, unknown>;
  const body = root['response'] as unknown;

  let rawList: unknown;
  if (Array.isArray(body)) {
    rawList = body;
  } else if (body && typeof body === 'object') {
    const b = body as Record<string, unknown>;
    rawList = b['response'] ?? b['data'] ?? b['Data'] ?? b['resultado'] ?? b['Resultado'];
  } else {
    rawList = body;
  }

  const source = Array.isArray(rawList) ? rawList : [];
  return source
    .map((x: unknown) => {
      const item = (x ?? {}) as Record<string, unknown>;
      return {
        idEmpresa: Number(item['idEmpresa'] ?? item['IdEmpresa'] ?? 0),
        nombre: String(item['nombre'] ?? item['Nombre'] ?? ''),
      };
    })
    .filter((x) => x.idEmpresa > 0);
}

const TITULOS_GENERICOS = new Set([
  'one or more validation errors occurred.',
  'bad request',
  'not found',
  'unauthorized',
  'forbidden',
]);

export function mensajeErrorApi(error: unknown, fallback: string): string {
  const cuerpo = cuerpoError(error);
  const desdeErrores = mensajesDeErrors(cuerpo);
  if (desdeErrores) {
    return desdeErrores;
  }

  if (cuerpo && typeof cuerpo === 'object') {
    const record = cuerpo as Record<string, unknown>;
    const mensaje = textoUtil(record['mensaje'] ?? record['Mensaje'] ?? record['detail'] ?? record['Detail']);
    if (mensaje) {
      return mensaje;
    }
    const title = textoUtil(record['title'] ?? record['Title']);
    if (title && !TITULOS_GENERICOS.has(title.toLowerCase())) {
      return title;
    }
  }

  if (typeof cuerpo === 'string') {
    const limpio = cuerpo.trim();
    if (limpio && !limpio.startsWith('<') && !esMensajeTecnico(limpio)) {
      return limpio;
    }
  }

  return fallback;
}

function cuerpoError(error: unknown): unknown {
  if (!error || typeof error !== 'object') {
    return null;
  }

  const e = error as { name?: string; error?: unknown; response?: unknown };
  if (e.name === 'TimeoutError') {
    return null;
  }

  if (e.error != null && e.error !== '' && !esEventoRed(e.error)) {
    return e.error;
  }

  if (e.response != null && e.response !== '') {
    return e.response;
  }

  return null;
}

function mensajesDeErrors(cuerpo: unknown): string | null {
  if (!cuerpo || typeof cuerpo !== 'object') {
    return null;
  }

  const record = cuerpo as Record<string, unknown>;
  const errors = record['errors'] ?? record['Errors'];
  if (!errors || typeof errors !== 'object' || Array.isArray(errors)) {
    return null;
  }

  const mapa = errors as Record<string, unknown>;
  const preferido = mapa['error'] ?? mapa['Error'];
  const textos = preferido != null
    ? aplanar(preferido)
    : Object.keys(mapa).flatMap((key) => aplanar(mapa[key]));

  const unicos = textos.filter((item, index) => textos.indexOf(item) === index);
  return unicos.length ? unicos.join(' ') : null;
}

function aplanar(value: unknown): string[] {
  if (typeof value === 'string') {
    const limpio = value.trim();
    return limpio && !esMensajeTecnico(limpio) ? [limpio] : [];
  }
  if (Array.isArray(value)) {
    return value.flatMap((item) => aplanar(item));
  }
  return [];
}

function textoUtil(value: unknown): string | null {
  if (typeof value !== 'string') {
    return null;
  }
  const limpio = value.trim();
  if (!limpio || limpio.startsWith('<') || esMensajeTecnico(limpio)) {
    return null;
  }
  return limpio;
}

function esMensajeTecnico(texto: string): boolean {
  const t = texto.toLowerCase();
  return t.startsWith('http failure response') || t.startsWith('ajax error');
}

function esEventoRed(value: unknown): boolean {
  return typeof ProgressEvent !== 'undefined' && value instanceof ProgressEvent;
}

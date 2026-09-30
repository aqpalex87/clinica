/** Normaliza respuestas paginadas de MC.API (camelCase o PascalCase en Newtonsoft). */
export function normalizeMCPaginated<T>(raw: unknown): { items: T[]; totalRows: number } {
  if (Array.isArray(raw)) {
    return { items: raw as T[], totalRows: raw.length };
  }

  let body: unknown = raw;
  if (body && typeof body === 'object') {
    const r = body as Record<string, unknown>;
    const inner = r['resultado'] ?? r['Resultado'] ?? r['data'] ?? r['Data'];
    if (inner && typeof inner === 'object') {
      const ir = inner as Record<string, unknown>;
      if ('items' in ir || 'Items' in ir) {
        body = inner;
      }
    }
  }
  if (!body || typeof body !== 'object') {
    return { items: [], totalRows: 0 };
  }
  const o = body as Record<string, unknown>;
  const itemsRaw = o['items'] ?? o['Items'];
  const items = (Array.isArray(itemsRaw) ? itemsRaw : []) as T[];
  const totalRaw = o['totalRows'] ?? o['TotalRows'] ?? 0;
  const totalRows = typeof totalRaw === 'number' ? totalRaw : Number(totalRaw) || 0;
  return { items, totalRows };
}

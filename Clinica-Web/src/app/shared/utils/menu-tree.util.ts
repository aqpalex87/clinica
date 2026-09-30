import { MenuApi } from 'src/app/models/menu-api.interface';

/**
 * Normaliza propiedades camelCase/PascalCase y reconstruye jerarquía si la API devolvió lista plana.
 */
export function normalizeMenuApiList(items: any[] | null | undefined): MenuApi[] {
  if (!items?.length) {
    return [];
  }

  const nodes = items.map((item) => normalizeMenuNode(item));

  const hasNestedChildren = nodes.some((n) => n.hijos?.length > 0);
  if (hasNestedChildren) {
    return nodes.map((n) => ({
      ...n,
      hijos: normalizeMenuApiList(n.hijos),
    }));
  }

  return buildMenuTreeFromFlat(nodes);
}

function normalizeMenuNode(item: any): MenuApi {
  const hijosRaw = item?.hijos ?? item?.Hijos ?? [];
  const hijosArr = Array.isArray(hijosRaw) ? hijosRaw : [];

  return {
    idMenu: Number(item?.idMenu ?? item?.IdMenu ?? 0),
    nombre: (item?.nombre ?? item?.Nombre ?? '').toString(),
    ruta: (item?.ruta ?? item?.Ruta ?? '').toString(),
    icono: (item?.icono ?? item?.Icono ?? '').toString(),
    idPadre:
      item?.idPadre !== undefined && item?.idPadre !== null
        ? Number(item.idPadre)
        : item?.IdPadre !== undefined && item?.IdPadre !== null
          ? Number(item.IdPadre)
          : null,
    orden:
      item?.orden !== undefined && item?.orden !== null
        ? Number(item.orden)
        : item?.Orden !== undefined && item?.Orden !== null
          ? Number(item.Orden)
          : null,
    hijos: hijosArr.map((h: any) => normalizeMenuNode(h)),
  };
}

function buildMenuTreeFromFlat(flat: MenuApi[]): MenuApi[] {
  const map = new Map<number, MenuApi>();

  flat.forEach((m) => {
    if (!m.idMenu) {
      return;
    }
    map.set(m.idMenu, { ...m, hijos: [] });
  });

  const roots: MenuApi[] = [];

  map.forEach((node) => {
    const pid = node.idPadre;
    if (pid != null && !Number.isNaN(pid) && map.has(pid)) {
      map.get(pid)!.hijos.push(node);
    } else {
      roots.push(node);
    }
  });

  const sortFn = (a: MenuApi, b: MenuApi) =>
    (a.orden ?? 99999) - (b.orden ?? 99999) || a.idMenu - b.idMenu;

  const walk = (list: MenuApi[]) => {
    list.sort(sortFn);
    list.forEach((n) => walk(n.hijos));
  };

  walk(roots);
  return roots;
}

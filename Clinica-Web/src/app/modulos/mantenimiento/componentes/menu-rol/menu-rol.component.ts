import { Component, OnInit } from '@angular/core';
import { TreeNode } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { finalize, timeout } from 'rxjs/operators';
import { EmpresaAuth } from 'src/app/models/empresa-sede.interface';
import { GuardarMenuRolRequest, MenuRolTree, RolSimple } from 'src/app/models/menu-rol.interface';
import { MenuRolAdminService } from 'src/app/services/menu-rol-admin.service';
import { SecurityService } from 'src/app/services/security.service';
import { parseEmpresasMCAjax } from 'src/app/shared/utils/MC-ajax-empresas.util';
import { mensajeErrorApi } from 'src/app/shared/utils/mc-api-error.util';

@Component({
  selector: 'app-menu-rol',
  templateUrl: './menu-rol.component.html',
  styleUrls: ['./menu-rol.component.css'],
})
export class MenuRolComponent implements OnInit {
  empresas: EmpresaAuth[] = [];
  roles: RolSimple[] = [];
  menuTree: TreeNode[] = [];
  selectedNodes: TreeNode[] = [];

  idEmpresaSesion = 0;
  empresaNombre = '';
  idEmpresa: number | null = null;
  idRol: number | null = null;

  loading = false;
  loadingTree = false;
  guardando = false;

  constructor(
    private menuRolService: MenuRolAdminService,
    private securityService: SecurityService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.idEmpresaSesion = Number(this.securityService.leerIdEmpresa() || 0);
    this.empresaNombre = this.securityService.leerEmpresa() || '-';
    this.cargarEmpresas();
  }

  onEmpresaChange(): void {
    this.idRol = null;
    this.menuTree = [];
    this.selectedNodes = [];
    this.roles = [];
    if (!this.idEmpresa || this.idEmpresa <= 0) {
      return;
    }

    this.loading = true;
    this.menuRolService.obtenerRolesPorEmpresa(this.idEmpresa)
      .pipe(
        timeout(15000),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: (roles) => {
          this.roles = roles ?? [];
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudieron obtener los roles de la empresa.')),
      });
  }

  onRolChange(): void {
    this.menuTree = [];
    this.selectedNodes = [];
    if (!this.idRol || this.idRol <= 0) {
      return;
    }

    this.loadingTree = true;
    this.menuRolService.obtenerMenusPorRol(this.idRol)
      .pipe(
        timeout(15000),
        finalize(() => (this.loadingTree = false))
      )
      .subscribe({
        next: (tree) => {
          this.menuTree = this.mapToTreeNodes(tree ?? []);
          this.selectedNodes = this.collectSelectedNodes(this.menuTree);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener el árbol de menús.')),
      });
  }

  expandAll(): void {
    this.toggleExpandCollapse(this.menuTree, true);
  }

  collapseAll(): void {
    this.toggleExpandCollapse(this.menuTree, false);
  }

  guardar(): void {
    if (!this.idRol || this.idRol <= 0) {
      this.toastr.warning('Seleccione un rol antes de guardar.');
      return;
    }

    const idsMenu = this.extractSelectedMenuIds(this.selectedNodes);
    const request: GuardarMenuRolRequest = {
      idRol: this.idRol,
      idsMenu
    };

    this.guardando = true;
    this.menuRolService.guardar(request)
      .pipe(
        timeout(15000),
        finalize(() => (this.guardando = false))
      )
      .subscribe({
        next: () => this.toastr.success('Menús del rol guardados correctamente.'),
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudieron guardar los menús del rol.')),
      });
  }

  private cargarEmpresas(): void {
    this.loading = true;
    this.securityService.obtenerEmpresasApi()
      .pipe(
        timeout(15000),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: (res: any) => {
          this.empresas = parseEmpresasMCAjax(res);

          if (this.idEmpresaSesion > 0 && this.empresas.some(x => x.idEmpresa === this.idEmpresaSesion)) {
            this.idEmpresa = this.idEmpresaSesion;
            this.onEmpresaChange();
          }
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener la lista de empresas.')),
      });
  }

  private mapToTreeNodes(items: MenuRolTree[]): TreeNode[] {
    return (items ?? []).map((item) => ({
      key: item.idMenu.toString(),
      label: item.nombre,
      data: { idMenu: item.idMenu, seleccionado: item.seleccionado },
      selectable: true,
      expanded: true,
      children: this.mapToTreeNodes(item.hijos ?? []),
    }));
  }

  private collectSelectedNodes(nodes: TreeNode[]): TreeNode[] {
    const selected: TreeNode[] = [];
    const walk = (list: TreeNode[]) => {
      list.forEach((n) => {
        if (n?.data?.seleccionado) {
          selected.push(n);
        }
        if (n.children?.length) {
          walk(n.children);
        }
      });
    };
    walk(nodes);
    return selected;
  }

  private extractSelectedMenuIds(nodes: TreeNode[]): number[] {
    return (nodes ?? [])
      .map((n) => Number(n?.data?.idMenu ?? n?.key ?? 0))
      .filter((id, index, arr) => id > 0 && arr.indexOf(id) === index);
  }

  private toggleExpandCollapse(nodes: TreeNode[], expanded: boolean): void {
    (nodes ?? []).forEach((node) => {
      node.expanded = expanded;
      if (node.children?.length) {
        this.toggleExpandCollapse(node.children, expanded);
      }
    });
    this.menuTree = [...this.menuTree];
  }
}

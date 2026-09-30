import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LazyLoadEvent } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { finalize, timeout } from 'rxjs/operators';
import { MenuCrud } from 'src/app/models/menu-crud.interface';
import { MenuAdminService } from 'src/app/services/menu-admin.service';
import { SecurityService } from 'src/app/services/security.service';
import { AuthSubjectService } from 'src/app/services/subjects/auth-subject.service';
import { mensajeErrorApi } from 'src/app/shared/utils/mc-api-error.util';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css'],
})
export class MenuComponent implements OnInit {
  menus: MenuCrud[] = [];
  padres: MenuCrud[] = [];
  totalRows = 0;
  rows = 10;
  loading = false;
  busqueda = '';

  modalVisible = false;
  editando = false;
  idEditando = 0;

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private menuService: MenuAdminService,
    private toastr: ToastrService,
    private securityService: SecurityService,
    private authSubjectService: AuthSubjectService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      ruta: ['', [Validators.maxLength(200)]],
      icono: ['', [Validators.maxLength(50)]],
      idPadre: [null],
      orden: [null],
      activo: [true],
    });
  }

  ngOnInit(): void {
    this.cargarPadres();
    this.cargarMenus(1, this.rows);
  }

  onLazyLoad(event: LazyLoadEvent): void {
    const first = event.first ?? 0;
    const pageSize = event.rows ?? this.rows;
    const pageNumber = Math.floor(first / pageSize) + 1;
    this.rows = pageSize;
    this.cargarMenus(pageNumber, pageSize);
  }

  buscar(): void {
    this.cargarMenus(1, this.rows);
  }

  limpiarBusqueda(): void {
    this.busqueda = '';
    this.cargarMenus(1, this.rows);
  }

  abrirNuevo(): void {
    this.editando = false;
    this.idEditando = 0;
    this.form.reset({
      nombre: '',
      ruta: '',
      icono: '',
      idPadre: null,
      orden: null,
      activo: true,
    });
    this.modalVisible = true;
  }

  editar(item: MenuCrud): void {
    this.editando = true;
    this.idEditando = item.idMenu;
    this.form.patchValue({
      nombre: item.nombre,
      ruta: item.ruta,
      icono: item.icono,
      idPadre: item.idPadre ?? null,
      orden: item.orden ?? null,
      activo: item.activo,
    });
    this.modalVisible = true;
  }

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const data: MenuCrud = {
      idMenu: this.idEditando,
      nombre: (this.form.value.nombre ?? '').trim(),
      ruta: (this.form.value.ruta ?? '').trim(),
      icono: (this.form.value.icono ?? '').trim(),
      idPadre: this.form.value.idPadre ?? null,
      orden: this.form.value.orden ?? null,
      activo: !!this.form.value.activo,
    };

    if (this.editando) {
      this.menuService.actualizar(this.idEditando, data).subscribe({
        next: () => {
          this.toastr.success('Menú actualizado correctamente.');
          this.modalVisible = false;
          this.cargarPadres();
          this.cargarMenus(1, this.rows);
          this.recargarMenusSidebar();
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo actualizar el menú.')),
      });
      return;
    }

    this.menuService.crear(data).subscribe({
      next: () => {
        this.toastr.success('Menú registrado correctamente.');
        this.modalVisible = false;
        this.cargarPadres();
        this.cargarMenus(1, this.rows);
        this.recargarMenusSidebar();
      },
      error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo registrar el menú.')),
    });
  }

  eliminar(item: MenuCrud): void {
    void Swal.fire({
      title: 'Confirmar eliminación',
      text: `¿Desea eliminar el menú "${item.nombre}"?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
      confirmButtonColor: '#00694f',
      cancelButtonColor: '#64748b',
      reverseButtons: true,
      focusCancel: true,
      customClass: {
        popup: 'menu-swal-popup',
        title: 'menu-swal-title',
        confirmButton: 'menu-swal-confirm',
        cancelButton: 'menu-swal-cancel',
      },
    }).then((result) => {
      if (!result.isConfirmed) {
        return;
      }

      this.menuService.eliminar(item.idMenu).subscribe({
        next: () => {
          this.toastr.success('Menú eliminado correctamente.');
          this.cargarPadres();
          this.cargarMenus(1, this.rows);
          this.recargarMenusSidebar();
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo eliminar el menú.')),
      });
    });
  }

  opcionesPadre(): { label: string; value: number | null }[] {
    return [
      { label: 'Sin padre', value: null },
      ...this.padres.map((x) => ({ label: x.nombre, value: x.idMenu })),
    ];
  }

  private cargarMenus(pageNumber: number, pageSize: number): void {
    this.loading = true;
    this.menuService
      .obtenerPaginado(this.busqueda, pageNumber, pageSize)
      .pipe(
        timeout(15000),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (response) => {
          this.menus = response?.items ?? [];
          this.totalRows = response?.totalRows ?? 0;
        },
        error: (err) => {
          this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener el listado de menús.'));
        },
      });
  }

  private cargarPadres(): void {
    this.menuService.obtenerPadres().subscribe({
      next: (response) => {
        this.padres = (response ?? []).filter((x) => x.activo);
      },
      error: (err) => {
        this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener la lista de menús padre.'));
      },
    });
  }

  private recargarMenusSidebar(): void {
    this.securityService.obtenerMenusApi().subscribe({
      next: (menusRes: any) => {
        this.securityService.guardarMenusApi(menusRes?.response ?? []);
        this.authSubjectService.setLoadedUser(true);
      },
      error: (err) => {
        this.toastr.warning(mensajeErrorApi(err, 'Se guardó el menú, pero no se pudo refrescar la barra. Recarga la página.'));
      },
    });
  }
}

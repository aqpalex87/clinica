import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LazyLoadEvent } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { finalize, timeout } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { RolCrud } from 'src/app/models/rol-crud.interface';
import { RolAdminService } from 'src/app/services/rol-admin.service';
import { SecurityService } from 'src/app/services/security.service';
import { mensajeErrorApi } from 'src/app/shared/utils/mc-api-error.util';
import { textoErrorCampo } from 'src/app/shared/utils/mc-form-error.util';

@Component({
  selector: 'app-rol',
  templateUrl: './rol.component.html',
  styleUrls: ['./rol.component.css'],
})
export class RolComponent implements OnInit {
  roles: RolCrud[] = [];
  totalRows = 0;
  rows = 10;
  loading = false;
  busqueda = '';

  modalVisible = false;
  editando = false;
  idEditando = 0;

  idEmpresaSesion = 0;
  empresaNombre = '';
  sedeNombre = '';
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private rolService: RolAdminService,
    private securityService: SecurityService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(150)]],
      descripcion: ['', [Validators.maxLength(300)]],
      activo: [true],
    });
  }

  ngOnInit(): void {
    this.idEmpresaSesion = Number(this.securityService.leerIdEmpresa() || 0);
    this.empresaNombre = this.securityService.leerEmpresa() || '-';
    this.sedeNombre = this.securityService.leerSede() || '-';
  }

  onLazyLoad(event: LazyLoadEvent): void {
    const first = event.first ?? 0;
    const pageSize = event.rows ?? this.rows;
    const pageNumber = Math.floor(first / pageSize) + 1;
    this.rows = pageSize;
    this.cargarRoles(pageNumber, pageSize);
  }

  buscar(): void {
    this.cargarRoles(1, this.rows);
  }

  limpiarBusqueda(): void {
    this.busqueda = '';
    this.cargarRoles(1, this.rows);
  }

  abrirNuevo(): void {
    this.editando = false;
    this.idEditando = 0;
    this.form.reset({
      nombre: '',
      descripcion: '',
      activo: true,
    });
    this.modalVisible = true;
  }

  editar(item: RolCrud): void {
    this.editando = true;
    this.idEditando = item.idRol;
    this.form.patchValue({
      nombre: item.nombre,
      descripcion: item.descripcion,
      activo: item.activo,
    });
    this.modalVisible = true;
  }

  mensajeCampo(nombre: string): string {
    return textoErrorCampo(this.form, nombre);
  }

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const data: RolCrud = {
      idRol: this.idEditando,
      idEmpresa: this.idEmpresaSesion,
      nombre: (this.form.value.nombre ?? '').trim(),
      descripcion: (this.form.value.descripcion ?? '').trim(),
      activo: !!this.form.value.activo,
    };

    if (this.editando) {
      this.rolService.actualizar(this.idEditando, data).subscribe({
        next: () => {
          this.toastr.success('Rol actualizado correctamente.');
          this.modalVisible = false;
          this.cargarRoles(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo actualizar el rol.')),
      });
      return;
    }

    this.rolService.crear(data).subscribe({
      next: () => {
        this.toastr.success('Rol registrado correctamente.');
        this.modalVisible = false;
        this.cargarRoles(1, this.rows);
      },
      error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo registrar el rol.')),
    });
  }

  eliminar(item: RolCrud): void {
    void Swal.fire({
      title: 'Confirmar eliminación',
      text: `¿Desea eliminar el rol "${item.nombre}"?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
      confirmButtonColor: '#dc2626',
      cancelButtonColor: '#64748b',
      reverseButtons: true,
      focusCancel: true,
    }).then((result) => {
      if (!result.isConfirmed) {
        return;
      }

      this.rolService.eliminar(item.idRol).subscribe({
        next: () => {
          this.toastr.success('Rol eliminado correctamente.');
          this.cargarRoles(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo eliminar el rol.')),
      });
    });
  }

  private cargarRoles(pageNumber: number, pageSize: number): void {
    this.loading = true;
    this.rolService.obtenerPaginado(this.idEmpresaSesion, this.busqueda, pageNumber, pageSize)
      .pipe(
        timeout(15000),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (response) => {
          this.roles = response?.items ?? [];
          this.totalRows = response?.totalRows ?? 0;
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener el listado de roles.')),
      });
  }

}

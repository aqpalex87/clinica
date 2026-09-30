import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LazyLoadEvent } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { finalize, timeout } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { RolSimple } from 'src/app/models/menu-rol.interface';
import { UsuarioCrud } from 'src/app/models/usuario-crud.interface';
import { MenuRolAdminService } from 'src/app/services/menu-rol-admin.service';
import { SecurityService } from 'src/app/services/security.service';
import { UsuarioAdminService } from 'src/app/services/usuario-admin.service';
import { mensajeErrorApi } from 'src/app/shared/utils/mc-api-error.util';
import { textoErrorCampo } from 'src/app/shared/utils/mc-form-error.util';

@Component({
  selector: 'app-usuario',
  templateUrl: './usuario.component.html',
  styleUrls: ['./usuario.component.css'],
})
export class UsuarioComponent implements OnInit {
  usuarios: UsuarioCrud[] = [];
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
  roles: RolSimple[] = [];
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private usuarioService: UsuarioAdminService,
    private menuRolService: MenuRolAdminService,
    private securityService: SecurityService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      usuarioLogin: ['', [Validators.required, Validators.maxLength(50)]],
      password: [''],
      idRol: [null, [Validators.required]],
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
    this.cargarUsuarios(pageNumber, pageSize);
  }

  buscar(): void {
    this.cargarUsuarios(1, this.rows);
  }

  limpiarBusqueda(): void {
    this.busqueda = '';
    this.cargarUsuarios(1, this.rows);
  }

  abrirNuevo(): void {
    this.editando = false;
    this.idEditando = 0;
    this.form.reset({
      usuarioLogin: '',
      password: '',
      idRol: null,
      activo: true,
    });
    this.form.get('password')?.setValidators([Validators.required, Validators.maxLength(100)]);
    this.form.get('password')?.updateValueAndValidity();
    this.roles = [];
    void this.cargarRolesFormulario(this.idEmpresaSesion);
    this.modalVisible = true;
  }

  editar(item: UsuarioCrud): void {
    this.editando = true;
    this.idEditando = item.idUsuario;
    this.form.patchValue({
      usuarioLogin: item.usuarioLogin,
      password: '',
      idRol: item.idRol,
      activo: item.activo,
    });
    this.form.get('password')?.clearValidators();
    this.form.get('password')?.updateValueAndValidity();
    this.roles = [];
    void this.cargarRolesFormulario(this.idEmpresaSesion, item.idRol);
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

    const data: UsuarioCrud = {
      idUsuario: this.idEditando,
      usuarioLogin: (this.form.value.usuarioLogin ?? '').trim(),
      password: (this.form.value.password ?? '').trim(),
      idEmpresa: this.idEmpresaSesion,
      idRol: Number(this.form.value.idRol),
      activo: !!this.form.value.activo,
    };

    if (this.editando) {
      this.usuarioService.actualizar(this.idEditando, data).subscribe({
        next: () => {
          this.toastr.success('Usuario actualizado correctamente.');
          this.modalVisible = false;
          this.cargarUsuarios(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo actualizar el usuario.')),
      });
      return;
    }

    this.usuarioService.crear(data).subscribe({
      next: () => {
        this.toastr.success('Usuario registrado correctamente.');
        this.modalVisible = false;
        this.cargarUsuarios(1, this.rows);
      },
      error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo registrar el usuario.')),
    });
  }

  eliminar(item: UsuarioCrud): void {
    void Swal.fire({
      title: 'Confirmar eliminación',
      text: `¿Desea eliminar el usuario "${item.usuarioLogin}"?`,
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

      this.usuarioService.eliminar(item.idUsuario).subscribe({
        next: () => {
          this.toastr.success('Usuario eliminado correctamente.');
          this.cargarUsuarios(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo eliminar el usuario.')),
      });
    });
  }

  private cargarUsuarios(pageNumber: number, pageSize: number): void {
    this.loading = true;
    this.usuarioService.obtenerPaginado(this.idEmpresaSesion, this.busqueda, pageNumber, pageSize)
      .pipe(
        timeout(15000),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (response) => {
          this.usuarios = response?.items ?? [];
          this.totalRows = response?.totalRows ?? 0;
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener el listado de usuarios.')),
      });
  }

  private async cargarRolesFormulario(idEmpresa: number, idRolSeleccionado?: number): Promise<void> {
    if (!idEmpresa || idEmpresa <= 0) {
      this.roles = [];
      return;
    }
    this.menuRolService.obtenerRolesPorEmpresa(idEmpresa)
      .pipe(timeout(15000))
      .subscribe({
        next: (roles) => {
          this.roles = roles ?? [];
          if (idRolSeleccionado && this.roles.some(r => r.idRol === idRolSeleccionado)) {
            this.form.patchValue({ idRol: idRolSeleccionado });
          }
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudieron cargar los roles de la empresa.'))
      });
  }
}

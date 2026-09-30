import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LazyLoadEvent } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { finalize, timeout } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { SedeCrud } from 'src/app/models/sede-crud.interface';
import { SecurityService } from 'src/app/services/security.service';
import { SedeAdminService } from 'src/app/services/sede-admin.service';
import { mensajeErrorApi } from 'src/app/shared/utils/mc-api-error.util';
import { textoErrorCampo } from 'src/app/shared/utils/mc-form-error.util';

@Component({
  selector: 'app-sede',
  templateUrl: './sede.component.html',
  styleUrls: ['./sede.component.css'],
})
export class SedeComponent implements OnInit {
  sedes: SedeCrud[] = [];
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
    private sedeService: SedeAdminService,
    private securityService: SecurityService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(150)]],
      codigo: ['', [Validators.maxLength(30)]],
      direccion: ['', [Validators.maxLength(250)]],
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
    this.cargarSedes(pageNumber, pageSize);
  }

  buscar(): void {
    this.cargarSedes(1, this.rows);
  }

  limpiarBusqueda(): void {
    this.busqueda = '';
    this.cargarSedes(1, this.rows);
  }

  abrirNuevo(): void {
    this.editando = false;
    this.idEditando = 0;
    this.form.reset({
      nombre: '',
      codigo: '',
      direccion: '',
      activo: true,
    });
    this.modalVisible = true;
  }

  editar(item: SedeCrud): void {
    this.editando = true;
    this.idEditando = item.idSede;
    this.form.patchValue({
      nombre: item.nombre,
      codigo: item.codigo,
      direccion: item.direccion,
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

    const data: SedeCrud = {
      idSede: this.idEditando,
      idEmpresa: this.idEmpresaSesion,
      nombre: (this.form.value.nombre ?? '').trim(),
      codigo: (this.form.value.codigo ?? '').trim(),
      direccion: (this.form.value.direccion ?? '').trim(),
      activo: !!this.form.value.activo,
    };

    if (this.editando) {
      this.sedeService.actualizar(this.idEditando, data).subscribe({
        next: () => {
          this.toastr.success('Sede actualizada correctamente.');
          this.modalVisible = false;
          this.cargarSedes(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo actualizar la sede.')),
      });
      return;
    }

    this.sedeService.crear(data).subscribe({
      next: () => {
        this.toastr.success('Sede registrada correctamente.');
        this.modalVisible = false;
        this.cargarSedes(1, this.rows);
      },
      error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo registrar la sede.')),
    });
  }

  eliminar(item: SedeCrud): void {
    void Swal.fire({
      title: 'Confirmar eliminación',
      text: `¿Desea eliminar la sede "${item.nombre}"?`,
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

      this.sedeService.eliminar(item.idSede).subscribe({
        next: () => {
          this.toastr.success('Sede eliminada correctamente.');
          this.cargarSedes(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo eliminar la sede.')),
      });
    });
  }

  private cargarSedes(pageNumber: number, pageSize: number): void {
    this.loading = true;
    this.sedeService.obtenerPaginado(this.idEmpresaSesion, this.busqueda, pageNumber, pageSize)
      .pipe(
        timeout(15000),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (response) => {
          this.sedes = response?.items ?? [];
          this.totalRows = response?.totalRows ?? 0;
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener el listado de sedes.')),
      });
  }

}

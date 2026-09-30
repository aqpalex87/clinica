import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LazyLoadEvent } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { finalize, timeout } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { EmpresaCrud } from 'src/app/models/empresa-crud.interface';
import { EmpresaAdminService } from 'src/app/services/empresa-admin.service';
import { mensajeErrorApi } from 'src/app/shared/utils/mc-api-error.util';
import { rucOpcional, textoErrorCampo } from 'src/app/shared/utils/mc-form-error.util';

@Component({
  selector: 'app-empresa',
  templateUrl: './empresa.component.html',
  styleUrls: ['./empresa.component.css'],
})
export class EmpresaComponent implements OnInit {
  empresas: EmpresaCrud[] = [];
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
    private empresaService: EmpresaAdminService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(150)]],
      ruc: ['', [Validators.maxLength(11), rucOpcional]],
      activo: [true],
    });
  }

  ngOnInit(): void {
    this.cargarEmpresas(1, this.rows);
  }

  onLazyLoad(event: LazyLoadEvent): void {
    const first = event.first ?? 0;
    const pageSize = event.rows ?? this.rows;
    const pageNumber = Math.floor(first / pageSize) + 1;
    this.rows = pageSize;
    this.cargarEmpresas(pageNumber, pageSize);
  }

  buscar(): void {
    this.cargarEmpresas(1, this.rows);
  }

  limpiarBusqueda(): void {
    this.busqueda = '';
    this.cargarEmpresas(1, this.rows);
  }

  abrirNuevo(): void {
    this.editando = false;
    this.idEditando = 0;
    this.form.reset({
      nombre: '',
      ruc: '',
      activo: true,
    });
    this.modalVisible = true;
  }

  editar(item: EmpresaCrud): void {
    this.editando = true;
    this.idEditando = item.idEmpresa;
    this.form.patchValue({
      nombre: item.nombre,
      ruc: item.ruc,
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

    const data: EmpresaCrud = {
      idEmpresa: this.idEditando,
      nombre: (this.form.value.nombre ?? '').trim(),
      ruc: (this.form.value.ruc ?? '').trim(),
      activo: !!this.form.value.activo,
    };

    if (this.editando) {
      this.empresaService.actualizar(this.idEditando, data).subscribe({
        next: () => {
          this.toastr.success('Empresa actualizada correctamente.');
          this.modalVisible = false;
          this.cargarEmpresas(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo actualizar la empresa.')),
      });
      return;
    }

    this.empresaService.crear(data).subscribe({
      next: () => {
        this.toastr.success('Empresa registrada correctamente.');
        this.modalVisible = false;
        this.cargarEmpresas(1, this.rows);
      },
      error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo registrar la empresa.')),
    });
  }

  eliminar(item: EmpresaCrud): void {
    void Swal.fire({
      title: 'Confirmar eliminación',
      text: `¿Desea eliminar la empresa "${item.nombre}"?`,
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

      this.empresaService.eliminar(item.idEmpresa).subscribe({
        next: () => {
          this.toastr.success('Empresa eliminada correctamente.');
          this.cargarEmpresas(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo eliminar la empresa.')),
      });
    });
  }

  private cargarEmpresas(pageNumber: number, pageSize: number): void {
    this.loading = true;
    this.empresaService.obtenerPaginado(this.busqueda, pageNumber, pageSize)
      .pipe(
        timeout(15000),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (response) => {
          this.empresas = response?.items ?? [];
          this.totalRows = response?.totalRows ?? 0;
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener el listado de empresas.')),
      });
  }
}

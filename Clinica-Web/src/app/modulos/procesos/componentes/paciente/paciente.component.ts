import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LazyLoadEvent } from 'primeng/api';
import { ToastrService } from 'ngx-toastr';
import { finalize, timeout } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { PacienteCrud } from 'src/app/models/paciente-crud.interface';
import { PacienteAdminService } from 'src/app/services/paciente-admin.service';
import { SecurityService } from 'src/app/services/security.service';
import { mensajeErrorApi } from 'src/app/shared/utils/mc-api-error.util';
import { celularPeru, correoOpcional, documentoPaciente, fechaNacimientoValida, textoErrorCampo } from 'src/app/shared/utils/mc-form-error.util';

@Component({
  selector: 'app-paciente',
  templateUrl: './paciente.component.html',
  styleUrls: ['./paciente.component.css'],
})
export class PacienteComponent implements OnInit {
  pacientes: PacienteCrud[] = [];
  totalRows = 0;
  rows = 10;
  loading = false;
  busquedaNombre = '';
  busquedaDocumento = '';
  busquedaContacto = '';

  modalVisible = false;
  editando = false;
  idEditando = 0;

  idEmpresaSesion = 0;
  idSedeSesion = 0;
  empresaNombre = '';
  sedeNombre = '';
  form: FormGroup;

  tiposDocumento = [
    { valor: 'DNI', etiqueta: 'DNI' },
    { valor: 'CE', etiqueta: 'Carné de extranjería' },
    { valor: 'PAS', etiqueta: 'Pasaporte' },
    { valor: 'CPP', etiqueta: 'CPP' },
  ];

  constructor(
    private fb: FormBuilder,
    private pacienteService: PacienteAdminService,
    private securityService: SecurityService,
    private toastr: ToastrService
  ) {
    this.form = this.fb.group({
      tipoDocumento: ['DNI', [Validators.required]],
      numeroDocumento: ['', [Validators.required, Validators.maxLength(20), documentoPaciente(() => this.form?.get('tipoDocumento')?.value)]],
      apellidoPaterno: ['', [Validators.required, Validators.maxLength(80)]],
      apellidoMaterno: ['', [Validators.maxLength(80)]],
      nombres: ['', [Validators.required, Validators.maxLength(120)]],
      fechaNacimiento: ['', [Validators.required, fechaNacimientoValida]],
      sexo: ['M', [Validators.required]],
      celular: ['', [Validators.required, Validators.maxLength(9), celularPeru]],
      correo: ['', [Validators.maxLength(120), correoOpcional]],
      direccion: ['', [Validators.maxLength(200)]],
      observacion: ['', [Validators.maxLength(300)]],
      activo: [true],
    });
  }

  ngOnInit(): void {
    this.idEmpresaSesion = Number(this.securityService.leerIdEmpresa() || 0);
    this.idSedeSesion = Number(this.securityService.leerIdSede() || 0);
    this.empresaNombre = this.securityService.leerEmpresa() || '-';
    this.sedeNombre = this.securityService.leerSede() || '-';
  }

  onLazyLoad(event: LazyLoadEvent): void {
    const first = event.first ?? 0;
    const pageSize = event.rows ?? this.rows;
    const pageNumber = Math.floor(first / pageSize) + 1;
    this.rows = pageSize;
    this.cargarPacientes(pageNumber, pageSize);
  }

  buscar(): void {
    this.cargarPacientes(1, this.rows);
  }

  limpiarBusqueda(): void {
    this.busquedaNombre = '';
    this.busquedaDocumento = '';
    this.busquedaContacto = '';
    this.cargarPacientes(1, this.rows);
  }

  abrirNuevo(): void {
    this.editando = false;
    this.idEditando = 0;
    this.form.reset({
      tipoDocumento: 'DNI',
      numeroDocumento: '',
      apellidoPaterno: '',
      apellidoMaterno: '',
      nombres: '',
      fechaNacimiento: '',
      sexo: 'M',
      celular: '',
      correo: '',
      direccion: '',
      observacion: '',
      activo: true,
    });
    this.modalVisible = true;
  }

  editar(item: PacienteCrud): void {
    this.editando = true;
    this.idEditando = item.idPaciente;
    this.form.reset({
      tipoDocumento: item.tipoDocumento,
      numeroDocumento: item.numeroDocumento,
      apellidoPaterno: item.apellidoPaterno,
      apellidoMaterno: item.apellidoMaterno ?? '',
      nombres: item.nombres,
      fechaNacimiento: this.aFechaInput(item.fechaNacimiento),
      sexo: item.sexo,
      celular: item.celular,
      correo: item.correo ?? '',
      direccion: item.direccion ?? '',
      observacion: item.observacion ?? '',
      activo: item.activo,
    });
    this.modalVisible = true;
  }

  alCambiarTipoDocumento(): void {
    this.form.get('numeroDocumento')?.updateValueAndValidity();
  }

  mensajeCampo(nombre: string): string {
    return textoErrorCampo(this.form, nombre);
  }

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const data: PacienteCrud = {
      idPaciente: this.idEditando,
      idEmpresa: this.idEmpresaSesion,
      idSede: this.idSedeSesion,
      tipoDocumento: (this.form.value.tipoDocumento ?? '').trim(),
      numeroDocumento: (this.form.value.numeroDocumento ?? '').trim(),
      apellidoPaterno: (this.form.value.apellidoPaterno ?? '').trim(),
      apellidoMaterno: (this.form.value.apellidoMaterno ?? '').trim(),
      nombres: (this.form.value.nombres ?? '').trim(),
      fechaNacimiento: this.form.value.fechaNacimiento,
      sexo: this.form.value.sexo,
      celular: String(this.form.value.celular ?? '').replace(/\D/g, ''),
      correo: (this.form.value.correo ?? '').trim(),
      direccion: (this.form.value.direccion ?? '').trim(),
      observacion: (this.form.value.observacion ?? '').trim(),
      activo: !!this.form.value.activo,
    };

    if (this.editando) {
      this.pacienteService.actualizar(this.idEditando, data).subscribe({
        next: () => {
          this.toastr.success('Paciente actualizado correctamente.');
          this.modalVisible = false;
          this.cargarPacientes(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo actualizar el paciente.')),
      });
      return;
    }

    this.pacienteService.crear(data).subscribe({
      next: () => {
        this.toastr.success('Paciente registrado correctamente.');
        this.modalVisible = false;
        this.cargarPacientes(1, this.rows);
      },
      error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo registrar el paciente.')),
    });
  }

  eliminar(item: PacienteCrud): void {
    void Swal.fire({
      title: 'Confirmar eliminación',
      text: `¿Desea eliminar al paciente "${item.nombreCompleto || item.nombres}"?`,
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

      this.pacienteService.eliminar(item.idPaciente).subscribe({
        next: () => {
          this.toastr.success('Paciente eliminado correctamente.');
          this.cargarPacientes(1, this.rows);
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo eliminar el paciente.')),
      });
    });
  }

  etiquetaSexo(sexo: string): string {
    return sexo === 'F' ? 'Femenino' : sexo === 'M' ? 'Masculino' : sexo;
  }

  private cargarPacientes(pageNumber: number, pageSize: number): void {
    this.loading = true;
    this.pacienteService
      .obtenerPaginado(this.idEmpresaSesion, this.busquedaNombre, this.busquedaDocumento, this.busquedaContacto, pageNumber, pageSize)
      .pipe(
        timeout(15000),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (response) => {
          this.pacientes = response?.items ?? [];
          this.totalRows = response?.totalRows ?? 0;
        },
        error: (err) => this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener el listado de pacientes.')),
      });
  }

  private aFechaInput(valor?: string): string {
    if (!valor) {
      return '';
    }
    return valor.toString().substring(0, 10);
  }
}

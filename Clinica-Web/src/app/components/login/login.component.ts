import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { SecurityService } from 'src/app/services/security.service';
import { DataSeguridad } from 'src/app/models/dataseguridad.interface';
import { ToastrService } from 'ngx-toastr';
import { timeout } from 'rxjs/operators';
import { EmpresaAuth, SedeAuth } from 'src/app/models/empresa-sede.interface';
import { mensajeErrorApi } from 'src/app/shared/utils/mc-api-error.util';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  codigoUsuario = '';
  password = '';
  idSede: number | null = null;
  sedes: SedeAuth[] = [];
  empresaLogin: EmpresaAuth | null = null;
  loading = false;
  sedeModalVisible = false;
  private returnUrl = '/home';
  private loginResponse: any = null;

  constructor(
    private securityService: SecurityService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {
    this.route.queryParams.subscribe((p) => {
      if (p['returnUrl']) {
        this.returnUrl = p['returnUrl'];
      }
    });
  }

  ngOnInit(): void {
    if (this.securityService.leerTokenMC()) {
      void this.router.navigateByUrl(this.returnUrl || '/home');
    }
  }

  iniciarSesion(): void {
    const c = this.codigoUsuario?.trim();
    if (!c || !this.password) {
      this.toastr.warning('Ingrese usuario y contraseña.', 'Datos incompletos');
      return;
    }

    this.loading = true;
    this.securityService.loginApi(c, this.password).pipe(timeout(15000)).subscribe({
      next: (result: any) => {
        const response = result.response;
        const token = response?.token || result.xhr?.getResponseHeader?.('authorization');
        const idEmpresaLogin = Number(response?.idEmpresa ?? response?.IdEmpresa ?? 0);
        const empresaNombreLogin = (response?.empresa ?? response?.Empresa ?? '-').toString();
        if (!token) {
          this.loading = false;
          this.toastr.error('No se recibió token de autenticación.', 'Error');
          return;
        }

        this.securityService.guardarTokenMC(token);
        this.securityService.guardaridPerfil(String(response?.idRol ?? response?.IdRol ?? '1'));
        this.securityService.guardarIdEmpresa(String(idEmpresaLogin));
        this.securityService.guardarPerfil(response?.rol ?? response?.Rol ?? 'Usuario');
        this.loginResponse = response;
        this.empresaLogin = {
          idEmpresa: idEmpresaLogin,
          nombre: empresaNombreLogin,
        };
        this.cargarSedes(idEmpresaLogin);
      },
      error: (err) => {
        this.loading = false;
        this.toastr.error(mensajeErrorApi(err, 'No se pudo iniciar sesión (credenciales, timeout o red).'), 'Error de acceso');
      },
    });
  }

  private cargarSedes(idEmpresa: number): void {
    if (!idEmpresa || idEmpresa <= 0) {
      this.loading = false;
      this.toastr.error('El usuario no tiene empresa asociada.');
      return;
    }
    this.securityService.obtenerSedesPorEmpresaApi(idEmpresa).pipe(timeout(15000)).subscribe({
      next: (res: any) => {
        const rawSedes = res?.response ?? [];
        this.sedes = rawSedes
          .map((x: any) => ({
            idSede: Number(x?.idSede ?? x?.IdSede ?? 0),
            idEmpresa: Number(x?.idEmpresa ?? x?.IdEmpresa ?? idEmpresa),
            nombre: (x?.nombre ?? x?.Nombre ?? '').toString(),
          }))
          .filter((x: SedeAuth) => x.idSede > 0 && !!x.nombre);
        this.idSede = null;
        if (this.sedes.length === 0) {
          this.loading = false;
          this.toastr.warning('La empresa no tiene sedes configuradas.');
          return;
        }
        this.sedeModalVisible = true;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.sedes = [];
        this.idSede = null;
        this.toastr.error(mensajeErrorApi(err, 'No se pudo obtener la lista de sedes.'));
      }
    });
  }

  confirmarSede(): void {
    if (!this.idSede) {
      this.toastr.warning('Seleccione una sede para continuar.');
      return;
    }
    const c = this.codigoUsuario?.trim();
    const sedeNombre = this.sedes.find(x => x.idSede === this.idSede)?.nombre ?? '-';
    const data: DataSeguridad = {
      vEmail: `${c}@local`,
      vNombre: this.loginResponse?.usuario ?? this.loginResponse?.Usuario ?? c,
      vUsuarioWeb: this.loginResponse?.usuario ?? this.loginResponse?.Usuario ?? c,
      vDescripcionAgencia: '-',
      vEmpresa: this.loginResponse?.empresa ?? this.loginResponse?.Empresa ?? this.empresaLogin?.nombre ?? '-',
      vSede: sedeNombre,
    };
    this.securityService.guardarDataSeguridad(data);
    this.sedeModalVisible = false;
    this.loading = true;
    this.securityService.seleccionarSedeApi(Number(this.idSede)).pipe(timeout(15000)).subscribe({
      next: (tokenRes: any) => {
        const response = tokenRes?.response;
        const token = response?.token || tokenRes?.xhr?.getResponseHeader?.('authorization');
        if (token) {
          this.securityService.guardarTokenMC(token);
        }
        this.securityService.obtenerMenusApi().pipe(timeout(15000)).subscribe({
          next: (menusRes: any) => {
            this.securityService.guardarMenusApi(menusRes.response ?? []);
            this.loading = false;
            this.toastr.success('Bienvenido', 'Sesion iniciada');
            void this.router.navigateByUrl(this.returnUrl || '/home');
          },
          error: (err) => {
            this.loading = false;
            this.toastr.error(mensajeErrorApi(err, 'No se pudieron cargar los menus (timeout o error de red).'), 'Error');
          }
        });
      },
      error: (err) => {
        this.loading = false;
        this.toastr.error(mensajeErrorApi(err, 'No se pudo asociar sede a la sesión.'), 'Error');
      }
    });
  }
}

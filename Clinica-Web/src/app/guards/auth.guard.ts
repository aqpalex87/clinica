import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { Router } from '@angular/router';
import { DomService } from '../services/dom.service';
import { SecurityService } from '../services/security.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { ToastrService } from 'ngx-toastr';
import { mensajeErrorApi } from '../shared/utils/mc-api-error.util';

@Injectable()
export class AuthGuard implements CanActivate {
  public loader = false;

  constructor(
    private securityService: SecurityService,
    private router: Router,
    private domService: DomService,
    private toastr: ToastrService
  ) {}

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    this.loader = true;
    this.domService.ShowLoading();

    const token = this.securityService.leerTokenMC();
    if (!token) {
      this.domService.HideLoading();
      await this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return false;
    }

    const helper = new JwtHelperService();
    const rawToken = token.startsWith('Bearer ') ? token.substring(7) : token;

    try {
      if (helper.isTokenExpired(rawToken)) {
        this.securityService.cerrarSesion();
        this.toastr.error('Sesion expirada. Vuelva a iniciar sesion.', 'Error');
        this.domService.HideLoading();
        await this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
      }
    } catch {
      this.securityService.cerrarSesion();
      this.domService.HideLoading();
      await this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return false;
    }

    if (this.securityService.leerMenusApi().length === 0) {
      try {
        const menus: any = await this.securityService.obtenerMenusApi().toPromise();
        this.securityService.guardarMenusApi(menus?.response ?? []);
      } catch (err) {
        this.securityService.cerrarSesion();
        this.toastr.error(mensajeErrorApi(err, 'No se pudieron cargar los menus del usuario.'), 'Error');
        this.domService.HideLoading();
        await this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
      }
    }

    this.domService.HideLoading();
    return true;
  }
}
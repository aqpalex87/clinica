import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MCApiRoutes } from 'src/app/core/MC-api.routes';
import { GuardarMenuRolRequest, MenuRolTree, RolSimple } from '../models/menu-rol.interface';
import { SecurityService } from './security.service';

@Injectable({ providedIn: 'root' })
export class MenuRolAdminService {
  private readonly controller = MCApiRoutes.menuroles();

  constructor(private http: HttpClient, private securityService: SecurityService) {}

  obtenerRolesPorEmpresa(idEmpresa: number): Observable<RolSimple[]> {
    return this.http.get<RolSimple[]>(`${this.controller}/roles/${idEmpresa}`, { headers: this.getHeaders() });
  }

  obtenerMenusPorRol(idRol: number): Observable<MenuRolTree[]> {
    return this.http.get<MenuRolTree[]>(`${this.controller}/menus/${idRol}`, { headers: this.getHeaders() });
  }

  guardar(data: GuardarMenuRolRequest): Observable<{ guardado: boolean }> {
    return this.http.post<{ guardado: boolean }>(`${this.controller}/guardar`, data, { headers: this.getHeaders() });
  }

  private getHeaders(): HttpHeaders {
    return this.securityService.MCHttpJsonHeaders();
  }
}

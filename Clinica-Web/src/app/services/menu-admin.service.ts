import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MCApiRoutes } from 'src/app/core/MC-api.routes';
import { normalizeMCPaginated } from 'src/app/shared/utils/mc-paginated.util';
import { MenuCrud, MenuPaginadoResponse } from '../models/menu-crud.interface';
import { SecurityService } from './security.service';

@Injectable({ providedIn: 'root' })
export class MenuAdminService {
  private readonly controller = MCApiRoutes.menu();

  constructor(private http: HttpClient, private securityService: SecurityService) {}

  obtenerPaginado(nombre: string, pageNumber: number, pageSize: number): Observable<MenuPaginadoResponse> {
    const params = new HttpParams()
      .set('nombre', nombre ?? '')
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<unknown>(this.controller, { params, headers: this.getHeaders() }).pipe(
      map((body) => normalizeMCPaginated<MenuCrud>(body))
    );
  }

  obtenerPadres(): Observable<MenuCrud[]> {
    return this.http.get<MenuCrud[]>(`${this.controller}/padres`, { headers: this.getHeaders() });
  }

  crear(data: MenuCrud): Observable<MenuCrud> {
    return this.http.post<MenuCrud>(this.controller, data, { headers: this.getHeaders() });
  }

  actualizar(idMenu: number, data: MenuCrud): Observable<MenuCrud> {
    return this.http.put<MenuCrud>(`${this.controller}/${idMenu}`, data, { headers: this.getHeaders() });
  }

  eliminar(idMenu: number): Observable<{ eliminado: boolean }> {
    return this.http.delete<{ eliminado: boolean }>(`${this.controller}/${idMenu}`, { headers: this.getHeaders() });
  }

  private getHeaders(): HttpHeaders {
    return this.securityService.MCHttpJsonHeaders();
  }
}

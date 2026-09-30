import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MCApiRoutes } from 'src/app/core/MC-api.routes';
import { normalizeMCPaginated } from 'src/app/shared/utils/mc-paginated.util';
import { UsuarioCrud, UsuarioPaginadoResponse } from '../models/usuario-crud.interface';
import { SecurityService } from './security.service';

@Injectable({ providedIn: 'root' })
export class UsuarioAdminService {
  private readonly controller = MCApiRoutes.usuarios();

  constructor(private http: HttpClient, private securityService: SecurityService) {}

  obtenerPaginado(idEmpresa: number | null, usuario: string, pageNumber: number, pageSize: number): Observable<UsuarioPaginadoResponse> {
    let params = new HttpParams()
      .set('usuario', usuario ?? '')
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    if (idEmpresa && idEmpresa > 0) {
      params = params.set('idEmpresa', idEmpresa.toString());
    }
    return this.http.get<unknown>(this.controller, { params, headers: this.getHeaders() }).pipe(
      map((body) => normalizeMCPaginated<UsuarioCrud>(body))
    );
  }

  crear(data: UsuarioCrud): Observable<UsuarioCrud> {
    return this.http.post<UsuarioCrud>(this.controller, data, { headers: this.getHeaders() });
  }

  actualizar(idUsuario: number, data: UsuarioCrud): Observable<UsuarioCrud> {
    return this.http.put<UsuarioCrud>(`${this.controller}/${idUsuario}`, data, { headers: this.getHeaders() });
  }

  eliminar(idUsuario: number): Observable<{ eliminado: boolean }> {
    return this.http.delete<{ eliminado: boolean }>(`${this.controller}/${idUsuario}`, { headers: this.getHeaders() });
  }

  private getHeaders(): HttpHeaders {
    return this.securityService.MCHttpJsonHeaders();
  }
}

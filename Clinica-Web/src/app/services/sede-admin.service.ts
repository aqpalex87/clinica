import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MCApiRoutes } from 'src/app/core/MC-api.routes';
import { normalizeMCPaginated } from 'src/app/shared/utils/mc-paginated.util';
import { SedeCrud, SedePaginadoResponse } from '../models/sede-crud.interface';
import { SecurityService } from './security.service';

@Injectable({ providedIn: 'root' })
export class SedeAdminService {
  private readonly controller = MCApiRoutes.sedes.base();

  constructor(private http: HttpClient, private securityService: SecurityService) {}

  obtenerPorEmpresa(idEmpresa: number): Observable<SedeCrud[]> {
    return this.http.get<unknown>(`${this.controller}/empresa/${idEmpresa}`, { headers: this.securityService.MCHttpJsonHeaders() }).pipe(
      map((list) =>
        (Array.isArray(list) ? list : []).map(
          (x: Record<string, unknown>) =>
            ({
              idSede: Number(x['idSede'] ?? x['IdSede'] ?? 0),
              idEmpresa: Number(x['idEmpresa'] ?? x['IdEmpresa'] ?? idEmpresa),
              nombreEmpresa: String(x['nombreEmpresa'] ?? x['NombreEmpresa'] ?? ''),
              nombre: String(x['nombre'] ?? x['Nombre'] ?? ''),
              codigo: String(x['codigo'] ?? x['Codigo'] ?? ''),
              direccion: String(x['direccion'] ?? x['Direccion'] ?? ''),
              activo: Boolean(x['activo'] ?? x['Activo'] ?? true),
            }) as SedeCrud
        )
      )
    );
  }

  obtenerPaginado(idEmpresa: number | null, nombre: string, pageNumber: number, pageSize: number): Observable<SedePaginadoResponse> {
    let params = new HttpParams()
      .set('nombre', nombre ?? '')
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    if (idEmpresa && idEmpresa > 0) {
      params = params.set('idEmpresa', idEmpresa.toString());
    }
    return this.http.get<unknown>(this.controller, { params, headers: this.getHeaders() }).pipe(
      map((body) => normalizeMCPaginated<SedeCrud>(body))
    );
  }

  crear(data: SedeCrud): Observable<SedeCrud> {
    return this.http.post<SedeCrud>(this.controller, data, { headers: this.getHeaders() });
  }

  actualizar(idSede: number, data: SedeCrud): Observable<SedeCrud> {
    return this.http.put<SedeCrud>(`${this.controller}/${idSede}`, data, { headers: this.getHeaders() });
  }

  eliminar(idSede: number): Observable<{ eliminado: boolean }> {
    return this.http.delete<{ eliminado: boolean }>(`${this.controller}/${idSede}`, { headers: this.getHeaders() });
  }

  private getHeaders(): HttpHeaders {
    return this.securityService.MCHttpJsonHeaders();
  }
}

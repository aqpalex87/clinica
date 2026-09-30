import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MCApiRoutes } from 'src/app/core/MC-api.routes';
import { normalizeMCPaginated } from 'src/app/shared/utils/mc-paginated.util';
import { PacienteCrud, PacientePaginadoResponse } from '../models/paciente-crud.interface';
import { SecurityService } from './security.service';

@Injectable({ providedIn: 'root' })
export class PacienteAdminService {
  private readonly controller = MCApiRoutes.pacientes();

  constructor(private http: HttpClient, private securityService: SecurityService) {}

  obtenerPaginado(
    idEmpresa: number | null,
    nombre: string,
    numeroDocumento: string,
    contacto: string,
    pageNumber: number,
    pageSize: number
  ): Observable<PacientePaginadoResponse> {
    let params = new HttpParams()
      .set('nombre', nombre ?? '')
      .set('numeroDocumento', numeroDocumento ?? '')
      .set('contacto', contacto ?? '')
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    if (idEmpresa && idEmpresa > 0) {
      params = params.set('idEmpresa', idEmpresa.toString());
    }
    return this.http.get<unknown>(this.controller, { params, headers: this.getHeaders() }).pipe(
      map((body) => normalizeMCPaginated<PacienteCrud>(body))
    );
  }

  crear(data: PacienteCrud): Observable<PacienteCrud> {
    return this.http.post<PacienteCrud>(this.controller, data, { headers: this.getHeaders() });
  }

  actualizar(idPaciente: number, data: PacienteCrud): Observable<PacienteCrud> {
    return this.http.put<PacienteCrud>(`${this.controller}/${idPaciente}`, data, { headers: this.getHeaders() });
  }

  eliminar(idPaciente: number): Observable<{ eliminado: boolean }> {
    return this.http.delete<{ eliminado: boolean }>(`${this.controller}/${idPaciente}`, { headers: this.getHeaders() });
  }

  private getHeaders(): HttpHeaders {
    return this.securityService.MCHttpJsonHeaders();
  }
}

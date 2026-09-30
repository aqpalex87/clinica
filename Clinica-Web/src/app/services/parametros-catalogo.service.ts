import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ParametroListaItem } from '../models/parametro-lista.interface';
import { SecurityService } from './security.service';

@Injectable({ providedIn: 'root' })
export class ParametrosCatalogoService {
  /* Debe coincidir con el segmento de ruta del API: ParametrosCatalogoController → "ParametrosCatalogo" (no kebab-case). */
  private readonly baseUrl = `${environment.UrlBase_MCAPI}/ParametrosCatalogo`;

  constructor(private http: HttpClient, private securityService: SecurityService) {}

  obtenerPorTipo(codigoTipo: string): Observable<ParametroListaItem[]> {
    const enc = encodeURIComponent(codigoTipo);
    return this.http.get<ParametroListaItem[]>(`${this.baseUrl}/por-tipo/${enc}`, { headers: this.getHeaders() });
  }

  private getHeaders(): HttpHeaders {
    return this.securityService.MCHttpJsonHeaders();
  }
}

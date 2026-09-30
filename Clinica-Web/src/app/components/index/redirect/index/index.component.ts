import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { SecurityService } from 'src/app/services/security.service';
import { environment } from 'src/environments/environment';
import { DataSeguridad } from '../../../../models/dataseguridad.interface';
import { Usuario } from '../../../../models/usuario.interface';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css'],
})
export class IndexComponent implements OnInit {
  private usuario: any;
  constructor(
    private router: ActivatedRoute,
    private _router: Router,
    private securityService: SecurityService
  ) {}

  ngOnInit(): void {
    this.router.queryParamMap.subscribe((result: ParamMap) => {

      const pToken: string | null = result.get('Token');
      const pIdPerfil: string | null = result.get('IdPerfil');

      if (pToken == null || pIdPerfil == null) {
        this._router.navigate(['error']);
      }

      this.usuario = {
        Token: pToken,
        idPerfil: pIdPerfil,
        UsuarioWeb: '',
      };

      this.securityService.guardarToken(pToken);
      this.securityService.guardaridPerfil(pIdPerfil);
    });

    

    forkJoin([
      this.securityService.obtenerUsuario(this.usuario as Usuario)
    ]).subscribe({
      next: data => {
        let dataResultado = data[0];
        let response = dataResultado['response'];
        this.securityService.guardarDataSeguridad(response);
        this.usuario.UsuarioWeb = response.vUsuarioWeb;
        this.securityService.autenticarMC().subscribe({
          next: result => {
            let tokenMC = result.xhr.getResponseHeader("authorization");
            this.securityService.guardarTokenMC(tokenMC);

            window.location.href = environment.UrlBase_MC;
          },
          error: () => {
            this._router.navigate(['error']);
          }
        });
        this._router.navigate(['/']);
      },
      error: () => {
        this._router.navigate(['error']);
      }
    });

    //this._router.navigate(['/']);
  }
}

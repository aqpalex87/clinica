import { Component, EventEmitter, OnInit } from '@angular/core';
import { DomService } from 'src/app/services/dom.service';
import { SecurityService } from 'src/app/services/security.service';
import { AuthSubjectService } from 'src/app/services/subjects/auth-subject.service';
import { MenuApi } from '../../models/menu-api.interface';
declare var $: any;
declare var M: any;
@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  ListaOpciones: any[] = [];
  SecurityModulos: MenuApi[] = [];
  isLoaded: boolean = false;

  constructor(
    public domService: DomService,
    public _securityService: SecurityService,
    private authSubjectService: AuthSubjectService
  ) {
    //$('body').on('click', '.sidenav-overlay', this.domService.HideSideBar);
  }

  ngOnInit() {

    this.authSubjectService.isUserLoaded$.subscribe(value => {
      if (value) {
        this.isLoaded = true;
        this.modulos();
      }
    });

    setTimeout(() => {
      if (!this.isLoaded && localStorage.getItem('idperfil') != null && localStorage.getItem('UsuarioWeb') != null) {
        this.modulos();
      }
    }, 2500);
  }

  public modulos() {
    this.inicializarEventos();
    this.SecurityModulos = this._securityService.leerMenusApi() ?? [];
    this._securityService.descripcionPerfil$.emit(this._securityService.leerPerfil());
    setTimeout(() => this.inicializarControles(), 0);
  }



  inicializarControles(): void {
    const elems = document.querySelectorAll('#slide-out.sidenav.collapsible, #slide-out .nested-menu.collapsible');
    M.Collapsible.init(elems, {});
  }

  sideNavOverlayClick(): void {
    this.domService.HideSideBar();
  }

  inicializarEventos() {
    //$("body").on("click", ".sidenav-overlay", this.sideNavOverlayClick);
  }

  clickSlideOut() {
    this.domService.ShowSideBar();
  }

  tieneHijos(menu: MenuApi): boolean {
    return !!menu.hijos && menu.hijos.length > 0;
  }

  obtenerRutaOpcion(menu: MenuApi) {
    if (!menu.ruta) {
      return '/home';
    }
    return menu.ruta.startsWith('/') ? menu.ruta : `/${menu.ruta}`;
  }

}

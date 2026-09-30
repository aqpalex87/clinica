import { Component, ElementRef, HostListener, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DomService } from 'src/app/services/dom.service';
import { SecurityService } from 'src/app/services/security.service';
import { AuthSubjectService } from 'src/app/services/subjects/auth-subject.service';
import $ from 'jquery';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: [ './navbar.component.css' ],
})
export class NavbarComponent implements OnInit {
  MostrarCerrarSesion: boolean = false;
  nombreUsuario!: string;
  descripcionPerfil!: string;
  usuario!: string;
  fechaAcceso!: string;
  perfil!: string;
  empresa!: string;
  sede!: string;
  menuUsuarioAbierto = false;

  constructor(private domService: DomService,
    private _securityService: SecurityService,
    private _router: Router,
    private _authSubjectService: AuthSubjectService,
    private elementRef: ElementRef) {

  }

  ngOnInit(): void {

    this.nombreUsuario = this._securityService.leerUsuarioWeb();
    this.usuario = this._securityService.leerUsuarioWeb();
    this.empresa = this._securityService.leerEmpresa();
    this.sede = this._securityService.leerSede();
    this.perfil = this._securityService.leerPerfil();
    this.fechaAcceso = new Date().toString();

    this._securityService.descripcionPerfil$.subscribe(perfil => {
      this.perfil = perfil;
    });

    this._authSubjectService.isUserLoaded$.subscribe(val => {
      if (val) {
        this.nombreUsuario = this._securityService.leerUsuarioWeb();
      }
    })

  }

  // MenuOpenClick(): void {
  //   this.domService.ShowSideBar();
  // }

  MenuOpenClick(): void {
    if (this.isMenuOpen())
      this.domService.HideSideBar();
    else
      this.domService.ShowSideBar();
  }

  isMenuOpen(): boolean {
    return $("body").hasClass("menu-open");
  }

  toggleMenuUsuario(): void {
    this.menuUsuarioAbierto = !this.menuUsuarioAbierto;
  }

  cerrarMenuUsuario(): void {
    this.menuUsuarioAbierto = false;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.menuUsuarioAbierto = false;
    }
  }

  cerrarSesion(): void {
    this.menuUsuarioAbierto = false;
    this._securityService.cerrarSesion();
    this.domService.HideSideBar();
    this._authSubjectService.setLoadedUser(false);
    void this._router.navigate(['/login']);
  }
}

import { Component, OnInit } from '@angular/core';
import { DomService } from 'src/app/services/dom.service';
import { SecurityService } from 'src/app/services/security.service';
import { SidebarComponent } from '../../shared/sidebar/sidebar.component';
declare var $:any;
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [SidebarComponent]
})
export class HomeComponent implements OnInit {

  loading:boolean;
  constructor(
    private _securityService: SecurityService,
    private sidebar: SidebarComponent,
    private domService: DomService
  ) { }

  ngOnInit(): void {
        this.CargarModulos(); 
    this.loading = true;
    setTimeout(() => {
      this.loading = false;
    }, 5000);
    $("body").addClass("menu-open");
  }

  CargarModulos() {
    this.domService.ShowLoading();
    this._securityService.leerMenusApi();
    this._securityService.descripcionPerfil$.emit(this._securityService.leerPerfil());
    setTimeout(() => {
      this.domService.HideLoading();
    }, 3000);

  }

}

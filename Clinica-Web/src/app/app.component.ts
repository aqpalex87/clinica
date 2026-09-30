import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { DomService } from './services/dom.service';
import { SecurityService } from "./services/security.service";
import { AuthGuard } from './guards/auth.guard';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: [ './app.component.css' ]
})
export class AppComponent implements OnInit {
  isSession: string;
  anio = new Date().getFullYear();

  constructor(
    public domService: DomService,
    private securityService: SecurityService,
    public authGuard: AuthGuard,
    private router: Router
  ) { }

  hideSideBar() {
    this.domService.HideSideBar();
  }

  ngOnInit() {
    this.refreshSession();
    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => this.refreshSession());
  }

  private refreshSession(): void {
    const hasSession = !!this.securityService.leerTokenMC();
    this.isSession = hasSession ? '1' : '';
    if (hasSession) {
      this.domService.ShowSideBar();
    } else {
      this.domService.HideSideBar();
    }
  }
}

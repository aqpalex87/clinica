import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { HomeComponent } from './components/home/home.component';
import { IndexComponent } from './components/index/redirect/index/index.component';
import { LoginComponent } from './components/login/login.component';


const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: 'mantenimiento-parametros',
    canActivate:[AuthGuard],
    loadChildren: () =>
      import('./modulos/mantenimiento/mantenimiento.module').then(
        (authentication) => authentication.MantenimientoModule
      ),
  },
  {
    path: 'procesos',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./modulos/procesos/procesos.module').then(
        (modulo) => modulo.ProcesosModule
      ),
  },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'Redirect/Index', component: IndexComponent },
  { path: '**', pathMatch: 'full', redirectTo: 'login' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

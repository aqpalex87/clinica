import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IndexComponent } from './componentes/index/index.component';
import { MenuComponent } from './componentes/menu/menu.component';
import { SedeComponent } from './componentes/sede/sede.component';
import { EmpresaComponent } from './componentes/empresa/empresa.component';
import { RolComponent } from './componentes/rol/rol.component';
import { MenuRolComponent } from './componentes/menu-rol/menu-rol.component';
import { UsuarioComponent } from './componentes/usuario/usuario.component';

const routes: Routes = [
  {
    path: '',
    component: IndexComponent,
  },
  {
    path: 'menus',
    component: MenuComponent
  },
  {
    path: 'sedes',
    component: SedeComponent
  },
  {
    path: 'empresas',
    component: EmpresaComponent
  },
  {
    path: 'roles',
    component: RolComponent
  },
  {
    path: 'menu-roles',
    component: MenuRolComponent
  },
  {
    path: 'usuarios',
    component: UsuarioComponent
  },
  {
    path: '**',
    redirectTo: ''
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MantenimientoRoutingModule { }

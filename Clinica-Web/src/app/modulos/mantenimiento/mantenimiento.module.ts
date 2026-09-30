import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MantenimientoRoutingModule } from './mantenimiento-routing.module';
import { IndexComponent } from './componentes/index/index.component';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { FormsModule, ReactiveFormsModule  } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { PanelModule } from 'primeng/panel';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { TooltipModule } from 'primeng/tooltip';
import { MenuComponent } from './componentes/menu/menu.component';
import { SedeComponent } from './componentes/sede/sede.component';
import { EmpresaComponent } from './componentes/empresa/empresa.component';
import { RolComponent } from './componentes/rol/rol.component';
import { MenuRolComponent } from './componentes/menu-rol/menu-rol.component';
import { TreeModule } from 'primeng/tree';
import { UsuarioComponent } from './componentes/usuario/usuario.component';

import { SharedModule } from 'src/app/shared/shared.module';
@NgModule({
  declarations: [
    IndexComponent,
    MenuComponent,
    SedeComponent,
    EmpresaComponent,
    RolComponent,
    MenuRolComponent,
    UsuarioComponent
  ],
  imports: [
    CommonModule,
    MantenimientoRoutingModule,
    TableModule,
    ToastModule,
    FormsModule,
    DropdownModule,
    ButtonModule,
    InputTextModule,
    CardModule,
    CheckboxModule,
    DialogModule,
    InputNumberModule,
    ReactiveFormsModule,
    TooltipModule,
    PanelModule,
    TreeModule,
    SharedModule
  ]
})
export class MantenimientoModule { }

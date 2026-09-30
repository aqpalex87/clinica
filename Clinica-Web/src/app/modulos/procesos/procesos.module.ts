import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { SharedModule } from 'src/app/shared/shared.module';
import { PacienteComponent } from './componentes/paciente/paciente.component';
import { ProcesosRoutingModule } from './procesos-routing.module';

@NgModule({
  declarations: [
    PacienteComponent
  ],
  imports: [
    CommonModule,
    ProcesosRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    DialogModule,
    SharedModule
  ]
})
export class ProcesosModule { }

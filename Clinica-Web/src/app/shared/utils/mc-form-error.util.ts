import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';

const MENSAJES: Record<string, string> = {
  required: 'Este campo es obligatorio.',
  celular: 'El celular debe tener 9 dígitos y empezar con 9.',
  correo: 'Correo no válido.',
  fecha: 'La fecha de nacimiento no es válida.',
  ruc: 'El RUC debe tener 11 dígitos.',
};

export function textoErrorCampo(form: FormGroup, nombre: string): string {
  const control = form.get(nombre);
  if (!control || !control.errors || !(control.touched || control.dirty)) {
    return '';
  }

  const error = control.errors;
  if (typeof error['documento'] === 'string') {
    return error['documento'];
  }
  if (error['maxlength']) {
    return `Máximo ${error['maxlength'].requiredLength} caracteres.`;
  }

  const clave = Object.keys(MENSAJES).find((item) => error[item]);
  return clave ? MENSAJES[clave] : 'Dato no válido.';
}

export function documentoPaciente(tipo: () => string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const documento = String(control.value ?? '').trim().toUpperCase();
    if (!documento) {
      return null;
    }

    const tipoDocumento = String(tipo() ?? '').trim().toUpperCase();
    if (tipoDocumento === 'DNI' && !/^\d{8}$/.test(documento)) {
      return { documento: 'El DNI debe tener 8 dígitos.' };
    }
    if (tipoDocumento === 'PAS' && !/^[A-Z0-9]{5,12}$/.test(documento)) {
      return { documento: 'El pasaporte debe tener entre 5 y 12 caracteres.' };
    }
    if ((tipoDocumento === 'CE' || tipoDocumento === 'CPP') && !/^[A-Z0-9]{6,12}$/.test(documento)) {
      return { documento: 'El documento debe tener entre 6 y 12 caracteres.' };
    }
    return null;
  };
}

export function celularPeru(control: AbstractControl): ValidationErrors | null {
  const celular = String(control.value ?? '').replace(/\D/g, '');
  if (!celular) {
    return null;
  }
  return /^9\d{8}$/.test(celular) ? null : { celular: true };
}

export function correoOpcional(control: AbstractControl): ValidationErrors | null {
  const correo = String(control.value ?? '').trim();
  if (!correo) {
    return null;
  }
  return /^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(correo) ? null : { correo: true };
}

export function fechaNacimientoValida(control: AbstractControl): ValidationErrors | null {
  const valor = String(control.value ?? '');
  if (!valor) {
    return null;
  }
  const hoy = new Date();
  const mes = String(hoy.getMonth() + 1).padStart(2, '0');
  const dia = String(hoy.getDate()).padStart(2, '0');
  const hoyIso = `${hoy.getFullYear()}-${mes}-${dia}`;
  if (valor < '1900-01-01' || valor > hoyIso) {
    return { fecha: true };
  }
  return null;
}

export function rucOpcional(control: AbstractControl): ValidationErrors | null {
  const ruc = String(control.value ?? '').trim();
  if (!ruc) {
    return null;
  }
  return /^\d{11}$/.test(ruc) ? null : { ruc: true };
}

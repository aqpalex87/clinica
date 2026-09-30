export interface Usuario{
    UsuarioWeb: string;
    Token: string;
    idPerfil: string;
}

export interface UserJWT {
    Codigo: string;
    CorreoElectronico: string;
    Nombre: string;
    NumeroDocumento: string;
    IdentificadorUnico: string;
    Perfil: string;
    nbf: number;
    exp: number;
    iat: number;
}
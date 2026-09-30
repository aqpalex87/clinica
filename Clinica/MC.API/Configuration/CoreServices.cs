using MC.Business.Logic.Layer.Implementation;
using MC.Business.Logic.Layer.Interfaces;
using MC.Control.Implementation;
using MC.Control.Interface;
using MC.Data.Access.Layer.DB2;
using MC.Data.Access.Layer.Implementation;
using MC.Data.Access.Layer.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CoreServices
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IAccessControl, AccessControl>();

            services.AddScoped(typeof(DB2DataAccess));

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IMenuRolService, MenuRolService>();
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<IEmpresaService, EmpresaService>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IParametrosMaestroRepository, ParametrosMaestroRepository>();
            services.AddScoped<IParametrosMaestroService, ParametrosMaestroService>();

            services.AddScoped<ISedesService, SedesService>();
            services.AddScoped<ISedesRepository, SedesRepository>();
        }
    }
}

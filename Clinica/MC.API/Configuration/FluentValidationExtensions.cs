using MC.API;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Globalization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FluentValidationExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation(fvc =>
            {
                ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es-PE");
                ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
                ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            });

            services.AddValidatorsFromAssemblyContaining<Startup>();
        }
    }
}

using MC.Business.Logic.Layer.Profiles;
using MC.Control.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using MC.Control.Filter;
using Microsoft.Extensions.Logging;
using System.IO;

namespace MC.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string Policy = "AllowOrigin";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(c =>
            {
                var origins = Configuration.GetSection("Origins").Get<string[]>();
                c.AddPolicy(Policy, options =>
                    options.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()
                //options.AllowAnyOrigin()
                //   .AllowAnyMethod()
                //   .AllowAnyHeader()
                );
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("TokenClave").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAutoMapper(typeof(ClienteProfile));
            services.AddCoreServices();
            //services.AddAutenticacion(Configuration);
            services.AddSwagger();
            services.AddFluentValidation();
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            }).AddNewtonsoftJson();

            //services.AddControllers().AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(option => option.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
            loggerFactory.AddFile(configuration["Logs"] + "LogMC-{Data}.txt");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }else
            {
                app.UseHsts();
            }

            // Orden original del proyecto (IIS Express + Swashbuckle): Swagger antes de ErrorHandling y UseRouting.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MC API V1");
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}

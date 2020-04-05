using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.CrossCutting.DependencyInjection;
using Api.Domain.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureService.ConfigureDependenciesService(services);
            ConfigureRepository.ConfigureDependenciesRepository(services);

            //Configuracao da injecao de dependencia para geracao do token
            SigningConfigurations signingConfigurations = new SigningConfigurations();
            TokenConfiguration tokenConfiguration = new TokenConfiguration();
            
            new ConfigureFromConfigurationOptions<TokenConfiguration>
            (
                Configuration.GetSection("TokenConfigurations")
            ).Configure(tokenConfiguration);

            services.AddSingleton(signingConfigurations);
            services.AddSingleton(tokenConfiguration);

            //Adicionando servico de autenticacao utilizando JWT
            services
            .AddAuthentication(
                authOptions =>
                    {
                        authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
            .AddJwtBearer(
                bearerOptions =>
                {
                    bearerOptions.TokenValidationParameters.IssuerSigningKey = signingConfigurations.Key;
                    bearerOptions.TokenValidationParameters.ValidAudience = tokenConfiguration.Audience;
                    bearerOptions.TokenValidationParameters.ValidIssuer = tokenConfiguration.Issuer;
                    bearerOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    bearerOptions.TokenValidationParameters.ValidateLifetime = true;
                    bearerOptions.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                });

            services
            .AddAuthorization(
                auth =>
                {
                    auth
                        .AddPolicy("Bearer",
                            new AuthorizationPolicyBuilder()
                                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                                .RequireAuthenticatedUser().Build()
                        );

                }
            );

            services.AddControllers();

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1", 
                    new OpenApiInfo 
                    {
                        Title = "API AspNetCore 3", 
                        Version = "v1",
                        Description = "API utilizando .Net Core 3",
                        Contact = new OpenApiContact 
                            {
                                Name = "Lucas Alves Marcello",
                                Url = new Uri("https://github.com/lucasmarcello")
                            }
                    });

                c.AddSecurityDefinition(
                    "Bearer", 
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Entre com o token JWT",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                c.AddSecurityRequirement
                (
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
                    }
                );

            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API .NetCore"); });

            //Redireciona a pagina principal para a documentacao do Swagger
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            #endregion

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

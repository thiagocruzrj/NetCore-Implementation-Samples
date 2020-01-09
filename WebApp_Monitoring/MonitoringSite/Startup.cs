using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MonitoringSite.Extensions;
using MonitoringSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;

namespace MonitoringSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var dadosDependencias = new List<MonitoringDependency>();

            new ConfigureFromConfigurationOptions<List<MonitoringDependency>>(
                Configuration.GetSection("Dependencies"))
                    .Configure(dadosDependencias);
            dadosDependencias = dadosDependencias.OrderBy(d => d.Name).ToList();

            // Verificando a disponibilidade dos bancos de dados
            // da aplicação através de Health Checks
            services.AddHealthChecks()
                .AddDependencies(dadosDependencias);
            services.AddHealthChecksUI();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Gera o endpoint que retornará os dados utilizados no dashboard
            app.UseHealthChecks("/healthchecks-data-ui", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Ativa o dashboard para a visualização da situação de cada Health Check
            app.UseHealthChecksUI();

            // Endpoint para retorno de informações sobre Health Checks
            // a serem utilizados pelo Worker Process de Monitoramento
            app.UseHealthChecks("/status-monitoramento",
               new HealthCheckOptions()
               {
                   ResponseWriter = async (context, report) =>
                   {
                       var result = JsonSerializer.Serialize(
                            report.Entries.Select(e => new
                            {
                                healthCheck = e.Key,
                                error = e.Value.Exception?.Message,
                                status = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                            }));
                       context.Response.ContentType = MediaTypeNames.Application.Json;
                       await context.Response.WriteAsync(result);
                   }
               });
        }
    }
}
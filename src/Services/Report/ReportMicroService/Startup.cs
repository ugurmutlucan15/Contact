using EventBusRabbitMQ;
using EventBusRabbitMQ.Contact;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using RabbitMQ.Client;

using ReportMicroService.Consumers;
using ReportMicroService.Data;
using ReportMicroService.Data.Interfaces;
using ReportMicroService.Extensions;
using ReportMicroService.Repositories;
using ReportMicroService.Repositories.Interfaces;
using ReportMicroService.Settings;

using System.IO;

namespace ReportMicroService
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Configuration Dependencies

            services.Configure<ReportDatabaseSettings>(Configuration.GetSection(nameof(ReportDatabaseSettings)));
            services.AddSingleton<IReportDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<ReportDatabaseSettings>>().Value);

            #endregion Configuration Dependencies

            #region Project Dependencies

            services.AddTransient<IReportContext, ReportContext>();
            services.AddTransient<IReportRepository, ReportRepository>();

            services.AddAutoMapper(typeof(Startup));

            #endregion Project Dependencies

            services.AddControllers();

            #region Swagger Dependencies

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ReportMicroService v1",
                    Version = "v1"
                });
            });

            #endregion Swagger Dependencies

            #region EventBus

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBus:HostName"]
                };

                if (!string.IsNullOrWhiteSpace(Configuration["EventBus:UserName"]))
                {
                    factory.UserName = Configuration["EventBus:UserName"];
                }

                if (!string.IsNullOrWhiteSpace(Configuration["EventBus:Password"]))
                {
                    factory.UserName = Configuration["EventBus:Password"];
                }

                var retryCount = 5;
                if (!string.IsNullOrWhiteSpace(Configuration["EventBus:RetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBus:RetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, retryCount, logger);
            });

            services.AddSingleton<EventBusRabbitMQContact>();
            services.AddSingleton<EventBusReportCreateConsumer>();

            #endregion EventBus

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("https://localhost:44398");
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReportMicroService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseRabbitListener();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), @"Export")),
                RequestPath = new PathString("/Export")
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
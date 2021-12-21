using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Registrators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Configuration;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Builder;
using OData.Swagger.Services;
using LAB.DataScanner.ConfigDatabaseApi.BusinessLogic.Registrators;
using LAB.DataScanner.ConfigDatabaseApi.Validation;

namespace LAB.DataScanner.ConfigDatabaseApi
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
            services.AddControllers().AddNewtonsoftJson();

            services.AddOData();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DataScanner UI API",
                    Description = "A DataScanner config database API",
                    Contact = new OpenApiContact
                    {
                        Name = "Vladimir Fominykh",
                        Email = "vladimir3591f@mail.ru",
                        Url = new Uri("https://github.com/Vladimir563"),
                    }
                });
            });

            services.AddOdataSwaggerSupport();

            services.AddMvc(p => p.EnableEndpointRouting = false);

            services.AddConfigDbContext()
                    .AddDataAccess()
                    .AddBusinessLogic()
                    .AddValidation();

            services.
                AddDbContext<DataScannerDbContext>(options => options
                .UseSqlServer(ConfigurationManager.ConnectionStrings["ConfigDataScannerDb"].ConnectionString));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(b =>
            {
                b.MapODataServiceRoute("ODataRoute", "odata", GetEdmModel());
            });

            app.UseExceptionHandlerMiddleware();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DataScanner application v1.0");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Filter().OrderBy().Count().MaxTop(10);
            });
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<ApplicationType>("ApplicationTypes").EntityType.HasKey(a => a.TypeId);

            builder.EntitySet<ApplicationInstance>("ApplicationInstances").EntityType.HasKey(a => a.InstanceId);

            builder.EntitySet<Binding>("Bindings").EntityType.HasKey(e => e.BindingId);

            return builder.GetEdmModel();
        }
    }
}

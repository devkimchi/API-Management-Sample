using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ApiManagementSample.Api.Filters;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

using Swashbuckle.SwaggerGen;
using Swashbuckle.SwaggerGen.XmlComments;

namespace ApiManagementSample.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            this.HostingEnvironment = env;
            this.ApplicationEnvironment = appEnv;

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IHostingEnvironment HostingEnvironment { get; }

        public IApplicationEnvironment ApplicationEnvironment { get; }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddSwaggerGen();

            services.ConfigureSwaggerDocument(
                options =>
                {
                    options.SingleApiVersion(new Info() { Version = "v1", Title = "Telstra Team Site API for Site Collection Provisioning" });
                    options.IgnoreObsoleteActions = true;
                    options.OperationFilter(new ApplyXmlActionComments(GetXmlPath(this.ApplicationEnvironment)));

                    options.DocumentFilter<SchemaDocumentFilter>();
                });

            services.ConfigureSwaggerSchema(
                options =>
                {
                    options.DescribeAllEnumsAsStrings = true;
                    options.IgnoreObsoleteProperties = true;
                    options.CustomSchemaIds(type => type.FriendlyId(true));
                    options.ModelFilter(new ApplyXmlTypeComments(GetXmlPath(this.ApplicationEnvironment)));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            app.UseSwaggerGen();
            app.UseSwaggerUi();

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        private static string GetXmlPath(IApplicationEnvironment appEnv)
        {
            var assembly = typeof(Startup).GetTypeInfo().Assembly;
            var assemblyName = assembly.GetName().Name;

            var path = $@"{appEnv.ApplicationBasePath}\{assemblyName}.xml";
            if (File.Exists(path))
            {
                return path;
            }

            var config = appEnv.Configuration;
            var runtime =
                $"{appEnv.RuntimeFramework.Identifier.ToLower()}{appEnv.RuntimeFramework.Version.ToString().Replace(".", string.Empty)}";

            path =
                $@"{appEnv.ApplicationBasePath}\..\..\artifacts\bin\{assemblyName}\{config}\{runtime}\{assemblyName}.xml";
            return path;
        }
    }
}

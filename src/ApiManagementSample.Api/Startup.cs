﻿using System.IO;
using System.Reflection;

using ApiManagementSample.Api.Filters;

using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using Swashbuckle.SwaggerGen;
using Swashbuckle.SwaggerGen.XmlComments;

namespace ApiManagementSample.Api
{
    /// <summary>
    /// This represents the main entry point of the web API application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env"><see cref="IHostingEnvironment"/> instance.</param>
        /// <param name="appEnv"><see cref="IApplicationEnvironment"/> instance.</param>
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            this.HostingEnvironment = env;
            this.ApplicationEnvironment = appEnv;

            // Set up configuration sources.
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the <see cref="IApplicationEnvironment"/> instance.
        /// </summary>
        public IApplicationEnvironment ApplicationEnvironment { get; }

        /// <summary>
        /// Gets the <see cref="IConfigurationRoot"/> instance.
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }

        /// <summary>
        /// Gets the <see cref="IHostingEnvironment"/> instance.
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Defines the main entry point of the web API application.
        /// </summary>
        /// <param name="args">List of arguments.</param>
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        /// <summary>
        /// Configures modules.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="env"><see cref="IHostingEnvironment"/> instance.</param>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/> instance.</param>
        /// <remarks>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</remarks>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            app.UseSwaggerGen();
            app.UseSwaggerUi();
        }

        /// <summary>
        /// Configures services including dependencies.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> instance.</param>
        /// <remarks>This method gets called by the runtime. Use this method to add services to the container.</remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().AddMvcOptions(
                options =>
                    {
                        options.Filters.Add(new GlobalActionFilterAttribute());
                        options.Filters.Add(new GlobalExceptionFilter());
                        options.Filters.Add(new RequireHttpsAttribute());
                    }).AddJsonOptions(
                        options =>
                            {
                                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                                options.SerializerSettings.Formatting = Formatting.Indented;
                                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                            });

            services.AddSwaggerGen();

            services.ConfigureSwaggerDocument(
                options =>
                    {
                        options.SingleApiVersion(
                            new Info()
                                {
                                    Version = "v1",
                                    Title = "API Management Sample API"
                                });
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
using AnyPreview.Core.Aliyun;
using AnyPreview.Core.Common;
using AnyPreview.Core.Redis;
using AnyPreview.Core.Settings;
using AnyPreview.Portal.Web;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CsvHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AnyPreview.Portal
{
    public class Startup
    {
        private readonly ILoggerFactory m_LoggerFactory;
        private readonly IConfiguration m_Configuration;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            m_LoggerFactory = loggerFactory;
            m_Configuration = configuration;
            ConfigureOthers();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(new WebApiExceptionFilter(m_LoggerFactory));
            });
            services.AddResponseCompression();
            services.AddSingleton(m_LoggerFactory.CreateLogger("AnyPreview.Root"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("AnyPreview", new Info
                {
                    Version = "1.0",
                    Title = "AnyPreview API v1.0"
                });

                var basePath = AppContext.BaseDirectory;
                c.IncludeXmlComments(Path.Combine(basePath, "AnyPreview.Portal.xml"));

                c.OperationFilter<ExamplesOperationFilter>();

                c.DescribeAllEnumsAsStrings();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
            });
            
            return ConfigureServiceProvider(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();

            app.UseResponseCompression();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.SwaggerEndpoint("/swagger/AnyPreview/swagger.json", "AnyPreview API v1.0 Docs");
                c.InjectStylesheet("/swagger/custom.css");
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        
        private IServiceProvider ConfigureServiceProvider(IServiceCollection services)
        {
            var immSetting = m_Configuration.GetSection("IMM").Get<IMMSetting>();
            services.AddAliyun(immSetting);

            var redisSetting = m_Configuration.GetSection("Redis").Get<RedisSetting>();
            services.AddRedis(redisSetting);

            var dpSetting = m_Configuration.GetSection("Preview").Get<PreviewSetting>();
            services.AddSingleton(dpSetting);

            var builder = new ContainerBuilder();
            builder.Populate(services);

            var assemblies = "Service".Split(',')
                .Select(s => Assembly.Load($"AnyPreview.{s}"))
                .ToArray();
            builder.RegisterAssemblyTypes(assemblies)
                .AsImplementedInterfaces()
                .AsSelf();

            return new AutofacServiceProvider(builder.Build());
        }

        private void ConfigureOthers()
        {
            using (var sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ContentType.csv")))
            {
                using (var csv = new CsvReader(sr))
                {
                    while (csv.Read())
                    {
                        CommomConstants.ContentTypeDict.Add(csv.GetField(0).ToLower(), csv.GetField(1).ToLower());
                    }
                }
            }
        }
    }
}

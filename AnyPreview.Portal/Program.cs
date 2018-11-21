using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using System;

namespace AnyPreview.Portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("程序初始化");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "程序出异常");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{GetEnvironmentName()}.json", optional: true, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
        }

        private static string GetEnvironmentName()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("environment.json", optional: true)
                .Build();
            return configuration["EnvironmentName"] ?? string.Empty;
        }
    }
}

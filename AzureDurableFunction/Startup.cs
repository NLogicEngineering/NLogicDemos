using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFunctionApp.Common;
using MyFunctionApp.Services;
using Polly;
using System;

[assembly: FunctionsStartup(typeof(MyFunctionApp.Startup))]
namespace MyFunctionApp
{
    public class Startup : FunctionsStartup
    {
        static IConfiguration Configuration { set; get; }

        static Startup()
        {
            string appEnv = Environment.GetEnvironmentVariable("AppEnvironment");
            if (appEnv == "local") // set in local.settings.json
            {
                Configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .Build();
            }
            else // running in Azure
            {
                Configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .Build();
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAppConfiguration>((s) => {
                return new AppConfiguration(Configuration);
            });

            builder.Services.AddHttpClient<IGitHubClient, GitHubClient>()
                .AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(1, _ => TimeSpan.FromSeconds(3))); // wait 3 sec and try again, just once
        }
    }
}

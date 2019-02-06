using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#nullable enable 
namespace GenericHost
{
    public class MyOptions
    {
        public int Test { get; set; }
    }
    class SomeRepo : IDisposable
    {
        readonly IOptionsMonitor<MyOptions> myOptionsMonitor;
        public SomeRepo(IOptionsMonitor<MyOptions> options)
        {
            this.myOptionsMonitor = options;
            Console.WriteLine("constructing some repo");
        }
        public void Dispose() =>
            Console.WriteLine("Disposing some repo");
        public int Test => this.myOptionsMonitor.CurrentValue.Test;
    }

    class MyHostedService : IHostedService, IDisposable
    {
        readonly SomeRepo s;
        volatile bool running;
        public MyHostedService(SomeRepo s) => this.s = s;

        public void Dispose() =>
            Console.WriteLine("Disposing MyHostedService");
        private void Loop()
        {
            while(running)
            {
                Thread.Sleep(1000);
                Console.WriteLine(s.Test);
            }
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("start MyHostedService");
            running = true;
            Task.Run(Loop);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            running = false;
            Console.WriteLine("stop MyHostedService");
            Thread.Sleep(3000);
            return Task.CompletedTask;
        }
    }


    class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = new HostBuilder()
                .UseContentRoot(".")
                .UseEnvironment(EnvironmentName.Development)
                .ConfigureHostConfiguration(configBuilder =>
                {

                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    hostContext.HostingEnvironment.ApplicationName = "CustomApplicationName";
                    configApp.AddJsonFile("appsettings.json", optional: true, reloadOnChange:true);
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                    configApp.AddCommandLine(args);
                    
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<HostOptions>(option =>
                    {
                        option.ShutdownTimeout = TimeSpan.FromSeconds(20);
                    })
                    .AddSingleton<SomeRepo>()
                    .AddHostedService<MyHostedService>();
                    services.Configure<MyOptions>(hostContext.Configuration);

                }).ConfigureLogging((hostContext, configLogging) =>
                    configLogging.AddConsole().AddDebug()
                ).Build();
            

            await host.StartAsync();

            await host.WaitForShutdownAsync();
        }
    }
}

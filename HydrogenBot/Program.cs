using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HydrogenBot.Database;
using HydrogenBot.Extentions;
using HydrogenBot.Scheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using Serilog.Events;

namespace HydrogenBot
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Quartz", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/latest.log")
                .CreateLogger();

            try
            {
                using var host = CreateHostBuilder(args).Build();
                await host.StartAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal exception");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostCtx, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true)
                        .AddJsonFile("appsettings.Development.json", true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hostCtx, services) =>
                {
                    services.AddDbContext<DatabaseContext>(opts =>
                    {
                        opts.UseMySql(hostCtx.Configuration.GetConnectionString("HydrogenBot"));
                    });

                    services.AddSingleton(new DiscordSocketClient(
                        new DiscordSocketConfig
                        {
                            MessageCacheSize = 100
                        }
                    ));

                    services.AddSingleton(new CommandService(new CommandServiceConfig
                    {
                        DefaultRunMode = RunMode.Async,
                        LogLevel = LogSeverity.Verbose,
                        CaseSensitiveCommands = false
                    }));

                    services.DiscoverAndMakeDiServicesAvailable();

                    ConfigureQuartz(services);
                    

                    services.AddHostedService<App>();
                })
                .UseSerilog()
                .UseConsoleLifetime();
        }

        private static void ConfigureQuartz(IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.SchedulerId = "BotScheduler";

                q.UseMicrosoftDependencyInjectionScopedJobFactory(options =>
                {
                    options.AllowDefaultConstructor = true;
                });

                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

                // configure jobs with code
                var jobs = typeof(BaseJob).GetAllInAssembly();
                var addJobMethod = typeof(ServiceCollectionExtensions).GetMethod(nameof(ServiceCollectionExtensions.AddJob));
                foreach (var job in jobs)
                {
                    var jobKey = new JobKey(job.Name);
                    var jobGeneric = addJobMethod?.MakeGenericMethod(job);
                    var interval = (int?) job.GetProperty(nameof(BaseJob.SecondsInterval))?.GetValue(null);

                    if (jobGeneric == null || interval == null)
                    {
                        continue;
                    }
                    jobGeneric.Invoke(null, parameters: new object?[]
                    {
                        q,
                        new Action<IServiceCollectionJobConfigurator>(j => j.StoreDurably().WithIdentity(jobKey)),
                    });

                    q.AddTrigger(t =>
                        t.ForJob(jobKey)
                            .StartAt(DateTime.UtcNow.AddSeconds(5)) // enough to log into discord
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(interval.Value).RepeatForever())
                    );
                }
            });

            services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });
        }
    }
}

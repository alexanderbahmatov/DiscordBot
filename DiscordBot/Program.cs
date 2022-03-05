using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Program
    {
        // Entry point of the program.
        static void Main (string[] args)
        {
            // One of the more flexable ways to access the configuration data is to use the Microsoft's Configuration model,
            // this way we can avoid hard coding the environment secrets. I opted to use the Json and environment variable providers here.
            IConfiguration config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DC_")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            RunAsync(config).GetAwaiter().GetResult();
        }

        static async Task RunAsync (IConfiguration configuration)
        {
            using var services = ConfigureServices(configuration);

            var client = services.GetRequiredService<DiscordSocketClient>();
            var commands = services.GetRequiredService<CommandService>();

            client.Log += LogAsync;
            commands.Log += LogAsync;

            await services.GetRequiredService<Core.Engine>().InitializeAsync();

            await client.LoginAsync(TokenType.Bot, configuration["token"]);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        static Task LogAsync (LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        static ServiceProvider ConfigureServices (IConfiguration configuration)
        {

            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All
            };
            return new ServiceCollection()
                .AddSingleton(configuration)
                .AddSingleton<DiscordSocketClient>(c => new DiscordSocketClient(config))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {                                       // Add the command service to the collection
                    LogLevel = LogSeverity.Verbose,     // Tell the logger to give V5erbose amount of info
                    DefaultRunMode = RunMode.Async,     // Force all commands to run async by default
                }))
                .AddSingleton<Core.Engine>()
                .BuildServiceProvider();
        }
            

        static bool IsDebug ()
        {
    #if DEBUG
                return true;
    #else
                return false;
    #endif
            }
        }
    //class Program
    //{
    //    DiscordSocketClient _client;
    //    Core.Engine _engine;
    //    static void Main (string[] args)
    //        => new Program().MainAsync().GetAwaiter().GetResult();

    //    private async Task MainAsync()
    //    {
    //        try
    //        {
    //            _engine = Core.Engine.Get();
    //            _client = new DiscordSocketClient();
    //            _client.MessageReceived += CommandsHandler;

    //            string token = "OTQyNzY4MzAyMzgyNTE4MzQy.YgpTZw.5wwhNCOQFadu-nu7B1bCeLEBumM";
    //            await _client.LoginAsync(Discord.TokenType.Bot, token);
    //            await _client.StartAsync();
    //            InteractionService interactionService = new();
    //            await Task.Delay(-1);
    //        }
    //        catch(Exception e)
    //        {

    //        }
    //    }

    //    private Task CommandsHandler(SocketMessage msg)
    //    {
    //        if (!msg.Author.IsBot)
    //        {
    //        }
    //        return Task.CompletedTask;
    //    }
    //}
}

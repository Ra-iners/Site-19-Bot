using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Site___.Extensions;
using Site___.Modules.Events;
using Site___.Storage;

namespace Site___
{
    public class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        
        public async Task MainAsync()
        {
            Globals.Log = "**Start** | Bot is attempting to start.";
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Globals.Client = new DiscordShardedClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadDefaultStickers =true,
                AlwaysDownloadUsers = true,
                AlwaysResolveStickers = true,
                MessageCacheSize = 4096,
                GatewayIntents = GatewayIntents.All
            });
            Globals.Commands = new CommandService();
            Globals.Services = new ServiceCollection()
                .AddSingleton(Globals.Client)
                .AddSingleton(Globals.Commands)
                .BuildServiceProvider();

            Globals.Client.Log += LogEvent;
            Globals.Client.ShardReady += ShardReady;

            await RegisterCommands();

            await Globals.Client.LoginAsync(TokenType.Bot, (string)Configuration.Get("Token"));
            await Globals.Client.StartAsync();

            await Task.Delay(-1);
        }

        static bool AlreadyInitialized = false;
        async Task ShardReady(DiscordSocketClient arg)
        {
            Globals.Log = "**Shard Ready** | Shard " + arg.ShardId + " is ready.";
            if(!AlreadyInitialized) // This was the issue on the last bot on why it would repeat messages
            {                       // Periodically the shard would reconnect so it would trigger the events again, duplicating them.
                AlreadyInitialized = true;
                
                new SuggestionReactions().Initialize();
                new DailyQOTD().Initialize();
            }
        }

        public async Task RegisterCommands()
        {
            Globals.Client.MessageReceived += HandleCommand;
            await Globals.Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Globals.Services);
        }

        public async Task HandleCommand(SocketMessage arg)
        {
            var Message = arg as SocketUserMessage;
            var Context = new ShardedCommandContext(Globals.Client, Message);
            if (Context.Message.Author.IsBot || Context.Message.Author.IsWebhook) return;

            int ArgPos = 0;
            if (Message.HasStringPrefix(Globals.Prefix, ref ArgPos) || Message.HasMentionPrefix(Globals.Client.CurrentUser, ref ArgPos))
            {
                var Result = await Globals.Commands.ExecuteAsync(Context, ArgPos, Globals.Services);
                if (Result.Error == CommandError.UnmetPrecondition)
                    await Context.Message.ReplyAsync(Result.ErrorReason);
            }
        }

        public Task LogEvent(LogMessage msg)
        {
            var Severity = msg.Severity switch
            {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => LogEventLevel.Information
            };
            Log.Write(Severity, msg.Exception, "[{Source}] {Message}", msg.Source, msg.Message);
            return Task.CompletedTask;
        }
    }
}

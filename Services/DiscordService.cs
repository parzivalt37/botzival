using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;
using botzival;
using botzival.HandlingFiles;
using botzival.Audio;
using botzival.Services;
using System;
using System.Threading.Tasks;
using Victoria;

namespace botzival.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly botzival.HandlingFiles.EventHandler _commandHandler;
        private readonly ServiceProvider _services;
        private readonly LavaNode _lavaNode;
        private readonly LavaLinkAudio _audioService;

        public DiscordService()
        {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commandHandler = _services.GetRequiredService<botzival.HandlingFiles.EventHandler>();
            _lavaNode = _services.GetRequiredService<LavaNode>();
            _audioService = _services.GetRequiredService<LavaLinkAudio>();

            SubscribeLavaLinkEvents();
            SubscribeDiscordEvents();
        }

        public async Task InitializeAsync()
        {
            await _client.LoginAsync(TokenType.Bot, Config.DiscordToken);
            await _client.StartAsync();

            await _commandHandler.InitializeAsync(_client);

            await Task.Delay(-1);
        }

        private void SubscribeLavaLinkEvents()
        {
            
        }

        private void SubscribeDiscordEvents()
        {
            _client.Ready += ReadyAsync;
            _client.Log += LogAsync;
        }

        private async Task ReadyAsync()
        {
            try
            {
                await _lavaNode.ConnectAsync();
                await _client.SetGameAsync("hating discord.net", null, ActivityType.CustomStatus);
            }
            catch (Exception ex)
            {
                await Logging.LogInformationAsync(ex.Source, ex.Message);
            }
        }

        private async Task LogAsync(LogMessage logMsg)
        {
            await Logging.LogAsync(logMsg.Source, logMsg.Severity, logMsg.Message);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<botzival.HandlingFiles.EventHandler>()
                .AddSingleton<LavaNode>()
                .AddSingleton(new LavaConfig())
                .AddSingleton<LavaLinkAudio>()
                .AddSingleton<AudioServices>()
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();
        }
    }
}
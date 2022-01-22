using System;
using Discord;
using System.Linq;
using Discord.Commands;
using System.Reflection;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Discord.Addons.Interactive;
using System.Timers;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using Discord.Rest;
using System.Collections.Generic;
using botzival.Commands;
using botzival.Audio;

namespace botzival.HandlingFiles
{
    class EventHandler
    {
        DiscordSocketClient _client;
        CommandService _service;
        readonly IServiceProvider serviceProdiver;

        public EventHandler(IServiceProvider services) => serviceProdiver = services;
        public static IDictionary<string, DateTimeOffset> timeList = new Dictionary<string, DateTimeOffset>();

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();

            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProdiver);

            _client.MessageReceived += HandleCommandAsync;

            _service.Log += Log;
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            SocketUserMessage msg = s as SocketUserMessage;
            string m = msg.Content.ToLower();
            if (msg == null) return;

            var context = new SocketCommandContext(_client, msg);
            int argPos = 0;
            if (msg.HasStringPrefix("!", ref argPos))
                await _service.ExecuteAsync(context, argPos, serviceProdiver, MultiMatchHandling.Exception);

            if (Regex.IsMatch(m, "[Gg]aming", RegexOptions.IgnoreCase))
                await context.Channel.SendMessageAsync("gamming");
                
            msg.Content.ToLower();
        }
    }
}

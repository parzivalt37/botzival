using System;
using Discord;
using System.IO;
using System.Net;
using System.Drawing;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace botzival.Services
{
    static class Utilities
    {
        //Universal Web Client
        public static readonly WebClient webClient = new WebClient();

        public static readonly Random getRandom = new Random();
        //random number

        public static int GetRandomNumber(int min, int max)
        {
            lock (getRandom) { return getRandom.Next(min, max); }
        }

        //generic embed template
        public static Embed Embed(string t, string d, Discord.Color c, string f, string thURL) => new EmbedBuilder()
            .WithTitle(t)
            .WithDescription(d)
            .WithColor(c)
            .WithFooter(f)
            .WithThumbnailUrl(thURL)
            .Build();

        //generic image embed template
        public static Embed ImageEmbed(string t, string d, Discord.Color c, string f, string imageURL) => new EmbedBuilder()
            .WithTitle(t)
            .WithDescription(d)
            .WithColor(c)
            .WithFooter(f)
            .WithImageUrl(imageURL)
            .Build();

        public static async Task PrintEmbed(this ISocketMessageChannel channel, string title, string message, Discord.Color color) => await channel.SendMessageAsync("", false, new EmbedBuilder()
            .WithTitle(title)
            .WithColor(color)
            .WithDescription(message)
            .Build());

        public static async Task SendEmbed(ISocketMessageChannel channel, string title, string description, Discord.Color color, string footer, string thumbnailURL)
        {
            await channel.SendMessageAsync(null, false, Embed(title, description, color, footer, thumbnailURL)).ConfigureAwait(false);
        }

        public static async Task PrintSuccess(this ISocketMessageChannel channel, string message) => await channel.PrintEmbed("Success", message, Colors.Green).ConfigureAwait(false);

        public static async Task PrintError(this ISocketMessageChannel channel, string message) => await channel.PrintEmbed("Error", message, Colors.Red).ConfigureAwait(false);
    }
}
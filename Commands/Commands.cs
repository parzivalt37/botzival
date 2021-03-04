using Discord;
using Interactivity;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using botzival.HandlingFiles;
using botzival.Preconditions;
using botzival.Audio;
using botzival.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace botzival.Commands
{
    [RequireContext(ContextType.Guild)]
    public class Commands : InteractiveBase
    {
        public string users;

        public LavaLinkAudio AudioService { get; set; }
        
        //Bot uptime
        [Command("uptime")]
        public async Task DisplayUptime()
        {
            var time = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
            string uptime = $"{time.Hours} hours, {time.Minutes} minutes, {time.Seconds} seconds";
            await Utilities.SendEmbed(Context.Channel, "", uptime, Colors.Blue, "", "");
        }

        //Fake ban
        [Command("banm")]
        public async Task Ban(SocketGuildUser user)
        {
            var banMessage = await Context.Channel.SendMessageAsync("Initiating ban protocol...");

            await Task.Delay(400);
            await banMessage.ModifyAsync(m => { m.Content = "Initiating ban protocol... ..."; });

            await Task.Delay(400);
            await banMessage.ModifyAsync(m => { m.Content = "Initiating ban protocol... ... ..."; });

            await Task.Delay(400);
            await banMessage.ModifyAsync(m => { m.Content = $"Initiating ban protocol... ... ... Done. {user} has been banned."; });
        }

        //joke command, that is a joke in and of itself
        [Command("joke")]
        public async Task Joke()
        {
            await Context.Channel.SendMessageAsync("Why did the chicken cross the road?");

            await Task.Delay(1000);
            await Context.Channel.SendMessageAsync("Because I said so. Now laugh.");
        }

        //pings you 10 times
        [Command("ping")]
        public async Task Ping()
        {
            for (int i = 0; i < 10; i++)
            {
                await Context.Channel.SendMessageAsync("<@" + Context.User.Id + ">");
                await Task.Delay(1000);
            }
        }

        //fake identity theft command that uses userID
        [Command("idt")]
        public async Task IDT()
        {
            var IDTmessage = await Context.Channel.SendMessageAsync("Stealing credit card number...");

            await Task.Delay(1000);
            await IDTmessage.ModifyAsync(m => { m.Content = "Stealing credit card number... ..."; });
            await Task.Delay(1000);
            await IDTmessage.ModifyAsync(m => { m.Content = "Stealing credit card number... ... ..."; });
            await Task.Delay(1000);
            await IDTmessage.ModifyAsync(m => { m.Content = "Stealing credit card number... ... ... ..."; });
            await Task.Delay(5000);

            Random r = new Random();
            long h = r.Next(0, 9);
            for (int i = 0; i < 15; i++)
            {
                h *= 10;
                h += r.Next(0, 9);
            }

            int j = r.Next(0, 9);
            for (int i = 0; i < 2; i++)
            {
                j *= 10;
                j += r.Next(0, 9);
            }
            await Context.Channel.SendMessageAsync("Credit card number: " + h + "\nSecurity code: " + j);

        }

        //help command
        [Command("help")]
        public async Task HelpComm()
        {
            await Context.Channel.SendMessageAsync("Prefix: !\nhelp - displays list of commands\nuptime - displays how long the bot has been online\njoke - tells a joke\nping - pings the user 10 times\nidt - steals your identity\n");
        }

        //music command help
        [Command("musichelp")]
        public async Task MusicHelp()
        {
            await Context.Channel.SendMessageAsync("Music commands are temporarily disabled until further notice");
        }

        //mod command help
        [Command("modhelp")]
        [RequireRole("Admins")]
        public async Task ModHelp()
        {
            await Context.Channel.SendMessageAsync("Prefix: !\nhelp - displays list of commands\nmodhelp - displays list of mod-only commands\npurge [int] - purges x amount of messages\nbanm - meme bans someone\nban [user] [reason] - bans a user\nunban [user] - unbans a user");
        }

        //DMs a user the Matrix lines
        /*[Command("mx")]
        public async Task MatrixMessage()
        {
            await Context.Message.DeleteAsync();
            var msg1 = await Context.User.SendMessageAsync("Wake up, " + Context.User.Username + ".");
            await Task.Delay(4000);
            await msg1.DeleteAsync();

            var msg2 = await Context.User.SendMessageAsync("The Matrix has you...");
            await Task.Delay(5000);
            await msg2.DeleteAsync();
            var msg3 = await Context.User.SendMessageAsync("Follow the white rabbit...");
            await Task.Delay(3000);
            await msg3.DeleteAsync();
        }*/

        //purge # of messages command
        [Command("purge")]
        [Alias("delete")]
        [RequireRole("Admins")]
        public async Task PurgeMsgs(int i)
        {
            await Context.Message.DeleteAsync();
            var msgs = await Context.Channel.GetMessagesAsync(i).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(msgs);
            var DelMsg = await Context.Channel.SendMessageAsync($"{i} messages have been deleted");
            await Task.Delay(5000);
            await Context.Channel.DeleteMessageAsync(DelMsg);
        }

        //bans a user
        [Command("ban")]
        [RequireRole("Admins")]
        public async Task BanUser(SocketGuildUser user, string reason)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync("No user specified.");
                return;
            }
            if (reason == null)
            {
                await Context.Channel.SendMessageAsync("No reason specified.");
                return;
            }

            await Context.Guild.AddBanAsync(user, 0, reason);
            await Context.Channel.SendMessageAsync($"{user.Username} has been banned: {reason}.");
        }

        //unbans a user
        [Command("unban")]
        [RequireRole("Admins")]
        public async Task UnbanUser(SocketGuildUser user)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync("No user specified.");
                return;
            }
            await Context.Guild.RemoveBanAsync(user);
            await Context.Channel.SendMessageAsync($"{user.Username} has been unbanned.");
        }

        //gives a user a warning
        [Command("warn")]
        [RequireRole("Admins")]
        public async Task WarnUser(SocketGuildUser user, string reason)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync("No user specified.");
                return;
            }
            if (reason == null)
            {
                await Context.Channel.SendMessageAsync("No reason specified.");
                return;
            }

            await Context.Channel.SendMessageAsync($"{user.Mention} You have been warned: {reason}");
        }

        //hate src#
        [Command("src#")]
        public async Task SRCsharp()
        {
            await Context.Message.DeleteAsync();

            for (int i = 0; i < 10; i++)
            {
                await Context.Channel.SendMessageAsync("SpeedrunComSharp sucks");
            }
        }
    }
}
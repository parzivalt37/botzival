using Discord;
using Discord.Commands;
using Discord.WebSocket;
using botzival.Audio;
using System;
using System.Threading.Tasks;

namespace botzival.Commands
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        public LavaLinkAudio AudioService { get; set; }

        [Command("join")]
        [Alias("summon")]
        public async Task JoinAndPlay()
            //await AudioService.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel);
            => await Context.Channel.SendMessageAsync("Music commands are disabled until further notice");


        [Command("leave")]
        public async Task Leave()
            //await AudioService.LeaveAsync(Context.Guild, Context.Channel);
            => await Context.Channel.SendMessageAsync("Music commands are disabled until further notice");

        [Command("play")]
        public async Task Play([Remainder] string search)
            //await AudioService.PlayAsync(Context.User as SocketGuildUser, Context.Guild, search, Context.Channel);
            => await Context.Channel.SendMessageAsync("Music commands are disabled until further notice");

        [Command("stop")]
        public async Task Stop()
            //await AudioService.StopAsync(Context.Guild, Context.Channel);
            => await Context.Channel.SendMessageAsync("Music commands are disabled until further notice");

        [Command("queue")]
        public async Task List()
            //await AudioService.ListAsync(Context.Guild, Context.Channel);
            => await Context.Channel.SendMessageAsync("Music commands are disabled until further notice");

        [Command("skip")]
        public async Task Skip()
            //await AudioService.SkipTrackAsync(Context.Guild, Context.Channel);
            => await Context.Channel.SendMessageAsync("Music commands are disabled until further notice");

        [Command("pause")]
        public async Task Pause()
            //await AudioService.PauseAsync(Context.Guild, Context.Channel);
            => await Context.Channel.SendMessageAsync("Music commands are disabled until further notice");

        [Command("resume")]
        public async Task Resume()
            //await AudioService.ResumeAsync(Context.Guild, Context.Channel);
            => await Context.Channel.SendMessageAsync("Music commands are disabled until further notice");

    }
}

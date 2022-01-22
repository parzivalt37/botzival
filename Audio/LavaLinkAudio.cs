using Discord;
using Discord.WebSocket;
using botzival.HandlingFiles;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.EventArgs;
using Victoria.Enums;
using Victoria.Responses.Rest;

namespace botzival.Audio
{
    public sealed class LavaLinkAudio
    {
        private readonly LavaNode _lavaNode;

        public LavaLinkAudio(LavaNode lavaNode) => _lavaNode = lavaNode;

        public async Task JoinAsync(IGuild guild, IVoiceState voiceState, ISocketMessageChannel textChannel)
        {
            if (_lavaNode.HasPlayer(guild))
            {
                await textChannel.SendMessageAsync("I'm already connected to a voice channel");
            }

            if (voiceState.VoiceChannel is null)
            {
                await textChannel.SendMessageAsync("You must be connected to a voice channel to summon me");
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel);
                await textChannel.SendMessageAsync($"Joined {voiceState.VoiceChannel.Name}");
            }

            catch (Exception ex)
            {
                await textChannel.SendMessageAsync(ex.Message);
            }
        }

        public async Task PlayAsync(SocketGuildUser user, IGuild guild, string query, ISocketMessageChannel textChannel)
        {
            if (user.VoiceChannel == null)
            {
                await textChannel.SendMessageAsync("You must be in a voice channel to play music");
            }

            if (!_lavaNode.HasPlayer(guild))
            {
                await textChannel.SendMessageAsync("I'm not connected to a voice channel");
            }

            try
            {
                var player = _lavaNode.GetPlayer(guild);

                LavaTrack track;

                var search = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                    await _lavaNode.SearchAsync(query)
                    : await _lavaNode.SearchYouTubeAsync(query);

                if (search.LoadStatus == LoadStatus.NoMatches)
                {
                    await textChannel.SendMessageAsync($"No results available for {query}");
                }

                track = search.Tracks.FirstOrDefault();
                 
                if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                {
                    player.Queue.Enqueue(track);
                    await textChannel.SendMessageAsync($"{track.Title} has been added to the queue");
                }

                await player.PlayAsync(track);
                await textChannel.SendMessageAsync($"Now playing {track.Title}");
            }

            catch (Exception ex)
            {
                await textChannel.SendMessageAsync(ex.Message);
            }
        }

        public async Task LeaveAsync(IGuild guild, ISocketMessageChannel textChannel)
        {
            try
            {
                var player = _lavaNode.GetPlayer(guild);

                if (player.PlayerState is PlayerState.Playing)
                {
                    await player.StopAsync();
                }

                await _lavaNode.LeaveAsync(player.VoiceChannel);

                await textChannel.SendMessageAsync("I have left the VC");
            }

            catch (InvalidOperationException ex)
            {
                await textChannel.SendMessageAsync(ex.Message);
            }
        }

        public async Task ListAsync(IGuild guild, ISocketMessageChannel textChannel)
        {
            try
            {
                var descriptionBuilder = new StringBuilder();

                var player = _lavaNode.GetPlayer(guild);

                if (player.PlayerState is PlayerState.Playing)
                {
                    if (player.Queue.Count < 1 && player.Track != null)
                    {
                        await textChannel.SendMessageAsync($"Now playing {player.Track.Title}\nNothing else is queued");
                    }
                    else
                    {
                        var trackNum = 2;
                        foreach (LavaTrack track in player.Queue)
                        {
                            descriptionBuilder.Append($"{trackNum}: [{track.Title}]({track.Url}) - {track.Id}\n");
                            trackNum++;
                        }
                        await textChannel.SendMessageAsync($"Now playing: [{player.Track.Title}]({player.Track.Url})\n{descriptionBuilder}");
                    }
                }
                else
                {
                    await textChannel.SendMessageAsync("Player doesn't seem to be playing anything right now");
                }
            }
            catch (Exception ex)
            {
                await textChannel.SendMessageAsync(ex.Message);
            }
        }

        public async Task SkipTrackAsync(IGuild guild, ISocketMessageChannel textChannel)
        {
            try
            {
                var player = _lavaNode.GetPlayer(guild);

                if (player.Queue.Count < 1)
                {
                    await textChannel.SendMessageAsync("Unable to skip a track - too few songs in the queue");
                }
                else
                {
                    try
                    {
                        var currentTrack = player.Track;
                        await player.SkipAsync();
                        await textChannel.SendMessageAsync($"{currentTrack.Title} has been skipped");
                    }
                    catch (Exception ex)
                    {
                        await textChannel.SendMessageAsync(ex.Message);
                    }
                }
            }
            
            catch (Exception ex)
            {
                await textChannel.SendMessageAsync(ex.Message);
            }
        }

        public async Task StopAsync(IGuild guild, ISocketMessageChannel textChannel)
        {
            try
            {
                var player = _lavaNode.GetPlayer(guild);

                if (player.PlayerState is PlayerState.Playing)
                {
                    await player.StopAsync();
                }

                await textChannel.SendMessageAsync("I have stopped playback, and the playlist has been cleared");
            }

            catch (Exception ex)
            {
                await textChannel.SendMessageAsync(ex.Message);
            }
        }

        public async Task PauseAsync(IGuild guild, ISocketMessageChannel textChannel)
        {
            try
            {
                var player = _lavaNode.GetPlayer(guild);
                if (!(player.PlayerState is PlayerState.Playing))
                {
                    await player.PauseAsync();
                    await textChannel.SendMessageAsync("There is nothing to pause");
                }

                await player.PauseAsync();
                await textChannel.SendMessageAsync($"**{player.Track.Title}** has been paused");
            }
            catch (InvalidOperationException ex)
            {
                await textChannel.SendMessageAsync(ex.Message);
            }
        }

        public async Task ResumeAsync(IGuild guild, ISocketMessageChannel textChannel)
        {
            var player = _lavaNode.GetPlayer(guild);

            if (player.PlayerState is PlayerState.Paused)
            {
                await player.ResumeAsync();
            }

            await textChannel.SendMessageAsync($"**{player.Track.Title}** has resumed");
        }

        public async Task TrackEnded(TrackEndedEventArgs args, ISocketMessageChannel textChannel)
        {
            if (!args.Reason.ShouldPlayNext())
                return;

            if (!args.Player.Queue.TryDequeue(out var queueable))
            {
                await textChannel.SendMessageAsync("Playback finished");
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                await textChannel.SendMessageAsync("Next item in queue is not a track");
                return;
            }

            await args.Player.PlayAsync(track);
            await textChannel.SendMessageAsync($"Now playing {track.Title} ({track.Url})");
        }
    }
}

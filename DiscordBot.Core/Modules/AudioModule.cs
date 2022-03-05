using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Guild;
using DiscordBot.Player;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DiscordBot.Core
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        private Engine _engine;
        public AudioModule(Engine engine)
        {
            _engine = engine;
        }
        [Command("play", RunMode = RunMode.Async)]
        [Summary("play {youtube.url}")]
        public async Task Play ([Remainder] string url)
        {
            SocketVoiceChannel voiceChannel = (Context.User as SocketGuildUser)?.VoiceChannel;

            if (voiceChannel is null)
            {
                await ReplyAsync("You r not in a voice channel");
                return;
            }

            GuildState guildState = GuildManager.GetGuildState(Context.Guild);

            if (guildState.AudioClient == null || guildState.AudioClient.ConnectionState != ConnectionState.Connected)
            {
                var audioClient = await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();
                guildState.SetAudioClient(audioClient);
            }

            guildState.Player.Enqueue(url);
            if (guildState.Player.Current == null)
            {
                guildState.Player.Play(guildState.OutPut);
            }
            var current = guildState.Player.Current;
            if (current != null && current.Info != null)
            {
                await ReplyAsync($"Now playing {current.Info.Title}");
            }
            else
            {
                await ReplyAsync($"Nothing to play");
            }
        }
        [Command("stop", RunMode = RunMode.Async)]
        [Summary("stop player, clearing queue and leaves the channel")]
        public async Task Stop()
        {
            GuildState guildState = GuildManager.GetGuildState(Context.Guild);
            await guildState.Player.Stop();
            guildState.Player.Tracks.Clear();
            GuildManager.VoiceStates.TryRemove(Context.Guild.Id, out _);
            await (Context.User as IVoiceState).VoiceChannel.DisconnectAsync();
        }
        [Command("pause", RunMode = RunMode.Async)]
        [Summary("pause current track")]
        public async Task Pause ()
        {
            GuildState guildState = GuildManager.GetGuildState(Context.Guild);
            guildState.Player.Pause();
        }
        [Command("next", RunMode = RunMode.Async)]
        [Summary("forward next track")]
        public async Task Next ()
        {
            GuildState guildState = GuildManager.GetGuildState(Context.Guild);
            await guildState.Player.Stop();
            guildState.Player.Next();
            var current = guildState.Player.Current;
            if (current != null && current.Info != null)
            {
                await ReplyAsync($"Now playing {current.Info.Title}");
            }
            else
            {
                await ReplyAsync($"Nothing to play");
            }
        }
        [Command("skip", RunMode = RunMode.Async)]
        [Summary("same as next")]
        public async Task Skip ()
        {
            GuildState guildState = GuildManager.GetGuildState(Context.Guild);
            var current = guildState.Player.Current;
            if (current != null && current.Info != null)
            {
                await ReplyAsync($"Skipping {current.Info.Title}");
            }
            else
            {
                await ReplyAsync($"Nothing to skip");
            }
            await Next();
        }
        [Command("np", RunMode = RunMode.Async)]
        [Summary("now playing track info")]
        public async Task NowPlaying ()
        {
            GuildState guildState = GuildManager.GetGuildState(Context.Guild);
            if (guildState.Player == null)
            {
                await ReplyAsync($"Nothing to play");
                return;
            }
            var current = guildState.Player.Current;
            if (current != null && current.Info != null)
            {
                await ReplyAsync($"Now playing {current.Info.Title}");
            }
            else
            {
                await ReplyAsync($"Nothing to play");
            }
        }
    }
}

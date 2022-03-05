using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Guild
{
    public static class GuildManager
    {
        public static readonly ConcurrentDictionary<ulong, GuildState> VoiceStates = new ConcurrentDictionary<ulong, GuildState>();
        public static GuildState GetGuildState (IGuild guild)
        {
            GuildState voiceState;

            if (!VoiceStates.ContainsKey(guild.Id))
            {
                VoiceStates.TryAdd(guild.Id, new GuildState());
            }
            voiceState = VoiceStates[guild.Id];

            return voiceState;
        }
    }
}

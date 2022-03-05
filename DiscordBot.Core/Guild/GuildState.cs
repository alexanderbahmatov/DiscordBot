using Discord.Audio;
using DiscordBot.Player;
using System.IO;

namespace DiscordBot.Core.Guild
{
    public class GuildState
    {
        public AudioPlayer Player { get; set; }
        public IAudioClient AudioClient { get; private set; }
        public Stream OutPut { get; set; }
        public GuildState()
        {
            Player = new AudioPlayer();
        }
        /// <param name="audioClient"></param>
        public void SetAudioClient (IAudioClient audioClient)
        {
            AudioClient = audioClient;
            OutPut?.Dispose();
            OutPut = audioClient.CreatePCMStream(AudioApplication.Music, 90000, 200);
        }
    }
}

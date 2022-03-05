using DiscordBot.Player.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DiscordBot.Player.Common
{
    public class InfoProvider
    {
        public static async Task<JObject> ExtractInfo (string query, bool isUrl = true)
        {
            string arguments = $" --dump-single-json  ytsearch:" + query;

            if (isUrl)
            {
                arguments = $" --dump-single-json  " + query;
            }

            Process ytdlProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "youtube-dl",
                Arguments = arguments,
                RedirectStandardOutput = true
            });

            string jsonString = await ytdlProcess.StandardOutput.ReadToEndAsync();

            return JObject.Parse(jsonString);
        }
        public static async Task<List<AudioTrack>> LoadAudioTracks (string query)
        {
            var fromUrl = Uri.IsWellFormedUriString(query, UriKind.Absolute);
            JObject ytdlResponseJson = await InfoProvider.ExtractInfo(query, fromUrl).ConfigureAwait(false);
            List<AudioTrack> songs = new List<AudioTrack>();

            if (ytdlResponseJson.ContainsKey("entries"))
            {
                if (fromUrl)
                {
                    foreach (JObject ytdlVideoJson in ytdlResponseJson["entries"].Value<JArray>())
                    {
                        Data songInfo = Data.ParseYtdlResponse(ytdlVideoJson);
                        songs.Add(new AudioTrack(songInfo.Url, songInfo));
                    }
                }
                else
                {
                    JObject ytdlVideoJson = ytdlResponseJson["entries"].Value<JArray>()[0].Value<JObject>();
                    Data firstEntrySong = Data.ParseYtdlResponse(ytdlVideoJson);
                    songs.Add(new AudioTrack(firstEntrySong.Url, firstEntrySong));
                }
            }
            else
            {
                Data songInfo = Data.ParseYtdlResponse(ytdlResponseJson);
                songs.Add(new AudioTrack(songInfo.Url, songInfo));
            }
            return songs;
        }
    }
}

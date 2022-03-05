using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using Discord.Addons.Music.Objects;
using Discord.Addons.Music.Source;
using Newtonsoft.Json.Linq;

namespace Discord.Addons.Music.Common
{

	public class TrackLoader
	{
		public static async Task<List<AudioTrack>> LoadAudioTrack (string query, bool fromUrl = true)
		{
			if (!fromUrl)
			{
				query = HttpUtility.UrlEncode(query);
			}
			JObject ytdlResponseJson = await YoutubeDLInfoProvider.ExtractInfo(query, fromUrl).ConfigureAwait(continueOnCapturedContext: false);
			List<AudioTrack> songs = new List<AudioTrack>();
			if (ytdlResponseJson.ContainsKey("entries"))
			{
				if (fromUrl)
				{
					foreach (JObject ytdlVideoJson2 in ytdlResponseJson["entries"].Value<JArray>()!)
					{
						AudioInfo songInfo2 = AudioInfo.ParseYtdlResponse(ytdlVideoJson2);
						songs.Add(new AudioTrack
						{
							Url = songInfo2.Url,
							Info = songInfo2
						});
					}
				}
				else
				{
					JObject ytdlVideoJson = ytdlResponseJson["entries"].Value<JArray>()![0].Value<JObject>();
					AudioInfo firstEntrySong = AudioInfo.ParseYtdlResponse(ytdlVideoJson);
					songs.Add(new AudioTrack
					{
						Url = firstEntrySong.Url,
						Info = firstEntrySong
					});
				}
			}
			else
			{
				AudioInfo songInfo = AudioInfo.ParseYtdlResponse(ytdlResponseJson);
				songs.Add(new AudioTrack
				{
					Url = songInfo.Url,
					Info = songInfo
				});
			}
			return songs;
		}

		public static Process LoadFFmpegProcess (string url)
		{
			return Process.Start(new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = "/C youtube-dl.exe --format --audio-quality 0 bestaudio -o - " + url + " | ffmpeg.exe -loglevel panic -i pipe:0 -c:a libopus -b:a 96K -ac 2 -f s16le -ar 48000 pipe:1",
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			});
		}
	}
}

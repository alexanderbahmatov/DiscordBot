using System.Threading.Tasks;
using Discord.Addons.Music.Exception;
using Discord.Addons.Music.Source;
using Discord.Audio;

namespace Discord.Addons.Music.Player
{
	public interface IAudioEvent
	{
		public delegate Task TrackStartAsync (IAudioClient audioClient, IAudioSource track);

		public delegate Task TrackEndAsync (IAudioClient audioClient, IAudioSource track);

		public delegate Task TrackErrorAsync (IAudioClient audioClient, IAudioSource track, TrackErrorException exception);

		event TrackStartAsync OnTrackStartAsync;

		event TrackEndAsync OnTrackEndAsync;

		event TrackErrorAsync OnTrackErrorAsync;
	}

}
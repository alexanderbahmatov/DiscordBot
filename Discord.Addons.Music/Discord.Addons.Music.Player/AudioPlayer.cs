using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Music.Exception;
using Discord.Addons.Music.Source;
using Discord.Audio;

namespace Discord.Addons.Music.Player
{

	public class AudioPlayer : IAudioEvent
	{
		private volatile bool paused = false;

		private CancellationTokenSource cts;

		private double volume = 1.0;

		public IAudioSource PlayingTrack { get; private set; }

		public Stream DiscordStream { get; set; }

		public IAudioClient AudioClient { get; private set; }

		public double Volume
		{
			get
			{
				return volume;
			}
			set
			{
				if (value > 100.0)
				{
					value = 100.0;
				}
				else if (value < 0.0)
				{
					value = 0.0;
				}
				if (value > 1.0)
				{
					value /= 100.0;
				}
				volume = value;
			}
		}

		public bool Paused
		{
			get
			{
				return paused;
			}
			set
			{
				paused = value;
			}
		}

		public event IAudioEvent.TrackStartAsync OnTrackStartAsync;

		public event IAudioEvent.TrackEndAsync OnTrackEndAsync;

		public event IAudioEvent.TrackErrorAsync OnTrackErrorAsync;

		public AudioPlayer ()
		{
			OnTrackStartAsync += TrackStartEventAsync;
			OnTrackEndAsync += TrackEndEventAsync;
			OnTrackErrorAsync += TrackErrorEventAsync;
		}

		public AudioPlayer (IAudioClient audioClient)
		{
			AudioClient = audioClient;
			DiscordStream = audioClient.CreatePCMStream(AudioApplication.Music);
			OnTrackStartAsync += TrackStartEventAsync;
			OnTrackEndAsync += TrackEndEventAsync;
			OnTrackErrorAsync += TrackErrorEventAsync;
		}

		public void SetAudioClient (IAudioClient audioClient)
		{
			AudioClient = audioClient;
			DiscordStream?.Dispose();
			DiscordStream = audioClient.CreatePCMStream(AudioApplication.Music);
		}

		private Task TrackStartEventAsync (IAudioClient audioClient, IAudioSource track)
		{
			paused = false;
			PlayingTrack.LoadProcess();
			return Task.CompletedTask;
		}

		private Task TrackEndEventAsync (IAudioClient audioClient, IAudioSource track)
		{
			paused = false;
			ResetStreams();
			PlayingTrack = null;
			cts?.Dispose();
			return Task.CompletedTask;
		}

		private Task TrackErrorEventAsync (IAudioClient audioClient, IAudioSource track, TrackErrorException exception)
		{
			paused = false;
			ResetStreams();
			PlayingTrack = null;
			return Task.CompletedTask;
		}

		protected async Task AudioLoopAsync (IAudioSource track, CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				if (DiscordStream == null)
				{
					await this.OnTrackErrorAsync(AudioClient, track, new TrackErrorException("Error when playing audio track: Discord stream gone."));
					break;
				}
				if (!paused)
				{
					int read = await PlayingTrack.Provide20msAudio(ct).ConfigureAwait(continueOnCapturedContext: false);
					if (read <= 0)
					{
						break;
					}
					if (Volume != 1.0)
					{
						await DiscordStream.WriteAsync(AdjustVolume(PlayingTrack.GetBufferFrame(), Volume), 0, read, ct).ConfigureAwait(continueOnCapturedContext: false);
					}
					else
					{
						await DiscordStream.WriteAsync(PlayingTrack.GetBufferFrame(), 0, read, ct).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else
				{
					await Task.Delay(4000);
				}
			}
		}

		public async Task StartTrackAsync (IAudioSource track)
		{
			if (track != null)
			{
				if (PlayingTrack != null)
				{
					Stop();
				}
				PlayingTrack = track;
				cts?.Dispose();
				cts = new CancellationTokenSource();
				await this.OnTrackStartAsync(AudioClient, PlayingTrack).ConfigureAwait(continueOnCapturedContext: false);
				await AudioLoopAsync(PlayingTrack, cts.Token).ConfigureAwait(continueOnCapturedContext: false);
				await this.OnTrackEndAsync(AudioClient, PlayingTrack).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public void Stop ()
		{
			try
			{
				cts?.Cancel(throwOnFirstException: false);
			}
			catch (ObjectDisposedException)
			{
			}
			cts?.Dispose();
		}

		protected unsafe static byte[] AdjustVolume (byte[] audioSamples, double volume)
		{
			if (Math.Abs(volume - 1.0) < 9.9999997473787516E-05)
			{
				return audioSamples;
			}
			int num = (int) Math.Round(volume * 65536.0);
			int num2 = audioSamples.Length / 2;
			fixed (byte* ptr = audioSamples)
			{
				short* ptr2 = (short*) ptr;
				int num3 = num2;
				while (num3 != 0)
				{
					*ptr2 = (short) (*ptr2 * num >> 16);
					num3--;
					ptr2++;
				}
			}
			return audioSamples;
		}

		protected void ResetStreams ()
		{
			DiscordStream?.Flush();
			PlayingTrack?.Dispose();
		}

		~AudioPlayer ()
		{
			DiscordStream?.Dispose();
			PlayingTrack?.Dispose();
			cts?.Dispose();
		}
	}
}

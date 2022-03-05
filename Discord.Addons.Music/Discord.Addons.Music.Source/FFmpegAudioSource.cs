using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Music.Object;

namespace Discord.Addons.Music.Source
{

	public abstract class FFmpegAudioSource : IAudioSource, IDisposable
	{
		private bool disposedValue;

		public Stream SourceStream { get; set; }

		public Process FFmpegProcess { get; set; }

		public IAudioInfo Info { get; set; }

		public string Url { get; set; }

		public abstract byte[] GetBufferFrame ();

		public abstract bool IsOpus ();

		public abstract void LoadProcess ();

		public abstract Task<int> Provide20msAudio (CancellationToken ct);

		protected virtual void Dispose (bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				SourceStream?.Dispose();
				FFmpegProcess.Dispose();
				SourceStream = null;
				FFmpegProcess = null;
				disposedValue = true;
			}
		}

		~FFmpegAudioSource ()
		{
			Dispose(disposing: false);
		}

		public void Dispose ()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Addons.Music.Source
{

	public class AudioTrack : FFmpegAudioSource
	{
		public byte[] BufferFrame = new byte[1024];

		public override void LoadProcess ()
		{
			string arguments = "/C youtube-dl.exe --format bestaudio --audio-quality 0 -o - " + base.Url + " | ffmpeg.exe -loglevel warning -re -vn -i pipe:0 -f s16le -b:a 128k -ar 48000 -ac 2 pipe:1";
			base.FFmpegProcess = Process.Start(new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = arguments,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			});
			base.SourceStream = base.FFmpegProcess.StandardOutput.BaseStream;
		}

		public override async Task<int> Provide20msAudio (CancellationToken ct)
		{
			return await base.SourceStream.ReadAsync(BufferFrame, 0, BufferFrame.Length, ct).ConfigureAwait(continueOnCapturedContext: false);
		}

		public override bool IsOpus ()
		{
			return false;
		}

		public override byte[] GetBufferFrame ()
		{
			return BufferFrame;
		}

		public AudioTrack MakeClone ()
		{
			Stream destination = new MemoryStream();
			base.SourceStream.CopyTo(destination);
			return new AudioTrack
			{
				Url = base.Url,
				SourceStream = null,
				FFmpegProcess = null,
				Info = base.Info
			};
		}
	}

}
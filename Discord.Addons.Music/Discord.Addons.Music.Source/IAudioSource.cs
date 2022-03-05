using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Addons.Music.Object;

namespace Discord.Addons.Music.Source
{

	public interface IAudioSource : IDisposable
	{
		IAudioInfo Info { get; set; }

		string Url { get; set; }

		Task<int> Provide20msAudio (CancellationToken ct);

		bool IsOpus ();

		void LoadProcess ();

		byte[] GetBufferFrame ();
	}

}
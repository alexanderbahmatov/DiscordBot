using System.Diagnostics;
using System.IO;

namespace Discord.Addons.Music.Source
{

	internal interface IFFmpegAudioSource
	{
		Stream SourceStream { get; set; }

		Process FFmpegProcess { get; set; }
	}
}

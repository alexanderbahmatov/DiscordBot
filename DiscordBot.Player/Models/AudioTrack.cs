using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Player.Models
{
    public class AudioTrack : IDisposable
    {
        private byte[] _buffer = new byte[1024];
        private string _url;
        private Process _ffmpegProcess;
        private Stream _sourceStream;
        public Data Info { get; set; }
        public AudioTrack (string url, Data info)
        {
            _url = url;
            Info = info;
        }

        public Task LoadProcess ()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {

                    string command = $"/C youtube-dl --format bestaudio --audio-quality 0 -o - {_url} | " +
                   "ffmpeg -loglevel warning -re -vn -i pipe:0 -f s16le -b:a 128k -ar 48000 -ac 2 pipe:1";
                    _ffmpegProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = command,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                } else
                {
                    string command = $"-c \"youtube-dl --format bestaudio --audio-quality 0 -o - {_url} | " +
                   "ffmpeg -loglevel warning -re -vn -i pipe:0 -f s16le -b:a 128k -ar 48000 -ac 2 pipe:1\"";
                    _ffmpegProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "bash",
                        Arguments = command,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }
                _sourceStream = _ffmpegProcess.StandardOutput.BaseStream;
            }
            catch(Exception e)
            {

            }
            return Task.CompletedTask;
        }

        public byte[] GetBuffer ()
        {
            return _buffer;
        }

        public void Dispose ()
        {
            _ffmpegProcess.Dispose();
            _sourceStream.Dispose();
        }

        public async Task<int> Provide20msAudio (CancellationToken ct)
        {
            var bytes = await _sourceStream.ReadAsync(_buffer, 0, _buffer.Length, ct).ConfigureAwait(false);
            return bytes;
        }
    }
}

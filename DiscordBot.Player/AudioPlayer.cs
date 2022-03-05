using DiscordBot.Player.Common;
using DiscordBot.Player.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Player
{
    public class AudioPlayer
    {
        private bool _paused;
        private CancellationTokenSource _cts;
        public Queue<AudioTrack> Tracks { get; set; } = new Queue<AudioTrack>();
        public AudioTrack Current { get; set; }
        private Stream _out { get; set; }
        public void Enqueue (string query)
        {
            var tracks = InfoProvider.LoadAudioTracks(query).ConfigureAwait(false).GetAwaiter().GetResult();
            foreach (var track in tracks)
            {
                Tracks.Enqueue(track);
            }
        }
        public void Enqueue (AudioTrack track)
        {
            Tracks.Enqueue(track);
        }
        public void Enqueue (List<AudioTrack> tracks)
        {
            foreach (var track in tracks)
            {
                Tracks.Enqueue(track);
            }
        }
        public void Clear ()
        {
            Stop();
        }
        public async Task Play (Stream outPut)
        {
            _cts?.Dispose();

            _out = outPut;
            while (Tracks.Count > 0)
            {
                await Stop();
                Current = null;
                _cts?.Dispose();
                await PlayInternal();
            }
            Reset();
        }

        public Task Stop()
        {
            try
            {
                _cts?.Cancel(false);
            }
            catch (ObjectDisposedException)
            { }
            _cts?.Dispose();
            return Task.CompletedTask;
        }
        public void Pause()
        {
            _paused = !_paused;
        }
        public async Task Next()
        {
            Current = null;
            _cts?.Dispose();
            if (Tracks.Count == 0)
            {
                return;
            } 
            while (Tracks.Count > 0)
            {
                await Stop();
                Current = null;
                _cts?.Dispose();
                await PlayInternal();
            }
            Reset();
        }

        private async Task PlayInternal()
        {
            _cts = new CancellationTokenSource();
            _paused = false;
            Current = Tracks.Dequeue();
            await Current.LoadProcess();
            int read = -1;
            while (true)
            {
                if (_cts.Token.IsCancellationRequested)
                {
                    return;
                }

                if (!_paused)
                {
                    // Read audio byte sample
                    Console.WriteLine("providing");
                    read = await Current.Provide20msAudio(_cts.Token).ConfigureAwait(false);
                    if (read > 0)
                    {
                        await _out.WriteAsync(Current.GetBuffer(), 0, read, _cts.Token).ConfigureAwait(false);
                    }
                    // Finished playing
                    else
                    {
                        return;
                    }
                }
                else
                {
                    await Task.Delay(4000);
                }
            }
        }

        private void Reset()
        {
            _out?.Flush();
            Current?.Dispose();
        }

        ~AudioPlayer ()
        {
            _out?.Dispose();
            Current?.Dispose();
            _cts?.Dispose();
        }
    }
}

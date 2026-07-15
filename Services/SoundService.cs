using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace AndonVoiceBroadcaster.Services
{
    public sealed class SoundService : IDisposable
    {
        private readonly SoundPlayer _player;
        private readonly string _soundPath;

        public SoundService(string soundPath)
        {
            _soundPath = soundPath;
            _player = new SoundPlayer(soundPath);
        }

        public bool Exists
        {
            get { return File.Exists(_soundPath); }
        }

        public Task PlayAsync(CancellationToken cancellationToken)
        {
            if (!Exists)
            {
                throw new FileNotFoundException("未找到提示音文件。", _soundPath);
            }
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                _player.PlaySync();
                cancellationToken.ThrowIfCancellationRequested();
            }, cancellationToken);
        }

        public void Stop()
        {
            _player.Stop();
        }

        public void Dispose()
        {
            _player.Dispose();
        }
    }
}

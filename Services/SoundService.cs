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

        public Task<bool> PlayAsync(CancellationToken cancellationToken)
        {
            if (!Exists)
            {
                throw new FileNotFoundException("未找到提示音文件。", _soundPath);
            }
            // 在线程池中同步播放，使调用方可以等待 WAV 播放完成。
            return Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }
                _player.PlaySync();
                return !cancellationToken.IsCancellationRequested;
            });
        }

        public void Stop()
        {
            // SoundPlayer 也属于系统音频组件，停止操作不能阻塞 UI 或退出线程。
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    ((SoundPlayer)state).Stop();
                }
                catch (Exception)
                {
                }
            }, _player);
        }

        public void Dispose()
        {
            _player.Dispose();
        }
    }
}

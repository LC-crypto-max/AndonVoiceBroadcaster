using System;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;

namespace AndonVoiceBroadcaster.Services
{
    public sealed class SpeechService : IDisposable
    {
        private readonly object _syncRoot = new object();
        private SpeechSynthesizer _activeSynthesizer;
        private string _selectedVoiceName;

        public string EnsureChineseVoice(string preferredVoiceName)
        {
            using (var synthesizer = new SpeechSynthesizer())
            {
                var voices = synthesizer.GetInstalledVoices()
                    .Where(v => v.Enabled && string.Equals(v.VoiceInfo.Culture.Name, "zh-CN", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (voices.Count == 0)
                {
                    throw new InvalidOperationException("未检测到可用的 Windows 中文语音（zh-CN）。");
                }

                var selected = voices.FirstOrDefault(v => string.Equals(v.VoiceInfo.Name, preferredVoiceName, StringComparison.OrdinalIgnoreCase))
                    ?? voices.First();
                _selectedVoiceName = selected.VoiceInfo.Name;
                return _selectedVoiceName;
            }
        }

        public Task SpeakAsync(string text, int rate, int volume, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_selectedVoiceName))
            {
                throw new InvalidOperationException("中文语音尚未初始化。");
            }

            var completion = new TaskCompletionSource<bool>();
            var worker = new Thread(() => SpeakOnStaThread(text, rate, volume, cancellationToken, completion));
            worker.IsBackground = true;
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();
            return completion.Task;
        }

        public void Cancel()
        {
            lock (_syncRoot)
            {
                if (_activeSynthesizer != null)
                {
                    _activeSynthesizer.SpeakAsyncCancelAll();
                }
            }
        }

        private void SpeakOnStaThread(string text, int rate, int volume, CancellationToken cancellationToken, TaskCompletionSource<bool> completion)
        {
            try
            {
                using (var synthesizer = new SpeechSynthesizer())
                using (var completed = new ManualResetEvent(false))
                {
                    Exception playbackError = null;
                    bool cancelled = false;
                    synthesizer.SelectVoice(_selectedVoiceName);
                    synthesizer.Rate = rate;
                    synthesizer.Volume = volume;
                    synthesizer.SpeakCompleted += (sender, args) =>
                    {
                        playbackError = args.Error;
                        cancelled = args.Cancelled;
                        completed.Set();
                    };

                    lock (_syncRoot)
                    {
                        _activeSynthesizer = synthesizer;
                    }

                    using (cancellationToken.Register(() => synthesizer.SpeakAsyncCancelAll()))
                    {
                        synthesizer.SpeakAsync(text);
                        completed.WaitOne();
                    }

                    if (playbackError != null)
                    {
                        completion.TrySetException(playbackError);
                    }
                    else if (cancelled || cancellationToken.IsCancellationRequested)
                    {
                        completion.TrySetCanceled();
                    }
                    else
                    {
                        completion.TrySetResult(true);
                    }
                }
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
            finally
            {
                lock (_syncRoot)
                {
                    _activeSynthesizer = null;
                }
            }
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}

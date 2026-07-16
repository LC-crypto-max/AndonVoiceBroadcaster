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
        private TaskCompletionSource<bool> _activeCompletion;
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
                // 优先使用配置的中文语音，不存在时选择首个可用语音。
                _selectedVoiceName = selected.VoiceInfo.Name;
                return _selectedVoiceName;
            }
        }

        public Task<bool> SpeakAsync(string text, int rate, int volume, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_selectedVoiceName))
            {
                throw new InvalidOperationException("中文语音尚未初始化。");
            }

            // 若测试按钮与正式播报发生重叠，先终止旧朗读，保证同一时刻只有一个活动任务。
            Cancel();
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_syncRoot)
            {
                _activeCompletion = completion;
            }
            // 在独立 STA 线程中朗读，避免阻塞 WinForms UI。
            var worker = new Thread(() => SpeakOnStaThread(text, rate, volume, cancellationToken, completion));
            worker.IsBackground = true;
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();
            return completion.Task;
        }

        public void Cancel()
        {
            SpeechSynthesizer synthesizer;
            TaskCompletionSource<bool> completion;
            lock (_syncRoot)
            {
                synthesizer = _activeSynthesizer;
                completion = _activeCompletion;
                _activeSynthesizer = null;
                _activeCompletion = null;
            }

            // 先解除协调器的等待，再异步通知底层 TTS；避免系统语音组件卡住退出线程。
            if (completion != null)
            {
                completion.TrySetResult(false);
            }
            RequestCancel(synthesizer);
        }

        private void SpeakOnStaThread(string text, int rate, int volume, CancellationToken cancellationToken, TaskCompletionSource<bool> completion)
        {
            try
            {
                if (completion.Task.IsCompleted || cancellationToken.IsCancellationRequested)
                {
                    completion.TrySetResult(false);
                    return;
                }

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
                        try
                        {
                            completed.Set();
                        }
                        catch (ObjectDisposedException)
                        {
                            // 取消令牌可能先结束等待；忽略随后到达的 TTS 完成事件。
                        }
                    };

                    bool isCurrent;
                    lock (_syncRoot)
                    {
                        isCurrent = ReferenceEquals(_activeCompletion, completion) && !completion.Task.IsCompleted;
                        if (isCurrent)
                        {
                            _activeSynthesizer = synthesizer;
                        }
                    }
                    if (!isCurrent)
                    {
                        RequestCancel(synthesizer);
                        return;
                    }

                    // CancellationToken 回调必须快速返回，不能被底层 COM/TTS 调用拖住。
                    using (cancellationToken.Register(() =>
                    {
                        completion.TrySetResult(false);
                        RequestCancel(synthesizer);
                    }))
                    {
                        synthesizer.SpeakAsync(text);
                        WaitHandle.WaitAny(new[] { completed, cancellationToken.WaitHandle });
                    }

                    if (playbackError != null)
                    {
                        completion.TrySetException(playbackError);
                    }
                    else if (cancelled || cancellationToken.IsCancellationRequested)
                    {
                        completion.TrySetResult(false);
                    }
                    else
                    {
                        completion.TrySetResult(true);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                completion.TrySetResult(false);
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
            finally
            {
                lock (_syncRoot)
                {
                    if (ReferenceEquals(_activeCompletion, completion))
                    {
                        _activeSynthesizer = null;
                        _activeCompletion = null;
                    }
                }
            }
        }

        private static void RequestCancel(SpeechSynthesizer synthesizer)
        {
            if (synthesizer == null)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    ((SpeechSynthesizer)state).SpeakAsyncCancelAll();
                }
                catch (Exception)
                {
                }
            }, synthesizer);
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}

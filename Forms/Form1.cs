using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AndonVoiceBroadcaster.Infrastructure;
using AndonVoiceBroadcaster.Models;
using AndonVoiceBroadcaster.Services;

namespace AndonVoiceBroadcaster.Forms
{
    public partial class Form1 : Form
    {
        private readonly string _applicationDirectory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
        private SettingsService _settingsService;
        private AppSettings _settings;
        private MesCredentials _credentials;
        private FileLogger _logger;
        private MesApiClient _apiClient;
        private SpeechService _speechService;
        private SoundService _soundService;
        private BroadcastCoordinator _coordinator;
        private bool _allowExit;
        private bool _shutdownStarted;
        private bool _shutdownCompleted;
        private int _exitWatchdogStarted;

        public Form1()
        {
            InitializeComponent();
        }

        private void MainFormOnLoad(object sender, EventArgs eventArgs)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime || DesignMode) return;
            _notifyIcon.Icon = SystemIcons.Information;
            _notifyIcon.Visible = true;
            // 普通设置与 DPAPI 加密的 MES 凭据分开加载。
            _settingsService = new SettingsService(_applicationDirectory);
            _settings = _settingsService.LoadSettings();
            _credentials = _settingsService.LoadCredentials();
            _logger = new FileLogger(Path.Combine(_applicationDirectory, "logs"), _settings.LogRetentionDays);
            _logger.Written += WriteLogToScreen;
            _logger.Info("程序启动。");

            StartCoordinator();

            if (_settings.StartMinimized || Environment.GetCommandLineArgs().Any(x => string.Equals(x, "--minimized", StringComparison.OrdinalIgnoreCase)))
            {
                BeginInvoke(new Action(HideToTray));
            }
        }

        private void StartCoordinator()
        {
            // 设置变化后释放旧服务，再按新配置重新初始化。
            StopCoordinator();
            _settings.Normalize();
            new StartupService().Configure(_settings.StartWithWindows, Application.ExecutablePath);
            _terminalValue.Text = _settings.TerminalCode;
            string workshopText = _settings.WorkshopNames != null && _settings.WorkshopNames.Count > 0
                ? string.Join(",", _settings.WorkshopNames)
                : (_settings.WorkshopIdList != null && _settings.WorkshopIdList.Count > 0
                    ? string.Join(",", _settings.WorkshopIdList)
                    : "未限制（可选）");
            string factoryText = !string.IsNullOrWhiteSpace(_settings.FactoryName)
                ? _settings.FactoryName
                : (_settings.FactoryId.HasValue ? "工厂 " + _settings.FactoryId.Value : "当前登录工厂");
            _workshopValue.Text = factoryText + " / " + workshopText;

            try
            {
                _soundService = new SoundService(Path.Combine(_applicationDirectory, "audio", "AlarmMusic.wav"));
                _speechService = new SpeechService();
                string voiceName = _speechService.EnsureChineseVoice(_settings.PreferredVoiceName);
                _logger.Info("中文语音初始化成功：" + voiceName);

                // 本地声音测试不依赖 MES；配置完整后才启动任务轮询。
                if (!_settings.IsComplete() || !_credentials.IsComplete())
                {
                    _pauseButton.Enabled = false;
                    SetStatus("本地播报测试就绪（MES 未配置）");
                    _logger.Info("MES 未配置，当前仅启用本地提示音和中文语音测试。");
                    return;
                }

                _apiClient = new MesApiClient(_settings.MesBaseUrl, _settings.LoginEndpoint);
                _coordinator = new BroadcastCoordinator(_settings, _credentials, _apiClient, _speechService, _soundService, _logger);
                _coordinator.StatusChanged += SetStatus;
                _coordinator.CurrentTaskChanged += ShowCurrentTask;
                _coordinator.TaskListChanged += ShowTaskList;
                _pauseButton.Enabled = true;
                _coordinator.Start();
            }
            catch (Exception exception)
            {
                SetStatus("初始化失败");
                _logger.Error("客户端初始化失败。", exception);
                MessageBox.Show(exception.Message, "安灯语音播报", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopCoordinator()
        {
            BroadcastCoordinator coordinator = _coordinator;
            MesApiClient apiClient = _apiClient;
            SpeechService speechService = _speechService;
            SoundService soundService = _soundService;
            _coordinator = null;
            _apiClient = null;
            _speechService = null;
            _soundService = null;

            if (coordinator != null)
            {
                try
                {
                    Task stopTask = coordinator.StopAsync();
                    Task completed = Task.WhenAny(stopTask, Task.Delay(TimeSpan.FromSeconds(5))).GetAwaiter().GetResult();
                    if (completed == stopTask)
                    {
                        stopTask.GetAwaiter().GetResult();
                    }
                    else
                    {
                        _logger.Warn("重载设置时协调器停止超时，将继续释放旧资源。");
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error("停止播报协调器失败。", exception);
                }
            }

            Task disposeTask = Task.Run(() => DisposeServiceSet(coordinator, apiClient, speechService, soundService));
            if (!disposeTask.Wait(TimeSpan.FromSeconds(2)))
            {
                _logger.Warn("旧音频资源在 2 秒内未释放完成，已从界面线程脱离处理。");
            }
        }

        private void PauseButtonOnClick(object sender, EventArgs eventArgs)
        {
            TogglePause();
        }

        private void TogglePause()
        {
            // 每小时重试导写 9：与协调器的新暂停状态保持一致，主窗口和托盘都从这里操作。
            // 1. 故障等待期间点击“继续播报”，仍调用 Resume，让它取消等待并立即尝试 MES 请求。
            // 2. 主动暂停调用 Pause，只能由用户继续解除；协调器的 IsPaused 需覆盖手动暂停与故障等待。
            if (_coordinator == null)
            {
                return;
            }
            if (_coordinator.IsPaused)
            {
                _coordinator.Resume();
                _pauseButton.Text = "暂停播报";
            }
            else
            {
                _coordinator.Pause();
                _pauseButton.Text = "继续播报";
            }
        }

        private async Task RunTestAsync(bool speech)
        {
            try
            {
                using (var cancellation = new CancellationTokenSource())
                {
                    if (speech)
                    {
                        if (_speechService == null)
                        {
                            throw new InvalidOperationException("中文语音未初始化，请确认 Windows 已安装 zh-CN 中文语音包。");
                        }
                        _logger.Info("开始播放本地测试文本：" + _settings.TestSpeechText);
                        await _speechService.SpeakAsync(_settings.TestSpeechText, _settings.TtsRate, _settings.TtsVolume, cancellation.Token);
                        _logger.Info("本地测试文本播放完成。");
                    }
                    else
                    {
                        if (_soundService == null)
                        {
                            throw new InvalidOperationException("提示音服务未初始化。");
                        }
                        await _soundService.PlayAsync(cancellation.Token);
                        _logger.Info("本地提示音播放完成。");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error("音频测试失败。", exception);
                MessageBox.Show(exception.Message, "音频测试失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task OpenSettingsAsync()
        {
            using (var form = new SettingsForm(_settingsService, _settings, _credentials))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Enabled = false;
                    SetStatus("正在应用新设置");
                    try
                    {
                        // 停止旧协调器可能涉及网络和系统音频，不能阻塞 WinForms 消息循环。
                        await Task.Run(() => StopCoordinator());
                        if (_shutdownStarted)
                        {
                            return;
                        }
                        _settings = _settingsService.LoadSettings();
                        _credentials = _settingsService.LoadCredentials();
                        StartCoordinator();
                    }
                    finally
                    {
                        if (!_shutdownStarted && !IsDisposed)
                        {
                            Enabled = true;
                        }
                    }
                }
            }
        }

        private void SetStatus(string status)
        {
            // 每小时重试导写 10：复用现有状态栏和按钮，显示故障等待与自动恢复的结果。
            // 1. 接收协调器发出的“故障等待，下次重试 HH:mm”状态；实际查询成功后再显示“运行中”。
            // 2. 故障等待时按钮显示“继续播报”，自动恢复后显示“暂停播报”，保留现有 UI 线程切换方式。
            InvokeIfAvailable(() =>
            {
                _statusValue.Text = status;
                if (_coordinator != null)
                {
                    _pauseButton.Text = _coordinator.IsPaused ? "继续播报" : "暂停播报";
                }
            });
        }

        private void ShowCurrentTask(VoiceBroadcastTask task)
        {
            InvokeIfAvailable(() =>
            {
                _currentTaskValue.Text = task == null
                    ? "无"
                    : string.Format("#{0}  {1}/{2}", task.Id, task.BroadcastedNum, task.VoiceNum);
            });
        }

        private void ShowTaskList(List<VoiceBroadcastTask> tasks)
        {
            InvokeIfAvailable(() =>
            {
                _taskList.BeginUpdate();
                _taskList.Items.Clear();
                foreach (VoiceBroadcastTask task in tasks)
                {
                    var item = new ListViewItem(task.Id.ToString());
                    item.SubItems.Add(task.ComeType ?? string.Empty);
                    item.SubItems.Add(string.Join(" / ", new[] { task.LineName, task.WorkstationName }.Where(x => !string.IsNullOrWhiteSpace(x))));
                    item.SubItems.Add(task.Content ?? string.Empty);
                    item.SubItems.Add(task.BroadcastedNum + "/" + task.VoiceNum);
                    item.SubItems.Add(task.State == "0" ? "待播报" : "已播报");
                    _taskList.Items.Add(item);
                }
                _taskList.EndUpdate();
            });
        }

        private void WriteLogToScreen(string line)
        {
            InvokeIfAvailable(() =>
            {
                _logTextBox.AppendText(line + Environment.NewLine);
                _logTextBox.SelectionStart = _logTextBox.TextLength;
                _logTextBox.ScrollToCaret();
            });
        }

        private void InvokeIfAvailable(Action action)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }
            if (InvokeRequired)
            {
                // 后台事件必须切换到 WinForms UI 线程更新控件。
                BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        private async void MainFormOnClosing(object sender, FormClosingEventArgs eventArgs)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime || DesignMode) return;
            // 普通关闭只隐藏到托盘；明确退出时才停止播报。
            if (!_allowExit && eventArgs.CloseReason == CloseReason.UserClosing)
            {
                eventArgs.Cancel = true;
                HideToTray();
                return;
            }

            if (_shutdownCompleted)
            {
                return;
            }
            eventArgs.Cancel = true;
            if (_shutdownStarted)
            {
                return;
            }
            _shutdownStarted = true;
            StartExitWatchdog();
            Enabled = false;
            SetStatus("正在安全退出");
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
            }

            try
            {
                await StopCoordinatorForExitAsync();
            }
            catch (Exception exception)
            {
                if (_logger != null)
                {
                    _logger.Error("安全退出过程中发生异常，将继续关闭程序。", exception);
                }
            }
            finally
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Dispose();
                    _notifyIcon = null;
                }
                if (_logger != null)
                {
                    _logger.Info("程序已退出。");
                }
                _shutdownCompleted = true;
                Close();
                Application.ExitThread();
            }
        }

        private async Task StopCoordinatorForExitAsync()
        {
            BroadcastCoordinator coordinator = _coordinator;
            MesApiClient apiClient = _apiClient;
            SpeechService speechService = _speechService;
            SoundService soundService = _soundService;
            _coordinator = null;
            _apiClient = null;
            _speechService = null;
            _soundService = null;

            if (coordinator != null)
            {
                Task stopTask = coordinator.StopAsync();
                // 限制退出等待时间，避免后台服务异常时窗口无法关闭。
                Task completed = await Task.WhenAny(stopTask, Task.Delay(TimeSpan.FromSeconds(5)));
                if (completed != stopTask && _logger != null)
                {
                    _logger.Warn("播报协调器在 5 秒内未完全停止，将继续释放资源并退出程序。");
                }
                else
                {
                    try
                    {
                        await stopTask;
                    }
                    catch (Exception exception)
                    {
                        if (_logger != null)
                        {
                            _logger.Error("停止播报协调器失败。", exception);
                        }
                    }
                }
            }

            Task disposeTask = Task.Run(() => DisposeServiceSet(coordinator, apiClient, speechService, soundService));
            if (await Task.WhenAny(disposeTask, Task.Delay(TimeSpan.FromSeconds(2))) != disposeTask && _logger != null)
            {
                _logger.Warn("音频资源释放超时，将由退出看门狗保证进程结束。");
            }
        }

        private void DisposeServiceSet(
            BroadcastCoordinator coordinator,
            MesApiClient apiClient,
            SpeechService speechService,
            SoundService soundService)
        {
            TryDispose(coordinator, "播报协调器");
            TryDispose(apiClient, "MES 客户端");
            TryDispose(speechService, "中文语音服务");
            TryDispose(soundService, "提示音服务");
        }

        private void TryDispose(IDisposable service, string name)
        {
            if (service == null)
            {
                return;
            }

            try
            {
                service.Dispose();
            }
            catch (Exception exception)
            {
                if (_logger != null)
                {
                    _logger.Error("释放" + name + "失败。", exception);
                }
            }
        }

        private void StartExitWatchdog()
        {
            if (Interlocked.Exchange(ref _exitWatchdogStarted, 1) != 0)
            {
                return;
            }

            // 用户确认退出后，即使系统音频组件永久阻塞，也保证进程最终结束。
            var watchdog = new Thread(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                try
                {
                    if (_logger != null)
                    {
                        _logger.Warn("正常退出超过 10 秒，执行进程级兜底退出。");
                    }
                }
                catch
                {
                }
                Environment.Exit(0);
            });
            watchdog.IsBackground = true;
            watchdog.Start();
        }

        private void HideToTray()
        {
            Hide();
            ShowInTaskbar = false;
            _notifyIcon.ShowBalloonTip(1000, "安灯语音播报", "程序仍在系统托盘运行，播放未停止。", ToolTipIcon.Info);
        }

        private void RestoreWindow()
        {
            Show();
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void OpenLogDirectory()
        {
            string directory = Path.Combine(_applicationDirectory, "logs");
            Directory.CreateDirectory(directory);
            Process.Start(new ProcessStartInfo(directory) { UseShellExecute = true });
        }

        private void ExitProgram()
        {
            if (MessageBox.Show("确认退出安灯语音播报程序吗？退出后将停止接收和播放安灯语音。", "确认退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            StartExitWatchdog();
            _allowExit = true;
            Close();
        }

        private void RestoreWindowOnClick(object sender, EventArgs e)
        {
            RestoreWindow();
        }

        private async void TestSoundButtonOnClick(object sender, EventArgs e)
        {
            await RunTestAsync(false);
        }

        private async void TestSpeechButtonOnClick(object sender, EventArgs e)
        {
            await RunTestAsync(true);
        }

        private async void SettingsButtonOnClick(object sender, EventArgs e)
        {
            await OpenSettingsAsync();
        }

        private void OpenLogDirectoryOnClick(object sender, EventArgs e)
        {
            OpenLogDirectory();
        }

        private void ExitProgramOnClick(object sender, EventArgs e)
        {
            ExitProgram();
        }
    }
}

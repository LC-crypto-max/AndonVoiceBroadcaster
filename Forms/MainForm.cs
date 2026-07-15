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
    public sealed class MainForm : Form
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
        private NotifyIcon _notifyIcon;
        private Label _statusValue;
        private Label _terminalValue;
        private Label _workshopValue;
        private Label _currentTaskValue;
        private Button _pauseButton;
        private ListView _taskList;
        private TextBox _logTextBox;
        private bool _allowExit;

        public MainForm()
        {
            InitializeComponent();
            Load += MainFormOnLoad;
            FormClosing += MainFormOnClosing;
        }

        private void InitializeComponent()
        {
            Text = "MES 安灯语音播报";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(880, 580);
            Size = new Size(1020, 690);

            var topPanel = new TableLayoutPanel { Dock = DockStyle.Top, Height = 115, ColumnCount = 4, RowCount = 2, Padding = new Padding(12) };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            _statusValue = AddStatusField(topPanel, "运行状态", 0, 0);
            _terminalValue = AddStatusField(topPanel, "终端编号", 2, 0);
            _workshopValue = AddStatusField(topPanel, "播报车间", 0, 1);
            _currentTaskValue = AddStatusField(topPanel, "当前任务", 2, 1);

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(12, 4, 12, 4) };
            _pauseButton = new Button { Text = "暂停播报", Width = 100 };
            _pauseButton.Click += PauseButtonOnClick;
            var testSoundButton = new Button { Text = "测试提示音", Width = 100 };
            testSoundButton.Click += async (sender, args) => await RunTestAsync(false);
            var testSpeechButton = new Button { Text = "测试中文语音", Width = 115 };
            testSpeechButton.Click += async (sender, args) => await RunTestAsync(true);
            var settingsButton = new Button { Text = "系统设置", Width = 100 };
            settingsButton.Click += (sender, args) => OpenSettings();
            buttonPanel.Controls.Add(_pauseButton);
            buttonPanel.Controls.Add(testSoundButton);
            buttonPanel.Controls.Add(testSpeechButton);
            buttonPanel.Controls.Add(settingsButton);

            _taskList = new ListView { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, GridLines = true };
            _taskList.Columns.Add("任务 ID", 90);
            _taskList.Columns.Add("来源", 80);
            _taskList.Columns.Add("产线/工位", 180);
            _taskList.Columns.Add("播报内容", 400);
            _taskList.Columns.Add("次数", 80);
            _taskList.Columns.Add("状态", 90);

            _logTextBox = new TextBox { Dock = DockStyle.Bottom, Height = 160, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Font = new Font(FontFamily.GenericMonospace, 9) };
            var taskCaption = new Label { Text = "待播报任务（按创建时间 FIFO）", Dock = DockStyle.Top, Height = 26, Padding = new Padding(12, 6, 0, 0) };
            Controls.Add(_taskList);
            Controls.Add(taskCaption);
            Controls.Add(_logTextBox);
            Controls.Add(buttonPanel);
            Controls.Add(topPanel);

            var menu = new ContextMenuStrip();
            menu.Items.Add("打开主窗口", null, (sender, args) => RestoreWindow());
            menu.Items.Add("开始/暂停播报", null, (sender, args) => TogglePause());
            menu.Items.Add("测试提示音", null, async (sender, args) => await RunTestAsync(false));
            menu.Items.Add("测试中文语音", null, async (sender, args) => await RunTestAsync(true));
            menu.Items.Add("系统设置", null, (sender, args) => OpenSettings());
            menu.Items.Add("打开日志目录", null, (sender, args) => OpenLogDirectory());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("退出程序", null, (sender, args) => ExitProgram());
            _notifyIcon = new NotifyIcon { Text = "MES 安灯语音播报", Icon = SystemIcons.Information, ContextMenuStrip = menu, Visible = true };
            _notifyIcon.DoubleClick += (sender, args) => RestoreWindow();
        }

        private static Label AddStatusField(TableLayoutPanel panel, string label, int labelColumn, int row)
        {
            panel.Controls.Add(new Label { Text = label + "：", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, labelColumn, row);
            var value = new Label { Text = "未初始化", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, AutoEllipsis = true };
            panel.Controls.Add(value, labelColumn + 1, row);
            return value;
        }

        private void MainFormOnLoad(object sender, EventArgs eventArgs)
        {
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
            StopCoordinator();
            _settings.Normalize();
            new StartupService().Configure(_settings.StartWithWindows, Application.ExecutablePath);
            _terminalValue.Text = _settings.TerminalCode;
            _workshopValue.Text = _settings.WorkshopNames != null && _settings.WorkshopNames.Count > 0
                ? string.Join(",", _settings.WorkshopNames)
                : (_settings.WorkshopIdList != null && _settings.WorkshopIdList.Count > 0
                    ? string.Join(",", _settings.WorkshopIdList)
                    : "未限制（可选）");

            try
            {
                _soundService = new SoundService(Path.Combine(_applicationDirectory, "audio", "AlarmMusic.wav"));
                _speechService = new SpeechService();
                string voiceName = _speechService.EnsureChineseVoice(_settings.PreferredVoiceName);
                _logger.Info("中文语音初始化成功：" + voiceName);

                // 本地语音验证不依赖 MES；只有服务地址和凭据齐全时才启动任务轮询。
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
            if (_coordinator != null)
            {
                try
                {
                    _coordinator.StopAsync().GetAwaiter().GetResult();
                }
                catch (Exception exception)
                {
                    _logger.Error("停止播报协调器失败。", exception);
                }
                _coordinator.Dispose();
                _coordinator = null;
            }
            if (_apiClient != null)
            {
                _apiClient.Dispose();
                _apiClient = null;
            }
            if (_speechService != null)
            {
                _speechService.Dispose();
                _speechService = null;
            }
            if (_soundService != null)
            {
                _soundService.Dispose();
                _soundService = null;
            }
        }

        private void PauseButtonOnClick(object sender, EventArgs eventArgs)
        {
            TogglePause();
        }

        private void TogglePause()
        {
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

        private void OpenSettings()
        {
            using (var form = new SettingsForm(_settingsService, _settings, _credentials))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _settings = _settingsService.LoadSettings();
                    _credentials = _settingsService.LoadCredentials();
                    StartCoordinator();
                }
            }
        }

        private void SetStatus(string status)
        {
            InvokeIfAvailable(() => _statusValue.Text = status);
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
                BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        private void MainFormOnClosing(object sender, FormClosingEventArgs eventArgs)
        {
            if (!_allowExit && eventArgs.CloseReason == CloseReason.UserClosing)
            {
                eventArgs.Cancel = true;
                HideToTray();
                return;
            }
            StopCoordinator();
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            if (_logger != null)
            {
                _logger.Info("程序已退出。");
            }
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
            _allowExit = true;
            Close();
        }
    }
}

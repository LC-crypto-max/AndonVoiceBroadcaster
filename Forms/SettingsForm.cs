using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AndonVoiceBroadcaster.Models;
using AndonVoiceBroadcaster.Services;

namespace AndonVoiceBroadcaster.Forms
{
    public sealed class SettingsForm : Form
    {
        private readonly SettingsService _settingsService;
        private readonly AppSettings _settings;
        private readonly MesCredentials _credentials;
        private TextBox _mesBaseUrl;
        private TextBox _username;
        private TextBox _password;
        private TextBox _terminalCode;
        private TextBox _workshopNames;
        private NumericUpDown _pollSeconds;
        private NumericUpDown _retrySeconds;
        private NumericUpDown _ttsRate;
        private NumericUpDown _ttsVolume;
        private TextBox _testSpeechText;
        private NumericUpDown _broadcastInterval;
        private CheckBox _startWithWindows;
        private CheckBox _startMinimized;

        public SettingsForm(SettingsService settingsService, AppSettings settings, MesCredentials credentials)
        {
            _settingsService = settingsService;
            _settings = settings;
            _credentials = credentials;
            InitializeComponent();
            BindValues();
        }

        private void InitializeComponent()
        {
            Text = "安灯语音播报 - 系统设置";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(650, 445);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14),
                ColumnCount = 2,
                RowCount = 13,
                AutoSize = false
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _mesBaseUrl = AddTextBox(panel, "MES 服务地址", 0);
            _username = AddTextBox(panel, "登录账号", 1);
            _password = AddTextBox(panel, "登录密码", 2);
            _password.UseSystemPasswordChar = true;
            _terminalCode = AddTextBox(panel, "终端编号", 3);
            _workshopNames = AddTextBox(panel, "车间 ID 或名称（可选，逗号分隔）", 4);
            _pollSeconds = AddNumber(panel, "查询间隔（秒）", 5, 1, 30, 2);
            _retrySeconds = AddNumber(panel, "网络重试（秒）", 6, 1, 300, 5);
            _ttsRate = AddNumber(panel, "TTS 语速", 7, -10, 10, 0);
            _ttsVolume = AddNumber(panel, "TTS 音量", 8, 0, 100, 100);
            _testSpeechText = AddTextBox(panel, "本地测试文本", 9);
            _broadcastInterval = AddNumber(panel, "播报间隔（秒）", 10, 0, 60, 1);

            _startWithWindows = new CheckBox { Text = "当前 Windows 用户登录后自动启动", AutoSize = true, Anchor = AnchorStyles.Left };
            _startMinimized = new CheckBox { Text = "启动后最小化到系统托盘", AutoSize = true, Anchor = AnchorStyles.Left };
            panel.Controls.Add(new Label { Text = "启动选项", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 11);
            var options = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoSize = true };
            options.Controls.Add(_startWithWindows);
            options.Controls.Add(_startMinimized);
            panel.Controls.Add(options, 1, 11);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var saveButton = new Button { Text = "保存", Width = 88, DialogResult = DialogResult.None };
            saveButton.Click += SaveButtonOnClick;
            var cancelButton = new Button { Text = "取消", Width = 88, DialogResult = DialogResult.Cancel };
            buttons.Controls.Add(saveButton);
            buttons.Controls.Add(cancelButton);
            panel.Controls.Add(new Label(), 0, 12);
            panel.Controls.Add(buttons, 1, 12);
            Controls.Add(panel);
            AcceptButton = saveButton;
            CancelButton = cancelButton;
        }

        private static TextBox AddTextBox(TableLayoutPanel panel, string label, int row)
        {
            panel.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, row);
            var textBox = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(3, 5, 3, 5) };
            panel.Controls.Add(textBox, 1, row);
            return textBox;
        }

        private static NumericUpDown AddNumber(TableLayoutPanel panel, string label, int row, decimal minimum, decimal maximum, decimal value)
        {
            panel.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, row);
            var number = new NumericUpDown { Dock = DockStyle.Left, Width = 160, Minimum = minimum, Maximum = maximum, Value = value };
            panel.Controls.Add(number, 1, row);
            return number;
        }

        private void BindValues()
        {
            _mesBaseUrl.Text = _settings.MesBaseUrl;
            _username.Text = _credentials.Username;
            _password.Text = _credentials.Password;
            _terminalCode.Text = _settings.TerminalCode;
            _workshopNames.Text = string.Join(",", (_settings.WorkshopIdList ?? new List<long>()).Select(x => x.ToString())
                .Concat(_settings.WorkshopNames ?? new List<string>()));
            _pollSeconds.Value = _settings.PollIntervalSeconds;
            _retrySeconds.Value = _settings.NetworkRetrySeconds;
            _ttsRate.Value = _settings.TtsRate;
            _ttsVolume.Value = _settings.TtsVolume;
            _testSpeechText.Text = _settings.TestSpeechText;
            _broadcastInterval.Value = _settings.BroadcastIntervalSeconds;
            _startWithWindows.Checked = _settings.StartWithWindows;
            _startMinimized.Checked = _settings.StartMinimized;
        }

        private void SaveButtonOnClick(object sender, EventArgs eventArgs)
        {
            List<string> workshopValues = (_workshopNames.Text ?? string.Empty)
                .Split(new[] { ',', '，', ';', '；' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var workshopIds = new List<long>();
            var workshopNames = new List<string>();
            foreach (string value in workshopValues)
            {
                long workshopId;
                if (long.TryParse(value, out workshopId) && workshopId >= 0)
                {
                    workshopIds.Add(workshopId);
                }
                else
                {
                    workshopNames.Add(value);
                }
            }

            _settings.MesBaseUrl = _mesBaseUrl.Text;
            _settings.TerminalCode = _terminalCode.Text;
            _settings.WorkshopIdList = workshopIds.Distinct().ToList();
            _settings.WorkshopNames = workshopNames;
            _settings.PollIntervalSeconds = Decimal.ToInt32(_pollSeconds.Value);
            _settings.NetworkRetrySeconds = Decimal.ToInt32(_retrySeconds.Value);
            _settings.TtsRate = Decimal.ToInt32(_ttsRate.Value);
            _settings.TtsVolume = Decimal.ToInt32(_ttsVolume.Value);
            _settings.TestSpeechText = _testSpeechText.Text;
            _settings.BroadcastIntervalSeconds = Decimal.ToInt32(_broadcastInterval.Value);
            _settings.StartWithWindows = _startWithWindows.Checked;
            _settings.StartMinimized = _startMinimized.Checked;
            _settings.Normalize();
            bool hasAnyMesSetting = !string.IsNullOrWhiteSpace(_settings.MesBaseUrl)
                || !string.IsNullOrWhiteSpace(_username.Text)
                || !string.IsNullOrWhiteSpace(_password.Text);
            if (hasAnyMesSetting && !_settings.IsComplete())
            {
                MessageBox.Show("MES 服务地址必须是有效的完整地址；如果当前只验证本地语音，可将 MES 地址、账号和密码全部留空。", "设置校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _credentials.Username = _username.Text == null ? string.Empty : _username.Text.Trim();
            _credentials.Password = _password.Text ?? string.Empty;
            if (hasAnyMesSetting && !_credentials.IsComplete())
            {
                MessageBox.Show("配置 MES 联调时，请同时填写登录账号和密码。", "设置校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _settingsService.SaveSettings(_settings);
            _settingsService.SaveCredentials(_credentials);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

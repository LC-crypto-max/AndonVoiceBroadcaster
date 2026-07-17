using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
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
        private ListBox _availableWorkshops;
        private ListBox _selectedWorkshops;
        private Button _refreshWorkshopButton;
        private Button _addWorkshopButton;
        private Button _removeWorkshopButton;
        private Button _addAllButton;
        private Button _removeAllButton;
        private Label _availableWorkshopCount;
        private Label _selectedWorkshopCount;
        private List<WorkshopOption> _workshopOptions;
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
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;
            ClientSize = new Size(900, 700);
            MinimumSize = new Size(820, 650);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                ColumnCount = 2,
                RowCount = 8,
                AutoSize = false
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int row = 0; row <= 3; row++)
            {
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            }
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 118));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

            _mesBaseUrl = AddTextBox(panel, "MES 服务地址", 0);
            _username = AddTextBox(panel, "登录账号", 1);
            _password = AddTextBox(panel, "登录密码", 2);
            _password.UseSystemPasswordChar = true;
            _terminalCode = AddTextBox(panel, "终端编号", 3);

            var workshopGroup = new GroupBox
            {
                Text = "车间播报范围（未选择时默认播报全部车间）",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Margin = new Padding(3, 8, 3, 8)
            };
            var workshopTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                AutoSize = false
            };
            workshopTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            workshopTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
            workshopTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            workshopTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            workshopTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            workshopTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            workshopTable.Controls.Add(new Label { Text = "可用车间", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            workshopTable.Controls.Add(new Label { Text = "已选车间", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 0);

            _availableWorkshops = CreateWorkshopListBox();
            _availableWorkshops.DoubleClick += AddWorkshopButtonOnClick;
            _availableWorkshops.SelectedIndexChanged += WorkshopSelectionChanged;
            workshopTable.Controls.Add(_availableWorkshops, 0, 1);

            _selectedWorkshops = CreateWorkshopListBox();
            _selectedWorkshops.DoubleClick += RemoveWorkshopButtonOnClick;
            _selectedWorkshops.SelectedIndexChanged += WorkshopSelectionChanged;
            workshopTable.Controls.Add(_selectedWorkshops, 2, 1);

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(8, 0, 8, 0)
            };
            _addWorkshopButton = new Button { Text = "添加 >", Width = 90, Height = 26, Margin = new Padding(0, 1, 0, 1) };
            _addWorkshopButton.Click += AddWorkshopButtonOnClick;
            _removeWorkshopButton = new Button { Text = "< 移除", Width = 90, Height = 26, Margin = new Padding(0, 1, 0, 5) };
            _removeWorkshopButton.Click += RemoveWorkshopButtonOnClick;
            _addAllButton = new Button { Text = "全部 >>", Width = 90, Height = 26, Margin = new Padding(0, 1, 0, 1) };
            _addAllButton.Click += AddAllButtonOnClick;
            _removeAllButton = new Button { Text = "<< 清空", Width = 90, Height = 26, Margin = new Padding(0, 1, 0, 1) };
            _removeAllButton.Click += RemoveAllButtonOnClick;
            buttonPanel.Controls.Add(_addWorkshopButton);
            buttonPanel.Controls.Add(_removeWorkshopButton);
            buttonPanel.Controls.Add(_addAllButton);
            buttonPanel.Controls.Add(_removeAllButton);
            workshopTable.Controls.Add(buttonPanel, 1, 0);
            workshopTable.SetRowSpan(buttonPanel, 2);

            _availableWorkshopCount = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            _selectedWorkshopCount = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            _refreshWorkshopButton = new Button { Text = "刷新列表", Dock = DockStyle.Fill, Margin = new Padding(8, 3, 8, 3) };
            _refreshWorkshopButton.Click += RefreshWorkshopButtonOnClick;
            workshopTable.Controls.Add(_availableWorkshopCount, 0, 2);
            workshopTable.Controls.Add(_refreshWorkshopButton, 1, 2);
            workshopTable.Controls.Add(_selectedWorkshopCount, 2, 2);

            workshopGroup.Controls.Add(workshopTable);
            panel.Controls.Add(workshopGroup, 0, 4);
            panel.SetColumnSpan(workshopGroup, 2);

            var parameterGroup = new GroupBox
            {
                Text = "轮询与播报参数",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Margin = new Padding(3, 0, 3, 8)
            };
            var parameterTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 3
            };
            parameterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            parameterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            parameterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            parameterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (int row = 0; row < 3; row++)
            {
                parameterTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            }
            _pollSeconds = AddNumber(parameterTable, "查询间隔（秒）", 0, 1, 30, 2, 0);
            _retrySeconds = AddNumber(parameterTable, "网络重试（秒）", 0, 1, 300, 5, 2);
            _ttsRate = AddNumber(parameterTable, "TTS 语速", 1, -10, 10, 0, 0);
            _ttsVolume = AddNumber(parameterTable, "TTS 音量", 1, 0, 100, 100, 2);
            _testSpeechText = AddTextBox(parameterTable, "本地测试文本", 2, 0);
            _broadcastInterval = AddNumber(parameterTable, "播报间隔（秒）", 2, 0, 60, 1, 2);
            parameterGroup.Controls.Add(parameterTable);
            panel.Controls.Add(parameterGroup, 0, 5);
            panel.SetColumnSpan(parameterGroup, 2);

            _startWithWindows = new CheckBox { Text = "当前 Windows 用户登录后自动启动", AutoSize = true, Anchor = AnchorStyles.Left };
            _startMinimized = new CheckBox { Text = "启动后最小化到系统托盘", AutoSize = true, Anchor = AnchorStyles.Left };
            panel.Controls.Add(new Label { Text = "启动选项", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 6);
            var options = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoSize = true };
            options.Controls.Add(_startWithWindows);
            options.Controls.Add(_startMinimized);
            panel.Controls.Add(options, 1, 6);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var saveButton = new Button { Text = "保存", Width = 88, DialogResult = DialogResult.None };
            saveButton.Click += SaveButtonOnClick;
            var cancelButton = new Button { Text = "取消", Width = 88, DialogResult = DialogResult.Cancel };
            buttons.Controls.Add(saveButton);
            buttons.Controls.Add(cancelButton);
            panel.Controls.Add(new Label(), 0, 7);
            panel.Controls.Add(buttons, 1, 7);
            Controls.Add(panel);
            AcceptButton = saveButton;
            CancelButton = cancelButton;
        }

        private static ListBox CreateWorkshopListBox()
        {
            return new ListBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 2),
                SelectionMode = SelectionMode.MultiExtended,
                HorizontalScrollbar = true,
                IntegralHeight = false
            };
        }

        private static TextBox AddTextBox(TableLayoutPanel panel, string label, int row, int labelColumn = 0)
        {
            panel.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, labelColumn, row);
            var textBox = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(3, 5, 3, 5) };
            panel.Controls.Add(textBox, labelColumn + 1, row);
            return textBox;
        }

        private static NumericUpDown AddNumber(TableLayoutPanel panel, string label, int row, decimal minimum, decimal maximum, decimal value, int labelColumn = 0)
        {
            panel.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, labelColumn, row);
            var number = new NumericUpDown { Dock = DockStyle.Fill, Minimum = minimum, Maximum = maximum, Value = value, Margin = new Padding(3, 5, 3, 5) };
            panel.Controls.Add(number, labelColumn + 1, row);
            return number;
        }

        private void BindValues()
        {
            _mesBaseUrl.Text = _settings.MesBaseUrl;
            _username.Text = _credentials.Username;
            _password.Text = _credentials.Password;
            _terminalCode.Text = _settings.TerminalCode;
            _pollSeconds.Value = _settings.PollIntervalSeconds;
            _retrySeconds.Value = _settings.NetworkRetrySeconds;
            _ttsRate.Value = _settings.TtsRate;
            _ttsVolume.Value = _settings.TtsVolume;
            _testSpeechText.Text = _settings.TestSpeechText;
            _broadcastInterval.Value = _settings.BroadcastIntervalSeconds;
            _startWithWindows.Checked = _settings.StartWithWindows;
            _startMinimized.Checked = _settings.StartMinimized;
            BindSavedWorkshopSelection();
            UpdateWorkshopControls();
        }

        private void SaveButtonOnClick(object sender, EventArgs eventArgs)
        {
            var workshopIds = new List<long>();
            var workshopNames = new List<string>();

            foreach (WorkshopOption option in _selectedWorkshops.Items.OfType<WorkshopOption>())
            {
                if (option.WorkshopId >= 0)
                {
                    workshopIds.Add(option.WorkshopId);
                }
                if (!string.IsNullOrWhiteSpace(option.WorkshopName))
                {
                    workshopNames.Add(option.WorkshopName.Trim());
                }
            }

            string mesBaseUrl = (_mesBaseUrl.Text ?? string.Empty).Trim().TrimEnd('/');
            string username = (_username.Text ?? string.Empty).Trim();
            string password = _password.Text ?? string.Empty;
            bool hasAnyMesSetting = !string.IsNullOrWhiteSpace(mesBaseUrl)
                || !string.IsNullOrWhiteSpace(username)
                || !string.IsNullOrWhiteSpace(password);
            if (hasAnyMesSetting && !Uri.IsWellFormedUriString(mesBaseUrl, UriKind.Absolute))
            {
                MessageBox.Show("MES 服务地址必须是有效的完整地址；如果当前只验证本地语音，可将 MES 地址、账号和密码全部留空。", "设置校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (hasAnyMesSetting && (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)))
            {
                MessageBox.Show("配置 MES 联调时，请同时填写登录账号和密码。", "设置校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 校验通过后再更新共享设置对象，避免无效输入或取消窗口影响正在运行的协调器。
            _settings.MesBaseUrl = mesBaseUrl;
            _settings.TerminalCode = _terminalCode.Text;
            _settings.WorkshopIdList = workshopIds.Distinct().ToList();
            _settings.WorkshopNames = workshopNames.Distinct().ToList();
            _settings.PollIntervalSeconds = Decimal.ToInt32(_pollSeconds.Value);
            _settings.NetworkRetrySeconds = Decimal.ToInt32(_retrySeconds.Value);
            _settings.TtsRate = Decimal.ToInt32(_ttsRate.Value);
            _settings.TtsVolume = Decimal.ToInt32(_ttsVolume.Value);
            _settings.TestSpeechText = _testSpeechText.Text;
            _settings.BroadcastIntervalSeconds = Decimal.ToInt32(_broadcastInterval.Value);
            _settings.StartWithWindows = _startWithWindows.Checked;
            _settings.StartMinimized = _startMinimized.Checked;
            _settings.Normalize();
            _credentials.Username = username;
            _credentials.Password = password;

            _settingsService.SaveSettings(_settings);
            _settingsService.SaveCredentials(_credentials);
            DialogResult = DialogResult.OK;
            Close();
        }

        private async void RefreshWorkshopButtonOnClick(object sender, EventArgs eventArgs)
        {
            string mesBaseUrl = (_mesBaseUrl.Text ?? string.Empty).Trim().TrimEnd('/');
            var credentials = new MesCredentials
            {
                Username = (_username.Text ?? string.Empty).Trim(),
                Password = _password.Text ?? string.Empty
            };
            if (!Uri.IsWellFormedUriString(mesBaseUrl, UriKind.Absolute) || !credentials.IsComplete())
            {
                MessageBox.Show("请先填写完整的 MES 服务地址、账号和密码。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedIds = _selectedWorkshops.Items.OfType<WorkshopOption>()
                .Where(x => x.WorkshopId >= 0)
                .Select(x => x.WorkshopId)
                .ToHashSet();
            var selectedNames = _selectedWorkshops.Items.OfType<WorkshopOption>()
                .Where(x => !string.IsNullOrWhiteSpace(x.WorkshopName))
                .Select(x => x.WorkshopName.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _refreshWorkshopButton.Enabled = false;
            _refreshWorkshopButton.Text = "正在加载...";
            try
            {
                using (var apiClient = new MesApiClient(mesBaseUrl, _settings.LoginEndpoint))
                {
                    await apiClient.LoginAsync(credentials, CancellationToken.None);
                    _workshopOptions = await apiClient.GetWorkshopsAsync(CancellationToken.None);
                }

                _availableWorkshops.Items.Clear();
                _selectedWorkshops.Items.Clear();
                foreach (WorkshopOption option in _workshopOptions
                    .Where(x => x != null)
                    .OrderBy(x => x.WorkshopId)
                    .ThenBy(x => x.WorkshopName))
                {
                    bool selected = selectedIds.Contains(option.WorkshopId)
                        || (!string.IsNullOrWhiteSpace(option.WorkshopName) && selectedNames.Contains(option.WorkshopName.Trim()));
                    (selected ? _selectedWorkshops : _availableWorkshops).Items.Add(option);
                }

                UpdateWorkshopControls();
                MessageBox.Show($"已加载 {_workshopOptions.Count} 个车间。", "刷新成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载车间列表失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _refreshWorkshopButton.Enabled = true;
                _refreshWorkshopButton.Text = "刷新列表";
            }
        }

        private void BindSavedWorkshopSelection()
        {
            _selectedWorkshops.Items.Clear();
            var ids = _settings.WorkshopIdList ?? new List<long>();
            var names = _settings.WorkshopNames ?? new List<string>();
            int pairCount = Math.Max(ids.Count, names.Count);
            for (int i = 0; i < pairCount; i++)
            {
                long id = i < ids.Count ? ids[i] : -1;
                string name = i < names.Count ? names[i] : string.Empty;
                _selectedWorkshops.Items.Add(new WorkshopOption { WorkshopId = id, WorkshopName = name });
            }
        }

        private void WorkshopSelectionChanged(object sender, EventArgs eventArgs)
        {
            UpdateWorkshopControls();
        }

        private void UpdateWorkshopControls()
        {
            _availableWorkshopCount.Text = "共 " + _availableWorkshops.Items.Count + " 个";
            _selectedWorkshopCount.Text = "已选 " + _selectedWorkshops.Items.Count + " 个";
            _addWorkshopButton.Enabled = _availableWorkshops.SelectedItems.Count > 0;
            _removeWorkshopButton.Enabled = _selectedWorkshops.SelectedItems.Count > 0;
            _addAllButton.Enabled = _availableWorkshops.Items.Count > 0;
            _removeAllButton.Enabled = _selectedWorkshops.Items.Count > 0;
        }

        private static void MoveSelectedItems(ListBox source, ListBox target)
        {
            var selected = source.SelectedItems.Cast<object>().ToList();
            foreach (object item in selected)
            {
                target.Items.Add(item);
                source.Items.Remove(item);
            }
        }

        private void AddWorkshopButtonOnClick(object sender, EventArgs eventArgs)
        {
            MoveSelectedItems(_availableWorkshops, _selectedWorkshops);
            UpdateWorkshopControls();
        }

        private void RemoveWorkshopButtonOnClick(object sender, EventArgs eventArgs)
        {
            MoveSelectedItems(_selectedWorkshops, _availableWorkshops);
            UpdateWorkshopControls();
        }

        private void AddAllButtonOnClick(object sender, EventArgs eventArgs)
        {
            while (_availableWorkshops.Items.Count > 0)
            {
                _selectedWorkshops.Items.Add(_availableWorkshops.Items[0]);
                _availableWorkshops.Items.RemoveAt(0);
            }
            UpdateWorkshopControls();
        }

        private void RemoveAllButtonOnClick(object sender, EventArgs eventArgs)
        {
            while (_selectedWorkshops.Items.Count > 0)
            {
                _availableWorkshops.Items.Add(_selectedWorkshops.Items[0]);
                _selectedWorkshops.Items.RemoveAt(0);
            }
            UpdateWorkshopControls();
        }
    }
}

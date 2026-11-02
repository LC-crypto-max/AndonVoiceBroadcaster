using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AndonVoiceBroadcaster.Models;
using AndonVoiceBroadcaster.Services;

namespace AndonVoiceBroadcaster.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly SettingsService _settingsService;
        private readonly AppSettings _settings;
        private readonly MesCredentials _credentials;
        private List<WorkshopOption> _workshopOptions;
        private bool _bindingFactory;

        public SettingsForm()
        {
            InitializeComponent();
        }

        public SettingsForm(SettingsService settingsService, AppSettings settings, MesCredentials credentials) : this()
        {
            _settingsService = settingsService;
            _settings = settings;
            _credentials = credentials;
            BindValues();
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
            _workStartTime.Value = DateTime.Today.Add(_settings.GetWorkStartTime());
            _workEndTime.Value = DateTime.Today.Add(_settings.GetWorkEndTime());
            _startWithWindows.Checked = _settings.StartWithWindows;
            _startMinimized.Checked = _settings.StartMinimized;
            BindSavedFactorySelection();
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
            TimeSpan workStart = _workStartTime.Value.TimeOfDay;
            TimeSpan workEnd = _workEndTime.Value.TimeOfDay;
            if (workEnd <= workStart)
            {
                MessageBox.Show("下班停止时间必须晚于上班开始时间。", "设置校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 校验通过后再更新共享设置对象，避免无效输入或取消窗口影响正在运行的协调器。
            _settings.MesBaseUrl = mesBaseUrl;
            _settings.TerminalCode = _terminalCode.Text;
            FactoryOption selectedFactory = _factory.SelectedItem as FactoryOption;
            _settings.FactoryId = selectedFactory == null ? null : selectedFactory.FactoryId;
            _settings.FactoryName = selectedFactory == null ? string.Empty : selectedFactory.FactoryName;
            _settings.WorkshopIdList = workshopIds.Distinct().ToList();
            _settings.WorkshopNames = workshopNames.Distinct().ToList();
            _settings.PollIntervalSeconds = Decimal.ToInt32(_pollSeconds.Value);
            _settings.NetworkRetrySeconds = Decimal.ToInt32(_retrySeconds.Value);
            _settings.TtsRate = Decimal.ToInt32(_ttsRate.Value);
            _settings.TtsVolume = Decimal.ToInt32(_ttsVolume.Value);
            _settings.TestSpeechText = _testSpeechText.Text;
            _settings.BroadcastIntervalSeconds = Decimal.ToInt32(_broadcastInterval.Value);
            _settings.WorkStartTime = workStart.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
            _settings.WorkEndTime = workEnd.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
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
                FactoryOption currentFactory;
                using (var apiClient = new MesApiClient(mesBaseUrl, _settings.LoginEndpoint))
                {
                    await apiClient.LoginAsync(credentials, CancellationToken.None);
                    _workshopOptions = await apiClient.GetWorkshopsAsync(CancellationToken.None);
                    currentFactory = await apiClient.GetCurrentFactoryAsync(CancellationToken.None);
                }

                BindFactoryOptions(currentFactory);
                BindWorkshopLists(selectedIds, selectedNames);
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

        private void BindSavedFactorySelection()
        {
            _bindingFactory = true;
            try
            {
                _factory.Items.Clear();
                if (_settings.FactoryId.HasValue || !string.IsNullOrWhiteSpace(_settings.FactoryName))
                {
                    _factory.Items.Add(new FactoryOption
                    {
                        FactoryId = _settings.FactoryId,
                        FactoryName = _settings.FactoryName
                    });
                    _factory.SelectedIndex = 0;
                }
            }
            finally
            {
                _bindingFactory = false;
            }
        }

        private void BindFactoryOptions(FactoryOption currentFactory)
        {
            var factories = (_workshopOptions ?? new List<WorkshopOption>())
                .Where(x => x != null && x.TenantId.HasValue)
                .Select(x => x.TenantId.Value)
                .Distinct()
                .OrderBy(x => x)
                .Select(id => new FactoryOption
                {
                    FactoryId = id,
                    FactoryName = currentFactory != null && currentFactory.FactoryId == id
                        ? currentFactory.FactoryName
                        : "工厂 " + id
                })
                .ToList();
            if (currentFactory != null && !factories.Any(x => x.FactoryId == currentFactory.FactoryId))
            {
                factories.Add(currentFactory);
            }
            if (factories.Count == 0)
            {
                factories.Add(currentFactory ?? new FactoryOption { FactoryName = "当前登录工厂" });
            }
            long? preferredId = _settings.FactoryId ?? (currentFactory == null ? null : currentFactory.FactoryId);
            _bindingFactory = true;
            try
            {
                _factory.Items.Clear();
                foreach (FactoryOption factory in factories)
                {
                    _factory.Items.Add(factory);
                }
                int selectedIndex = preferredId.HasValue
                    ? factories.FindIndex(x => x.FactoryId == preferredId)
                    : 0;
                _factory.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
            }
            finally
            {
                _bindingFactory = false;
            }
        }

        private void FactoryOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (_bindingFactory || _workshopOptions == null)
            {
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
            BindWorkshopLists(selectedIds, selectedNames);
        }

        private void BindWorkshopLists(HashSet<long> selectedIds, HashSet<string> selectedNames)
        {
            FactoryOption selectedFactory = _factory.SelectedItem as FactoryOption;
            bool hasFactoryData = (_workshopOptions ?? new List<WorkshopOption>()).Any(x => x != null && x.TenantId.HasValue);
            IEnumerable<WorkshopOption> candidates = (_workshopOptions ?? new List<WorkshopOption>()).Where(x => x != null);
            if (hasFactoryData && selectedFactory != null && selectedFactory.FactoryId.HasValue)
            {
                candidates = candidates.Where(x => x.TenantId == selectedFactory.FactoryId);
            }

            _availableWorkshops.Items.Clear();
            _selectedWorkshops.Items.Clear();
            foreach (WorkshopOption option in candidates.OrderBy(x => x.WorkshopId).ThenBy(x => x.WorkshopName))
            {
                bool selected = selectedIds.Contains(option.WorkshopId)
                    || (!string.IsNullOrWhiteSpace(option.WorkshopName) && selectedNames.Contains(option.WorkshopName.Trim()));
                (selected ? _selectedWorkshops : _availableWorkshops).Items.Add(option);
            }
            UpdateWorkshopControls();
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

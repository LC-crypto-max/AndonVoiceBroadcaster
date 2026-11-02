using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndonVoiceBroadcaster.Infrastructure;
using AndonVoiceBroadcaster.Models;

namespace AndonVoiceBroadcaster.Services
{
    public sealed class BroadcastCoordinator : IDisposable
    {
        private readonly AppSettings _settings;
        private readonly MesCredentials _credentials;
        private readonly MesApiClient _apiClient;
        private readonly SpeechService _speechService;
        private readonly SoundService _soundService;
        private readonly FileLogger _logger;
        private readonly object _syncRoot = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _runTask;
        // 每小时重试导写 1：在这里补充“手动暂停 / 故障等待”的状态、下次重试时间和固定 1 小时间隔。
        // 1. _paused 目前同时表示两种暂停，后续必须能区分原因；故障等待也应对外显示为已暂停。
        // 2. 暂停原因、重试时间和失败次数的关联更新要统一同步，避免 UI 操作与后台恢复互相覆盖。
        private volatile bool _paused;
        private bool _workshopsResolved;
        private int _consecutiveFailureCount;
        private List<long> _resolvedWorkshopIds = new List<long>();

        public BroadcastCoordinator(
            AppSettings settings,
            MesCredentials credentials,
            MesApiClient apiClient,
            SpeechService speechService,
            SoundService soundService,
            FileLogger logger)
        {
            _settings = settings;
            _credentials = credentials;
            _apiClient = apiClient;
            _speechService = speechService;
            _soundService = soundService;
            _logger = logger;
        }

        public event Action<string> StatusChanged;
        public event Action<VoiceBroadcastTask> CurrentTaskChanged;
        public event Action<List<VoiceBroadcastTask>> TaskListChanged;

        public bool IsPaused
        {
            get { return _paused; }
        }

        public void Start()
        {
            // 每小时重试导写 2：启动时初始化恢复状态；已有任务运行时，也要明确本次 Start 是否代表立即继续。
            // 1. 开始新运行时清除旧的故障等待时间与失败计数，避免继承上次的一小时等待。
            // 2. 如保留下面“已有任务就恢复”的行为，需与 Resume 使用相同的状态重置规则。
            lock (_syncRoot)
            {
                if (_runTask != null && !_runTask.IsCompleted)
                {
                    _paused = false;
                    RaiseStatus("运行中");
                    return;
                }
                _paused = false;
                _cancellationTokenSource = new CancellationTokenSource();
                _runTask = Task.Run(() => RunLoopAsync(_cancellationTokenSource.Token));
            }
        }

        public void Pause()
        {
            // 每小时重试导写 3：将这里明确为“用户手动暂停”。
            // 1. 标记手动暂停并撤销自动重试计划，保留下面取消语音和提示音的操作。
            // 2. 即使后台恢复请求正在返回，也不能覆盖手动暂停，更不能自动开始播放。
            _paused = true;
            _speechService.Cancel();
            _soundService.Stop();
            RaiseStatus("已暂停");
            _logger.Info("播报已由用户暂停。");
        }

        public void Resume()
        {
            // 每小时重试导写 4：点击继续时立即尝试，不必等满一小时。
            // 1. 清除手动暂停、故障等待和下次重试时间，再清零失败次数，让主循环重新请求 MES。
            // 2. 这里的“继续”只是开始尝试；是否连接恢复，应等真正查询 MES 成功后再记录。
            _consecutiveFailureCount = 0;
            _paused = false;
            RaiseStatus("运行中");
            _logger.Info("播报已恢复，连续失败计数已清零。");
        }

        public async Task StopAsync()
        {
            Task task;
            CancellationTokenSource cancellation;
            lock (_syncRoot)
            {
                cancellation = _cancellationTokenSource;
                task = _runTask;
            }

            // Cancel 会同步执行所有回调，放到线程池可避免某个系统回调拖住 UI/退出线程。
            if (cancellation != null)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        ((CancellationTokenSource)state).Cancel();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }, cancellation);
            }
            _speechService.Cancel();
            _soundService.Stop();
            if (task != null)
            {
                try
                {
                    await task;
                }
                catch (OperationCanceledException)
                {
                }
            }
            RaiseStatus("已停止");
        }

        public async Task TestSoundAsync(CancellationToken cancellationToken)
        {
            await _soundService.PlayAsync(cancellationToken);
        }

        public async Task TestSpeechAsync(CancellationToken cancellationToken)
        {
            await _speechService.SpeakAsync("这是一条安灯语音播报测试。", _settings.TtsRate, _settings.TtsVolume, cancellationToken);
        }

        private async Task RunLoopAsync(CancellationToken cancellationToken)
        {
            RaiseStatus("正在连接 MES");
            _logger.Info("播报协调器已启动，终端=" + _settings.TerminalCode
                + "，工作时间=" + _settings.WorkStartTime + "-" + _settings.WorkEndTime
                + (string.IsNullOrWhiteSpace(_settings.FactoryName) ? string.Empty : "，工厂=" + _settings.FactoryName));
            // 持续轮询 MES，并按创建时间依次处理待播报任务。
            while (!cancellationToken.IsCancellationRequested)
            {
                int retryDelaySeconds = 0;
                try
                {
                    // 每小时重试导写 5：把下面“只要暂停就跳过”的判断拆成按暂停原因处理。
                    // 1. 手动暂停始终等待；故障等待未到期也只短暂等待，不发起 MES 请求。
                    // 2. 故障等待到期且处于工作时段时，允许进行一次恢复尝试；此时先不要清零失败计数。
                    // 3. 保留故障等待标记直到查询成功，失败时才能直接再等一小时，而非重新短间隔连试三次。
                    // 4. 非工作时间沿用下方工作时段限制，到上班且重试已到期时再尝试；不要提前恢复播报。
                    if (_paused)
                    {
                        await Task.Delay(500, cancellationToken);
                        continue;
                    }

                    DateTime now = DateTime.Now;
                    if (!_settings.IsWithinWorkHours(now))
                    {
                        TimeSpan wait = _settings.GetDelayUntilWorkStarts(now);
                        string hours = _settings.WorkStartTime + "-" + _settings.WorkEndTime;
                        RaiseStatus("非工作时间（" + hours + "），到点自动恢复");
                        RaiseCurrentTask(null);
                        RaiseTaskList(new List<VoiceBroadcastTask>());
                        _logger.Info("当前不在播报时段 " + hours + "，等待 " + wait.TotalMinutes.ToString("0") + " 分钟后恢复。");
                        await Task.Delay(wait, cancellationToken);
                        continue;
                    }

                    await EnsureLoggedInAsync(cancellationToken);
                    List<VoiceBroadcastTask> tasks = await _apiClient.GetPendingTasksAsync(_resolvedWorkshopIds, cancellationToken);
                    // 每小时重试导写 6：在这里确认“恢复成功”，空任务列表也算接口正常。
                    // 1. 使用上面真实的登录、任务查询结果判断恢复，不新增 ping 探测；Token 失效沿用重登流程。
                    // 2. 查询成功后清除故障等待、下次重试时间和失败次数，并更新状态与恢复日志。
                    // 3. 更新前再次核对暂停原因：期间若用户手动暂停，则继续保持暂停，禁止进入实际播放。
                    // 4. EnsureLoggedInAsync 内也会报告“运行中”，需与此处协调，避免尚未查询成功就显示已恢复。
                    _consecutiveFailureCount = 0;
                    // 时间格式固定为 yyyy-MM-dd HH:mm:ss，可按字符串实现 FIFO。
                    tasks = tasks.Where(x => x != null && x.IsPending)
                        .OrderBy(x => string.IsNullOrWhiteSpace(x.CreateTime) ? "9999-12-31 23:59:59" : x.CreateTime)
                        .ThenBy(x => x.Id)
                        .ToList();
                    RaiseTaskList(tasks);
                    _logger.Info("查询到待播报任务 " + tasks.Count + " 条。");

                    foreach (VoiceBroadcastTask task in tasks)
                    {
                        if (cancellationToken.IsCancellationRequested || _paused || !_settings.IsWithinWorkHours(DateTime.Now))
                        {
                            break;
                        }
                        await ProcessTaskAsync(task, cancellationToken);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(_settings.PollIntervalSeconds), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (MesUnauthorizedException exception)
                {
                    _apiClient.ClearToken();
                    _workshopsResolved = false;
                    retryDelaySeconds = RegisterFailure("MES 登录已失效", exception);
                }
                catch (Exception exception)
                {
                    retryDelaySeconds = RegisterFailure("MES 连接或接口异常", exception);
                }

                if (retryDelaySeconds > 0)
                {
                    // 每小时重试导写 8：这里仅保留原来的短间隔重试；一小时等待由第 5 步检查到期时间。
                    // 1. 不在这里直接 Task.Delay 一小时，否则点击继续后仍可能要等到延时结束才发请求。
                    // 2. 维持取消令牌处理，确保等待期间退出或重载设置仍可及时结束后台任务。
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds), cancellationToken);
                }
            }
            _logger.Info("播报协调器已停止。");
        }

        private int RegisterFailure(string summary, Exception exception)
        {
            // 每小时重试导写 7：在此安排故障等待，并区分首次达到上限与每小时恢复再次失败。
            // 1. 首次连续失败达到 MaxRequestRetryCount（默认 3）后，标记故障等待，安排从此刻起一小时后重试。
            // 2. 已处于故障等待的恢复尝试若失败，直接将下次时间推迟一小时，不重启三次短间隔重试。
            // 3. 安排好时间后返回 0，由主循环管理等待；更新下方状态和日志，显示实际错误与下次重试时间。
            // 4. 若用户已手动暂停，失败回调只记录错误，不得把它改为可自动恢复的故障等待。
            // 5. 改完验证：三次失败、每小时单次失败后重排、成功恢复、空列表成功、手动暂停与继续、工作时段与退出。
            _consecutiveFailureCount++;
            int maximum = Math.Max(1, _settings.MaxRequestRetryCount);
            _logger.Error(summary + "，连续失败 " + _consecutiveFailureCount + "/" + maximum + "。", exception);

            if (_consecutiveFailureCount >= maximum)
            {
                // 达到失败上限后等待人工恢复，避免持续请求异常服务。
                _paused = true;
                RaiseStatus("连续失败已达上限，已暂停；请检查后点击继续播报");
                _logger.Warn("已达到最大重试次数，停止自动请求 MES。修复配置或服务后，请点击“继续播报”。");
                return 0;
            }

            RaiseStatus(summary + "，等待第 " + (_consecutiveFailureCount + 1) + " 次尝试");
            return _settings.NetworkRetrySeconds;
        }

        private async Task ProcessTaskAsync(VoiceBroadcastTask task, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(task.Content))
            {
                _logger.Warn("任务 " + task.Id + " 的播报内容为空，保留待播报状态。\r\n");
                return;
            }

            while (task.IsPending && !_paused)
            {
                if (cancellationToken.IsCancellationRequested || !_settings.IsWithinWorkHours(DateTime.Now))
                {
                    return;
                }
                int expectedBroadcastedNum = task.BroadcastedNum;
                RaiseCurrentTask(task);
                RaiseStatus("正在播报任务 " + task.Id);
                _logger.Info("开始播报任务=" + task.Id + "，次数=" + (expectedBroadcastedNum + 1) + "/" + task.VoiceNum);

                try
                {
                    // 提示音和语音都完成后，才向 MES 确认本次次数。
                    if (_soundService.Exists)
                    {
                        bool soundCompleted = await _soundService.PlayAsync(cancellationToken);
                        if (!soundCompleted || _paused || cancellationToken.IsCancellationRequested)
                        {
                            _logger.Info("任务 " + task.Id + " 在提示音阶段被取消，未确认次数。");
                            return;
                        }
                    }
                    else
                    {
                        _logger.Warn("提示音文件不存在，跳过提示音：" + Path.GetFileName("AlarmMusic.wav"));
                    }

                    bool speechCompleted = await _speechService.SpeakAsync(task.Content, _settings.TtsRate, _settings.TtsVolume, cancellationToken);
                    if (!speechCompleted || _paused || cancellationToken.IsCancellationRequested)
                    {
                        _logger.Info("任务 " + task.Id + " 在语音阶段被取消，未确认次数。");
                        return;
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.Info("任务 " + task.Id + " 的当前播报已取消，未确认次数。");
                    return;
                }
                catch (Exception exception)
                {
                    _logger.Error("任务 " + task.Id + " 播放失败，未确认次数。", exception);
                    return;
                }

                bool confirmed = await ConfirmPlayedWithRecoveryAsync(task.Id, expectedBroadcastedNum, task.VoiceNum, cancellationToken);
                if (!confirmed)
                {
                    _logger.Warn("任务 " + task.Id + " 的确认被拒绝或状态已变化，将重新查询任务。\r\n");
                    return;
                }

                task.BroadcastedNum = expectedBroadcastedNum + 1;
                task.State = task.BroadcastedNum >= task.VoiceNum ? "1" : "0";
                _logger.Info("任务 " + task.Id + " 第 " + task.BroadcastedNum + " 次播报确认成功。");
                RaiseCurrentTask(task);

                if (task.IsPending && _settings.BroadcastIntervalSeconds > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_settings.BroadcastIntervalSeconds), cancellationToken);
                }
            }
        }

        private async Task<bool> ConfirmPlayedWithRecoveryAsync(long taskId, int expectedBroadcastedNum, int voiceNum, CancellationToken cancellationToken)
        {
            // 确认结果不明确时查询实际状态，避免直接重复播放。
            bool reloginRequired = false;
            try
            {
                return await _apiClient.ConfirmPlayedAsync(taskId, expectedBroadcastedNum, voiceNum, cancellationToken);
            }
            catch (MesUnauthorizedException)
            {
                _apiClient.ClearToken();
                _workshopsResolved = false;
                reloginRequired = true;
            }
            catch (MesApiException exception)
            {
                _logger.Warn("确认任务 " + taskId + " 时发生异常，先查询任务实际状态：" + exception.Message);
            }

            if (reloginRequired)
            {
                await EnsureLoggedInAsync(cancellationToken);
            }

            VoiceBroadcastTask actual = await _apiClient.GetTaskAsync(taskId, cancellationToken);
            if (actual == null)
            {
                return false;
            }
            if (actual.BroadcastedNum > expectedBroadcastedNum)
            {
                // 后端次数已增加，说明前一次确认实际上已经成功。
                return true;
            }
            if (!actual.IsPending || actual.BroadcastedNum != expectedBroadcastedNum)
            {
                return false;
            }
            return await _apiClient.ConfirmPlayedAsync(taskId, expectedBroadcastedNum, voiceNum, cancellationToken);
        }

        private async Task EnsureLoggedInAsync(CancellationToken cancellationToken)
        {
            // 每小时重试导写 6（配套）：登录成功与连接完全恢复是两件事。
            // 1. 保留登录、车间解析职责；故障恢复期间不要在任务查询之前提前将界面改为“运行中”。
            // 2. 最终恢复状态与日志统一由主循环在任务查询成功后处理，且必须尊重期间的手动暂停。
            if (_apiClient.HasToken && _workshopsResolved)
            {
                return;
            }
            if (!_credentials.IsComplete())
            {
                throw new MesApiException("未配置 MES 登录账号或密码。");
            }
            if (!_apiClient.HasToken)
            {
                await _apiClient.LoginAsync(_credentials, cancellationToken);
            }
            await ResolveWorkshopsAsync(cancellationToken);
            RaiseStatus("运行中");
            _logger.Info("MES 登录成功。");
        }

        private async Task ResolveWorkshopsAsync(CancellationToken cancellationToken)
        {
            if ((_settings.WorkshopNames == null || _settings.WorkshopNames.Count == 0)
                && _settings.WorkshopIdList != null
                && _settings.WorkshopIdList.Count > 0)
            {
                _resolvedWorkshopIds = _settings.WorkshopIdList.Distinct().ToList();
                _workshopsResolved = true;
                return;
            }

            List<WorkshopOption> options = await _apiClient.GetWorkshopsAsync(cancellationToken);
            if (_settings.FactoryId.HasValue && options.Any(x => x != null && x.TenantId.HasValue))
            {
                options = options.Where(x => x != null && x.TenantId == _settings.FactoryId).ToList();
                if (options.Count == 0)
                {
                    throw new MesApiException("当前账号未查询到所选工厂（ID " + _settings.FactoryId.Value + "）下的可用车间。");
                }
                _logger.Info("已按工厂过滤车间：" + (_settings.FactoryName ?? string.Empty) + "（ID " + _settings.FactoryId.Value + "）。");
            }
            if (_settings.WorkshopNames == null || _settings.WorkshopNames.Count == 0)
            {
                // 参考 MES 以 0 表示同时包含未绑定产线的公共任务；其余 ID 覆盖全部车间产线。
                _resolvedWorkshopIds = new[] { 0L }
                    .Concat(options.Where(x => x != null).Select(x => x.WorkshopId).Where(x => x > 0))
                    .Distinct()
                    .ToList();
                _workshopsResolved = true;
                _logger.Info("未限制播报车间，已加载全部车间 ID：" + string.Join(",", _resolvedWorkshopIds));
                return;
            }
            var byName = options
                .Where(x => x != null && x.WorkshopId >= 0 && !string.IsNullOrWhiteSpace(x.WorkshopName))
                .GroupBy(x => x.WorkshopName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.First().WorkshopId, StringComparer.OrdinalIgnoreCase);
            var matched = new List<long>();
            var missing = new List<string>();
            foreach (string name in _settings.WorkshopNames)
            {
                long workshopId;
                if (byName.TryGetValue(name.Trim(), out workshopId))
                {
                    matched.Add(workshopId);
                }
                else
                {
                    missing.Add(name);
                }
            }
            if (missing.Count > 0)
            {
                throw new MesApiException("未匹配到车间：" + string.Join("、", missing) + "。请确认名称、启用状态及账号权限。");
            }
            _resolvedWorkshopIds = matched
                .Concat(_settings.WorkshopIdList ?? new List<long>())
                .Distinct()
                .ToList();
            _workshopsResolved = true;
            _logger.Info("车间 ID/名称已解析为 ID：" + string.Join(",", _resolvedWorkshopIds));
        }

        private void RaiseStatus(string value)
        {
            var handler = StatusChanged;
            if (handler != null)
            {
                handler(value);
            }
        }

        private void RaiseCurrentTask(VoiceBroadcastTask task)
        {
            var handler = CurrentTaskChanged;
            if (handler != null)
            {
                handler(task);
            }
        }

        private void RaiseTaskList(List<VoiceBroadcastTask> tasks)
        {
            var handler = TaskListChanged;
            if (handler != null)
            {
                handler(tasks);
            }
        }

        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }
    }
}

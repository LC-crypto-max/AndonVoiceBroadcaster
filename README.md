# AndonVoiceBroadcaster

AndonVoiceBroadcaster 是运行在车间固定 Windows 电脑上的 MES 安灯语音播报客户端，目标框架为 **.NET Framework 4.8**。程序从 MES 查询待播报任务，通过电脑连接的喇叭播放提示音和中文语音，并回写播报次数及完成状态。

当前版本面向“一台车间 Windows 电脑控制一套喇叭”的使用场景，已具备完整演示所需的工厂、车间和工作时间配置能力。

## 当前实现情况（2026-07-18）

| 功能 | 状态 | 说明 |
|------|------|------|
| MES 登录与任务轮询 | 已实现 | 使用专用账号登录并轮询待播报任务 |
| 工厂选择 | 已实现 | 以 MES `tenantId` 作为工厂标识，显示当前登录工厂 |
| 车间选择 | 已实现 | 按工厂筛选车间，通过双列表添加、移除、全选和清空 |
| 工作时间 | 已实现 | 默认每天 08:00-17:00 查询和播报，可在设置中修改 |
| 中文语音播报 | 已实现 | 使用 Windows 中文 TTS，可配置语速、音量和测试文本 |
| 提示音 | 已实现 | 播报前播放 `audio/AlarmMusic.wav` |
| MES 状态回写 | 已实现 | 更新播报次数，达到次数后回写完成状态 |
| 暂停与继续 | 已实现 | 主界面和托盘菜单均可控制 |
| 异常重试 | 已实现 | 连续失败达到上限后暂停，排障后可继续 |
| 配置持久化 | 已实现 | 普通设置保存为 JSON，密码使用当前 Windows 用户 DPAPI 加密 |
| 开机启动与托盘运行 | 已实现 | 可配置登录后启动及启动后最小化 |
| 日志与安全退出 | 已实现 | 写入本地日志，退出时释放后台任务和音频资源 |
| 本地声音测试 | 已实现 | 无需 MES 连接即可测试提示音和中文语音 |

## 快速体验

1. 使用 Visual Studio 2022 打开 `AndonVoiceBroadcaster.csproj`，或执行：

   ```powershell
   dotnet msbuild .\AndonVoiceBroadcaster.csproj -t:Rebuild -p:Configuration=Debug -p:Platform=AnyCPU
   ```

2. 运行 `bin\Debug\AndonVoiceBroadcaster.exe`。
3. 点击“测试提示音”检查喇叭，点击“测试中文语音”检查 Windows 中文语音。
4. 打开“系统设置”，填写 MES 信息后刷新工厂和车间数据。

MES 地址或账号尚未配置时，本地声音测试仍可使用。

## 工厂、车间和工作时间配置

在“系统设置”中按以下顺序配置：

1. 填写 MES 服务地址、登录账号、密码和终端编号。
2. 点击“刷新列表”，程序登录 MES 并读取当前工厂和车间。
3. 在“工厂”下拉框中确认工厂。当前 MES 接口只返回登录账号所属租户时，下拉框显示该工厂；车间数据包含多个 `tenantId` 时，可按工厂切换筛选。
4. 从左侧“可用车间”选择车间，使用中间按钮添加到右侧“已选车间”；也可双击列表项快速移动。
5. 设置上班开始时间和下班停止时间，默认分别为 08:00 和 17:00。
6. 保存设置并回到主界面，确认状态栏显示的工厂、车间和工作时间正确。

工作时间采用本机时间，开始时刻包含、结束时刻不包含，即默认有效区间为 `[08:00, 17:00)`。非工作时间程序保持运行但不发起任务轮询；本地提示音和语音测试不受此限制。

## 设置项

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| MES 服务地址 | MES 系统访问地址 | 空 |
| 登录账号、密码 | MES 专用账号凭据 | 空 |
| 终端编号 | 当前电脑的业务标识 | 空 |
| 工厂 | MES 租户/工厂范围 | 当前登录工厂 |
| 车间 | 双列表选择要播报的车间 | 未选择时为全部可用车间 |
| 上班开始时间 | 开始轮询和播报的本地时间 | 08:00 |
| 下班停止时间 | 停止新任务轮询和播报的本地时间 | 17:00 |
| 查询间隔 | MES 任务轮询间隔 | 2 秒 |
| 网络重试 | 请求失败后的重试间隔 | 5 秒 |
| 最大连续失败次数 | 达到后自动暂停 | 3 次 |
| TTS 语速 | Windows 中文语音语速 | 0 |
| TTS 音量 | Windows 中文语音音量 | 100 |
| 播报间隔 | 同一任务多次播报之间的间隔 | 1 秒 |
| 开机启动 | 当前 Windows 用户登录后启动 | 否 |
| 启动后最小化 | 启动后进入系统托盘 | 否 |

## MES 接口与运行约束

- 默认登录地址为 `/loginWithNoCode`，可通过 `config/appsettings.json` 的 `LoginEndpoint` 调整。
- 工厂标识来自登录租户及车间数据中的 `tenantId`；车间显示格式为 `车间ID - 车间名称`。
- 未达到播报次数时调用 `POST /system/voiceBroadcast/addBroadcastedNum`；达到次数后调用 `POST /system/voiceBroadcast/broadcastCompleted`。
- 登录账号需具有登录、租户/车间查询、播报任务查询和结果回写权限。
- 同一批任务应只由一台播报终端消费，避免与 PDA 播报页面重复播放。
- 正式账号应遵循最小权限原则，不要在源码、README 或部署说明中记录真实密码。

## 构建部署包

开发机需安装 Visual Studio 2022 Build Tools（含 .NET Framework 4.8 开发工具包）或等效 MSBuild 环境。执行：

```powershell
powershell -ExecutionPolicy Bypass -File .\build-deploy.ps1
```

脚本会执行 Release 构建、校验交付文件，并在项目根目录生成 `AndonVoiceBroadcaster_Deploy.zip`。操作员解压后先运行 `安装检查.bat`，全部关键项通过后再运行 `启动程序.bat`。

目标电脑建议使用 Windows 10/11，并需要：

- .NET Framework 4.8 或更高版本；
- Windows 中文语音组件；
- 可用的扬声器/功放及正确的默认音频输出设备；
- 到 MES 服务地址的网络访问能力；
- 对程序目录的读写权限（用于保存配置、加密凭据和日志）。

详细操作见 `deploy/部署说明.txt`。

## 项目结构

```text
AndonVoiceBroadcaster/
├── Forms/                         # 主界面和设置界面
├── Infrastructure/                # 日志、DPAPI、单实例保护
├── Models/
│   ├── AppSettings.cs             # 应用设置及工作时间逻辑
│   ├── FactoryOption.cs           # 工厂选项
│   ├── WorkshopOption.cs          # 车间选项
│   └── VoiceBroadcastTask.cs      # 播报任务
├── Services/                      # MES、播报、语音、提示音和启动服务
├── audio/AlarmMusic.wav           # 默认提示音
├── config/appsettings.json        # 默认配置
├── deploy/                        # 操作员部署材料和脚本
├── docs/                          # 内部定制设计和复盘文档（Git 忽略）
├── build-deploy.ps1               # Release 构建及打包脚本
└── AndonVoiceBroadcaster.csproj
```

## 内部文档

`docs/` 包含定制业务设计、数据流、实施计划和工作日复盘，已按项目保密要求加入 `.gitignore`，不会随 GitHub 仓库提交。当前本地文档包括：

- `docs/技术设计文档7.18实现情况.md`
- `docs/第4工作日问题与解决方案.md`
- `docs/数据流.md`
- `docs/技术设计文档7.17.md`
- `docs/未实现功能与实施计划.md`

## 退出与排障

关闭主窗口只会最小化到系统托盘。需要完全退出时，请右键托盘图标并选择“退出程序”。发生连接或播报异常时，先查看程序状态和 `logs/` 下的日志，再按 `deploy/部署说明.txt` 的检查顺序处理。

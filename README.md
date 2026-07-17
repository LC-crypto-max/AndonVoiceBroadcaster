# AndonVoiceBroadcaster

这是 MES 安灯语音播报的 WinForms 客户端，目标框架为 .NET Framework 4.8。客户端适合车间固定电脑常驻运行：从现有 `sys_voice_broadcast` 查询任务，播放提示音和 Windows 中文语音，并通过现有 MES 接口更新播报次数和完成状态。

## 功能特性

| 功能 | 状态 | 说明 |
|------|------|------|
| 音量调节 | ✅ | 通过 TTS 音量设置项实现，范围 0-100 |
| 语速调节 | ✅ | 通过 TTS 语速设置项实现，范围 -10 到 10 |
| 播放暂停 | ✅ | 通过托盘菜单和主界面按钮实现 |
| 数据同步 | ✅ | 播放完成后回写 MES 系统 |
| 车间筛选 | ✅ | 通过双列表选择器选择车间 |
| 日志记录 | ✅ | 配置持久化，下次启动自动加载 |
| 开机启动 | ✅ | 当前用户登录后自动启动 |
| 安全退出 | ✅ | 后台进程自动关闭，不产生残留 |
| 无感知启动 | ✅ | 启动后最小化到系统托盘 |
| 异常重试 | ✅ | 连续失败达到上限后暂停，可手动恢复 |
| 本地测试 | ✅ | 不依赖 MES 配置，可直接测试语音 |

## 第一天本地语音验证

当前阶段不需要安装 MySQL，也不需要先配置 MES。直接启动程序，点击"测试中文语音"，程序会朗读配置文件中的 `TestSpeechText`；默认内容为"这是一条安灯语音播报测试，语音功能正常。"。可在"系统设置"的"本地测试文本"中修改内容。点击"测试提示音"可单独验证 `audio/AlarmMusic.wav`。

本地语音与 MES 轮询已经解耦：MES 地址、账号密码为空时，界面显示"本地播报测试就绪（MES 未配置）"，仍可测试声音。开发机默认不开机启动、不自动最小化，便于第一天人工验收。

2026-07-15 已在当前 Windows 用户会话使用 `Microsoft Huihui Desktop (zh-CN)` 完成实际中文朗读调用；Debug 版本也已通过 MSBuild 重新构建。

## MES 联调配置

需要联调时，在"系统设置"中填写 MES 地址和账号密码。

### 车间选择

车间选择已从手动输入改为可视化双列表选择器：

1. 填写完整的 MES 服务地址、账号和密码
2. 点击"刷新车间列表"按钮加载车间数据
3. 在左侧"可用车间"列表中选择车间，点击"添加 ->"移到右侧"已选车间"列表
4. 使用"全部添加"快速添加所有车间，使用"全部移除"清空已选列表
5. 在右侧"已选车间"列表中选择车间，点击"<- 移除"移回左侧
6. 未选择任何车间时，默认播报全部车间

车间显示格式为 `车间ID - 车间名称`，便于用户识别。

### 登录配置

默认登录地址为 `/loginWithNoCode`，可在 `config/appsettings.json` 的 `LoginEndpoint` 修改为 `/login`。无验证码登录只建议用于受控的本地测试，并应使用最小权限的专用账号；不要在源码或文档中保存管理员密码。

账号密码保存在当前 Windows 用户 DPAPI 加密文件 `data/credentials.dat`，不写入明文配置。`audio/AlarmMusic.wav` 与生成的 EXE 一起部署。

### 异常重试

MES 登录或接口请求连续失败达到 `MaxRequestRetryCount`（默认 3 次）后，客户端会停止自动请求并进入暂停状态；排除故障后点击"继续播报"会清零失败计数并重新连接。

## 设置界面说明

设置界面包含以下配置项：

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| MES 服务地址 | MES 系统的访问地址 | - |
| 登录账号 | MES 登录账号 | - |
| 登录密码 | MES 登录密码 | - |
| 终端编号 | 当前终端的唯一标识 | - |
| 选择车间 | 通过双列表选择器选择要播报的车间 | 全部车间 |
| 查询间隔（秒） | MES 任务轮询间隔 | 2 |
| 网络重试（秒） | 请求失败后的重试间隔 | 5 |
| TTS 语速 | 中文语音的语速 | 0 |
| TTS 音量 | 中文语音的音量 | 100 |
| 本地测试文本 | 用于测试中文语音的文本 | 这是一条安灯语音播报测试，语音功能正常。 |
| 播报间隔（秒） | 同一任务多次播报之间的间隔 | 1 |
| 当前 Windows 用户登录后自动启动 | 开机后自动启动程序 | 否 |
| 启动后最小化到系统托盘 | 启动后自动最小化 | 否 |

## 构建与运行

使用 Visual Studio 2022 打开 `AndonVoiceBroadcaster.csproj`，选择 .NET Framework 4.8、Debug/Release 和 Any CPU 构建；也可执行 `dotnet msbuild AndonVoiceBroadcaster.csproj -t:Rebuild -p:Configuration=Debug`。启动后关闭主窗口会最小化到托盘；通过托盘"退出程序"并确认后才会停止播报。安全退出会取消当前任务，最多等待协调器 5 秒，随后释放 HTTP、TTS、提示音和托盘资源并结束消息循环。

## 后端前置条件

- 客户端沿用现有 PDA 接口：未达到次数时调用 `POST /system/voiceBroadcast/addBroadcastedNum`，达到次数时调用 `POST /system/voiceBroadcast/broadcastCompleted`。
- 同一批任务只应由一台车间播报终端消费，避免 PDA 页面与 WinForms 客户端重复播放。
- 登录账号需要车间列表查询权限，以及语音播报查询/确认权限。
- 正式车间电脑不建议与 PDA `voicebroadcast` 页面同时消费同一批任务。

## 项目结构

```
AndonVoiceBroadcaster/
├── Forms/                    # WinForms 窗体
│   ├── MainForm.cs           # 主窗体
│   └── SettingsForm.cs       # 设置窗体
├── Infrastructure/           # 基础设施
│   ├── DpapiSecretStore.cs   # DPAPI 加密存储
│   ├── FileLogger.cs         # 文件日志
│   └── SingleInstanceGuard.cs # 单实例保护
├── Models/                   # 数据模型
│   ├── AppSettings.cs        # 应用设置
│   ├── MesCredentials.cs     # MES 凭据
│   ├── VoiceBroadcastTask.cs # 语音播报任务
│   └── WorkshopOption.cs     # 车间选项
├── Services/                 # 业务服务
│   ├── BroadcastCoordinator.cs # 播报协调器
│   ├── MesApiClient.cs       # MES API 客户端
│   ├── SettingsService.cs    # 设置服务
│   ├── SoundService.cs       # 提示音服务
│   ├── SpeechService.cs      # TTS 语音服务
│   └── StartupService.cs     # 启动服务
├── audio/                    # 音频文件
│   └── AlarmMusic.wav        # 提示音
├── config/                   # 配置文件
│   └── appsettings.json      # 默认配置
├── docs/                     # 文档
│   ├── 技术设计文档7.17.md
│   ├── 当前设计方案与核心理念.md
│   ├── 第2工作日问题与解决方案.md
│   ├── 第3工作日问题与解决方案.md
│   ├── 技术设计文档达成内容确认.md
│   └── 2026-07-16暂停换车间与退出问题复盘.md
├── AndonVoiceBroadcaster.csproj
├── Program.cs
└── README.md
```

## 问题与解决方案

项目开发过程中的问题与解决方案记录在 docs 文件夹中：

- [第2工作日问题与解决方案](docs/第2工作日问题与解决方案.md)
- [第3工作日问题与解决方案](docs/第3工作日问题与解决方案.md)
- [2026-07-16暂停换车间与退出问题复盘](docs/2026-07-16暂停换车间与退出问题复盘.md)

## 技术设计文档

- [技术设计文档](docs/技术设计文档7.17.md)
- [当前设计方案与核心理念](docs/当前设计方案与核心理念.md)
- [技术设计文档达成内容确认](docs/技术设计文档达成内容确认.md)

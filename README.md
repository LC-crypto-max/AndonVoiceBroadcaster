# AndonVoiceBroadcaster

这是 MES 安灯语音播报的 WinForms 客户端，目标框架为 .NET Framework 4.8。客户端适合车间固定电脑常驻运行：从现有 `sys_voice_broadcast` 查询任务，播放提示音和 Windows 中文语音，并通过原子确认接口更新播报次数。

## 第一天本地语音验证

当前阶段不需要安装 MySQL，也不需要先配置 MES。直接启动程序，点击“测试中文语音”，程序会朗读配置文件中的 `TestSpeechText`；默认内容为“这是一条安灯语音播报测试，语音功能正常。”。可在“系统设置”的“本地测试文本”中修改内容。点击“测试提示音”可单独验证 `audio/AlarmMusic.wav`。

本地语音与 MES 轮询已经解耦：MES 地址、账号密码为空时，界面显示“本地播报测试就绪（MES 未配置）”，仍可测试声音。开发机默认不开机启动、不自动最小化，便于第一天人工验收。

2026-07-15 已在当前 Windows 用户会话使用 `Microsoft Huihui Desktop (zh-CN)` 完成实际中文朗读调用；Debug 版本也已通过 MSBuild 重新构建。

## MES 联调配置

需要联调时，在“系统设置”中填写 MES 地址和账号密码。车间名称是可选系统设置：填写后，程序调用 `/mes/md/workshop/listAll`，将中文名称精确匹配为内部车间 ID 并限定查询范围；留空时不附加车间限制。旧版 `WorkshopIdList` 仍作为兼容回退。

默认登录地址为 `/loginWithNoCode`，可在 `config/appsettings.json` 的 `LoginEndpoint` 修改为 `/login`。无验证码登录只建议用于受控的本地测试，并应使用最小权限的专用账号；不要在源码或文档中保存管理员密码。

账号密码保存在当前 Windows 用户 DPAPI 加密文件 `data/credentials.dat`，不写入明文配置。`audio/AlarmMusic.wav` 与生成的 EXE 一起部署。

## 构建与运行

使用 Visual Studio 2022 打开 `AndonVoiceBroadcaster.csproj`，选择 .NET Framework 4.8、Debug/Release 和 Any CPU 构建；也可执行 `dotnet msbuild AndonVoiceBroadcaster.csproj -t:Rebuild -p:Configuration=Debug`。启动后关闭主窗口会最小化到托盘；通过托盘“退出程序”并确认后才会停止播报。

## 后端前置条件

- Java 侧已提供 `POST /system/voiceBroadcast/confirmPlayed` 原子确认接口。
- 登录账号需要车间列表查询权限，以及语音播报查询/确认权限。
- 正式车间电脑不建议与 PDA `voicebroadcast` 页面同时消费同一批任务。

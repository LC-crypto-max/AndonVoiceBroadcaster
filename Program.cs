using System;
using System.Windows.Forms;
using AndonVoiceBroadcaster.Infrastructure;

namespace AndonVoiceBroadcaster
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // 使用全局互斥量，防止程序重复启动。
            using (var instance = new SingleInstanceGuard("AndonVoiceBroadcaster"))
            {
                if (!instance.IsFirstInstance)
                {
                    MessageBox.Show("安灯语音播报程序已经在运行。", "安灯语音播报", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                // 初始化 WinForms 并进入主界面消息循环。
                Application.Run(new Forms.Form1());
            }
        }
    }
}

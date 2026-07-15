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
            using (var instance = new SingleInstanceGuard("AndonVoiceBroadcaster"))
            {
                if (!instance.IsFirstInstance)
                {
                    MessageBox.Show("安灯语音播报程序已经在运行。", "安灯语音播报", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Forms.MainForm());
            }
        }
    }
}

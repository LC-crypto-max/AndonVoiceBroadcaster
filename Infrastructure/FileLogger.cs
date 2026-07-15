using System;
using System.Globalization;
using System.IO;

namespace AndonVoiceBroadcaster.Infrastructure
{
    public sealed class FileLogger
    {
        private readonly object _syncRoot = new object();
        private readonly string _logDirectory;
        private readonly int _retentionDays;

        public FileLogger(string logDirectory, int retentionDays)
        {
            _logDirectory = logDirectory;
            _retentionDays = retentionDays;
            Directory.CreateDirectory(_logDirectory);
            RemoveExpiredFiles();
        }

        public event Action<string> Written;

        public void Info(string message)
        {
            Write("INFO", message, null);
        }

        public void Warn(string message)
        {
            Write("WARN", message, null);
        }

        public void Error(string message, Exception exception)
        {
            Write("ERROR", message, exception);
        }

        private void Write(string level, string message, Exception exception)
        {
            string line = string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm:ss.fff} {1} {2}", DateTime.Now, level, message);
            if (exception != null)
            {
                line += Environment.NewLine + exception;
            }

            lock (_syncRoot)
            {
                string path = Path.Combine(_logDirectory, string.Format("app-{0:yyyy-MM-dd}.log", DateTime.Now));
                File.AppendAllText(path, line + Environment.NewLine, System.Text.Encoding.UTF8);
            }

            var handler = Written;
            if (handler != null)
            {
                handler(line);
            }
        }

        private void RemoveExpiredFiles()
        {
            DateTime deadline = DateTime.Today.AddDays(-_retentionDays);
            foreach (string path in Directory.GetFiles(_logDirectory, "app-*.log"))
            {
                if (File.GetLastWriteTime(path) < deadline)
                {
                    File.Delete(path);
                }
            }
        }
    }
}

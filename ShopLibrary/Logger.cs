using System;
using System.IO;

namespace ShopLibrary
{
    // Одинак для запису всіх дій користувача у файл
    public sealed class Logger
    {
        private static Logger instance;
        private static readonly object locker = new object();

        public static int CreatedCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string LogPath { get; private set; }

        private Logger()
        {
            CreatedCount++;
            CreatedAt = DateTime.Now;
            string folder = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(folder);
            LogPath = Path.Combine(folder, "log.txt");
        }

        public static Logger Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                        instance = new Logger();
                    return instance;
                }
            }
        }

        public void Log(string message)
        {
            lock (locker)
            {
                File.AppendAllText(LogPath, "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + message + Environment.NewLine);
            }
        }

        public string ReadAll()
        {
            lock (locker)
            {
                return File.Exists(LogPath) ? File.ReadAllText(LogPath) : "";
            }
        }
    }
}

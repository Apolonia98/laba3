using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShopLibrary
{
    // Одинак: глобальна точка доступу до сховища документів (чеки, звіти)
    public sealed class DocumentSaver
    {
        private static DocumentSaver instance;
        private static readonly object locker = new object();

        public static int CreatedCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Folder { get; private set; }

        private DocumentSaver()
        {
            CreatedCount++;
            CreatedAt = DateTime.Now;
            Folder = Path.Combine(AppContext.BaseDirectory, "documents");
            Directory.CreateDirectory(Folder);
        }

        public static DocumentSaver Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                        instance = new DocumentSaver();
                    return instance;
                }
            }
        }

        public string Save(string type, string content)
        {
            lock (locker)
            {
                string name = type + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".txt";
                File.WriteAllText(Path.Combine(Folder, name), content);
                Logger.Instance.Log("DocumentSaver: збережено документ " + name);
                return name;
            }
        }

        public List<string> GetDocuments()
        {
            lock (locker)
            {
                return Directory.GetFiles(Folder, "*.txt").Select(Path.GetFileName).OrderByDescending(n => n).ToList();
            }
        }

        public string Read(string name)
        {
            lock (locker)
            {
                string path = Path.Combine(Folder, Path.GetFileName(name));
                return File.Exists(path) ? File.ReadAllText(path) : "";
            }
        }
    }
}

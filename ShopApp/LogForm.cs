using System;
using System.Drawing;
using System.Windows.Forms;
using ShopLibrary;

namespace ShopApp
{
    public class LogForm : Form
    {
        private TextBox txtLog;

        public LogForm()
        {
            Text = "Журнал дій";
            Width = 820;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10);

            txtLog = new TextBox { Left = 15, Top = 15, Width = 775, Height = 345, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, WordWrap = false, Font = new Font("Consolas", 10) };
            Button btnRefresh = new Button { Text = "Оновити", Left = 15, Top = 370, Width = 120, Height = 34 };
            btnRefresh.Click += (s, e) => LoadLog();
            Label labelPath = new Label { Left = 150, Top = 377, Width = 640, Height = 24, ForeColor = Color.DimGray, Text = "Файл: " + Logger.Instance.LogPath };

            Controls.AddRange(new Control[] { txtLog, btnRefresh, labelPath });
            Controls.Add(SingletonInfo.CreateLabel());
            LoadLog();
        }

        private void LoadLog()
        {
            txtLog.Text = Logger.Instance.ReadAll();
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
    }
}

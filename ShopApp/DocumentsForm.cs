using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShopLibrary;

namespace ShopApp
{
    public class DocumentsForm : Form
    {
        private ListBox listDocuments;
        private TextBox txtContent;

        public DocumentsForm()
        {
            Text = "Документи";
            Width = 820;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10);

            listDocuments = new ListBox { Left = 15, Top = 15, Width = 300, Height = 340 };
            listDocuments.SelectedIndexChanged += listDocuments_SelectedIndexChanged;
            txtContent = new TextBox { Left = 330, Top = 15, Width = 460, Height = 340, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 10) };

            Button btnReport = new Button { Text = "Звіт про залишки", Left = 15, Top = 368, Width = 190, Height = 34 };
            Button btnRefresh = new Button { Text = "Оновити", Left = 215, Top = 368, Width = 100, Height = 34 };
            btnReport.Click += btnReport_Click;
            btnRefresh.Click += (s, e) => LoadDocuments();

            Controls.AddRange(new Control[] { listDocuments, txtContent, btnReport, btnRefresh });
            Controls.Add(SingletonInfo.CreateLabel());
            LoadDocuments();
        }

        private void LoadDocuments()
        {
            listDocuments.Items.Clear();
            foreach (string name in DocumentSaver.Instance.GetDocuments())
                listDocuments.Items.Add(name);
        }

        private void listDocuments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listDocuments.SelectedItem == null)
                return;
            string name = listDocuments.SelectedItem.ToString();
            txtContent.Text = DocumentSaver.Instance.Read(name);
            Logger.Instance.Log("Переглянуто документ " + name);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("Звіт про залишки товарів");
            report.AppendLine("Дата: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            report.AppendLine("--------------------------------");
            decimal sum = 0;
            foreach (Product p in DbManager.Instance.GetProducts())
            {
                report.AppendLine(p.Name + ": " + p.Quantity + " шт. по " + p.Price.ToString("0.00") + " грн");
                sum += p.Price * p.Quantity;
            }
            report.AppendLine("--------------------------------");
            report.AppendLine("Вартість залишків: " + sum.ToString("0.00") + " грн");

            string name = DocumentSaver.Instance.Save("Звіт", report.ToString());
            Logger.Instance.Log("Сформовано звіт про залишки " + name);
            LoadDocuments();
            listDocuments.SelectedItem = name;
        }
    }
}

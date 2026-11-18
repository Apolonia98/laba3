using System;
using System.Drawing;
using System.Windows.Forms;
using ShopLibrary;

namespace ShopApp
{
    public class MainForm : Form
    {
        private Label labelCheck;

        public MainForm()
        {
            Text = "Продуктовий магазин";
            Width = 640;
            Height = 420;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);

            Label title = new Label
            {
                Text = "Продуктовий магазин «Кошик»",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Left = 20, Top = 15, AutoSize = true
            };

            Button btnProducts = MakeButton("Товари", 20, 70);
            Button btnSale = MakeButton("Продаж", 320, 70);
            Button btnDocuments = MakeButton("Документи", 20, 140);
            Button btnLog = MakeButton("Журнал дій", 320, 140);
            Button btnCheck = MakeButton("Перевірити Singleton", 20, 210);
            btnCheck.Width = 580;

            btnProducts.Click += (s, e) => OpenForm(new ProductsForm(), "Товари");
            btnSale.Click += (s, e) => OpenForm(new SaleForm(), "Продаж");
            btnDocuments.Click += (s, e) => OpenForm(new DocumentsForm(), "Документи");
            btnLog.Click += (s, e) => OpenForm(new LogForm(), "Журнал дій");
            btnCheck.Click += btnCheck_Click;

            labelCheck = new Label { Left = 20, Top = 270, Width = 580, Height = 60, ForeColor = Color.DarkGreen };

            Controls.AddRange(new Control[] { title, btnProducts, btnSale, btnDocuments, btnLog, btnCheck, labelCheck });
            Controls.Add(SingletonInfo.CreateLabel());
        }

        private Button MakeButton(string text, int left, int top)
        {
            return new Button { Text = text, Left = left, Top = top, Width = 280, Height = 50 };
        }

        private void OpenForm(Form form, string name)
        {
            Logger.Instance.Log("Відкрито форму «" + name + "»");
            form.Show(this);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            bool same = ReferenceEquals(DbManager.Instance, DbManager.Instance)
                && ReferenceEquals(DocumentSaver.Instance, DocumentSaver.Instance)
                && ReferenceEquals(Logger.Instance, Logger.Instance);
            labelCheck.Text = (same ? "Усі звернення повертають той самий об’єкт. " : "Помилка: різні об’єкти! ")
                + "Створено екземплярів: DbManager " + DbManager.CreatedCount
                + ", DocumentSaver " + DocumentSaver.CreatedCount
                + ", Logger " + Logger.CreatedCount;
            Logger.Instance.Log("Перевірка Singleton: " + (same ? "один екземпляр" : "різні екземпляри"));
        }
    }
}

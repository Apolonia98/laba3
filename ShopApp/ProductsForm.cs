using System;
using System.Drawing;
using System.Windows.Forms;
using ShopLibrary;

namespace ShopApp
{
    public class ProductsForm : Form
    {
        private ListView listView1;
        private TextBox txtName;
        private TextBox txtCategory;
        private NumericUpDown numPrice;
        private NumericUpDown numQuantity;
        private Label labelStatus;

        public ProductsForm()
        {
            Text = "Товари";
            Width = 760;
            Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10);

            listView1 = new ListView { Left = 15, Top = 15, Width = 715, Height = 280, View = View.Details, FullRowSelect = true, GridLines = true };
            listView1.Columns.Add("№", 50);
            listView1.Columns.Add("Назва", 220);
            listView1.Columns.Add("Категорія", 170);
            listView1.Columns.Add("Ціна, грн", 110);
            listView1.Columns.Add("Залишок", 110);

            Label l1 = new Label { Text = "Назва:", Left = 15, Top = 312, AutoSize = true };
            txtName = new TextBox { Left = 100, Top = 308, Width = 200 };
            Label l2 = new Label { Text = "Категорія:", Left = 320, Top = 312, AutoSize = true };
            txtCategory = new TextBox { Left = 410, Top = 308, Width = 160 };
            Label l3 = new Label { Text = "Ціна:", Left = 15, Top = 350, AutoSize = true };
            numPrice = new NumericUpDown { Left = 100, Top = 346, Width = 100, Maximum = 100000, DecimalPlaces = 2 };
            Label l4 = new Label { Text = "Кількість:", Left = 320, Top = 350, AutoSize = true };
            numQuantity = new NumericUpDown { Left = 410, Top = 346, Width = 100, Maximum = 100000 };

            Button btnAdd = new Button { Text = "Додати товар", Left = 15, Top = 390, Width = 170, Height = 34 };
            Button btnDelete = new Button { Text = "Видалити вибраний", Left = 195, Top = 390, Width = 190, Height = 34 };
            Button btnRefresh = new Button { Text = "Оновити", Left = 395, Top = 390, Width = 120, Height = 34 };
            btnAdd.Click += btnAdd_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += (s, e) => LoadProducts();

            labelStatus = new Label { Left = 530, Top = 396, Width = 200, Height = 24 };

            Controls.AddRange(new Control[] { listView1, l1, txtName, l2, txtCategory, l3, numPrice, l4, numQuantity, btnAdd, btnDelete, btnRefresh, labelStatus });
            Controls.Add(SingletonInfo.CreateLabel());
            LoadProducts();
        }

        private void LoadProducts()
        {
            listView1.Items.Clear();
            foreach (Product p in DbManager.Instance.GetProducts())
            {
                ListViewItem item = new ListViewItem(p.Id.ToString());
                item.SubItems.Add(p.Name);
                item.SubItems.Add(p.Category);
                item.SubItems.Add(p.Price.ToString("0.00"));
                item.SubItems.Add(p.Quantity.ToString());
                listView1.Items.Add(item);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "" || numPrice.Value <= 0)
            {
                labelStatus.Text = "Введіть назву і ціну";
                return;
            }
            Product p = DbManager.Instance.AddProduct(txtName.Text.Trim(), txtCategory.Text.Trim(), numPrice.Value, (int)numQuantity.Value);
            Logger.Instance.Log("Додано товар «" + p.Name + "», ціна " + p.Price + ", кількість " + p.Quantity);
            labelStatus.Text = "Товар додано";
            txtName.Clear();
            txtCategory.Clear();
            LoadProducts();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                labelStatus.Text = "Виберіть товар";
                return;
            }
            int id = int.Parse(listView1.SelectedItems[0].Text);
            string name = listView1.SelectedItems[0].SubItems[1].Text;
            if (DbManager.Instance.DeleteProduct(id))
            {
                Logger.Instance.Log("Видалено товар «" + name + "»");
                labelStatus.Text = "Товар видалено";
            }
            LoadProducts();
        }
    }
}

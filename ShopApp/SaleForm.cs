using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShopLibrary;

namespace ShopApp
{
    public class SaleForm : Form
    {
        private ComboBox comboProducts;
        private NumericUpDown numQuantity;
        private ListBox listReceipt;
        private Label labelTotal;
        private Label labelStatus;

        private List<Product> cart = new List<Product>();
        private List<int> cartQuantity = new List<int>();

        public SaleForm()
        {
            Text = "Продаж";
            Width = 700;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10);

            Label l1 = new Label { Text = "Товар:", Left = 15, Top = 19, AutoSize = true };
            comboProducts = new ComboBox { Left = 80, Top = 15, Width = 330, DropDownStyle = ComboBoxStyle.DropDownList };
            Label l2 = new Label { Text = "К-сть:", Left = 425, Top = 19, AutoSize = true };
            numQuantity = new NumericUpDown { Left = 485, Top = 15, Width = 60, Minimum = 1, Maximum = 1000, Value = 1 };
            Button btnAdd = new Button { Text = "У чек", Left = 555, Top = 13, Width = 110, Height = 32 };
            btnAdd.Click += btnAdd_Click;

            listReceipt = new ListBox { Left = 15, Top = 60, Width = 650, Height = 250 };
            labelTotal = new Label { Left = 15, Top = 320, Width = 400, Height = 28, Font = new Font("Segoe UI", 11, FontStyle.Bold) };

            Button btnSell = new Button { Text = "Оформити продаж", Left = 15, Top = 355, Width = 200, Height = 36 };
            Button btnClear = new Button { Text = "Очистити чек", Left = 225, Top = 355, Width = 160, Height = 36 };
            btnSell.Click += btnSell_Click;
            btnClear.Click += (s, e) => { ClearCart(); labelStatus.Text = ""; };

            labelStatus = new Label { Left = 15, Top = 400, Width = 650, Height = 24 };

            Controls.AddRange(new Control[] { l1, comboProducts, l2, numQuantity, btnAdd, listReceipt, labelTotal, btnSell, btnClear, labelStatus });
            Controls.Add(SingletonInfo.CreateLabel());
            LoadProducts();
            ShowTotal();
        }

        private void LoadProducts()
        {
            comboProducts.Items.Clear();
            foreach (Product p in DbManager.Instance.GetProducts())
                comboProducts.Items.Add(p);
            if (comboProducts.Items.Count > 0)
                comboProducts.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Product p = comboProducts.SelectedItem as Product;
            if (p == null)
                return;
            int q = (int)numQuantity.Value;
            if (q > p.Quantity)
            {
                labelStatus.Text = "На складі лише " + p.Quantity + " шт.";
                return;
            }
            cart.Add(p);
            cartQuantity.Add(q);
            listReceipt.Items.Add(p.Name + " x " + q + " = " + (p.Price * q).ToString("0.00") + " грн");
            ShowTotal();
        }

        private decimal Total()
        {
            decimal sum = 0;
            for (int i = 0; i < cart.Count; i++)
                sum += cart[i].Price * cartQuantity[i];
            return sum;
        }

        private void ShowTotal()
        {
            labelTotal.Text = "Разом: " + Total().ToString("0.00") + " грн";
        }

        private void ClearCart()
        {
            cart.Clear();
            cartQuantity.Clear();
            listReceipt.Items.Clear();
            ShowTotal();
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                labelStatus.Text = "Чек порожній";
                return;
            }
            StringBuilder receipt = new StringBuilder();
            receipt.AppendLine("Продуктовий магазин «Кошик»");
            receipt.AppendLine("Чек від " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            receipt.AppendLine("--------------------------------");
            for (int i = 0; i < cart.Count; i++)
            {
                if (!DbManager.Instance.Sell(cart[i].Id, cartQuantity[i]))
                {
                    labelStatus.Text = "Недостатньо товару «" + cart[i].Name + "»";
                    Logger.Instance.Log("Помилка продажу: недостатньо товару «" + cart[i].Name + "»");
                    LoadProducts();
                    return;
                }
                receipt.AppendLine(cart[i].Name + " x " + cartQuantity[i] + " = " + (cart[i].Price * cartQuantity[i]).ToString("0.00") + " грн");
            }
            receipt.AppendLine("--------------------------------");
            receipt.AppendLine("РАЗОМ: " + Total().ToString("0.00") + " грн");

            string name = DocumentSaver.Instance.Save("Чек", receipt.ToString());
            Logger.Instance.Log("Продаж на суму " + Total().ToString("0.00") + " грн, чек " + name);
            labelStatus.Text = "Продаж оформлено, чек збережено: " + name;
            ClearCart();
            LoadProducts();
        }
    }
}

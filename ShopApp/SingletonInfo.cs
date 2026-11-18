using System.Drawing;
using System.Windows.Forms;
using ShopLibrary;

namespace ShopApp
{
    // Підпис внизу кожної форми: видно, що всі форми працюють з тими самими екземплярами
    internal static class SingletonInfo
    {
        public static string Text()
        {
            return "DbManager #" + DbManager.Instance.GetHashCode()
                + "   DocumentSaver #" + DocumentSaver.Instance.GetHashCode()
                + "   Logger #" + Logger.Instance.GetHashCode();
        }

        public static Label CreateLabel()
        {
            return new Label
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                Text = Text(),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };
        }
    }
}

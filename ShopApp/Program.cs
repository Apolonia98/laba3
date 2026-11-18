using System;
using System.Windows.Forms;
using ShopLibrary;

namespace ShopApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Logger.Instance.Log("Програму запущено");
            Application.Run(new MainForm());
            Logger.Instance.Log("Програму закрито");
        }
    }
}

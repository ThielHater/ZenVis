using System;
using System.Windows.Forms;

namespace ZenVis
{
    internal static class Program
    {
        static Program()
        {
        }

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
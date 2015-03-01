using System;
using System.Windows.Forms;
using Liquid;

namespace LiquidSim
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new Form1();
            form.Show();
            while (form.Running)
            {
                Application.DoEvents();
                form.Step();
            }
        }
    }
}

//EOF

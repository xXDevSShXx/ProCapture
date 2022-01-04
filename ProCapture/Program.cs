using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProCapture
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process[] inctanses = Process.GetProcessesByName("ProCapture");
            if(inctanses.Length > 1)
            {
                inctanses.Where(x => x.Id == Process.GetCurrentProcess().Id).FirstOrDefault().Kill();
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            App.Initialize(Application.StartupPath, "appSettings.json");

            Application.Run(new MainForm());
        }
    }
}

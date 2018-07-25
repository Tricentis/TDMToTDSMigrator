using System;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    static class MainMethod
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TDSMigrator());

        }
    }
}

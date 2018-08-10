using System;
using System.Threading;
using System.Windows.Forms;

namespace MigratorUI {
    internal static class Program {
        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static readonly Mutex Mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        private static void Main() {
            if (Mutex.WaitOne(TimeSpan.Zero, true)) {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TdsMigrator());
                Mutex.ReleaseMutex();
            } else {
                NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
            }

            #endregion
        }


    }
}
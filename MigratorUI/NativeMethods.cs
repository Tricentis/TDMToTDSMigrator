using System;
using System.Runtime.InteropServices;

namespace MigratorUI {
    internal class NativeMethods {
        #region Constants

        public const int HWND_BROADCAST = 0xffff;

        #endregion

        #region Static Fields

        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

        #endregion

        #region Public Methods and Operators

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        #endregion
    }
}
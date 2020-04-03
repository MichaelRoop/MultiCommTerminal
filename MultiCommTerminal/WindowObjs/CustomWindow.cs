using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MultiCommTerminal.WindowObjs {

    // https://www.codeproject.com/Tips/1155345/How-to-Remove-the-Close-Button-from-a-WPF-ToolWind
    // https://archive.codeplex.com/?p=wpfwindow

    public partial class CustomWindow : Window {

        #region Data

        // Prep stuff needed to remove close button on window
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        #endregion

        #region interop declarations

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion

        public CustomWindow() {
            Loaded += ToolWindow_Loaded;
        }

        void ToolWindow_Loaded(object sender, RoutedEventArgs e) {
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

    }
}

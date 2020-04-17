using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.WPF_Helpers {
    public static class WindowHelpers {

        /// <summary>
        /// Call in Window OnApplyTemplate for Windows using MyWindowStyle to bind drag and move
        /// </summary>
        /// <remarks>Call in the overriden OnApplyTemplate function</remarks>
        /// <param name="win">The window calling this function</param>
        public static void BindMouseDownToCustomTitleBar(this Window win) {
            Border b = win.Template.FindName("PART_topBar", win) as Border;
            //b.MouseDown += TitleBar_MouseDown;
            b.MouseDown += (sender, args) => {
                win.DragMove();
            };
        }


    }
}

using LanguageFactory.data;
using LogUtils.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WindowObjs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static MultiCommTerminal.WindowObjs.MsgBoxYesNo;

namespace MultiCommTerminal.WPF_Helpers {

    public static class WindowHelpers {

        static ClassLog LOG = new ClassLog("WindowHelpers");

        /// <summary>
        /// Call in Window OnApplyTemplate for Windows using MyWindowStyle to bind drag and move
        /// </summary>
        /// <remarks>Call in the overriden OnApplyTemplate function</remarks>
        /// <param name="win">The window calling this function</param>
        public static void BindMouseDownToCustomTitleBar(this Window win) {
            Border b = win.Template.FindName("PART_topBar", win) as Border;
            if (b != null) {
                b.MouseDown += (sender, args) => {
                    win.DragMove();
                };
            }
            else {
                LOG.Error(9999, "Could not find PART_topBar - are you sure you have style set to MyWindowStyle?");
            }
        }


        public static void HideTitleBarIcon(this Window win) {
            Border b = win.Template.FindName("PART_IconBorder", win) as Border;
            if (b != null) {
                b.Visibility = Visibility.Collapsed;
            }
            else {
                LOG.Error(9999, "Could not find PART_IconBorder - are you sure you have style set to MyWindowStyle?");
            }
        }



        public static void ShowMsg(string msg) {
            MsgBoxSimple.ShowBox(DI.Wrapper.GetText(MsgCode.Error), msg);

        }


        public static void ShowMsgTitle(string title, string msg) {
            MsgBoxSimple.ShowBox(title, msg);
        }


        /// <summary>Open a custom message box centered on caller</summary>
        /// <param name="win">The window opening the message box</param>
        /// <param name="msg">The message to display</param>
        public static void ShowMsgBox(this Window win, string msg) {
            MsgBoxSimple.ShowBox(win, msg);
        }


        /// <summary>Open a custom message box centered on caller</summary>
        /// <param name="win">The window opening the message box</param>
        /// <param name="title">The text to show on title bar</param>
        /// <param name="msg">The message to display</param>
        public static void ShowMsgBox(this Window win, string title, string msg) {
            MsgBoxSimple.ShowBox(win, title, msg);
        }


        /// <summary>Open a custom message box centered on caller</summary>
        /// <param name="win">The window opening the message box</param>
        /// <param name="msg">The message to display</param>
        public static MsgBoxResult ShowMsgBoxYesNo(this Window win, string msg, bool suppressContinue = false) {
            return MsgBoxYesNo.ShowBox(win, msg, suppressContinue);
        }


        /// <summary>Open a custom message box centered on caller</summary>
        /// <param name="win">The window opening the message box</param>
        /// <param name="title">The text to show on title bar</param>
        /// <param name="msg">The message to display</param>
        public static MsgBoxResult ShowMsgBoxYesNo(this Window win, string title, string msg, bool suppressContinue = false) {
            return MsgBoxYesNo.ShowBox(win, title, msg, suppressContinue);
        }


    }
}

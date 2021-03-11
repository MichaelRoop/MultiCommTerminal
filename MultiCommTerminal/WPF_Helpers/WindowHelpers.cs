using ChkUtils.Net;
using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WpfHelperClasses.Core;
using static MultiCommTerminal.NetCore.WindowObjs.MsgBoxYesNo;
//using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace MultiCommTerminal.NetCore.WPF_Helpers {

    public static class WindowHelpers {

        static ClassLog LOG = new ClassLog("WindowHelpers");

        /// <summary>
        /// Call in Window OnApplyTemplate for Windows using MyWindowStyle to bind drag and move
        /// </summary>
        /// <remarks>Call in the overriden OnApplyTemplate function</remarks>
        /// <param name="win">The window calling this function</param>
        public static void BindMouseDownToCustomTitleBar(this Window win) {
            try {
                if (win != null) {
                    Border b = win.Template.FindName("brdTitle", win) as Border;
                    if (b != null) {
                        b.MouseDown += (sender, args) => {
                            WrapErr.ToErrReport(9999, "Drag when mouse not down", () => {
                                if (win.WindowState == WindowState.Maximized) {
                                    // Dislodge it from maximized state to move
                                    win.WindowState = WindowState.Normal;

                                    // Center the window on the click point
                                    Point p = args.GetPosition(win);
                                    win.Top = p.Y - 15; // Middle of top bar
                                    win.Left = p.X - (win.Width / 2.0);
                                    win.DragMove();
                                }
                                else {
                                    win.DragMove();
                                }
                            });
                        };
                    }
                    else {
                        LOG.Error(9999, "Could not find PART_topBar - are you sure you have style set to MyWindowStyle?");
                    }
                }
            }
            catch (Exception e) {
                LOG.Exception(9999, "BindMouseDownToCustomTitleBar", "", e);
            }
        }


        public static void HideTitleBarIcon(this Window win) {
            try {
                if (win != null) {
                    Border b = win.Template.FindName("PART_IconBorder", win) as Border;
                    if (b != null) {
                        b.Visibility = Visibility.Collapsed;
                    }
                    else {
                        LOG.Error(9999, "Could not find PART_IconBorder - are you sure you have style set to MyWindowStyle?");
                    }
                }
            }
            catch (Exception e) {
                LOG.Exception(9999, "HideTitleBarIcon", "", e);
            }
        }


        public static void ShowMsg(string msg) {
            try {
                MsgBoxSimple.ShowBox(DI.Wrapper.GetText(MsgCode.Error), msg);
            }
            catch (Exception e) {
                LOG.Exception(9999, "ShowMsg", "", e);
            }
        }


        public static void ShowMsgTitle(string title, string msg) {
            try {
                MsgBoxSimple.ShowBox(title, msg);
            }
            catch (Exception e) {
                LOG.Exception(9999, "ShowMsgTitle", "", e);
            }
        }


        /// <summary>Open a custom message box centered on caller</summary>
        /// <param name="win">The window opening the message box</param>
        /// <param name="msg">The message to display</param>
        public static void ShowMsgBox(this Window win, string msg) {
            try {
                MsgBoxSimple.ShowBox(win, msg);
            }
            catch (Exception e) {
                LOG.Exception(9999, "ShowMsgBox", "", e);
            }
        }


        /// <summary>Open a custom message box centered on caller</summary>
        /// <param name="win">The window opening the message box</param>
        /// <param name="title">The text to show on title bar</param>
        /// <param name="msg">The message to display</param>
        public static void ShowMsgBox(this Window win, string title, string msg) {
            try {
                MsgBoxSimple.ShowBox(win, title, msg);
            }
            catch (Exception e) {
                LOG.Exception(9999, "ShowMsgBox", "", e);
            }
        }


        /// <summary>Open a custom message box centered on caller</summary>
        /// <param name="win">The window opening the message box</param>
        /// <param name="msg">The message to display</param>
        public static MsgBoxResult ShowMsgBoxYesNo(this Window win, string msg, bool suppressContinue = false) {
            try {
                return MsgBoxYesNo.ShowBox(win, msg, suppressContinue);
            }
            catch (Exception e) {
                LOG.Exception(9999, "ShowMsgBoxYesNo", "", e);
                return MsgBoxResult.No; 
            }
        }


        /// <summary>Open a custom message box centered on caller</summary>
        /// <param name="win">The window opening the message box</param>
        /// <param name="title">The text to show on title bar</param>
        /// <param name="msg">The message to display</param>
        public static MsgBoxResult ShowMsgBoxYesNo(this Window win, string title, string msg, bool suppressContinue = false) {
            try {
                return MsgBoxYesNo.ShowBox(win, title, msg, suppressContinue);
            }
            catch (Exception e) {
                LOG.Exception(9999, "ShowMsgBoxYesNo", "", e);
                return MsgBoxResult.No;
            }
        }


        /// <summary>Gets the selection and validate 
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <param name="onFound"></param>
        /// <param name="onNone"></param>
        public static void GetSelectedChk<T>(this Selector selector, Action<T> onFound, Action<string> onNone) where T : class {
            try {
                selector.GetSelected<T>(
                    onFound, () => onNone.Invoke(DI.Wrapper.GetText(MsgCode.NothingSelected)));
            }
            catch (Exception e) {
                LOG.Exception(9999, "GetSelectedChk", "", e);
            }
        }

    }
}

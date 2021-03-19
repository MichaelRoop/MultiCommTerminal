using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLECmdEdit.xaml</summary>
    public partial class BLECmdEdit : Window {

        private ClassLog log = new ClassLog("BLECmdEdit");


        //https://social.msdn.microsoft.com/Forums/vstudio/en-US/cbc8bf43-eb0e-439a-a08d-5336dd1ce5c5/wpf-how-to-make-textbox-only-allow-enter-certain-range-of-numbers?forum=wpf#:~:text=In%20our%20WPF%20application%2C%20we,number%20from%205%20to%209999.&text=To%20only%209999%2C%20we%20can,TextBox%20MaxLength%20%3D%20%224%22.

        #region Constructors and window events


        private BLE_DataType dataType = BLE_DataType.Bool;
        private Window parent = null;


        public BLECmdEdit(Window parent, BLE_DataType dataType) {
            this.parent = parent;
            this.dataType = dataType;
            InitializeComponent();

            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

        }

        #endregion

        #region Edit box events

        private void edDec_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            //TextBox tb = sender as TextBox;
            //this.log.Info("edDec_PreviewTextInput", () => string.Format(
            //    "TB.Text:{0}, Args.Text:{1}", tb.Text, e.Text));
        }


        private void edDec_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            //TextBox tb = sender as TextBox;
            //this.log.Info("edDec_PreviewMouseDown", () => string.Format(
            //    "TB.Text:{0}", tb.Text));
        }


        //        private void edDec_KeyDown(object sender, KeyEventArgs e) {
        ////            this.ProcessKeys(sender as TextBox, true, e);

        //            //TextBox tb = sender as TextBox;
        //            //this.log.Info("edDec_KeyDown", () => string.Format(
        //            //    "TB.Text:{0}, Key:{1}",
        //            //    tb.Text, e.Key.ToString()));

        //        }


        //private void edDec_KeyUp(object sender, KeyEventArgs e) {
        //    this.ProcessKeys(sender as TextBox, true, e);

        //}

        private void edDec_PreviewKeyDown(object sender, KeyEventArgs e) {
            this.ProcessKeys(sender as TextBox, true, e);
        }

        private void edDec_PreviewKeyUp(object sender, KeyEventArgs e) {
            this.ProcessKeys(sender as TextBox, true, e);
        }


        private void edDec_MouseMove(object sender, MouseEventArgs e) {
            //(sender as TextBox).SelectionLength = 0;
        }



        private void edHex_PreviewTextInput(object sender, TextCompositionEventArgs e) {

        }

        private void edBin_PreviewTextInput(object sender, TextCompositionEventArgs e) {

        }

        #endregion


        #region Button events

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.None:
                    break;
                case Key.Cancel:
                    break;
                case Key.Back:
                    break;
                case Key.Tab:
                    break;
                case Key.LineFeed:
                    break;
                case Key.Clear:
                    break;
                case Key.Enter:
                    break;
                case Key.Pause:
                    break;
                case Key.Capital:
                    break;
                case Key.HangulMode:
                    break;
                case Key.JunjaMode:
                    break;
                case Key.FinalMode:
                    break;
                case Key.HanjaMode:
                    break;
                case Key.Escape:
                    break;
                case Key.ImeConvert:
                    break;
                case Key.ImeNonConvert:
                    break;
                case Key.ImeAccept:
                    break;
                case Key.ImeModeChange:
                    break;
                case Key.Space:
                    break;
                case Key.PageUp:
                    break;
                case Key.Next:
                    break;
                case Key.End:
                    break;
                case Key.Home:
                    break;
                case Key.Left:
                    break;
                case Key.Up:
                    break;
                case Key.Right:
                    break;
                case Key.Down:
                    break;
                case Key.Select:
                    break;
                case Key.Print:
                    break;
                case Key.Execute:
                    break;
                case Key.PrintScreen:
                    break;
                case Key.Insert:
                    break;
                case Key.Delete:
                    break;
                case Key.Help:
                    break;
                case Key.D0:
                    break;
                case Key.D1:
                    break;
                case Key.D2:
                    break;
                case Key.D3:
                    break;
                case Key.D4:
                    break;
                case Key.D5:
                    break;
                case Key.D6:
                    break;
                case Key.D7:
                    break;
                case Key.D8:
                    break;
                case Key.D9:
                    this.edDec.Text = string.Format("{0}{1}", this.edDec.Text, e.Key.ToString());

                    break;
                case Key.A:
                    break;
                case Key.B:
                    break;
                case Key.C:
                    break;
                case Key.D:
                    break;
                case Key.E:
                    break;
                case Key.F:
                    break;
                case Key.G:
                    break;
                case Key.H:
                    break;
                case Key.I:
                    break;
                case Key.J:
                    break;
                case Key.K:
                    break;
                case Key.L:
                    break;
                case Key.M:
                    break;
                case Key.N:
                    break;
                case Key.O:
                    break;
                case Key.P:
                    break;
                case Key.Q:
                    break;
                case Key.R:
                    break;
                case Key.S:
                    break;
                case Key.T:
                    break;
                case Key.U:
                    break;
                case Key.V:
                    break;
                case Key.W:
                    break;
                case Key.X:
                    break;
                case Key.Y:
                    break;
                case Key.Z:
                    break;
                case Key.LWin:
                    break;
                case Key.RWin:
                    break;
                case Key.Apps:
                    break;
                case Key.Sleep:
                    break;
                case Key.NumPad0:
                    break;
                case Key.NumPad1:
                    break;
                case Key.NumPad2:
                    this.edDec.Text = string.Format("{0}{1}", this.edDec.Text, e.Key.ToString());
                    break;
                case Key.NumPad3:
                    break;
                case Key.NumPad4:
                    break;
                case Key.NumPad5:
                    break;
                case Key.NumPad6:
                    break;
                case Key.NumPad7:
                    break;
                case Key.NumPad8:
                    break;
                case Key.NumPad9:
                    break;
                case Key.Multiply:
                    break;
                case Key.Add:
                    break;
                case Key.Separator:
                    break;
                case Key.Subtract:
                    break;
                case Key.Decimal:
                    break;
                case Key.Divide:
                    break;
                case Key.F1:
                    break;
                case Key.F2:
                    break;
                case Key.F3:
                    break;
                case Key.F4:
                    break;
                case Key.F5:
                    break;
                case Key.F6:
                    break;
                case Key.F7:
                    break;
                case Key.F8:
                    break;
                case Key.F9:
                    break;
                case Key.F10:
                    break;
                case Key.F11:
                    break;
                case Key.F12:
                    break;
                case Key.F13:
                    break;
                case Key.F14:
                    break;
                case Key.F15:
                    break;
                case Key.F16:
                    break;
                case Key.F17:
                    break;
                case Key.F18:
                    break;
                case Key.F19:
                    break;
                case Key.F20:
                    break;
                case Key.F21:
                    break;
                case Key.F22:
                    break;
                case Key.F23:
                    break;
                case Key.F24:
                    break;
                case Key.NumLock:
                    break;
                case Key.Scroll:
                    break;
                case Key.LeftShift:
                    break;
                case Key.RightShift:
                    break;
                case Key.LeftCtrl:
                    break;
                case Key.RightCtrl:
                    break;
                case Key.LeftAlt:
                    break;
                case Key.RightAlt:
                    break;
                case Key.BrowserBack:
                    break;
                case Key.BrowserForward:
                    break;
                case Key.BrowserRefresh:
                    break;
                case Key.BrowserStop:
                    break;
                case Key.BrowserSearch:
                    break;
                case Key.BrowserFavorites:
                    break;
                case Key.BrowserHome:
                    break;
                case Key.VolumeMute:
                    break;
                case Key.VolumeDown:
                    break;
                case Key.VolumeUp:
                    break;
                case Key.MediaNextTrack:
                    break;
                case Key.MediaPreviousTrack:
                    break;
                case Key.MediaStop:
                    break;
                case Key.MediaPlayPause:
                    break;
                case Key.LaunchMail:
                    break;
                case Key.SelectMedia:
                    break;
                case Key.LaunchApplication1:
                    break;
                case Key.LaunchApplication2:
                    break;
                case Key.Oem1:
                    break;
                case Key.OemPlus:
                    break;
                case Key.OemComma:
                    break;
                case Key.OemMinus:
                    break;
                case Key.OemPeriod:
                    break;
                case Key.Oem2:
                    break;
                case Key.Oem3:
                    break;
                case Key.AbntC1:
                    break;
                case Key.AbntC2:
                    break;
                case Key.Oem4:
                    break;
                case Key.Oem5:
                    break;
                case Key.Oem6:
                    break;
                case Key.Oem7:
                    break;
                case Key.Oem8:
                    break;
                case Key.Oem102:
                    break;
                case Key.ImeProcessed:
                    break;
                case Key.System:
                    break;
                case Key.DbeAlphanumeric:
                    break;
                case Key.DbeKatakana:
                    break;
                case Key.DbeHiragana:
                    break;
                case Key.DbeSbcsChar:
                    break;
                case Key.DbeDbcsChar:
                    break;
                case Key.DbeRoman:
                    break;
                case Key.Attn:
                    break;
                case Key.CrSel:
                    break;
                case Key.DbeEnterImeConfigureMode:
                    break;
                case Key.DbeFlushString:
                    break;
                case Key.DbeCodeInput:
                    break;
                case Key.DbeNoCodeInput:
                    break;
                case Key.DbeDetermineString:
                    break;
                case Key.DbeEnterDialogConversionMode:
                    break;
                case Key.OemClear:
                    break;
                case Key.DeadCharProcessed:
                    break;
            }


        }


        private void ProcessKeys(TextBox t, bool hex, KeyEventArgs args) {

            this.log.Info("ProcessKeys", () => string.Format("KEY '{0}'", args.Key.ToString()));



            if (this.IsSysKey(args.Key)) {
                args.Handled = false;
                // At this point you need to see the back or delete button result

                return;
            }

            // TODO - intercept other keys like space to block them
            if (args.Key == Key.Space) {
                this.log.Info("ProcessKeys", "Space");
                args.Handled = true;
                return;
            }


            string current = t.Text;
            string newVal = this.GetNumericValue(args.Key);
            if (newVal == string.Empty && hex) {
                newVal = this.GetHexValue(args.Key);
            }

            this.log.Info("ProcessKeys", () => string.Format("Old:{0}  Extra: '{1}'", t.Text, newVal));

            if (newVal.Length > 0) {
                // can do some validation here
                t.Text = string.Format("{0}{1}", t.Text, newVal);
                this.log.Info("ProcessKeys", () => string.Format("Completed:{0}", t.Text));
            }
            t.Focus();
            t.CaretIndex = t.Text.Length;
            args.Handled = true;
        }


        private bool IsSysKey(Key key) {
            switch (key) {
                case Key.None:
                case Key.Cancel:
                case Key.Back:
                case Key.Tab:
                case Key.LineFeed:
                case Key.Clear:
                case Key.Enter:
                case Key.Delete:
                    return true;

                default:
                    return false;
            }
        }




        private string GetNumericValue(Key key) {
            switch (key) {
                case Key.NumPad0:
                case Key.D0:
                    return "0";
                case Key.NumPad1:
                case Key.D1:
                    return "1";
                case Key.NumPad2:
                case Key.D2:
                    return "2";
                case Key.NumPad3:
                case Key.D3:
                    return "3";
                case Key.NumPad4:
                case Key.D4:
                    return "4";
                case Key.NumPad5:
                case Key.D5:
                    return "5";
                case Key.NumPad6:
                case Key.D6:
                    return "6";
                case Key.NumPad7:
                case Key.D7:
                    return "7";
                case Key.NumPad8:
                case Key.D8:
                    return "8";
                case Key.NumPad9:
                case Key.D9:
                    return "9";
                default:
                    return string.Empty;
            }
        }

        private string GetHexValue(Key key) {
            switch (key) {
                case Key.A:
                    return "A";
                case Key.B:
                    return "B";
                case Key.C:
                    return "C";
                case Key.D:
                    return "D";
                case Key.E:
                    return "E";
                case Key.F:
                    return "F";
                default:
                    return string.Empty;
            }

        }

    }
}

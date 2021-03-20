using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
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
using WpfHelperClasses.Core;

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
            this.txtDataType.Content = this.dataType.ToStr();
            DI.Wrapper.BLE_GetRangeDisplay(
                this.dataType, str => this.txtRange.Content = str, this.onFailure);
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.CenterToParent(this.parent);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

        }

        #endregion

        #region Edit box events


        private void ValidateRange(string value, KeyEventArgs args) {
            DI.Wrapper.ValidateBLEValue(
                this.dataType, value, () => { },
                err => {
                    args.Handled = true;
                    App.ShowMsg(err);
                });
        }

        /// <summary>Prevent invalid characters ever getting entered</summary>
        /// <param name="sender">The EditBox</param>
        /// <param name="args">The event args</param>
        private void tbDec_PreviewKeyUp(object sender, KeyEventArgs args) {
            try {
                // Filter out forbidden characters and A-F
                if (this.IsForebidden(args.Key) || this.IsHex(args.Key)) {
                    args.Handled = true;
                    return;
                }
                else {
                    if (this.IsNumeric(args.Key)) {
                        string s = this.GetNumericValue(args.Key);
                        this.ValidateRange(string.Format("{0}{1}", this.tbDec.Text, s), args);
                    }
                }
            }
            catch(Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }


        /// <summary>Invalid characters already filtered. Do translation</summary>
        /// <param name="sender">The EditBox</param>
        /// <param name="args">The event args</param>
        private void tbDec_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                // remove multiple leading zeros
                this.log.Info("tbDec_TextChanged", this.tbDec.Text);
                if (this.tbDec.Text.Length == 0) {
                    this.edHex.Text = "";
                    this.edBin.Text = "";
                }
                else {
                    //// Now translate to hex and binary
                    this.edHex.Text = UInt16.Parse(this.tbDec.Text).ToString("X");
                    this.edBin.Text = this.ToBinary(Convert.ToUInt32(this.tbDec.Text));
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }



        private void edHex_PreviewKeyDown(object sender, KeyEventArgs args) {
            try {
                if (this.IsForebidden(args.Key)) {
                    args.Handled = true;
                    return;
                }
                else {
                    if (this.IsHexDecimal(args.Key)) {
                        var s = string.Format("{0}{1}", this.edHex.Text, this.GetHexDecimalValue(args.Key));

                        this.log.Info("edHex_PreviewKeyDown", () =>
                            string.Format("'{0}'  '{1}'  '{2}'  '{3}'", 
                            args.Key.ToString(), this.edHex.Text, this.GetHexDecimalValue(args.Key), s));
                        
                        UInt32 tmp = Convert.ToUInt32(s, 16);
                        this.ValidateRange(tmp.ToString(), args);
                    }
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }

        }

        private void edHex_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                // remove multiple leading zeros
                this.log.Info("tbDec_TextChanged", this.edHex.Text);
                if (this.edHex.Text.Length == 0) {
                    this.tbDec.Text = "";
                    this.edBin.Text = "";
                }
                else {
                    //// Now translate to decimal and binary
                    UInt32 val = Convert.ToUInt32(this.edHex.Text, 16);
                    this.tbDec.Text = val.ToString();
                    this.edBin.Text = this.ToBinary(val);
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
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


        private void onFailure(string msg) {
            App.ShowMsg(msg);
            this.Close();
        }


        #endregion



        //private void ProcessKeys(TextBox t, bool hex, KeyEventArgs args) {
        //    this.log.Info("ProcessKeys", () => string.Format("KEY '{0}'", args.Key.ToString()));

        //    // Allow certain system keys to go through
        //    if (!this.IsSysKey(args.Key)) {
        //        string current = t.Text;
        //        string newVal = this.GetNumericValue(args.Key);
        //        if (newVal == string.Empty && hex) {
        //            newVal = this.GetHexValue(args.Key);
        //        }

        //        if (newVal.Length > 0) {
        //            // can do some validation here
        //            t.Text = string.Format("{0}{1}", t.Text, newVal);
        //            this.log.Info("ProcessKeys", () => string.Format("Completed:{0}", t.Text));
        //        }
        //        t.Focus();
        //        t.CaretIndex = t.Text.Length;

        //        // Consider all other keys handled
        //        args.Handled = true;

        //    }
        //}


        private void ProcessNumericBox(KeyEventArgs args) {
            // Filter out forbidden characters and A-F
            if (this.IsForebidden(args.Key) || this.IsHex(args.Key)) {
                args.Handled = true;
                return;
            }

            if (this.IsNumeric(args.Key)) {
                //string newVal = this.GetNumericValue(args.Key);
                //if (newVal != string.Empty) {
                    string newVal = string.Format("{0}{1}", 
                        this.tbDec.Text,
                        this.GetNumericValue(args.Key));


                    // Validate the range
                    DI.Wrapper.ValidateBLEValue(
                        this.dataType,
                        newVal,
                        () => {
                            //this.SetText(this.tbDec, newVal);
                            //// Now translate to hex and binary
                            //this.edHex.Text = UInt16.Parse(newVal).ToString("X");
                            //this.edBin.Text = this.ToBinary(Convert.ToUInt32(newVal));
                        },
                        err => {
                            args.Handled = true;
                            App.ShowMsg(err);
                        });
                //}
                //else {
                //    args.Handled = true;

                //}
            }







            //// Allow certain system keys to go through
            //if (!this.IsSysKey(args.Key)) {
            //    //string current = this.tbDec.Text;
            //    string newVal = this.GetNumericValue(args.Key);
            //    if (newVal != string.Empty) {
            //        newVal = string.Format("{0}{1}", this.tbDec.Text, newVal);
            //        // Validate the range
            //        DI.Wrapper.ValidateBLEValue(
            //            this.dataType,
            //            newVal,
            //            () => {
            //                this.SetText(this.tbDec, newVal);

            //                // Now translate to hex and binary
            //                this.edHex.Text = UInt16.Parse(newVal).ToString("X");
            //                this.edBin.Text = this.ToBinary(Convert.ToUInt32(newVal));
            //            },
            //            App.ShowMsg);
            //    }
            //    // Consider all other keys handled
            //    args.Handled = true;
            //}


        }

        //https://stackoverflow.com/questions/2954962/convert-integer-to-binary-in-c-sharp
        public string ToBinary(UInt32 base10) {
            string binary = "";
            do {
                binary = (base10 % 2) + binary;
                base10 /= 2;
            }
            while (base10 > 0);

            // Need to add spaces
            Char[] arr = binary.ToCharArray();
            Array.Reverse(arr);
            List<char> target = new List<char>();
            for (int i = 0; i < arr.Length; i++) {
                if (i % 4 == 0) {
                    target.Add(' ');
                }
                target.Add(arr[i]);
            }

            target.Reverse();
            return new string(target.ToArray());



            //return binary;
        }


        private void SetText(TextBox t, string text) {
            t.Text = text;
            t.Focus();
            t.CaretIndex = this.tbDec.Text.Length;
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



        //private bool IsNumeric(Key key) {
        //    if ((key >= Key.NumPad0 && key <= Key.NumPad9) || 
        //        (key >= Key.D0 && key <= Key.D9)) {
        //        return true;
        //    }
        //    return false;
        //}


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




        private string GetHexDecimalValue(Key key) {
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
                    return this.GetNumericValue(key);
            }
        }


        private bool IsNumeric(Key key) {
            switch (key) {
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                    return true;
                default:
                    return false;
            }
        }


        private bool IsHex(Key key) {
            switch (key) {
                case Key.A:
                case Key.B:
                case Key.C:
                case Key.D:
                case Key.E:
                case Key.F:
                    return true;
                default:
                    return false;
            }
        }


        private bool IsHexDecimal(Key key) {
            switch (key) {
                case Key.A:
                case Key.B:
                case Key.C:
                case Key.D:
                case Key.E:
                case Key.F:
                    return true;
                default:
                    return this.IsNumeric(key);

            }
        }




        private bool IsForebidden(Key key) {
            switch (key) {
                case Key.Tab:
                case Key.Space:
                case Key.LineFeed:
                case Key.Clear:
                case Key.Enter:
                case Key.PageUp:
                case Key.Next:
                case Key.End:
                case Key.Home:
                case Key.Multiply:
                case Key.Add:
                case Key.Separator:
                case Key.Subtract:
                case Key.Decimal:
                case Key.Divide:
                case Key.G:
                case Key.H:
                case Key.I:
                case Key.J:
                case Key.K:
                case Key.L:
                case Key.M:
                case Key.N:
                case Key.O:
                case Key.P:
                case Key.Q:
                case Key.R:
                case Key.S:
                case Key.T:
                case Key.U:
                case Key.V:
                case Key.W:
                case Key.X:
                case Key.Y:
                case Key.Z:
                    return true;
                default:
                    return false;

                    #region not used
                    //case Key.None:
                    //    break;
                    //case Key.Cancel:
                    //    break;
                    //case Key.Back:
                    //    break;
                    //case Key.LineFeed:
                    //    break;
                    //case Key.Clear:
                    //    break;
                    //case Key.Enter:
                    //    break;
                    //case Key.Pause:
                    //    break;
                    //case Key.Capital:
                    //    break;
                    //case Key.HangulMode:
                    //    break;
                    //case Key.JunjaMode:
                    //    break;
                    //case Key.FinalMode:
                    //    break;
                    //case Key.HanjaMode:
                    //    break;
                    //case Key.Escape:
                    //    break;
                    //case Key.ImeConvert:
                    //    break;
                    //case Key.ImeNonConvert:
                    //    break;
                    //case Key.ImeAccept:
                    //    break;
                    //case Key.ImeModeChange:
                    //    break;
                    //case Key.PageUp:
                    //    break;
                    //case Key.Next:
                    //    break;
                    //case Key.End:
                    //    break;
                    //case Key.Home:
                    //    break;
                    //case Key.Left:
                    //    break;
                    //case Key.Up:
                    //    break;
                    //case Key.Right:
                    //    break;
                    //case Key.Down:
                    //    break;
                    //case Key.Select:
                    //    break;
                    //case Key.Print:
                    //    break;
                    //case Key.Execute:
                    //    break;
                    //case Key.PrintScreen:
                    //    break;
                    //case Key.Insert:
                    //    break;
                    //case Key.Delete:
                    //    break;
                    //case Key.Help:
                    //    break;
                    //case Key.D0:
                    //    break;
                    //case Key.D1:
                    //    break;
                    //case Key.D2:
                    //    break;
                    //case Key.D3:
                    //    break;
                    //case Key.D4:
                    //    break;
                    //case Key.D5:
                    //    break;
                    //case Key.D6:
                    //    break;
                    //case Key.D7:
                    //    break;
                    //case Key.D8:
                    //    break;
                    //case Key.D9:
                    //    this.edDec.Text = string.Format("{0}{1}", this.edDec.Text, e.Key.ToString());

                    //    break;
                    //case Key.A:
                    //    break;
                    //case Key.B:
                    //    break;
                    //case Key.C:
                    //    break;
                    //case Key.D:
                    //    break;
                    //case Key.E:
                    //    break;
                    //case Key.F:
                    //    break;
                    //case Key.G:
                    //    break;
                    //case Key.H:
                    //    break;
                    //case Key.I:
                    //    break;
                    //case Key.J:
                    //    break;
                    //case Key.K:
                    //    break;
                    //case Key.L:
                    //    break;
                    //case Key.M:
                    //    break;
                    //case Key.N:
                    //    break;
                    //case Key.O:
                    //    break;
                    //case Key.P:
                    //    break;
                    //case Key.Q:
                    //    break;
                    //case Key.R:
                    //    break;
                    //case Key.S:
                    //    break;
                    //case Key.T:
                    //    break;
                    //case Key.U:
                    //    break;
                    //case Key.V:
                    //    break;
                    //case Key.W:
                    //    break;
                    //case Key.X:
                    //    break;
                    //case Key.Y:
                    //    break;
                    //case Key.Z:
                    //    break;
                    //case Key.LWin:
                    //    break;
                    //case Key.RWin:
                    //    break;
                    //case Key.Apps:
                    //    break;
                    //case Key.Sleep:
                    //    break;
                    //case Key.NumPad0:
                    //    break;
                    //case Key.NumPad1:
                    //    break;
                    //case Key.NumPad2:
                    //    this.edDec.Text = string.Format("{0}{1}", this.edDec.Text, e.Key.ToString());
                    //    break;
                    //case Key.NumPad3:
                    //    break;
                    //case Key.NumPad4:
                    //    break;
                    //case Key.NumPad5:
                    //    break;
                    //case Key.NumPad6:
                    //    break;
                    //case Key.NumPad7:
                    //    break;
                    //case Key.NumPad8:
                    //    break;
                    //case Key.NumPad9:
                    //    break;
                    //case Key.Multiply:
                    //    break;
                    //case Key.Add:
                    //    break;
                    //case Key.Separator:
                    //    break;
                    //case Key.Subtract:
                    //    break;
                    //case Key.Decimal:
                    //    break;
                    //case Key.Divide:
                    //    break;
                    //case Key.F1:
                    //    break;
                    //case Key.F2:
                    //    break;
                    //case Key.F3:
                    //    break;
                    //case Key.F4:
                    //    break;
                    //case Key.F5:
                    //    break;
                    //case Key.F6:
                    //    break;
                    //case Key.F7:
                    //    break;
                    //case Key.F8:
                    //    break;
                    //case Key.F9:
                    //    break;
                    //case Key.F10:
                    //    break;
                    //case Key.F11:
                    //    break;
                    //case Key.F12:
                    //    break;
                    //case Key.F13:
                    //    break;
                    //case Key.F14:
                    //    break;
                    //case Key.F15:
                    //    break;
                    //case Key.F16:
                    //    break;
                    //case Key.F17:
                    //    break;
                    //case Key.F18:
                    //    break;
                    //case Key.F19:
                    //    break;
                    //case Key.F20:
                    //    break;
                    //case Key.F21:
                    //    break;
                    //case Key.F22:
                    //    break;
                    //case Key.F23:
                    //    break;
                    //case Key.F24:
                    //    break;
                    //case Key.NumLock:
                    //    break;
                    //case Key.Scroll:
                    //    break;
                    //case Key.LeftShift:
                    //    break;
                    //case Key.RightShift:
                    //    break;
                    //case Key.LeftCtrl:
                    //    break;
                    //case Key.RightCtrl:
                    //    break;
                    //case Key.LeftAlt:
                    //    break;
                    //case Key.RightAlt:
                    //    break;
                    //case Key.BrowserBack:
                    //    break;
                    //case Key.BrowserForward:
                    //    break;
                    //case Key.BrowserRefresh:
                    //    break;
                    //case Key.BrowserStop:
                    //    break;
                    //case Key.BrowserSearch:
                    //    break;
                    //case Key.BrowserFavorites:
                    //    break;
                    //case Key.BrowserHome:
                    //    break;
                    //case Key.VolumeMute:
                    //    break;
                    //case Key.VolumeDown:
                    //    break;
                    //case Key.VolumeUp:
                    //    break;
                    //case Key.MediaNextTrack:
                    //    break;
                    //case Key.MediaPreviousTrack:
                    //    break;
                    //case Key.MediaStop:
                    //    break;
                    //case Key.MediaPlayPause:
                    //    break;
                    //case Key.LaunchMail:
                    //    break;
                    //case Key.SelectMedia:
                    //    break;
                    //case Key.LaunchApplication1:
                    //    break;
                    //case Key.LaunchApplication2:
                    //    break;
                    //case Key.Oem1:
                    //    break;
                    //case Key.OemPlus:
                    //    break;
                    //case Key.OemComma:
                    //    break;
                    //case Key.OemMinus:
                    //    break;
                    //case Key.OemPeriod:
                    //    break;
                    //case Key.Oem2:
                    //    break;
                    //case Key.Oem3:
                    //    break;
                    //case Key.AbntC1:
                    //    break;
                    //case Key.AbntC2:
                    //    break;
                    //case Key.Oem4:
                    //    break;
                    //case Key.Oem5:
                    //    break;
                    //case Key.Oem6:
                    //    break;
                    //case Key.Oem7:
                    //    break;
                    //case Key.Oem8:
                    //    break;
                    //case Key.Oem102:
                    //    break;
                    //case Key.ImeProcessed:
                    //    break;
                    //case Key.System:
                    //    break;
                    //case Key.DbeAlphanumeric:
                    //    break;
                    //case Key.DbeKatakana:
                    //    break;
                    //case Key.DbeHiragana:
                    //    break;
                    //case Key.DbeSbcsChar:
                    //    break;
                    //case Key.DbeDbcsChar:
                    //    break;
                    //case Key.DbeRoman:
                    //    break;
                    //case Key.Attn:
                    //    break;
                    //case Key.CrSel:
                    //    break;
                    //case Key.DbeEnterImeConfigureMode:
                    //    break;
                    //case Key.DbeFlushString:
                    //    break;
                    //case Key.DbeCodeInput:
                    //    break;
                    //case Key.DbeNoCodeInput:
                    //    break;
                    //case Key.DbeDetermineString:
                    //    break;
                    //case Key.DbeEnterDialogConversionMode:
                    //    break;
                    //case Key.OemClear:
                    //    break;
                    //case Key.DeadCharProcessed:
                    //    break;
                    #endregion

            }
        }




#if BLAH_THIS_IS_FULL_SWITCH
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


#endif

    }
}

using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.Utils {

    /// <summary>Interaction logic for CrashReport.xaml</summary>
    public partial class CrashReport : Window {

        #region Data

        private Window parent = null;
        private ButtonGroupSizeSyncManager buttonWidthManager = null;

        #endregion

        #region Constructors and window events

        public static void ShowBox(Exception ex, Window parent) {
            CrashReport win = new CrashReport(ex, parent);
            win.ShowDialog();
        }


        public CrashReport(Exception ex, Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.ProcessException(ex);
            this.buttonWidthManager = new ButtonGroupSizeSyncManager(this.btnCopy, this.btnCancel, this.btnEmail);
            this.buttonWidthManager.PrepForChange();
        }

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.SizeToContent = SizeToContent.WidthAndHeight;
            if (this.parent != null) {
                this.CenterToParent(this.parent);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.buttonWidthManager.Teardown();
        }

        #endregion

        #region Button handlers

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e) {
            this.errBox.SelectAll();
            this.errBox.Copy();
            this.errBox.Select(0, 0);
            this.Close();
        }


        private void btnEmail_Click(object sender, RoutedEventArgs e) {
            try {
                string body = this.errBox.Text.Replace("\r\n", "%0d%0A");
                StringBuilder sb = new StringBuilder();
                sb.Append("mailto:MultiCommTerminal@gmail.com")
                    .Append("?subject=Multi Comm Terminal CRASH REPORT")
                    .Append("&body=App Build number:").Append(TxtBinder.BuildNumber).Append("%0d%0A").Append("%0d%0A")
                    .Append(DateTime.Now.ToLongDateString()).Append("%0d%0A").Append("%0d%0A")
                    .Append(body).Append("%0d%0A");
                Process.Start(new ProcessStartInfo(sb.ToString()) { UseShellExecute = true });
            }
            catch (Exception) {
            }
            this.Close();
        }

        #endregion

        #region Private

        private void ProcessException(Exception e) {
            try {
                if (e != null) {
                    ErrReportException erex = e as ErrReportException;
                    ErrReport report = null;
                    if (erex != null) {
                        report = erex.Report;
                    }
                    else {
                        report = WrapErr.GetErrReport(0, e.Message, e);
                    }
                    this.errBox.Text = string.Format("{0}\r\n{1}", erex.Report.Msg,erex.Report.StackTrace);
                }
                else {
                    this.errBox.Text = "Null exception. No info";
                }
            }
            catch (Exception) {
                this.errBox.Text = "Failed to populate";
            }
        }

        #endregion

    }

}

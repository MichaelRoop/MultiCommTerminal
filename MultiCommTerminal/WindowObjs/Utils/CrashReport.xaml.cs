using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using MultiCommData.Net.Enumerations;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MultiCommTerminal.NetCore.WindowObjs.Utils {

    /// <summary>Interaction logic for CrashReport.xaml</summary>
    public partial class CrashReport : Window {

        #region Data

        private ButtonGroupSizeSyncManager buttonWidthManager = null;

        #endregion

        #region Constructors and window events

        public static void ShowBox(Exception ex) {
            CrashReport win = new CrashReport(ex);
            win.ShowDialog();
        }


        public CrashReport(Exception ex) {
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
            //this.CenterToParent(this.parent);
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
            string uri = 
                string.Format(
                    @"mailto:MultiCommTerminal@gmail.com?subject=Multi Comm Terminal Support Question&body=App Build number:{0}\n\n{1}", 
                    TxtBinder.BuildNumber, this.errBox.Text);
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            this.Close();
        }

        #endregion


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
                    this.errBox.Text = string.Format("{0}\n{1}", erex.Report.Msg,erex.Report.StackTrace);
                }
                else {
                    this.errBox.Text = "Null exception. No info";
                }
            }
            catch (Exception) {
                this.errBox.Text = "Failed to populate";
            }
        }
            




    }
}

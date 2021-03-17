using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LogUtils.Net;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Text;
using System.Threading.Tasks;
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
            try {
                CrashReport win = new CrashReport(ex, parent);
                win.ShowDialog();
            }
            catch (Exception e) {
                Log.Exception(9999, "CrashReport", "ShowBox", e);
            }
        }

        public static void ShowBox(ErrReport report, Window parent) {
            CrashReport win = new CrashReport(report, parent);
            win.ShowDialog();
        }


        public CrashReport(Exception ex, Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.ProcessException(ex);
            this.buttonWidthManager = new ButtonGroupSizeSyncManager(this.btnCopy, this.btnCancel, this.btnEmail);
            this.buttonWidthManager.PrepForChange();
        }

        public CrashReport(ErrReport report, Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.ProcessException(report);
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
            try {
                this.errBox.SelectAll();
                this.errBox.Copy();
                StringBuilder sb = new StringBuilder();
                sb.Append("Date: ").Append(DateTime.Now.ToLongDateString()).AppendLine("")
                    .Append("Windows Ver: ").Append(Environment.OSVersion.VersionString).AppendLine("")
                    .Append("App Build number: ").Append(TxtBinder.BuildNumber).AppendLine("").AppendLine("")
                    .Append(Clipboard.GetText());
                Clipboard.Clear();
                Clipboard.SetText(sb.ToString());
                this.errBox.Select(0, 0);
                this.Close();
            }
            catch(Exception ex) {
                Log.Exception(8888, "btnCopy_Click", "", ex);
                this.Close();
            }
        }


        private void btnEmail_Click(object sender, RoutedEventArgs e) {
            Dispatcher.Invoke(async () => {
                try {
                    string body = this.errBox.Text.Replace("\r\n", "%0d%0A");
                    StringBuilder sb = new StringBuilder();
                    sb.Append("mailto:MultiCommTerminal@gmail.com")
                        .Append("?subject=Multi Comm Terminal CRASH REPORT")
                        .Append("&body=")
                        .Append("Date: ").Append(DateTime.Now.ToLongDateString()).Append("%0d%0A")
                        .Append("Windows Ver: ").Append(Environment.OSVersion.VersionString).Append("%0d%0A")
                        .Append("App Build number: ").Append(TxtBinder.BuildNumber).Append("%0d%0A").Append("%0d%0A")
                        .Append(body).Append("%0d%0A");
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(sb.ToString()));
                    this.Close();
                }
                catch (Exception ex) {
                    Log.Exception(8888, "btnEmail_Click", "", ex);
                    this.Close();
                }
            });
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
                    this.errBox.Text = string.Format(
                        "{0}  {1}:{2}\r\n{3}\r\n{4}", 
                        report.Code, report.AtClass, report.AtMethod, report.Msg, report.StackTrace);
                }
                else {
                    this.errBox.Text = "Null exception. No info";
                }
            }
            catch (Exception) {
                this.errBox.Text = "Failed to populate";
            }
        }


        private void ProcessException(ErrReport report) {
            try {
                if (report != null) {
                    this.errBox.Text = string.Format("{0}\r\n{1}", report.Msg, report.StackTrace);
                }
                else {
                    this.errBox.Text = "Null exception. No info";
                }
            }
            catch (Exception) {
                this.errBox.Text = "Failed to populate";
            }
        }

        #region Experimental code
#if THISDOESNOTWORK
        private void DoEmail() {

            StringBuilder sb = new StringBuilder();
            sb
                .Append("&body=App Build numberXXX:").Append(TxtBinder.BuildNumber).Append("%0d%0A").Append("%0d%0A")
                .Append(DateTime.Now.ToLongDateString()).Append("%0d%0A").Append("%0d%0A")
                .Append(this.errBox.Text.Replace("\r\n", "%0d%0A")).Append("%0d%0A");
            string body = sb.ToString();

            // This does not work. Suspend for now. App store would not approve either. More work to do
            // https://social.msdn.microsoft.com/Forums/windowsapps/en-US/d4e48c03-33e8-4bd2-b0f1-fd68100a2c04/uwpsend-emails-through-outlook-from-uwp-out-of-process-background-task?forum=wpdevelop
            EmailMessage emailMsg = new Windows.ApplicationModel.Email.EmailMessage();
            emailMsg.Subject = "MultiCommTerminal CRASH REPORT";
            emailMsg.Body = body;
            EmailRecipient recipient = new EmailRecipient("MultiCommTerminal@gmail.com", "Support");
            emailMsg.To.Add(recipient);
            AutoResetEvent done = new AutoResetEvent(false);

            Task.Run(async () => {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () => {
                        try {
                            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMsg);
                            done.Set();
                            App.ShowMsg("Done opening email");
                        }
                        catch (Exception e) {
                            App.ShowMsg("Crash opening email\r\n\r\n" + e.Message);
                            done.Set();
                        }
                    });
            });

            App.ShowMsg("Waiting");
            done.WaitOne();
            App.ShowMsg("Done Waiting");

        }
#endif
        #endregion

        #endregion

    }

}

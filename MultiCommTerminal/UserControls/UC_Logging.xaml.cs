using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.UserControls {

    /// <summary>Interaction logic for UC_Logging.xaml</summary>
    public partial class UC_Logging : UserControl {

        private ClassLog log = new ClassLog("UC_Logging");
        private ScrollViewer logScroll = null;
        private ButtonGroupSizeSyncManager buttonSizer = null;

        public event EventHandler OnMsgReceived;


        public UC_Logging() {
            InitializeComponent();
            this.buttonSizer = new ButtonGroupSizeSyncManager(this.btnClearLog, this.btnCopyLog);
            this.buttonSizer.PrepForChange();
        }


        /// <summary>Call when parent window is loaded</summary>
        public void OnStartup() {
            try {
                DI.Wrapper.CurrentSupportedLanguage(this.SetLanguage);
                this.logScroll = this.lbLog.GetScrollViewer();
                DI.Wrapper.LanguageChanged += this.languageChangedHandler;
                App.STATIC_APP.LogMsgEvent += this.AppLogMsgEventHandler;
            }
            catch(Exception ex) {
                this.log.Exception(9999, "OnLoaded", "", ex);
            }
        }


        /// <summary>Call when parent window shutting down</summary>
        public void OnShutdown() {
            DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
            App.STATIC_APP.LogMsgEvent -= this.AppLogMsgEventHandler;
        }


        private void btnCopyLog_Click(object sender, RoutedEventArgs e) {
            try {
                lock (this.lbLog) {
                    StringBuilder sb = new StringBuilder();
                    this.lbLog.SelectAll();
                    foreach (var item in lbLog.SelectedItems) {
                        sb.AppendLine(item.ToString());
                    }
                    Clipboard.SetText(sb.ToString());
                    this.lbLog.SelectedItem = null;
                    this.lbLog.UnselectAll();
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "btnCopyLog_Click", "", ex);
            }
        }


        private void btnClearLog_Click(object sender, RoutedEventArgs e) {
            try {
                lock (this.lbLog) {
                    if (this.logScroll != null) {
                        this.lbLog.Items.Clear();
                    }
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "btnClearLog_Click", "", ex);
            }
        }


        private void AppLogMsgEventHandler(object sender, string msg) {
            // Race condition with messages coming before window rendered
            try {
                lock (this.lbLog) {
                    if (this.logScroll != null) {
                        this.lbLog.AddAndScroll(msg, this.logScroll, 400);
                        try {
                            this.OnMsgReceived?.Invoke(this, new EventArgs());
                        }
                        catch (Exception) {
                        }
                    }
                }
            }
            catch (Exception) { }
        }



        private void SetLanguage(SupportedLanguage l) {
            this.Dispatcher.Invoke(() => {
                this.buttonSizer.PrepForChange();
                this.InvalidateVisual();
                this.btnClearLog.Content = l.GetText(MsgCode.Clear);
                this.btnCopyLog.Content = l.GetText(MsgCode.copy);
            });
        }


        private void languageChangedHandler(object sender, SupportedLanguage language) {
            this.SetLanguage(language);
        }

    }

}

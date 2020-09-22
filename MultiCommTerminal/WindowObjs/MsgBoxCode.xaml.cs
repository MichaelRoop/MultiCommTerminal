using AurelienRibon.Ui.SyntaxHighlightBox.src;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {
    /// <summary>
    /// Interaction logic for MsgBoxCode.xaml
    /// </summary>
    /// <remarks>
    /// using an archived project as starting point for highlightintg
    /// Had to modify to build with .NET 4.7. Namespace could not be save 
    /// as Assembly name. Also added CPPDL a C++ higlight definition file
    /// https://archive.codeplex.com/?p=syntaxhighlightbox
    /// </remarks>

    public partial class MsgBoxCode : Window {

        private Window parent = null;
        private CommMediumType medium = CommMediumType.None;
        private ButtonGroupSizeSyncManager buttonWidthManager = null;


        public MsgBoxCode(Window parent, CommMediumType medium) {
            this.parent = parent;
            this.medium = medium;
            InitializeComponent();
            this.Title = DI.Wrapper.GetText(medium);
            WrapErr.ToErrReport(9999, () => this.Icon = medium.ResourceWhiteBitmap());
            this.codeBox.CurrentHighlighter = HighlighterManager.Instance.Highlighters["CPPDL"];
            this.buttonWidthManager = new ButtonGroupSizeSyncManager(this.btnCopy, this.btnExit);
            this.buttonWidthManager.PrepForChange();
        }


        /// <summary>Bind Mouse drag to Template style</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            DI.Wrapper.GetCodeSample(medium, this.OnSampleLoad, this.OnError);
            // Have to size after text is loaded. Bug with highlight box
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.CenterToParent(this.parent);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.buttonWidthManager.Teardown();
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void OnSampleLoad(string sample) {
            this.Dispatcher.Invoke(() => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    this.codeBox.Text = sample;
                });
                if (report.Code != 0) {
                    OnError("", report.Msg);
                }
            });
        }


        private void OnError(string title, string msg) {
            WindowHelpers.ShowMsgTitle(title, msg);
            this.Close();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e) {
            this.codeBox.SelectAll();
            this.codeBox.Copy();
            this.codeBox.Select(0, 0);
        }

    }
}

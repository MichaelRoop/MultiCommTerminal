//using AurelienRibon.Ui.SyntaxHighlightBox.src;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using MultiCommData.Net.UserDisplayData;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using System.Windows;
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
        private CommHelpType medium = CommHelpType.Application;
        private ButtonGroupSizeSyncManager buttonWidthManager = null;


        public MsgBoxCode(Window parent, CommHelpType helpType) {
            this.parent = parent;
            this.medium = helpType;
            InitializeComponent();
            this.Title = DI.Wrapper.GetText(helpType);
            WrapErr.ToErrReport(9999, () => this.Icon = helpType.ResourceWhiteBitmap());
            //this.codeBox.CurrentHighlighter = HighlighterManager.Instance.Highlighters["CPPDL"];
            this.buttonWidthManager = new ButtonGroupSizeSyncManager(this.btnCopy, this.btnExit);
            this.buttonWidthManager.PrepForChange();
        }


        /// <summary>Bind Mouse drag to Template style</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
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

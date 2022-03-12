using BluetoothLE.Net.Enumerations;
using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.UserControls.BLE;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLESelectDataType.xaml</summary>
    public partial class BLESelectDataType : Window {

        #region Data

        Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private List<BLETypeDisplay> lbDataTypes = new ();

        #endregion


        public static void ShowBox(Window parent) {
            try {
                BLESelectDataType win = new (parent);
                win.ShowDialog();
            }
            catch (Exception) { }
        }


        public BLESelectDataType(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.lbDataTypes.Add(new BLETypeDisplay(BLE_DataType.Bool));
            this.lbDataTypes.Add(new BLETypeDisplay(BLE_DataType.UInt_8bit));
            this.lbDataTypes.Add(new BLETypeDisplay(BLE_DataType.UInt_16bit));
            this.lbDataTypes.Add(new BLETypeDisplay(BLE_DataType.UInt_32bit));
            this.cbDataTypes.ItemsSource = this.lbDataTypes;
            this.cbDataTypes.SelectedIndex = 0;
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnOk);
            this.widthManager.PrepForChange();
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
            if (this.widthManager != null) {
                this.widthManager.Teardown();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {
            var dt = this.cbDataTypes.SelectedItem as BLETypeDisplay;
            if (dt != null) {
                BLECommandsEdit.ShowBox(this, dt.DataType);
            }
            else {
                App.ShowMsg(DI.Wrapper.GetText(MsgCode.NothingSelected));
            }
            this.Close();
        }

    }
}

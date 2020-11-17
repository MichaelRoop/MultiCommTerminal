using BluetoothLE.Net.DataModels;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for DeviceInfo_BLESerial.xaml    /// </summary>
    public partial class DeviceInfo_BLESerial : Window {

        Window parent = null;
        //private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        public DeviceInfo_BLESerial(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.borderInput.MouseLeftButtonDown += this.BorderInput_MouseLeftButtonDown;
            this.borderOutput.MouseLeftButtonDown += this.BorderOutput_MouseLeftButtonDown;
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnExit, this.btnSave);
            this.widthManager.PrepForChange();

            #region Test data for info display
            // ---------------------------------------------
            BLE_ServiceDataModel service1 = new BLE_ServiceDataModel();
            service1.DisplayName = "Hogwarts 1 service";
            service1.Characteristics.Add("1", new BLE_CharacteristicDataModel());
            service1.Characteristics["1"].CharName = "George Characteristic";
            service1.Characteristics["1"].Descriptors.Add("1", new BLE_DescriptorDataModel());
            service1.Characteristics["1"].Descriptors["1"].DisplayName = "Output descriptor";
            service1.Characteristics.Add("2", new BLE_CharacteristicDataModel());
            service1.Characteristics["2"].CharName = "Fred Characteristic";
            service1.Characteristics["2"].Descriptors.Add("1", new BLE_DescriptorDataModel());
            service1.Characteristics["2"].Descriptors["1"].DisplayName = "Input descriptor";
            service1.Characteristics["2"].Descriptors.Add("2", new BLE_DescriptorDataModel());
            service1.Characteristics["2"].Descriptors["2"].DisplayName = "Name of stuff";
            this.treeDict.Add("1", service1);
            // ---------------------------------------------

            // ---------------------------------------------
            BLE_ServiceDataModel service2 = new BLE_ServiceDataModel();
            service2.DisplayName = "Hogwarts 2 service";
            service2.Characteristics.Add("1", new BLE_CharacteristicDataModel());
            service2.Characteristics["1"].CharName = "Hermioni characteristic";
            service2.Characteristics.Add("2", new BLE_CharacteristicDataModel());
            service2.Characteristics["2"].CharName = "Ginny characteristic";
            this.treeDict.Add("2", service2);

            // Just pass list of Values to avoid headach in XAML    
            this.treeServices.ItemsSource = this.treeDict.Values;
            #endregion

            // Set the input as the active
            this.borderInput.BorderThickness = new Thickness(2);
        }
        ServiceTreeDict treeDict = new ServiceTreeDict();

        BluetoothLEDeviceInfo info = null;

        public DeviceInfo_BLESerial(Window parent, BluetoothLEDeviceInfo info) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.borderInput.MouseLeftButtonDown += this.BorderInput_MouseLeftButtonDown;
            this.borderOutput.MouseLeftButtonDown += this.BorderOutput_MouseLeftButtonDown;
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnExit, this.btnSave);
            this.widthManager.PrepForChange();

            this.info = info;
            // TODO - check if any previous selections
            this.treeServices.ItemsSource = this.info.Services.Values;
        }



        /// <summary>Bind Mouse drag to Template style</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.CenterToParent(this.parent);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }


        private void BorderOutput_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.borderInput.BorderThickness = new Thickness(0);
            this.borderOutput.BorderThickness = new Thickness(2);
            //this.treeServices. TODDO unselect all
        }

        private void BorderInput_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.borderInput.BorderThickness = new Thickness(2);
            this.borderOutput.BorderThickness = new Thickness(0);
            //this.treeServices. TODDO unselect all
        }


        private void btnSave_Click(object sender, RoutedEventArgs e) {
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void treeServices_Selected(object sender, RoutedEventArgs e) {
            if (sender is TreeView) {
                TreeView tv = sender as TreeView;
                if (tv.SelectedItem is BLE_CharacteristicDataModel) {
                    BLE_CharacteristicDataModel ch = tv.SelectedItem as BLE_CharacteristicDataModel;
                    if (this.borderInput.BorderThickness.Top > 0) {
                        // TODO - save the input selection
                        this.labelInputToDevice.Content = ch.CharName;
                    }
                    else {
                        // TODO - save the output selection
                        this.labelOutputFromDevice.Content = ch.CharName;
                    }
                }
                else {
                    if (e.OriginalSource is TreeViewItem) {
                        TreeViewItem tvi = e.OriginalSource as TreeViewItem;
                        tvi.IsSelected = false;
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
//189

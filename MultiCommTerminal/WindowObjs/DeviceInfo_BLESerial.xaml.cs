using BluetoothLE.Net.DataModels;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
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

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for DeviceInfo_BLESerial.xaml    /// </summary>
    public partial class DeviceInfo_BLESerial : Window {

        Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private ButtonGroupSizeSyncManager widthManager2 = null;

        public DeviceInfo_BLESerial(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.borderCharacteristicSelect.Visibility = Visibility.Collapsed;
            this.borderInput.MouseLeftButtonDown += this.BorderInput_MouseLeftButtonDown;
            this.borderOutput.MouseLeftButtonDown += this.BorderOutput_MouseLeftButtonDown;
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSelect);
            this.widthManager.PrepForChange();
            this.widthManager2 = new ButtonGroupSizeSyncManager(this.btnExit, this.btnSave);
            this.widthManager2.PrepForChange();



            //BLE_ServiceDataModel dm = new BLE_ServiceDataModel();

            //this.dev = new BluetoothLEDeviceInfo();
            //this.treeServices.ItemsSource = this.dev.ServiceProperties;

            //// Descriptors
            //Dictionary<string, BLE_DescriptorDataModel> descriptors = new Dictionary<string, BLE_DescriptorDataModel>();
            //descriptors.Add("1", new BLE_DescriptorDataModel() {
            //    DisplayName = "Input from device"
            //});

            //Dictionary<string, BLE_CharacteristicDataModel> ch1 = new Dictionary<string, BLE_CharacteristicDataModel>();
            //BLE_DescriptorDataModel desc0 = new BLE_DescriptorDataModel() {
            //    DisplayName = "Data input to device",
            //};

            //BLE_CharacteristicDataModel dm1 = new BLE_CharacteristicDataModel() {
            //    CharName = "george characteristic", 
            //    Descriptors = descriptors,
            //};
            //ch1.Add("1",dm1);


            //ch1.Add("2", new BLE_CharacteristicDataModel() {
            //    CharName = "fred characteristic"
            //});
            //this.tree.Add(new BLE_ServiceDataModel() {
            //    DisplayName = "Hogwarts 1 service",
            //    Characteristics = ch1,
            //});

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

        }
        //SevicesTree tree = new SevicesTree();
        ServiceTreeDict treeDict = new ServiceTreeDict();


            //BluetoothLEDeviceInfo dev;


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
            this.widthManager2.Teardown();
        }

        #region Characteristic controls

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.ToggleControlSet();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            this.ToggleControlSet();
        }

        #endregion

        private void BorderOutput_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            // TODO - set label IF VISIBLE
            this.ToggleControlSet();
            //this.lblInputOutputType.Content = "Input";
            this.borderOutput.BorderThickness = new Thickness(2);
        }

        private void BorderInput_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.ToggleControlSet();
            // TODO - set label IF VISIBLE
            //this.lblInputOutputType.Content = "Output";
            this.borderInput.BorderThickness = new Thickness(2);
        }




        private void btnSave_Click(object sender, RoutedEventArgs e) {
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void ToggleControlSet() {
            this.Toggle(this.borderCharacteristicSelect);
            this.Toggle(this.borderMainButtons);
            this.ToggleEvents();
        }

        private void Toggle(Border control) {
            control.Visibility = (control.Visibility == Visibility.Visible)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }


        private void ToggleEvents() {
            if (this.borderCharacteristicSelect.Visibility == Visibility.Visible) {
                this.borderInput.MouseLeftButtonDown -= this.BorderInput_MouseLeftButtonDown;
                this.borderOutput.MouseLeftButtonDown -= this.BorderOutput_MouseLeftButtonDown;
            }
            else {
                this.borderInput.MouseLeftButtonDown += this.BorderInput_MouseLeftButtonDown;
                this.borderOutput.MouseLeftButtonDown += this.BorderOutput_MouseLeftButtonDown;

                this.borderOutput.BorderThickness = new Thickness(0);
                this.borderInput.BorderThickness = new Thickness(0);

            }
        }

        private void treeServices_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            // TODO REMOVE
        }

        private void treeServices_Selected(object sender, RoutedEventArgs e) {
            if (sender is TreeView) {
                TreeView tv = sender as TreeView;
                if (tv.SelectedItem is BLE_CharacteristicDataModel) {
                    BLE_CharacteristicDataModel ch = tv.SelectedItem as BLE_CharacteristicDataModel;
                    if (this.borderInput.BorderThickness.Top > 0) {
                        // TODO - save the other info
                        this.labelInputToDevice.Content = ch.CharName;
                    }
                    else {
                        // TODO - save the other info
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

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



            Dictionary<string, BLE_CharacteristicDataModel> ch1 = new Dictionary<string, BLE_CharacteristicDataModel>();
            ch1.Add("1", new BLE_CharacteristicDataModel() {
                CharName = "george characteristic"
            });
            ch1.Add("2", new BLE_CharacteristicDataModel() {
                CharName = "fred characteristic"
            });
            this.tree.Add(new BLE_ServiceDataModel() {
                DisplayName = "Hogwarts 1 service",
                Characteristics = ch1,
            });


            Dictionary<string, BLE_CharacteristicDataModel> ch2 = new Dictionary<string, BLE_CharacteristicDataModel>();
            ch2.Add("1", new BLE_CharacteristicDataModel() {
                CharName = "Hermioni characteristic"
            });
            ch2.Add("2", new BLE_CharacteristicDataModel() {
                CharName = "Ginny characteristic"
            });
            this.tree.Add(new BLE_ServiceDataModel() {
                DisplayName = "Hogwarts 2 service",
                Characteristics = ch2,
            });

            #region As list characteristics works
            //List<BLE_CharacteristicDataModel> ch1 = new List<BLE_CharacteristicDataModel>();
            //ch1.Add(new BLE_CharacteristicDataModel() {
            //    CharName = "george characteristic"
            //});
            //ch1.Add(new BLE_CharacteristicDataModel() {
            //    CharName = "fred characteristic"
            //});
            //this.tree.Add(new BLE_ServiceDataModel() {
            //    DisplayName = "Hogwarts 1 service",
            //    tstListChar = ch1,
            //});


            //List<BLE_CharacteristicDataModel> ch2 = new List<BLE_CharacteristicDataModel>();
            //ch2.Add(new BLE_CharacteristicDataModel() {
            //    CharName = "Hermioni characteristic"
            //});
            //ch2.Add(new BLE_CharacteristicDataModel() {
            //    CharName = "Ginny characteristic"
            //});
            //this.tree.Add(new BLE_ServiceDataModel() {
            //    DisplayName = "Hogwarts 2 service",
            //    tstListChar = ch2,
            //});
            #endregion


            this.treeServices.ItemsSource = this.tree;

        }
        SevicesTree tree = new SevicesTree();

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
    }
}

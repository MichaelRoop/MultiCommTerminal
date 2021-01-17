using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.Enumerations;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLE_Full.xaml</summary>
    public partial class BLE_Full : Window {

        #region Data

        private ClassLog log = new ClassLog("BLE_Full");
        private Window parent = null;
        private ButtonGroupSizeSyncManager buttonSizer = null;
        private ScrollViewer logScroll = null;
        BluetoothLEDeviceInfo currentDevice = null;
        public static int Instances { get; private set; } 
        bool isBusy = false;

        #endregion

        #region Properties

        public bool IsBusy {
            get { lock (this) { return this.isBusy; } }
            set {
                lock (this) {
                    this.isBusy = value;
                    this.Dispatcher.Invoke(() => {
                        if (this.isBusy) {
                            this.gridWait.Show();
                        }
                        else {
                            this.gridWait.Collapse();
                        }
                    });
                }
            }
        }

        #endregion

        #region Constructors and Window events

        public BLE_Full(Window parent) {
            Instances++;
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.buttonSizer = new ButtonGroupSizeSyncManager(
                this.btnConnect, this.btnExit, this.btnLog);
            this.buttonSizer.PrepForChange();
        }

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.logScroll = this.lbLog.GetScrollViewer();
            this.lbLog.Collapse();

            this.AddEventHandlers();
            DI.Wrapper.CurrentSupportedLanguage(this.SetLanguage);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.RemoveEventHandlers();
            this.buttonSizer.Teardown();
            Instances--;
        }

        #endregion


        private void btnSend_Click(object sender, RoutedEventArgs e) {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnLog_Click(object sender, RoutedEventArgs e) {
            this.lbLog.ToggleVisibility();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            this.SetConnectState(false);
            this.treeServices.ItemsSource = null;
            this.currentDevice = null;
            var device = BLESelect.ShowBox(this.parent, true);
            if (device != null) {
                this.IsBusy = true;
                DI.Wrapper.BLE_DeviceConnectResult += this.DeviceConnectResultHandler;
                DI.Wrapper.BLE_ConnectAsync(device);
            }
        }


        private void btnCopyLog_Click(object sender, RoutedEventArgs e) {

        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e) {

        }



        #region Event handlers

        private void AddEventHandlers() {
            DI.Wrapper.LanguageChanged += this.languageChangedHandler;
            App.STATIC_APP.LogMsgEvent += this.AppLogMsgEventHandler;
        }


        private void RemoveEventHandlers() {
            DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
            App.STATIC_APP.LogMsgEvent -= this.AppLogMsgEventHandler;
            DI.Wrapper.BLE_DeviceConnectResult -= this.DeviceConnectResultHandler;
        }

        private void languageChangedHandler(object sender, SupportedLanguage language) {
            this.SetLanguage(language);
        }


        private void AppLogMsgEventHandler(object sender, string msg) {
            // Race condition with messages coming before window rendered
            try {
                lock (this.lbLog) {
                    if (this.logScroll != null) {
                        this.lbLog.AddAndScroll(msg, this.logScroll, 400);
                    }
                }
            }
            catch (Exception) { }
        }


        private void CharacteristicDataChangeHandler() {
            if (this.treeServices != null && this.treeServices.ItemsSource != null && this.treeServices.Items != null) {
                this.treeServices.Items.Refresh();
                this.ExpandTree();
            }
        }


        private void DeviceConnectResultHandler(object sender, BLEGetInfoStatus info) {
            this.Dispatcher.Invoke(() => {
                this.IsBusy = false;
                DI.Wrapper.BLE_DeviceConnectResult -= this.DeviceConnectResultHandler;
                switch (info.Status) {
                    case BLEOperationStatus.Failed:
                    case BLEOperationStatus.UnhandledError:
                    case BLEOperationStatus.UnknownError:
                    case BLEOperationStatus.NotFound:
                    case BLEOperationStatus.NoServices:
                    case BLEOperationStatus.GetServicesFailed:
                        App.ShowMsg(info.Message);
                        break;
                    case BLEOperationStatus.Success:
                        this.SetConnectState(true);
                        this.currentDevice = info.DeviceInfo;
                        this.Title = string.Format("(BLE) {0}", this.currentDevice.Name);
                        this.treeServices.ItemsSource = this.currentDevice.Services;
                        break;
                }
            });
        }

        #endregion

        #region Private

        private void SetConnectState(bool isConnected) {
            this.Dispatcher.Invoke(() => {
                if (isConnected) {
                    this.connectedOff.Collapse();
                    this.connectedOn.Show();
                }
                else {
                    this.connectedOn.Collapse();
                    this.connectedOff.Show();
                }
            });
        }


        private void SetLanguage(SupportedLanguage l) {
            this.btnSend.Content = l.GetText(MsgCode.send);
            this.btnConnect.Content = l.GetText(MsgCode.connect);
            this.btnLog.Content = l.GetText(MsgCode.Log);
            this.btnExit.Content = l.GetText(MsgCode.exit);
        }


        private void ExpandTree() {

            // This will expand the characteristic level so that we can see the updated data.
            foreach (object o in this.treeServices.Items) {
                TreeViewItem item = this.treeServices.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
                if (item != null) {
                    //this.ExpandAll(item, true);
                    item.IsExpanded = true;

                    // The this.ExpandAll(item, true) doesn't work but this 
                    // will expand characteristics to show descriptors
                    //item.ExpandSubtree();
                }
            }
        }

        #endregion

        #region THIS_DOES_NOT_YET_WORK

#if BLAH_BLAH_BLAH
                private static TreeViewItem ContainerFromItem(ItemContainerGenerator containerGenerator, object item) {
            TreeViewItem container = (TreeViewItem)containerGenerator.ContainerFromItem(item);
            if (container != null) {
                return container;
            }

            foreach (object childItem in containerGenerator.Items) {
                TreeViewItem parent = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                if (parent == null) {
                    continue;
                }

                container = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (container != null) {
                    return container;
                }

                container = ContainerFromItem(parent.ItemContainerGenerator, item);
                if (container != null) {
                    return container;
                }
            }
            return null;
        }




        ///////////////////////////

        private void Expand3() {
            foreach (var service in this.treeServices.Items) {
                BLE_ServiceDataModel s = service as BLE_ServiceDataModel;
                if (s!= null) {
                    TreeViewItem tviS = treeServices.ItemContainerGenerator.ContainerFromItem(s) as TreeViewItem;
                    if (tviS != null) {
                        // This expands the characteristics held in the service tree object. A List
                        tviS.IsExpanded = true;
                        foreach (object xxx in tviS.Items) {
                            LogUtils.Net.Log.Info("----------------", "------------------", () => string.Format("Type name:{0}", xxx.GetType().Name));

                        }




                        //foreach(var ch in s.Characteristics) {
                        //    TreeViewItem tviC = tviS.ItemContainerGenerator.ContainerFromItem(ch.Value) as TreeViewItem;
                        //    if (tviC != null) {
                        //        tviC.IsExpanded = true;
                        //        foreach (var d in ch.Value.Descriptors) {
                        //            TreeViewItem tviD = tviC.ItemContainerGenerator.ContainerFromItem(d.Value) as TreeViewItem;
                        //            if (tviD != null) {
                        //                tviD.IsExpanded = true;
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
            }
        }



        private void Expand2() {
            //var tvi = FindTviFromObjectRecursive(this.treeServices, )
        }






        //https://www.xspdf.com/help/52475752.html
        public static TreeViewItem FindTviFromObjectRecursive(ItemsControl ic, object o) {
            //Search for the object model in first level children (recursively)
            TreeViewItem tvi = ic.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
            if (tvi != null) {
                return tvi;
            }
            //Loop through user object models
            foreach (object i in ic.Items) {
                //Get the TreeViewItem associated with the iterated object model
                TreeViewItem tvi2 = ic.ItemContainerGenerator.ContainerFromItem(i) as TreeViewItem;
                tvi = FindTviFromObjectRecursive(tvi2, o);
                if (tvi != null) {
                    return tvi;
                }
            }
            return null;
        }


        //////////////////////////
        ///

#endif


        #endregion

    }


#if TEST_CODE_TO_WRITE_TO_CHARACTERISTIC_AND_UPDATE
    public static class TreeViewExtensions {

        public static void ExpandAll(this TreeViewItem treeViewItem, bool isExpanded = true) {
            try {
                var stack = new Stack<TreeViewItem>(treeViewItem.Items.Cast<TreeViewItem>());
                while (stack.Count > 0) {
                    TreeViewItem item = stack.Pop();

                    foreach (var child in item.Items) {
                        var childContainer = child as TreeViewItem;
                        if (childContainer == null) {
                            childContainer = item.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
                        }

                        stack.Push(childContainer);
                    }

                    item.IsExpanded = isExpanded;
                }
            }
            catch (System.Exception) {
            }
        }


        public static void CollapseAll(this TreeViewItem treeViewItem) {
            treeViewItem.ExpandAll(false);
        }

    }
#endif


}

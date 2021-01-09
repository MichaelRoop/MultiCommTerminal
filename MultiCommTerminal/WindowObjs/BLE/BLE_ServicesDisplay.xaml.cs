using BluetoothLE.Net.DataModels;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLE_ServicesDisplay.xaml</summary>
    public partial class BLE_ServicesDisplay : Window {

        Window parent = null;

        public static void ShowBox(Window parent, BluetoothLEDeviceInfo info) {
            BLE_ServicesDisplay win = new BLE_ServicesDisplay(parent, info);
            win.ShowDialog();
        }


        public BLE_ServicesDisplay(Window parent, BluetoothLEDeviceInfo info) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.treeServices.ItemsSource = info.Services.Values;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.CenterToParent(this.parent);
        }

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        #region Debug code for dev

        ServiceTreeDict treeDict = new ServiceTreeDict();
        private void CreateDebugTree() {
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

        #endregion

#if TEST_CODE_TO_WRITE_TO_CHARACTERISTIC_AND_UPDATE

        bool blopped = false;

        private void btnTest_Click(object sender, RoutedEventArgs e) {
            string sVal = blopped ? "BLIP" : "BLOP";
            blopped = !blopped;
            foreach (var sKey in this.info.Services.Keys) {
                var s = info.Services[sKey];
                foreach (var cKey in s.Characteristics.Keys) {
                    var c = s.Characteristics[cKey];
                    c.CharValue = sVal;
                }
            }

            this.treeServices.Items.Refresh();
            this.Expand();
            //this.Expand3();
        }



        /// <summary>
        /// https://www.wpf-tutorial.com/treeview-control/treeview-data-binding-multiple-templates/
        /// </summary>
        /// <remarks>
        /// //https://stackoverflow.com/questions/834081/wpf-treeview-where-is-the-expandall-method
        /// </remarks>
        /// <param name="items"></param>
        /// <param name="expand"></param>
        private void ExpandAll(ItemsControl items, bool expand) {
            LogUtils.Net.Log.Info("----------------", "------------------", "ExpandAll");

            ItemContainerGenerator g = this.treeServices.ItemContainerGenerator;

            // DOES NOT WORK
            foreach (var obj in items.Items) {

                //ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                ItemsControl childControl = g.ContainerFromItem(obj) as ItemsControl;

                if (childControl != null) {
                    ExpandAll(childControl, expand);
                    TreeViewItem item = childControl as TreeViewItem;
                    if (item != null) {
                        item.IsExpanded = true;
                    }
                }
                else {
                    // Failed as an items control
                    //TreeViewItem item = items.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItem;
                    TreeViewItem item = g.ContainerFromItem(obj) as TreeViewItem;
                    if (item != null) {
                        item.IsExpanded = true;
                    }
                    else {
                        item = obj as TreeViewItem;
                        if (item != null) {
                            item.IsExpanded = true;
                        }
                    }

                }

                //TreeViewItem item = childControl as TreeViewItem;
                //if (item != null) {
                //    item.IsExpanded = true;
                //}
                //else {
                //    // nope
                //    //item = obj as TreeViewItem;
                //    //item = this.treeServices.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItem;
                //    item = items.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItem;
                //    if (item != null) {
                //        item.IsExpanded = true;
                //    }
                //}

                // None of these work for the lowest level of the tree

            }
        }

        // TODO - figure out if I can detect if expanded before

        private void Expand() {

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

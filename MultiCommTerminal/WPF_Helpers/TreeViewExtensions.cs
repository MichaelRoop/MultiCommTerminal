using BluetoothLE.Net.DataModels;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MultiCommTerminal.NetCore.WPF_Helpers {

    /// <summary>Extensions for the TreeView</summary>
    /// <remarks>
    /// https://social.msdn.microsoft.com/Forums/en-US/a2988ae8-e7b8-4a62-a34f-b851aaf13886/windows-presentation-foundation-faq?forum=wpf#expand_treeview
    /// </remarks>
    public static class TreeViewExtensions {

        private static ClassLog log = new ClassLog("TreeViewExtensions");

        public static void RefreshAndExpand(this TreeView treeView) {
            List<string> expanded = new List<string>();
            GetExpandedSubHeaders(treeView, expanded);
            treeView.Items.Refresh();
            ExpandSelectedSubHeaders(treeView, expanded);
        }


        // TODO replace with IUniqueIdentifiable
        private static string GetGuid(object o) {
            if (o is BLE_ServiceDataModel) {
                return (o as BLE_ServiceDataModel).Uuid.ToString();
            }
            else if (o is BLE_CharacteristicDataModel) {
                return (o as BLE_CharacteristicDataModel).Uuid.ToString();
            }
            else if (o is BLE_DescriptorDataModel) {
                return (o as BLE_DescriptorDataModel).Uuid.ToString();
            }
            return Guid.NewGuid().ToString();
        }



        private static void GetExpandedSubHeaders(ItemsControl parentContainer, List<string> expanded) {
            foreach (Object item in parentContainer.Items) {
                try {
                    TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                    if (currentContainer != null) {
                        if (currentContainer.IsExpanded) {
                            expanded.Add(GetGuid(item));
                        }
                        if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                            // If sub containers is not ready, we need to wait until they are generated.
                            currentContainer.ItemContainerGenerator.StatusChanged += delegate {
                                GetExpandedSubHeaders(currentContainer, expanded);
                            };
                        }
                        else {
                            // If sub containers is ready, directly go to the next iteration to expand
                            GetExpandedSubHeaders(currentContainer, expanded);
                        }
                    }
                }
                catch (Exception e) {
                    log.Exception(1111, "GetExpandedSubHeaders", "", e);
                }
            }
        }

        //https://stackoverflow.com/questions/11540459/create-refresh-a-wpf-treeview-and-remember-expanded-nodes-without-xaml
        private static void ExpandSelectedSubHeaders(ItemsControl parentContainer, List<string> expanded) {
            foreach (Object item in parentContainer.Items) {
                try {
                    TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                    if (currentContainer != null) {
                        string id = GetGuid(item);
                        if (expanded.FirstOrDefault(x => x == id) != null) {
                            currentContainer.IsExpanded = true;
                        }
                        if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                            // If sub containers is not ready, we need to wait until they are generated.
                            currentContainer.ItemContainerGenerator.StatusChanged += delegate {
                                ExpandSelectedSubHeaders(currentContainer, expanded);
                            };
                        }
                        else {
                            // If sub containers is ready, directly go to the next iteration to expand
                            ExpandSelectedSubHeaders(currentContainer, expanded);
                        }
                    }
                }
                catch (Exception e) {
                    log.Exception(2222, "ExpandSelectedSubHeaders", "", e);
                }
            }
        }



        public static void ExpandAll(this TreeView treeView) {
            ExpandSubContainers(treeView);
        }


        private static void ExpandSubContainers(ItemsControl parentContainer) {
            foreach (Object item in parentContainer.Items) {
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (currentContainer != null && currentContainer.Items.Count > 0) {
                    // Expand the current item.
                    currentContainer.IsExpanded = true;
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                        // If the sub containers of current item is not ready, we need to wait until
                        // they are generated.
                        currentContainer.ItemContainerGenerator.StatusChanged += delegate {
                            ExpandSubContainers(currentContainer);
                        };
                    }
                    else {
                        // If the sub containers of current item is ready, we can directly go to the next
                        // iteration to expand them.
                        ExpandSubContainers(currentContainer);
                    }
                }
            }
        }

#if FAILED_EXPERIMENT

        private static void ExpandSelectedSubContainers(ItemsControl parentContainer, Stack<bool> stack) {
            foreach (Object item in parentContainer.Items) {
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (currentContainer != null && currentContainer.Items.Count > 0) {
                    // Expand the current item depending on the queue value
                    //bool expand = false;

                    // The expanded only works if always set true. Think the problem is reading the existing
                    //currentContainer.IsExpanded = true;

                    if (stack.Count > 0) {
                        currentContainer.IsExpanded = stack.Pop();
                    }
                    else {
                        log.Info("ExpandSelectedSubContainers", "********Ran out of stack");
                    }

                    //currentContainer.IsExpanded = expand;
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                        // If the sub containers of current item is not ready, we need to wait until
                        // they are generated.

                        bool isExp = stack.Count > 0 ? stack.Pop() : false;
                        currentContainer.ItemContainerGenerator.StatusChanged += delegate {
                            // This seems to be the problem
                            if (currentContainer.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) {
                                //ExpandSelectedSubContainers(currentContainer, stack);
                                log.Info("ExpandSelectedSubContainers", "----- DELAYED -----");
                                bool exp = isExp;
                                ExpandSelectedSubContainers(currentContainer, stack);
                            }
                        };
                    }
                    else {
                        // If the sub containers of current item is ready, we can directly go to the next
                        // iteration to expand them.
                        ExpandSelectedSubContainers(currentContainer, stack);
                    }
                }
            }
        }



        private static void GetExpandedSubContainers(ItemsControl parentContainer, Stack<bool> stack) {
            foreach (Object item in parentContainer.Items) {
                TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (currentContainer != null && currentContainer.Items.Count > 0) {
                    // Expand the current item.
                    stack.Push(currentContainer.IsExpanded);

                    log.Info("GetExpandedSubContainers", () => string.Format("Adding to stack:{0}", stack.Count));


                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                        // If the sub containers of current item is not ready, we need to wait until
                        // they are generated.
                        currentContainer.ItemContainerGenerator.StatusChanged += delegate {
                            GetExpandedSubContainers(currentContainer, stack);
                            log.Info("GetExpandedSubContainers", "+++++ DELAYED +++++");
                        };
                    }
                    else {
                        // If the sub containers of current item is ready, we can directly go to the next
                        // iteration to expand them.
                        GetExpandedSubContainers(currentContainer, stack);
                    }
                }
            }
        }

#endif


    }
}

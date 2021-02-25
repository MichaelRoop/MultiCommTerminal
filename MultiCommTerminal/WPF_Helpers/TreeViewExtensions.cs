using BluetoothLE.Net.interfaces;
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


        private static void GetExpandedSubHeaders(ItemsControl parentContainer, List<string> expanded) {
            if (parentContainer != null && parentContainer.Items != null) {
                foreach (Object item in parentContainer.Items) {
                    try {
                        TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                        if (currentContainer != null) {
                            if (currentContainer.IsExpanded) {
                                expanded.Add((item as IUniquelyIdentifiable).Uuid.ToString());
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
        }


        //https://stackoverflow.com/questions/11540459/create-refresh-a-wpf-treeview-and-remember-expanded-nodes-without-xaml
        private static void ExpandSelectedSubHeaders(ItemsControl parentContainer, List<string> expanded) {
            if (parentContainer != null && parentContainer.Items != null) {
                foreach (Object item in parentContainer.Items) {
                    try {
                        TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                        if (currentContainer != null) {
                            if (expanded.FirstOrDefault(x => x == (item as IUniquelyIdentifiable).Uuid.ToString()) != null) {
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
        }


        public static void ExpandAll(this TreeView treeView) {
            ExpandSubContainers(treeView);
        }


        private static void ExpandSubContainers(ItemsControl parentContainer) {
            if (parentContainer != null && parentContainer.Items != null) {
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
        }

        #region Selecting previous

        public static List<string> GetSelected(this TreeView treeView) {
            List<string> expanded = new List<string>();
            GetSelectedSubHeaders(treeView, expanded);
            return expanded;
        }

        public static void RestoreSelected(this TreeView treeView, List<string> data) {
            SelectSubHeaders(treeView, data);
        }


        private static void GetSelectedSubHeaders(ItemsControl parentContainer, List<string> expanded) {
            if (parentContainer != null && parentContainer.Items != null) {
                foreach (Object item in parentContainer.Items) {
                    try {
                        TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                        if (currentContainer != null) {
                            if (currentContainer.IsSelected) {
                                expanded.Add((item as IUniquelyIdentifiable).Uuid.ToString());
                            }
                            if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                                // If sub containers is not ready, we need to wait until they are generated.
                                currentContainer.ItemContainerGenerator.StatusChanged += delegate {
                                    GetSelectedSubHeaders(currentContainer, expanded);
                                };
                            }
                            else {
                                // If sub containers is ready, directly go to the next iteration to expand
                                GetSelectedSubHeaders(currentContainer, expanded);
                            }
                        }
                    }
                    catch (Exception e) {
                        log.Exception(1111, "GetExpandedSubHeaders", "", e);
                    }
                }
            }
        }


        private static void SelectSubHeaders(ItemsControl parentContainer, List<string> expanded) {
            if (parentContainer != null && parentContainer.Items != null) {
                foreach (Object item in parentContainer.Items) {
                    try {
                        TreeViewItem currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                        if (currentContainer != null) {
                            if (expanded.FirstOrDefault(x => x == (item as IUniquelyIdentifiable).Uuid.ToString()) != null) {
                                currentContainer.IsSelected = true;
                            }
                            if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                                // If sub containers is not ready, we need to wait until they are generated.
                                currentContainer.ItemContainerGenerator.StatusChanged += delegate {
                                    SelectSubHeaders(currentContainer, expanded);
                                };
                            }
                            else {
                                // If sub containers is ready, directly go to the next iteration to expand
                                SelectSubHeaders(currentContainer, expanded);
                            }
                        }
                    }
                    catch (Exception e) {
                        log.Exception(2222, "ExpandSelectedSubHeaders", "", e);
                    }
                }
            }
        }

        #endregion

    }
}

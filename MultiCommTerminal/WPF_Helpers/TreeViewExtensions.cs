using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MultiCommTerminal.NetCore.WPF_Helpers {

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://social.msdn.microsoft.com/Forums/en-US/a2988ae8-e7b8-4a62-a34f-b851aaf13886/windows-presentation-foundation-faq?forum=wpf#expand_treeview
    /// 
    /// </remarks>
    public static class TreeViewExtensions {

        public static void RefreshAndExpand(this TreeView treeView) {
            treeView.Items.Refresh();
            treeView.ExpandAll();
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

    }
}

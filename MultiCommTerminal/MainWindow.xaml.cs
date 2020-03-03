using MultiCommData.UserDisplayData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiCommTerminal {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private MediumGroup mediumGroup = new MediumGroup();
        private CommMediumType currentMedium = CommMediumType.None;

        public MainWindow() {
            InitializeComponent();
            this.OnStartupSuccess();
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            // Must force the window size down
            this.Width = this.grdMain.ActualWidth;
            this.Height = this.grdMain.ActualHeight + 40; // TODO Weird have to add this
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            // TODO need to disconnect
            this.Close();
        }



        private void OnStartupSuccess() {
            // TODO for now init manually
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Bluetooth", CommMediumType.Bluetooth));
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Ethernet", CommMediumType.Ethernet));
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Wifi", CommMediumType.Wifi));
            this.cbComm.ItemsSource = this.mediumGroup.Mediums;
            this.cbComm.SelectedIndex = 0;
        }

 
        private void cbComm_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //MessageBox.Show(String.Format("Enum selected:{0}",
            //    (this.cbComm.SelectedItem as CommMedialDisplay).MediumType.ToString()));

            // Hide all the options
            this.spBluetooth.Visibility = Visibility.Collapsed;
            this.spEthernet.Visibility = Visibility.Collapsed;
            this.spWifi.Visibility = Visibility.Collapsed;

            switch((this.cbComm.SelectedItem as CommMedialDisplay).MediumType) {
                case CommMediumType.Bluetooth:
                    this.spBluetooth.Visibility = Visibility.Visible;
                    this.btnConnect.Visibility = Visibility.Visible;
                    break;
                case CommMediumType.Ethernet:
                    this.spEthernet.Visibility = Visibility.Visible;
                    this.btnConnect.Visibility = Visibility.Visible;
                    break;
                case CommMediumType.Wifi:
                    this.spWifi.Visibility = Visibility.Visible;
                    this.btnConnect.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        // This will be moved out of UI
        private void DoDisconnect(CommMediumType newMedium) {
            if (this.currentMedium != CommMediumType.None && 
                this.currentMedium != newMedium) {
                // Disconnect whatever we are connected to
                switch (this.currentMedium) {
                    case CommMediumType.Bluetooth:
                        break;
                    case CommMediumType.Ethernet:
                        break;
                    case CommMediumType.Wifi:
                        break;
                    default:
                        break;
                }
                this.currentMedium = CommMediumType.None;
                this.btnConnect.Visibility = Visibility.Collapsed;
            }


        }

 
    }
}

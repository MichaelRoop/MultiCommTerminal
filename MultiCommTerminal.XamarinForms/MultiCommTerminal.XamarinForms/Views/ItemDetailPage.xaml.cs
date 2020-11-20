using MultiCommTerminal.XamarinForms.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.Views {
    public partial class ItemDetailPage : ContentPage {
        public ItemDetailPage() {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
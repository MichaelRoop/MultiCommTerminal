using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MultiCommTerminal.NetCore.UserControls {

    /// <summary>Buttons with image that can be set in templates</summary>
    public class UC_IconButton : Button {

        #region Properties

        /// <summary>Exposes the ImageSource of the icon for template injection</summary>
        /// <remarks>
        /// Usage in XAML
        /// Using a static function to inject the image source string
        /// IconSource="{Binding Source={x:Static wpfHelper:IconBinder.Exit}}"
        /// Using the inmage source string when image is compiled as resource
        /// IconSource="AppName;component;/folderName/folderName/imageName.emageExtension"
        /// </remarks>
        public ImageSource IconSource {
            get {
                return (ImageSource)GetValue(IconSourceProperty);
            }
            set {
                SetValue(IconSourceProperty, value);
            }
        }


        /// <summary>
        /// Exposes border around image to force resize in template using Padding 
        /// </summary>
        /// <remarks>
        /// Usage in XAML - Can use 1, 2 or 4 values in string just like Padding property
        /// IconMargin="5" Same value on all sides
        /// IconMargin="2,4" Width and Height dimensions
        /// IconMargin="2,2,2,2" All sides can be different
        /// </remarks>
        public Thickness IconMargin {
            get {
                return (Thickness)GetValue(IconMarginProperty);
            }
            set {
                SetValue(IconMarginProperty, value);
            }
        }

        #endregion

        #region Constructors

        static UC_IconButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UC_IconButton),
                new FrameworkPropertyMetadata(typeof(UC_IconButton)));
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register(
            "IconMargin",
            typeof(Thickness),
            typeof(UC_IconButton),
            new PropertyMetadata(default(Thickness)));


        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
            "IconSource",
            typeof(ImageSource),
            typeof(UC_IconButton),
            new PropertyMetadata(default(ImageSource)));

        #endregion
    }
}

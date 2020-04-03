using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MultiCommTerminal.UserControls {

    /// <summary>Base class for buttons which will have an image so you can set the values in templates</summary>
    public class UC_IconButton : Button {

        #region Properties

        public ImageSource Img {
            get {
                return (ImageSource)GetValue(ImgProperty);
            }
            set {
                SetValue(ImgProperty, value);
            }
        }


        public double ImgHeight {
            get {
                return (double)GetValue(ImgHeightProperty);
            }
            set {
                SetValue(ImgHeightProperty, value);
            }
        }


        public double ImgWidth {
            get {
                return (double)GetValue(ImgWidthProperty);
            }
            set {
                SetValue(ImgWidthProperty, value);
            }
        }

        public Thickness ImgMargin {
            get {
                return (Thickness)GetValue(ImgMarginProperty);
            }
            set {
                SetValue(ImgMarginProperty, value);
            }
        }

        #endregion

        #region Constructors

        static UC_IconButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UC_IconButton),
                new FrameworkPropertyMetadata(typeof(UC_IconButton)));
        }

        #endregion

        public static readonly DependencyProperty ImgHeightProperty = DependencyProperty.Register(
            "ImgHeight",
            typeof(double),
            typeof(UC_IconButton),
            new PropertyMetadata(default(double)));


        public static readonly DependencyProperty ImgWidthProperty = DependencyProperty.Register(
            "ImgWidth",
            typeof(double),
            typeof(UC_IconButton),
            new PropertyMetadata(default(double)));


        public static readonly DependencyProperty ImgMarginProperty = DependencyProperty.Register(
            "ImgMargin",
            typeof(Thickness),
            typeof(UC_IconButton),
            new PropertyMetadata(default(Thickness)));


        public static readonly DependencyProperty ImgProperty = DependencyProperty.Register(
            "Img",
            typeof(ImageSource),
            typeof(UC_IconButton),
            new PropertyMetadata(default(ImageSource)));

    }
}

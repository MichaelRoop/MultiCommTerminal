using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MultiCommTerminal.NetCore.UserControls {

    public class UC_RoundIconButton : Button {

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


        /// <summary>Exposes border around image to force resize in template using Padding</summary>
        /// <remarks>
        /// Usage in XAML - Can use 1, 2 or 4 values in string
        /// IconPadding="5" Same value on all sides
        /// IconPadding="2,4" Width and Height dimensions
        /// IconPadding="2,2,2,2" All sides can be different
        /// </remarks>
        public Thickness IconPadding {
            get {
                return (Thickness)GetValue(IconPaddingProperty);
            }
            set {
                SetValue(IconPaddingProperty, value);
            }
        }


        public CornerRadius ButtonCornerRadius {
            get {
                return (CornerRadius)GetValue(ButtonCornerRadiusProperty);
            }
            set {
                SetValue(ButtonCornerRadiusProperty, value);
            }
        }

        #endregion

        #region Constructors

        static UC_RoundIconButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UC_RoundIconButton),
                new FrameworkPropertyMetadata(typeof(UC_RoundIconButton)));
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty IconPaddingProperty = DependencyProperty.Register(
            "IconPadding",
            typeof(Thickness),
            typeof(UC_RoundIconButton),
            new PropertyMetadata(default(Thickness)));


        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
            "IconSource",
            typeof(ImageSource),
            typeof(UC_RoundIconButton),
            new PropertyMetadata(default(ImageSource)));


        public static readonly DependencyProperty ButtonCornerRadiusProperty = DependencyProperty.Register(
            "ButtonCornerRadius",
            typeof(CornerRadius),
            typeof(UC_RoundIconButton),
            new PropertyMetadata(default(CornerRadius)));



        #endregion


    }




}

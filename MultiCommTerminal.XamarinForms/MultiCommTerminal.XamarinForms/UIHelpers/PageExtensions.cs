﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.UIHelpers {
    
    public static class PageExtensions {


        public static void OnErr(this ContentPage page, string err) {
            App.ShowError(page, err);
        }


    }
}
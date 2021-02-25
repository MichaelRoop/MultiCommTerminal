using LogUtils.Net;
using System;
using System.Globalization;
using System.Windows.Data;
using VariousUtils.Net;

namespace MultiCommTerminal.NetCore.WPF_Helpers {

    /// <summary>Converts a true false value in a field to a check or x</summary>
    public class BoolConvertToXorCheck : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                return string.Format(" {0} ", this.Convert(value, targetType) ? UTF8Helpers.HeavyCheckMark : UTF8Helpers.HeavyBallotX);
            }
            catch (Exception) {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return false;
        }


        private bool Convert(object value, Type type) {
            try {
                if (type == typeof(bool)) {
                    return (bool)value;
                }
                switch (value.ToString().ToLower()) {
                    case "true":
                        return true;
                    case "false":
                        return false;
                    default:
                        Log.Error(9999, "", "", () => string.Format("Invalid value '{0}'", value.ToString()));
                        return false;
                }
            }
            catch (Exception e) {
                Log.Exception(9999, "BoolConvertToXorCheck", "Convert", "", e);
                return false;
            }

        }

    }

}

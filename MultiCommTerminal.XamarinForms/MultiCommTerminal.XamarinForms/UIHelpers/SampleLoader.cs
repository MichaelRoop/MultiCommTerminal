using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommData.Net.UserDisplayData;
using System;
using System.IO;
using System.Reflection;

namespace MultiCommTerminal.XamarinForms.UIHelpers {


    /// <summary>File located in Xamarin Forms since the resource is in this assembly
    /// 
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/data/files?tabs=windows
    /// </remarks>
    public static class SampleLoader {

        private const string SAMPLES_RES_PREFIX = "MultiCommTerminal.XamarinForms.Samples";

        public static void Load(CommHelpType commHelpType, Action<string> onSuccess, Action<string,string> onErr) {
            try {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
                string filename = string.Format("{0}.{1}", SAMPLES_RES_PREFIX, GetSimpleFilename(commHelpType));
                Stream stream = assembly.GetManifestResourceStream(filename);
                if (stream == null) {
                    Log.Error(9999, "SampleLoader", "Load", () => string.Format("Not found:{0}", filename));
                    onErr.Invoke(App.GetText(MsgCode.NotFound), filename);
                }
                else {
                    using (StreamReader reader = new System.IO.StreamReader(stream)) {
                        string txt = reader.ReadToEnd();
                        if (txt.Length == 0) {
                            onErr.Invoke(App.GetText(MsgCode.NotFound), filename);
                        }
                        else {
                            onSuccess.Invoke(txt);
                        }
                    }
                }
            }
            catch (Exception e) {
                Log.Exception(9999, "GetCodeSampleFromResources", "", e);
                onErr.Invoke(App.GetText(MsgCode.Error), App.GetText(MsgCode.LoadFailed));
            }
        }


        private static string GetSimpleFilename(CommHelpType medium) {
            switch (medium) {
                case CommHelpType.Bluetooth:
                    return "BTSample.txt";
                case CommHelpType.BluetoothLE:
                    return "BLESample.txt";
                case CommHelpType.Wifi:
                    return "WifiSample.txt";
                case CommHelpType.Usb:
                    return "USBSample.txt";
                case CommHelpType.Ethernet:
                    return "EthernetSample.txt";
                default:
                    return "";
            }
        }


    }
}

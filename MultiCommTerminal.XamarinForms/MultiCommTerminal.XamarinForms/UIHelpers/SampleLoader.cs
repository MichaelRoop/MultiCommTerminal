using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommData.Net.UserDisplayData;
using System;
using System.IO;
using System.Reflection;
using VariousUtils.Net;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.UIHelpers {


    /// <summary>File located in Xamarin Forms since the resource is in this assembly
    /// 
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/data/files?tabs=windows
    /// </remarks>
    public static class SampleLoader {

        private const string SAMPLES_RES_PREFIX = "MultiCommTerminal.XamarinForms.Samples";
        private const string USER_MANUAL_NAME = "UserManual.pdf";
        private const string USER_MANUAL_DIR = "Documents";

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


        public static void LoadUserManual(Action<string, string> onErr) {
            string filePath = string.Empty;
            try {
                // Copy over file first time from resources
                string dir = Path.Combine(App.Wrapper.GetDataFilesPath(), USER_MANUAL_DIR);
                DirectoryHelpers.CreateStorageDir(dir);
                filePath = Path.Combine(dir, USER_MANUAL_NAME);
                Log.Info("SampleLoader", "LoadUserManual", () => string.Format("Dir '{0}'", dir));
                Log.Info("SampleLoader", "LoadUserManual", () => string.Format("File '{0}'", filePath));
                if (!File.Exists(filePath)) {
                    var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
                    string filename = string.Format("{0}.{1}", SAMPLES_RES_PREFIX, USER_MANUAL_NAME);
                    Log.Info("SampleLoader", "LoadUserManual", () => string.Format("File does not exist. Creating"));
                    using (Stream inStream = assembly.GetManifestResourceStream(filename)) {
                        Log.Info("SampleLoader", "LoadUserManual", () => string.Format("Loaded from resource"));
                        using (Stream file = File.Create(filePath)) {
                            Log.Info("SampleLoader", "LoadUserManual", () => string.Format("File created"));

                            // copy 8k at a time
                            byte[] buff = new byte[8192];
                            int len = 0;
                            while ((len = inStream.Read(buff, 0, buff.Length)) > 0) {
                                Log.Info("SampleLoader", "LoadUserManual", () => string.Format("."));
                                file.Write(buff, 0, len);
                            }
                        }
                    }
                }

                if (File.Exists(filePath)) {
                    Device.BeginInvokeOnMainThread(async () => {
                        try {
                            await Launcher.OpenAsync(new OpenFileRequest() {
                                File = new ReadOnlyFile(filePath)
                            });
                        }
                        catch(Exception e) {
                            onErr(App.GetText(MsgCode.LoadFailed), filePath);
                        }
                    });
                }
                else {
                    Log.Error(9999, "SampleLoader", "LoadUserManual", () => string.Format("Did not find file"));
                    onErr.Invoke(App.GetText(MsgCode.NotFound), filePath);
                }
            }
            catch(Exception e) {
                Log.Exception(9999, "SampleLoader", "LoadUserManual", "", e);
                onErr.Invoke(App.GetText(MsgCode.LoadFailed), filePath);
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

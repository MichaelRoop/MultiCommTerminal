using BluetoothCommon.Net.interfaces;
using CommunicationStack.Net.interfaces;
using IconFactory.interfaces;
using LanguageFactory.interfaces;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using VariousUtils;

namespace MultiCommWrapper.Net.WrapCode {


    public partial class CommWrapper : ICommWrapper {

        #region Data

        IStorageManagerFactory storageFactory = null;
        ILangFactory languages = null;
        IIconFactory iconFactory = null;
        IBTInterface classicBluetooth = null;
        IBLETInterface bleBluetooth = null;
        IStorageManager<SettingItems> settings = null;
        ICommStackLevel0 BtClassicStack = null;

        private ClassLog log = new ClassLog("CommWrapper");

        #endregion

        #region Constructors

        /// <summary>Code logic and error handling here rather than UI</summary>
        /// <remarks>This makes less work when adding extra OS UIs</remarks>
        /// <param name="storageFactory"></param>
        /// <param name="languages"></param>
        /// <param name="iconFactory"></param>
        /// <param name="classicBluetooth"></param>
        /// <param name="bleBluetooth"></param>
        public CommWrapper(
            IStorageManagerFactory storageFactory,
            ILangFactory languages,
            IIconFactory iconFactory,
            IBTInterface classicBluetooth,
            ICommStackLevel0 classicBluetoothStack,
            IBLETInterface bleBluetooth ) {

            this.storageFactory = storageFactory;
            this.languages = languages;
            this.iconFactory = iconFactory;
            this.classicBluetooth = classicBluetooth;
            this.bleBluetooth = bleBluetooth;
            this.BtClassicStack = classicBluetoothStack;
            this.InitializeAll();
        }

        #endregion

        #region Init and teardown

        public void Teardown() {
            // TODO - shut down anything needed and dispose
            this.TeardownLanguages();
            this.TeardownBluetoothClassic();
            this.BLE_TearDown();
        }


        /// <summary>Make sure there are default files for all storage managers</summary>
        private void InitializeAll() {
            this.InitLanguages();
            this.InitStorage();
            this.InitBluetoothClassic();
            this.BLE_Init();
        }

        #endregion

    }
}

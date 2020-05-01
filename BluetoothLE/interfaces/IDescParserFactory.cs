using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothLE.Net.interfaces {

    public interface IDescParserFactory {

        IDescParser GetParser(Guid descriptorUuid);

        string GetParsedValueAsString(Guid descriptorUuid, byte[] value);


    }
}

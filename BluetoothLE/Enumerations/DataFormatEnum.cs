using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothLE.Net.Enumerations {


    /// <summary>Used in the format presenter descriptor
    /// 
    /// </summary>
    public enum DataFormatEnum : byte {
        Reserved = 0,
        Boolean = 1,
        Unsigned_2bit_integer = 2,
        unsigned_4bit_integer = 3,
        unsigned_8bit_integer = 4,
        unsigned_12bit_integer = 5,
        unsigned_16bit_integer = 6,
        unsigned_24bit_integer = 7,
        unsigned_32bit_integer = 8,
        unsigned_48bit_integer = 9,
        unsigned_64bit_integer = 10,
        unsigned_128bit_integer = 11,
        signed_8bit_integer = 12,
        signed_12bit_integer = 13,
        signed_16bit_integer = 14,
        signed_24bit_integer = 15,
        signed_32bit_integer = 16,
        signed_48bit_integer = 17,
        signed_64bit_integer = 18,
        signed_128bit_integer = 19,
        IEEE_754_32bit_floating_point = 20,
        IEEE_754_64bit_floating_point = 21,
        IEEE_11073_16bit_SFLOAT = 22,
        IEEE_11073_32bit_FLOAT = 23,
        IEEE_20601_format = 24,
        UTF8_String = 25,
        UTF16_String = 26,
        OpaqueStructure = 27,

        // My own enum to 
        Unhandled = 255,
    }
}

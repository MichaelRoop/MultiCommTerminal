using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothLE.Net.interfaces {
    

    /// <summary>Common properties and methods of Descriptor value parsers</summary>
    public interface IDescParser {

        /// <summary>Contains variable number of bytes from reading the descriptor</summary>
        byte[] RawData { get; }


        /// <summary>
        /// The type of the implementation class which have extra properties
        /// based on the types of data derived from the raw bytes
        /// </summary>
        Type ImplementationType { get; }


        /// <summary>Parse out the variable values from the read bytes</summary>
        /// <param name="data">The bytes from Descriptor read</param>
        /// <returns>Display string with the parsed data</returns>
        string Parse(byte[] data);


        /// <summary>User friendly display of descriptor value(s)</summary>
        /// <returns></returns>
        string DisplayString();

    }
}

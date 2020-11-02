using LogUtils.Net;
using System;
using System.Collections.Generic;

namespace MultiCommData.Net.Helpers {

    /// <summary>
    /// Generate user freindly name for USB vendor and Products
    /// for a limited set of products.  Add as required
    /// </summary>
    public class UsbDisplayFactory {

        private static Dictionary<ushort, string> vendors = null;
        private static Dictionary<ushort, Dictionary<ushort, string>> products;
        private static ushort ARDUINO_ID = 0x2341;
        private static ushort ATMEL_ID = 0x3EB;

        public static Tuple<string,string> Get(ushort vendeorId, ushort productId) {
            if (vendors == null) {
                Init();
            }
            try {
                string v = vendors.ContainsKey(vendeorId)
                    ? vendors[vendeorId]
                    : string.Format("0x{0:X}", vendeorId);

                string p = string.Format("0x{0:X}", productId);
                if (products.ContainsKey(vendeorId)) {
                    var pr = products[vendeorId];
                    if (pr.ContainsKey(productId)) {
                        p = pr[productId];
                    }
                }
                return new Tuple<string, string>(v, p);
            }
            catch(Exception e) {
                Log.Exception(9999, "", e);
                return new Tuple<string, string>(
                    string.Format("0x{0:X}", vendeorId),
                    string.Format("0x{0:X}", productId));
            }
        }


        private static void Init() {
            vendors = new Dictionary<ushort, string>();
            vendors.Add(ARDUINO_ID, "Arduino");
            vendors.Add(ATMEL_ID, "Atmel");

            products = new Dictionary<ushort, Dictionary<ushort, string>>();
            BuildArduinoProducts();
            BuildAtmelProducts();
        }

        private static void BuildArduinoProducts() {
            Dictionary<ushort, string> tmp = new Dictionary<ushort, string>();
            tmp.Add(0x8036, "Leonardo(CDC ACM, HID)");
            tmp.Add(0x8038, "Robot Control Board(CDC ACM, HID)");
            tmp.Add(0x8039, "Robot Motor Board(CDC ACM, HID)");
            tmp.Add(0x003F, "Mega ADK(CDC ACM)");
            tmp.Add(0x0042, "Mega 2560 R3(CDC ACM)");
            tmp.Add(0x0043, "Uno R3(CDC ACM)");
            tmp.Add(0x0044, "Mega ADK R3(CDC ACM)");
            tmp.Add(0x0045, "Serial R3(CDC ACM)");
            tmp.Add(0x0049, "ISP");
            tmp.Add(0x0001, "Uno(CDC ACM)");
            tmp.Add(0x0010, "Mega 2560(CDC ACM)");
            tmp.Add(0x0036, "Leonardo Bootloader");
            tmp.Add(0x003B, "Serial Adapter(CDC ACM)");
            tmp.Add(0x003D, "Due Programming Port");
            tmp.Add(0x003E, "Due");
            products.Add(ARDUINO_ID, tmp);
        }


        private static void BuildAtmelProducts() {
            Dictionary<ushort, string> tmp = new Dictionary<ushort, string>();

            tmp.Add(0x7617, "AT76C505AS Wireless Adapter");
            tmp.Add(0x7800, "Mini Album");
            tmp.Add(0x800C, "Airspy HF +");
            tmp.Add(0xFF01, "WootingOne");
            tmp.Add(0xFF02, "WootingTwo");
            tmp.Add(0xFF07, "Tux Droid fish dongle");
            tmp.Add(0x6200, "AT91SAM HID Mouse Demo Application");
            tmp.Add(0x7603, "D - Link DWL - 120 802.11b Wireless Adapter[Atmel at76c503a]");
            tmp.Add(0x7604, "at76c503a 802.11b Adapter");
            tmp.Add(0x7605, "at76c503a 802.11b Adapter");
            tmp.Add(0x7606, "at76c505 802.11b Adapter");
            tmp.Add(0x7611, "at76c510 rfmd2948 802.11b Access Point");
            tmp.Add(0x7613, "WL - 1130 USB");
            tmp.Add(0x7614, "AT76c505a Wireless Adapter");
            tmp.Add(0x7615, "AT76C505AMX Wireless Adapter");
            tmp.Add(0x3312, "4 - Port Hub");
            tmp.Add(0x4102, "AirVast W-Buddie WN210");
            tmp.Add(0x5601, "at76c510 Prism-II 802.11b Access Point");
            tmp.Add(0x5603, "Cisco 7920 WiFi IP Phone");
            tmp.Add(0x6119, "AT91SAM CDC Demo Application");
            tmp.Add(0x6124, "at91sam SAMBA bootloader");
            tmp.Add(0x6127, "AT91SAM HID Keyboard Demo Application");
            tmp.Add(0x6129, "AT91SAM Mass Storage Demo Application");
            tmp.Add(0x2FF6, "at32uc3b0/ 1 DFU bootloader");
            tmp.Add(0x2FF7, "at90usb82 DFU bootloader");
            tmp.Add(0x2FF8, "at32uc3a0/ 1 DFU bootloader");
            tmp.Add(0x2FF9, "at90usb646/ 647 DFU bootloader");
            tmp.Add(0x2FFA, "at90usb162 DFU bootloader");
            tmp.Add(0x2FFB, "at90usb AVR DFU bootloader");
            tmp.Add(0x2FFD, "at89c5130/ c5131 DFU bootloader");
            tmp.Add(0x2FFF, "at89c5132/ c51snd1c DFU bootloader");
            tmp.Add(0x3301, "at43301 4 - Port Hub");
            tmp.Add(0x2FE6, "Cactus V6(DFU)");
            tmp.Add(0x2FEA, "Cactus RF60(DFU)");
            tmp.Add(0x2FEE, "atmega8u2 DFU bootloader");
            tmp.Add(0x2FEF, "atmega16u2 DFU bootloader");
            tmp.Add(0x2FF0, "atmega32u2 DFU bootloader");
            tmp.Add(0x2FF1, "at32uc3a3 DFU bootloader");
            tmp.Add(0x2FF3, "atmega16u4 DFU bootloader");
            tmp.Add(0x2FF4, "atmega32u4 DFU bootloader");
            tmp.Add(0x2110, "AVR JTAGICE3 Debugger and Programmer");
            tmp.Add(0x2111, "Xplained Pro board debugger and programmer");
            tmp.Add(0x2122, "XMEGA - A1 Explained evaluation kit");
            tmp.Add(0x2140, "AVR JTAGICE3(v3.x) Debugger and Programmer");
            tmp.Add(0x2141, "ICE debugger");
            tmp.Add(0x2145, "ATMEGA328P - XMINI(CDC ACM)");
            tmp.Add(0x2310, "EVK11xx evaluation board");
            tmp.Add(0x2404, "The Micro");
            tmp.Add(0x2FE4, "ATxmega32A4U DFU bootloader");
            tmp.Add(0x2068, "LUFA Virtual Serial / Mass Storage Demo");
            tmp.Add(0x2069, "LUFA Webserver Project");
            tmp.Add(0x2103, "JTAG ICE mkII");
            tmp.Add(0x2104, "AVR ISP mkII");
            tmp.Add(0x2105, "AVRONE!");
            tmp.Add(0x2106, "STK600 development board");
            tmp.Add(0x2107, "AVR Dragon");
            tmp.Add(0x2109, "STK541 ZigBee Development Board");
            tmp.Add(0x210A, "AT86RF230[RZUSBSTICK] transceiver");
            tmp.Add(0x210D, "XPLAIN evaluation kit(CDC ACM)");
            tmp.Add(0x204F, "LUFA Generic HID Demo Application");
            tmp.Add(0x2060, "Benito Programmer Project");
            tmp.Add(0x2061, "LUFA Combined Mass Storage and Keyboard Demo Application");
            tmp.Add(0x2062, "LUFA Combined CDC and Mouse Demo Application");
            tmp.Add(0x2063, "LUFA Datalogger Device");
            tmp.Add(0x2064, "Interfaceless Control-Only LUFA Devices");
            tmp.Add(0x2065, "LUFA Test and Measurement Demo Application");
            tmp.Add(0x2066, "LUFA Multiple Report HID Demo");
            tmp.Add(0x2045, "LUFA Mass Storage Demo Application");
            tmp.Add(0x2046, "LUFA Audio Output Demo Application");
            tmp.Add(0x2047, "LUFA Audio Input Demo Application");
            tmp.Add(0x2048, "LUFA MIDI Demo Application");
            tmp.Add(0x2049, "Stripe Snoop Magnetic Stripe Reader");
            tmp.Add(0x204A, "LUFA CDC Class Bootloader");
            tmp.Add(0x204B, "LUFA USB to Serial Adapter Project");
            tmp.Add(0x204C, "LUFA RNDIS Demo Application");
            tmp.Add(0x204D, "LUFA Combined Mouse and Keyboard Demo Application");
            tmp.Add(0x204E, "LUFA Dual CDC Demo Application");
            tmp.Add(0x201C, "at90usbkey sample firmware(HID mouse)");
            tmp.Add(0x201D, "at90usbkey sample firmware(HID generic)");
            tmp.Add(0x2022, "at90usbkey sample firmware(composite device)");
            tmp.Add(0x2040, "LUFA Test PID");
            tmp.Add(0x2041, "LUFA Mouse Demo Application");
            tmp.Add(0x2042, "LUFA Keyboard Demo Application");
            tmp.Add(0x2043, "LUFA Joystick Demo Application");
            tmp.Add(0x2044, "LUFA CDC Demo Application");
            tmp.Add(0x0902, "4 - Port Hub");
            tmp.Add(0x2002, "Mass Storage Device");
            tmp.Add(0x2015, "at90usbkey sample firmware(HID keyboard)");
            tmp.Add(0x2018, "at90usbkey sample firmware(CDC ACM)");
            tmp.Add(0x2019, "stk525 sample firmware(microphone)");

            products.Add(ATMEL_ID, tmp);
        }



    }
}

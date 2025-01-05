using Java.Lang;
using OS.Communication;
using Android.Hardware.Usb;
using String = System.String;
using Exception = System.Exception;
using Java.Util;
using IOException = Java.IO.IOException;

namespace OS.PlatformShared;

/// <summary>
/// CH34X USB to Serial Converter IC devices
/// </summary>
public class FTDI : AndroidUSB_Base, IDevicesService, ICommunicationDevice, ISerialDevice
{
    protected override uint VENDOR_ID => 0x0403;

    const uint FtdiFt231X = 0x6015;
    const uint FtdiFt2322 = 0x6010;
    const uint FtdiFt4232H = 0x6011;

    public static int USB_TYPE_STANDARD = 0x00 << 5;
    public static int USB_TYPE_CLASS = 0x00 << 5;
    public static int USB_TYPE_VENDOR = 0x00 << 5;
    public static int USB_TYPE_RESERVED = 0x00 << 5;

    public static int USB_RECIP_DEVICE = 0x00;
    public static int USB_RECIP_INTERFACE = 0x01;
    public static int USB_RECIP_ENDPOINT = 0x02;
    public static int USB_RECIP_OTHER = 0x03;

    public static int USB_ENDPOINT_IN = 0x80;
    public static int USB_ENDPOINT_OUT = 0x00;

    public static int USB_WRITE_TIMEOUT_MILLIS = 5000;
    public static int USB_READ_TIMEOUT_MILLIS = 5000;

    public static int SIO_SET_RTS_MASK = 0x2;
    public static int SIO_SET_RTS_HIGH = 2 | (SIO_SET_RTS_MASK << 8);
    public static int SIO_SET_RTS_LOW = 0 | (SIO_SET_RTS_MASK << 8);

    public static int SIO_SET_DTR_MASK = 0x1;
    public static int SIO_SET_DTR_HIGH = (1 | (SIO_SET_DTR_MASK << 8));
    public static int SIO_SET_DTR_LOW = (0 | (SIO_SET_DTR_MASK << 8));

    //    // From ftdi.h
    //    /**
    //     * Reset the port.
    //     */
    private static int _sioResetRequest = 0;

    //    /**
    //     * Set the modem control register.
    //     */
    private static int _sioModemCtrlRequest = 1;

    //    /**
    //     * Set flow control register.
    //     */
    private static int SIO_SET_FLOW_CTRL_REQUEST = 2;

    private static int SIO_SET_LATENCY_TIMER_REQUEST = 9;

    //    /**
    //     * Set baud rate.
    //     */
    private const int SioSetBaudRateRequest = 3;

    //    /**
    //     * Set the data characteristics of the port.
    //     */
    private const int SioSetDataRequest = 4;

    private const int SioResetSio = 0;
    private const int SioResetPurgeRx = 1;
    private const int SioResetPurgeTx = 2;


    public static readonly UsbAddressing FtdiDeviceOutReqtype =
                (UsbAddressing)(UsbConstants.UsbTypeVendor | USB_RECIP_DEVICE | USB_ENDPOINT_OUT);

    public static int FTDI_DEVICE_IN_REQTYPE = UsbConstants.UsbTypeVendor | USB_RECIP_DEVICE | USB_ENDPOINT_IN;

    private const int ModemStatusHeaderLength = 2;

    private string TAG = typeof(FtdiSerialDriver).Name;

    private DeviceType _type;

    private readonly FtdiSerialDriver _driver;

    private int _index;

    private UsbEndpoint _readEndpoint;


    public override int BUFFER_SIZE =>1024;


    public FTDI() : base()
    {

    }

    protected override void InitUSBSerial()
    {
        SetParameters((int)BaudRate, 8, 1, 0);
    }


    private int setBaudRate(int baudRate)
    {
        var vals = GetBaudRate(baudRate);

        int result = deviceConnection.ControlTransfer(FtdiDeviceOutReqtype,
            SioSetBaudRateRequest, vals.Value, vals.Index,
            null, 0, USB_WRITE_TIMEOUT_MILLIS);

        if (result != 0)
        {
            throw new Java.IO.IOException($"Setting baudrate failed: result={result}");
        }

        return vals.BaudRate;
    }

    public override bool Initialize()
    {
        return base.Initialize();
    }

    public void SetParameters(int baudRate, int dataBits, decimal stopBits, int parity)
    {
        setBaudRate(baudRate);

        int config = (int)dataBits;

        switch (parity)
        {
            case 0:
                config |= (0x00 << 8);
                break;
            case 1:
                config |= (0x01 << 8);
                break;
            case 2:
                config |= (0x02 << 8);
                break;
            case 3:
                config |= (0x03 << 8);
                break;
            case 4:
                config |= (0x04 << 8);
                break;
            default:
                throw new IllegalArgumentException("Unknown parity value: " + parity);
        }

        switch (stopBits)
        {
            case 1m:
                config |= (0x00 << 11);
                break;
            case 1.5m:
                config |= (0x01 << 11);
                break;
            case 2m:
                config |= (0x02 << 11);
                break;
            default:
                throw new IllegalArgumentException("Unknown stopBits value: " + stopBits);
        }

        int result = deviceConnection.ControlTransfer(FtdiDeviceOutReqtype,
            SioSetDataRequest, config, _index,
            null, 0, USB_WRITE_TIMEOUT_MILLIS);

        if (result != 0)
        {
            throw new IOException("Setting parameters failed: result=" + result);
        }
    }


    private uint H_CLK = 120000000;
    private uint C_CLK = 48000000;

    private BaudRateResponse GetBaudRate(int baudRate)
    {
        int result = 1;
        int[] divisors = new int[2];
        int status = 0;

        if (_type != DeviceType.TYPE_4232H)
        {
            switch (baudRate)
            {
                case 300:
                    divisors[0] = 10000;
                    break;
                case 600:
                    divisors[0] = 5000;
                    break;
                case 1200:
                    divisors[0] = 2500;
                    break;
                case 2400:
                    divisors[0] = 1250;
                    break;
                case 4800:
                    divisors[0] = 625;
                    break;
                case 9600:
                    divisors[0] = 16696;
                    break;
                case 19200:
                    divisors[0] = 32924;
                    break;
                case 38400:
                    divisors[0] = 49230;
                    break;
                case 57600:
                    divisors[0] = 52;
                    break;
                case 115200:
                    divisors[0] = 26;
                    break;
                case 230400:
                    divisors[0] = 13;
                    break;
                case 460800:
                    divisors[0] = 16390;
                    break;
                case 921600:
                    divisors[0] = 32771;
                    break;
                    //default:
                    //    if ((isHiSpeed()) && (baudRate >= 1200))
                    //    {
                    //        result = FT_BaudRate.FT_GetDivisorHi(baudRate, divisors);
                    //    }
                    //    else {
                    //        result = FT_BaudRate.FT_GetDivisor(baudRate, divisors,
                    //          isBmDevice());
                    //    }
                    //    status = 255;
                    //    break;
            }
        }
        else
        {
            divisors[0] = (int)Ftdi2232HBaudToDivisor(baudRate);
        }

        var urbValue = (UInt16)divisors[0];

        var index = (UInt16)(divisors[0] >> 16);

        if (isMultiIfDevice())
        {
            index = (UInt16)((index << 8) | _index);
        }

        return new BaudRateResponse
        {
            Index = index,
            Value = urbValue
        };
    }
    private uint Ftdi2232HBaudToDivisor(int baud)
    {
        return Ftdi2232HBaudBaseToDivisor(baud, 120000000);
    }

    private uint Ftdi2232HBaudBaseToDivisor(int baud, int @base)
    {
        int[] divfrac = { 0, 3, 2, 4, 1, 5, 6, 7 };
        uint divisor3 = (uint)(@base / 10 / baud) * 8; // hi-speed baud rate is 10-bit sampling instead of 16-bit
        uint divisor = divisor3 >> 3;
        divisor |= (uint)divfrac[divisor3 & 0x7] << 14;
        /* Deal with special cases for highest baud rates. */
        if (divisor == 1) divisor = 0;
        else    // 1.0
        if (divisor == 0x4001) divisor = 1; // 1.5
        /* Set this bit to turn off a divide by 2.5 on baud rate generator */
        /* This enables baud rates up to 12Mbaud but cannot reach below 1200 baud with this bit set */
        divisor |= 0x00020000;

        return divisor;
    }
    private bool isHiSpeed()
    {
        return _type == DeviceType.TYPE_232H
            || _type == DeviceType.TYPE_2232H
            || _type == DeviceType.TYPE_4232H;
    }

    private bool isBmDevice()
    {
        return _type == DeviceType.TYPE_BM;
        //(isFt232b()) || (isFt2232()) || (isFt232r()) || (isFt2232h()) || (isFt4232h()) || (isFt232h()) || (isFt232ex());
    }

    bool isMultiIfDevice()
    {
        return _type == DeviceType.TYPE_2232H
               || _type == DeviceType.TYPE_4232H
               || _type == DeviceType.TYPE_2232C;
    }

    private BaudRateResponse FtdiConverBaudrate(int baudrate)
    {
        int bestBaud;
        ulong encodedDivisor = 0;
        ushort index;

        if (baudrate <= 0)
            throw new ArgumentException($"baudrate cannot be 0 or lower", nameof(baudrate));

        if (_type == DeviceType.TYPE_2232H || _type == DeviceType.TYPE_4232H | _type == DeviceType.TYPE_232H)
        {
            if (baudrate * 10 > H_CLK / 0x3fff)
            {
                bestBaud = FtdiToClkbits(baudrate, H_CLK, 10, ref encodedDivisor);
                encodedDivisor |= 0x20000;
            }
            else
            {
                bestBaud = FtdiToClkbits(baudrate, C_CLK, 16, ref encodedDivisor);
            }
        }
        else if (_type == DeviceType.TYPE_BM || _type == DeviceType.TYPE_2232C || _type == DeviceType.TYPE_R)
        {
            bestBaud = FtdiToClkbits(baudrate, C_CLK, 16, ref encodedDivisor);
        }
        else
        {
            bestBaud = ftdi_to_clkbits_AM(baudrate, ref encodedDivisor);
        }

        var value = (ushort)(encodedDivisor & 0xffff);
        if (_type == DeviceType.TYPE_2232H || _type == DeviceType.TYPE_4232H || _type == DeviceType.TYPE_232H)
        {
            index = (ushort)(encodedDivisor >> 8);
            index &= 0xff00;
            index |= (ushort)(_index + 1);
        }
        else
            index = (ushort)(encodedDivisor >> 16);

        return new BaudRateResponse
        {
            Index = index,
            BaudRate = bestBaud,
            Value = value
        };
    }

    private int FtdiToClkbits(int baudrate, uint clk, int clkDivisor, ref ulong encodedDivisor)
    {
        int[] fracCode = { 0, 3, 2, 4, 1, 5, 6, 7 };
        int bestBaud;

        if (baudrate >= clk / clkDivisor)
        {
            encodedDivisor = 0;
            bestBaud = (int)(clk / clkDivisor);
        }
        else if (baudrate >= clk / (clkDivisor + clkDivisor / 2))
        {
            encodedDivisor = 1;
            bestBaud = (int)(clk / (2 * clkDivisor));
        }
        else if (baudrate >= clk / (2 * clkDivisor))
        {
            encodedDivisor = 2;
            bestBaud = (int)(clk / (2 * clkDivisor));
        }
        else
        {
            var divisor = (int)(clk * 16 / clkDivisor / baudrate);
            int bestDivisor;
            if ((divisor & 1) > 0)
            {
                bestDivisor = divisor / 2 + 1;
            }
            else
            {
                bestDivisor = divisor / 2;
            }

            if (bestDivisor > 0x20000)
                bestDivisor = 0x1ffff;

            bestBaud = (int)(clk * 16 / clkDivisor / bestDivisor);
            encodedDivisor = (ulong)((bestDivisor >> 1) | (fracCode[bestDivisor & 0x7] << 14));
        }

        return bestBaud;
    }

    private static int ftdi_to_clkbits_AM(int baudrate, ref ulong encodedDivisor)
    {
        int[] fracCode = { 0, 3, 2, 4, 1, 5, 6, 7 };

        int[] amAdjustUp = { 0, 0, 0, 1, 0, 3, 2, 1 };

        int[] amAdjustDn = { 0, 0, 0, 1, 0, 1, 2, 3 };

        int i;

        var divisor = 24000000 / baudrate;

        divisor -= amAdjustDn[divisor & 7];

        var bestDivisor = 0;
        var bestBaud = 0;
        var bestBaudDiff = 0;

        for (i = 0; i < 2; i++)
        {
            int tryDivisor = divisor + i;

            int baudDiff;

            if (tryDivisor <= 8)
            {
                tryDivisor = 8;
            }
            else if (divisor < 16)
            {
                tryDivisor = 16;
            }
            else
            {
                tryDivisor += amAdjustUp[tryDivisor & 7];
                if (tryDivisor > 0x1fff8)
                {
                    tryDivisor = 0x1fff8;
                }
            }

            var baudEstimate = (24000000 + (tryDivisor / 2)) / tryDivisor;

            if (baudEstimate < baudrate)
            {
                baudDiff = baudrate - baudEstimate;
            }
            else
            {
                baudDiff = baudEstimate - baudrate;
            }

            if (i == 0 || baudDiff < bestBaudDiff)
            {
                bestDivisor = tryDivisor;
                bestBaud = baudEstimate;
                bestBaudDiff = baudDiff;
                if (baudDiff == 0)
                    break;
            }
        }

        encodedDivisor = (ulong)((bestDivisor >> 3) | (fracCode[bestDivisor & 7] << 14));
        if (encodedDivisor == 1)
        {
            encodedDivisor = 0;
        }
        else if (encodedDivisor == 0x4001)
        {
            encodedDivisor = 1;
        }

        return bestBaud;
    }

    public bool Cd => false;

    private bool _cts;
    public bool Cts
    {
        get { return _cts; }
        set
        {
            if (_cts == value)
                return;

            _cts = value;

            CtsChanged?.Invoke(this, _cts);
        }
    }

    public event EventHandler<bool> CtsChanged;

    public bool Dsr => false;

    public bool Dtr
    {
        get { return false; }
        set { }
    }

    public bool Ri => false;

    private bool _rts;

    public bool Rts
    {
        get { return _rts; }
        set
        {
            if (value == _rts)
                return;

            _rts = value;

            ushort usbValue = (ushort)SIO_SET_DTR_LOW;

            if (_rts)
                usbValue |= (ushort)SIO_SET_RTS_HIGH;
            else
                usbValue |= (ushort)SIO_SET_RTS_LOW;

            int result = deviceConnection.ControlTransfer(FtdiDeviceOutReqtype, _sioModemCtrlRequest,
                usbValue, _index + 1, null, 0, USB_WRITE_TIMEOUT_MILLIS);

            if (result != 0)
            {
                throw new IOException("Set RTS failed result=" + result);
            }
        }
    }

    public bool PurgeHwBuffers(bool purgeReadBuffers, bool purgeWriteBuffers)
    {
        if (purgeReadBuffers)
        {
            int result = deviceConnection.ControlTransfer(FtdiDeviceOutReqtype, _sioResetRequest,
                SioResetPurgeRx, _index + 1, null, 0, USB_WRITE_TIMEOUT_MILLIS);

            if (result != 0)
            {
                throw new IOException("Flushing RX failed: result=" + result);
            }
        }

        if (purgeWriteBuffers)
        {
            int result = deviceConnection.ControlTransfer(FtdiDeviceOutReqtype, _sioResetRequest,
                SioResetPurgeTx, _index + 1, null, 0, USB_WRITE_TIMEOUT_MILLIS);

            if (result != 0)
            {
                throw new IOException("Flushing RX failed: result=" + result);
            }
        }
        return true;
    }









    /// <summary>
    /// Filter FTDI status bytes from buffer
    /// </summary>
    /// <param name="src">The source buffer (which contains status bytes)</param>
    /// <param name="dest">The destination buffer to write the status bytes into (can be src)</param>
    /// <param name="totalBytesRead">Number of bytes read to src</param>
    /// <param name="maxPacketSize">The USB endpoint max packet size</param>
    /// <returns>The number of payload bytes</returns>
    private int filterStatusBytes(byte[] src, byte[] dest, int totalBytesRead, int maxPacketSize)
    {
        int packetsCount = totalBytesRead / maxPacketSize + (totalBytesRead % maxPacketSize == 0 ? 0 : 1);
        for (int packetIdx = 0; packetIdx < packetsCount; ++packetIdx)
        {
            int count = (packetIdx == (packetsCount - 1))
                ? (totalBytesRead % maxPacketSize) - ModemStatusHeaderLength
                : maxPacketSize - ModemStatusHeaderLength;
            if (count > 0)
            {
                Buffer.BlockCopy(src,
                    packetIdx * maxPacketSize + ModemStatusHeaderLength,
                    dest,
                    packetIdx * (maxPacketSize - ModemStatusHeaderLength),
                    count);
            }
        }

        return totalBytesRead - (packetsCount * 2);
    }

    public void Reset()
    {
        int result = deviceConnection.ControlTransfer(FtdiDeviceOutReqtype, _sioResetRequest,
            SioResetSio, _index, null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Reset failed: result=" + result);
        }

        var productId = usbDevice.ProductId;

        if (productId == 0x6001)
            _type = DeviceType.TYPE_BM;
        else if (productId == 0x6010)
            _type = DeviceType.TYPE_2232H;
        else if (productId == 0x6011)
            _type = DeviceType.TYPE_4232H;
    }

}



public class BaudRateResponse
{
    public int Value { get; set; }
    public int Index { get; set; }
    public int BaudRate { get; set; }
}
public class FtdiSerialDriver// : IUsbSerialDriver
{
    public UsbDevice Device { get; }

//      public List<IUsbSerialPort> Ports { get; } = new List<IUsbSerialPort>();

    private System.Threading.Thread _receiver;

    private UsbDeviceConnection _connection;

    public FtdiSerialDriver(UsbDevice device)
    {
        //Device = device;

        //for (int i = 0; i < Device.InterfaceCount; i++)
        //{
        //    Ports.Add(new FTDI(Device, i, this));
        //}
    }

//    public static Dictionary<int, int[]> GetSupportedDevices()
//    {
//        return new Dictionary<int, int[]>
//        {
//            {
//                UsbId.VendorFtdi, new[]
//                {
//                    UsbId.FtdiFt232R,
//                    UsbId.FtdiFt231X,
//                    UsbId.FtdiFt2322,
//                    UsbId.FtdiFt4232H
//                }
//            },
//            {
//                UsbId.VendorSyncromatics, new[]
//                {
//                    UsbId.SyncromaticsCovertAlarm
//                }
//            }
//        };
//    }
}
internal enum DeviceType
{
    TYPE_BM,
    TYPE_AM,
    TYPE_2232C,
    TYPE_R,
    TYPE_2232H,
    TYPE_4232H,
    TYPE_232H
}
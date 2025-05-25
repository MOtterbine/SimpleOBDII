using OS.Communication;
using Android.Hardware.Usb;
using String = System.String;
using Exception = System.Exception;


namespace OS.PlatformShared;

/// <summary>
/// CH34X USB to Serial Converter IC devices
/// </summary>
public class CP21X : AndroidUSB_Base, IDevicesService, ICommunicationDevice, ISerialDevice
{
    protected override uint VENDOR_ID => 0x10c4;
    public static readonly int SILABS_CP2102 = 0xea60;
    public static readonly int SILABS_CP2105 = 0xea70;
    public static readonly int SILABS_CP2108 = 0xea71;
    public static readonly int SILABS_CP2110 = 0xea80;

    private const int SILABSER_SET_LINE_CTL_REQUEST_CODE = 0x03;

    private const int REQTYPE_HOST_TO_DEVICE = 0x41;
    private const int SILABSER_SET_BAUDRATE = 0x1E;

    public bool RTS
    {
        get => rts;
        set
        {
            rts = value;
        }
    }
    public bool DTR
    {
        get => dtr;
        set
        {
            dtr = value;
        }
    }
    private bool dtr = false;

    private bool rts = false;



    public override int BUFFER_SIZE => 1024;

    public CP21X() : base() { }


    protected override void InitUSBSerial()
    {
        byte[] buffer = new byte[10];

        int dataBits = 8;
        int parity = 0;
        int stopBits = 1;

        setBaudRate((int)BaudRate);

        int configDataBits = 0;
        switch (dataBits)
        {
            case 5:
                configDataBits |= 0x0500;
                break;
            case 6:
                configDataBits |= 0x0600;
                break;
            case 7:
                configDataBits |= 0x0700;
                break;
            case 8:
                configDataBits |= 0x0800;
                break;
            default:
                configDataBits |= 0x0800;
                break;
        }

        switch (parity)
        {
            case 0: // parity none
                break;
            case 1:// Parity.Odd:
                configDataBits |= 0x0010;
                break;
            case 2:// Parity.Even:
                configDataBits |= 0x0020;
                break;
        }

        switch (stopBits)
        {
            case 1:
                configDataBits |= 0;
                break;
            case 2:
                configDataBits |= 2;
                break;
        }
        SetConfigSingle(SILABSER_SET_LINE_CTL_REQUEST_CODE, configDataBits);
    }

    private int SetConfigSingle(int request, int value)
    {
        return deviceConnection.ControlTransfer((UsbAddressing)REQTYPE_HOST_TO_DEVICE, request, value,
                0, null, 0, 200);
    }

    private void setBaudRate(int baudRate)
    {

        byte[] data = new byte[] {
                (byte) ( baudRate & 0xff),
                (byte) ((baudRate >> 8 ) & 0xff),
                (byte) ((baudRate >> 16) & 0xff),
                (byte) ((baudRate >> 24) & 0xff)
            };

        int ret = deviceConnection.ControlTransfer((UsbAddressing)REQTYPE_HOST_TO_DEVICE, SILABSER_SET_BAUDRATE,
                0, 0, data, 4, 200);
        if (ret < 0)
        {
            throw new Java.IO.IOException("Error setting baud rate.");
        }

    }

    public override bool Initialize()
    {
        return base.Initialize();
    }

}
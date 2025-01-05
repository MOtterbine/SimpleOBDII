using Java.Lang;
using OS.Communication;
using Android.Hardware.Usb;
using String = System.String;
using Exception = System.Exception;
using IOException = Java.IO.IOException;

namespace OS.PlatformShared;

/// <summary>
/// CH34X USB to Serial Converter IC devices
/// </summary>
public class CH34X : AndroidUSB_Base, IDevicesService, ICommunicationDevice, ISerialDevice
{
    protected override uint VENDOR_ID => 0x1A86;
    const uint CH340_PRODUCT_ID = 0x7523;
    const uint CH341_PRODUCT_ID = 0x5523;

    const uint CH34x_BUF_SIZE = 1024;
    const uint CH34x_TMP_BUF_SIZE = 1024;

    const uint LCR_ENABLE_RX = 0x80;
    const uint LCR_ENABLE_TX = 0x40;
    const uint LCR_MARK_SPACE = 0x20;
    const uint LCR_PAR_EVEN = 0x10;
    const uint LCR_ENABLE_PAR = 0x08;
    const uint LCR_STOP_BITS_2 = 0x04;
    const uint LCR_CS8 = 0x03;
    const uint LCR_CS7 = 0x02;
    const uint LCR_CS6 = 0x01;
    const uint LCR_CS5 = 0x00;

    //Vendor define
    const uint VENDOR_WRITE_TYPE = 0x40;
    const uint VENDOR_READ_TYPE = 0xC0;

    const uint VENDOR_READ = 0x95;
    const uint VENDOR_WRITE = 0x9A;
    const uint VENDOR_SERIAL_INIT = 0xA1;
    const uint VENDOR_MODEM_OUT = 0xA4;
    const uint VENDOR_VERSION = 0x5F;

    //For CMD 0xA4
    const uint UART_CTS = 0x01;
    const uint UART_DSR = 0x02;
    const uint UART_RING = 0x04;
    const uint UART_DCD = 0x08;
    const uint CONTROL_OUT = 0x10;
    const uint CONTROL_DTR = 0x20;
    const uint CONTROL_RTS = 0x40;

    //Uart state
    const uint UART_STATE = 0x00;
    const uint UART_OVERRUN_ERROR = 0x01;
    //const uint UART_BREAK_ERROR	//no define
    const uint UART_PARITY_ERROR = 0x02;
    const uint UART_FRAME_ERROR = 0x06;
    const uint UART_RECV_ERROR = 0x02;
    const uint UART_STATE_TRANSIENT_MASK = 0x07;

    //Port state
    const uint PORTA_STATE = 0x01;
    const uint PORTB_STATE = 0x02;
    const uint PORTC_STATE = 0x03;

    //CH34x Baud Rate
    const ulong CH34x_BAUDRATE_FACTOR = 1532620800;
    const short CH34x_BAUDRATE_DIVMAX = 3;

    private static int USB_RECIP_INTERFACE = 0x01;
    private static readonly int UsbRtAcm = UsbConstants.UsbTypeClass | USB_RECIP_INTERFACE;
    private static int SET_LINE_CODING = 0x20;  // USB CDC 1.1 section 6.2


    // ELM327 uart rx buffer is 512 bytes
    public override int BUFFER_SIZE =>1024;


    public CH34X() : base()
    {

    }

    protected override void InitUSBSerial()
    { 
        byte[] buffer = new byte[10];

        checkState("init #1", 0x5f, 0, new int[] { -1 /* 0x27, 0x30 */, 0x00 });

        if (deviceConnection.ControlTransfer((UsbAddressing)VENDOR_WRITE_TYPE, (int)VENDOR_SERIAL_INIT, 0, 0, null, 0, 200) < 0)
        {
            throw new IOException("Init failed: #2");
        }

        checkState("init #4", 0x95, 0x2518, new int[] { -1 /* 0x56, c3*/, 0x00 });

        if (deviceConnection.ControlTransfer((UsbAddressing)VENDOR_WRITE_TYPE, (int)VENDOR_WRITE, 0x2518, (int)(LCR_ENABLE_RX | LCR_ENABLE_TX | LCR_CS8), null, 0, 200) < 0)
        {
            throw new IOException("Init failed: #5");
        }

        checkState("init #6", 0x95, 0x0706, new int[] { -1/*0xf?*/, -1/*0xec,0xee*/});

        if (deviceConnection.ControlTransfer((UsbAddressing)VENDOR_WRITE_TYPE, (int)VENDOR_SERIAL_INIT, 0x501f, 0xd90a, null, 0, 200) < 0)
        {
            throw new IOException("Init failed: #7");
        }


        checkState("init #10", 0x95, 0x0706, new int[] { -1/* 0x9f, 0xff*/, -1/*0xec,0xee*/});

        if (deviceConnection.ControlTransfer((UsbAddressing)VENDOR_WRITE_TYPE, (int)VENDOR_WRITE, 0x2727, 0, null, 0, 200) < 0)
        {
            throw new IOException("Init failed: #8");
        }

        if (deviceConnection.ControlTransfer((UsbAddressing)VENDOR_WRITE_TYPE, (int)VENDOR_MODEM_OUT, 0x009F, 0, null, 0, 200) < 0)
        {
            throw new IOException("Init failed: #9");
        }

        setBaudRate((int)BaudRate);

    }

    private void checkState(String msg, int request, int value, int[] expected)// throws IOException
    {
        byte[] buffer = new byte[expected.Length];

        int ret = deviceConnection.ControlTransfer((UsbAddressing)VENDOR_READ_TYPE, request, value, 0, buffer, expected.Length, 200);

        if (ret< 0) {
            throw new IOException("Failed to send ctrl cmd [" + msg + "]");
        }

        if (ret != expected.Length) {
            throw new IOException($"Expected {expected.Length} bytes, but got {ret} [{msg}]");
        }

        for (int i = 0; i < expected.Length; i++)
        {
            if (expected[i] == -1)
            {
                continue;
            }

            int current = buffer[i] & 0xff;
            if (expected[i] != current)
            {
                throw new IOException("Expected 0x" + Integer.ToHexString(expected[i]) + " byte, but get 0x" + Integer.ToHexString(current) + " [" + msg + "]");
            }
        }
    }

    private void setBaudRate(int baudRate)// throws IOException
    {
        long factor;
        long divisor;

        if (baudRate == 921600) {
            divisor = 7;
            factor = 0xf300;
        } else {
            long BAUDBASE_FACTOR = 1532620800;
            int BAUDBASE_DIVMAX = 3;

            factor = BAUDBASE_FACTOR / baudRate;
            divisor = BAUDBASE_DIVMAX;
            while ((factor > 0xfff0) && divisor > 0) {
                factor >>= 3;
                divisor--;
            }
            if (factor > 0xfff0)
            {
                throw new UnsupportedOperationException("Unsupported baud rate: " + baudRate);
            }
            factor = 0x10000 - factor;
        }

        divisor |= 0x0080; // else ch341a waits until buffer full
        int val1 = (int)((factor & 0xff00) | divisor);
        int val2 = (int)(factor & 0xff);
        FireInfoEvent(String.Format("baud rate=%d, 0x1312=0x%04x, 0x0f2c=0x%04x", baudRate, val1, val2));

        int ret = -1;

        ret = deviceConnection.ControlTransfer((UsbAddressing)VENDOR_WRITE_TYPE, (int)VENDOR_WRITE, 0x1312, (int)val1, null, 0, 0, 250);
        if (ret < 0)
        {
            throw new IOException($"Error setting baud rate: #1):{ret}");
        }

        ret = deviceConnection.ControlTransfer((UsbAddressing)VENDOR_WRITE_TYPE, (int)VENDOR_WRITE, 0x0f2c, (int)val2, null, 0, 0, 250);
        if (ret < 0)
        {
            throw new IOException($"Error setting baud rate: #1):{ret}");
        }

    }

    public override bool Initialize()
    {
        return base.Initialize();
    }

}
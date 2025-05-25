using OS.Communication;
using Android.Hardware.Usb;
using Java.Lang;
using String = System.String;
using Exception = System.Exception;

namespace OS.PlatformShared;


public class USBSerial_FTDI : AndroidUSB_Base, IDevicesService, ICommunicationDevice, ISerialDevice
{
    protected override uint VENDOR_ID => 0x0403;


    private static int USB_WRITE_TIMEOUT_MILLIS = 5000;
    private static int READ_HEADER_LENGTH = 2; // contains MODEM_STATUS


    public static int USB_RECIP_DEVICE = 0x00;
    public static int USB_RECIP_INTERFACE = 0x01;
    public static int USB_RECIP_ENDPOINT = 0x02;
    public static int USB_RECIP_OTHER = 0x03;

    public static int USB_ENDPOINT_IN = 0x80;
    public static int USB_ENDPOINT_OUT = 0x00;


    private static readonly UsbAddressing REQTYPE_HOST_TO_DEVICE =
                (UsbAddressing)(UsbConstants.UsbTypeVendor | USB_RECIP_DEVICE | USB_ENDPOINT_OUT);
    private static readonly UsbAddressing REQTYPE_DEVICE_TO_HOST = (UsbAddressing)(UsbConstants.UsbTypeVendor | USB_RECIP_DEVICE | USB_ENDPOINT_IN);

    private static int RESET_REQUEST = 0;
    private static int MODEM_CONTROL_REQUEST = 1;
    private static int SET_FLOW_CONTROL_REQUEST = 2;
    private static int SET_BAUD_RATE_REQUEST = 3;
    private static int SET_DATA_REQUEST = 4;
    private static int GET_MODEM_STATUS_REQUEST = 5;
    private static int SET_LATENCY_TIMER_REQUEST = 9;
    private static int GET_LATENCY_TIMER_REQUEST = 10;

    private static int MODEM_CONTROL_DTR_ENABLE = 0x0101;
    private static int MODEM_CONTROL_DTR_DISABLE = 0x0100;
    private static int MODEM_CONTROL_RTS_ENABLE = 0x0202;
    private static int MODEM_CONTROL_RTS_DISABLE = 0x0200;
    private static int MODEM_STATUS_CTS = 0x10;
    private static int MODEM_STATUS_DSR = 0x20;
    private static int MODEM_STATUS_RI = 0x40;
    private static int MODEM_STATUS_CD = 0x80;
    private static int RESET_ALL = 0;
    private static int RESET_PURGE_RX = 1;
    private static int RESET_PURGE_TX = 2;

    private bool baudRateWithPort = false;

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
    private int breakConfig = 0;

    private int _index = 0;


    protected override void InitUSBSerial()
    {

        Reset();

        var result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, MODEM_CONTROL_REQUEST,
                (dtr ? MODEM_CONTROL_DTR_ENABLE : MODEM_CONTROL_DTR_DISABLE) |
                        (rts ? MODEM_CONTROL_RTS_ENABLE : MODEM_CONTROL_RTS_DISABLE),
                _index +1 , null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Init RTS,DTR failed: result=" + result);
        }
        SetFlowControl(FlowControl.NONE);

        // mDevice.getVersion() would require API 23
        byte[] rawDescriptors = deviceConnection.GetRawDescriptors();
        if (rawDescriptors == null || rawDescriptors.Length < 14)
        {
            throw new IOException("Could not get device descriptors");
        }
        int deviceType = rawDescriptors[13];
        baudRateWithPort = deviceType == 7 || deviceType == 8 || deviceType == 9 // ...H devices
                || usbDevice.InterfaceCount > 1; // FT2232C

        SetParameters((int)BaudRate, 8, 1, 0);

        setLatencyTimer(250);

    }


    public void Reset()
    {
        int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, RESET_REQUEST,
            RESET_ALL, _index +1 , null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Reset failed: result=" + result);
        }

        //var productId = usbDevice.ProductId;

        //if (productId == 0x6001)
        //    Type = DeviceType.TYPE_BM;
        //else if (productId == 0x6010)
        //    _type = DeviceType.TYPE_2232H;
        //else if (productId == 0x6011)
        //    _type = DeviceType.TYPE_4232H;
    }


    private void SetBaudrate(int baudRate)
    {
        int divisor, subdivisor, effectiveBaudRate;
        if (baudRate > 3500000)
        {
            throw new UnsupportedOperationException("Baud rate too high");
        }
        else if (baudRate >= 2500000)
        {
            divisor = 0;
            subdivisor = 0;
            effectiveBaudRate = 3000000;
        }
        else if (baudRate >= 1750000)
        {
            divisor = 1;
            subdivisor = 0;
            effectiveBaudRate = 2000000;
        }
        else
        {
            divisor = (24000000 << 1) / baudRate;
            divisor = (divisor + 1) >> 1; // round
            subdivisor = divisor & 0x07;
            divisor >>= 3;
            if (divisor > 0x3fff) // exceeds bit 13 at 183 baud
                throw new UnsupportedOperationException("Baud rate too low");
            effectiveBaudRate = (24000000 << 1) / ((divisor << 3) + subdivisor);
            effectiveBaudRate = (effectiveBaudRate + 1) >> 1;
        }
        double baudRateError = Java.Lang.Math.Abs(1.0 - (effectiveBaudRate / (double)baudRate));
        if (baudRateError >= 0.031) // can happen only > 1.5Mbaud
            throw new UnsupportedOperationException(String.Format("Baud rate deviation %.1f%% is higher than allowed 3%%", baudRateError * 100));
        int value = divisor;
        int index = 0;
        switch (subdivisor)
        {
            case 0: break; // 16,15,14 = 000 - sub-integer divisor = 0
            case 4: value |= 0x4000; break; // 16,15,14 = 001 - sub-integer divisor = 0.5
            case 2: value |= 0x8000; break; // 16,15,14 = 010 - sub-integer divisor = 0.25
            case 1: value |= 0xc000; break; // 16,15,14 = 011 - sub-integer divisor = 0.125
            case 3: value |= 0x0000; index |= 1; break; // 16,15,14 = 100 - sub-integer divisor = 0.375
            case 5: value |= 0x4000; index |= 1; break; // 16,15,14 = 101 - sub-integer divisor = 0.625
            case 6: value |= 0x8000; index |= 1; break; // 16,15,14 = 110 - sub-integer divisor = 0.75
            case 7: value |= 0xc000; index |= 1; break; // 16,15,14 = 111 - sub-integer divisor = 0.875
        }
        if (baudRateWithPort)
        {
            index <<= 8;
            index |= _index + 1;
        }

        //Debug.wr.d(TAG, String.format("baud rate=%d, effective=%d, error=%.1f%%, value=0x%04x, index=0x%04x, divisor=%d, subdivisor=%d",
        //        baudRate, effectiveBaudRate, baudRateError * 100, value, index, divisor, subdivisor));
        int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, SET_BAUD_RATE_REQUEST,
                value, index, null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Setting baudrate failed: result=" + result);
        }

    }

    public void SetParameters(int baudRate, int dataBits, int stopBits, int parity)
    {
        if (baudRate <= 0)
        {
            throw new IllegalArgumentException("Invalid baud rate: " + baudRate);
        }
        SetBaudrate(baudRate);

        int config = 0 | dataBits;
        //    switch (dataBits) {
        //        case DATABITS_5:
        //case DATABITS_6:
        //    throw new UnsupportedOperationException("Unsupported data bits: " + dataBits);
        //case DATABITS_7:
        //case DATABITS_8:
        //    config |= dataBits;
        //    break;
        //default:
        //    throw new IllegalArgumentException("Invalid data bits: " + dataBits);
        //}

        switch (parity)
        {
            case 0://PARITY_NONE:
                break;
            case 1://PARITY_ODD:
                config |= 0x100;
                break;
            case 2://PARITY_EVEN:
                config |= 0x200;
                break;
            case 3://PARITY_MARK:
                config |= 0x300;
                break;
            case 4://PARITY_SPACE:
                config |= 0x400;
                break;
            default:
                throw new IllegalArgumentException("Invalid parity: " + parity);
        }


        switch (stopBits)
        {
            case 1://STOPBITS_1:
                break;
            case 3://STOPBITS_1_5:
                // throw new UnsupportedOperationException("Unsupported stop bits: 1.5");
            case 2://STOPBITS_2:
                config |= 0x1000;
                break;
            default:
                throw new IllegalArgumentException("Invalid stop bits: " + stopBits);
        }

        // Set non-baud parameters from above
        int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, SET_DATA_REQUEST,
                config, _index , null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Setting parameters failed: result=" + result);
        }
        breakConfig = config;

    }


    public override async Task Listen()
    {
        if (this.usbDevice == null)
        {
            this.FireErrorEvent($"ERROR: Unable to access USB device");
            return;
        }

        if (!this.IsConnected) return;
        int bytesRead = 0;
        try
        {
            this.tokenSource = new CancellationTokenSource();

            if (this._endpoint_rx != null)
            {
                this.openEvent.Set(); // the device open even is resolved

                while (!this.tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        rcvLock.WaitOne();
                        for (int i = 0; i < 40; i++)
                        {
                            TmpBuffer[i] = 0x00;
                        }
                        rcvLock.Reset();
                        bytesRead = await deviceConnection.BulkTransferAsync(_endpoint_rx, TmpBuffer, BUFFER_SIZE, 370);

                        // Two modem status bytes must be stripped off
                        if (bytesRead > 0)
                        {
                            // var sbuf = TmpBuffer.Take(bytesRead).ToArray();
                            byte[] sbuf = new byte[32];
                            if (bytesRead > 2)
                            {
                                FireReceiveEvent(TmpBuffer.Take(new System.Range(2, bytesRead)).ToArray());
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {
                        rcvLock.Set();
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }
        finally
        {
            this.tokenSource.Cancel();
            this.rcvLock.Set();
        }

    }


    private int getStatus()
    {
        byte[] data = new byte[2];
        int result = deviceConnection.ControlTransfer(REQTYPE_DEVICE_TO_HOST, GET_MODEM_STATUS_REQUEST,
                0, _index + 0, data, data.Length, USB_WRITE_TIMEOUT_MILLIS);
        if (result != data.Length)
        {
            throw new IOException("Get modem status failed: result=" + result);
        }
        return data[0];
    }

    public bool getCD()
    {
        return (getStatus() & MODEM_STATUS_CD) != 0;
    }

    public bool getCTS()
    {
        return (getStatus() & MODEM_STATUS_CTS) != 0;
    }

    public bool getDSR()
    {
        return (getStatus() & MODEM_STATUS_DSR) != 0;
    }

    public bool getDTR()
    {
        return dtr;
    }

    public void setDTR(bool value)
    {
        int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, MODEM_CONTROL_REQUEST,
                value ? MODEM_CONTROL_DTR_ENABLE : MODEM_CONTROL_DTR_DISABLE, _index + 0, null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Set DTR failed: result=" + result);
        }
        dtr = value;
    }

    public bool getRI()
    {
        return (getStatus() & MODEM_STATUS_RI) != 0;
    }

    public bool getRTS()
    {
        return rts;
    }

    public void setRTS(bool value)
    {
        int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, MODEM_CONTROL_REQUEST,
                value ? MODEM_CONTROL_RTS_ENABLE : MODEM_CONTROL_RTS_DISABLE, _index + 0, null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Set DTR failed: result=" + result);
        }
        rts = value;
    }

    public void SetFlowControl(FlowControl flowControl) 
    {
        int value = 0;
        int index = _index + 1;
        switch (flowControl) {
            case FlowControl.NONE:
                break;
            case FlowControl.RTS_CTS:
                index |= 0x100;
                break;
            case FlowControl.DTR_DSR:
                index |= 0x200;
                break;
            case FlowControl.XON_XOFF_INLINE:
                value = CHAR_XON + (CHAR_XOFF << 8);
                index |= 0x400;
                break;
            default:
                throw new UnsupportedOperationException();
        }

        int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, SET_FLOW_CONTROL_REQUEST,
                value, index, null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0) throw new IOException("Set flow control failed: result=" + result);
        mFlowControl = flowControl;
    }

    public void purgeHwBuffers(bool purgeWriteBuffers, bool purgeReadBuffers)
    {
        if (purgeWriteBuffers)
        {
            int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, RESET_REQUEST,
                    RESET_PURGE_RX, _index + 0, null, 0, USB_WRITE_TIMEOUT_MILLIS);
            if (result != 0)
            {
                throw new IOException("Purge write buffer failed: result=" + result);
            }
        }

        if (purgeReadBuffers)
        {
            int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, RESET_REQUEST,
                    RESET_PURGE_TX, _index + 0, null, 0, USB_WRITE_TIMEOUT_MILLIS);
            if (result != 0)
            {
                throw new IOException("Purge read buffer failed: result=" + result);
            }
        }
    }

    public void setBreak(bool value)
    {
        int config = breakConfig;
        if (value) config |= 0x4000;
        int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, SET_DATA_REQUEST,
                config, _index + 0, null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Setting BREAK failed: result=" + result);
        }
    }

    public void setLatencyTimer(int latencyTime)
    {
        int result = deviceConnection.ControlTransfer(REQTYPE_HOST_TO_DEVICE, SET_LATENCY_TIMER_REQUEST,
                latencyTime, _index + 0, null, 0, USB_WRITE_TIMEOUT_MILLIS);
        if (result != 0)
        {
            throw new IOException("Set latency timer failed: result=" + result);
        }
    }

    public int getLatencyTimer()
    {
        byte[] data = new byte[1];
        int result = deviceConnection.ControlTransfer(REQTYPE_DEVICE_TO_HOST, GET_LATENCY_TIMER_REQUEST,
            0, _index + 0, data, data.Length, USB_WRITE_TIMEOUT_MILLIS);
        if (result != data.Length)
        {
            throw new IOException("Get latency timer failed: result=" + result);
        }
        return data[0];
    }



    //    @SuppressWarnings({ "unused"})
    //    public static Map<Integer, int[]> getSupportedDevices()
    //{
    //    final Map<Integer, int[]> supportedDevices = new LinkedHashMap<>();
    //    supportedDevices.put(UsbId.VENDOR_FTDI,
    //    new int[] {
    //        UsbId.FTDI_FT232R,
    //        UsbId.FTDI_FT232H,
    //                    UsbId.FTDI_FT2232H,
    //                    UsbId.FTDI_FT4232H,
    //                    UsbId.FTDI_FT231X,  // same ID for FT230X, FT231X, FT234XD
    //            });
    //    return supportedDevices;

}



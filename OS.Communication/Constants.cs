﻿
namespace OS.Communication;

public class Constants
{
    public const string MSG_SENDING = "Sending...";
    public const string MSG_CONNECTING = "Connecting...";
    public const string MSG_QUERYING = "Querying";
    public const string MSG_COMMUNICATING = "Communicating...";
    public const string MSG_DATA_RECEIVED = "Data Received";
    public const string MSG_DEVICE_FOUND = "Device Found";

    public const string PREFS_KEY_DEVICE_WIFI_MODE = "DeviceWifiType";
    public const string PREFS_BLUETOOTH_TYPE_DESCRIPTOR = @"Bluetooth/USB";
    public const string PREFS_WIFI_TYPE_DESCRIPTOR = "WiFi/Network";
    public const string PREFS_KEY_CANID = "CANDID";
    public const string PREFS_KEY_BLUETOOTH_DEVICE = "BluetoothDevice";
    public const string PREFS_KEY_IP_ADDRESS = "IPAddress";
    public const string PREFS_KEY_IP_PORT = "IPPort";
    public const string PREFS_KEY_DEVICE_COMM_TYPE = "CommunicationType";
    public const string PREFS_SERIAL_BAUD_RATE = "SerialBaudRate";

    public const string COMMUNICATION_DEVICE_NOT_SETUP = "*** Device Not Set ***";

    public const string DEFAULTS_WIFI_IPADDRESS = "192.168.0.10";
    public const int DEFAULTS_WIFI_PORT = 35000;
    public const int DEFAULT_COMM_NO_RESPONSE_TIMEOUT = 1500;
    public const int DEFAULT_COMM_NO_RESPONSE_RETRY_COUNT = 5;
    public const int DEFAULT_COMM_CONNECT_RETRY_COUNT = 3;
    //public const string DEFAULT_DIAG_FUNCADDR_CAN_ID_11 = "7DF";
    //public const string DEFAULT_TEST_CAN_ID_29 = "18DB33F1";




}
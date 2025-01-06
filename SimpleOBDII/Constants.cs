
namespace OS.OBDII;

public class Constants: OS.Communication.Constants
{
#if WINDOWS
public const bool ADS_ARE_ACTIVE = false;
#elif IOS
#if ADS_SUPPORTED // I created this - M.O.
    public const bool ADS_ARE_ACTIVE = true;
#else
    public const bool ADS_ARE_ACTIVE = false;
#endif
#elif ANDROID
#if ADS_SUPPORTED // I created this - M.O.
    public const bool ADS_ARE_ACTIVE = true;
#else
public const bool ADS_ARE_ACTIVE = false;
#endif
#endif

    public const string STRING_LOG_TAG = "SIMPLE_OBDII";

    // these are fore Windows platform UIs
    public const int MIN_WINDOW_WIDTH_WINDOWS = 600;
    public const int MIN_WINDOW_HEIGHT_WINDOWS = 750;

    public const bool SPECIAL_EDITION = false;
    public const bool DEV_EDITION = false;
    public const bool REQUIRE_APP_ID = false;

    public const string TEST_BANNER_AD_ID = "ca-app-pub-3940256099942544/6300978111";
    public const string REAL_BANNER_AD_ID = "";
    public const string BANNER_AD_ID = TEST_BANNER_AD_ID;




    public const int UI_RADIUS_STD = 10;

    public const string USER_PIDS_JSON_FILE = "userpids.json";
    public const string USER_TESTS_JSON_FILE = "usertests.json";
    public const string LOG_FILE_PATH = "SimpleOBDII_log.txt";
    public const int MAX_LOG_FILE_LENGTH = 10000;

    public const int LAUNCH_BUTTON_RESIZE_MAX_WINDOW_WIDTH = 2000;
    public const double LAUNCH_BUTTON_WIDTH_PROPORTION = .67;

    public const string APP_PROPERTY_KEY_ADD = "UserPIDSViewModel_add";
    public const string APP_PROPERTY_KEY_EDIT = "UserPIDSViewModel_edit";
    public const string APP_PROPERTY_KEY_INITIAL_VIEW = "UserPIDSViewModel_discover_pids";

    public const char CARRIAGE_RETURN = '\r';

    public const int DEFAULTS_PLOT_HEIGHT = 150;

    public const string DTC_CODES_GROUP_CONFIRMED = "Confirmed";
    public const string DTC_CODES_GROUP_PENDING = "Pending";
    public const string DTC_CODES_GROUP_PERMANENT = "Permanent";
    public const string DTC_CODES_GROUP_STORED = "Stored";
    public const string USER_PIDS_VM_CODE = "UserPIDSViewModel_code";


    public const string PREFS_KEY_PLATFORM_DESCRIPTION_IOS = "IOS";
    public const string PREFS_KEY_PLATFORM_DESCRIPTION_ANDROID = "Android";
    public const string PREFS_KEY_PLATFORM_DESCRIPTION_WINDOWS = "Windows";
    public const string PREFS_KEY_SELECTED_PROTOCOL = "SelectedProtocol";
    public const string PREFS_USE_HEADER = "USE_HEADER";
    public const string PREFS_USE_METRIC = "USE_METRIC";
    public const string PREFS_REMEMBER_VIN = "RememberVIN";
    public const string PREFS_KEY_PLOT_HEIGHT = "PLOT_HEIGHT";
    public const string PREFS_KEY_LAST_VALID_VIN = "LAST_VALID_VIN";
    public const string PREFS_KEY_MANUFACTURER = "Code Descriptions";
    public const string PREFS_KEY_APPLICATION_REGISTRATION_ANSWER = "AppInstallationHash";
    public const string PREFS_KEY_SHOW_CODES = "ShowActiveTestCodes";

    public const int MAX_PLOT_HEIGHT = 500;
    public const int MIN_PLOT_HEIGHT = 75;


    public const string DEFAULTS_DESCRIPTOR_MONITORS_SINCE_DTCS_CLEARED = "Since DTCs Cleared";
    public const uint DEFAULT_DIAG_FUNCADDR_CAN_ID_11 = 0x7DF;
    public const uint DEFAULT_DIAG_FUNCADDR_CAN_ID_29 = 0x18DB33F1;
    public const uint EXPANDED_CODE_MASK = 0xF000;

    public const string DONGLE_UNRECOGNIZED_COMMAND_STRING = "Command not recognized";
    public const string MSG_NOT_SET = "<Not Set>";
    public const string MSG_NO_RESPONSE_VEHICLE = "No Response From Vehicle";
    public const string MSG_NO_RESPONSE_SYSTEM = "No Response From System";
    public const string MSG_NOT_SELECTED = "Not Selected";
    public const string MSG_OK = "Ok";
    public const string MSG_SUCCESS = "Success";
    public const string MSG_ERROR = "** ERROR **";
    public const string MSG_NOT_APPLICABLE = "[N/A]";
    public const string STRING_ALL_TESTS_COMPLETE = "*** All Tests Complete ***";
    public const string STRING_NO_CODES_FOUND = "No Codes Found";
    public const string STRING_NO_PROTOCOL = "<None Detected>";
    public const string STRING_NOT_SET = "<Not Set>";


    public const string STRING_ESTABLISHING_LINK = "Establishing Link...";
    public const string STRING_INITIALIZING = "Initializing...";
    public const string STRING_COMM_BLINKING_VISUAL_ELEMENT = "BlinkingVisualElement";
    public const string STRING_NO_DATA = "** No Data **";
    public const string STRING_BLANK_LINE = "-----";
    public const string STRING_BACK = "Back";
    public const string STRING_STOP = "Stop";
    public const string STRING_STOPPED = "Stopped";
    public const string STRING_START = "Start";
    public const string STRING_PAUSE = "Pause";
    public const string STRING_RESTART = "Restart";
    public const string STRING_CONTINUE = "Continue";
    public const string STRING_GO = "Go";
    public const string STRING_BUSY = "Device Busy, Try Again";

    public const double STYLES_BREAKPOINT_0 = 340;
    public const int COMM_IDLE_TIMEOUT = 5000;
    public const int SLIDER_TIMEOUT = 225;
    public const int PLOT_POINT_WIDTH = 100;
    public const int PLOT_MAX_COUNT = 4;

    public const int APP_LIMIT_MAX_USER_PID_ROWS = 3;
    public const int APP_LIMIT_MAX_FF_PID_ROWS = 6;
    public const bool FORCE_LIMITS = false;




    public const string PROTOCOL_AUTO = "Auto";
    public const string PROTOCOL_SAE_J1850_PWM = "SAE J1850 PWM";
    public const string PROTOCOL_SAE_J1850_VPW = "SAE J1850 VPW";
    public const string PROTOCOL_ISO_9141_2 = "ISO 9141-2";
    public const string PROTOCOL_ISO_14230_4_KWP_SLOW = "ISO 14230-4 KWP Slow Init";
    public const string PROTOCOL_ISO_14230_4_KWP = "ISO 14230-4 KWP";
    public const string PROTOCOL_ISO_15765_4_CAN_11_500 = "ISO 15765-4 (11 bit, 500k)";
    public const string PROTOCOL_ISO_15765_4_CAN_29_500 = "ISO 15765-4 (29 bit, 500k)";
    public const string PROTOCOL_ISO_15765_4_CAN_11_250 = "ISO 15765-4 (11 bit, 250k)";
    public const string PROTOCOL_ISO_15765_4_CAN_29_250 = "ISO 15765-4 (29 bit, 250k)";
    public const string PROTOCOL_SAE_J1939_CAN_29_250 = "SAE J1939 CAN (29 bit, 250k)";


}

Rem You will need the IP address of the device.
Rem A statically assigned IP address for the device on the local network helps to ensure this file always works
Rem The Android SDK platform-tools folder location must be correct
cd C:\Program Files (x86)\Android\android-sdk\platform-tools
adb tcpip 5555
adb connect 192.168.1.111:5555
adb devices
Rem This is a pause, a wait
timeout /t -1
Rem You will need the IP address of the device
cd C:\Program Files (x86)\Android\android-sdk\platform-tools
adb tcpip 5555
adb connect 192.168.1.144:5555
adb devices
Rem This is a pause, a wait
timeout /t -1
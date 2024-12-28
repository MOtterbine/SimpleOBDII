## RUN THIS RIGHT FROM HERE as Administrator (Documents folder)

$winPlatform = 'win10-x64';
$winVer = 'net8.0-windows10.0.22621.0';


## publishing the actual, usable, unpackaged app
dotnet publish "..\SimpleOBDII\SimpleOBDII.csproj" -f $winVer -c Release -p:RuntimeIdentifierOverride=$winPlatform -p:WindowsPackageType=None



Read-Host -Prompt "Press <Enter> to continue"
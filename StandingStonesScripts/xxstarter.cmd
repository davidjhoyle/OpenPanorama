Set src=f:
Set dst=f:
Set MasterPC=STONEYWONEY
set srcfolder=UK Stone Circles

If NOT X%LIDARCache%X==XX Goto SkipLidarCache
Set LIDARCache=%src%\LIDARCache\
:SkipLidarCache

If X%LocalLidar%X==XX Goto SkipLidar
MD %LocalLidar%
XCopy /d /s /r /y "%src%\LIDAR\*.*" "%LocalLidar%\"
XCopy /d /s /r /y "%src%\LIDARCACHE\*.*" "%LocalLidar%\LIDARCACHE\"
:SkipLidar

REM Setup the OS Map Data and SRTM data copies locally.
Set SRTMDir=%APPDATA%\SRTM Data
Set OSDir=%APPDATA%\OS Terrain 50\terr50_cgml_gb\data

md "%SRTMDir%"
md "%OSDir%"
md "%LIDARCache%"

XCopy /d /s /r /y "%src%\SRTM Data\*.*" "%SRTMDir%\"
XCopy /d /s /r /y "%src%\OS Terrain 50\terr50_cgml_gb\data\*.*" "%OSDir%\"

copy /y "%src%\%srcfolder%\DownloadCSVs.ps1
powershell .\DownloadCSVs.ps1
copy csvs\MegP_Ireland_(Northern).csv

copy /y "f:\UK Stone Circles\CSVs\MegP_Ireland_(Northern).csv" "f:\UK Stone Circles\CSVs\MegP_NorthernIreland.csv";
copy /y "f:\UK Stone Circles\CSVs\MegP_Ireland_(Republic_of).csv" "f:\UK Stone Circles\CSVs\MegP_Ireland.csv";

FOR /L %%i IN (1,1,%1) DO md "Proc%%i"
FOR /L %%i IN (1,1,%1) DO Copy "%src%\%srcfolder%\XXAll.cmd" "Proc%%i"
FOR /L %%i IN (1,1,%1) DO timeout /T 2 && start /BELOWNORMAL  Proc%%i\XXAll.cmd Proc%%i %%i %2

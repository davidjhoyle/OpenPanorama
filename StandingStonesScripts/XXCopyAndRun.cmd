Set CC=%1
Set Country=%2

If X%CC%X==XX Goto EndMe
If X%Country%X==XX Goto EndMe
If X%src%X==XX Goto ENDME
If X%dst%X==XX Goto ENDME

Echo Country %Country% Shortcode %CC%

Set CSVsDir=CSVs
md "%CSVsDir%"
XCopy /d /y "%src%\%srcfolder%\%CSVsDir%\*.csv" "%CSVsDir%"

REM Special UK CSV handling.
type "%CSVsDir%"\MegP_Wales.csv > "%CSVsDir%"\MegP_GreatBritain.csv
type "%CSVsDir%"\MegP_England.csv >> "%CSVsDir%"\MegP_GreatBritain.csv
type "%CSVsDir%"\MegP_Scotland.csv >> "%CSVsDir%"\MegP_GreatBritain.csv
type "%CSVsDir%"\MegP_NorthernIreland.csv >> "%CSVsDir%"\MegP_GreatBritain.csv
type "%CSVsDir%"\ThomUK.csv >> "%CSVsDir%"\MegP_GreatBritain.csv
REM Special Channel Islands handling. Alderney is part of Jersey.
type "%CSVsDir%"\MegP_Alderney.csv >> "%CSVsDir%"\MegP_Jersey.csv


REM This is the old Panorama App
Set BinDir=Panorama
Set EXE=%BinDir%\Panorama.exe
md "%BinDir%"
XCopy /d /y "%src%\%srcfolder%\Panorama\*.*" %BinDir%

REM This is the new Panorama App
Set BinDir=OpenPanorama
Set EXE=%BinDir%\OpenPanorama.exe
md "%BinDir%"
XCopy /d /y /s "%src%\%srcfolder%\OpenPanorama\*.*" %BinDir%


copy /y "%src%\%srcfolder%\XXDoAllBulk.cmd" XXDoAllBulk.cmd


Set SRTMDir=%USERPROFILE%\Appdata\roaming\SRTM Data
Set OSDir=%USERPROFILE%\Appdata\roaming\OS Terrain 50\terr50_cgml_gb\data


Set LIDARFolder=%src%\LIDAR\
If NOT X%LIDARCache%X==XX Goto SkipLidarCache
Set LIDARCache=%src%\LIDARCache\
:SkipLidarCache

If X%LocalLidar%X==XX Goto SkipLidar
Set LIDARFolder=%LocalLidar%\
Set LIDARCache=%LocalLidar%\LIDARCACHE\
:SkipLidar

set MyCSV=%CSVsDir%\MegP_%Country%.csv
Set AllCSVs=%MyCSV%
Set Group=All
Set CSV=%CC%%Group%.csv
Set Out=%CC%%Group%ImagesSRTM.txt



set baseFolder=%dst%\standingstonesorg
Set DstFolder=%baseFolder%\%CC%

REM Enable Cross Quarters for all sites.
Set XQ= -xq

REM Set the ages for all sites - the 0 at the end is 2000AD.
Set ages=3750,4000,4500,5000,0
IF X%CC%X==XtrX Set ages=4000,5000,6000,8000,10000,0

REM Settings for use of the OS Maps and MaxJobs - the latter is 1 for sites with LIDAR due to memroy leak issues in the ZIP library for large files.
Set OSLIDAR= -nospots -MaxJobs 20
IF X%Country%X==XukX Set OSLIDAR= -contours -MaxJobs 20 -nospots 
IF X%Country%X==XEnglandX Set OSLIDAR= -contours -lidar -MaxJobs 1 -min 3 -nospots -minPPDistance 300
IF X%Country%X==XWalesX Set OSLIDAR= -contours -lidar -MaxJobs 1  -min 3 -nospots -minPPDistance 300
IF X%Country%X==XScotlandX Set OSLIDAR= -contours -lidar -MaxJobs 1 -min 50  -nospots -minPPDistance 300
IF X%Country%X==XGreatBritainX Set OSLIDAR= -contours -lidar -MaxJobs 1 -nospots 
If X%CC%X==XxxX Set OSLIDAR= -contours -lidar -MaxJobs 1 -min 50
IF X%CC%X==XieX Set OSLIDAR= -lidar -MaxJobs 20 -nospots -min 20 
IF X%CC%X==XbdX Set OSLIDAR= -lidar -MaxJobs 1 -nospots

IF X%CC%X==XnoX Set OSLIDAR= -MaxJobs 1 -nospots -min 15 -lidarrange 20000 -lidar -LIDARRes 10  -LIDARSize 100000 


Set maxDist=150000
If X%CC%X==XxxX Set maxDist=200000


Rem General flags for all jobs.
Set Flags=-LidarDebug 
Set Flags=-HiResHorizon -PP -Correlate -PPJSON -PPCSV -CorrelateCSV -CorrelateJSON -Skim -SkimCSV -SkimJSON -DeclinationPeakCSV -DeclinationPeakJSON 
Set Flags=%Flags% -country %CC% -srtm -lidarrange 12000 -lidarfolder %LIDARFolder% -lidarcache %LIDARCache% -max %maxDist% %OSLIDAR% -CountryLong %Country% 
Set Flags=%Flags% -NegRange 5 -mr -pp -ch -t -AllMoon -Solstice -Equinox -stellarium -hzCSV %XQ% -CacheOSMap 20 
Rem Colour settings
Set Flags=%Flags% -ColourSea 000000 -ColourSky 00008B -ColourBase 006400 -ColourTops A52A2A -ColourDistant 696969 -ColourSun FFFF00 -ColourMoon 808080
Set Flags=%Flags% -ColourSaMText 000000 -ColourTitle FFFFFF -ColourCairns 006400 -ColourStones 2F4F4F -ColourSpotHeight 8B0000 -ColourReticleText 000000
Set Flags=%Flags% -ColourReticleBackground FFFFFF -ColourReticleMajor 808080 -ColourReticleMinor D3D3D3 -ColourReticleHorizontal 000000 -ColourContrast 1.3
Set Flags=%Flags% -SiteTypes "Stone Circle,Ring Cairn,Standing Stone (Menhir),Standing Stones,Stone Row / Alignment,Henge,Multiple Stone Rows / Avenue,Viewpoint,Ancient Temple,Passage Grave,Court Tomb,Portal Tomb,Chambered Tomb,Wedge Tomb,Chambered Cairn,Clava Cairn,Burial Chamber or Dolmen,Timber Circle,Cursus,Causewayed Enclosure,Long Barrow,Ancient Palace,Holed Stone"



Rem Handle Grids
IF X%Country%X==XWalesX goto FineGrid
IF X%Country%X==XEnglandX goto FineGrid
IF X%Country%X==XScotlandX goto FineGrid
IF X%Country%X==XGreatBritainX goto FineGrid
IF X%Country%X==XukX goto FineGrid
IF X%Country%X==XieX goto FineGrid
Goto SimpleGrid
:FineGrid
Set Flags=%Flags% -allgrids
Goto DoneGrid
:SimpleGrid
Set Flags=%Flags% -grid
:DoneGrid



Rem SPecial case handling for Turkey site types.
rem IF X%CC%X==XtrX Set Flags=%Flags% -SiteTypes "Stone Circle,Ring Cairn,Standing Stone (Menhir),Standing Stones,Stone Row / Alignment,Henge,Multiple Stone Rows / Avenue,Viewpoint,Ancient Temple,Passage Grave,Court Tomb,Portal Tomb,Chambered Tomb,Wedge Tomb,Chambered Cairn,Clava Cairn,Burial Chamber or Dolmen,Timber Circle,Cursus,Causewayed Enclosure,Long Barrow,Ancient Palace"


REM Master PC has 32GB RAM so can cache more.
If x%COMPUTERNAME%x==x%MasterPC%x Set Flags=%Flags% -CacheLIDAR 8

Rem Remember the flags without the JSON generation.
Set NoJSONFlags=%Flags%

Rem if this is not the first Process and not on the Master PC then skip - only the master needs to generate JSON.
If Not x%COMPUTERNAME%x==x%MasterPC%x IF X%Country%X==XGreatBritainX Goto EndMe

Rem if this is the first Process and on the Master PC and the first time run for this country then generate JSON and SID files.
If Not x%COMPUTERNAME%x==x%MasterPC%x Goto SkipJSON
If X%ProcID%X==XX Goto DoJSON
If X%ProcID%X==X1X Goto DoJSON
Goto SkipJSON

:DoJSON
Rem Add the additional JSON and SID handling falgs for the process 1 on the Master PC the first time through.
Set Flags=%Flags% -json "%DstFolder%\%Country%.txt" -locs -county areas%Country%.txt -countyHTML %Country%.html
Set Flags=%Flags% -SidFiles "%baseFolder%\sids" 
REM Next line to force all horizon maps to get rebuilt.
rem Set Flags=%Flags% -rch 
:SkipJSON

:ErrorRetry
"%EXE%" -srtmFolder "%SRTMDir%" -osFolder "%OSDir%" -out "%DstFolder%" -threads 1 -a "%ages%" -csv "%AllCSVs%" %Flags% 
Set errlevel=%errorlevel%
echo %errlevel% occured >> "ErrorRetry.txt"
echo "GetExactTime" | time | findstr "current" >> "ErrorRetry.txt"
date /t >> "ErrorRetry.txt"

Set Flags=%NoJSONFlags%

if x%errlevel%x==x-536852669x Goto ErrorRetry
if x%errlevel%x==x-532462766x Goto ErrorRetry
if x%errlevel%x==x999x Goto ErrorRetry


:EndMe
Echo XXCopyAndRun CountryCode (FR, etc)

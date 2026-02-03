cd /d f:\standingstonesorg\uk

md "C:\Users\david\Desktop\Standing Stones\Correlations\"
del "C:\Users\david\Desktop\Standing Stones\Correlations\*_correlation.csv"
for /r %%x in (*_correlation.csv) do @copy /y "%%x" "C:\Users\david\Desktop\Standing Stones\Correlations\"
type "C:\Users\david\Desktop\Standing Stones\Correlations\*_correlation.csv" > "C:\Users\david\Desktop\Standing Stones\Correlations\AllCorrelations.csv"
echo Name,GridRef,MonType,Age,Region,Country,CorrelationValueWeighted,CorrelationValue,SlopeWeighted,ElevDiffWeighted,DeltaBearing,RASPLatitude,RASPLongitude,Body,TCB,RASPBearing,RASPElevation,RASPSetting,IsPeak,SlopeWeight,ElevDiffWeaight > f:\standingstonesorg\uk\AllCorrelations.csv
findstr /v /i gridref "C:\Users\david\Desktop\Standing Stones\Correlations\AllCorrelations.csv" >> f:\standingstonesorg\uk\AllCorrelations.csv


md "C:\Users\david\Desktop\Standing Stones\Skims\"
del "C:\Users\david\Desktop\Standing Stones\Skims\*_skim.csv"
for /r %%x in (*_skim.csv) do @copy /y "%%x" "C:\Users\david\Desktop\Standing Stones\Skims\"
type "C:\Users\david\Desktop\Standing Stones\Skims\*_skim.csv" > "C:\Users\david\Desktop\Standing Stones\Skims\AllSkims.csv"
echo Name,GridRef,MonType,Age,Region,Country,Body,TCB,Setting,Bearing1,Elevation1,Latitude1,Longitude1,Bearing2,Elevation2,Latitude2,Longitude2 > f:\standingstonesorg\uk\AllSkims.csv
findstr /v /i gridref "C:\Users\david\Desktop\Standing Stones\Skims\AllSkims.csv" >> f:\standingstonesorg\uk\AllSkims.csv




md "C:\Users\david\Desktop\Standing Stones\Peaks\"
del "C:\Users\david\Desktop\Standing Stones\Peaks\*_Peaks.csv"
for /r %%x in (*_Peaks.csv) do @copy /y "%%x" "C:\Users\david\Desktop\Standing Stones\Peaks\"
type "C:\Users\david\Desktop\Standing Stones\Peaks\*_Peaks.csv" > "C:\Users\david\Desktop\Standing Stones\Peaks\AllPeaks.csv"
echo Name,GridRef,MonType,Region,Country,IsPeak,SlopeWeight,ElevDiffWeight,PeakIndex,HorizonLatitude,HorizonLongitude,HorizonElevation,HorizonDistance,HorizonBearing > f:\standingstonesorg\uk\AllPeaks.csv
findstr /v /i gridref "C:\Users\david\Desktop\Standing Stones\Peaks\AllPeaks.csv" >> f:\standingstonesorg\uk\AllPeaks.csv



md "C:\Users\david\Desktop\Standing Stones\declpeaks\"
del "C:\Users\david\Desktop\Standing Stones\declpeaks\*_declpeaks.csv"
for /r %%x in (*_declpeaks.csv) do @copy /y "%%x" "C:\Users\david\Desktop\Standing Stones\declpeaks\"
type "C:\Users\david\Desktop\Standing Stones\declpeaks\*_declpeaks.csv" > "C:\Users\david\Desktop\Standing Stones\declpeaks\Alldeclpeaks.csv"
echo Name,GridRef,MonType,Region,Country,IsPeak,SkimWidth,ElevDiffWeight,PeakIndex,HorizonLatitude,HorizonLongitude,Declination,HorizonDistance,HorizonBearing,Quality,NotchDeclin > f:\standingstonesorg\uk\Alldeclpeaks.csv
findstr /v /i gridref "C:\Users\david\Desktop\Standing Stones\declpeaks\Alldeclpeaks.csv" >> f:\standingstonesorg\uk\Alldeclpeaks.csv


c:


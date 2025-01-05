
$BaseURL="https://environment.data.gov.uk"

$JobURL= $BaseURL + "/arcgis/rest/services/gp/DataDownload/GPServer/DataDownload/submitJob"
$jobParams = "?f=json&SourceToken=&OutputFormat=0&RequestMode=SURVEY&"

$downloadJob = $BaseURL + "/arcgis/rest/services/gp/DataDownload/GPServer/DataDownload/jobs/"
# jf22ae0ffcad149618560f76d92c0913c?f=json&dojo.preventCache=1589465362852"


$ResultsURL = $BaseURL + "/arcgis/rest/directories/arcgisjobs/gp/datadownload_gpserver/"
$ResultsSuffix = "/scratch/results.json"

$DestFolder = "F:\LIDAR\England\"

# Brighton X 190421 Y 054383
# penzance X 147092 Y 030263
# orkney   x 335087 Y 1006615
# Scilly   x 086338 Y 006973
#EastCoast x 663756 Y 303128
#Shetland  x 471351 Y 1221790


$XXMin = 0000000
$YYMin = 0000000
$XXMax = 0700000
$YYMax = 1300000
 
# Start from below the lakes and up to the border...
#$XXMin = 260000
#$YYMin = 440000
#$XXMax = 600000
#$YYMax = 700000


#$XXMin = 0310000
#$YYMin = 0260000
#$XXMax = 0310000
#$YYMax = 0440000


#$XXMin = 420000
#$YYMin = 370000
#$XXMax = 440000
#$YYMax = 390000

$XInc = 020000
$YInc = 020000

$wc = New-Object System.Net.WebClient
$wcsiz = New-Object System.Net.WebClient


Function DownloadTile( $xxx, $yyy )
{

    $x1 = $xxx
    $y1 = $yyy
    $x2 = $xxx + $XInc -1 
    $y2 = $yyy
    $x3 = $xxx + $XInc -1 
    $y3 = $yyy + $YInc -1
    $x4 = $xxx
    $y4 = $yyy + $YInc -1

    $AOI = "{""geometryType"":""esriGeometryPolygon"",""features"":[{""geometry"":{""rings"":[["
    $AOI += "[$x1,$y1],[$x2,$y2],[$x3,$y3],[$x4,$y4],[$x1,$y1]"
    $AOI += "]],""spatialReference"":{""wkid"":27700,""latestWkid"":27700}}}],""sr"":{""wkid"":27700,""latestWkid"":27700}}"

    Write-Host "Unencoded $AOI"

    $AOI = [uri]::EscapeDataString($AOI) 

    Write-Host "Encoded $AOI"


    $url = "$JobURL$jobParams"
    $url += "AOI=$AOI"

    Write-Host "DownloadString  $url"
    $output = $wc.DownloadString( $url )
#    Write-Host "DownloadString  output $output"

    $jobdata = ConvertFrom-Json $output

    Write-Host "JSON Job Data $jobdata"

    for ( $i=0; $i -lt 100; $i += 1 )
    {
        Start-Sleep -s 1
        $url = $downloadJob
        $url += $jobdata.jobId
        $url += "?f=json&dojo.preventCache=999$i"

        Write-Host "DownloadString  $url"
        $output = $wc.DownloadString( $url )
#        Write-Host "DownloadString  output $output"

        $jobstatusdata = ConvertFrom-Json $output
        Write-Host "JSON Job Status Data $jobstatusdata"

        if ( $jobstatusdata.jobStatus -eq "esriJobSucceeded" )
        {
            $url = $ResultsURL
            $url += $jobdata.jobId
            $url += $ResultsSuffix

            Write-Host "DownloadString  $url"
            $output = $wc.DownloadString( $url )
#            Write-Host "DownloadString output $output"

            $ajobstatusdata = ConvertFrom-Json $output

            for ( $d=0; $d -lt $ajobstatusdata.data.Length; $d +=1 )
            {
                for ( $y=0; $y -lt $ajobstatusdata.data[$d].years.Length; $y += 1 )
                {
                    for ( $r=0; $r -lt $ajobstatusdata.data[$d].years[$y].resolutions.Length; $r += 1 )
                    {
                        for ( $t=0; $t -lt $ajobstatusdata.data[$d].years[$y].resolutions[$r].tiles.Length; $t += 1 )
                        {
                            $myurl = $ajobstatusdata.data[$d].years[$y].resolutions[$r].tiles[$t].url

                            $matcher2 = "/LIDARCOMP/"
                            $fileindx2 = $myurl.Indexof( $matcher2 )

                            # https://environment.data.gov.uk/UserDownloads/interactive/e3e272be1ccb4875b88727b6a2b0c9bd27748/LIDARCOMP/LIDAR-DTM-1m-2022-SW32ne.zip

                            $matcher3 = "-1M-"
                            $fileindx3 = $myurl.ToUpper().Indexof( $matcher3 )

                            $matcher4 = "-2M-"
                            $fileindx4 = $myurl.ToUpper().Indexof( $matcher4 )

                            $matcher = "/NLP/"
                            $fileindx = $myurl.Indexof( $matcher )

                            if ( ($fileindx -gt 0 ) -or (($fileindx2 -gt 0) -and ($fileindx3 -gt 0)) -or (($fileindx2 -gt 0) -and ($fileindx4 -gt 0)))
                            {
                                if ($fileindx2 -gt 0)
                                {
                                    $fileindx = $fileindx2
                                    $matcher = $matcher2
                                }

                                $filnam = $DestFolder + $myurl.Substring( $fileindx + $matcher.Length )
                                #Write-Host "Possible download $filnam"

                                if ( ($filnam.Indexof( "-DTM-" ) -gt 0 )) #  -gt 0 -or $filnam.Indexof( "-DSM-" ) -gt 0 ))
                                {
                                    if ( ! ([System.IO.File]::Exists($filnam)))
                                    {
                                        Write-Host "URLs to Download $myurl"
                                        $wc.DownloadFile( $myurl, $filnam )
                                    }
                                    else
                                    {
                                        $tmpstream = $wcsiz.OpenRead($myurl);
                                        $bytes_total = [uint64] (($wcsiz.ResponseHeaders["Content-Length"]));
                                        $wcsiz.CancelAsync();

                                        $local_file_size = [uint64] ((Get-Item $filnam).length);

                                        if ( $bytes_total -ne  $local_file_size )
                                        {
                                            Write-Host "File Sizes Differ - Web Size $bytes_total local size $local_file_size - Download $myurl";
                                            $wc.DownloadFile( $myurl, $filnam )
                                        }
                                        else
                                        {
                                            Write-Host "File already downloaded $filnam"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            break;
        }
    }
}



for ($yy=$YYMin; $yy -lt $YYMax; $yy += $YInc )
{
    for ($xx=$XXMin; $xx -lt $XXMax; $xx += $XInc )
    {
         DownloadTile $xx $yy
    }
}





# https://remotesensingdata.gov.scot/data#/download
# 

# https://scotland-gov-lidar.s3-eu-west-1.amazonaws.com/phase-1/dtm/27700/gridded/HY20_1M_DTM_PHASE1.tif?
# https://scotland-gov-lidar.s3-eu-west-1.amazonaws.com/phase-2/dtm/27700/gridded/HU32_1M_DTM_PHASE2.tif?
# https://scotland-gov-lidar.s3-eu-west-1.amazonaws.com/phase-3/dtm/27700/gridded/NS47NE_50CM_DTM_PHASE3.tif?
# https://scotland-gov-lidar.s3-eu-west-1.amazonaws.com/phase-4/dtm/27700/gridded/NT20NW_50CM_DTM_PHASE4.tif?
# https://scotland-gov-lidar.s3-eu-west-1.amazonaws.com/outer-hebrides/2019/dtm/50cm/27700/gridded/NF71SE_50CM_DTM_OUTERHEBRIDES2019.tif?
# https://srsp-open-data.s3.eu-west-2.amazonaws.com/lidar/outer-hebrides/2019/dtm/50cm/27700/gridded/NA91NE_50CM_DTM_OUTERHEBRIDES2019.tif
# https://srsp-open-data.s3.eu-west-2.amazonaws.com/lidar/phase-4/dtm/27700/gridded/NO11NE_50CM_DTM_PHASE4.tif
# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/phase-2/dtm/27700/gridded/NT20NW_50CM_DTM_PHASE2.tif?
# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/phase-5/dtm/27700/gridded/NO10NE_50CM_DTM_PHASE5.tif

# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/phase-3/dtm/27700/gridded/NF71SE_50CM_DTM_PHASE3.tif

# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/phase-5/dtm/27700/gridded/NO20NE_50CM_DTM_PHASE5.tif
# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/phase-6/dtm/27700/gridded/NS16NE_50CM_DTM_PHASE6.tif
# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/phase-6/dtm/27700/gridded/HO58_1M_DTM_PHASE6.tif?

# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/hes/hes_2010/dtm/27700/gridded/NS7793_50CM_DTM_HES2010.tif?
# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/hes/hes-2010s10/dtm/27700/gridded/HY1700_50CM_DTM_HESS10_2010.tif?
# https://scotland-gov-lidar-beta.s3-eu-west-1.amazonaws.com/hes/hes-2016/dtm/27700/gridded/NM2522_25CM_DTM_HES2016.tif?
# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/hes/hes-2017/dtm/27700/gridded/NM8734_25CM_DTM_HES_2017.tif?
# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/hes/hes-2017sp3/dtm/27700/gridded/HY2216_50CM_DTM_HES_2017SP3.tif?


$BaseURL="https://scotland-gov-lidar.s3-eu-west-1.amazonaws.com/"
$BaseURL="https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/"
$hes2916BaseUrl="https://scotland-gov-lidar-beta.s3-eu-west-1.amazonaws.com/"
$datasets = @('phase-5/dtm','phase-6/dtm','phase-1/dtm','phase-2/dtm','phase-3/dtm','phase-4/dtm', 'outer-hebrides/2019/dtm/50cm', 'hes/hes_2010/dtm', 'hes/hes-2010s10/dtm', 'hes/hes-2016/dtm', 'hes/hes-2017/dtm', 'hes/hes-2017sp3/dtm' )
$datasetsuffixes = @('PHASE5','PHASE6','PHASE1','PHASE2','PHASE3','PHASE4', 'OUTERHEBRIDES2019', 'HES2010', 'HESS10_2010', 'HES2016', 'HES_2017', 'HES_2017SP3')
$datasetsizes = @('50CM','50CM','1M','1M','50CM','50CM','50CM', '50CM', '50CM', '25CM', '25CM', '50CM')
$datapaddders = @('0',   '0',   '0', '0', '0',   '0',   '0',    '00',   '00',   '00',   '00',   '00')

$corners = @('NE','NW','SE','SW')


$GridSquares = @(   'HO','HP',
		            'HT','HU',
	      'HW','HX','HY','HZ',
     'NA','NB','NC','ND','NE',
     'NF','NG','NH','NJ','NK',
     'NL','NM','NN','NO','NP',
	      'NR','NS','NT','NU',
	      'NW','NX','NY','NZ','OV')

$GridSquares = @(   'NR' )

$DestFolder = "F:\LIDAR\Scotland\"


$wc = New-Object System.Net.WebClient
$wcsiz = New-Object System.Net.WebClient


Function DownloadTile( $xxx, $yyy, $mygrid, $myset, $mysuffix, $datasetsize, $mycorner, $paddder)
{
    $myfilename = $mygrid + $xxx.ToString($paddder) + $yyy.ToString($paddder) + $mycorner + "_" + $datasetsize + "_DTM_" + $mysuffix + ".tif"
    $url = $BaseURL + $myset + "/27700/gridded/" + $myfilename # + "?"
    if ( $url.contains("hes-2016"))
	{
        $url = $hes2916BaseUrl + $myset + "/27700/gridded/" + $myfilename # + "?"
    }
    $fullfilename = $DestFolder + $myfilename

    if (!(Test-Path $fullfilename))
    {
        Write-Host "DownloadFile $url $fullfilename"
        $wc.DownloadFile( $url, $fullfilename )
    }
    else
    {
        $tmpstream = $wcsiz.OpenRead($url);
        $bytes_total = [uint64] (($wcsiz.ResponseHeaders["Content-Length"]));
        $wcsiz.CancelAsync();

        if ( $bytes_total -gt 0 )
        {
            $local_file_size = [uint64] ((Get-Item $fullfilename).length);

            if ( $bytes_total -ne  $local_file_size )
            {
                Write-Host "File Sizes Differ - Web Size $bytes_total local size $local_file_size - Download $url";
                $wc.DownloadFile( $url, $fullfilename )
            }
            else
            {
                Write-Host "File Already Downloaded - Skipping $fullfilename"
            }
        }
        else
        {
            Write-Host "Source Zero Bytes - Skipping $fullfilename"
        }
    }
}



Function NewDownloadTile( $url )
{
    $myfilename = $url.Substring($url.lastIndexOf('/') + 1)
    $fullfilename = $DestFolder + $myfilename

    if (!(Test-Path $fullfilename))
    {
        Write-Host "DownloadFile $url $fullfilename"
        $wc.DownloadFile( $url, $fullfilename )
    }
    else
    {
        $tmpstream = $wcsiz.OpenRead($url);
        $bytes_total = [uint64] (($wcsiz.ResponseHeaders["Content-Length"]));
        $wcsiz.CancelAsync();

        if ( $bytes_total -gt 0 )
        {
            $local_file_size = [uint64] ((Get-Item $fullfilename).length);

            if ( $bytes_total -ne  $local_file_size )
            {
                Write-Host "File Sizes Differ - Web Size $bytes_total local size $local_file_size - Download $url";
                $wc.DownloadFile( $url, $fullfilename )
            }
            else
            {
                Write-Host "File Already Downloaded - Skipping $fullfilename"
            }
        }
        else
        {
            Write-Host "Source Zero Bytes - Skipping $fullfilename"
        }
    }
}


# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/phase-3/dtm/27700/gridded/NF71SE_50CM_DTM_PHASE3.tif
# https://srsp-open-data.s3-eu-west-2.amazonaws.com/lidar/phase-5/dtm/27700/gridded/NO10NE_50CM_DTM_PHASE5.tif
# DownloadTile 1 0 "NO" $datasets[0] $datasetsuffixes[0] $datasetsizes[0] "NE" $datapaddders[0]

  
#DownloadTile 2 0 "HY" $datasets[0] $datasetsuffixes[0] $datasetsizes[0] ""
#DownloadTile 3 2 "HU" $datasets[1] $datasetsuffixes[1] $datasetsizes[1] ""
#DownloadTile 4 7 "NS" $datasets[2] $datasetsuffixes[2] $datasetsizes[2] "NE"
#DownloadTile 2 0 "NT" $datasets[3] $datasetsuffixes[3] $datasetsizes[3] "NW"
#DownloadTile 7 1 "NF" $datasets[4] $datasetsuffixes[4] $datasetsizes[4] "SE"


$productURL = "https://srsp-catalog.jncc.gov.uk/search/product"
# POST JSON
$GetCollectionsJSON = "{ ""collections"" : [ ""scotland-gov/lidar/ogc"" ] }"


$AllCollectionsResp = Invoke-WebRequest -Uri $productURL -Method POST -Body ($GetCollectionsJSON) -ContentType "application/json" -UserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/112.0" 

$AllCollections = $AllCollectionsResp.Content | ConvertFrom-Json

foreach( $col in $AllCollections.result )
{
    $colname = $col.data.product.catalog.collection

    if (-not $colname.Contains("dtm" ))
    {
        echo "Collection $colname Not dtm Skipped"
        continue;
    }

    if ( $colname.Contains("laz" ))
    {
        echo "Collection $colname laz Skipped"
        continue;
    }

    echo "Collection $colname Processing"

    $COllectionJSON = "
                {
	                ""collections"": [
		                ""$colname""
	                ],
	                ""footprint"": ""POLYGON((-9.514 54.265, -9.514 61.27, 2.747 61.27, 2.747 54.265, -9.514 54.265))"",
	                ""limit"": 10000,
	                ""offset"": 0,
	                ""spatialop"": ""intersects""
                }"


    $aCollectionsResp = Invoke-WebRequest -Uri $productURL -Method POST -Body ($COllectionJSON) -ContentType "application/json" -UserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/112.0" 
    $aCollection = $aCollectionsResp.Content | ConvertFrom-Json

    foreach( $colres in $aCollection.result )
    {
        $url = $colres.data.product.http.url

        NewDownloadTile $url
    }

}


exit

foreach( $grid in $GridSquares )
{
    Write-Host "Grid = $grid"

    for( $set=9; $set -lt $datasets.Count; $set +=1 )
    {
        $outstr = "Set = $set " + $datasets[$set] + " " + $datasetsuffixes[$set] + " " + $datasetsizes[$set]
        Write-Host $outstr 

        $maxxy = 10
        if ( $datasets[$set].contains( "hes" ))
        {
            $maxxy = 100
        }

        for ($xx=0; $xx -lt $maxxy; $xx += 1 )
        {
            for ($yy=0; $yy -lt $maxxy; $yy += 1 )
            {
                if ( $datasetsizes[$set] -eq "50CM" -and -not $datasets[$set].contains( "hes" ) )
                {
                    foreach ( $corner in $corners )
                    {
                        DownloadTile $xx $yy $grid $datasets[$set] $datasetsuffixes[$set] $datasetsizes[$set] $corner $datapaddders[$set]
                    }
                }
                else
                {
                    DownloadTile $xx $yy $grid $datasets[$set] $datasetsuffixes[$set] $datasetsizes[$set] "" $datapaddders[$set]
                }
            }
        }
    }
}

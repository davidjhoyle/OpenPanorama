
# http://lle.gov.wales/Catalogue/Item/LidarCompositeDataset/?lang=en
# http://lle.blob.core.windows.net/lidar/1m_res_SJ_dtm.zip

# lle.blob.core.windows.net/lidar/1m_res_ST_dtm.zip

$BaseURL="http://lle.blob.core.windows.net/lidar/"
$datasets = 'dtm'
$datasetsuffixes = '.zip'
$datasetsizes = @('1m_res','2m_res','50cm_res')
$GridSquares = @( 'SJ','SR', 'SO', 'SM', 'SS', 'ST', 'SH', 'SN' )

$DestFolder = "F:\LIDAR\WalesSrc\"
$DestFolder = "C:\Users\david\Downloads\"

$wc = New-Object System.Net.WebClient
$wcsiz = New-Object System.Net.WebClient

Function DownloadTile( $mygrid, $myset, $mysuffix, $datasetsize)
{
    $myfilename = $datasetsize + "_" +  $mygrid + "_" + $myset + $mysuffix
    $url = $BaseURL + $myfilename
    $fullfilename = $DestFolder + $myfilename

    try
    {
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
    catch
    {
    }
}


foreach( $grid in $GridSquares )
{
    Write-Host "Grid = $grid"

    for( $set=0; $set -lt $datasets.Count; $set +=1 )
    {
	    for( $sizer=0; $sizer -lt $datasetsizes.Count; $sizer += 1 )
	    {
		    Write-Host "DownloadTile $grid $datasets $datasetsuffixes " + $datasetsizes[$sizer]
		    DownloadTile $grid $datasets $datasetsuffixes $datasetsizes[$sizer]

            $myfilename = $datasetsizes[$sizer] + "_" +  $grid + "_" + $datasets + $datasetsuffixes
            $fullfilename = $DestFolder + $myfilename
            $dstfullfilename = $fullfilename.replace(".zip","" )

            echo "Expand-Archive -LiteralPath $fullfilename -DestinationPath $dstfullfilename"
            Expand-Archive -LiteralPath $fullfilename -DestinationPath $dstfullfilename
	    }
    }
}



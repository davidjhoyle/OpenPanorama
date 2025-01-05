
$client = new-object System.Net.WebClient
$client.ses


#lang=english; 
#user=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX;
#LastVisit=2022-12-15+14%3A15;
#PHPSESSID=ddddddddddddddddddddddd


$headers = @{
    'Accept'='text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8'
    'Accept-Language'='en-GB,en;q=0.5'
    'Accept-Encoding'='gzip, deflate, br'
    'DNT'='1'
    'Cookie'='lang=english; user=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX; user=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX; LastVisit=2022-12-15+14%3A15'
    'Host'='www.megalithic.co.uk'
    'Sec-Fetch-Dest'='document'
    'Sec-Fetch-Mode'='navigate'
    'Sec-Fetch-Site'='none'
    'Sec-Fetch-User'='?1'
    'Upgrade-Insecure-Requests'='1'
    'User-Agent'='Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0'
}


$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    
$cookie = New-Object System.Net.Cookie("lang", "english", "/", "www.megalithic.co.uk")
$session.Cookies.Add($cookie)

$cookie2 = New-Object System.Net.Cookie("user", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "", "www.megalithic.co.uk")
$session.Cookies.Add($cookie2)

$cookie3 = New-Object System.Net.Cookie("user", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "/", "www.megalithic.co.uk")
$session.Cookies.Add($cookie3)

$cookie4 = New-Object System.Net.Cookie("LastVisit", "2022-12-15+14%3A15", "/", "www.megalithic.co.uk")
$session.Cookies.Add($cookie4)


# "https://www.megalithic.co.uk/asb_gps.php?country=1&kmltitle=England"
# "MegP_Sudan.csv"
# "f:\UK Stone Circles\CSVs\"

$startURL = "https://www.megalithic.co.uk/topics.php?countries=1&kmldown=1"

$Response = Invoke-WebRequest -Uri $startURL -SessionVariable $session 

$responseTxt=$Response.ParsedHtml.body.innerHTML

$splittxt = $responseTxt.Split( "<" );
$Ahrefs = @()


$baseUri = "http://www.megalithic.co.uk/asb_gps.php" 


foreach( $str in $splittxt )
{
    if (  $str.ToLower().Contains( "a href=""asb_gps.php?country=" ) -and $str.Contains("CSV"))
    {
        $tmpstr = $str.Replace("&amp;kmltitle", "" )
        $tmpstr = $tmpstr.Replace(""">CSV", "" )

        # A href="asb_gps.php?country=129&amp;kmltitle=Malawi">CSV

        $parts = $tmpstr.Split("=");

        $country = $parts[3]
        $countryid = $parts[2]

        $outfile = "f:\UK Stone Circles\CSVs\MegP_" + $country + ".csv";

        $myURL = $baseUri + "?country=" + $countryid + "&kmltitle=" + $country

        echo $myURL

#        $Response2 = Invoke-WebRequest -Uri $myURL -SessionVariable $session -OutFile $outfile -UserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0" -Headers $headers
        $Response2 = Invoke-WebRequest -Uri $myURL -WebSession $session  -OutFile $outfile -ContentType "application/json" -UserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0" -Headers $headers
    }
}


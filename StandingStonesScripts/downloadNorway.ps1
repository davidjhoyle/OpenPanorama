$UrlContents = Get-Content NorwayLidar.txt | %{ Invoke-WebRequest $_ } | select -expand Content 

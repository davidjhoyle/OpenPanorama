If X%src%X==XX Goto ENDME
If X%dst%X==XX Goto ENDME

If X%1X==XX Goto SkipCD
cd %1
:SkipCD

If X%2X==XX Goto SkipProc
Set ProcID=%2
:SkipProc

If X%3X==XX Goto SkipLidar
Set LocalLidar=F:\LIDAR
md %LocalLidar%
:SkipLidar

Copy /y "%src%\%srcfolder%\AllCountries.txt" 
Copy /y "%src%\%srcfolder%\XXCopyAndRun.cmd"

FOR /F "tokens=1,2* " %%C IN (AllCountries.txt) DO MD %dst%\standingstonesorg\%%C 
FOR /F "tokens=1,2* " %%C IN (AllCountries.txt) DO call XXCopyAndRun.cmd %%C %%D

If NOT x%COMPUTERNAME%x==x%MasterPC%x GOTO ENDME
If NOT X%2X==X1X GOTO ENDME

Copy /y "%src%\%srcfolder%\CopyCorrelationsSkims.cmd"
call CopyCorrelationsSkims.cmd

Copy /y "%src%\%srcfolder%\standingstonesorgAZCopy.cmd"
call standingstonesorgAZCopy.cmd
timeout /t 3600

rem call standingstonesorgAZCopy.cmd

:ENDME

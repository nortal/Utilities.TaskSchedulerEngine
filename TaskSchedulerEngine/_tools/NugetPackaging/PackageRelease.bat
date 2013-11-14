@ECHO OFF
..\..\..\.nuget\nuget.exe pack -Outputdirectory output -Build -Properties Configuration=Release ..\..\TaskSchedulerEngine.csproj
pause
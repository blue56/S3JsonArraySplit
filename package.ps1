$date = Get-Date
$version = $date.ToString("yyyy-dd-M--HH-mm-ss")
$filename = "S3JsonArraySplit-" + $version + ".zip"
cd .\S3JsonArraySplit\src\S3JsonArraySplit
dotnet lambda package ..\..\..\Packages\$filename --configuration Release -frun dotnet6 -farch arm64
cd ..\..\..
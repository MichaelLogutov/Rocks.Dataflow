cls

cd $PSScriptRoot

$id = ((Get-Item -Path ".\..").Name)
& 'c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe' /t:clean
& 'c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe' /t:restore
& 'c:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe' /t:pack /p:Configuration=Release

$package_file = @(Get-ChildItem "$id\bin\Release\*.nupkg" -Exclude "*.symbols.*" | Sort-Object -Property CreationTime -Descending)[0]
$package_file.Name

& nuget.exe push $package_file.FullName -source nuget.org

$package_file | Remove-Item
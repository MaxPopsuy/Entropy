@echo off
echo Wait for build...
dotnet publish --configuration Release
del bin\Release\net6.0\win-x86\publish\Splash.pdb
start explorer.exe bin\Release\net6.0\win-x86\publish
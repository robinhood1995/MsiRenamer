# MSI Renamer
This applications to rename the msi that the deployment package generates.

Here is how to install and use. Go under you installer project and click on "PostBuildEvents". In the parameter you will need to call the .exe and then add the variables/macros as follows:

EXE INPUTFOLDER OUTPUTFOLDER

C:\temp\MsiRenamer.exe "$(BuiltOuputPath)" "$(ProjectDir)$(Configuration)\"

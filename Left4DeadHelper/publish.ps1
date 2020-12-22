# This script stops and starts a Windows service.
# It assumes SubInACL was used to grant that permission to the current user.

$Runtime = 'win10-x64'
$Configuration = 'Release'
$ServiceName = "Left4DeadHelper"
$ServiceDescription = "Left 4 Dead Helper bot service"

$Service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if ($Service -and $Service.Status -eq [System.ServiceProcess.ServiceControllerStatus]::Running)
{
    $Service.Stop()
    $Service.WaitForStatus([System.ServiceProcess.ServiceControllerStatus]::Stopped)
}

Remove-Item -Recurse dist\* -ErrorAction SilentlyContinue
dotnet clean --configuration $Configuration --nologo
dotnet restore --runtime $Runtime
dotnet build --runtime $Runtime --no-restore  `
    --configuration $Configuration --nologo
dotnet publish --configuration Release `
    --no-restore --no-build --nologo `
    --runtime $Runtime `
    --output dist

if ($Service)
{
    $Service.Start()
}
else
{
    # This part requires admin. Too lazy to check for it since we'd
    # need to conditionally do that near the start of the script
	# (we don't always need to install the service).

    $BinPath = (Get-Item "dist\Left4DeadHelper.exe").FullName
    $Service = New-Service -Name $ServiceName -Description $ServiceDescription `
		-BinaryPathName $BinPath -StartupType Automatic -DependsOn "TcpIp"
    $Service.Start()

    # Let the current user stop and start the service without admin.
    # Requires that SubInACL is installed.
    # TODO check if SubInACL is installed.
    & "C:\Program Files (x86)\Windows Resource Kits\Tools\subinacl.exe" /service $ServiceName "/grant=$($env:UserName)=PTO"
}

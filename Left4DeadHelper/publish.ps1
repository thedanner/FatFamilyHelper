# This script stops and starts a Windows service.

$Runtime = 'win10-x64'
$Configuration = 'Release'
$ServiceName = "Left4DeadHelper"
$ServiceDescription = "Left 4 Dead Helper bot service"


$CurrentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
$IsAdmin = $CurrentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

$Service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if ($Service -eq $null)
{
    if (-not $IsAdmin)
    {
        Write-Error "The bot's service is not installed. Please run this script with administrator permissions to create it."
        exit 1
    }

    Write-Output "The bot's service will be installed after a successful build."
}


dotnet test --nologo `
    --runtime $Runtime  --configuration $Configuration `
    "..\Left4DeadHelper.Tests.Unit\"

if ($LASTEXITCODE -ne 0)
{
    Write-Warning "Tests failed; cannot publish."
    exit $LASTEXITCODE
}

if ($Service.Status -eq [System.ServiceProcess.ServiceControllerStatus]::Running)
{
    try
    {
        $Service.Stop()
        $Service.WaitForStatus([System.ServiceProcess.ServiceControllerStatus]::Stopped)
    }
    catch [System.Management.Automation.MethodInvocationException]
    {
        Write-Error "The current user doesn't have permission to stop the bot service. Fix the service permissions,"
        Write-Error "or delete the service and run this script with administrator permissions to recreate it correctly."
        exit 2
    }
}

Remove-Item -Recurse "dist\*" -ErrorAction SilentlyContinue
dotnet clean --configuration $Configuration --nologo
dotnet restore --runtime $Runtime
dotnet build --runtime $Runtime  --configuration $Configuration `
    --no-restore --nologo

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
    if ($IsAdmin)
    {
        $BinPath = (Get-Item "dist\Left4DeadHelper.exe").FullName
        $Service = New-Service -Name $ServiceName -Description $ServiceDescription `
            -BinaryPathName $BinPath -StartupType Automatic -DependsOn "TcpIp"
        $Service.Start()

        $Sid = (wmic useraccount where name=`'$([Environment]::UserName)`' get sid)[2].Trim()
        $CurrentSD = (sc.exe sdshow "$ServiceName")[1].Trim()

        # Since SubInACL isn't hosted anymore, do the same thing it did by hand.

        # http://woshub.com/set-permissions-on-windows-service/ was helpful here.
        # Add to the discretionary list (starts with "D:"):
        $SdToAdd = "(A;;RPWPDT;;;$Sid)"
        #               ^^^^^^ RP = SERVICE_START, WP = SERVICE_STOP, DT = SERVICE_PAUSE_CONTINUE
        #            ^ A = Allow (D = Deny)

        if ($CurrentSd.Contains("S:("))
        {
            $NewSd = $CurrentSD -replace 'S:\(', ($SdToAdd + "S:(")
        }
        else
        {
            $NewSD = $CurrentSD + $SdToAdd
        }

        sc.exe sdset "$ServiceName" "$NewSd"
    }
    else
    {
        Write-Error "Please re-run this script with administrator permissions to properly install and configure the bot service."
        exit 3
    }
}

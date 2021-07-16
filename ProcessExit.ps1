# setup process exit monitor
$processName = "testhost.net472.x86.exe"
$key = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\$processName"

if (-not (Test-Path $key)) {
    # do not use force here, registry FS provider will delete the whole key
    # and re-create it instead of checking if it exists and doing nothing
    New-Item $key | Out-Null
}

New-ItemProperty $key -Name GlobalFlag -PropertyType Dword -Value 0x200 -Force

# teardown process exit monitor
$processName = "testhost.net472.x86.exe"
$key = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\$processName"

if (Test-Path $key) {
    Remove-Item -Force $key
}

# show what happened

"Exits:"
Get-WinEvent -FilterHashtable @{
    LogName      = "Application";
    ProviderName = "Microsoft-Windows-ProcessExitMonitor"
} -MaxEvents 10 | Select-Object TimeCreated, Id, Message, @{
    Name = "Properties";
    e    = { ($_.Properties | ForEach-Object { $_.Value }) }
} -ErrorAction Ignore | Format-List | Out-String

"Errors:"
Get-WinEvent -FilterHashtable @{
    LogName = "Application";
    Level   = [int][System.Diagnostics.Eventing.Reader.StandardEventLevel]::error
} -MaxEvents 200 -ErrorAction Ignore | Where-Object { $_.ProviderName -in ".NET Runtime", "ApplicationError" } | Select-Object TimeCreated, Id, Message, @{
    Name = "Properties";
    e    = { ($_.Properties | ForEach-Object { $_.Value }) }
} | Select-Object -First 10 | Format-List | Out-String

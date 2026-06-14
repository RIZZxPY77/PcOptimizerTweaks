$dllPath = "c:\Users\Administrator\Documents\optimize\PCOptimizer\bin\Debug\net8.0-windows7.0\PCOptimizer.dll"
[System.Reflection.Assembly]::LoadFrom($dllPath)
$opt = New-Object PCOptimizer.Services.EmulatorOptimizer
$list = $opt.GetEmulatorsStatus()
foreach ($emu in $list) {
    Write-Host "Name: $($emu.Name)"
    Write-Host "ExecutableName: $($emu.ExecutableName)"
    Write-Host "IsRunning: $($emu.IsRunning)"
    Write-Host "IsInstalled: $($emu.IsInstalled)"
    Write-Host "InstalledPath: $($emu.InstalledPath)"
    Write-Host "PriorityStatus: $($emu.PriorityStatus)"
    Write-Host "AffinityStatus: $($emu.AffinityStatus)"
    Write-Host "CpuUsage: $($emu.CpuUsage)"
    Write-Host "RamUsage: $($emu.RamUsage)"
    Write-Host "BoostDescription: $($emu.BoostDescription)"
    Write-Host "--------------------------------------"
}

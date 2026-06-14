# Replicating GetEmulatorsStatus in PowerShell
$emulators = @(
    @{
        Name = "BlueStacks 5"
        ExecutableName = "HD-Player.exe"
        ProcessNames = @("HD-Player", "BlueStacks", "BstkSVC")
    },
    @{
        Name = "LDPlayer"
        ExecutableName = "dnplayer.exe"
        ProcessNames = @("dnplayer", "LdVBoxHeadless")
    },
    @{
        Name = "NoxPlayer"
        ExecutableName = "Nox.exe"
        ProcessNames = @("Nox", "NoxVMSHandle", "NoxPack")
    },
    @{
        Name = "MEmu"
        ExecutableName = "MEmu.exe"
        ProcessNames = @("MEmu", "MEmuHeadless")
    },
    @{
        Name = "GameLoop"
        ExecutableName = "AndroidEmulator.exe"
        ProcessNames = @("AndroidEmulator", "aow_exe", "AndroidEmulatorEn", "AppMarket")
    },
    @{
        Name = "MSI App Player"
        ExecutableName = "HD-Player.exe"
        ProcessNames = @("HD-Player", "MSIAppPlayer")
    }
)

$activeProcesses = Get-Process
$activeNames = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::OrdinalIgnoreCase)
foreach ($p in $activeProcesses) {
    [void]$activeNames.Add($p.ProcessName)
    [void]$activeNames.Add($p.ProcessName + ".exe")
}

Write-Host "Active process names related to emulators:"
foreach ($n in $activeNames) {
    if ($n -match "player|nox|emu|aow|blue|hd-|ldplayer|bstk") {
        Write-Host "  - $n"
    }
}
Write-Host "--------------------------------------"

foreach ($emu in $emulators) {
    $isRunning = $activeNames.Contains($emu.ExecutableName)
    $matching = $emu.ExecutableName
    if (!$isRunning) {
        foreach ($pName in $emu.ProcessNames) {
            if ($activeNames.Contains($pName)) {
                $isRunning = $true
                $matching = $pName
                break
            }
        }
    }
    
    Write-Host "Emulator: $($emu.Name)"
    Write-Host "  IsRunning: $isRunning"
    if ($isRunning) {
        Write-Host "  Matching process: $matching"
    }
}

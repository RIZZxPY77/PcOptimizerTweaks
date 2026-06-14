$jsonPath = "c:\Users\Administrator\Documents\optimize\PCOptimizer\Services\translations.json"
$newJsonPath = "c:\Users\Administrator\Documents\optimize\PCOptimizer\Services\new_keys.json"

if (!(Test-Path $jsonPath)) {
    Write-Error "translations.json not found!"
    exit 1
}

if (!(Test-Path $newJsonPath)) {
    Write-Error "new_keys.json not found!"
    exit 1
}

$rawJson = Get-Content $jsonPath -Raw -Encoding utf8
$data = ConvertFrom-Json $rawJson

$newRawJson = Get-Content $newJsonPath -Raw -Encoding utf8
$newKeys = ConvertFrom-Json $newRawJson

foreach ($key in $newKeys.psobject.Properties.Name) {
    $translations = $newKeys.$key
    foreach ($lang in $translations.psobject.Properties.Name) {
        $val = $translations.$lang
        if ($data.$lang -eq $null) {
            continue
        }
        $data.$lang | Add-Member -MemberType NoteProperty -Name $key -Value $val -Force
    }
}

$mergedJson = ConvertTo-Json $data -Depth 20
[System.IO.File]::WriteAllText($jsonPath, $mergedJson, [System.Text.Encoding]::UTF8)
Write-Output "Successfully merged translations!"

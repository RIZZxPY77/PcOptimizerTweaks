$filePath = "c:\Users\Administrator\Documents\optimize\PCOptimizer\Services\TranslationService.cs"
$jsonPath = "c:\Users\Administrator\Documents\optimize\PCOptimizer\Services\translations.json"

if (!(Test-Path $filePath)) {
    Write-Error "File not found: $filePath"
    exit 1
}

$content = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
$json = Get-Content $jsonPath -Raw -Encoding utf8 | ConvertFrom-Json

foreach ($lang in $json.psobject.Properties.Name) {
    $dict = $json.$lang
    $dictStr = ""
    foreach ($prop in $dict.psobject.Properties) {
        $key = $prop.Name
        $val = $prop.Value.Replace('"', '\"')
        $dictStr += "            $lang[`"$key`"] = `"$val`";`r`n"
    }
    
    $targetPattern = "Translations[`"$lang`"] = $lang;"
    $replacement = $dictStr + "            Translations[`"$lang`"] = $lang;"
    
    $content = $content.Replace($targetPattern, $replacement)
}

[System.IO.File]::WriteAllText($filePath, $content, [System.Text.Encoding]::UTF8)
Write-Output "Successfully injected translations into TranslationService.cs!"

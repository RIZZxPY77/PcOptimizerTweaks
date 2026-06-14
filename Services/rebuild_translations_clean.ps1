$jsonPath = "c:\Users\Administrator\Documents\optimize\PCOptimizer\Services\translations.json"
$outPath = "c:\Users\Administrator\Documents\optimize\PCOptimizer\Services\TranslationService.cs"

if (!(Test-Path $jsonPath)) {
    Write-Error "JSON file not found: $jsonPath"
    exit 1
}

# Backup the original first
Copy-Item $outPath ($outPath + ".bak") -Force

$json = Get-Content $jsonPath -Raw -Encoding utf8 | ConvertFrom-Json

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine('using System;')
[void]$sb.AppendLine('using System.Collections.Generic;')
[void]$sb.AppendLine('using Microsoft.Win32;')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('namespace PCOptimizer.Services')
[void]$sb.AppendLine('{')
[void]$sb.AppendLine('    public static class TranslationService')
[void]$sb.AppendLine('    {')
[void]$sb.AppendLine('        public static string CurrentLanguage { get; set; } = "id";')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('        private static readonly Dictionary<string, Dictionary<string, string>> Translations = new();')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('        static TranslationService()')
[void]$sb.AppendLine('        {')
[void]$sb.AppendLine('            LoadLanguage();')
[void]$sb.AppendLine('            InitializeTranslations();')
[void]$sb.AppendLine('        }')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('        public static void LoadLanguage()')
[void]$sb.AppendLine('        {')
[void]$sb.AppendLine('            try')
[void]$sb.AppendLine('            {')
[void]$sb.AppendLine('                using var key = Registry.CurrentUser.OpenSubKey(@"Software\ClayxOptimize");')
[void]$sb.AppendLine('                var lang = key?.GetValue("AppLanguage")?.ToString();')
[void]$sb.AppendLine('                if (!string.IsNullOrEmpty(lang))')
[void]$sb.AppendLine('                {')
[void]$sb.AppendLine('                    CurrentLanguage = lang;')
[void]$sb.AppendLine('                }')
[void]$sb.AppendLine('            }')
[void]$sb.AppendLine('            catch {}')
[void]$sb.AppendLine('        }')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('        public static string Get(string key)')
[void]$sb.AppendLine('        {')
[void]$sb.AppendLine('            if (Translations.TryGetValue(CurrentLanguage, out var langDict) && langDict.TryGetValue(key, out var val))')
[void]$sb.AppendLine('            {')
[void]$sb.AppendLine('                return val;')
[void]$sb.AppendLine('            }')
[void]$sb.AppendLine('            if (Translations.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var enVal))')
[void]$sb.AppendLine('            {')
[void]$sb.AppendLine('                return enVal;')
[void]$sb.AppendLine('            }')
[void]$sb.AppendLine('            return key;')
[void]$sb.AppendLine('        }')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('        private static void InitializeTranslations()')
[void]$sb.AppendLine('        {')

foreach ($lang in $json.psobject.Properties.Name) {
    [void]$sb.AppendLine("            // ==================== $($lang.ToUpper()) ====================")
    $dictLine = '            var ' + $lang + ' = new Dictionary<string, string>()'
    [void]$sb.AppendLine($dictLine)
    [void]$sb.AppendLine('            {')
    
    $dict = $json.$lang
    foreach ($prop in $dict.psobject.Properties) {
        $key = $prop.Name
        $val = $prop.Value
        
        # Proper escaping for C# string literal
        $val = $val.Replace('\', '\\')
        $val = $val.Replace('"', '\"')
        $val = $val.Replace("`r", "\r")
        $val = $val.Replace("`n", "\n")
        
        $entryLine = '                { "' + $key + '", "' + $val + '" },'
        [void]$sb.AppendLine($entryLine)
    }
    
    [void]$sb.AppendLine('            };')
    $assignLine = '            Translations["' + $lang + '"] = ' + $lang + ';'
    [void]$sb.AppendLine($assignLine)
    [void]$sb.AppendLine('')
}

[void]$sb.AppendLine('        }')
[void]$sb.AppendLine('    }')
[void]$sb.AppendLine('}')

[System.IO.File]::WriteAllText($outPath, $sb.ToString(), [System.Text.Encoding]::UTF8)
Write-Output "Successfully rebuilt TranslationService.cs from translations.json!"

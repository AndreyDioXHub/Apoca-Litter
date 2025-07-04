param(
    [Parameter(Mandatory=$true)]
    [string]$InputFileName,
    
    [Parameter(Mandatory=$true)]
    [string]$OutputFileName
)

# Проверяем, есть ли расширение .json
if (-not $InputFileName.EndsWith('.json')) {
    $InputFileName = $InputFileName + '.json'
}

# Проверяем, есть ли расширение .csv
if (-not $OutputFileName.EndsWith('.csv')) {
    $OutputFileName = $OutputFileName + '.csv'
}

# Проверка существования файла
if (-not (Test-Path $InputFileName)) {
    Write-Error "Файл $InputFileName не найден!"
    exit 1
}

# Чтение JSON файла
$jsonContent = Get-Content -Path $InputFileName -Raw | ConvertFrom-Json

# Конвертация JSON в CSV
$jsonContent.bots | Select-Object name, speed, type, link, xp, theme, icon, attack | Export-Csv -Path $OutputFileName -NoTypeInformation -Encoding UTF8

Write-Host "Файл $InputFileName успешно конвертирован в $OutputFileName"
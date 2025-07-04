param(
    [Parameter(Mandatory=$true)]
    [string]$InputFileName,
    
    [Parameter(Mandatory=$true)]
    [string]$OutputFileName
)

# Проверяем, есть ли расширение .csv
if (-not $InputFileName.EndsWith('.csv')) {
    $InputFileName = $InputFileName + '.csv'
}

# Проверяем, есть ли расширение .json
if (-not $OutputFileName.EndsWith('.json')) {
    $OutputFileName = $OutputFileName + '.json'
}

# Проверка существования файла
if (-not (Test-Path $InputFileName)) {
    Write-Error "Файл $InputFileName не найден!"
    exit 1
}

# Чтение CSV файла с разделителем ";"
$csvData = Import-Csv -Path $InputFileName -Delimiter ";"

# Создание структуры JSON
$jsonStructure = @{
    bots = @(
        foreach ($row in $csvData) {
            
            # Создание иконки из имени (в нижнем регистре с подчеркиваниями)
            #$icon = $row.name.ToLower().Replace(" ", "_") + "_icon"
            
            # Создание объекта для каждого бота
            @{
                name = $row.name
                type = $row.type
                xp = $row.xp
                speed = $row.speed
                attack = $row.attack
                link = if ($row.link -eq "null") { "ZombiSkin" } else { $row.link }
                icon = $row.icon
                theme = $row.theme
            }
        }
    )
}

# Конвертация в JSON и сохранение в файл
$jsonStructure | ConvertTo-Json -Depth 10 | Out-File $OutputFileName -Encoding UTF8

Write-Host "Конвертация завершена. Файл $InputFileName сконвертирован в $OutputFileName"
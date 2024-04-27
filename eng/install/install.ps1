# Install script that downloads Metamod:Source and Counter-Strike Sharp (with runtime)

$MM_DOWNLOAD_URL = "https://mms.alliedmods.net/mmsdrop/2.0/mmsource-2.0.0-git1286-windows.zip"
$TARGET_DIR = "./game/csgo"
$GAMEINFO_FILE = Join-Path $TARGET_DIR "gameinfo.gi"
$ProgressPreference = 'SilentlyContinue'

# Verification
if (-not (Test-Path $GAMEINFO_FILE)) {
    Write-Error "Error: $GAMEINFO_FILE does not exist in the specified directory."
    exit 1
}

# GitHub API for Counter-Strike Sharp Releases
$RELEASE_INFO = (Invoke-WebRequest -Uri "https://api.github.com/repos/roflmuffin/CounterStrikeSharp/releases/latest").Content | ConvertFrom-Json

# Filtering download URLs
$CSSHARP_DOWNLOAD_URL = $RELEASE_INFO.assets | 
    Where-Object { $_.browser_download_url -like '*windows*.zip*' -and $_.browser_download_url -notlike '*with-runtime*' } | 
    Select-Object -First 1 -ExpandProperty browser_download_url

$CSSHARP_RUNTIME_DOWNLOAD_URL = $RELEASE_INFO.assets | 
    Where-Object { $_.browser_download_url -like '*windows*.zip*' -and $_.browser_download_url -like '*with-runtime*' } | 
    Select-Object -First 1 -ExpandProperty browser_download_url

### METAMOD:SOURCE ###
Write-Output "Downloading Metamod:Source..."

Invoke-WebRequest -Uri $MM_DOWNLOAD_URL -OutFile metamod.zip
Write-Output "Extracting Metamod:Source to $TARGET_DIR..."
Expand-Archive -Force -Path metamod.zip -DestinationPath $TARGET_DIR
Remove-Item metamod.zip 

### GAMEINFO.GI UPDATE ###
$NEW_ENTRY = "			Game    csgo/addons/metamod"
$FILE_CONTENT = Get-Content $GAMEINFO_FILE

Write-Output "Updating $GAMEINFO_FILE..."
if ($FILE_CONTENT -contains $NEW_ENTRY) {
    Write-Output "The entry '$NEW_ENTRY' already exists in $GAMEINFO_FILE. No changes were made."
} else {
    $Pattern = "Game_LowViolence"
    $Modified = $false
    $NewContent = @()

    foreach ($line in $FILE_CONTENT) {
        if ($line -match $Pattern -and -not $Modified) {
            $NewContent += $line
            $NewContent += $NEW_ENTRY
            $Modified = $true
        } else {
            $NewContent += $line
        }
    }

    $NewContent | Set-Content $GAMEINFO_FILE
    Write-Host "The file $GAMEINFO_FILE has been modified successfully. '$NEW_ENTRY' has been added."
}

### COUNTER-STRIKE SHARP ###
Write-Output "Downloading Counter-Strike Sharp (with runtime)..."

# Determine if runtime needs to be downloaded
if (-not (Test-Path "./game/csgo/addons/CounterStrikeSharp/dotnet/dotnet.exe")) {
    $CSSHARP_DOWNLOAD_URL = $CSSHARP_RUNTIME_DOWNLOAD_URL
}

Invoke-WebRequest -Uri $CSSHARP_DOWNLOAD_URL -OutFile cssharp.zip
Write-Output "Extracting Counter-Strike Sharp to $TARGET_DIR..."
Expand-Archive -Force -Path cssharp.zip -DestinationPath $TARGET_DIR
Remove-Item cssharp.zip 

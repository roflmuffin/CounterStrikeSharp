#!/bin/bash

# Install script that downloads Metamod:Source and Counter-Strike Sharp (with runtime)

MM_DOWNLOAD_URL="https://mms.alliedmods.net/mmsdrop/2.0/mmsource-2.0.0-git1286-linux.tar.gz"
TARGET_DIR="./game/csgo"
GAMEINFO_FILE="${TARGET_DIR}/gameinfo.gi"

if [ ! -f "${GAMEINFO_FILE}" ]; then
    printf "Error: %s does not exist in the specified directory.\n" "$GAMEINFO_FILE"
    exit 1
fi

RELEASE_INFO=$(curl -s https://api.github.com/repos/roflmuffin/CounterStrikeSharp/releases/latest)

# Filter and store download URLs
CSSHARP_DOWNLOAD_URL=$(echo "$RELEASE_INFO" | grep -o "browser_download_url.*linux.*\.zip" | cut -d '"' -f 3 | grep -v "with-runtime")
CSSHARP_RUNTIME_DOWNLOAD_URL=$(echo "$RELEASE_INFO" | grep -o "browser_download_url.*linux.*\.zip" | cut -d '"' -f 3 | grep "with-runtime")

### METAMOD:SOURCE ###
printf "Downloading Metamod:Source...\n"

curl -s -L -o metamod.tar.gz "$MM_DOWNLOAD_URL"

if [ $? -eq 0 ]; then
    printf "Extracting Metamod:Source to %s...\n" "$TARGET_DIR"
    tar -xzf metamod.tar.gz -C "$TARGET_DIR"

    rm metamod.tar.gz
else
    echo "Download failed. Please check the URL and your connection."
fi

### GAMEINFO.GI UPDATE ###
NEW_ENTRY="			Game    csgo/addons/metamod"

printf "Updating %s...\n" "$GAMEINFO_FILE"
if grep -Fxq "$NEW_ENTRY" "$GAMEINFO_FILE"; then
    echo "The entry '$(echo $NEW_ENTRY | xargs)' already exists in ${GAMEINFO_FILE}. No changes were made."
else
    awk -v new_entry="$NEW_ENTRY" '
        BEGIN { found=0; }
        // {
            if (found) {
                print new_entry;
                found=0;
            }
            print;
        }
        /Game_LowViolence/ { found=1; }
    ' "$GAMEINFO_FILE" > "$GAMEINFO_FILE.tmp" && mv "$GAMEINFO_FILE.tmp" "$GAMEINFO_FILE"

    printf "The file %s has been modified successfully. '%s' has been added.\n" "$GAMEINFO_FILE" "$(echo $NEW_ENTRY | xargs)"
fi

printf "Downloading Counter-Strike Sharp (with runtime)...\n"

# If ./game/csgo/addons/CounterStrikeSharp/dotnet/dotnet does not exist, use the runtime download url
if [ ! -f "./game/csgo/addons/CounterStrikeSharp/dotnet/dotnet" ]; then
    CSSHARP_DOWNLOAD_URL="$CSSHARP_RUNTIME_DOWNLOAD_URL"
fi

curl -s -L -o cssharp.zip "$CSSHARP_DOWNLOAD_URL"

if [ $? -eq 0 ]; then
    printf "Extracting Counter-Strike Sharp to %s...\n" "$TARGET_DIR"
    unzip -q -o cssharp.zip -d "$TARGET_DIR"

    rm cssharp.zip
else
    echo "Download failed. Please check the URL and your connection."
fi

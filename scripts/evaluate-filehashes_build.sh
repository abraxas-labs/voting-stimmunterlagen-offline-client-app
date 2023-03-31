#!/bin/bash

targetFolder=${1:-"."}
outputHashFile=${2:-"filehashes_build.txt"}

root=$(pwd)
rm -f $outputHashFile

find "$root/$targetFolder" -type f | while read file; do
  relativePath="${file#$root/$targetFolder/}"
  hash=$(sha256sum "$file" | awk '{print $1}')
  echo "$(echo "$relativePath" | sed 's#\./##g' | sed 's#/#\\#g');$hash" >> "$outputHashFile"
done

echo "Hash values for files in $targetFolder and its subfolders have been saved to $outputHashFile"

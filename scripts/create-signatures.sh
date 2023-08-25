#!/bin/bash

targetFolder=${1:-"."}
codeSigningCert=${2:-"./Codesigning.pfx"}
codeSigningCertPassword=${3:-''}
root=$(pwd)
currDir="$root/$targetFolder"

sign () {
  echo "Sign binary: $1"
  signed="$1.signed"
  osslsigncode sign \
    -pkcs12 $codeSigningCert \
    -pass $codeSigningCertPassword \
    -h sha256 \
    -time 1672531200 \
    -n "VOTING Stimmunterlagen Offline Client" \
    -i "https://abraxas.ch/" \
    -in "$1" \
    -out "$signed"
  mv -f $signed $1
}

if [ ! -f "$codeSigningCert" ]; then
    echo "Skip signing binaries. No code signing certificate provided."
    exit 0
fi

sign "$currDir/voting-stimmunterlagen-offline.exe"
sign "$currDir/resources/tools/EchDeliveryJsonConverter/EchDeliveryJsonConverter.exe"
sign "$currDir/resources/tools/CryptoTool/CryptoTool.exe"
sign "$currDir/resources/tools/PdfMerger/PdfMerger.exe"
sign "$currDir/resources/tools/VotingCardGenerator/HtmlGeneration.dll"
sign "$currDir/resources/tools/VotingCardGenerator/HtmlGeneration.RazorLight.dll"
sign "$currDir/resources/tools/VotingCardGenerator/PdfGeneration.dll"
sign "$currDir/resources/tools/VotingCardGenerator/PdfGeneration.Prince.dll"
sign "$currDir/resources/tools/VotingCardGenerator/Voting.Stimmunterlagen.OfflineClient.Logging.dll"
sign "$currDir/resources/tools/VotingCardGenerator/VotingCardGenerator.exe"
sign "$currDir/resources/tools/VotingCardGenerator/VotingCardGenerator.dll"
sign "$currDir/resources/tools/ZipTool/ZipTool.exe"

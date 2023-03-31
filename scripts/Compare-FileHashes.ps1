<#
 .Synopsis
  Generates hashes from files and compares earch with a reference to ensure integrity.

 .Description
  Generates hashes recursively for all files within the target folder and 
  writes the hashes to an output file. Compares the hashes from a given
  reference input file with the generated hashes to ensure integrity.

 .Parameter sourceFolder
  The source folder where the build file hashes are located and local hashes and error reports get saved.

 .Parameter targetFolder
  The target folder to generate hashes from.

 .Example
   Compare-FileHashes.ps1 -sourceFolder ..

#>

[CmdletBinding()]
param(
  [Parameter(Mandatory=$true)]
  [string] $sourceFolder,
  [string] $targetFolder = ".",
  [string] $localHashFileName = "filehashes_local.txt",
  [string] $buildHashFileName = "filehashes_build.txt",
  [string] $reportFileName = "hash_error_report.txt"
)

Write-Host "-------------------------------------------------------------------------"
Write-Host "List Parameters"
Write-Host "-------------------------------------------------------------------------"
$localHashFilePath = Join-Path -Path $sourceFolder $localHashFileName
$buildHashFilePath = Join-Path -Path $sourceFolder $buildHashFileName
$reportFilePath = Join-Path -Path $sourceFolder $reportFileName
$root = (Get-Item -Path $targetFolder).FullName

Write-Host " >> sourceFolder: $sourceFolder"
Write-Host " >> targetFolder: $targetFolder"
Write-Host " >> localHashFileName: $localHashFileName"
Write-Host " >> localHashFilePath: $localHashFilePath"
Write-Host " >> buildHashFileName: $buildHashFileName"
Write-Host " >> buildHashFilePath: $buildHashFilePath"
Write-Host " >> reportFileName: $reportFileName"
Write-Host " >> reportFilePath: $reportFilePath"
Write-Host " >> root: $root"

Write-Host "`n-------------------------------------------------------------------------"
Write-Host "Generate Local Hash Values"
Write-Host "-------------------------------------------------------------------------"

if (Test-Path $localHashFilePath) { 
  Write-Host " >> Delete existing output file $localHashFilePath before recreating local hashes."
  Remove-Item $localHashFilePath 
}

$fileCount = (Get-ChildItem -Path $targetFolder -File -Recurse | Measure-Object).Count
Write-Host " >> Start generating hash values for $fileCount files."

Get-ChildItem -Path $targetFolder -File -Recurse | ForEach-Object {
  $hash = (Get-FileHash $_.FullName).Hash.ToLower()
  $relativePath = $_.FullName.Substring($root.Length + 1)
  "$relativePath;$hash" | Out-File -FilePath $localHashFilePath -Append
}

Write-Host "`n-------------------------------------------------------------------------"
Write-Host "Compare Hash Values"
Write-Host "-------------------------------------------------------------------------"

if (Test-Path $reportFilePath) { 
  Write-Host " >> Delete existing output file $reportFilePath before recreating local hashes."
  Remove-Item $reportFilePath 
}

$errorCount = 0

$localHashes = Get-Content $localHashFilePath
$buildHashes = Get-Content $buildHashFilePath

foreach ($localHash in $localHashes) {
  if ($buildHashes -notcontains $localHash) {
    "$localHash" | Out-File -FilePath $reportFilePath -Append
    $errorCount += 1;
    Write-Error " >> Local hash <$localHash> does not match with any of the build hashes."
  }
}

if ($localHashes.Count -ne $buildHashes.Count) {
  Write-Error " >> The number of local file hash values ($($localHashes.Count)) is not equal to the number of build hash values ($($buildHashes.Count))."
  $errorCount +=1;
}

if ($errorCount -gt 0) {
  Write-Error " >> $errorCount line(s) from $localHashFilePath not found in $buildHashFilePath."
  Write-Host " >> Error report has been logged into $reportFilePath"
}
else {
  Write-Host " >> SUCCESS: No hash value missmatches detected."
}

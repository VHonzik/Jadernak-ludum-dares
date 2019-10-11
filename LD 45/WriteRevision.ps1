 param (
    [Parameter(Mandatory=$true)][string]$file
 )

 Write-Host "Writing current commit SHA to $file"

 $currentSHA = git rev-parse --short HEAD
 Write-Verbose "Current SHA: $currentSHA"

if (Test-Path $file -PathType Leaf)
{
	$writtenSHA = (Get-Content $file -First 1).substring(2)
	Write-Verbose "$file already exist, and has SHA: $writtenSHA"

	if ($currentSHA -like $writtenSHA)
	{
		 Write-Host "  SHA is already up-to-date in $file"
		 exit 0
	}
}

$content = "//$currentSHA

#pragma once
namespace LD45
{
const char kBuildHash[] = `"$currentSHA`";
}
"

Set-Content -Path $file -Value $content
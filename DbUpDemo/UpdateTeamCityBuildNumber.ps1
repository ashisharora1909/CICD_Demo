[CmdletBinding()]
param (
	[Parameter(Mandatory =$true)]
	[string]
	$BranchName,

	[Parameter(Mandatory =$true)]
	[string]
	$BuildCounter
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$versionTxtPath = Resolve-Path -Path (Join-Path -Path $PSScriptRoot -ChildPath "..\..\version.txt")
$versionNumberPrefix = Get-Content -LiteralPath $versionTxtPath -Raw
$fullVersionNumber = "$versionNumberPrefix$BuildCounter"

$prereleaseTag =""
$runOctoPack = $true

if ($BranchName -eq "develop") {
	$prereleaseTag ="develop"
}

if ($BranchName -match "release/*") {
	$prereleaseTag ="release"
}

if ($prereleaseTag) {
	$fullVersionNumber = "$fullVersionNumber-$prereleaseTag"
}

Write-Host "##teamcity[setParameter name='octoPack.run' value='$runOctoPack']"
Write-Host "##teamcity[buildNumber '$fullVersionNumber']"


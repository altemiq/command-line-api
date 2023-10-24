$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path

Get-ChildItem -Path $ScriptDir -Include *.*proj,*.props,*.targets -Exclude *_wpftmp.*,*.g.props,*.g.targets -Recurse | ForEach-Object {
	$relative = Resolve-Path $_.FullName -Relative
    Write-Host "Processing $relative"
    $temp = New-TemporaryFile
    $changed = $false
    $lastEmpty = $false

    Get-Content $_.FullName | ForEach-Object {
      $trim = $_.TrimEnd()
      if ($trim -ne $_) {
        $changed = $true
      }

      $lastEmpty = $trim.Length -eq 0
      $trim
    } | Out-File -FilePath $temp -Encoding utf8BOM

    if ($lastEmpty -eq $false) {
      # need to append a newline at the end
      $changed = $true
      Add-Content $_.FullName -Value [Environment]::NewLine
    }

    if ($true -eq $changed) {
      Copy-Item -Path $temp -Destination $_.FullName
    }

    Remove-Item $temp
}
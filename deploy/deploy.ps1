'XIVDBDownloader の出力を取得する'
if (Test-Path .\tools) {
  Remove-Item .\tools -Force -Recurse
}
Copy-Item -Recurse -Path ..\..\..\XIVDBDownloader\bin\Release\* -Destination .\
'Done'

'配布対象外のファイルを削除する'
if (Test-Path deploy.dll) {
  Remove-Item deploy.dll -Force
}

Remove-Item -Force -Recurse -Path .\* -Include *.xml -Exclude *系*,preset-*
Remove-Item -Force -Recurse -Path .\* -Include *.pdb
Remove-Item -Force -Recurse -Path .\* -Include "Action icons",locale
'Done'

'配布ファイルをアーカイブする'
if (Test-Path deploy.zip) {
  Remove-Item deploy.zip -Force
}

$files = Get-ChildItem -Path .\ -Exclude deploy.ps1,*.zip
Compress-Archive -CompressionLevel Optimal -Path $files -DestinationPath deploy.zip
'Done'

Read-Host "終了するには何かキーを教えてください..."

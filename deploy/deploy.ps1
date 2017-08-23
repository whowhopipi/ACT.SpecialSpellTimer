'配布対象外のファイルを削除する'
Remove-Item deploy.dll -Force
Remove-Item *.xml -Force
Remove-Item *.pdb -Force
Remove-Item -Path .\* -Force -Recurse -Include "Action icons","locale"
'Done'

'配布ファイルをアーカイブする'
if (Test-Path deploy.zip) {
    Remove-Item deploy.zip -Force
}

$files = Get-ChildItem -Path .\ -Exclude deploy.ps1
Compress-Archive -CompressionLevel Optimal -Path $files -DestinationPath deploy.zip
'Done'

Read-Host "終了するには何かキーを教えてください..."

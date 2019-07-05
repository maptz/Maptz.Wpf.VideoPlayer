#Dependencies are required from:
# - FFMPEG: https://ffmpeg.zeranoe.com/builds/
# - SoundTouch: https://www.surina.net/soundtouch/download.html

$ffmpegUrl="https://ffmpeg.zeranoe.com/builds/win64/shared/ffmpeg-4.1.3-win64-shared.zip"
$soundTouchUrl="https://www.surina.net/soundtouch/soundtouch_dll-2.1.1.zip"

Function DownloadAndExtract([string]$url)
{
    $fileName = [Linq.Enumerable]::Last($url -split '/')
    $zipFileDirectory = "$PSScriptRoot\external\zips"
    If(!(test-path $zipFileDirectory))
    {
      New-Item -ItemType Directory -Force -Path $zipFileDirectory
    }
    $zipFilePath = "$zipFileDirectory\$fileName"
    If(!(test-path $zipFilePath))
    {
        Write-Host "Downloading file from url $url"
        (New-Object System.Net.WebClient).DownloadFile($url, $zipFilePath)
        Add-Type -assembly "System.IO.Compression.Filesystem"
    }
    else{
        Write-Host "File alraedy downloaded from url $url"
    }
    $dest = [System.IO.Path]::Combine($PSScriptRoot, "external", [System.IO.Path]::GetFileNameWithoutExtension($fileName))
    If(!(test-path $dest))
    {
      New-Item -ItemType Directory -Force -Path $dest
    }

    [System.IO.Directory]::CreateDirectory($dest)
    [IO.Compression.Zipfile]::ExtractToDirectory($zipFilePath, $dest)
}


DownloadAndExtract $ffmpegUrl
DownloadAndExtract $soundTouchUrl


$dll64Bit = [System.IO.Path]::Combine($PSScriptRoot, "external", "soundtouch_dll-2.1.1/SoundTouch_x64.dll")
$dll64BitResolvedPath = [System.IO.Path]::Combine($PSScriptRoot, "external", "soundtouch_dll-2.1.1-resolved")
$dll64BitResolved = [System.IO.Path]::Combine($dll64BitResolvedPath, "SoundTouch.dll")
If(!(test-path $dll64BitResolvedPath))
    {
      New-Item -ItemType Directory -Force -Path $dll64BitResolvedPath
    }
copy-item $dll64Bit  $dll64BitResolved 


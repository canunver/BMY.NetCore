<#
.SYNOPSIS
    Yayın paketi: SGB.Net'i yayınlar (dotnet publish) ve verilen tarihten sonra değişen dosyaları
    kurumun Yazılım Güncelle sayfasına yüklenecek bir Zip dosyasına koyar.

.DESCRIPTION
    Adımlar:
      1. SGB.Net projesi Release olarak depo ile aynı seviyedeki ..\Sgb.NETCoreYayin\publish klasörüne yayınlanır (-YayinlamaYok verilirse atlanır).
      2. Kurumun Yazılım Güncelle sayfasının üst satırında görünen "en son değişen kitaplık" tarihi -Tarih olarak verilir
         (örnek: "SGB.Net.dll (14.10.26 17:45)" için -Tarih "14.10.26 17:45").
      3. Yayın çıktısında bu tarihten sonra değişen dosyalar seçilir. Önceki pakette olmayan (yeni eklenen) dosyalar
         tarihi eski olsa da alınır (NuGet kitaplıkları paketteki eski tarihleriyle gelir).
      4. Kuruma özel dosyalar alınmaz: *.config (SGB.Net.dll.config, web.config), appsettings*.json, App_Data\,
         wwwroot\img\giris\ (giriş arka planları), wwwroot\img\kurumlogo.*, *.pdb. Menu\*.xml pakete girer.
      5. Zip dosyası ..\Sgb.NETCoreYayin\SGB_Guncelleme_<tarih>.zip olarak yazılır; Yazılım Güncelle sayfasında "Zip'ten Güncelle"
         ile yüklenir.

.EXAMPLE
    .\_araclar\YayinPaketi.ps1 -Tarih "14.10.26 17:45"
.EXAMPLE
    .\_araclar\YayinPaketi.ps1 -Hepsi            # ilk kurulum / bütün dosyalar (kuruma özel olanlar yine hariç)
.EXAMPLE
    .\_araclar\YayinPaketi.ps1 -Tarih "14.10.26 17:45" -YayinlamaYok   # son yayın çıktısından yeniden paketle
#>
param(
    [string]$Tarih,
    [switch]$Hepsi,
    [switch]$YayinlamaYok,
    # Yayın klasörü; verilmezse depo klasörünün yanında <depo adı>Yayin (C:\sources\asp.net\Sgb.NETCoreYayin)
    [string]$Klasor
)

$ErrorActionPreference = 'Stop'
$kok = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($Klasor)) {
    $Klasor = Join-Path (Split-Path -Parent $kok) ((Split-Path -Leaf $kok) + 'Yayin')
}
$yayinKlasor = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Klasor)
$cikti = Join-Path $yayinKlasor 'publish'
$listeDosya = Join-Path $yayinKlasor 'son-paket-listesi.txt'

# Pakete alınmayan dosyalar (yayın klasörüne göre yol, büyük/küçük harf duyarsız)
$haricler = @(
    '\.config$',
    '^appsettings.*\.json$',
    '^App_Data\\',
    '^wwwroot\\img\\giris\\',
    '^wwwroot\\img\\kurumlogo\.',
    '\.pdb$',
    '^logs\\'
)

if (-not $Hepsi) {
    if ([string]::IsNullOrWhiteSpace($Tarih)) {
        throw 'Kurumun Yazılım Güncelle sayfasındaki en son kitaplık tarihini -Tarih ile verin (örnek: -Tarih "14.10.26 17:45") ya da -Hepsi kullanın.'
    }
    $bicimler = [string[]]@('dd.MM.yy HH:mm', 'dd.MM.yyyy HH:mm', 'dd.MM.yy HH:mm:ss', 'dd.MM.yyyy HH:mm:ss', 'dd.MM.yy', 'dd.MM.yyyy')
    $sinir = [datetime]::ParseExact($Tarih.Trim(), $bicimler, [Globalization.CultureInfo]::GetCultureInfo('tr-TR'), [Globalization.DateTimeStyles]::None)
}

New-Item -ItemType Directory -Force -Path $yayinKlasor | Out-Null

# 1. Yayınla
if (-not $YayinlamaYok) {
    Write-Host "Yayınlanıyor: $cikti" -ForegroundColor Cyan
    # Çıkarılan kitaplıklar eski yayından kalmasın diye publish klasörü her seferinde boşaltılır. Dosya tarihleri
    # derleme çıktısından geldiği için (kopyalama tarihi korur) değişmeyen dosyaların tarihi aynı kalır.
    if (Test-Path $cikti) { Remove-Item -Path $cikti -Recurse -Force }
    # Yayında hata ayıklama dosyaları (.pdb) üretilmez
    & dotnet publish (Join-Path $kok 'SGB.Net\SGB.Net.csproj') -c Release -o $cikti --nologo -p:DebugType=none -p:DebugSymbols=false
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish başarısız (çıkış kodu $LASTEXITCODE)." }
}
if (-not (Test-Path (Join-Path $cikti 'SGB.Net.dll'))) { throw "Yayın çıktısı bulunamadı: $cikti" }
if (-not (Test-Path (Join-Path $cikti 'Menu\menu.xml'))) { throw 'Yayın çıktısında Menu\menu.xml yok (SGB.Net.csproj içindeki Menu satırına bakın).' }

# Önceki paketin dosya listesi: orada olmayan dosya yenidir, tarihine bakılmadan alınır
$oncekiler = @{}
if (Test-Path $listeDosya) {
    Get-Content -Path $listeDosya -Encoding UTF8 | ForEach-Object { if ($_) { $oncekiler[$_.ToLowerInvariant()] = $true } }
}

# 2-3. Değişen dosyaları seç
$tumu = @()
$secilen = @()
$atlanan = @()
$onEk = $cikti.TrimEnd('\') + '\'
foreach ($f in Get-ChildItem -Path $cikti -Recurse -File) {
    $goreli = $f.FullName.Substring($onEk.Length)
    $haric = $false
    foreach ($h in $haricler) { if ($goreli -imatch $h) { $haric = $true; break } }
    if ($haric) { $atlanan += $goreli; continue }
    $tumu += $goreli
    $yeni = ($oncekiler.Count -gt 0) -and (-not $oncekiler.ContainsKey($goreli.ToLowerInvariant()))
    if ($Hepsi -or $yeni -or $f.LastWriteTime -gt $sinir) {
        $secilen += [pscustomobject]@{ Goreli = $goreli; Tam = $f.FullName; Tarih = $f.LastWriteTime; Yeni = $yeni }
    }
}

if ($secilen.Count -eq 0) {
    Write-Host 'Verilen tarihten sonra değişen dosya yok; paket oluşturulmadı.' -ForegroundColor Yellow
    return
}

# 4. Zip
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zipYol = Join-Path $yayinKlasor ('SGB_Guncelleme_' + (Get-Date -Format 'yyyyMMdd_HHmm') + '.zip')
if (Test-Path $zipYol) { Remove-Item $zipYol }
$zip = [System.IO.Compression.ZipFile]::Open($zipYol, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($s in $secilen) {
        # Giriş tarihi dosyanın kendi tarihidir; Yazılım Güncelle bu tarihe göre karşılaştırır.
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $s.Tam, $s.Goreli.Replace('\', '/'), [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
}
finally {
    $zip.Dispose()
}

# Sonraki paket için bu yayının dosya listesi
[IO.File]::WriteAllLines($listeDosya, [string[]]$tumu, (New-Object System.Text.UTF8Encoding($false)))

Write-Host ''
Write-Host ("Pakete giren {0} dosya:" -f $secilen.Count) -ForegroundColor Green
$secilen | Sort-Object Goreli | ForEach-Object {
    Write-Host ("  {0:dd.MM.yy HH:mm}  {1}{2}" -f $_.Tarih, $_.Goreli, $(if ($_.Yeni) { '  (yeni)' } else { '' }))
}
if ($atlanan.Count -gt 0) {
    Write-Host ''
    Write-Host ("Kuruma özel olduğu için alınmayan {0} dosya: {1}" -f $atlanan.Count, ($atlanan -join ', ')) -ForegroundColor DarkGray
}
Write-Host ''
Write-Host "Zip: $zipYol" -ForegroundColor Green
Write-Host 'Kurumun Yazılım Güncelle sayfasında "Zip''ten Güncelle" ile yükleyin.'

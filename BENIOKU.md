# Sgb.NETCore

Sgb.NETv2'nin .NET 8'e taşınan sürümü. Eski depo değiştirilmedi; buradaki her şey kopyadır.

> **Durum:** Bu kod henüz derlenmedi. Hazırlandığı ortamda .NET SDK yoktu; ilk derleme Visual Studio 2022 ile
> `SGB.sln` açılarak yapılmalı. Beklenen derleme sorunları aşağıda listeli.

## İçerik

| Yol | Açıklama |
|---|---|
| `SGB.sln`, `Directory.Build.props` | Çözüm ve tüm projelerin ortak ayarları (net8.0) |
| `Sunucu/GNLServis`, `VERServis`, `DEGServis`, `UZYServis`, `KYMServis`, `ORGServis`, `KHMServis` | Pilot kapsamında taşınan servisler (arayüz + iş kuralları) |
| `Sunucu/*ServisI` (BHMv2, BKD, BUM, HRC, IKY, KPM, MUH, PER, TMM) | Pilotun derlenmesi için gereken arayüz/tip projeleri. İş kuralları (C projeleri) henüz taşınmadı |
| `Sunucu/SGBServisler` | Arayüzleri iş kuralları sınıflarına bağlayan kitaplık; web uygulaması buna başvurur (birinci aşama) |
| `Sunucu/SGBApi` | Servisleri JSON API olarak yayınlayan ASP.NET Core uygulaması (ikinci aşama, çok katmanlı yapı için) |
| `_araclar` | Kopyalama ve generic dönüşüm betikleri, raporlar |

## Yeni tipler (`GNLServis/TNS.Generic.cs`)

`TNSCollection<T>`, `ObjectArray<T>`, `ObjectVAN<T>`. Eski tipsiz sınıflar duruyor ve ikisi arasında iki yönde
örtük dönüşüm var; bu yüzden `ObjectArray x = servis.MarkaListele(...)` gibi eski kod değişmeden derlenir.

Arayüz ve servis sınıfındaki dönüş tipleri değiştirildi, iç sınıflar tipsiz kaldı (dönüşüm `return` anında yapılır).
Hangi metodun hangi tipe çevrildiği ve çevrilemeyenler: `_araclar/generic_rapor.md`.

**Davranış farkı:** Eski kod hatayı bazen listenin içine `Sonuc` nesnesi koyarak bildiriyordu
(`donusDeger.Add(new Sonuc(false, "Yıl bilgisi boş olamaz!", ""))`). Tipli listede buna yer yok:
- `ObjectArray<T>`: listedeki hata `sonuc` alanına taşınır.
- `TNSCollection<T>`: `TNSIslemHatasi` fırlatılır; API bunu HTTP 500 + `{ "hata", "sonuc" }` olarak döndürür,
  istemcide de `TNSIslemHatasi` olarak yakalanır. Listenin ilk elemanı `Sonuc` mu diye bakan istemci kodu buna göre değişmeli.

## Birinci aşama: tek katman (`Sunucu/SGBServisler`)

İlk aşamada ayrı bir API sunucusu yok. Arayüz ve iş kuralları projeleri kitaplık olarak derlenir, web uygulaması
`SGBServisler` projesine başvurur ve açılışta bir kez şunu çağırır:

```csharp
TNS.Api.ServisKayit.Varsayilanlar();
```

Bundan sonra `TNS.KHM.Arac.Tanimla()` ağ çağrısı yapmadan doğrudan `KHMServis` nesnesini döndürür
(eski `web.config`'deki `<wellknown ...>` satırlarının karşılığı `SGBServisler/ServisKayit.cs` içindedir).

- Web uygulamasının ayar dosyası, eski **sunucu** `web.config`'indeki appSettings'i de içermelidir
  (bağlantı satırları, `DegiskenXML`, `UzayDosyasi` vb.); iş kuralları artık aynı süreçte çalışır.
- Bir sayfanın kullandığı her servisin iş kuralları (C) projesi taşınmış ve `ServisKayit`'a eklenmiş olmalıdır.
- Çok katmanlı yapıya dönmek için kod değişmez: servis `ServisKayit`'tan çıkarılır, `...ServisURL` ayarı API adresine çevrilir.
  Aşağıdaki `SGBApi` projesi ve `ServisIstemci` bu ikinci aşama için hazır bekliyor.

## Web uygulaması (`SGB.Net` + `Istemci`)

- **`SGB.Net`** çalıştırılabilir projedir (`SGB.Net.exe`); başlangıç projesi budur. İçinde yalnızca `Program.cs` ve `App.config` vardır
  ve neredeyse hiç değişmez.
- **`Istemci`** uygulamanın kendisidir ve kitaplık olarak derlenir (`Istemci.dll`): sayfalar (cshtml derlenmiş olarak dll'in içindedir),
  JSON uçları, oturum ve `wwwroot` dosyaları. Kurulum `IstemciBaslangic.cs` içindedir; yeni sayfa/uç/servis buraya eklenir.

Şu an giriş akışı, açılış sayfası ve duyuru sayfaları var. Aşağıdaki yollar `Istemci` projesine göredir.

| Yol | Açıklama |
|---|---|
| `Pages/Giris.cshtml` | Giriş sayfası (kullanıcı kodu + parola, ardından doğrulama kodu). Razor yalnızca birkaç değeri basar; işlemler `wwwroot/js/giris.js` içinde |
| `Pages/OrtakSayfa/GirisBilgi.cshtml` | Girişten sonra açılan sayfa: bilgi ekranları (eski portal pencereleri) üç sütunda, her biri kendi çerçevesinde. Liste ve yerleşim eskisi gibi `TNS.KYM.Arac.bilgiEkranlar` / `ModulYerlesimBul`'dan gelir (`OrtakSayfa/GirisBilgiUclari.cs`, `wwwroot/js/girisbilgi.js`). Eski yapıdaki gibi giriş kökte, diğer bütün sayfalar bir alt klasörde durur; sayfalar birbirine `../Modul/Sayfa` ile başvurur |
| `Pages/Shared/_Layout.cshtml` | Ortak çerçeve (eski `SayfaUstAltBolumYaz`): sayfa yalnızca içeriğini yazar. Üst şerit, sol menü ve alt bilgi `wwwroot/js/menu.js` ile çizilir. Üst şeritte menü yoktur: yazılım ikonu, bulunulan sayfa (menüdeki yeri + adı) ve kullanıcı adı (tema, dil, parola değiştirme, çıkış). Sol menünün üç görünümü vardır: açık (sabit), ikon (fare gelince açılır, ayrılınca daralır; varsayılan) ve gizli; başında ekran arama (Ctrl+K) ve ayrı bir kutuda modül (menü grubu) seçici bulunur. Menüden ekran seçilince ikon görünümüne döner. İkon: `menu.xml`'de `Ikon="dosya.svg"` → `wwwroot/img/menu/`; yoksa adın iki harfi. `?menuYok=1` ile menüsüz açılır |
| `OrtakSayfa/MenuUclari.cs`, `MenuOkuyucu.cs` | Menü verisi (`/api/ortaksayfa/menu`): `menu.xml` kullanıcının modül ve yetkilerine göre süzülür. Veri açılış sayfasında okunup tarayıcıda (`localStorage`) saklanır; diğer sayfalar saklanandan çizer |
| `wwwroot/css/menu.css` | Menü ve üst şeridin görünümü ve renk temaları (`Theme1..4`; kullanıcı değişkeni `TEMA`) |
| `../SGB.Net/Menu/menu*.xml` | Menü tanımları (`MenuXML` ayarı; varsayılan `Menu\menu.xml`) |
| `Pages/Duyuru/Duyuru.cshtml` | Duyuru ekranı (eski `Duyuru/Duyuru.aspx`): açılış sayfasında bilgi ekranı olarak çerçevede açılır, sığmazsa kayar. "Pencerede göster" işaretli duyuru varsa açılış sayfası bunları oturumda bir kez pencerede gösterir (`?penceremi=1`). `wwwroot/js/duyuru.js` |
| `Pages/Duyuru/DuyuruGiris.cshtml` | Duyuru Girişi (eski `Duyuru/DuyuruGiris.aspx`; yalnızca sistem yöneticisi ve destek başvuru yöneticisi): arama + AG Grid listesi, duyuru formu ve ortak HTML editörü. `wwwroot/js/duyurugiris.js`. Duyurunun ekli dosyaları ortak dosya bileşeniyle (modül `DUY`, form = duyuru kodu) |
| `Duyuru/DuyuruUclari.cs` | Duyuru uçları: `GET /api/duyuru/goster`, `/tanimlar`, `/liste`, `/{kod}`; `POST /api/duyuru/kaydet`, `/sil` |
| `Pages/Duyuru/EkliDosyalarForm.cshtml` | Ekli dosyalar (eski `EkliDosyalarForm.aspx`). Eski adresler çalışır: `?mdl=DUY&frm=12` liste, `?no=5&mdl=DUY&frm=12` dosyanın kendisi (duyuru metnindeki bağlantılar), `brm=1` birim adlı salt okunur liste |
| `EkliDosya/EkliDosyaUclari.cs`, `wwwroot/js/eklidosya.js` | Ortak ekli dosyalar bileşeni `Sgb.ekliDosyalar(el, { modul, form })`: yükleme (açıklamalı), AG Grid listesi, indirme, silme; DUY/YRD modüllerinde kilitleme ve bağlantı adresi kopyalama. Uçlar: `GET /api/ekli-dosya`, `/indir`; `POST /yukle`, `/sil`, `/kilit`. Yetki denetimi iş kurallarındadır (`KYMEkliDosyalar.YetkiKontrol`). Dosya boyu sınırı `EkliDosyaAzamiMB` (varsayılan 20). Betik çalıştırabilecek türler (HTML, SVG ...) her zaman indirilir, sayfada açılmaz |
| `wwwroot/js/ortak.js` | Sayfaların ortak yardımcıları (jQuery): `Sgb.al` / `Sgb.gonder` (JSON uçları, istek jetonu, 401'de girişe dönüş), `Sgb.mesaj`, `Sgb.grid` (Türkçe AG Grid), `Sgb.tarih`. `_Layout` jQuery ile birlikte her sayfaya ekler |
| `wwwroot/js/editor.js` | Ortak HTML editörü `Sgb.editor(textarea, ayarlar)`: Jodit 4 (MIT) üzerine kurulu; yazı tipi, boyut, yazı/arka plan rengi, listeler, hizalama, tablo, bağlantı, kaynak kodu. Biçimler satır içi stil olarak yazılır. `html()`, `html(deger)`, `bos()`. Kullanan sayfa `lib/jodit/jodit.min.css`, `lib/jodit/jodit.min.js` ve `js/editor.js`'i ekler |
| `wwwroot/lib/` | Bootstrap 3.3.6, jQuery 3.7.1, AG Grid Community 32.3.9 (`ag-grid-community.min.js`, stilleri içinde), Jodit 4.17.3. Tablo/editör kullanan sayfa kendisi ekler |
| `Kaynak/*.resx`, `Metin.cs` | Çok dilli metinler: `Metin.Ortak` (eski OrtakClassRes), `Metin.Menu` (menü, üst şerit, giriş), `Metin.Duyuru` (eski Duyuru kaynağı + yeni metinler) |
| `Giris/GirisUclari.cs` | Giriş sayfasının çağırdığı JSON uçları: `/api/giris/parola`, `/tamamla`, `/kod-yenile`, `/cikis` |
| `Oturum/OturumBilgisi.cs` | Oturum: tarayıcıda şifreli çerez; oturum verisi kullanıcı veri tabanındaki `KYMOTURUM` tablosunda (State Server yerine). Uygulama yeniden başlasa da oturumlar korunur. Süre: `OturumSuresiDakika` (varsayılan 40) |
| `wwwroot/img/giris/` | Giriş arka planları. Her açılışta klasördeki resimlerden biri rastgele seçilir; resim eklemek/çıkarmak yeterlidir |
| `wwwroot/img/kurumlogo.jpg` | Kurum logosu (dosya yoksa logo gösterilmez) |
| `../SGB.Net/App.config` | Ayarlar: eski sunucu ve istemci `web.config` appSettings birleşimi (exe projesinde; depoya girmez) |

Çerez şifreleme anahtarları `App_Data/anahtarlar` klasöründe tutulur (`AnahtarKlasoru` ayarıyla değiştirilebilir); bu klasör
güncellemelerde silinmemeli ve yalnızca uygulama havuzu kimliğinin erişimine açık olmalıdır.

Adresler: sayfa içi bağlantılar ve JavaScript çağrıları görelidir, uygulama IIS'te alt klasörde (sanal dizin) de çalışır.
Eski `Sayfa.aspx` adresleri uzantısı atılarak aynı sayfaya gider (`giris.aspx` -> `Giris`).

Giriş ile ilgili ayarlar: `ikiAsamaliGiris` (1/2 = doğrulama kodu istenir), `sistemUyarisi`, `yazilimAd`;
sistem değişkenleri `/KURUMAD`, `/TEKKULLANICI` (bakım modu), `/GirisDenemeSayi` (hatalı deneme sınırı).

Eski `Giris.aspx`'ten bilerek alınmayanlar: oturum numarasıyla şifresiz giriş (Entegrasyon), SSO1/SSO2 eklentileri,
dil bağlantıları, yardım/çağrı merkezi bağlantıları, "parolamı unuttum", güvenlik resmi (yerine hatalı deneme
sınırı aşılınca 15 dakikalık bekleme), parola süresi dolunca parola değiştirme sayfasına yönlendirme (sayfa henüz taşınmadı).

## API (`Sunucu/SGBApi`)

```
GET  /api                      yayınlanan servisler
GET  /api/KHM                  servisin metotları, imzaları, parametre adları
POST /api/KHM/ListeleOnBulgu   gövde: { "kullanan": {...}, "form": {...} }
```

- Gövde, parametre adlarıyla bir JSON nesnesidir; yanıt metodun dönüş değeridir.
- Aynı adlı birden fazla metot varsa ve parametre adları ayırt etmiyorsa `?imza=Tip1,Tip2` eklenir (`GET /api/{servis}` imzaları listeler).
- `ref/out` parametreli metotlarda yanıt `{ "donus": ..., "cikis": { "ad": değer } }` biçimindedir.
- `TNSDateTime` JSON'da ISO tarih dizgisi ya da `null` olur.
- Yeni servis eklemek için `SGBServisler/ServisKayit.cs` içine bir `Ekle<I...>("AD", () => new ...())` satırı yeterli.
- Ayarlar `App.config` içindedir (eski `web.config` appSettings kopyası; parola içerir, `.gitignore`'da).

## Araç sınıfı (`GNLServis/TNS.Api.ServisIstemci.cs`)

`Arac.Tanimla()` artık `Activator.GetObject` yerine `TNS.Api.ServisIstemci.Olustur<IKHMServis>(adres)` çağırır.
Dönen nesne arayüzü çalışma zamanında uygular: her metot çağrısını API'ye POST eder, gelen JSON'u dönüş tipine çevirir.
İstemci kodundaki `TNS.KHM.Arac.Tanimla().Metot(...)` çağrıları değişmez.

- İstemci tarafında `...ServisURL` ayarları yeni adresi göstermeli: `KHMServisURL = http://sunucu/api/KHM`
  (ORG servisi eskiden olduğu gibi KYM sınıfında: `ORGServisURL = http://sunucu/api/ORG`).
- API'nin içinde aynı çağrılar HTTP'ye çıkmaz, doğrudan nesneyi kullanır.
- Oturum/yetki başlığı eklemek için `ServisIstemci.IstekHazirla` kullanılır.

## Bilinen eksikler ve beklenen derleme sorunları

1. **Dış web servisleri** (İçişleri `YetkiSGB`, Mernis KPS v1, Mernis KPS v2): .NET Framework'e özgü altyapılar
   (ASMX istemcisi, WSE 2.0, WIF) .NET 8 karşılıklarıyla yeniden yazıldı; **gerçek servislere karşı henüz denenmedi**.
   - `YetkiSGB` ve KPS v1: `GNLServis/TNS.Soap.SoapIstemci.cs` tabanlı (HttpClient + XmlSerializer). KPS v1'de WS-Security
     başlığı elle üretiliyor.
   - KPS v2: `System.ServiceModel.Federation` ile; bağlamalar `Mernis.KPSv2.Types/KPSServiceFactory.cs` içinde kodla tanımlı
     (eski `app.config` içindeki `system.serviceModel` bölümü .NET 8'de okunmaz).
2. Diğer eski web referansları `_araclar/asmx_istemci_uret.py <Reference.cs>` ile aynı şekilde çevrilebilir.
3. **Oracle**: Oracle desteği çıkarıldı (`VERServis`). `ConnectionString.Data = ORACLE` ayarı artık hata verir; SQL Server, OleDb ve Odbc duruyor.
4. **`GNLServis/Calculator.cs`**: çalışma zamanında C# derliyor (CodeDom). Derlenir ama .NET 8'de çalışırken hata verir; Roslyn'e çevrilmeli.
5. **Henüz taşınmayan servisler**: KHM ve KYM iş kuralları MUH, BKD, BHMv2, BUM, PER, TMM, KPM servislerini çağırıyor.
   Bunların iş kuralları taşınana kadar o kod yolları "servis henüz yeni API'ye taşınmadı" hatası verir.
6. **Parametreler**: yalnızca dönüş tipleri generic yapıldı. `TNSCollection` alan parametreler (ör. `ItaAmirleriKaydet(..., TNSCollection detaylar)`)
   tipsiz kaldı; C# istemcisinden çalışır (`$type` ile), JavaScript'ten çağrılacaksa tipli hale getirilmeli.
7. **Kimlik**: metotlar hâlâ `Kullanici kullanan` parametresini istemciden alıyor. Giriş/oturum tasarlanınca kullanıcı sunucuda oturumdan doldurulmalı.

## Kalan modülleri taşımak

```
python3 _araclar/kopyala.py <Sgb.NETv2/Sunucu> <Sgb.NETCore/Sunucu> MUHServis/MUHServisC/MUHServisC.csproj
python3 _araclar/generic_donustur.py <Sgb.NETCore/Sunucu> <Sgb.NETv2/Sunucu> MUH            # önizleme
python3 _araclar/generic_donustur.py <Sgb.NETCore/Sunucu> <Sgb.NETv2/Sunucu> MUH --uygula
```
`kopyala.py` daha önce taşınmış projeleri (hedefte csproj'u olanları) atlar; yalnızca yeni projeleri kopyalar.
Yeni taşınan servis için `SGBServisler.csproj`'a proje başvurusu ve `ServisKayit.cs`'e kayıt satırı eklenir.

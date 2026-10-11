# Sgb.NETCore

Sgb.NETv2'nin .NET 8'e taşınan sürümü. Eski depo değiştirilmedi; buradaki her şey kopyadır.

> **Durum:** Bu kod henüz derlenmedi. Hazırlandığı ortamda .NET SDK yoktu; ilk derleme Visual Studio 2022 ile
> `SGB.sln` açılarak yapılmalı. Beklenen derleme sorunları aşağıda listeli.

## İçerik

| Yol | Açıklama |
|---|---|
| `SGB.sln`, `Directory.Build.props` | Çözüm ve tüm projelerin ortak ayarları (net8.0) |
| `Sunucu/GNLServis`, `VERServis`, `DEGServis`, `UZYServis`, `KYMServis`, `ORGServis`, `KHMServis` | Pilot kapsamında taşınan servisler (arayüz + iş kuralları) |
| `Sunucu/*Servis/*ServisI`, `*ServisC` (ATM, BHF, BHMv2, BKD, BUM, GEL, HRC, IKY, KPM, MUH, PEB, PER, STR, TMM, TRM, YAT) | Diğer bütün servislerin arayüz ve iş kuralları projeleri (aşağıda "Taşınan iş kuralları"). `EVRServis` kopyalandı ama derlenmiyor |
| `Sunucu/OrtakDLL` | Eski depodan alınan ikili bağımlılıklar: `Aspose.Email.dll` (TRM), `lpsolve55.dll` (PEB) |
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

### Taşınan iş kuralları (EVR dışında hepsi)

Eski `Sunucu` altındaki bütün servislerin arayüz (I) ve iş kuralları (C) projeleri taşındı ve `ServisKayit`'a eklendi:
ATM, BHF, BHMv2, BKD, BUM, DEG, GEL, HRC, IKY, KHM, KPM, KYM/ORG, MUH, PEB, PER, STR, TMM, TRM, UZY, YAT.
**EVR (evrak) taşınmadı**: dosyaları `Sunucu/EVRServis` altına kopyalandı ama derlenmiyor ve çözümde yok; arayüzü istemci
projesindeki servis başvurularına (`WebServisler`: Sgbnet WCF, mobil imza, OCR ...) ve Aspose.Words/Pdf/Cells ile
`StratekCrypto` kitaplıklarına bağlı. Boş olan `...ServisV` projeleri ile `Liste`, `MalzemeAktarim*` araçları alınmadı.

Tümü için geçerli notlar:

- **Kaynak eski deponun 2026-10-09 (a184b2c) hâlidir.** Eski depo yaşamaya devam ediyor; ilk taşımadan sonra orada değişen
  arayüz dosyaları (DEG `TKDKMi`, MUH, HRC `DBSiniflari`/`IHRCServis`) buraya da işlendi. Eski depoda yapılan sonraki
  değişiklikler elle aktarılmalıdır.
- **Tablo değişiklikleri**: her servis ilk kullanımda eski sistemdeki gibi bekleyen komutlarını (`XTabloDegisiklikleri`,
  `VERSIYON` tablosu) çalıştırır. Geliştirme veri tabanında 2026-10-10'da bekleyen: HRC 2 (81, 91), IKY 18, TMM 15;
  MUH, ATM, PEB, TRM 0 (BHMv2, BUM, KHM, KYM'e bakılmadı).
- **ASMX istemcileri**: araçla çevrilebilenler `SoapIstemci`'ye çevrildi (HRC: gib/su/tedas, TRM: cezali, MUH: SBB).
  Çevrilemeyenler (KPM `TarimMernisServiceProxy`, TMM `TCCB_BW2WS`, TRM `tkdkservice`, MUH `HarcamaMuhasebatServisi`)
  değiştirilmeden derlenir: `GNLServis/TNS.Soap.AsmxUyum.cs` eski `System.Web.Services` tiplerinin karşılığını sağlar
  (yalnızca eşzamanlı çağrılar, SOAP 1.1 document/literal). Hiçbiri gerçek servise karşı denenmedi.
- **TMM**: aktarım dosyasını (Excel) okuyan kod istemci kitaplığındaki `OrtakClass.Tablo`'yu kullanıyordu; yerine yer tutucu
  `TMMExcelOkuyucu` kondu (çağrılırsa `NotSupportedException`). Bir Excel okuyucu seçilince doldurulmalı.
- **TRM**: e-posta (IMAP) için eski depodaki `Aspose.Email.dll` (.NET Framework derlemesi, `Sunucu/OrtakDLL`) başvuruluyor;
  .NET 8'de çalışma zamanında denenmedi.
- **PEB**: `lpsolve55.dll` (yerel kitaplık, `Sunucu/OrtakDLL`) çıktıya kopyalanır; gösterge formülü DLL'i `AppDomain` yerine
  toplanabilir `AssemblyLoadContext` ile yüklenir. STR projeleri eskiden `OrtakDll`'den dll başvuruyordu, proje başvurusu yapıldı.
- **HRC**: `FormsAuthentication.HashPasswordForStoringInConfigFile(..., "sha1")` yerine eşdeğeri (büyük harf onaltılık SHA1).
- `VERServis`'e ilk taşımada dışarıda kalan `ENUMAlanTip` eklendi. BUM ve MUH'taki "her sertifikayı kabul et" kodu kalktı.
- `kopyala.py` uyarısı: bir proje `Sunucu` dışına (`..\Istemci\...`) başvuruyorsa araç onu da hedefin dışına kopyalar
  (EVR ve TMM'de oldu, geri alındı). Çalıştırmadan önce başvurulara bakın.

MUH, BHMv2 ve YAT için ayrıntı (dosyalar eskisinin kopyasıdır; yapılan değişiklikler):

- `Arac.Tanimla()` içindeki Remoting çağrıları `ServisIstemci.Olustur<T>` oldu (BHF, YAT, PEB arayüzleri).
- SBB ASMX istemcileri (`tr.gov.sbb.programbutce`, `...listeler`) `asmx_istemci_uret.py` ile `SoapIstemci`'ye çevrildi.
- `HarcamaMuhasebatServisi` "Bare" biçimli olduğu için araçla çevrilemedi; değiştirilmeden, uyum katmanıyla derlenir
  (onu çağıran kod eski sistemde de kapalıydı).
- Muhasebat WCF istemcileri (`MuhasebatMaliyeWS`, `MuhasebatMaliyeYevmiyeWS`): adsız kurucu, eski `app.config`'teki
  `basicHttpBinding` ve adresi kodla verir (`system.serviceModel` bölümü .NET 8'de okunmaz).
- Her sertifikayı kabul eden `CertificatePolicy` kaldırıldı (.NET 8'de yok); SBB servislerinin sertifikaları normal doğrulanır.
- `TABMuhasebeBirim`: liste sorgusuna vergi no eklendi; güncelleme cümlesi değerleri `DegerYap` ile yazıyor
  (adında kesme işareti olan birim güncellenemiyordu).
- Dönüş tipleri generic yapılmadı (`generic_donustur.py` çalıştırılmadı); JavaScript'ten doğrudan çağrılacaksa çalıştırılmalı.

Dış servisler (SBB, Muhasebat) gerçek adreslere karşı denenmedi. Servisler ilk kullanımda eski sistemdeki gibi bekleyen tablo
değişikliklerini (`...TabloDegisiklikleri`, `VERSIYON` tablosu) çalıştırır.

## Web uygulaması (`SGB.Net` + `Istemci`)

- **`SGB.Net`** çalıştırılabilir projedir (`SGB.Net.exe`); başlangıç projesi budur. İçinde yalnızca `Program.cs` ve `App.config` vardır
  ve neredeyse hiç değişmez.
- **`Istemci`** uygulamanın kendisidir ve kitaplık olarak derlenir (`Istemci.dll`): sayfalar (cshtml derlenmiş olarak dll'in içindedir),
  JSON uçları, oturum ve `wwwroot` dosyaları. Kurulum `IstemciBaslangic.cs` içindedir; yeni sayfa/uç/servis buraya eklenir.

Şu an giriş akışı, açılış sayfası, duyuru sayfaları, muhasebe / muhasebe birimi tanım sayfaları kullanıcı arama / giriş sayfaları ve genel görev tanımlama sayfası var. Aşağıdaki yollar `Istemci` projesine göredir.

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
| `Pages/ButceMuhasebe/TanimMuhasebe.cshtml` | Muhasebe Bilgileri (eski `ButceMuhasebe/TanimMuhasebe.aspx`; yalnızca sistem yöneticisi ve muhasebe yöneticisi): solda arama + AG Grid listesi, sağda muhasebenin (saymanlığın) bilgileri. `wwwroot/js/tanimmuhasebe.js`. "Saymanlık Taşı" penceresi de buradadır; birimleri HRC servisindeki idare tanımından denetler (HRC iş kuralları taşındı; taşıma işlemi denenmedi) |
| `Pages/ButceMuhasebe/TanimMuhasebeBirim.cshtml` | Muhasebe Birim Bilgileri (eski `ButceMuhasebe/TanimMuhasebeBirim.aspx`; aynı yetki): arama (muhasebe, birim kodu/adı, kurumsal kod) + liste, sağda birim formu. `wwwroot/js/tanimmuhasebebirim.js`. Eski "Yazdır" yerine liste Excel'in açtığı CSV olarak indirilir; işlevi kapatılmış olan "Aktar" düğmesi alınmadı. Listede vergi no da gösterilir |
| `Muhasebe/MuhasebeUclari.cs` | Muhasebe tanım uçları: `GET /api/muhasebe/saymanlik/liste`; `POST /api/muhasebe/saymanlik/kaydet`, `/sil`, `/tasi`; `GET /api/muhasebe/birim/tanimlar`, `/liste`; `POST /api/muhasebe/birim/kaydet`, `/sil`. İşlemler `TNS.MUH.Arac.Tanimla()` üzerinden yapılır; servis metotları kullanıcı almadığı için yetki uçlarda denetlenir |
| `wwwroot/css/tanim.css` | Tanım sayfalarının ortak yerleşimi (`tanim-sayfa`, `.tanim`): solda arama + liste, sağda form; etiketler solda; geniş ekranda bölmeler kendi içinde kayar. Ek işlemler için `<dialog class="tanim-pencere">`. Yeni tanım sayfaları bunu kullanır |
| `Pages/KYM/KullaniciArama.cshtml` | Kullanıcı Arama (eski `KYM/KullaniciArama.aspx`): solda sorgu alanları, sağda AG Grid listesi; açılışta liste boştur, son arama tarayıcı oturumunda saklanır. `wwwroot/js/kullaniciarama.js`. Alınmayanlar: entegrasyon sunucusunda arama, grup oluşturma/atama, yetki tipli Excel raporu, Evrak yetki sütunu; "Yazdır" yerine CSV |
| `Pages/KYM/KullaniciGiris.cshtml` | Kullanıcı Giriş (eski `KYM/KullaniciGiris.aspx` + `KullaniciYetkiGiris`, `YetkiTipiDegerSec`, `YetkiTipiAgactanSec`; yalnızca sistem, kullanıcı ve destek yöneticisi): kullanıcı bilgileri, kullanıcı tipleri ve yetki değerleri (yazıyla / listeden / ağaçtan), yeni parola atama, aktif/pasif yapma, Mernis'ten ad soyad. `wwwroot/js/kullanicigiris.js`. Eski sayfa düzenlenen kullanıcıyı oturumda tutuyordu; burada tipler ve değerler tarayıcıda tutulur, Kaydet'te birlikte gönderilir |
| `Kullanici/KullaniciUclari.cs` | Kullanıcı uçları: `GET /api/kullanici/arama-tanimlar`, `/liste`, `/giris-tanimlar`, `/oku`, `/mernis`, `/yetki-liste`; `POST /tip-yetkileri`, `/kaydet`, `/parola-ata`, `/durum`. Kaydederken kullanıcı veri tabanından yeniden okunur; yapanın düzenleyemeyeceği tipler ve yetki değerleri olduğu gibi kalır. Eskiye göre farklar: doğum tarihi var olan kullanıcıda zorunlu değil (boşsa kayıtlı tarih korunur); silinen tipin yetki değeri başka bir tip de kullanıyorsa silinmez |
| `Pages/Organizasyon/GenelGorevAna.cshtml` | Genel Görev Tanımlama (eski `Organizasyon/GenelGorevAna.aspx` + çerçevedeki `GenelGorevDetay.aspx`; yalnızca sistem yöneticisi): solda görevler, sağda görevin bilgileri (ad, açıklama, max. görev sayısı, zorunlu, birim türleri) ve yazılım yetkileri. `wwwroot/js/genelgorev.js`. Adreste `?orgKod` yok; birime bağlı görevler anahtarıyla (`birimKodu_sıraNo`) desteklenir ama o ekran (ORGAna) henüz taşınmadı |
| `Pages/Organizasyon/GorevAta.cshtml` | Göreve Kişi Atama (eski "Gruba Kullanıcı Ata": `ORGAna.aspx?gorevata=1` içindeki birim ağacı + `OrgGorev.aspx` + `OrgGorevKisit.aspx`; yalnızca sistem yöneticisi): solda birim ağacı (düzey düzey okunur), ortada birimin görevleri, sağda göreve atanmış kişiler; kişi ekleme (kullanıcılar arasında arama), görevden silme, kişi kısıtı (faaliyet kodu). Kişi ile görev T.C. kimlik numarasıyla ilişkilenir. `?kullanici=kod` ile açılırsa o kullanıcının şimdiki görevleri üstte görünür ve tek tuşla eklenir. `Organizasyon/GorevAtaUclari.cs` (`/api/gorevata/...`), `wwwroot/js/gorevata.js`, `wwwroot/css/gorevata.css`. Kullanıcı Giriş'teki **Görevler** ve Kullanıcı Arama'daki **Görev Tanımla** / **Göreve Kişi Ata** tuşları bu sayfaları `Sgb.pencere` ile pencerede açar (`ortak.js`; sayfa çerçevede `?menuYok=1` ile gelir). Birim ağacı ve görev listesi ortak ağaç bileşeniyle çizilir: `Sgb.agac(ul, { yukle, secilince / isaretli + isaretlenince, ilkAcik, duz, bosMetin, hata })` (`ortak.js`, görünümü `ortak.css` içinde `.sgb-agac`); düğümler `{ kod, ad, altVar }` biçimindedir, alt düğümler dal açılınca okunur, alt düğümü olmayanın yanında açma ikonu çizilmez. Kullanıcı Giriş'teki ağaçtan yetki değeri seçme de aynı bileşeni onay kutulu kullanır; yeni ağaçlı ekranlar (kod seçimi vb.) bunu kullanmalıdır. Taşınmayanlar: Excel raporları, kişinin personel listesinden seçilmesi (Personel ekranları taşınmadı) |
| `Pages/Organizasyon/ORGAna.cshtml` | Organizasyon Şeması (eski `Organizasyon/ORGAna.aspx` + çerçevelerdeki `ORGAgac.aspx`, `ORGBirim.aspx`, `OrgGorev.aspx`): solda birim ağacı (`Sgb.agac`, birim türüne göre simge; Yeni Birim, Sil, Birim Aç), sağda iki sekme. **Birim Bilgileri**: birim özellikleri (yönetici, kurum, harcama, bütçe, taşınır, ambar, personel, evrak, iletişim, coğrafi birim, çizim) ve özelliğe göre açılan bölümler (yönetici unvanları, muhasebe kodu, bütçe kodları, harcama kodları, ambar, vergi, evrak, ülke/bölge/il/ilçe, iletişim); Kaydet, Birim Kapat. **Görevler**: seçili birim için `GorevAta?orgKod=..&orgAd=..` sayfası çerçevede (birim ağacı gizli). `Organizasyon/OrgBirimUclari.cs` (`/api/orgbirim/...`), `wwwroot/js/organa.js`, `wwwroot/css/organa.css`. Raporlar eski kodun aynısıdır (`OrtakClass.Tablo` / `TabloAspose`): **Yazdır** seçili birim ve altındaki birimleri (seçili yoksa bütün şemayı) `KYM\OrgBirimSablon.xlt` şablonuyla, **Görev Yaz** (yalnızca sistem yöneticisi) kullanıcıların birimlere göre görevlerini `KYM\KullaniciGorevGruplu.xltx` şablonuyla Excel'e yazar. Sayfayı menüde Organizasyon Şeması'nı görebilenler açar; yalnızca sistem yöneticisi değiştirir (`/OrgSemaReadOnly` > 0 ise kimse), diğerleri yetkili oldukları birimleri salt okunur görür. `?sablon=1` şablon şemasını düzenler. Taşınmayanlar: diğer sekmeler (Stratejik Plan, Ödenek Durumu, Taşınır, Personel, Evrak, Mevzuat, E-Fatura), sürükle-bırak ile taşıma, şablon uygulama, Excel'den aktarma, yönetici unvanına kişi seçme (Personel listesi taşınmadı; kayıtlı kişi korunur, kaldırılabilir), vergi numarasının vergi dairesi servisinden doğrulanması (vergi dairesi elle yazılır) |
| `Pages/ButceHazirlikv2/GenelKodTanimla.cshtml` | Genel Kod Tanımlama (eski `ButceHazirlikv2/GenelKodTanimla.aspx` + çerçevedeki `GenelKodTanimlaDetay.aspx`): solda kod ağacı (`Sgb.agac`, ada göre süzme; yıla bağlı türlerde önceki/sonraki yıl), sağda kodun her dildeki adı ve açıklaması ile koda uyan ek alanlar; Kaydet, Yeni, Sil, Önceki Yıldan Kopyala, Rapor Hazırla (Excel; eski kodla, `BKD\KodListesi.xlt` şablonundan) ve Dosyadan Yükle (Excel'den kod yükleme; eski kodla okunur: sütunlar kod, her dil için ad ve açıklama, ek alanlar; başlık satırı yok). Aynı sayfa adresteki `Tur` ile farklı kodları düzenler: menüde **Birim Tür Tanımlama** = `?Tur=21`, İl-İlçe Bilgileri = `?Tur=20`, bütçe sınıflandırmaları = `?Tur=1..7`, Alt Faaliyetler = `?Tur=14`. `ButceKod/KodTanimUclari.cs` (`/api/kod/...`), `wwwroot/js/genelkodtanimla.js`, `wwwroot/css/kodtanim.css`. Yetki eskisi gibi menüden gelir: kullanıcı menüde o `Tur` seçeneğini görebiliyorsa düzenler (`MenuOkuyucu.SayfaYetkili`); menüde olmayan tür yalnızca sistem yöneticisine açıktır. Kodun yeni mi kayıtlı mı olduğu kod okunarak belirlenir (eskisi uzay sorgusuyla yapıyordu; uzayı olmayan türlerde yanılıyordu). Taşınmayanlar: sgb.net'ten aktarma, türe bağlı ek sayfa sekmesi |
| `Pages/PerformansButce/StratejikPlanAna.cshtml` | Stratejik Plan (eski `PerformansButce/StratejikPlanAna.aspx` ve pencerede açtığı `DonemSec`, `Misyon`, `Vizyon`, `StratejikAmac`, `StratejikHedef` sayfaları): seçili kurum ve plan döneminin misyonu, vizyonu, amaçları ve altlarında hedefleri. Alt sayfalar aynı sayfadaki pencerelerdir (`<dialog>`): Dönem Seç, Misyon (kurum, plan yılı, dönem; planı başka yıla kopyalama), Vizyon, Amaç, Hedef (sekmeler: sorumlu/ilgili birimler, riskler, stratejiler, tespitler, ihtiyaçlar, program bağı, üst politika belgeleri) ve hedefin Göstergeler / Ödenekler tabloları. `StratejikPlan/StratejikPlanUclari.cs` (`/api/strplan/...`), `wwwroot/js/stratejikplan.js`, `wwwroot/css/stratejikplan.css`. Üzerinde çalışılan kurum ve dönem eskisi gibi kullanıcının `StrPlan` değişkeninde (`kurumKod-yıl`) saklanır. Yetki menüden gelir (Stratejik Plan seçeneğini görebilenler). Raporlar eski kodun aynısıyla üretilir: Hedef Kartları ve hedef penceresindeki Yazdır (Word; Aspose.Words ile `RaporSablonYol` ayarındaki klasörde `PEB\StrHedefKarti.dotx` şablonundan), Hedef - Birim Tablosu ve Tahmini Maliyet Tablosu (Excel; `OrtakClass.IkiBoyutluTablo`). Bunun için eski `OrtakClass`'tan şu dosyalar `Istemci/OrtakClass/` altına alındı: `Tablo.cs` (arayüz ve sabitler; eski `GenelIslemler.cs`'ten), `TabloAspose.cs` (olduğu gibi; yalnızca `using Range = Aspose.Cells.Range;` eklendi), `IkiBoyutluTablo.cs` (eski `ExcelRapor.cs`'in ikinci yarısı), `AsposeAraclari.cs` (`AsposeAlanaYaz`, `AsposeBuilderaDocEkle`, `DosyaIslem.DosyaAdUret`, lisans yükleme). `Aspose.Words.dll` ve `Aspose.Cells.dll` eski projedeki sürümlerdir (`Sunucu/OrtakDLL`), lisans `Istemci/Properties/Aspose.Total.lic` varsa derlemeye gömülür (eski `Global.asax`'taki gibi). Lisans dosyası depoya girmez (`.gitignore`: `*.lic`); yeni bir çalışma kopyasında eski projedeki `Istemci\Properties\Aspose.Total.lic` elle kopyalanmalıdır, yoksa raporlar Aspose deneme damgasıyla çıkar; .NET 8'de çalışmaları için `System.Drawing.Common` ve `System.Security.Permissions` paketleri eklendi. Başka raporlar taşınırken de bu sınıflar kullanılmalıdır. Dosyalar `Sgb.indir` ile iner (`ortak.js`; sunucu dosya yerine `{ hata }` döndürürse bildirim gösterir). Taşınmayanlar: gösterge tanımlarını düzenleme sayfası, menüdeki Genel Değerlendirme / Kurum Değerlendirme sayfaları |
| `Pages/PerformansButce/StratejikGostGerc.cshtml` | İzleme Tablosu (eski `PerformansButce/StratejikGostGerc.aspx`): solda arama (kurum, plan dönemi, gerçekleşme yılı, dönem: altı aylık izleme / yıl sonu değerlendirme) ve hedefler; sağda seçili hedefin göstergelerinin gerçekleşmeleri (gerçekleşme değeri, açıklama; yıl sonunda ilgililik, etkililik, etkinlik, sürdürülebilirlik ayrı sütunlarda; tablo sığmazsa sağa sola kaydırılır, gösterge sütunu yerinde kalır), değerlendirmeler, sapma nedeni, alınacak önlem. Performans yüzdesi yazarken hesaplanır (`StrHedefGosterge.Oranla` ile aynı kural). `StratejikPlan/IzlemeUclari.cs` (`/api/strizleme/...`), `wwwroot/js/stratejikgostgerc.js`, `wwwroot/css/stratejikgostgerc.css`. Raporlar eski kodun aynısıdır, `Istemci/PerformansButce/` altına alındı: `HedefIzlemeRaporlari.cs` (Yazdır, Yazdır (v2), Rapor, Rapor (v1); Word, şablonlar `PEB\StrHedef[Rapor|v2][Izleme|Degerlendirme].dotx`), `SPIzlemeKurum.cs` (Hedef Bşrm, Göst. Bşrm; Excel, şablonlar `PEB\SPKurumIzleme[Gost].xltx`; eski sayfa sınıfının durağan yordamları), `StratejikPlanYardimcilari.cs` (raporların eski adlarıyla çağırdığı yardımcılar). Kaydederken satırlar sunucuda yeniden okunur, yalnızca kullanıcının değiştirebildiği alanlar üzerine yazılır. Dağılım ve Seyir tuşları grafiği pencerede açar (`OrtakSayfa/GrafikGoster`) |
| `Pages/PerformansButce/SPIzlemeKurum.cshtml` | Kurum Değerlendirme (eski `PerformansButce/SPIzlemeKurum.aspx`): solda arama (kurum, yıl, dönem: 6 aylık / yıllık, başarım yüzdesi alt ve üst sınırı; boş sınır eski sayfadaki -1'dir) ve seçilen yılda başarı düzeylerine göre hedef sayıları; sağda hedefler ve planın ilk yılından seçilen yıla kadar başarım yüzdeleri. Satırlar başarı notuna göre renklenir (eski renkler), satır açılınca hedefin göstergelerinin başarımları gelir, kalem tuşuyla hedef açıklaması düzenlenir (KYM büyük metin, `SP` / `HDFA_yıl_kurum_hedef`; anahtardaki yıl listede okunan plan yılıdır). `?yil=&kur=&donem=` ile çağrılırsa liste hemen gelir. `StratejikPlan/KurumIzlemeUclari.cs` (`/api/strkurum/...`), `wwwroot/js/spizlemekurum.js`, `wwwroot/css/spizlemekurum.css`. Veri ve iki Excel raporu (Yazdır (Genel), Yazdır (Özet)) eski kodla hazırlanır: `Istemci/PerformansButce/SPIzlemeKurum.cs` (dört yordamı public yapıldı). Hedef Başarı Düzeyi ve Başarı Seyri sekmeleri grafiği sayfanın içinde çizer (`/api/grafik`, `Sgb.grafik`) |
| `Pages/OrtakSayfa/GrafikGoster.cshtml` | Grafik gösterme sayfası (eski `OrtakSayfa/GrafikGoster.aspx?param=...`). Eski sayfa `param`ın ilk parçasına göre ilgili modülün `IChartGenerator` sınıfını çalıştırıp Ext JS grafik tanımı basıyordu. Yenisinde `OrtakSayfa/GrafikUclari.cs` (`GET /api/grafik?param=`, aynı `param`) kütüphaneden bağımsız sade veri döndürür (`GrafikVeri`: başlık, tür pasta / cizgi / sutun, etiketler, seriler) ve grafik `ortak.js`teki `Sgb.grafik(kutu, veri)` ile SVG olarak çizilir (ek kütüphane yok; her sayfa kendi içinde de kullanabilir). `wwwroot/js/grafikgoster.js`. Taşınan üretici: 1 = `Istemci/PerformansButce/GrafikDataUret.cs`, türler `sp1` (Başarı Düzeyine Göre Hedef Dağılımı) ve `sp2` (Stratejik Planın Başarı Seyri); veri yordamları eski koddan aynen alındı. Kullanan yerler: Kurum Değerlendirme sekmeleri, İzleme Tablosu'ndaki Dağılım ve Seyir tuşları (pencerede). Öteki üreticiler (2 kullanım, 3 bütçe hazırlık, 4 ekonomik gösterge, 5-6 okul, 7 görev yönetimi, 8-9 teftiş) ait oldukları sayfalarla birlikte `GrafikUclari.Ureticiler` listesine eklenecek; o zamana kadar uç "Bu grafik henüz taşınmadı" der |
| `Organizasyon/YetkiTanimlari.cs` | Eski 25 `Organizasyon/YETxxx.aspx` sayfasının karşılığı: her yazılım yetkisi türünün alanları (damga adı, türü, etiketi, seçenek listesinin yetki tipi) tablo olarak tutulur ve tek ortak pencereyle gösterilir. Hangi tanımın kullanılacağı eskisi gibi `BirimGorev.SayfaAd` ile bulunur. Yeni yetki türü = buraya bir satır. Eski `YETKIMLIKPAYLASIMI.aspx` yanlışlıkla sistem yöneticisi (14) olarak kaydediyordu; burada kendi sırası (16) kullanılır. `YETEVRAKEski.aspx` alınmadı |
| `Organizasyon/GenelGorevUclari.cs` | Uçlar: `GET /api/genelgorev/liste`, `/oku`, `/yetki`; `POST /yeni`, `/kaydet`, `/yetki-kaydet`, `/yetki-sil`. Seçenek listesini sağlayan modül çalışmıyorsa (ör. Evrak) pencere uyarıyla açılır; kayıtlı olup listede bulunmayan değerler korunur |
| `Pages/Duyuru/EkliDosyalarForm.cshtml` | Ekli dosyalar (eski `EkliDosyalarForm.aspx`). Eski adresler çalışır: `?mdl=DUY&frm=12` liste, `?no=5&mdl=DUY&frm=12` dosyanın kendisi (duyuru metnindeki bağlantılar), `brm=1` birim adlı salt okunur liste |
| `EkliDosya/EkliDosyaUclari.cs`, `wwwroot/js/eklidosya.js` | Ortak ekli dosyalar bileşeni `Sgb.ekliDosyalar(el, { modul, form })`: yükleme (açıklamalı), AG Grid listesi, indirme, silme; DUY/YRD modüllerinde kilitleme ve bağlantı adresi kopyalama. Uçlar: `GET /api/ekli-dosya`, `/indir`; `POST /yukle`, `/sil`, `/kilit`. Yetki denetimi iş kurallarındadır (`KYMEkliDosyalar.YetkiKontrol`). Dosya boyu sınırı `EkliDosyaAzamiMB` (varsayılan 20). Betik çalıştırabilecek türler (HTML, SVG ...) her zaman indirilir, sayfada açılmaz |
| `wwwroot/js/ortak.js` | Sayfaların ortak yardımcıları (jQuery): `Sgb.al` / `Sgb.gonder` (JSON uçları, istek jetonu, 401'de girişe dönüş), `Sgb.mesaj`, `Sgb.grid` (Türkçe AG Grid), `Sgb.tarih`. `_Layout` jQuery ile birlikte her sayfaya ekler |
| `wwwroot/js/editor.js` | Ortak HTML editörü `Sgb.editor(textarea, ayarlar)`: Jodit 4 (MIT) üzerine kurulu; yazı tipi, boyut, yazı/arka plan rengi, listeler, hizalama, tablo, bağlantı, kaynak kodu. Biçimler satır içi stil olarak yazılır. `html()`, `html(deger)`, `bos()`. Kullanan sayfa `lib/jodit/jodit.min.css`, `lib/jodit/jodit.min.js` ve `js/editor.js`'i ekler |
| `wwwroot/lib/` | Bootstrap 3.3.6, jQuery 3.7.1, AG Grid Community 32.3.9 (`ag-grid-community.min.js`, stilleri içinde), Jodit 4.17.3. Tablo/editör kullanan sayfa kendisi ekler |
| `Kaynak/*.resx`, `Metin.cs` | Çok dilli metinler: `Metin.Ortak` (eski OrtakClassRes), `Metin.Menu` (menü, üst şerit, giriş), `Metin.Duyuru` (eski Duyuru kaynağı + yeni metinler), `Metin.Muhasebe` (eski ButceMuhasebe kaynağından gerekenler + yeni metinler; yalnızca Türkçe) |
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
5. **Henüz taşınmayan servis**: yalnızca EVR (evrak). Onu çağıran kod yolları "servis kayıtlı değil" hatası verir.
   Taşınan iş kurallarından yalnızca KYM, KHM ve MUH'un bir kısmı çalıştırılarak denendi; diğerleri yalnızca derlendi.
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

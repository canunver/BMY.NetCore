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

1. **KPM / Mernis** (`Mernis.KPS.Types`, `Mernis.KPSv2.Types`): WSE ve WIF kullanan SOAP istemcileri .NET 8'de yok.
   `dotnet-svcutil` ile yeniden üretilmeli ya da KPM pilot dışında bırakılmalı.
2. **KYMServisI `Web References\IcisleriKullaniciServis`**: ASMX istemcisi (`SoapHttpClientProtocol`) .NET 8'de yok;
   `KullaniciServis.cs` bunu kullanıyor.
3. **Oracle**: `System.Data.OracleClient` yerine `Oracle.ManagedDataAccess.Core` konuldu (`VERServis`). Oracle kullanan kurum varsa denenmeli.
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

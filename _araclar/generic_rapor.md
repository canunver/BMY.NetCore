# Generic dönüşüm raporu

Arayüzlerde `ObjectArray` / `ObjectVAN` / `TNSCollection` dönen metotlar ve bulunan eleman tipi.

## KHM

28 metot: 25 dönüştürüldü, 3 dönüştürülmedi.

### Dönüştürülmeyenler (elle karar gerekli)

| Metot | Dönüş | Durum | Bulunan tipler |
|---|---|---|---|
| `KesinHesapFonksiyonelCetvelHazirla` | TNSCollection | bilinmiyor | ?, TNS.Sonuc |
| `KesinHesapFinansalCetvelHazirla` | TNSCollection | bilinmiyor | ?, TNS.Sonuc |
| `KesinHesapEkonomikCetvelHazirla` | TNSCollection | bilinmiyor | ?, TNS.Sonuc |

### Dönüştürülenler

| Metot | Yeni dönüş tipi |
|---|---|
| `KesinHesapGiderCetveliHazirla` | `TNSCollection<TNS.BHMv2.UcgenSatir>` |
| `KesinHesapKodCetvelHazirla` | `TNSCollection<TertipBilgi>` |
| `KesinHesapGiderCetveliHazirlaKurumsal` | `TNSCollection<TNS.BHMv2.UcgenSatir>` |
| `KesinHesapGiderCetveliHazirlaRaporTur` | `TNSCollection<TNS.BHMv2.UcgenSatir>` |
| `KesinHesapCaprazCetvelHazirla` | `TNSCollection<TertipBilgi>` |
| `KesinHesapMahsupCetveliHazirla` | `TNSCollection<TertipHarcamaBilgi>` |
| `BIDevirAkreditifListele` | `TNSCollection<TertipOdenekBilgi>` |
| `KHDevirAkreditifListele` | `TNSCollection<TertipOdenekBilgi>` |
| `KesinHesapGelirCetveliHazirla` | `TNSCollection<TNS.BHMv2.UcgenSatir>` |
| `GiderGerekceListele` | `ObjectArray<GiderGerekce>` |
| `KesinHesapMizanHazirla` | `TNSCollection<TNS.BHMv2.UcgenSatir>` |
| `KesinHesapGiderCokYilli` | `TNSCollection<TertipBilgiler>` |
| `KesinHesapGelirCetveliHazirlaCokYilli` | `TNSCollection<TNS.BHMv2.UcgenSatir>` |
| `KesinHesapDevirCetveliHazirla` | `TNSCollection<TNS.BHMv2.UcgenSatir>` |
| `YilIciIptalListele` | `TNSCollection<TertipOdenekBilgi>` |
| `GiderTertipDegerListele` | `ObjectArray<TertipBilgi>` |
| `GiderTertipListele` | `ObjectArray<TNS.BKD.Tertip>` |
| `GiderGerekceListeleDetay` | `ObjectArray<GiderGerekceDetay>` |
| `ItaAmirleriListele` | `TNSCollection<KHItaAmiri>` |
| `FaaliyetAciklamaDamgaSayBul` | `ObjectArray<GiderGerekceDetay>` |
| `ListeleOnBulgu` | `ObjectArray<SayBulgu>` |
| `OkuOnBulgu` | `ObjectVAN<SayBulgu>` |
| `BulguIliskiliBirimListele` | `ObjectArray<TNS.MUH.MuhasebeBirim>` |
| `OnBulguCevapListele` | `ObjectArray<SayBulgu>` |
| `BirimGirisIzniListele` | `ObjectArray<BirimGirisIzni>` |

## KYM

18 metot: 17 dönüştürüldü, 1 dönüştürülmedi.

### Dönüştürülmeyenler (elle karar gerekli)

| Metot | Dönüş | Durum | Bulunan tipler |
|---|---|---|---|
| `LogoListele` | ObjectArray | bilinmiyor | - |

### Dönüştürülenler

| Metot | Yeni dönüş tipi |
|---|---|
| `KullaniciAra` | `ObjectArray<Kullanici>` |
| `KullaniciRaporla` | `ObjectArray<Kullanici>` |
| `KullaniciAdOner` | `ObjectArray<Kullanici>` |
| `KullaniciSessionListele` | `ObjectArray<KullaniciIslemYapan>` |
| `KullaniciSessionGrafik` | `ObjectArray<KullaniciIslemYapan>` |
| `DegiskenleriListele` | `TNSCollection<string>` |
| `DUYListesi` | `ObjectArray<Duyuru>` |
| `EPostaGonderOncekiGonderiler` | `ObjectArray<Posta>` |
| `EPostaGonderilenlerListele` | `ObjectArray<Posta>` |
| `HataDosyaListele` | `ObjectArray<EkliDosyalar>` |
| `BuyukMetinListele` | `ObjectArray<TNS.StrKodStrAd>` |
| `ListeleEvrakAktarim` | `ObjectArray<EvrakAktarimMeta>` |
| `ListeleBirimMesajlari` | `ObjectArray<BirimMesaj>` |
| `ListeleEkliDosyalar` | `ObjectArray<EkliDosyalar>` |
| `OkuEkliDosyaIcerik` | `ObjectVAN<EkliDosyalar>` |
| `YerleskeOdaListele` | `ObjectArray<YerleskeOda>` |
| `KullaniciIslemTarihceListele` | `ObjectArray<Kullanici>` |

## ORG

9 metot: 7 dönüştürüldü, 2 dönüştürülmedi.

### Dönüştürülmeyenler (elle karar gerekli)

| Metot | Dönüş | Durum | Bulunan tipler |
|---|---|---|---|
| `GorevYerTarihListele` | ObjectArray | karisik | TNS.ORG.GorevYerTarih, TNS.PER.GorevYerTarih |
| `GorevPersonelListele` | ObjectArray | karisik | TNS.ORG.GorevPersonel, TNS.PER.GorevPersonel |

### Dönüştürülenler

| Metot | Yeni dönüş tipi |
|---|---|
| `GorevListele` | `ObjectArray<Gorev>` |
| `GorevSorgula` | `ObjectArray<Gorev>` |
| `GorevTertipListele` | `ObjectArray<GorevTertip>` |
| `OrganizasyonBirimListeleKullanici` | `ObjectArray<OrganizasyonBirim>` |
| `OrganizasyonBirimListele` | `ObjectArray<OrganizasyonBirim>` |
| `OrganizasyonBirimListele` | `ObjectArray<OrganizasyonBirim>` |
| `Il_IlceBul` | `ObjectArray<OrganizasyonBirim>` |

## UZY

2 metot: 0 dönüştürüldü, 2 dönüştürülmedi.

### Dönüştürülmeyenler (elle karar gerekli)

| Metot | Dönüş | Durum | Bulunan tipler |
|---|---|---|---|
| `UzayDegeriToplu` | ObjectArray | karisik | double, string |
| `UzaylariListele` | TNSCollection | bilinmiyor | - |

### Dönüştürülenler

| Metot | Yeni dönüş tipi |
|---|---|

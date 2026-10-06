using System;

namespace TNS.HRC
{

    public enum ENUMUCDURUM
    {
        HEPSI = 0,
        DOGRU = 1,
        YANLIS = 2,
    }

    public enum ENUMCikisDurum
    {
        TANIMSIZ = 0,
        HEPSI = 1,
        CIKISYAPILAN = 2,
        CIKISYAPILMAYAN = 3
    }

    public enum ENUMIslemTuru
    {
        TANIMSIZ = 0,
        KAYIT = 1,
        SILME = 2,
        LISTELEME = 3,
        GUNCELLEME = 4,
    }

    public enum ENUMSGKBelgeDurum
    {
        TANIMSIZ = 0,
        YENI = 1,
        CEVAPLANDI = 2,
        HATA = 3
    }

    /// <summary>
    /// Taahhütname türü
    /// </summary>
    public enum ENUMTaahhutname
    {
        /// <summary>
        /// Vergi Borcu
        /// </summary>
        VERGIBORCU = 1,
        /// <summary>
        /// SGK Prim
        /// </summary>
        SGKPRIM = 2,
    }

    /// <summary>
    /// Okuma Görme Yazma yetki türü
    /// </summary>
    public enum ENUMOGY
    {
        /// <summary>
        /// Onaylama
        /// </summary>
        Onaylama = 1,
        /// <summary>
        /// Görme
        /// </summary>
        Gorme = 2,
        /// <summary>
        /// Yazma
        /// </summary>
        Yazma = 3,
        /// <summary>
        /// Onay Kaldır
        /// </summary>
        OnayKaldir = 5,
        /// <summary>
        /// İptal
        /// </summary>
        Iptal = 6,
    }

    /// <summary>
    /// Kurum Bütçe Türü
    /// </summary>
    public enum ENUMKurumButceTur
    {
        /// <summary>
        /// Genel Bütçe
        /// </summary>
        GENELBUTCE = 1,
        /// <summary>
        /// Özel Bütçe
        /// </summary>
        OZELBUTCE = 2,
    }

    /// <summary>
    /// Ödenek Kontrol Türü
    /// </summary>
    public enum ENUMOdenekKontrolTur
    {
        /// <summary>
        /// Ödenek Gönderme
        /// </summary>
        ODENEKGONDERME = 1,
        /// <summary>
        /// Ödenek
        /// </summary>
        ODENEK = 2,
    }

    /// <summary>
    /// Ödeme Belge Türü
    /// </summary>
    public enum ENUMOdemeBelgeTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Ödeme Emri Belgesi
        /// </summary>
        ODEMEEMRI = 1,
        /// <summary>
        /// Muhasebe İşlem Fişi
        /// </summary>
        MIF = 2,
    }

    /// <summary>
    /// Abone Türü
    /// </summary>
    public enum ENUMAboneTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Elektrik
        /// </summary>
        ELEKTRIK = 1,
        /// <summary>
        /// Su
        /// </summary>
        SU = 2,
        /// <summary>
        /// Doğalgaz
        /// </summary>
        DOGALGAZ = 3,
        /// <summary>
        /// Telefon
        /// </summary>
        TELEFON = 4,
        /// <summary>
        /// ADSL
        /// </summary>
        ADSL = 5,
        /// <summary>
        /// Avea
        /// </summary>
        AVEA = 6,
        /// <summary>
        /// Vodafone
        /// </summary>
        VODAFONE = 7,
        /// <summary>
        /// Turkcell
        /// </summary>
        TURKCELL = 8,
        /// <summary>
        /// Özel Servisler
        /// </summary>
        OZELSERVISTELLER = 9,
        /// <summary>
        /// Noktadan Noktaya
        /// </summary>
        NOKTADANNOKTAYA = 10,
        /// <summary>
        /// Hat Kirası
        /// </summary>
        HATKIRASI = 11,
        /// <summary>
        /// Kablolu TV
        /// </summary>
        KABLOLUTV = 12,
        /// <summary>
        /// Jeotermal Nükleer Enerji
        /// </summary>
        JEOTERMALNUKLEERENERJI = 13,
        /// <summary>
        /// Gsm İnternet
        /// </summary>
        GSMINTERNET = 14,
        AKARYAKIT = 15,
        COP = 16,
        GUVENLIK = 17,
        /// <summary>
        /// Diğer
        /// </summary>
        DIGER = 99,
    }

    /// <summary>
    /// Süreç Durum Türü
    /// </summary>
    public enum ENUMSurecDurum
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yeni
        /// </summary>
        YENI = 1,
        /// <summary>
        /// Devam Ediyor
        /// </summary>
        DEVAMEDIYOR = 2,
        /// <summary>
        /// Son Aşamada
        /// </summary>
        SONASAMADA = 3,
        /// <summary>
        /// Bitti
        /// </summary>
        BITTI = 4,
        /// <summary>
        /// Arşiv
        /// </summary>
        ARSIV = 5,
        /// <summary>
        /// İptal
        /// </summary>
        IPTAL = 9,
    }

    /// <summary>
    /// Süreç Parça Durum Türü
    /// </summary>
    public enum ENUMSurecParcaDurum
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Başladı
        /// </summary>
        BASLADI = 1,
        /// <summary>
        /// Devam Ediyor
        /// </summary>
        DEVAMEDIYOR = 2,
        /// <summary>
        /// Tamamlandı
        /// </summary>
        TAMAMLANDI = 3,
    }

    /// <summary>
    /// Komisyon Türü
    /// </summary>
    public enum ENUMKomisyon
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yaklaşık Maliyet Firma
        /// </summary>
        YAKLASIKMALIYETFIRMA = 1,
        /// <summary>
        /// Yaklaşık Maliyet Dönem
        /// </summary>
        YAKLASIKMALIYETDONEM = 2,
        /// <summary>
        /// Piyasa Fiyat Araştırması
        /// </summary>
        PIYASAFIYATARASTIRMASI = 3,
        /// <summary>
        /// İhale
        /// </summary>
        IHALE = 4,
        /// <summary>
        /// Muayene Kabul
        /// </summary>
        MUAYENEKABUL = 5,
    }

    /// <summary>
    /// Komisyon Görev Türü
    /// </summary>
    public enum ENUMKomisyonGorev
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Başkan
        /// </summary>
        BASKAN = 1,
        /// <summary>
        /// Üye
        /// </summary>
        UYE = 2,
        /// <summary>
        /// Yedek Üye
        /// </summary>
        YEDEKUYE = 3,
    }

    /// <summary>
    /// İhale Usul Türü
    /// </summary>
    public enum ENUMIhaleUsul
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Açık İhale
        /// </summary>
        ACIK = 1,
        /// <summary>
        /// Belirli İstekli İhale
        /// </summary>
        BELIRLIISTEKLI = 2,
        /// <summary>
        /// Pazarlık Usulü İhale
        /// </summary>
        PAZARLIKUSULU = 3,
        /// <summary>
        /// Doğrudan Temin İhale
        /// </summary>
        DOGRUDANTEMIN = 4,
    }

    /// <summary>
    /// İhale Teklif Belirleme Türü
    /// </summary>
    public enum ENUMIhaleTeklifBelirlemeTuru
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// En düşük fiyat
        /// </summary>
        ENDUSUKFIYAT = 1,
        /// <summary>
        /// Diğer Unsurlar
        /// </summary>
        DIGERUNSURLAR = 2
    }

    /// <summary>
    /// İhale Teklif Türü
    /// </summary>
    public enum ENUMIhaleTeklifTuru
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Götürü Bedel
        /// </summary>
        GOTURUBEDEL = 1,
        /// <summary>
        /// Birim Fiyat
        /// </summary>
        BIRIMFIYAT = 2
    }

    /// <summary>
    /// İhale Konsorsiyum Teklif Türü
    /// </summary>
    public enum ENUMIhaleKonsorsiyumTeklif
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Verilebilir
        /// </summary>
        VERILEBILIR = 1,
        /// <summary>
        /// Verilemez
        /// </summary>
        VERILEMEZ = 2
    }

    /// <summary>
    /// İhale Katılım Türü
    /// </summary>
    public enum ENUMIhaleKatilim
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Sadece Yerli
        /// </summary>
        SADECEYERLI = 1,
        /// <summary>
        /// Tüm İstekliler
        /// </summary>
        TUMISTEKLILER = 2,
        /// <summary>
        /// Tüm İstekliler İndirimli
        /// </summary>
        TUMISTEKLILERINDIRIMLI = 3
    }

    /// <summary>
    /// İlan Zaman Türü
    /// </summary>
    public enum ENUMIlanZamanTuru
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// İhale Tarihi
        /// </summary>
        IHALETARIHI = 1,
        /// <summary>
        /// Son Başvuru Tarihi
        /// </summary>
        SONBASVURUTARIHI = 2,
    }

    /// <summary>
    /// Teklif Türü
    /// </summary>
    public enum ENUMTeklifTuru
    {
        /// <summary>
        /// Yaklaşık Maliyet Firma
        /// </summary>
        YAKLASIKMALIYETFIRMA = 1,
        /// <summary>
        /// Yaklaşık Maliyet Dönem
        /// </summary>
        YAKLASIKMALIYETDONEM = 2,
        /// <summary>
        /// Piyasa Fiyat Araştırması
        /// </summary>
        PIYASAFIYATARASTIRMASI = 3,
        /// <summary>
        /// Ortalama
        /// </summary>
        ORTALAMA = 4,
        /// <summary>
        /// Kısmi Teklif
        /// </summary>
        KISMITEKLIF = 5,
    }

    /// <summary>
    /// Yardım Türü
    /// </summary>
    public enum ENUMYardim
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yardım
        /// </summary>
        YARDIM = 1,
        /// <summary>
        /// Mevzuat
        /// </summary>
        MEVZUAT = 2,
    }

    /// <summary>
    /// İhale Türü
    /// </summary>
    public enum ENUMIhaleTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Mal
        /// </summary>
        MAL = 1,
        /// <summary>
        /// Hizmet
        /// </summary>
        HIZMET = 2,
        /// <summary>
        /// Yapım
        /// </summary>
        YAPIM = 3,
    }

    /// <summary>
    /// Teklif Geliş Türü
    /// </summary>
    public enum ENUMTeklifGelisTuru
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Elden
        /// </summary>
        ELDEN = 1,
        /// <summary>
        /// Posta
        /// </summary>
        POSTA = 2,
        /// <summary>
        /// Gelmedi
        /// </summary>
        GELMEDI = 3,
    }
    /// <summary>
    /// Teklif Dosyası Türü
    /// </summary>
    public enum ENUMTeklifDosyasi
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Verdi
        /// </summary>
        VERDI = 1,
        /// <summary>
        /// Vermedi
        /// </summary>
        VERMEDI = 2,
        /// <summary>
        /// Hepsi
        /// </summary>
        HEPSI = 3,
    }

    /// <summary>
    /// Evet Hayır
    /// </summary>
    public enum ENUMEvetHayir
    {
        /// <summary>
        /// Hayır
        /// </summary>
        HAYIR = 0,
        /// <summary>
        /// Evet
        /// </summary>
        EVET = 1,
        /// <summary>
        /// Hepsi
        /// </summary>
        HEPSI = 2,
    }

    /// <summary>
    /// İhale Sonucu Türü
    /// </summary>
    public enum ENUMIhaleSonucu
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Onaylandı
        /// </summary>
        ONAYLANDI = 1,
        /// <summary>
        /// İptal Edildi
        /// </summary>
        IPTALEDILDI = 2,
        /// <summary>
        /// Tüm Teklifler Red
        /// </summary>
        TUMTEKIFLERRED = 3,
    }

    /// <summary>
    /// Uygun Mu Türü
    /// </summary>
    public enum ENUMUygunMu
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Uygun
        /// </summary>
        UYGUN = 1,
        /// <summary>
        /// Uygun Değil
        /// </summary>
        UYGUNDEGIL = 2
    }

    /// <summary>
    /// Fatura Detay Türü
    /// </summary>
    public enum ENUMFaturaDetayTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Hesap Cetveli
        /// </summary>
        HESAPCETVELI = 1,
        /// <summary>
        /// Harcama Pusulası
        /// </summary>
        HARCAMAPUSULASI = 2,
    }

    /// <summary>
    /// Personel Türü
    /// </summary>
    public enum ENUMPersonelTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Sözleşmeli
        /// </summary>
        SOZLESMELI = 1,
        /// <summary>
        /// İşçi
        /// </summary>
        ISCI = 2,
    }

    /// <summary>
    /// Gelen Giden Personel Türü
    /// </summary>
    public enum ENUMPersonelGelenGidenTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Gelen
        /// </summary>
        GELEN = 1,
        /// <summary>
        /// Giden
        /// </summary>
        GIDEN = 2,
    }

    /// <summary>
    /// Olay Niteliği Türü
    /// </summary>
    public enum ENUMOlayNiteligi
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Aile Yardımı
        /// </summary>
        AILEYARDIMI = 1,
        /// <summary>
        /// Doğum
        /// </summary>
        DOGUM = 2,
        /// <summary>
        /// Ölüm
        /// </summary>
        OLUM = 3,
    }

    /// <summary>
    /// Yakınlık Derecesi Türü
    /// </summary>
    public enum ENUMYakinlikDerecesi
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Kendisi
        /// </summary>
        KENDISI = 1,
        /// <summary>
        /// Eşi
        /// </summary>
        ESI = 2,
        /// <summary>
        /// 6 yaşından küçük çocuk
        /// </summary>
        COCUK6DANKUCUK = 3,
        /// <summary>
        /// 6 yaşından büyük çocuk
        /// </summary>
        COCUK6DANBUYUK = 4,
    }

    /// <summary>
    /// Akrabalık Derecesi Türü
    /// </summary>
    public enum ENUMAkrabalikDerecesi
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Eş
        /// </summary>
        ES = 1,
        /// <summary>
        /// Çocuk
        /// </summary>
        COCUK = 2,
        /// <summary>
        /// Anne
        /// </summary>
        ANNE = 3,
        /// <summary>
        /// Baba
        /// </summary>
        BABA = 4,
        /// <summary>
        /// Kendisi
        /// </summary>
        KENDISI = 5,
        /// <summary>
        /// Kardeşi
        /// </summary>
        KARDESI = 6,

    }

    /// <summary>
    /// Belge Durum Türü
    /// </summary>
    public enum ENUMBelgeDurumu
    {
        /// <summary>
        /// Yeni
        /// </summary>
        YENI = 1,
        /// <summary>
        /// Değiştirildi
        /// </summary>
        DEGISTIRILDI = 2,
        /// <summary>
        /// Onaylı
        /// </summary>
        ONAYLI = 5,
        /// <summary>
        /// Onay Kaldır
        /// </summary>
        ONAYKALDIR = 6,
        /// <summary>
        /// İptal
        /// </summary>
        IPTAL = 9,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Yolluk Gündelik Türü
    /// </summary>
    public enum ENUMYollukGundelikTuru
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Sürekli Geçici
        /// </summary>
        SUREKLIGECICI = 1,
        /// <summary>
        /// Denetim Elemanı
        /// </summary>
        DENETIMELEMANI = 2,
        /// <summary>
        /// Geçici Yurtdışı
        /// </summary>
        GECICIYURTDISI = 3,
        /// <summary>
        /// Tedavi Yolluğu
        /// </summary>
        TEDAVIYOLLUGU = 4,
        /// <summary>
        /// Kovuşturma
        /// </summary>
        KOVUSTURMA = 5,
        /// <summary>
        /// Anketör Yolluğu
        /// </summary>
        ANKETOR = 6,

        GECICIYURTDISINDANGELIS = 20,
        GECICIYURTDISIMISAFIR = 21,
        GECICIYURTDISINDANGELISMISAFIR = 22,

    }

    /// <summary>
    /// Sürekli Görev Türü
    /// </summary>
    public enum ENUMSurekliGorevTuru
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yurtiçi
        /// </summary>
        YURTICI = 1,
        /// <summary>
        /// Yurtdışı
        /// </summary>
        YURTDISI = 2,
    }

    /// <summary>
    /// Çeşitli Ödeme Türü
    /// </summary>
    public enum ENUMCesitliOdemeTur
    {
        /// <summary>
        /// Ödül İkramiye
        /// </summary>
        ODULIKRAMIYE = 1,
        /// <summary>
        /// Kovuşturma
        /// </summary>
        KOVUSTURMA = 2,
        /// <summary>
        /// Konferans
        /// </summary>
        KONFERANS = 3,
        /// <summary>
        /// Giyim
        /// </summary>
        GIYIM = 4,
        /// <summary>
        /// Vekalet
        /// </summary>
        VEKALET = 5,
        /// <summary>
        /// Vekalet
        /// </summary>
        UZMAN = 6,
        /// <summary>
        /// Diğer
        /// </summary>
        DIGER = 9,
        /// <summary>
        /// Katılımcı
        /// </summary>
        KATILIMCI = 10
    }

    /// <summary>
    /// İşçi Maaş Bordro Türü
    /// </summary>
    public enum ENUMIsciMaasBordroTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Maaş
        /// </summary>
        MAAS = 1,
        /// <summary>
        /// Sendikal İkramiye
        /// </summary>
        SENDIKALIKRAMIYE = 2,
        /// <summary>
        /// İlave Tediye
        /// </summary>
        ILAVETEDIYE = 3,
    }

    /// <summary>
    /// Yolculuk Araç Türü
    /// </summary>
    public enum ENUMYolculukAracTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Uçak
        /// </summary>
        UCAK = 1,
        /// <summary>
        /// Otobüs
        /// </summary>
        OTOBUS = 2,
        /// <summary>
        /// Tren
        /// </summary>
        TREN = 3,
        /// <summary>
        /// Taksi
        /// </summary>
        TAKSI = 4,
        /// <summary>
        /// Toplu Taşıma
        /// </summary>
        TOPLUTASIMA = 5,
        /// <summary>
        /// Minibüs
        /// </summary>
        MINIBUS = 6,
        /// <summary>
        /// Dolmuş
        /// </summary>
        DOLMUS = 7,
        /// <summary>
        /// Havaş
        /// </summary>
        HAVAS = 8,
        /// <summary>
        /// Havaş + Taksi
        /// </summary>
        HAVASTAKSI = 9,
        /// <summary>
        /// Diğer
        /// </summary>
        DIGER = 10,
    }

    /// <summary>
    /// Geçici Görev Yolluk Yön Türü
    /// </summary>
    public enum ENUMGeciciGorevYolculukYonu
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,

        /// <summary>
        /// Gidiş
        /// </summary>
        GIDIS = 1,

        /// <summary>
        /// Dönüş
        /// </summary>
        DONUS = 2,

        /// <summary>
        /// Konaklama
        /// </summary>
        GOREV = 3,
    }

    /// <summary>
    /// Rapor Türü
    /// </summary>
    public enum ENUMRaporTuru
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Mal Alımı Harcama Takip
        /// </summary>
        MALALIMIHARACAMATAKIP = 1,
        /// <summary>
        /// Hizmet Alımı Harcama Takip
        /// </summary>
        HIZMETALIMIHARACAMATAKIP = 2,
        /// <summary>
        /// Yapım İşi Harcama Takip
        /// </summary>
        YAPIMISIHARACAMATAKIP = 3,
        /// <summary>
        /// Mal Hizme Yapım İhale Türü Toplam
        /// </summary>
        MHYIHALETURUTOPLAM = 4,
        /// <summary>
        /// Mal Hizme Yapım Toplamları
        /// </summary>
        MHYILTOPLAMLARI = 5,
        /// <summary>
        /// Abonelik İşlemleri Harcama Takip
        /// </summary>
        ABONELIKISLEMLERIHARCAMATAKIP = 6,
        /// <summary>
        /// Ürün Mukayese
        /// </summary>
        URUNMUKAYESE = 7,
        /// <summary>
        /// Sürekli Geçici Yolluk Özet
        /// </summary>
        SGYOLLUGUOZET = 8,
        /// <summary>
        /// Merkez Birimleri Dağılım Tablosu
        /// </summary>
        MERKEZBIRIMLERIDAGILIMTABLOSU = 9,
        /// <summary>
        /// Abonelik İstatistik
        /// </summary>
        ABONELIKISTATISTIK = 50,
        /// <summary>
        /// Personel İstatistik
        /// </summary>
        PERSONELISTATISTIK = 51,
    }

    /// <summary>
    /// Rapor Şekli Türü
    /// </summary>
    public enum ENUMRaporSekli
    {
        /// <summary>
        /// Özet
        /// </summary>
        OZET = 0,
        /// <summary>
        /// Detay
        /// </summary>
        DETAY = 1,
        /// <summary>
        /// Sıralama
        /// </summary>
        SIRALAMA = 2,
    }

    /// <summary>
    /// Rapor Gruop By Türü
    /// </summary>
    public enum ENUMRaporGrupBy
    {
        /// <summary>
        /// Süreç
        /// </summary>
        SUREC = 0,
        /// <summary>
        /// Birim
        /// </summary>
        BIRIM = 1,
        /// <summary>
        /// Muhasebe
        /// </summary>
        MUHASEBE = 2,
        /// <summary>
        /// İl
        /// </summary>
        IL = 3,
    }

    /// <summary>
    /// Tedavi Türü
    /// </summary>
    public enum ENUMTedaviTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Tedavi
        /// </summary>
        TEDAVI = 1,
        /// <summary>
        /// İlaç
        /// </summary>
        ILAC = 2,
        /// <summary>
        /// Cenaze
        /// </summary>
        CENAZE = 3,
    }

    /// <summary>
    /// İhale Belge Türü
    /// </summary>
    public enum ENUMIhaleBelgeTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Sözleşme
        /// </summary>
        SOZLESME = 1,
        /// <summary>
        /// Şartname
        /// </summary>
        SARTNAME = 2,
    }

    /// <summary>
    /// Sözleşme Türü
    /// </summary>
    public enum ENUMSozlesmeTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Hizmet
        /// </summary>
        HIZMETTIP = 1,
        /// <summary>
        /// Mal
        /// </summary>
        MALTIP = 2,
        /// <summary>
        /// Yapım Birim Fiyat
        /// </summary>
        YAPIBIRIMFIYAT = 3,
        /// <summary>
        /// Yapım Anahtar Teslim
        /// </summary>
        YAPIANAHTARTESLIM = 4,
    }

    /// <summary>
    /// Şartname Türü
    /// </summary>
    public enum ENUMSartnameTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Mal Alım Açık İhale
        /// </summary>
        MAL_ACIKIHALE = 1,
        /// <summary>
        /// Mal Alım Ön Yeterlik
        /// </summary>
        MAL_ONYETERLIK = 2,
        /// <summary>
        /// Mal Alım İdari
        /// </summary>
        MAL_IDARI = 3,
        /// <summary>
        /// Mal Alım Pazarlık
        /// </summary>
        MAL_PAZARLIK = 4,
        /// <summary>
        /// Hizmet Alımı Açık İhale
        /// </summary>
        HIZMET_ACIKIHALE = 5,
        /// <summary>
        /// Hizmet Alımı Ön Yeterlik
        /// </summary>
        HIZMET_ONYETERLIK = 6,
        /// <summary>
        /// Hizmet Alımı İdari
        /// </summary>
        HIZMET_IDARI = 7,
        /// <summary>
        /// Hizmet Alımı Pazarlık
        /// </summary>
        HIZMET_PAZARLIK = 8,
        /// <summary>
        /// Yapım İşi Açık İhale
        /// </summary>
        YAPIM_ACIKIHALE = 9,
        /// <summary>
        /// Yapım İşi Ön Yeterlik
        /// </summary>
        YAPIM_ONYETERLIK = 10,
        /// <summary>
        /// Yapım İşi İdari
        /// </summary>
        YAPIM_IDARI = 11,
        /// <summary>
        /// Yapım İşi Pazarlık
        /// </summary>
        YAPIM_PAZARLIK = 12,
        /// <summary>
        /// Danışmanlık İdari
        /// </summary>
        DANISMANLIK_IDARI = 13,
        /// <summary>
        /// Danışmanlık Ön Yeterlik
        /// </summary>
        DANISMANLIK_ONYETERLIK = 14,
    }

    /// <summary>
    /// HTML Kontrol Türü
    /// </summary>
    public enum ENUMHTMLKontrol
    {
        /// <summary>
        /// Textbox
        /// </summary>
        TEXTBOX = 1,
        /// <summary>
        /// Radio Button
        /// </summary>
        RADIOBUTON = 2,
        /// <summary>
        /// Checkbox
        /// </summary>
        CHECKBOX = 3,
        /// <summary>
        /// Div
        /// </summary>
        DIV = 4,
        /// <summary>
        /// Farpoint
        /// </summary>
        FARPOINT = 5,
        /// <summary>
        /// Dropdownlist
        /// </summary>
        DROPDOWNLIST = 6,
    }

    /// <summary>
    /// Abone Rapor Türü
    /// </summary>
    public enum ENUMAboneRaporTipi
    {
        /// <summary>
        /// Fatura Bazında
        /// </summary>
        FATURABAZINDA = 1,
        /// <summary>
        /// Muhasebe Birim Bazında
        /// </summary>
        MUHASEBEBIRIMBAZINDA = 2,
        /// <summary>
        /// Birim Bazında
        /// </summary>
        BIRIMBAZINDA = 3,
        /// <summary>
        /// İlçe Bazında
        /// </summary>
        ILCEBAZINDA = 4,
        /// <summary>
        /// İl Bazında
        /// </summary>
        ILBAZINDA = 5,
        /// <summary>
        /// Kurumsal Kod Bazında
        /// </summary>
        KURUMSALKODBAZINDA = 6
    }

    /// <summary>
    /// Borcu Yok Durum Türü
    /// </summary>
    public enum ENUMBorcuYokDurum
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Gönderildi
        /// </summary>
        GONDERILDI = 1,
        /// <summary>
        /// Sırada
        /// </summary>
        SIRADA = 2,
        /// <summary>
        /// Cevaplanmadı
        /// </summary>
        CEVAPLANMADI = 3,
        /// <summary>
        /// Cevap Olumlu
        /// </summary>
        CEVAPOLUMLU = 4,
        /// <summary>
        /// Cevap Olumsuz
        /// </summary>
        CEVAPOLUMSUZ = 5,
        /// <summary>
        /// Dilekçe Sorgu Hatası
        /// </summary>
        DILEKCESORGUHATASI = 6,
    }

    /// <summary>
    /// Borcu Yok Talep Nedeni Türü
    /// </summary>
    public enum ENUMBorcuYokTalepNedeni
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// İhale
        /// </summary>
        IHALE = 1,
        /// <summary>
        /// Ödeme Almak
        /// </summary>
        ODEMEALMAK = 2,
        /// <summary>
        /// Diğer
        /// </summary>
        DIGER = 3,
    }

    /// <summary>
    /// Kur Bilgileri Türü
    /// </summary>
    public enum ENUMKurBilgileriTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yurtdışı Yolluk
        /// </summary>
        YURTDISIYOLLUK = 1,
        /// <summary>
        /// Avans Hesap
        /// </summary>
        AVANSHESAP = 2,
    }

    /// <summary>
    /// Ön Yeterlik Davet Sayisi Türü
    /// </summary>
    public enum ENUMOnYeterlikDavetSayisi
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yeterli Bulunanlar
        /// </summary>
        YETERLIBULUNANLAR = 1,
        /// <summary>
        /// Şartnamede Belirtilen Sayıda
        /// </summary>
        SARTNAMEDEBELIRTILENSAYIDA = 2,
    }

    /// <summary>
    /// Zarf İçerik Değerlendirme Kriter Türü
    /// </summary>
    public enum ENUMZarfIcerikDegerlendirmeKriteri
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Var - Yok
        /// </summary>
        VAR_YOK = 1,
        /// <summary>
        /// Uygun - Uygun Değil
        /// </summary>
        UYGUN_UYGUNDEGIL = 2,
    }

    /// <summary>
    /// Kiralamma Türü
    /// </summary>
    public enum ENUMKiralamaTur
    {
        /// <summary>
        /// Araç
        /// </summary>
        ARAC = 1,
        /// <summary>
        /// Taşınır
        /// </summary>
        TASINIR = 2,
        /// <summary>
        /// Taşınmaz
        /// </summary>
        TASINMAZ = 3
    }

    /// <summary>
    /// Bordro Türü
    /// </summary>
    public enum ENUMBordroTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Aylık
        /// </summary>
        AYLIK = 1,
        /// <summary>
        /// Fark
        /// </summary>
        FARK = 2,
    }

    /// <summary>
    /// Bordro Türü
    /// </summary>
    public enum ENUMBordroTip
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// 4B SÖZLEŞMELİ
        /// </summary>
        SOZLESMELI4B = 1,
        /// <summary>
        /// 4C SÖZLEŞMELİ
        /// </summary>
        SOZLESMELI4C = 2,
        /// <summary>
        /// ÖĞRENCİ
        /// </summary>
        OGRENCI = 3,
    }

    /// <summary>
    /// Aktif Pasif Türü
    /// </summary>
    public enum ENUMAktifPasif
    {
        /// <summary>
        /// Pasif
        /// </summary>
        PASIF = 0,
        /// <summary>
        /// Aktif
        /// </summary>
        AKTIF = 1,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 2,
    }

    /// <summary>
    /// Damga Vergisi Türü
    /// </summary>
    public enum ENUMDamgaVergisiTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Kişi
        /// </summary>
        KISI = 1,

        /// <summary>
        /// Yolluklar
        /// </summary>
        YOLLUK = 2,

        /// <summary>
        /// Diğer
        /// </summary>
        DIGER = 9,
    }

    /// <summary>
    /// Yurt içi geçici görev yolluk türü
    /// </summary>
    public enum ENUMYIGGYTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// İl - İlçe
        /// </summary>
        IL_ILCE = 1,
        /// <summary>
        /// İl Dışı
        /// </summary>
        ILDISI = 2,
        /// <summary>
        /// Günübirlik
        /// </summary>
        GUNUBIRLIK = 3,
        /// <summary>
        /// Eğitim
        /// </summary>
        EGITIM = 4,
        /// <summary>
        /// Hepsi
        /// </summary>
        HEPSI = 10,
    }

    /// <summary>
    /// Yurtdışı geçici görev yolluk türü
    /// </summary>
    public enum ENUMYDGGYTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Avrupa Birliği
        /// </summary>
        AB = 1,
        /// <summary>
        /// Amerika Birleşik Devletleri
        /// </summary>
        ABD = 2,
        /// <summary>
        /// Diğer Ülkeler
        /// </summary>
        DIGERULKELER = 3,
        /// <summary>
        /// Avrupa Birliği Eğitim
        /// </summary>
        ABEGITIM = 4,
        /// <summary>
        /// Amerika Birleşik Devletleri Eğitim
        /// </summary>
        ABDEGITIM = 5,
        /// <summary>
        /// Hepsi
        /// </summary>
        HEPSI = 10,
    }

    /// <summary>
    /// Yurtiçi sürekli görev yolluk türü
    /// </summary>
    public enum ENUMYISGYTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yer Değiştirme
        /// </summary>
        YERDEGISTIRME = 1,
        /// <summary>
        /// Emekli
        /// </summary>
        EMEKLI = 2,
        /// <summary>
        /// Hepsi
        /// </summary>
        HEPSI = 10,
    }

    /// <summary>
    /// Yurtdışı sürekli görev yolluk türü
    /// </summary>
    public enum ENUMYDSGYTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Yer Değiştirme
        /// </summary>
        YERDEGISTIRME = 1,
        /// <summary>
        /// Hepsi
        /// </summary>
        HEPSI = 10,
    }

    /// <summary>
    /// Seyyar Görev Aşamaları
    /// </summary>
    public enum ENUMSeyyarGorevAsamalari
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Talep
        /// </summary>
        TALEP = 1,
        /// <summary>
        /// Kurum Teklid
        /// </summary>
        KURUMTEKLIF = 2,
        /// <summary>
        /// Vize Edilen
        /// </summary>
        VIZEEDILEN = 3,
    }

    /// <summary>
    /// Süreç Durum Türü
    /// </summary>
    public enum ENUMSatinalmaTalepFormDurum
    {
        TANIMSIZ = 0,
        YENI = 1,
        TALEPGONDERILDI = 2,
        MUHASEBEONAYLADI = 3,
        SATINALMAONAYLADI = 4,
        YAKLASIKMALIYETBEKLIYOR = 5,
        YAKLASIKMALIYETBELIRLENIYOR = 6,
        YAKLASIKMALIYETBELIRLENDI = 7,
        SURECBASLADI = 8,
        IPTAL = 9,
        DOGRUDANTEMINTALEBIYAPILDI = 99,
        ARSIV = 10,
    }

    public enum ENUMYetkiTipi
    {
        YAPABILIR = 1,
        GOREBILIR = 2,
        ONAYLAYABILIR = 3
    }

    public enum ENUMKullaniciYetkiTipi
    {
        TALEP = 1,
        SURECBASLAT = 2,
        ONAYBELGESI = 3,
        IHALEISLEMLERI = 4,
        FATURA = 5,
        TASINIRISLEMFISI = 6,
        ONMALIKONTROL = 7,
        ODEMEEMRIBELGESI = 8,
        BIRIMODEMECI = 9
    }

    public enum ENUMGeciciGorevYollukTalepDurum
    {
        TANIMSIZ = 0,
        YENI = 1,
        TALEPGONDERILDI = 2,
        MUHASEBEYEILETILDI = 3,
        //SATINALMAONAYLADI = 4,
        //YAKLASIKMALIYETBEKLIYOR = 5,
        //YAKLASIKMALIYETBELIRLENIYOR = 6,
        //YAKLASIKMALIYETBELIRLENDI = 7,
        SURECBASLADI = 8,
        IPTAL = 9,

    }

    public enum ENUMOnMaliKontrolTur
    {
        SOZLESMEONCESI = 1,
        ODEMEONCESI = 2
    }

    public enum ENUMManasSatinAlmaIslemTur
    {
        YAKLASIKMALIYET = 1,
        PIYASAFIYAT = 2
    }

    public enum ENUMKontrolDurum
    {
        KontrolEtme = 0,
        HataVer = 1,
        UyariVer = 2
    }

    public enum ENUMTeminatMektubuIslemTur
    {
        GIRIS = 1,
        CIKIS = 2,
    }

    public enum ENUMManasSatinAlmaIhaleUsulu
    {
        TANIMSIZ = 0,
        ACIKIHALEUSULU = 1,
        BELLIISTEKLIARASINDA = 2,
        DOGRUDANTEMIN = 3,
        PAZARLIKUSULU = 4
    }

    public enum ENUMManasSatinAlmaAlimTuru
    {
        TANIMSIZ = 0,
        DANISMANLIK = 1,
        HIZMET = 2,
        MAL = 3,
        MALVEHIZMET = 4,
        YAPIM = 5
    }

    public enum ENUMManasSatinAlmaParaBirimi
    {
        TANIMSIZ = 0,
        DOLAR = 1,
        SOM = 2,
        TURKLIRASI = 3
    }

    public enum ENUMManasSatinAlmaAlimYontemi
    {
        TANIMSIZ = 0,
        CIFTASAMA = 1,
        TEKASAMA = 2
    }

    public enum ENUMGundelikTur
    {
        TANIMSIZ = 0,
        GIDIS = 1,
        GELIS = 2,
        YURTICI = 3,
    }
}
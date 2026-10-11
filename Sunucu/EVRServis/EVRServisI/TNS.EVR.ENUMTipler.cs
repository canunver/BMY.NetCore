using System;

namespace TNS.EVR
{
    /// <summary>
    /// Evrak Türü
    /// </summary>
    public enum ENUMEvrakTipi
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Gelen Evrak
        /// </summary>
        GELEN = 1,
        /// <summary>
        /// Giden Evrak
        /// </summary>
        GIDEN = 2,
        FORMLAR = 3
    }

    /// <summary>
    /// Giden Evrak Türü
    /// </summary>
    public enum ENUMGidenEvrakTuru
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Dış Yazışma
        /// </summary>
        DISYAZISMA = 1,
        /// <summary>
        /// Kurum içi Yazışma
        /// </summary>
        KURUMICYAZISMA = 2,
        /// <summary>
        /// Dış Genelge
        /// </summary>
        DISGENELGE = 3,
        /// <summary>
        /// İç Genelge
        /// </summary>
        ICGENELGE = 4,
        /// <summary>
        /// Başkanlık Makam Onayı
        /// </summary>
        BASKANLIKMAKAMONAYI = 5,
        /// <summary>
        /// Bakanlık Makam Onayı
        /// </summary>
        BAKANLIKMAKAMONAYI = 6,
        /// <summary>
        /// Birim içi Yazışma
        /// </summary>
        BIRIMICYAZISMA = 7,
        /// <summary>
        /// Görevlendirme Yazışma
        /// </summary>
        GOREVLENDIRME = 8,
        /// <summary>
        /// Hastalık Rapor Yazışma
        /// </summary>
        HASTALIKRAPOR = 9,
    }

    /// <summary>
    /// Geldiği Yer Türü
    /// </summary>
    public enum ENUMEvrakGeldigiYerTur
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Kurum İçi
        /// </summary>
        KURUMICI = 1,
        /// <summary>
        /// Kamu
        /// </summary>
        KAMU = 2,
        /// <summary>
        /// Tüzel
        /// </summary>
        TUZEL = 3,
        /// <summary>
        /// Özel
        /// </summary>
        OZEL = 4,

    }

    /// <summary>
    /// Genel Evrak Birimi Sorgu Türü
    /// </summary>
    public enum ENUMGenelEvrakBirimi
    {
        /// <summary>
        /// Evet
        /// </summary>
        EVET = 1,
        /// <summary>
        /// Hayır
        /// </summary>
        HAYIR = 2,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
    }

    /// <summary>
    /// Evrak Yetki Şablon Türü
    /// </summary>
    public enum ENUMEvrakYetkiSablonu
    {
        /// <summary>
        /// Evrak Yetkilisi
        /// </summary>
        EVRAKYETKILISI = 1,
        /// <summary>
        /// Evrak Kullanıcısı
        /// </summary>
        EVRAKKULLANICISI = 2,
        /// <summary>
        /// Evrak memuru
        /// </summary>
        EVRAKMEMURU = 3,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
    }

    /// <summary>
    /// Evrak Onay Yetki Şablon Türü
    /// </summary>
    public enum ENUMEvrakOnayYetkiSablonu
    {
        /// <summary>
        /// İmzalar
        /// </summary>
        IMZALAR = 1,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
    }

    /// <summary>
    /// Evrak Tarihçe Durum Türü
    /// </summary>
    public enum ENUMEvrakTarihceDurum
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
        /// Dosyadan Alındı
        /// </summary>
        DOSYADANALINDI = 3,
        /// <summary>
        /// Havale Geri Alındı
        /// </summary>
        HAVALEGERIALINDI = 4,
        /// <summary>
        /// İade Edildi
        /// </summary>
        IADEEDILDI = 5,
        /// <summary>
        /// İptal Geri Alındı
        /// </summary>
        IPTALGERIALINDI = 6,
        /// <summary>
        /// Numara Alındı. Giden için
        /// </summary>
        NUMARAALINDI = 7,
        /// <summary>
        /// Geri Gönderildi. Giden İçin
        /// </summary>
        GERIGONDERILDI = 11,

        /// <summary>
        /// Elektronik İmzalandı. Giden için
        /// </summary>
        EIMZALANDI = 14,

        /// <summary>
        /// İmza Paraf Kaldırıldı
        /// </summary>
        IMZAPARAFKALDIRILDI = 15,

        /// <summary>
        /// İptal
        /// </summary>
        IPTAL = 9,
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = -1,
        /// <summary>
        /// Havale Edildi
        /// </summary>
        HAVALEEDILDI = 21,
        /// <summary>
        /// Bütün Belgeler
        /// </summary>
        BUTUNBELGELER = 22,
        /// <summary>
        /// Havale Gelen
        /// </summary>
        HAVALEGELEN = 23,
        /// <summary>
        /// Dosyaya Kaldır
        /// </summary>
        DOSYAYAKALDIR = 24,

        GORUNTULENDI = 25,

        KOPYALANDI = 26,

        DEVAMEDENISLER = 27,

        DEVAMEDENISLERVEMIADLIISLER = 28,

        IMZAYAGELENLER = 29,
    }

    /// <summary>
    /// Evrak Hareket Durum Türü
    /// </summary>
    public enum ENUMEvrakHareketDurum
    {
        /// <summary>
        /// Yeni
        /// </summary>
        YENI = 1,
        /// <summary>
        /// Teslim Aldı
        /// </summary>
        TESLIMALDI = 2,
        /// <summary>
        /// Dosyadan Alındı
        /// </summary>
        DOSYADANALINDI = 3,
        /// <summary>
        /// Havale Geri Alındı
        /// </summary>
        HAVALEGERIALINDI = 4,
        /// <summary>
        /// İade Edildi
        /// </summary>
        IADEEDILDI = 5,
        /// <summary>
        /// İptal Geri Alındı
        /// </summary>
        IPTALGERIALINDI = 6,
        /// <summary>
        /// Numara Alındı. Giden için
        /// </summary>
        NUMARAALINDI = 7,
        /// <summary>
        /// İmzalandı. Giden için
        /// </summary>
        IMZALANDI = 8,
        /// <summary>
        /// Geri Gönderildi. Giden için
        /// </summary>
        GERIGONDERILDI = 11,
        /// <summary>
        /// İmzaya Gönder. Giden için
        /// </summary>
        IMZAYAGONDER = 12,
        /// <summary>
        /// Cevaplandı
        /// </summary>
        CEVAPLANDI = 13,

        /// <summary>
        /// Elektronik İmzalandı. Giden için
        /// </summary>
        EIMZALANDI = 14,

        /// <summary>
        /// Paraflandı. Giden için
        /// </summary>
        PARAFLANDI = 15,

        /// <summary>
        /// Giden Evrak Havale.
        /// </summary>
        HAVALEEDILDIGIDEN = 16,

        /// <summary>
        /// Dağıtım
        /// </summary>
        DAGITIMYAPILDI = 17,

        /// <summary>
        /// Elektronik Paraflandı. Giden için
        /// </summary>
        EPARAFLANDI = 18,

        /// <summary>
        /// İmza / paraf kaldırıldı
        /// </summary>
        IMZAPARAFKALDIRILDI = 19,

        /// <summary>
        /// Giden evrak havale geri alındı
        /// </summary>
        HAVALEGERIALINDIGIDEN = 20,

        /// <summary>
        /// İmzadan geri alındı
        /// </summary>
        IMZADANGERIALINDI = 30,

        /// <summary>
        /// Kep İle Gönderildi
        /// </summary>
        KEPILEGONDERILDI = 31,

        /// <summary>
        /// Havale Edildi
        /// </summary>
        HAVALEEDILDI = 51,

        /// <summary>
        /// Dağıtım Kayıt
        /// </summary>
        DAGITIMKAYIT = 52,

        /// <summary>
        /// Dosyaya Kaldır
        /// </summary>
        DOSYAYAKALDIR = 60,
        /// <summary>
        /// Arşive Kaldır
        /// </summary>
        ARSIVEKALDIR = 61,
        /// <summary>
        /// İptal Edildi
        /// </summary>
        IPTALEDILDI = 99,

        /// <summary>
        /// Kişi Üzerinde. Durum değil sorgulamak için
        /// </summary>
        KISIUZERINDE = 999,
    }

    /// <summary>
    /// Evrak Havale Türü
    /// </summary>
    public enum ENUMEvrakHavaleTur
    {
        /// <summary>
        /// Gereği
        /// </summary>
        GEREGI = 0,
        /// <summary>
        /// Bilgi
        /// </summary>
        BILGI = 1,
    }

    ////BARIS
    ///// <summary>
    ///// Yapılan Veritabanı İşleminin Türü.
    ///// </summary>
    //public enum ENUMKaydetmeIslemTur
    //{
    //    /// <summary>
    //    /// Kaydey
    //    /// </summary>
    //    KAYDET = 0,
    //    /// <summary>
    //    /// Güncelle
    //    /// </summary>
    //    GUNCELLE =1
    //}

    /// <summary>
    /// GSM Operatör Türü
    /// </summary>
    public enum ENUMGSMOperator
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Turkcell
        /// </summary>
        TURKCELL = 1,
        /// <summary>
        /// Avea
        /// </summary>
        AVEA = 2,
    }

    /// <summary>
    /// Haberci Alan Türü
    /// </summary>
    public enum ENUMHaberciAlan
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Geldiği Yer
        /// </summary>
        GELDIGIYER = 1,
        /// <summary>
        /// Konusu
        /// </summary>
        KONUSU = 2,
        /// <summary>
        /// Geliş Şekli
        /// </summary>
        GELISSEKLI = 3,
        /// <summary>
        /// Yazı Durumu
        /// </summary>
        YAZIDURUMU = 4,
        /// <summary>
        /// Gizlilik Derecesi
        /// </summary>
        GIZLILIKDERECESI = 5,
        /// <summary>
        /// Süreli
        /// </summary>
        SURELI = 6,
        /// <summary>
        ///Dosya Kodu 
        /// </summary>
        DOSYAKODU = 7
    }

    /// <summary>
    /// Haberci İşleç Türü
    /// </summary>
    public enum ENUMHaberciIslec
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Eşit
        /// </summary>
        ESIT = 1,
        /// <summary>
        /// Büyük
        /// </summary>
        BUYUK = 2,
        /// <summary>
        /// Küçük
        /// </summary>
        KUCUK = 3,
        /// <summary>
        /// Büyük Eşit
        /// </summary>
        BUYUKESIT = 4,
        /// <summary>
        /// Küçük Eşit
        /// </summary>
        KUCUKESIT = 5,
        /// <summary>
        /// İle Başlayan
        /// </summary>
        ILEBASLAYAN = 6,
        /// <summary>
        /// İçinde Geçen
        /// </summary>
        ICINDEGECEN = 7
    }

    /// <summary>
    /// Havale Kural Alan Türü
    /// </summary>
    public enum ENUMHavaleKuralAlan
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Geldiği Yer
        /// </summary>
        GELDIGIYER = 1,
        /// <summary>
        /// Konusu
        /// </summary>
        KONUSU = 2,
        /// <summary>
        /// Geliş Şekli
        /// </summary>
        GELISSEKLI = 3,
        /// <summary>
        /// Yazı Durumu
        /// </summary>
        YAZIDURUMU = 4,
        /// <summary>
        /// Gizlilik Derecesi
        /// </summary>
        GIZLILIKDERECESI = 5,
        /// <summary>
        /// Süreli
        /// </summary>
        SURELI = 6,
        /// <summary>
        /// Dosya Kodu
        /// </summary>
        DOSYAKODU = 7
    }

    /// <summary>
    /// Havale Kural İşleç Türü
    /// </summary>
    public enum ENUMHavaleKuralIslec
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Eşit
        /// </summary>
        ESIT = 1,
        /// <summary>
        /// Büyük
        /// </summary>
        BUYUK = 2,
        /// <summary>
        /// Küçük
        /// </summary>
        KUCUK = 3,
        /// <summary>
        /// Büyük Eşit
        /// </summary>
        BUYUKESIT = 4,
        /// <summary>
        /// Küçük Eşit
        /// </summary>
        KUCUKESIT = 5,
        /// <summary>
        /// İle Başlayan
        /// </summary>
        ILEBASLAYAN = 6,
        /// <summary>
        /// İçinde Geçen
        /// </summary>
        ICINDEGECEN = 7
    }

    /// <summary>
    /// Havale Kural Gereği Bilgi Türü
    /// </summary>
    public enum ENUMHavaleKuralGB
    {
        /// <summary>
        /// Gereği Bilgi için
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Gereği
        /// </summary>
        GEREGI = 1,
        /// <summary>
        /// Bilgi
        /// </summary>
        BILGI = 2,
    }

    /// <summary>
    /// Posta Türü
    /// </summary>
    public enum ENUMPOSTATIP
    {
        /// <summary>
        /// Havale
        /// </summary>
        HAVALE = 1,
        /// <summary>
        /// Haberci
        /// </summary>
        HABERCI = 2,
        /// <summary>
        /// İmza
        /// </summary>
        IMZA = 3,
        HAVALEGIDEN = 4,
    }

    /// <summary>
    /// Evrak Rapor Türü
    /// </summary>
    public enum ENUMEvrakRaporTuru
    {
        /// <summary>
        /// Birim
        /// </summary>
        BIRIM = 1,
        /// <summary>
        /// Konu
        /// </summary>
        KONU = 2,
        /// <summary>
        /// Geldiği Yer
        /// </summary>
        GELDIGIYER = 3,
        /// <summary>
        /// Geliş Şekli
        /// </summary>
        GELISSEKLI = 4,
        BEKLEYENEVRAK = 5,
        PERSONEL = 6,
        PERSONELTOPLAM = 7,
    }

    /// <summary>
    /// Evrak İlişki Türü
    /// </summary>
    public enum ENUMEvrakIliskiTip
    {
        /// <summary>
        /// İlgi
        /// </summary>
        ILGI = 0,
        /// <summary>
        /// Cevap
        /// </summary>
        CEVAP = 1,
        /// <summary>
        /// İlet
        /// </summary>
        ILET = 2,
    }

    /// <summary>
    /// Tanımlama Form Türü
    /// </summary>
    public enum ENUMTamimlamaFormTur
    {
        /// <summary>
        /// Evrak Giden Türü
        /// </summary>
        EVRGIDENTUR = 1,
        /// <summary>
        /// Evrak Geliş Şekli
        /// </summary>
        EVRGELISSEKLI = 2,
        /// <summary>
        /// Evrak Yazı Durumu
        /// </summary>
        EVRYAZIDURUMU = 3,
        /// <summary>
        /// Evrak Gizlilik
        /// </summary>
        EVRGIZLILIK = 4,
        /// <summary>
        /// Evrak Posta İşlem Türü
        /// </summary>
        EVRPOSTAISLEMTUR = 5,
        /// <summary>
        /// Evrak Cinsi İşlem Türü
        /// </summary>
        EVREVRAKCINSI = 6,
        /// <summary>
        /// Şablon içeriği için alan bilgisi
        /// </summary>
        EVRSABLONICERIKALAN = 7,
        /// <summary>
        /// Şablon içeriği için alan bilgisi
        /// </summary>
        EVRDOSYAPLANLARI = 8
    }

    /// <summary>
    /// Gizlilik Türü
    /// </summary>
    public enum ENUMGizlilik
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Çok Gizli
        /// </summary>
        COKGIZLI = 1,
        /// <summary>
        /// Gizli
        /// </summary>
        GIZLI = 2,
        /// <summary>
        /// Hizmete Özel
        /// </summary>
        HIZMETEOZEL = 3,
        /// <summary>
        /// Kişiye Özel
        /// </summary>
        KISIYEOZEL = 4,
        /// <summary>
        /// Tasnif Dışı
        /// </summary>
        TASNIFDISI = 5,
    }

    /// <summary>
    /// Yazı Durum Türü
    /// </summary>
    public enum ENUMYaziDurum
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// İvedi
        /// </summary>
        IVEDI = 1,
        /// <summary>
        /// Çok Acele
        /// </summary>
        COKACELE = 2,
        /// <summary>
        /// Dağıtımlı
        /// </summary>
        DAGITIMLI = 3,
        /// <summary>
        /// Dilekçe
        /// </summary>
        DILEKCE = 4,
        /// <summary>
        /// Cevaplı Günlü
        /// </summary>
        CEVAPLIGUNLU = 5,
        /// <summary>
        /// Takipli
        /// </summary>
        TAKIPLI = 6,
        /// <summary>
        /// Tekit
        /// </summary>
        TEKIT = 7,
    }

    /// <summary>
    /// Evrak Okundu Durum Türü
    /// </summary>
    public enum ENUMEvrakOkunduDurum
    {
        /// <summary>
        /// Okunmadı
        /// </summary>
        OKUNMADI = 0,
        /// <summary>
        /// Okundu
        /// </summary>
        OKUNDU = 1,
        /// <summary>
        /// Hepsi
        /// </summary>
        HEPSİ = 2,
    }


    public enum ENUMOlaySorguTip
    {
        DOSYAYAKALDIRILMISMI = 0,
    }

    public enum ENUMEvrakKayitIslem
    {
        GENEL = 0,
        DAGITIMDANKAYIT = 1,
        EYAZISMA = 2,
        KEPPTT = 3,
        EDILEKCE = 4
    }

    public enum ENUMEvrakBelgeTur
    {
        TANIMSIZ = 0,
        GOREVLENDIRME = 1,
        PERSONELIZIN = 2,
    }

    public enum ENUMImzaYoluTip
    {
        TANIMSIZ = 0,
        PARAF = 1,
        IMZA = 2,
        KOORDINASYON = 3,
    }

    public enum ENUMEvrakArsivTur
    {
        TANIMSIZ = 0,
        KURUMSAL = 1,
        KISISEL = 2
    }

    public enum ENUMArsivYetkiTip
    {
        TANIMSIZ = 0,
        KISI = 1,
        UNVAN = 2,
        BIRIM = 3
    }

    public enum ENUMOneriTip
    {
        TANIMSIZ = 0,
        UNVAN = 1
    }

    public enum ENUMMetaTip
    {
        METIN = 0,
        TARIH = 1,
        YIL = 2,
        NUMARA = 3,
        COMBOBOX = 4,
        CHECKBOX = 5,
        IL = 6
    }

    public enum ENUMDosyaPlaniTarihceIslem
    {
        TANIMSIZ = 0,
        YENIKAYIT = 1,
        KAYDET = 2,
        DEGISTIRILDI = 3,
        SIL = 4
    }

    public enum ENUMOkumaYazma
    {
        TANIMSIZ = 0,
        OKUMA = 1,
        YAZMA = 2,
    }

    public enum ENUMEvrakArsivTarihceDurum
    {
        YENI = 1,
        DEGISIKLIKMETA = 2,
        DEGISIKLIKDOSYA = 3,
        IPTAL = 99,
    }

    public enum ENUMEvrakOrtakAlanTarihceDurum
    {
        YENI = 1,
        GUNCELLEME = 2,
        KOPYALA = 3,
        TASI = 4,
        SIL = 5,
        ISIMDEGISTIR = 6,
        INDIRME = 7,
        ONIZLEME = 8,
        NOTEKLEME = 9,
        NOTGUNCELLE = 10,
        NOTSIL = 11,
        YETKILENDIRME = 12,
        SURUMYUKLE = 13
    }

    public enum ENUMEvrakAyarlarAnahtar
    {
        GELENEVRAKHAVALEPOSTASIAL = 1,
        ASPOSEEMAILAYARLARSERVERADRES = 2,
        ASPOSEEMAILAYARLARSERVERPORTNO = 3,
        ASPOSEEMAILAYARLARSERVERSSL = 4,
        ASPOSEEMAILAYARLARKULLANICIADRES = 5,
        ASPOSEEMAILAYARLARKULLANICISIFRE = 6,
        ASPOSEEMAILAYARLARSERVERTIP = 7,
        ASPOSEEMAILAYARLARSYETKILIKODU = 8,
        ASPOSEEMAILAYARLARTARIHBASLANGICI = 9,
        ASPOSEEMAILAYARLARTARIHBITISI = 10,
        ASPOSEEMAILAYARLARMESAJTURU = 11,
        ASPOSEEMAILAYARLARTNBSONUC = 12,
        EDITORKULLANICI = 13,
        EDITOREVRAK = 14,
        EDITOR2KULLANICI = 15,
        EDITOR2EVRAK = 16,
        TWAINKULLANICI = 17,
        PREVRAKNOBILGISINEGORESIRALA = 18,
    }

    public enum ENUMEvrakDosyaOCRIslem
    {
        YAPILACAK = 0,
        YAPILDI = 1,
        YAPILIYOR = 2,
        YAPILMAYACAK = 3,
        ISLEMIBITENLER = 99,
        DOSYAYOK = 10,
    }

    public enum ENUMEvrakDosyaKriptoIslem
    {
        YAPILACAK = 0,
        YAPILDI = 1,
    }

    public enum ENUMEvrakDosyaIcerikIslem
    {
        YAPILACAK = 0,
        YAPILDI = 1,
        YAPILMAYACAK = 3
    }

    public enum ENUMTanimlamaSureBaslangici
    {
        /// <summary>
        /// Belge Üretim Tarihi
        /// </summary>
        BELGEURETIMTARIHI = 1,
        /// <summary>
        ///  Klasör Açılış Tarihi
        /// </summary>
        KLASORACILISTARIHI = 2,
        /// <summary>
        /// Klasör Kapanış Tarihi
        /// </summary>
        KLASORKAPANISTARIHI = 3,
        /// <summary>
        ///Klasördeki ilk Dosya Tarihi
        /// </summary>
        KLASORDEKIILKDOSYATARIHI = 4,
        /// <summary>
        /// Klsördeki Son Dosya Tarihi
        /// </summary>
        KLASORDEKISONDOSYATARIHI = 5,

    }

    public enum ENUMTanimlamaTasfiyePlani
    {
        /// <summary>
        /// Sürekli saklama 
        /// </summary>
        SUREKLISAKLAMA = 1,
        /// <summary>
        ///  Değerlendirme
        /// </summary>
        DEGERLENDIRME = 2,
        /// <summary>
        /// İmha
        /// </summary>
        IMHA = 3,
        /// <summary>
        ///Transfer
        /// </summary>
        TRANSFER = 4,
        /// <summary>
        ///Verilmemiş
        /// </summary>
        VERILMEMIS = 5,
    }

    public enum ENUMDilTurleri
    {
        /// <summary>
        /// Türkçe
        /// </summary>
        TURKCE = 1,
        /// <summary>
        ///  İngilizce
        /// </summary>
        INGILIZCE = 2,
        /// <summary>
        /// Almanca
        /// </summary>
        ALMANCA = 3,
        /// <summary>
        ///Fransızca
        /// </summary>
        FRANSIZCA = 4,
        RUSCA = 5,
        KIRGIZCA = 6,
    }

    [Serializable]
    public enum ENUMParafImzaIslemTip
    {
        PARAFLA = 0,
        PARAFLAVEPARAFAGONDER = 1,
        PARAFLAVEIMZAYAGONDER = 2,
        PARAFLAVEEIMZAYAGONDER = 3,
        IMZALA = 4,
        IMZALAVEIMZAYAGONDER = 5,
        EIMZALA = 6,
        EIMZALAVEEIMZAYAGONDER = 7,
    }

    [Serializable]
    public enum ENUMParafImzaGonderTip
    {
        PARAF = 1,
        IMZA = 2,
        EIMZA = 3
    }

    public enum ENUMAramaKriteriTipi
    {
        /// <summary>
        ///Evrak Sorgula
        /// </summary>
        EVRAKSORGULA = 1,
        /// <summary>
        /// Ornekle sorgula
        /// </summary>
        ORNEKLESORGULA = 2,

    }

    public enum ENUMEvrakFaksOncelikTipi
    {
        /// <summary>
        ///Normal
        /// </summary>
        NORMAL = 7,
        /// <summary>
        /// Acil
        /// </summary>
        ACIL = 4,
        /// <summary>
        /// Kritik
        /// </summary>
        KRITIK = 1,

    }

    public enum ENUMEvrakDagitimHareketIslem
    {
        ISLEMEALINDI = 1,
        GONDERILDI = 2,
        GONDERILMEDI = 3,
        HATAOLUSTU = 4,
    }

    public enum ENUMEvrakFaksDurumu
    {
        /// <summary>
        ///Gönderilecek
        /// </summary>
        GONDERILECEK = 1,
        /// <summary>
        /// Gönderiliyor
        /// </summary>
        GONDERILIYOR = 2,
        /// <summary>
        /// Gönderildi
        /// </summary>
        GONDERILDI = 3,
        /// <summary>
        /// Gönderilemedi
        /// </summary>
        GONDERILEMEDI = 4,
        /// <summary>
        /// iptal
        /// </summary>
        IPTAL = 5,
        /// <summary>
        /// Tekrar Gönder
        /// </summary>
        TEKRARGONDER = 6,
    }

    public enum ENUMEYazismaServisTur
    {
        TANIMSIZ = 0,
        DISISLERI = 1,
        DISISLERITEST = 2,
    }

    public enum ENUMDosyaPlaniPeriyod
    {
        /// <summary>
        /// YILLIK
        /// </summary>
        YILLIK = 1,
        /// <summary>
        ///  AYLIK
        /// </summary>
        AYLIK = 2,
    }

    public enum ENUMTanimlamaGizlilik
    {
        /// <summary>
        /// Gizli Değil
        /// </summary>
        GIZLIDEGIL = 0,
        /// <summary>
        ///  Gizli
        /// </summary>
        GIZLI = 1,
        /// <summary>
        ///  Çok Gizli
        /// </summary>
        COKGIZLI = 2,
    }

    public enum ENUMEvrakSaklamaIslem
    {
        TANIMSIZ = 0,
        SUREHESAPLANDI = 1,
        RAPORALINDI = 2,
        TASFIYEEDILDI = 3,
        IMHAEDILDI = 4,
        TRANSFEREDILDI = 5,
    }
    public enum ENUMOrtakDosyalarDosyaTipleri
    {
        TANIMSIZ = 0,
        KLASOR = 1,
        DOSYA = 2,
    }

    public enum ENUMOrtakDosyalarGosterilecekDosyalar
    {
        TANIMSIZ = 0,
        KENDIM = 1,
        PAYLASILAN = 2,
    }

    public enum ENUMOrtakDosyalarYetkiTur
    {
        TANIMSIZ = 0,
        SAHIBI = 1,
        DEVIR = 2,
        PAYLASILAN = 3
    }

    public enum ENUMOrtakDosyalarIzinler
    {
        TANIMSIZ = 0,
        EPOSTAGONDER = 1,
        INDIR = 2,
        EKLE = 3,
        ADDEGISTIR = 4,
        KOPYALA = 5,
        TASIMA = 6,
        SILME = 7,
        SURUMYUKLE = 8,
        YETKILENDIRME = 9,
        GORME = 99,

    }

    public enum ENUMSiraNoDegistirTablo
    {
        EVREVRAKDOSYA = 0,
        INFOCOMDOSYAIMZA = 1,
    }

    public enum ENUMWebConfigEvrakAyarlarIstemci
    {
        EvrakImzaYoluBirOncekineGeriGonder = 1,             //value="1"
        EvrakBilgiIcinGelenEvrakHavaleEdilsin = 2,          //value="1"
        EvrakDosyayaKaldirDagitimIsaretlenmesin = 3,        //value="1"
        EvrakImzaYoluMuSurecMiSecilebilsin = 4,             //value="1"
        EvrakAyniGidenEvrakKontrolu = 5,                    //value="1"
        EvrakAramaIslemiBulunduguBirimeGoreYapilsin = 6,    //value="1"
        EvrakEImzaZorunlu = 7,                              //value="1"
        EvrakImzalaveUstAmireGonder = 8,                    //value="1"
        EvrakDosyaPlaniEnAltKrilimSecmekZorunlu = 9,        //value="1"
        EvrakRaporSablonZimmetFis = 10,                     //value="2"
        EvrakIsTalepKaydet = 11,                            //value="1"
        EvrakKepImzaAdresi = 12,                            //value="http://uygulama.stratek.com.tr"
        EvrakSayiAlmaYontemi = 13,                          //value="R.30.{YIL}/{KISAKOD}-{EVRAKNO}"
        EvrakDurumSorgulaKokYetki = 14,                     //value="1"
    }

    public enum ENUMWebConfigEvrakAyarlarSunucu
    {
        EvrakNoSayacSifirlanmaYil = 1,                      //value="1"
        EvrakKullanicisiEvrakNoAlmasin = 2,                 //value="B.12.0.TRG;"
        EvrakHavaleIcinPersonelIzinKontrolDurumu = 3,       //value="1"
        EvrakImzaYolundaDegistirilebilsin = 4,              //value="1"
        EvrakImzaliEvrakDegisiklikYapilsin = 5,             //value="B.12.0.GKG;"
        EvrakBilgiIcinGelenEvrakGeriGonderilebilsin = 6,    //value="B.12.0.GKG;"
        EvrakIsAkisiHatalariniGoster = 7,                   //value="1"
        EvrakLoginCentraCubeWorkflowName = 8,               //value="Login"
        EvrakHataStrYaz = 9,                                //value="1"
        EvrakGizliEvraklarGorunmesin = 10,                  //value="1"
        EvrakKisiyeDagitimYapilabilsin = 11,                //value="1"
        EvrakProjeServisUrl = 12,                           //value="http://proje.stratek.com.tr"
        EvrakDagitimYapilabilirBirimlerYetkisi = 13,        //value="1"
        EvrakHavaleYapilabilirBirimlerYetkisi = 14,         //value="1"
        EvrakYetkiKontroluBirimeGoreYapilsin = 15,          //value="1"
        EvrakSayilariParolasizSorgulansin = 16,             //value="1"
        EvrakEImzaEYPPaketiKullan = 17,                     //value="1"
        EvrakKendiBirimineDagitimYapabilsin = 18            //value="B.12.0.GKG;"
    }

    public enum ENUMHavaleGrupPaylasimTip
    {
        TANIMSIZ = 0,
        BIRIM = 1,
        KISI = 2,
    }

    public enum ENUMEvrakSinifIslem
    {
        GENEL = 0,
        EVRAKTASIMA = 1,
        KEPGONDER = 2,
    }

    public enum ENUMYetkiler
    {
        GELENEVRAK = 1,
        GIDENEVRAK = 2,
        EVRAKISLEMLERI = 3,
        ARAMA = 4,
        DURUMSORGULAMA = 5,
        TASIMA = 6,
        POSTAISLEMLERI = 7,
        EPOSTAISLEMLERI = 8,
        FAKSISLEMLERI = 9,
        HAVALEGRUP = 10,
        HAVALEKURAL = 11,
        DAGITIM = 12,
        GELENKONUTANIMLAMA = 13,
        GELENYERTANIMLAMA = 14,
        ANTET = 15,
        BIRIM = 16,
        DOSYATASNIFPLANI = 17,
        DOSYATASNIFPLANIGRUP = 18,
        DOSYATASNIFPLANIBIRIM = 19,
        GIZLILIK = 20,
        YAZIDURUMU = 21,
        GELISSEKLI = 22,
        GIDENTUR = 23,
        POSTATUR = 24,
        EVRAKCINSI = 25,
        IMZAYOLU = 26,
        AVSIVKLASORLERI = 27,
        METAGRUP = 28,
        META = 29,
        VEKALET = 30,
        DINAMIKFORM = 31,
        ETIKETKISISEL = 32,
        ETIKETKURUMSAL = 33,

        RAPORLAR = 300,
    }

    public enum ENUMYetkiIslem
    {
        OKUMA = 1,
        YAZMA = 2,
        SILME = 4
    }

    public enum ENUMKEPEvrakIslem
    {
        TANIMSIZ = 0,
        KAYIT = 1,
        GONDER = 2,
        GONDERHATA = 3,
        GONDERIPTAL = 4,
        IPTAL = 99
    }


    public enum ENUMVeritabaniIndexIslem
    {
        TANIMSIZ = 0,
        REBUILD = 1,
        REORGANIZE = 2,
        DROP = 3,
        CREATE = 4,
    }

    public enum ENUMEvrakSinifParemetre
    {
        ISTALEPKAYDET = 1,
    }

    public enum ENUMEvrakIsTalepIslem
    {
        TANIMSIZ = 0,
        YENIKAYIT = 1,
        ATANDI = 2,
        HATAOLUSTU = 3,
        SERVISEERISIMYOK = 4,

        TAMAMLANDI = 10,
        IPTAL = 99
    }

    public enum ENUMKepAdresTip
    {
        TANIMSIZ = 0,
        KAMU = 1,
        TUZEL = 2,
        SAHIS = 3,
    }
}

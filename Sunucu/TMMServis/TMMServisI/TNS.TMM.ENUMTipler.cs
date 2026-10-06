namespace TNS.TMM
{
    public enum ENUMTuketimUretim : int
    {
        TUKETIM = 1,
        URETIM = 2
    }

    public enum ENUMTalepKarsilamaDurum : int
    {
        BEKLEMEDE = 1,
        SATINALMADA = 2,
        KAPATILDI = 3,
        IPTAL = 99
    }

    public enum ENUMTalepKarsilamaDepoDurum : int
    {
        YENI = 1,
        ISLEMYAPILIYOR = 2,
        TAMAMLANDI = 90,
        IPTAL = 99
    }

    public enum ENUMTesisBilesenDurum : int
    {
        KAYITEDILDI = 1,
        IPTAL = 9
    }

    public enum ENUMSayimRFIDDurum : int
    {
        TANIMSIZ = 0,
        DEVAMEDIYOR = 1,
        BITTI = 2
    }

    /// <summary>
    /// RFID etiketli taşınır malzemelerin sayılırken ki durumlarını ifade eden liste
    /// </summary>
    public enum ENUMSayimRFIDDemirbasDurum : int
    {
        /// <summary>
        /// RFID etiketi ile sayım yapılırken bulunamayan taşınır malzemeleri ifade eden liste değeri
        /// </summary>
        SAYIMDABULUNAMAYAN = 0,

        /// <summary>
        /// RFID etiketi ile sayım yapılırken bulunan taşınır malzemeleri ifade eden liste değeri
        /// </summary>
        SAYIMDABULUNAN = 1,

        /// <summary>
        /// RFID etiketi ile sayım yapılırken fazla çıkan taşınır malzemeleri ifade eden liste değeri
        /// </summary>
        SAYIMFAZLASI = 2
    }

    /// <summary>
    /// Amortisman modülünde yapılabilecek işlem türlerini tutan liste
    /// </summary>
    public enum ENUMAmortismanIslemTur : int
    {
        /// <summary>
        /// Amortisman işlem türünü ifade eden liste değeri
        /// </summary>
        AMORTISMAN = 1,

        /// <summary>
        /// Değerleme işlem türünü ifade eden liste değeri
        /// </summary>
        DEGERLEME = 2
    }

    /// <summary>
    /// Amortisman işleminin türlerini tutan liste
    /// </summary>
    public enum ENUMAmortismanTur : int
    {
        /// <summary>
        /// Normal amortismanı ifade eden liste değeri
        /// </summary>
        NORMAL = 1,

        /// <summary>
        /// Fevkalade amortismanı ifade eden liste değeri
        /// Fevkalade amortisman türü, malzemenin kalan süresine ve/veya kalan değerine
        /// bakılmaksızın tamamına amortisman uygulanması anlamına gelmektedir.
        /// </summary>
        FEVKALADE = 2,

        /// <summary>
        /// Değerleme işleminden kaynaklanmış amortismanı ifade eden liste değeri
        /// Değerleme işlemi yapılmış taşınır malzeme varsa değerlemeden
        /// kaynaklanan değere uygulanacak amortisman anlamına gelmektedir.
        /// </summary>
        DEGERLEME = 3,

        /// <summary>
        /// Enflasyon düzeltmesi işleminden kaynaklanmış amortismanı ifade eden liste değeri
        /// </summary>
        ENFLASYONDUZELTMESI = 4
    }

    /// <summary>
    /// Kullanıcının ekrandan yaptığı işlemin, seçilen malzemeler için mi yoksa
    /// girilmiş olan kriterlere uyan tüm malzemeler için mi olduğunu tutan liste
    /// Amortisman işleminin yanlışlıkla tüm malzemelere veya bir kısmına
    /// yapılmasını önlemek amacıyla konulmuş bir kontroldür.
    /// </summary>
    public enum ENUMAmortismanUygulamaTur : int
    {
        /// <summary>
        /// Ekrana girilmiş kriterlere uyan tüm malzemeleri ifade eden liste değeri
        /// </summary>
        KRITERLEREGORE = 1,

        /// <summary>
        /// Ekrandaki listeden seçilmiş malzemeleri ifade eden liste değeri
        /// </summary>
        SECILENLEREGORE = 2
    }

    /// <summary>
    /// Taşınır depo durum raporunun ne bazda alınacağının türünü tutan liste
    /// </summary>
    public enum ENUMDepoDurumRaporTur : int
    {
        /// <summary>
        /// Muhasebe birimi bazlı taşınır depo durum raporunu ifade eden liste değeri
        /// </summary>
        MUHASEBE = 0,

        /// <summary>
        /// Harcama birimi bazlı taşınır depo durum raporunu ifade eden liste değeri
        /// </summary>
        HARCAMA = 1,

        /// <summary>
        /// Ambar bazlı taşınır depo durum raporunu ifade eden liste değeri
        /// </summary>
        AMBAR = 2,

        /// <summary>
        /// İl bazlı taşınır depo durum raporunu ifade eden liste değeri
        /// </summary>
        IL = 3,

        /// <summary>
        /// Kurum bazlı taşınır depo durum raporunu ifade eden liste değeri
        /// </summary>
        KURUM = 4,

        /// <summary>
        /// Muhasebe birimi ve harcama birimi bazlı taşınır depo durum raporunu ifade eden liste değeri
        /// </summary>
        MUHASEBE_HARCAMA = 5
    }

    /// <summary>
    /// Yetkili bilgilerini tutan liste
    /// </summary>
    public enum ENUMImzaYer
    {
        /// <summary>
        /// Taşınır kayıt ve kontrol yetkilisini ifade eden liste değeri
        /// </summary>
        TASINIRKAYITYETKILISI = 1,

        /// <summary>
        /// Harcama birimi yetkilisini ifade eden liste değeri
        /// </summary>
        HARCAMAYETKILISI = 2,

        /// <summary>
        /// Muhasebe birimi yetkilisini ifade eden liste değeri
        /// </summary>
        MUHASEBEYETKILISI = 3,

        /// <summary>
        /// Taşınır konsolide görevlisini ifade eden liste değeri
        /// </summary>
        TASINIRKONSOLIDEGOREVLISI = 4,

        /// <summary>
        /// Merkezdeki taşınır konsolide görevlisini ifade eden liste değeri
        /// </summary>
        MERKEZDEKITASINIRKONSOLIDEGOREVLISI = 5,

        /// <summary>
        /// Kayıttan düşme komisyonu başkanını ifade eden liste değeri
        /// </summary>
        KAYITTANDUSMEKOMISYONBASKANI = 6,

        /// <summary>
        /// Kayıttan düşme komisyonu üyesini ifade eden liste değeri
        /// </summary>
        KAYITTANDUSMEKOMISYONUYESI = 7,

        /// <summary>
        /// Ambar devir ve teslim kurulu başkanını ifade eden liste değeri
        /// </summary>
        AMBARDEVIRVETESLIMKURULUBASKANI = 8,

        /// <summary>
        /// Ambar devir ve teslim kurulu 1. üyesini ifade eden liste değeri
        /// </summary>
        AMBARDEVIRVETESLIMKURULUUYE1 = 9,

        /// <summary>
        /// Ambar devir ve teslim kurulu 2. üyesini ifade eden liste değeri
        /// </summary>
        AMBARDEVIRVETESLIMKURULUUYE2 = 10,

        /// <summary>
        /// Sayım kurulu başkanını ifade eden liste değeri
        /// </summary>
        SAYIMKURULUBASKANI = 11,

        /// <summary>
        /// Sayım kurulu 1. üyesini ifade eden liste değeri
        /// </summary>
        SAYIMKURULUUYE1 = 12,

        /// <summary>
        /// Sayım kurulu 2. üyesini ifade eden liste değeri
        /// </summary>
        SAYIMKURULUUYE2 = 13,

        /// <summary>
        /// Birim yetkilisini ifade eden liste değeri
        /// </summary>
        BIRIMYETKILISI = 14,

        /// <summary>
        /// Üst yöneticiyi ifade eden liste değeri
        /// </summary>
        USTYONETICI = 15,

        /// <summary>
        /// Taşınır Kontrol yetkilisi
        /// </summary>
        TASINIRKONTROLYETKILISI = 16
    }

    /// <summary>
    /// Cetvellerde gösterilen rakamların gruplanmış halini tutan liste
    /// </summary>
    public enum ENUMCetvelRakamTipi
    {
        /// <summary>
        /// Cetvellerde gösterilen geçen yıldan devreden giriş rakamlarını ifade eden liste değeri
        /// </summary>
        GECENYILDANDEVREDENGIRIS = 1,

        /// <summary>
        /// Cetvellerde gösterilen geçen yıldan devreden çıkış rakamlarını ifade eden liste değeri
        /// </summary>
        GECENYILDANDEVREDENCIKIS = 2,

        /// <summary>
        /// Cetvellerde gösterilen yıl içinde giren rakamlarını ifade eden liste değeri
        /// </summary>
        YILICINDEGIREN = 3,

        /// <summary>
        /// Cetvellerde gösterilen yıl içinde çıkan rakamlarını ifade eden liste değeri
        /// </summary>
        YILICINDECIKAN = 4
    }

    /// <summary>
    /// Cetvel türlerini tutan liste
    /// </summary>
    public enum ENUMCetvelTipi
    {
        /// <summary>
        /// Taşınır sayım ve döküm cetvelini ifade eden liste değeri
        /// </summary>
        SAYIMCETVELI = 1,

        /// <summary>
        /// Harcama birimi taşınır yönetim hesabı cetvelini ifade eden liste değeri
        /// </summary>
        YONETIMHESABICETVELI = 2,

        /// <summary>
        /// Taşınır hesap cetvelini ifade eden liste değeri
        /// </summary>
        HESAPCETVELI = 3,

        /// <summary>
        /// Taşınır hesap cetvelini (İl) ifade eden liste değeri
        /// </summary>
        HESAPCETVELIIL = 4,

        /// <summary>
        /// Taşınır kesin hesap cetvelini ifade eden liste değeri
        /// </summary>
        KESINHESAPCETVELI = 5,

        /// <summary>
        /// Taşınır kesin hesap icmal cetvelini ifade eden liste değeri
        /// </summary>
        KESINHESAPCETVELIICMAL = 6,

        /// <summary>
        /// Müze/Kütüphane yönetim hesabı cetvelini ifade eden liste değeri
        /// </summary>
        MUZEKUTUPHANECETVELI = 7,

        /// <summary>
        /// Taşınır hesap cetvelini (Bölge) ifade eden liste değeri
        /// </summary>
        HESAPCETVELIBOLGE = 8
    }

    /// <summary>
    /// Taşınır işlem fişi, zimmet fişi gibi belgelerin alabileceği durumları tutan liste
    /// </summary>
    public enum ENUMBelgeDurumu
    {
        /// <summary>
        /// Yeni belge durumunu ifade eden liste değeri
        /// </summary>
        YENI = 1,

        /// <summary>
        /// Değiştirildi belge durumunu ifade eden liste değeri
        /// </summary>
        DEGISTIRILDI = 2,

        /// <summary>
        /// Onaylı belge durumunu ifade eden liste değeri
        /// </summary>
        ONAYLI = 5,

        /// <summary>
        /// Onay kaldırıldı belge durumunu ifade eden liste değeri
        /// </summary>
        ONAYKALDIR = 6,

        /// <summary>
        /// İptal belge durumunu ifade eden liste değeri
        /// </summary>
        IPTAL = 9,

        /// <summary>
        /// Belge durumlarından hiçbiri değilse tanımsız olduğunu ifade eden liste değeri
        /// </summary>
        TANIMSIZ = -1
    }

    public enum ENUMEOnayDurumu
    {
        /// <summary>
        /// Zimmet Form EOnaya gönderilmedi ifade eden liste değeri
        /// </summary>
        GONDERILMEDI = 0,

        /// <summary>
        /// Zimmet Form EOnaya gönderildi ifade eden liste değeri
        /// </summary>
        GONDERILDI = 1,

        /// <summary>
        /// Zimmet Form EOnaya Olumlu Cevap verildiğini ifade eden liste değeri
        /// </summary>
        CEVAPLADIKABUL = 4,

        /// <summary>
        /// Zimmet Form EOnaya Olumsuz Cevap verildiğini ifade eden liste değeri
        /// </summary>
        CEVAPLADIRED = 5,

        /// <summary>
        /// immet Form EOnay durumlarından hiçbiri değilse tanımsız olduğunu ifade eden liste değeri
        /// </summary>
        TANIMSIZ = -1
    }

    /// <summary>
    /// Taşınır malzemeler için kullanılan işlem tiplerini tutan liste
    /// </summary>
    public enum ENUMIslemTipi
    {
        GECICIIHRAC = 24,

        GECICIALINDI = -2,

        /// <summary>
        /// Değer arttırıcı işlem tipini ifade eden liste değeri
        /// </summary>
        DEGERARTTIR = -1,

        /// <summary>
        /// Değer arttırıcı işlem tipini ifade eden liste değeri
        /// </summary>
        ENFLASYONARTISI = -2,

        /// <summary>
        /// İşlem tiplerinden hiçbiri değilse belirsiz olduğunu ifade eden liste değeri
        /// </summary>
        BELIRSIZ = 0,

        /// <summary>
        /// Satın alma giriş işlem tipini ifade eden liste değeri
        /// </summary>
        SATINALMAGIRIS = 1,

        /// <summary>
        /// Bağış giriş işlem tipini ifade eden liste değeri
        /// </summary>
        BAGISGIRIS = 2,

        /// <summary>
        /// Sayım fazlası giriş işlem tipini ifade eden liste değeri
        /// </summary>
        SAYIMFAZLASIGIRIS = 3,

        /// <summary>
        /// İade giriş işlem tipini ifade eden liste değeri
        /// </summary>
        IADEGIRIS = 4,

        /// <summary>
        /// Devir giriş işlem tipini ifade eden liste değeri
        /// </summary>
        DEVIRGIRIS = 5,

        /// <summary>
        /// Üretilen giriş işlem tipini ifade eden liste değeri
        /// </summary>
        URETILENGIRIS = 6,

        /// <summary>
        /// Devir kurum giriş işlem tipini ifade eden liste değeri
        /// </summary>
        DEVIRGIRISKURUM = 7,

        /// <summary>
        /// Dağıtım İade giriş işlem tipini ifade eden liste değeri
        /// </summary>
        DAGITIMIADEGIRIS = 8,

        /// <summary>
        /// Kişiden zimmet düşme işlem tipini ifade eden liste değeri
        /// </summary>
        ZFDUSME = 46,

        /// <summary>
        /// Ortak alandan zimmet düşme işlem tipini ifade eden liste değeri
        /// </summary>
        DTLDUSME = 47,

        /// <summary>
        /// Yıl devri giriş işlem tipini ifade eden liste değeri
        /// </summary>
        YILDEVIRGIRIS = 48,

        /// <summary>
        /// Açılış işlem tipini ifade eden liste değeri
        /// </summary>
        ACILIS = 49,

        /// <summary>
        /// Tüketim çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        TUKETIMCIKIS = 50,

        /// <summary>
        /// Devir çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        DEVIRCIKIS = 51,

        /// <summary>
        /// Bağış çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        BAGISCIKIS = 52,

        /// <summary>
        /// Satış çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        SATISCIKIS = 53,

        /// <summary>
        /// Kullanılmaz hal çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        KULLANILMAZCIKIS = 54,

        /// <summary>
        /// Hurda çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        HURDACIKIS = 55,

        /// <summary>
        /// Devir kurum çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        DEVIRCIKISKURUM = 56,

        /// <summary>
        /// Sayım noksanı çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        SAYIMNOKSANICIKIS = 57,

        /// <summary>
        /// Yıl devri çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        YILDEVIRCIKIS = 58,

        /// <summary>
        /// Tüektim/İade (Emniyet) işlem tipini ifade eden liste değeri
        /// </summary>
        DAGITIMIADECIKIS = 59,

        URETIMCIKIS = 60,

        /// <summary>
        /// Kişiye zimmet verme işlem tipini ifade eden liste değeri
        /// </summary>
        ZFVERME = 90,

        /// <summary>
        /// Ortak alana zimmet verme işlem tipini ifade eden liste değeri
        /// </summary>
        DTLVERME = 91,

        /// <summary>
        /// KHK Giriş işlem tipini ifade eden liste değeri
        /// </summary>
        KHKGIRIS = 26,

        /// <summary>
        /// KHK çıkış işlem tipini ifade eden liste değeri
        /// </summary>
        KHKCIKIS = 27
    }

    /// <summary>
    /// Zimmet verme/düşme işlemlerinde zimmet verilecek malzemelerin tipini tutan liste
    /// </summary>
    public enum ENUMZimmetTipi
    {
        /// <summary>
        /// Demirbaş, makine veya cihaz malzemelerini ifade eden liste değeri
        /// </summary>
        DEMIRBASCIHAZ = 1,

        /// <summary>
        /// Taşıt veya iş makinesi malzemelerini ifade eden liste değeri
        /// </summary>
        TASITMAKINE = 2
    }

    /// <summary>
    /// Zimmet işlemlerinde malzemelerin verileceğini mi alınacağını mı tutan liste
    /// </summary>
    public enum ENUMZimmetVermeDusme
    {
        /// <summary>
        /// Zimmet işlemlerinde malzemelerin verileceğini ifade eden liste değeri
        /// </summary>
        ZIMMETVERME = 1,

        /// <summary>
        /// Zimmet işlemlerinde malzemelerin alınacağını ifade eden liste değeri
        /// </summary>
        ZIMMETTENDUSME = 2
    }

    /// <summary>
    /// Zimmet işleminin verme/düşme, kişi/ortak alan türlerini tutan liste
    /// </summary>
    public enum ENUMZimmetIslemTipi
    {
        /// <summary>
        /// Kişiye zimmet verme işlem türünü ifade eden liste değeri
        /// </summary>
        ZFVERME = 1001,

        /// <summary>
        /// Kişiden zimmet düşme işlem türünü ifade eden liste değeri
        /// </summary>
        ZFDUSME = 1002,

        /// <summary>
        /// Ortak alana zimmet verme işlem türünü ifade eden liste değeri
        /// </summary>
        DTLVERME = 1003,

        /// <summary>
        /// Ortak alandan zimmet düşme işlem türünü ifade eden liste değeri
        /// </summary>
        DTLDUSME = 1004
    }

    /// <summary>
    /// Taşınır malzemelerine ait hesap kodlarını tutan liste
    /// </summary>
    public enum ENUMTasinirHesapKodu
    {
        /// <summary>
        /// Tüketim taşınır malzemelerinin kodunu ifade eden liste değeri
        /// </summary>
        TUKETIM = 150,

        /// <summary>
        /// Makine ve cihaz taşınır malzemelerinin kodunu ifade eden liste değeri
        /// </summary>
        MAKINECIHAZ = 253,

        /// <summary>
        /// Taşıt taşınır malzemelerinin kodunu ifade eden liste değeri
        /// </summary>
        TASIT = 254,

        /// <summary>
        /// Demirbaş taşınır malzemelerinin kodunu ifade eden liste değeri
        /// </summary>
        DEMIRBAS = 255,

        /// <summary>
        /// Kütüphane taşınır malzemelerinin kodunu ifade eden liste değeri
        /// </summary>
        KUTUPHANE = 25507,

        /// <summary>
        /// Müze taşınır malzemelerinin kodunu ifade eden liste değeri
        /// </summary>
        MUZE = 25506
    }

    /// <summary>
    /// Demirbaş sicil numaralarının durumlarını tutan liste
    /// </summary>
    public enum ENUMSicilDurum
    {
        /// <summary>
        /// Aktif olan demirbaş sicil numaralarını ifade eden liste değeri
        /// </summary>
        AKTIF = 1,

        /// <summary>
        /// İptal/pasif olan demirbaş sicil numaralarını ifade eden liste değeri
        /// </summary>
        IPTAL = 2
        //DEVIRLECIKTI = 3,
        //BAGISLACIKTI = 4,
        //SATISLACIKTI = 5,
        //HURDAYLACIKTI = 6,
        //NUMARAGECICIOLARAKVERILDI = 99
    }

    /// <summary>
    /// Okuma, yazma, onaylama gibi yetki değerlerini tutan liste
    /// </summary>
    public enum ENUMOGY
    {
        /// <summary>
        /// Onaylama yetkisini ifade eden liste değeri
        /// </summary>
        Onaylama = 1,

        /// <summary>
        /// Görme yetkisini ifade eden liste değeri
        /// </summary>
        Gorme = 2,

        /// <summary>
        /// Yazma yetkisini ifade eden liste değeri
        /// </summary>
        Yazma = 3,

        /// <summary>
        /// Onay kaldırma yetkisini ifade eden liste değeri
        /// </summary>
        OnayKaldir = 5
        //Iptal = 6
    }

    /// <summary>
    /// Zimmet fişi belgelerinin türlerini tutan liste
    /// </summary>
    public enum ENUMZimmetBelgeTur
    {
        /// <summary>
        /// Belge türlerinden hiçbiri değilse belirsiz olduğunu ifade eden liste değeri
        /// </summary>
        BELIRSIZ = 0,

        /// <summary>
        /// Kişi zimmet fişi belge türünü ifade eden liste değeri
        /// </summary>
        ZIMMETFISI = 1,

        /// <summary>
        /// Ortak alan zimmet fişi belge türünü ifade eden liste değeri
        /// </summary>
        DAYANIKLITL = 2
    }

    public enum ENUMTasinirIslemFormOnayDurumu
    {
        ONAY = -1,//Bu durum BA Onay ekranı için eklendi çünkü sorguda sadece onay bekleyenler çekilecek
        TANIMSIZ = 0,
        GONDERILDIB = 1,
        GONDERILDIA = 2,
        TAMAMLANDI = 3,
        GERIGONDERILDI = 4,
    }

    public enum ENUMTasinirIslemFormOnayTur
    {
        TANIMSIZ = 0,
        BONAYI = 1,
        AONAYI = 2,
    }

    public enum ENUMMuhasebeIslemiOnayDurumu
    {
        TANIMSIZ = 0,
        GONDERILDI = 20,

        /// <summary>
        /// Islendi
        /// </summary>
        ISLENDI = 1,

        /// <summary>
        /// Hatali
        /// </summary>
        HATALI = 2,

        /// <summary>
        /// Hazir
        /// </summary>
        HAZIR = 3,

        /// <summary>
        /// Beklemede
        /// </summary>
        BEKLEMEDE = 4,

        /// <summary>
        /// HenuzIslenmedi
        /// </summary>
        HENUZISLENMEDI = 5,

        /// <summary>
        /// IslemBulunamadi
        /// </summary>
        ISLEMBULUNAMADI = 6,

        /// <summary>
        /// BilinmeyenDurum
        /// </summary>
        BILINMEYENDURUM = 7,
    }

    public enum ENUMAmortismanFormDurumu
    {
        /// <summary>
        /// Yeni belge durumunu ifade eden liste değeri
        /// </summary>
        YENI = 1,

        /// <summary>
        /// Değiştirildi belge durumunu ifade eden liste değeri
        /// </summary>
        DEGISTIRILDI = 2,

        /// <summary>
        /// Onaylı belge durumunu ifade eden liste değeri
        /// </summary>
        ONAYLI = 5,

        /// <summary>
        /// Onay kaldırıldı belge durumunu ifade eden liste değeri
        /// </summary>
        ONAYKALDIR = 6,

        /// <summary>
        /// İptal belge durumunu ifade eden liste değeri
        /// </summary>
        IPTAL = 9,

        /// <summary>
        /// Belge durumlarından hiçbiri değilse tanımsız olduğunu ifade eden liste değeri
        /// </summary>
        TANIMSIZ = -1
    }

    public enum ENUMMuhasebeIslemTur
    {
        TANIMSIZ = 0,

        /// <summary>
        /// Taşınır işlem fişi düzenlendiğinde
        /// </summary>
        TASINIRISLEMFISI = 1,

        /// <summary>
        /// Amortisman işlemi yapıldığında
        /// </summary>
        AMORTISMAN = 2,

        /// <summary>
        /// Değer artışı yapıldığında
        /// </summary>
        DEGERARTIS = 3,
        /// <summary>
        /// Enflasyon düzeltmesi yapıldığında
        /// </summary>
        ENFLASYONDUZELTMESI = 4,
    }

    public enum ENUMMBRaporTur
    {
        BIRIMMALZEMELISTESI = 1,
        BIRIMMALZEMELISTESIAMOTRISMANLI = 2,
        TAMAMIAMORTIEDILMISDEMIRBASLAR = 3,
        TAMAMIAMORTIEDILMEMISDEMIRBASLAR = 4,
        IHRACLISTESIAMORTISMANLI = 5,
        IHRACLISTESIMALZEMETURU = 6,
        ORANBAZINDATAMAMIAMORTIEDILMIS = 7,
        ORANBAZINDATAMAMIAMORTIEDILMEMISLER = 8,
        BIRMALZEMETURUNEGORE = 9,
        YILBAZINDATAMAMIAMORTIEDILMISLER = 10,
        YILBAZINDATAMAMIAMORTIEDILMEMISLER = 11,
        MALZEMEGURUBUNAGOREADETVEBEDELLER = 12,
        GENELSIGORTALISTESI = 13,
        OTOVETABLOLARHARICSIGORTALISTESI = 14,
        YERLESKEYERINEGORETOPLAMAMORTISMAN = 15,
        SOSYALTESISLEREAYRILANCARIAMAMORTISMAN = 16,
        YAZILIM = 17,
        BIRIMLEREGOREDEMIRBASAMORTISMAN = 18,
        BIRIMLEREGOREGAYRIMENKULAMORTISMAN = 19,
        DURANVARLIKHAREKETRAPORU = 20,
        DEMIRBASDEFTERI = 21,
        MUHASEBEBAKIYEKARSILASTIRMA = 22,
        ENFLASYONDUZELTMESI = 23,
        MALZEMEGURUBUNAGOREADETVEBEDELLERTTK = 24,
    }

    /// <summary>
    /// Odaların durumlarını tutan liste
    /// </summary>
    public enum ENUMOdaDurum
    {
        /// <summary>
        /// Aktif olan odaları ifade eden liste değeri
        /// </summary>
        AKTIF = 1,

        /// <summary>
        /// Pasif olan odaları ifade eden liste değeri
        /// </summary>
        PASIF = -2
    }

    public enum ENUMHarmoniTip
    {
        LOKASYON = 1,
        ZIMMET = 2
    }

    public enum ENUMTalepKarsilamaTur
    {
        TANIMSIZ = 0,
        TALEP = 1,
        ISTEK = 2
    }

    public enum ENUMEBYSCumhurbaskanligiBelgeTur
    {
        TANIMSIZ = 0,
        ZIMMETVERME = 1,
        ZIMMETDUSME = 2,
        TIFTUKETIMCIKIS = 3,
    }

    public enum ENUMRFIDEtiketIslemTur: int
    {
        UYGULAMA = 1,
        MOBIL = 2,
    }

    public enum ENUMRFIDEtiketDurum : int
    {
        TANIMSIZ = 0,
        BASILIYOR = 1,
        BASILDI = 10,
        HATA = 98,
        IPTAL = 99,
    }
}
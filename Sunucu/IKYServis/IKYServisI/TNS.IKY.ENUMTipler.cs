using System;

namespace TNS.IKY
{
    public enum ENUMYetkiIslem : int
    {
        OzlukOzet = 00,
        OzlukDetay = 01,
        Nufus = 02,
        Iletisim = 03,
        Askerlik = 04,
        Aile = 05,
        Ogrenim = 06,
        YabanciDil = 07,
        HizmetIciEgitim = 08,
        Izin = 09,
        Hizmetler = 10,
        CezaOdul = 11,
        MaasParametreleri = 12,
        Kartlar = 13,
        IzinDevir = 14,
        MalBildirimi = 15,
        KadroKontenjan = 16,
        Lojman = 17,
        Nitelik = 18,
        Tarihce = 19,
        KullaniciIslemleri = 20,
        SiteYonetimi = 21,
        LojmanTanim = 22,
        PersonelIslemleri = 23,
        HizliBilgi = 24,
        Gider = 25,
        Tanimsiz = 99,
    }

    public enum ENUMSozlukUnsurlari
    {
        //CINSIYET = 1,
        SAKATLIKDURUMU = 2,
        MEDENIDURUMU = 4,
        YAKININDURUMU = 5,
        //YAKINLIKDERECESI = 6,
        ASKERLIKDURUMU = 7,
        OGRENIMKOD = 8,
        UNIVERSITEKODU = 9,
        FAKULTEKODU = 10,
        BOLUMKODU = 11,
        OGRENIMYERI = 12,
        ASKERLIKRUTBESI = 13,
        ASKERLIKSINIFI = 14,
        DILKODU = 15,
        DILBELGETUR = 16,
        OGRENIMKOD2 = 17,
        OGRENIMBELGETUR = 18,
        KURSKOD = 19,
        DURUMKOD = 20,
        SOSYALGUVENCEKOD = 21,
        KADROSINIF = 22,
        //KADRODURUMU = 23,
        ATAMASEKLI = 24,
        ATAMATURU = 25,
        AYRILMANEDENI = 26,
        HAREKETDURUMU = 27,
        AILEOGRENIMTIPI = 28,
        CEZAODULTIPI = 30,
        KARTTIPI = 31,
        HIZMETTURU = 32,
        DAIRETURU = 33,
        SITEYONETICITIPI = 34
    }

    //    AKTIFKAYITHESAPLAMA = 207,

    public enum ENUMKadroDurumu
    {
        TANIMSIZ = 0,
        YENI = 1,

        AKTIFKAYIT = 106,
        AKTIFKAPANIYOR = 116,
        AKTIFKAPANDI = 126,
        AKTIFUCRETSIZIZIN = 136,
        AKTIFYENI = 146,

        AYRILDI = 201,
        AYRILDIHESAPLAMA = 208,
        REDDEDILDI = 209,
        SIL = 999,
        IPTALLERHARIC = 998,
        ONAYSIZLAR = 997
    }

    public enum ENUMYakinlikDerecesi
    {
        Esi = 91,
        Kizi = 92,
        Oglu = 93,
        Babasi = 94,
        Annesi = 95,
        Kardesi = 96,
        Diger = 198,
        Kendisi = 608
    }

    public enum ENUMYakininDurumu
    {
        Okuyor = 1,
        Mezun = 2,
        Evli = 3,
        Calisiyor = 4,
        Calismiyor = 5
    }

    public enum ENUMHizmetTuru
    {
        HIZMET = 1,
        KIDEM = 2,
        KHA = 3,
        EKEA = 4,
        IZIN = 5
    }

    public enum ENUMAyrilmaNedeni
    {
        YENIKADROATAMASI = 1,
    }

    public enum ENUMHareketDurumu
    {
        ONAYBEKLIYOR = 1,
        ONAYLANDI = 2,
        REDDEDILDI = 3
    }

    public enum ENUMPersonelTur
    {
        Memur = 1,
        Isci = 2
    }

    public enum ENUMAskerlikDurumu
    {
        Tecilli = 1,
        ErErbas = 2,
        YedekSubay = 3,
        Bedelli = 4,
        Muaf = 5,
        YedekSubayOgretmen = 6,
        ErOgretmen = 7,
        OGRENIMKOD = 8,
        ErliktenYedekSubaylıgaGecen = 9,
        YedekSubaylıktanErligeGecen = 10
    }

    public enum ENUMOgrenimKod
    {
        OkurYazar = 1,
        Ilkokul = 2,
        Ortaokul = 3,
        Ilkogretim = 4,
        Lise = 5,
        MeslekLisesi = 6,
        YuksekOkul = 7,
        LisansTamamlama = 8,
        Universite = 9,
        YuksekLisans = 10,
        Doktora = 11
    }

    public enum ENUMTerfiTipi
    {
        KHA = 1,
        EKEA = 2,
        KHAEKEA = 3
    }

    public enum ENUMTerfiDurumu
    {
        Beklemede = 1,
        Onay = 2,
        Red = 3
    }

    public enum ENUMHizmetTablolari
    {
        EmekliSandigi = 10,
        SSK = 20,
        Bagkur = 30,
        Borclanma = 40
    }

    public enum ENUMTahsisTipi
    {
        Tahsis = 1,
        Rezervasyon = 2
    }

    public enum ENUMKomisyonTip
    {
        Paraf = 1,
        Olur = 2,
        Imza = 3
    }

    public enum ENUMImzaTur
    {
        Uye = 1,
        Baskan = 2
    }

    public class Description : Attribute
    {
        public string Text;
        public Description(string text)
        {
            Text = text;
        }
    }
}

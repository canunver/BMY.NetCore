
namespace TNS.TRM
{
    public enum ENUMHibeTur
    {
        MAKINE = 1,
        SULAMA = 2,
        EKONOMIKYATIRIM = 3
    }

    public enum ENUMHibeBasvuruBolum
    {

    }

    public enum ENUMHibeMakineBasvuruDurum
    {
        ONLINE = -1,
        TANIMSIZ = 0,
        YENI = 1,
        DEGISTIRILDI = 2,

        PROJENUMARASIVERILDI = 5,

        ODEMEGERIGELDI = 6,

        SOZLESMEYAPILDI = 10,
        FATURAGELDI = 12,
        YERINDETESPIT = 14,
        ODEMEYEGONDER = 16,
        ODEMEBASLAT = 18,
        BANKAYAGONDERILDI = 19,
        ODENDI = 20,
        BASVURUREDDEDILDI = 99
    }

    public enum ENUMHibeSulamaBasvuruDurum
    {
        TANIMSIZ = 0,
        YENI = 1,
        DEGISTIRILDI = 2,

        PROJENUMARASIVERILDI = 5,

        ODEMEGERIGELDI = 6,

        SOZLESMEYAPILDI = 10,
        FATURAGELDI = 12,
        YERINDETESPIT = 14,
        ODEMEYEGONDER = 16,
        ODEMEBASLAT = 18,
        BANKAYAGONDERILDI = 19,
        ODENDI = 20,
        BASVURUREDDEDILDI = 99,
        ASILYEDEK = 100,
    }

    public enum ENUMHibeSulamaRedTip
    {
        BASVURURED = 1,
        SOZLESMEIMZALAMAMA = 2,
        SOZLESMEFESHI = 3,
    }

    public enum ENUMHibeYatirimBasvuruDurum
    {
        ONLINE = -1,
        TANIMSIZ = 0,
        YENI = 1,
        DEGISTIRILDI = 2,
        ILDEKABULEDILENLER = 3,
        KABULEDILDI = 5,
        SOZLESMEYAPILDI = 10,
        UYGULAMASOZLESMESIYAPILDI = 11,
        BASVURUREDDEDILDIMERKEZ = 98,
        BASVURUREDDEDILDI = 99,
        DEGERLENDIRILDI = 100,
        SOZLESMESILINDI = 101,
        GERICEKILDI = 102,
        SOZLESMEIMZALAMADI = 103,
    }

    public enum ENUMYatirimRaporAsama
    {
        TANIMSIZ = 0,
        BASVURU = 1,
        ILKABUL = 2,
        MERKEZKABUL = 3,
        SOZLESMEYAPILAN = 4,
        SOZLESMESIDEVAMEDEN = 5,
        SOZLESMESIBITEN = 6,
        SOZLESMESIIPTALOLAN = 7,
        HAKEDISODENEN = 8
    }

    public enum ENUMHibeYatirimUygulamaSozlesmeTur
    {
        INSAAT = 1,
        MALALIMI = 2,
        ANAHTARTESLIM = 3,
    }

    public enum ENUMHibeYatirimSozlesmeDurum
    {
        DEVAMEDIYOR = 1,
        BITTI = 2,
        HATALI = 3,
        IPTAL = 4
    }

    public enum ENUMHibeYatirimUygulamaSozlesmeDurum
    {
        DEVAMEDIYOR = 1,
        BITTI = 2,
        HATALI = 3,
        IPTAL = 4
    }

    public enum ENUMHibeYatirimHakedisDurum
    {
        YENI = 1,
        ONAYLANDI = 2,
        ODEMEDE = 16,
        ODEMEBASLAT = 18,
        BANKAYAGONDERILDI = 19,
        ODENDI = 20,
        HATALI = 98,
        RED = 99
    }

    public enum ENUMHibeMakineHukukiStatu
    {
        TANIMSIZ = 0,
        GERCEKKISI = 1,
        TUZELKISI = 2,
        GENCCIFTCI = 3,
        SUTBIRLIK = 4
    }

    public enum ENUMHibeSulamaHukukiStatu
    {
        TANIMSIZ = 0,
        GERCEKKISI = 1,
        TUZELKISI = 2,
        GENCCIFTCI = 3,
        KOOPERATIF = 4,
        BIRLIK = 5,
        TUMGERCEKKISI = 11,
        TUMTUZELKISI = 12
    }

    public enum ENUMHibeYatirimHukukiStatu
    {
        TANIMSIZ = 0,
        GERCEKKISI = 1,
        TUZELKISI = 2
    }

    public enum ENUMSulamaMalzemeOzellikKolonTip
    {
        TANIMSIZ = 0,
        INT = 1,
        STRING = 2,
        DOUBLE = 3
    }

    public enum ENUMHibeSulamaMalzemeTip
    {
        TANIMSIZ = 0,
        PROJE = 1,
        UYGULAMA = 2
    }

    public enum ENUMHibeOdemeTuru
    {
        BANKAGONDER = 1,
        ODENDI = 2,
        TANIMSIZ = 0
    }

    public enum ENUMYatirimGiderKalemTuru
    {
        HIBEESASINSAAT = 1,
        HIBEESASMAKINE = 2,
        HIBEESAS = 3,
        AYNIINSAAT = 4,
        AYNIMAKINE = 5,
        AYNI = 6,
        HIBEESASAYNI = 7
    }

    public enum ENUMYatirimFinansKaynakTip
    {
        HIBEDESTEGI = 1,
        KATILIMPAYI = 2
    }
}
using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPersonelKadro : TABOrtak
    {
        public TABPersonelKadro(VeriErisim veriErisim) : base(veriErisim) {
        }

        /// <summary>
        /// Tablo adı
        /// </summary>
        public static string TABLOAD = "IKYPERSONELKADRO";
        public static string TABLOKADROAYAR = "IKYPERSONELKADROAYAR";
        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        public enum ENUMALANLAR : int
        {
            /// <summary>
            /// KOD
            /// </summary>
            KOD = 0,
            /// <summary>
            /// SIRANO
            /// </summary>
            PKOD = 1,
            /// <summary>
            /// Organizasyon kodu
            /// </summary>
            ORGANIZASYONKOD = 2,
            UNVANKOD = 3,
            /// <summary>
            /// Kadro Derece
            /// </summary>
            KADRODERECE = 4,
            /// <summary>
            /// Kadro Tarihi
            /// </summary>
            KADROTARIH = 5,
            /// <summary>
            /// Evrak Numarası
            /// </summary>
            EVRAKNO = 6,
            /// <summary>
            /// Yasal Dayanak
            /// </summary>
            YASALDAYANAK = 7,
            /// <summary>
            /// Onay Tarihi
            /// </summary>
            ONAYTARIH = 8,
            /// <summary>
            /// Açıklama
            /// </summary>
            ACIKLAMA = 9,

            /// <summary>
            /// Ödemeye Esas Derece
            /// </summary>
            ODEMEYEESASDERECE = 10,
            /// <summary>
            /// Ödemeye Esas Kademe
            /// </summary>
            ODEMEYEESASKADEME = 11,
            /// <summary>
            /// Ödemeye Esas Tarih
            /// </summary>
            ODEMEYEESASTARIH = 12,
            /// <summary>
            /// Kazanılmış Hak Derece
            /// </summary>
            KAZANILMISHAKDERECE = 13,
            /// <summary>
            /// Kazanılmış Hak Kademe
            /// </summary>
            KAZANILMISHAKKADEME = 14,
            /// <summary>
            /// Kazanılmış Hak Tarih
            /// </summary>
            KAZANILMISHAKTARIH = 15,
            /// <summary>
            /// Emekliye Esas Derece
            /// </summary>
            EMEKLIYEESASDERECE = 16,
            /// <summary>
            /// Emekliye Esas Kademe
            /// </summary>
            EMEKLIYEESASKADEME = 17,
            /// <summary>
            /// Emekliye Esas Tarih
            /// </summary>
            EMEKLIYEESASTARIH = 18,
            /// <summary>
            /// Ödemeye Esas Gösterge
            /// </summary>
            ODEMEYEESASGOSTERGE = 19,
            /// <summary>
            /// Ödemeye Esas Ek Gösterge
            /// </summary>
            ODEMEYEESASEKGOSTERGE = 20,
            /// <summary>
            /// Kazanılmış Gösterge
            /// </summary>
            KAZANILMISGOSTERGE = 21,
            /// <summary>
            /// Kazanılmış Ek Gösterge
            /// </summary>
            KAZANILMISEKGOSTERGE = 22,
            /// <summary>
            /// Emekliye Esas Gösterge
            /// </summary>
            EMEKLIYEESASGOSTERGE = 23,
            /// <summary>
            /// Emekliye Esas Ek Gösterge
            /// </summary>
            EMEKLIYEESASEKGOSTERGE = 24,
            KADRODURUMU = 25,
            GOREVYERI = 26,
            HIZMETSEBEBI = 27,
            HIZMETSEBEBIKOD = 28,

            BELGECINSI = 29,
            BELGENO = 30,
            BELGETARIHI = 31,
            AYRILMATARIHI = 32,
            AYRILMANEDENI = 33,
            //EskiOrganizasyonKod = 34,
            //EskiUnvanKod = 35,
            //EskiKadroDerece = 36,
            //EskiKazanilmisHakDerece = 37,
            //EskiKazanilmisHakKademe = 38,
            //EskiKazanilmisHakGosterge = 39,
            //EskiKazanilmisHakEkGosterge = 40,
            //EskiOdemeyeEsasDerece = 41,
            //EskiOdemeyeEsasKademe = 42,
            //EskiOdemeyeEsasGosterge = 43,
            //EskiOdemeyeEsasEkGosterge = 44,
            //EskiEmekliyeEsasDerece = 45,
            //EskiEmekliyeEsasKademe = 46,
            //EskiEmekliyeEsasGosterge = 47,
            //EskiEmekliyeEsasEkGosterge = 48,
            EskiKadroKod = 34,
            GorunenUnvanKodu = 35,
            AtamaTarihi = 36,
            Onaylayan = 37,
            EskiHizmetFlag = 38,
            EskiHizmetTipi = 39,
            KurumDisi = 40,
            HizmetGunSayisi = 41,
            AYRILMAGEREKCESI = 42,
            KHADurum = 43,
            YedekArtis = 44,
            MuhasebeBirimi = 45,
            MuhasebeKodu = 46,
            KurumsalKod = 47,
            PERSONELTUR = 48,
            ILISKILIKADROISLEMNO = 49,
            ATANACAGIKADROUNVANKOD = 50,
            ATANACAGIKADRODERECE = 51,
            GOREVORGANIZASYONKOD = 52,

            HITAPKOD = 53,
            HITAPKHADURUM = 54,
            HITAPSGKNEVI = 55,
            HITAPSICILNO = 56,
            HITAPBORCNEVI = 57,
            HITAPTOPLAMTUTAR = 58,
            HITAPODENENMIKTAR = 59,
            HITAPODEMETARIHI = 60,
            TAYINOLACAGIBIRIMKOD = 61,
            MESLEKKODU = 62
        }
        public enum ENUMALANLARKADROAYARI : int
        {
            /// <summary>
            /// PKOD
            /// </summary>
            PKOD = 0,
            /// <summary>
            /// EMEKLITERFIAYI
            /// </summary>
            EMEKLITERFIAYI = 1,
            /// <summary>
            /// KAZANILMISTERFIAYI
            /// </summary>
            KAZANILMISTERFIAYI = 2,
            /// <summary>
            /// KIDEMTERFIAYI
            /// </summary>
            KIDEMTERFIAYI = 3,
            /// <summary>
            /// MAXIMUMDERECE
            /// </summary>
            MAXIMUMDERECE = 4,
            /// <summary>
            /// EMEKLITERFITARIHI
            /// </summary>
            EMEKLITERFITARIHI = 5,
            /// <summary>
            /// KAZANILMISTERFITARIHI
            /// </summary>
            KAZANILMISTERFITARIHI = 6,
            /// <summary>
            /// KADRODERECEAKTIF
            /// </summary>
            KADRODERECEAKTIF = 7
        }
        /// <summary>
        /// Tablo alanları
        /// </summary>
        public static string[] ALANLAR = { "KOD", "PKOD", "ORGANIZASYONKOD", "UNVANKOD", "KADRODERECE", "KADROTARIH", "EVRAKNO",
                                           "YASALDAYANAK", "ONAYTARIH", "ACIKLAMA", "ODEMEYEESASDERECE", "ODEMEYEESASKADEME",
                                           "ODEMEYEESASTARIH", "KAZANILMISHAKDERECE", "KAZANILMISHAKKADEME", "KAZANILMISHAKTARIH",
                                           "EMEKLIYEESASDERECE", "EMEKLIYEESASKADEME", "EMEKLIYEESASTARIH", "ODEMEYEESASGOSTERGE",
                                           "ODEMEYEESASEKGOSTERGE", "KAZANILMISGOSTERGE", "KAZANILMISEKGOSTERGE", "EMEKLIYEESASGOSTERGE",
                                           "EMEKLIYEESASEKGOSTERGE", "KADRODURUMU", "GOREVYERI", "HIZMETSEBEBI", "HIZMETSEBEBIKOD",
                                           "BELGECINSI", "BELGENO", "BELGETARIHI", "AYRILMATARIHI", "AYRILMANEDENI",
                                           "EskiKadroKod", "GorunenUnvanKodu", "AtamaTarihi", "Onaylayan",
                                           "EskiHizmetFlag","EskiHizmetTipi","KurumDisi","HizmetGunSayisi", "AYRILMAGEREKCESI", "KHADurum", "YedekArtis",
                                           "MuhasebeBirimi", "MuhasebeKodu", "KurumsalKod", "PERSONELTUR", "ILISKILIKADROISLEMNO", "ATANACAGIKADROUNVANKOD", "ATANACAGIKADRODERECE", "GOREVORGANIZASYONKOD", "HITAPKOD", "HITAPKHADURUM", "HITAPSGKNEVI", "HITAPSICILNO", "HITAPBORCNEVI", "HITAPTOPLAMTUTAR", "HITAPODENENMIKTAR", "HITAPODEMETARIHI", "TAYINOLACAGIBIRIMKOD", "MESLEKKODU"}; // Ticaret için, "MESLEKKODU", "HIZMETKURUMU" olacak

        public static string[] ALANLARKADROAYAR = { "PKOD", "EMEKLITERFIAYI", "KAZANILMISTERFIAYI", "KIDEMTERFIAYI", "MAXIMUMDERECE", "EMEKLITERFITARIHI", "KAZANILMISTERFITARIHI",
                                           "KADRODERECEAKTIF"};

        public Sonuc KaydetKadro(PersonelKadro p)
        {
            string sql;
            if (p.kod == 0)
            {
                p.kod = TABGenel.SayacDegerAlInt(veriErisim, TABLOAD);
                if (!string.IsNullOrWhiteSpace(veriErisim.hata)) return new Sonuc(veriErisim.hata);
            }
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            sql += VeriErisim.DegerYap(p.organizasyonKodu) + ",";
            if (p.unvan > 0)
                sql += VeriErisim.DegerYap(p.unvan) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
            sql += VeriErisim.DegerYap(p.kadroTarih) + ",";
            sql += VeriErisim.DegerYap(p.evrakNo) + ",";
            sql += VeriErisim.DegerYap(p.yasalDayanak) + ",";
            sql += VeriErisim.DegerYap(p.onayTarih) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasDerece) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasKademe) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasTarih) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakDerece) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakKademe) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakTarih) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasDerece) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasKademe) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasTarih) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasGosterge) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisGosterge) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasGosterge) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasEkGosterge) + ",";
            if ((int)p.kadroDurum > 0)
                sql += VeriErisim.DegerYap((int)p.kadroDurum) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.gorevYeri) + ",";
            sql += VeriErisim.DegerYap(p.hizmetSebebi) + ",";
            if (p.hizmetSebebiKod > 0)
                sql += VeriErisim.DegerYap(p.hizmetSebebiKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";

            sql += VeriErisim.DegerYap(p.belgeCinsi) + ",";
            sql += VeriErisim.DegerYap(p.belgeNo) + ",";
            sql += VeriErisim.DegerYap(p.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.ayrilmaTarihi) + ",";
            //if (p.ayrilmaNedeni > 0)
            sql += VeriErisim.DegerYap(p.ayrilmaNedeni) + ",";
            //else
            //    sql += VeriErisim.DegerYap((string)null) + ",";

            sql += VeriErisim.DegerYap(p.eskiKod) + ",";
            if (p.vekilUnvanKod > 0)
                sql += VeriErisim.DegerYap(p.vekilUnvanKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.atamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.onaylayan) + ",";
            if (p.eskiHizmetTipiFlag > 0)
                sql += VeriErisim.DegerYap(p.eskiHizmetTipiFlag) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.eskiHizmetTipi > 0)
                sql += VeriErisim.DegerYap(p.eskiHizmetTipi) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(0) + ",";
            sql += VeriErisim.DegerYap(p.hizmetGunSayisi) + ",";
            if (p.ayrilmaGerekcesi > 0)
                sql += VeriErisim.DegerYap(p.ayrilmaGerekcesi) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.KHADurumu) + ",";
            sql += VeriErisim.DegerYap(0) + ",";
            sql += VeriErisim.DegerYap(p.muhasebeBirimi) + ",";
            sql += VeriErisim.DegerYap(p.muhasebeKodu) + ",";
            sql += VeriErisim.DegerYap((string.IsNullOrWhiteSpace(p.kurumsalKod) ? null : p.kurumsalKod.Replace(".", "")));
            sql += "," + VeriErisim.DegerYap(p.personelTip);
            sql += "," + VeriErisim.DegerYap(p.iliskiliKadroIslemNo);
            if (p.atananacagiKadroUnvanKod > 0)
                sql += "," + VeriErisim.DegerYap(p.atananacagiKadroUnvanKod);
            else
                sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap(p.atananacagiKadroDerece);
            sql += "," + VeriErisim.DegerYap(p.gorevOrganizasyonKodu);
            sql += "," + VeriErisim.DegerYap(p.hitapKod);
            sql += "," + VeriErisim.DegerYap(p.hitapkhaDurum);
            sql += "," + VeriErisim.DegerYap((int)p.hitapsgkNevi);
            sql += "," + VeriErisim.DegerYap(p.hitapSicilNo);
            sql += "," + VeriErisim.DegerYap(p.hitapBorcNevi);
            sql += "," + VeriErisim.DegerYap(p.hitaptoplamTutar);
            sql += "," + VeriErisim.DegerYap(p.hitapodenenMiktar);
            sql += "," + VeriErisim.DegerYap(p.hitapodemeTarihi);
            if (p.tayinOlacagiBirim > 0)
                sql += "," + VeriErisim.DegerYap(p.tayinOlacagiBirim);
            else
                sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap(p.meslekKodu);
            //sql += "," + VeriErisim.DegerYap((string)null);
            sql += ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleKadro(PersonelKadro p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ORGANIZASYONKOD], p.organizasyonKodu) + ",";
            if (p.unvan > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.UNVANKOD], p.unvan) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.UNVANKOD], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KADRODERECE], p.kadroDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KADROTARIH], p.kadroTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EVRAKNO], p.evrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.YASALDAYANAK], p.yasalDayanak) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ONAYTARIH], p.onayTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ACIKLAMA], p.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ODEMEYEESASDERECE], p.odemeyeEsasDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ODEMEYEESASKADEME], p.odemeyeEsasKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ODEMEYEESASTARIH], p.odemeyeEsasTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KAZANILMISHAKDERECE], p.kazanilmisHakDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KAZANILMISHAKKADEME], p.kazanilmisHakKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KAZANILMISHAKTARIH], p.kazanilmisHakTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EMEKLIYEESASDERECE], p.emekliyeEsasDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EMEKLIYEESASKADEME], p.emekliyeEsasKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EMEKLIYEESASTARIH], p.emekliyeEsasTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ODEMEYEESASGOSTERGE], p.odemeyeEsasGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ODEMEYEESASEKGOSTERGE], p.odemeyeEsasEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KAZANILMISGOSTERGE], p.kazanilmisGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KAZANILMISEKGOSTERGE], p.kazanilmisEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EMEKLIYEESASGOSTERGE], p.emekliyeEsasGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EMEKLIYEESASEKGOSTERGE], p.emekliyeEsasEkGosterge) + ",";
            if ((int)p.kadroDurum > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KADRODURUMU], (int)p.kadroDurum) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KADRODURUMU], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.GOREVYERI], p.gorevYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HIZMETSEBEBI], p.hizmetSebebi) + ",";
            if (p.hizmetSebebiKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HIZMETSEBEBIKOD], p.hizmetSebebiKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HIZMETSEBEBIKOD], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.BELGECINSI], p.belgeCinsi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.BELGENO], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.BELGETARIHI], p.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMATARIHI], p.ayrilmaTarihi) + ",";
            //if (p.ayrilmaNedeni > 0)
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMANEDENI], p.ayrilmaNedeni) + ",";
            //else
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMANEDENI], (string)null) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EskiKadroKod], p.eskiKod) + ",";
            if (p.vekilUnvanKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.GorunenUnvanKodu], p.vekilUnvanKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.GorunenUnvanKodu], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AtamaTarihi], p.atamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.Onaylayan], p.onaylayan) + ",";
            if (p.eskiHizmetTipiFlag > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EskiHizmetFlag], p.eskiHizmetTipiFlag) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EskiHizmetFlag], (string)null) + ",";
            if (p.eskiHizmetTipi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EskiHizmetTipi], p.eskiHizmetTipi) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.EskiHizmetTipi], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KurumDisi], p.kurumDisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HizmetGunSayisi], p.hizmetGunSayisi) + ",";
            if (p.ayrilmaGerekcesi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMAGEREKCESI], p.ayrilmaGerekcesi) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMAGEREKCESI], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KHADurum], p.KHADurumu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.YedekArtis], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.MuhasebeBirimi], p.muhasebeBirimi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.MuhasebeKodu], p.muhasebeKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KurumsalKod], (string.IsNullOrWhiteSpace(p.kurumsalKod) ? null : p.kurumsalKod.Replace(".", "")));
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.PERSONELTUR], p.personelTip);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ILISKILIKADROISLEMNO], p.iliskiliKadroIslemNo);
            if (p.atananacagiKadroUnvanKod > 0)
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ATANACAGIKADROUNVANKOD], p.atananacagiKadroUnvanKod);
            else
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ATANACAGIKADROUNVANKOD], (string)null);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ATANACAGIKADRODERECE], p.atananacagiKadroDerece);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.GOREVORGANIZASYONKOD], p.gorevOrganizasyonKodu);

            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HITAPKOD], p.hitapKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HITAPKHADURUM], p.hitapkhaDurum);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HITAPSGKNEVI], (int)p.hitapsgkNevi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HITAPSICILNO], p.hitapSicilNo);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HITAPBORCNEVI], p.hitapBorcNevi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HITAPTOPLAMTUTAR], p.hitaptoplamTutar);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HITAPODENENMIKTAR], p.hitapodenenMiktar);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HITAPODEMETARIHI], p.hitapodemeTarihi);

            if (p.tayinOlacagiBirim > 0)
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.TAYINOLACAGIBIRIMKOD], p.tayinOlacagiBirim);
            else
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.TAYINOLACAGIBIRIMKOD], (string)null);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.MESLEKKODU], p.meslekKodu);
            //sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.MESLEKKODU], (string)null);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleAyrilis(PersonelKadro kadro)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KADRODURUMU], (int)kadro.kadroDurum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMANEDENI], kadro.ayrilmaNedeni) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMATARIHI], kadro.ayrilmaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMAGEREKCESI], kadro.ayrilmaGerekcesi) + ",";

            if (kadro.tayinOlacagiBirim > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.TAYINOLACAGIBIRIMKOD], kadro.tayinOlacagiBirim);
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.TAYINOLACAGIBIRIMKOD], (string)null);

            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HizmetGunSayisi], kadro.hizmetGunSayisi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.KOD], "=", kadro.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc TumAktifleriKapat(PersonelKadro kadro, bool tarihiBosOlanlar)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KADRODURUMU], (int)ENUMKadroDurumu.AYRILDI);
            if (tarihiBosOlanlar)
            {
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMATARIHI], kadro.atamaTarihi);
                sql += ", " + ALANLAR[(int)ENUMALANLAR.HizmetGunSayisi] + " = dbo.gunDondur(" + ALANLAR[(int)ENUMALANLAR.AtamaTarihi] + ", " + ALANLAR[(int)ENUMALANLAR.AYRILMATARIHI] + ", 1, NULL, HizmetGunSayisi" + "), ";
                if (kadro.hizmetSebebiKod > 0)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMAGEREKCESI], kadro.hizmetSebebiKod);
                else
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMAGEREKCESI], (string)null);

            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.PKOD], "=", kadro.personelKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.KOD], "<>", kadro.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", TABGenel.KadroDurumuAktifler(ALANLAR[(int)ENUMALANLAR.KADRODURUMU]));
            if (tarihiBosOlanlar)
                sql = VeriErisim.KosulEkle(sql, "AND", ALANLAR[(int)ENUMALANLAR.AYRILMATARIHI] + " IS NULL");

            return KomutCalistir(sql, 0);
        }

        public Sonuc GuncelleKadroDurumu(PersonelKadro kadro, bool eskiMi, int kadroIslemNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KADRODURUMU], (int)kadro.kadroDurum);
            if (eskiMi)
            {
                //sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMANEDENI], kadro.ayrilmaNedeni);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMATARIHI], kadro.ayrilmaTarihi);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMAGEREKCESI], kadro.ayrilmaGerekcesi);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HizmetGunSayisi], kadro.hizmetGunSayisi);
            }
            else
            {
                // sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ONAYTARIH], kadro.onayTarih);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.Onaylayan], kadro.onaylayan);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.ILISKILIKADROISLEMNO], kadroIslemNo);
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.KOD], "=", kadro.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader HizmetGunuDondur(DateTime baslangicT, DateTime bitisT, int kadroKod)
        {
            string sql = "";
            TNSDateTime tnsBitisT = new TNSDateTime(bitisT);
            TNSDateTime tnsbaslangicT = new TNSDateTime(baslangicT);

            if (kadroKod > 0)
                sql = "select dbo.gunDondur(AtamaTarihi, " + VeriErisim.DegerYap(tnsBitisT) + ", 1, null, HizmetGunSayisi) from IKYPERSONELKADRO where kod = '" + kadroKod + "'";
            else
                if (baslangicT.Year != 1 && bitisT.Year != 1)
                sql = "select dbo.gunDondur(" + VeriErisim.DegerYap(tnsbaslangicT) + ", " + VeriErisim.DegerYap(tnsBitisT) + ", 1, null, 0)";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YukselebilecegiDereceDondur(int personelKod)
        {
            string sql = "select YUKSELECEGIDERECE from IKYPERSONELOGRENIM O LEFT OUTER JOIN IKYSOZLUK S ON S.KOD = O.OGRENIMKOD LEFT OUTER JOIN IKYOGRENIMDERECE D ON S.KAYITKOD = D.OGRENIMKOD where o.BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) FROM IKYPERSONELOGRENIM WHERE PKOD = o.PKOD) AND O.pkod = " + personelKod;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc GuncelleEskiKadro(PersonelKadro kadro)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.KADRODURUMU], (int)kadro.kadroDurum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMATARIHI], kadro.ayrilmaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.AYRILMAGEREKCESI], kadro.ayrilmaGerekcesi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.HizmetGunSayisi], kadro.hizmetGunSayisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.YedekArtis], 0);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.KOD], "=", kadro.kod));

            return KomutCalistir(sql, 1);
        }

        //public Sonuc GuncelleYedekArtis(PersonelKadro kadro)
        //{
        //    string sql = "";

        //    sql = "UPDATE " + TABLOAD + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMALANLAR.YedekArtis], 0);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.KOD], "=", kadro.kod));

        //    return KomutCalistir(sql, 1);
        //}

        public Sonuc SonKaydiAktifYap(int personelKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMALANLAR.KADRODURUMU] + " = " + VeriErisim.DegerYap((int)ENUMKadroDurumu.AKTIFKAPANDI);
            sql += " WHERE " + ALANLAR[(int)ENUMALANLAR.KOD] + " = (SELECT MAX(KOD) FROM  " + TABLOAD + " IC WHERE IC.PKOD = " + TABLOAD + ".PKOD AND " + TABGenel.KadroDurumuOnaylilar("IC.KADRODURUMU") + " AND IC.AtamaTarihi = (SELECT MAX(ENIC.AtamaTarihi) FROM " + TABLOAD + " ENIC WHERE ENIC.PKOD=IC.PKOD AND " + TABGenel.KadroDurumuOnaylilar("ENIC.KADRODURUMU") + " ))";
            sql += " AND " + ALANLAR[(int)ENUMALANLAR.PKOD] + " = " + VeriErisim.DegerYap(personelKod);
            return KomutCalistir(sql, 0);
        }

        public int AktifSay(int personelKod, bool acik)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMALANLAR.PKOD], "=", personelKod));
            if (acik)
                sql = VeriErisim.KosulEkle(sql, "AND", TABGenel.KadroDurumuAktifler(TABLOAD + "." + ALANLAR[(int)ENUMALANLAR.KADRODURUMU]));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", TABGenel.KadroDurumuAktifAciklarYenisiz(TABLOAD + "." + ALANLAR[(int)ENUMALANLAR.KADRODURUMU]));
            return this.veriErisim.TekKolonSQLInt(sql);
        }

        public static string SorguAlanlar(bool sadeceSayilar)
        {
            string sql = "";
            if (sadeceSayilar)
                sql = "SELECT COUNT(*)";
            else
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", IKYUNVAN.AD + ' (' + IKYUNVAN.ADKISA + ')' UNVANAD, IKYUNVAN.SINIFKOD, " +
                       " IKYPERSONELTIPI.AD TIPAD, IKYSOZLUKHIZMETSEBEBI.SEBEPADI, s2.KAYITAD as sinifAd, IKYPERSONELIC.TCKIMLIKNO, IKYPERSONELIC.AD + ' ' + IKYPERSONELIC.SOYAD ADSOYAD, " +
                       " s2.KISAAD SINIFAD," +
                       " dbo.gunDondur(AtamaTarihi, AYRILMATARIHI, 1, null, HizmetGunSayisi) HizmetGunSayisi, o.ADI BIRIMADI, U2.AD + ' (' + U2.ADKISA + ')' GORUNENUNVANAD, IKYSOZLUKHIZMETSEBEBI.TIP GIRISSEBEPTIP, HS2.TIP AYRILMASEBEPTIP, " +
                       " HS2.SEBEPADI, IKYPERSONELTIPI.HITAP, O2.ADI GOREVYAPTIGIBIRIM, O.KISAADI BIRIMKISAAD, IKYPERSONELTIPI.KURUMDISI PTKURUMDISI, O3.ADI GIDECEGIBIRIMAD, O.TURKOD, O2.TURKOD";

            sql += " FROM " + TABLOAD + " LEFT OUTER JOIN IKYUNVAN ON " + TABLOAD + ".UNVANKOD = IKYUNVAN.KOD ";
            sql += " LEFT OUTER JOIN " + TABPersonel.TABLOAD + " IKYPERSONELIC ON IKYPERSONELIC." + TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.KOD] + "=" + ALANLAR[(int)ENUMALANLAR.PKOD];
            sql += " LEFT OUTER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD = IKYPERSONELKADRO.PERSONELTUR " +
                   " LEFT OUTER JOIN IKYSOZLUK as s2 ON s2.KOD = IKYUNVAN.SINIFKOD " +
                   " LEFT OUTER JOIN IKYSOZLUKHIZMETSEBEBI ON IKYSOZLUKHIZMETSEBEBI.KOD = IKYPERSONELKADRO.HIZMETSEBEBIKOD " +
                   " LEFT OUTER JOIN IKYSOZLUKHIZMETSEBEBI HS2 ON HS2.KOD = IKYPERSONELKADRO.AYRILMAGEREKCESI " +
                   " LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM O ON O.KOD = IKYPERSONELKADRO.ORGANIZASYONKOD " +
                   " LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM O2 ON O2.KOD = IKYPERSONELKADRO.GOREVORGANIZASYONKOD " +
                   " LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM O3 ON O3.KOD = IKYPERSONELKADRO.TAYINOLACAGIBIRIMKOD " +
                   " LEFT OUTER JOIN IKYUNVAN as U2 ON " + TABLOAD + ".GorunenUnvanKodu = U2.KOD ";
            return sql;
        }

        public IDataReader ListeleKadro(PersonelKadro p, bool tarihArtan, bool sadeceSayilar)
        {
            string sql = SorguAlanlar(sadeceSayilar);
            string kosul = "";

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMALANLAR.KOD], "=", p.kod));
            else
            {
                if (p.personelKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMALANLAR.PKOD], "=", p.personelKod));

                if ((int)p.kadroDurum > 0)
                {
                    if (p.kadroDurum == ENUMKadroDurumu.IPTALLERHARIC)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", TABGenel.KadroDurumuIptalHaric(TABLOAD + "." + ALANLAR[(int)ENUMALANLAR.KADRODURUMU]));
                    else if (p.kadroDurum == ENUMKadroDurumu.ONAYSIZLAR)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", TABGenel.KadroDurumuOnaysizlar(TABLOAD + "." + ALANLAR[(int)ENUMALANLAR.KADRODURUMU]));
                    else if (p.kadroDurum == ENUMKadroDurumu.AKTIFKAYIT)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", TABGenel.KadroDurumuAktifAciklarYenisiz(TABLOAD + "." + ALANLAR[(int)ENUMALANLAR.KADRODURUMU]));
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMALANLAR.KADRODURUMU], "=", (int)p.kadroDurum));
                }
                if (p.hizmetSebebiKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMALANLAR.HIZMETSEBEBIKOD], "=", p.hizmetSebebiKod));

                if (p.personelTCKimlikNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.TCKIMLIKNO], "=", p.personelTCKimlikNo));

                if (p.personelTip > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, ALANLAR[(int)ENUMALANLAR.PERSONELTUR], "=", p.personelTip));

                if (p.organizasyonKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.ORGANIZASYONKOD], "=", p.organizasyonKodu));

                if (!string.IsNullOrWhiteSpace(p.organizasyonKopruKodu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("O", "KOPRUKOD", "LIKE", p.organizasyonKopruKodu + "%"));

                if (!string.IsNullOrWhiteSpace(p.gitOrganizasyonKopruKodu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("O3", "KOPRUKOD", "LIKE", p.gitOrganizasyonKopruKodu + "%"));

                if (p.unvan > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.UNVANKOD], "=", p.unvan));

                if (p.sinifi > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABUnvan.TABLOAD, TABUnvan.ALANLAR[(int)TABUnvan.ENUMAlanlar.SINIFKOD], "=", p.sinifi));

                if (p.kadroDerece > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.KADRODERECE], "=", p.kadroDerece));

                if (!string.IsNullOrEmpty(p.aciklama))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMALANLAR.ACIKLAMA], "LIKE", "%" + p.aciklama + "%"));

                if (p.eskiHizmetTipiFlag == 1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.EskiHizmetFlag], "=", 1));
                }
                else if (p.eskiHizmetTipiFlag > 1)
                {
                    string alanAd = TABPersonelKadro.TABLOAD + "." + TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.EskiHizmetFlag];
                    kosul = VeriErisim.KosulEkle(kosul, "AND", string.Format("({0} IS NULL OR {0} = 0)", alanAd));
                }
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 2, " + ALANLAR[(int)ENUMALANLAR.AtamaTarihi] + " " + (tarihArtan ? "ASC" : "DESC");

            return veriErisim.DataReaderDondur(sql);
        }

        public static string ListeleKadroUstTablo(string ustTabloAlan, bool tarihArtan)
        {
            string sql = SorguAlanlar(false);
            sql += "WHERE IKYPERSONELKADRO.PKOD = " + ustTabloAlan;
            return sql;
        }

        public Sonuc SilKadro(PersonelKadro kadro)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.KOD], "=", kadro.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilPersonelKadro(int perKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMALANLAR.PKOD], "=", perKod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleEskiHizmetTipi()
        {
            string sql = "";

            sql = "SELECT [KOD], [HizmetAdi], [Toplam], [Aylik], [Emekli], [Izin], [Cb] FROM [InsanKaynaklari].[dbo].[IKYSOZLUKESKIHIZMET]";

            return veriErisim.DataReaderDondur(sql);
        }

        public static string HizmetleriGetirSql(string organizasyonKopruKod, string personelKod, string personelTur, TNSDateTime sonTarih, int siralamaListesi, int siraAlanNo, int aktiflik)//, int organizasyonKodu)
        {
            string sonTarihText = "NULL";
            if (sonTarih != null && !sonTarih.isNull)
            {
                sonTarihText = VeriErisim.DegerYap(sonTarih);
            }

            string alanlar = "k.PKOD, p.AD + ' ' + p.SOYAD, uSon.AD, O.ADI, p.KURUMABASLAMATARIHI, p.MEMURIYETEBASLAMATARIHI, kk.kadroTarihi, p.TCKIMLIKNO, p.KURUMSICILNO, p.GELDIGIKURUM, SONKADRO.KADRODERECE, SONKADRO.KAZANILMISEKGOSTERGE, SONKADRO.KAZANILMISHAKDERECE, SONKADRO.KAZANILMISHAKKADEME, SONKADRO.EMEKLIYEESASDERECE, SONKADRO.EMEKLIYEESASKADEME, uSon.SIRANO, o.KOPRUKOD, uGSon.AD";
            string sql = "SELECT " + alanlar + ","
                       + " SUM(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H1, " + sonTarihText + ", K.HizmetGunSayisi)) as hizmetGunu, "
                       + " sum(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H6, " + sonTarihText + ", K.HizmetGunSayisi)) as kidemGunu, "
                       + " sum(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H4, " + sonTarihText + ", K.HizmetGunSayisi)) as KHAGunu, "
                       + " sum(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H5, " + sonTarihText + ", K.HizmetGunSayisi)) as EKEAGunu, "
                       + " sum(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H2, " + sonTarihText + ", K.HizmetGunSayisi)) as izinHizmetGunu, "
                       + " sum(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H3, " + sonTarihText + ", K.HizmetGunSayisi)) as kurumHizmetGunu, "
                       + " sum(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H7, " + sonTarihText + ", K.HizmetGunSayisi)) as kadroHizmetGunu "
                       + " from IKYPERSONELKADRO K "
                       + " inner join IKYPERSONEL p on p.KOD = k.PKOD "
                       + " left outer join IKYPERSONELKADRO SONKADRO on SONKADRO.PKOD = p.KOD AND " + TABGenel.KadroDurumuAktifler("SONKADRO.KADRODURUMU")
                       + " left outer join IKYUNVAN u on u.KOD = k.UNVANKOD "
                       + " left outer join Kullanici.dbo.ORGBIRIM O on O.kod = SONKADRO.ORGANIZASYONKOD"
                       + " left outer join IKYUNVAN uSon on uSon.KOD = SONKADRO.UNVANKOD "
                        + " left outer join IKYUNVAN uGSon on uGSon.KOD = SONKADRO.GorunenUnvanKodu "
                       + " LEFT OUTER JOIN IKYPERSONELTIPI PT ON K.PERSONELTUR = PT.KOD "
                       + " left outer join (select min(AtamaTarihi) kadroTarihi, PKOD from IKYPERSONELKADRO INNER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD = IKYPERSONELKADRO.PERSONELTUR where IKYPERSONELTIPI.KADROLU = 1 and IKYPERSONELTIPI.KURUMDISI = 0 group by PKOD) kk on kk.PKOD =  p.KOD"
                       + " WHERE ";

            sql += TABGenel.KadroDurumuHesaplananlar("K.KADRODURUMU");
            sql += TABRapor.IN_KosulOl("AND", "p.KOD", personelKod, false);
            sql += TABRapor.IN_KosulOl("AND", "SONKADRO.PERSONELTUR", personelTur, false);
            if (aktiflik > -1)
                sql += " AND p.AKTIFMI = " + aktiflik;
            sql += " group by " + alanlar;
            string siralama = "";
            if (siralamaListesi == 1)
            {
                siralama = "o.KOPRUKOD, uSon.SIRANO ASC, p.AD + ' ' + p.SOYAD";
            }
            else if (siralamaListesi == 2)
            {
                siralama = "p.AD + ' ' + p.SOYAD, uSon.SIRANO ASC, o.KOPRUKOD";
            }
            else if (siralamaListesi == 3)
            {
                siralama = siraAlanNo.ToString() + " desc, uSon.SIRANO ASC, o.KOPRUKOD, p.AD + ' ' + p.SOYAD";
            }
            else
            {
                siralama = "uSon.SIRANO ASC, o.KOPRUKOD, p.AD + ' ' + p.SOYAD";
            }

            return sql + " order by " + siralama;
        }

        public static string GuncelBirimUnvanAdDondurSql(int pkod, string personelKod, string personelTur)
        {
            string sql = "select K.PKOD, IKYUNVAN.AD, O.ADI, p.KURUMABASLAMATARIHI, p.ad +' '+ p.soyad as isim, p.MEMURIYETEBASLAMATARIHI, "
                        + " kk.kadroTarihi, p.TCKIMLIKNO, p.KURUMSICILNO, p.GELDIGIKURUM, k.KADRODERECE, k.KAZANILMISEKGOSTERGE, "
                        + " k.KAZANILMISHAKDERECE, k.KAZANILMISHAKKADEME, k.EMEKLIYEESASDERECE, k.EMEKLIYEESASKADEME"
                        + " from IKYPERSONELKADRO  K"
                        + " inner join  IKYPERSONEL p on p.kod = K.PKOD"
                        + " left outer join IKYUNVAN on IKYUNVAN.KOD = UNVANKOD"
                        + " left outer join Kullanici.dbo.ORGBIRIM O on O.kod = K.ORGANIZASYONKOD"
                        + " left outer join (select min(AtamaTarihi) kadroTarihi, PKOD from IKYPERSONELKADRO INNER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD = IKYPERSONELKADRO.PERSONELTUR where IKYPERSONELTIPI.KADROLU = 1 group by PKOD) kk on kk.PKOD =  p.KOD"
                        + " where " + TABGenel.KadroDurumuAktifler("K.KADRODURUMU");
            if (pkod > 0)
                sql += " and K.PKOD = " + pkod;
            sql += TABRapor.IN_KosulOl("AND", "K.PKOD", personelKod, false);
            sql += TABRapor.IN_KosulOl("AND", "k.PERSONELTUR", personelTur, false);

            return sql;
        }

        //public IDataReader GuncelBirimUnvanAdDondur(int pkod)
        //{
        //    return veriErisim.DataReaderDondur(GuncelBirimUnvanAdDondurSql(pkod, "", ""));
        //}

        //public IDataReader MinAtamaTarihiniDondur(int tip, int pkod)
        //{
        //    string sql = "select min(AtamaTarihi) from IKYPERSONELKADRO WHERE pkod = " + pkod;
        //    if (tip == 1)
        //        sql += " AND HIZMETKURUMU = " + VeriErisim.DegerYap(S2900005(this.veriErisim));

        //    return veriErisim.DataReaderDondur(sql);
        //}

        public IDataReader UnvanaGoreHizmetGrupla(int pkod)
        {
            string sql = "select 1 as SIRA, IKYUNVAN.AD as unvan, Min(DATEPART(year, AtamaTarihi)) as basladigiYil, MAX(DATEPART(year, AYRILMATARIHI)) as bittigiYil, "
                       + " 0 as Gun from IKYPERSONELKADRO INNER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD=IKYPERSONELKADRO.PERSONELTUR"
                       + " left outer join IKYUNVAN on IKYUNVAN.KOD = UNVANKOD"
                       + " where HITAP = " + VeriErisim.DegerYap((int)ENUMHizmetTablolari.EmekliSandigi) + " and pkod = " + pkod + " group by IKYUNVAN.AD";
            sql += " union all "
                       + " select 2 as SIRA, 'SSK', 0 as basladigiYil, 0 as bittigiYil, "
                       + " sum(dbo.gunDondur(AtamaTarihi, AYRILMATARIHI, 1, null, HizmetGunSayisi)) as Gun"
                       + " from IKYPERSONELKADRO k INNER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD=k.PERSONELTUR"
                       + " left outer join IKYUNVAN on IKYUNVAN.KOD = UNVANKOD"
                       + " where HITAP = " + VeriErisim.DegerYap((int)ENUMHizmetTablolari.SSK) + " and k.pkod = '" + pkod + "' group by IKYUNVAN.AD";
            sql += " union all "
                       + " select 3 as SIRA, 'Bağkur', 0 as bittigiYil, 0 as bittigiYil, "
                       + " sum(dbo.gunDondur(AtamaTarihi, AYRILMATARIHI, 1, null, HizmetGunSayisi)) as Gun"
                       + " from IKYPERSONELKADRO k INNER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD=k.PERSONELTUR"
                       + " left outer join IKYUNVAN on IKYUNVAN.KOD = UNVANKOD"
                       + " where HITAP = " + VeriErisim.DegerYap((int)ENUMHizmetTablolari.Bagkur) + " and k.pkod = " + pkod + " group by IKYUNVAN.AD";
            sql += " ORDER BY 1,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetKadroAyar(KadroAyar ka)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOKADROAYAR + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.EMEKLITERFIAYI], ka.eTerfiAyi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.KAZANILMISTERFIAYI], ka.kTerfiAyi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.KIDEMTERFIAYI], ka.kidemTerfiAyi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.MAXIMUMDERECE], ka.maximumDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.EMEKLITERFITARIHI], ka.eAcilisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.KAZANILMISTERFITARIHI], ka.kAcilisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.KADRODERECEAKTIF], ka.aktif);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.PKOD], "=", ka.pKod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOKADROAYAR + " (" + VeriErisim.TumAlanlar("", ALANLARKADROAYAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(ka.pKod) + ",";
                sql += VeriErisim.DegerYap(ka.eTerfiAyi) + ",";
                sql += VeriErisim.DegerYap(ka.kTerfiAyi) + ",";
                sql += VeriErisim.DegerYap(ka.kidemTerfiAyi) + ",";
                sql += VeriErisim.DegerYap(ka.maximumDerece) + ",";
                sql += VeriErisim.DegerYap(ka.eAcilisTarihi) + ",";
                sql += VeriErisim.DegerYap(ka.kAcilisTarihi) + ",";
                sql += VeriErisim.DegerYap(ka.aktif) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc SilKadroAyar(int kod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOKADROAYAR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.PKOD], "=", kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleKadroAyar(KadroAyar ka)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOKADROAYAR, ALANLARKADROAYAR);
            sql += " FROM " + TABLOKADROAYAR;

            if (ka.pKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOKADROAYAR, ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.PKOD], "=", ka.pKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARKADROAYAR[(int)ENUMALANLARKADROAYARI.PKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        //public static int[] sozlukKodlar29 = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //private static int Dondur(VeriErisim veriErisim, string kod, int indis)
        //{
        //    if (sozlukKodlar29[indis] == 0)
        //    {
        //        VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", "");
        //        sozlukKodlar29[indis] = ve.TekKolonSQLInt("select kod from IKYSOZLUK where KAYITKOD = " + VeriErisim.DegerYap(kod));
        //        ve.VeriErisimKapat();
        //    }
        //    return sozlukKodlar29[indis];
        //}

        //public static int S2900001(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00001", 1);
        //}

        //public static int S2900002(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00002", 2);
        //}

        //public static int S2900003(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00003", 3);
        //}

        //public static int S2900004(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00004", 4);
        //}
        //public static int S2900005(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00005", 5);
        //}
        //public static int S2900006(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00006", 6);
        //}
        //public static int S2900007(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00007", 7);
        //}
        //public static int S2900008(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00008", 8);
        //}
        //public static int S2900010(VeriErisim veriErisim)
        //{
        //    return Dondur(veriErisim, "29.00010", 10);
        //}

        internal static string HizmetSuresiGetirUstTablo(string ustTabloKod, TNSDateTime sonTarih)
        {
            string sonTarihText = "NULL";
            if (sonTarih != null && !sonTarih.isNull)
            {
                sonTarihText = VeriErisim.DegerYap(sonTarih);
            }
            string baslangicTarihi = "K.AtamaTarihi";
            int yontem = TNS.IKY.Arac.HizmetBulmaYontemiBul();

            if (yontem == 2)
                baslangicTarihi = "case when K.PERSONELTUR>=10000 then K.AtamaTarihi else IKYP.KURUMABASLAMATARIHI end";

            string sql = "SELECT SUM(dbo.gunDondur(" + baslangicTarihi + ", K.AYRILMATARIHI, PT.H1, " + sonTarihText + ", K.HizmetGunSayisi)) as hizmetGunu, "
                       + " sum(dbo.gunDondur(" + baslangicTarihi + ", K.AYRILMATARIHI, PT.H6, " + sonTarihText + ", K.HizmetGunSayisi)) as kidemGunu, "
                       + " sum(dbo.gunDondur(" + baslangicTarihi + ", K.AYRILMATARIHI, PT.H4, " + sonTarihText + ", K.HizmetGunSayisi)) as KHAGunu, "
                       + " sum(dbo.gunDondur(" + baslangicTarihi + ", K.AYRILMATARIHI, PT.H5, " + sonTarihText + ", K.HizmetGunSayisi)) as EKEAGunu, "
                       + " sum(dbo.gunDondur(" + baslangicTarihi + ", K.AYRILMATARIHI, PT.H2, " + sonTarihText + ", K.HizmetGunSayisi)) as izinHizmetGunu, "
                       + " sum(dbo.gunDondur(" + baslangicTarihi + ", K.AYRILMATARIHI, PT.H3, " + sonTarihText + ", K.HizmetGunSayisi)) as kurumHizmetGunu, "
                       + " sum(dbo.gunDondur(" + baslangicTarihi + ", K.AYRILMATARIHI, PT.H7, " + sonTarihText + ", K.HizmetGunSayisi)) as kadroHizmetGunu "
                       + " from IKYPERSONELKADRO K "
                   + " LEFT OUTER JOIN IKYPERSONEL IKYP ON IKYP.KOD = K.PKOD "
                   + " LEFT OUTER JOIN IKYPERSONELTIPI PT ON K.PERSONELTUR = PT.KOD "
                   + " WHERE K.PKOD = " + ustTabloKod;
            if (yontem == 2)
                sql += " AND (" + TABGenel.KadroDurumuAktifAciklarYenisiz("K.KADRODURUMU") + " OR K.EskiHizmetFlag=1)";
            else
                sql += " AND " + TABGenel.KadroDurumuHesaplananlar("K.KADRODURUMU");
            return sql;
        }
    }
}

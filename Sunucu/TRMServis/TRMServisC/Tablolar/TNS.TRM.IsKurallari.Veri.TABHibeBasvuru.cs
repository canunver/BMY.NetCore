using System.Data;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeBasvuru : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeBasvuru(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBEBASVURU";
        public enum ENUMAlanlar
        {
            HIBEKOD = 0,
            TURKOD = 1,
            BOLUMKOD = 2,
            METIN = 3,
        }
        public static string[] ALANLAR = { "HIBEKOD", "TURKOD", "BOLUMKOD", "METIN" };

        public static string TABLOADSABLON = "TRMHIBEBASVURUSABLON";
        public enum ENUMAlanlarSablon
        {
            KOD = 0,
            TURKOD = 1,
            ETAPNO = 2,
            BOLUMKOD = 3,
            BOLUMAD = 4,
            ACIKLAMA = 5,
            SABLON = 6,
            TABLOGIRISSEKLI = 7,
            ALTYAPIA = 8,
            ALTYAPIB = 9

        }
        public static string[] ALANLARSABLON = { "KOD", "TURKOD", "ETAPNO", "BOLUMKOD", "BOLUMAD", "ACIKLAMA", "SABLON", "TABLOGIRISSEKLI", "ALTYAPIA", "ALTYAPIB" };

        public static string TABLOADONLINE = "TRMHIBEONLINEBASVURU";
        public enum ENUMAlanlarOnline
        {
            KOD = 0,
            KIMLIKNO = 1,
            ADI = 2,
            SOYADI = 3,
            BABAADI = 4,
            VERGIDAIRESI = 5,
            EPOSTAADRESI = 6,
            PAROLA = 7,
            GUVENLIKSORUSU = 8,
            GUVENLIKCEVABI = 9,
            TARIH = 10,
            IP = 11,
            AKTIF = 12,
            UYEKIMLIKNO = 13,
            UYEADI = 14,
            UYESOYADI = 15,
            UYEBABAADI = 16,
            DOGUMTARIH = 17,
        }
        public static string[] ALANLARONLINE = { "KOD", "KIMLIKNO", "ADI", "SOYADI", "BABAADI", "VERGIDAIRESI", "EPOSTAADRESI", "PAROLA", "GUVENLIKSORUSU", "GUVENLIKCEVABI", "TARIH", "IP", "AKTIF", "UYEKIMLIKNO", "UYEADI", "UYESOYADI", "UYEBABAADI", "DOGUMTARIH" };

        public Sonuc Kaydet(Basvuru bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.METIN], bilgi.metin);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", bilgi.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURKOD], "=", bilgi.turKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BOLUMKOD], "=", bilgi.bolumKod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.turKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.bolumKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.metin) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc BasvuruKopyasiAl(int kod)
        {
            string sql = "INSERT INTO TRMHIBEBASVURU SELECT HIBEKOD,TURKOD+10,BOLUMKOD,METIN FROM TRMHIBEBASVURU ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", kod));
            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetSablon(BasvuruSablon bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADSABLON + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.BOLUMKOD], bilgi.bolumKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.BOLUMAD], bilgi.bolumAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.SABLON], bilgi.sablon) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.ALTYAPIA], bilgi.altYapiA) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.ALTYAPIB], bilgi.altYapiB) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.TABLOGIRISSEKLI], bilgi.tabloGirisSekli);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOADSABLON + " (" + VeriErisim.TumAlanlar("", ALANLARSABLON) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.turKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.bolumKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.bolumAd) + ",";
                sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
                sql += VeriErisim.DegerYap(bilgi.sablon) + ",";
                sql += VeriErisim.DegerYap(bilgi.tabloGirisSekli) + ",";
                sql += VeriErisim.DegerYap(bilgi.altYapiA) + ",";
                sql += VeriErisim.DegerYap(bilgi.altYapiB) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc KaydetOnlineBasvuru(OnlineBasvuruKaydi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "INSERT INTO " + TABLOADONLINE + " (" + VeriErisim.TumAlanlar("", ALANLARONLINE) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kimlikNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.adi) + ",";
            sql += VeriErisim.DegerYap(bilgi.soyadi) + ",";
            sql += VeriErisim.DegerYap(bilgi.babaAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.vergiDairesi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ePostaAdresi) + ",";
            sql += VeriErisim.DegerYap(bilgi.parola) + ",";
            sql += VeriErisim.DegerYap(bilgi.guvenlikSorusu) + ",";
            sql += VeriErisim.DegerYap(bilgi.guvenlikSorusuCevap) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.DegerYap(bilgi.ip) + ",";
            sql += VeriErisim.DegerYap(bilgi.aktif) + ",";
            sql += VeriErisim.DegerYap(bilgi.uyeKimlikNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.uyeAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.uyeSoyadi) + ",";
            sql += VeriErisim.DegerYap(bilgi.uyeBabaAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.dogumTarih) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc GuncelleOnlineBasvuru(OnlineBasvuruKaydi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "UPDATE " + TABLOADONLINE + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.UYEKIMLIKNO], bilgi.uyeKimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.UYEADI], bilgi.uyeAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.UYESOYADI], bilgi.uyeSoyadi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.UYEBABAADI], bilgi.uyeBabaAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.EPOSTAADRESI], bilgi.ePostaAdresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.PAROLA], bilgi.parola) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.GUVENLIKSORUSU], bilgi.guvenlikSorusu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.GUVENLIKCEVABI], bilgi.guvenlikSorusuCevap) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.TARIH], new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.IP], bilgi.ip);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc GuncelleOnlineBasvuruAktivasyon(string kod, int aktif)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "UPDATE " + TABLOADONLINE + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.AKTIF], aktif);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.KOD], "=", kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc GuncelleOnlineBasvuruParola(string kod, string parola)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "UPDATE " + TABLOADONLINE + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.PAROLA], parola);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.KOD], "=", kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc Sil(Basvuru bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", bilgi.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURKOD], "=", bilgi.turKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BOLUMKOD], "=", bilgi.bolumKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilSablon(BasvuruSablon bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADSABLON;
            if (bilgi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.KOD], "=", bilgi.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.TURKOD], "=", bilgi.turKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.ETAPNO], "=", bilgi.etapNo));
            }

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilOnlineBasvuru(OnlineBasvuruKaydi bilgi)
        {
            string sql = "DELETE FROM " + TABLOADONLINE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleOnlineBasvuruParola(OnlineBasvuruKaydi bilgi)
        {
            string sql = "UPDATE " + TABLOADONLINE + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.EPOSTAADRESI], bilgi.ePostaAdresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.GUVENLIKSORUSU], bilgi.guvenlikSorusu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.GUVENLIKCEVABI], bilgi.guvenlikSorusuCevap);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(Basvuru bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", bilgi.hibeKod));

            if (bilgi.turKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURKOD], "=", bilgi.turKod));

            if (bilgi.bolumKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BOLUMKOD], "=", bilgi.bolumKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BOLUMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleSablon(BasvuruSablon bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSABLON) + " FROM " + TABLOADSABLON;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.KOD], "=", bilgi.kod));

            if (bilgi.turKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.TURKOD], "=", bilgi.turKod));

            if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.ETAPNO], "=", bilgi.etapNo));

            if (bilgi.bolumKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.BOLUMKOD], "=", bilgi.bolumKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARSABLON[(int)ENUMAlanlarSablon.BOLUMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleOnlineBasvuru(OnlineBasvuruKaydi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARONLINE) + " FROM " + TABLOADONLINE;

            if (bilgi.kimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.KIMLIKNO], "=", bilgi.kimlikNo));

            if (bilgi.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.KOD], "=", bilgi.kod));

            //if (bilgi.turKod > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.TURKOD], "=", bilgi.turKod));

            //if (bilgi.etapNo > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.ETAPNO], "=", bilgi.etapNo));

            //if (bilgi.sonGonderilenMi > 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.SONGONDERILENMI], "=", bilgi.sonGonderilenMi));

            if (bilgi.ePostaAdresi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARONLINE[(int)ENUMAlanlarOnline.EPOSTAADRESI], "=", bilgi.ePostaAdresi));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARONLINE[(int)ENUMAlanlarOnline.TARIH];

            return veriErisim.DataReaderDondur(sql);
        }

        public int EnBuyukKodSablon()
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLARSABLON[(int)ENUMAlanlarSablon.KOD] + ")";
            sql += " FROM " + TABLOADSABLON;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }
    }
}

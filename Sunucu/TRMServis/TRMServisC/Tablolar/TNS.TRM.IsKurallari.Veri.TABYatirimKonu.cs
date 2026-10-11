using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABYatirimKonu : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYatirimKonu(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMYATIRIMKONU";
        /// <summary>
        /// Malzeme grubu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 1,
            KISAADI = 2,
            AKTIF = 3,
            NITELIK = 4
        }
        /// <summary>
        /// Malzeme grubu tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "ADI", "KISAADI", "AKTIF", "NITELIK" };

        public static string TABLOADKONUIL = "TRMHIBEYATIRIMKONUIL";
        public enum ENUMAlanlarKonuIl
        {
            ETAPNO = 0,
            ILKOD = 1,
            KONUKOD = 2,
        }
        public static string[] ALANLARKONUIL = { "ETAPNO", "ILKOD", "KONUKOD" };

        public static string TABLOADALTKONUIL = "TRMHIBEYATIRIMKONUALTIL";
        public enum ENUMAlanlarAltKonuIl
        {
            ETAPNO = 0,
            ILKOD = 1,
            ALTKONUKOD = 2,
        }
        public static string[] ALANLARALTKONUIL = { "ETAPNO", "ILKOD", "ALTKONUKOD" };

        public static string TABLOADKONUUSTSINIR = "TRMHIBEYATIRIMKONUUSTSINIR";
        public enum ENUMAlanlarKonuUstSinir
        {
            ETAPNO = 0,
            KONUKOD = 1,
            NITELIKKOD = 2,
            GERCEK = 3,
            TUZEL = 4
        }
        public static string[] ALANLARKONUUSTSINIR = { "ETAPNO", "KONUKOD", "NITELIKKOD", "GERCEK", "TUZEL" };

        public static string TABLOADALTKONUDETAY = "TRMYATIRIMALTKONUDETAY";
        public enum ENUMAlanlarAltKonuDetay
        {
            KOD = 0,
            ALTKONUKOD = 1,
            NUMARA = 2,
            ADI = 3,
            AKTIF = 4
        }
        public static string[] ALANLARALTKONUDETAY = { "KOD", "ALTKONUKOD", "NUMARA", "ADI", "AKTIF" };

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(YatirimKonu bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISAADI], bilgi.kisaAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], bilgi.aktif) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NITELIK], bilgi.nitelik);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.kisaAd) + ",";
                sql += VeriErisim.DegerYap(bilgi.aktif) + ",";
                sql += VeriErisim.DegerYap(bilgi.nitelik) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(YatirimKonu bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Malzeme grubu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(YatirimKonu bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrEmpty(bilgi.kod))
            {
                string islec = " LIKE ";
                if (bilgi.not)
                    islec = " NOT LIKE ";
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], islec, bilgi.kod));
            }

            if (!string.IsNullOrEmpty(bilgi.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ADI], " LIKE ", "%" + bilgi.ad + "%"));

            if (bilgi.aktif > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], "=", bilgi.aktif));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public string KonuKisaAdBul(string kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KISAADI] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return veriErisim.TekKolonSQLStr(sql);
        }

        public string AltKonuDetayNoBul(string kod)
        {
            string sql = "SELECT NUMARA FROM TRMYATIRIMALTKONUDETAY";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "=", kod));
            return veriErisim.TekKolonSQLStr(sql);
        }

        public string KonuNitelikBul(string kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.NITELIK] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return veriErisim.TekKolonSQLStr(sql);
        }

        public Sonuc KaydetKonuIl(YatirimKonuIl bilgi)
        {
            string sql = "INSERT INTO " + TABLOADKONUIL + " (" + VeriErisim.TumAlanlar("", ALANLARKONUIL) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.konuKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilKonuIl(int etapNo, string ilKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADKONUIL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKONUIL[(int)ENUMAlanlarKonuIl.ETAPNO], "=", etapNo));

            if (ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKONUIL[(int)ENUMAlanlarKonuIl.ILKOD], "=", ilKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleKonuIl(int etapNo, string ilKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKONUIL) + ", TRMYATIRIMKONU.KISAADI FROM " + TABLOADKONUIL;
            sql += " INNER JOIN TRMYATIRIMKONU ON TRMHIBEYATIRIMKONUIL.KONUKOD = TRMYATIRIMKONU.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKONUIL[(int)ENUMAlanlarKonuIl.ETAPNO], "=", etapNo));
            if (ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKONUIL[(int)ENUMAlanlarKonuIl.ILKOD], "=", ilKod));

            sql += " ORDER BY " + ALANLARKONUIL[(int)ENUMAlanlarKonuIl.ILKOD] + "," + ALANLARKONUIL[(int)ENUMAlanlarKonuIl.KONUKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc AktarKonuIl(int eskiEtap, int yeniEtap)
        {
            string sql = "INSERT INTO TRMHIBEYATIRIMKONUIL (ETAPNO, ILKOD, KONUKOD)";
            sql += " SELECT " + yeniEtap + ", ILKOD, KONUKOD";
            sql += " FROM TRMHIBEYATIRIMKONUIL AS TRMHIBEYATIRIMKONUIL_1";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKONUIL[(int)ENUMAlanlarKonuIl.ETAPNO], "=", eskiEtap));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KaydetAltKonuIl(YatirimAltKonuIl bilgi)
        {
            string sql = "INSERT INTO " + TABLOADALTKONUIL + " (" + VeriErisim.TumAlanlar("", ALANLARALTKONUIL) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.altKonuKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilAltKonuIl(int etapNo, string ilKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADALTKONUIL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALTKONUIL[(int)ENUMAlanlarAltKonuIl.ETAPNO], "=", etapNo));
            if (ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARALTKONUIL[(int)ENUMAlanlarAltKonuIl.ILKOD], "=", ilKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc AktarAltKonuIl(int eskiEtap, int yeniEtap)
        {
            string sql = "INSERT INTO TRMHIBEYATIRIMKONUALTIL (ETAPNO, ILKOD, ALTKONUKOD)";
            sql += " SELECT " + yeniEtap + ", ILKOD, ALTKONUKOD";
            sql += " FROM TRMHIBEYATIRIMKONUALTIL AS TRMHIBEYATIRIMKONUALTIL_1";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALTKONUIL[(int)ENUMAlanlarAltKonuIl.ETAPNO], "=", eskiEtap));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleAltKonuIl(int etapNo, string ilKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARALTKONUIL) + ", TRMYATIRIMKONU.ADI FROM " + TABLOADALTKONUIL;
            sql += " INNER JOIN TRMYATIRIMKONU ON TRMHIBEYATIRIMKONUALTIL.ALTKONUKOD = TRMYATIRIMKONU.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALTKONUIL[(int)ENUMAlanlarAltKonuIl.ETAPNO], "=", etapNo));
            if (ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARALTKONUIL[(int)ENUMAlanlarAltKonuIl.ILKOD], "=", ilKod));

            sql += " ORDER BY " + ALANLARALTKONUIL[(int)ENUMAlanlarAltKonuIl.ILKOD] + "," + ALANLARALTKONUIL[(int)ENUMAlanlarAltKonuIl.ALTKONUKOD];

            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc KaydetKonuUstSinir(YatirimKonuUstSinir bilgi)
        {
            string sql = "INSERT INTO " + TABLOADKONUUSTSINIR + " (" + VeriErisim.TumAlanlar("", ALANLARKONUUSTSINIR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.konuKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.nitelikKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.gercek) + ",";
            sql += VeriErisim.DegerYap(bilgi.tuzel) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilKonuUstSinir(int etapNo, int nitelikKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADKONUUSTSINIR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKONUUSTSINIR[(int)ENUMAlanlarKonuUstSinir.ETAPNO], "=", etapNo));
            if (nitelikKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKONUUSTSINIR[(int)ENUMAlanlarKonuUstSinir.NITELIKKOD], "=", nitelikKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleKonuUstSinir(int etapNo, int nitelik, string konuKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKONUUSTSINIR) + ", TRMYATIRIMKONU.ADI FROM " + TABLOADKONUUSTSINIR;
            sql += " LEFT JOIN TRMYATIRIMKONU ON TRMHIBEYATIRIMKONUUSTSINIR.KONUKOD = TRMYATIRIMKONU.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKONUUSTSINIR[(int)ENUMAlanlarKonuUstSinir.ETAPNO], "=", etapNo));
            if (nitelik > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKONUUSTSINIR[(int)ENUMAlanlarKonuUstSinir.NITELIKKOD], "=", nitelik));
            if (konuKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKONUUSTSINIR[(int)ENUMAlanlarKonuUstSinir.KONUKOD], "=", konuKod));

            sql += " ORDER BY " + ALANLARKONUUSTSINIR[(int)ENUMAlanlarKonuUstSinir.KONUKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc AktarKonuUstSinir(int eskiEtap, int yeniEtap)
        {
            string sql = "INSERT INTO TRMHIBEYATIRIMKONUUSTSINIR (ETAPNO, KONUKOD, NITELIKKOD, GERCEK, TUZEL)";
            sql += " SELECT " + yeniEtap + ", KONUKOD, NITELIKKOD, GERCEK, TUZEL";
            sql += " FROM TRMHIBEYATIRIMKONUUSTSINIR AS TRMHIBEYATIRIMKONUUSTSINIR_1";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKONUUSTSINIR[(int)ENUMAlanlarKonuUstSinir.ETAPNO], "=", eskiEtap));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KaydetAltKonuDetay(YatirimAltKonuDetay bilgi)
        {
            string sql = "INSERT INTO " + TABLOADALTKONUDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARALTKONUDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.altKonuKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.numara) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.aktif) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleAltKonuDetay(YatirimAltKonuDetay bilgi)
        {
            string sql = "UPDATE " + TABLOADALTKONUDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.ADI], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.NUMARA], bilgi.numara) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.AKTIF], bilgi.aktif) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.ALTKONUKOD], bilgi.altKonuKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilAltKonuDetay(string kod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADALTKONUDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleAltKonuDetay(YatirimAltKonuDetay kriter)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADALTKONUDETAY, ALANLARALTKONUDETAY) + ", TRMYATIRIMKONU.ADI FROM " + TABLOADALTKONUDETAY;
            sql += " INNER JOIN TRMYATIRIMKONU ON TRMYATIRIMALTKONUDETAY.ALTKONUKOD = TRMYATIRIMKONU.KOD";

            if (kriter.altKonuKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADALTKONUDETAY, ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.ALTKONUKOD], "=", kriter.altKonuKod));
            if (kriter.ad != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADALTKONUDETAY, ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.ADI], "LIKE", "%" + kriter.ad + "%"));
            if (kriter.aktif > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADALTKONUDETAY, ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.AKTIF], "=", kriter.aktif));

            sql += " ORDER BY " + ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.ALTKONUKOD] + "," + ALANLARALTKONUDETAY[(int)ENUMAlanlarAltKonuDetay.NUMARA];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// İş/görev bilgilerini tutan PEBISGOREV veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABIsGorev : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIsGorev(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBISGOREV";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// ISGOREVKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ISGOREVKOD = 2,

            /// <summary>
            /// ALTFAALIYETKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTFAALIYETKOD = 3,

            /// <summary>
            /// ISGOREVAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ISGOREVAD = 4,

            /// <summary>
            /// ACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 5,

                /// <summary>
            /// HEDEF alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            HEDEF = 6,

            /// <summary>
            /// TEKRARSIKLIGI alanının TEKRARSIKLIGI dizisindeki sıra numarası (indisi)
            /// </summary>
            TEKRARSIKLIGI = 7
        }

        /// <summary>
        /// İş/görevlerin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "ISGOREVKOD",
                                   "ALTFAALIYETKOD", "ISGOREVAD", "ACIKLAMA", "HEDEF","TEKRARSIKLIGI" };








        static string TABLOADTARIHCE = "PEBISGOREVTARIHCE";
        public enum ENUMAlanlarTarihce : int
        {
            KOD = 0,
            KULLANICI = 1,
            TARIH = 2,
            DEGISIKLIK = 3,
            YIL=4,
            KURKOD=5
        }
        static string[] ALANLARTARIHCE = { "KOD", "KULLANICI", "TARIH", "DEGISIKLIK","YIL" ,"KURKOD"};



        /// <summary>
        /// İş/görev bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// İş/görevin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(IsGorev ig)
        {
            ig.isGorevKod = IsGorevKodAl(ig);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ig.yil) + ",";
            sql += VeriErisim.DegerYap(ig.orgKod) + ",";
            sql += VeriErisim.DegerYap(ig.isGorevKod) + ",";
            sql += VeriErisim.DegerYap(ig.af.afKod) + ",";
            sql += VeriErisim.DegerYap(ig.isGorevAd) + ",";
            sql += VeriErisim.DegerYap(ig.aciklama) + ",";
            sql += VeriErisim.DegerYap(ig.hedef) + ",";
            sql += VeriErisim.DegerYap(ig.tekrarSikligi) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = ig.isGorevKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan iş/görev bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(IsGorev ig)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD], ig.af.afKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISGOREVAD], ig.isGorevAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], ig.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDEF], ig.hedef) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKRARSIKLIGI], ig.tekrarSikligi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", ig.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", ig.orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "=", ig.isGorevKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve iş/görevleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <param name="tcKimlikNo">İş/görevleri istenen sorumlu personelin T.C. kimlik numarası</param>
        /// <returns>Verilen kriterlere uygun olan iş/görevleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(IsGorev ig, string tcKimlikNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", PEBALTFAALIYET.ALTFAALIYETAD";
            sql += ", PEBFAALIYETPROJE.FPKOD";
            sql += ", PEBFAALIYETPROJE.FPAD";
            sql += ", PEBSABITMALIYET.SMKOD";
            sql += ", PEBSABITMALIYET.SMAD";
            sql += ", PEBFAALIYETPROJE.KURKOD";
            sql += ", PEBSABITMALIYET.SMKURKOD";
            sql += " FROM " + TABLOAD;

            sql += " INNER JOIN PEBALTFAALIYET";
            sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = PEBALTFAALIYET.YIL";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + " = PEBALTFAALIYET.KURKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD] + " = PEBALTFAALIYET.ALTFAALIYETKOD";

            sql += " LEFT OUTER JOIN PEBFAALIYETPROJE";
            sql += " ON PEBALTFAALIYET.YIL = PEBFAALIYETPROJE.YIL";
            //sql += " AND PEBALTFAALIYET.KURKOD = PEBFAALIYETPROJE.KURKOD";
            sql += " AND PEBALTFAALIYET.FPKOD = PEBFAALIYETPROJE.FPKOD";

            sql += " LEFT OUTER JOIN PEBSABITMALIYET";
            sql += " ON PEBALTFAALIYET.YIL = PEBSABITMALIYET.YIL";
            //sql += " AND PEBALTFAALIYET.KURKOD = PEBSABITMALIYET.SMKURKOD";
            sql += " AND PEBALTFAALIYET.SMKOD = PEBSABITMALIYET.SMKOD";

            //sql += " LEFT OUTER JOIN PEBDAIRE";
            //sql += " ON PEBALTFAALIYET.YIL = PEBDAIRE.YIL";
            //sql += " AND PEBALTFAALIYET.KURKOD = PEBDAIRE.KURKOD";
            //sql += " AND PEBALTFAALIYET.DAIREKOD = PEBDAIRE.DAIREKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ig.yil));
            if (ig.orgKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", ig.orgKod));
            if (ig.isGorevKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "=", ig.isGorevKod));
            if (ig.af.afKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD], "=", ig.af.afKod));
            if (!string.IsNullOrEmpty(ig.isGorevAd))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISGOREVAD], "LIKE", "%" + ig.isGorevAd + "%"));
            if (!string.IsNullOrEmpty(ig.aciklama))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + ig.aciklama + "%"));
            if (!string.IsNullOrEmpty(tcKimlikNo))
            {
                string icSQL = "SELECT 1 FROM PEBISGOREVSORUMLU";
                icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = PEBISGOREVSORUMLU.YIL";
                icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + " = PEBISGOREVSORUMLU.KURKOD";
                icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISGOREVKOD] + " = PEBISGOREVSORUMLU.ISGOREVKOD";
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("PEBISGOREVSORUMLU", "TCKIMLIKNO", "=", tcKimlikNo));

                sql = VeriErisim.KosulEkle(sql, "AND EXISTS", "(" + icSQL + ")");
            }

            if (ig.af.fp.fpKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFAALIYETPROJE", "FPKOD", "=", ig.af.fp.fpKod));

            if (ig.af.sm.smKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBSABITMALIYET", "SMKOD", "=", ig.af.sm.smKod));

            //if (ig.af.daire.daireKod > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBDAIRE", "DAIREKOD", "=", ig.af.daire.daireKod));

            if (ig.zamanlamalar.Count > 0)
            {
                IsGorevZamanlama z = (IsGorevZamanlama)ig.zamanlamalar[0];
                if (!(z.basTarih.isNull && z.basTarih2.isNull &&
                    z.bitTarih.isNull && z.bitTarih2.isNull))
                {
                    string icSQL = "SELECT 1 FROM PEBISGOREVZAMANLAMA";
                    icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = PEBISGOREVZAMANLAMA.YIL";
                    icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + " = PEBISGOREVZAMANLAMA.KURKOD";
                    icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISGOREVKOD] + " = PEBISGOREVZAMANLAMA.ISGOREVKOD";

                    if (!z.basTarih.isNull)
                        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("PEBISGOREVZAMANLAMA", "BASTARIH", ">=", z.basTarih));

                    if (!z.basTarih2.isNull)
                        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("PEBISGOREVZAMANLAMA", "BASTARIH", "<=", z.basTarih2));

                    if (!z.bitTarih.isNull)
                        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("PEBISGOREVZAMANLAMA", "BITTARIH", ">=", z.bitTarih));

                    if (!z.bitTarih2.isNull)
                        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("PEBISGOREVZAMANLAMA", "BITTARIH", "<=", z.bitTarih2));

                    sql = VeriErisim.KosulEkle(sql, "AND EXISTS", "(" + icSQL + ")");
                }
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen iş/görevi kullanarak,
        /// ilgili iş/görevi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(IsGorev ig)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", ig.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", ig.orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "=", ig.isGorevKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen iş/görevi kullanarak,
        /// iş/görevin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool IsGorevVarMi(IsGorev ig)
        {
            return KayitVarmi(TABLOAD, ig);
        }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen iş/görev var mı kontrolü yapan yordam
        /// </summary>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, IsGorev ig)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { ig.orgKod.ToString() };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.ISGOREVKOD] };
            int[] intDeger = { ig.yil, ig.isGorevKod };
            return TABGenel.KayitVarmi(veriErisim, tablo, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen iş/görev nesnesinin yıl ve birim özellikleri
        /// bazında yeni iş/görev programatik kodu döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Yeni iş/görev programatik kodu döndürülür.</returns>
        private int IsGorevKodAl(IsGorev ig)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { ig.orgKod.ToString() };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { ig.yil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], strAlan, strDeger, intAlan, intDeger);
        }

        public bool IsGorevKullanilmisMi(IsGorev ig)
        {
            if (KayitVarmi("PEBISGOREVGOSTERGE", ig)
             || KayitVarmi("PEBISGOREVSORUMLU", ig)
             || KayitVarmi("PEBISGOREVZAMANLAMA", ig))
                return true;
            else
                return false;
        }

        //İş Görevde tarihçe için kayıtlı olan değerleri getirmek için
        public IDataReader ListeleIsGorev(IsGorev ig)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "=", ig.isGorevKod));
            sql =VeriErisim.KosulEkle(sql," and ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], " = ", ig.yil));
            sql = VeriErisim.KosulEkle(sql, " and ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], " = ", ig.orgKod));
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc KaydetTarihce(IsGorevTarihce p)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.kullanici) + ",";
            sql += VeriErisim.DegerYap(p.tarih) + ",";
            sql += VeriErisim.DegerYap(p.degisiklik) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.kurKod) + ")";

            return KomutCalistir(sql, 1);
        }
    }
}
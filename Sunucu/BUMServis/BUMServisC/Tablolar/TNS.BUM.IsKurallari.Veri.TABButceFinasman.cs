using System.Data;
using TNS.Veri;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe Finansman bilgilerini tutan ABSBUTCEFINANSMAN veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABButceFinansman : TABButce
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABButceFinansman(VeriErisim veriErisim)
            : base(veriErisim)
        {

        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ABSBUTCEFINANSMAN";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURKOD", "EKOKOD", "KANUN", "KBO", "EKLENEN", "DUSULEN", "SERBEST", "EKLENEN_TASLAK", "DUSULEN_TASLAK", "SERBEST_TASLAK" };

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Kurumsal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 2,
            /// <summary>
            /// Kanun alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KANUN = 3,
            /// <summary>
            /// KBÖ alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KBO = 4,
            /// <summary>
            /// Eklenen alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKLENEN = 5,
            /// <summary>
            /// Düşülen alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DUSULEN = 6,
            /// <summary>
            /// Serbest alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SERBEST = 7,
            /// <summary>
            /// Eklenen taslak alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKLENEN_TASLAK = 8,
            /// <summary>
            /// Düşülen taslak alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DUSULEN_TASLAK = 9,
            /// <summary>
            /// Serbest taslak alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SERBEST_TASLAK = 10
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// Kesintili Başlangıç Ödeneği(KBÖ) toplamını YSOTBUL rutinini çağırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kesintili Başlangıç Ödeneğini tutan Double türünde nesne döndürür</returns>
        public override double KBOBul(int yil, string[] kistasKodlar)
        {

            return YSOTBul(yil, kistasKodlar, true);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kodlar bilgisini kıstas olarak kullanarak,
        /// verilen kıstasa uygun Yıl Sonu Ödenek Toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kboBul">Kesintili Başlangıç Ödeneği ile ilgili Sql cümlesinin değişip değişmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Yıl Sonu Ödenek Toplamını Tutan Double türünde nesne döndürür</returns>
        public override double YSOTBul(int yil, string[] kistasKodlar, bool kboBul)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kistasKodlar);
            string sql;
            if (kboBul)
                sql = "SELECT SUM(KBO) from " + TABLOAD + " where " + kosul;
            else
                sql = "SELECT (SUM(KBO)+SUM(EKLENEN)-SUM(DUSULEN)) AS YSOT from " + TABLOAD + " where " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Serbest toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Yıl Sonu Ödenek Toplamını Tutan Double türünde nesne döndürür</returns>
        public override double SerbestBul(int yil, string[] kistasKodlar)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kistasKodlar);
            string sql = "SELECT SUM(SERBEST) from " + TABLOAD + " where " + kosul;
            return veriErisim.TekKolonSQLDbl(sql);
        }


        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisine göre,
        /// verilen kıstaslara uygun Kanun, KBÖ, Eklenek, Düşülen, Serbest ve Taslakları döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kanun">Kanun bilgisini tutan, Double türünde nesne</param>
        /// <param name="kbo">Kesintili Başlangıç Ödeneği bilgisini tutan, Double türünde nesne</param>
        /// <param name="eklenen">Eklenen bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen">Düşülen bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbest">Serbest bilgisini tutan, Double türünde nesne</param>
        /// <param name="eklenen_taslak">Eklenen Taslak bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen_taslak">Düşülen Taslak bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbest_taslak">Serbest Taslak bilgisini tutan, Double türünde nesne</param>
        public override void BilgiBul(int yil, string[] kistasKodlar, out double kanun, out double kbo,
                                                                    out double eklenen, out double dusulen, out double serbest,
                                                                    out double eklenen_taslak, out double dusulen_taslak, out double serbest_taslak)
        {
            kanun = 0;
            kbo = 0;
            eklenen = 0;
            dusulen = 0;
            serbest = 0;
            eklenen_taslak = 0;
            dusulen_taslak = 0;
            serbest_taslak = 0;

            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kistasKodlar);
            string sql = "SELECT " + GenelKosul.Islec("", "SUM", ALANLAR[(int)ENUMAlanlar.KANUN]);
            sql += GenelKosul.Islec(",", "SUM", ALANLAR[(int)ENUMAlanlar.KBO]);
            sql += GenelKosul.Islec(",", "SUM", ALANLAR[(int)ENUMAlanlar.EKLENEN]);
            sql += GenelKosul.Islec(",", "SUM", ALANLAR[(int)ENUMAlanlar.DUSULEN]);
            sql += GenelKosul.Islec(",", "SUM", ALANLAR[(int)ENUMAlanlar.SERBEST]);
            sql += GenelKosul.Islec(",", "SUM", ALANLAR[(int)ENUMAlanlar.EKLENEN_TASLAK]);
            sql += GenelKosul.Islec(",", "SUM", ALANLAR[(int)ENUMAlanlar.DUSULEN_TASLAK]);
            sql += GenelKosul.Islec(",", "SUM", ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK]);
            sql += " FROM " + TABLOAD + " where " + kosul;
            IDataReader dr = veriErisim.DataReaderDondur(sql);
            if (dr != null)
            {
                if (dr.Read())
                {
                    kanun = VeriErisim.DegerAl(dr, 0, (double)0);
                    kbo = VeriErisim.DegerAl(dr, 1, (double)0);
                    eklenen = VeriErisim.DegerAl(dr, 2, (double)0);
                    dusulen = VeriErisim.DegerAl(dr, 3, (double)0);
                    serbest = VeriErisim.DegerAl(dr, 4, (double)0);
                    eklenen_taslak = VeriErisim.DegerAl(dr, 5, (double)0);
                    dusulen_taslak = VeriErisim.DegerAl(dr, 6, (double)0);
                    serbest_taslak = VeriErisim.DegerAl(dr, 7, (double)0);
                }
                dr.Close();
            }
        }

        /// <summary>
        /// Parametre olarak verien Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Bütçe Tertip bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kbolu">KBO parametresine göre ilgili koşulun eklenip eklenmeyeceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Bütçe Tertip bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public override IDataReader ButceTertipListele(int yil, string[] kodlar, bool kbolu)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);
            if (kbolu)
                kosul = VeriErisim.KosulEkle(kosul, " and", VeriErisim.KosulOl("", "KBO", "<>", 0));
            string sql = "SELECT  " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.KBO] + "), SUM(" + ALANLAR[(int)ENUMAlanlar.SERBEST] + ")";
            sql = sql + " FROM " + TABLOAD;
            sql = sql + " WHERE " + kosul;
            sql = sql + " GROUP BY " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            sql = sql + " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// veri tabanından eklenen, düşülen ve serbest miktarlarını güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="eklenen">Eklenen bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen">Düşülen bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbest">Serbest bilgisini tutan, Double türünde nesne</param>
        /// <param name="taslakMi">Taslak mı parametresine göre Eklenen, Düşülen, Serbest değerlerinin mi yoksa taslaklarının mı güncelleneceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc TaslakDegistir(int yil, string[] kodlar, double eklenen, double dusulen, double serbest, bool taslakMi)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);
            string sql = "UPDATE " + TABLOAD + " SET ";
            int a1, a2, a3;
            if (taslakMi)
            {
                a1 = (int)ENUMAlanlar.EKLENEN_TASLAK;
                a2 = (int)ENUMAlanlar.DUSULEN_TASLAK;
                a3 = (int)ENUMAlanlar.SERBEST_TASLAK;
            }
            else
            {
                a1 = (int)ENUMAlanlar.EKLENEN;
                a2 = (int)ENUMAlanlar.DUSULEN;
                a3 = (int)ENUMAlanlar.SERBEST;
            }

            sql += "  " + ALANLAR[a1] + " = " + ALANLAR[a1] + " + " + VeriErisim.DegerYap(eklenen);
            sql += ", " + ALANLAR[a2] + " = " + ALANLAR[a2] + " + " + VeriErisim.DegerYap(dusulen);
            sql += ", " + ALANLAR[a3] + " = " + ALANLAR[a3] + " + " + VeriErisim.DegerYap(serbest);
            sql += " WHERE " + kosul;
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// Eklenen, Düşülen, Serbest ve Taslaklar miktarlarını veri tabanında güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="eklenen">Eklenen bilgisini tutan, Double türünde nesne</param>
        /// <param name="dusulen">Düşülen bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbest">Serbest bilgisini tutan, Double türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc TaslaktanOnaya(int yil, string[] kodlar, double eklenen, double dusulen, double serbest)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += "  " + ALANLAR[(int)ENUMAlanlar.EKLENEN] + " = " + ALANLAR[(int)ENUMAlanlar.EKLENEN] + " + " + VeriErisim.DegerYap(eklenen);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.DUSULEN] + " = " + ALANLAR[(int)ENUMAlanlar.DUSULEN] + " + " + VeriErisim.DegerYap(dusulen);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.SERBEST] + " = " + ALANLAR[(int)ENUMAlanlar.SERBEST] + " + " + VeriErisim.DegerYap(serbest);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.EKLENEN_TASLAK] + " = " + ALANLAR[(int)ENUMAlanlar.EKLENEN_TASLAK] + " - " + VeriErisim.DegerYap(eklenen);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.DUSULEN_TASLAK] + " = " + ALANLAR[(int)ENUMAlanlar.DUSULEN_TASLAK] + " - " + VeriErisim.DegerYap(dusulen);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK] + " = " + ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK] + " - " + VeriErisim.DegerYap(serbest);
            sql += " WHERE " + kosul;
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// Serbest miktarları veri tabanından güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="serbest">Serbest bilgisini tutan, Double türünde nesne</param>
        /// <param name="serbestTaslak">Serbest Taslak bilgisini tutan, Double türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc SerbestleriYaz(int yil, string[] kodlar, double serbest, double serbestTaslak)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += "  " + VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SERBEST], serbest);
            sql += ", " + VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK], serbestTaslak);
            sql += " WHERE " + kosul;
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen AHP Serbest Ay, Yıl ve Kodlar bilgisini kullanarak
        /// Serbest miktarını veri tabanında güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ahpSerbestAy">AHP Serbest Ay bilgisini tutan, Int türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasKodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc SerbestleriHesapla(int ahpSerbestAy, int yil, string[] kistasKodlar)
        {
            string sql;
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kistasKodlar);

            if (ahpSerbestAy == 0)
            {
                sql = "update " + TABLOAD + " set ";
                sql += ALANLAR[(int)ENUMAlanlar.SERBEST] + "= IsNull(";
                sql += "      (select SUM(" + "ODENEK" + ") from ISLGELIR";
                sql += "      where ISLGELIR.YIL=" + TABLOAD + ".YIL";
                sql += "      and ISLGELIR.KURKOD=" + TABLOAD + ".KURKOD";
                sql += "      and ISLGELIR.EKOKOD=" + TABLOAD + ".EKOKOD";
                sql += "      and ISLGELIR.FORMDURUM>=2 AND ISLGELIR.FORMDURUM<=5";
                sql += "      and " + GenelKosul.IslemAlanOl("ISLGELIR", "ISLEM") + " IN ('0103', '0104', '0201', '020101', '020102', '020103', '020104', '020105', '020106', '0202', '020201', '020202', '020203')";
                sql += "        ) ,0 ), ";
                sql += ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK] + "= IsNull(";
                sql += "      (select SUM(ODENEK) from ISLGELIR";
                sql += "      where ISLGELIR.YIL=" + TABLOAD + ".YIL";
                sql += "      and ISLGELIR.KURKOD=" + TABLOAD + ".KURKOD";
                sql += "      and ISLGELIR.EKOKOD=" + TABLOAD + ".EKOKOD";
                sql += "      and ISLGELIR.FORMDURUM<2 ";
                sql += "      and " + GenelKosul.IslemAlanOl("ISLGELIR", "ISLEM") + " IN ('0103', '0104', '0201', '020101', '020102', '020103', '020104', '020105', '020106', '0202', '020201', '020202', '020203')";
                sql += "        ) ,0 )";
                sql += "where " + kosul;
            }
            else
            {
                sql = "update " + TABLOAD + " set ";
                sql += ALANLAR[(int)ENUMAlanlar.SERBEST] + "= IsNull(";
                sql += "      (select SUM(" + "ODENEK" + ") from ISLAPGELIR";
                sql += "      where ISLAPGELIR.YIL=ABSBUTCE.YIL";
                sql += "      and ISLAPGELIR.KURKOD=ABSBUTCE.KURKOD";
                sql += "      and ISLAPGELIR.EKOKOD=ABSBUTCE.EKOKOD";
                sql += "      and " + VeriErisim.KosulOl("ISLAPGELIR", "AY", "<=", ahpSerbestAy);
                sql += "      and ISLAPGELIR.FORMDURUM>=2 AND ISLAPGELIR.FORMDURUM<=5";
                sql += "        ) ,0 ), ";
                sql += ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK] + "= IsNull(";
                sql += "      (select SUM(" + "ODENEK" + ") from ISLAPGELIR";
                sql += "      where ISLAPGELIR.YIL=ABSBUTCE.YIL";
                sql += "      and ISLAPGELIR.KURKOD=ABSBUTCE.KURKOD";
                sql += "      and ISLAPGELIR.EKOKOD=ABSBUTCE.EKOKOD";
                sql += "      and " + VeriErisim.KosulOl("ISLAPGELIR", "AY", "<=", ahpSerbestAy);
                sql += "      and ISLAPGELIR.FORMDURUM<2 ";
                sql += "        ) ,0 )";
                sql += " where " + kosul;
            }
            if (sql != "")
                return KomutCalistir(sql, 0);
            else
                return new Sonuc(false, ButceUygulamaResI.BUM343, "");
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// veri tabanından Tertip Listesini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="ekKosul">Sql cümlesine eklenecek ek koşul ifadesini tutan, String türünde nesne</param>
        /// <param name="kurYapi">Kurumsal Yapı bilgisini tutan, String türünde nesne</param>
        /// <param name="hiyerarsik">Hiyerarşi bilgisinin uygulanıp uygulanmayacağı bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tertip Listesini tutan IDataReader türünde nesne döndürür</returns>
        public override IDataReader TertipListesiBul(int yil, string[] kodlar, string ekKosul, string kurYapi, bool hiyerarsik)
        {
            string kosul = "";
            string sql = "";
            string islecArti = VeriErisim.IslecBul("+", veriErisim.baglantiAd);

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (kodlar.Length == 0)
                return null;

            if (string.IsNullOrEmpty(kodlar[0]))
                return null;

            kosul = VeriErisim.KosulEkle(kosul, "AND", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", (string)kodlar[0]));

            if (ekKosul != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);

            sql = "select distinct " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + islecArti + "'-'" + islecArti + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", " +
                "ABSGELIRKOD.EKOAD" +
                " from " + TABLOAD + ", ABSGELIRKOD " +
                " where " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = ABSGELIRKOD.YIL" + " AND " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + " = ABSGELIRKOD.EKOKOD" + " AND " + kosul +
                " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen Bütçe Finansman Bilgierini veri tabanına kaydeden,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kanun">Kanun bilgisini tutan, Double türünde nesne</param>
        /// <param name="kbo">KBÖ bilgisini tutan, Double türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc Kaydet(int yil, string[] kodlar, double kanun, double kbo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[0]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[1]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(kanun) + ", ";
            sql += VeriErisim.DegerYap(kbo) + ", ";
            sql += "0,0,0,0,0,0) ";
            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// İlgili Finansman bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc Sil(int yil, string[] kodlar)
        {
            string sql = "DELETE FROM " + TABLOAD + " WHERE " + GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// veri tabanına yeni bilgileri ekleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc IslemdekiYenileriEkle(int yil, string[] kodlar)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") ";
            sql += " SELECT " + ALANLAR[(int)ENUMAlanlar.YIL] + "," + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", 0, 0, 0, 0, 0, 0";
            sql += " FROM " + TABISLGider.TABLOAD + " WHERE ";
            sql += GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
            sql += " AND 1 NOT IN (SELECT 1 FROM " + TABLOAD + " ";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + " = " + TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + " = " + TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            sql += ")";
            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADDEGER = "ABSFINANSMANDEGER";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDEGER = { "YIL", "KURUM", "BORCLANMA", "LIKIT" };
        
        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDeger : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Kurum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUM = 1,
            /// <summary>
            /// Borçlanma alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCLANMA = 2,
            /// <summary>
            /// Likit alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            LIKIT = 3
        }


        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun veri tabanından Finasman Değerleri döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Finansman Değerleri tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader FinansmanDegerler(int yil, string kurum)
        {
            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARDEGER[(int)ENUMAlanlarDeger.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARDEGER[(int)ENUMAlanlarDeger.KURUM], "=", kurum));

            string sql = "SELECT  " + ALANLARDEGER[(int)ENUMAlanlarDeger.BORCLANMA] + "," + ALANLARDEGER[(int)ENUMAlanlarDeger.LIKIT];
            sql = sql + " FROM " + TABLOADDEGER;
            sql = sql + " WHERE " + kosul;
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen Finasman Değer bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="borc">Borç bilgisini tutan, Double türünde nesne</param>
        /// <param name="likit">Likit bilgisini tutan, Double türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc FinansmanDegerKaydet(int yil, string kurum, double borc, double likit)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";

            sql = "insert into " + TABLOADDEGER + " (" + VeriErisim.TumAlanlar("", ALANLARDEGER) + ") VALUES (" +
                VeriErisim.DegerYap(yil) + ", " +
                VeriErisim.DegerYap(kurum) + ", " +
                VeriErisim.DegerYap(borc) + ", " +
                VeriErisim.DegerYap(likit) + ")";

            return sonuc = KomutCalistir(sql, 1);

        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum bilgisini kullanarak,
        /// İlgili Finansman Değerlerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc FinansmanDegerSil(int yil, string kurum)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADDEGER, ALANLARDEGER[(int)ENUMAlanlarDeger.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADDEGER, ALANLARDEGER[(int)ENUMAlanlarDeger.KURUM], "=", kurum));

            sql = "delete from " + TABLOADDEGER;
            sql = sql + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }
    }

    /*
  public bool HazineYardimiVarmi(int yil, string kurum)
  {
      bool donusDeger = true;
      string kosul = "";
      string sql = "SELECT COUNT(*) FROM " + TABLOAD;
      int kayitSay = 0;

      kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
      kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurum.Replace(".", "") + "%"));
      kosul = kosul + " AND (" + ALANLAR[(int)ENUMAlanlar.EKOKOD] + " LIKE '042102%' OR " + ALANLAR[(int)ENUMAlanlar.EKOKOD] + " LIKE '042202%')";
      sql = sql + " where " + kosul;

      System.Data.IDataReader r = veriErisim.DataReaderDondur(sql);

      if (veriErisim.hata == "")
      {
          if (r.Read())
              if (!r.IsDBNull(0))
              {
                  kayitSay = r.GetInt32(0);
                  if (kayitSay > 0)
                      donusDeger = true;
                  else
                      donusDeger = false;
              }
              else
                  donusDeger = true;
          r.Close();
      }
      return donusDeger;
  }

  // Kendisine  geçirilen,  yil ve kodlar dizisinde 
  // 0. Eleman Kurumsal
  // 3. Eleman Eko. 
  // 
  // olmak üzere, tablodaki değerlerin toplamını alan, sorgu oluşturur.
  // 
  // sorgu sonucu, kanun, kbo, eklOnay, dusOnay, eklYeni, dusYeni kolonlarının toplamı elde edilir ve datareader olarak döndürülür.

  public double kboBul(int yil, string kurKod, string ekoKod)
  {
      string[] x = new string[2];
      x[0] = kurKod;
      x[1] = ekoKod;
      return YSOTBul(yil, x, true);
  }

  public double YSOTBul(int yil, string kurKod, string ekoKod, bool KboBul)
  {
      string[] x = new string[2];
      x[0] = kurKod;
      x[1] = ekoKod;
      return YSOTBul(yil, x, KboBul);
  }

  private string kosulEkle(string kosul, string tabloAd, string alanAd, int yer, string[] kodlar, bool kurumMu)
  {
      if (yer < kodlar.Count)
      {
          string kosulParam = (string)kodlar[yer];
          if (kosulParam != "")
          {
              StringArray sa = new StringArray();
              int say = OrtakFonksiyonlar.StringiParcala(kosulParam, ",", sa);
              string icKosul = "";
              for (int i = 0; i < sa.str.Count; i++)
              {
                  string enIcKosul = "";
                  string icParca = (string)sa.str[i];
                  if (icParca.Length == 2 && kurumMu)
                      enIcKosul = "(" + VeriErisim.KosulOl(tabloAd, alanAd, ">", icParca) + " AND " + VeriErisim.KosulOl(TABLOAD, alanAd, "<", icParca + BHM.Tertip.BagimsizKurumBaslangic()) + ")";
                  else
                      enIcKosul = VeriErisim.KosulOl(tabloAd, alanAd, "LIKE", icParca + "%");
                  icKosul = VeriErisim.KosulEkle(icKosul, "OR", enIcKosul);
              }
              icKosul = "(" + icKosul + ")";
              return VeriErisim.KosulEkle(kosul, "AND", icKosul);
          }
          else
              return kosul;
      }
      else
          return kosul;
  }
*/
} 

using System.Data;
using TNS.Veri;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe Gider bilgilerini tutan ABSBUTCEGIDER veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABButceGider : TABButce
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABButceGider(VeriErisim veriErisim)
            : base(veriErisim)
        {

        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ABSBUTCEGIDER";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "KANUN", "KBO", "EKLENEN", "DUSULEN", "SERBEST", "EKLENEN_TASLAK", "DUSULEN_TASLAK", "SERBEST_TASLAK" };

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
            /// Fonksiyonel kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 2,
            /// <summary>
            /// Finansal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 3,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,
            /// <summary>
            /// Kanun alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KANUN = 5,
            /// <summary>
            /// KBÖ alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KBO = 6,
            /// <summary>
            /// Eklenen alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKLENEN = 7,
            /// <summary>
            /// Düşülen alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DUSULEN = 8,
            /// <summary>
            /// Serbest alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SERBEST = 9,
            /// <summary>
            /// Eklenen taslak alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKLENEN_TASLAK = 10,
            /// <summary>
            /// Düşülen taslak alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DUSULEN_TASLAK = 11,
            /// <summary>
            /// Serbest taslak alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SERBEST_TASLAK = 12
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
            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kistasKodlar, false);
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
            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kistasKodlar, false);
            string sql = "SELECT SUM(SERBEST) from " + TABLOAD + " where " + kosul;
            return veriErisim.TekKolonSQLDbl(sql);
        }


        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisine göre,
        /// verilen kıstaslara uygun Kanun, KBÖ, Eklenen, Düşülen, Serbest ve Taslakları döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde </param>
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

            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kistasKodlar, false);
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
            string sql;
            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
            if (kbolu)
                kosul = VeriErisim.KosulEkle(kosul, " and", VeriErisim.KosulOl("", "KBO", ">", 0));
            sql = "SELECT  " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.FONKOD] + "," + ALANLAR[(int)ENUMAlanlar.FINKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.KBO] + "), SUM(" + ALANLAR[(int)ENUMAlanlar.SERBEST] + ")";
            sql = sql + " FROM " + TABLOAD;
            sql = sql + " WHERE " + kosul;
            sql = sql + " GROUP BY " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.FONKOD] + "," + ALANLAR[(int)ENUMAlanlar.FINKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            sql = sql + " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.FONKOD] + "," + ALANLAR[(int)ENUMAlanlar.FINKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD];
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
            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
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
            Sonuc sonuc = KomutCalistir(sql, 1);
            //if (!sonuc.islemSonuc)
            //{
            //    veriErisim.hata = "";
            //    sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";
            //    sql += VeriErisim.DegerYap(yil) + ", ";
            //    sql += VeriErisim.DegerYap(((string)kodlar[0]).Replace(".", "")) + ", ";
            //    sql += VeriErisim.DegerYap(((string)kodlar[1]).Replace(".", "")) + ", ";
            //    sql += VeriErisim.DegerYap(((string)kodlar[2]).Replace(".", "")) + ", ";
            //    sql += VeriErisim.DegerYap(((string)kodlar[3]).Replace(".", "")) + ", ";
            //    sql += "0,0," + eklenen + "," + dusulen + "," + serbest + ",0,0,0) ";
            //    sonuc = KomutCalistir(sql, 1);
            //}

            return sonuc;
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
            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
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
            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
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
            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kistasKodlar, false);

            if (ahpSerbestAy == 0)
            {
                sql = "update " + TABLOAD + " set ";
                sql += ALANLAR[(int)ENUMAlanlar.SERBEST] + "= " + VeriErisim.IslecBul("IsNull") + "(";
                sql += "      (select SUM(" + "ODENEK" + ") from ISLAPGIDER";
                sql += "      where ISLAPGIDER.YIL=ABSBUTCEGIDER.YIL";
                sql += "      and ISLAPGIDER.KURKOD=ABSBUTCEGIDER.KURKOD";
                sql += "      and ISLAPGIDER.FONKOD=ABSBUTCEGIDER.FONKOD";
                sql += "      and ISLAPGIDER.FINKOD=ABSBUTCEGIDER.FINKOD";
                sql += "      and ISLAPGIDER.EKOKOD LIKE ABSBUTCEGIDER.EKOKOD + '%'";
                sql += "      and ISLAPGIDER.FORMDURUM>=2 AND ISLAPGIDER.FORMDURUM<=5";
                sql += "      and " + GenelKosul.IslemAlanOl("ISLAPGIDER", "ISLEM") + " IN ('0103', '0104', '0201', '020101', '020102', '020103', '020104', '020105', '020106', '0202', '020201', '020202', '020203')";
                sql += "        ) ,0 ), ";
                sql += ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK] + "= " + VeriErisim.IslecBul("IsNull") + "(";
                sql += "      (select SUM(ODENEK) from ISLAPGIDER";
                sql += "      where ISLAPGIDER.YIL=ABSBUTCEGIDER.YIL";
                sql += "      and ISLAPGIDER.KURKOD=ABSBUTCEGIDER.KURKOD";
                sql += "      and ISLAPGIDER.FONKOD=ABSBUTCEGIDER.FONKOD";
                sql += "      and ISLAPGIDER.FINKOD=ABSBUTCEGIDER.FINKOD";
                sql += "      and ISLAPGIDER.EKOKOD LIKE ABSBUTCEGIDER.EKOKOD + '%'";
                sql += "      and ISLAPGIDER.FORMDURUM<2 ";
                sql += "      and " + GenelKosul.IslemAlanOl("ISLAPGIDER", "ISLEM") + " IN ('0103', '0104', '0201', '020101', '020102', '020103', '020104', '020105', '020106', '0202', '020201', '020202', '020203')";
                sql += "        ) ,0 )";
                sql += " where " + kosul;
            }
            else
            {
                sql = "update " + TABLOAD + " set ";
                sql += ALANLAR[(int)ENUMAlanlar.SERBEST] + "= " + VeriErisim.IslecBul("IsNull") + "(";
                //                sql += ALANLAR[(int)ENUMAlanlar.SERBEST] + "= " ;
                sql += "      (select SUM(" + "ODENEK" + ") from ISLAPGIDER";
                sql += "      where ISLAPGIDER.YIL=ABSBUTCEGIDER.YIL";
                sql += "      and ISLAPGIDER.KURKOD=ABSBUTCEGIDER.KURKOD";
                sql += "      and ISLAPGIDER.FONKOD=ABSBUTCEGIDER.FONKOD";
                sql += "      and ISLAPGIDER.FINKOD=ABSBUTCEGIDER.FINKOD";
                sql += "      and ISLAPGIDER.EKOKOD LIKE ABSBUTCEGIDER.EKOKOD + '%'";
                sql += "      and " + VeriErisim.KosulOl("ISLAPGIDER", "AY", "<=", ahpSerbestAy);
                sql += "      and ISLAPGIDER.FORMDURUM>=2 AND ISLAPGIDER.FORMDURUM<=5";
                sql += "        ) ,0 ), ";
                //                sql += "        ), ";
                sql += ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK] + "= " + VeriErisim.IslecBul("IsNull") + "(";
                //                sql += ALANLAR[(int)ENUMAlanlar.SERBEST_TASLAK] + "= ";
                sql += "      (select SUM(" + "ODENEK" + ") from ISLAPGIDER";
                sql += "      where ISLAPGIDER.YIL=ABSBUTCEGIDER.YIL";
                sql += "      and ISLAPGIDER.KURKOD=ABSBUTCEGIDER.KURKOD";
                sql += "      and ISLAPGIDER.FONKOD=ABSBUTCEGIDER.FONKOD";
                sql += "      and ISLAPGIDER.FINKOD=ABSBUTCEGIDER.FINKOD";
                sql += "      and ISLAPGIDER.EKOKOD LIKE ABSBUTCEGIDER.EKOKOD + '%'";
                sql += "      and " + VeriErisim.KosulOl("ISLAPGIDER", "AY", "<=", ahpSerbestAy);
                sql += "      and ISLAPGIDER.FORMDURUM<2 ";
                sql += "        ) ,0 )";
                //                sql += "        ) ";
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

            kosul = GenelKosul.KosulEkle(kosul, TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], 0, kodlar, true);
            kosul = GenelKosul.KosulEkle(kosul, TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], 1, kodlar, false);
            kosul = GenelKosul.KosulEkle(kosul, TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], 2, kodlar, false);
            kosul = GenelKosul.KosulEkle(kosul, TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], 3, kodlar, false);

            if (ekKosul != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", ekKosul);

            if (hiyerarsik && kodlar[0].Length < kurYapi.Length)
            {
                sql = "select distinct " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + islecArti + "'-'" + islecArti + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD] + islecArti + "'-'" + islecArti + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD] + " from " + TABLOAD +
                    " where " +
                    kosul +
                    " order by 1";
            }
            else if (hiyerarsik && string.IsNullOrEmpty(kodlar[1]))
                sql = "select distinct " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + islecArti + "'-'" + islecArti + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD] +
                    " from " + TABLOAD +
                    " where " +
                    kosul +
                    " order by 1";
            else
                sql = "select distinct " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + islecArti + "'-'" + islecArti + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD] + islecArti + "'-'" + islecArti + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD] + islecArti + "'-'" + islecArti + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] +
                    " from " + TABLOAD +
                    " where " +
                    kosul +
                    " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen Bütçe Gider bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="kanun">Kanun bilgisin tutan, Double türünde nesne</param>
        /// <param name="kbo">KBÖ bilgisini tutan, Double türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc Kaydet(int yil, string[] kodlar, double kanun, double kbo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[0]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[1]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[2]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[3]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(kanun) + ", ";
            sql += VeriErisim.DegerYap(kbo) + ", ";
            sql += "0,0,0,0,0,0) ";
            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kullanarak,
        /// İlgili Gider bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Finansal, Fonksiyonel, Ekonomoik Kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc Sil(int yil, string[] kodlar)
        {
            string sql = "DELETE FROM " + TABLOAD + " WHERE " + GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
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
            sql += " SELECT " + ALANLAR[(int)ENUMAlanlar.YIL] + "," + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.FONKOD] + "," + ALANLAR[(int)ENUMAlanlar.FINKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", 0, 0, 0, 0, 0, 0";
            sql += " FROM " + TABISLGider.TABLOAD + " WHERE ";
            sql += GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
            sql += " AND 1 NOT IN (SELECT 1 FROM " + TABLOAD + " ";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + " = " + TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD] + " = " + TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD] + " = " + TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + " = " + TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            sql += ")";
            Sonuc s = KomutCalistir(sql, 1);

            return s;
        }

    }
}

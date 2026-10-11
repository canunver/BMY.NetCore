using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Performans göstergesinin hedeflenen değerlerini tutan PEBGOSTERGEHEDEF veri tabanı
    /// tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABGostergeHedef : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGostergeHedef(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBGOSTERGEHEDEF";

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
            /// PHKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PHKURKOD = 1,

            /// <summary>
            /// ASAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ASAMA = 2,

            /// <summary>
            /// PERHEDEFKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PERHEDEFKOD = 3,

            /// <summary>
            /// GOSTERGEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GOSTERGEKOD = 4,

            /// <summary>
            /// MIKTAR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 5,

            /// <summary>
            /// SIRANO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 6,

            /// <summary>
            /// ACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 7,

            /// <summary>
            /// GHKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GHKURKOD = 8
        }

        /// <summary>
        /// Performans göstergesinin hedeflenen değerlerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "PHKURKOD", "ASAMA", "PERHEDEFKOD", "GOSTERGEKOD", "MIKTAR", "SIRANO", "ACIKLAMA", "GHKURKOD" };

        /// <summary>
        /// Performans göstergesinin hedeflenen değerlerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="gHedef">Performans göstergesi için girilen hedefenen değerlerin üst bilgilerini tutan nesne</param>
        /// <param name="gHedefDeger">Performans göstergesi için girilen hedefenen değerleri tutan nesne</param>
        /// <param name="miktarIndex">Kayıt edilmek istenen hedeflenen değerin dizideki indeksi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(PerformansGostergeHedeflenen gHedef, PerformansGostergeHedeflenenDegerler gHedefDeger, int miktarIndex)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(gHedef.yil) + ",";
            sql += VeriErisim.DegerYap(gHedef.phKurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(gHedef.asama) + ",";
            sql += VeriErisim.DegerYap(gHedef.perHedefKod) + ",";
            sql += VeriErisim.DegerYap(gHedefDeger.gosterge.gostergeKod) + ",";
            sql += VeriErisim.DegerYap(gHedefDeger.miktar[miktarIndex]) + ",";
            sql += VeriErisim.DegerYap(gHedefDeger.siraNo) + ",";
            sql += VeriErisim.DegerYap(gHedefDeger.aciklama[miktarIndex]) + ",";
            sql += VeriErisim.DegerYap(gHedef.ghKurKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kullanarak, ilgili
        /// performans göstergesinin hedeflenen değerlerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="gHedef">Performans göstergesi için girilen hedefenen değerlerin üst bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(PerformansGostergeHedeflenen gHedef)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", gHedef.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PHKURKOD], "=", gHedef.phKurKod.Replace(".", "")));
            if (gHedef.asama > -1)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama + 10));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama + 20));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama + 30));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama + 40));
                sql += ")";
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], "=", gHedef.perHedefKod));
            //Performans Hedef silinince buraya geliyor, bütün perHedef silinmeli birim bazında değil
            if (!string.IsNullOrEmpty(gHedef.ghKurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GHKURKOD], "=", gHedef.ghKurKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve performans göstergesinin
        /// hedeflenen değerlerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="gHedef">Performans göstergesi için girilen hedefenen değerlerin üst bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan performans göstergesinin hedeflenen değerlerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(PerformansGostergeHedeflenen gHedef)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PHKURKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GHKURKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ASAMA];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD];
            sql += ", " + "PEBGOSTERGE.GOSTERGEAD" + ", " + "PEBOLCUBIRIM.OLCUBIRIMAD" + ", " + "PEBGOSTERGE.EBUTCEKOD";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MIKTAR] + ")";
            if (!string.IsNullOrEmpty(gHedef.ghKurKod))
            {
                sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACIKLAMA];
                sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];
            }
            sql += ", PEBPERHEDEF.HEDEF, PEBPERHEDEF.HEDEFGORKOD ";
            sql += " FROM " + TABLOAD + ", PEBGOSTERGE" + ", PEBOLCUBIRIM, PEBPERHEDEF";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD] + "=" + "PEBGOSTERGE.GOSTERGEKOD";
            sql += " AND " + "PEBGOSTERGE.OLCUBIRIMKOD" + "=" + "PEBOLCUBIRIM.OLCUBIRIMKOD";
            sql += " AND " + "PEBPERHEDEF.YIL" + "=" + TABLOAD + ".YIL";
            sql += " AND " + "PEBPERHEDEF.KURKOD" + "=" + TABLOAD + ".PHKURKOD";
            sql += " AND " + "PEBPERHEDEF.HEDEFKOD" + "=" + TABLOAD + ".PERHEDEFKOD";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", gHedef.yil));
            if (!string.IsNullOrEmpty(gHedef.phKurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PHKURKOD], "=", gHedef.phKurKod.Replace(".", "")));
            if (gHedef.asama > -1)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama + 10));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama + 20));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama + 30));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", gHedef.asama + 40));
                sql += ")";
            }
            if (gHedef.perHedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], "=", gHedef.perHedefKod));
            if (!string.IsNullOrEmpty(gHedef.ghKurKod))
            {
                if (gHedef.ghKurKod.Length <= 5)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GHKURKOD], "LIKE", gHedef.ghKurKod.Replace(".", "") + "%"));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GHKURKOD], "=", gHedef.ghKurKod.Replace(".", "")));
            }
            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PHKURKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GHKURKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ASAMA];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD];
            sql += ", " + "PEBGOSTERGE.GOSTERGEAD" + ", " + "PEBOLCUBIRIM.OLCUBIRIMAD" + ", " + "PEBGOSTERGE.EBUTCEKOD";
            if (!string.IsNullOrEmpty(gHedef.ghKurKod))
            {
                sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACIKLAMA];
                sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];
            }
            sql += ", PEBPERHEDEF.HEDEFGORKOD, PEBPERHEDEF.HEDEF ";
            sql += " ORDER BY PEBPERHEDEF.HEDEFGORKOD, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PHKURKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GHKURKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD];
            if (!string.IsNullOrEmpty(gHedef.ghKurKod))
                sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int yil, string ghKurKod, int kaynakAsama, int hedefAsama)
        {
            string sql = "INSERT INTO PEBGOSTERGEHEDEF(YIL, PHKURKOD, ASAMA, PERHEDEFKOD, GOSTERGEKOD, GHKURKOD, MIKTAR, SIRANO, ACIKLAMA)";
            sql += " SELECT YIL, PHKURKOD, " + VeriErisim.DegerYap(hedefAsama) + " ASAMA, PERHEDEFKOD, GOSTERGEKOD, GHKURKOD, MIKTAR, SIRANO, ACIKLAMA from PEBGOSTERGEHEDEF";
            sql += " WHERE YIL = " + VeriErisim.DegerYap(yil);
            sql += " AND GHKURKOD LIKE " + VeriErisim.DegerYap(ghKurKod.Replace(".","") + "%");
            sql += " AND ASAMA = " + VeriErisim.DegerYap(kaynakAsama);

            return KomutCalistir(sql, 0);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            throw new NotImplementedException();
        }
    }
}
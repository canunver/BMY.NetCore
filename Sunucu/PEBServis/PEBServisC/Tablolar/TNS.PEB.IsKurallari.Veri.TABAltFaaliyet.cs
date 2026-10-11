using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Operasyonel plan alt faaliyet bilgilerini tutan PEBALTFAALIYET veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABAltFaaliyet : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAltFaaliyet(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBALTFAALIYET";

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
            /// ALTFAALIYETKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTFAALIYETKOD = 2,

            /// <summary>
            /// FPKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FPKOD = 3,

            /// <summary>
            /// DAIREKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DAIREKOD = 4,

            /// <summary>
            /// ALTFAALIYETAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTFAALIYETAD = 5,

            /// <summary>
            /// SMKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SMKOD = 6
        }

        /// <summary>
        /// Operasyonel plan alt faaliyetinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "ALTFAALIYETKOD", "FPKOD", "DAIREKOD", "ALTFAALIYETAD", "SMKOD" };

        /// <summary>
        /// Operasyonel plan alt faaliyeti bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Operasyonel plan alt faaliyetinin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="af">Operasyonel plan alt faaliyeti bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(AltFaaliyet af)
        {
            af.afKod = AltFaaliyetKodAl(af);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(af.yil) + ",";
            sql += VeriErisim.DegerYap(af.orgKod) + ",";
            sql += VeriErisim.DegerYap(af.afKod) + ",";
            sql += VeriErisim.DegerYap(af.fp.fpKod) + ",";
            sql += VeriErisim.DegerYap(0) + ",";
            sql += VeriErisim.DegerYap(af.afAd) + ",";
            sql += VeriErisim.DegerYap(af.sm.smKod) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = af.afKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan operasyonel plan alt faaliyeti bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="af">Operasyonel plan alt faaliyeti bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(AltFaaliyet af)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FPKOD], af.fp.fpKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAIREKOD], 0) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTFAALIYETAD], af.afAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SMKOD], af.sm.smKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", af.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", af.orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD], "=", af.afKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen operasyonel plan alt faaliyetini
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve operasyonel plan alt faaliyetlerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="af">Operasyonel plan alt faaliyeti bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan operasyonel plan alt faaliyetlerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(AltFaaliyet af)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            //sql += ", PEBFAALIYETPROJE.FPAD";
            //sql += ", PEBDAIRE.DAIREAD";
            //sql += ", PEBSABITMALIYET.SMAD";
            sql += " FROM " + TABLOAD;

            //sql += " INNER JOIN PEBDAIRE";
            //sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "PEBDAIRE.YIL";
            //sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + "=" + "PEBDAIRE.KURKOD";
            //sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DAIREKOD] + "=" + "PEBDAIRE.DAIREKOD";

            //sql += " LEFT OUTER JOIN PEBFAALIYETPROJE";
            //sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "PEBFAALIYETPROJE.YIL";
            //sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + "=" + "PEBFAALIYETPROJE.KURKOD";
            //sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FPKOD] + "=" + "PEBFAALIYETPROJE.FPKOD";

            //sql += " LEFT OUTER JOIN PEBSABITMALIYET";
            //sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "PEBSABITMALIYET.YIL";
            //sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + "=" + "PEBSABITMALIYET.SMKURKOD";
            //sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SMKOD] + "=" + "PEBSABITMALIYET.SMKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", af.yil));
            if(af.orgKod>0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", af.orgKod));

            if (af.afKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD], "=", af.afKod));
            if (af.fp.fpKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKOD], "=", af.fp.fpKod));
            //if (af.daire.daireKod > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DAIREKOD], "=", af.daire.daireKod));
            if (!string.IsNullOrEmpty(af.afAd))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTFAALIYETAD], "LIKE", "%" + af.afAd + "%"));
            if (af.sm.smKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SMKOD], "=", af.sm.smKod));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen operasyonel plan alt faaliyetini kullanarak,
        /// ilgili operasyonel plan alt faaliyetini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="af">Operasyonel plan alt faaliyeti bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(AltFaaliyet af)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", af.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", af.orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD], "=", af.afKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen operasyonel plan alt faaliyetini kullanarak,
        /// operasyonel plan alt faaliyetinin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="af">Operasyonel plan alt faaliyeti bilgilerini tutan nesne</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool AltFaaliyetVarMi(AltFaaliyet af)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { af.orgKod.ToString()};
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD] };
            int[] intDeger = { af.yil, af.afKod };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen operasyonel plan alt faaliyetinin, iş/görevlerden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="af">Operasyonel plan alt faaliyeti bilgilerini tutan nesne</param>
        /// <returns>Operasyonel plan alt faaliyeti; iş/görevlerde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool AltFaaliyetKullanilmisMi(AltFaaliyet af)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { af.orgKod.ToString()};
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD] };
            int[] intDeger = { af.yil, af.afKod };
            return TABGenel.KayitVarmi(veriErisim, "PEBISGOREV", strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen operasyonel plan alt faaliyeti nesnesinin yıl ve birim
        /// özelliklerini kullanarak, yeni operasyonel plan alt faaliyeti programatik kodu döndüren yordam
        /// </summary>
        /// <param name="af">Operasyonel plan alt faaliyeti bilgilerini tutan nesne</param>
        /// <returns>Yeni operasyonel plan alt faaliyeti programatik kodu döndürülür.</returns>
        private int AltFaaliyetKodAl(AltFaaliyet af)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { af.orgKod.ToString() };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { af.yil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTFAALIYETKOD], strAlan, strDeger, intAlan, intDeger);
        }
    }
}
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Sabit maliyet bilgilerini tutan PEBSABITMALIYET veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABSabitMaliyet : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSabitMaliyet(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBSABITMALIYET";

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
            /// SMKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SMKURKOD = 1,

            /// <summary>
            /// SMKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SMKOD = 2,

            /// <summary>
            /// SMAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SMAD = 3,

            /// <summary>
            /// TUR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TUR = 4,

            /// <summary>
            /// PHKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PHKURKOD = 5,

            /// <summary>
            /// PERHEDEFKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PERHEDEFKOD = 6
        }

        /// <summary>
        /// Sabit maliyetin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "SMKURKOD", "SMKOD", "SMAD", "TUR", "PHKURKOD", "PERHEDEFKOD" };

        /// <summary>
        /// Sabit maliyet bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Sabit maliyetin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="sm">Sabit maliyet bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(SabitMaliyet sm)
        {
            sm.smKod = SMKodAl(); //sm.yil, sm.kurKod
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sm.yil) + ",";
            sql += VeriErisim.DegerYap(sm.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sm.smKod) + ",";
            sql += VeriErisim.DegerYap(sm.smAd) + ",";
            sql += VeriErisim.DegerYap(sm.tur) + ",";
            sql += VeriErisim.DegerYap(sm.perHedef.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sm.perHedef.perHedefKod) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = sm.smKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan sabit maliyet bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="sm">Sabit maliyet bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(SabitMaliyet sm)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SMAD], sm.smAd);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], sm.tur);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PHKURKOD], sm.perHedef.kurKod.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], sm.perHedef.perHedefKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sm.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SMKURKOD], "=", sm.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SMKOD], "=", sm.smKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen sabit maliyeti kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve sabit maliyetleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="sm">Sabit maliyet bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan sabit maliyetleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(SabitMaliyet sm)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", PEBPERHEDEF.HEDEF";
            sql += " FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN PEBPERHEDEF";
            sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "PEBPERHEDEF.YIL";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PHKURKOD] + "=" + "PEBPERHEDEF.KURKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD] + "=" + "PEBPERHEDEF.HEDEFKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sm.yil));
            if (!string.IsNullOrEmpty(sm.kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SMKURKOD], "=", sm.kurKod.Replace(".", "")));
            if (sm.tur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", sm.tur));
            if (sm.smKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SMKOD], "=", sm.smKod));
            if (!string.IsNullOrEmpty(sm.perHedef.kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PHKURKOD], "=", sm.perHedef.kurKod.Replace(".", "")));
            if (sm.perHedef.perHedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], "=", sm.perHedef.perHedefKod));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen sabit maliyeti kullanarak,
        /// ilgili sabit maliyeti veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="sm">Sabit maliyet bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(SabitMaliyet sm)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sm.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SMKURKOD], "=", sm.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SMKOD], "=", sm.smKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen yılı, birim kodunu ve sabit maliyet programatik kodunu kullanarak,
        /// ilgili sabit maliyetin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="smKod">Sabit maliyetin programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool SMVarMi(int yil, int smKod)
        {
            string[] strAlan = null; //{ ALANLAR[(int)ENUMAlanlar.SMKURKOD] };
            string[] strDeger = null; //{ kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.SMKOD] };
            int[] intDeger = { yil, smKod };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen yılı ve birim kodunu baz alarak
        /// yeni sabit maliyet programatik kodu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Yeni sabit maliyet programatik kodu döndürülür.</returns>
        public int SMKodAl()
        {
            string[] strAlan = null; //{ ALANLAR[(int)ENUMAlanlar.SMKURKOD] };
            string[] strDeger = null; //{ kurKod.Replace(".", "") };
            string[] intAlan = null; //{ ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = null; //{ yil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SMKOD], strAlan, strDeger, intAlan, intDeger);
        }

        //public bool SMVarMiFromTur(int yil, string kurKod, int tur)
        //{
        //    string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SMKURKOD] };
        //    string[] strDeger = { kurKod.Replace(".", "") };
        //    string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.TUR] };
        //    int[] intDeger = { yil, tur };
        //    return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        //}

        public IDataReader AyniNumaraliBul()
        {

            string sql = @"select DISTINCT D.SMKURKOD,D.SMKOD from PEBSABITMALIYET D
                            where 1 IN (
                            select 1 from PEBSABITMALIYET I
                            WHERE I.YIL=D.YIL AND I.SMKOD=D.SMKOD
                            group by I.YIL,I.SMKOD
                            HAVING COUNT(*)>1)
                            order by 1,2";
            return veriErisim.DataReaderDondur(sql);

        }
    }
}
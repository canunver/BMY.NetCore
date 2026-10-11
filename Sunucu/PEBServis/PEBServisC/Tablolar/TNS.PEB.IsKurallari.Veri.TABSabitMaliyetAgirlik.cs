using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Sabit maliyetlerin faaliyet/projelerle bağını kuran ağırlık bilgilerini
    /// tutan PEBSABITMALIYETAGIRLIK veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABSabitMaliyetAgirlik : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSabitMaliyetAgirlik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBSABITMALIYETAGIRLIK";

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
            /// FPKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FPKURKOD = 3,

            /// <summary>
            /// FPKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FPKOD = 4,

            /// <summary>
            /// AGIRLIK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AGIRLIK = 5,

            /// <summary>
            /// ODENEK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ODENEK = 6,

            /// <summary>
            /// SENARYOKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 7
        }

        /// <summary>
        /// Sabit maliyetin ağırlık bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "SMKURKOD", "SMKOD", "FPKURKOD", "FPKOD", "AGIRLIK", "ODENEK", "SENARYOKOD" };

        /// <summary>
        /// Sabit maliyetin ağırlık bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="smAgirlik">Sabit maliyetin ağırlık bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(SabitMaliyetAgirlik smAgirlik)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(smAgirlik.yil) + ",";
            sql += VeriErisim.DegerYap(smAgirlik.smKurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(smAgirlik.smKod) + ",";
            sql += VeriErisim.DegerYap(smAgirlik.fpKurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(smAgirlik.fpKod) + ",";
            sql += VeriErisim.DegerYap(smAgirlik.agirlik) + ",";
            sql += VeriErisim.DegerYap(smAgirlik.odenek) + ",";
            sql += VeriErisim.DegerYap(smAgirlik.senaryoKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen sabit maliyet ağırlık nesnesinin yılını,
        /// birim kodunu ve sabit maliyet programatik kodunu kullanarak,
        /// ilgili sabit maliyetin ağırlık bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="smAgirlik">Sabit maliyetin ağırlık bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(SabitMaliyetAgirlik smAgirlik)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", smAgirlik.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SMKURKOD], "=", smAgirlik.smKurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SMKOD], "=", smAgirlik.smKod));
            if (!string.IsNullOrEmpty(smAgirlik.senaryoKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", smAgirlik.senaryoKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen sabit maliyet ağırlık bilgilerini
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve ilgili sabit maliyete ait ağırlıkları DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="smAgirlik">Sabit maliyetin ağırlık bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan sabit maliyet ağırlıklarını tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(SabitMaliyetAgirlik smAgirlik)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", PEBSABITMALIYET.SMAD";
            sql += ", A.HARCAMA";
            sql += " FROM " + TABLOAD;

            sql += " INNER JOIN PEBSABITMALIYET";
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "PEBSABITMALIYET.YIL");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SMKURKOD] + "=" + "PEBSABITMALIYET.SMKURKOD");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SMKOD] + "=" + "PEBSABITMALIYET.SMKOD");

            sql += " LEFT OUTER JOIN PEBFPHARCAMAYANSIYAN A";
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "A.YIL");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SMKURKOD] + "=" + "A.SMKURKOD");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SMKOD] + "=" + "A.SMKOD");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FPKURKOD] + "=" + "A.FPKURKOD");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FPKOD] + "=" + "A.FPKOD");

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", smAgirlik.yil));

            string icSQL = "SELECT MAX(B.AY) FROM PEBFPHARCAMAYANSIYAN B WHERE A.YIL=B.YIL AND A.FPKURKOD=B.FPKURKOD AND A.FPKOD=B.FPKOD AND A.SMKURKOD=B.SMKURKOD AND A.SMKOD=B.SMKOD";
            sql = VeriErisim.KosulEkle(sql, "AND", "(A.AY = (" + icSQL + ") OR A.AY IS NULL)");

            if (!string.IsNullOrEmpty(smAgirlik.senaryoKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", smAgirlik.senaryoKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(smAgirlik.smKurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SMKURKOD], "=", smAgirlik.smKurKod.Replace(".", "")));
            if (smAgirlik.smKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SMKOD], "=", smAgirlik.smKod));
            if (!string.IsNullOrEmpty(smAgirlik.fpKurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKURKOD], "=", smAgirlik.fpKurKod.Replace(".", "")));
            if (smAgirlik.fpKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKOD], "=", smAgirlik.fpKod));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SMKURKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SMKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
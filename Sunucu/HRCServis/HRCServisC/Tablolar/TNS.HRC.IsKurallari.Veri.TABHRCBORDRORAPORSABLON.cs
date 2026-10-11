using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// TABHRCBORDRORAPORSABLON veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCBORDRORAPORSABLON : TABOrtak
    {
        /// <summary>
        /// TABHRCBORDRORAPORSABLON sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCBORDRORAPORSABLON(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCBORDRORAPORSABLON";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            KULLANICIKODU = 1,
            AD = 2,
            ACIKLAMA = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "KULLANICIKODU", "AD", "ACIKLAMA" };



        public IDataReader Listele(BordroRaporSablon form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kriter = "";

            if (form.kod > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (!string.IsNullOrEmpty(form.kullaniciKod))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", form.kullaniciKod));
            if (!string.IsNullOrEmpty(form.ad))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "=", form.ad));

            if (kriter != "")
                sql += " WHERE " + kriter;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(BordroRaporSablon form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ", ";
            sql += VeriErisim.DegerYap(form.kullaniciKod) + ", ";
            sql += VeriErisim.DegerYap(form.ad) + ", ";
            sql += VeriErisim.DegerYap(form.aciklama) + " )";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc Sil(BordroRaporSablon form)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            if (!string.IsNullOrWhiteSpace(form.kullaniciKod))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", form.kullaniciKod));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 0);
        }

    }
}

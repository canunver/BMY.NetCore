using System;
using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// İhtiyaç Fazlası Taşınır Malzeme Detay bilgilerini tutan TASACIKPAZARDETAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAcikPazarDetay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAcikPazarDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASACIKPAZARDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Muhasebe Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// Harcama Birim Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 1,
            /// <summary>
            /// Sicil No alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNO = 2,
            /// <summary>
            /// Sıra No alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,
            /// <summary>
            /// Açıklama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 4,
            /// <summary>
            /// Eklenis Tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKLENISTARIHI = 5
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "SICILNO", "SIRANO", "ACIKLAMA", "EKLENISTARIHI" };

        /// <summary>
        /// İhtiyaç Fazlası Taşınır Malzeme Detay bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="detay">İhtiyaç Fazlası Taşınır Malzeme bilgilerini tutan, AcikPazarDetay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(AcikPazarDetay detay)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(detay.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(detay.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(detay.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(detay.siraNo) + ",";
            sql += VeriErisim.DegerYap(detay.aciklama) + ",";
            sql += VeriErisim.DegerYap(detay.eklenisTarihi) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen İhtiyaç Fazlası Taşınır Malzeme Detay bilgilerini kullanarak
        /// ilgili İhtiyaç Fazlası Taşınır Malzeme Detay bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="detay">İhtiyaç Fazlası Taşınır Malzeme bilgilerini tutan, AcikPazarDetay türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(AcikPazarDetay detay)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", detay.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", detay.harcamaKod.Replace(".", "")));

            if (detay.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], "=", detay.prSicilNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen İhtiyaç Fazlası Taşınır Malzeme Form bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Malzeme bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="apf">İhtiyaç Fazlası Taşınır Malzeme Form bilgilerini tutan, AcikPazarForm türünde nesne</param>
        /// <param name="sorgudan">Tarih kontrolünün yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun İhtiyaç Fazlası Taşınır Malzeme bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(AcikPazarForm apf, bool sorgudan)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", TASAMBARSICILVIEW.MUHASEBEAD";
            sql += ", TASAMBARSICILVIEW.HARCAMABIRIMAD";
            sql += ", TASAMBARSICILVIEW.SICILNO";
            sql += ", TASAMBARSICILVIEW.HESAPPLANKOD";
            sql += ", TASAMBARSICILVIEW.HESAPPLANAD";
            sql += ", TASAMBARSICILVIEW.KDVORAN";
            sql += ", TASAMBARSICILVIEW.FIYAT";
            sql += " FROM " + TABLOAD;

            sql += " INNER JOIN TASAMBARSICILVIEW";
            sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + "TASAMBARSICILVIEW.MUHASEBEKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + "=" + "TASAMBARSICILVIEW.HARCAMABIRIMKOD";
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SICILNO] + "=" + "TASAMBARSICILVIEW.PRSICILNO";

            sql += " INNER JOIN TASMUHASEBE";
            sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + "TASMUHASEBE.KOD";

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(apf.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", apf.muhasebeKod));

            if (!string.IsNullOrEmpty(apf.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", apf.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(apf.hesapKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSICILVIEW", "HESAPPLANKOD", "LIKE", apf.hesapKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(apf.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASMUHASEBE", "ILKOD", "=", apf.ilKod));

            if (!string.IsNullOrEmpty(apf.ilceKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASMUHASEBE", "ILCEKOD", "=", apf.ilceKod));

            if (sorgudan)
            {
                TNSDateTime kontrolTarih = new TNSDateTime(DateTime.Now.Date);
                kontrolTarih.AddDays(-60);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKLENISTARIHI], ">", kontrolTarih));
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen İhtiyaç Fazlası Taşınır Malzeme detay bilgilerini kullanarak,
        /// İhtiyaç Fazlası Taşınır Malzeme Detay bilgilerinin kayıtlı olup olmadığını,
        /// KayıtVarMı rutinini çalıştırarak bulan ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="apd">İhtiyaç Fazlası Taşınır Malzeme bilgilerini tutan, AcikPazarDetay türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, kayıt var ise true yoksa false değerini veren Bool türünde nesne döndürür</returns>
        public bool AcikPazardaMi(AcikPazarDetay apd)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] };
            string[] strDeger = { apd.muhasebeKod, apd.harcamaKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.SICILNO] };
            int[] intDeger = { apd.prSicilNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }
    }
}
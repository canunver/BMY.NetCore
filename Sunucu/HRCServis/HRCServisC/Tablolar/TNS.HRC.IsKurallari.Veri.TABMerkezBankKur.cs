using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCMERKEZBANKKUR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCMERKEZBANKKUR : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCMERKEZBANKKUR(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCMERKEZBANKKUR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// KURTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURTARIH = 0,

            /// <summary>
            /// PARABIRIMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PARABIRIMI = 1,

            /// <summary>
            /// DOVIZALIS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOVIZALIS = 2,

            /// <summary>
            /// DOVIZSATIS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOVIZSATIS = 3,

            /// <summary>
            /// EFEKTIFALIS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EFEKTIFALIS = 4,

            /// <summary>
            /// EFEKTIFSATIS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EFEKTIFSATIS = 5,

            /// <summary>
            /// SERBESTALIS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SERBESTALIS = 6,

            /// <summary>
            /// SERBESTSATIS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SERBESTSATIS = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KURTARIH"
            , "PARABIRIMI"
            , "DOVIZALIS"
            , "DOVIZSATIS"
            , "EFEKTIFALIS"
            , "EFEKTIFSATIS"
            , "SERBESTALIS"
            , "SERBESTSATIS"
        };

        /// <summary>
        /// Para birimi bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADPARABIRIMI = "HRCPARABIRIMI";

        /// <summary>
        /// Para birimi bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarParaBirimi
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 1,
            /// <summary>
            /// KISALTMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KISALTMA = 2,
        }

        /// <summary>
        /// Para birimi bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARPARABIRIMI = { "KOD", "ADI", "KISALTMA" };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, MerkezBankasiKurSinifyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MerkezBankasiKurSinif form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURTARIH], "=", form.kurTarih));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, satirlar dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, MerkezBankasiKurSinifyapısında nesne</param>
        /// <param name="satirNo">satirlar dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(MerkezBankasiKurSinif form, int satirNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZALIS], form.satirlar[satirNo].dovizAlis) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZSATIS], form.satirlar[satirNo].dovizSatis) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EFEKTIFALIS], form.satirlar[satirNo].efektifAlis) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EFEKTIFSATIS], form.satirlar[satirNo].efektifSatis);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMI], "=", form.satirlar[satirNo].paraBirim.kisaltma));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURTARIH], "=", form.kurTarih));

            Sonuc sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
                sql += "(" + VeriErisim.DegerYap(form.kurTarih);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].paraBirim.kisaltma);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].dovizAlis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].dovizSatis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].efektifAlis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].efektifSatis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].serbestAlis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].serbestSatis);
                sql += ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, MerkezBankasiKurSinifyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(MerkezBankasiKurSinif form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
               TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KOD] + "," +
               TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.ADI];
            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADPARABIRIMI + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARABIRIMI] + "=" + TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KISALTMA];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURTARIH], "=", new TNSDateTime(form.kurTarih.Oku().Date)));
            if (form.satirlar != null && form.satirlar.Length > 0)
            {
                string kodlar = "";
                for (int i = 0; i < form.satirlar.Length; i++)
                {
                    if (i != 0) kodlar += ",";
                    kodlar += VeriErisim.DegerYap(form.satirlar[i].paraBirim.kisaltma);
                }
                sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARABIRIMI] + " IN (" + kodlar + ")");
            }

            //OrtakFonksiyonlar.HataDosyaYaz("KurSorgu.txt", sql, false);
            return veriErisim.DataReaderDondur(sql);
        }


        /// <summary>
        /// Guncelles the specified form.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="satirNo">The satir no.</param>
        /// <returns></returns>
        public Sonuc Guncelle(MerkezBankasiKurSinif form, int satirNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZALIS], form.satirlar[satirNo].dovizAlis) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZSATIS], form.satirlar[satirNo].dovizSatis) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EFEKTIFALIS], form.satirlar[satirNo].efektifAlis) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EFEKTIFSATIS], form.satirlar[satirNo].efektifSatis) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SERBESTALIS], form.satirlar[satirNo].serbestAlis) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SERBESTSATIS], form.satirlar[satirNo].serbestSatis);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMI], "=", form.satirlar[satirNo].paraBirim.kisaltma));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURTARIH], "=", form.satirlar[satirNo].kurTarihi));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
                sql += "(" + VeriErisim.DegerYap(form.kurTarih);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].paraBirim.kisaltma);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].dovizAlis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].dovizSatis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].efektifAlis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].efektifSatis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].serbestAlis);
                sql += "," + VeriErisim.DegerYap(form.satirlar[satirNo].serbestSatis);
                sql += ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        public IDataReader YakinGunBul(MerkezBankasiKurSinif form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KURTARIH] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURTARIH], "<=", form.kurTarih));
            if (form.satirlar != null && form.satirlar.Length > 0)
            {
                string kodlar = "";
                for (int i = 0; i < form.satirlar.Length; i++)
                {
                    if (i != 0) kodlar += ",";
                    kodlar += VeriErisim.DegerYap(form.satirlar[i].paraBirim.kisaltma);
                }
                sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARABIRIMI] + " IN (" + kodlar + ")");
            }
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

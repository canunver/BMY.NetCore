using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// KYMCOGRAFYA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABORGCOGRAFYA : TABOrtak
    {
        /// <summary>
        /// TABKYMCOGRAFYA sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABORGCOGRAFYA(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ORGCOGRAFYA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,

            /// <summary>
            /// AD2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD2 = 2,

            /// <summary>
            /// KISAAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISAAD = 3,

            /// <summary>
            /// PLAKAKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLAKAKOD = 4,

            /// <summary>
            /// NUFUS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NUFUS = 5,

            /// <summary>
            /// MUHASEBE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBE = 6,

            /// <summary>
            /// MERNISKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MERNISKOD = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KOD"
            , "AD"
            , "AD2"
            , "KISAAD"
            , "PLAKAKOD"
            , "NUFUS"
            , "MUHASEBE"
            , "MERNISKOD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name=\"form\">Anahtar değişkenleri tutan, " + icAd + "yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Cografya form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, Cografya yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(Cografya form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap(form.ad2);
            sql += "," + VeriErisim.DegerYap(form.kisaAd);
            sql += "," + VeriErisim.DegerYap(form.plakaKod);
            sql += "," + VeriErisim.DegerYap(form.nufus);
            sql += "," + VeriErisim.DegerYap(form.muhasebe);
            sql += "," + VeriErisim.DegerYap(form.mernisKod);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name=\"form\">Select edilecek kayıtlara ait kıstas bilgisini tutan, Cografya yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(Cografya form, string kodYapi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            if (!string.IsNullOrEmpty(form.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod.Replace(".", "")));
            else
            {
                if (form.ilceMi && form.ilMi)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PLAKAKOD], "!=", ""));
                }
                else if (form.ilceMi || form.ilMi || form.ulkeMi)
                {
                    int uz;
                    if (form.ilMi) uz = OrtakFonksiyonlar.KodUzunlukBul(kodYapi, 3); 
                    else if (form.ulkeMi) uz = OrtakFonksiyonlar.KodUzunlukBul(kodYapi, 1);
                    else uz = OrtakFonksiyonlar.KodUzunlukBul(kodYapi, 4);
                    string alan = VeriErisim.IslecBul("LEN") + "(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", alan, "=", uz));
                }

                if (!string.IsNullOrEmpty(form.ad))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", form.ad + "%"));

                if (!string.IsNullOrEmpty(form.plakaKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PLAKAKOD], "=", form.plakaKod));

                if (form.muhasebe > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", form.muhasebe));

                if (form.mernisKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MERNISKOD], "=", form.mernisKod));
            }
            if (!string.IsNullOrEmpty(kosul))
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            if (form.ilMi && form.ilceMi)
            {
                sql += " order by PLAKAKOD";
            }
            else if (form.ilMi || form.ilceMi)
            {
                sql += " order by AD";
            }
            return veriErisim.DataReaderDondur(sql);
        }

    }
}

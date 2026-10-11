using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Yatırım alt projelerinin doküman bilgilerini tutan YATDOKUMANLAR
    /// veri tabanı tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATDokuman : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATDokuman(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATDOKUMANLAR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// ALTPROJENO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 0,

            /// <summary>
            /// SIRANO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// DOKUMANTURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANTURKOD = 2,

            /// <summary>
            /// AD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AD = 3,

            /// <summary>
            /// DOSYAAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DOSYAAD = 4
        }

        /// <summary>
        /// Doküman bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "ALTPROJENO", "SIRANO", "DOKUMANTURKOD", "AD", "DOSYAAD" };

        /// <summary>
        /// Doküman bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Dokümanın programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="dokuman">Doküman bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Dokuman dokuman)
        {
            string sql = "";
            int kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO]);
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(dokuman.altProjeKodu) + ",";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(dokuman.tur.kod) + ",";
            sql += VeriErisim.DegerYap(dokuman.ad) + ",";
            sql += VeriErisim.DegerYap(dokuman.dosyaAd) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan doküman bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="dokuman">Doküman bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(Dokuman dokuman)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANTURKOD], dokuman.tur.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], dokuman.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYAAD], dokuman.dosyaAd);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", dokuman.siraNo));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = dokuman.siraNo;
            return sonuc;
        }

        /// <summary>
        /// Verilen yatırım alt projesinin programatik kodunu kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve dokümanları DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="altProjeKodu">Yatırım alt projesinin programatik kodu</param>
        /// <returns>Verilen kriterlere uygun olan dokümanları tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int altProjeKodu)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (altProjeKodu > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeKodu);

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ALTPROJENO] + "," + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen doküman programatik kodunu kullanarak,
        /// ilgili dokümanı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="siraNo">Doküman programatik kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(string siraNo)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen doküman programatik kodunu kullanarak
        /// dokümanın veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="siraNo">Doküman programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool DokumanVarMi(string siraNo)
        {
            string[] strAlan ={ ALANLAR[(int)ENUMAlanlar.SIRANO] };
            string[] strDeger = { siraNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, new string[0], new int[0]);
        }

        /// <summary>
        /// Verilen doküman türün, dokümanlardan herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="dokumanTurKod">Doküman tür kodu</param>
        /// <returns>Doküman türü; dokümanlarda kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool DokumanTurKullanilmisMi(int dokumanTurKod)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.DOKUMANTURKOD] };
            int[] intDeger ={ dokumanTurKod };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }    
    }
}
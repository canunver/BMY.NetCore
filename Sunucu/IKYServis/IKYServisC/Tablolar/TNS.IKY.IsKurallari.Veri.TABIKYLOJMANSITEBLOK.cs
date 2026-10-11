using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
namespace TNS.IKY.IsKurallari.Veri
{
    /// <summary>
    /// IKYLOJMANSITE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIKYLOJMANSITEBLOK : TABOrtak
    {
        /// <summary>
        /// TABIKYLOJMANSITE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIKYLOJMANSITEBLOK(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYLOJMANBLOK";

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
            /// SKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SKOD = 1,

            /// <summary>
            /// BlokAdi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BlokAdi = 2,

            /// <summary>
            /// BlokNo alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BlokNo = 3,

            /// <summary>
            /// BlokDaireSayisi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BlokDaireSayisi = 4,

            /// <summary>
            /// BlokSiteGideriKesilsinmi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BlokSiteGideriKesilsinmi = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "KOD"
            , "SKOD"
            , "BlokAdi"
            , "BlokNo"
            , "BlokDaireSayisi"
            , "BlokSiteGideriKesilsinmi"           
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, LojmanSite yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, LojmanSite yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(LojmanSiteBlok form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.siteKod);
            sql += "," + VeriErisim.DegerYap(form.blokAdi);
            sql += "," + VeriErisim.DegerYap(form.blokNo);
            sql += "," + VeriErisim.DegerYap(form.daireSayisi);
            sql += "," + VeriErisim.DegerYap(form.siteGideri);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Güncellenecek kayıt bilgisini tutan, LojmanSite yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(LojmanSiteBlok form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SKOD], form.siteKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BlokAdi], form.blokAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BlokNo], form.blokNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BlokDaireSayisi], form.daireSayisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BlokSiteGideriKesilsinmi], form.siteGideri);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IKYLOJMANSITE yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(LojmanSiteBlok form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", ";
            sql += TABIKYLOJMANSITE.ALANLAR[(int)TABIKYLOJMANSITE.ENUMAlanlar.SITEADI] + ", ";
            sql += " (select count(*) from IKYLOJMANDAIRE d where d.BKOD = IKYLOJMANBLOK.kod) tum, "; 
            sql += " (select count(*) from IKYLOJMANDAIRE d where d.BKOD = IKYLOJMANBLOK.kod and d.kod not in (select dkod from IKYPERSONELLOJMAN where dkod=d.kod and (BitTarihi is null and TahsisTip = 1 or TahsisTip = 2))) bos "; 
            sql += " FROM " + TABLOAD;
            sql += " inner join " + TABIKYLOJMANSITE.TABLOAD + " on " + TABIKYLOJMANSITE.TABLOAD + "." + TABIKYLOJMANSITE.ALANLAR[(int)TABIKYLOJMANSITE.ENUMAlanlar.KOD] + "=" + ALANLAR[(int)ENUMAlanlar.SKOD];
            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.siteKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SKOD], "=", form.siteKod));
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " order by " + ALANLAR[(int)ENUMAlanlar.BlokNo];
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

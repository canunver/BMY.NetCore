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
    public class TABIKYLOJMANSITE : TABOrtak
    {
        /// <summary>
        /// TABIKYLOJMANSITE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIKYLOJMANSITE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYLOJMANSITE";
        public static string TABLOADSITEYETKI = "IKYSITEYETKILILERI";

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
            /// SITEADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SITEADI = 1,

            /// <summary>
            /// MAHALLE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MAHALLE = 2,

            /// <summary>
            /// CADDE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CADDE = 3,

            /// <summary>
            /// SOKAK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOKAK = 4,

            /// <summary>
            /// SEMT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SEMT = 5,

            /// <summary>
            /// ILCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILCE = 6,

            /// <summary>
            /// IL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IL = 7,

            /// <summary>
            /// YAKITKESILECEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAKITKESILECEK = 8,

            /// <summary>
            /// KESINTITIPI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTITIPI = 9,

            /// <summary>
            /// KESINTICEVREGIDERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTICEVREGIDERI = 10,

            /// <summary>
            /// KESINTISUGIDERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTISUGIDERI = 11,

            /// <summary>
            /// KESINTIDIGER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTIDIGER = 12,

            /// <summary>
            /// KESINTIDIGERACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTIDIGERACIKLAMA = 13,

            /// <summary>
            /// KESINTIORTAKGIDER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KESINTIORTAKGIDER = 14,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "KOD"
            , "SITEADI"
            , "MAHALLE"
            , "CADDE"
            , "SOKAK"
            , "SEMT"
            , "ILCE"
            , "IL"
            , "YAKITKESILECEK"
            , "KESINTITIPI"
            , "KESINTICEVREGIDERI"
            , "KESINTISUGIDERI"
            , "KESINTIDIGER"
            , "KESINTIDIGERACIKLAMA"
            , "KESINTIORTAKGIDER"
        };

        public enum ENUMAlanlarSITEYETKI : int
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
            /// PKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PKOD = 2,

            /// <summary>
            /// AKTIF alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AKTIF = 3,

            /// <summary>
            /// TIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIP = 4,
        }

        public static string[] ALANLARSITEYETKI = {
            "KOD"
            , "SKOD"
            , "PKOD"
            , "AKTIF"
            , "TIP"           
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
        public Sonuc Kaydet(LojmanSite form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.siteAdi);
            sql += "," + VeriErisim.DegerYap(form.mahalle);
            sql += "," + VeriErisim.DegerYap(form.cadde);
            sql += "," + VeriErisim.DegerYap(form.sokak);
            sql += "," + VeriErisim.DegerYap(form.semt);
            sql += "," + VeriErisim.DegerYap(form.ilce);
            sql += "," + VeriErisim.DegerYap(form.il);
            sql += "," + VeriErisim.DegerYap(form.yakitKesilecek);
            sql += "," + VeriErisim.DegerYap(form.kesintiTipi);
            sql += "," + VeriErisim.DegerYap(form.kesintiCevreGideri);
            sql += "," + VeriErisim.DegerYap(form.kesintiSuGideri);
            sql += "," + VeriErisim.DegerYap(form.kesintiDiger);
            sql += "," + VeriErisim.DegerYap(form.kesintiDigerAciklama);
            sql += "," + VeriErisim.DegerYap(form.kesintiOrtakGider);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Güncellenecek kayıt bilgisini tutan, LojmanSite yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(LojmanSite form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SITEADI], form.siteAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MAHALLE], form.mahalle) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CADDE], form.cadde) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOKAK], form.sokak) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEMT], form.semt) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCE], form.ilce) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IL], form.il) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKITKESILECEK], form.yakitKesilecek) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESINTITIPI], form.kesintiTipi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESINTICEVREGIDERI], form.kesintiCevreGideri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESINTISUGIDERI], form.kesintiSuGideri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESINTIDIGER], form.kesintiDiger) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESINTIDIGERACIKLAMA], form.kesintiDigerAciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESINTIORTAKGIDER], form.kesintiOrtakGider);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IKYLOJMANSITE yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(LojmanSite form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if(form.kod>0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            sql += " order by " + ALANLAR[(int)ENUMAlanlar.KOD];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YetkiliKisiVarmi(IKYSiteYetki y)
        {
            string sql = "SELECT kod from IKYSITEYETKILILERI where AKTIF=1 and PKOD = "+ y.pkod;

            if (y.yetkiliOlduguSiteKod > 0)
                sql += " and SKOD = " + y.yetkiliOlduguSiteKod;

            if (y.kod > 0)
                sql += " and kod <>" + y.kod;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SiteYetkilileriListele(int siteKod)
        {
            string sql = @" select  sy.kod,p.KOD, p.AD+' '+p.SOYAD PersonelAd, ps.SITEADI,b.BlokNo,d.DaireNo,l.KOD, s.kod yskod,
                            s.SITEADI ysad, sy.TIP, sy.AKTIF, SO.KAYITAD
                            from IKYSITEYETKILILERI sy
                            inner join IKYLOJMANSITE s on s.KOD = sy.SKOD
                            inner join IKYPERSONEL p on sy.PKOD = p.KOD
                            left outer join IKYPERSONELLOJMAN l on l.PKOD = p.KOD and TahsisTip = 1 and BitTarihi is null
                            left outer join IKYLOJMANDAIRE d on d.KOD = l.DKOD
                            left outer join IKYLOJMANBLOK b on b.KOD = d.BKOD
                            left outer join IKYLOJMANSITE ps on ps.KOD = b.SKOD
                            left outer join IKYSOZLUK SO ON SO.KOD = sy.TIP
                            WHERE s.kod = " + siteKod;
//                        @"    union 
//                            select  0,p.KOD, p.AD+' '+p.SOYAD PersonelAd,s.SITEADI,b.BlokNo,d.DaireNo,l.KOD,0,'', 0, 0
//                            from IKYPERSONELLOJMAN l
//                            inner join IKYLOJMANDAIRE d on d.KOD = l.DKOD
//                            inner join IKYLOJMANBLOK b on b.KOD = d.BKOD
//                            inner join IKYLOJMANSITE s on s.KOD = b.SKOD
//                            inner join IKYPERSONEL p on l.PKOD = p.KOD
//                            where TahsisTip = 1
//                            and BitTarihi is null
//                            and pkod not in (select pkod from IKYSITEYETKILILERI) and s.kod = "  + siteKod ;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SiteYetkiKaydet(IKYSiteYetki form)
        {
            string sql = "INSERT INTO " + TABLOADSITEYETKI + "(" + VeriErisim.TumAlanlar("", ALANLARSITEYETKI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.yetkiliOlduguSiteKod);
            sql += "," + VeriErisim.DegerYap(form.pkod);
            sql += "," + VeriErisim.DegerYap(form.aktif);
            sql += "," + VeriErisim.DegerYap(form.tip);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SiteYetkiGuncelle(IKYSiteYetki form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADSITEYETKI + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARSITEYETKI[(int)ENUMAlanlarSITEYETKI.SKOD], form.yetkiliOlduguSiteKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEYETKI[(int)ENUMAlanlarSITEYETKI.PKOD], form.pkod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEYETKI[(int)ENUMAlanlarSITEYETKI.AKTIF], form.aktif) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSITEYETKI[(int)ENUMAlanlarSITEYETKI.TIP], form.tip);
          
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarSITEYETKI.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SiteYetkiAktifPasifYap(IKYSiteYetki form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADSITEYETKI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSITEYETKI[(int)ENUMAlanlarSITEYETKI.AKTIF], form.aktif);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarSITEYETKI.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }
    }
}

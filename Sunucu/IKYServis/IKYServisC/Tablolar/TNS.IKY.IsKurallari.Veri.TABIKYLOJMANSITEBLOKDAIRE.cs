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
    public class TABIKYLOJMANSITEBLOKDAIRE : TABOrtak
    {
        /// <summary>
        /// TABIKYLOJMANSITE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIKYLOJMANSITEBLOKDAIRE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYLOJMANDAIRE";

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
            /// BKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BKOD = 1,

            /// <summary>
            /// DaireNo alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireNo = 2,

            /// <summary>
            /// DaireTipi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireTipi = 3,

            /// <summary>
            /// DaireMetreKare alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireMetreKare = 4,

            /// <summary>
            /// DaireElektrikNo alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireElektrikNo = 5,

            /// <summary>
            /// DaireSuNo alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireSuNo = 6,

            /// <summary>
            /// DaireDogalGazNo alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireDogalGazNo = 7,

            /// <summary>
            /// DaireCevreTemizlik alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireCevreTemizlik = 8,

            /// <summary>
            /// DaireTelefon alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireTelefon = 9,

            /// <summary>
            /// DaireTuru alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireTuru = 10,

            /// <summary>
            /// DaireKatNo alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireKatNo = 11,

            /// <summary>
            /// DaireAciklama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DaireAciklama = 12,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KOD"
            , "BKOD"
            , "DaireNo"
            , "DaireTipi"
            , "DaireMetreKare"
            , "DaireElektrikNo"  
            , "DaireSuNo"
            , "DaireDogalGazNo"
            , "DaireCevreTemizlik"
            , "DaireTelefon"
            , "DaireTuru"
            , "DaireKatNo"
            , "DaireAciklama"
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
        /// <param name="form">Eklenecek kayıt bilgisini tutan, LojmanSiteBlokDaire yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(LojmanSiteBlokDaire form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.blokKod);
            sql += "," + VeriErisim.DegerYap(form.daireNo);
            sql += "," + VeriErisim.DegerYap(form.daireTipi);
            sql += "," + VeriErisim.DegerYap(form.metrekare);
            sql += "," + VeriErisim.DegerYap(form.elektrikNo);
            sql += "," + VeriErisim.DegerYap(form.suNo);
            sql += "," + VeriErisim.DegerYap(form.dogalgazNo);
            sql += "," + VeriErisim.DegerYap(form.cevtemNo);
            sql += "," + VeriErisim.DegerYap(form.dahiliTel);
            if (form.daireTuru > 0)
                sql += "," + VeriErisim.DegerYap(form.daireTuru);
            else
                sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap(form.daireKatNo);
            sql += "," + VeriErisim.DegerYap(form.daireAciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Güncellenecek kayıt bilgisini tutan, LojmanSiteBlokDaire yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(LojmanSiteBlokDaire form)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BKOD], form.blokKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireNo], form.daireNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireTipi], form.daireTipi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireMetreKare], form.metrekare) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireElektrikNo], form.elektrikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireSuNo], form.suNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireDogalGazNo], form.dogalgazNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireCevreTemizlik], form.cevtemNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireTelefon], form.dahiliTel) + ",";
            if (form.daireTuru > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireTuru], form.daireTuru) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireTuru], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireKatNo], form.daireKatNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DaireAciklama], form.daireAciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IKYLOJMANSITE yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(LojmanSiteBlokDaire form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", " +
                TABIKYLOJMANSITE.ALANLAR[(int)TABIKYLOJMANSITE.ENUMAlanlar.SITEADI] + ", " +
                TABIKYLOJMANSITEBLOK.ALANLAR[(int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.BlokNo] + ", " +
                TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KAYITAD]
                + @" , (select  count(*)
from IKYLOJMANDAIRE d
inner join IKYPERSONELLOJMAN pl on pl.DKOD = d.KOD
where pl.BitTarihi is null and d.KOD = IKYLOJMANDAIRE.KOD and pl.TahsisTip = " + (int)ENUMTahsisTipi.Tahsis + ") dolu "
+ @" , (select  count(*)
from IKYLOJMANDAIRE d
inner join IKYPERSONELLOJMAN pl on pl.DKOD = d.KOD
where d.KOD = IKYLOJMANDAIRE.KOD and pl.TahsisTip = " + (int)ENUMTahsisTipi.Rezervasyon + ") rezerv "
                + " FROM " + TABLOAD;
            sql += " inner join " + TABIKYLOJMANSITEBLOK.TABLOAD + " on " + TABIKYLOJMANSITEBLOK.TABLOAD + "." + TABIKYLOJMANSITEBLOK.ALANLAR[(int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.KOD] + "=" + ALANLAR[(int)ENUMAlanlar.BKOD];
            sql += " inner join " + TABIKYLOJMANSITE.TABLOAD + " on " + TABIKYLOJMANSITE.TABLOAD + "." + TABIKYLOJMANSITE.ALANLAR[(int)TABIKYLOJMANSITE.ENUMAlanlar.KOD] + "=" + TABIKYLOJMANSITEBLOK.ALANLAR[(int)TABIKYLOJMANSITEBLOK.ENUMAlanlar.SKOD];
            sql += " left outer join " + TABSozluk.TABLOAD + " on " + TABSozluk.TABLOAD + "." + TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KOD] + "=" + ALANLAR[(int)ENUMAlanlar.DaireTuru];

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.blokKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BKOD], "=", form.blokKod));
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " order by " + ALANLAR[(int)ENUMAlanlar.DaireNo];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BosDaireListesi(int blokKod)
        {
            string sql = @"select d.kod, d.DaireNo, s.KAYITAD, d.DaireMetreKare 
                            from IKYLOJMANDAIRE d
                            left outer join IKYSOZLUK s on s.KOD = d.DaireTuru
                            where d.kod not in (
                            select d.KOD
                            from IKYLOJMANDAIRE d
                            inner join IKYPERSONELLOJMAN pl on pl.DKOD = d.KOD
                            where pl.BitTarihi is null and BKOD = '" + blokKod + "'" +
                           " ) and BKOD = '" + blokKod + "' order by DaireNo";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DaireDoluMu(int daireKod, int plojmanKod)
        {
            // , pl.TahsisTip, p.TCKIMLIKNO +'/'+ p.AD+ ' ' + p.Soy
            string sql = @"select pl.TahsisTip, p.TCKIMLIKNO +'/'+ p.AD+ ' ' + p.SOYAD, U.AD
                        from IKYLOJMANDAIRE d
                        inner join IKYPERSONELLOJMAN pl on pl.DKOD = d.KOD
                        inner join IKYPERSONEL p on p.kod = pl.PKOD
                        left outer join IKYPERSONELKADRO k on k.pkod = p.kod and " + TABGenel.KadroDurumuAktifler("KADRODURUMU") +
                        " LEFT OUTER JOIN IKYUNVAN U ON U.KOD = K.UNVANKOD " +
                        " where d.KOD = '" + daireKod + "' and pl.KOD != '" + plojmanKod + "'" +
                        " and ( (pl.BitTarihi is null and pl.TahsisTip = " + (int)ENUMTahsisTipi.Tahsis + ") " +
                        " or pl.TahsisTip = " + (int)ENUMTahsisTipi.Rezervasyon + ")";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

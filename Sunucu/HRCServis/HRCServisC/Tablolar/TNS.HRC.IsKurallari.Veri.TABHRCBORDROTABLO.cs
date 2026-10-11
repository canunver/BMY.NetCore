using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCBORDROTABLOTANIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCBORDROTABLOTANIM : TABOrtak
    {
        /// <summary>
        /// TABHRCBORDROTABLOTANIM sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCBORDROTABLOTANIM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCBORDROTABLOTANIM";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "HRCBORDROTABLODETAY";

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
            /// TUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUR = 2,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDETAY : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 1,

            /// <summary>
            /// UNVANKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UNVANKOD = 2,

            /// <summary>
            /// DERECE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DERECE = 3,

            /// <summary>
            /// DEGER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGER = 4,

            /// <summary>
            /// CARPAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CARPAN = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KOD"
            , "AD"
            , "TUR"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = {
            "KOD"
            , "YIL"
            , "UNVANKOD"
            , "DERECE"
            , "DEGER"
            , "CARPAN"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BordroTabloTanim yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleTanim(BordroTabloAna tablo)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.AD] + "=" + VeriErisim.DegerYap(tablo.ad);
            sql += ", " + ALANLAR[(int)ENUMAlanlar.TUR] + "=" + VeriErisim.DegerYap((int)tablo.tur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tablo.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BordroTabloTanim yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleTanim(BordroTabloAna tablo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tablo.kod);
            sql += "," + VeriErisim.DegerYap(tablo.ad);
            sql += "," + VeriErisim.DegerYap((int)tablo.tur);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BordroTabloTanim yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuTanim(int tabloKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (tabloKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tabloKod));
            sql += " order by 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BordroTabloTanim yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(int tabloKod, int yil, int unvanKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.KOD], "=", tabloKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.YIL], "=", yil));
            if (unvanKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.UNVANKOD], "=", unvanKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BordroTabloTanim yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KopyalaDetay(int hedefKod, int yil, int unvanKod, int kaynakKod)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + "(KOD, YIL, UNVANKOD, DERECE, DEGER, CARPAN)";
            sql += $" SELECT {hedefKod} KOD, YIL, UNVANKOD, DERECE, DEGER, CARPAN FROM " + TABLOADDETAY;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.KOD], "=", kaynakKod));

            if (unvanKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.UNVANKOD], "=", unvanKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, bordroTabloDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BordroTabloTanim yapısında nesne</param>
        /// <param name="satirNo">bordroTabloDetay dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleDetay(int tabloKod, int yil, int unvanKod, BordroTabloDetay form)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tabloKod);
            sql += "," + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(unvanKod);
            sql += "," + VeriErisim.DegerYap(form.derece);
            sql += "," + VeriErisim.DegerYap(form.deger);
            sql += "," + VeriErisim.DegerYap(form.carpan);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BordroTabloTanim yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleDetay(BordroTabloDetay form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + ", " + TABLOAD + ".AD, HRCPERUNVANVIEW.AD FROM " + TABLOADDETAY + " LEFT OUTER JOIN " + TABLOAD + " ON " + TABLOADDETAY + ".KOD = " + TABLOAD + ".KOD LEFT OUTER JOIN HRCPERUNVANVIEW ON HRCPERUNVANVIEW.KOD = " + TABLOADDETAY + ".UNVANKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDETAY.KOD], "=", form.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDETAY.YIL], "=", form.yil));
            if (form.unvanKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDETAY.UNVANKOD], "=", form.unvanKod));
            if (form.derece > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDETAY.DERECE], "=", form.derece));
            sql += " ORDER BY 1,HRCPERUNVANVIEW.SIRANO,7,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int kaynak, int hedef, int yil)
        {
            string sql = "INSERT INTO " + TABLOADDETAY;
            sql += " SELECT " + hedef.ToString() + " KOD, YIL, UNVANKOD, DERECE, DEGER, CARPAN FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDETAY.KOD], "=", kaynak));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDETAY.YIL], "=", yil));

            return KomutCalistir(sql, 0);
        }

        public Sonuc Carp(int tabloKod, int yil, double artisKatsayisi)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET " + ALANLARDETAY[(int)ENUMAlanlarDETAY.DEGER] + "= ROUND(" + ALANLARDETAY[(int)ENUMAlanlarDETAY.DEGER] + "*" + VeriErisim.DegerYap(artisKatsayisi) + ", 0)";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tabloKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDETAY.YIL], "=", yil));
            return KomutCalistir(sql, 0);
        }
    }
}

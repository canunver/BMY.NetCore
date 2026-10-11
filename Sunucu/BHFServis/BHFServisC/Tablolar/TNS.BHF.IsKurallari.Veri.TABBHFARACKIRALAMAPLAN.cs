using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// BHFARACKIRALAMAPLANANA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHFARACKIRALAMAPLAN : TABOrtak
    {
        /// <summary>
        /// TABBHFARACKIRALAMAPLANANA sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHFARACKIRALAMAPLAN(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADANA = "BHFARACKIRALAMAPLANANA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarAna : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 3,

            /// <summary>
            /// KAYDEDEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYDEDEN = 4,

            /// <summary>
            /// KAYITTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYITTARIHI = 5,

            /// <summary>
            /// ONAYLAYAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYLAYAN = 6,

            /// <summary>
            /// ONAYTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYTARIHI = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARANA = {
            "YIL"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "DURUM"
            , "KAYDEDEN"
            , "KAYITTARIHI"
            , "ONAYLAYAN"
            , "ONAYTARIHI"
        };


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "BHFARACKIRALAMAPLANDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDetay : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,

            /// <summary>
            /// SIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRA = 3,

            /// <summary>
            /// TASITTIPI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASITTIPI = 4,

            /// <summary>
            /// TASITKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASITKODU = 5,

            /// <summary>
            /// BUTCEKAYNAGI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEKAYNAGI = 6,

            /// <summary>
            /// BUYILKIRALANAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUYILKIRALANAN = 7,

            /// <summary>
            /// BUYILKIRALANANSURE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUYILKIRALANANSURE = 8,

            /// <summary>
            /// GELECEKYILKIRALANACAK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GELECEKYILKIRALANACAK = 9,

            /// <summary>
            /// GELECEKYILKIRALANACAKSURE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GELECEKYILKIRALANACAKSURE = 10,

            /// <summary>
            /// BUYILFIYAT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUYILFIYAT = 11,

            /// <summary>
            /// GELECEKYILKIRALANACAKSURE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GELECEKYILFIYAT = 12,

            /// <summary>
            /// BUYILKIRALANAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONCYILKIRALANAN = 13,

            /// <summary>
            /// BUYILKIRALANANSURE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONCYILKIRALANANSURE = 14,

            /// <summary>
            /// ONCYILFIYAT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONCYILFIYAT = 15,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = {
            "YIL"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "SIRA"
            , "TASITTIPI"
            , "TASITKODU"
            , "BUTCEKAYNAGI"
            , "BUYILKIRALANAN"
            , "BUYILKIRALANANSURE"
            , "GELECEKYILKIRALANACAK"
            , "GELECEKYILKIRALANACAKSURE"
            , "BUYILFIYAT", "GELECEKYILFIYAT", "ONCYILKIRALANAN", "ONCYILKIRALANANSURE", "ONCYILFIYAT"
        };


        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, AracKiralamaPlanAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilAna(AracKiralamaPlanAna form)
        {
            string sql = "DELETE FROM " + TABLOADANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.BIRIMKOD], "=", form.birimKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, AracKiralamaPlanAna yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleAna(AracKiralamaPlanAna form, string kullaniciKodu)
        {
            string sql = "INSERT INTO " + TABLOADANA + "(" + VeriErisim.TumAlanlar("", ALANLARANA) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.muhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(1);
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += ",null, null";
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, AracKiralamaPlanAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuAna(AracKiralamaPlanAna form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADANA, ALANLARANA) + " FROM " + TABLOADANA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.BIRIMKOD], "=", form.birimKod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, AracKiralamaPlanAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(AracKiralamaPlanAna form)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", form.birimKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, aracKiralamaPlanDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, AracKiralamaPlanAna yapısında nesne</param>
        /// <param name="satirNo">aracKiralamaPlanDetay dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleDetay(AracKiralamaPlanAna form, int satirNo)
        {
            AracKiralamaPlanDetay detay = (AracKiralamaPlanDetay)form.aracKiralamaPlanDetay[satirNo];
            string sql = "INSERT INTO " + TABLOADDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.muhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(satirNo + 1);
            sql += "," + VeriErisim.DegerYap(detay.tasitTipi);
            sql += "," + VeriErisim.DegerYap(detay.tasitKodu);
            sql += "," + VeriErisim.DegerYap(detay.butceKaynagi);
            sql += "," + VeriErisim.DegerYap(detay.buYilKiralanan);
            sql += "," + VeriErisim.DegerYap(detay.buYilKiralananSure);
            sql += "," + VeriErisim.DegerYap(detay.gelecekYilKiralanacak);
            sql += "," + VeriErisim.DegerYap(detay.gelecekYilKiralanacakSure);

            sql += "," + VeriErisim.DegerYap(detay.buYilFiyat);
            sql += "," + VeriErisim.DegerYap(detay.gelecekYilFiyat);
            sql += "," + VeriErisim.DegerYap(detay.oncYilKiralanan);
            sql += "," + VeriErisim.DegerYap(detay.oncYilKiralananSure);
            sql += "," + VeriErisim.DegerYap(detay.oncYilFiyat);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc OnayGuncelle(AracKiralamaPlanAna form, string kullaniciKodu)
        {
            string sql = "UPDATE " + TABLOADANA;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.DURUM], form.durum);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.ONAYLAYAN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARANA[(int)ENUMAlanlarAna.ONAYTARIHI], new TNSDateTime(DateTime.Now));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARANA[(int)ENUMAlanlarAna.BIRIMKOD], "=", form.birimKod));
            return KomutCalistir(sql, 1);
        }

        public Sonuc OnayEkle(AracKiralamaPlanAna form, string kullaniciKodu)
        {
            string sql = "INSERT INTO " + TABLOADANA + "(" + VeriErisim.TumAlanlar("", ALANLARANA) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.muhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.durum);
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, AracKiralamaPlanAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OkuDetay(AracKiralamaPlanAna form, int siralaYontem)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", form.birimKod));
            if (siralaYontem == 1)
                sql += " ORDER BY BUTCEKAYNAGI, TASITKODU, SIRA";
            else
                sql += " ORDER BY SIRA";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

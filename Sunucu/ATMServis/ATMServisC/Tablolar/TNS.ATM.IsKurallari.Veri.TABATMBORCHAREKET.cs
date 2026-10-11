using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.ATM;
namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// ATMBORCHAREKET veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMBORCHAREKET : TABOrtak
    {
        /// <summary>
        /// TABATMBORCHAREKET sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMBORCHAREKET(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ATMBORCHAREKET";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// IPARDDONEMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IPARDDONEMI = 0,

            /// <summary>
            /// DOSYAANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYAANAHTAR = 1,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 3,

            /// <summary>
            /// TUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUR = 4,

            /// <summary>
            /// KARAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KARAR = 5,

            /// <summary>
            /// ONGORULENSILINMETARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONGORULENSILINMETARIHI = 6,

            /// <summary>
            /// MIKTARAB alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTARAB = 7,

            /// <summary>
            /// MIKTARULUSAL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTARULUSAL = 8,

            /// <summary>
            /// FAIZAB alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAIZAB = 9,

            /// <summary>
            /// FAIZULUSAL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAIZULUSAL = 10,

            /// <summary>
            /// YATIRILANAB alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YATIRILANAB = 11,

            /// <summary>
            /// YATIRILANULUSAL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YATIRILANULUSAL = 12,

            /// <summary>
            /// DUZELTIMTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DUZELTIMTARIHI = 13,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 14,

            AKTARIMTARIHI = 15,
            AKTARIMTUTARI = 16,
            /// <summary>
            /// ILGILIOLDUGUTAKSIT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIOLDUGUTAKSIT = 17,
            ERTELEMEAB = 18,
            ERTELEMEUL = 19,

            KAYDEDEN = 20,
            KAYITTARIHI = 21,
            ONAYLAYAN = 22,
            ONAYTARIHI = 23,
            GEREKCE = 24,
            ILGILIISLEMSIRANO = 25
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "IPARDDONEMI"
            , "DOSYAANAHTAR"
            , "SIRANO"
            , "TARIH"
            , "TUR"
            , "KARAR"
            , "ONGORULENSILINMETARIHI"
            , "MIKTARAB"
            , "MIKTARULUSAL"
            , "FAIZAB"
            , "FAIZULUSAL"
            , "YATIRILANAB"
            , "YATIRILANULUSAL"
            , "DUZELTIMTARIHI"
            , "DURUM"
            , "AKTARIMTARIHI"
            , "AKTARIMTUTARI"
            , "ILGILIOLDUGUTAKSIT"
            , "ERTELEMEAB"
            , "ERTELEMEUL"
            , "KAYDEDEN"
            , "KAYITTARIHI"
            , "ONAYLAYAN"
            , "ONAYTARIHI"
            , "GEREKCE"
            , "ILGILIISLEMSIRANO"
        };

        ///// <summary>
        ///// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Anahtar değişkenleri tutan, DosyaAB yapısında nesne</param>
        ///// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        //public Sonuc Sil(DosyaAB form, BorcHareket.ENUMHareketTur harTur)
        //{
        //    string sql = "DELETE FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
        //    if (harTur != BorcHareket.ENUMHareketTur.Tanimsiz)
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)harTur));
        //    return KomutCalistir(sql, 0);
        //}

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DosyaAB yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int iPARDDonemi, int dosyaAnahtar, int siraNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", iPARDDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran, borcHareket dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DosyaAB yapısında nesne</param>
        /// <param name="satirNo">borcHareket dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(DosyaAB form, BorcHareket detay, string kullaniciKodu)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.iPARDDonemi);
            sql += "," + VeriErisim.DegerYap(form.dosyaAnahtar);
            sql += "," + VeriErisim.DegerYap(detay.siraNo);
            sql += "," + VeriErisim.DegerYap(detay.tarih);
            sql += "," + VeriErisim.DegerYap((int)detay.tur);
            sql += "," + VeriErisim.DegerYap(detay.karar);
            sql += ", null"; // + VeriErisim.DegerYap(detay.ongorulenSilinmeTarihi);
            sql += "," + VeriErisim.DegerYap(detay.miktarAB);
            sql += "," + VeriErisim.DegerYap(detay.miktarUlusal);
            sql += "," + VeriErisim.DegerYap(detay.faizAB);
            sql += "," + VeriErisim.DegerYap(detay.faizUlusal);

            sql += "," + VeriErisim.DegerYap(detay.yatirilanAB);
            sql += "," + VeriErisim.DegerYap(detay.yatirilanUlusal);
            sql += "," + VeriErisim.DegerYap(detay.duzeltimTarihi);
            sql += "," + VeriErisim.DegerYap(1); //Durum
            sql += "," + VeriErisim.DegerYap(detay.aktarimTarihi);
            sql += "," + VeriErisim.DegerYap(detay.aktarimTutari);
            sql += "," + VeriErisim.DegerYap(detay.borcunIlgiliOlduguTaksit);
            sql += "," + VeriErisim.DegerYap(detay.ertelemeAB);
            sql += "," + VeriErisim.DegerYap(detay.ertelemeUl);
            sql += "," + VeriErisim.DegerYap(kullaniciKodu); //KAYDEDEN
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)); //KAYITTARIHI
            sql += ",NULL, NULL"; //ONAYLAYAN, ONAYTARIHI
            sql += "," + VeriErisim.DegerYap(detay.gerekce); //GEREKCE
            sql += "," + VeriErisim.DegerYap(detay.ilgiliIslemSiraNo); //GEREKCE
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(DosyaAB form, BorcHareket detay, string kullaniciKodu)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], detay.tarih);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], (int)detay.tur);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARAR], detay.karar);
            //sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONGORULENSILINMETARIHI], detay.ongorulenSilinmeTarihi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTARAB], detay.miktarAB);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTARULUSAL], detay.miktarUlusal);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZAB], detay.faizAB);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZULUSAL], detay.faizUlusal);

            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YATIRILANAB], detay.yatirilanAB);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YATIRILANULUSAL], detay.yatirilanUlusal);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DUZELTIMTARIHI], detay.duzeltimTarihi);

            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTARIMTARIHI], detay.aktarimTarihi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTARIMTUTARI], detay.aktarimTutari);

            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILGILIOLDUGUTAKSIT], detay.borcunIlgiliOlduguTaksit);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ERTELEMEAB], detay.ertelemeAB);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ERTELEMEUL], detay.ertelemeUl);

            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYDEDEN], kullaniciKodu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], detay.gerekce);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILGILIISLEMSIRANO], detay.ilgiliIslemSiraNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", detay.siraNo));
            return KomutCalistir(sql, 1);
        }

        public int AnahtarBul(DosyaAB form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            double d = veriErisim.TekKolonSQLDbl(sql);
            return (int)d + 1;
        }

        public IDataReader EskiAnahtarBul(DosyaAB form, BorcHareket detay)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.SIRANO] + ","+ ALANLAR[(int)ENUMAlanlar.DURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH], "=", detay.tarih));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)detay.tur));

            return veriErisim.DataReaderDondur(sql);
        }

        ///// <summary>
        ///// Form yapısında duran alanları kıstas olarak kullanarak,
        ///// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DosyaAB yapısında nesne</param>
        ///// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        //public IDataReader Oku(DosyaAB form, BorcHareket detay)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", D1.DOVIZALIS, KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN FROM " + TABLOAD;
        //    sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCMERKEZBANKKUR D1 ON D1.PARABIRIMI='EUR' AND D1.KURTARIH=" + TABATMDOSYAAB.OncekiAyIlkGun(TABLOAD + ".TARIH");
        //    sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=" + TABLOAD + ".KAYDEDEN";
        //    sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=" + TABLOAD + ".ONAYLAYAN";

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
        //    if (detay != null)
        //    {
        //        if (detay.siraNo > 0)
        //            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", detay.siraNo));
        //    }
        //    return veriErisim.DataReaderDondur(sql);
        //}

        internal static string ListeleUstTablo(string ipardDonemiAlanAd, string dosyaAnahtarAlanAd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", D1.DOVIZALIS, KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN FROM " + TABLOAD;
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCMERKEZBANKKUR D1 ON D1.PARABIRIMI='EUR' AND D1.KURTARIH=" + TABATMDOSYAAB.OncekiAyIlkGun(TABLOAD + ".TARIH");
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=" + TABLOAD + ".KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=" + TABLOAD + ".ONAYLAYAN";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI] + " = " + ipardDonemiAlanAd;
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR] + " = " + dosyaAnahtarAlanAd;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TARIH] + ", " + ALANLAR[(int)ENUMAlanlar.TUR];
            return sql;
        }

        public Sonuc DurumDegistir(TNSDateTime islemZamani, int ipardDonemi, int dosyaAnahtar, int siraNo, int islemTur, string kullaniciKodu)
        {
            string sql = "UPDATE " + TABLOAD;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYLAYAN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], islemTur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            return KomutCalistir(sql, 0);
        }

        public Sonuc DurumDegistirToplu(TNSDateTime hesapTarihi, BorcHareket.ENUMHareketTur islemTur, ENUMDosyaIslemTur islemDurum, string kullaniciKodu, TNSDateTime islemZamani)
        {
            string sql = "UPDATE " + TABLOAD;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYLAYAN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)islemDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH], "=", hesapTarihi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)islemTur));
            return KomutCalistir(sql, 0);
        }
    }
}

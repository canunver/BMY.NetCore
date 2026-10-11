using System;
using System.Data;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABGundelik : TABOrtak
    {
        public TABGundelik(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "HRCGUNDELIK";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADYOLLUKDETAY = "PSLGECICIGOREVYOLLUKMANASDETAY";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADYOLLUK = "PSLGECICIGOREVYOLLUKMANAS";

        public enum ENUMAlanlar
        {
            KOD = 0,
            YIL = 1,
            UNVANKOD = 2,
            COGRAFYAKOD = 3,
            GUNDELIK = 4,
            PARABIRIMKOD = 5,
            TUR = 6
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarYOLLUK : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// YI_YD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YI_YD = 2,

            /// <summary>
            /// KIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KIMLIKNO = 3,

            /// <summary>
            /// UNVANKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UNVANKODU = 4,

            /// <summary>
            /// GEREKCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 5,

            /// <summary>
            /// BASLANGICTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASLANGICTARIH = 6,

            /// <summary>
            /// BITISTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISTARIH = 7,

            /// <summary>
            /// BUTCETERTIBI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCETERTIBI = 8,

            /// <summary>
            /// AVANSTUTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AVANSTUTARI = 9,

            /// <summary>
            /// AVANSODEMEEMRI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AVANSODEMEEMRI = 10,

            /// <summary>
            /// ODEMEEMRI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRI = 11,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 12,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarYOLLUKDETAY : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// TUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUR = 3,

            /// <summary>
            /// GUN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GUN = 4,

            /// <summary>
            /// GECELIKKONAKLAMAUCRETI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GECELIKKONAKLAMAUCRETI = 5,
            /// <summary>
            /// YERKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YERKOD = 6,

            /// <summary>
            /// GUNDELIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GUNDELIK = 7,

            /// <summary>
            /// GUNDELIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENECEK = 8,

            /// <summary>
            /// AÇIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 9,
        }

        static string[] ALANLAR = { "KOD", "YIL", "UNVANKOD", "COGRAFYAKOD", "GUNDELIK", "PARABIRIMKOD", "TUR" };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARYOLLUK = {
            "YIL"
            , "SURECNO"
            , "YI_YD"
            , "KIMLIKNO"
            , "UNVANKODU"
            , "GEREKCE"
            , "BASLANGICTARIH"
            , "BITISTARIH"
            , "BUTCETERTIBI"
            , "AVANSTUTARI"
            , "AVANSODEMEEMRI"
            , "ODEMEEMRI"
            , "DURUM"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARYOLLUKDETAY = {
            "YIL"
            , "SURECNO"
            , "SIRANO"
            , "TUR"
            , "GUN"
            , "GECELIKKONAKLAMAUCRETI"
            , "YERKOD"
            , "GUNDELIK"
            , "ODENECEK"
            , "ACIKLAMA"
        };

        public Sonuc Kaydet(Gundelik bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.unvanKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ulkeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.gundelikTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.paraBirimi.kod) + ",";
            sql += VeriErisim.DegerYap((int)bilgi.tur) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(Gundelik bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], bilgi.yil) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVANKOD], bilgi.unvanKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.COGRAFYAKOD], bilgi.ulkeKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNDELIK], bilgi.gundelikTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARABIRIMKOD], bilgi.paraBirimi.kod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], (int)bilgi.tur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(Gundelik bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public double GundelikBul(int yil, int unvanKodu, string ulkeKod, ENUMGundelikTur tur)
        {
            string sql = "SELECT MAX(GUNDELIK) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.UNVANKOD], "=", unvanKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.COGRAFYAKOD], "=", ulkeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)tur));
            return veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader Listele(Gundelik bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLAR);
            sql += ", UNVAN.AD, COGRAFYA.AD, HRCPARABIRIMI.KISALTMA";
            sql += " FROM " + TABLOAD + " A";

            sql += " LEFT OUTER JOIN InsanKaynaklari.dbo.IKYUNVAN UNVAN ON UNVAN.KOD = A.UNVANKOD";
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA COGRAFYA ON COGRAFYA.KOD = A.COGRAFYAKOD";
            sql += " LEFT OUTER JOIN HRCPARABIRIMI ON HRCPARABIRIMI.KOD = A.PARABIRIMKOD";

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.unvanKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.UNVANKOD], "=", bilgi.unvanKod));
            if (!string.IsNullOrWhiteSpace(bilgi.ulkeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.COGRAFYAKOD], "=", bilgi.ulkeKod));
            if (bilgi.gundelikTutar > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.GUNDELIK], "=", bilgi.gundelikTutar));
            if (bilgi.paraBirimi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.PARABIRIMKOD], "=", bilgi.paraBirimi.kod));
            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)bilgi.tur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY A." + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC, A." + ALANLAR[(int)ENUMAlanlar.UNVANKOD] + ", A." + ALANLAR[(int)ENUMAlanlar.COGRAFYAKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SonrakiYilaKopyala(int yil)
        {
            string sql = @"IF(SELECT COUNT(*) FROM HRCGUNDELIK WHERE YIL = {YIL}+1) = 0
                            BEGIN
                                DECLARE @sayac int = (SELECT MAX(KOD) FROM HRCGUNDELIK)

                                INSERT INTO HRCGUNDELIK SELECT 
                                @sayac + row_number() over (order by (select NULL)), YIL+1, UNVANKOD, COGRAFYAKOD, GUNDELIK, PARABIRIMKOD, TUR
                                FROM HRCGUNDELIK WHERE YIL = {YIL}

                                UPDATE SAYAC SET SAYAC = (SELECT MAX(KOD) FROM HRCGUNDELIK) WHERE KOD = 'HRCGUNDELIK'

                                SELECT 1
                            END
                            ELSE
                                SELECT 0";

            sql = sql.Replace("{YIL}", yil + "");

            int deger = veriErisim.TekKolonSQLInt(sql);

            Sonuc sonuc = new Sonuc();
            if (deger == 1)
            {
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = string.Format("{0} yılının verileri {1} yılına kopyalandı.", yil, yil + 1);
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = string.Format("{0} yılı için daha önceden kayıtlı veriler var.", yil + 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, GeciciGorevYollukManas yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(GeciciGorevYollukManas form)
        {
            string sql = "INSERT INTO " + TABLOADYOLLUK + "(" + VeriErisim.TumAlanlar("", ALANLARYOLLUK) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += ", " + VeriErisim.DegerYap(form.surecNo);
            sql += ", " + VeriErisim.DegerYap((int)form.yi_yd);
            sql += ", " + VeriErisim.DegerYap(form.kimlikNo);
            sql += ", " + VeriErisim.DegerYap(form.unvanKodu);
            sql += ", " + VeriErisim.DegerYap(form.gerekce);
            sql += ", " + VeriErisim.DegerYap(form.baslangicTarih);
            sql += ", " + VeriErisim.DegerYap(form.bitisTarih);
            sql += ", " + VeriErisim.DegerYap(form.butceTertibi);
            sql += ", NULL";
            sql += ", NULL";
            sql += ", NULL";
            sql += ", 0";
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(GeciciGorevYollukManas form)
        {
            string sql = "UPDATE " + TABLOADYOLLUK;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.YI_YD], (int)form.yi_yd);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.KIMLIKNO], form.kimlikNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.UNVANKODU], form.unvanKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.GEREKCE], form.gerekce);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.BASLANGICTARIH], form.baslangicTarih);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.BITISTARIH], form.bitisTarih);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.BUTCETERTIBI], form.butceTertibi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.SURECNO], "=", form.surecNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleOE(GeciciGorevYollukManas form, bool avans)
        {
            string sql = "UPDATE " + TABLOADYOLLUK + " SET ";
            if (avans)
            {
                sql += VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.AVANSTUTARI], (int)form.avansTutari);
                sql += ", " + VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.AVANSODEMEEMRI], form.avansOdemeEmri);
            }
            else
                sql += VeriErisim.AtamaOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.ODEMEEMRI], form.odemeEmri);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.SURECNO], "=", form.surecNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, GeciciGorevYollukManas yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(GeciciGorevYollukManas form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLARYOLLUK) + ", UNVAN.AD, PER.AD, PER.SOYAD, MIF.FORMDURUM, PER.ISTIHDAMSEKLI, FRM.AD, FRM.UYRUK FROM " + TABLOADYOLLUK + " A";

            sql += " LEFT OUTER JOIN Personel.dbo.PERUNVAN UNVAN ON UNVAN.KOD = A.UNVANKODU";
            sql += " LEFT OUTER JOIN Personel.dbo.PERPERSONEL PER ON PER.TCKIMLIKNO = A.KIMLIKNO";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMIFFORM MIF ON MIF.YIL = A.YIL AND MIF.BELGENO = A.ODEMEEMRI";
            sql += $" LEFT OUTER JOIN HRCFIRMABILGISI FRM ON FRM.MUHASEBEKOD = {form.muhasebe} AND FRM.VERGINO = A.KIMLIKNO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLARYOLLUK[(int)ENUMAlanlarYOLLUK.SURECNO], "=", form.surecNo));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, GeciciGorevYollukManas yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(GeciciGorevYollukManas form)
        {
            string sql = "DELETE FROM " + TABLOADYOLLUKDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYOLLUKDETAY[(int)ENUMAlanlarYOLLUKDETAY.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARYOLLUKDETAY[(int)ENUMAlanlarYOLLUKDETAY.SURECNO], "=", form.surecNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, geciciGorevYollukManasDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, GeciciGorevYollukManas yapısında nesne</param>
        /// <param name="satirNo">geciciGorevYollukManasDetay dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(GeciciGorevYollukManas form, int satirNo)
        {
            GeciciGorevYollukManasDetay det = (GeciciGorevYollukManasDetay)form.detay[satirNo];
            string sql = "INSERT INTO " + TABLOADYOLLUKDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARYOLLUKDETAY) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap((int)det.tur);
            sql += "," + VeriErisim.DegerYap(det.gun);
            sql += "," + VeriErisim.DegerYap(det.gecelikKonaklamaUcreti);
            sql += ", " + VeriErisim.DegerYap(det.yerKod.Replace(".", ""));
            sql += ", " + VeriErisim.DegerYap(det.gundelik);
            sql += ", " + VeriErisim.DegerYap(det.odenecek);
            sql += ", " + VeriErisim.DegerYap(det.aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, GeciciGorevYollukManas yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleDetay(GeciciGorevYollukManas form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLARYOLLUKDETAY) + ",C.AD FROM " + TABLOADYOLLUKDETAY + " A";
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA C ON C.KOD = A.YERKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", ALANLARYOLLUKDETAY[(int)ENUMAlanlarYOLLUKDETAY.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLARYOLLUKDETAY[(int)ENUMAlanlarYOLLUKDETAY.SURECNO], "=", form.surecNo));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

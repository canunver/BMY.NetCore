using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeSulama : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeSulama(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Hibe Sulama bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBESULAMA";
        /// <summary>
        /// Hibe Sulama Detay bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "TRMHIBESULAMADETAY";
        /// <summary>
        /// Hibe Sulama Tarihçe bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTARIHCE = "TRMHIBESULAMATARIHCE";

        /// <summary>
        /// Hibe Sulama bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 1,
            /// <summary>
            /// BASVURUNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BASVURUNO = 2,
            /// <summary>
            /// ILKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILKOD = 3,
            /// <summary>
            /// ILCEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILCEKOD = 4,
            /// <summary>
            /// KOYKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOYKOD = 5,
            /// <summary>
            /// KASABA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KASABA = 6,
            /// <summary>
            /// HUKUKISTATU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HUKUKISTATU = 7,
            /// <summary>
            /// KIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KIMLIKNO = 8,
            /// <summary>
            /// VERGIDAIRE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGIDAIRE = 9,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 10,
            /// <summary>
            /// EVRAKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVRAKNO = 11,
            /// <summary>
            /// EVRAKTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVRAKTARIH = 12,
            /// <summary>
            /// ADRES alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADRES = 13,
            /// <summary>
            /// TELEFON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TELEFON = 14,
            /// <summary>
            /// FAKS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FAKS = 15,
            /// <summary>
            /// EPOSTA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EPOSTA = 16,
            /// <summary>
            /// BANKASUBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKASUBEKOD = 17,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 18,
            /// <summary>
            /// SOZLESMETARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOZLESMETARIH = 19,
            /// <summary>
            /// FATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURANO = 20,
            /// <summary>
            /// FATURATARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURATARIH = 21,
            /// <summary>
            /// SOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SOYAD = 22,
            /// <summary>
            /// BABAAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BABAAD = 23,
            /// <summary>
            /// DOGUMTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOGUMTARIH = 24,
            /// <summary>
            /// YERINDETESPITTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YERINDETESPITTARIH = 25,
            /// <summary>
            /// PROJENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PROJENO = 26,
            /// <summary>
            /// GRUPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GRUPKOD = 27,
            /// <summary>
            /// ETAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ETAPNO = 28,
            /// <summary>
            /// SULAMAMODEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SULAMAMODEL = 29,
            /// <summary>
            /// SULANACAKALAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SULANACAKALAN = 30,
            PUAN = 31,
            REDGEREKCE = 32,
            ASILYEDEK = 33,
            PARSELMEVKISI = 34,
            KABULEDILDI = 35,
            VARYOK = 36,
            REDTIP = 37,
            FATURAACIKLAMA = 38,
            CINSIYET = 39
        }

        /// <summary>
        /// Hibe Sulama tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "YIL", "BASVURUNO", "ILKOD", "ILCEKOD", "KOYKOD", "KASABA", "HUKUKISTATU", "KIMLIKNO", "VERGIDAIRE", "AD", "EVRAKNO", "EVRAKTARIH", "ADRES", "TELEFON", "FAKS", "EPOSTA", "BANKASUBEKOD", "DURUM", "SOZLESMETARIH", "FATURANO", "FATURATARIH", "SOYAD", "BABAAD", "DOGUMTARIH", "YERINDETESPITTARIH", "PROJENO", "GRUPKOD", "ETAPNO", "SULAMAMODEL", "SULANACAKALAN", "PUAN", "REDGEREKCE", "ASILYEDEK", "PARSELMEVKISI", "KABULEDILDI", "VARYOK", "REDTIP", "FATURAACIKLAMA", "CINSIYET" };

        public enum ENUMAlanlarDetay
        {
            /// <summary>
            /// HIBEKOD  alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// SATIRNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SATIRNO = 1,
            /// <summary>
            /// SULAMAGRUBUKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SULAMAGRUBUKOD = 2,
            /// <summary>
            /// PROFORMATUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PROFORMATUTAR = 3,
            /// <summary>
            /// MALZEMESERINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMESERINO = 4,
            /// <summary>
            /// FIRMAVERGINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAVERGINO = 5,
            /// <summary>
            /// FIRMAAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAAD = 6,
            /// <summary>
            /// MALZEMETUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMETUTAR = 7,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 8,
            /// <summary>
            /// HIBETUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBETUTAR = 9,
            /// <summary>
            /// HIBEESASTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEESASTUTAR = 10,
            /// <summary>
            /// AYNIKATKI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYNIKATKI = 11,
            /// <summary>
            /// TOPLAMTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 12,
            /// <summary>
            /// REFERANSTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEESASTUTARPROJE = 13,
            HIBEESASTUTARUYGULAMA = 14,

            GERCEKFATURA = 15,

            /// <summary>
            /// GERCEKHIBETUTARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GERCEKHIBETUTARI = 16,
            /// <summary>
            /// EKSIKHIBETUTARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKSIKHIBETUTARI = 17,
            /// <summary>
            /// EKSIKODEMEODENECEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKSIKODEMEODENECEK = 18,
            /// <summary>
            /// EKSIKODEMEDUZELTILDI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKSIKODEMEDUZELTILDI = 19,
            /// <summary>
            /// EKSIKODEMEACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKSIKODEMEACIKLAMA = 20,

            PROFORMAREFERANSTUTAR = 21,

            AYNIKATKIPROJE = 22,

            BASVURUTOPLAMMALALIMTUTAR = 23,

            BASVURUHIBEYEESASMALALIMTUTAR = 24,

            BASVURUAYNIKATKITUTAR = 25,

            BASVURUTALEPEDILENHIBETUTAR = 26
        }

        /// <summary>
        /// Hibe Sulama Detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARDETAY = { "HIBEKOD", "SATIRNO", "SULAMAGRUBUKOD", "PROFORMATUTAR", "MALZEMESERINO", "FIRMAVERGINO", "FIRMAAD", "MALZEMETUTAR", "ACIKLAMA", "HIBETUTAR", "HIBEESASTUTAR", "AYNIKATKI", "TOPLAMTUTAR", "HIBEESASTUTARPROJE", "HIBEESASTUTARUYGULAMA", "GERCEKFATURA", "GERCEKHIBETUTARI", "EKSIKHIBETUTARI", "EKSIKODEMEODENECEK", "EKSIKODEMEDUZELTILDI", "EKSIKODEMEACIKLAMA", "PROFORMAREFERANSTUTAR", "AYNIKATKIPROJE", "BASVURUTOPLAMMALALIMTUTAR", "BASVURUHIBEYEESASMALALIMTUTAR", "BASVURUAYNIKATKITUTAR", "BASVURUTALEPEDILENHIBETUTAR" };

        public enum ENUMAlanlarTarihce
        {
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 0,
            /// <summary>
            /// KULLANICIKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKOD = 1,
            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 2,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 3,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 4,
        }

        /// <summary>
        /// Hibe Sulama tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTARIHCE = { "HIBEKOD", "KULLANICIKOD", "TARIH", "DURUM", "ACIKLAMA" };

        /// <summary>
        /// Hibe Sulama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe sulama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(HibeSulama bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], bilgi.ilKod.Replace(".", "")) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEKOD], bilgi.ilceKod.Replace(".", "")) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOYKOD], bilgi.koyKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KASABA], bilgi.kasaba) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], bilgi.hukukiStatu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], bilgi.kimlikNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIDAIRE], bilgi.vergiDaire) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKNO], bilgi.evrakNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKTARIH], bilgi.evrakTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], bilgi.adres) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], bilgi.telefon) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], bilgi.faks) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], bilgi.ePosta) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOYAD], bilgi.soyad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BABAAD], bilgi.babaAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOGUMTARIH], bilgi.dogumTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SULAMAMODEL], bilgi.sulamaModel) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SULANACAKALAN], bilgi.sulanacakAlan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PUAN], bilgi.puan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.REDGEREKCE], bilgi.redGerekce) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CINSIYET], bilgi.cinsiyet) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARSELMEVKISI], bilgi.parselMevkisi);
            if (bilgi.durum != (int)ENUMHibeSulamaBasvuruDurum.TANIMSIZ)
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Kaydet(HibeSulama bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.basvuruNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilceKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.koyKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kasaba) + ",";
            sql += VeriErisim.DegerYap(bilgi.hukukiStatu) + ",";
            sql += VeriErisim.DegerYap(bilgi.kimlikNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.vergiDaire) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrakNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrakTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.adres) + ",";
            sql += VeriErisim.DegerYap(bilgi.telefon) + ",";
            sql += VeriErisim.DegerYap(bilgi.faks) + ",";
            sql += VeriErisim.DegerYap(bilgi.ePosta) + ",";
            sql += VeriErisim.DegerYap(bilgi.bankaSubeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.faturaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.faturaTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.soyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.babaAd) + ",";
            sql += VeriErisim.DegerYap(bilgi.dogumTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.yerindeTespitTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.projeNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.grupKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.sulamaModel) + ",";
            sql += VeriErisim.DegerYap(bilgi.sulanacakAlan) + ",";
            sql += VeriErisim.DegerYap(bilgi.puan) + ",";
            sql += VeriErisim.DegerYap(bilgi.redGerekce) + ",";
            sql += VeriErisim.DegerYap(bilgi.asilYedek) + ",";
            sql += VeriErisim.DegerYap(bilgi.parselMevkisi) + ",";
            sql += VeriErisim.DegerYap(bilgi.kabulEdildi) + ",";
            sql += VeriErisim.DegerYap(bilgi.varYokPuan) + ",";
            sql += VeriErisim.DegerYap(0) + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap(bilgi.cinsiyet) + ")";

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Hibe Sulama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe sulama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSozlesme(HibeSulama bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], bilgi.sozlesmeTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Hibe Sulama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe sulama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetProjeNo(HibeSulama bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], bilgi.projeNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// hibe sulama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe sulama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetEksikHibeMiktar(HibeSulamaDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADDETAY + " SET ";
            //sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.GERCEKFATURA], bilgi.gercekFatura) + ", ";
            //sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.EKSIKODEMEODENECEK], bilgi.gercekHibeTutar - bilgi.eksikHibeTutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.GERCEKHIBETUTARI], bilgi.gercekHibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.EKSIKHIBETUTARI], bilgi.eksikHibeTutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.EKSIKODEMEACIKLAMA], bilgi.eksikFazlaOdemeAciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", bilgi.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO], "=", bilgi.satirNo));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc KaydetReddedildi(HibeSulama bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], string.Empty) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.REDTIP], bilgi.redTip) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.REDGEREKCE], bilgi.redGerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetAsilYedek(int hibeKod, string asilYedek)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASILYEDEK], asilYedek);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc KaydetKabul(int hibeKod, string kabul)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KABULEDILDI], kabul);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc KaydetDurumDegistir(int hibeKod, int durum)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }

        //////////////  Başvuru durumuna getirildiğinde fatura referans fiyatlarının  ////
        ////////////    başvuru referans fiyatlar olarak güncellenmesi   ////////////////
        public Sonuc KaydetDurumBasvuruReferans2FaturaReferans(int hibeKod)
        {

            //string sql = "UPDATE TRMHIBESULAMAMALZEMEFIYAT ";
            //sql += " SET REFERANSFIYAT=";
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            string sql = " UPDATE f  ";
            sql += " SET ";
            sql += "    f.REFERANSFIYAT = f2.REFERANSFIYAT   ";   ///Referans fiyat basvuru duruma getirilir
            sql += "    ,f.BIRIMFIYAT = f2.BIRIMFIYAT   ";        ///Birim fiyat basvuru duruma getirilir                                                ///
            sql += " FROM TRMHIBESULAMAMALZEMEFIYAT f    ";
            sql += " INNER JOIN TRMHIBESULAMAMALZEMEFIYAT f2    ";
            sql += "   ON f.HIBEKOD=f2.HIBEKOD and f.MALZEMEKOD=f2.MALZEMEKOD and f.SATIRNO=f2.SATIRNO    ";
            sql += "      and f.TIP=2 and f2.TIP=1   ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "f.HIBEKOD", "=", hibeKod));


            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Hibe Sulama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe sulama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetFatura(HibeSulama bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKASUBEKOD], bilgi.bankaSubeKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], bilgi.faturaNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURATARIH], bilgi.faturaTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURAACIKLAMA], bilgi.faturaAciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Hibe Sulama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe sulama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetYerindeTespit(HibeSulama bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERINDETESPITTARIH], bilgi.yerindeTespitTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        ////Grup İşlemleri**************************
        public Sonuc KaydetGrup(HibeSulama bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], bilgi.grupKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc KaydetGrupCikart(int kod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], "") + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMHibeSulamaBasvuruDurum.YERINDETESPIT);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }
        public bool GruptaFarkliDurumVarMi(string grupKod, int durum)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", grupKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", durum));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
        public bool GruptaKayitVarMi(string grupKod)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI));

            string kosulsql = "";

            kosulsql = VeriErisim.KosulEkle(kosulsql, "", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BASVURUNO], "=", grupKod));
            kosulsql = VeriErisim.KosulEkle(kosulsql, "OR", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", grupKod));

            sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosulsql + ")");

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
        public Sonuc GuncelleDurumGruptakiler(string grupKod, int durum)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.DURUM], durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", grupKod));

            return KomutCalistir(sql, 0);
        }
        public IDataReader HibeSulamaListeleGrupBazliToplam(int durum, int tur = -1, params string[] yetkiliIller)
        {
            //string sql = "SELECT " + VeriErisim.TumAlanlar(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR);
            //sql += ", ORGCOGRAFYA.AD";
            //sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + ")";
            //sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI] + ")";
            //sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR] + ")";
            //sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR] + ")";
            //sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.EKSIKODEMEODENECEK] + ")";
            //sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.BASVURUAYNIKATKITUTAR] + ")";
            //sql += " FROM " + TABHibeMakineSulamaGrup.TABLOAD + ", " + TABLOAD + ", " + TABLOADDETAY + ", ORGCOGRAFYA";
            //sql += " WHERE ";
            //if (tur == 1 || tur == 2)
            //    sql += TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GRUPKOD];
            //if (tur == 3 || tur == 4)
            //    sql += TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASVURUNO];//Eksik ödeme yapılanlar için grup kodu değil başvuru numarası yazılıyor
            //sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD];
            //sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILKOD] + "=" + "ORGCOGRAFYA.KOD";
            //if (durum != (int)ENUMHibeSulamaBasvuruDurum.TANIMSIZ)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], "=", (int)durum));
            //else
            //{
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], ">=", (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER));
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], "<", (int)ENUMHibeSulamaBasvuruDurum.ODENDI));
            //}

            //if (tur > -1)
            //{
            //    if (tur == 1 || tur == 3)//GAP ve GAP Eksik Ödeme
            //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "1"));
            //    else
            //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
            //}

            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.ODEMEKOD], "=", 0));

            //if (yetkiliIller != null && yetkiliIller.Length > 0)
            //{
            //    string ilKosul = string.Empty;
            //    foreach (string il in yetkiliIller)
            //    {
            //        if (string.IsNullOrEmpty(il.Trim()))
            //            continue;
            //        if (!string.IsNullOrEmpty(ilKosul))
            //            ilKosul += ", ";
            //        ilKosul += VeriErisim.DegerYap(il.Trim().Replace(".", ""));
            //    }
            //    if (!string.IsNullOrEmpty(ilKosul))
            //        sql = VeriErisim.KosulEkle(sql, "AND", TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.ILKOD] + " IN (" + ilKosul + ")");
            //}

            //sql += " GROUP BY ORGCOGRAFYA.AD," + VeriErisim.TumAlanlar(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR);
            //sql += " ORDER BY ORGCOGRAFYA.AD";
            //sql += ", " + TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD];

            ///  Yeni değişiklik için referans fiyat hesaplamasında kullanılmak üzere yazıldı TAMAY  ////
            string sql = "SELECT a1.KOD, a1.ILKOD, a1.DURUM, a1.ODEMEKOD, a1.TIPI, d1.AD, a1.KONTROLTARIHI, ";
            sql += " SUM(c1.TOPLAMTUTAR) ToplamTutar,  ";
            //sql += " SUM(c1.AYNIKATKI) AyniKatki, ";
            //sql += " (	select SUM(aa.Aynikatki2) ";
            //sql += " 	 from ";
            //sql += " 	(select c.HIBEKOD, ";
            //sql += " 	c.AYNIKATKI, ";
            //sql += " 	c.BASVURUAYNIKATKITUTAR, ";
            //sql += " 	c.AYNIKATKIPROJE, ";
            //sql += " 	case  ";
            //sql += " 	  when (c.BASVURUAYNIKATKITUTAR > 0) then ";
            //sql += " 	          case ";
            //sql += " 	      	   when ((b.HUKUKISTATU = 1) and (c.MALZEMETUTAR >= 100000) ";
            //sql += " 	      	      or (b.HUKUKISTATU = 2) and (c.MALZEMETUTAR >= 200000)) ";
            //sql += " 	      		  then ";
            //sql += " 	      		  			  case ";
            //sql += " 	      		  			   when (c.MALZEMETUTAR < c.PROFORMAREFERANSTUTAR) ";
            //sql += " 	      		  			    then 0 ";
            //sql += " 	      		  				else (c.MALZEMETUTAR-c.PROFORMAREFERANSTUTAR) ";
            //sql += " 	      		  			   end ";
            //sql += " 	             else ";
            //sql += " 	      	      (c.MALZEMETUTAR-c.HIBEESASTUTARUYGULAMA)   ";
            //sql += " 	             end             ";
            //sql += " 		  else ";
            //sql += " 		  c.AYNIKATKI ";
            //sql += " 		  end Aynikatki2 ";
            //sql += " 		from TRMHIBEMAKINAGRUP a, TRMHIBESULAMA b, TRMHIBESULAMADETAY c, ORGCOGRAFYA d ";
            //sql += " 	  where a.KOD=b.GRUPKOD AND  ";
            //sql += " b.KOD=c.HIBEKOD AND b.ILKOD=d.KOD  ";
            //sql += " AND a.DURUM=16 AND d.GAP <> '1'  ";
            //sql += " AND a1.KOD=a.KOD  ";
            //sql += " AND a.ODEMEKOD=0) aa) AyniKatkiYeni,  ";
            //sql += " SUM(c1.MALZEMETUTAR-c1.GERCEKFATURA) AyniKatkiYeni, ";
            //sql += " SUM(c1.AYNIKATKI) AyniKatkiYeni, ";
            sql += " SUM(c1.TOPLAMTUTAR-c1.GERCEKFATURA) AyniKatkiYeni, ";
            sql += " SUM(c1.HIBEESASTUTAR) HibeEsasTutar,  ";
            sql += " SUM(c1.HIBETUTAR) HibeTutar,  ";
            sql += " SUM(c1.EKSIKHIBETUTARI) Eksikodemeodeneck,  ";
            sql += " SUM(c1.BASVURUAYNIKATKITUTAR) Basvuruaynikatkitutar,  ";
            sql += " SUM(ROUND((c1.TOPLAMTUTAR-c1.BASVURUAYNIKATKITUTAR-(c1.TOPLAMTUTAR-c1.GERCEKFATURA))/2,2))";
            sql += " FROM TRMHIBEMAKINAGRUP a1, TRMHIBESULAMA b1, TRMHIBESULAMADETAY c1, ORGCOGRAFYA d1  ";
            sql += " WHERE ";
            //sql += " a1.KOD=b1.GRUPKOD ";
            if (tur == 1 || tur == 2 || tur == 10 || tur == 20 || tur == 30)
                sql += " a1.KOD = b1.GRUPKOD ";
            if (tur == 3 || tur == 4 || tur == 11 || tur == 21 || tur == 31)
                sql += " a1.KOD = b1.BASVURUNO ";//Eksik ödeme yapılanlar için grup kodu değil başvuru numarası yazılıyor
            sql += " AND b1.KOD=c1.HIBEKOD AND b1.ILKOD=d1.KOD  ";
            //sql += " AND a1.DURUM=16 ";
            if (durum != (int)ENUMHibeSulamaBasvuruDurum.TANIMSIZ)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("a1", TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], "=", (int)durum));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("a1", TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], ">=", (int)ENUMHibeSulamaBasvuruDurum.ODEMEYEGONDER));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("a1", TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], "<", (int)ENUMHibeSulamaBasvuruDurum.ODENDI));
            }
            //sql += " AND d1.GAP <> '1'  ";
            if (tur > -1)
            {
                //Ödeme türü kaldırıldı. melih 01.02.2023
                //if (tur == 1 || tur == 3)//GAP ve GAP Eksik Ödeme
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("d1", "GAP", "=", "1"));
                //else if (tur == 10 || tur == 11)//KOP
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("d1", "GAP", "=", "3"));
                //else if (tur == 20 || tur == 21)//DAP
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("d1", "GAP", "=", "4"));
                //else if (tur == 30 || tur == 31)//DOKAP
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("d1", "GAP", "=", "5"));
                //else if (tur == 2 || tur == 4)//Diğer İller ve diğer iller eksik ödeme
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("d1", "GAP", "=", "0"));
            }

            //sql += " AND a1.ODEMEKOD=0  ";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("a1", TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.ODEMEKOD], "=", 0));

            if (yetkiliIller != null && yetkiliIller.Length > 0)
            {
                string ilKosul = string.Empty;
                foreach (string il in yetkiliIller)
                {
                    if (string.IsNullOrEmpty(il.Trim()))
                        continue;
                    if (!string.IsNullOrEmpty(ilKosul))
                        ilKosul += ", ";
                    ilKosul += VeriErisim.DegerYap(il.Trim().Replace(".", ""));
                }
                if (!string.IsNullOrEmpty(ilKosul))
                    sql = VeriErisim.KosulEkle(sql, "AND", "a1" + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.ILKOD] + " IN (" + ilKosul + ")");
            }

            sql += " GROUP BY d1.AD,a1.KOD, a1.ILKOD,  ";
            sql += " a1.DURUM, a1.ODEMEKOD, a1.TIPI, a1.KONTROLTARIHI  ";
            sql += " ORDER BY d1.AD, a1.KOD ";

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc KaydetTarihceGruptakiler(string grupKod, HibeSulamaTarihce tarihce)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE;
            sql += " SELECT " + ALANLAR[(int)ENUMAlanlar.KOD];
            sql += ", " + VeriErisim.DegerYap(tarihce.tarih);
            sql += ", " + VeriErisim.DegerYap(tarihce.kullaniciKod);
            sql += ", " + VeriErisim.DegerYap(tarihce.durum);
            sql += ", " + VeriErisim.DegerYap(tarihce.aciklama);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", grupKod));
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BASVURUNO], "=", grupKod));//Eksik ödeme işlemi yapılırken gerekli

            return KomutCalistir(sql, 0);
        }
        ////Grup İşlemleri**************************

        public Sonuc YatirimciDegistir(HibeSulama bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOYAD], bilgi.soyad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BABAAD], bilgi.babaAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], bilgi.adres) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOGUMTARIH], bilgi.dogumTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIDAIRE], bilgi.vergiDaire) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], bilgi.kimlikNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Hibe Sulama bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek hibe sulama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(HibeSulama bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }
        public int ListeleDurum(int hibeKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return veriErisim.TekKolonSQLInt(sql);
        }
        public double HibeSulamaOdemeMiktarGetir(int tur)
        {
            string sql = "SELECT SUM(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR] + ") FROM " + TABLOAD +
                " INNER JOIN " + TABHibeMakineSulamaGrup.TABLOAD + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GRUPKOD] + "=" + TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD] +
                " INNER JOIN " + TABLOADDETAY + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD] +
                " INNER JOIN ORGCOGRAFYA ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILKOD] + "=" + "ORGCOGRAFYA.KOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], "=", (int)ENUMHibeSulamaBasvuruDurum.ODEMEBASLAT));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.ODEMEKOD], "=", 0));

            if (tur > -1)
            {
                //Ödeme türü kaldırıldı. melih 01.02.2023
                //if (tur == 1 || tur == 3)//GAP ve GAP Eksik Ödeme
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "1"));
                //else
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
            }

            return veriErisim.TekKolonSQLDbl(sql);
        }
        /// <summary>
        /// Hibe Sulama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hibe sulama bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hibe Sulama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(HibeSulama bilgi, string orderByKolon, bool asc)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            //sql += ", " + TABSulamaGrubu.TABLOAD + "." + TABSulamaGrubu.ALANLAR[(int)TABSulamaGrubu.ENUMAlanlar.AD] + " AS GRUPAD";
            //sql += ", " + TABSulamaGrubu.TABLOAD + "." + TABSulamaGrubu.ALANLAR[(int)TABSulamaGrubu.ENUMAlanlar.HIBEORAN];
            sql += ",SULAMAKONU";
            sql += ", " + TABKoy.TABLOAD + "." + TABKoy.ALANLAR[(int)TABKoy.ENUMAlanlar.ADI] + ", ORGCOGRAFYA.AD AS ILAD, ORGCOGRAFYA_1.AD AS ILCEAD, PARSELNO_LISTE";
            sql += " FROM TRMHIBESULAMA LEFT OUTER JOIN";
            sql += " TRMHIBESULAMADETAY ON TRMHIBESULAMADETAY.HIBEKOD = TRMHIBESULAMA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBESULAMA.ILKOD = ORGCOGRAFYA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBESULAMA.ILCEKOD = ORGCOGRAFYA_1.KOD LEFT OUTER JOIN";
            sql += " TRMKOY ON TRMHIBESULAMA.KOYKOD = TRMKOY.KOD";
            sql += " CROSS APPLY (Select DISTINCT TB.AD+', ' From TRMHIBESULAMADIGER TA INNER JOIN TRMSULAMAGRUBU TB ON TA.SULAMAGRUPKOD = TB.KOD WHERE TA.HIBEKOD=TRMHIBESULAMA.KOD For XML PATH('')) TST(SULAMAKONU)";
            sql += " CROSS APPLY (SELECT DISTINCT TA.PARSELNO + '; ' FROM TRMHIBESULAMADIGER TA WHERE TA.HIBEKOD = TRMHIBESULAMA.KOD  AND LEN(TA.PARSELNO)>1 FOR XML PATH('')) PST(PARSELNO_LISTE)";
            string kosul = ListeleKriterOl(bilgi);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY ";
            if (string.IsNullOrEmpty(orderByKolon))
                sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];
            else
                sql += orderByKolon;
            if (asc)
                sql += " ASC";
            else
                sql += " DESC";

            return veriErisim.DataReaderDondur(sql);
        }
        public int HibeSulamaBasvuruSayiDondur(HibeSulama bilgi)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD + " INNER JOIN ";
            sql += TABLOADDETAY + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD];

            string kosul = ListeleKriterOl(bilgi);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLInt(sql);
        }
        private string ListeleKriterOl(HibeSulama bilgi)
        {
            string kosul = string.Empty;
            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

            if (bilgi.sorguKod != "")
            {
                string icKosul = "";
                string[] kodlar = bilgi.sorguKod.Split(',');
                foreach (string kod in kodlar)
                {
                    if (OrtakFonksiyonlar.ConvertToInt(kod, 0) > 0)
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", OrtakFonksiyonlar.ConvertToInt(kod, 0)));
                }

                if (icKosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            }

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.etapNolar != null && bilgi.etapNolar.Length > 1)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ETAPNO] + " IN (" + String.Join(",", bilgi.etapNolar) + ")");
            }
            else if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            if (bilgi.sorguEtapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], ">=", bilgi.sorguEtapNo));
            if (!string.IsNullOrEmpty(bilgi.grupKod))
            {
                string[] gler = bilgi.grupKod.Split(';');
                string icKosul = "";
                foreach (string g in gler)
                {
                    if (!string.IsNullOrEmpty(g.Trim()))
                    {
                        if (g.Length == 11)
                            icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASVURUNO], "=", g.Trim()));
                        else
                            icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", g.Trim()));
                    }
                }
                if (icKosul != "")
                    kosul += " AND (" + icKosul + ")";
            }
            if (!string.IsNullOrEmpty(bilgi.basvuruNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASVURUNO], "=", bilgi.basvuruNo));
            if (!string.IsNullOrEmpty(bilgi.cinsiyet))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CINSIYET], "=", bilgi.cinsiyet));
            if (!string.IsNullOrEmpty(bilgi.ilKod))
            {
                string[] iller = bilgi.ilKod.Split(';');
                string ilKosul = "";
                foreach (string il in iller)
                {
                    if (il.Trim() == "") continue;
                    ilKosul = VeriErisim.KosulEkle(ilKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", il.Trim().Replace(".", "")));
                }
                if (ilKosul != "")
                    kosul += " AND (" + ilKosul + ")";
            }
            if (!string.IsNullOrEmpty(bilgi.ilceKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCEKOD], "=", bilgi.ilceKod.Replace(".", "")));
            if (bilgi.koyKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOYKOD], "=", bilgi.koyKod));
            if (!string.IsNullOrEmpty(bilgi.kasaba))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KASABA], "LIKE", "%" + bilgi.kasaba + "%"));
            if (bilgi.hukukiStatu > 0)
            {
                if (bilgi.hukukiStatu < 10)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "=", bilgi.hukukiStatu));
                }
                if (bilgi.hukukiStatu == 11)
                {
                    string ilKosul = "";
                    ilKosul = VeriErisim.KosulEkle(ilKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "=", (bilgi.hukukiStatu - 10)));
                    ilKosul = VeriErisim.KosulEkle(ilKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "=", (bilgi.hukukiStatu - 8)));
                    kosul += " AND (" + ilKosul + ")";
                }
                if (bilgi.hukukiStatu == 12)
                {
                    string ilKosul = "";
                    ilKosul = VeriErisim.KosulEkle(ilKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "=", (bilgi.hukukiStatu - 10)));
                    ilKosul = VeriErisim.KosulEkle(ilKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "=", (bilgi.hukukiStatu - 8)));
                    ilKosul = VeriErisim.KosulEkle(ilKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "=", (bilgi.hukukiStatu - 7)));
                    kosul += " AND (" + ilKosul + ")";
                }
            }
            if (!string.IsNullOrEmpty(bilgi.kimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", bilgi.kimlikNo));
            if (!string.IsNullOrEmpty(bilgi.vergiDaire))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VERGIDAIRE], "=", bilgi.vergiDaire));
            if (!string.IsNullOrEmpty(bilgi.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD] + "+' '+" + ALANLAR[(int)ENUMAlanlar.SOYAD], "LIKE", "%" + bilgi.ad + "%"));
            if (!string.IsNullOrEmpty(bilgi.evrakNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKNO], "=", bilgi.evrakNo));
            if (!string.IsNullOrEmpty(bilgi.projeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJENO], "=", bilgi.projeNo));
            if (!bilgi.evrakTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKTARIH], "=", bilgi.evrakTarih));
            if (!bilgi.sozlesmeTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], "=", bilgi.sozlesmeTarih));
            if (!bilgi.evrakTarihBaslangic.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKTARIH], ">=", bilgi.evrakTarihBaslangic));
            if (!bilgi.evrakTarihBitis.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKTARIH], "<=", bilgi.evrakTarihBitis));
            if (!string.IsNullOrEmpty(bilgi.adres))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADRES], "LIKE", "%" + bilgi.adres + "%"));
            if (!string.IsNullOrEmpty(bilgi.telefon))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TELEFON], "=", bilgi.telefon));
            if (!string.IsNullOrEmpty(bilgi.faks))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FAKS], "=", bilgi.faks));
            if (!string.IsNullOrEmpty(bilgi.ePosta))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EPOSTA], "=", bilgi.ePosta));
            if (!string.IsNullOrEmpty(bilgi.bankaSubeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BANKASUBEKOD], "=", bilgi.bankaSubeKod));
            if (!string.IsNullOrEmpty(bilgi.asilYedek))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASILYEDEK], "=", bilgi.asilYedek));
            if (!string.IsNullOrEmpty(bilgi.kabulEdildi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KABULEDILDI], "=", bilgi.kabulEdildi));
            if (bilgi.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));
            if (bilgi.redTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.REDTIP], "=", bilgi.redTip));

            if (!bilgi.sozlesmeTarihBaslangic.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], ">=", bilgi.sozlesmeTarihBaslangic));
            if (!bilgi.sozlesmeTarihBitis.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], "<=", bilgi.sozlesmeTarihBitis));
            if (bilgi.sulamaModel > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SULAMAMODEL], "=", bilgi.sulamaModel));
            if (bilgi.sulanacakAlan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SULANACAKALAN], "=", bilgi.sulanacakAlan));
            if (bilgi.puan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], "=", bilgi.puan));
            if (bilgi.puanBas > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], ">=", bilgi.puanBas));
            if (bilgi.puanBit > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], "<=", bilgi.puanBit));

            if (bilgi.detay != null && bilgi.detay.Length > 0)
            {
                if (bilgi.detay[0].sulamaGrubuKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "TRMHIBESULAMA.KOD IN (SELECT HIBEKOD FROM TRMHIBESULAMADIGER IC WHERE IC.HIBEKOD=TRMHIBESULAMA.KOD AND IC.SULAMAGRUPKOD=" + VeriErisim.DegerYap(bilgi.detay[0].sulamaGrubuKod) + ")");

                if (bilgi.detay[0].firmaVergiNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "TRMHIBESULAMA.KOD IN (SELECT HIBEKOD FROM TRMHIBESULAMADIGER IC WHERE IC.HIBEKOD=TRMHIBESULAMA.KOD AND IC.FIRMAVERGINO=" + VeriErisim.DegerYap(bilgi.detay[0].firmaVergiNo) + ")");

                if (!string.IsNullOrWhiteSpace(bilgi.detay[0].parselListe))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "TRMHIBESULAMA.KOD IN (SELECT HIBEKOD FROM TRMHIBESULAMADIGER IC WHERE  IC.HIBEKOD=TRMHIBESULAMA.KOD AND IC.PARSELNO LIKE " + VeriErisim.DegerYap("%" + bilgi.detay[0].parselListe + "%") + ")");
            }
            return kosul;
        }
        public bool FarkliHukukiStatudeKaydiVarMi(HibeSulama bilgi)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", bilgi.kimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "<>", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "<>", bilgi.hukukiStatu));

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }
        public IDataReader KontrolKimlikNoKayitVarMi(HibeSulama bilgi, int hukukiStatu, int sulamaGrubuKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            string baglac = "WHERE";
            if (hukukiStatu >= (int)ENUMHibeSulamaHukukiStatu.KOOPERATIF)
            {
                sql += " INNER JOIN " + TABLOADDETAY;
                sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD];
                sql = VeriErisim.KosulEkle(sql, baglac, VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SULAMAGRUBUKOD], "=", sulamaGrubuKod));
                baglac = "AND";
            }
            sql = VeriErisim.KosulEkle(sql, baglac, VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));

            if (bilgi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.kimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", bilgi.kimlikNo));

            if (!string.IsNullOrEmpty(bilgi.ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KontrolEvrakKaydi(HibeSulama bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));

            //if (bilgi.kod > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.evrakNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKNO], "=", bilgi.evrakNo));

            if (!string.IsNullOrEmpty(bilgi.ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KontrolParsel(HibeSulama bilgi, string parseller, int kosulYil, int etapNo)
        {
            string sql = "SELECT TRMHIBESULAMA.ETAPNO,TRMHIBESULAMA.KIMLIKNO, TRMHIBESULAMA.ILKOD, TRMHIBESULAMA.ILCEKOD, TRMHIBESULAMA.KOYKOD, TRMHIBESULAMADIGER.PARSELNO";
            sql += " FROM TRMHIBESULAMA ";
            sql += " INNER JOIN TRMHIBESULAMADIGER ON TRMHIBESULAMA.KOD = TRMHIBESULAMADIGER.HIBEKOD";
            sql += " WHERE TRMHIBESULAMA.DURUM>5 AND TRMHIBESULAMA.DURUM<90 ";
            sql += " AND TRMHIBESULAMADIGER.TIP < 999 ";
            sql += " AND TRMHIBESULAMADIGER.PARSELNO NOT IN ('','ParselNo')";
            sql += " AND TRMHIBESULAMA.ETAPNO>=" + (etapNo - kosulYil);
            sql += " AND TRMHIBESULAMA.KIMLIKNO='" + bilgi.kimlikNo + "'";
            sql += " AND TRMHIBESULAMA.ILKOD='" + bilgi.ilKod + "'";
            sql += " AND TRMHIBESULAMA.ILCEKOD='" + bilgi.ilceKod + "'";
            sql += " AND TRMHIBESULAMA.KOYKOD=" + bilgi.koyKod;
            sql += " AND TRMHIBESULAMADIGER.PARSELNO IN (" + parseller + ")";

            if (bilgi.kod != 0)
                sql += " AND TRMHIBESULAMA.KOD<>" + bilgi.kod;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KontrolOncedenReddilenDestek(HibeSulama bilgi, int kosulYil, int etapNo)
        {
            string sql = "SELECT TRMHIBESULAMA.ETAPNO,TRMHIBESULAMA.KIMLIKNO, TRMHIBESULAMA.ILKOD, TRMHIBESULAMA.ILCEKOD, TRMHIBESULAMA.KOYKOD";//, TRMHIBESULAMADIGER.PARSELNO";
            sql += " FROM TRMHIBESULAMA ";
            //sql += " INNER JOIN TRMHIBESULAMADIGER ON TRMHIBESULAMA.KOD = TRMHIBESULAMADIGER.HIBEKOD";
            sql += " WHERE TRMHIBESULAMA.DURUM=99 ";
            sql += " AND TRMHIBESULAMA.REDTIP=3";
            //sql += " AND TRMHIBESULAMADIGER.TIP < 999 ";
            //sql += " AND TRMHIBESULAMADIGER.PARSELNO NOT IN ('','ParselNo')";
            sql += " AND TRMHIBESULAMA.ETAPNO>=" + (etapNo - kosulYil);
            sql += " AND TRMHIBESULAMA.KIMLIKNO='" + bilgi.kimlikNo + "'";
            //sql += " AND TRMHIBESULAMA.ILKOD='" + bilgi.ilKod + "'";
            //sql += " AND TRMHIBESULAMA.ILCEKOD='" + bilgi.ilceKod + "'";
            //sql += " AND TRMHIBESULAMA.KOYKOD=" + bilgi.koyKod;
            //sql += " AND TRMHIBESULAMADIGER.PARSELNO IN (" + parseller + ")";

            if (bilgi.kod != 0)
                sql += " AND TRMHIBESULAMA.KOD<>" + bilgi.kod;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hibe Sulama Detay bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(HibeSulamaDetay bilgi, int hibeKod)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.satirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.sulamaGrubuKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.proformaTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.malzemeSeriNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firmaVergiNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firmaAd) + ",";
            sql += VeriErisim.DegerYap(bilgi.malzemeTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeEsasTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.ayniKatki) + ",";
            sql += VeriErisim.DegerYap(bilgi.toplamTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeEsasTutarProje) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeEsasTutarUygulama) + ",";
            sql += VeriErisim.DegerYap(bilgi.gercekFatura) + ",";
            sql += VeriErisim.DegerYap(bilgi.gercekHibeTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.eksikHibeTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.eksikOdemeOdenecek) + ",";
            sql += VeriErisim.DegerYap(bilgi.eksikOdemeDuzeltildi) + ",";
            sql += VeriErisim.DegerYap(bilgi.eksikFazlaOdemeAciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.proformaReferansTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.ayniKatkiProje) + ",";
            sql += VeriErisim.DegerYap(bilgi.basvuruToplamMalAlimTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.basvuruHibeyeEsasMalAlimTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.basvuruAyniKatkiTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.basvuruTalepEdilenHibeTutar) + ")";

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Hibe Sulama Detay fatura bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetFaturaDetay(HibeSulamaDetay bilgi, int hibeKod)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMESERINO], bilgi.malzemeSeriNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAVERGINO], bilgi.firmaVergiNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAAD], bilgi.firmaAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMETUTAR], bilgi.malzemeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ACIKLAMA], bilgi.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR], bilgi.hibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI], bilgi.ayniKatki) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.GERCEKFATURA], bilgi.gercekFatura) + ", ";

            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PROFORMATUTAR], bilgi.proformaTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTARUYGULAMA], bilgi.hibeEsasTutarUygulama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PROFORMAREFERANSTUTAR], bilgi.proformaReferansTutar) + ", ";


            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR], bilgi.toplamTutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO], "=", bilgi.satirNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleDetay(HibeSulamaDetay bilgi, int hibeKod, ENUMHibeSulamaMalzemeTip tip)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";

            if (tip == ENUMHibeSulamaMalzemeTip.PROJE)
            {
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR], bilgi.hibeTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR], bilgi.toplamTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI], bilgi.ayniKatki) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PROFORMATUTAR], bilgi.proformaTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTARPROJE], bilgi.hibeEsasTutarProje) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PROFORMAREFERANSTUTAR], bilgi.proformaReferansTutar);
            }
            else
            {
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR], bilgi.hibeTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR], bilgi.toplamTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI], bilgi.ayniKatki) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMETUTAR], bilgi.malzemeTutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.GERCEKFATURA], bilgi.gercekFatura) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTARUYGULAMA], bilgi.hibeEsasTutarUygulama);
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Hibe Sulama Detay bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek hibe sulama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(int hibeKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", hibeKod));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Hibe Sulama Detay bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hibe sulama bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hibe Sulama Detay bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleDetay(HibeSulamaDetay bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) +
                ", " + TABSulamaGrubu.TABLOAD + "." + TABSulamaGrubu.ALANLAR[(int)TABSulamaGrubu.ENUMAlanlar.AD] + " AS GRUPAD" +
                ", " + TABSulamaGrubu.TABLOAD + "." + TABSulamaGrubu.ALANLAR[(int)TABSulamaGrubu.ENUMAlanlar.HIBEORAN] +
                " FROM " + TABLOADDETAY + " LEFT OUTER JOIN " +
                TABSulamaGrubu.TABLOAD + " ON " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SULAMAGRUBUKOD] +
                " = " + TABSulamaGrubu.TABLOAD + "." + TABSulamaGrubu.ALANLAR[(int)TABSulamaGrubu.ENUMAlanlar.KOD];

            string kosul = string.Empty;
            if (bilgi.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", bilgi.hibeKod));
            if (bilgi.satirNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO], "=", bilgi.satirNo));
            if (bilgi.sulamaGrubuKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SULAMAGRUBUKOD], "=", bilgi.sulamaGrubuKod));
            if (bilgi.proformaTutar > decimal.Zero)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.PROFORMATUTAR], "=", bilgi.proformaTutar));
            if (!string.IsNullOrEmpty(bilgi.malzemeSeriNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMESERINO], "=", bilgi.malzemeSeriNo));
            if (!string.IsNullOrEmpty(bilgi.firmaVergiNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAVERGINO], "=", bilgi.firmaVergiNo));
            if (!string.IsNullOrEmpty(bilgi.firmaAd))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAAD], "=", bilgi.firmaAd));
            if (bilgi.malzemeTutar > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMETUTAR], "=", bilgi.malzemeTutar));
            if (!string.IsNullOrEmpty(bilgi.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ACIKLAMA], "=", bilgi.aciklama));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD] + "," + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO];

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// Hibe Sulama Tarihçe bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek tarihçe bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetTarihce(HibeSulamaTarihce bilgi)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kullaniciKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Hibe Sulama Tarihçe bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak hibe sulama tarihçe bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hibe Sulama Tarihçe bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTarihce(HibeSulamaTarihce kriter)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE) + " FROM  " + TABLOADTARIHCE;

            string kosul = string.Empty;
            if (kriter.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.HIBEKOD], "=", kriter.hibeKod));
            if (!string.IsNullOrEmpty(kriter.kullaniciKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KULLANICIKOD], "=", kriter.kullaniciKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader KimlikBilgisiEksikBasvurular()
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KOD] + ", " + ALANLAR[(int)ENUMAlanlar.KIMLIKNO];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, "LEN(" + ALANLAR[(int)ENUMAlanlar.KIMLIKNO] + ")", "=", 11));
            sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.AD], "=", string.Empty));
            sql = VeriErisim.KosulEkle(sql, "OR", ALANLAR[(int)ENUMAlanlar.AD] + " IS NULL");
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOYAD], "=", string.Empty));
            sql = VeriErisim.KosulEkle(sql, "OR", ALANLAR[(int)ENUMAlanlar.SOYAD] + " IS NULL");
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BABAAD], "=", string.Empty));
            sql = VeriErisim.KosulEkle(sql, "OR", ALANLAR[(int)ENUMAlanlar.BABAAD] + " IS NULL");
            sql = VeriErisim.KosulEkle(sql, "OR", ALANLAR[(int)ENUMAlanlar.DOGUMTARIH] + " IS NULL)");

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc KimlikBilgisiEksikGuncelle(HibeSulama hm)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.AD], hm.ad);
            sql += ", " + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SOYAD], hm.soyad);
            sql += ", " + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BABAAD], hm.babaAd);
            sql += ", " + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.DOGUMTARIH], hm.dogumTarih);
            sql += ", " + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ADRES], hm.adres);
            sql += ", " + VeriErisim.AtamaOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KASABA], "1");
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", hm.kod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc KaydetHibeTutarlari(HibeSulamaDetay bilgi, int hibeKod)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR], bilgi.hibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI], bilgi.ayniKatki) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR], bilgi.toplamTutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO], "=", bilgi.satirNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetDegerlendirmeSonuc(int hibeKod, double anketPuan)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PUAN], anketPuan);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetVarYokSonuc(int hibeKod, double anketPuan)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VARYOK], anketPuan);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader HazineRaporTuzel(int yil, int ay, int tur, int etapNo)
        {
            TNSDateTime baslama = new TNSDateTime("01." + ay + "." + yil);
            TNSDateTime bitis = new TNSDateTime(baslama.Oku().AddMonths(1));

            string sozlesmeTur = "";
            if (tur == 2) sozlesmeTur = ",SOZLESMETUR";

            string sql = "SELECT  PROJENO, KIMLIKNO, ILKOD, ILCEKOD, YETSEKKOD, MIN(SOZLESMETARIH), MIN(GERSURE), TARIH1, TARIH2, DESTEKUNSUR, DURUM, ANAHTAR, AD, SUM(HIBEESASTUTAR), SUM(HIBETUTAR)" + sozlesmeTur + " FROM HIBESULAMAHAZINETUZEL";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "GERSURE", ">=", baslama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GERSURE", "<", bitis));
            //if (etapNo > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ETAPNO", "=", etapNo));

            sql += " GROUP BY PROJENO, KIMLIKNO, ILKOD, ILCEKOD, DESTEKUNSUR, TARIH1, TARIH2, YETSEKKOD, DURUM, ANAHTAR, AD" + sozlesmeTur;
            sql += " ORDER BY PROJENO, ILKOD, ILCEKOD, KIMLIKNO" + sozlesmeTur;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HazineRaporGercek(int yil, int ay, int tur, int etapNo)
        {
            TNSDateTime baslama = new TNSDateTime("01." + ay + "." + yil);
            TNSDateTime bitis = new TNSDateTime(baslama.Oku().AddMonths(1));

            string sozlesmeTur = "";
            if (tur == 2) sozlesmeTur = ",SOZLESMETUR";

            string sql = "SELECT  PROJENO, KIMLIKNO, ILKOD, ILCEKOD, YETSEKKOD, MIN(SOZLESMETARIH), MIN(GERSURE), TARIH1, TARIH2, DESTEKUNSUR, DURUM, ANAHTAR, AD, SUM(HIBEESASTUTAR), SUM(HIBETUTAR)" + sozlesmeTur + " FROM HIBESULAMAHAZINEGERCEK";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "GERSURE", ">=", baslama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GERSURE", "<", bitis));
            //if (etapNo > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ETAPNO", "=", etapNo));
            sql += " GROUP BY PROJENO, KIMLIKNO, ILKOD, ILCEKOD, DESTEKUNSUR, TARIH1, TARIH2, YETSEKKOD, DURUM, ANAHTAR, AD" + sozlesmeTur;
            sql += " ORDER BY PROJENO, ILKOD, ILCEKOD, KIMLIKNO" + sozlesmeTur;

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleKabulCalismasiIlToplami(HibeSulama kriter)
        {
            string sql = "SELECT TRMHIBESULAMA.ILKOD, SUM(TRMHIBESULAMADETAY.HIBETUTAR) FROM TRMHIBESULAMA";
            sql += " INNER JOIN TRMHIBESULAMADETAY ON TRMHIBESULAMADETAY.HIBEKOD = TRMHIBESULAMA.KOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", kriter.etapNo));

            if (kriter.ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", kriter.ilKod.Trim().Replace(".", "")));

            sql = VeriErisim.KosulEkle(sql, "AND", "(DURUM <> 99) AND (KABULEDILDI = 'K')");
            sql += "GROUP BY ILKOD";

            return veriErisim.DataReaderDondur(sql);
        }


        public IDataReader KonusunaGoreDagilimRapor(HibeSulama kriter)
        {
            string sql = "SELECT ORGCOGRAFYA.AD,ORGCOGRAFYA2.AD,ORGCOGRAFYA3.ADI, TRMSULAMAGRUBU.AD AS Expr1, TRMHIBESULAMADIGER.PARSELSULANACAKALAN, ";
            sql += " (HIBESULAMAMALZEMEHIBETUTAR.HIBEESASTUTAR-TRMHIBESULAMADETAY.BASVURUAYNIKATKITUTAR)/2, TRMHIBESULAMA.ETAPNO, TRMHIBESULAMADIGER.PARSELNO";
            sql += " FROM HIBESULAMAMALZEMEHIBETUTAR AS HIBESULAMAMALZEMEHIBETUTAR";
            sql += " INNER JOIN TRMHIBESULAMA AS TRMHIBESULAMA ON HIBESULAMAMALZEMEHIBETUTAR.HIBEKOD = TRMHIBESULAMA.KOD";
            sql += " INNER JOIN TRMSULAMAGRUBU AS TRMSULAMAGRUBU ON HIBESULAMAMALZEMEHIBETUTAR.SULAMAGRUPKOD = TRMSULAMAGRUBU.KOD";
            sql += " INNER JOIN ORGCOGRAFYA AS ORGCOGRAFYA ON TRMHIBESULAMA.ILKOD = ORGCOGRAFYA.KOD";
            sql += " INNER JOIN ORGCOGRAFYA AS ORGCOGRAFYA2 ON TRMHIBESULAMA.ILCEKOD = ORGCOGRAFYA2.KOD";
            sql += " INNER JOIN TRMKOY AS ORGCOGRAFYA3 ON TRMHIBESULAMA.KOYKOD = ORGCOGRAFYA3.KOD";
            sql += " INNER JOIN TRMHIBESULAMADIGER ON HIBESULAMAMALZEMEHIBETUTAR.HIBEKOD = TRMHIBESULAMADIGER.HIBEKOD AND HIBESULAMAMALZEMEHIBETUTAR.SULAMAGRUPKOD = TRMHIBESULAMADIGER.SULAMAGRUPKOD AND HIBESULAMAMALZEMEHIBETUTAR.PARSELID = TRMHIBESULAMADIGER.PARSELID";
            sql += " INNER JOIN dbo.TRMHIBESULAMADETAY ON TRMHIBESULAMA.KOD = dbo.TRMHIBESULAMADETAY.HIBEKOD";

            sql += " WHERE";


            if (kriter.etapNo > 0 && kriter.etapNo != 999)
                sql += " (TRMHIBESULAMA.ETAPNO = " + kriter.etapNo + ") AND";

            if (kriter.ilAd != "")
                sql += " (ORGCOGRAFYA.AD = '" + kriter.ilAd + "') AND";

            if (kriter.grupKod != "")
                sql += " (TRMSULAMAGRUBU.KOD = " + kriter.grupKod + ") AND";

            sql += " (TRMHIBESULAMA.DURUM >= 20) AND";
            sql += " (TRMHIBESULAMA.DURUM < 99) AND";
            sql += " (HIBESULAMAMALZEMEHIBETUTAR.TIP = 2) AND";
            sql += " (HIBESULAMAMALZEMEHIBETUTAR.SULAMAGRUPKOD > 0)";
            sql += " ORDER BY ORGCOGRAFYA.AD,ORGCOGRAFYA2.AD,ORGCOGRAFYA3.ADI, Expr1, TRMHIBESULAMADIGER.PARSELNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BilgiNotuRapor()
        {
            string sql = "SELECT TRMHIBESULAMA.ETAPNO, ORGCOGRAFYA.KOD, ORGCOGRAFYA.AD, TRMHIBESULAMA.DURUM, COUNT(*) AS ADET, SUM(ISNULL(TRMHIBESULAMA.SULANACAKALAN, 0)) AS ALAN, SUM(TRMHIBESULAMADETAY.HIBETUTAR) AS HIBETUTAR";
            sql += " FROM TRMHIBESULAMADETAY";
            sql += " INNER JOIN TRMHIBESULAMA ON TRMHIBESULAMADETAY.HIBEKOD = TRMHIBESULAMA.KOD ";
            sql += " INNER JOIN ORGCOGRAFYA ON TRMHIBESULAMA.ILKOD = ORGCOGRAFYA.KOD";
            sql += " WHERE (TRMHIBESULAMA.DURUM <= 20)";
            sql += " GROUP BY TRMHIBESULAMA.ETAPNO, ORGCOGRAFYA.KOD, ORGCOGRAFYA.AD, TRMHIBESULAMA.DURUM";
            sql += " ORDER BY TRMHIBESULAMA.ETAPNO, ORGCOGRAFYA.KOD, ORGCOGRAFYA.AD, TRMHIBESULAMA.DURUM";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KonuIstatistik(HibeSulama kriter)
        {
            string sql = "";
            //sql = "SELECT TRMHIBESULAMA.ETAPNO, TRMHIBESULAMADETAY.SULAMAGRUBUKOD, COUNT(*) AS ADET, SUM(ISNULL(TRMHIBESULAMA.SULANACAKALAN, 0)) AS ALAN, SUM(TRMHIBESULAMADETAY.HIBETUTAR) AS HIBETUTAR";
            //sql += " FROM TRMHIBESULAMADETAY";
            //sql += " INNER JOIN TRMHIBESULAMA ON TRMHIBESULAMADETAY.HIBEKOD = TRMHIBESULAMA.KOD ";
            //sql += " WHERE (TRMHIBESULAMA.DURUM = 20)";

            //if (kriter.ilKod != "")
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMA", "ILKOD", "=", kriter.ilKod.Trim().Replace(".", "")));

            //sql += " GROUP BY TRMHIBESULAMA.ETAPNO,TRMHIBESULAMADETAY.SULAMAGRUBUKOD";
            //sql += " ORDER BY TRMHIBESULAMA.ETAPNO,TRMHIBESULAMADETAY.SULAMAGRUBUKOD";


            sql += " SELECT TRMHIBESULAMA.ETAPNO, TRMSULAMAGRUBU.KOD SULAMAGRUBUKOD, count(*) ADET, sum(TRMHIBESULAMADIGER.PARSELSULANACAKALAN) ALAN, ";
            sql += " sum(HIBESULAMAMALZEMEHIBETUTAR.HIBEESASTUTAR - TRMHIBESULAMADETAY.BASVURUAYNIKATKITUTAR) / 2 HIBETUTAR";
            sql += " FROM HIBESULAMAMALZEMEHIBETUTAR";
            sql += " INNER JOIN TRMHIBESULAMA ON HIBESULAMAMALZEMEHIBETUTAR.HIBEKOD = TRMHIBESULAMA.KOD";
            sql += " INNER JOIN TRMSULAMAGRUBU ON HIBESULAMAMALZEMEHIBETUTAR.SULAMAGRUPKOD = TRMSULAMAGRUBU.KOD";
            sql += " INNER JOIN TRMHIBESULAMADIGER ON HIBESULAMAMALZEMEHIBETUTAR.HIBEKOD = TRMHIBESULAMADIGER.HIBEKOD AND HIBESULAMAMALZEMEHIBETUTAR.SULAMAGRUPKOD = TRMHIBESULAMADIGER.SULAMAGRUPKOD AND HIBESULAMAMALZEMEHIBETUTAR.PARSELID = TRMHIBESULAMADIGER.PARSELID";
            sql += " INNER JOIN dbo.TRMHIBESULAMADETAY ON TRMHIBESULAMA.KOD = dbo.TRMHIBESULAMADETAY.HIBEKOD";
            sql += " WHERE(TRMHIBESULAMA.DURUM = 20) and(HIBESULAMAMALZEMEHIBETUTAR.TIP = 2) AND(HIBESULAMAMALZEMEHIBETUTAR.SULAMAGRUPKOD > 0)";
            if (kriter.ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMA", "ILKOD", "=", kriter.ilKod.Trim().Replace(".", "")));
            if (kriter.ilceKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMA", "ILCEKOD", "=", kriter.ilceKod.Trim().Replace(".", "")));
            sql += " group by TRMHIBESULAMA.ETAPNO,TRMSULAMAGRUBU.KOD";
            sql += " order by TRMHIBESULAMA.ETAPNO,TRMSULAMAGRUBU.KOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Istatistik(HibeSulama kriter)
        {
            string sql = "";
            sql += " SELECT TRMHIBESULAMA.ETAPNO, ORGCOGRAFYA.AD AS IL, COUNT(*) AS ADET, SUM(TRMHIBESULAMADIGER.PARSELSULANACAKALAN) AS ALAN, SUM(HIBESULAMAMALZEMEHIBETUTAR.HIBEESASTUTAR - TRMHIBESULAMADETAY.BASVURUAYNIKATKITUTAR) / 2 AS HIBETUTAR";
            sql += " FROM ORGCOGRAFYA";
            sql += " INNER JOIN HIBESULAMAMALZEMEHIBETUTAR";
            sql += " INNER JOIN TRMHIBESULAMA ON HIBESULAMAMALZEMEHIBETUTAR.HIBEKOD = TRMHIBESULAMA.KOD";
            sql += " INNER JOIN TRMHIBESULAMADIGER ON HIBESULAMAMALZEMEHIBETUTAR.HIBEKOD = TRMHIBESULAMADIGER.HIBEKOD AND HIBESULAMAMALZEMEHIBETUTAR.SULAMAGRUPKOD = TRMHIBESULAMADIGER.SULAMAGRUPKOD AND HIBESULAMAMALZEMEHIBETUTAR.PARSELID = TRMHIBESULAMADIGER.PARSELID";
            sql += " INNER JOIN TRMHIBESULAMADETAY ON TRMHIBESULAMA.KOD = TRMHIBESULAMADETAY.HIBEKOD ON ORGCOGRAFYA.KOD = TRMHIBESULAMA.ILKOD";
            sql += " WHERE (TRMHIBESULAMA.DURUM = 20) AND (HIBESULAMAMALZEMEHIBETUTAR.TIP = 2) AND (HIBESULAMAMALZEMEHIBETUTAR.SULAMAGRUPKOD > 0)";

            if (kriter.ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMA", "ILKOD", "=", kriter.ilKod.Trim().Replace(".", "")));

            sql += " GROUP BY TRMHIBESULAMA.ETAPNO, ORGCOGRAFYA.AD";
            sql += " ORDER BY ORGCOGRAFYA.AD,TRMHIBESULAMA.ETAPNO";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
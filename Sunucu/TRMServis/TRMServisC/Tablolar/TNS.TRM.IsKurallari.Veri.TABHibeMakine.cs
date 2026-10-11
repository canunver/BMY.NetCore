using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeMakine : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeMakine(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// hibe makine bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBEMAKINA";
        /// <summary>
        /// hibe makine Detay bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "TRMHIBEMAKINADETAY";
        /// <summary>
        /// hibe makine Tarihçe bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTARIHCE = "TRMHIBEMAKINATARIHCE";

        /// <summary>
        /// hibe makine bilgisi tablo alanlarının sırasını tutan enumeration
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

            OZELLIK = 29,
            PUAN = 30,
            PUANORJINAL = 31,

        }

        /// <summary>
        /// hibe makine tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "YIL", "BASVURUNO", "ILKOD", "ILCEKOD", "KOYKOD", "KASABA", "HUKUKISTATU", "KIMLIKNO", "VERGIDAIRE", "AD", "EVRAKNO", "EVRAKTARIH", "ADRES", "TELEFON", "FAKS", "EPOSTA", "BANKASUBEKOD", "DURUM", "SOZLESMETARIH", "FATURANO", "FATURATARIH", "SOYAD", "BABAAD", "DOGUMTARIH", "YERINDETESPITTARIH", "PROJENO", "GRUPKOD", "ETAPNO", "OZELLIK", "PUAN", "PUANORJINAL" };

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
            /// MALZEMEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMEKOD = 2,
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
            REFERANSTUTAR = 13,
            /// <summary>
            /// ADET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADET = 14,
            /// <summary>
            /// GERCEKHIBETUTARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>

            GERCEKFATURA = 15,

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
            EKSIKODEMEACIKLAMA = 20
        }

        /// <summary>
        /// hibe makine Detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARDETAY = { "HIBEKOD", "SATIRNO", "MALZEMEKOD", "PROFORMATUTAR", "MALZEMESERINO", "FIRMAVERGINO", "FIRMAAD", "MALZEMETUTAR", "ACIKLAMA", "HIBETUTAR", "HIBEESASTUTAR", "AYNIKATKI", "TOPLAMTUTAR", "REFERANSTUTAR", "ADET", "GERCEKFATURA", "GERCEKHIBETUTARI", "EKSIKHIBETUTARI", "EKSIKODEMEODENECEK", "EKSIKODEMEDUZELTILDI", "EKSIKODEMEACIKLAMA" };

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
            ACIKLAMA = 4,
        }

        /// <summary>
        /// hibe makine tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTARIHCE = { "HIBEKOD", "KULLANICIKOD", "TARIH", "DURUM", "ACIKLAMA" };

        /// <summary>
        /// hibe makine bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(HibeMakine bilgi)
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
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OZELLIK], bilgi.ozellik) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOGUMTARIH], bilgi.dogumTarih);
            if (bilgi.durum != (int)ENUMHibeMakineBasvuruDurum.TANIMSIZ)
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Kaydet(HibeMakine bilgi)
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
            sql += VeriErisim.DegerYap(bilgi.ozellik) + ",";
            sql += VeriErisim.DegerYap(bilgi.puan) + ",";
            sql += VeriErisim.DegerYap(bilgi.puanOrjinal) + ")";

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// hibe makine bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSozlesme(HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], bilgi.sozlesmeTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        /// <summary>
        /// hibe makine bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc BasvuruyuIslemeAl(int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMHibeMakineBasvuruDurum.YENI);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        /// <summary>
        /// hibe makine bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetProjeNo(HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], bilgi.projeNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// hibe makine bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetEksikHibeMiktar(HibeMakineDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.GERCEKFATURA], bilgi.gercekFatura) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.GERCEKHIBETUTARI], bilgi.gercekHibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.EKSIKHIBETUTARI], bilgi.eksikHibeTutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.EKSIKODEMEODENECEK], bilgi.gercekHibeTutar - bilgi.eksikHibeTutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.EKSIKODEMEACIKLAMA], bilgi.eksikFazlaOdemeAciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", bilgi.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO], "=", bilgi.satirNo));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        public Sonuc KaydetReddedildi(HibeMakine bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], string.Empty) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc YatirimciDegistir(HibeMakine bilgi)
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
        public Sonuc KaydetDurumDegistir(int hibeKod, int durum)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "NOT LIKE", "%x"));

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// hibe makine bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetFatura(HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKASUBEKOD], bilgi.bankaSubeKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], bilgi.faturaNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURATARIH], bilgi.faturaTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        /// <summary>
        /// hibe makine bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetYerindeTespit(HibeMakine bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERINDETESPITTARIH], bilgi.yerindeTespitTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        ////Grup İşlemleri**************************
        public Sonuc KaydetGrup(HibeMakine bilgi)
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
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], "");
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
        public IDataReader HibeMakineListeleGrupBazliToplam(int etepNo, int durum, int tur = -1, params string[] yetkiliIller)
        {
            //EtapNo parametre olarak alınıyor fakat sorguya konulmadı. Melih 12.02.2014

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR);
            sql += ", ORGCOGRAFYA.AD";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR] + ")";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.EKSIKODEMEODENECEK] + ")";
            sql += " FROM " + TABHibeMakineSulamaGrup.TABLOAD + ", " + TABLOAD + ", " + TABLOADDETAY + ", ORGCOGRAFYA";
            sql += " WHERE ";
            if (tur == 1 || tur == 2)
                sql += TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GRUPKOD];
            if (tur == 3 || tur == 4)
                sql += TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASVURUNO];//Eksik ödeme yapılanlar için grup kodu değil başvuru numarası yazılıyor
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILKOD] + "=" + "ORGCOGRAFYA.KOD";
            if (durum != (int)ENUMHibeMakineBasvuruDurum.TANIMSIZ)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], "=", (int)durum));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], ">=", (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], "<", (int)ENUMHibeMakineBasvuruDurum.ODENDI));
            }

            if (tur > -1)
            {
                if (tur == 1 || tur == 3)//GAP ve GAP Eksik Ödeme
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "1"));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
            }

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.ODEMEKOD], "=", 0));

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
                    sql = VeriErisim.KosulEkle(sql, "AND", TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.ILKOD] + " IN (" + ilKosul + ")");
            }

            sql += " GROUP BY ORGCOGRAFYA.AD," + VeriErisim.TumAlanlar(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR);
            sql += " ORDER BY ORGCOGRAFYA.AD";
            sql += ", " + TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc KaydetTarihceGruptakiler(string grupKod, HibeMakineTarihce tarihce)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE;
            sql += " SELECT " + ALANLAR[(int)ENUMAlanlar.KOD];
            sql += ", " + VeriErisim.DegerYap(tarihce.tarih);
            sql += ", " + VeriErisim.DegerYap(tarihce.kullaniciKod);
            sql += ", " + VeriErisim.DegerYap(tarihce.durum);
            sql += ", " + VeriErisim.DegerYap(tarihce.aciklama);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", grupKod));
            sql = VeriErisim.KosulEkle(sql, "or", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BASVURUNO], "=", grupKod));//Eksik ödeme işlemi yapılırken gerekli

            return KomutCalistir(sql, 0);
        }
        ////Grup İşlemleri**************************

        /// <summary>
        /// hibe makine bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(HibeMakine bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }
        public int ListeleDurum(int hibeKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return veriErisim.TekKolonSQLInt(sql);
        }
        public double HibeMakineOdemeMiktarGetir(int tur)
        {
            string sql = "SELECT SUM(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR] + ") FROM " + TABLOAD +
                " INNER JOIN " + TABHibeMakineSulamaGrup.TABLOAD + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GRUPKOD] + "=" + TABHibeMakineSulamaGrup.TABLOAD + "." + TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.KOD] +
                " INNER JOIN " + TABLOADDETAY + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD] +
                " INNER JOIN ORGCOGRAFYA ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILKOD] + "=" + "ORGCOGRAFYA.KOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.DURUM], "=", (int)ENUMHibeMakineBasvuruDurum.ODEMEBASLAT));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABHibeMakineSulamaGrup.TABLOAD, TABHibeMakineSulamaGrup.ALANLAR[(int)TABHibeMakineSulamaGrup.ENUMAlanlar.ODEMEKOD], "=", 0));

            if (tur > -1)
            {
                if (tur == 1 || tur == 3)//GAP ve GAP Eksik Ödeme
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", "1"));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
            }

            return veriErisim.TekKolonSQLDbl(sql);
        }
        /// <summary>
        /// hibe makine bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hibe makine bilgisi kıstas parametresi</param>
        /// <returns>
        /// hibe makine bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(HibeMakine bilgi, string orderByKolon, bool asc, int kayitSay)
        {
            string top = "";
            if (kayitSay > 0) top = " TOP " + kayitSay;

            string sql = "SELECT " + top + " " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + ", TRMMALZEME.MALZEMEGRUBUKOD AS GRUPKOD, TRMMALZEMEGRUBU.AD AS GRUPAD, TRMMALZEME.AD AS MALZEME, TRMMALZEMEGRUBU.HIBEORAN, TRMKOY.ADI, ORGCOGRAFYA.AD AS ILAD, ORGCOGRAFYA_1.AD AS ILCEAD";
            sql += " FROM ";
            sql += " TRMMALZEMEGRUBU INNER JOIN";
            sql += " TRMMALZEME ON TRMMALZEMEGRUBU.KOD = TRMMALZEME.MALZEMEGRUBUKOD INNER JOIN";
            sql += " TRMHIBEMAKINADETAY ON TRMMALZEME.KOD = TRMHIBEMAKINADETAY.MALZEMEKOD RIGHT OUTER JOIN";
            sql += " TRMHIBEMAKINA ON TRMHIBEMAKINADETAY.HIBEKOD = TRMHIBEMAKINA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEMAKINA.ILKOD = ORGCOGRAFYA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEMAKINA.ILCEKOD = ORGCOGRAFYA_1.KOD LEFT OUTER JOIN";
            sql += " TRMKOY ON TRMHIBEMAKINA.KOYKOD = TRMKOY.KOD";

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

            veriErisim.sureAsimi = 9000;

            return veriErisim.DataReaderDondur(sql);
        }
        public int HibeMakineBasvuruSayiDondur(HibeMakine bilgi)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) ";
            sql += " FROM ";
            sql += " TRMMALZEMEGRUBU INNER JOIN";
            sql += " TRMMALZEME ON TRMMALZEMEGRUBU.KOD = TRMMALZEME.MALZEMEGRUBUKOD INNER JOIN";
            sql += " TRMHIBEMAKINADETAY ON TRMMALZEME.KOD = TRMHIBEMAKINADETAY.MALZEMEKOD RIGHT OUTER JOIN";
            sql += " TRMHIBEMAKINA ON TRMHIBEMAKINADETAY.HIBEKOD = TRMHIBEMAKINA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEMAKINA.ILKOD = ORGCOGRAFYA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEMAKINA.ILCEKOD = ORGCOGRAFYA_1.KOD LEFT OUTER JOIN";
            sql += " TRMKOY ON TRMHIBEMAKINA.KOYKOD = TRMKOY.KOD";

            string kosul = ListeleKriterOl(bilgi);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLInt(sql);
        }
        public IDataReader AktifEtapdaKimlikNoVarmi(string kimlikNo, int etapNo, int tip)
        {
            string tablo = "";
            string sql = "";

            if (tip != 0)
            {
                switch (tip)
                {
                    case 1:
                        tablo = "TRMHIBEMAKINA";
                        break;
                    case 2:
                        tablo = "TRMHIBESULAMA";
                        break;
                    case 3:
                        tablo = "TRMHIBEYATIRIM";
                        break;
                }

                sql = "SELECT KOD, DURUM, HUKUKISTATU";
                sql += " FROM " + tablo;
                sql += " WHERE KIMLIKNO = '" + kimlikNo + "' and ETAPNO = '" + etapNo + "'";
            }

            return veriErisim.DataReaderDondur(sql);
        }
        private string ListeleKriterOl(HibeMakine bilgi)
        {
            string kosul = string.Empty;
            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.etapNolar != null && bilgi.etapNolar.Length > 1)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ETAPNO] + " IN (" + String.Join(",", bilgi.etapNolar) + ")");
            }
            else if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
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
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            }
            if (!string.IsNullOrEmpty(bilgi.basvuruNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASVURUNO], "=", bilgi.basvuruNo));
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
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + ilKosul + ")");
            }
            if (!string.IsNullOrEmpty(bilgi.ilceKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCEKOD], "=", bilgi.ilceKod.Replace(".", "")));
            if (bilgi.koyKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOYKOD], "=", bilgi.koyKod));
            if (!string.IsNullOrEmpty(bilgi.kasaba))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KASABA], "LIKE", "%" + bilgi.kasaba + "%"));
            if (bilgi.hukukiStatu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "=", bilgi.hukukiStatu));
            if (bilgi.ozellik > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OZELLIK], "=", bilgi.ozellik));
            if (bilgi.puan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], ">=", bilgi.puan));
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
            if (bilgi.durum > 0 || bilgi.durum == -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", -2));

            if (!bilgi.sozlesmeTarihBaslangic.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], ">=", bilgi.sozlesmeTarihBaslangic));
            if (!bilgi.sozlesmeTarihBitis.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], "<=", bilgi.sozlesmeTarihBitis));

            if (bilgi.detay != null && bilgi.detay.Length > 0)
            {
                if (bilgi.detay[0].malzemeGrubuKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMMALZEME", "MALZEMEGRUBUKOD", "=", bilgi.detay[0].malzemeGrubuKod));
                if (bilgi.detay[0].malzemeKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMEKOD], "=", bilgi.detay[0].malzemeKod));
                if (bilgi.detay[0].firmaVergiNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAVERGINO], "=", bilgi.detay[0].firmaVergiNo));
            }
            return kosul;
        }
        public IDataReader KontrolKimlikNoKayitVarMi(HibeMakine bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));

            if (bilgi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.kimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", bilgi.kimlikNo));

            return veriErisim.DataReaderDondur(sql);
        }
        /// <summary>
        /// hibe makine Detay bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(HibeMakineDetay bilgi, int hibeKod)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.satirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.malzemeKod) + ",";
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
            sql += VeriErisim.DegerYap(bilgi.referansTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.adet) + ",";
            sql += VeriErisim.DegerYap(bilgi.gercekFatura) + ",";
            sql += VeriErisim.DegerYap(bilgi.gercekHibeTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.eksikHibeTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.eksikOdemeOdenecek) + ",";
            sql += VeriErisim.DegerYap(bilgi.eksikOdemeDuzeltildi) + ",";
            sql += VeriErisim.DegerYap(bilgi.eksikFazlaOdemeAciklama) + ")";

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// hibe makine Detay bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleDetay(HibeMakineDetay bilgi)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAAD], bilgi.firmaAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAVERGINO], bilgi.firmaVergiNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMEKOD], bilgi.malzemeKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMESERINO], bilgi.malzemeSeriNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PROFORMATUTAR], bilgi.proformaTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.REFERANSTUTAR], bilgi.referansTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMETUTAR], bilgi.malzemeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR], bilgi.toplamTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI], bilgi.ayniKatki) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR], bilgi.hibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ADET], bilgi.adet);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", bilgi.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO], "=", bilgi.satirNo));
            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// hibe makine Detay fatura bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetFaturaDetay(HibeMakineDetay bilgi, int hibeKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMESERINO], bilgi.malzemeSeriNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAVERGINO], bilgi.firmaVergiNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FIRMAAD], bilgi.firmaAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMETUTAR], bilgi.malzemeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ACIKLAMA], bilgi.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR], bilgi.hibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI], bilgi.ayniKatki) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.REFERANSTUTAR], bilgi.referansTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR], bilgi.toplamTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ADET], bilgi.adet);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO], "=", bilgi.satirNo));
            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        /// <summary>
        /// hibe makine Detay HibeTutarlari bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetHibeTutarlari(HibeMakineDetay bilgi, int hibeKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBETUTAR], bilgi.hibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AYNIKATKI], bilgi.ayniKatki) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR], bilgi.toplamTutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SATIRNO], "=", bilgi.satirNo));
            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        /// <summary>
        /// hibe makine Detay bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(int hibeKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", hibeKod));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// hibe makine Tarihçe bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek tarihçe bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetTarihce(HibeMakineTarihce bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kullaniciKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }
        /// <summary>
        /// hibe makine Tarihçe bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak hibe makine tarihçe bilgisi kıstas parametresi</param>
        /// <returns>
        /// hibe makine Tarihçe bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTarihce(HibeMakineTarihce kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE) + " FROM  " + TABLOADTARIHCE;

            if (kriter.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.HIBEKOD], "=", kriter.hibeKod));
            if (!string.IsNullOrEmpty(kriter.kullaniciKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KULLANICIKOD], "=", kriter.kullaniciKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc MalzemeDegistir(HibeMakineDetay bilgi)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl(string.Empty, ALANLARDETAY[(int)ENUMAlanlarDetay.MALZEMEKOD], bilgi.malzemeKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARDETAY[(int)ENUMAlanlarDetay.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 1);
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
        public Sonuc KimlikBilgisiEksikGuncelle(HibeMakine hm)
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
        public decimal ReferansFiyatBul(int etapNo, string ilKod, int malzemeKod)
        {
            string sql = "SELECT FIYAT FROM TRMHIBEMAKINAREFFIYAT";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "ETAPNO", "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ILKOD", "=", ilKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MALZEMEKOD", "=", malzemeKod));

            return veriErisim.TekKolonSQLDecimal(sql);
        }

        public Sonuc KaydetDegerlendirmeSonuc(int hibeKod, double anketPuan, double puanOrjinal)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PUAN], anketPuan);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PUANORJINAL], puanOrjinal);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }

        public double ListeleDegerlendirmePuan(int hibeKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.PUANORJINAL] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader HibeMakineListForTKDK(string kimlikNo)
        {
            string sql = @"select TRMHIBEMAKINA.AD+' '+TRMHIBEMAKINA.SOYAD, HUKUKISTATU, ORGCOGRAFYA.AD, TRMHIBEMAKINA.ADRES ,TRMMALZEMEGRUBU.AD, TRMMALZEME.AD, 
                        TRMHIBEMAKINA.DURUM,TRMHIBEMAKINA.SOZLESMETARIH, TRMHIBEMAKINA.YIL, ETAPNO
                        from TRMHIBEMAKINA 
                        inner join TRMHIBEMAKINADETAY 
                        on TRMHIBEMAKINA.KOD = TRMHIBEMAKINADETAY.HIBEKOD
                        left outer join ORGCOGRAFYA 
                        on ORGCOGRAFYA.KOD = TRMHIBEMAKINA.ILKOD
                        left outer join TRMMALZEMEGRUBU
                        on TRMMALZEMEGRUBU.KOD = TRMHIBEMAKINADETAY.MALZEMEGRUBUKOD
                        left outer join TRMMALZEME
                        on TRMMALZEME.KOD = TRMHIBEMAKINADETAY.MALZEMEKOD
                        ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", kimlikNo));
            sql += " Order by ETAPNO desc";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HazineRaporTuzel(int yil, int ay)
        {
            TNSDateTime baslama = new TNSDateTime("01." + ay + "." + yil);
            TNSDateTime bitis = new TNSDateTime(baslama.Oku().AddMonths(1));

            string sql = "SELECT * FROM HIBEMAKINAHAZINETUZEL";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "GERSURE", ">=", baslama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GERSURE", "<", bitis));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HazineRaporGercek(int yil, int ay)
        {
            TNSDateTime baslama = new TNSDateTime("01." + ay + "." + yil);
            TNSDateTime bitis = new TNSDateTime(baslama.Oku().AddMonths(1));

            string sql = "SELECT  PROJENO, KIMLIKNO, ILKOD, ILCEKOD, YETSEKKOD, MIN(SOZLESMETARIH), MIN(GERSURE), TARIH1, TARIH2, DESTEKUNSUR, DURUM, ANAHTAR, AD, SUM(HIBEESASTUTAR), SUM(HIBETUTAR) FROM HIBEMAKINAHAZINEGERCEK";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "GERSURE", ">=", baslama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GERSURE", "<", bitis));
            sql += " GROUP BY PROJENO, KIMLIKNO, ILKOD, ILCEKOD, DESTEKUNSUR, TARIH1, TARIH2, YETSEKKOD, DURUM, ANAHTAR, AD";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

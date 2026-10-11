using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeYatirim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeYatirim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Hibe Yatırım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBEYATIRIM";
        /// <summary>
        /// Hibe Yatirim Tarihçe bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTARIHCE = "TRMHIBEYATIRIMTARIHCE";

        /// <summary>
        /// Hibe Yatirim bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// GRUPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GRUPKOD = 26,
            /// <summary>
            /// ETAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ETAPNO = 27,
            PROJENITELIK = 28,
            PROJEKONU = 29,
            PROJEALTKONU = 30,
            PROJEADI = 31,
            SOZLESMEMALIYET = 32,
            BASVURUMALIYET = 33,
            HIBETUTAR = 34,
            HIBEESASTUTAR = 35,
            AYNIKATKI = 36,
            TOPLAMTUTAR = 37,
            ANKET1 = 38,
            ANKET2 = 39,
            ANKET3 = 40,
            ANKET4 = 41,
            ANKET3PUAN = 42,
            ANKET4PUAN = 43,
            PUAN = 44,
            KONUNITELIKUYUSMA = 45,
            DEGERLENDIRMEDISI = 46,
            DEGERLENDIRMEDISIGEREKCE = 47,
            KABULCALISMASI = 48,
            ONLINETUZELBASVURUKIMLIKNO = 49,
            PNITELIK = 50,
            MEVKI = 51,
            PAFTANO = 52,
            ADANO = 53,
            PARSELNO = 54,
            OZET = 55,
            HEDEF = 56,
            ISTIHDAM = 57,
            DILEKCENO = 58,
            ESKIETAPDAVARMI = 59,
            BASVURUYAPILDI = 60,
            KOORDINAT = 61,
            ALTYAPIGRUP = 62,
            CINSIYET = 63,
            ENGELLI = 64,
            ORGANIK = 65,
            YAS = 66,
            ALTKONUDETAYKOD = 67
        }

        /// <summary>
        /// Hibe Yatirim tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "YIL", "BASVURUNO", "ILKOD", "ILCEKOD", "KOYKOD", "KASABA", "HUKUKISTATU", "KIMLIKNO", "VERGIDAIRE", "AD",
                                            "EVRAKNO", "EVRAKTARIH", "ADRES", "TELEFON", "FAKS", "EPOSTA", "BANKASUBEKOD", "DURUM", "SOZLESMETARIH", "FATURANO",
                                            "FATURATARIH", "SOYAD", "BABAAD", "DOGUMTARIH", "YERINDETESPITTARIH", "GRUPKOD", "ETAPNO", "PROJENITELIK", "PROJEKONU",
                                            "PROJEALTKONU", "PROJEADI", "SOZLESMEMALIYET", "BASVURUMALIYET", "HIBETUTAR", "HIBEESASTUTAR", "AYNIKATKI", "TOPLAMTUTAR",
                                            "ANKET1", "ANKET2", "ANKET3", "ANKET4", "ANKET3PUAN", "ANKET4PUAN", "PUAN", "KONUNITELIKUYUSMA", "DEGERLENDIRMEDISI",
                                            "DEGERLENDIRMEDISIGEREKCE", "KABULCALISMASI", "ONLINETUZELBASVURUKIMLIKNO", "PNITELIK", "MEVKI", "PAFTANO", "ADANO", "PARSELNO",
                                            "OZET", "HEDEF", "ISTIHDAM", "DILEKCENO", "ESKIETAPDAVARMI","BASVURUYAPILDI","KOORDINAT","ALTYAPIGRUP","CINSIYET","ENGELLI","ORGANIK","YAS","ALTKONUDETAYKOD" };

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
            IPADRESI = 5,
        }

        /// <summary>
        /// Hibe Yatirim tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTARIHCE = { "HIBEKOD", "KULLANICIKOD", "TARIH", "DURUM", "ACIKLAMA", "IPADRESI" };

        static string TABLOADSOZLESME = "TRMHIBEYATIRIMSOZLESME";
        public enum ENUMAlanlarSozlesme
        {
            HIBEKOD = 0,
            SOZLESMETARIH = 1,
            PROJEBASTARIH = 2,
            PROJEBITTARIH = 3,
            ISTIHDAM = 4,
            URETIMKAPASITESI = 5,
            TEMINATTUTAR = 6,
            TEMINATTUR = 7,
            TEMINATTARIH = 8,
            BANKASUBEKOD = 9,
            KAPASITEBIRIM = 10,
            ISTIHDAMKADIN = 11,
            DURUM = 12,
            NIHAIRAPORTARIHI = 13,
            YAPIKIMLIKNO = 14
        }
        public static string[] ALANLARSOZLESME = { "HIBEKOD", "SOZLESMETARIH", "PROJEBASTARIH", "PROJEBITTARIH", "ISTIHDAM",
                                                   "URETIMKAPASITESI", "TEMINATTUTAR", "TEMINATTUR", "TEMINATTARIH","BANKASUBEKOD","KAPASITEBIRIM","ISTIHDAMKADIN", "DURUM", "NIHAIRAPORTARIHI","YAPIKIMLIKNO" };

        static string TABLOADMALIYET = "TRMHIBEYATIRIMMALIYET";
        public enum ENUMAlanlarMaliyet
        {
            HIBEKOD = 0,
            DURUM = 1,
            HIBEESASTUTARINSAAT = 2,
            HIBEESASTUTARMAL = 3,
            HIBEESASTUTARTOPLAM = 4,
            AYNIKATKIINSAAT = 5,
            AYNIKATKIMAL = 6,
            AYNIKATKITOPLAM = 7,
            PROJETOPLAMTUTAR = 8,
            HIBETUTAR = 9,
            AYNIKATKIDANISMANLIK = 10
        }
        public static string[] ALANLARMALIYET = { "HIBEKOD", "DURUM", "HIBEESASTUTARINSAAT", "HIBEESASTUTARMAL", "HIBEESASTUTARTOPLAM",
                                                   "AYNIKATKIINSAAT", "AYNIKATKIMAL", "AYNIKATKITOPLAM", "PROJETOPLAMTUTAR", "HIBETUTAR","AYNIKATKIDANISMANLIK" };

        static string TABLOADISLETMEPLANI = "TRMYATIRIMISLETMEPLANI";
        public enum ENUMAlanlarIsletmePlani
        {
            HIBEKOD = 0,
            TOPLAMYATIRIMTUTARI = 1,
            SABITYATIRIMTUTARI = 2,
            SERMAYEYATIRIMTUTARI = 3,
            ISLETMEGELIRI = 4,
            ISLETMEGIDERI = 5,
            BRUTNAKITAKIM = 6,
            ICKARLILIKORANI = 7,
            FAYDAMASRAFORANI = 8,
            GERIDONUSURESI = 9,
            BRUTKATMADEGER = 10,
            NETKATMADEGER = 11,
            BASABASNOKTASI = 12,
            BASLAMATARIHI = 13,
            TAMAMLANMATARIHI = 14,
            MEVCUTKAPASITE = 15,
            PROJEILESAGLANACAKKAPASITE = 16,
            PROJESONRASIOLUSACAKKAPASITE = 17,
            MEVCUTISTAIHDAMSAYISI = 18,
            PROJEILESAGLANACAKISTIHDAMSAYISI = 19,
            PROJESONRASIISTIHDAMSAYISI = 20
        }
        public static string[] ALANLARISLETMEPLANI = { "HIBEKOD", "TOPLAMYATIRIMTUTARI", "SABITYATIRIMTUTARI", "SERMAYEYATIRIMTUTARI", "ISLETMEGELIRI",
                                                   "ISLETMEGIDERI", "BRUTNAKITAKIM", "ICKARLILIKORANI", "FAYDAMASRAFORANI", "GERIDONUSURESI", "BRUTKATMADEGER", "NETKATMADEGER",
                                                   "BASABASNOKTASI", "BASLAMATARIHI","TAMAMLANMATARIHI", "MEVCUTKAPASITE","PROJEILESAGLANACAKKAPASITE","PROJESONRASIOLUSACAKKAPASITE",
                                                   "MEVCUTISTAIHDAMSAYISI","PROJEILESAGLANACAKISTIHDAMSAYISI","PROJESONRASIISTIHDAMSAYISI"
                                                     };


        static string TABLOADEKSIKBELGE = "TRMHIBEYATIRIMEKSIKBELGE";
        public enum ENUMAlanlarEksikBelge
        {
            KOD = 0,
            HIBEKOD = 1,
            BELGEADI = 2,
            BELGENO = 3,
        }
        public static string[] ALANLAREKSIKBELGE = { "KOD", "HIBEKOD", "BELGEADI", "BELGENO" };


        static string TABLOADSORU = "TRMHIBEYATIRIMBASVURUSORULAR";
        public enum ENUMAlanlarSoru
        {
            HIBEKOD = 0,
            SORUKOD = 1,
            DEGER = 2,
        }
        public static string[] ALANLARSORU = { "HIBEKOD", "SORUKOD", "DEGER" };


        static string TABLOADGORUNTU = "TRMHIBEYATIRIMGORUNTU";
        public enum ENUMAlanlarGoruntu
        {
            HIBEKOD = 0,
            BASVURUNO = 1,
            EPOSTAADRESI = 2,
            KAMERA1ADRES = 3,
            KAMERA1KULLANICIADI = 4,
            KAMERA1PAROLA = 5,
            KAMERA2ADRES = 6,
            KAMERA2KULLANICIADI = 7,
            KAMERA2PAROLA = 8,
            KAMERA3ADRES = 9,
            KAMERA3KULLANICIADI = 10,
            KAMERA3PAROLA = 11,
            KAMERA4ADRES = 12,
            KAMERA4KULLANICIADI = 13,
            KAMERA4PAROLA = 14,
        }
        public static string[] ALANLARGORUNTU = { "HIBEKOD", "BASVURUNO", "EPOSTAADRESI", "KAMERA1ADRES", "KAMERA1KULLANICIADI", "KAMERA1PAROLA", "KAMERA2ADRES", "KAMERA2KULLANICIADI", "KAMERA2PAROLA", "KAMERA3ADRES", "KAMERA3KULLANICIADI", "KAMERA3PAROLA", "KAMERA4ADRES", "KAMERA4KULLANICIADI", "KAMERA4PAROLA" };

        static string TABLOADSOZLESMEFESIH = "TRMHIBEYATIRIMSOZLESMEFESIH";

        public static string[] ALANLARSOZLESMEFESIH = { "KOD", "HIBEKOD", "TARIH", "DURUM", "ACIKLAMA", "TUTAR", "ISLEMYAPANADI", "ISLEMTARIHI" };

        public enum ENUMAlanlarSozlesmeFesih
        {
            KOD = 0,
            HIBEKOD = 1,
            TARIH = 2,
            DURUM = 3,
            ACIKLAMA = 4,
            TUTAR = 5,
            ISLEMYAPANADI = 6,
            ISLEMTARIHI = 7
        }

        static string TABLOADYATIRIMCIKIMLIKNO = "TRMHIBEYATIRIMYATIRIMCIKIMLIKNO";

        public static string[] ALANLARYATIRIMCIKIMLIKNO = { "KOD", "HIBEKOD", "ESKIKIMLIKNO", "YENIKIMLIKNO" };

        public enum ENUMAlanlarYatirimciKimlikNo
        {
            KOD = 0,
            HIBEKOD = 1,
            ESKIKIMLIKNO = 2,
            YENIKIMLIKNO = 3
        }

        /// <summary>
        /// Hibe Yatirim bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe Yatirim bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASVURUNO], bilgi.basvuruNo) + ", ";
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

            if (bilgi.durum != (int)ENUMHibeYatirimBasvuruDurum.TANIMSIZ)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum) + ", ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJENITELIK], bilgi.projeNitelik) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJEKONU], bilgi.projeKonu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJEALTKONU], bilgi.projeAltKonu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJEADI], bilgi.projeAdi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASVURUMALIYET], bilgi.basvuruMaliyet) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIBETUTAR], bilgi.hibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", "; ;
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYNIKATKI], bilgi.ayniKatki) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMTUTAR], bilgi.toplamTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONUNITELIKUYUSMA], bilgi.konuNitelikUyusma ? 1 : 0) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONLINETUZELBASVURUKIMLIKNO], bilgi.onlineTuzeleBasvuran) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PNITELIK], bilgi.pNitelik) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEVKI], bilgi.mevki) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PAFTANO], bilgi.paftaNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADANO], bilgi.adaNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARSELNO], bilgi.parselNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOORDINAT], bilgi.koordinat) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], bilgi.altYapiGrup) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CINSIYET], bilgi.cinsiyet) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ENGELLI], bilgi.engelli) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGANIK], bilgi.organik) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAS], bilgi.yas) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTKONUDETAYKOD], bilgi.altKonuDetayKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OZET], bilgi.ozet) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDEF], bilgi.hedef) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTIHDAM], bilgi.istihdamSayisi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// hibe yatırım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe yatırım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc BasvuruyuIslemeAl(int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMHibeYatirimBasvuruDurum.YENI);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc BasvuruyuOnlineYap(int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMHibeYatirimBasvuruDurum.ONLINE) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASVURUYAPILDI], 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc GuncelleSozlesmeSonrasi(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEKOD], bilgi.ilceKod.Replace(".", "")) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOYKOD], bilgi.koyKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KASABA], bilgi.kasaba) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADANO], bilgi.adaNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARSELNO], bilgi.parselNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOORDINAT], bilgi.koordinat) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PAFTANO], bilgi.paftaNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEVKI], bilgi.mevki) + ", ";
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
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJEADI], bilgi.projeAdi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleDurum(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleSozlesmeDurum(HibeYatirimSozlesme bilgi)
        {
            string sql = "UPDATE " + TABLOADSOZLESME + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.DURUM], (int)bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleNihaiRaporTarih(HibeYatirimSozlesme bilgi)
        {
            string sql = "UPDATE " + TABLOADSOZLESME + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.NIHAIRAPORTARIHI], bilgi.nihaiRaporTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleYapiKimlikNo(HibeYatirimSozlesme bilgi)
        {
            string sql = "UPDATE " + TABLOADSOZLESME + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.YAPIKIMLIKNO], bilgi.yapiKimlikNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Kaydet(HibeYatirim bilgi)
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
            sql += VeriErisim.DegerYap(bilgi.grupKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";

            sql += VeriErisim.DegerYap(bilgi.projeNitelik) + ",";
            sql += VeriErisim.DegerYap(bilgi.projeKonu) + ",";
            sql += VeriErisim.DegerYap(bilgi.projeAltKonu) + ",";
            sql += VeriErisim.DegerYap(bilgi.projeAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeMaliyet) + ",";
            sql += VeriErisim.DegerYap(bilgi.basvuruMaliyet) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeEsasTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.ayniKatki) + ",";
            sql += VeriErisim.DegerYap(bilgi.toplamTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.anket1) + ",";
            sql += VeriErisim.DegerYap(bilgi.anket2) + ",";
            sql += VeriErisim.DegerYap(bilgi.anket3) + ",";
            sql += VeriErisim.DegerYap(bilgi.anket4) + ",";
            sql += VeriErisim.DegerYap(bilgi.anket3Puan) + ",";
            sql += VeriErisim.DegerYap(bilgi.anket4Puan) + ",";
            sql += VeriErisim.DegerYap(bilgi.puan) + ",";
            sql += VeriErisim.DegerYap(bilgi.konuNitelikUyusma ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(bilgi.degerlendirmeDisi ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(bilgi.degerlendirmeDisiGerekce) + ",";
            sql += VeriErisim.DegerYap(bilgi.kabulCalismasi) + ",";
            sql += VeriErisim.DegerYap(bilgi.onlineTuzeleBasvuran) + ",";
            sql += VeriErisim.DegerYap(bilgi.pNitelik) + ",";
            sql += VeriErisim.DegerYap(bilgi.mevki) + ",";
            sql += VeriErisim.DegerYap(bilgi.paftaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.adaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.parselNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ozet) + ",";
            sql += VeriErisim.DegerYap(bilgi.hedef) + ",";
            sql += VeriErisim.DegerYap(bilgi.istihdamSayisi) + ",";
            sql += VeriErisim.DegerYap(bilgi.dilekceNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.eskiEtapdaVarmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.basvuruYapildi) + ",";
            sql += VeriErisim.DegerYap(bilgi.koordinat) + ",";
            sql += VeriErisim.DegerYap(bilgi.altYapiGrup) + ",";
            sql += VeriErisim.DegerYap(bilgi.cinsiyet) + ",";
            sql += VeriErisim.DegerYap(bilgi.engelli) + ",";
            sql += VeriErisim.DegerYap(bilgi.organik) + ",";
            sql += VeriErisim.DegerYap(bilgi.yas) + ",";
            sql += VeriErisim.DegerYap(bilgi.altKonuDetayKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc BasvuruKopyasiAl(int kod)
        {
            string sql = "INSERT INTO TRMHIBEYATIRIMSOZLESMEONCESI SELECT * FROM TRMHIBEYATIRIM ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleBasvuruSiraNo(int kod, string basvuruNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET BASVURUNO='" + basvuruNo + "'";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetKabul(int etapNo, int basvuruTur)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERLENDIRMEDISI], 0) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE], "");
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KABULCALISMASI], "=", 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "<", (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI));
            if (basvuruTur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
            else if (basvuruTur == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">=", "51"));


            return KomutCalistir(sql, 0);
        }

        public IDataReader KaydetKabulGeriAlListele(int etapNo, int basvuruTur, string altYapiGrup)
        {
            string sql = "SELECT KOD FROM TRMHIBEYATIRIM ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", 5));

            if (basvuruTur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
            else if (basvuruTur == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">=", "51"));

            if (altYapiGrup != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], "=", altYapiGrup));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetKabulGeriAlKabulYaz(int etapNo, int basvuruTur, string altYapiGrup)
        {
            string sql = "UPDATE TRMHIBEYATIRIM SET KABULCALISMASI=6";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", 5));

            if (basvuruTur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
            else if (basvuruTur == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">=", "51"));

            if (altYapiGrup != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], "=", altYapiGrup));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KaydetKabulGeriAl(int etapNo, int basvuruTur, string altYapiGrup)
        {
            string sql = "UPDATE TRMHIBEYATIRIM SET DURUM=3";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", 5));

            if (basvuruTur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
            else if (basvuruTur == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">=", "51"));

            if (altYapiGrup != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], "=", altYapiGrup));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KaydetKabulCalismasi(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KABULCALISMASI], bilgi.kabulCalismasi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERLENDIRMEDISI], 0) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE], "");
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetCBasvuruYapildi(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASVURUYAPILDI], bilgi.basvuruYapildi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleBasvuruYapildi(int kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.BASVURUYAPILDI];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetUygunlukCalismasi(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ESKIETAPDAVARMI], bilgi.eskiEtapdaVarmi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetTarihceKabul(int etapNo, HibeYatirimTarihce tarihce)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE;
            sql += " SELECT " + ALANLAR[(int)ENUMAlanlar.KOD];
            sql += ", " + VeriErisim.DegerYap(tarihce.tarih);
            sql += ", " + VeriErisim.DegerYap(tarihce.kullaniciKod);
            sql += ", " + VeriErisim.DegerYap(tarihce.durum);
            sql += ", " + VeriErisim.DegerYap(tarihce.aciklama);
            sql += ", " + VeriErisim.DegerYap(tarihce.ipAdresi);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KABULCALISMASI], "=", 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI));

            return KomutCalistir(sql, 0);
        }

        //public Sonuc KaydetKabulVazgec(int etapNo, string ilKod, HibeYatirim[] bilgiler)
        //{
        //    string sql = "UPDATE " + TABLOAD + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMHibeYatirimBasvuruDurum.YENI);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", ALANLAR[(int)ENUMAlanlar.KOD] + " NOT IN (" + KodlariVirgulle(bilgiler) + ")");
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", ilKod));

        //    return KomutCalistir(sql, 0);
        //}

        private string KodlariVirgulle(HibeYatirim[] bilgiler)
        {
            string kodlar = string.Empty;
            foreach (HibeYatirim bilgi in bilgiler)
            {
                if (!string.IsNullOrEmpty(kodlar))
                    kodlar += ", ";
                kodlar += bilgi.kod.ToString();
            }
            return kodlar;
        }

        /// <summary>
        /// Hibe Yatirim bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe Yatirim bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSozlesme(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], bilgi.sozlesmeTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEMALIYET], bilgi.sozlesmeMaliyet) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetSozlesmeDetay(HibeYatirimSozlesme s)
        {
            string sql = "INSERT INTO " + TABLOADSOZLESME + " (" + VeriErisim.TumAlanlar("", ALANLARSOZLESME) + ") VALUES (";
            sql += VeriErisim.DegerYap(s.hibeKod) + ",";
            sql += VeriErisim.DegerYap(s.sozlesmeTarih) + ",";
            sql += VeriErisim.DegerYap(s.projeBasTarih) + ",";
            sql += VeriErisim.DegerYap(s.projeBitTarih) + ",";
            sql += VeriErisim.DegerYap(s.istihdam) + ",";
            sql += VeriErisim.DegerYap(s.uretimKapasitesi) + ",";
            sql += VeriErisim.DegerYap(s.teminatTutar) + ",";
            sql += VeriErisim.DegerYap(s.teminatTur) + ",";
            sql += VeriErisim.DegerYap(s.teminatTarih) + ",";
            sql += VeriErisim.DegerYap(s.bankaSubeKod) + ",";
            sql += VeriErisim.DegerYap(s.kapasiteBirimKodu) + ",";
            sql += VeriErisim.DegerYap(s.istihdamKadin) + ",";
            sql += VeriErisim.DegerYap((int)s.durum) + ",";
            sql += VeriErisim.DegerYap(s.nihaiRaporTarihi) + ",";
            sql += VeriErisim.DegerYap(s.yapiKimlikNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilSozlesmeDetay(HibeYatirimSozlesme s)
        {
            string sql = "DELETE FROM " + TABLOADSOZLESME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.HIBEKOD], "=", s.hibeKod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleSozlesme(int hibeKod)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASVURUNO];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DURUM];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KIMLIKNO];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ILKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TOPLAMTUTAR];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AYNIKATKI];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HIBEESASTUTAR];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HIBETUTAR];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ETAPNO];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJEKONU];
            sql += ", " + VeriErisim.TumAlanlar(TABLOADSOZLESME, ALANLARSOZLESME);
            sql += ", " + VeriErisim.TumAlanlar(TABLOADMALIYET, ALANLARMALIYET);
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOADSOZLESME;
            sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADSOZLESME + "." + ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.HIBEKOD];
            sql += " LEFT OUTER JOIN " + TABLOADMALIYET;
            sql += " ON " + TABLOADSOZLESME + "." + ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.HIBEKOD] + " = " + TABLOADMALIYET + "." + ALANLARMALIYET[(int)ENUMAlanlarMaliyet.HIBEKOD];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOADMALIYET, ALANLARMALIYET[(int)ENUMAlanlarMaliyet.DURUM], ">=", (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI));
            sql = VeriErisim.KosulEkle(sql, "OR", TABLOADMALIYET + "." + ALANLARMALIYET[(int)ENUMAlanlarMaliyet.DURUM] + " IS NULL)");

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetMaliyet(HibeYatirimMaliyet m, ENUMHibeYatirimBasvuruDurum durum)
        {
            string sql = "INSERT INTO " + TABLOADMALIYET + " (" + VeriErisim.TumAlanlar("", ALANLARMALIYET) + ") VALUES (";
            sql += VeriErisim.DegerYap(m.hibeKod) + ",";
            sql += VeriErisim.DegerYap((int)durum) + ",";
            sql += VeriErisim.DegerYap(m.hibeEsasTutarInsaat) + ",";
            sql += VeriErisim.DegerYap(m.hibeEsasTutarMal) + ",";
            sql += VeriErisim.DegerYap(m.hibeEsasTutarToplam) + ",";
            sql += VeriErisim.DegerYap(m.ayniKatkiInsaat) + ",";
            sql += VeriErisim.DegerYap(m.ayniKatkiMal) + ",";
            sql += VeriErisim.DegerYap(m.ayniKatkiToplam) + ",";
            sql += VeriErisim.DegerYap(m.projeToplamTutar) + ",";
            sql += VeriErisim.DegerYap(m.hibeTutar) + ",";
            sql += VeriErisim.DegerYap(m.ayniKatkiDanismanlik) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilMaliyet(HibeYatirimMaliyet m, ENUMHibeYatirimBasvuruDurum durum)
        {
            string sql = "DELETE FROM " + TABLOADMALIYET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALIYET[(int)ENUMAlanlarMaliyet.HIBEKOD], "=", m.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALIYET[(int)ENUMAlanlarMaliyet.DURUM], "=", (int)durum));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleMaliyet(int hibeKod, ENUMHibeYatirimBasvuruDurum durum)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMALIYET, ALANLARMALIYET) + " FROM " + TABLOADMALIYET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARMALIYET[(int)ENUMAlanlarMaliyet.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARMALIYET[(int)ENUMAlanlarMaliyet.DURUM], "=", (int)durum));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HibeEsasTutarGetir(int hibeKod, ENUMHibeYatirimBasvuruDurum durum)
        {
            //string sql = "SELECT hibeesastutar = case when " + TABLOADMALIYET + "." + ALANLARMALIYET[(int)ENUMAlanlarMaliyet.DURUM] + "=" + (int) durum;
            //sql += " then " + TABLOADMALIYET + "." + ALANLARMALIYET[(int)ENUMAlanlarMaliyet.HIBEESASTUTARTOPLAM];
            //sql += " else " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HIBEESASTUTAR] + " end";
            //sql += " FROM " + TABLOAD;          
            //sql += " LEFT OUTER JOIN " + TABLOADMALIYET;
            //sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADMALIYET + "." + ALANLARMALIYET[(int)ENUMAlanlarMaliyet.HIBEKOD];
            string sql = "select BASVURUMALIYET from TRMHIBEYATIRIM ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetReddedildi(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], string.Empty) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetOnline(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], string.Empty) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc BasvuruyuGeriCek(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], string.Empty) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DILEKCENO], bilgi.dilekceNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], bilgi.kimlikNo + "_Iptal") + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetDegerlendirme(HibeYatirim[] bilgiler)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgiler[0].durum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERLENDIRMEDISI], bilgiler[0].degerlendirmeDisi ? 1 : 0) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KABULCALISMASI], bilgiler[0].kabulCalismasi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE], bilgiler[0].degerlendirmeDisiGerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", ALANLAR[(int)ENUMAlanlar.KOD] + " IN (" + KodlariVirgulle(bilgiler) + ")");
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KONUNITELIKUYUSMA], "=", 0));

            return KomutCalistir(sql, 0);
        }

        public int ListeleDurum(int hibeKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DURUM] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Hibe Yatirim bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hibe Yatirim bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hibe Yatirim bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(HibeYatirim bilgi, string orderByKolon, bool asc, bool gecenler = false)
        {
            string sql = "";
            sql += " SELECT " + VeriErisim.TumAlanlar("YATIRIM", ALANLAR) + ", TRMYATIRIMNITELIK.ADI, TRMYATIRIMKONU.ADI, TRMYATIRIMKONU_1.ADI, TRMKOY.ADI, ORGCOGRAFYA.AD AS ILAD, ORGCOGRAFYA_1.AD AS ILCEAD, HIBEYATIRIMODENEN.ODENENINSAAT, HIBEYATIRIMODENEN.ODENENMAL," + VeriErisim.TumAlanlar(TABLOADSOZLESME, ALANLARSOZLESME) + "," + VeriErisim.TumAlanlar(TABLOADMALIYET, ALANLARMALIYET);
            sql += ", (SELECT MAX(ODEMETARIH) AS SONODEMETARIH FROM TRMHIBEYATIRIMHAKEDIS WHERE TRMHIBEYATIRIMHAKEDIS.HIBEKOD=YATIRIM.KOD GROUP BY HIBEKOD)";
            sql += ", (SELECT COUNT(*) FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.DURUM>=10 AND ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO<YATIRIM.ETAPNO)";
            sql += ", (SELECT KOD,BASVURUNO,KIMLIKNO,AD,SOYAD,ETAPNO,PROJEADI FROM TRMHIBEYATIRIMSOZLESME AS SOZLESME,TRMHIBEYATIRIM SYATIRIM WHERE SOZLESME.HIBEKOD=SYATIRIM.KOD AND SOZLESME.DURUM=1 AND SOZLESME.HIBEKOD IN (SELECT KOD FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO<YATIRIM.ETAPNO) FOR XML RAW, ELEMENTS xsinil, TYPE)";
            sql += ", (SELECT KOD,BASVURUNO,KIMLIKNO,AD,SOYAD,ETAPNO,PROJEADI FROM TRMHIBEYATIRIMSOZLESME AS SOZLESME,TRMHIBEYATIRIM SYATIRIM WHERE SOZLESME.HIBEKOD=SYATIRIM.KOD AND SOZLESME.DURUM=2 AND SOZLESME.HIBEKOD IN (SELECT KOD FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO>=(YATIRIM.ETAPNO-5)) FOR XML RAW, ELEMENTS xsinil, TYPE)";
            sql += ", (SELECT KOD,BASVURUNO,KIMLIKNO,AD,SOYAD,ETAPNO,PROJEADI FROM TRMHIBEYATIRIMSOZLESME AS SOZLESME,TRMHIBEYATIRIM SYATIRIM WHERE SOZLESME.HIBEKOD=SYATIRIM.KOD AND SOZLESME.DURUM=2 AND SOZLESME.HIBEKOD IN (SELECT KOD FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO<(YATIRIM.ETAPNO-3)) FOR XML RAW, ELEMENTS xsinil, TYPE)";
            sql += ", (SELECT KOD,BASVURUNO,KIMLIKNO,AD,SOYAD,ETAPNO,PROJEADI FROM TRMHIBEYATIRIMSOZLESME AS SOZLESME,TRMHIBEYATIRIM SYATIRIM WHERE SOZLESME.HIBEKOD=SYATIRIM.KOD AND SOZLESME.DURUM=4 AND SOZLESME.HIBEKOD IN (SELECT KOD FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO<YATIRIM.ETAPNO) FOR XML RAW, ELEMENTS xsinil, TYPE)";
            sql += ", (SELECT KOD,BASVURUNO,KIMLIKNO,AD,SOYAD,ETAPNO,PROJEADI FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.DURUM>=5 AND ICYATIRIM.DURUM<10 AND ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO<YATIRIM.ETAPNO FOR XML RAW, ELEMENTS xsinil, TYPE)";
            sql += ", (SELECT KOD,BASVURUNO,KIMLIKNO,AD,SOYAD,ETAPNO,PROJEADI FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.DURUM>=1 AND ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO=8 FOR XML RAW, ELEMENTS xsinil, TYPE)";
            sql += ", (SELECT KOD,BASVURUNO,KIMLIKNO,AD,SOYAD,ETAPNO,PROJEADI FROM TRMHIBEYATIRIMSOZLESME AS SOZLESME,TRMHIBEYATIRIM SYATIRIM WHERE SOZLESME.HIBEKOD=SYATIRIM.KOD AND SOZLESME.DURUM>0 AND SYATIRIM.ETAPNO>=(YATIRIM.ETAPNO-5) AND SOZLESME.HIBEKOD IN (SELECT HIBEKOD FROM TRMHIBEYATIRIMORTAKLAR AS ORTAK WHERE ORTAK.KIMLIKNO=YATIRIM.KIMLIKNO) FOR XML RAW, ELEMENTS xsinil, TYPE)";
            sql += ", (SELECT COUNT(*) FROM TRMHIBEYATIRIMORTAKLAR AS ORTAKLAR WHERE ORTAKLAR.HIBEKOD<>YATIRIM.KOD AND ORTAKLAR.KIMLIKNO=YATIRIM.KIMLIKNO AND ORTAKLAR.HIBEKOD IN (SELECT KOD FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.ETAPNO=YATIRIM.ETAPNO AND ICYATIRIM.DURUM>0))";
            sql += ", (SELECT cast(MAX(TARIH) as date) FROM TRMHIBEYATIRIMTARIHCE AS TARIHCE WHERE TARIHCE.HIBEKOD=YATIRIM.KOD AND DURUM = 1 GROUP BY HIBEKOD)";
            sql += ", (SELECT TRMYATIRIMMALZEMETANIM.KOD, TRMYATIRIMMALZEMEGRUPANA.ADI AS GADI, TRMYATIRIMMALZEMETUR.ADI AS TADI, TRMYATIRIMMALZEMETANIM.ADI FROM TRMYATIRIMMALZEMETANIM INNER JOIN TRMYATIRIMMALZEMETUR ON TRMYATIRIMMALZEMETANIM.MALZEMETURKOD = TRMYATIRIMMALZEMETUR.KOD INNER JOIN  TRMYATIRIMMALZEME ON TRMYATIRIMMALZEMETANIM.KOD = TRMYATIRIMMALZEME.MALZEMEKOD INNER JOIN TRMYATIRIMMALZEMEGRUPANA ON TRMYATIRIMMALZEMEGRUPANA.KOD=TRMYATIRIMMALZEMETUR.ANAKOD WHERE TRMYATIRIMMALZEME.HIBEKOD=YATIRIM.KOD FOR XML RAW, ELEMENTS xsinil, TYPE)";
            if (bilgi.kod > 0)
                sql += ", (SELECT KOD, BASVURUNO, KIMLIKNO, AD, SOYAD, ETAPNO, PROJEADI FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.DURUM>=1 AND ICYATIRIM.KOD != YATIRIM.KOD AND ICYATIRIM.ETAPNO=YATIRIM.ETAPNO AND ADRES = YATIRIM.ADRES FOR XML RAW, ELEMENTS xsinil, TYPE) AS ADRESKONTROL";
            else
                sql += ", '' AS ADRESKONTROL";
            sql += ", TRMYATIRIMALTKONUDETAY.ALTKONUKOD, TRMYATIRIMALTKONUDETAY.ADI";
            sql += $",{UygulamaSozlesmesi(2)}, {UygulamaSozlesmesi(1)}";
            sql += " FROM TRMHIBEYATIRIM AS YATIRIM";
            sql += " LEFT OUTER JOIN HIBEYATIRIMODENEN ON YATIRIM.KOD = HIBEYATIRIMODENEN.HIBEKOD";
            sql += " LEFT OUTER JOIN TRMHIBEYATIRIMMALIYET ON YATIRIM.KOD = TRMHIBEYATIRIMMALIYET.HIBEKOD AND TRMHIBEYATIRIMMALIYET.DURUM = " + (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI;
            sql += " LEFT OUTER JOIN TRMHIBEYATIRIMSOZLESME ON YATIRIM.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD";
            sql += " LEFT OUTER JOIN TRMYATIRIMKONU AS TRMYATIRIMKONU_1 ON YATIRIM.PROJEALTKONU = TRMYATIRIMKONU_1.KOD";
            sql += " LEFT OUTER JOIN TRMYATIRIMNITELIK ON YATIRIM.PROJENITELIK = TRMYATIRIMNITELIK.KOD";
            sql += " LEFT OUTER JOIN TRMYATIRIMKONU ON YATIRIM.PROJEKONU = TRMYATIRIMKONU.KOD";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA ON YATIRIM.ILKOD = ORGCOGRAFYA.KOD";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA AS ORGCOGRAFYA_1 ON YATIRIM.ILCEKOD = ORGCOGRAFYA_1.KOD";
            sql += " LEFT OUTER JOIN TRMKOY ON YATIRIM.KOYKOD = TRMKOY.KOD";
            sql += " LEFT OUTER JOIN TRMYATIRIMALTKONUDETAY ON YATIRIM.ALTKONUDETAYKOD = TRMYATIRIMALTKONUDETAY.KOD";

            string kosul = ListeleKriterOl(bilgi, gecenler);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY ";
            if (string.IsNullOrEmpty(orderByKolon))
            {
                if (bilgi.komisyonRaporu)
                    sql += "YATIRIM" + "." + ALANLAR[(int)ENUMAlanlar.BASVURUNO];
                else
                    sql += "YATIRIM" + "." + ALANLAR[(int)ENUMAlanlar.KOD];
            }
            else if (orderByKolon.Equals("ListeleOncelikliForKabul"))
            {
                sql += "YATIRIM" + "." + ALANLAR[(int)ENUMAlanlar.PUAN] + " DESC, ";
                sql += VeriErisim.IslecBul("RIGHT") + "(" + "YATIRIM" + "." + ALANLAR[(int)ENUMAlanlar.BASVURUNO] + ", 5)";
            }
            else
                sql += orderByKolon;
            if (asc)
                sql += " ASC";
            else
                sql += " DESC";

            sql = sql.Replace(TABLOAD + ".", "YATIRIM.");

            return veriErisim.DataReaderDondur(sql);
        }

        private string UygulamaSozlesmesi(int sozlesmeTur)
        {
            return $"(SELECT SUM(SOZLESMETUTAR) FROM TRMHIBEYATIRIMUYGULAMASOZLESME WHERE HIBEKOD=YATIRIM.KOD AND SOZLESMETUR={sozlesmeTur})"; 
        }

        public IDataReader ListeleEksikBelge(HibeYatirim bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", ORGCOGRAFYA.AD, ORGCOGRAFYA_1.AD";
            sql += ", STUFF((SELECT '^' + TRMHIBEYATIRIMEKSIKBELGE.BELGEADI FROM TRMHIBEYATIRIMEKSIKBELGE WHERE HIBEKOD=TRMHIBEYATIRIM.kod FOR XML PATH('')), 1, 1, '') AS EksikBelgeler";
            sql += " FROM TRMHIBEYATIRIM";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD ";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEYATIRIM.ILCEKOD = ORGCOGRAFYA_1.KOD";
            sql += " WHERE TRMHIBEYATIRIM.KOD IN (SELECT HIBEKOD FROM TRMHIBEYATIRIMEKSIKBELGE GROUP BY HIBEKOD)";

            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.etapNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            if (!string.IsNullOrEmpty(bilgi.ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", bilgi.ilKod.Replace(".", "")));
            if (bilgi.altYapiGrup != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], "=", bilgi.altYapiGrup));
            if (bilgi.yatirimOncelikTur == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
            if (bilgi.yatirimOncelikTur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">=", "51"));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASVURUNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleMerkezdeKabulHali(int kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += " FROM TRMHIBEYATIRIMSOZLESMEONCESI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleKabulCalismasiIlToplami(Butce kriter)
        {
            string sql = "SELECT ILKOD, SUM(HIBETUTAR) FROM TRMHIBEYATIRIM";
            string yatirimKosul = "";
            if (kriter.yatirimOncelikTur == 0)
                yatirimKosul = "AND (ANKET3 = 1)";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", kriter.etapNo));
            if (kriter.yatirimOncelikTur == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
            if (kriter.yatirimOncelikTur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">=", "51"));

            if (kriter.ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", kriter.ilKod.Trim().Replace(".", "")));

            //!!!!
            //sql = VeriErisim.KosulEkle(sql, "AND", "(DURUM <> - 2) AND (ANKET1 = 1) AND (ANKET2 = 1) " + yatirimKosul + " AND (ANKET4 = 1) AND (DURUM <> 99) AND (KABULCALISMASI <= 1)");
            sql = VeriErisim.KosulEkle(sql, "AND", "(DURUM <> - 2) AND (ANKET1 = 1) AND (ANKET2 = 1) " + yatirimKosul + " AND (DURUM <> 99) AND (KABULCALISMASI <= 1)");
            sql += "GROUP BY ILKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleYatirimVeSozlesme(int hibeKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", " + VeriErisim.TumAlanlar(TABLOADSOZLESME, ALANLARSOZLESME) + ", " + VeriErisim.TumAlanlar(TABLOADMALIYET, ALANLARMALIYET);
            sql += ", TRMYATIRIMNITELIK.ADI, TRMYATIRIMKONU.ADI, TRMYATIRIMKONU_1.ADI, TRMKOY.ADI, ORGCOGRAFYA.AD AS ILAD, ORGCOGRAFYA_1.AD AS ILCEAD";
            sql += " FROM TRMHIBEYATIRIM ";
            sql += " LEFT OUTER JOIN TRMHIBEYATIRIMSOZLESME ON TRMHIBEYATIRIM.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD";
            sql += " LEFT OUTER JOIN TRMHIBEYATIRIMMALIYET ON TRMHIBEYATIRIMSOZLESME.HIBEKOD = TRMHIBEYATIRIMMALIYET.HIBEKOD AND TRMHIBEYATIRIM.DURUM = TRMHIBEYATIRIMMALIYET.DURUM";
            sql += " LEFT OUTER JOIN TRMYATIRIMKONU AS TRMYATIRIMKONU_1 ON TRMHIBEYATIRIM.PROJEALTKONU = TRMYATIRIMKONU_1.KOD LEFT OUTER JOIN";
            sql += " TRMYATIRIMNITELIK ON TRMHIBEYATIRIM.PROJENITELIK = TRMYATIRIMNITELIK.KOD LEFT OUTER JOIN";
            sql += " TRMYATIRIMKONU ON TRMHIBEYATIRIM.PROJEKONU = TRMYATIRIMKONU.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEYATIRIM.ILCEKOD = ORGCOGRAFYA_1.KOD LEFT OUTER JOIN";
            sql += " TRMKOY ON TRMHIBEYATIRIM.KOYKOD = TRMKOY.KOD";
            sql += " WHERE TRMHIBEYATIRIM.KOD = '" + hibeKod + "'";

            return veriErisim.DataReaderDondur(sql);
        }

        public int HibeYatirimBasvuruSayiDondur(HibeYatirim bilgi)
        {
            return veriErisim.TekKolonSQLInt(TekKolonSQLDondur(bilgi, VeriErisim.IslecBul("COUNT") + "(*)"));
        }

        public decimal DagitilanTutarDondur(HibeYatirim bilgi)
        {

            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.HIBETUTAR] + ")" + " FROM ";
            sql += " TRMHIBEYATIRIM";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KABULCALISMASI], "=", 1));

            if (bilgi.altYapiGrup != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], "=", bilgi.altYapiGrup));

            if (bilgi.yatirimOncelikTur == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
            if (bilgi.yatirimOncelikTur == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">=", "51"));

            if (!string.IsNullOrEmpty(bilgi.ilKod) && bilgi.ilKod != "-1")
            {
                string[] iller = bilgi.ilKod.Split(';');
                string ilKosul = "";
                foreach (string il in iller)
                {
                    if (il.Trim() == "") continue;
                    ilKosul = VeriErisim.KosulEkle(ilKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", il.Trim().Replace(".", "")));
                }
                if (ilKosul != "")
                {
                    if (sql == "")
                        sql += " (" + ilKosul + ")";
                    else
                        sql += " AND (" + ilKosul + ")";
                }
            }

            return veriErisim.TekKolonSQLDecimal(sql);
        }

        public bool KabulSonrasiVarMi(HibeYatirim bilgi)
        {
            string sql = TekKolonSQLDondur(bilgi, VeriErisim.IslecBul("COUNT") + "(*)");
            if (sql.Contains("WHERE"))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], ">", (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], ">", (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        private string TekKolonSQLDondur(HibeYatirim bilgi, string alan, bool gecenler = false)
        {
            string sql = "SELECT " + alan + " FROM ";
            sql += " TRMHIBEYATIRIM LEFT OUTER JOIN";
            sql += " TRMYATIRIMKONU AS TRMYATIRIMKONU_1 ON TRMHIBEYATIRIM.PROJEALTKONU = TRMYATIRIMKONU_1.KOD LEFT OUTER JOIN";
            sql += " TRMYATIRIMNITELIK ON TRMHIBEYATIRIM.PROJENITELIK = TRMYATIRIMNITELIK.KOD LEFT OUTER JOIN";
            sql += " TRMYATIRIMKONU ON TRMHIBEYATIRIM.PROJEKONU = TRMYATIRIMKONU.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEYATIRIM.ILCEKOD = ORGCOGRAFYA_1.KOD LEFT OUTER JOIN";
            sql += " TRMKOY ON TRMHIBEYATIRIM.KOYKOD = TRMKOY.KOD";
            if (bilgi.sozlesmeBilgileri != null && (int)bilgi.sozlesmeBilgileri.durum > 0)
                sql += " LEFT OUTER JOIN TRMHIBEYATIRIMSOZLESME ON TRMHIBEYATIRIM.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD";

            string kosul = ListeleKriterOl(bilgi, gecenler);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return sql;
        }

        public IDataReader ListeleMalzeme(HibeYatirim bilgi, string orderByKolon, bool asc, bool gecenler = false)
        {
            //string sql = "SELECT TRMHIBEYATIRIM.KOD,TRMHIBEYATIRIM.BASVURUNO,TRMHIBEYATIRIM.ILKOD,TRMHIBEYATIRIM.KIMLIKNO,TRMHIBEYATIRIM.AD,TRMHIBEYATIRIM.SOYAD,TRMHIBEYATIRIM.ETAPNO,TRMHIBEYATIRIM.PROJEADI,ORGCOGRAFYA.AD AS ILAD,TRMYATIRIMMALZEMETANIM.ADI AS MALADI,TRMYATIRIMMALZEME.KOD AS MKOD,TRMYATIRIMMALZEME.MALZEMETURKOD AS MTURKOD,TRMYATIRIMMALZEME.MALZEMEKOD AS MMKOD,TRMYATIRIMMALZEMETUR.ADI AS MTADI";
            //sql += " FROM TRMHIBEYATIRIM ";
            //sql += " LEFT OUTER JOIN ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD";
            //sql += " LEFT OUTER JOIN TRMYATIRIMMALZEME ON TRMYATIRIMMALZEME.HIBEKOD = TRMHIBEYATIRIM.KOD";
            //sql += " LEFT OUTER JOIN TRMYATIRIMMALZEMETANIM ON TRMYATIRIMMALZEMETANIM.KOD = TRMYATIRIMMALZEME.MALZEMEKOD";
            //sql += " LEFT OUTER JOIN TRMYATIRIMMALZEMETUR ON TRMYATIRIMMALZEMETUR.KOD = TRMYATIRIMMALZEMETANIM.MALZEMETURKOD";

            string sql = "SELECT TRMHIBEYATIRIM.KOD,TRMHIBEYATIRIM.BASVURUNO,TRMHIBEYATIRIM.ILKOD,TRMHIBEYATIRIM.KIMLIKNO,TRMHIBEYATIRIM.AD,TRMHIBEYATIRIM.SOYAD,TRMHIBEYATIRIM.ETAPNO,TRMHIBEYATIRIM.PROJEADI,ORGCOGRAFYA.AD AS ILAD, MALZEME";
            sql += " FROM TRMHIBEYATIRIM ";
            sql += " LEFT OUTER JOIN ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD";
            sql += " CROSS APPLY(SELECT TRMYATIRIMMALZEMETANIM.ADI +'^'+ CONVERT(VARCHAR,TRMYATIRIMMALZEME.KOD) +'^'+ CONVERT(VARCHAR,TRMYATIRIMMALZEME.MALZEMETURKOD) +'^'+ TRMYATIRIMMALZEME.MALZEMEKOD +'^'+ ISNULL(TRMYATIRIMMALZEMETUR.ADI,'') + '^' + CONVERT(VARCHAR,TRMYATIRIMMALZEME.MIKTAR) +'|' FROM TRMYATIRIMMALZEME LEFT OUTER JOIN TRMYATIRIMMALZEMETANIM ON TRMYATIRIMMALZEMETANIM.KOD = TRMYATIRIMMALZEME.MALZEMEKOD LEFT OUTER JOIN TRMYATIRIMMALZEMETUR ON TRMYATIRIMMALZEMETUR.KOD = TRMYATIRIMMALZEMETANIM.MALZEMETURKOD  WHERE TRMYATIRIMMALZEME.HIBEKOD = TRMHIBEYATIRIM.KOD ORDER BY TRMYATIRIMMALZEME.OZELLIK DESC FOR XML PATH('')) MAL(MALZEME)";

            string kosul = ListeleKriterOl(bilgi, gecenler);
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeleKriterOl(HibeYatirim bilgi, bool gecenler = false)
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
            if (bilgi.altYapiGrup != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], "=", bilgi.altYapiGrup));
            else
            {
                if (bilgi.eskiBasvuru)
                {
                    for (int i = 1; i < 7; i++)
                    {
                        if (i == 1)
                        {
                            if (!string.IsNullOrEmpty(kosul))
                                kosul += " AND ";
                            kosul += " (";
                        }

                        kosul += " TRMHIBEYATIRIM.ETAPNO= " + i;

                        if (i != 6)
                            kosul += " OR ";
                        if (i == 6)
                            kosul += ") ";
                    }
                }
            }
            if (!string.IsNullOrEmpty(bilgi.basvuruNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASVURUNO], "=", bilgi.basvuruNo));
            if (!string.IsNullOrEmpty(bilgi.cinsiyet))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CINSIYET], "=", bilgi.cinsiyet));
            if (!string.IsNullOrEmpty(bilgi.engelli))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ENGELLI], "=", bilgi.engelli));
            if (!string.IsNullOrEmpty(bilgi.organik))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIK], "=", bilgi.organik));
            if (bilgi.yas > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YAS], ">=", bilgi.yas));
            if (bilgi.altKonuDetayKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTKONUDETAYKOD], "=", bilgi.altKonuDetayKod));
            if (!string.IsNullOrEmpty(bilgi.ilKod) && bilgi.ilKod != "-1")
            {
                string[] iller = bilgi.ilKod.Split(';');
                string ilKosul = "";
                foreach (string il in iller)
                {
                    if (il.Trim() == "") continue;
                    ilKosul = VeriErisim.KosulEkle(ilKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", il.Trim().Replace(".", "")));
                }
                if (ilKosul != "")
                {
                    if (kosul == "")
                        kosul += " (" + ilKosul + ")";
                    else
                        kosul += " AND (" + ilKosul + ")";
                }
            }
            if (!string.IsNullOrEmpty(bilgi.ilceKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCEKOD], "=", bilgi.ilceKod.Replace(".", "")));
            if (bilgi.koyKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOYKOD], "=", bilgi.koyKod));
            if (!string.IsNullOrEmpty(bilgi.kasaba))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KASABA], "LIKE", "%" + bilgi.kasaba + "%"));
            if (bilgi.hukukiStatu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], "=", bilgi.hukukiStatu));
            if (!string.IsNullOrEmpty(bilgi.kimlikNo))
            {
                string[] kimlikNo = bilgi.kimlikNo.Split(',');
                string icKosul = "";
                for (int i = 0; i < kimlikNo.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(kimlikNo[i])) continue;
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", kimlikNo[i]));
                }
                if (icKosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            }
            //if (!string.IsNullOrEmpty(bilgi.kimlikNo))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", bilgi.kimlikNo));
            if (!string.IsNullOrEmpty(bilgi.projeAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEADI], "LIKE", "%" + bilgi.projeAdi + "%"));
            if (!string.IsNullOrEmpty(bilgi.vergiDaire))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VERGIDAIRE], "=", bilgi.vergiDaire));
            if (!string.IsNullOrEmpty(bilgi.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD] + "+' '+" + ALANLAR[(int)ENUMAlanlar.SOYAD], "LIKE", "%" + bilgi.ad + "%"));
            if (!string.IsNullOrEmpty(bilgi.evrakNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKNO], "=", bilgi.evrakNo));
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
            if (bilgi.projeNitelik > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJENITELIK], "=", bilgi.projeNitelik));

            if (!string.IsNullOrEmpty(bilgi.projeKonu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "=", bilgi.projeKonu));

            if (bilgi.yatirimOncelikTur == 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
            if (bilgi.yatirimOncelikTur == 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">=", "51"));

            if (bilgi.puanBas > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], ">=", bilgi.puanBas));
            if (bilgi.puanBit > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], "<=", bilgi.puanBit));

            if (bilgi.sorguHibeTutar1 > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEESASTUTAR], ">=", bilgi.sorguHibeTutar1));
            if (bilgi.sorguHibeTutar2 > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEESASTUTAR], "<=", bilgi.sorguHibeTutar2));


            if (!string.IsNullOrEmpty(bilgi.projeAltKonu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEALTKONU], "LIKE", bilgi.projeAltKonu + "%"));

            if (bilgi.sozlesmeBilgileri != null && (int)bilgi.sozlesmeBilgileri.durum > 0)
            {
                if (bilgi.sozlesmeBilgileri.durum == ENUMHibeYatirimSozlesmeDurum.DEVAMEDIYOR)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSOZLESME, ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.DURUM], "<=", (int)bilgi.sozlesmeBilgileri.durum));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSOZLESME, ALANLARSOZLESME[(int)ENUMAlanlarSozlesme.DURUM], "=", (int)bilgi.sozlesmeBilgileri.durum));
            }

            if (bilgi.durum > 0 || bilgi.durum == -1)
            {
                if (bilgi.durum == (int)ENUMHibeYatirimBasvuruDurum.ILDEKABULEDILENLER)
                {
                    gecenler = true;
                    //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KABULCALISMASI], ">=", 1));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<", (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI));

                    //string ekKosul1 = "";
                    //ekKosul1 = VeriErisim.KosulEkle(ekKosul1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "51"));
                    //ekKosul1 = VeriErisim.KosulEkle(ekKosul1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], ">=", 45));//ekonomik yatırımlarda
                    //ekKosul1 = "(" + ekKosul1 + ")";

                    //string ekKosul2 = "";
                    //ekKosul2 = VeriErisim.KosulEkle(ekKosul2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">", "51"));
                    //ekKosul2 = VeriErisim.KosulEkle(ekKosul2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PUAN], ">=", 50));//altyapıda 
                    //ekKosul2 = "(" + ekKosul2 + ")";

                    //kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + ekKosul1 + " OR " + ekKosul2 + ")");
                }
                else if (bilgi.durum != (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ && bilgi.durum != -1)
                {
                    int sonDurum = (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI;
                    if (bilgi.durum == 5 || bilgi.durum == 10)//Merkezde kabul
                        sonDurum = (int)ENUMHibeYatirimBasvuruDurum.UYGULAMASOZLESMESIYAPILDI;

                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], ">=", bilgi.durum));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<=", sonDurum));
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));
            }
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", -2));

            if (!bilgi.sozlesmeTarihBaslangic.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], ">=", bilgi.sozlesmeTarihBaslangic));
            if (!bilgi.sozlesmeTarihBitis.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOZLESMETARIH], "<=", bilgi.sozlesmeTarihBitis));

            if (bilgi.sorguGosterme != "")
            {
                string[] gler = bilgi.sorguGosterme.Split(',');
                string icKosul = "";
                foreach (var item in gler)
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;
                    if (item == "7" || item == "8")
                    {
                        if (item == "7")
                            icKosul = VeriErisim.KosulEkle(icKosul, "AND", "YATIRIM.ILKOD NOT IN ('0105001','0106001','0106002','0107004','0106003','0105004','0105007','0106009','0107009','0105008','0106006')");
                        if (item == "8")
                            icKosul = VeriErisim.KosulEkle(icKosul, "AND", "YATIRIM.ILKOD IN ('0105001','0106001','0106002','0107004','0106003','0105004','0105007','0106009','0107009','0105008','0106006')");

                    }
                    else
                        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KABULCALISMASI], "<>", item));
                }
                if (icKosul != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
            }

            if (gecenler)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANKET1], "=", 1));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANKET2], "=", 1));

                if (bilgi.yatirimOncelikTur == 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANKET3], "=", 1));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANKET4], "=", 1));

                //!!!!
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANKET4], "=", 1));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI));
            }

            return kosul;
        }

        public IDataReader KontrolKimlikNoKayitVarMi(HibeYatirim bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));

            if (bilgi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.kimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", bilgi.kimlikNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KontrolKimlikNoKayitEskiEtaplardaVarMi(HibeYatirim bilgi)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", bilgi.kimlikNo));

            sql += " and (";
            for (int i = 1; i < 3; i++)
            {
                sql += " ETAPNO= " + (bilgi.etapNo - i).ToString();
                if (i == 1)
                    sql += " or ";
            }
            sql += ")";
            sql += " and (DURUM > " + (int)ENUMHibeYatirimBasvuruDurum.ILDEKABULEDILENLER + " or DURUM < " + (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI + " )";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hibe Yatirim HibeTutarlari bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe turar bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetHibeTutarlari(HibeYatirim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASVURUMALIYET], bilgi.basvuruMaliyet) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIBETUTAR], bilgi.hibeTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIBEESASTUTAR], bilgi.hibeEsasTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYNIKATKI], bilgi.ayniKatki) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMTUTAR], bilgi.toplamTutar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Hibe Yatirim Tarihçe bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek tarihçe bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetTarihce(HibeYatirimTarihce bilgi)
        {
            string sablon = "kullaniciKod=\"{0}\" kullaniciAdi=\"{1}\" IP=\"{2}\" tarih=\"{3}\"";
            sablon = string.Format(sablon, bilgi.kullaniciKod, bilgi.kullaniciAdi, bilgi.ipAdresi, bilgi.tarih.ToLongString());
            if (bilgi.aciklama.IndexOf("<tarihce") > -1) bilgi.aciklama = bilgi.aciklama.Replace("[ekBilgiler]", sablon);

            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kullaniciKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.ipAdresi) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Hibe Yatirim Tarihçe bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak hibe Yatirim tarihçe bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hibe Yatirim Tarihçe bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleTarihce(HibeYatirimTarihce kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE) + " , " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASVURUNO] + " , " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DURUM];
            sql += " FROM  " + TABLOADTARIHCE;
            sql += " RIGHT JOIN " + TABLOAD + " ON " + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.HIBEKOD];

            if (kriter.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.HIBEKOD], "=", kriter.hibeKod));
            if (!string.IsNullOrEmpty(kriter.kullaniciKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.KULLANICIKOD], "=", kriter.kullaniciKod));


            if (kosul != "")
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

        public IDataReader DegerlendirmeSayilariniGetir(int etapNo, string ilKod, int yatirimOncelikTur, string altYapiGrup)
        {
            string ekKosul = "";
            string elenen3Kriter = "";
            string yatirimKriter = "";
            if (yatirimOncelikTur == 0)
            {
                ekKosul = "TRMHIBEYATIRIM.PROJEKONU <51";
                yatirimKriter = "and ANKET3=1";
                elenen3Kriter = "and ANKET3=-1";
            }
            else
            {
                ekKosul = "TRMHIBEYATIRIM.PROJEKONU >=51";
                if (etapNo >= 15)
                {
                    if (altYapiGrup == "") altYapiGrup = "B";

                    ekKosul += " AND ALTYAPIGRUP='" + altYapiGrup + "'";
                    //elenen3Kriter = "and ANKET3=-1";
                    //yatirimKriter = "and ANKET4=1";
                    yatirimKriter = "and ANKET3=1";
                    elenen3Kriter = "and ANKET3=-1";
                }

                //!!!!
                //elenen3Kriter = "and ANKET4=-1";
            }

            string sql = @"select COUNT(*), (select COUNT(*) from TRMHIBEYATIRIM where ILKOD= '" + ilKod + "' and ETAPNO= " + etapNo;
            sql += @"   and DURUM=" + (int)ENUMHibeYatirimBasvuruDurum.YENI + " and " + ekKosul + " and ANKET1=-1) as elenen1,";
            sql += @"   (select COUNT(*) from TRMHIBEYATIRIM where ILKOD='" + ilKod + "' and ETAPNO= " + etapNo;
            sql += @"   and DURUM=" + (int)ENUMHibeYatirimBasvuruDurum.YENI + " and " + ekKosul + " and ANKET1>0 and ANKET2=-1) as elenen2,";
            sql += @"   (select COUNT(*) from TRMHIBEYATIRIM where ILKOD='" + ilKod + "' and ETAPNO= " + etapNo;
            sql += @"   and DURUM=" + (int)ENUMHibeYatirimBasvuruDurum.YENI + " and " + ekKosul + " and ANKET1>0 " + elenen3Kriter + ") as elenen3,";
            sql += @"   (select COUNT(*) from TRMHIBEYATIRIM where ILKOD='" + ilKod + "' and ETAPNO= " + etapNo;
            sql += @"   and DURUM=" + (int)ENUMHibeYatirimBasvuruDurum.YENI + " and " + ekKosul + " and ANKET1>0 and ANKET4=-1) as elenen4,";
            sql += @"   (select COUNT(*) from TRMHIBEYATIRIM where ILKOD='" + ilKod + "' and ETAPNO= " + etapNo;

            //!!!!
            //sql += @"   and DURUM=1 and " + ekKosul + " and ANKET1=1 and ANKET2=1 " + yatirimKriter + " and ANKET4=1) as gecen from TRMHIBEYATIRIM where ILKOD='" + ilKod + "' and ETAPNO= " + etapNo;
            sql += @"   and DURUM=1 and " + ekKosul + " and ANKET1=1 and ANKET2=1 " + yatirimKriter + ") as gecen from TRMHIBEYATIRIM where ILKOD='" + ilKod + "' and ETAPNO= " + etapNo;
            sql += @"   and DURUM=" + (int)ENUMHibeYatirimBasvuruDurum.YENI + " and " + ekKosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KimlikBilgisiEksikGuncelle(HibeYatirim hm)
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

        public Sonuc KaydetAnketSonuc(int hibeKod, ENUMAlanlar anketTur, int anketPuan, int anketDurum, int basvuruCesidi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)anketTur], anketDurum);

            if (anketTur == ENUMAlanlar.ANKET1)
            {
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET2] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET3] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET4] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET3PUAN] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET4PUAN] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.PUAN] + " = 0";
            }
            else if (anketTur == ENUMAlanlar.ANKET2)
            {
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET3] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET4] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET3PUAN] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET4PUAN] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.PUAN] + " = 0";
            }
            else if (anketTur == ENUMAlanlar.ANKET3)
            {
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET4] + " = 0";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.ANKET4PUAN] + " = 0";
            }


            if (anketTur == ENUMAlanlar.ANKET3 || anketTur == ENUMAlanlar.ANKET4)
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)anketTur + 2], anketPuan);
            if (anketTur == ENUMAlanlar.ANKET3)
                sql += ", " + ALANLAR[(int)ENUMAlanlar.PUAN] + " = " + anketPuan + " * 1";
            else if (anketTur == ENUMAlanlar.ANKET4)
            {
                if (basvuruCesidi == 2)
                    //sql += ", " + ALANLAR[(int)ENUMAlanlar.PUAN] + " = " + anketPuan;
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.PUAN] + " = " + ALANLAR[(int)ENUMAlanlar.ANKET3PUAN] + " * 1 + " + anketPuan + " * 1";
                else
                    sql += ", " + ALANLAR[(int)ENUMAlanlar.PUAN] + " = " + ALANLAR[(int)ENUMAlanlar.ANKET3PUAN] + " * 1 + " + anketPuan + " * 1";
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc sifirlaAnketSonuc(int hibeKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANKET1], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANKET2], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANKET3], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANKET4], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANKET3PUAN], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANKET4PUAN], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PUAN], 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hibeKod));

            return KomutCalistir(sql, 1);
        }

        public bool YatirimKabulYapilmismi(int etapNo, int basvuruCesidi, int depremIli)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], ">=", (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<", (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ));

            if (basvuruCesidi == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], "<", "50"));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKONU], ">", "50"));

            //if (depremIli > -1)
            //    sql += " AND TRMHIBEYATIRIM.ILKOD IN ('0105001','0106001','0106002','0107004','0106003','0105004','0105007','0106009','0107009','0105008','0106006')";

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        public IDataReader KapasiteBirimListele()
        {
            string sql = "SELECT KAPASITEBIRIM FROM " + TABLOADSOZLESME;
            sql += " GROUP BY KAPASITEBIRIM";
            sql += " ORDER BY KAPASITEBIRIM";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetIsletmePlani(YatirimIsletmePlani bilgi)
        {
            string sql = "UPDATE " + TABLOADISLETMEPLANI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.TOPLAMYATIRIMTUTARI], bilgi.toplamYatirimTutari) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.SABITYATIRIMTUTARI], bilgi.sabitYatirimTutari) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.SERMAYEYATIRIMTUTARI], bilgi.sermayeYatirimTutari) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.ISLETMEGELIRI], bilgi.isletmeGeliri) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.ISLETMEGIDERI], bilgi.isletmeGideri) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.BRUTNAKITAKIM], bilgi.brutNakitAkim) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.ICKARLILIKORANI], bilgi.IcKarlilikOrani) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.FAYDAMASRAFORANI], bilgi.faydaMasrafOrani) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.GERIDONUSURESI], bilgi.geriDonusSuresi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.BRUTKATMADEGER], bilgi.brutKatmaDeger) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.NETKATMADEGER], bilgi.netKatmaDeger) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.BASABASNOKTASI], bilgi.basabasNoktasi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.BASLAMATARIHI], bilgi.baslamaTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.TAMAMLANMATARIHI], bilgi.tamamlanmaTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.MEVCUTKAPASITE], bilgi.mevcutKapasite) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.PROJEILESAGLANACAKKAPASITE], bilgi.projeIleSaglananKapasite) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.PROJESONRASIOLUSACAKKAPASITE], bilgi.projeSonrasiOlacakKapasite) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.MEVCUTISTAIHDAMSAYISI], bilgi.mevcutIstihdam) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.PROJEILESAGLANACAKISTIHDAMSAYISI], bilgi.projeIleSaglananIstihdam) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.PROJESONRASIISTIHDAMSAYISI], bilgi.projeSonrasiIstihdam);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.HIBEKOD], "=", bilgi.hibeKod));

            Sonuc sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOADISLETMEPLANI + " (" + VeriErisim.TumAlanlar("", ALANLARISLETMEPLANI) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.toplamYatirimTutari) + ",";
                sql += VeriErisim.DegerYap(bilgi.sabitYatirimTutari) + ",";
                sql += VeriErisim.DegerYap(bilgi.sermayeYatirimTutari) + ",";
                sql += VeriErisim.DegerYap(bilgi.isletmeGeliri) + ",";
                sql += VeriErisim.DegerYap(bilgi.isletmeGideri) + ",";
                sql += VeriErisim.DegerYap(bilgi.brutNakitAkim) + ",";
                sql += VeriErisim.DegerYap(bilgi.IcKarlilikOrani) + ",";
                sql += VeriErisim.DegerYap(bilgi.faydaMasrafOrani) + ",";
                sql += VeriErisim.DegerYap(bilgi.geriDonusSuresi) + ",";
                sql += VeriErisim.DegerYap(bilgi.brutKatmaDeger) + ",";
                sql += VeriErisim.DegerYap(bilgi.netKatmaDeger) + ",";
                sql += VeriErisim.DegerYap(bilgi.basabasNoktasi) + ",";
                sql += VeriErisim.DegerYap(bilgi.baslamaTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.tamamlanmaTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.mevcutKapasite) + ",";
                sql += VeriErisim.DegerYap(bilgi.projeIleSaglananKapasite) + ",";
                sql += VeriErisim.DegerYap(bilgi.projeSonrasiOlacakKapasite) + ",";
                sql += VeriErisim.DegerYap(bilgi.mevcutIstihdam) + ",";
                sql += VeriErisim.DegerYap(bilgi.projeIleSaglananIstihdam) + ",";
                sql += VeriErisim.DegerYap(bilgi.projeSonrasiIstihdam) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public IDataReader YatirimIsletmePlaniGetir(int hibeKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADISLETMEPLANI, ALANLARISLETMEPLANI);
            sql += " FROM " + TABLOADISLETMEPLANI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARISLETMEPLANI[(int)ENUMAlanlarIsletmePlani.HIBEKOD], "=", hibeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HibeYatirimListForTKDK(string kimlikNo)
        {
            string sql = @"select  TRMHIBEYATIRIM.AD+' '+TRMHIBEYATIRIM.SOYAD, HUKUKISTATU, ORGCOGRAFYA.AD, TRMHIBEYATIRIM.ADRES , TRMYATIRIMKONU.ADI,  PROJEADI, 
                        TRMHIBEYATIRIM.DURUM, TRMHIBEYATIRIM.SOZLESMETARIH, TRMHIBEYATIRIM.YIL, ETAPNO
                        from TRMHIBEYATIRIM
                        left outer join ORGCOGRAFYA on ORGCOGRAFYA.KOD = TRMHIBEYATIRIM.ILKOD
                        left outer join TRMYATIRIMKONU on TRMYATIRIMKONU.KOD = TRMHIBEYATIRIM.PROJEKONU";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", kimlikNo));
            sql += " Order by ETAPNO desc";

            return veriErisim.DataReaderDondur(sql);
        }

        public int BasvuruDurumBilgisi(string basvuruNo)
        {
            string sql = "SELECT DURUM FROM TRMHIBEYATIRIM ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASVURUNO], "=", basvuruNo));
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", OrtakFonksiyonlar.ConvertToInt(basvuruNo, -99999)));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public int MerkezKabulAsamasiGectimi(int etapNo)
        {
            string sql = "SELECT COUNT(*) FROM TRMHIBEYATIRIM ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], ">=", (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<=", (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ));
            return veriErisim.TekKolonSQLInt(sql);
        }
        public Sonuc YatirimciDegistir(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HUKUKISTATU], bilgi.hukukiStatu) + ", ";
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

        public Sonuc BasvuruNoDegistir(HibeYatirim bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASVURUNO], bilgi.basvuruNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader HazineRaporYillik(int yil1, int yil2, int etapNo)
        {
            string sql = "SELECT ILKOD, YEAR(GERSURE), SUM(HIBETUTAR)";
            sql += " FROM HIBEYATIRIMHAZINE";
            sql += " WHERE YEAR(GERSURE)>=" + yil1 + " AND YEAR(GERSURE)<=" + yil2;
            if (etapNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ETAPNO", "=", etapNo));

            sql += " GROUP BY ILKOD, YEAR(GERSURE)";
            sql += " ORDER BY ILKOD, YEAR(GERSURE)";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HazineRaporTuzel(int yil, int ay, int tur, int etapNo)
        {
            TNSDateTime baslama = new TNSDateTime("01." + ay + "." + yil);
            TNSDateTime bitis = new TNSDateTime(baslama.Oku().AddMonths(1));

            string sozlesmeTur = "";
            if (tur == 2) sozlesmeTur = ",SOZLESMETUR";

            string sql = "SELECT  PROJENO, KIMLIKNO, ILKOD, ILCEKOD, YETSEKKOD, MIN(SOZLESMETARIH), MIN(GERSURE), TARIH1, TARIH2, DESTEKUNSUR, DURUM, ANAHTAR, AD, SUM(HIBEESASTUTAR), SUM(HIBETUTAR)" + sozlesmeTur + " FROM HIBEYATIRIMHAZINETUZEL";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "GERSURE", ">=", baslama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GERSURE", "<", bitis));

            if (etapNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ETAPNO", "=", etapNo));

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

            string sql = "SELECT  PROJENO, KIMLIKNO, ILKOD, ILCEKOD, YETSEKKOD, MIN(SOZLESMETARIH), MIN(GERSURE), MIN(TARIH1), MAX(TARIH2), DESTEKUNSUR, DURUM, ANAHTAR, AD, SUM(HIBEESASTUTAR), SUM(HIBETUTAR)" + sozlesmeTur + " FROM HIBEYATIRIMHAZINEGERCEK";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "GERSURE", ">=", baslama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GERSURE", "<", bitis));

            if (etapNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ETAPNO", "=", etapNo));

            sql += " GROUP BY PROJENO, KIMLIKNO, ILKOD, ILCEKOD, DESTEKUNSUR, YETSEKKOD, DURUM, ANAHTAR, AD" + sozlesmeTur;
            sql += " ORDER BY PROJENO, ILKOD, ILCEKOD, KIMLIKNO" + sozlesmeTur;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetEksikBelge(int kod, int hibeKod, string belgeAdi, int belgeNo)
        {
            string sql = "INSERT INTO " + TABLOADEKSIKBELGE + " (" + VeriErisim.TumAlanlar("", ALANLAREKSIKBELGE) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(hibeKod) + ",";
            sql += VeriErisim.DegerYap(belgeAdi) + ",";
            sql += VeriErisim.DegerYap(belgeNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilEksikBelge(int hibeKod)
        {
            string sql = "DELETE FROM " + TABLOADEKSIKBELGE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAREKSIKBELGE[(int)ENUMAlanlarEksikBelge.HIBEKOD], "=", hibeKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleEksikBelge(int hibeKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAREKSIKBELGE);
            sql += " FROM " + TABLOADEKSIKBELGE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAREKSIKBELGE[(int)ENUMAlanlarEksikBelge.HIBEKOD], "=", hibeKod));
            sql += " ORDER BY " + TABLOADEKSIKBELGE + "." + ALANLAREKSIKBELGE[(int)ENUMAlanlarEksikBelge.BELGENO];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetGoruntu(Goruntu s)
        {
            string sql = "INSERT INTO " + TABLOADGORUNTU + " (" + VeriErisim.TumAlanlar("", ALANLARGORUNTU) + ") VALUES (";
            sql += VeriErisim.DegerYap(s.hibeKod) + ",";
            sql += VeriErisim.DegerYap(s.basvuruNo) + ",";
            sql += VeriErisim.DegerYap(s.ePosta) + ",";
            sql += VeriErisim.DegerYap(s.kamera1Adres) + ",";
            sql += VeriErisim.DegerYap(s.kamera1Kullanici) + ",";
            sql += VeriErisim.DegerYap(s.kamera1Parola) + ",";
            sql += VeriErisim.DegerYap(s.kamera2Adres) + ",";
            sql += VeriErisim.DegerYap(s.kamera2Kullanici) + ",";
            sql += VeriErisim.DegerYap(s.kamera2Parola) + ",";
            sql += VeriErisim.DegerYap(s.kamera3Adres) + ",";
            sql += VeriErisim.DegerYap(s.kamera3Kullanici) + ",";
            sql += VeriErisim.DegerYap(s.kamera3Parola) + ",";
            sql += VeriErisim.DegerYap(s.kamera4Adres) + ",";
            sql += VeriErisim.DegerYap(s.kamera4Kullanici) + ",";
            sql += VeriErisim.DegerYap(s.kamera4Parola) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADGORUNTU + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.EPOSTAADRESI], s.ePosta) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA1ADRES], s.kamera1Adres) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA1KULLANICIADI], s.kamera1Kullanici) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA1PAROLA], s.kamera1Parola) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA2ADRES], s.kamera2Adres) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA2KULLANICIADI], s.kamera2Kullanici) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA2PAROLA], s.kamera2Parola) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA3ADRES], s.kamera3Adres) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA3KULLANICIADI], s.kamera3Kullanici) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA3PAROLA], s.kamera3Parola) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA4ADRES], s.kamera4Adres) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA4KULLANICIADI], s.kamera4Kullanici) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.KAMERA4PAROLA], s.kamera4Parola);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.HIBEKOD], "=", s.hibeKod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public IDataReader ListeleGoruntu(Goruntu goruntu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARGORUNTU);
            sql += " FROM " + TABLOADGORUNTU;

            if (goruntu.hibeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.HIBEKOD], "=", goruntu.hibeKod));
            if (goruntu.ePosta != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.EPOSTAADRESI], "=", goruntu.ePosta));

            return veriErisim.DataReaderDondur(sql);
        }

        public bool GoruntuEPostaAdresiVarmi(Goruntu goruntu)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADGORUNTU;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.EPOSTAADRESI], "=", goruntu.ePosta));

            if (goruntu.hibeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGORUNTU[(int)ENUMAlanlarGoruntu.HIBEKOD], "<>", goruntu.hibeKod));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }


        public Sonuc KaydetSozlesmeFesih(HibeYatirimSozlesmeFesih bilgi)
        {
            string sql = "INSERT INTO " + TABLOADSOZLESMEFESIH + " (" + VeriErisim.TumAlanlar("", ALANLARSOZLESMEFESIH) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemYapanAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemTarihi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleSozlesmeFesih(HibeYatirimSozlesmeFesih bilgi)
        {
            string sql = "UPDATE " + TABLOADSOZLESMEFESIH + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.TARIH], bilgi.tarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.DURUM], bilgi.durum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.ACIKLAMA], bilgi.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.TUTAR], bilgi.tutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.ISLEMYAPANADI], bilgi.islemYapanAdi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.ISLEMTARIHI], bilgi.islemTarihi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.KOD], "=", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.HIBEKOD], "=", bilgi.hibeKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilSozlesmeFesih(HibeYatirimSozlesmeFesih bilgi)
        {
            string sql = "DELETE FROM " + TABLOADSOZLESMEFESIH;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.HIBEKOD], "=", bilgi.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleSozlesmeFesih(HibeYatirimSozlesmeFesih bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSOZLESMEFESIH, ALANLARSOZLESMEFESIH) + " FROM " + TABLOADSOZLESMEFESIH;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.HIBEKOD], "=", bilgi.hibeKod));

            if (bilgi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.KOD], "=", bilgi.kod));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SozlesmeFesihListele(HibeYatirimSozlesmeFesih bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("YATIRIM", ALANLAR) + ", TRMYATIRIMNITELIK.ADI, TRMYATIRIMKONU.ADI, TRMYATIRIMKONU_1.ADI, TRMKOY.ADI, ORGCOGRAFYA.AD AS ILAD, ORGCOGRAFYA_1.AD AS ILCEAD, HIBEYATIRIMODENEN.ODENENINSAAT, HIBEYATIRIMODENEN.ODENENMAL,TRMHIBEYATIRIMSOZLESMEFESIH.TARIH, TRMHIBEYATIRIMSOZLESMEFESIH.DURUM,TRMHIBEYATIRIMSOZLESMEFESIH.TUTAR,TRMHIBEYATIRIMSOZLESMEFESIH.ACIKLAMA," + VeriErisim.TumAlanlar(TABLOADSOZLESME, ALANLARSOZLESME) + "," + VeriErisim.TumAlanlar(TABLOADMALIYET, ALANLARMALIYET);
            sql += " , (SELECT COUNT(*) FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.DURUM>=10 AND ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO<YATIRIM.ETAPNO)";
            sql += " , (SELECT COUNT(*) FROM TRMHIBEYATIRIMSOZLESME AS SOZLESME WHERE SOZLESME.DURUM=1 AND SOZLESME.HIBEKOD IN (SELECT KOD FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO<YATIRIM.ETAPNO))";
            sql += " , (SELECT COUNT(*) FROM TRMHIBEYATIRIMSOZLESME AS SOZLESME WHERE SOZLESME.DURUM=2 AND SOZLESME.HIBEKOD IN (SELECT KOD FROM TRMHIBEYATIRIM AS ICYATIRIM WHERE ICYATIRIM.KIMLIKNO=YATIRIM.KIMLIKNO AND ICYATIRIM.ETAPNO<YATIRIM.ETAPNO))";
            sql += " FROM TRMHIBEYATIRIM AS YATIRIM LEFT OUTER JOIN";
            sql += " HIBEYATIRIMODENEN ON YATIRIM.KOD = HIBEYATIRIMODENEN.HIBEKOD LEFT OUTER JOIN";
            sql += " TRMHIBEYATIRIMMALIYET ON YATIRIM.KOD = TRMHIBEYATIRIMMALIYET.HIBEKOD AND TRMHIBEYATIRIMMALIYET.DURUM = " + (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI + " LEFT OUTER JOIN";
            sql += " TRMHIBEYATIRIMSOZLESME ON YATIRIM.KOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD LEFT OUTER JOIN";
            sql += " TRMYATIRIMKONU AS TRMYATIRIMKONU_1 ON YATIRIM.PROJEALTKONU = TRMYATIRIMKONU_1.KOD LEFT OUTER JOIN";
            sql += " TRMYATIRIMNITELIK ON YATIRIM.PROJENITELIK = TRMYATIRIMNITELIK.KOD LEFT OUTER JOIN";
            sql += " TRMYATIRIMKONU ON YATIRIM.PROJEKONU = TRMYATIRIMKONU.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA ON YATIRIM.ILKOD = ORGCOGRAFYA.KOD LEFT OUTER JOIN";
            sql += " ORGCOGRAFYA AS ORGCOGRAFYA_1 ON YATIRIM.ILCEKOD = ORGCOGRAFYA_1.KOD LEFT OUTER JOIN";
            sql += " TRMKOY ON YATIRIM.KOYKOD = TRMKOY.KOD";
            sql += " INNER JOIN TRMHIBEYATIRIMSOZLESMEFESIH ON TRMHIBEYATIRIMSOZLESMEFESIH.HIBEKOD = TRMHIBEYATIRIMSOZLESME.HIBEKOD";

            if (bilgi.durum > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESMEFESIH", "DURUM", "=", bilgi.durum));

            if (!bilgi.tarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "SOZLESMETARIH", ">=", bilgi.tarih));

            if (!bilgi.islemTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIMSOZLESME", "SOZLESMETARIH", "<=", bilgi.islemTarihi));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SozlesmeHakKazananlar(HibeYatirim bilgi)
        {
            string sql = "SELECT TRMHIBEYATIRIM.KOD, TRMHIBEYATIRIM.BASVURUNO, TRMHIBEYATIRIM.HUKUKISTATU, TRMHIBEYATIRIM.ALTYAPIGRUP, TRMYATIRIMKONU.ADI AS ProjeKonuAdi, ";
            sql += " TRMYATIRIMKONU_1.ADI AS AltKonuAdi, TRMYATIRIMNITELIK.ADI AS NitelikAdi, ORGCOGRAFYA.AD AS İlAdı, ORGCOGRAFYA_1.AD AS İlçeAdı, ";
            sql += " TRMHIBEYATIRIM.KIMLIKNO, TRMHIBEYATIRIM.AD, TRMHIBEYATIRIM.SOYAD, TRMHIBEYATIRIM.EPOSTA, TRMHIBEYATIRIM.DOGUMTARIH, ";
            sql += " TRMHIBEYATIRIM.PROJENITELIK, TRMHIBEYATIRIM.PROJEKONU, TRMHIBEYATIRIM.PROJEALTKONU, TRMHIBEYATIRIM.PROJEADI, ";
            sql += " TRMHIBEYATIRIM.BASVURUMALIYET, TRMHIBEYATIRIM.HIBETUTAR, TRMHIBEYATIRIM.HIBEESASTUTAR, TRMHIBEYATIRIM.AYNIKATKI, ";
            sql += " TRMHIBEYATIRIM.TOPLAMTUTAR, TRMHIBEYATIRIM.PNITELIK, TRMHIBEYATIRIM.ISTIHDAM, PUAN, ANKET1,ANKET2,ANKET3,ANKET4,KABULCALISMASI, MALZEME";
            sql += " FROM TRMHIBEYATIRIM ";
            sql += " INNER JOIN ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD ";
            sql += " INNER JOIN ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEYATIRIM.ILCEKOD = ORGCOGRAFYA_1.KOD ";
            sql += " INNER JOIN TRMYATIRIMKONU ON TRMHIBEYATIRIM.PROJEKONU = TRMYATIRIMKONU.KOD ";
            sql += " INNER JOIN TRMYATIRIMKONU AS TRMYATIRIMKONU_1 ON TRMHIBEYATIRIM.PROJEALTKONU = TRMYATIRIMKONU_1.KOD ";
            sql += " INNER JOIN TRMYATIRIMNITELIK ON TRMHIBEYATIRIM.PROJENITELIK = TRMYATIRIMNITELIK.KOD";
            //sql += " LEFT JOIN TRMYATIRIMMALZEME ON TRMYATIRIMMALZEME.HIBEKOD = TRMHIBEYATIRIM.KOD";
            //sql += " LEFT JOIN TRMYATIRIMMALZEMETANIM ON TRMYATIRIMMALZEMETANIM.KOD = TRMYATIRIMMALZEME.MALZEMEKOD";
            //sql += " LEFT JOIN TRMYATIRIMMALZEMETUR ON TRMYATIRIMMALZEMETANIM.MALZEMETURKOD = TRMYATIRIMMALZEMETUR.KOD";

            sql += " CROSS APPLY(SELECT TRMYATIRIMMALZEMETUR.ADI +'^'+ TRMYATIRIMMALZEMETANIM.ADI +'|' FROM TRMYATIRIMMALZEME LEFT OUTER JOIN TRMYATIRIMMALZEMETANIM ON TRMYATIRIMMALZEMETANIM.KOD = TRMYATIRIMMALZEME.MALZEMEKOD LEFT OUTER JOIN TRMYATIRIMMALZEMETUR ON TRMYATIRIMMALZEMETUR.KOD = TRMYATIRIMMALZEMETANIM.MALZEMETURKOD  WHERE TRMYATIRIMMALZEME.HIBEKOD = TRMHIBEYATIRIM.KOD ORDER BY TRMYATIRIMMALZEME.OZELLIK DESC FOR XML PATH('')) MAL(MALZEME)";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", ">", 0));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", "<", 99));
            //sql += " AND ((TRMHIBEYATIRIM.PROJEKONU < '51' AND TRMHIBEYATIRIM.PUAN >= 45) OR (TRMHIBEYATIRIM.PROJEKONU > '51' AND TRMHIBEYATIRIM.PUAN >= 50)) ";
            sql += " AND (TRMHIBEYATIRIM.ANKET1 = 1 AND TRMHIBEYATIRIM.ANKET2 = 1 AND TRMHIBEYATIRIM.ANKET4=1)";

            if (bilgi.ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ILKOD", "=", bilgi.ilKod));

            if (bilgi.projeKonu == "50")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", "<=", 50));

            if (bilgi.projeKonu == "52")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", ">=", 50));

            if (bilgi.altYapiGrup != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ALTYAPIGRUP", "=", bilgi.altYapiGrup));

            if (bilgi.cinsiyet != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "CINSIYET", "=", bilgi.cinsiyet));

            if (bilgi.engelli != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ENGELLI", "=", bilgi.engelli));

            if (bilgi.organik != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ORGANIK", "=", bilgi.organik));

            if (bilgi.kabulCalismasi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "KABULCALISMASI", "=", bilgi.kabulCalismasi));

            if (bilgi.basvuruYapildi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", "=", (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI));

            //sql += " ORDER BY KABULCALISMASI,ORGCOGRAFYA_1.AD,TRMHIBEYATIRIM.AD, TRMHIBEYATIRIM.SOYAD";
            sql += " ORDER BY KABULCALISMASI,PUAN DESC,TRMHIBEYATIRIM.BASVURUNO";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IldeReddilenler(HibeYatirim bilgi)
        {
            string sql = "SELECT TRMHIBEYATIRIM.KOD, TRMHIBEYATIRIM.BASVURUNO, TRMHIBEYATIRIM.HUKUKISTATU, TRMHIBEYATIRIM.ALTYAPIGRUP, TRMYATIRIMKONU.ADI AS ProjeKonuAdi, ";
            sql += " TRMYATIRIMKONU_1.ADI AS AltKonuAdi, TRMYATIRIMNITELIK.ADI AS NitelikAdi, ORGCOGRAFYA.AD AS İlAdı, ORGCOGRAFYA_1.AD AS İlçeAdı, ";
            sql += " TRMHIBEYATIRIM.KIMLIKNO, TRMHIBEYATIRIM.AD, TRMHIBEYATIRIM.SOYAD, TRMHIBEYATIRIM.EPOSTA, TRMHIBEYATIRIM.DOGUMTARIH, ";
            sql += " TRMHIBEYATIRIM.PROJENITELIK, TRMHIBEYATIRIM.PROJEKONU, TRMHIBEYATIRIM.PROJEALTKONU, TRMHIBEYATIRIM.PROJEADI, ";
            sql += " TRMHIBEYATIRIM.BASVURUMALIYET, TRMHIBEYATIRIM.HIBETUTAR, TRMHIBEYATIRIM.HIBEESASTUTAR, TRMHIBEYATIRIM.AYNIKATKI, ";
            sql += " TRMHIBEYATIRIM.TOPLAMTUTAR, TRMHIBEYATIRIM.PNITELIK, TRMHIBEYATIRIM.ISTIHDAM, PUAN, ANKET1,ANKET2,ANKET3,ANKET4,KABULCALISMASI";
            sql += " FROM TRMHIBEYATIRIM ";
            sql += " INNER JOIN ORGCOGRAFYA ON TRMHIBEYATIRIM.ILKOD = ORGCOGRAFYA.KOD ";
            sql += " INNER JOIN ORGCOGRAFYA AS ORGCOGRAFYA_1 ON TRMHIBEYATIRIM.ILCEKOD = ORGCOGRAFYA_1.KOD ";
            sql += " INNER JOIN TRMYATIRIMKONU ON TRMHIBEYATIRIM.PROJEKONU = TRMYATIRIMKONU.KOD ";
            sql += " INNER JOIN TRMYATIRIMKONU AS TRMYATIRIMKONU_1 ON TRMHIBEYATIRIM.PROJEALTKONU = TRMYATIRIMKONU_1.KOD ";
            sql += " INNER JOIN TRMYATIRIMNITELIK ON TRMHIBEYATIRIM.PROJENITELIK = TRMYATIRIMNITELIK.KOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ETAPNO", "=", bilgi.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "DURUM", "=", 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "KABULCALISMASI", "=", 0));

            if (bilgi.ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ILKOD", "=", bilgi.ilKod));

            if (bilgi.projeKonu == "50")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", "<=", 50));

            if (bilgi.projeKonu == "52")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "PROJEKONU", ">=", 50));

            if (bilgi.altYapiGrup != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEYATIRIM", "ALTYAPIGRUP", "=", bilgi.altYapiGrup));



            sql += " ORDER BY BASVURUNO";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetYatirimciKimlikNo(HibeYatirimYatirimciKimlikNo bilgi)
        {
            string sql = "INSERT INTO " + TABLOADYATIRIMCIKIMLIKNO + " (" + VeriErisim.TumAlanlar("", ALANLARYATIRIMCIKIMLIKNO) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.eskiKimlikNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.yeniKimlikNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleYatirimciKimlikNo(string kimlikNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADYATIRIMCIKIMLIKNO, ALANLARYATIRIMCIKIMLIKNO) + " FROM " + TABLOADYATIRIMCIKIMLIKNO;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYATIRIMCIKIMLIKNO[(int)ENUMAlanlarYatirimciKimlikNo.ESKIKIMLIKNO], "=", kimlikNo));
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLARYATIRIMCIKIMLIKNO[(int)ENUMAlanlarYatirimciKimlikNo.YENIKIMLIKNO], "=", kimlikNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleKonuKod(int hibeKod)
        {
            string sql = "SELECT PROJEKONU FROM TRMHIBEYATIRIM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEYATIRIM", "KOD", "=", hibeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public string GetirYatirimciKimlikNo(int hibeKod)
        {
            string sql = "SELECT YENIKIMLIKNO FROM " + TABLOADYATIRIMCIKIMLIKNO;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARYATIRIMCIKIMLIKNO[(int)ENUMAlanlarYatirimciKimlikNo.HIBEKOD], "=", hibeKod));


            return veriErisim.TekKolonSQLStr(sql);
        }

        public Sonuc GuncelleYatirimciKimlikNo(HibeYatirimYatirimciKimlikNo bilgi)
        {
            string sql = "UPDATE " + TABLOADYATIRIMCIKIMLIKNO + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARYATIRIMCIKIMLIKNO[(int)ENUMAlanlarYatirimciKimlikNo.ESKIKIMLIKNO], bilgi.eskiKimlikNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARYATIRIMCIKIMLIKNO[(int)ENUMAlanlarYatirimciKimlikNo.YENIKIMLIKNO], bilgi.yeniKimlikNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYATIRIMCIKIMLIKNO[(int)ENUMAlanlarYatirimciKimlikNo.HIBEKOD], "=", bilgi.hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARYATIRIMCIKIMLIKNO[(int)ENUMAlanlarYatirimciKimlikNo.YENIKIMLIKNO], "=", bilgi.eskiKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetSoru(HibeYatirimSoru bilgi)
        {
            string sql = "INSERT INTO " + TABLOADSORU + " (" + VeriErisim.TumAlanlar("", ALANLARSORU) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.soruKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.deger) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilSoru(int hibeKod)
        {
            string sql = "DELETE FROM " + TABLOADSORU;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSoru.HIBEKOD], "=", hibeKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleSoru(int hibeKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSORU, ALANLARSORU) + " FROM " + TABLOADSORU;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSORU[(int)ENUMAlanlarSoru.HIBEKOD], "=", hibeKod));

            return veriErisim.DataReaderDondur(sql);

        }

        public int EnBuyukKod()
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLARSOZLESMEFESIH[(int)ENUMAlanlarSozlesmeFesih.KOD] + ")";
            sql += " FROM " + TABLOADSOZLESMEFESIH;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }
    }
}
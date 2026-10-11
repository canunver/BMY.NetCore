using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABPersonel : TABOrtak
    {
        public TABPersonel(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "PERPERSONEL";
        public enum ENUMAlanlar : int
        {
            TCKIMLIKNO = 0,
            SICILNO = 1,
            AD = 2,
            SOYAD = 3,
            BABAAD = 4,
            ANAAD = 5,
            DOGUMTARIHI = 6,
            CINSIYET = 7,
            MEDENIHALI = 8,
            KANGRUBU = 9,
            ISTELEFON = 10,
            CEPTELEFON = 11,
            FAKS = 12,
            EPOSTA = 13,
            ASKERLIKKODU = 14,
            OGRENIMKODU = 15,
            ESDURUMUKODU = 16,
            SAGLIKKARNENO = 17,
            DURUM = 18,
            EMEKLISICILNO = 19,
            GUNCELLENMETARIHI = 20,
            GUNCELLEYENKISI = 21,
            BANKAKODU = 22,
            BANKASUBEKODU = 23,
            BANKAHESAPNO = 24,
            TESKILATKODU = 25,
            UNVANKODU = 26,
            MUHASEBEKODU = 27,
            BIRIMKODU = 28,
            MEMURBASLAMATARIHI = 29,
            ISEBASLAMATARIHI = 30,
            ISTENCIKISTARIHI = 31,
            SINIFKODU = 32,
            KIDEMAY = 33,
            KIDEMYIL = 34,
            KADRODERECE = 35,
            KADROTARIH = 36,
            ODEMEESASDERECE = 37,
            ODEMEESASKADEME = 38,
            ODEMEESASGOSTERGE = 39,
            ODEMEESASTARIH = 40,
            EMEKLIESASDERECE = 41,
            EMEKLIESASKADEME = 42,
            EMEKLIESASGOSTERGE = 43,
            EMEKLIESASTARIH = 44,
            KAZANILMISHAKDERECE = 45,
            KAZANILMISHAKKADEME = 46,
            KAZANILMISHAKTARIH = 47,
            ISTIHDAMSEKLI = 48,
            ADRES = 49,
            EVTELEFON = 50,
            VERGIDAIRESI = 51,
            USTBIRIMKODU = 52,
            EVRAKBIRIMKOD = 53,
            ODA = 54,
            ILKSOYAD = 55,
            DOGUMYERI = 56,
            NUFUSILKOD = 57,
            NUFUSILCEKOD = 58,
            CUZDANSERINO = 59,
            CUZDANKAYITNO = 60,
            NUFUSMAHALLEKOY = 61,
            NUFUSCILTNO = 62,
            NUFUSSIRANO = 63,
            NUFUSAILESIRANO = 64,
            CUZDANVERILISNEDENI = 65,
            CUZDANVERILDIGIYER = 66,
            CUZDANVERILISTARIHI = 67,
            BAKANLIKKARTNO = 68,
            SSKSICILNO = 69,
            BAGKURSICILNO = 70,
            SURUCUBELGENO = 71,
            SURUCUBELGETARIHI = 72,
            YASTASHIHTARIHI = 73,
            ORGANIZASYONKOD = 74,
            KURUM = 75,
            URL = 76,
            MOBILIMZATELEFON = 77,
            TECILTARIHI = 78,
            ASKERLIKMUAFNEDEN = 79,
            ASKERLIKBASLAMATARIHI = 80,
            TERHISTARIHI = 81,
            ASKERLIKCUZDANNO = 82,
            MULKDURUM = 83,
            TASINMATARIHI = 84,
            OTURMASURESI = 85,
            SONGOREVBASLAMATARIHI = 86,
            SONBIRIMBASLAMATARIHI = 87,
            KADROORGANIZASYONKOD = 88,
            KADROMUHASEBEKODU = 89,
            KADROBIRIMKODU = 90,
            KADROKURUM = 91,
            TEBLIGATADRES = 92,
            ACILKISI = 93,
            ACILKISINUMARA = 94,
            KAZANILMISHAKGOSTERGE = 95,
            ODEMEESASEKGOSTERGE = 96,
            EMEKLIESASEKGOSTERGE = 97,
            KAZANILMISHAKEKGOSTERGE = 98,
            AKTIFMI = 99,
            KOD = 100


        }
        public static string[] ALANLAR = { "TCKIMLIKNO", "SICILNO", "AD", "SOYAD", "BABAAD", "ANAAD", "DOGUMTARIHI", "CINSIYET", "MEDENIHALI", "KANGRUBU", "ISTELEFON", "CEPTELEFON", "FAKS", "EPOSTA", "ASKERLIKKODU", "OGRENIMKODU", "ESDURUMUKODU", "SAGLIKKARNENO", "DURUM", "EMEKLISICILNO", "GUNCELLENMETARIHI", "GUNCELLEYENKISI", "BANKAKODU", "BANKASUBEKODU", "BANKAHESAPNO", "TESKILATKODU", "UNVANKODU", "MUHASEBEKODU", "BIRIMKODU", "MEMURBASLAMATARIHI", "ISEBASLAMATARIHI", "ISTENCIKISTARIHI", "SINIFKODU", "KIDEMAY", "KIDEMYIL", "KADRODERECE", "KADROTARIH", "ODEMEESASDERECE", "ODEMEESASKADEME", "ODEMEESASGOSTERGE", "ODEMEESASTARIH", "EMEKLIESASDERECE", "EMEKLIESASKADEME", "EMEKLIESASGOSTERGE", "EMEKLIESASTARIH", "KAZANILMISHAKDERECE", "KAZANILMISHAKKADEME", "KAZANILMISHAKTARIH", "ISTIHDAMSEKLI", "ADRES", "EVTELEFON", "VERGIDAIRESI", "USTBIRIMKODU", "EVRAKBIRIMKOD", "ODA", "ILKSOYAD", "DOGUMYERI", "NUFUSILKOD", "NUFUSILCEKOD", "CUZDANSERINO", "CUZDANKAYITNO", "NUFUSMAHALLEKOY", "NUFUSCILTNO", "NUFUSSIRANO", "NUFUSAILESIRANO", "CUZDANVERILISNEDENI", "CUZDANVERILDIGIYER", "CUZDANVERILISTARIHI", "BAKANLIKKARTNO", "SSKSICILNO", "BAGKURSICILNO", "SURUCUBELGENO", "SURUCUBELGETARIHI", "YASTASHIHTARIHI", "ORGANIZASYONKOD", "KURUM", "URL", "MOBILIMZATELEFON", "TECILTARIHI", "ASKERLIKMUAFNEDEN", "ASKERLIKBASLAMATARIHI", "TERHISTARIHI", "ASKERLIKCUZDANNO", "MULKDURUM", "TASINMATARIHI", "OTURMASURESI", "SONGOREVBASLAMATARIHI", "SONBIRIMBASLAMATARIHI", "KADROORGANIZASYONKOD", "KADROMUHASEBEKODU", "KADROBIRIMKODU", "KADROKURUM", "TEBLIGATADRES", "ACILKISI", "ACILKISINUMARA", "KAZANILMISHAKGOSTERGE", "ODEMEESASEKGOSTERGE", "EMEKLIESASEKGOSTERGE", "KAZANILMISHAKEKGOSTERGE", "AKTIFMI", "KOD" };

        static string TABLOADBELGELER = "PERPERSONELBELGELER";
        public enum ENUMAlanlarBelgeler : int
        {
            TCKIMLIKNO = 0,
            TIP = 1,
            KAYITSIRANO = 2,
            BELGESIRANO = 3,
            BELGEADI = 4,
            BELGE = 5
        }
        static string[] ALANLARBELGELER = { "TCKIMLIKNO", "TIP", "KAYITSIRANO", "BELGESIRANO", "BELGEADI", "BELGE" };

        static string TABLOADAILE = "PERPERSONELAILE";
        public enum ENUMAlanlarAile : int
        {
            PERSONELTCKIMLIKNO = 0,
            TCKIMLIKNO = 1,
            AD = 2,
            SOYAD = 3,
            DOGUMTARIHI = 4,
            CINSIYET = 5,
            YAKINLIKDERECESI = 6,
            DURUM = 7,
            SAGLIKKARNENO = 8,
            EVLENMETARIHI = 9,
            AILECUZDANISERINO = 10,
            ESININCALISTIGIKURUM = 11,
            ESININUNVANI = 12,
            ESININGOREVYERI = 13,
            COCUGUNUNOKULUNUNADI = 14
        }
        static string[] ALANLARAILE = { "PERSONELTCKIMLIKNO", "TCKIMLIKNO", "AD", "SOYAD", "DOGUMTARIHI", "CINSIYET", "YAKINLIKDERECESI", "DURUM", "SAGLIKKARNENO", "EVLENMETARIHI", "AILECUZDANISERINO", "ESININCALISTIGIKURUM", "ESININUNVANI", "ESININGOREVYERI", "COCUGUNUNOKULUNUNADI" };

        static string TABLOADMALBILDIRIMI = "PERPERSONELMALBILDIRIMI";
        public enum ENUMAlanlarMalBildirimi : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            TARIH = 2,
            ACIKLAMA = 3
        }
        static string[] ALANLARMALBILDIRIMI = { "TCKIMLIKNO", "SIRANO", "TARIH", "ACIKLAMA" };

        static string TABLOADCEZAODUL = "PERPERSONELCEZAODUL";
        public enum ENUMAlanlarCezaOdul : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            TIP = 2,
            CEZAODULTIP = 3,
            VERILISNEDENI = 4,
            TARIH = 5,
            BELGENO = 6,
            DAYANAK = 7,
            BELGETARIHI = 8
        }
        static string[] ALANLARCEZAODUL = { "TCKIMLIKNO", "SIRANO", "TIP", "CEZAODULTIP", "VERILISNEDENI", "TARIH", "BELGENO", "DAYANAK", "BELGETARIHI" };

        public static string TABLOADOGRENIM = "PERPERSONELOGRENIM";
        public enum ENUMAlanlarOgrenim : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            OGRENIMKOD = 2,
            OKULADI = 3,
            FAKULTEBOLUM = 4,
            BELGETUR = 5,
            BELGETARIHI = 6,
            BELGENO = 7,
            BASLANGICTARIHI = 8,
            BITIRMETARIHI = 9,
            DERECE = 10,
            GECERLI = 11
        }
        public static string[] ALANLAROGRENIM = { "TCKIMLIKNO", "SIRANO", "OGRENIMKOD", "OKULADI", "FAKULTEBOLUM", "BELGETUR", "BELGETARIHI", "BELGENO", "BASLANGICTARIHI", "BITIRMETARIHI", "DERECE", "GECERLI" };

        static string TABLOADDIL = "PERPERSONELDIL";
        public enum ENUMAlanlarDil : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            DILKODU = 2,
            DUZEY = 3,
            PUAN = 4,
            DONEMAY = 5,
            DONEMYIL = 6,
            BELGETUR = 7,
            BELGETARIHI = 8,
            BELGENO = 9,
            BORDROYAESAS = 10
        }
        static string[] ALANLARDIL = { "TCKIMLIKNO", "SIRANO", "DILKODU", "DUZEY", "PUAN", "DONEMAY", "DONEMYIL", "BELGETUR", "BELGETARIHI", "BELGENO", "BORDROYAESAS" };


        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOADMEMURIYET = "PERPERSONELMEMURIYET";
        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        public enum ENUMAlanlarMemuriyet : int
        {
            /// <summary>
            /// T.C. Kimlik Numarası
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// SIRANO
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// Organizasyon kodu
            /// </summary>
            ORGANIZASYONKOD = 2,
            /// <summary>
            /// Muhasebe Kodu
            /// </summary>
            MUHASEBEKODU = 3,
            /// <summary>
            /// Birim Kodu
            /// </summary>
            BIRIMKODU = 4,
            /// <summary>
            /// Kurumsal Kodu
            /// </summary>
            KURUM = 5,
            /// <summary>
            /// Birim Adı
            /// </summary>
            BIRIMAD = 6,
            /// <summary>
            /// Görev
            /// </summary>
            GOREV = 7,
            /// <summary>
            /// İstihdam şekli
            /// </summary>
            ISTIHDAMSEKLI = 8,
            /// <summary>
            /// Açıklama
            /// </summary>
            ACIKLAMA = 9,
            /// <summary>
            /// Başlama Tarihi
            /// </summary>
            BASLAMATARIHI = 10,
            /// <summary>
            /// Ayrılma Tarihi
            /// </summary>
            AYRILMATARIHI = 11,
            /// <summary>
            /// Başlama Nedeni
            /// </summary>
            BASLAMANEDENI = 12,
            /// <summary>
            /// Ayrılma Nedeni
            /// </summary>
            AYRILMANEDENI = 13,
            /// <summary>
            /// Evrak Numarası
            /// </summary>
            EVRAKNO = 14,
            /// <summary>
            /// Yasal Dayanak
            /// </summary>
            YASALDAYANAK = 15,
            /// <summary>
            /// Onay Tarihi
            /// </summary>
            ONAYTARIH = 16,
            KADRODERECE = 17,
            KADROTARIH = 18,
            ODEMEYEESASDERECE = 19,
            ODEMEYEESASKADEME = 20,
            ODEMEYEESASTARIH = 21,
            KAZANILMISHAKDERECE = 22,
            KAZANILMISHAKKADEME = 23,
            KAZANILMISHAKTARIH = 24,
            EMEKLIYEESASDERECE = 25,
            EMEKLIYEESASKADEME = 26,
            EMEKLIYEESASTARIH = 27,
            ODEMEYEESASGOSTERGE = 28,
            ODEMEYEESASEKGOSTERGE = 29,
            KAZANILMISGOSTERGE = 30,
            KAZANILMISEKGOSTERGE = 31,
            EMEKLIYEESASGOSTERGE = 32,
            EMEKLIYEESASEKGOSTERGE = 33,
            MALUL = 34,
            DAIREKODU = 35
        }
        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLARMEMURIYET = { "TCKIMLIKNO", "SIRANO", "ORGANIZASYONKOD", "MUHASEBEKODU", "BIRIMKODU", "KURUM", "BIRIMAD", "GOREV", "ISTIHDAMSEKLI", "ACIKLAMA", "BASLAMATARIHI", "AYRILMATARIHI", "BASLAMANEDENI", "AYRILMANEDENI", "EVRAKNO", "YASALDAYANAK", "ONAYTARIH", "KADRODERECE", "KADROTARIH", "ODEMEYEESASDERECE", "ODEMEYEESASKADEME", "ODEMEYEESASTARIH", "KAZANILMISHAKDERECE", "KAZANILMISHAKKADEME", "KAZANILMISHAKTARIH", "EMEKLIYEESASDERECE", "EMEKLIYEESASKADEME", "EMEKLIYEESASTARIH", "ODEMEYEESASGOSTERGE", "ODEMEYEESASEKGOSTERGE", "KAZANILMISGOSTERGE", "KAZANILMISEKGOSTERGE", "EMEKLIYEESASGOSTERGE", "EMEKLIYEESASEKGOSTERGE", "MALUL", "DAIREKODU" };

        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOADKADRO = "PERPERSONELKADRODERECE";
        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        public enum ENUMAlanlarKadro : int
        {
            /// <summary>
            /// T.C. Kimlik Numarası
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// SIRANO
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// Organizasyon kodu
            /// </summary>
            ORGANIZASYONKOD = 2,
            /// <summary>
            /// Muhasebe Kodu
            /// </summary>
            MUHASEBEKODU = 3,
            /// <summary>
            /// Birim Kodu
            /// </summary>
            BIRIMKODU = 4,
            /// <summary>
            /// Kurumsal Kodu
            /// </summary>
            KURUM = 5,
            /// <summary>
            /// Birim Adı
            /// </summary>
            BIRIMAD = 6,
            /// <summary>
            /// Ünvan
            /// </summary>
            UNVANKOD = 7,
            /// <summary>
            /// Sınıfı
            /// </summary>
            SINIFKOD = 8,
            /// <summary>
            /// Evrak Numarası
            /// </summary>
            EVRAKNO = 9,
            /// <summary>
            /// Yasal Dayanak
            /// </summary>
            YASALDAYANAK = 10,
            /// <summary>
            /// Onay Tarihi
            /// </summary>
            ONAYTARIH = 11,
            /// <summary>
            /// Açıklama
            /// </summary>
            ACIKLAMA = 12,
            /// <summary>
            /// Kadro Derece
            /// </summary>
            KADRODERECE = 13,
            /// <summary>
            /// Kadro Tarihi
            /// </summary>
            KADROTARIH = 14,
            /// <summary>
            /// Ödemeye Esas Derece
            /// </summary>
            ODEMEYEESASDERECE = 15,
            /// <summary>
            /// Ödemeye Esas Kademe
            /// </summary>
            ODEMEYEESASKADEME = 16,
            /// <summary>
            /// Ödemeye Esas Tarih
            /// </summary>
            ODEMEYEESASTARIH = 17,
            /// <summary>
            /// Kazanılmış Hak Derece
            /// </summary>
            KAZANILMISHAKDERECE = 18,
            /// <summary>
            /// Kazanılmış Hak Kademe
            /// </summary>
            KAZANILMISHAKKADEME = 19,
            /// <summary>
            /// Kazanılmış Hak Tarih
            /// </summary>
            KAZANILMISHAKTARIH = 20,
            /// <summary>
            /// Emekliye Esas Derece
            /// </summary>
            EMEKLIYEESASDERECE = 21,
            /// <summary>
            /// Emekliye Esas Kademe
            /// </summary>
            EMEKLIYEESASKADEME = 22,
            /// <summary>
            /// Emekliye Esas Tarih
            /// </summary>
            EMEKLIYEESASTARIH = 23,
            /// <summary>
            /// Ödemeye Esas Gösterge
            /// </summary>
            ODEMEYEESASGOSTERGE = 24,
            /// <summary>
            /// Ödemeye Esas Ek Gösterge
            /// </summary>
            ODEMEYEESASEKGOSTERGE = 25,
            /// <summary>
            /// Kazanılmış Gösterge
            /// </summary>
            KAZANILMISGOSTERGE = 26,
            /// <summary>
            /// Kazanılmış Ek Gösterge
            /// </summary>
            KAZANILMISEKGOSTERGE = 27,
            /// <summary>
            /// Emekliye Esas Gösterge
            /// </summary>
            EMEKLIYEESASGOSTERGE = 28,
            /// <summary>
            /// Emekliye Esas Ek Gösterge
            /// </summary>
            EMEKLIYEESASEKGOSTERGE = 29,

        }
        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLARKADRO = { "TCKIMLIKNO", "SIRANO", "ORGANIZASYONKOD", "MUHASEBEKODU", "BIRIMKODU", "KURUM", "BIRIMAD", "UNVANKOD", "SINIFKOD", "EVRAKNO", "YASALDAYANAK", "ONAYTARIH", "ACIKLAMA", "KADRODERECE", "KADROTARIH", "ODEMEYEESASDERECE", "ODEMEYEESASKADEME", "ODEMEYEESASTARIH", "KAZANILMISHAKDERECE", "KAZANILMISHAKKADEME", "KAZANILMISHAKTARIH", "EMEKLIYEESASDERECE", "EMEKLIYEESASKADEME", "EMEKLIYEESASTARIH", "ODEMEYEESASGOSTERGE", "ODEMEYEESASEKGOSTERGE", "KAZANILMISGOSTERGE", "KAZANILMISEKGOSTERGE", "EMEKLIYEESASGOSTERGE", "EMEKLIYEESASEKGOSTERGE" };


        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOADKURS = "PERPERSONELKURS";
        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        enum ENUMAlanlarKurs : int
        {
            /// <summary>
            /// T.C. Kimlik Numarası
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// Sıra Numarası
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// Eğitim(Kurs)
            /// </summary>
            KURSKODU = 2,
            /// <summary>
            /// Aldığı Yer
            /// </summary>
            ALDIGIYER = 3,
            /// <summary>
            /// Başlama Tarihi
            /// </summary>
            BASLAMATARIHI = 4,
            /// <summary>
            /// Bitirme Tarihi
            /// </summary>
            BITIRMETARIHI = 5,
            /// <summary>
            /// Derece
            /// </summary>
            DERECE = 6,
            /// <summary>
            /// Kurs Adı
            /// </summary>
            KURSADI = 7,
            /// <summary>
            /// Ders Saati
            /// </summary>
            DERSSAATI = 8,
            /// <summary>
            /// Eğitim Yeri
            /// </summary>
            EGITIMYERI = 9,
            /// <summary>
            /// Açıklama
            /// </summary>
            ACIKLAMA = 10,
            /// <summary>
            /// Dönem
            /// </summary>
            DONEM = 11,
            /// <summary>
            /// Yıl
            /// </summary>
            YIL = 12,
            /// <summary>
            /// Dosya Numarası
            /// </summary>
            DOSYANO = 13,
            /// <summary>
            /// Gün
            /// </summary>
            GUN = 14,
            /// <summary>
            /// Katılım Tipi
            /// </summary>
            KATILIMTIPI = 15,
            /// <summary>
            /// Katılım Tipi
            /// </summary>
            KURSNITELIGI = 16,
            /// <summary>
            /// Eğitimin yapıldığı il
            /// </summary>
            YAPILDIGIIL = 17,
            /// <summary>
            /// eğitim saati
            /// </summary>
            EGITIMSAATI = 18,
            /// <summary>
            /// eğitim tür kodu
            /// </summary>
            EGITIMTURKODU = 19,// Eğitim seviyesi 3 olanlar için,
            /// <summary>
            /// eğitim tür ADI
            /// </summary>
            EGITIMTURADI = 19,// Eğitim seviyesi 3 olanlar için,


        }
        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLARKURS = { "TCKIMLIKNO", "SIRANO", "KURSKODU", "ALDIGIYER", "BASLAMATARIHI", "BITIRMETARIHI", "DERECE", "KURSADI", "DERSSAATI", "EGITIMYERI", "ACIKLAMA", "DONEM", "YIL", "DOSYANO", "GUN", "KATILIMTIPI", "KURSNITELIGI", "YAPILDIGIIL", "EGITIMSAATI", "EGITIMTURKODU" };

        /// <summary>
        /// Tablo adı
        /// </summary>
        static string TABLOADEGITIMTURLERI = "PEREGITIMTURLERI";
        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        enum ENUMAlanlarEgitimTurleri : int
        {
            /// <summary>
            /// eğitim kodu
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Eğitimin Atakodu
            /// </summary>
            ATAKOD = 1,
            /// <summary>
            /// Eğitim Adı
            /// </summary>
            AD = 2,
            /// <summary>
            /// Eğitim seviyesi
            /// </summary>
            SEVIYE = 3,

        }
        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLAREGITIMTURLERI = { "KOD", "ATAKOD", "AD", "SEVIYE" };

        static string TABLOADDILSOZLUK = "PERDIL";
        enum ENUMAlanlarDilSozluk : int
        {
            KOD = 0,
            AD = 1,
        }
        static string[] ALANLARDILSOZLUK = { "KOD", "AD" };

        static string TABLOADKURSSOZLUK = "PERKURS";
        enum ENUMAlanlarKursSozluk : int
        {
            KOD = 0,
            AD = 1,
        }
        static string[] ALANLARKURSSOZLUK = { "KOD", "AD" };

        //MAAŞ
        //*****************************************************************
        static string TABLOADSOZLESMELIBORDROTANIM = "PERPERSONELSOZBORDROTANIM";
        public enum ENUMAlanlarSozlesmeliBordro : int
        {
            TCKIMLIKNO = 0,
            YIL = 1,
            AY = 2,
            KUMULATIFVERGIMATRAHI = 3,
        }
        static string[] ALANLARSOZLESMELIBORDROTANIM = { "TCKIMLIKNO", "YIL", "AY", "KUMULATIFVERGIMATRAHI" };

        static string TABLOADIZIN = "PERPERSONELIZIN";
        public enum ENUMAlanlarIzin : int
        {
            TCKIMLIKNO = 0,
            SIRANO = 1,
            TIP = 2,
            ACIKLAMA = 3,
            BASLANGICTARIHI = 4,
            BITISTARIHI = 5,
            TELEFON = 6,
            KULLANILANGUN = 7,
            IZINNOTU = 8,
            ONAY = 9,
            EVREVRAKNO = 10,
            PREVRAKNO = 11,
            EVRAKALAN1 = 12,
            EVRAKNOT1 = 13,
            RAPORTARIHI = 14,
            RAPORVEREN = 15,
            RAPORNUMARA = 16,
            //RAPORTURU = 17,
        }
        static string[] ALANLARIZIN = { "TCKIMLIKNO", "SIRANO", "TIP", "ACIKLAMA", "BASLANGICTARIHI", "BITISTARIHI", "TELEFON", "KULLANILANGUN", "IZINNOTU", "ONAY", "EVREVRAKNO", "PREVRAKNO", "EVRAKALAN1", "EVRAKNOT1", "RAPORTARIHI", "RAPORVEREN", "RAPORNUMARA" };

        static string TABLOADIZINTALEP = "PERPERSONELIZINTALEP";
        public enum ENUMAlanlarIzinTalep : int
        {
            TCKIMLIKNO = 0,
            YIL = 1,
            DONEMID = 2,
            BASLAMATARIHI = 3,
            BITISTARIHI = 4,
            KULLANILACAKGUN = 5,
            TALEPACIKLAMA = 6,
            DURUMID = 7,
            ONAY = 8
        }
        static string[] ALANLARIZINTALEP = { "TCKIMLIKNO", "YIL", "DONEMID", "BASLAMATARIHI", "BITISTARIHI", "KULLANILACAKGUN", "TALEPACIKLAMA", "DURUMID", "ONAY" };

        static string TABLOADPERDONEM = "PERDONEM";
        public enum ENUMAlanlarPerDonem : int
        {
            DONEMID = 0,
            DONEMYIL = 1,
            DONEMBASLANGICTARIHI = 2,
            DONEMBITISTARIHI = 3,
            DONEMACIKLAMA = 4
        }
        static string[] ALANLARPERDONEM = { "DONEMID", "DONEMYIL", "DONEMBASLANGICTARIHI", "DONEMBITISTARIHI", "DONEMACIKLAMA" };

        static string TABLOADIZINDEVIR = "PERPERSONELIZINDEVIR";
        public enum ENUMAlanlarIzinDevir : int
        {
            TCKIMLIKNO = 0,
            YIL = 1,
            DEVREDEN = 2,
            YILICI = 3,
        }
        static string[] ALANLARIZINDEVIR = { "TCKIMLIKNO", "YIL", "DEVREDEN", "YILICI" };

        static string TABLOADTARIHCE = "PERPERSONELTARIHCE";
        public enum ENUMAlanlarTarihce : int
        {
            PERSONEL = 0,
            KULLANICI = 1,
            TARIH = 2,
            DEGISIKLIK = 3,
            KULLANICIAD = 4
        }
        static string[] ALANLARTARIHCE = { "PERSONEL", "KULLANICI", "TARIH", "DEGISIKLIK", "KULLANICIAD" };


        static string TABLOADDAIRE = "PERPERSONELDAIRE";
        public enum ENUMAlanlarDaire : int
        {
            KOD = 0,
            ANAKOD = 1,
            BIRIMKODU = 2,
            ADI = 3,
            DURUM = 4
        }
        static string[] ALANLARDAIRE = { "KOD", "ANAKOD", "BIRIMKODU", "ADI", "DURUM" };

        //*****************************************************************

        public IDataReader Listele(Personel p, string disSqleBagla)
        {
            string sql;

            string kosul = string.Empty;
            if (!string.IsNullOrWhiteSpace(disSqleBagla))
            {
                sql = "SELECT 1 FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADSOZLESMELIBORDROTANIM + " ON " + TABLOAD + ".TCKIMLIKNO = " + TABLOADSOZLESMELIBORDROTANIM + ".TCKIMLIKNO LEFT OUTER JOIN PERUNVAN ON " + TABLOAD + ".UNVANKODU = PERUNVAN.KOD ";
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", disSqleBagla));
            }
            else
            {
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) +
                     "," + VeriErisim.TumAlanlar(TABLOADSOZLESMELIBORDROTANIM, ALANLARSOZLESMELIBORDROTANIM) + ",PERUNVAN.AD,PERUNVAN.ADKISA FROM " + TABLOAD + " LEFT OUTER JOIN " + TABLOADSOZLESMELIBORDROTANIM + " ON " + TABLOAD + ".TCKIMLIKNO = " + TABLOADSOZLESMELIBORDROTANIM + ".TCKIMLIKNO LEFT OUTER JOIN PERUNVAN ON " + TABLOAD + ".UNVANKODU = PERUNVAN.KOD ";
            }

            if (p.tcList == null)
            {
                if (p.sicilNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SICILNO], "=", p.sicilNo));

                if (p.ad != "")
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", p.ad + "%"));
                }

                if (p.soyad != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOYAD], "LIKE", p.soyad + "%"));

                if (!string.IsNullOrWhiteSpace(p.adSoyadTc))
                {
                    string[] aranan = p.adSoyadTc.Split(' ');

                    foreach (string ara in aranan)
                    {
                        if (string.IsNullOrEmpty(ara.Trim()))
                            continue;
                        string icKosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], " LIKE ", ara.Replace(".", "").Trim() + "%");
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + ara + "%"));
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SOYAD], " LIKE ", "%" + ara + "%"));
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", $"{TABLOAD}.AD + ' ' + {TABLOAD}.SOYAD", " LIKE ", "%" + ara + "%"));
                        icKosul += " collate SQL_Latin1_General_CP1_CI_AS ";

                        kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
                    }
                }

                if (p.babaAd != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BABAAD], "LIKE", p.babaAd + "%"));

                if (p.anaAd != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAAD], "LIKE", p.anaAd + "%"));

                if (p.dogumTarihi.ToString() != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOGUMTARIHI], "=", p.dogumTarihi));

                if (p.cinsiyet > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CINSIYET], "=", p.cinsiyet));

                if (p.medeniHali > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MEDENIHALI], "=", p.medeniHali));

                if (p.kanGrubu != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KANGRUBU], "=", p.kanGrubu));

                if (p.askerlikKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASKERLIKKODU], "=", p.askerlikKodu));

                if (p.ogrenimKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OGRENIMKODU], "=", p.ogrenimKodu));

                if (p.esDurumuKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ESDURUMUKODU], "=", p.esDurumuKodu));

                if (p.durum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", p.durum));

                if (p.bankaKodu != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BANKAKODU], "=", p.bankaKodu));

                if (p.bankaSubeKodu != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BANKASUBEKODU], "=", p.bankaSubeKodu));

                if (p.evrakBirimKod != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", p.evrakBirimKod));

                if (p.teskilatKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TESKILATKODU], "=", p.teskilatKodu));

                if (p.istihdamSekli > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTIHDAMSEKLI], "=", p.istihdamSekli));

                if (p.istihdamSekliListe != null && p.istihdamSekliListe.Length > 0)
                {
                    string altKosul = string.Empty;
                    foreach (var istihdamSekli in p.istihdamSekliListe)
                    {
                        altKosul = VeriErisim.KosulEkle(altKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISTIHDAMSEKLI], "=", istihdamSekli));
                    }

                    if (!string.IsNullOrEmpty(altKosul))
                        kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + altKosul + ")");
                    //kosul += " AND (" + altKosul + ")";
                }

                if (p.unvanKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVANKODU], "=", p.unvanKodu));

                if (p.kadroDerece > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KADRODERECE], "=", p.kadroDerece));

                if (p.tcKimlikNo != "")
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));
                }

                //if (p.muhasebeKodu > -1)
                //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));

                //if (kosul != "")
                //    kosul += " AND ";
                //else
                //    kosul = " ";
                //kosul = VeriErisim.KosulEkle(kosul, " (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
                //kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", 0) + ")");

                if (p.muhasebeKodu > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
                    if (!string.IsNullOrWhiteSpace(p.birimKodu))
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
                    }
                }
                else if (p.organizasyonKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

                if (p.sinifKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SINIFKODU], "=", p.sinifKodu));

                if (p.ustBirimKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.USTBIRIMKODU], "=", p.ustBirimKodu));
            }
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " IN (" + VirgulleBirlestir(p.tcList) + ")");
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (p.unvanaGoreSirala)
                sql += " ORDER BY " + "PERUNVAN.SIRANO," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD];
            else
                sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD];

            IDataReader r = null;
            if (kosul != "")
                r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        private string VirgulleBirlestir(TNSCollection liste)
        {
            string retVal = "";
            foreach (string item in liste)
            {
                if (retVal != "") retVal += ",";
                retVal += VeriErisim.DegerYap(item);
            }
            return retVal;
        }

        public IDataReader ListelePersonelRapor(Personel p)
        {
            string sql = "";
            string kosul = "";
            sql = @"SELECT p.TCKIMLIKNO, p.AD, p.SOYAD, pu.AD, ps.KISAAD, p.CINSIYET, p.DOGUMTARIHI, ppo1.OKULADI, po1.TIP, ppo1.BITIRMETARIHI, pd.AD, ppd.PUAN, 
ppd.DUZEY, ppd.DONEMYIL, ppd.DONEMAY, ppd.BELGETUR, ppo1.GECERLI,ppo1.FAKULTEBOLUM 
FROM PERPERSONEL p 
LEFT OUTER JOIN PERUNVAN pu ON p.UNVANKODU = pu.KOD 
LEFT OUTER JOIN PERSINIF ps ON p.SINIFKODU = ps.KOD 
LEFT OUTER JOIN PERPERSONELOGRENIM ppo1 ON p.TCKIMLIKNO = ppo1.TCKIMLIKNO 
LEFT OUTER JOIN PERPERSONELDIL ppd ON p.TCKIMLIKNO = ppd.TCKIMLIKNO and ppd.BORDROYAESAS=1 
LEFT OUTER JOIN PERDIL pd ON ppd.DILKODU = pd.KOD 
LEFT OUTER JOIN PEROGRENIM po1 ON ppo1.OGRENIMKOD = po1.KOD ";


            if (p.muhasebeKodu > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
                if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) == 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", ""));
                    kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", "0") + ")");
                }
                else if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) != -1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
                }
            }
            else if (p.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + "p." + ALANLAR[(int)ENUMAlanlar.AD] + ", p." + ALANLAR[(int)ENUMAlanlar.SOYAD] + ",po1.TIP, ppo1.OKULADI";
            IDataReader r = null;
            if (kosul != "")
                r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader BirimBazliPersonelSonMemuriyetListele(Personel p)
        {
            string sql = "";

            //+' ' + p.SOYAD,p.BIRIMKODU,p.MUHASEBEKODU
            sql = @"select distinct p.TCKIMLIKNO, p.AD, p.SOYAD, u.AD 
                from PERPERSONELMEMURIYET pm
                left outer join PERPERSONEL p on p.TCKIMLIKNO = pm.TCKIMLIKNO
                left outer join PERUNVAN U ON p.UNVANKODU = U.KOD
                where pm.MUHASEBEKODU = " + p.muhasebeKodu + " and pm.BIRIMKODU = '" + p.birimKodu + "' ";
            sql += " and p.MUHASEBEKODU =" + p.muhasebeKodu + " and (p.BIRIMKODU !='" + p.birimKodu + "' and p.BIRIMKODU <> '' and p.BIRIMKODU <> '0') ";
            sql += " and pm.AYRILMATARIHI is null ";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleBelgeler(PersonelBelgeler p)
        {
            string sql = "";
            string kosul = "";

            if (p.belgeSiraNo > 0)
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBELGELER, ALANLARBELGELER) + " FROM " + TABLOADBELGELER;
            else
                sql = "SELECT TCKIMLIKNO, TIP, KAYITSIRANO, BELGESIRANO, BELGEADI FROM " + TABLOADBELGELER;

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.TIP], "=", p.tip));

            if (!string.IsNullOrEmpty(p.kayitSiraNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.KAYITSIRANO], "=", p.kayitSiraNo));

            if (p.belgeSiraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGESIRANO], "=", p.belgeSiraNo));

            if (!string.IsNullOrEmpty(p.belgeAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGEADI], "=", p.belgeAdi));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARBELGELER[(int)ENUMAlanlarBelgeler.KAYITSIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleUnvanKadro(Personel p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT PERUNVAN.AD, CINSIYET, COUNT(CINSIYET) FROM " + TABLOAD + " LEFT OUTER JOIN PERUNVAN ON " + TABLOAD + ".UNVANKODU = PERUNVAN.KOD ";

            if (p.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "and", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));
            if (!string.IsNullOrEmpty(p.birimKodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
            //kosul = VeriErisim.KosulEkle(sql, "and", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));
            if (p.unvanKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVANKODU], "=", p.unvanKodu));
            //kosul = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVANKODU], "=", p.unvanKodu));
            if (p.cinsiyet > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CINSIYET], "=", p.cinsiyet));

            // kosul = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CINSIYET], "=", p.cinsiyet));

            if (kosul != "")
                sql += " WHERE " + kosul;
            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + ", PERUNVAN.AD, " + ALANLAR[(int)ENUMAlanlar.CINSIYET];
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.UNVANKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleAile(string personelTCKimlikNo, string tcKimlikNo)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADAILE, ALANLARAILE) + " FROM " + TABLOADAILE;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.PERSONELTCKIMLIKNO], "=", personelTCKimlikNo));

            if (!string.IsNullOrEmpty(tcKimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.TCKIMLIKNO], "=", tcKimlikNo));

            sql += " ORDER BY " + ALANLARAILE[(int)ENUMAlanlarAile.YAKINLIKDERECESI];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleMalBildirimi(PersonelMalBildirimi p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI) + " FROM " + TABLOADMALBILDIRIMI;

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.sirano > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.SIRANO], "=", p.sirano));

            if (!p.tarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TARIH], "=", p.tarih));

            if (!string.IsNullOrEmpty(p.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.ACIKLAMA], "LIKE", "%" + p.aciklama + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleCezaOdul(PersonelCezaOdul p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADCEZAODUL, ALANLARCEZAODUL) + " FROM " + TABLOADCEZAODUL;

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.sirano > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.SIRANO], "=", p.sirano));

            if (!p.tarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.TARIH], "=", p.tarih));

            if (p.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.TIP], "=", p.tip));

            if (p.cezaodultip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.CEZAODULTIP], "=", p.cezaodultip));

            if (!string.IsNullOrEmpty(p.verilisNedeni))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.VERILISNEDENI], "LIKE", "%" + p.verilisNedeni + "%"));

            if (!string.IsNullOrEmpty(p.belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.BELGENO], "LIKE", "%" + p.belgeNo + "%"));

            if (!string.IsNullOrEmpty(p.dayanak))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.DAYANAK], "LIKE", "%" + p.dayanak + "%"));

            if (!p.belgeTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.BELGETARIHI], "=", p.belgeTarihi));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleOgrenim(PersonelOgrenim p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADOGRENIM, ALANLAROGRENIM) + " FROM " + TABLOADOGRENIM;

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.SIRANO], "=", p.siraNo));

            if (p.ogrenimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.OGRENIMKOD], "=", p.ogrenimKod));

            if (!string.IsNullOrEmpty(p.okulAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.OKULADI], "LIKE", "%" + p.okulAdi + "%"));

            if (!string.IsNullOrEmpty(p.fakulteBolum))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.FAKULTEBOLUM], "LIKE", "%" + p.fakulteBolum + "%"));

            if (p.belgeTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BELGETUR], "=", p.belgeTur));

            if (!p.belgeTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BELGETARIHI], "=", p.belgeTarihi));

            if (!string.IsNullOrEmpty(p.belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BELGENO], "=", p.belgeNo));

            if (!p.baslangicTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BASLANGICTARIHI], "=", p.baslangicTarihi));

            if (!p.bitirmeTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BITIRMETARIHI], "=", p.bitirmeTarihi));

            if (p.derece > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.DERECE], "=", p.derece));

            if (p.gecerli > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.GECERLI], "=", p.gecerli));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BASLANGICTARIHI];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDil(PersonelDil p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDIL, ALANLARDIL) + "," + VeriErisim.TumAlanlar(TABLOADDILSOZLUK, ALANLARDILSOZLUK);
            sql += " FROM " + TABLOADDIL + " LEFT OUTER JOIN " + TABLOADDILSOZLUK + " ON PERPERSONELDIL.DILKODU = PERDIL.KOD";

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.SIRANO], "=", p.siraNo));

            if (p.dilKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.DILKODU], "=", p.dilKod));

            if (!string.IsNullOrEmpty(p.duzey))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.DUZEY], "=", p.duzey));

            if (p.puan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.PUAN], "=", p.puan));

            if (p.donemAy > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.DONEMAY], "=", p.donemAy));

            if (p.donemYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.DONEMYIL], "=", p.donemYil));

            if (p.belgeTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.BELGETUR], "=", p.belgeTur));

            if (!p.belgeTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.BELGETARIHI], "=", p.belgeTarihi));

            if (!string.IsNullOrEmpty(p.belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.BELGENO], "LIKE", "%" + p.belgeNo + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARDIL[(int)ENUMAlanlarDil.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleMemuriyet(PersonelMemuriyet p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADMEMURIYET, ALANLARMEMURIYET) + ",PERNEDENBASLAMA.AD, PERNEDENAYRILMA.AD,PERPERSONELDAIRE.ADI ";
            sql += " FROM PERNEDEN PERNEDENAYRILMA ";
            sql += "RIGHT OUTER JOIN PERPERSONELMEMURIYET ON PERNEDENAYRILMA.KOD = PERPERSONELMEMURIYET.AYRILMANEDENI  ";
            sql += "LEFT OUTER JOIN PERNEDEN PERNEDENBASLAMA ON PERNEDENBASLAMA.KOD=PERPERSONELMEMURIYET.BASLAMANEDENI ";
            sql += " LEFT OUTER JOIN PERPERSONELDAIRE ON " + TABLOADMEMURIYET + "." + ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.DAIREKODU] + " = PERPERSONELDAIRE.KOD";

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMEMURIYET, ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMEMURIYET, ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.SIRANO], "=", p.siraNo));

            if (!string.IsNullOrEmpty(p.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMEMURIYET, ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ACIKLAMA], "LIKE", "%" + p.aciklama + "%"));

            //TODO: Diğer alanlar da kontrol edilip sorguya eklenecek

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.BASLAMATARIHI] + " ASC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleKadro(PersonelKadro p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKADRO, ALANLARKADRO) + ",PERUNVAN.AD FROM " + TABLOADKADRO + " LEFT OUTER JOIN PERUNVAN ON " + TABLOADKADRO + ".UNVANKOD = PERUNVAN.KOD ";

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKADRO, ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKADRO, ALANLARKADRO[(int)ENUMAlanlarKadro.SIRANO], "=", p.siraNo));

            if (!string.IsNullOrEmpty(p.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKADRO, ALANLARKADRO[(int)ENUMAlanlarKadro.ACIKLAMA], "LIKE", "%" + p.aciklama + "%"));

            //TODO: Diğer alanlar da kontrol edilip sorguya eklenecek, sıralama şartı güncellenecek

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO] + "," +
                                  ALANLARKADRO[(int)ENUMAlanlarKadro.KADROTARIH] + " DESC, " +
                                  ALANLARKADRO[(int)ENUMAlanlarKadro.KADRODERECE] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleKurs(PersonelKurs p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKURS, ALANLARKURS) + "," + VeriErisim.TumAlanlar(TABLOADKURSSOZLUK, ALANLARKURSSOZLUK) + " from " + TABLOADKURS;
            sql += @" left outer join PERKURS on PERKURS.KOD=PERPERSONELKURS.KURSKODU
                    left outer join PEREGITIMTURLERI on PEREGITIMTURLERI.KOD=PERPERSONELKURS.EGITIMTURKODU ";

            //kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.KURSKODU] + "=" + TABLOADKURSSOZLUK + "." + ALANLARKURSSOZLUK[(int)ENUMAlanlarKursSozluk.KOD]);

            if (!string.IsNullOrEmpty(p.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.ACIKLAMA], "LIKE", "%" + p.aciklama + "%"));

            if (!string.IsNullOrEmpty(p.aldigiYer))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.ALDIGIYER], "LIKE", "%" + p.aldigiYer + "%"));

            if (!string.IsNullOrEmpty(p.derece))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.DERECE], "=", p.derece));

            if (p.dersSaati > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.DERSSAATI], "=", p.dersSaati));

            if (p.donem > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.DONEM], "=", p.donem));

            if (!string.IsNullOrEmpty(p.dosyaNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.DOSYANO], "=", p.dosyaNo));

            if (!string.IsNullOrEmpty(p.egitimYeri))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMYERI], "LIKE", "%" + p.egitimYeri + "%"));

            if (p.gun > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.GUN], "=", p.gun));

            if (!string.IsNullOrEmpty(p.kursAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.KURSADI], "LIKE", "%" + p.kursAdi + "%"));

            if (p.kursKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.KURSKODU], "=", p.kursKod));

            if (p.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.SIRANO], "=", p.siraNo));

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (p.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.YIL], "=", p.yil));

            if (p.katilimTipi > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.KATILIMTIPI], "=", p.katilimTipi));


            if (!string.IsNullOrEmpty(p.kursAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.KURSNITELIGI], "LIKE", "%" + p.kursNiteligi + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARKURS[(int)ENUMAlanlarKurs.BASLAMATARIHI];

            return veriErisim.DataReaderDondur(sql);
        }


        public IDataReader ListeleEgitimTurleri(PersonelEgitimTurleri pk)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT "
                + TABLOADEGITIMTURLERI + "." + ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.KOD] + ", "
                + TABLOADEGITIMTURLERI + "." + ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.ATAKOD] + ", "
                + TABLOADEGITIMTURLERI + "." + ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.AD] + ", "
                + TABLOADEGITIMTURLERI + "." + ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.SEVIYE]
                + " FROM " + TABLOADEGITIMTURLERI;
            if (pk.ataKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADEGITIMTURLERI, ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.ATAKOD], "=", pk.ataKod));
            }
            if (pk.seviye > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADEGITIMTURLERI, ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.SEVIYE], "=", pk.seviye));
            }
            if (pk.kod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADEGITIMTURLERI, ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.KOD], "=", pk.kod));
            }
            sql += " WHERE " + kosul;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DonemlikEgitimFaaliyetRaporu(Personel p, PersonelKurs pk)
        {
            string sql = "";
            string kosul = "";//, k.ACIKLAMA, k.EGITIMYERI, k.KURSADI kursAdi
            sql = @"select k.ALDIGIYER,k.EGITIMTURKODU, K.KURSADI,k.BASLAMATARIHI, k.BITIRMETARIHI,k.GUN,k.DERSSAATI, 
                    u.AD unvan, count(*) sayı,o.AD, K.EGITIMYERI
                    from PERPERSONELKURS k
                    inner join PERPERSONEL p on p.TCKIMLIKNO = k.TCKIMLIKNO
                    left outer join Kullanici.dbo.ORGCOGRAFYA o on o.KOD=k.YAPILDIGIIL
                    --left outer join Kullanici.dbo.ORGBIRIM b ON p.BIRIMKODU = b.MUHASEBEBIRIM and p.MUHASEBEKODU = b.MUHASEBEKODU --??
                    left outer join PERUNVAN u on u.KOD = p.UNVANKODU where k.KATILIMTIPI = 1 ";
            //where 
            // (k.ALDIGIYER like '%sgb%' or k.ALDIGIYER like '%strateji%')
            //--and k.YIL = '2017' --and k.DONEM = 4
            //and p.BIRIMKODU='923' and p.MUHASEBEKODU='0007'";

            if (p.muhasebeKodu > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
                if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) == 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", ""));
                    kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", "0") + ")");
                }
                else if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) != -1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
                }
            }
            else if (p.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

            if (!string.IsNullOrEmpty(pk.kursAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("k", ALANLARKURS[(int)ENUMAlanlarKurs.KURSADI], "LIKE", pk.kursAdi + "%"));
            if (pk.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("k", ALANLARKURS[(int)ENUMAlanlarKurs.YIL], "=", pk.yil));
            if (pk.donem > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("k", ALANLARKURS[(int)ENUMAlanlarKurs.DONEM], "=", pk.donem));

            if (!string.IsNullOrEmpty(pk.aldigiYer))
            {
                string[] yerler = pk.aldigiYer.Split(new Char[] { ',', ';', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string yerKosulu = "(";
                for (int i = 0; i < yerler.Length; i++)
                {
                    yerKosulu += " k.ALDIGIYER like '%" + yerler[i] + "%' ";
                    if (i != yerler.Length - 1)
                        yerKosulu += " or ";
                }

                yerKosulu += ")";

                kosul = VeriErisim.KosulEkle(kosul, "AND", yerKosulu);
            }
            if (!string.IsNullOrEmpty(pk.egitimYeri))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("k", ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMYERI], "LIKE", pk.egitimYeri + "%"));

            if (kosul != "")
                sql += " AND " + kosul;

            sql += @" group  by k.ALDIGIYER,k.EGITIMTURKODU, K.KURSADI,k.BASLAMATARIHI, k.BITIRMETARIHI,k.GUN,k.DERSSAATI, 
u.AD,o.AD, K.EGITIMYERI";// k.EGITIMYERI, k.ACIKLAMA ";,k.KURSADI

            sql += @" order  by k.ALDIGIYER,k.EGITIMTURKODU, K.KURSADI,k.BASLAMATARIHI, k.BITIRMETARIHI,k.GUN,k.DERSSAATI, 
u.AD,o.AD, K.EGITIMYERI";//, k.EGITIMYERI, k.ACIKLAMA ";,k.KURSADI

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleEgitimRapor(Personel p, PersonelKurs pk)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + ", "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ", "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD] + ", "
                + "PERUNVAN.AD, "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.KURSADI] + ", "
                + "PERKURS.AD, "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.BASLAMATARIHI] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.BITIRMETARIHI] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.DERSSAATI] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.ALDIGIYER] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMYERI] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.ACIKLAMA] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.DONEM] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.YIL] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.DOSYANO] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMTURKODU] + ", "
                + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.KATILIMTIPI]
                + " FROM " + TABLOAD
                + " INNER JOIN " + TABLOADKURS + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO]
                + " LEFT OUTER JOIN PERUNVAN ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = PERUNVAN.KOD"
                + " LEFT OUTER JOIN PERKURS ON " + TABLOADKURS + "." + ALANLARKURS[(int)ENUMAlanlarKurs.KURSKODU] + " = PERKURS.KOD";

            if (p.muhasebeKodu > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
                if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) == 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", ""));
                    kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", "0") + ")");
                }
                else if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) != -1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
                }
            }
            else if (p.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

            if (!string.IsNullOrEmpty(pk.kursAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.KURSADI], "LIKE", pk.kursAdi + "%"));
            if (pk.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.YIL], "=", pk.yil));
            if (pk.donem > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.DONEM], "=", pk.donem));
            if (!string.IsNullOrEmpty(pk.aldigiYer))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.ALDIGIYER], "LIKE", pk.aldigiYer + "%"));
            if (!string.IsNullOrEmpty(pk.egitimYeri))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMYERI], "LIKE", pk.egitimYeri + "%"));

            //EZGİ
            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + "" + TABLOADKURS + ".BASLAMATARIHI, PERKURS.AD, PERUNVAN.KOD, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + " " + VeriErisim.IslecBul("+") + " " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD];
            IDataReader r = null;
            if (kosul != "")
                r = veriErisim.DataReaderDondur(sql);

            return r;
        }



        public IDataReader ListeleIzinRapor(Personel p, PersonelIzin pk)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + ", "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ", "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD] + ", "
                + "PERUNVAN.AD, "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SICILNO] + ", "
                + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI] + ", "
                + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI] + ", "
                + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ", "
                + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.TIP] + ", "
                + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.ACIKLAMA]
                + " FROM " + TABLOAD
                + " INNER JOIN " + TABLOADIZIN + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO]
                + " LEFT OUTER JOIN PERUNVAN ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = PERUNVAN.KOD";

            if (p.muhasebeKodu > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
                if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) == 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", ""));
                    kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", "0") + ")");
                }
                else if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) != -1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
                }
            }
            else if (p.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));

            if (!string.IsNullOrEmpty(pk.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", pk.tcKimlikNo));
            if (pk.baslangicTarihi.Yil > 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", pk.baslangicTarihi));
            if (pk.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "=", pk.tip));
            if (!String.IsNullOrEmpty(pk.donem))
            {
                if (pk.donem == "I. 6 Aylık") //1.6ay
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + pk.baslangicTarihi.Yil)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", new TNSDateTime("30.06." + pk.baslangicTarihi.Yil)));
                }
                else if (pk.donem == "II. 6 Aylık") //2.6ay
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.07." + pk.baslangicTarihi.Yil)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", new TNSDateTime("31.12." + pk.baslangicTarihi.Yil)));
                }

            }
            else //Dönem seçilmemişse, seçili yıla ait kayıtların gelmesini sağlayan kod
            {
                if (pk.baslangicTarihi.Yil > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + pk.baslangicTarihi.Yil)));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], "<=", new TNSDateTime("31.12." + pk.baslangicTarihi.Yil)));
                }

            }

            if (kosul != "")
                sql += " WHERE " + kosul;
            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + " , " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD] + " , " + TABLOADIZIN + ".BASLANGICTARIHI";

            IDataReader r = null;
            if (kosul != "")
                r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader ListeleIzinDurum(PersonelIzinDurum pk)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT "
                + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.TIP] + ", "
                + "PERIZINTUR.AD" + ", "
                + "PERIZINTUR.SURE" + ", "
                + "SUM(" + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ")"
                + " FROM " + TABLOADIZIN
                + " INNER JOIN PERIZINTUR ON " + ALANLARIZIN[(int)ENUMAlanlarIzin.TIP] + " = PERIZINTUR.KOD ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", pk.tcKimlikNo));
            if (pk.listelemeYili > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", pk.listelemeYili.ToString("d4") + "-01-01 00:00:00"));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<", (pk.listelemeYili + 1).ToString("d4") + "-01-01 00:00:00"));
            }

            sql += " WHERE " + kosul;
            sql += " GROUP BY " + TABLOADIZIN + ".TIP" + ", PERIZINTUR.AD" + ", PERIZINTUR.SURE";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleAdres(string tcKimlikNo)
        {
            string sql = "SELECT ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ADRES] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEBLIGATADRES] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MULKDURUM] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TASINMATARIHI] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISTELEFON] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EVTELEFON] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.CEPTELEFON] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FAKS] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EPOSTA] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.URL] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MOBILIMZATELEFON] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACILKISI] + ",";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACILKISINUMARA];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleIzin(PersonelIzin p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIZIN, ALANLARIZIN) + " FROM " + TABLOADIZIN;

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
            if (p.sirano > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));
            if (!p.baslangicTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", p.baslangicTarihi));
            if (!p.bitisTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", p.bitisTarihi));
            if (!string.IsNullOrEmpty(p.prEvrakNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.PREVRAKNO], "=", p.prEvrakNo));
            if (!string.IsNullOrEmpty(p.evrEvrakNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.EVREVRAKNO], "=", p.evrEvrakNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleIzinDevir(PersonelIzinDevir p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIZINDEVIR, ALANLARIZINDEVIR) + ",PERPERSONEL.AD, PERPERSONEL.SOYAD, PERPERSONEL.SICILNO FROM ";
            sql += "PERPERSONEL LEFT OUTER JOIN PERPERSONELIZINDEVIR ON PERPERSONEL.TCKIMLIKNO = PERPERSONELIZINDEVIR.TCKIMLIKNO";

            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.TCKIMLIKNO], "=", p.tcKimlikNo));
            if (p.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL], "=", p.yil));

            if (kosul != "")
                sql += " WHERE " + kosul;
            sql += " ORDER BY " + ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL] + " DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleTarihce(PersonelTarihce p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE) + " FROM " + TABLOADTARIHCE;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.PERSONEL], "=", p.personel));
            sql += " WHERE " + kosul;
            sql += " ORDER BY " + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TARIH] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleMemuriyetTarihleri(string tcKimlikNo)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.MEMURBASLAMATARIHI] + "," + ALANLAR[(int)ENUMAlanlar.SONBIRIMBASLAMATARIHI] + "," + ALANLAR[(int)ENUMAlanlar.SONGOREVBASLAMATARIHI] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public int ListeleKullanilanIzin(string tcKimlikNo, int yil, TNSDateTime baslangicTarih, int izinTip)
        {
            string sql = "";
            sql = "SELECT SUM(" + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ") FROM " + TABLOADIZIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", new TNSDateTime("31.12." + yil)));

            if (baslangicTarih != null)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], "<", baslangicTarih));

            //if (izinSiraNo > 0)//Verilen sirano ile kayıtlı izin hariç kullanılan izin süresi hesaplamak için. Önce den kayıt edilen izin düzeltilerek kayot edilemek istendiğinde süre kontrolü yapmak için
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "<>", izinSiraNo));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "=", izinTip));//1->yıllık izin
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.BELIRSIZ));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.MAZERET));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.REFAKATLI));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.SIHHI));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.UCRETLI));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "<>", (int)ENUMIZINTIP.UCRETSIZ));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc Kaydet(Personel p)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.sicilNo) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.soyad) + ",";
            sql += VeriErisim.DegerYap(p.babaAd) + ",";
            sql += VeriErisim.DegerYap(p.anaAd) + ",";
            sql += VeriErisim.DegerYap(p.dogumTarihi) + ",";
            sql += VeriErisim.DegerYap(p.cinsiyet) + ",";
            sql += VeriErisim.DegerYap(p.medeniHali) + ",";
            sql += VeriErisim.DegerYap(p.kanGrubu) + ",";
            sql += VeriErisim.DegerYap(p.isTelefon) + ",";
            sql += VeriErisim.DegerYap(p.cepTelefon) + ",";
            sql += VeriErisim.DegerYap(p.faks) + ",";
            sql += VeriErisim.DegerYap(p.eposta) + ",";
            sql += VeriErisim.DegerYap(p.askerlikKodu) + ",";
            sql += VeriErisim.DegerYap(p.ogrenimKodu) + ",";
            sql += VeriErisim.DegerYap(p.esDurumuKodu) + ",";
            sql += VeriErisim.DegerYap(p.saglikKarneNo) + ",";
            sql += VeriErisim.DegerYap(p.durum) + ",";
            sql += VeriErisim.DegerYap(p.emekliSicilNo) + ",";
            sql += VeriErisim.DegerYap(p.guncellenmeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.guncelleyenKisi) + ",";
            sql += VeriErisim.DegerYap(p.bankaKodu) + ",";
            sql += VeriErisim.DegerYap(p.bankaSubeKodu) + ",";
            sql += VeriErisim.DegerYap(p.bankaHesapNo) + ",";
            sql += VeriErisim.DegerYap(p.teskilatKodu) + ",";
            sql += VeriErisim.DegerYap(p.unvanKodu) + ",";
            sql += VeriErisim.DegerYap(p.muhasebeKodu) + ",";
            sql += VeriErisim.DegerYap(p.birimKodu) + ",";
            sql += VeriErisim.DegerYap(p.memurBaslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.iseBaslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.istenCikisTarihi) + ",";
            sql += VeriErisim.DegerYap(p.sinifKodu) + ",";
            sql += VeriErisim.DegerYap(p.kidemAy) + ",";
            sql += VeriErisim.DegerYap(p.kidemYil) + ",";
            sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
            sql += VeriErisim.DegerYap(p.kadroTarih) + ",";
            sql += VeriErisim.DegerYap(p.odemeEsasDerece) + ",";
            sql += VeriErisim.DegerYap(p.odemeEsasKademe) + ",";
            sql += VeriErisim.DegerYap(p.odemeEsasGosterge) + ",";
            sql += VeriErisim.DegerYap(p.odemeEsasTarih) + ",";
            sql += VeriErisim.DegerYap(p.emekliEsasDerece) + ",";
            sql += VeriErisim.DegerYap(p.emekliEsasKademe) + ",";
            sql += VeriErisim.DegerYap(p.emekliEsasGosterge) + ",";
            sql += VeriErisim.DegerYap(p.emekliEsasTarih) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakDerece) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakKademe) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakTarih) + ",";
            sql += VeriErisim.DegerYap(p.istihdamSekli) + ",";
            sql += VeriErisim.DegerYap(p.adres) + ",";
            sql += VeriErisim.DegerYap(p.evTelefon) + ",";
            sql += VeriErisim.DegerYap(p.vergiDairesi) + ",";
            sql += VeriErisim.DegerYap(p.ustBirimKodu) + ",";
            sql += VeriErisim.DegerYap(p.evrakBirimKod) + ",";
            sql += VeriErisim.DegerYap(p.odaKodu) + ",";
            sql += VeriErisim.DegerYap(p.ilkSoyad) + ",";
            sql += VeriErisim.DegerYap(p.dogumYeri) + ",";
            sql += VeriErisim.DegerYap(p.nufusIlKod) + ",";
            sql += VeriErisim.DegerYap(p.nufusIlceKod) + ",";
            sql += VeriErisim.DegerYap(p.cuzdanSeriNo) + ",";
            sql += VeriErisim.DegerYap(p.cuzdanKayitNo) + ",";
            sql += VeriErisim.DegerYap(p.nufusMahalleKoy) + ",";
            sql += VeriErisim.DegerYap(p.nufusCiltNo) + ",";
            sql += VeriErisim.DegerYap(p.nufusSiraNo) + ",";
            sql += VeriErisim.DegerYap(p.nufusAileSiraNo) + ",";
            sql += VeriErisim.DegerYap(p.cuzdanVerilisNedeni) + ",";
            sql += VeriErisim.DegerYap(p.cuzdanVerildigiYer) + ",";
            sql += VeriErisim.DegerYap(p.cuzdanVerilisTarihi) + ",";
            sql += VeriErisim.DegerYap(p.bakanlikKartNo) + ",";
            sql += VeriErisim.DegerYap(p.sskSicilNo) + ",";
            sql += VeriErisim.DegerYap(p.bagkurSicilNo) + ",";
            sql += VeriErisim.DegerYap(p.surucuBelgeNo) + ",";
            sql += VeriErisim.DegerYap(p.surucuBelgeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.yasTashihTarihi) + ",";
            sql += VeriErisim.DegerYap(p.organizasyonKodu) + ",";
            sql += VeriErisim.DegerYap(p.kurum.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(p.url) + ",";
            sql += VeriErisim.DegerYap(p.mobilImzaTelNo) + ",";
            sql += VeriErisim.DegerYap(p.tecilTarihi) + ",";
            sql += VeriErisim.DegerYap(p.askerlikMuafNeden) + ",";
            sql += VeriErisim.DegerYap(p.askerlikBaslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.terhisTarihi) + ",";
            sql += VeriErisim.DegerYap(p.askerlikCuzdanNo) + ",";
            sql += VeriErisim.DegerYap(p.mulkDurumu) + ",";
            sql += VeriErisim.DegerYap(p.tasinmaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.oturmaSuresi) + ",";
            sql += VeriErisim.DegerYap(p.sonGoreveBaslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.sonBirimeBaslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.kadroOrganizasyonKodu) + ",";
            sql += VeriErisim.DegerYap(p.kadroMuhasebeKodu) + ",";
            sql += VeriErisim.DegerYap(p.kadroBirimKodu) + ",";
            sql += VeriErisim.DegerYap(p.kadroKurum) + ",";
            sql += VeriErisim.DegerYap(p.tebligatAdresi) + ",";
            sql += VeriErisim.DegerYap(p.acilKisi) + ",";
            sql += VeriErisim.DegerYap(p.acilKisiNumara) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakGosterge) + ",";
            sql += VeriErisim.DegerYap(p.odemeEsasEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.emekliEsasEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakEkGosterge) + ",";
            sql += VeriErisim.DegerYap(1) + ",";
            sql += VeriErisim.DegerYap(0) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetBelgeler(PersonelBelgeler p)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOADBELGELER + " (" + VeriErisim.TumAlanlar("", ALANLARBELGELER) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.tip) + ",";
            sql += VeriErisim.DegerYap(p.kayitSiraNo) + ",";
            sql += VeriErisim.DegerYap(BelgeSiraNoAl(p.tcKimlikNo, p.tip, p.kayitSiraNo)) + ",";
            sql += VeriErisim.DegerYap(p.belgeAdi) + ",";
            string imgStr = "0x" + System.BitConverter.ToString(p.belge).Replace("-", string.Empty);
            sql += imgStr + ")";

            return KomutCalistir(sql, 1);
        }
        public int BelgeSiraNoAl(string tcKimlikNo, int tip, string kayitSiraNo)
        {
            string sql = "";
            int siraNo = 0;

            sql = "SELECT MAX(" + ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGESIRANO] + ") FROM " + TABLOADBELGELER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.TCKIMLIKNO], "=", tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.TIP], "=", tip));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.KAYITSIRANO], "=", kayitSiraNo));

            IDataReader r = veriErisim.DataReaderDondur(sql);
            try
            {
                if (r.Read())
                    siraNo = r.GetInt32(0);
            }
            catch { }
            r.Close();

            return (siraNo + 1);
        }

        public bool PersonelVarMi(Personel p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT COUNT(TCKIMLIKNO) FROM " + TABLOAD;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        public bool PersonelAileVarMi(PersonelAile p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT COUNT(TCKIMLIKNO) FROM " + TABLOADAILE;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.TCKIMLIKNO], "=", p.tcKimlikNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.PERSONELTCKIMLIKNO], "=", p.personelTcKimlikNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        public Sonuc KaydetBirim(Personel p)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], p.muhasebeKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], p.birimKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], p.organizasyonKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUM], p.kurum.Replace(".", ""));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetAile(PersonelAile p)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOADAILE + " (" + VeriErisim.TumAlanlar("", ALANLARAILE) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.personelTcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.soyad) + ",";
            sql += VeriErisim.DegerYap(p.dogumTarihi) + ",";
            sql += VeriErisim.DegerYap(p.cinsiyet) + ",";
            sql += VeriErisim.DegerYap(p.yakinlik) + ",";
            sql += VeriErisim.DegerYap(p.durum) + ",";
            sql += VeriErisim.DegerYap(p.saglikKarneNo) + ",";
            sql += VeriErisim.DegerYap(p.evlenmeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.aileCuzdaniSeriNo) + ",";

            sql += VeriErisim.DegerYap(p.esininCalistigiKurum) + ",";
            sql += VeriErisim.DegerYap(p.esininUnvani) + ",";
            sql += VeriErisim.DegerYap(p.esininGorevYeri) + ",";
            sql += VeriErisim.DegerYap(p.cocugununOkulununAdi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetMalBildirimi(PersonelMalBildirimi p)
        {
            string sql = "";
            string sayacKosul = ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
            p.sirano = TABGenel.SayacDegerAl(veriErisim, TABLOADMALBILDIRIMI, ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.SIRANO], sayacKosul);

            sql = "INSERT INTO " + TABLOADMALBILDIRIMI + " (" + VeriErisim.TumAlanlar("", ALANLARMALBILDIRIMI) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.sirano) + ",";
            sql += VeriErisim.DegerYap(p.tarih) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetCezaOdul(PersonelCezaOdul p)
        {
            string sql = "";
            string sayacKosul = ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
            p.sirano = TABGenel.SayacDegerAl(veriErisim, TABLOADCEZAODUL, ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.SIRANO], sayacKosul);

            sql = "INSERT INTO " + TABLOADCEZAODUL + " (" + VeriErisim.TumAlanlar("", ALANLARCEZAODUL) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.sirano) + ",";
            sql += VeriErisim.DegerYap(p.tip) + ",";
            sql += VeriErisim.DegerYap(p.cezaodultip) + ",";
            sql += VeriErisim.DegerYap(p.verilisNedeni) + ",";
            sql += VeriErisim.DegerYap(p.tarih) + ",";
            sql += VeriErisim.DegerYap(p.belgeNo) + ",";
            sql += VeriErisim.DegerYap(p.dayanak) + ",";
            sql += VeriErisim.DegerYap(p.belgeTarihi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetOgrenim(PersonelOgrenim p)
        {
            string sql = "";
            string sayacKosul = ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
            p.siraNo = TABGenel.SayacDegerAl(veriErisim, TABLOADOGRENIM, ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.SIRANO], sayacKosul);

            sql = "INSERT INTO " + TABLOADOGRENIM + " (" + VeriErisim.TumAlanlar("", ALANLAROGRENIM) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.ogrenimKod) + ",";
            sql += VeriErisim.DegerYap(p.okulAdi) + ",";
            sql += VeriErisim.DegerYap(p.fakulteBolum) + ",";
            sql += VeriErisim.DegerYap(p.belgeTur) + ",";
            sql += VeriErisim.DegerYap(p.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.belgeNo) + ",";
            sql += VeriErisim.DegerYap(p.baslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(p.bitirmeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.derece) + ",";
            sql += VeriErisim.DegerYap(p.gecerli) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetDil(PersonelDil p)
        {
            string sql = "";
            string sayacKosul = ALANLARDIL[(int)ENUMAlanlarDil.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
            p.siraNo = TABGenel.SayacDegerAl(veriErisim, TABLOADDIL, ALANLARDIL[(int)ENUMAlanlarDil.SIRANO], sayacKosul);

            sql = "INSERT INTO " + TABLOADDIL + " (" + VeriErisim.TumAlanlar("", ALANLARDIL) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.dilKod) + ",";
            sql += VeriErisim.DegerYap(p.duzey) + ",";
            sql += VeriErisim.DegerYap(p.puan) + ",";
            sql += VeriErisim.DegerYap(p.donemAy) + ",";
            sql += VeriErisim.DegerYap(p.donemYil) + ",";
            sql += VeriErisim.DegerYap(p.belgeTur) + ",";
            sql += VeriErisim.DegerYap(p.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.belgeNo) + ",";
            sql += VeriErisim.DegerYap(p.bordroyaEsas) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetMemuriyet(PersonelMemuriyet p)
        {
            string sql = "";
            string sayacKosul = ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
            p.siraNo = TABGenel.SayacDegerAl(veriErisim, TABLOADMEMURIYET, ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.SIRANO], sayacKosul);

            sql = "INSERT INTO " + TABLOADMEMURIYET + " (" + VeriErisim.TumAlanlar("", ALANLARMEMURIYET) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.organizasyonKodu) + ",";
            sql += VeriErisim.DegerYap(p.muhasebeKodu) + ",";
            sql += VeriErisim.DegerYap(p.birimKodu) + ",";
            sql += VeriErisim.DegerYap(p.kurum.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(p.birimAd) + ",";
            sql += VeriErisim.DegerYap(p.gorev) + ",";
            sql += VeriErisim.DegerYap(p.istihdamSekli) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ",";
            sql += VeriErisim.DegerYap(p.baslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.ayrilmaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.baslamaNedeni) + ",";
            sql += VeriErisim.DegerYap(p.ayrilmaNedeni) + ",";
            sql += VeriErisim.DegerYap(p.evrakNo) + ",";
            sql += VeriErisim.DegerYap(p.yasalDayanak) + ",";
            sql += VeriErisim.DegerYap(p.onayTarih) + ",";
            sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
            sql += VeriErisim.DegerYap(p.kadroTarih) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasDerece) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasKademe) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasTarih) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakDerece) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakKademe) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakTarih) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasDerece) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasKademe) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasTarih) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasGosterge) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisGosterge) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasGosterge) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.malul) + ",";
            sql += VeriErisim.DegerYap(p.daireKodu) + ")";


            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetKadro(PersonelKadro p)
        {
            string sql = "";
            string sayacKosul = ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
            p.siraNo = TABGenel.SayacDegerAl(veriErisim, TABLOADKADRO, ALANLARKADRO[(int)ENUMAlanlarKadro.SIRANO], sayacKosul);

            sql = "INSERT INTO " + TABLOADKADRO + " (" + VeriErisim.TumAlanlar("", ALANLARKADRO) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.organizasyonKodu) + ",";
            sql += VeriErisim.DegerYap(p.muhasebeKodu) + ",";
            sql += VeriErisim.DegerYap(p.birimKodu) + ",";
            sql += VeriErisim.DegerYap(p.kurum.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(p.birimAd) + ",";
            sql += VeriErisim.DegerYap(p.unvan) + ",";
            sql += VeriErisim.DegerYap(p.sinifi) + ",";
            sql += VeriErisim.DegerYap(p.evrakNo) + ",";
            sql += VeriErisim.DegerYap(p.yasalDayanak) + ",";
            sql += VeriErisim.DegerYap(p.onayTarih) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ",";
            sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
            sql += VeriErisim.DegerYap(p.kadroTarih) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasDerece) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasKademe) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasTarih) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakDerece) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakKademe) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisHakTarih) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasDerece) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasKademe) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasTarih) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasGosterge) + ",";
            sql += VeriErisim.DegerYap(p.odemeyeEsasEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisGosterge) + ",";
            sql += VeriErisim.DegerYap(p.kazanilmisEkGosterge) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasGosterge) + ",";
            sql += VeriErisim.DegerYap(p.emekliyeEsasEkGosterge) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetKurs(PersonelKurs p)
        {
            string sql = "";
            p.siraNo = TABGenel.SayacDegerAl(veriErisim, TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.SIRANO], ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo));

            sql = "INSERT INTO " + TABLOADKURS + " (" + VeriErisim.TumAlanlar("", ALANLARKURS) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.kursKod) + ",";
            sql += VeriErisim.DegerYap(p.aldigiYer) + ",";
            sql += VeriErisim.DegerYap(p.baslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(p.bitirmeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.derece) + ",";
            sql += VeriErisim.DegerYap(p.kursAdi) + ",";
            sql += VeriErisim.DegerYap(p.dersSaati) + ",";
            sql += VeriErisim.DegerYap(p.egitimYeri) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ",";
            sql += VeriErisim.DegerYap(p.donem) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(p.gun) + ",";
            sql += VeriErisim.DegerYap(p.katilimTipi) + ",";
            sql += VeriErisim.DegerYap(p.kursNiteligi) + ",";
            sql += VeriErisim.DegerYap(p.yapildigiIl) + ",";
            sql += VeriErisim.DegerYap(p.egitimSaati) + ",";
            sql += VeriErisim.DegerYap(p.egitimTurKodu) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetEgitimTanim(PersonelEgitimTurleri p)
        {
            string sql = "";
            p.kod = TABGenel.SayacDegerAl(veriErisim, TABLOADEGITIMTURLERI, ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.KOD], "");
            if (p.seviye == (int)ENUMEGITIMSEVIYEBILGISI.EGITIMGRUBU)
            {
                sql += "INSERT INTO " + TABLOADEGITIMTURLERI + " (KOD,AD,SEVIYE) VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                sql += VeriErisim.DegerYap(p.ad) + ",";
                sql += VeriErisim.DegerYap(p.seviye) + ")";
            }
            else
            {
                sql = "INSERT INTO " + TABLOADEGITIMTURLERI + " (" + VeriErisim.TumAlanlar("", ALANLAREGITIMTURLERI) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                sql += VeriErisim.DegerYap(p.ataKod) + ",";
                sql += VeriErisim.DegerYap(p.ad) + ",";
                sql += VeriErisim.DegerYap(p.seviye) + ")";
            }


            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetIzin(PersonelIzin p)
        {
            string sql = "";

            string sayacKosul = ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO] + "=" + VeriErisim.DegerYap(p.tcKimlikNo);
            p.sirano = TABGenel.SayacDegerAl(veriErisim, TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], sayacKosul);

            sql = "INSERT INTO " + TABLOADIZIN + " (" + VeriErisim.TumAlanlar("", ALANLARIZIN) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.sirano) + ",";
            sql += VeriErisim.DegerYap(p.tip) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ",";
            sql += VeriErisim.DegerYap(p.baslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(p.bitisTarihi) + ",";
            sql += VeriErisim.DegerYap(p.telefon) + ",";
            sql += VeriErisim.DegerYap(p.kullanilanGun) + ",";
            sql += VeriErisim.DegerYap(p.izinNotu) + ",";
            sql += VeriErisim.DegerYap(p.onay) + ",";
            sql += VeriErisim.DegerYap(p.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(p.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(p.alan1) + ",";
            sql += VeriErisim.DegerYap(p.not) + ",";
            sql += VeriErisim.DegerYap(p.raporTarihi) + ",";
            sql += VeriErisim.DegerYap(p.raporVeren) + ",";
            sql += VeriErisim.DegerYap(p.raporNumara) + ")";
            //sql += VeriErisim.DegerYap(p.raporTuru) + ")";

            return KomutCalistir(sql, 1);
        }

        public int KaydetIzinUygunMu(PersonelIzin p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT COUNT(*) FROM " + TABLOADIZIN;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], "<=", p.bitisTarihi));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], ">=", p.baslangicTarihi));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "<>", p.sirano));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc KaydetOnayIzin(PersonelIzin p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADIZIN + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.ONAY], p.onay);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetIzinAlan(PersonelIzin p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADIZIN + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVRAKALAN1], p.alan1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVRAKNOT1], p.not);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetIzinEvrakBilgi(PersonelIzin p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADIZIN + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVREVRAKNO], p.evrEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.PREVRAKNO], p.prEvrakNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetIzinDevir(PersonelIzinDevir p)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOADIZINDEVIR + " (" + VeriErisim.TumAlanlar("", ALANLARIZINDEVIR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.devreden) + ",";
            sql += VeriErisim.DegerYap(p.yilici) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetIzinTalep(PersonelIzinTalep p)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOADIZINTALEP + " (" + VeriErisim.TumAlanlar("", ALANLARIZINTALEP) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.donemId) + ",";
            sql += VeriErisim.DegerYap(p.talepBaslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(p.talepBitisTarihi) + ",";
            sql += VeriErisim.DegerYap(11) + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap(0) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetTarihce(PersonelTarihce p)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.personel) + ",";
            sql += VeriErisim.DegerYap(p.kullanici) + ",";
            sql += VeriErisim.DegerYap(p.tarih) + ",";
            sql += VeriErisim.DegerYap(p.degisiklik) + ",";
            sql += VeriErisim.DegerYap(p.kullaniciAd) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetSozlesmeliBordroTanim(SozlesmeliBordroTanim p)
        {
            string sql = "INSERT INTO " + TABLOADSOZLESMELIBORDROTANIM + " (" + VeriErisim.TumAlanlar("", ALANLARSOZLESMELIBORDROTANIM) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.ay) + ",";
            sql += VeriErisim.DegerYap(p.kumulatifVergiMatrahi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(Personel p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], p.sicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], p.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOYAD], p.soyad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BABAAD], p.babaAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAAD], p.anaAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOGUMTARIHI], p.dogumTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CINSIYET], p.cinsiyet) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEDENIHALI], p.medeniHali) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KANGRUBU], p.kanGrubu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASKERLIKKODU], p.askerlikKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OGRENIMKODU], p.ogrenimKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ESDURUMUKODU], p.esDurumuKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAGLIKKARNENO], p.saglikKarneNo) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], p.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLISICILNO], p.emekliSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNCELLENMETARIHI], p.guncellenmeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNCELLEYENKISI], p.guncelleyenKisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKAKODU], p.bankaKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKASUBEKODU], p.bankaSubeKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKAHESAPNO], p.bankaHesapNo) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESKILATKODU], p.teskilatKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVANKODU], p.unvanKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], p.muhasebeKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], p.birimKodu) + ",";

            if (TNS.PER.Arac.MerkezBankasiKullaniyor())
            {
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEPTELEFON], p.cepTelefon) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], p.eposta) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], p.faks) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], p.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVANKODU], p.unvanKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACILKISI], p.acilKisi) + ",";
            }


            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEMURBASLAMATARIHI], p.memurBaslamaTarihi) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], p.iseBaslamaTarihi) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTENCIKISTARIHI], p.istenCikisTarihi) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIFKODU], p.sinifKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIDEMAY], p.kidemAy) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIDEMYIL], p.kidemYil) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADRODERECE], p.kadroDerece) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROTARIH], p.kadroTarih) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASDERECE], p.odemeEsasDerece) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASKADEME], p.odemeEsasKademe) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASGOSTERGE], p.odemeEsasGosterge) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASTARIH], p.odemeEsasTarih) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASDERECE], p.emekliEsasDerece) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASKADEME], p.emekliEsasKademe) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASGOSTERGE], p.emekliEsasGosterge) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASTARIH], p.emekliEsasTarih) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKDERECE], p.kazanilmisHakDerece) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKKADEME], p.kazanilmisHakKademe) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKTARIH], p.kazanilmisHakTarih) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTIHDAMSEKLI], p.istihdamSekli) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIDAIRESI], p.vergiDairesi) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.USTBIRIMKODU], p.ustBirimKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], p.evrakBirimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODA], p.odaKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKSOYAD], p.ilkSoyad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOGUMYERI], p.dogumYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NUFUSILKOD], p.nufusIlKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NUFUSILCEKOD], p.nufusIlceKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CUZDANSERINO], p.cuzdanSeriNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CUZDANKAYITNO], p.cuzdanKayitNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NUFUSMAHALLEKOY], p.nufusMahalleKoy) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NUFUSCILTNO], p.nufusCiltNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NUFUSSIRANO], p.nufusSiraNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NUFUSAILESIRANO], p.nufusAileSiraNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CUZDANVERILISNEDENI], p.cuzdanVerilisNedeni) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CUZDANVERILDIGIYER], p.cuzdanVerildigiYer) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CUZDANVERILISTARIHI], p.cuzdanVerilisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BAKANLIKKARTNO], p.bakanlikKartNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SSKSICILNO], p.sskSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BAGKURSICILNO], p.bagkurSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURUCUBELGENO], p.surucuBelgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURUCUBELGETARIHI], p.surucuBelgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YASTASHIHTARIHI], p.yasTashihTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], p.organizasyonKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUM], p.kurum.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TECILTARIHI], p.tecilTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASKERLIKMUAFNEDEN], p.askerlikMuafNeden) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASKERLIKBASLAMATARIHI], p.askerlikBaslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERHISTARIHI], p.terhisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASKERLIKCUZDANNO], p.askerlikCuzdanNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleAdres(PersonelAdres p)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], p.adres) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEBLIGATADRES], p.tebligatAdres) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MULKDURUM], p.mulkDurumu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMATARIHI], p.tasinmaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTELEFON], p.isTel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVTELEFON], p.evTel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEPTELEFON], p.cepTel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], p.faks) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], p.eposta) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.URL], p.url) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MOBILIMZATELEFON], p.mobilImzaTel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACILKISI], p.acilKisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACILKISINUMARA], p.acilKisiNumara);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleAile(PersonelAile p)
        {
            string sql = "UPDATE " + TABLOADAILE + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.AD], p.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.SOYAD], p.soyad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.DOGUMTARIHI], p.dogumTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.CINSIYET], p.cinsiyet) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.YAKINLIKDERECESI], p.yakinlik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.DURUM], p.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.SAGLIKKARNENO], p.saglikKarneNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.EVLENMETARIHI], p.evlenmeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.AILECUZDANISERINO], p.aileCuzdaniSeriNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.ESININCALISTIGIKURUM], p.esininCalistigiKurum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.ESININUNVANI], p.esininUnvani) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.ESININGOREVYERI], p.esininGorevYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARAILE[(int)ENUMAlanlarAile.COCUGUNUNOKULUNUNADI], p.cocugununOkulununAdi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARAILE[(int)ENUMAlanlarAile.PERSONELTCKIMLIKNO], "=", p.personelTcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAILE[(int)ENUMAlanlarAile.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleMalBildirimi(PersonelMalBildirimi p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADMALBILDIRIMI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TARIH], p.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.ACIKLAMA], p.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleCezaOdul(PersonelCezaOdul p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADCEZAODUL + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.TIP], p.tip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.CEZAODULTIP], p.cezaodultip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.VERILISNEDENI], p.verilisNedeni) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.TARIH], p.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.BELGENO], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.DAYANAK], p.dayanak) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.BELGETARIHI], p.belgeTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleOgrenim(PersonelOgrenim p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADOGRENIM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.OGRENIMKOD], p.ogrenimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.OKULADI], p.okulAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.FAKULTEBOLUM], p.fakulteBolum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BELGETUR], p.belgeTur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BELGETARIHI], p.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BELGENO], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BASLANGICTARIHI], p.baslangicTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.BITIRMETARIHI], p.bitirmeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.DERECE], p.derece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.GECERLI], p.gecerli);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleDil(PersonelDil p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADDIL + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.DILKODU], p.dilKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.DUZEY], p.duzey) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.PUAN], p.puan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.DONEMAY], p.donemAy) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.DONEMYIL], p.donemYil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.BELGETUR], p.belgeTur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.BELGETARIHI], p.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.BELGENO], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.BORDROYAESAS], p.bordroyaEsas);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleMemuriyet(PersonelMemuriyet p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADMEMURIYET + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ORGANIZASYONKOD], p.organizasyonKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.MUHASEBEKODU], p.muhasebeKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.BIRIMKODU], p.birimKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.KURUM], p.kurum.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.BIRIMAD], p.birimAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.GOREV], p.gorev) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ISTIHDAMSEKLI], p.istihdamSekli) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ACIKLAMA], p.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.BASLAMATARIHI], p.baslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.AYRILMATARIHI], p.ayrilmaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.BASLAMANEDENI], p.baslamaNedeni) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.AYRILMANEDENI], p.ayrilmaNedeni) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.EVRAKNO], p.evrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.YASALDAYANAK], p.yasalDayanak) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ONAYTARIH], p.onayTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.MALUL], p.malul) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.DAIREKODU], p.daireKodu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleKadro(PersonelKadro p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADKADRO + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ORGANIZASYONKOD], p.organizasyonKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.MUHASEBEKODU], p.muhasebeKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.BIRIMKODU], p.birimKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KURUM], p.kurum.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.BIRIMAD], p.birimAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.UNVANKOD], p.unvan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.SINIFKOD], p.sinifi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.EVRAKNO], p.evrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.YASALDAYANAK], p.yasalDayanak) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ONAYTARIH], p.onayTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ACIKLAMA], p.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KADRODERECE], p.kadroDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KADROTARIH], p.kadroTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ODEMEYEESASDERECE], p.odemeyeEsasDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ODEMEYEESASKADEME], p.odemeyeEsasKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ODEMEYEESASTARIH], p.odemeyeEsasTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KAZANILMISHAKDERECE], p.kazanilmisHakDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KAZANILMISHAKKADEME], p.kazanilmisHakKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KAZANILMISHAKTARIH], p.kazanilmisHakTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.EMEKLIYEESASDERECE], p.emekliyeEsasDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.EMEKLIYEESASKADEME], p.emekliyeEsasKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.EMEKLIYEESASTARIH], p.emekliyeEsasTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ODEMEYEESASGOSTERGE], p.odemeyeEsasGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ODEMEYEESASEKGOSTERGE], p.odemeyeEsasEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KAZANILMISGOSTERGE], p.kazanilmisGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KAZANILMISEKGOSTERGE], p.kazanilmisEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.EMEKLIYEESASGOSTERGE], p.emekliyeEsasGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.EMEKLIYEESASEKGOSTERGE], p.emekliyeEsasEkGosterge);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleKurs(PersonelKurs p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADKURS + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.KURSKODU], p.kursKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.ALDIGIYER], p.aldigiYer) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.BASLAMATARIHI], p.baslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.BITIRMETARIHI], p.bitirmeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.DERECE], p.derece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.KURSADI], p.kursAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.DERSSAATI], p.dersSaati) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMYERI], p.egitimYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.ACIKLAMA], p.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.DONEM], p.donem) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.YIL], p.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.DOSYANO], p.dosyaNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.GUN], p.gun) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.KATILIMTIPI], p.katilimTipi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.KURSNITELIGI], p.kursNiteligi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.YAPILDIGIIL], p.yapildigiIl) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMSAATI], p.egitimSaati) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARKURS[(int)ENUMAlanlarKurs.EGITIMTURKODU], p.egitimTurKodu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKURS[(int)ENUMAlanlarKurs.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }
        public Sonuc GuncelleEgitimTanim(PersonelEgitimTurleri p)
        {
            string sql = "";
            if (p.seviye == (int)ENUMEGITIMSEVIYEBILGISI.EGITIMGRUBU)
            {
                sql = "UPDATE " + TABLOADEGITIMTURLERI + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.AD], p.ad);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.KOD], "=", p.kod));
            }
            else
            {
                sql = "UPDATE " + TABLOADEGITIMTURLERI + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.AD], p.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.ATAKOD], p.ataKod);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.KOD], "=", p.kod));

            }

            return KomutCalistir(sql, 1);
        }
        public Sonuc GuncelleIzin(PersonelIzin p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADIZIN + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], p.tcKimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], p.tip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.ACIKLAMA], p.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], p.baslangicTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], p.bitisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.TELEFON], p.telefon) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN], p.kullanilanGun) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.IZINNOTU], p.izinNotu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.ONAY], p.onay) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVREVRAKNO], p.evrEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.PREVRAKNO], p.prEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVRAKALAN1], p.alan1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.EVRAKNOT1], p.not) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.RAPORTARIHI], p.raporTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.RAPORVEREN], p.raporVeren) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.RAPORNUMARA], p.raporNumara);
            //sql += VeriErisim.AtamaOl("", ALANLARIZIN[(int)ENUMAlanlarIzin.RAPORTURU], p.raporTuru);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleIzinDevir(PersonelIzinDevir p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADIZINDEVIR + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.DEVREDEN], p.devreden) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YILICI], p.yilici);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL], "=", p.yil));

            return base.KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleIzinTalep(PersonelIzinTalep p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADIZINTALEP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.BASLAMATARIHI], p.talepBaslangicTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.BITISTARIHI], p.talepBitisTarihi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZINTALEP, ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZINTALEP, ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.YIL], "=", p.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZINTALEP, ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.DONEMID], "=", p.donemId));

            return base.KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleSozlesmeliBordroTanim(SozlesmeliBordroTanim p)
        {
            string sql = "UPDATE " + TABLOADSOZLESMELIBORDROTANIM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.KUMULATIFVERGIMATRAHI], p.kumulatifVergiMatrahi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.YIL], p.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.AY], p.ay);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(Personel p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], "=", p.sicilNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilBelgeler(PersonelBelgeler p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADBELGELER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.TIP], "=", p.tip));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.KAYITSIRANO], "=", p.kayitSiraNo));

            if (p.belgeSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGESIRANO], "=", p.belgeSiraNo));

            if (p.belgeSiraNo > 0)
                return KomutCalistir(sql, 1);
            else
                return KomutCalistir(sql, 0);
        }

        public Sonuc SilSozlesmeliBordroTanim(SozlesmeliBordroTanim p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADSOZLESMELIBORDROTANIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSOZLESMELIBORDROTANIM[(int)ENUMAlanlarSozlesmeliBordro.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilKurs(PersonelKurs p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADKURS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADKURS, ALANLARKURS[(int)ENUMAlanlarKurs.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilEgitimTanim(PersonelEgitimTurleri p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADEGITIMTURLERI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADEGITIMTURLERI, ALANLAREGITIMTURLERI[(int)ENUMAlanlarEgitimTurleri.KOD], "=", p.kod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilAile(PersonelAile p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADAILE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.PERSONELTCKIMLIKNO], "=", p.personelTcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADAILE, ALANLARAILE[(int)ENUMAlanlarAile.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilMalBildirimi(PersonelMalBildirimi p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADMALBILDIRIMI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilCezaOdul(PersonelCezaOdul p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADCEZAODUL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARCEZAODUL[(int)ENUMAlanlarCezaOdul.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilOgrenim(PersonelOgrenim p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADOGRENIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDil(PersonelDil p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADDIL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilMemuriyet(PersonelMemuriyet p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADMEMURIYET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilKadro(PersonelKadro p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADKADRO;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMALBILDIRIMI[(int)ENUMAlanlarMalBildirimi.SIRANO], "=", p.siraNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilIzin(PersonelIzin p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADIZIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
            if (p.sirano > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "=", p.sirano));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilIzinDevir(PersonelIzinDevir p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADIZIN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL], "=", p.yil));

            return KomutCalistir(sql, 0);
        }

        public void GuncelleOgrenimGecerli(PersonelOgrenim p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADOGRENIM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.GECERLI], 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.TCKIMLIKNO], "=", p.tcKimlikNo));
            KomutCalistir(sql, 0);


            sql = "UPDATE " + TABLOADOGRENIM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.GECERLI], 1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROGRENIM[(int)ENUMAlanlarOgrenim.SIRANO], "=", p.siraNo));
            KomutCalistir(sql, 1);


            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OGRENIMKODU], p.ogrenimKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));
            KomutCalistir(sql, 1);
        }

        public void GuncelleDilBordroyaEsas(PersonelDil p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADDIL + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.BORDROYAESAS], 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.TCKIMLIKNO], "=", p.tcKimlikNo));
            KomutCalistir(sql, 0);


            sql = "UPDATE " + TABLOADDIL + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDIL[(int)ENUMAlanlarDil.BORDROYAESAS], 1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDIL[(int)ENUMAlanlarDil.SIRANO], "=", p.siraNo));
            KomutCalistir(sql, 1);
        }

        public Sonuc MemuriyetPersonelBirimGuncelle(PersonelMemuriyet p)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], p.muhasebeKodu) + ",";//Ahmet ilhan bu işleme gerek olmadığını söyledi (Melih)
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], p.birimKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], p.organizasyonKodu) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUM], p.kurum.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTIHDAMSEKLI], p.istihdamSekli);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc MemuriyetPersonelAyrilmaGuncelle(string tcKimlikNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKODU], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "") + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROMUHASEBEKODU], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROBIRIMKODU], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROORGANIZASYONKOD], 0) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROKURUM], "");
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc MemuriyetTarihleriGuncelle(string tcKimlikNo, TNSDateTime memuriyetBaslamaTarihi, TNSDateTime sonBirimeBaslamaTarihi, TNSDateTime sonGoreveBaslamaTarihi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEMURBASLAMATARIHI], memuriyetBaslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONBIRIMBASLAMATARIHI], sonBirimeBaslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONGOREVBASLAMATARIHI], sonGoreveBaslamaTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc MemuriyetKadroBilgileriniGuncelle(PersonelMemuriyet p)
        {
            string sql = "UPDATE " + TABLOADMEMURIYET + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.KADRODERECE], p.kadroDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.KADROTARIH], p.kadroTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ODEMEYEESASDERECE], p.odemeyeEsasDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ODEMEYEESASKADEME], p.odemeyeEsasKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ODEMEYEESASTARIH], p.odemeyeEsasTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.KAZANILMISHAKDERECE], p.kazanilmisHakDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.KAZANILMISHAKKADEME], p.kazanilmisHakKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.KAZANILMISHAKTARIH], p.kazanilmisHakTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.EMEKLIYEESASDERECE], p.emekliyeEsasDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.EMEKLIYEESASKADEME], p.emekliyeEsasKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.EMEKLIYEESASTARIH], p.emekliyeEsasTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ODEMEYEESASGOSTERGE], p.odemeyeEsasGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.ODEMEYEESASEKGOSTERGE], p.odemeyeEsasEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.KAZANILMISGOSTERGE], p.kazanilmisGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.KAZANILMISEKGOSTERGE], p.kazanilmisEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.EMEKLIYEESASGOSTERGE], p.emekliyeEsasGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.EMEKLIYEESASEKGOSTERGE], p.emekliyeEsasEkGosterge);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.TCKIMLIKNO], "=", p.tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.SIRANO], "=", p.siraNo));
            return KomutCalistir(sql, 1);
        }

        public IDataReader MemuriyeteUygunKadroBilgileriniAl(string tcKimlikNo, TNSDateTime ayrilisTarihi, int tip)
        {
            string sql = "";
            if (tip == 1)
            {
                sql = "SELECT TOP (1) KADRODERECE, KADROTARIH FROM PERPERSONELKADRODERECE";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", tcKimlikNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KADROTARIH], "<=", ayrilisTarihi));
                sql += " ORDER BY KADROTARIH DESC";
            }
            else if (tip == 2)
            {
                sql = "SELECT TOP (1) ODEMEYEESASDERECE, ODEMEYEESASKADEME, ODEMEYEESASTARIH, ODEMEYEESASGOSTERGE, ODEMEYEESASEKGOSTERGE FROM PERPERSONELKADRODERECE";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", tcKimlikNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.ODEMEYEESASTARIH], "<=", ayrilisTarihi));
                sql += " ORDER BY ODEMEYEESASTARIH DESC";
            }
            else if (tip == 3)
            {
                sql = "SELECT TOP (1) KAZANILMISHAKDERECE, KAZANILMISHAKKADEME, KAZANILMISHAKTARIH, KAZANILMISGOSTERGE, KAZANILMISEKGOSTERGE FROM PERPERSONELKADRODERECE";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", tcKimlikNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.KAZANILMISHAKTARIH], "<=", ayrilisTarihi));
                sql += " ORDER BY KAZANILMISHAKTARIH DESC";
            }
            else if (tip == 4)
            {
                sql = "SELECT TOP (1) EMEKLIYEESASDERECE, EMEKLIYEESASKADEME, EMEKLIYEESASTARIH, EMEKLIYEESASGOSTERGE, EMEKLIYEESASEKGOSTERGE FROM PERPERSONELKADRODERECE";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.TCKIMLIKNO], "=", tcKimlikNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKADRO[(int)ENUMAlanlarKadro.EMEKLIYEESASTARIH], "<=", ayrilisTarihi));
                sql += " ORDER BY EMEKLIYEESASTARIH DESC";
            }
            return veriErisim.DataReaderDondur(sql);
        }

        public TNSDateTime MemuriyetTarihiAl(string tcKimlikNo, int tarihTip)
        {
            string sql = "";
            if (tarihTip == 1)//memuriyet başlama tarihi
                sql = "SELECT BASLAMATARIHI FROM PERPERSONELMEMURIYET WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "' and BASLAMATARIHI = (SELECT MIN(BASLAMATARIHI) FROM PERPERSONELMEMURIYET  WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "')";
            else if (tarihTip == 2)//Göreve başlama tarihi
                sql = "SELECT BASLAMATARIHI FROM PERPERSONELMEMURIYET WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "' and BASLAMATARIHI = (SELECT MAX(BASLAMATARIHI) FROM PERPERSONELMEMURIYET  WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "')";
            else if (tarihTip == 3)//Birime başlama tarihi
                sql = "SELECT BASLAMATARIHI FROM PERPERSONELMEMURIYET WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "' and BASLAMATARIHI = (SELECT MAX(BASLAMATARIHI) FROM PERPERSONELMEMURIYET  WHERE ISTIHDAMSEKLI <> 21 AND TCKIMLIKNO = '" + tcKimlikNo + "')";

            return veriErisim.TekKolonSQLDate(sql);
        }

        public Sonuc KadroBilgisiPersonelGuncelle(Personel p)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROORGANIZASYONKOD], p.kadroOrganizasyonKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROMUHASEBEKODU], p.kadroMuhasebeKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROBIRIMKODU], p.kadroBirimKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROKURUM], p.kadroKurum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADRODERECE], p.kadroDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROTARIH], p.kadroTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASDERECE], p.odemeEsasDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASKADEME], p.odemeEsasKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASGOSTERGE], p.odemeEsasGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASEKGOSTERGE], p.odemeEsasEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEESASTARIH], p.odemeEsasTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASDERECE], p.emekliEsasDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASKADEME], p.emekliEsasKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASGOSTERGE], p.emekliEsasGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASEKGOSTERGE], p.emekliEsasEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLIESASTARIH], p.emekliEsasTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKDERECE], p.kazanilmisHakDerece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKKADEME], p.kazanilmisHakKademe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKTARIH], p.kazanilmisHakTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKGOSTERGE], p.kazanilmisHakGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAZANILMISHAKEKGOSTERGE], p.kazanilmisHakEkGosterge) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVANKODU], p.unvanKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIFKODU], p.sinifKodu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));

            return KomutCalistir(sql, 1);
        }

        public IDataReader KadroBilgisiAl(string tcKimlikNo, int tip)
        {
            string sql = "";
            if (tip == 1)//kadro bilgisi
                sql = "SELECT ORGANIZASYONKOD, MUHASEBEKODU, BIRIMKODU, KURUM, UNVANKOD, SINIFKOD, KADRODERECE, KADROTARIH FROM PERPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "' AND KADROTARIH = (SELECT MAX(KADROTARIH) FROM PERPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "')";
            else if (tip == 2)//Ödeme Esas
                sql = "SELECT ODEMEYEESASDERECE, ODEMEYEESASKADEME, ODEMEYEESASGOSTERGE, ODEMEYEESASTARIH, ODEMEYEESASEKGOSTERGE FROM PERPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "' AND ODEMEYEESASTARIH = (SELECT MAX(ODEMEYEESASTARIH) FROM PERPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "')";
            else if (tip == 3)//Kazanılmış hak
                sql = "SELECT KAZANILMISHAKDERECE, KAZANILMISHAKKADEME, KAZANILMISHAKTARIH, KAZANILMISGOSTERGE, KAZANILMISEKGOSTERGE FROM PERPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "' AND KAZANILMISHAKTARIH = (SELECT MAX(KAZANILMISHAKTARIH) FROM PERPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "')";
            else if (tip == 4)//Emekliye Esas
                sql = "SELECT EMEKLIYEESASDERECE, EMEKLIYEESASKADEME, EMEKLIYEESASGOSTERGE, EMEKLIYEESASTARIH, EMEKLIYEESASEKGOSTERGE FROM PERPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "' AND EMEKLIYEESASTARIH = (SELECT MAX(EMEKLIYEESASTARIH) FROM PERPERSONELKADRODERECE WHERE TCKIMLIKNO = '" + tcKimlikNo + "')";

            return veriErisim.DataReaderDondur(sql);
        }

        public bool IzinKayitKontrol(PersonelIzin p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT SUM(" + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ")"
                + " FROM " + TABLOADIZIN
                + " INNER JOIN PERIZINTUR ON " + ALANLARIZIN[(int)ENUMAlanlarIzin.TIP] + " = PERIZINTUR.KOD";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TIP], "=", p.tip));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.TCKIMLIKNO], "=", p.tcKimlikNo));
            if (p.sirano > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.SIRANO], "<>", p.sirano));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + DateTime.Now.Year.ToString("d4"))));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZIN, ALANLARIZIN[(int)ENUMAlanlarIzin.BITISTARIHI], "<=", new TNSDateTime("31.12." + DateTime.Now.Year.ToString("d4"))));

            sql += " WHERE " + kosul;
            sql += " GROUP BY PERIZINTUR.KOD";
            int dahaOnceKullandigiToplamIzin = veriErisim.TekKolonSQLInt(sql);
            sql = "SELECT PERIZINTUR.SURE FROM PERIZINTUR WHERE PERIZINTUR.KOD=" + p.tip;
            int kullanabilecegiToplamIzin = veriErisim.TekKolonSQLInt(sql);
            if (kullanabilecegiToplamIzin == 0)
                return true;
            if ((kullanabilecegiToplamIzin - dahaOnceKullandigiToplamIzin) >= p.kullanilanGun)
                return true;
            else
                return false;
        }

        public int IstihdamSekliniVer(string tcKimlikNo)
        {
            string sql = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.ISTIHDAMSEKLI] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", tcKimlikNo));

            return veriErisim.TekKolonSQLInt(sql);

        }

        public Sonuc IzinEvrakNoKaydet(string tcKimlikNo, int siraNo, string prEvrakNo)
        {
            string sql = "UPDATE PERPERSONELIZIN SET ";
            sql += VeriErisim.AtamaOl("", "PREVRAKNO", prEvrakNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "TCKIMLIKNO", "=", tcKimlikNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SIRANO", "=", siraNo));

            return KomutCalistir(sql, 1);
        }

        public string IzinEvrakNoOku(string tcKimlikNo, int siraNo)
        {
            string sql = "SELECT PREVRAKNO FROM PERPERSONELIZIN ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SIRANO", "=", siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "TCKIMLIKNO", "=", tcKimlikNo));

            return veriErisim.TekKolonSQLStr(sql);
        }

        //
        //  Personel Yıllık İzin Talep İşlemleri
        //
        public IDataReader ListeleIzinTalep(PersonelIzinTalep pk)
        {
            string sql = "SELECT "
                + TABLOADPERDONEM + "." + ALANLARPERDONEM[(int)ENUMAlanlarPerDonem.DONEMID] + ", "
                + TABLOADPERDONEM + "." + ALANLARPERDONEM[(int)ENUMAlanlarPerDonem.DONEMYIL] + ", "
                + TABLOADPERDONEM + "." + ALANLARPERDONEM[(int)ENUMAlanlarPerDonem.DONEMBASLANGICTARIHI] + ", "
                + TABLOADPERDONEM + "." + ALANLARPERDONEM[(int)ENUMAlanlarPerDonem.DONEMBITISTARIHI] + ", "
                + TABLOADPERDONEM + "." + ALANLARPERDONEM[(int)ENUMAlanlarPerDonem.DONEMACIKLAMA] + ", "
                + TABLOADIZINTALEP + "." + ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.TCKIMLIKNO] + ", "
                + TABLOADIZINTALEP + "." + ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.BASLAMATARIHI] + ", "
                + TABLOADIZINTALEP + "." + ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.BITISTARIHI] + ", "
                + TABLOADIZINTALEP + "." + ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.DURUMID]
                // + ", "
                //+ "SUM(" + TABLOADIZIN + "." + ALANLARIZIN[(int)ENUMAlanlarIzin.KULLANILANGUN] + ")"
                + " FROM " + TABLOADPERDONEM
                + " LEFT JOIN " + TABLOADIZINTALEP
                + " ON " + ALANLARPERDONEM[(int)ENUMAlanlarPerDonem.DONEMYIL] + " = " + ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.YIL]
                + " and " + TABLOADPERDONEM + "." + ALANLARPERDONEM[(int)ENUMAlanlarPerDonem.DONEMID] + " = " + TABLOADIZINTALEP + "." + ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.DONEMID]
                + " and " + TABLOADIZINTALEP + "." + ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.TCKIMLIKNO] + " = " + pk.tcKimlikNo;

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZINTALEP, ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.TCKIMLIKNO], "=", pk.tcKimlikNo));
            //if (pk.yil > 0)
            //{
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADIZINTALEP, ALANLARIZINTALEP[(int)ENUMAlanlarIzinTalep.YIL], "=", pk.yil));
            //}

            //sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc GuncelleKullaniciMail(PersonelAdres p)
        {
            string sql = "UPDATE Kullanici.dbo.KULLANICI SET EMAIL = " + VeriErisim.DegerYap(p.eposta);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MERNIS", "=", p.tcKimlikNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc PersonelBirimKoduDegistir(int eskiSaymanlik, string eskiBirim, int yeniSaymanlik, string yeniBirim)
        {
            string sql = "UPDATE " + TABLOAD + " SET MUHASEBEKODU= " + VeriErisim.DegerYap(yeniSaymanlik) + ", BIRIMKODU=" + VeriErisim.DegerYap(yeniBirim);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MUHASEBEKODU", "=", eskiSaymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIMKODU", "=", eskiBirim));

            return KomutCalistir(sql, 0);
        }

        public Sonuc MemuriyetBirimKoduDegistir(int eskiSaymanlik, string eskiBirim, int yeniSaymanlik, string yeniBirim)
        {
            string sql = "UPDATE " + TABLOADMEMURIYET + " SET MUHASEBEKODU= " + VeriErisim.DegerYap(yeniSaymanlik) + ", BIRIMKODU=" + VeriErisim.DegerYap(yeniBirim);
            //sql += ", UPDATE " + TABLOADMEMURIYET + " SET MUHASEBEKODU= " + VeriErisim.DegerYap(yeniSaymanlik) + ", BIRIMKODU=" + VeriErisim.DegerYap(yeniBirim);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MUHASEBEKODU", "=", eskiSaymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIMKODU", "=", eskiBirim));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleBirimDaireBilgileri(BirimDaireBilgileri d)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDAIRE, ALANLARDAIRE) + " FROM " + TABLOADDAIRE;


            if (!string.IsNullOrEmpty(d.birimkodu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDAIRE, ALANLARDAIRE[(int)ENUMAlanlarDaire.BIRIMKODU], "=", d.birimkodu));

            if (d.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDAIRE, ALANLARDAIRE[(int)ENUMAlanlarDaire.DURUM], "=", d.durum));



            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDaireRapor(Personel p, PersonelMemuriyet pk)
        {

            string sql = "";
            string kosul = "";
            sql = "SELECT "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + ", "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + ", "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD] + ", "
                + "PERUNVAN.AD, "
                 + "PERPERSONELDAIRE.ADI, "
                + TABLOADMEMURIYET + "." + ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.BASLAMATARIHI] + ", "
                + TABLOADMEMURIYET + "." + ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.AYRILMATARIHI]
                + " FROM " + TABLOAD
                + " INNER JOIN " + TABLOADMEMURIYET + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + TABLOADMEMURIYET + "." + ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.TCKIMLIKNO]
                + " LEFT OUTER JOIN PERUNVAN ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = PERUNVAN.KOD"
                + " LEFT OUTER JOIN PERPERSONELDAIRE ON " + TABLOADMEMURIYET + "." + ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.DAIREKODU] + " = PERPERSONELDAIRE.KOD";

            if (p.muhasebeKodu > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", p.muhasebeKodu));
                if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) == 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", ""));
                    kosul = VeriErisim.KosulEkle(kosul, " OR ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", "0") + ")");
                }
                else if (OrtakFonksiyonlar.ConvertToInt(p.birimKodu, 0) != -1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKODU], "=", p.birimKodu));
                }
            }
            else if (p.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGANIZASYONKOD], "=", p.organizasyonKodu));
            if (pk.daireKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADMEMURIYET, ALANLARMEMURIYET[(int)ENUMAlanlarMemuriyet.DAIREKODU], "=", pk.daireKodu));
            if (!string.IsNullOrEmpty(p.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tcKimlikNo));
            // kosul += " AND TCKIMLIKNO='" + pk.tcKimlikNo + "'";
            if (kosul != "")
                sql += " WHERE " + kosul;

            //sql += " ORDER BY " + "" + TABLOADKURS + ".BASLAMATARIHI, PERKURS.AD, PERUNVAN.KOD, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD] + " " + VeriErisim.IslecBul("+") + " " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SOYAD];
            IDataReader r = null;
            if (kosul != "")
                r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader ListeleEgitimTurleriRecursive(int kod)
        {
            string sql = @"WITH EGITIMTURLERICTE(KOD, AD,ATAKOD,SEVIYE) 
AS 
( 
   SELECT KOD, AD,ATAKOD,SEVIYE FROM PEREGITIMTURLERI 
   WHERE KOD= " + kod;
            sql += @" UNION ALL 
   SELECT C.KOD, C.AD, C.ATAKOD,C.SEVIYE 
   FROM PEREGITIMTURLERI C JOIN EGITIMTURLERICTE as CTE ON C.KOD = CTE.ATAKOD 
) 
SELECT * FROM EGITIMTURLERICTE order by ATAKOD";

            return veriErisim.DataReaderDondur(sql);
        }


        public bool EgitimTanimKontrol(PersonelEgitimTurleri p)
        {
            string sqlIlk = "";
            string sqlSon = "";
            int altKayit = 0;
            int kursKayit = 0;

            sqlIlk = "SELECT count (*) FROM PEREGITIMTURLERI WHERE PEREGITIMTURLERI.ATAKOD=" + p.kod;
            altKayit = veriErisim.TekKolonSQLInt(sqlIlk);

            sqlSon = "SELECT count (*) FROM PERPERSONELKURS WHERE PERPERSONELKURS.KURSKODU=" + p.kod;
            kursKayit = veriErisim.TekKolonSQLInt(sqlSon);

            if (altKayit == 0 && kursKayit == 0)
                return true;
            else
                return false;
        }
    }
}

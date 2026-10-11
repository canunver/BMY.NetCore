using System.Data;
using TNS.Veri;
using System;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak bilgilerini içeren EVREVRAK, EVREVRAKSICIL ve EVREVRAKILISKI veri tabanı tabloları ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABEvrak : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrak sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrak(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Evrak bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAK";

        /// <summary>
        /// Evrak bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            EVREVRAKNO = 0,
            YIL = 1,
            EVRAKTIPI = 2,
            EVRAKBIRIMKODU = 3,
            EVRAKNO = 4,
            EVRAKTARIH = 5,
            GELDIGIYERTUR = 6,
            GELDIGIYER = 7,
            GELENEVRAKNO = 8,
            GELENEVRAKTARIH = 9,
            KONU = 10,
            GIZLILIK = 11,
            GELISSEKLI = 12,
            YAZIDURUM = 13,
            CEVAPSURESI = 14,
            ACIKLAMA = 15,
            EKADEDI = 16,
            HAVALEBIRIMKODU = 17,
            EVRAKTURU = 18,
            PARAF = 19,
            ANTET = 20,
            ICERIK = 21,
            IMZA1 = 22,
            IMZA2 = 23,
            IMZA3 = 24,
            EK = 25,
            ALTBILGI = 26,
            OLUSTURMATARIHI = 27,
            IMZATARIHI = 28,
            EKYAZIALAN = 29,
            SIRANO = 30,
            DOSYANO = 31,
            EVRAKICERIK = 32,
            EVRAKCINSI = 33,
            ESASNO = 34,
            EVRAKBELGETUR = 35,
            IMZAYOLU = 36,
            BILGIEDINME = 37,
            KISISELBILGI = 38,
            DOSYASIVAR = 39,
            BAKANLIKEVRAKNO = 40,
            BASKANLIKEVRAKNO = 41,
            DAGITIMTARIH = 42,
            DINAMIKSAYFAKOD = 43,
            DINAMIKSURECNO = 44,
            SURECAD = 45,
            KAYITISLEM = 46,
            RESMIBELGE = 47,
            DIL = 48,
            EIMZALANDI = 49,
            EIMZATARIHI = 50,
            EIMZALAYAN = 51,
            URETICI = 52,
            URETIMTARIHI = 53,
            IMZABITTI = 54,
            URETIMVERSIYON = 55,
            GIZLILIKGECERLILIKSURE = 56,
            ILETIMTARIHI = 57,
            ARSIVLEMETARIHI = 58,
            TRANSFERTARIHI = 59,
            IMHATARIHI = 60,
            ILGILERIEKTEGOSTER = 61,
            KEPGONDERIMTARIH = 62,
            EDITORGUNCELLENMELI = 63,
            ASILNUSHAGIDECEK = 64
        }

        /// <summary>
        /// Evrak bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = {  "EVREVRAKNO", "YIL", "EVRAKTIPI", "EVRAKBIRIMKODU", "EVRAKNO", "EVRAKTARIH", "GELDIGIYERTUR", "GELDIGIYER", "GELENEVRAKNO", "GELENEVRAKTARIH", "KONU", "GIZLILIK", "GELISSEKLI", "YAZIDURUM", "CEVAPSURESI", "ACIKLAMA", "EKADEDI", "HAVALEBIRIMKODU",
                                            "EVRAKTURU","PARAF","ANTET","ICERIK","IMZA1","IMZA2","IMZA3","EK","ALTBILGI",
                                            "OLUSTURMATARIHI","IMZATARIHI","EKYAZIALAN","SIRANO", "DOSYANO", "EVRAKICERIK", "EVRAKCINSI", "ESASNO","EVRAKBELGETUR","IMZAYOLU","BILGIEDINME","KISISELBILGI","DOSYASIVAR","BAKANLIKEVRAKNO", "BASKANLIKEVRAKNO","DAGITIMTARIH",
                                            "DINAMIKSAYFAKOD","DINAMIKSURECNO", "SURECAD",
                                            "KAYITISLEM","RESMIBELGE","DIL","EIMZALANDI","EIMZATARIHI","EIMZALAYAN","URETICI","URETIMTARIHI", "IMZABITTI", "URETIMVERSIYON", "GIZLILIKGECERLILIKSURE",
                                            "ILETIMTARIHI","ARSIVLEMETARIHI","TRANSFERTARIHI","IMHATARIHI","ILGILERIEKTEGOSTER", "KEPGONDERIMTARIH", "EDITORGUNCELLENMELI", "ASILNUSHAGIDECEK"};

        /// <summary>
        /// Evrak sicil bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADSICIL = "EVREVRAKSICIL";

        /// <summary>
        /// Evrak sicil bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarSicil
        {
            EVREVRAKNO = 0,
            PREVRAKNO = 1,
            ASIL = 2,
        }

        /// <summary>
        /// Evrak sicil bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLARSICIL = { "EVREVRAKNO", "PREVRAKNO", "ASIL" };

        /// <summary>
        /// Evrak ilişki bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADILISKI = "EVREVRAKILISKI";

        /// <summary>
        /// Evrak ilişki bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarIliski
        {
            PREVRAKNO = 0,
            PREVRAKNOILISKI = 1,
            TIP = 2,
        }

        /// <summary>
        /// Evrak ilişki bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLARILISKI = { "PREVRAKNO", "PREVRAKNOILISKI", "TIP" };

        /// <summary>
        /// Evrak etiket tanım bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADETIKETTANIM = "EVRETIKET";

        /// <summary>
        /// Evrak etiket bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADETIKET = "EVREVRAKETIKET";

        /// <summary>
        /// Evrak etiket tanım bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanEtiketTanim
        {
            KOD = 0,
            AD = 1,
            USTKOD = 2,
            SAHIBI = 3,
            DETAY = 4,
        }

        /// <summary>
        /// Evrak etiket  bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanEtiket
        {
            ETIKETKOD = 0,
            PREVRAKNO = 1,
        }

        /// <summary>
        /// Evrak etiket tanım bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLARETIKETTANIM = { "KOD", "AD", "USTKOD", "SAHIBI", "DETAY" };

        /// <summary>
        /// Evrak etiket  bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLARETIKET = { "ETIKETKOD", "PREVRAKNO" };

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre; evrak bilgisini yeni kayıt olarak, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc Kaydet(EvrakSinif form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            //Gelen evrak kısmı riskli olduğu için burda kayıt var mı yok mu kontrolü yaptım.--harun--
            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo);

            if (veriErisim.TekKolonSQLInt(sql) <= 0)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.yil) + ",";
                sql += VeriErisim.DegerYap(form.evrakTipi) + ",";
                sql += VeriErisim.DegerYap(form.evrakBirimKodu) + ",";
                sql += VeriErisim.DegerYap(form.evrakNo) + ",";
                sql += VeriErisim.DegerYap(form.evrakTarih) + ",";
                sql += VeriErisim.DegerYap(form.geldigiYerTur) + ",";
                sql += VeriErisim.DegerYap(form.geldigiYer) + ",";
                sql += VeriErisim.DegerYap(form.gelenEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.gelenEvrakTarih) + ",";
                sql += VeriErisim.DegerYap(form.konu) + ",";
                sql += VeriErisim.DegerYap(form.gizlilik) + ",";
                sql += VeriErisim.DegerYap(form.gelisSekli) + ",";
                sql += VeriErisim.DegerYap(form.yaziDurum) + ",";
                sql += VeriErisim.DegerYap(form.cevapSuresi) + ",";
                sql += VeriErisim.DegerYap(form.aciklama) + ",";
                sql += VeriErisim.DegerYap(form.ekAdedi) + ",";
                sql += VeriErisim.DegerYap(form.havaleBirimKodu) + ",";
                sql += VeriErisim.DegerYap(form.evrakTuru) + ",";
                sql += VeriErisim.DegerYap(form.paraf) + ",";
                sql += VeriErisim.DegerYap(form.antet) + ",";
                sql += VeriErisim.DegerYap(form.icerik) + ",";
                sql += VeriErisim.DegerYap(form.imza1) + ",";
                sql += VeriErisim.DegerYap(form.imza2) + ",";
                sql += VeriErisim.DegerYap(form.imza3) + ",";
                sql += VeriErisim.DegerYap(form.ek) + ",";
                sql += VeriErisim.DegerYap(form.altBilgi) + ",";
                sql += VeriErisim.DegerYap(form.evrakOlusturmaTarihi) + ",";
                sql += VeriErisim.DegerYap(form.imzaTarihi) + ",";
                sql += VeriErisim.DegerYap(form.ekYaziAlan) + ",";
                sql += VeriErisim.DegerYap(form.siraNo) + ",";
                sql += VeriErisim.DegerYap(form.dosyaNo) + ",";
                sql += VeriErisim.DegerYap(form.evrakIcerigi) + ",";
                sql += VeriErisim.DegerYap(form.evrakCinsi) + ",";
                sql += VeriErisim.DegerYap(form.esasNo) + ",";
                sql += VeriErisim.DegerYap(form.evrakBelgeTur) + ",";
                sql += VeriErisim.DegerYap(form.imzaYolu) + ",";
                sql += VeriErisim.DegerYap(form.bilgiEdinme) + ",";
                sql += VeriErisim.DegerYap(form.kisiselBilgi) + ",";
                sql += VeriErisim.DegerYap(form.dosyasiVar) + ",";
                sql += VeriErisim.DegerYap(form.bakanlikEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.baskanlikEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.dagitimTarih) + ",";
                sql += VeriErisim.DegerYap(form.dinamikSayfaKod) + ",";
                sql += VeriErisim.DegerYap(form.dinamikSurecNo) + ",";
                sql += VeriErisim.DegerYap(form.surecAd) + ",";
                sql += VeriErisim.DegerYap(form.kayitIslem) + ",";
                sql += VeriErisim.DegerYap(form.resmiBelge) + ",";
                sql += VeriErisim.DegerYap(form.dil) + ",";
                sql += VeriErisim.DegerYap(form.eImzalandi) + ",";
                sql += VeriErisim.DegerYap(form.eImzaTarihi) + ",";
                sql += VeriErisim.DegerYap(form.eImzalayan) + ",";
                sql += VeriErisim.DegerYap(form.uretici) + ",";
                sql += VeriErisim.DegerYap(form.uretimTarihi) + ",";
                sql += VeriErisim.DegerYap(form.imzaBitti) + ",";
                sql += VeriErisim.DegerYap(form.uretimVersiyon) + ",";
                sql += VeriErisim.DegerYap(form.gizlilikGecerlilikSuresi) + ",";
                sql += VeriErisim.DegerYap(form.iletimTarihi) + ",";
                sql += VeriErisim.DegerYap(form.arsivlemeTarihi) + ",";
                sql += VeriErisim.DegerYap(form.transferTarihi) + ",";
                sql += VeriErisim.DegerYap(form.imhaTarihi) + ",";
                sql += VeriErisim.DegerYap(form.ilgileriEkteGoster) + ",";
                sql += VeriErisim.DegerYap(form.kepGonderimTarih) + ",";
                sql += VeriErisim.DegerYap(form.editorGuncellenmeli) + ",";
                sql += VeriErisim.DegerYap(form.asilNushaGidecek) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            else
            {
                veriErisim.hata = "";
                sonuc = Guncelle(form);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSinif nesnesine göre; kayıtlı olan evrak bilgisinin verilen son halini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Guncelle(EvrakSinif form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "UPDATE " + TABLOAD + " SET ";
            string sqlTemp = "";

            if (form.evrakNo > 0)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKNO], form.evrakNo) + ",";
            if (form.geldigiYerTur > 0)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELDIGIYERTUR], form.geldigiYerTur) + ",";
            if (!string.IsNullOrEmpty(form.geldigiYer))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELDIGIYER], form.geldigiYer) + ",";
            if (!form.gelenEvrakTarih.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELENEVRAKTARIH], form.gelenEvrakTarih) + ",";
            if (!form.evrakTarih.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKTARIH], form.evrakTarih) + ",";
            if (!string.IsNullOrEmpty(form.gelenEvrakNo))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELENEVRAKNO], form.gelenEvrakNo) + ",";
            if (form.gizlilik > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIZLILIK], form.gizlilik) + ",";
            if (form.gelisSekli > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GELISSEKLI], form.gelisSekli) + ",";
            if (form.yaziDurum > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAZIDURUM], form.yaziDurum) + ",";
            if (!string.IsNullOrEmpty(form.konu))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONU], form.konu) + ",";
            if (form.aciklama != "")
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], form.aciklama) + ",";
            if (form.ekAdedi != "")
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKADEDI], form.ekAdedi.Trim()) + ",";
            if (!string.IsNullOrEmpty(form.havaleBirimKodu))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAVALEBIRIMKODU], form.havaleBirimKodu) + ",";
            if (form.cevapSuresi > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEVAPSURESI], form.cevapSuresi) + ",";
            if (form.evrakTipi > 0)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKTIPI], form.evrakTipi) + ",";
            if (!string.IsNullOrEmpty(form.paraf) || form.imzaYoluDegisti)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARAF], form.paraf) + ",";
            if (form.evrakTuru > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKTURU], form.evrakTuru) + ",";
            if (!string.IsNullOrEmpty(form.antet))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANTET], form.antet) + ",";
            if (!string.IsNullOrEmpty(form.icerik))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICERIK], form.icerik) + ",";
            if (!string.IsNullOrEmpty(form.imza1) || form.imzaYoluDegisti)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZA1], form.imza1) + ",";
            if (!string.IsNullOrEmpty(form.imza2) || form.imzaYoluDegisti)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZA2], form.imza2) + ",";
            if (!string.IsNullOrEmpty(form.imza3) || form.imzaYoluDegisti)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZA3], form.imza3) + ",";
            if (form.ek != "")
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EK], form.ek.Trim()) + ",";
            if (!string.IsNullOrEmpty(form.altBilgi))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTBILGI], form.altBilgi) + ",";
            if (form.imzaTarihi == null || !form.imzaTarihi.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZATARIHI], form.imzaTarihi) + ",";
            if (!string.IsNullOrEmpty(form.ekYaziAlan))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKYAZIALAN], form.ekYaziAlan) + ",";
            if (form.siraNo > 0)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], form.siraNo) + ",";
            if (form.yil > 0)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], form.yil) + ",";
            if (!string.IsNullOrEmpty(form.dosyaNo))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], form.dosyaNo.Replace("|", "")) + ",";
            if (!string.IsNullOrEmpty(form.evrakIcerigi))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKICERIK], form.evrakIcerigi) + ",";
            if (form.evrakCinsi > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKCINSI], form.evrakCinsi) + ",";
            if (!string.IsNullOrEmpty(form.esasNo))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ESASNO], form.esasNo) + ",";
            if (form.evrakBelgeTur > 0)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBELGETUR], form.evrakBelgeTur) + ",";
            if (form.imzaYolu > 0 || form.imzaYoluDegisti)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZAYOLU], form.imzaYolu) + ",";
            if (form.bilgiEdinme > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BILGIEDINME], form.bilgiEdinme) + ",";
            if (form.kisiselBilgi > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISISELBILGI], form.kisiselBilgi) + ",";
            if (form.dosyasiVar > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYASIVAR], form.dosyasiVar) + ",";
            if (!string.IsNullOrEmpty(form.bakanlikEvrakNo))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BAKANLIKEVRAKNO], form.bakanlikEvrakNo) + ",";
            if (!string.IsNullOrEmpty(form.baskanlikEvrakNo))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASKANLIKEVRAKNO], form.baskanlikEvrakNo) + ",";
            if (!string.IsNullOrEmpty(form.evrakBirimKodu))
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], form.evrakBirimKodu) + ",";
            if (!form.dagitimTarih.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAGITIMTARIH], form.dagitimTarih) + ",";
            if (form.dinamikSayfaKod != "")
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DINAMIKSAYFAKOD], form.dinamikSayfaKod) + ",";
            if (form.dinamikSurecNo != "")
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DINAMIKSURECNO], form.dinamikSurecNo) + ",";
            if (form.surecAd != "")
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURECAD], form.surecAd) + ",";
            if (form.kayitIslem > 0)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITISLEM], form.kayitIslem) + ",";
            if (form.resmiBelge > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RESMIBELGE], form.resmiBelge) + ",";
            if (form.dil > 0)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIL], form.dil) + ",";
            if (form.eImzalandi > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EIMZALANDI], form.eImzalandi) + ",";
            if (form.eImzaTarihi == null || !form.eImzaTarihi.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EIMZATARIHI], form.eImzaTarihi) + ",";
            if (form.eImzalayan != "")
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EIMZALAYAN], form.eImzalayan) + ",";
            if (form.imzaBitti > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZABITTI], form.imzaBitti) + ",";
            if (form.uretimVersiyon != "")
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.URETIMVERSIYON], form.uretimVersiyon) + ",";
            if (!form.gizlilikGecerlilikSuresi.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIZLILIKGECERLILIKSURE], form.gizlilikGecerlilikSuresi) + ",";
            if (!form.iletimTarihi.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILETIMTARIHI], form.iletimTarihi) + ",";
            if (!form.arsivlemeTarihi.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARSIVLEMETARIHI], form.arsivlemeTarihi) + ",";
            if (form.transferTarihi == null || !form.transferTarihi.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TRANSFERTARIHI], form.transferTarihi) + ",";
            if (form.imhaTarihi == null || !form.imhaTarihi.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMHATARIHI], form.imhaTarihi) + ",";
            if (form.ilgileriEkteGoster > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILGILERIEKTEGOSTER], form.ilgileriEkteGoster) + ",";
            if (!form.kepGonderimTarih.isNull)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KEPGONDERIMTARIH], form.kepGonderimTarih) + ",";
            if (form.editorGuncellenmeli > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EDITORGUNCELLENMELI], form.editorGuncellenmeli) + ",";
            if (form.asilNushaGidecek > -1)
                sqlTemp += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASILNUSHAGIDECEK], form.asilNushaGidecek) + ",";

            sqlTemp = sqlTemp.Substring(0, sqlTemp.Length - 1);//Sonda kalan virgülü atmak için

            if (string.IsNullOrEmpty(sqlTemp))
            {
                sonuc.bilgiStr = "Güncelleme yapacak bilgi girmemişsiniz.";
                sonuc.islemSonuc = true;
                return sonuc;
            }

            sqlTemp += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo);
            sql += sqlTemp;

            //if (form.evrakNo > 0)
            //    OrtakFonksiyonlar.HataStrYaz("evrak no yazılıyor" + sql);

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                if (form.evrakNo > 0)
                    OrtakFonksiyonlar.HataStrYaz("evrak no yazma hatalı" + sonuc.hataStr);
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen EvrakSinif nesnesine göre; evrak bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(EvrakSinif form, bool sadecePrEvrakNo = false, int start = 0, int limit = 0, bool sadeceSatirSayisi = false)
        {
            form.siralamaKolonu = SiralamaKolonuBul(form.siralamaKolonu);

            string hareketTablo = "";
            string tasfiyeTablo = "";

            if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.YENI || form.hareket.islem == (int)ENUMEvrakTarihceDurum.HAVALEGELEN || form.hareket.islem == (int)ENUMEvrakTarihceDurum.BUTUNBELGELER || form.hareket.islem == (int)ENUMEvrakTarihceDurum.IMZAYAGELENLER)
                hareketTablo = TABGenel.VIEWHAREKETSONDURUM;//Evrak havale edildikten sonra bilgilerinde değişiklik yapılırsa bile izi olan birimlerde listelenmeli
            else if (form.aciklama != "zimmet" && (form.hareket.islem == (int)ENUMEvrakTarihceDurum.HAVALEEDILDI || form.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN || form.hareket.islem == (int)ENUMEvrakHareketDurum.IMZAYAGONDER) || form.hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLER || form.hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLERVEMIADLIISLER)
                hareketTablo = "EVREVRAKHAREKET"; //geri alınan evraklar halen havale edildi olarak gösterildiği için kaldırıldı EVREVRAKHAREKET tablosuna bakmamamalı ama TABGenel.VIEWHAREKETSONDURUMBIRIM tablosunada bakıldığında havale edilen evraklar birim içinde işlem gördükçe kişinin havale ettiği evraklar listesinden çıkıyor
            else if (form.evrEvrakNo != "" || form.prEvrakNo != "" || form.evrEvrakNoListe != "" || form.prEvrakNoListe != "")
                hareketTablo = TABGenel.VIEWHAREKETSONDURUM;
            else if (form.islem == ENUMEvrakSinifIslem.EVRAKTASIMA || form.islem == ENUMEvrakSinifIslem.KEPGONDER)
                hareketTablo = TABGenel.VIEWHAREKETSONDURUM;
            else
                hareketTablo = TABGenel.VIEWHAREKETSONDURUMBIRIM;

            if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.BUTUNBELGELER && form.aciklama == "*") // Personel'e gelen tüm evrakları listelemek için "EVREVRAKHAREKET" tablosuna bakılması gerekiyor. Geçici olarak eklendi. 23.09.2011 - Hüseyin (Not: Kullanılan yerlere [Anlam:1002] yazıldı)
                hareketTablo = "EVREVRAKHAREKET";

            string ekleFrom = "";

            string sql = "SELECT ";
            if (!sadecePrEvrakNo)
            {
                sql += VeriErisim.TumAlanlar(TABLOAD, ALANLAR);

                sql += ",EVRYAZIDURUMU.AD, EVRGIZLILIK.AD, EVRGELISSEKLI.AD";
                //Performans için JOIN kısmı çıkartıldı 21.08.2014 Melih (Tekrar açıldı Raporlamada ihtiyaç var)
                //sql += ",'' AS EVRYAZIDURUMUAD, '' AS EVRGIZLILIKAD, '' AS EVRGELISSEKLIAD";


                sql += "," + hareketTablo + ".SIRANO, " + hareketTablo + ".BIRIMKOD1, " + hareketTablo + ".BIRIMKOD2, " + hareketTablo + ".KISI1, " + hareketTablo + ".KISI2, " + hareketTablo + ".ISLEMYAPAN, " + hareketTablo + ".ISLEMTARIH, " + hareketTablo + ".ISLEM, " + hareketTablo + ".KISI1AD, " + hareketTablo + ".KISI2AD, " + hareketTablo + ".ISLEMYAPANAD, " + hareketTablo + ".OKUNDU, " + hareketTablo + ".HAVALENOTU ";


                sql += ",EVRBKD1.AD AS BIRIMAD1,EVRBKD2.AD AS BIRIMAD2 ,EVRBKD0.AD AS BIRIMAD3";
                //Performans için JOIN kısmı çıkartıldı 21.08.2014 Melih (Tekrar açıldı Raporlamada ihtiyaç var)
                //sql += ", '' AS BIRIMAD1, '' AS BIRIMAD2 , '' AS BIRIMAD3";


                sql += ",TS1.PREVRAKNO, TS1.ASIL, DOSYAPLANKODLARI";

                ekleFrom = "CROSS APPLY (Select TDP.DOSYAPLANKODU + ';' From EVREVRAKDOSYAPLANI TDP Where TDP.PREVRAKNO = TS1.PREVRAKNO For XML PATH ('')) dd(DOSYAPLANKODLARI)";

                if (form.tasfiye.tasfiyeKod != 5 && form.tasfiye.tasfiyeKod != -1)
                    sql += ",EVRKTSFIY.TASFIYEKOD";
                else
                    sql += ",0 as 'TASFIYEKOD'";

                sql += ",(SELECT COUNT(EVREVRAKNO) FROM EVREVRAKSICIL WHERE EVREVRAKNO=EVREVRAK.EVREVRAKNO) AS EVRAKSAYISI";
            }
            else
                sql += "DISTINCT TS1.PREVRAKNO";

            if (sadeceSatirSayisi)
                sql = "SELECT COUNT(1) ";


            sql += " FROM ";

            sql += TABLOAD;
            sql += " INNER JOIN EVREVRAKSICIL TS1 ON EVREVRAK.EVREVRAKNO = TS1.EVREVRAKNO ";
            sql += " INNER JOIN " + hareketTablo + " ON TS1.PREVRAKNO = " + hareketTablo + ".PREVRAKNO ";

            if (form.hareket.islem == (int)ENUMEvrakHareketDurum.IMZAYAGONDER || form.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN)
                sql += " INNER JOIN EVREVRAKHAREKETSONDURUM ON EVREVRAKHAREKETSONDURUM.PREVRAKNO = TS1.PREVRAKNO AND EVREVRAKHAREKETSONDURUM.ISLEM = EVREVRAKHAREKET.ISLEM ";

            if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLER || form.hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLERVEMIADLIISLER)
                sql += " INNER JOIN EVREVRAKHAREKETSONDURUM ON TS1.PREVRAKNO = EVREVRAKHAREKETSONDURUM.PREVRAKNO ";

            //Raporlamada bu tablolara ihtiyaç var.
            sql += " LEFT OUTER JOIN EVRYAZIDURUMU ON EVREVRAK.YAZIDURUM = EVRYAZIDURUMU.KOD ";
            sql += " LEFT OUTER JOIN EVRGIZLILIK ON EVREVRAK.GIZLILIK = EVRGIZLILIK.KOD ";
            sql += " LEFT OUTER JOIN EVRGELISSEKLI ON EVREVRAK.GELISSEKLI = EVRGELISSEKLI.KOD ";
            sql += "  LEFT OUTER  JOIN EVRBIRIM EVRBKD0 ON EVREVRAK.HAVALEBIRIMKODU = EVRBKD0.KOD ";
            sql += "  LEFT OUTER  JOIN EVRBIRIM EVRBKD2 ON " + hareketTablo + ".BIRIMKOD2 = EVRBKD2.KOD ";
            sql += "  LEFT OUTER  JOIN EVRBIRIM EVRBKD1 ON " + hareketTablo + ".BIRIMKOD1 = EVRBKD1.KOD ";
            if (form.tasfiye.tasfiyeKod != 5 && form.tasfiye.tasfiyeKod != -1)
                sql += " LEFT OUTER JOIN EVREVRAKTASFIYE EVRKTSFIY  ON EVRKTSFIY.PREVRAKNO=TS1.PREVRAKNO ";

            sql += ekleFrom;

            string kosul = "";
            string ekKosul = "";

            if (form.evrEvrakNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));
            if (form.prEvrakNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TS1", ALANLARSICIL[(int)ENUMAlanlarSicil.PREVRAKNO], "=", form.prEvrakNo));

            if (!string.IsNullOrEmpty(form.bulunduguYer) && (form.evrEvrakNo != "" || form.prEvrakNo != "") && (form.hareket.islem != (int)ENUMEvrakTarihceDurum.HAVALEGELEN))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD1", "LIKE", form.bulunduguYer));

            if (!form.imzaTarihi.isNull)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IMZATARIHI], ">=", form.imzaTarihi));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IMZATARIHI], "<=", new TNSDateTime(form.imzaTarihi.Oku().AddDays(1))));
            }

            if (form.etiket.etiketKod > 0)
            {
                ekKosul = " (TS1.PREVRAKNO IN " +
               "(SELECT DISTINCT PREVRAKNO FROM EVREVRAKETIKET WHERE " +
               "(ETIKETKOD = " + VeriErisim.DegerYap(form.etiket.etiketKod) + "))) ";

                if (kosul != "") kosul += " AND ";
                kosul += " (" + ekKosul + ")";
            }

            if (form.evrEvrakNo == "" && form.prEvrakNo == "")
            {
                if (!string.IsNullOrEmpty(form.bulunduguYer))
                {
                    ekKosul = "";
                    if (form.hareket.kisi2 == "-500")   //Evrak yetkilisi olmayan üzerinden geçen bütün evrakları arayabilsin 19.03.2013
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD1", "LIKE", form.bulunduguYer));

                        if (form.hareket.kisi1 != "")
                        {
                            ekKosul = " (TS1.PREVRAKNO IN " +
                             "(SELECT DISTINCT PREVRAKNO FROM EVREVRAKHAREKET WHERE " +
                             "(KISI1 = " + VeriErisim.DegerYap(form.hareket.kisi1) + ") OR " +
                             "(KISI2 = " + VeriErisim.DegerYap(form.hareket.kisi1) + ") )) ";
                        }

                        string ekKosulDosya = "";
                        if (form.sorguKriterleri.ContainsKey("dosyaTasnifPlani"))
                        {
                            string[] tmpDosyaPlanKodlari = (form.sorguKriterleri["dosyaTasnifPlani"] + "").TrimEnd(';').Split(';');
                            foreach (var kod in tmpDosyaPlanKodlari)
                            {
                                if (kod.Trim() == "") continue;
                                if (ekKosulDosya != "") ekKosulDosya += " OR ";
                                ekKosulDosya += " DOSYAPLANKODLARI LIKE '" + kod + "%'";
                            }
                        }

                        //Gizlilik ile ilgili
                        string ilkDegerler = "";
                        string yetkiDegerler = "";
                        ObjectArray gizlilikDegerleri = (ObjectArray)form.sorguKriterleri["gizlilikDegerleri"];
                        //Gizlilik yetkisi verilmemiş ise sadece gizli olmayan dosyaları göstermek için
                        if (gizlilikDegerleri != null)
                        {
                            foreach (Tanimlama t in gizlilikDegerleri.objeler)
                            {
                                if (t.derece > 0) continue;
                                if (ilkDegerler != "") ilkDegerler += ",";
                                ilkDegerler += t.kod.ToString();
                            }
                        }

                        if (form.sorguKriterleri.ContainsKey("gizlilikDegeri"))
                        {
                            //Gizlilik yetkisi verilmiş ise verilen gizlilik kodunun derecesine göre dosyaları göstermek için
                            string[] tmpDosyaPlanKodlari = (form.sorguKriterleri["gizlilikDegeri"] + "").TrimEnd(';').Split(';');
                            foreach (var kod in tmpDosyaPlanKodlari)
                            {
                                int derece = 0;
                                foreach (Tanimlama t in gizlilikDegerleri.objeler)
                                {
                                    if (t.kod == OrtakFonksiyonlar.ConvertToInt(kod, 0))
                                    {
                                        if (derece > t.derece) continue;
                                        derece = t.derece;
                                    }
                                }
                                foreach (Tanimlama t in gizlilikDegerleri.objeler)
                                {
                                    if (derece >= t.derece)
                                    {
                                        if (yetkiDegerler != "") yetkiDegerler += ",";
                                        yetkiDegerler += t.kod;
                                    }
                                }
                            }
                        }
                        if (yetkiDegerler == "") yetkiDegerler = ilkDegerler;
                        string gizlilikKosul = "EVREVRAK.GIZLILIK IN (" + yetkiDegerler + ")";
                        //************************************************************

                        if (ekKosulDosya != "")
                        {
                            ekKosulDosya = "(" + ekKosulDosya + ")";

                            ekKosulDosya = VeriErisim.KosulEkle(ekKosulDosya, "AND", gizlilikKosul);

                            ekKosulDosya = VeriErisim.KosulEkle(ekKosulDosya, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR));
                            ekKosulDosya = "(" + ekKosulDosya + ")";

                            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", ekKosulDosya);
                        }

                        if (ekKosul != "")
                            kosul += " AND (" + ekKosul + ")";

                        form.evrakBirimKodu = "";
                    }
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.HAVALEGELEN)
                    {
                        if (form.bulunduguYer.IndexOf("%*") > -1) // Kullanıcının bütün bürimlerde teslim alınmayan evraklarını görmek için. Geçici olarak eklendi. 23.09.2011 - Hüseyin (Not: Kullanılan yerlere [Anlam:1001] yazıldı)
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD2", "LIKE", form.bulunduguYer.TrimEnd('*')));
                        else
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD2", "=", form.bulunduguYer));

                        ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI2", "=", form.hareket.kisi1));
                        if (form.hareket.kisi1 == "")//Evrak yetkisili olan kişinin adına gönderilen evraklar için
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(hareketTablo, "KISI2", "=", form.hareket.kisi1Ad));//Sorgulayan evrak yetkilisi ise kisi1Ad alanına ServisC.Listeleme metodunda mernis numarası konuluyor

                        kosul += " AND (" + ekKosul + ")";
                    }
                    else if (form.hareket.islem == (int)ENUMEvrakHareketDurum.IMZAYAGONDER || form.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN)
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD2", "=", form.bulunduguYer));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI2", "=", form.hareket.kisi2));
                    }
                    else if (form.islem == ENUMEvrakSinifIslem.EVRAKTASIMA)
                    {

                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD1", "=", form.bulunduguYer));

                        if (form.hareket.kisi2 != "")
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI1", "=", form.hareket.kisi2));
                        else
                        {
                            //kosul += " AND (EVREVRAKHAREKETSONDURUM.KISI1 <> '' AND NOT (EVREVRAKHAREKETSONDURUM.KISI1 IN ";
                            //kosul += "(SELECT Kullanici.dbo.KULLANICI.MERNIS FROM Kullanici.dbo.KULLANICI_YT INNER JOIN Kullanici.dbo.KULLANICI ON Kullanici.dbo.KULLANICI_YT.KULLANICIKODU = Kullanici.dbo.KULLANICI.KULLANICIKODU ";
                            //kosul += "WHERE (Kullanici.dbo.KULLANICI_YT.YTKOD = 52) AND (Kullanici.dbo.KULLANICI_YT.DEGER LIKE '%" + form.bulunduguYer + "%') ";
                            //kosul += " OR (Kullanici.dbo.KULLANICI_YT.YTKOD = 80) AND (Kullanici.dbo.KULLANICI_YT.DEGER LIKE '%" + form.bulunduguYer + "%') ) ))";
                        }
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "<", 50));


                        ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD2", "=", form.bulunduguYer));

                        if (form.hareket.kisi2 != "")
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI2", "=", form.hareket.kisi2));
                        else
                        {
                            //ekKosul += " AND (EVREVRAKHAREKETSONDURUM.KISI2 <> '' AND NOT (EVREVRAKHAREKETSONDURUM.KISI2 IN ";
                            //ekKosul += "(SELECT Kullanici.dbo.KULLANICI.MERNIS FROM Kullanici.dbo.KULLANICI_YT INNER JOIN Kullanici.dbo.KULLANICI ON Kullanici.dbo.KULLANICI_YT.KULLANICIKODU = Kullanici.dbo.KULLANICI.KULLANICIKODU ";
                            //ekKosul += "WHERE (Kullanici.dbo.KULLANICI_YT.YTKOD = 52) AND (Kullanici.dbo.KULLANICI_YT.DEGER LIKE '%" + form.bulunduguYer + "%') ";
                            //ekKosul += " OR (Kullanici.dbo.KULLANICI_YT.YTKOD = 80) AND (Kullanici.dbo.KULLANICI_YT.DEGER LIKE '%" + form.bulunduguYer + "%') ) ))";
                        }
                        ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", 51));

                        kosul += " OR (" + ekKosul + ")";

                    }
                    else if (form.islem == ENUMEvrakSinifIslem.KEPGONDER)
                    {
                        form.evrakTipi = (int)ENUMEvrakTipi.GIDEN;
                        form.sicil.asil = 1;

                        sql += " LEFT OUTER JOIN EVRKEPEVRAK ON EVREVRAK.EVREVRAKNO = EVRKEPEVRAK.EVREVRAKNO ";

                        if (kosul != "")
                            kosul += "AND";
                        kosul += " (EVRKEPEVRAK.KOD IS NULL)";
                    }
                    else
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD1", "LIKE", form.bulunduguYer));

                        ekKosul = "";
                        if ((form.hareket.islem != (int)ENUMEvrakTarihceDurum.DOSYAYAKALDIR && form.hareket.islem != (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR) ||
                            form.hareket.kisi1 == "")
                        {
                            if (form.hareket.kisi2 == "-200")//Evrak yetkilisi olmayan sadece kendi evrakını görsün. 23.06.2011
                            {
                                form.hareket.kisi2 = "";
                                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI1", "=", form.hareket.kisi1));
                                kosul += " AND (" + ekKosul + ")";
                            }
                            else if (form.hareket.kisi1 != "-100" && form.hareket.kisi1 != "-300")//-100 gelmiş ise sorgulanmasın
                            {
                                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI1", "=", form.hareket.kisi1));
                                if (form.hareket.kisi1 == "")
                                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(hareketTablo, "KISI1", "=", form.hareket.kisi1Ad));

                                //Evrak Yetkilisi yetkisine ait bir kişi evrak kullanıcı olduğunda önceden işlem yaptıklarında kisi1 veya kisi2 alanları boş bırakılıyordu ve eski kayıtlar gösterilemiyordu. 25.05.2014 Melih (Teftiş)
                                if (form.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDI)
                                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(hareketTablo, "ISLEMYAPAN", "=", form.hareket.kisi1));

                                kosul += " AND (" + ekKosul + ")";
                            }
                            else if (form.hareket.kisi1 == "-300")
                            {
                                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI1", "=", ""));
                                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(hareketTablo, "KISI1", "=", form.hareket.kisi1Ad));
                                kosul += " AND (" + ekKosul + ")";
                            }
                        }
                        else if (form.hareket.kisi2 == "-200")//Evrak yetkilisi olmayan sadece kendi evrakını görsün. 23.06.2011
                        {
                            form.hareket.kisi2 = "";
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI1", "=", form.hareket.kisi1));
                            kosul += " AND (" + ekKosul + ")";
                        }

                        if (form.hareket.birimKod2 != "")
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "BIRIMKOD2", "=", form.hareket.birimKod2));
                        if (form.hareket.kisi2 != "")
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI2", "=", form.hareket.kisi2));
                    }

                    //Evrak yettkilisi değilse kolntrolü
                    //if (form.hareket.kisi2 != "-500" && form.hareket.kisi1 != "" && form.hareket.kisi1.Length == 11)
                    //{
                    //    ekKosul = "TS1.PREVRAKNO IN ";
                    //    ekKosul += "(";
                    //    ekKosul += "SELECT DISTINCT PREVRAKNO FROM EVREVRAKHAREKET WHERE ";
                    //    ekKosul += "(ERISEMEZ IS NULL OR ERISEMEZ = 0) AND ";
                    //    ekKosul += "(( (ISLEMYAPAN = " + VeriErisim.DegerYap(form.hareket.kisi1) + ") AND (ISLEM <> 51) ) OR ( (KISI2 = " + VeriErisim.DegerYap(form.hareket.kisi1) + ") AND (ISLEM = 51) )) ";
                    //    ekKosul += ")";

                    //    kosul += " AND " + ekKosul;
                    //}
                }
                else
                {
                    //Giden evrak listeleme yapılıyor ise sadece kendi evraklarını görsün. Bulunduguyer bilgisi boş geliyor (Web service-word add-in için yapıldı)
                    if (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN && form.hareket.kisi1 != "" && form.hareket.kisi2 == "-999")
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "KISI1", "=", form.hareket.kisi1));
                }

                if (form.prEvrakNoListe != "")
                {
                    string prEvrakNoKosul = "";
                    string[] nolar = form.prEvrakNoListe.Split(';');
                    for (int i = 0; i < nolar.Length; i++)
                    {
                        if (nolar[i].Trim() != "")
                            prEvrakNoKosul = VeriErisim.KosulEkle(prEvrakNoKosul, "OR", VeriErisim.KosulOl("TS1", ALANLARSICIL[(int)ENUMAlanlarSicil.PREVRAKNO], "=", nolar[i].Trim()));
                    }
                    if (prEvrakNoKosul != "")
                    {
                        if (kosul != "") kosul += " AND ";
                        kosul += "(" + prEvrakNoKosul + ")";
                    }
                }

                if (form.evrEvrakNoListe != "")
                {
                    string evrEvrakNoKosul = "";
                    string[] nolar = form.evrEvrakNoListe.Split(';');
                    for (int i = 0; i < nolar.Length; i++)
                    {
                        if (nolar[i].Trim() != "")
                            evrEvrakNoKosul = VeriErisim.KosulEkle(evrEvrakNoKosul, "OR", VeriErisim.KosulOl("TS1", ALANLARSICIL[(int)ENUMAlanlarSicil.EVREVRAKNO], "=", nolar[i].Trim()));
                    }
                    if (evrEvrakNoKosul != "")
                    {
                        if (kosul != "") kosul += " AND ";
                        kosul += "(" + evrEvrakNoKosul + ")";
                    }
                }

                if (form.yil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                if (form.evrakNo > 0 || form.evrakNo == -100)//-100: evrakKodu>0 işlemi için
                {
                    if (form.sorguEvrakNo > 0)
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKNO], ">=", form.evrakNo));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKNO], "<=", form.sorguEvrakNo));
                    }
                    else
                        if (form.evrakNo == -100)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKNO], ">", 0));
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKNO], "=", form.evrakNo));
                }
                if (!form.evrakTarih.isNull)
                {
                    if (form.sorguEvrakTarih.isNull)
                        form.sorguEvrakTarih = new TNSDateTime(form.evrakTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli

                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKTARIH], ">=", form.evrakTarih));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKTARIH], "<=", new TNSDateTime(form.sorguEvrakTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
                }
                if (!string.IsNullOrEmpty(form.sorguIcerik))
                {
                    if (!string.IsNullOrEmpty(kosul))
                    {
                        form.sorguIcerik = "(" + form.sorguIcerik + ")";
                        kosul = VeriErisim.KosulEkle(kosul, "AND ", form.sorguIcerik);
                    }

                    else
                        kosul += "(" + form.sorguIcerik + ")";

                }

                if (!string.IsNullOrEmpty(form.havaleBirimKodu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HAVALEBIRIMKODU], "=", form.havaleBirimKodu));
                if (!string.IsNullOrEmpty(form.gelenEvrakNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GELENEVRAKNO], "=", form.gelenEvrakNo));
                if (!form.gelenEvrakTarih.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GELENEVRAKTARIH], "=", form.gelenEvrakTarih));
                if (!string.IsNullOrEmpty(form.evrakBirimKodu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "LIKE", form.evrakBirimKodu));
                if (!string.IsNullOrEmpty(form.geldigiYer) && form.geldigiYer.Length > 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GELDIGIYER], "LIKE", form.geldigiYer + "%"));
                if (!string.IsNullOrEmpty(form.konu) && form.konu.Length > 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONU], "LIKE", form.konu + "%"));
                if (form.geldigiYerTur > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GELDIGIYERTUR], "=", form.geldigiYerTur));
                if (!string.IsNullOrEmpty(form.icerik) && form.icerik.Length > 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ICERIK], "LIKE", form.icerik + "%"));
                if (form.gizlilik > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GIZLILIK], "=", form.gizlilik));
                if (form.yaziDurum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YAZIDURUM], "=", form.yaziDurum));
                if (form.gelisSekli > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GELISSEKLI], "=", form.gelisSekli));
                if (form.evrakTipi > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "EVRAKTIPI", "=", form.evrakTipi));
                else if (form.evrakTipi < 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "EVRAKTIPI", "<>", form.evrakTipi));
                if (form.sicil.asil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TS1", ALANLARSICIL[(int)ENUMAlanlarSicil.ASIL], "=", form.sicil.asil));
                if (!string.IsNullOrEmpty(form.evrakIcerigi) && form.evrakIcerigi.Length > 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKICERIK], "LIKE", form.evrakIcerigi + "%"));
                if (form.dosyaKodlari.Count > 0)
                {
                    string kosulDp = "";
                    string tarihKosul = "";
                    foreach (DosyaPlani dp in form.dosyaKodlari)
                    {
                        kosulDp = VeriErisim.KosulEkle(kosulDp, ",", "'" + dp.kod.Replace("%", "") + "'");

                        if (dp.acilisTarihi.ToString() != "" && dp.kapanisTarihi.ToString() != "")
                            tarihKosul += " AND (" + hareketTablo + ".PREVRAKNO IN (SELECT DISTINCT PREVRAKNO FROM EVREVRAKDOSYAPLANI INNER JOIN EVRDOSYAPLANI ON EVRDOSYAPLANI.KOD=EVREVRAKDOSYAPLANI.DOSYAPLANKODU WHERE EVRDOSYAPLANI.ACILISTARIHI= '" + dp.acilisTarihi + "' AND EVRDOSYAPLANI.KAPANISTARIHI= '" + dp.kapanisTarihi + "')" + ")";
                        //if (dp.kapanisTarihi != null)
                        //  tarihKosul += " AND (" + hareketTablo + ".PREVRAKNO IN (SELECT DISTINCT PREVRAKNO FROM EVREVRAKDOSYAPLANI INNER JOIN EVRDOSYAPLANI ON EVRDOSYAPLANI.KOD=EVREVRAKDOSYAPLANI.DOSYAPLANKODU WHERE EVRDOSYAPLANI.KAPANISTARIHI= '" + dp.kapanisTarihi + "'  )" + ")";
                    }

                    if (kosulDp != "")
                    {
                        kosul += " AND (" + hareketTablo + ".PREVRAKNO IN (SELECT DISTINCT PREVRAKNO FROM EVREVRAKDOSYAPLANI WHERE (DOSYAPLANKODU IN (" + kosulDp + ") ) )" + ")";
                    }
                    kosul += tarihKosul;
                }

                if (form.hareket.islem > 0)
                {
                    if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.YENI || form.hareket.islem == (int)ENUMEvrakHareketDurum.YENI)
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "<", 50));
                        if (form.prEvrakNoListe == "")
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "<>", (int)ENUMEvrakHareketDurum.IMZAYAGONDER));
                    }
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.IMZAYAGELENLER)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.IMZAYAGONDER));
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.BUTUNBELGELER)
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "<>", (int)ENUMEvrakHareketDurum.IPTALEDILDI));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "<>", (int)ENUMEvrakHareketDurum.ARSIVEKALDIR));
                    }
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.IPTAL || form.hareket.islem == (int)ENUMEvrakHareketDurum.IPTALEDILDI)
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.IPTALEDILDI));
                        //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TS1", ALANLARSICIL[(int)ENUMAlanlarSicil.ASIL], "=", 1));//Bilgi için gönderilenler hariç
                    }
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.HAVALEGELEN)
                    {
                        string havaleGelenKosul = "";
                        havaleGelenKosul = VeriErisim.KosulEkle(havaleGelenKosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.HAVALEEDILDI));
                        havaleGelenKosul = VeriErisim.KosulEkle(havaleGelenKosul, "OR", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.DAGITIMKAYIT));
                        kosul += " AND (" + havaleGelenKosul + ")";
                    }
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.DOSYAYAKALDIR || form.hareket.islem == (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.DOSYAYAKALDIR));
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.HAVALEEDILDI || form.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDI)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.HAVALEEDILDI));
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.HAVALEGERIALINDI || form.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEGERIALINDI)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.HAVALEGERIALINDI));
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.DOSYADANALINDI || form.hareket.islem == (int)ENUMEvrakHareketDurum.DOSYADANALINDI)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.DOSYADANALINDI));
                    else if (form.hareket.islem == (int)ENUMEvrakHareketDurum.IMZAYAGONDER)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.IMZAYAGONDER));
                    else if (form.hareket.islem == (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.HAVALEEDILDIGIDEN));
                    else if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLER || form.hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLERVEMIADLIISLER)
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", (int)ENUMEvrakHareketDurum.HAVALEEDILDI));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "ISLEM", "<", 60));

                        if (form.hareket.islem == (int)ENUMEvrakTarihceDurum.DEVAMEDENISLERVEMIADLIISLER)
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "CEVAPSURESI", ">", 0));
                    }
                }
                else
                {
                    if (form.hareket.islem > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "=", form.hareket.islem));
                    else
                    {
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "<>", (int)ENUMEvrakHareketDurum.IPTALEDILDI));
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEM", "<>", (int)ENUMEvrakHareketDurum.ARSIVEKALDIR));
                    }
                }
                if (!form.hareket.islemTarih.isNull)
                {
                    if (form.hareket.sorguIslemTarih.isNull)
                        form.hareket.sorguIslemTarih = new TNSDateTime(form.hareket.islemTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli

                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEMTARIH", ">=", form.hareket.islemTarih));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEMTARIH", "<=", new TNSDateTime(form.hareket.sorguIslemTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
                }

                if (form.hareket.islemYapan != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(hareketTablo, "ISLEMYAPAN", "=", form.hareket.islemYapan));

                if (!string.IsNullOrEmpty(form.dosyaNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", form.dosyaNo));
                if (!string.IsNullOrEmpty(form.evrakIcerigi))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKICERIK], "LIKE", "%" + form.evrakIcerigi + "%"));
                if (form.evrakCinsi > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKCINSI], "=", form.evrakCinsi));
                if (!string.IsNullOrEmpty(form.esasNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ESASNO], "=", form.esasNo));
                if (form.evrakTuru > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKTURU], "=", form.evrakTuru));
                if (!string.IsNullOrEmpty(form.bakanlikEvrakNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BAKANLIKEVRAKNO], "=", form.bakanlikEvrakNo));
                if (!string.IsNullOrEmpty(form.baskanlikEvrakNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASKANLIKEVRAKNO], "=", form.baskanlikEvrakNo));

                if (form.tasfiye.tasfiyeKod != 0 && form.tasfiye.tasfiyeKod != 5 && form.tasfiye.tasfiyeKod != -1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tasfiyeTablo, "TASFIYEKOD", "=", form.tasfiye.tasfiyeKod));

                if (form.saklama.sure != 0 || form.saklama.kriterKod != 0 || form.saklama.baslangicKod != 0) //saklama ile ilgili bir kayır aranmak isteniyosa
                {
                    ekKosul = "";
                    ekKosul = " TS1.PREVRAKNO IN " +
                       "( SELECT DISTINCT PREVRAKNO FROM EVREVRAKSAKLAMA WHERE 1=1";
                    if (form.saklama.kriterKod != 0)
                        ekKosul += " AND KRITERKOD = " + VeriErisim.DegerYap(form.saklama.kriterKod);
                    if (form.saklama.baslangicKod != 0)
                        ekKosul += " AND BASLANGICKOD = " + VeriErisim.DegerYap(form.saklama.baslangicKod);
                    if (form.saklama.sure != 0)
                        ekKosul += " AND SURE = " + VeriErisim.DegerYap(form.saklama.sure);

                    kosul += " AND (" + ekKosul + " ) )";
                }

                if (!form.dagitimTarih.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DAGITIMTARIH], "=", form.dagitimTarih));
                if (form.dinamikSayfaKod != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DINAMIKSAYFAKOD], "=", form.dinamikSayfaKod));
                if (form.dinamikSurecNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DINAMIKSURECNO], "=", form.dinamikSurecNo));
                if (form.surecAd != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECAD], "=", form.surecAd));
                if (form.dosyaNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", form.dosyaNo));
                if (!string.IsNullOrEmpty(form.prEvrakNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TS1", ALANLARSICIL[(int)ENUMAlanlarSicil.PREVRAKNO], "=", form.prEvrakNo));
                if (form.eImzalandi > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EIMZALANDI], "=", form.eImzalandi));
            }

            //Evrak Arşiv listesi için
            if (form.arsiv.kod != 0)
            {
                sql += "INNER JOIN EVREVRAKARSIV ON TS1.PREVRAKNO = EVREVRAKARSIV.PREVRAKNO ";
                sql += "INNER JOIN EVRARSIV ON EVREVRAKARSIV.ARSIVKOD = EVRARSIV.KOD ";
                sql += "INNER JOIN EVRARSIVYETKI ON EVRARSIV.KOD = EVRARSIVYETKI.ARSIVKOD ";
                sql = sql.Replace("SELECT", "SELECT DISTINCT");

                kosul = VeriErisim.KosulEkle(kosul, "AND", " (EVREVRAKARSIV.ARSIVKOD = " + form.arsiv.kod + ") ");
                kosul += " AND ( ";
                kosul += "(EVRARSIVYETKI.YETKI = " + VeriErisim.DegerYap(form.arsiv.kullaniciBilgi.mernis) + ") AND (EVRARSIVYETKI.YETKITIP = " + (int)ENUMArsivYetkiTip.KISI + ") OR ";
                kosul += "(EVRARSIVYETKI.YETKI = " + VeriErisim.DegerYap(form.arsiv.kullaniciBilgi.unvan) + ") AND (EVRARSIVYETKI.YETKITIP = " + (int)ENUMArsivYetkiTip.UNVAN + ") ";
                kosul += " )";
            }
            //*******************

            if (!form.gizlilikGecerlilikSuresi.isNull)
            {
                if (form.gizlilikGecerlilikSuresi.isNull)
                    form.gizlilikGecerlilikSuresi = new TNSDateTime(form.gizlilikGecerlilikSuresi.Oku());

                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GIZLILIKGECERLILIKSURE], ">=", form.gizlilikGecerlilikSuresi));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GIZLILIKGECERLILIKSURE], "<=", new TNSDateTime(form.sorguGizlilikGecerlilikSuresi.isNull ? form.gizlilikGecerlilikSuresi.Oku().AddDays(1) : form.sorguGizlilikGecerlilikSuresi.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
            }


            if (kosul != "")
                sql += " WHERE " + kosul;

            if (form.tasfiye.tasfiyeKod == 5)
                sql += " AND TS1.PREVRAKNO NOT IN (SELECT PREVRAKNO FROM [EVREVRAKTASFIYE]) ";

            if (!sadeceSatirSayisi && !sadecePrEvrakNo && (form.evrEvrakNo == "" && form.prEvrakNo == ""))
            {
                if (form.siralamaKolonu != "")
                {
                    if (form.siralamaKolonu.IndexOf("EVREVRAK.EVRAKNO") == -1)
                        form.siralamaKolonu += ",EVREVRAK.EVRAKNO";

                    sql += " ORDER BY " + form.siralamaKolonu;
                }
                else
                    sql += " ORDER BY EVREVRAK.EVRAKNO DESC";
            }

            //OFFSET ve FETCH SQL Server 2012 de geçerli olduğu için kapatıldı
            //if (limit > 0 && !sadeceSatirSayisi && !sadecePrEvrakNo)
            //    sql += " OFFSET " + start + " ROWS FETCH NEXT " + limit + " ROWS ONLY";

            veriErisim.sureAsimi = 180;
            if (!string.IsNullOrEmpty(kosul))
                return veriErisim.DataReaderDondur(sql);
            else
                return null;
        }

        string SiralamaKolonuBul(string siralama)
        {
            string donenDeger = "";
            if (siralama.StartsWith("evrEvrakNo ")) donenDeger = siralama.Replace("evrEvrakNo", "EVREVRAK.EVREVRAKNO");
            if (siralama.StartsWith("EVREVRAKNO ")) donenDeger = siralama.Replace("EVREVRAKNO", "EVREVRAK.EVREVRAKNO");
            if (siralama.StartsWith("prEvrakNo ")) donenDeger = siralama.Replace("prEvrakNo", "PREVRAKNO");
            if (siralama.StartsWith("PREVRAKNO ")) donenDeger = siralama.Replace("PREVRAKNO", "PREVRAKNO");
            if (siralama.StartsWith("evrakNo ")) donenDeger = siralama.Replace("evrakNo", "EVREVRAK.EVRAKNO");
            if (siralama.StartsWith("EVRAKNO ")) donenDeger = siralama.Replace("EVRAKNO", "EVREVRAK.EVRAKNO");
            if (siralama.StartsWith("evrakTarih ")) donenDeger = siralama.Replace("evrakTarih", "EVREVRAK.EVRAKTARIH");
            if (siralama.StartsWith("EVRAKTARIH ")) donenDeger = siralama.Replace("EVRAKTARIH", "EVREVRAK.EVRAKTARIH");
            if (siralama.StartsWith("konu ")) donenDeger = siralama.Replace("konu", "EVREVRAK.KONU");
            if (siralama.StartsWith("KONU ")) donenDeger = siralama.Replace("KONU", "EVREVRAK.KONU");
            if (siralama.StartsWith("KONUYER ")) donenDeger = siralama.Replace("KONUYER", "EVREVRAK.KONU");
            if (siralama.StartsWith("geldigiYer ")) donenDeger = siralama.Replace("geldigiYer", "EVREVRAK.GELDIGIYER");
            if (siralama.StartsWith("GELDIGIYER ")) donenDeger = siralama.Replace("GELDIGIYER", "EVREVRAK.GELDIGIYER");
            if (siralama.StartsWith("GELENTARIH ")) donenDeger = siralama.Replace("GELENTARIH", "EVREVRAK.GELENEVRAKTARIH");

            return donenDeger;
        }

        /// <summary>
        /// Paramete olarak verilen EvrakSinif nesnesine göre; evrak'ın kullanıcının kendi biriminde olup olmadığını, sql cümlesi hazırlayıp çalıştırarak veritabanından sorgulayan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <param name="teslimAlmaIslemi">Teslim alma durum bilgisini içerene nesne</param>
        /// <returns>Evrak kendi biriminde ise Doğru, değilse Yanlış döndürülür.</returns>
        public bool EvrakKendiBirimindeMi(EvrakSinif form, bool teslimAlmaIslemi, bool altBirimleriDahilEt)
        {
            string sql = "SELECT COUNT(1) FROM EVREVRAKHAREKETSONDURUM ";

            if (altBirimleriDahilEt)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "PREVRAKNO", "LIKE", form.prEvrakNo + "%"));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "PREVRAKNO", "=", form.prEvrakNo));

            if (!teslimAlmaIslemi)
            {
                //Belge yöneticisi kendi biriminde olmayan evraklar üzerinde değişiklik yapabilmesi için
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIMKOD1", "=", form.bulunduguYer));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIMKOD1", "LIKE", form.bulunduguYer + "%"));
                if (!string.IsNullOrEmpty(form.hareket.kisi1))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KISI1", "=", form.hareket.kisi1));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BIRIMKOD2", "=", form.bulunduguYer));
                if (!string.IsNullOrEmpty(form.hareket.kisi1))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KISI2", "=", form.hareket.kisi1));
            }

            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        public string AyniGidenEvrakMevcutMu(EvrakSinif form)
        {
            string sql = "SELECT EVREVRAKNO FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "EVRAKBIRIMKODU", "LIKE", form.evrakBirimKodu + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "YIL", "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "EVRAKTIPI", "=", (int)ENUMEvrakTipi.GIDEN));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "EVRAKNO", ">", 0));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GELDIGIYER", "=", form.geldigiYer));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KONU", "=", form.konu));

            return veriErisim.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Paramete olarak verilen EvrakSinif nesnesine göre; aynı evrak'ın veritabanında olup olmadığını, sql cümlesi hazırlayıp çalıştırarak veritabanından sorgulayan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <returns>Sonuç evrEvrakNo bilgisi olarak döndürülür.</returns>
        public string AyniEvraktanVarmi(EvrakSinif form)
        {
            TNSDateTime tarih1 = new TNSDateTime(form.gelenEvrakTarih.Oku());
            TNSDateTime tarih2 = new TNSDateTime(form.gelenEvrakTarih.Oku());
            tarih2.AddDays(1);

            string sql = "SELECT EVREVRAKNO FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "GELDIGIYER", "=", form.geldigiYer));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GELENEVRAKTARIH", ">=", tarih1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GELENEVRAKTARIH", "<", tarih2));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "GELENEVRAKNO", "=", form.gelenEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "EVRAKTIPI", "=", (int)ENUMEvrakTipi.GELEN));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "EVRAKBIRIMKODU", "LIKE", form.evrakBirimKodu + "%"));

            if (form.evrEvrakNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "EVREVRAKNO", "<>", form.evrEvrakNo));

            return veriErisim.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Paramete olarak verilen EvrakSinif nesnesine göre; evrak'ın birim veya kişiye uğrayıp uğramadığını, sql cümlesi hazırlayıp çalıştırarak veritabanından sorgulayan yordam.
        /// </summary>
        /// <param name="form">Evrak bilgisini içeren nesne</param>
        /// <returns>Evrak birim veya kişiye uğradıysa doğru, yoksa yanlış döndürülür.</returns>
        public bool EvrakBirimeKisiyeUgradiMi(EvrakSinif form)
        {
            string sql = "SELECT COUNT(1) FROM EVREVRAKHAREKET ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "PREVRAKNO", "=", form.prEvrakNo));

            string birimKisiKosul1 = "";
            string birimKisiKosul2 = "";

            if (!string.IsNullOrEmpty(form.bulunduguYer))
                birimKisiKosul1 = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("", "BIRIMKOD1", "=", form.bulunduguYer));
            if (!string.IsNullOrEmpty(form.hareket.kisi1))
                birimKisiKosul1 = VeriErisim.KosulEkle(birimKisiKosul1, " AND ", VeriErisim.KosulOl("", "KISI1", "=", form.hareket.kisi1));
            birimKisiKosul1 = "(" + birimKisiKosul1 + ")";

            if (!string.IsNullOrEmpty(form.bulunduguYer))
                birimKisiKosul2 = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("", "BIRIMKOD2", "=", form.bulunduguYer));
            if (!string.IsNullOrEmpty(form.hareket.kisi1))
                birimKisiKosul2 = VeriErisim.KosulEkle(birimKisiKosul2, " AND ", VeriErisim.KosulOl("", "KISI2", "=", form.hareket.kisi1));
            birimKisiKosul2 = "(" + birimKisiKosul2 + ")";

            if (birimKisiKosul1 == "")//birim veya kişi boş gelmemesi gerek
                birimKisiKosul1 = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("", "BIRIMKOD1", "=", "XXX"));
            if (birimKisiKosul2 == "")//birim veya kişi boş gelmemesi gerek
                birimKisiKosul2 = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("", "BIRIMKOD1", "=", "XXX"));

            sql = sql + " AND (" + birimKisiKosul1 + " OR " + birimKisiKosul2 + ")";

            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        /// <summary>
        /// Paramete olarak verilen evrak bilgisine göre; evrak'ın son hareket işlem bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından sorgulayan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisini içeren nesne</param>
        /// <returns>Evrak harket durumunu int türünde döndürülür.</returns>
        public int EvrakSonHareket(string prEvrakNo)
        {
            string sql = "SELECT ISLEM FROM EVREVRAKHAREKETSONDURUM ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "PREVRAKNO", "=", prEvrakNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Paramete olarak verilen bilgilere göre; evrak konu bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu bilgisini içeren nesne</param>
        /// <param name="kullanimSayisi">Evrak konu bilgisinin kullanım sayısını içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader KonuListele(string birim, string konu, int kullanimSayisi)
        {
            string sql = "SELECT EVRAKBIRIMKODU,KONU,KULLANIMSAYISI FROM EVRGELENKONU ";
            string kosul = "";

            if (!string.IsNullOrEmpty(birim.Trim()))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVRGELENKONU", "EVRAKBIRIMKODU", "=", birim.Trim()));
            if (!string.IsNullOrEmpty(konu.Trim()))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVRGELENKONU", "KONU", "LIKE", "%" + konu.Trim() + "%"));
            if (kullanimSayisi > 0)
            {
                string ekKosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("EVRGELENKONU", "KULLANIMSAYISI", ">", kullanimSayisi));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("EVRGELENKONU", "KALICI", "=", 1));
                kosul += " AND (" + ekKosul + ")";
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY KULLANIMSAYISI DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; evrak konu bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu bilgisini içeren nesne</param>
        /// <param name="kalici">Kalıcı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KonuKaydet(string birim, string konu, int kalici)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            konu = konu.Replace("'", " ");

            sql = "INSERT INTO EVRGELENKONU (EVRAKBIRIMKODU,KONU,KULLANIMSAYISI,KALICI) VALUES (";
            sql += VeriErisim.DegerYap(birim.Trim()) + ",";
            sql += VeriErisim.DegerYap(konu.Trim()) + ",";
            sql += VeriErisim.DegerYap(1) + ",";
            sql += VeriErisim.DegerYap(kalici) + ")";
            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE EVRGELENKONU SET ";
                sql += "KULLANIMSAYISI=(SELECT COUNT(*) FROM EVREVRAK WHERE EVRAKBIRIMKODU=" + VeriErisim.DegerYap(birim.Trim()) + " AND KONU=" + VeriErisim.DegerYap(konu.Trim()) + ")";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVRGELENKONU", "EVRAKBIRIMKODU", "=", birim.Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRGELENKONU", "KONU", "=", konu.Trim()));
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak birim kodu ve konu bilgisine göre; evrak konu bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="konu">Evrak konu bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KonuSil(string birim, string konu)
        {
            string sql = "DELETE FROM EVRGELENKONU ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVRGELENKONU", "EVRAKBIRIMKODU", "=", birim.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRGELENKONU", "KONU", "=", konu.Trim()));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Paramete olarak verilen bilgilere göre; evrak meta grup bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="kod">The kod.</param>
        /// <param name="adi">The adi.</param>
        /// <returns>
        /// İşlem sonucu DataReader olarak döndürülür.
        /// </returns>
        public IDataReader MetaGrupListele(string birim, int kod, string adi)
        {
            string sql = "SELECT EVRAKBIRIMKODU,KOD,ADI FROM EVRMETAGRUP ";
            string kosul = "";

            if (!string.IsNullOrEmpty(birim.Trim()))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVRMETAGRUP", "EVRAKBIRIMKODU", "=", birim.Trim()));
            if (kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVRMETAGRUP", "KOD", "=", kod));
            if (!string.IsNullOrEmpty(adi.Trim()))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVRMETAGRUP", "ADI", "LIKE", "%" + adi.Trim() + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY EVRAKBIRIMKODU,ADI";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; evrak meta grup bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="kod">The kod.</param>
        /// <param name="adi">The adi.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc MetaGrupKaydet(string birim, int kod, string adi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            adi = adi.Replace("'", " ");

            sql = "INSERT INTO EVRMETAGRUP (EVRAKBIRIMKODU,KOD,ADI) VALUES (";
            sql += VeriErisim.DegerYap(birim.Trim()) + ",";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(adi) + ")";
            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE EVRMETAGRUP SET ";
                sql += VeriErisim.AtamaOl("", "ADI", adi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVRMETAGRUP", "EVRAKBIRIMKODU", "=", birim.Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRMETAGRUP", "KOD", "=", kod));
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak birim kodu ve meta grup bilgisine göre; evrak meta grup bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="kod">The kod.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc MetaGrupSil(string birim, int kod)
        {
            string sql = "DELETE FROM EVRMETAGRUP ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVRMETAGRUP", "EVRAKBIRIMKODU", "=", birim.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRMETAGRUP", "KOD", "=", kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Paramete olarak verilen bilgilere göre; evrak yer isimlerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer ad bilgisini içeren nesne</param>
        /// <param name="tur">Evrak yer tür bilgisini içeren nesne</param>
        /// <param name="kullanimSayisi">Evrak yer bilgisinin kullanım sayısını içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader YerIsimleriListele(string birim, string yer, int tur, int kullanimSayisi)
        {
            string sql = "SELECT EVRAKBIRIMKODU,YERADI,YERTURU,YERBIRIMKODU,KULLANIMSAYISI FROM EVRGELENYER ";
            string kosul = "";

            if (!string.IsNullOrEmpty(birim.Trim()))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVRGELENYER", "EVRAKBIRIMKODU", "=", birim.Trim()));
            if (!string.IsNullOrEmpty(yer.Trim()))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVRGELENYER", "YERADI", "LIKE", "%" + yer.Trim() + "%"));
            if (tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVRGELENYER", "YERTURU", "=", tur));
            if (kullanimSayisi > 0)
            {
                string ekKosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("EVRGELENYER", "KULLANIMSAYISI", ">", kullanimSayisi));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("EVRGELENYER", "KALICI", "=", 1));
                kosul += " AND (" + ekKosul + ")";
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY YERADI";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; evrak yer bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer ad bilgisini içeren nesne</param>
        /// <param name="tur">Evrak yer tür bilgisini içeren nesne</param>
        /// <param name="yerBirimKodu">Kaydedilecek yerin evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="kalici">Kalıcı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc YerIsimleriKaydet(string birim, string yer, int tur, string yerBirimKodu, int kalici)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO EVRGELENYER (EVRAKBIRIMKODU,YERADI,YERTURU,YERBIRIMKODU,KULLANIMSAYISI,KALICI) VALUES (";
            sql += VeriErisim.DegerYap(birim.Trim()) + ",";
            sql += VeriErisim.DegerYap(yer.Trim()) + ",";
            sql += VeriErisim.DegerYap(tur) + ",";
            sql += VeriErisim.DegerYap(yerBirimKodu.Trim()) + ",";
            sql += VeriErisim.DegerYap(1) + ",";
            sql += VeriErisim.DegerYap(kalici) + ")";
            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE EVRGELENYER SET ";
                sql += "KULLANIMSAYISI=(SELECT COUNT(*) FROM EVREVRAK WHERE EVRAKBIRIMKODU=" + VeriErisim.DegerYap(birim.Trim()) + " AND GELDIGIYER=" + VeriErisim.DegerYap(yer.Trim()) + " AND GELDIGIYERTUR=" + tur + "),";
                sql += "KALICI=" + kalici;
                if (yerBirimKodu.Trim() != "" || kalici > 0)
                    sql += "," + VeriErisim.AtamaOl("", "YERBIRIMKODU", yerBirimKodu.Trim());
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVRGELENYER", "EVRAKBIRIMKODU", "=", birim.Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRGELENYER", "YERADI", "=", yer.Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRGELENYER", "YERTURU", "=", tur));
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; evrak yer bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="birim">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="yer">Evrak yer ad bilgisini içeren nesne</param>
        /// <param name="tur">Evrak yer tür bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc YerIsimleriSil(string birim, string yer, int tur)
        {
            string sql = "DELETE FROM EVRGELENYER ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVRGELENYER", "EVRAKBIRIMKODU", "=", birim.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRGELENYER", "YERADI", "=", yer.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRGELENYER", "YERTURU", "=", tur));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen EvrakSicil nesnesine göre; evrak sicil bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Evrak sicil bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KaydetSicil(EvrakSicil form)
        {
            string sql = "INSERT INTO " + TABLOADSICIL + " (" + VeriErisim.TumAlanlar("", ALANLARSICIL) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.asil) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen EvrakIliski nesne koleksiyonuna göre; evrak ilişki bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">EvrakIliski nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc IliskiKaydet(EvrakIliski form)
        {
            string sql = "INSERT INTO " + TABLOADILISKI + " (" + VeriErisim.TumAlanlar("", ALANLARILISKI) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.prEvrakNoIliski) + ",";
            sql += VeriErisim.DegerYap(form.tip) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen EvrakIliski nesne koleksiyonuna göre; evrak ilişki bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="form">EvrakIliski nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc IliskiSil(EvrakIliski form)
        {
            string sql = "DELETE FROM " + TABLOADILISKI + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADILISKI, ALANLARILISKI[(int)ENUMAlanlarIliski.PREVRAKNO], "=", form.prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADILISKI, ALANLARILISKI[(int)ENUMAlanlarIliski.PREVRAKNOILISKI], "=", form.prEvrakNoIliski));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADILISKI, ALANLARILISKI[(int)ENUMAlanlarIliski.TIP], "=", ei.tip));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Evrak ilişki bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="prEvrakNo">The pr evrak no.</param>
        /// <param name="tip">The tip.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc IliskiSilHepsi(string prEvrakNo, int tip)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "DELETE FROM " + TABLOADILISKI + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADILISKI, ALANLARILISKI[(int)ENUMAlanlarIliski.PREVRAKNO], "=", prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADILISKI, ALANLARILISKI[(int)ENUMAlanlarIliski.TIP], "=", tip));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; evrak ilişki bilgilerine bakarak, evrEvrakNo ve prEvrakNo bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="iliskiprEvrakNo">İlişki prEvrakNo bilgisini içeren nesne</param>
        /// <param name="iliskiTip">İlişki tipini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader EvrakIliskidenEvrVePrEvrakNoVer(string iliskiprEvrakNo, int iliskiTip)
        {
            string sql = "";
            sql = "SELECT     EVREVRAK.EVREVRAKNO, EVREVRAKSICIL.PREVRAKNO ";
            sql += "FROM EVREVRAK INNER JOIN ";
            sql += "EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO INNER JOIN ";
            sql += "EVREVRAKILISKI ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKILISKI.PREVRAKNO ";
            sql += "WHERE (EVREVRAK.EVRAKTIPI = " + (int)ENUMEvrakTipi.GIDEN + ") AND (EVREVRAKILISKI.PREVRAKNOILISKI = " + VeriErisim.DegerYap(iliskiprEvrakNo) + ") AND (EVREVRAKILISKI.TIP = " + iliskiTip + ")";
            //sql = "SELECT  EVREVRAK.EVREVRAKNO, EVREVRAKSICIL.PREVRAKNO ";
            //sql += "FROM   EVREVRAK INNER JOIN EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO ";
            //sql += "WHERE  (EVREVRAK.ILGIEVRAKNO = '" + prEvrakNo + "') AND (EVREVRAK.EVRAKTIPI = " + (int)ENUMEvrakTipi.GIDEN + ")";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisine göre; evrak numrası ve tarihini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader EvrakNoVeTarihVer(string prEvrakNo)
        {
            string sql = "";
            sql = "SELECT EVREVRAK.EVRAKNO, EVREVRAK.EVRAKTARIH, EVREVRAK.EVREVRAKNO ";
            sql += "FROM EVREVRAK INNER JOIN ";
            sql += "EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO ";
            sql += "WHERE (EVREVRAKSICIL.PREVRAKNO = " + VeriErisim.DegerYap(prEvrakNo) + ")";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; ilişkili evrakları, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisini içeren nesne</param>
        /// <param name="tip">Parametre olarak verilen evrak bilgisine göre, ilgi olarak gösterdikleri veya ilgi olarak gösterilenler bilgisini içeren nesne. 1: İlgi gösterdikleri, 2: İlgi gösterilenler</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader ListeleIliskiliEvraklar(string prEvrakNo, int tip)
        {
            string sql = "WITH T2 AS ( SELECT DISTINCT PREVRAKNOILISKI,PREVRAKNO  from EVREVRAKILISKI WHERE  ";
            sql += " (PREVRAKNO=" + VeriErisim.DegerYap(prEvrakNo) + " OR PREVRAKNOILISKI=" + VeriErisim.DegerYap(prEvrakNo) + ") )";
            if (tip == 1)
                sql += " SELECT DISTINCT ei.PREVRAKNO from EVREVRAKILISKI as ei INNER JOIN t2 on ei.PREVRAKNOILISKI=t2.PREVRAKNO";
            else if (tip == 2)
                sql += " SELECT DISTINCT ei.PREVRAKNOILISKI from EVREVRAKILISKI as ei INNER JOIN t2 on ei.PREVRAKNOILISKI=t2.PREVRAKNOILISKI";
            else
                sql += " SELECT DISTINCT ei.PREVRAKNO from EVREVRAKILISKI as ei INNER JOIN t2 on ei.PREVRAKNOILISKI=t2.PREVRAKNOILISKI";

            //Vakıflar GM. de PREVRAKNO=20120288165 evrakında ilişki alınamıyordu.21.11.2012
            //string sql = "WITH T2 (ILISKIPREVRAKNO,SAYAC) AS ( SELECT PREVRAKNOILISKI,0 AS SAYAC  from EVREVRAKILISKI WHERE ";
            //sql += " (PREVRAKNO=" + VeriErisim.DegerYap(prEvrakNo) + " OR PREVRAKNOILISKI=" + VeriErisim.DegerYap(prEvrakNo) + ")";
            //sql += " UNION ALL ";
            //if (tip == 1)
            //    sql += " SELECT PREVRAKNO,       SAYAC+1 from EVREVRAKILISKI as ei INNER JOIN t2 on ei.PREVRAKNOILISKI=ILISKIPREVRAKNO AND SAYAC<99) SELECT EVREVRAKSICIL.PREVRAKNO,EVREVRAK.* FROM T2 INNER JOIN EVREVRAKSICIL ON T2.ILISKIPREVRAKNO=EVREVRAKSICIL.PREVRAKNO INNER JOIN EVREVRAK ON EVREVRAKSICIL.EVREVRAKNO=EVREVRAK.EVREVRAKNO;";
            //else
            //    sql += " SELECT PREVRAKNOILISKI, SAYAC+1 from EVREVRAKILISKI as ei INNER JOIN t2 on ei.PREVRAKNO=      ILISKIPREVRAKNO AND SAYAC<99) SELECT EVREVRAKSICIL.PREVRAKNO,EVREVRAK.* FROM T2 INNER JOIN EVREVRAKSICIL ON T2.ILISKIPREVRAKNO=EVREVRAKSICIL.PREVRAKNO INNER JOIN EVREVRAK ON EVREVRAKSICIL.EVREVRAKNO=EVREVRAK.EVREVRAKNO;";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleIliskiliEvraklarDetay(string[] prEvrakNolar)
        {
            string kosul = "";
            for (int i = 0; i < prEvrakNolar.Length; i++)
            {
                if (string.IsNullOrEmpty(prEvrakNolar[i])) break;
                if (!string.IsNullOrEmpty(kosul)) kosul += ",";
                kosul += VeriErisim.DegerYap(prEvrakNolar[i]);
            }

            if (kosul != "")
            {
                string sql = "SELECT EVREVRAKSICIL.PREVRAKNO,EVREVRAK.* FROM EVREVRAKSICIL INNER JOIN";
                sql += " EVREVRAK ON EVREVRAKSICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO";
                sql += " WHERE PREVRAKNO IN(";
                sql += kosul;
                sql += " )";

                return veriErisim.DataReaderDondur(sql);
            }
            else
                return null;
        }

        //public IDataReader KisiUzerindekileriListele(EvrakSinif form)
        //{
        //    string kisi1Kosul = "";
        //    string kisi2Kosul = "";
        //    string kosul = "";

        //    string sql = "SELECT EVREVRAK.YIL, EVREVRAK.EVREVRAKNO, EVREVRAK.EVRAKTIPI, EVREVRAK.EVRAKNO,EVREVRAK.EVRAKTARIH, EVREVRAK.GELDIGIYER, EVREVRAK.KONU,EVREVRAK.CEVAPSURESI,EVREVRAKHAREKETSONDURUM.KISI1AD,EVREVRAKHAREKETSONDURUM.KISI2AD, EVREVRAKHAREKETSONDURUM.ISLEMYAPANAD, EVREVRAKHAREKETSONDURUM.ISLEM,EVREVRAKHAREKETSONDURUM.ISLEMTARIH,DATEDIFF(D,EVREVRAKHAREKETSONDURUM.ISLEMTARIH,GETDATE()) SURE ";
        //    sql += "FROM EVREVRAKSICIL INNER JOIN ";
        //    sql += "EVREVRAK ON EVREVRAKSICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO INNER JOIN ";
        //    sql += "EVREVRAKHAREKETSONDURUM ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKHAREKETSONDURUM.PREVRAKNO ";

        //    if (!form.hareket.islemTarih.isNull)
        //    {
        //        if (form.hareket.sorguIslemTarih.isNull)
        //            form.hareket.sorguIslemTarih = new TNSDateTime(form.hareket.islemTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "ISLEMTARIH", ">=", form.hareket.islemTarih));
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "ISLEMTARIH", "<=", new TNSDateTime(form.hareket.sorguIslemTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
        //    }

        //    if (form.yil > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "YIL", "=", form.yil));

        //    if (form.evrakTipi > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTIPI", "=", form.evrakTipi));

        //    if (form.hareket.kisi2 != "")
        //    {
        //        kisi1Kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "KISI1", "=", form.hareket.kisi2));
        //        kisi2Kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "KISI2", "=", form.hareket.kisi2));

        //    }

        //    sql += "WHERE ";
        //    sql += "(" + kosul + " AND EVREVRAKHAREKETSONDURUM.ISLEM < 50 " + kisi1Kosul + ") OR ";
        //    sql += "(" + kosul + " AND EVREVRAKHAREKETSONDURUM.ISLEM = 51 " + kisi2Kosul + ") ";
        //    sql += "ORDER BY EVRAKTIPI,SURE DESC";

        //    veriErisim.sureAsimi = 1000;
        //    return veriErisim.DataReaderDondur(sql);
        //}

        public IDataReader EvrakHareketleriniListele(EvrakSinif form)
        {
            //string kosul = "";

            //string sql = "SELECT EVREVRAK.EVRAKNO,EVRBIRIM.AD AS 'birimKod1Ad',B2.AD AS 'birimKod2Ad',EVREVRAKHAREKETSONDURUMBIRIM.ISLEMTARIH,EVREVRAKHAREKETSONDURUMBIRIM.ISLEM,EVREVRAKHAREKETSONDURUMBIRIM.ISLEMYAPANAD,EVREVRAK.KONU,EVREVRAK.GELDIGIYER,EVREVRAK.EVRAKTARIH,EVREVRAK.EVREVRAKNO,EVREVRAKSICIL.PREVRAKNO ";
            //sql += "FROM EVREVRAKHAREKETSONDURUMBIRIM INNER JOIN ";
            //sql += "EVREVRAKSICIL ON EVREVRAKHAREKETSONDURUMBIRIM.PREVRAKNO=EVREVRAKSICIL.PREVRAKNO INNER JOIN ";
            //sql += "EVREVRAK ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO LEFT JOIN ";
            //sql += "EVRBIRIM ON EVRBIRIM.KOD=EVREVRAK.HAVALEBIRIMKODU LEFT JOIN ";
            //sql += "EVRBIRIM AS B2 ON B2.KOD=EVREVRAKHAREKETSONDURUMBIRIM.BIRIMKOD2 LEFT JOIN ";
            //sql += "EVREVRAKDOSYAPLANI ON EVREVRAKDOSYAPLANI.PREVRAKNO=EVREVRAKSICIL.PREVRAKNO ";

            //if (!form.evrakTarih.isNull)
            //{
            //    if (form.sorguEvrakTarih.isNull)
            //        form.sorguEvrakTarih = new TNSDateTime(form.evrakTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUMBIRIM", "ISLEMTARIH", ">=", form.evrakTarih));
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUMBIRIM", "ISLEMTARIH", "<=", new TNSDateTime(form.sorguEvrakTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
            //}

            //if (!string.IsNullOrEmpty(form.hareket.kisi2))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUMBIRIM", "KISI2", "=", form.hareket.kisi2));

            //if (!string.IsNullOrEmpty(form.hareket.birimKod2))
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUMBIRIM", "BIRIMKOD1", "LIKE", form.hareket.birimKod2 + "%"));

            //if (!string.IsNullOrEmpty(form.geldigiYer) && form.geldigiYer.Length > 1)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "GELDIGIYER", "LIKE", form.geldigiYer + "%"));

            //if (!string.IsNullOrEmpty(form.tarihce.islemYapan))
            //{
            //    kosul += " AND (" +
            //        "EVREVRAK.EVREVRAKNO IN (SELECT EVREVRAKNO FROM EVREVRAKTARIHCE WHERE   EVREVRAKTARIHCE.ISLEMYAPAN =   '" + form.tarihce.islemYapan + "')"
            //        + ")";
            //}

            //if (!string.IsNullOrEmpty(form.tarihce.islem))
            //{
            //    kosul += " AND (" +
            //        "EVREVRAK.EVREVRAKNO IN (SELECT EVREVRAKNO FROM EVREVRAKTARIHCE WHERE   EVREVRAKTARIHCE.ISLEM =   '" + form.tarihce.islem + "')"
            //        + ")";
            //}

            //if (form.evrakNo != -1)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKNO", "=", form.evrakNo));

            //if (form.evrakTipi != 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTIPI", "=", form.evrakTipi));

            //if (form.hareket.islem != 0)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUMBIRIM", "ISLEM", "=", form.hareket.islem));

            //sql += "WHERE ";
            //sql += "(" + kosul + ") ";
            //sql += "ORDER BY EVREVRAKSICIL.PREVRAKNO,EVREVRAKHAREKETSONDURUMBIRIM.ISLEMTARIH,EVREVRAK.EVRAKNO,EVREVRAK.EVREVRAKNO,EVREVRAKHAREKETSONDURUMBIRIM.SIRANO";

            //return veriErisim.DataReaderDondur(sql);


            string hareketKosul = "";
            string tarihceKosul = "";
            bool hareketVar = true;
            bool tarihceVar = true;

            if (!form.evrakTarih.isNull)
            {
                if (form.sorguEvrakTarih.isNull)
                    form.sorguEvrakTarih = new TNSDateTime(form.evrakTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli

                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", "ISLEMTARIH", ">=", form.evrakTarih));
                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", "ISLEMTARIH", "<=", new TNSDateTime(form.sorguEvrakTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAKTARIHCE", "ISLEMTARIH", ">=", form.evrakTarih));
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAKTARIHCE", "ISLEMTARIH", "<=", new TNSDateTime(form.sorguEvrakTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
            }

            if (!string.IsNullOrEmpty(form.geldigiYer) && form.geldigiYer.Length > 1)
            {
                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "GELDIGIYER", "LIKE", form.geldigiYer + "%"));
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "GELDIGIYER", "LIKE", form.geldigiYer + "%"));
            }
            if (form.evrakNo != -1)
            {
                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKNO", "=", form.evrakNo));
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKNO", "=", form.evrakNo));
            }
            if (form.evrakTipi != 0)
            {
                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTIPI", "=", form.evrakTipi));
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTIPI", "=", form.evrakTipi));
            }

            if (!string.IsNullOrEmpty(form.hareket.birimKod2))
            {
                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", "BIRIMKOD1", "LIKE", form.hareket.birimKod2 + "%"));
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKBIRIMKOD", "LIKE", form.hareket.birimKod2 + "%"));
            }

            if (!string.IsNullOrEmpty(form.tarihce.islemYapan))
            {
                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", "ISLEMYAPAN", "=", form.tarihce.islemYapan));
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAKTARIHCE", "ISLEMYAPAN", "=", form.tarihce.islemYapan));
            }

            if (!string.IsNullOrEmpty(form.tarihce.islem))
            {
                hareketVar = false;
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAKTARIHCE", "ISLEM", "=", form.tarihce.islem));
            }

            if (form.hareket.islem != 0)
            {
                tarihceVar = false;
                hareketVar = true;
                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKET", "ISLEM", "=", form.hareket.islem));
            }

            if (!string.IsNullOrEmpty(form.evrEvrakNo))
            {
                tarihceKosul = VeriErisim.KosulEkle(tarihceKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVREVRAKNO", "=", form.evrEvrakNo));
                hareketKosul = VeriErisim.KosulEkle(hareketKosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVREVRAKNO", "=", form.evrEvrakNo));
            }

            string sql = "";

            if (hareketVar)
            {
                sql += " SELECT EVREVRAK.EVRAKNO,EVRBIRIM1.AD AS 'birimKod1Ad',EVRBIRIM1.AD AS 'birimKod2Ad',EVREVRAKHAREKET.ISLEMTARIH AS ITARIH,EVREVRAKHAREKET.ISLEM AS HAREKETISLEM, EVREVRAKHAREKET.ISLEMYAPANAD,EVREVRAK.KONU,EVREVRAK.GELDIGIYER,EVREVRAK.EVRAKTARIH,EVREVRAK.EVREVRAKNO,EVREVRAKSICIL.PREVRAKNO,'' AS TARIHCEISLEM  ";
                sql += " FROM ";
                sql += " EVRBIRIM AS EVRBIRIM1 RIGHT OUTER JOIN";
                sql += " EVREVRAKHAREKET ON EVRBIRIM1.KOD = EVREVRAKHAREKET.BIRIMKOD1 RIGHT OUTER JOIN";
                sql += " EVREVRAK INNER JOIN";
                sql += " EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO ON EVREVRAKHAREKET.PREVRAKNO = EVREVRAKSICIL.PREVRAKNO LEFT OUTER JOIN";
                sql += " EVRBIRIM AS EVRBIRIM2 ON EVRBIRIM2.KOD = EVREVRAKHAREKET.BIRIMKOD2";
                sql += " WHERE ";
                sql += "(" + hareketKosul + ") ";
            }

            if (hareketVar && tarihceVar)
                sql += " UNION ";

            if (tarihceVar)
            {
                sql += "SELECT EVREVRAK.EVRAKNO, '' AS 'birimKod1Ad', '' AS 'birimKod2Ad', EVREVRAKTARIHCE.ISLEMTARIH AS ITARIH, '0' AS HAREKETISLEM, EVREVRAKTARIHCE.ISLEMYAPANAD, EVREVRAK.KONU, EVREVRAK.GELDIGIYER,EVREVRAK.EVRAKTARIH, EVREVRAK.EVREVRAKNO, '' AS PREVRAKNO,EVREVRAKTARIHCE.ISLEM AS TARIHCEISLEM";
                sql += " FROM EVREVRAKTARIHCE INNER JOIN";
                sql += " EVREVRAK ON EVREVRAK.EVREVRAKNO = EVREVRAKTARIHCE.EVREVRAKNO";
                sql += " WHERE ";
                sql += "(" + tarihceKosul + ") ";
            }
            sql += "ORDER BY EVREVRAK.EVREVRAKNO,ITARIH";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader EvrakDosyalariniListele(EvrakSinif form)
        {
            string kosul = "";

            string sql = "SELECT EVREVRAK.EVREVRAKNO,EVREVRAK.EVRAKNO,EVREVRAK.EVRAKTARIH,EVREVRAK.KONU,EVREVRAK.GELDIGIYER,EVREVRAKDOSYA.DOSYAADI,EVREVRAKDOSYA.DOSYATARIHI,EVREVRAKDOSYA.DOSYABOYUTU ";
            sql += "FROM EVREVRAK INNER JOIN ";
            sql += "EVREVRAKDOSYA ON EVREVRAK.EVREVRAKNO=EVREVRAKDOSYA.EVREVRAKNO ";

            if (!form.evrakTarih.isNull)
            {
                if (form.sorguEvrakTarih.isNull)
                    form.sorguEvrakTarih = new TNSDateTime(form.evrakTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTARIH", ">=", form.evrakTarih));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTARIH", "<=", new TNSDateTime(form.sorguEvrakTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
            }

            if (!string.IsNullOrEmpty(form.geldigiYer) && form.geldigiYer.Length > 1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "GELDIGIYER", "LIKE", form.geldigiYer + "%"));

            if (!string.IsNullOrEmpty(form.tarihce.islemYapan))
            {
                kosul += " AND (" +
                    "EVREVRAK.EVREVRAKNO IN (SELECT EVREVRAKNO FROM EVREVRAKTARIHCE WHERE EVREVRAKTARIHCE.ISLEMYAPAN ='" + form.tarihce.islemYapan + "')"
                    + ")";
            }

            if (form.evrakNo != -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKNO", "=", form.evrakNo));

            if (form.evrakTipi != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAK", "EVRAKTIPI", "=", form.evrakTipi));

            if (form.evrDosya.dosyaBoyutu != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKDOSYA", "DOSYABOYUTU", ">=", form.evrDosya.dosyaBoyutu));

            if (form.evrDosya.dosyaBoyutuSorgu != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKDOSYA", "DOSYABOYUTU", "<=", form.evrDosya.dosyaBoyutuSorgu));

            if (!string.IsNullOrEmpty(form.evrDosya.dosyaAdi))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKDOSYA", "DOSYAADI", "LIKE", "%" + form.evrDosya.dosyaAdi));
            }

            sql += "WHERE ";
            sql += "(" + kosul + ") ";
            sql += "AND EVREVRAKDOSYA.IPTAL<>1 ";
            sql += "ORDER BY EVREVRAK.EVRAKNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DosyaTasnifPlaniRaporla(EvrakSinif form)
        {
            string kosul = "";

            string sql = "SELECT EVRDOSYAPLANI.KOD,EVRDOSYAPLANI.AD, COUNT(EVREVRAKHAREKETSONDURUM.PREVRAKNO) AS Sayi ";
            sql += "FROM EVREVRAKHAREKETSONDURUM INNER JOIN ";
            sql += "EVREVRAKDOSYAPLANI ON EVREVRAKDOSYAPLANI.PREVRAKNO=EVREVRAKHAREKETSONDURUM.PREVRAKNO INNER JOIN  ";
            sql += "EVRDOSYAPLANI ON EVRDOSYAPLANI.KOD=EVREVRAKDOSYAPLANI.DOSYAPLANKODU ";

            if (!form.hareket.islemTarih.isNull)
            {
                if (form.hareket.sorguIslemTarih.isNull)
                    form.hareket.sorguIslemTarih = new TNSDateTime(form.hareket.islemTarih.Oku());//Tarih alanında saat bilgisi olduğu için aralık sorgusu yapılması gerekli
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "ISLEMTARIH", ">=", form.hareket.islemTarih));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "ISLEMTARIH", "<=", new TNSDateTime(form.hareket.sorguIslemTarih.Oku().AddDays(1))));//tarih alanında saat bilgisi olduğu için içinde bulunduğumuz günü alması için
            }

            if (!string.IsNullOrEmpty(form.hareket.birimKod1))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKHAREKETSONDURUM", "BIRIMKOD1", "LIKE", form.hareket.birimKod1 + "%"));

            sql += "WHERE ";
            sql += "( EVREVRAKHAREKETSONDURUM.ISLEM=60 AND " + kosul + ") ";
            sql += " GROUP BY EVREVRAKDOSYAPLANI.DOSYAPLANKODU,EVRDOSYAPLANI.KOD,EVRDOSYAPLANI.AD  ORDER BY EVRDOSYAPLANI.KOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader EvrakImhaListele(TNSDateTime baslangicTarihi, TNSDateTime bitisTarihi)
        {
            string kosul = "";
            string sql = "SELECT EVREVRAKSICIL.PREVRAKNO, EVREVRAK.EVREVRAKNO, EVREVRAK.YIL, EVREVRAK.EVRAKTIPI, EVREVRAK.EVRAKNO, EVREVRAK.EVRAKTARIH, ";
            sql += "EVREVRAK.GELDIGIYER, EVREVRAK.KONU, EVREVRAK.GELENEVRAKTARIH, EVREVRAK.GELENEVRAKNO, EVRDOSYAPLANI.SAKLAMASURESI, DATEADD(year, ";
            sql += "EVRDOSYAPLANI.SAKLAMASURESI, EVREVRAK.EVRAKTARIH) AS SAKLAMABITISSURESI, EVREVRAKDOSYAPLANI.DOSYAPLANKODU, EVRDOSYAPLANI.AD ";
            sql += "FROM EVREVRAK INNER JOIN ";
            sql += "EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO INNER JOIN ";
            sql += "EVREVRAKDOSYAPLANI ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKDOSYAPLANI.PREVRAKNO INNER JOIN ";
            sql += "EVRDOSYAPLANI ON EVREVRAKDOSYAPLANI.DOSYAPLANKODU = EVRDOSYAPLANI.KOD ";
            //sql += "WHERE (DATEADD(year, EVRDOSYAPLANI.SAKLAMASURESI, EVREVRAK.EVRAKTARIH) >= CONVERT(DATETIME, '2005-01-01 00:00:00', 102)) AND (DATEADD(year, ";
            //sql += "EVRDOSYAPLANI.SAKLAMASURESI, EVREVRAK.EVRAKTARIH) < CONVERT(DATETIME, '2009-05-10 00:00:00', 102)) ";
            kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl("", "DATEADD(year, EVRDOSYAPLANI.SAKLAMASURESI, EVREVRAK.EVRAKTARIH)", ">=", baslangicTarihi));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "DATEADD(year, EVRDOSYAPLANI.SAKLAMASURESI, EVREVRAK.EVRAKTARIH)", "<=", bitisTarihi));
            sql += " WHERE " + kosul;

            sql += " ORDER BY EVRDOSYAPLANI.SAKLAMASURESI DESC, EVREVRAK.EVRAKNO DESC ";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader EvrakTasfiyeImhaListele(TNSDateTime baslangicTarihi, TNSDateTime bitisTarihi, EvrakTasfiyeImha form)
        {
            string kosul = "";
            string sql = "SELECT EVREVRAK.EVRAKBIRIMKODU, EVREVRAK.EVREVRAKNO, EVREVRAKSICIL.PREVRAKNO, EVREVRAK.YIL, EVREVRAK.EVRAKTIPI, EVREVRAK.EVRAKNO, EVREVRAK.EVRAKTARIH,";
            sql += " EVREVRAK.GELDIGIYER, EVREVRAK.KONU, EVREVRAKSAKLAMA.BITISTARIH, EVREVRAKSAKLAMA.RAPORTARIH, EVREVRAKSAKLAMA.TASFIYEPLANI, EVREVRAKSAKLAMA.ISLEM, EVREVRAKSAKLAMA.ISLEMTARIH,";
            sql += " EVREVRAKFIZIKSELYERI.BINA + ' / ' + EVREVRAKFIZIKSELYERI.KAT + ' / ' + EVREVRAKFIZIKSELYERI.ODA + ' / ' + EVREVRAKFIZIKSELYERI.BOLUM + ' / ' + EVREVRAKFIZIKSELYERI.RAF + ' / ' + EVREVRAKFIZIKSELYERI.KLASOR AS FIZIKSELYERI";
            sql += " FROM EVREVRAK INNER JOIN";
            sql += " EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO INNER JOIN";
            sql += " EVREVRAKSAKLAMA ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKSAKLAMA.PREVRAKNO LEFT OUTER JOIN";
            sql += " EVREVRAKFIZIKSELYERI ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKFIZIKSELYERI.PREVRAKNO ";

            kosul = VeriErisim.KosulEkle(kosul, "WHERE", VeriErisim.KosulOl("EVREVRAK", "EVRAKBIRIMKODU", "LIKE", form.evrakBirimKod + "%"));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKSICIL", "ASIL", "=", 1));
            if (!baslangicTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKSAKLAMA", "BITISTARIH", ">=", baslangicTarihi));
            if (!bitisTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKSAKLAMA", "BITISTARIH", "<=", bitisTarihi));
            if ((int)form.saklamaPlani.islem > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKSAKLAMA", "ISLEM", "=", form.saklamaPlani.islem));
            if (!form.saklamaPlani.bitisTarih.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKSAKLAMA", "BITISTARIH", "<", form.saklamaPlani.bitisTarih));

            sql += " WHERE " + kosul;
            sql += " ORDER BY EVREVRAKSAKLAMA.BITISTARIH DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public int EvrakTasfiyeImhaZamaniGelenEvrakSayisi(string evrakBirimKod, TNSDateTime bitisTarihi)
        {
            string sql = "SELECT COUNT(*) FROM EVREVRAK ";
            sql += " INNER JOIN EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO ";
            sql += " INNER JOIN EVREVRAKSAKLAMA ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKSAKLAMA.PREVRAKNO ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("EVREVRAK", "EVRAKBIRIMKODU", "LIKE", evrakBirimKod + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKSICIL", "ASIL", "=", 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKSAKLAMA", "ISLEM", "=", 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKSAKLAMA", "BITISTARIH", "<", bitisTarihi));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public string EvrEvrakNoGetir(string prEvrakNo)
        {
            string sql = "SELECT " + ALANLARSICIL[(int)ENUMAlanlarSicil.EVREVRAKNO] + " FROM " + TABLOADSICIL;
            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl("", ALANLARSICIL[(int)ENUMAlanlarSicil.PREVRAKNO], "=", prEvrakNo));
            return veriErisim.TekKolonSQLStr(sql);
        }

        public bool EvrakImzaliMi(string evrEvrakNo)
        {
            string sql = "SELECT IMZATARIHI FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "EVREVRAKNO", "=", evrEvrakNo));
            return veriErisim.TekKolonSQLDate(sql).isNull ? false : true;
        }

        public bool EditorKullaniliyorMu(string evrEvrakNo)
        {
            string sql = "SELECT ICERIK FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "EVREVRAKNO", "=", evrEvrakNo));
            return !string.IsNullOrWhiteSpace(veriErisim.TekKolonSQLStr(sql));
        }

        //public Sonuc EditorGuncellenmeliDurumunuDegistir(string evrEvrakNo, bool guncellenmeli)
        //{
        //    string sql = "UPDATE EVREVRAK SET EDITORGUNCELLENMELI=" + (guncellenmeli ? 1 : 0);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "EVREVRAKNO", "=", evrEvrakNo));

        //    return KomutCalistir(sql, 1);
        //}

        public IDataReader EvrakEImzaBilgiListele(string evrEvrakNo)
        {
            string sql = "SELECT EIMZALANDI, EIMZALAYAN, EIMZATARIHI FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "EVREVRAKNO", "=", evrEvrakNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc EtiketKaydet(Etiket bilgi)
        {
            string sql = "INSERT INTO " + TABLOADETIKETTANIM + " (" + VeriErisim.TumAlanlar("", ALANLARETIKETTANIM) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ad) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ustKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.sahibi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.detay) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc EtiketGuncelle(Etiket bilgi)
        {
            string sql = "UPDATE " + TABLOADETIKETTANIM + " SET ";

            if (bilgi.ad != "")
                sql += VeriErisim.AtamaOl("", ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.AD], bilgi.ad);

            if (bilgi.ustKod > -1)
                sql += VeriErisim.AtamaOl("", ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.USTKOD], bilgi.ustKod);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc EtiketGuncelleDetay(string sahibi)
        {
            string sql = "UPDATE EVRETIKET SET DETAY=(SELECT COUNT(*) FROM EVRETIKET B WHERE B.USTKOD=EVRETIKET.KOD)";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.SAHIBI], "=", sahibi));

            return KomutCalistir(sql, 0);
        }

        public Sonuc EtiketSil(Etiket bilgi)
        {
            string sql = "DELETE FROM " + TABLOADETIKETTANIM + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.KOD], "=", bilgi.kod);

            return KomutCalistir(sql, 1);
        }

        public IDataReader EtiketListele(Etiket bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADETIKETTANIM, ALANLARETIKETTANIM) + " FROM " + TABLOADETIKETTANIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADETIKETTANIM, ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.SAHIBI], "=", bilgi.sahibi));
            if (!string.IsNullOrEmpty(bilgi.sahibi2))
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOADETIKETTANIM, ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.SAHIBI], "=", bilgi.sahibi2));
            if (bilgi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADETIKETTANIM, ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.KOD], "=", bilgi.kod));
            if (bilgi.ustKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADETIKETTANIM, ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.USTKOD], "=", bilgi.ustKod));

            sql += " ORDER BY " + ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.USTKOD] + "," + ALANLARETIKETTANIM[(int)ENUMAlanEtiketTanim.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc EvrakEtiketKaydet(EvrakEtiket bilgi)
        {
            string sql = "INSERT INTO " + TABLOADETIKET + " (" + VeriErisim.TumAlanlar("", ALANLARETIKET) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.etiketKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.prEvrakNo) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc EvrakEtiketSil(EvrakEtiket bilgi)
        {
            string sql = "DELETE FROM " + TABLOADETIKET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADETIKET, ALANLARETIKET[(int)ENUMAlanEtiket.ETIKETKOD], "=", bilgi.etiketKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADETIKET, ALANLARETIKET[(int)ENUMAlanEtiket.PREVRAKNO], "=", bilgi.prEvrakNo));

            return KomutCalistir(sql, 1);
        }

        public bool EvrakDosyaTasnifPlaniMevcutMu(string prEvrakNo, string dosyaTasnifPlanKodlari)
        {
            string sql = "SELECT COUNT(*) FROM EVREVRAKDOSYAPLANI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "PREVRAKNO", "=", prEvrakNo));

            string ekKosul = "";
            string[] tmpListe = dosyaTasnifPlanKodlari.TrimEnd(';').Split(';');
            for (int i = 0; i < tmpListe.Length; i++)
            {
                if (i > 0) ekKosul += " OR ";
                ekKosul += " DOSYAPLANKODU =" + VeriErisim.DegerYap(tmpListe[i]);
            }
            sql += " AND (" + ekKosul + ")";

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        public IDataReader ListeleDosyayaKaldirilmisEvraklar(EvrakSinif bilgi)
        {
            string sql = "SELECT DISTINCT EVREVRAK.EVREVRAKNO,EVREVRAK.EVRAKNO,EVREVRAK.EVRAKTARIH,EVREVRAK.KONU,EVREVRAK.ARSIVLEMETARIHI,EVREVRAKTASFIYE.TASFIYEKOD,EVREVRAKSAKLAMA.BITISTARIH ";
            sql += "FROM " + TABLOAD;
            sql += " INNER JOIN EVREVRAKSICIL ON EVREVRAKSICIL.EVREVRAKNO=EVREVRAK.EVREVRAKNO";
            sql += " INNER JOIN EVREVRAKTASFIYE ON EVREVRAKTASFIYE.PREVRAKNO=EVREVRAKSICIL.PREVRAKNO";
            sql += " INNER JOIN EVREVRAKSAKLAMA ON EVREVRAKSAKLAMA.PREVRAKNO=EVREVRAKSICIL.PREVRAKNO";
            sql += " INNER JOIN EVREVRAKHAREKETSONDURUM ON EVREVRAKHAREKETSONDURUM.PREVRAKNO=EVREVRAKSICIL.PREVRAKNO WHERE EVREVRAKHAREKETSONDURUM.ISLEM=60";

            if (bilgi.tasfiye.tasfiyeKod != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKTASFIYE", "TASFIYEKOD", "=", bilgi.tasfiye.tasfiyeKod));
            if (!bilgi.saklama.bitisTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVREVRAKSAKLAMA", "BITISTARIH", "<=", bilgi.saklama.bitisTarih));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.EVRAKNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public bool HavalePersonelIzinKontrol(string tcKimlikNo, string kontrolTur)
        {
            TNSDateTime tarih = new TNSDateTime(DateTime.Now.ToShortDateString());

            string sql = "SELECT  COUNT(*) PERPERSONEL";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "TCKIMLIKNO", "=", tcKimlikNo));
            sql += " AND ";

            //string sqlIzin = " (CONVERT(DATETIME, '2014-07-17 00:00:00', 102) >= IZINBASLANGICTARIHI ) AND (CONVERT(DATETIME, '2014-07-17 00:00:00', 102) < IZINBITISTARIHI) ";
            //string sqlGorev = " (CONVERT(DATETIME, '2014-07-17 00:00:00', 102) >= GOREVBASLANGICTARIH ) AND (CONVERT(DATETIME, '2014-07-17 00:00:00', 102) < GOREVBITISTARIHI) ";

            string sqlIzin = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("", "IZINBASLANGICTARIHI", "<=", tarih));
            sqlIzin = VeriErisim.KosulEkle(sqlIzin, "AND", VeriErisim.KosulOl("", "IZINBITISTARIHI", ">", tarih));

            string sqlGorev = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("", "GOREVBASLANGICTARIH", "<=", tarih));
            sqlGorev = VeriErisim.KosulEkle(sqlGorev, "AND", VeriErisim.KosulOl("", "GOREVBITISTARIHI", ">", tarih));

            switch (kontrolTur)
            {
                case "1": //İzin Kontrol Et 
                    sql += sqlIzin;
                    break;
                case "2": //Görev Kontrol Et 
                    sql += sqlGorev;
                    break;
                case "3": //İzin ve Görev Kontrol Et 
                    sql += "(" + sqlIzin + " OR " + sqlGorev + ")";
                    break;
            }

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }

        public IDataReader ListeleSicil(EvrakSicil form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSICIL, ALANLARSICIL) + " FROM " + TABLOADSICIL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSICIL, ALANLARSICIL[(int)ENUMAlanlarSicil.EVREVRAKNO], "=", form.evrEvrakNo));
            sql += " ORDER BY " + ALANLARSICIL[(int)ENUMAlanlarSicil.PREVRAKNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SiraNoDegistir(string primaryKey, int siraNo1, int siraNo2, ENUMSiraNoDegistirTablo tabloAd)
        {
            Sonuc sonuc = new Sonuc(false, "İşlem Yapılamadı.", "");
            string sql = "";

            switch (tabloAd)
            {
                case ENUMSiraNoDegistirTablo.EVREVRAKDOSYA:
                    {
                        string key = "EVREVRAKNO";

                        sql = "UPDATE " + tabloAd + " SET SIRANO = 5000 WHERE (" + key + " = " + VeriErisim.DegerYap(primaryKey) + ") AND (SIRANO = " + VeriErisim.DegerYap(siraNo1) + ")";
                        sonuc = KomutCalistir(sql, 1);
                        if (sonuc.islemSonuc)
                        {
                            sql = "UPDATE " + tabloAd + " SET SIRANO = " + VeriErisim.DegerYap(siraNo1) + " WHERE (" + key + " = " + VeriErisim.DegerYap(primaryKey) + ") AND (SIRANO = " + VeriErisim.DegerYap(siraNo2) + ")";
                            sonuc = KomutCalistir(sql, 1);

                            if (sonuc.islemSonuc)
                            {
                                sql = "UPDATE " + tabloAd + " SET SIRANO = " + VeriErisim.DegerYap(siraNo2) + " WHERE (" + key + " = " + VeriErisim.DegerYap(primaryKey) + ") AND (SIRANO = 5000)";
                                sonuc = KomutCalistir(sql, 1);
                            }
                        }
                    }
                    break;
                case ENUMSiraNoDegistirTablo.INFOCOMDOSYAIMZA:
                    {
                        string key = "EVREVRAKNO";

                        sql = "UPDATE " + tabloAd + " SET SIRANO = 5000 WHERE (" + key + " = " + VeriErisim.DegerYap(primaryKey) + ") AND (SIRANO = " + VeriErisim.DegerYap(siraNo1) + ")";
                        sonuc = KomutCalistir(sql, 0);

                        sql = "UPDATE " + tabloAd + " SET SIRANO = " + VeriErisim.DegerYap(siraNo1) + " WHERE (" + key + " = " + VeriErisim.DegerYap(primaryKey) + ") AND (SIRANO = " + VeriErisim.DegerYap(siraNo2) + ")";
                        sonuc = KomutCalistir(sql, 0);

                        sql = "UPDATE " + tabloAd + " SET SIRANO = " + VeriErisim.DegerYap(siraNo2) + " WHERE (" + key + " = " + VeriErisim.DegerYap(primaryKey) + ") AND (SIRANO = 5000)";
                        sonuc = KomutCalistir(sql, 0);

                    }
                    break;

            }

            return sonuc;
        }


    }
}

using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Onay form belge bilgisini tutan HRCIHALEONAYFORM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOnayForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOnayForm(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Onay form belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCIHALEONAYFORM";

        /// <summary>
        /// Onay form belge bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// IDAREADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IDAREADI = 4,
            /// <summary>
            /// BELGETARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETARIH = 5,
            /// <summary>
            /// EVRAKSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVRAKSAYI = 6,
            /// <summary>
            /// ISTANIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISTANIM = 7,
            /// <summary>
            /// ISNITELIK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISNITELIK = 8,
            /// <summary>
            /// ISMIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISMIKTAR = 9,
            /// <summary>
            /// ODENEKYILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKYILI = 10,
            /// <summary>
            /// YAKLASIKMALIYET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYET = 11,
            /// <summary>
            /// YMPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMPARABIRIMI = 12,
            /// <summary>
            /// YMTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YMTIP = 13,
            /// <summary>
            /// DOKUMANBEDELI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANBEDELI = 14,
            /// <summary>
            /// BUTCETERTIBI1 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BUTCETERTIBI1 = 15,
            /// <summary>
            /// BUTCETERTIBI2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BUTCETERTIBI2 = 16,
            /// <summary>
            /// ODENEKTUTAR1 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKTUTAR1 = 17,
            /// <summary>
            /// ODENEKTUTAR2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKTUTAR2 = 18,
            /// <summary>
            /// PROJENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PROJENO = 19,
            /// <summary>
            /// AVANSVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AVANSVARMI = 20,
            /// <summary>
            /// AVANSSARTLARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AVANSSARTLARI = 21,
            /// <summary>
            /// IHALEUSULU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEUSULU = 22,
            /// <summary>
            /// IHALEMADDESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEMADDESI = 23,
            /// <summary>
            /// ILANVERILECEKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANVERILECEKMI = 24,
            /// <summary>
            /// ILANSEKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILANSEKLI = 25,
            /// <summary>
            /// ONYETERLILIK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONYETERLILIK = 26,
            /// <summary>
            /// FIYATFARKIVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIYATFARKIVARMI = 27,
            /// <summary>
            /// FIYATFARKI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIYATFARKI = 28,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 29,
            /// <summary>
            /// GERCEKGOREVLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GERCEKGOREVLI = 30,
            /// <summary>
            /// GERCEKGOREVLIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GERCEKGOREVLIUNVAN = 31,
            /// <summary>
            /// ONAYASUNUMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYASUNUMTARIHI = 32,
            /// <summary>
            /// IHALEYETKILISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEYETKILISI = 33,
            /// <summary>
            /// IHALEYETKILISIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEYETKILISIUNVAN = 34,
            /// <summary>
            /// ONAYLAMATARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONAYLAMATARIHI = 35,
            /// <summary>
            /// IHALEMAKAMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEMAKAMI = 36,
            /// <summary>
            /// KOMISYONVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOMISYONVARMI = 37,
            /// <summary>
            /// HTGSURESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTGSURESI = 38,
            /// <summary>
            /// HTGUSULU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTGUSULU = 39,
            /// <summary>
            /// HTGGOREVLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTGGOREVLI = 40,
            /// <summary>
            /// HTMIHMANDARVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTMIHMANDARVARMI = 41,
            /// <summary>
            /// HTPERTCNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HTPERTCNO = 42,
            /// <summary>
            /// HRCTALIMAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HRCTALIMAT = 43,
            /// <summary>
            /// ONILANYILCEYREK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANYILCEYREK = 44,
            /// <summary>
            /// ONILANGERCEKGOREVLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANGERCEKGOREVLI = 45,
            /// <summary>
            /// ONILANGERCEKGOREVLIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANGERCEKGOREVLIUNVAN = 46,
            /// <summary>
            /// ONILANIHALEYETKILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANIHALEYETKILI = 47,
            /// <summary>
            /// ONILANIHALEYETKILIUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANIHALEYETKILIUNVAN = 48,
            /// <summary>
            /// ONILANONAYLAMATARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANONAYLAMATARIHI = 49,
            /// <summary>
            /// ONILANONAYASUNUMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANONAYASUNUMTARIHI = 50,
            /// <summary>
            /// ONILANVARMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ONILANVARMI = 51,
            /// <summary>
            /// DOKUMANPOSTASATIS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANPOSTASATIS = 52,
            /// <summary>
            /// DOKUMANYURTICIBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANYURTICIBEDEL = 53,
            /// <summary>
            /// DOKUMANYURTDISIBEDEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANYURTDISIBEDEL = 54,
        }

        /// <summary>
        /// Onay form belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "IDAREADI", "BELGETARIH",
        "EVRAKSAYI", "ISTANIM", "ISNITELIK", "ISMIKTAR", "ODENEKYILI", "YAKLASIKMALIYET", "YMPARABIRIMI", 
        "YMTIP", "DOKUMANBEDELI", "BUTCETERTIBI1", "BUTCETERTIBI2", "ODENEKTUTAR1", "ODENEKTUTAR2", 
        "PROJENO", "AVANSVARMI", "AVANSSARTLARI", "IHALEUSULU", "IHALEMADDESI", "ILANVERILECEKMI", 
        "ILANSEKLI", "ONYETERLILIK", "FIYATFARKIVARMI", "FIYATFARKI", "ACIKLAMA", "GERCEKGOREVLI", 
        "GERCEKGOREVLIUNVAN", "ONAYASUNUMTARIHI", "IHALEYETKILISI", "IHALEYETKILISIUNVAN", "ONAYLAMATARIHI", 
        "IHALEMAKAMI", "KOMISYONVARMI", "HTGSURESI","HTGUSULU","HTGGOREVLI","HTMIHMANDARVARMI","HTPERTCNO","HRCTALIMATI",
        "ONILANYILCEYREK","ONILANGERCEKGOREVLI","ONILANGERCEKGOREVLIUNVAN","ONILANIHALEYETKILI","ONILANIHALEYETKILIUNVAN",
        "ONILANONAYLAMATARIHI","ONILANONAYASUNUMTARIHI","ONILANVARMI","DOKUMANPOSTASATIS","DOKUMANYURTICIBEDEL","DOKUMANYURTDISIBEDEL"};

        /// <summary>
        /// Onay form tertip bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTERTIP = "HRCIHALEONAYTERTIP";

        /// <summary>
        /// Onay form tertip bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTertip
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 3,
            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 4,
            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 5,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 6,
            /// <summary>
            /// ODENEKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEKTUTAR = 7,
            /// <summary>
            /// BLOKETUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BLOKETUTAR = 8,
            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 9,
            /// <summary>
            /// KULLANILACAKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANILACAKTUTAR = 10,
            /// <summary>
            /// TERTIPYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERTIPYIL = 11,
            /// <summary>
            /// KDVTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVTUTAR = 12,
            /// <summary>
            /// KDVLISOZLESMETUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDVLISOZLESMETUTAR = 13,
        }

        /// <summary>
        /// Onay form tertip tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTERTIP = { "YIL", "SURECNO", "SIRANO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEKTUTAR", "BLOKETUTAR", "PEBKOD", "KULLANILACAKTUTAR", "TERTIPYIL", "KDVTUTAR", "KDVLISOZLESMETUTAR" };

        /// <summary>
        /// Süreç bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSUREC = "HRCSUREC";

        /// <summary>
        /// Süreç bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSurec
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// ISLEMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMKOD = 4,
            /// <summary>
            /// SURECTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECTARIH = 5,
            /// <summary>
            /// SURECDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECDURUM = 6,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 7,
            /// <summary>
            /// ISINADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISINADI = 8,
        }

        /// <summary>
        /// Süreç tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSUREC = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "ISLEMKOD", "SURECTARIH", "SURECDURUM", "ACIKLAMA", "ISINADI" };

        /// <summary>
        /// Onay form belge bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek onay form belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(OnayForm bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR);
            if (bilgi.harcGorevliVar)
                sql += ", HARCGOREVAD, HARCGOREVUNVAN";
            sql += ") VALUES (";

            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.idareAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrakSayi) + ",";
            sql += VeriErisim.DegerYap(bilgi.isTanimi) + ",";
            sql += VeriErisim.DegerYap(bilgi.isNiteligi) + ",";
            sql += VeriErisim.DegerYap(bilgi.isMiktari) + ",";
            sql += VeriErisim.DegerYap(bilgi.odenekYili) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyet) + ",";
            sql += VeriErisim.DegerYap(bilgi.yMParaBirimi) + ",";
            sql += VeriErisim.DegerYap(bilgi.yMTip) + ",";
            sql += VeriErisim.DegerYap(bilgi.dokumanBedeli) + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap(0.0) + ",";
            sql += VeriErisim.DegerYap(0.0) + ",";
            sql += VeriErisim.DegerYap(bilgi.projeNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.avansVarmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.avansSartlari) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleUsulu) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleMaddesi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilanVerilecekmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilanSekli) + ",";
            sql += VeriErisim.DegerYap(bilgi.onYeterlilik) + ",";
            sql += VeriErisim.DegerYap(bilgi.fiyatFarkiVarmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.fiyatFarki) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.gercekGorevli) + ",";
            sql += VeriErisim.DegerYap(bilgi.gercekGorevliUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onayaSunumTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleYetkili) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleYetkiliUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onaylamaTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleMakami) + ",";
            sql += VeriErisim.DegerYap(bilgi.komisyonVarmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.hTgerceklestirmeSuresi) + ",";
            sql += VeriErisim.DegerYap(bilgi.hTgerceklestirmeUsulu) + ",";
            sql += VeriErisim.DegerYap(bilgi.hTgerceklestirmeGorevli) + ",";
            sql += VeriErisim.DegerYap(bilgi.hTmihmandarlikVarmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.hTperTcNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.hrcTalimati) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanYilCeyrek) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanGercekGorevli) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanGercekGorevliUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanIhaleYetkili) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanIhaleYetkiliUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanOnaylamaTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanOnayaSunumTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanVarMi) + ",";
            sql += VeriErisim.DegerYap(bilgi.dokumanPostaSatisVarMi) + ",";
            sql += VeriErisim.DegerYap(bilgi.dokumanYurticiBedel) + ",";
            sql += VeriErisim.DegerYap(bilgi.dokumanYurtdisiBedel);

            if (bilgi.harcGorevliVar)
            {
                sql += "," + VeriErisim.DegerYap(bilgi.harcGorevAd);
                sql += "," + VeriErisim.DegerYap(bilgi.harcGorevUnvan);
            }
            sql += ")";
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIH], bilgi.belgeTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISTANIM], bilgi.isTanimi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISNITELIK], bilgi.isNiteligi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISMIKTAR], bilgi.isMiktari) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENEKYILI], bilgi.odenekYili) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYET], bilgi.yaklasikMaliyet) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YMPARABIRIMI], bilgi.yMParaBirimi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YMTIP], bilgi.yMTip) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANBEDELI], bilgi.dokumanBedeli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCETERTIBI1], "") + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCETERTIBI2], "") + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENEKTUTAR1], 0.0) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENEKTUTAR2], 0.0) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], bilgi.projeNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AVANSVARMI], bilgi.avansVarmi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AVANSSARTLARI], bilgi.avansSartlari) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEUSULU], bilgi.ihaleUsulu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEMADDESI], bilgi.ihaleMaddesi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILANVERILECEKMI], bilgi.ilanVerilecekmi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILANSEKLI], bilgi.ilanSekli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONYETERLILIK], bilgi.onYeterlilik) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIYATFARKIVARMI], bilgi.fiyatFarkiVarmi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIYATFARKI], bilgi.fiyatFarki) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IDAREADI], bilgi.idareAdi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKSAYI], bilgi.evrakSayi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERCEKGOREVLI], bilgi.gercekGorevli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERCEKGOREVLIUNVAN], bilgi.gercekGorevliUnvan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYASUNUMTARIHI], bilgi.onayaSunumTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEYETKILISI], bilgi.ihaleYetkili) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEYETKILISIUNVAN], bilgi.ihaleYetkiliUnvan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYLAMATARIHI], bilgi.onaylamaTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEMAKAMI], bilgi.ihaleMakami) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOMISYONVARMI], bilgi.komisyonVarmi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HTGGOREVLI], bilgi.hTgerceklestirmeGorevli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HTGSURESI], bilgi.hTgerceklestirmeSuresi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HTGUSULU], bilgi.hTgerceklestirmeUsulu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HTMIHMANDARVARMI], bilgi.hTmihmandarlikVarmi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HTPERTCNO], bilgi.hTperTcNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HRCTALIMAT], bilgi.hrcTalimati) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONILANYILCEYREK], bilgi.onIlanYilCeyrek) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONILANGERCEKGOREVLI], bilgi.onIlanGercekGorevli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONILANGERCEKGOREVLIUNVAN], bilgi.onIlanGercekGorevliUnvan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONILANIHALEYETKILI], bilgi.onIlanIhaleYetkili) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONILANIHALEYETKILIUNVAN], bilgi.onIlanIhaleYetkiliUnvan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONILANONAYLAMATARIHI], bilgi.onIlanOnaylamaTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONILANONAYASUNUMTARIHI], bilgi.onIlanOnayaSunumTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONILANVARMI], bilgi.onIlanVarMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANPOSTASATIS], bilgi.dokumanPostaSatisVarMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANYURTICIBEDEL], bilgi.dokumanYurticiBedel) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANYURTDISIBEDEL], bilgi.dokumanYurtdisiBedel);
                if (bilgi.harcGorevliVar)
                {
                    sql += "," + VeriErisim.AtamaOl("", "HARCGOREVAD", bilgi.harcGorevAd);
                    sql += "," + VeriErisim.AtamaOl("", "HARCGOREVUNVAN", bilgi.harcGorevUnvan);
                }
                kosul = VeriErisim.KosulEkle(" WHERE", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

                sql += kosul;

                sonuc = KomutCalistir(sql, 1);
            }


            return sonuc;
        }

        /// <summary>
        /// Onay form belgesi evrak sayı bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek onay form belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc EvrakSayiKaydet(OnayForm bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";

            veriErisim.hata = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKSAYI], bilgi.evrakSayi);

            kosul = VeriErisim.KosulEkle(" WHERE", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

            sql += kosul;

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Onay form belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak onay form belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Onay form belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(OnayForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            if (bilgi.harcGorevliVar)
                sql += ", HARCGOREVAD, HARCGOREVUNVAN";
            sql += " FROM " + TABLOAD;
            if (bilgi.yil != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.avansVarmi > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AVANSVARMI], "=", bilgi.avansVarmi));
            if (bilgi.hrcTalimati > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HRCTALIMAT], "=", bilgi.hrcTalimati));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SURECNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Onay form belge tertip bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak onay form belge tertip bilgisi kıstas parametresi</param>
        /// <returns>
        /// Onay form belge tertip bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader TertipListele(OnayFormTertip bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTERTIP, ALANLARTERTIP) + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD] + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD] + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.ISINADI];
            sql += " FROM " + TABLOADTERTIP;
            sql += " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND " +
                   TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], ">", 0));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tertipYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            if (bilgi.kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "=", bilgi.kurKod.Replace(".", "")));
            if (bilgi.fonKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "=", bilgi.fonKod.Replace(".", "")));
            if (bilgi.finKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "=", bilgi.finKod.Replace(".", "")));
            if (bilgi.ekoKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "=", bilgi.ekoKod.Replace(".", "")));
            if (bilgi.pebKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD], "=", bilgi.birim));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD] + "," +
                                  TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD] + "," +
                                  TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BlokeTertipListele(OnayFormTertip bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTERTIP, ALANLARTERTIP) + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD] + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD] + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.ISINADI] + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM];
            sql += " FROM " + TABLOADTERTIP;
            sql += " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND " +
                   TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], ">", 0));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tertipYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            if (bilgi.kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "LIKE", bilgi.kurKod.Replace(".", "") + "%"));
            if (bilgi.fonKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "LIKE", bilgi.fonKod.Replace(".", "") + "%"));
            if (bilgi.finKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "LIKE", bilgi.finKod.Replace(".", "") + "%"));
            if (bilgi.ekoKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "LIKE", bilgi.ekoKod.Replace(".", "") + "%"));
            if (bilgi.pebKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "LIKE", bilgi.pebKod.Replace(".", "") + "%"));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD], "=", bilgi.birim));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD] + "," +
                                  TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD] + "," +
                                  TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Onay form belge tertip bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak onay form belge tertip bilgisi kıstas parametresi</param>
        /// <returns>
        /// Onay form belge tertip bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader TertipListeleBenzer(OnayFormTertip bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTERTIP, ALANLARTERTIP) + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD] + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD] + "," +
                TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.ISINADI];
            sql += " FROM " + TABLOADTERTIP;
            sql += " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND " +
                   TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], ">", 0));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tertipYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            if (bilgi.kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "LIKE", bilgi.kurKod.Replace(".", "") + "%"));
            if (bilgi.fonKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "LIKE", bilgi.fonKod.Replace(".", "") + "%"));
            if (bilgi.finKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "LIKE", bilgi.finKod.Replace(".", "") + "%"));
            if (bilgi.ekoKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "LIKE", bilgi.ekoKod.Replace(".", "") + "%"));
            if (bilgi.pebKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "LIKE", bilgi.pebKod.Replace(".", "") + "%"));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD], "=", bilgi.birim));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD] + "," +
                                  TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD] + "," +
                                  TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Onay form belge tertip bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Silinecek yıl bilgisini tutan nesne</param>
        /// <param name="surecNo">Silinecek süreç numarası bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc TertipSil(int yil, string surecNo)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", surecNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Onay form belge tertip bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Silinecek yıl bilgisini tutan nesne</param>
        /// <param name="surecNo">Silinecek süreç numarası bilgisini tutan nesne</param>
        /// <param name="tertipYil">Silinecek tertiplere ait yıl bilgisi</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc TertipSil(int yil, string surecNo, int tertipYil)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", tertipYil));

            sqlUretilen = sql;

            return KomutCalistir(sql, 0);
        }

        public string sqlUretilen;

        /// <summary>
        /// Onay form belge tertip bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek onay form belge tertip bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc TertipKaydet(OnayFormTertip bilgi)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOADTERTIP + " (" + VeriErisim.TumAlanlar("", ALANLARTERTIP) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.fonKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.finKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.ekoKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.odenekTutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.blokeTutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.pebKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.kullanilacakTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.tertipYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.kdvTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeKdvliTutar) + ")";

            sqlUretilen = sql;

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Onay form belge tertip bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Güncellenecek onay form belge tertip bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc TertipOnayGuncelle(OnayFormTertip bilgi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOADTERTIP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], bilgi.blokeTutari) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KULLANILACAKTUTAR], bilgi.kullanilacakTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.ODENEKTUTAR], bilgi.odenekTutari) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KDVTUTAR], bilgi.kdvTutar) + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "=", bilgi.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "=", bilgi.fonKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "=", bilgi.finKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "=", bilgi.ekoKod.Replace(".", "")));
            if (bilgi.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SIRANO], "=", bilgi.siraNo));
            if (bilgi.pebKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));

            sqlUretilen = sql;

            return KomutCalistir(sql, 1);
        }

        public Sonuc TertipGuncelle(OnayFormTertip bilgi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOADTERTIP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], bilgi.blokeTutari) + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "=", bilgi.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "=", bilgi.fonKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "=", bilgi.finKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "=", bilgi.ekoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SIRANO], "=", bilgi.siraNo));
            if (bilgi.pebKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public Sonuc TertipSozlesmeGuncelle(OnayFormTertip bilgi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOADTERTIP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KDVLISOZLESMETUTAR], bilgi.sozlesmeKdvliTutar) + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "=", bilgi.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "=", bilgi.fonKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "=", bilgi.finKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "=", bilgi.ekoKod.Replace(".", "")));
            if (bilgi.pebKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));

            //if (bilgi.siraNo > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SIRANO], "=", bilgi.siraNo));
            //if (bilgi.kullanilacakTutar > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KULLANILACAKTUTAR], "=", bilgi.kullanilacakTutar));

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Onay form belge tertip bloke bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Güncellenecek onay form belge tertip bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc TertipBlokeGuncelle(OnayFormTertip bilgi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOADTERTIP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], bilgi.blokeTutari) + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SIRANO], "=", bilgi.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "=", bilgi.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "=", bilgi.fonKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "=", bilgi.finKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "=", bilgi.ekoKod.Replace(".", "")));
            if (bilgi.pebKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));
            //if (bilgi.kullanilacakTutar > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KULLANILACAKTUTAR], "=", bilgi.kullanilacakTutar));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Onay form belge tertip üzerindeki bloke miktarı bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Güncellenecek onay form belge tertip bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc TertipBlokeKaldir(OnayFormTertip bilgi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOADTERTIP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], bilgi.blokeTutari) + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tertipYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            if (bilgi.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SIRANO], "=", bilgi.siraNo));
            if (bilgi.kurKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "=", bilgi.kurKod.Replace(".", "")));
            if (bilgi.fonKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "=", bilgi.fonKod.Replace(".", "")));
            if (bilgi.finKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "=", bilgi.finKod.Replace(".", "")));
            if (bilgi.ekoKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "=", bilgi.ekoKod.Replace(".", "")));
            if (bilgi.pebKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public Sonuc TertipBlokeEkle(OnayFormTertip bilgi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOADTERTIP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.BLOKETUTAR], bilgi.blokeTutari) + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tertipYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.TERTIPYIL], "=", bilgi.tertipYil));
            if (bilgi.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.SIRANO], "=", bilgi.siraNo));
            if (bilgi.kurKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.KURKOD], "=", bilgi.kurKod.Replace(".", "")));
            if (bilgi.fonKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FONKOD], "=", bilgi.fonKod.Replace(".", "")));
            if (bilgi.finKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.FINKOD], "=", bilgi.finKod.Replace(".", "")));
            if (bilgi.ekoKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.EKOKOD], "=", bilgi.ekoKod.Replace(".", "")));
            if (bilgi.pebKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTertip.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Onay belgesi harcama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="ay">Listeleme için kullanılacak ay kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        /// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <param name="peb">Listeleme için kullanılacak alt faaliyet kod kıstas parametresi</param>
        /// <returns>
        /// Onay belgesi harcama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleButceHazirlikOnayBelgesiHarcama(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            string sql = string.Empty;

            string groupbyAlanlar = string.Empty;

            sql = "SELECT YEAR(HRCIHALEONAYFORM.BELGETARIH) AS ISLEMYIL, MONTH(HRCIHALEONAYFORM.BELGETARIH) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL, " +
                  " HRCIHALEONAYFORM.SURECNO, HRCIHALEONAYFORM.MUHASEBEKOD, HRCIHALEONAYFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, " +
                  " HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, " +
                  " SUM(HRCIHALEONAYTERTIP.KULLANILACAKTUTAR) AS TOPLAM, HRCSUREC.YILLARASARIISLEMMI " +

                  " FROM         HRCIHALEONAYFORM LEFT OUTER JOIN " +
                    "  HRCIHALEONAYTERTIP ON HRCIHALEONAYFORM.YIL = HRCIHALEONAYTERTIP.YIL AND " +
                     " HRCIHALEONAYFORM.SURECNO = HRCIHALEONAYTERTIP.SURECNO LEFT OUTER JOIN " +
                     " HRCSUREC ON HRCIHALEONAYFORM.YIL = HRCSUREC.YIL AND HRCIHALEONAYFORM.SURECNO = HRCSUREC.SURECNO " +

                  " WHERE HRCSUREC.SURECDURUM <> 9 ";

            if (yil > 0)
            {
                groupbyAlanlar += " HRCIHALEONAYFORM.YIL,";
                sql += " AND (YEAR(HRCIHALEONAYFORM.BELGETARIH) = " + yil + ") ";
            }
            if (ay > 0)
            {
                sql += " AND (MONTH(HRCIHALEONAYFORM.BELGETARIH) = " + ay + ") ";
            }
            if (muhasebe > 0)
            {
                groupbyAlanlar += " HRCIHALEONAYFORM.MUHASEBEKOD,";
                sql += " AND (HRCIHALEONAYFORM.MUHASEBEKOD = " + muhasebe + ") ";
            }
            if (!string.IsNullOrEmpty(birim))
            {
                groupbyAlanlar += " HRCIHALEONAYFORM.BIRIMKOD,";
                sql += " AND (HRCIHALEONAYFORM.BIRIMKOD = '" + birim + "') ";
            }
            if (!string.IsNullOrEmpty(kur))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.KURKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.KURKOD = '" + kur.Replace(".", "") + "') ";
            }
            if (!string.IsNullOrEmpty(fon))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.FONKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.FONKOD = '" + fon.Replace(".", "") + "') ";
            }
            if (!string.IsNullOrEmpty(fin))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.FINKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.FINKOD = '" + fin.Replace(".", "") + "') ";
            }
            if (!string.IsNullOrEmpty(eko))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.EKOKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.EKOKOD = '" + eko.Replace(".", "") + "') ";
            }
            if (!string.IsNullOrEmpty(peb))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.PEBKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.PEBKOD = '" + peb.Replace(".", "") + "') ";
            }

            sql += " GROUP BY YEAR(HRCIHALEONAYFORM.BELGETARIH), MONTH(HRCIHALEONAYFORM.BELGETARIH), HRCIHALEONAYFORM.SURECNO, " +
                      " HRCIHALEONAYFORM.MUHASEBEKOD, HRCIHALEONAYFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, " +
                      " HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL " +
                    " ORDER BY ISLEMYIL DESC, ISLEMAY DESC, HRCIHALEONAYFORM.SURECNO, HRCIHALEONAYFORM.MUHASEBEKOD, HRCIHALEONAYFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, " +
                      " HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Onay belgesi dmo harcama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="ay">Listeleme için kullanılacak ay kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        /// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <param name="peb">Listeleme için kullanılacak alt faaliyet kod kıstas parametresi</param>
        /// <returns>
        /// Onay belgesi dmo harcama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleButceHazirlikOnayBelgesiHarcamaDmo(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            string sql = string.Empty;

            string groupbyAlanlar = string.Empty;

            sql = "SELECT YEAR(HRCIHALEONAYFORM.BELGETARIH) AS ISLEMYIL, MONTH(HRCIHALEONAYFORM.BELGETARIH) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL, " +
                      " HRCIHALEONAYFORM.SURECNO, HRCIHALEONAYFORM.MUHASEBEKOD, HRCIHALEONAYFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, " +
                      " HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, " +
                      " SUM(HRCIHALEONAYTERTIP.KULLANILACAKTUTAR) AS TOPLAM, HRCSUREC.YILLARASARIISLEMMI " +
                    " FROM HRCIHALEONAYFORM LEFT OUTER JOIN " +
                      " HRCIHALEONAYTERTIP ON HRCIHALEONAYFORM.YIL = HRCIHALEONAYTERTIP.YIL AND " +
                      " HRCIHALEONAYFORM.SURECNO = HRCIHALEONAYTERTIP.SURECNO LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCIHALEONAYFORM.YIL = HRCSUREC.YIL AND HRCIHALEONAYFORM.SURECNO = HRCSUREC.SURECNO " +
                    " WHERE (HRCSUREC.SURECDURUM <> 9) AND (HRCSUREC.ISLEMKOD = 18) ";

            if (yil > 0)
            {
                groupbyAlanlar += " HRCIHALEONAYFORM.YIL,";
                sql += " AND (YEAR(HRCIHALEONAYFORM.BELGETARIH) = " + yil + ") ";
            }
            if (ay > 0)
            {
                sql += " AND (MONTH(HRCIHALEONAYFORM.BELGETARIH) = " + ay + ") ";
            }
            if (muhasebe > 0)
            {
                groupbyAlanlar += " HRCIHALEONAYFORM.MUHASEBEKOD,";
                sql += " AND (HRCIHALEONAYFORM.MUHASEBEKOD = " + muhasebe + ") ";
            }
            if (!string.IsNullOrEmpty(birim))
            {
                groupbyAlanlar += " HRCIHALEONAYFORM.BIRIMKOD,";
                sql += " AND (HRCIHALEONAYFORM.BIRIMKOD = '" + birim + "') ";
            }
            if (!string.IsNullOrEmpty(kur))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.KURKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.KURKOD = '" + kur.Replace(".", "") + "') ";
            }
            if (!string.IsNullOrEmpty(fon))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.FONKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.FONKOD = '" + fon.Replace(".", "") + "') ";
            }
            if (!string.IsNullOrEmpty(fin))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.FINKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.FINKOD = '" + fin.Replace(".", "") + "') ";
            }
            if (!string.IsNullOrEmpty(eko))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.EKOKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.EKOKOD = '" + eko.Replace(".", "") + "') ";
            }
            if (!string.IsNullOrEmpty(peb))
            {
                groupbyAlanlar += " HRCIHALEONAYTERTIP.PEBKOD,";
                sql += " AND (HRCIHALEONAYTERTIP.PEBKOD = '" + peb.Replace(".", "") + "') ";
            }

            sql += " GROUP BY HRCIHALEONAYFORM.MUHASEBEKOD, HRCIHALEONAYFORM.BIRIMKOD, HRCIHALEONAYFORM.SURECNO, HRCIHALEONAYTERTIP.KURKOD, " +
                      " HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, " +
                      " YEAR(HRCIHALEONAYFORM.BELGETARIH), MONTH(HRCIHALEONAYFORM.BELGETARIH), HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL " +
                    " ORDER BY ISLEMYIL DESC, ISLEMAY DESC, HRCIHALEONAYFORM.SURECNO, HRCIHALEONAYFORM.MUHASEBEKOD, HRCIHALEONAYFORM.BIRIMKOD, " +
                      " HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, " +
                      " HRCIHALEONAYTERTIP.PEBKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public decimal BlokeToplamListele(int yil, string surecNo)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT SUM(" + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO] + ")";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.YIL], "=", yil));
            if (surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTertip.SURECNO], "=", surecNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDecimal(sql);
        }

        public IDataReader MalHizmetYapimListele(int yil, int MalHizmetYapim, string saymanlikKodlar, string birimKodlar)
        {
            string sql = "SELECT HRCIHALEONAYTERTIP.EKOKOD, ABSEKO.EKOAD, HRCIHALETURMADDE.ADI,";
            sql += " SUM( case";
            sql += " when HRCIHALEONAYTERTIP.KDVLISOZLESMETUTAR IS NULL OR HRCIHALEONAYTERTIP.KDVLISOZLESMETUTAR=0 then HRCIHALEONAYTERTIP.KDVTUTAR + HRCIHALEONAYTERTIP.KULLANILACAKTUTAR";
            sql += " else HRCIHALEONAYTERTIP.KDVLISOZLESMETUTAR end)";
            sql += " FROM HRCIHALEONAYFORM INNER JOIN";
            sql += " HRCIHALEONAYTERTIP ON HRCIHALEONAYFORM.YIL = HRCIHALEONAYTERTIP.YIL AND HRCIHALEONAYFORM.SURECNO = HRCIHALEONAYTERTIP.SURECNO INNER JOIN";
            sql += " HRCSUREC ON HRCIHALEONAYFORM.YIL = HRCSUREC.YIL AND HRCIHALEONAYFORM.SURECNO = HRCSUREC.SURECNO INNER JOIN";
            sql += " HRCIS ON HRCSUREC.ISLEMKOD = HRCIS.KOD INNER JOIN";
            sql += " ABSEKO ON HRCIHALEONAYTERTIP.TERTIPYIL = ABSEKO.YIL AND HRCIHALEONAYTERTIP.EKOKOD = ABSEKO.EKOKOD LEFT OUTER JOIN";
            sql += " HRCIHALETURMADDE ON HRCIHALEONAYFORM.IHALEMADDESI = HRCIHALETURMADDE.KOD AND HRCIHALEONAYFORM.IHALEUSULU = HRCIHALETURMADDE.IHALEKOD";
            sql += " WHERE (HRCIHALEONAYTERTIP.TERTIPYIL = " + VeriErisim.DegerYap(yil) + ") AND (HRCSUREC.SURECDURUM < 9) AND HRCIS.TUR = " + VeriErisim.DegerYap(MalHizmetYapim);
            sql += " GROUP BY HRCIHALEONAYTERTIP.EKOKOD, ABSEKO.EKOAD, HRCIHALETURMADDE.ADI";
            sql += " order by 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
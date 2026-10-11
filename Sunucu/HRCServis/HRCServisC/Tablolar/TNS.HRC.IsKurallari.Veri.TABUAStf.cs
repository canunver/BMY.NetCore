using System;
using System.Data;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Para birimi bilgisini tutan HRCPARABIRIMI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABUAStf : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABUAStf(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Satın alma talep formu tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCUASTF";

        /// <summary>
        /// Satın alma talep formu tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            YIL = 0,
            FORMKOD = 1,
            EVRAKSAYI = 2,
            KONU = 3,
            GEREKCE = 4,
            ISEBASLAMATARIHI = 5,
            ACIKLAMA = 6,
            YAKLASIKMALIYET = 7,
            TERTIP = 8,
            IHALETURKOD = 9,
            IHALEMADDEKOD = 10,
            ISLEMKOD = 11,
            DURUM = 12,
            SURECYIL = 13,
            SURECNO = 14,
            MUHASEBE = 15,
            BIRIM = 16,
            KURKOD = 17,
            ALIMTURU = 18,
            ONAYASUNANADSOYAD = 19,
            ONAYASUNANUNVAN = 20,
            ONAYLAYANADSOYAD = 21,
            ONAYLAYANUNVAN = 22,
            OLUSTURULMATARIH = 23
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "FORMKOD", "EVRAKSAYI", "KONU", "GEREKCE", "ISEBASLAMATARIHI", "ACIKLAMA", "YAKLASIKMALIYET", "TERTIP", "IHALETURKOD", "IHALEMADDEKOD", "ISLEMKOD", "DURUM", "SURECYIL", "SURECNO", "MUHASEBE", "BIRIM", "KURKOD", "ALIMTURU", "ONAYASUNANADSOYAD", "ONAYASUNANUNVAN", "ONAYLAYANADSOYAD", "ONAYLAYANUNVAN", "OLUSTURULMATARIH" };

        /// <summary>
        /// Para birimi bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADBASLIK = "HRCUASTFIHTIYACBASLIK";

        /// <summary>
        /// Para birimi bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarBaslik
        {
            YIL = 0,
            FORMKOD = 1,
            SIRANO = 2,
            ACIKLAMA = 3
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBASLIK = { "YIL", "FORMKOD", "SIRANO", "ACIKLAMA" };


        /// <summary>
        /// Para birimi bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADDETAY = "HRCUASTFIHTIYACDETAY";

        /// <summary>
        /// Para birimi bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarDetay
        {
            YIL = 0,
            FORMKOD = 1,
            SIRANO = 2,
            DETAYSATIRNO = 3,
            IHTIYACKOD = 4,
            IHTIYACAD = 5,
            MIKTAR1 = 6,
            OLCUBIRIMKOD1 = 7,
            MIKTAR2 = 8,
            OLCUBIRIMKOD2 = 9,
            ACIKLAMA = 10
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = { "YIL", "FORMKOD", "SIRANO", "DETAYSATIRNO", "IHTIYACKOD", "IHTIYACAD", "MIKTAR1", "OLCUBIRIMKOD1", "MIKTAR2", "OLCUBIRIMKOD2", "ACIKLAMA" };

        /// <summary>
        /// Para birimi bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTEKLIF = "HRCUASTFIHTIYACTEKLIF";

        /// <summary>
        /// Para birimi bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarTeklif
        {
            YIL = 0,
            SURECNO = 1,
            SIRANO = 2,
            DETAYSATIRNO = 3,
            IHTIYACKOD = 4,
            IHTIYACAD = 5,
            MIKTAR1 = 6,
            OLCUBIRIMKOD1 = 7,
            MIKTAR2 = 8,
            OLCUBIRIMKOD2 = 9,
            FIRMAKOD = 10,
            BIRIMFIYAT = 11,
            TIP = 12
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTEKLIF = { "YIL", "SURECNO", "SIRANO", "DETAYSATIRNO", "IHTIYACKOD", "IHTIYACAD", "MIKTAR1", "OLCUBIRIMKOD1", "MIKTAR2", "OLCUBIRIMKOD2", "FIRMAKOD", "BIRIMFIYAT", "TIP" };


        /// <summary>
        /// İş bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADIS = "HRCIS";

        /// <summary>
        /// İş bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarIS
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 2,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 3,
            /// <summary>
            /// ODEMEHESAPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEHESAPKOD = 4
        }

        /// <summary>
        /// İş tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARIS = { "KOD", "AD", "EKOKOD", "TUR", "ODEMEHESAPKOD" };


        /// <summary>
        /// Para birimi bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(SatinAlmaTalepForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.formKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrakSayi) + ",";
            sql += VeriErisim.DegerYap(bilgi.konu) + ",";
            sql += VeriErisim.DegerYap(bilgi.gerekce) + ",";
            sql += VeriErisim.DegerYap(bilgi.iseBaslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyet) + ",";
            sql += VeriErisim.DegerYap(bilgi.tertipStr) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleTurKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleMaddeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap((int)bilgi.alimTuru) + ",";
            sql += VeriErisim.DegerYap(bilgi.onayaSunanAdSoyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.onayaSunanUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onaylayanAdSoyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.onaylayanUnvan) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ")";
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKSAYI], bilgi.evrakSayi) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONU], bilgi.konu) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GEREKCE], bilgi.gerekce) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], bilgi.iseBaslamaTarihi) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TERTIP], bilgi.tertipStr) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALETURKOD], bilgi.ihaleTurKod) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IHALEMADDEKOD], bilgi.ihaleMaddeKod) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], bilgi.islemKod) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYET], bilgi.yaklasikMaliyet) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALIMTURU], (int)bilgi.alimTuru) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYASUNANADSOYAD], bilgi.onayaSunanAdSoyad) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYASUNANUNVAN], bilgi.onayaSunanUnvan) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYLAYANADSOYAD], bilgi.onaylayanAdSoyad) + ", ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYLAYANUNVAN], bilgi.onaylayanUnvan);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMKOD], "=", bilgi.formKod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public IDataReader AlimlariListele(HizmetAlimi bilgi)
        {
            string sql = "";
            sql = "SELECT HRCUASTF.* FROM HRCIS INNER JOIN HRCUASTF ON HRCIS.KOD = HRCUASTF.ISLEMKOD AND HRCIS.TUR=" + VeriErisim.DegerYap(bilgi.Tur) + " WHERE HRCUASTF.SURECNO <> '' AND HRCUASTF.SURECYIL =" + bilgi.SurecYil;
            sql += " AND 1 IN (SELECT 1 FROM HRCTEKLIF WHERE HRCTEKLIF.YIL = HRCUASTF.SURECYIL AND HRCTEKLIF.SURECNO = HRCUASTF.SURECNO AND HRCTEKLIF.KAZANAN = 1)";// sadece kazanan firmaları getirmek için
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DurumGuncelle(int yil, string formKod, int durum)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMKOD], "=", formKod));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc SurecBilgiGuncelle(int yil, string formKod, int surecYil, string surecNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECYIL], surecYil) + ",";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], surecNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMKOD], "=", formKod));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(SatinAlmaTalepForm bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FORMKOD], "=", bilgi.formKod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(SatinAlmaTalepForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR)
                          + "," + VeriErisim.TumAlanlar(TABLOADIS, ALANLARIS);
            sql += " FROM " + TABLOAD;


            sql += " LEFT OUTER JOIN " + TABLOADIS + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADIS + "." + ALANLARIS[(int)ENUMAlanlarIS.KOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (bilgi.formKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMKOD], "=", bilgi.formKod));

            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", bilgi.muhasebe));

            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", bilgi.birim));

            if (bilgi.kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", bilgi.kurKod));

            if (bilgi.surecYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", bilgi.surecYil));

            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.FORMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetDetay(int yil, string formKod, int siraNo, int detaySatirNo, SatinAlmaTalepIhtiyacDetay bilgi)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(formKod) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(detaySatirNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihtiyacKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihtiyacAd) + ",";
            sql += VeriErisim.DegerYap(bilgi.miktar1) + ",";
            sql += VeriErisim.DegerYap(bilgi.olcuBirimiKod1) + ",";
            sql += VeriErisim.DegerYap(bilgi.miktar2) + ",";
            sql += VeriErisim.DegerYap(bilgi.olcuBirimiKod2) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetTeklif(int yil, string surecNo, int firmaKod, int tip, TNSCollection detay, int siraNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            sonuc = SilTeklif(yil, surecNo, firmaKod, tip, siraNo);
            if (sonuc.islemSonuc)
            {
                foreach (SatinAlmaTalepIhtiyacTeklif bilgi in detay)
                {
                    if (bilgi.firmaKod == 0) bilgi.firmaKod = firmaKod;
                    if (bilgi.siraNo == 0) bilgi.siraNo = siraNo;
                    string sql = "";
                    sql = "INSERT INTO " + TABLOADTEKLIF + " (" + VeriErisim.TumAlanlar("", ALANLARTEKLIF) + ") VALUES (";
                    sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                    sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                    sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
                    sql += VeriErisim.DegerYap(bilgi.detaySatirNo) + ",";
                    sql += VeriErisim.DegerYap(bilgi.ihtiyacKod) + ",";
                    sql += VeriErisim.DegerYap(bilgi.ihtiyacAd) + ",";
                    sql += VeriErisim.DegerYap(bilgi.miktar1) + ",";
                    sql += VeriErisim.DegerYap(bilgi.olcuBirimiKod1) + ",";
                    sql += VeriErisim.DegerYap(bilgi.miktar2) + ",";
                    sql += VeriErisim.DegerYap(bilgi.olcuBirimiKod2) + ",";
                    sql += VeriErisim.DegerYap(bilgi.firmaKod) + ",";
                    sql += VeriErisim.DegerYap(bilgi.birimFiyat) + ",";
                    sql += VeriErisim.DegerYap(bilgi.tip) + ")";

                    sonuc = KomutCalistir(sql, 1);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        public Sonuc SilDetay(int yil, string formKod, int siraNo)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FORMKOD], "=", formKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", siraNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilTeklif(int yil, string surecNo, int firmaKod, int tip, int siraNo)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADTEKLIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.SURECNO], "=", surecNo));
            if (firmaKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.FIRMAKOD], "=", firmaKod));
            if (tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.TIP], "=", tip));
            if (siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.SIRANO], "=", siraNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleBaslik(int yil, string formKod, int siraNo)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBASLIK, ALANLARBASLIK) + " FROM " + TABLOADBASLIK;

            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", yil));

            if (formKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", formKod));

            if (siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.SIRANO], "=", siraNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD] + "," + ALANLARBASLIK[(int)ENUMAlanlarBaslik.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDetay(int yil, string formKod, int siraNo)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;

            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));

            if (formKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FORMKOD], "=", formKod));

            if (siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", siraNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARDETAY[(int)ENUMAlanlarDetay.FORMKOD] + "," + ALANLARDETAY[(int)ENUMAlanlarDetay.DETAYSATIRNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleTeklif(int yil, string surecNo, int firmaKod, int tip, int siraNo)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTEKLIF, ALANLARTEKLIF) + " FROM " + TABLOADTEKLIF;

            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTEKLIF, ALANLARTEKLIF[(int)ENUMAlanlarTeklif.YIL], "=", yil));

            if (surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTEKLIF, ALANLARTEKLIF[(int)ENUMAlanlarTeklif.SURECNO], "=", surecNo));

            if (firmaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTEKLIF, ALANLARTEKLIF[(int)ENUMAlanlarTeklif.FIRMAKOD], "=", firmaKod));

            if (tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTEKLIF, ALANLARTEKLIF[(int)ENUMAlanlarTeklif.TIP], "=", tip));

            if (siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTEKLIF, ALANLARTEKLIF[(int)ENUMAlanlarTeklif.SIRANO], "=", siraNo));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARTEKLIF[(int)ENUMAlanlarTeklif.SIRANO] + "," + ALANLARTEKLIF[(int)ENUMAlanlarTeklif.DETAYSATIRNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Sorgula(SatinAlmaTalepFormSorgu bilgi, bool SureceBaglanacakAvansliAlimMi, string surecNo)
        {
            string sql = "";
            string select = "";
            string from = "";
            string innerJoin = "";
            string where = "";
            string kosul = "";
            string orderBy = "";

            //SELECT//
            select += "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR)
                          + "," + VeriErisim.TumAlanlar(TABLOADIS, ALANLARIS);

            //FROM//
            from += " FROM " + TABLOAD;

            innerJoin += " LEFT OUTER JOIN " + TABLOADIS + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADIS + "." + ALANLARIS[(int)ENUMAlanlarIS.KOD];

            kosul = "";
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", bilgi.muhasebe));
            if (!string.IsNullOrWhiteSpace(bilgi.kurKod))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", bilgi.kurKod.Replace(".", "") + "%"));
            }
            if (!string.IsNullOrWhiteSpace(bilgi.birim))
            {
                string[] birimler = bilgi.birim.Split(';');
                string birimKosul = "";
                for (int i = 0; i < birimler.Length; i++)
                {
                    if (birimler[i].Trim() != "")
                        birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birimler[i].Trim()));
                }
                if (birimKosul != "")
                    kosul += " AND (" + birimKosul + ")";
            }
            if (SureceBaglanacakAvansliAlimMi && string.IsNullOrWhiteSpace(surecNo))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALIMTURU], "=", 1));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", ""));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], ">=", 3));
            }
            if (SureceBaglanacakAvansliAlimMi == false && !string.IsNullOrWhiteSpace(surecNo))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALIMTURU], "=", 1));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", bilgi.surecYil));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], ">=", 3));
            }
            else
            {
                if (bilgi.durum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMSatinalmaTalepFormDurum.IPTAL));

                if (bilgi.islemKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", bilgi.islemKod));

                if (bilgi.formKodBasla != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMKOD], ">=", bilgi.formKodBasla));
                if (bilgi.formKodBitis != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMKOD], "<=", bilgi.formKodBitis));
                if (!bilgi.iseBaslamaTarihiBasla.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], ">=", bilgi.iseBaslamaTarihiBasla));
                if (!bilgi.iseBaslamaTarihiBitis.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], "<=", bilgi.iseBaslamaTarihiBitis));
                if (bilgi.yaklasikMaliyetBasla > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYET], ">=", bilgi.yaklasikMaliyetBasla));
                if (bilgi.yaklasikMaliyetBitis > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], "<=", bilgi.yaklasikMaliyetBitis));
                if (!string.IsNullOrWhiteSpace(bilgi.surecNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
                if (!string.IsNullOrWhiteSpace(bilgi.konu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONU], "LIKE", "%" + bilgi.konu + "%"));
            }
            where += " WHERE " + kosul;
            //---------------------->

            //ORDERBY//
            orderBy += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + "," + ALANLAR[(int)ENUMAlanlar.FORMKOD];
            //----------------------->

            sql = select + from + innerJoin + where + orderBy;
            //OrtakFonksiyonlar.HataStrYaz(sql);
            return veriErisim.DataReaderDondur(sql);
        }

        public string TalepNoBul(int yil, string surecNo)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.FORMKOD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));
            return this.veriErisim.TekKolonSQLStr(sql);
        }

        public int MaksIhtiyacSatirNo(int yil, string formKod)
        {
            string sql = "SELECT MAX(" + ALANLARBASLIK[(int)ENUMAlanlarBaslik.SIRANO] + ") FROM " + TABLOADBASLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", formKod));
            return (int)this.veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc IhtiyacSatirEkle(int yil, string formKod, int siraNo, string aciklama)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOADBASLIK + " (" + VeriErisim.TumAlanlar("", ALANLARBASLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(formKod) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(aciklama) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc IhtiyacSatirGuncelle(int yil, string formKod, int siraNo, string aciklama)
        {
            string sql = "UPDATE " + TABLOADBASLIK + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.ACIKLAMA], aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", formKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.SIRANO], "=", siraNo));
            return KomutCalistir(sql, 1);
        }

        public Sonuc SilBaslik(int yil, string formKod, int siraNo)
        {
            string sql = "DELETE FROM " + TABLOADBASLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", formKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.SIRANO], "=", siraNo));
            return KomutCalistir(sql, 1);
        }

        public Sonuc IhtiyacDetayKopyala(int yil, string formKod, int hedefSiraNo, int kaynakSiraNo)
        {
            string sql = "INSERT INTO HRCUASTFIHTIYACDETAY SELECT YIL, FORMKOD, " + hedefSiraNo + " AS SIRANO,  DETAYSATIRNO, IHTIYACKOD, IHTIYACAD, MIKTAR1, OLCUBIRIMKOD1, MIKTAR2, OLCUBIRIMKOD2, ACIKLAMA FROM HRCUASTFIHTIYACDETAY";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FORMKOD], "=", formKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", kaynakSiraNo));
            return KomutCalistir(sql, 0);
        }
    }
}
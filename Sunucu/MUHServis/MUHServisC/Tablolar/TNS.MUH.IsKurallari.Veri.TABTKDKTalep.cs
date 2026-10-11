using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHTkdkTalepForm veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTKDKTalep : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTKDKTalep(VeriErisim veriErisim) : base(veriErisim) { }


        /// <summary>
        /// create table MUHTkdkTalepForm
        /// (
        /// 	KOD INT, 
        /// 	JSON varchar(MAX),
        /// 	KAYITEDEN varchar(50),
        /// 	KAYITTARIHI DATETIME,
        /// 	primary key (KOD)
        /// )
        /// </summary>
        static string TABLOAD = "MUHTKDKTALEPFORM";

        enum ENUMAlanlar
        {
            KOD = 0,
            BELGENO = 1,
            YIL = 2,
            AY = 3,
            TUR = 4,
            MUHASEBEKOD = 5,
            BIRIMKOD = 6,
            DURUM = 7,
            KAYITEDEN = 8,
            KAYITEDENADI = 9,
            KAYITTARIHI = 10,
            GEREKCE = 11,
            ODEMEEMRINO1 = 12,
            ODEMEEMRINO2 = 13,
            ODEMEEMRINO3 = 14,
            ODEMEEMRINO4 = 15,
            MEVCUTBAKIYE = 16,
        }
        static string[] ALANLAR = { "KOD", "BELGENO", "YIL", "AY", "TUR", "MUHASEBEKOD", "BIRIMKOD", "DURUM", "KAYITEDEN", "KAYITEDENADI", "KAYITTARIHI", "GEREKCE", "ODEMEEMRINO1", "ODEMEEMRINO2", "ODEMEEMRINO3", "ODEMEEMRINO4", "MEVCUTBAKIYE" };

        static string TABLOADDETAY = "MUHTKDKTALEPDETAY";

        enum ENUMAlanlarDetay
        {
            KOD = 0,
            TALEPKOD = 1,
            SIRANO = 2,
            EKOKOD = 3,
            TUTAR = 4,
            TUTAR1 = 5,
            TUTAR2 = 6,
            TUTAR3 = 7,
            GEREKCE = 8,
            EKOADI = 9,
            TERTIP = 10,
        }
        static string[] ALANLARDETAY = { "KOD", "TALEPKOD", "SIRANO", "EKOKOD", "TUTAR", "TUTAR1", "TUTAR2", "TUTAR3", "GEREKCE", "EKOADI", "TERTIP" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(TkdkTalepForm bilgi)
        {
            //bilgi.kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.belgeNo) + ", ";
            sql += VeriErisim.DegerYap(bilgi.yil) + ", ";
            sql += VeriErisim.DegerYap(bilgi.ay) + ", ";
            sql += VeriErisim.DegerYap(bilgi.tur) + ", ";
            sql += VeriErisim.DegerYap(bilgi.muhasebeKod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.birimKod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.durum) + ", ";
            sql += VeriErisim.DegerYap(bilgi.kayitEden) + ", ";
            sql += VeriErisim.DegerYap(bilgi.kayitEdenAdi) + ", ";
            sql += VeriErisim.DegerYap(bilgi.kayitTarihi) + ", ";
            sql += VeriErisim.DegerYap(bilgi.gerekce) + ", ";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriNo1) + ", ";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriNo2) + ", ";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriNo3) + ", ";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriNo4) + ", ";
            sql += VeriErisim.DegerYap(bilgi.mevcutBakiye) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanında güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Kayıt bilgisini tutan nesne</param>
        /// <returns>Komutun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(TkdkTalepForm bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], bilgi.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AY], bilgi.ay) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], bilgi.tur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], bilgi.muhasebeKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], bilgi.birimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], bilgi.gerekce) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], 1) + ",";//Eğer guncelleme yapılıyorsa durumu yeni olsun1
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEVCUTBAKIYE], bilgi.mevcutBakiye);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        public Sonuc TertipKaydet(TkdkTalepDetay bilgi)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TERTIP], bilgi.tertip);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleDurum(TkdkTalepForm bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TkdkTalepForm bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(TkdkTalepForm bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", MUHMUHASEBEBIRIM.BIRIMAD, HRC.IBAN, HRC.VNO, HRC.VADI";
            sql += @",(SELECT SUM(MUHMIF.BORC - MUHMIF.ALACAK) AS Expr1
                        FROM MUHMIFFORM 
                        INNER JOIN MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO
                        WHERE  (MUHMIF.HESAPKOD = '5100301') 
                        AND (MUHMIFFORM.YIL = MUHTKDKTALEPFORM.YIL) 
                        AND (MUHMIF.ALTHESAPKOD = HRC.VNO) 
                        AND (MUHMIFFORM.FORMDURUM < 9) 
                        AND (MONTH(MUHMIFFORM.ISLEMTARIH) < MUHTKDKTALEPFORM.AY))";
            sql += " , MUHMUHASEBEBIRIM.KURKOD";
            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN MUHMUHASEBEBIRIM ON MUHMUHASEBEBIRIM.BIRIMKOD = MUHTKDKTALEPFORM.BIRIMKOD" +
                   " AND MUHMUHASEBEBIRIM.MUHASEBEKOD = MUHTKDKTALEPFORM.MUHASEBEKOD";
            sql += " LEFT OUTER JOIN HarcamaYonetimi.dbo.HRCIDAREBILGI HRC ON HRC.MUHASEBEKOD = MUHMUHASEBEBIRIM.MUHASEBEKOD" +
                   " AND HRC.BIRIMKOD = MUHMUHASEBEBIRIM.BIRIMKOD";
            string kosul = "";

            if (bilgi.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));
            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));
            if (bilgi.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            if (bilgi.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birimKod));
            if (bilgi.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY MUHTKDKTALEPFORM.YIL, MUHTKDKTALEPFORM.AY, MUHTKDKTALEPFORM.TUR, MUHMUHASEBEBIRIM.BIRIMAD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(TkdkTalepDetay bilgi)
        {
            //bilgi.kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);

            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.talepKod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ", ";
            sql += VeriErisim.DegerYap(bilgi.ekoKod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.tutar) + ", ";
            sql += VeriErisim.DegerYap(bilgi.tutar1) + ", ";
            sql += VeriErisim.DegerYap(bilgi.tutar2) + ", ";
            sql += VeriErisim.DegerYap(bilgi.tutar3) + ", ";
            sql += VeriErisim.DegerYap(bilgi.gerekce) + ", ";
            sql += VeriErisim.DegerYap(bilgi.ekoAdi) + ", ";
            sql += VeriErisim.DegerYap(bilgi.tertip) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        public Sonuc OdemeEmriNoGuncelle(TkdkTalepForm bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            if (bilgi.odemeEmriNo1 != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO1], bilgi.odemeEmriNo1);
            if (bilgi.odemeEmriNo2 != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO2], bilgi.odemeEmriNo2);
            if (bilgi.odemeEmriNo3 != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO3], bilgi.odemeEmriNo3);
            if (bilgi.odemeEmriNo4 != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO4], bilgi.odemeEmriNo4);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMTKDKTalepDurum.TAMAMLANDI));

            if (bilgi.kod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(TkdkTalepForm bilgi)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPKOD], "=", bilgi.kod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleDetay(TkdkTalepDetay bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDETAY) + " FROM " + TABLOADDETAY;
            string kosul = "";
            if (bilgi.kod != "")
                kosul = VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KOD], "=", bilgi.kod);
            if (bilgi.talepKod != "")
                kosul = VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPKOD], "=", bilgi.talepKod);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHODENEKTALEPBELGE, MUHODENEKTALEPDETAY, MUHODENEKTALEPTARIHCE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTalep : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTalep(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOADBELGE = "MUHODENEKTALEPBELGE";
        /// <summary>
        /// Talep tablosunun alanları
        /// </summary>
        public enum ENUMAlanlarBelge
        {
            /// <summary>
            /// Yıl
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Talep No
            /// </summary>
            TALEPNO = 1,
            /// <summary>
            /// Muhasebe Kodu
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// Harcama Birim kodu
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// Açıklama
            /// </summary>
            ACIKLAMA = 4,
            /// <summary>
            /// Talep tarihi
            /// </summary>
            TALEPTARIH = 5,
            /// <summary>
            /// Talep eden kullanıcı kodu
            /// </summary>
            TALEPKULLANICIKOD = 6,
            /// <summary>
            /// Form durum
            /// </summary>
            FORMDURUM = 7,
            /// <summary>
            /// Durum tarihi
            /// </summary>
            FORMDURUMTARIH = 8,
            /// <summary>
            /// Ödenek gönderme belge numarası
            /// </summary>
            ODENEKGONDERMEBELGENO = 9,
            /// <summary>
            /// Süreç yıl
            /// </summary>
            SURECYIL = 10,
            /// <summary>
            /// süreç no
            /// </summary>
            SURECNO = 11,
            /// <summary>
            /// Talebin karşılandığı tarih
            /// </summary>
            KARSILANANTARIH = 12,
            /// <summary>
            /// Talebi karşılayan kullanıcı
            /// </summary>
            KARSILAYANKULLANICIKOD = 13,
        }
        static string[] ALANLARBELGE = { "YIL", "TALEPNO", "MUHASEBEKOD", "BIRIMKOD", "ACIKLAMA", "TALEPTARIH", "TALEPKULLANICIKOD", "FORMDURUM", "FORMDURUMTARIH", "ODENEKGONDERMEBELGENO", "SURECYIL", "SURECNO", "KARSILANANTARIH", "KARSILAYANKULLANICIKOD" };

        static string TABLOADDETAY = "MUHODENEKTALEPDETAY";
        /// <summary>
        /// Detay tablosunun alanları
        /// </summary>
        public enum ENUMAlanlarDetay
        {
            /// <summary>
            /// Yıl
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Talep No
            /// </summary>
            TALEPNO = 1,
            /// <summary>
            /// Sıra no
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// Kurumsal kod
            /// </summary>
            KURKOD = 3,
            /// <summary>
            /// Fonksiyonel kod
            /// </summary>
            FONKOD = 4,
            /// <summary>
            /// Finans kodu
            /// </summary>
            FINKOD = 5,
            /// <summary>
            /// Ekonomik kod
            /// </summary>
            EKOKOD = 6,
            /// <summary>
            /// Faaliyet kodu
            /// </summary>
            PEBKOD = 7,
            /// <summary>
            /// Talep edilen tutar
            /// </summary>
            TALEPTUTAR = 8,
            /// <summary>
            /// Talep açıklama
            /// </summary>
            TALEPACIKLAMA = 9,
            /// <summary>
            /// Talebin karşılanmasında girilecek açıklama
            /// </summary>
            KARSILANANACIKLAMA = 10,
        }
        static string[] ALANLARDETAY = { "YIL", "TALEPNO", "SIRANO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "TALEPTUTAR", "TALEPACIKLAMA", "KARSILANANACIKLAMA" };

        static string TABLOADTARIHCE = "MUHODENEKTALEPTARIHCE";
        /// <summary>
        /// Tarihçe alanları
        /// </summary>
        public enum ENUMAlanlarTarihce
        {
            /// <summary>
            /// Yıl
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Talep No
            /// </summary>
            TALEPNO = 1,
            /// <summary>
            /// Sıra no
            /// </summary>
            SIRANO = 2,
            /// <summary>
            /// tarih
            /// </summary>
            TARIH = 3,
            /// <summary>
            /// İşlem yapan kullanıcı
            /// </summary>
            ISLEMYAPAN = 4,
            /// <summary>
            /// Yapılan işlem
            /// </summary>
            ISLEM = 5,
        }
        static string[] ALANLARTARIHCE = { "YIL", "TALEPNO", "SIRANO", "TARIH", "ISLEMYAPAN", "ISLEM" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(TalepBelge o)
        {
            //Belge Kaydet
            Sonuc sonuc = KaydetBelge(o);

            //Detay Kaydet
            if (sonuc.islemSonuc)
            {
                int siraNo = 1;
                foreach (TalepDetay detay in o.detay.objeler)
                {
                    //if (detay.talepNo > 0 && detay.siraNo > 0)
                    //    sonuc = SilDetay(detay);
                    //if (sonuc.islemSonuc)
                    //{
                    detay.siraNo = siraNo;
                    detay.talepNo = o.talepNo;
                    sonuc = KaydetDetay(detay);
                    if (!sonuc.islemSonuc)
                        break;
                    siraNo++;
                    //}
                    //else
                    //    break;
                }
            }

            //Tarihçe Kaydet
            //if (sonuc.islemSonuc)
            //{
            //    foreach (TalepTarihce tarihce in o.tarihce.objeler)
            //    {
            //        sonuc = KaydetTarihce(tarihce);
            //        if (!sonuc.islemSonuc)
            //            break;
            //    }
            //}

            return sonuc;
        }
        
        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetBelge(TalepBelge o)
        {
            string sql = "INSERT INTO " + TABLOADBELGE + " (" + VeriErisim.TumAlanlar("", ALANLARBELGE) + ") VALUES (";
            sql += VeriErisim.DegerYap(o.yil) + ", ";
            sql += VeriErisim.DegerYap(o.talepNo) + ", ";
            sql += VeriErisim.DegerYap(o.muhasebeKod) + ", ";
            sql += VeriErisim.DegerYap(o.birimKod) + ", ";
            sql += VeriErisim.DegerYap(o.aciklama) + ", ";
            sql += VeriErisim.DegerYap(o.talepTarih) + ", ";
            sql += VeriErisim.DegerYap(o.talepKullaniciKod) + ", ";
            sql += VeriErisim.DegerYap(o.formDurum) + ", ";
            sql += VeriErisim.DegerYap(o.formDurumTarih) + ", ";
            sql += VeriErisim.DegerYap(o.odenekGondermeBelgeNo) + ", ";
            sql += VeriErisim.DegerYap(o.surecYil) + ", ";
            sql += VeriErisim.DegerYap(o.surecNo) + ",";
            sql += VeriErisim.DegerYap(o.karsilananTarih) + ",";
            sql += VeriErisim.DegerYap(o.karsilayanKullaniciKod) + ")";

            return KomutCalistir(sql, 1);
        }
        
        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(TalepDetay o)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(o.yil) + ", ";
            sql += VeriErisim.DegerYap(o.talepNo) + ", ";
            sql += VeriErisim.DegerYap(o.siraNo) + ", ";
            sql += VeriErisim.DegerYap(o.kurKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.fonKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.finKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.ekoKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.pebKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.talepTutar) + ", ";
            sql += VeriErisim.DegerYap(o.talepAciklama) + ", ";
            sql += VeriErisim.DegerYap(o.karsilananAciklama) + ") ";

            return KomutCalistir(sql, 1);
        }
        
        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetTarihce(TalepTarihce o)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + " (" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES (";
            sql += VeriErisim.DegerYap(o.yil) + ", ";
            sql += VeriErisim.DegerYap(o.talepNo) + ", ";
            sql += VeriErisim.DegerYap(o.siraNo) + ", ";
            sql += VeriErisim.DegerYap(o.tarih) + ", ";
            sql += VeriErisim.DegerYap(o.islemYapan) + ", ";
            sql += VeriErisim.DegerYap(o.islem) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanında güncelleme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Güncellenecek kayıt bilgisini tutan nesne</param>
        /// <returns></returns>
        public Sonuc GuncelleBelge(TalepBelge o)
        {
            string sql = string.Empty;
            sql = "UPDATE " + TABLOADBELGE + " SET ";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.ACIKLAMA], o.aciklama) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.FORMDURUM], o.formDurum) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.FORMDURUMTARIH], o.formDurumTarih) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.ODENEKGONDERMEBELGENO], o.odenekGondermeBelgeNo) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPKULLANICIKOD], o.talepKullaniciKod) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPTARIH], o.talepTarih) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.SURECNO], o.surecNo) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.SURECYIL], o.surecYil) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.SURECNO], o.karsilananTarih) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.SURECYIL], o.karsilayanKullaniciKod);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.YIL], "=", o.yil));
            if (o.talepNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPNO], "=", o.talepNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.MUHASEBEKOD], "=", o.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.BIRIMKOD], "=", o.birimKod));
            if (!string.IsNullOrEmpty(o.odenekGondermeBelgeNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.ODENEKGONDERMEBELGENO], "=", o.odenekGondermeBelgeNo));

            return KomutCalistir(sql, 1);
        }
        
        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanında güncelleme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Güncellenecek kayıt bilgisini tutan nesne</param>
        /// <returns></returns>
        public Sonuc GuncelleDetay(TalepDetay o)
        {
            string sql = string.Empty;
            sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KURKOD], o.kurKod) + ",";
            sql += VeriErisim.AtamaOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FONKOD], o.fonKod) + ",";
            sql += VeriErisim.AtamaOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FINKOD], o.finKod) + ",";
            sql += VeriErisim.AtamaOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.EKOKOD], o.ekoKod) + ",";
            sql += VeriErisim.AtamaOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.PEBKOD], o.pebKod) + ",";
            sql += VeriErisim.AtamaOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KARSILANANACIKLAMA], o.karsilananAciklama) + ",";
            sql += VeriErisim.AtamaOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPACIKLAMA], o.talepAciklama) + ",";
            sql += VeriErisim.AtamaOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPTUTAR], o.talepTutar);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPNO], "=", o.talepNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", o.siraNo));

            return KomutCalistir(sql, 1);
        }
        
        /// <summary>
        /// Parametre olarak gelen bilgilere ait durum bilgisini veri tabanında güncelleme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="talepNo">Talep no.</param>
        /// <param name="talepFormDurum">Talep form durum.</param>
        /// <returns></returns>
        public Sonuc DurumDegistir(int yil, int talepNo, int talepFormDurum)
        {
            string sql = string.Empty;
            sql = "UPDATE " + TABLOADBELGE + " SET ";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.FORMDURUM], talepFormDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPNO], "=", talepNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Karşılanan talepler için oluşturulmuş ödenek gönderme belgelerinin numaralarını veri tabanında güncelleme sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="talepNo">Talep no.</param>
        /// <param name="odenekGondermeBelgeNo">Ödenek gönderme belge no.</param>
        /// <returns></returns>
        public Sonuc KaydetOdenekGondermeBelgeNo(int yil, int talepNo, string odenekGondermeBelgeNo)
        {
            string sql = string.Empty;
            sql = "UPDATE " + TABLOADBELGE + " SET ";
            sql += VeriErisim.AtamaOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.ODENEKGONDERMEBELGENO], odenekGondermeBelgeNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPNO], "=", talepNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TalepBelge o)
        {
            //Belge Sil
            Sonuc sonuc = SilBelge(o);

            //Detay Sil
            if (sonuc.islemSonuc)
            {
                if (o.detay.ObjeSayisi > 0)
                {
                    foreach (TalepDetay detay in o.detay.objeler)
                    {
                        sonuc = SilDetay(detay);
                        if (!sonuc.islemSonuc)
                            break;
                    }
                }
            }

            //Tarihçe Sil
            //if (sonuc.islemSonuc)
            //{
            //    if (o.tarihce.ObjeSayisi > 0)
            //    {
            //        foreach (TalepTarihce tarihce in o.tarihce.objeler)
            //        {
            //            sonuc = SilTarihce(tarihce);
            //            if (!sonuc.islemSonuc)
            //                break;
            //        }
            //    }
            //}
            return sonuc;
        }
        
        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilBelge(TalepBelge o)
        {
            string sql = "DELETE FROM " + TABLOADBELGE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPNO], "=", o.talepNo));

            return KomutCalistir(sql, 1);
        }
        
        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(TalepDetay o)
        {
            int effectedRowCount = 0;

            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPNO], "=", o.talepNo));
            if (o.siraNo > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", o.siraNo));
                effectedRowCount = 1;
            }

            return KomutCalistir(sql, effectedRowCount);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilTarihce(TalepTarihce o)
        {
            int effectedRowCount = 0;

            string sql = "DELETE FROM " + TABLOADTARIHCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TALEPNO], "=", o.talepNo));
            if (o.siraNo > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTARIHCE[(int)ENUMAlanlarTarihce.SIRANO], "=", o.siraNo));
                effectedRowCount = 1;
            }

            return KomutCalistir(sql, effectedRowCount);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="o">kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader ListeleBelge(TalepBelge o)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBELGE, ALANLARBELGE);
            sql += " FROM " + TABLOADBELGE;

            string kosul = string.Empty;
            if (o.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.YIL], "=", o.yil));
            if (o.talepNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPNO], "=", o.talepNo));
            if (o.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.MUHASEBEKOD], "=", o.muhasebeKod));
            if (o.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.BIRIMKOD], "=", o.birimKod));
            if (o.formDurum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.FORMDURUM], "=", o.formDurum));
            if (o.odenekGondermeBelgeNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.ODENEKGONDERMEBELGENO], "=", o.odenekGondermeBelgeNo));
            if (o.surecYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.SURECYIL], "=", o.surecYil));
            if (o.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.SURECNO], "=", o.surecNo));
            if (o.talepKullaniciKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGE, ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPKULLANICIKOD], "=", o.talepKullaniciKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADBELGE + "." + ALANLARBELGE[(int)ENUMAlanlarBelge.YIL];
            sql += ", " + TABLOADBELGE + "." + ALANLARBELGE[(int)ENUMAlanlarBelge.TALEPNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="o">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader ListeleDetay(TalepDetay o)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            sql += " FROM " + TABLOADDETAY;

            string kosul = string.Empty;
            if (o.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", o.yil));
            if (o.talepNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPNO], "=", o.talepNo));
            if (o.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", o.siraNo));
            if (o.kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KURKOD], "=", o.kurKod));
            if (o.fonKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FONKOD], "=", o.fonKod));
            if (o.finKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FINKOD], "=", o.finKod));
            if (o.ekoKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.EKOKOD], "=", o.ekoKod));
            if (o.pebKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.PEBKOD], "=", o.pebKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL];
            sql += ", " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPNO];
            sql += ", " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        IDataReader ListeleTarihce(TalepTarihce o)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTARIHCE, ALANLARTARIHCE);
            sql += " FROM " + TABLOADTARIHCE;

            string kosul = string.Empty;
            if (o.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.YIL], "=", o.yil));
            if (o.talepNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TALEPNO], "=", o.talepNo));
            if (o.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.SIRANO], "=", o.siraNo));
            if (o.islem > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ISLEM], "=", o.islem));
            if (o.islemYapan != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADTARIHCE, ALANLARTARIHCE[(int)ENUMAlanlarTarihce.ISLEMYAPAN], "=", o.islemYapan));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADTARIHCE + "." + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.YIL];
            sql += ", " + TABLOADTARIHCE + "." + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.TALEPNO];
            sql += ", " + TABLOADTARIHCE + "." + ALANLARTARIHCE[(int)ENUMAlanlarTarihce.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
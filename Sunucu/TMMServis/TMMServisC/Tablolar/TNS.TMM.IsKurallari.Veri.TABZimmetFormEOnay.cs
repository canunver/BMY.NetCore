using System;
using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Zimmet Form eonay bilgilerini tutan TASZIMMETFORMEOANY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABZimmetFormEOnay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABZimmetFormEOnay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASZIMMETFORMEONAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// Belge türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 4,
            /// <summary>
            /// eOnay için mail atan kişi TC Kimlik bilgisi (indisi)
            /// </summary>
            EMAILKIMDEN = 5,
            /// <summary>
            /// eOnay için mail atan kişi ad bilgisi (indisi)
            /// </summary>
            EMAILKIMDENAD = 6,
            /// <summary>
            /// eOnay için mail atılan kişi TC Kimlik bilgisi (indisi)
            /// </summary>            
            EMAILKIME = 7,
            /// <summary>
            /// eOnay için mail atılan kişi ad bilgisi (indisi)
            /// </summary>            
            EMAILKIMEAD = 8,
            /// <summary>
            /// mail gönderme tarihi (indisi)
            /// </summary>
            EMAILGONDERMETARIH = 9,
            /// <summary>
            /// mail cevap tarihi (indisi)
            /// </summary>
            EMAILCEVAPTARIH = 10,
            /// <summary>
            /// Durum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 11
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "BELGETUR", "EMAILKIMDEN", "EMAILKIMDENAD", "EMAILKIME", "EMAILKIMEAD", "EMAILGONDERMETARIH", "EMAILCEVAPTARIH", "DURUM" };

        /// <summary>
        /// Zimmet Form EOnay bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="zimmetEOnay">Zimmet Form EOnay bilgilerini tutan, ZimmetFormEOnay türünde nesne</param>
        /// <returns>Ekleme, Ekleme komutunu çalıştıramazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(ZimmetFormEOnay zimmetEOnay)
        {
            string sql = "SELECT * FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmetEOnay.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmetEOnay.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmetEOnay.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmetEOnay.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmetEOnay.belgeTur));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAILKIMDEN], zimmetEOnay.emailKimden) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAILKIMDENAD], zimmetEOnay.emailKimdenAd) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAILKIME], zimmetEOnay.emailKime) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAILKIMEAD], zimmetEOnay.emailKimeAd) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAILGONDERMETARIH], zimmetEOnay.emailGondermeTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAILCEVAPTARIH], zimmetEOnay.emailCevapTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], zimmetEOnay.durum);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmetEOnay.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmetEOnay.fisNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmetEOnay.harcamaBirimKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmetEOnay.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmetEOnay.belgeTur));

                KaydetTarihce(zimmetEOnay.yil, zimmetEOnay.muhasebeKod, zimmetEOnay.harcamaBirimKod, zimmetEOnay.fisNo, zimmetEOnay.belgeTur, zimmetEOnay.emailKimdenAd, ZimmetFormEOnay.FormEOnayDurumAciklama((int)ENUMEOnayDurumu.GONDERILDI));
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(zimmetEOnay.yil) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.harcamaBirimKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.fisNo) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.belgeTur) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.emailKimden) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.emailKimdenAd) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.emailKime) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.emailKimeAd) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.emailGondermeTarih) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.emailCevapTarih) + ",";
                sql += VeriErisim.DegerYap(zimmetEOnay.durum) + ")";

                KaydetTarihce(zimmetEOnay.yil, zimmetEOnay.muhasebeKod, zimmetEOnay.harcamaBirimKod, zimmetEOnay.fisNo, zimmetEOnay.belgeTur, zimmetEOnay.emailKimdenAd, ZimmetFormEOnay.FormEOnayDurumAciklama((int)ENUMEOnayDurumu.GONDERILDI));
            }

            return KomutCalistir(sql, 1);
        }


        /// <summary>
        /// Parametre olarak verilen Zimmet Form EOnay ve Zimmet Form EOnay Kriter bilgisine göre,
        /// Zimmet Form EOnay bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="zimmet">Zimmet Form EOnay bilgisini tutan, ZimmetForm Eonay türünde nesne</param>
        /// <param name="kriter">Zimmet Form EOnay Kriter bilgisini tutan, ZimmetFormEOnayKriter türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, Zimmet Form bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(ZimmetFormEOnay zimmetEonay, ZimmetFormEOnayKriter kriter)
        {
            string secAlanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            string sql = "SELECT " + secAlanlar + " FROM " + TABLOAD;
            //if (kriter.sicilNo != string.Empty)
            //{
            //    sql += " JOIN TASZIMMETDETAY ON TASZIMMETFORM.FISNO=TASZIMMETDETAY.FISNO AND TASZIMMETFORM.YIL=TASZIMMETDETAY.YIL AND TASZIMMETFORM.MUHASEBEKOD=TASZIMMETDETAY.MUHASEBEKOD AND TASZIMMETFORM.HARCAMABIRIMKOD=TASZIMMETDETAY.HARCAMABIRIMKOD AND TASZIMMETFORM.BELGETUR=TASZIMMETDETAY.BELGETUR";
            //}
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmetEonay.yil));

            if (zimmetEonay.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmetEonay.muhasebeKod));
            if (zimmetEonay.harcamaBirimKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmetEonay.harcamaBirimKod.Replace(".", "")));
            if (zimmetEonay.fisNo != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmetEonay.fisNo));

            if (zimmetEonay.belgeTur != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmetEonay.belgeTur));

            if (!kriter.eonayGondermeTarihBasla.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAILGONDERMETARIH], ">=", kriter.eonayGondermeTarihBasla));
            if (!kriter.eonayGondermeTarihBitis.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAILGONDERMETARIH], "<=", kriter.eonayGondermeTarihBitis));


            if (!kriter.eonayCevapTarihBasla.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAILCEVAPTARIH], ">=", kriter.eonayCevapTarihBasla));
            if (!kriter.eonayCevapTarihBitis.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAILCEVAPTARIH], "<=", kriter.eonayCevapTarihBitis));


            if (kriter.emailKimden != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAILKIMDEN], "=", kriter.emailKimden));
            if (kriter.emailKime != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAILKIME], "=", kriter.emailKime));

            if (kriter.durum > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", zimmetEonay.durum));
            }

            return veriErisim.DataReaderDondur(sql);
        }


        /// <summary>
        /// Parametre olarak verilen Zimmet Form ve Zimmet Form Kriter bilgisine göre,
        /// Kişi/ortak alan Zimmet Formları bilgisini EOnay bilgileri ile beraber döndüren fonksiyon
        /// </summary>
        /// <param name="zimmet">Zimmet Form bilgisini tutan, ZimmetForm türünde nesne</param>
        /// <param name="kriter">Zimmet Form Kriter bilgisini tutan, ZimmetFormKriter türünde nesne</param>
        /// <param name="eonaykriter"> Sorgulama için eonay kriter bilgilerini tutan nesne</param> 
        /// <returns>Select komutunun çalışması sonucunda, Zimmet Form bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ListeleKisiOrtakEOnayli(ZimmetFormEOnayKriter eonaykriter)
        {
            string secAlanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR);

            string sql = "SELECT " + secAlanlar  + ", TASMUHASEBE.AD, TASHARCAMABIRIM.AD "+ " FROM " + TABLOAD;

            sql += " LEFT JOIN TASMUHASEBE on TASZIMMETFORMEONAY.MUHASEBEKOD=TASMUHASEBE.KOD ";
            sql += " LEFT JOIN TASHARCAMABIRIM on TASZIMMETFORMEONAY.HARCAMABIRIMKOD=TASHARCAMABIRIM.KOD ";

            sql += " WHERE 1=1 ";

            if (eonaykriter.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", eonaykriter.muhasebeKod));

            if (eonaykriter.harcamaBirimKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", eonaykriter.harcamaBirimKod));

            if (eonaykriter.emailKime != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAILKIME], "=", eonaykriter.emailKime));

            if (eonaykriter.emailKimden != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMAILKIMDEN], "=", eonaykriter.emailKimden));

            if (eonaykriter.durum != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", eonaykriter.durum));

            sql += " ORDER BY 1,2,3,4";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe Kodu, Harcama Birim Kodu, Fiş No, Belge Türü, İşlem Yapan ve İşlem bilgilerine göre,
        /// Zimmet Formunun tarihçesini kaydeden fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebeKod">Muhasebe kodunu tutan, String türünde nesne</param>
        /// <param name="harcamaBirimKod">Harcama Birim kodunu tutan, String türünde nesne</param>
        /// <param name="fisNo">Fiş no bilgisini tutan, String türünde nesne</param>
        /// <param name="belgeTur">Belge Tür bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemYapan">İşlem Yapan bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        public void KaydetTarihce(int yil, string muhasebeKod, string harcamaBirimKod, string fisNo, int belgeTur, string islemYapan, string islem)
        {
            string sql = "";

            sql = "INSERT INTO TASZIMMETFORMTARIHCE (YIL,MUHASEBEKOD,HARCAMABIRIMKOD,FISNO,BELGETUR,ISLEMYAPAN,ISLEM,ISLEMTARIH) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(harcamaBirimKod.Trim().Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fisNo.Trim()) + ", ";
            sql = sql + VeriErisim.DegerYap(belgeTur) + ", ";
            sql = sql + VeriErisim.DegerYap(islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ")";

            KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Form bilgisine göre,
        /// Belge durum bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="zimmetEOnay">Zimmet Form EOnay bilgisini tutan, ZimmetFormEOnay türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kıstaslara uygun zimmet durumları bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BelgeEOnayDurumGetir(ZimmetFormEOnay zimmetEOnay)
        {
            string sql = "SELECT DURUM FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmetEOnay.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmetEOnay.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmetEOnay.harcamaBirimKod.Trim().Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmetEOnay.muhasebeKod.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmetEOnay.belgeTur));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Form EONay bilgisi ve Durum bilgisine göre,
        /// Zimmet EOnay durumunu değiştiren fonksiyon
        /// </summary>
        /// <param name="zfeonay">Zimmet EOnay bilgisini tutan, ZimmetFormEOnay türünde nesne</param>
        /// <param name="yeniDurum">Güncellenecek durum bilgisini tutan, String türünde nesne</param>
        /// <returns>Güncelleme komutunun çalışması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BelgeEOnayDurumDegistir(ZimmetFormEOnay zfeonay, string yeniDurum, string personelKod)
        {
            //int durum = TABGenel.FormDurumNoVer(yeniDurum);
            //if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
            //    durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;
            int durum = OrtakFonksiyonlar.ConvertToInt(yeniDurum,0);

            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAILCEVAPTARIH], new TNSDateTime(DateTime.Now));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", zfeonay.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", zfeonay.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zfeonay.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zfeonay.harcamaBirimKod.Trim().Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zfeonay.belgeTur));

            KaydetTarihce(zfeonay.yil, zfeonay.muhasebeKod, zfeonay.harcamaBirimKod, zfeonay.fisNo, zfeonay.belgeTur, personelKod, ZimmetFormEOnay.FormEOnayDurumAciklama(durum));

            return KomutCalistir(sql, 1);
        }
    }
}
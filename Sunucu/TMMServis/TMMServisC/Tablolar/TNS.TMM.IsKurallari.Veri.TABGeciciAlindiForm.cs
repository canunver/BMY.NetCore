using System;
using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Geçici olarak teslim alınan (Konsinye) taşınır malzemelerin form bilgisini tutan TASGECICIFORM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABGeciciAlindiForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGeciciAlindiForm(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASGECICIFORM";

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
            /// Harcama birimi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// Fiş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISTARIH = 4,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 5,
            /// <summary>
            /// Nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDENGELDI = 6,
            /// <summary>
            /// Durum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 7,
            /// <summary>
            /// İşlem tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 8,
            /// <summary>
            /// İşlem yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 9,
            /// <summary>
            /// Fatura no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURANO = 10,
            /// <summary>
            /// Fatura tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURATARIH = 11,

            TIFFISNO = 12
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO","FISTARIH", "AMBARKOD",
                                    "NEREDENGELDI","DURUM","ISLEMTARIH","ISLEMYAPAN","FATURANO","FATURATARIH","TIFFISNO"};

        /// <summary>
        /// Konsinye olarak alınan Taşınır Malzeme ilişkin form bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgilerini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Ekleme,Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(TasinirIslemForm tf)
        {
            string sql = "SELECT YIL, HARCAMABIRIMKOD, FISNO FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISTARIH], tf.fisTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], tf.ambarKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], tf.faturaNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURATARIH], tf.faturaTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREDENGELDI], tf.neredenGeldi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], tf.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], tf.islemYapan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], tf.islemTarih);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(tf.yil) + ",";
                sql += VeriErisim.DegerYap(tf.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(tf.harcamaKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(tf.fisNo) + ",";
                sql += VeriErisim.DegerYap(tf.fisTarih) + ",";
                sql += VeriErisim.DegerYap(tf.ambarKod) + ",";
                sql += VeriErisim.DegerYap(tf.neredenGeldi) + ",";
                sql += VeriErisim.DegerYap(tf.durum) + ",";
                sql += VeriErisim.DegerYap(tf.islemTarih) + ",";
                sql += VeriErisim.DegerYap(tf.islemYapan) + ",";
                sql += VeriErisim.DegerYap(tf.faturaNo) + ",";
                sql += VeriErisim.DegerYap(tf.faturaTarih) + ",";
                sql += VeriErisim.DegerYap(tf.tifFisNo) + ")";
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Konsinye olarak alınan Taşınır Malzeme Form bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Konsinye olarak alınan Taşınır Malzeme form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader FisAc(TasinirIslemForm tf)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASMUHASEBE.AD, TASIL.KOD, TASIL.AD, TASILCE.KOD, TASILCE.AD, TASHARCAMABIRIM.AD, TASAMBAR.AD FROM " + TABLOAD;

            sql += " JOIN TASMUHASEBE ON TASGECICIFORM.MUHASEBEKOD = TASMUHASEBE.KOD";
            sql += " JOIN TASILCE ON TASMUHASEBE.ILKOD=TASILCE.ILKOD AND TASMUHASEBE.ILCEKOD=TASILCE.KOD JOIN TASIL ON TASILCE.ILKOD = TASIL.KOD";
            sql += " JOIN TASHARCAMABIRIM ON TASGECICIFORM.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD AND TASGECICIFORM.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD";
            sql += " JOIN TASAMBAR ON TASGECICIFORM.AMBARKOD = TASAMBAR.KOD AND TASGECICIFORM.HARCAMABIRIMKOD=TASAMBAR.HARCAMABIRIMKOD AND TASGECICIFORM.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND TASGECICIFORM.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini ve Taşınır Form Kriter bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Konsinye olarak alınan Taşınır Malzeme Form bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="kriter">Taşınır Form Kriter bilgisini tutan, TasinirFormKriter türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Konsinye olarak alınan Taşınır Malzeme form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(TasinirIslemForm tf, TasinirFormKriter kriter)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TASMUHASEBE.AD, TASHARCAMABIRIM.AD, TASAMBAR.AD FROM ";
            sql += " TASHARCAMABIRIM INNER JOIN ";
            sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD INNER JOIN ";
            sql += " TASAMBAR ON TASHARCAMABIRIM.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND ";
            sql += " TASHARCAMABIRIM.KOD = TASAMBAR.HARCAMABIRIMKOD INNER JOIN ";
            sql += " TASGECICIFORM ON TASAMBAR.MUHASEBEKOD = TASGECICIFORM.MUHASEBEKOD AND ";
            sql += " TASAMBAR.HARCAMABIRIMKOD = TASGECICIFORM.HARCAMABIRIMKOD AND TASAMBAR.KOD = TASGECICIFORM.AMBARKOD ";


            if (kriter.hesapKodu != string.Empty)
            {
                sql += " INNER JOIN TASGECICIDETAY ON TASGECICIFORM.YIL = TASGECICIDETAY.YIL AND TASGECICIFORM.MUHASEBEKOD = TASGECICIDETAY.MUHASEBEKOD AND ";
                sql += " TASGECICIFORM.HARCAMABIRIMKOD = TASGECICIDETAY.HARCAMABIRIMKOD AND TASGECICIFORM.FISNO = TASGECICIDETAY.FISNO ";
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));

            if (tf.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));

            if (tf.durum > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", tf.durum));
                if (tf.durum == 1) //Yeni ve değişen belgeleri getir.
                    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI));
                sql += ")";
            }
            else if (tf.durum == 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));
            }
            if (tf.harcamaKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            if (tf.ambarKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", tf.ambarKod));

            if (!kriter.belgeTarihBasla.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISTARIH], ">=", kriter.belgeTarihBasla));
            if (!kriter.belgeTarihBit.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISTARIH], "<=", kriter.belgeTarihBit));
            if (kriter.belgeNoBasla != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], ">=", kriter.belgeNoBasla));
            if (kriter.belgeNoBit != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "<=", kriter.belgeNoBit));
            if (kriter.islemYapan != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", kriter.islemYapan));
            if (kriter.hesapKodu != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASGECICIDETAY", "HESAPPLANKOD", "LIKE", kriter.hesapKodu.Replace(".", "") + "%"));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu ve Yeni Durum bilgilerini kullanarak,
        /// Konsinye olarak alınan Taşınır Malzeme ilişkin form durum bilgisini güncelleyen,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemform türünde nesne</param>
        /// <param name="yeniDurum">Güncellenecek form durum bilgisi, String türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BelgeDurumDegistir(TasinirIslemForm tf, string yeniDurum)
        {
            int durum = TABGenel.FormDurumNoVer(yeniDurum);
            if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], tf.islemYapan) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen durum koduna göre,
        /// Konsinye olarak alınan Taşınır Malzeme ilişkin form durum bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="durum">Form durum kodunu tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda Form durumunu tutan String türünde nesne döndürür</returns>
        public string FormDurumAciklama(int durum)
        {
            string islem = "";

            if (durum == (int)ENUMBelgeDurumu.YENI) islem = "Yeni Giriş";
            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI) islem = "Değiştirildi";
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI) islem = "Onaylandı";
            else if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR) islem = "Onay Kaldırıldı";
            else if (durum == (int)ENUMBelgeDurumu.IPTAL) islem = "İptal Edildi";
            else
                islem = "Bilinmiyor (" + durum + ")";

            return islem;
        }

        /// <summary>
        /// Parametre olarak Taşınır İşlem Form bilgilerini kullanarak,
        /// Konsinye olarak alınan Taşınır Malzeme ilişkin form durum bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgilerini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Konsinye olarak alınan Taşınır Malzeme form durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BelgeDurumGetir(TasinirIslemForm tf)
        {
            string sql = "SELECT TASGECICIFORM.DURUM FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc TifFisNoKaydet(TasinirIslemForm tf)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIFFISNO], tf.tifFisNo) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }
    }
}

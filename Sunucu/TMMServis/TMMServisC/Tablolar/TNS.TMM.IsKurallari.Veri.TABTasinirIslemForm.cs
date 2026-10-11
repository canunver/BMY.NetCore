using System;
using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır İşlem Form bilgilerini tutan TASISLEMFORM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABTasinirIslemForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTasinirIslemForm(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASISLEMFORM";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADMIF = "TASISLEMMIF";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSUREC = "TASISLEMSUREC";

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
            /// Fiş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISTARIH = 4,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 5,
            /// <summary>
            /// İşlem tipi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPKOD = 6,
            /// <summary>
            /// Muayene no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUAYENENO = 7,
            /// <summary>
            /// Muayene tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUAYENETARIH = 8,
            /// <summary>
            /// Dayanak no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAYANAKNO = 9,
            /// <summary>
            /// Dayanak tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAYANAKTARIH = 10,
            /// <summary>
            /// Nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDENGELDI = 11,
            /// <summary>
            /// Nereye gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREYEGITTI = 12,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GMUHASEBEKOD = 13,
            /// <summary>
            /// Harcama kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GHARCAMAKOD = 14,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GAMBARKOD = 15,
            /// <summary>
            /// Durum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 16,
            /// <summary>
            /// İşlem tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 17,
            /// <summary>
            /// İşlemi yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 18,
            /// <summary>
            /// Fatura no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURANO = 19,
            /// <summary>
            /// Fatura tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURATARIH = 20,
            /// <summary>
            /// Kime gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KIMEGITTI = 21,
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GYIL = 22,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GFISNO = 23,

            KOD = 24,
            ONAYDURUM = 25,
            ACIKLAMA = 26,
            ONAYACIKLAMA = 27,
            MUTEMMIM = 28
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO","FISTARIH", "AMBARKOD", "ISLEMTIPIKOD",
                                    "MUAYENENO","MUAYENETARIH","DAYANAKNO","DAYANAKTARIH","NEREDENGELDI",
                                    "NEREYEGITTI","GMUHASEBEKOD","GHARCAMAKOD","GAMBARKOD","DURUM","ISLEMTARIH","ISLEMYAPAN","FATURANO","FATURATARIH","KIMEGITTI","GYIL","GFISNO","KOD","ONAYDURUM","ACIKLAMA","ONAYACIKLAMA", "MUTEMMIM"};

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarMIF
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
            /// Belge no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGENO = 4,
            /// <summary>
            /// Durum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 5,
            /// <summary>
            /// Belge tür alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARMIF = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "BELGENO", "DURUM", "BELGETUR" };

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSurec
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
            /// Belge no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSUREC = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "SURECNO" };


        /// <summary>
        /// Taşınır İşlem Form bilgilerini veri tabanına kaydeden,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Ekleme, Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
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
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTIPKOD], tf.islemTipKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUAYENENO], tf.muayeneNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUAYENETARIH], tf.muayeneTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAYANAKNO], tf.dayanakNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAYANAKTARIH], tf.dayanakTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], tf.faturaNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURATARIH], tf.faturaTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREDENGELDI], tf.neredenGeldi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREYEGITTI], tf.nereyeGitti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMEGITTI], tf.kimeGitti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GMUHASEBEKOD], tf.gMuhasebeKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GHARCAMAKOD], tf.gHarcamaKod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GAMBARKOD], tf.gAmbarKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], tf.durum) + ",";
                //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], tf.islemYapan) + ",";
                //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], tf.islemTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GYIL], tf.gYil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GFISNO], tf.gFisNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], tf.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYDURUM], tf.onayDurum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], tf.aciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYACIKLAMA], tf.onayAciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUTEMMIM], tf.mutemmim);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));

                KaydetTarihce(tf.yil, tf.muhasebeKod, tf.harcamaKod, tf.fisNo, tf.islemYapan, TasinirIslemForm.FormDurumAciklama((int)ENUMBelgeDurumu.DEGISTIRILDI), tf.kod, tf.ipAdresi);
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
                sql += VeriErisim.DegerYap(tf.islemTipKod) + ",";
                sql += VeriErisim.DegerYap(tf.muayeneNo) + ",";
                sql += VeriErisim.DegerYap(tf.muayeneTarih) + ",";
                sql += VeriErisim.DegerYap(tf.dayanakNo) + ",";
                sql += VeriErisim.DegerYap(tf.dayanakTarih) + ",";
                sql += VeriErisim.DegerYap(tf.neredenGeldi) + ",";
                sql += VeriErisim.DegerYap(tf.nereyeGitti) + ",";
                sql += VeriErisim.DegerYap(tf.gMuhasebeKod) + ",";
                sql += VeriErisim.DegerYap(tf.gHarcamaKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(tf.gAmbarKod) + ",";
                sql += VeriErisim.DegerYap(tf.durum) + ",";
                sql += VeriErisim.DegerYap(tf.islemTarih) + ",";
                sql += VeriErisim.DegerYap(tf.islemYapan) + ",";
                sql += VeriErisim.DegerYap(tf.faturaNo) + ",";
                sql += VeriErisim.DegerYap(tf.faturaTarih) + ",";
                sql += VeriErisim.DegerYap(tf.kimeGitti) + ",";
                sql += VeriErisim.DegerYap(tf.gYil) + ",";
                sql += VeriErisim.DegerYap(tf.gFisNo) + ",";
                sql += VeriErisim.DegerYap(tf.kod) + ",";
                sql += VeriErisim.DegerYap(tf.onayDurum) + ",";
                sql += VeriErisim.DegerYap(tf.aciklama) + ",";
                sql += VeriErisim.DegerYap(tf.onayAciklama) + ",";
                sql += VeriErisim.DegerYap(tf.mutemmim) + ")";

                KaydetTarihce(tf.yil, tf.muhasebeKod, tf.harcamaKod, tf.fisNo, tf.islemYapan, TasinirIslemForm.FormDurumAciklama((int)ENUMBelgeDurumu.YENI), tf.kod, tf.ipAdresi);
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Fişlerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Fiş bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader FisAc(TasinirIslemForm tf)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASMUHASEBE.AD, TASIL.KOD, TASIL.AD, TASILCE.KOD, TASILCE.AD, TASHARCAMABIRIM.AD, TASAMBAR.AD FROM " + TABLOAD;

            sql += " JOIN TASMUHASEBE ON TASISLEMFORM.MUHASEBEKOD = TASMUHASEBE.KOD";
            sql += " JOIN TASILCE ON TASMUHASEBE.ILKOD=TASILCE.ILKOD AND TASMUHASEBE.ILCEKOD=TASILCE.KOD JOIN TASIL ON TASILCE.ILKOD = TASIL.KOD";
            sql += " JOIN TASHARCAMABIRIM ON TASISLEMFORM.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD AND TASISLEMFORM.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD";
            sql += " JOIN TASAMBAR ON TASISLEMFORM.AMBARKOD = TASAMBAR.KOD AND TASISLEMFORM.HARCAMABIRIMKOD=TASAMBAR.HARCAMABIRIMKOD AND TASISLEMFORM.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND TASISLEMFORM.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD";

            if (tf.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            if (tf.fisNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo.Trim()));
            if (tf.muhasebeKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod.Trim()));
            if (tf.harcamaKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Trim().Replace(".", "")));
            if (tf.gYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GYIL], "=", tf.gYil));
            if (tf.gMuhasebeKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GMUHASEBEKOD], "=", tf.gMuhasebeKod.Trim()));
            if (tf.gHarcamaKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GHARCAMAKOD], "=", tf.gHarcamaKod.Trim().Replace(".", "")));
            if (tf.gFisNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GFISNO], "=", tf.gFisNo.Trim()));

            if (tf.islemTipKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTIPKOD], "=", tf.islemTipKod));


            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form ve Taşınır Form Kriter bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Form bilgisini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="kriter">Taşınır Form Kriter bilgisini tutan, TasinirFormKriter türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Form bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(TasinirIslemForm tf, TasinirFormKriter kriter)
        {
            string secAlanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TASMUHASEBE.AD, TASHARCAMABIRIM.AD, TASAMBAR.AD, TASISLEMTIP.AD";

            string sql = "SELECT " + secAlanlar;
            sql += ", (SELECT SUM(D.BIRIMFIYAT) FROM TASISLEMDETAY D WHERE TASISLEMFORM.YIL = D.YIL AND TASISLEMFORM.MUHASEBEKOD = D.MUHASEBEKOD AND TASISLEMFORM.HARCAMABIRIMKOD = D.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO = D.FISNO) AS BIRIMFIYAT ";
            sql += ", (SELECT COUNT(*) FROM TASISLEMDOSYA D WHERE TASISLEMFORM.YIL = D.YIL AND TASISLEMFORM.MUHASEBEKOD = D.MUHASEBEKOD AND TASISLEMFORM.HARCAMABIRIMKOD = D.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO = D.FISNO) AS DOSYASAYISI ";
            sql += " FROM TASHARCAMABIRIM INNER JOIN ";
            sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD INNER JOIN ";
            sql += " TASAMBAR ON TASHARCAMABIRIM.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND ";
            sql += " TASHARCAMABIRIM.KOD = TASAMBAR.HARCAMABIRIMKOD INNER JOIN ";
            sql += " TASISLEMFORM ON TASAMBAR.MUHASEBEKOD = TASISLEMFORM.MUHASEBEKOD AND ";
            sql += " TASAMBAR.HARCAMABIRIMKOD = TASISLEMFORM.HARCAMABIRIMKOD AND TASAMBAR.KOD = TASISLEMFORM.AMBARKOD INNER JOIN ";
            sql += " TASISLEMTIP ON TASISLEMFORM.ISLEMTIPIKOD = TASISLEMTIP.KOD ";

            if (kriter.hesapKodu != string.Empty || kriter.degilHesapKodu != string.Empty)
            {
                sql += " INNER JOIN TASISLEMDETAY ON TASISLEMFORM.YIL = TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD = TASISLEMDETAY.MUHASEBEKOD AND ";
                sql += " TASISLEMFORM.HARCAMABIRIMKOD = TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO = TASISLEMDETAY.FISNO ";
            }

            //B-A Onayı yetki kontnrolü için eklendi. HÖ - 27.20.2017
            if (tf.onayDurum == -100)
            {
                sql += " INNER JOIN ";
                sql += " TASISLEMONAYYETKI ON TASISLEMFORM.MUHASEBEKOD = TASISLEMONAYYETKI.MUHASEBEKOD AND ";
                sql += " TASISLEMFORM.HARCAMABIRIMKOD = TASISLEMONAYYETKI.HARCAMABIRIMKOD AND ";
                sql += " TASISLEMFORM.ONAYDURUM = TASISLEMONAYYETKI.ONAYTUR ";

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASISLEMONAYYETKI", "MERNIS", "=", kriter.mernis));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));

                if (kriter.mernis != string.Empty)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMONAYYETKI", "MERNIS", "=", kriter.mernis));
            }

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
            if (!kriter.durumTarihBasla.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", kriter.durumTarihBasla));
            if (!kriter.durumTarihBit.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<=", kriter.durumTarihBit));
            if (kriter.islemYapan != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", kriter.islemYapan));

            if (kriter.faturaNo != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURANO], "LIKE", "%" + kriter.faturaNo.Trim() + "%"));

            if (kriter.aciklama != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + kriter.aciklama.Trim() + "%"));

            if (kriter.hesapKodu != string.Empty)
            {
                kriter.hesapKodu = kriter.hesapKodu.Replace("@", "");
                string[] aranan = kriter.hesapKodu.Split(' ');
                string ekKosul = "";
                for (int i = 0; i < aranan.Length; i++)
                {
                    if (aranan[i] == "") continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("TASISLEMDETAY", "HESAPPLANKOD", "LIKE", aranan[i].Replace(".", "") + "%"));
                }
                if (ekKosul != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
            }

            //Merkez bankasında normal taşınır sorgusunda ayrimenkul ve yazılım gelmesin
            if (kriter.degilHesapKodu != string.Empty)
            {
                kriter.degilHesapKodu = kriter.degilHesapKodu.Replace("@", "");
                string[] aranan = kriter.degilHesapKodu.Split(' ');
                string ekKosul = "";
                for (int i = 0; i < aranan.Length; i++)
                {
                    if (aranan[i] == "") continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("TASISLEMDETAY", "HESAPPLANKOD", "NOT LIKE", aranan[i].Replace(".", "") + "%"));
                }
                if (ekKosul != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
            }


            if (kriter.islemTipi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTIPKOD], "=", kriter.islemTipi));
            if (kriter.nereyeVeridi != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NEREYEGITTI], "LIKE", kriter.nereyeVeridi + "%"));
            if (kriter.kimeVeridi != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMEGITTI], "LIKE", kriter.kimeVeridi + "%"));
            if (kriter.neredenGeldi != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NEREDENGELDI], "LIKE", kriter.neredenGeldi + "%"));
            if (tf.gMuhasebeKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GMUHASEBEKOD], "=", tf.gMuhasebeKod.Replace(".", "")));
            if (tf.gHarcamaKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GHARCAMAKOD], "=", tf.gHarcamaKod.Replace(".", "")));
            if (tf.gAmbarKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GAMBARKOD], "=", tf.gAmbarKod.Replace(".", "")));
            if (tf.gYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GYIL], "=", tf.gYil));
            if (tf.gFisNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GFISNO], "=", tf.gFisNo.Trim()));
            if (tf.kod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", tf.kod));
            if (tf.onayDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYDURUM], "=", tf.onayDurum));
            if (tf.aciklama != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "=", tf.aciklama));


            sql += " GROUP BY " + secAlanlar;

            sql += " ORDER BY 1,2,3,4";

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form ve Durum bilgisine göre,
        /// Taşınır İşlem Formunun durumunu değiştiren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="yeniDurum">Güncellencek durum bilgisini tutan, String türünde nesne</param>
        /// <returns>Güncelleme komutunun çalışması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BelgeDurumDegistir(TasinirIslemForm tf, string yeniDurum)
        {
            int durum = TABGenel.FormDurumNoVer(yeniDurum);
            if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

            string sql = "UPDATE " + TABLOAD + " SET ";

            if (yeniDurum == "Onay" && OrtakFonksiyonlar.WebConfigOku("TIFFisTarihiOnayTarihiOlsun", "0") == "1")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISTARIH], new TNSDateTime(DateTime.Now.Date)) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], tf.islemYapan);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Trim().Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KHKFisNoGuncelle(TasinirIslemForm tf)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            if (tf.gFisNo != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GFISNO], tf.gFisNo);
            else if (tf.kimeGitti != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMEGITTI], tf.kimeGitti);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Trim().Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe Kodu, Harcama Birim Kodu, Fiş No, İşlem Yapan, İşlem bilgisine göre,
        /// Taşınır İşlem Formunun tarhiçesini veri tabanına ekleyen,
        /// sql cümlesini oluşturup çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebeKod">Muhasebe Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaBirimKod">Harcama Birim Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="fisNo">Fiş no bilgisin tutan, String türünde nesne</param>
        /// <param name="islemYapan">İşlem Yapan bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        public void KaydetTarihce(int yil, string muhasebeKod, string harcamaBirimKod, string fisNo, string islemYapan, string islem, string tasinirIslemFormKod, string ipAdresi)
        {
            string sql = "";

            sql = "INSERT INTO TASISLEMFORMTARIHCE (YIL,MUHASEBEKOD,HARCAMABIRIMKOD,FISNO,ISLEMYAPAN,ISLEM,ISLEMTARIH,TASISLEMFORMKOD,IPADRESI) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(harcamaBirimKod.Trim().Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fisNo.Trim()) + ", ";
            sql = sql + VeriErisim.DegerYap(islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(tasinirIslemFormKod) + ",";
            sql = sql + VeriErisim.DegerYap(ipAdresi) + ")";

            KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Formunun durumunu döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Taşınır İşlem Form durum bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BelgeDurumGetir(TasinirIslemForm tf)
        {
            string sql = "SELECT TASISLEMFORM.DURUM FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Trim().Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod.Trim()));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Formu bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Belge Durumunu döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Formu bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Belge Durum bilgisini tutan Int türünde nesne döndürür</returns>
        public int BelgeDurumGetirTekKolon(TasinirIslemForm tf)
        {
            string sql = "SELECT TASISLEMFORM.DURUM FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Trim().Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod.Trim()));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Devir Giriş Kaydı Yapılan kayıtları döndüren,
        /// sql cümlesini oluşturup çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Devir Giriş Kaydı yapılan kayıt bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader DevirGirisKaydiYapilmismi(TasinirIslemForm tf)
        {
            string sql = "SELECT YIL,FISNO,DURUM FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GYIL], "=", tf.gYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GFISNO], "=", tf.gFisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GHARCAMAKOD], "=", tf.gHarcamaKod.Replace(".", "").Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GMUHASEBEKOD], "=", tf.gMuhasebeKod.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));
            if (tf.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
            if (tf.harcamaKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));

            if (tf.fisNo != "")
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "<>", tf.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "<>", tf.fisNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<>", tf.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "<>", tf.harcamaKod.Replace(".", "")));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Yıl Devri Giriş Kaydı Yapılan kayıtları döndüren,
        /// sql cümlesini oluşturup çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Yıl Devri Giriş Kaydı yapılan kayıt bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader YilDevriGirisKaydiYapilmismi(TasinirIslemForm tf)
        {
            string sql = "SELECT YIL,FISNO FROM " + TABLOAD;
            sql += " INNER JOIN " + TABIslemTip.TABLOAD + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMTIPKOD] + " = " + TABIslemTip.TABLOAD + "." + TABIslemTip.ALANLAR[(int)TABIslemTip.ENUMAlanlar.KOD];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", tf.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABIslemTip.TABLOAD, TABIslemTip.ALANLAR[(int)TABIslemTip.ENUMAlanlar.TUR], "=", (int)ENUMIslemTipi.YILDEVIRGIRIS));
            if (tf.gYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GYIL], "=", tf.gYil));
            else if (tf.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));

            if (tf.gFisNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GFISNO], "=", tf.gFisNo));

            if (tf.fisNo != "")
            {
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "<>", tf.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "<>", tf.fisNo));
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<>", tf.muhasebeKod));
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "<>", tf.harcamaKod.Replace(".", "")));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Satın Alma Kaydı Yapılan kayıtları döndüren,
        /// sql cümlesini oluşturup çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Satın Alma Kaydı yapılan kayıt bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SatinAlmaKaydiYapilmismi(TasinirIslemForm tf)
        {
            string sql = "SELECT YIL,FISNO FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], ">", 0));

            //Fatura no kontrolü birimden bağımsız olarak yapılsın Cumhurbaşkanlığı 24.12.2021 Melih
            //Fatura no kontrolü tarihten bağımsız olarak yapılsın Cumhurbaşkanlığı 08.12.2021 Melih
            if (!tf.harcamaKod.StartsWith("01") && !tf.harcamaKod.StartsWith("02"))//Cumhurbaşkanlığı ve meclis=sosyalsgb sadece fiş nodan kontrol istiyor
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURATARIH], "=", tf.faturaTarih));
            }

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURANO], "=", tf.faturaNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NEREDENGELDI], "=", tf.neredenGeldi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));

            if (tf.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));

            if (tf.fisNo != "")
            {
                string icSql = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
                icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));
                sql = VeriErisim.KosulEkle(sql, "AND", " NOT (" + icSql + ")");
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form ve Belge Numarasına göre,
        /// Muhasebe İşlem Fişi(MİF)'ni veri tabanına kaydeden,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="belgeNo">Belge No bilgisini tutan, String türünde nesne</param>
        /// <returns>Ekleme, Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc MIFNoKaydet(TasinirIslemForm f, string belgeNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOADMIF + " (" + VeriErisim.TumAlanlar("", ALANLARMIF) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(f.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(f.fisNo) + ",";
            sql += VeriErisim.DegerYap(belgeNo) + ",";
            sql += VeriErisim.DegerYap(f.durum) + ",";
            sql += VeriErisim.DegerYap(0) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADMIF + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARMIF[(int)ENUMAlanlarMIF.BELGENO], belgeNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARMIF[(int)ENUMAlanlarMIF.DURUM], f.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARMIF[(int)ENUMAlanlarMIF.BELGETUR], 0);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.MUHASEBEKOD], "=", f.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.HARCAMABIRIMKOD], "=", f.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.FISNO], "=", f.fisNo));

                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Sicil No Hareket bilgisine göre,
        /// Muhasebe İşlem Fişi(MİF)'in durumunu iptal olarak veri tabanında güncelleyen,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Sicil No Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc MIFNoIptal(SicilNoHareket f)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOADMIF + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMIF[(int)ENUMAlanlarMIF.DURUM], f.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.MUHASEBEKOD], "=", f.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.HARCAMABIRIMKOD], "=", f.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.FISNO], "=", f.fisNo));

            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Muhasebe İşlem Fişi(MİF) bilgisini döndüren,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Muhasebe İşlem Fişi bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader MIFNoListele(TasinirIslemForm f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMIF) + " FROM " + TABLOADMIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", f.yil));

            if (f.muhasebeKod.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.MUHASEBEKOD], "=", f.muhasebeKod));
            if (f.harcamaKod.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.HARCAMABIRIMKOD], "=", f.harcamaKod.Replace(".", "")));
            if (f.fisNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.FISNO], "=", f.fisNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe, Harcama Kodu ve Fiş no bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Muhasebe İşlem Fişi(MİF) Numarasını döndüren,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebe">Muhasebe bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaKod">Harcamam Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="fisNo">Fiş no bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Muhasebe İşlem Fiş Numarası bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public string MIFNover(int yil, string muhasebe, string harcamaKod, string fisNo)
        {
            string sql = "";

            sql = "SELECT " + ALANLARMIF[(int)ENUMAlanlarMIF.BELGENO] + " FROM " + TABLOADMIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.HARCAMABIRIMKOD], "=", harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMIF[(int)ENUMAlanlarMIF.FISNO], "=", fisNo));

            return veriErisim.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form ve Belge Numarasına göre,
        /// Muhasebe İşlem Fişi(MİF)'ni veri tabanına kaydeden,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="belgeNo">Belge No bilgisini tutan, String türünde nesne</param>
        /// <returns>Ekleme, Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc SurecNoKaydet(TasinirIslemForm f, string surecNo)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "INSERT INTO " + TABLOADSUREC + " (" + VeriErisim.TumAlanlar("", ALANLARSUREC) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(f.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(f.fisNo) + ",";
            sql += VeriErisim.DegerYap(surecNo) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOADSUREC + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO], surecNo);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", f.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD], "=", f.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.HARCAMABIRIMKOD], "=", f.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.FISNO], "=", f.fisNo));

                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Muhasebe İşlem Fişi(MİF) bilgisini döndüren,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Muhasebe İşlem Fişi bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SurecNoListele(TasinirIslemForm f)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSUREC) + " FROM " + TABLOADSUREC;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", f.yil));

            if (f.muhasebeKod.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD], "=", f.muhasebeKod));
            if (f.harcamaKod.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.HARCAMABIRIMKOD], "=", f.harcamaKod.Replace(".", "")));
            if (f.fisNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.FISNO], "=", f.fisNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe ve Harcama Kodunu kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır İşlem Form Numarasını döndüren,
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebe">Muhasebe bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaKod">Harcama kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Taşınır İşlem Form Numarası bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader HarcamaYonetimiIcinTIFNoVer(int yil, string muhasebe, string harcamaKod)
        {
            string sql = "SELECT FISNO,DURUM FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", harcamaKod.Replace(".", "")));
            sql += " ORDER BY FISNO DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DagitimIadeNoKaydet(int yil, string muhasebe, string harcamaKod, string fisNo, string yeniFisNo)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAYANAKNO], yeniFisNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarMIF.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarMIF.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarMIF.HARCAMABIRIMKOD], "=", harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarMIF.FISNO], "=", fisNo));

            return KomutCalistir(sql, 1);
        }

        //**************************************
        /// <summary>
        /// Gümrük bakanlığı kayıtlarını aktarmak için
        /// </summary>
        /// <returns></returns>
        public IDataReader GumrukTIFAl()
        {
            string sql = "SELECT MUHASEBEKOD, HARCAMABIRIMKOD, FISNO FROM _TASISLEMFORM ORDER BY MUHASEBEKOD, HARCAMABIRIMKOD, FISNO DESC";
            return veriErisim.DataReaderDondur(sql);
        }
        public string GumrukNoAl(string kod)
        {
            string sql = "SELECT _sayac FROM _sayac WHERE KOD='" + kod + "'";
            return veriErisim.TekKolonSQLStr(sql);
        }
        public Sonuc GumrukGuncelle(string muhasebe, string birim, string yeniFisNo, string eskiFisNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "UPDATE _TASISLEMFORM SET FISNO='" + yeniFisNo + "',MUAYENENO=MUAYENENO+'XX' WHERE MUHASEBEKOD='" + muhasebe + "' AND HARCAMABIRIMKOD='" + birim + "' AND FISNO='" + eskiFisNo + "'";
            veriErisim.ExecuteNonquery(sql);
            if (veriErisim.hata != "")
                sonuc.islemSonuc = false;

            sql = "UPDATE _TASISLEMDETAY SET FISNO='" + yeniFisNo + "',ESICILNO='X' WHERE MUHASEBEKOD='" + muhasebe + "' AND HARCAMABIRIMKOD='" + birim + "' AND FISNO='" + eskiFisNo + "'";
            veriErisim.ExecuteNonquery(sql);
            if (veriErisim.hata != "")
                sonuc.islemSonuc = false;

            return sonuc;
        }
        //**************************************

        public int OnayDurumGetirTekKolon(TasinirIslemForm tf)
        {
            string sql = "SELECT TASISLEMFORM.ONAYDURUM FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", tf.kod));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc OnayDurumDegistir(TasinirIslemForm tf, ENUMTasinirIslemFormOnayDurumu yeniDurum, string onayAciklama)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            if (!string.IsNullOrWhiteSpace(onayAciklama))
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYACIKLAMA], onayAciklama) + ",";

            if (!tf.fisTarih.isNull && tf.fisTarih.Yil > 1)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISTARIH], tf.fisTarih) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYDURUM], (int)yeniDurum);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", tf.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader BAOnayiYapilacaklarListele(string mernis, BAOnay kriter)
        {
            string sql = "SELECT YIL, MUHASEBEKOD, HARCAMABIRIMKOD, FISNO, FISTARIH, AMBARKOD, ISLEMTIPIKOD, MUAYENENO, MUAYENETARIH, DAYANAKNO, DAYANAKTARIH, NEREDENGELDI, NEREYEGITTI, ";
            sql += " GMUHASEBEKOD, GHARCAMAKOD, GAMBARKOD, DURUM, ISLEMTARIH, ISLEMYAPAN, FATURANO, FATURATARIH, KIMEGITTI, GYIL, GFISNO, KOD, ONAYDURUM, ACIKLAMA, ONAYACIKLAMA, MUHASEBEAD, HARCAMAAD, AMBARAD, TASISLEMTIP, MERNIS, DONEM ";
            sql += " FROM TASBAONAYLISTEVIEW ";
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "MERNIS", "=", mernis));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));

            if (!kriter.bOnayTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "FISTARIH", ">=", kriter.bOnayTarih));

            if (!kriter.aOnayTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "FISTARIH", "<=", kriter.aOnayTarih));

            if (kriter.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", kriter.muhasebeKod));

            if (kriter.harcamaKod != "")
            {
                string ekKriter = "";

                foreach (string harcamaKod in kriter.harcamaKod.Split(','))
                {
                    if (ekKriter != "")
                        ekKriter += " OR ";
                    ekKriter += "HARCAMABIRIMKOD = " + VeriErisim.DegerYap(harcamaKod);
                }

                sql += " AND (" + ekKriter + ")";
            }


            if (kriter.ambarKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "AMBARKOD", "=", kriter.ambarKod));

            if (kriter.onayDurum == (int)ENUMTasinirIslemFormOnayDurumu.ONAY)//ONAY SAYFASI İÇİN
                sql += " AND (ONAYDURUM = 1 OR ONAYDURUM = 2)";
            else //ONAY SORGU SAYFASI İÇİN
                sql += " AND ONAYDURUM = " + kriter.onayDurum;

            sql += " ORDER BY 1,2,3,4";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BAOnayiSorgulama(BAOnay tf)
        {
            string sql = "SELECT DISTINCT YIL, MUHASEBEKOD, HARCAMABIRIMKOD, FISNO";
            sql += " ,(SELECT MAX(ISLEMTARIH) FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'B Onayına Gönderildi') BYEGONDERTARIH";
            sql += " ,(SELECT ISLEMYAPAN FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'B Onayına Gönderildi'  AND ISLEMTARIH = (SELECT MAX(ISLEMTARIH) FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'B Onayına Gönderildi')) BYEGONDEREN";
            sql += " ,(SELECT MAX(ISLEMTARIH) FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'A Onayına Gönderildi') BONAYLAMATARIH";
            sql += " ,(SELECT ISLEMYAPAN FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'A Onayına Gönderildi'  AND ISLEMTARIH = (SELECT MAX(ISLEMTARIH) FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'A Onayına Gönderildi')) BONAYLAYAN";
            sql += " ,(SELECT MAX(ISLEMTARIH) FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'Onaylandı') AONAYLAMATARIH";
            sql += " ,(SELECT ISLEMYAPAN FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'Onaylandı' AND ISLEMTARIH = (SELECT MAX(ISLEMTARIH) FROM TASISLEMFORMTARIHCE IC WHERE IC.TASISLEMFORMKOD = TASISLEMFORMTARIHCE.TASISLEMFORMKOD AND ISLEM = 'Onaylandı')) AONAYLAYAN";
            sql += " FROM TASISLEMFORMTARIHCE";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "ISLEMTARIH", ">=", tf.bOnayTarih));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMTARIH", "<=", tf.aOnayTarih));

            if (tf.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MUHASEBEKOD", "=", tf.muhasebeKod));
            if (tf.harcamaKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "HARCAMABIRIMKOD", "=", tf.harcamaKod));

            if (tf.aOnaylayan != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISLEMYAPAN", "LIKE", tf.aOnaylayan + "%"));

            sql += " ORDER BY YIL, MUHASEBEKOD, HARCAMABIRIMKOD, FISNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public bool OnaylanmamisBelgeVarMi(TasinirIslemForm form)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "<>", 9)); //İptaller Hariç
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ONAYDURUM], "<>", 3)); //B/A onayı tamamlananlar hariç
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "<>", "aktarim")); //Aktarım ile gelen kayıtlar hariç

            if (!form.fisTarih.isNull)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISTARIH], ">=", form.fisTarih));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISTARIH], "<", form.islemTarih));
            }

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        public bool OnaylanmamisGeciciIhracKaydiVarMi(string sicilNo)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD + " F";
            sql += " INNER JOIN TASISLEMDETAY D ON F.YIL = D.YIL AND F.MUHASEBEKOD = D.MUHASEBEKOD AND F.HARCAMABIRIMKOD = D.HARCAMABIRIMKOD AND F.FISNO = D.FISNO ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("F", ALANLAR[(int)ENUMAlanlar.DURUM], "=", 1)); //Onaysız
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLAR[(int)ENUMAlanlar.ONAYDURUM], "<>", 3)); //B/A onayı tamamlananlar hariç
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", ALANLAR[(int)ENUMAlanlar.ISLEMTIPKOD], "=", 24)); //Geçici İhraç


            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "SICILNO", "=", sicilNo)); //sicilNo
            string[] aranan = sicilNo.Split(',');
            string ekKosul = "";
            for (int i = 0; i < aranan.Length; i++)
            {
                if (aranan[i] == "") continue;
                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("D", "SICILNO", "=", aranan[i]));
            }
            if (ekKosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");


            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
    }
}

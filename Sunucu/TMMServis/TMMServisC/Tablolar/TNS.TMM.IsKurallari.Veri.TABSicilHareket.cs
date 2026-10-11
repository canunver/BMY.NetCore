using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Sicil hareket bilgilerini tutan TASSICILNOHAREKET veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSicilHareket : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSicilHareket(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TASSICILNOHAREKET";

        /// <summary>
        /// Taşınır sicil no verme şeklini konfigürasyon dosyasında alarak tutan değişken
        /// </summary>
        public static string verisSekli = System.Configuration.ConfigurationManager.AppSettings.Get("TasinirSicilNoVermeTuru") + "";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Programatik sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSICILNO = 0,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 2,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 3,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 4,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 5,
            /// <summary>
            /// Fiş sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISSIRANO = 6,
            /// <summary>
            /// Sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNO = 7,
            /// <summary>
            /// Sicil sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILSIRANO = 8,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 9,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 10,
            /// <summary>
            /// Fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FIYAT = 11,
            /// <summary>
            /// Kdv oran alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 12,
            /// <summary>
            /// İşlem tipi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTIPIKOD = 13,
            /// <summary>
            /// İşlem türü kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTURUKOD = 14,
            /// <summary>
            /// İşlem tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 15,
            /// <summary>
            /// İşlem yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 16,
            /// <summary>
            /// Durum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 17,
            /// <summary>
            /// Fiş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISTARIH = 18,
            /// <summary>
            /// Nereden geldi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREDENGELDI = 19,
            /// <summary>
            /// Nereye gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREYEGITTI = 20,
            /// <summary>
            /// Kime gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KIMEGITTI = 21,
            /// <summary>
            /// Zimmet özellik alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ZIMMETOZELLIK = 22,
            /// <summary>
            /// Kdvli birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVLIBIRIMFIYAT = 23,
            /// <summary>
            /// Değer artışı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERARTISI = 24,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GONMUHASEBEKOD = 25,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GONHARCAMABIRIMKOD = 26,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GONAMBARKOD = 27,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "PRSICILNO", "SIRANO", "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "FISSIRANO", "SICILNO", "SICILSIRANO", "HESAPPLANKOD", "AMBARKOD", "FIYAT", "KDVORAN", "ISLEMTIPIKOD", "ISLEMTURUKOD", "ISLEMTARIH", "ISLEMYAPAN", "DURUM", "FISTARIH", "NEREDENGELDI", "NEREYEGITTI", "KIMEGITTI", "ZIMMETOZELLIK", "KDVLIBIRIMFIYAT", "DEGERARTISI", "GONMUHASEBEKOD", "GONHARCAMABIRIMKOD", "GONAMBARKOD" };


        public static string TABLOADNUMARA = "TASSICILNONUMARA";

        public enum ENUMAlanlarNumara : int
        {

            PRSICILNO = 0,
            YIL = 1,
            MUHASEBEKOD = 2,
            HARCAMABIRIMKOD = 3,
            AMBARKOD = 4,
            FISNO = 5
        }

        public static string[] ALANLARNUMARA = { "PRSICILNO", "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "FISNO" };

        /// <summary>
        /// Sicil Hareket bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <param name="muhasebeKdvMukellefiMi">KDV Mükellefi durumuna göre alanların değişip değişmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Ekleme, Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(SicilNoHareket sicilNo, bool muhasebeKdvMukellefiMi)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc(false, "", "");

            int siraNo = sicilNo.siraNo;
            if (siraNo == 0)
            {
                sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", sicilNo.prSicilNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMSicilDurum.IPTAL));
                siraNo = veriErisim.TekKolonSQLInt(sql) + 1;
                if (!string.IsNullOrEmpty(veriErisim.hata))
                {
                    sonuc.hataStr = TasinirMalResI.TMM004;
                    return sonuc;
                }
            }

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sicilNo.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(sicilNo.yil) + ",";
            sql += VeriErisim.DegerYap(sicilNo.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(sicilNo.harcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sicilNo.fisNo) + ",";
            sql += VeriErisim.DegerYap(sicilNo.fisSiraNo) + ",";
            sql += VeriErisim.DegerYap(sicilNo.sicilNo) + ",";
            sql += VeriErisim.DegerYap(sicilNo.sicilSiraNo) + ",";
            sql += VeriErisim.DegerYap(sicilNo.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sicilNo.ambarKod) + ",";
            sql += VeriErisim.DegerYap(sicilNo.fiyat) + ",";
            sql += VeriErisim.DegerYap(sicilNo.kdvOran) + ",";
            sql += VeriErisim.DegerYap(sicilNo.islemTipKod) + ",";
            sql += VeriErisim.DegerYap(sicilNo.islemTurKod) + ",";
            sql += VeriErisim.DegerYap(sicilNo.islemTarih) + ",";
            sql += VeriErisim.DegerYap(sicilNo.islemYapan) + ",";
            sql += VeriErisim.DegerYap(sicilNo.durum) + ",";
            sql += VeriErisim.DegerYap(sicilNo.fisTarih) + ",";
            sql += VeriErisim.DegerYap(sicilNo.neredenGeldi) + ",";
            sql += VeriErisim.DegerYap(sicilNo.nereyeGitti) + ",";
            sql += VeriErisim.DegerYap(sicilNo.kimeGitti) + ",";
            sql += VeriErisim.DegerYap(sicilNo.zimmetOzellik) + ",";

            //Erdal
            //Bazı yerlerden KDV'li Birim Fiyat sıfır geliyordu
            //Tek tek oraları düzeltmektense burada hesapladım
            if (muhasebeKdvMukellefiMi)
                sicilNo.kdvliBirimFiyat = sicilNo.fiyat;
            else
                sicilNo.kdvliBirimFiyat = (1 + (OrtakFonksiyonlar.ConvertToDecimal(sicilNo.kdvOran) / 100)) * sicilNo.fiyat;
            sql += VeriErisim.DegerYap(sicilNo.kdvliBirimFiyat) + ",";

            sql += VeriErisim.DegerYap(sicilNo.degerArtisi) + ",";
            sql += VeriErisim.DegerYap(sicilNo.gonMuhasebeKod) + ",";
            sql += VeriErisim.DegerYap(sicilNo.gonHarcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sicilNo.gonAmbarKod) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";

                if (sicilNo.sicilNo != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], sicilNo.sicilNo) + ",";
                if (sicilNo.sicilSiraNo != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SICILSIRANO], sicilNo.sicilSiraNo) + ",";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], sicilNo.yil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], sicilNo.muhasebeKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], sicilNo.harcamaBirimKod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISNO], sicilNo.fisNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISSIRANO], sicilNo.fisSiraNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], sicilNo.hesapPlanKod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], sicilNo.ambarKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIYAT], sicilNo.fiyat) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVORAN], sicilNo.kdvOran) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD], sicilNo.islemTipKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTURUKOD], sicilNo.islemTurKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], sicilNo.islemTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], sicilNo.islemYapan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], sicilNo.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISTARIH], sicilNo.fisTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREDENGELDI], sicilNo.neredenGeldi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREYEGITTI], sicilNo.nereyeGitti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMEGITTI], sicilNo.kimeGitti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZIMMETOZELLIK], sicilNo.zimmetOzellik) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVLIBIRIMFIYAT], sicilNo.kdvliBirimFiyat) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERARTISI], sicilNo.degerArtisi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GONMUHASEBEKOD], sicilNo.gonMuhasebeKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GONHARCAMABIRIMKOD], sicilNo.gonHarcamaBirimKod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GONAMBARKOD], sicilNo.gonAmbarKod);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", sicilNo.prSicilNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));//Erdal Insert edemediği kaydı update etsin başkalarını etmesin.
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMSicilDurum.IPTAL));

                if (verisSekli.ToUpper() != "KURUM")
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
                }

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisine göre,
        /// Giriş Belgesini veri tabanından silen,
        /// sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc GirisBelgesiIptalEt(SicilNoHareket sicilNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNo.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNo.fisNo));
            sql += " AND NOT (ISLEMTURUKOD=" + (int)ENUMIslemTipi.SATINALMAGIRIS +
                         " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.BAGISGIRIS +
                         " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS +
                         " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.IADEGIRIS +
                         " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.URETILENGIRIS +
                         " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.ACILIS +
                         " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.DEVIRGIRISKURUM + ")";
            sql += " AND ISLEMTIPIKOD<" + (int)ENUMZimmetIslemTipi.ZFVERME;
            sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "UPDATE " + TABLOAD;
                sql += " SET DURUM=" + sicilNo.durum;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNo.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNo.fisNo));
                sql += " AND (ISLEMTURUKOD=" + (int)ENUMIslemTipi.SATINALMAGIRIS +
                             " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.BAGISGIRIS +
                             " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS +
                             " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.IADEGIRIS +
                             " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.URETILENGIRIS +
                             " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.ACILIS +
                             " OR ISLEMTURUKOD=" + (int)ENUMIslemTipi.DEVIRGIRISKURUM + ")";
                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisine göre,
        /// Devir Belgesi durumunu ve programatik sicil numarası bilgilerini güncelleyen,
        /// sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <param name="yeniPrSicilNo">Güncellenecek Programatik Sicil bilgisini tutan, Int türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc DevirBelgesiIptalEt(SicilNoHareket sicilNo, int yeniPrSicilNo)
        {
            //Eğer devir giriş belgesi onayı kaldırılıyorsa Harcama birimi bazında sicil no verilenlerde sicil no kaybı oluyordu
            //Devir çıkış belgeside onayı kaldırılınca 
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOAD;
            sql += " SET DURUM=" + sicilNo.durum;
            sql += " , PRSICILNO=" + yeniPrSicilNo;
            sql += " , SIRANO=1";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNo.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", sicilNo.prSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNo.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD], "<", (int)ENUMZimmetIslemTipi.ZFVERME));

            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisine göre,
        /// Sicil Hareket bilgisini veri tabanından silen,
        /// sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc IptalEt(SicilNoHareket sicilNo)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNo.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNo.fisNo));

            if (sicilNo.islemTurKod == (int)ENUMIslemTipi.ZFVERME || sicilNo.islemTurKod == (int)ENUMIslemTipi.ZFDUSME || sicilNo.islemTurKod == (int)ENUMIslemTipi.DTLVERME || sicilNo.islemTurKod == (int)ENUMIslemTipi.DTLDUSME)
            {
                if ((sicilNo.islemTipKod != (int)ENUMZimmetIslemTipi.ZFVERME && sicilNo.islemTipKod != (int)ENUMZimmetIslemTipi.ZFDUSME && sicilNo.islemTipKod != (int)ENUMZimmetIslemTipi.DTLVERME && sicilNo.islemTipKod != (int)ENUMZimmetIslemTipi.DTLDUSME) || sicilNo.islemTipKod == 0)
                {
                    Sonuc sonuc = new Sonuc();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM005;
                    return sonuc;
                }

                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTURUKOD], "=", sicilNo.islemTurKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD], "=", sicilNo.islemTipKod));
            }
            else
            {
                Sonuc sonuc = new Sonuc();
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM006;
                return sonuc;
            }

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Programatik Sicil ve Sıra numarasını bilgilerini döndüren,
        /// sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Programatik Sicil ve Sıra Numarası bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader PrSicilNoveSiraNoVer(SicilNoHareket sicilNo)
        {
            string sql = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNo.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNo.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISSIRANO], "=", sicilNo.fisSiraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Fiş detayındaki malzemelerin Programatik Sicil Numarası ve Sıra numarasını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil No hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Programatik Sicil ve Sıra Numarası bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader PrSicilNoveSiraNoVerFisteki(SicilNoHareket sicilNo)
        {
            string sql = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNo.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNo.fisNo));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTURUKOD], "<", 90));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTURUKOD], "=", sicilNo.islemTurKod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun En yüksek Devir Çıkışı Sıra Numarasını döndüren,
        /// sql komutunu oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <param name="islemTurKod">The islem tur kod.</param>
        /// <returns>
        /// Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun En yüksek Devir Çıkış Sıra numarasını tutan IDataReader türünde nesne döndürür
        /// </returns>
        public IDataReader DevirCikisiSiraNoVer(SicilNoHareket sicilNo, int islemTurKod)
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.eMuhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.eHarcamaBirimKod.Replace(".", "")));

            if (islemTurKod == (int)ENUMIslemTipi.DEVIRGIRIS)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTURUKOD], "=", (int)ENUMIslemTipi.DEVIRCIKIS));
            else if (islemTurKod == (int)ENUMIslemTipi.DAGITIMIADEGIRIS)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTURUKOD], "=", (int)ENUMIslemTipi.DAGITIMIADECIKIS));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", sicilNo.eSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMSicilDurum.IPTAL));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Programatik Sicil Numarası, Sıra Numarası, Muhasebe Kodu ve Harcama birimini kıstas olarak kullanarak,
        /// kıstasa uygun Malzemenin sıra numarasından sonraki hareketlerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="prSicil">Programatik Sicil Numarası bilgisini tutan, Int türünde nesne</param>
        /// <param name="siraNo">Sıra numarası bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebeKod">Muhasebe kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Hareket olup olmadığı, hareket varsa true yoksa false değerini tutan Bool türünde nesne döndürür</returns>
        public bool SonrasindaHareketVarmi(int prSicil, int siraNo, string muhasebeKod, string harcamaBirimi)
        {
            string sql = "";

            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], ">", siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMSicilDurum.IPTAL));
            if (verisSekli.ToUpper() != "KURUM")
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", harcamaBirimi.Replace(".", "")));
            }

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

        /// <summary>
        /// Parametre olarak verilen Programatik Sicil, Sıra No, Muhasebe Kodu, Harcama birimi ve Başlangıç ve Bitiş Sayaç sayılarını kıstas olarak kullanarak,
        /// kıstasa uygun verilen malzemelerin sıra numarasındaki hareketleri bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="prSicil">Programatik Sicil numarası bilgisini tutan, Int dizi türünde nesne</param>
        /// <param name="siraNo">Sıra numarası bilgisin tutan, Int dizi türünde nesne</param>
        /// <param name="muhasebeKod">Muhasebe kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan, String türünde nesne</param>
        /// <param name="basSayac">Başlangıç sayaç değerini tutan, Int türünde nesne</param>
        /// <param name="bitSayac">Bitiş sayaç değerini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun hareket bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SonrasindakiHareketler(int[] prSicil, int[] siraNo, string muhasebeKod, string harcamaBirimi, int basSayac, int bitSayac)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMSicilDurum.IPTAL));

            string sicilSql = "";
            for (int i = basSayac; i < bitSayac; i++)
            {
                if (prSicil[i] == 0)
                    break;

                if (sicilSql != "") sicilSql += " OR ";
                sicilSql += "(";
                sicilSql = VeriErisim.KosulEkle(sicilSql, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicil[i]));
                sicilSql = VeriErisim.KosulEkle(sicilSql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], ">", siraNo[i]));
                sicilSql += ")";

            }
            sql += " AND (" + sicilSql + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe, Harcama Birimi, Hesap Plan Kodu, Programatik Sicil ve Sıra No bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun İptal Edilen Sicil numarası bilgilerin döndüren,
        /// sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebe">Muhasebe bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaBirimi">Harcama Birimi bilgisini tutan, String türünde nesne</param>
        /// <param name="hesapPlanKod">Hesap Planı Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="prSicilNo">Programatik Sicil No bilgisini tutan, Int türünde nesne</param>
        /// <param name="siraNo">Sıra No bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun İptal Edilen Sicil numaralarını tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader IptalEdilenNolariVer(int yil, string muhasebe, string harcamaBirimi, string hesapPlanKod, int prSicilNo, int siraNo, string fisNo)
        {
            string sql = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + "," + ALANLAR[(int)ENUMAlanlar.SICILNO] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO] + "," + ALANLAR[(int)ENUMAlanlar.SICILSIRANO] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (muhasebe != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            if (harcamaBirimi != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", harcamaBirimi.Replace(".", "")));
            if (prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            if (siraNo == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            if (fisNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", fisNo));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "=", hesapPlanKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMSicilDurum.IPTAL));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + "," + ALANLAR[(int)ENUMAlanlar.SICILSIRANO];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Ambardaki Sicil numara bilgisini döndüren,
        /// sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ambar Numaralarını tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader AmbardakiSicilNolar(SicilNoHareket sicilNo)
        {
            string sql = "SELECT ";
            sql += "* FROM TASAMBARSICILVIEW ";
            sql += " WHERE MUHASEBEKOD = '" + sicilNo.muhasebeKod + "'";
            if (!string.IsNullOrEmpty(sicilNo.harcamaBirimKod))
                sql += " AND HARCAMABIRIMKOD = '" + sicilNo.harcamaBirimKod.Replace(".", "").Trim() + "'";
            //Erdal //Açık Pazarda ambar yok
            if (!string.IsNullOrEmpty(sicilNo.ambarKod))
                sql += " AND AMBARKOD = '" + sicilNo.ambarKod + "'";
            if (sicilNo.ozellik.markaKod > 0)
                sql += " AND MARKAKOD = " + sicilNo.ozellik.markaKod;
            if (sicilNo.ozellik.modelKod > 0)
                sql += " AND MODELKOD = " + sicilNo.ozellik.modelKod;
            if (sicilNo.ozellik.plaka.Trim() != "")
                sql += " AND PLAKA LIKE '%" + sicilNo.ozellik.plaka + "%'";
            if (sicilNo.hesapPlanKod.Replace(".", "").Trim() != "")
                sql += " AND HESAPPLANKOD LIKE '" + sicilNo.hesapPlanKod.Replace(".", "").Trim() + "%'";
            if (sicilNo.hesapPlanAd.Replace(".", "").Trim() != "")
                sql += " AND HESAPPLANAD LIKE '%" + sicilNo.hesapPlanAd.Replace(".", "").Trim() + "%'";
            if (sicilNo.sicilNo.Replace(".", "").Trim() != "")
            {
                if (sicilNo.sorguSicilNoIcindeGecen)
                    sql += " AND SICILNO LIKE '%" + sicilNo.sicilNo.Replace(".", "").Trim() + "%'";
                else
                    sql += " AND SICILNO ='" + sicilNo.sicilNo.Replace(".", "").Trim() + "'";
            }

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SICILNO", "NOT LIKE", "255070201??13%"));

            if (sicilNo.prSicilNo > 0)
                sql += " AND PRSICILNO =" + sicilNo.prSicilNo;
            if (sicilNo.fiyat > 0 && sicilNo.sicilNo == "")
                sql += " AND round(FIYAT,10) =" + System.Math.Round(sicilNo.fiyat, 10).ToString().Replace(",", ".");
            if (sicilNo.fisNo.Replace(".", "").Trim() != "")
            {
                if (sicilNo.yil > 0)
                    sql += " AND YIL=" + sicilNo.yil;
                sql += " AND FISNO LIKE '" + sicilNo.fisNo + "'";
            }
            else //Erdal yaptı ama Melih beye de sordu :)
            {
                if (sicilNo.yil > 0 && sicilNo.prSicilNo == 0 && sicilNo.sicilNo == "")
                    sql += " AND YIL<=" + sicilNo.yil;
            }
            if (sicilNo.eSicilNo.Trim() != "")
                sql += " AND (DISSICILNO LIKE '%" + sicilNo.eSicilNo + "%'" + " OR DISSICILNO2 LIKE '%" + sicilNo.eSicilNo + "%')";
            if (sicilNo.ozellik.disSicilNo.Trim() != "")
                sql += " AND (DISSICILNO LIKE '%" + sicilNo.ozellik.disSicilNo + "%'" + " OR DISSICILNO2 LIKE '%" + sicilNo.ozellik.disSicilNo + "%')";

            if (sicilNo.sorguPrSicilNoListe != "")
            {
                string[] aranan = sicilNo.sorguPrSicilNoListe.Split('|');
                string ekKosul = "";
                for (int i = 0; i < aranan.Length; i++)
                {
                    if (aranan[i] == "") continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("", "PRSICILNO", "=", OrtakFonksiyonlar.ConvertToInt(aranan[i], 0)));
                }
                if (ekKosul != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
            }


            if (!string.IsNullOrEmpty(sicilNo.ozellik.saseNo))
            {
                string[] parcaliSaseNo = sicilNo.ozellik.saseNo.Split(';');
                string parcaliSQL = string.Empty;
                foreach (string sn in parcaliSaseNo)
                {
                    if (string.IsNullOrEmpty(sn))
                        continue;
                    if (!string.IsNullOrEmpty(parcaliSQL))
                        parcaliSQL += " OR ";
                    parcaliSQL += "SASENO = '" + sn + "'";
                }
                if (!string.IsNullOrEmpty(parcaliSQL))
                    sql += " AND (" + parcaliSQL + ")";
            }

            if (sicilNo.prSicilNo == 0 && sicilNo.sicilNo == "")
                sql += " ORDER BY SICILNO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Ambardaki Bütün Sicil numara bilgilerini döndüren,
        /// sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ambarda bulunan sicil  Numaralarını tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader AmbardakiButunSicilNolar(SicilNoHareket sicilNo, Sayfalama sayfa)
        {
            string sql = "";
            string kosul = "";

            /* Merkez bankası için, ön onaylı (sicil no verme) işlemin raporlarda ve aramalarda gelmemsi için numara verildikten sonra durumu onaysız tutuluyor. 
               Bu nedenlede sicil listelemede gelmesi için TASSICILHAREKETVIEWHEPSI isimli view yapıldı.       
            */

            sql = "SELECT H.*, TASMARKA.AD AS MARKAADI, TASMODEL.AD AS MODELADI";
            sql += " FROM TASMODEL RIGHT OUTER JOIN";
            sql += " TASMARKA ON TASMODEL.MARKAKOD = TASMARKA.KOD RIGHT OUTER JOIN";
            sql += (sicilNo.siraNo == -100 ? " TASSICILHAREKETVIEWHEPSI" : " TASSICILHAREKETVIEW"); //Merkez bankası
            sql += " AS H ON TASMODEL.KOD = H.MODELKOD AND TASMARKA.KOD = H.MARKAKOD ";

            if (sicilNo.yil > 0)
                kosul = " YIL =" + sicilNo.yil;
            else
                kosul = "YIL>0";//koşulun dolu olmaı için

            if (sicilNo.sicilNolar.Length > 0)
            {
                kosul += " and SICILNOHAREKETPRSICILNO in (";
                int cnt = 1;
                foreach (int i in sicilNo.sicilNolar)
                {
                    kosul += "'" + i + "'";
                    if (cnt != sicilNo.sicilNolar.Length)
                        kosul += ",";
                    cnt++;
                }
                kosul += ")";
            }
            else
            {
                if (!string.IsNullOrEmpty(sicilNo.muhasebeKod))
                    kosul += " AND MUHASEBEKOD = '" + sicilNo.muhasebeKod + "'";
                if (!string.IsNullOrEmpty(sicilNo.harcamaBirimKod))
                    kosul += " AND HARCAMABIRIMKOD = '" + sicilNo.harcamaBirimKod.Replace(".", "").Trim() + "'";
                if (!string.IsNullOrEmpty(sicilNo.ambarKod))
                    kosul += " AND AMBARKOD = '" + sicilNo.ambarKod + "'";
                if (sicilNo.ozellik.markaKod > 0)
                    kosul += " AND MARKAKOD = " + sicilNo.ozellik.markaKod;
                if (sicilNo.ozellik.modelKod > 0)
                    kosul += " AND MODELKOD = " + sicilNo.ozellik.modelKod;
                if (sicilNo.ozellik.plaka.Trim() != "")
                    kosul += " AND PLAKA LIKE '%" + sicilNo.ozellik.plaka + "%'";
                if (sicilNo.hesapPlanKod.Replace(".", "").Trim() != "")
                    kosul += " AND HESAPPLANKOD LIKE '" + sicilNo.hesapPlanKod.Replace(".", "").Trim() + "%'";
                if (sicilNo.hesapPlanAd.Replace(".", "").Trim() != "")
                    kosul += " AND HESAPPLANAD LIKE '%" + sicilNo.hesapPlanAd.Replace(".", "").Trim() + "%'";
                if (sicilNo.sicilNo.Replace(".", "").Trim() != "")
                    kosul += " AND SICILNOHAREKETSICILNO LIKE '%" + sicilNo.sicilNo.Replace(".", "").Trim() + "%'";
                if (sicilNo.prSicilNo > 0)
                    kosul += " AND SICILNOHAREKETPRSICILNO =" + sicilNo.prSicilNo;
                if (sicilNo.eSicilNo != "")
                    kosul += " AND (DISSICILNO LIKE '%" + sicilNo.eSicilNo + "%'" + " OR DISSICILNO2 LIKE '%" + sicilNo.eSicilNo + "%')";
                if (sicilNo.ozellik.disSicilNo.Trim() != "")
                    kosul += " AND (DISSICILNO LIKE '%" + sicilNo.ozellik.disSicilNo + "%'" + " OR DISSICILNO2 LIKE '%" + sicilNo.ozellik.disSicilNo + "%')";

                if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SICILNOHAREKETSICILNO", "NOT LIKE", "255070201??13%"));

                if (sicilNo.sorguFisNoTif != "" || sicilNo.fisNo != "")
                {
                    //kosul += " AND YIL =" + sicilNo.yil;
                    if (sicilNo.sorguFisNoTif != "")
                        kosul += " AND FISNO ='" + sicilNo.sorguFisNoTif + "'";
                    else if (sicilNo.fisNo != "")
                        kosul += " AND FISNO ='" + sicilNo.fisNo + "'";
                    kosul += " AND ISLEMTIPKOD < 1000";
                }
                else if (sicilNo.sorguFisNoZimmet != "")
                {
                    //kosul += " AND YIL =" + sicilNo.yil;
                    kosul += " AND FISNO ='" + sicilNo.sorguFisNoZimmet + "'";
                    kosul += " AND ISLEMTIPKOD > 1000";
                }
                else
                    kosul += " AND ((ISLEMTIPTUR >= 1 AND ISLEMTIPTUR <= 50) OR ISLEMTIPTUR >=1000)";

                if (!string.IsNullOrEmpty(sicilNo.ozellik.saseNo))
                {
                    string[] parcaliSaseNo = sicilNo.ozellik.saseNo.Split(';');
                    string parcaliSQL = string.Empty;
                    foreach (string sn in parcaliSaseNo)
                    {
                        if (string.IsNullOrEmpty(sn))
                            continue;
                        if (!string.IsNullOrEmpty(parcaliSQL))
                            parcaliSQL += " OR ";
                        parcaliSQL += "SASENO = '" + sn + "'";
                    }
                    if (!string.IsNullOrEmpty(parcaliSQL))
                        kosul += " AND (" + parcaliSQL + ")";
                }
            }

            if (sicilNo.sadeceBirimdeOlanlar && sicilNo.muhasebeKod != "" && sicilNo.harcamaBirimKod != "")
            {
                //XELEXT dikkat
                kosul += " AND (SICILNOHAREKETPRSICILNO IN " +
                         "(SELECT PRSICILNO XXOX TASDEMIRBASVIEW " +
                         " WHERE MUHASEBEKOD = '" + sicilNo.muhasebeKod + "' " +
                         " AND HARCAMABIRIMKOD = '" + sicilNo.harcamaBirimKod.Replace(".", "").Trim() + "'))";
            }

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "SICILNOHAREKETSICILNO", "NOT LIKE", "255070201??13%"));

            if (kosul != "")
                sql = sql + " WHERE " + kosul;

            string siralama = "";
            if (sayfa.siralamaAlani != "")
                siralama = " ORDER BY " + sayfa.siralamaAlani + " " + sayfa.siralamaYon;
            else
                siralama = " ORDER BY SICILNOHAREKETSICILNO";

            sql = TABGenel.GetPaginatedSQL(sayfa.sayfaNo, sayfa.kayitSayisi, sql, siralama);

            //Özellik sayfasından listelema yapıldığında o birimden çıkış yapmış olan malzemelerde listeleniyor.
            //Paging ise içerde SELECT cümlesi gördüğünde hatalı SQL üretiyor.
            //Bu nedenle bu işlem yapılıyor Melih
            sql = sql.Replace("XXOX", "FROM");

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Zimmette bulunan Sicil numara bilgilerini döndüren,
        /// sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Zimmette bulunan sicil  Numaralarını tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ZimmettekiSicilNolar(SicilNoHareket sicilNo)
        {
            string sql = "";

            sql = "SELECT ";
            sql += "* FROM TASAMBARZIMMETVIEW ";
            sql += " WHERE MUHASEBEKOD = '" + sicilNo.muhasebeKod + "' AND ";
            sql += " HARCAMABIRIMKOD = '" + sicilNo.harcamaBirimKod.Replace(".", "") + "' AND ";
            sql += " AMBARKOD = '" + sicilNo.ambarKod + "'";
            if (sicilNo.kimeGitti != "")
                sql += " AND KIMEGITTI = " + "'" + sicilNo.kimeGitti + "'";
            if (sicilNo.nereyeGitti != "")
                sql += " AND NEREYEGITTI = " + "'" + sicilNo.nereyeGitti + "'";
            if (sicilNo.ozellik.plaka.Trim() != "")
                sql += " AND PLAKA LIKE '%" + sicilNo.ozellik.plaka + "%'";
            if (sicilNo.hesapPlanKod.Replace(".", "").Trim() != "")
                sql += " AND HESAPPLANKOD LIKE '" + sicilNo.hesapPlanKod.Replace(".", "").Trim() + "%'";
            if (sicilNo.hesapPlanAd.Replace(".", "").Trim() != "")
                sql += " AND HESAPPLANAD LIKE '%" + sicilNo.hesapPlanAd.Replace(".", "").Trim() + "%'";
            if (sicilNo.sicilNo.Replace(".", "").Trim() != "")
                sql += " AND SICILNO LIKE '%" + sicilNo.sicilNo.Replace(".", "").Trim() + "%'";
            if (sicilNo.fisNo.Replace(".", "").Trim() != "")
                sql += " AND FISNO LIKE '" + sicilNo.fisNo + "'";
            if (sicilNo.islemTurKod == (int)ENUMIslemTipi.ZFVERME || sicilNo.islemTurKod == (int)ENUMIslemTipi.DTLVERME)
                sql += " AND ISLEMTURUKOD = " + sicilNo.islemTurKod;
            if (sicilNo.eSicilNo.Trim() != "")
                sql += " AND (DISSICILNO LIKE '%" + sicilNo.eSicilNo + "%'" + " OR DISSICILNO2 LIKE '%" + sicilNo.eSicilNo + "%')";
            if (sicilNo.ozellik.disSicilNo.Trim() != "")
                sql += " AND (DISSICILNO LIKE '%" + sicilNo.ozellik.disSicilNo + "%'" + " OR DISSICILNO2 LIKE '%" + sicilNo.ozellik.disSicilNo + "%')";

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SICILNO", "NOT LIKE", "255070201??13%"));

            if (!string.IsNullOrEmpty(sicilNo.ozellik.saseNo))
            {
                string[] parcaliSaseNo = sicilNo.ozellik.saseNo.Split(';');
                string parcaliSQL = string.Empty;
                foreach (string sn in parcaliSaseNo)
                {
                    if (string.IsNullOrEmpty(sn))
                        continue;
                    if (!string.IsNullOrEmpty(parcaliSQL))
                        parcaliSQL += " OR ";
                    parcaliSQL += "SASENO = '" + sn + "'";
                }
                if (!string.IsNullOrEmpty(parcaliSQL))
                    sql += " AND (" + parcaliSQL + ")";
            }

            sql += " ORDER BY SICILNO";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil No Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun malzemeleri döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil No Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun malzeme bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BarkodSicilNolar(SicilNoHareket sicilNo, Sayfalama sayfa)
        {
            string sql = "";

            //Merkez Bankası eski sicil nodan eşit ile arama yaparken Manas Eski sicilk noda birden fazla bilgi olduğu için LIKE araması gerekiyor. Melih 25.02.2018
            string aramaOp = "=";
            string aramaIs = "";
            if (sicilNo.sorguEskiSicilNoAramaLike)
            {
                aramaOp = " LIKE ";
                aramaIs = "%";
            }

            string dosyaVar = "(SELECT COUNT(*) XXOX TASRESIM WHERE V.PRSICILNO=TASRESIM.PRSICILNO)";

            sql = "SELECT V.SICILNO,V.HESAPPLANKOD,V.HESAPPLANAD,V.KISIAD,V.ODAAD,V.ADI,V.YAZARADI,V.DISSICILNO,V.YAYINTARIHI,V.CILTNO,V.YERIKONUSU,V.PRSICILNO" +
                ",V.SASENO,V.MUHASEBEKOD,V.HARCAMABIRIMKOD,V.AMBARKOD,V.KIMEGITTI,V.NEREYEGITTI, RFIDNO, V.KDVORAN, V.KDVLIBIRIMFIYAT, V.FIYAT" +
                ", V.MUHASEBEAD, V.HARCAMABIRIMAD, V.AMBARAD, V.MARKAAD, V.MODELAD,TASISLEMTIP.AD, TASSICILNODURUM.DURUMACIKLAMA, TASSICILNODURUM.DURUMISLEM" +
                ", V.GIAI, V.FISTARIH, V.BISNO, V.ESKIBISNO1, V.EKNO, CASE WHEN OZ.AMORTISMANYIL > 0 THEN OZ.AMORTISMANYIL ELSE V.AMORTIYIL END AS AMORTISMANYIL, V.BULUNDUGUYER, V.BULUNDUGUYERAD, OZ.AMORTISMANBITTI, H.FISTARIH AS ALIMTARIHI" +
                ", V.ISLEMTARIH, " + dosyaVar + " AS DOSYAVAR";
            sql += " FROM TASDEMIRBASVIEW AS V";
            sql += " LEFT OUTER JOIN TASISLEMTIP ON V.ISLEMTIPIKOD = TASISLEMTIP.KOD";
            sql += " LEFT OUTER JOIN TASSICILNODURUM ON V.PRSICILNO = TASSICILNODURUM.PRSICILNO";
            sql += " LEFT JOIN TASSICILNOHAREKET AS H ON V.PRSICILNO = H.PRSICILNO AND H.SIRANO = 1"; //ALIMTARIHI
            sql += " LEFT JOIN TASSICILNOOZELLIK AS OZ ON V.PRSICILNO = OZ.PRSICILNO";

            if (sicilNo.altOdalar && OrtakFonksiyonlar.ConvertToInt(sicilNo.nereyeGitti, 0) > 0)
            {
                sql += " INNER JOIN dbo.TASODAALT(" + OrtakFonksiyonlar.ConvertToInt(sicilNo.nereyeGitti, 0) + ") TASODAALT ON  CAST(TASODAALT.KOD as varchar(100)) = V.NEREYEGITTI ";
            }


            sql += " WHERE V.YIL>0";//koşul doldurmak için

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("V", "SICILNO", "NOT LIKE", "255070201??13%"));

            if (sicilNo.muhasebeKod != "")
            {
                sql += " AND V.MUHASEBEKOD = '" + sicilNo.muhasebeKod + "'  ";
                if (sicilNo.harcamaBirimKod.Replace(".", "").Trim() != "")
                    sql += " AND V.HARCAMABIRIMKOD = '" + sicilNo.harcamaBirimKod.Replace(".", "").Trim() + "'";
            }
            else if (sicilNo.prSicilNo > 0)
                sql += " AND V.PRSICILNO =" + sicilNo.prSicilNo;

            if (sicilNo.ambarKod != "")
                sql += " AND V.AMBARKOD = '" + sicilNo.ambarKod + "'";
            if (sicilNo.ozellik.markaKod > 0)
                sql += " AND V.MARKAKOD = " + sicilNo.ozellik.markaKod;
            if (sicilNo.ozellik.modelKod > 0)
                sql += " AND V.MODELKOD = " + sicilNo.ozellik.modelKod;
            if (sicilNo.ozellik.plaka.Trim() != "")
                sql += " AND V.PLAKA LIKE '%" + sicilNo.ozellik.plaka + "%'";
            if (sicilNo.ozellik.bisNo.Trim() != "")
                sql += " AND (V.BISNO='" + sicilNo.ozellik.bisNo.Trim() + "' OR V.PRSICILNO=" + sicilNo.ozellik.bisNo + ")";
            if (sicilNo.ozellik.bulunduguYer.Trim() != "")
                sql += " AND (V.BULUNDUGUYER='" + sicilNo.ozellik.bulunduguYer + "')";
            if (sicilNo.sorguFiyat1 > 0)
                sql += " AND V.FIYAT >= " + sicilNo.sorguFiyat1;
            if (sicilNo.sorguFiyat2 > 0)
                sql += " AND V.FIYAT <= " + sicilNo.sorguFiyat2;
            if (!sicilNo.ozellik.sorguAlimTarihi1.isNull)
                sql += " AND " + VeriErisim.KosulOl("H", "FISTARIH", ">=", sicilNo.ozellik.sorguAlimTarihi1);
            if (!sicilNo.ozellik.sorguAlimTarihi2.isNull)
                sql += " AND " + VeriErisim.KosulOl("H", "FISTARIH", "<=", sicilNo.ozellik.sorguAlimTarihi2);
            if (sicilNo.amortismanYil > 0)
                sql += " AND V.AMORTIYIL = " + sicilNo.amortismanYil;
            if (sicilNo.accessNo == 1)
                sql += " AND " + dosyaVar + " > " + 0;//dosya varları sorgula
            if (sicilNo.accessNo == 2)
                sql += " AND " + dosyaVar + " = " + 0;//dosya yokları sorgula

            if (sicilNo.ozellik.amortismanBitti != -1)
            {
                if (TNS.TMM.Arac.MerkezBankasiKullaniyor())//Manas bundan dolayı malzemeleri eksik listeliyor Melih 09.04.2018
                    sql += " AND OZ.AMORTISMANBITTI = " + sicilNo.ozellik.amortismanBitti;
            }

            //PAGİNG İŞLEMİ SORGUYU BOZUYOR SON WHERE BAKTIĞI İÇİN
            //string kosul = "";
            //if (!sicilNo.ozellik.sorguAlimTarihi1.isNull)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", " (SELECT FISTARIH FROM TASSICILNOHAREKET IC WHERE TASDEMIRBASVIEW.PRSICILNO=IC.PRSICILNO AND IC.SIRANO=1)", ">=", sicilNo.ozellik.sorguAlimTarihi1));
            //if (!sicilNo.ozellik.sorguAlimTarihi2.isNull)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", " (SELECT FISTARIH FROM TASSICILNOHAREKET IC WHERE TASDEMIRBASVIEW.PRSICILNO=IC.PRSICILNO AND IC.SIRANO=1)", "<=", sicilNo.ozellik.sorguAlimTarihi2));
            //if (kosul != "")
            //    sql = VeriErisim.KosulEkle(sql, "AND", kosul);
            //PAGİNG İŞLEMİ SORGUYU BOZUYOR SON WHERE BAKTIĞI İÇİN


            if (sicilNo.hesapPlanKod.Replace(".", "").Trim() != "")
            {
                if (sicilNo.hesapPlanKod.StartsWith("@"))
                {
                    sicilNo.hesapPlanKod = sicilNo.hesapPlanKod.Replace("@", "");
                    string[] aranan = sicilNo.hesapPlanKod.Split(' ');
                    string ekKosul = "";
                    for (int i = 0; i < aranan.Length; i++)
                    {
                        if (aranan[i] == "") continue;
                        ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("V", "HESAPPLANKOD", "LIKE", aranan[i].Replace(".", "") + "%"));
                    }
                    if (ekKosul != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                }
                else
                    sql += " AND V.HESAPPLANKOD LIKE '" + sicilNo.hesapPlanKod.Replace(".", "").Trim() + "%'";
            }
            if (sicilNo.hesapPlanAd.Replace(".", "").Trim() != "")
                sql += " AND V.HESAPPLANAD LIKE '%" + sicilNo.hesapPlanAd.Replace(".", "").Trim() + "%'";

            if (sicilNo.sicilNoAciklama != "")
            {
                if (sicilNo.sicilNoAciklama.StartsWith("@"))
                {
                    sicilNo.sicilNoAciklama = sicilNo.sicilNoAciklama.Replace("@", "");
                    string[] aranan = sicilNo.sicilNoAciklama.Split(' ');
                    string ekKosul = "";
                    for (int i = 0; i < aranan.Length; i++)
                    {
                        if (aranan[i] == "") continue;
                        ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("V", ALANLAR[(int)ENUMAlanlar.SICILNO], "=", aranan[i].Replace(".", "")));
                    }
                    if (ekKosul != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                }
            }
            else
            {
                if (sicilNo.sicilNo.Replace(".", "").Trim() != "")
                {
                    if (sicilNo.sorguSicilNoIcindeGecen)
                        sql += " AND V.SICILNO LIKE '%" + sicilNo.sicilNo.Replace(".", "").Trim() + "%'";
                    else
                        sql += " AND V.SICILNO = '" + sicilNo.sicilNo.Replace(".", "").Trim() + "'";
                }
            }


            if (!string.IsNullOrWhiteSpace(sicilNo.sorguButunSicilNolardaAra))
            {
                if (sicilNo.sorguButunSicilNolardaAra.StartsWith("@"))
                {
                    sicilNo.sorguButunSicilNolardaAra = sicilNo.sorguButunSicilNolardaAra.Replace("@", "");
                    string[] aranan = sicilNo.sorguButunSicilNolardaAra.Split(' ');
                    string ekKosul = "";
                    for (int i = 0; i < aranan.Length; i++)
                    {
                        if (aranan[i] == "") continue;

                        if (ekKosul != "")
                            ekKosul += " OR ";

                        ekKosul += "(V.DISSICILNO " + aramaOp + "'" + aramaIs + aranan[i] + aramaIs + "'" + " OR V.DISSICILNO2" + aramaOp + "'" + aramaIs + aranan[i] + aramaIs + "' OR V.SICILNO = '" + aranan[i].Replace(".", "").Trim() + "')";
                    }
                    if (ekKosul != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                }
                else
                    sql += " AND (V.DISSICILNO " + aramaOp + "'" + aramaIs + sicilNo.sorguButunSicilNolardaAra + aramaIs + "'" + " OR V.DISSICILNO2" + aramaOp + "'" + aramaIs + sicilNo.sorguButunSicilNolardaAra + aramaIs + "' OR V.SICILNO = '" + sicilNo.sorguButunSicilNolardaAra.Replace(".", "").Trim() + "')";
            }

            if (sicilNo.eSicilNo != "")
                sql += " AND (V.DISSICILNO" + aramaOp + "'" + aramaIs + sicilNo.eSicilNo + aramaIs + "'" + " OR V.DISSICILNO2" + aramaOp + "'" + aramaIs + sicilNo.eSicilNo + aramaIs + "')";
            if (sicilNo.ozellik.disSicilNo.Trim() != "")
                sql += " AND (V.DISSICILNO" + aramaOp + "'" + aramaIs + sicilNo.ozellik.disSicilNo + aramaIs + "'" + " OR V.DISSICILNO2" + aramaOp + "'" + aramaIs + sicilNo.ozellik.disSicilNo + aramaIs + "')";

            if (sicilNo.ozellik.saseNo.Trim() != "")
                sql += " AND V.SASENO LIKE '%" + sicilNo.ozellik.saseNo + "%'";
            if (sicilNo.prSicilNo > 0)
                sql += " AND V.PRSICILNO =" + sicilNo.prSicilNo;
            if (sicilNo.epc != "")
                sql += " AND RFIDNO =" + sicilNo.epc;

            if (sicilNo.sorguHaricPrSicilNoListe != "")
                sql += " AND V.SICILNO NOT IN (" + sicilNo.sorguHaricPrSicilNoListe + ")";

            string fisNo = "";
            if (sicilNo.sorguFisNoTif.Trim() != "" && sicilNo.sorguFisNoZimmet.Trim() != "")
            {
                if (sicilNo.sorguFisNoTif.Trim() == sicilNo.sorguFisNoZimmet.Trim())
                    fisNo = sicilNo.sorguFisNoTif.Trim();
            }
            else if (sicilNo.sorguFisNoTif.Trim() != "")
                fisNo = sicilNo.sorguFisNoTif.Trim();
            else if (sicilNo.sorguFisNoZimmet.Trim() != "")
                fisNo = sicilNo.sorguFisNoZimmet.Trim();
            else if (sicilNo.fisNo.Trim() != "")
                fisNo = sicilNo.fisNo.Trim();

            if (fisNo != "")
            {
                sql += " AND V.FISNO = '" + fisNo + "'";

                if (sicilNo.sorguFisNoTif.Trim() != "")
                    sql += " AND V.KIMEGITTI = ''";
                else if (sicilNo.sorguFisNoZimmet.Trim() != "" && sicilNo.durum != 1)//Ambarda ise düşme fişinde KIMEGITTI boş oluyor //melih 03.07.2023
                    sql += " AND V.KIMEGITTI <> ''";

                if (sicilNo.yil > 0)
                    sql += " AND V.YIL =" + sicilNo.yil;
            }
            else
            {
                if (sicilNo.durum == 1)
                    sql += " AND (V.ISLEMTIPIKOD < 1000 OR V.ISLEMTIPIKOD=1002 OR V.ISLEMTIPIKOD=1004)";
                else if (sicilNo.durum == 2)
                    sql += " AND (V.ISLEMTIPIKOD=1001 OR V.ISLEMTIPIKOD=1003)";
            }

            if (!string.IsNullOrEmpty(sicilNo.ozellik.adi))
                sql += " AND V.YERIKONUSU LIKE '%" + sicilNo.ozellik.adi + "%'";

            if (sicilNo.kimeGitti != "")
                sql += " AND V.KIMEGITTI = " + "'" + sicilNo.kimeGitti + "'";
            if (sicilNo.nereyeGitti != "" && !sicilNo.altOdalar)
                sql += " AND V.NEREYEGITTI = " + "'" + sicilNo.nereyeGitti + "'";


            string siralama = "";
            if (sayfa.siralamaAlani != "")
            {
                if (sayfa.siralamaAlani == "DOSYAVAR")
                    siralama = " ORDER BY " + dosyaVar + " " + sayfa.siralamaYon;
                else if (sayfa.siralamaAlani == "ZIMMETODA")
                    siralama = " ORDER BY " + "V.ODAAD" + " " + sayfa.siralamaYon;
                else
                    siralama = " ORDER BY V." + sayfa.siralamaAlani + " " + sayfa.siralamaYon;
            }
            else if (!sicilNo.sorguEskiSicilNoAramaLike)
                siralama = " ORDER BY V.PRSICILNO";
            else
                siralama = " ORDER BY V.SICILNO";

            if (sicilNo.fisNo == "" && (sicilNo.sorguFisNoTif == "" || sicilNo.sorguFisNoZimmet == ""))
            {
                if (sicilNo.sorguFisNoTif != "")
                {
                    sql = sql.Replace("FROM TASDEMIRBASVIEW AS V", "FROM TASAMBARSICILVIEW AS V LEFT OUTER JOIN TASSICILNORFID ON V.PRSICILNO = TASSICILNORFID.PRSICILNO ");
                    //sql = sql.Replace("TASDEMIRBASVIEW", "TASAMBARSICILVIEW");
                    sql = sql.Replace("V.KISIAD,", "''KISIAD,");
                    sql = sql.Replace("V.ODAAD,", "''ODAAD,");
                    sql = sql.Replace("V.KIMEGITTI,", "''KIMEGITTI,");
                    sql = sql.Replace("V.NEREYEGITTI,", "''NEREYEGITTI,");
                }
                else if (sicilNo.sorguFisNoZimmet != "")
                {
                    if (sicilNo.durum != 1)
                        sql = sql.Replace("FROM TASDEMIRBASVIEW AS V", "FROM TASAMBARZIMMETVIEW AS V LEFT OUTER JOIN TASSICILNORFID ON V.PRSICILNO = TASSICILNORFID.PRSICILNO ");

                    //sql = sql.Replace("TASDEMIRBASVIEW", "TASAMBARZIMMETVIEW");
                }
            }


            sql = TABGenel.GetPaginatedSQL(sayfa.sayfaNo, sayfa.kayitSayisi, sql, siralama);

            sql = sql.Replace("XXOX", "FROM");

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Programatik Sicil No bilgisini döndüren,
        /// Sql cümlesini oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Programatik Sicil numarasını tutan Int türünde nesne döndürür</returns>
        public int ProgramSicilNoBul(SicilNoHareket sicilNo)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " FROM " + TABLOAD;
            sql += " INNER JOIN TASSICILNOOZELLIK ON TASSICILNOOZELLIK.PRSICILNO=TASSICILNOHAREKET.PRSICILNO";

            if (sicilNo.sicilNo != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SICILNO], "=", sicilNo.sicilNo));
            else
                sql += " WHERE (TASSICILNOOZELLIK.DISSICILNO LIKE '%" + sicilNo.eSicilNo + "%'" + " OR TASSICILNOOZELLIK.DISSICILNO2 LIKE '%" + sicilNo.eSicilNo + "%')";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMSicilDurum.AKTIF));

            if (sicilNo.muhasebeKod != "")//Kurum bazında sicil verilenleri sadece sicilnodan bulmak için boş gelebilir
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));

            if (sicilNo.harcamaBirimKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Sicil Hareket bilgilerini döndüren
        /// sql komutunu oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNo">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Sicil Hareket bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(SicilNoHareket sicilNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASISLEMTIP.AD, TASSICILNOOZELLIK.DISSICILNO, TASODA.AD AS NEREYEGITTIAD FROM " + TABLOAD + " LEFT OUTER JOIN TASODA ON TASODA.KOD = TASSICILNOHAREKET.NEREYEGITTI LEFT OUTER JOIN TASSICILNOOZELLIK ON TASSICILNOOZELLIK.PRSICILNO =TASSICILNOHAREKET.PRSICILNO ,TASISLEMTIP";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMSicilDurum.IPTAL));
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD] + "=TASISLEMTIP.KOD";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", sicilNo.prSicilNo));

            //if (sicilNo.yil > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNo.yil));

            if (sicilNo.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", sicilNo.siraNo));

            if (sicilNo.muhasebeKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));

            if (sicilNo.harcamaBirimKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));

            if (sicilNo.ambarKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sicilNo.ambarKod));

            if (sicilNo.sicilNo != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SICILNO], "=", sicilNo.sicilNo));

            if (sicilNo.hesapPlanKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "LIKE", sicilNo.hesapPlanKod.Trim().Replace(".", "") + "%"));

            if (sicilNo.fisNo != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNo.fisNo));

            if (sicilNo.eSicilNo != "")
                sql += " AND (TASSICILNOOZELLIK.DISSICILNO LIKE '%" + sicilNo.eSicilNo + "%'" + " OR TASSICILNOOZELLIK.DISSICILNO2 LIKE '%" + sicilNo.eSicilNo + "%')";
            if (sicilNo.ozellik.disSicilNo != "")
                sql += " AND (TASSICILNOOZELLIK.DISSICILNO LIKE '%" + sicilNo.ozellik.disSicilNo + "%'" + " OR TASSICILNOOZELLIK.DISSICILNO2 LIKE '%" + sicilNo.ozellik.disSicilNo + "%')";

            sql += " ORDER BY 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Sicil Hareket bilgilerini döndüren
        /// sql komutunu oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Sicil Hareket bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader KontrolSicilNo(SicilNoHareket bilgi)
        {
            string kosul = "";
            string sql = "SELECT PRSICILNO, MUHASEBEKOD, HARCAMABIRIMKOD, SICILNO FROM " + TABLOAD;

            if (bilgi.sicilNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SICILNO], "=", bilgi.sicilNo));

            if (bilgi.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", bilgi.prSicilNo));

            if (bilgi.siraNo < 0)//DEGER ARTIŞ SIRANI -1 DEN ANLIYORUZ
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));

            if (bilgi.muhasebeKod != string.Empty)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));

            if (bilgi.harcamaBirimKod != string.Empty)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", bilgi.harcamaBirimKod.Replace(".", "")));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Programatik Sicil numarası ve Sıra Numarası bilgilerini döndüren
        /// sql komutunu oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNoHareket">Sicil Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Programatik Sicil Numarası ve Sıra Numarası bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader HareketPrSicilSiraDondur(SicilNoHareket sicilNoHareket)
        {
            string sql = "SELECT PRSICILNO,SIRANO FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNoHareket.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNoHareket.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNoHareket.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNoHareket.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD], "=", sicilNoHareket.islemTipKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTURUKOD], "=", sicilNoHareket.islemTurKod));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil hareket bilgisin kıstas olarak kullanarak,
        /// kıstasa uygun verilen hareket sıra numarasından sonra işlem görüp görmediğini, gördüyse hareket listesini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sicilNoHareket">Sicil No Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunu çalışması sonucunda, verilen kriterlere uygun hareket bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SonrasindaHareketVarMiZimmet(SicilNoHareket sicilNoHareket)
        {
            string sql = "SELECT YIL, MUHASEBEKOD, HARCAMABIRIMKOD, FISNO, ISLEMTIPIKOD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", sicilNoHareket.prSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], ">", sicilNoHareket.siraNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BisSicilNoListele(SicilNoHareket sicilNo)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("TASSICILNORFIDVIEW", ALANLAR) + ", TASHESAPPLAN.ACIKLAMA ";
            sql += " FROM " + "TASSICILNORFIDVIEW";
            sql += " INNER JOIN TASSICILNOOZELLIK ON TASSICILNORFIDVIEW.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO ";
            sql += " INNER JOIN TASSICILNORFIDVIEW AS TASSICILNORFIDVIEW_1 ON TASSICILNOOZELLIK.BISNO = TASSICILNORFIDVIEW_1.PRSICILNO ";
            sql += " INNER JOIN TASHESAPPLAN  ON TASSICILNORFIDVIEW.HESAPPLANKOD = TASHESAPPLAN.KOD ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASSICILNORFIDVIEW" + "_1", ALANLAR[(int)ENUMAlanlar.SICILNO], "=", sicilNo.sicilNo));

            if (sicilNo.eSicilNo != "")
                sql += " AND TASSICILNOOZELLIK.DISSICILNO LIKE '%" + sicilNo.eSicilNo + "%'" + " OR TASSICILNOOZELLIK.DISSICILNO2 LIKE '%" + sicilNo.eSicilNo + "%'";

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.PRSICILNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DosyaSicilNoListele(SicilNoHareket bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", bilgi.prSicilNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IliskiliMalzemeListele(int[] prSicilNoListe, bool sadeceSatirSayisi)
        {
            string sql = "";

            sql = "SELECT " + (sadeceSatirSayisi ? " COUNT(*) " : VeriErisim.TumAlanlar("TASSICILNORFIDVIEW", ALANLAR) + ", TASHESAPPLAN.ACIKLAMA ");
            sql += " FROM " + "TASSICILNORFIDVIEW";
            sql += " INNER JOIN TASSICILNOOZELLIK ON TASSICILNORFIDVIEW.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO ";
            sql += " INNER JOIN TASSICILNORFIDVIEW AS TASSICILNORFIDVIEW_1 ON TASSICILNOOZELLIK.BISNO = TASSICILNORFIDVIEW_1.PRSICILNO ";
            sql += " INNER JOIN TASHESAPPLAN  ON TASSICILNORFIDVIEW.HESAPPLANKOD = TASHESAPPLAN.KOD ";

            string kosul = "";
            foreach (var prSicilNo in prSicilNoListe)
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("TASSICILNORFIDVIEW" + "_1", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            if (kosul != "")
                sql += " WHERE (" + kosul + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc GuncelleDegerArtis(SicilNoHareket bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGERARTISI], bilgi.degerArtisi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", bilgi.prSicilNo));

            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        public decimal SonBedelGetir(int prSicilNo)
        {
            string sql = "SELECT H.KDVLIBIRIMFIYAT + ISNULL((SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5),0) AS SONBEDEL FROM TASSICILNOHAREKET AS H ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", 1));

            return veriErisim.TekKolonSQLDecimal(sql);
        }

        public IDataReader FiyatGetir(int prSicilNo)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KDVLIBIRIMFIYAT] + "," + ALANLAR[(int)ENUMAlanlar.DEGERARTISI] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", 1));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader TasinirFiyatDuzelt(int i)
        {
            string sql = "SELECT 'update TASSICILNOHAREKET set FIYAT=' + CONVERT(VARCHAR, FIYAT) + ',KDVLIBIRIMFIYAT=' + CONVERT(VARCHAR, KDVLIBIRIMFIYAT) + ' WHERE SICILNO=''' + SICILNO + ''''  FROM dbo.TASSICILNOHAREKET A WHERE SIRANO = 1 AND (FIYAT<>(SELECT FIYAT FROM TASSICILNOHAREKET B WHERE A.PRSICILNO = B.PRSICILNO AND SIRANO = 1) or KDVLIBIRIMFIYAT<>(SELECT KDVLIBIRIMFIYAT FROM TASSICILNOHAREKET B WHERE A.PRSICILNO = B.PRSICILNO AND SIRANO = " + i + "))";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetSicilNoNumara(SicilNoHareket sicilNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc(false, "", "");

            sql = "INSERT INTO " + TABLOADNUMARA + " (" + VeriErisim.TumAlanlar("", ALANLARNUMARA) + ") VALUES (";
            sql += VeriErisim.DegerYap(sicilNo.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(sicilNo.yil) + ",";
            sql += VeriErisim.DegerYap(sicilNo.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(sicilNo.harcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sicilNo.ambarKod) + ",";
            sql += VeriErisim.DegerYap(sicilNo.fisNo) + ")";
            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        public Sonuc SilSicilNoHareket(SicilNoHareket sicilNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sicilNo.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sicilNo.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", sicilNo.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTIPIKOD], "<", "1001"));
            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        public IDataReader ListeleSicilNoNumara(SicilNoHareket sicilNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADNUMARA, ALANLARNUMARA) + " FROM " + TABLOADNUMARA;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADNUMARA, ALANLARNUMARA[(int)ENUMAlanlarNumara.YIL], "=", sicilNo.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADNUMARA, ALANLARNUMARA[(int)ENUMAlanlarNumara.MUHASEBEKOD], "=", sicilNo.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADNUMARA, ALANLARNUMARA[(int)ENUMAlanlarNumara.HARCAMABIRIMKOD], "=", sicilNo.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADNUMARA, ALANLARNUMARA[(int)ENUMAlanlarNumara.AMBARKOD], "=", sicilNo.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADNUMARA, ALANLARNUMARA[(int)ENUMAlanlarNumara.FISNO], "=", sicilNo.fisNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public void TasinirFiyatDuzelt(System.Collections.Generic.List<string> sqlListe)
        {
            foreach (string sql in sqlListe)
            {
                KomutCalistir(sql, 0);
            }
        }
    }
}
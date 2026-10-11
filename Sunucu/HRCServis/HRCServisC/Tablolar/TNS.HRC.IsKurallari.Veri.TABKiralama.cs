using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Kiralama bilgisini tutan HRCKIRALAMA veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKiralama : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKiralama(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Kiralama bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCKIRALAMA";

        /// <summary>
        /// Kiralama bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
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
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 3,
            /// <summary>
            /// MARKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MARKA = 4,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 5,
            /// <summary>
            /// MODEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MODEL = 6,
            /// <summary>
            /// MODELYILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MODELYILI = 7,
            /// <summary>
            /// CINSI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CINSI = 8,
            /// <summary>
            /// PLAKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PLAKA = 9,
            /// <summary>
            /// MENSEI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MENSEI = 10,
            /// <summary>
            /// SILINDIRHACMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SILINDIRHACMI = 11,
            /// <summary>
            /// MOTORGUCU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MOTORGUCU = 12,
            /// <summary>
            /// AGIRLIGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AGIRLIGI = 13,
            /// <summary>
            /// KOLTUKSAYISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOLTUKSAYISI = 14,
            /// <summary>
            /// KULLANISAMACI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANISAMACI = 15,
            /// <summary>
            /// TASINIRADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRADI = 16,
            /// <summary>
            /// TASINIRMARKASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRMARKASI = 17,
            /// <summary>
            /// TASINIRMODEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRMODEL = 18,
            /// <summary>
            /// TASINIRMODELYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRMODELYIL = 19,
            /// <summary>
            /// TASINIRSERISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINIRSERISI = 20,
            /// <summary>
            /// TASINMAZCINSI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINMAZCINSI = 21,
            /// <summary>
            /// TASINMAZADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINMAZADI = 22,
            /// <summary>
            /// TASINMAZADA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINMAZADA = 23,
            /// <summary>
            /// TASINMAZYUZOLCUMU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINMAZYUZOLCUMU = 24,
            /// <summary>
            /// TASINMAZHISSEORANI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINMAZHISSEORANI = 25,
            /// <summary>
            /// TASINMAZKATSAYISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINMAZKATSAYISI = 26,
            /// <summary>
            /// TASINMAZYAPIMYILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASINMAZYAPIMYILI = 27
        }

        /// <summary>
        /// Kiralama tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "YIL", "SURECNO", "SIRANO", "TUR", 
            "MARKA", "TIP", "MODEL", "MODELYILI", "CINSI", "PLAKA", "MENSEI", "SILINDIRHACMI", "MOTORGUCU", "AGIRLIGI", "KOLTUKSAYISI", "KULLANISAMACI", 
            "TASINIRADI" , "TASINIRMARKASI", "TASINIRMODEL", "TASINIRMODELYIL", "TASINIRSERISI", 
            "TASINMAZCINSI", "TASINMAZADI", "TASINMAZADA", "TASINMAZYUZOLCUMU", "TASINMAZHISSEORANI", "TASINMAZKATSAYISI", "TASINMAZYAPIMYILI"};

        /// <summary>
        /// Süreç bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSUREC = "HRCSUREC";

        /// <summary>
        /// Süreç bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarSUREC
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
            /// SURECDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECDURUM = 4,
        }

        /// <summary>
        /// Süreç tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSUREC = { "YIL", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "SURECDURUM" };

        /// <summary>
        /// Kiralama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kiralama bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Kiralama bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.marka) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.model) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.modelYili) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.cinsi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.plaka) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.mensei) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.silindirHacmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.motorGucu) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.agirligi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.koltukSayisi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Arac.kullanisAmaci) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinir.adi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinir.markasi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinir.model) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinir.modelYili) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinir.serisi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinmaz.cinsi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinmaz.adi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinmaz.paftaAdaParsel) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinmaz.yuzolcumu) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinmaz.hisseOrani) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinmaz.katSayisi) + ",";
            sql += VeriErisim.DegerYap(bilgi.Tasinmaz.yapimYili) + ")";


            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MARKA], bilgi.Arac.marka) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], bilgi.Arac.tip) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODEL], bilgi.Arac.model) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODELYILI], bilgi.Arac.modelYili) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CINSI], bilgi.Arac.cinsi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PLAKA], bilgi.Arac.plaka) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MENSEI], bilgi.Arac.mensei) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SILINDIRHACMI], bilgi.Arac.silindirHacmi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MOTORGUCU], bilgi.Arac.motorGucu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AGIRLIGI], bilgi.Arac.agirligi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOLTUKSAYISI], bilgi.Arac.koltukSayisi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANISAMACI], bilgi.Arac.kullanisAmaci) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINIRADI], bilgi.Tasinir.adi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINIRMARKASI], bilgi.Tasinir.markasi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINIRMODEL], bilgi.Tasinir.model) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINIRMODELYIL], bilgi.Tasinir.modelYili) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINIRSERISI], bilgi.Tasinir.serisi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZCINSI], bilgi.Tasinmaz.cinsi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZADI], bilgi.Tasinmaz.adi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZADA], bilgi.Tasinmaz.paftaAdaParsel) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYUZOLCUMU], bilgi.Tasinmaz.yuzolcumu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZHISSEORANI], bilgi.Tasinmaz.hisseOrani) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKATSAYISI], bilgi.Tasinmaz.katSayisi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYAPIMYILI], bilgi.Tasinmaz.yapimYili);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", (bilgi.siraNo)));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Kiralama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak kiralama bilgisi kıstas parametresi</param>
        /// <returns>
        /// Kiralama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(Kiralama bilgi)
        {
            string sql = "";
            string kosul = "";
            string kriter = Convert.ToString(bilgi.tag);

            if (!string.IsNullOrEmpty(kriter))
            {
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", " + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSUREC.MUHASEBEKOD] + ", " + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSUREC.BIRIMKOD] + " FROM " + TABLOAD;
                sql += " INNER JOIN " + TABLOADSUREC + " ON " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSUREC.YIL] + " AND " +
                        TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSUREC.SURECNO];

                string[] kiralamaTurler = kriter.Split(';');
                string kiralamTurKosul = "";

                for (int i = 0; i < kiralamaTurler.Length; i++)
                {
                    kiralamTurKosul = VeriErisim.KosulEkle(kiralamTurKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", kiralamaTurler[i]));
                }

                if (bilgi.yil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                if (bilgi.muhasebe > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSUREC.MUHASEBEKOD], "=", bilgi.muhasebe));
                if (bilgi.birim != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSUREC.BIRIMKOD], "=", bilgi.birim));

                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSUREC.SURECDURUM], "<", 9));

                sql += " WHERE " + kosul;

                if (kiralamTurKosul != "")
                    sql += " AND (" + kiralamTurKosul + ")";

            }
            else
            {
                sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

                if (bilgi.yil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                if (bilgi.surecNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
                if (bilgi.siraNo > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));

                if (kosul != "")
                    sql += " WHERE " + kosul;

                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SURECNO];
            }

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
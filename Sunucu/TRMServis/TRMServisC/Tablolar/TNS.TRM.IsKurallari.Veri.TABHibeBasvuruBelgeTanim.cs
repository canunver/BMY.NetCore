using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeBasvuruBelgeTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeBasvuruBelgeTanim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBEBASVURUBELGETANIMLAMA";

        /// <summary>
        /// Malzeme grubu özellik bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBELGELER = "TRMHIBEBASVURUBELGEADLARI";

        /// <summary>
        /// Başvuru tanım bilgisini tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 1,
            /// <summary>
            /// MALZEMEGRUBU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMEGRUBU = 2,
            /// <summary>
            /// HIBETURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBETURU = 3,
            /// <summary>
            /// ETAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ETAPNO = 4,
        }

        /// <summary>
        /// Basvuru belge isim bilgilerini tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBelgeler
        {
            /// <summary>
            /// BELGEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEKOD = 0,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// BELGEADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEADI = 2,
            BELGEKISAADI = 3,
            GERCEKTUZEL = 4,
            YATIRIMNITELIK = 5,
            KOSUL = 6
        }

        /// <summary>
        /// Basvuru belge tanımlama tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "YIL", "MALZEMEGRUBU", "HIBETURU", "ETAPNO" };

        /// <summary>
        /// Başvuru belge isimlerinin tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARBELGELER = { "BELGEKOD", "SIRANO", "BELGEADI", "BELGEKISAADI", "GERCEKTUZEL", "YATIRIMNITELIK", "KOSUL" };

        /// <summary>                                                                
        /// Başvuru belge tanımlama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(BasvuruBelgeTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], bilgi.yil) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBU], bilgi.malzemeGrubu.kod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIBETURU], bilgi.hibeTuru) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], bilgi.etapNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.malzemeGrubu.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.hibeTuru) + ",";
                sql += VeriErisim.DegerYap(bilgi.etapNo) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Başvuru belge adları bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetBelgeler(BasvuruBelgeAdlari bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADBELGELER + " (" + VeriErisim.TumAlanlar("", ALANLARBELGELER) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.belgeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.belgeAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.belgeKisaAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.gercekTuzel) + ",";
            sql += VeriErisim.DegerYap(bilgi.nitelik) + ",";
            sql += VeriErisim.DegerYap(bilgi.kosul) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Başvuru belge tanım bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(BasvuruBelgeTanim bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            if (bilgi.kod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBETURU], "=", bilgi.hibeTuru));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            }

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilBelgeler(BasvuruBelgeTanim bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADBELGELER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGEKOD], "=", bilgi.kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader HibeTuruYilMalzemeGrubunaGoreBelgeTanimKaydiVarmi(BasvuruBelgeTanim bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;


            if (!string.IsNullOrEmpty(bilgi.hibeTuru))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBETURU], "=", bilgi.hibeTuru));

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));

            if (bilgi.malzemeGrubu != null && bilgi.malzemeGrubu.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBU], "=", bilgi.malzemeGrubu.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Başvuru belge tanim bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Malzeme grubu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(BasvuruBelgeTanim bilgi)
        {
            string sql = "";
            string kosul = "";
            string ad = "";
            string where = "";

            if (bilgi.hibeTuru == "Ekonomik Yatırım")
            {
                ad = "TRMYATIRIMNITELIK.ADI";
                where = " LEFT OUTER JOIN TRMYATIRIMNITELIK ON TRMYATIRIMNITELIK.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBU];
            }
            else
            {
                ad = "TRMMALZEMEGRUBU.AD ";
                where = " LEFT OUTER JOIN TRMMALZEMEGRUBU ON TRMMALZEMEGRUBU.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBU];
            }

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " , " + ad + " FROM " + TABLOAD;
            sql += where;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.hibeTuru))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBETURU], "=", bilgi.hibeTuru));

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));

            if (bilgi.malzemeGrubu != null && bilgi.malzemeGrubu.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBU], "=", bilgi.malzemeGrubu.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBU];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Başvuru belge tanim bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Malzeme grubu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleBelgeler(BasvuruBelgeTanim bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADBELGELER, ALANLARBELGELER) + " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOADBELGELER + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADBELGELER + "." + ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGEKOD];

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.hibeTuru))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBETURU], "=", bilgi.hibeTuru));

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));

            if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));

            if (bilgi.malzemeGrubu != null && bilgi.malzemeGrubu.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBU], "=", bilgi.malzemeGrubu.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBU];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BasvuruBelgeAdlariniListele(int belgeKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBELGELER, ALANLARBELGELER) + " FROM " + TABLOADBELGELER;

            if (belgeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBELGELER, ALANLARBELGELER[(int)ENUMAlanlarBelgeler.BELGEKOD], "=", belgeKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADBELGELER + "." + ALANLARBELGELER[(int)ENUMAlanlarBelgeler.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeDegerlendirmeKomisyonu : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeDegerlendirmeKomisyonu(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Komisyon bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBEDEGERLENDIRMEKOMISYONU";


        /// <summary>
        /// Komisyon Detay bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDetay = "TRMHIBEDEGERLENDIRMEKOMISYONUDETAY";

        /// <summary>
        /// Komisyon bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// ETAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ETAPNO = 1,
            /// <summary>
            /// HIBETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBETUR = 2,
            /// <summary>
            /// IL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IL = 3,
            /// <summary>
            /// KILIT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KILIT = 4,
            KONUTUR = 5,
            ALTYAPIGRUP = 6
        }

        /// <summary>
        /// Komisyon detay bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDetay
        {
            /// <summary>
            /// KKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KKOD = 0,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 1,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 2,
            /// <summary>
            /// EMAIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EMAIL = 3,
            /// <summary>
            /// TELEFON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TELEFON = 4,
            /// <summary>
            /// BASKAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BASKAN = 5,
            /// <summary>
            /// DEGERLENDIRMECI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGERLENDIRMECI = 6,
            /// <summary>
            /// DOGUMTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOGUMTARIH = 7
        }

        /// <summary>
        /// Komisyon tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "ETAPNO", "HIBETUR", "IL", "KILIT", "KONUTUR", "ALTYAPIGRUP" };

        /// <summary>
        /// Komisyon tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARDetay = { "KKOD", "TCKIMLIKNO", "ADSOYAD", "EMAIL", "TELEFON", "BASKAN", "DEGERLENDIRMECI", "DOGUMTARIH" };

        /// <summary>
        /// Komisyon bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetIlDegerlendirmeKomisyonu(IlDegerlendirmeKomisyonu bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADDetay + " (" + VeriErisim.TumAlanlar("", ALANLARDetay) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tcNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.adSoyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.eMail) + ",";
            sql += VeriErisim.DegerYap(bilgi.telefon) + ",";
            sql += VeriErisim.DegerYap(bilgi.baskan) + ",";
            sql += VeriErisim.DegerYap(bilgi.degerlendirmeci) + ",";
            sql += VeriErisim.DegerYap(bilgi.dogumTarih) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Komisyon bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetIlDegerlendirmeKomisyonuAna(IlDegerlendirmeKomisyonu bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeTuru) + ",";
            sql += VeriErisim.DegerYap(bilgi.il) + ",";
            sql += VeriErisim.DegerYap(bilgi.kilit) + ",";
            sql += VeriErisim.DegerYap(bilgi.konuTur) + ",";
            sql += VeriErisim.DegerYap(bilgi.altYapiGrup) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc SilIlDegerlendirmeKomisyonu(IlDegerlendirmeKomisyonu bilgi)
        {
            string tc = bilgi.tcNo.Replace("*******", "%");
            string sql = "DELETE FROM " + TABLOADDetay;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDetay, ALANLARDetay[(int)ENUMAlanlarDetay.KKOD], "=", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDetay, ALANLARDetay[(int)ENUMAlanlarDetay.TCKIMLIKNO], "LIKE", tc));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KilitleIlDegerlendirmeKomisyonu(int kod, int durum)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KILIT], durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }


        /// <summary>
        /// Yonetim Kurulu bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Yonetim Kurulu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleIlDegerlendirmeKomisyonu(IlDegerlendirmeKomisyonu bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", " + VeriErisim.TumAlanlar(TABLOADDetay, ALANLARDetay) + ", ORGCOGRAFYA.AD FROM " + TABLOAD;
            sql += " INNER JOIN " + TABLOADDetay + " ON " + TABLOADDetay + "." + ALANLARDetay[(int)ENUMAlanlarDetay.KKOD] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];
            sql += " LEFT OUTER JOIN ORGCOGRAFYA ON ORGCOGRAFYA.KOD = " + ALANLAR[(int)ENUMAlanlar.IL];

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (!string.IsNullOrEmpty(bilgi.il))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IL], "=", bilgi.il));
            if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            if (bilgi.hibeTuru > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBETUR], "=", bilgi.hibeTuru));
            if (bilgi.kilit > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KILIT], "=", bilgi.kilit));
            if (bilgi.konuTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONUTUR], "=", bilgi.konuTur));
            if (bilgi.altYapiGrup != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], "=", bilgi.altYapiGrup));
            if (bilgi.tcNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDetay, ALANLARDetay[(int)ENUMAlanlarDetay.TCKIMLIKNO], "=", bilgi.tcNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " , " + TABLOADDetay + "." + ALANLARDetay[(int)ENUMAlanlarDetay.BASKAN] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleIlDegerlendirmeKomisyonuAna(IlDegerlendirmeKomisyonu bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            else
            {
                if (!string.IsNullOrEmpty(bilgi.il))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IL], "=", bilgi.il));
                if (bilgi.etapNo > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
                if (bilgi.hibeTuru > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBETUR], "=", bilgi.hibeTuru));
                if (bilgi.kilit > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KILIT], "=", bilgi.kilit));
                if (bilgi.konuTur > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONUTUR], "=", bilgi.konuTur));
                if (bilgi.altYapiGrup != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTYAPIGRUP], "=", bilgi.altYapiGrup));
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

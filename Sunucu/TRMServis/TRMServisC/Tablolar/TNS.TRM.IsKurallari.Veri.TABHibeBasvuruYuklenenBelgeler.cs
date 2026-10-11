using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeBasvuruYuklenenBelgeler : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeBasvuruYuklenenBelgeler(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBEBASVURUYUKLENENBELGELER";

        /// <summary>
        /// Başvuru belgeleri bilgisini tablo alanlarının sırasını tutan enumeration
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
            /// ETAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ETAPNO = 2,
            /// <summary>
            /// BASVURUNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BASVURUNO = 3,
            /// <summary>
            /// BELGEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGEKOD = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// EVREVRAKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVREVRAKNO = 6,
            /// <summary>
            /// YUKLEYEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YUKLEYEN = 7,
            /// <summary>
            /// DOSYAADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOSYAADI = 8,
            /// <summary>
            /// HIBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIBEKOD = 9,
            HIBETUR = 10,
            SABLONADI = 11
        }

        /// <summary>
        /// Basvuru belge tanımlama tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "YIL", "ETAPNO", "BASVURUNO", "BELGEKOD", "SIRANO", "EVREVRAKNO", "YUKLEYEN", "DOSYAADI", "HIBEKOD", "HIBETUR", "SABLONADI" };

        /// <summary>
        /// Başvuru belge tanımlama bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(BasvurudaYuklenenBelgeler bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            veriErisim.hata = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.basvuruNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.belgeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.yukleyen) + ",";
            sql += VeriErisim.DegerYap(bilgi.dosyaAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.hibeTur) + ",";
            sql += VeriErisim.DegerYap(bilgi.sablonAdi) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Başvuru belge tanım bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(BasvurudaYuklenenBelgeler bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
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
        public IDataReader Listele(BasvurudaYuklenenBelgeler bilgi, bool eksikBelgeler)
        {
            string sql = "";
            string kosul = "";

            string belgeAd;
            string baglama;
            if (eksikBelgeler)
            {
                belgeAd = "TRMHIBEYATIRIMEKSIKBELGE.BELGEADI";
                baglama = "TRMHIBEYATIRIMEKSIKBELGE ON TRMHIBEBASVURUYUKLENENBELGELER.BELGEKOD = TRMHIBEYATIRIMEKSIKBELGE.KOD";
            }
            else
            {
                belgeAd = "TRMHIBEBASVURUBELGEADLARI.BELGEKISAADI";
                baglama = "TRMHIBEBASVURUBELGEADLARI ON TRMHIBEBASVURUYUKLENENBELGELER.BELGEKOD = TRMHIBEBASVURUBELGEADLARI.BELGEKOD AND TRMHIBEBASVURUYUKLENENBELGELER.SIRANO = TRMHIBEBASVURUBELGEADLARI.SIRANO";
            }

            sql = $"SELECT {VeriErisim.TumAlanlar(TABLOAD, ALANLAR)}, {belgeAd} FROM {TABLOAD} LEFT OUTER JOIN {baglama}";
            if (!string.IsNullOrEmpty(bilgi.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.belgeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGEKOD], "=", bilgi.belgeKod));

            if (bilgi.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));

            if (bilgi.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", bilgi.hibeKod));

            if (bilgi.hibeTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBETUR], "=", bilgi.hibeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;
            //OrtakFonksiyonlar.HataDosyaYaz("sqller.txt", sql, false);
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
        public IDataReader ListeleBasvurununYuklenenBelgeleri(BasvurudaYuklenenBelgeler bilgi)
        {
            string sql = @"select * from TRMHIBEBASVURUBELGETANIMLAMA
                        left outer join TRMHIBEBASVURUBELGEADLARI
                        on TRMHIBEBASVURUBELGETANIMLAMA.KOD = TRMHIBEBASVURUBELGEADLARI.BELGEKOD
                        left outer join TRMHIBEBASVURUYUKLENENBELGELER
                        on TRMHIBEBASVURUBELGEADLARI.BELGEKOD = TRMHIBEBASVURUYUKLENENBELGELER.BELGEKOD
                        and TRMHIBEBASVURUBELGEADLARI.SIRANO = TRMHIBEBASVURUYUKLENENBELGELER.SIRANO";

            string kosul = "";

            if (bilgi.hibeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", bilgi.hibeKod));

            if (bilgi.belgeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGEKOD], "=", bilgi.belgeKod));

            if (bilgi.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));

            if (bilgi.hibeTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBETUR], "=", bilgi.hibeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
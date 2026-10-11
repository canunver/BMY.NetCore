using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Aile Yardım Bildirim bilgisini tutan PSLAILEYARDIMBILDIRIMIBASLIK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABAileYardimBildirimi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAileYardimBildirimi(VeriErisim veriErisim)
            : base(veriErisim)
        {
            if (veriErisim.baglantiTip == "ORACLE")
            {
                TABLOADBASLIK = "PSLAILEYARBILBASLIK";
                TABLOADSATIR = "PSLAILEYARBILSATIR";
            }
        }

        /// <summary>
        /// Aile Yardım Bildirim Başlık tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBASLIK = "PSLAILEYARDIMBILDIRIMIBASLIK";

        /// <summary>
        /// Aile Yardım Bildirim Başlık tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBaslik
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
            /// <summary>
            /// PERSADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERSADSOYAD = 5,
            /// <summary>
            /// UNVANKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVANKODU = 6,
            /// <summary>
            /// MEDENIHALI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MEDENIHALI = 7,
            /// <summary>
            /// ILGILIAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIAY = 8,
            /// <summary>
            /// ILGILIYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIYIL = 9,
            /// <summary>
            /// ESADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ESADSOYAD = 10,
            /// <summary>
            /// EVLENMETARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVLENMETARIHI = 11,
            /// <summary>
            /// AILECUZDANISERINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AILECUZDANISERINO = 12,
            /// <summary>
            /// ESISDURUMU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ESISDURUMU = 13,
            /// <summary>
            /// ESACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ESACIKLAMA = 14,
        }

        /// <summary>
        /// Aile Yardım Bildirim Başlık tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARBASLIK = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TCKIMLIKNO", "PERSADSOYAD", "UNVANKODU", "MEDENIHALI", "ILGILIAY", "ILGILIYIL", "ESADSOYAD", "EVLENMETARIHI", "AILECUZDANISERINO", "ESISDURUMU", "ESACIKLAMA" };

        /// <summary>
        /// Aile Yardım Bildirim Satır tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSATIR = "PSLAILEYARDIMBILDIRIMISATIR";

        /// <summary>
        /// Aile Yardım Bildirim Satır tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSatir
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// PERTCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PERTCKIMLIKNO = 4,
            /// <summary>
            /// COCUKTCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            COCUKTCKIMLIKNO = 5,
            /// <summary>
            /// COCUKADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            COCUKADSOYAD = 6,
            /// <summary>
            /// DOGUMTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOGUMTARIHI = 7,
            /// <summary>
            /// CINSIYET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CINSIYET = 8,
            /// <summary>
            /// BABAAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BABAAD = 9,
            /// <summary>
            /// ANNEAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ANNEAD = 10,
            /// <summary>
            /// COCUKYAKINLIGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            COCUKYAKINLIGI = 11,
            /// <summary>
            /// YUKSOGRKAYITTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YUKSOGRKAYITTARIHI = 12,
            /// <summary>
            /// YUKSOGROKULADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YUKSOGROKULADI = 13,
            /// <summary>
            /// SINIF alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SINIF = 14,
            /// <summary>
            /// BURSDURUMU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BURSDURUMU = 15,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 16,
        }

        /// <summary>
        /// Aile Yardım Bildirim Satır tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSATIR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PERTCKIMLIKNO", "COCUKTCKIMLIKNO", "COCUKADSOYAD", "DOGUMTARIHI", "CINSIYET", "BABAAD", "ANNEAD", "COCUKYAKINLIGI", "YUKSOGRKAYITTARIHI", "YUKSOGROKULADI", "SINIF", "BURSDURUMU", "ACIKLAMA" };


        /// <summary>
        /// Aile Yardım Bildirim Başlık bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek Aile Yardım Bildirim Başlık bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(PSLAileYardimBildirimiBaslik bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADBASLIK + " (" + VeriErisim.TumAlanlar("", ALANLARBASLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.TCKimlikNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.perAdSoyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.unvanKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.medeniHali) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilgiliAy) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilgiliYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.esAdSoyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.evlenmeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.aileCuzdaniSeriNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.esIsDurumu) + ",";
            sql += VeriErisim.DegerYap(bilgi.esAciklama) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOADBASLIK + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.TCKIMLIKNO], bilgi.TCKimlikNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.ESISDURUMU], bilgi.esIsDurumu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.ESACIKLAMA], bilgi.esAciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.ILGILIAY], bilgi.ilgiliAy) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.ILGILIYIL], bilgi.ilgiliYil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.PERSADSOYAD], bilgi.perAdSoyad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.UNVANKODU], bilgi.unvanKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.MEDENIHALI], bilgi.medeniHali) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.ESADSOYAD], bilgi.esAdSoyad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.EVLENMETARIHI], bilgi.evlenmeTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.AILECUZDANISERINO], bilgi.aileCuzdaniSeriNo);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", bilgi.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.MUHASEBEKOD], "=", bilgi.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.BIRIMKOD], "=", bilgi.birim));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.SURECNO], "=", bilgi.surecNo));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Aile Yardım Bildirim Satır bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek Aile Yardım Bildirim Satır bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSatir(PSLAileYardimBildirimiBaslik bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (PSLAileYardimBildirimiSatir f in bilgi.cocuklar.objeler)
            {

                sql = "INSERT INTO " + TABLOADSATIR + " (" + VeriErisim.TumAlanlar("", ALANLARSATIR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.TCKimlikNo) + ",";
                sql += VeriErisim.DegerYap(f.cocukTCKimlikNo) + ",";
                sql += VeriErisim.DegerYap(f.cocukAdSoyad) + ",";
                sql += VeriErisim.DegerYap(f.dogumTarihi) + ",";
                sql += VeriErisim.DegerYap(f.cinsiyet) + ",";
                sql += VeriErisim.DegerYap(f.babaAd) + ",";
                sql += VeriErisim.DegerYap(f.anaAd) + ",";
                sql += VeriErisim.DegerYap(f.cocukYakinligi) + ",";
                sql += VeriErisim.DegerYap(f.yuksOgrKayitTarihi) + ",";
                sql += VeriErisim.DegerYap(f.yuksOgrOkulAdi) + ",";
                sql += VeriErisim.DegerYap(f.sinif) + ",";
                sql += VeriErisim.DegerYap(f.bursDurumu) + ",";
                sql += VeriErisim.DegerYap(f.aciklama) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        /// <summary>
        /// Aile Yardım Bildirim Başlık bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek Aile Yardım Bildirim Başlık bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Aile Yardım Bildirim Başlık bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleBaslik(PSLAileYardimBildirimiBaslik bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADBASLIK, ALANLARBASLIK) + "," + DBDegiskeni.TABLOADUNVAN + ".AD " + "," + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEAD " + "," + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMAD " + "FROM " + TABLOADBASLIK + " LEFT OUTER JOIN " + DBDegiskeni.TABLOADUNVAN + " ON " + TABLOADBASLIK + "." + ALANLARBASLIK[(int)ENUMAlanlarBaslik.UNVANKODU] + " = " + DBDegiskeni.TABLOADUNVAN + ".KOD " +
                " LEFT OUTER JOIN " + DBDegiskeni.TABLOADMUHASEBE + " ON " + TABLOADBASLIK + "." + ALANLARBASLIK[(int)ENUMAlanlarBaslik.MUHASEBEKOD] + " = " + DBDegiskeni.TABLOADMUHASEBE + ".MUHASEBEKOD " +
                " LEFT OUTER JOIN " + DBDegiskeni.TABLOADMUHASEBEBIRIM + " ON " + TABLOADBASLIK + "." + ALANLARBASLIK[(int)ENUMAlanlarBaslik.BIRIMKOD] + " = " + DBDegiskeni.TABLOADMUHASEBEBIRIM + ".BIRIMKOD";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.SURECNO], "=", bilgi.surecNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(DBDegiskeni.TABLOADMUHASEBEBIRIM, ALANLARBASLIK[(int)ENUMAlanlarBaslik.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.TCKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.TCKIMLIKNO], "=", bilgi.TCKimlikNo));


            if (kosul != "")
                sql += " WHERE " + kosul;


            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Aile Yardım Bildirim Satır bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek Aile Yardım Bildirim Başlık bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Aile Yardım Bildirim Satır bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleSatir(PSLAileYardimBildirimiBaslik bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSATIR, ALANLARSATIR) + " FROM " + TABLOADSATIR;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSATIR, ALANLARSATIR[(int)ENUMAlanlarSatir.YIL], "=", bilgi.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSATIR, ALANLARSATIR[(int)ENUMAlanlarSatir.MUHASEBEKOD], "=", bilgi.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSATIR, ALANLARSATIR[(int)ENUMAlanlarSatir.BIRIMKOD], "=", bilgi.birim));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSATIR, ALANLARSATIR[(int)ENUMAlanlarSatir.SURECNO], "=", bilgi.surecNo));
            if (bilgi.TCKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSATIR, ALANLARSATIR[(int)ENUMAlanlarSatir.PERTCKIMLIKNO], "=", bilgi.TCKimlikNo));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADSATIR + "." + ALANLARSATIR[(int)ENUMAlanlarSatir.DOGUMTARIHI];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Aile Yardım Bildirim Başlık bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek Aile Yardım Bildirim Başlık bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilSatir(PSLAileYardimBildirimiBaslik bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADSATIR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSATIR[(int)ENUMAlanlarSatir.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSATIR[(int)ENUMAlanlarSatir.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSATIR[(int)ENUMAlanlarSatir.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSATIR[(int)ENUMAlanlarSatir.SURECNO], "=", bilgi.surecNo));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSATIR[(int)ENUMAlanlarSatir.PERTCKIMLIKNO], "=", bilgi.TCKimlikNo));

            return KomutCalistir(sql, 0);
        }
    }
}
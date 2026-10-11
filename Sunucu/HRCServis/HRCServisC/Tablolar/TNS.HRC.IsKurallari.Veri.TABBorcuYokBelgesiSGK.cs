using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Borcu Yok Belge bilgisini tutan HRCBORCUYOKTUR veri tabanı ve 
    /// belge sorgulayan kullanıcı log bilgsinini tutan HRCGIBLOG veri tabanı 
    /// tabloları ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBorcuYokBelgesiSGK : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBorcuYokBelgesiSGK(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Borcu Yok Belgesi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCBORCUYOKTURSGK";

        /// <summary>
        /// Borcu Yok Belgesi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "REFNO", "VERGINO", "SORGUTIP", "DURUM", "TARIH", "KAYITTARIH", "ISLEMYAPAN" };

        /// <summary>
        /// Borcu Yok Belgesi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            MUHASEBEKOD = 1,
            REFNO = 2,
            VERGINO = 3,
            SORGUTIP = 4,
            DURUM = 5,
            TARIH = 6,
            KAYITTARIH = 7,
            ISLEMYAPAN = 8,
        }

        /// <summary>
        /// Borcu Yok Belgesi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSONUC = "HRCBORCUYOKTURSGKSONUC";

        /// <summary>
        /// Borcu Yok Belgesi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSONUC = { "KOD", "REFNO", "DAMGABORC", "EKPBORC", "IPCBORC", "ISSIZLIKBORC", "OIVBORC", "PRIMBORC" };

        /// <summary>
        /// Borcu Yok Belgesi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSonuc
        {
            KOD = 0,
            REFNO = 1,
            DAMGABORC = 2,
            EKPBORC = 3,
            IPCBORC = 4,
            ISSIZLIKBORC = 5,
            OIVBORC = 6,
            PRIMBORC = 7,
        }

        /// <summary>
        /// Borcu Yok Belgesi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADLOG = "HRCSGKLOG";

        /// <summary>
        /// Borcu Yok Belgesi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARLOG = { "SORGULANAN", "ISLEMTARIH", "ISLEMYAPAN", "SORGUTIP", "REFNO", "HATAKOD", "HATAMESAJ", "ISLEMYAPANKURUM" };

        /// <summary>
        /// Borcu Yok Belge Log tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarLOG
        {
            /// <summary>
            /// SORGULANAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SORGULANAN = 0,
            /// <summary>
            /// ISLEMTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 1,
            /// <summary>
            /// ISLEMYAPAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 2,
            /// <summary>
            /// SORGUTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SORGUTIP = 3,
            /// <summary>
            /// REFNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            REFNO = 4,
            /// <summary>
            /// HATAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HATAKOD = 5,
            /// <summary>
            /// HATAMESAJ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HATAMESAJ = 6,
            /// <summary>
            /// ISLEMYAPANKURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANKURUM = 7,
        }

        /// <summary>
        /// Borcu Yok Belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="belge">Listelenecek Borcu Yok Belge bilgisi için kıstas parametresi</param>
        /// <param name="yakinTarih">Eğer <c>true</c> verilirse tarihe göre tersten sıralı olarak listeleme yapar</param>
        /// <returns>
        /// Borcu Yok Belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(BorcuYokBelgesiSGK belge)
        {
            string kosul = string.Empty;
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            if (belge.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", belge.kod));
            else
            {
                if (belge.muhasebe > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", belge.muhasebe));
                if (!string.IsNullOrEmpty(belge.refNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.REFNO], "=", belge.refNo));
                if (!string.IsNullOrEmpty(belge.vergiNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VERGINO], "=", belge.vergiNo));
                if (belge.sorguTip > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORGUTIP], "=", belge.sorguTip));
                if (belge.durum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORGUTIP], "=", belge.durum));
                if (belge.tarih != null)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TARIH], ">=", belge.tarih));
                if (belge.kayitTarih != null)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITTARIH], "<=", belge.kayitTarih));
                if (!string.IsNullOrEmpty(belge.islemYapan))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", belge.islemYapan));
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;


            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Borcu Yok Belge bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="belge">Eklenecek Borcu Yok Belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(BorcuYokBelgesiSGK belge)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(belge.kod) + ",";
            sql += VeriErisim.DegerYap(belge.muhasebe) + ",";
            sql += VeriErisim.DegerYap(belge.refNo) + ",";
            sql += VeriErisim.DegerYap(belge.vergiNo) + ",";
            sql += VeriErisim.DegerYap(belge.sorguTip) + ",";
            sql += VeriErisim.DegerYap(belge.durum) + ",";
            sql += VeriErisim.DegerYap(belge.tarih) + ",";
            sql += VeriErisim.DegerYap(belge.kayitTarih) + ",";
            sql += VeriErisim.DegerYap(belge.islemYapan) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Borcu Yok Belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="belge">Listelenecek Borcu Yok Belge bilgisi için kıstas parametresi</param>
        /// <param name="yakinTarih">Eğer <c>true</c> verilirse tarihe göre tersten sıralı olarak listeleme yapar</param>
        /// <returns>
        /// Borcu Yok Belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleSonuc(BorcuYokBelgesiSGKSonuc belge)
        {
            string kosul = string.Empty;
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            if (belge.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", belge.kod));
            if (!string.IsNullOrEmpty(belge.refNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.REFNO], "=", belge.refNo));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Borcu Yok Belge Sonuç bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="belge">Eklenecek Borcu Yok Belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSonuc(BorcuYokBelgesiSGKSonuc belge)
        {
            string sql = "INSERT INTO " + TABLOADSONUC + " (" + VeriErisim.TumAlanlar("", ALANLARSONUC) + ") VALUES (";
            sql += VeriErisim.DegerYap(belge.kod) + ",";
            sql += VeriErisim.DegerYap(belge.refNo) + ",";
            sql += VeriErisim.DegerYap(belge.damgaBorc) + ",";
            sql += VeriErisim.DegerYap(belge.ekpBorc) + ",";
            sql += VeriErisim.DegerYap(belge.ipcBorc) + ",";
            sql += VeriErisim.DegerYap(belge.issizlikBorc) + ",";
            sql += VeriErisim.DegerYap(belge.oivBorc) + ",";
            sql += VeriErisim.DegerYap(belge.primBorc) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Borcu Yok Belge bilgisini sorgusuna ait log bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sorgulanan">Sorgulama yapılan kişi/kurum TC Kimlik/Vergi numarası bilgisi</param>
        /// <param name="islemYapan">Sorgulama yapan kullanıcı kodu bilgisi</param>
        /// <param name="sorguTipi">The sorgu tipi.</param>
        /// <param name="refNo">The ref no.</param>
        /// <param name="hataKodu">The hata kodu.</param>
        /// <param name="hataMesaji">The hata mesaji.</param>
        /// <param name="kurum">Sorgulamayı yapan kurum bilgisi</param>
        /// <returns>
        /// Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc SGKLogKaydet(string sorgulanan, string islemYapan, int sorguTipi, string refNo, int hataKodu, string hataMesaji, string kurum)
        {
            string sql = "INSERT INTO " + TABLOADLOG + " (" + VeriErisim.TumAlanlar("", ALANLARLOG) + ") VALUES (";
            sql += VeriErisim.DegerYap(sorgulanan) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now.ToString())) + ",";
            sql += VeriErisim.DegerYap(islemYapan) + ",";
            sql += VeriErisim.DegerYap(sorguTipi) + ",";
            sql += VeriErisim.DegerYap(refNo) + ",";
            sql += VeriErisim.DegerYap(hataKodu) + ",";
            sql += VeriErisim.DegerYap(hataMesaji) + ",";
            sql += VeriErisim.DegerYap(kurum.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Borcu Yok Belgesine ait durum bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="belge">Güncellenecek Borcu Yok Belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc BelgeDurumDegistir(BorcuYokBelgesiSGK belge)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], belge.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], belge.tarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", belge.kod));

            return KomutCalistir(sql, 1);
        }
    }
}
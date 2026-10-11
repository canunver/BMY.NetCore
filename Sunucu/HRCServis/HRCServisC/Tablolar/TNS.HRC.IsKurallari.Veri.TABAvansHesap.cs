using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Avans hesap bilgisini tutan HRCAVANSHESAP veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABAvansHesap : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAvansHesap(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Avans Hesap tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCAVANSHESAP";

        /// <summary>
        /// Avans Hesap tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
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
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// GUNDELIKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNDELIKTUTAR = 5,
            /// <summary>
            /// GUNDELIKPARABIRIMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNDELIKPARABIRIMI = 6,
            /// <summary>
            /// GUNSAYISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNSAYISI = 7,
            /// <summary>
            /// ARTTIRIMLIGUNSAYISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ARTTIRIMLIGUNSAYISI = 8,
            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 9,
            /// <summary>
            /// TUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUTAR = 10,
            /// <summary>
            /// PARABIRIMIKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARABIRIMIKOD = 11,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 12,
            /// <summary>
            /// DAMGAVERGISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DAMGAVERGISI = 13,
            /// <summary>
            /// TARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIH = 14,
            /// <summary>
            /// TARIHARTTIRIMLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIHARTTIRIMLI = 15,
        }

        /// <summary>
        /// Avans Hesap tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "SIRANO", "GUNDELIKTUTAR", "GUNDELIKPARABIRIMI", "GUNSAYISI", "ARTTIRIMLIGUNSAYISI", "TUR", "TUTAR", "PARABIRIMIKOD", "ACIKLAMA", "DAMGAVERGISI", "TARIH", "TARIHARTTIRIMLI" };

        /// <summary>
        /// Para birimi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADPARABIRIMI = "HRCPARABIRIMI";

        /// <summary>
        /// Para birimi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarParaBirimi
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 1,
            /// <summary>
            /// KISALTMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KISALTMA = 2,
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARPARABIRIMI = { "KOD", "ADI", "KISALTMA" };

        /// <summary>
        /// Avans Hesap bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek Avans Hesap bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).yil) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).muhasebe) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).birim) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).surecNo) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).siraNo) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).gundelikTutar) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).gundelikParaBirimi.kod) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).gunSayisi) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).arttirimliGunSayisi) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).tur) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).tutar) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).paraBirimi.kod) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).aciklama) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).damgaVergisi) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).tarihBaslangic) + ",";
                sql += VeriErisim.DegerYap(((AvansHesap)bilgi.objeler[i]).tarihBaslangicArttirimli) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Avans Hesap bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek Avans Hesap bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(AvansHesap bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Avans Hesap bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek Avans Hesap bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Avans Hesap bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(AvansHesap bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) +
              "," + TABLOADPARABIRIMI + "_1." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.ADI] +
              "," + TABLOADPARABIRIMI + "_1." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KISALTMA] +
              "," + TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.ADI] +
              "," + TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KISALTMA];

            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOADPARABIRIMI + " HRCPARABIRIMI_1 ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GUNDELIKPARABIRIMI] + "=" + TABLOADPARABIRIMI + "_1." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KOD];
            sql += " LEFT OUTER JOIN " + TABLOADPARABIRIMI + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARABIRIMIKOD] + "=" + TABLOADPARABIRIMI + "." + ALANLARPARABIRIMI[(int)ENUMAlanlarParaBirimi.KOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + ", " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
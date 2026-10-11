using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Hastalık İzni bilgisini tutan PSLHASTALIKIZNI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABPSLHastalikIzni : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPSLHastalikIzni(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Hastalık İzni bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLHASTALIKIZNI";

        /// <summary>
        /// Hastalık İzni bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,
            /// <summary>
            /// IZINYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IZINYIL = 5,
            /// <summary>
            /// IZINAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IZINAY = 6,
            /// <summary>
            /// MAASNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MAASNO = 7,
            /// <summary>
            /// SAY2000NO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAY2000NO = 8,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 9,
            /// <summary>
            /// BUAYAKADARKULHASTIZNI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BUAYAKADARKULHASTIZNI = 10,
            /// <summary>
            /// BUAYKULHASTIZNI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BUAYKULHASTIZNI = 11,
            /// <summary>
            /// TOPLAMHASTIZNI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMHASTIZNI = 12,
            /// <summary>
            /// KESILMESIGEREKHASTIZNI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESILMESIGEREKHASTIZNI = 13,
        }

        /// <summary>
        /// Hastalık İzni bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TCKIMLIKNO", "IZINYIL", "IZINAY", "MAASNO", "SAY2000NO", "ADSOYAD", "BUAYAKADARKULHASTIZNI", "BUAYKULHASTIZNI", "TOPLAMHASTIZNI", "KESILMESIGEREKHASTIZNI" };

        /// <summary>
        /// Hastalık izni bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="list">Eklenecek hastalık izni bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            int siraNo = 0;
            foreach (TNS.HRC.PSLHastalikIzni bilgi in list.objeler)
            {
                siraNo++;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.tCKimlikNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.izinYil) + ",";
                sql += VeriErisim.DegerYap(bilgi.izinAy) + ",";
                sql += VeriErisim.DegerYap(bilgi.maasNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.say2000No) + ",";
                sql += VeriErisim.DegerYap(bilgi.adSoyad) + ",";
                sql += VeriErisim.DegerYap(bilgi.buAyaKadarKulHastIzni) + ",";
                sql += VeriErisim.DegerYap(bilgi.buAyKulHastIzni) + ",";
                sql += VeriErisim.DegerYap(bilgi.toplamHastIzni) + ",";
                sql += VeriErisim.DegerYap(bilgi.kesilmesiGerekenHastIzni) + ")";


                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    break;

            }
            return sonuc;
        }

        /// <summary>
        /// Hastalık izni bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak hastalık izni bilgisi kıstas parametresi</param>
        /// <returns>
        /// Hastalık izni bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TNS.HRC.PSLHastalikIzni bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + DBDegiskeni.TABLOADSENELIKTOPLAMIZIN + ".TOPLAMIZIN " +
                    " FROM " + TABLOAD +
                    " LEFT OUTER JOIN " + DBDegiskeni.TABLOADSENELIKTOPLAMIZIN + " ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + DBDegiskeni.TABLOADSENELIKTOPLAMIZIN + ".TCKIMLIKNO" + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.IZINYIL] + " = " + DBDegiskeni.TABLOADSENELIKTOPLAMIZIN + ".YIL";

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

            //if (bilgi.surecNo != "")
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (bilgi.izinYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(DBDegiskeni.TABLOADSENELIKTOPLAMIZIN, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.izinYil));

            if (bilgi.tCKimlikNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tCKimlikNo));
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (bilgi.izinAy > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IZINAY], "=", bilgi.izinAy));
            if (bilgi.izinYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IZINYIL], "=", bilgi.izinYil));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ADSOYAD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hastalık izni bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek hastalık izni bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TNS.HRC.PSLHastalikIzni bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            sonuc = KomutCalistir(sql, 0);

            if (!sonuc.islemSonuc)
                return sonuc;

            return sonuc;
        }
    }

}
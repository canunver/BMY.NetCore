using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Kazanan firma bilgisini tutan HRCKAZANANFIRMA veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKazananFirma : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKazananFirma(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Kazanan firma bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCKAZANANFIRMA";

        /// <summary>
        /// Kazanan firma bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
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
            /// FIRMAKOD1 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKOD1 = 4,
            /// <summary>
            /// FIRMAKOD2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKOD2 = 5,
        }

        /// <summary>
        /// Kazanan firma tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKOD1", "FIRMAKOD2" };

        /// <summary>
        /// Kazanan firma bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kazanan firma bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(KazananFirma bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD1], bilgi.firmaKod1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD2], bilgi.firmaKod2);


            kosul = VeriErisim.KosulEkle(" WHERE", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

            sql += kosul;

            sonuc = KomutCalistir(sql, 1);


            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.firmaKod1) + ",";
                sql += VeriErisim.DegerYap(bilgi.firmaKod2) + ")";

                sonuc = KomutCalistir(sql, 1);

            }



            return sonuc;
        }

        /// <summary>
        /// Kazanan firma bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak kazanan firma bilgisi kıstas parametresi</param>
        /// <returns>
        /// Kazanan firma bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(KazananFirma bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD]
                + "," + TABFirmaBilgisi.TABLOAD + "_1." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.AD] + "," + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.VERGINO]
                + "," + TABFirmaBilgisi.TABLOAD + "_1." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.VERGINO] + " FROM " + TABLOAD + " INNER JOIN " + TABFirmaBilgisi.TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKOD1] + " = " + TABFirmaBilgisi.TABLOAD + "." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];
            sql += " LEFT OUTER JOIN " + TABFirmaBilgisi.TABLOAD + " " + TABFirmaBilgisi.TABLOAD + "_1" + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKOD2] + " = " + TABFirmaBilgisi.TABLOAD + "_1." + TABFirmaBilgisi.ALANLAR[(int)TABFirmaBilgisi.ENUMAlanlar.KOD];
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            //melih gerek yok
            //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SURECNO] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kazanan firma bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek kazanan firma bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(KazananFirma bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}
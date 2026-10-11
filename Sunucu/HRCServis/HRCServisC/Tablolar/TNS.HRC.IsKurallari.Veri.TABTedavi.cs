using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Para birimi bilgisini tutan HRCPARABIRIMI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTedavi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTedavi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Para birimi bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLTEDAVI";

        /// <summary>
        /// Para birimi bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "PSLTEDAVIDETAY";

        /// <summary>
        /// Para birimi bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADFIRMABILGISI = "HRCFIRMABILGISI";

        /// <summary>
        /// Para birimi bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 4,
            /// <summary>
            /// FIRMAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAKOD = 5,
            /// <summary>
            /// TERTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERTIP = 6,
            /// <summary>
            /// ODEMEHESAP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEHESAP = 7,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 8,
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TIP", "FIRMAKOD", "TERTIP", "ODEMEHESAP", "ODEMEEMRINO" };

        /// <summary>
        /// Para birimi bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDetay
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
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// FATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURANO = 6,
            /// <summary>
            /// FATURATARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURATARIH = 7,
            /// <summary>
            /// FATURATUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURATUTAR = 8,
            /// <summary>
            /// KATILIMPAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KATILIMPAYI = 9,
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TIP", "SIRANO", "FATURANO", "FATURATARIH", "FATURATUTAR", "KATILIMPAYI" };

        /// <summary>
        /// Para birimi bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarFirmaBilgisi
        {
            /// <summary>
            /// VERGINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGINO = 0,
            /// <summary>
            /// HESAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPNO = 1,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 2,
            /// <summary>
            /// VERGIDAIRESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGIDAIRESI = 3,
            /// <summary>
            /// BANKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKA = 4,
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 5,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 6,
        }

        /// <summary>
        /// Para birimi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARFIRMABILGISI = { "VERGINO", "HESAPNO", "AD", "VERGIDAIRESI", "BANKA", "KOD", "MUHASEBEKOD" };

        /// <summary>
        /// Para birimi bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Tedavi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tertip) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeHesap) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriNo) + ")";

            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], bilgi.firma.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERTIP], bilgi.tertip) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEHESAP], bilgi.odemeHesap);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Para birimi bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(Tedavi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TIP], "=", (bilgi.tip)));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Para birimi bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(TedaviDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.faturaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.faturaTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.faturaTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.katilimPayi) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Para birimi bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Para birimi bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(Tedavi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " , " +
                              VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " , " +
                              VeriErisim.TumAlanlar(TABLOADFIRMABILGISI, ALANLARFIRMABILGISI);

            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOADDETAY + " ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TIP] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TIP] +
                    " LEFT OUTER JOIN " + TABLOADFIRMABILGISI + " ON " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKOD] + " = " + TABLOADFIRMABILGISI + "." + ALANLARFIRMABILGISI[(int)ENUMAlanlarFirmaBilgisi.KOD] + " AND " +
                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + " = " + TABLOADFIRMABILGISI + "." + ALANLARFIRMABILGISI[(int)ENUMAlanlarFirmaBilgisi.MUHASEBEKOD];

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ", " +
                                  TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " +
                                  TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " +
                                  TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + ", " +
                                  TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Para birimi bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(Tedavi bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Para birimi bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Para birimi bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleYillikTedaviOrtalamasi(TedaviIstatistik kriter)
        {
            string sql = "";
            string kosul = "";
            string groupBy = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD]
                + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TIP]
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.FATURATUTAR] + ")"
                 + ", " + VeriErisim.IslecBul("MIN") + "(" + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".SAYI)"
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.FATURATUTAR] + ") /"
                + VeriErisim.IslecBul("MIN") + "(" + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".SAYI)"
                + " FROM " + TABLOAD;

            sql += " INNER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TIP] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TIP];

            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";
            sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL];

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<>", (int)ENUMSurecDurum.IPTAL));

            sql += " INNER JOIN " + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + " ON ";
            sql += "  " + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".MUHASEBEKODU" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            sql += " AND " + DBDegiskeni.TABLOADPERBIRIMDAGILIMIVIEW + ".BIRIMKODU" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];


            sql += " INNER JOIN " + DBDegiskeni.MUHMIF + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO];
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIF, "HESAPKOD", "LIKE", "830%"));

            sql += " INNER JOIN " + DBDegiskeni.MUHMIFFORM + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + DBDegiskeni.MUHMIFFORM + ".BELGENO";
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + DBDegiskeni.MUHMIFFORM + ".YIL";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIFFORM, "FORMDURUM", "<", 9));


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kriter.birim));

            groupBy = " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD]
             + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TIP];

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (groupBy != "")
                sql += groupBy;

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
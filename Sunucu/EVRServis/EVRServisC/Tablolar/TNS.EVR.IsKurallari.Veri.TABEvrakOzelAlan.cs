using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak özel alan bilgilerini içeren EVREVRAKOZELALAN veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class TABEvrakOzelAlan : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakOzelAlan sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakOzelAlan(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Evrak özel alan bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOAD = "EVREVRAKOZELALAN";

        /// <summary>
        /// Evrak özel alan bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            PREVRAKNO = 0,
            ALANKODU = 1,
            DEGER = 2,
        }

        /// <summary>
        /// Evrak özel alan bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "PREVRAKNO", "ALANKODU", "DEGER" };

        /// <summary>
        /// Parametre olarak verilen EvrakOzelAlan nesne koleksiyonuna göre; evrak özel alan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">EvrakOzelAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ArrayList liste)
        {
            Sonuc sonuc = new Sonuc();
            foreach (EvrakOzelAlan bilgi in liste)
            {
                if (bilgi.prEvrakNo.Trim() == "")
                    continue;
                if (bilgi.deger.Trim() == "")
                {
                    Sil(bilgi);
                    continue;
                }
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.prEvrakNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.ozelAlan.alanKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.deger) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGER], bilgi.deger);
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", bilgi.prEvrakNo);
                    sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALANKODU], "=", bilgi.ozelAlan.alanKod);

                    sonuc = KomutCalistir(sql, 1);

                    if (!sonuc.islemSonuc)
                        return sonuc;
                }
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisi ve birim kod bilgisine göre; evrak özel alan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <param name="birimKod">Evrak birim kodunu içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(string prEvrakNo, string birimKod)
        {
            string sql = " SELECT EVREVRAKOZELALAN.PREVRAKNO, EVREVRAKOZELALAN.DEGER,EVROZELALAN.ALANKODU, EVROZELALAN.EVRAKBIRIMKODU, EVROZELALAN.DOSYAKODU, EVROZELALAN.SIRANO, EVROZELALAN.AD FROM EVROZELALAN LEFT OUTER JOIN  EVREVRAKOZELALAN ON EVROZELALAN.ALANKODU = EVREVRAKOZELALAN.ALANKODU WHERE EVREVRAKOZELALAN.PREVRAKNO = " + VeriErisim.DegerYap(prEvrakNo);
            if (birimKod != "")
            {
                sql += " UNION ";
                sql += " SELECT '','',EVROZELALAN.ALANKODU, EVROZELALAN.EVRAKBIRIMKODU, EVROZELALAN.DOSYAKODU, EVROZELALAN.SIRANO, EVROZELALAN.AD FROM EVROZELALAN  WHERE EVROZELALAN.EVRAKBIRIMKODU=" + VeriErisim.DegerYap(birimKod);
            }

            sql += " ORDER BY " + TABOzelAlan.TABLOAD + "." + TABOzelAlan.ALANLAR[(int)TABOzelAlan.ENUMAlanlar.SIRANO] + "," + TABOzelAlan.TABLOAD + "." + TABOzelAlan.ALANLAR[(int)TABOzelAlan.ENUMAlanlar.EVRAKBIRIMKODU];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen evrak özel alan bilgisine göre; evrak özel alan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="bilgi">Evrak özel alan bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(EvrakOzelAlan bilgi)
        {
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", bilgi.prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALANKODU], "=", bilgi.ozelAlan.alanKod));

            return KomutCalistir(sql, 0);
        }
    }
}
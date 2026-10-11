using System;
using TNS.Veri;
using System.Data;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Birim bilgilerini içeren EVREVRAKSAKLAMA veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABSaklama : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABSaklama sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABSaklama(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Saklama bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKSAKLAMA";

        /// <summary>
        /// Saklama bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            PREVRAKNO = 1,
            KRITERKOD = 2,
            BASLANGICKOD = 3,
            SURE = 4,
            SUREBIRIM = 5,
            BITISTARIH = 6,
            ISLEM = 7,
            TASFIYEPLANI = 8,
            RAPORTARIH = 9,
            ISLEMTARIH = 10,
        }

        /// <summary>
        /// Saklama bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KOD", "PREVRAKNO", "KRITERKOD", "BASLANGICKOD", "SURE", "SUREBIRIM", "BITISTARIH", "ISLEM", "TASFIYEPLANI", "RAPORTARIH", "ISLEMTARIH" };

        /// <summary>
        /// Parametre olarak verilen Saklama nesne koleksiyonuna göre; Saklama bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Saklama nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(EvrakSaklama bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.prEvrakNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kriterKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.baslangicKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.sure) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.sureBirim) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.bitisTarih) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.islem) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.tasfiyePlani) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.raporTarih) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.islemTarih) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], bilgi.prEvrakNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KRITERKOD], bilgi.kriterKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICKOD], bilgi.baslangicKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURE], bilgi.sure) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SUREBIRIM], bilgi.sureBirim) + ",";
                if (!bilgi.bitisTarih.isNull)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], bilgi.bitisTarih) + ",";
                if (bilgi.islem > 0)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], bilgi.islem) + ",";
                if (bilgi.tasfiyePlani > 0)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASFIYEPLANI], bilgi.tasfiyePlani) + ",";
                if (!bilgi.raporTarih.isNull)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RAPORTARIH], bilgi.raporTarih) + ",";
                if (!bilgi.islemTarih.isNull)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], bilgi.islemTarih) + ",";

                sql = sql.TrimEnd(',');

                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Saklama nesne koleksiyonuna göre; Saklama bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Saklama nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(string kod)
        {
            string sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod);

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Saklama olarak verilen Tasfiye nesnesine göre; Saklama bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Saklama bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>

        public IDataReader Listele(string prEvrakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc IslemGuncelle(string prEvrakNo, int islem, TNSDateTime islemTarih)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], islem) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], islemTarih) + ",";
            if (islem == (int)ENUMEvrakSaklamaIslem.RAPORALINDI)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RAPORTARIH], islemTarih) + ",";

            sql = sql.TrimEnd(',');
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo);

            return KomutCalistir(sql, 0);
        }
    }
}

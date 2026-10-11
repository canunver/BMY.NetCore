using System;
using TNS.Veri;
using System.Data;

namespace TNS.EVR.IsKurallari.Veri
{

    /// <summary>
    /// kimlik bilgilerini içeren EVRFAKSKIMLIK veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABEvrakFaksKimlik : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakFaksTarihce sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakFaksKimlik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// TABEvrakFaksKimlik bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRFAKSKIMLIK";

        /// <summary>
        /// Faks kimlik bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            KIMLIKKODU = 0,
            BIRIMKOD = 1,
            KAYITZAMANI = 2,
        }

        /// <summary>
        /// Faks kimlik bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KIMLIKKODU", "BIRIMKOD", "KAYITZAMANI"};

        /// <summary>
        /// Parametre olarak verilen Faks kimlik nesne koleksiyonuna göre; kimlik bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Faks kimlik nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(EvrakFaksKimlik form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
           
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(form.kimlikKodu) + ", ";
                sql = sql + VeriErisim.DegerYap(form.birimKod) + ", ";
                sql = sql + VeriErisim.DegerYap(form.kayitZamani) + ")";

                sonuc = KomutCalistir(sql, 1);
                sonuc.anahtar = form.kimlikKodu;
                if (!sonuc.islemSonuc)
                    return sonuc;
            return sonuc;
        }

        public string BirimBul(string kimlik)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "SELECT BIRIMKOD" + " FROM " + TABLOAD;
            if (kimlik != "")
                sql += " WHERE KIMLIKKODU= " + "'" + kimlik + "'";

            sql += " ORDER BY 1";

            return veriErisim.TekKolonSQLStr(sql);
        }

        public string Getir(string birim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "SELECT KIMLIKKODU" + " FROM " + TABLOAD;
            if (birim != "")
                sql += " WHERE BIRIMKOD= " + "'" + birim + "'";

            sql += " ORDER BY 1";

            return veriErisim.TekKolonSQLStr(sql);
        }

        public IDataReader DosyaKoduGetir(string birim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            lock (this)
            {
                veriErisim.BeginTransaction();
                sql = "SELECT TOP 1 * FROM [Evrak].[dbo].[EVRFAKS] ";
                if (birim != "")
                    sql += " WHERE BIRIMKOD= " + "'" + birim + "'";
                sql += " AND DURUMU=1 ORDER BY ONCELIK ASC,TEKRARSAYISI ASC,GONDERILMEZAMANI DESC";
            }
                return veriErisim.DataReaderDondur(sql);
        }
    }
}

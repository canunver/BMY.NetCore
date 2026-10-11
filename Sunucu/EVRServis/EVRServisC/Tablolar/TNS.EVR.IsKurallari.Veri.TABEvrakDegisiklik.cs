using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakDegisiklik : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakDegisiklik sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakDegisiklik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// TABEvrakDegisiklik sınıfına ait, tablo adını tutan değişken
        /// </summary>
        private string TABLOAD = "EVREVRAKDEGISIKLIK";

        /// <summary>
        /// TABEvrakDegisiklik sınıfına ait, tanımlama işlem bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar { EVREVRAKNO = 0, KULLANICI = 1, TARIH = 2, DEGISIKLIK = 3, ISLEMYAPANAD = 4 }

        /// <summary>
        /// TABEvrakDegisiklik sınıfına ait, tanımlama işlem bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        private string[] ALANLAR = { "EVREVRAKNO", "KULLANICI", "TARIH", "DEGISIKLIK", "ISLEMYAPANAD" };

        /// <summary>
        /// TABEvrakDegisiklik sınıfına ait, sql cümlesini tutan değişken
        /// </summary>
        private string sql = "";

        /// <summary>
        /// Parametre olarak verilen EvrakDegisiklik nesne koleksiyonuna göre; EvrakDegisiklik bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">EvrakDegisiklik nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(EvrakDegisiklik form)
        {
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(form.evrEvrakNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.kullanici) + ", ";
            sql = sql + VeriErisim.DegerYap(form.tarih) + ", ";
            sql = sql + VeriErisim.DegerYap(form.deger) + ", ";
            sql = sql + VeriErisim.DegerYap(form.islemYapanAd) + ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EvrakDegisiklik bilgi)
        {
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", bilgi.evrEvrakNo));
            if (!string.IsNullOrEmpty(bilgi.kullanici))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICI], "=", bilgi.kullanici));
            if (!bilgi.tarih.isNull)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TARIH], "=", bilgi.tarih));

            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TARIH] + " DESC";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

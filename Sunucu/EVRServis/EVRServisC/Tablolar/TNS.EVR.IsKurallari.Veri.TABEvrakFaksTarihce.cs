using System;
using TNS.Veri;
using System.Data;

namespace TNS.EVR.IsKurallari.Veri
{

    /// <summary>
    /// Birim bilgilerini içeren EVRFAKSTARIHCE veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABEvrakFaksTarihce : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakFaksTarihce sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakFaksTarihce(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// TABEvrakFaksTarihce bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRFAKSTARIHCE";

        /// <summary>
        /// Faks Tarihce bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            TARIHCEKODU = 0,
            KODU = 1,
            KAYITEDEN = 2,
            KAYITZAMANI = 3,
            KAYITACIKLAMA = 4,
        }

        /// <summary>
        /// Faks Tarihce bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "TARIHCEKODU", "KODU", "KAYITEDEN", "KAYITZAMANI", "KAYITACIKLAMA" };

        /// <summary>
        /// Parametre olarak verilen Faks Tarihce nesne koleksiyonuna göre; tarihce bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Faks Tarihce nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            foreach (EvrakFaksTarihce bilgi in liste.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.tarihceKodu) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kodu) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kayitEden) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kayitZamani) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kayitAciklama) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen faks tarihce nesne koleksiyonuna göre; faks tarihce bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">tarihce nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(string kodu)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODU], "=", kodu);

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
                return sonuc;

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen Tasfiye nesnesine göre; Tasfiye bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Tasfiye bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(EvrakFaksTarihce bilgi)
        {
            string sql = "";
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.tarihceKodu != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TARIHCEKODU], "=", bilgi.tarihceKodu));
            if (bilgi.kodu != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KODU], "=", bilgi.kodu));
            if (bilgi.kayitEden != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITEDEN], "=", bilgi.kayitEden));
            if (!bilgi.kayitZamani.isNull)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITZAMANI], "=", bilgi.kayitZamani));
            if (bilgi.kayitAciklama != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITACIKLAMA], "=", bilgi.kayitAciklama));
            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);

        }
    }
}

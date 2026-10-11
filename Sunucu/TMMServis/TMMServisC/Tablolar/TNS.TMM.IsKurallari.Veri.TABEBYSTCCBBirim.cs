using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Muhasebe Bilgi kayıtlarını tutan TASEBYSCumhurbaskanligiBirim veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABEBYSCumhurbaskanligiBirim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEBYSCumhurbaskanligiBirim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASEBYSTCCBBIRIM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            MUHASEBEKOD = 2,
            HARCAMAKOD = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "AD", "MUHASEBEKOD", "HARCAMAKOD" };


        public Sonuc Kaydet(EBYSCumhurbaskanligiBirim form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.ad) + ",";
            sql += VeriErisim.DegerYap(form.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(form.harcamaKod.Replace(".", "").Trim()) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EBYSCumhurbaskanligiBirim form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMAKOD], "=", form.harcamaKod.Replace(".", "").Trim()));
            }

            return veriErisim.DataReaderDondur(sql);
        }

    }
}

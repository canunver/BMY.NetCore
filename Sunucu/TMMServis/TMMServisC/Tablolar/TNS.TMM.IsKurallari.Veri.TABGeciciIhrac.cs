using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{

    /// <summary>
    /// Geçici İhraç
    /// </summary>
    public class TABGeciciIhrac : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGeciciIhrac(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASGECICIIHRAC";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            PRSICILNO = 0,
            YIL = 1,
            MUHASEBEKOD = 2,
            HARCAMABIRIMKOD = 3,
            FISNO = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "PRSICILNO", "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO" };

        public Sonuc Kaydet(GeciciIhrac form)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(form.yil) + ",";
            sql += VeriErisim.DegerYap(form.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(form.harcamaKod) + ",";
            sql += VeriErisim.DegerYap(form.fisNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public bool GeciciIhracKaydiVarMi(int prSicilNo)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            return veriErisim.TekKolonSQLInt(sql) > 0;
        }

    }
}
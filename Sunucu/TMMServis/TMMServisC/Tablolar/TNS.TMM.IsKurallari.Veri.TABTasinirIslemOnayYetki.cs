using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır İşlem Formu Onay Yetki bilgisini tutan TASISLEMONAYYETKI veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABTasinirIslemOnayYetki : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTasinirIslemOnayYetki(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASISLEMONAYYETKI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            MUHASEBEKOD = 0,
            HARCAMABIRIMKOD = 1,
            MERNIS = 2,
            ADSOYAD = 3,
            ONAYTUR = 4
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "MERNIS", "ADSOYAD", "ONAYTUR" };

        public IDataReader Listele(TasinirIslemOnayYetki form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",TASPERSONEL.EPOSTA FROM " + TABLOAD;
            sql += " INNER JOIN TASPERSONEL ON TASISLEMONAYYETKI.MERNIS = TASPERSONEL.KOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaBirimKod.Replace(".", "")));

            if (form.mernis != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MERNIS], "=", form.mernis));
            if (form.onayTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYTUR], "=", form.onayTur));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(TasinirIslemOnayYetki form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(form.harcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(form.mernis) + ",";
            sql += VeriErisim.DegerYap(form.adSoyad) + ",";
            sql += VeriErisim.DegerYap(form.onayTur) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(TasinirIslemOnayYetki form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaBirimKod.Replace(".", "")));
            if (form.mernis != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.mernis));
            if (form.onayTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ONAYTUR], "=", form.onayTur));

            return KomutCalistir(sql, 0);
        }
    }
}

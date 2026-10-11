using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{

    /// <summary>
    /// Sicil no özellik bilgilerini tutan TASSICILNODURUM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSicilNoDurum : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSicilNoDurum(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSICILNODURUM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            PRSICILNO = 0,
            GORSICILNO = 1,
            TIFKOD = 2,
            DURUM = 3,
            DURUMISLEM = 4,
            DURUMACIKLAMA = 5,
            ISLEMTARIH = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "PRSICILNO", "GORSICILNO", "TIFKOD", "DURUM", "DURUMISLEM", "DURUMACIKLAMA", "ISLEMTARIH" };

        public Sonuc Kaydet(SicilNoDurum form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(form.gorSicilNo) + ",";
            sql += VeriErisim.DegerYap(form.tifKod) + ",";
            sql += VeriErisim.DegerYap(form.durum) + ",";
            sql += VeriErisim.DegerYap(form.durumIslem) + ",";
            sql += VeriErisim.DegerYap(form.durumAciklama) + ",";
            sql += VeriErisim.DegerYap(form.islemTarihi) + ")";

            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIFKOD], form.tifKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUMISLEM], form.durumIslem) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUMACIKLAMA], form.durumAciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], form.islemTarihi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", form.prSicilNo));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public IDataReader Listele(int prSicilNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
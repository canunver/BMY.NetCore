using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Muhasebe Bilgi kayıtlarını tutan TABRFIDHareket veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABRFIDHareket : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABRFIDHareket(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASRFIDETIKETHAREKET";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            PRSICILNO = 1,
            ETIKETTURKOD = 2,
            ISLEMTARIH = 3,
            ISLEMYAPAN = 4,
            ISLEMTUR = 5,
            DURUM = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "PRSICILNO", "ETIKETTURKOD", "ISLEMTARIH", "ISLEMYAPAN", "ISLEMTUR", "DURUM" };


        public Sonuc Kaydet(RFIDHareket form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(form.etiketTurKod) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemTur) + ",";
            sql += VeriErisim.DegerYap(form.durum) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(RFIDHareket form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", form.prSicilNo));
                if (form.etiketTurKod > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETIKETTURKOD], "=", form.etiketTurKod));
                if (form.islemYapan != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));
                if (form.islemTur != 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", form.islemTur));
                if (form.durum != 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", form.durum));
            }

            return veriErisim.DataReaderDondur(sql);
        }

    }
}

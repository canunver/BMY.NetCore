using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Muhasebe Bilgi kayıtlarını tutan TASEBYSCumhurbaskanligi veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABEBYSCumhurbaskanligi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEBYSCumhurbaskanligi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASEBYSTCCB";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            EVRAKID = 0,
            YIL = 1,
            MUHASEBEKOD = 2,
            HARCAMAKOD = 3,
            FISNO = 4,
            BELGETUR = 5,
            ISLEMYAPAN = 6,
            ISLEMTARIH = 7,
            DURUM = 8,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "EVRAKID", "YIL", "MUHASEBEKOD", "HARCAMAKOD", "FISNO", "BELGETUR", "ISLEMYAPAN", "ISLEMTARIH", "DURUM" };


        public Sonuc Kaydet(EBYSCumhurbaskanligi form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrakId) + ",";
            sql += VeriErisim.DegerYap(form.yil) + ",";
            sql += VeriErisim.DegerYap(form.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(form.harcamaKod.Replace(".", "").Trim()) + ",";
            sql += VeriErisim.DegerYap(form.fisNo) + ",";
            sql += VeriErisim.DegerYap(form.belgeTur) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.durum) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EBYSCumhurbaskanligi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (form.evrakId > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKID], "=", form.evrakId));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMAKOD], "=", form.harcamaKod.Replace(".", "").Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", form.fisNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", form.belgeTur));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public int DurumGetirTekKolon(EBYSCumhurbaskanligi form)
        {
            string sql = "SELECT DURUM FROM " + TABLOAD;

            if (form.evrakId > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKID], "=", form.evrakId));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMAKOD], "=", form.harcamaKod.Replace(".", "").Trim()));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", form.fisNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", form.belgeTur));
            }

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc DurumDegistir(EBYSCumhurbaskanligi form, int yeniDurum)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)yeniDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKID], "=", form.evrakId));

            return KomutCalistir(sql, 1);
        }

    }
}

using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// RFID Yazıcı bilgilerini tutan TASRFIDYAZICI veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABRFIDYazici : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABRFIDYazici(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASRFIDYAZICI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            MUHASEBEKOD = 1,
            HARCAMABIRIMKOD = 2,
            IP = 3,
            AD = 4
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "HARCAMABIRIMKOD", "IP", "AD" };

        public Sonuc Kaydet(RFIDYazici form)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(form.harcamaBirimKod) + ",";
            sql += VeriErisim.DegerYap(form.ip) + ",";
            sql += VeriErisim.DegerYap(form.ad) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(RFIDYazici form)
        {
            string sql = "";
            string kriter = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            if (!string.IsNullOrWhiteSpace(form.muhasebeKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], form.muhasebeKod);
            }
            if (!string.IsNullOrWhiteSpace(form.harcamaBirimKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], form.harcamaBirimKod);
            }
            if (!string.IsNullOrWhiteSpace(form.ip))
            {
                if (kriter != "") kriter += ", ";
                kriter += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IP], form.ip);
            }
            if (!string.IsNullOrWhiteSpace(form.ad))
            {
                if (kriter != "") kriter += ", ";
                kriter += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            }

            if (!string.IsNullOrWhiteSpace(kriter))
                sql += kriter;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));


            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(RFIDYazici form)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(RFIDYazici form)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            if (form.ip != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IP], "=", form.ip));

            if (form.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + form.ad + "%"));

            if (form.muhasebeKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));

            if (form.harcamaBirimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaBirimKod));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + " , " + ALANLAR[(int)ENUMAlanlar.IP] + " , " + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
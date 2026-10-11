using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProjeYurutucu : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeYurutucu(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJEYURUTUCU";
        public static string[] ALANLAR = {
                "PROJEKOD",
                "TCKIMLIKNO",
                "ADSOYAD",
                "KURUM",
                "UNVAN",
                "TEL",
                "EPOSTA",
                "PROJEDEKIROLU",
        };

        public enum ENUMAlanlar
        {
            PROJEKOD = 0,
            TCKIMLIKNO = 1,
            ADSOYAD = 2,
            KURUM = 3,
            UNVAN = 4,
            TEL = 5,
            EPOSTA = 6,
            PROJEDEKIROLU = 7,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeYurutucu form)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql += VeriErisim.DegerYap(form.projeKod) + ", ";
            sql += VeriErisim.DegerYap(form.tcKimlikNo) + ", ";
            sql += VeriErisim.DegerYap(form.adSoyad) + ", ";
            sql += VeriErisim.DegerYap(form.kurum) + ", ";
            sql += VeriErisim.DegerYap(form.unvan) + ", ";
            sql += VeriErisim.DegerYap(form.tel) + ", ";
            sql += VeriErisim.DegerYap(form.eposta) + ", ";
            sql += VeriErisim.DegerYap(form.projedekiRolu) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";

                if (form.adSoyad != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADSOYAD], form.adSoyad) + ",";
                if (form.kurum != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUM], form.kurum) + ",";
                if (form.unvan != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVAN], form.unvan) + ",";
                if (form.tel != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEL], form.tel) + ",";
                if (form.tel != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], form.tel) + ",";
                if (form.projedekiRolu != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJEDEKIROLU], form.projedekiRolu) + ",";

                sql = sql.TrimEnd(',');

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", form.tcKimlikNo));
                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(ProjeYurutucu form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.tcKimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", form.tcKimlikNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ProjeYurutucu form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));
            if (!string.IsNullOrWhiteSpace(form.tcKimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", form.tcKimlikNo));

            return KomutCalistir(sql, 0);
        }

    }
}

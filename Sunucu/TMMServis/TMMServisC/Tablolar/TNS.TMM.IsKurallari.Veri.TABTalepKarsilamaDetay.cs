using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABTalepKarsilamaDetay : TABOrtak
    {
        public TABTalepKarsilamaDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        public static string TABLOAD = "TASTALEPKARSILAMADETAY";

        public enum ENUMAlanlar : int
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            HARCAMABIRIMKOD = 2,
            BELGENO = 3,
            SIRANO = 4,
            HESAPPLANKOD = 5,
            AMBARKOD = 6,
            ISTENILENMIKTAR = 7,
            KARSILANANMIKTAR = 8,
            SATINALINANMIKTAR = 9,
            ACIKLAMA = 10,
            TIFYIL = 11,
            TIFFISNO = 12,
            IPTAL = 13,
            BIRIMFIYAT = 14,
            REFIDANA = 15,
            REFID = 16,
            PARABIRIMIKOD = 17,
        }

        public static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "BELGENO", "SIRANO", "HESAPPLANKOD", "AMBARKOD",
                                            "ISTENILENMIKTAR", "KARSILANANMIKTAR", "SATINALINANMIKTAR", "ACIKLAMA", "TIFYIL", "TIFFISNO", "IPTAL","BIRIMFIYAT","REFIDANA","REFID","PARABIRIMIKOD" };

        public Sonuc Kaydet(TalepKarsilama tk, TalepKarsilamaDetay tkd)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.yil) + ",";
            sql += VeriErisim.DegerYap(tk.muhasebe.kod) + ",";
            sql += VeriErisim.DegerYap(tk.harcamaBirimi.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tk.belgeNo) + ",";
            sql += VeriErisim.DegerYap(tkd.siraNo) + ",";
            sql += VeriErisim.DegerYap(tkd.hesapPlani.hesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tkd.ambar == null ? string.Empty : tkd.ambar.kod) + ",";
            sql += VeriErisim.DegerYap(tkd.istenilenMiktar) + ",";
            sql += VeriErisim.DegerYap(tkd.karsilananMiktar) + ",";
            sql += VeriErisim.DegerYap(tkd.satinAlinanMiktar) + ",";
            sql += VeriErisim.DegerYap(tkd.aciklama) + ",";
            sql += VeriErisim.DegerYap(tkd.tifYil) + ",";
            sql += VeriErisim.DegerYap(tkd.tifFisNo) + ",";
            sql += VeriErisim.DegerYap(tkd.iptal) + ",";
            sql += VeriErisim.DegerYap(tkd.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(tkd.refIdAna) + ",";
            sql += VeriErisim.DegerYap(tkd.refId) + ",";
            sql += VeriErisim.DegerYap(tkd.paraBirimiKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(TalepKarsilama tk)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tk.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tk.muhasebe.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tk.harcamaBirimi.kod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tk.belgeNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(TalepKarsilama tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASHESAPPLAN.ACIKLAMA, TASOLCUBIRIM.AD, ButceIslemleri.dbo.MUHHESAPPLANI.ACIKLAMA";
            //sql += " FROM " + TABLOAD + ", TASHESAPPLAN, TASOLCUBIRIM";
            //sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD] + " = TASHESAPPLAN.KOD";
            //sql += " AND TASHESAPPLAN.OLCUBIRIMKOD = TASOLCUBIRIM.KOD";

            sql += " FROM TASOLCUBIRIM RIGHT OUTER JOIN";
            sql += " TASHESAPPLAN ON TASOLCUBIRIM.KOD = TASHESAPPLAN.OLCUBIRIMKOD RIGHT OUTER JOIN";
            sql += " TASTALEPKARSILAMADETAY ON TASHESAPPLAN.KOD = TASTALEPKARSILAMADETAY.HESAPPLANKOD LEFT OUTER JOIN";
            sql += " ButceIslemleri.dbo.MUHHESAPPLANI ON TASTALEPKARSILAMADETAY.YIL = ButceIslemleri.dbo.MUHHESAPPLANI.YIL AND ";
            sql += " TASTALEPKARSILAMADETAY.HESAPPLANKOD = ButceIslemleri.dbo.MUHHESAPPLANI.HESAPKOD";


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tk.yil));

            if (!string.IsNullOrEmpty(tk.muhasebe.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tk.muhasebe.kod));

            if (!string.IsNullOrEmpty(tk.harcamaBirimi.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tk.harcamaBirimi.kod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tk.belgeNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tk.belgeNo));

            if (!string.IsNullOrEmpty(tk.refId))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.REFIDANA], "=", tk.refId));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Iptal(string urunRefId)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IPTAL], 1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.REFID], "=", urunRefId));

            return KomutCalistir(sql, 1);
        }
    }
}
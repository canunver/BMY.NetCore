using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABKepEvrak : TABOrtak
    {
        public TABKepEvrak(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRKEPEVRAK";

        public enum ENUMAlanlar : int
        {
            KOD = 0, EVREVRAKNO = 1, GONDEREN = 2, ALICI = 3, ISLEM = 4, ISLEMTARIH = 5, ISLEMYAPAN = 6, ISLEMYAPANAD = 7, KEPID = 8, KEPMESAJID = 9,
        }

        public static string[] ALANLAR = { "KOD", "EVREVRAKNO", "GONDEREN", "ALICI", "ISLEM", "ISLEMTARIH", "ISLEMYAPAN", "ISLEMYAPANAD", "KEPID", "KEPMESAJID" };

        public Sonuc Kaydet(KepEvrak form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.gonderen) + ",";
                sql += VeriErisim.DegerYap(form.alici) + ",";
                sql += VeriErisim.DegerYap((int)form.islem) + ",";
                sql += VeriErisim.DegerYap(form.islemTarihi) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
                sql += VeriErisim.DegerYap(form.kepId) + ",";
                sql += VeriErisim.DegerYap(form.kepMesajId) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        public Sonuc Guncelle(KepEvrak form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            if (form.islem != ENUMKEPEvrakIslem.TANIMSIZ)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], (int)form.islem) + ",";
            if (form.kepId != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KEPID], form.kepId) + ",";
            if (form.kepMesajId != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KEPMESAJID], form.kepMesajId) + ",";

            sql = sql.TrimEnd(',');

            if (form.kod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
                return KomutCalistir(sql, 1);
            }
            else if (form.evrEvrakNo != "")
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));
                return KomutCalistir(sql, 0);
            }
            return new Sonuc("KepEvrak bilgi güncellme işlemi için kod veya evrEvrakNo bilgisi bulunamadı.");
        }

        public IDataReader Listele(KepEvrak form)
        {
            string sql = "";
            if (form.kod > 0)
            {
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            }
            else
            {
                sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", " +
                        "EVREVRAKSICIL.PREVRAKNO, " +
                        "EVREVRAK.YIL, " +
                        "EVREVRAK.EVRAKNO, " +
                        "EVREVRAK.EVRAKTARIH, " +
                        "EVREVRAK.GELDIGIYER, " +
                        "EVREVRAK.KONU, " +
                        "EVREVRAK.DOSYASIVAR, " +
                        "EVREVRAK.DAGITIMTARIH, " +
                        "EVREVRAK.EIMZATARIHI, " +
                        "EVREVRAK.IMZABITTI ";
                sql += "FROM " + TABLOAD + " INNER JOIN ";
                sql += "EVREVRAK ON " + TABLOAD + ".EVREVRAKNO = EVREVRAK.EVREVRAKNO " +
                       "INNER JOIN EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO";

                string ekKosul = "(EVREVRAKSICIL.ASIL = 1) ";

                if (form.evrEvrakNo != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));
                if (form.kepId != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KEPID], "=", form.kepId));
                if (form.kepMesajId != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KEPMESAJID], "=", form.kepMesajId));
                if (form.islemTarihi.Yil > 1)
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", form.islemTarihi));

                if (form.islem != ENUMKEPEvrakIslem.TANIMSIZ)
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "=", (int)form.islem));

                sql += " WHERE " + ekKosul;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(KepEvrak form)
        {
            form.islem = ENUMKEPEvrakIslem.IPTAL;
            return Guncelle(form);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}

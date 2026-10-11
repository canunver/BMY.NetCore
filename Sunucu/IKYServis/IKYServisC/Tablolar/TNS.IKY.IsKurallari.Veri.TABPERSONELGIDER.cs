using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPERSONELGIDER : TABOrtak
    {
        public TABPERSONELGIDER(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELGIDER";

        public enum ENUMAlanlar : int
        {
            PKOD = 0,
            YERLESKEODAKOD = 1,
            HARCAMATUR = 2,
            TARIH = 3,
            TUTAR = 4,
            MAILGONDERILDI = 5,
            DURUM = 6,
            DEGISIKLIGIYAPAN = 7,
            DEGISIKLIKZAMANI = 8
        }
        static string[] ALANLAR = { "PKOD", "YERLESKEODAKOD", "HARCAMATUR", "TARIH", "TUTAR", "MAILGONDERILDI", "DURUM", "DEGISIKLIGIYAPAN", "DEGISIKLIKZAMANI" };

        public Sonuc Kaydet(string kullaniciKodu, TNS.IKY.IKYPersonelGider p)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += veriErisim.DegerYapPRM(p.pKod) + ",";
            sql += veriErisim.DegerYapPRM(p.yerleskeKod) + ",";
            sql += veriErisim.DegerYapPRM((int)p.harcamaTur) + ",";
            sql += veriErisim.DegerYapPRM(p.tarih) + ",";
            sql += veriErisim.DegerYapPRM(p.tutar) + ",";
            sql += veriErisim.DegerYapPRM(0) + ",";
            sql += veriErisim.DegerYapPRM(1) + ",";
            sql += veriErisim.DegerYapPRM(kullaniciKodu) + ",";
            sql += veriErisim.DegerYapPRM(new TNSDateTime(DateTime.Now)) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(string kullaniciKodu, TNS.IKY.IKYPersonelGider p)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += veriErisim.AtamaOlPRM("", ALANLAR[(int)ENUMAlanlar.TUTAR], p.tutar);
            sql += ", " + veriErisim.AtamaOlPRM("", ALANLAR[(int)ENUMAlanlar.DURUM], 1);
            sql += ", " + veriErisim.AtamaOlPRM("", ALANLAR[(int)ENUMAlanlar.DEGISIKLIGIYAPAN], kullaniciKodu);
            sql += ", " + veriErisim.AtamaOlPRM("", ALANLAR[(int)ENUMAlanlar.DEGISIKLIKZAMANI], new TNSDateTime(DateTime.Now));
            sql += ", " + veriErisim.AtamaOlPRM("", ALANLAR[(int)ENUMAlanlar.MAILGONDERILDI], 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", veriErisim.KosulOlPRM("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.pKod));
            sql = VeriErisim.KosulEkle(sql, "AND", veriErisim.KosulOlPRM("", ALANLAR[(int)ENUMAlanlar.YERLESKEODAKOD], "=", p.yerleskeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", veriErisim.KosulOlPRM("", ALANLAR[(int)ENUMAlanlar.HARCAMATUR], "=", (int)p.harcamaTur));
            sql = VeriErisim.KosulEkle(sql, "AND", veriErisim.KosulOlPRM("", ALANLAR[(int)ENUMAlanlar.TARIH], "=", p.tarih));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(TNS.IKY.IKYPersonelGider p)
        {
            string sql = "";
            string kosul = "";
            
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", P.TCKIMLIKNO, P.AD, P.SOYAD, K.MuhasebeKodu, K.MuhasebeBirimi, Y.AD FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + "Kullanici.dbo.YERLESKEODA Y ON Y.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YERLESKEODAKOD];

            sql += " LEFT OUTER JOIN " + "IKYPERSONEL P ON P.KOD = " + TABLOAD + "." + "PKOD";
            sql += " LEFT OUTER JOIN " + "IKYPERSONELKADRO K ON K.PKOD = " + TABLOAD + "." + "PKOD";
            sql += " AND K.KADRODURUMU>=100 AND K.KADRODURUMU<202 AND K.AtamaTarihi <= " + TABLOAD + "." + "TARIH  AND (K.AYRILMATARIHI is null or " + TABLOAD + "." + "TARIH < AYRILMATARIHI)";

            if (p.pKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", veriErisim.KosulOlPRM(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.pKod));
            }
            if (p.yerleskeKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", veriErisim.KosulOlPRM(TABLOAD, ALANLAR[(int)ENUMAlanlar.YERLESKEODAKOD], "=", p.yerleskeKod));
            }
            if (p.harcamaTur > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", veriErisim.KosulOlPRM(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMATUR], "=", (int)p.harcamaTur));
            }
            if (!TNSDateTime.IsNull(p.tarih))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", veriErisim.KosulOlPRM(TABLOAD, ALANLAR[(int)ENUMAlanlar.TARIH], "=", p.tarih));
            }
            if (p.kriterAy > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", veriErisim.KosulOlPRM("", "DATEPART(MONTH, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TARIH] + ")", "=", p.kriterAy));
            }
            if (p.kriterYil > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", veriErisim.KosulOlPRM("", "DATEPART(YEAR, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TARIH] + ")", "=", p.kriterYil));
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TARIH];

            return veriErisim.DataReaderDondur(sql);
        }

        internal static string ListeleUstTablo(TNSDateTime giderBasla, TNSDateTime giderBitis, string ustTabloAlan)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.HARCAMATUR] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.TUTAR] + ") FROM " + TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PKOD] + "=" + ustTabloAlan;
            string kesisimSorgu = string.Format("dbo.GunTarihAraligindaMi(" + ALANLAR[(int)ENUMAlanlar.TARIH] + ",{0},{1}) = 1", VeriErisim.DegerYap(giderBasla), VeriErisim.DegerYap(giderBitis));
            sql = VeriErisim.KosulEkle(sql, "AND", kesisimSorgu);
            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.HARCAMATUR];
            return sql;
        }

        public Sonuc AktarSil(TNS.IKY.IKYPersonelGider p)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", veriErisim.KosulOlPRM("", "DATEPART(YEAR, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TARIH] + ")", "=", p.kriterYil));
            sql = VeriErisim.KosulEkle(sql, "AND", veriErisim.KosulOlPRM("", "DATEPART(MONTH, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TARIH] + ")", "=", p.kriterAy));
            sql = VeriErisim.KosulEkle(sql, "AND", veriErisim.KosulOlPRM(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMATUR], "=", (int)p.harcamaTur));

            return KomutCalistir(sql, 0);
        }

        public Sonuc Aktar(TNS.IKY.IKYPersonelGider gider, string kullaniciKodu)
        {
            string aktarTabloAd;
            if (gider.harcamaTur == TNS.IKY.IKYPersonelGider.ENUMPersonelHarcamaTur.Yemek)
                aktarTabloAd = "VWPERGIDERYEMEK";
            else
                aktarTabloAd = "VWPERGIDERYEMEK";
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += " SELECT " + ALANLAR[(int)ENUMAlanlar.PKOD] + ", " + ALANLAR[(int)ENUMAlanlar.YERLESKEODAKOD] + ", " + (int)gider.harcamaTur + " " + ALANLAR[(int)ENUMAlanlar.HARCAMATUR] + ", " + ALANLAR[(int)ENUMAlanlar.TARIH] + ", " + ALANLAR[(int)ENUMAlanlar.TUTAR]
             + ", 0 " + ALANLAR[(int)ENUMAlanlar.MAILGONDERILDI]
             + ", 1 " + ALANLAR[(int)ENUMAlanlar.DURUM]
             + ", " + VeriErisim.DegerYap(kullaniciKodu) + " " + ALANLAR[(int)ENUMAlanlar.DEGISIKLIGIYAPAN]
             + ", " + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + " " + ALANLAR[(int)ENUMAlanlar.DEGISIKLIKZAMANI] + " FROM " + aktarTabloAd;
            sql = VeriErisim.KosulEkle(sql, "WHERE", veriErisim.KosulOlPRM("", "DATEPART(YEAR, " + ALANLAR[(int)ENUMAlanlar.TARIH] + ")", "=", gider.kriterYil));
            sql = VeriErisim.KosulEkle(sql, "AND", veriErisim.KosulOlPRM("", "DATEPART(MONTH, " + ALANLAR[(int)ENUMAlanlar.TARIH] + ")", "=", gider.kriterAy));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KayitSil(int yerleskeKod, TNS.IKY.IKYPersonelGider.ENUMPersonelHarcamaTur harcamaTur, TNSDateTime tarih)
        {
            throw new NotImplementedException();
        }

        //public Sonuc GuncelleMailGonderildi(IKYPersonelYemek p)
        //{
        //    string sql = "";
        //    sql = "UPDATE " + TABLOAD + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.yemekHareketMailGonderildi], p.mailGonderildi);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.pkYemekHareket], "=", p.kod));
        //    return KomutCalistir(sql, 1);
        //}
    }
}

using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{

    public class TABButceBildirim : TABOrtak
    {
        public TABButceBildirim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "HRCBUTCEBILDIRIM";

        public enum ENUMAlanlar : int
        {
            ORGKOD = 0,
            YIL = 1,
            AY = 2,
            ISLEMTUR = 3,
            TUTAR = 4,
            TARIH = 5
        }

        public static string[] ALANLAR = { 
            "ORGKOD",
            "YIL",
            "AY",
            "ISLEMTUR",
            "TUTAR",   
            "TARIH"   
        };

        public Sonuc Kaydet(ButceBildirim form)
        {
            Sonuc sonuc = this.Guncelle(form);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.orgKod) + ",";
                sql += VeriErisim.DegerYap(form.yil) + ",";
                sql += VeriErisim.DegerYap(form.ay) + ",";
                sql += VeriErisim.DegerYap(form.islemTur) + ",";
                sql += VeriErisim.DegerYap(form.tutar) + ",";
                sql += VeriErisim.DegerYap(form.tarih) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(ButceBildirim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTAR], form.tutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], form.tarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", form.orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", form.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", form.islemTur));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ButceBildirim form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", form.orgKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));

            if (form.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", form.ay));

            if (form.islemTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", form.islemTur));

            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ButceBildirim form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", form.orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", form.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", form.islemTur));

            return KomutCalistir(sql, 1);
        }
    }

}
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Stratejik amaç bilgilerini tutan STRGENELHAVUZ veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABPerformansHamVeri : TABOrtak
    {
        public TABPerformansHamVeri(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRPERFORMANSHAMVERI";

        public enum ENUMAlanlar : int
        {
            YIL = 0,
            BILGITURU = 1,
            BIRIMTOPLAM = 2,
            ORGKOD = 3,
            ORGTUR = 4,
            ANAHTAR1 = 5,
            ANAHTAR2 = 6,
            ANAHTAR3 = 7,
            ANAHTAR4 = 8,
            BILGI = 9,
            BOLEN = 10,
            IL = 11,
            ILCE = 12
            //KOPRUKOD = 12
        }

        public static string[] ALANLAR = { "YIL", "BILGITURU", "BIRIMTOPLAM", "ORGKOD", "ORGTUR", "ANAHTAR1", "ANAHTAR2", "ANAHTAR3", "ANAHTAR4", "BILGI", "BOLEN", "IL", "ILCE" };

        public Sonuc Kaydet(PerformansHamVeri phVeri, PerformansHamVeriDetay phVeriDetay)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(phVeri.yil) + ",";
            sql += VeriErisim.DegerYap((int)phVeri.bilgiTuru) + ",";
            sql += VeriErisim.DegerYap((int)phVeri.birimToplam) + ",";
            sql += VeriErisim.DegerYap(phVeri.birim.kod) + ",";
            sql += VeriErisim.DegerYap(phVeri.birim.turKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(phVeriDetay.anahtar1) + ",";
            sql += VeriErisim.DegerYap(phVeriDetay.anahtar2) + ",";
            sql += VeriErisim.DegerYap(phVeriDetay.anahtar3) + ",";
            sql += VeriErisim.DegerYap(phVeriDetay.anahtar4) + ",";
            sql += VeriErisim.DegerYap(phVeriDetay.bilgi) + ",";
            sql += VeriErisim.DegerYap(phVeriDetay.bolen) + ",";
            sql += VeriErisim.DegerYap(phVeri.birim.il) + ",";
            sql += VeriErisim.DegerYap(phVeri.birim.ilce) + ")";
            //sql += VeriErisim.DegerYap(phVeri.birim.kopruKod) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(PerformansHamVeri phVeri)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", phVeri.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BILGITURU], "=", (int)phVeri.bilgiTuru));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMTOPLAM], "=", (int)phVeri.birimToplam));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", phVeri.birim.kod));
            if (phVeri.birimToplam != 1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGTUR], "=", phVeri.birim.turKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(PerformansHamVeri phVeri)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", phVeri.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BILGITURU], "=", (int)phVeri.bilgiTuru));
            if (phVeri.birimToplam > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMTOPLAM], "=", (int)phVeri.birimToplam));
            if (phVeri.birim != null && phVeri.birim.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", (int)phVeri.birim.kod));
            if (phVeri.detay != null && phVeri.detay.Length > 0)
            {
                string kosul = string.Empty;
                for (int i = 0; i < phVeri.detay.Length; i++)
                {
                    string icKosul = string.Empty;
                    if (!string.IsNullOrEmpty(phVeri.detay[i].anahtar1) && phVeri.detay[i].anahtar1 != "-")
                        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAHTAR1], "=", phVeri.detay[i].anahtar1));
                    if (!string.IsNullOrEmpty(phVeri.detay[i].anahtar2) && phVeri.detay[i].anahtar2 != "-")
                        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAHTAR2], "=", phVeri.detay[i].anahtar2));
                    if (!string.IsNullOrEmpty(phVeri.detay[i].anahtar3) && phVeri.detay[i].anahtar3 != "-")
                        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAHTAR3], "=", phVeri.detay[i].anahtar3));
                    if (!string.IsNullOrEmpty(phVeri.detay[i].anahtar4) && phVeri.detay[i].anahtar4 != "-")
                        icKosul = VeriErisim.KosulEkle(icKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAHTAR4], "=", phVeri.detay[i].anahtar4));
                    if (!string.IsNullOrEmpty(icKosul))
                    {
                        icKosul = "(" + icKosul + ")";
                        if (!string.IsNullOrEmpty(kosul)) kosul += " OR ";
                        kosul += icKosul;
                    }
                }

                if (!string.IsNullOrEmpty(kosul))
                    sql += " AND (" + kosul + ")";
            }
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ToplamListele(int yil, int IlceIlUlke, string ilceKodIlKod)
        {
            int birimToplam = 1;
            if (IlceIlUlke == 1)
                birimToplam = 1;

            string orderAlanlar = ALANLAR[(int)ENUMAlanlar.BILGITURU] + ", " + ALANLAR[(int)ENUMAlanlar.ORGTUR] + ", " + ALANLAR[(int)ENUMAlanlar.ANAHTAR1] + ", " + ALANLAR[(int)ENUMAlanlar.ANAHTAR2] + ", " + ALANLAR[(int)ENUMAlanlar.ANAHTAR3] + ", " + ALANLAR[(int)ENUMAlanlar.ANAHTAR4];
            string sql = "SELECT " + orderAlanlar + ", SUM(" + ALANLAR[(int)ENUMAlanlar.BILGI] + "), SUM(" + ALANLAR[(int)ENUMAlanlar.BOLEN] + ")";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (IlceIlUlke == 1)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMTOPLAM], "=", birimToplam));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCE], "=", ilceKodIlKod));
            }
            else
            {
                if (IlceIlUlke == 2)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMTOPLAM], "=", 2));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IL], "=", ilceKodIlKod));
                }
                else
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMTOPLAM], "=", 3));
                }
            }
            sql += " GROUP BY " + orderAlanlar;
            sql += " ORDER BY " + orderAlanlar;
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
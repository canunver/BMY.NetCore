using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakDagitimHareket : TABOrtak
    {
        public TABEvrakDagitimHareket(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKDAGITIMHAREKET";

        public enum ENUMAlanlar : int
        {
            PREVRAKNO = 0,
            SIRANO = 1,
            EVRAKNO = 2,
            EVRAKTARIHI = 3,
            KONU = 4,
            DAGITIMYERI = 5,
            DAGITIMTIP = 6,
            IDARIBIRIMKOD = 7,
            EVRAKBIRIMKOD = 8,
            ISLEM = 9,
            SONISLEMTARIHI = 10,
            KISI = 11
        }

        public static string[] ALANLAR = { "PREVRAKNO", "SIRANO", "EVRAKNO", "EVRAKTARIHI", "KONU", "DAGITIMYERI", "DAGITIMTIP", "IDARIBIRIMKOD", "EVRAKBIRIMKOD", "ISLEM", "SONISLEMTARIHI", "KISI" };

        public Sonuc Kaydet(EvrakDagitimHareket form)
        {
            if (form.siraNo > 0)
                return Guncelle(form);

            form.siraNo = MaksSiraNo(form);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.evrakNo) + ",";
            sql += VeriErisim.DegerYap(form.evrakTarihi) + ",";
            sql += VeriErisim.DegerYap(form.konu) + ",";
            sql += VeriErisim.DegerYap(form.dagitimYeri) + ",";
            sql += VeriErisim.DegerYap(form.dagitimTip) + ",";
            sql += VeriErisim.DegerYap(form.idariBirimKimlikKod) + ",";
            sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
            sql += VeriErisim.DegerYap(form.islem) + ",";
            sql += VeriErisim.DegerYap(form.sonIslemTarihi) + ",";
            sql += VeriErisim.DegerYap(form.kisi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(EvrakDagitimHareket form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            if (form.idariBirimKimlikKod != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IDARIBIRIMKOD], form.idariBirimKimlikKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], form.evrakBirimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], form.islem) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONISLEMTARIHI], form.sonIslemTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI], form.kisi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

            return KomutCalistir(sql, 1);
        }
        public IDataReader Listele(EvrakDagitimHareket form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(string prEvrakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            return KomutCalistir(sql, 0);
        }

        public int MaksSiraNo(EvrakDagitimHareket form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            return this.veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}

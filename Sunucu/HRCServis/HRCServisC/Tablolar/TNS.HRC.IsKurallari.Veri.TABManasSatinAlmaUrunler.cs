
using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABManasSatinAlmaUrunler : TABOrtak
    {
        public TABManasSatinAlmaUrunler(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MANASSATINALMAURUNLER";
        public enum ENUMAlanlar
        {
            SATINALMAKOD = 0,
            SIRANO = 1,
            TALEPREFID = 2,
            MUHASEBEKODU = 3,
            HARCAMABIRIMKODU = 4,
            HARCAMABIRIMADI = 5,
            TALEPBELGENO = 6,
            TALEPTARIH = 7,
            HESAPPLANKOD = 8,
            HESAPPLANAD = 9,
            YANSITMAHESAPKOD = 10,
            ACIKLAMA = 11,
            MIKTAR = 12,
            OLCUBIRIMI = 13
        }
        public static string[] ALANLAR = { "SATINALMAKOD", "SIRANO", "TALEPREFID", "MUHASEBEKODU", "HARCAMABIRIMKODU", "HARCAMABIRIMADI", "TALEPBELGENO", "TALEPTARIH", "HESAPPLANKOD", "HESAPPLANAD", "YANSITMAHESAPKOD", "ACIKLAMA", "MIKTAR", "OLCUBIRIMI" };

        public Sonuc Kaydet(ManasSatinAlmaUrun form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.satinAlmaKod) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.talepRefId) + ",";
            sql += VeriErisim.DegerYap(form.muhasebeBirim.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(form.muhasebeBirim.birimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(form.muhasebeBirim.birimAd) + ",";
            sql += VeriErisim.DegerYap(form.talepBelgeNo) + ",";
            sql += VeriErisim.DegerYap(form.talepTarihi) + ",";
            sql += VeriErisim.DegerYap(form.hesapPlanKodu.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(form.hesapPlanAdi) + ",";
            sql += VeriErisim.DegerYap(form.yansitmaHesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(form.aciklama) + ",";
            sql += VeriErisim.DegerYap(form.miktar) + ",";
            sql += VeriErisim.DegerYap(form.olcuBirimi) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ManasSatinAlmaUrun form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SATINALMAKOD], "=", form.satinAlmaKod));

            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SATINALMAKOD] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int satinAlmaKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SATINALMAKOD], "=", satinAlmaKod));

            return KomutCalistir(sql, 0);
        }
    }
}

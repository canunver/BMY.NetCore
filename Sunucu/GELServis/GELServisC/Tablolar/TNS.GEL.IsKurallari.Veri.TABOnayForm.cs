using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABOnayForm : TABOrtak
    {
        public TABOnayForm(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "GELIHALEONAYFORM";
        enum ENUMAlanlar
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            ISLEMTANIM = 4,
            ISNITELIK = 5,
            ISMIKTAR = 6,
            MUHAMMENBEDEL = 7,
            MUHAMMENBEDELPARABIRIMI = 8,
            GECICITEMINATMIKTAR = 9,
            AVANSVARMI = 10,
            AVANSSARTLARI = 11,
            ONILANUYGULANSIN = 12,
            IHALETURU = 13,
            IHALEMADDESI = 14,
            ILANVERILECEKMI = 15,
            ILANSEKLI = 16,
            DIGERACIKLAMA = 17,
            YATIRIMPROJENO = 18,
            DOKUMANSATISBEDELI = 19,
            FIYATFARKIDURUM = 20,
            IHKOMKURULACAK = 21,
            DOKUMANPOSTAILEGONDER = 22,
            ONAYMAKAMI = 23,
            ONAYBELGESIRANO = 24,
            ONAYBELGETARIH = 25,
            ONAYASUNANAD = 26,
            ONAYASUNANUNVAN = 27,
            SUNUMTARIHI = 28,
            ONAYLAYANAD = 29,
            ONAYLAYANUNVAN = 30,
            ONAYTARIHI = 31,
            SOZLESMEYAPILACAK=32

        }
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ISLEMTANIM", "ISNITELIK", "ISMIKTAR", "MUHAMMENBEDEL", "MUHAMMENBEDELPARABIRIMI", "GECICITEMINATMIKTAR", "AVANSVARMI", "AVANSSARTLARI", "ONILANUYGULANSIN", "IHALETURU", "IHALEMADDESI", "ILANVERILECEKMI", "ILANSEKLI", "DIGERACIKLAMA", "YATIRIMPROJENO", "DOKUMANSATISBEDELI", "FIYATFARKIDURUM", "IHKOMKURULACAK", "DOKUMANPOSTAILEGONDER", "ONAYMAKAMI", "ONAYBELGESIRANO", "ONAYBELGETARIH", "ONAYASUNANAD", "ONAYASUNANUNVAN", "SUNUMTARIHI", "ONAYLAYANAD", "ONAYLAYANUNVAN", "ONAYTARIHI", "SOZLESMEYAPILACAK" };

        public IDataReader Listele(OnayForm bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(OnayForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.isTanimi) + ",";
            sql += VeriErisim.DegerYap(bilgi.isNiteligi) + ",";
            sql += VeriErisim.DegerYap(bilgi.isMiktari) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhammenBedel) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhammenBedelPBKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.geciciTeminatMiktari) + ",";
            sql += VeriErisim.DegerYap(bilgi.avansVarmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.avansSartlari) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilanVerilecekmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleUsulu) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleMaddesi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilanVerilecekmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilanSekli) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.projeNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.dokumanBedeli) + ",";
            sql += VeriErisim.DegerYap(bilgi.fiyatFarkiVarmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.komisyonVarmi) + ",";
            sql += VeriErisim.DegerYap(bilgi.dokumanPostaSatisVarMi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleMakami) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrakSayi) + ",";
            sql += VeriErisim.DegerYap(bilgi.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanGercekGorevli) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanGercekGorevliUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanOnayaSunumTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanIhaleYetkili) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanIhaleYetkiliUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onIlanOnaylamaTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeYapilacak) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(OnayForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public decimal MuhammenBedel(int yil, int muhasebe, string birim, string surecno)
        {
            string sql = "";
            sql = " SELECT SUM(" + TABMuhammenBedel.ENUMAlanlar.TUTAR + ")";
            sql += " FROM " + TABMuhammenBedel.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)TABMuhammenBedel.ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)TABMuhammenBedel.ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)TABMuhammenBedel.ENUMAlanlar.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)TABMuhammenBedel.ENUMAlanlar.SURECNO], "=", surecno));
            return veriErisim.TekKolonSQLDecimal(sql);
        }

    }
}

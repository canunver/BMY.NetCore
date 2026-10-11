using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABTahsilat : TABOrtak
    {
        public TABTahsilat(VeriErisim veriErisim) : base(veriErisim) { }
        public static string TABLOADTAHSILATBILGI = "GELTAHSILATBILGI";
        public enum ENUMAlanlarTahsilatBilgi
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            FIRMAKODU = 4,
            TASINMAZKODU = 5,

            IHALETARIHI = 6,
            KOMKARARNO = 7,
            KOMKARARTARIHI = 8,
            SOZLESMENO = 9,
            SOZLESMETARIHI = 10,
            SOZLESMEBEDELI = 11,
            ISSURESI = 12,
            ISSURESITURU = 13,
            TESLIMTARIHI = 14,
            BITISTARIHI = 15,
            TEKLIFTIPI = 16


        }
        static string[] ALANLARTAHSILATBILGI = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU", "TASINMAZKODU", "IHALETARIHI", "KOMKARARNO", "KOMKARARTARIHI", "SOZLESMENO", "SOZLESMETARIHI", "SOZLESMEBEDELI", "ISSURESI", "ISSURESITURU", "TESLIMTARIHI", "BITISTARIHI", "TEKLIFTIPI" };


        public static string TABLOADTAHSILATODEMEPLAN = "GELTAHSILATPLAN";
        public enum ENUMAlanlarTahsilatPlan
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            FIRMAKODU = 4,
            TASINMAZKODU = 5,
            TAKSITNO = 6,
            ODEMETARIHI = 7,
            ODEMETUTARI = 8,
            FIYATFARKIORANI = 9,
            FIYATFARKI = 10,

            TEKLIFTIPI = 11

        }
        public static string[] ALANLARTAHSILATPLAN = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU", "TASINMAZKODU", "TAKSITNO", "ODEMETARIHI", "ODEMETUTARI", "FIYATFARKIORANI", "FIYATFARKI", "TEKLIFTIPI" };

        public Sonuc BilgiKaydet(Tahsilat bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADTAHSILATBILGI + " (" + VeriErisim.TumAlanlar("", ALANLARTAHSILATBILGI) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.firmaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleKararNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleKararTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.sozlesmeBedeli) + ",";
            sql += VeriErisim.DegerYap(bilgi.isinSuresi) + ",";
            sql += VeriErisim.DegerYap(bilgi.isinSuresiTuru) + ",";
            sql += VeriErisim.DegerYap(bilgi.teslimTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.bitimTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.tekliftipi) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc BilgiSil(Tahsilat bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADTAHSILATBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.FIRMAKODU], "=", bilgi.firma.firmaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader BilgiListele(Tahsilat bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARTAHSILATBILGI) + " FROM " + TABLOADTAHSILATBILGI;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.BIRIMKOD], "=", bilgi.birim));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.FIRMAKODU], "=", bilgi.firma.firmaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            return veriErisim.DataReaderDondur(sql);
        }

        public bool BilgiKaydiVarmi(Tahsilat bilgi, bool komkararsildengeldi = false)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOADTAHSILATBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.SURECNO], "=", bilgi.surecNo));
            if (!komkararsildengeldi)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.FIRMAKODU], "=", bilgi.firma.firmaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            kayitvar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitvar;
        }

        public Sonuc BilgiGuncelle(Tahsilat bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADTAHSILATBILGI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.YIL], bilgi.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.MUHASEBEKOD], bilgi.muhasebe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.BIRIMKOD], bilgi.birim) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.SURECNO], bilgi.surecNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.FIRMAKODU], bilgi.firma.firmaNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.TASINMAZKODU], bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.IHALETARIHI], bilgi.ihaleTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.KOMKARARNO], bilgi.ihaleKararNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.KOMKARARTARIHI], bilgi.ihaleKararTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.SOZLESMENO], bilgi.sozlesmeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.SOZLESMETARIHI], bilgi.sozlesmeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.SOZLESMEBEDELI], bilgi.sozlesmeBedeli) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.ISSURESI], bilgi.isinSuresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.ISSURESITURU], bilgi.isinSuresiTuru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.TESLIMTARIHI], bilgi.teslimTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.BITISTARIHI], bilgi.bitimTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.TEKLIFTIPI], bilgi.tekliftipi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.FIRMAKODU], "=", bilgi.firma.firmaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATBILGI[(int)ENUMAlanlarTahsilatBilgi.TEKLIFTIPI], "=", bilgi.tekliftipi));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }


        public Sonuc TaksitKaydet(TasinmazOdemePlan bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADTAHSILATODEMEPLAN + " (" + VeriErisim.TumAlanlar("", ALANLARTAHSILATPLAN) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.firmaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeTutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.fiyatFarkiOrani) + ",";
            sql += VeriErisim.DegerYap(bilgi.iscilikFiyatFarki) + ",";
            //sql += VeriErisim.DegerYap(bilgi.prmakbuzno) + ",";
            //sql += VeriErisim.DegerYap(bilgi.makbuzno) + ",";
            //sql += VeriErisim.DegerYap(bilgi.prmmuhasebefisno) + ",";
            //sql += VeriErisim.DegerYap(bilgi.muhasebefisno) + ",";
            sql += VeriErisim.DegerYap(bilgi.tekliftipi) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public Sonuc TaksitSil(int yil, int muhasebe, string birim, string surecNo, int sirano, int firmakod, string tasinmazkod, int tekliftipi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADTAHSILATODEMEPLAN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.SURECNO], "=", surecNo));
            if (sirano > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.TAKSITNO], "=", sirano));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.FIRMAKODU], "=", firmakod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.TASINMAZKODU], "=", tasinmazkod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader TaksiltListele(int yil, int muhasebe, string birim, string surecNo, int firmano, string tasinmazkodu)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARTAHSILATPLAN) + " FROM " + TABLOADTAHSILATODEMEPLAN;
            sql += " WHERE 1=1 ";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.FIRMAKODU], "=", firmano));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.TASINMAZKODU], "=", tasinmazkodu));
            return veriErisim.DataReaderDondur(sql);
        }

        public bool TaksitKaydiVarmi(TasinmazOdemePlan bilgi)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOADTAHSILATODEMEPLAN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.FIRMAKODU], "=", bilgi.firma.firmaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.TAKSITNO], "=", bilgi.siraNo));
            kayitvar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitvar;
        }

        public Sonuc TaksitGuncelle(TasinmazOdemePlan bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADTAHSILATODEMEPLAN + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.ODEMETARIHI], bilgi.odemeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.ODEMETUTARI], bilgi.odemeTutari) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.FIYATFARKIORANI], bilgi.fiyatFarkiOrani) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.FIYATFARKI], bilgi.iscilikFiyatFarki) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.MAKBUZNO], bilgi.prmakbuzno) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.MAKBUZNO], bilgi.makbuzno) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.MUHASEBEFISNO], bilgi.prmmuhasebefisno) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.MUHASEBEFISNO], bilgi.muhasebefisno) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.TEKLIFTIPI], bilgi.tekliftipi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.FIRMAKODU], "=", bilgi.firma.firmaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTAHSILATPLAN[(int)ENUMAlanlarTahsilatPlan.TAKSITNO], "=", bilgi.siraNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }


        //public Sonuc TaksitMakbuzNoGuncelle(HakedisOdemePlan bilgi)
        //{
        //    string sql = "";
        //    Sonuc sonuc = new Sonuc();
        //    sql = "UPDATE " + TABLOADHAKEDISODEMEPLAN + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.MAKBUZNO], bilgi.makbuzno);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.YIL], "=", bilgi.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.MUHASEBEKOD], "=", bilgi.muhasebe));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.BIRIMKOD], "=", bilgi.birim));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.SURECNO], "=", bilgi.surecNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.FIRMAKODU], "=", bilgi.firma.firmaNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.kod));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARHAKEDISPLAN[(int)ENUMAlanlarHakedisPlan.TAKSITNO], "=", bilgi.siraNo));
        //    sonuc = KomutCalistir(sql, 1);
        //    return sonuc;
        //}

    }
}

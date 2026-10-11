using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;
namespace TNS.GEL.IsKurallari.Veri
{
    public class TABIhaleTeklif : TABOrtak
    {
        public TABIhaleTeklif(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADIHLTEKLIFTASINMAZ = "GELIHALETEKLIKTASINMAZ";
        public static string TABLOADIHLTEKLIFFIRMA = "GELIHALETEKLIFFIRMA";
        public static string TABLOADIHLTEKLIF = "GELIHALETEKLIF";

        public enum ENUMAlanlarIHLTeklifTasinmaz
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            TASINMAZKODU = 4,
            KIRALAMASURESI = 5,
            KIRALAMATIPI = 6,

        }
        public static string[] ALANLARIHLTEKLIFTASINMAZ = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TASINMAZKODU", "KIRALAMASURESI", "KIRALAMATIPI" };

        public enum ENUMAlanlarIHLTeklifFirma
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            FIRMAKODU = 4,

        }
        public static string[] ALANLARIHLTEKLIFFIRMA = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU" };

        public enum ENUMAlanlarIHLTeklif
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            FIRMAKODU = 4,
            TASINMAZKODU = 5,
            TEKLIFNO = 6,
            TEKLIFTUTARI = 7,
            TEKLIFTIPI = 8,
            TEKLIFTARIHI = 9
        }
        public static string[] ALANLARIHLTEKLIF = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU", "TASINMAZKODU", "TEKLIFNO", "TEKLIFTUTARI", "TEKLIFTIPI", "TEKLIFTARIHI" };

        public Sonuc TeklifFirmaKaydet(TNS.HRC.TeklifFirma bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            sql = "INSERT INTO " + TABLOADIHLTEKLIFFIRMA + " (" + VeriErisim.TumAlanlar("", ALANLARIHLTEKLIFFIRMA) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firmaNo) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
        public Sonuc TeklifFirmaSil(TNS.HRC.TeklifFirma bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADIHLTEKLIFFIRMA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.FIRMAKODU], "=", bilgi.firmaNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }
        public IDataReader TeklifFirmaListele(TNS.HRC.TeklifFirma bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARIHLTEKLIFFIRMA);
            sql += " FROM " + TABLOADIHLTEKLIFFIRMA;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.BIRIMKOD], "=", bilgi.birim));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.SURECNO], "=", bilgi.surecNo));
            if (bilgi.firmaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.FIRMAKODU], "=", bilgi.firmaNo));
            sql += " ORDER BY " + ENUMAlanlarIHLTeklifFirma.FIRMAKODU;
            return veriErisim.DataReaderDondur(sql);
        }
        public bool TeklifFirmaVarmi(TNS.HRC.TeklifFirma bilgi)
        {
            string sql = "";
            sql = "SELECT COUNT(*) ";
            sql += " FROM " + TABLOADIHLTEKLIFFIRMA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFFIRMA[(int)ENUMAlanlarIHLTeklifFirma.FIRMAKODU], "=", bilgi.firmaNo));
            return veriErisim.TekKolonSQLInt(sql) > 0;
        }


        public bool TasinmazaAitTeklifVarmi(TeklifTasinmaz bilgi)
        {
            string sql = "";
            sql = "SELECT COUNT(*)  ";
            sql += " FROM " + TABLOADIHLTEKLIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFTIPI], "=", bilgi.tekliftipi));
            //if (bilgi.tasinmazBilgisi.kod > 0)
            //  sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TASINMAZKODU], "=", bilgi.tasinmazBilgisi.kod));
            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
        public bool FirmayaAitTeklifVarmi(TeklifFirma bilgi)
        {
            string sql = "";
            sql = "SELECT COUNT(*)  ";
            sql += " FROM " + TABLOADIHLTEKLIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.FIRMAKODU], "=", bilgi.firmaNo));
            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
        public IDataReader TasinmazKiralamaSuresiTipiGetir(int yil, int muhasebe, string birim, string surecno, string tasinmazkodu)
        {
            string sql = "";
            sql = "SELECT " + ENUMAlanlarIHLTeklifTasinmaz.KIRALAMASURESI + ',' + ENUMAlanlarIHLTeklifTasinmaz.KIRALAMATIPI;
            sql += " FROM " + TABLOADIHLTEKLIFTASINMAZ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.SURECNO], "=", surecno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.TASINMAZKODU], "=", tasinmazkodu));
            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc TeklifTasinmazKaydet(TeklifTasinmaz bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            sql = "INSERT INTO " + TABLOADIHLTEKLIFTASINMAZ + " (" + VeriErisim.TumAlanlar("", ALANLARIHLTEKLIFTASINMAZ) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmazBilgisi.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.kiralanmaSuresi) + ",";
            sql += VeriErisim.DegerYap(bilgi.kiralanmaTipi) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
        public Sonuc TeklifTasinmazSil(TeklifTasinmaz bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADIHLTEKLIFTASINMAZ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.TASINMAZKODU], "=", bilgi.tasinmazBilgisi.tasinmazSistemKodu));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
        public IDataReader TeklifTasinmazListele(TeklifTasinmaz bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARIHLTEKLIFTASINMAZ);
            sql += " FROM " + TABLOADIHLTEKLIFTASINMAZ;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.BIRIMKOD], "=", bilgi.birim));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.SURECNO], "=", bilgi.surecNo));
            if (!string.IsNullOrEmpty(bilgi.tasinmazBilgisi.tasinmazSistemKodu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.TASINMAZKODU], "=", bilgi.tasinmazBilgisi.tasinmazSistemKodu));
            if (bilgi.kiralanmaTipi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.KIRALAMATIPI], "=", bilgi.kiralanmaTipi));
            return veriErisim.DataReaderDondur(sql);
        }
        public bool TeklifTasinmazVarmi(TeklifTasinmaz bilgi)
        {
            string sql = "";
            sql = "SELECT COUNT(*) ";
            sql += " FROM " + TABLOADIHLTEKLIFTASINMAZ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.TASINMAZKODU], "=", bilgi.tasinmazBilgisi.tasinmazSistemKodu));
            return veriErisim.TekKolonSQLInt(sql) > 0;
        }
        public Sonuc TeklifTasinmazGuncelle(TeklifTasinmaz bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADIHLTEKLIFTASINMAZ + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.KIRALAMASURESI], bilgi.kiralanmaSuresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.KIRALAMATIPI], bilgi.kiralanmaTipi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIFTASINMAZ[(int)ENUMAlanlarIHLTeklifTasinmaz.TASINMAZKODU], "=", bilgi.tasinmazBilgisi.tasinmazSistemKodu));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }



        public Sonuc TeklifKaydet(Teklif bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            sql = "INSERT INTO " + TABLOADIHLTEKLIF + " (" + VeriErisim.TumAlanlar("", ALANLARIHLTEKLIF) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.firmaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.teklifno) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarihi) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
        public Sonuc TeklifSil(Teklif bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADIHLTEKLIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.SURECNO], "=", bilgi.surecNo));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.FIRMAKODU], "=", bilgi.firma.firmaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFNO], "=", bilgi.teklifno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFTIPI], "=", bilgi.tip));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public bool TeklifKayitVarmi(Teklif bilgi)
        {
            string sql = "";
            sql = "SELECT count(*) FROM " + TABLOADIHLTEKLIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.SURECNO], "=", bilgi.surecNo));
            if (bilgi.firma.firmaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.FIRMAKODU], "=", bilgi.firma.firmaNo));
            if (!string.IsNullOrEmpty( bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu ))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            if (bilgi.teklifno > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFNO], "=", bilgi.teklifno));
            if (bilgi.tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFTIPI], "=", bilgi.tip));
            return veriErisim.TekKolonSQLInt(sql) >= 1;

        }

        public IDataReader TeklifKayitVarmi(int yil,int muhasebe,string birim,string surecno)
        {
            string sql = "";
            sql = "SELECT " + ENUMAlanlarIHLTeklif.TEKLIFTIPI +  " FROM " + TABLOADIHLTEKLIF;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.SURECNO], "=", surecno));
            sql += " GROUP BY " + ENUMAlanlarIHLTeklif.TEKLIFTIPI;
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc TeklifKayitGuncelle(Teklif bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADIHLTEKLIF + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFTUTARI], bilgi.tutari) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFTARIHI], bilgi.tarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.SURECNO], "=", bilgi.surecNo));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.FIRMAKODU], "=", bilgi.firma.firmaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFNO], "=", bilgi.teklifno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFTIPI], "=", bilgi.tip));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader TeklifListele(Teklif bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARIHLTEKLIF);
            sql += " FROM " + TABLOADIHLTEKLIF;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.BIRIMKOD], "=", bilgi.birim));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.SURECNO], "=", bilgi.surecNo));
            if (!string.IsNullOrEmpty( bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu ))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            if (bilgi.firma.firmaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.FIRMAKODU], "=", bilgi.firma.firmaNo));
            if (bilgi.teklifno > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFNO], "=", bilgi.teklifno));
            if (bilgi.tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFTIPI], "=", bilgi.tip));
            sql += " ORDER BY " + ENUMAlanlarIHLTeklif.FIRMAKODU + "," + ENUMAlanlarIHLTeklif.TEKLIFNO;
            return veriErisim.DataReaderDondur(sql);
        }


        public IDataReader SonTeklifGetir(Teklif bilgi)
        {

            string sql = "";
            sql = " SELECT TOP(1) " + ENUMAlanlarIHLTeklif.TEKLIFNO + ", " + ENUMAlanlarIHLTeklif.TEKLIFTUTARI;
            sql += " FROM " + TABLOADIHLTEKLIF;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TEKLIFTIPI], "=", bilgi.tip));
            if (bilgi.firma.firmaBilgisi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.FIRMAKODU], "=", bilgi.firma.firmaBilgisi.kod));
            if (!string.IsNullOrEmpty( bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu ))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARIHLTEKLIF[(int)ENUMAlanlarIHLTeklif.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            sql += " GROUP BY " + ENUMAlanlarIHLTeklif.TEKLIFNO + "," + ENUMAlanlarIHLTeklif.TEKLIFTUTARI;
            sql += " ORDER BY " + ENUMAlanlarIHLTeklif.TEKLIFNO + " DESC";
            return veriErisim.DataReaderDondur(sql);

        }



    }
}

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    public class TABYATKrediDetay : TABOrtak
    {
        public TABYATKrediDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        static string TABLOAD = "YATKREDIDETAY";
        /*
         * 
         CREATE TABLE YATKREDIDETAY(
            BUTCEYIL INT,
            SENARYOKOD VARCHAR(20),
            KAYNAKNO INT,
            YIL INT,
            ANLASMATUTARI MONEY,
            KULLANILANTUTAR MONEY,
            KULLANILACAKTUTAR MONEY,
            ANLASMATUTARIIC MONEY,
            KULLANILANTUTARIC MONEY,
            KULLANILACAKTUTARIC MONEY, PRIMARY KEY (BUTCEYIL, SENARYOKOD, KAYNAKNO , YIL))
         * 
         * */
        enum ENUMAlanlar : int
        {
            BUTCEYIL = 0,
            SENARYOKOD = 1,
            KAYNAKNO = 2,
            YIL = 3,
            ANLASMATUTARI = 4,
            KULLANILANTUTAR = 5,
            KULLANILACAKTUTAR = 6,
            ANLASMATUTARIIC = 7,
            KULLANILANTUTARIC = 8,
            KULLANILACAKTUTARIC = 9
        }

        static string[] ALANLAR = { "BUTCEYIL", "SENARYOKOD", "KAYNAKNO", "YIL", "ANLASMATUTARI", "KULLANILANTUTAR", "KULLANILACAKTUTAR", "ANLASMATUTARIIC", "KULLANILANTUTARIC", "KULLANILACAKTUTARIC" };

        public Sonuc Kaydet(KrediDetay krediDetay)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(krediDetay.butceYil) + ",";
            sql += VeriErisim.DegerYap(krediDetay.senaryoKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(krediDetay.kaynakNo) + ",";
            sql += VeriErisim.DegerYap(krediDetay.yil) + ",";
            sql += VeriErisim.DegerYap(krediDetay.anlasmaTutari) + ",";
            sql += VeriErisim.DegerYap(krediDetay.kullanilanTutar) + ",";
            sql += VeriErisim.DegerYap(krediDetay.kullanilacakTutar) + ",";
            sql += VeriErisim.DegerYap(krediDetay.anlasmaTutariIc) + ",";
            sql += VeriErisim.DegerYap(krediDetay.kullanilanTutarIc) + ",";
            sql += VeriErisim.DegerYap(krediDetay.kullanilacakTutarIc) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int butceYil, string senaryoKod, int kaynakNo)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BUTCEYIL], "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(int butceYil, string senaryoKod, int kaynakNo)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BUTCEYIL], "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KaynakListele(int butceYil, string senaryoKod, int projeKodu, int altProjeKodu, ENUMButceKaynakTur[] kaynakTurler)
        {
            string sql = "SELECT DISTINCT K.KAYNAKNO FROM  YATKREDIDETAY ";
            sql += "INNER JOIN YATBUTCEKAYNAK K ON K.YIL=YATKREDIDETAY.BUTCEYIL AND K.SENARYOKOD=YATKREDIDETAY.SENARYOKOD AND K.KAYNAKNO=YATKREDIDETAY.KAYNAKNO ";
            sql += "INNER JOIN YATALTPROJE AP ON K.YIL=AP.YIL AND K.SENARYOKOD=AP.SENARYOKOD AND K.ALTPROJENO=AP.ALTPROJENO ";
            sql += "INNER JOIN YATPROJE P ON P.YIL=AP.YIL AND P.SENARYOKOD=AP.SENARYOKOD AND P.PROJENO=AP.PROJENO ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("YATKREDIDETAY", ALANLAR[(int)ENUMAlanlar.BUTCEYIL], "=", butceYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATKREDIDETAY", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (kaynakTurler != null && kaynakTurler.Length > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "K.KAYNAKTUR IN (" + ListeYap(kaynakTurler) + ")");
            }

            if (projeKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", "PROJENO", "=", projeKodu));
            if (altProjeKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AP", "ALTPROJENO", "=", altProjeKodu));
            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeYap(ENUMButceKaynakTur[] kaynakTurler)
        {
            string retVal = "";
            for (int i = 0; i < kaynakTurler.Length; i++)
            {
                if (i > 0) retVal += ", ";
                retVal += ((int)kaynakTurler[i]).ToString();
            }
            return retVal;
        }
    }
}
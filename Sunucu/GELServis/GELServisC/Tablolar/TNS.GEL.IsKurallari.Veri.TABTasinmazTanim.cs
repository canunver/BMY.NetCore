using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;
namespace TNS.GEL.IsKurallari.Veri
{
    public class TABTasinmazTanim : TABOrtak
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTasinmazTanim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "GELTASINMAZ";

        public enum ENUMAlanlar
        {

            TASINMAZKOD = 0,
            YIL = 1,
            BOLGE = 2,
            IL = 3,
            ILCE = 4,
            MAHALLE = 5,
            KOY = 6,
            KISAKOD = 7,
            AD = 8,
            CINS = 9,
            PAFTANO = 10,
            ADANO = 11,
            PARSELNO = 12,
            CILTNO = 13,
            CILTSAYFANO = 14,
            CILTSIRANO = 15,
            YUZOLCUMUMIKTARI = 16,
            YUZOLCUMUORANI = 17,
            EDINMESEKLI = 18,
            EDINMETARIHI = 19,
            EDINMESURESI = 20,
            TESCILTARIHI = 21,
            KAYITTANCIKISNEDENI = 22,
            KAYITTANCIKISTARIHI = 23,
            ARSIVTARIHI = 24,
            ARSIVNO = 25,
            ACIKLAMALAR = 26,
            ENLEM = 27,
            BOYLAM = 28,
            MEVKI = 29,
            MALIK = 30,
            BUTCE = 31,
            DAIRESAYISI = 32,
            DAIREALANI = 33,
            DURUM = 34,
            KAYITTARIHI = 35,
            ISLEMTARIHI = 36,
            KULLANICI = 37
        }

        public static string[] ALANLAR = { 
                                    "TASINMAZKOD",
                                    "YIL",
                                    "BOLGE",
                                    "IL",
                                    "ILCE",
                                    "MAHALLE",
                                    "KOY",
                                    "KISAKOD",
                                    "AD",
                                    "CINS",
                                    "PAFTANO",
                                    "ADANO",
                                    "PARSELNO",
                                    "CILTNO",
                                    "CILTSAYFANO",
                                    "CILTSIRANO",
                                    "YUZOLCUMUMIKTARI",
                                    "YUZOLCUMUORANI",
                                    "EDINMESEKLI",
                                    "EDINMETARIHI",
                                    "EDINMESURESI",
                                    "TESCILTARIHI",
                                    "KAYITTANCIKISNEDENI",
                                    "KAYITTANCIKISTARIHI",
                                    "ARSIVTARIHI",
                                    "ARSIVNO",
                                    "ACIKLAMALAR",
                                    "ENLEM",
                                    "BOYLAM",
                                    "MEVKI",
                                    "MALIK",
                                    "BUTCE",
                                    "DAIRESAYISI",
                                    "DAIREALANI",
                                    "DURUM",
                                    "KAYITTARIHI",
                                    "ISLEMTARIHI",
                                    "KULLANICI"
                                     };

        public Sonuc Kaydet(VeriErisim ve, Tasinmaz bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.bolge.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilceKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.mahalle) + ",";
            sql += VeriErisim.DegerYap(bilgi.koy) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmazKisaKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmazAd) + ",";
            sql += VeriErisim.DegerYap(bilgi.cins.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.paftano) + ",";
            sql += VeriErisim.DegerYap(bilgi.adano) + ",";
            sql += VeriErisim.DegerYap(bilgi.parselno) + ",";
            sql += VeriErisim.DegerYap(bilgi.ciltno) + ",";
            sql += VeriErisim.DegerYap(bilgi.ciltSayfaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ciltSiraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.yuzOlcumuMiktari) + ",";
            sql += VeriErisim.DegerYap(bilgi.yuzOlcumuOrani) + ",";
            sql += VeriErisim.DegerYap(bilgi.edinmeSekli) + ",";
            sql += VeriErisim.DegerYap(bilgi.edinmeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.edinmeSuresi) + ",";
            sql += VeriErisim.DegerYap(bilgi.tescilTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayittanCikisNedeni) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayittanCikisTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.arsivTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.arsivNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.enlem) + ",";
            sql += VeriErisim.DegerYap(bilgi.boylam) + ",";
            sql += VeriErisim.DegerYap(bilgi.mevki) + ",";
            sql += VeriErisim.DegerYap(bilgi.malik) + ",";
            sql += VeriErisim.DegerYap(bilgi.butceTur) + ",";
            sql += VeriErisim.DegerYap(bilgi.daireSayisi) + ",";
            sql += VeriErisim.DegerYap(bilgi.daireAlani) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayitTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.kullanici) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(VeriErisim ve, string tasinmazKod, int yil)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE  " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMKayitDurum.SILINDI);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader Listele(Tasinmaz tasinmaz)
        {
            string sql = "";
            sql = " SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",CINS.AD AS CINSAD";
            sql += ", " + TABTasinmazBolge.TABLOAD + "." + TABTasinmazBolge.ALANLAR[(int)TABTasinmazBolge.ENUMAlanlar.AD];
            sql += " FROM " + TABLOAD;
            sql += " LEFT JOIN " + TABTasinmazCins.TABLOAD + " AS CINS ";
            sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.CINS] + "= CINS." + TABTasinmazCins.ALANLAR[(int)TABTasinmazCins.ENUMAlanlar.KOD];
            sql += " LEFT JOIN " + TABTasinmazBolge.TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BOLGE] + "=" + TABTasinmazBolge.TABLOAD + "." + TABTasinmazBolge.ALANLAR[(int)TABTasinmazBolge.ENUMAlanlar.KOD];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMKayitDurum.SILINDI));
            if (tasinmaz.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tasinmaz.yil));
            if (!string.IsNullOrEmpty(tasinmaz.tasinmazSistemKodu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", tasinmaz.tasinmazSistemKodu));
            if (tasinmaz.tasinmazAd != null && tasinmaz.tasinmazAd.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", tasinmaz.tasinmazAd + "%"));
            if (tasinmaz.ilKod != null && tasinmaz.ilKod.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IL], "=", tasinmaz.ilKod));
            if (tasinmaz.ilceKod != null && tasinmaz.ilceKod.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCE], "=", tasinmaz.ilceKod));
            if (tasinmaz.mahalle != null && tasinmaz.mahalle.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MAHALLE], "LIKE", tasinmaz.mahalle + "%"));
            if (tasinmaz.koy != null && tasinmaz.koy.Trim() != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOY], "LIKE", tasinmaz.koy + "%"));
             
            if (tasinmaz.cins != null && !string.IsNullOrEmpty(tasinmaz.cins.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CINS], "=", tasinmaz.cins.kod.Replace(".", "")));

            if (tasinmaz.bolge.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BOLGE], "=", tasinmaz.bolge.kod));

            if (tasinmaz.butceTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BUTCE], "=", tasinmaz.butceTur));
            return veriErisim.DataReaderDondur(sql);
        }

        public string TasinmazAdiGetir(string tasinmazKod, int yil)
        {
            string sql = "";
            sql = "SELECT " + ENUMAlanlar.AD;
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            return veriErisim.TekKolonSQLStr(sql);

        }

        public bool KayitVarmi(string tasinmazKod, int yil)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            kayitvar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitvar;
        }

        public Sonuc Guncelle(Tasinmaz bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOLGE], bilgi.bolge.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IL], bilgi.ilKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCE], bilgi.ilceKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MAHALLE], bilgi.mahalle) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOY], bilgi.koy) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISAKOD], bilgi.tasinmazKisaKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.tasinmazAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CINS], bilgi.cins.kod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PAFTANO], bilgi.paftano) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADANO], bilgi.adano) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARSELNO], bilgi.parselno) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CILTNO], bilgi.ciltno) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CILTSAYFANO], bilgi.ciltSayfaNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CILTSIRANO], bilgi.ciltSiraNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YUZOLCUMUMIKTARI], bilgi.yuzOlcumuMiktari) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YUZOLCUMUORANI], bilgi.yuzOlcumuOrani) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EDINMESEKLI], bilgi.edinmeSekli) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EDINMETARIHI], bilgi.edinmeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EDINMESURESI], bilgi.edinmeSuresi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESCILTARIHI], bilgi.tescilTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITTANCIKISNEDENI], bilgi.kayittanCikisNedeni) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITTANCIKISTARIHI], bilgi.kayittanCikisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARSIVTARIHI], bilgi.arsivTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARSIVNO], bilgi.arsivNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMALAR], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ENLEM], bilgi.enlem) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOYLAM], bilgi.boylam) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEVKI], bilgi.mevki) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALIK], bilgi.malik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCE], bilgi.butceTur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAIRESAYISI], bilgi.daireSayisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DAIREALANI], bilgi.daireAlani) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], bilgi.islemTarihi) +",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICI], bilgi.kullanici) ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", bilgi.tasinmazSistemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public int TasinmazSiraNoAl(int yil)
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.TASINMAZKOD + "),0)+1 AS MAXSIRANO ";
            sql += " FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            return veriErisim.TekKolonSQLInt(sql);
        }



        public IDataReader IcmalListele(TasinmazIcmal icmal)
        {
            string sql = "";
            sql = " SELECT CINS.KOD,";
            sql += "CINS." + TABTasinmazCins.ALANLAR[(int)TABTasinmazCins.ENUMAlanlar.AD];
            sql += ", GECENYIL.ADET GECENYILADET,GECENYIL.YUZOLCUMMIKTAR GECENYILYUZOLCUMMIKTAR, ";
            sql += "  YILICI.ADET YILICIADET ,YILICI.YUZOLCUMMIKTAR  YILICIYUZOLCUMMIKTAR,YILICICIKAN.ADET YILICICIKANADET, ";
            sql += "  YILICICIKAN.YUZOLCUMMIKTAR YILICICIKANYUZOLCUMMIKTAR ";
            sql += " FROM ( ";
            sql += "        SELECT " + TABTasinmazCins.ALANLAR[(int)TABTasinmazCins.ENUMAlanlar.KOD] + "," + TABTasinmazCins.ALANLAR[(int)TABTasinmazCins.ENUMAlanlar.AD] + " FROM " + TABTasinmazCins.TABLOAD;
            sql += "      ) CINS LEFT JOIN ";
            sql += "	  (	 SELECT " + ALANLAR[(int)ENUMAlanlar.CINS] + ",COUNT(*) AS ADET,SUM(" + ALANLAR[(int)ENUMAlanlar.YUZOLCUMUMIKTARI] + ") AS YUZOLCUMMIKTAR  ";
            sql += "		 FROM  " + TABLOAD;
            if (icmal.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", icmal.yil));
            sql += "		 GROUP BY " + ALANLAR[(int)ENUMAlanlar.CINS];
            sql += "	 )   YILICI ON CINS.KOD= YILICI.CINS LEFT JOIN ";
            sql += "	 ( ";
            sql += "		 SELECT " + ALANLAR[(int)ENUMAlanlar.CINS] + ",COUNT(*) AS ADET,SUM(" + ALANLAR[(int)ENUMAlanlar.YUZOLCUMUMIKTARI] + ") AS YUZOLCUMMIKTAR  ";
            sql += "		 FROM " + TABLOAD;
            if (icmal.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", icmal.yil - 1));

            sql += "		 GROUP BY " + ALANLAR[(int)ENUMAlanlar.CINS];
            sql += "	 )   GECENYIL  ON CINS.KOD =GECENYIL.CINS LEFT OUTER JOIN  ";
            sql += "	 ( ";
            sql += "		 SELECT " + ALANLAR[(int)ENUMAlanlar.CINS] + ",COUNT(*) AS ADET,SUM(" + ALANLAR[(int)ENUMAlanlar.YUZOLCUMUMIKTARI] + ") AS YUZOLCUMMIKTAR  ";
            sql += "		 FROM  " + TABLOAD;
            if (icmal.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", icmal.yil));
            sql += "		 GROUP BY " + ALANLAR[(int)ENUMAlanlar.CINS];
            sql += "	 )  YILICICIKAN ON CINS.KOD=YILICICIKAN.CINS ";
            sql += " WHERE GECENYIL.ADET > 0";
            if (!string.IsNullOrEmpty(icmal.tasinmaz.cins.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("CINS", TABTasinmazCins.ALANLAR[(int)TABTasinmazCins.ENUMAlanlar.KOD], "=", icmal.tasinmaz.cins.kod.Replace(".", "")));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

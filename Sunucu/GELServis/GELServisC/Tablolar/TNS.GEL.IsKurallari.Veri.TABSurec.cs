using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABSurec : TABOrtak
    {
        public TABSurec(VeriErisim veriErisim) : base(veriErisim) { }
        public static string TABLOADSUREC = "GELSUREC";
        public enum ENUMAlanlarSurec
        {

            YIL = 0,
            SURECTARIH = 1,
            SURECNO = 2,
            MUHASEBEKOD = 3,
            BIRIMKOD = 4,
            ISLEMKOD = 5,
            ISINADI = 6,
            ACIKLAMA = 7,
            SURECDURUM = 8
            //TEKLIFTIPI = 9

        }
        public static string[] ALANLARSUREC = { "YIL", "SURECTARIH", "SURECNO", "MUHASEBEKOD", "BIRIMKOD", "ISLEMKOD", "ISINADI", "ACIKLAMA", "SURECDURUM" };

        public static string TABLOADSURECPARCADURUM = "GELSURECDURUM";
        public enum ENUMAlanlarSurecParcaDurum
        {
            YIL = 0,
            SURECNO = 1,
            ISLEMKOD = 2,
            PARCAKOD = 3,
            TARIH = 4,
            KULLANICIAD = 5,
            DURUM = 6
        }
        public static string[] ALANLARSURECPARCADURUM = { "YIL", "SURECNO", "ISLEMKOD", "PARCAKOD", "TARIH", "KULLANICIAD", "DURUM" };


        public static string TABLOADSURECPARCA = "GELSURECPARCA";
        public enum ENUMAlanlarSurecParca
        {
            YIL = 0,
            SURECNO = 1,
            ISLEMKOD = 2,
            PARCAKOD = 3,
            SIRANO = 4,
            IHALEUSUL = 5,
            IHALEMADDE = 6,
            DUZEY = 7,
            ZORUNLU = 8
        }
        public static string[] ALANLARSURECPARCA = { "YIL", "SURECNO", "ISLEMKOD", "PARCAKOD", "SIRANO", "IHALEUSUL", "IHALEMADDE", "DUZEY", "ZORUNLU" };

        public static string TABLOADSURECSIFRE = "GELSURECSIFRE";
        public enum ENUMAlanlarSurecSifre
        {
            YIL = 0,
            SURECNO = 1,
            PARCAKOD = 2,
            SIFRE = 3,
            TCKIMLIK = 4,
            TARIH = 5
        }
        public static string[] ALANLARSURECSIFRE = { "YIL", "SURECNO", "PARCAKOD", "SIFRE", "TCKIMLIK", "TARIH" };

        public Sonuc SurecSifreKaydet(SurecSifre bilgi, Parca parca)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADSURECSIFRE + " (" + VeriErisim.TumAlanlar("", ALANLARSURECSIFRE) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(parca.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.sifre) + ",";
            sql += VeriErisim.DegerYap(bilgi.TCKimlik) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc SurecSifreSil(int yil, string surecno)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE FROM " + TABLOADSURECSIFRE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSURECSIFRE[(int)ENUMAlanlarSurecSifre.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECSIFRE[(int)ENUMAlanlarSurecSifre.SURECNO], "=", surecno));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader SurecSifreListele(int yil, string surecno, int parcakod, string sifre)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSURECSIFRE);
            sql += " FROM " + TABLOADSURECSIFRE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSURECSIFRE[(int)ENUMAlanlarSurecSifre.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECSIFRE[(int)ENUMAlanlarSurecSifre.SURECNO], "=", surecno));
            if (parcakod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECSIFRE[(int)ENUMAlanlarSurecSifre.PARCAKOD], "=", parcakod));
            if (!string.IsNullOrEmpty(sifre))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECSIFRE[(int)ENUMAlanlarSurecSifre.SIFRE], "=", sifre));
            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc ParcaKaydet(Surec bilgi, int sirano, int ihaleusul, int ihalemadde)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADSURECPARCA + " (" + VeriErisim.TumAlanlar("", ALANLARSURECPARCA) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.islem.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.parca.kod) + ",";
            sql += VeriErisim.DegerYap(sirano) + ",";
            sql += VeriErisim.DegerYap(ihaleusul) + ",";
            sql += VeriErisim.DegerYap(ihalemadde) + ",";
            sql += VeriErisim.DegerYap(bilgi.parca.duzey) + ",";
            sql += VeriErisim.DegerYap(bilgi.parca.zorunlu) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader ParcaListele(int yil, string surecno)
        {
            string sql;
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSURECPARCA, ALANLARSURECPARCA) + ",";
            sql += VeriErisim.TumAlanlar(TABParca.TABLOAD, TABParca.ALANLAR);
            sql += " FROM " + TABLOADSURECPARCA;
            sql += " LEFT OUTER JOIN " + TABParca.TABLOAD + " ON ";
            sql += TABLOADSURECPARCA + "." + ALANLARSURECPARCA[(int)ENUMAlanlarSurecParca.PARCAKOD] + "=" + TABParca.TABLOAD + "." + TABParca.ALANLAR[(int)TABParca.ENUMAlanlar.KOD];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSURECPARCA[(int)ENUMAlanlarSurecParca.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCA[(int)ENUMAlanlarSurecParca.SURECNO], "=", surecno));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc ParcaDurumKaydet(SurecParcaDurum bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADSURECPARCADURUM + " (" + VeriErisim.TumAlanlar("", ALANLARSURECPARCADURUM) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.islemKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.parcaKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.kullaniciAd) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader ParcaDurumListele(int yil, string surecno, int islemkod, int parcakod)
        {
            string sql;
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSURECPARCADURUM);
            sql += " FROM " + TABLOADSURECPARCADURUM;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.SURECNO], "=", surecno));

            if (islemkod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.ISLEMKOD], "=", islemkod));
            if (parcakod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.PARCAKOD], "=", parcakod));
            return veriErisim.DataReaderDondur(sql);
        }

        public bool ParcaDurumVarmi(SurecParcaDurum bilgi)
        {
            bool surecVar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOADSURECPARCADURUM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.ISLEMKOD], "=", bilgi.islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.PARCAKOD], "=", bilgi.parcaKod));
            surecVar = veriErisim.TekKolonSQLInt(sql) > 0;
            return surecVar;
        }

        public Sonuc ParcaDurumGuncelle(SurecParcaDurum bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADSURECPARCADURUM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.TARIH], bilgi.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.KULLANICIAD], bilgi.kullaniciAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.ISLEMKOD], "=", bilgi.islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSURECPARCADURUM[(int)ENUMAlanlarSurecParcaDurum.PARCAKOD], "=", bilgi.parcaKod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public Sonuc SurecKaydet(Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADSUREC + " (" + VeriErisim.TumAlanlar("", ALANLARSUREC) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.islem.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.isinAdi) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecDurum);
            //sql += VeriErisim.DegerYap(bilgi.teklifturu) ;
            sql += ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc SurecSil(Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE FROM " + TABLOADSUREC;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO], "=", bilgi.surecNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int SurecNoAl(int yil)
        {
            string sql = "";

            sql = "SELECT  MAX(" + ENUMAlanlarSurec.SURECNO + " ) ";
            sql += " FROM " + TABLOADSUREC;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", yil));
            return veriErisim.TekKolonSQLInt(sql) + 1;

        }

        public bool SurecVarmi(Surec bilgi)
        {
            bool surecVar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOADSUREC;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO], "=", bilgi.surecNo));
            surecVar = veriErisim.TekKolonSQLInt(sql) > 0;
            return surecVar;
        }

        public Sonuc SurecGuncelle(Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADSUREC + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.ISINADI], bilgi.isinAdi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.ACIKLAMA], bilgi.aciklama);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO], "=", bilgi.surecNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public Sonuc SurecDurumGuncelle(Surec bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADSUREC + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], bilgi.surecDurum);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO], "=", bilgi.surecNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }



        //public Sonuc SurecTeklifTuruGuncelle(int yil, string surecno, int teklifturu)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");
        //    string sql = "";
        //    sql = "UPDATE " + TABLOADSUREC + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.TEKLIFTIPI], teklifturu);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO], "=", surecno));
        //    sonuc = KomutCalistir(sql, 1);
        //    return sonuc;
        //}


        public IDataReader SurecListele(Surec bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSUREC);
            sql += " FROM " + TABLOADSUREC;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.YIL], "=", bilgi.yil));
            if (!bilgi.surecTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECTARIH], "=", bilgi.surecTarih));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO], "=", bilgi.surecNo));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.islem.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.ISLEMKOD], "=", bilgi.islem.kod));
            if (bilgi.surecDurum > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "=", bilgi.surecDurum));
            return veriErisim.DataReaderDondur(sql);
        }

        public int TipGetir(Surec bilgi)
        {
            string sql = "";

            sql = "SELECT " + TABIhaleTeklif.ENUMAlanlarIHLTeklif.TEKLIFTIPI;
            sql += " FROM " + TABIhaleTeklif.TABLOADIHLTEKLIF;
            sql += " GROUP BY " + TABIhaleTeklif.ENUMAlanlarIHLTeklif.YIL + "," + TABIhaleTeklif.ENUMAlanlarIHLTeklif.MUHASEBEKOD + "," + TABIhaleTeklif.ENUMAlanlarIHLTeklif.BIRIMKOD + "," + TABIhaleTeklif.ENUMAlanlarIHLTeklif.SURECNO + "," + TABIhaleTeklif.ENUMAlanlarIHLTeklif.TEKLIFTIPI;
            sql = VeriErisim.KosulEkle(sql, "HAVING", VeriErisim.KosulOl("", TABIhaleTeklif.ALANLARIHLTEKLIF[(int)TABIhaleTeklif.ENUMAlanlarIHLTeklif.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABIhaleTeklif.ALANLARIHLTEKLIF[(int)TABIhaleTeklif.ENUMAlanlarIHLTeklif.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABIhaleTeklif.ALANLARIHLTEKLIF[(int)TABIhaleTeklif.ENUMAlanlarIHLTeklif.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABIhaleTeklif.ALANLARIHLTEKLIF[(int)TABIhaleTeklif.ENUMAlanlarIHLTeklif.BIRIMKOD], "=", bilgi.birim));
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}

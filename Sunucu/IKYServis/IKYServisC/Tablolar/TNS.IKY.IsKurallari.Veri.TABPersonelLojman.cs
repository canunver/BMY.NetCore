using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPersonelLojman : TABOrtak
    {
        public TABPersonelLojman(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELLOJMAN";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            PKOD = 1,
            DKOD = 2,
            LojmanTipi = 3,
            BasTarihi = 4,
            BitTarihi = 5,
            Adres = 6,
            KesintiVar = 7,
            TazminatiVar = 8,
            SuParasiKesiliyor = 9,
            KonutYardimTarihi = 10,
            OzelTarih = 11,
            AdresTipi = 12,
            IsTahakkuk = 13,
            TahsisTuru = 14,
            TahsisTip = 15,
            RezervTarihi = 16,
            OncelikDerecesi = 17,
            Aciklama = 18,
        }
        static string[] ALANLAR = { "KOD", "PKOD", "DKOD", "LojmanTipi", "BasTarihi", "BitTarihi", "Adres", "KesintiVar",
                                      "TazminatiVar", "SuParasiKesiliyor", "KonutYardimTarihi", "OzelTarih", "AdresTipi",
                                      "IsTahakkuk", "TahsisTuru", "TahsisTip","RezervTarihi", "OncelikDerecesi", "Aciklama" };

        public IDataReader Listele(PersonelLojmani p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " , lt.ETIKET, d.DaireNo, b.BlokNo, s.SITEADI, b.KOD, b.SKOD, d.DaireMetreKare, s.YAKITKESILECEK";
            sql += " FROM " + TABLOAD;
            sql += " left outer join IKYSOZLUKLOJMANTIPI lt on IKYPERSONELLOJMAN.LojmanTipi = lt.KOD";
            sql += " inner join IKYLOJMANDAIRE d on d.KOD = IKYPERSONELLOJMAN.DKOD";
            sql += " inner join IKYLOJMANBLOK b on b.KOD = d.BKOD";
            sql += " inner join IKYLOJMANSITE s on s.KOD = b.SKOD";

            if (p.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.daireKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DKOD], "=", p.daireKod));

            if ((int)p.tashisTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TahsisTip], "=", (int)p.tashisTip));

            if (p.yil > 0 && p.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", "CONVERT(CHAR(6), BasTarihi, 112) = " + p.yilAy);

            if (p.sadeceOturanlar)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", "BitTarihi is NULL AND TahsisTip=1");
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BasTarihi] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListelePLojmanK(PersonelLojmani p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT s.KOD, s.SITEADI, b.KOD, b.BlokNo, d.KOD, d.DaireNo, p.KOD, p.AD+' '+p.SOYAD, u.AD, l.KOD";
            sql += " FROM IKYPERSONELLOJMAN l ";
            sql += " inner join IKYLOJMANDAIRE d on d.KOD = l.DKOD inner join IKYLOJMANBLOK b on b.KOD = d.BKOD inner join IKYLOJMANSITE s on s.KOD = b.SKOD inner join IKYPERSONEL p on l.PKOD = p.KOD left outer join IKYPERSONELKADRO pk on pk.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU");
            sql += " left outer join IKYUNVAN u on u.KOD = pk.UNVANKOD ";

            string tarAlan = "";
            if (p.tashisTip == ENUMTahsisTipi.Rezervasyon)
            {
                sql += " where l.TahsisTip = " + (int)ENUMTahsisTipi.Rezervasyon;
                tarAlan = "l.RezervTarihi";
            }
            else
            {
                sql += " where l.TahsisTip = " + (int)ENUMTahsisTipi.Tahsis;
                tarAlan = "l.BasTarihi";
            }

            if (p.yilAyDolu == 1)
                sql += " AND CONVERT(CHAR(6), " + "l.BasTarihi" + ", 112) <= " + VeriErisim.DegerYap(p.yilAy) + " and (CONVERT(CHAR(6), l.BitTarihi, 112) >= '" + p.yilAy + "' or l.BitTarihi is null)";
            else
                sql += " AND CONVERT(CHAR(6), " + tarAlan + ", 112) = " + VeriErisim.DegerYap(p.yilAy);

            if (p.siteKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", " s.KOD = " + p.siteKod);

            if (p.blokKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", " b.KOD = " + p.blokKod);

            if (p.yil > 0 && p.ay > 0 && p.komisyonMu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", "CONVERT(CHAR(6), l.BasTarihi, 112) = '" + p.yilAy + "'");

            if (kosul != "")
                sql += " AND " + kosul;

            sql += " order by s.KOD, b.BlokNo, d.DaireNo desc";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleLojmanTuru(LojmanTuru p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT KOD, ETIKET, TOPLAMYIL FROM IKYSOZLUKLOJMANTIPI";

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", "IKYSOZLUKLOJMANTIPI.KOD = " + p.kod);

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(PersonelLojmani p)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            sql += VeriErisim.DegerYap(p.daireKod) + ",";
            if (p.lojmanTipi > 0)
                sql += VeriErisim.DegerYap(p.lojmanTipi) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.basTarihi) + ",";
            sql += VeriErisim.DegerYap(p.bitTarihi) + ",";
            sql += VeriErisim.DegerYap(p.adres) + ",";
            sql += VeriErisim.DegerYap(p.kesintiVar) + ",";
            sql += VeriErisim.DegerYap(p.tazminatiVar) + ",";
            sql += VeriErisim.DegerYap(p.suParasiKesintisi) + ",";
            sql += VeriErisim.DegerYap(p.konutYardimTarihi) + ",";
            sql += VeriErisim.DegerYap(p.ozelTarih) + ",";
            sql += VeriErisim.DegerYap(p.adresTipi) + ",";
            sql += VeriErisim.DegerYap(p.tahakkuk) + ",";
            sql += VeriErisim.DegerYap(p.tahsisTuru) + ",";
            sql += VeriErisim.DegerYap((int)p.tashisTip) + ",";
            sql += VeriErisim.DegerYap(p.rezervTarih) + ",";
            sql += VeriErisim.DegerYap(p.oncelikDerecesi) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(PersonelLojmani p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DKOD], p.daireKod) + ",";
            if (p.lojmanTipi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.LojmanTipi], p.lojmanTipi) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.LojmanTipi], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BasTarihi], p.basTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BitTarihi], p.bitTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.Adres], p.adres) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KesintiVar], p.kesintiVar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TazminatiVar], p.tazminatiVar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SuParasiKesiliyor], p.suParasiKesintisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KonutYardimTarihi], p.konutYardimTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OzelTarih], p.ozelTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AdresTipi], p.adresTipi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IsTahakkuk], p.tahakkuk) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TahsisTuru], p.tahsisTuru) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TahsisTip], (int)p.tashisTip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RezervTarihi], p.rezervTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OncelikDerecesi], p.oncelikDerecesi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.Aciklama], p.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(PersonelLojmani p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc RezervdenTahsisEt(PersonelLojmani p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BasTarihi], p.basTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TahsisTip], (int)p.tashisTip);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        internal static string ListeleUstTablo(string ustTabloAlan, TNSDateTime oturulanTarih, bool sadeceOturanlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " , lt.ETIKET, d.DaireNo, b.BlokNo, s.SITEADI, b.KOD blokKod, b.SKOD, d.DaireMetreKare, s.YAKITKESILECEK, d.DaireTuru";
            sql += " FROM " + TABLOAD;
            sql += " left outer join IKYSOZLUKLOJMANTIPI lt on IKYPERSONELLOJMAN.LojmanTipi = lt.KOD";
            sql += " inner join IKYLOJMANDAIRE d on d.KOD = IKYPERSONELLOJMAN.DKOD";
            sql += " inner join IKYLOJMANBLOK b on b.KOD = d.BKOD";
            sql += " inner join IKYLOJMANSITE s on s.KOD = b.SKOD";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PKOD] + " = " + ustTabloAlan + " AND TahsisTip=" + (int)ENUMTahsisTipi.Tahsis;

            if (sadeceOturanlar)
            {
                if (oturulanTarih != null && !oturulanTarih.isNull)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", "BasTarihi <= " + VeriErisim.DegerYap(oturulanTarih) + " AND (BitTarihi is NULL or BitTarihi >" + VeriErisim.DegerYap(oturulanTarih) + ") ");
                }
                else
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", "BitTarihi is NULL");
                }
            }
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BasTarihi] + " DESC";

            return sql;
        }

        public IDataReader PersonelinTahsisliLojmaniVarmi(int pKod, int plKod)
        {
            string sql = @"select kod
                        from IKYPERSONELLOJMAN 
                        where BitTarihi is null and PKOD = '" + pKod + "' and TahsisTip = " + (int)ENUMTahsisTipi.Tahsis +
                        " and KOD <> ' " + plKod + "'";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPersonelTerfi : TABOrtak
    {
        public TABPersonelTerfi(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYTERFI";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            PKOD = 1,
            PKADROKOD = 2,
            PYENIKADROKOD = 3,
            DURUM = 4,
            SEBEP = 5,
            TIP = 6,
            YIL = 7,
            AY = 8
        }

        static string[] ALANLAR = { "KOD", "PKOD", "PKADROKOD", "PYENIKADROKOD", "DURUM", "SEBEP", "TIP", "YIL", "AY" };

        public IDataReader Listele(Terfi p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", p.TCKIMLIKNO, p.KURUMSICILNO, p.AD +' '+ p.SOYAD, o.ADI as birim, "
                + " u.AD as unvan, k.KADRODERECE, k.KADROTARIH, k.ODEMEYEESASDERECE, k.ODEMEYEESASKADEME, k.ODEMEYEESASTARIH, k.KAZANILMISHAKDERECE, "
                + " k.KAZANILMISHAKKADEME, k.KAZANILMISHAKTARIH, k.EMEKLIYEESASDERECE, k.EMEKLIYEESASKADEME, k.EMEKLIYEESASTARIH, s.KAYITAD as ogrenim, "
                + " od.YUKSELECEGIDERECE, s2.SEBEPADI, yk.ODEMEYEESASDERECE, yk.ODEMEYEESASKADEME, yk.ODEMEYEESASEKGOSTERGE, yk.KAZANILMISHAKDERECE, "
                + " yk.KAZANILMISHAKKADEME, yk.KAZANILMISEKGOSTERGE, yk.EMEKLIYEESASDERECE, yk.EMEKLIYEESASKADEME, yk.EMEKLIYEESASEKGOSTERGE, IKYPERSONELTIPI.AD, "
                + " k.KAZANILMISEKGOSTERGE, k.EMEKLIYEESASEKGOSTERGE, k.ODEMEYEESASEKGOSTERGE";
            sql += " FROM " + TABLOAD;
            sql += " left outer join IKYPERSONELKADRO k on IKYTERFI.PKADROKOD = k.kod"
                    + " left outer join IKYPERSONEL p on p.kod = IKYTERFI.pkod"
                    + " left outer join Kullanici.dbo.ORGBIRIM o on o.kod = k.ORGANIZASYONKOD"
                    + " left outer join IKYUNVAN u on u.kod = k.UNVANKOD "
                    + " left outer join IKYPERSONELOGRENIM og on og.PKOD = p.kod and og.BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) FROM IKYPERSONELOGRENIM WHERE PKOD = og.PKOD)"
                    + " left outer join IKYSOZLUK s on s.KOD = og.OGRENIMKOD"
                    + " left outer join IKYSOZLUKHIZMETSEBEBI s2 on s2.KOD = IKYTERFI.SEBEP"
                    + " left outer join IKYOGRENIMDERECE od on od.OGRENIMKOD = s.KAYITKOD"
                    + " left outer join IKYPERSONELKADRO yk on IKYTERFI.PYENIKADROKOD = yk.kod"
                    + " left outer join IKYPERSONELTIPI on IKYPERSONELTIPI.KOD = k.PERSONELTUR ";

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.pKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.pKod));

            if (p.pKadroKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKADROKOD], "=", p.pKadroKod));

            if (p.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", p.durum));

            if (p.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", p.tip));

            if (p.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", p.yil));

            if (p.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", p.ay));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, 1";
            //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Terfi p)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.pKod) + ",";
            sql += VeriErisim.DegerYap(p.pKadroKod) + ",";
            if (p.pYeniKadroKod > 0)
                sql += VeriErisim.DegerYap(p.pYeniKadroKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.durum > 0)
                sql += VeriErisim.DegerYap(p.durum) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.hizmetSebebi > 0)
                sql += VeriErisim.DegerYap(p.hizmetSebebi) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.tip) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.ay) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(Terfi p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PKOD], p.pKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PKADROKOD], p.pKadroKod) + ",";
            if (p.pYeniKadroKod > 0)
                sql += VeriErisim.DegerYap(p.pYeniKadroKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.durum > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], p.durum) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (string)null) + ",";
            if (p.hizmetSebebi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEBEP], p.hizmetSebebi) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEBEP], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], p.tip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], p.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AY], p.ay);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int kod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader TerfisiGelenler(string yilAy, int tip)
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(yilAy.Substring(0, 4), -1);
            int ay = OrtakFonksiyonlar.ConvertToInt(yilAy.Substring(4, 2), -1);
            string sql = "select k.KOD, k.PKOD, p.TCKIMLIKNO, p.KURUMSICILNO, p.AD +' '+ p.SOYAD, o.ADI as birim, u.AD as unvan, KADRODERECE, KADROTARIH,";
            sql += " ODEMEYEESASDERECE, ODEMEYEESASKADEME, ODEMEYEESASTARIH,";
            sql += " KAZANILMISHAKDERECE, KAZANILMISHAKKADEME, KAZANILMISHAKTARIH,";
            sql += " EMEKLIYEESASDERECE, EMEKLIYEESASKADEME, EMEKLIYEESASTARIH,";
            sql += " dbo.terfiTipiDondur(ka.KAZANILMISTERFIAYI, ka.EMEKLITERFIAYI, '" + yilAy.Substring(4, 2) + "') as tip,";
            sql += " s.KAYITAD as ogrenim, od.YUKSELECEGIDERECE";
            sql += " FROM  IKYPERSONELKADRO k";
            sql += " INNER JOIN IKYPERSONEL p on p.kod = k.pkod";
            sql += " INNER JOIN IKYPERSONELKADROAYAR ka on ka.PKOD = p.KOD";
            sql += " INNER JOIN IKYPERSONELTIPI PERTIP on PERTIP.KOD = k.PERSONELTUR";
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM o on o.kod = k.ORGANIZASYONKOD";
            sql += " LEFT OUTER JOIN IKYUNVAN u on u.kod = k.UNVANKOD";
            sql += " LEFT OUTER JOIN IKYPERSONELOGRENIM og on og.PKOD = p.kod and og.BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) FROM IKYPERSONELOGRENIM WHERE PKOD = og.PKOD)";
            sql += " LEFT OUTER JOIN IKYSOZLUK s on s.KOD = og.OGRENIMKOD";
            sql += " LEFT OUTER JOIN IKYOGRENIMDERECE od on od.OGRENIMKOD = s.KAYITKOD";
            sql += " WHERE K.KOD NOT in (select PKADROKOD from IKYTERFI where YIL=" + yil + " and AY=" + ay + " and DURUM<>3 and PKADROKOD=K.KOD) and " + TABGenel.KadroDurumuTerfiler("k.KADRODURUMU");
            sql += " AND PERTIP.TERFIYEDAHIL = 1";

            //sql += " AND (dbo.terfiMiDondur(ka.KAZANILMISTERFIAYI, '" + yilAy + "', CONVERT(CHAR(8), ka.KAZANILMISTERFITARIHI, 112)) = 1 or dbo.terfiMiDondur(ka.EMEKLITERFIAYI, '" + yilAy + "', CONVERT(CHAR(8), ka.EMEKLITERFITARIHI, 112)) = 1)";
            if (tip == 1)
                sql += " and ka.KAZANILMISTERFIAYI = " + ay; // + "' AND ka.EMEKLITERFIAYI != '" + yilAy.Substring(4, 2) + "'";
            else if (tip == 2)
                sql += " and ka.EMEKLITERFIAYI = " + ay; // + "' AND ka.KAZANILMISTERFIAYI != '" + yilAy.Substring(4, 2) + "'";
            //else if (tip == 3)
            //    sql += " and ka.KAZANILMISTERFIAYI = '" + yilAy.Substring(4, 2) + "' AND ka.EMEKLITERFIAYI = '" + yilAy.Substring(4, 2) + "'";
            else
                sql += " and (ka.KAZANILMISTERFIAYI = " + ay + " OR ka.EMEKLITERFIAYI = " + ay + ")";

            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, 5";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc GuncelleTerfiDurumu(Terfi terfi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], terfi.durum);

            if (terfi.pYeniKadroKod > 0)
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PYENIKADROKOD], terfi.pYeniKadroKod);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", terfi.kod));

            return KomutCalistir(sql, 1);
        }
    }
}



















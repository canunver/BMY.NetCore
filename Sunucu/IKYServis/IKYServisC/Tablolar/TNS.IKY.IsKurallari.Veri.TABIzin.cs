using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPersonelIzin : TABOrtak
    {
        public TABPersonelIzin(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELIZIN";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            PKOD = 1,
            IZINTUR = 2,
            ACIKLAMA = 3,
            BASLANGICTARIHI = 4,
            BITISTARIHI = 5,
            KULLANILANGUN = 6,
            TELEFON = 7,
            IZINNOTU = 8,
            ONAY = 9,
            EVREVRAKNO = 10,
            BelgeNo = 11,
            BelgeTarihi = 12,
            ImzalayanPersonel = 13,
            TeklifEdenPersonel = 14,
            YerineGorevliPersonel = 15,
            KayitTarihi = 16,
            Memuriyeti = 17,
            SaglikKurulusu = 18,
            RAPORTARIHI = 19, RAPORNO = 20, YATILIGECENSURE = 21, YERKOD = 22,
            IZINNOTU2 = 23
        }
        public static string[] ALANLAR = { "KOD", "PKOD", "IZINTUR", "ACIKLAMA", "BASLANGICTARIHI", "BITISTARIHI", "KULLANILANGUN", "TELEFON",
                                           "IZINNOTU", "ONAYLIMI", "EVREVRAKNO", "BelgeNo", "BelgeTarihi", "ImzalayanPersonel", "TeklifEdenPersonel",
                                           "YerineGorevliPersonel", "KayitTarihi", "Memuriyeti", "SaglikKurulusu", "RAPORTARIHI",  "RAPORNO", "YATILIGECENSURE", "YERKOD","IZINNOTU2" };

        public static string TABLOADIZINDEVIR = "IKYPERSONELIZINDEVIR";
        public enum ENUMAlanlarIzinDevir : int
        {
            KOD = 0,
            PKOD = 1,
            YIL = 2,
            DEVREDEN = 3,
            YILICI = 4
        }
        public static string[] ALANLARIZINDEVIR = { "KOD", "PKOD", "YIL", "DEVREDEN", "YILICI" };


        public IDataReader ListeleIzin(PersonelIzin p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", IKYPERSONEL.AD + ' ' + IKYPERSONEL.SOYAD,  " +
                " IKYPERSONEL2.AD + ' ' + IKYPERSONEL2.SOYAD, IKYPERSONEL3.AD + ' ' + IKYPERSONEL3.SOYAD, pk.AD+' '+pk.SOYAD, B.ADI, " +
                " pk.KURUMSICILNO,UT.AD, UI.AD, IT.AD, COG.AD FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN IKYPERSONEL ON IKYPERSONEL.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ImzalayanPersonel];
            sql += " LEFT OUTER JOIN IKYPERSONEL as IKYPERSONEL2  ON IKYPERSONEL2.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TeklifEdenPersonel];
            sql += " LEFT OUTER JOIN IKYPERSONEL as IKYPERSONEL3  ON IKYPERSONEL3.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YerineGorevliPersonel];
            sql += " INNER JOIN IKYPERSONEL as pk ON pk.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PKOD];
            sql += " LEFT OUTER JOIN IKYPERSONELKADRO SONKADRO on SONKADRO.PKOD = pk.KOD AND " + TABGenel.KadroDurumuAktifler("SONKADRO.KADRODURUMU");
            sql += " LEFT OUTER JOIN Kullanici.DBO.ORGBIRIM AS B ON B.KOD=SONKADRO.ORGANIZASYONKOD";
            sql += " LEFT OUTER JOIN IKYPERSONELKADRO ki on ki.PKOD = IKYPERSONEL.KOD AND " + TABGenel.KadroDurumuAktifler("ki.KADRODURUMU");
            sql += " LEFT OUTER JOIN IKYPERSONELKADRO kt on kt.PKOD = IKYPERSONEL2.KOD AND " + TABGenel.KadroDurumuAktifler("kt.KADRODURUMU");
            sql += " LEFT OUTER JOIN IKYUNVAN AS UT ON UT.KOD=kt.GorunenUnvanKodu";
            sql += " LEFT OUTER JOIN IKYUNVAN AS UI ON UI.KOD=ki.GorunenUnvanKodu";
            sql += " LEFT OUTER JOIN IKYSOZLUKIZINTUR AS IT ON IT.KOD=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.IZINTUR];
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA AS COG ON COG.KOD=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YERKOD];

            if (p.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));
            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            if (p.izinTurKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IZINTUR], "=", p.izinTurKod));
            if (!p.baslangicTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], ">=", p.baslangicTarihi));
            if (!p.bitisTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIHI], "<=", p.bitisTarihi));
            if (!string.IsNullOrEmpty(p.evrEvrakNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", p.evrEvrakNo));
            if (p.donemYil > 0)
            {
                DateTime donemBasDT = TNS.IKY.IzinArac.DonemBasiBul(p.donemYil);
                DateTime donemSonDT = TNS.IKY.IzinArac.DonemSonuBul(p.donemYil);
                TNSDateTime donemBas = new TNSDateTime(donemBasDT);
                TNSDateTime donemSon = new TNSDateTime(donemSonDT);
                string kesisimSorgu = string.Format("dbo.DonemlerKesisiyorMu({0},{1},{2},{3}) = 1", VeriErisim.DegerYap(donemBas), VeriErisim.DegerYap(donemSon), TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIHI]);
                kosul = VeriErisim.KosulEkle(kosul, "AND", kesisimSorgu);
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleIzinDevir(PersonelIzinDevir p)
        {
            string kullanilan = ", NULL kullanilan";
            string izinGunSayisi = ", NULL izinGunSayisi";

            if (p.yil > 0)
            {
                kullanilan = ", (select sum(KULLANILANGUN) from IKYPERSONELIZIN where PKOD= " + TABLOADIZINDEVIR + ".PKOD and BASLANGICTARIHI >= " + veriErisim.DegerYapPRM(new TNSDateTime(TNS.IKY.IzinArac.DonemBasiBul(p.yil))) + " and BASLANGICTARIHI < " + veriErisim.DegerYapPRM(new TNSDateTime(TNS.IKY.IzinArac.DonemSonuBul(p.yil))) + " AND IZINTUR=1) kullanilan";
                izinGunSayisi = ", (select sum(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H2, null, K.HizmetGunSayisi)) from IKYPERSONELKADRO K LEFT OUTER JOIN IKYPERSONELTIPI PT ON K.PERSONELTUR = PT.KOD  where K.PKOD = " + TABLOADIZINDEVIR + ".PKOD) izinGunSayisi";
            }
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADIZINDEVIR, ALANLARIZINDEVIR) + kullanilan + izinGunSayisi + " FROM " + TABLOADIZINDEVIR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.PKOD], "=", p.personelKod));
            if (p.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL], "=", p.yil));
            sql += " order by " + ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleIzinDurum(PersonelIzinDurum pk, TNSDateTime donemBas, TNSDateTime donemSon)
        {
            string sql = "";

            sql = "SELECT "
                + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.IZINTUR] + ", "
                + "IKYSOZLUKIZINTUR.AD" + ", "
                + "IKYSOZLUKIZINTUR.SURE" + ", "
                + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI] + ", " + ALANLAR[(int)ENUMAlanlar.BITISTARIHI]
                + " FROM " + TABLOAD
                + " INNER JOIN IKYSOZLUKIZINTUR ON " + ALANLAR[(int)ENUMAlanlar.IZINTUR] + " = IKYSOZLUKIZINTUR.KOD ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", pk.personelKod));
            string kesisimSorgu = string.Format("dbo.DonemlerKesisiyorMu({0},{1},{2},{3}) = 1", VeriErisim.DegerYap(donemBas), VeriErisim.DegerYap(donemSon), TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIHI]);
            sql = VeriErisim.KosulEkle(sql, "AND", kesisimSorgu);

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.IZINTUR] + ", IKYSOZLUKIZINTUR.AD" + ", IKYSOZLUKIZINTUR.SURE";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleYillikIzin(int personelKod, int yil, int haricKod, TNSDateTime donemBas, TNSDateTime donemSon)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI] + "," + ALANLAR[(int)ENUMAlanlar.BITISTARIHI] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", personelKod));
            string kesisimSorgu = string.Format("dbo.DonemlerKesisiyorMu({0},{1},{2},{3}) = 1", VeriErisim.DegerYap(donemBas), VeriErisim.DegerYap(donemSon), TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIHI]);
            sql = VeriErisim.KosulEkle(sql, "AND", kesisimSorgu);
            if (haricKod > 0) //Verilen sirano ile kayıtlı izin hariç kullanılan izin süresi hesaplamak için. Önceden kayıt edilen izin düzeltilerek kayıt edilmek istendiğinde süre kontrolü yapmak için
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "<>", haricKod));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IZINTUR], "=", 1));//1->yıllık izin
            return this.veriErisim.DataReaderDondur(sql);
        }

        public IDataReader PersonelYilBazliYillikIzinBilgileri(int personelKod, DateTime tarih)
        {
            tarih = tarih.Date;
            string sql = "select (select sum(dbo.gunDondur(K.AtamaTarihi, K.AYRILMATARIHI, PT.H2, " + veriErisim.DegerYapPRM(new TNSDateTime(tarih)) + ", K.HizmetGunSayisi)) " +
                         " from IKYPERSONELKADRO K LEFT OUTER JOIN IKYPERSONELTIPI PT ON K.PERSONELTUR = PT.KOD where k.PKOD = p.KOD and " + TABGenel.KadroDurumuHesaplananlar("K.KADRODURUMU") + ") as izinGunu," +
                         " (select DEVREDEN from IKYPERSONELIZINDEVIR where PKOD = p.KOD and YIL = DATEPART(year, @p0) ) as devir, " +
                         " (select sum(KULLANILANGUN) from IKYPERSONELIZIN where PKOD = p.KOD and IZINTUR = 1 and BASLANGICTARIHI >= " + new TNSDateTime(new DateTime(tarih.Year, 1, 1)) + "  and BASLANGICTARIHI <= " + new TNSDateTime(new DateTime(tarih.Year, 12, 31)) + " ) as kullanilan " +
                         " from IKYPERSONEL p where p.KOD = " + veriErisim.DegerYapPRM(personelKod);

            return veriErisim.DataReaderDondur(sql);
        }

        public int ListeleKullanilanIzinAdet(int personelKod, int yil)
        {
            string sql = @"select count(*) adet
                            from IKYPERSONELIZIN
                            where DATEPART(year,BASLANGICTARIHI) = '" + yil +
                            "'    and PKOD = '" + personelKod +
                            "' and IZINTUR = " + 1;
            return veriErisim.TekKolonSQLInt(sql);
        }

        public int ListeleKullanilanIzinKendindenKucuk(int personelKod, TNSDateTime baslamaTarihi)
        {
            string sql = "";

            sql = "SELECT SUM(" + ALANLAR[(int)ENUMAlanlar.KULLANILANGUN] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", personelKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + baslamaTarihi.Yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], "<", baslamaTarihi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IZINTUR], "=", 1));//1->yıllık izin

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc KaydetIzin(PersonelIzin p)
        {
            string sql = "";

            //string sayacKosul = ALANLAR[(int)ENUMAlanlar.PKOD] + "=" + VeriErisim.DegerYap(p.personelKod);
            //p.kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], sayacKosul);

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            if (p.izinTurKod > 0)
                sql += VeriErisim.DegerYap(p.izinTurKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.aciklama) + ",";
            sql += VeriErisim.DegerYap(p.baslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(p.bitisTarihi) + ",";
            sql += VeriErisim.DegerYap(p.kullanilanGun) + ",";
            sql += VeriErisim.DegerYap(p.telefon) + ",";
            sql += VeriErisim.DegerYap(p.izinYeri) + ",";
            sql += VeriErisim.DegerYap(p.onay) + ",";
            sql += VeriErisim.DegerYap(p.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(p.belgeNo) + ",";
            sql += VeriErisim.DegerYap(p.belgeTarihi) + ",";
            if (p.imzalayan > 0)
                sql += VeriErisim.DegerYap(p.imzalayan) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.teklifEden > 0)
                sql += VeriErisim.DegerYap(p.teklifEden) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.yerineGorevli > 0)
                sql += VeriErisim.DegerYap(p.yerineGorevli) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.kayitTarihi) + ",";
            sql += VeriErisim.DegerYap(p.memuriyeti) + ",";
            sql += VeriErisim.DegerYap(p.saglikKurulusu) + ",";

            sql += VeriErisim.DegerYap(p.raporTarihi) + ",";
            sql += VeriErisim.DegerYap(p.raporNo) + ",";
            sql += VeriErisim.DegerYap(p.yatiliGecenSure) + ",";
            sql += VeriErisim.DegerYap(p.yerKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(p.izinNotu2) + ")";

            return KomutCalistir(sql, 1);
        }

        public int KaydetIzinUygunMu(PersonelIzin p)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], "<", p.bitisTarihi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIHI], ">", p.baslangicTarihi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "<>", p.kod));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc KaydetIzinDevir(PersonelIzinDevir p)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOADIZINDEVIR + " (" + VeriErisim.TumAlanlar("", ALANLARIZINDEVIR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            sql += VeriErisim.DegerYap(p.yil) + ",";
            sql += VeriErisim.DegerYap(p.devreden) + ",";
            sql += VeriErisim.DegerYap(p.yilici) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleIzin(PersonelIzin p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PKOD], p.personelKod) + ",";
            if (p.izinTurKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IZINTUR], p.izinTurKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IZINTUR], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], p.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], p.baslangicTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIHI], p.bitisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], p.telefon) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANILANGUN], p.kullanilanGun) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IZINNOTU], p.izinYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAY], p.onay) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], p.evrEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BelgeNo], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BelgeTarihi], p.belgeTarihi) + ",";
            if (p.imzalayan > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ImzalayanPersonel], p.imzalayan) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ImzalayanPersonel], (string)null) + ",";
            if (p.teklifEden > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TeklifEdenPersonel], p.teklifEden) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TeklifEdenPersonel], (string)null) + ",";
            if (p.yerineGorevli > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YerineGorevliPersonel], p.yerineGorevli) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YerineGorevliPersonel], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KayitTarihi], p.kayitTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.Memuriyeti], p.memuriyeti) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SaglikKurulusu], p.saglikKurulusu) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RAPORTARIHI], p.raporTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RAPORNO], p.raporNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YATILIGECENSURE], p.yatiliGecenSure) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERKOD], p.yerKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IZINNOTU2], p.izinNotu2);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleIzinDevir(PersonelIzinDevir p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADIZINDEVIR + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.DEVREDEN], p.devreden) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YILICI], p.yilici);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.PKOD], "=", p.personelKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL], "=", p.yil));

            return base.KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleIzinImza(PersonelIzin p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            if (p.teklifEden > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TeklifEdenPersonel], p.teklifEden) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ImzalayanPersonel], p.imzalayan);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return base.KomutCalistir(sql, 1);
        }

        public Sonuc SilIzin(PersonelIzin p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));
            if (p.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilIzinDevir(PersonelIzinDevir p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADIZINDEVIR;
            if (p.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.KOD], "=", p.kod));
            else
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.PKOD], "=", p.personelKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADIZINDEVIR, ALANLARIZINDEVIR[(int)ENUMAlanlarIzinDevir.YIL], "=", p.yil));
            }

            return KomutCalistir(sql, 0);
        }

        public bool IzinKayitKontrol(PersonelIzin p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT SUM(" + ALANLAR[(int)ENUMAlanlar.KULLANILANGUN] + ")"
                + " FROM " + TABLOAD
                + " INNER JOIN IKYSOZLUKIZINTUR ON " + ALANLAR[(int)ENUMAlanlar.IZINTUR] + " = IKYSOZLUKIZINTUR.KOD";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IZINTUR], "=", p.izinTurKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));
            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "<>", p.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], ">=", new TNSDateTime("01.01." + DateTime.Now.Year.ToString("d4"))));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIHI], "<=", new TNSDateTime("31.12." + DateTime.Now.Year.ToString("d4"))));

            sql += " WHERE " + kosul;
            sql += " GROUP BY IKYSOZLUKIZINTUR.KOD";
            int dahaOnceKullandigiToplamIzin = veriErisim.TekKolonSQLInt(sql);
            sql = "SELECT IKYSOZLUKIZINTUR.SURE FROM IKYSOZLUKIZINTUR WHERE IKYSOZLUKIZINTUR.KOD=" + p.izinTurKod;
            int kullanabilecegiToplamIzin = veriErisim.TekKolonSQLInt(sql);
            if (kullanabilecegiToplamIzin == 0)
                return true;
            if ((kullanabilecegiToplamIzin - dahaOnceKullandigiToplamIzin) >= p.kullanilanGun)
                return true;
            else
                return false;
        }

        internal static string ListeleUstTablo(Personel per, string ustTabloAlan)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("I", ALANLAR) + ",C2.AD,C.AD FROM " + TABLOAD + " I LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA C ON C.KOD=I.YERKOD";
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGCOGRAFYA C2 ON C2.KOD=LEFT(I.YERKOD,3)";
            sql += " WHERE " + "I." + ALANLAR[(int)ENUMAlanlar.PKOD] + "=" + ustTabloAlan;
            string kesisimSorgu = string.Format("dbo.DonemlerKesisiyorMu({0},{1},{2},{3}) = 1", VeriErisim.DegerYap(per.izinDonemBas), VeriErisim.DegerYap(per.izinDonemSon), "I." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], "I." + ALANLAR[(int)ENUMAlanlar.BITISTARIHI]);
            sql = VeriErisim.KosulEkle(sql, "AND", kesisimSorgu);
            if (!string.IsNullOrWhiteSpace(per.izinTurler))
            {
                sql += VeriErisim.IN_KosulOl("AND", ALANLAR[(int)ENUMAlanlar.IZINTUR], per.izinTurler, false, false);
            }
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI] + " DESC";

            return sql;
        }
    }
}

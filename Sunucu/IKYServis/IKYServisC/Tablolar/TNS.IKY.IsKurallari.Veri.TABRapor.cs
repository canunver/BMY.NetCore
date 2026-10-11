using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABRapor : TABOrtak
    {
        public TABRapor(VeriErisim veriErisim) : base(veriErisim) { }

        public static string ListeleKadroKontenjanSql(PersonelKadro pk)
        {
            string sql = "select IKYUNVAN.SIRANO, UNVANKOD, KADRODERECE, IKYUNVAN.AD, IKYUNVAN.ADKISA, sum(KONTENJAN)"
                   + " from IKYKADROKONTENJAN"
                   + " INNER JOIN IKYKADROKONTENJANDETAY ON IKYKADROKONTENJANDETAY.KKOD = IKYKADROKONTENJAN.KOD"
                   + " INNER JOIN IKYUNVAN ON IKYUNVAN.KOD = IKYKADROKONTENJANDETAY.UNVANKOD";

            string kosul = "";

            if (pk.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYKADROKONTENJAN", "ORGANIZASYONKOD", "=", pk.organizasyonKodu));

            if (pk.personelTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYKADROKONTENJAN", "PERSONELTUR", "=", pk.personelTip));

            if (pk.unvan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYKADROKONTENJANDETAY", "UNVANKOD", "=", pk.unvan));

            if (pk.kadroDerece > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYKADROKONTENJANDETAY", "KADRODERECE", "=", pk.kadroDerece));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += IN_KosulOl(BaglacDondur(kosul), "IKYKADROKONTENJAN.PERSONELTUR", pk.personelTipCoklu, false);

            sql += " group by UNVANKOD, KADRODERECE, IKYUNVAN.AD, IKYUNVAN.ADKISA, IKYUNVAN.SIRANO";
            sql += " order by IKYUNVAN.SIRANO ASC, IKYUNVAN.AD";

            return sql;
        }

        public static string ListelePersonelKadroKullanimiSql(PersonelKadro pk)
        {
            string sql = "select IKYUNVAN.SIRANO, IKYPERSONELKADRO.UNVANKOD, IKYPERSONELKADRO.KADRODERECE, IKYUNVAN.AD, IKYUNVAN.ADKISA, count(*) from IKYPERSONELKADRO INNER JOIN IKYPERSONEL on PKOD = IKYPERSONEL.KOD and " + TABGenel.KadroDurumuAktifAciklar("IKYPERSONELKADRO.KADRODURUMU");
            sql += " INNER JOIN IKYUNVAN ON IKYUNVAN.KOD = IKYPERSONELKADRO.UNVANKOD INNER JOIN IKYPERSONELTIPI ON IKYPERSONELTIPI.KOD = IKYPERSONELKADRO.PERSONELTUR";

            string kosul = VeriErisim.KosulOl("IKYPERSONEL", "AKTIFMI", "=", 1);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONELTIPI", "KADROLU", "=", 1));
            kosul = VeriErisim.KosulEkle(kosul, "AND", TABGenel.KadroDurumuAktifAciklar("IKYPERSONELKADRO.KADRODURUMU"));

            if (pk.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONELKADRO", "ORGANIZASYONKOD", "=", pk.organizasyonKodu));

            if (pk.personelTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONELKADRO", "PERSONELTUR", "=", pk.personelTip));

            if (pk.unvan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONELKADRO", "UNVANKOD", "=", pk.unvan));

            if (pk.kadroDerece > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONELKADRO", "KADRODERECE", "=", pk.kadroDerece));

            if (pk.kisiHaric > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONELKADRO", "PKOD", "<>", pk.kisiHaric));

            if (kosul != "")
                sql += " WHERE " + kosul;
            sql += " GROUP BY IKYUNVAN.SIRANO, IKYPERSONELKADRO.UNVANKOD, IKYPERSONELKADRO.KADRODERECE, IKYUNVAN.AD, IKYUNVAN.ADKISA";
            sql += " ORDER BY IKYUNVAN.SIRANO, IKYUNVAN.AD, IKYUNVAN.ADKISA";
            return sql;
        }

        public IDataReader ListelePersonelKadroKullanimi(PersonelKadro pk)
        {
            string sql = "select count(*) from IKYPERSONELKADRO INNER JOIN IKYPERSONEL on PKOD = IKYPERSONEL.KOD where UNVANKOD = " + pk.unvan + " and KADRODERECE = " + pk.kadroDerece + " AND " + TABGenel.KadroDurumuAktifler("KADRODURUMU");

            string kosul = "";

            if (pk.organizasyonKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONELKADRO", "ORGANIZASYONKOD", "=", pk.organizasyonKodu));
            if (pk.personelTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONEL", "PERSONELTUR", "=", pk.personelTip));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IsimliKadroListesi(string personelTur, string personelKod, string organizasyonKopruKodu, int siralamaListesi)
        {
            string sql = "SELECT case when k.ORGANIZASYONKOD is null then k.GOREVYERI when k.ORGANIZASYONKOD = 0 then k.GOREVYERI else o.ADI end, p.AD + ' ' + p.SOYAD,"
                    + " k.KADRODERECE, k.KAZANILMISEKGOSTERGE, IKYPERSONELTIPI.AD"
                    + " from IKYPERSONEL p"
                    + " left outer join IKYPERSONELKADRO k on k.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU")
                    + " left outer join IKYUNVAN u on u.KOD = k.UNVANKOD"
                    + " left outer join IKYPERSONELTIPI on IKYPERSONELTIPI.KOD = k.PERSONELTUR"
                    + " left outer join Kullanici.dbo.ORGBIRIM o on o.KOD = k.ORGANIZASYONKOD where p.AKTIFMI = 1";

            sql += IN_KosulOl("AND", "k.PERSONELTUR", personelTur, false);

            sql += IN_KosulOl("AND", "p.KOD", personelKod, false);

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND o.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            string siralama = "";
            if (siralamaListesi == 1)
            {
                siralama = "o.KOPRUKOD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, p.AD + ' ' + p.SOYAD";
            }
            else if (siralamaListesi == 2)
            {
                siralama = "p.AD + ' ' + p.SOYAD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD";
            }
            else
            {
                siralama = "case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD, p.AD + ' ' + p.SOYAD";
            }
            return veriErisim.DataReaderDondur(sql + " ORDER BY " + siralama);
        }

        public IDataReader IsimliUnvanListesi(string personelTur, string personelKod, string organizasyonKopruKodu)
        {
            string sql = "SELECT s.KISAAD, u.AD , p.AD + ' ' + p.SOYAD"
                        + " from IKYPERSONEL p"
                        + " left outer join IKYPERSONELKADRO k on k.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU")
                        + " left outer join IKYUNVAN u on u.KOD = k.UNVANKOD"
                        + " left outer join IKYSOZLUK s on s.KOD = u.SINIFKOD"
                        + " left outer join Kullanici.dbo.ORGBIRIM o on o.KOD = k.ORGANIZASYONKOD where p.AKTIFMI = 1";

            sql += IN_KosulOl("AND", "k.PERSONELTUR", personelTur, false);

            sql += IN_KosulOl("AND", "p.KOD", personelKod, false);

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND o.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, p.AD, p.SOYAD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IsimsizKadroListesi(string personelTur, string personelKod, string organizasyonKopruKodu)
        {
            string sql = "SELECT s2.KAYITAD, u.AD as Unvan, COUNT(*), k.KADRODERECE, k.ODEMEYEESASEKGOSTERGE, IKYPERSONELTIPI.AD, u.SIRANO"
                    + " from IKYPERSONEL p"
                    + " left outer join IKYPERSONELKADRO k on k.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU")
                    + " left outer join IKYPERSONELTIPI on IKYPERSONELTIPI.KOD = k.PERSONELTUR"
                    + " left outer join IKYUNVAN u on u.KOD = k.UNVANKOD"
                    + " left outer join Kullanici.dbo.ORGBIRIM o on o.KOD = k.ORGANIZASYONKOD "
                    + " left outer join IKYSOZLUK s2 on s2.KOD = u.SINIFKOD  where p.AKTIFMI = 1";

            sql += IN_KosulOl("AND", "k.PERSONELTUR", personelTur, false);

            sql += IN_KosulOl("AND", "p.KOD", personelKod, false);

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND o.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);
            sql += " group by s2.KAYITAD, u.AD, k.KADRODERECE, k.ODEMEYEESASEKGOSTERGE, IKYPERSONELTIPI.AD, u.SIRANO";
            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, 1";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KadroDereceDurumListesi(string personelTur, string personelKod, string organizasyonKopruKodu, int siralamaListesi)
        {
            string sql = "SELECT p.AD + ' ' + p.SOYAD, u.AD, IKYPERSONELTIPI.AD, k.KADRODERECE,"
                    + " k.KAZANILMISHAKDERECE, k.KAZANILMISHAKKADEME, k.KAZANILMISEKGOSTERGE, k.EMEKLIYEESASDERECE, k.EMEKLIYEESASKADEME, k.EMEKLIYEESASEKGOSTERGE, P.KOD, Derece.KAYITAD, Derece.YUKSELECEGIDERECE"
                    + " from IKYPERSONEL p"
                    + " left outer join (SELECT PKOD, s.KAYITAD KAYITAD, d.YUKSELECEGIDERECE YUKSELECEGIDERECE FROM IKYPERSONELOGRENIM o left outer join IKYSOZLUK s on s.KOD = o.OGRENIMKOD left outer join IKYOGRENIMDERECE d on s.KAYITKOD = d.OGRENIMKOD where o.BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) FROM IKYPERSONELOGRENIM WHERE PKOD = o.PKOD)) Derece on Derece.PKOD = p.KOD"
                    + " left outer join IKYPERSONELKADRO k on k.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU")
                    + " left outer join IKYUNVAN u on u.KOD = k.UNVANKOD "
                    + " left outer join Kullanici.dbo.ORGBIRIM o on o.KOD = k.ORGANIZASYONKOD "
                    + " left outer join IKYPERSONELTIPI on IKYPERSONELTIPI.KOD = k.PERSONELTUR where p.AKTIFMI = 1";

            sql += IN_KosulOl("AND", "k.PERSONELTUR", personelTur, false);
            sql += IN_KosulOl("AND", "p.KOD", personelKod, false);

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND o.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            string siralama = "";
            if (siralamaListesi == 1)
            {
                siralama = "o.KOPRUKOD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, p.AD + ' ' + p.SOYAD";
            }
            else if (siralamaListesi == 2)
            {
                siralama = "p.AD + ' ' + p.SOYAD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD";
            }
            else
            {
                siralama = "case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD, p.AD + ' ' + p.SOYAD";
            }

            sql += " ORDER BY " + siralama;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BakmaklaYukumluListesi(string personelKod, string personelTur, string organizasyonKopruKodu, int yukumlu, int aktiflik)
        {
            string sql = "select p.kod, p.AD + ' ' + p.SOYAD as personelAd, u.AD as unvan, A.AD + ' ' + A.SOYAD as YakinininAdi,"
                        + " s3.KAYITAD as ogrenim, a.DOGUMTARIHI, s1.KAYITKOD as durumu,"
                        + " dbo.gunDondur(a.DOGUMTARIHI, getdate(), 1, null, 0)/360 as yas, A.YAKINLIKDERECESI "
                        + " from IKYPERSONELAILE A"
                        + " left outer join IKYSOZLUK s1 on s1.kod = a.DURUM"
                        + " left outer join IKYSOZLUK s3 on s3.kod = a.OGRENIMTIPI"
                        + " inner join IKYPERSONEL p on p.KOD = a.PKOD"
                        + " left outer join IKYPERSONELKADRO K on k.PKOD = p.KOD AND " + TABGenel.KadroDurumuAktifler("K.KADRODURUMU")
                        + " left outer join Kullanici.dbo.ORGBIRIM o on o.KOD = k.ORGANIZASYONKOD "
                        + " left outer join IKYUNVAN u on u.KOD = k.UNVANKOD ";

            string kosul = "";

            if (aktiflik > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("p", "AKTIFMI", "=", aktiflik));

            if (yukumlu > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", "BAKMAKLAYUKUMLU", "=", yukumlu));
                string icSorgu = "";
                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", " YUKUMLULUKBITISTARIHI >= GETDATE() ");
                icSorgu = VeriErisim.KosulEkle(icSorgu, "OR", " YUKUMLULUKBITISTARIHI is null ");
                kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icSorgu + ")");
            }

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("o", "KOPRUKOD", "LIKE", VeriErisim.DegerYap(organizasyonKopruKodu)));

            kosul += IN_KosulOl(BaglacDondur(kosul), "K.PERSONELTUR", personelTur, false);
            kosul += IN_KosulOl(BaglacDondur(kosul), "P.KOD", personelKod, false);

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, p.kod";
            return veriErisim.DataReaderDondur(sql);
        }

        private static string BaglacDondur(string kosul)
        {
            if (!string.IsNullOrEmpty(kosul))
                return " and ";
            else
                return " where ";
        }

        public IDataReader OgrenimDurumuListesi(string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik)
        {
            string sql = @"select OKULADI, s1.KAYITAD as ogrenim, P.AD + ' ' + P.SOYAD as isim, U.AD unvan,
                         org.ADI birim, DATEDIFF(YEAR, IKYPERSONELNUFUS.DOGUMTARIHI, GETDATE()) as yasGunu,
                         kk.gun kurumHizmetGunu, IKYPERSONELTIPI.AD
                         from IKYPERSONELOGRENIM O 
                         left outer join IKYSOZLUK s1 on s1.KOD = O.OGRENIMKOD 
                         inner join IKYPERSONEL P on p.KOD = O.PKOD 
                         inner join IKYPERSONELNUFUS on IKYPERSONELNUFUS.KOD = O.PKOD
                         left outer join IKYPERSONELKADRO K on k.PKOD = p.KOD AND " + TABGenel.KadroDurumuAktifler("K.KADRODURUMU") +
                        @" left outer join Kullanici.dbo.ORGBIRIM org on org.KOD = k.ORGANIZASYONKOD 
                         left outer join IKYUNVAN u on u.KOD = k.UNVANKOD
                         left outer join IKYPERSONELTIPI on IKYPERSONELTIPI.KOD = K.PERSONELTUR
                         left outer join (select SUM(dbo.gunDondur(pk.AtamaTarihi, pk.AYRILMATARIHI,  1, null, pk.HizmetGunSayisi)) as gun, 
                         pk.PKOD FROM IKYPERSONELKADRO pk INNER JOIN IKYPERSONELTIPI on IKYPERSONELTIPI.KOD = pk.PERSONELTUR where IKYPERSONELTIPI.KURUMDISI=0 group by pk.PKOD) as KK on KK.PKOD = P.KOD
                         where --GECERLI = 1 
                         o.BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) FROM IKYPERSONELOGRENIM WHERE PKOD = o.PKOD)";

            if (aktiflik > -1)
                sql += " and p.AKTIFMI = " + aktiflik;

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND org.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            sql += IN_KosulOl("AND", "K.PERSONELTUR", personelTur, false);
            sql += IN_KosulOl("AND", "P.KOD", personelKod, false);
            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, u.AD, p.AD";
            return veriErisim.DataReaderDondur(sql);
        }

        public static string IN_KosulOl(string baglac, string alan, string parametreler, bool tirnakla)
        {
            if (!string.IsNullOrWhiteSpace(parametreler))
            {
                return IN_KosulOl(baglac, alan, parametreler.Split(',', ';'), tirnakla);
            }
            return "";
        }

        public static string IN_KosulOl(string baglac, string alan, string[] parametreler, bool tirnakla)
        {
            string donusDeger = "";
            if (parametreler != null && parametreler.Length > 0)
            {
                foreach (string item in parametreler)
                {
                    if (!tirnakla && (OrtakFonksiyonlar.ConvertToInt(item, 0) == 0)) continue;

                    if (donusDeger == "")
                        donusDeger = baglac + " " + alan + " IN (";
                    else
                        donusDeger += ", ";

                    donusDeger += Tirnakla(item, tirnakla);
                }
                if (donusDeger != "")
                {
                    donusDeger += ")";
                }
            }
            return donusDeger;
        }

        private static string Tirnakla(string deger, bool tirnakla)
        {
            if (tirnakla)
                return VeriErisim.DegerYap(deger);
            else
                return deger;
        }

        public IDataReader MailImzaRaporu(int personelKod)
        {
            string sql = "select  p.AD+' '+p.SOYAD as isim, ISTELEFON, CEPTELEFON ,i.FAKS, u.AD as unvan, b.ADI as birim from IKYPERSONEL p"
                        + " left outer join IKYPERSONELILETISIM i on i.KOD = p.KOD "
                        + " left outer join IKYPERSONELKADRO k on p.KOD = k.PKOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU")
                        + " left outer join dbo.IKYUNVAN u on u.KOD = k.UNVANKOD "
                        + " left outer join Kullanici.dbo.ORGBIRIM b on b.KOD = k.ORGANIZASYONKOD where p.AKTIFMI = 1";

            if (personelKod > 0)
                sql += " and p.kod = " + personelKod;
            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, u.AD, p.AD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IzinliListesi(string personelKod, string personelTur, string organizasyonKopruKodu, string izinTurler, TNSDateTime kucukTarih, TNSDateTime buyukTarih, int siralamaListesi, int aktiflik)
        {
            TNSCollection arr = new TNSCollection();
            string sql = "select p.ad+' '+p.SOYAD isim, u.AD unvan, KADRODERECE, t.AD izinTuru, "
                       + " BASLANGICTARIHI, BITISTARIHI, i.ACIKLAMA, KULLANILANGUN, p.KOD, i.IZINNOTU from IKYPERSONELIZIN i "
                       + " inner join IKYPERSONEL p on p.KOD = i.PKOD "
                       + " left outer join IKYPERSONELKADRO k on i.PKOD = k.PKOD "
                       + " left outer join IKYUNVAN u on u.KOD = k.UNVANKOD "
                       + " left outer join IKYSOZLUKIZINTUR t on t.KOD = i.IZINTUR "
                       + " left outer join Kullanici.dbo.ORGBIRIM o on o.KOD = k.ORGANIZASYONKOD "
                       + " where i.BASLANGICTARIHI >= k.AtamaTarihi and (i.BASLANGICTARIHI < k.AYRILMATARIHI or k.AYRILMATARIHI is null)";

            if (aktiflik > -1)
                sql += " and p.AKTIFMI = " + aktiflik;

            if (!string.IsNullOrWhiteSpace(izinTurler))
                sql += " AND i.IZINTUR IN (" + izinTurler + ") ";

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND o.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            sql += IN_KosulOl("AND", "k.PERSONELTUR", personelTur, false);
            sql += IN_KosulOl("AND", "p.KOD", personelKod, false);

            if (buyukTarih.Yil < 1900)
                buyukTarih = new TNSDateTime(System.DateTime.Now);
            if (kucukTarih.Yil < 1900)
                kucukTarih = new TNSDateTime("01.01." + (System.DateTime.Now.Year - 1).ToString());

            sql += " and ( (i.BASLANGICTARIHI >= " + veriErisim.DegerYapPRM(kucukTarih) + " and i.BASLANGICTARIHI <= "+ veriErisim.DegerYapPRM(buyukTarih) + ") or (i.BITISTARIHI > @p0 and i.BITISTARIHI < @p1))";

            string siralama = "";
            if (siralamaListesi == 1)
                siralama = "o.KOPRUKOD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, p.AD + ' ' + p.SOYAD";
            else if (siralamaListesi == 2)
                siralama = "p.AD + ' ' + p.SOYAD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD";
            else
                siralama = "case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD, p.AD + ' ' + p.SOYAD";

            sql += " ORDER BY p.AD + ' ' + p.SOYAD+' ' + p.TCKIMLIKNO, " + siralama;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KartListesi(string personelKod, string personelTur, string organizasyonKopruKodu, string raporTur)
        {
            string sql = "select s1.KAYITAD kartTipi, p.ad+' '+p.SOYAD isim, u.AD unvan, "
                        + " k.Yakinligi, AdiSoyadi, kartNo, Plaka, TagCihazNo, ZimmetTarihi, IadeTarihi, KayipTarihi"
                        + " from IKYPERSONELKART K"
                        + " inner join IKYPERSONEL P on P.KOD = k.PKOD"
                        + " left outer join IKYPERSONELKADRO pk on pk.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU")
                        + " left outer join IKYSOZLUK s1 on s1.KOD = k.kartTipi"
                        + " left outer join IKYUNVAN u on u.KOD = pk.UNVANKOD"
                        + " left outer join Kullanici.dbo.ORGBIRIM o on o.KOD = pk.ORGANIZASYONKOD ";

            if (raporTur == "IK030")
                sql += " where P.AKTIFMI = 1  and k.KayipTarihi is not null ";// and k.AktifMi = 0";
            else if (raporTur == "IK029")
                sql += " where P.AKTIFMI = 0 and k.IadeTarihi is null ";
            else
                sql += " where P.AKTIFMI = 1 ";//and K.AktifMi = 1 ";

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND o.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            sql += IN_KosulOl("AND", "pk.PERSONELTUR", personelTur, false);
            sql += IN_KosulOl("AND", "P.KOD", personelKod, false);
            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, u.AD, p.AD";
            return veriErisim.DataReaderDondur(sql);
        }

        //public IDataReader SirtlikListesi(string personelKod, string personelTur, string organizasyonKopruKodu)
        //{
        //    string sql = "select p.KOD, p.AD+ ' ' +p.SOYAD ad, r.RESIM, p.KURUMSICILNO, p.TCKIMLIKNO"
        //                + " from IKYPERSONEL p"
        //                + " left outer join IKYPERSONELRESIM r on p.KOD = r.KOD"
        //                + " left outer join IKYPERSONELKADRO k on k.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("k.KADRODURUMU")
        //                + " left outer join Kullanici.dbo.ORGBIRIM o on o.KOD = k.ORGANIZASYONKOD "
        //                + " where p.AKTIFMI = 1 ";

        //    if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
        //        sql += " AND o.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

        //    sql += IN_KosulOl("AND", "k.PERSONELTUR", personelTur, false);
        //    sql += IN_KosulOl("AND", "P.KOD", personelKod, false);

        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public IDataReader DinamikRaporListesi(string personelKod, string personelTur, string organizasyonKopruKodu, string siralamalar, int aktiflik)
        //{
        //    string sql = "select r.RESIM, p.KOD, p.AD, p.SOYAD, p.TCKIMLIKNO, p.KURUMSICILNO, u.AD as unvan, b.ADI as birim, k.AtamaTarihi, k.AYRILMATARIHI"
        //                + " , i.CEPTELEFON, i.ISTELEFON, n.BABAAD, k.KADRODERECE, k.KAZANILMISEKGOSTERGE, n.DOGUMYERI, n.DOGUMTARIHI,o.AD, "
        //                + " s.KAYITAD as ogrenim, p.GELDIGIKURUM, og.OKULADI, pt.AD"
        //                + " from IKYPERSONEL p "
        //                + " left outer join IKYPERSONELNUFUS n on p.kod = n.kod"
        //                + " left outer join IKYPERSONELILETISIM i on i.kod = p.KOD"
        //                + " left outer join Kullanici.dbo.ORGCOGRAFYA o on o.KOD = n.NUFUSILCEKOD"
        //                + " left outer join IKYPERSONELRESIM r on r.KOD = p.KOD"
        //                + " left outer join IKYPERSONELOGRENIM og on og.PKOD = p.KOD and og.BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) FROM IKYPERSONELOGRENIM WHERE PKOD = og.PKOD)"
        //                + " left outer join IKYSOZLUK s on s.KOD = og.OGRENIMKOD "
        //                + " left outer join IKYPERSONELKADRO k on k.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("k.KADRODURUMU")
        //                + " left outer join IKYUNVAN u on u.KOD = k.UNVANKOD "
        //                + " left outer join Kullanici.dbo.ORGBIRIM b on b.KOD = k.ORGANIZASYONKOD"
        //                + " left outer join IKYPERSONELTIPI pt on pt.KOD = k.PERSONELTUR";

        //    string kosul = "";

        //    if (aktiflik > -1)
        //        kosul = BaglacDondur(kosul) + " p.AKTIFMI = " + aktiflik;

        //    if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
        //        kosul += BaglacDondur(kosul) + " b.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

        //    kosul += IN_KosulOl(BaglacDondur(kosul), "K.PERSONELTUR", personelTur, false);
        //    kosul += IN_KosulOl(BaglacDondur(kosul), "P.KOD", personelKod, false);

        //    if (!string.IsNullOrWhiteSpace(kosul))
        //        sql += kosul;

        //    if (!string.IsNullOrWhiteSpace(siralamalar))
        //    {
        //        sql += " order by ";
        //        int cnt = 1;
        //        string[] liste = siralamalar.Split(',', ';');
        //        string siralamaAlani = "";
        //        foreach (string item in liste)
        //        {
        //            int siraKod = OrtakFonksiyonlar.ConvertToInt(item, 0);
        //            if (siraKod > 0)
        //            {
        //                if (!string.IsNullOrWhiteSpace(siralamaAlani))
        //                    siralamaAlani += ", ";
        //                switch (siraKod)
        //                {
        //                    case 1:
        //                        siralamaAlani += " p.kod";
        //                        break;
        //                    case 2:
        //                        siralamaAlani += " p.Ad";
        //                        break;
        //                    case 3:
        //                        siralamaAlani += " p.Soyad";
        //                        break;
        //                    case 4:
        //                        siralamaAlani += " p.TCKIMLIKNO";
        //                        break;
        //                    case 5:
        //                        siralamaAlani += " p.KURUMSICILNO";
        //                        break;
        //                    case 6:
        //                        siralamaAlani += " u.AD";
        //                        break;
        //                    case 7:
        //                        siralamaAlani += " b.ADI";
        //                        break;
        //                    case 8:
        //                        siralamaAlani += " pt.AD";
        //                        break;
        //                    case 9:
        //                        siralamaAlani += " k.AtamaTarihi";
        //                        break;
        //                    case 10:
        //                        siralamaAlani += " k.AYRILMATARIHI";
        //                        break;
        //                    case 11:
        //                        siralamaAlani += " i.CEPTELEFON";
        //                        break;
        //                    case 12:
        //                        siralamaAlani += " i.ISTELEFON";
        //                        break;
        //                    case 13:
        //                        siralamaAlani += " n.BABAAD";
        //                        break;
        //                    case 14:
        //                        siralamaAlani += " k.KADRODERECE";
        //                        break;
        //                    case 15:
        //                        siralamaAlani += " k.KAZANILMISEKGOSTERGE";
        //                        break;
        //                    case 16:
        //                        siralamaAlani += " n.DOGUMYERI";
        //                        break;
        //                    case 17:
        //                        siralamaAlani += " n.DOGUMTARIHI";
        //                        break;
        //                    case 18:
        //                        siralamaAlani += " o.AD";
        //                        break;
        //                    case 19:
        //                        siralamaAlani += " s.KAYITAD";
        //                        break;
        //                    case 20:
        //                        siralamaAlani += " p.GELDIGIKURUM";
        //                        break;
        //                    case 21:
        //                        siralamaAlani += " og.OKULADI";
        //                        break;
        //                }
        //            }
        //            cnt++;
        //        }

        //        sql += siralamaAlani;
        //    }

        //    return veriErisim.DataReaderDondur(sql);
        //}

        public IDataReader NakilBidirimRaporu(int pkod)
        {
            string sql = "select p.AD + ' ' + p.SOYAD, p.KURUMSICILNO, p.EMEKLISICILNO, p.TCKIMLIKNO, u.AD unvan, b.ADI birim, k.KAZANILMISHAKDERECE, ";
            sql += "k.KAZANILMISHAKKADEME, k.EMEKLIYEESASDERECE,k.EMEKLIYEESASKADEME, k.AtamaTarihi,k.AYRILMATARIHI, saglikIzni.izin,  ";
            //sql += "kalanIzin.DEVREDEN,";
            sql += "s.KAYITAD medeniHali, cocuklar.cocukSayisi, esi.durumAdi, o.OKULADI, kidem.kidemGunu ";
            sql += "from IKYPERSONEL p ";
            sql += "left outer join IKYPERSONELNUFUS n on n.KOD = p.KOD ";
            sql += "left outer join IKYSOZLUK s on s.KOD = n.MEDENIHALI ";
            sql += "left outer join IKYPERSONELKADRO k on k.PKOD = p.KOD  ";
            sql += "left outer join IKYUNVAN u on u.KOD = k.UNVANKOD ";
            sql += "left outer join Kullanici.dbo.ORGBIRIM b on b.KOD = k.ORGANIZASYONKOD ";
            sql += "left outer join (select sum(KULLANILANGUN) as izin, PKOD from IKYPERSONELIZIN where IZINTUR = 6 and BASLANGICTARIHI >= " + veriErisim.DegerYapPRM(new TNSDateTime(new DateTime(DateTime.Now.Year, 1, 1))) + " ";
            sql += "and BASLANGICTARIHI <= " + veriErisim.DegerYapPRM(new TNSDateTime(new DateTime(DateTime.Now.Year, 12, 31))) + " group by PKOD) saglikIzni on saglikIzni.PKOD = p.KOD ";
            //sql += "left outer join (select DEVREDEN, PKOD from IKYPERSONELIZINDEVIR where YIL = DATEPART(YEAR,GETDATE())) kalanIzin on kalanIzin.PKOD = p.KOD ";
            sql += "left outer join (select PKOD,count(*) cocukSayisi from IKYPERSONELAILE where YAKINLIKDERECESI in  ";
            sql += "  (" + ((int)ENUMYakinlikDerecesi.Kizi).ToString() + "," + ((int)ENUMYakinlikDerecesi.Oglu).ToString() + ") group by PKOD) cocuklar on cocuklar.PKOD = p.KOD ";
            sql += "left outer join (select PKOD, s.KAYITAD durumAdi from IKYPERSONELAILE left outer join IKYSOZLUK s on s.KOD = DURUM ";
            sql += "  where YAKINLIKDERECESI =  ";
            sql += "  (select kod from IKYSOZLUK where KAYITKOD = ('06.00001'))) esi on esi.PKOD = p.KOD ";
            sql += "left outer join IKYPERSONELOGRENIM o on o.PKOD = p.KOD and o.BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) FROM IKYPERSONELOGRENIM WHERE PKOD = o.PKOD) ";
            sql += "left outer join ( ";
            sql += "  select sum(dbo.gunDondur(AtamaTarihi, AYRILMATARIHI, H.H6, null, HizmetGunSayisi)) as kidemGunu, k.PKOD ";
            sql += "  from IKYPERSONELKADRO K ";
            sql += "  left outer join IKYPERSONELTIPI H on H.KOD = K.PERSONELTUR ";
            sql += "  group by k.PKOD) kidem on kidem.PKOD = p.KOD  ";
            sql += "where " + TABGenel.KadroDurumuAktifler("k.KADRODURUMU") + " and p.KOD = " + VeriErisim.DegerYap(pkod);
            sql += " ORDER BY case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, u.AD, p.AD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader LojmanListesiRaporu(string personelKod, string personelTur, string organizasyonKopruKodu, int aktif, int siteKod, int tip, int siralamaListesi, int yil1, int yil2)
        {
            string oturduguGunSorgu = "(SELECT SUM(dbo.gunDondur(IC.BasTarihi, IC.BitTarihi, 1, GetDate(), 0)) FROM IKYPERSONELLOJMAN IC where IC.PKOD=l.PKOD And IC.TahsisTip=1)";
            string oturduguGunAlan = oturduguGunSorgu + "  oturGun";
            string sql = "SELECT p.AD+' '+p.SOYAD, i.EPOSTA, i.CEPTELEFON, l.BasTarihi, d.DaireNo, b.BlokNo, s.SITEADI, so.KAYITAD, " + oturduguGunAlan + ", l.RezervTarihi, u.AD, lt.ETIKET";
            if ((int)ENUMTahsisTipi.Rezervasyon == tip)
                sql += ", (select count(*) from IKYPERSONELLOJMAN where PKOD=p.KOD and TahsisTip=1) as oturdugulojmanSayisi, l.OncelikDerecesi";
            sql += " FROM IKYPERSONELLOJMAN l";
            sql += " inner join IKYLOJMANDAIRE d on d.KOD = l.DKOD";
            sql += " inner join IKYLOJMANBLOK b on b.KOD = d.BKOD";
            sql += " inner join IKYLOJMANSITE s on s.KOD = b.SKOD";
            sql += " inner join IKYPERSONEL p on p.KOD = l.PKOD";
            sql += " left outer join IKYSOZLUKLOJMANTIPI lt on l.LojmanTipi = lt.KOD";
            sql += " left outer join IKYPERSONELILETISIM i on i.KOD = p.KOD";
            sql += " left outer join IKYSOZLUK so on so.KOD = d.DaireTuru";
            sql += " left outer join IKYPERSONELKADRO pk on pk.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU");
            sql += " left outer join IKYUNVAN u on u.KOD = pk.UNVANKOD ";
            sql += " left outer join Kullanici.dbo.ORGBIRIM o on o.kod = pk.ORGANIZASYONKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", "l.TahsisTip = " + tip);

            if ((int)ENUMTahsisTipi.Tahsis == tip)
                sql += " AND l.BitTarihi is null ";

            if (siteKod > 0)
                sql += " AND s.KOD = " + siteKod;

            if (yil1 >= 0)
            {
                sql += " AND " + oturduguGunSorgu + " >= " + yil1 * 360;
            }
            if (yil2 >= 0)
            {
                sql += " AND " + oturduguGunSorgu + " <= " + yil2 * 360;
            }
            string sira;
            if (siralamaListesi == 1)
                sira = " case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD, p.AD + ' ' + p.SOYAD, l.RezervTarihi desc";
            else if (siralamaListesi == 2)
                sira = " o.KOPRUKOD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, p.AD + ' ' + p.SOYAD, l.RezervTarihi desc";
            else if (siralamaListesi == 3)
                sira = " p.AD + ' ' + p.SOYAD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD, l.RezervTarihi desc";
            else if (siralamaListesi == 4)
                sira = " l.OncelikDerecesi, p.AD+' '+p.SOYAD, case when u.SIRANO IS NULL then 999999999 when u.SIRANO=0 then 999999998 else u.SIRANO end, o.KOPRUKOD, l.RezervTarihi desc ";
            else if (siralamaListesi == 5)
                sira = " oturGun desc ";
            else
                sira = " s.KOD, b.BlokNo, d.DaireNo desc ";

            if (siralamaListesi >= 1 && siralamaListesi <= 5)
                sira += ", s.KOD ASC, b.BlokNo ASC, d.DaireNo DESC";

            sql += " ORDER BY " + sira;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader PersonelSiteKesintiRaporu(string personelKod, string personelTur, string organizasyonKopruKodu, int aktif, int yil, int ay, int siteKod)
        {
            string kosul = "";
            string sql = @" select pl.kod, sk.PKOD, p.AD+' '+p.SOYAD, i.EPOSTA, u.AD, sg.AD, sk.TUTAR, p.TCKIMLIKNO, d.DaireNo, b.BlokNo, 
                            s.SITEADI, sk.YIL,sk.AY, sg.KOD, s.KOD
                            from IKYPERSONELKESINTI sk
                            inner join IKYPERSONELLOJMAN pl on pl.KOD = sk.PLOJMANKOD
                            inner join [dbo].[IKYLOJMANDAIRE] d on d.KOD = pl.DKOD
                            inner join [dbo].[IKYLOJMANBLOK] b on b.KOD = d.BKOD
                            inner join [dbo].[IKYLOJMANSITE] s on s.KOD = b.SKOD
                            inner join IKYPERSONEL p on p.KOD = pl.PKOD
                            left outer join IKYPERSONELILETISIM i on i.KOD = p.KOD
                            left outer join IKYPERSONELKADRO pk on pk.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU") +
                            @" left outer join [Kullanici].[dbo].[ORGBIRIM] o on o.kod = pk.ORGANIZASYONKOD
                            left outer join IKYUNVAN u on u.KOD = pk.UNVANKOD 
                            inner join IKYSOZLUKGIDER sg on sg.KOD = sk.GIDERKOD";

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                kosul += BaglacDondur(kosul) + " o.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            kosul += IN_KosulOl(BaglacDondur(kosul), "pk.PERSONELTUR", personelTur, false);
            kosul += IN_KosulOl(BaglacDondur(kosul), "p.KOD", personelKod, false);

            //if (aktif > -1)
            //    kosul += BaglacDondur(kosul) + " p.AKTIFMI = " + aktif;

            if (yil > 0)
                kosul += BaglacDondur(kosul) + " sk.YIL = " + yil;

            if (ay > 0)
                kosul += BaglacDondur(kosul) + " sk.AY = " + ay;

            if (siteKod > 0)
                kosul += BaglacDondur(kosul) + " s.KOD = " + siteKod;

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += kosul;

            sql += " ORDER BY sk.YIL desc, sk.AY Desc, s.KOD, b.BlokNo, d.DaireNo, p.Ad ";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader LojmanDaireBazliRaporu(int siteKod, int daireBosluk)
        {
            string sql = "";
            string oturduguGunAlan = "(SELECT sum(dbo.gunDondur(IC.BasTarihi, IC.BitTarihi, 1, GetDate(), 0)) FROM IKYPERSONELLOJMAN IC where IC.PKOD=l.PKOD And IC.TahsisTip=1) oturGun";

            sql = @"select d.KOD, d.DaireNo, so.KAYITAD daireTuru, d.DaireMetreKare, b.BlokNo, s.SITEADI, p.AD+' '+p.SOYAD personelAd, s.KOD, l.BitTarihi, " + oturduguGunAlan + ", u.AD, lt.ETIKET";
            sql += " from IKYLOJMANDAIRE d ";
            sql += " inner join IKYLOJMANBLOK b on d.BKOD = b.KOD";
            sql += " inner join IKYLOJMANSITE s on s.KOD = b.SKOD ";
            sql += " left outer join IKYSOZLUK so on so.KOD = d.DaireTuru ";
            sql += " left outer join IKYPERSONELLOJMAN l on l.DKOD = d.KOD";
            sql += " left outer join IKYSOZLUKLOJMANTIPI lt on l.LojmanTipi = lt.KOD";
            sql += " left outer join IKYPERSONEL p on p.KOD = l.PKOD";
            sql += " left outer join IKYPERSONELKADRO pk on pk.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU");
            sql += " left outer join IKYUNVAN u on u.KOD = pk.UNVANKOD ";

            if (siteKod > 0)
            {
                sql += " and s.KOD = " + siteKod;
            }

            sql += " ORDER BY s.KOD ASC, b.BlokNo ASC, d.DaireNo DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        //        public IDataReader LojmanDegisiklikListesiRaporu(int aktif, int siteKod)
        //        {
        //            string sql = @"select p.AD+' '+p.SOYAD+' '+p.TCKIMLIKNO, i.EPOSTA, i.CEPTELEFON, l.BasTarihi, d.DaireNo, b.BlokNo, s.SITEADI, 
        //                                  so.KAYITAD, l.BitTarihi, p.KOD
        //                                from IKYPERSONELLOJMAN l
        //                                inner join [dbo].[IKYLOJMANDAIRE] d on d.KOD = l.DKOD
        //                                inner join [dbo].[IKYLOJMANBLOK] b on b.KOD = d.BKOD
        //                                inner join [dbo].[IKYLOJMANSITE] s on s.KOD = b.SKOD
        //                                inner join [dbo].[IKYPERSONEL] p on p.KOD = l.PKOD
        //                                left outer join [dbo].[IKYPERSONELILETISIM] i on i.KOD = p.KOD
        //                                left outer join [dbo].[IKYSOZLUK] so on so.KOD = d.DaireTuru
        //                                where PKOD in (
        //                                select pkod
        //                                from IKYPERSONELLOJMAN 
        //                                WHERE TahsisTip = " + (int)ENUMTahsisTipi.Tahsis +
        //                              @"  group by PKOD
        //                                having count(*)>1
        //                                ) and 
        //DATEDIFF(day,(select max(BitTarihi) from IKYPERSONELLOJMAN where l.PKOD = PKOD and BitTarihi is not null), 
        //(select BasTarihi from IKYPERSONELLOJMAN where l.PKOD = PKOD and BitTarihi is null)) < 360
        //and l.TahsisTip = " + (int)ENUMTahsisTipi.Tahsis;

        //            if (aktif > -1)
        //                sql += " AND p.AKTIFMI = " + aktif;

        //            if (siteKod > 0)
        //                sql += " AND s.KOD = " + siteKod;

        //            //sql += " order by p.KOD, s.KOD, l.BasTarihi";
        //            sql += " order by p.AD+' '+p.SOYAD+p.TCKIMLIKNO, s.SITEADI, l.BasTarihi";
        //            return veriErisim.DataReaderDondur(sql);
        //        }

        //        public IDataReader YillarArasiLojmandaOturanlarListesiRaporu(int aktif, int siteKod, int yil1, int yil2)
        //        {
        //            string sql = @"select p.AD+' '+p.SOYAD, i.EPOSTA, i.CEPTELEFON, l.BasTarihi, d.DaireNo, b.BlokNo, s.SITEADI, so.KAYITAD, l.BitTarihi
        //, DATEDIFF(day, l.BasTarihi, (select case when l.BitTarihi is null then GETDATE() else l.BitTarihi end ))
        //from IKYPERSONELLOJMAN l
        //inner join [dbo].[IKYLOJMANDAIRE] d on d.KOD = l.DKOD
        //inner join [dbo].[IKYLOJMANBLOK] b on b.KOD = d.BKOD
        //inner join [dbo].[IKYLOJMANSITE] s on s.KOD = b.SKOD
        //inner join [dbo].[IKYPERSONEL] p on p.KOD = l.PKOD
        //left outer join [dbo].[IKYPERSONELILETISIM] i on i.KOD = p.KOD
        //left outer join [dbo].[IKYSOZLUK] so on so.KOD = d.DaireTuru
        //where l.TahsisTip =  " + (int)ENUMTahsisTipi.Tahsis +
        //" and DATEDIFF(day, l.BasTarihi, (select case when l.BitTarihi is null then GETDATE() else l.BitTarihi end )) / 360 >= " + yil1 +
        //" and DATEDIFF(day, l.BasTarihi, (select case when l.BitTarihi is null then GETDATE() else l.BitTarihi end )) / 360 <= " + yil2;
        //            //" and DATEDIFF(YEAR, l.BasTarihi, (select case when l.BitTarihi is null then GETDATE() else l.BitTarihi end )) between "+ yil1 + " and " + yil2;

        //            if (aktif > -1)
        //                sql += " AND p.AKTIFMI = " + aktif;

        //            if (siteKod > 0)
        //                sql += " AND s.KOD = " + siteKod;

        //            sql += " order by DATEDIFF(day, l.BasTarihi, (select case when l.BitTarihi is null then GETDATE() else l.BitTarihi end )) desc";
        //            return veriErisim.DataReaderDondur(sql);
        //        }

        public IDataReader YemekhanelereGoreYemekListesi(string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik,
                                                         string yemekhaneler, DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            string sql = @" SELECT ky.kodYerleskeBlokAdi yemekhane, p.KOD pkod, p.AD+' '+p.SOYAD ad, i.EPOSTA, u.AD unvan, pt.AD personeltipi
                            ,SUM(INDIRIMLISAY)+SUM(INDIRIMSIZSAY) toplam
                            ,SUM(INDIRIMLISAY) indirimli
                            ,SUM(INDIRIMSIZSAY) indirimsiz
                            FROM yemekHareketTutar
                            inner join kodYerleskeBlok ky on ky.pkKodYerleskeBlok = yemekHareketTutar.yemekHareketKodYemekhanePk
                            inner join IKYPERSONEL p on p.KOD = yemekHareketTutar.yemekHareketPersonelPk
                            left outer join IKYPERSONELILETISIM i on i.KOD = p.KOD
                            left outer join IKYPERSONELKADRO pk on pk.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU") +
                         @" left outer join IKYUNVAN u on u.KOD = pk.UNVANKOD 
                            left outer join IKYPERSONELTIPI pt on pt.KOD = pk.PERSONELTUR
                            left outer join Kullanici.dbo.ORGBIRIM org on org.KOD = pk.ORGANIZASYONKOD ";

            if (baslangicTarihi.Year < 1900)
                baslangicTarihi = DateTime.Now.Date;

            if (bitisTarihi.Year < 1900)
                bitisTarihi = DateTime.Now.Date;

            TNSDateTime TNSbitisTarihi = new TNSDateTime(bitisTarihi.AddDays(1));
            TNSDateTime TNSbaslangicTarihi = new TNSDateTime(baslangicTarihi);

            sql += " WHERE yemekHareketTutar.yemekHareketTarih < " + VeriErisim.DegerYap(TNSbitisTarihi) + " and yemekHareketTutar.yemekHareketTarih >= " + VeriErisim.DegerYap(TNSbaslangicTarihi);

            if (aktiflik > -1)
                sql += " and p.AKTIFMI = " + aktiflik;

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND org.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            sql += IN_KosulOl("AND", "pk.PERSONELTUR", personelTur, false);
            sql += IN_KosulOl("AND", "p.KOD", personelKod, false);
            sql += IN_KosulOl("AND", "ky.pkKodYerleskeBlok", yemekhaneler, false);

            sql += @" group by ky.kodYerleskeBlokAdi , p.KOD , p.AD+' '+p.SOYAD , i.EPOSTA, u.AD , pt.AD 
                      order by 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader PersonelHesaplanmisYemekListesi(string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik,
                                                         string yemekhaneler, int yil, int ay)
        {
            string sql = @" SELECT p.KOD, p.AD+' '+p.SOYAD , i.EPOSTA, u.AD , pt.AD
                            ,SUM(INDIRIMLISAY)+SUM(INDIRIMSIZSAY) toplamSayi
                            ,SUM(INDIRIMLISAY) indirimliSayi
                            ,SUM(INDIRIMSIZSAY) indirimsizSayi
                            ,SUM(INDIRIMLITUTAR) + SUM(INDIRIMSIZTUTAR) toplamTutar
                            ,SUM(INDIRIMLITUTAR) indirimliTutar
                            ,SUM(INDIRIMSIZTUTAR) indirimsizTutar
                            FROM yemekHareketTutar
                            inner join IKYPERSONEL p on p.KOD = yemekHareketTutar.yemekHareketPersonelPk
                            inner join kodYerleskeBlok ky on ky.pkKodYerleskeBlok = yemekHareketTutar.yemekHareketKodYemekhanePk
                            left outer join IKYPERSONELILETISIM i on i.KOD = p.KOD
                            left outer join IKYPERSONELKADRO pk on pk.PKOD = p.KOD and " + TABGenel.KadroDurumuAktifler("KADRODURUMU") +
                         @" left outer join IKYUNVAN u on u.KOD = pk.UNVANKOD 
                            left outer join IKYPERSONELTIPI pt on pt.KOD = pk.PERSONELTUR
                            left outer join Kullanici.dbo.ORGBIRIM org on org.KOD = pk.ORGANIZASYONKOD ";

            if (yil == 0)
                yil = DateTime.Now.Date.Year;

            if (ay == 0)
                ay = DateTime.Now.Date.Month;

            sql += " WHERE DATEPART(YEAR,  yemekHareketTarih) = '" + yil + "' and DATEPART(MONTH,  yemekHareketTarih) = '" + ay + "'";

            if (aktiflik > -1)
                sql += " and p.AKTIFMI = " + aktiflik;

            if (!string.IsNullOrWhiteSpace(organizasyonKopruKodu) && organizasyonKopruKodu != "%")
                sql += " AND org.KOPRUKOD LIKE " + VeriErisim.DegerYap(organizasyonKopruKodu);

            sql += IN_KosulOl("AND", "pk.PERSONELTUR", personelTur, false);
            sql += IN_KosulOl("AND", "p.KOD", personelKod, false);
            sql += IN_KosulOl("AND", "ky.pkKodYerleskeBlok", yemekhaneler, false);

            sql += @" group by p.KOD, p.AD+' '+p.SOYAD , i.EPOSTA, u.AD , pt.AD 
                      order by 1";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

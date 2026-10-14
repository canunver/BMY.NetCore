using System;
using System.Collections;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.UZY;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Koordinasyon raporu (TavanEkoFaaliyet sayfasındaki "İmzalı Çıktı"; eski Rapor.xml 7500-6 → ButceHazirlikv2.GorusmeRaporu,
    /// EKBILGI=6, şablon BHMv2\KoordinasyonRapor.xltx). Eski GorusmeRaporu.KoordinasyonRapor kodunun aynısıdır; yalnızca
    /// Raporlama altyapısı yerine doğrudan çağrılır ve sonuç dosyası şablonun yanına değil geçici klasöre yazılır.
    /// </summary>
    public class KoordinasyonRaporu
    {
        int yil;
        private int eko2Uz;
        string peb;
        TNS.UZY.IUZYServis uzyServis = null;

        /// <param name="peb">Birim (PEB) kodu; boşsa bütün birimler ve sonda genel toplam.</param>
        public static string Hazirla(int yil, string peb, string sablonAd, string kurumAd)
        {
            KoordinasyonRaporu r = new KoordinasyonRaporu();
            r.yil = yil;
            r.peb = peb ?? "";
            r.uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            r.eko2Uz = t.KodUzunlukBul(3, 2, true);
            return r.KoordinasyonRapor(sablonAd, DosyaIslem.DosyaAdUret("Koordinasyon_", ""), kurumAd);
        }

        private string KoordinasyonRapor(string sablonAd, string sonucDosyaAd, string kurumAd)
        {
            string sqlO = $"SELECT LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD, TG.KURKOD, TG.FONKOD, TG.FINKOD, LEFT(TG.EKOKOD, {eko2Uz}), SUM(TG.ODENEK), e2.EKOAD, p8.PEBAD, p2.PEBAD, P.AD, S.SEKTORAD, P.BITISYILI, P.BASLANGICYILI, e3.EKOAD";
            sqlO += " FROM ButceHazirlikV2.dbo.BHMODENEK AS TG";
            sqlO += " LEFT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS KAYPEB ON TG.YIL = KAYPEB.YIL AND TG.PEBKOD = KAYPEB.PEBKOD AND TG.SENARYOKOD = KAYPEB.SENARYOKOD";
            sqlO += " LEFT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS KAY ON KAY.YIL = KAYPEB.YIL AND KAY.KAYNAKNO = KAYPEB.KAYNAKNO AND KAY.SENARYOKOD = KAYPEB.SENARYOKOD";
            sqlO += " LEFT OUTER JOIN Yatirim.dbo.YATALTPROJE AS AP ON KAY.ALTPROJENO = AP.ALTPROJENO AND KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD";
            sqlO += " LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON AP.PROJENO = P.PROJENO AND AP.SENARYOKOD = P.SENARYOKOD AND AP.YIL = P.YIL";
            sqlO += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSSEKTOR AS S ON P.SEKTORNO = S.SEKTORKOD AND KAY.YIL = S.YIL";
            sqlO += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD AS p8 ON TG.PEBKOD = p8.PEBKOD AND TG.YIL = p8.YIL";
            sqlO += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD AS p2 ON TG.YIL = p2.YIL AND LEFT(TG.PEBKOD,2) = p2.PEBKOD";
            sqlO += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO AS e2 ON TG.YIL = e2.YIL AND LEFT(TG.EKOKOD,2) = e2.EKOKOD";
            sqlO += $" LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO AS e3 ON TG.YIL = e3.YIL AND LEFT(TG.EKOKOD, {eko2Uz}) = e3.EKOKOD";
            sqlO += " WHERE (TG.YIL = {0}) AND (TG.TEKLIFYIL = {2}) AND (TG.SENARYOKOD = '{1}') ";

            if (peb != "")
                sqlO += " AND (LEFT(TG.PEBKOD,2) = '" + peb.Replace(".", "").Replace("'", "") + "') ";
            sqlO += $" group by LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD, TG.KURKOD, TG.FONKOD, TG.FINKOD, LEFT(TG.EKOKOD, {eko2Uz}), e2.EKOAD, p8.PEBAD, p2.PEBAD, P.AD, S.SEKTORAD, P.BITISYILI, P.BASLANGICYILI, e3.EKOAD ";
            sqlO += $" order by LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD, TG.KURKOD, TG.FONKOD, TG.FINKOD, LEFT(TG.EKOKOD, {eko2Uz})";

            string sqlE = $"SELECT LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD, TG.KURKOD, TG.FONKOD, TG.FINKOD, LEFT(TG.EKOKOD, {eko2Uz}), SUM(TG.ODENEK), SUM(TG.ODENEK1), SUM(TG.ODENEK2), e2.EKOAD, p8.PEBAD, p2.PEBAD, P.AD, S.SEKTORAD, P.BITISYILI, P.BASLANGICYILI, e3.EKOAD";
            sqlE += " FROM ButceHazirlikV2.dbo.BHMTAVANUSTU AS TG";
            sqlE += " LEFT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS KAYPEB ON TG.YIL = KAYPEB.YIL AND TG.PEBKOD = KAYPEB.PEBKOD AND TG.SENARYOKOD = KAYPEB.SENARYOKOD";
            sqlE += " LEFT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAK AS KAY ON KAY.YIL = KAYPEB.YIL AND KAY.KAYNAKNO = KAYPEB.KAYNAKNO AND KAY.SENARYOKOD = KAYPEB.SENARYOKOD";
            sqlE += " LEFT OUTER JOIN Yatirim.dbo.YATALTPROJE AS AP ON KAY.ALTPROJENO = AP.ALTPROJENO AND KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD";
            sqlE += " LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON AP.PROJENO = P.PROJENO AND AP.SENARYOKOD = P.SENARYOKOD AND AP.YIL = P.YIL";
            sqlE += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSSEKTOR AS S ON P.SEKTORNO = S.SEKTORKOD AND KAY.YIL = S.YIL";
            sqlE += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD AS p8 ON TG.PEBKOD = p8.PEBKOD AND TG.YIL = p8.YIL";
            sqlE += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD AS p2 ON TG.YIL = p2.YIL AND LEFT(TG.PEBKOD,2) = p2.PEBKOD";
            sqlE += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO AS e2 ON TG.YIL = e2.YIL AND LEFT(TG.EKOKOD,2) = e2.EKOKOD";
            sqlE += $" LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO AS e3 ON TG.YIL = e3.YIL AND LEFT(TG.EKOKOD, {eko2Uz}) = e3.EKOKOD";
            sqlE += " WHERE (TG.YIL = {0}) AND (TG.SENARYOKOD = '{1}') ";

            if (peb != "")
                sqlE += " AND (LEFT(TG.PEBKOD,2) = '" + peb.Replace(".", "").Replace("'", "") + "') ";
            sqlE += $" group by LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD, TG.KURKOD, TG.FONKOD, TG.FINKOD, LEFT(TG.EKOKOD, {eko2Uz}), e2.EKOAD, p8.PEBAD, p2.PEBAD, P.AD, S.SEKTORAD, P.BITISYILI, P.BASLANGICYILI, e3.EKOAD ";
            sqlE += $" order by LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD, TG.KURKOD, TG.FONKOD, TG.FINKOD, LEFT(TG.EKOKOD, {eko2Uz})";

            string[] sorgular = new string[4];
            string[] baglantilar = new string[4];

            sorgular[0] = String.Format(sqlO, yil, "0000", 0);
            sorgular[1] = String.Format(sqlO, yil, "0000", 1);
            sorgular[2] = String.Format(sqlO, yil, "0000", 2);
            sorgular[3] = String.Format(sqlE, yil, "0000");
            baglantilar[0] = "BaglantiSatiriBHM";
            baglantilar[1] = "BaglantiSatiriBHM";
            baglantilar[2] = "BaglantiSatiriBHM";
            baglantilar[3] = "BaglantiSatiriBHM";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0;
            int fSat1 = 0;
            int fSat2 = 0;
            int fSut1 = 0;
            int fSut2 = 0;
            int baslaSat = 0;
            int baslaSut = 0;

            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("KurumAd", kurumAd);
            int eskiBirimSat = -1;
            string eskiBirim = "";

            int eskiEkoSat = -1;
            string eskiEko = "";

            int eskiSektorSat = -1;
            string eskiSektor = "";

            int eskiProjeSat = -1;
            string eskiProje = "";
            string eskiFaal = "";
            int eskiFaalSat = -1;

            ToplamAl toplamAl = new ToplamAl(7, 6);
            int satir = baslaSat;

            SortedList sl = uzyServis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string birimKod = BossaDoldur(alAnahtar.StrAl(0));
                string ekoKod = BossaDoldur(alAnahtar.StrAl(1));
                string sektorKod = OrtakFonksiyonlar.KodNoktaKoy("99.99", BossaDoldur(alAnahtar.StrAl(2)));
                string projeNo = BossaDoldur(alAnahtar.StrAl(3));
                string faalKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(4), BossaDoldur(alAnahtar.StrAl(4)));
                string kurKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), BossaDoldur(alAnahtar.StrAl(5)));
                string fonKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(1), BossaDoldur(alAnahtar.StrAl(6)));
                string finKod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(2), BossaDoldur(alAnahtar.StrAl(7)));
                string ekoKod3 = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(3), BossaDoldur(alAnahtar.StrAl(8)));

                toplamAl.DegerAta(6, 0, alVeri.DblAl(0));
                toplamAl.DegerAta(6, 2, alVeri.DblAl(9));
                toplamAl.DegerAta(6, 4, alVeri.DblAl(18));
                toplamAl.DegerAta(6, 1, alVeri.DblAl(27));
                toplamAl.DegerAta(6, 3, alVeri.DblAl(28));
                toplamAl.DegerAta(6, 5, alVeri.DblAl(29));

                //0SUM(TG.ODENEK), 1e2.EKOAD, 2p8.PEBAD, 3p2.PEBAD, 4P.AD, 5S.SEKTORAD, 6P.BITISYILI, 7P.BASLANGICYILI, 8e3.EKOAD

                string ekoAd = alVeri_StrAl(alVeri, 1, 10, 19, 30);
                string faalAd = alVeri_StrAl(alVeri, 2, 11, 20, 31);
                string birimAd = alVeri_StrAl(alVeri, 3, 12, 21, 32);
                string projeAd = alVeri_StrAl(alVeri, 4, 13, 22, 33);
                string sektorAd = alVeri_StrAl(alVeri, 5, 14, 23, 34);
                string ekoAd3 = alVeri_StrAl(alVeri, 8, 17, 26, 37);
                if (CariMi(ekoKod))
                {
                    if (sektorKod == "-")
                    {
                        sektorKod = "00";
                        sektorAd = "Cari";
                    }
                    if (projeNo == "-")
                    {
                        projeNo = "Cari";
                        projeAd = "Cari";
                    }
                }
                if (eskiBirim != birimKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        RakamYazKoor(XLS, eskiBirimSat, baslaSut + 7, toplamAl, 1);
                        toplamAl.SatirSifirla(1);
                    }

                    XLS.HucreKopyala(fSat1, fSut1, fSat1, fSut1 + 25, satir, baslaSut);
                    OnYaz(XLS, satir, 0, birimKod, "", "", "", "", "", "", "", "");
                    XLS.HucreDegerYaz(satir, baslaSut, birimKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 6, birimAd);
                    eskiBirim = birimKod;
                    eskiBirimSat = satir;
                    satir++;
                    eskiEko = "";
                }
                if (eskiEko != ekoKod)
                {
                    if (eskiEkoSat > 0)
                    {
                        RakamYazKoor(XLS, eskiEkoSat, baslaSut + 7, toplamAl, 2);
                        toplamAl.SatirSifirla(2);
                    }

                    XLS.HucreKopyala(fSat1 + 1, fSut1, fSat1 + 1, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 1, ekoKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 6, ekoAd);
                    OnYaz(XLS, satir, 0, birimKod, ekoKod, "", "", "", "", "", "", "");
                    eskiEko = ekoKod;
                    eskiEkoSat = satir;
                    satir++;
                    eskiSektor = "";
                }

                if (eskiSektor != sektorKod)
                {
                    if (eskiSektorSat > 0)
                    {
                        RakamYazKoor(XLS, eskiSektorSat, baslaSut + 7, toplamAl, 3);
                        toplamAl.SatirSifirla(3);
                    }

                    XLS.HucreKopyala(fSat1 + 2, fSut1, fSat1 + 2, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 2, sektorKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 6, sektorAd);
                    OnYaz(XLS, satir, 0, birimKod, ekoKod, sektorKod, "", "", "", "", "", "");
                    eskiSektor = sektorKod;
                    eskiSektorSat = satir;
                    satir++;
                    eskiProje = "";
                }

                if (eskiProje != projeNo)
                {
                    if (eskiProjeSat > 0)
                    {
                        RakamYazKoor(XLS, eskiProjeSat, baslaSut + 7, toplamAl, 4);
                        toplamAl.SatirSifirla(4);
                    }

                    XLS.HucreKopyala(fSat1 + 3, fSut1, fSat1 + 3, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 3, projeNo);
                    XLS.HucreDegerYaz(satir, baslaSut + 6, projeAd);
                    OnYaz(XLS, satir, 0, birimKod, ekoKod, sektorKod, projeNo, "", "", "", "", "");
                    eskiProje = projeNo;
                    eskiProjeSat = satir;
                    satir++;
                    eskiFaal = "";
                }

                if (eskiFaal != faalKod)
                {
                    if (eskiFaalSat > 0)
                    {
                        RakamYazKoor(XLS, eskiFaalSat, baslaSut + 7, toplamAl, 5);
                        toplamAl.SatirSifirla(5);
                    }

                    XLS.HucreKopyala(fSat1 + 4, fSut1, fSat1 + 4, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 4, faalKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 6, faalAd);
                    OnYaz(XLS, satir, 0, birimKod, ekoKod, sektorKod, projeNo, faalKod, "", "", "", "");
                    eskiFaal = faalKod;
                    eskiFaalSat = satir;
                    satir++;
                }
                XLS.HucreKopyala(fSat1 + 5, fSut1, fSat1 + 5, fSut1 + 25, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut + 5, kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod3);
                OnYaz(XLS, satir, 0, birimKod, ekoKod, sektorKod, projeNo, faalKod, kurKod, fonKod, finKod, ekoKod3);
                XLS.HucreDegerYaz(satir, baslaSut + 6, ekoAd3);
                RakamYazKoor(XLS, satir, baslaSut + 7, toplamAl, 6);
                toplamAl.ToplamEkle(5, 6);
                toplamAl.ToplamEkle(4, 6);
                toplamAl.ToplamEkle(3, 6);
                toplamAl.ToplamEkle(2, 6);
                toplamAl.ToplamEkle(1, 6);
                toplamAl.ToplamEkle(0, 6);
                satir++;
            }

            if (eskiBirimSat > 0)
            {
                RakamYazKoor(XLS, eskiBirimSat, baslaSut + 7, toplamAl, 1);
                toplamAl.SatirSifirla(1);
            }
            if (eskiEkoSat > 0)
            {
                RakamYazKoor(XLS, eskiEkoSat, baslaSut + 7, toplamAl, 2);
                toplamAl.SatirSifirla(2);
            }
            if (eskiSektorSat > 0)
            {
                RakamYazKoor(XLS, eskiSektorSat, baslaSut + 7, toplamAl, 3);
                toplamAl.SatirSifirla(3);
            }
            if (eskiProjeSat > 0)
            {
                RakamYazKoor(XLS, eskiProjeSat, baslaSut + 7, toplamAl, 4);
                toplamAl.SatirSifirla(4);
            }
            if (eskiFaalSat > 0)
            {
                RakamYazKoor(XLS, eskiFaalSat, baslaSut + 7, toplamAl, 5);
                toplamAl.SatirSifirla(5);
            }
            if (peb == "")
            {
                XLS.HucreKopyala(fSat1, fSut1, fSat1, fSut1 + 25, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut, "");
                XLS.HucreDegerYaz(satir, baslaSut + 6, "T O P L A M");
                RakamYazKoor(XLS, satir, baslaSut + 7, toplamAl, 0);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>Eski BHMv2Ortak.CariMi.</summary>
        private static bool CariMi(string kod)
        {
            return kod.StartsWith("01") || kod.StartsWith("02") || kod.StartsWith("03") || kod.StartsWith("05");
        }

        private string alVeri_StrAl(HTArray alVeri, params int[] sira)
        {
            for (int i = 0; i < sira.Length; i++)
            {
                string oku = alVeri.StrAl(sira[i]);
                if (!string.IsNullOrWhiteSpace(oku)) return oku;
            }
            return "";
        }

        private void OnYaz(Tablo XLS, int sat, int sut, string birimKod, string ekoKod, string sektorKod, string projeNo, string faalKod, string kurKod, string fonKod, string finKod, string ekoKod3)
        {
            XLS.HucreDegerYaz(sat, sut + 0, birimKod);
            XLS.HucreDegerYaz(sat, sut + 1, ekoKod);
            XLS.HucreDegerYaz(sat, sut + 2, sektorKod);
            XLS.HucreDegerYaz(sat, sut + 3, projeNo);
            XLS.HucreDegerYaz(sat, sut + 4, faalKod);
            XLS.HucreDegerYaz(sat, sut + 5, kurKod);
            XLS.HucreDegerYaz(sat, sut + 6, fonKod);
            XLS.HucreDegerYaz(sat, sut + 7, finKod);
            XLS.HucreDegerYaz(sat, sut + 8, ekoKod3);
        }

        private string BossaDoldur(string p)
        {
            if (string.IsNullOrWhiteSpace(p)) return "-";
            return p;
        }

        private void RakamYazKoor(Tablo XLS, int sat, int sut, ToplamAl toplamAl, int tSat)
        {
            XLS.HucreDegerYaz(sat, sut + 0, toplamAl.toplam[tSat, 0]);
            XLS.HucreDegerYaz(sat, sut + 1, toplamAl.toplam[tSat, 1]);
            XLS.HucreDegerYaz(sat, sut + 2, toplamAl.toplam[tSat, 2]);
            XLS.HucreDegerYaz(sat, sut + 3, toplamAl.toplam[tSat, 3]);
            XLS.HucreDegerYaz(sat, sut + 4, toplamAl.toplam[tSat, 4]);
            XLS.HucreDegerYaz(sat, sut + 5, toplamAl.toplam[tSat, 5]);
        }
    }
}

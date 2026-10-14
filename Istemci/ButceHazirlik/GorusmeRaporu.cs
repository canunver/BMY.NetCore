using System;
using System.Collections;
using System.Collections.Generic;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.KYM;
using TNS.UZY;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Görüşme sayfalarının (Görüşme Cari, Yatırım Görüşme, Vize Tavanı) "Rapor Al" menüsü: eski BHGiderRaporlariEkle(menu, false)
    /// raporları ve sayfanın eklediği 7500 raporları (eski ListelemeMenuRaporSenenegi("RaporAl", 7500, n)).
    /// </summary>
    public static class GorusmeRaporlari
    {
        public static List<object> Liste(params string[] ekKodlar)
        {
            List<object> l = BHGiderRaporlari.ListeGider(false);
            foreach (string kod in ekKodlar)
                l.Add(new { kod = kod, ad = BHGiderRaporlari.RaporAd(kod) });
            return l;
        }

        public static bool Tanimli(string kod, params string[] ekKodlar)
        {
            return BHGiderRaporlari.Tanimli(kod) || Array.IndexOf(ekKodlar, kod) >= 0;
        }

        /// <summary>Eski BHMv2Ortak.RaporAl ile aynı parametreler; üretilen dosyanın yolunu döndürür.</summary>
        public static string Al(Kullanici kullanan, string raporKod, int yil, string senaryo, string kur, string fon, string fin, string eko, string peb, string raporSablonYol)
        {
            if (GorusmeRaporu.Tanimli(raporKod))
                return GorusmeRaporu.Hazirla(raporKod, yil, senaryo, peb, raporSablonYol, BHGiderRaporlari.KurumAd());
            return BHGiderRaporlari.RaporAl(kullanan, raporKod, yil, senaryo, kur, fon, fin, eko, peb, raporSablonYol);
        }
    }

    /// <summary>
    /// Birim görüşme raporları (eski Rapor.xml 7500-2..5 → ButceHazirlikv2.GorusmeRaporu): Görüşme Cari sayfasının Rapor Al
    /// menüsündeki 7500-4 / 7500-5 (cari görüşme, şablon BHMv2\CariGorusme.xltx; EKBILGI 3 seçili birim, 4 hepsi) ve Yatırım Görüşme /
    /// Vize sayfalarındaki 7500-2 / 7500-3 (yatırım görüşme, şablon BHMv2\YatirimGorusme.xltx; EKBILGI 1 seçili birim, 2 hepsi).
    /// Eski GorusmeRaporu.CariGorusme / YatirimGorusme kodunun aynısıdır; yalnızca Raporlama altyapısı yerine doğrudan çağrılır ve
    /// sonuç dosyası şablonun yanına değil geçici klasöre yazılır (KoordinasyonRaporu gibi).
    /// </summary>
    public class GorusmeRaporu
    {
        int yil;
        private int eko2Uz;
        string senaryo;
        string peb;
        string ekBilgi;
        TNS.UZY.IUZYServis uzyServis = null;

        /// <summary>Rapor kodu bu sınıfın raporlarından mı (7500-2..5)?</summary>
        public static bool Tanimli(string raporKod)
        {
            return raporKod == "7500-2" || raporKod == "7500-3" || raporKod == "7500-4" || raporKod == "7500-5";
        }

        /// <param name="raporKod">7500-2..5</param>
        /// <param name="senaryo">Eski sayfanın verdiği senaryo (Görüşme Cari ve Yatırım Görüşme "0100", Vize "1000").</param>
        /// <param name="peb">Eski sayfanın verdiği PebKod (birim ya da faaliyet; boşsa bütün birimler).</param>
        public static string Hazirla(string raporKod, int yil, string senaryo, string peb, string raporSablonYol, string kurumAd)
        {
            // Kod SQL'e yazılır: yalnızca rakam ve nokta olabilir.
            if (!System.Text.RegularExpressions.Regex.IsMatch(peb ?? "", @"^[0-9.]*$") || !System.Text.RegularExpressions.Regex.IsMatch(senaryo ?? "", @"^[0-9.]*$"))
                throw new RaporHatasi("Kod hatalı!");
            GorusmeRaporu r = new GorusmeRaporu();
            r.yil = yil;
            r.senaryo = senaryo ?? "";
            r.peb = peb ?? "";
            r.uzyServis = TNS.UZY.Arac.Tanimla();
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            r.eko2Uz = t.KodUzunlukBul(3, 2, true);
            switch (raporKod)
            {
                case "7500-2":
                    r.ekBilgi = "1";
                    return r.YatirimGorusme(raporSablonYol + "/BHMv2/YatirimGorusme.xltx", DosyaIslem.DosyaAdUret("YatirimGorusme_", ""), kurumAd);
                case "7500-3":
                    r.ekBilgi = "2";
                    return r.YatirimGorusme(raporSablonYol + "/BHMv2/YatirimGorusme.xltx", DosyaIslem.DosyaAdUret("YatirimGorusme_", ""), kurumAd);
                case "7500-4":
                    r.ekBilgi = "3";
                    return r.CariGorusme(raporSablonYol + "/BHMv2/CariGorusme.xltx", DosyaIslem.DosyaAdUret("CariGorusme_", ""), kurumAd);
                case "7500-5":
                    r.ekBilgi = "4";
                    return r.CariGorusme(raporSablonYol + "/BHMv2/CariGorusme.xltx", DosyaIslem.DosyaAdUret("CariGorusme_", ""), kurumAd);
            }
            throw new RaporHatasi("Rapor seçimi hatalı!");
        }

        private string CariGorusme(string sablonAd, string sonucDosyaAd, string kurumAd)
        {
            string sql = $"SELECT LEFT(O.PEBKOD, 2), LEFT(O.EKOKOD, 2), O.PEBKOD, O.KURKOD, O.FONKOD, O.FINKOD, LEFT(O.EKOKOD,  {eko2Uz}), SUM(O.ODENEK), P2.PEBAD, P8.PEBAD, ";
            sql += " E2.EKOAD, E3.EKOAD ";
            sql += " FROM ButceHazirlikV2.dbo.BHMODENEK AS O INNER JOIN ";
            sql += " ABSPEBKOD AS P2 ON O.YIL = P2.YIL AND LEFT(O.PEBKOD,2) = P2.PEBKOD INNER JOIN ";
            sql += " ABSPEBKOD AS P8 ON O.YIL = P8.YIL AND O.PEBKOD = P8.PEBKOD INNER JOIN ";
            sql += " ABSEKO AS E2 ON O.YIL = E2.YIL AND LEFT(O.EKOKOD,2) = E2.EKOKOD INNER JOIN ";
            sql += $" ABSEKO AS E3 ON O.YIL = E3.YIL AND LEFT(O.EKOKOD, {eko2Uz}) = E3.EKOKOD ";
            sql += " WHERE (O.YIL = {0}) AND (O.TEKLIFYIL = {1}) AND (O.SENARYOKOD = '{2}') AND (O.EKOKOD LIKE '01%' OR ";
            sql += " O.EKOKOD LIKE '02%' OR ";
            sql += " O.EKOKOD LIKE '03%' OR ";
            sql += " O.EKOKOD LIKE '05%') ";

            if (peb != "" && ekBilgi == "3")
                sql += " AND (LEFT(O.PEBKOD,2) = '" + peb.Replace(".", "") + "') ";

            sql += $" GROUP BY LEFT(O.PEBKOD, 2), LEFT(O.EKOKOD, 2), O.PEBKOD, O.KURKOD, O.FONKOD, O.FINKOD, LEFT(O.EKOKOD,  {eko2Uz}), P2.PEBAD, P8.PEBAD, E2.EKOAD, E3.EKOAD ";
            sql += " ORDER BY 1, 2, O.PEBKOD, O.KURKOD, O.FONKOD, O.FINKOD, 7 ";

            string sql2 = $"SELECT LEFT(O.PEBKOD, 2), LEFT(O.EKOKOD, 2), O.PEBKOD, O.KURKOD, O.FONKOD, O.FINKOD, LEFT(O.EKOKOD,  {eko2Uz}), SUM(O.ODENEK), SUM(O.ODENEK1), SUM(O.ODENEK2)";
            sql2 += " FROM ButceHazirlikV2.dbo.BHMTAVANUSTU O";
            sql2 += " WHERE (O.YIL = {0}) AND (O.SENARYOKOD = '{1}') AND (O.EKOKOD LIKE '01%' OR ";
            sql2 += " O.EKOKOD LIKE '02%' OR ";
            sql2 += " O.EKOKOD LIKE '03%' OR ";
            sql2 += " O.EKOKOD LIKE '05%') ";

            if (peb != "" && ekBilgi == "3")
                sql2 += " AND (LEFT(O.PEBKOD,2) = '" + peb.Replace(".", "") + "') ";

            sql2 += $" GROUP BY LEFT(O.PEBKOD, 2), LEFT(O.EKOKOD, 2), O.PEBKOD, O.KURKOD, O.FONKOD, O.FINKOD, LEFT(O.EKOKOD,  {eko2Uz}) ";
            sql2 += " ORDER BY 1, 2, O.PEBKOD, O.KURKOD, O.FONKOD, O.FINKOD, 7 ";

            string[] sorgular = new string[9];
            string[] baglantilar = new string[9];

            sorgular[0] = String.Format(sql, yil, 0, senaryo.Replace(".", ""));
            sorgular[1] = String.Format(sql, yil, 1, senaryo.Replace(".", ""));
            sorgular[2] = String.Format(sql, yil, 2, senaryo.Replace(".", ""));
            sorgular[3] = String.Format(sql2, yil, senaryo.Replace(".", ""));

            sorgular[4] = String.Format(sql, yil, 0, "0000");
            sorgular[5] = String.Format(sql, yil, 1, "0000");
            sorgular[6] = String.Format(sql, yil, 2, "0000");
            sorgular[7] = String.Format(sql2, yil, "0000");
            sorgular[8] = String.Format(sql, yil - 1, 0, "0600");

            baglantilar[0] = "BaglantiSatiriBHM";
            baglantilar[1] = "BaglantiSatiriBHM";
            baglantilar[2] = "BaglantiSatiriBHM";
            baglantilar[3] = "BaglantiSatiriBHM";
            baglantilar[4] = "BaglantiSatiriBHM";
            baglantilar[5] = "BaglantiSatiriBHM";
            baglantilar[6] = "BaglantiSatiriBHM";
            baglantilar[7] = "BaglantiSatiriBHM";
            baglantilar[8] = "BaglantiSatiriBHM";

            SortedList sl = uzyServis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4, 5, 6 });
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
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

            int eskiFaalSat = -1;
            string eskiFaal = "";

            ToplamAl toplamAl = new ToplamAl(5, 13);
            int satir = baslaSat;

            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string birimKod = alAnahtar.StrAl(0);
                string ekoKod = alAnahtar.StrAl(1);
                string faalKod = alAnahtar.StrAl(2);

                string kurKod = alAnahtar.StrAl(3);
                string fonKod = alAnahtar.StrAl(4);
                string finKod = alAnahtar.StrAl(5);
                string eko2Kod = alAnahtar.StrAl(6);

                toplamAl.DegerAta(4, 0, alVeri.DblAl(0));   //0. sorgu odenek
                string birimAd = alVeri.StrAl(1); //0. sorgu odenek
                string faalAd = alVeri.StrAl(2); //0. sorgu odenek
                string ekoAd = alVeri.StrAl(3); //0. sorgu odenek
                string eko2Ad = alVeri.StrAl(4); //0. sorgu odenek

                toplamAl.DegerAta(4, 1, alVeri.DblAl(5));   //1. sorgu odenek
                if (string.IsNullOrWhiteSpace(birimAd))
                    birimAd = alVeri.StrAl(6); //1. sorgu odenek
                if (string.IsNullOrWhiteSpace(faalAd))
                    faalAd = alVeri.StrAl(7); //1. sorgu odenek
                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(8); //1. sorgu odenek
                if (string.IsNullOrWhiteSpace(eko2Ad))
                    eko2Ad = alVeri.StrAl(9); //1. sorgu odenek

                toplamAl.DegerAta(4, 2, alVeri.DblAl(10));  //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(birimAd))
                    birimAd = alVeri.StrAl(11); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(faalAd))
                    faalAd = alVeri.StrAl(12); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(13); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(eko2Ad))
                    eko2Ad = alVeri.StrAl(14); //2. sorgu odenek

                toplamAl.DegerAta(4, 3, alVeri.DblAl(15));  //3. sorgu odenek
                toplamAl.DegerAta(4, 4, alVeri.DblAl(16));  //3. sorgu odenek
                toplamAl.DegerAta(4, 5, alVeri.DblAl(17));  //3. sorgu odenek

                toplamAl.DegerAta(4, 6, alVeri.DblAl(18));  //4. sorgu odenek
                if (string.IsNullOrWhiteSpace(birimAd))
                    birimAd = alVeri.StrAl(19); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(faalAd))
                    faalAd = alVeri.StrAl(20); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(21); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(eko2Ad))
                    eko2Ad = alVeri.StrAl(22); //2. sorgu odenek

                toplamAl.DegerAta(4, 7, alVeri.DblAl(23));  //5. sorgu odenek
                if (string.IsNullOrWhiteSpace(birimAd))
                    birimAd = alVeri.StrAl(24); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(faalAd))
                    faalAd = alVeri.StrAl(25); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(26); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(eko2Ad))
                    eko2Ad = alVeri.StrAl(27); //2. sorgu odenek


                toplamAl.DegerAta(4, 8, alVeri.DblAl(28));  //6. sorgu odenek
                if (string.IsNullOrWhiteSpace(birimAd))
                    birimAd = alVeri.StrAl(29); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(faalAd))
                    faalAd = alVeri.StrAl(30); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(31); //2. sorgu odenek
                if (string.IsNullOrWhiteSpace(eko2Ad))
                    eko2Ad = alVeri.StrAl(32); //2. sorgu odenek

                toplamAl.DegerAta(4, 9, alVeri.DblAl(33));  //7. sorgu odenek
                toplamAl.DegerAta(4, 10, alVeri.DblAl(34));  //7. sorgu odenek
                toplamAl.DegerAta(4, 11, alVeri.DblAl(35));  //7. sorgu odenek

                toplamAl.DegerAta(4, 12, alVeri.DblAl(36));  //8. sorgu odenek
                if (string.IsNullOrWhiteSpace(birimAd))
                    birimAd = alVeri.StrAl(37); //8. sorgu odenek
                if (string.IsNullOrWhiteSpace(faalAd))
                    faalAd = alVeri.StrAl(38); //8. sorgu odenek
                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(39); //8. sorgu odenek
                if (string.IsNullOrWhiteSpace(eko2Ad))
                    eko2Ad = alVeri.StrAl(40); //8. sorgu odenek

                if (eskiBirim != birimKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        RakamYaz(XLS, eskiBirimSat, baslaSut + 5, toplamAl, 1);
                        toplamAl.SatirSifirla(1);
                    }

                    XLS.HucreKopyala(fSat1, fSut1, fSat1, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, birimKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 4, birimAd);
                    eskiBirim = birimKod;
                    eskiBirimSat = satir;
                    satir++;
                    eskiEko = "";
                }
                if (eskiEko != ekoKod)
                {
                    if (eskiEkoSat > 0)
                    {
                        RakamYaz(XLS, eskiEkoSat, baslaSut + 5, toplamAl, 2);
                        toplamAl.SatirSifirla(2);
                    }

                    XLS.HucreKopyala(fSat1 + 1, fSut1, fSat1 + 1, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 1, ekoKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 4, ekoAd);
                    eskiEko = ekoKod;
                    eskiEkoSat = satir;
                    satir++;
                    eskiFaal = "";
                }

                if (eskiFaal != faalKod)
                {
                    if (eskiFaalSat > 0)
                    {
                        RakamYaz(XLS, eskiFaalSat, baslaSut + 5, toplamAl, 3);
                        toplamAl.SatirSifirla(3);
                    }

                    XLS.HucreKopyala(fSat1 + 2, fSut1, fSat1 + 2, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 2, faalKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 4, faalAd);
                    eskiFaal = faalKod;
                    eskiFaalSat = satir;
                    satir++;
                }

                XLS.HucreKopyala(fSat1 + 3, fSut1, fSat1 + 3, fSut1 + 25, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut + 3, kurKod + "-" + fonKod + "-" + finKod + "-" + eko2Kod);
                XLS.HucreDegerYaz(satir, baslaSut + 4, eko2Ad);
                RakamYaz(XLS, satir, baslaSut + 5, toplamAl, 4);
                toplamAl.ToplamEkle(3, 4);
                toplamAl.ToplamEkle(2, 4);
                toplamAl.ToplamEkle(1, 4);
                toplamAl.ToplamEkle(0, 4);
                satir++;
            }

            if (eskiBirimSat > 0)
            {
                RakamYaz(XLS, eskiBirimSat, baslaSut + 5, toplamAl, 1);
                toplamAl.SatirSifirla(1);
            }

            if (eskiEkoSat > 0)
            {
                RakamYaz(XLS, eskiEkoSat, baslaSut + 5, toplamAl, 2);
                toplamAl.SatirSifirla(2);
            }

            if (eskiFaalSat > 0)
            {
                RakamYaz(XLS, eskiFaalSat, baslaSut + 5, toplamAl, 3);
                toplamAl.SatirSifirla(3);
            }

            if (peb == "" || ekBilgi != "3")
            {
                XLS.HucreKopyala(fSat1, fSut1, fSat1, fSut1 + 25, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut, "");
                XLS.HucreDegerYaz(satir, baslaSut + 4, "T O P L A M");
                RakamYaz(XLS, satir, baslaSut + 5, toplamAl, 0);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string YatirimGorusme(string sablonAd, string sonucDosyaAd, string kurumAd)
        {
            string sql = "SELECT TG.KOD3, TG.KOD1, TG.KOD2, P.DPTPROJENO, TG.KOD4, TG.YIL0, TG.YIL1, TG.YIL2, TG.EK0, TG.EK1, TG.EK2, e2.EKOAD, p8.PEBAD, p2.PEBAD, P.AD, S.SEKTORAD, P.BITISYILI, P.BASLANGICYILI ";
            sql += " FROM ButceHazirlikV2.dbo.BHMTAVANGENEL AS TG LEFT OUTER JOIN ";
            sql += " Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS KAYPEB ON TG.YIL = KAYPEB.YIL AND TG.KOD4 = KAYPEB.PEBKOD AND TG.ASAMA = KAYPEB.SENARYOKOD LEFT OUTER JOIN ";
            sql += " Yatirim.dbo.YATBUTCEKAYNAK AS KAY ON KAY.YIL = KAYPEB.YIL AND KAY.KAYNAKNO = KAYPEB.KAYNAKNO AND KAY.SENARYOKOD = KAYPEB.SENARYOKOD LEFT OUTER JOIN ";
            sql += " Yatirim.dbo.YATALTPROJE AS AP ON KAY.ALTPROJENO = AP.ALTPROJENO AND KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD LEFT OUTER JOIN ";
            sql += " Yatirim.dbo.YATPROJE AS P ON AP.PROJENO = P.PROJENO AND AP.SENARYOKOD = P.SENARYOKOD AND AP.YIL = P.YIL LEFT OUTER JOIN ";
            sql += " ButceHazirlik.dbo.ABSSEKTOR AS S ON P.SEKTORNO = S.SEKTORKOD AND KAY.YIL = S.YIL LEFT OUTER JOIN ";
            sql += " ButceHazirlik.dbo.ABSPEBKOD AS p8 ON TG.KOD4 = p8.PEBKOD AND TG.YIL = p8.YIL LEFT OUTER JOIN ";
            sql += " ButceHazirlik.dbo.ABSPEBKOD AS p2 ON TG.YIL = p2.YIL AND TG.KOD3 = p2.PEBKOD LEFT OUTER JOIN ";
            sql += " ButceHazirlik.dbo.ABSEKO AS e2 ON TG.YIL = e2.YIL AND TG.KOD1 = e2.EKOKOD ";
            sql += " WHERE (TG.YIL = {0}) AND (TG.ASAMA = '{1}') AND (TG.TAVANTUR = 2) AND (TG.KOD1 LIKE '06%' OR TG.KOD1 LIKE '07%' OR TG.KOD1 LIKE '08%') ";
            if (peb != "" && ekBilgi == "1")
                sql += " AND (TG.KOD3 = '" + peb.Replace(".", "") + "') ";
            sql += " order by KOD3, KOD1, KOD2, P.DPTPROJENO, KOD4";


            string sql2 = "SELECT LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD, SUM(TG.ODENEK), e2.EKOAD, p8.PEBAD, p2.PEBAD, P.AD, S.SEKTORAD, P.BITISYILI, P.BASLANGICYILI  "; //
            sql2 += " FROM ButceHazirlikV2.dbo.BHMODENEK AS TG LEFT OUTER JOIN ";
            sql2 += " Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS KAYPEB ON TG.YIL = KAYPEB.YIL AND TG.PEBKOD = KAYPEB.PEBKOD AND TG.SENARYOKOD = KAYPEB.SENARYOKOD LEFT OUTER JOIN ";
            sql2 += " Yatirim.dbo.YATBUTCEKAYNAK AS KAY ON KAY.YIL = KAYPEB.YIL AND KAY.KAYNAKNO = KAYPEB.KAYNAKNO AND KAY.SENARYOKOD = KAYPEB.SENARYOKOD LEFT OUTER JOIN ";
            sql2 += " Yatirim.dbo.YATALTPROJE AS AP ON KAY.ALTPROJENO = AP.ALTPROJENO AND KAY.YIL = AP.YIL AND KAY.SENARYOKOD = AP.SENARYOKOD LEFT OUTER JOIN ";
            sql2 += " Yatirim.dbo.YATPROJE AS P ON AP.PROJENO = P.PROJENO AND AP.SENARYOKOD = P.SENARYOKOD AND AP.YIL = P.YIL LEFT OUTER JOIN ";
            sql2 += " ButceHazirlik.dbo.ABSSEKTOR AS S ON P.SEKTORNO = S.SEKTORKOD AND KAY.YIL = S.YIL LEFT OUTER JOIN ";
            sql2 += " ButceHazirlik.dbo.ABSPEBKOD AS p8 ON TG.PEBKOD = p8.PEBKOD AND TG.YIL = p8.YIL LEFT OUTER JOIN ";
            sql2 += " ButceHazirlik.dbo.ABSPEBKOD AS p2 ON TG.YIL = p2.YIL AND LEFT(TG.PEBKOD,2) = p2.PEBKOD LEFT OUTER JOIN ";
            sql2 += " ButceHazirlik.dbo.ABSEKO AS e2 ON TG.YIL = e2.YIL AND LEFT(TG.EKOKOD,2) = e2.EKOKOD ";
            sql2 += " WHERE (TG.YIL = {0}) AND (TG.TEKLIFYIL = 0) AND (TG.SENARYOKOD = '{1}') AND (TG.EKOKOD LIKE '06%' OR TG.EKOKOD LIKE '07%' OR TG.EKOKOD LIKE '08%') ";

            if (peb != "" && ekBilgi == "1")
                sql2 += " AND (LEFT(TG.PEBKOD,2) = '" + peb.Replace(".", "") + "') ";
            sql2 += " group by LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD, e2.EKOAD, p8.PEBAD, p2.PEBAD, P.AD, S.SEKTORAD, P.BITISYILI, P.BASLANGICYILI ";
            sql2 += " order by LEFT(TG.PEBKOD,2), LEFT(TG.EKOKOD,2), P.SEKTORNO, P.DPTPROJENO, TG.PEBKOD";

            string[] sorgular = new string[3];
            string[] baglantilar = new string[3];

            sorgular[0] = String.Format(sql, yil, senaryo.Replace(".", ""));
            baglantilar[0] = "BaglantiSatiriBHMv2";

            sorgular[1] = String.Format(sql, yil, "0000");
            baglantilar[1] = "BaglantiSatiriBHMv2";

            sorgular[2] = String.Format(sql2, yil - 1, "0600");
            baglantilar[2] = "BaglantiSatiriBHM";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
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

            ToplamAl toplamAl = new ToplamAl(6, 13);
            int satir = baslaSat;

            SortedList sl = uzyServis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4 });

            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string birimKod = BossaDoldur(alAnahtar.StrAl(0));
                string ekoKod = BossaDoldur(alAnahtar.StrAl(1));
                string sektorKod = BossaDoldur(alAnahtar.StrAl(2));
                string projeNo = BossaDoldur(alAnahtar.StrAl(3));
                string faalKod = BossaDoldur(alAnahtar.StrAl(4));

                toplamAl.DegerAta(5, 0, alVeri.DblAl(0));
                toplamAl.DegerAta(5, 1, alVeri.DblAl(1));
                toplamAl.DegerAta(5, 2, alVeri.DblAl(2));
                toplamAl.DegerAta(5, 3, alVeri.DblAl(3));
                toplamAl.DegerAta(5, 4, alVeri.DblAl(4));
                toplamAl.DegerAta(5, 5, alVeri.DblAl(5));

                string ekoAd = alVeri.StrAl(6);
                string faalAd = alVeri.StrAl(7);
                string birimAd = alVeri.StrAl(8);
                string projeAd = alVeri.StrAl(9);
                string sektorAd = alVeri.StrAl(10);
                int bitYil = alVeri.IntAl(11);
                int baslaYil = alVeri.IntAl(12);

                toplamAl.DegerAta(5, 6, alVeri.DblAl(13));
                toplamAl.DegerAta(5, 7, alVeri.DblAl(14));
                toplamAl.DegerAta(5, 8, alVeri.DblAl(15));
                toplamAl.DegerAta(5, 9, alVeri.DblAl(16));
                toplamAl.DegerAta(5, 10, alVeri.DblAl(17));
                toplamAl.DegerAta(5, 11, alVeri.DblAl(18));

                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(19);
                if (string.IsNullOrWhiteSpace(faalAd))
                    faalAd = alVeri.StrAl(20);
                if (string.IsNullOrWhiteSpace(birimAd))
                    birimAd = alVeri.StrAl(21);
                if (string.IsNullOrWhiteSpace(projeAd))
                    projeAd = alVeri.StrAl(22);
                if (string.IsNullOrWhiteSpace(sektorAd))
                    sektorAd = alVeri.StrAl(23);
                if (bitYil == 0)
                    bitYil = alVeri.IntAl(24);
                if (baslaYil == 0)
                    baslaYil = alVeri.IntAl(25);

                toplamAl.DegerAta(5, 12, alVeri.DblAl(26));

                //e2.EKOAD, p8.PEBAD, p2.PEBAD, P.AD, S.SEKTORAD, P.BITISYILI, P.BASLANGICYILI
                if (string.IsNullOrWhiteSpace(ekoAd))
                    ekoAd = alVeri.StrAl(27);
                if (string.IsNullOrWhiteSpace(faalAd))
                    faalAd = alVeri.StrAl(28);
                if (string.IsNullOrWhiteSpace(birimAd))
                    birimAd = alVeri.StrAl(29);
                if (string.IsNullOrWhiteSpace(projeAd))
                    projeAd = alVeri.StrAl(30);
                if (string.IsNullOrWhiteSpace(sektorAd))
                    sektorAd = alVeri.StrAl(31);
                if (bitYil == 0)
                    bitYil = alVeri.IntAl(32);
                if (baslaYil == 0)
                    baslaYil = alVeri.IntAl(33);

                if (eskiBirim != birimKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        RakamYaz(XLS, eskiBirimSat, baslaSut + 8, toplamAl, 1);
                        toplamAl.SatirSifirla(1);
                    }

                    XLS.HucreKopyala(fSat1, fSut1, fSat1, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, birimKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 5, birimAd);
                    eskiBirim = birimKod;
                    eskiBirimSat = satir;
                    satir++;
                    eskiEko = "";
                }
                if (eskiEko != ekoKod)
                {
                    if (eskiEkoSat > 0)
                    {
                        RakamYaz(XLS, eskiEkoSat, baslaSut + 8, toplamAl, 2);
                        toplamAl.SatirSifirla(2);
                    }

                    XLS.HucreKopyala(fSat1 + 1, fSut1, fSat1 + 1, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 1, ekoKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 5, ekoAd);
                    eskiEko = ekoKod;
                    eskiEkoSat = satir;
                    satir++;
                    eskiSektor = "";
                }

                if (eskiSektor != sektorKod)
                {
                    if (eskiSektorSat > 0)
                    {
                        RakamYaz(XLS, eskiSektorSat, baslaSut + 8, toplamAl, 3);
                        toplamAl.SatirSifirla(3);
                    }

                    XLS.HucreKopyala(fSat1 + 2, fSut1, fSat1 + 2, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 2, sektorKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 5, sektorAd);
                    eskiSektor = sektorKod;
                    eskiSektorSat = satir;
                    satir++;
                    eskiProje = "";
                }

                if (eskiProje != projeNo)
                {
                    if (eskiProjeSat > 0)
                    {
                        RakamYaz(XLS, eskiProjeSat, baslaSut + 8, toplamAl, 4);
                        toplamAl.SatirSifirla(4);
                    }

                    XLS.HucreKopyala(fSat1 + 3, fSut1, fSat1 + 3, fSut1 + 25, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 3, projeNo);
                    XLS.HucreDegerYaz(satir, baslaSut + 5, projeAd);
                    XLS.HucreDegerYaz(satir, baslaSut + 6, baslaYil == yil ? "Yeni" : "Devam E.");
                    XLS.HucreDegerYaz(satir, baslaSut + 7, bitYil);
                    eskiProje = projeNo;
                    eskiProjeSat = satir;
                    satir++;
                }

                XLS.HucreKopyala(fSat1 + 4, fSut1, fSat1 + 4, fSut1 + 25, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut + 4, faalKod);
                XLS.HucreDegerYaz(satir, baslaSut + 5, faalAd);
                RakamYaz(XLS, satir, baslaSut + 8, toplamAl, 5);
                toplamAl.ToplamEkle(4, 5);
                toplamAl.ToplamEkle(3, 5);
                toplamAl.ToplamEkle(2, 5);
                toplamAl.ToplamEkle(1, 5);
                toplamAl.ToplamEkle(0, 5);
                satir++;
            }

            if (eskiBirimSat > 0)
            {
                RakamYaz(XLS, eskiBirimSat, baslaSut + 8, toplamAl, 1);
                toplamAl.SatirSifirla(1);
            }
            if (eskiEkoSat > 0)
            {
                RakamYaz(XLS, eskiEkoSat, baslaSut + 8, toplamAl, 2);
                toplamAl.SatirSifirla(2);
            }
            if (eskiSektorSat > 0)
            {
                RakamYaz(XLS, eskiSektorSat, baslaSut + 8, toplamAl, 3);
                toplamAl.SatirSifirla(3);
            }
            if (eskiProjeSat > 0)
            {
                RakamYaz(XLS, eskiProjeSat, baslaSut + 8, toplamAl, 4);
                toplamAl.SatirSifirla(4);
            }

            if (peb == "" || ekBilgi != "1")
            {
                XLS.HucreKopyala(fSat1, fSut1, fSat1, fSut1 + 25, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut, "");
                XLS.HucreDegerYaz(satir, baslaSut + 5, "T O P L A M");
                RakamYaz(XLS, satir, baslaSut + 8, toplamAl, 0);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string BossaDoldur(string p)
        {
            if (string.IsNullOrWhiteSpace(p)) return "-";
            return p;
        }

        private void RakamYaz(Tablo XLS, int sat, int sut, ToplamAl toplamAl, int tSat)
        {
            XLS.HucreDegerYaz(sat, sut + 0, toplamAl.toplam[tSat, 0]);
            XLS.HucreDegerYaz(sat, sut + 2, toplamAl.toplam[tSat, 1]);
            XLS.HucreDegerYaz(sat, sut + 4, toplamAl.toplam[tSat, 2]);
            XLS.HucreDegerYaz(sat, sut + 1, toplamAl.toplam[tSat, 3]);
            XLS.HucreDegerYaz(sat, sut + 3, toplamAl.toplam[tSat, 4]);
            XLS.HucreDegerYaz(sat, sut + 5, toplamAl.toplam[tSat, 5]);

            XLS.HucreDegerYaz(sat, sut + 6, toplamAl.toplam[tSat, 6]);
            XLS.HucreDegerYaz(sat, sut + 8, toplamAl.toplam[tSat, 7]);
            XLS.HucreDegerYaz(sat, sut + 10, toplamAl.toplam[tSat, 8]);
            XLS.HucreDegerYaz(sat, sut + 7, toplamAl.toplam[tSat, 9]);
            XLS.HucreDegerYaz(sat, sut + 9, toplamAl.toplam[tSat, 10]);
            XLS.HucreDegerYaz(sat, sut + 11, toplamAl.toplam[tSat, 11]);

            XLS.HucreDegerYaz(sat, sut + 12, toplamAl.toplam[tSat, 12]);
        }
    }
}

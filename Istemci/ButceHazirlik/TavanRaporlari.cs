using System.Collections.Generic;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Tavan sayfalarının Excel raporları; eski kodun aynısıdır, yalnızca dosyayı göndermek yerine üretilen dosyanın yolunu döndürür:
    ///   TavanEkoSektorBirimYazdir: eski TavanEkoSektorBirim.aspx btnYazdir_Click (şablon BHMv2\TavanEkoSektorBirim.xltx)
    ///   YatirimVizeYazdirOzet:     eski TavanYatirimVize.aspx YazdirOzet (şablon BHMv2\YatirimVizeTavan.xltx)
    ///   YatirimVizeYazdirDetay:    eski TavanYatirimVize.aspx YazdirDetay (şablon BHMv2\YatirimVizeTavanDetay.xltx)
    ///   TavanEkoFaaliyetDetayYazdir: eski TavanEkoFaaliyet.aspx YazdirDetay (şablon BHMv2\TavanEkoFaaliyetDetay.xltx)
    /// </summary>
    public static class TavanRaporlari
    {
        class SatirSutunBilgi
        {
            public string kod;
            public int tur;
        }

        class SutunBilgi : SatirSutunBilgi
        {
            public string ekoKod;
            public string ekoAd;
            public string sektorKod;
            public string sektorAd;
        }

        class SatirBilgi : SatirSutunBilgi
        {
            public string birimKod;
            public string birimAd;
        }

        /// <summary>Eski GenelIslemlerIstemci.VarsayilanKurumBulAd.</summary>
        private static string KurumAd()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }

        // ------------------------------------------------------------------ Ekonomik kod / sektör / birim tavanları

        public static string TavanEkoSektorBirimYazdir(IBHMv2Servis servis, Kullanici kullanan, int yil, string oncekiYilSenaryo, string raporSablonYol)
        {
            ObjectArray liste = servis.ListeleEkoSektorTavan(kullanan, yil, "0000", null, null, null, oncekiYilSenaryo);
            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaYol = DosyaIslem.DosyaAdUret("OdenekTavanEkoSektorBirim_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(raporSablonYol + "/BHMv2/TavanEkoSektorBirim.xltx", sonucDosyaYol);
            XLS.HucreAdBulYaz("Yil", yil);
            int fsheetNo = 0;
            int fsatir1 = 0;
            int fsutun1 = 0;
            int fsatir2 = 0;
            int fsutun2 = 0;

            XLS.HucreAdAdresCoz("FormatSatir", ref fsheetNo, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);
            int satir = 5;
            int sutun = 2;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            TNSCollection sutunlar = new TNSCollection();
            TNSCollection satirlar = new TNSCollection();
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            for (int i = 0; i < liste.ObjeSayisi; i++)
            {
                TavanGenel tg = (TavanGenel)liste[i];
                SutunYap(sutunlar, tg, uzyServis, kullanan, yil);
                SatirYap(satirlar, tg);
            }

            EkonomikToplamEkle(sutunlar);
            SutunToplamEkle(sutunlar);
            SatirToplamEkle(satirlar);

            int ek;

            XLS.HucreKopyala(fsatir1, fsutun1, fsatir2, fsutun1 + 1, satir, sutun);

            string oncEko = "";
            string oncEkoAd = "";
            int ekoSay = 0;
            int oncEkoSutun = 0;
            List<int> ekoFormulSutunlar = new List<int>();
            List<int> sutunToplamSutunlar = new List<int>();
            List<int> sektorToplamSutunlar = new List<int>();
            int simdSutun = 0;
            for (int i = 0; i < sutunlar.Count; i++)
            {
                SutunBilgi sb = (SutunBilgi)sutunlar[i];
                simdSutun = sutun + i + 2;

                string simdEko = sb.ekoKod;
                if (simdEko != oncEko)
                {
                    if (oncEko != "")
                    {
                        XLS.HucreDegerYaz(satir, oncEkoSutun, oncEkoAd);
                        XLS.HucreBirlestir(satir, oncEkoSutun, satir, simdSutun - 1);
                    }
                    oncEkoSutun = simdSutun;
                    oncEko = simdEko;
                    oncEkoAd = sb.ekoAd;
                    ekoSay++;
                }
                if (ekoSay % 2 == 1) ek = 2;
                else ek = 4;
                if (sb.tur != 1) ek++;
                XLS.HucreKopyala(fsatir1, fsutun1 + ek, fsatir2, fsutun1 + ek, satir, simdSutun);
                string yaz = "";
                if (sb.tur == 1 || sb.tur == 2)
                {
                    yaz = sb.sektorKod + "-" + sb.sektorAd;
                }
                else if (sb.tur == 3)
                {
                    yaz = "Eko:" + sb.ekoKod + " Toplam";
                }
                else
                    yaz = "Toplam";

                XLS.HucreDegerYaz(satir + 1, simdSutun, yaz);

                if (sb.tur <= 2 && sb.sektorKod.Length == 2)
                {
                    ekoFormulSutunlar.Add(simdSutun);
                }

                if (sb.tur == 2)
                {
                    sektorToplamSutunlar.Clear();
                    for (int j = i + 1; j < sutunlar.Count; j++)
                    {
                        int simdSutunIc = sutun + j + 2;
                        SutunBilgi sbIc = (SutunBilgi)sutunlar[j];
                        if (sbIc.sektorKod.StartsWith(sb.sektorKod))
                        {
                            sektorToplamSutunlar.Add(simdSutunIc);
                        }
                        else
                            break;
                        ToplamYap(XLS, satir + 2, simdSutun, sektorToplamSutunlar);
                    }
                }
                else if (sb.tur == 3) //Ekonomik toplam sutunu
                {
                    sutunToplamSutunlar.Add(simdSutun);
                    ToplamYap(XLS, satir + 2, simdSutun, ekoFormulSutunlar);
                    ekoFormulSutunlar.Clear();
                }
                else if (sb.tur == 4)
                {
                    ToplamYap(XLS, satir + 2, simdSutun, sutunToplamSutunlar);
                }
            }

            XLS.HucreBirlestir(satir - 4, sutun, satir - 4, simdSutun);
            XLS.HucreBirlestir(satir - 3, sutun, satir - 3, simdSutun);
            XLS.HucreDegerYaz(satir - 4, sutun, KurumAd());
            XLS.HucreDegerYaz(satir - 1, simdSutun, "(Bin TL)");

            List<int> satirToplamSatirlar = new List<int>();

            int simdSatir = satir + 2;
            for (int i = 0; i < satirlar.Count; i++)
            {
                SatirBilgi sb = (SatirBilgi)satirlar[i];
                if (i > 0)
                {
                    XLS.HucreKopyala(satir + 2, sutun, satir + 7, simdSutun, simdSatir, sutun);
                }

                if (sb.tur == 4)
                {
                    XLS.HucreDegerYaz(simdSatir, sutun + 0, KurumAd());
                    for (int sut = 0; sut < sutunlar.Count; sut++)
                    {
                        SutunBilgi sbIc = (SutunBilgi)sutunlar[sut];
                        if (sbIc.tur == 1)
                        {
                            simdSutun = sutun + sut + 2;
                            string sutunHarf = HarfYap(simdSutun);
                            for (int j = 0; j < 6; j++)
                            {
                                string toplamFormul = "";
                                foreach (int icSat in satirToplamSatirlar)
                                {
                                    if (toplamFormul != "") toplamFormul += "+";
                                    toplamFormul += sutunHarf + (icSat + j + 1);
                                }
                                XLS.HucreFormulYaz(simdSatir + j, simdSutun, "=" + toplamFormul);
                            }
                        }
                    }
                }
                else
                {
                    XLS.HucreDegerYaz(simdSatir, sutun + 0, sb.birimAd);
                    satirToplamSatirlar.Add(simdSatir);
                }
                simdSatir += 6;
            }

            for (int i = 0; i < liste.ObjeSayisi; i++)
            {
                int yer1, yer2;
                TavanGenel tg = (TavanGenel)liste[i];
                SatirBilgi satBil = (SatirBilgi)SatirSutunBul(satirlar, tg.kod3, out yer1);
                SutunBilgi sutBil = (SutunBilgi)SatirSutunBul(sutunlar, tg.kod1 + "-" + tg.kod2, out yer2);
                if (satBil != null && sutBil != null)
                {
                    simdSutun = sutun + yer2 + 2;
                    simdSatir = satir + 2 + yer1 * 6;
                    XLS.HucreDegerYaz(simdSatir + 0, simdSutun, tg.odenek_2 / 1000);
                    XLS.HucreDegerYaz(simdSatir + 1, simdSutun, tg.harcama_2 / 1000);
                    XLS.HucreDegerYaz(simdSatir + 2, simdSutun, tg.odenek_1 / 1000);
                    XLS.HucreDegerYaz(simdSatir + 3, simdSutun, tg.yil0 / 1000);
                    XLS.HucreDegerYaz(simdSatir + 4, simdSutun, tg.yil1 / 1000);
                    XLS.HucreDegerYaz(simdSatir + 5, simdSutun, tg.yil2 / 1000);
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaYol;
        }

        private static void ToplamYap(Tablo XLS, int satir, int sutun, List<int> formulSutunlar)
        {
            for (int i = 0; i < 6; i++)
            {
                XLS.HucreFormulYaz(satir + i, sutun, FormulOl(satir + i, formulSutunlar));
            }
        }

        private static string FormulOl(int satir, List<int> formulSutunlar)
        {
            string formul = "";
            for (int i = 0; i < formulSutunlar.Count; i++)
            {
                if (formul != "") formul += "+";
                formul += HarfYap(formulSutunlar[i]) + (satir + 1).ToString();
            }
            return "=" + formul;
        }

        private static string HarfYap(int sutun)
        {
            string retVal = "";
            int harfGrup = sutun / 26;
            int harfIc = sutun % 26;
            if (harfGrup > 0)
            {
                retVal += TekHarf(harfGrup - 1);
            }
            retVal += TekHarf(harfIc);
            return retVal;
        }

        private static char TekHarf(int sutun)
        {
            int tut = 65 + sutun;
            return (char)tut;
        }

        private static void SatirToplamEkle(TNSCollection satirlar)
        {
            SatirBilgi sb = new SatirBilgi();
            sb.kod = "";
            sb.tur = 4;
            satirlar.Add(sb);
        }

        private static void SutunToplamEkle(TNSCollection sutunlar)
        {
            SutunBilgi sb = new SutunBilgi();
            sb.kod = "";
            sb.ekoKod = "";
            sb.ekoAd = "";
            sb.sektorKod = "";
            sb.sektorAd = "";
            sb.tur = 4;
            sutunlar.Add(sb);
        }

        private static void SatirYap(TNSCollection satirlar, TavanGenel tg)
        {
            int yer;
            string kod = tg.kod3;
            SatirBilgi sb = (SatirBilgi)SatirSutunBul(satirlar, kod, out yer);
            if (sb == null)
            {
                sb = new SatirBilgi();
                sb.kod = kod;
                sb.birimKod = kod;
                sb.birimAd = tg.ad3;
                sb.tur = 1;
                if (yer == -1)
                    satirlar.Add(sb);
                else
                    satirlar.Insert(yer, sb);
            }
        }

        private static void EkonomikToplamEkle(TNSCollection sutunlar)
        {
            string oncEko = "";
            for (int i = 0; i < sutunlar.Count; i++)
            {
                SutunBilgi ssb = (SutunBilgi)sutunlar[i];
                if (oncEko != ssb.ekoKod)
                {
                    if (oncEko != "")
                    {
                        YeniEkoToplamEkle(sutunlar, i, oncEko);
                        i++;
                    }
                    oncEko = ssb.ekoKod;
                }
            }
            if (oncEko != "")
            {
                YeniEkoToplamEkle(sutunlar, -1, oncEko);
            }
        }

        private static void YeniEkoToplamEkle(TNSCollection sutunlar, int yer, string eko)
        {
            SutunBilgi sb = new SutunBilgi();
            sb.kod = eko;
            sb.ekoKod = eko;
            sb.ekoAd = "";
            sb.sektorKod = "";
            sb.sektorAd = "";
            sb.tur = 3;
            if (yer == -1)
                sutunlar.Add(sb);
            else
                sutunlar.Insert(yer, sb);
        }

        private static void SutunYap(TNSCollection sutunlar, TavanGenel tg, TNS.UZY.IUZYServis uzyServis, Kullanici kullanan, int yil)
        {
            int yer;
            string kod = "";
            SutunBilgi sb = null;
            if (tg.kod2.Length > 2)
            {
                kod = tg.kod1 + "-" + tg.kod2.Substring(0, 2);
                sb = (SutunBilgi)SatirSutunBul(sutunlar, kod, out yer);
                if (sb == null)
                {
                    sb = new SutunBilgi();
                    sb.kod = kod;
                    sb.ekoKod = tg.kod1;
                    sb.ekoAd = tg.ad1;
                    sb.sektorKod = tg.kod2.Substring(0, 2);
                    sb.sektorAd = uzyServis.UzayDegeriStr(kullanan, "ABSSEKTOR", yil.ToString() + "-" + sb.sektorKod, true);
                    sb.tur = 2;
                    if (yer == -1)
                        sutunlar.Add(sb);
                    else
                        sutunlar.Insert(yer, sb);
                }
            }

            kod = tg.kod1 + "-" + tg.kod2;
            sb = (SutunBilgi)SatirSutunBul(sutunlar, kod, out yer);
            if (sb == null)
            {
                sb = new SutunBilgi();
                sb.kod = kod;
                sb.ekoKod = tg.kod1;
                sb.ekoAd = tg.ad1;
                sb.sektorKod = tg.kod2;
                sb.sektorAd = tg.ad2;
                sb.tur = 1;
                if (yer == -1)
                    sutunlar.Add(sb);
                else
                    sutunlar.Insert(yer, sb);
            }
        }

        private static SatirSutunBilgi SatirSutunBul(TNSCollection satirlar, string kod, out int yer)
        {
            yer = -1;
            for (int i = 0; i < satirlar.Count; i++)
            {
                SatirSutunBilgi ssb = (SatirSutunBilgi)satirlar[i];
                int comp = ssb.kod.CompareTo(kod);
                if (comp == 0)
                {
                    yer = i;
                    return ssb;
                }
                else if (comp > 0)
                {
                    yer = i;
                    return null;
                }
            }
            return null;
        }

        // ------------------------------------------------------------------ Yatırım vize tavanları (özet)

        public static string YatirimVizeYazdirOzet(IBHMv2Servis servis, Kullanici kullanan, int yil, string pebKod, string ekoKod, string finKod, string raporSablonYol)
        {
            string kriter = "";
            if (!string.IsNullOrWhiteSpace(pebKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Faal: " + pebKod;
            }
            if (!string.IsNullOrWhiteSpace(ekoKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Eko: " + ekoKod;
            }
            if (!string.IsNullOrWhiteSpace(finKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Fin: " + finKod;
            }
            if (string.IsNullOrWhiteSpace(kriter)) kriter = "Yok";
            kriter = "(Kriter: " + kriter + ")";
            ObjectArray liste = servis.ListeleYatirimVizeTavanRapor(kullanan, yil, "10.00", false, pebKod, ekoKod, finKod);
            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaYol = DosyaIslem.DosyaAdUret() + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(raporSablonYol + "/BHMv2/" + "YatirimVizeTavan.xltx", sonucDosyaYol);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreDegerYaz(7, 5, "Açıklama\r\n" + kriter);
            string simdSek = "";
            string oncSek = null;
            int oncSekSat = -1;

            string simdProj = "";
            string oncProj = null;
            int oncProjSat = -1;

            int formatSatir = 1;
            int satYuk = XLS.SatirYukseklikAl(formatSatir + 4);
            int baslaSatir = 9;
            int satir = baslaSatir;
            ToplamAl toplamAl = new ToplamAl(4, 6);

            for (int i = 0; i < liste.ObjeSayisi; i++)
            {
                TavanGenel hk = (TavanGenel)liste[i];
                simdSek = hk.kod1;
                simdProj = hk.kod4;

                if (oncSek != simdSek)
                {
                    if (oncSekSat >= 0)
                    {
                        ToplamAlYaz(XLS, oncSekSat, toplamAl, 1, 6);
                    }
                    toplamAl.SatirSifirla(1);
                    oncSekSat = satir;
                    XLS.HucreKopyala(formatSatir, 0, formatSatir, 14, satir, 0);
                    XLS.HucreDegerYaz(satir, 2, hk.kod1);
                    XLS.HucreDegerYaz(satir, 5, hk.ad1);
                    satir++;
                    oncSek = simdSek;
                    oncProj = null;
                }

                if (oncProj != simdProj)
                {
                    if (oncProjSat >= 0)
                    {
                        ToplamAlYaz(XLS, oncProjSat, toplamAl, 2, 6);
                    }
                    toplamAl.SatirSifirla(2);
                    oncProjSat = satir;
                    XLS.HucreKopyala(formatSatir + 1, 0, formatSatir + 1, 14, satir, 0);
                    XLS.HucreDegerYaz(satir, 3, hk.kod4);
                    XLS.HucreDegerYaz(satir, 5, hk.ad4);
                    satir++;
                    oncProj = simdProj;
                }

                XLS.HucreKopyala(formatSatir + 2, 0, formatSatir + 2, 14, satir, 0);
                XLS.HucreDegerYaz(satir, 0, hk.kod1);
                XLS.HucreDegerYaz(satir, 1, hk.kod4);

                XLS.HucreDegerYaz(satir, 4, hk.kod5);
                XLS.HucreDegerYaz(satir, 5, hk.ad5);

                toplamAl.toplam[3, 0] = hk.yil0;
                toplamAl.toplam[3, 1] = hk.yil1;
                toplamAl.toplam[3, 2] = hk.yil2;
                toplamAl.toplam[3, 3] = hk.teklif0;
                toplamAl.toplam[3, 4] = hk.teklif1;
                toplamAl.toplam[3, 5] = hk.teklif2;

                ToplamAlYaz(XLS, satir, toplamAl, 3, 6);
                toplamAl.ToplamEkle(2, 3);
                toplamAl.ToplamEkle(1, 3);
                toplamAl.ToplamEkle(0, 3);
                satir++;
            }

            if (oncSekSat >= 0)
                ToplamAlYaz(XLS, oncSekSat, toplamAl, 1, 6);
            if (oncProjSat >= 0)
            {
                ToplamAlYaz(XLS, oncProjSat, toplamAl, 2, 6);
                XLS.HucreKopyala(formatSatir, 0, formatSatir, 14, satir, 0);
                XLS.HucreDegerYaz(satir, 5, "T O P L A M " + kriter);
                ToplamAlYaz(XLS, satir, toplamAl, 0, 6);
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir, satYuk);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaYol;
        }

        // ------------------------------------------------------------------ Yatırım vize tavanları (detay)

        /// <summary>Eski TavanYatirimVize.aspx YazdirDetay (şablon BHMv2\YatirimVizeTavanDetay.xltx).</summary>
        public static string YatirimVizeYazdirDetay(IBHMv2Servis servis, Kullanici kullanan, int yil, string pebKod, string ekoKod, string finKod, string raporSablonYol)
        {
            string kriter = "";
            if (!string.IsNullOrWhiteSpace(pebKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Faal: " + pebKod;
            }
            if (!string.IsNullOrWhiteSpace(ekoKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Eko: " + ekoKod;
            }
            if (!string.IsNullOrWhiteSpace(finKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Fin: " + finKod;
            }
            if (string.IsNullOrWhiteSpace(kriter)) kriter = "Yok";
            kriter = "(Kriter: " + kriter + ")";
            ObjectArray liste = servis.ListeleYatirimVizeTavanRapor(kullanan, yil, "10.00", true, pebKod, ekoKod, finKod);
            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaYol = DosyaIslem.DosyaAdUret() + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(raporSablonYol + "/BHMv2/" + "YatirimVizeTavanDetay.xltx", sonucDosyaYol);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreDegerYaz(9, 9, "Açıklama\r\n" + kriter);
            string simdSek = "";
            string oncSek = null;
            int oncSekSat = -1;

            string simdEko = "";
            string oncEko = null;
            int oncEkoSat = -1;

            string simdFin = "";
            string oncFin = null;
            int oncFinSat = -1;

            string simdProj = "";
            string oncProj = null;
            int oncProjSat = -1;

            int formatSatir = 1;
            int satYuk = XLS.SatirYukseklikAl(formatSatir + 6);
            int baslaSatir = 11;
            int satir = baslaSatir;
            ToplamAl toplamAl = new ToplamAl(6, 6);

            for (int i = 0; i < liste.ObjeSayisi; i++)
            {
                TavanGenel hk = (TavanGenel)liste[i];
                simdSek = hk.kod1;
                simdEko = hk.kod2;
                simdFin = hk.kod3;
                simdProj = hk.kod4;

                if (oncSek != simdSek)
                {
                    if (oncSekSat >= 0)
                    {
                        ToplamAlYaz(XLS, oncSekSat, toplamAl, 1, 10);
                    }
                    toplamAl.SatirSifirla(1);
                    oncSekSat = satir;
                    XLS.HucreKopyala(formatSatir, 0, formatSatir, 18, satir, 0);
                    XLS.HucreDegerYaz(satir, 4, hk.kod1);
                    XLS.HucreDegerYaz(satir, 9, hk.ad1);
                    satir++;
                    oncSek = simdSek;
                    oncEko = null;
                }

                if (oncEko != simdEko)
                {
                    if (oncEkoSat >= 0)
                    {
                        ToplamAlYaz(XLS, oncEkoSat, toplamAl, 2, 10);
                    }
                    toplamAl.SatirSifirla(2);
                    oncEkoSat = satir;
                    XLS.HucreKopyala(formatSatir + 1, 0, formatSatir + 1, 18, satir, 0);
                    XLS.HucreDegerYaz(satir, 5, hk.kod2);
                    XLS.HucreDegerYaz(satir, 9, hk.ad2);
                    satir++;
                    oncEko = simdEko;
                    oncFin = "";
                }

                if (oncFin != simdFin)
                {
                    if (oncFinSat >= 0)
                    {
                        ToplamAlYaz(XLS, oncFinSat, toplamAl, 3, 10);
                    }
                    toplamAl.SatirSifirla(3);
                    oncFinSat = satir;
                    XLS.HucreKopyala(formatSatir + 2, 0, formatSatir + 2, 18, satir, 0);
                    XLS.HucreDegerYaz(satir, 6, hk.kod3);
                    XLS.HucreDegerYaz(satir, 9, hk.ad3);
                    satir++;
                    oncFin = simdFin;
                    oncProj = "";
                }

                if (oncProj != simdProj)
                {
                    if (oncProjSat >= 0)
                    {
                        ToplamAlYaz(XLS, oncProjSat, toplamAl, 4, 10);
                    }
                    toplamAl.SatirSifirla(4);
                    oncProjSat = satir;
                    XLS.HucreKopyala(formatSatir + 3, 0, formatSatir + 3, 18, satir, 0);
                    XLS.HucreDegerYaz(satir, 7, hk.kod4);
                    XLS.HucreDegerYaz(satir, 9, hk.ad4);
                    satir++;
                    oncProj = simdProj;
                }

                XLS.HucreKopyala(formatSatir + 4, 0, formatSatir + 4, 18, satir, 0);
                XLS.HucreDegerYaz(satir, 0, hk.kod1);
                XLS.HucreDegerYaz(satir, 1, hk.kod2);
                XLS.HucreDegerYaz(satir, 2, hk.kod3);
                XLS.HucreDegerYaz(satir, 3, hk.kod4);

                XLS.HucreDegerYaz(satir, 8, hk.kod5);
                XLS.HucreDegerYaz(satir, 9, hk.ad5);

                toplamAl.toplam[5, 0] = hk.yil0;
                toplamAl.toplam[5, 1] = hk.yil1;
                toplamAl.toplam[5, 2] = hk.yil2;
                toplamAl.toplam[5, 3] = hk.teklif0;
                toplamAl.toplam[5, 4] = hk.teklif1;
                toplamAl.toplam[5, 5] = hk.teklif2;

                ToplamAlYaz(XLS, satir, toplamAl, 5, 10);
                toplamAl.ToplamEkle(4, 5);
                toplamAl.ToplamEkle(3, 5);
                toplamAl.ToplamEkle(2, 5);
                toplamAl.ToplamEkle(1, 5);
                toplamAl.ToplamEkle(0, 5);
                satir++;
            }

            if (oncSekSat >= 0)
                ToplamAlYaz(XLS, oncSekSat, toplamAl, 1, 10);
            if (oncEkoSat >= 0)
                ToplamAlYaz(XLS, oncEkoSat, toplamAl, 2, 10);
            if (oncFinSat >= 0)
                ToplamAlYaz(XLS, oncFinSat, toplamAl, 3, 10);
            if (oncProjSat >= 0)
            {
                ToplamAlYaz(XLS, oncProjSat, toplamAl, 4, 10);
                XLS.HucreKopyala(formatSatir, 0, formatSatir, 18, satir, 0);
                XLS.HucreDegerYaz(satir, 9, "T O P L A M " + kriter);
                ToplamAlYaz(XLS, satir, toplamAl, 0, 10);
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir, satYuk);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaYol;
        }

        // ------------------------------------------------------------------ Faaliyet tavanları (TavanEkoFaaliyet)

        /// <summary>Eski TavanEkoFaaliyet.aspx YazdirDetay (şablon BHMv2\TavanEkoFaaliyetDetay.xltx).</summary>
        public static string TavanEkoFaaliyetDetayYazdir(IBHMv2Servis servis, Kullanici kullanan, int yil, string ekoKod, string sektorKod, string pebBirimKod, string raporSablonYol)
        {
            string kriter = "";
            if (!string.IsNullOrWhiteSpace(ekoKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Eko: " + ekoKod;
            }
            else ekoKod = null;
            if (!string.IsNullOrWhiteSpace(sektorKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Fin: " + sektorKod;
            }
            else
                sektorKod = null;
            if (!string.IsNullOrWhiteSpace(pebBirimKod))
            {
                if (kriter != "") kriter += ", ";
                kriter += "Faal: " + pebBirimKod;
            }
            else
                pebBirimKod = null;
            if (string.IsNullOrWhiteSpace(kriter)) kriter = "Yok";
            kriter = "(Kriter: " + kriter + ")";

            ObjectArray liste = servis.ListeleEkoSektorFaaliyetTavan(kullanan, yil, "0000", ekoKod, sektorKod, pebBirimKod, null);
            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaYol = DosyaIslem.DosyaAdUret() + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(raporSablonYol + "/BHMv2/" + "TavanEkoFaaliyetDetay.xltx", sonucDosyaYol);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreDegerYaz(9, 8, "Açıklama\r\n" + kriter);
            string simdEko = "";
            string oncEko = null;
            int oncEkoSat = -1;

            string simdSek = "";
            string oncSek = null;
            int oncSekSat = -1;

            string simdPeb2 = "";
            string oncPeb2 = null;
            int oncPeb2Sat = -1;
            int RAKAM_BASLA = 9;

            int formatSatir = 1;
            int satYuk = XLS.SatirYukseklikAl(formatSatir + 6);
            int baslaSatir = 11;
            int satir = baslaSatir;
            ToplamAl toplamAl = new ToplamAl(6, 6);

            for (int i = 0; i < liste.ObjeSayisi; i++)
            {
                TavanGenel hk = (TavanGenel)liste[i];
                simdEko = hk.kod1;
                simdSek = hk.kod2;
                simdPeb2 = hk.kod3;

                if (oncEko != simdEko)
                {
                    if (oncEkoSat >= 0)
                    {
                        ToplamAlYaz(XLS, oncEkoSat, toplamAl, 1, RAKAM_BASLA);
                    }
                    toplamAl.SatirSifirla(1);
                    oncEkoSat = satir;
                    XLS.HucreKopyala(formatSatir, 0, formatSatir, 18, satir, 0);
                    XLS.HucreDegerYaz(satir, 4, hk.kod1);
                    XLS.HucreDegerYaz(satir, 8, hk.ad1);
                    satir++;
                    oncEko = simdEko;
                    oncSek = null;
                }

                if (oncSek != simdSek)
                {
                    if (oncSekSat >= 0)
                    {
                        ToplamAlYaz(XLS, oncSekSat, toplamAl, 2, RAKAM_BASLA);
                    }
                    toplamAl.SatirSifirla(2);
                    oncSekSat = satir;
                    XLS.HucreKopyala(formatSatir + 1, 0, formatSatir + 1, 18, satir, 0);
                    XLS.HucreDegerYaz(satir, 5, hk.kod2);
                    XLS.HucreDegerYaz(satir, 8, hk.ad2);
                    satir++;
                    oncSek = simdSek;
                    oncPeb2 = "";
                }

                if (oncPeb2 != simdPeb2)
                {
                    if (oncPeb2Sat >= 0)
                    {
                        ToplamAlYaz(XLS, oncPeb2Sat, toplamAl, 3, RAKAM_BASLA);
                    }
                    toplamAl.SatirSifirla(3);
                    oncPeb2Sat = satir;
                    XLS.HucreKopyala(formatSatir + 2, 0, formatSatir + 2, 18, satir, 0);
                    XLS.HucreDegerYaz(satir, 6, hk.kod3);
                    XLS.HucreDegerYaz(satir, 8, hk.ad3);
                    satir++;
                    oncPeb2 = simdPeb2;
                }

                XLS.HucreKopyala(formatSatir + 3, 0, formatSatir + 3, 18, satir, 0);
                XLS.HucreDegerYaz(satir, 0, hk.kod1);
                XLS.HucreDegerYaz(satir, 1, hk.kod2);
                XLS.HucreDegerYaz(satir, 2, hk.kod3);
                XLS.HucreDegerYaz(satir, 3, hk.kod4);
                XLS.HucreDegerYaz(satir, 7, hk.kod4);
                XLS.HucreDegerYaz(satir, 8, hk.ad4);

                toplamAl.toplam[4, 0] = hk.yil0;
                toplamAl.toplam[4, 1] = hk.yil1;
                toplamAl.toplam[4, 2] = hk.yil2;
                toplamAl.toplam[4, 3] = hk.teklif0;
                toplamAl.toplam[4, 4] = hk.teklif1;
                toplamAl.toplam[4, 5] = hk.teklif2;

                ToplamAlYaz(XLS, satir, toplamAl, 4, RAKAM_BASLA);
                toplamAl.ToplamEkle(3, 4);
                toplamAl.ToplamEkle(2, 4);
                toplamAl.ToplamEkle(1, 4);
                toplamAl.ToplamEkle(0, 4);
                satir++;
            }

            if (oncEkoSat >= 0)
                ToplamAlYaz(XLS, oncEkoSat, toplamAl, 1, RAKAM_BASLA);
            if (oncSekSat >= 0)
                ToplamAlYaz(XLS, oncSekSat, toplamAl, 2, RAKAM_BASLA);
            if (oncPeb2Sat >= 0)
            {
                ToplamAlYaz(XLS, oncPeb2Sat, toplamAl, 3, RAKAM_BASLA);
                XLS.HucreKopyala(formatSatir, 0, formatSatir, 18, satir, 0);
                XLS.HucreDegerYaz(satir, 9, "T O P L A M " + kriter);
                ToplamAlYaz(XLS, satir, toplamAl, 0, RAKAM_BASLA);
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir, satYuk);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaYol;
        }

        private static void ToplamAlYaz(Tablo XLS, int satir, ToplamAl toplamAl, int toplamSat, int sutun)
        {
            XLS.HucreDegerYaz(satir, sutun++, toplamAl.toplam[toplamSat, 0]);
            XLS.HucreDegerYaz(satir, sutun++, toplamAl.toplam[toplamSat, 1]);
            XLS.HucreDegerYaz(satir, sutun++, toplamAl.toplam[toplamSat, 2]);
            XLS.HucreDegerYaz(satir, sutun++, toplamAl.toplam[toplamSat, 3]);
            XLS.HucreDegerYaz(satir, sutun++, toplamAl.toplam[toplamSat, 4]);
            XLS.HucreDegerYaz(satir, sutun++, toplamAl.toplam[toplamSat, 5]);
        }
    }
}

using System;
using System.Collections;
using TNS;
using TNS.KYM;
using TNS.UZY;

// Eski OrtakClass\ExcelRapor.cs'ten alındı: Rapor.xml'de RAPORTIP="EXCEL" olan raporlar (şablondaki SatirBaslik / SutunBaslik
// bölgelerine göre uzay değerleriyle doldurulan Excel şablonları). Hücre adresi çözümü eski JExcel (jxl.CellReferenceHelper)
// yerine burada yapılır; geri kalanı eski kodun aynısıdır.
namespace OrtakClass
{
    class RaporDegiskenDeger
    {
        public string kolonAd = "";
        public string satirDeger = "";
        public string sutunDeger = "";
        public RaporDegiskenDeger(string p_kolonAd, string p_satirDeger, string p_sutunDeger)
        {
            kolonAd = p_kolonAd;
            satirDeger = p_satirDeger;
            sutunDeger = p_sutunDeger;
        }
    }

    class RaporDegiskenDegerler
    {
        TNSCollection raporDegiskenDeger = new TNSCollection();

        public int YerBul(string kolonAd)
        {
            for (int i = 0; i < raporDegiskenDeger.Count; i++)
            {
                if (((RaporDegiskenDeger)raporDegiskenDeger[i]).kolonAd == kolonAd)
                    return i;
            }
            return -1;
        }

        public RaporDegiskenDeger DegiskenDondur(int yer)
        {
            return (RaporDegiskenDeger)raporDegiskenDeger[yer];
        }

        public void SatirDegerAta(string kolonAd, string deger)
        {
            int yer = YerBul(kolonAd);
            if (yer == -1)
            {
                raporDegiskenDeger.Add(new RaporDegiskenDeger(kolonAd, deger, ""));
            }
            else
                ((RaporDegiskenDeger)raporDegiskenDeger[yer]).satirDeger = deger;
        }

        public void SutunDegerAta(string kolonAd, string deger)
        {
            int yer = YerBul(kolonAd);
            if (yer == -1)
            {
                raporDegiskenDeger.Add(new RaporDegiskenDeger(kolonAd, "", deger));
            }
            else
                ((RaporDegiskenDeger)raporDegiskenDeger[yer]).sutunDeger = deger;
        }
    }

    public static class ExcelRapor
    {
        // Eski VeriAl yalnızca "*KODLİSTELE(...)" komutunu tanıyordu ve gövdesi yorum satırındaydı (hep boş liste döndürüyordu).
        private static TNSCollection VeriAl(Tablo XLS, string komut, Kullanici kullanan)
        {
            return new TNSCollection();
        }

        private static void VeriYineleSatir(Tablo XLS, int satir, int sutun1, int sutun2, ref int bitisSatir, int sonSutun, Kullanici kullanan)
        {
            for (int sutun = sutun1; sutun <= sutun2; sutun++)
            {
                string HucreDeger = XLS.HucreDegerAl(satir, sutun);
                if (HucreDeger != null && HucreDeger != "" && HucreDeger.StartsWith("*"))
                {
                    TNSCollection veriler = VeriAl(XLS, HucreDeger, kullanan);
                    if (veriler.Count > 1)
                        XLS.SatirAc(satir + 1, veriler.Count - 1);
                    bitisSatir += (veriler.Count - 1);
                    for (int i = 0; i < veriler.Count; i++)
                    {
                        if (i != 0)
                        {
                            XLS.HucreKopyala(satir, sutun1, satir, sonSutun, satir + i, sutun1);
                            XLS.SatirYukseklikAyarla(satir + i, satir + i, XLS.SatirYukseklikAl(satir));
                        }
                        XLS.HucreDegerYaz(satir + i, sutun, (string)veriler[i]);
                    }
                }
            }
        }

        private static void IslecAl(ref string formul, out string degiskenStr, out char islecYeni)
        {
            degiskenStr = "";
            islecYeni = ' ';
            while (true)
            {
                if (formul == null || formul == "")
                    return;
                char c = formul[0];
                formul = formul.Substring(1);
                if (c == '+' || c == '-' || c == '&')
                {
                    islecYeni = c;
                    return;
                }
                else
                    degiskenStr += c;
            }
        }

        public static bool RakamMi(char r)
        {
            return r >= '0' && r <= '9';
        }

        /// <summary>"B12" biçimindeki hücre adresini (0'dan başlayan) satır ve sütuna çevirir; adres değilse -1 döner.</summary>
        private static void AdresCoz(string hucreAdres, ref int sat, ref int sut)
        {
            string harfler = "";
            string rakamlar = "";
            int yer = 0;
            sat = -1;
            sut = -1;
            while (yer < hucreAdres.Length)
            {
                if (!RakamMi(hucreAdres[yer]))
                {
                    harfler += hucreAdres[yer];
                    yer++;
                }
                else
                    break;
            }
            while (yer < hucreAdres.Length)
            {
                if (RakamMi(hucreAdres[yer]))
                {
                    rakamlar += hucreAdres[yer];
                    yer++;
                }
                else
                    break;
            }
            if (yer != hucreAdres.Length || harfler == "" || rakamlar == "" || harfler.Length > 2)
                return;
            int sutun = 0;
            foreach (char c in harfler.ToUpperInvariant())
            {
                if (c < 'A' || c > 'Z') return;
                sutun = sutun * 26 + (c - 'A' + 1);
            }
            sut = sutun - 1;
            sat = OrtakFonksiyonlar.ConvertToInt(rakamlar, 0) - 1;
        }

        private static string FormulHesapla(Tablo XLS, string formul)
        {
            double dSonuc = 0;
            string sSonuc = "";
            bool sonucStr = true;
            char islec = ' ';
            while (formul != "")
            {
                string degiskenStr;
                char islecYeni;
                IslecAl(ref formul, out degiskenStr, out islecYeni);
                if (degiskenStr.Length > 0)
                {
                    if (!RakamMi(degiskenStr[0]))
                    {
                        int sat = 0, sut = 0;
                        AdresCoz(degiskenStr, ref sat, ref sut);
                        if (sat >= 0)
                            degiskenStr = XLS.HucreDegerAl(sat, sut);
                        else
                            degiskenStr = XLS.HucreAdDegerAl(degiskenStr);
                    }
                    if (degiskenStr != "")
                    {
                        if (islec == ' ')
                        {
                            sSonuc = degiskenStr;
                        }
                        else if (islec == '+' || islec == '-')
                        {
                            if (sonucStr)
                            {
                                dSonuc = OrtakFonksiyonlar.ConvertToDbl(sSonuc);
                                sonucStr = false;
                            }
                            double degisken = OrtakFonksiyonlar.ConvertToDbl(degiskenStr.Replace(".", ","));
                            if (islec == '+')
                                dSonuc = dSonuc + degisken;
                            else if (islec == '-')
                                dSonuc = dSonuc - degisken;
                            sSonuc = dSonuc.ToString();
                        }
                        else if (islec == '&')
                        {
                            sonucStr = true;
                            sSonuc = sSonuc + degiskenStr;
                        }
                    }
                    islec = islecYeni;
                }
                else
                    formul = "";
            }

            return sSonuc;
        }

        private static string FormulHesapla(Tablo XLS, string degiskenAd, int sat, int sut, Hashtable raporParam)
        {
            string deger = XLS.HucreDegerAl(sat, sut);
            string formul = XLS.HucreFormulAl(sat, sut);
            if (!string.IsNullOrWhiteSpace(formul))
            {
                deger = FormulHesapla(XLS, formul);
            }
            return deger;
        }

        public static string ExcelRaporHesapla(object o_kullanan, IUZYServis servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam)
        {
            int satirBaslaSat = 0, satirBaslaSut = 0, sutunBaslaSat = 0, sutunBaslaSut = 0;
            int satirBitisSat = 0, satirBitisSut = 0, sutunBitisSat = 0, sutunBitisSut = 0;
            int sheetNo = -1;
            Kullanici kullanan = (Kullanici)o_kullanan;
            Tablo XLS = GenelIslemler.NewTablo();
            if (servis == null)
            {
                servis = TNS.UZY.Arac.Tanimla();
            }

            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            foreach (DictionaryEntry de in raporParam)
            {
                XLS.HucreAdBulYaz((string)de.Key, Convert.ToString(de.Value));
            }
            TNSCollection uzlar = servis.UzaylariListele();
            XLS.HucreAdAdresCoz("SifirlarGizli", ref satirBaslaSat, ref satirBaslaSut);
            bool sifirlariGizle = false;
            if (satirBaslaSat >= 0 && satirBaslaSut >= 0)
                sifirlariGizle = true;
            for (int DonguKontrol = 0; DonguKontrol > -1; DonguKontrol++)
            {
                string ek = "";
                if (DonguKontrol != 0)
                    ek = DonguKontrol.ToString();
                XLS.HucreAdAdresCoz("SatirBaslik" + ek, ref sheetNo, ref satirBaslaSat, ref satirBaslaSut, ref satirBitisSat, ref satirBitisSut);
                if (satirBaslaSat < 0 || satirBaslaSut < 0)
                    break;
                XLS.HucreAdAdresCoz("SutunBaslik" + ek, ref sheetNo, ref sutunBaslaSat, ref sutunBaslaSut, ref sutunBitisSat, ref sutunBitisSut);
                if (sutunBaslaSat < 0 || sutunBaslaSut < 0)
                    break;
                XLS.AktifSheetDegistir(sheetNo);

                RaporDegiskenDegerler rd = new RaporDegiskenDegerler();
                VeriYineleSatir(XLS, satirBaslaSat + 1, satirBaslaSut + 1, satirBitisSut, ref satirBitisSat, sutunBitisSut + 10, kullanan);
                for (int sat = satirBaslaSat + 1; sat <= satirBitisSat; sat++)
                {
                    bool veriGeldi = false;
                    string uzayAdSatir = XLS.HucreDegerAl(sat, satirBaslaSut);
                    if (uzayAdSatir.ToUpper() != "X")
                    {
                        for (int sut = satirBaslaSut + 1; sut <= satirBitisSut; sut++)
                        {
                            string degiskenAd = XLS.HucreDegerAl(satirBaslaSat, sut);
                            string deger = FormulHesapla(XLS, degiskenAd, sat, sut, raporParam);
                            rd.SatirDegerAta(degiskenAd, deger);
                        }

                        for (int sut = sutunBaslaSut + 1; sut <= sutunBitisSut; sut++)
                        {
                            string UzayAdSutun = XLS.HucreDegerAl(sutunBaslaSat, sut);
                            if (UzayAdSutun.ToUpper() != "X")
                            {
                                string uzayAd;
                                if (uzayAdSatir == "")
                                    uzayAd = UzayAdSutun;
                                else
                                    uzayAd = uzayAdSatir;
                                string uzAd = "", alanAd = "";
                                OrtakFonksiyonlar.StringiIkiyeBol(uzayAd, ",", ref uzAd, ref alanAd);
                                uzayAd = uzAd;
                                for (int satIc = sutunBaslaSat + 1; satIc <= sutunBitisSat; satIc++)
                                {
                                    string degiskenAd = XLS.HucreDegerAl(satIc, sutunBaslaSut);
                                    string deger = FormulHesapla(XLS, degiskenAd, satIc, sut, raporParam);
                                    rd.SutunDegerAta(degiskenAd, deger);
                                }

                                for (int uzNo = 0; uzNo < uzlar.Count; uzNo++)
                                {
                                    Uzay uz = (Uzay)uzlar[uzNo];
                                    if (uz.ad == uzayAd)
                                    {
                                        string uzSorgu = "";
                                        for (int alanNo = 0; alanNo < uz.AlanSay(); alanNo++)
                                        {
                                            string degisikenDeger;
                                            UzayAlan uzAlan = uz.AlanBul(alanNo);
                                            int yer = rd.YerBul(uzAlan.ad);
                                            if (yer > -1)
                                            {
                                                RaporDegiskenDeger rdd = rd.DegiskenDondur(yer);
                                                if (rdd.satirDeger == "" || rdd.satirDeger == "0")
                                                    degisikenDeger = rdd.sutunDeger;
                                                else
                                                    degisikenDeger = rdd.satirDeger;
                                            }
                                            else
                                                degisikenDeger = "";
                                            uzSorgu += degisikenDeger;
                                            if (alanNo < uz.AlanSay() - 1)
                                                uzSorgu += "-";
                                        }
                                        if (XLS.HucreDegerAl(sat, sut) == "")
                                        {
                                            if (uz.tip == "N")
                                            {
                                                double d = servis.UzayDegeriDbl(kullanan, uzAd, uzSorgu, false, alanAd);
                                                XLS.HucreDegerYaz(sat, sut, d);
                                                if (d != 0) veriGeldi = true;
                                            }
                                            else
                                                XLS.HucreDegerYaz(sat, sut, servis.UzayDegeriStr(kullanan, uzAd, uzSorgu, false, alanAd));
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                        veriGeldi = true;
                    if (!veriGeldi && sifirlariGizle)
                    {
                        XLS.SatirGizle(sat, sat, true);
                    }
                }
                XLS.SelectSheet(sheetNo);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }
}

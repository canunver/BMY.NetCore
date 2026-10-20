using System;
using System.Collections;
using System.IO;
using OrtakClass;
using TNS;
using TNS.HRC;
using TNS.KYM;

namespace HarcamaSurecleri
{
    /// <summary>
    /// Bordro raporları (Rapor.xml NESNE "HarcamaSurecleri.BordroRaporlari"; grup 20000 rapor 1, EKBILGI "Liste",
    /// şablon HRC\BordroKisiListe.xltx; kıstas: tcKimlikNo, isteğe bağlı yil, ay, bTur, bIslem, sablonNo).
    /// Personel İşlemleri > Maaş Parametreleri "Bordro Yazdır" düğmesi bunu RaporlamaServis.TekRaporAl(20000, 1, ...) ile çağırır.
    /// Eski HarcamaSurecleri/PSLDinBordroOrtak.cs içindeki BordroRaporlari sınıfından alındı; değişenler:
    ///   - System.Web, Ext.NET ve TNS.Raporlama.IRapor arayüzü kaldırıldı (RaporlamaServis RaporAl'ı yansımayla bulur).
    ///   - RaporAl'ın kullandığı eski PSLDinBordroOrtak yardımcıları (AdresCoz, HucreyeYaz, ToplamYaz, YazilsinMi, YerBul,
    ///     TarihYaz, OncekiIleBirlestir) Core'da olmadığından bu sınıfa private olarak aynen alındı.
    ///   - Eski BordroTanim.DosyaYol(false) (eski BordroTanim sayfasında) yerine aynı mantıktaki SablonDosyaYol kullanılır
    ///     (HarcamaDosyaYol\BordroSablonlar; Core'daki BordroTanimUclari.DosyaYol private olduğu için kopyalandı).
    ///   - Sonuç dosyası adı: RaporlamaServis'in verdiği sonucDosyaAd + ".xlsx" (eski kod her seferinde yeni geçici ad
    ///     üretiyordu; RaporlamaServis boşsa zaten geçici ad verir).
    ///   - Ekranlarda kullanılmayan DosyaUretDinamik (ve yalnızca onun kullandığı DegiskenGrup / NesneListesi) taşınmadı.
    /// </summary>
    public class BordroRaporlari
    {
        public static Hashtable ht = new Hashtable();
        public static string[] genelAlanlar = { "Yil", "Ay", "AyAd", "SayKod", "SayAd", "BirKod", "BirAd", "SayBirAd", "SurecYil", "SurecNo", "BordroTur", "OdemeTarih", "DYil", "DAy", "DAyAd", "PersonelTipAd", "EPosta", "MuhasebatNo", "BordroIslemAd", "gorevYeri", "birimYeri", "SendikaKod", "SendikaAd", "SendikaAdres", "SendikaIBAN", "DIlkGun", "DSonGun", "Hazirlayan", "OnayaSunan", "Onaylayan" };
        public static string[] genelTipler = { "Tamsayı", "Tamsayı", "Yazı", "Tamsayı", "Yazı", "Yazı", "Yazı", "Yazı", "Tamsayı", "Yazı", "Tamsayı", "Tarih", "Tamsayı", "Tamsayı", "Yazı", "Yazı", "Yazı", "Yazı", "Yazı", "Yazı", "Yazı", "Tamsayı", "Yazı", "Yazı", "Yazı", "Tarih", "Tarih", "Yazı", "Yazı", "Yazı" };
        public static string[] genelEtiketler = { "Yıl", "Ay", "Ay Adı", "Saymanlık Kodu", "Saymanlık Adı", "Birim Kodu", "Birim Adı", "Saymanlık Birim Adı", "Sureç Yılı", "Sureç No", "Bordro Türü", "Ödeme Tarihi", "Dönem Yılı", "Dönem Ay", "Dönem Ay Adı", "Personel Tip Adı", "ePosta Adresi", "Muhasebat No", "Ödeme Türü", "Görev Yeri", "Kadro Yeri", "Sendika Kodu", "Sendika Adı", "Sendika Adresi", "Hesap No/IBAN", "Dönem İlk Günü", "Dönem Son Günü", "Hazırlayan", "Onaya Sunan", "Onaylayan" };

        public static bool PersonelAranacakMi(int genelAlanNo)//genelAlanNo
        {
            return (genelAlanNo == 15 || genelAlanNo == 16 || genelAlanNo == 17 || genelAlanNo == 19 || genelAlanNo == 20);
        }

        public static void OrtakDegerBul(int ii, KisiSatir kisiSatir, BordroYapi by, ref object o, ref VariableType vt, ObjectArray personeller, int sendikaBilgiYer)
        {
            if (kisiSatir.kisiSatirBilgi != null)
            {
                if (ii == 0) { o = kisiSatir.kisiSatirBilgi.odemeTarih.Yil; vt = VariableType.Integer; }
                else if (ii == 1) { o = kisiSatir.kisiSatirBilgi.odemeTarih.Ay; vt = VariableType.Integer; }
                else if (ii == 2) { o = GenelIslemler.AyAdDondur(kisiSatir.kisiSatirBilgi.odemeTarih.Ay); }
                else if (ii == 3) { o = kisiSatir.kisiSatirBilgi.muhasebeKod; vt = VariableType.Integer; }
                else if (ii == 4) { o = kisiSatir.kisiSatirBilgi.muhasebeAd; }
                else if (ii == 5) { o = kisiSatir.kisiSatirBilgi.birimKod; }
                else if (ii == 6) { o = kisiSatir.kisiSatirBilgi.birimAd; }
                else if (ii == 7) { o = TNS.MUH.Arac.BirimAdOl(kisiSatir.kisiSatirBilgi.muhasebeAd, kisiSatir.kisiSatirBilgi.birimAd); }
                else if (ii == 8) { o = kisiSatir.kisiSatirBilgi.surecYil; vt = VariableType.Integer; }
                else if (ii == 9) { o = kisiSatir.kisiSatirBilgi.surecNo; }
                else if (ii == 11) { o = kisiSatir.kisiSatirBilgi.odemeTarih.Oku(); vt = VariableType.Date; }
                else if (ii == 12) { o = kisiSatir.kisiSatirBilgi.oncBordroTarih.Yil; vt = VariableType.Integer; }
                else if (ii == 13) { o = kisiSatir.kisiSatirBilgi.oncBordroTarih.Ay; vt = VariableType.Integer; }
                else if (ii == 14) { o = GenelIslemler.AyAdDondur(kisiSatir.kisiSatirBilgi.oncBordroTarih.Ay); }
                else if (ii == 25) { o = kisiSatir.kisiSatirBilgi.oncBordroTarih.Oku(); }
                else if (ii == 26) { o = kisiSatir.kisiSatirBilgi.oncBordroTarih.Oku().AddMonths(1).AddDays(-1); }
                else if (ii == 27) { o = kisiSatir.kisiSatirBilgi.hazirlayan; }
                else if (ii == 28) { o = kisiSatir.kisiSatirBilgi.onayaSunan; }
                else if (ii == 29) { o = kisiSatir.kisiSatirBilgi.onaylayan; }

            }
            if (ii == 10) { o = by.bordroTur; vt = VariableType.Integer; }
            else if (PersonelAranacakMi(ii))
            {
                o = kisiselBilgiBul(kisiSatir, personeller, ii);
                vt = VariableType.String;
            }
            else if (ii == 18)
            {
                o = BordroIslemTur.TurYapiAdBul((BordroIslemTur.ENUMBordroIslemTurYapi)by.bordroIslemTur);
                vt = VariableType.String;
            }
            else if (ii == 21)
            {
                o = SendikaNoBul(kisiSatir, by, sendikaBilgiYer);
                vt = VariableType.Integer;
            }
            else if (ii == 22 || ii == 23 || ii == 24)
            {
                vt = VariableType.String;
                int sn = SendikaNoBul(kisiSatir, by, sendikaBilgiYer);
                TNS.IKY.SendikaBilgi sb = TNS.IKY.Arac.SendikaBilgi(sn);
                if (sb != null)
                {
                    switch (ii)
                    {
                        case 22:
                            o = sb.sendikaAd;
                            break;
                        case 23:
                            o = sb.adres;
                            break;
                        case 24:
                            o = sb.iBAN;
                            break;
                        default:
                            break;
                    }
                }
                else
                    o = null;
            }
        }

        private static int SendikaNoBul(KisiSatir kisiSatir, BordroYapi by, int sendikaBilgiYer)
        {
            if (sendikaBilgiYer >= 0)
            {
                string s = (string)kisiSatir.degerler[sendikaBilgiYer];
                return OrtakFonksiyonlar.ConvertToInt(s.Split('-')[0], 0);
            }
            return -1;
        }

        private static string kisiselBilgiBul(KisiSatir kisiSatir, ObjectArray personeller, int id)
        {
            if (personeller != null && personeller.objeler != null)
            {
                foreach (TNS.IKY.Personel item in personeller.objeler)
                {
                    if (item.tcKimlikNo == (string)kisiSatir.degerler[0])
                    {
                        if (id == 15)
                            return item.personelTipAd;
                        else if (id == 16)
                            return item.iletisim.eposta;
                        else if (id == 19)
                            return item.kadro.birimGorevAd;
                        else if (id == 20)
                            return item.birimAd;
                        else
                            return item.geldigiKurumSicili;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// RaporlamaServis.MetodCagir bu imzayla çağırır: (kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi).
        /// Üretilen .xlsx dosyasının tam yolunu döndürür.
        /// </summary>
        public string RaporAl(object o_kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {

            Kullanici kullanan = (Kullanici)o_kullanan;
            IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            sonucDosyaAd = (string.IsNullOrWhiteSpace(sonucDosyaAd) ? DosyaIslem.DosyaAdUret("", "") : sonucDosyaAd) + ".xlsx";

            BordroSablonDosya bsd = new BordroSablonDosya();

            bsd.bordroTur = 0;
            bsd.islemTur = 0;
            bsd.sablonKod = OrtakFonksiyonlar.ConvertToInt((string)raporParam["sablonNo"], 0);
            if (bsd.sablonKod > 0)
                sablonAd = Path.Combine(SablonDosyaYol(), bsd.SablonDosyaAd);

            BordroKistas bordroKistas = new BordroKistas(0, null, new TNSCollection((string)raporParam["tcKimlikNo"]));
            bordroKistas.donemYil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["yil"], 0);
            bordroKistas.donemAy = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ay"], 0);

            bordroKistas.bordroTur = OrtakFonksiyonlar.ConvertToInt((string)raporParam["bTur"], 0);
            bordroKistas.bordroIslem = OrtakFonksiyonlar.ConvertToInt((string)raporParam["bIslem"], 0);

            Tablo XLS = GenelIslemler.NewTablo();

            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            try
            {

                int satir = 0;
                int sutun = 0;

                XLS.HucreAdBulYaz("Yil", bordroKistas.donemYil);
                XLS.HucreAdBulYaz("Ay", bordroKistas.donemAy);
                XLS.HucreAdBulYaz("AyAd", OrtakFonksiyonlar.AyAdiGetir(bordroKistas.donemAy));

                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                int bordroSatBasla = 0, bordroSutBasla = 0, bordroSatBit = 0, bordroSutBit = 0;
                int bordroSatBasla2 = 0, bordroSutBasla2 = 0, bordroSatBit2 = 0, bordroSutBit2 = 0;
                int gtSatBasla = 0, gtSutBasla = 0, gtSatBit = 0, gtSutBit = 0;
                int stSatBasla = 0, stSutBasla = 0, stSatBit = 0, stSutBit = 0;
                int sheetNo = 0;
                int sheetNo2 = 0;
                XLS.HucreAdAdresCoz("BordroSatir", ref sheetNo, ref bordroSatBasla, ref bordroSutBasla, ref bordroSatBit, ref bordroSutBit);
                int satirSay = bordroSatBit - bordroSatBasla + 1;
                XLS.HucreAdAdresCoz("BordroSatir2", ref sheetNo2, ref bordroSatBasla2, ref bordroSutBasla2, ref bordroSatBit2, ref bordroSutBit2);
                if (bordroSatBasla2 < 1)
                {
                    sheetNo2 = sheetNo;
                    bordroSatBasla2 = bordroSatBasla;
                    bordroSutBasla2 = bordroSutBasla;
                    bordroSatBit2 = bordroSatBit;
                    bordroSutBit2 = bordroSutBit;
                }
                int siraSatir = 0, siraSutun = 0, siraSatirBit = 0, siraSutunBit = 0;
                XLS.HucreAdAdresCoz("SayfaToplam", ref sheetNo, ref stSatBasla, ref stSutBasla, ref stSatBit, ref stSutBit);
                XLS.HucreAdAdresCoz("GenelToplam", ref sheetNo, ref gtSatBasla, ref gtSutBasla, ref gtSatBit, ref gtSutBit);
                XLS.HucreAdAdresCoz("SiraNo", ref sheetNo, ref siraSatir, ref siraSutun, ref siraSatirBit, ref siraSutunBit);
                siraSatir = siraSatir - bordroSatBasla;
                siraSutun = siraSutun - bordroSutBasla;
                siraSatirBit = siraSatirBit - bordroSatBasla;
                siraSutunBit = siraSutunBit - bordroSutBasla;

                int satirYuk = OrtakFonksiyonlar.ConvertToInt(XLS.HucreAdDegerAl("SatirYuk"), -1);
                if (satirYuk > 0) satirYuk = satirYuk * 1010;
                int toplamAyniSayfada = OrtakFonksiyonlar.ConvertToInt(XLS.HucreAdDegerAl("ToplamAyniSayfada"), 0);
                string tut = XLS.HucreAdDegerAl("SayfaKisi");
                int sayfaKisi = OrtakFonksiyonlar.ConvertToInt(tut, 0);

                bordroKistas.siralama = XLS.HucreAdDegerAl("Siralama");
                ObjectArray oa;
                try
                {
                    oa = servisHRC.CalisanDinamikBordroListele(kullanan, bordroKistas);
                }
                catch (Exception e)
                {
                    XLS.HucreDegerYaz(0, 0, e.Message);
                    XLS.DosyaSaklaTamYol();
                    return sonucDosyaAd;
                }
                int[,] satirYerler = null;
                int[,] satirYerlerOnc = null;
                bool oncVar = false;
                BordroYapi by = null;
                int ilkSatir = satir;

                ToplamAl t = new ToplamAl(3, 200);
                bool tek = true;
                bool ilk = true;
                int j = 0;
                foreach (CalisanDinamikBordroDonem item in oa.objeler)
                {
                    if (ilk)
                    {
                        ilk = false;
                        XLS.HucreAdBulYaz("DYil", item.bordroDonemBasTarihDT.Year);
                        XLS.HucreAdBulYaz("DAy", item.bordroDonemBasTarihDT.Month);
                        XLS.HucreAdBulYaz("DAyAd", GenelIslemler.AyAdDondur(item.bordroDonemBasTarihDT.Month));

                        XLS.HucreAdBulYaz("SayKod", item.muhasebeKod);
                        XLS.HucreAdBulYaz("SayAd", item.muhasebeAd);
                        XLS.HucreAdBulYaz("BirKod", item.birimKod);
                        XLS.HucreAdBulYaz("BirAd", item.birimAd);
                        XLS.HucreAdBulYaz("SayBirAd", TNS.MUH.Arac.BirimAdOl(item.muhasebeAd, item.birimAd));
                        TarihYaz(XLS, "DIlkGun", item.bordroDonemBasTarihDT);
                        TarihYaz(XLS, "DSonGun", item.sonGunDT);
                    }
                    bool pKisiselBilgi = false;
                    ObjectArray personeller;

                    by = item.bordroYapi;
                    satirYerler = new int[by.hepsi.Length + 1 + genelAlanlar.Length, 12]; //+1 tckimlikno icin
                    satirYerlerOnc = new int[by.hepsi.Length + 1 + genelAlanlar.Length, 12]; //+1 tckimlikno icin
                    int sendikaBilgiYer = -1;
                    for (int i = 0; i < by.hepsi.Length + 1 + genelAlanlar.Length; i++)
                    {
                        string ad;
                        int genelAlanNo = i - (by.hepsi.Length + 1);
                        if (i == 0) ad = "tckimlikno";
                        else if (genelAlanNo >= 0) ad = genelAlanlar[genelAlanNo];
                        else ad = by.hepsi[i - 1].ad;

                        if (ad == "sendikaBilgi")
                            sendikaBilgiYer = i;

                        AdresCoz(XLS, "a_" + ad, satirYerler, i, 0, bordroSatBasla, bordroSutBasla, bordroSatBit, bordroSutBit);
                        AdresCoz(XLS, "s_" + ad, satirYerler, i, 4, stSatBasla, stSutBasla, stSatBit, stSutBit);
                        AdresCoz(XLS, "t_" + ad, satirYerler, i, 8, gtSatBasla, gtSutBasla, gtSatBit, gtSutBit);

                        AdresCoz(XLS, "o_a_" + ad, satirYerlerOnc, i, 0, bordroSatBasla, bordroSutBasla, bordroSatBit, bordroSutBit);
                        AdresCoz(XLS, "o_s_" + ad, satirYerlerOnc, i, 4, stSatBasla, stSutBasla, stSatBit, stSutBit);
                        AdresCoz(XLS, "o_t_" + ad, satirYerlerOnc, i, 8, gtSatBasla, gtSutBasla, gtSatBit, gtSutBit);
                        if (!oncVar && satirYerlerOnc[i, 0] >= 0) oncVar = true;
                        if ((satirYerler[i, 0] >= 0))
                        {
                            if (!pKisiselBilgi && PersonelAranacakMi(genelAlanNo))
                                pKisiselBilgi = true;
                        }
                    }
                    if (oncVar)
                    {
                        OncekiIleBirlestir(servisHRC, kullanan, item.calisanDinamikBordro, by, item.yil, item.surecNo, null);
                    }

                    KisiselVerileriBul(kullanan, out personeller, item);

                    int alanNo = 0;
                    int kosulIndeks = -1;
                    double degerDbl = 0;
                    string kosulAlan = XLS.HucreAdDegerAl("DoluAlan");
                    if (!string.IsNullOrWhiteSpace(kosulAlan))
                    {
                        int kosulYer;
                        YerBul(kosulAlan, kosullar, out kosulIndeks, out kosulYer);
                        if (kosulIndeks >= 0 && kosulYer > 0)
                        {
                            string alan = kosulAlan.Substring(0, kosulYer);
                            alanNo = by.YerBul(alan, 0) + 1;
                            if (alanNo == 0) alanNo = -1000;
                            string deger = kosulAlan.Substring(kosulYer + kosullar[kosulIndeks].Length);
                            degerDbl = OrtakFonksiyonlar.ConvertToDbl(deger);
                        }
                    }

                    for (int jj = 0; jj < item.calisanDinamikBordro.kisiDegerler.Count; jj++)
                    {
                        KisiSatir kisiSatir = (KisiSatir)item.calisanDinamikBordro.kisiDegerler[jj];
                        object[] degerler = kisiSatir.degerler;

                        if (!YazilsinMi(degerler, alanNo, kosulIndeks, degerDbl)) continue;

                        if (tek)
                            XLS.HucreKopyala(bordroSatBasla, bordroSutBasla, bordroSatBit, bordroSutBit, satir, sutun);
                        else
                            XLS.HucreKopyala(bordroSatBasla2, bordroSutBasla2, bordroSatBit2, bordroSutBit2, satir, sutun);
                        tek = !tek;
                        if (siraSatir >= 0 && siraSutun >= 0)
                        {
                            XLS.HucreDegerYaz(satir + siraSatir, sutun + siraSutun, j + 1);
                        }

                        for (int i = by.hepsi.Length + genelAlanlar.Length; i >= 0; i--)
                        {
                            int simdOncSay = oncVar ? 2 : 1;
                            for (int simdOnc = 0; simdOnc < simdOncSay; simdOnc++)
                            {
                                int satIc = 0, sutIc = 0;
                                AdresCoz(simdOnc == 0 ? satirYerler : satirYerlerOnc, i, 0, satir, sutun, ref satIc, ref sutIc);
                                if (satIc > 0)
                                {
                                    string ad;
                                    VariableType vt = VariableType.String;
                                    object o = (string)null;
                                    int ii = -1;
                                    if (i >= by.hepsi.Length + 1)
                                    {
                                        ii = i - (by.hepsi.Length + 1);
                                        ad = genelAlanlar[ii];
                                        OrtakDegerBul(ii, kisiSatir, by, ref o, ref vt, personeller, sendikaBilgiYer);
                                    }
                                    else
                                    {
                                        if (simdOnc == 0 && degerler != null)
                                            o = degerler[i];
                                        else if (simdOnc == 1 && kisiSatir.degerlerOnc != null)
                                            o = kisiSatir.degerlerOnc[i];
                                        if (i == 0) ad = "tckimlikno";
                                        else
                                        {
                                            ad = by.hepsi[i - 1].ad;
                                            vt = by.hepsi[i - 1].tip;
                                        }
                                    }
                                    HucreyeYaz(XLS, satIc, sutIc, o, vt, t, i);
                                }
                            }
                        }
                        t.ToplamEkle(1, 2);
                        t.ToplamEkle(0, 2);
                        satir += satirSay;
                        if (sayfaKisi > 0)
                        {
                            if (((j + 1) % sayfaKisi) == 0)
                            {
                                ToplamYaz(XLS, ref satir, sutun, "s_", satirYerler, 4, stSatBasla, stSutBasla, stSatBit, stSutBit, t, 1, by, 0);
                                t.SatirSifirla(1);
                                XLS.SayfaSonuKoy(satir);
                            }
                        }
                        j++;
                    }
                }

                if (oa.ObjeSayisi > 0)
                {
                    if (toplamAyniSayfada == 0)
                        XLS.SayfaSonuKoy(satir);
                    ToplamYaz(XLS, ref satir, sutun, "t_", satirYerler, 8, gtSatBasla, gtSutBasla, gtSatBit, gtSutBit, t, 0, by, 0);
                    if (satirYuk > 0)
                    {
                        int baslamaSat = ilkSatir;
                        XLS.SatirYukseklikAyarla(baslamaSat, satir, satirYuk);
                    }
                }
                int satirSil = OrtakFonksiyonlar.ConvertToInt(XLS.HucreAdDegerAl("SatirSil"), -1);
                int sutunSil = OrtakFonksiyonlar.ConvertToInt(XLS.HucreAdDegerAl("SutunSil"), -1);

                if (satirSil > 0 || sutunSil > 0)
                {
                    XLS.FormulleriSil();
                    if (satirSil > 0)
                        XLS.SatirSil(0, satirSil - 1);
                    if (sutunSil > 0)
                        XLS.SutunSil(0, sutunSil - 1);
                }
            }
            catch (Exception ex2)
            {
                DebugYaz(kullanan, ex2.Message);
                XLS.HucreDegerYaz(0, 0, ex2.Message + " (Ana akış)");
            }
            if (sonucDosyaAd.EndsWith("pdf"))
            {
                XLS.DosyaSaklamaFormatAta("pdf");
            }
            try
            {
                XLS.DosyaSaklaTamYol();
            }
            catch (Exception ex3)
            {
                DebugYaz(kullanan, ex3.Message);
            }
            return sonucDosyaAd;
        }

        private static void DebugYaz(Kullanici kullanan, string yazi, bool sifirla = false)
        {
            //if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            //    OrtakFonksiyonlar.HataDosyaYaz("Debug.txt", new TNSDateTime(DateTime.Now).ToTimeStamp() + ": " + yazi, sifirla);
        }

        static void KisiselVerileriBul(Kullanici kullanan, out ObjectArray personeller, CalisanDinamikBordroDonem donem)
        {
            personeller = new ObjectArray();

            TNS.IKY.Personel pSorgu = new TNS.IKY.Personel();
            pSorgu.tcList = new TNSCollection();
            pSorgu.bordroDisiDahil = false;

            int parcaBoyutu = 5000;

            for (int jj = 0; jj < donem.calisanDinamikBordro.kisiDegerler.Count; jj++)
            {
                KisiSatir kisiSatir = (KisiSatir)donem.calisanDinamikBordro.kisiDegerler[jj];
                object[] degerler = (object[])kisiSatir.degerler;
                string tcNo = (string)degerler[0];

                if (!pSorgu.tcList.Contains(tcNo))
                    pSorgu.tcList.Add(tcNo);

                if (pSorgu.tcList.Count >= parcaBoyutu)
                {
                    SorguYapVeEkle(kullanan, pSorgu, personeller);

                    pSorgu.tcList = new TNSCollection();
                    pSorgu.bordroDisiDahil = false;
                }
            }

            if (pSorgu.tcList.Count > 0)
            {
                SorguYapVeEkle(kullanan, pSorgu, personeller);
            }

        }

        private static void SorguYapVeEkle(Kullanici kullanan, TNS.IKY.Personel pSorgu, ObjectArray personeller)
        {
            ObjectArray gelenPersoneller =
                TNS.IKY.Arac.Tanimla().PersonelListele(kullanan, pSorgu, false, true);

            if (gelenPersoneller == null)
                return;

            personeller.objeler.AddRange(gelenPersoneller.objeler);
        }

        // ------------------------------------------------------------------ eski PSLDinBordroOrtak yardımcıları (aynen)

        /// <summary>Eski BordroTanim.DosyaYol(false): HarcamaDosyaYol\BordroSablonlar (Core BordroTanimUclari.DosyaYol ile aynı).</summary>
        private static string SablonDosyaYol()
        {
            return Path.Combine(Istemci.Ayar.Oku("HarcamaDosyaYol", ""), "BordroSablonlar");
        }

        private static readonly string[] kosullar = { "<=", ">=", "!=", "=", "<", ">" };

        private static bool YazilsinMi(object[] degerler, int alanNo, int kosulIndeks, double degerDbl)
        {
            if (alanNo == -1000) return false;
            if (degerler == null)
            {
                if (kosulIndeks == -1) return true;
                else return false;
            }
            if (alanNo >= 0 && alanNo < degerler.Length)
            {
                double d = 0;
                object o = degerler[alanNo];
                if (o.GetType() == typeof(double))
                {
                    d = (double)o;
                }
                else if (o.GetType() == typeof(int))
                {
                    d = (int)o;
                }
                else
                    return true;
                if (kosulIndeks == 0) return (d <= degerDbl);
                else if (kosulIndeks == 1) return (d >= degerDbl);
                else if (kosulIndeks == 2) return (d != degerDbl);
                else if (kosulIndeks == 3) return (d == degerDbl);
                else if (kosulIndeks == 4) return (d < degerDbl);
                else if (kosulIndeks == 5) return (d > degerDbl);
            }
            return true;
        }

        private static void OncekiIleBirlestir(IHRCServis servisHRC, Kullanici kullanan, CalisanDinamikBordro calisanBordro, BordroYapi by, int surecYil, string surecNo, TNSCollection tcListe)
        {
            CalisanDinamikBordroDonem oncDonem = servisHRC.OncekiDonemBul(surecYil, surecNo);
            if (oncDonem != null)
            {
                BordroKistas bk = new BordroKistas(oncDonem.yil, oncDonem.surecNo, tcListe);

                CalisanDinamikBordro calisanBordroOnc = servisHRC.CalisanDinamikBordroOku(kullanan, by, bk);
                if (calisanBordroOnc != null && calisanBordroOnc.kisiDegerler != null)
                {
                    foreach (KisiSatir oncKisi in calisanBordroOnc.kisiDegerler)
                    {
                        KisiSatir simdKisi = calisanBordro.KisiBul(oncKisi);
                        if (simdKisi == null)
                        {
                            calisanBordro.kisiDegerler.Add(oncKisi);
                            oncKisi.degerlerOnc = oncKisi.degerler;
                            oncKisi.degerler = null;
                        }
                        else
                            simdKisi.degerlerOnc = oncKisi.degerler;
                    }
                }
            }
        }

        private static void TarihYaz(Tablo XLS, string alanAd, DateTime tarih)
        {
            int sat = 0, sut = 0;
            XLS.HucreAdAdresCoz(alanAd, ref sat, ref sut);
            if (sat >= 0 && sut >= 0)
            {
                XLS.HucreDegerYaz(sat, sut, tarih);
            }
        }

        private static void HucreyeYaz(Tablo XLS, int satIc, int sutIc, object o, VariableType vt, ToplamAl t, int i)
        {
            if (o == null) return;
            try
            {
                if (vt == VariableType.Integer)
                {
                    XLS.HucreDegerYaz(satIc, sutIc, (int)o);
                    if (t != null)
                        t.DegerAta(2, i, (int)o);
                }
                else if (vt == VariableType.Money)
                {
                    XLS.HucreDegerYaz(satIc, sutIc, (double)o);
                    if (t != null)
                        t.DegerAta(2, i, (double)o);
                }
                else if (vt == VariableType.Date)
                {
                    XLS.HucreDegerYaz(satIc, sutIc, (DateTime)o);
                    if (t != null)
                        t.DegerAta(2, i, 0);
                }
                else
                {
                    XLS.HucreDegerYaz(satIc, sutIc, (string)o);
                    if (t != null)
                        t.DegerAta(2, i, 0);
                }
            }
            catch (Exception ex)
            {
                XLS.HucreDegerYaz(satIc, sutIc, (o == null ? "NULL" : o.ToString()) + "(" + ex.Message + ")");
            }
        }

        private static void YerBul(string kosulAlan, string[] kosullar, out int kosulIndeks, out int kosulYer)
        {
            kosulIndeks = -1;
            kosulYer = -1;
            if (string.IsNullOrWhiteSpace(kosulAlan)) return;
            if (kosullar == null) return;
            for (int i = 0; i < kosullar.Length; i++)
            {
                kosulYer = kosulAlan.IndexOf(kosullar[i]);
                if (kosulYer >= 0)
                {
                    kosulIndeks = i;
                    return;
                }
            }
        }

        private static void ToplamYaz(Tablo XLS, ref int satir, int sutun, string alanEkle, int[,] satirYerler, int yerIndeks, int satBasla, int sutBasla, int satBit, int sutBit, ToplamAl t, int toplamSat, BordroYapi by, int toplaSay)
        {
            if (satBasla > 0)
            {
                bool ilk = true;
                for (int i = 0; i < by.hepsi.Length + 1; i++)
                {
                    string ad;
                    VariableType vt = VariableType.String;
                    if (i == 0) ad = "tckimlikno";
                    else
                    {
                        ad = by.hepsi[i - 1].ad;
                        vt = by.hepsi[i - 1].tip;
                    }

                    int satIc = 0, sutIc = 0;
                    AdresCoz(satirYerler, i, yerIndeks, satir, sutun, ref satIc, ref sutIc);
                    if (satIc > 0)
                    {
                        if (ilk)
                        {
                            XLS.HucreKopyala(satBasla, sutBasla, satBit, sutBit, satir, sutun);
                            ilk = false;
                        }

                        XLS.HucreDegerYaz(satIc, sutIc, t.toplam[toplamSat, i]);
                        if (alanEkle == "t_")
                        {
                            int btSat = 0, btSut = 0;
                            XLS.HucreAdAdresCoz("bt_" + ad, ref btSat, ref btSut);
                            if (btSat > 0 && btSut > 0)
                                XLS.HucreDegerYaz(btSat, btSut, t.toplam[toplamSat, i]);
                        }
                    }
                }
                if (!ilk)
                {
                    if (alanEkle == "t_")
                    {
                        int btSat = 0, btSut = 0;
                        XLS.HucreAdAdresCoz("bt_" + "SiraNo", ref btSat, ref btSut);
                        if (btSat > 0 && btSut > 0)
                            XLS.HucreDegerYaz(btSat, btSut, toplaSay);
                    }
                }
                satir += (satBit - satBasla + 1);
            }
        }

        private static void AdresCoz(int[,] satirYerler, int dSat, int dSut, int aktifSat, int aktifSut, ref int satIc, ref int sutIc)
        {
            int d1 = satirYerler[dSat, dSut + 0];
            if (d1 >= 0)
            {
                satIc = aktifSat + d1;
                sutIc = aktifSut + satirYerler[dSat, dSut + 1];
            }
            else
            {
                satIc = -1;
                sutIc = -1;
            }
        }

        private static void AdresCoz(Tablo XLS, string ad, int[,] satirYerler, int dSat, int dSut, int satBasla_off, int sutBasla_off, int satBit_off, int sutBit_off)
        {
            int sheetNo = 0;
            int satBasla = 0, sutBasla = 0, satBit = 0, sutBit = 0;
            XLS.HucreAdAdresCoz(ad, ref sheetNo, ref satBasla, ref sutBasla, ref satBit, ref sutBit);
            if (sheetNo >= 0)
            {
                satirYerler[dSat, dSut + 0] = satBasla - satBasla_off;
                satirYerler[dSat, dSut + 1] = sutBasla - sutBasla_off;
                satirYerler[dSat, dSut + 2] = satBit - satBit_off;
                satirYerler[dSat, dSut + 3] = sutBit - sutBit_off;
            }
            else
            {
                satirYerler[dSat, dSut + 0] = -1;
                satirYerler[dSat, dSut + 1] = -1;
                satirYerler[dSat, dSut + 2] = -1;
                satirYerler[dSat, dSut + 3] = -1;
            }
        }
    }
}

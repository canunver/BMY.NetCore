using System;
using System.Data;
using System.IO;
using OrtakClass;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Hizmet Belgesi sekmesinin raporu: eski InsanKaynaklari/PersonelKadro.aspx.cs HizmetBelgesiYazdir kodunun aynısıdır
    /// (IKY\HizmetBelgesi.docx şablonu, Aspose.Words alan yazma + Hizmetler / Birlestirilenler / Borclanmalar bölgeleri, PDF).
    /// Şablon önce kurumun RaporSablonYol\IKY klasöründe, yoksa uygulamayla gelen SGB.Net\RaporSablon\IKY klasöründe aranır.
    /// </summary>
    public static class PersonelKadroRaporlari
    {
        public const string HizmetBelgesiSablon = "HizmetBelgesi.docx";

        public static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "IKY", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "IKY", sablonAd);
            return File.Exists(u) ? u : null;
        }

        /// <summary>Eski HizmetBelgesiYazdir. Üretilen geçici PDF dosyasının yolunu döndürür (hata olursa null ve hata metni).</summary>
        public static string HizmetBelgesiYazdir(Kullanici kullanan, IIKYServis servisIKY, int personelKod, string dosyaAd, out string hata)
        {
            hata = null;
            TNS.IKY.Personel personel = servisIKY.HizmetBelgesiRaporu(kullanan, personelKod);
            if (personel == null || personel.kod == 0)
            {
                hata = "Personel bilgileri okunamadı.";
                return null;
            }
            string sonucDosyaad = Path.Combine(TNS.OrtakFonksiyonlar.GeciciYolBul(), Path.GetRandomFileName()) + ".pdf";
            string birim = "", unvan = "";

            AsposeLisans.Yukle();
            Aspose.Words.Document doc = new Aspose.Words.Document(dosyaAd);
            Aspose.Words.DocumentBuilder docb = new Aspose.Words.DocumentBuilder(doc);

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "TC", personel.tcKimlikNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "ESicil", personel.emekliSicilNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KSicil", personel.sicilNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "SSicil", personel.sskSicilNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Ad", personel.ad + " " + personel.soyad);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Dogum", (personel.nufus != null ? personel.nufus.dogumYeri : "") + " / " + (personel.nufus != null ? personel.dogumTarihi.ToString() : ""));

            string hazirlik = "", lisansustu = "", sonOgrenim = "", ilkOgrenim = "";
            int ogrenimKod = 0;
            if (personel.ogrenim != null)
            {
                foreach (TNS.IKY.PersonelOgrenim o in personel.ogrenim)
                {
                    if (o == null) continue;
                    string[] parca = (o.ogrenimKodKayitKod ?? "").Split('.');
                    ogrenimKod = OrtakFonksiyonlar.ConvertToInt(parca.Length > 1 ? parca[1] : "", 0);
                    hazirlik = "";
                    if (o.memuriyeteBasladigiOkul == 1)
                    {
                        ilkOgrenim = o.ogrenimAdi + " / " + o.okulAdi + " / Öğr. Süresi: " + o.ogrenimSuresi + " / " + o.belgeTarihi.ToString();

                        if (o.hazirlikVarMi == 1)
                            hazirlik = "Var";
                        else
                            hazirlik = "Yok";
                    }
                    if (o.sonOkul == 1)
                        sonOgrenim = o.ogrenimAdi + " / " + o.okulAdi + " / Öğr. Süresi: " + o.ogrenimSuresi + " / " + o.belgeTarihi.ToString();

                    if (ogrenimKod == (int)ENUMOgrenimKod.YuksekLisans || ogrenimKod == (int)ENUMOgrenimKod.Doktora)
                        lisansustu += o.ogrenimAdi + " / " + o.okulAdi + " / Öğr. Süresi: " + o.ogrenimSuresi + " / " + o.belgeTarihi.ToString() + "\n";
                }
            }

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Ogrenim", ilkOgrenim);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "SonOgrenim", sonOgrenim);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Hazirlik", hazirlik);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Lisansustu", lisansustu);

            string askerlik = "";
            if (personel.askerlik != null && personel.askerlik.kod != 0)
            {
                YilAyGun result = GetYMD(personel.askerlik.askerlikBaslamaTarihi.Oku(), personel.askerlik.terhisTarihi.Oku());
                askerlik = personel.askerlik.askerlikDurumuAd + " " + personel.askerlik.askerlikBaslamaTarihi.ToString() + " - " + personel.askerlik.terhisTarihi.ToString() + " " + result.Years + " yıl " + result.Months + " ay " + result.Days + " gün ";
            }
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Askerlik", askerlik);

            int hizmetGunu = 0, borclanmaGunu = 0, toplamHizmetGunu = 0, KAHGunu = 0, KHAGunu = 0, EKEAGunu = 0, BHGun = 0;
            int hizmetCnt = 0;
            int birlestirilenCnt = 0;
            int borcCnt = 0;

            DataTable tabHizmetler = new DataTable();
            tabHizmetler.TableName = "Hizmetler";
            tabHizmetler.Columns.Add("Gorev");
            tabHizmetler.Columns.Add("hizmetUnvan");
            tabHizmetler.Columns.Add("Sinif");
            tabHizmetler.Columns.Add("KadroDerece");
            tabHizmetler.Columns.Add("ODED");
            tabHizmetler.Columns.Add("ODEK");
            tabHizmetler.Columns.Add("ODEE");
            tabHizmetler.Columns.Add("KHAD");
            tabHizmetler.Columns.Add("KHAK");
            tabHizmetler.Columns.Add("KHAE");
            tabHizmetler.Columns.Add("EMED");
            tabHizmetler.Columns.Add("EMEK");
            tabHizmetler.Columns.Add("EMEE");
            tabHizmetler.Columns.Add("HBaslamaTarihi");
            tabHizmetler.Columns.Add("HBitisTarihi");
            tabHizmetler.Columns.Add("HizmetSebep");

            DataTable tabBorclanmalar = new DataTable();
            tabBorclanmalar.TableName = "Borclanmalar";
            tabBorclanmalar.Columns.Add("BSebep");
            tabBorclanmalar.Columns.Add("BBaslamaTarihi");
            tabBorclanmalar.Columns.Add("BBitisTarihi");
            tabBorclanmalar.Columns.Add("BorclanmaGun");

            DataTable tabBHizmetler = new DataTable();
            tabBHizmetler.TableName = "Birlestirilenler";
            tabBHizmetler.Columns.Add("BHSebep");
            tabBHizmetler.Columns.Add("BHBaslamaTarihi");
            tabBHizmetler.Columns.Add("BHBitisTarihi");
            tabBHizmetler.Columns.Add("OnSekizAlti");
            tabBHizmetler.Columns.Add("PrimGun");
            tabBHizmetler.Columns.Add("KHADurum");
            tabBHizmetler.Columns.Add("HizmetGun");
            tabBHizmetler.Columns.Add("KidemGun");
            tabBHizmetler.Columns.Add("KHAGun");
            tabBHizmetler.Columns.Add("EKEAGun");

            if (personel.kadrolar != null && personel.kadrolar.ObjeSayisi > 0)
            {
                foreach (TNS.IKY.PersonelKadro k in personel.kadrolar.objeler)
                {
                    if (k.eskiHizmetTipiFlag == 1) continue;
                    // Eski kodda olduğu gibi: personelin kadro birimi boşsa sabit kurum adı yazılır.
                    if (personel.kadro == null || string.IsNullOrEmpty(personel.kadro.birimAd))
                        birim = "CUMHURBAŞKANLIĞI GENEL SEKRETERLİĞİ";
                    else
                        birim = k.birimAd;
                    unvan = k.unvanAd;
                    if (k.hizmetHesabi == ENUMHizmetTablolari.Borclanma)
                    {
                        DataRow row = tabBorclanmalar.NewRow();
                        row["BSebep"] = TNS.IKY.Arac.HitapBorcNeviAd(k.hitapBorcNevi);
                        row["BBaslamaTarihi"] = k.atamaTarihi.ToString();
                        row["BBitisTarihi"] = k.ayrilmaTarihi.ToString();
                        row["BorclanmaGun"] = k.hizmetGunSayisi;
                        borclanmaGunu += k.hizmetGunSayisi;
                        tabBorclanmalar.Rows.Add(row);
                        borcCnt++;
                    }
                    else if (k.hizmetHesabi == ENUMHizmetTablolari.SSK || k.hizmetHesabi == ENUMHizmetTablolari.Bagkur)
                    {
                        birlestirilenCnt++;
                        DataRow row = tabBHizmetler.NewRow();
                        row["BHSebep"] = TNS.IKY.Arac.ENUMSGKNeviAd(k.hitapsgkNevi) + " (" + k.gorevYeri + ")";
                        row["BHBaslamaTarihi"] = k.atamaTarihi.ToString();
                        row["BHBitisTarihi"] = k.ayrilmaTarihi.ToString();
                        row["OnSekizAlti"] = "";
                        row["PrimGun"] = k.hizmetGunSayisi;
                        row["KHADurum"] = TNS.IKY.Arac.HitapKHADurum(k.hitapkhaDurum);
                        row["HizmetGun"] = k.hizmetGunSayisi;
                        row["KidemGun"] = "";
                        row["KHAGun"] = "";
                        row["EKEAGun"] = k.hizmetGunSayisi;
                        BHGun += k.hizmetGunSayisi;
                        tabBHizmetler.Rows.Add(row);
                    }
                    else if (k.hizmetHesabi == ENUMHizmetTablolari.EmekliSandigi)
                    {
                        hizmetCnt++;
                        DataRow row = tabHizmetler.NewRow();
                        row["Gorev"] = k.birimGorevAd;
                        row["hizmetUnvan"] = k.unvanAd;
                        row["Sinif"] = k.sinifKisaAd;
                        row["KadroDerece"] = k.kadroDerece;
                        row["ODED"] = k.odemeyeEsasDerece;
                        row["ODEK"] = k.odemeyeEsasKademe;
                        row["ODEE"] = k.odemeyeEsasEkGosterge;
                        row["KHAD"] = k.kazanilmisHakDerece;
                        row["KHAK"] = k.kazanilmisHakKademe;
                        row["KHAE"] = k.kazanilmisEkGosterge;
                        row["EMED"] = k.emekliyeEsasDerece;
                        row["EMEK"] = k.emekliyeEsasKademe;
                        row["EMEE"] = k.emekliyeEsasEkGosterge;
                        row["HBaslamaTarihi"] = k.atamaTarihi.ToString();
                        row["HBitisTarihi"] = "-";
                        row["HizmetSebep"] = k.hizmetSebebiAd;
                        hizmetGunu += k.hizmetGunSayisi;
                        tabHizmetler.Rows.Add(row);
                        if (k.ayrilmaGerekcesiTip == ENUMHizmetSebebiNevi.AYRILMA)
                        {
                            row = tabHizmetler.NewRow();
                            row["Gorev"] = k.gorevYeri;
                            row["hizmetUnvan"] = k.unvanAd;
                            row["Sinif"] = k.sinifKisaAd;
                            row["KadroDerece"] = k.kadroDerece;
                            row["ODED"] = k.odemeyeEsasDerece;
                            row["ODEK"] = k.odemeyeEsasKademe;
                            row["ODEE"] = k.odemeyeEsasEkGosterge;
                            row["KHAD"] = k.kazanilmisHakDerece;
                            row["KHAK"] = k.kazanilmisHakKademe;
                            row["KHAE"] = k.kazanilmisEkGosterge;
                            row["EMED"] = k.emekliyeEsasDerece;
                            row["EMEK"] = k.emekliyeEsasKademe;
                            row["EMEE"] = k.emekliyeEsasEkGosterge;
                            row["HizmetSebep"] = k.ayrilmaGerekcesiAd;
                            row["HBaslamaTarihi"] = "-";
                            row["HBitisTarihi"] = k.ayrilmaTarihi.ToString();
                            tabHizmetler.Rows.Add(row);
                        }
                    }
                }

                if (hizmetCnt == 0)
                {
                    DataRow row = tabHizmetler.NewRow();
                    foreach (DataColumn c in tabHizmetler.Columns) row[c] = "";
                    tabHizmetler.Rows.Add(row);
                }
                if (borcCnt == 0)
                {
                    DataRow row = tabBorclanmalar.NewRow();
                    foreach (DataColumn c in tabBorclanmalar.Columns) row[c] = "";
                    tabBorclanmalar.Rows.Add(row);
                }
                if (birlestirilenCnt == 0)
                {
                    DataRow row = tabBHizmetler.NewRow();
                    foreach (DataColumn c in tabBHizmetler.Columns) row[c] = "";
                    tabBHizmetler.Rows.Add(row);
                }
            }
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "THizmet", Hizmet.YilAyGunDondur(hizmetGunu));
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "BorclanmaHizmeti", Hizmet.YilAyGunDondur(borclanmaGunu));
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "BHizmetSure", Hizmet.YilAyGunDondur(BHGun));

            if (personel.hizmetSureleri != null)
            {
                foreach (Hizmet h in personel.hizmetSureleri) //Tek personel gidiyor 1 satır dönecek
                {
                    if (h == null) continue;
                    toplamHizmetGunu = h.hizmetGunu;
                    KAHGunu = h.kidemGunu;
                    KHAGunu = h.KHAGunu;
                    EKEAGunu = h.EKEAGunu;
                }
            }

            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "ToplamHizmetSuresi", Hizmet.YilAyGunDondur(toplamHizmetGunu));
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KidemAyligiSuresi", Hizmet.YilAyGunDondur(KAHGunu));
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "KHASuresi", Hizmet.YilAyGunDondur(KHAGunu));
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "EKEASuresi", Hizmet.YilAyGunDondur(EKEAGunu));
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Kurum", birim);
            GenelIslemlerIstemci.AsposeAlanaYaz(docb, "Unvan", unvan);

            doc.MailMerge.ExecuteWithRegions(tabHizmetler);
            doc.MailMerge.ExecuteWithRegions(tabBHizmetler);
            doc.MailMerge.ExecuteWithRegions(tabBorclanmalar);

            doc.Save(sonucDosyaad, Aspose.Words.SaveFormat.Pdf);

            servisIKY.PersonelTarihceKaydet(kullanan, new TNS.IKY.PersonelKadro(), new TNS.IKY.PersonelKadro { personelKod = personel.kod }, ENUMTARIHCE.KADRO, personelKod, ENUMTARIHCEISLEMTIP.YAZDIR);

            return sonucDosyaad;
        }

        /// <summary>Eski GetYMD'nin döndürdüğü yıl / ay / gün.</summary>
        private struct YilAyGun
        {
            public int Years;
            public int Months;
            public int Days;
        }

        /// <summary>Eski GetYMD (dynamic yerine yapı; hesap aynı).</summary>
        private static YilAyGun GetYMD(DateTime dt1, DateTime dt2)
        {
            int years = 0;
            for (int i = 0; i <= dt2.Year - dt1.Year; i++)
            {
                if (dt1.AddYears(i) <= dt2)
                {
                    years = i;
                }
            }

            var dt1y = dt1.AddYears(years);

            int months = 0;
            for (int i = 0; i < 12; i++)
            {
                if (dt1y.AddMonths(i) <= dt2)
                {
                    months = i;
                }
            }
            var dt1ym = dt1y.AddMonths(months);
            int days = (int)(dt2 - dt1ym).TotalDays;

            return new YilAyGun { Years = years, Months = months, Days = days };
        }
    }
}

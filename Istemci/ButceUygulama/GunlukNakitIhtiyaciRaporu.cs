using System;
using System.IO;
using OrtakClass;
using TNS;
using TNS.BUM;
using TNS.KYM;

namespace Istemci.ButceUygulama
{
    /// <summary>
    /// Günlük Nakit İhtiyacı Excel raporu; eski ButceUygulama/GunlukNakitIhtiyaci.aspx.cs içindeki RPRGunlukNakitIhtiyaci.Yazdir
    /// kodunun aynısıdır (eski BUMRaporAl.aspx, RaporNo=1). Seçilen tarihin 5 iş günü öncesi ve sonrası, bütün birimler için
    /// (eskisi gibi birimKod "0") günlük toplamlar: satırlar birimler, sütunlar günler, en altta toplam satırı.
    /// Şablon önce kurumun RaporSablonYol\BUM klasöründe, yoksa uygulamayla gelen SGB.Net\RaporSablon\BUM klasöründe aranır.
    /// </summary>
    public static class GunlukNakitIhtiyaciRaporu
    {
        public const string SablonAd = "GunlukNakitIhtiyaci.xltx";

        public static string SablonBul(string uygulamaKlasoru)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "BUM", SablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "BUM", SablonAd);
            return File.Exists(u) ? u : null;
        }

        /// <summary>Raporu üretir, geçici Excel dosyasının yolunu döndürür.</summary>
        public static string Yazdir(Kullanici kullanan, DateTime tarih, string birimKod, string sablonAd)
        {
            IBUMServis servis = TNS.BUM.Arac.Tanimla();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("GunlukNakitIhtiyaci_", "") + "." + GenelIslemler.ExcelTur();

            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            TNSDateTime t = new TNSDateTime(tarih);
            ObjectArray oa = servis.ListeleGunlukNakitIhtiyac(kullanan, t, birimKod, true);

            if (oa == null || !oa.sonuc.islemSonuc || oa.objeler == null)
                throw new Exception("Veri alınamadı");

            int fSatir = 0, fSutun = 0;
            int bSatir = 0, bSutun = 0;

            XLS.HucreAdAdresCoz("FormatSatir", ref fSatir, ref fSutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref bSatir, ref bSutun);

            if (oa.ObjeSayisi > 0)
            {
                NakitPlanGrup ilk = (NakitPlanGrup)oa.objeler[0];
                int kolonSayisi = ilk.GunMiktarlari.Count;

                int baslikSutun = bSutun + 1;
                int baslikSatir = bSatir - 1;
                int sutGen = XLS.SutunGenislikAl(baslikSutun);
                foreach (GunDeger gun in ilk.GunMiktarlari)
                {
                    if (baslikSutun != bSutun + 1)
                    {
                        XLS.HucreKopyala(fSatir, bSutun + 1, baslikSatir, bSutun + 1, fSatir, baslikSutun);
                        XLS.SutunGenislikAyarlaPixel(baslikSutun, baslikSutun, sutGen);
                    }

                    XLS.HucreDegerYaz(baslikSatir, baslikSutun, gun.TarihDT);
                    baslikSutun++;
                }
                int satir = bSatir;

                foreach (NakitPlanGrup detay in oa.objeler)
                {
                    XLS.HucreKopyala(fSatir, fSutun, fSatir, fSutun + detay.GunMiktarlari.Count, satir, bSutun);

                    XLS.HucreDegerYaz(satir, bSutun, detay.birimAd);

                    int sutun = bSutun + 1;
                    foreach (GunDeger gun in detay.GunMiktarlari)
                    {
                        XLS.HucreDegerYaz(satir, sutun, gun.Toplam);
                        sutun++;
                    }
                    satir++;
                }

                int toplamSatir = satir;

                XLS.HucreKopyala(fSatir + 2, fSutun, fSatir + 2, fSutun + kolonSayisi, toplamSatir, bSutun);

                for (int sutun = bSutun + 1; sutun <= bSutun + kolonSayisi; sutun++)
                {
                    string baslangicHucre = XLS.CellIndexToName(bSatir, sutun);
                    string bitisHucre = XLS.CellIndexToName(toplamSatir - 1, sutun);

                    string formul = $"=SUM({baslangicHucre}:{bitisHucre})";

                    XLS.HucreFormulYaz(toplamSatir, sutun, formul);
                }

                XLS.SatirYukseklikAyarla(bSatir, toplamSatir, -1, 7, 17);
            }
            else
                XLS.HucreDegerYaz(bSatir, bSutun, "Veri bulunamadı!");

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }
}

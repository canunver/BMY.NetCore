using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Yatırım raporlarının (YatirimRaporlari.cs, ProjeDosyasiRaporlari.cs) kullandığı, Core'da karşılığı olmayan küçük eski
    /// yardımcılar (eski OrtakClass\Genel.cs GenelIslemlerIstemci'den). Kodları eskisinin aynısıdır.
    /// </summary>
    public static class YatRaporAraclari
    {
        /// <summary>Eski GenelIslemlerIstemci.AsposeAlanaYazHtml: birleştirme alanının yerine HTML yazar.</summary>
        public static void AsposeAlanaYazHtml(Aspose.Words.DocumentBuilder builder, string alanAd, string deger)
        {
            if (deger == null) deger = "";
            while (builder.MoveToMergeField(alanAd))
            {
                //builder.Font.ClearFormatting();
                builder.InsertHtml(deger);
            }
        }

        /// <summary>Eski GenelIslemlerIstemci.VarsayilanKurumBul(out, out).</summary>
        public static void VarsayilanKurumBul(out string kurKod, out string kurAd)
        {
            kurKod = TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD");
            kurAd = TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }

        /// <summary>Eski GenelIslemlerIstemci.VarsayilanKurumBul.</summary>
        public static string VarsayilanKurumBul()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMKOD");
        }

        /// <summary>Eski GenelIslemlerIstemci.VarsayilanKurumBulAd.</summary>
        public static string VarsayilanKurumBulAd()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }
    }

    /// <summary>
    /// Eski YATIzlemeDegerlendirme.aspx.cs sayfasının raporların da kullandığı statik yordamları (aynen alındı; eskisi
    /// AltProjeYollukPlan.PebKodlariBul'u çağırıyordu, burada aynısı olan YollukPlanlamaRaporu.PebKodlariBul çağrılır).
    /// </summary>
    public static class YatIzlemeAraclari
    {
        public static int AyOl(int asama)
        {
            if (asama == 3 || asama == 4) return 3;
            if (asama == 5 || asama == 6) return 6;
            if (asama == 7 || asama == 8) return 9;
            return 12;
        }

        public static string OdenekTabloOl(IBHMv2Servis servisBHMv2, Kullanici kullanan, int yil, int ay, int pKod, int apKod, bool sadeceTablo)
        {
            OdenekBilgi[] odnler = null;
            string pebKodlar = YollukPlanlamaRaporu.PebKodlariBul(kullanan, servisBHMv2, yil, "06.00", pKod, apKod);
            if (!string.IsNullOrWhiteSpace(pebKodlar))
            {
                Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "", pebKodlar);
                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(0), 1));

                EkDegerParam[] ekBilgiler = new EkDegerParam[3];
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "06.00");
                ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniForm, 0, 0, ";0/" + (ay + 1).ToString() + ";" + TNS.BHMv2.Arac.EklemeDusmeIslemler() + ";;");
                ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBHarcama, 0, 0, ay.ToString());

                odnler = servisBHMv2.ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, false);
            }
            string frmt = @"<center><table border=0 cellspacing=0 cellpadding=0 style='border-collapse:collapse;'>
 <tr>
  <td style='text-align:center;font-weight:bold;width:4.5cm;border:solid windowtext 1pt;padding:3pt 3pt 3pt 3pt;'>PROGRAM<br>ÖDENEĞİ (K.B.Ö.)<br>(1)</td>
  <td style='text-align:center;font-weight:bold;width:4.5cm;border:solid windowtext 1pt;padding:3pt 3pt 3pt 3pt;'>REVİZE<br>ÖDENEK<br>(2)</td>
  <td style='text-align:center;font-weight:bold;width:4.5cm;border:solid windowtext 1pt;padding:3pt 3pt 3pt 3pt;'>HARCANAN<br>ÖDENEK<br>(3)</td>
  <td style='text-align:center;font-weight:bold;width:4.5cm;border:solid windowtext 1pt;padding:3pt 3pt 3pt 3pt;'>HARCAMA <br>ORANI (%)<br>(3/2)</td>
 </tr>
 <tr>
  <td style='text-align:center;width:4.5cm;border:solid windowtext 1pt;padding:3pt 3pt 3pt 3pt;'>{0}</td>
  <td style='text-align:center;width:4.5cm;border:solid windowtext 1pt;padding:3pt 3pt 3pt 3pt;'>{1}</td>
  <td style='text-align:center;width:4.5cm;border:solid windowtext 1pt;padding:3pt 3pt 3pt 3pt;'>{2}</td>
  <td style='text-align:center;width:4.5cm;border:solid windowtext 1pt;padding:3pt 3pt 3pt 3pt;'>{3}</td>
 </tr></table></center>";
            if (odnler != null && odnler.Length == 1)
            {
                double ys = odnler[0].EkRakam(0) + odnler[0].EkRakam(1);
                return string.Format(frmt, odnler[0].EkRakam(0).ToString("#,##0"), ys.ToString("#,##0"), odnler[0].EkRakam(2).ToString("#,##0"), OrtakFonksiyonlar.YuzdeBul(odnler[0].EkRakam(2), ys, 2).ToString("#,##0"));
            }
            else
                return string.Format(frmt, "-", "-", "-", "-");
        }

        public static string AnahtarBulGenel(int yil, int asama, string sonEk)
        {
            return "YID" + yil.ToString() + "_" + "G" + "_" + asama.ToString() + "_" + sonEk;
        }

        public static string AnahtarBul(int yil, int pKod, int apKod, int asama, string sonEk)
        {
            return "YID" + yil.ToString() + "_" + pKod.ToString() + "_" + apKod.ToString() + "_" + asama.ToString() + "_" + sonEk;
        }

        public static string AnahtarBulOnay(int pKod, int apKod, int asama)
        {
            return "YID" + pKod.ToString() + "_" + apKod.ToString() + "_" + asama.ToString();
        }
    }
}

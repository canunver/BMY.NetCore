using System;
using System.IO;
using OrtakClass;
using TNS;
using TNS.ATM;
using TNS.MUH;

namespace Istemci.AlacakTakip
{
    /// <summary>
    /// Alacak Takip ödeme sayfalarının Excel raporları; eski kodun aynısıdır:
    ///   OdemeListe      : eski AlacakTakip/Odeme.aspx btnYazdir_Click (ATM\ATM-OdemeListe.xlt)
    ///   TopluOdemeEmri  : eski AlacakTakip/AlacakTakipOdemeEmri.aspx OdemeEmriRapor (ATM\ATM-TopluOdemeEmriListe.xlt)
    /// Şablon önce kurumun RaporSablonYol\ATM klasöründe, yoksa uygulamayla gelen SGB.Net\RaporSablon\ATM klasöründe aranır.
    /// </summary>
    public static class OdemeRaporlari
    {
        public const string OdemeListeSablon = "ATM-OdemeListe.xlt";
        public const string TopluOdemeEmriSablon = "ATM-TopluOdemeEmriListe.xlt";

        public static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "ATM", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "ATM", sablonAd);
            return File.Exists(u) ? u : null;
        }

        /// <summary>Ödeme Bilgileri / Yazdır: dosyanın taksitleri (tutarı sıfır olanlar yazılmaz). Geçici Excel dosyasının yolunu döndürür.</summary>
        public static string OdemeListe(int dosyaNo, ObjectArray bilgi, string sablonAd)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int kaynakSatir = 0;
            int sutun = 0;
            int yazSatir = 0;

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("ATM-OdemeListe_", "") + "." + GenelIslemler.ExcelTur();

            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref yazSatir, ref sutun);

            XLS.HucreAdBulYaz("dosyaNo", dosyaNo);

            foreach (DosyaTaksit item in bilgi.objeler)
            {
                if (item.miktar == 0)
                    continue;

                XLS.SatirAc(yazSatir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 7, yazSatir, sutun);

                XLS.HucreDegerYaz(yazSatir, 0, item.miktar);
                XLS.HucreDegerYaz(yazSatir, 1, item.FaizToplam);
                XLS.HucreDegerYaz(yazSatir, 2, item.SATIRTUTAR);
                XLS.HucreDegerYaz(yazSatir, 3, item.ODEMETARIH.ToShortDateString());

                if (item.GERCEKODEMETARIH.Year > 1)
                    XLS.HucreDegerYaz(yazSatir, 4, item.GERCEKODEMETARIH.ToShortDateString());

                XLS.HucreDegerYaz(yazSatir, 5, item.makbuzNo);

                string[] parca = (item.odemeEmriNo ?? "").Split('/');
                string odemeBelgeYil = "";
                string odemeBelgeNo = "";
                if (parca.Length > 1)
                {
                    odemeBelgeYil = parca[0];
                    odemeBelgeNo = parca[1];
                }
                parca = (item.faizOdemeEmriNo ?? "").Split('/');
                string faizBelgeNo = "";
                if (parca.Length > 1)
                {
                    faizBelgeNo = parca[1];
                }
                if (faizBelgeNo != "")
                    XLS.HucreDegerYaz(yazSatir, 6, odemeBelgeYil + "/" + faizBelgeNo);
                if (odemeBelgeNo != "")
                    XLS.HucreDegerYaz(yazSatir, 7, odemeBelgeYil + "/" + odemeBelgeNo);

                yazSatir++;
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>Toplu Ödeme Emri / Ödeme Emri Yazdır: oluşturulacak ödeme emrinin satırları ve toplamı. Geçici Excel dosyasının yolunu döndürür.</summary>
        public static string TopluOdemeEmri(OdemeEmriMIFForm odemeEmri, string sablonAd)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("ATM-TopluOdemeEmriListe_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;

            double borc = 0.0;
            double alacak = 0.0;

            foreach (OdemeEmriMIFDetay oed in odemeEmri.detay.objeler)
            {
                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, satir - kaynakSatir);
                XLS.HucreDegerYaz(satir, sutun + 1, oed.hesapKod);
                XLS.HucreDegerYaz(satir, sutun + 2, oed.detayBirim);
                XLS.HucreDegerYaz(satir, sutun + 3, oed.altHesapKod);
                XLS.HucreDegerYaz(satir, sutun + 4, oed.borc);
                XLS.HucreDegerYaz(satir, sutun + 5, oed.alacak);

                borc += oed.borc;
                alacak += oed.alacak;
            }

            satir++;
            XLS.SatirAc(satir, 1);
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);
            XLS.HucreDegerYaz(satir, sutun + 3, "TOPLAM");
            XLS.HucreDegerYaz(satir, sutun + 4, borc);
            XLS.HucreDegerYaz(satir, sutun + 5, alacak);
            XLS.KoyuYap(satir, sutun, satir, sutun + 5, true);

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(430));
            XLS.DosyaSaklaTamYol();
            return XLS.SonucDosyaAd();
        }
    }
}

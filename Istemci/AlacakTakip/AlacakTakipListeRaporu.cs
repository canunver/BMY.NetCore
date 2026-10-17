using OrtakClass;
using TNS;
using TNS.ATM;

namespace Istemci.AlacakTakip
{
    /// <summary>
    /// Takip İşlemleri Liste Yazdır raporları; eski AlacakTakipListe.aspx.cs içindeki ExceleYaz (Kısa Liste, ATM-DosyaListe.xlt) ve
    /// ExceleYaz2 (Uzun Liste, ATM-AlacakTakip.xltx) kodunun aynısıdır. Geçici Excel dosyasının yolunu döndürür.
    /// </summary>
    public static class AlacakTakipListeRaporu
    {
        /// <summary>Kısa Liste (eski ExceleYaz).</summary>
        public static string KisaListe(ObjectArray bilgi, string sablonYol, string kurKod)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("AlacakTakipListe_", "." + GenelIslemler.ExcelTur());
            XLS.DosyaAc(sablonYol, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            satir = kaynakSatir;
            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                Dosya d = (Dosya)bilgi.objeler[i];

                satir++;
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, d.yil + "/" + d.dosyaNo);
                XLS.HucreDegerYaz(satir, sutun + 1, d.saymanlikKodu + "-" + d.saymanlikAdi + " (" + d.birimKodu + "-" + d.birimAdi + ")");
                XLS.HucreDegerYaz(satir, sutun + 2, d.kimlikNo);
                XLS.HucreDegerYaz(satir, sutun + 3, d.borcluAd);
                XLS.HucreDegerYaz(satir, sutun + 4, TNS.ATM.Arac.BorcBul(d, kurKod).ToString("#,###.00") + " " + AlacakTakipListeUclari.ParaBirimiStr(d.paraBirimi));
                XLS.HucreDegerYaz(satir, sutun + 5, d.saymanlikDosyaNo);
                XLS.HucreDegerYaz(satir, sutun + 6, d.sonDurum.ad);
                XLS.HucreDegerYaz(satir, sutun + 7, d.borcDogusTarihi.ToString());
                XLS.HucreDegerYaz(satir, sutun + 8, d.faizBaslangicTarihi.ToString());
                XLS.HucreDegerYaz(satir, sutun + 9, d.borcTespitTarihi.ToString());
                XLS.HucreDegerYaz(satir, sutun + 10, d.tebligTarihi.ToString());
                XLS.HucreDegerYaz(satir, sutun + 11, d.borcDegerlendirmeTarihi.ToString());
            }

            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(430));
            XLS.DosyaSaklaTamYol();
            return XLS.SonucDosyaAd();
        }

        /// <summary>Uzun Liste (eski ExceleYaz2).</summary>
        public static string UzunListe(ObjectArray bilgi, string sablonYol, string kurKod)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int formatSatirBas = 0, formatSutunBas = 0, formatSatirBit = 0, formatSutunBit = 0, sayfa = 0;
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("AlacakTakip_", "." + GenelIslemler.ExcelTur());
            XLS.DosyaAc(sablonYol, sonucDosyaAd);

            XLS.HucreAdAdresCoz("FormatSatir", ref sayfa, ref formatSatirBas, ref formatSutunBas, ref formatSatirBit, ref formatSutunBit);
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);
            int yuks = XLS.SatirYukseklikAl(kaynakSatir);
            satir = kaynakSatir;

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                Dosya d = (Dosya)bilgi.objeler[i];

                XLS.HucreKopyala(formatSatirBas, formatSutunBas, formatSatirBit, formatSutunBit, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, i);
                XLS.HucreDegerYaz(satir, sutun + 1, d.saymanlikKodu);
                XLS.HucreDegerYaz(satir, sutun + 2, d.birimKodu);
                XLS.HucreDegerYaz(satir, sutun + 3, TNS.MUH.Arac.BirimAdOl(d.saymanlikAdi, d.birimAdi));
                XLS.HucreDegerYaz(satir, sutun + 4, d.yil + "/" + d.dosyaNo + "-" + d.saymanlikDosyaNo);
                XLS.HucreDegerYaz(satir, sutun + 5, d.kimlikNo);
                XLS.HucreDegerYaz(satir, sutun + 6, d.borcluAd);

                XLS.HucreDegerYaz(satir, sutun + 7, d.gelenYaziYer + " " + d.gelenYaziTarih.ToString() + "/" + d.gelenYaziSayi);
                XLS.HucreDegerYaz(satir, sutun + 8, d.borcSebep);
                XLS.HucreDegerYaz(satir, sutun + 9, d.saymanligaGidenYaziTarih.ToString() + "/" + d.saymanligaGidenYaziSayi);
                XLS.HucreDegerYaz(satir, sutun + 10, d.kisiyiGidenYaziTarih.ToString() + "/" + d.kisiyeGidenYaziSayi);
                XLS.HucreDegerYaz(satir, sutun + 11, d.makbuzBilgi);
                XLS.HucreDegerYaz(satir, sutun + 12, d.sonDurum.ad);
                XLS.HucreDegerYaz(satir, sutun + 13, TNS.ATM.Arac.BorcBul(d, kurKod));
                XLS.HucreDegerYaz(satir, sutun + 14, d.odenenBorc);
                XLS.HucreDegerYaz(satir, sutun + 15, d.odenenFaiz);
                XLS.HucreDegerYaz(satir, sutun + 16, d.odenenTaksit + "/" + d.toplamTaksit);
                satir++;
            }

            XLS.SatirYukseklikAyarla(kaynakSatir, satir, yuks);
            XLS.SatirYukseklikAyarla(kaynakSatir, satir, -1);
            XLS.DosyaSaklaTamYol();
            return XLS.SonucDosyaAd();
        }
    }
}

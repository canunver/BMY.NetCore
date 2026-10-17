using System;
using System.Collections;
using System.IO;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Makbuz raporları; eski kodun aynısıdır:
    ///   KesilenMakbuzlar : eski Makbuz.aspx btnKestigimMakbuzlar_Click (MUH\MakbuzListesi.xlt)
    ///   HesapBazinda     : eski Makbuz.aspx btnRaporHesap_Click (MUH\MakbuzListesiMizan.xlt)
    ///   MakbuzYazdir     : eski MakbuzYazdir.aspx (MUH\Makbuz.dotx, Aspose.Words ile PDF)
    /// Şablon önce kurumun RaporSablonYol\MUH klasöründe, yoksa uygulamayla gelen SGB.Net\RaporSablon\MUH klasöründe aranır.
    /// </summary>
    public static class MakbuzRaporlari
    {
        public const string ListeSablon = "MakbuzListesi.xlt";
        public const string MizanSablon = "MakbuzListesiMizan.xlt";
        public const string MakbuzSablon = "Makbuz.dotx";

        public static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "MUH", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "MUH", sablonAd);
            return File.Exists(u) ? u : null;
        }

        private static void BaslikYaz(Tablo XLS, DateTime t1, DateTime t2)
        {
            if (t1 == t2)
                XLS.HucreAdBulYaz("Baslik", t1.ToShortDateString() + " Tarihinde Kesilen Makbuzlar");
            else
                XLS.HucreAdBulYaz("Baslik", t1.ToShortDateString() + " - " + t2.ToShortDateString() + " Tarihleri Arası Kesilen Makbuzlar");
        }

        /// <summary>Kesilen Makbuzlar: makbuz başına bir satır, sonda para birimlerine göre toplamlar. Geçici Excel dosyasının yolunu döndürür.</summary>
        public static string KesilenMakbuzlar(ObjectArray mler, string sablonAd, DateTime basTarih, DateTime bitTarih)
        {
            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("Makbuz_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            BaslikYaz(XLS, basTarih.Date, bitTarih.Date);

            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            int satir = kaynakSatir;
            Hashtable toplamHT = new Hashtable();

            foreach (Makbuz m in mler.objeler)
            {
                satir++;

                string paraBirimiKod = m.paraBirimiKod;
                if (string.IsNullOrWhiteSpace(paraBirimiKod)) paraBirimiKod = "TRY";

                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun, m.yevmiyeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, m.islemTarihDT.ToShortDateString());
                XLS.HucreDegerYaz(satir, sutun + 2, m.muhasebe + " - " + m.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 3, m.birim + " - " + m.birimAd);
                XLS.HucreDegerYaz(satir, sutun + 4, m.makbuzTipi.aciklama);
                XLS.HucreDegerYaz(satir, sutun + 5, m.kisiAd);
                XLS.HucreDegerYaz(satir, sutun + 6, m.genelToplam);
                XLS.HucreDegerYaz(satir, sutun + 7, paraBirimiKod);
                XLS.HucreDegerYaz(satir, sutun + 8, m.aciklama);
                XLS.HucreDegerYaz(satir, sutun + 9, m.duzenleyen);
                XLS.HucreDegerYaz(satir, sutun + 10, m.unvan);
                if (m.durum == 9)
                {
                    XLS.HucreDegerYaz(satir, sutun + 6, 0);
                    XLS.HucreDegerYaz(satir, sutun + 11, "MAKBUZ İPTAL");
                }
                else
                {
                    XLS.HucreDegerYaz(satir, sutun + 11, m.odemeEmriNo);
                    toplamHT[paraBirimiKod] = OrtakFonksiyonlar.HTDoubleEkle(toplamHT, paraBirimiKod, m.genelToplam);
                }
            }

            satir++;

            foreach (DictionaryEntry item in toplamHT)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + 5, "TOPLAM (" + (string)item.Key + ")");
                XLS.HucreDegerYaz(satir, sutun + 6, (double)item.Value);
                XLS.ArkaPlanRenk(satir, sutun, satir, sutun + 11, System.Drawing.Color.LightYellow);
                XLS.KoyuYap(satir, sutun, satir, sutun + 11, true);

                satir++;
            }

            // Eklenen satırların yükseklikleri ayarlanıyor
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, GenelIslemler.JexcelBirimtoExcelBirim(400));
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>Hesap Bazında Rapor: makbuzların hesap satırları (hesap kodu, alt hesap, hesap adı, tutar).</summary>
        public static string HesapBazinda(ObjectArray mler, string sablonAd, DateTime basTarih, DateTime bitTarih)
        {
            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("MakbuzMizan_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            BaslikYaz(XLS, basTarih.Date, bitTarih.Date);

            int sutun = 0;
            int kaynakSatir = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref kaynakSatir, ref sutun);

            int satir = kaynakSatir;
            string altHesap = "";
            foreach (Makbuz m in mler.objeler)
            {
                foreach (MakbuzHesap mh in m.hesaplar)
                {
                    altHesap = (mh.altHesap ?? "").Trim();
                    if (altHesap != "")
                        altHesap += "-" + mh.altHesapAd;

                    satir++;

                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 8, satir, sutun);

                    XLS.HucreDegerYaz(satir, sutun, mh.hesapKod);
                    XLS.HucreDegerYaz(satir, sutun + 1, altHesap);
                    XLS.HucreDegerYaz(satir, sutun + 2, mh.hesapAd);
                    XLS.HucreDegerYaz(satir, sutun + 3, mh.tutar);
                }
            }

            // Eklenen satırların yükseklikleri ayarlanıyor
            XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir, -1);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>Makbuz çıktısı (eski MakbuzYazdir.Yazdir): şablondaki alanlar doldurulur, PDF olarak saklanır. PDF dosyasının yolunu döndürür.</summary>
        public static string MakbuzYazdir(Kullanici kullanan, Makbuz makbuz, string sablonAd)
        {
            double kurDegeri = 1;
            if (!string.IsNullOrWhiteSpace(makbuz.paraBirimiKod) && makbuz.paraBirimiKod != "TRY")
            {
                TNS.HRC.IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
                TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();
                kriter.kurTarih = makbuz.islemTarih;
                TNS.HRC.MerkezBankasiKurSinif kurListe = servisHRC.MerkezBankKurOku(kullanan, kriter);
                foreach (TNS.HRC.MerkezBankasiKurDetaySinif kur in kurListe.satirlar)
                {
                    if (kur.paraBirim.kisaltma == makbuz.paraBirimiKod)
                    {
                        kurDegeri = OrtakFonksiyonlar.ConvertToDbl(kur.DegerBul("ALIS", 1));
                        break;
                    }
                }
            }

            Aspose.Words.Document doc = new Aspose.Words.Document(sablonAd);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            makbuz.HesapSatirEkle(10);
            for (int i = 0; i < makbuz.hesaplar.Count && i < 10; i++)
            {
                MakbuzHesap mh = (MakbuzHesap)makbuz.hesaplar[i];

                string yaz1 = "";
                string yaz2 = "";
                if (!string.IsNullOrWhiteSpace(mh.hesapKod) && mh.hesaplananAlan == 0)
                {
                    yaz1 = mh.hesapKod;
                    if (!string.IsNullOrEmpty(mh.altHesap))
                        yaz1 += " / " + mh.altHesap;

                    yaz2 = mh.tutar.ToString("#,##0.00");
                }

                double yaz2Orjinal = OrtakFonksiyonlar.ConvertToDouble(yaz2);
                if (yaz2Orjinal > 0)
                    yaz2 += " " + makbuz.paraBirimiKod;

                if (kurDegeri > 1)
                {
                    if (yaz2Orjinal > 0)
                        yaz2 += " - " + Math.Round(yaz2Orjinal * kurDegeri, 2).ToString("#,###.00") + " TRY";
                }

                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Hesap" + (i + 1).ToString(), yaz1);
                GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Tutar" + (i + 1).ToString(), yaz2);
            }

            string kisi = makbuz.tcKimlikNoVergiNo ?? "";
            if (kisi != "") kisi += " - ";
            kisi += makbuz.kisiAd;

            string TutarRakam = makbuz.genelToplam.ToString("#,###.00");
            string TutarYazi = OrtakFonksiyonlar.YaziylaParasiz(makbuz.genelToplam) + " " + makbuz.paraBirimiKod;
            string HesapKodTutar = (makbuz.makbuzTipiTutar / ((double)makbuz.makbuzTipi.kdv / 100 + 1)).ToString("#,###.00");
            string KdvHesapKodTutar = (makbuz.makbuzTipiTutar - (makbuz.makbuzTipiTutar / ((double)makbuz.makbuzTipi.kdv / 100 + 1))).ToString("#,###.00");
            string KrediHesapKodTutar = makbuz.krediKartiIleOdenen.ToString("#,###.00");
            string KasaHesapKodTutar = (makbuz.genelToplam - makbuz.krediKartiIleOdenen).ToString("#,###.00");

            double HesapKodTutarOrjinal = OrtakFonksiyonlar.ConvertToDouble(HesapKodTutar);
            double KdvHesapKodTutarOrjinal = OrtakFonksiyonlar.ConvertToDouble(KdvHesapKodTutar);
            double KrediHesapKodTutarOrjinal = OrtakFonksiyonlar.ConvertToDouble(KrediHesapKodTutar);
            double KasaHesapKodTutarOrjinal = OrtakFonksiyonlar.ConvertToDouble(KasaHesapKodTutar);

            HesapKodTutar += " " + makbuz.paraBirimiKod;
            KdvHesapKodTutar += " " + makbuz.paraBirimiKod;
            KrediHesapKodTutar += " " + makbuz.paraBirimiKod;
            KasaHesapKodTutar += " " + makbuz.paraBirimiKod;

            if (kurDegeri > 1)
            {
                HesapKodTutar += " - " + Math.Round(HesapKodTutarOrjinal * kurDegeri, 2).ToString("#,###.00") + " TRY";
                KdvHesapKodTutar += " - " + Math.Round(KdvHesapKodTutarOrjinal * kurDegeri, 2).ToString("#,###.00") + " TRY";
                KrediHesapKodTutar += " - " + Math.Round(KrediHesapKodTutarOrjinal * kurDegeri, 2).ToString("#,###.00") + " TRY";
                KasaHesapKodTutar += " - " + Math.Round(KasaHesapKodTutarOrjinal * kurDegeri, 2).ToString("#,###.00") + " TRY";
            }

            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "YevmiyeNo", makbuz.yevmiyeNo);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TutarRakam", TutarRakam);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "ParaBirimi", makbuz.paraBirimiKod);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "TutarYazi", TutarYazi);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Kisi", kisi);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Aciklama", makbuz.aciklama);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Tarih", makbuz.islemTarih.Oku().ToString());
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "Duzenleyen", makbuz.duzenleyen + " - " + makbuz.birimAd);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HesapKod", makbuz.makbuzTipi.hesapKod);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "HesapKodTutar", HesapKodTutar);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KdvHesapKod", makbuz.makbuzTipi.kdvHesapKod);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KdvHesapKodTutar", KdvHesapKodTutar);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KrediHesapKod", makbuz.makbuzTipi.krediHesapKod);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KrediHesapKodTutar", KrediHesapKodTutar);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KasaHesapKod", makbuz.makbuzTipi.kasaHesapKod);
            GenelIslemlerIstemci.AsposeAlanaYaz(builder, "KasaHesapKodTutar", KasaHesapKodTutar);

            string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".pdf";
            doc.Save(sonucDosyaAd, Aspose.Words.SaveFormat.Pdf);
            return sonucDosyaAd;
        }
    }
}

using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.KYM;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Bütçe Çalışmalarında Görevli Personel Listesi Excel raporu; eski ButceCalismasiPersoneliForm.aspx btnYazdir_Click'in aynısıdır
    /// (şablon BHMv2\ButceCalismasiPersoneliForm.xltx), yalnızca dosyayı göndermek yerine üretilen dosyanın yolunu döndürür.
    /// Liste okunamazsa null döner, hata metni hata'ya yazılır.
    /// </summary>
    public static class ButceCalismasiPersoneliRaporu
    {
        /// <summary>Eski GenelIslemlerIstemci.VarsayilanKurumBulAd.</summary>
        private static string KurumAd()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD");
        }

        public static string Yazdir(IBHMv2Servis servis, Kullanici kullanan, int yil, string pebkod, string raporSablonYol, out string hata)
        {
            hata = "";
            TNS.UZY.IUZYServis servisUzy = TNS.UZY.Arac.Tanimla();
            ButceCalismasiPersoneli form = new ButceCalismasiPersoneli();
            form.yil = yil;
            form.pebkod = pebkod;

            ObjectArray oa = servis.ListeleButceCalismasiPersoneli(kullanan, form);
            if (!oa.sonuc.islemSonuc)
            {
                hata = oa.sonuc.hataStr;
                return null;
            }

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            XLS.DosyaAc(raporSablonYol + "/BHMv2/ButceCalismasiPersoneliForm.xltx", sonucDosyaAd);

            XLS.HucreAdBulYaz("Yil", form.yil);
            if (string.IsNullOrWhiteSpace(form.pebkod))
                XLS.HucreAdBulYaz("BirimAd", KurumAd());
            else
                XLS.HucreAdBulYaz("BirimAd", servisUzy.UzayDegeriStr(kullanan, "ABSPEBKOD", form.yil + "-" + form.pebkod, true));

            int baslaSatir = 0;
            int baslaSutun = 0;

            int sheetNo = 0, formatSatirBas = 0, formatSutunBas = 0, formatSatirBit = 0, formatSutunBit = 0;

            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref formatSatirBas, ref formatSutunBas, ref formatSatirBit, ref formatSutunBit);

            int yuks = XLS.SatirYukseklikAl(baslaSatir);
            int satir = baslaSatir;
            foreach (ButceCalismasiPersoneli item in oa.objeler)
            {
                XLS.SatirAc(satir, 1);
                XLS.HucreKopyala(formatSatirBas, formatSutunBas, formatSatirBit, formatSutunBit, satir, baslaSutun);
                XLS.HucreDegerYaz(satir, baslaSutun + 0, item.ad);
                XLS.HucreDegerYaz(satir, baslaSutun + 1, item.soyad);
                XLS.HucreDegerYaz(satir, baslaSutun + 2, item.unvan);
                XLS.HucreDegerYaz(satir, baslaSutun + 3, item.birim);
                XLS.HucreDegerYaz(satir, baslaSutun + 4, item.pebkod);
                XLS.HucreDegerYaz(satir, baslaSutun + 5, item.pebad);
                XLS.HucreDegerYaz(satir, baslaSutun + 6, item.isTel);
                XLS.HucreDegerYaz(satir, baslaSutun + 7, item.cepTel);
                XLS.HucreDegerYaz(satir, baslaSutun + 8, item.faks);
                XLS.HucreDegerYaz(satir, baslaSutun + 9, item.ePosta);
                satir++;
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir, yuks);
            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }
}

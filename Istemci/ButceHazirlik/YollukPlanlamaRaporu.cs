using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.KYM;
using TNS.YAT;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Yolluk Planlama Formu raporu; eski kodun aynısıdır (eski YatirimRaporlari.YollukPlani, Rapor.xml 2003-14, ek bilgi "YP";
    /// boş faaliyet için AltProjeYollukPlan.PebKodlariBul), yalnızca rapor parametreleri doğrudan verilir ve üretilen dosyanın
    /// yolu döndürülür. Şablon: RaporSablonYol\YAT\YATYollukPlanlama.xltx.
    /// </summary>
    public static class YollukPlanlamaRaporu
    {
        /// <summary>Eski AltProjeYollukPlan.PebKodlariBul: projenin / alt projenin bağlı faaliyet kodları (virgülle).</summary>
        public static string PebKodlariBul(Kullanici kullanan, IBHMv2Servis servisBHMv2, int yil, string senaryoKod, int projeKodu, int altProjeKodu)
        {
            AltFaaliyet[] pebKodlar;
            if (projeKodu > 0 && altProjeKodu == 0)
                pebKodlar = servisBHMv2.YatBagPebKodlariListele(kullanan, yil, projeKodu, senaryoKod);
            else
            {
                AltFaaliyet p = new AltFaaliyet();
                p.yil = yil;
                p.altProje = altProjeKodu;
                pebKodlar = servisBHMv2.YatBagPebKodlariListele(kullanan, p, senaryoKod);
            }
            string kodlar = "";
            foreach (AltFaaliyet peb in pebKodlar ?? new AltFaaliyet[0])
            {
                if (!string.IsNullOrEmpty(kodlar))
                    kodlar += ",";
                kodlar += peb.pebKod;
            }
            return kodlar;
        }

        /// <summary>
        /// Eski YatirimRaporlari.YollukPlani. Sayfadan proje / alt proje gelmediğinden (eski "-") ikisi de -1'dir;
        /// faaliyet kodu boşsa eskisi gibi PebKodlariBul ile bulunur.
        /// </summary>
        public static string YollukPlani(Kullanici kullanan, int yil, string senaryoKod, string pebKod, string raporSablonYol)
        {
            int projeNo = -1;
            int altProjeNo = -1;
            IYATServis servisYAT = TNS.YAT.Arac.Tanimla();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("YollukPlanlama_", "") + "." + GenelIslemler.ExcelTur();

            Tablo XLS = GenelIslemler.NewTablo();
            int formatSatir = 0;
            int formatSutun = 0;
            int baslaSatir = 0;
            int baslaSutun = 0;
            XLS.DosyaAc(raporSablonYol + "/YAT/YATYollukPlanlama.xltx", sonucDosyaAd);

            XLS.HucreAdAdresCoz("FormatSatir", ref formatSatir, ref formatSutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("Yil", yil);
            int yuks = XLS.SatirYukseklikAl(baslaSatir);
            IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            if (string.IsNullOrWhiteSpace(pebKod))
            {
                pebKod = PebKodlariBul(kullanan, servisBHMv2, yil, senaryoKod, projeNo, altProjeNo);
            }
            ObjectArray oa = null;
            if (string.IsNullOrWhiteSpace(pebKod))
                oa = new ObjectArray();
            else
                oa = servisYAT.ListeleYollukPlanlama(kullanan, yil, senaryoKod, null, pebKod);
            if (!oa.sonuc.islemSonuc)
                throw new RaporHatasi(oa.sonuc.hataStr);
            int satir = baslaSatir;
            double top = 0;
            foreach (YollukPlan yp in oa.objeler)
            {
                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, formatSutun + 12, satir, baslaSutun);
                XLS.HucreDegerYaz(satir, baslaSutun + 0, satir - baslaSatir + 1);
                string yaz = yp.yurutenBirim;
                if (string.IsNullOrWhiteSpace(yaz)) yaz = yp.faaliyetBirimAd;
                XLS.HucreDegerYaz(satir, baslaSutun + 1, yaz);

                XLS.HucreDegerYaz(satir, baslaSutun + 2, yp.dairesi);

                yaz = yp.altProjeAdi;
                if (string.IsNullOrWhiteSpace(yaz)) yaz = yp.faaliyetAd;
                XLS.HucreDegerYaz(satir, baslaSutun + 3, yaz);
                XLS.HucreDegerYaz(satir, baslaSutun + 4, yp.konusu);
                XLS.HucreDegerYaz(satir, baslaSutun + 5, yp.suresi);
                XLS.HucreDegerYaz(satir, baslaSutun + 6, yp.kisiSayisi);
                XLS.HucreDegerYaz(satir, baslaSutun + 8, yp.yapilacagiAy);
                XLS.HucreDegerYaz(satir, baslaSutun + 9, yp.yeri);
                XLS.HucreDegerYaz(satir, baslaSutun + 10, yp.tertip);
                XLS.HucreDegerYaz(satir, baslaSutun + 11, yp.mALIYET);
                top += yp.mALIYET;
                satir++;
            }
            XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir + 1, formatSutun + 12, satir, baslaSutun);
            XLS.HucreDegerYaz(satir, baslaSutun + 11, top);
            XLS.SatirYukseklikAyarla(baslaSatir, satir, yuks);
            XLS.SatirYukseklikAyarla(baslaSatir, satir, -1);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }
}

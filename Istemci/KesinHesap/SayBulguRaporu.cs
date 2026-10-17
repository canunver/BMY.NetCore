using System;
using System.IO;
using Istemci;
using OrtakClass;
using TNS;
using TNS.KHM;
using TNS.KYM;

namespace Istemci.KesinHesap
{
    /// <summary>
    /// Sayıştay bulguları Excel listesi; eski SayistayBulgu/SayBulgu.aspx.cs içindeki RPRSayBulgular sınıfının aynısıdır.
    /// Değişenler: birim kodu ayrı parametredir, kurum adı GenelIslemlerIstemci'den, şablon önce kurumun RaporSablonYol\KHM
    /// klasöründe, yoksa uygulamayla gelen RaporSablon\KHM klasöründe aranır.
    /// </summary>
    public class SayBulguRaporu
    {
        public const string SablonAd = "SAYBulguListe.xltx";

        Tablo XLS;
        int fSatir = 0, fSutun = 0;
        int bSatir = 0, bSutun = 0;
        int satir, sutun;
        readonly bool cevapli;
        ObjectArray<SayBulgu> oaCevaplar;

        /// <param name="cevapli">Cevaplı Liste Yazdır: her bulgunun altına birimlerin cevapları yazılır.</param>
        public SayBulguRaporu(bool cevapli)
        {
            this.cevapli = cevapli;
        }

        public static string SablonBul(string uygulamaKlasoru)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "KHM", SablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "KHM", SablonAd);
            return File.Exists(u) ? u : null;
        }

        /// <param name="birim">Verilirse (Cevap sayfası) birimin cevapları listelenir; boşsa bulgular.</param>
        /// <returns>Üretilen Excel dosyasının yolu.</returns>
        public string Yazdir(Kullanici kullanan, SayBulgu sayBulgu, string birim, string sablonAd)
        {
            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("FormatSatir", ref fSatir, ref fSutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref bSatir, ref bSutun);

            XLS.HucreAdBulYaz("Yil", sayBulgu.yil);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreAdBulYaz("RaporTarih", new TNSDateTime(DateTime.Now.Date).ToString());

            IKHMServis servis = TNS.KHM.Arac.Tanimla();
            ObjectArray<SayBulgu> oa;

            int tur;
            if (!string.IsNullOrWhiteSpace(birim))
            {
                tur = 2;
                oa = servis.OnBulguCevapListele(kullanan, sayBulgu.yil, birim);
                oaCevaplar = null;
            }
            else
            {
                tur = 1;
                oa = servis.ListeleOnBulgu(kullanan, sayBulgu);
                if (oa.sonuc.islemSonuc && cevapli)
                {
                    oaCevaplar = servis.OnBulguCevapListele(kullanan, sayBulgu.yil, null);
                    if (!oaCevaplar.sonuc.islemSonuc)
                    {
                        oa.sonuc = oaCevaplar.sonuc;
                    }
                }
                else
                    oaCevaplar = null;
            }
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
            {
                satir = bSatir;
                int sayac = 1;
                int oncBulguTur = 0;
                foreach (SayBulgu item in oa.objeler)
                {
                    if ((int)item.bulguTurKod != oncBulguTur)
                    {
                        XLS.HucreKopyala(fSatir + 0, fSutun, fSatir + 0, fSutun + 10, satir, bSutun);
                        XLS.HucreDegerYaz(satir, bSutun + 1, SayBulgu.BulguTurAdBul(item.bulguTurKod).ToUpper());
                        sayac = 1;
                        oncBulguTur = (int)item.bulguTurKod;
                        XLS.SatirYukseklikAyarla(satir, satir, -1, 11, 23);
                        satir++;
                    }
                    XLS.HucreKopyala(fSatir + 1, fSutun, fSatir + 1, fSutun + 10, satir, bSutun);
                    sutun = bSutun;
                    XLS.HucreDegerYaz(satir, sutun++, sayac++);
                    XLS.HucreDegerYaz(satir, sutun++, item.bulguNo + ": " + item.bulguBasligi);
                    bool yerineGetirildi;
                    TNSDateTime ongorulenTamamlanmaTarihi;
                    string yapilacakFaaliyetler, idareCevabi;
                    if (tur == 1)
                    {
                        yerineGetirildi = item.yerineGetirildi;
                        ongorulenTamamlanmaTarihi = item.ongorulenTamamlanmaTarihi;
                        yapilacakFaaliyetler = item.yapilacakFaaliyetler;
                        idareCevabi = item.idareCevabi;
                    }
                    else
                    {
                        yerineGetirildi = item.yerineGetirildiCevap;
                        ongorulenTamamlanmaTarihi = item.ongorulenTamamlanmaTarihiCevap;
                        yapilacakFaaliyetler = item.birimIsler;
                        idareCevabi = item.birimCevap;
                    }

                    XLS.HucreDegerYaz(satir, sutun++, yerineGetirildi ? "Yerine Getirildi" : "Yerine Getirilmedi");
                    XLS.HucreDegerYaz(satir, sutun++, idareCevabi);
                    XLS.HucreDegerYaz(satir, sutun++, yapilacakFaaliyetler);

                    XLS.HucreDegerYaz(satir, sutun++, item.ilgiliBirimKurum);
                    XLS.HucreDegerYaz(satir, sutun++, item.gorevOrtagi);

                    if (!TNSDateTime.IsNull(ongorulenTamamlanmaTarihi))
                        XLS.HucreDegerYaz(satir, sutun++, ongorulenTamamlanmaTarihi.Oku());
                    else
                        sutun++;
                    satir++;
                    if (cevapli)
                    {
                        CevaplariYaz(item.bulguID);
                    }
                }
            }
            else
            {
                if (oa.sonuc.islemSonuc)
                    oa.sonuc.DegerAta("Aranılan özelliklere uyan kayıt bulunamadı!");
                XLS.HucreDegerYaz(bSatir, bSutun, oa.sonuc.hataStr);
            }

            XLS.FormulleriSil();
            XLS.SatirSil(0, 6);
            XLS.DosyaSaklaTamYol();
            return XLS.SonucDosyaAd();
        }

        private void CevaplariYaz(int bulguID)
        {
            if (oaCevaplar != null)
            {
                foreach (SayBulgu item in oaCevaplar.objeler)
                {
                    if (item.bulguID == bulguID)
                    {
                        XLS.HucreKopyala(fSatir + 2, fSutun, fSatir + 2, fSutun + 10, satir, bSutun);
                        XLS.HucreDegerYaz(satir, bSutun + 1, item.muhasebeBirimAd);
                        XLS.HucreDegerYaz(satir, bSutun + 3, item.birimCevap);
                        XLS.HucreDegerYaz(satir, bSutun + 4, item.birimIsler);
                        satir++;
                    }
                }
            }
        }
    }
}

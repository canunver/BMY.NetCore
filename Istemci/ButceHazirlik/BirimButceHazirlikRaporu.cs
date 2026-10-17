using System;
using System.IO;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    /// <summary>
    /// Birim Bütçe Teklifi Excel raporları; eski ButceHazirlikv2/BirimButceHazirlikForm.aspx.cs içindeki YazdirTek, YazdirGrup ve
    /// YazdirOzet kodunun aynısıdır. Yalnızca dosyayı göndermek yerine üretilen dosyanın yolunu döndürür (hata olursa null, hata alanında
    /// metin). Şablonlar önce kurumun RaporSablonYol\BHMv2 klasöründe, yoksa uygulamayla gelen SGB.Net\RaporSablon\BHMv2 klasöründe aranır:
    ///   YazdirTek:  Yazdır (BirimButceHazirlik.xltx)
    ///   YazdirGrup: Rapor Yazdır (BirimButceHazirlikGrup.xltx; faaliyet birimine göre gruplu, il raporunda birimler ayrı)
    ///   YazdirOzet: Özet Yazdır (BirimButceHazirlikGrup.xltx; eko 2 düzeyinde, birimlere göre)
    /// </summary>
    public class BirimButceHazirlikRaporu
    {
        private const string Asama = "00.00";

        private readonly Kullanici kullanan;
        private readonly string uygulamaKlasoru;
        private readonly int yil;
        private readonly int saymanlik;
        private readonly string birim;
        private readonly IBHMv2Servis servis;

        public string hata;

        public BirimButceHazirlikRaporu(Kullanici kullanan, string uygulamaKlasoru, int yil, int saymanlik, string birim)
        {
            this.kullanan = kullanan;
            this.uygulamaKlasoru = uygulamaKlasoru;
            this.yil = yil;
            this.saymanlik = saymanlik;
            this.birim = birim ?? "";
            servis = TNS.BHMv2.Arac.Tanimla();
        }

        private string SablonBul(string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "BHMv2", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "BHMv2", sablonAd);
            if (File.Exists(u)) return u;
            hata = "Rapor şablonu bulunamadı (BHMv2\\" + sablonAd + ").";
            return null;
        }

        private ObjectArray Listele(bool birimGrupla, bool eko2)
        {
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, GenelIslemlerIstemci.VarsayilanKurumBul(), "", "", "", "");
            ObjectArray oa = servis.ListeleBirimButceHazirlik(kullanan, t, saymanlik, birim, Asama, birimGrupla, eko2);
            if (oa == null || !oa.sonuc.islemSonuc)
            {
                hata = oa?.sonuc.hataStr;
                if (string.IsNullOrWhiteSpace(hata) || hata.Contains("Exception")) hata = "Liste okunamadı.";
                return null;
            }
            return oa;
        }

        private string MuhasebeAdGetir()
        {
            return TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "MUHMUHASEBE", saymanlik.ToString(), true) ?? "";
        }

        private string BirimAdGetir()
        {
            if (string.IsNullOrWhiteSpace(birim) || saymanlik <= 0) return "";
            return TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "MUHMUHASEBEBIRIM", birim.Trim() + "-" + saymanlik, true) ?? "";
        }

        private void BaslikYaz(Tablo XLS)
        {
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("Saymanlik", saymanlik);
            XLS.HucreAdBulYaz("Birim", birim);
            XLS.HucreAdBulYaz("SaymanlikAd", MuhasebeAdGetir());
            XLS.HucreAdBulYaz("BirimAd", BirimAdGetir());
        }

        /// <summary>Yazdır (eski YazdirTek).</summary>
        public string YazdirTek()
        {
            string sablon = SablonBul("BirimButceHazirlik.xltx");
            if (sablon == null) return null;
            ObjectArray oa = Listele(false, false);
            if (oa == null) return null;

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaYol = DosyaIslem.DosyaAdUret("OdenekTalep_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablon, sonucDosyaYol);
            BaslikYaz(XLS);
            int fsheetNo = 0, fsatir1 = 0, fsutun1 = 0, fsatir2 = 0, fsutun2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref fsheetNo, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);
            int satir = 5;
            int sutun = 2;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int satirYukseklik = XLS.SatirYukseklikAl(satir - 2);
            for (int i = 0; i < oa.ObjeSayisi; i++)
            {
                BirimButceHazirlikKarsilanan bbhk = (BirimButceHazirlikKarsilanan)oa[i];
                bool detayMi = bbhk.detayMi;
                int ek = detayMi ? 1 : 0;
                XLS.HucreKopyala(fsatir1 + ek, fsutun1, fsatir2 + ek, fsutun2, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, bbhk.tertip);
                XLS.HucreDegerYaz(satir, sutun + 1, bbhk.pebad);
                XLS.HucreDegerYaz(satir, sutun + 2, bbhk.ekoad);
                XLS.HucreDegerYaz(satir, sutun + 3, bbhk.harcama);
                XLS.HucreDegerYaz(satir, sutun + 4, bbhk.planlanan);
                XLS.HucreDegerYaz(satir, sutun + 5, bbhk.talepEdilen);
                if (!detayMi)
                    XLS.HucreDegerYaz(satir, sutun + 6, bbhk.talepEdilen - bbhk.planlanan);
                XLS.SatirYukseklikAyarla(satir, satir, satirYukseklik);
                satir++;
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaYol;
        }

        /// <summary>Rapor Yazdır (eski YazdirGrup).</summary>
        public string YazdirGrup()
        {
            string sablon = SablonBul("BirimButceHazirlikGrup.xltx");
            if (sablon == null) return null;
            ObjectArray oa = Listele(true, false);
            if (oa == null) return null;

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaYol = DosyaIslem.DosyaAdUret("BirimButceTeklif_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablon, sonucDosyaYol);
            BaslikYaz(XLS);
            int fsheetNo = 0, fsatir1 = 0, fsutun1 = 0, fsatir2 = 0, fsutun2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref fsheetNo, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);
            int satir = 13;
            int sutun = 2;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int satirYukseklik = XLS.SatirYukseklikAl(satir - 1);
            string eksiFaalBirim = "";
            int eskiSaymanlik = -1;
            string eskiBirim = "";
            int ekSatir;
            bool ilRapor = (birim == "260");
            int eskiFaalSatir = -1;
            int eskiBirimSatir = -1;
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            TNS.HRC.IHRCServis hrcServis = TNS.HRC.Arac.Tanimla();

            ToplamAl toplamAl = new ToplamAl(4, 3);
            for (int i = 0; i < oa.ObjeSayisi; i++)
            {
                BirimButceHazirlikKarsilanan bbhk = (BirimButceHazirlikKarsilanan)oa[i];
                string tertip = bbhk.tertip;
                double harcama = bbhk.harcama;
                double planlanan = bbhk.planlanan;
                double talepEdilen = bbhk.talepEdilen;
                bool detayMi = bbhk.detayMi;
                if (!detayMi)
                {
                    string faalBirim = tertip.Substring(29, 2);
                    if (eksiFaalBirim != faalBirim)
                    {
                        if (eksiFaalBirim != "")
                        {
                            ToplamYaz(XLS, eskiFaalSatir, sutun + 5, toplamAl, 2);
                            satir++;
                            XLS.SayfaSonuKoy(satir);
                        }
                        toplamAl.SatirSifirla(2);
                        XLS.HucreKopyala(fsatir1 + 5, fsutun1, fsatir1 + 8, fsutun2, satir, sutun);
                        XLS.SatirYukseklikAyarla(satir + 0, satir + 0, 40000);
                        XLS.SatirYukseklikAyarla(satir + 1, satir + 1, 31500);
                        XLS.SatirYukseklikAyarla(satir + 2, satir + 2, 31500);
                        XLS.SatirYukseklikAyarla(satir + 3, satir + 3, 31500);

                        XLS.HucreDegerYaz(satir, sutun + 2, uzyServis.UzayDegeriStr(kullanan, "ABSPEBKOD", yil.ToString() + "-" + faalBirim, true));
                        string ilBirimAd = IlAdBirimAdBul(hrcServis, uzyServis, saymanlik, birim, true);
                        XLS.HucreDegerYaz(satir + 1, sutun, String.Format("{0}\r\n{1} YILI BÜTÇE TEKLİFİ", ilBirimAd, yil));
                        XLS.HucreDegerYaz(satir + 3, sutun, String.Format("{0} TOPLAM TEKLİF", ilBirimAd));
                        eskiFaalSatir = satir + 3;
                        eksiFaalBirim = faalBirim;
                        satir += 4;
                        eskiSaymanlik = -1;
                        eskiBirim = "";
                    }
                    if (ilRapor)
                    {
                        if (eskiSaymanlik != bbhk.saymanlik || eskiBirim != bbhk.birim)
                        {
                            if (eskiBirimSatir > -1)
                                ToplamYaz(XLS, eskiBirimSatir, sutun + 5, toplamAl, 1);
                            ekSatir = bbhk.birim == "250" ? 1 : 0;
                            toplamAl.SatirSifirla(1);
                            eskiSaymanlik = bbhk.saymanlik;
                            eskiBirim = bbhk.birim;
                            XLS.HucreKopyala(fsatir1 + ekSatir, fsutun1, fsatir1 + ekSatir, fsutun2, satir, sutun);
                            XLS.SatirYukseklikAyarla(satir, satir, 18000);
                            XLS.HucreDegerYaz(satir, sutun + 2, BirimAdBul(uzyServis, bbhk.saymanlik, bbhk.birim));
                            eskiBirimSatir = satir;
                            satir++;
                        }
                    }
                    toplamAl.DegerAta(0, 0, harcama);
                    toplamAl.DegerAta(0, 1, planlanan);
                    toplamAl.DegerAta(0, 2, talepEdilen);

                    toplamAl.ToplamEkle(1, 0);
                    toplamAl.ToplamEkle(2, 0);
                    toplamAl.ToplamEkle(3, 0);
                }

                ekSatir = detayMi ? 3 : 2;
                XLS.HucreKopyala(fsatir1 + ekSatir, fsutun1, fsatir1 + ekSatir, fsutun2, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + 0, bbhk.saymanlik);
                XLS.HucreDegerYaz(satir, sutun + 1, bbhk.birim);
                XLS.HucreDegerYaz(satir, sutun + 2, tertip);
                XLS.HucreDegerYaz(satir, sutun + 3, bbhk.ekoad);
                XLS.HucreDegerYaz(satir, sutun + 4, bbhk.pebad);

                XLS.HucreDegerYaz(satir, sutun + 5, harcama);
                XLS.HucreDegerYaz(satir, sutun + 6, planlanan);
                XLS.HucreDegerYaz(satir, sutun + 7, talepEdilen);
                XLS.SatirYukseklikAyarla(satir, satir, satirYukseklik);

                satir++;
            }
            if (eskiFaalSatir > -1)
                ToplamYaz(XLS, eskiFaalSatir, sutun + 5, toplamAl, 2);
            if (eskiBirimSatir > -1)
                ToplamYaz(XLS, eskiBirimSatir, sutun + 5, toplamAl, 1);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaYol;
        }

        /// <summary>Özet Yazdır (eski YazdirOzet).</summary>
        public string YazdirOzet()
        {
            string sablon = SablonBul("BirimButceHazirlikGrup.xltx");
            if (sablon == null) return null;
            ObjectArray oa = Listele(true, true);
            if (oa == null) return null;

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaYol = DosyaIslem.DosyaAdUret("BirimButceTeklifOzet_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablon, sonucDosyaYol);
            BaslikYaz(XLS);
            int fsheetNo = 0, fsatir1 = 0, fsutun1 = 0, fsatir2 = 0, fsutun2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref fsheetNo, ref fsatir1, ref fsutun1, ref fsatir2, ref fsutun2);

            int satir = 13;
            int sutun = 2;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            int satirYukseklik = XLS.SatirYukseklikAl(satir - 1);
            int eskiToplamSatir;
            int eskiSaymanlik;
            string eskiBirim;
            int ekSatir;
            int eskiBirimSatir = -1;
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            TNS.HRC.IHRCServis hrcServis = TNS.HRC.Arac.Tanimla();

            ToplamAl toplamAl = new ToplamAl(4, 3);

            toplamAl.SatirSifirla(2);
            XLS.HucreKopyala(fsatir1 + 6, fsutun1, fsatir1 + 8, fsutun2, satir, sutun);
            XLS.SatirYukseklikAyarla(satir + 0, satir + 0, 31500);
            XLS.SatirYukseklikAyarla(satir + 1, satir + 1, 31500);
            XLS.SatirYukseklikAyarla(satir + 2, satir + 2, 31500);

            string ilBirimAd = IlAdBirimAdBul(hrcServis, uzyServis, saymanlik, birim, true);
            XLS.HucreDegerYaz(satir + 0, sutun, String.Format("{0}\r\n{1} YILI BÜTÇE TEKLİFİ", ilBirimAd, yil));
            XLS.HucreDegerYaz(satir + 2, sutun, String.Format("{0} TOPLAM TEKLİF", ilBirimAd));
            eskiToplamSatir = satir + 2;
            satir += 3;
            eskiSaymanlik = -1;
            eskiBirim = "";

            for (int i = 0; i < oa.ObjeSayisi; i++)
            {
                BirimButceHazirlikKarsilanan bbhk = (BirimButceHazirlikKarsilanan)oa[i];
                double harcama = bbhk.harcama;
                double planlanan = bbhk.planlanan;
                double talepEdilen = bbhk.talepEdilen;

                if (eskiSaymanlik != bbhk.saymanlik || eskiBirim != bbhk.birim)
                {
                    if (eskiBirimSatir > -1)
                        ToplamYaz(XLS, eskiBirimSatir, sutun + 5, toplamAl, 1);
                    ekSatir = bbhk.birim == "250" ? 1 : 0;
                    toplamAl.SatirSifirla(1);
                    eskiSaymanlik = bbhk.saymanlik;
                    eskiBirim = bbhk.birim;
                    XLS.HucreKopyala(fsatir1 + ekSatir, fsutun1, fsatir1 + ekSatir, fsutun2, satir, sutun);
                    XLS.SatirYukseklikAyarla(satir, satir, 18000);
                    XLS.HucreDegerYaz(satir, sutun + 2, BirimAdBul(uzyServis, bbhk.saymanlik, bbhk.birim));
                    eskiBirimSatir = satir;
                    satir++;
                }
                toplamAl.DegerAta(0, 0, harcama);
                toplamAl.DegerAta(0, 1, planlanan);
                toplamAl.DegerAta(0, 2, talepEdilen);

                toplamAl.ToplamEkle(1, 0);
                toplamAl.ToplamEkle(2, 0);

                ekSatir = -1;
                XLS.HucreKopyala(fsatir1 + ekSatir, fsutun1, fsatir1 + ekSatir, fsutun2, satir, sutun);

                XLS.HucreDegerYaz(satir, sutun + 0, bbhk.saymanlik);
                XLS.HucreDegerYaz(satir, sutun + 1, bbhk.birim);
                XLS.HucreDegerYaz(satir, sutun + 2, bbhk.tertip);
                XLS.HucreDegerYaz(satir, sutun + 3, bbhk.ekoad);

                XLS.HucreDegerYaz(satir, sutun + 5, harcama);
                XLS.HucreDegerYaz(satir, sutun + 6, planlanan);
                XLS.HucreDegerYaz(satir, sutun + 7, talepEdilen);
                XLS.SatirYukseklikAyarla(satir, satir, satirYukseklik);

                satir++;
            }
            XLS.SutunGenislikAyarla(sutun + 3, sutun + 3, XLS.SutunGenislikAl(sutun + 3) + XLS.SutunGenislikAl(sutun + 4));
            XLS.SutunGizle(sutun + 4, sutun + 4, true);
            if (eskiToplamSatir > -1)
                ToplamYaz(XLS, eskiToplamSatir, sutun + 5, toplamAl, 2);
            if (eskiBirimSatir > -1)
                ToplamYaz(XLS, eskiBirimSatir, sutun + 5, toplamAl, 1);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaYol;
        }

        private string BirimAdBul(TNS.UZY.IUZYServis uzy, int say, string bir)
        {
            string saymanliAd = uzy.UzayDegeriStr(kullanan, "MUHMUHASEBE", say.ToString(), true);
            string birimAd = uzy.UzayDegeriStr(kullanan, "MUHMUHASEBEBIRIM", bir + "-" + say.ToString(), true);
            return TNS.BHMv2.Arac.BirimAdOl(saymanliAd, birimAd);
        }

        private string IlAdBirimAdBul(TNS.HRC.IHRCServis hrc, TNS.UZY.IUZYServis uzy, int say, string bir, bool kodVar)
        {
            string ad = "";
            string kod;
            if (bir == "260") // il adı döndürülecek
            {
                int il = say / 1000;
                kod = il.ToString("00");
                TNS.HRC.Il ilBilgi = hrc.IlListele(kullanan, kod);
                if (ilBilgi != null)
                    ad = ilBilgi.ad.ToUpper() + " İLİ";
            }
            else
            {
                ad = BirimAdBul(uzy, say, bir);
                kod = say.ToString() + "-" + bir;
            }
            if (kodVar)
                ad = kod + "-" + ad;
            return ad;
        }

        private static void ToplamYaz(Tablo XLS, int eskiFaalSatir, int sutun, ToplamAl toplamAl, int toplamSatir)
        {
            XLS.HucreDegerYaz(eskiFaalSatir, sutun, toplamAl.toplam[toplamSatir, 0]);
            XLS.HucreDegerYaz(eskiFaalSatir, sutun + 1, toplamAl.toplam[toplamSatir, 1]);
            XLS.HucreDegerYaz(eskiFaalSatir, sutun + 2, toplamAl.toplam[toplamSatir, 2]);
        }
    }
}

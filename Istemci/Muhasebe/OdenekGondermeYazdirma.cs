using System;
using System.Collections;
using Istemci;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>İcmal tertip bilgilerini tutan nesne (eski ButceMuhasebe.TertipIcmal).</summary>
    public class TertipIcmal
    {
        public string tertip = "";
        public double miktar = 0;
    }

    /// <summary>
    /// Ödenek Gönderme / Tenkis belge ve icmal çıktıları (eski ButceMuhasebe/OdenekGondermeTenkisYazdirma.cs, aynı şablon ve yerleşimle).
    /// Eski koddan farkı: kullanıcı oturumdan değil parametreden gelir, dosya burada gönderilmez; üretilen dosyanın yolu döner
    /// (uç DosyaIslem.DosyaGonder ile gönderir). Hata olursa hata metni döner, dosya null kalır.
    /// </summary>
    public static class OdenekGondermeYazdirma
    {
        public static string SablonYol(string sablonAd)
        {
            return Ayar.Oku("RaporSablonYol") + "/MUH/" + sablonAd;
        }

        /// <summary>Tek belge (eski Listele: GondermeTenkisTekBelgeListele).</summary>
        public static ObjectArray Listele(Kullanici kullanan, IMUHServis servisMUH, int yil, ENUMBelgeTuru belgeTur, string belgeNo, int tasra = 0)
        {
            GondermeTenkisForm f = new GondermeTenkisForm();
            f.yil = yil;
            f.belgeTur = belgeTur;
            f.belgeNo = belgeNo;
            f.tasra = tasra;

            return servisMUH.GondermeTenkisTekBelgeListele(kullanan, f);
        }

        /// <summary>Belge çıktısı (eski Yazdir). excelYazYer boşsa geçici dosyaya yazılır.</summary>
        public static string Yazdir(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string belgeNo, string excelYazYer, out string dosya, out string gidenAd)
        {
            dosya = null;
            gidenAd = belgeNo;
            string hataStr = "";
            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            ObjectArray fler = Listele(kullanan, servisMUH, yil, belgeTur, belgeNo);

            if (fler.sonuc.islemSonuc)
            {
                if (fler.objeler.Count == 0)
                    hataStr = string.Format("Görmek istediğiniz {0} nolu belge bulunamadı", belgeNo);
                else
                {
                    GondermeTenkisForm form = (GondermeTenkisForm)fler.objeler[0];
                    if (form.belgeNo != "")
                    {
                        dosya = ExceleYaz(kullanan, servisMUH, form, excelYazYer);
                        gidenAd = form.belgeNo;
                    }
                    else
                        hataStr = "İstenilen belge bulunamadığı için gösterilemiyor.";
                }
            }
            else
                hataStr = fler.sonuc.hataStr;

            return hataStr;
        }

        private static string ExceleYaz(Kullanici kullanan, IMUHServis servisMUH, GondermeTenkisForm form, string excelYazYer)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;

            int satirD = 0, sutunD = 0;
            int satirG = 0, sutunG = 0;
            int satirS = 0, sutunS = 0;

            string sablonAd = "";
            if (form.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
            {
                if (form.formDurum == (int)ENUMBelgeDurumu.TALEP || form.formDurum == (int)ENUMBelgeDurumu.YENITALEP)
                    sablonAd = "MUH-OdenekGondermeTalep.xlt";
                else
                    sablonAd = "MUH-OdenekGondermeBelge.xlt";
            }
            else if (form.belgeTur == ENUMBelgeTuru.TENKIS)
                sablonAd = "MUH-TenkisBelge.xlt";

            string sonucDosyaAd = string.IsNullOrEmpty(excelYazYer)
                ? DosyaIslem.DosyaAdUret("OdenekGonderme_", "") + "." + GenelIslemler.ExcelTur()
                : excelYazYer;

            XLS.DosyaAc(SablonYol(sablonAd), sonucDosyaAd);

            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtGenelToplam", ref satirG, ref sutunG);
            XLS.HucreAdAdresCoz("txtSayfaToplam", ref satirS, ref sutunS);

            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            XLS.HucreAdBulYaz("txtBelgeNo", form.belgeNo);
            XLS.HucreAdBulYaz("txtYil", form.yil);
            XLS.HucreAdBulYaz("txtMuhasebe", form.muhasebe);
            XLS.HucreAdBulYaz("txtMuhasebeAd", form.muhasebeAd);
            XLS.HucreAdBulYaz("txtKurum", form.kurum + " - " + form.kurumAd);
            XLS.HucreAdBulYaz("txtBirim", form.birim + " - " + form.birimAd);
            XLS.HucreAdBulYaz("txtBelgeTarih", form.islemTarih.ToString());
            XLS.HucreAdBulYaz("txtAciklamaEkler", form.aciklama);

            double sayfaToplam = 0;
            double devredenToplam = 0;
            double genelToplam = 0;
            int satirAc = 0;
            int ilkSayfaSatirSayisi = 35;
            int sonrakiSayfaSatirSayisi = 60;

            if (form.detay.objeler.Count <= ilkSayfaSatirSayisi)
                satirAc = ilkSayfaSatirSayisi;
            if (form.detay.objeler.Count > ilkSayfaSatirSayisi)
            {
                satirAc = form.detay.objeler.Count - ilkSayfaSatirSayisi;
                double karar = OrtakFonksiyonlar.ConvertToDbl(satirAc) / OrtakFonksiyonlar.ConvertToDbl(sonrakiSayfaSatirSayisi);

                if (Math.Round(karar) > karar)
                    karar = Math.Round(karar);
                else
                    karar = Math.Round(karar) + 1;
                satirAc = OrtakFonksiyonlar.ConvertToInt(karar, 0) * sonrakiSayfaSatirSayisi + ilkSayfaSatirSayisi;
                if (satirAc < form.detay.objeler.Count + (karar * 2))
                    satirAc += sonrakiSayfaSatirSayisi;
            }

            int sayfaSayisi = 0;
            for (int i = 0; i < satirAc; i++)
            {
                if (i == ilkSayfaSatirSayisi || (i - ilkSayfaSatirSayisi) == sonrakiSayfaSatirSayisi * sayfaSayisi)
                {
                    XLS.HucreKopyala(satirS, sutunS, satirS, sutunS + 20, satir + i, sutun);
                    XLS.HucreBirlestir(satir + i, sutun, satir + i, sutun + 10);
                    XLS.HucreBirlestir(satir + i, sutun + 11, satir + i, sutun + 13);

                    i++;
                    XLS.SayfaSonuKoy(satir + i);

                    XLS.HucreKopyala(satirS, sutunS, satirS, sutunS + 20, satir + i, sutun);
                    XLS.HucreBirlestir(satir + i, sutun, satir + i, sutun + 10);
                    XLS.HucreDegerYaz(satir + i, sutun, "D  E  V  R  E  D  E  N     T  O  P  L  A  M");

                    sayfaSayisi++;
                }
                else
                    XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir + i, sutun);

                XLS.HucreBirlestir(satir + i, sutun + 11, satir + i, sutun + 13);
            }
            XLS.HucreKopyala(satirG, sutunG, satirG, sutunG + 20, satir + satirAc, sutun);
            XLS.SatirYukseklikAyarla(satir, satir + satirAc, GenelIslemler.JexcelBirimtoExcelBirim(300));
            XLS.HucreBirlestir(satir + satirAc, sutun, satir + satirAc, sutun + 10);
            XLS.HucreBirlestir(satir + satirAc, sutun + 11, satir + satirAc, sutun + 13);

            int genelToplamSatir = satir + satirAc;

            int satirSayac = 0;
            sayfaSayisi = 0;
            foreach (GondermeTenkisDetay detay in form.detay.objeler)
            {
                int ekleSutun = 0;

                if (satirSayac == ilkSayfaSatirSayisi ||
                    (satirSayac - ilkSayfaSatirSayisi) == sonrakiSayfaSatirSayisi * sayfaSayisi)
                {
                    XLS.HucreDegerYaz(satir, sutun + 11, sayfaToplam);
                    satir++;
                    XLS.HucreDegerYaz(satir, sutun + 11, sayfaToplam);
                    satir++;

                    satirSayac += 2;
                    sayfaSayisi++;
                }

                for (int j = 0; j < detay.tertip.kodlar.Length; j++)
                {
                    XLS.HucreDegerYaz(satir, sutun + ekleSutun, detay.tertip.kodlar[j]);
                    if (j == 0 || j == 1) ekleSutun += 3;
                    else if (j == 2) ekleSutun += 1;
                    else ekleSutun += 2;
                }

                XLS.HucreDegerYaz(satir, sutun + 11, detay.miktar);

                satir++;
                satirSayac++;

                sayfaToplam += detay.miktar;
                devredenToplam += detay.miktar;
                genelToplam += detay.miktar;
            }

            XLS.HucreDegerYaz(genelToplamSatir, sutun + 11, genelToplam);
            XLS.HucreAdBulYaz("txtToplamMiktarYaziyla", OrtakFonksiyonlar.Yaziyla(genelToplam));

            ImzaBilgileriniYaz(servisMUH, XLS, kullanan, form.belgeTur, 0, form.birim, form.islemTarih);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>İcmal belgesi (eski IcmalBelgesiYazdir; iki sayfa: belgeler ve tertip bazında icmal).</summary>
        public static string IcmalBelgesiYazdir(Kullanici kullanan, int yil, ENUMBelgeTuru belgeTur, string kurum, int icmalNo, out string dosya)
        {
            dosya = null;
            int[] kolUzunluk = { 4, 4, 1, 2 };
            string hataStr = "";
            ObjectArray tertipIcmal = new ObjectArray();

            if (yil == 0)
                hataStr = "İcmal belgesini yazdırmak için Yıl alanını boş bırakamazsınız.";
            if ((kurum ?? "").Trim() == "")
                hataStr += "İcmal belgesini yazdırmak için Kurum alanını boş bırakamazsınız.";
            if (icmalNo == 0)
                hataStr += "İcmal belgesini yazdırmak için İcmal No alanını boş bırakamazsınız.";
            if (belgeTur == ENUMBelgeTuru.TANIMSIZ)
                hataStr += "Belge Türü bilgisi alınamadı.";

            if (hataStr != "")
                return hataStr;

            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            GondermeTenkisForm f = new GondermeTenkisForm();
            f.belgeTur = belgeTur;
            f.yil = yil;
            f.kurum = kurum;
            f.icmalNo = icmalNo;

            ObjectArray fler = servisMUH.GondermeTenkisListele(kullanan, f, 1);

            if (fler.sonuc.islemSonuc)
            {
                if (fler.objeler.Count == 0)
                    return string.Format("Görmek istediğiniz {0} nolu belge bulunamadı", f.belgeNo);
            }
            else
                return fler.sonuc.hataStr;

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;

            int satirB = 0, sutunB = 0;
            int satirD = 0, sutunD = 0;
            int satirG = 0, sutunG = 0;
            int satirS = 0, sutunS = 0;

            string sablonAd = "";
            if (belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                sablonAd = "MUH-OdenekGondermeIcmal.XLT";
            else if (belgeTur == ENUMBelgeTuru.TENKIS)
                sablonAd = "MUH-TenkisIcmal.XLT";

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("Icmal_", "") + "." + GenelIslemler.ExcelTur();

            XLS.DosyaAc(SablonYol(sablonAd), sonucDosyaAd);

            XLS.HucreAdAdresCoz("txtBelgeToplam", ref satirB, ref sutunB);
            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtGenelToplam", ref satirG, ref sutunG);
            XLS.HucreAdAdresCoz("txtSayfaToplam", ref satirS, ref sutunS);

            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            bool ilkGiris = true;
            int belgeToplamiSatir = 0;
            double belgeToplami = 0;
            double sayfaToplam = 0;
            double devredenToplam = 0;
            double genelToplam = 0;
            int sayfaSatir = 0;
            bool sayfaToplamiYaz = false;
            string imzaBirim = "";

            string belgeYil = "";
            string belgeKurum = "";
            string belgeIcmalNo = "";
            string belgeIcmalTarih = "";

            foreach (GondermeTenkisForm form in fler.objeler)
            {
                if (ilkGiris)
                {
                    belgeYil = form.yil.ToString();
                    belgeKurum = form.kurum + " - " + form.kurumAd;
                    belgeIcmalNo = form.icmalNo.ToString();
                    belgeIcmalTarih = form.icmalTarih.ToString();
                    XLS.HucreAdBulYaz("txtYil", belgeYil);
                    XLS.HucreAdBulYaz("txtKurum", belgeKurum);
                    XLS.HucreAdBulYaz("txtTarih", belgeIcmalTarih);
                    XLS.HucreAdBulYaz("txtIcmalNo", belgeIcmalNo);
                    ilkGiris = false;
                }

                XLS.HucreKopyala(satirB, sutunB, satirB, sutunB + 20, satir, sutun);
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
                XLS.HucreBirlestir(satir, sutun + 4, satir, sutun + 14);
                XLS.HucreBirlestir(satir, sutun + 15, satir, sutun + 16);

                XLS.HucreDegerYaz(satir, sutun, form.belgeNo);
                XLS.HucreDegerYaz(satir, sutun + 1, form.muhasebe);
                XLS.HucreDegerYaz(satir, sutun + 2, form.muhasebeAd);
                XLS.HucreDegerYaz(satir, sutun + 3, form.birim);
                imzaBirim = form.birim;
                belgeToplamiSatir = satir;

                belgeToplami = 0;
                foreach (GondermeTenkisDetay detay in form.detay.objeler)
                {
                    detay.tertip.Tertip23Ayarla(3);
                    satir++;
                    sayfaSatir++;

                    if ((!sayfaToplamiYaz && sayfaSatir >= 44) || (sayfaToplamiYaz && sayfaSatir >= 61))
                    {
                        SayfaToplamiYaz(XLS, ref satir, sutun, satirS, sutunS, sayfaToplam, devredenToplam);
                        sayfaToplamiYaz = true;
                        sayfaToplam = 0;
                        sayfaSatir = 2;
                    }
                    XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir, sutun);
                    XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
                    XLS.HucreBirlestir(satir, sutun, satir, sutun + 3);
                    XLS.HucreBirlestir(satir, sutun + 15, satir, sutun + 16);
                    int ekleSutun = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        if ((string)detay.tertip.kodlar[j] != null)
                        {
                            string[] t = ((string)detay.tertip.kodlar[j]).Split('.');

                            for (int i = 0; i < Math.Max(t.Length, kolUzunluk[j]); i++)
                            {
                                XLS.HucreDegerYaz(satir, sutun + 4 + ekleSutun, OrtakFonksiyonlar.DiziElemanAl(t, i, ""));
                                ekleSutun++;
                            }
                        }
                    }

                    XLS.HucreDegerYaz(satir, sutun + 15, detay.miktar);
                    belgeToplami += detay.miktar;
                    sayfaToplam += detay.miktar;
                    devredenToplam += detay.miktar;
                    genelToplam += detay.miktar;

                    bool bulundu = false;
                    foreach (TertipIcmal ti in tertipIcmal.objeler)
                    {
                        if (ti.tertip == detay.tertip.TertipStrOl())
                        {
                            bulundu = true;
                            ti.miktar += detay.miktar;
                        }
                    }

                    if (!bulundu)
                    {
                        TertipIcmal tIcmal = new TertipIcmal();
                        tIcmal.tertip = detay.tertip.TertipStrOl();
                        tIcmal.miktar = detay.miktar;
                        tertipIcmal.objeler.Add(tIcmal);
                    }
                }

                XLS.HucreDegerYaz(belgeToplamiSatir, sutun + 15, belgeToplami);
                satir++;
                sayfaSatir++;

                if ((!sayfaToplamiYaz && sayfaSatir + 1 >= 44) || (sayfaToplamiYaz && sayfaSatir + 1 >= 61))
                {
                    SayfaToplamiYaz(XLS, ref satir, sutun, satirS, sutunS, sayfaToplam, devredenToplam);
                    sayfaToplamiYaz = true;
                    sayfaToplam = 0;
                    sayfaSatir = 2;
                }
            }

            if (sayfaToplamiYaz)
            {
                XLS.HucreKopyala(satirS, sutunS, satirS, sutunS + 20, satir, sutun);
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
                XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);
                XLS.HucreBirlestir(satir, sutun + 15, satir, sutun + 16);
                XLS.HucreDegerYaz(satir, sutun + 15, sayfaToplam);

                satir++;
            }

            XLS.HucreKopyala(satirG, sutunG, satirG, sutunG + 20, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);
            XLS.HucreBirlestir(satir, sutun + 15, satir, sutun + 16);

            XLS.HucreDegerYaz(satir, sutun + 15, genelToplam);
            XLS.HucreAdBulYaz("txtGenelToplamYaziyla", OrtakFonksiyonlar.Yaziyla(genelToplam));

            string belgeImzaUstLimit = TNS.MUH.Arac.DegiskenDegerBul(yil, "/OEBELGEIMZAUSTLIMIT");
            bool yuksek = false;

            if (!string.IsNullOrEmpty(belgeImzaUstLimit))
                yuksek = (belgeToplami > OrtakFonksiyonlar.ConvertToDbl(belgeImzaUstLimit));

            ImzaBilgileriniYaz(servisMUH, XLS, kullanan, belgeTur, 0, imzaBirim, new TNSDateTime(belgeIcmalTarih), yuksek);

            // Tertip bazında icmal (ikinci sayfa)
            XLS.AktifSheetDegistir(1);
            XLS.HucreAdBulYaz("txtYilT", belgeYil);
            XLS.HucreAdBulYaz("txtKurumT", belgeKurum);
            XLS.HucreAdBulYaz("txtTarihT", belgeIcmalTarih);
            XLS.HucreAdBulYaz("txtIcmalNoT", belgeIcmalNo);

            XLS.HucreAdAdresCoz("txtBelgeToplamT", ref satirB, ref sutunB);
            XLS.HucreAdAdresCoz("txtDetaySatirT", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtGenelToplamT", ref satirG, ref sutunG);
            XLS.HucreAdAdresCoz("txtSayfaToplamT", ref satirS, ref sutunS);

            XLS.HucreAdAdresCoz("txtBaslaSatirT", ref satir, ref sutun);

            foreach (TertipIcmal ti in tertipIcmal.objeler)
            {
                XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir, sutun);
                XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
                XLS.HucreBirlestir(satir, sutun + 11, satir, sutun + 12);

                string[] ttt = ti.tertip.Split('-');

                int ekleSutun = 0;
                for (int j = 0; j < 4; j++)
                {
                    string[] t = OrtakFonksiyonlar.DiziElemanAl(ttt, j, "").Split('.');

                    for (int i = 0; i < Math.Max(t.Length, kolUzunluk[j]); i++)
                    {
                        XLS.HucreDegerYaz(satir, sutun + ekleSutun, OrtakFonksiyonlar.DiziElemanAl(t, i, ""));
                        ekleSutun++;
                    }
                }

                XLS.HucreDegerYaz(satir, sutun + 11, ti.miktar);
                satir++;
            }

            XLS.HucreKopyala(satirG, sutunG, satirG, sutunG + 20, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 10);
            XLS.HucreBirlestir(satir, sutun + 11, satir, sutun + 12);

            XLS.HucreDegerYaz(satir, sutun + 11, genelToplam);

            XLS.DosyaSaklaTamYol();
            dosya = sonucDosyaAd;
            return "";
        }

        /// <summary>İcmalde sayfa sonu: sayfa toplamı, sayfa sonu ve devreden toplam satırları (eski koddaki iki aynı blok).</summary>
        private static void SayfaToplamiYaz(Tablo XLS, ref int satir, int sutun, int satirS, int sutunS, double sayfaToplam, double devredenToplam)
        {
            XLS.HucreKopyala(satirS, sutunS, satirS, sutunS + 20, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);
            XLS.HucreBirlestir(satir, sutun + 15, satir, sutun + 16);
            XLS.HucreDegerYaz(satir, sutun + 15, sayfaToplam);

            satir++;
            XLS.SayfaSonuKoy(satir);

            XLS.HucreKopyala(satirS, sutunS, satirS, sutunS + 20, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir, GenelIslemler.JexcelBirimtoExcelBirim(330));
            XLS.HucreBirlestir(satir, sutun, satir, sutun + 14);
            XLS.HucreBirlestir(satir, sutun + 15, satir, sutun + 16);
            XLS.HucreDegerYaz(satir, sutun, " D E V R E D E N   T O P L A M");
            XLS.HucreDegerYaz(satir, sutun + 15, devredenToplam);
            satir++;
        }

        /// <summary>
        /// İmza bilgileri (eski ButceMuhasebeGenel.ImzaBilgileriniYaz): birime, sonra genele, sonra "stratek" kullanıcısının
        /// tanımlarına bakılır; yüksek tutarlı belgede önce belgeTur + 10 imzaları denenir.
        /// </summary>
        public static void ImzaBilgileriniYaz(IMUHServis servis, Tablo XLS, Kullanici kullanan, ENUMBelgeTuru belgeTur, int muhasebe, string birim, TNSDateTime belgeTarihi, bool yuksek = false)
        {
            if (yuksek) belgeTur = (ENUMBelgeTuru)((int)belgeTur + 10);
            ObjectArray iV = null;
            while (true)
            {
                iV = servis.ImzaListele(kullanan, belgeTur, muhasebe, birim);
                if (!KayitVarMi(iV) && birim != "") iV = servis.ImzaListele(kullanan, belgeTur, 0, "");
                if (!KayitVarMi(iV)) { string tut = kullanan.kullaniciKodu; kullanan.kullaniciKodu = "stratek"; iV = servis.ImzaListele(kullanan, belgeTur, 0, birim); kullanan.kullaniciKodu = tut; }
                if (!KayitVarMi(iV) && birim != "") { string tut = kullanan.kullaniciKodu; kullanan.kullaniciKodu = "stratek"; iV = servis.ImzaListele(kullanan, belgeTur, 0, ""); kullanan.kullaniciKodu = tut; }
                if (KayitVarMi(iV)) break;
                if ((int)belgeTur >= 10) belgeTur = (ENUMBelgeTuru)((int)belgeTur - 10);
                else break;
            }

            int sayac = 1;
            if (iV.sonuc.islemSonuc)
            {
                foreach (ImzaBilgisi imza in iV.objeler)
                {
                    sayac = OrtakFonksiyonlar.ConvertToInt(imza.imzaYer, 0);
                    if (imza.kullanilmiyor == 0)
                    {
                        XLS.HucreAdBulYaz("txtImzaAd" + sayac, imza.adSoyad);
                        XLS.HucreAdBulYaz("txtImzaUnvan" + sayac, imza.unvan);
                        XLS.HucreAdBulYaz("txtImzaGorev" + sayac, imza.gorevUnvan);
                        if (imza.tarih > 0)
                            XLS.HucreAdBulYaz("txtImzaTarih" + sayac, belgeTarihi.ToString());
                    }
                    else
                    {
                        XLS.HucreAdBulYaz("txtImzaAd" + sayac, "");
                        XLS.HucreAdBulYaz("txtImzaUnvan" + sayac, "");
                        XLS.HucreAdBulYaz("txtImzaGorev" + sayac, "");
                        XLS.HucreAdBulYaz("txtImzaTarih" + sayac, "");
                        XLS.HucreAdBulYaz("txtImzaYer" + sayac, "");
                        XLS.HucreAdBulYaz("txtImzaAciklama" + sayac, "");
                    }
                }
            }
        }

        private static bool KayitVarMi(ObjectArray iV)
        {
            return iV != null && iV.sonuc.islemSonuc && iV.ObjeSayisi > 0;
        }
    }

    /// <summary>
    /// Sorgu listesinin Excel raporları (eski OdenekGondermeRaporlar: Başlıklar = 1, Detaylı (Eko 2) = 2, Detaylı (Faal) = 3).
    /// </summary>
    public class OdenekGondermeRaporlar
    {
        private string sablonAd;
        private string sonucDosyaAd;
        private GondermeTenkisForm gtf;
        private Kullanici kullanan;
        private IMUHServis servisMUH;

        public static Hashtable RaporParamOl(GondermeTenkisForm f)
        {
            Hashtable ht = new Hashtable();

            ht["yil"] = f.yil.ToString();
            ht["durum"] = f.formDurum.ToString();

            ht["belgeTur"] = ((int)f.belgeTur).ToString();
            ht["kurum"] = f.kurum;
            ht["muhasebe"] = f.muhasebe.ToString();
            ht["birim"] = f.birim;
            ht["icmalNo"] = f.icmalNo.ToString();
            ht["belgeNo"] = f.belgeNo;

            ht["icmalTarihi"] = f.sorguIcmalTarih.ToString();
            ht["islemYapan"] = f.islemYapan;
            ht["sorguIslemTarih"] = f.sorguIslemTarih;
            ht["sorguFormDurumTarih"] = f.sorguFormDurumTarih;
            ht["tutarbe"] = f.tutarBE.ToString();
            ht["tutarke"] = f.tutarKE.ToString();
            ht["surecno"] = f.surecNo;
            ht["dortDuzeyDegisim"] = f.DortDuzeyDegisim.ToString();

            // taşra: bütün belgeleri listeleme için (talep sayfaları)
            ht["tasra"] = f.tasra.ToString();

            if (f.detay.ObjeSayisi == 1)
            {
                GondermeTenkisDetay detay = (GondermeTenkisDetay)f.detay[0];
                ht["kur"] = detay.tertip.kodlar[0];
                ht["fon"] = detay.tertip.kodlar[1];
                ht["fin"] = detay.tertip.kodlar[2];
                ht["eko"] = detay.tertip.kodlar[3];
                ht["peb"] = detay.tertip.kodlar[4];
            }
            return ht;
        }

        public static GondermeTenkisForm GondermeTenkisOl(Hashtable ht)
        {
            GondermeTenkisForm f = new GondermeTenkisForm();

            f.yil = OrtakFonksiyonlar.ConvertToInt(ht["yil"], 0);
            f.formDurum = OrtakFonksiyonlar.ConvertToInt(ht["durum"], 0);

            f.belgeTur = (ENUMBelgeTuru)OrtakFonksiyonlar.ConvertToInt(ht["belgeTur"], 0);
            f.kurum = (string)ht["kurum"];
            f.muhasebe = OrtakFonksiyonlar.ConvertToInt(ht["muhasebe"], 0);
            f.birim = (string)ht["birim"];
            f.icmalNo = OrtakFonksiyonlar.ConvertToInt(ht["icmalNo"], 0);
            f.belgeNo = (string)ht["belgeNo"];

            f.sorguIcmalTarih = (string)ht["icmalTarihi"];
            f.islemYapan = (string)ht["islemYapan"];
            f.sorguIslemTarih = (string)ht["sorguIslemTarih"];
            f.sorguFormDurumTarih = (string)ht["sorguFormDurumTarih"];
            f.tutarBE = OrtakFonksiyonlar.ConvertToDbl(ht["tutarbe"]);
            f.tutarKE = OrtakFonksiyonlar.ConvertToDbl(ht["tutarke"]);
            f.surecNo = (string)ht["surecno"];
            f.DortDuzeyDegisim = OrtakFonksiyonlar.ConvertToInt(ht["dortDuzeyDegisim"], 0);

            f.tasra = OrtakFonksiyonlar.ConvertToInt(ht["tasra"], 0);

            GondermeTenkisDetay detay = new GondermeTenkisDetay(f.yil);
            detay.tertip.kodlar[0] = (string)ht["kur"];
            detay.tertip.kodlar[1] = (string)ht["fon"];
            detay.tertip.kodlar[2] = (string)ht["fin"];
            detay.tertip.kodlar[3] = (string)ht["eko"];
            detay.tertip.kodlar[4] = (string)ht["peb"];
            f.detay.Ekle(detay);
            return f;
        }

        public string RaporAl(Kullanici kullanan, string sablonAd, Hashtable raporParam, string ekBilgi)
        {
            this.kullanan = kullanan;
            this.sablonAd = sablonAd;
            this.servisMUH = TNS.MUH.Arac.Tanimla();

            this.sonucDosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            gtf = GondermeTenkisOl(raporParam);
            if (ekBilgi == "1" || ekBilgi == "2" || ekBilgi == "3")
            {
                int detayli = 0;
                if (ekBilgi == "2") detayli = 1;
                else if (ekBilgi == "3") detayli = 2;
                return BaslikRaporla(detayli);
            }
            throw new Exception("Tanımsız rapor geldi!");
        }

        private string BaslikRaporla(int detayli)
        {
            ObjectArray fler = servisMUH.GondermeTenkisListele(kullanan, gtf, detayli);
            if (!fler.sonuc.islemSonuc)
                throw new Exception(fler.sonuc.hataStr);
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;

            int satirD = 0;
            int sutunD = 0;
            string baslik = "";

            if (gtf.belgeTur == ENUMBelgeTuru.ODENEKGONDERME)
                baslik = "ÖDENEK GÖNDERME BELGELERİ";
            else if (gtf.belgeTur == ENUMBelgeTuru.TENKIS)
                baslik = "TENKİS BELGELERİ";
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int shNo = 0;
            int fSat1 = 0;
            int fSut1 = 0;
            int fSat2 = 0;
            int fSut2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref shNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            int satYuk = XLS.SatirYukseklikAl(satirD);
            XLS.HucreAdBulYaz("txtBaslik", baslik);
            string yil = gtf.yil.ToString();
            string kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            XLS.HucreAdBulYaz("txtYil", yil);
            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);

            int i = 0;
            satir = satirD;
            foreach (GondermeTenkisForm ff in fler.objeler)
            {
                i++;
                XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, 0);

                XLS.HucreDegerYaz(satir, 1, i.ToString());
                XLS.HucreDegerYaz(satir, 2, ff.belgeNo);
                XLS.HucreDegerYaz(satir, 3, ff.islemTarih.ToString());
                XLS.HucreDegerYaz(satir, 4, ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd);
                XLS.HucreDegerYaz(satir, 5, ff.birim + " - " + ff.birimAd);
                XLS.HucreDegerYaz(satir, 6, ff.belgeToplami);
                XLS.HucreDegerYaz(satir, 7, ff.icmalNo);
                XLS.HucreDegerYaz(satir, 8, ff.icmalTarih.ToString());
                XLS.HucreDegerYaz(satir, 9, ff.onayNo);
                XLS.HucreDegerYaz(satir, 10, ff.onayTarih.ToString());
                XLS.HucreDegerYaz(satir, 11, TNS.MUH.Arac.FormDurumAciklama(ff.formDurum, ff.belgeTur));
                XLS.HucreDegerYaz(satir, 12, ff.formDurumTarih.ToString());
                XLS.HucreDegerYaz(satir, 13, ff.islemYapan);
                if (detayli > 0)
                {
                    bool ilk = true;
                    foreach (GondermeTenkisDetay item in ff.detay.objeler)
                    {
                        if (!ilk)
                            XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, 0);
                        else
                            ilk = false;
                        XLS.HucreDegerYaz(satir, 14, ff.aciklama);
                        XLS.HucreDegerYaz(satir, 15, item.tertip.TertipStrOl());
                        XLS.HucreDegerYaz(satir, 16, item.miktar);

                        XLS.HucreDegerYaz(satir, 20, ff.belgeNo);
                        XLS.HucreDegerYaz(satir, 21, ff.muhasebe);
                        XLS.HucreDegerYaz(satir, 22, ff.birim);
                        XLS.HucreDegerYaz(satir, 23, ff.icmalNo);
                        XLS.HucreDegerYaz(satir, 24, ff.formDurum);
                        XLS.HucreDegerYaz(satir, 25, item.tertip.kodlar[0]);
                        XLS.HucreDegerYaz(satir, 26, item.tertip.kodlar[1]);
                        XLS.HucreDegerYaz(satir, 27, item.tertip.kodlar[2]);
                        XLS.HucreDegerYaz(satir, 28, item.tertip.kodlar[3]);
                        XLS.HucreDegerYaz(satir, 29, item.tertip.kodlar[4]);
                        XLS.HucreDegerYaz(satir, 30, item.miktar);
                        satir++;
                    }
                }
                else
                    satir++;
            }
            XLS.SatirYukseklikAyarla(satirD, satir, -1, 7, satYuk);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }
}

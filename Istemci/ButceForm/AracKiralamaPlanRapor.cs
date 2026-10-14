using System;
using System.Collections;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.KYM;

namespace ButceForm
{
    /// <summary>
    /// Kiralık Taşıt Planlama Formu raporları; eski ButceForm/AracKiralamaPlaniForm.aspx.cs içindeki AracKiralamaPlanRapor
    /// sınıfının aynısıdır (Yazdır, Kurum Yazdır, Detay Yazdır tuşları; Excel şablonları BHF/AracKiralamaBilgiFormu.xltx ve
    /// BHF/AracKiralamaListe.xltx). Değişenler: dosya Response'a yazılmaz, üretilen dosyanın yolu döndürülür (uç indirir);
    /// kurum adı TNS.DEG'den (/KURUMAD) okunur; Onay İşlemleri penceresinin sorgusu (BirimSorgu) da buradadır.
    /// </summary>
    public class AracKiralamaPlanRapor
    {
        private Kullanici kullanan;
        private string sablonYol;
        private AracKiralamaPlanAna form;
        TNS.BHF.IBHFServis bhfServis;

        public AracKiralamaPlanRapor(TNS.BHF.IBHFServis bhfServis, Kullanici kullanan, string sablonYol, AracKiralamaPlanAna form)
        {
            this.kullanan = kullanan;
            this.sablonYol = sablonYol;
            this.form = form;
            this.bhfServis = bhfServis;
        }

        /// <summary>Yazdır: seçili muhasebe/birimin formu. Üretilen dosyanın yolunu döndürür.</summary>
        internal string YazdirBirim(string birimAd)
        {
            form.siralaYontem = 1;
            ObjectVAN ov = bhfServis.OkuAracKiralamaPlani(kullanan, form);
            return YazdirOkunan(ov.sonuc, (AracKiralamaPlanAna)ov.o, birimAd);
        }

        private string YazdirOkunan(Sonuc sonuc, AracKiralamaPlanAna okunan, string birimAd)
        {
            string sonucDosya = DosyaIslem.DosyaAdUret("AracKiralamaBilgiFormu_", $"_{form.yil}_{birimAd}") + "." + GenelIslemler.ExcelTur();
            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonYol + "/BHF/AracKiralamaBilgiFormu.xltx", sonucDosya);
            int fSat = 0, fSut = 0, baslaSatir = 0, sutun = 0, baslikSatir = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref fSat, ref fSut);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            XLS.HucreAdAdresCoz("BaslikSatir", ref baslikSatir, ref sutun);
            XLS.HucreAdBulYaz("Yil", form.yil);
            XLS.HucreAdBulYaz("BirimAd", birimAd);

            BaslikBilgiYaz(XLS, baslikSatir, "GENEL BÜTÇE");
            double baslik1Yuk = XLS.SatirGercekYukseklikAl(baslikSatir);
            double baslik2Yuk = XLS.SatirGercekYukseklikAl(baslikSatir + 1);
            int satir = baslaSatir;

            if (!sonuc.islemSonuc)
            {
                XLS.HucreDegerYaz(satir, 0, sonuc.hataStr);
            }
            else if (okunan == null || okunan.aracKiralamaPlanDetay == null || okunan.aracKiralamaPlanDetay.Count == 0)
            {
                XLS.HucreDegerYaz(satir, 0, "Yazılacak veri bulunamadı");
            }
            else
            {
                ToplamAl toplamAl = new ToplamAl(3, 5);
                toplamAl.HepsiniSifirla();
                int oncButceKaynagi = -1;
                TasitSozluk[] tasitSozlukler = bhfServis.TasitSozlukListele(form.yil) ?? new TasitSozluk[0];
                foreach (AracKiralamaPlanDetay veriSatir in okunan.aracKiralamaPlanDetay)
                {
                    if (oncButceKaynagi != veriSatir.butceKaynagi)
                    {
                        if (oncButceKaynagi != -1)
                        {
                            ToplamYaz(XLS, fSat + 1, satir, toplamAl, 1);
                            satir += 2;
                            XLS.HucreKopyala(baslikSatir, 0, baslikSatir + 1, 13, satir, 0);
                            BaslikBilgiYaz(XLS, satir, "DÖNER SERMAYE");
                            XLS.SatirGercekYukseklikAyarla(satir, satir, baslik1Yuk);
                            XLS.SatirGercekYukseklikAyarla(satir + 1, satir + 1, baslik2Yuk);
                            satir += 2;
                        }
                        oncButceKaynagi = veriSatir.butceKaynagi;
                    }
                    int sut = 0;
                    XLS.HucreKopyala(fSat, 0, fSat, 13, satir, 0);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.tasitKodu);
                    XLS.HucreDegerYaz(satir, sut++, TasitKodAdBul(tasitSozlukler, veriSatir.tasitKodu));
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.oncYilKiralanan);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.oncYilKiralananSure);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.oncYilFiyat);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.oncYilFiyat * veriSatir.oncYilKiralananSure * veriSatir.oncYilKiralanan);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.buYilKiralanan);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.buYilKiralananSure);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.buYilFiyat);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.buYilFiyat * veriSatir.buYilKiralananSure * veriSatir.buYilKiralanan);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.gelecekYilKiralanacak);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.gelecekYilKiralanacakSure);
                    XLS.HucreDegerYaz(satir, sut++, veriSatir.gelecekYilKiralanacakSure * veriSatir.gelecekYilKiralanacak);
                    satir++;
                    toplamAl.DegerAta(2, 0, veriSatir.oncYilKiralanan * veriSatir.oncYilKiralananSure);
                    toplamAl.DegerAta(2, 1, veriSatir.oncYilFiyat * veriSatir.oncYilKiralananSure * veriSatir.oncYilKiralanan);
                    toplamAl.DegerAta(2, 2, veriSatir.buYilKiralanan * veriSatir.buYilKiralananSure);
                    toplamAl.DegerAta(2, 3, veriSatir.buYilFiyat * veriSatir.buYilKiralananSure * veriSatir.buYilKiralanan);
                    toplamAl.DegerAta(2, 4, veriSatir.gelecekYilKiralanacakSure * veriSatir.gelecekYilKiralanacak);
                    toplamAl.ToplamEkle(1, 2);
                    toplamAl.ToplamEkle(0, 2);
                }
                if (oncButceKaynagi != -1)
                {
                    ToplamYaz(XLS, fSat + 1, satir, toplamAl, 1);
                    if (oncButceKaynagi != 1)
                    {
                        satir += 2;
                        ToplamYaz(XLS, fSat + 1, satir, toplamAl, 0);
                    }
                }
            }
            XLS.FormulleriSil();
            XLS.SatirSil(0, 3);
            XLS.DosyaSaklaTamYol();
            return sonucDosya;
        }

        private string TasitKodAdBul(TasitSozluk[] tasitSozlukler, string tasitKodu)
        {
            foreach (TasitSozluk item in tasitSozlukler)
            {
                if (item.siraNo == tasitKodu)
                    return item.Aciklama;
            }
            return tasitKodu;
        }

        private void BaslikBilgiYaz(Tablo XLS, int baslikSatir, string butceAd)
        {
            XLS.HucreDegerYaz(baslikSatir, 2, $"{form.yil - 1} YILINDA {butceAd} KAPSAMINDA KİRALANAN");
            XLS.HucreDegerYaz(baslikSatir, 6, $"{form.yil} YILINDA {butceAd} KAPSAMINDA KİRALANAN (YILLIK)");
            XLS.HucreDegerYaz(baslikSatir, 10, $"{form.yil + 1} YILINDA {butceAd} KAPSAMINDA KİRALANACAK");
        }

        private void ToplamYaz(Tablo XLS, int formatSat, int satir, ToplamAl toplamAl, int toplamSatir)
        {
            XLS.HucreKopyala(formatSat, 0, formatSat, 13, satir, 0);

            if (toplamSatir == 0)
                XLS.HucreDegerYaz(satir, 0, "G E N E L    T O P L A M");
            int sut = 2;
            XLS.HucreDegerYaz(satir, sut++, toplamAl.toplam[toplamSatir, 0] / 12);
            XLS.HucreDegerYaz(satir, sut++, 12);
            double ayMal;
            if (toplamAl.toplam[toplamSatir, 0] > 0)
                ayMal = toplamAl.toplam[toplamSatir, 1] / toplamAl.toplam[toplamSatir, 0];
            else
                ayMal = 0;
            XLS.HucreDegerYaz(satir, sut++, toplamAl.toplam[toplamSatir, 1] / toplamAl.toplam[toplamSatir, 0]);
            XLS.HucreDegerYaz(satir, sut++, toplamAl.toplam[toplamSatir, 1]);

            XLS.HucreDegerYaz(satir, sut++, toplamAl.toplam[toplamSatir, 2] / 12);
            XLS.HucreDegerYaz(satir, sut++, 12);
            if (toplamAl.toplam[toplamSatir, 2] > 0)
                ayMal = toplamAl.toplam[toplamSatir, 3] / toplamAl.toplam[toplamSatir, 2];
            else
                ayMal = 0;
            XLS.HucreDegerYaz(satir, sut++, toplamAl.toplam[toplamSatir, 3] / toplamAl.toplam[toplamSatir, 2]);
            XLS.HucreDegerYaz(satir, sut++, toplamAl.toplam[toplamSatir, 3]);

            XLS.HucreDegerYaz(satir, sut++, toplamAl.toplam[toplamSatir, 4] / 12);
            XLS.HucreDegerYaz(satir, sut++, 12);
            XLS.HucreDegerYaz(satir, sut++, toplamAl.toplam[toplamSatir, 4]);
            toplamAl.SatirSifirla(toplamSatir);
        }

        /// <summary>Birimlerin form durumu ve toplamları (eski DurumlariYukle / Onay İşlemleri penceresi).</summary>
        internal static string BirimSorgu(AracKiralamaPlanAna form)
        {
            string sql = "SELECT MB.MUHASEBEKOD, MB.BIRIMKOD, M.MUHASEBEAD, MB.BIRIMAD, A.DURUM, SUM(BUYILKIRALANAN*BUYILKIRALANANSURE), SUM(BUYILKIRALANAN*BUYILKIRALANANSURE*BUYILFIYAT), SUM(GELECEKYILKIRALANACAK*GELECEKYILKIRALANACAKSURE), SUM(ONCYILKIRALANAN*ONCYILKIRALANANSURE), SUM(ONCYILKIRALANAN*ONCYILKIRALANANSURE*ONCYILFIYAT) ";
            sql += " FROM ButceIslemleri.dbo.MUHMUHASEBE M";
            sql += " INNER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM MB ON MB.MUHASEBEKOD = M.MUHASEBEKOD";
            sql += $" LEFT OUTER JOIN ButceHazirlik.dbo.BHFARACKIRALAMAPLANANA A ON A.MUHASEBEKOD = MB.MUHASEBEKOD AND A.BIRIMKOD = MB.BIRIMKOD AND A.YIL={form.yil} ";
            sql += $" LEFT OUTER JOIN ButceHazirlik.dbo.BHFARACKIRALAMAPLANDETAY D ON D.MUHASEBEKOD = MB.MUHASEBEKOD AND D.BIRIMKOD = MB.BIRIMKOD AND D.YIL={form.yil}";
            sql += " WHERE MB.BIRIMKOD!='000' AND MB.BIRIMKOD!='250'";
            if (form.muhasebeKod > 0)
                sql += $" AND M.MUHASEBEKOD = {form.muhasebeKod} ";
            if (!string.IsNullOrWhiteSpace(form.birimKod))
                sql += $" AND MB.BIRIMKOD = {OrtakFonksiyonlar.Tirnakla(form.birimKod)} ";

            sql += $" AND 1 NOT IN (SELECT 1 FROM ButceIslemleri.dbo.MUHMUHASEBEBIRIMKAPALI K where K.BIRIMKOD = MB.BIRIMKOD AND K.MUHASEBEKOD = MB.MUHASEBEKOD) ";

            sql += " group by MB.MUHASEBEKOD, MB.BIRIMKOD, M.MUHASEBEAD, MB.BIRIMAD, A.DURUM";
            sql += " order by 1, 2";
            return sql;
        }

        /// <summary>Kurum Yazdır: bütün birimlerin taşıt kodlarına göre toplamı. Üretilen dosyanın yolunu döndürür.</summary>
        internal string YazdirKurum()
        {
            string[] sql = new string[1];
            string[] baglanti = new string[1];

            sql[0] = "SELECT D.BUTCEKAYNAGI, D.TASITKODU, SUM(BUYILKIRALANAN*BUYILKIRALANANSURE), SUM(BUYILKIRALANAN*BUYILKIRALANANSURE*BUYILFIYAT), SUM(GELECEKYILKIRALANACAK*GELECEKYILKIRALANACAKSURE), SUM(ONCYILKIRALANAN*ONCYILKIRALANANSURE), SUM(ONCYILKIRALANAN*ONCYILKIRALANANSURE*ONCYILFIYAT) ";
            sql[0] += " FROM ButceHazirlik.dbo.BHFARACKIRALAMAPLANDETAY D";
            sql[0] += $" WHERE D.YIL={form.yil} GROUP BY D.BUTCEKAYNAGI, D.TASITKODU ORDER BY D.BUTCEKAYNAGI, D.TASITKODU ";
            baglanti[0] = "BaglantiSatiriBHM";

            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            SortedList sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0, 1 });

            Sonuc s = new Sonuc();
            AracKiralamaPlanAna o = new AracKiralamaPlanAna();
            o.yil = form.yil;
            o.aracKiralamaPlanDetay = new TNSCollection();

            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                AracKiralamaPlanDetay det = new AracKiralamaPlanDetay();
                det.butceKaynagi = alAnahtar.IntAl(0);
                det.tasitKodu = alAnahtar.StrAl(1);

                double aracAyBu = alVeri.DblAl(0);
                double toplamMaliyetBu = alVeri.DblAl(1);
                double aracAyGel = alVeri.DblAl(2);
                double aracAyOnc = alVeri.DblAl(3);
                double toplamMaliyetOnc = alVeri.DblAl(4);

                if (aracAyOnc > 0)
                {
                    det.oncYilFiyat = toplamMaliyetOnc / aracAyOnc;
                    det.oncYilKiralanan = aracAyOnc / 12;
                    det.oncYilKiralananSure = 12;
                }

                if (aracAyBu > 0)
                {
                    det.buYilFiyat = toplamMaliyetBu / aracAyBu;
                    det.buYilKiralanan = aracAyBu / 12;
                    det.buYilKiralananSure = 12;
                }
                if (aracAyGel > 0)
                {
                    det.gelecekYilKiralanacak = aracAyGel / 12;
                    det.gelecekYilKiralanacakSure = 12;
                }
                o.aracKiralamaPlanDetay.Add(det);
            }

            return YazdirOkunan(s, o, TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD") ?? "");
        }

        /// <summary>Detay Yazdır: bütün birimlerin satırları alt alta. Üretilen dosyanın yolunu döndürür.</summary>
        internal string YazdirDetay()
        {
            string[] sqller = new string[1];
            string[] baglanti = new string[1];

            string sql = "SELECT D.MUHASEBEKOD, D.BIRIMKOD, SIRA, M.MUHASEBEAD, MB.BIRIMAD, TASITKODU, BUTCEKAYNAGI, ONCYILKIRALANAN, ONCYILKIRALANANSURE, ONCYILFIYAT, BUYILKIRALANAN, BUYILKIRALANANSURE, BUYILFIYAT, GELECEKYILKIRALANACAK, GELECEKYILKIRALANACAKSURE FROM ButceHazirlik.dbo.BHFARACKIRALAMAPLANDETAY D ";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE M ON M.MUHASEBEKOD = D.MUHASEBEKOD";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM MB ON MB.MUHASEBEKOD = D.MUHASEBEKOD AND MB.BIRIMKOD = D.BIRIMKOD";
            sql += $" WHERE D.YIL={form.yil}";
            sql += " order by 1, 2, 3";

            sqller[0] = sql;
            baglanti[0] = "BaglantiSatiriBHM";
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            SortedList sl = uzyServis.SorguBirlestir(sqller, baglanti, new int[] { 0, 1, 2 });

            string sonucDosya = DosyaIslem.DosyaAdUret("AracKiralamaListe_", "") + "." + GenelIslemler.ExcelTur();
            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonYol + "/BHF/AracKiralamaListe.xltx", sonucDosya);
            int fSat = 0, fSut = 0, baslaSatir = 0, sutun = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref fSat, ref fSut);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);

            int satir = baslaSatir;
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                int sut = 0;
                XLS.HucreKopyala(fSat, fSut, fSat, fSut + 18, satir, sut);
                XLS.HucreDegerYaz(satir, sut++, alAnahtar.IntAl(0));
                XLS.HucreDegerYaz(satir, sut++, alAnahtar.StrAl(1));
                XLS.HucreDegerYaz(satir, sut++, TNS.MUH.Arac.BirimAdOl(alVeri.StrAl(0), alVeri.StrAl(1)));
                XLS.HucreDegerYaz(satir, sut++, alVeri.StrAl(2));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(3));

                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(4));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(5));
                XLS.HucreDegerYaz(satir, sut++, alVeri.DblAl(6));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(4) * alVeri.IntAl(5));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(4) * alVeri.IntAl(5) * alVeri.DblAl(6));

                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(7));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(8));
                XLS.HucreDegerYaz(satir, sut++, alVeri.DblAl(9));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(7) * alVeri.IntAl(8));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(7) * alVeri.IntAl(8) * alVeri.DblAl(9));

                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(10));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(11));
                XLS.HucreDegerYaz(satir, sut++, alVeri.IntAl(10) * alVeri.IntAl(11));
                satir++;
            }
            XLS.FormulleriSil();
            XLS.SatirSil(0, 2);
            XLS.DosyaSaklaTamYol();
            return sonucDosya;
        }
    }
}

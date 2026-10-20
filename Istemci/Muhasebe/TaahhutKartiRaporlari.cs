using System;
using System.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;
using static Istemci.Muhasebe.MuhTanimOrtak;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Taahhüt Kartı raporları; eski TaahhutKarti.aspx kodunun aynısıdır:
    ///   Rapor     : btnRapor_Click (MUH\TaahhutListe.xlt)
    ///   Yazdir    : btnYazdir_Click (MUH\TaahhutKarti.xlt)
    ///   KaraListe : btnKaraListe_Click (MUH\MUH-KaraListeFirma.xlt)
    /// Şablon önce kurumun RaporSablonYol\MUH klasöründe, yoksa SGB.Net\RaporSablon\MUH klasöründe aranır.
    /// </summary>
    public static class TaahhutKartiRaporlari
    {
        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static string Q(HttpContext baglam, string ad) { return Temiz(baglam.Request.Query[ad]); }
        private static int QSayi(HttpContext baglam, string ad, int varsayilan = 0) { return OrtakFonksiyonlar.ConvertToInt(Q(baglam, ad), varsayilan); }

        /// <summary>Belge Arama "Rapor" tuşu: arama kriterlerine uyan taahhütlerin listesi.</summary>
        public static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam)
        {
            int tur = QSayi(baglam, "tur", 1) == 2 ? 2 : 1;
            IResult ret = Denetle(baglam, k => TaahhutKartiUclari.Yetkili(k, ortam.ContentRootPath, tur), out Kullanici kullanan);
            if (ret != null) return ret;
            string sablon = SablonBul(ortam.ContentRootPath, "TaahhutListe.xlt");
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\TaahhutListe.xlt).");
            try
            {
                TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
                bilgi.yil = QSayi(baglam, "yil");
                bilgi.muhasebeKod = QSayi(baglam, "muhasebeKod");
                bilgi.birimKod = Q(baglam, "birimKod");
                bilgi.muteahhitKimlikNo = Q(baglam, "vergiNo");
                bilgi.muteahhitUnvan = Q(baglam, "adSoyad");
                bilgi.projeSiraNo = Q(baglam, "projeSiraNo");
                bilgi.konu = Q(baglam, "konu");
                bilgi.sozTur = tur == 2 ? TaahhutKartiBilgi.ENUMSozTur.Personel : TaahhutKartiBilgi.ENUMSozTur.Normal;
                ObjectArray liste = Servis().TaahhutKartiListele(kullanan, bilgi, 0);
                if (liste != null && !liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc.hataStr, "Rapor hazırlanamadı."));

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int baslangicSatiri = 0;
                int baslangicSutunu = 0;
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("TaahhutListe_", "") + "." + GenelIslemler.ExcelTur();
                XLS.DosyaAc(sablon, sonucDosyaAd);
                XLS.HucreAdAdresCoz("BaslangicSatiri", ref baslangicSatiri, ref baslangicSutunu);

                int satir = baslangicSatiri + 1;
                int sayi = 1;
                double ihaleBedeli = 0.0;
                double sorguTaahhutTutar = 0;
                double sorguOdenenTutar = 0;
                Hashtable birimAd = new Hashtable();
                string birim = "";

                if (liste != null)
                {
                    foreach (TaahhutKartiBilgi t in liste.objeler)
                    {
                        string bk = t.birimKod + "-" + t.muhasebeKod.ToString().Trim();
                        if (birimAd[bk] == null)
                        {
                            birim = GenelIslemler.KodAd(12, bk, false);
                            birimAd[bk] = birim;
                        }
                        else
                            birim = (string)birimAd[bk];

                        string kur = "", fon = "", fin = "", eko = "", peb = "";
                        if (!string.IsNullOrEmpty(t.odenekTertip))
                        {
                            string[] tertip = t.odenekTertip.Split('-');
                            kur = tertip.Length > 0 ? tertip[0] : "";
                            fon = tertip.Length > 1 ? tertip[1] : "";
                            fin = tertip.Length > 2 ? tertip[2] : "";
                            eko = tertip.Length > 3 ? tertip[3] : "";
                            peb = tertip.Length > 4 ? tertip[4] : "";
                        }

                        XLS.HucreKopyala(baslangicSatiri, baslangicSutunu, baslangicSatiri, 18, satir, baslangicSutunu);
                        XLS.HucreDegerYaz(satir, baslangicSutunu, sayi);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 1, t.projeSiraNo);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 2, birim);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 3, t.muteahhitKimlikNo);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 4, t.muteahhitUnvan);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 5, t.konu);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 6, kur);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 7, fon);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 8, fin);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 9, eko);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 10, peb);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 11, t.sozlesmeTarihi.ToString());
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 12, t.alimUsuluKod);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 13, t.sgkBildirimi == 1 ? "Var" : "Yok");
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 14, t.ihaleBedeli);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 15, t.sorguTaahhutTutar);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 16, t.sorguOdenenTutar);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 17, t.sorguTaahhutTutar - t.sorguOdenenTutar);
                        satir++;
                        sayi++;

                        ihaleBedeli += t.ihaleBedeli;
                        sorguTaahhutTutar += t.sorguTaahhutTutar;
                        sorguOdenenTutar += t.sorguOdenenTutar;
                    }

                    if (liste.objeler.Count > 0)
                    {
                        XLS.HucreKopyala(baslangicSatiri, baslangicSutunu, baslangicSatiri, 18, satir, baslangicSutunu);
                        XLS.KoyuYap(satir, baslangicSutunu, satir, baslangicSutunu + 18, true);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 14, ihaleBedeli);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 15, sorguTaahhutTutar);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 16, sorguOdenenTutar);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 17, sorguTaahhutTutar - sorguOdenenTutar);
                    }
                }

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), "TaahhutListe", true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        /// <summary>Kara Liste: kartın muhasebe ve birimindeki kara listedeki firmalar.</summary>
        public static IResult KaraListe(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, k => TaahhutKartiUclari.YetkiVar(k, ortam.ContentRootPath), out Kullanici kullanan);
            if (ret != null) return ret;
            string sablon = SablonBul(ortam.ContentRootPath, "MUH-KaraListeFirma.xlt");
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\MUH-KaraListeFirma.xlt).");
            try
            {
                TNS.HRC.FirmaBilgisi fSorgu = new TNS.HRC.FirmaBilgisi();
                fSorgu.muhasebe = QSayi(baglam, "muhasebe");
                fSorgu.birim = Q(baglam, "birim");
                fSorgu.karaListe = 1;
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, fSorgu);

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                int baslangicSatiri = 0;
                int baslangicSutunu = 0;
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("KaraListe_", "") + "." + GenelIslemler.ExcelTur();
                XLS.DosyaAc(sablon, sonucDosyaAd);
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslangicSatiri, ref baslangicSutunu);

                int satir = baslangicSatiri + 1;
                int sayi = 1;
                if (bilgi != null && bilgi.sonuc.islemSonuc)
                {
                    foreach (TNS.HRC.FirmaBilgisi f in bilgi.objeler)
                    {
                        XLS.HucreKopyala(baslangicSatiri, baslangicSutunu, baslangicSatiri, 15, satir, baslangicSutunu);
                        XLS.HucreDegerYaz(satir, baslangicSutunu, sayi);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 1, f.vergiNo);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 2, f.vergiDairesi);
                        XLS.HucreDegerYaz(satir, baslangicSutunu + 3, f.ad);
                        satir++;
                        sayi++;
                    }
                }
                XLS.SatirSil(baslangicSatiri, baslangicSatiri);

                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), "KaraListe", true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        private static void KodlariYaz(Tablo XLS, string parca, params string[] adlar)
        {
            if (parca == null) return;
            string[] kodlarArr = parca.Split('.');
            for (int i = 0; i < adlar.Length && i < kodlarArr.Length; i++)
                XLS.HucreAdBulYaz(adlar[i], kodlarArr[i]);
        }

        /// <summary>Belge Düzenleme "Yazdır" tuşu: Taahhüt Kartı (1. sayfa kart, 2. sayfa ödemeler).</summary>
        public static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, k => TaahhutKartiUclari.YetkiVar(k, ortam.ContentRootPath), out Kullanici kullanan);
            if (ret != null) return ret;
            int kod = QSayi(baglam, "kod");
            if (kod <= 0) return Hata("Önce bir taahhüt kartı seçin.");
            string sablon = SablonBul(ortam.ContentRootPath, "TaahhutKarti.xlt");
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\TaahhutKarti.xlt).");
            try
            {
                IMUHServis servisMUH = Servis();
                TaahhutKartiBilgi sorgu = new TaahhutKartiBilgi();
                sorgu.kod = kod;
                ObjectArray liste = servisMUH.TaahhutKartiListele(kullanan, sorgu, 0);
                if (liste == null) return Hata("Taahhüt kartı okunamadı.");
                if (!liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc.hataStr, "Taahhüt kartı okunamadı."));

                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string sonucDosyaAd = DosyaIslem.DosyaAdUret("TaahhutKarti_", "") + "." + GenelIslemler.ExcelTur();
                XLS.DosyaAc(sablon, sonucDosyaAd);

                Hashtable htDegerArtisi = new Hashtable();
                double ihaleBedeli = 0.0;
                int ihaleYili = 0;
                TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

                foreach (TaahhutKartiBilgi b in liste.objeler)
                {
                    // Değer artışı
                    if (b.degerArtisi != null && b.degerArtisi.objeler != null)
                        foreach (TaahhutKartiDegerArtisi da in b.degerArtisi.objeler)
                            TNS.MUH.Arac.HTDoubleEkle(htDegerArtisi, da.sozlesmeTarihi.Yil.ToString(), da.sozlesmeTutari, false);

                    XLS.HucreAdBulYaz("muhasebeKodu", b.muhasebeKod);
                    XLS.HucreAdBulYaz("muhasebeAdi", servisUZY.UzayDegeriStr(new Kullanici(), "MUHMUHASEBE", b.muhasebeKod.ToString(), true, "MUHASEBEAD"));
                    XLS.HucreAdBulYaz("projeSiraNo", b.projeSiraNo.ToString());
                    XLS.HucreAdBulYaz("DptProjeSiraNo", b.DptProjeSiraNo.ToString());
                    XLS.HucreAdBulYaz("kurumKodu", GenelIslemlerIstemci.VarsayilanKurumBul());
                    XLS.HucreAdBulYaz("kurumAdi", GenelIslemlerIstemci.VarsayilanKurumBulAd());
                    XLS.HucreAdBulYaz("muteahhitKimlikNo", b.muteahhitKimlikNo);
                    XLS.HucreAdBulYaz("muteahhitUnvan", b.muteahhitUnvan);
                    XLS.HucreAdBulYaz("muteahhitAdres", b.muteahhitAdres);
                    XLS.HucreAdBulYaz("konu", b.konu);
                    XLS.HucreAdBulYaz("kesifBeldeli", b.kesifBeldeli.ToString("#,###.00"));
                    XLS.HucreAdBulYaz("indirimOrani", b.indirimOrani.ToString("#,###.00"));
                    XLS.HucreAdBulYaz("ihaleBedeli", b.ihaleBedeli.ToString("#,###.00"));
                    XLS.HucreAdBulYaz("gerceklesenTaahhutTutar", b.gerceklesenTaahhutTutar.ToString("#,###.00"));
                    XLS.HucreAdBulYaz("damgaVergisiTarihi", b.damgaVergisiTarihi.ToString());
                    XLS.HucreAdBulYaz("damgaVergisiNo", b.damgaVergisiNo);
                    XLS.HucreAdBulYaz("damgaVergisiTutar", b.damgaVergisiTutar.ToString("#,###.00"));
                    XLS.HucreAdBulYaz("sozlesmeDamgaVergisiTarihi", b.sozlesmeDVTarihi.ToString());
                    XLS.HucreAdBulYaz("sozlesmeDamgaVergisiNumarasi", b.sozlesmeDVNo);
                    XLS.HucreAdBulYaz("sozlesmeDamgaVergisiTutari", b.sozlesmeDVTutar.ToString("#,###.00"));

                    ihaleBedeli = b.ihaleBedeli;
                    ihaleYili = b.iseBaslamaTarihi.Yil;

                    XLS.HucreAdBulYaz("sozlesmeTarihi", b.sozlesmeTarihi.ToString());
                    XLS.HucreAdBulYaz("iseBaslamaTarihi", b.iseBaslamaTarihi.ToString());
                    XLS.HucreAdBulYaz("geciciKabulTarihi", b.geciciKabulTarihi.ToString());
                    XLS.HucreAdBulYaz("isBitirmeTarihi", b.isBitirmeTarihi.ToString());
                    XLS.HucreAdBulYaz("sureUzatimi1", b.sureUzatimi1);
                    XLS.HucreAdBulYaz("sureUzatimi2", b.sureUzatimi2);
                    XLS.HucreAdBulYaz("sureUzatimi3", b.sureUzatimi3);
                    XLS.HucreAdBulYaz("taahhutAciklama", b.aciklama);

                    if (!string.IsNullOrEmpty(b.odenekTertip))
                    {
                        string[] tertipArr = b.odenekTertip.Split('-');
                        KodlariYaz(XLS, tertipArr.Length > 0 ? tertipArr[0] : null, "taahhutKurumsalKod1", "taahhutKurumsalKod2", "taahhutKurumsalKod3", "taahhutKurumsalKod4");
                        KodlariYaz(XLS, tertipArr.Length > 1 ? tertipArr[1] : null, "taahhutFonksiyonKod1", "taahhutFonksiyonKod2", "taahhutFonksiyonKod3", "taahhutFonksiyonKod4");
                        KodlariYaz(XLS, tertipArr.Length > 2 ? tertipArr[2] : null, "taahhutFinansalKod");
                        KodlariYaz(XLS, tertipArr.Length > 3 ? tertipArr[3] : null, "taahhutEkomomikKod1", "taahhutEkomomikKod2");
                    }

                    if (!string.IsNullOrEmpty(b.duranTertip))
                    {
                        string[] tertipArr = b.duranTertip.Split('-');
                        KodlariYaz(XLS, tertipArr.Length > 0 ? tertipArr[0] : null, "duranVarlikKurumsalKod1", "duranVarlikKurumsalKod2", "duranVarlikKurumsalKod3", "duranVarlikKurumsalKod4");
                        KodlariYaz(XLS, tertipArr.Length > 1 ? tertipArr[1] : null, "duranVarlikFonksiyonKod1", "duranVarlikFonksiyonKod2", "duranVarlikFonksiyonKod3", "duranVarlikFonksiyonKod4");
                        KodlariYaz(XLS, tertipArr.Length > 2 ? tertipArr[2] : null, "duranVarlikFinansalKod");
                        KodlariYaz(XLS, tertipArr.Length > 3 ? tertipArr[3] : null, "duranVarlikEkonomikKod1", "duranVarlikEkonomikKod2", "duranVarlikEkonomikKod3", "duranVarlikEkonomikKod4");
                    }
                }

                // Avanslar
                int satirAvans = 0;
                int sutunAvans = 0;
                XLS.HucreAdAdresCoz("AvansBaslaSatir", ref satirAvans, ref sutunAvans);
                int yazSatirAvans = satirAvans + 1;
                ObjectArray listeAvanslar = servisMUH.TaahhutAvanslarListele(kullanan, new TaahhutAvanslar() { taahhutKod = kod });
                if (listeAvanslar != null && listeAvanslar.objeler != null)
                {
                    foreach (TaahhutAvanslar a in listeAvanslar.objeler)
                    {
                        XLS.HucreKopyala(satirAvans, sutunAvans, satirAvans, sutunAvans + 7, yazSatirAvans, sutunAvans);
                        XLS.HucreDegerYaz(yazSatirAvans, sutunAvans + 0, a.tarih.ToString());
                        XLS.HucreDegerYaz(yazSatirAvans, sutunAvans + 3, a.numara);
                        XLS.HucreDegerYaz(yazSatirAvans, sutunAvans + 4, a.verilenAvans);
                        XLS.HucreDegerYaz(yazSatirAvans, sutunAvans + 7, a.mahsupEdilenAvans);
                        yazSatirAvans++;
                    }
                }
                XLS.SatirYukseklikAyarla(satirAvans + 1, yazSatirAvans, -1);

                // Alınan teminatlar
                int satirTeminat = 0;
                int sutunTeminat = 0;
                XLS.HucreAdAdresCoz("AvansBaslaTeminat", ref satirTeminat, ref sutunTeminat);
                int yazSatirTeminat = satirTeminat + 1;
                ObjectArray listeTeminat = servisMUH.TaahhutTeminatlarListele(kullanan, new TaahhutTeminatlar() { taahhutKod = kod });
                if (listeTeminat != null && listeTeminat.objeler != null)
                {
                    foreach (TaahhutTeminatlar t in listeTeminat.objeler)
                    {
                        XLS.HucreDegerYaz(yazSatirTeminat, sutunTeminat + 0, t.tarih.ToString());
                        XLS.HucreDegerYaz(yazSatirTeminat, sutunTeminat + 2, t.numara);
                        XLS.HucreDegerYaz(yazSatirTeminat, sutunTeminat + 3, t.aciklama);
                        XLS.HucreDegerYaz(yazSatirTeminat, sutunTeminat + 10, t.tutar);
                        yazSatirTeminat++;
                    }
                }
                XLS.SatirYukseklikAyarla(satirTeminat + 1, yazSatirTeminat, -1);

                // Yıllara göre taahhüt planı
                int satirTaahhut = 0;
                int sutunTaahhut = 0;
                XLS.HucreAdAdresCoz("taahhutSatir", ref satirTaahhut, ref sutunTaahhut);
                int yazSatirTaahhut = satirTaahhut;

                ObjectArray listeTaahhut = servisMUH.TaahhutListele(kullanan, new TaahhutBilgi() { taahhutKod = kod });
                if (listeTaahhut == null) listeTaahhut = new ObjectArray();
                if (listeTaahhut.objeler == null) listeTaahhut.objeler = new TNSCollection();

                // Taahhüdün kendisi
                bool bulunduIhale = false;
                foreach (TaahhutBilgi t in listeTaahhut.objeler)
                {
                    if (t.yil == ihaleYili)
                    {
                        t.ilkTaahhutTutari += ihaleBedeli;
                        bulunduIhale = true;
                        break;
                    }
                }
                if (!bulunduIhale)
                    listeTaahhut.objeler.Add(new TaahhutBilgi() { yil = ihaleYili, ilkTaahhutTutari = ihaleBedeli });

                // Değer artışını taahhüt bilgisine yaz
                foreach (DictionaryEntry dt in htDegerArtisi)
                {
                    int anahtarYil = OrtakFonksiyonlar.ConvertToInt((string)dt.Key, 0);
                    double tutar = (double)dt.Value;
                    bool bulundu = false;
                    foreach (TaahhutBilgi t in listeTaahhut.objeler)
                    {
                        if (t.yil == anahtarYil)
                        {
                            t.digerTaahhutler += tutar;
                            bulundu = true;
                            break;
                        }
                    }
                    if (!bulundu)
                        listeTaahhut.objeler.Add(new TaahhutBilgi() { yil = anahtarYil, digerTaahhutler = tutar });
                }

                int[] taahutKolonNo = { 18, 22, 26, 29, 32, 35 };
                for (int i = 0; i < taahutKolonNo.Length; i++)
                    XLS.HucreDegerYaz(yazSatirTaahhut, taahutKolonNo[i], ihaleYili + i);

                if (listeTaahhut.objeler.Count > 0)
                {
                    int kolon = 0;
                    int satirT = yazSatirTaahhut;
                    double ilkTaahhutTutari = 0.0;
                    double fiyatFarki = 0.0;
                    double ilaveTaahhutler = 0.0;
                    double digerTaahhutler = 0.0;
                    double toplamTaahut = 0.0;

                    foreach (TaahhutBilgi t in listeTaahhut.objeler)
                    {
                        if (t.yil == 0) continue;
                        int kararYil = t.yil - ihaleYili;
                        if (kararYil < 0) kararYil = 0;
                        // Eski kodda 6 yıldan sonrası dizinin dışına çıkıp raporu bozuyordu; son sütuna yazılır.
                        if (kararYil >= taahutKolonNo.Length) kararYil = taahutKolonNo.Length - 1;
                        kolon = taahutKolonNo[kararYil];

                        yazSatirTaahhut = satirT;
                        yazSatirTaahhut++;
                        XLS.HucreDegerYaz(yazSatirTaahhut, kolon, t.ilkTaahhutTutari.ToString("#,###.00"));
                        ilkTaahhutTutari += t.ilkTaahhutTutari;
                        yazSatirTaahhut++;
                        XLS.HucreDegerYaz(yazSatirTaahhut, kolon, t.fiyatFarki.ToString("#,###.00"));
                        fiyatFarki += t.fiyatFarki;
                        yazSatirTaahhut++;
                        XLS.HucreDegerYaz(yazSatirTaahhut, kolon, t.ilaveTaahhutler.ToString("#,###.00"));
                        ilaveTaahhutler += t.ilaveTaahhutler;
                        yazSatirTaahhut++;
                        XLS.HucreDegerYaz(yazSatirTaahhut, kolon, t.digerTaahhutler.ToString("#,###.00"));
                        digerTaahhutler += t.digerTaahhutler;
                        yazSatirTaahhut++;
                        yazSatirTaahhut++;
                        yazSatirTaahhut++;
                        XLS.HucreDegerYaz(yazSatirTaahhut, kolon, t.ilkTaahhutTutari + t.fiyatFarki + t.ilaveTaahhutler + t.digerTaahhutler);
                        toplamTaahut += t.ilkTaahhutTutari + t.fiyatFarki + t.ilaveTaahhutler + t.digerTaahhutler;
                    }
                    yazSatirTaahhut = satirT;
                    yazSatirTaahhut++;
                    XLS.HucreDegerYaz(yazSatirTaahhut, sutunTaahhut + 20, ilkTaahhutTutari.ToString("#,###.00"));
                    yazSatirTaahhut++;
                    XLS.HucreDegerYaz(yazSatirTaahhut, sutunTaahhut + 20, fiyatFarki.ToString("#,###.00"));
                    yazSatirTaahhut++;
                    XLS.HucreDegerYaz(yazSatirTaahhut, sutunTaahhut + 20, ilaveTaahhutler.ToString("#,###.00"));
                    yazSatirTaahhut++;
                    XLS.HucreDegerYaz(yazSatirTaahhut, sutunTaahhut + 20, digerTaahhutler.ToString("#,###.00"));
                    yazSatirTaahhut = yazSatirTaahhut + 3;
                    XLS.HucreDegerYaz(yazSatirTaahhut, sutunTaahhut + 20, toplamTaahut.ToString("#,###.00"));
                }
                XLS.SatirYukseklikAyarla(sutunTaahhut + 1, yazSatirTaahhut, -1);

                // Borçlar ve temlikler
                int satirBorclar = 0;
                int sutunBorclar = 0;
                XLS.HucreAdAdresCoz("AvansBaslaBorclar", ref satirBorclar, ref sutunBorclar);
                int yazSatirBorclar = satirBorclar + 1;
                ObjectArray listeBorclar = servisMUH.TaahhutBorclarListele(kullanan, new TaahhutBorclar() { taahhutKod = kod });
                int siraNo = 0;
                if (listeBorclar != null && listeBorclar.objeler != null)
                {
                    foreach (TaahhutBorclar b in listeBorclar.objeler)
                    {
                        siraNo++;
                        XLS.HucreKopyala(satirBorclar, sutunBorclar, satirBorclar, sutunBorclar + 11, yazSatirBorclar, sutunBorclar);
                        XLS.HucreDegerYaz(yazSatirBorclar, sutunBorclar + 0, siraNo);
                        XLS.HucreDegerYaz(yazSatirBorclar, sutunBorclar + 1, b.alacakliAdSoyad);
                        XLS.HucreDegerYaz(yazSatirBorclar, sutunBorclar + 6, b.kod);
                        XLS.HucreDegerYaz(yazSatirBorclar, sutunBorclar + 7, b.temlikTutari.ToString("#,###.00"));
                        XLS.HucreDegerYaz(yazSatirBorclar, sutunBorclar + 11, "");
                        yazSatirBorclar++;
                    }
                }
                XLS.SatirYukseklikAyarla(satirBorclar + 1, yazSatirBorclar, -1);

                // 2. sayfa: ödemeler
                int satirOdeme = 0;
                int sutunOdeme = 0;
                XLS.AktifSheetDegistir(1);
                XLS.HucreAdAdresCoz("odemeBaslaSatir", ref satirOdeme, ref sutunOdeme);
                int yazSatirOdeme = satirOdeme + 1;
                ObjectArray listeOdeme = servisMUH.TaahhutOdemeEmriListele(kullanan, new TaahhutOdeme() { taahhutKod = kod });
                if (listeOdeme != null && listeOdeme.objeler != null)
                {
                    double toplamHakedis = 0;
                    foreach (TaahhutOdeme t in listeOdeme.objeler)
                    {
                        XLS.HucreKopyala(satirOdeme, sutunOdeme, satirOdeme, sutunOdeme + 20, yazSatirOdeme, sutunOdeme);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 0, t.odemeEmriTarihi.Oku());
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 1, t.odemeEmriKod);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 2, t.aciklama);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 3, t.hakedisTutari);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 4, t.damgaVergisi);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 5, t.gelirVergisiStopaji);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 6, t.fonPayi);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 7, t.paraCezalari);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 8, t.mahsupEdilenAvans);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 9, t.icraKesintisi);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 10, t.teminatKesintisi);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 11, t.sskPrimiVeGecikmeZammi);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 12, t.vergiBorcu);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 13, t.kdv);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 14, t.kdvTevkifati);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 15, t.temlik);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 16, t.butceEmaneti);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 17, "");
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 18, "");
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 19, t.kesintilerToplami);
                        XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 20, t.yapilanOdeme);
                        toplamHakedis += t.hakedisTutari;
                        yazSatirOdeme++;
                    }
                    XLS.HucreKopyala(satirOdeme, sutunOdeme, satirOdeme, sutunOdeme + 20, yazSatirOdeme, sutunOdeme);
                    XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 2, "TOPLAM");
                    XLS.HucreDegerYaz(yazSatirOdeme, sutunOdeme + 3, toplamHakedis);
                    XLS.KoyuYap(yazSatirOdeme, 0, yazSatirOdeme, 25, true);
                }
                XLS.SatirYukseklikAyarla(satirOdeme + 1, yazSatirOdeme, -1);
                XLS.AktifSheetDegistir(0);
                XLS.DosyaSaklaTamYol();

                return DosyaIslem.DosyaGonder(XLS.SonucDosyaAd(), "TaahhutKarti_" + DateTime.Now.Ticks, true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Taahhüt kartı yazdırılamadı.");
            }
        }
    }
}

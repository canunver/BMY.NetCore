using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System.Collections;
using System;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeSulamaMalzeme
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, HibeSulamaMalzemeFiyat[] mfler, HibeSulama hs)
        {
            Sonuc sonuc = new Sonuc();

            if (mfler == null || mfler.Length == 0)
            {
                sonuc.hataStr = "Hiçbir malzeme fiyat bilgisi girilmemiş.";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (mfler[0].hibeKod == 0)
            {
                sonuc.hataStr = "İşlem yapılacak kayıta ait kod bilgisi alınamadı";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ObjectArray hmler = new TRMHibeSulama().Listele(kullanan, new HibeSulama { kod = mfler[0].hibeKod });
            if (hmler == null || hmler.objeler.Count == 0 || hmler.objeler.Count > 1)
            {
                sonuc.hataStr = "İşlem yapılacak kayıta ait bilgi okunamadı";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            HibeSulama hm = (HibeSulama)hmler.objeler[0];
            if (mfler[0].tip == ENUMHibeSulamaMalzemeTip.PROJE && hm.durum > (int)ENUMHibeSulamaBasvuruDurum.DEGISTIRILDI)
            {
                if (string.IsNullOrWhiteSpace(hs.kontrolTip))
                {
                    sonuc.hataStr = "Sonraki aşamaya geçildiği için başvuru bilgilerinde değişiklik yapılamaz";
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }


            //if (!hs.yerindeTespitTarih.isNull)
            //{
            //    sonuc.hataStr = "Yerinde Tespit Tutanağı kayıt edilmiş değişiklik yapılamaz.";
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            //if (!Yetki.YetkiVarMiYazmaya(kullanan, hs.ilKod, -1, string.Empty, ENUMHibeTur.SULAMA))
            if (!Yetki.YetkiVarMiYazmaya(kullanan, hs.ilKod, -1, sonuc, string.Empty, ENUMHibeTur.SULAMA, hs.kod, true))
            {
                return sonuc;
            }

            foreach (HibeSulamaMalzemeFiyat mf in mfler)
            {
                if (mf.hibeKod <= 0)
                    sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
                if (mf.tip <= 0)
                    sonuc.hataStr += "Tip boş bırakılamaz.<br>";
                if (mf.malzemeKod <= 0)
                    sonuc.hataStr += "Malzeme kodu boş bırakılamaz.<br>";
                //if ((mf.tip == ENUMHibeSulamaMalzemeTip.UYGULAMA) && (mf.miktar <= 0))
                //    sonuc.hataStr += "Miktar boş bırakılamaz.<br>";
                if ((mf.tip == ENUMHibeSulamaMalzemeTip.PROJE) && (mf.miktar <= 0))
                    sonuc.hataStr += "Miktar boş bırakılamaz.<br>";
                if (mf.birimFiyat <= 0 && mf.malzemeKod != 999)
                    sonuc.hataStr += "Birim fiyat boş bırakılamaz.<br>";
                if (mf.sulamaGrupKod == 0)
                    sonuc.hataStr += "Sulama grup boş bırakılamaz.<br>";
                if (mf.referansFiyat <= 0)
                {
                    if (mf.tip != ENUMHibeSulamaMalzemeTip.UYGULAMA && mf.malzemeKod != 999)
                        sonuc.hataStr += "Referans fiyat boş bırakılamaz.";
                }

                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.islemSonuc = false;
                    return sonuc;
                }
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            //Bu işlem başvuru sırasında girilen mazlemeler ile fatura aşamaındaki malzemelerin referasn fiyatlarını kontrol ediyor. Fakat dinamik olan listeleri karşılaştırma sırasında hatalı işlem yapıyor. Bu nedenle kapatıldı. 17.11.2013 Melih
            if (mfler[0].tip == ENUMHibeSulamaMalzemeTip.UYGULAMA)
            {
                sonuc = ProjeFiyatKontrol(ve, kullanan, hs.ilKod, mfler, mfler[0].parselID);
                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            TABHibeSulamaMalzeme tMalzeme = new TABHibeSulamaMalzeme(ve);
            ve.BeginTransaction();

            sonuc = Sil(ve, kullanan, mfler[0].hibeKod, mfler[0].tip, hs.ilKod, mfler[0].sulamaGrupKod, mfler[0].parselID, false);

            //sonuc = new TRMHibeSulama().DetayKonuKaydet(ve, kullanan, hs.detayKonu);/////DETAY KONUYA İHTİYAÇ KALMADI

            //************************TOPLAM TUTARLARI KAYIT ETMEK ICIN*****************************
            sonuc = Sil(ve, kullanan, mfler[0].hibeKod, mfler[0].tip, hs.ilKod, 0, "", false);

            HibeSulamaMalzemeFiyat[] liste = Listele(ve, kullanan, mfler[0].hibeKod, mfler[0].tip, hs.ilKod, 0, "");

            HibeSulamaMalzemeFiyat toplamMf = new HibeSulamaMalzemeFiyat();
            toplamMf.hibeKod = mfler[0].hibeKod;
            toplamMf.tip = mfler[0].tip;
            foreach (HibeSulamaMalzemeFiyat mf in liste)
            {
                toplamMf.miktar += mf.miktar;
                toplamMf.birimFiyat += mf.birimFiyat;
                toplamMf.referansFiyat += mf.referansFiyat;
                toplamMf.projeTutar += mf.projeTutar;
                toplamMf.hibeEsasTutar += mf.hibeEsasTutar;
            }

            for (int i = 0; i < mfler.Length; i++)
            {
                toplamMf.miktar += mfler[i].miktar;
                toplamMf.birimFiyat += mfler[i].birimFiyat;
                toplamMf.referansFiyat += mfler[i].referansFiyat;
                toplamMf.projeTutar += mfler[i].projeTutar = OrtakFonksiyonlar.Yuvarla(mfler[i].miktar * mfler[i].birimFiyat, 2);
                toplamMf.hibeEsasTutar += OrtakFonksiyonlar.Yuvarla(mfler[i].miktar * (mfler[i].birimFiyat < mfler[i].referansFiyat ? mfler[i].birimFiyat : mfler[i].referansFiyat), 2);
            }

            decimal projeToplam = toplamMf.projeTutar;
            decimal hibeEsasToplam = toplamMf.hibeEsasTutar;

            sonuc = tMalzeme.Kaydet(toplamMf);
            //************************TOPLAM TUTARLARI KAYIT ETMEK ICIN*****************************

            int satirNo = tMalzeme.SatirNoVer(mfler[0].hibeKod, mfler[0].sulamaGrupKod);

            if (sonuc.islemSonuc)
            {
                for (int i = 0; i < mfler.Length; i++)
                {
                    satirNo++;
                    mfler[i].satirNo = satirNo;
                    mfler[i].projeTutar = OrtakFonksiyonlar.Yuvarla(mfler[i].miktar * mfler[i].birimFiyat, 2);
                    mfler[i].hibeEsasTutar = OrtakFonksiyonlar.Yuvarla(mfler[i].miktar * (mfler[i].birimFiyat < mfler[i].referansFiyat ? mfler[i].birimFiyat : mfler[i].referansFiyat), 2);
                    sonuc = tMalzeme.Kaydet(mfler[i]);
                    if (!sonuc.islemSonuc)
                        break;

                    sonuc = KaydetOzellik(tMalzeme, mfler[i]);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc && hs.detay != null && hs.detay.Length > 0 &&
                ((mfler[0].tip == ENUMHibeSulamaMalzemeTip.PROJE && (hs.detay[0].proformaTutar != projeToplam || hs.detay[0].hibeEsasTutarProje != hibeEsasToplam)) ||
                (mfler[0].tip == ENUMHibeSulamaMalzemeTip.UYGULAMA && (hs.detay[0].malzemeTutar != projeToplam || hs.detay[0].hibeEsasTutarUygulama != hibeEsasToplam || hs.detay[0].gercekFatura == 0))))
            {
                if (mfler[0].tip == ENUMHibeSulamaMalzemeTip.PROJE)
                {
                    hs.detay[0].proformaTutar = projeToplam;
                    hs.detay[0].hibeEsasTutarProje = hibeEsasToplam;
                    hs.detay[0].proformaReferansTutar = hibeEsasToplam;
                }
                else
                {
                    hs.detay[0].malzemeTutar = projeToplam;
                    hs.detay[0].hibeEsasTutarUygulama = hibeEsasToplam;
                    /////   Fatura girişinde fatura referans fiyatı tutmak için eklenildi  ////
                    hs.detay[0].gercekFatura = hibeEsasToplam;
                }

                //if (hs.projeNo == "" )
                //{
                //    sonuc = new TRMHibeSulama().Kaydet(ve, kullanan, hs);
                //}

                hs.yazilimIciKayit = true;
                sonuc = new TRMHibeSulama().Kaydet(ve, kullanan, hs);

                TRMHibeSulama.HibeTutarHesapla(hs.hukukiStatu, ref hs.detay[0].hibeEsasTutarProje, ref hs.detay[0].hibeEsasTutarUygulama, hs.detay[0].proformaTutar, hs.detay[0].malzemeTutar, ref hs.detay[0].hibeEsasTutar, ref hs.detay[0].toplamTutar, ref hs.detay[0].ayniKatki, ref hs.detay[0].hibeTutar, hs.detay[0].hibeOran, hs.detay[0].basvuruAyniKatkiTutar);

                if (sonuc.islemSonuc)
                    sonuc.anahtar = projeToplam.ToString() + ";" + hs.detay[0].hibeEsasTutarProje.ToString() + ";" + hs.detay[0].proformaReferansTutar.ToString();
            }

            if (sonuc.islemSonuc)
            {
                for (int i = 0; i < hs.diger.Length; i++)
                {
                    if (!string.IsNullOrEmpty(hs.diger[i].faturaNo))
                    {
                        sonuc = new TRMHibeSulamaDiger().Guncelle(ve, kullanan, hs.diger[i]);
                    }
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                }
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc ProjeFiyatKontrol(VeriErisim ve, Kullanici kullanan, string hibeIlKod, HibeSulamaMalzemeFiyat[] mfler, string parselNo)
        {
            HibeSulamaMalzemeFiyat[] mflerProje = Listele(ve, kullanan, mfler[0].hibeKod, ENUMHibeSulamaMalzemeTip.PROJE, hibeIlKod, 0, parselNo);
            if (mflerProje == null || mflerProje.Length == 0)
                return new Sonuc(false, "Proje aşamasında malzeme girilmemiş.", string.Empty);

            decimal projeTutar = 0;
            decimal projeHibeEsasTutar = 0;
            decimal uygulamaTutar = 0;
            decimal uygulamaHibeEsasTutar = 0;

            foreach (HibeSulamaMalzemeFiyat mfProje in mflerProje)
            {
                projeTutar += mfProje.projeTutar;
                projeHibeEsasTutar += OrtakFonksiyonlar.Yuvarla(mfProje.hibeEsasTutar, 2);
            }

            for (int i = 0; i < mfler.Length; i++)
            {
                uygulamaTutar += mfler[i].projeTutar = OrtakFonksiyonlar.Yuvarla(mfler[i].miktar * mfler[i].birimFiyat, 2);
                uygulamaHibeEsasTutar += OrtakFonksiyonlar.Yuvarla(mfler[i].miktar * (mfler[i].birimFiyat < mfler[i].referansFiyat ? mfler[i].birimFiyat : mfler[i].referansFiyat), 2);
            }


            Sonuc sonuc = new Sonuc();
            sonuc.islemSonuc = true;
            if (uygulamaTutar > projeTutar || uygulamaHibeEsasTutar > projeHibeEsasTutar)
            {
                //Sistem Yöneticisine .5 kuruşlar için uyarı verme
                if (!(OrtakFonksiyonlar.ConvertToDbl(uygulamaHibeEsasTutar - projeHibeEsasTutar) < 0.5 && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Fatura malzeme tutarı, proje malzeme tutarlarından büyük olamaz.<br>Proje Tutarı:" + projeTutar.ToString("#,###.00") + "<br>Proje Hibeye Esas Tutar:" + projeHibeEsasTutar.ToString("#,###.00") + "<br>Fatura Tutarı:" + uygulamaTutar.ToString("#,###.00") + "<br>Fatura Hibeye Esas Tutar:" + uygulamaHibeEsasTutar.ToString("#,###.00");
                }
            }

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, int hibeKod, ENUMHibeSulamaMalzemeTip tip, string hibeIlKod, int sulamaGrupKod, string parselID, bool toplamHesapla)
        {
            Sonuc sonuc = new Sonuc();

            if (hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (tip <= 0)
                sonuc.hataStr += "Tip boş bırakılamaz.";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //if (!Yetki.YetkiVarMiYazmaya(kullanan, hibeIlKod, -1, string.Empty, ENUMHibeTur.SULAMA))
            if (!Yetki.YetkiVarMiYazmaya(kullanan, hibeIlKod, -1, sonuc, string.Empty, ENUMHibeTur.SULAMA, hibeKod, true))
            {
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulamaMalzeme tMalzeme = new TABHibeSulamaMalzeme(ve);
            ve.BeginTransaction();

            sonuc = tMalzeme.SilOzellik(hibeKod, tip, sulamaGrupKod, parselID);
            if (sonuc.islemSonuc)
            {
                sonuc = tMalzeme.Sil(hibeKod, tip, sulamaGrupKod, parselID);
                sonuc = tMalzeme.Sil(hibeKod, tip, 0, "");
            }

            if (sonuc.islemSonuc)
            {

                if (toplamHesapla)
                {
                    //************************TOPLAM TUTARLARI KAYIT ETMEK ICIN*****************************
                    sonuc = tMalzeme.Sil(hibeKod, tip, 0, "");

                    HibeSulamaMalzemeFiyat[] liste = Listele(ve, kullanan, hibeKod, tip, hibeIlKod, 0, "");

                    HibeSulamaMalzemeFiyat toplamMf = new HibeSulamaMalzemeFiyat();
                    toplamMf.hibeKod = hibeKod;
                    toplamMf.tip = tip;
                    foreach (HibeSulamaMalzemeFiyat mf in liste)
                    {
                        toplamMf.miktar += mf.miktar;
                        toplamMf.birimFiyat += mf.birimFiyat;
                        toplamMf.referansFiyat += mf.referansFiyat;
                        toplamMf.projeTutar += mf.projeTutar;
                        toplamMf.hibeEsasTutar += mf.hibeEsasTutar;
                    }

                    decimal projeToplam = toplamMf.projeTutar;
                    decimal hibeEsasToplam = toplamMf.hibeEsasTutar;

                    HibeSulamaDetay hsd = new HibeSulamaDetay();
                    hsd.hibeKod = hibeKod;

                    if (liste.Length > 0)
                    {
                        sonuc = tMalzeme.Kaydet(toplamMf);

                        if (sonuc.islemSonuc)
                        {
                            if (tip == ENUMHibeSulamaMalzemeTip.PROJE)
                            {
                                hsd.hibeEsasTutar = hibeEsasToplam;
                                hsd.hibeTutar = Math.Round(hibeEsasToplam / 2);
                                hsd.toplamTutar = projeToplam;
                                hsd.proformaTutar = projeToplam;
                                hsd.hibeEsasTutarProje = hibeEsasToplam;
                                hsd.proformaReferansTutar = hibeEsasToplam;
                            }
                            else
                            {
                                hsd.hibeEsasTutar = hibeEsasToplam;
                                hsd.hibeTutar = Math.Round(hibeEsasToplam / 2);
                                hsd.toplamTutar = projeToplam;
                                hsd.malzemeTutar = projeToplam;
                                hsd.gercekFatura = hibeEsasToplam;
                                hsd.hibeEsasTutarUygulama = hibeEsasToplam;
                            }

                        }
                    }
                    sonuc = new TRMHibeSulama().GuncelleDetay(kullanan, hsd, tip);
                    //************************TOPLAM TUTARLARI KAYIT ETMEK ICIN*****************************
                }

                ve.Commit();
                sonuc.bilgiStr = "Silme işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static HibeSulamaMalzemeFiyat[] Listele(VeriErisim ve, Kullanici kullanan, int hibeKod, ENUMHibeSulamaMalzemeTip tip, string hibeIlKod, int sulamaGrupKod, string parselNo)
        {
            if (!Yetki.YetkiVarMiOkumaya(kullanan, hibeIlKod, ENUMHibeTur.SULAMA))
                throw new Exception("Okuma yetkiniz bulunmamaktadır.");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulamaMalzeme tMalzeme = new TABHibeSulamaMalzeme(ve);
            IDataReader r = tMalzeme.Listele(hibeKod, tip, sulamaGrupKod, parselNo);
            if (!string.IsNullOrEmpty(tMalzeme.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tMalzeme.Hata());
            }

            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeSulamaMalzeme tMalzeme2 = new TABHibeSulamaMalzeme(ve2);

            ArrayList mfler = new ArrayList();

            int sonKolon = (int)TABHibeSulamaMalzeme.ENUMAlanlar.HIBEESASTUTAR;
            while (r.Read())
            {
                HibeSulamaMalzemeFiyat gelen = new HibeSulamaMalzemeFiyat();
                gelen.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.HIBEKOD, 0);
                gelen.tip = (ENUMHibeSulamaMalzemeTip)VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.TIP, 0);
                gelen.malzemeKod = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.MALZEMEKOD, 0);
                gelen.satirNo = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.SATIRNO, 0);
                gelen.sulamaGrupKod = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.SULAMAGRUPKOD, 0);
                gelen.parselID = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.PARSELID, "");
                gelen.miktar = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.MIKTAR, (decimal)0);
                gelen.birimFiyat = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.BIRIMFIYAT, (decimal)0);
                gelen.referansFiyat = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.REFERANSFIYAT, (decimal)0);
                gelen.projeTutar = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.PROJETUTAR, (decimal)0);
                gelen.hibeEsasTutar = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlar.HIBEESASTUTAR, (decimal)0);
                gelen.sulamaGrupAd = VeriErisim.DegerAl(r, sonKolon + 1, "");
                gelen.parselNo = VeriErisim.DegerAl(r, sonKolon + 2, "");

                gelen.ozellikler = ListeleOzellik(tMalzeme2, gelen.hibeKod, gelen.tip, gelen.malzemeKod, gelen.satirNo);

                mfler.Add(gelen);
            }
            r.Close();

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return (HibeSulamaMalzemeFiyat[])mfler.ToArray(typeof(HibeSulamaMalzemeFiyat));
        }


        public static HibeSulamaMalzemeFiyat[] ListeleSulamaGrupTutar(VeriErisim ve, Kullanici kullanan, int hibeKod, int tip)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeSulamaMalzeme tMalzeme = new TABHibeSulamaMalzeme(ve);
            IDataReader r = tMalzeme.ListeleSulamaGrupTutar(hibeKod, tip);
            if (!string.IsNullOrEmpty(tMalzeme.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tMalzeme.Hata());
            }

            //VeriErisim ve2 = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            //TABHibeSulamaMalzeme tMalzeme2 = new TABHibeSulamaMalzeme(ve2);

            ArrayList mfler = new ArrayList();
            while (r.Read())
            {
                HibeSulamaMalzemeFiyat gelen = new HibeSulamaMalzemeFiyat();
                gelen.hibeKod = VeriErisim.DegerAl(r, 0, 0);
                gelen.sulamaGrupKod = VeriErisim.DegerAl(r, 1, 0);
                gelen.projeTutar = VeriErisim.DegerAl(r, 2, (decimal)0);
                gelen.hibeEsasTutar = VeriErisim.DegerAl(r, 3, (decimal)0);
                gelen.sulamaGrupAd = VeriErisim.DegerAl(r, 4, "");
                gelen.parselAlan = VeriErisim.DegerAl(r, 5, 0.0);
                gelen.sulanacakAlan = VeriErisim.DegerAl(r, 6, 0.0);

                if (gelen.sulamaGrupKod == 0) gelen.sulamaGrupAd = "TOPLAM";
                if (gelen.sulamaGrupKod == 999) gelen.sulamaGrupAd = "Proje Yapım Bedeli";

                mfler.Add(gelen);
            }
            r.Close();

            double toplamSuAlan = 0.0;
            double toplamAlan = 0.0;

            foreach (HibeSulamaMalzemeFiyat gelen in mfler)
            {
                if (gelen.sulamaGrupKod == 0) continue;
                toplamSuAlan += gelen.sulanacakAlan;
                toplamAlan += gelen.parselAlan;
            }
            foreach (HibeSulamaMalzemeFiyat gelen in mfler)
            {
                if (gelen.sulamaGrupKod == 0)
                {
                    gelen.sulanacakAlan = toplamSuAlan;
                    gelen.parselAlan = toplamAlan;
                }
            }

            ve.VeriErisimKapat();
            //ve2.VeriErisimKapat();
            return (HibeSulamaMalzemeFiyat[])mfler.ToArray(typeof(HibeSulamaMalzemeFiyat));
        }

        private static Sonuc KaydetOzellik(TABHibeSulamaMalzeme tMalzeme, HibeSulamaMalzemeFiyat mf)
        {
            Sonuc sonuc = new Sonuc();
            if (mf.ozellikler == null || mf.ozellikler.Length == 0)
                return sonuc;

            foreach (HibeSulamaMalzemeOzellik ozellik in mf.ozellikler)
            {
                if (ozellik.ozellikKod <= 0 || string.IsNullOrEmpty(ozellik.deger) || OrtakFonksiyonlar.ConvertToDouble(ozellik.deger, (double)-1) == 0)
                    continue;

                ozellik.hibeKod = mf.hibeKod;
                ozellik.tip = mf.tip;
                ozellik.malzemeKod = mf.malzemeKod;
                ozellik.satirNo = mf.satirNo;
                ozellik.sulamaGrupKod = mf.sulamaGrupKod;
                ozellik.parselID = mf.parselID;
                sonuc = tMalzeme.KaydetOzellik(ozellik);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        private static HibeSulamaMalzemeOzellik[] ListeleOzellik(TABHibeSulamaMalzeme tMalzeme, int hibeKod, ENUMHibeSulamaMalzemeTip tip, int malzemeKod, int satir)
        {
            IDataReader r = tMalzeme.ListeleOzellik(hibeKod, tip, malzemeKod, satir);

            ArrayList al = new ArrayList();
            while (r.Read())
            {
                HibeSulamaMalzemeOzellik gelen = new HibeSulamaMalzemeOzellik();
                gelen.hibeKod = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlarMalzemeOzellik.HIBEKOD, 0);
                gelen.tip = (ENUMHibeSulamaMalzemeTip)VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlarMalzemeOzellik.TIP, 0);
                gelen.malzemeKod = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlarMalzemeOzellik.MALZEMEKOD, 0);
                gelen.satirNo = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlarMalzemeOzellik.SATIRNO, 0);
                gelen.ozellikKod = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlarMalzemeOzellik.OZELLIKKOD, 0);
                gelen.deger = VeriErisim.DegerAl(r, (int)TABHibeSulamaMalzeme.ENUMAlanlarMalzemeOzellik.DEGER, string.Empty);

                al.Add(gelen);
            }
            r.Close();

            return (HibeSulamaMalzemeOzellik[])al.ToArray(typeof(HibeSulamaMalzemeOzellik));
        }
    }
}
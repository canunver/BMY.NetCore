using System;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using TRMServisC.tr.gov.tarim.cezali;

namespace TNS.TRM.IsKurallari
{
    public abstract class TRMHibeYatirimAnket
    {
        public abstract Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int hibeKod, string anahtar, string ip);

        public abstract int PuanHesapla(AnketBilgi anketBilgi);

        public abstract bool GecerMi(AnketBilgi anketBilgi, string konuKod);

        public Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, int hibeKod, TABHibeYatirim.ENUMAlanlar anketTur, string anahtar, string ip)
        {
            string hata = string.Empty;
            Sonuc sonuc = new Sonuc(false, "", "");

            if (hibeKod <= 0)
                hata = "Başvuru kodu boş bırakılamaz.<br>";
            if ((int)anketTur < (int)TABHibeYatirim.ENUMAlanlar.ANKET1 || (int)anketTur > (int)TABHibeYatirim.ENUMAlanlar.ANKET4)
                hata += "Anket türü boş bırakılamaz.";
            if (!string.IsNullOrEmpty(hata))
                return new Sonuc(false, hata, string.Empty);

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, "AnketKaydet");
            TABHibeYatirim to = new TABHibeYatirim(ve);

            //Yetki kontrolü
            ObjectArray hibeler = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = hibeKod });
            if (!hibeler.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return hibeler.sonuc;
            }
            if (hibeler.objeler.Count == 0)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, "İlgili hibe başvurusu bulunamadı!", string.Empty);
            }

            //if (!Yetki.YetkiVarMiYazmaya(kullanan, ((HibeYatirim)hibeler.objeler[0]).ilKod, ((HibeYatirim)hibeler.objeler[0]).etapNo, string.Empty, ENUMHibeTur.EKONOMIKYATIRIM))
            //    return new Sonuc(false, "Yazma yetkiniz bulunmamaktadır.", string.Empty);
            //string basvuruNo = "";
            //ObjectArray list = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = hibeKod });

            HibeYatirim hm = new HibeYatirim();
            if (hibeler.ObjeSayisi > 0)
                hm = (HibeYatirim)hibeler.objeler[0];

            int anketTip = 0;
            if ((int)anketTur == (int)TABHibeYatirim.ENUMAlanlar.ANKET1)
                anketTip = 1;
            else if ((int)anketTur == (int)TABHibeYatirim.ENUMAlanlar.ANKET2)
                anketTip = 2;
            else if ((int)anketTur == (int)TABHibeYatirim.ENUMAlanlar.ANKET3)
                anketTip = 3;
            else if ((int)anketTur == (int)TABHibeYatirim.ENUMAlanlar.ANKET4)
                anketTip = 4;

            bool kontrolYap = true;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI))
                kontrolYap = false;

            if (anketTip != 4 && kontrolYap)
                hata = TRMHibeYatirimYonetim.HibeYatirimDegerlendirmeKapandiMi(hm.basvuruNo, hm.projeKonu);

            if (!string.IsNullOrEmpty(hata))
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, hata, "");
            }

            int basvuruCesidi = 0;
            if (hm.etapNo >= 14)
            {
                if (OrtakFonksiyonlar.ConvertToInt(hm.projeKonu, 0) < 51)
                    basvuruCesidi = 1;
                else
                    basvuruCesidi = 2;
            }

            AnketBilgi anket = AnketBilgiOku(kullanan, hibeKod, anketTur, anahtar);
            int anketDurum = GecerMi(anket, hm.projeKonu) ? 1 : -1;
            int anketPuan = PuanHesapla(anket);

            //string depremIller = "0105001,0106001,0106002,0107004,0106003,0105004,0105007,0106009,0107009,0105008,0106006";
            int depremIli = -1;// depremIller.IndexOf(hm.ilKod);
            bool kabulIslemiYapilmis = to.YatirimKabulYapilmismi(hm.etapNo, basvuruCesidi, depremIli);
            if (kabulIslemiYapilmis)
            {
                sonuc.hataStr = "Anket kayıtlarında değişiklik yapılamaz.";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }
            bool gecme = true;
            double eskiPuan = anketPuanGetir(kullanan, hibeKod, anketTip, hibeler);
            if (anketTip == 3 || anketTip == 4)
            {
                if (anketPuan == eskiPuan)
                    gecme = false;
            }
            else if (anketTip == 1 || anketTip == 2)
            {
                if (anketDurum == eskiPuan)
                    gecme = false;
            }
            if (!gecme)
            {
                sonuc.hataStr = "Anketde değişiklik yapılmadığı için yatırım tablosundaki puan alanları güncellenmedi.";
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            sonuc = to.KaydetAnketSonuc(hibeKod, anketTur, anketPuan, anketDurum, basvuruCesidi);
            if (sonuc.islemSonuc)
            {
                HibeYatirimTarihce tarihce = new HibeYatirimTarihce();
                tarihce.hibeKod = hibeKod;
                tarihce.tarih = new TNSDateTime(DateTime.Now);
                tarihce.kullaniciKod = kullanan.mernis == "" ? kullanan.kullaniciKodu : kullanan.mernis;
                tarihce.kullaniciAdi = kullanan.ADSOYAD;
                tarihce.durum = 100;//Değerlendirme kodu olsun.
                tarihce.aciklama = "Tablo " + ((int)anketTur - (int)TABHibeYatirim.ENUMAlanlar.ANKET1 + 1);
                tarihce.ipAdresi = ip;

                sonuc = to.KaydetTarihce(tarihce);
            }

            if (sonuc.islemSonuc)
            {
                sonuc.kayitSay = anketDurum * 1000 + anketPuan;
                sonuc.bilgiStr = "İşlem başarıyla tamamlandı.";
                ve.Commit();

                if (anketTur == TABHibeYatirim.ENUMAlanlar.ANKET3 || anketTur == TABHibeYatirim.ENUMAlanlar.ANKET4)
                {
                    sonuc.anahtar = GenelPuanGetir(ve, kullanan, hibeKod).ToString();
                }
            }
            else
            {
                sonuc.hataStr = "İşlem sırasında hata oluştu.<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static AnketBilgi AnketBilgiOku(Kullanici kullanan, int hibeKod, TABHibeYatirim.ENUMAlanlar anketTur, string anahtar)
        {
            try
            {
                AnketBilgi anketKistasBilgi = new AnketBilgi();
                anketKistasBilgi.modul = Arac.anketModul;
                anketKistasBilgi.anketAnahtar = anahtar;
                anketKistasBilgi.orgBirimKod = 1;
                anketKistasBilgi.cevapAnahtar = hibeKod.ToString();

                AnketBilgi anketBilgi = TNS.KYM.Arac.Tanimla().AnketBilgiOku(kullanan, anketKistasBilgi, true, true);
                if (anketBilgi == null)
                    throw new Exception("Kayıt bulunamadı!");
                return anketBilgi;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static double GenelPuanGetir(VeriErisim ve, Kullanici kullanan, int hibeKod)
        {
            ObjectArray hibeBilgi = new TRMHibeYatirim().Listele(ve, kullanan, new HibeYatirim() { kod = hibeKod });
            if (hibeBilgi.sonuc.islemSonuc && hibeBilgi.objeler.Count > 0)
                return ((HibeYatirim)hibeBilgi.objeler[0]).puan;
            return 0;
        }

        public static double anketPuanGetir(Kullanici kullanan, int hibeKod, int anketTur, ObjectArray hibeBilgi)
        {
            //ObjectArray hibeBilgi = new TRMHibeYatirim().Listele(null, kullanan, new HibeYatirim() { kod = hibeKod });
            if (hibeBilgi.sonuc.islemSonuc && hibeBilgi.objeler.Count > 0)
            {
                if (anketTur == 1)
                    return ((HibeYatirim)hibeBilgi.objeler[0]).anket1;
                else if (anketTur == 2)
                    return ((HibeYatirim)hibeBilgi.objeler[0]).anket2;
                else if (anketTur == 3)
                    return ((HibeYatirim)hibeBilgi.objeler[0]).anket3Puan;
                else if (anketTur == 4)
                    return ((HibeYatirim)hibeBilgi.objeler[0]).anket4Puan;
            }
            return 0;
        }
    }

    public class TRMHibeYatirimAnket1 : TRMHibeYatirimAnket
    {
        public override Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int hibeKod, string anahtar, string ip)
        {
            return Kaydet(veriErisim, kullanan, hibeKod, TABHibeYatirim.ENUMAlanlar.ANKET1, anahtar, ip);
        }

        public override int PuanHesapla(AnketBilgi anketBilgi)
        {
            if (anketBilgi == null || anketBilgi.sorular == null || anketBilgi.sorular.Length == 0)
                return 0;

            int dogruCevapSay = 0;
            foreach (AnketSoru soru in anketBilgi.sorular)
            {
                if (soru.CevapBul(0) != null || soru.CevapBul(2) != null)
                    dogruCevapSay++;
            }
            if (dogruCevapSay == anketBilgi.sorular.Length)
                return 1;
            else
                return 0;
        }

        public override bool GecerMi(AnketBilgi anketBilgi, string konuKod)
        {
            return PuanHesapla(anketBilgi) == 1;
        }
    }

    public class TRMHibeYatirimAnket2 : TRMHibeYatirimAnket
    {
        public override Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int hibeKod, string anahtar, string ip)
        {
            return Kaydet(veriErisim, kullanan, hibeKod, TABHibeYatirim.ENUMAlanlar.ANKET2, anahtar, ip);
        }

        public override int PuanHesapla(AnketBilgi anketBilgi)
        {
            if (anketBilgi == null || anketBilgi.sorular == null || anketBilgi.sorular.Length == 0)
                return 0;

            int dogruCevapSay = 0;
            foreach (AnketSoru soru in anketBilgi.sorular)
            {
                if (soru.CevapBul(0) != null || soru.CevapBul(2) != null)
                    dogruCevapSay++;
            }
            if (dogruCevapSay == anketBilgi.sorular.Length)
                return 1;
            else
                return 0;
        }

        public override bool GecerMi(AnketBilgi anketBilgi, string konuKod)
        {
            return PuanHesapla(anketBilgi) == 1;
        }
    }

    public class TRMHibeYatirimAnket3 : TRMHibeYatirimAnket
    {
        public override Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int hibeKod, string anahtar, string ip)
        {
            Sonuc sonuc = Kaydet(veriErisim, kullanan, hibeKod, TABHibeYatirim.ENUMAlanlar.ANKET3, anahtar, ip);
            //sonuc.anahtar = GenelPuanGetir(kullanan, hibeKod).ToString();
            return sonuc;
        }

        public override int PuanHesapla(AnketBilgi anketBilgi)
        {
            if (anketBilgi == null || anketBilgi.sorular == null || anketBilgi.sorular.Length == 0)
                return 0;

            int puan = 0;
            foreach (AnketSoru soru in anketBilgi.sorular)
            {
                for (int i = 0; i < soru.CevapSay(); i++)
                {
                    if (soru.Cevap(i) != null)
                        puan += OrtakFonksiyonlar.ConvertToInt(soru.Cevap(i).cevapYazi, 0);
                }
            }
            return puan;
        }

        public override bool GecerMi(AnketBilgi anketBilgi, string konuKod)
        {
            return PuanHesapla(anketBilgi) >= 0;//sadece yatırımlar yapılıyor
        }
    }

    public class TRMHibeYatirimAnket4 : TRMHibeYatirimAnket
    {
        public override Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, int hibeKod, string anahtar, string ip)
        {
            Sonuc sonuc = Kaydet(veriErisim, kullanan, hibeKod, TABHibeYatirim.ENUMAlanlar.ANKET4, anahtar, ip);
            //sonuc.anahtar = GenelPuanGetir(kullanan, hibeKod).ToString();
            return sonuc;
        }

        public override int PuanHesapla(AnketBilgi anketBilgi)
        {
            if (anketBilgi == null || anketBilgi.sorular == null || anketBilgi.sorular.Length == 0)
                return 0;

            int puan = 0;
            foreach (AnketSoru soru in anketBilgi.sorular)
            {
                for (int i = 0; i < soru.CevapSay(); i++)
                {
                    if (soru.Cevap(i) != null)
                        puan += OrtakFonksiyonlar.ConvertToInt(soru.Cevap(i).cevapYazi, 0);
                }
            }
            return puan;
        }

        public override bool GecerMi(AnketBilgi anketBilgi, string konuKod)
        {
            //Ekonomikte 65
            //Alt yatırımda 50

            int sinir = 0;
            if (OrtakFonksiyonlar.ConvertToInt(konuKod, 0) > 51)
                sinir = 50;

            sinir = 0;//17 (15.Etap) dan sonra sınır kaldırıldı
            return PuanHesapla(anketBilgi) >= sinir;

            //if (anketBilgi == null || anketBilgi.sorular == null || anketBilgi.sorular.Length == 0)
            //    return false;

            //int genelPuan = 0;
            //int cnt = 0;
            //foreach (AnketSoru soru in anketBilgi.sorular)
            //{
            //    int soruPuan = 0;
            //    for (int i = 0; i < soru.CevapSay(); i++)
            //    {
            //        if (soru.Cevap(i) != null)
            //            soruPuan += OrtakFonksiyonlar.ConvertToInt(soru.Cevap(i).cevapYazi, 0);
            //    }
            //    if (soruPuan < soru.soruSeviye)
            //        return false;
            //    if (cnt == 0 && soruPuan < 12)
            //        return false;
            //    if (cnt == 1 && soruPuan < 18)
            //        return false;
            //    genelPuan += soruPuan;
            //    cnt++;
            //}
            //return genelPuan >= 65;
        }
    }
}
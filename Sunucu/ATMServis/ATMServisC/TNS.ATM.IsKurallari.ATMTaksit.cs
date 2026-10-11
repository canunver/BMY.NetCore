using System;
using System.Data;
using TNS.ATM.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.ATM.IsKurallari
{
    /// <summary>
    /// Alacak Takip Dosya Taksit Sınıfı
    /// </summary>
    public class DosyaTaksitClass
    {
        /// <summary>
        /// Kullanıcı ve ödeme planına göre Ödeme planı hazırlayan fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="odemePlani">Ödeme planı bilgisi, OdemePlani tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc OdemePlaniHazirla(VeriErisim veriErisim, Kullanici kullanan, OdemePlani odemePlani)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);

            if (odemePlani.taksitSayisi <= 0)
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA037);
            }

            if (odemePlani.periyod <= 0)
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA038);
            }

            if (TNSDateTime.IsNull(odemePlani.ilkOdemeTarihi))
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA039);
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));

            Dosya okunanDosya = DosyaClass.Oku(ve, kullanan, odemePlani.yil, odemePlani.dosyaNo);

            if (okunanDosya == null)
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA040);
            }
            else
            {
                if (TNSDateTime.IsNull(okunanDosya.taksitBaslangicTarihi))
                {
                    donusDeger.DegerAta("Alacak dosyasına Taksitlendirme Tarihi Girilmemiş!");
                }
                else
                {
                    if (odemePlani.ilkOdemeTarihi.Oku() < okunanDosya.taksitBaslangicTarihi.Oku())
                    {
                        donusDeger.DegerAta("İlk ödeme tarihi taksitlendirme tarihinden sonra olmalıdır!");
                    }
                    foreach (DosyaTaksit item in okunanDosya.taksitBilgi.objeler)
                    {
                        if (item.ODEMETARIH >= okunanDosya.taksitBaslangicTarihi.Oku() || item.GERCEKODEMETARIH >= okunanDosya.taksitBaslangicTarihi.Oku())
                        {
                            donusDeger.DegerAta("Girilmiş olan taksit tarihinden sonra kayıtlı işlem bulunduğundan ödeme planı hazırlanamaz, elle satır açıp planı oluşturabilirsiniz!");
                            break;
                        }
                    }
                }
            }

            if (!donusDeger.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }


            int sonDurum = okunanDosya.sonDurum.kod;

            ve.BeginTransaction();
            try
            {
                donusDeger = TaksitEkleIc(ve, okunanDosya, odemePlani, okunanDosya.faizBaslangicTarihi, okunanDosya.taksitBaslangicTarihi, okunanDosya.faizTur);

                if (donusDeger.islemSonuc && sonDurum != (int)ENUMDurum.Taksitlendirildi && !TNSDateTime.IsNull(okunanDosya.taksitBaslangicTarihi))
                {
                    DosyaDurumTarihceClass durumTarihce = new DosyaDurumTarihceClass();
                    DosyaDurumTarihce durum = new DosyaDurumTarihce();
                    durum.yil = odemePlani.yil;
                    durum.dosyaNo = odemePlani.dosyaNo;
                    durum.durum.kod = (int)ENUMDurum.Taksitlendirildi;
                    durum.tarih = new TNSDateTime(DateTime.Today.Date.ToString());
                    durum.kullaniciKodu = kullanan.kullaniciKodu;
                    TNSCollection tc = new TNSCollection();
                    tc.Add(durum);
                    donusDeger = durumTarihce.Kaydet(ve, kullanan, tc);
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Kullanıcı ve Ödeme planı bilgilerine göre taksit ekleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="odemePlani">Ödeme planı bilgisi, OdemePlani tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc TaksitEkle(VeriErisim veriErisim, Kullanici kullanan, OdemePlani odemePlani)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            Dosya okunanDosya = DosyaClass.Oku(ve, kullanan, odemePlani.yil, odemePlani.dosyaNo);

            if (okunanDosya == null)
            {
                donusDeger.hataStr = AlacakTakipResI.ATA043;
                donusDeger.islemSonuc = false;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            ve.BeginTransaction();
            try
            {
                TABATMDosyaTaksit tabTaksit = new TABATMDosyaTaksit(ve);


                DosyaTaksit taksit = new DosyaTaksit();
                taksit.yil = odemePlani.yil;
                taksit.dosyaNo = odemePlani.dosyaNo;
                taksit.siraNo = okunanDosya.OdemeSiraBul();
                taksit.sonOdemeTarihi.Yaz(DateTime.Now.Date);

                taksit.miktar = 0;
                taksit.faizMiktar = 0;
                taksit.taksitTarihiFaizi = 0;

                donusDeger = tabTaksit.SatirEkle(taksit);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Ödeme planı ve Taksit ekle bilgisine göre Taksit ekleyen fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="odemePlani">Ödeme planı bilgisi, OdemePlani tipinde sınıf</param>
        /// <param name="taksitEkle">Taksit Ekle bilgisi, Bool tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        private static Sonuc TaksitEkleIc(VeriErisim ve, Dosya okunan, OdemePlani odemePlani, TNSDateTime faizBaslangicTarihi, TNSDateTime taksitBaslangicTarihi, FaizTur fTur)
        {
            Sonuc donusDeger = new Sonuc();

            TABATMDosyaTaksit tabTaksit = new TABATMDosyaTaksit(ve);

            try
            {
                double borc = TNS.ATM.Arac.BorcBul(okunan, TNS.MUH.Arac.VarsayilanKurumKod());
                borc = borc - okunan.Odenen;

                DosyaTaksit taksit = new DosyaTaksit();
                taksit.odemeTarihi = new TNSDateTime(okunan.taksitBaslangicTarihi.Oku().AddDays(-1));
                taksit.miktar = borc;
                taksit.FaizleriHesapla(okunan);
                double borcFaizi = taksit.faizMiktar;

                double taksitBorc = OrtakFonksiyonlar.KurusYap(borc / odemePlani.taksitSayisi);
                double taksitBorcFaizi = OrtakFonksiyonlar.KurusYap(borcFaizi / odemePlani.taksitSayisi);

                int sonNo = okunan.OdemeSiraBul();
                for (int i = 0; i < odemePlani.taksitSayisi; i++)
                {
                    taksit = new DosyaTaksit();
                    taksit.yil = odemePlani.yil;
                    taksit.dosyaNo = odemePlani.dosyaNo;
                    taksit.siraNo = sonNo + i;
                    taksit.sonOdemeTarihi.Yaz(odemePlani.ilkOdemeTarihi.Oku().AddMonths(i * odemePlani.periyod));

                    if (i < odemePlani.taksitSayisi - 1)
                    {
                        taksit.miktar = taksitBorc;
                        taksit.taksitTarihiFaizi = taksitBorcFaizi;
                    }
                    else
                    {
                        taksit.miktar = OrtakFonksiyonlar.KurusYap(borc - taksitBorc * i);
                        taksit.taksitTarihiFaizi = OrtakFonksiyonlar.KurusYap(borcFaizi - taksitBorcFaizi * i);
                    }

                    taksit.odemeTarihi = new TNSDateTime(taksit.sonOdemeTarihi.Oku());
                    taksit.FaizleriHesapla(okunan);
                    taksit.odemeTarihi = null;

                    donusDeger = tabTaksit.SatirEkle(taksit);
                    if (!donusDeger.islemSonuc)
                        break;
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            return donusDeger;
        }

        /// <summary>
        /// Taksitlerin ödenme durumunu dönen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="taksit">Taksit bilgisi, TNSCollection tipinde sınıf</param>
        /// <returns>Sonuc tipinde sınıf döndürür</returns>
        public static Sonuc Odendi(VeriErisim veriErisim, Kullanici kullanan, Dosya dosya)
        {
            Sonuc donusDeger = Yetki.YazmaHakkiVarMi(kullanan);
            if (!donusDeger.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDosyaTaksit tabTaksit = new TABATMDosyaTaksit(ve);

            Dosya okunan = DosyaClass.Oku(ve, kullanan, dosya.yil, dosya.dosyaNo);

            if (okunan == null)
            {
                donusDeger.DegerAta(AlacakTakipResI.ATA044);
            }
            else
            {
                foreach (DosyaTaksit t in okunan.taksitBilgi.objeler)
                {
                    DosyaTaksit gelenTaksit = dosya.TaksitBul(t.siraNo);
                    if (gelenTaksit == null)
                        donusDeger.DegerAta("Gelen taksit veri tabanında bulunamadı!");
                    else
                    {
                        //if (gelenTaksit.miktar <= 0 || gelenTaksit.sonOdemeTarihi.isNull || gelenTaksit.faizMiktar < 0 || gelenTaksit.odemeTarihi == null || gelenTaksit.odemeTarihi.isNull)
                        if (gelenTaksit.miktar < 0)
                        {
                            donusDeger.DegerAta("Negatif miktar olamaz!");
                        }
                        else if (gelenTaksit.miktar == 0 && (!string.IsNullOrWhiteSpace(t.odemeEmriNo) || !string.IsNullOrWhiteSpace(t.faizOdemeEmriNo)))
                        {
                            donusDeger.DegerAta("Ödeme emri ile ilişkilendirilmiş bir satır silinemez.");
                        }
                        else
                        {
                            if (t.FarkliMi(gelenTaksit))
                            {
                                t.sonOdemeTarihi = gelenTaksit.sonOdemeTarihi;
                                t.miktar = gelenTaksit.miktar;
                                t.odemeTarihi = gelenTaksit.odemeTarihi;
                                t.makbuzNo = gelenTaksit.makbuzNo;
                                t.taksitTarihiFaizi = gelenTaksit.taksitTarihiFaizi;

                                t.FaizleriHesapla(okunan);
                            }
                        }
                    }
                }
            }

            ve.BeginTransaction();

            try
            {
                foreach (DosyaTaksit t in okunan.taksitBilgi.objeler)
                {
                    //if (!Genel.TaksitOdendiMi(ve, t.yil, t.dosyaNo, t.siraNo - 1) && t.siraNo > 1)
                    //{
                    //    //donusDeger.hataStr = AlacakTakipResI.ATA045 + " " + t.siraNo.ToString() + AlacakTakipResI.ATA046;
                    //    //donusDeger.islemSonuc = false;
                    //    //break;
                    //    continue;
                    //}

                    if (t.miktar == 0)
                        donusDeger = tabTaksit.Sil(dosya, t);
                    else
                        donusDeger = tabTaksit.Odendi(dosya, t);
                    if (!donusDeger.islemSonuc)
                        break;
                }

                if (donusDeger.islemSonuc)
                {
                    ve.Commit();

                    //Ödeme kaydı yapılırken neden yeniden ödeme planı hazırlanıyor???? Melih 12.06.2014 
                    //DosyaTaksit dt = (DosyaTaksit)taksit[0];
                    //OdemePlani odemePlani = new OdemePlani();
                    //odemePlani.yil = dt.yil;
                    //odemePlani.dosyaNo = dt.dosyaNo;
                    //donusDeger = TaksitEkle(ve, odemePlani, true);
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Yıl ve Dosya numarası bilgilerine göre Alacak takip dosya taksitlerini listeleyen fonksiyon
        /// </summary>
        /// <param name="veriErisim">Veri Erişim bilgisi, VeriErisim tipinde sınıf</param>
        /// <param name="kullanan">Kullanıcı bilgisi, Kullanici tipinde sınıf</param>
        /// <param name="yil">Yıl bilgisi, Int tipinde sınıf</param>
        /// <param name="dosyaNo">Dosya numarası bilgisi, Int tipinde sınıf</param>
        /// <returns>ObjectArray tipinde sınıf döndürür</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, int dosyaNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABATMDosyaTaksit tabTaksit = new TABATMDosyaTaksit(ve);
            IDataReader dr = tabTaksit.Listele(yil, dosyaNo);

            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                DosyaTaksit gelen = TaksitOku(dr, 0);
                donusDeger.objeler.Add(gelen);
            }

            if (donusDeger.objeler.Count == 0)
                donusDeger.sonuc.bilgiStr = AlacakTakipResI.ATA048;

            donusDeger.sonuc.islemSonuc = true;

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        public static DosyaTaksit TaksitOku(IDataReader dr, int sutunBasla)
        {
            DosyaTaksit gelen = new DosyaTaksit();
            gelen.yil = VeriErisim.DegerAl(dr, sutunBasla + 0, -1);
            gelen.dosyaNo = VeriErisim.DegerAl(dr, sutunBasla + 1, -1);
            gelen.siraNo = VeriErisim.DegerAl(dr, sutunBasla + 2, -1);
            gelen.sonOdemeTarihi = VeriErisim.DegerAl(dr, sutunBasla + 3);
            gelen.miktar = VeriErisim.DegerAl(dr, sutunBasla + 4, 0.0);
            gelen.odemeTarihi = VeriErisim.DegerAl(dr, sutunBasla + 5);
            gelen.faizMiktar = VeriErisim.DegerAl(dr, sutunBasla + 6, 0.0);
            gelen.taksitTarihiFaizi = VeriErisim.DegerAl(dr, sutunBasla + 7, 0.0);
            gelen.makbuzNo = VeriErisim.DegerAl(dr, sutunBasla + 8, "");
            gelen.odemeEmriNo = VeriErisim.DegerAl(dr, sutunBasla + 9, "");
            gelen.faizOdemeEmriNo = VeriErisim.DegerAl(dr, sutunBasla + 10, "");
            return gelen;
        }

        /// <summary>
        /// Odemes the emri kaydet taksit.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="yil">The yil.</param>
        /// <param name="dosyaNo">The dosya no.</param>
        /// <param name="siraNo">The sira no.</param>
        /// <param name="belgeYil">The belge yil.</param>
        /// <param name="belgeNo">The belge no.</param>
        /// <param name="faizMi">if set to <c>true</c> [faiz mi].</param>
        /// <returns></returns>
        public static Sonuc OdemeEmriKaydetTaksit(VeriErisim veriErisim, int yil, int dosyaNo, int siraNo, int belgeYil, string belgeNo, bool faizMi)
        {
            Sonuc donusDeger = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriATM", TNS.KYM.Arac.KimlikBul(null));
            TABATMDosyaTaksit tabAlacakTakip = new TABATMDosyaTaksit(ve);
            ve.BeginTransaction();

            try
            {
                donusDeger = tabAlacakTakip.OdemeEmriKaydetTaksit(yil, dosyaNo, siraNo, belgeYil, belgeNo, faizMi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.Message;
            }

            if (donusDeger.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return donusDeger;
        }
    }
}
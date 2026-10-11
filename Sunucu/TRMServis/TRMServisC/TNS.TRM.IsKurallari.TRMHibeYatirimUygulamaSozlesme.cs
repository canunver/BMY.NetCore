using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;
using System.Collections;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeYatirimUygulamaSozlesme
    {
        public static Sonuc Kaydet(ref VeriErisim ve, Kullanici kullanan, HibeYatirimUygulamaSozlesme us)
        {
            Sonuc sonuc = new Sonuc();
            if (!TNS.TRM.Yetki.YetkiVarMiYazmayaKisa(kullanan))
                sonuc.hataStr = "Yetkiniz bulunmamaktadır!<br>";

            if (us.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (us.hukukiStatu <= 0)
                sonuc.hataStr += "Hukuki statü boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(us.kimlikNo))
                sonuc.hataStr += "Kimlik no boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(us.yukleniciAd))
                sonuc.hataStr += "Yüklenici adı boş bırakılamaz.<br>";
            if ((int)us.sozlesmeTur <= 0)
                sonuc.hataStr += "Sözleşme türü boş bırakılamaz.<br>";
            if (us.sozlesmeTarih == null || us.sozlesmeTarih.isNull)
                sonuc.hataStr += "Sözleşme tarihi boş bırakılamaz.<br>";
            else if (us.sozlesmeTarih.Oku() > new TNSDateTime(DateTime.Now).Oku())
                sonuc.hataStr += "Sözleşme tarihi bugünden sonra olamaz.<br>";
            if (us.sozlesmeTutar <= 0)
                sonuc.hataStr += "Sözleşme tutarı boş bırakılamaz.<br>";
            if (us.hukukiStatu == (int)ENUMHibeYatirimHukukiStatu.GERCEKKISI && !string.IsNullOrEmpty(us.kimlikNo) && us.kimlikNo.Length != 11)
                sonuc.hataStr += "Kimlik no 11 karakter olmalıdır.<br>";
            if (us.hukukiStatu == (int)ENUMHibeYatirimHukukiStatu.TUZELKISI && !string.IsNullOrEmpty(us.kimlikNo) && us.kimlikNo.Length != 10)
                sonuc.hataStr += "Kimlik no 10 karakter olmalıdır.<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            sonuc = SozlesmeyeUygunMu(ve, kullanan, us);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }
            if (us.sozlesmeKod > 0)
            {
                if (us.sozlesmeDurum == ENUMHibeYatirimUygulamaSozlesmeDurum.HATALI && new TABHibeYatirimHakedis(ve).SayacDegerAl(us.hibeKod, us.sozlesmeKod) > 1)
                {
                    ve.VeriErisimKapat();
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Durumunu " + HibeYatirimUygulamaSozlesme.SozlesmeDurumAdBul(ENUMHibeYatirimUygulamaSozlesmeDurum.HATALI) + " yapmaya çalıştığınız uygulama sözleşmesine ait hakedişler var. Bu yüzden bu işlemi yapamazsınız.";
                    return sonuc;
                }
                sonuc = HakediseUygunMu(ve, us);
                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            TABHibeYatirimUygulamaSozlesme tabUS = new TABHibeYatirimUygulamaSozlesme(ve);
            ve.BeginTransaction();

            if (us.sozlesmeKod <= 0)
            {
                us.sozlesmeDurum = ENUMHibeYatirimUygulamaSozlesmeDurum.DEVAMEDIYOR;
                us.sozlesmeKod = tabUS.SayacDegerAl(us.hibeKod);
                sonuc = tabUS.Kaydet(us);
                if (sonuc.islemSonuc)
                {
                    int count = tabUS.UygulamaSozlesmeAdedi(us.hibeKod);
                    if (count >= 1)
                    {
                        TABHibeYatirim ty = new TABHibeYatirim(ve);
                        HibeYatirim y = new HibeYatirim();
                        y.kod = us.hibeKod;
                        y.durum = (int)ENUMHibeYatirimBasvuruDurum.UYGULAMASOZLESMESIYAPILDI;
                        sonuc = ty.GuncelleDurum(y);
                        sonuc.anahtar = us.sozlesmeKod.ToString();
                    }
                }
            }
            else
                sonuc = tabUS.Guncelle(us);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(ref VeriErisim ve, Kullanici kullanan, HibeYatirimUygulamaSozlesme us)
        {
            Sonuc sonuc = new Sonuc();
            if (us.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (us.sozlesmeKod <= 0)
                sonuc.hataStr += "Sözleşme kodu boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUMERKEZ) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DESTEKBASVURUYONETICI)))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = "Uygulama Sözleşmeyi silme yetkiniz yok. Merkez tarafından silinebilir";
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            HibeYatirimHakedis hk = new HibeYatirimHakedis();
            hk.hibeKod = us.hibeKod;
            hk.sozlesmeKod = us.sozlesmeKod;
            HibeYatirimHakedis[] hklar = TRMHibeYatirimHakedis.Listele(ref ve, kullanan, hk);
            if (hklar.Length > 0)
            {
                sonuc.hataStr = "Bu uygulama sözleşmesine ait hakediş kaydı bulunmaktadır. Bu nedenle silme işlemi yapılamaz";
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeYatirimUygulamaSozlesme tabUS = new TABHibeYatirimUygulamaSozlesme(ve);
            ve.BeginTransaction();
            sonuc = tabUS.Sil(us);

            if (sonuc.islemSonuc)
            {
                int count = tabUS.UygulamaSozlesmeAdedi(us.hibeKod);
                if (count == 0)
                {
                    TABHibeYatirim ty = new TABHibeYatirim(ve);
                    HibeYatirim y = new HibeYatirim();
                    y.kod = us.hibeKod;
                    y.durum = (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI;
                    sonuc = ty.GuncelleDurum(y);
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Uygulama sözleşmesi başarıyla silindi.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc EskiEtapUygSozlesmeKaydet(ref VeriErisim ve, Kullanici kullanan, HibeYatirimUygulamaSozlesme[] uygSozlesme)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirimUygulamaSozlesme tabUS = new TABHibeYatirimUygulamaSozlesme(ve);
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            ve.BeginTransaction();
            Sonuc sonuc = new Sonuc();

            sonuc = tabHakedis.SilHepsi(new HibeYatirimHakedis { hibeKod = uygSozlesme[0].hibeKod });
            if (sonuc.islemSonuc)
                sonuc = tabUS.SilHepsi(new HibeYatirimUygulamaSozlesme { hibeKod = uygSozlesme[0].hibeKod });

            if (sonuc.islemSonuc)
            {
                foreach (HibeYatirimUygulamaSozlesme us in uygSozlesme)
                {
                    if (us != null && sonuc.islemSonuc)
                    {
                        if (us.hibeKod <= 0)
                            sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
                        if (string.IsNullOrEmpty(us.kimlikNo))
                            sonuc.hataStr += "Kimlik no boş bırakılamaz.<br>";
                        if ((int)us.sozlesmeTur <= 0)
                            sonuc.hataStr += "Sözleşme türü boş bırakılamaz.<br>";
                        if (us.sozlesmeTutar <= 0)
                            sonuc.hataStr += "Sözleşme tutarı boş bırakılamaz.<br>";

                        if (!string.IsNullOrEmpty(sonuc.hataStr))
                        {
                            sonuc.islemSonuc = false;
                            return sonuc;
                        }

                        ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
                        sonuc = SozlesmeyeUygunMu(ve, kullanan, us);
                        if (!sonuc.islemSonuc)
                        {
                            ve.VeriErisimKapat();
                            return sonuc;
                        }

                        us.sozlesmeKod = tabUS.SayacDegerAl(us.hibeKod);
                        sonuc = tabUS.Kaydet(us);
                        if (sonuc.islemSonuc)
                        {
                            if (us.hakedisBilgileri[0] != null)
                            {
                                us.hakedisBilgileri[0].sozlesmeKod = us.sozlesmeKod;
                                us.hakedisBilgileri[0].hakedisKod = new TABGenel(ve).BelgeNoAlInt("HIBEYATIRIMHAKEDIS", "000000000");
                                sonuc = tabHakedis.Kaydet(us.hakedisBilgileri[0]);
                            }
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc SozlesmeyeUygunMu(VeriErisim ve, Kullanici kullanan, HibeYatirimUygulamaSozlesme us)
        {
            try
            {
                HibeYatirim hy = TRMHibeYatirim.ListeleSozlesme(ref ve, kullanan, us.hibeKod);
                if (hy.maliyetBilgileri != null && hy.maliyetBilgileri.Length > 0)
                {
                    TABHibeYatirimUygulamaSozlesme tabUS = new TABHibeYatirimUygulamaSozlesme(ve);
                    TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
                    decimal sozlesmeTutar = tabUS.SozlesmeTutarToplamAl(us.hibeKod, us.sozlesmeKod, false, us.sozlesmeTur);
                    sozlesmeTutar += tabHakedis.FaturaTutarToplamAlUygSozIptalOlanlar(us.hibeKod, us.sozlesmeKod, us.sozlesmeTur);
                    decimal fark = 0;

                    if (us.sozlesmeTur == ENUMHibeYatirimUygulamaSozlesmeTur.INSAAT)
                        fark = hy.maliyetBilgileri[0].hibeEsasTutarInsaat - sozlesmeTutar;
                    else if (us.sozlesmeTur == ENUMHibeYatirimUygulamaSozlesmeTur.MALALIMI)
                        fark = hy.maliyetBilgileri[0].hibeEsasTutarMal - sozlesmeTutar;
                    else if (us.sozlesmeTur == ENUMHibeYatirimUygulamaSozlesmeTur.ANAHTARTESLIM)
                        fark = (hy.maliyetBilgileri[0].hibeEsasTutarMal + hy.maliyetBilgileri[0].hibeEsasTutarInsaat) - sozlesmeTutar;

                    if (hy.maliyetBilgileri[0].hibeEsasTutarMal + hy.maliyetBilgileri[0].hibeEsasTutarInsaat == 0)
                        return new Sonuc(false, "Sözleşmesine kaydı yapılmamış", string.Empty);

                    if (us.sozlesmeTutar > fark && hy.altYapiGrup != "A")//A grubu ise istenilen tutar girilebilir
                        return new Sonuc(false, "Uygulama sözleşmesinin tutarı, uygulama sözleşmesine bağlanmamış tutar olan " + fark.ToString("N2") + " TL'den büyük olamaz..", string.Empty);

                    if (hy.altYapiGrup == "A" && us.sozlesmeKod == 0)
                    {
                        HibeYatirimUygulamaSozlesme uy = new HibeYatirimUygulamaSozlesme();
                        uy.hibeKod = us.hibeKod;
                        HibeYatirimUygulamaSozlesme[] hyu = Listele(ref ve, kullanan, uy, false);
                        if (hyu != null && hyu.Length > 0)
                        {
                            return new Sonuc(false, "Alt yapı için sadece bir adet uygulama sözleşmesi kayıt edilebilir", string.Empty);
                        }
                    }

                    return new Sonuc();
                }
                else
                    return new Sonuc(false, "Sözleşme bilgileri bulunamadı.", string.Empty);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), string.Empty);
            }
        }

        private static Sonuc HakediseUygunMu(VeriErisim ve, HibeYatirimUygulamaSozlesme us)
        {
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            decimal hakedisTutar = tabHakedis.FaturaTutarToplamAl(us.hibeKod, us.sozlesmeKod, 0, false);
            if (us.sozlesmeTutar < hakedisTutar)
                return new Sonuc(false, "Uygulama sözleşmesinin tutarı, şu ana kadar kayıt edilmiş olan hakediş tutarlarının toplamı olan " + hakedisTutar.ToString("N2") + " TL'den küçük olamaz.", string.Empty);
            return new Sonuc();
        }

        public static HibeYatirimUygulamaSozlesme[] Listele(ref VeriErisim ve, Kullanici kullanan, HibeYatirimUygulamaSozlesme us, bool iptalVeHataliHaric)
        {
            if (us.hibeKod <= 0)
                throw new Exception("Hibe kodu boş bırakılamaz.");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirimUygulamaSozlesme tabUS = new TABHibeYatirimUygulamaSozlesme(ve);
            IDataReader dr = tabUS.Listele(us, iptalVeHataliHaric);
            if (!string.IsNullOrEmpty(tabUS.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tabUS.Hata());
            }

            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                HibeYatirimUygulamaSozlesme gelen = new HibeYatirimUygulamaSozlesme();
                gelen.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.HIBEKOD, 0);
                gelen.sozlesmeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMEKOD, 0);
                gelen.hukukiStatu = VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.HUKUKISTATU, 0);
                gelen.kimlikNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.KIMLIKNO, string.Empty);
                gelen.yukleniciAd = VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.YUKLENICIAD, string.Empty);
                gelen.sozlesmeTur = (ENUMHibeYatirimUygulamaSozlesmeTur)VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMETUR, 0);
                gelen.sozlesmeTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMETARIH);
                gelen.sozlesmeTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMETUTAR, (decimal)0);
                gelen.sozlesmeDurum = (ENUMHibeYatirimUygulamaSozlesmeDurum)VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMEDURUM, 0);

                al.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (HibeYatirimUygulamaSozlesme[])al.ToArray(typeof(HibeYatirimUygulamaSozlesme));
        }
    }
}
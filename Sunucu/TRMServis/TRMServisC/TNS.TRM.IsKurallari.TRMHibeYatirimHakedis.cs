using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeYatirimHakedis
    {
        public static Sonuc Kaydet(ref VeriErisim ve, Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            Sonuc sonuc = new Sonuc();
            if (hakedis.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz!<br>";
            if (hakedis.sozlesmeKod <= 0)
                sonuc.hataStr += "Sözleşme kodu boş bırakılamaz!<br>";
            if (string.IsNullOrEmpty(hakedis.faturaNo))
                sonuc.hataStr += "Fatura no boş bırakılamaz!<br>";

            if (hakedis.sonOdeme != 0 && hakedis.sonOdeme != 1)
                sonuc.hataStr += "Son ödeme olup olmadığını giriniz!<br>";

            if (hakedis.faturaTarih == null || hakedis.faturaTarih.isNull)
                sonuc.hataStr += "Fatura tarihi boş bırakılamaz!<br>";
            else if (hakedis.faturaTarih.Oku() > new TNSDateTime(DateTime.Now).Oku())
                sonuc.hataStr += "Fatura tarihi bugünden sonra olamaz!<br>";

            if (hakedis.faturaTutar <= 0)
                sonuc.hataStr += "Fatura tutarı boş bırakılamaz!<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            sonuc = SozlesmeyeUygunMu(ve, kullanan, hakedis, true);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            ve.BeginTransaction();

            if (hakedis.hakedisKod <= 0)
            {
                hakedis.hakedisDurum = ENUMHibeYatirimHakedisDurum.YENI;
                hakedis.hakedisKod = new TABGenel(ve).BelgeNoAlInt("HIBEYATIRIMHAKEDIS", "000000000");
                sonuc = tabHakedis.Kaydet(hakedis);
            }
            else
            {
                if (hakedis.hakedisDurum == ENUMHibeYatirimHakedisDurum.ONAYLANDI)
                {
                    hakedis.onayTarih = new TNSDateTime(DateTime.Now);
                    sonuc = tabHakedis.GuncelleDurum(hakedis);
                }
                else
                    sonuc = tabHakedis.Guncelle(hakedis);
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                sonuc.anahtar = hakedis.hakedisKod.ToString();
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc EskiEtapHakedisleriKaydet(ref VeriErisim ve, Kullanici kullanan, HibeYatirimHakedis[] hakedisler)
        {
            Sonuc sonuc = new Sonuc();

            if (hakedisler.Length <= 0)
                return sonuc;

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);

            ve.BeginTransaction();

            if (hakedisler[0].sozlesmeKod > 0)
            {
                foreach (HibeYatirimHakedis hakedis in hakedisler)
                {
                    if (sonuc.islemSonuc)
                    {
                        if (hakedis != null)
                        {
                            hakedis.hakedisDurum = ENUMHibeYatirimHakedisDurum.YENI;
                            hakedis.hakedisKod = new TABGenel(ve).BelgeNoAlInt("HIBEYATIRIMHAKEDIS", "000000000");
                            sonuc = tabHakedis.Kaydet(hakedis);
                        }
                    }
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            return sonuc;
        }

        public static Sonuc Sil(ref VeriErisim ve, Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            Sonuc sonuc = new Sonuc();
            if (hakedis.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (hakedis.sozlesmeKod <= 0)
                sonuc.hataStr += "Sözleşme kodu boş bırakılamaz.<br>";
            if (hakedis.hakedisKod <= 0)
                sonuc.hataStr += "Hakediş kodu boş bırakılamaz.<br>";
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
                sonuc.hataStr = "Hakediş silme yetkiniz yok. Merkez tarafından silinebilir";
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            //Silmeye uygun olmayan hakediş var mı?
            HibeYatirimHakedis[] hler = Listele(ref ve, kullanan, hakedis);
            foreach (HibeYatirimHakedis h in hler)
            {
                if (h.odemeKod > 0)
                {
                    sonuc.hataStr = "Bu hakediş ödendiği için silme işlemi yapılamaz";
                    sonuc.islemSonuc = false;
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }


            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            ve.BeginTransaction();
            sonuc = tabHakedis.Sil(hakedis);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Hakediş başarıyla silindi.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc SozlesmeyeUygunMu(VeriErisim ve, Kullanici kullanan, HibeYatirimHakedis hakedis, bool sonOdemeKontrol)
        {
            decimal sozlesmeTutar = new TABHibeYatirimUygulamaSozlesme(ve).SozlesmeTutarToplamAl(hakedis.hibeKod, hakedis.sozlesmeKod, true);//kendisi true ise sozlesmeTur'ün ne olduğu önemli değil
            decimal faturaTutar = new TABHibeYatirimHakedis(ve).FaturaTutarToplamAl(hakedis.hibeKod, hakedis.sozlesmeKod, hakedis.hakedisKod, false);
            decimal fark = sozlesmeTutar - faturaTutar;
            if (hakedis.faturaTutar > fark)
                return new Sonuc(false, "Hakedişin fatura tutarı, hakedişe bağlanmamış tutar olan " + fark.ToString("N2") + " TL'den büyük olamaz.", string.Empty);
            //if (sonOdemeKontrol)
            //    if (hakedis.faturaTutar == fark && hakedis.sonOdeme != 1)
            //        return new Sonuc(false, "Lüffen tüm kalan tutar ödeniyorsa son ödeme işareti koyunuz!", string.Empty);

            if (hakedis.hakedisKod > 0)
            {
                HibeYatirimHakedis h = new HibeYatirimHakedis();
                h.hakedisKod = hakedis.hakedisKod;
                h.hibeKod = hakedis.hibeKod;
                HibeYatirimHakedis[] liste = Listele(ref ve, kullanan, h);
                foreach (HibeYatirimHakedis l in liste)
                {
                    if (l.sozlesmeKod != hakedis.sozlesmeKod)
                        return new Sonuc(false, "Daha önce kayıt edilmiş hakediş sahibini değiştiremezsiniz.", string.Empty);
                }
            }

            return new Sonuc();
        }

        public static HibeYatirimHakedis[] Listele(ref VeriErisim ve, Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            if (hakedis.hibeKod == 0 && hakedis.odemeGrupNo.Trim() == "")
                throw new Exception("Hibe kodu veya ödeme grup numarası boş bırakılamaz.");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            IDataReader dr = tabHakedis.Listele(hakedis);
            if (!string.IsNullOrEmpty(tabHakedis.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tabHakedis.Hata());
            }

            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                HibeYatirimHakedis gelen = new HibeYatirimHakedis();
                gelen.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.HIBEKOD, 0);
                gelen.sozlesmeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.SOZLESMEKOD, 0);
                gelen.hakedisKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.HAKEDISKOD, 0);
                gelen.faturaNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.FATURANO, string.Empty);
                gelen.faturaTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.FATURATARIH);
                gelen.faturaTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.FATURATUTAR, (decimal)0);
                gelen.hakedisDurum = (ENUMHibeYatirimHakedisDurum)VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.HAKEDISDURUM, 0);
                gelen.onayTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.ONAYTARIH);
                gelen.redAciklama = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.REDACIKLAMA, string.Empty);
                gelen.odemeTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.ODEMETARIH);
                gelen.odemeGrupNo = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.ODEMEGRUPNO, string.Empty);
                gelen.odemeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.ODEMEKOD, 0);
                gelen.kontrol = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.KONTROL, 0);
                gelen.kontrolTarih = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.KONTROLTARIH);
                gelen.esasFaturaTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.ESASFATURATUTAR, (decimal)0);
                gelen.sonOdeme = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.SONODEME, (int)0);
                //gelen.sonOdeme = 1;
                al.Add(gelen);
            }
            dr.Close();

            foreach (HibeYatirimHakedis h in al)
            {
                HibeYatirimUygulamaSozlesme[] sozlesmeBilgi = TRMHibeYatirimUygulamaSozlesme.Listele(ref ve, kullanan, new HibeYatirimUygulamaSozlesme() { hibeKod = h.hibeKod, sozlesmeKod = h.sozlesmeKod }, false);
                if (sozlesmeBilgi != null && sozlesmeBilgi.Length > 0)
                    h.sozlesmeBilgi = sozlesmeBilgi[0];
            }

            ve.VeriErisimKapat();
            return (HibeYatirimHakedis[])al.ToArray(typeof(HibeYatirimHakedis));
        }

        public static HibeYatirimUygulamaSozlesme[] ListeleUygulamaSozlesmesiVeHakedisler(ref VeriErisim ve, Kullanici kullanan, int hibeKod)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            IDataReader r = tabHakedis.ListeleUygulamaSozlesmesiVeHakedisler(hibeKod);
            if (!string.IsNullOrEmpty(tabHakedis.Hata()))
                throw new Exception(tabHakedis.Hata());

            ArrayList al = new ArrayList();
            int hibeSonKolon = ((int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMEDURUM) + 1;
            while (r.Read())
            {
                HibeYatirimUygulamaSozlesme uygSozlesmeBilgileri = new HibeYatirimUygulamaSozlesme();
                uygSozlesmeBilgileri.hibeKod = VeriErisim.DegerAl(r, 0, 0);
                uygSozlesmeBilgileri.sozlesmeKod = VeriErisim.DegerAl(r, 1, 0);
                uygSozlesmeBilgileri.hukukiStatu = VeriErisim.DegerAl(r, 2, 0);
                uygSozlesmeBilgileri.kimlikNo = VeriErisim.DegerAl(r, 3, string.Empty);
                uygSozlesmeBilgileri.yukleniciAd = VeriErisim.DegerAl(r, 4, string.Empty);
                uygSozlesmeBilgileri.sozlesmeTur = (ENUMHibeYatirimUygulamaSozlesmeTur)VeriErisim.DegerAl(r, 5, 0);
                uygSozlesmeBilgileri.sozlesmeTarih = VeriErisim.DegerAl(r, 6);
                uygSozlesmeBilgileri.sozlesmeTutar = VeriErisim.DegerAl(r, 7, decimal.Zero);
                uygSozlesmeBilgileri.sozlesmeDurum = (ENUMHibeYatirimUygulamaSozlesmeDurum)VeriErisim.DegerAl(r, 8, 0);

                uygSozlesmeBilgileri.hakedisBilgileri = new HibeYatirimHakedis[1];
                uygSozlesmeBilgileri.hakedisBilgileri[0] = new HibeYatirimHakedis();
                uygSozlesmeBilgileri.hakedisBilgileri[0].hibeKod = VeriErisim.DegerAl(r, hibeSonKolon, 0);
                uygSozlesmeBilgileri.hakedisBilgileri[0].sozlesmeKod = VeriErisim.DegerAl(r, hibeSonKolon + 1, 0);
                uygSozlesmeBilgileri.hakedisBilgileri[0].hakedisKod = VeriErisim.DegerAl(r, hibeSonKolon + 2, 0);
                uygSozlesmeBilgileri.hakedisBilgileri[0].faturaNo = VeriErisim.DegerAl(r, hibeSonKolon + 3, string.Empty);
                uygSozlesmeBilgileri.hakedisBilgileri[0].faturaTarih = VeriErisim.DegerAl(r, hibeSonKolon + 4);
                uygSozlesmeBilgileri.hakedisBilgileri[0].faturaTutar = VeriErisim.DegerAl(r, hibeSonKolon + 5, decimal.Zero);
                uygSozlesmeBilgileri.hakedisBilgileri[0].hakedisDurum = (ENUMHibeYatirimHakedisDurum)VeriErisim.DegerAl(r, hibeSonKolon + 6, 0);
                uygSozlesmeBilgileri.hakedisBilgileri[0].onayTarih = VeriErisim.DegerAl(r, hibeSonKolon + 7);
                uygSozlesmeBilgileri.hakedisBilgileri[0].redAciklama = VeriErisim.DegerAl(r, hibeSonKolon + 8, string.Empty);
                uygSozlesmeBilgileri.hakedisBilgileri[0].odemeTarih = VeriErisim.DegerAl(r, hibeSonKolon + 9);
                uygSozlesmeBilgileri.hakedisBilgileri[0].odemeGrupNo = VeriErisim.DegerAl(r, hibeSonKolon + 10, string.Empty);
                uygSozlesmeBilgileri.hakedisBilgileri[0].odemeKod = VeriErisim.DegerAl(r, hibeSonKolon + 11, 0);
                uygSozlesmeBilgileri.hakedisBilgileri[0].esasFaturaTutar = VeriErisim.DegerAl(r, hibeSonKolon + 12, decimal.Zero);
                uygSozlesmeBilgileri.hakedisBilgileri[0].sonOdeme = VeriErisim.DegerAl(r, hibeSonKolon + 13, 0);

                al.Add(uygSozlesmeBilgileri);
            }
            r.Close();

            ve.VeriErisimKapat();
            return (HibeYatirimUygulamaSozlesme[])al.ToArray(typeof(HibeYatirimUygulamaSozlesme));
        }

        public static HibeYatirim[] ListeleOdemeGonder(ref VeriErisim ve, Kullanici kullanan, HibeYatirim kriter)
        {
            if (!Yetki.YetkiVarMiOkumaya(kullanan, kriter.ilKod, ENUMHibeTur.EKONOMIKYATIRIM))
                throw new Exception("Okuma yetkiniz bulunmamaktadır.");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            IDataReader r = tabHakedis.ListeleOdemeGonder(kriter);
            if (!string.IsNullOrEmpty(tabHakedis.Hata()))
                throw new Exception(tabHakedis.Hata());

            ArrayList al = new ArrayList();
            int hibeSonKolon = ((int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD);
            while (r.Read())
            {
                HibeYatirim f = new HibeYatirim();

                f.kod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOD, 0);
                f.yil = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YIL, 0);
                f.basvuruNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUNO, string.Empty);
                f.ilKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILKOD, string.Empty);
                f.ilceKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ILCEKOD, string.Empty);
                f.koyKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KOYKOD, 0);
                f.kasaba = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KASABA, string.Empty);
                f.hukukiStatu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HUKUKISTATU, 0);
                f.kimlikNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KIMLIKNO, string.Empty);
                f.vergiDaire = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.VERGIDAIRE, string.Empty);
                f.ad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AD, string.Empty);
                f.evrakNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKNO, string.Empty);
                f.evrakTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EVRAKTARIH);
                f.adres = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ADRES, string.Empty);
                f.telefon = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TELEFON, string.Empty);
                f.faks = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FAKS, string.Empty);
                f.ePosta = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.EPOSTA, string.Empty);
                f.bankaSubeKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BANKASUBEKOD, string.Empty);
                f.durum = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DURUM, 0);
                f.sozlesmeTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMETARIH);
                f.faturaNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURANO, string.Empty);
                f.faturaTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.FATURATARIH);
                f.soyad = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOYAD, string.Empty);
                f.babaAd = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BABAAD, string.Empty);
                f.dogumTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DOGUMTARIH);
                f.yerindeTespitTarih = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YERINDETESPITTARIH);
                f.grupKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.GRUPKOD, string.Empty);
                f.etapNo = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ETAPNO, 0);
                f.projeNitelik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJENITELIK, 0);
                f.projeKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEKONU, string.Empty);
                f.projeAltKonu = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEALTKONU, string.Empty);
                f.projeAdi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PROJEADI, string.Empty);
                f.sozlesmeMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.SOZLESMEMALIYET, decimal.Zero);
                f.basvuruMaliyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.BASVURUMALIYET, decimal.Zero);
                f.hibeTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBETUTAR, decimal.Zero);
                f.hibeEsasTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.HIBEESASTUTAR, decimal.Zero);
                f.ayniKatki = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.AYNIKATKI, decimal.Zero);
                f.toplamTutar = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.TOPLAMTUTAR, decimal.Zero);
                f.anket1 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET1, 0);
                f.anket2 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET2, 0);
                f.anket3 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3, 0);
                f.anket4 = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4, 0);
                f.anket3Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET3PUAN, 0);
                f.anket4Puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ANKET4PUAN, 0);
                f.puan = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.PUAN, (double)0);
                f.konuNitelikUyusma = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.KONUNITELIKUYUSMA, 0) == 1 ? true : false;
                f.degerlendirmeDisi = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISI, 0) == 1 ? true : false;
                f.degerlendirmeDisiGerekce = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.DEGERLENDIRMEDISIGEREKCE, string.Empty);
                f.altYapiGrup = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTYAPIGRUP, string.Empty);
                f.cinsiyet = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.CINSIYET, string.Empty);
                f.engelli = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ENGELLI, string.Empty);
                f.organik = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ORGANIK, string.Empty);
                f.yas = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.YAS, 0);
                f.altKonuDetayKod = VeriErisim.DegerAl(r, (int)TABHibeYatirim.ENUMAlanlar.ALTKONUDETAYKOD, "");

                f.uygSozlesmeBilgileri = new HibeYatirimUygulamaSozlesme[1];
                f.uygSozlesmeBilgileri[0] = new HibeYatirimUygulamaSozlesme();
                f.uygSozlesmeBilgileri[0].hibeKod = VeriErisim.DegerAl(r, hibeSonKolon + 1, 0);
                f.uygSozlesmeBilgileri[0].sozlesmeKod = VeriErisim.DegerAl(r, hibeSonKolon + 2, 0);
                f.uygSozlesmeBilgileri[0].hukukiStatu = VeriErisim.DegerAl(r, hibeSonKolon + 3, 0);
                f.uygSozlesmeBilgileri[0].kimlikNo = VeriErisim.DegerAl(r, hibeSonKolon + 4, string.Empty);
                f.uygSozlesmeBilgileri[0].yukleniciAd = VeriErisim.DegerAl(r, hibeSonKolon + 5, string.Empty);
                f.uygSozlesmeBilgileri[0].sozlesmeTur = (ENUMHibeYatirimUygulamaSozlesmeTur)VeriErisim.DegerAl(r, hibeSonKolon + 6, 0);
                f.uygSozlesmeBilgileri[0].sozlesmeTarih = VeriErisim.DegerAl(r, hibeSonKolon + 7);
                f.uygSozlesmeBilgileri[0].sozlesmeTutar = VeriErisim.DegerAl(r, hibeSonKolon + 8, decimal.Zero);
                f.uygSozlesmeBilgileri[0].sozlesmeDurum = (ENUMHibeYatirimUygulamaSozlesmeDurum)VeriErisim.DegerAl(r, hibeSonKolon + 9, 0);

                f.uygSozlesmeBilgileri[0].hakedisBilgileri = new HibeYatirimHakedis[1];
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0] = new HibeYatirimHakedis();
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].hibeKod = VeriErisim.DegerAl(r, hibeSonKolon + 10, 0);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].sozlesmeKod = VeriErisim.DegerAl(r, hibeSonKolon + 11, 0);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].hakedisKod = VeriErisim.DegerAl(r, hibeSonKolon + 12, 0);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].faturaNo = VeriErisim.DegerAl(r, hibeSonKolon + 13, string.Empty);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].faturaTarih = VeriErisim.DegerAl(r, hibeSonKolon + 14);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].faturaTutar = VeriErisim.DegerAl(r, hibeSonKolon + 15, decimal.Zero);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].hakedisDurum = (ENUMHibeYatirimHakedisDurum)VeriErisim.DegerAl(r, hibeSonKolon + 16, 0);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].onayTarih = VeriErisim.DegerAl(r, hibeSonKolon + 17);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].redAciklama = VeriErisim.DegerAl(r, hibeSonKolon + 18, string.Empty);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].odemeTarih = VeriErisim.DegerAl(r, hibeSonKolon + 19);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].odemeGrupNo = VeriErisim.DegerAl(r, hibeSonKolon + 20, string.Empty);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].odemeKod = VeriErisim.DegerAl(r, hibeSonKolon + 21, 0);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].kontrol = VeriErisim.DegerAl(r, hibeSonKolon + 22, 0);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].kontrolTarih = VeriErisim.DegerAl(r, hibeSonKolon + 23);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].esasFaturaTutar = VeriErisim.DegerAl(r, hibeSonKolon + 24, decimal.Zero);
                f.uygSozlesmeBilgileri[0].hakedisBilgileri[0].sonOdeme = VeriErisim.DegerAl(r, hibeSonKolon + 25, 0);

                f.projeNitelikAdi = VeriErisim.DegerAl(r, hibeSonKolon + 26, "");
                f.projeKonuAdi = VeriErisim.DegerAl(r, hibeSonKolon + 27, "");
                f.projeAltKonuAdi = VeriErisim.DegerAl(r, hibeSonKolon + 28, "");

                f.ilAd = VeriErisim.DegerAl(r, hibeSonKolon + 29, "");
                f.ilceAd = VeriErisim.DegerAl(r, hibeSonKolon + 30, "");
                f.koyAd = VeriErisim.DegerAl(r, hibeSonKolon + 31, "");

                al.Add(f);
            }
            r.Close();

            ve.VeriErisimKapat();
            return (HibeYatirim[])al.ToArray(typeof(HibeYatirim));
        }

        public static Sonuc OdemeyeGonder(ref VeriErisim ve, Kullanici kullanan, string odemeGrupNo)
        {
            Sonuc sonuc = new Sonuc();

            HibeYatirimHakedis h = new HibeYatirimHakedis();
            h.odemeGrupNo = odemeGrupNo;

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            HibeYatirimHakedis[] liste = Listele(ref ve, kullanan, h);
            sonuc = OdemeIslemYap(ref ve, kullanan, liste, ENUMHibeYatirimHakedisDurum.ODEMEDE);

            if (sonuc.islemSonuc)
            {
                string[] parcalar = odemeGrupNo.Split('.');
                string plaka = parcalar[1];
                string ilKod = new TABGenel(ve).IlKodBul(plaka);

                //Ödeme Grubunu kayıt etmek için
                TABHibeMakineSulamaGrup tGrup = new TABHibeMakineSulamaGrup(ve);
                HibeMakineSulamaGrup g = new HibeMakineSulamaGrup();
                g.durum = (int)ENUMHibeYatirimHakedisDurum.ODEMEDE;
                g.ilKod = ilKod;
                g.odemeKod = 0;
                g.kod = odemeGrupNo;
                g.tipi = (int)ENUMHibeTur.EKONOMIKYATIRIM;
                sonuc = tGrup.Kaydet(g);

                sonuc.bilgiStr = "Ödemeye gönderme işlemi başarıyla tamamlandı";
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc OdemeIslemYap(ref VeriErisim ve, Kullanici kullanan, HibeYatirimHakedis[] hakedisler, ENUMHibeYatirimHakedisDurum yeniHakedisDurum)
        {
            if (hakedisler == null || hakedisler.Length == 0)
                return new Sonuc(false, "Ödeme işlemine alınacak herhangi bir hakediş seçilmemiş.", string.Empty);
            if (!(yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.ONAYLANDI ||
                yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.RED ||
                yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.ODEMEDE ||
                yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.ODENDI))
                return new Sonuc(false, "Hakediş durumu, \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ONAYLANDI)
                    + "\", \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.RED)
                    + "\", \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ODEMEDE)
                    + "\" ya da \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ODENDI)
                    + "\" olmalıdır.", string.Empty);
            if (yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.RED && string.IsNullOrEmpty(hakedisler[0].redAciklama))
                return new Sonuc(false, "Red açıklaması boş bırakılamaz.", string.Empty);

            Sonuc sonuc = new Sonuc();
            for (int i = 0; i < hakedisler.Length; i++)
            {
                HibeYatirimHakedis hakedis = hakedisler[i];
                if (hakedis.hibeKod <= 0)
                    sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
                if (hakedis.hakedisKod <= 0)
                    sonuc.hataStr += "Hakediş kodu boş bırakılamaz.<br>";
                if (!string.IsNullOrEmpty(sonuc.hataStr))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = (i + 1).ToString() + ". başvuruda aşağıdaki hatalar var. Lütfen kontrol ediniz.<br>" + sonuc.hataStr;
                    return sonuc;
                }
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            sonuc = OdemeIslemineUygunMu(tabHakedis, hakedisler, yeniHakedisDurum);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            foreach (HibeYatirimHakedis hakedis in hakedisler)
            {
                hakedis.hakedisDurum = yeniHakedisDurum;
                sonuc = tabHakedis.GuncelleDurum(hakedis);
                if (!sonuc.islemSonuc)
                    break;
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

        private static Sonuc OdemeIslemineUygunMu(TABHibeYatirimHakedis tabHakedis, HibeYatirimHakedis[] hakedisler, ENUMHibeYatirimHakedisDurum yeniHakedisDurum)
        {
            for (int i = 0; i < hakedisler.Length; i++)
            {
                HibeYatirimHakedis hakedis = hakedisler[i];
                hakedis.hakedisDurum = tabHakedis.DurumGetir(hakedis);

                if (yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.ONAYLANDI && hakedis.hakedisDurum != ENUMHibeYatirimHakedisDurum.ODEMEDE)
                    return new Sonuc(false, "Durumu \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ONAYLANDI) + "\" yapılmak istenen " + (i + 1).ToString() + ". hakedişin durumu \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ODEMEDE) + "\" olmak zorundadır.", string.Empty);
                if (yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.RED && hakedis.hakedisDurum > ENUMHibeYatirimHakedisDurum.ONAYLANDI)
                    return new Sonuc(false, "Durumu \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.RED) + "\" yapılmak istenen " + (i + 1).ToString() + ". hakedişin durumu \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.YENI) + "\" veya \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ONAYLANDI) + "\" olmak zorundadır.", string.Empty);
                if (yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.ODEMEDE && hakedis.hakedisDurum != ENUMHibeYatirimHakedisDurum.ONAYLANDI)
                    return new Sonuc(false, "Durumu \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ODEMEDE) + "\" yapılmak istenen " + (i + 1).ToString() + ". hakedişin durumu \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ONAYLANDI) + "\" olmak zorundadır.", string.Empty);
                if (yeniHakedisDurum == ENUMHibeYatirimHakedisDurum.ODEMEBASLAT && hakedis.hakedisDurum != ENUMHibeYatirimHakedisDurum.ODEMEDE)
                    return new Sonuc(false, "Durumu \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ODEMEBASLAT) + "\" yapılmak istenen " + (i + 1).ToString() + ". hakedişin durumu \"" + HibeYatirimHakedis.HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum.ODEMEDE) + "\" olmak zorundadır.", string.Empty);
            }
            return new Sonuc();
        }

        public static Sonuc GuncelleOdemeGrupNo(ref VeriErisim ve, Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            Sonuc sonuc = new Sonuc();
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            if (hakedis.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (hakedis.hakedisKod <= 0)
                sonuc.hataStr = "Hakediş kodu boş bırakılamaz.<br>";

            //Kayıt edilmek istenilen grup uygunmu. ödemeye gönderilmiş mi?
            //********************************************************************
            HibeYatirim hy = TRMHibeYatirim.ListeleSozlesme(ref ve, kullanan, hakedis.hibeKod);

            if (hy.kod > 0)
            {
                HibeMakineSulamaGrup g = new HibeMakineSulamaGrup();
                g.tipi = (int)ENUMHibeTur.EKONOMIKYATIRIM;
                g.kod = hakedis.odemeGrupNo;
                g.ilKod = hy.ilKod;
                HibeMakineSulamaGrup[] odemeListe = TRMHibeMakineSulamaGrup.Listele(ve, kullanan, g, null);
                foreach (HibeMakineSulamaGrup o in odemeListe)
                {
                    if (o.durum > 1)
                    {
                        sonuc.hataStr = hakedis.odemeGrupNo + " nolu ödeme grup numarası kullanılamaz.";
                        break;
                    }
                }
            }
            else
            {
                sonuc.hataStr = hakedis.odemeGrupNo + " nolu ödeme grup numarası kullanılamaz. Hibe kaydı bulunamadı";
            }
            //********************************************************************
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            sonuc = SozlesmeyeUygunMu(ve, kullanan, hakedis, false);
            if (!sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            ve.BeginTransaction();

            sonuc = tabHakedis.GuncelleOdemeGrupNo(hakedis);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
                sonuc.anahtar = hakedis.hakedisKod.ToString();
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc GuncelleKontrol(ref VeriErisim ve, Kullanici kullanan, HibeYatirimHakedis hakedis)
        {
            Sonuc sonuc = new Sonuc();
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            if (hakedis.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (hakedis.hakedisKod <= 0)
                sonuc.hataStr = "Hakediş kodu boş bırakılamaz.<br>";

            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            ve.BeginTransaction();
            if (hakedis.kontrol > 0)
                hakedis.kontrolTarih = new TNSDateTime(DateTime.Now);

            sonuc = tabHakedis.GuncelleKontrol(hakedis);

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

        public static HibeMakineSulamaGrup[] HibeYatirimHakedisListeleOdeme(VeriErisim ve, Kullanici kullanan, int etapNo, int durum, int tur, int odemeKod, bool kontrolEdilen, TNSDateTime baslamaTarih, TNSDateTime bitisTarihi)
        {
            if (baslamaTarih == null) baslamaTarih = new TNSDateTime();
            if (bitisTarihi == null) bitisTarihi = new TNSDateTime();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirimHakedis tHibe = new TABHibeYatirimHakedis(ve);
            IDataReader r = tHibe.ListeleOdeme(etapNo, tur, durum, odemeKod, kontrolEdilen, baslamaTarih, bitisTarihi);
            if (!string.IsNullOrEmpty(tHibe.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tHibe.Hata());
            }

            IList<HibeMakineSulamaGrup> gruplar = new List<HibeMakineSulamaGrup>();
            while (r.Read())
            {
                HibeMakineSulamaGrup grup = new HibeMakineSulamaGrup();
                grup.kod = VeriErisim.DegerAl(r, 1, string.Empty);
                grup.yatirimciKimlikNo = VeriErisim.DegerAl(r, 2, string.Empty);
                grup.ilAd = VeriErisim.DegerAl(r, 3, string.Empty);
                grup.basvuruNo = VeriErisim.DegerAl(r, 5, "");
                grup.yatirimciAd = VeriErisim.DegerAl(r, 6, "");
                grup.yukleniciAd = VeriErisim.DegerAl(r, 7, "");

                int sozlesmeTur = VeriErisim.DegerAl(r, 8, 0);
                if (sozlesmeTur == 1)
                    grup.sozlesmeTur = "Mal";
                else if (sozlesmeTur == 2)
                    grup.sozlesmeTur = "İnşaat";
                else
                    grup.sozlesmeTur = "Anahtar Teslim";

                grup.hibeEsasTutar = VeriErisim.DegerAl(r, 10, (decimal)0);
                grup.hibeTutar = grup.hibeEsasTutar / 2;

                grup.ilKod = VeriErisim.DegerAl(r, 11, string.Empty);
                grup.durum = VeriErisim.DegerAl(r, 12, 0);
                grup.sozlesmeKod = VeriErisim.DegerAl(r, 13, 0);
                grup.hibeKod = VeriErisim.DegerAl(r, 14, 0);
                grup.yatirimciBankaSube = VeriErisim.DegerAl(r, 16, "");
                grup.yatirimciSoyad = VeriErisim.DegerAl(r, 17, "");
                grup.yatirimciBabaAd = VeriErisim.DegerAl(r, 18, "");
                grup.yatirimciDogumTarih = VeriErisim.DegerAl(r, 19);
                grup.kontrol = VeriErisim.DegerAl(r, 20, 0);
                grup.kontrolTarih = VeriErisim.DegerAl(r, 21);
                grup.hakedisKod = VeriErisim.DegerAl(r, 22, 0);
                grup.odemeTarih = VeriErisim.DegerAl(r, 23);
                grup.telefon = VeriErisim.DegerAl(r, 24, "");

                //grup.toplamTutar = VeriErisim.DegerAl(r, sonKolon + 2, (decimal)0);
                //grup.ayniKatki = VeriErisim.DegerAl(r, sonKolon + 3, (decimal)0);

                grup.tipi = (int)ENUMHibeTur.EKONOMIKYATIRIM;
                if (grup.hibeTutar > 0)
                    gruplar.Add(grup);
            }
            r.Close();

            ve.VeriErisimKapat();
            HibeMakineSulamaGrup[] donusDeger = new HibeMakineSulamaGrup[gruplar.Count];
            gruplar.CopyTo(donusDeger, 0);
            return donusDeger;
        }

        public static Sonuc GrupOdemeBaslat(VeriErisim ve, Kullanici kullanan, HibeMakineSulamaGrup[] bilgi, int odemeKod)
        {
            Sonuc sonuc = new Sonuc();

            ObjectArray liste = new ObjectArray();
            foreach (HibeMakineSulamaGrup g in bilgi)
            {
                string[] bler = g.kod.Split('é');
                HibeYatirimHakedis y = new HibeYatirimHakedis();
                y.odemeGrupNo = bler[0];
                y.hibeKod = OrtakFonksiyonlar.ConvertToInt(bler[1], 0);
                y.sozlesmeKod = OrtakFonksiyonlar.ConvertToInt(bler[2], 0);
                liste.objeler.Add(y);
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri, TNS.KYM.Arac.KimlikBul(kullanan));
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);

            foreach (HibeYatirimHakedis y in liste.objeler)
            {
                sonuc = SozlesmeyeUygunMu(ve, kullanan, y, false);
                if (!sonuc.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }
            }

            ve.BeginTransaction();
            foreach (HibeYatirimHakedis y in liste.objeler)
            {
                sonuc = tabHakedis.OdemeBaslatildiYap(y.hibeKod, y.sozlesmeKod, y.odemeGrupNo, odemeKod);
                if (!sonuc.islemSonuc)
                    break;
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

        public static Sonuc GrupOdendi(VeriErisim ve, Kullanici kullanan, int odemeKod, int durum)
        {
            Sonuc sonuc = new Sonuc();

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);

            ve.BeginTransaction();

            sonuc = tabHakedis.OdemeDurumDegistir(odemeKod, durum);

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

        public static Sonuc OdenenHakedislerinUygulamaSozlesmesiDurumGuncellemesi(Kullanici kullanan, int odemeKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            IDataReader dr = tabHakedis.OdemeKodunaGoreOdenenHakedisler(odemeKod);
            if (!string.IsNullOrEmpty(tabHakedis.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tabHakedis.Hata());
            }
            TABHibeYatirimUygulamaSozlesme tabUygS = new TABHibeYatirimUygulamaSozlesme(ve);
            HibeYatirimUygulamaSozlesme usozlesme = new HibeYatirimUygulamaSozlesme();
            TABHibeYatirim tabYatirim = new TABHibeYatirim(ve);

            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                HibeYatirimHakedis odenen = new HibeYatirimHakedis();
                odenen.hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.HIBEKOD, 0);
                odenen.sozlesmeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.SOZLESMEKOD, 0);
                odenen.faturaTutar = VeriErisim.DegerAl(dr, 2, (decimal)0);

                al.Add(odenen);
            }
            dr.Dispose();
            decimal sozlesmeTutar = 0;
            decimal faturaTutar = 0;

            foreach (HibeYatirimHakedis h in al)
            {
                faturaTutar = 0;
                sozlesmeTutar = 0;
                usozlesme.hibeKod = h.hibeKod;
                usozlesme.sozlesmeKod = h.sozlesmeKod;
                h.hakedisDurum = ENUMHibeYatirimHakedisDurum.ODENDI;

                dr = tabHakedis.DigerOdenenHakedisler(h);
                if (!string.IsNullOrEmpty(tabHakedis.Hata()))
                {
                    ve.VeriErisimKapat();
                    throw new Exception(tabHakedis.Hata());
                }
                if (dr.Read())
                    faturaTutar = VeriErisim.DegerAl(dr, 2, (decimal)0);
                dr.Dispose();

                dr = tabUygS.Listele(usozlesme, false);
                if (!string.IsNullOrEmpty(tabUygS.Hata()))
                {
                    ve.VeriErisimKapat();
                    throw new Exception(tabHakedis.Hata());
                }
                if (dr.Read())
                    sozlesmeTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirimUygulamaSozlesme.ENUMAlanlar.SOZLESMETUTAR, (decimal)0);
                dr.Dispose();

                if (faturaTutar == sozlesmeTutar)
                {
                    ENUMHibeYatirimUygulamaSozlesmeDurum durum = ENUMHibeYatirimUygulamaSozlesmeDurum.BITTI;
                    sonuc = tabUygS.GuncelleDurum(new HibeYatirimUygulamaSozlesme { sozlesmeDurum = durum, hibeKod = h.hibeKod, sozlesmeKod = h.sozlesmeKod });
                }
            }

            sonuc = OdenenHakedislerinSozlesmeDurumGuncellemesi(kullanan, odemeKod);

            dr.Close();
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc OdenenHakedislerinSozlesmeDurumGuncellemesi(Kullanici kullanan, int odemeKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirimHakedis tabHakedis = new TABHibeYatirimHakedis(ve);
            TABHibeYatirim tabYatirim = new TABHibeYatirim(ve);
            TABHibeYatirimUygulamaSozlesme tabUygSozlesme = new TABHibeYatirimUygulamaSozlesme(ve);
            IDataReader dr = tabHakedis.OdemeKodunaGoreOdenenHakedislerGroupByHibeKod(odemeKod);
            if (!string.IsNullOrEmpty(tabHakedis.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tabHakedis.Hata());
            }

            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                int hibeKod = VeriErisim.DegerAl(dr, (int)TABHibeYatirimHakedis.ENUMAlanlar.HIBEKOD, 0);
                al.Add(hibeKod);
            }
            dr.Dispose();
            decimal hibeEsasTutar = 0, sozlemeTutari = 0;
            foreach (int hibeKod in al)
            {
                hibeEsasTutar = 0; sozlemeTutari = 0;
                dr = tabYatirim.ListeleMaliyet(hibeKod, ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI);
                if (dr.Read())
                {
                    hibeEsasTutar = VeriErisim.DegerAl(dr, (int)TABHibeYatirim.ENUMAlanlarMaliyet.HIBEESASTUTARTOPLAM, (decimal)0);
                    dr.Dispose();
                }

                dr = tabUygSozlesme.ListeleBitenUygulamaSozlesmelerininToplamTutarini(new HibeYatirimUygulamaSozlesme { sozlesmeDurum = ENUMHibeYatirimUygulamaSozlesmeDurum.BITTI, hibeKod = hibeKod });
                if (dr.Read())
                {
                    sozlemeTutari = VeriErisim.DegerAl(dr, 0, (decimal)0);
                    dr.Dispose();
                }
                if (hibeEsasTutar != 0 && sozlemeTutari != 0 && sozlemeTutari == hibeEsasTutar)
                    sonuc = tabYatirim.GuncelleSozlesmeDurum(new HibeYatirimSozlesme { durum = ENUMHibeYatirimSozlesmeDurum.BITTI, hibeKod = hibeKod });
            }

            dr.Close();
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc YatirimciyiOdemedenCikart(Kullanici kullanan, TNS.TRM.HibeYatirimHakedis hakedis)
        {
            Sonuc sonuc = new Sonuc();
            decimal hakedisTutari = 0;
            int odemeKod = 0;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABHibeYatirimHakedis to = new TABHibeYatirimHakedis(ve);

            HibeYatirimHakedis[] liste = Listele(ref ve, kullanan, hakedis);

            foreach (HibeYatirimHakedis item in liste)
            {

                if (item.odemeGrupNo == "" || item.odemeKod == 0)
                {
                    sonuc.hataStr = "Hakediş ödeme listesinde bulunmadığı için işlem yapılamadı";
                    break;
                }

                if (item.hakedisDurum == ENUMHibeYatirimHakedisDurum.ODENDI)
                {
                    sonuc.hataStr = "Hakediş ödendiği için işlem yapılamadı";
                    break;
                }

                odemeKod = item.odemeKod;
                hakedisTutari = Math.Round(item.faturaTutar / 2, 2);
            }

            if (sonuc.hataStr != "" || odemeKod == 0)
            {
                sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return sonuc;
            }

            ve.BeginTransaction();

            sonuc = to.YatirimciyiOdemedenCikart(hakedis);
            if (sonuc.islemSonuc)
                sonuc = to.YatirimciyiOdemedenCikartOdemeSatiriRakamDuzelt(odemeKod, hakedisTutari);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            //TRMHIBEMAKINAODEME tablosundadaki rakamları güncelle
            return sonuc;
        }
    }
}
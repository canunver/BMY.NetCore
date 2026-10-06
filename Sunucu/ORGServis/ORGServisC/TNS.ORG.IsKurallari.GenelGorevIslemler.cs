using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.ORG.IsKurallari.Veri;
using System.Data;
using System.Collections;
using TNS.KYM;

namespace TNS.ORG.IsKurallari
{
    public class GenelGorevIslemler
    {
        internal static BirimGenelGorev GenelGorevOku(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim goreveSahipOrgBirim, int siraNo, bool detayVar)
        {
            int orgKod;
            if (goreveSahipOrgBirim == null) orgKod = 0;
            else orgKod = goreveSahipOrgBirim.kod;
            BirimGenelGorev bgg = null;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            IDataReader r = tab.Oku(orgKod, siraNo);
            if (r == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            if (r.Read())
            {
                bgg = new BirimGenelGorev(VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.GOREVSIRANO, 0), VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.AD, ""));
                bgg.aciklama = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.ACIKLAMA, "");
                bgg.maxGorev = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.MAKSGOREV, -1);
                bgg.zorunlu = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.ZORUNLU, -1) > 0;
                bgg.orgBirim = goreveSahipOrgBirim;
            }
            r.Close();
            if (detayVar)
            {
                if (bgg == null)
                {
                    bgg = new BirimGenelGorev(siraNo, "");
                }

                ArrayList al;

                r = tab.BirimTurIliskiListele(orgKod, siraNo);
                if (r == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }

                al = new ArrayList();
                while (r.Read())
                {
                    al.Add((BirimTip)VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlarBIRIMTUR.BIRIMTIP, 0));
                }
                r.Close();

                bgg.iliskiliBirimler = (TNS.ORG.BirimTip[])al.ToArray(typeof(TNS.ORG.BirimTip));

                r = tab.ListeleIslemler(orgKod, siraNo, 0);
                if (r == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }

                al = new ArrayList();
                while (r.Read())
                {
                    YazilimYetki yy = new YazilimYetki();
                    yy.siraNo = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlarISLEMLER.YAZSIRANO, 0);
                    yy.birimGorevlerIndis = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlarISLEMLER.ISLEMKOD, 0);
                    yy.gorevAciklama = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlarISLEMLER.ISLEMAD, "");

                    al.Add(yy);
                }
                r.Close();

                bgg.yazilimYetkiler = (TNS.ORG.YazilimYetki[])al.ToArray(typeof(TNS.ORG.YazilimYetki));
                if (bgg.orgBirim != null)
                    bgg.orgBirim = ORGBirim.KayitVer(ve, bgg.orgBirim, false, true);
            }
            ve.VeriErisimKapat();

            return bgg;
        }

        internal static BirimGenelGorev[] GenelGorevListele(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim orgBirim, bool aktifler, int gorevSiraNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            ArrayList al = new ArrayList();
            if (orgBirim == null)
            {
                IDataReader r = tab.Listele(0, aktifler, gorevSiraNo);
                if (r == null)
                {
                    ve.VeriErisimKapat();
                    throw new Exception(ve.hata);
                }

                AlyeEkleKapat(r, al, null);
            }
            else
            {
                try
                {
                    orgBirim = IsKurallari.ORGBirim.KayitVer(null, orgBirim, false, true);

                    TNS.ORG.BirimTip[] birimTipler = BirimTipBul(orgBirim);
                    IDataReader r;
                    if (birimTipler != null && birimTipler.Length > 0)
                    {
                        r = tab.GenelListele(birimTipler, gorevSiraNo);
                        if (r == null)
                        {
                            ve.VeriErisimKapat();
                            throw new Exception(ve.hata);
                        }
                        AlyeEkleKapat(r, al, null);
                    }
                    r = tab.Listele(orgBirim.kod, aktifler, gorevSiraNo);
                    if (r == null)
                    {
                        ve.VeriErisimKapat();
                        throw new Exception(ve.hata);
                    }

                    AlyeEkleKapat(r, al, orgBirim);

                }
                catch (Exception ex)
                {
                    ve.VeriErisimKapat();
                    throw ex;
                }
                int orgKod = orgBirim.kod;
            }

            ve.VeriErisimKapat();
            return (BirimGenelGorev[])al.ToArray(typeof(BirimGenelGorev));
        }

        private static void AlyeEkleKapat(IDataReader r, ArrayList al, OrganizasyonBirim orgBirim)
        {
            if (r != null)
            {
                while (r.Read())
                {
                    BirimGenelGorev bgg = new BirimGenelGorev(VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.GOREVSIRANO, 0), VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.AD, ""));
                    bgg.orgBirim = orgBirim;
                    bgg.aciklama = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.ACIKLAMA, "");
                    bgg.maxGorev = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.MAKSGOREV, -1);
                    bgg.zorunlu = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlar.ZORUNLU, -1) > 0;
                    al.Add(bgg);
                }
                r.Close();
            }
        }

        /// <summary>
        /// Org birimden, yazılım yetki birim tipi bul
        /// </summary>
        /// <param name="orgBirim"></param>
        /// <returns></returns>
        private static BirimTip[] BirimTipBul(OrganizasyonBirim orgBirim)
        {
            if (orgBirim == null) return null;
            return orgBirim.YazilimYetkiBirimTipBul();

        }

        internal static BirimGenelGorev YeniGorevEkle(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim orgBirim)
        {
            Sonuc s = KullananBirimeKayitYapabilirMi(kullanan, orgBirim);
            if (!s.islemSonuc)
                throw new Exception(s.hataStr);
            int orgKod;
            if (orgBirim == null) orgKod = 0;
            else orgKod = orgBirim.kod;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            int sonSayi = tab.SonSayiBul(orgKod);
            if (sonSayi <= 0) sonSayi = 1;
            else sonSayi++;
            BirimGenelGorev bg = new BirimGenelGorev(sonSayi, "YENİ GÖREV");
            bg.orgBirim = orgBirim;

            ve.BeginTransaction();
            s = tab.Ekle(bg);
            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            if (s.islemSonuc)
                return bg;
            else
                throw new Exception(s.hataStr);
        }

        internal static Sonuc GenelGorevKaydet(VeriErisim veriErisim, Kullanici kullanan, BirimGenelGorev birimGenelGorev)
        {
            if (birimGenelGorev == null) birimGenelGorev = new BirimGenelGorev();
            if (birimGenelGorev.orgBirim == null) birimGenelGorev.orgBirim = new OrganizasyonBirim();

            Sonuc s = KullananBirimeKayitYapabilirMi(kullanan, birimGenelGorev.orgBirim);
            if (!s.islemSonuc)
                return s;
            int orgKod;
            if (birimGenelGorev.orgBirim == null) orgKod = 0;
            else orgKod = birimGenelGorev.orgBirim.kod;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            ve.BeginTransaction();

            s = tab.Guncelle(birimGenelGorev);
            if (s.islemSonuc)
                tab.SilBirimTur(orgKod, birimGenelGorev.siraNo);
            if (birimGenelGorev.iliskiliBirimler != null)
            {
                for (int i = 0; i < birimGenelGorev.iliskiliBirimler.Length && s.islemSonuc; i++)
                {
                    s = tab.BirimTurIliskiEkle(orgKod, birimGenelGorev.siraNo, birimGenelGorev.iliskiliBirimler[i]);
                }
            }

            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static GenelGorevYetki GenelGorevYetkiOku(VeriErisim veriErisim, Kullanici kullanan, BirimGenelGorev birimGenelGorev, int yazYetkiSiraNo)
        {
            GenelGorevYetki ggy = null;
            if (yazYetkiSiraNo == 0) return ggy;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            IDataReader r = tab.ListeleIslemler(birimGenelGorev.OrgKod, birimGenelGorev.siraNo, yazYetkiSiraNo);
            if (r == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            if (r.Read())
            {
                ggy = new GenelGorevYetki();
                ggy.yetkiAd = VeriErisim.DegerAl(r, (int)TABGENELGOREV.ENUMAlanlarISLEMLER.ISLEMAD, "");

            }
            r.Close();
            if (ggy == null) return ggy;

            r = tab.DamgalariListele(birimGenelGorev, yazYetkiSiraNo);
            if (r == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            ggy.yetkiDamgalar = new Hashtable();
            while (r.Read())
            {
                ggy.yetkiDamgalar.Add(VeriErisim.DegerAl(r, 0, ""), VeriErisim.DegerAl(r, 1, ""));
            }
            r.Close();
            ve.VeriErisimKapat();

            return ggy;
        }

        internal static Sonuc GenelGorevYetkiKaydet(VeriErisim veriErisim, Kullanici kullanan, GenelGorevYetki genelGorevYetki)
        {
            Sonuc s = KullananBirimeKayitYapabilirMi(kullanan, genelGorevYetki.birimGenelGorev.orgBirim);
            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            ve.BeginTransaction();
            if (genelGorevYetki.yazYetkiSiraNo == 0)
            {
                genelGorevYetki.yazYetkiSiraNo = tab.GorevYazilimSiraNoBul(genelGorevYetki.birimGenelGorev.OrgKod, genelGorevYetki.birimGenelGorev.siraNo);
                if (!string.IsNullOrEmpty(ve.hata))
                    s = new Sonuc(false, ve.hata, "");
                else
                {
                    genelGorevYetki.yazYetkiSiraNo++;
                    s = tab.YazilimIslemEkle(genelGorevYetki);
                }
            }
            else
                s = tab.YazilimIslemGuncelle(genelGorevYetki);

            if (s.islemSonuc)
                s = tab.DamgalariSil(genelGorevYetki);

            foreach (DictionaryEntry de in genelGorevYetki.yetkiDamgalar)
            {
                if (!s.islemSonuc)
                    break;
                s = tab.DamgaEkle(genelGorevYetki, (string)de.Key, (string)de.Value);
            }

            if (s.islemSonuc)
            {
                ve.Commit();
                s.anahtar = genelGorevYetki.yazYetkiSiraNo.ToString();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc GenelGorevYetkiSil(VeriErisim veriErisim, Kullanici kullanan, int orgKod, int gorevSiraNo, int yazSiraNo)
        {
            GenelGorevYetki genelGorevYetki = new GenelGorevYetki();
            genelGorevYetki.birimGenelGorev = new BirimGenelGorev(gorevSiraNo, null);
            genelGorevYetki.birimGenelGorev.OrgKod = orgKod;
            genelGorevYetki.yazYetkiSiraNo = yazSiraNo;

            Sonuc s = KullananBirimeKayitYapabilirMi(kullanan, genelGorevYetki.birimGenelGorev.orgBirim);
            if (!s.islemSonuc)
                return s;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            ve.BeginTransaction();
            s = tab.DamgalariSil(genelGorevYetki);
            if (s.islemSonuc)
                s = tab.YazilimGorevYetkiSil(genelGorevYetki);

            if (s.islemSonuc)
            {
                ve.Commit();
            }
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc GenelGorevKisileriEkle(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, Personel[] personeller)
        {
            IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray oaBirimListesi = ORGBirim.Listele(ve, organizasyonBirim, false, false);
            if (!oaBirimListesi.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return oaBirimListesi.sonuc;
            }
            if (oaBirimListesi.ObjeSayisi != 1)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, "Birden fazla birim bulundu", "");
            }
            organizasyonBirim = (OrganizasyonBirim)oaBirimListesi[0];

            Sonuc s = KullananBirimeKayitYapabilirMi(kullanan, organizasyonBirim);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            TABGENELGOREV tab = new TABGENELGOREV(ve);
            BirimGenelGorev okunanGorev = null;
            try
            {
                BirimGenelGorev[] gorevListe = GenelGorevListele(ve, kullanan, organizasyonBirim, true, birimGenelGorev.siraNo);
                if (gorevListe != null && gorevListe.Length == 1)
                {
                    okunanGorev = gorevListe[0];
                }
            }
            catch (Exception ex)
            {
                s.DegerAta(ex.Message);
                ve.VeriErisimKapat();
                return s;
            }
            //BirimGenelGorev okunanGorev = GenelGorevOku(ve, kullanan, ob, birimGenelGorev.siraNo, false);
            if (okunanGorev == null)
            {
                OrganizasyonBirim ob = new OrganizasyonBirim();
                ob.kod = birimGenelGorev.OrgKod;
                okunanGorev = GenelGorevOku(ve, kullanan, ob, birimGenelGorev.siraNo, false);
                if (okunanGorev == null)
                    s.DegerAta("İstenilen görev bulunamadı!");
                else
                    s.DegerAta(string.Format("{0} görevi, {1} birimi ile ilişkilendirilemez!", okunanGorev.ad, organizasyonBirim.adi));
                ve.VeriErisimKapat();
                return s;
            }

            Sonuc ss = GenelGorevSayiKontrol(ve, kullanan, organizasyonBirim, okunanGorev, personeller.Length, true);
            if (!ss.islemSonuc)
            {
                ve.VeriErisimKapat();
                return ss;
            }

            for (int i = 0; i < personeller.Length; i++)
            {
                KullaniciVAN kv = kymServis.KullaniciOku(personeller[i].tcKimlikNo);
                if (!kv.sonuc.islemSonuc)
                {
                    if (TNS.DEG.Arac.DegiskenDegerBul(0, "/GOREVEKENINDANKULLANICIOLUSTUR") == "1")
                    {
                        Sonuc ksonuc = KullaniciOlustur(kullanan, personeller[i]);
                    }
                    else
                    {
                        kv.sonuc.DegerAta("İlgili kişiye görev atanabilmesi için önce kullanıcı olarak tanımlamanız gerekmektedir. İş görev yönetimi->Personel->Personel Özlük Bilgileri ekranından personeli bulup, Kullanıcı İşlemleri sekmesinden ilgili kişiyi kullanıcı yapabilirsiniz.");
                        ve.VeriErisimKapat();
                        return kv.sonuc;
                    }
                }
            }

            ve.BeginTransaction();

            for (int i = 0; i < personeller.Length; i++)
            {
                s = tab.GoreveKisiEkle(organizasyonBirim, birimGenelGorev, personeller[i]);
                if (!s.islemSonuc)
                    ve.hata = "";
            }

            ve.Commit();
            return new Sonuc(true, "", "");
        }

        private static Sonuc KullaniciOlustur(Kullanici kullanan, Personel personel)
        {
            IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
            Kullanici yeniKullanici = KullaniciOl(kullanan, personel);
            Sonuc sonuc = servisKYM.KullaniciKaydet(kullanan, yeniKullanici, true);

            return sonuc;
        }

        private static Kullanici KullaniciOl(Kullanici kullanan, Personel personel)
        {
            TNS.PER.IPERServis servisPER = TNS.PER.Arac.Tanimla();

            Kullanici yeniKullanici = new Kullanici();
            yeniKullanici.kullaniciKodu = personel.tcKimlikNo;
            yeniKullanici.mernis = personel.tcKimlikNo;

            TNS.PER.Personel per = new TNS.PER.Personel();
            per.tcKimlikNo = personel.tcKimlikNo;
            //***Kullanıcı Bilgileri
            ObjectArray perList = servisPER.PersonelListele(kullanan, per);
            if (perList.sonuc.islemSonuc)
            {
                if (perList.objeler.Count == 1)
                {
                    per = (TNS.PER.Personel)perList.objeler[0];
                    yeniKullanici.ad = per.ad;
                    yeniKullanici.soyad = per.soyad;
                    yeniKullanici.unvan = per.unvanAdKisa + ".";
                    yeniKullanici.aktifMi = ENUMAktiflik.AKTIF;
                    yeniKullanici.anaAdi = per.anaAd;
                    yeniKullanici.babaAdi = per.babaAd;
                    yeniKullanici.calistigiBirim = per.birimKodu;
                    yeniKullanici.calistigiKurum = per.muhasebeKodu.ToString();
                    yeniKullanici.dogumTarihi = per.dogumTarihi;
                    yeniKullanici.kullaniciTipi = new ENUMKullaniciTipi[1];
                    yeniKullanici.kullaniciTipi[0] = ENUMKullaniciTipi.MISAFIR;

                    yeniKullanici.rumuz = per.acilKisi;//Merkez bankasında kullanılıyor
                    //yeniKullanici.rumuz = "";


                    yeniKullanici.aramadaGosterme = 0;
                    yeniKullanici.sicilNo = per.sicilNo;

                    if (per.cinsiyet == 1)
                        yeniKullanici.cinsiyet = "E";
                    else if (per.cinsiyet == 2)
                        yeniKullanici.cinsiyet = "K";
                    else
                        yeniKullanici.cinsiyet = "";

                    //***Email Adresi Bilgisi
                    TNS.PER.PersonelAdres perAdres = new TNS.PER.PersonelAdres();
                    perAdres.tcKimlikNo = per.tcKimlikNo;

                    ObjectArray adresList = servisPER.PersonelAdresListele(kullanan, perAdres);
                    if (adresList.sonuc.islemSonuc)
                    {
                        if (adresList.objeler.Count >= 1)
                        {
                            perAdres = (TNS.PER.PersonelAdres)adresList.objeler[0];
                            yeniKullanici.emailAdresi = perAdres.eposta;
                            if (!string.IsNullOrWhiteSpace(perAdres.eposta))
                                yeniKullanici.kullaniciKodu = perAdres.eposta.Substring(0, perAdres.eposta.IndexOf('@'));

                            yeniKullanici.telefon = perAdres.isTel;
                            if (string.IsNullOrEmpty(yeniKullanici.telefon)) yeniKullanici.telefon = "Girilmemiş";
                        }
                    }
                }
            }

            return yeniKullanici;
        }

        private static Sonuc GenelGorevSayiKontrol(VeriErisim ve, Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev okunanGorev, int eklenenDusulen, bool ekle)
        {
            Sonuc s = new Sonuc(true, "", "");
            //Görevdeki maksimum sayıları bul ve büyük eşit varsa hata ver (-1 olmayanlar için)

            TABGENELGOREV tab = new TABGENELGOREV(ve);
            if (ekle)
            {
                if (okunanGorev.maxGorev == 0)
                {
                    s.DegerAta(string.Format("{0} görevi kullanıma kapatılmıştır!", okunanGorev.ad));
                    return s;
                }
                else if (okunanGorev.maxGorev > 0)
                {
                    int say = tab.BirimKisiSayisiBul(organizasyonBirim, okunanGorev);
                    if (say + eklenenDusulen > okunanGorev.maxGorev)
                    {
                        s.DegerAta(string.Format("{1} görevinde en fazla {0} kişi atanabilir!", say, okunanGorev.ad));
                        return s;
                    }
                }
            }
            else
            {
                if (okunanGorev.zorunlu)
                {
                    int say = tab.BirimKisiSayisiBul(organizasyonBirim, okunanGorev);
                    if (say - eklenenDusulen <= 0)
                    {
                        s.DegerAta(string.Format("{0} görevinde en az bir kişi olması gerekiyor!", okunanGorev.ad));
                        return s;
                    }
                }
            }
            return s;
        }

        internal static KisiGorev[] KisiGenelGorevleriListele(VeriErisim veriErisim, Kullanici kullanan, KisiGorev bilgi)
        {
            List<KisiGorev> donusdeger = new List<KisiGorev>();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            IDataReader dr = tab.KisiGorevListele(bilgi);
            if (dr != null)
            {
                while (dr.Read())
                {
                    KisiGorev kg = new KisiGorev();
                    kg.tcKimlikNo = VeriErisim.DegerAl(dr, 0, "");
                    kg.orgKod = VeriErisim.DegerAl(dr, 1, 0);
                    kg.gorevOrgKod = VeriErisim.DegerAl(dr, 2, 0);
                    kg.gorevSiraNo = VeriErisim.DegerAl(dr, 3, 0);
                    kg.orgAd = VeriErisim.DegerAl(dr, 4, "");
                    kg.gorevAd = VeriErisim.DegerAl(dr, 5, "");
                    donusdeger.Add(kg);
                }
                dr.Close();
                ve.VeriErisimKapat();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            return donusdeger.ToArray();
        }

        internal static Personel[] GenelGorevKisileriListele(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev)
        {
            ArrayList al = new ArrayList();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            IDataReader r = null;
            if (TNS.DEG.Arac.DegiskenDegerBul(0, "/PersonelListeKullanicidan") == "1")
                r = tab.GorevKisiListeleKullanici(organizasyonBirim, birimGenelGorev);
            else
                r = tab.GorevKisiListele(organizasyonBirim, birimGenelGorev);

            if (r == null)
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }

            while (r.Read())
            {
                Personel p = new Personel();
                p.tcKimlikNo = VeriErisim.DegerAl(r, 0, "");
                p.ad = VeriErisim.DegerAl(r, 1, "");
                p.soyad = VeriErisim.DegerAl(r, 2, "");
                al.Add(p);
            }
            r.Close();
            ve.VeriErisimKapat();

            return (Personel[])al.ToArray(typeof(Personel));
        }

        internal static Sonuc KisiGenelGorevleriSil(VeriErisim veriErisim, Kullanici kullanan, KisiGorev[] bilgiler)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            int sirano = 1;


            int ilkOrg = -1;
            int ilkGorev = -1;
            //***Kişinin silme yetkisi varm kontrolü yapılmalı ?
            foreach (KisiGorev bilgi in bilgiler)
            {
                if (bilgi.orgKod == 0)
                    hatalar.Append("SıraNo:" + sirano + " Organizasyon Kodu Sıfırdan Büyük Olmalıdır<br/>");
                else
                {
                    if (ilkOrg == -1)
                        ilkOrg = bilgi.orgKod;
                    else if (ilkOrg != bilgi.orgKod)
                    {
                        hatalar.Append("SıraNo:" + sirano + " birden fazla Organizasyon Kodu bulundu!<br/>");
                    }
                }
                //if (bilgi.gorevOrgKod == 0)
                //    hatalar.Append("SıraNo:" + sirano + " Görev Organizasyon Kodu Sıfırdan Büyük Olmalıdır<br/>");
                if (bilgi.gorevSiraNo == 0)
                    hatalar.Append("SıraNo:" + sirano + " Organizasyon Sıra Numarası Sıfırdan Büyük Olmalıdır<br/>");
                else
                {
                    if (ilkGorev == -1)
                        ilkGorev = bilgi.gorevSiraNo;
                    else if (ilkGorev != bilgi.gorevSiraNo)
                    {
                        hatalar.Append("SıraNo:" + sirano + " birden fazla Görev kodubulundu!<br/>");
                    }
                }
                if (string.IsNullOrEmpty(bilgi.tcKimlikNo))
                    hatalar.Append("SıraNo:" + sirano + " Kişi T.C Kimlik Numarası Boş Olmamalıdır<br/>");
                sirano++;
            }
            OrganizasyonBirim orgBirim = null;
            if (String.IsNullOrWhiteSpace(hatalar.ToString().Trim()))
            {
                orgBirim = new OrganizasyonBirim();
                orgBirim.kod = ilkOrg;
                Sonuc s = KullananBirimeKayitYapabilirMi(kullanan, orgBirim);
                if (!s.islemSonuc)
                {
                    return s;
                }
            }

            if (hatalar.ToString().Trim() != string.Empty)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
            }
            else
            {
                VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));

                BirimGenelGorev birimGenelGorev = new BirimGenelGorev();
                birimGenelGorev.siraNo = ilkGorev;
                Sonuc ss = GenelGorevSayiKontrol(ve, kullanan, orgBirim, birimGenelGorev, bilgiler.Length, false);
                if (!ss.islemSonuc)
                {
                    ve.VeriErisimKapat();
                    return ss;
                }

                TABGENELGOREV tab = new TABGENELGOREV(ve);
                ve.BeginTransaction();
                foreach (KisiGorev bilgi in bilgiler)
                {
                    if (!sonuc.islemSonuc) break;
                    sonuc = tab.KisidenGorevSil(bilgi);
                    if (sonuc.islemSonuc)
                        sonuc = tab.SilKisit(bilgi.orgKod, bilgi.gorevOrgKod, bilgi.gorevSiraNo, bilgi.tcKimlikNo);
                }
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = "İşlem Başarıyla Gerçekleştirildi";
                }
                else
                {
                    ve.Rollback();
                }
                ve.VeriErisimKapat();
            }

            return sonuc;
        }

        internal static Sonuc GenelGorevKisileriSil(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, Personel[] personeller)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            organizasyonBirim = ORGBirim.KayitVer(ve, organizasyonBirim, false, false);
            if (organizasyonBirim == null)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, "Birim bulunamadı!", "");
            }

            Sonuc s = KullananBirimeKayitYapabilirMi(kullanan, organizasyonBirim);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            s = GenelGorevSayiKontrol(ve, kullanan, organizasyonBirim, birimGenelGorev, personeller.Length, false);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            ve.BeginTransaction();
            for (int i = 0; i < personeller.Length; i++)
            {
                s = tab.GorevdenKisiSil(organizasyonBirim, birimGenelGorev, personeller[i]);
                if (s.islemSonuc)
                    s = tab.SilKisit(organizasyonBirim.kod, birimGenelGorev.OrgKod, birimGenelGorev.siraNo, personeller[i].tcKimlikNo);
                if (!s.islemSonuc)
                    break;
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static Kullanici PersonelGorevYetkiBul(VeriErisim veriErisim, string tcKimlikNo)
        {
            Kullanici kullanan = new Kullanici();
            Sonuc s = new Sonuc(true, "", "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            IDataReader r = tab.PersonelYetkiListeleDamgasiz(tcKimlikNo);
            ArrayList al = new ArrayList();
            if (r != null)
            {
                while (r.Read())
                {
                    int iliskiliBirimKod = VeriErisim.DegerAl(r, 0, 0);
                    int gorevSahipKod = VeriErisim.DegerAl(r, 1, 0);
                    int gorevSiraNo = VeriErisim.DegerAl(r, 2, 0);
                    int yazSiraNo = VeriErisim.DegerAl(r, 3, 0);
                    int islemKod = VeriErisim.DegerAl(r, 4, 0);
                    al.Add(new int[] { iliskiliBirimKod, gorevSahipKod, gorevSiraNo, yazSiraNo, islemKod });
                }
                r.Close();
            }
            int e_iliskiliBirimKod = -1;
            OrganizasyonBirim orgBirim = null;
            int grupSay = 0;
            foreach (int[] degerler in al)
            {
                int iliskiliBirimKod = degerler[0];
                int gorevSahipKod = degerler[1];
                int gorevSiraNo = degerler[2];
                int yazSiraNo = degerler[3];
                int islemKod = degerler[4];

                Hashtable damgalar = new Hashtable();

                r = tab.GorevDamgaListele(gorevSahipKod, gorevSiraNo, yazSiraNo);
                if (r != null)
                {
                    while (r.Read())
                    {
                        string damgaAd = VeriErisim.DegerAl(r, 0, "");
                        string damgaDeger = VeriErisim.DegerAl(r, 1, "");
                        damgalar.Add(damgaAd, damgaDeger);
                    }
                    r.Close();
                }

                if (e_iliskiliBirimKod != iliskiliBirimKod)
                {
                    e_iliskiliBirimKod = iliskiliBirimKod;
                    orgBirim = new OrganizasyonBirim();
                    orgBirim.kod = iliskiliBirimKod;
                    orgBirim = IsKurallari.ORGBirim.KayitVer(null, orgBirim, false, true);
                }

                if (orgBirim != null)
                {
                    BirimGenelGorev birimGenelGorev = new BirimGenelGorev();
                    birimGenelGorev.OrgKod = gorevSahipKod;
                    birimGenelGorev.siraNo = gorevSiraNo;
                    GorevKisitDeger[] kisitDegerler = GenelGorevKisiKisitListele(ve, null, orgBirim, birimGenelGorev, tcKimlikNo);

                    if (OkunanDamgalariEkle(grupSay, damgalar, kullanan, orgBirim, islemKod, kisitDegerler))
                        grupSay++;
                }
                else
                    SabitGorevlerArac.DebugYaz($"{kullanan.ad} için birim olmadığı için yetki ataması yapılmadı!");

            }
            if (grupSay == 0)
                kullanan = null;
            else
            {
                Kullanici islemYapan = new Kullanici();
                islemYapan.KullaniciTipiEkle(ENUMKullaniciTipi.SISTEMYONETICISI);

                TNS.PER.IPERServis perServis = TNS.PER.Arac.Tanimla();
                PER.Personel personel = new PER.Personel();
                personel.tcKimlikNo = tcKimlikNo;
                ObjectArray oa = perServis.PersonelListele(islemYapan, personel);
                if (oa.ObjeSayisi == 1)
                {
                    personel = (PER.Personel)oa[0];
                    kullanan.calistigiKurum = personel.muhasebeKodu.ToString();
                    kullanan.aktifMi = personel.aktifMi == 1 ? ENUMAktiflik.AKTIF : ENUMAktiflik.PASIF;
                }
                //.calistigiKurum
            }
            ve.VeriErisimKapat();
            return kullanan;
        }

        private static bool OkunanDamgalariEkle(int grupSay, Hashtable damgalar, Kullanici kullanan, OrganizasyonBirim orgBirim, int islemKod, GorevKisitDeger[] kisitDegerler)
        {
            if (orgBirim == null || islemKod < 0 || islemKod >= SabitGorevlerArac.birimGorevler.Length)
            {
                if(orgBirim == null)
                    SabitGorevlerArac.DebugYaz($"{kullanan.ad} için birim olmadığı için yetki ataması yapılmadı!");
                else
                    SabitGorevlerArac.DebugYaz($"{kullanan.ad} için hatalı işlem kod {islemKod} geldiğinden  yetki ataması yapılmadı!");

                return false;
            }
            //SabitGorevlerArac.DebugYaz($"{kullanan.ad}, işlem kod {islemKod}, {orgBirim.adi} için yetki ataması yapılıyor!");
            return SabitGorevlerArac.birimGorevler[islemKod].KullaniciYetkiOl(grupSay, damgalar, orgBirim, kullanan, kisitDegerler);
        }

        public static Sonuc KullananBirimeKayitYapabilirMi(Kullanici kullanan, OrganizasyonBirim orgBirim)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                return s;

            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI))
                return s;

            if (!string.IsNullOrWhiteSpace(kullanan.YetkiTipiDegeriBul(101)))
            {
                if (orgBirim != null && orgBirim.kod > 0)
                {
                    if (string.IsNullOrWhiteSpace(orgBirim.kopruKod))
                    {
                        VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
                        ObjectArray oaBirimListesi = ORGBirim.Listele(ve, orgBirim, false, false);
                        if (!oaBirimListesi.sonuc.islemSonuc)
                        {
                            ve.VeriErisimKapat();
                            return oaBirimListesi.sonuc;
                        }
                        if (oaBirimListesi.ObjeSayisi != 1)
                        {
                            ve.VeriErisimKapat();
                            return new Sonuc(false, "Birden fazla birim bulundu", "");
                        }
                        ve.VeriErisimKapat();
                        orgBirim = (OrganizasyonBirim)oaBirimListesi[0];
                    }
                    for (int i = kullanan.yetkiTipiDegerleri.MinIndex(); i <= kullanan.yetkiTipiDegerleri.MaxIndex(); i++)
                    {
                        string perYetki = kullanan.yetkiTipiDegerleri["Personel Kullanıcı Yönetir", i];
                        if (perYetki == "1")
                        {
                            string yetKopruKod = kullanan.yetkiTipiDegerleri["KopruKod", i];
                            if (string.IsNullOrEmpty(yetKopruKod)) continue;
                            if (orgBirim.kopruKod.StartsWith(yetKopruKod))
                                return s;
                        }
                    }
                }
            }
            s.DegerAta(string.Format("{0}-{1} birimine kayıt yapma yetkiniz bulunmamaktadır!", orgBirim.kod, orgBirim.adi));
            return s;
        }

        internal static Sonuc GenelGorevKisiKisitEkle(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, string tckimlikno, GorevKisitDeger[] gorevKisitlar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            organizasyonBirim = ORGBirim.KayitVer(ve, organizasyonBirim, false, false);
            if (organizasyonBirim == null)
            {
                ve.VeriErisimKapat();
                return new Sonuc(false, "Birim bulunamadı!", "");
            }

            Sonuc s = KullananBirimeKayitYapabilirMi(kullanan, organizasyonBirim);
            if (!s.islemSonuc)
            {
                ve.VeriErisimKapat();
                return s;
            }

            for (int i = 0; s.islemSonuc && i < gorevKisitlar.Length; i++)
            {
                if (!FaaliyetKodlarGecerliMi(gorevKisitlar[i], organizasyonBirim.altFaaliyetKodlar))
                {
                    s.DegerAta(string.Format("Girdiğiniz faaliyet kodlar {0} ile başlamalı!", organizasyonBirim.altFaaliyetKodlar));
                    ve.VeriErisimKapat();
                    return s;
                }
            }

            TABGENELGOREV tab = new TABGENELGOREV(ve);

            ve.BeginTransaction();

            s = tab.SilKisit(organizasyonBirim.kod, birimGenelGorev.OrgKod, birimGenelGorev.siraNo, tckimlikno);
            for (int i = 0; s.islemSonuc && i < gorevKisitlar.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(gorevKisitlar[i].deger))
                    s = tab.EkleKisit(organizasyonBirim.kod, birimGenelGorev.OrgKod, birimGenelGorev.siraNo, tckimlikno, gorevKisitlar[i].gorevKisitKod.kod, gorevKisitlar[i].deger);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        private static bool FaaliyetKodlarGecerliMi(GorevKisitDeger gorevKisitDeger, string altFaaliyetKodlar)
        {
            if (string.IsNullOrWhiteSpace(altFaaliyetKodlar)) return true;
            if (gorevKisitDeger.kod != 2) return true;
            if (string.IsNullOrWhiteSpace(gorevKisitDeger.deger)) return true;

            string[] girilenDegerler = gorevKisitDeger.deger.Replace(".", "").Split(',', ';');
            string[] altFaalDegerler = altFaaliyetKodlar.Replace(".", "").Split(',', ';');

            for (int j = 0; j < girilenDegerler.Length; j++)
            {
                if (string.IsNullOrWhiteSpace(girilenDegerler[j])) continue;
                bool gecerliVar = false;
                for (int i = 0; i < altFaalDegerler.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(altFaalDegerler[i])) continue;
                    if (FaaliyetKoduGecerliMi(altFaalDegerler[i], girilenDegerler[j])) { gecerliVar = true; break; }
                }
                if (!gecerliVar) return false;
            }
            return true;
        }

        private static bool FaaliyetKoduGecerliMi(string altFaal, string girilen)
        {
            if (girilen.StartsWith(altFaal))
            {
                return true;
            }
            return false;
        }

        internal static GorevKisitDeger[] GenelGorevKisiKisitListele(VeriErisim veriErisim, Kullanici kullanan, OrganizasyonBirim organizasyonBirim, BirimGenelGorev birimGenelGorev, string tckimlikno)
        {
            GorevKisitDeger[] l = new GorevKisitDeger[SabitGorevlerArac.GorevKisitAlanlar.Length];
            for (int i = 0; i < SabitGorevlerArac.GorevKisitAlanlar.Length; i++)
            {
                l[i] = new GorevKisitDeger();
                l[i].gorevKisitKod = SabitGorevlerArac.GorevKisitAlanlar[i];
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);

            ve.BeginTransaction();

            IDataReader dr = tab.ListeleKisit(organizasyonBirim.kod, birimGenelGorev.OrgKod, birimGenelGorev.siraNo, tckimlikno);
            if (dr != null)
            {
                while (dr.Read())
                {
                    int kisitKod = VeriErisim.DegerAl(dr, (int)TABGENELGOREV.ENUMAlanlarGENELGOREVPERSONELKISIT.KISITNO, 0);
                    string deger = VeriErisim.DegerAl(dr, (int)TABGENELGOREV.ENUMAlanlarGENELGOREVPERSONELKISIT.KISITDEGER, "");
                    int yer = SabitGorevlerArac.GorevKisitAlanlarYerBul(kisitKod);
                    if (yer >= 0) l[yer].deger = deger;
                }
                dr.Close();
            }
            else
            {
                ve.VeriErisimKapat();
                throw new Exception(ve.hata);
            }
            ve.VeriErisimKapat();
            return l;
        }

        internal static Sonuc GenelGorevKisiTemizle(VeriErisim veriErisim, Kullanici kullanan, string tcKimlikNo)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGENELGOREV tab = new TABGENELGOREV(ve);
            ve.BeginTransaction();

            Sonuc s = tab.KisiGorevleriSil(tcKimlikNo);
            if (s.islemSonuc)
            {
                s = tab.KisiTipleriSil(tcKimlikNo);
            }
            if (s.islemSonuc)
            {
                s = tab.KisiYetkiDegerleriSil(tcKimlikNo);
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }
    }
}

using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;

namespace TNS.IKY.IsKurallari
{
    class IKYRapor
    {
        public static ObjectArray DoluBosKadroListele(VeriErisim veriErisim, PersonelKadro pk)
        {
            ObjectArray pVAN2 = new ObjectArray();

            string[] sqller = new string[2];
            string[] baglar = new string[2];
            sqller[0] = TABRapor.ListeleKadroKontenjanSql(pk);
            baglar[0] = "BaglantiSatiriIKY";

            sqller[1] = TABRapor.ListelePersonelKadroKullanimiSql(pk);
            baglar[1] = "BaglantiSatiriIKY";
            TNS.UZY.IUZYServis uzy = TNS.UZY.Arac.Tanimla();
            System.Collections.SortedList sl = uzy.SorguBirlestir(sqller, baglar, new int[] { 0, 1, 2 });
            try
            {
                foreach (System.Collections.DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                    KadroDoluBos kullanim = new KadroDoluBos();
                    kullanim.siraNo = alAnahtar.IntAl(0);
                    kullanim.unvanKod = alAnahtar.IntAl(1);
                    kullanim.kadroDerece = alAnahtar.IntAl(2);

                    kullanim.unvanAd = alVeri.StrAl(0);
                    kullanim.sinifAd = alVeri.StrAl(1);
                    kullanim.kontenjan = alVeri.IntAl(2);
                    if (string.IsNullOrWhiteSpace(kullanim.unvanAd)) kullanim.unvanAd = alVeri.StrAl(3);
                    if (string.IsNullOrWhiteSpace(kullanim.sinifAd)) kullanim.sinifAd = alVeri.StrAl(4);
                    kullanim.dolu = alVeri.IntAl(5);
                    if (!pk.bosKadrolar || (pk.bosKadrolar && kullanim.bos > 0))
                        pVAN2.Ekle(kullanim);
                }
            }
            catch (Exception ex)
            {
                pVAN2.sonuc.DegerAta(ex.Message);
            }

            return pVAN2;
        }

        public static ObjectArray KadroRaporlari(VeriErisim veriErisim, Kullanici kullanan, string personelTuru, string tip, string personelKod, string organizasyonKopruKodu, int siralamaListesi)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));

            TABRapor tab = new TABRapor(ve);

            IDataReader r = null;
            try
            {
                int sira = 1;
                switch (tip)
                {
                    case "IK013":
                        {
                            r = tab.IsimliKadroListesi(personelTuru, personelKod, organizasyonKopruKodu, siralamaListesi);
                            if (r == null)
                            {
                                ve.VeriErisimKapat();
                                throw new Exception(ve.hata);
                            }
                            while (r.Read())
                            {
                                PersonelKadroRapor kk = new PersonelKadroRapor();
                                kk.siraNo = sira++;
                                kk.birim = VeriErisim.DegerAl(r, 0, "");
                                kk.isim = VeriErisim.DegerAl(r, 1, "");
                                kk.kadroDerece = VeriErisim.DegerAl(r, 2, 0);
                                kk.kEkGosterge = VeriErisim.DegerAl(r, 3, 0);
                                kk.statu = VeriErisim.DegerAl(r, 4, "");
                                pVAN.objeler.Add(kk);
                            }
                            r.Close();
                            break;
                        }
                    case "IK014":
                        {
                            r = tab.IsimsizKadroListesi(personelTuru, personelKod, organizasyonKopruKodu);
                            if (r == null)
                            {
                                ve.VeriErisimKapat();
                                throw new Exception(ve.hata);
                            }
                            while (r.Read())
                            {
                                PersonelKadroRapor kk = new PersonelKadroRapor();
                                kk.siraNo = sira++;
                                kk.sinif = VeriErisim.DegerAl(r, 0, "");
                                kk.unvan = VeriErisim.DegerAl(r, 1, "");
                                kk.adet = VeriErisim.DegerAl(r, 2, 0);
                                kk.kadroDerece = VeriErisim.DegerAl(r, 3, 0);
                                kk.kEkGosterge = VeriErisim.DegerAl(r, 4, 0);
                                kk.statu = VeriErisim.DegerAl(r, 5, "");
                                pVAN.objeler.Add(kk);
                            }
                            r.Close();
                            break;
                        }
                    case "IK036":
                    case "IK015":
                        {
                            r = tab.KadroDereceDurumListesi(personelTuru, personelKod, organizasyonKopruKodu, siralamaListesi);
                            if (r == null)
                            {
                                ve.VeriErisimKapat();
                                throw new Exception(ve.hata);
                            }
                            while (r.Read())
                            {
                                PersonelKadroRapor kk = new PersonelKadroRapor();
                                kk.siraNo = sira++;
                                kk.isim = VeriErisim.DegerAl(r, 0, "");
                                kk.unvan = VeriErisim.DegerAl(r, 1, "");
                                kk.statu = VeriErisim.DegerAl(r, 2, "");
                                kk.kadroDerece = VeriErisim.DegerAl(r, 3, 0);
                                kk.kDerece = VeriErisim.DegerAl(r, 4, 0);
                                kk.kKademe = VeriErisim.DegerAl(r, 5, 0);
                                kk.kEkGosterge = VeriErisim.DegerAl(r, 6, 0);
                                kk.eDerece = VeriErisim.DegerAl(r, 7, 0);
                                kk.eKademe = VeriErisim.DegerAl(r, 8, 0);
                                kk.eEkGosterge = VeriErisim.DegerAl(r, 9, 0);
                                kk.personelKod = VeriErisim.DegerAl(r, 10, 0);
                                kk.derece.ogrenim = VeriErisim.DegerAl(r, 11, "");
                                kk.derece.derece = VeriErisim.DegerAl(r, 12, 0);
                                pVAN.objeler.Add(kk);
                            }
                            r.Close();
                            break;
                        }
                    case "IK042":
                        {
                            r = tab.IsimliUnvanListesi(personelTuru, personelKod, organizasyonKopruKodu);
                            if (r == null)
                            {
                                ve.VeriErisimKapat();
                                throw new Exception(ve.hata);
                            }
                            while (r.Read())
                            {
                                PersonelKadroRapor kk = new PersonelKadroRapor();
                                kk.siraNo = sira++;
                                kk.sinif = VeriErisim.DegerAl(r, 0, "");
                                kk.unvan = VeriErisim.DegerAl(r, 1, "");
                                kk.isim = VeriErisim.DegerAl(r, 2, "");
                                pVAN.objeler.Add(kk);
                            }
                            r.Close();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                ve.VeriErisimKapat();
                pVAN.sonuc.islemSonuc = false;
                pVAN.sonuc.hataStr = ex.Message;

                return pVAN;
            }

            return pVAN;
        }

        public static ObjectArray BakmaklaYukumluListesi(VeriErisim veriErisim, Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int yukumlu, int aktiflik)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRapor tab = new TABRapor(ve);

            IDataReader r = tab.BakmaklaYukumluListesi(personelKod, personelTur, organizasyonKopruKodu, yukumlu, aktiflik);

            if (r == null)
            {
                pVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return pVAN;
            }

            while (r.Read())
            {
                PersonelAile bilgi = new PersonelAile();
                bilgi.personelKod = VeriErisim.DegerAl(r, 0, 0);
                bilgi.personelAdi = VeriErisim.DegerAl(r, 1, "");
                bilgi.unvan = VeriErisim.DegerAl(r, 2, "");
                bilgi.ad = VeriErisim.DegerAl(r, 3, "");
                bilgi.ogrenimAd = VeriErisim.DegerAl(r, 4, "");
                bilgi.dogumTarihi = VeriErisim.DegerAl(r, 5);
                bilgi.durumKayitKod = VeriErisim.DegerAl(r, 6, "");
                bilgi.yas = VeriErisim.DegerAl(r, 7, 0);
                bilgi.yakinlik = (ENUMYakinlikDerecesi)VeriErisim.DegerAl(r, 8, 0);
                pVAN.objeler.Add(bilgi);
            }
            r.Close();
            ve.VeriErisimKapat();
            return pVAN;
        }

        public static ObjectArray OgrenimDurumuListesi(VeriErisim veriErisim, Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRapor tab = new TABRapor(ve);

            IDataReader r = tab.OgrenimDurumuListesi(personelKod, personelTur, organizasyonKopruKodu, aktiflik);
            if (r == null)
            {
                pVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return pVAN;
            }

            int say = 1;
            while (r.Read())
            {
                OgrenimRaporu bilgi = new OgrenimRaporu();
                bilgi.siraNo = say++;
                bilgi.okulAdi = VeriErisim.DegerAl(r, 0, "");
                bilgi.okulTipi = VeriErisim.DegerAl(r, 1, "");
                bilgi.personelAd = VeriErisim.DegerAl(r, 2, "");
                bilgi.unvan = VeriErisim.DegerAl(r, 3, "");
                bilgi.birim = VeriErisim.DegerAl(r, 4, "");
                bilgi.yasGunu = VeriErisim.DegerAl(r, 5, 0);
                bilgi.kurumHizmetGunu = VeriErisim.DegerAl(r, 6, 0);
                bilgi.statu = VeriErisim.DegerAl(r, 7, "");
                pVAN.objeler.Add(bilgi);
            }
            r.Close();
            ve.VeriErisimKapat();
            return pVAN;
        }

        public MailImzaRapor MailImzaRaporu(int personelKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            TABRapor tab = new TABRapor(ve);
            IDataReader r = null;
            MailImzaRapor imza = new MailImzaRapor();

            try
            {
                r = tab.MailImzaRaporu(personelKod);
                if (r.Read())
                {
                    imza.isim = VeriErisim.DegerAl(r, 0, "");
                    imza.isTel = VeriErisim.DegerAl(r, 1, "");
                    imza.cepTel = VeriErisim.DegerAl(r, 2, "");
                    imza.fax = VeriErisim.DegerAl(r, 3, "");
                    imza.unvan = VeriErisim.DegerAl(r, 4, "");
                    imza.birim = VeriErisim.DegerAl(r, 5, "");
                }
            }
            catch
            {
                ve.VeriErisimKapat();
                return imza;
            }

            return imza;
        }

        public static ObjectArray IzinListesi(VeriErisim veriErisim, Kullanici kullanan, TNS.IKY.Personel personel)
        {
            return TNS.IKY.IsKurallari.IKYPersonel.Listele(null, kullanan, personel, false, false);
            //IDataReader r = tab.IzinliListesi(personelKod, personelTur, organizasyonKopruKodu, izinTurler, null, null, siralamaListesi, aktiflik);
        }

        public static ObjectArray KartListesi(VeriErisim veriErisim, Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, string raporTuru)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRapor tab = new TABRapor(ve);

            IDataReader r = tab.KartListesi(personelKod, personelTur, organizasyonKopruKodu, raporTuru);

            if (r == null)
            {
                pVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return pVAN;
            }
            int say = 1;
            while (r.Read())
            {
                PersonelKart bilgi = new PersonelKart();
                bilgi.siraNo = say++;
                bilgi.kartTipiAd = VeriErisim.DegerAl(r, 0, "");
                bilgi.personelAd = VeriErisim.DegerAl(r, 1, "");
                bilgi.unvan = VeriErisim.DegerAl(r, 2, "");
                bilgi.yakinlikAd = TNS.IKY.Arac.YakinlikAd((ENUMYakinlikDerecesi)VeriErisim.DegerAl(r, 3, 0));
                bilgi.adiSoyadi = VeriErisim.DegerAl(r, 4, "");
                bilgi.kartNo = VeriErisim.DegerAl(r, 5, "");
                bilgi.plaka = VeriErisim.DegerAl(r, 6, "");
                bilgi.tagCihazNo = VeriErisim.DegerAl(r, 7, "");
                bilgi.zimmetTarihi = VeriErisim.DegerAl(r, 8);
                bilgi.iadeTarihi = VeriErisim.DegerAl(r, 9);
                bilgi.kayipTarihi = VeriErisim.DegerAl(r, 10);
                pVAN.objeler.Add(bilgi);
            }
            r.Close();
            ve.VeriErisimKapat();

            return pVAN;
        }

        public static ObjectArray LojmanListesiRaporu(VeriErisim veriErisim, Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, int siteKod, int tahsisTip, int siralamaListesi, int yil1, int yil2)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRapor tab = new TABRapor(ve);

            IDataReader r = tab.LojmanListesiRaporu(personelKod, personelTur, organizasyonKopruKodu, aktiflik, siteKod, tahsisTip, siralamaListesi, yil1, yil2);

            if (r == null)
            {
                pVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return pVAN;
            }
            int say = 1;
            while (r.Read())
            {
                PersonelLojmani bilgi = new PersonelLojmani();
                bilgi.siraNo = say++;
                bilgi.personelAd = VeriErisim.DegerAl(r, 0, "");
                bilgi.personelMail = VeriErisim.DegerAl(r, 1, "");
                bilgi.personelTel = VeriErisim.DegerAl(r, 2, "");
                bilgi.basTarihi = VeriErisim.DegerAl(r, 3);
                bilgi.daireNo = VeriErisim.DegerAl(r, 4, 0);
                bilgi.blokNo = VeriErisim.DegerAl(r, 5, "");
                bilgi.siteAdi = VeriErisim.DegerAl(r, 6, "");
                bilgi.daireTipiAd = VeriErisim.DegerAl(r, 7, "");
                bilgi.oturmaGunu = VeriErisim.DegerAl(r, 8, 0);
                bilgi.rezervTarih = VeriErisim.DegerAl(r, 9);
                bilgi.unvanAd = VeriErisim.DegerAl(r, 10, "");
                bilgi.lojmanTipiEtiketi = VeriErisim.DegerAl(r, 11, "");
                if (tahsisTip == (int)ENUMTahsisTipi.Rezervasyon)
                {
                    bilgi.oturduguLojmanSayisi = VeriErisim.DegerAl(r, 12, 0);
                    bilgi.oncelikDerecesi = VeriErisim.DegerAl(r, 13, 0);
                }
                pVAN.objeler.Add(bilgi);
            }
            r.Close();
            ve.VeriErisimKapat();

            return pVAN;
        }

        //public static ObjectArray LojmanDegisiklikListesiRaporu(VeriErisim veriErisim, Kullanici kullanan, int aktiflik, int siteKod)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    TABRapor tab = new TABRapor(ve);

        //    IDataReader r = tab.LojmanDegisiklikListesiRaporu(aktiflik, siteKod);

        //    if (r == null)
        //    {
        //        pVAN.sonuc.DegerAta(ve.hata);
        //        ve.VeriErisimKapat();
        //        return pVAN;
        //    }
        //    int say = 1;
        //    while (r.Read())
        //    {
        //        PersonelLojmani bilgi = new PersonelLojmani();
        //        bilgi.siraNo = say++;
        //        bilgi.personelAd = VeriErisim.DegerAl(r, 0, "");
        //        bilgi.personelMail = VeriErisim.DegerAl(r, 1, "");
        //        bilgi.personelTel = VeriErisim.DegerAl(r, 2, "");
        //        bilgi.basTarihi = VeriErisim.DegerAl(r, 3);
        //        bilgi.daireNo = VeriErisim.DegerAl(r, 4, 0);
        //        bilgi.blokNo = VeriErisim.DegerAl(r, 5, "");
        //        bilgi.siteAdi = VeriErisim.DegerAl(r, 6, "");
        //        bilgi.daireTipiAd = VeriErisim.DegerAl(r, 7, "");
        //        bilgi.bitTarihi = VeriErisim.DegerAl(r, 8);
        //        bilgi.personelKod = VeriErisim.DegerAl(r, 9, 0);
        //        pVAN.objeler.Add(bilgi);
        //    }
        //    r.Close();
        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}

        public static ObjectArray PersonelSiteKesintiRaporu(VeriErisim veriErisim, Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, int yil, int ay, int siteKod)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRapor tab = new TABRapor(ve);

            IDataReader r = tab.PersonelSiteKesintiRaporu(personelKod, personelTur, organizasyonKopruKodu, aktiflik, yil, ay, siteKod);

            if (r == null)
            {
                pVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return pVAN;
            }
            int say = 1;
            while (r.Read())
            {
                PersonelSiteKesinti bilgi = new PersonelSiteKesinti();
                bilgi.siraNo = say++;
                bilgi.pLojmanKod = VeriErisim.DegerAl(r, 0, 0);
                bilgi.personelKod = VeriErisim.DegerAl(r, 1, 0);
                bilgi.personelAd = VeriErisim.DegerAl(r, 2, "");
                bilgi.personelMail = VeriErisim.DegerAl(r, 3, "");
                bilgi.unvanAd = VeriErisim.DegerAl(r, 4, "");
                bilgi.giderTurAd = VeriErisim.DegerAl(r, 5, "");
                bilgi.tutar = VeriErisim.DegerAl(r, 6, 0.0);
                bilgi.TC = VeriErisim.DegerAl(r, 7, "");
                bilgi.daireNo = VeriErisim.DegerAl(r, 8, 0);
                bilgi.blokNo = VeriErisim.DegerAl(r, 9, "");
                bilgi.siteAdi = VeriErisim.DegerAl(r, 10, "");
                bilgi.yil = VeriErisim.DegerAl(r, 11, 0);
                bilgi.ay = VeriErisim.DegerAl(r, 12, 0);
                bilgi.giderKod = VeriErisim.DegerAl(r, 13, 0);
                bilgi.siteKod = VeriErisim.DegerAl(r, 14, 0);
                pVAN.objeler.Add(bilgi);
            }
            r.Close();
            ve.VeriErisimKapat();

            return pVAN;
        }

        public static ObjectArray LojmanDaireBazliRaporu(VeriErisim veriErisim, Kullanici kullanan, int siteKod, int daireBosluk)
        {
            ObjectArray pVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

            TABRapor tab = new TABRapor(ve);

            IDataReader r = tab.LojmanDaireBazliRaporu(siteKod, daireBosluk);

            if (r == null)
            {
                pVAN.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return pVAN;
            }
            int say = 1;
            while (r.Read())
            {
                PersonelLojmani bilgi = new PersonelLojmani();
                bilgi.siraNo = say++;
                bilgi.daireKod = VeriErisim.DegerAl(r, 0, 0);
                bilgi.daireNo = VeriErisim.DegerAl(r, 1, 0);
                bilgi.daireTipiAd = VeriErisim.DegerAl(r, 2, "");
                bilgi.metrekare = VeriErisim.DegerAl(r, 3, 0);
                bilgi.blokNo = VeriErisim.DegerAl(r, 4, "");
                bilgi.siteAdi = VeriErisim.DegerAl(r, 5, "");
                bilgi.personelAd = VeriErisim.DegerAl(r, 6, "");
                bilgi.siteKod = VeriErisim.DegerAl(r, 7, 0);
                bilgi.bitTarihi = VeriErisim.DegerAl(r, 8);
                bilgi.oturmaGunu = VeriErisim.DegerAl(r, 9, 0);
                bilgi.unvanAd = VeriErisim.DegerAl(r, 10, "");
                bilgi.lojmanTipiEtiketi = VeriErisim.DegerAl(r, 11, "");

                pVAN.objeler.Add(bilgi);
            }
            r.Close();
            ve.VeriErisimKapat();

            return pVAN;
        }

        //public static ObjectArray SirtlikListesi(VeriErisim veriErisim, Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));

        //    TABRapor tab = new TABRapor(ve);

        //    IDataReader r = tab.SirtlikListesi(personelKod, personelTur, organizasyonKopruKodu);

        //    if (r == null)
        //    {
        //        pVAN.sonuc.DegerAta(ve.hata);
        //        ve.VeriErisimKapat();
        //        return pVAN;
        //    }

        //    while (r.Read())
        //    {
        //        PersonelResim resim = new PersonelResim();
        //        resim.kod = VeriErisim.DegerAl(r, 0, 0);
        //        resim.personelAd = VeriErisim.DegerAl(r, 1, "");
        //        object rb = r.GetValue(2);
        //        if (rb != DBNull.Value)
        //            resim.resim = (byte[])r.GetValue(2);
        //        else
        //            resim.resim = null;
        //        resim.sicilNo = VeriErisim.DegerAl(r, 3, "");
        //        resim.kimlikNo = VeriErisim.DegerAl(r, 4, "");
        //        pVAN.objeler.Add(resim);
        //    }
        //    r.Close();
        //    ve.VeriErisimKapat();
        //    return pVAN;
        //}

        //public static ObjectArray DinamikRaporListesi(string personelKod, string personelTur, string organizasyonKopruKodu, string siralamalar, int aktiflik)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));

        //    TABRapor tab = new TABRapor(ve);

        //    IDataReader r = null;
        //    try
        //    {
        //        r = tab.DinamikRaporListesi(personelKod, personelTur, organizasyonKopruKodu, siralamalar, aktiflik);
        //        while (r.Read())
        //        {
        //            DinamikRapor dinamik = new DinamikRapor();
        //            object rb = r.GetValue(0);
        //            if (rb != DBNull.Value)
        //                dinamik.resim = (byte[])rb;
        //            else
        //                dinamik.resim = null;
        //            dinamik.personelKod = VeriErisim.DegerAl(r, 1, 0);
        //            dinamik.personelAd = VeriErisim.DegerAl(r, 2, "");
        //            dinamik.personelSoyad = VeriErisim.DegerAl(r, 3, "");
        //            dinamik.personelTC = VeriErisim.DegerAl(r, 4, "");
        //            dinamik.sicilNo = VeriErisim.DegerAl(r, 5, "");
        //            dinamik.unvan = VeriErisim.DegerAl(r, 6, "");
        //            dinamik.birim = VeriErisim.DegerAl(r, 7, "");
        //            dinamik.baslamaTarihi = VeriErisim.DegerAl(r, 8);
        //            dinamik.ayrilisTarihi = VeriErisim.DegerAl(r, 9);
        //            dinamik.cepTel = VeriErisim.DegerAl(r, 10, "");
        //            dinamik.dahiliTel = VeriErisim.DegerAl(r, 11, "");
        //            dinamik.personelBabaAdi = VeriErisim.DegerAl(r, 12, "");
        //            dinamik.kadroDerece = VeriErisim.DegerAl(r, 13, 0);
        //            dinamik.ekGosterge = VeriErisim.DegerAl(r, 14, 0);
        //            dinamik.dogumYeri = VeriErisim.DegerAl(r, 15, "");
        //            dinamik.dogumTarihi = VeriErisim.DegerAl(r, 16);
        //            dinamik.nufusIlcesi = VeriErisim.DegerAl(r, 17, "");
        //            dinamik.ogrenimDurumu = VeriErisim.DegerAl(r, 18, "");
        //            dinamik.geldigiKurum = VeriErisim.DegerAl(r, 19, "");
        //            dinamik.okulAdi = VeriErisim.DegerAl(r, 20, "");
        //            dinamik.tip = VeriErisim.DegerAl(r, 21, "");
        //            pVAN.objeler.Add(dinamik);
        //        }
        //        //string s = @"u.AD as unvan, b.ADI as birim, k.AtamaTarihi, k.AYRILMATARIHI, i.CEPTELEFON, i.ISTELEFON, n.BABAAD, k.KADRODERECE, k.KAZANILMISEKGOSTERGE, n.DOGUMYERI, n.DOGUMTARIHI,o.AD, s.KAYITAD as ogrenim, p.GELDIGIKURUM, og.OKULADI";
        //    }
        //    catch (Exception ex)
        //    {
        //        ve.VeriErisimKapat();
        //        pVAN.sonuc.islemSonuc = false;
        //        pVAN.sonuc.hataStr = ex.Message;

        //        return pVAN;
        //    }

        //    return pVAN;
        //}

        public Personel NakilBildirimRaporu(int pkod)
        {
            Personel personel = new Personel();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(null));
            TABRapor tab = new TABRapor(ve);
            IDataReader r = null;

            try
            {
                r = tab.NakilBidirimRaporu(pkod);
                if (r.Read())
                {
                    personel.ad = VeriErisim.DegerAl(r, 0, "");
                    personel.sicilNo = VeriErisim.DegerAl(r, 1, "");
                    personel.emekliSicilNo = VeriErisim.DegerAl(r, 2, "");
                    personel.tcKimlikNo = VeriErisim.DegerAl(r, 3, "");
                    personel.kadro.unvanAd = VeriErisim.DegerAl(r, 4, "");
                    personel.kadro.birimAd = VeriErisim.DegerAl(r, 5, "");
                    personel.kadro.kazanilmisHakDerece = VeriErisim.DegerAl(r, 6, 0);
                    personel.kadro.kazanilmisHakKademe = VeriErisim.DegerAl(r, 7, 0);
                    personel.kadro.emekliyeEsasDerece = VeriErisim.DegerAl(r, 8, 0);
                    personel.kadro.emekliyeEsasKademe = VeriErisim.DegerAl(r, 9, 0);
                    personel.kadro.atamaTarihi = VeriErisim.DegerAl(r, 10);
                    personel.kadro.ayrilmaTarihi = VeriErisim.DegerAl(r, 11);
                    personel.kullanilanIzinGun = VeriErisim.DegerAl(r, 12, 0);
                    //personel.izin[0].devreden = VeriErisim.DegerAl(r, 13, 0);
                    personel.nufus.medeniHaliAd = VeriErisim.DegerAl(r, 13, "");
                    personel.aile[0].cocukSayisi = VeriErisim.DegerAl(r, 14, 0);
                    personel.aile[0].durumAd = VeriErisim.DegerAl(r, 15, "");
                    personel.ogrenim[0].okulAdi = VeriErisim.DegerAl(r, 16, "");
                    personel.kadro.kidemGunu = VeriErisim.DegerAl(r, 17, 0);
                }
            }
            catch (Exception ex)
            {
                ve.VeriErisimKapat();
                throw ex;
            }
            return personel;
        }

        //public static ObjectArray YillarArasiLojmandaOturanlarListesiRaporu(VeriErisim veriErisim, Kullanici kullanan, int aktiflik, int siteKod, int yil1, int yil2)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    TABRapor tab = new TABRapor(ve);

        //    IDataReader r = tab.YillarArasiLojmandaOturanlarListesiRaporu(aktiflik, siteKod, yil1, yil2);

        //    if (r == null)
        //    {
        //        pVAN.sonuc.DegerAta(ve.hata);
        //        ve.VeriErisimKapat();
        //        return pVAN;
        //    }
        //    int say = 1;
        //    while (r.Read())
        //    {
        //        PersonelLojmani bilgi = new PersonelLojmani();
        //        bilgi.siraNo = say++;
        //        bilgi.personelAd = VeriErisim.DegerAl(r, 0, "");
        //        bilgi.personelMail = VeriErisim.DegerAl(r, 1, "");
        //        bilgi.personelTel = VeriErisim.DegerAl(r, 2, "");
        //        bilgi.basTarihi = VeriErisim.DegerAl(r, 3);
        //        bilgi.daireNo = VeriErisim.DegerAl(r, 4, 0);
        //        bilgi.blokNo = VeriErisim.DegerAl(r, 5, "");
        //        bilgi.siteAdi = VeriErisim.DegerAl(r, 6, "");
        //        bilgi.daireTipiAd = VeriErisim.DegerAl(r, 7, "");
        //        bilgi.bitTarihi = VeriErisim.DegerAl(r, 8);
        //        bilgi.oturmaGunu = VeriErisim.DegerAl(r, 9, 0);
        //        pVAN.objeler.Add(bilgi);
        //    }
        //    r.Close();
        //    ve.VeriErisimKapat();

        //    return pVAN;
        //}

        //public static ObjectArray YemekhanelereGoreYemekListesi(VeriErisim veriErisim, Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, string yemekhaneler, DateTime baslamaTarihi, DateTime bitisTarihi)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    //pVAN.sonuc = IKYYetki.YetkiVarMiOGY(kullanan, organizasyonKopruKodu, 0, pVAN.sonuc, MUH.ENUMOGY.Gorme, ENUMYetkiIslem.Yemek, false, null);
        //    //if (!pVAN.sonuc.islemSonuc)
        //    //    return pVAN;

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    //TABRapor tab = new TABRapor(ve);

        //    //IDataReader r = tab.YemekhanelereGoreYemekListesi(personelKod, personelTur, organizasyonKopruKodu, aktiflik, yemekhaneler, baslamaTarihi, bitisTarihi);
        //    //if (r == null)
        //    //{
        //    //    pVAN.sonuc.DegerAta(ve.hata);
        //    //    ve.VeriErisimKapat();
        //    //    return pVAN;
        //    //}

        //    //while (r.Read())
        //    //{
        //    //    IKYPersonelYemek bilgi = new IKYPersonelYemek();
        //    //    bilgi.yerleskeAdi = VeriErisim.DegerAl(r, 0, "");
        //    //    bilgi.pKod = VeriErisim.DegerAl(r, 1, 0);
        //    //    bilgi.personelAd = VeriErisim.DegerAl(r, 2, "");
        //    //    bilgi.personelMail = VeriErisim.DegerAl(r, 3, "");
        //    //    bilgi.personelUnvan = VeriErisim.DegerAl(r, 4, "");
        //    //    bilgi.personelTur = VeriErisim.DegerAl(r, 5, "");
        //    //    bilgi.adet = VeriErisim.DegerAl(r, 6, 0);
        //    //    bilgi.indirimli = VeriErisim.DegerAl(r, 7, 0);
        //    //    bilgi.indirimsiz = VeriErisim.DegerAl(r, 8, 0);
        //    //    pVAN.objeler.Add(bilgi);
        //    //}
        //    //r.Close();
        //    //ve.VeriErisimKapat();
        //    return pVAN;
        //}

        //public static ObjectArray PersonelHesaplanmisYemekListesi(VeriErisim veriErisim, Kullanici kullanan, string personelKod, string personelTur, string organizasyonKopruKodu, int aktiflik, string yemekhaneler, int yil, int ay)
        //{
        //    ObjectArray pVAN = new ObjectArray();

        //    //pVAN.sonuc = IKYYetki.YetkiVarMiOGY(kullanan, organizasyonKopruKodu, 0, pVAN.sonuc, MUH.ENUMOGY.Gorme, ENUMYetkiIslem.Yemek, false, null);
        //    //if (!pVAN.sonuc.islemSonuc)
        //    //    return pVAN;

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriIKY", TNS.KYM.Arac.KimlikBul(kullanan));

        //    //TABRapor tab = new TABRapor(ve);

        //    //IDataReader r = tab.PersonelHesaplanmisYemekListesi(personelKod, personelTur, organizasyonKopruKodu, aktiflik, yemekhaneler, yil, ay);
        //    //if (r == null)
        //    //{
        //    //    pVAN.sonuc.DegerAta(ve.hata);
        //    //    ve.VeriErisimKapat();
        //    //    return pVAN;
        //    //}

        //    //while (r.Read())
        //    //{
        //    //    IKYPersonelYemek bilgi = new IKYPersonelYemek();
        //    //    bilgi.pKod = VeriErisim.DegerAl(r, 0, 0);
        //    //    bilgi.personelAd = VeriErisim.DegerAl(r, 1, "");
        //    //    bilgi.personelMail = VeriErisim.DegerAl(r, 2, "");
        //    //    bilgi.personelUnvan = VeriErisim.DegerAl(r, 3, "");
        //    //    bilgi.personelTur = VeriErisim.DegerAl(r, 4, "");
        //    //    bilgi.adet = VeriErisim.DegerAl(r, 5, 0);
        //    //    bilgi.indirimli = VeriErisim.DegerAl(r, 6, 0);
        //    //    bilgi.indirimsiz = VeriErisim.DegerAl(r, 7, 0);
        //    //    bilgi.ucret = VeriErisim.DegerAl(r, 8, 0.0);
        //    //    bilgi.indirimliTutar = VeriErisim.DegerAl(r, 9, 0.0);
        //    //    bilgi.indirimsizTutar = VeriErisim.DegerAl(r, 10, 0.0);
        //    //    pVAN.objeler.Add(bilgi);
        //    //}
        //    //r.Close();
        //    //ve.VeriErisimKapat();
        //    return pVAN;
        //}
    }
}

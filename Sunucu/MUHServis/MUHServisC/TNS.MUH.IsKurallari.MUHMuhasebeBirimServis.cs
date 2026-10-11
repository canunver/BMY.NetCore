using System;
using System.Data;
//using MUHServisC.ebutce.Muhasebe;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// Muhasebe-Birim işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHMuhasebeBirimServis
    {
        /// <summary>
        /// Veri tabanı bağlantısı, web.config dosyasında bu bağlantı ile ile ilgili parametreler bulunmalıdır.
        /// </summary>
        public static string VERIERISIMAD = "BaglantiSatiriBUM";

        /// <summary>
        /// Kullanıcının yetkili olduğu muhasebe birimleri
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="pKurum">Kurum kodu</param>
        /// <returns></returns>
        public ObjectArray KullaniciMuhasebeBirimleri(Kullanici kullanan, int muhasebe, string pKurum)
        {
            return KullaniciMuhasebeBirimleri(kullanan, muhasebe, pKurum, null);
        }

        /// <summary>
        /// Kullanıcının yetkili olduğu muhasebe birimleri
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="pKurum">Kurum kodu</param>
        /// <returns></returns>
        public ObjectArray KullaniciMuhasebeBirimleri(Kullanici kullanan, int muhasebe, string pKurum, string kistasKodAd)
        {
            ObjectArray mv = new ObjectArray();
            bool bayrak = false;
            if (pKurum.StartsWith("12"))
                bayrak = true;

            string kurumlar = "";

            if (kullanan != null && !(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)(ENUMKullaniciTipi.ODENEKGONDERME))) && BKD.Arac.KurumMu(pKurum))
            {
                kurumlar = TNS.MUH.Yetki.OdemeEmriMIFKurumlariVer(kullanan);
                if (kurumlar != "")
                    kurumlar += ";";
                kurumlar += MUHYetkiServis.GondermeBelgesiKurumlariVer(kullanan);
                if (kurumlar != "")
                    kurumlar += ";";
                kurumlar += MUHYetkiServis.AlacakTakipKurumlariVer(kullanan);

                if (kurumlar.Trim() == "")
                    kurumlar = pKurum;
            }
            else
                kurumlar = pKurum;

            if (kurumlar == "")
            {
                mv.sonuc.hataStr = ButceMuhasebeResI.MUH119;
                mv.sonuc.islemSonuc = false;
                return mv;
            }

            ObjectArray dizi = new ObjectArray();
            ObjectArray kurum = new ObjectArray();

            List<int> muhasebeler = new List<int>();
            List<string> birimler = new List<string>();
            int muhasebeSayac = 0;
            int birimSayac = 0;

            if (kullanan != null && !(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI, (int)(ENUMKullaniciTipi.ODENEKGONDERME))))
            {
                int sayac = OrtakFonksiyonlar.StringiParcala(kurumlar, ";", dizi);
                for (int i = 0; i < sayac; i++)
                    if ((string)dizi.objeler[i] != "")
                    {
                        bool bulundu = false;
                        for (int j = 0; j < kurum.objeler.Count; j++)
                        {
                            if ((string)kurum[j] == (string)dizi.objeler[i])
                            {
                                bulundu = true;
                                break;
                            }
                        }
                        if (!bulundu)
                            kurum.objeler.Add((string)dizi.objeler[i]);
                    }

                //Yetkili olduğu muhasebeler
                string yetkiTipiAdi = "";
                yetkiTipiAdi = "Ödeme Emri/MİF Saymanlık-Birim Yetkisi";
                string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);

                //Personel yetkisi var mı?
                string yetkiTipiDegeriPersonel = MUHYetkiServis.PersonelKurumlariVer(kullanan);
                if (yetkiTipiDegeri != "")
                    yetkiTipiDegeri += ";";
                yetkiTipiDegeri += yetkiTipiDegeriPersonel;

                if (yetkiTipiDegeri.Trim() != "")
                {
                    string[] d = yetkiTipiDegeri.Split(';');

                    for (int i = 0; i < d.Length; i++)
                    {
                        string[] muhasebeBirim = d[i].Split('-');
                        if (muhasebeBirim.Length != 2)
                            continue;

                        muhasebeler.Add(OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], -1));
                        muhasebeSayac++;
                        birimler.Add(muhasebeBirim[1]);
                        birimSayac++;
                    }
                }
            }

            if (kurum.ObjeSayisi == 0 && bayrak)
                kurum.objeler.Add(kurumlar);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            IDataReader r = to.KullaniciHarcamaBirimleriGetir(kurum, muhasebe, kistasKodAd);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MuhasebeBirim muhasebeBirim = new MuhasebeBirim();
                    if (muhasebe > 0)
                    {
                        muhasebeBirim.birimKod = VeriErisim.DegerAl(r, 0, "");
                        muhasebeBirim.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        muhasebeBirim.kurum = VeriErisim.DegerAl(r, 2, "");
                        muhasebeBirim.birimAd = VeriErisim.DegerAl(r, 3, "");
                        //muhasebeBirim.vno = VeriErisim.DegerAl(r, 4, "");
                        muhasebeBirim.muhasebeAd = VeriErisim.DegerAl(r, 5, "");
                    }
                    else
                    {
                        muhasebeBirim.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                        muhasebeBirim.muhasebeAd = VeriErisim.DegerAl(r, 1, "");
                    }

                    bool vardir = false;
                    if (muhasebeSayac > 0)
                    {
                        for (int i = 0; i < muhasebeler.Count; i++)
                        {
                            if (MuhasebeGorurMu(muhasebeBirim.muhasebe, muhasebeler[i]) || muhasebeler[i] == -1)
                            {
                                vardir = true;
                                break;
                            }
                        }
                    }
                    else
                        vardir = true;

                    if (birimSayac > 0 && muhasebe > 0)//Bir muhasebe birimi gönderilmiş ise
                    {
                        vardir = false;
                        for (int i = 0; i < muhasebeler.Count; i++)
                        {
                            if (string.IsNullOrEmpty(birimler[i]))
                                break;

                            if (muhasebeBirim.birimKod == birimler[i] || birimler[i].Trim() == "*")
                            {
                                vardir = true;
                                break;
                            }
                        }
                    }

                    if (vardir)
                        mv.objeler.Add(muhasebeBirim);
                }
                r.Close();
                mv.sonuc.islemSonuc = true;
            }
            else
                mv.sonuc.islemSonuc = false;

            ve.VeriErisimKapat();

            return mv;
        }

        public static bool MuhasebeGorurMu(int muhasebe, int yetkiMuhasebe)
        {
            if (muhasebe == yetkiMuhasebe) return true;
            int ilceKod = yetkiMuhasebe % 1000;
            if (ilceKod == 100 || ilceKod == 150)
            {
                return (muhasebe / 1000) == (yetkiMuhasebe / 1000);
            }
            return false;
        }

        /// <summary>
        /// Harcama birimine ait kurumsal kod listesi
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum kodu</param>
        /// <param name="muhasebe">Muhasebe kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <returns></returns>
        public ABSKodlarVAN HarcamaBirimleriKurumsalKod(VeriErisim veriErisim, int yil, string kurum, int muhasebe, string birim)
        {
            ABSKodlarVAN kv = new ABSKodlarVAN();
            string hata = "";

            if (kurum == "")
                hata += ButceMuhasebeResI.MUH119;

            if (muhasebe == 0)
                hata += ButceMuhasebeResI.MUH120;

            if (birim == "")
                hata += ButceMuhasebeResI.MUH121;

            if (hata != "")
            {
                kv.sonuc.hataStr = hata;
                kv.sonuc.islemSonuc = false;
                return kv;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            IDataReader r = to.HarcamaBirimleriKurumsalKod(yil, kurum, muhasebe, birim);

            if (r != null)
            {
                while (r.Read())
                {
                    ABSKod kod = new ABSKod();

                    kod.kod = VeriErisim.DegerAl(r, 0, "");
                    kod.AdAta(VeriErisim.DegerAl(r, 1, ""));
                    kv.absKodlar.Add(kod);
                }
                kv.sonuc.islemSonuc = true;
                r.Close();
            }

            ve.VeriErisimKapat();

            return kv;
        }

        ObjectArray KullanicininHarcamaBirimleri(Kullanici kullanan)
        {
            ObjectArray mv = new ObjectArray();

            string kurumlar = "";

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                kurumlar = TNS.MUH.Yetki.OdemeEmriMIFKurumlariVer(kullanan);
                if (kurumlar != "")
                    kurumlar += ";";
                kurumlar += MUHYetkiServis.GondermeBelgesiKurumlariVer(kullanan);
            }

            if (kurumlar == "")
            {
                mv.sonuc.hataStr = ButceMuhasebeResI.MUH119;
                mv.sonuc.islemSonuc = false;
                return mv;
            }

            ObjectArray dizi = new ObjectArray();
            ObjectArray kurum = new ObjectArray();
            int sayac = OrtakFonksiyonlar.StringiParcala(kurumlar, ";", dizi);
            for (int i = 0; i < sayac; i++)
                if ((string)dizi.objeler[i] != "")
                    kurum.objeler.Add((string)dizi.objeler[i]);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            IDataReader r = to.KullanicininHarcamaBirimleri(kurum, false);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MuhasebeBirim muhasebeBirim = new MuhasebeBirim();

                    muhasebeBirim.birimKod = VeriErisim.DegerAl(r, 0, "");

                    mv.objeler.Add(muhasebeBirim);
                }
                r.Close();
                mv.sonuc.islemSonuc = true;
            }
            else
                mv.sonuc.islemSonuc = false;

            ve.VeriErisimKapat();

            return mv;
        }

        /// <summary>
        /// Kullanıcıya ait Harcama birimlerini listeler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yetkiTipiAdi">yetki tipi adi</param>
        /// <returns></returns>
        public ObjectArray KullanicininHarcamaBirimleri(Kullanici kullanan, string yetkiTipiAdi)
        {
            ObjectArray mv = new ObjectArray();

            string kurumlar = "";

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)))
            {
                kurumlar = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, yetkiTipiAdi);
            }

            if (kurumlar == "")
            {
                mv.sonuc.hataStr = ButceMuhasebeResI.MUH119;
                mv.sonuc.islemSonuc = false;
                return mv;
            }

            ObjectArray dizi = new ObjectArray();
            ObjectArray kurum = new ObjectArray();
            int sayac = OrtakFonksiyonlar.StringiParcala(kurumlar, ";", dizi);
            for (int i = 0; i < sayac; i++)
                if ((string)dizi.objeler[i] != "")
                    kurum.objeler.Add((string)dizi.objeler[i]);

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            IDataReader r = to.KullanicininHarcamaBirimleri(kurum, true);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    MuhasebeBirim muhasebeBirim = new MuhasebeBirim();

                    muhasebeBirim.birimKod = VeriErisim.DegerAl(r, 0, "");
                    muhasebeBirim.muhasebe = VeriErisim.DegerAl(r, 1, 0);

                    mv.objeler.Add(muhasebeBirim);
                }
                r.Close();
                mv.sonuc.islemSonuc = true;
            }
            else
                mv.sonuc.islemSonuc = false;

            ve.VeriErisimKapat();

            return mv;
        }

        /// <summary>
        /// Muhasebe birimi kaydet.
        /// </summary>
        /// <param name="muhasebeBirimler">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc Kaydet(ObjectArray muhasebeBirimler)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH122, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();

            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            TABGenel tg = new TABGenel(ve);

            foreach (MuhasebeBirim mb in muhasebeBirimler.objeler)
            {
                if (mb.birimKod != "" && mb.muhasebe > 0 && mb.kurum != "" && mb.birimAd != "")
                    sonuc = to.Kaydet(mb);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH123;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe birimi sil.
        /// </summary>
        /// <param name="muhasebeBirimler">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public Sonuc Sil(ObjectArray muhasebeBirimler)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH124, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            ve.BeginTransaction();
            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);

            foreach (MuhasebeBirim mb in muhasebeBirimler.objeler)
            {
                if (mb.birimKod != "" && mb.muhasebe > 0 && mb.kurum != "")
                    sonuc = to.Sil(mb);

                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH125;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebe birim listele.
        /// </summary>
        /// <param name="pMuhasebeBirim">İşlem yapılacak bilgilerin taşındığı nesne</param>
        /// <returns></returns>
        public ObjectArray Listele(MuhasebeBirim pMuhasebeBirim)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            IDataReader r = to.Listele(pMuhasebeBirim);

            Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABButce);

            if (r != null)
            {
                while (r.Read())
                {
                    MuhasebeBirim mb = new MuhasebeBirim();

                    mb.birimKod = VeriErisim.DegerAl(r, 0, "");
                    mb.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    mb.kurum = VeriErisim.DegerAl(r, 2, "");
                    mb.birimAd = VeriErisim.DegerAl(r, 3, "");
                    //mb.vno = VeriErisim.DegerAl(r, 4, "");
                    mb.muhasebeAd = VeriErisim.DegerAl(r, 4, "");
                    mb.pasif = VeriErisim.DegerAl(r, 5, 0); mb.vno = VeriErisim.DegerAl(r, 6, ""); /* Core: vergi no */

                    t.kodlar[0] = mb.kurum;
                    t.Noktala();
                    mb.kurum = t.kodlar[0].ToString();
                    if ((pMuhasebeBirim.pasif_0Hepsi_1Aktif_2Pasif == 0) 
                        || (pMuhasebeBirim.pasif_0Hepsi_1Aktif_2Pasif == 1 && mb.pasif == 0) 
                        || (pMuhasebeBirim.pasif_0Hepsi_1Aktif_2Pasif == 2 && mb.pasif == 1)) iVAN.objeler.Add(mb);
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return iVAN;
        }

        public static MuhasebeBirim ListeleMuhBirim(VeriErisim veriErisim, MuhasebeBirim muhBirim)
        {
            MuhasebeBirim mb = new MuhasebeBirim();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            IDataReader r = to.ListeleMuhBirim(muhBirim);

            if (r != null)
            {
                while (r.Read())
                {
                    mb.birimKod = VeriErisim.DegerAl(r, 0, "");
                    mb.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    mb.kurum = VeriErisim.DegerAl(r, 2, "");
                    mb.birimAd = VeriErisim.DegerAl(r, 3, "");
                    mb.vno = VeriErisim.DegerAl(r, 4, "");
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return mb;
        }

        ///// <summary>
        ///// Muhasebe birim bilgisini ebutceden aktar.
        ///// </summary>
        ///// <param name="kurum">Kurum kodu</param>
        ///// <param name="muhasebe">Muhasebe kodu</param>
        ///// <param name="kod">Muhasebe birim kodu</param>
        ///// <param name="ad">Muhasebe birim adı</param>
        ///// <returns></returns>
        //public Sonuc EbutcedenAktar(string kurum, string muhasebe, string kod, string ad)
        //{
        //    Sonuc sonuc = new Sonuc(false, "", "");

        //    //System.Net.ServicePointManager.CertificatePolicy = new CertificatePolicy();
        //    MUHServisC.ebutce.Muhasebe.Muhasebe mEbutce = new MUHServisC.ebutce.Muhasebe.Muhasebe();
        //    mEbutce.Proxy = OrtakFonksiyonlar.ProxyAyarla();

        //    ObjectArray muhlar = new ObjectArray();
        //    kurum = kurum.Replace(".", "");
        //    MuhasebeBirimlerW mW = mEbutce.MuhasebeBirimListele(kurum, muhasebe, kod, ad);

        //    for (int i = 0; i < mW.muhsebeBirim.Length; i++)
        //    {
        //        bool ekle = false;
        //        if (kurum.Length == 2)
        //        {
        //            if (mW.muhsebeBirim[i].kurum != "")
        //                if (mW.muhsebeBirim[i].kurum.Replace(".", "").StartsWith(kurum.Replace(".", "")))
        //                    ekle = true;
        //        }
        //        else
        //            ekle = true;

        //        if (ekle)
        //        {
        //            MuhasebeBirim muh = new MuhasebeBirim();
        //            muh.birimAd = mW.muhsebeBirim[i].birimAd;
        //            muh.birimKod = mW.muhsebeBirim[i].birimKod;
        //            muh.kurum = mW.muhsebeBirim[i].kurum;
        //            muh.muhasebe = mW.muhsebeBirim[i].muhasebe;
        //            muh.muhasebeAd = mW.muhsebeBirim[i].muhasebeAd;

        //            muhlar.objeler.Add(muh);
        //        }
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(null));

        //    TABMuhasebeBirim to = new TABMuhasebeBirim(ve);

        //    if (mW.muhsebeBirim.Length > 0)
        //    {
        //        if (muhasebe == "" && kod == "" && ad == "")
        //            sonuc = to.SilHepsini();
        //        else
        //            sonuc.islemSonuc = true;

        //        if (sonuc.islemSonuc)
        //            sonuc = Kaydet(muhlar);

        //        if (sonuc.islemSonuc)
        //            sonuc.bilgiStr = ButceMuhasebeResI.MUH126;
        //        else
        //        {
        //            sonuc.islemSonuc = false;
        //            sonuc.hataStr = ButceMuhasebeResI.MUH127;
        //        }
        //    }
        //    else
        //    {
        //        sonuc.islemSonuc = false;
        //        sonuc.hataStr = ButceMuhasebeResI.MUH127;
        //    }
        //    ve.VeriErisimKapat();

        //    return sonuc;
        //}

        /// <summary>
        /// Kullanıcının yetkili olduğu birimleri belirler
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">İşlem yapılacak bilgileri taşıyan nesne</param>
        public static void KullanicininYetkiliOlduguBirimler(Kullanici kullanan, OdemeEmriMIFForm form)
        {
            MUHMuhasebeBirimServis ms = new MUHMuhasebeBirimServis();

            if (!(kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI) ||
                kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.KULLANICIYONETICISI)))
            {
                //if (form.muhasebe == 0 || form.birim == "") KUllanıcı muhasebe ve birim bilgilerini yazarak yetkisi olmayan birimleri listeyebilir.
                {
                    if (form.muhasebe > 0 || form.birim != "")
                    {
                        if (!TNS.MUH.Yetki.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebe.ToString(), form.birim))
                        {
                            form.aciklama = "YetkisiYok";
                            return;
                        }
                    }
                    string sakliBirim = form.birim;

                    ObjectArray mV = ms.KullanicininHarcamaBirimleri(kullanan);
                    if (mV.sonuc.islemSonuc)
                    {
                        foreach (MuhasebeBirim m in mV.objeler)
                        {
                            if (form.birim != "")
                                form.birim += ";";

                            if (sakliBirim == m.birimKod)
                            {
                                form.birim = sakliBirim;
                                break;
                            }
                            form.birim += m.birimKod;
                        }
                    }

                    string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Ödeme Emri/MİF Saymanlık-Birim Yetkisi");
                    if (yetkiTipiDegeri != "")
                    {
                        ObjectArray d = new ObjectArray();
                        int parca = OrtakFonksiyonlar.StringiParcala(yetkiTipiDegeri, ";", d);
                        form.birim = sakliBirim;
                        foreach (string yetkiTipiDegeriParca in d.objeler)
                        {
                            string[] muhasebeBirim = yetkiTipiDegeriParca.Split('-');
                            if (muhasebeBirim.Length == 2)
                            {
                                if (muhasebeBirim[0].Trim() != "*")
                                {
                                    if (form.muhasebeListe != "")
                                        form.muhasebeListe += ";";
                                    form.muhasebeListe += OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[0], 0).ToString();
                                }

                                if (muhasebeBirim[1].Trim() != "*")
                                {
                                    if (form.birim != "")
                                        form.birim += ";";

                                    if (sakliBirim == OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[1], 0).ToString("000"))
                                    {
                                        form.birim = sakliBirim;
                                        break;
                                    }

                                    form.birim += OrtakFonksiyonlar.ConvertToInt(muhasebeBirim[1], 0).ToString("000");//EGO 012 gibi birimler int'e çevrildiğinde 12 geliyordu. Bu nedenle başına 0 koymak için formatlanoyor. Melih 22.06.2015
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static MuhasebeBirim[] Listele(VeriErisim veriErisim, Kullanici kullanan, int saymanlikKod, string birimTurler)
        {
            List<MuhasebeBirim> iVAN = new List<MuhasebeBirim>();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            IDataReader r = to.Listele(saymanlikKod, birimTurler);

            if (r != null)
            {
                while (r.Read())
                {
                    MuhasebeBirim mb = new MuhasebeBirim();

                    mb.birimKod = VeriErisim.DegerAl(r, 0, "");
                    mb.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    mb.kurum = VeriErisim.DegerAl(r, 2, "");
                    mb.birimAd = VeriErisim.DegerAl(r, 3, "");
                    mb.vno = VeriErisim.DegerAl(r, 4, "");
                    mb.muhasebeAd = VeriErisim.DegerAl(r, 5, "");
                    mb.orgCografyaKod = VeriErisim.DegerAl(r, 6, "");
                    string cografiAd = VeriErisim.DegerAl(r, 7, "");
                    if (cografiAd != "MERKEZ")
                    {
                        if (!string.IsNullOrEmpty(cografiAd))
                        {
                            mb.birimAd = cografiAd + " " + mb.birimAd;
                        }
                        mb.birimAd = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToLower(mb.birimAd);
                        mb.birimAd = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(mb.birimAd);
                        iVAN.Add(mb);
                    }
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return iVAN.ToArray();
        }

        internal string SaymanlikBirimKurkodBul(int saymanlik, string birim)
        {
            TNS.MUH.MuhasebeBirim muhasebeBirim = new TNS.MUH.MuhasebeBirim();
            muhasebeBirim.birimKod = birim;
            muhasebeBirim.muhasebe = saymanlik;

            ObjectArray gelenler = Listele(muhasebeBirim);
            string kurKod = "";
            if (gelenler.sonuc.islemSonuc && gelenler.ObjeSayisi > 0)
            {
                foreach (TNS.MUH.MuhasebeBirim mb in gelenler.objeler)
                {
                    if (kurKod != "") kurKod += ",";
                    kurKod += mb.kurum;
                }
            }
            if (string.IsNullOrWhiteSpace(kurKod))
                kurKod = TNS.DEG.Arac.VarsayilanKurumBul();

            return kurKod;
        }

        internal ObjectArray MuhasebeBirimListeleSablondan(Kullanici kullanan, string sablon)
        {
            ObjectArray mv = new ObjectArray();
            string kurum = TNS.DEG.Arac.VarsayilanKurumBul();
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABMuhasebeBirim to = new TABMuhasebeBirim(ve);
            IDataReader r = to.KullaniciHarcamaBirimleriGetir(kurum, sablon);

            if (r != null)
            {
                while (r.Read())
                {
                    MuhasebeBirim muhasebeBirim = new MuhasebeBirim();

                    muhasebeBirim.birimKod = VeriErisim.DegerAl(r, 0, "");
                    muhasebeBirim.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    muhasebeBirim.kurum = VeriErisim.DegerAl(r, 2, "");
                    muhasebeBirim.birimAd = VeriErisim.DegerAl(r, 3, "");
                    muhasebeBirim.muhasebeAd = VeriErisim.DegerAl(r, 4, "");

                    mv.objeler.Add(muhasebeBirim);
                }
                r.Close();
            }
            else
                mv.sonuc.DegerAta(ve.hata);
            ve.VeriErisimKapat();
            return mv;
        }


        /// <summary>
        /// MuhasebeBirim nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MuhasebeBirim yapısında MuhasebeBirim:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public static Sonuc BirimAcKapat(VeriErisim veriErisim, Kullanici kullanan, MuhasebeBirim form, bool kapat)
        {
            Sonuc s = new Sonuc(true, "", "");
            if (!kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Bu işlemi yapmaya yetkiniz bulunmamaktadır!");
                return s;
            }

            if (!(form.muhasebe > 0 && form.birimKod.Trim().Length == 3 && !string.IsNullOrWhiteSpace(form.kurum)))
            {
                s.DegerAta("Muhasebe, birim ve kurumsal kod dolu olmalıdır!");
                return s;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHMUHASEBEBIRIMKAPALI tab = new TABMUHMUHASEBEBIRIMKAPALI(ve);

            ve.BeginTransaction();

            if (s.islemSonuc)
            {
                if (kapat)
                    s = tab.Ekle(form);
                else
                    s = tab.Sil(form);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return s;
        }

        /// <summary>
        /// MuhasebeBirim listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MuhasebeBirim yapısında MuhasebeBirim:  bilgilerini tutan değişken</param>
        public static ObjectArray KapaliBirimListele(VeriErisim veriErisim, Kullanici kullanan, MuhasebeBirim form)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABMUHMUHASEBEBIRIMKAPALI tab = new TABMUHMUHASEBEBIRIMKAPALI(ve);

            ObjectArray alAna = new ObjectArray();

            IDataReader dr = tab.Lislele(form);
            if (dr == null)
            {
                alAna.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return alAna;
            }
            while (dr.Read())
            {
                MuhasebeBirim detay = new MuhasebeBirim();
                detay.muhasebe = VeriErisim.DegerAl(dr, (int)TABMUHMUHASEBEBIRIMKAPALI.ENUMAlanlar.MUHASEBEKOD, 0);
                detay.birimKod = VeriErisim.DegerAl(dr, (int)TABMUHMUHASEBEBIRIMKAPALI.ENUMAlanlar.BIRIMKOD, "");
                detay.kurum = VeriErisim.DegerAl(dr, (int)TABMUHMUHASEBEBIRIMKAPALI.ENUMAlanlar.KURKOD, "");

                detay.muhasebeAd = VeriErisim.DegerAl(dr, 3, "");
                detay.birimAd = VeriErisim.DegerAl(dr, 4, "");

                alAna.Ekle(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return alAna;
        }

        public static bool BirimKapaliMi(VeriErisim veriErisim, Kullanici kullanan, int saymanlik, string birimKod, string kurKod)
        {
            MuhasebeBirim form = new MuhasebeBirim();
            form.muhasebe = saymanlik;
            form.birimKod = birimKod;
            form.kurum = kurKod;
            ObjectArray oa = KapaliBirimListele(veriErisim, kullanan, form);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi > 0)
            {
                return true;
            }
            return false;
        }
    }
}

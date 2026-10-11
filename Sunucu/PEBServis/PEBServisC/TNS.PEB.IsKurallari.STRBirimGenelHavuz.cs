using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using TNS.BKD;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Stratejik plan mevcut durum analizinde belirlenecek olan Hizmet veya Üst Belge'nin yönetilmesi
    /// ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// seçme ve iptal etme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class BirimGenelHavuz
    {
        /// <summary>
        /// Hizmet veya Üst Belge'nin seçili olma durumunu kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen birimler nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="birimler">Hizmet veya Üst Belge bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz[] birimler, string formId)
        {
            Sonuc sonuc = new Sonuc();

            if (birimler == null || birimler.Length <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR040;
                return sonuc;
            }

            foreach (Birim_SpGenelHavuz birim in birimler)
            {
                string hata = string.Empty;
                if (birim.donem == null || birim.donem.donemNo <= 0)
                    hata = PerformansButceResI.STR032 + "<br>";
                if (birim.orgBirimKod <= 0)
                    hata += PerformansButceResI.STR033 + "<br>";
                if (birim.spGenelHavuz == null || string.IsNullOrEmpty(birim.spGenelHavuz.kod))
                    hata += PerformansButceResI.STR041 + "<br>";
                if (!string.IsNullOrEmpty(hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = hata;
                    return sonuc;
                }
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birimler[0].orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            ENUMSurec surec = (ENUMSurec)System.Enum.Parse(typeof(ENUMSurec), System.Enum.GetName(typeof(ENUMSurec), (int)(EnumSPGenelHavuzSurec)System.Enum.Parse(typeof(EnumSPGenelHavuzSurec), System.Enum.GetName(typeof(EnumSPGenelHavuzTur), OrtakFonksiyonlar.ConvertToInt(birimler[0].spGenelHavuz.kod.Substring(0, 2), 0)))));
            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birimler[0].donem.donemNo, birimler[0].orgBirimKod, surec, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            Sonuc altKatmanSonuc = AltKatmanlardaKullanilmisMi(ve, kullanan, birimler);
            if (altKatmanSonuc.islemSonuc || (!altKatmanSonuc.islemSonuc && !string.IsNullOrEmpty(altKatmanSonuc.hataStr)))
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                if (altKatmanSonuc.islemSonuc)
                    sonuc.hataStr = altKatmanSonuc.bilgiStr;
                else
                    sonuc.hataStr = altKatmanSonuc.hataStr;
                return sonuc;
            }

            TABBirimGenelHavuz tBirim = new TABBirimGenelHavuz(ve);
            ve.BeginTransaction();

            tBirim.Sil(birimler[0]);
            foreach (Birim_SpGenelHavuz birim in birimler)
            {
                sonuc = tBirim.Kaydet(birim);
                if (!sonuc.islemSonuc)
                    break;
            }

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, birimler[0].donem.donemNo, birimler[0].orgBirimKod, surec);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static Sonuc AltKatmanlardaKullanilmisMi(VeriErisim veriErisim, Kullanici kullanan, params Birim_SpGenelHavuz[] kaydetBirimler)
        {
            Sonuc sonuc = new Sonuc(false, string.Empty, string.Empty);
            if (kaydetBirimler == null || kaydetBirimler.Length <= 0)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            Birim_SpGenelHavuz[] listeleBirimler = null;
            Birim_SpGenelHavuz listeleKriter = null;
            try
            {
                listeleKriter = new Birim_SpGenelHavuz();
                listeleKriter.donem = kaydetBirimler[0].donem;
                listeleKriter.orgBirimKod = kaydetBirimler[0].orgBirimKod;
                listeleKriter.spGenelHavuz = new SpGenelHavuz();
                listeleKriter.spGenelHavuz.kod = kaydetBirimler[0].spGenelHavuz.kod.Substring(0, 2);

                listeleBirimler = Listele(ve, kullanan, listeleKriter);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
                return sonuc;
            }

            string[] listeleFarkKaydet = ListeleFarkKaydetBul(listeleBirimler, kaydetBirimler);
            if (listeleFarkKaydet == null || listeleFarkKaydet.Length <= 0)
            {
                ve.VeriErisimKapat();
                return sonuc;
            }

            try
            {
                SpGenelHavuzBag[] listeBag = STRGenelHavuz.BirimdeKullanilmislarinBaglariniListele(ve, kullanan, listeleBirimler[0].donem.donemNo, listeleBirimler[0].orgBirimKod, string.Empty, listeleFarkKaydet);
                if (listeBag == null || listeBag.Length <= 0)
                {
                    ve.VeriErisimKapat();
                    return sonuc;
                }

                sonuc.islemSonuc = true;

                string ustAdlar = string.Empty;
                string altAdlar = string.Empty;
                foreach (SpGenelHavuzBag lb in listeBag)
                {
                    foreach (Birim_SpGenelHavuz ust in listeleBirimler)
                    {
                        if (lb.kod.Replace(".", "").Equals(ust.spGenelHavuz.kod.Replace(".", "")))
                        {
                            if (!ustAdlar.Contains(ust.spGenelHavuz.absKod.Ad(kullanan.aktifKultur)))
                            {
                                if (!string.IsNullOrEmpty(ustAdlar))
                                    ustAdlar += ", ";
                                ustAdlar += "\"" + ust.spGenelHavuz.absKod.Ad(kullanan.aktifKultur) + "\"";
                            }
                            break;
                        }
                    }

                    if (!altAdlar.Contains(lb.bagliKod.Ad(kullanan.aktifKultur)))
                    {
                        if (!string.IsNullOrEmpty(altAdlar))
                            altAdlar += ", ";
                        altAdlar += "\"" + lb.bagliKod.Ad(kullanan.aktifKultur) + "\"";
                    }
                }

                sonuc.bilgiStr = string.Format(PerformansButceResI.STR087, ustAdlar, PerformansButceResI.ResourceManager.GetString("STR088_" + listeleKriter.spGenelHavuz.kod),
                                altAdlar, PerformansButceResI.ResourceManager.GetString("STR089_" + listeleKriter.spGenelHavuz.kod),
                                PerformansButceResI.ResourceManager.GetString("STR090_" + listeleKriter.spGenelHavuz.kod),
                                PerformansButceResI.ResourceManager.GetString("STR091_" + listeleKriter.spGenelHavuz.kod));
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
                return sonuc;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        private static string[] ListeleFarkKaydetBul(Birim_SpGenelHavuz[] listeleBirimler, params Birim_SpGenelHavuz[] kaydetBirimler)
        {
            if (listeleBirimler == null)
                return null;

            ArrayList liste = new ArrayList();

            foreach (Birim_SpGenelHavuz lb in listeleBirimler)
            {
                bool farkli = true;

                if (kaydetBirimler != null)
                {
                    foreach (Birim_SpGenelHavuz kb in kaydetBirimler)
                    {
                        if (lb.spGenelHavuz.kod.Replace(".", "").Equals(kb.spGenelHavuz.kod.Replace(".", "")))
                        {
                            farkli = false;
                            break;
                        }
                    }
                }

                if (farkli)
                    liste.Add(lb.spGenelHavuz.kod);
            }

            return (string[])liste.ToArray(typeof(string));
        }

        /// <summary>
        /// Hizmet veya Üst Belge silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen birim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="birim">Hizmet veya Üst Belge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz birim, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (birim.donem == null || birim.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (birim.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (birim.spGenelHavuz == null || string.IsNullOrEmpty(birim.spGenelHavuz.kod))
                hata += PerformansButceResI.STR041 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, birim.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            ENUMSurec surec = (ENUMSurec)System.Enum.Parse(typeof(ENUMSurec), System.Enum.GetName(typeof(ENUMSurec), (int)(EnumSPGenelHavuzSurec)System.Enum.Parse(typeof(EnumSPGenelHavuzSurec), System.Enum.GetName(typeof(EnumSPGenelHavuzTur), OrtakFonksiyonlar.ConvertToInt(birim.spGenelHavuz.kod.Substring(0, 2), 0)))));
            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, birim.donem.donemNo, birim.orgBirimKod, surec, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            Sonuc altKatmanSonuc = AltKatmanlardaKullanilmisMi(ve, kullanan, birim);
            if (altKatmanSonuc.islemSonuc || (!altKatmanSonuc.islemSonuc && !string.IsNullOrEmpty(altKatmanSonuc.hataStr)))
            {
                ve.VeriErisimKapat();
                sonuc.islemSonuc = false;
                if (altKatmanSonuc.islemSonuc)
                    sonuc.hataStr = altKatmanSonuc.bilgiStr;
                else
                    sonuc.hataStr = altKatmanSonuc.hataStr;
                return sonuc;
            }

            TABBirimGenelHavuz tBirim = new TABBirimGenelHavuz(ve);
            ve.BeginTransaction();

            sonuc = tBirim.Sil(birim);
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = PerformansButceResI.STR014;
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = PerformansButceResI.STR015 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Hizmet veya Üst Belge ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="birim">Hizmet veya Üst Belge bilgisini tutan nesne</param>
        /// <returns>Bir Hizmet veya Üst Belge için Hizmet veya Üst Belge ayrıntılarını döndürür</returns>
        public static Birim_SpGenelHavuz[] Listele(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz birim)
        {
            return Listele(veriErisim, kullanan, birim, false);
        }

        public static Birim_SpGenelHavuz[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz birim)
        {
            string hata = string.Empty;
            if (birim.donem == null || birim.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (birim.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (birim.spGenelHavuz == null || string.IsNullOrEmpty(birim.spGenelHavuz.kod))
                hata += PerformansButceResI.STR041 + "<br>";
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            return Listele(veriErisim, kullanan, birim, true);
        }

        private static Birim_SpGenelHavuz[] Listele(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz birim, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, birim.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimGenelHavuz tBirim = new TABBirimGenelHavuz(ve);

            IDataReader dr = null;
            try
            {
                dr = tBirim.Listele(birim, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection birimKataloglar = new TNSCollection();
            while (dr.Read())
            {
                Birim_SpGenelHavuz gelen = new Birim_SpGenelHavuz();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABBirimGenelHavuz.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABBirimGenelHavuz.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.spGenelHavuz = new SpGenelHavuz();
                gelen.spGenelHavuz.kod = VeriErisim.DegerAl(dr, (int)TABBirimGenelHavuz.ENUMAlanlar.GENELHAVUZKOD, string.Empty);

                birimKataloglar.Add(gelen);
            }
            dr.Close();

            Birim_SpGenelHavuz[] havuzDizi = (Birim_SpGenelHavuz[])birimKataloglar.ToArray(typeof(Birim_SpGenelHavuz));
            AbsKodAdGetir(ve, kullanan, havuzDizi, birim.spGenelHavuz != null ? birim.spGenelHavuz.kod : string.Empty);

            ve.VeriErisimKapat();
            return havuzDizi;
        }

        public static Birim_SpGenelHavuz[] AgacGetir(VeriErisim veriErisim, Kullanici kullanan, int donemNo, int orgBirimKod, EnumSPGenelHavuzTur girisNoktasi, EnumSPGenelHavuzTur cikisNoktasi)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            Birim_SpGenelHavuz kriter = new Birim_SpGenelHavuz();
            kriter.donem = new Donem();
            kriter.donem.donemNo = donemNo;
            kriter.orgBirimKod = orgBirimKod;
            kriter.spGenelHavuz = new SpGenelHavuz();
            kriter.spGenelHavuz.kod = ((int)girisNoktasi).ToString("d2");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABBirimGenelHavuz tBirim = new TABBirimGenelHavuz(ve);

            Birim_SpGenelHavuz[] kok = new Birim_SpGenelHavuz[0];
            try
            {
                //SpGenelHavuzBag[] ust = STRGenelHavuz.BirimdeKullanilmislarinBaglariniListele(veriErisim, kullanan, donemNo, orgBirimKod, "02......");

                kok = Listele(veriErisim, kullanan, kriter, false);
                for (int i = 0; i < kok.Length; i++)
                {
                    kok[i].kod = kok[i].spGenelHavuz.kod;
                    kok[i].ad = kok[i].spGenelHavuz.absKod.Ad(kullanan.aktifKultur);
                    kok[i].spGenelHavuz = null;

                    SpGenelHavuzBag[] bag = STRGenelHavuz.BirimdeKullanilmislarinBaglariniListele(veriErisim, kullanan, donemNo, orgBirimKod, null, kok[i].kod);
                    if (bag.Length > 0) kok[i].altHavuzlar = new Birim_SpGenelHavuz[bag.Length];
                    for (int j = 0; j < bag.Length; j++)
                    {
                        kok[i].altHavuzlar[j] = new Birim_SpGenelHavuz();
                        kok[i].altHavuzlar[j].kod = bag[j].bagliKod.kod.Replace(".", "");
                        kok[i].altHavuzlar[j].ad = bag[j].bagliKod.Ad(kullanan.aktifKultur);

                        SpGenelHavuzBag[] bag2 = STRGenelHavuz.BirimdeKullanilmislarinBaglariniListele(veriErisim, kullanan, donemNo, orgBirimKod, null, kok[i].altHavuzlar[j].kod);
                        if (bag2.Length > 0) kok[i].altHavuzlar[j].altHavuzlar = new Birim_SpGenelHavuz[bag2.Length];
                        for (int k = 0; k < bag2.Length; k++)
                        {
                            kok[i].altHavuzlar[j].altHavuzlar[k] = new Birim_SpGenelHavuz();
                            kok[i].altHavuzlar[j].altHavuzlar[k].kod = bag2[k].bagliKod.kod.Replace(".", "");
                            kok[i].altHavuzlar[j].altHavuzlar[k].ad = bag2[k].bagliKod.Ad(kullanan.aktifKultur);
                        }
                    }
                }
            }
            catch
            {
                return kok;
            }

            ve.VeriErisimKapat();
            return kok;
        }

        /// <summary>
        /// spHavuzlar nesnesinin absKod'unu ayarlayan yordam.
        /// spHavuzlar nesnesinin absKod özelliğini ustKod parametresi ile işleme sokarak ayarlar.
        /// </summary>
        /// <param name="spHavuzlar">özelliğinin ayarlanması istediğimiz nesne</param>
        /// <param name="ustKod">üst kodunu belirlemek için kullanacağımız parametre</param>
        private static void AbsKodAdGetir(VeriErisim veriErisim, Kullanici kullanan, Birim_SpGenelHavuz[] spHavuzlar, string ustKod)
        {
            SpGenelHavuz[] kodAdlar = STRGenelHavuz.Listele(veriErisim, kullanan, ustKod, EnumSPGenelHavuzDurum.Hepsi, false);
            if (kodAdlar == null || kodAdlar.Length <= 0)
                return;

            foreach (SpGenelHavuz kodAd in kodAdlar)
                foreach (Birim_SpGenelHavuz spHavuz in spHavuzlar)
                    if (kodAd.kod.Replace(".", "").Equals(spHavuz.spGenelHavuz.kod.Replace(".", "")))
                        spHavuz.spGenelHavuz = kodAd;
        }
    }
}
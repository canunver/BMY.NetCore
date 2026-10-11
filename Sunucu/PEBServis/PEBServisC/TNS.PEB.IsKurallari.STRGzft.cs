using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Stratejik plan mevcut durum analizinde belirlenecek olan Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) Analizlerinin yönetilmesi 
    /// ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRGzft
    {
        /// <summary>
        /// Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen gzft nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gzft">Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Gzft gzft, string formId, bool sadeceAciklama)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (gzft.donem == null || gzft.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (gzft.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (string.IsNullOrEmpty(gzft.analizYontemi))
                hata += PerformansButceResI.STR042 + "<br>";
            if (gzft.gzftKonuListe == null || gzft.gzftKonuListe.Length <= 0)
                hata += PerformansButceResI.STR044;
            else
            {
                foreach (GzftDetay gdetay in gzft.gzftKonuListe)
                {
                    if (string.IsNullOrEmpty(gdetay.aciklama))
                        hata += PerformansButceResI.STR052 + "<br>";
                    if (!sadeceAciklama)
                    {
                        if (gdetay.oncelikKod < 1)
                            hata += PerformansButceResI.STR059 + "<br>";
                        if (gdetay.oncelikKod != 4 && string.IsNullOrEmpty(gdetay.sorunAlanKod))
                            hata += PerformansButceResI.STR108 + "<br>";
                    }
                    if (!string.IsNullOrEmpty(hata))
                        break;
                }
            }
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, gzft.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, gzft.donem.donemNo, gzft.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            ve.BeginTransaction();

            Sil(ve, gzft.donem.donemNo, gzft.orgBirimKod);
            sonuc = Kaydet(ve, gzft);

            //GZFT ile ilişkilendirilmiş sorun alanlardan birimin temaları ve sorun alanları bulunup kayıt ediliyor.
            if (sonuc.islemSonuc)
                sonuc = BirimSorunAlanKaydet(ve, kullanan, gzft, formId);
            if (sonuc.islemSonuc)
                sonuc = BirimTemaKaydet(ve, kullanan, gzft, formId);

            if (sonuc.islemSonuc)
                sonuc = STRGenel.AsamaKontrolBaslat(ve, kullanan, gzft.donem.donemNo, gzft.orgBirimKod, ENUMSurec.MevcutDurumAnalizi);

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

        private static Sonuc Kaydet(VeriErisim ve, Gzft gzft)
        {
            TABGzft to = new TABGzft(ve);
            Sonuc sonuc = to.Kaydet(gzft);

            if (sonuc.islemSonuc)
            {
                foreach (GzftDetay gd in gzft.gzftKonuListe)
                {
                    sonuc = to.KaydetDetay(gzft.donem.donemNo, gzft.orgBirimKod, gd);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            return sonuc;
        }

        private static Sonuc BirimSorunAlanKaydet(VeriErisim ve, Kullanici kullanan, Gzft gzft, string gzftFormId)
        {
            TNSCollection sorunAlanlar = new TNSCollection();
            foreach (GzftDetay detay in gzft.gzftKonuListe)
            {
                if (string.IsNullOrEmpty(detay.sorunAlanKod) || Birim_SpGenelHavuz_VarMi(sorunAlanlar, detay.sorunAlanKod))
                    continue;

                Birim_SpGenelHavuz bspgh = new Birim_SpGenelHavuz();
                bspgh.donem = gzft.donem;
                bspgh.orgBirimKod = gzft.orgBirimKod;
                bspgh.spGenelHavuz = new SpGenelHavuz();
                bspgh.spGenelHavuz.kod = detay.sorunAlanKod;
                bspgh.spGenelHavuz.tur = detay.sorunAlanKod.Substring(0, 2);

                sorunAlanlar.Add(bspgh);
            }

            if (sorunAlanlar.Count > 0)
                return BirimGenelHavuz.Kaydet(ve, kullanan, (Birim_SpGenelHavuz[])sorunAlanlar.ToArray(typeof(Birim_SpGenelHavuz)), gzftFormId);
            else
                return BirimSorunAlanSil(ve, kullanan, gzft, gzftFormId);
        }

        private static Sonuc BirimTemaKaydet(VeriErisim ve, Kullanici kullanan, Gzft gzft, string gzftFormId)
        {
            TNSCollection temalar = new TNSCollection();
            foreach (GzftDetay detay in gzft.gzftKonuListe)
            {
                if (string.IsNullOrEmpty(detay.sorunAlanKod))
                    continue;

                SpGenelHavuzBag[] tema = STRGenelHavuz.ListeleBag(ve, kullanan, detay.sorunAlanKod);
                if (tema == null || tema.Length <= 0)
                    continue;

                if (Birim_SpGenelHavuz_VarMi(temalar, tema[0].bagliKod.kod))
                    continue;

                Birim_SpGenelHavuz bspgh = new Birim_SpGenelHavuz();
                bspgh.donem = gzft.donem;
                bspgh.orgBirimKod = gzft.orgBirimKod;
                bspgh.spGenelHavuz = new SpGenelHavuz();
                bspgh.spGenelHavuz.kod = tema[0].bagliKod.kod;
                bspgh.spGenelHavuz.tur = tema[0].bagliKod.kod.Substring(0, 2);

                temalar.Add(bspgh);
            }

            if (temalar.Count > 0)
                return BirimGenelHavuz.Kaydet(ve, kullanan, (Birim_SpGenelHavuz[])temalar.ToArray(typeof(Birim_SpGenelHavuz)), gzftFormId);
            else
                return BirimTemaSil(ve, kullanan, gzft, gzftFormId);
        }

        private static bool Birim_SpGenelHavuz_VarMi(TNSCollection dizi, string arananKod)
        {
            foreach (Birim_SpGenelHavuz eleman in dizi)
                if (eleman.spGenelHavuz.kod.Equals(arananKod))
                    return true;
            return false;
        }

        private static Sonuc Sil(VeriErisim ve, int donemNo, int orgBirimKod)
        {
            TABGzft to = new TABGzft(ve);
            Sonuc sonuc = to.SilDetay(donemNo, orgBirimKod);
            if (sonuc.islemSonuc)
                sonuc = to.Sil(donemNo, orgBirimKod);

            return sonuc;
        }

        /// <summary>
        /// Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen gzft nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gzft">Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Gzft gzft, string formId)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (gzft.donem == null || gzft.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (gzft.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            sonuc = YetkiSTR.YazmaHakkiVarMi(kullanan, gzft.orgBirimKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            bool onayliMi = STRGenel.AsamaKontrolDetayOnayliMi(ve, gzft.donem.donemNo, gzft.orgBirimKod, ENUMSurec.MevcutDurumAnalizi, formId);
            if (onayliMi)
            {
                ve.VeriErisimKapat();

                sonuc.islemSonuc = false;
                sonuc.hataStr = PerformansButceResI.STR081;
                return sonuc;
            }

            ve.BeginTransaction();

            sonuc = Sil(ve, gzft.donem.donemNo, gzft.orgBirimKod);

            //Birimin temaları ve sorun alanları siliniyor.
            if (sonuc.islemSonuc)
                sonuc = BirimSorunAlanSil(ve, kullanan, gzft, formId);
            if (sonuc.islemSonuc)
                sonuc = BirimTemaSil(ve, kullanan, gzft, formId);

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

        private static Sonuc BirimSorunAlanSil(VeriErisim ve, Kullanici kullanan, Gzft gzft, string gzftFormId)
        {
            Birim_SpGenelHavuz silKriter = new Birim_SpGenelHavuz();
            silKriter.donem = gzft.donem;
            silKriter.orgBirimKod = gzft.orgBirimKod;
            silKriter.spGenelHavuz = new SpGenelHavuz();
            silKriter.spGenelHavuz.kod = ((int)EnumSPGenelHavuzTur.SorunAlan).ToString().PadLeft(2, '0');
            return BirimGenelHavuz.Sil(ve, kullanan, silKriter, gzftFormId);
        }

        private static Sonuc BirimTemaSil(VeriErisim ve, Kullanici kullanan, Gzft gzft, string gzftFormId)
        {
            Birim_SpGenelHavuz silKriter = new Birim_SpGenelHavuz();
            silKriter.donem = gzft.donem;
            silKriter.orgBirimKod = gzft.orgBirimKod;
            silKriter.spGenelHavuz = new SpGenelHavuz();
            silKriter.spGenelHavuz.kod = ((int)EnumSPGenelHavuzTur.Tema).ToString().PadLeft(2, '0');
            return BirimGenelHavuz.Sil(ve, kullanan, silKriter, gzftFormId);
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) ayrıntısını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gzft">Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) bilgisini tutan nesne</param>
        /// <returns>Bir Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) için Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) ayrıntılarını döndürür</returns>
        public static Gzft ListeleTek(VeriErisim veriErisim, Kullanici kullanan, Gzft gzft)
        {
            Gzft[] liste = Listele(veriErisim, kullanan, gzft, true);
            if (liste != null && liste.Length > 0)
                return liste[0];
            return null;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) ayrıntılarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="gzft">Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) bilgisini tutan nesne</param>
        /// <returns>Bir Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) için Güçlü Yönler, Zayıf Yönler, Fırsatlar veya Tehditler(GZFT) ayrıntılarını döndürür</returns>
        public static Gzft[] Listele(VeriErisim veriErisim, Kullanici kullanan, Gzft gzft, bool detay)
        {
            return Listele(veriErisim, kullanan, gzft, detay, false);
        }

        public static Gzft[] Sorgula(VeriErisim veriErisim, Kullanici kullanan, Gzft gzft)
        {
            string hata = string.Empty;
            if (gzft.donem == null || gzft.donem.donemNo <= 0)
                hata = PerformansButceResI.STR032 + "<br>";
            if (gzft.orgBirimKod <= 0)
                hata += PerformansButceResI.STR033;
            if (!string.IsNullOrEmpty(hata))
                throw new Exception(hata);

            if (gzft.gzftKonuListe != null && gzft.gzftKonuListe.Length == 1)
            {
                gzft.gzftKonuListe[0].donem = gzft.donem;
                gzft.gzftKonuListe[0].orgBirimKod = gzft.orgBirimKod;
            }

            return Listele(veriErisim, kullanan, gzft, false, true);
        }

        private static Gzft[] Listele(VeriErisim veriErisim, Kullanici kullanan, Gzft gzft, bool detay, bool sorgu)
        {
            Sonuc sonuc = YetkiSTR.OkumaHakkiVarMi(kullanan, gzft.orgBirimKod);
            if (!sonuc.islemSonuc)
                throw new Exception(sonuc.hataStr);

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGzft to = new TABGzft(ve);

            IDataReader dr = null;
            try
            {
                dr = to.Listele(gzft, sorgu);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            VeriErisim ve2 = null;
            if (detay || (sorgu && gzft.gzftKonuListe == null))
                ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            TNSCollection gzftListe = new TNSCollection();
            while (dr.Read())
            {
                Gzft gelen = new Gzft();
                gelen.donem = new Donem();
                gelen.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlar.DONEMNO, 0);
                gelen.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlar.ORGBIRIMKOD, 0);
                gelen.orgBirimAd = PEBServis.UzayYaziDegerGetir("ORGBIRIM", gelen.orgBirimKod.ToString(), true, string.Empty);
                gelen.analizYontemi = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlar.ANALIZYONTEMI, string.Empty);

                if (detay || (sorgu && gzft.gzftKonuListe == null))
                {
                    try
                    {
                        GzftDetay d = new GzftDetay();
                        d.donem = gelen.donem;
                        d.orgBirimKod = gelen.orgBirimKod;

                        gelen.gzftKonuListe = ListeleDetay(ve2, d, sorgu);
                    }
                    catch (Exception)
                    {
                    }
                }

                gzftListe.Add(gelen);
            }
            dr.Close();

            if (sorgu && gzft.gzftKonuListe != null && gzft.gzftKonuListe.Length == 1)
            {
                GzftDetay[] gd = ListeleDetay(ve, gzft.gzftKonuListe[0], sorgu);
                gzftListe = SorguFormVeDetayBirlestir(gzftListe, gd);
            }

            ve.VeriErisimKapat();
            if (ve2 != null)
                ve2.VeriErisimKapat();
            return (Gzft[])gzftListe.ToArray(typeof(Gzft));
        }

        private static GzftDetay[] ListeleDetay(VeriErisim ve, GzftDetay gzftDetay, bool sorgu)
        {
            TABGzft to = new TABGzft(ve);

            IDataReader dr = null;
            try
            {
                dr = to.ListeleDetay(gzftDetay, sorgu);
            }
            catch (Exception e)
            {
                throw e;
            }

            TNSCollection liste = new TNSCollection();
            while (dr.Read())
            {
                GzftDetay detay = new GzftDetay();
                detay.donem = new Donem();
                detay.donem.donemNo = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlarDetay.DONEMNO, 0);
                detay.orgBirimKod = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlarDetay.ORGBIRIMKOD, 0);
                detay.tur = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlarDetay.TUR, 0);
                detay.siraNo = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlarDetay.SIRANO, 0);
                detay.aciklama = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlarDetay.ACIKLAMA, string.Empty);
                detay.oncelikKod = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlarDetay.ONCELIK, 0);
                detay.sorunAlanKod = VeriErisim.DegerAl(dr, (int)TABGzft.ENUMAlanlarDetay.SORUNALANKOD, string.Empty);

                liste.Add(detay);
            }
            dr.Close();

            return (GzftDetay[])liste.ToArray(typeof(GzftDetay));
        }

        private static TNSCollection SorguFormVeDetayBirlestir(TNSCollection formlar, GzftDetay[] detaylar)
        {
            TNSCollection bulunanFormlar = new TNSCollection();

            foreach (Gzft form in formlar)
            {
                bool bulundu = false;
                TNSCollection bulunanDetaylar = new TNSCollection();

                foreach (GzftDetay detay in detaylar)
                {
                    if (form.donem.donemNo == detay.donem.donemNo && form.orgBirimKod == detay.orgBirimKod)
                    {
                        bulundu = true;
                        bulunanDetaylar.Add(detay);
                    }
                }

                if (bulundu)
                {
                    form.gzftKonuListe = (GzftDetay[])bulunanDetaylar.ToArray(typeof(GzftDetay));
                    bulunanFormlar.Add(form);
                }
            }

            return bulunanFormlar;
        }
    }
}
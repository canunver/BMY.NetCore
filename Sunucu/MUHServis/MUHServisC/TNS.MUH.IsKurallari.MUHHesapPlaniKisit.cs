using System.Data;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using TNS.BKD;
using System;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// HesapPlaniKisit işlemlerinin yapıldığı sınıf
    /// </summary>
    public class MUHHesapPlaniKisit
    {
        /// <summary>
        /// HesapPlaniKisit kaydet
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Kullanici kullanan, HesapPlaniKisit bilgi)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABHesapPlaniKisit to = new TABHesapPlaniKisit(ve);

            if (bilgi.kod == 0)
                bilgi.kod = to.SiraNoBul(bilgi.kod);

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = ButceMuhasebeResI.MUH201;
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// HesapPlaniKisit
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public ObjectArray Listele(Kullanici kullanan, HesapPlaniKisit bilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHesapPlaniKisit to = new TABHesapPlaniKisit(ve);

            IDataReader r = to.Listele(bilgi);

            if (r != null)
            {
                Tertip t = new Tertip(DateTime.Now.Year, ENUMTabloKod.TABOdemeEmri);

                while (r.Read())
                {
                    HesapPlaniKisit kisit = new HesapPlaniKisit();

                    kisit.kod = VeriErisim.DegerAl(r, 0, 0);
                    kisit.yil = VeriErisim.DegerAl(r, 1, 0);
                    kisit.muhasebeKod = VeriErisim.DegerAl(r, 2, 0);
                    kisit.birimKod = VeriErisim.DegerAl(r, 3, "");
                    kisit.hesapKod = VeriErisim.DegerAl(r, 4, "");
                    kisit.hesapAdi = VeriErisim.DegerAl(r, 5, "");
                    kisit.islemYapanAd = VeriErisim.DegerAl(r, 6, "");
                    kisit.islemYapanKod = VeriErisim.DegerAl(r, 7, "");
                    kisit.islemTarihi = VeriErisim.DegerAl(r, 8);

                    kisit.hesapKod = OrtakFonksiyonlar.KodNoktaKoy((string)t.Kirilimlar(0), kisit.hesapKod);

                    iVAN.objeler.Add(kisit);
                }
                r.Close();
            }

            ve.VeriErisimKapat();
            return iVAN;
        }

        /// <summary>
        /// HesapPlaniKisit sil.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kisitListe">The kisit liste.</param>
        /// <returns></returns>
        public Sonuc Sil(Kullanici kullanan, ObjectArray kisitListe)
        {
            Sonuc sonuc = new Sonuc(true, ButceMuhasebeResI.MUH001, "");

            if (kisitListe.objeler.Count == 0 || kisitListe == null)
            {
                sonuc.DegerAta(ButceMuhasebeResI.MUH133);
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABHesapPlaniKisit to = new TABHesapPlaniKisit(ve);

            foreach (HesapPlaniKisit t in kisitListe.objeler)
            {
                sonuc = to.Sil(t.kod);
                if (!sonuc.islemSonuc)
                    break;
            }


            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt silindi";
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Muhasebes the belgesi kontrol.
        /// </summary>
        /// <param name="ve">The ve.</param>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc MuhasebeBelgesiKontrol(VeriErisim ve, Kullanici kullanan, OdemeEmriMIFForm form)
        {
            Sonuc sonuc = new Sonuc();

            if (form.fisTur[0] == ENUMFISTUR.KAPANIS)
            {
                return sonuc;
            }

            string hesapKodlari = "";
            foreach (OdemeEmriMIFDetay oed in form.detay.objeler)
            {
                if (hesapKodlari != "") hesapKodlari += ";";
                hesapKodlari += oed.hesapKod.Trim();
            }

            if (hesapKodlari == "")
            {
                sonuc.islemSonuc = true;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHesapPlaniKisit to = new TABHesapPlaniKisit(ve);
            IDataReader r = to.MuhasebeBelgesiKontrol(form.yil, hesapKodlari);

            if (r != null)
            {
                ObjectArray kisitListe = new ObjectArray();

                while (r.Read())
                {
                    HesapPlaniKisit kisit = new HesapPlaniKisit();

                    kisit.kod = VeriErisim.DegerAl(r, 0, 0);
                    kisit.yil = VeriErisim.DegerAl(r, 1, 0);
                    kisit.muhasebeKod = VeriErisim.DegerAl(r, 2, 0);
                    kisit.birimKod = VeriErisim.DegerAl(r, 3, "");
                    kisit.hesapKod = VeriErisim.DegerAl(r, 4, "");
                    kisit.hesapAdi = VeriErisim.DegerAl(r, 5, "");

                    kisitListe.objeler.Add(kisit);
                }
                r.Close();

                bool kapanisFisi = false;
                if (form.fisTur[0] == ENUMFISTUR.KAPANIS ||
                    form.fisTur[0] == ENUMFISTUR.ACILIS ||
                    form.fisTur[0] == ENUMFISTUR.SONFIS)
                {
                    kapanisFisi = true;
                }

                foreach (OdemeEmriMIFDetay oed in form.detay.objeler)
                {
                    bool hesapVar = false;
                    bool birimVar = false;
                    string birimler = "";
                    foreach (HesapPlaniKisit kisit in kisitListe.objeler)
                    {
                        if (oed.hesapKod == kisit.hesapKod)
                        {
                            hesapVar = true;
                            if (oed.detayBirim == kisit.birimKod) birimVar = true;
                            if (birimler != "") birimler = "; ";
                            birimler += kisit.birimKod;
                        }
                    }

                    if (hesapVar && !birimVar && !kapanisFisi)
                        sonuc.hataStr += "<br>" + oed.hesapKod + "-" + oed.hesapAd + " hesabı, sadece " + birimler + " birimi tarafından kullanılabilir.";
                }
            }
            ve.hata = "";

            if (sonuc.hataStr != "")
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Hesap Plani Kısıt listesi aktar
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kaynakYil">Kaynak yil</param>
        /// <param name="hedefYil">Hedef yil</param>
        /// <returns></returns>
        public Sonuc Aktar(Kullanici kullanan, int kaynakYil, int hedefYil)
        {
            Sonuc sonuc = new Sonuc(false, ButceMuhasebeResI.MUH001, "");

            ObjectArray kaynakListe = Listele(kullanan, new HesapPlaniKisit { yil = kaynakYil });
            if (kaynakListe.objeler.Count == 0)
            {
                sonuc.hataStr = "Kaynak gösterilen yılda kayıt yok";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();
            TABHesapPlaniKisit to = new TABHesapPlaniKisit(ve);
            sonuc = to.Aktar(kaynakYil, hedefYil);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                ve.BeginTransaction();
                foreach (HesapPlaniKisit hk in kaynakListe.objeler)
                {
                    hk.yil = hedefYil;
                    hk.kod = 0;
                    sonuc = Kaydet(kullanan, hk);
                    if (!sonuc.islemSonuc) break;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format("Hesap Planı Kısıt bilgileri {0} yılına aktarıldı", hedefYil);
            }
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

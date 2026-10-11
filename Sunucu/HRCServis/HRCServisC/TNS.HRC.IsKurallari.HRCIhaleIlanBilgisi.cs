using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale ilan bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleIlanBilgisi
    {
        TNSDateTime HaftaSonuMu(TNSDateTime tarih)
        {
            if (tarih.HaftaninGunu == 6)
            {
                tarih.AddDays(2);
                return tarih;

            }
            if (tarih.HaftaninGunu == 0)
            {
                tarih.AddDays(1);
                return tarih;
            }
            return tarih;
        }

        /// <summary>
        /// Arguman olarak verilen, ihale ilan bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IhaleIlanBilgisi bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (bilgi.ilanVerilecekmi)
            {
                if ((bilgi.ihaleTarihi.YilinGunu < bilgi.tarih.YilinGunu) && (bilgi.ihaleTarihi.Yil <= bilgi.tarih.Yil))
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00176;
                    return sonuc;
                }
            }

            //if (bilgi.kismiTeklifeAcikMi == (int)ENUMEvetHayir.HAYIR && bilgi.teklifTuru != 1)
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00177;
            //    return sonuc;
            //}


            OnayForm of = new OnayForm();
            of.yil = bilgi.yil;
            of.muhasebe = bilgi.muhasebe;
            of.birim = bilgi.birim;
            of.surecNo = bilgi.surecNo;

            ObjectArray ofBilgi = new HRCOnayForm().Listele(kullanan, of);
            if (ofBilgi.objeler.Count > 0)
            {
                int zaman = ((OnayForm)(ofBilgi.objeler[0])).ilanTanimi.zaman;
                int zamanBaslamaTur = ((OnayForm)(ofBilgi.objeler[0])).ilanTanimi.zamanBaslamaTur;

                bilgi.tarih.AddDays(zaman); //gerekli zaman ekleniyor.
                if ((bilgi.ihaleTarihi.YilinGunu < bilgi.tarih.YilinGunu) && (bilgi.ihaleTarihi.Yil <= bilgi.tarih.Yil))
                {
                    sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00178, zaman.ToString(), HaftaSonuMu(bilgi.tarih).ToString());
                    return sonuc;
                }

                if (HaftaSonuMu(bilgi.ihaleTarihi) != bilgi.ihaleTarihi)
                {
                    sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00179, bilgi.ihaleTarihi);
                    return sonuc;
                }
                bilgi.tarih.AddDays(-zaman);
            }
            if (bilgi.ilanVerilecekmi)
            {
                if (bilgi.tarih.isNull)
                    sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00180 + "<br>";
            }
            if (bilgi.ihaleTarihi.isNull)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00181 + "<br>";

            if (bilgi.ihaleSaati == "")
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00182 + "<br>";

            //01032010 KmL KİK ile bağlantıda sorun var yeni servis açılıncaya kadar kontrol kaldırıldı.
            //if (bilgi.sektorKodu == "")
            //    sonuc.hataStr += "Sektör Kodu bilgisi boş bırakılmış.<br>";

            //if (bilgi.ymaKod == 0)
            //    sonuc.hataStr += "Yaklaşık Maliyet Aralığı bilgisi boş bırakılmış.<br>";

            //if (bilgi.ilanAdedi == 0)
            //    sonuc.hataStr += "İlan Adedi bilgisi boş bırakılmış.<br>";

            if (sonuc.hataStr != "")
                return sonuc;

            try
            {
                string gunSaat = bilgi.ihaleSaati;
                int koordinat = gunSaat.IndexOf(":");

                if (koordinat == -1)
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00183 + "<br>";
                    return sonuc;
                }
            }
            catch
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00183 + "<br>";
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;
            int yil = bilgi.yil;
            string surecNo = bilgi.surecNo;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisi to = new TABIhaleIlanBilgisi(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc = IhaleNoKaydet(kullanan, bilgi, islemKod, parcaKod);
                if (sonuc.islemSonuc)
                {

                    //Parca Adım devam ediyor olarak işaretle.
                    ///////////////////////////////////////////////////////////////////////////
                    new HRCSurec().ParcaDurumKaydet(null, kullanan, yil, surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                    ///////////////////////////////////////////////////////////////////////////

                    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        private Sonuc IhaleNoKaydet(Kullanici kullanan, IhaleIlanBilgisi bilgi, int islemKod, int parcaKod)
        {
            if (!string.IsNullOrEmpty(bilgi.ihaleNo))
            {
                IhaleNumarasi iNo = new IhaleNumarasi();
                iNo.yil = bilgi.yil;
                iNo.muhasebe = bilgi.muhasebe;
                iNo.birim = bilgi.birim;
                iNo.surecNo = bilgi.surecNo;
                iNo.ihaleNo = bilgi.ihaleNo;

                return new HRCIhaleNumarasi().Kaydet(kullanan, iNo, islemKod, parcaKod);
            }
            else
                return new Sonuc(true, "", TNS.HRC.HarcamaSurecleriResI.HRC00010);
        }

        /// <summary>
        /// Arguman olarak verilen, şartname ihale ilan bilgisini veritabanına kaydeder
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>
        /// İşlem sonucunu döndüren, Sonuc sınıfında nesne
        /// </returns>
        public Sonuc SartnameIhaleIlanBilgisiKaydet(Kullanici kullanan, IhaleIlanBilgisi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (bilgi.ilanVerilecekmi)
            {
                if ((bilgi.ihaleTarihi.YilinGunu < bilgi.tarih.YilinGunu) && (bilgi.ihaleTarihi.Yil <= bilgi.tarih.Yil))
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00176;
                    return sonuc;
                }
            }
            OnayForm of = new OnayForm();
            of.yil = bilgi.yil;
            of.muhasebe = bilgi.muhasebe;
            of.birim = bilgi.birim;
            of.surecNo = bilgi.surecNo;

            ObjectArray ofBilgi = new HRCOnayForm().Listele(kullanan, of);
            if (ofBilgi.objeler.Count > 0)
            {
                int zaman = ((OnayForm)(ofBilgi.objeler[0])).ilanTanimi.zaman;
                int zamanBaslamaTur = ((OnayForm)(ofBilgi.objeler[0])).ilanTanimi.zamanBaslamaTur;

                bilgi.tarih.AddDays(zaman); //gerekli zaman ekleniyor.
                if ((bilgi.ihaleTarihi.YilinGunu < bilgi.tarih.YilinGunu) && (bilgi.ihaleTarihi.Yil <= bilgi.tarih.Yil))
                {
                    sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00178, zaman.ToString(), HaftaSonuMu(bilgi.tarih).ToString());
                    return sonuc;
                }

                if (HaftaSonuMu(bilgi.ihaleTarihi) != bilgi.ihaleTarihi)
                {
                    sonuc.hataStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00179, bilgi.ihaleTarihi);
                    return sonuc;
                }
                bilgi.tarih.AddDays(-zaman);
            }
            if (bilgi.ilanVerilecekmi)
            {
                if (bilgi.tarih.isNull)
                    sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00180 + "<br>";
            }
            if (bilgi.ihaleTarihi.isNull)
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00181 + "<br>";

            if (bilgi.ihaleSaati == "")
                sonuc.hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00182 + "<br>";

            if (sonuc.hataStr != "")
                return sonuc;

            try
            {
                string gunSaat = bilgi.ihaleSaati;
                int koordinat = gunSaat.IndexOf(":");

                if (koordinat == -1)
                {
                    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00183 + "<br>";
                    return sonuc;
                }
            }
            catch
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00183 + "<br>";
                return sonuc;
            }


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleIlanBilgisi to = new TABIhaleIlanBilgisi(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale ilan bilgisini veritabanından okur 
        /// ve IhaleIlanBilgisi nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale ilan bilgisini IhaleIlanBilgisi nesnesi olarak tutan nesne</returns>
        public IhaleIlanBilgisi Listele(Kullanici kullanan, IhaleIlanBilgisi kriter)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleIlanBilgisi to = new TABIhaleIlanBilgisi(ve);
            IDataReader r = to.Listele(kriter);

            IhaleIlanBilgisi f = new IhaleIlanBilgisi();
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    f.birim = VeriErisim.DegerAl(r, 2, "");
                    f.surecNo = VeriErisim.DegerAl(r, 3, "");
                    f.tarih = VeriErisim.DegerAl(r, 4);
                    f.teslimYeri = VeriErisim.DegerAl(r, 5, "");
                    f.teslimTarihi = VeriErisim.DegerAl(r, 6, "");
                    f.ihaleYeri = VeriErisim.DegerAl(r, 7, "");
                    f.ihaleTarihi = VeriErisim.DegerAl(r, 8);
                    f.ihaleSaati = VeriErisim.DegerAl(r, 9, "");
                    f.ihaleTeminYeri = VeriErisim.DegerAl(r, 10, "");
                    f.teklifSuresi = VeriErisim.DegerAl(r, 11, 0);
                    f.isSuresi = VeriErisim.DegerAl(r, 12, 0);
                    f.teklifTuru = VeriErisim.DegerAl(r, 13, 0);
                    f.teklifBelirlemeTuru = VeriErisim.DegerAl(r, 14, 0);
                    f.katilimTuru = VeriErisim.DegerAl(r, 15, 0);
                    f.indirimOrani = VeriErisim.DegerAl(r, 16, 0);
                    f.konsorsiyum = VeriErisim.DegerAl(r, 17, 0);
                    f.ilanAdedi = VeriErisim.DegerAl(r, 18, 0);
                    f.ilanSekli = VeriErisim.DegerAl(r, 19, 0);
                    f.kaynakTuru = VeriErisim.DegerAl(r, 20, 0);
                    f.kismiTeklifeAcikMi = VeriErisim.DegerAl(r, 21, 0);
                    f.sektorKodu = VeriErisim.DegerAl(r, 22, "");
                    f.sektorAdi = VeriErisim.DegerAl(r, 23, "");
                    f.ymaKod = VeriErisim.DegerAl(r, 24, 0);
                    f.ymaBaslangicTarihi = VeriErisim.DegerAl(r, 25);
                    f.ymaBitisTarihi = VeriErisim.DegerAl(r, 26);
                    f.ymaAciklama = VeriErisim.DegerAl(r, 27, "");
                    f.onYeterlikDavetTuru = VeriErisim.DegerAl(r, 28, 0);
                    f.onYeterlikDavetSayi = VeriErisim.DegerAl(r, 29, 0);
                    f.ihaleNo = VeriErisim.DegerAl(r, 30, "");
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return f;
        }
    }
}
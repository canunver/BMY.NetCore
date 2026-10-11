using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// Hakediş bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCHakedis
    {
        /// <summary>
        /// Arguman olarak verilen, hakediş bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Hakedis bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //if (!HRCYetkiKontrol.Yoneticimi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            string hata = "";
            if (!bilgi.hesapPlaniMi)
            {
                if (bilgi.ihaleTarihi.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00124 + "<br>";
                if (bilgi.sozlesmeBedeli == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00125 + "<br>";
                if (bilgi.sozlesmeTarihi.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00126 + "<br>";
                if (bilgi.bitimTarihi.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00127 + "<br>";
                if (bilgi.teslimTarihi.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00128 + "<br>";
                if (bilgi.sozlesmeNo == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00129 + "<br>";
                if (bilgi.ihaleKararNo == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00130 + "<br>";
                if (bilgi.ihaleKararTarihi.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00131 + "<br>";
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHakedis to = new TABHakedis(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetOnBilgi(bilgi);

            if (bilgi.hesapPlaniMi && sonuc.islemSonuc)//Eğer ödeme planı ekranından geliyor ise  
            {
                to.SilOdemePlani(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo);
                sonuc = to.KaydetOdemePlani(bilgi.odemePlan);
            }
            else if (sonuc.islemSonuc)
            {
                to.SilOdemePlani(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo);
                to.SilSureUzat(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo);
                to.SilEkSozlesme(bilgi.yil, bilgi.muhasebe, bilgi.birim, bilgi.surecNo);

                sonuc = to.KaydetOdemePlani(bilgi.odemePlan);
                if (sonuc.islemSonuc)
                {
                    sonuc = to.KaydetSureUzat(bilgi.sureUzatim);
                    if (sonuc.islemSonuc)
                        sonuc = to.KaydetEkSozlesme(bilgi.ekSozlesme);
                }
            }

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, bilgi.yil, bilgi.surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////
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
        /// Arguman olarak verilen, ihalenin teklif değerlendirme işlemleri bittikten sonra girilen hakediş ön bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc KaydetOnBilgi(Kullanici kullanan, Hakedis bilgi, int islemKod, int parcaKod, TNSCollection tertipler)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hata = "";
            if (bilgi.ihaleTarihi.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00124 + "<br>";
            if (bilgi.sozlesmeBedeli == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00125 + "<br>";
            if (bilgi.sozlesmeTarihi.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00126 + "<br>";
            if (bilgi.bitimTarihi.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00127 + "<br>";
            if (bilgi.teslimTarihi.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00128 + "<br>";
            if (bilgi.sozlesmeNo == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00129 + "<br>";
            if (bilgi.ihaleKararNo == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00130 + "<br>";
            if (bilgi.ihaleKararTarihi.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00131 + "<br>";
            if (bilgi.sozlesmeKDVBedeli < 0)
                hata += "Sözleşme KDV bilgisi hatalı" + "<br>";
            if (tertipler != null)
            {
                double tutar = 0;
                foreach (OnayFormTertip item in tertipler)
                {
                    tutar += item.sozlesmeKdvliTutar;
                }
                double toplamSozlesmeBedeli = bilgi.sozlesmeKDVBedeli + bilgi.sozlesmeBedeli;
                if (Math.Round(tutar, 2) != Math.Round(toplamSozlesmeBedeli, 2))
                {
                    hata += String.Format("KDVli Sözleşme bedeli ({0}) ile tertiplere yapılan dağılım ({1}) eşit değil!", toplamSozlesmeBedeli, tutar) + "<br>";
                }
            }
            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHakedis to = new TABHakedis(ve);
            ve.BeginTransaction();

            sonuc = to.KaydetOnBilgi(bilgi);

            if (sonuc.islemSonuc && tertipler != null)
            {
                TABOnayForm tof = new TABOnayForm(ve);
                foreach (OnayFormTertip item in tertipler)
                {
                    item.yil = bilgi.yil;
                    item.surecNo = bilgi.surecNo;
                    sonuc = tof.TertipSozlesmeGuncelle(item);
                    if (!sonuc.islemSonuc) break;
                }
            }

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
                if (tertipler != null)
                {
                    new HRCOnayForm().BlokeTertipleriOtomatikGuncelle(null, kullanan, bilgi.yil, bilgi.surecNo, DateTime.Now.Year, ENUMKontrolDurum.KontrolEtme);
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

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hakediş bilgisini veritabanından okur 
        /// ve Hakedis nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hakediş bilgisini Hakedis nesnesi olarak tutan nesne</returns>
        public Hakedis Listele(Kullanici kullanan, Hakedis kriter)
        {
            Hakedis donusDegeri = new Hakedis();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHakedis to = new TABHakedis(ve);
            IDataReader r = to.ListeleOnBilgi(kriter);

            if (r != null)
            {
                if (r.Read())
                {
                    donusDegeri.yil = VeriErisim.DegerAl(r, 0, 0);
                    donusDegeri.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    donusDegeri.birim = VeriErisim.DegerAl(r, 2, "");
                    donusDegeri.surecNo = VeriErisim.DegerAl(r, 3, "");
                    donusDegeri.sozlesmeBedeli = VeriErisim.DegerAl(r, 4, 0.0);
                    donusDegeri.ihaleTarihi = VeriErisim.DegerAl(r, 5);
                    donusDegeri.ihaleKararTarihi = VeriErisim.DegerAl(r, 6);
                    donusDegeri.ihaleKararNo = VeriErisim.DegerAl(r, 7, "");
                    donusDegeri.sozlesmeTarihi = VeriErisim.DegerAl(r, 8);
                    donusDegeri.sozlesmeNo = VeriErisim.DegerAl(r, 9, "");
                    donusDegeri.teslimTarihi = VeriErisim.DegerAl(r, 10);
                    donusDegeri.isinSuresi = VeriErisim.DegerAl(r, 11, "");
                    donusDegeri.bitimTarihi = VeriErisim.DegerAl(r, 12);
                    donusDegeri.sozlesmeKDVBedeli = VeriErisim.DegerAl(r, 13, (double)-1);

                    r.Close();
                    r = to.ListeleOdemePlani(kriter);
                    while (r.Read())
                    {
                        HakedisOdemePlan h = new HakedisOdemePlan();
                        h.yil = VeriErisim.DegerAl(r, 0, 0);
                        h.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        h.birim = VeriErisim.DegerAl(r, 2, "");
                        h.surecNo = VeriErisim.DegerAl(r, 3, "");
                        h.siraNo = VeriErisim.DegerAl(r, 4, 0);
                        h.odemeTarihi = VeriErisim.DegerAl(r, 5);
                        h.odemeTutari = VeriErisim.DegerAl(r, 6, 0.0);
                        h.fiyatFarkiOrani = VeriErisim.DegerAl(r, 7, 0.0);
                        h.iscilikFiyatFarki = VeriErisim.DegerAl(r, 8, 0.0);
                        h.avansMiktari = VeriErisim.DegerAl(r, 9, 0.0);
                        h.prFaturaNo = VeriErisim.DegerAl(r, 10, 0);
                        //Faturadan gelenler
                        h.faturaNo = VeriErisim.DegerAl(r, 11, "");
                        h.odemeEmriNo = VeriErisim.DegerAl(r, 12, "");

                        donusDegeri.odemePlan.Ekle(h);
                    }

                    r.Close();
                    r = to.ListeleSureUzatim(kriter);
                    while (r.Read())
                    {
                        HakedisSureUzatim h = new HakedisSureUzatim();
                        h.yil = VeriErisim.DegerAl(r, 0, 0);
                        h.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        h.birim = VeriErisim.DegerAl(r, 2, "");
                        h.surecNo = VeriErisim.DegerAl(r, 3, "");
                        h.siraNo = VeriErisim.DegerAl(r, 4, 0);
                        h.onayTarihi = VeriErisim.DegerAl(r, 5);
                        h.onayNo = VeriErisim.DegerAl(r, 6, "");
                        h.sure = VeriErisim.DegerAl(r, 7, 0);
                        h.bitimTarihi = VeriErisim.DegerAl(r, 8);

                        donusDegeri.sureUzatim.Ekle(h);
                    }

                    r.Close();
                    r = to.ListeleEkSozlesme(kriter);
                    while (r.Read())
                    {
                        HakedisEkSozlesme h = new HakedisEkSozlesme();
                        h.yil = VeriErisim.DegerAl(r, 0, 0);
                        h.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        h.birim = VeriErisim.DegerAl(r, 2, "");
                        h.surecNo = VeriErisim.DegerAl(r, 3, "");
                        h.siraNo = VeriErisim.DegerAl(r, 4, 0);
                        h.onayTarihi = VeriErisim.DegerAl(r, 5);
                        h.onayNo = VeriErisim.DegerAl(r, 6, "");
                        h.sozlesmeBedeli = VeriErisim.DegerAl(r, 7, 0.0);

                        donusDegeri.ekSozlesme.Ekle(h);
                    }
                }
                r.Close();
            }

            ve.VeriErisimKapat();

            return donusDegeri;
        }

        /// <summary>
        /// Arguman olarak verilen, hakediş ödeme planına fatura numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="yil">İşlem yapılacak yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <param name="hakedisSiraNo">İşlem yapılacak hakediş sıra numarası bilgisi</param>
        /// <param name="faturaNo">İşlem yapılacak fatura numarası bilgisi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc FaturaNoKaydet(Kullanici kullanan, int yil, string surecNo, int hakedisSiraNo, int faturaNo)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHakedis to = new TABHakedis(ve);
            ve.BeginTransaction();

            sonuc = to.FaturaNoKaydet(yil, surecNo, hakedisSiraNo, faturaNo);

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
        /// Arguman olarak verilen, hakediş ödeme planı bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemePlaniSil(Kullanici kullanan, HakedisOdemePlan bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, bilgi.yil, bilgi.surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            if (bilgi.siraNo == 0)
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00132;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABHakedis to = new TABHakedis(ve);

            ve.BeginTransaction();

            sonuc = to.SilOdemePlani(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018 + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }


        private Hakedis ListeleOnBilgi(Kullanici kullanan, Hakedis kriter)
        {
            Hakedis donusDegeri = new Hakedis();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABHakedis to = new TABHakedis(ve);
            IDataReader r = to.ListeleOnBilgi(kriter);

            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    donusDegeri.yil = VeriErisim.DegerAl(r, 0, 0);
                    donusDegeri.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    donusDegeri.birim = VeriErisim.DegerAl(r, 2, "");
                    donusDegeri.surecNo = VeriErisim.DegerAl(r, 3, "");
                    donusDegeri.sozlesmeBedeli = VeriErisim.DegerAl(r, 4, 0);
                    donusDegeri.ihaleTarihi = VeriErisim.DegerAl(r, 5);
                    donusDegeri.ihaleKararTarihi = VeriErisim.DegerAl(r, 6);
                    donusDegeri.ihaleKararNo = VeriErisim.DegerAl(r, 7, "");
                    donusDegeri.sozlesmeTarihi = VeriErisim.DegerAl(r, 8);
                    donusDegeri.sozlesmeNo = VeriErisim.DegerAl(r, 9, "");
                    donusDegeri.teslimTarihi = VeriErisim.DegerAl(r, 10);
                    donusDegeri.isinSuresi = VeriErisim.DegerAl(r, 11, "");
                    donusDegeri.bitimTarihi = VeriErisim.DegerAl(r, 12);
                    donusDegeri.sozlesmeKDVBedeli = VeriErisim.DegerAl(r, 13, (double)-1);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return donusDegeri;
        }

        private ObjectArray BHMAlimHarcamaListesiSozlesme(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));

            TABHakedis to = new TABHakedis(ve);
            IDataReader r = to.ListeleButceHazirlikAlimHarcamaSozlesme(yil, ay, muhasebe, birim, kur, fon, fin, eko, peb);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    BHMHarcama bilgi = new BHMHarcama();

                    bilgi.islemYil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.islemAy = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.surecYil = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 5, "");
                    bilgi.kur = VeriErisim.DegerAl(r, 6, "");
                    bilgi.fon = VeriErisim.DegerAl(r, 7, "");
                    bilgi.fin = VeriErisim.DegerAl(r, 8, "");
                    bilgi.eko = VeriErisim.DegerAl(r, 9, "");
                    bilgi.peb = VeriErisim.DegerAl(r, 10, "");
                    bilgi.toplamTutar = VeriErisim.DegerAl(r, 11, (decimal)0);
                    bilgi.yillaraSariIslemMi = Convert.ToBoolean(VeriErisim.DegerAl(r, 12, 0));

                    if (bilgi.yillaraSariIslemMi)
                        bilgi.yil = bilgi.surecYil + 1;
                    else
                        bilgi.yil = bilgi.surecYil;

                    liste.objeler.Add(bilgi);
                }
            }
            r.Close();
            ve.VeriErisimKapat();
            return liste;
        }

    }
}

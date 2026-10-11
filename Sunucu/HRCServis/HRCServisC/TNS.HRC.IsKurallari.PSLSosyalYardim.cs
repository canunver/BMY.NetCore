using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.PER;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Personel sosyal yardım belgesi bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCSosyalYardim
    {
        /// <summary>
        /// Personel modülü servisine ulaşmak için kullanılan değişken
        /// </summary>
        public static IPERServis servis = TNS.PER.Arac.Tanimla();

        /// <summary>
        /// Arguman olarak verilen, personel sosyal yardım belgesi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="list">Kaydedilecek bilgileri PSLSosyalYardimi nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray list, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(true, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");
            //if (!HRCYetkiKontrol.Yoneticimi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}
            string hataStr = "";


            PSLSosyalYardimi sosyalYardim = new PSLSosyalYardimi();
            if (list.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                sosyalYardim = (PSLSosyalYardimi)list.objeler[0];
                if (sosyalYardim.yil == 0)
                    hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00172 + "<br>";

                if (sosyalYardim.muhasebe == 0)
                    hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00173 + "<br>";

                if (sosyalYardim.birim.Trim() == "")
                    hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00174 + "<br>";

                if (sosyalYardim.surecNo == "")
                    hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00175 + "<br>";

                if (sosyalYardim.TCKimlikNo == "")
                    hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00294 + "<br>";
                else
                {
                    TNS.PER.Personel personel = new TNS.PER.Personel();
                    //personel.birimKodu = sosyalYardim.birim;
                    //personel.muhasebeKodu = sosyalYardim.muhasebe;
                    personel.tcKimlikNo = sosyalYardim.TCKimlikNo;
                    ObjectArray ObjectArray = servis.PersonelListele(kullanan, personel);

                    if (ObjectArray.sonuc.islemSonuc)
                    {
                        TNS.PER.Personel bulunanPer = (TNS.PER.Personel)ObjectArray.objeler[0];

                        foreach (PSLSosyalYardimi yardim in list.objeler)
                        {
                            yardim.unvanKodu = bulunanPer.unvanKodu;
                            yardim.adSoyad = bulunanPer.ad + " " + bulunanPer.soyad;
                        }
                    }
                    else
                        hataStr = ObjectArray.sonuc.hataStr;//Hata var ise geri dön uyarı ver
                }
            }

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////
            bool tamamlandi = true;

            int yil = sosyalYardim.yil;
            string surecNo = sosyalYardim.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }

            ///////////////////////////////////////////////////////////////////////////

            //1.ADIM maaş temel bilgisinden aylik katsayiyi alacaz
            //PSLMaasTemelBilgisi maasKriter = new PSLMaasTemelBilgisi();
            //maasKriter.ay = sosyalYardim.olayAyi;
            //maasKriter.yil = sosyalYardim.olayYili;
            //ObjectArray maasTemelList = new HRCMaasTemelBilgisi().Listele(kullanan, maasKriter);
            PSLMaasTemelBilgisi maasTemelBilgisi = new PSLMaasTemelBilgisi();
            maasTemelBilgisi = DondurMaasTemelBilgisi(kullanan, sosyalYardim);

            //if (maasTemelList.ObjeSayisi > 0)
            //    maasTemelBilgisi = (PSLMaasTemelBilgisi)maasTemelList[0];
            //else
            //    hataStr += "Aylik katsayi bilgisi alınamadı.<br>";

            if (maasTemelBilgisi == null)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00295 + "<br>";

            //2.ADIM sosyal yardim bilgisinden ilgili ayin katsayilarını al
            //PSLSosyalYardimTemelBilgisi sosYardBilgisiKriter = new PSLSosyalYardimTemelBilgisi();
            //sosYardBilgisiKriter.ay = sosyalYardim.olayAyi;
            //sosYardBilgisiKriter.yil = sosyalYardim.olayYili;
            //ObjectArray sosYardTemelList = new HRCSosYardTemelBilgisi().Listele(kullanan, sosYardBilgisiKriter);
            PSLSosyalYardimTemelBilgisi sosYardTemelBilgisi = new PSLSosyalYardimTemelBilgisi();
            sosYardTemelBilgisi = DondurSosyalYardimTemelBilgisi(kullanan, sosyalYardim);

            if (sosYardTemelBilgisi == null)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00295 + "<br>";

            //if (sosYardTemelList.ObjeSayisi > 0)
            //    sosYardTemelBilgisi = (PSLSosyalYardimTemelBilgisi)sosYardTemelList[0];
            //else
            //    hataStr += "Sosyal yardım temel bilgisi alınamadı.<br>";


            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //3.ADIM gerekli hesaplamalar yapılır ve kaydetme aşamasına geçilir
            foreach (PSLSosyalYardimi sosYardim in list.objeler)
            {
                if (sosYardim.olayAyi != sosYardTemelBilgisi.ay || sosYardim.olayYili != sosYardTemelBilgisi.yil)
                {
                    maasTemelBilgisi = DondurMaasTemelBilgisi(kullanan, sosYardim);
                    if (maasTemelBilgisi == null)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00296, sosYardim.olayYili, sosYardim.olayAyi) + "<br>";

                    sosYardTemelBilgisi = DondurSosyalYardimTemelBilgisi(kullanan, sosYardim);
                    if (sosYardTemelBilgisi == null)
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00296, sosYardim.olayYili, sosYardim.olayAyi) + "<br>";

                    if (hataStr != "")
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = hataStr;
                        return sonuc;
                    }
                }
                switch (sosYardim.olayNiteligi)
                {
                    case (int)ENUMOlayNiteligi.DOGUM:
                        sosYardim.tutar = sosYardTemelBilgisi.dogumYardimGost * maasTemelBilgisi.aylikKatsayi;
                        sosYardim.oran = sosYardTemelBilgisi.dogumYardimGost;
                        sosYardim.aylikKatsayi = maasTemelBilgisi.aylikKatsayi;
                        break;

                    case (int)ENUMOlayNiteligi.OLUM:
                        if (sosYardim.yakinlikDerecesi == (int)ENUMYakinlikDerecesi.KENDISI)
                        {
                            sosYardim.tutar = maasTemelBilgisi.enYuksMemurMaasGost * maasTemelBilgisi.aylikKatsayi * sosYardTemelBilgisi.memurOlumKatsayi;
                            sosYardim.katsayi = sosYardTemelBilgisi.memurOlumKatsayi;
                        }
                        else
                        {
                            sosYardim.tutar = maasTemelBilgisi.enYuksMemurMaasGost * maasTemelBilgisi.aylikKatsayi * sosYardTemelBilgisi.esCocukOlumKatsayi;
                            sosYardim.katsayi = sosYardTemelBilgisi.esCocukOlumKatsayi;
                        }
                        sosYardim.enYuksMemurMaasi = maasTemelBilgisi.enYuksMemurMaasGost;

                        break;

                    case (int)ENUMOlayNiteligi.AILEYARDIMI:
                        if (sosYardim.yakinlikDerecesi == (int)ENUMYakinlikDerecesi.ESI)
                        {
                            sosYardim.tutar = sosYardTemelBilgisi.aileYardimGost * maasTemelBilgisi.aylikKatsayi;
                            sosYardim.oran = sosYardTemelBilgisi.aileYardimGost;
                        }
                        else if (sosYardim.yakinlikDerecesi == (int)ENUMYakinlikDerecesi.COCUK6DANKUCUK)
                        {
                            sosYardim.tutar = sosYardTemelBilgisi.cocukYardimGost0_6 * maasTemelBilgisi.aylikKatsayi;
                            sosYardim.oran = sosYardTemelBilgisi.cocukYardimGost0_6;
                        }
                        else if (sosYardim.yakinlikDerecesi == (int)ENUMYakinlikDerecesi.COCUK6DANBUYUK)
                        {
                            sosYardim.tutar = sosYardTemelBilgisi.cocukYardimGost6_X * maasTemelBilgisi.aylikKatsayi;
                            sosYardim.oran = sosYardTemelBilgisi.cocukYardimGost6_X;
                        }
                        break;
                }
                sosYardim.aylikKatsayi = maasTemelBilgisi.aylikKatsayi;
                sosYardim.tutar = Math.Round(sosYardim.tutar, 2);

            }

            //4.ADIM kaydetme aşaması
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSosyalYardim to = new TABSosyalYardim(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(sosyalYardim);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(list);

            if (sonuc.islemSonuc)
            {
                //Süreç devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().SurecDurumKaydet(null, kullanan, yil, surecNo, ENUMSurecDurum.DEVAMEDIYOR);
                ///////////////////////////////////////////////////////////////////////////

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

            ve.VeriErisimKapat();
            return sonuc;
        }

        private PSLMaasTemelBilgisi DondurMaasTemelBilgisi(Kullanici kullanan, PSLSosyalYardimi sosyalYardim)
        {
            PSLMaasTemelBilgisi maasKriter = new PSLMaasTemelBilgisi();
            maasKriter.ay = sosyalYardim.olayAyi;
            maasKriter.yil = sosyalYardim.olayYili;
            ObjectArray maasTemelList = new HRCMaasTemelBilgisi().Listele(kullanan, maasKriter);
            PSLMaasTemelBilgisi maasTemelBilgisi = new PSLMaasTemelBilgisi();


            if (maasTemelList.ObjeSayisi > 0)
            {
                maasTemelBilgisi = (PSLMaasTemelBilgisi)maasTemelList[0];
                return maasTemelBilgisi;
            }
            else
                return maasTemelBilgisi;
        }

        private PSLSosyalYardimTemelBilgisi DondurSosyalYardimTemelBilgisi(Kullanici kullanan, PSLSosyalYardimi sosyalYardim)
        {
            PSLSosyalYardimTemelBilgisi sosYardBilgisiKriter = new PSLSosyalYardimTemelBilgisi();
            sosYardBilgisiKriter.ay = sosyalYardim.olayAyi;
            sosYardBilgisiKriter.yil = sosyalYardim.olayYili;
            ObjectArray sosYardTemelList = new HRCSosYardTemelBilgisi().Listele(kullanan, sosYardBilgisiKriter);
            PSLSosyalYardimTemelBilgisi sosYardTemelBilgisi = new PSLSosyalYardimTemelBilgisi();

            if (sosYardTemelList.ObjeSayisi > 0)
            {
                sosYardTemelBilgisi = (PSLSosyalYardimTemelBilgisi)sosYardTemelList[0];
                return sosYardTemelBilgisi;
            }
            else
                return sosYardTemelBilgisi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, personel sosyal yardım belgesi bilgisini veritabanından okur 
        /// ve PSLSosyalYardimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, personel sosyal yardım belgesi bilgisi listesini PSLSosyalYardimi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, PSLSosyalYardimi kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSosyalYardim to = new TABSosyalYardim(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    PSLSosyalYardimi bilgi = new PSLSosyalYardimi();

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.TCKimlikNo = VeriErisim.DegerAl(r, 5, "");
                    bilgi.yakinTCKimlikNo = VeriErisim.DegerAl(r, 6, "");
                    bilgi.yakinlikDerecesi = VeriErisim.DegerAl(r, 7, 0);
                    bilgi.olayYili = VeriErisim.DegerAl(r, 8, 0);
                    bilgi.olayAyi = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.aylikKatsayi = (decimal)VeriErisim.DegerAl(r, 10, 0.0);
                    bilgi.katsayi = (decimal)VeriErisim.DegerAl(r, 11, 0.0);
                    bilgi.enYuksMemurMaasi = (decimal)VeriErisim.DegerAl(r, 12, 0.0);
                    bilgi.oran = (decimal)VeriErisim.DegerAl(r, 13, 0.0);
                    bilgi.tutar = (decimal)VeriErisim.DegerAl(r, 14, 0.0);
                    bilgi.olayNiteligi = VeriErisim.DegerAl(r, 15, 0);
                    bilgi.olayTarihi = VeriErisim.DegerAl(r, 16);
                    bilgi.adSoyad = VeriErisim.DegerAl(r, 17, "");
                    bilgi.yakinAdSoyad = VeriErisim.DegerAl(r, 18, "");
                    bilgi.unvanKodu = VeriErisim.DegerAl(r, 19, 0);
                    bilgi.odemeEmriNo = VeriErisim.DegerAl(r, 20, "");
                    bilgi.unvanAdi = VeriErisim.DegerAl(r, 21, "");
                    //bilgi.birimAd = VeriErisim.DegerAl(r, 22, "");
                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }

        /// <summary>
        /// Arguman olarak verilen, personel sosyal yardım belgesine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, PSLSosyalYardimi kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSosyalYardim to = new TABSosyalYardim(ve);
            ve.BeginTransaction();

            sonuc = to.OdemeEmriNoKaydet(kriter);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, ödeme emri belge numarası bilgisini veritabanından okur 
        /// ve PSLSosyalYardimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">İşlem yapılacak süreç yıl bilgisi</param>
        /// <param name="surecNo">İşlem yapılacak süreç no bilgisi</param>
        /// <returns>İstenilen kriterdeki, ödeme emri belge numarası bilgisi listesini PSLSosyalYardimi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleOdemeEmriNo(int yil, string surecNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABSosyalYardim toSosyalYardim = new TABSosyalYardim(ve);

            IDataReader dr = toSosyalYardim.ListeleOdemeEmri(yil, surecNo);

            while (dr.Read())
            {
                PSLSosyalYardimi sd = new PSLSosyalYardimi();
                sd.yil = VeriErisim.DegerAl(dr, 0, 0);
                sd.odemeEmriNo = VeriErisim.DegerAl(dr, 1, "");

                bilgi.Ekle(sd);
            }
            dr.Close();

            if (bilgi.ObjeSayisi == 0)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00287;
            }
            else
                bilgi.sonuc.islemSonuc = true;

            ve.VeriErisimKapat();
            return bilgi;
        }
    }

}

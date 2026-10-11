using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System.Collections;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Rapor bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCRaporlar
    {
        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, rapor bilgisini veritabanından okur 
        /// ve ilgili nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, rapor bilgisi listesini ilgili nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray RaporAl(Kullanici kullanan, RaporKriter kriter)
        {
            ObjectArray raporListe = new ObjectArray();

            //PSLYollukIstatistikSinif sb = new PSLYollukIstatistikSinif();
            //sb.yil = 2009;
            //raporListe = (new PSLSurekliGorevYolluk()).ListeleIstatistikKurlu(kullanan, sb);
            //return raporListe;

            switch (kriter.raporTipi)
            {
                case (int)ENUMRaporTuru.MALALIMIHARACAMATAKIP:

                    kriter.islemKodu = 1;
                    if (kriter.raporSekli == (int)ENUMRaporSekli.OZET)
                        raporListe = MHYAlimiHarcamaTakipOzetRaporu(kriter);
                    else if (kriter.raporSekli == (int)ENUMRaporSekli.DETAY)
                        raporListe = MHYAlimiHarcamaTakipDetayRaporu(kriter);
                    break;
                case (int)ENUMRaporTuru.HIZMETALIMIHARACAMATAKIP:
                    kriter.islemKodu = 2;
                    if (kriter.raporSekli == (int)ENUMRaporSekli.OZET)
                        raporListe = MHYAlimiHarcamaTakipOzetRaporu(kriter);
                    else if (kriter.raporSekli == (int)ENUMRaporSekli.DETAY)
                        raporListe = MHYAlimiHarcamaTakipDetayRaporu(kriter);
                    break;

                case (int)ENUMRaporTuru.YAPIMISIHARACAMATAKIP:
                    kriter.islemKodu = 3;
                    if (kriter.raporSekli == (int)ENUMRaporSekli.OZET)
                        raporListe = MHYAlimiHarcamaTakipOzetRaporu(kriter);
                    else if (kriter.raporSekli == (int)ENUMRaporSekli.DETAY)
                        raporListe = MHYAlimiHarcamaTakipDetayRaporu(kriter);
                    break;

                case (int)ENUMRaporTuru.MHYIHALETURUTOPLAM:
                    raporListe = MHYIhaleTurToplamlariRaporu(kriter);
                    break;

                case (int)ENUMRaporTuru.MHYILTOPLAMLARI:
                    raporListe = MHYIlDagilimlariRaporu(kriter);
                    break;

                case (int)ENUMRaporTuru.URUNMUKAYESE:
                    kriter.islemKodu = 1;
                    if (kriter.raporSekli == (int)ENUMRaporSekli.OZET)
                        raporListe = AlinmisUrunMukayeseOzetRaporu(kriter);
                    else if (kriter.raporSekli == (int)ENUMRaporSekli.DETAY)
                        raporListe = AlinmisUrunMukayeseDetayRaporu(kriter);

                    break;

                case (int)ENUMRaporTuru.SGYOLLUGUOZET:

                    SGYollukOzet(kriter);
                    break;
                case (int)ENUMRaporTuru.MERKEZBIRIMLERIDAGILIMTABLOSU:
                    if (kriter.raporSekli == (int)ENUMRaporSekli.OZET)
                        raporListe = IlEkonomikKodBazliMukayeseHRCTakipOzet(kriter);
                    break;

                case (int)ENUMRaporTuru.ABONELIKISTATISTIK:
                    raporListe = AbonelikIstatistik(kriter);
                    break;
                case (int)ENUMRaporTuru.PERSONELISTATISTIK:
                    raporListe = PersonelIstatistik(kriter);
                    break;
            }

            return raporListe;
        }


        private ObjectArray SGYollukOzet(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRSGYollukOzet rapor = new RPRSGYollukOzet();

            IDataReader r = null;
            try
            {
                r = tab.SGYollukOzetEmekli(kriter);
            }
            catch
            {

            }
            if (r.Read())
            {

                rapor.emekliTutar = VeriErisim.DegerAl(r, 0, 0.0);
                rapor.emekliSayi = VeriErisim.DegerAl(r, 1, 0);

            }
            r.Close();

            try
            {
                r = tab.SGYollukOzetAtanmis(kriter);
            }
            catch
            {

            }
            if (r.Read())
            {

                rapor.atanmisTutar = VeriErisim.DegerAl(r, 0, 0.0);
                rapor.atanmisSayi = VeriErisim.DegerAl(r, 1, 0);

            }
            r.Close();

            try
            {
                r = tab.SGYollukOzetIlListesi(kriter);
            }
            catch
            {

            }

            ArrayList al = new ArrayList();
            RPRYollukIlOzet ilOzet = new RPRYollukIlOzet();
            while (r.Read())
            {

                ilOzet.ilAd = VeriErisim.DegerAl(r, 0, "");
                ilOzet.ilKod = VeriErisim.DegerAl(r, 1, 0);
                ilOzet.ilAdet = VeriErisim.DegerAl(r, 2, 0);
                ilOzet.ilTutar = VeriErisim.DegerAl(r, 3, 0.0);

            }
            r.Close();

            ve.VeriErisimKapat();

            return returnList;
        }

        private ObjectArray IlEkonomikKodBazliMukayeseHRCTakipOzet(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRIlEkonomikKodBazliMukayeseHRCTakipOzet donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
            decimal genelToplam = 0;
            decimal turkiyeOrt = 0;
            int tumPersonelSayisi = 0;


            IDataReader r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetTumPersonelSayisiHesapla(kriter);
            }
            catch
            {
            }

            if (r.Read())
            {
                tumPersonelSayisi = VeriErisim.DegerAl(r, 0, 0);
            }
            r.Close();
            r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetTurkiyeToplamiHesapla(kriter);
            }
            catch
            {
            }

            if (r.Read())
            {
                genelToplam = (decimal)VeriErisim.DegerAl(r, 0, 0.0);
                turkiyeOrt = genelToplam / tumPersonelSayisi;
            }
            r.Close();
            r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetMerkezToplamTutarHesapla(kriter);
            }
            catch
            {
            }

            while (r.Read())
            {
                donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();

                donenNesne.muhasebeAd = VeriErisim.DegerAl(r, 0, "");
                donenNesne.ilKodu = VeriErisim.DegerAl(r, 1, 0);
                donenNesne.ilAdi = VeriErisim.DegerAl(r, 2, "");
                donenNesne.muhasebeKod = VeriErisim.DegerAl(r, 3, 0);
                donenNesne.birimKod = VeriErisim.DegerAl(r, 4, "");
                donenNesne.toplamHarcTutari = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                donenNesne.personelSayi = VeriErisim.DegerAl(r, 6, 0);
                donenNesne.kisiBasiOrt = (decimal)VeriErisim.DegerAl(r, 7, 0.0);
                donenNesne.ortalamadanSapma = (100 * turkiyeOrt / donenNesne.kisiBasiOrt) - 100;
                donenNesne.toplamIcindekiPayi = (100 * donenNesne.toplamHarcTutari) / genelToplam;
                returnList.Ekle(donenNesne);
            }
            r.Close();
            ve.VeriErisimKapat();
            return returnList;
        }

        private ObjectArray AlinmisUrunMukayeseOzetRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRUrunMukayeseHRCTakipOzet donenNesne = new RPRUrunMukayeseHRCTakipOzet();

            AlinmisUrunMukayeseOzetToplamTutarHesapla(tab, kriter, donenNesne);

            returnList.Ekle(donenNesne);

            IDataReader r = null;
            int maxCount = 10;
            int sonIndeks = 0;
            try
            {
                r = tab.AlinmisUrunMukayeseOzetSiraliBirimFiyatHesapla(kriter, true);
            }
            catch
            {
            }

            while (r.Read() && sonIndeks < maxCount)
            {
                donenNesne = new RPRUrunMukayeseHRCTakipOzet();
                donenNesne.enDusukFiyattanAlinanMiktar = (decimal)VeriErisim.DegerAl(r, 0, 0.0);
                donenNesne.enDusukBirimFiyat = (decimal)VeriErisim.DegerAl(r, 1, 0.0);
                returnList.Ekle(donenNesne);
                sonIndeks++;
            }
            r.Close();

            r = null;

            try
            {
                r = tab.AlinmisUrunMukayeseOzetSiraliBirimFiyatHesapla(kriter, false);
            }
            catch
            {
            }
            int sira = 1;
            while (r.Read() && sira < sonIndeks)
            {
                donenNesne = ((RPRUrunMukayeseHRCTakipOzet)returnList.objeler[sira]);
                donenNesne.enYuksekFiyattanAlinanMiktar = (decimal)VeriErisim.DegerAl(r, 0, 0.0);
                donenNesne.enYuksekBirimFiyat = (decimal)VeriErisim.DegerAl(r, 1, 0.0);

                sira++;
            }
            r.Close();
            ve.VeriErisimKapat();
            return returnList;
        }

        private ObjectArray AlinmisUrunMukayeseDetayRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRUrunMukayeseHRCTakipOzet urunOzetRaporu = new RPRUrunMukayeseHRCTakipOzet();

            AlinmisUrunMukayeseOzetToplamTutarHesapla(tab, kriter, urunOzetRaporu);

            RPRUrunMukayeseHRCTakipDetay donenNesne = new RPRUrunMukayeseHRCTakipDetay();

            IDataReader r = null;

            try
            {
                r = tab.AlinmisUrunMukayeseDetayToplamTutarHesapla(kriter);
            }
            catch
            {
            }

            while (r.Read())
            {
                donenNesne = new RPRUrunMukayeseHRCTakipDetay();
                donenNesne.muhasebeKod = VeriErisim.DegerAl(r, 0, 0);
                donenNesne.birimKod = VeriErisim.DegerAl(r, 1, "");
                donenNesne.surecNo = VeriErisim.DegerAl(r, 2, "");
                donenNesne.toplamTutar = (decimal)VeriErisim.DegerAl(r, 3, 0.0);
                donenNesne.miktar = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                donenNesne.birimFiyat = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                donenNesne.alisTarihi = VeriErisim.DegerAl(r, 6);
                donenNesne.muhasebeAdi = VeriErisim.DegerAl(r, 7, "");
                donenNesne.birimAdi = VeriErisim.DegerAl(r, 8, "");

                donenNesne.sapmaTutari = donenNesne.birimFiyat - urunOzetRaporu.urunOrtBirimFiyat;
                donenNesne.sapmaOrani = (donenNesne.birimFiyat - urunOzetRaporu.urunOrtBirimFiyat) / 100;

                returnList.Ekle(donenNesne);
            }
            r.Close();
            ve.VeriErisimKapat();
            return returnList;
        }

        private int AlinmisUrunMukayeseOzetToplamTutarHesapla(TABRaporlar tab, RaporKriter kriter, RPRUrunMukayeseHRCTakipOzet ozetRapor)
        {
            IDataReader r = null;
            try
            {
                r = tab.AlinmisUrunMukayeseOzetToplamTutarHesapla(kriter);
            }
            catch
            {
                return 0;
            }
            if (r.Read())
            {
                ozetRapor.urunKodAd = VeriErisim.DegerAl(r, 0, "");
                ozetRapor.muhasebeSayi = VeriErisim.DegerAl(r, 1, 0);
                ozetRapor.birimSayi = VeriErisim.DegerAl(r, 2, 0);
                ozetRapor.toplamIslemSayi = VeriErisim.DegerAl(r, 3, 0);
                ozetRapor.urunToplamTutari = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                ozetRapor.urunMiktari = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                ozetRapor.urunOrtBirimFiyat = (decimal)VeriErisim.DegerAl(r, 6, 0.0);

            }
            r.Close();
            return 1;
        }

        private ObjectArray MHYAlimiHarcamaTakipDetayRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            ObjectArray detayListe = new ObjectArray();
            ObjectArray ozetListe = new ObjectArray();

            ozetListe = MHYAlimiHarcamaTakipOzetRaporu(kriter);
            returnList.Ekle(ozetListe);

            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRMalHizmetYapimIsiHRCTakipDetay detayRapor = new RPRMalHizmetYapimIsiHRCTakipDetay();

            IDataReader r = null;
            string birOncekiSurec = "";
            try
            {
                r = tab.MHYDetayToplamTutarHesapla(kriter);
            }
            catch
            {

            }
            int devamEdenParcaKodu = 0;
            if (r != null)
            {
                while (r.Read())
                {
                    detayRapor = new RPRMalHizmetYapimIsiHRCTakipDetay();
                    detayRapor.surecNo = VeriErisim.DegerAl(r, 0, "");
                    detayRapor.muhasebeKod = VeriErisim.DegerAl(r, 1, 0);
                    detayRapor.birimKod = VeriErisim.DegerAl(r, 2, "");
                    detayRapor.onayTarihi = VeriErisim.DegerAl(r, 3);
                    if (birOncekiSurec != detayRapor.surecNo)
                    {
                        detayRapor.yaklasikMaliyet = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                        detayRapor.alimTutari = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                        detayRapor.odemeTahakkukTutar = (decimal)VeriErisim.DegerAl(r, 10, 0.0);
                    }

                    detayRapor.firmaKodu = VeriErisim.DegerAl(r, 6, 0);
                    detayRapor.firmaAdi = VeriErisim.DegerAl(r, 7, "");
                    detayRapor.faturaTutari = (decimal)VeriErisim.DegerAl(r, 8, 0.0);
                    detayRapor.faturaTarihi = VeriErisim.DegerAl(r, 9);

                    if (detayRapor.faturaTutari == (decimal)VeriErisim.DegerAl(r, 10, 0.0))
                    {
                        detayRapor.odemeTahakkukTutar = (decimal)VeriErisim.DegerAl(r, 10, 0.0);

                    }
                    detayRapor.odemeTarihi = VeriErisim.DegerAl(r, 11);
                    detayRapor.muhasebeAdi = VeriErisim.DegerAl(r, 12, "");
                    devamEdenParcaKodu = VeriErisim.DegerAl(r, 13, 0);
                    detayRapor.surecAsama = MHYParcaAsmasiniDonder(kriter.islemKodu, devamEdenParcaKodu);
                    detayRapor.ekoKodOnay = VeriErisim.DegerAl(r, 15, "");
                    if (VeriErisim.DegerAl(r, 16, "") != "")
                        detayRapor.ekoKodOnay += "," + VeriErisim.DegerAl(r, 16, "");

                    detayRapor.birimAdi = VeriErisim.DegerAl(r, 17, "");

                    if (kriter.odemeEmriGoster)
                    {
                        detayRapor.ekoKodOdemeEmri = VeriErisim.DegerAl(r, 18, "");
                        detayRapor.odemeEmriNo = VeriErisim.DegerAl(r, 19, "");
                    }
                    birOncekiSurec = detayRapor.surecNo;
                    //detayRapor.birimAdi = VeriErisim.DegerAl(r, 13, "");

                    detayListe.Ekle(detayRapor);
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            returnList.Ekle(detayListe);
            return returnList;

        }

        private string MHYParcaAsmasiniDonder(int islemKod, int devamEdenParcaKodu)
        {
            string surecAsama = "";
            switch (islemKod)
            {
                case (int)ENUMIhaleTur.MAL:
                    if (devamEdenParcaKodu < 2)
                    {
                        surecAsama = "Y.Maliyet";
                    }
                    else if (devamEdenParcaKodu == 2)
                    {
                        surecAsama = "Onayda";
                    }
                    else if (devamEdenParcaKodu <= 6)
                    {
                        surecAsama = "İlan";
                    }
                    else if (devamEdenParcaKodu < 10)
                    {
                        surecAsama = "İhale";
                    }
                    else if (devamEdenParcaKodu == 10)
                    {
                        surecAsama = "T.Değerlendirme";
                    }
                    else if (devamEdenParcaKodu == 11)
                    {
                        surecAsama = "Fatura";
                    }
                    else if (devamEdenParcaKodu > 11 && 14 > devamEdenParcaKodu)
                    {
                        surecAsama = "Taşınır";
                    }
                    else if (devamEdenParcaKodu == 14)
                    {
                        surecAsama = "Ödeme Emri";
                    }
                    else if (devamEdenParcaKodu == 19)
                    {
                        surecAsama = "Hakediş";
                    }
                    else if (devamEdenParcaKodu > 20 && devamEdenParcaKodu < 25)
                    {
                        surecAsama = "İhale 2.Aşama";
                    }
                    break;



            }

            return surecAsama;

        }

        private ObjectArray MHYAlimiHarcamaTakipOzetRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRMalHizmetYapimIsiHRCTakipOzet donenNesne = new RPRMalHizmetYapimIsiHRCTakipOzet();
            //OnayFormdan olusturdugum viewden çıkacaklar
            //Farklı muhasebe sayısını al
            donenNesne.muhasebeSayi = OnayFarkliMuhasebeSayisi(tab, kriter);
            //Farklı birim sayısını al
            donenNesne.birimSayi = OnayFarkliBirimSayisi(tab, kriter);

            //Toplam işlem sayısını al
            //Butun Asama Durumların Sayısı toplandıktan sonra Toplam islem sayısı bulunur

            //Onay Aşamasındaki islem sayısı ve tutarı

            kriter.parcaKod1 = 2;
            kriter.parcaDurum1 = (int)ENUMSurecParcaDurum.DEVAMEDIYOR;
            kriter.parcaDurum2 = (int)ENUMSurecParcaDurum.TAMAMLANDI;
            donenNesne.onayIslemSayi = OnayAsamaDurumSurecSayisi(tab, kriter);

            //Alım Aşamasındaki islem sayısı ve tutarı

            kriter.parcaKod1 = 2;
            kriter.parcaKod2 = 11;
            kriter.parcaDurum1 = (int)ENUMSurecParcaDurum.DEVAMEDIYOR;
            kriter.parcaDurum2 = (int)ENUMSurecParcaDurum.TAMAMLANDI;
            donenNesne.alimIslemSayi = OnayAsamaDurumSurecSayisi(tab, kriter);

            //Fatura Aşamsında işlem sayısı

            kriter.parcaKod1 = 11;
            kriter.parcaKod2 = 0;
            kriter.parcaDurum1 = (int)ENUMSurecParcaDurum.DEVAMEDIYOR;
            kriter.parcaDurum2 = (int)ENUMSurecParcaDurum.TAMAMLANDI;
            donenNesne.faturaIslemSayi = OnayAsamaDurumSurecSayisi(tab, kriter);

            //Odeme Aşamsında işlem sayısı

            kriter.parcaKod1 = 14;
            kriter.parcaKod2 = 0;
            kriter.parcaDurum1 = (int)ENUMSurecParcaDurum.DEVAMEDIYOR;
            kriter.parcaDurum2 = (int)ENUMSurecParcaDurum.TAMAMLANDI;
            donenNesne.odemeSayi = OnayAsamaDurumSurecSayisi(tab, kriter);

            kriter.parcaKod1 = 0;
            kriter.parcaKod2 = 0;
            kriter.parcaDurum1 = (int)ENUMSurecParcaDurum.TANIMSIZ;
            donenNesne.toplamIslemSayi = OnayAsamaDurumSurecSayisi(tab, kriter);

            MHYOzetToplamTutarHesapla(tab, kriter, donenNesne);

            returnList.Ekle(donenNesne);
            ve.VeriErisimKapat();
            return returnList;
        }
        //Mal Hizmet Yapım toplam tutarları hesapla

        private int MHYOzetToplamTutarHesapla(TABRaporlar tab, RaporKriter kriter, RPRMalHizmetYapimIsiHRCTakipOzet ozetRapor)
        {
            IDataReader r = null;
            try
            {
                r = tab.MHYOzetToplamTutarHesapla(kriter);
            }
            catch
            {
                return 0;
            }
            if (r.Read())
            {
                ozetRapor.onayTutar = (decimal)VeriErisim.DegerAl(r, 0, 0.0);
                ozetRapor.alimToplamTutar = (decimal)VeriErisim.DegerAl(r, 1, 0.0);
                ozetRapor.faturaToplamTutar = (decimal)VeriErisim.DegerAl(r, 2, 0.0);
                ozetRapor.odemeToplam = (decimal)VeriErisim.DegerAl(r, 3, 0.0);

            }
            r.Close();

            return 1;
        }
        //YOLLUK HARCAMA TAKİP RAPORU

        private ObjectArray MHYIhaleTurToplamlariRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRIhaleTurToplam detayRapor = new RPRIhaleTurToplam();

            IDataReader r = null;
            try
            {
                r = tab.MHYIhaleTurToplamlariRaporu(kriter);
            }
            catch
            {

            }
            while (r.Read())
            {
                detayRapor = new RPRIhaleTurToplam();
                detayRapor.islemKod = VeriErisim.DegerAl(r, 0, 0);
                detayRapor.tutar = VeriErisim.DegerAl(r, 1, 0.0);
                detayRapor.islemSayi = VeriErisim.DegerAl(r, 2, 0);
                detayRapor.ihaleUsul = VeriErisim.DegerAl(r, 3, 0);
                detayRapor.ihaleUsulMadde = VeriErisim.DegerAl(r, 4, 0);
                detayRapor.ihaleAd = VeriErisim.DegerAl(r, 5, "");

                returnList.Ekle(detayRapor);
            }
            r.Close();
            ve.VeriErisimKapat();

            return returnList;

        }

        private ObjectArray MHYIlDagilimlariRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRIhaleIlToplam detayRapor = new RPRIhaleIlToplam();

            IDataReader r = null;
            try
            {
                r = tab.MHYIlToplamlariRaporu(kriter);
            }
            catch
            {

            }
            while (r.Read())
            {
                detayRapor = new RPRIhaleIlToplam();
                detayRapor.islemKod = VeriErisim.DegerAl(r, 0, 0);
                detayRapor.tutar = VeriErisim.DegerAl(r, 1, 0.0);
                detayRapor.islemSayi = VeriErisim.DegerAl(r, 2, 0);
                detayRapor.ilKod = VeriErisim.DegerAl(r, 3, 0);
                detayRapor.ilAd = VeriErisim.DegerAl(r, 4, "");

                returnList.Ekle(detayRapor);
            }
            r.Close();
            ve.VeriErisimKapat();

            return returnList;
        }

        private int OnayAsamaDurumSurecSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retValue = 0;
            try
            {
                r = tab.OnayAsamaDurumSurecSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retValue = VeriErisim.DegerAl(r, 0, 0);
            r.Close();

            return retValue;
        }

        private int AsamaDurumSurecSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retValue = 0;
            try
            {
                r = tab.AsamaDurumSurecSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retValue = VeriErisim.DegerAl(r, 0, 0);
            r.Close();

            return retValue;
        }

        private int OnayFarkliMuhasebeSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retVal = 0;
            try
            {
                r = tab.OnayFarkliMuhasebeSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retVal = VeriErisim.DegerAl(r, 0, 0);
            r.Close();

            return retVal;
        }

        private int OnayFarkliBirimSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retVal = 0;
            try
            {
                r = tab.OnayFarkliBirimSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retVal = VeriErisim.DegerAl(r, 0, 0);

            r.Close();

            return retVal;
        }

        private int SurecFarkliMuhasebeSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retVal = 0;
            try
            {
                r = tab.SurecFarkliMuhasebeSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retVal = VeriErisim.DegerAl(r, 0, 0);
            r.Close();

            return retVal;
        }

        private int SurecFarkliBirimSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retVal = 0;
            try
            {
                r = tab.SurecFarkliBirimSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retVal = VeriErisim.DegerAl(r, 0, 0);

            r.Close();

            return retVal;
        }

        private int IslemTuruSurecSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retVal = 0;
            try
            {
                r = tab.IslemTuruSurecSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retVal = VeriErisim.DegerAl(r, 0, 0);

            r.Close();

            return retVal;
        }

        private int AboneSurecFaturaAsamaSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retVal = 0;
            try
            {
                r = tab.AboneSurecFaturaAsamaSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retVal = VeriErisim.DegerAl(r, 0, 0);

            r.Close();

            return retVal;
        }

        private int AboneSurecOdemeAsamaSayisi(TABRaporlar tab, RaporKriter kriter)
        {
            IDataReader r = null;
            int retVal = 0;
            try
            {
                r = tab.AboneSurecOdemeAsamaSayisi(kriter);
            }
            catch
            {
            }
            if (r.Read())
                retVal = VeriErisim.DegerAl(r, 0, 0);

            r.Close();

            return retVal;
        }

        private ObjectArray AbonelikIstatistik(RaporKriter kriter)
        {
            ObjectArray o = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);

            IDataReader r = tab.AbonelikIstatistik(kriter);

            while (r.Read())
            {
                RPRAbonelikIstatistik a = new RPRAbonelikIstatistik();
                a.tutar = VeriErisim.DegerAl(r, 0, 0.0);
                a.aboneSayisi = VeriErisim.DegerAl(r, 1, 0);

                if (kriter.raporSekli == 3)
                {
                    a.il = VeriErisim.DegerAl(r, 2, 0);
                    a.ilAd = OrtakFonksiyonlar.SehirAdiVer(a.il);
                }
                else if (kriter.raporSekli == 4)
                {
                    a.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    a.muhasebeAd = HRCServis.UzayYaziDegerGetir("MUHMUHASEBE", a.muhasebe.ToString(), true, "");

                }
                else if (kriter.raporSekli == 2 || kriter.raporSekli == 5)
                {
                    a.muhasebe = VeriErisim.DegerAl(r, 2, 0);
                    a.birim = VeriErisim.DegerAl(r, 3, "");
                    a.birimAd = HRCServis.UzayYaziDegerGetir("MUHMUHASEBEBIRIM", a.birim + "-" + a.muhasebe, false, "");
                }

                o.objeler.Add(a);
            }
            r.Close();

            //Personel Sayısı için
            PersonelSayisiVer(kriter, ref o);

            double ortalama = AbonelikOrtalamaIstatistik(kriter);
            foreach (RPRAbonelikIstatistik a in o.objeler)
                a.ortalama = ortalama;

            ve.VeriErisimKapat();
            return o;
        }

        private void PersonelSayisiVer(RaporKriter kriter, ref ObjectArray o)
        {
            ObjectArray per = PersonelIstatistik(kriter);
            foreach (RPRAbonelikIstatistik a in per.objeler)
            {
                foreach (RPRAbonelikIstatistik b in o.objeler)
                {
                    if (kriter.raporSekli == 3)
                    {
                        if (a.il == b.il)
                        {
                            b.personelSayisi = a.personelSayisi;
                            break;
                        }
                    }
                    else if (kriter.raporSekli == 4)
                    {
                        if (a.muhasebe == b.muhasebe)
                        {
                            b.personelSayisi = a.personelSayisi;
                            break;
                        }
                    }
                    else if (kriter.raporSekli == 2 || kriter.raporSekli == 5)
                    {
                        if (a.muhasebe == b.muhasebe && a.birim == b.birim)
                        {
                            b.personelSayisi = a.personelSayisi;
                            break;
                        }
                    }
                    else
                    {
                        b.personelSayisi = a.personelSayisi;
                        break;
                    }
                }
            }
        }

        private ObjectArray PersonelIstatistik(RaporKriter kriter)
        {
            ObjectArray o = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);

            IDataReader r = tab.PersonelIstatistik(kriter);

            while (r.Read())
            {
                RPRAbonelikIstatistik a = new RPRAbonelikIstatistik();
                a.personelSayisi = VeriErisim.DegerAl(r, 0, 0);

                if (kriter.raporSekli == 3)
                    a.il = VeriErisim.DegerAl(r, 1, 0);
                else if (kriter.raporSekli == 4)
                    a.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                else if (kriter.raporSekli == 2 || kriter.raporSekli == 5)
                {
                    a.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    a.birim = VeriErisim.DegerAl(r, 2, "");
                }

                o.objeler.Add(a);
            }
            r.Close();

            ve.VeriErisimKapat();
            return o;
        }

        private double AbonelikOrtalamaIstatistik(RaporKriter kriter)
        {
            double ortalama = 0.0;

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABRaporlar tab = new TABRaporlar(ve);

            IDataReader r = tab.AbonelikOrtalamaIstatistik(kriter);

            if (r.Read())
                ortalama = VeriErisim.DegerAl(r, 0, 0.0);

            r.Close();

            ve.VeriErisimKapat();
            return ortalama;
        }


        /*
        public ObjectArray IlBazindaEkonomikKodDetayRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRIlEkonomikKodBazliMukayeseHRCTakipOzet donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
            decimal genelToplam = 0;
            decimal turkiyeOrt = 0;
            int tumPersonelSayisi = 0;


            IDataReader r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetTumPersonelSayisiHesapla(kriter);
            }
            catch
            {
            }

            if (r.Read())
            {
                tumPersonelSayisi = VeriErisim.DegerAl(r, 0, 0);
            }
            r.Close();
            r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetTurkiyeToplamiHesapla(kriter);
            }
            catch
            {
            }

            if (r.Read())
            {
                genelToplam = (decimal)VeriErisim.DegerAl(r, 0, 0.0);
                turkiyeOrt = genelToplam / tumPersonelSayisi;
            }
            r.Close();

            r = null;

            try
            {
                r = tab.IlKarsilastirmaDetayToplamTutarHesapla(kriter);
            }
            catch
            {
            }

            while (r.Read())
            {
                donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();

                donenNesne.muhasebeAd = VeriErisim.DegerAl(r, 0, "");
                donenNesne.ilKodu = VeriErisim.DegerAl(r, 1, 0);
                donenNesne.ilAdi = VeriErisim.DegerAl(r, 2, "");
                donenNesne.muhasebeKod = VeriErisim.DegerAl(r, 3, 0);
                donenNesne.birimKod = VeriErisim.DegerAl(r, 4, "");
                donenNesne.toplamHarcTutari = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                donenNesne.personelSayi = VeriErisim.DegerAl(r, 6, 0);
                donenNesne.kisiBasiOrt = (decimal)VeriErisim.DegerAl(r, 7, 0.0);
                donenNesne.ortalamadanSapma = 100 - ((donenNesne.kisiBasiOrt * 100) / turkiyeOrt); //(100 * turkiyeOrt / donenNesne.kisiBasiOrt) - 100;
                donenNesne.toplamIcindekiPayi = (100 * donenNesne.toplamHarcTutari) / genelToplam;
                returnList.Ekle(donenNesne);
            }
            r.Close();
            ve.VeriErisimKapat();
            return returnList;
        }
        */
        /*
        public ObjectArray IlBazindaEkonomikKodOzetRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRIlEkonomikKodBazliMukayeseHRCTakipOzet donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
            decimal genelToplam = 0;
            decimal turkiyeOrt = 0;
            int tumPersonelSayisi = 0;


            IDataReader r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetTumPersonelSayisiHesapla(kriter);
            }
            catch
            {
            }

            if (r.Read())
            {
                tumPersonelSayisi = VeriErisim.DegerAl(r, 0, 0);
            }
            r.Close();
            r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetTurkiyeToplamiHesapla(kriter);
            }
            catch
            {
            }

            if (r.Read())
            {
                genelToplam = (decimal)VeriErisim.DegerAl(r, 0, 0.0);
                turkiyeOrt = genelToplam / tumPersonelSayisi;
            }
            r.Close();
            r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetToplamTutarHesapla(kriter);
            }
            catch
            {
            }

            while (r.Read())
            {
                donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
                donenNesne.ilKodu = VeriErisim.DegerAl(r, 0, 0);
                donenNesne.ilAdi = VeriErisim.DegerAl(r, 1, "");
                donenNesne.toplamHarcTutari = (decimal)VeriErisim.DegerAl(r, 2, 0.0);
                donenNesne.personelSayi = VeriErisim.DegerAl(r, 3, 0);
                donenNesne.kisiBasiOrt = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                donenNesne.ortalamadanSapma = (100 * turkiyeOrt / donenNesne.kisiBasiOrt) - 100;
                donenNesne.toplamIcindekiPayi = (100 * donenNesne.toplamHarcTutari) / genelToplam;
                returnList.Ekle(donenNesne);
            }
            r.Close();

            r = null;
            try
            {
                r = tab.IlKarsilastirmaOzetMerkezToplamTutarHesapla(kriter);
            }
            catch
            {
            }

            while (r.Read())
            {
                donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();

                donenNesne.muhasebeAd = VeriErisim.DegerAl(r, 0, "");
                donenNesne.ilKodu = VeriErisim.DegerAl(r, 1, 0);
                donenNesne.ilAdi = VeriErisim.DegerAl(r, 2, "");
                donenNesne.muhasebeKod = VeriErisim.DegerAl(r, 3, 0);
                donenNesne.birimKod = VeriErisim.DegerAl(r, 4, "");
                donenNesne.toplamHarcTutari = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                donenNesne.personelSayi = VeriErisim.DegerAl(r, 6, 0);
                donenNesne.kisiBasiOrt = (decimal)VeriErisim.DegerAl(r, 7, 0.0);
                donenNesne.ortalamadanSapma = (100 * turkiyeOrt / donenNesne.kisiBasiOrt) - 100;
                donenNesne.toplamIcindekiPayi = (100 * donenNesne.toplamHarcTutari) / genelToplam;
                returnList.Ekle(donenNesne);
            }
            r.Close();
            ve.VeriErisimKapat();
            return returnList;
        }
        */
        /*
        public ObjectArray IlBazindaEkonomikKodOzetSiralamaRaporu(RaporKriter kriter)
        {
            ObjectArray returnList = new ObjectArray();
            //baglanti ayarları
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRaporlar tab = new TABRaporlar(ve);
            RPRIlEkonomikKodBazliMukayeseHRCTakipOzet donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
            decimal genelToplam = 0;
            int enYuksekIlSayısı = 10;
            int indeks = 0;
            IDataReader r = null;
            try
            {
                r = tab.IlKarsilastirmaOzetTurkiyeToplamiHesapla(kriter);
            }
            catch
            {
            }

            if (r.Read())
            {
                genelToplam = (decimal)VeriErisim.DegerAl(r, 0, 0.0);

            }
            r.Close();
            r = null;
            try
            {
                r = tab.IlKarsilastirmaOzetToplamHarcamaBazlıSiralama(kriter, false, false);
            }
            catch
            {
            }

            while (r.Read() && indeks < enYuksekIlSayısı)
            {
                donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
                donenNesne.ilKodu = VeriErisim.DegerAl(r, 0, 0);
                donenNesne.ilAdi = VeriErisim.DegerAl(r, 1, "");
                donenNesne.toplamHarcTutari = (decimal)VeriErisim.DegerAl(r, 2, 0.0);
                donenNesne.personelSayi = VeriErisim.DegerAl(r, 3, 0);
                donenNesne.kisiBasiOrt = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                donenNesne.toplamIcindekiPayi = (100 * donenNesne.toplamHarcTutari) / genelToplam;
                returnList.Ekle(donenNesne);
                indeks++;
            }
            r.Close();

            donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
            returnList.Ekle(donenNesne);

            r = null;
            indeks = 0;
            try
            {
                r = tab.IlKarsilastirmaOzetToplamHarcamaBazlıSiralama(kriter, true, false);
            }
            catch
            {
            }

            while (r.Read() && indeks < enYuksekIlSayısı)
            {
                donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
                donenNesne.ilKodu = VeriErisim.DegerAl(r, 0, 0);
                donenNesne.ilAdi = VeriErisim.DegerAl(r, 1, "");
                donenNesne.toplamHarcTutari = (decimal)VeriErisim.DegerAl(r, 2, 0.0);
                donenNesne.personelSayi = VeriErisim.DegerAl(r, 3, 0);
                donenNesne.kisiBasiOrt = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                donenNesne.toplamIcindekiPayi = (100 * donenNesne.toplamHarcTutari) / genelToplam;
                returnList.Ekle(donenNesne);
                indeks++;
            }
            r.Close();

            donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
            returnList.Ekle(donenNesne);

            indeks = 0;
            r = null;

            try
            {
                r = tab.IlKarsilastirmaOzetToplamHarcamaBazlıSiralama(kriter, false, true);
            }
            catch
            {
            }

            while (r.Read() && indeks < enYuksekIlSayısı)
            {
                donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
                donenNesne.ilKodu = VeriErisim.DegerAl(r, 0, 0);
                donenNesne.ilAdi = VeriErisim.DegerAl(r, 1, "");
                donenNesne.toplamHarcTutari = (decimal)VeriErisim.DegerAl(r, 2, 0.0);
                donenNesne.personelSayi = VeriErisim.DegerAl(r, 3, 0);
                donenNesne.kisiBasiOrt = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                donenNesne.toplamIcindekiPayi = (100 * donenNesne.toplamHarcTutari) / genelToplam;
                returnList.Ekle(donenNesne);
                indeks++;
            }
            r.Close();

            donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
            returnList.Ekle(donenNesne);

            r = null;
            indeks = 0;
            try
            {
                r = tab.IlKarsilastirmaOzetToplamHarcamaBazlıSiralama(kriter, true, true);
            }
            catch
            {
            }

            while (r.Read() && indeks < enYuksekIlSayısı)
            {
                donenNesne = new RPRIlEkonomikKodBazliMukayeseHRCTakipOzet();
                donenNesne.ilKodu = VeriErisim.DegerAl(r, 0, 0);
                donenNesne.ilAdi = VeriErisim.DegerAl(r, 1, "");
                donenNesne.toplamHarcTutari = (decimal)VeriErisim.DegerAl(r, 2, 0.0);
                donenNesne.personelSayi = VeriErisim.DegerAl(r, 3, 0);
                donenNesne.kisiBasiOrt = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                donenNesne.toplamIcindekiPayi = (100 * donenNesne.toplamHarcTutari) / genelToplam;
                returnList.Ekle(donenNesne);
                indeks++;
            }
            r.Close();
            ve.VeriErisimKapat();
            return returnList;
        }
        */
    }

}

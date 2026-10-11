using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Hesap cetveli bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCHesapCetveli
    {
        /// <summary>
        /// Arguman olarak verilen, hesap cetveli bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="gelenHesapCetveli">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, HesapCetveli gelenHesapCetveli, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(true, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            //if (!HRCYetkiKontrol.Yoneticimi(kullanan))
            //{
            //    sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            string hata = "";
            //Alan kontrolleri yapilacak
            hata = Kontrol(gelenHesapCetveli);
            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////

            bool tamamlandi = true;
            int yil = gelenHesapCetveli.yil;
            string surecNo = gelenHesapCetveli.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            FaturaForm faturaBaslik = gelenHesapCetveli.faturaBaslik;
            ///////////////////////////////////////////////////////////////////////////
            //1.ADIM
            //Hesap Cetvelinde surecte pRFaturaNo kayıtlı ise alınır
            //Gelen detaySiraNo aslinda sadece hesap Pusulası için kullanılıyor ve faturada en son kaydedilen
            //hesap pusulasi sira numarası hesapcetveli tablosunda kayıtlı olmaktadır
            HesapCetveli kayitEdilecekHesapCetveli = new HesapCetveli();
            HesapCetveli kayitliHesapCetveli = HesapCetveliBilgisiAl(kullanan, gelenHesapCetveli, ve);
            faturaBaslik.prFaturaNo = kayitliHesapCetveli.pRFaturaNo;
            faturaBaslik.durum = (int)ENUMAktiflik.AKTIF;
            if (gelenHesapCetveli.hPDetaySiraNo == 0)
                kayitEdilecekHesapCetveli.hPDetaySiraNo = kayitliHesapCetveli.hPDetaySiraNo;
            else
                kayitEdilecekHesapCetveli.hPDetaySiraNo = gelenHesapCetveli.hPDetaySiraNo;

            //2.ADIM
            //Eğer HesapCetvelinden gelen pRFaturaNo bilgisi ile Fatura kayıt edilir

            faturaBaslik.detayTip = gelenHesapCetveli.faturaDetayTip;
            if (faturaBaslik.detayTip == (int)ENUMFaturaDetayTip.HESAPCETVELI)
                kayitEdilecekHesapCetveli.hPDetaySiraNo = gelenHesapCetveli.hCetvelSatirSayi;
            else if (faturaBaslik.detayTip == (int)ENUMFaturaDetayTip.HARCAMAPUSULASI)
            {
                //if (faturaBaslik.detaySiraNo == 0)
                if (gelenHesapCetveli.hPDetaySiraNo == 0)
                {
                    //harcama pusulası için detaySiraNo her kayıtta 1 arttırılır
                    //Çünkü her defasında 1 kayıt yapılır
                    kayitEdilecekHesapCetveli.hPDetaySiraNo += 1;
                    faturaBaslik.detaySiraNo = kayitEdilecekHesapCetveli.hPDetaySiraNo;
                    //faturaBaslik.detaySiraNo = kayitliHesapCetveli.hPDetaySiraNo;
                }
            }

            if (sonuc.islemSonuc)
            {
                HRCFaturaForm faturaServis = new HRCFaturaForm();
                sonuc = faturaServis.Kaydet(kullanan, faturaBaslik, islemKod, parcaKod);
            }
            //3.ADIM
            //Eğer pRFaturaNo HesapCetvelinde Kaydedilmemis ise Sonuc bilgisi ile donen prFaturaNo yu
            //Hesap Cetveli tablosuna yazmalıyız

            if (sonuc.islemSonuc)
            {
                TABHesapCetveli to = new TABHesapCetveli(ve);
                kayitEdilecekHesapCetveli.pRFaturaNo = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0);
                kayitEdilecekHesapCetveli.yil = gelenHesapCetveli.yil;
                kayitEdilecekHesapCetveli.muhasebe = gelenHesapCetveli.muhasebe;
                kayitEdilecekHesapCetveli.birim = gelenHesapCetveli.birim;
                kayitEdilecekHesapCetveli.surecNo = gelenHesapCetveli.surecNo;

                sonuc = to.Kaydet(kayitEdilecekHesapCetveli);
            }

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

        private HesapCetveli HesapCetveliBilgisiAl(Kullanici kullanan, HesapCetveli kriter, VeriErisim ve)
        {
            HesapCetveli bilgi = new HesapCetveli();

            TABHesapCetveli to = new TABHesapCetveli(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                if (r.Read())
                {

                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.pRFaturaNo = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.hPDetaySiraNo = VeriErisim.DegerAl(r, 5, 0);
                }
            }

            r.Close();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, hesap cetveli bilgisini veritabanından okur 
        /// ve HesapCetveli nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, hesap cetveli bilgisini HesapCetveli nesnesi olarak tutan nesne</returns>
        public HesapCetveli Listele(Kullanici kullanan, HesapCetveli kriter)
        {
            HesapCetveli bilgi = new HesapCetveli();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ///////////////////////////////////////////////////////////////////////////
            //1.ADIM
            //Hesap Cetvelinde surecte pRFaturaNo kayıtlı ise alınır
            HesapCetveli kayitliHesapCetveli = HesapCetveliBilgisiAl(kullanan, kriter, ve);
            int kayitliPrFaturaNo = kayitliHesapCetveli.pRFaturaNo;

            if (kayitliPrFaturaNo != 0)
            {
                FaturaForm faturaKriter = new FaturaForm();
                faturaKriter.yil = kriter.yil;
                faturaKriter.muhasebe = kriter.muhasebe;
                faturaKriter.birim = kriter.birim;
                faturaKriter.surecNo = kriter.surecNo;
                faturaKriter.prFaturaNo = kayitliPrFaturaNo;
                faturaKriter.detayTip = kriter.faturaDetayTip;
                faturaKriter.detaySiraNo = kriter.faturaBaslik.detaySiraNo;
                faturaKriter.durum = (int)ENUMAktiflik.AKTIF;

                HRCFaturaForm faturaServis = new HRCFaturaForm();
                ObjectArray faturaList = faturaServis.Listele(kullanan, faturaKriter, false);
                if (faturaList.ObjeSayisi > 0)
                    bilgi.faturaBaslik = (FaturaForm)faturaList[0];
            }

            ve.VeriErisimKapat();

            return bilgi;
        }

        private string Kontrol(HesapCetveli gelenHesapCetveli)
        {
            string hata = "";

            foreach (FaturaDetay detay in gelenHesapCetveli.faturaBaslik.detay.objeler)
            {
                if (detay.adi.Trim() == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00133 + "<br>";
                if (detay.miktar == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00134 + "<br>";
                if (detay.birimFiyat == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00135 + "<br>";
                if (detay.belgeSahibi.Trim() == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00136 + "<br>";
            }

            return hata;
        }
    }
}


using System;
using System.Data;
using TNS.KYM;
using TNS.TMM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// Taşınır demirbaşlarının RFID bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class TMMSicilRFID
    {
        /// <summary>
        /// Verilen demirbaş sicil ve RFID eşleşme bilgilerini teker teker varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen eşleşmelerden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="rfIdler">Demirbaş sicil ve RFID eşleşme bilgileri listesini tutan nesne</param>
        /// <param name="webServis">Eşleşme bilgileri web servisten mi geliyor yoksa başka bir arayüzden mi bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, TNSCollection rfIdler, bool webServis)
        {
            Sonuc sonuc = new Sonuc();
            if (rfIdler.Count <= 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TasinirMalResI.TMM220;
                return sonuc;
            }

            SicilRFID srf = (SicilRFID)rfIdler[0];
            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, srf.muhasebeKod, srf.harcamaKod, srf.ambarKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            if (webServis)
            {
                foreach (SicilRFID rfId in rfIdler)
                {
                    if (rfId.prSicilNo <= 0 || rfId.rfIdNo <= 0)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = TasinirMalResI.TMM221;
                        return sonuc;
                    }
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilRFID tabRFID = new TABSicilRFID(ve);
            ve.BeginTransaction();

            try
            {
                foreach (SicilRFID rfId in rfIdler)
                {
                    if (!webServis && rfId.rfIdNo > 0 && !tabRFID.RfIdBostaMi(rfId.rfIdNo))
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = string.Format(TasinirMalResI.TMM222, rfId.rfIdNo);
                        break;
                    }

                    sonuc = tabRFID.Guncelle(rfId, webServis);
                    if (!sonuc.islemSonuc)
                    {
                        if (tabRFID.PrSicilNoKullanilmisMi(rfId.prSicilNo))
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = string.Format(TasinirMalResI.TMM223, rfId.prSicilNo);
                            break;
                        }

                        if (string.IsNullOrEmpty(ve.hata))
                            sonuc = tabRFID.Kaydet(rfId);

                        if (!sonuc.islemSonuc)
                        {
                            sonuc.islemSonuc = true;//TİF Belge onayı verildiğinde unique index hatası veriyor
                            sonuc.hataStr = "";
                            //break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Demirbaş sicil ve RFID eşleşme bilgilerini teker teker silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="rfIdler">Demirbaş sicil ve RFID eşleşme bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, TNSCollection rfIdler)
        {
            Sonuc sonuc = new Sonuc();

            SicilRFID srf = (SicilRFID)rfIdler[0];
            sonuc.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, srf.muhasebeKod, srf.harcamaKod, srf.ambarKod);
            if (!sonuc.islemSonuc)
            {
                sonuc.hataStr = TasinirMalResI.TMM050;
                return sonuc;
            }

            foreach (SicilRFID rfId in rfIdler)
            {
                if (rfId.prSicilNo <= 0 && rfId.rfIdNo <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TasinirMalResI.TMM224;
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilRFID tabRFID = new TABSicilRFID(ve);
            ve.BeginTransaction();

            try
            {
                foreach (SicilRFID rfId in rfIdler)
                {
                    sonuc = tabRFID.Sil(rfId);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.Message;
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş sicil ve RFID eşleşme bilgilerini döndüren yordam
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Listeleme kriterlerini tutan nesne</param>
        /// <param name="rfIdOlanlar">RFID eşleşmesi olanlar dahil olsun mu bilgisi</param>
        /// <param name="rfIdOlmayanlar">RFID eşleşmesi olmayanlar dahil olsun mu bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public static ObjectArray SicilListele(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo, bool rfIdOlanlar, bool rfIdOlmayanlar, int ilkKayit, int kayitSay)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSicilRFID tabSicilRFID = new TABSicilRFID(ve);

            ObjectArray bilgi = new ObjectArray();
            bilgi.sonuc.kayitSay = tabSicilRFID.SicilListeleKayitSay(sicilNo, rfIdOlanlar, rfIdOlmayanlar);

            if (ilkKayit < 0)
                ilkKayit = 0;
            if (kayitSay < 0)
                kayitSay = 0;

            int sayac = -1;
            System.Collections.Hashtable isimler = new System.Collections.Hashtable();
            IDataReader dr = tabSicilRFID.SicilListele(sicilNo, rfIdOlanlar, rfIdOlmayanlar);
            while (dr.Read())
            {
                sayac++;
                if (sayac < ilkKayit)
                    continue;
                if (sayac != 0 && kayitSay != 0 && sayac == ilkKayit + kayitSay)
                    break;

                SicilNoHareket sicil = new SicilNoHareket();
                sicil.prSicilNo = VeriErisim.DegerAl(dr, 0, 0);
                sicil.siraNo = VeriErisim.DegerAl(dr, 1, 0);
                sicil.yil = VeriErisim.DegerAl(dr, 2, 0);
                sicil.muhasebeKod = VeriErisim.DegerAl(dr, 3, string.Empty);
                sicil.harcamaBirimKod = VeriErisim.DegerAl(dr, 4, string.Empty);
                sicil.harcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), sicil.harcamaBirimKod);
                sicil.fisNo = VeriErisim.DegerAl(dr, 5, string.Empty);
                sicil.fisSiraNo = VeriErisim.DegerAl(dr, 6, 0);
                sicil.sicilNo = VeriErisim.DegerAl(dr, 7, string.Empty);
                sicil.sicilSiraNo = VeriErisim.DegerAl(dr, 8, string.Empty);
                sicil.hesapPlanKod = VeriErisim.DegerAl(dr, 9, string.Empty);
                sicil.hesapPlanKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(true), sicil.hesapPlanKod);
                sicil.ambarKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                sicil.fiyat = VeriErisim.DegerAl(dr, 11, (decimal)0);
                sicil.kdvOran = VeriErisim.DegerAl(dr, 12, 0);
                sicil.islemTipKod = VeriErisim.DegerAl(dr, 13, 0);
                sicil.islemTurKod = VeriErisim.DegerAl(dr, 14, 0);
                sicil.islemTarih = VeriErisim.DegerAl(dr, 15);
                sicil.islemYapan = VeriErisim.DegerAl(dr, 16, string.Empty);
                sicil.durum = VeriErisim.DegerAl(dr, 17, 0);
                sicil.fisTarih = VeriErisim.DegerAl(dr, 18);
                sicil.neredenGeldi = VeriErisim.DegerAl(dr, 19, string.Empty);

                sicil.nereyeGitti = VeriErisim.DegerAl(dr, 20, string.Empty);
                sicil.odaKod = sicil.nereyeGitti;
                if (!string.IsNullOrEmpty(sicil.odaKod))
                    sicil.odaAd = Genel.AdGetir(isimler, sicil.muhasebeKod + "-" + sicil.harcamaBirimKod + "-" + sicil.odaKod, "TASODA");

                sicil.kimeGitti = VeriErisim.DegerAl(dr, 21, string.Empty);
                sicil.personelKod = sicil.kimeGitti;
                if (!string.IsNullOrEmpty(sicil.personelKod))
                    sicil.kisiAd = Genel.AdGetir(isimler, sicil.personelKod, "TASPERSONEL");

                sicil.zimmetOzellik = VeriErisim.DegerAl(dr, 22, string.Empty);
                sicil.kdvliBirimFiyat = VeriErisim.DegerAl(dr, 23, (decimal)0);
                sicil.degerArtisi = VeriErisim.DegerAl(dr, 24, (decimal)0);
                sicil.gonMuhasebeKod = VeriErisim.DegerAl(dr, 25, string.Empty);
                sicil.gonHarcamaBirimKod = VeriErisim.DegerAl(dr, 26, string.Empty);
                sicil.gonHarcamaBirimKod = OrtakFonksiyonlar.KodNoktaKoy(Genel.KodYapiDondur(false), sicil.gonHarcamaBirimKod);
                sicil.gonAmbarKod = VeriErisim.DegerAl(dr, 27, string.Empty);
                sicil.muhasebeAd = VeriErisim.DegerAl(dr, 28, string.Empty);
                sicil.harcamaBirimAd = VeriErisim.DegerAl(dr, 29, string.Empty);
                sicil.ambarAd = VeriErisim.DegerAl(dr, 30, string.Empty);
                sicil.hesapPlanAd = VeriErisim.DegerAl(dr, 31, string.Empty);
                sicil.islemTipiAd = VeriErisim.DegerAl(dr, 32, string.Empty);
                sicil.rfIdNo = VeriErisim.DegerAl(dr, 33, 0);
                sicil.killNo = VeriErisim.DegerAl(dr, 34, 0);
                sicil.accessNo = VeriErisim.DegerAl(dr, 35, 0);
                sicil.eSicilNo = VeriErisim.DegerAl(dr, 36, string.Empty);
                sicil.tagType = VeriErisim.DegerAl(dr, 37, 0);

                ObjectArray ozellikler = TMMSicilNoOzellik.Listele(ve2, kullanan, sicil.prSicilNo);
                if (ozellikler.sonuc.islemSonuc && ozellikler.objeler.Count > 0)
                {
                    SicilNoOzellik ozellik = (SicilNoOzellik)ozellikler.objeler[0];
                    sicil.ozellik.markaAd = ozellik.markaAd;
                    sicil.ozellik.modelAd = ozellik.modelAd;
                    sicil.ozellik.saseNo = ozellik.saseNo;
                }

                bilgi.Ekle(sicil);
            }
            dr.Close();

            if (bilgi.objeler.Count == 0)
                bilgi.sonuc.bilgiStr = TasinirMalResI.TMM029;

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return bilgi;
        }

        //public static int SicilListeleKayitSay(VeriErisim veriErisim, Kullanici kullanan, SicilNoHareket sicilNo, bool rfIdOlanlar, bool rfIdOlmayanlar)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(kullanan));
        //    int kayitSay = new TABSicilRFID(ve).SicilListeleKayitSay(sicilNo, rfIdOlanlar, rfIdOlmayanlar);
        //    ve.VeriErisimKapat();
        //    return kayitSay;
        //}
    }
}
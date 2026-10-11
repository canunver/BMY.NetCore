using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// İhale için firma tarafından teklif edilen fiyat bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifEdilenFiyat
    {
        /// <summary>
        /// Arguman olarak verilen, ihale için firma tarafından teklif edilen fiyat bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifEdilenFiyat nesne koleksiyonu olarak tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (liste.objeler.Count == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                TNSDateTime bugunTarihi = new TNSDateTime(DateTime.Now.ToString());
                TeklifEdilenFiyat teklif = (TeklifEdilenFiyat)liste.objeler[0];
                if (teklif.tarih.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00149 + "<br>";
                if ((teklif.tarih.Yil > bugunTarihi.Yil) || ((teklif.tarih.Yil >= bugunTarihi.Yil) && (teklif.tarih.YilinGunu > bugunTarihi.YilinGunu)))
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00194;
                if (teklif.saat == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00150 + "<br>";
                try
                {
                    string gunSaat = teklif.saat;
                    int koordinat = gunSaat.IndexOf(":");
                    int saat = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(0, koordinat), 0);
                    int dakika = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(koordinat + 1, gunSaat.Length - koordinat - 1), 0);
                    /*  if ((teklif.tarih.Yil == bugunTarihi.Yil) && (bugunTarihi.YilinGunu == teklif.tarih.YilinGunu))
                      {
                          if (saat > bugunTarihi.Saat)
                              hata += "Belge Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                          else if ((saat == bugunTarihi.Saat) && (dakika > bugunTarihi.Dakika))
                              hata += "Belge Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                      }*/
                }
                catch
                {
                    hata = TNS.HRC.HarcamaSurecleriResI.HRC00183 + "<br>";
                }
                if (teklif.yil == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00172 + "<br>";
                if (teklif.muhasebe == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00173 + "<br>";
                if (teklif.birim.Trim() == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00174 + "<br>";
                if (teklif.surecNo == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00175 + "<br>";

                if (hata != "")
                {
                    foreach (TeklifEdilenFiyat k in liste.objeler)
                    {
                        if (k.firmaAd.Trim() == "")
                            hata += TNS.HRC.HarcamaSurecleriResI.HRC00201 + "<br>";
                        if (k.firmaKod == 0)
                            hata += TNS.HRC.HarcamaSurecleriResI.HRC00202 + "<br>";
                        if (k.teklifTutari == 0.0)
                            hata += TNS.HRC.HarcamaSurecleriResI.HRC00277 + "<br>";
                    }
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
            TeklifEdilenFiyat eksikBelge = ((TeklifEdilenFiyat)liste.objeler[0]);
            int yil = eksikBelge.yil;
            string surecNo = eksikBelge.surecNo;

            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifEdilenFiyat to = new TABTeklifEdilenFiyat(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(eksikBelge);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

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

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale için firma tarafından teklif edilen fiyat bilgisini veritabanından okur 
        /// ve TeklifEdilenFiyat nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale için firma tarafından teklif edilen fiyat bilgisi listesini TeklifEdilenFiyat nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TeklifEdilenFiyat kriter)
        {
            ObjectArray donusDegeri = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifEdilenFiyat to = new TABTeklifEdilenFiyat(ve);
            IDataReader r = to.Listele(kriter);
            TeklifEdilenFiyat ekleObj;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ekleObj = new TeklifEdilenFiyat();
                    ekleObj.yil = VeriErisim.DegerAl(r, 0, 0);
                    ekleObj.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    ekleObj.birim = VeriErisim.DegerAl(r, 2, "");
                    ekleObj.surecNo = VeriErisim.DegerAl(r, 3, "");
                    ekleObj.tarih = VeriErisim.DegerAl(r, 4);
                    ekleObj.saat = VeriErisim.DegerAl(r, 5, "");
                    ekleObj.teklifTutari = VeriErisim.DegerAl(r, 6, 0.0);
                    ekleObj.fiyatDisiTeklifTutari = VeriErisim.DegerAl(r, 7, 0.0);
                    ekleObj.yerliMaliTeklifTutari = VeriErisim.DegerAl(r, 8, 0.0);
                    ekleObj.firmaKod = VeriErisim.DegerAl(r, 9, 0);
                    ekleObj.teklifTur = VeriErisim.DegerAl(r, 10, 0);
                    ekleObj.firmaAd = VeriErisim.DegerAl(r, 11, "");
                    donusDegeri.objeler.Add(ekleObj);
                }
            }
            r.Close();

            TABTeklifEdilenFiyat to2 = new TABTeklifEdilenFiyat(ve);
            IDataReader r1 = to2.ListeleFiyatTeklifEtmeyenler(kriter);

            if (to2.Hata() == "")
            {
                while (r1.Read())
                {

                    ekleObj = new TeklifEdilenFiyat();
                    ekleObj.yil = kriter.yil;
                    ekleObj.muhasebe = kriter.muhasebe;
                    ekleObj.birim = kriter.birim;
                    ekleObj.surecNo = kriter.surecNo;

                    ekleObj.firmaKod = VeriErisim.DegerAl(r1, 0, 0);
                    ekleObj.firmaAd = VeriErisim.DegerAl(r1, 1, "");

                    donusDegeri.objeler.Add(ekleObj);
                }

            }

            r1.Close();

            ve.VeriErisimKapat();


            return donusDegeri;
        }
    }

}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// İhale kararı bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleKarari
    {
        /// <summary>
        /// Arguman olarak verilen, ihale kararı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IhaleKarari bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan, bilgi.muhasebe, bilgi.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            TNSDateTime bugunTarihi = new TNSDateTime(DateTime.Now.ToString());

            string hata = "";
            if (bilgi.tarih.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00193 + "<br>";
            if ((bilgi.tarih.Yil > bugunTarihi.Yil) || ((bilgi.tarih.Yil >= bugunTarihi.Yil) && (bilgi.tarih.YilinGunu > bugunTarihi.YilinGunu)))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00194;
            if (bilgi.saat.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00195 + "<br>";
            try
            {
                string gunSaat = bilgi.saat;
                int koordinat = gunSaat.IndexOf(":");
                int saat = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(0, koordinat), 0);
                int dakika = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(koordinat + 1, gunSaat.Length - koordinat - 1), 0);
                /* if ((bilgi.tarih.Yil == bugunTarihi.Yil) && (bugunTarihi.YilinGunu == bilgi.tarih.YilinGunu))
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
            TABIhaleKarari to = new TABIhaleKarari(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale kararı bilgisini veritabanından okur 
        /// ve IhaleKarari nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale kararı bilgisini IhaleKarari nesnesi olarak tutan nesne</returns>
        public IhaleKarari Listele(Kullanici kullanan, IhaleKarari kriter)
        {
            IhaleKarari donusDegeri = new IhaleKarari();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleKarari to = new TABIhaleKarari(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                if (r.Read())
                {

                    donusDegeri.yil = VeriErisim.DegerAl(r, 0, 0);
                    donusDegeri.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    donusDegeri.birim = VeriErisim.DegerAl(r, 2, "");
                    donusDegeri.surecNo = VeriErisim.DegerAl(r, 3, "");
                    donusDegeri.tarih = VeriErisim.DegerAl(r, 4);
                    donusDegeri.saat = VeriErisim.DegerAl(r, 5, "");
                    donusDegeri.gerekce = VeriErisim.DegerAl(r, 6, "");

                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return donusDegeri;
        }
    }

}

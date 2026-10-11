using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// İhale sonucu bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleSonucu
    {
        /// <summary>
        /// Arguman olarak verilen, ihale sonucu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IhaleSonucu bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            if (bilgi.tarih.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00193 + "<br>";
            if (bilgi.saat.Trim() == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00195 + "<br>";
            if (bilgi.sonuc == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00198 + "<br>";
            if ((bilgi.sonuc == (int)ENUMIhaleSonucu.IPTALEDILDI) && (bilgi.gerekce == ""))
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00199 + "<br>";


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
            TABIhaleSonucu to = new TABIhaleSonucu(ve);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale sonucu bilgisini veritabanından okur 
        /// ve IhaleSonucu nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale sonucu bilgisini IhaleSonucu nesnesi olarak tutan nesne</returns>
        public IhaleSonucu Listele(Kullanici kullanan, IhaleSonucu kriter)
        {
            IhaleSonucu donusDegeri = new IhaleSonucu();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleSonucu to = new TABIhaleSonucu(ve);
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
                    donusDegeri.sonuc = VeriErisim.DegerAl(r, 6, 0);
                    donusDegeri.gerekce = VeriErisim.DegerAl(r, 7, "");


                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return donusDegeri;
        }
    }

}

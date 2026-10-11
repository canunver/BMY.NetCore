using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// İhaleyi kazanan firma bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCKazananFirma
    {
        /// <summary>
        /// Arguman olarak verilen, ihaleyi kazanan firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, KazananFirma bilgi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

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
            TABKazananFirma to = new TABKazananFirma(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(bilgi);
            if (sonuc.islemSonuc)
            {

                TeklifEdilenFiyat firma = new TeklifEdilenFiyat();
                firma.muhasebe = bilgi.muhasebe;
                firma.birim = bilgi.birim;
                firma.yil = bilgi.yil;
                firma.surecNo = bilgi.surecNo;
                firma.firmaKod = bilgi.firmaKod1;

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
        /// Arguman olarak verilen, ihaleyi kazanan firma bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Güncellenecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Guncelle(Kullanici kullanan, KazananFirma bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");


            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABKazananFirma to = new TABKazananFirma(ve);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihaleyi kazanan firma bilgisini veritabanından okur 
        /// ve KazananFirma nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihaleyi kazanan firma bilgisini KazananFirma nesnesi olarak tutan nesne</returns>
        public KazananFirma Listele(Kullanici kullanan, KazananFirma kriter)
        {
            KazananFirma donusDegeri = new KazananFirma();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABKazananFirma to = new TABKazananFirma(ve);
            IDataReader r = to.Listele(kriter);

            if (r != null)
            {
                if (r.Read())
                {
                    donusDegeri.yil = VeriErisim.DegerAl(r, 0, 0);
                    donusDegeri.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    donusDegeri.birim = VeriErisim.DegerAl(r, 2, "");
                    donusDegeri.surecNo = VeriErisim.DegerAl(r, 3, "");
                    donusDegeri.firmaKod1 = VeriErisim.DegerAl(r, 4, 0);
                    donusDegeri.firmaKod2 = VeriErisim.DegerAl(r, 5, 0);
                    donusDegeri.firmaAd1 = VeriErisim.DegerAl(r, 6, "");
                    donusDegeri.firmaAd2 = VeriErisim.DegerAl(r, 7, "");
                    donusDegeri.firmaVergiNo1 = VeriErisim.DegerAl(r, 8, "");
                    donusDegeri.firmaVergiNo2 = VeriErisim.DegerAl(r, 9, "");
                }
                r.Close();
            }
            ve.VeriErisimKapat();
            return donusDegeri;
        }
    }
}

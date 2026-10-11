using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale firma sonucu bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIhaleFirmaSonucu
    {
        /// <summary>
        /// Arguman olarak verilen, ihale firma sonucu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</param>
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
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00066 + "<br>";
            else
            {
                IhaleFirmaSonucu red = (IhaleFirmaSonucu)liste.objeler[0];
                
                if (red.yil == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00172+"<br>";
                if (red.muhasebe == 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00173+"<br>";
                if (red.birim.Trim() == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00174+"<br>";
                if (red.surecNo == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00175+"<br>";
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
            IhaleFirmaSonucu firmaRed = ((IhaleFirmaSonucu)liste.objeler[0]);


            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, firmaRed.yil, firmaRed.surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleFirmaSonucu to = new TABIhaleFirmaSonucu(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(firmaRed);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

            if (sonuc.islemSonuc)
            {
                //Parca Adım devam ediyor olarak işaretle.
                ///////////////////////////////////////////////////////////////////////////
                new HRCSurec().ParcaDurumKaydet(null, kullanan, firmaRed.yil, firmaRed.surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
                ///////////////////////////////////////////////////////////////////////////
                sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00011+"<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale firma sonucu bilgisini veritabanından okur 
        /// ve IhaleFirmaSonucu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale firma sonucu listesini IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IhaleFirmaSonucu kriter)
        {
            ObjectArray donusDegeri = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleFirmaSonucu to = new TABIhaleFirmaSonucu(ve);
            IDataReader r = to.ListeleTeklifVermisOlanlar(kriter);
            IhaleFirmaSonucu ekleObj;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ekleObj = new IhaleFirmaSonucu();
                    ekleObj.yil = VeriErisim.DegerAl(r, 0, 0);
                    ekleObj.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    ekleObj.birim = VeriErisim.DegerAl(r, 2, "");
                    ekleObj.surecNo = VeriErisim.DegerAl(r, 3, "");
                    ekleObj.firmaKod = VeriErisim.DegerAl(r, 4, 0);
                    ekleObj.onayTarihi = VeriErisim.DegerAl(r, 5);
                    ekleObj.gidisTarihi = VeriErisim.DegerAl(r, 6);
                    ekleObj.gidisSekli = VeriErisim.DegerAl(r, 7, 0);
                    ekleObj.gerekce = VeriErisim.DegerAl(r, 8, "");
                    ekleObj.firmaAd = VeriErisim.DegerAl(r, 9, "");

                    donusDegeri.objeler.Add(ekleObj);
                }
            }
            r.Close();

            TABIhaleFirmaSonucu to2 = new TABIhaleFirmaSonucu(ve);
            IDataReader r1 = to2.ListeleSonraTeklifVerenler(kriter);

            if (to2.Hata() == "")
            {
                while (r1.Read())
                {

                    ekleObj = new IhaleFirmaSonucu();
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

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, kısmi teklif verilebilen ihale için ihale firma sonucu bilgisini veritabanından okur 
        /// ve IhaleFirmaSonucu nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, kısmi teklif verilebilen ihale için ihale firma sonucu bilgisi listesini IhaleFirmaSonucu nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleKismiTeklif(Kullanici kullanan, IhaleFirmaSonucu kriter)
        {
            ObjectArray donusDegeri = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleFirmaSonucu to = new TABIhaleFirmaSonucu(ve);
            IDataReader r = to.Listele(kriter);
            IhaleFirmaSonucu ekleObj;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ekleObj = new IhaleFirmaSonucu();
                    ekleObj.yil = VeriErisim.DegerAl(r, 0, 0);
                    ekleObj.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    ekleObj.birim = VeriErisim.DegerAl(r, 2, "");
                    ekleObj.surecNo = VeriErisim.DegerAl(r, 3, "");
                    ekleObj.firmaKod = VeriErisim.DegerAl(r, 4, 0);
                    ekleObj.onayTarihi = VeriErisim.DegerAl(r, 5);
                    ekleObj.gidisTarihi = VeriErisim.DegerAl(r, 6);
                    ekleObj.gidisSekli = VeriErisim.DegerAl(r, 7, 0);
                    ekleObj.gerekce = VeriErisim.DegerAl(r, 8, "");
                    ekleObj.firmaAd = VeriErisim.DegerAl(r, 9, "");

                    donusDegeri.objeler.Add(ekleObj);
                }
            }
            r.Close();
            ve.VeriErisimKapat();

            return donusDegeri;
        }
    }

}

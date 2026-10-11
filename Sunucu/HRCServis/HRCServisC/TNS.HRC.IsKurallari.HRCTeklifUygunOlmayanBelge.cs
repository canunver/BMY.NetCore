using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{

    /// <summary>
    /// Teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifUygunOlmayanBelge
    {
        /// <summary>
        /// Arguman olarak verilen, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifUygunOlmayanBelge nesne koleksiyonu olarak tutan nesne</param>
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
                if (((TeklifUygunOlmayanBelge)liste.objeler[0]).tarih.isNull)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00149 + "<br>";
                if ((((TeklifUygunOlmayanBelge)liste.objeler[0]).tarih.Yil > bugunTarihi.Yil) || (((TeklifUygunOlmayanBelge)liste.objeler[0]).tarih.YilinGunu > bugunTarihi.YilinGunu))
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00276;
                if (((TeklifUygunOlmayanBelge)liste.objeler[0]).saat == "")
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00150 + "<br>";
                try
                {
                    string gunSaat = ((TeklifUygunOlmayanBelge)liste.objeler[0]).saat;
                    int koordinat = gunSaat.IndexOf(":");
                    int saat = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(0, koordinat), 0);
                    int dakika = OrtakFonksiyonlar.ConvertToInt(gunSaat.Substring(koordinat + 1, gunSaat.Length - koordinat - 1), 0);
                    /*   if ((((TeklifUygunOlmayanBelge)liste.objeler[0]).tarih.Yil == bugunTarihi.Yil) && (bugunTarihi.YilinGunu == ((TeklifUygunOlmayanBelge)liste.objeler[0]).tarih.YilinGunu))
                       {
                           if (saat > 24)
                               hata += "Belge Saat bilgisi 24'den büyük girilmiştir.<br>";
                           else if (saat > bugunTarihi.Saat)
                               hata += "Belge Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                           else if (dakika > 60)
                               hata += "Belge Saat bilgisi 59'dan büyük girilmiştir.<br>";
                           else if ((saat == bugunTarihi.Saat) && (dakika > bugunTarihi.Dakika))
                               hata += "Belge Saat bilgisi anlık saatten ileri bir saat girilmiştir.<br>";
                       }*/
                }
                catch
                {
                    hata = TNS.HRC.HarcamaSurecleriResI.HRC00183 + "<br>";
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
            TeklifUygunOlmayanBelge eksikBelge = ((TeklifUygunOlmayanBelge)liste.objeler[0]);
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
            TABTeklifUygunOlmayanBelge to = new TABTeklifUygunOlmayanBelge(ve);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisini veritabanından okur 
        /// ve TeklifUygunOlmayanBelge nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, teklif veren firmanın teklif zarfındaki uygun olmayan belge bilgisi listesini TeklifUygunOlmayanBelge nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TeklifUygunOlmayanBelge kriter)
        {
            ObjectArray donusDegeri = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifUygunOlmayanBelge to = new TABTeklifUygunOlmayanBelge(ve);
            IDataReader r = to.Listele(kriter);
            TeklifUygunOlmayanBelge ekleObj;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    ekleObj = new TeklifUygunOlmayanBelge();
                    ekleObj.yil = VeriErisim.DegerAl(r, 0, 0);
                    ekleObj.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    ekleObj.birim = VeriErisim.DegerAl(r, 2, "");
                    ekleObj.surecNo = VeriErisim.DegerAl(r, 3, "");
                    ekleObj.tarih = VeriErisim.DegerAl(r, 4);
                    ekleObj.saat = VeriErisim.DegerAl(r, 5, "");
                    ekleObj.gerekce = VeriErisim.DegerAl(r, 6, "");
                    ekleObj.firmaKod = VeriErisim.DegerAl(r, 7, 0);
                    ekleObj.belgeKod = VeriErisim.DegerAl(r, 8, 0);
                    ekleObj.belgeAd = VeriErisim.DegerAl(r, 9, "");
                    ekleObj.ihaleAsamasi = VeriErisim.DegerAl(r, 10, 0);
                    ekleObj.firmaAd = VeriErisim.DegerAl(r, 11, "");
                    donusDegeri.objeler.Add(ekleObj);
                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return donusDegeri;
        }
    }

}

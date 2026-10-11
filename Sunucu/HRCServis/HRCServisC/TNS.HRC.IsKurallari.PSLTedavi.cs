using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Tedavi bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class PSLTedavi
    {
        /// <summary>
        /// Arguman olarak verilen, tedavi bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tedavi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Tedavi tedavi, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            if (tedavi.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (tedavi.muhasebe == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00004 + "<br>";
            if (tedavi.birim == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00005 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTedavi to = new TABTedavi(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(tedavi);

            if (sonuc.islemSonuc)
            {
                sonuc = to.SilDetay(tedavi);

                int siraNo = 0;
                foreach (TedaviDetay detay in tedavi.detay)
                {
                    siraNo++;
                    detay.siraNo = siraNo;
                    sonuc = to.KaydetDetay(detay);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }


            //Süreç devam ediyor olarak işaretle.
            ///////////////////////////////////////////////////////////////////////////
            if (sonuc.hataStr == "")
                sonuc = new HRCSurec().SurecDurumKaydet(null, kullanan, tedavi.yil, tedavi.surecNo, ENUMSurecDurum.DEVAMEDIYOR);
            ///////////////////////////////////////////////////////////////////////////

            //Parca Adım devam ediyor olarak işaretle.
            /////////////////////////////////////////////////////////////////////////
            if (sonuc.islemSonuc)
                sonuc = new HRCSurec().ParcaDurumKaydet(null, kullanan, tedavi.yil, tedavi.surecNo, islemKod, parcaKod, ENUMSurecParcaDurum.DEVAMEDIYOR, kullanan.kullaniciKodu);
            /////////////////////////////////////////////////////////////////////////

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
        /// Arguman olarak verilen kriterlere bağlı olarak, tedavi bilgisini veritabanından okur 
        /// ve Tedavi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, tedavi bilgisini Tedavi nesnesi olarak tutan nesne</returns>
        public Tedavi Listele(Kullanici kullanan, Tedavi kriter)
        {
            Tedavi t = new Tedavi();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTedavi to = new TABTedavi(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    t.yil = VeriErisim.DegerAl(r, 0, 0);
                    t.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    t.birim = VeriErisim.DegerAl(r, 2, "");
                    t.surecNo = VeriErisim.DegerAl(r, 3, "");
                    t.tip = VeriErisim.DegerAl(r, 4, 0);
                    t.firma.kod = VeriErisim.DegerAl(r, 5, 0);
                    t.tertip = VeriErisim.DegerAl(r, 6, "");
                    t.odemeHesap = VeriErisim.DegerAl(r, 7, "");
                    t.odemeEmriNo = VeriErisim.DegerAl(r, 8, "");

                    t.firma.vergiNo = VeriErisim.DegerAl(r, 19, "");
                    t.firma.hesapNo = VeriErisim.DegerAl(r, 20, "");
                    t.firma.ad = VeriErisim.DegerAl(r, 21, "");
                    t.firma.vergiDairesi = VeriErisim.DegerAl(r, 22, "");
                    t.firma.banka = VeriErisim.DegerAl(r, 23, "");

                    TedaviDetay td = new TedaviDetay();
                    td.yil = VeriErisim.DegerAl(r, 9, 0);
                    td.muhasebe = VeriErisim.DegerAl(r, 10, 0);
                    td.birim = VeriErisim.DegerAl(r, 11, "");
                    td.surecNo = VeriErisim.DegerAl(r, 12, "");
                    td.tip = VeriErisim.DegerAl(r, 13, 0);
                    td.siraNo = VeriErisim.DegerAl(r, 14, 0);
                    td.faturaNo = VeriErisim.DegerAl(r, 15, "");
                    td.faturaTarih = VeriErisim.DegerAl(r, 16);
                    td.faturaTutar = VeriErisim.DegerAl(r, 17, 0.0);
                    td.katilimPayi = VeriErisim.DegerAl(r, 18, 0.0);

                    t.detay.Add(td);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return t;
        }

        /// <summary>
        /// Arguman olarak verilen, tedavi bilgisine ait ödeme emri belge numarası bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc OdemeEmriNoKaydet(Kullanici kullanan, Tedavi kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTedavi to = new TABTedavi(ve);
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
        /// Arguman olarak verilen kriterlere bağlı olarak, yıllık tedavi harcama ortalama bilgisini veritabanından okur 
        /// ve TedaviIstatistik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, yıllık tedavi harcama ortalama bilgisi listesini TedaviIstatistik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray ListeleYillikTedaviOrtalamasi(Kullanici kullanan, TedaviIstatistik kriter)
        {
            ObjectArray donusList = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTedavi to = new TABTedavi(ve);
            IDataReader r = to.ListeleYillikTedaviOrtalamasi(kriter);

            TedaviIstatistik t=null;
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    t = new TedaviIstatistik();

                    t.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                    t.birim = VeriErisim.DegerAl(r, 1, "");
                    t.tip = VeriErisim.DegerAl(r, 2, 0);
                    t.toplamHarcama = VeriErisim.DegerAl(r, 3, 0.0);
                    t.perSayi = VeriErisim.DegerAl(r, 4, 0);
                    t.ortHarcama = VeriErisim.DegerAl(r, 5, 0.0);

                    donusList.objeler.Add(t);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return donusList;
        }
    }
}

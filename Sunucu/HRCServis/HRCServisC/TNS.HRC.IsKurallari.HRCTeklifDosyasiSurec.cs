using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale teklif dosyası içerik bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifDosyasiIcerikListe
    {
        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası içerik bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
        /// <param name="islemKod">İşlem yapılacak sürece ait işlem kodu</param>
        /// <param name="parcaKod">İşlem yapılacak sürece ait parça kodu</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, TeklifDosyasiIcerikListe liste, int islemKod, int parcaKod)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan, liste.muhasebe, liste.birim))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = "";

            if (liste.yil == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00172 + "<br>";
            if (liste.muhasebe == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00173 + "<br>";
            if (liste.birim.Trim() == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00174 + "<br>";
            if (liste.surecNo == "")
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00175 + "<br>";

            if (liste.liste.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (TeklifDosyasiIcerik bilgi in liste.liste.objeler)
                {
                    if (bilgi.isTuru == 0)
                    {
                        hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00146 + "<br>";
                        break;
                    }
                    if (bilgi.ihaleUsul == 0)
                    {
                        hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00147 + "<br>";
                        break;
                    }
                    if (bilgi.adi == "")
                        hataStr += string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00148, bilgi.sira) + "<br>";
                }
            }


            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            //Parca Adım tamamlandımı kontrolü. Eğer tamamlandı ise kayıt edilmeyecek.
            ///////////////////////////////////////////////////////////////////////////

            bool tamamlandi = true;
            int yil = liste.yil;
            string surecNo = liste.surecNo;
            tamamlandi = new HRCSurec().ParcaTamamlandiMi(kullanan, yil, surecNo, parcaKod);
            if (tamamlandi)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00070;
                return sonuc;
            }
            ///////////////////////////////////////////////////////////////////////////

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifDosyasiIcerikListe to = new TABTeklifDosyasiIcerikListe(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(liste);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası içerik bilgisini veritabanından okur 
        /// ve TeklifDosyasiIcerikListe nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası içerik bilgisini TeklifDosyasiIcerikListe nesnesi olarak tutan nesne</returns>
        public TeklifDosyasiIcerikListe Listele(Kullanici kullanan, TeklifDosyasiIcerikListe kriter)
        {
            TeklifDosyasiIcerikListe bilgi = new TeklifDosyasiIcerikListe();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifDosyasiIcerikListe to = new TABTeklifDosyasiIcerikListe(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {

                    if (bilgi.yil == 0)
                    {
                        bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                        bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                        bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                        bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    }

                    TeklifDosyasiIcerik idi = new TeklifDosyasiIcerik();

                    idi.isTuru = VeriErisim.DegerAl(r, 4, 0);
                    idi.ihaleUsul = VeriErisim.DegerAl(r, 5, 0);
                    idi.sira = VeriErisim.DegerAl(r, 6, 0);
                    idi.adi = VeriErisim.DegerAl(r, 7, "");
                    idi.dosyaAdi = VeriErisim.DegerAl(r, 8, "");
                    idi.degKriteri = VeriErisim.DegerAl(r, 9, 0);
                    bilgi.liste.objeler.Add(idi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

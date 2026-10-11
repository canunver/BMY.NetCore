using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale teklif dosyası içerik tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifDosyasiIcerik
    {
        /// <summary>
        /// Arguman olarak verilen, ihale teklif dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hataStr = "";
            if (liste.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";
            else
            {
                foreach (TeklifDosyasiIcerik bilgi in liste.objeler)
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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifDosyasiIcerik to = new TABTeklifDosyasiIcerik(ve);
            ve.BeginTransaction();
            TeklifDosyasiIcerik iDI;
            iDI = (TeklifDosyasiIcerik)liste.objeler[0];
            sonuc = to.Sil(iDI);

            if (sonuc.islemSonuc)
                sonuc = to.Kaydet(liste);

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
        /// Arguman olarak verilen, ihale teklif dosyası içerik tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Güncelleme servisinde kullanılır.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc ListeKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            string hataStr = "";
            if (liste.objeler.Count == 0)
                hataStr += TNS.HRC.HarcamaSurecleriResI.HRC00007 + "<br>";

            if (hataStr != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hataStr;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifDosyasiIcerik to = new TABTeklifDosyasiIcerik(ve);
            ve.BeginTransaction();


            sonuc = to.Sil(new TeklifDosyasiIcerik());
            //silme işleminde hiçbir şey silinmez ise false geliyor
            sonuc = to.Kaydet(liste);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif dosyası içerik bilgisini veritabanından okur 
        /// ve TeklifDosyasiIcerik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif dosyası içerik bilgisi listesini TeklifDosyasiIcerik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TeklifDosyasiIcerik kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifDosyasiIcerik to = new TABTeklifDosyasiIcerik(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {

                    TeklifDosyasiIcerik bilgi = new TeklifDosyasiIcerik();
                    bilgi.isTuru = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ihaleUsul = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.sira = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.adi = VeriErisim.DegerAl(r, 3, "");
                    bilgi.dosyaAdi = VeriErisim.DegerAl(r, 4, "");
                    bilgi.degKriteri = VeriErisim.DegerAl(r, 5, 0);

                    liste.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }
    }
}

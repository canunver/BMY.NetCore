using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhale teklif zarfı biçim tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTeklifZarfBicimi
    {
        /// <summary>
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, TeklifZarfBicimi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.kod <= 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00211 + "<br>";

            if (bilgi.adi == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00212 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifZarfBicimi to = new TABTeklifZarfBicimi(ve);
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
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner.
        /// Güncelleme servisinde kullanılır.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri TeklifZarfBicimi nesne koleksiyonu olarak tutan nesne</param>
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
            TABTeklifZarfBicimi to = new TABTeklifZarfBicimi(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(new TeklifZarfBicimi());

            foreach (TeklifZarfBicimi var in liste.objeler)
            {

                sonuc = to.Kaydet(var);
                if (!sonuc.islemSonuc)
                    break;
            }

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
        /// Arguman olarak verilen, ihale teklif zarfı biçim tanım bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, TeklifZarfBicimi bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            if (bilgi.kod == 0)
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00211;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTeklifZarfBicimi to = new TABTeklifZarfBicimi(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00213, bilgi.kod) + "<br>";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00019 + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ihale teklif zarfı biçim tanım bilgisini veritabanından okur 
        /// ve TeklifZarfBicimi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale teklif zarfı biçim tanım bilgisi listesini TeklifZarfBicimi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TeklifZarfBicimi kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTeklifZarfBicimi to = new TABTeklifZarfBicimi(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TeklifZarfBicimi f = new TeklifZarfBicimi();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.adi = VeriErisim.DegerAl(r, 1, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

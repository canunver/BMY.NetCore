using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Sosyal yardım tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCSosYardTemelBilgisi
    {
        /// <summary>
        /// Arguman olarak verilen, sosyal yardım tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak tutan nesne</param>
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

            string hata = "";

            if (liste.objeler.Count == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABSosYardTemelBilgi to = new TABSosYardTemelBilgi(ve);
            ve.BeginTransaction();

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
        /// Arguman olarak verilen kriterlere bağlı olarak, sosyal yardım tanım bilgisini veritabanından okur 
        /// ve PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sosyal yardım tanım bilgisi listesini PSLSosyalYardimTemelBilgisi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TNS.HRC.PSLSosyalYardimTemelBilgisi kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABSosYardTemelBilgi to = new TABSosYardTemelBilgi(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TNS.HRC.PSLSosyalYardimTemelBilgisi f = new TNS.HRC.PSLSosyalYardimTemelBilgisi();

                    f.yil = VeriErisim.DegerAl(r, 0, 0);
                    f.ay = VeriErisim.DegerAl(r, 1, 0);
                    f.aileYardimGost = (decimal)VeriErisim.DegerAl(r, 2, 0.0);
                    f.cocukYardimGost0_6 = (decimal)VeriErisim.DegerAl(r, 3, 0.0);
                    f.cocukYardimGost6_X = (decimal)VeriErisim.DegerAl(r, 4, 0.0);
                    f.dogumYardimGost = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                    f.memurOlumKatsayi = (decimal)VeriErisim.DegerAl(r, 6, 0.0);
                    f.esCocukOlumKatsayi = (decimal)VeriErisim.DegerAl(r, 7, 0.0);


                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

    }

}

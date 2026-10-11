using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Ülke bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCUlke
    {
        /// <summary>
        /// Arguman olarak verilen, ulke bilgisi listesini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri Ulke nesne koleksiyonu olarak tutan nesne</param>
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

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABUlke to = new TABUlke(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(new Ulke());
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ülke bilgisini veritabanından okur 
        /// ve Ulke nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>İstenilen kriterdeki, ulke billgisi listesini Ulke nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUlke to = new TABUlke(ve);
            IDataReader r = to.Listele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Ulke f = new Ulke();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ad = VeriErisim.DegerAl(r, 1, "");
                    f.paraBirimiKod = VeriErisim.DegerAl(r, 2, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, ülke bilgisini veritabanından okur 
        /// ve Ulke nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ulkeKod">İşlem yapılacak ulke kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, ulke bilgisini Ulke nesnesi olarak tutan nesne</returns>
        public Ulke Listele(Kullanici kullanan, string ulkeKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABUlke to = new TABUlke(ve);
            IDataReader r = to.Listele();

            Ulke f = new Ulke();
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ad = VeriErisim.DegerAl(r, 1, "");
                    f.paraBirimiKod = VeriErisim.DegerAl(r, 2, 0);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return f;
        }
    }
}

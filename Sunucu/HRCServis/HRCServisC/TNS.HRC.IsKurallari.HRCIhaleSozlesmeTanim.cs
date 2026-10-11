using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhalede kullanılacak sözleşme metni bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// TanimIhaleSozlesme formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCIhaleSozlesmeTanim
    {
        /// <summary>
        /// İhalede kullanılacak sözleşme metnini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek sözleşme metnine ait bilgileri içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IhaleSozlesmeTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.ihaleUsulu == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00147 + "<br>";
            if (bilgi.ihaleMaddesi == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00197 + "<br>";
            if (bilgi.tur == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00200+"<br>";
            if (bilgi.isTur == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00146 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleSozlesmeTanim to = new TABIhaleSozlesmeTanim(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc && bilgi.metin.Length > 0)
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
        /// Arguman olarak verilen kriterlere bağlı olarak, ihalede kullanılacak sözleşme metnine ait bilgileri veritabanından okur 
        /// ve IhaleSozlesmeTanim nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren IhaleSozlesmeTanim nesnesi</param>
        /// <returns>İstenilen kriterdeki, ihalede kullanılacak sözleşme metnine ait bilgiyi IhaleSozlesmeTanim nesnesi olarak tutan nesne</returns>
        public IhaleSozlesmeTanim Listele(Kullanici kullanan, IhaleSozlesmeTanim bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleSozlesmeTanim to = new TABIhaleSozlesmeTanim(ve);
            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi.ihaleUsulu = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ihaleMaddesi = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.tur = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.isTur = VeriErisim.DegerAl(r, 3, 0);
                    bilgi.metin.Append(VeriErisim.DegerAl(r, 5, ""));
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

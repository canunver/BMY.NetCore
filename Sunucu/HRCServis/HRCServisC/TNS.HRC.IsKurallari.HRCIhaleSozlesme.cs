using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İhalede kullanılacak sözleşme metni işlemlerini yürüten fonksiyonları yöneten sınıf
    /// IHLSozlesme formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCIhaleSozlesme
    {
        /// <summary>
        /// Arguman olarak verilen, ihalede kullanılacak sözleşme metnine ait bilgileri veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner 
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek bilgileri tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IhaleSozlesme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.yil == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00003 + "<br>";
            if (bilgi.surecNo == "")
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00067 + "<br>";
            if (bilgi.tur == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00144 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIhaleSozlesme to = new TABIhaleSozlesme(ve);
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
        /// ve IhaleSozlesme nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, ihale sözleşme bilgisini IhaleSozlesme nesnesi olarak tutan nesne</returns>
        public IhaleSozlesme Listele(Kullanici kullanan, IhaleSozlesme bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIhaleSozlesme to = new TABIhaleSozlesme(ve);
            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi.yil = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.birim = VeriErisim.DegerAl(r, 2, "");
                    bilgi.surecNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.tur = VeriErisim.DegerAl(r, 4, 0);
                    bilgi.metin.Append(VeriErisim.DegerAl(r, 6, ""));
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İlişkilendirilmiş işlem ve parça bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// TanimIsParca formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCIsParca
    {
        /// <summary>
        /// İlişkilendirilmiş işlem ve parça bilgilerini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek işlem ve parça bilgilerini tutan IsParca nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, IsParca bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.isAd.kod == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00211 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsParca to = new TABIsParca(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(bilgi.isAd.kod);

            if (sonuc.islemSonuc)
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
        /// İlişkilendirilmiş işlem ve parça bilgilerini veritabanından okur 
        /// ve IsParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="isKod">Parça bilgileri listelenecek olan işlem kodu</param>
        /// <returns>Verilen işlem koduna ait parça bilgilerini tutan nesne</returns>
        public IsParca Listele(Kullanici kullanan, int isKod)
        {
            IsParca bilgi = new IsParca();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIsParca to = new TABIsParca(ve);
            IDataReader r = to.Listele(isKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi.isAd.kod = VeriErisim.DegerAl(r, 0, 0);

                    Parca pBilgi = new Parca();
                    pBilgi.kod = VeriErisim.DegerAl(r, 5, 0);
                    pBilgi.ad = VeriErisim.DegerAl(r, 6, "");
                    pBilgi.sayfa = VeriErisim.DegerAl(r, 7, "");
                    pBilgi.yukseklik = VeriErisim.DegerAl(r, 8, 0);
                    pBilgi.genislik = VeriErisim.DegerAl(r, 9, 0);
                    pBilgi.duzey = VeriErisim.DegerAl(r, 3, 0);
                    pBilgi.zorunlu = VeriErisim.DegerAl(r, 4, 0);

                    bilgi.parcalar.objeler.Add(pBilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// İlişkilendirilmiş ihale ve parça bilgilerini veritabanından okur 
        /// ve IsParca nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="isKod">İhaleye türüne göre parça bilgileri listelenecek olan işlem kodu</param>
        /// <returns>Verilen işlem koduna ait ihale parça bilgilerini tutan nesne</returns>
        public IsParca ListeleIsIhaleParca(Kullanici kullanan, int isKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIsParca to = new TABIsParca(ve);
            IDataReader r = to.ListeleIsIhaleParca(isKod);

            IsParca bilgi = new IsParca();
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi.isAd.kod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.isAd.ad = VeriErisim.DegerAl(r, 1, "");

                    Parca pBilgi = new Parca();
                    pBilgi.kod = VeriErisim.DegerAl(r, 2, 0);
                    pBilgi.ad = VeriErisim.DegerAl(r, 3, "");
                    pBilgi.duzey = VeriErisim.DegerAl(r, 4, 0);
                    pBilgi.zorunlu = VeriErisim.DegerAl(r, 5, 0);

                    bilgi.parcalar.objeler.Add(pBilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

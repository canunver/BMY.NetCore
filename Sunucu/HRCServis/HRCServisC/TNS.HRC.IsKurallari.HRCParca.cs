using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Harcama Yönetiminde kullanılan parça bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// TanimParca formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCParca
    {
        /// <summary>
        /// Parça bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek parça bilgisini içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Parca bilgi)
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

            if (bilgi.ad == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00255 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABParca to = new TABParca(ve);
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
        /// Harcama Yönetiminde kullanılan parça bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek parça bilgisini içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, Parca bilgi)
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
            TABParca to = new TABParca(ve);

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
        /// Parça bilgisini veritabanından okur 
        /// ve Parca nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="pbilgi">Listelemede kriter olarak kullanılacak değerleri içeren Parca nesnesi</param>
        /// <returns>İstenilen kriterdeki parça listesini Parca nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Parca pbilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABParca to = new TABParca(ve);
            IDataReader r = to.Listele(pbilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Parca bilgi = new Parca();

                    bilgi.kod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ad = VeriErisim.DegerAl(r, 1, "");
                    bilgi.sayfa = VeriErisim.DegerAl(r, 2, "");
                    bilgi.yukseklik = VeriErisim.DegerAl(r, 3, 0);
                    bilgi.genislik = VeriErisim.DegerAl(r, 4, 0);

                    iVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}

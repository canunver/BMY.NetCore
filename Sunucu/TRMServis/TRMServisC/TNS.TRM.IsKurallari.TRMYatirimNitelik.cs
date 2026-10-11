using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.TRM.IsKurallari
{
    public class TRMYatirimNitelik
    {
        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, YatirimNitelik bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = "TRM00013";
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            string hata = "";

            if (bilgi.ad == "")
                hata += "Ad bilgisi boş bırakılamaz" + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimNitelik to = new TABYatirimNitelik(ve);
            ve.BeginTransaction();

            if (bilgi.kod == 0)
                bilgi.kod = to.EnBuyukKod();

            sonuc = to.Kaydet(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu" + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen, malzeme grubu bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, YatirimNitelik bilgi)
        {
            Sonuc sonuc = new Sonuc();

            //if (!TRMYetkiKontrol.YoneticiMi(kullanan))
            //{
            //    sonuc.hataStr = TNS.TRM.HarcamaSurecleriResI.TRM00013;
            //    sonuc.islemSonuc = false;
            //    return sonuc;
            //}

            if (bilgi.kod == 0)
                sonuc.hataStr = "Kod bilgisi boş bırakılamaz" + "<br>";

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABYatirimNitelik to = new TABYatirimNitelik(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Bilgi silinirken bir hata oluştu" + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, malzeme grubu bilgisini veritabanından okur 
        /// ve YatirimNitelik nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, malzeme grubu listesini YatirimNitelik nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, YatirimNitelik kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);

            TABYatirimNitelik to = new TABYatirimNitelik(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    YatirimNitelik f = new YatirimNitelik();

                    f.kod = VeriErisim.DegerAl(r, 0, 0);
                    f.ad = VeriErisim.DegerAl(r, 1, string.Empty);
                    f.aktif = VeriErisim.DegerAl(r, 2, 0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

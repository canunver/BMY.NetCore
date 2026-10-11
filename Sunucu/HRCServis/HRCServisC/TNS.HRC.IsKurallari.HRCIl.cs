using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İl bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIl
    {
        /// <summary>
        /// Arguman olarak verilen, il bilgisi listesini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri Il nesne koleksiyonu olarak tutan nesne</param>
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
            TABIl to = new TABIl(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(new Il());
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
        /// Arguman olarak verilen kriterlere bağlı olarak, il bilgisini veritabanından okur 
        /// ve Il nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <returns>İstenilen kriterdeki, il billgisi listesini Il nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIl to = new TABIl(ve);
            IDataReader r = to.Listele();

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Il f = new Il();

                    f.kod = VeriErisim.DegerAl(r, 0, "");
                    f.ad = VeriErisim.DegerAl(r, 1, "");

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, il bilgisini veritabanından okur 
        /// ve Il nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="ilKod">İşlem yapılacak il kod bilgisi.</param>
        /// <returns>İstenilen kriterdeki, il bilgisini Il nesnesi olarak tutan nesne</returns>
        public Il Listele(Kullanici kullanan, string ilKod)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIl to = new TABIl(ve);
            IDataReader r = to.Listele(ilKod);

            Il f = new Il();
            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    f.kod = VeriErisim.DegerAl(r, 0, "");
                    f.ad = VeriErisim.DegerAl(r, 1, "");
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return f;
        }
    }
}

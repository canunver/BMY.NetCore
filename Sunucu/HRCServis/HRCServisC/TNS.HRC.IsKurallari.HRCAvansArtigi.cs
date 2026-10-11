using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Avans artığı bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCAvansArtigi
    {
        /// <summary>
        /// Arguman olarak verilen, avans artığı bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="avansArtigi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, AvansArtigi avansArtigi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";
            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAvansArtigi to = new TABAvansArtigi(ve);
            ve.BeginTransaction();
            if (avansArtigi.avansArtigiMifYil == 0) avansArtigi.avansArtigiMifYil = avansArtigi.yil;
            sonuc = to.Kaydet(avansArtigi);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, avans artığı bilgisini veritabanından okur 
        /// ve AvansArtigi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, avans artığı listesini AvansArtigi nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, AvansArtigi kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAvansArtigi to = new TABAvansArtigi(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AvansArtigi artik = new AvansArtigi();

                    artik.yil = VeriErisim.DegerAl(r, 0, 0);
                    artik.surecNo = VeriErisim.DegerAl(r, 1, "");
                    artik.avansArtigiMifveYil = VeriErisim.DegerAl(r, 2, "");
                    bilgi.objeler.Add(artik);
                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, muhasebe işlem fişi belge bilgisini veritabanından okur 
        /// ve AvansArtigi nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="surecNo">The surec no.</param>
        /// <returns></returns>
        public ObjectArray ListeleMifNo(int yil, string surecNo)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", null);

            TABAvansArtigi to = new TABAvansArtigi(ve);
            IDataReader r = to.ListeleMifNo(yil, surecNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AvansArtigi artik = new AvansArtigi();

                    artik.yil = VeriErisim.DegerAl(r, 0, 0);
                    artik.avansArtigiMifveYil = VeriErisim.DegerAl(r, 1, "");

                    bilgi.objeler.Add(artik);
                }
            }
            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen, avans artığı bilgisini veritabanından siler
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Silinecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, AvansArtigi kriter)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABAvansArtigi tab = new TABAvansArtigi(ve);

            sonuc = tab.Sil(kriter);

            if (sonuc.islemSonuc)
            {
                HRCOdemeEmriForm hrcOdemeForm = new HRCOdemeEmriForm();
                OdemeEmriMIFForm mifForm = new OdemeEmriMIFForm();
                mifForm.surecNo = kriter.surecNo;
                mifForm.yil = kriter.yil;
                mifForm.odemeEmriYil = kriter.avansArtigiMifYil;
                mifForm.odemeEmriNo = kriter.avansArtigiMif;
                sonuc = hrcOdemeForm.MifIptal(kullanan, mifForm);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = TNS.HRC.HarcamaSurecleriResI.HRC00018;
                }
            }

            if (!sonuc.islemSonuc)
                ve.Rollback();

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

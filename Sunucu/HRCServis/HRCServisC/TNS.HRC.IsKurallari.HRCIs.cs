using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Harcama Yönetiminde kullanılan işlem bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// TanimIs formu ile ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// güncelleme işlemlerini gerçekleştiren fonksiyonları tutan sınıf.
    /// </summary>
    public class HRCIs
    {
        /// <summary>
        /// İşlem bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Kaydedilecek olan Is nesnesi</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Is bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.kod == 0)
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00211 + "<br>";

            if (bilgi.ad == string.Empty)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00212 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIs to = new TABIs(ve);
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
        /// İşlem bilgisini veritabanından siler 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Silinecek işlem bilgisini içeren nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Sil(Kullanici kullanan, Is bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (!HRCYetkiKontrol.YoneticiMi(kullanan))
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
            TABIs to = new TABIs(ve);

            ve.BeginTransaction();

            sonuc = to.Sil(bilgi);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = string.Format(TNS.HRC.HarcamaSurecleriResI.HRC00213, bilgi.kod) + " <br>";
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
        /// İşlem bilgisini veritabanından okur ve Is nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">Kullanici bilgilerini içeren nesne</param>
        /// <param name="pbilgi">Listelemede kriter olarak kullanılacak değerleri içeren Is nesnesi</param>
        /// <returns>İstenilen kriterdeki işlem listesini Is nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Is pbilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIs to = new TABIs(ve);

            string yetkiTipiDegeri = OrtakFonksiyonlar.YetkiTipiDegeriBul(kullanan.yetkiTipiDegerleri, "Kişisel İşler");
            if (!string.IsNullOrWhiteSpace(yetkiTipiDegeri))
            {
                if (yetkiTipiDegeri.EndsWith(";")) yetkiTipiDegeri = yetkiTipiDegeri.Substring(0, yetkiTipiDegeri.Length - 1);
            }

            IDataReader r = to.Listele(pbilgi, yetkiTipiDegeri);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Is bilgi = new Is();

                    bilgi.kod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ad = VeriErisim.DegerAl(r, 1, "");
                    bilgi.ekoKod = VeriErisim.DegerAl(r, 2, "");
                    bilgi.odemeHesapKod = VeriErisim.DegerAl(r, 3, "");
                    bilgi.yansitmaKod = VeriErisim.DegerAl(r, 4, "");
                    bilgi.damgaVergisi = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.tur = VeriErisim.DegerAl(r, 6, 0);

                    iVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }

        internal int HizmetAlimTurBul(int isKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABIs t = new TABIs(ve);
            int tut = t.HizmetAlimTurBul(isKod);

            ve.VeriErisimKapat();
            return tut;
        }

        internal string IsEkoKodBul(int isKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(null));
            TABIs t = new TABIs(ve);
            string tut = t.EkoKodBul(isKod);

            ve.VeriErisimKapat();
            return tut;
        }
    }
}

using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Avans tutarı hesaplamasında kullanılan avans hesap bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCAvansHesap
    {
        /// <summary>
        /// Arguman olarak verilen, avans tutarı hesaplamasında kullanılan avans hesap bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgiyi tutan nesne</param>
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
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00056;
            else
            {
                double gundelik = ((AvansHesap)liste.objeler[0]).gundelikTutar;
                if (gundelik <= 0)
                    hata += TNS.HRC.HarcamaSurecleriResI.HRC00057;
            }

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABAvansHesap to = new TABAvansHesap(ve);
            ve.BeginTransaction();

            sonuc = to.Sil((AvansHesap)liste.objeler[0]);

            if (sonuc.islemSonuc)
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
        /// Arguman olarak verilen kriterlere bağlı olarak, avans tutarı hesaplamasında kullanılan avans hesap bilgisini veritabanından okur 
        /// ve AvansHesap nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, avans tutarı hesaplamasında kullanılan avans hesap bilgisi listesini AvansHesap nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, AvansHesap kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABAvansHesap to = new TABAvansHesap(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    AvansHesap avans = new AvansHesap();

                    avans.yil = VeriErisim.DegerAl(r, 0, 0);
                    avans.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    avans.birim = VeriErisim.DegerAl(r, 2, "");
                    avans.surecNo = VeriErisim.DegerAl(r, 3, "");
                    avans.siraNo = VeriErisim.DegerAl(r, 4, 0);
                    avans.gundelikTutar = VeriErisim.DegerAl(r, 5, 0.0);
                    avans.gundelikParaBirimi.kod = VeriErisim.DegerAl(r, 6, 0);
                    avans.gunSayisi = VeriErisim.DegerAl(r, 7, 0);
                    avans.arttirimliGunSayisi = VeriErisim.DegerAl(r, 8, 0);
                    avans.tur = VeriErisim.DegerAl(r, 9, 0);
                    avans.tutar = VeriErisim.DegerAl(r, 10, 0);
                    avans.paraBirimi.kod = VeriErisim.DegerAl(r, 11, 0);
                    avans.aciklama = VeriErisim.DegerAl(r, 12, "");
                    avans.damgaVergisi = VeriErisim.DegerAl(r, 13, 0.0);
                    avans.tarihBaslangic = VeriErisim.DegerAl(r, 14);
                    avans.tarihBaslangicArttirimli = VeriErisim.DegerAl(r, 15);
                    avans.gundelikParaBirimi.adi = VeriErisim.DegerAl(r, 16, "");
                    avans.gundelikParaBirimi.kisaltma = VeriErisim.DegerAl(r, 17, "");
                    avans.paraBirimi.adi = VeriErisim.DegerAl(r, 18, "");
                    avans.paraBirimi.kisaltma = VeriErisim.DegerAl(r, 19, "");

                    bilgi.objeler.Add(avans);
                }

            }
            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Kurs the bilgisi al.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kur">The kur.</param>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        private decimal KurBilgisiAl(VeriErisim veriErisim, Kullanici kullanan, KurBilgisi kur, int kod)
        {
            if (kod == 1) //TL
                return 1;

            kur.paraBirimi.kod = kod;
            ObjectArray donusDeger = new HRCKurBilgisi().Listele(veriErisim, kullanan, kur);

            if (donusDeger.objeler.Count > 0)
                return ((KurBilgisi)donusDeger.objeler[0]).kur;
            else
                return 1;
        }
    }
}

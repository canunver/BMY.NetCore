using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Sayfa yardım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCYardim
    {
        /// <summary>
        /// Arguman olarak verilen, sayfa yardım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, Yardim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (bilgi.sayfa.Trim() == "")
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00282;

            if (bilgi.tur == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00283;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYardim to = new TABYardim(ve);
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
        /// Arguman olarak verilen, sayfa yardım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="list">Kaydedilecek bilgileri Yardim nesne koleksiyonu olarak tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc GuncellemeKaydet(Kullanici kullanan, ObjectArray list)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYardim to = new TABYardim(ve);
            ve.BeginTransaction();

            sonuc = to.Sil(new Yardim());//hepsini Siler

            if (sonuc.islemSonuc)
                sonuc = to.GuncellemeKaydet(list);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, sayfa yardım bilgisini veritabanından okur 
        /// ve Yardım nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sayfa yardım bilgisini Yardim nesnesi olarak tutan nesne</returns>
        public Yardim Goster(Kullanici kullanan, Yardim bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABYardim to = new TABYardim(ve);
            IDataReader r = to.Goster(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi.sayfa = VeriErisim.DegerAl(r, 0, "");
                    bilgi.tur = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.metin.Append(VeriErisim.DegerAl(r, 3, ""));
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }

        /// <summary>
        /// Arguman olarak verilen kriterlere bağlı olarak, sayfa yardım bilgisini veritabanından okur 
        /// ve Yardim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="bilgi">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, sayfa yardım listesini Yardim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, Yardim bilgi)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            ObjectArray donenListe = new ObjectArray();
            TABYardim to = new TABYardim(ve);
            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    Yardim yrdm = new Yardim();
                    yrdm.sayfa = VeriErisim.DegerAl(r, 0, "");
                    yrdm.tur = VeriErisim.DegerAl(r, 1, 0);
                    yrdm.sira = VeriErisim.DegerAl(r, 2, 0);
                    yrdm.metinParca = VeriErisim.DegerAl(r, 3, "");
                    donenListe.Ekle(yrdm);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return donenListe;
        }
    }
}

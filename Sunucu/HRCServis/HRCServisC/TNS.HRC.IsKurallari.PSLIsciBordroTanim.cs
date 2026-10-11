using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// İşçi maaş bordro hesaplamasında kullanılacak tanım bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCIsciMaasBordroTanim
    {
        /// <summary>
        /// Arguman olarak verilen, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="liste">Kaydedilecek bilgileri IsciBordroTanim nesne koleksiyonu olarak tutan nesne</param>
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
                hata = TNS.HRC.HarcamaSurecleriResI.HRC00007;

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsciBordroTanim to = new TABIsciBordroTanim(ve);
            ve.BeginTransaction();

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
        /// Arguman olarak verilen kriterlere bağlı olarak, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisini veritabanından okur 
        /// ve IsciBordroTanim nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, işçi maaş bordro hesaplamasında kullanılacak tanım bilgisi listesini IsciBordroTanim nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, IsciBordroTanim kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABIsciBordroTanim to = new TABIsciBordroTanim(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    IsciBordroTanim f = new IsciBordroTanim();

                    f.muhasebe = VeriErisim.DegerAl(r, 0, 0);
                    f.birim = VeriErisim.DegerAl(r, 1, "");
                    f.yil = VeriErisim.DegerAl(r, 2, 0);
                    f.ay = VeriErisim.DegerAl(r, 3, 0);
                    f.tcKimlikNo = VeriErisim.DegerAl(r, 4, "");
                    f.yevmiye = (decimal)VeriErisim.DegerAl(r, 5, 0.0);
                    f.asgariGecimIndirim = (decimal)VeriErisim.DegerAl(r, 6, 0.0);
                    f.ozelSigorta = (decimal)VeriErisim.DegerAl(r, 7, 0.0);
                    f.sakatlikIndirimi = (decimal)VeriErisim.DegerAl(r, 8, 0.0);
                    f.prim = (decimal)VeriErisim.DegerAl(r, 9, 0.0);
                    f.sosyalYardim = (decimal)VeriErisim.DegerAl(r, 10, 0.0);
                    f.tasitYardimi = (decimal)VeriErisim.DegerAl(r, 11, 0.0);
                    f.yemekYardimi = (decimal)VeriErisim.DegerAl(r, 12, 0.0);
                    f.sendikaAidati = (decimal)VeriErisim.DegerAl(r, 13, 0.0);
                    f.kumulatifVergiMatrahiBaslangic = (decimal)VeriErisim.DegerAl(r, 14, 0.0);

                    bilgi.objeler.Add(f);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return bilgi;
        }
    }
}

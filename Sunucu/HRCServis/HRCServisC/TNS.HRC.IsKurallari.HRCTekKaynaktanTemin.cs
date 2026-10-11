using System;
using System.Data;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    /// <summary>
    /// Tek kaynaktan temin bilgisi işlemlerini yürüten fonksiyonları yöneten sınıf
    /// </summary>
    public class HRCTekKaynaktanTemin
    {
        /// <summary>
        /// Arguman olarak verilen, tek kaynaktan temin bilgisini veritabanına kaydeder 
        /// ve işlem sonucunu Sonuc nesnesi olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Kaydedilecek bilgiyi tutan nesne</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        public Sonuc Kaydet(Kullanici kullanan, TekKaynaktanTemin kriter)
        {
            Sonuc sonuc = new Sonuc(false, TNS.HRC.HarcamaSurecleriResI.HRC00001, "");

            if (!HRCYetkiKontrol.YazmaYetkisiVarMi(kullanan))
            {
                sonuc.hataStr = TNS.HRC.HarcamaSurecleriResI.HRC00013;
                sonuc.islemSonuc = false;
                return sonuc;
            }

            string hata = "";

            if (kriter.firmaKod == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00201 + "<br>";
            if (kriter.teslimatZamani.isNull)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00274+"<br>";
            if (kriter.yaklasikMaliyet == 0)
                hata += TNS.HRC.HarcamaSurecleriResI.HRC00244+"<br>";
            

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));
            TABTekKaynaktanTemin to = new TABTekKaynaktanTemin(ve);
            ve.BeginTransaction();

            sonuc = to.Kaydet(kriter);

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
        /// Arguman olarak verilen kriterlere bağlı olarak, tek kaynaktan temin bilgisini veritabanından okur 
        /// ve TekKaynaktanTemin nesne koleksiyonu olarak döner
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="kriter">Listelemede kriter olarak kullanılacak değerleri içeren nesne</param>
        /// <returns>İstenilen kriterdeki, tek kaynaktan temin bilgi listesini TekKaynaktanTemin nesne koleksiyonu olarak tutan nesne</returns>
        public ObjectArray Listele(Kullanici kullanan, TekKaynaktanTemin kriter)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriHRC", TNS.KYM.Arac.KimlikBul(kullanan));

            TABTekKaynaktanTemin to = new TABTekKaynaktanTemin(ve);
            IDataReader r = to.Listele(kriter);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    TekKaynaktanTemin tkt = new TekKaynaktanTemin();

                    tkt.yil = VeriErisim.DegerAl(r, 0, 0);
                    tkt.muhasebe = VeriErisim.DegerAl(r, 1, 0);
                    tkt.birim = VeriErisim.DegerAl(r, 2, "");
                    tkt.surecNo = VeriErisim.DegerAl(r, 3, "");
                    tkt.firmaKod = VeriErisim.DegerAl(r, 4, 0);
                    tkt.yaklasikMaliyet = VeriErisim.DegerAl(r, 5, 0.0);
                    tkt.teslimatZamani = VeriErisim.DegerAl(r, 6);
                    tkt.ihtiyacKonusu = VeriErisim.DegerAl(r, 7, "");
                    tkt.gercekTuzel = VeriErisim.DegerAl(r, 8, "");
                    liste.objeler.Add(tkt);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return liste;
        }
    }
}

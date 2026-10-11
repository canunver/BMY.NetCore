using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Dağıtım bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRDagitim
    {
        /// <summary>
        /// Parametre olarak verilen bilgilere göre, evrak dağıtım bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Dağıtım bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Dagitim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDagitim tab = new TABDagitim(ve);

            ve.BeginTransaction();
            try
            {
                if (form.kod == 0) { sonuc = tab.Kaydet(form); } else { sonuc = tab.Guncelle(form); }

                if (sonuc.islemSonuc)
                {
                    foreach (DagitimAlan dAlan in form.alanlar)
                        dAlan.dagitimKodu = form.kod;
                    tab.SilAlanlar(form.kod);
                    sonuc = tab.KaydetAlanlar(form.alanlar);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr += EvrakI.EVRI023;
                    }
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI024 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{B5FCCC91-5E39-4AD0-BB72-3C52ACC35459} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, dağıtım bilgisini Dagitim nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kodList">Dağıtım kod listesini içeren nesne</param>
        /// <param name="evrakBrimKod">Evrak birim kodunu içeren nesne</param>
        /// <param name="alanlarYuklensin">Dağıtım alanlarının listelenme durumunu içeren nesne</param>
        /// <returns>Dagitim nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, string kodList, string evrakBrimKod, bool alanlarYuklensin)
        {
            Dagitim bilgi;
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDagitim to = new TABDagitim(ve);
            IDataReader r = to.Listele(kodList, evrakBrimKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new Dagitim();
                    bilgi.kod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.evrakBrimKodu = VeriErisim.DegerAl(r, 1, "");
                    bilgi.ad = VeriErisim.DegerAl(r, 2, "");
                    hVAN.objeler.Add(bilgi);
                }
                r.Close();

                if (alanlarYuklensin && hVAN.objeler.Count > 0)
                {
                    kodList = "";
                    foreach (Dagitim liste in hVAN.objeler)
                        kodList += liste.kod + ",";
                    kodList = kodList.Substring(0, kodList.Length - 1);

                    ObjectArray Liste2 = new ObjectArray();
                    DagitimAlan bilgi2;
                    r = to.ListeleAlanlar(kodList);

                    if (to.Hata() == "")
                    {
                        while (r.Read())
                        {
                            bilgi2 = new DagitimAlan();
                            bilgi2.dagitimKodu = VeriErisim.DegerAl(r, 0, 0);
                            bilgi2.siraNo = VeriErisim.DegerAl(r, 1, 0);
                            bilgi2.yer = VeriErisim.DegerAl(r, 2, "");
                            bilgi2.evrakBrimKodu = VeriErisim.DegerAl(r, 3, "");
                            bilgi2.arzRica = VeriErisim.DegerAl(r, 4, 0);
                            bilgi2.kurumBilgi = VeriErisim.DegerAl(r, 5, 0);
                            Liste2.objeler.Add(bilgi2);
                        }
                        r.Close();

                        foreach (Dagitim dagitim in hVAN.objeler)
                        {
                            foreach (DagitimAlan dAlan in Liste2.objeler)
                            {
                                if (dagitim.kod == dAlan.dagitimKodu)
                                {
                                    dagitim.alanlar.Add(dAlan);
                                }
                            }
                        }
                    }

                }

            }
            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, dağıtım bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="dagitimKod">Dağıtım kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int dagitimKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDagitim to = new TABDagitim(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = to.Sil(dagitimKod);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI025;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{D9D9B87E-E051-46EE-AF60-869FE9EC1C98} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

    }
}

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
    /// Paraf bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRParaf
    {
        /// <summary>
        /// Parametere olarak verilen bilgilere göre, paraf bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Evrak paraf biglisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Paraf bilgi)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABParaf to = new TABParaf(ve);

                if (bilgi.kod == 0)
                    bilgi.kod = KodAl(to);

                sonuc = to.Kaydet(bilgi);

                if (sonuc.islemSonuc)
                {
                    foreach (ParafDetay pDetay in bilgi.detaylar)
                        pDetay.parafKodu = bilgi.kod;
                    to.SilDetay(bilgi.kod);
                    sonuc = to.KaydetDetay(bilgi.detaylar);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr += EvrakI.EVRI111;
                    }
                }

                if (!sonuc.islemSonuc)
                {
                    sonuc.hataStr = EvrakI.EVRI112 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }


            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{CF2CD111-0C18-4D41-81BB-50D941AD0ABA} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametere olarak verilen bilgilere göre, evrak paraf bilgisini Paraf nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="kod">Paraf kod bilgisini içeren nesne</param>
        /// <param name="kullaniciKod">Kullanıcı kod bilgisini içeren nesne</param>
        /// <param name="detaylarYuklensin">Paraf detaylarının listelenme durumunu içeren nesne</param>
        /// <returns>Paraf nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, int kod, string kullaniciKod, bool detaylarYuklensin)
        {
            Paraf bilgi;
            ObjectArray hVAN = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABParaf to = new TABParaf(ve);
            IDataReader r = to.Listele(kod, kullaniciKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new Paraf();
                    bilgi.kod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.kullaniciKod = VeriErisim.DegerAl(r, 1, "");
                    bilgi.ad = VeriErisim.DegerAl(r, 2, "");
                    hVAN.objeler.Add(bilgi);
                }
                r.Close();

                if (detaylarYuklensin && hVAN.objeler.Count > 0)
                {
                    string kodList = "";
                    foreach (Paraf liste in hVAN.objeler)
                        kodList += liste.kod + ",";
                    kodList = kodList.Substring(0, kodList.Length - 1);

                    ObjectArray Liste2 = new ObjectArray();
                    ParafDetay bilgi2;
                    r = to.ListeleDetay(kodList);

                    if (to.Hata() == "")
                    {
                        while (r.Read())
                        {
                            bilgi2 = new ParafDetay();
                            bilgi2.parafKodu = VeriErisim.DegerAl(r, 0, 0);
                            bilgi2.siraNo = VeriErisim.DegerAl(r, 1, 0);
                            bilgi2.ad = VeriErisim.DegerAl(r, 2, "");
                            bilgi2.soyad = VeriErisim.DegerAl(r, 3, "");
                            bilgi2.unvan = VeriErisim.DegerAl(r, 4, "");
                            Liste2.objeler.Add(bilgi2);
                        }
                        r.Close();

                        foreach (Paraf paraf in hVAN.objeler)
                        {
                            foreach (ParafDetay pDetay in Liste2.objeler)
                            {
                                if (paraf.kod == pDetay.parafKodu)
                                {
                                    paraf.detaylar.Add(pDetay);
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
        /// Parametre olarak verilen bilgilere göre, evrak paraf bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="parafKod">Paraf kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int kullaniciKod)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI113, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABParaf to = new TABParaf(ve);

                sonuc = to.Sil(kullaniciKod);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI114;
                }
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{350F9651-E97F-40AA-99B4-C9924F58CCEA} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen veritabanı paraf nesnesine göre, veritabanından son paraf kodunun bir fazlasını getiren yordam.
        /// </summary>
        /// <param name="to">Veritabanı paraf bilgilerini içeren nesne</param>
        /// <returns>Paraf kodu int türünde döndürülür.</returns>
        private static int KodAl(TABParaf to)
        {
            int kod = 0;
            IDataReader r = to.KodAl();
            if (to.Hata() == "")
            {
                if (r.Read())
                {
                    kod = VeriErisim.DegerAl(r, 0, 0);
                }
                r.Close();
            }
            return kod;
        }
    }
}

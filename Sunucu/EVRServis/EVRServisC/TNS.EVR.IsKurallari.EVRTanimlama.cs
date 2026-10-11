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
    /// Tanımlama tür bilgisine göre, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRTamimlama
    {
        /// <summary>
        /// Parametre olarak verilen tanımlama tür bilisine göre, tamımlama bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Tanımlama nesne koleksiyonunu içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, ENUMTamimlamaFormTur tanimlamFormTur)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI153, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABTamimlama to = new TABTamimlama(ve, tanimlamFormTur.ToString());
                if (tanimlamFormTur == ENUMTamimlamaFormTur.EVRGIZLILIK)
                    sonuc = to.KaydetGizlilik(liste);
                else
                    sonuc = to.Kaydet(liste);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();

                    sonuc.bilgiStr += EvrakI.EVRI154;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{29CEB427-631A-41C6-9744-EFB57645C526} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, tanımlama bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Tanımlama nesne koleksiyonunu içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, ENUMTamimlamaFormTur tanimlamFormTur)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI155, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABTamimlama to = new TABTamimlama(ve, tanimlamFormTur.ToString());
                if (tanimlamFormTur == ENUMTamimlamaFormTur.EVRGIZLILIK)
                    sonuc = to.SilGizlilik(liste);
                else
                    sonuc = to.Sil(liste);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI156;
                }
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{A34A7977-5CDF-4867-9A26-9BCF697F1622} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, tanımlama bilgilerini Tanımlama nesne koleksiyonunu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">>Tanımlama bilgisini içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>Tanımlama nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Tanimlama bilgi, ENUMTamimlamaFormTur tanimlamFormTur)
        {
            ObjectArray hVAN = new ObjectArray();
            IDataReader r;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABTamimlama to = new TABTamimlama(ve, tanimlamFormTur.ToString());
            if (tanimlamFormTur == ENUMTamimlamaFormTur.EVRGIZLILIK)
                r = to.ListeleGizlilik(bilgi);
            else
                r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new Tanimlama();

                    bilgi.kod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.ad = VeriErisim.DegerAl(r, 1, "");
                    if (tanimlamFormTur == ENUMTamimlamaFormTur.EVRGIZLILIK)
                        bilgi.derece = VeriErisim.DegerAl(r, 2, 0);

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen tanımlama kodu ve türüne göre evrak tür adını veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kod">Evrak tür kodunu içeren nesne</param>
        /// <param name="tanimlamFormTur">Tanımlama tür bilgisini içeren nesne</param>
        /// <returns>String tipinde nesne döndürülür.</returns>
        public static string Listele(VeriErisim veriErisim, int kod, ENUMTamimlamaFormTur tanimlamFormTur)
        {
            string donenDeger = "";
            IDataReader r;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABTamimlama to = new TABTamimlama(ve, tanimlamFormTur.ToString());
            Tanimlama bilgi = new Tanimlama();
            bilgi.kod = kod;
            if (tanimlamFormTur == ENUMTamimlamaFormTur.EVRGIZLILIK)
                r = to.ListeleGizlilik(bilgi);
            else
                r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                if (r.Read())
                    donenDeger = VeriErisim.DegerAl(r, 1, "");
            }

            r.Close();

            ve.VeriErisimKapat();

            return donenDeger;
        }

    }
}

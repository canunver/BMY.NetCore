using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak ilgi bilgileriyle ilgili, kaydetme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVREvrakIlgi
    {
        /// <summary>
        /// Parametre olarak verilen evrak bilgisi ve EvrakIlgi nesne koleksiyonunu ilişkilendirilerek veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <param name="liste">EvrakIlgi nesne koleksiyonunu içreren nesne</param>
        /// <returns></returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo, ArrayList liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakIlgi to = new TABEvrakIlgi(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = to.Sil(prEvrakNo);
                if (sonuc.islemSonuc)
                {
                    int siraNo = 1;
                    foreach (EvrakIlgi form in liste)
                    {
                        form.prEvrakNo = prEvrakNo;
                        form.siraNo = siraNo++;

                        sonuc = to.Kaydet(form);
                        if (!sonuc.islemSonuc)
                            break;
                    }

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr += EvrakI.EVRI085;
                    }
                    else
                        ve.Rollback();
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{3864848F-2419-47E6-8ED6-3169789FC3F9} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen prEvrakNo bilgisine göre, evrak'ın ilgi bilgisini EvrakIlgi nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <returns>EvrakIlgi nesne koleksiyonunu olarak döndürülür.</returns>
        public static ArrayList Listele(VeriErisim veriErisim, string prEvrakNo)
        {
            ArrayList hVAN = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakIlgi to = new TABEvrakIlgi(ve);
            IDataReader r = to.Listele(prEvrakNo);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    EvrakIlgi bilgi = new EvrakIlgi();

                    bilgi.prEvrakNo = VeriErisim.DegerAl(r, 0, "");
                    bilgi.siraNo = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.ilgiTarihi = VeriErisim.DegerAl(r, 2);
                    bilgi.ilgiNo = VeriErisim.DegerAl(r, 3, "");
                    bilgi.aciklama = VeriErisim.DegerAl(r, 4, "");

                    hVAN.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

    }
}

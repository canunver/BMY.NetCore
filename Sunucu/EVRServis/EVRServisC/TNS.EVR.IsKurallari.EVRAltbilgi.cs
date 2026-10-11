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
    /// Alt bilgi bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRAltbilgi
    {
        /// <summary>
        /// Paramete olarak verilen Altbilgi nesne koleksiyonunu, kullanıcının evrak altbilgileri olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABAltbilgi to = new TABAltbilgi(ve);
            ve.BeginTransaction();
            try
            {
                sonuc = to.Kaydet(liste);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();

                    sonuc.bilgiStr += EvrakI.EVRI004;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{D8505A41-A770-4526-BE63-DFCA43D9E3BE} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Altbilgi nesne koleksiyonunu içeren Altbilgileri veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI005, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABAltbilgi to = new TABAltbilgi(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = to.Sil(liste);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI006;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{60B34419-C1C2-4E4C-A06B-0E315E173654} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen Altbilgi nesnesine göre, kullanıcının evrak altbilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>Altbilgi nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Altbilgi bilgi)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABAltbilgi to = new TABAltbilgi(ve);
            IDataReader r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new Altbilgi();

                    bilgi.kullaniciKod = VeriErisim.DegerAl(r, 0, "");
                    bilgi.altbilgi = VeriErisim.DegerAl(r, 1, "");
                    bilgi.kurumAdres = VeriErisim.DegerAl(r, 2, "");
                    bilgi.ayrintiliBilgi = VeriErisim.DegerAl(r, 3, "");
                    bilgi.telefon = VeriErisim.DegerAl(r, 4, "");
                    bilgi.faks = VeriErisim.DegerAl(r, 5, "");
                    bilgi.ePosta = VeriErisim.DegerAl(r, 6, "");
                    bilgi.elektronikAg = VeriErisim.DegerAl(r, 7, "");

                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }
    }
}

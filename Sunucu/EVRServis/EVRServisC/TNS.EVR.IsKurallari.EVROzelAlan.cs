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
    /// Özel Alan bilgileriyle ilgili işlemleri tutan sınıf.
    /// </summary>
    public class EVROzelAlan
    {
        /// <summary>
        /// OzelAlan bilgisini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">OzelAlan bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, OzelAlan bilgi)
        {
            Sonuc sonuc = new Sonuc();

            string hata = "";

            if (string.IsNullOrEmpty(bilgi.ad))
                hata += EvrakI.EVRI103 + "<br>";

            if (string.IsNullOrEmpty(bilgi.evrakBirim.kod))
                hata += EvrakI.EVRI104 + "<br>";

            if (hata != "")
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOzelAlan to = new TABOzelAlan(ve);
            TABGenel tabGenel = new TABGenel(ve);

            ve.BeginTransaction();
            try
            {
                if (bilgi.alanKod == 0)
                {
                    string sayacKod = "EVROZELALAN";
                    bilgi.alanKod = OrtakFonksiyonlar.ConvertToInt(tabGenel.BelgeNoAl(sayacKod, "000000000"), 0);
                }
                sonuc = to.Kaydet(bilgi);

                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = EvrakI.EVRI105;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = EvrakI.EVRI106 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{66223777-7595-401B-AC05-EE372325B442} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// OzelAlan bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="bilgi">OzelAlan bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, OzelAlan bilgi)
        {
            Sonuc sonuc = new Sonuc();

            if (bilgi.alanKod == 0)
                sonuc.hataStr = EvrakI.EVRI107;

            if (sonuc.hataStr != "")
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABOzelAlan to = new TABOzelAlan(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = to.Sil(bilgi);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = string.Format(EvrakI.EVRI108, bilgi.sira) + "<br>";
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI109 + "<br>" + sonuc.hataStr;
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{65AA9BB0-ADC0-4B47-B8B5-25C30F0543B2} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// OzelAlan bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="pbilgi">OzelAlan bilgisini içeren nesne</param>
        /// <returns>OzelAlan nesne koleksiyonunu olarak döndürülür.</returns>
        public ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, OzelAlan pbilgi)
        {
            ObjectArray iVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABOzelAlan to = new TABOzelAlan(ve);
            IDataReader r = to.Listele(pbilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    OzelAlan bilgi = new OzelAlan();

                    bilgi.alanKod = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.evrakBirim.kod = VeriErisim.DegerAl(r, 1, "");
                    bilgi.dosya.kod = VeriErisim.DegerAl(r, 2, "");
                    bilgi.sira = VeriErisim.DegerAl(r, 3, 0);
                    bilgi.ad = VeriErisim.DegerAl(r, 4, "");
                    bilgi.dosya.ad = VeriErisim.DegerAl(r, 5, "");
                    iVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return iVAN;
        }
    }
}

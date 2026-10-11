using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Evrak Özel Alan bilgileriyle ilgili işlemleri tutan sınıf.
    /// </summary>
    public class EVREvrakOzelAlan
    {
        /// <summary>
        /// Kullanıcının verilen nesneyi veri tabanına yazma yetkisinin var olup olmadığına bakan fonksiyon
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Kullanıcının kaydetmeye çalıştığı verileri tutan nesne</param>
        /// </summary>
        /// <returns>Yazma yetkisi varsa true, yoksa false döndürür.</returns>
        public static bool YazmaYetkisiVarMi(Kullanici kullanan, EvrakOzelAlan form)
        {
            EvrakSinif evrakForm = new EvrakSinif();
            evrakForm.prEvrakNo = form.prEvrakNo;
            ObjectArray liste = TNS.EVR.IsKurallari.EVREvrak.Listele(null, kullanan, evrakForm);
            foreach (EvrakSinif ef in liste.objeler)
            {
                ef.bulunduguYer = form.kullaniciAktifBirimi;
                return TNS.EVR.Yetki.YazmaYetkisiVarMi(kullanan, ef);
            }

            return false;
        }

        /// <summary>
        /// Parametre olarak verilen EvrakOzelAlan nesne koleksiyonunu veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">EvrakOzelAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ArrayList liste)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            foreach (EvrakOzelAlan form in liste)
            {
                if (!YazmaYetkisiVarMi(kullanan, form))
                {
                    sonuc.DegerAta("Yazma yetkiniz yok!");
                    return sonuc;
                }
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakOzelAlan to = new TABEvrakOzelAlan(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = to.Kaydet(liste);

                if (sonuc.islemSonuc)
                    ve.Commit();
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{63949FB7-A4DF-497E-B562-736766BF3F35} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// EvrakOzelAlan bilgilerini veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        /// <param name="birimKod">Birim kod bilgisini içeren nesne </param>
        /// <param name="dosyaKodlari">EvrakOzelAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>EvrakOzelAlan nesne koleksiyonunu olarak döndürülür.</returns>
        public static ArrayList Listele(VeriErisim veriErisim, string prEvrakNo, string birimKod, ArrayList dosyaKodlari)
        {
            ArrayList degerler = new ArrayList();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakOzelAlan to = new TABEvrakOzelAlan(ve);
            IDataReader r = to.Listele(prEvrakNo, birimKod);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    EvrakOzelAlan bilgi = new EvrakOzelAlan();
                    bool var = false;

                    bilgi.prEvrakNo = VeriErisim.DegerAl(r, 0, "");
                    bilgi.deger = VeriErisim.DegerAl(r, 1, "");
                    bilgi.ozelAlan.alanKod = VeriErisim.DegerAl(r, 2, 0);
                    bilgi.ozelAlan.evrakBirim.kod = VeriErisim.DegerAl(r, 3, "");
                    bilgi.ozelAlan.dosya.kod = VeriErisim.DegerAl(r, 4, "");
                    bilgi.ozelAlan.sira = VeriErisim.DegerAl(r, 5, 0);
                    bilgi.ozelAlan.ad = VeriErisim.DegerAl(r, 6, "");

                    for (int i = 0; i < degerler.Count; i++)
                    {
                        EvrakOzelAlan o = new EvrakOzelAlan();
                        o = (EvrakOzelAlan)degerler[i];

                        if (o.ozelAlan.alanKod == bilgi.ozelAlan.alanKod && o.prEvrakNo == "")
                        {
                            degerler[i] = bilgi;
                            var = true;
                            break;
                        }
                    }

                    if (!var)
                        degerler.Add(bilgi);
                }
                r.Close();
            }


            ve.VeriErisimKapat();

            return degerler;
        }
    }
}

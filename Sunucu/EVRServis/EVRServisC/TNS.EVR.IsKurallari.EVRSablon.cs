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
    /// Şablon bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRSablon : EVRSablonDagitim
    {
        /// <summary>
        /// Paramete olarak verilen Sablon nesnesini, kullanıcı bilgileri ile birlikte evrak şablonunu olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Şablon bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Sablon form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI146, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABSablon to = new TABSablon(ve);

                sonuc = to.Kaydet(form);

                if (sonuc.islemSonuc && form.dagitim.Count > 0)
                    sonuc = EVRSablonDagitim.Kaydet(ve, kullanan, form.kod, form.dagitim);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr += EvrakI.EVRI147;
                }
                else
                    ve.Rollback();

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{743B15A3-A25C-4D82-B368-A6D878E58236} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen şablon bilgisine göre, evrak'ın şablon bilgisini Sablon nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Şablon bilgilerini içeren nesne</param>
        /// <returns>Sablon nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Sablon form)
        {
            ObjectArray hVAN = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABSablon to = new TABSablon(ve);
            IDataReader r = to.Listele(form);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    form = new Sablon();

                    form.kod = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.AD, "");
                    form.kullaniciKodu = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.KULLANICIKODU, "");
                    form.evrakBrimKodu = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.EVRAKBIRIMKODU, "");
                    form.kayitTarihi = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.KAYITTARIHI);
                    form.evrakTuru = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.EVRAKTURU, 0);
                    form.gizlilik = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.GIZLILIK, 0);
                    form.antet = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.ANTET, "");
                    form.yaziDurum = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.YAZIDURUM, 0);
                    form.dosyaPlanKodu = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.DOSYAPLANKODU, "");
                    form.konu = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.KONU, "");
                    form.gidecegiYer = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.GIDECEGIYER, "");
                    form.icerik = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.ICERIK, "");
                    form.ek = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.EK, "");
                    //form.dagitimGeregi = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.DAGITIMGEREGI, "");
                    //form.dagitimBilgi = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.DAGITIMBILGI, "");
                    form.paraf = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.PARAF, "");
                    form.altBilgi = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.ALTBILGI, "");
                    form.detay = VeriErisim.DegerAl(r, (int)TABSablon.ENUMAlanlar.DETAY, 0);

                    form.dagitim = EVRSablonDagitim.Listele(null, kullanan, form.kod);
                    hVAN.objeler.Add(form);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, şablon bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="sablonKod">The sablon kod.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int sablonKod)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI148, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABSablon to = new TABSablon(ve);

                sonuc = EVRSablonDagitim.Sil(ve, kullanan, sablonKod);

                if (sonuc.islemSonuc)
                    sonuc = to.Sil(sablonKod);

                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI149;
                }
                else
                    ve.Rollback();
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{AC5EBE15-FBAD-4C42-A3FF-468F1AE710E8} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sablon[] Listele(VeriErisim veriErisim, Kullanici kullanan, Sablon form, bool detay = false)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABSablon tab = new TABSablon(ve);

            IDataReader dr = tab.Listele(form);
            if (dr != null)
            {
                while (dr.Read())
                {
                    form = new Sablon();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABSablon.ENUMAlanlar.KOD, 0);
                    form.ad = VeriErisim.DegerAl(dr, (int)TABSablon.ENUMAlanlar.AD, "");
                    form.detay = VeriErisim.DegerAl(dr, (int)TABSablon.ENUMAlanlar.DETAY, 0);
                    liste.objeler.Add(form);
                }

                dr.Close();
            }

            ve.VeriErisimKapat();

            return (Sablon[])liste.objeler.ToArray(typeof(Sablon));
        }

        public static Sablon Oku(VeriErisim veriErisim, Kullanici kullanan, Sablon form)
        {
            if (form.kod == 0) return null;

            Sablon[] list = EVRSablon.Listele(veriErisim, kullanan, form, true);
            if (list != null && list.Length == 1)
                form = list[0];

            return form;
        }
    }
}

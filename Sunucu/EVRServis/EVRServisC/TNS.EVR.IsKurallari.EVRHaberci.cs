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
    /// Haberci bilgileriyle ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf. 
    /// </summary>
    public class EVRHaberci
    {
        /// <summary>
        /// Paramete olarak verilen Haberci nesnesini, evrak habercisi olarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Haberci bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Haberci form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI087, "");
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABHaberci to = new TABHaberci(ve);

                sonuc = to.Kaydet(form);
                if (sonuc.islemSonuc)
                {
                    foreach (HaberciDetay detay in form.detaylar)
                        detay.haberciKodu = form.kod;

                    sonuc = to.SilDetay(form.kod);
                    if (sonuc.islemSonuc)
                        sonuc = to.KaydetDetay(form.detaylar);
                }

                if (sonuc.islemSonuc)
                {
                    ve.Commit(); sonuc.bilgiStr += EvrakI.EVRI088;
                }
                else
                {
                    ve.Rollback(); sonuc.hataStr = EvrakI.EVRI089 + "<br>" + sonuc.hataStr;
                }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{A6E37763-6AEC-49AC-9422-AAF22F110A97} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen bilgisine göre, evrak haberci listesini Haberci nesne koleksiyonu olarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="kodList">Haberci kod listesini içeren nesne</param>
        /// <param name="evrakBirimKodu">Evrak birim kodunu içeren nesne</param>
        /// <param name="detaylariGetir">Detay bilgilerinin getirilmesini belirten nesne</param>
        /// <returns>Haberci nesne koleksiyonunu olarak döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, string tcKimlikNo, string kodList, string evrakBirimKodu, bool detaylariGetir)
        {
            ObjectArray hVAN = new ObjectArray();
            Haberci form = new Haberci();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABHaberci to = new TABHaberci(ve);
            IDataReader r = to.Listele(kodList, tcKimlikNo, evrakBirimKodu);

            if (to.Hata() == "")
            {
                int eskiKod = 0;
                int index = ((int)TABHaberci.ENUMAlanlar.BITISTARIH) + 1;
                while (r.Read())
                {
                    int kod = VeriErisim.DegerAl(r, (int)TABHaberci.ENUMAlanlar.KOD, 0);

                    if (eskiKod != kod)
                    {
                        if (eskiKod > 0)
                            hVAN.objeler.Add(form);

                        form = new Haberci();
                        form.kod = kod;
                        form.kullanici = VeriErisim.DegerAl(r, (int)TABHaberci.ENUMAlanlar.KULLANICI, "");
                        form.evrakBrimKodu = VeriErisim.DegerAl(r, (int)TABHaberci.ENUMAlanlar.EVRAKBIRIMKODU, "");
                        form.ad = VeriErisim.DegerAl(r, (int)TABHaberci.ENUMAlanlar.AD, "");
                        form.baslangicTarih = VeriErisim.DegerAl(r, (int)TABHaberci.ENUMAlanlar.BASLANGICTARIH);
                        form.bitisTarih = VeriErisim.DegerAl(r, (int)TABHaberci.ENUMAlanlar.BITISTARIH);
                    }

                    HaberciDetay bilgi2 = new HaberciDetay();
                    bilgi2.haberciKodu = VeriErisim.DegerAl(r, index + (int)TABHaberci.ENUMAlanlarDetay.HABERCIKODU, 0);
                    bilgi2.kod = VeriErisim.DegerAl(r, index + (int)TABHaberci.ENUMAlanlarDetay.KOD, 0);
                    bilgi2.alan = VeriErisim.DegerAl(r, index + (int)TABHaberci.ENUMAlanlarDetay.ALAN, 0);
                    bilgi2.islec = VeriErisim.DegerAl(r, index + (int)TABHaberci.ENUMAlanlarDetay.ISLEC, 0);
                    bilgi2.deger = VeriErisim.DegerAl(r, index + (int)TABHaberci.ENUMAlanlarDetay.DEGER, "");
                    form.detaylar.Add(bilgi2);

                    eskiKod = form.kod;
                }
                r.Close();

                if (eskiKod > 0)
                    hVAN.objeler.Add(form);
            }
            ve.VeriErisimKapat();

            return hVAN;
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre, haberci bilgisini veritabanından silen yordam.
        /// </summary>
        /// <param name="haberciKod">Haberci kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Sil(VeriErisim veriErisim, int haberciKod)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI090, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABHaberci to = new TABHaberci(ve);

                sonuc = to.Sil(haberciKod);

                if (sonuc.islemSonuc)
                {
                    ve.Commit(); sonuc.bilgiStr += EvrakI.EVRI091;
                }
                else { ve.Rollback(); sonuc.hataStr = EvrakI.EVRI092 + "<br>" + sonuc.hataStr; }

            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{F19B0E40-9E6C-4784-B6A5-5759254020A1} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

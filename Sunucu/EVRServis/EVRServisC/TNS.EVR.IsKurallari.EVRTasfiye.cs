
using TNS.Veri;
using TNS.KYM;
using TNS.EVR.IsKurallari.Veri;
using EVRServisI;
using System.Data;
using System;
namespace TNS.EVR.IsKurallari
{
    /// <summary>
    /// Tasfiye bilgileriyle ilgili, tanımlama, silme, listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class EVRTasfiye
    {
        /// <summary>
        /// Parametre olarak verilen EvrakTasfiye nesnesine göre, Tasfiye bilgilerini veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Tasfiye bilgilerini içeren nesne </param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            sonuc = KontrolEt(ve, kullanan, liste, "Yazma");

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABTasfiye to = new TABTasfiye(ve);

                    sonuc = to.Kaydet(liste);

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();

                        sonuc.bilgiStr += EvrakI.EVRI011;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{FF8A89A1-CA66-4E17-982C-39E5F7429C9C} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Saklama birimlerini veritabanından silen yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="liste">Saklama bilgilerini içeren nesne koleksiyonu</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        /// 

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABTasfiye to = new TABTasfiye(ve);

                    if (sonuc.hataStr == "")
                        sonuc = to.Sil(prEvrakNo);
                    else
                        sonuc.islemSonuc = false;

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = EvrakI.EVRI175;
                    }
                    else
                        ve.Rollback();

                }
                catch (Exception ex)
                {
                    ve.Rollback();
                    OrtakFonksiyonlar.HataStrYaz("{6935EBB4-5740-4217-9347-93E97AF004EB} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakSinif form)
        {
            ObjectArray hVAN = new ObjectArray();
            bool kullaniciEvrakYetkisi = false;

            if (kullanan == null) kullanan = new Kullanici();

            if (!TNS.EVR.Yetki.EvrakUstDereceYetkisiVarmi(kullanan, form.bulunduguYer) ||
                (form.evrakTipi == (int)ENUMEvrakTipi.GIDEN && form.hareket.kisi1 != "-100" && form.hareket.kisi1 != "-300"))
                form.hareket.kisi1 = kullanan.mernis;
            else
            {
                form.hareket.kisi1Ad = kullanan.mernis;
                kullaniciEvrakYetkisi = true;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, TABGenel.BaglantiSatiri);
            TABTasfiye to = new TABTasfiye(ve);
            IDataReader r = to.Listele(form);
            int sayac = 0;

            if (r != null)
            {
                while (r.Read())
                {
                    sayac++;

                    form = new EvrakSinif();
                    form.evrEvrakNo = VeriErisim.DegerAl(r, 0, "");
                    form.yil = VeriErisim.DegerAl(r, 1, 0);
                    form.evrakTipi = VeriErisim.DegerAl(r, 2, 0);
                    form.evrakBirimKodu = VeriErisim.DegerAl(r, 3, "");
                    form.evrakNo = VeriErisim.DegerAl(r, 4, 0);
                    form.evrakTarih = VeriErisim.DegerAl(r, 5);
                    //form.geldigiYerTur = VeriErisim.DegerAl(r, (int)TABEvrak.ENUMAlanlar.GELDIGIYERTUR, 0);
                    form.geldigiYer = VeriErisim.DegerAl(r, 6, "");
                    form.gelenEvrakNo = VeriErisim.DegerAl(r, 7, "");
                    form.gelenEvrakTarih = VeriErisim.DegerAl(r, 8);
                    form.konu = VeriErisim.DegerAl(r, 9, "");
                    form.gizlilik = VeriErisim.DegerAl(r, 10, 0);
                    form.yaziDurum = VeriErisim.DegerAl(r, 11, 0);
                    form.surecAd = VeriErisim.DegerAl(r, 12, "");
                    form.yaziDurumAd = VeriErisim.DegerAl(r, 13, "");
                    form.gizlilikAd = VeriErisim.DegerAl(r, 14, "");
                    form.hareket.islem = VeriErisim.DegerAl(r, 15, 0);
                    form.prEvrakNo = VeriErisim.DegerAl(r, 16, "");
                    string dosyaKodlari = VeriErisim.DegerAl(r, 18, "");
                    string[] dler = dosyaKodlari.Split(';');
                    form.tasfiye.tasfiyeKod = VeriErisim.DegerAl(r, 19, 0);
                    form.evrakOlusturmaTarihi = VeriErisim.DegerAl(r, 20);
                    form.evrakBirimKodu = VeriErisim.DegerAl(r, 21, "");
                    foreach (string d in dler)
                    {
                        DosyaPlani dosyaPlani = new DosyaPlani();
                        dosyaPlani.kod = d;
                        if (dosyaPlani.kod.Trim() != "")
                            form.dosyaKodlari.Add(dosyaPlani);
                    }

                    if (form.gelisSekli == -1) form.gelisSekli = 0;
                    if (form.gizlilik == -1) form.gizlilik = 0;
                    if (form.evrakCinsi == -1) form.evrakCinsi = 0;

                    hVAN.objeler.Add(form);
                }

                r.Close();
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();

            if (to.Hata() != "")
            {
                hVAN.sonuc.islemSonuc = false;
                hVAN.sonuc.hataStr = to.Hata();
            }

            else

                hVAN.sonuc.anahtar = sayac.ToString();

            return hVAN;
        }

        public static Sonuc KontrolEt(VeriErisim veriErisim, Kullanici kullanan, ObjectArray liste, string yetkiTipi)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (liste != null && yetkiTipi == "Yazma")
            {
                foreach (EvrakTasfiye bilgi in liste.objeler)
                {
                    if (bilgi.prEvrakNo == "")
                    {
                        hata = EvrakI.EVRI171 + "<br>";
                        break;
                    }
                    if (bilgi.tasfiyeKod == 0)
                    {
                        hata = EvrakI.EVRI170 + "<br>";
                        break;
                    }
                }
            }

            if (hata == "")
                sonuc.islemSonuc = true;

            sonuc.hataStr = hata;

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

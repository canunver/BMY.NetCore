
using TNS.Veri;
using TNS.KYM;
using TNS.EVR.IsKurallari.Veri;
using EVRServisI;
using System.Data;
using System;
namespace TNS.EVR.IsKurallari
{
    public class EVREvrakAdresDefteri
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakAdresDefteri form)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABEvrakAdresDefteri to = new TABEvrakAdresDefteri(ve);

                    sonuc = to.Kaydet(form);

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
                    OrtakFonksiyonlar.HataStrYaz("{9E7DF268-D74A-45B1-A5B1-8B1025C61FE1} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABEvrakAdresDefteri to = new TABEvrakAdresDefteri(ve);

                    if (sonuc.hataStr == "")
                        sonuc = to.Sil(kod);
                    else
                        sonuc.islemSonuc = false;

                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = EvrakI.EVRI097;
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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakAdresDefteri form, int grupKod)
        {
            ObjectArray hVAN = new ObjectArray();

            if (kullanan == null) kullanan = new Kullanici();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakAdresDefteri to = new TABEvrakAdresDefteri(ve);
            IDataReader r = to.Listele(form,grupKod);
            int sayac = 0;

            if (r != null)
            {
                while (r.Read())
                {
                    sayac++;

                    form = new EvrakAdresDefteri();
                    form.kod = VeriErisim.DegerAl(r, 0, 0);
                    form.adres = VeriErisim.DegerAl(r, 1, "");
                    form.adi = VeriErisim.DegerAl(r, 2, "");
                    form.islemYapan = VeriErisim.DegerAl(r, 3, "");
                    form.islemYapanBirimKod = VeriErisim.DegerAl(r, 4, "");
                    form.islemTarihi = VeriErisim.DegerAl(r,5);
                    
                   // string dosyaKodlari = VeriErisim.DegerAl(r, 18, "");
                    //foreach (string d in dler)
                    //{
                    //    DosyaPlani dosyaPlani = new DosyaPlani();
                    //    dosyaPlani.kod = d;
                    //    if (dosyaPlani.kod.Trim() != "")
                    //        form.dosyaKodlari.Add(dosyaPlani);
                    //}

                    hVAN.objeler.Add(form);
                }

                r.Close();
            }

            ve.VeriErisimKapat();

            if (to.Hata() != "")
            {
                hVAN.sonuc.islemSonuc = false;
                hVAN.sonuc.hataStr = to.Hata();
            }

            else

                hVAN.sonuc.anahtar = sayac.ToString();

            return hVAN;
        }

        public static Sonuc KontrolEt(VeriErisim veriErisim, Kullanici kullanan, EvrakAdresGrubTablosu form)
        {
            string hata = "";
            Sonuc sonuc = new Sonuc(false, "Hata oluştu", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            // aynı grup varmı yokmu kontrol et

            if (hata == "")
                sonuc.islemSonuc = true;

            sonuc.hataStr = hata;

            ve.VeriErisimKapat();

            return sonuc;
        }
    }
}

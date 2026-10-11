
using TNS.Veri;
using TNS.KYM;
using TNS.EVR.IsKurallari.Veri;
using EVRServisI;
using System.Data;
using System;
namespace TNS.EVR.IsKurallari
{
    public class EVREvrakAdresGrubu
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, EvrakAdresGrubuAdi form, ObjectArray adresListesi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();

                try
                {
                    TABEvrakAdresGrubu to = new TABEvrakAdresGrubu(ve);

                    sonuc = to.Kaydet(form, adresListesi);

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
                    OrtakFonksiyonlar.HataStrYaz("{D96A8700-C135-4254-9E6D-C7B791387290} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, int grupKod)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            if (sonuc.islemSonuc)
            {
                ve.BeginTransaction();
                try
                {
                    TABEvrakAdresGrubu to = new TABEvrakAdresGrubu(ve);

                    if (sonuc.hataStr == "")
                        sonuc = to.Sil(grupKod);
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
                    OrtakFonksiyonlar.HataStrYaz("{46BCC352-42D1-454C-99EE-23CF158AE4B4} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
                }

            }
            ve.VeriErisimKapat();

            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, EvrakAdresGrubuAdi form)
        {
            ObjectArray hVAN = new ObjectArray();

            if (kullanan == null) kullanan = new Kullanici();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABEvrakAdresGrubu to = new TABEvrakAdresGrubu(ve);
            IDataReader r = to.Listele(form);
            int sayac = 0;

            if (r != null)
            {
                while (r.Read())
                {
                    sayac++;

                    form = new EvrakAdresGrubuAdi();
                    form.kod = VeriErisim.DegerAl(r, 0, 0);
                    form.adi = VeriErisim.DegerAl(r, 1, "");
                    form.islemYapan = VeriErisim.DegerAl(r, 2, "");
                    form.islemYapanBirimKod = VeriErisim.DegerAl(r, 3, "");
                    form.islemTarihi = VeriErisim.DegerAl(r,4);
                    
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

        public static EvrakAdresDefteri[] ListeleGrupAdresleri(VeriErisim veriErisim, Kullanici kullanan, string[] gruplar)
        {
            if (kullanan == null) kullanan = new Kullanici();

            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            TABEvrakAdresGrubu to = new TABEvrakAdresGrubu(ve);
            IDataReader r = to.ListeleGrupAdres(gruplar);

            if (r != null)
            {
                while (r.Read())
                {
                    EvrakAdresDefteri form = new EvrakAdresDefteri();
                    form.kod = VeriErisim.DegerAl(r, 0, 0);
                    form.adres = VeriErisim.DegerAl(r, 1, "");

                    liste.objeler.Add(form);
                }

                r.Close();
            }

            ve.VeriErisimKapat();

            return (EvrakAdresDefteri[])liste.objeler.ToArray(typeof(EvrakAdresDefteri));
        }
        
    }
}

using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRDuyuruGrubu
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, DuyuruGrubu form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI153, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABDuyuruGrubu to = new TABDuyuruGrubu(ve, "EVRDUYURUGRUBU");

                sonuc = to.Kaydet(form);
                if (sonuc.islemSonuc)
                {
                    sonuc = to.KaydetKisiler(ve, form.kisiler, sonuc.anahtar);
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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, DuyuruGrubu form)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI155, "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);

            ve.BeginTransaction();
            try
            {
                TABDuyuruGrubu to = new TABDuyuruGrubu(ve, "EVRDUYURUGRUBU");
                sonuc = to.Sil(form);

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
                OrtakFonksiyonlar.HataStrYaz("{77FE29FE-DEA3-4FDB-AB68-78659A3173AF} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, DuyuruGrubu bilgi)
        {
            ObjectArray hVAN = new ObjectArray();
            IDataReader r;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDuyuruGrubu to = new TABDuyuruGrubu(ve, "EVRDUYURUGRUBU");

            r = to.Listele(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new DuyuruGrubu();

                    bilgi.grupID = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.grupAdi = VeriErisim.DegerAl(r, 1, "");
                    bilgi.islemYapan = VeriErisim.DegerAl(r, 2, "");
                    bilgi.islemTarihi = VeriErisim.DegerAl(r, 3);
                    bilgi.islemYapanBirimKod = VeriErisim.DegerAl(r, 4, "");
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }

        public static ObjectArray ListeleKisiler(VeriErisim veriErisim, Kullanici kullanan, DuyuruGrubuKisiler bilgi)
        {
            ObjectArray hVAN = new ObjectArray();
            IDataReader r;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABDuyuruGrubu to = new TABDuyuruGrubu(ve, "EVRDUYURUGRUBUKISILER");

            r = to.ListeleKisiler(bilgi);

            if (to.Hata() == "")
            {
                while (r.Read())
                {
                    bilgi = new DuyuruGrubuKisiler();

                    bilgi.grupKisilerID = VeriErisim.DegerAl(r, 0, 0);
                    bilgi.grupID = VeriErisim.DegerAl(r, 1, 0);
                    bilgi.KisiKimlikNo = VeriErisim.DegerAl(r, 2, "");
                    bilgi.KisiBirimKod = VeriErisim.DegerAl(r, 3, "");
                    bilgi.tip = VeriErisim.DegerAl(r, 4, 0);
                    hVAN.objeler.Add(bilgi);
                }
            }

            r.Close();

            ve.VeriErisimKapat();

            return hVAN;
        }
    }
}

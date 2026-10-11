using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.KYM;
using TNS.MUH;
using TNS.Veri;
using TNS.BUM.IsKurallari.Veri;

namespace TNS.BUM.IsKurallari
{
    public class NakitIhtiyacServisi
    {
        public static string VERIERISIMAD = "BaglantiSatiriBUM";

        private static bool YetkisiVarMi(Kullanici kullanan, string muhasebeKod, string birimKod, ENUMOGY ogy, Sonuc sonuc)
        {
            IMUHServis muhServis = TNS.MUH.Arac.Tanimla();
            bool yetki = muhServis.MuhasebeBirimYetkiVarMi(kullanan, muhasebeKod, birimKod, ogy);
            if (!yetki)
                sonuc.DegerAta("Bu işlemi yapmaya yetkiniz yok!");
            return yetki;
        }

        internal static ObjectArray ListeleGunlukNakitIhtiyac(VeriErisim veriErisim, Kullanici kullanan, TNSDateTime tarih, string birimKod, bool tarihAraligiOlsun)
        {
            ObjectArray oa = new ObjectArray();

            TNS.MUH.MuhasebeBirim muhBirim = new TNS.MUH.MuhasebeBirim();
            muhBirim.MuhasebeBirimKurum = birimKod;

            if (!YetkisiVarMi(kullanan, muhBirim.muhasebe.ToString(), muhBirim.birimKod, ENUMOGY.Gorme, oa.sonuc))
                return oa;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBUMNAKITPLANI tab = new TABBUMNAKITPLANI(ve);

            IDataReader dr;

            TNSDateTime tarih1 = new TNSDateTime(tarih.Oku());
            TNSDateTime tarih2 = new TNSDateTime(tarih.Oku());
            
            if (tarihAraligiOlsun)
            {
                tarih1.IsGunuEkle(-5);
                tarih2.IsGunuEkle(5);
            }

            dr = tab.ListeleGunlukNakitIhtiyac(1, tarih1, tarih2, muhBirim.muhasebe, muhBirim.birimKod);

            if (dr == null)
            {
                ve.VeriErisimKapat();
                oa.sonuc.DegerAta(ve.hata);
                return oa;
            }

            NakitPlanGrup oncDetay = null;

            while (dr.Read())
            {
                int kol = 0;

                int tur = VeriErisim.DegerAl(dr, kol++, 0);
                DateTime gunTarih = VeriErisim.DegerAl(dr, kol++).Oku().Date;
                int muhasebe = VeriErisim.DegerAl(dr, kol++, 0);
                string birim = VeriErisim.DegerAl(dr, kol++, "");
                string kurKod = OrtakFonksiyonlar.KodNoktaKoy("9999.9999", VeriErisim.DegerAl(dr, kol++, ""));

                int harcamaTur = VeriErisim.DegerAl(dr, kol++, 0);
                double miktar = VeriErisim.DegerAl(dr, kol++, 0.0);

                string muhasebeAd = VeriErisim.DegerAl(dr, kol++, "");
                string birimAd = VeriErisim.DegerAl(dr, kol++, "");
                NakitPlanGrup detay = new NakitPlanGrup
                {
                    tur = tur,
                    Muhasebe = muhasebe,
                    Birim = birim,
                    KurKod = kurKod,
                    muhasebeAd = muhasebeAd,
                    birimAd = birimAd
                };

                if (oncDetay == null || oncDetay.Anahtar != detay.Anahtar)
                {
                    oncDetay = detay;
                    oncDetay.GunleriEkle(tarih1, tarih2);
                    oa.Ekle(oncDetay);
                }

                GunDeger gunDeger = oncDetay.GunGetir(gunTarih);

                if (gunDeger != null)
                {
                    gunDeger.DegerAta(harcamaTur, miktar);
                }
            }

            dr.Close();
            ve.VeriErisimKapat();

            if (oa.ObjeSayisi == 0)
            {
                NakitPlanGrup detay = new NakitPlanGrup();
                detay.tur = 1;
                detay.Muhasebe = muhBirim.muhasebe;
                detay.Birim = muhBirim.birimKod;
                detay.KurKod = muhBirim.kurum;
                detay.GunleriEkle(tarih1, tarih2);
                oa.Ekle(detay);
            }

            return oa;
        }

        internal static Sonuc KaydetGunlukNakitIhtiyac(VeriErisim veriErisim, Kullanici kullanan, TNSDateTime tarih, string birimKod, GunDegerHarcamaTur[] harcamaTutarlari)
        {
            if (harcamaTutarlari == null || harcamaTutarlari.Length == 0)
                return new Sonuc("Harcama verisi gelmedi");

            Sonuc sonuc = new Sonuc();
            TNS.MUH.MuhasebeBirim muhBirim = new TNS.MUH.MuhasebeBirim();
            muhBirim.MuhasebeBirimKurum = birimKod;

            if (!YetkisiVarMi(kullanan, muhBirim.muhasebe.ToString(), muhBirim.birimKod, ENUMOGY.Yazma, sonuc))
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, VERIERISIMAD, TNS.KYM.Arac.KimlikBul(kullanan));
            TABBUMNAKITPLANI tab = new TABBUMNAKITPLANI(ve);

            ve.BeginTransaction();
            sonuc = tab.SilGunlukNakitIhtiyac(1, tarih, muhBirim.muhasebe, muhBirim.birimKod);

            for (int i = 0; i < harcamaTutarlari.Length && sonuc.islemSonuc; i++)
            {
                if (harcamaTutarlari[i].tutar > 0)
                {
                    sonuc = tab.EkleGunlukNakitIhtiyac(1, tarih, muhBirim.muhasebe, muhBirim.birimKod, muhBirim.kurum, harcamaTutarlari[i].harcamaTur, harcamaTutarlari[i].tutar);
                }
            }

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }
    }
}

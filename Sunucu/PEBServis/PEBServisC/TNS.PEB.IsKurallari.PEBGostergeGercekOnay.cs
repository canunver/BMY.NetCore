using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;

namespace TNS.PEB.IsKurallari
{
    public class PEBGostergeGercekOnay
    {
        public static Sonuc DurumDegistir(VeriErisim veriErisim, Kullanici kullanan, PerformansGostergeGerceklesen pgg, string islemTur)
        {
            Sonuc sonuc;
            if (islemTur == "onayKaldir")
            {
                sonuc = new Sonuc();
                if (!Yetki.YoneticiMi(kullanan))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = PerformansButceResI.PEB116;
                }
            }
            else
                sonuc = Yetki.OnaylamaHakkiVarMi(kullanan, pgg.ggKurKod);
            if (!sonuc.islemSonuc)
                return sonuc;

            string hata = string.Empty;
            if (pgg.yil <= 0)
                hata += PerformansButceResI.PEB039 + "<br>";
            if (string.IsNullOrEmpty(pgg.ggKurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            //if (pgg.perHedefKod <= 0)
            //    hata += PerformansButceResI.PEB069 + "<br>";
            if (pgg.ay <= 0)
                hata += PerformansButceResI.PEB070 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGostergeGercekOnay tGercekOnay = new TABGostergeGercekOnay(ve);
            string kurumKod = "0";

            bool onayliMi = tGercekOnay.OnayliMi(pgg.yil, kurumKod, 0, pgg.ay, pgg.ggKurKod);
            if (onayliMi && islemTur == "onayla")
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = PerformansButceResI.PEB112 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }
            if (!onayliMi && islemTur == "onayKaldir")
            {
                ve.VeriErisimKapat();
                sonuc.hataStr = PerformansButceResI.PEB113 + "<br>";
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve.BeginTransaction();

            if (islemTur == "onayla")
                sonuc = tGercekOnay.Kaydet(pgg.yil, kurumKod, 0, pgg.ay, pgg.ggKurKod);
            else
                sonuc = tGercekOnay.Sil(pgg.yil, kurumKod, 0, pgg.ay, pgg.ggKurKod);

            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.PEB114;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.PEB115 + "<br>" + sonuc.hataStr;
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static bool OnayliMi(VeriErisim veriErisim, Kullanici kullanan, PerformansGostergeGerceklesen pgg)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABGostergeGercekOnay tGercekOnay = new TABGostergeGercekOnay(ve);
            bool onayliMi = tGercekOnay.OnayliMi(pgg.yil, "0", 0, pgg.ay, pgg.ggKurKod);
            ve.VeriErisimKapat();
            return onayliMi;
        }
    }
}
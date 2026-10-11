using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using System.Data;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    public static class RTFRaporYonetim
    {
        internal static Sonuc Kaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, int sablonNo, RTFOutline[] rtfOutlineSatirlar)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRTFRapor tabRaporSablon = new TABRTFRapor(ve);
            Sonuc s = null;
            int maksNo = tabRaporSablon.MaksNoBul(sablonNo);

            ve.BeginTransaction();
            for (int i = 0; i < rtfOutlineSatirlar.Length; i++)
            {
                RTFOutline ol = rtfOutlineSatirlar[i];
                if (ol.kod <= 0)
                {
                    ol.kod = ++maksNo;
                    s = tabRaporSablon.Kaydet(sablonNo, ol);
                }
                else
                    s = tabRaporSablon.Guncelle(sablonNo, ol);
                if (!s.islemSonuc)
                    break;
            }
            if (s == null)
                s = new Sonuc(false, "Kaydedilecek kayıt bulunamadı!", "");

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static RTFOutline[] Listele(VeriErisim veriErisim, KYM.Kullanici kullanan, int sablonNo, int merkezTasra)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRTFRapor tabRaporSablon = new TABRTFRapor(ve);

            IDataReader dr = tabRaporSablon.Listele(sablonNo, merkezTasra);

            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception("Veriler okunamadı:" + ve.hata);
            }

            ArrayList al = new ArrayList();
            while (dr.Read())
            {
                RTFOutline ol = new RTFOutline();
                ol.kod = VeriErisim.DegerAl(dr, 0, 0);
                ol.sira = VeriErisim.DegerAl(dr, 1, 0);
                ol.metin = VeriErisim.DegerAl(dr, 2, "");
                ol.icerlek = VeriErisim.DegerAl(dr, 3, 0);
                al.Add(ol);
            }
            dr.Close();
            ve.VeriErisimKapat();

            return (RTFOutline[])al.ToArray(typeof(RTFOutline));
        }

        internal static Sonuc TurDegistir(VeriErisim veriErisim, KYM.Kullanici kullanan, int sablonNo, RTFOutline rtfOutline)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRTFRapor tabRaporSablon = new TABRTFRapor(ve);
            Sonuc s = null;
            TNS.KYM.IKYMServis kymServis = TNS.KYM.Arac.Tanimla();

            ve.BeginTransaction();
            s = tabRaporSablon.TurDegistir(sablonNo, rtfOutline);
            if (s.islemSonuc)
            {
                s = kymServis.BuyukMetinSakla(kullanan, "STRRTF", sablonNo.ToString() + "_" + rtfOutline.kod.ToString(), "tr", rtfOutline.varsayilan);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();
            return s;
        }

        internal static RTFOutline TurOku(VeriErisim veriErisim, KYM.Kullanici kullanan, int sablonNo, int kod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABRTFRapor tabRaporSablon = new TABRTFRapor(ve);

            IDataReader dr = tabRaporSablon.TurOku(sablonNo, kod);

            if (dr == null)
            {
                ve.VeriErisimKapat();
                throw new Exception("Veriler okunamadı:" + ve.hata);
            }

            RTFOutline ol = new RTFOutline();
            if (dr.Read())
            {
                ol.kod = VeriErisim.DegerAl(dr, (int)TABRTFRapor.ENUMAlanlarSABLON.KOD, 0);
                ol.sira = VeriErisim.DegerAl(dr, (int)TABRTFRapor.ENUMAlanlarSABLON.SIRA, 0);
                ol.metin = VeriErisim.DegerAl(dr, (int)TABRTFRapor.ENUMAlanlarSABLON.METIN, "");
                ol.icerlek = VeriErisim.DegerAl(dr, (int)TABRTFRapor.ENUMAlanlarSABLON.ICERLEK, 0);
                ol.merkezTasra = VeriErisim.DegerAl(dr, (int)TABRTFRapor.ENUMAlanlarSABLON.MERKEZTASRA, 0);
                ol.tur = VeriErisim.DegerAl(dr, (int)TABRTFRapor.ENUMAlanlarSABLON.TUR, "");
            }
            dr.Close();
            ve.VeriErisimKapat();


            TNS.KYM.IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
            ol.varsayilan = kymServis.BuyukMetinOku(kullanan, "STRRTF", sablonNo.ToString() + "_" + kod.ToString(), "tr");

            return ol;
        }

        internal static string RaporDetayOku(VeriErisim veriErisim, KYM.Kullanici kullanan, int sablonNo, int kod, int donemNo, int birimKod, int yil, bool varsayilanDondur)
        {
            TNS.KYM.IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
            string htmlMetin = null;
            htmlMetin = kymServis.BuyukMetinOku(kullanan, "STRRTF", sablonNo.ToString() + "_" + kod.ToString() + "_" + donemNo.ToString() + "_" + birimKod.ToString() + "_" + yil.ToString(), "tr");

            if (string.IsNullOrEmpty(htmlMetin) && varsayilanDondur)
            {
                RTFOutline ro = TurOku(null, kullanan, sablonNo, kod);
                htmlMetin = ro.varsayilan;
            }
            return htmlMetin;
        }

        internal static Sonuc RaporDetayYaz(VeriErisim veriErisim, KYM.Kullanici kullanan, int sablonNo, int kod, int donemNo, int birimKod, int yil, string htmlMetin)
        {
            TNS.KYM.IKYMServis kymServis = TNS.KYM.Arac.Tanimla();
            return kymServis.BuyukMetinSakla(kullanan, "STRRTF", sablonNo.ToString() + "_" + kod.ToString() + "_" + donemNo.ToString() + "_" + birimKod.ToString() + "_" + yil.ToString(), "tr", htmlMetin);
        }
    }
}

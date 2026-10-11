using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.KYM;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class GELIs
    {
        protected static string IsKontrol(Is bilgi, ENUMIslemTuru islemturu)
        {
            StringBuilder hatalar = new StringBuilder();
            if (islemturu != ENUMIslemTuru.LISTELEME)
            {
                if (bilgi.kod <= 0)
                    hatalar.Append("Kod Boş Olmamalıdır.<br/>");
                if (bilgi.tur <= 0)
                    hatalar.Append("Tür Boş Olmamalıdır.<br/>");
            }
            if (islemturu == ENUMIslemTuru.KAYIT)
            {
                if (string.IsNullOrEmpty(bilgi.ad))
                    hatalar.Append("Ad Boş Olmamalıdır.<br/>");
                if (string.IsNullOrEmpty(bilgi.ekoKod))
                    hatalar.Append("Ekonomik Kod Boş Olmamalıdır.<br/>");
                if (string.IsNullOrEmpty(bilgi.odemeHesapKod))
                    hatalar.Append("Hesap Kodu Boş Olmamalıdır.<br/>");
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Is bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = IsKontrol(bilgi, ENUMIslemTuru.KAYIT);
            if (hata.Trim() != string.Empty) hatalar.Append(hata);
            if (hatalar.ToString() != string.Empty)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIs tabIs = new TABIs(ve);
            ve.BeginTransaction();
            if (tabIs.KayitVarmi(bilgi.kod, bilgi.tur))
            {
                sonuc = tabIs.Guncelle(bilgi);
            }
            else
            {
                bilgi.kod = tabIs.SiraNoAl();
                if (bilgi.kod <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00094;
                }
                else
                    sonuc = tabIs.Kaydet(bilgi);
            }
            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00025;
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Is bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = IsKontrol(bilgi, ENUMIslemTuru.SILME);
            if (hata.Trim() != string.Empty) hatalar.Append(hata);
            if (hatalar.ToString() != string.Empty)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIs tabIs = new TABIs(ve);
            ve.BeginTransaction();
            tabIs.Sil(bilgi.kod, bilgi.tur);
            if (ve.hata.Trim() == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = TNS.GEL.GelirSurecleriResI.GEL00020;
            }
            else
            {
                ve.Rollback();
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Is bilgi)
        {
            ObjectArray donusdeger = new ObjectArray();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = IsKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != string.Empty) hatalar.Append(hata);
            if (hatalar.ToString() != string.Empty)
            {
                donusdeger.sonuc.islemSonuc = false;
                donusdeger.sonuc.hataStr = hatalar.ToString();
                return donusdeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABIs tabIs = new TABIs(ve);
            ve.BeginTransaction();
            IDataReader dr = tabIs.Listele(bilgi.kod, bilgi.tur);
            while (dr.Read())
            {
                Is iss = new Is();
                iss.kod = VeriErisim.DegerAl(dr, 0, 0);
                iss.ad = VeriErisim.DegerAl(dr, 1, "");
                iss.ekoKod = VeriErisim.DegerAl(dr, 2, "");
                iss.odemeHesapKod = VeriErisim.DegerAl(dr, 3, "");
                iss.tur = VeriErisim.DegerAl(dr, 4, 0);
                donusdeger.objeler.Add(iss);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusdeger;
        }


    }
}

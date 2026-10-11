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
    public class GELParca
    {
        protected static string ParcaKontrol(Parca bilgi, ENUMIslemTuru islemturu)
        {
            StringBuilder hatalar = new StringBuilder();
            if (islemturu != ENUMIslemTuru.LISTELEME)
            {
                if (bilgi.kod <= 0)
                    hatalar.Append("Kod Boş Olmamalıdır.<br/>");
            }
            if (islemturu == ENUMIslemTuru.KAYIT)
            {
                if (string.IsNullOrEmpty(bilgi.ad))
                    hatalar.Append("Ad Boş Olmamalıdır.<br/>");
                if (string.IsNullOrEmpty(bilgi.sayfa))
                    hatalar.Append("Sayfa Bilgisi Boş Olmamalıdır.<br/>");
                if (bilgi.yukseklik<=0)
                    hatalar.Append("Yükseklik Boş Olmamalıdır.<br/>");
                if (bilgi.genislik <= 0)
                    hatalar.Append("Genişlik Boş Olmamalıdır.<br/>");
            }
            return hatalar.ToString();
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Parca bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = ParcaKontrol(bilgi, ENUMIslemTuru.KAYIT);
            if (hata.Trim() != string.Empty) hatalar.Append(hata);
            if (hatalar.ToString() != string.Empty)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABParca tabParca = new TABParca(ve);
            ve.BeginTransaction();
            if (tabParca.KayitVarmi(bilgi.kod))
            {
                sonuc = tabParca.Guncelle(bilgi);
            }
            else
            {
                bilgi.kod = tabParca.SiraNoAl();
                if (bilgi.kod <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = TNS.GEL.GelirSurecleriResI.GEL00094;
                }
                else
                    sonuc = tabParca.Kaydet(bilgi);
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

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Parca bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = ParcaKontrol(bilgi, ENUMIslemTuru.SILME);
            if (hata.Trim() != string.Empty) hatalar.Append(hata);
            if (hatalar.ToString() != string.Empty)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABParca tabParca = new TABParca(ve);
            ve.BeginTransaction();
            tabParca.Sil(bilgi.kod);
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

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Parca bilgi)
        {
            ObjectArray donusdeger = new ObjectArray();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append(TNS.GEL.GelirSurecleriResI.GEL00003 + "<br/>");
            string hata = ParcaKontrol(bilgi, ENUMIslemTuru.LISTELEME);
            if (hata.Trim() != string.Empty) hatalar.Append(hata);
            if (hatalar.ToString() != string.Empty)
            {
                donusdeger.sonuc.islemSonuc = false;
                donusdeger.sonuc.hataStr = hatalar.ToString();
                return donusdeger;
            }
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriGEL");
            TABParca tabParca = new TABParca(ve);
            ve.BeginTransaction();
            IDataReader dr = tabParca.Listele(bilgi);
            while (dr.Read())
            {
                Parca parca = new Parca();
                parca.kod = VeriErisim.DegerAl(dr, 0, 0);
                parca.ad = VeriErisim.DegerAl(dr, 1, "");
                parca.sayfa= VeriErisim.DegerAl(dr, 2, "");
                parca.yukseklik = VeriErisim.DegerAl(dr, 3, 0);
                parca.genislik = VeriErisim.DegerAl(dr, 4, 0);
                donusdeger.objeler.Add(parca);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusdeger;
        }


    }
}

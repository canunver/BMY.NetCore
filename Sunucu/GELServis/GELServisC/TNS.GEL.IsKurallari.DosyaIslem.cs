using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using TNS.GEL.IsKurallari.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari
{
    public class DosyaIslem
    {
        public static string DosyaKontrol(Dosya bilgi,ENUMIslemTuru islemturu)
        { 
            StringBuilder hatalar=new StringBuilder();
            if (bilgi.sirano <= 0 && islemturu==ENUMIslemTuru.SILME)
                hatalar.Append("Dosya Sıra Numarası Sıfırdan Büyük Olmalıdır.<br/>");
            if (bilgi.yil <= 0)
                hatalar.Append("Dosya Yılı Sıfırdan Büyük Olmalıdır.<br/>");
            return hatalar.ToString();

        }

        public static Sonuc Kaydet(VeriErisim veriErisim, KYM.Kullanici kullanan, Dosya bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append("Yazma Yetkiniz Bulunmamaktadır" + "<br/>");
            string hata = DosyaKontrol(bilgi,ENUMIslemTuru.KAYIT);
            if (hata.Trim().Length > 0) hatalar.Append(hata);
            if (hatalar.ToString().Length > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM");
            TABDosyaIslem tabDosyaIslem = new TABDosyaIslem(ve);
            ve.BeginTransaction();
            tabDosyaIslem.Sil(bilgi);
            if (ve.hata.Trim() == "")
            {
                if (bilgi.sirano == 0)
                    bilgi.sirano = tabDosyaIslem.SiraNoAl();
                if (bilgi.sirano <= 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Sıra Numarası Sıfırdan Büyük Olmalıdır";
                }
                else
                {
                    sonuc = tabDosyaIslem.Kaydet(bilgi);
                    if (sonuc.islemSonuc)
                    {
                        ve.Commit();
                        sonuc.bilgiStr = "Kayıt Gerçekleştrildi...";
                    }
                    else
                    {
                        ve.Rollback();
                    }
                }
            }
            else
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ve.hata;
                ve.Rollback();

            }
            ve.VeriErisimKapat();
            return sonuc;
        }


        public static Sonuc Sil(VeriErisim veriErisim, KYM.Kullanici kullanan, Dosya bilgi)
        {
            Sonuc sonuc = new Sonuc();
            StringBuilder hatalar = new StringBuilder();
            if (!Yetki.YazmaYetkisiVarMi(kullanan))
                hatalar.Append("Silme Yetkiniz Bulunmamaktadır" + "<br/>");
            string hata = DosyaKontrol(bilgi, ENUMIslemTuru.SILME);
            if (hata.Trim().Length > 0) hatalar.Append(hata);
            if (hatalar.ToString().Length > 0)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hatalar.ToString();
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM");
            TABDosyaIslem tabDosyaIslem = new TABDosyaIslem(ve);
            ve.BeginTransaction();
            tabDosyaIslem.Sil(bilgi);
            if (ve.hata.Trim() == "")
            {
                ve.Commit();
                sonuc.islemSonuc = true;
                sonuc.bilgiStr = "Kayıt Silindi";
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

        public static ObjectArray Listele(VeriErisim veriErisim, KYM.Kullanici kullanan, Dosya bilgi)
        {
            ObjectArray donusdeger = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM");
            TABDosyaIslem tabDosyaIslem = new TABDosyaIslem(ve);
            ve.BeginTransaction();
            IDataReader dr = tabDosyaIslem.Listele(bilgi);
            while (dr.Read())
            {
                Dosya dosya = new Dosya();
                dosya.sirano = VeriErisim.DegerAl(dr, 0, 0);
                dosya.yil = VeriErisim.DegerAl(dr, 1, 0);
                dosya.anahtar = VeriErisim.DegerAl(dr, 2, "");
                dosya.dosyayolu = VeriErisim.DegerAl(dr, 3, "");
                dosya.dosyaadi = VeriErisim.DegerAl(dr, 4, "");
                dosya.tur = VeriErisim.DegerAl(dr, 5, 0);
                dosya.aciklama = VeriErisim.DegerAl(dr, 6, "");
                donusdeger.objeler.Add(dosya);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusdeger;
        }
    }
}

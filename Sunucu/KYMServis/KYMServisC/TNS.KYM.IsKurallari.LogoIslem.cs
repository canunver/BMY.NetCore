using System;
using System.Data;
using System.Linq;
using System.Text;
using TNS.KYM.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.KYM.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class LogoIslem
    {
        private static string VeriKontrol(Logo bilgi, ENUMIslemTuru islemtur)
        {
            StringBuilder hatalar = new StringBuilder();
            if (islemtur != ENUMIslemTuru.GORME)
            {
                if (bilgi.turkod <= 0)
                    hatalar.Append("Logo tür bilgisi tanımlanmamış.</br>");
            }
            if (islemtur == ENUMIslemTuru.KAYIT)
            {
                if (bilgi.data == null)
                    hatalar.Append("Geçerli bir resim seçilmemiş.</br>");
                else if (bilgi.data.Length > 30000)
                    hatalar.Append("Kayıt edilmek istenilen logonun dosya boyutu en fazla 30KB olmalıdır.</br>");
            }
            return hatalar.ToString();
        }

        public static Sonuc TabloOlustur(VeriErisim veriErisim, Kullanici kullanan)
        {
            return new Sonuc("Dinamik LOGO kullanımı aktif değildir!");
            //Sonuc sonuc = new Sonuc();
            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            //ve.BeginTransaction();
            //TABLogoIslem TABLogo = new TABLogoIslem(ve);
            //sonuc = TABLogo.TabloOlustur();
            //if (sonuc.islemSonuc)
            //    ve.Commit();
            //else
            //    ve.Rollback();
            //ve.VeriErisimKapat();
            //return sonuc;
        }

        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, Logo bilgi)
        {
            return new Sonuc("Dinamik LOGO kullanımı aktif değildir!");
            //Sonuc sonuc = new Sonuc();

            //string hatalar = VeriKontrol(bilgi, ENUMIslemTuru.KAYIT);
            //if (hatalar != string.Empty)
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = hatalar;
            //}
            //else
            //{
            //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            //    ve.BeginTransaction();
            //    TABLogoIslem TABLogo = new TABLogoIslem(ve);
            //    sonuc = TABLogo.Sil(bilgi.turkod);
            //    if (sonuc.islemSonuc)
            //    {
            //        sonuc = TABLogo.Kaydet(bilgi);
            //        if (sonuc.islemSonuc)
            //        {
            //            ve.Commit();
            //            sonuc.bilgiStr = "Logo başarıyla Kayıt edildi.";
            //        }
            //        else
            //            ve.Rollback();
            //    }
            //    else
            //    {
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = "Veri Kaydedilemedi";
            //        ve.Rollback();
            //    }

            //    ve.VeriErisimKapat();
            //}

            //return sonuc;
        }


        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, Logo[] bilgiler)
        {
            return new Sonuc("Dinamik LOGO kullanımı aktif değildir!");

            //Sonuc sonuc = new Sonuc();
            //string hatalar = "";
            //foreach (Logo bilgi in bilgiler)
            //    hatalar += VeriKontrol(bilgi, ENUMIslemTuru.SILME);

            //if (hatalar != string.Empty)
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = hatalar;
            //}
            //else
            //{
            //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            //    ve.BeginTransaction();
            //    TABLogoIslem TABLogo = new TABLogoIslem(ve);
            //    foreach (Logo item in bilgiler)
            //    {
            //        if (!sonuc.islemSonuc) break;
            //        sonuc = TABLogo.Sil(item.turkod);
            //    }

            //    if (sonuc.islemSonuc)
            //    {
            //        ve.Commit();
            //        sonuc.bilgiStr = "Logo başarıyla silindi.";
            //    }
            //    else
            //    {
            //        ve.Rollback();
            //    }
            //    ve.VeriErisimKapat();
            //}
            //return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, Logo bilgi)
        {
            ObjectArray donusdeger = new ObjectArray();
            donusdeger.sonuc.DegerAta("Dinamik LOGO kullanımı aktif değildir!");

            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriKYM", kullanan.KimlikBul());
            //ve.BeginTransaction();
            //TABLogoIslem TABLogo = new TABLogoIslem(ve);
            //IDataReader dr = TABLogo.Listele(bilgi);
            //if (dr != null)
            //{
            //    while (dr.Read())
            //    {
            //        Logo logo = new Logo();
            //        logo.turkod = VeriErisim.DegerAl(dr, 0, 0);
            //        logo.turad = LogoTurAdBul(logo.turkod);
            //        logo.data = (byte[])dr.GetValue(1);
            //        donusdeger.objeler.Add(logo);
            //    }
            //}
            //ve.VeriErisimKapat();
            return donusdeger;
        }

        private static string LogoTurAdBul(int turkod)
        {
            string ad = String.Empty;
            TNS.KYM.ENUMLogoTuru[] array = (TNS.KYM.ENUMLogoTuru[])(Enum.GetValues(typeof(TNS.KYM.ENUMLogoTuru)).Cast<TNS.KYM.ENUMLogoTuru>());
            foreach (TNS.KYM.ENUMLogoTuru item in array)
            {
                if ((int)item == turkod)
                {
                    ad = item.ToString();
                    break;
                }
            }
            return ad;
        }
    }
}

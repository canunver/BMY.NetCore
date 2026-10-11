using System;
using System.Collections;
using System.Data;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH.IsKurallari.Veri;
using TNS.Veri;
using System.Collections.Generic;
using TNS.DEG;

namespace TNS.MUH.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    public class MUHIsOdenek
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, IsOdenek[] liste)
        {
            if (liste == null || liste.Length == 0)
                return new Sonuc(false, "Kaydedilecek bilgi bulunamadı.", "");

            Sonuc sonuc = Kontrol("KAYDET", kullanan, liste);

            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABIsOdenek to = new TABIsOdenek(ve);
            TABGenel toGenel = new TABGenel(ve);

            sonuc = Sil(ve, kullanan, liste[0]);
            if (sonuc.islemSonuc)
            {
                foreach (var form in liste)
                {
                    sonuc = to.Kaydet(form);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi tamamlandı";
            }
            else
            {
                ve.Rollback();
                if (ve.hata != "")
                    sonuc.DegerAta("Hata: " + ve.hata);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, IsOdenek form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            sonuc = Kontrol("SIL", kullanan, new IsOdenek[] { form });
            if (!sonuc.islemSonuc)
                return sonuc;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            ve.BeginTransaction();

            TABIsOdenek to = new TABIsOdenek(ve);

            sonuc = to.Sil(form);

            if (sonuc.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, IsOdenek form)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsOdenek to = new TABIsOdenek(ve);
            Tertip t = new Tertip(form.yil, ENUMTabloKod.TABPEBOdenek);

            IDataReader dr = null;

            try
            {
                dr = to.Listele(form);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            while (dr.Read())
            {
                IsOdenek b = new IsOdenek();
                b.isKod = VeriErisim.DegerAl(dr, (int)TABIsOdenek.ENUMAlanlar.ISKOD, 0);
                b.yil = VeriErisim.DegerAl(dr, (int)TABIsOdenek.ENUMAlanlar.YIL, 0);
                b.kurKod = VeriErisim.DegerAl(dr, (int)TABIsOdenek.ENUMAlanlar.KURKOD, "");
                b.fonKod = VeriErisim.DegerAl(dr, (int)TABIsOdenek.ENUMAlanlar.FONKOD, "");
                b.finKod = VeriErisim.DegerAl(dr, (int)TABIsOdenek.ENUMAlanlar.FINKOD, "");
                b.ekoKod = VeriErisim.DegerAl(dr, (int)TABIsOdenek.ENUMAlanlar.EKOKOD, "");
                b.pebKod = VeriErisim.DegerAl(dr, (int)TABIsOdenek.ENUMAlanlar.PEBKOD, "");
                b.blokeTutar = VeriErisim.DegerAl(dr, (int)TABIsOdenek.ENUMAlanlar.BLOKETUTAR, 0.0);



                liste.Ekle(b);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return liste;
        }

        public static ObjectArray DurumListele(VeriErisim veriErisim, Kullanici kullanan, int yil, int muhasebe, string birim, int isKod)
        {
            ObjectArray liste = new ObjectArray();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", TNS.KYM.Arac.KimlikBul(kullanan));
            TABIsOdenek to = new TABIsOdenek(ve);
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            IDataReader dr = null;

            try
            {
                dr = to.DurumListele(yil, muhasebe, birim, isKod);
            }
            catch (Exception e)
            {
                liste.sonuc.islemSonuc = false;
                liste.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return liste;
            }

            while (dr.Read())
            {
                IsOdenek b = new IsOdenek();
                b.kurKod = VeriErisim.DegerAl(dr, 0, "");
                b.fonKod = VeriErisim.DegerAl(dr, 1, "");
                b.finKod = VeriErisim.DegerAl(dr, 2, "");
                b.ekoKod = VeriErisim.DegerAl(dr, 3, "");
                b.pebKod = VeriErisim.DegerAl(dr, 4, "");
                b.blokeTutar = VeriErisim.DegerAl(dr, 5, 0.0);

                b.yil = yil;

                liste.Ekle(b);
            }

            dr.Close();
            ve.VeriErisimKapat();

            return liste;
        }


        private static Sonuc Kontrol(string tip, Kullanici kullanan, IsOdenek[] liste)
        {
            Sonuc sonuc = new Sonuc();

            foreach (var form in liste)
            {
                if (tip == "SIL")
                {
                    if (form.isKod <= 0)
                        sonuc.hataStr = "Silinecek kaydı seçiniz.";
                }

                if (tip == "KAYDET" || tip == "GUNCELLE")
                {
                    if (form.yil <= 0)
                        sonuc.hataStr += "Yıl bilgisi boş olamaz." + "<br>";
                    if (form.isKod <= 0)
                        sonuc.hataStr += "İş kod bilgisi boş olamaz." + "<br>";
                    if (form.blokeTutar <= 0)
                        sonuc.hataStr += "Bloke tutarı sıfırdan büyük olmalıdır." + "<br>";
                }
            }

            if (!string.IsNullOrEmpty(sonuc.hataStr))
                sonuc.islemSonuc = false;
            else
                sonuc.islemSonuc = true;

            return sonuc;
        }

    }
}

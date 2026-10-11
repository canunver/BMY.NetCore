using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.PEB.IsKurallari.Veri;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    /// <summary> 
    /// Stratejik plan mevcut durum analizinde yapılacak olan Politik/Yasal, Ekonomik, Sosyal/Kültürel, Teknolojik (PEST) analizlerinin yönetilmesi 
    /// ile ilgili veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma, yazma, silme işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class STRPerformansHamVeri
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, PerformansHamVeri phVeri)
        {
            Sonuc sonuc = new Sonuc();

            string hata = string.Empty;
            if (phVeri.yil <= 0)
                hata += "Yıl boş bırakılamaz" + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPerformansHamVeri to = new TABPerformansHamVeri(ve);

            sonuc = to.Sil(phVeri);
            foreach (PerformansHamVeriDetay phVeriDetay in phVeri.detay)
            {
                sonuc = to.Kaydet(phVeri, phVeriDetay);
                if (!sonuc.islemSonuc) break;
            }
            if (sonuc.islemSonuc)
            {
                sonuc.bilgiStr = PerformansButceResI.STR010;
                ve.Commit();
            }
            else
            {
                sonuc.hataStr = PerformansButceResI.STR011 + "(" + sonuc.hataStr + ")";
                ve.Rollback();
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc ToplamAl(VeriErisim veriErisim, Kullanici kullanan, int yil, int IlceIlUlke, string ilceKodIlKod)
        {
            Sonuc sonuc = new Sonuc();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));

            ve.BeginTransaction();

            TABPerformansHamVeri to = new TABPerformansHamVeri(ve);
            ilceKodIlKod = ilceKodIlKod.Replace(".", "");
            IDataReader dr = null;
            try
            {
                dr = to.ToplamListele(yil, IlceIlUlke, ilceKodIlKod);
                if (dr == null)
                    sonuc.DegerAta(ve.hata);
            }
            catch (Exception e)
            {
                sonuc.DegerAta(e.Message);
            }

            if (sonuc.islemSonuc)
            {
                int oncBilgiTuru = -1;
                string oncBirimTuru = "";
                ArrayList liste = null;
                while (dr.Read())
                {
                    int bilgiTuru = VeriErisim.DegerAl(dr, 0, 0);
                    string birimTuru = VeriErisim.DegerAl(dr, 1, "");
                    string anahtar1 = VeriErisim.DegerAl(dr, 2, "");
                    string anahtar2 = VeriErisim.DegerAl(dr, 3, "");
                    string anahtar3 = VeriErisim.DegerAl(dr, 4, "");
                    string anahtar4 = VeriErisim.DegerAl(dr, 5, "");
                    double sumBilgi = VeriErisim.DegerAl(dr, 6, (double)0);
                    double sumBolen = VeriErisim.DegerAl(dr, 7, (double)0);
                    if (bilgiTuru != oncBilgiTuru || birimTuru != oncBirimTuru)
                    {
                        if (liste != null)
                            ToplamKaydet(kullanan, yil, oncBilgiTuru, oncBirimTuru, IlceIlUlke, ilceKodIlKod, liste);
                        liste = new ArrayList();
                        oncBilgiTuru = bilgiTuru;
                        oncBirimTuru = birimTuru;
                    }
                    PerformansHamVeriDetay phvd = new PerformansHamVeriDetay();
                    phvd.anahtar1 = anahtar1;
                    phvd.anahtar2 = anahtar2;
                    phvd.anahtar3 = anahtar3;
                    phvd.anahtar4 = anahtar4;
                    phvd.bilgi = sumBilgi;
                    phvd.bolen = sumBolen;
                    liste.Add(phvd);
                }
                if (liste != null)
                    ToplamKaydet(kullanan, yil, oncBilgiTuru, oncBirimTuru, IlceIlUlke, ilceKodIlKod, liste);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }

        private static void ToplamKaydet(Kullanici kullanan, int yil, int bilgiTuru, string birimTuru, int IlceIlUlke, string ilceKodIlKod, ArrayList liste)
        {
            PerformansHamVeri phVeri = new PerformansHamVeri();
            phVeri.yil = yil;
            phVeri.bilgiTuru = bilgiTuru;
            phVeri.birimToplam = IlceIlUlke + 1;
            phVeri.birim = new ORG.OrganizasyonBirim();

            phVeri.birim.turKod = birimTuru;
            if (IlceIlUlke == 1)
            {
                phVeri.birim.kod = OrtakFonksiyonlar.ConvertToInt(ilceKodIlKod, -1);
                phVeri.birim.ilce = ilceKodIlKod;
                phVeri.birim.il = ilceKodIlKod.Substring(0, 7);
            }
            else if (IlceIlUlke == 2)
            {
                phVeri.birim.kod = OrtakFonksiyonlar.ConvertToInt(ilceKodIlKod, -1);
                phVeri.birim.ilce = "";
                phVeri.birim.il = ilceKodIlKod;
            }
            else
            {
                phVeri.birim.kod = 0;
                phVeri.birim.ilce = "";
                phVeri.birim.il = "";
            }
            phVeri.detay = (PerformansHamVeriDetay[])liste.ToArray(typeof(PerformansHamVeriDetay));
            Kaydet(null, kullanan, phVeri);
        }

        public static PerformansHamVeri[] Listele(VeriErisim veriErisim, Kullanici kullanan, PerformansHamVeri phVeri)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABPerformansHamVeri to = new TABPerformansHamVeri(ve);

            IDataReader dr = null;
            try
            {
                dr = to.Listele(phVeri);
                if (dr == null)
                    throw new Exception(ve.hata);
            }
            catch (Exception e)
            {
                ve.VeriErisimKapat();
                throw e;
            }

            TNSCollection liste = new TNSCollection();
            TNSCollection detayListe = null;
            PerformansHamVeri gelen = null;
            while (dr.Read())
            {
                int orgKod = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.ORGKOD, 0);

                if (gelen == null || gelen.birim.kod != orgKod)
                {
                    if (gelen != null && detayListe != null)
                        gelen.detay = (PerformansHamVeriDetay[])detayListe.ToArray(typeof(PerformansHamVeriDetay));
                    gelen = new PerformansHamVeri();
                    gelen.yil = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.YIL, 0);
                    gelen.bilgiTuru = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.BILGITURU, 0);
                    gelen.birim = new ORG.OrganizasyonBirim();
                    gelen.birim.kod = orgKod;
                    gelen.birim.turKod = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.ORGTUR, "");
                    //gelen.birim.il = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.IL, "");
                    //gelen.birim.ilce = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.ILCE, "");
                    //gelen.birim.kopruKod = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.KOPRUKOD, "");
                    detayListe = new TNSCollection();
                    liste.Add(gelen);
                }

                PerformansHamVeriDetay detay = new PerformansHamVeriDetay();
                detay.anahtar1 = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.ANAHTAR1, "");
                detay.anahtar2 = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.ANAHTAR2, "");
                detay.anahtar3 = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.ANAHTAR3, "");
                detay.anahtar4 = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.ANAHTAR4, "");
                detay.bilgi = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.BILGI, 0.00);
                detay.bolen = VeriErisim.DegerAl(dr, (int)TABPerformansHamVeri.ENUMAlanlar.BOLEN, 1.00);
                detayListe.Add(detay);
            }
            dr.Close();
            ve.VeriErisimKapat();
            if (gelen != null && detayListe != null)
                gelen.detay = (PerformansHamVeriDetay[])detayListe.ToArray(typeof(PerformansHamVeriDetay));


            return (PerformansHamVeri[])liste.ToArray(typeof(PerformansHamVeri));
        }
    }
}
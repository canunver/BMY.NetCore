using System.Data;
using TNS.KYM;
using TNS.TRM.IsKurallari.Veri;
using TNS.Veri;
using System;
using System.Collections;

namespace TNS.TRM.IsKurallari
{
    public class TRMHibeYatirimMalzeme
    {
        public static Sonuc Kaydet(VeriErisim ve, Kullanici kullanan, YatirimMalzeme us)
        {
            Sonuc sonuc = new Sonuc();
            if (us.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (us.malzemeTurKod <= 0)
                sonuc.hataStr += "Malzeme türü boş bırakılamaz.<br>";
            if (string.IsNullOrEmpty(us.marka) && us.malzeme.kod == "")
                sonuc.hataStr += "Marka boş bırakılamaz.<br>";
            if (us.tutar <= 0 && us.malzeme.kod == "")
                sonuc.hataStr += "Tutar boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            TABYatirimMalzeme tabUS = new TABYatirimMalzeme(ve);
            ve.BeginTransaction();

            if (us.kod <= 0)
            {
                us.kod = new TABGenel(ve).BelgeNoAlInt("YATIRIMMALZEME", "000000000");
                sonuc = tabUS.Kaydet(us);
            }
            else
                sonuc = tabUS.Guncelle(us);

            if (sonuc.islemSonuc)
            {
                sonuc.anahtar = us.kod.ToString();
                ve.Commit();
                sonuc.bilgiStr = "Kayıt işlemi başarıyla tamamlandı.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Kayıt sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc Sil(VeriErisim ve, Kullanici kullanan, YatirimMalzeme us)
        {
            Sonuc sonuc = new Sonuc();
            if (us.hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (us.kod <= 0)
                sonuc.hataStr += "Malzeme Kodu boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            TABYatirimMalzeme tabUS = new TABYatirimMalzeme(ve);
            ve.BeginTransaction();
            sonuc = tabUS.Sil(us);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Malzeme bilgisi başarıyla silindi.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static Sonuc SilHibeKoduyla(VeriErisim ve, Kullanici kullanan, int hibeKod)
        {
            Sonuc sonuc = new Sonuc();
            if (hibeKod <= 0)
                sonuc.hataStr = "Hibe kodu boş bırakılamaz.<br>";
            if (!string.IsNullOrEmpty(sonuc.hataStr))
            {
                sonuc.islemSonuc = false;
                return sonuc;
            }

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);

            TABYatirimMalzeme tabUS = new TABYatirimMalzeme(ve);
            ve.BeginTransaction();
            sonuc = tabUS.SilHibeKoduyla(hibeKod);

            if (sonuc.islemSonuc)
            {
                ve.Commit();
                sonuc.bilgiStr = "Malzeme bilgisi başarıyla silindi.";
            }
            else
            {
                ve.Rollback();
                sonuc.hataStr = "Silme sırasında bir hata oluştu." + "<br>" + sonuc.hataStr;
            }

            ve.VeriErisimKapat();
            return sonuc;
        }

        public static YatirimMalzeme[] Listele(VeriErisim ve, Kullanici kullanan, YatirimMalzeme us)
        {
            if (us.hibeKod == 0)
                throw new Exception("Hibe kodu boş bırakılamaz.");

            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABYatirimMalzeme tabUS = new TABYatirimMalzeme(ve);
            IDataReader dr = tabUS.Listele(us);
            if (!string.IsNullOrEmpty(tabUS.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tabUS.Hata());
            }

            ArrayList al = new ArrayList();
            int sonKolon = (int)TABYatirimMalzeme.ENUMAlanlar.MIKTAR;
            while (dr.Read())
            {
                YatirimMalzeme gelen = new YatirimMalzeme();
                gelen.kod = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.KOD, 0);
                gelen.hibeKod = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.HIBEKOD, 0);
                gelen.malzemeTurKod = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.MALZEMETURKOD, 0);
                gelen.marka = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.MARKA, "");
                gelen.model = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.MODEL, "");
                gelen.ozellik = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.OZELLIK, "");
                gelen.tutar = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.TUTAR, 0.0);
                gelen.malzeme.kod = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.MALZEMEKOD, "");
                gelen.referansTutar = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.REFERANSTUTAR, 0.0);
                gelen.miktar = VeriErisim.DegerAl(dr, (int)TABYatirimMalzeme.ENUMAlanlar.MIKTAR, 0);

                gelen.malzemeTurAdi = VeriErisim.DegerAl(dr, sonKolon + 1, "");
                gelen.hibe.etapNo = VeriErisim.DegerAl(dr, sonKolon + 2, 0);
                gelen.hibe.basvuruNo = VeriErisim.DegerAl(dr, sonKolon + 3, "");
                gelen.hibe.kimlikNo = VeriErisim.DegerAl(dr, sonKolon + 4, "");
                gelen.hibe.ad = VeriErisim.DegerAl(dr, sonKolon + 5, "");
                gelen.hibe.soyad = VeriErisim.DegerAl(dr, sonKolon + 6, "");
                gelen.hibe.ilAd = VeriErisim.DegerAl(dr, sonKolon + 7, "");
                gelen.hibe.projeAdi = VeriErisim.DegerAl(dr, sonKolon + 8, "");
                gelen.hibe.projeNitelikAdi = VeriErisim.DegerAl(dr, sonKolon + 9, "");
                gelen.hibe.projeKonuAdi = VeriErisim.DegerAl(dr, sonKolon + 10, "");
                gelen.malzeme.ad = VeriErisim.DegerAl(dr, sonKolon + 11, "");
                gelen.malzemeAnaGrupKod = VeriErisim.DegerAl(dr, sonKolon + 12, 0);
                gelen.malzemeAnaGrupAdi = VeriErisim.DegerAl(dr, sonKolon + 13, "");
                gelen.kosul = VeriErisim.DegerAl(dr, sonKolon + 14, "");


                al.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (YatirimMalzeme[])al.ToArray(typeof(YatirimMalzeme));
        }

        public static YatirimMalzeme[] ListeleIstatistik(VeriErisim ve, Kullanici kullanan, YatirimMalzeme us)
        {
            ve = VeriErisim.VeriErisimBul(ve, TABGenel.BaglantiSatiri);
            TABYatirimMalzeme tabUS = new TABYatirimMalzeme(ve);
            IDataReader dr = tabUS.ListeleIstatistik(us);
            if (!string.IsNullOrEmpty(tabUS.Hata()))
            {
                ve.VeriErisimKapat();
                throw new Exception(tabUS.Hata());
            }

            ArrayList al = new ArrayList();
            int sonKolon = (int)TABYatirimMalzeme.ENUMAlanlar.TUTAR;
            while (dr.Read())
            {
                YatirimMalzeme gelen = new YatirimMalzeme();
                gelen.hibe.etapNo = VeriErisim.DegerAl(dr, 0, 0);
                gelen.hibe.ilAd = VeriErisim.DegerAl(dr, 1, "");
                gelen.malzemeTurAdi = VeriErisim.DegerAl(dr, 2, "");
                gelen.tutar = VeriErisim.DegerAl(dr, 3, 0.0);
                gelen.sorguAdet = VeriErisim.DegerAl(dr, 4, 0);

                al.Add(gelen);
            }
            dr.Close();

            ve.VeriErisimKapat();
            return (YatirimMalzeme[])al.ToArray(typeof(YatirimMalzeme));
        }
    }
}
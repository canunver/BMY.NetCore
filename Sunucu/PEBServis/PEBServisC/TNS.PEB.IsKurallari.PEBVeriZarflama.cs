using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.PEB.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.PEB.IsKurallari
{
    public static class PEBVeriZarflama
    {
        internal static DegiskenSablon[] DegiskenSablonListele(VeriErisim veriErisim, Kullanici kullanan)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tabDegiskenSablon = new TABVZATablolar(ve);

            IDataReader dr = tabDegiskenSablon.DegiskenSablonListele();

            System.Collections.ArrayList al = new System.Collections.ArrayList();
            if (dr != null)
            {
                while (dr.Read())
                {
                    al.Add(new DegiskenSablon(VeriErisim.DegerAl(dr, 0, -1), VeriErisim.DegerAl(dr, 1, "")));
                }
                dr.Close();
            }
            return (DegiskenSablon[])al.ToArray(typeof(DegiskenSablon));
        }

        internal static DegiskenSablon DegiskenSablonTanimEkle(VeriErisim veriErisim, Kullanici kullanan)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tabDegiskenSablon = new TABVZATablolar(ve);

            int enYuksekID = tabDegiskenSablon.EnYuksekSablonIDBul() + 1;

            DegiskenSablon d = new DegiskenSablon(enYuksekID, "Yeni Kayıt (" + enYuksekID.ToString() + ")");
            ve.BeginTransaction();
            Sonuc s = tabDegiskenSablon.DegiskenSablonEkle(d);
            if (s.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                d.kod = -1;
                d.metin = ve.hata;
            }
            ve.VeriErisimKapat();
            return d;
        }

        internal static Sonuc DegiskenSablonAdDegistir(VeriErisim veriErisim, Kullanici kullanan, int kod, string metin)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tabDegiskenSablon = new TABVZATablolar(ve);

            ve.BeginTransaction();
            Sonuc s = tabDegiskenSablon.DegiskenSablonAdDegistir(kod, metin);
            if (s.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc DegiskenSablonSil(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tabDegiskenSablon = new TABVZATablolar(ve);

            ve.BeginTransaction();
            Sonuc s = tabDegiskenSablon.DegiskenleriSil(kod);
            if (s.islemSonuc)
                s = tabDegiskenSablon.BirimleriSil(kod);
            if (s.islemSonuc)
                s = tabDegiskenSablon.DegiskenSablonSil(kod);
            if (s.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        //internal static KarsilastirmaGrubu[] KarsilastirmaGruplariListele(VeriErisim veriErisim, Kullanici kullanan, int kod = -1)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABVZATablolar tab = new TABVZATablolar(ve);

        //    IDataReader dr = tab.KarsilastirmaGruplariListele(kod);

        //    System.Collections.ArrayList al = new System.Collections.ArrayList();
        //    if (dr != null)
        //    {
        //        while (dr.Read())
        //        {
        //            KarsilastirmaGrubu kg = new KarsilastirmaGrubu(VeriErisim.DegerAl(dr, 0, -1), VeriErisim.DegerAl(dr, 1, ""));
        //            kg.cografiBirim = (ENUMCografiBirim)VeriErisim.DegerAl(dr, 2, 0);
        //            kg.karsilastirmaGrubu = (ENUMKarsilastirmaGrubu)VeriErisim.DegerAl(dr, 3, 0);
        //            al.Add(kg);
        //        }
        //        dr.Close();
        //    }
        //    return (KarsilastirmaGrubu[])al.ToArray(typeof(KarsilastirmaGrubu));
        //}

        //internal static Sonuc KarsilastirmaGrubuAdDegistir(VeriErisim veriErisim, Kullanici kullanan, int kod, string metin)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABVZATablolar tab = new TABVZATablolar(ve);

        //    ve.BeginTransaction();
        //    Sonuc s = tab.KarsilastirmaGrubuAdDegistir(kod, metin);
        //    if (s.islemSonuc)
        //        ve.Commit();
        //    else
        //    {
        //        ve.Rollback();
        //    }
        //    ve.VeriErisimKapat();
        //    return s;
        //}

        //internal static Sonuc KarsilastirmaGrubuSil(VeriErisim veriErisim, Kullanici kullanan, int kod)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABVZATablolar tab = new TABVZATablolar(ve);

        //    ve.BeginTransaction();
        //    Sonuc s = tab.KGBirimleriSil(kod);
        //    if (s.islemSonuc)
        //        s = tab.KarsilastirmaGrubuSil(kod);
        //    if (s.islemSonuc)
        //        ve.Commit();
        //    else
        //    {
        //        ve.Rollback();
        //    }
        //    ve.VeriErisimKapat();
        //    return s;
        //}

        //internal static KarsilastirmaGrubu KarsilastirmaGrubuTanimEkle(VeriErisim veriErisim, Kullanici kullanan)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABVZATablolar tabDegiskenSablon = new TABVZATablolar(ve);

        //    int enYuksekID = tabDegiskenSablon.EnYuksekKGIDBul() + 1;

        //    KarsilastirmaGrubu d = new KarsilastirmaGrubu(enYuksekID, "Yeni Kayıt (" + enYuksekID.ToString() + ")");
        //    ve.BeginTransaction();
        //    Sonuc s = tabDegiskenSablon.KarsilastirmaGrubuTanimEkle(d);
        //    if (s.islemSonuc)
        //        ve.Commit();
        //    else
        //    {
        //        ve.Rollback();
        //        d.kod = -1;
        //        d.metin = ve.hata;
        //    }
        //    ve.VeriErisimKapat();
        //    return d;
        //}

        internal static DegiskenSablon DegiskenSablonDetayOku(VeriErisim veriErisim, Kullanici kullanan, int kod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);
            IDataReader dr = tab.DegiskenleriOku(kod);
            ArrayList alGirdi = new ArrayList();
            ArrayList alCikti = new ArrayList();
            if (dr == null) { ve.VeriErisimKapat(); throw new Exception(ve.hata); }
            while (dr.Read())
            {
                int girdiCikti = VeriErisim.DegerAl(dr, 1, 1);
                STRGosterge gt = new STRGosterge();
                gt.kod = VeriErisim.DegerAl(dr, 2, 1);
                if (girdiCikti == 1)
                    alGirdi.Add(gt);
                else
                    alCikti.Add(gt);
            }
            dr.Close();
            DegiskenSablon d = new DegiskenSablon(kod, "");
            d.girdiler = (STRGosterge[])alGirdi.ToArray(typeof(STRGosterge));
            d.ciktilar = (STRGosterge[])alCikti.ToArray(typeof(STRGosterge));

            dr = tab.SablonOrgTurListele(kod);
            if (dr == null) { ve.VeriErisimKapat(); throw new Exception(ve.hata); }
            d.orgTurler = new ObjectArray();
            while (dr.Read())
            {
                SpGenelHavuzOrganizasyon item = new SpGenelHavuzOrganizasyon();
                item.orgKod = TNS.OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(dr, 1, ""));
                d.orgTurler.Ekle(item);
            }
            dr.Close();
            return d;
        }

        internal static Sonuc DegiskenSablonDetayYaz(VeriErisim veriErisim, Kullanici kullanan, DegiskenSablon d)
        {
            Sonuc s = new Sonuc(true, "", "");
            string hata;

            if (kullanan == null || !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
            {
                s.DegerAta("Bu işlemi yapmak için sistem yöneticisi olmanız gerekir.");
            }
            if (!DegiskenlerGecerliMi(d.ciktilar, "Çıktı", out hata))
            {
                s.DegerAta(hata);
            }

            if (!DegiskenlerGecerliMi(d.girdiler, "Girdi", out hata))
            {
                s.DegerAta(hata);
            }

            if (KesisenVarMi(d.girdiler, d.ciktilar, out hata))
            {
                s.DegerAta(hata);
            }

            if (d.orgTurler == null || d.orgTurler.ObjeSayisi == 0)
                s.DegerAta("Hiç organizayon türü girilmemiş.");
            if (!s.islemSonuc)
                return s;
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);

            ve.BeginTransaction();
            s = tab.DegiskenleriSil(d.kod);
            if (s.islemSonuc)
                s = tab.BirimleriSil(d.kod);

            for (int i = 0; s.islemSonuc && i < d.girdiler.Length; i++)
            {
                s = tab.DegiskenEkle(d, 1, i);
            }
            for (int i = 0; s.islemSonuc && i < d.ciktilar.Length; i++)
            {
                s = tab.DegiskenEkle(d, 2, i);
            }
            for (int i = 0; s.islemSonuc && i < d.orgTurler.ObjeSayisi; i++)
            {
                s = tab.OrgTurEkle(d, i);
            }
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();

            return s;
        }

        private static bool KesisenVarMi(STRGosterge[] d1, STRGosterge[] d2, out string hata)
        {
            for (int i = 0; i < d1.Length; i++)
            {
                for (int j = 0; j < d2.Length; j++)
                {
                    if (d1[i].kod == d2[j].kod)
                    {
                        hata = string.Format("Girdi değişkenlerinin {0}. elemanı ile, Çıktı değişkenlerinin {1}. elemanı aynı", (i + 1), (j + 1));
                        return true;
                    }
                }
            }
            hata = "";
            return false;
        }

        private static bool DegiskenlerGecerliMi(STRGosterge[] dler, string degiskenGrupAd, out string hata)
        {
            if (dler == null || dler.Length == 0)
            {
                hata = string.Format("{0} değikenleri boş olamaz", degiskenGrupAd);
                return false;
            }
            for (int i = 0; i < dler.Length; i++)
            {
                for (int j = i + 1; j < dler.Length; j++)
                {
                    if (dler[i].kod == dler[j].kod)
                    {
                        hata = string.Format("{0} değişkenlerinin {1}. elemanı ile {2}. elemanı aynı", degiskenGrupAd, (i + 1), (j + 1));
                        return false;
                    }
                }
            }
            hata = "";
            return true;
        }

        //internal static KarsilastirmaGrubu KarsilastirmaGrubuDetayOku(VeriErisim veriErisim, Kullanici kullanan, int kod)
        //{
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABVZATablolar tab = new TABVZATablolar(ve);

        //    KarsilastirmaGrubu[] dler = KarsilastirmaGruplariListele(ve, kullanan, kod);
        //    if (dler == null || dler.Length != 1) { ve.VeriErisimKapat(); return null; }

        //    KarsilastirmaGrubu d = dler[0];

        //    d.orgTurler = new ObjectArray();
        //    IDataReader dr = tab.KGOrgTurListele(kod);
        //    if (dr == null) { ve.VeriErisimKapat(); throw new Exception(ve.hata); }
        //    d.orgTurler = new ObjectArray();
        //    while (dr.Read())
        //    {
        //        SpGenelHavuzOrganizasyon item = new SpGenelHavuzOrganizasyon();
        //        item.orgKod = TNS.OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(dr, 1, ""));
        //        d.orgTurler.Ekle(item);
        //    }
        //    dr.Close();

        //    return d;
        //}

        //internal static Sonuc KarsilastirmaGrubuDetayYaz(VeriErisim veriErisim, Kullanici kullanan, KarsilastirmaGrubu d)
        //{
        //    Sonuc s = new Sonuc(true, "", "");

        //    if (kullanan == null || !kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
        //    {
        //        s.DegerAta("Bu işlemi yapmak için sistem yöneticisi olmanız gerekir.");
        //    }
        //    if (d.cografiBirim == ENUMCografiBirim.TANIMSIZ)
        //    {
        //        s.DegerAta("Geçerli bir coğrafi birim giriniz!");
        //    }

        //    if (d.karsilastirmaGrubu == ENUMKarsilastirmaGrubu.TANIMSIZ)
        //    {
        //        s.DegerAta("Geçerli bir karşılaştırma grubu giriniz!");
        //    }

        //    if (d.karsilastirmaGrubu == ENUMKarsilastirmaGrubu.KURUM)
        //    {
        //        if (d.orgTurler == null || d.orgTurler.ObjeSayisi == 0)
        //            s.DegerAta("Hiç organizayon türü girilmemiş.");
        //    }
        //    if (!s.islemSonuc)
        //        return s;
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABVZATablolar tab = new TABVZATablolar(ve);

        //    ve.BeginTransaction();
        //    s = tab.KGBirimleriSil(d.kod);
        //    if (s.islemSonuc)
        //        s = tab.KGBilgiGuncelle(d);

        //    for (int i = 0; s.islemSonuc && i < d.orgTurler.ObjeSayisi; i++)
        //    {
        //        s = tab.KGOrgTurEkle(d, i);
        //    }
        //    if (s.islemSonuc)
        //        ve.Commit();
        //    else
        //        ve.Rollback();
        //    ve.VeriErisimKapat();

        //    return s;
        //}

        internal static VZAAnalizGrup[] AnalizGrupDetayListele(VeriErisim veriErisim, Kullanici kullanan)
        {

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);

            ArrayList al = new ArrayList();
            ArrayList alIc = new ArrayList();
            IDataReader dr = tab.AnalizGrupListele();
            if (dr != null)
            {
                VZAAnalizGrup vg = null;
                while (dr.Read())
                {
                    int kod = VeriErisim.DegerAl(dr, 0, 0);
                    int altKod = VeriErisim.DegerAl(dr, 1, 0);
                    string metin = VeriErisim.DegerAl(dr, 2, "");
                    int degiskenKod = VeriErisim.DegerAl(dr, 3, 0);
                    ENUMCografiBirim kgKod = (ENUMCografiBirim)VeriErisim.DegerAl(dr, 4, 0);
                    int cozumYontem = VeriErisim.DegerAl(dr, 5, 0);
                    int varsayilan = VeriErisim.DegerAl(dr, 6, 0);
                    if (altKod == 0)
                    {
                        if (vg != null)
                        {
                            vg.analizDetaylar = (VZAAnalizDetay[])alIc.ToArray(typeof(VZAAnalizDetay));
                            alIc.Clear();
                            vg = null;
                        }
                        vg = new VZAAnalizGrup(kod, metin);
                        vg.varsayilan = varsayilan;
                        al.Add(vg);
                    }
                    else
                    {
                        VZAAnalizDetay vd = new VZAAnalizDetay(altKod, metin);
                        vd.degiskenKod = degiskenKod;
                        vd.kgKod = kgKod;
                        vd.cozumYontem = cozumYontem;
                        alIc.Add(vd);
                    }
                }
                if (vg != null)
                {
                    vg.analizDetaylar = (VZAAnalizDetay[])alIc.ToArray(typeof(VZAAnalizDetay));
                    alIc.Clear();
                    vg = null;
                }
            }
            return (VZAAnalizGrup[])al.ToArray(typeof(VZAAnalizGrup));
        }

        internal static VZAAnalizGrup AnalizGrupTanimEkle(VeriErisim veriErisim, Kullanici kullanan)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);

            int enYuksekID = tab.EnYuksekAnalizGrupIDBul(0) + 1;

            VZAAnalizGrup d = new VZAAnalizGrup(enYuksekID, "Yeni Kayıt (" + enYuksekID.ToString() + ")");
            ve.BeginTransaction();
            Sonuc s = tab.AnalizGrupEkle(enYuksekID, 0, d.metin, 0, 0, 0);
            if (s.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
                d.kod = -1;
                d.metin = ve.hata;
            }
            ve.VeriErisimKapat();
            return d;
        }

        internal static Sonuc AnalizGrupSil(VeriErisim veriErisim, Kullanici kullanan, int grupKod, int altKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);
            ve.BeginTransaction();

            Sonuc s = tab.SonucDegiskenSil(grupKod, altKod);
            if (s.islemSonuc)
                s = tab.SonucSil(grupKod, altKod);
            if (s.islemSonuc)
                s = tab.AnalizGrupCografiKodSil(grupKod, altKod);
            if (s.islemSonuc)
                s = tab.VZAIslemDetaySil(grupKod, altKod);
            if (s.islemSonuc)
                s = tab.VZAIslemAnaSil(grupKod, altKod);
            if (s.islemSonuc)
                s = tab.AnalizGrupSil(grupKod, altKod);

            if (s.islemSonuc)
            {
                ve.Commit();
                s.bilgiStr = PerformansButceResI.STR014;
            }
            else
            {
                ve.Rollback();
                s.hataStr = PerformansButceResI.STR015;
            }

            ve.VeriErisimKapat();
            return s;
        }

        internal static Sonuc VZAAnalizDetayTanimEkle(VeriErisim veriErisim, Kullanici kullanan, int grupKod, VZAAnalizDetay d)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);

            int enYuksekID = tab.EnYuksekAnalizGrupIDBul(grupKod) + 1;

            ve.BeginTransaction();
            Sonuc s = tab.AnalizGrupEkle(grupKod, enYuksekID, d.metin, d.degiskenKod, d.kgKod, d.cozumYontem);

            if (s.islemSonuc && d.cografiKodlar != null)
            {
                for (int i = 0; i < d.cografiKodlar.Length; i++)
                {
                    if (d.cografiKodlar[i].Length > 0)
                        s = tab.AnalizGrupCografiKodEkle(grupKod, enYuksekID, d.cografiKodlar[i]);
                }
            }

            if (s.islemSonuc)
            {
                s.anahtar = grupKod.ToString() + "_" + enYuksekID.ToString();
                ve.Commit();
            }
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();

            return s;
        }

        internal static Sonuc VZAAnalizGrupAdDegistir(VeriErisim veriErisim, Kullanici kullanan, int kod, int altKod, string metin)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);

            ve.BeginTransaction();
            Sonuc s = tab.AnalizGrupAdDegistir(kod, altKod, metin);
            if (s.islemSonuc)
                ve.Commit();
            else
            {
                ve.Rollback();
            }
            ve.VeriErisimKapat();
            return s;
        }

        internal static VZAAnalizDetay VZAIslemDurumListele(VeriErisim veriErisim, Kullanici kullanan, int islemYil, int grupKod, int altKod, int listelenecekler)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);
            VZAAnalizDetay a_d = AnalizDetayOku(tab, grupKod, altKod);
            a_d.durumKod = IslemDurumBul(tab, islemYil, grupKod, altKod);
            if (a_d.durumKod == 1) //bu işlem bu yıl için çalışmaya başlamış
            {
                int ilkGrupKod, ilkAltKod, ilkYil;
                a_d.IslenenYerler = IslemSonucBul(tab, islemYil, grupKod, altKod, true, listelenecekler, 1, 0, out ilkYil, out ilkGrupKod, out ilkAltKod);
            }
            else
            {
                a_d.IslenenYerler = IslemeAlinacaklarBul(tab, grupKod, altKod, true);
            }

            ve.VeriErisimKapat();

            return a_d;
        }

        private static VZAAnalizDetay AnalizDetayOku(TABVZATablolar tab, int grupKod, int altKod)
        {
            VZAAnalizDetay a_d = new VZAAnalizDetay(altKod, "");
            IDataReader dr = tab.AnalizDetayOku(grupKod, altKod);
            if (dr != null)
            {
                if (dr.Read())
                {
                    a_d.degiskenKod = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZTANIM.DEGISKENKOD, -1);
                    a_d.kgKod = (ENUMCografiBirim)VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZTANIM.KARGRUPKOD, -1);
                    a_d.cozumYontem = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZTANIM.COZUMYONTEM, -1);
                    a_d.metin = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZTANIM.METIN, "");
                }
                dr.Close();
            }
            return a_d;
        }

        private static VZAIslemSonuc[] IslemeAlinacaklarBul(TABVZATablolar tab, int grupKod, int altKod, bool aciklamali)
        {
            ArrayList al = new ArrayList();
            IDataReader dr = tab.CografiKodlariListele(grupKod, altKod);
            if (dr != null)
            {
                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                while (dr.Read())
                {
                    VZAIslemSonuc i_s = new VZAIslemSonuc();
                    i_s.kod = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZCOGRAFYA.COGRAFIKOD, "");
                    i_s.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99.999", i_s.kod);
                    if (aciklamali)
                        i_s.metin = uzyServis.UzayDegeriStr(null, "COGRAFYA", i_s.kod, true);
                    al.Add(i_s);
                }
                dr.Close();
                return (VZAIslemSonuc[])al.ToArray(typeof(VZAIslemSonuc));
            }
            return null;
        }

        private static VZAIslemSonuc[] IslemSonucBul(TABVZATablolar tab, int islemYil, int grupKod, int altKod, bool aciklamali, int listelenecekler, int basla, int kayitSay, out int ilkYil, out int ilkGrupKod, out int ilkAltKod)
        {
            ArrayList al = new ArrayList();
            IDataReader dr = tab.IslemDetayListele(grupKod, altKod, islemYil, listelenecekler, kayitSay);
            ilkGrupKod = -1;
            ilkAltKod = -1;
            ilkYil = -1;
            if (dr != null)
            {
                int say = 1;
                while (say < basla)
                {
                    dr.Read();
                    say++;
                }
                say = 0;
                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                while (dr.Read())
                {
                    VZAIslemSonuc i_s = new VZAIslemSonuc();
                    i_s.kod = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZISLEMDETAY.COGRAFIKOD, "");
                    i_s.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99.999.999", i_s.kod);
                    if (aciklamali)
                        i_s.metin = uzyServis.UzayDegeriStr(null, "COGRAFYA", i_s.kod, true);
                    i_s.durum = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZISLEMDETAY.DURUM, 0);
                    if (say == 0)
                    {
                        ilkGrupKod = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZISLEMDETAY.KOD, 0);
                        ilkAltKod = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZISLEMDETAY.ALTKOD, 0);
                        ilkYil = VeriErisim.DegerAl(dr, (int)TABVZATablolar.ENUMAlanlarANALIZISLEMDETAY.YIL, 0);
                    }
                    al.Add(i_s);
                    say++;
                    if (kayitSay > 0 && say >= kayitSay) break;
                }
                dr.Close();
                return (VZAIslemSonuc[])al.ToArray(typeof(VZAIslemSonuc));
            }
            return null;
        }

        private static int IslemDurumBul(TABVZATablolar tab, int islemYil, int grupKod, int altKod)
        {
            return tab.VZAIslemDurumBul(islemYil, grupKod, altKod);
        }

        internal static Sonuc VZAIslemeAl(VeriErisim veriErisim, Kullanici kullanan, int islemYil, int grupKod, int altKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);
            VZAAnalizDetay a_d = AnalizDetayOku(tab, grupKod, altKod);
            a_d.durumKod = IslemDurumBul(tab, islemYil, grupKod, altKod);
            Sonuc s = new Sonuc(true, "", "");
            if (a_d.durumKod == 1) //bu işlem bu yıl için çalışmaya başlamış
            {
                ve.VeriErisimKapat();
                s.DegerAta("Zaten devreye alınmış!");
                return s;
            }

            if (a_d.kgKod == ENUMCografiBirim.ULKE)
            {
                a_d.IslenenYerler = new VZAIslemSonuc[1];
                a_d.IslenenYerler[0] = new VZAIslemSonuc();
                a_d.IslenenYerler[0].kod = "01";
            }
            else
            {
                a_d.IslenenYerler = IslemeAlinacaklarBul(tab, grupKod, altKod, false);
                if (a_d.kgKod == ENUMCografiBirim.ILCE && a_d.IslenenYerler != null)
                {

                    TNS.BKD.IBKDServis bkdS = TNS.BKD.Arac.Tanimla();
                    ArrayList al = new ArrayList();
                    for (int i = 0; i < a_d.IslenenYerler.Length; i++)
                    {
                        TNS.BKD.ABSKodlarVAN akv = bkdS.KodListeleSeviye(0, "20", a_d.IslenenYerler[i].kod, 4, true, false);
                        if (akv.sonuc.islemSonuc)
                        {
                            foreach (TNS.BKD.ABSKod item in akv.absKodlar)
                            {
                                VZAIslemSonuc vis = new VZAIslemSonuc();
                                vis.kod = item.kod;
                                al.Add(vis);
                            }
                        }
                    }
                    a_d.IslenenYerler = (VZAIslemSonuc[])al.ToArray(typeof(VZAIslemSonuc));
                }
            }

            ve.BeginTransaction();

            s = tab.VZAIslemAnaKayitEkle(islemYil, grupKod, altKod, kullanan.kullaniciKodu, new TNSDateTime(DateTime.Now));
            if (s.islemSonuc && a_d.IslenenYerler != null)
            {
                for (int i = 0; i < a_d.IslenenYerler.Length; i++)
                {
                    s = tab.VZAIslemDetayKayitEkle(islemYil, grupKod, altKod, a_d.IslenenYerler[i].kod);
                    if (!s.islemSonuc) break;
                }
            }

            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();
            ve.VeriErisimKapat();

            return s;
        }

        internal static Sonuc VZALPIslem(VeriErisim veriErisim, Kullanici kullanan, int islemYil, int grupKod, int altKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);
            int ilkGrupKod, ilkAltKod, ilkYil;
            VZAIslemSonuc[] islemler = IslemSonucBul(tab, islemYil, grupKod, altKod, true, 1, 1, 1, out ilkYil, out ilkGrupKod, out ilkAltKod);
            Sonuc s = new Sonuc(true, "", "");
            if (islemler == null || islemler.Length == 0) return s;

            VZAIslemSonuc islem = islemler[0];
            if (islem.durum == 1)
                s = IslemDurumAta(ve, tab, islem, ilkYil, ilkGrupKod, ilkAltKod, 2);
            if (!s.islemSonuc) { ve.VeriErisimKapat(); return s; }
            VZAAnalizDetay a_d = AnalizDetayOku(tab, ilkGrupKod, ilkAltKod);
            DegiskenSablon ds = DegiskenSablonDetayOku(ve, kullanan, a_d.degiskenKod);

            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            LPUretim lpUretim = new LPUretim();
            lpUretim.yil = ilkYil;
            lpUretim.siraNo = ilkGrupKod.ToString() + "_" + ilkAltKod.ToString() + "_" + islem.kod.Replace(".", "");
            lpUretim.aciklama = islem.metin;

            ORG.OrganizasyonBirim p = new ORG.OrganizasyonBirim();
            if (islem.kod.Length <= 2)
            {
                p.kopruKod = "001";
            }
            else if (islem.kod.Length <= 9)
            {
                p.il = islem.kod;
                p.ilMi = 1;
            }
            else
            {
                p.ilce = islem.kod;
                p.ilceMi = 1;
            }

            TNS.ORG.IORGServis orgServis = TNS.ORG.Arac.Tanimla();
            ObjectArray oa = orgServis.OrganizasyonBirimListele(p, false, false);
            ORG.OrganizasyonBirim p2 = null;
            if (oa.ObjeSayisi > 0)
                p2 = (ORG.OrganizasyonBirim)oa[0];

            if (p2 == null)
            {
                IslemDurumAta(ve, tab, islem, ilkYil, ilkGrupKod, ilkAltKod, 6);
                ve.VeriErisimKapat();
                s.DegerAta("Aranan birim bulunamadığından işlem iptal edildi.");
                return s;
            }

            lpUretim.orgTur = StringeCevir(ds.orgTurler, ",");
            lpUretim.cozumYontem = a_d.cozumYontem;
            lpUretim.durum = islem.durum;
            lpUretim.kopruKod = p2.kopruKod;

            ArrayList al = new ArrayList();
            DiziyeEkle(al, ds.girdiler, 1);
            DiziyeEkle(al, ds.ciktilar, 2);
            lpUretim.degiskenler = (STRGosterge[])al.ToArray(typeof(STRGosterge));
            if (islem.durum == 2)
            {
                VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
                s = PEBLPUret.FormModelUret(uzyServis, kullanan, ve2, lpUretim);
                if (s.islemSonuc)
                {
                    s = IslemDurumAta(ve, tab, islem, ilkYil, ilkGrupKod, ilkAltKod, 3);
                    if (!s.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        return s;
                    }
                }
                else
                {
                    IslemDurumAta(ve, tab, islem, ilkYil, ilkGrupKod, ilkAltKod, 7);
                    ve.VeriErisimKapat();
                    return s;
                }
            }
            if (islem.durum == 3)
            {
                DataBaseBilgi dbBilgi = new DataBaseBilgi();
                dbBilgi.grupKod = ilkGrupKod;
                dbBilgi.islemYil = ilkYil;
                dbBilgi.altKod = ilkAltKod;
                dbBilgi.ve = ve;
                dbBilgi.cogKod = islem.kod;
                dbBilgi.tab = tab;
                dbBilgi.sonuc = new Sonuc(true, "", "");
                s = tab.IslemSonucDegiskenSil(ilkYil, ilkGrupKod, ilkAltKod, islem.kod);
                if (s.islemSonuc)
                    tab.IslemSonucAnaSil(ilkYil, ilkGrupKod, ilkAltKod, islem.kod);

                if (s.islemSonuc)
                    s = PEBLPUret.FormSonucUret(null, lpUretim, dbBilgi);
                if (s.islemSonuc)
                {
                    s = IslemDurumAta(ve, tab, islem, ilkYil, ilkGrupKod, ilkAltKod, 4);
                    if (!s.islemSonuc)
                    {
                        ve.VeriErisimKapat();
                        return s;
                    }
                }
                else
                {
                    IslemDurumAta(ve, tab, islem, ilkYil, ilkGrupKod, ilkAltKod, 8);
                    ve.VeriErisimKapat();
                    return s;
                }
            }
            if (islem.durum == 4)
            {
                //s = PEBLPUret.FormSonucYaz(null, lpUretim);
                s = IslemDurumAta(ve, tab, islem, ilkYil, ilkGrupKod, ilkAltKod, 5);
            }

            return s;
        }

        private static Sonuc IslemDurumAta(VeriErisim ve, TABVZATablolar tab, VZAIslemSonuc islem, int islemYil, int grupKod, int altKod, int durumKod)
        {
            ve.BeginTransaction();
            Sonuc s = tab.IslemDurumGuncelle(islemYil, grupKod, altKod, islem.kod, durumKod);
            if (s.islemSonuc)
            {
                islem.durum = durumKod;
                ve.Commit();
            }
            else
            {
                ve.Rollback();
            }
            return s;
        }

        private static void DiziyeEkle(ArrayList al, STRGosterge[] gostergeTanim, int girdiCikti)
        {
            foreach (STRGosterge item in gostergeTanim)
            {
                STRGosterge degisken = new STRGosterge();
                degisken.kod = item.kod;
                degisken.girdiCikti = girdiCikti;
                al.Add(degisken);
            }
        }

        private static string StringeCevir(ObjectArray objectArray, string ayrac)
        {
            string donDeger = "";
            foreach (SpGenelHavuzOrganizasyon item in objectArray.objeler)
            {
                if (donDeger != "") donDeger += ayrac;
                donDeger += item.orgKod;
            }
            return donDeger;
        }

        public static VZASonucBirim VZASonucDondur(VeriErisim veriErisim, Kullanici kullanan, int islemYil, int orgKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);

            int grupKod;
            int altKod;
            double amac;
            int ornek;
            VZABirimGrupAltGrupBul(tab, islemYil, orgKod, out grupKod, out altKod, out  amac, out  ornek);

            VZAAnalizDetay a_d = AnalizDetayOku(tab, grupKod, altKod);

            int sablonKod = a_d.degiskenKod;
            int girdiCikti = 1;
            if (a_d.cozumYontem >= 3)
                girdiCikti = 2;

            IDataReader dr = tab.SonucDegiskenOku(orgKod, islemYil, grupKod, altKod, sablonKod, girdiCikti);
            ArrayList degiskenler = new ArrayList();
            VZASonucBirim sb = new VZASonucBirim();
            sb.amac = amac;
            sb.ornekBirimSay = ornek;
            sb.degiskenler = new ArrayList();

            if (dr != null)
            {
                TNS.UZY.IUZYServis uzy = TNS.UZY.Arac.Tanimla();
                STRGosterge gt = new STRGosterge();
                while (dr.Read())
                {
                    VZASonucDegisken sonDeg = new VZASonucDegisken();
                    sonDeg.gostergeKod = VeriErisim.DegerAl(dr, 0, 0);
                    sonDeg.girisDeger = VeriErisim.DegerAl(dr, 1, 0.0);
                    sonDeg.cikisDeger = VeriErisim.DegerAl(dr, 2, 0.0);
                    sonDeg.oran = VeriErisim.DegerAl(dr, 3, 0.0);
                    sb.degiskenler.Add(sonDeg);

                    gt.kod = sonDeg.gostergeKod;
                    sonDeg.gostergeAd = gt.DegiskenAciklamaBul(uzy);
                }
                dr.Close();
            }
            return sb;
        }

        private static void VZABirimGrupAltGrupBul(TABVZATablolar tab, int islemYil, int orgKod, out int grupKod, out int altKod, out double amac, out int ornek)
        {
            grupKod = -1;
            altKod = -1;
            amac = -1;
            ornek = -1;
            IDataReader dr = tab.VZABirimGrupAltGrupBul(islemYil, orgKod);
            if (dr != null)
            {
                if (dr.Read())
                {
                    grupKod = VeriErisim.DegerAl(dr, 0, 0);
                    altKod = VeriErisim.DegerAl(dr, 1, 0);
                    amac = VeriErisim.DegerAl(dr, 2, 0.0);
                    ornek = VeriErisim.DegerAl(dr, 3, 0);
                }
                dr.Close();
            }
        }

        public static VZASonucBilgi VZASonucDondur(VeriErisim veriErisim, Kullanici kullanan, int islemYil, int grupKod, int altKod, string cogKod)
        {
            TNS.UZY.IUZYServis uzy = TNS.UZY.Arac.Tanimla();
            VZASonucBilgi sb = new VZASonucBilgi();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);

            IDataReader dr = tab.SonucBirimlerOku(islemYil, grupKod, altKod, cogKod);

            if (dr == null) { ve.VeriErisimKapat(); return null; }
            sb.sonucBirimler = new ArrayList();
            sb.gostergeler = new ArrayList();
            while (dr.Read())
            {
                VZASonucBirim sonucBirim = new VZASonucBirim();
                sonucBirim.orgKod = VeriErisim.DegerAl(dr, 0, 0);
                sonucBirim.amac = VeriErisim.DegerAl(dr, 1, 0.0);
                sonucBirim.ornekBirimSay = VeriErisim.DegerAl(dr, 2, 0);
                sonucBirim.degiskenler = new ArrayList();
                sb.sonucBirimler.Add(sonucBirim);
            }

            dr.Close();

            TNS.ORG.IORGServis orgServis = TNS.ORG.Arac.Tanimla();

            VZAAnalizDetay a_d = AnalizDetayOku(tab, grupKod, altKod);

            int sablonKod = a_d.degiskenKod;
            int girdiCikti = 1;
            if (a_d.cozumYontem >= 3)
                girdiCikti = 2;

            foreach (VZASonucBirim sonucBirim in sb.sonucBirimler)
            {
                ORG.OrganizasyonBirim p = new ORG.OrganizasyonBirim();
                p.kod = sonucBirim.orgKod;

                ObjectArray oa = orgServis.OrganizasyonBirimListele(p, false, false);
                if (oa.ObjeSayisi > 0)
                {
                    sonucBirim.orgAd = ((ORG.OrganizasyonBirim)oa[0]).adi;
                }

                dr = tab.SonucDegiskenOku(sonucBirim.orgKod, islemYil, grupKod, altKod, sablonKod, girdiCikti);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        VZASonucDegisken sonDeg = new VZASonucDegisken();
                        sonDeg.gostergeKod = VeriErisim.DegerAl(dr, 0, 0);
                        sonDeg.girisDeger = VeriErisim.DegerAl(dr, 1, 0.0);
                        sonDeg.cikisDeger = VeriErisim.DegerAl(dr, 2, 0.0);
                        sonDeg.oran = VeriErisim.DegerAl(dr, 3, 0.0);

                        sonucBirim.degiskenler.Add(sonDeg);
                        GostergeEkle(uzy, sb, sonDeg.gostergeKod);
                    }
                    dr.Close();
                }
            }

            ve.VeriErisimKapat();
            return sb;
        }

        private static void GostergeEkle(TNS.UZY.IUZYServis uzy, VZASonucBilgi sb, int gostergeKod)
        {
            STRGosterge gt = new STRGosterge();
            gt.kod = gostergeKod;
            if (!sb.gostergeler.Contains(gt))
            {
                gt.ad = gt.kod.ToString() + "-" + gt.DegiskenAciklamaBul(uzy);
                sb.gostergeler.Add(gt);
            }
        }

        internal static double VZANotBul(VeriErisim veriErisim, Kullanici kullanan, int yil, int orgKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);

            double d = tab.VZANotBul(yil, orgKod);
            ve.VeriErisimKapat();

            return d * 100;
        }

        internal static Sonuc VZAAnalizGrupVarsayilanYap(VeriErisim veriErisim, Kullanici kullanan, int grupKod, int varsayilan)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriSTR", TNS.KYM.Arac.KimlikBul(kullanan));
            TABVZATablolar tab = new TABVZATablolar(ve);
            ve.BeginTransaction();
            Sonuc s = tab.VZAAnalizGrupVarsayilanYap(grupKod, varsayilan);
            if (s.islemSonuc)
                ve.Commit();
            else
                ve.Rollback();

            ve.VeriErisimKapat();

            return s;
        }
    }
}

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.KYM;
using TNS.Veri;
using TNS.YAT.IsKurallari.Veri;
using TNS.BKD;
using TNS.BHMv2;
using TNS.UZY;
using System.Collections;

namespace TNS.YAT.IsKurallari
{
    /// <summary>
    /// Yatırım modülü raporlarıyla ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// raporları üreten fonksiyonları tutan sınıf.
    /// </summary>
    public class Raporlar
    {
        /// <summary>
        /// Bütçe hazırlık servisine ulaşmak için kullanılan değişken
        /// </summary>
        static IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();

        /// <summary>
        /// Uzaylar servisine ulaşmak için kullanılan değişken
        /// </summary>
        static IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// İstenen yatırım projesini verilen yatırım projeleri listesinden bulup döndüren yordam
        /// </summary>
        /// <param name="projeKod">Yatırım projesinin programatik kodu</param>
        /// <param name="objeler">Yatırım projelerini tutan nesne</param>
        /// <returns>Yatırım projesi listede bulunursa proje, bulunamazsa null döndürülür.</returns>
        private static Proje ProjeBul(int projeKod, TNSCollection objeler)
        {
            foreach (Proje p in objeler)
                if (projeKod == p.kod)
                    return p;

            return null;
        }

        /// <summary>
        /// Yatırım projeleri ve alt projeleri bilgilerini liste şeklinde üreten yordam
        /// Üretilen liste; rakamsiz parametresi false ise yatırım projeleri ve
        /// alt projeleri bazında maliyet, ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <param name="etudMu">Etüd mü değil mi</param>
        /// <param name="projeNo">Yatırım projesinin programatik kodu</param>
        /// <param name="rakamsiz">Dönecek liste; maliyet, ödenek teklif ve harcama bilgilerini içersin mi içermesin mi</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN ProjeAltProjeListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string etudMu, int projeNo, int projeTur, ENUMButceKaynakTur[] bkTur, bool rakamsiz, int ilkYil, bool kullanilmiyor, string kurKod, int altProjeNo, bool eskiYillarKBO, string araString, bool yetkiYok)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            BilgiVAN donusDeger = new BilgiVAN();
            if (!yetkiYok)
            {
                if (altProjeNo > 0 || projeNo > 0)
                {
                    Sonuc snc = ProjeClass.ProjeAltProjeYazmaGormeHakkiVarMi(ve, kullanan, projeNo, altProjeNo, yil, senaryoKod, ENUMOGY.Gorme);
                    if (snc != null)
                        donusDeger.sonuc = snc;
                }
                else
                    donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, kurKod);
            }
            if (!donusDeger.sonuc.islemSonuc)
            {
                ve.VeriErisimKapat();
                return donusDeger;
            }

            if (yil <= 0)
                donusDeger.sonuc.hataStr = YatirimResI.YAT032 + "<br>";
            if (string.IsNullOrEmpty(senaryoKod))
                donusDeger.sonuc.hataStr += YatirimResI.YAT052;
            if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            {
                donusDeger.sonuc.islemSonuc = false;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            string etud = etudMu;

            Proje gelen = null;
            int kolonSayProje = TABYATProje.ALANLAR.Length;
            int kolonSayAP = TABYATAltProje.ALANLAR.Length;
            int kolonSay = kolonSayProje + kolonSayAP;
            Tertip t = new Tertip(yil, ENUMTabloKod.TABOdenek);

            string[] birimKodlar;
            if (!yetkiYok)
                birimKodlar = Yetki.BirimKodlariDondur(kullanan, kurKod);
            else
                birimKodlar = null;

            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATRaporlar tabRaporlar = new TABYATRaporlar(ve);
            IDataReader dr = tabRaporlar.Tablo2Listele(yil, senaryoKod, sektorNo, etud, projeNo, projeTur, bkTur, birimKodlar, altProjeNo, araString);
            if (dr != null)
            {
                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                Hashtable ht = new Hashtable();
                Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                while (dr.Read())
                {
                    int projeKod = VeriErisim.DegerAl(dr, 0, -1);
                    if (donusDeger.liste.Count > 0)
                        gelen = ProjeBul(projeKod, donusDeger.liste);

                    if (gelen == null)
                    {
                        gelen = new Proje();
                        gelen.kod = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.PROJENO, -1);
                        gelen.DPTProjeNo = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.DPTPROJENO, string.Empty);

                        gelen.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99", VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.SEKTORNO, string.Empty));

                        gelen.tur.kod = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.TURKODU, -1);
                        gelen.ad = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.AD, string.Empty);
                        gelen.karakteristik = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.KARAKTERISTIK, string.Empty);
                        gelen.karakteristikOzet = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.KARAKTERISTIKOZET, string.Empty);
                        gelen.etudProjeMi = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.ETUDPROJEMI, string.Empty) == "E";
                        gelen.baslangicYili = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.BASLANGICYILI, 0);
                        gelen.bitisYili = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.BITISYILI, 0);

                        gelen.model = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.MODEL, -1);

                        string kurAd = "";
                        gelen.yurutecekKurum = ProjeClass.GecerliDegilseBosalt(kullanan, uzyServis, VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.YURUTECEKKURUM, ""), tUz.Kirilimlar(0), yil, ht, out kurAd);
                        gelen.yurutecekKurumAd = kurAd;

                        gelen.gerekce = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.GEREKCE, string.Empty);
                        gelen.aktifMi = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.AKTIF, 0) == 1;
                        gelen.eskiYeni = VeriErisim.DegerAl(dr, (int)TABYATProje.ENUMAlanlar.ESKIYENI, 0) == 1;

                        gelen.sektor.ad = VeriErisim.DegerAl(dr, kolonSay, string.Empty);
                        gelen.tur.ad = VeriErisim.DegerAl(dr, kolonSay + 1, string.Empty);

                        gelen.ilIlceler = IlIlce.Listele(ve2, kullanan, yil, senaryoKod, gelen.kod, 0);

                        donusDeger.liste.Add(gelen);
                    }

                    AltProje altProje = AltProjeyiAl(dr, kolonSayProje, senaryoKod, t);
                    gelen.altProjeler.Add(altProje);
                    altProje.ilIlceler = IlIlce.Listele(ve2, kullanan, yil, senaryoKod, gelen.kod, altProje.kod);

                    if (!rakamsiz)
                    {
                        BilgiVAN bilgiBK = ButceKaynakClass.BilgiOku(ve2, kullanan, 0, altProje.kod, 0, yil, senaryoKod, string.Empty, null, true, false, altProje.birim_kod, false, null, eskiYillarKBO);
                        if (!string.IsNullOrEmpty(ve2.hata) || !bilgiBK.sonuc.islemSonuc)
                        {
                            donusDeger.sonuc.DegerAta(ve2.hata + bilgiBK.sonuc.hataStr);
                            ve.VeriErisimKapat();
                            ve2.VeriErisimKapat();
                            return donusDeger;
                        }
                        for (int i = 0; i < bilgiBK.liste.Count; i++)
                        {
                            ButceKaynak bk = (ButceKaynak)bilgiBK.liste[i];
                            if (bk.teklifler.Count > 0)
                                altProje.butceKaynaklari.Add(bk);
                        }
                    }
                }
                dr.Close();
            }
            else
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = ve.hata;
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return donusDeger;
            }
            donusDeger.liste.Sort(new ProjeSirala(yil));
            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return donusDeger;
        }

        private class ProjeSirala : System.Collections.IComparer
        {
            int yil;

            public ProjeSirala(int yil)
            {
                this.yil = yil;
            }

            public int CompareIc(Proje x, Proje y)
            {
                int sektorSonuc = x.sektor.kod.CompareTo(y.sektor.kod);
                if (sektorSonuc != 0) return sektorSonuc;
                int edySonuc = x.EtudDevamEdenYeni(yil).CompareTo(y.EtudDevamEdenYeni(yil));
                if (edySonuc != 0) return edySonuc;
                int a = x.BuYilBiterMi(yil).CompareTo(y.BuYilBiterMi(yil));
                if (a != 0) return a;
                return x.DPTProjeNo.CompareTo(y.DPTProjeNo);
            }

            public int Compare(object x, object y)
            {
                return CompareIc((Proje)x, (Proje)y);
            }
        }

        public static BilgiVAN VizeCetveliRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string birimKod)
        {
            BilgiVAN bv = new BilgiVAN();

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));

            TABYATRaporlar tab = new TABYATRaporlar(ve);

            IDataReader dr = tab.VizeCetveliRaporla(yil, senaryoKod, birimKod, sektorNo);

            if (dr == null)
            {
                bv.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                return bv;
            }
            string oncAnahtar = "";
            object[] satirNesneler = null;
            while (dr.Read())
            {
                string SEKTORNO = VeriErisim.DegerAl(dr, 0, "");
                string SEKTORAD = VeriErisim.DegerAl(dr, 1, "");
                int PROJENO = VeriErisim.DegerAl(dr, 2, 0);
                string DPTPROJENO = VeriErisim.DegerAl(dr, 3, "");
                string PROJEAD = VeriErisim.DegerAl(dr, 4, "");
                int ALTPROJENO = VeriErisim.DegerAl(dr, 5, 0);
                string BIRIM = VeriErisim.DegerAl(dr, 6, "");
                string ALTPROJEAD = VeriErisim.DegerAl(dr, 7, "");
                string PEBKOD = VeriErisim.DegerAl(dr, 8, "");
                int TOPLAMADAHIL = VeriErisim.DegerAl(dr, 9, 0);
                string PEBAD = VeriErisim.DegerAl(dr, 10, "");
                string EKO1 = VeriErisim.DegerAl(dr, 11, "");
                int TEKLIFYIL = VeriErisim.DegerAl(dr, 12, 0);
                string FINKOD = VeriErisim.DegerAl(dr, 13, "");
                double TUTAR = VeriErisim.DegerAl(dr, 14, (double)0.0);
                string anahtar = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", SEKTORNO, PROJENO, ALTPROJENO, BIRIM, PEBKOD, EKO1);
                double sifir = 0;
                if (anahtar != oncAnahtar || satirNesneler == null)
                {
                    satirNesneler = new object[18];

                    satirNesneler[0] = SEKTORNO;
                    satirNesneler[1] = SEKTORAD;

                    satirNesneler[2] = PROJENO;
                    satirNesneler[3] = DPTPROJENO;
                    satirNesneler[4] = PROJEAD;

                    satirNesneler[5] = ALTPROJENO;
                    satirNesneler[6] = BIRIM;
                    satirNesneler[7] = ALTPROJEAD;

                    satirNesneler[8] = PEBKOD;
                    satirNesneler[9] = TOPLAMADAHIL;
                    satirNesneler[10] = PEBAD;

                    satirNesneler[11] = EKO1;

                    satirNesneler[12] = sifir;
                    satirNesneler[13] = sifir;
                    satirNesneler[14] = sifir;
                    satirNesneler[15] = sifir;
                    satirNesneler[16] = sifir;
                    satirNesneler[17] = sifir;
                    oncAnahtar = anahtar;
                    bv.liste.Add(satirNesneler);
                }
                int yer = TEKLIFYIL * 2;
                if (FINKOD == "07") yer++;
                satirNesneler[12 + yer] = TUTAR;
            }
            dr.Close();
            ve.VeriErisimKapat();
            return bv;
        }

        ///// <summary>
        ///// DPT Tablo-4 raporunu üreten yordam
        ///// Rapor; proje tutar ve karakteristik bilgilerini içerir.
        ///// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Verilerin alınacağı senaryo kod</param>
        ///// <param name="sektorNo">Sektör kriteri</param>
        ///// <param name="projeTuru">Proje tür kriteri</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında Tablo4 nesneleri döndürülür.</returns>
        //public static BilgiVAN Tablo4Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string sektorNo, int projeTuru)
        //{
        //    BilgiVAN donusDeger = new BilgiVAN();

        //    donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
        //    if (!donusDeger.sonuc.islemSonuc)
        //        return donusDeger;

        //    if (yil <= 0)
        //        donusDeger.sonuc.hataStr = YatirimResI.YAT032 + "<br>";
        //    if (projeTuru <= 0)
        //        donusDeger.sonuc.hataStr += YatirimResI.YAT053;
        //    if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        return donusDeger;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABYATRaporlar tabRaporlar = new TABYATRaporlar(ve);
        //    IDataReader dr = tabRaporlar.Tablo4Listele(yil, senaryoKod, sektorNo, projeTuru, Yetki.BirimKodlariDondur(kullanan));

        //    if (dr != null)
        //    {
        //        while (dr.Read())
        //        {
        //            Tablo4 gelen = new Tablo4();
        //            gelen.dptNo = VeriErisim.DegerAl(dr, 0, string.Empty);
        //            gelen.ad = VeriErisim.DegerAl(dr, 1, string.Empty);
        //            gelen.karakteristik = VeriErisim.DegerAl(dr, 2, string.Empty);
        //            gelen.butce = VeriErisim.DegerAl(dr, 3, (double)0);
        //            gelen.butceDis = VeriErisim.DegerAl(dr, 4, (double)0);

        //            donusDeger.liste.Add(gelen);
        //        }
        //        dr.Close();
        //    }
        //    if (!string.IsNullOrEmpty(ve.hata))
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = ve.hata;
        //    }
        //    else if (donusDeger.liste.Count <= 0)
        //    {
        //        donusDeger.sonuc.islemSonuc = true;
        //        donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;
        //    }

        //    ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        ///// <summary>
        ///// DPT Tablo-5 raporunu üreten yordam
        ///// Rapor; istenen projenin yıllar itibariyle proje tutar, ödenek teklif ve harcama bilgilerini içerir.
        ///// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        ///// </summary>
        ///// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        ///// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bir tane proje nesnesi döndürülür.</returns>
        //public static BilgiVAN Tablo5Listele(VeriErisim veriErisim, Kullanici kullanan, int projeKodu)
        //{
        //    BilgiVAN donusDeger = new BilgiVAN();

        //    donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
        //    if (!donusDeger.sonuc.islemSonuc)
        //        return donusDeger;

        //    if (projeKodu <= 0)
        //        donusDeger.sonuc.hataStr = YatirimResI.YAT054;
        //    if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        return donusDeger;
        //    }

        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
        //    TABYATRaporlar tabRaporlar = new TABYATRaporlar(ve);
        //    IDataReader dr = null;

        //    try
        //    {
        //        dr = tabRaporlar.Tablo5HarcamaBilgileriListele(projeKodu, Yetki.BirimKodlariDondur(kullanan));
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.Message;
        //        ve.VeriErisimKapat();
        //        return donusDeger;
        //    }

        //    int eskiKod = -2;
        //    Proje gelen = new Proje();
        //    donusDeger.liste.Add(gelen);
        //    AltProje altProje = new AltProje();
        //    gelen.altProjeler.Add(altProje);
        //    ButceKaynak bk = new ButceKaynak();
        //    altProje.butceKaynaklari.Add(bk);

        //    while (dr.Read())
        //    {
        //        if (eskiKod != VeriErisim.DegerAl(dr, 0, -1))
        //        {
        //            gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
        //            gelen.DPTProjeNo = VeriErisim.DegerAl(dr, 1, string.Empty);

        //            gelen.sektor.kod = VeriErisim.DegerAl(dr, 2, string.Empty);
        //            if (!string.IsNullOrEmpty(gelen.sektor.kod))
        //            {
        //                SektorSozluk s = new SektorSozluk();
        //                gelen.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy(s.kod, gelen.sektor.kod);
        //                gelen.sektor.ad = YATServis.UzayYaziDegerGetir("YATSEKTOR", gelen.sektor.kod, true, string.Empty);
        //            }

        //            gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
        //            if (gelen.tur.kod != -1)
        //                gelen.tur.ad = YATServis.UzayYaziDegerGetir("YATPROJETUR", gelen.tur.kod.ToString(), true, string.Empty);

        //            gelen.ad = VeriErisim.DegerAl(dr, 4, string.Empty);
        //            gelen.karakteristik = VeriErisim.DegerAl(dr, 5, string.Empty);
        //            gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, string.Empty);

        //            string etud = VeriErisim.DegerAl(dr, 7, string.Empty);
        //            if (etud == "E")
        //                gelen.etudProjeMi = true;
        //            else
        //                gelen.etudProjeMi = false;

        //            gelen.baslangicYili = VeriErisim.DegerAl(dr, 8, 0);
        //            gelen.bitisYili = VeriErisim.DegerAl(dr, 9, 0);

        //            gelen.il.kod = VeriErisim.DegerAl(dr, 10, -1);
        //            if (gelen.il.kod != -1)
        //                gelen.il.ad = YATServis.UzayYaziDegerGetir("YATIL", gelen.il.kod.ToString(), true, string.Empty);

        //            gelen.ilce.il.kod = gelen.il.kod;
        //            gelen.ilce.ilceKodu = VeriErisim.DegerAl(dr, 11, -1);
        //            if (gelen.ilce.il.kod != -1 && gelen.ilce.ilceKodu != -1)
        //                gelen.ilce.ilceAdi = YATServis.UzayYaziDegerGetir("YATILCE", gelen.ilce.il.kod.ToString() + "-" + gelen.ilce.ilceKodu.ToString(), true, string.Empty);

        //            gelen.model = VeriErisim.DegerAl(dr, 12, -1);
        //            gelen.yurutecekKurum = VeriErisim.DegerAl(dr, 13, string.Empty);
        //            gelen.gerekce = VeriErisim.DegerAl(dr, 14, string.Empty);

        //            eskiKod = gelen.kod;
        //        }

        //        //ButceKaynakHarcama harcama = new ButceKaynakHarcama();
        //        //harcama.yil = VeriErisim.DegerAl(dr, 15, 0);
        //        //harcama.yilSonuHarcama = VeriErisim.DegerAl(dr, 16, (double)0);
        //        //harcama.yilSonuHarcamaDis = VeriErisim.DegerAl(dr, 17, (double)0);
        //        //bk.harcamalar.Add(harcama);
        //    }
        //    dr.Close();

        //    try
        //    {
        //        TeklifBilgisiListele(ref bk, kullanan, tabRaporlar, projeKodu, 0, true);
        //        OdenekBilgisiListele(ref bk, kullanan, tabRaporlar, projeKodu, 0, "06.00", true);
        //    }
        //    catch (Exception e)
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = e.Message;
        //        ve.VeriErisimKapat();
        //        return donusDeger;
        //    }

        //    if (!string.IsNullOrEmpty(ve.hata))
        //    {
        //        donusDeger.sonuc.islemSonuc = false;
        //        donusDeger.sonuc.hataStr = ve.hata;
        //    }
        //    else if (donusDeger.liste.Count <= 0)
        //    {
        //        donusDeger.sonuc.islemSonuc = true;
        //        donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;
        //    }

        //    ve.VeriErisimKapat();
        //    return donusDeger;
        //}

        /// <summary>
        /// Verilen kriterlere uygun olan bütçede yer almayan ödenek teklif bilgilerini bulup bk değişkenine ekleyen yordam
        /// Eğer bk değişkeni içinde bütçede yer almayan ödenek teklif bilgilerini ekleyeceği uygun nesneyi bulursa ekler,
        /// bulamazsa ButceKaynakTeklif yapısından yeni bir nesne oluşturur ve bütçede yer almayan
        /// ödenek teklif bilgilerini bu nesneye doldurur, bu nesneyi de bk değişkenine ekler.
        /// </summary>
        /// <param name="bk">Teklif bilgilerini tutan nesne</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tabRaporlar">Raporlama ile ilgili veri tabanından bilgi okumaları yapan fonksiyonları tutan nesne</param>
        /// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="tablo5">Çağrının DPT Tablo-5 raporu için mi yoksa DPT Tablo-7 raporu için mi olduğunu gösteren parametre</param>
        private static void TeklifBilgisiListele(ref ButceKaynak bk, Kullanici kullanan, TABYATRaporlar tabRaporlar, int projeKodu, int yil, bool tablo5, string senaryoKod)
        {
            //????
            IDataReader dr = null;

            try
            {
                if (tablo5)
                    dr = tabRaporlar.Tablo5TeklifBilgileriListele(projeKodu, Yetki.BirimKodlariDondur(kullanan, null), senaryoKod);
                else
                    dr = tabRaporlar.Tablo7TeklifBilgisiListele(projeKodu, yil, Yetki.BirimKodlariDondur(kullanan, null), senaryoKod);
            }
            catch (Exception e)
            {
                throw e;
            }

            ButceKaynakTeklif teklif;
            ENUMProjeMaliYapiKod index = ENUMProjeMaliYapiKod.teklif0;
            while (dr.Read())
            {
                teklif = null;
                foreach (ButceKaynakTeklif t in bk.teklifler)
                {
                    if (t.butceYil == VeriErisim.DegerAl(dr, 0, 0))
                    {
                        teklif = t;
                        break;
                    }
                }
                if (teklif == null)
                {
                    teklif = new ButceKaynakTeklif();
                    teklif.butceYil = VeriErisim.DegerAl(dr, 0, 0);
                    bk.teklifler.Add(teklif);
                }

                index = (ENUMProjeMaliYapiKod)VeriErisim.DegerAl(dr, 1, 0);
                teklif.DegerAta(index, VeriErisim.DegerAl(dr, 2, (double)0), VeriErisim.DegerAl(dr, 3, (double)0), VeriErisim.DegerAl(dr, 4, (double)0));
            }
            dr.Close();
        }

        ///// <summary>
        ///// Verilen kriterlere uygun olan bütçede yer alan ödenek teklif bilgilerini bulup bk değişkenine ekleyen yordam
        ///// Eğer bk değişkeni içinde bütçede yer alan ödenek teklif bilgilerini ekleyeceği uygun nesneyi bulursa ekler,
        ///// bulamazsa ButceKaynakTeklif yapısından yeni bir nesne oluşturur ve bütçede yer alan
        ///// ödenek teklif bilgilerini bu nesneye doldurur, bu nesneyi de bk değişkenine ekler.
        ///// </summary>
        ///// <param name="bk">Teklif bilgilerini tutan nesne</param>
        ///// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        ///// <param name="tabRaporlar">Raporlama ile ilgili veri tabanından bilgi okumaları yapan fonksiyonları tutan nesne</param>
        ///// <param name="projeKodu">Yatırım projesinin programatik kodu</param>
        ///// <param name="yil">Yıl kriteri</param>
        ///// <param name="senaryoKod">Senaryo kriteri</param>
        ///// <param name="tablo5">Çağrının DPT Tablo-5 raporu için mi yoksa DPT Tablo-7 raporu için mi olduğunu gösteren parametre</param>
        //private static void OdenekBilgisiListele(ref ButceKaynak bk, Kullanici kullanan, TABYATRaporlar tabRaporlar, int projeKodu, int yil, string senaryoKod, bool tablo5)
        //{
        //    //??? bi bak
        //    //IDataReader dr = null;

        //    //try
        //    //{
        //    //    if (tablo5)
        //    //        dr = tabRaporlar.Tablo5OdenekBilgileriListele(projeKodu, senaryoKod, Yetki.BirimKodlariDondur(kullanan));
        //    //    else
        //    //        dr = tabRaporlar.Tablo7OdenekBilgisiListele(projeKodu, yil, senaryoKod, Yetki.BirimKodlariDondur(kullanan));
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    throw e;
        //    //}

        //    //ButceKaynakTeklif teklif;
        //    //int index = 0;
        //    //while (dr.Read())
        //    //{
        //    //    teklif = null;
        //    //    foreach (ButceKaynakTeklif t in bk.teklifler)
        //    //    {
        //    //        if (t.yil == VeriErisim.DegerAl(dr, 0, 0))
        //    //        {
        //    //            teklif = t;
        //    //            break;
        //    //        }
        //    //    }
        //    //    if (teklif == null)
        //    //    {
        //    //        teklif = new ButceKaynakTeklif();
        //    //        teklif.yil = VeriErisim.DegerAl(dr, 0, 0);
        //    //        bk.teklifler.Add(teklif);
        //    //    }

        //    //    teklif.yilButce.yil = VeriErisim.DegerAl(dr, 0, 0);
        //    //    index = VeriErisim.DegerAl(dr, 1, 0);
        //    //    teklif.yilButce.odenek[index] = VeriErisim.DegerAl(dr, 2, (double)0);
        //    //}
        //    //dr.Close();
        //}

        /// <summary>
        /// DPT Tablo-6 raporunu üreten yordam
        /// Rapor; istenen kredinin kimlik bilgilerini, projesinin kimlik bilgilerini ve yıllar itibariyle rakamsal bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kaynak">Kredinin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında proje nesneleri ve detayında da kredi bilgileri döndürülür.</returns>
        public static BilgiVAN Tablo6Listele(VeriErisim veriErisim, Kullanici kullanan, int kaynak)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            //donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            //if (!donusDeger.sonuc.islemSonuc)
            //    return donusDeger;

            //if (kaynak <= 0)
            //    donusDeger.sonuc.hataStr = YatirimResI.YAT055;
            //if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            //{
            //    donusDeger.sonuc.islemSonuc = false;
            //    return donusDeger;
            //}

            //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            //VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT");
            //TABYATRaporlar tabRaporlar = new TABYATRaporlar(ve);
            //IDataReader dr = null;

            //int altProjeKodu = tabRaporlar.AltProjeKoduAlFromKaynak(kaynak);
            //int projeKodu = tabRaporlar.ProjeKoduAlFromAltProje(altProjeKodu);
            //if (projeKodu <= 0)
            //{
            //    donusDeger.sonuc.islemSonuc = false;
            //    donusDeger.sonuc.hataStr = YatirimResI.YAT056;
            //    ve.VeriErisimKapat();
            //    ve2.VeriErisimKapat();
            //    return donusDeger;
            //}
            //int maxYil = tabRaporlar.MaxYilAlFromKaynakGeriOdeme(kaynak);

            //try
            //{
            //    dr = tabRaporlar.Tablo6ProjeVeTeklifBilgisiListele(projeKodu, Yetki.BirimKodlariDondur(kullanan));
            //}
            //catch (Exception e)
            //{
            //    donusDeger.sonuc.islemSonuc = false;
            //    donusDeger.sonuc.hataStr = e.Message;
            //    ve.VeriErisimKapat();
            //    ve2.VeriErisimKapat();
            //    return donusDeger;
            //}

            //while (dr.Read())
            //{
            //    Proje gelen = new Proje();
            //    gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
            //    gelen.DPTProjeNo = VeriErisim.DegerAl(dr, 1, string.Empty);

            //    gelen.sektor.kod = VeriErisim.DegerAl(dr, 2, string.Empty);
            //    if (!string.IsNullOrEmpty(gelen.sektor.kod))
            //    {
            //        SektorSozluk s = new SektorSozluk();
            //        gelen.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy(s.kod, gelen.sektor.kod);
            //        gelen.sektor.ad = YATServis.UzayYaziDegerGetir("YATSEKTOR", gelen.sektor.kod, true, string.Empty);
            //    }

            //    gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
            //    if (gelen.tur.kod != -1)
            //        gelen.tur.ad = YATServis.UzayYaziDegerGetir("YATPROJETUR", gelen.tur.kod.ToString(), true, string.Empty);

            //    gelen.ad = VeriErisim.DegerAl(dr, 4, string.Empty);
            //    gelen.karakteristik = VeriErisim.DegerAl(dr, 5, string.Empty);
            //    gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, string.Empty);

            //    string etud = VeriErisim.DegerAl(dr, 7, string.Empty);
            //    if (etud == "E")
            //        gelen.etudProjeMi = true;
            //    else
            //        gelen.etudProjeMi = false;

            //    gelen.baslangicYili = VeriErisim.DegerAl(dr, 8, 0);
            //    gelen.bitisYili = VeriErisim.DegerAl(dr, 9, 0);

            //    gelen.il.kod = VeriErisim.DegerAl(dr, 10, -1);
            //    if (gelen.il.kod != -1)
            //        gelen.il.ad = YATServis.UzayYaziDegerGetir("YATIL", gelen.il.kod.ToString(), true, string.Empty);

            //    gelen.ilce.il.kod = gelen.il.kod;
            //    gelen.ilce.ilceKodu = VeriErisim.DegerAl(dr, 11, -1);
            //    if (gelen.ilce.il.kod != -1 && gelen.ilce.ilceKodu != -1)
            //        gelen.ilce.ilceAdi = YATServis.UzayYaziDegerGetir("YATILCE", gelen.ilce.il.kod.ToString() + "-" + gelen.ilce.ilceKodu.ToString(), true, string.Empty);

            //    gelen.model = VeriErisim.DegerAl(dr, 12, -1);
            //    gelen.yurutecekKurum = VeriErisim.DegerAl(dr, 13, string.Empty);
            //    gelen.gerekce = VeriErisim.DegerAl(dr, 14, string.Empty);

            //    AltProje altProje = new AltProje();
            //    gelen.altProjeler.Add(altProje);
            //    ButceKaynak bk = new ButceKaynak();
            //    altProje.butceKaynaklari.Add(bk);

            //    ButceKaynakTeklif projeRakam = new ButceKaynakTeklif();
            //    projeRakam.projeButce[0] = VeriErisim.DegerAl(dr, 15, (double)0);
            //    projeRakam.projeButceDis[0] = VeriErisim.DegerAl(dr, 16, (double)0);
            //    projeRakam.yilButceDis[0] = VeriErisim.DegerAl(dr, 17, (double)0);
            //    bk.teklifler.Add(projeRakam);

            //    BilgiVAN detay = ButceKaynakClass.Listele(ve2, kullanan, kaynak, "06.00", true);
            //    if (detay.sonuc.islemSonuc && detay.liste.Count > 0)
            //    {
            //        ButceKaynak butceKaynak = (ButceKaynak)detay.liste[0];
            //        altProje.butceKaynaklari.Add(butceKaynak);

            //        ButceKaynakGeriOdeme bulunan = null;
            //        foreach (ButceKaynakGeriOdeme go in butceKaynak.geriOdeme)
            //        {
            //            if (maxYil == go.yil)
            //            {
            //                bulunan = go;
            //                break;
            //            }
            //        }
            //        butceKaynak.geriOdeme.Clear();
            //        butceKaynak.geriOdeme.Add(bulunan);
            //    }

            //    donusDeger.liste.Add(gelen);
            //}

            //if (!string.IsNullOrEmpty(ve.hata + ve2.hata))
            //{
            //    donusDeger.sonuc.islemSonuc = false;
            //    donusDeger.sonuc.hataStr = ve.hata + ve2.hata;
            //}
            //else if (donusDeger.liste.Count <= 0)
            //{
            //    donusDeger.sonuc.islemSonuc = true;
            //    donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;
            //}

            //dr.Close();
            //ve.VeriErisimKapat();
            //ve2.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// DPT Tablo-7 raporunu üreten yordam
        /// Rapor; istenen kredi talebin ve projesinin bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="krediTalepNo">Kredi talebin programatik kodu</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında proje nesneleri ve detayında da kredi talep bilgileri döndürülür.</returns>
        public static BilgiVAN Tablo7Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int krediTalepNo)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (krediTalepNo <= 0)
                donusDeger.sonuc.hataStr = YatirimResI.YAT057;
            if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            {
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            VeriErisim ve2 = VeriErisim.VeriErisimBul(null, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATRaporlar tabRaporlar = new TABYATRaporlar(ve);
            IDataReader dr = null;

            int altProjeKodu = KrediTalepClass.AltProjeKoduBul(ve, krediTalepNo);
            int projeKodu = tabRaporlar.ProjeKoduAlFromAltProje(yil, senaryoKod, altProjeKodu);
            if (projeKodu <= 0)
            {
                donusDeger.sonuc.DegerAta(YatirimResI.YAT058);
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return donusDeger;
            }

            dr = tabRaporlar.Tablo6ProjeVeTeklifBilgisiListele(yil, senaryoKod, projeKodu, Yetki.BirimKodlariDondur(kullanan, null));
            if (dr == null)
            {
                donusDeger.sonuc.DegerAta(ve.hata);
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return donusDeger;
            }
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            Hashtable ht = new Hashtable();
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            while (dr.Read())
            {
                Proje gelen = new Proje();
                gelen.kod = VeriErisim.DegerAl(dr, 0, -1);
                gelen.DPTProjeNo = VeriErisim.DegerAl(dr, 1, string.Empty);

                gelen.sektor.kod = VeriErisim.DegerAl(dr, 2, string.Empty);
                if (!string.IsNullOrEmpty(gelen.sektor.kod))
                {
                    SektorSozluk s = new SektorSozluk();
                    gelen.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy(s.kod, gelen.sektor.kod);
                    gelen.sektor.ad = YATServis.UzayYaziDegerGetir("YATSEKTOR", gelen.sektor.kod, true, string.Empty);
                }

                gelen.tur.kod = VeriErisim.DegerAl(dr, 3, -1);
                if (gelen.tur.kod != -1)
                    gelen.tur.ad = YATServis.UzayYaziDegerGetir("YATPROJETUR", gelen.tur.kod.ToString(), true, string.Empty);

                gelen.ad = VeriErisim.DegerAl(dr, 4, string.Empty);
                gelen.karakteristik = VeriErisim.DegerAl(dr, 5, string.Empty);
                gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 6, string.Empty);

                string etud = VeriErisim.DegerAl(dr, 7, string.Empty);
                if (etud == "E")
                    gelen.etudProjeMi = true;
                else
                    gelen.etudProjeMi = false;

                gelen.baslangicYili = VeriErisim.DegerAl(dr, 8, 0);
                gelen.bitisYili = VeriErisim.DegerAl(dr, 9, 0);

                gelen.model = VeriErisim.DegerAl(dr, 10, -1);

                string kurAd = "";
                gelen.yurutecekKurum = ProjeClass.GecerliDegilseBosalt(kullanan, uzyServis, VeriErisim.DegerAl(dr, 11, ""), tUz.Kirilimlar(0), yil, ht, out kurAd);
                gelen.yurutecekKurumAd = kurAd;

                gelen.gerekce = VeriErisim.DegerAl(dr, 12, string.Empty);

                AltProje altProje = new AltProje();
                gelen.altProjeler.Add(altProje);
                ButceKaynak bk = new ButceKaynak();
                altProje.butceKaynaklari.Add(bk);

                ButceKaynakTeklif projeRakam = new ButceKaynakTeklif();
                projeRakam.DegerAta(0, VeriErisim.DegerAl(dr, 13, (double)0), VeriErisim.DegerAl(dr, 14, (double)0), VeriErisim.DegerAl(dr, 15, (double)0));
                bk.teklifler.Add(projeRakam);

                BilgiVAN bilgiKrediTalep = KrediTalepClass.Listele(ve2, kullanan, altProjeKodu, krediTalepNo);
                if (bilgiKrediTalep.sonuc.islemSonuc)
                    foreach (KrediTalep kt in bilgiKrediTalep.liste)
                        altProje.krediTalepleri.Add(kt);

                donusDeger.liste.Add(gelen);
            }
            dr.Close();

            try
            {
                foreach (Proje p in donusDeger.liste)
                {
                    ButceKaynak butceKaynak = new ButceKaynak();
                    TeklifBilgisiListele(ref butceKaynak, kullanan, tabRaporlar, projeKodu, yil, false, senaryoKod);
                    ((AltProje)p.altProjeler[0]).butceKaynaklari.Add(butceKaynak);
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                ve2.VeriErisimKapat();
                return donusDeger;
            }

            if (!string.IsNullOrEmpty(ve.hata + ve2.hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = ve.hata + ve2.hata;
            }
            else if (donusDeger.liste.Count <= 0)
            {
                donusDeger.sonuc.islemSonuc = true;
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;
            }

            ve.VeriErisimKapat();
            ve2.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// DPT Tablo-10 raporunu üreten yordam
        /// Rapor; yatırım projeleri ve alt projeleri bazında kamulaştırma maliyet,
        /// kamulaştırma ödenek teklif ve kamulaştırma harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <param name="kurKod">Kurumsal Kod</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN Tablo10Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string sektorNo, string kurKod)
        {
            BilgiVAN donusDeger = ProjeAltProjeListele(veriErisim, kullanan, yil, senaryoKod, sektorNo, "H", -5, -1, null, true, 0, false, kurKod, -1, true, "", false);
            if (!donusDeger.sonuc.islemSonuc || donusDeger.liste.Count <= 0)
                return donusDeger;

            try
            {
                KamulastirmaBilgileriEkle(kullanan, yil, senaryoKod, ref donusDeger.liste, kurKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.liste.Count <= 0)
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

            return donusDeger;
        }

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık kamulaştırma (ekonomik 06.4) ödenek tekliflerini
        /// yatırım projesi ve alt projesi bazında bulup projeler değişkenine ekleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="projeler">Kamulaştırma bilgileri istenen yatırım projeleri ve alt projeleri</param>
        /// <param name="kurKod">Kurumsal Kod</param>
        private static void KamulastirmaBilgileriEkle(Kullanici kullanan, int yil, string senaryoKod, ref TNSCollection projeler, string kurKod)
        {
            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            tertip.kodlar[0] = kurKod;
            tertip.kodlar[3] = "06.4";

            UcgenKodBilgileri kodBilgi = new UcgenKodBilgileri();
            kodBilgi.Add(new UcgenKodBilgi(4, 2));

            EkDegerParam[] ekBilgi = new EkDegerParam[4];
            ekBilgi[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -1, 0, "06.00");
            ekBilgi[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgi[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, senaryoKod);
            ekBilgi[3] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, senaryoKod);

            double harcamaTahmin = servisUZY.UzayDegeriDbl(kullanan, "ABSHARCAMATAHMIN", yil - 1 + "-" + senaryoKod + "-" + tertip.kodlar[0] + "---" + tertip.kodlar[3], false);

            try
            {
                foreach (Proje p in projeler)
                {
                    foreach (AltProje ap in p.altProjeler)
                    {
                        tertip.kodlar[4] = PebKodVer(kullanan, yil, 0, ap.kod, senaryoKod);
                        if (string.IsNullOrEmpty(tertip.kodlar[4]))
                            continue;
                        TNS.BHMv2.OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, tertip, kodBilgi, ekBilgi, true);
                        if (odnler.Length <= 0)
                            continue;

                        TNSCollection rakamlar;
                        foreach (TNS.BHMv2.OdenekBilgi odn in odnler)
                        {
                            rakamlar = new TNSCollection();
                            for (int i = 0; i < odn.ekRakamlar.Count; i++)
                            {
                                rakamlar.Add(odn.ekRakamlar[i]);
                                if (i == 0)
                                    rakamlar.Add(harcamaTahmin);
                            }
                            odn.ekRakamlar = rakamlar;
                            ap.odenekBilgiler.Add(odn);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Gönderilen data readerdan baslaKolon kolon numarasından başlayarak yatırım alt projesini toplayan ve döndüren yordam
        /// </summary>
        /// <param name="dr">Yatırım alt projesini tutan data reader nesnesi</param>
        /// <param name="baslaKolon">Yatırım alt projesinin başladığı kolon numarası</param>
        /// <returns>Yatırım alt projesi bilgilerini tutan nesne döndürülür.</returns>
        public static AltProje AltProjeyiAl(IDataReader dr, int baslaKolon, string senaryoKod, Tertip t)
        {
            AltProje gelen = new AltProje();
            gelen.kod = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.ALTPROJENO, -1);
            gelen.birim.kod = OrtakFonksiyonlar.KodNoktaKoy((t.Kirilimlar(0)), VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.BIRIM, string.Empty));
            gelen.proje.kod = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.PROJENO, -1);
            gelen.tur.kod = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.TURKODU, -1);
            gelen.ad = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.AD, string.Empty);
            gelen.karakteristik = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.KARAKTERISTIK, string.Empty);
            gelen.karakteristikOzet = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.KARAKTERISTIKOZET, string.Empty);
            gelen.sorumluAd = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.SORUMLUADSOYAD, string.Empty);
            gelen.sorumluGorev = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.SORUMLUGOREV, string.Empty);
            gelen.sorumluTelefon = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.SORUMLUTELEFON, string.Empty);
            gelen.sorumluEPosta = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.SORUMLUEPOSTA, string.Empty);
            gelen.onerenAd = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.ONERENAD, string.Empty);
            gelen.onerenAdres = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.ONERENADRES, string.Empty);
            gelen.onerenTelefon = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.ONERENTELEFON, string.Empty);
            gelen.onerenFaks = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.ONERENFAKS, string.Empty);
            gelen.gerekce = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.GEREKCE, string.Empty);
            gelen.amac = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.AMAC, string.Empty);
            gelen.hedef = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.HEDEF, string.Empty);
            gelen.maliyet = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.MALIYET, (double)0);
            gelen.finansmanKaynak = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.FINANSMANKAYNAK, string.Empty);
            gelen.baslangicYil = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.BASLANGICYILI, 0);
            gelen.bitisYil = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.BITISYILI, 0);
            gelen.model = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.MODEL, -1);
            gelen.DPTDeVar = VeriErisim.DegerAl(dr, baslaKolon + (int)TABYATAltProje.ENUMAlanlar.DPTDEVAR, 1) == 1;

            if (!string.IsNullOrEmpty(gelen.birim.kod))
                gelen.birim.AdAta(YATServis.UzayYaziDegerGetir("ABSKUR", t.yil.ToString() + "-" + gelen.birim.kod, true, string.Empty));
            //if (gelen.proje.kod != -1)
            //    gelen.proje.ad = YATServis.UzayYaziDegerGetir("YATPROJE", t.yil + "-" + senaryoKod + "-" + gelen.proje.kod.ToString(), true, string.Empty);
            if (gelen.tur.kod != -1)
                gelen.tur.ad = YATServis.UzayYaziDegerGetir("YATPROJETUR", gelen.tur.kod.ToString(), true, string.Empty);

            return gelen;
        }

        /// <summary>
        /// DPT Tablo-12-13 raporunu üreten yordam
        /// Rapor; model bazında yatırım projelerinin tutar ve karakteristik bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorNo">Sektor kriteri</param>
        /// <param name="model">Model kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN Tablo1213Listele(VeriErisim veriErisim, Kullanici kullanan, int yil, string sektorNo, int model, string senaryoKod, string kurKod)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, kurKod);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil <= 0)
                donusDeger.sonuc.hataStr = YatirimResI.YAT032 + "<br>";
            if (model <= 0)
                donusDeger.sonuc.hataStr += YatirimResI.YAT059;
            if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            {
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATRaporlar tabRaporlar = new TABYATRaporlar(ve);
            IDataReader dr = null;

            try
            {
                dr = tabRaporlar.Tablo1213Listele(yil, senaryoKod, sektorNo, model, Yetki.BirimKodlariDondur(kullanan, kurKod));
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            while (dr.Read())
            {
                Proje gelen = new Proje();
                gelen.ad = VeriErisim.DegerAl(dr, 0, string.Empty);
                gelen.karakteristikOzet = VeriErisim.DegerAl(dr, 1, string.Empty);

                AltProje a = new AltProje();
                gelen.altProjeler.Add(a);
                ButceKaynak bk = new ButceKaynak();
                a.butceKaynaklari.Add(bk);

                ButceKaynakTeklif teklif = new ButceKaynakTeklif();
                teklif.Odenek0Toplam = VeriErisim.DegerAl(dr, 2, (double)0);
                bk.teklifler.Add(teklif);

                donusDeger.liste.Add(gelen);
            }

            if (!string.IsNullOrEmpty(ve.hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = ve.hata;
            }
            else if (donusDeger.liste.Count <= 0)
            {
                donusDeger.sonuc.islemSonuc = true;
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;
            }

            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Projelere ve tertiplere göre yatırım teklifleri dağılım raporunu üreten yordam
        /// Rapor; yatırım projeleri ve tertipler bazında ödenek teklif bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="sektor">Sektör kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN TertipDagilimListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string sektor, string kurKod)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, kurKod);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil <= 0)
                donusDeger.sonuc.hataStr = YatirimResI.YAT032 + "<br>";
            if (string.IsNullOrEmpty(senaryoKod))
                donusDeger.sonuc.hataStr += YatirimResI.YAT052;
            if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            {
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            try
            {
                donusDeger = ProjeClass.Listele(null, kullanan, yil, senaryoKod, new Proje(), kurKod, false);
                if (!donusDeger.sonuc.islemSonuc)
                    return donusDeger;
                TertipDagilimTertipBilgileriEkle(kullanan, yil, senaryoKod, donusDeger.liste);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.liste.Count <= 0)
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

            return donusDeger;
        }

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini
        /// yatırım projesi ve tertipleri bazında bulup projeler değişkenine ekleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="projeler">Tertip ve ödenek teklif bilgileri istenen yatırım projeleri</param>        
        private static void TertipDagilimTertipBilgileriEkle(Kullanici kullanan, int yil, string senaryoKod, TNSCollection projeler)
        {
            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            UcgenKodBilgileri kodBilgi = new UcgenKodBilgileri();
            kodBilgi.Add(new UcgenKodBilgi(1, tertip.Kirilimlar(0).Split('.').Length));
            kodBilgi.Add(new UcgenKodBilgi(2, tertip.Kirilimlar(1).Split('.').Length));
            kodBilgi.Add(new UcgenKodBilgi(3, tertip.Kirilimlar(2).Split('.').Length));
            kodBilgi.Add(new UcgenKodBilgi(4, tertip.Kirilimlar(3).Split('.').Length));
            kodBilgi.Add(new UcgenKodBilgi(14, tertip.Kirilimlar(4).Split('.').Length));

            EkDegerParam[] ekBilgi = new EkDegerParam[1];
            ekBilgi[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);

            try
            {
                foreach (Proje p in projeler)
                {
                    tertip.kodlar[0] = TNS.DEG.Arac.VarsayilanKurumBul();
                    tertip.kodlar[4] = PebKodVer(kullanan, yil, p.kod, senaryoKod);
                    if (string.IsNullOrEmpty(tertip.kodlar[4]))
                        continue;
                    TNS.BHMv2.OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, tertip, kodBilgi, ekBilgi, true);
                    if (odnler.Length <= 0)
                        continue;
                    p.odenekBilgiler.AddRange(odnler);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Yatırım teklifleri dağılım raporunu üreten yordam
        /// Rapor; birim, yatırım projesi ve tertip bazında yatırım projeleri ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorKod">Sektor kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN TeklifDagilimListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string sektorKod, string kurKod)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, kurKod);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil <= 0)
                donusDeger.sonuc.hataStr = YatirimResI.YAT032;
            if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            {
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            try
            {
                BilgiVAN projeler = ProjeClass.Listele(null, kullanan, yil, senaryoKod, null, kurKod, false);
                if (!projeler.sonuc.islemSonuc)
                    return projeler;
                donusDeger = TeklifDagilimTertipBilgileriEkle(kullanan, yil, projeler.liste, senaryoKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.liste.Count <= 0)
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

            return donusDeger;
        }

        /// <summary>
        /// Alt faaliyet-yatırım bağı üzerinden bütçe hazırlık ödenek tekliflerini
        /// birim, yatırım projesi ve tertip bazında döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="projeler">Birim, tertip ve ödenek teklif bilgileri istenen yatırım projeleri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında teklif dağılım nesneleri döndürülür.</returns>
        private static BilgiVAN TeklifDagilimTertipBilgileriEkle(Kullanici kullanan, int yil, TNSCollection projeler, string senaryoKod)
        {
            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            UcgenKodBilgileri kodBilgi = new UcgenKodBilgileri();
            kodBilgi.Add(new UcgenKodBilgi(1, tertip.Kirilimlar(0).Split('.').Length));
            kodBilgi.Add(new UcgenKodBilgi(2, tertip.Kirilimlar(1).Split('.').Length));
            kodBilgi.Add(new UcgenKodBilgi(3, tertip.Kirilimlar(2).Split('.').Length));
            kodBilgi.Add(new UcgenKodBilgi(4, tertip.Kirilimlar(3).Split('.').Length));
            kodBilgi.Add(new UcgenKodBilgi(14, tertip.Kirilimlar(4).Split('.').Length));

            EkDegerParam[] ekBilgi = new EkDegerParam[7];
            ekBilgi[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, -1, 0, "06.00");
            ekBilgi[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "02.00");
            ekBilgi[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "03.00");
            ekBilgi[3] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, "02.00");
            ekBilgi[4] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, "03.00");
            ekBilgi[5] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, "02.00");
            ekBilgi[6] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, "03.00");

            string kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
            BilgiVAN donusDeger = new BilgiVAN();
            try
            {
                foreach (Proje p in projeler)
                {
                    tertip.kodlar[0] = kurKod;
                    tertip.kodlar[4] = PebKodVer(kullanan, yil, p.kod, senaryoKod);
                    if (string.IsNullOrEmpty(tertip.kodlar[4]))
                        continue;
                    TNS.BHMv2.OdenekBilgi[] odnler = servisBHMv2.ListeTertipOl(kullanan, tertip, kodBilgi, ekBilgi, true);
                    if (odnler.Length <= 0)
                        continue;

                    foreach (TNS.BHMv2.OdenekBilgi odn in odnler)
                    {
                        TeklifDagilim teklifDagilim = null;
                        foreach (TeklifDagilim td in donusDeger.liste)
                        {
                            if (td.birim.kod == odn.tertip.kodlar[0])
                            {
                                teklifDagilim = td;
                                break;
                            }
                        }
                        if (teklifDagilim == null)
                        {
                            teklifDagilim = new TeklifDagilim();
                            teklifDagilim.birim.kod = odn.tertip.kodlar[0];
                            teklifDagilim.birim.AdAta(odn.tertip.adlar[0]);
                            donusDeger.liste.Add(teklifDagilim);
                        }

                        Proje proje = null;
                        foreach (Proje tdp in teklifDagilim.projeler)
                        {
                            if (tdp.kod == p.kod)
                            {
                                proje = tdp;
                                break;
                            }
                        }
                        if (proje == null)
                        {
                            proje = new Proje(p);
                            teklifDagilim.projeler.Add(proje);
                        }
                        proje.odenekBilgiler.Add(odn);
                    }
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                return donusDeger;
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen yıla ve yatırım projesine ait olan alt faaliyet kodlarını bulup
        /// virgül ayracı ile birleştirip döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="projeKod">Yatırım projesinin programatik kodu</param>
        /// <returns>Virgül ile ayrılmış alt faaliyetleri tutan bir string döndürülür.</returns>
        private static string PebKodVer(Kullanici kullanan, int yil, int projeKod, string senaryoKod)
        {
            return PebKodVer(kullanan, yil, projeKod, 0, senaryoKod);
        }

        /// <summary>
        /// Verilen yıla ve yatırım projesine veya yatırım alt projesine ait olan
        /// alt faaliyet kodlarını bulup virgül ayracı ile birleştirip döndüren yordam
        /// Eğer yatırım projesinin programatik kodu sıfırdan büyükse projenin,
        /// değilse alt projenin alt faaliyetleri döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="projeKod">Yatırım projesinin programatik kodu</param>
        /// <param name="altProjeKod">Yatırım alt projesinin programatik kodu</param>
        /// <returns>Virgül ile ayrılmış alt faaliyetleri tutan bir string döndürülür.</returns>
        private static string PebKodVer(Kullanici kullanan, int yil, int projeKod, int altProjeKod, string senaryoKod)
        {
            string kodlar = string.Empty;
            try
            {
                AltFaaliyet[] pebKodlar;
                if (projeKod > 0)
                    pebKodlar = servisBHMv2.YatBagPebKodlariListele(kullanan, yil, projeKod, senaryoKod);
                else
                {
                    AltFaaliyet p = new AltFaaliyet();
                    p.yil = yil;
                    p.altProje = altProjeKod;
                    pebKodlar = servisBHMv2.YatBagPebKodlariListele(kullanan, p, senaryoKod);
                }
                if (pebKodlar.Length <= 0)
                    return string.Empty;

                foreach (AltFaaliyet peb in pebKodlar)
                {
                    if (!string.IsNullOrEmpty(kodlar))
                        kodlar += ",";
                    kodlar += peb.pebKod;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return kodlar;
        }

        /// <summary>
        /// Birimlere ve sektörlere göre yatırım teklifleri dağılım raporunu üreten yordam
        /// Rapor; birim ve sektör bazında yatırım projeleri ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında teklif dağılım listesi döndürülür.</returns>
        public static BilgiVAN SektorDagilimListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string kurKod)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, kurKod);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil <= 0)
                donusDeger.sonuc.hataStr = YatirimResI.YAT032 + "<br>";
            if (string.IsNullOrEmpty(senaryoKod))
                donusDeger.sonuc.hataStr += YatirimResI.YAT052;
            if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            {
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynakTeklif tabTeklif = new TABYATButceKaynakTeklif(ve);
            IDataReader dr = null;

            try
            {
                dr = tabTeklif.ListeleToplam(0, yil, 0, senaryoKod, Yetki.BirimKodlariDondur(kullanan, kurKod), true, yil - 100, "", "", true, true);
                if (dr == null) throw new Exception(ve.hata);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                ve.VeriErisimKapat();
                return donusDeger;
            }

            string eskiBirimKod = null;
            string eskiSektor = null;
            int eskiYil = 0;
            TeklifDagilim teklifDagilimBirim = null;
            ButceKaynak bk = null;
            ButceKaynakTeklif bkh = null;
            while (dr.Read())
            {
                string gelenBirimKod = VeriErisim.DegerAl(dr, 0, string.Empty).Trim();
                string gelenSektor = VeriErisim.DegerAl(dr, 1, string.Empty).Trim();
                int gelenYil = VeriErisim.DegerAl(dr, 2, 0);
                if (eskiBirimKod != gelenBirimKod)
                {
                    teklifDagilimBirim = new TeklifDagilim();
                    Tertip t = new Tertip(yil, ENUMTabloKod.TABOdenek);
                    teklifDagilimBirim.birim.kod = OrtakFonksiyonlar.KodNoktaKoy(t.Kirilimlar(0), gelenBirimKod);
                    if (!string.IsNullOrEmpty(teklifDagilimBirim.birim.kod))
                        teklifDagilimBirim.birim.AdAta(YATServis.UzayYaziDegerGetir("ABSKUR", yil.ToString() + "-" + teklifDagilimBirim.birim.kod, true, string.Empty));
                    donusDeger.liste.Add(teklifDagilimBirim);
                    eskiSektor = null;
                    eskiBirimKod = gelenBirimKod;
                }
                if (eskiSektor != gelenSektor)
                {
                    Proje proje = new Proje();
                    proje.sektor.kod = gelenSektor;
                    if (!string.IsNullOrEmpty(proje.sektor.kod))
                    {
                        SektorSozluk s = new SektorSozluk();
                        proje.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy(s.kod, proje.sektor.kod);
                        proje.sektor.ad = YATServis.UzayYaziDegerGetir("YATSEKTOR", proje.sektor.kod, true, string.Empty);
                    }
                    AltProje altProje = new AltProje();
                    proje.altProjeler.Add(altProje);

                    bk = new ButceKaynak();
                    altProje.butceKaynaklari.Add(bk);
                    teklifDagilimBirim.projeler.Add(proje);
                    eskiYil = 0;
                    eskiSektor = gelenSektor;
                }
                if (eskiYil != gelenYil)
                {
                    bkh = new ButceKaynakTeklif();
                    bkh.butceYil = gelenYil;
                    bk.teklifler.Add(bkh);
                    eskiYil = gelenYil;
                }

                ENUMProjeMaliYapiKod indeks = (ENUMProjeMaliYapiKod)VeriErisim.DegerAl(dr, 3, 0);

                double kaynakToplam = VeriErisim.DegerAl(dr, 4, (double)0);
                double fin7 = VeriErisim.DegerAl(dr, 5, (double)0);
                double disOzkaynak = VeriErisim.DegerAl(dr, 6, (double)0);

                bkh.DegerAta(indeks, fin7, kaynakToplam, disOzkaynak);
            }
            dr.Close();

            if (!string.IsNullOrEmpty(ve.hata))
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = ve.hata;
            }
            else if (donusDeger.liste.Count <= 0)
            {
                donusDeger.sonuc.islemSonuc = true;
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;
            }

            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Gönderilen data readerdaki ödenek teklif bilgilerini eğer varsa birimler listesi içindeki ilgili
        /// TeklifDagilim nesnesinin ilgili yatırım projesi nesnesine ekleyen, yoksa yeni TeklifDagilim ve/veya yatırım projesi
        /// nesnesi oluşturup ödenek teklif bilgilerini bu nesneye, bu nesneyi de birimler listesine ekleyen yordam
        /// </summary>
        /// <param name="birimler">TeklifDagilim nesnelerinden oluşan listeyi tutan nesne</param>
        /// <param name="dr">Ödenek teklif bilgilerini tutan data reader nesnesi</param>
        private static void TeklifBilgileriEkle(ref TNSCollection birimler, IDataReader dr)
        {
            //??? bi bak
            //int aYil = Genel.AktifYilDondur();
            //Tertip tertip = new Tertip(aYil, ENUMTabloKod.TABOdenek);

            //TeklifDagilim gelen;
            //ButceKaynak bk;
            //ButceKaynakTeklif bkt;
            //while (dr.Read())
            //{
            //    gelen = null;
            //    bk = null;
            //    bkt = null;
            //    foreach (TeklifDagilim birim in birimler)
            //    {
            //        string kontrolBirimKodu = OrtakFonksiyonlar.KodNoktaKoy(tertip.Kirilimlar(0), VeriErisim.DegerAl(dr, 0, string.Empty));
            //        if (birim.birim.kod == kontrolBirimKodu)
            //        {
            //            gelen = birim;
            //            foreach (Proje proje in birim.projeler)
            //            {
            //                string kontrolSektorKodu = VeriErisim.DegerAl(dr, 1, string.Empty);
            //                SektorSozluk s = new SektorSozluk();
            //                kontrolSektorKodu = OrtakFonksiyonlar.KodNoktaKoy(s.kod, kontrolSektorKodu);
            //                if (proje.sektor.kod == kontrolSektorKodu)
            //                {
            //                    bk = (ButceKaynak)((AltProje)proje.altProjeler[0]).butceKaynaklari[0];
            //                    break;
            //                }
            //            }
            //            if (bk != null)
            //                break;
            //        }
            //    }
            //    if (gelen == null)
            //    {
            //        gelen = new TeklifDagilim();
            //        gelen.birim.kod = OrtakFonksiyonlar.KodNoktaKoy(tertip.Kirilimlar(0), VeriErisim.DegerAl(dr, 0, string.Empty));
            //        if (!string.IsNullOrEmpty(gelen.birim.kod))
            //            gelen.birim.AdAta(YATServis.UzayYaziDegerGetir("ABSKUR", aYil.ToString() + "-" + gelen.birim.kod, true, string.Empty));
            //        birimler.Add(gelen);
            //    }
            //    if (bk == null)
            //    {
            //        Proje proje = new Proje();
            //        proje.sektor.kod = VeriErisim.DegerAl(dr, 1, string.Empty);
            //        if (!string.IsNullOrEmpty(proje.sektor.kod))
            //        {
            //            SektorSozluk s = new SektorSozluk();
            //            proje.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy(s.kod, proje.sektor.kod);
            //            proje.sektor.ad = YATServis.UzayYaziDegerGetir("YATSEKTOR", proje.sektor.kod, true, string.Empty);
            //        }
            //        AltProje altProje = new AltProje();
            //        bk = new ButceKaynak();
            //        altProje.butceKaynaklari.Add(bk);
            //        proje.altProjeler.Add(altProje);
            //        gelen.projeler.Add(proje);
            //    }

            //    foreach (ButceKaynakTeklif t in bk.teklifler)
            //    {
            //        if (t.yil == VeriErisim.DegerAl(dr, 2, 0))
            //        {
            //            bkt = t;
            //            break;
            //        }
            //    }
            //    if (bkt == null)
            //    {
            //        bkt = new ButceKaynakTeklif();
            //        bkt.yil = VeriErisim.DegerAl(dr, 2, 0);
            //        bkt.yilButce.yil = bkt.yil;
            //        bk.teklifler.Add(bkt);
            //    }
            //    int indeks = VeriErisim.DegerAl(dr, 3, 0);
            //    bkt.yilButce.odenek[indeks] = VeriErisim.DegerAl(dr, 4, (double)0);
            //}
            //dr.Close();
        }

        /// <summary>
        /// Birimlerin yatırım projeleri ve ödenek teklifleri raporunu üreten yordam
        /// Rapor; istenen birime ait yatırım projelerini ve ödenek tekliflerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="birimKodu">Birim kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN BirimProjeleriListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string birimKodu, bool altProjelerDahil)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, birimKodu);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil <= 0)
                donusDeger.sonuc.hataStr = YatirimResI.YAT032;
            if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            {
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            TABYATButceKaynakTeklif tabTeklif = new TABYATButceKaynakTeklif(ve);
            IDataReader dr = tabTeklif.ListeleProjeToplam(0, yil, 0, senaryoKod, Yetki.BirimKodlariDondur(kullanan, null), true, yil - 100, birimKodu, null, altProjelerDahil);

            if (dr == null)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = ve.hata;
                ve.VeriErisimKapat();
                return donusDeger;
            }
            Proje proje = null;
            ButceKaynak bk = null;
            ButceKaynakTeklif bkh = null;
            int eskiProjeNo = -1;
            int eskiYil = -1;
            int eskiAltProjeNo = -1;
            int veriKolon = 6;
            if (altProjelerDahil)
                veriKolon = 9;
            while (dr.Read())
            {
                int gelenProjeNo = VeriErisim.DegerAl(dr, 0, 0);
                if (gelenProjeNo != eskiProjeNo)
                {
                    proje = new Proje();
                    proje.kod = gelenProjeNo;
                    proje.ad = VeriErisim.DegerAl(dr, 1, "");
                    proje.DPTProjeNo = VeriErisim.DegerAl(dr, 2, "");
                    proje.gerekce = VeriErisim.DegerAl(dr, 3, "");
                    proje.sektor.kod = VeriErisim.DegerAl(dr, 4, "");
                    proje.sektor.kod = OrtakFonksiyonlar.KodNoktaKoy("99.99", proje.sektor.kod);
                    proje.sektor.ad = YATServis.UzayYaziDegerGetir("YATSEKTOR", proje.sektor.kod, true, string.Empty);
                    proje.karakteristik = VeriErisim.DegerAl(dr, 5, "");
                    eskiProjeNo = gelenProjeNo;
                    if (!altProjelerDahil)
                    {
                        AltProje altProje = new AltProje();
                        proje.altProjeler.Add(altProje);
                        bk = new ButceKaynak();
                        altProje.butceKaynaklari.Add(bk);
                    }
                    eskiYil = 0;
                    eskiAltProjeNo = -1;
                    donusDeger.liste.Add(proje);
                }
                if (altProjelerDahil)
                {
                    int yeniAltProjeNo = VeriErisim.DegerAl(dr, 6, 0);
                    if (yeniAltProjeNo != eskiAltProjeNo)
                    {
                        AltProje altProje = new AltProje();
                        altProje.kod = yeniAltProjeNo;
                        altProje.birim.kod = VeriErisim.DegerAl(dr, 7, "");
                        altProje.ad = VeriErisim.DegerAl(dr, 8, "");
                        proje.altProjeler.Add(altProje);
                        bk = new ButceKaynak();
                        altProje.butceKaynaklari.Add(bk);
                        eskiAltProjeNo = yeniAltProjeNo;
                    }
                }
                int gelenYil = VeriErisim.DegerAl(dr, veriKolon, 0);
                if (eskiYil != gelenYil)
                {
                    bkh = new ButceKaynakTeklif();
                    bkh.butceYil = gelenYil;
                    bk.teklifler.Add(bkh);
                    eskiYil = gelenYil;
                }

                ENUMProjeMaliYapiKod indeks = (ENUMProjeMaliYapiKod)VeriErisim.DegerAl(dr, veriKolon + 1, 0);

                double kaynakToplam = VeriErisim.DegerAl(dr, veriKolon + 2, (double)0);
                double fin7 = VeriErisim.DegerAl(dr, veriKolon + 3, (double)0);
                double disOzkaynak = VeriErisim.DegerAl(dr, veriKolon + 4, (double)0);

                bkh.DegerAta(indeks, fin7, kaynakToplam, disOzkaynak);
            }
            dr.Close();
            ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Gönderilen data readerdaki ödenek teklif bilgilerini eğer varsa projeler listesi içindeki ilgili
        /// yatırım projesi nesnesine ekleyen, yoksa yeni yatırım projesi nesnesi oluşturup
        /// ödenek teklif bilgilerini bu nesneye, bu nesneyi de projeler listesine ekleyen yordam
        /// </summary>
        /// <param name="projeler">Proje nesnelerinden oluşan listeyi tutan nesne</param>
        /// <param name="dr">Ödenek teklif bilgilerini tutan data reader nesnesi</param>
        private static void BirimProjeleriTeklifBilgileriEkle(ref TNSCollection projeler, IDataReader dr)
        {
            //??? bi bak
            //int eskiKod = -2;
            //ButceKaynak bk;
            //while (dr.Read())
            //{
            //    bk = null;
            //    foreach (Proje proje in projeler)
            //    {
            //        eskiKod = VeriErisim.DegerAl(dr, 0, -1);
            //        if (proje.kod == eskiKod)
            //        {
            //            bk = (ButceKaynak)((AltProje)proje.altProjeler[0]).butceKaynaklari[0];
            //            break;
            //        }
            //    }
            //    if (bk == null)
            //    {
            //        Proje proje = new Proje();
            //        proje.kod = VeriErisim.DegerAl(dr, 0, -1);
            //        proje.ad = VeriErisim.DegerAl(dr, 1, string.Empty);
            //        proje.DPTProjeNo = VeriErisim.DegerAl(dr, 2, string.Empty);
            //        proje.gerekce = VeriErisim.DegerAl(dr, 3, string.Empty);
            //        projeler.Add(proje);

            //        AltProje altProje = new AltProje();
            //        proje.altProjeler.Add(altProje);
            //        bk = new ButceKaynak();
            //        altProje.butceKaynaklari.Add(bk);
            //    }
            //    ButceKaynakTeklif bkt = new ButceKaynakTeklif();
            //    bkt.yil = VeriErisim.DegerAl(dr, 4, 0);
            //    int indeks = VeriErisim.DegerAl(dr, 5, 0);
            //    bkt.projeButce[indeks] = VeriErisim.DegerAl(dr, 6, (double)0);
            //    bk.teklifler.Add(bkt);
            //}
            //dr.Close();
        }

        /// <summary>
        /// Konsolide yatırım bilgileri raporunu üreten yordam
        /// Rapor; yatırım projelerini, alt projelerini, proje maliyetini, ödenek tekliflerini ve harcamalarını içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN KonsolideYatirimListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string sektorNo, string kurKod)
        {
            return ProjeTakipListele(veriErisim, kullanan, yil, sektorNo, true, kurKod);
        }

        /// <summary>
        /// Yatırım projeleri, alt projeleri, proje maliyeti, ödenek teklifleri ve harcamaları döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="sektorNo">Sektör kriteri</param>
        /// <param name="konsolide">konsolide parametresi true ise yil ve yil-1 bilgileri, false ise bütün yıllara ait bilgiler döndürürlür.</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN ProjeTakipListele(VeriErisim veriErisim, Kullanici kullanan, int yil, string sektorNo, bool konsolide, string kurKod)
        {
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriYAT", TNS.KYM.Arac.KimlikBul(kullanan));
            BilgiVAN donusDeger = ProjeAltProjeListele(ve, kullanan, yil, "06.00", sektorNo, "H", -5, -1, null, true, 0, false, kurKod, -1, true, "", false);
            //if (!donusDeger.sonuc.islemSonuc)
            //{
            //    ve.VeriErisimKapat();
            //    return donusDeger;
            //}
            //if (donusDeger.liste.Count <= 0)
            //    return donusDeger;

            //BilgiVAN detay = null;
            //foreach (Proje p in donusDeger.liste)
            //{
            //    foreach (AltProje ap in p.altProjeler)
            //    {
            //        detay = ButceKaynakClass.Listele(ve, kullanan, ap.kod, 0, -5, "06.00", null, true);
            //        if (!detay.sonuc.islemSonuc)
            //            continue;
            //        if (!konsolide)
            //        {
            //            ap.butceKaynaklari.AddRange(detay.liste);
            //            continue;
            //        }
            //        foreach (ButceKaynak butceKaynak in detay.liste)
            //        {
            //            for (int i = 0; i < butceKaynak.teklifler.Count; i++)
            //            {
            //                ButceKaynakTeklif t = (ButceKaynakTeklif)butceKaynak.teklifler[i];
            //                if (t.yil != yil)
            //                {
            //                    butceKaynak.teklifler.RemoveAt(i);
            //                    i--;
            //                }
            //            }
            //            for (int i = 0; i < butceKaynak.harcamalar.Count; i++)
            //            {
            //                ButceKaynakHarcama h = (ButceKaynakHarcama)butceKaynak.harcamalar[i];
            //                if (h.yil != yil && h.yil != yil - 1)
            //                {
            //                    butceKaynak.harcamalar.RemoveAt(i);
            //                    i--;
            //                }
            //            }
            //            if (butceKaynak.teklifler.Count > 0 || butceKaynak.harcamalar.Count > 0)
            //                ap.butceKaynaklari.Add(butceKaynak);
            //        }
            //    }
            //}

            //ve.VeriErisimKapat();
            return donusDeger;
        }

        /// <summary>
        /// Proje takip raporunu üreten yordam
        /// Rapor; yatırım projesinin yıllar itibariyle maliyet, ödenek teklif ve harcama bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yatırım projeleri döndürülür.</returns>
        public static BilgiVAN ProjeTakipTablosu(VeriErisim veriErisim, Kullanici kullanan, int yil, string kurKod)
        {
            return ProjeTakipListele(veriErisim, kullanan, yil, string.Empty, false, kurKod);
        }

        /// <summary>
        /// DPT Vize Cetveli raporunu üreten yordam
        /// Rapor; sektörler ve tertipler bazında ödenek teklif bilgilerini içerir.
        /// Kullanıcı işleme yetkili değilse veya zorunlu kriterler gönderilmemişse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tertip">Tertip bilgilerini tutan nesne</param>
        /// <returns>Hata varsa BilgiVAN içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında DPTVizeBilgi nesneleri döndürülür.</returns>
        public static BilgiVAN DPTVizeCetveliRaporla(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, Tertip tertip)
        {
            BilgiVAN donusDeger = new BilgiVAN();

            donusDeger.sonuc = Yetki.OkumaHakkiVarMi(kullanan, tertip.kodlar[0]);
            if (!donusDeger.sonuc.islemSonuc)
                return donusDeger;

            if (yil <= 0)
                donusDeger.sonuc.hataStr = YatirimResI.YAT032 + "<br>";
            if (string.IsNullOrEmpty(senaryoKod))
                donusDeger.sonuc.hataStr += YatirimResI.YAT052;
            if (!string.IsNullOrEmpty(donusDeger.sonuc.hataStr))
            {
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }

            try
            {
                BilgiVAN prjler = ProjeClass.Listele(null, kullanan, yil, senaryoKod, new Proje(), tertip.kodlar[0], false);
                if (!prjler.sonuc.islemSonuc)
                    return prjler;

                OdenekBilgi[] odnler = DPTVizeCetveliOdenekListele(kullanan, yil, senaryoKod, tertip);

                foreach (Proje prj in prjler.liste)
                {
                    AltFaaliyet[] pebler = servisBHMv2.YatBagPebKodlariListele(kullanan, yil, prj.kod, senaryoKod);
                    if (pebler.Length <= 0)
                        continue;

                    foreach (AltFaaliyet peb in pebler)
                    {
                        foreach (OdenekBilgi odn in odnler)
                        {
                            if (peb.pebKod == odn.tertip.kodlar[4])
                            {
                                string tertipStr = odn.tertip.TertipStrOl();
                                tertipStr = tertipStr.Substring(0, tertipStr.LastIndexOf('-'));

                                DPTVizeBilgi dptVize = null;
                                foreach (DPTVizeBilgi o in donusDeger.liste)
                                {
                                    if (o.sektor.kod == prj.sektor.kod &&
                                        o.tertip.TertipStrOl() == tertipStr)
                                    {
                                        dptVize = o;
                                        break;
                                    }
                                }
                                if (dptVize == null)
                                {
                                    dptVize = new DPTVizeBilgi(yil);
                                    dptVize.sektor = prj.sektor;
                                    dptVize.tertip.TertipAta(tertipStr);
                                    dptVize.tertip.AdlariBul(servisUZY);
                                    donusDeger.liste.Add(dptVize);
                                }
                                for (int i = 0; i < dptVize.odenek.Length; i++)
                                    dptVize.odenek[i] += (double)odn.ekRakamlar[i];
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.Message;
                return donusDeger;
            }

            if (donusDeger.sonuc.islemSonuc && donusDeger.liste.Count <= 0)
                donusDeger.sonuc.bilgiStr = YatirimResI.YAT017;

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan bütçe hazırlık ödenek teklif bilgilerini tertip bazında döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tertip">Tertip bilgilerini tutan nesne</param>
        /// <returns>Tertip bazında 3 yıllık ödenek teklif bilgilerini tutan OdenekBilgi dizisi döndürülür.</returns>
        private static OdenekBilgi[] DPTVizeCetveliOdenekListele(Kullanici kullanan, int yil, string senaryoKod, Tertip tertip)
        {
            try
            {
                UcgenKodBilgileri kodBilgi = new UcgenKodBilgileri();
                kodBilgi.Add(new UcgenKodBilgi(1, tertip.Kirilimlar(0).Split('.').Length));
                kodBilgi.Add(new UcgenKodBilgi(2, tertip.Kirilimlar(1).Split('.').Length));
                kodBilgi.Add(new UcgenKodBilgi(3, tertip.Kirilimlar(2).Split('.').Length));
                kodBilgi.Add(new UcgenKodBilgi(4, tertip.Kirilimlar(3).Split('.').Length));
                kodBilgi.Add(new UcgenKodBilgi(14, tertip.Kirilimlar(4).Split('.').Length));

                EkDegerParam[] ekBilgi = new EkDegerParam[3];
                ekBilgi[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
                ekBilgi[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, senaryoKod);
                ekBilgi[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, senaryoKod);

                if (string.IsNullOrEmpty(tertip.kodlar[3]))
                    tertip.kodlar[3] = "06,07,08";

                return servisBHMv2.ListeTertipOl(kullanan, tertip, kodBilgi, ekBilgi, true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
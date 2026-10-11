using System;
using System.Data;
using TNS.KYM;
using TNS.Veri;
using TNS.BKD;
using TNS.PEB.IsKurallari.Veri;
using System.Collections;
using TNS.BHMv2;

namespace TNS.PEB.IsKurallari
{
    /// <summary>
    /// Faaliyet/proje ödenek bilgileriyle ilgili, iş kuralları kontrollerini yapıp,
    /// veri tabanı bağlantısını alıp, veri tabanı fonksiyonlarını çağırarak
    /// okuma işlemlerini gerçekleştiren fonksiyonları tutan sınıf
    /// </summary>
    public class PEBFaaliyetProjeOdenek
    {
        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje ödenek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="fpOdenek">Faaliyet/proje ödenek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje ödenek listesi döndürülür.</returns>
        public static ObjectArray Listele(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProjeOdenek fpOdenek)
        {
            ObjectArray fpOdenekVAN = new ObjectArray();
            fpOdenekVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, fpOdenek.kurKod);
            if (!fpOdenekVAN.sonuc.islemSonuc)
                return fpOdenekVAN;

            string hata = string.Empty;
            if (string.IsNullOrEmpty(fpOdenek.kurKod))
                hata += PerformansButceResI.PEB005 + "<br>";
            if (fpOdenek.fpKod <= 0)
                hata += PerformansButceResI.PEB047 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                fpOdenekVAN.sonuc.islemSonuc = false;
                fpOdenekVAN.sonuc.hataStr = hata;
                return fpOdenekVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenek tOdenek = new TABOdenek(ve);

            IDataReader dr = tOdenek.Listele(fpOdenek.yil, fpOdenek.kurKod, fpOdenek.senaryoKod, fpOdenek.fpKod, (int)ENUMOdenekTur.FaaliyetProje);
            while (dr.Read())
            {
                FaaliyetProjeOdenek gelen = new FaaliyetProjeOdenek();
                gelen.yil = VeriErisim.DegerAl(dr, 0, 0);
                gelen.kurKod = VeriErisim.DegerAl(dr, 1, string.Empty);
                gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
                gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");
                gelen.senaryoKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                gelen.fpKod = VeriErisim.DegerAl(dr, 3, 0);

                string kurKod = VeriErisim.DegerAl(dr, 5, string.Empty);
                kurKod = PEBGenel.KurKodNoktala(kurKod);

                string fonKod = VeriErisim.DegerAl(dr, 6, string.Empty);
                fonKod = PEBGenel.FonKodNoktala(fonKod);

                string finKod = VeriErisim.DegerAl(dr, 7, string.Empty);

                string ekoKod = VeriErisim.DegerAl(dr, 8, string.Empty);
                ekoKod = PEBGenel.EkoKodNoktala(ekoKod);

                gelen.tertip.TertipAta(kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod);

                gelen.odenek = VeriErisim.DegerAl(dr, 9, (double)0);

                gelen.fpFonKod = VeriErisim.DegerAl(dr, 10, string.Empty);
                gelen.fpFonKod = PEBGenel.FonKodNoktala(gelen.fpFonKod);

                fpOdenekVAN.objeler.Add(gelen);
            }
            dr.Close();

            if (fpOdenekVAN.objeler.Count == 0)
                fpOdenekVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return fpOdenekVAN;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan faaliyet/proje ödenek bilgilerini döndüren yordam
        /// Ödenekler e-butce sisteminin istediği yapıda yani ekonomik kodun ilk kırılımı bazında toplanıyor.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">ENUMFaaliyetProjeTur listesindeki türlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında faaliyet/proje ödenek listesi döndürülür.</returns>
        public static ObjectArray FaaliyetProjeOdenekleriEbutce(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, int tur)
        {
            ObjectArray fpVAN = new ObjectArray();

            string hata = string.Empty;
            if (yil <= 0)
                hata += PerformansButceResI.PEB039 + "<br>";
            if (string.IsNullOrEmpty(senaryoKod))
                hata += PerformansButceResI.PEB045 + "<br>";
            if (!string.IsNullOrEmpty(hata))
            {
                fpVAN.sonuc.islemSonuc = false;
                fpVAN.sonuc.hataStr = hata;
                return fpVAN;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB", TNS.KYM.Arac.KimlikBul(kullanan));
            TABOdenek tOdenek = new TABOdenek(ve);

            int indeks = 0;
            if (tur <= (int)ENUMFaaliyetProjeTur.MerkezTasra)
                indeks = 3;

            FaaliyetProje gelen = null;
            IDataReader dr = tOdenek.FaaliyetProjeOdenekleriEbutce(yil, senaryoKod, tur);
            while (dr.Read())
            {
                if (gelen == null ||
                    (tur > (int)ENUMFaaliyetProjeTur.MerkezTasra && gelen.kurKod != VeriErisim.DegerAl(dr, 0, string.Empty)) ||
                    (tur <= (int)ENUMFaaliyetProjeTur.MerkezTasra && (gelen.kurKod != VeriErisim.DegerAl(dr, 0, string.Empty) || gelen.fpKod != VeriErisim.DegerAl(dr, 1, 0))))
                {
                    gelen = new FaaliyetProje();
                    gelen.yil = yil;
                    gelen.kurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
                    if (tur <= (int)ENUMFaaliyetProjeTur.MerkezTasra)
                    {
                        gelen.fpKod = VeriErisim.DegerAl(dr, 1, 0);
                        gelen.ebutceKod = VeriErisim.DegerAl(dr, 2, string.Empty);
                    }
                    fpVAN.objeler.Add(gelen);
                }

                TertipOdenek odenek = new TertipOdenek(yil);
                odenek.tertip.kodlar[(int)ENUMAbsKodSinif.KURKOD - 1] = VeriErisim.DegerAl(dr, indeks, string.Empty);
                odenek.tertip.kodlar[(int)ENUMAbsKodSinif.EKOKOD - 1] = VeriErisim.DegerAl(dr, indeks + 1, string.Empty);
                odenek.odenek = VeriErisim.DegerAl(dr, indeks + 2, (double)0);

                gelen.odenekler.Add(odenek);
            }
            dr.Close();

            if (tur <= (int)ENUMFaaliyetProjeTur.MerkezTasra)
            {
                TNSCollection maliyetDonusumluFPler = new TNSCollection();
                foreach (FaaliyetProje fp in fpVAN.objeler)
                {
                    foreach (TertipOdenek to in fp.odenekler)
                    {
                        FaaliyetProje bulunan = null;
                        foreach (FaaliyetProje mdfp in maliyetDonusumluFPler)
                        {
                            if (mdfp.yil == fp.yil &&
                                mdfp.kurKod == to.tertip.kodlar[(int)ENUMAbsKodSinif.KURKOD - 1] &&
                                mdfp.fpKod == fp.fpKod)
                            {
                                bulunan = mdfp;
                                break;
                            }
                        }

                        if (bulunan == null)
                        {
                            bulunan = new FaaliyetProje();
                            bulunan.yil = fp.yil;
                            bulunan.kurKod = to.tertip.kodlar[(int)ENUMAbsKodSinif.KURKOD - 1];
                            bulunan.fpKod = fp.fpKod;
                            bulunan.ebutceKod = fp.ebutceKod;

                            maliyetDonusumluFPler.Add(bulunan);
                        }
                        bulunan.odenekler.Add(to);
                    }
                }
                fpVAN.objeler = maliyetDonusumluFPler;
            }

            if (fpVAN.objeler.Count == 0)
                fpVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

            ve.VeriErisimKapat();
            return fpVAN;
        }

        //public static ObjectArray FarkliTertipListele(VeriErisim veriErisim, Kullanici kullanan, FaaliyetProjeOdenek fpOdenek)
        //{
        //    ObjectArray fpOdenekVAN = new ObjectArray();
        //    fpOdenekVAN.sonuc = Yetki.OkumaHakkiVarMi(kullanan, fpOdenek.kurKod);
        //    if (!fpOdenekVAN.sonuc.islemSonuc)
        //        return fpOdenekVAN;

        //    string hata = string.Empty;

        //    //VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriPEB");
        //    VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, PEBGenel.VeriErisimAdBulBHMv2(fpOdenek.yil));
        //    TABOdenek tOdenek = new TABOdenek(ve);

        //    IDataReader dr = tOdenek.FarkliTertipDüzenListele(fpOdenek.yil, fpOdenek.tertip.kodlar[0]);
        //    while (dr.Read())
        //    {
        //        FaaliyetProjeOdenek gelen = new FaaliyetProjeOdenek();
        //        gelen.kurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
        //        gelen.kurKod = PEBGenel.KurKodNoktala(gelen.kurKod);
        //        gelen.kurAd = PEBServis.UzayYaziDegerGetir("ABSKUR", gelen.yil.ToString() + "-" + gelen.kurKod, true, "");

        //        string kurKod = VeriErisim.DegerAl(dr, 0, string.Empty);
        //        kurKod = PEBGenel.KurKodNoktala(kurKod);

        //        string fonKod = VeriErisim.DegerAl(dr, 1, string.Empty);
        //        fonKod = PEBGenel.FonKodNoktala(fonKod);

        //        string finKod = VeriErisim.DegerAl(dr, 2, string.Empty);

        //        string ekoKod = VeriErisim.DegerAl(dr, 3, string.Empty);
        //        ekoKod = PEBGenel.EkoKodNoktala(ekoKod);

        //        gelen.tertip.TertipAta(kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod);

        //        gelen.fpFonKod = VeriErisim.DegerAl(dr, 4, string.Empty);
        //        gelen.fpFonKod = PEBGenel.FonKodNoktala(gelen.fpFonKod);

        //        // gelen.fpAd = PEBServis.UzayYaziDegerGetir("PEBFAALIYETPROJE", fpOdenek.yil.ToString() + "-" + kurKod + "-6-" + gelen.fpKod.ToString(), true, "");

        //        fpOdenekVAN.objeler.Add(gelen);
        //    }
        //    dr.Close();

        //    if (fpOdenekVAN.objeler.Count == 0)
        //        fpOdenekVAN.sonuc.bilgiStr = PerformansButceResI.PEB017;

        //    ve.VeriErisimKapat();
        //    return fpOdenekVAN;
        //}

        internal static BHMv2.UcgenSatir[] OdenekListeleFGT(VeriErisim veriErisim, Kullanici kullanan, int yil, string senaryoKod, string kurKod, int ekoUz, bool sifirlarDahil)
        {
            int strUz = 0;
            string ekoAlan = "O.EKOKOD";
            string groupBy;
            if (ekoUz == 0)
            {
                ekoAlan = "''";
                groupBy = "";
            }
            else
            {
                if (ekoUz == 1) strUz = 2;
                else if (ekoUz == 2) strUz = 3;
                else if (ekoUz == 3) strUz = 4;
                else strUz = 6;
                ekoAlan = "SUBSTRING(O.EKOKOD, 1, " + strUz.ToString() + ")";
                groupBy = " GROUP BY " + ekoAlan;
            }

            string sqlFaal = "SELECT " + ekoAlan + ", SUM(O.ODENEK) FROM ButceHazirlikV2.dbo.BHMODENEK AS O LEFT OUTER JOIN";
            sqlFaal += " PEBFAALIYETPROJEPEBFON ON O.YIL = PEBFAALIYETPROJEPEBFON.YIL AND O.PEBKOD = PEBFAALIYETPROJEPEBFON.PEBKOD INNER JOIN";
            sqlFaal += " PEBFAALIYETPROJE ON PEBFAALIYETPROJEPEBFON.YIL = PEBFAALIYETPROJE.YIL AND PEBFAALIYETPROJEPEBFON.MALIYETKURKOD = PEBFAALIYETPROJE.KURKOD AND ";
            sqlFaal += " PEBFAALIYETPROJEPEBFON.MALIYETKOD = PEBFAALIYETPROJE.FPKOD";
            sqlFaal += " WHERE O.TEKLIFYIL = 0 ";
            sqlFaal += " and O.YIL = " + VeriErisim.DegerYap(yil);
            sqlFaal += " and O.KURKOD LIKE " + VeriErisim.DegerYap(kurKod.Replace(".", "") + "%");
            sqlFaal += " and O.SENARYOKOD = " + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sqlFaal += " {0}";
            sqlFaal += " {1}";
            sqlFaal += " order by 1";

            string sqlSabit = "SELECT " + ekoAlan + ", SUM(O.ODENEK)";
            sqlSabit += " FROM ButceHazirlikV2.dbo.BHMODENEK AS O INNER JOIN";
            sqlSabit += " PEBFAALIYETPROJEPEBFON ON O.YIL = PEBFAALIYETPROJEPEBFON.YIL AND O.PEBKOD = PEBFAALIYETPROJEPEBFON.PEBKOD INNER JOIN";
            sqlSabit += " PEBSABITMALIYET ON PEBFAALIYETPROJEPEBFON.YIL = PEBSABITMALIYET.YIL AND PEBFAALIYETPROJEPEBFON.MALIYETKURKOD = PEBSABITMALIYET.SMKURKOD AND ";
            sqlSabit += " PEBFAALIYETPROJEPEBFON.MALIYETKOD = PEBSABITMALIYET.SMKOD";
            sqlSabit += " WHERE O.TEKLIFYIL = 0 AND O.YIL = " + VeriErisim.DegerYap(yil) + " AND O.KURKOD LIKE " + VeriErisim.DegerYap(kurKod.Replace(".", "") + "%") + " AND O.SENARYOKOD = " + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sqlSabit += " {0}";
            sqlSabit += " ORDER BY 1";
            string[] sqller = new string[4];
            string[] baglar = new string[4];

            sqller[0] = string.Format(sqlFaal, " and PEBFAALIYETPROJE.TUR=5", groupBy);
            sqller[1] = string.Format(sqlFaal, " and (PEBFAALIYETPROJE.TUR<>5 OR PEBFAALIYETPROJE.TUR IS NULL)", groupBy);
            sqller[2] = string.Format(sqlSabit, groupBy);
            if (strUz > 0)
                sqller[3] = "SELECT EKOKOD, EKOAD FROM ABSEKO WHERE YIL=" + VeriErisim.DegerYap(yil) + " AND LEN(EKOKOD)=" + VeriErisim.DegerYap(strUz);
            else
                sqller[3] = null;
            baglar[0] = baglar[1] = baglar[2] = "BaglantiSatiriPEB";
            baglar[3] = "BaglantiSatiriBHM";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();
            SortedList ht = servis.SorguBirlestir(sqller, baglar, new int[] { 0 });
            ObjectArray donusDeger = new ObjectArray();
            double rTransfer = 0;
            double rFaal = 0;
            double rSm = 0;

            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                OdenekBilgi ob = new OdenekBilgi(yil, ENUMTabloKod.TABPEBOdenek);

                ob.tertip.kodlar[0] = "";
                ob.tertip.kodlar[1] = "";
                ob.tertip.kodlar[2] = "";
                ob.tertip.kodlar[3] = OrtakFonksiyonlar.KodNoktaKoy(ob.tertip.Kirilimlar(3), alAnahtar.StrAl(0));

                ob.tertip.adlar[3] = alVeri.StrAl(3);
                rTransfer = alVeri.DblAl(0);
                rFaal = alVeri.DblAl(1);
                rSm = alVeri.DblAl(2);
                if ((rTransfer > 0 || rFaal > 0 || rSm > 0) || sifirlarDahil)
                {
                    ob.ekRakamlar.Add(rFaal);
                    ob.ekRakamlar.Add(rSm);
                    ob.ekRakamlar.Add(rTransfer);
                    donusDeger.Ekle(ob);
                }
            }
            if (ekoUz > 0)
            {
                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(4, ekoUz));
                return TNS.BHMv2.Arac.UcgenTertipOl(kullanan, kodBilgileri, true, (OdenekBilgi[])donusDeger.objeler.ToArray(typeof(OdenekBilgi)), yil);
            }
            else
            {
                BHMv2.UcgenSatir[] usD = new UcgenSatir[1];
                BHMv2.UcgenSatir us = new UcgenSatir();
                us.ekDegerler.Add(rFaal);
                us.ekDegerler.Add(rSm);
                us.ekDegerler.Add(rTransfer);
                usD[0] = us;
                return usD;
            }
        }
    }
}
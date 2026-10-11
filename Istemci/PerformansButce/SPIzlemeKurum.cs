using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OrtakClass;
using TNS;
using TNS.BKD;
using TNS.KYM;
using TNS.PEB;
using TNS.UZY;

// Eski PerformansButce\SPIzlemeKurum.aspx.cs dosyasından alındı: kurum izleme raporlarının (Excel; şablonlar
// RaporSablon\PEB\SPKurumIzleme*.xltx) verisini hazırlayan ve yazan durağan yordamlar. Sayfanın kendisi taşınmadı.
// Tek fark: üretilen dosya yordamın içinden tarayıcıya gönderilmez, yolu döndürülür (gönderme işini uç yapar).
// Sayfanın uçlarının (StratejikPlan\KurumIzlemeUclari.cs) çağırdığı dört yordam (MisyonBul, GostergeGelisimBul, BMHedefOl, MetinOku)
// public yapıldı; gövdeleri aynıdır.
namespace PerformansButce
{
    public static class SPIzlemeKurum
    {
        public class IzlemeKurum
        {
            public int hedefKod { get; set; }

            public string hedefKisa { get; set; }

            public string hedefAd { get; set; }

            public int gostergeSay { get; set; }

            public double hedefOran0 { get; set; }
            public double hedefOran1 { get; set; }
            public double hedefOran2 { get; set; }
            public double hedefOran3 { get; set; }
            public double hedefOran4 { get; set; }
            public double hedefOran
            {
                get
                {
                    if (aktifOranNo == 0) return hedefOran0;
                    if (aktifOranNo == 1) return hedefOran1;
                    if (aktifOranNo == 2) return hedefOran2;
                    if (aktifOranNo == 3) return hedefOran3;
                    return hedefOran4;
                }
            }
            public int basariNotu
            {
                get
                {
                    return Ortak.SPNotBul(hedefOran);
                }
            }

            public int aktifOranNo { get; set; }

            public string aciklama { get; set; }
        }

        public static List<IzlemeKurum> IzlemeGenelOlustur(Kullanici kullanan, IPEBServis servisPEB, int yil, int donem, string kurum, int basariAlt, int basariUst, out Misyon ms, out int[,] basariSay)
        {
            basariSay = new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
            ms = MisyonBul(kullanan, servisPEB, kurum, yil);
            List<IzlemeKurum> liste = new List<IzlemeKurum>();
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            string[] sqller = new string[6];
            string[] baglantilar = new string[6];
            sqller[0] = KurumHedefleri(ms.donemBYil, kurum);
            baglantilar[0] = "BaglantiSatiriSTR";
            for (int i = ms.aralikBasiYil; i <= ms.aralikSonuYil; i++)
            {
                int fark = (i - ms.aralikBasiYil);
                if (fark > 4) break;
                sqller[1 + fark] = KurumlaraGoreBasariRaporla(ms.donemBYil, i, donem, kurum);
                baglantilar[1 + fark] = "BaglantiSatiriSTR";
                //grdGosterge.ColumnModel.SetColumnHeader(5 + fark, i.ToString() + "<br>Başarım (%)");
                //grdGosterge.ColumnModel.SetHidden(5 + fark, i > yil);
            }

            SortedList sl = uzyServis.SorguBirlestir(sqller, baglantilar, new int[] { 0 });
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                IzlemeKurum okunan = new IzlemeKurum();

                okunan.hedefKod = alAnahtar.IntAl(0);
                okunan.hedefKisa = alVeri.StrAl(0);
                okunan.hedefAd = alVeri.StrAl(1);
                okunan.gostergeSay = alVeri.IntAl(2);
                okunan.hedefOran0 = alVeri.DblAl(3);
                okunan.hedefOran1 = alVeri.DblAl(5);
                okunan.hedefOran2 = alVeri.DblAl(7);
                okunan.hedefOran3 = alVeri.DblAl(9);
                okunan.hedefOran4 = alVeri.DblAl(11);

                for (int i = 0; i < 5; i++)
                {
                    okunan.aktifOranNo = i;
                    basariSay[i, okunan.basariNotu - 1]++;
                }
                okunan.aktifOranNo = yil - ms.aralikBasiYil;

                listeyeEkle(kullanan, liste, ms, okunan, basariAlt, basariUst);
            }
            return liste;
        }

        private static void listeyeEkle(Kullanici kullanan, List<IzlemeKurum> liste, Misyon ms, IzlemeKurum okunan, int basariAlt, int basariUst)
        {
            if (SPIzlemeGenel.AraliktaMi(okunan.hedefOran, basariAlt, basariUst))
            {
                okunan.aciklama = MetinOku(kullanan, ms.donemBYil, ms.kurumKod, okunan.hedefKod);
                liste.Add(okunan);
            }
        }

        public static Misyon MisyonBul(Kullanici kullanan, IPEBServis servisPEB, string kurum, int yil)
        {
            Misyon ms = null;

            if (yil > 0 && !string.IsNullOrWhiteSpace(kurum))
            {
                Misyon misyon = new Misyon();
                misyon.izlemeYil = yil;
                misyon.kurumKod = kurum;
                ObjectArray bilgiler = servisPEB.MisyonListele(kullanan, misyon);

                if (bilgiler.sonuc.islemSonuc && bilgiler.objeler.Count > 0)
                {
                    ms = (TNS.PEB.Misyon)bilgiler.objeler[0];
                }
            }

            if (ms == null)
            {
                ms = new Misyon();
                ms.kurumKod = kurum;
                ms.donemBYil = yil;
                ms.aralikBasiYil = yil;
                ms.aralikSonuYil = ms.aralikBasiYil + 4;
            }
            return ms;
        }

        public static string YazdirHedef(Kullanici kullanan, IPEBServis servisPEB, int yil, string kurumKod, int gDonem, int basAlt, int basUst, string raporSablonYol)
        {
            Misyon ms = null;
            int[,] basariSay = null;
            List<IzlemeKurum> liste = IzlemeGenelOlustur(kullanan, servisPEB, yil, gDonem, kurumKod, basAlt, basUst, out ms, out basariSay);

            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosya = DosyaIslem.DosyaAdUret("SPKurumIzleme" + kurumKod.Replace(".", ""), "") + ".xlsx";
            XLS.DosyaAc(raporSablonYol + "\\PEB\\" + "SPKurumIzleme.xltx", sonucDosya);

            int sayfaNo = 0, satir1 = 0, sutun1 = 0, satir2 = 0, sutun2 = 0;
            int baslaSatir = 0, baslaSutun = 0;
            int yer = yil - ms.aralikBasiYil;
            XLS.HucreAdAdresCoz("FormatSatir", ref sayfaNo, ref satir1, ref sutun1, ref satir2, ref sutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("donemBYil", ms.aralikBasiYil);
            int yuks = XLS.SatirYukseklikAl(baslaSatir);
            int satir = baslaSatir;
            foreach (IzlemeKurum item in liste)
            {
                int kopyaSatirNo = item.basariNotu;
                if (kopyaSatirNo < 1) kopyaSatirNo = 1;
                else if (kopyaSatirNo > 4) kopyaSatirNo = 4;
                kopyaSatirNo--;
                XLS.SatirAc(0, satir, 1);
                XLS.HucreKopyala(satir1 + kopyaSatirNo, sutun1, satir2 + kopyaSatirNo, sutun2, satir, baslaSutun);
                XLS.HucreDegerYaz(satir, baslaSutun + 0, item.hedefKisa);
                XLS.HucreDegerYaz(satir, baslaSutun + 1, item.hedefAd);
                XLS.HucreDegerYaz(satir, baslaSutun + 2, item.hedefOran0);
                XLS.HucreDegerYaz(satir, baslaSutun + 3, item.hedefOran1);
                XLS.HucreDegerYaz(satir, baslaSutun + 4, item.hedefOran2);
                XLS.HucreDegerYaz(satir, baslaSutun + 5, item.hedefOran3);
                XLS.HucreDegerYaz(satir, baslaSutun + 6, item.hedefOran4);
                XLS.HucreDegerYaz(satir, baslaSutun + 7, item.aciklama);
                XLS.SatirYukseklikAyarla(satir, satir, -1, 7, yuks);

                satir++;
            }
            XLS.HucreAdBulYaz("basari0", basariSay[yer, 0]);
            XLS.HucreAdBulYaz("basari1", basariSay[yer, 1]);
            XLS.HucreAdBulYaz("basari2", basariSay[yer, 2]);
            XLS.HucreAdBulYaz("basari3", basariSay[yer, 3]);
            int seySayfa = 0;
            int seySat1 = 0;
            int seySat2 = 0;
            int seySut1 = 0;
            int seySut2 = 0;
            XLS.HucreAdAdresCoz("SeyirVeriBasla", ref seySayfa, ref seySat1, ref seySut1, ref seySat2, ref seySut2);
            int asEski = XLS.AktifSheet();
            XLS.AktifSheetDegistir(seySayfa);
            int farkYil = yil - ms.aralikBasiYil;
            if (farkYil < 0) farkYil = 0;
            else if (farkYil > 4) farkYil = 4;
            farkYil++;
            for (int i = 0; i < 5; i++)
            {
                XLS.HucreDegerYaz(seySat1 + i, seySut1, ms.aralikBasiYil + i);
                XLS.HucreDegerYaz(seySat1 + i, seySut1 + 1, basariSay[yer, 0]);
                XLS.HucreDegerYaz(seySat1 + i, seySut1 + 2, basariSay[yer, 1]);
                XLS.HucreDegerYaz(seySat1 + i, seySut1 + 3, basariSay[yer, 2]);
                XLS.HucreDegerYaz(seySat1 + i, seySut1 + 4, basariSay[yer, 3]);
            }
            if (farkYil < 4)
                XLS.SatirSil(seySat1 + farkYil, seySat1 + 4);

            XLS.AktifSheetDegistir(asEski);
            if (farkYil < 4)
                XLS.SutunGenislikAyarla(baslaSutun + farkYil + 2, baslaSutun + 6, 0);
            XLS.DosyaSaklaTamYol();

            return sonucDosya;
        }

        public static string YazdirHedefGost(Kullanici kullanan, IPEBServis servisPEB, int yil, string kurumKod, int gDonem, int basAlt, int basUst, string raporSablonYol)
        {
            Misyon ms = null;
            int[,] basariSay = null;
            List<IzlemeKurum> liste = IzlemeGenelOlustur(kullanan, servisPEB, yil, gDonem, kurumKod, basAlt, basUst, out ms, out basariSay);

            Tablo XLS = GenelIslemler.NewTablo();

            string sonucDosya = DosyaIslem.DosyaAdUret("SPKurumIzlemeGost" + kurumKod.Replace(".", ""), "") + ".xlsx";
            XLS.DosyaAc(raporSablonYol + "\\PEB\\" + "SPKurumIzlemeGost.xltx", sonucDosya);

            int sayfaNo = 0, satir1 = 0, sutun1 = 0, satir2 = 0, sutun2 = 0;
            int baslaSatir = 0, baslaSutun = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sayfaNo, ref satir1, ref sutun1, ref satir2, ref sutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("donemBYil", ms.aralikBasiYil);
            int yuks = XLS.SatirYukseklikAl(baslaSatir);
            int satir = baslaSatir;
            foreach (IzlemeKurum item in liste)
            {
                int kopyaSatirNo = item.basariNotu;
                if (kopyaSatirNo < 1) kopyaSatirNo = 1;
                else if (kopyaSatirNo > 4) kopyaSatirNo = 4;
                kopyaSatirNo--;
                XLS.HucreKopyala(satir1 + kopyaSatirNo, sutun1, satir2 + kopyaSatirNo, sutun2, satir, baslaSutun);
                XLS.HucreDegerYaz(satir, baslaSutun + 0, item.hedefKisa);
                XLS.HucreDegerYaz(satir, baslaSutun + 1, item.hedefAd);
                XLS.HucreDegerYaz(satir, baslaSutun + 4, item.hedefOran0 / 100);
                XLS.HucreDegerYaz(satir, baslaSutun + 8, item.hedefOran1 / 100);
                XLS.HucreDegerYaz(satir, baslaSutun + 12, item.hedefOran2 / 100);
                XLS.HucreDegerYaz(satir, baslaSutun + 16, item.hedefOran3 / 100);
                XLS.HucreDegerYaz(satir, baslaSutun + 20, item.hedefOran4 / 100);
                XLS.HucreDegerYaz(satir, baslaSutun + 24, item.aciklama);
                XLS.SatirYukseklikAyarla(satir, satir, -1, 7, Convert.ToInt32(yuks * 1.6));

                satir++;
                ArrayList al = GostergeGelisimBul(item.hedefKod, ms, gDonem, yil);
                foreach (StrHedefGosterge gos in al)
                {
                    int tut = Ortak.SPNotBul(gos.GostergeOranBul);
                    XLS.HucreKopyala(satir1 + 4 + tut, sutun1, satir2 + 4 + tut, sutun2, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, gos.gorKod);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, gos.gostergeAd);
                    XLS.HucreDegerYaz(satir, baslaSutun + 2, gos.hedefeEtkisi / 100);
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, gos.donemBaslangic);

                    XLS.HucreDegerYaz(satir, baslaSutun + 5, gos.hedeflenenBirikimliDeger);
                    XLS.HucreDegerYaz(satir, baslaSutun + 6, gos.gerceklesen0);
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, gos.gostOran0 / 100);

                    XLS.HucreDegerYaz(satir, baslaSutun + 9, gos.hedeflenenBirikimliDeger);
                    XLS.HucreDegerYaz(satir, baslaSutun + 10, gos.gerceklesen1);
                    XLS.HucreDegerYaz(satir, baslaSutun + 11, gos.gostOran1 / 100);

                    XLS.HucreDegerYaz(satir, baslaSutun + 13, gos.hedeflenenBirikimliDeger);
                    XLS.HucreDegerYaz(satir, baslaSutun + 14, gos.gerceklesen2);
                    XLS.HucreDegerYaz(satir, baslaSutun + 15, gos.gostOran2 / 100);

                    XLS.HucreDegerYaz(satir, baslaSutun + 17, gos.hedeflenenBirikimliDeger);
                    XLS.HucreDegerYaz(satir, baslaSutun + 18, gos.gerceklesen3);
                    XLS.HucreDegerYaz(satir, baslaSutun + 19, gos.gostOran3 / 100);

                    XLS.HucreDegerYaz(satir, baslaSutun + 21, gos.hedeflenenBirikimliDeger);
                    XLS.HucreDegerYaz(satir, baslaSutun + 22, gos.gerceklesen4);
                    XLS.HucreDegerYaz(satir, baslaSutun + 23, gos.gostOran4 / 100);

                    XLS.HucreDegerYaz(satir, baslaSutun + 24, gos.AciklamaOl(gDonem));

                    XLS.SatirYukseklikAyarla(satir, satir, -1, 7, yuks);
                    satir++;
                }
            }
            XLS.HucreKopyala(satir1 + 10, sutun1, satir2 + 10, sutun2, satir, baslaSutun);
            int farkYil = yil - ms.aralikBasiYil;
            XLS.SutunGizle(sutun1 + 4, sutun1 + 4, true);
            YilGizle(XLS, sutun1, 0, farkYil - 1);
            YilGizle(XLS, sutun1, farkYil + 1, 4);
            //    XLS.SutunGenislikAyarla(baslaSutun + farkYil + 3, baslaSutun + 7, 0);
            XLS.DosyaSaklaTamYol();

            return sonucDosya;
        }

        private static void YilGizle(Tablo XLS, int sutun1, int basla, int bit)
        {
            if (bit >= basla)
            {
                XLS.SutunGizle(sutun1 + 4 + basla * 4, sutun1 + 4 + bit * 4 + 3, true);
                //    XLS.SutunGenislikAyarla(baslaSutun + farkYil + 3, baslaSutun + 7, 0);
            }
        }

        public static string KurumHedefleri(int donemByil, string kurum)
        {
            string sql = "select STRHEDEF.HEDEFKOD, STRHEDEF.HEDEFKISA, STRYAZI.KISAYAZI ";
            sql += "FROM STRHEDEF ";
            sql += "INNER JOIN STRYAZI";
            sql += "    ON STRYAZI.DONEMBYIL=STRHEDEF.DONEMBYIL ";
            sql += "    AND STRYAZI.KURUMKOD=STRHEDEF.KURUMKOD ";
            sql += "    AND STRYAZI.KODTUR=5 ";
            sql += "    AND STRYAZI.KOD=STRHEDEF.HEDEFKOD ";
            sql += "    AND STRYAZI.DILKOD=1 ";
            sql += "where STRHEDEF.DONEMBYIL=" + donemByil.ToString() + " ";
            sql += "AND STRHEDEF.KURUMKOD='" + kurum.Replace(".", "") + "' ";
            sql += "order by STRHEDEF.HEDEFKISA ";
            return sql;
        }

        public static string KurumlaraGoreBasariRaporla(int donemBYil, int yil, int donem, string kurumKod)
        {
            string sql = "select STRHEDEFGOSTERGE.HEDEFKOD, COUNT(DISTINCT STRHEDEFGOSTERGE.GOSTERGEKOD), dbo.HedefOranBul(STRHEDEFGOSTERGE.KURUMKOD, " + yil.ToString() + ", STRHEDEFGOSTERGE.HEDEFKOD, " + donem.ToString() + ")";
            sql += "FROM STRHEDEFGOSTERGE ";

            sql += "where STRHEDEFGOSTERGE.KURUMKOD='" + kurumKod.Replace(".", "").ToString() + "' ";
            sql += "AND STRHEDEFGOSTERGE.DONEMBYIL=" + donemBYil.ToString() + " ";
            sql += " GROUP BY STRHEDEFGOSTERGE.HEDEFKOD, STRHEDEFGOSTERGE.KURUMKOD";
            return sql;
        }

        public static ArrayList GostergeGelisimBul(int hedefKod, Misyon ms, int donem, int islemYil)
        {
            ArrayList liste = new ArrayList();
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            string[] sqller = new string[6];
            string[] baglantilar = new string[6];
            sqller[0] = GostergeleriRaporla(ms.donemBYil, ms.kurumKod, hedefKod);
            baglantilar[0] = "BaglantiSatiriSTR";
            for (int i = ms.aralikBasiYil; i <= ms.aralikSonuYil; i++)
            {
                int fark = (i - ms.aralikBasiYil);
                if (fark > 4) break;
                sqller[1 + fark] = GostergeGerceklesmeleriRaporla(ms.donemBYil, i, donem, ms.kurumKod, hedefKod);
                baglantilar[1 + fark] = "BaglantiSatiriSTR";
            }
            int farkYil = (islemYil - ms.aralikBasiYil);

            SortedList sl = uzyServis.SorguBirlestir(sqller, baglantilar, new int[] { 0 });
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                StrHedefGosterge okunan = new StrHedefGosterge();

                okunan.gostergeKod = alVeri.IntAl(0);

                okunan.gostergeAd = alVeri.StrAl(0);
                okunan.gorKod = alVeri.StrAl(1);
                okunan.gerceklesen = alVeri.DblAl(2);
                okunan.tahmin0 = alVeri.DblAl(3);
                okunan.tahmin1 = alVeri.DblAl(4);
                okunan.tahmin2 = alVeri.DblAl(5);
                okunan.tahmin3 = alVeri.DblAl(6);
                okunan.tahmin4 = alVeri.DblAl(7);
                okunan.hedefeEtkisi = alVeri.DblAl(8);

                okunan.gerceklesen0 = alVeri.DblAl(9);
                if (farkYil == 0) MetinOku(alVeri, 10, okunan);
                okunan.gerceklesen1 = alVeri.DblAl(15);
                if (farkYil == 1) MetinOku(alVeri, 16, okunan);
                okunan.gerceklesen2 = alVeri.DblAl(21);
                if (farkYil == 2) MetinOku(alVeri, 22, okunan);
                okunan.gerceklesen3 = alVeri.DblAl(27);
                if (farkYil == 3) MetinOku(alVeri, 28, okunan);
                okunan.gerceklesen4 = alVeri.DblAl(33);
                if (farkYil == 4) MetinOku(alVeri, 34, okunan);

                okunan.aktifOranNo = islemYil - ms.aralikBasiYil;

                liste.Add(okunan);
            }
            return liste;
        }

        private static void MetinOku(HTArray alVeri, int kolon, StrHedefGosterge okunan)
        {
            okunan.aciklama = alVeri.StrAl(kolon);
            okunan.ilgililik = alVeri.StrAl(kolon + 1);
            okunan.etkililik = alVeri.StrAl(kolon + 2);
            okunan.etkinlik = alVeri.StrAl(kolon + 3);
            okunan.surdurulebilirlik = alVeri.StrAl(kolon + 4);
        }

        private static string GostergeGerceklesmeleriRaporla(int donemBYil, int gercYil, int donem, string kurumKod, int hedefKod)
        {
            string sql = "select STRHEDEFGOSTERGEGERCEKLESME.GOSTERGEKOD, BIRIKIMLIGERCEKLESME, ACIKLAMA, ILGILILIK, ETKILILIK, ETKINLIK, SURDURULEBILIRLIK";
            sql += " FROM STRHEDEFGOSTERGEGERCEKLESME";

            sql += " where STRHEDEFGOSTERGEGERCEKLESME.KURUMKOD='" + kurumKod.Replace(".", "").ToString() + "'";
            sql += " AND STRHEDEFGOSTERGEGERCEKLESME.DONEMBYIL=" + donemBYil.ToString();
            sql += " AND STRHEDEFGOSTERGEGERCEKLESME.HEDEFKOD=" + hedefKod.ToString();
            sql += " AND STRHEDEFGOSTERGEGERCEKLESME.GERCYIL=" + gercYil.ToString();
            sql += " AND STRHEDEFGOSTERGEGERCEKLESME.DONEM=" + donem.ToString();
            sql += " ORDER BY STRHEDEFGOSTERGEGERCEKLESME.GOSTERGEKOD";
            return sql;
        }

        private static string GostergeleriRaporla(int donemBYil, string kurumKod, int hedefKod)
        {
            string sql = "select STRHEDEFGOSTERGE.GOSTERGEKOD, GOSTERGEAD, STRHEDEFGOSTERGE.GORKOD, STRHEDEFGOSTERGE.GERCEKLESEN, TAHMIN0, TAHMIN1, TAHMIN2, TAHMIN3, TAHMIN4, HEDEFEETKISI";
            sql += " FROM STRHEDEFGOSTERGE INNER JOIN PerformansButce.dbo.PEBGOSTERGE GOS on GOS.GOSTERGEKOD = STRHEDEFGOSTERGE.GOSTERGEKOD";

            sql += " where STRHEDEFGOSTERGE.KURUMKOD='" + kurumKod.Replace(".", "").ToString() + "'";
            sql += " AND STRHEDEFGOSTERGE.DONEMBYIL=" + donemBYil.ToString();
            sql += " AND STRHEDEFGOSTERGE.HEDEFKOD=" + hedefKod.ToString();
            sql += " ORDER BY STRHEDEFGOSTERGE.GOSTERGEKOD";
            return sql;
        }

        public static string BMHedefOl(int yil, string kurumKod, int hedefKod)
        {
            return "HDFA_" + yil.ToString() + "_" + kurumKod.Replace(".", "") + "_" + hedefKod;
        }

        public static string MetinOku(Kullanici kullanan, int yil, string kurumKod, int hedefKod)
        {
            IKYMServis sKym = TNS.KYM.Arac.Tanimla();
            return sKym.BuyukMetinOku(kullanan, "SP", BMHedefOl(yil, kurumKod, hedefKod), "TR");
        }
    }

    /// <summary>Eski SPIzlemeGenel.aspx.cs içinden (yalnızca raporların kullandığı yordam).</summary>
    public static class SPIzlemeGenel
    {
        public static bool AraliktaMi(double hedefBasariOrt, int basariAlt, int basariUst)
        {
            bool alt = true;
            bool ust = true;
            if (basariAlt >= 0)
            {
                if (hedefBasariOrt < basariAlt) alt = false;
            }
            if (basariUst >= 0)
            {
                if (hedefBasariOrt > basariUst) ust = false;
            }
            return alt && ust;
        }
    }
}

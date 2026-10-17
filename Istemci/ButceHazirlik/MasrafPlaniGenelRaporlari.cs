using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;
using TNS.UZY;
using TNS.YAT;

namespace ButceHazirlikv2
{
    // Masraf Planı ekranının (ButceHazirlikv2/MasrafPlaniGenel) rapor listesindeki Rapor.xml grup 998 METOD raporları. Eski
    // ButceHazirlikv2/MasrafPlaniRaporlar.cs'ten aynen alındı (NESNE adları aynı kalsın diye isim alanı ButceHazirlikv2); değişenler:
    // TNS.Raporlama.IRapor ve System.Web kaldırıldı, servis "as" ile alınır (eski GetType() == typeof(arayüz) hiç tutmuyordu).
    //   5 RaporlarBirimDonem, 3 RaporlarEko2, 4 RaporlarBirimEko, 41-53 ve 99 MPDetayProgram.
    //   30-37 MPRaporDetay: Core'daki Istemci.ButceHazirlik.MPRaporDetay'a (MasrafPlaniRaporlari.cs) yönlenir.
    //   1 (F8_MP) BHGiderRaporlari.cs ButceHazirlikRaporlari.OdenekCetveliTasra'dadır.

    public class RaporlarEko2
    {
        Tablo XLS;
        int sutun = -1;

        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            XLS = GenelIslemler.NewTablo();
            //int satir = 0;
            //int sutun = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis = o_servis as IUZYServis ?? TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kurumsalYapiKod = (string)raporParam["kurumsalyapi"] + "";
            string kurumsalYapiAd = (string)raporParam["BirimAd"] + "";
            ENUMMasrafPlanAsama asama = (ENUMMasrafPlanAsama)OrtakFonksiyonlar.ConvertToInt((string)raporParam["Asama"] + "", 2);
            string asamaAd = (string)raporParam["AsamaAd"] + "";
            string kurKod = (string)raporParam["KurKod"] + "";
            string kurAd = (string)raporParam["KurAd"] + "";
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABTasraOdenek);
            kistasTertip.TertibeKodEkle(0, kurKod, kurAd);
            kistasTertip.TertibeKodEkle(5, kurumsalYapiKod, kurumsalYapiAd);
            UcgenKodBilgileri ukb = new UcgenKodBilgileri();
            ukb.Add(new UcgenKodBilgi(4, 2));
            BirimMasrafPlaniGenel bmpg = servis.MasrafPlaniBulTavan(kullanan, asama, kistasTertip, ukb, "");

            ToplamAl toplamAl = new ToplamAl(3, 8);

            string oncAnaEko = "";
            int anaSatir = -1;
            int satir = -1;
            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("BirimAd", kurumsalYapiAd);
            XLS.HucreAdBulYaz("KurAd", kurAd);
            XLS.HucreAdBulYaz("Asama", (int)asama);
            int yukseklik = XLS.SatirYukseklikAl(satir);
            int baslaSatir = satir;
            for (int i = 0; i < bmpg.bmpTertip.tertipBilgiListe.Length; i++)
            {
                MPTertipBilgi mpeb = bmpg.bmpTertip.tertipBilgiListe[i];
                GenelOdenekBilgi gob = mpeb.genelOdenekBilgi;
                string gelenEko = mpeb.tertipOdenek.tertip.kodlar[3];
                string gelenEkoAd = mpeb.tertipOdenek.tertip.adlar[3];
                string anaEko = gelenEko.Substring(0, 2);
                int kaynakSatirFark = -3;
                if (anaEko != oncAnaEko)
                {
                    if (anaSatir != -1)
                    {
                        AnaSatirYaz(baslaSatir - 5, anaSatir, oncAnaEko, "", toplamAl, 1, yukseklik);
                        kaynakSatirFark = -4;
                        toplamAl.SatirSifirla(1);
                    }
                    anaSatir = satir++;
                    oncAnaEko = anaEko;
                }
                toplamAl.DegerAta(2, 0, mpeb.tertipOdenek.odenek[0]);
                toplamAl.DegerAta(2, 3, gob.Toplam());
                toplamAl.DegerAta(2, 4, gob.Donem(0));
                toplamAl.DegerAta(2, 5, gob.Donem(1));
                toplamAl.DegerAta(2, 6, gob.Donem(2));
                toplamAl.DegerAta(2, 7, gob.Donem(3));
                toplamAl.ToplamEkle(0, 2);
                toplamAl.ToplamEkle(1, 2);
                AnaSatirYaz(baslaSatir + kaynakSatirFark, satir, gelenEko, gelenEkoAd, toplamAl, 2, yukseklik);
                satir++;
            }
            if (anaSatir != -1)
            {
                AnaSatirYaz(baslaSatir - 5, anaSatir, oncAnaEko, "", toplamAl, 1, yukseklik);
            }
            AnaSatirYaz(baslaSatir - 1, satir, "", "", toplamAl, 0, yukseklik);

            satir += 4;
            XLS.HucreKopyala(0, sutun, 0, sutun + 13, satir, sutun);
            XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
            XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 11);
            XLS.HucreDegerYaz(satir, sutun + 1, (string)raporParam["HazKisi"]);
            XLS.HucreDegerYaz(satir, sutun + 7, (string)raporParam["ImzKisi"]);
            satir += 1;
            XLS.HucreKopyala(0, sutun, 0, sutun + 13, satir, sutun);
            XLS.HucreBirlestir(satir, sutun + 1, satir, sutun + 2);
            XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 11);
            XLS.HucreDegerYaz(satir, sutun + 1, (string)raporParam["HazGorev"]);
            XLS.HucreDegerYaz(satir, sutun + 7, (string)raporParam["ImzGorev"]);
            satir += 1;

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void AnaSatirYaz(int kaynakSatir, int hedefSatir, string ekoKod, string ekoAd, ToplamAl toplamAl, int toplamSatir, int yukseklik)
        {
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, hedefSatir, sutun);
            if (string.IsNullOrEmpty(ekoKod))
            {
                XLS.HucreBirlestir(hedefSatir, sutun + 0, hedefSatir, sutun + 1);
                XLS.HucreDegerYaz(hedefSatir, sutun + 0, "T O P L A M");
            }
            else
                XLS.HucreDegerYaz(hedefSatir, sutun + 0, ekoKod);

            if (string.IsNullOrEmpty(ekoAd) && !string.IsNullOrEmpty(ekoKod))
                ekoAd = GenelIslemler.KodAd(4, DateTime.Now.Year.ToString() + "-" + ekoKod, true);

            XLS.HucreDegerYaz(hedefSatir, sutun + 1, ekoAd);
            XLS.HucreDegerYaz(hedefSatir, sutun + 2, toplamAl.toplam[toplamSatir, 0]);
            XLS.HucreDegerYaz(hedefSatir, sutun + 3, toplamAl.toplam[toplamSatir, 3]);
            XLS.HucreDegerYaz(hedefSatir, sutun + 5, toplamAl.toplam[toplamSatir, 4]);
            XLS.HucreDegerYaz(hedefSatir, sutun + 7, toplamAl.toplam[toplamSatir, 5]);
            XLS.HucreDegerYaz(hedefSatir, sutun + 9, toplamAl.toplam[toplamSatir, 6]);
            XLS.HucreDegerYaz(hedefSatir, sutun + 11, toplamAl.toplam[toplamSatir, 7]);
            XLS.SatirYukseklikAyarla(hedefSatir, hedefSatir, yukseklik);
        }

    }

    public class RaporlarBirimEkoDonem
    {
        Tablo XLS;
        int sutun = -1;

        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, int ekBilgi)
        {
            XLS = GenelIslemler.NewTablo();
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis = o_servis as IBHMv2Servis ?? TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis = o_servis as IUZYServis ?? TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kurumsalYapiKod = (string)raporParam["kurumsalyapi"] + "";
            string kurumsalYapiAd = (string)raporParam["BirimAd"] + "";
            string kurKod = (string)raporParam["KurKod"] + "";
            string kurAd = (string)raporParam["KurAd"] + "";
            string ekoKod = (string)raporParam["EkoKod"] + "";
            string ekoAd = (string)raporParam["EkoAd"] + "";
            ENUMMasrafPlanAsama asama = (ENUMMasrafPlanAsama)OrtakFonksiyonlar.ConvertToInt((string)raporParam["Asama"] + "", 2);

            TertipOdenek[] ob = servis.BirimMasrafPlaniOzetListele(kullanan, yil, kurumsalYapiKod, asama, kurKod, ekoKod, ekBilgi);
            int sutSay;
            if (ekBilgi == 1)
                sutSay = 9;
            else
                sutSay = 4;


            ToplamAl toplamAl = new ToplamAl(2, sutSay);

            int satir = -1;
            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("BirimAd", kurumsalYapiAd);


            XLS.HucreAdBulYaz("KurAd", kurAd);
            XLS.HucreAdBulYaz("EkoAd", ekoAd);

            int yukseklik = XLS.SatirYukseklikAl(satir);
            int baslaSatir = satir;
            for (int i = 0; i < ob.Length; i++)
            {
                int kaynakSatirFark;
                if (baslaSatir == satir)
                    kaynakSatirFark = -4;
                else
                    kaynakSatirFark = -3;

                for (int j = 0; j < sutSay; j++)
                {
                    if (ekBilgi == 1)
                        toplamAl.DegerAta(1, j, ob[i].odenek[j]);
                    else
                    {
                        int basla = j * 3;
                        toplamAl.DegerAta(1, j, ob[i].odenek[basla] + ob[i].odenek[basla + 1] + ob[i].odenek[basla + 2]);
                    }
                }

                AnaSatirYaz(baslaSatir + kaynakSatirFark, satir, ob[i].tertip.kodlar[5], ob[i].tertip.adlar[5], toplamAl, 1, yukseklik);
                toplamAl.ToplamEkle(0, 1);
                satir++;
            }
            AnaSatirYaz(baslaSatir - 1, satir, "", "", toplamAl, 0, yukseklik);

            if (sutSay == 4)
            {
                satir += 4;
                XLS.HucreKopyala(0, sutun, 0, sutun + 13, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);
                XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 11);
                XLS.HucreDegerYaz(satir, sutun + 2, (string)raporParam["HazKisi"]);
                XLS.HucreDegerYaz(satir, sutun + 7, (string)raporParam["ImzKisi"]);
                satir += 1;
                XLS.HucreKopyala(0, sutun, 0, sutun + 13, satir, sutun);
                XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);
                XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 11);
                XLS.HucreDegerYaz(satir, sutun + 2, (string)raporParam["HazGorev"]);
                XLS.HucreDegerYaz(satir, sutun + 7, (string)raporParam["ImzGorev"]);
                satir += 1;
            }


            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void AnaSatirYaz(int kaynakSatir, int hedefSatir, string kod, string ad, ToplamAl toplamAl, int toplamSatir, int yukseklik)
        {
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, hedefSatir, sutun);
            if (string.IsNullOrEmpty(kod))
            {
                XLS.HucreBirlestir(hedefSatir, sutun + 0, hedefSatir, sutun + 2);
                XLS.HucreDegerYaz(hedefSatir, sutun + 0, "T O P L A M");
            }
            else
            {
                XLS.HucreBirlestir(hedefSatir, sutun + 1, hedefSatir, sutun + 2);
                string[] kodDizi = kod.Split('.');
                XLS.HucreDegerYaz(hedefSatir, sutun + 0, kodDizi[kodDizi.Length - 1]);
                XLS.HucreDegerYaz(hedefSatir, sutun + 1, ad);
            }

            if (toplamAl.SutunSay() == 9)
            {
                for (int i = 0; i < 9; i++)
                {
                    XLS.HucreDegerYaz(hedefSatir, sutun + 3 + i, toplamAl.toplam[toplamSatir, i]);
                }
            }
            else
            {
                XLS.HucreDegerYaz(hedefSatir, sutun + 3 + 0, toplamAl.toplam[toplamSatir, 0]);
                XLS.HucreDegerYaz(hedefSatir, sutun + 3 + 2, toplamAl.toplam[toplamSatir, 1]);
                XLS.HucreDegerYaz(hedefSatir, sutun + 3 + 4, toplamAl.toplam[toplamSatir, 2]);
                XLS.HucreDegerYaz(hedefSatir, sutun + 3 + 6, toplamAl.toplam[toplamSatir, 3]);
            }
            XLS.SatirYukseklikAyarla(hedefSatir, hedefSatir, yukseklik);
        }

    }

    public class RaporlarBirimEko
    {
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            RaporlarBirimEkoDonem rbeb = new RaporlarBirimEkoDonem();
            return rbeb.RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, 1);
        }
    }

    public class RaporlarBirimDonem
    {
        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            RaporlarBirimEkoDonem rbeb = new RaporlarBirimEkoDonem();
            return rbeb.RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, 2);
        }
    }

    /// <summary>Rapor.xml NESNE "ButceHazirlikv2.MPRaporDetay" (998-30..37): kod Istemci.ButceHazirlik.MPRaporDetay'dadır.</summary>
    public class MPRaporDetay : Istemci.ButceHazirlik.MPRaporDetay
    {
    }

    public class MPDetayProgram
    {
        Kullanici kullanan;
        string sablonAd;
        string sonucDosyaAd;
        System.Collections.Hashtable raporParam;
        private IBHMv2Servis servisBHMv2;
        int yil;

        public string TasraToplam()
        {
            string ILKOD = "case SUBSTRING(MPTAVAN.KYKOD,2,2) when '00' then '06' else SUBSTRING(MPTAVAN.KYKOD,2,2) end";
            string sql = "SELECT " + ILKOD + "  ILKOD, SUBSTRING(MPTAVAN.KYKOD,1,1) IL_TESK, ";
            sql += "case SUBSTRING(MPTAVAN.KYKOD,4,3) ";
            sql += "when '015' then 1 ";
            sql += "when '100' then 1 ";
            sql += "when '150' then 1 ";
            sql += "else 2 ";
            sql += "end ILMUD, ";
            sql += "MPTAVAN.KYKOD, LEFT(MPTAVAN.EKOKOD,2) EKOKOD,  ";
            sql += "ButceHazirlik.dbo.ABSEKO.EKOAD, k2.ad ILAD, k3.AD ILCEAD, ";
            sql += "ButceIslemleri.dbo.BHMKURUMSALYAPI.AD BIRIMAD, SUM(MPTAVAN.ODENEK) ODENEK ";
            sql += "FROM  MPTAVAN INNER JOIN ";
            sql += "ButceHazirlik.dbo.ABSEKO ON MPTAVAN.YIL = ButceHazirlik.dbo.ABSEKO.YIL AND LEFT(MPTAVAN.EKOKOD,2) = ButceHazirlik.dbo.ABSEKO.EKOKOD INNER JOIN ";
            sql += "ButceIslemleri.dbo.BHMKURUMSALYAPI ON MPTAVAN.KYKOD = ButceIslemleri.dbo.BHMKURUMSALYAPI.KOD, ButceIslemleri.dbo.BHMKURUMSALYAPI k2, ButceIslemleri.dbo.BHMKURUMSALYAPI k3 ";
            sql += "where MPTAVAN.YIL=" + yil.ToString() + " AND MPTAVAN.DETAYMI=1 AND  ";
            sql += "k2.KOD=LEFT(MPTAVAN.KYKOD,3) AND ";
            sql += "k3.KOD=LEFT(MPTAVAN.KYKOD,6) AND ";
            sql += "(KYKOD LIKE '2%' OR KYKOD LIKE '3%') ";
            sql += "group by " + ILKOD + ", SUBSTRING(MPTAVAN.KYKOD,1,1), ";
            sql += "case SUBSTRING(MPTAVAN.KYKOD,4,3) ";
            sql += "when '015' then 1 ";
            sql += "when '100' then 1 ";
            sql += "when '150' then 1 ";
            sql += "else 2 ";
            sql += "end,   ";
            sql += "MPTAVAN.KYKOD, LEFT(MPTAVAN.EKOKOD,2),  ";
            sql += "ButceHazirlik.dbo.ABSEKO.EKOAD,  ";
            sql += "ButceIslemleri.dbo.BHMKURUMSALYAPI.AD, k2.AD, k3.AD ";
            sql += "order by 1,2,3,4,5";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0;
            int fSat1 = 0;
            int fSat2 = 0;
            int fSut1 = 0;
            int fSut2 = 0;
            int baslaSat = 0;
            int baslaSut = 0;
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);
            VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBHMv2", 0);
            int eskiIlSat = -1;
            int eskiBirimSat = -1;
            string eskiIl = "";
            string eskiBirim = "";
            ToplamAl toplamAl = new ToplamAl(4, 1);
            int satir = baslaSat;
            while (true)
            {
                System.Data.DataRow row = vts.satirOku();
                if (row == null) break;

                string ilKod = vts.StringAl(0);
                string il_Tesk = vts.StringAl(1);
                string ilMud = vts.StringAl(2);
                string kyKod = vts.StringAl(3);
                string ekoKod = vts.StringAl(4);
                string ekoAd = vts.StringAl(5);
                string ilAd = vts.StringAl(6);
                string ilceAd = vts.StringAl(7);
                string birimAd = vts.StringAl(8);
                double odenek = vts.DoubleAl(9);
                if (Gecersiz(ekoKod)) continue;
                if (eskiIl != ilKod)
                {
                    if (eskiIlSat > 0)
                    {
                        XLS.HucreDegerYaz(eskiIlSat, baslaSut + 3, toplamAl.toplam[1, 0]);
                        toplamAl.SatirSifirla(1);
                    }

                    if (eskiIl != "")
                        XLS.SayfaSonuKoy(satir);

                    XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, ilAd);
                    eskiIlSat = satir;
                    satir++;
                    eskiIl = ilKod;
                    eskiBirim = "";
                }
                if (eskiBirim != kyKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        XLS.HucreDegerYaz(eskiBirimSat, baslaSut + 3, toplamAl.toplam[2, 0]);
                        toplamAl.SatirSifirla(2);
                    }
                    XLS.HucreKopyala(fSat1 + 1, fSut1, fSat2 + 1, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, TNS.BHMv2.Arac.BirimAdOl(ilceAd, birimAd));
                    eskiBirimSat = satir;
                    satir++;
                    eskiBirim = kyKod;
                }

                XLS.HucreKopyala(fSat1 + 2, fSut1, fSat2 + 2, fSut2, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut + 1, ekoKod);
                XLS.HucreDegerYaz(satir, baslaSut + 2, ekoAd);
                XLS.HucreDegerYaz(satir, baslaSut + 3, odenek);
                toplamAl.DegerAta(3, 0, odenek);
                toplamAl.ToplamEkle(2, 3);
                toplamAl.ToplamEkle(1, 3);
                toplamAl.ToplamEkle(0, 3);
                satir++;
            }

            if (eskiIlSat > 0)
            {
                XLS.HucreDegerYaz(eskiIlSat, baslaSut + 3, toplamAl.toplam[1, 0]);
                toplamAl.SatirSifirla(1);
            }
            if (eskiBirimSat > 0)
            {
                XLS.HucreDegerYaz(eskiBirimSat, baslaSut + 3, toplamAl.toplam[2, 0]);
                toplamAl.SatirSifirla(2);
            }
            satir++;
            XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut, "Taşra Teşkilatı Ödenekleri Toplamı");
            XLS.HucreDegerYaz(satir, baslaSut + 3, toplamAl.toplam[0, 0]);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private bool Gecersiz(string ekoKod)
        {
            if (ekoKod.StartsWith("03")) return false;
            if (ekoKod.StartsWith("06")) return false;
            return true;
        }

        public string MerkezToplam()
        {
            string sql = "SELECT MPTAVAN.KYKOD, LEFT(MPTAVAN.EKOKOD,2) EKOKOD, ButceHazirlik.dbo.ABSEKO.EKOAD, Upper(ButceIslemleri.dbo.BHMKURUMSALYAPI.AD) BIRIMAD, SUM(MPTAVAN.ODENEK) ODENEK ";
            sql += "FROM MPTAVAN INNER JOIN ButceHazirlik.dbo.ABSEKO ON MPTAVAN.YIL = ButceHazirlik.dbo.ABSEKO.YIL AND LEFT(MPTAVAN.EKOKOD,2) = ButceHazirlik.dbo.ABSEKO.EKOKOD ";
            sql += "INNER JOIN ButceIslemleri.dbo.BHMKURUMSALYAPI ON MPTAVAN.KYKOD = ButceIslemleri.dbo.BHMKURUMSALYAPI.KOD ";
            sql += "where MPTAVAN.YIL=" + yil.ToString() + " AND MPTAVAN.DETAYMI=1 AND (KYKOD LIKE '1%') ";
            sql += "group by  MPTAVAN.KYKOD, LEFT(MPTAVAN.EKOKOD,2), ButceHazirlik.dbo.ABSEKO.EKOAD, ButceIslemleri.dbo.BHMKURUMSALYAPI.AD ";
            sql += "order by 1,2";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0;
            int fSat1 = 0;
            int fSat2 = 0;
            int fSut1 = 0;
            int fSut2 = 0;
            int baslaSat = 0;
            int baslaSut = 0;
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);
            VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBHMv2", 0);
            int eskiBirimSat = -1;
            string eskiBirim = "";
            ToplamAl toplamAl = new ToplamAl(4, 1);
            int satir = baslaSat;
            while (true)
            {
                System.Data.DataRow row = vts.satirOku();
                if (row == null) break;

                string kyKod = vts.StringAl(0);
                string ekoKod = vts.StringAl(1);
                string ekoAd = vts.StringAl(2);
                string birimAd = vts.StringAl(3);
                double odenek = vts.DoubleAl(4);
                if (Gecersiz(ekoKod)) continue;

                if (eskiBirim != kyKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        XLS.HucreDegerYaz(eskiBirimSat, baslaSut + 3, toplamAl.toplam[2, 0]);
                        toplamAl.SatirSifirla(2);
                    }
                    XLS.HucreKopyala(fSat1 + 0, fSut1, fSat2 + 0, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, birimAd);
                    eskiBirimSat = satir;
                    satir++;
                    eskiBirim = kyKod;
                }

                XLS.HucreKopyala(fSat1 + 2, fSut1, fSat2 + 2, fSut2, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut + 1, ekoKod);
                XLS.HucreDegerYaz(satir, baslaSut + 2, ekoAd);
                XLS.HucreDegerYaz(satir, baslaSut + 3, odenek);
                toplamAl.DegerAta(3, 0, odenek);
                toplamAl.ToplamEkle(2, 3);
                toplamAl.ToplamEkle(1, 3);
                toplamAl.ToplamEkle(0, 3);
                satir++;
            }

            if (eskiBirimSat > 0)
            {
                XLS.HucreDegerYaz(eskiBirimSat, baslaSut + 3, toplamAl.toplam[2, 0]);
                toplamAl.SatirSifirla(2);
            }
            satir++;
            XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut, "Merkez Teşkilatı Ödenekleri Toplamı");
            XLS.HucreDegerYaz(satir, baslaSut + 3, toplamAl.toplam[0, 0]);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string MerkezMerkezToplam()
        {
            //string kyKod = vts.StringAl(0);
            //string ekoKod = vts.StringAl(1);
            //string ekoAd = vts.StringAl(2);
            //string birimAd = vts.StringAl(3);
            //double odenek = vts.DoubleAl(4);


            string sql = "SELECT T2.BIRIMKOD, T2.EKO2, T2.EKOAD, T2.BIRIMAD, (T2.ODENEK- ISNULL(T1.ODENEK,0)) ODENEK ";
            sql += "FROM (SELECT LEFT(T.PEBKOD,2) BIRIMKOD, LEFT(T.EKOKOD,2) EKO2, ";
            sql += "SUM(ODENEK) ODENEK ";
            sql += "FROM MPTAVAN T ";
            sql += "WHERE T.YIL=" + yil.ToString() + " and T.DETAYMI=1 ";
            sql += "group by LEFT(T.PEBKOD,2), LEFT(T.EKOKOD,2)) T1 ";
            sql += "RIGHT OUTER JOIN ";
            sql += "(SELECT LEFT(T.PEBKOD,2) BIRIMKOD, LEFT(T.EKOKOD,2) EKO2, EKO.EKOAD EKOAD, ";
            sql += "UPPER(P1.PEBAD) BIRIMAD, SUM(ODENEK) ODENEK ";
            sql += "FROM BHMODENEK T, ButceHazirlik.dbo.ABSPEBKOD P1, ButceHazirlik.dbo.ABSEKO EKO ";
            sql += "WHERE P1.PEBKOD=LEFT(T.PEBKOD,2) and P1.YIL=T.YIL ";
            sql += "AND EKO.EKOKOD=LEFT(T.EKOKOD,2) and EKO.YIL=T.YIL ";
            sql += "and T.YIL=" + yil.ToString() + " and T.SENARYOKOD='0600' and T.TEKLIFYIL=0 ";
            sql += "group by  LEFT(T.PEBKOD,2), LEFT(T.EKOKOD,2), EKO.EKOAD, UPPER(P1.PEBAD)) T2 ";
            sql += "ON T1.BIRIMKOD=T2.BIRIMKOD ";
            sql += "AND T1.EKO2=T2.EKO2 ";
            sql += "where (T2.ODENEK- ISNULL(T1.ODENEK,0))<>0 ";
            sql += "order by 1,2";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0;
            int fSat1 = 0;
            int fSat2 = 0;
            int fSut1 = 0;
            int fSut2 = 0;
            int baslaSat = 0;
            int baslaSut = 0;
            XLS.HucreAdBulYaz("Yil", yil);

            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);
            VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBHMv2", 0);
            int eskiBirimSat = -1;
            string eskiBirim = "";
            ToplamAl toplamAl = new ToplamAl(4, 1);
            int satir = baslaSat;
            while (true)
            {
                System.Data.DataRow row = vts.satirOku();
                if (row == null) break;

                string kyKod = vts.StringAl(0);
                string ekoKod = vts.StringAl(1);
                string ekoAd = vts.StringAl(2);
                string birimAd = vts.StringAl(3);
                double odenek = vts.DoubleAl(4);
                if (Gecersiz(ekoKod)) continue;

                if (eskiBirim != kyKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        XLS.HucreDegerYaz(eskiBirimSat, baslaSut + 3, toplamAl.toplam[2, 0]);
                        toplamAl.SatirSifirla(2);
                    }
                    XLS.HucreKopyala(fSat1 + 0, fSut1, fSat2 + 0, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, birimAd);
                    eskiBirimSat = satir;
                    satir++;
                    eskiBirim = kyKod;
                }

                XLS.HucreKopyala(fSat1 + 2, fSut1, fSat2 + 2, fSut2, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut + 1, ekoKod);
                XLS.HucreDegerYaz(satir, baslaSut + 2, ekoAd);
                XLS.HucreDegerYaz(satir, baslaSut + 3, odenek);
                toplamAl.DegerAta(3, 0, odenek);
                toplamAl.ToplamEkle(2, 3);
                toplamAl.ToplamEkle(1, 3);
                toplamAl.ToplamEkle(0, 3);
                satir++;
            }

            if (eskiBirimSat > 0)
            {
                XLS.HucreDegerYaz(eskiBirimSat, baslaSut + 3, toplamAl.toplam[2, 0]);
                toplamAl.SatirSifirla(2);
            }
            satir++;
            XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut, "Merkez Merkez Ödenekleri Toplamı");
            XLS.HucreDegerYaz(satir, baslaSut + 3, toplamAl.toplam[0, 0]);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string TasraDetay(int eko2Uz)
        {
            string ILKOD = "case SUBSTRING(T.KYKOD,2,2) when '00' then '06' else SUBSTRING(T.KYKOD,2,2) end";

            string sql = "SELECT " + ILKOD + " IL, CASE SUBSTRING(T.KYKOD,1,1) ";
            sql += "when '2' then  SUBSTRING(T.KYKOD,1,3) ";
            sql += "else   T.KYKOD  ";
            sql += "END BIRIM, K.AD BIRIMAD ";
            sql += $", LEFT(T.PEBKOD,8) PROJE, P2.PEBAD, T.KURKOD, T.FONKOD, T.FINKOD, LEFT(T.EKOKOD,2) EKO2, SUBSTRING(T.EKOKOD,3,{eko2Uz}) EKO3, P1.PEBAD2, SUM(ODENEK) ODENEK ";
            sql += "FROM MPTAVAN T, ButceIslemleri.dbo.BHMKURUMSALYAPI K, ButceHazirlik.dbo.ABSPEBKOD P2, ButceHazirlik.dbo.ABSPEBKOD P1 ";
            sql += "WHERE CASE SUBSTRING(T.KYKOD,1,1) ";
            sql += "when '2' then  SUBSTRING(T.KYKOD,1,3) ";
            sql += "else   T.KYKOD  ";
            sql += "END=K.KOD ";
            sql += "and P2.PEBKOD=LEFT(T.PEBKOD,8) and P2.YIL=T.YIL ";
            sql += "and P1.PEBKOD=LEFT(T.PEBKOD,2) and P1.YIL=T.YIL ";
            sql += "and T.YIL=" + yil.ToString() + " and T.DETAYMI=1 and (T.KYKOD LIKE '2%' OR T.KYKOD LIKE '3%') ";
            sql += "group by " + ILKOD;
            sql += ", CASE SUBSTRING(T.KYKOD,1,1) ";
            sql += "when '2' then  SUBSTRING(T.KYKOD,1,3) ";
            sql += "else T.KYKOD ";
            sql += "END, K.AD ";
            sql += ",LEFT(T.PEBKOD,8), P2.PEBAD ";
            sql += ",T.KURKOD ";
            sql += ",T.FONKOD ";
            sql += ",T.FINKOD ";
            sql += ",LEFT(T.EKOKOD,2) ";
            sql += $",SUBSTRING(T.EKOKOD,3,{eko2Uz}) ";
            sql += ",P1.PEBAD2 ";
            sql += "ORDER BY 1,2,4,6,7,8,9,10";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0;
            int fSat1 = 0;
            int fSat2 = 0;
            int fSut1 = 0;
            int fSut2 = 0;
            int baslaSat = 0;
            int baslaSut = 0;

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);
            VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBHMv2", 0);
            int eskiIlSat = -1;
            int eskiBirimSat = -1;
            int eskiTertipSat = -1;

            string eskiIl = "";
            string eskiBirim = "";
            string eskiProje = "";
            string eskiTertip = "";
            ToplamAl toplamAl = new ToplamAl(5, 9);
            int satir = baslaSat;
            string ilAd = "";
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            while (true)
            {
                System.Data.DataRow row = vts.satirOku();
                if (row == null) break;

                string ilKod = vts.StringAl(0);
                string kyKod = vts.StringAl(1);
                string birimAd = vts.StringAl(2);
                string proje = vts.StringAl(3);
                string pebAd = vts.StringAl(4);
                string kurKod = vts.StringAl(5);
                string fonKod = vts.StringAl(6);
                string finKod = vts.StringAl(7);
                string eko2 = vts.StringAl(8);
                string eko3 = vts.StringAl(9);
                string pebAd2 = vts.StringAl(10);
                double odenek = vts.DoubleAl(11);
                if (Gecersiz(eko2)) continue;

                string tertip = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), kurKod) + "-" + OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), fonKod) + "-" + finKod + "-" + OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), eko2);
                //XLS.HucreDegerYaz(satir, baslaSut + 1, ilKod);
                //XLS.HucreDegerYaz(satir, baslaSut + 2, birim);
                //XLS.HucreDegerYaz(satir, baslaSut + 3, birimAd);
                //XLS.HucreDegerYaz(satir, baslaSut + 4, proje);
                //XLS.HucreDegerYaz(satir, baslaSut + 5, pebAd);
                //XLS.HucreDegerYaz(satir, baslaSut + 6, kurKod + "-" + fonKod + "-" + finKod + "-" + eko2);
                //XLS.HucreDegerYaz(satir, baslaSut + 7, eko3);
                //XLS.HucreDegerYaz(satir, baslaSut + 8, pebAd2);
                //XLS.HucreDegerYaz(satir, baslaSut + 9, odenek);

                if (eskiIl != ilKod)
                {
                    if (eskiIlSat > 0)
                    {
                        ToplamSatirYaz(XLS, eskiIlSat, baslaSut + 3, toplamAl, 1);
                    }

                    if (eskiIl != "")
                        XLS.SayfaSonuKoy(satir);

                    XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
                    ilAd = birimAd;
                    XLS.HucreDegerYaz(satir, baslaSut, ilAd);
                    eskiIlSat = satir;
                    satir++;
                    eskiIl = ilKod;
                    eskiBirim = "";
                }
                if (eskiBirim != kyKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        ToplamSatirYaz(XLS, eskiBirimSat, baslaSut + 3, toplamAl, 2);
                    }
                    XLS.HucreKopyala(fSat1 + 1, fSut1, fSat2 + 1, fSut2, satir, baslaSut);
                    string birimIcAd = "";
                    if (kyKod.Length == 3) birimIcAd = ilAd + " İl ve İlçe Müdürlükleri";
                    else birimIcAd = birimAd;
                    XLS.HucreDegerYaz(satir, baslaSut, birimIcAd);
                    eskiBirimSat = satir;
                    satir++;
                    eskiBirim = kyKod;
                }
                if (eskiProje != proje || eskiTertip != tertip)
                {
                    if (eskiTertipSat > 0)
                    {
                        ToplamSatirYaz(XLS, eskiTertipSat, baslaSut + 3, toplamAl, 3);
                    }
                    XLS.HucreKopyala(fSat1 + 2, fSut1, fSat2 + 2, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, pebAd2);
                    XLS.HucreDegerYaz(satir, baslaSut + 1, pebAd);
                    XLS.HucreDegerYaz(satir, baslaSut + 2, tertip);

                    XLS.HucreDegerYaz(satir, baslaSut + 25, kyKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 26, proje);
                    XLS.HucreDegerYaz(satir, baslaSut + 27, kurKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 28, fonKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 29, finKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 30, eko2);
                    XLS.HucreDegerYaz(satir, baslaSut + 31, pebAd);
                    XLS.HucreDegerYaz(satir, baslaSut + 32, birimAd);

                    eskiTertipSat = satir;
                    satir++;
                    eskiProje = proje;
                    eskiTertip = tertip;
                }
                int yer = OrtakFonksiyonlar.ConvertToInt(eko3, 0) - 1;
                if (yer >= 0)
                {
                    toplamAl.DegerAta(4, yer, odenek);
                    toplamAl.DegerArttir(3, yer, odenek);
                    toplamAl.DegerArttir(2, yer, odenek);
                    toplamAl.DegerArttir(1, yer, odenek);
                    toplamAl.DegerArttir(0, yer, odenek);
                }
            }

            if (eskiIlSat > 0)
            {
                ToplamSatirYaz(XLS, eskiIlSat, baslaSut + 3, toplamAl, 1);
            }
            if (eskiBirimSat > 0)
            {
                ToplamSatirYaz(XLS, eskiBirimSat, baslaSut + 3, toplamAl, 2);
            }
            if (eskiTertipSat > 0)
            {
                ToplamSatirYaz(XLS, eskiTertipSat, baslaSut + 3, toplamAl, 3);
            }

            satir++;
            XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut, "Taşra Teşkilatı Ödenekleri Toplamı");
            ToplamSatirYaz(XLS, satir, baslaSut + 3, toplamAl, 0);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string MerkezDetay(int eko2uz)
        {
            string sql = $"SELECT T.KYKOD, T.PEBKOD PROJE, T.KURKOD, T.FONKOD, T.FINKOD, LEFT(T.EKOKOD,2) EKO2, SUBSTRING(T.EKOKOD,3,{eko2uz}) EKO3, P2.PEBAD, UPPER(K.AD) PEBAD2, SUM(ODENEK) ODENEK ";
            sql += "FROM MPTAVAN T, ButceIslemleri.dbo.BHMKURUMSALYAPI K, ButceHazirlik.dbo.ABSPEBKOD P2 ";
            sql += "WHERE T.KYKOD = K.KOD ";
            sql += "and P2.PEBKOD = T.PEBKOD and P2.YIL=T.YIL ";
            sql += "and T.YIL=" + yil.ToString() + " and T.DETAYMI=1 and ( T.KYKOD LIKE '1%') ";
            sql += $"group by T.KYKOD, T.PEBKOD, T.KURKOD, T.FONKOD, T.FINKOD, LEFT(T.EKOKOD,2), SUBSTRING(T.EKOKOD,3,{eko2uz}), P2.PEBAD, UPPER(K.AD) ";
            sql += "order by 1,2,3,4,5,6,7";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0;
            int fSat1 = 0;
            int fSat2 = 0;
            int fSut1 = 0;
            int fSut2 = 0;
            int baslaSat = 0;
            int baslaSut = 0;
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);
            VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBHMv2", 0);
            int eskiBirimSat = -1;
            int eskiTertipSat = -1;

            string eskiBirim = "";
            string eskiProje = "";
            string eskiTertip = "";
            ToplamAl toplamAl = new ToplamAl(5, 9);
            int satir = baslaSat;
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            while (true)
            {
                System.Data.DataRow row = vts.satirOku();
                if (row == null) break;

                string kyKod = vts.StringAl(0);
                string proje = vts.StringAl(1);
                string kurKod = vts.StringAl(2);
                string fonKod = vts.StringAl(3);
                string finKod = vts.StringAl(4);
                string eko2 = vts.StringAl(5);
                string eko3 = vts.StringAl(6);
                string pebAd = vts.StringAl(7);
                string birimAd = vts.StringAl(8);
                double odenek = vts.DoubleAl(9);
                if (Gecersiz(eko2)) continue;

                string tertip = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), kurKod) + "-" + OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), fonKod) + "-" + finKod + "-" + OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), eko2);
                //XLS.HucreDegerYaz(satir, baslaSut + 1, ilKod);
                //XLS.HucreDegerYaz(satir, baslaSut + 2, birim);
                //XLS.HucreDegerYaz(satir, baslaSut + 3, birimAd);
                //XLS.HucreDegerYaz(satir, baslaSut + 4, proje);
                //XLS.HucreDegerYaz(satir, baslaSut + 5, pebAd);
                //XLS.HucreDegerYaz(satir, baslaSut + 6, kurKod + "-" + fonKod + "-" + finKod + "-" + eko2);
                //XLS.HucreDegerYaz(satir, baslaSut + 7, eko3);
                //XLS.HucreDegerYaz(satir, baslaSut + 8, pebAd2);
                //XLS.HucreDegerYaz(satir, baslaSut + 9, odenek);

                if (eskiBirim != kyKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        ToplamSatirYaz(XLS, eskiBirimSat, baslaSut + 3, toplamAl, 2);
                    }
                    XLS.HucreKopyala(fSat1 + 1, fSut1, fSat2 + 1, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, birimAd);
                    eskiBirimSat = satir;
                    satir++;
                    eskiBirim = kyKod;
                }
                if (eskiProje != proje || eskiTertip != tertip)
                {
                    if (eskiTertipSat > 0)
                    {
                        ToplamSatirYaz(XLS, eskiTertipSat, baslaSut + 3, toplamAl, 3);
                    }
                    XLS.HucreKopyala(fSat1 + 2, fSut1, fSat2 + 2, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 1, pebAd);
                    XLS.HucreDegerYaz(satir, baslaSut + 2, tertip);

                    XLS.HucreDegerYaz(satir, baslaSut + 25, kyKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 26, proje);
                    XLS.HucreDegerYaz(satir, baslaSut + 27, kurKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 28, fonKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 29, finKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 30, eko2);
                    XLS.HucreDegerYaz(satir, baslaSut + 31, pebAd);
                    XLS.HucreDegerYaz(satir, baslaSut + 32, birimAd);

                    eskiTertipSat = satir;
                    satir++;
                    eskiProje = proje;
                    eskiTertip = tertip;
                }
                int yer = OrtakFonksiyonlar.ConvertToInt(eko3, 0) - 1;
                if (yer >= 0)
                {
                    toplamAl.DegerAta(4, yer, odenek);
                    toplamAl.DegerArttir(3, yer, odenek);
                    toplamAl.DegerArttir(2, yer, odenek);
                    toplamAl.DegerArttir(1, yer, odenek);
                    toplamAl.DegerArttir(0, yer, odenek);
                }
            }

            if (eskiBirimSat > 0)
            {
                ToplamSatirYaz(XLS, eskiBirimSat, baslaSut + 3, toplamAl, 2);
            }
            if (eskiTertipSat > 0)
            {
                ToplamSatirYaz(XLS, eskiTertipSat, baslaSut + 3, toplamAl, 3);
            }

            satir++;
            XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut, "Merkez Teşkilatı Ödenekleri Toplamı");
            ToplamSatirYaz(XLS, satir, baslaSut + 3, toplamAl, 0);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string MerkezMerkezDetay(int eko2Uz)
        {
            string sql = "SELECT T2.BIRIMKOD, T2.PROJE, T2.KURKOD, T2.FONKOD, T2.FINKOD, T2.EKO2, T2.EKO3, T2.PEBAD, T2.BIRIMAD, (T2.ODENEK- ISNULL(T1.ODENEK,0)) ODENEK ";
            sql += $"FROM (SELECT LEFT(T.PEBKOD,2) BIRIMKOD,  T.PEBKOD PROJE, T.KURKOD KURKOD, T.FONKOD FONKOD, T.FINKOD FINKOD, LEFT(T.EKOKOD,2) EKO2, SUBSTRING(T.EKOKOD,3,{eko2Uz}) EKO3, ";
            sql += "P2.PEBAD PEBAD, UPPER(P1.PEBAD) BIRIMAD, SUM(ODENEK) ODENEK ";
            sql += "FROM MPTAVAN T,  ButceHazirlik.dbo.ABSPEBKOD P2,  ButceHazirlik.dbo.ABSPEBKOD P1 ";
            sql += "WHERE P2.PEBKOD=T.PEBKOD and P2.YIL=T.YIL ";
            sql += "and P1.PEBKOD=LEFT(T.PEBKOD,2) and P1.YIL=T.YIL ";
            sql += "and T.YIL=" + yil.ToString() + " and T.DETAYMI=1 ";
            sql += $"group by  LEFT(T.PEBKOD,2), T.PEBKOD, T.KURKOD, T.FONKOD, T.FINKOD, LEFT(T.EKOKOD,2), SUBSTRING(T.EKOKOD,3,{eko2Uz}), P2.PEBAD, UPPER(P1.PEBAD) ";
            sql += ") T1 RIGHT OUTER JOIN  ( ";
            sql += $"SELECT LEFT(T.PEBKOD,2) BIRIMKOD,  T.PEBKOD PROJE, T.KURKOD KURKOD, T.FONKOD FONKOD, T.FINKOD FINKOD, LEFT(T.EKOKOD,2) EKO2, SUBSTRING(T.EKOKOD,3,{eko2Uz}) EKO3, ";
            sql += "P2.PEBAD PEBAD, UPPER(P1.PEBAD) BIRIMAD, SUM(ODENEK) ODENEK ";
            sql += "FROM BHMODENEK T,  ButceHazirlik.dbo.ABSPEBKOD P2,  ButceHazirlik.dbo.ABSPEBKOD P1 ";
            sql += "WHERE P2.PEBKOD=T.PEBKOD and P2.YIL=T.YIL ";
            sql += "and P1.PEBKOD=LEFT(T.PEBKOD,2) and P1.YIL=T.YIL ";
            sql += "and T.YIL=" + yil.ToString() + " and T.SENARYOKOD='0600' and T.TEKLIFYIL=0 ";
            sql += $"group by  LEFT(T.PEBKOD,2), T.PEBKOD, T.KURKOD, T.FONKOD, T.FINKOD, LEFT(T.EKOKOD,2), SUBSTRING(T.EKOKOD,3,{eko2Uz}), P2.PEBAD, UPPER(P1.PEBAD)) T2 ";
            sql += "ON T1.BIRIMKOD=T2.BIRIMKOD ";
            sql += "AND T1.PROJE=T2.PROJE ";
            sql += "AND T1.KURKOD=T2.KURKOD ";
            sql += "AND T1.FONKOD=T2.FONKOD ";
            sql += "AND T1.FINKOD=T2.FINKOD ";
            sql += "AND T1.EKO2=T2.EKO2 ";
            sql += "AND T1.EKO3=T2.EKO3 ";
            sql += "where (T2.ODENEK- ISNULL(T1.ODENEK,0))<>0 ";
            sql += "order by 1,2,3,4,5,6,7 ";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0;
            int fSat1 = 0;
            int fSat2 = 0;
            int fSut1 = 0;
            int fSut2 = 0;
            int baslaSat = 0;
            int baslaSut = 0;
            XLS.HucreAdBulYaz("Yil", yil);

            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);
            VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBHMv2", 0);
            int eskiBirimSat = -1;
            int eskiTertipSat = -1;

            string eskiBirim = "";
            string eskiTertip = "";
            string eskiProje = "";
            ToplamAl toplamAl = new ToplamAl(5, 9);
            int satir = baslaSat;
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            while (true)
            {
                System.Data.DataRow row = vts.satirOku();
                if (row == null) break;

                string kyKod = vts.StringAl(0);
                string proje = vts.StringAl(1);
                string kurKod = vts.StringAl(2);
                string fonKod = vts.StringAl(3);
                string finKod = vts.StringAl(4);
                string eko2 = vts.StringAl(5);
                string eko3 = vts.StringAl(6);
                string pebAd = vts.StringAl(7);
                string birimAd = vts.StringAl(8);
                double odenek = vts.DoubleAl(9);
                if (Gecersiz(eko2)) continue;

                string tertip = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), kurKod) + "-" + OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), fonKod) + "-" + finKod + "-" + OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), eko2);
                if (eskiBirim != kyKod)
                {
                    if (eskiBirimSat > 0)
                    {
                        ToplamSatirYaz(XLS, eskiBirimSat, baslaSut + 3, toplamAl, 2);
                    }
                    XLS.HucreKopyala(fSat1 + 1, fSut1, fSat2 + 1, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut, birimAd);
                    eskiBirimSat = satir;
                    satir++;
                    eskiBirim = kyKod;
                }
                if (eskiProje != proje || eskiTertip != tertip)
                {
                    if (eskiTertipSat > 0)
                    {
                        ToplamSatirYaz(XLS, eskiTertipSat, baslaSut + 3, toplamAl, 3);
                    }
                    XLS.HucreKopyala(fSat1 + 2, fSut1, fSat2 + 2, fSut2, satir, baslaSut);
                    XLS.HucreDegerYaz(satir, baslaSut + 1, pebAd);
                    XLS.HucreDegerYaz(satir, baslaSut + 2, tertip);

                    XLS.HucreDegerYaz(satir, baslaSut + 25, kyKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 26, proje);
                    XLS.HucreDegerYaz(satir, baslaSut + 27, kurKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 28, fonKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 29, finKod);
                    XLS.HucreDegerYaz(satir, baslaSut + 30, eko2);
                    XLS.HucreDegerYaz(satir, baslaSut + 31, pebAd);
                    XLS.HucreDegerYaz(satir, baslaSut + 32, birimAd);

                    eskiTertipSat = satir;
                    satir++;
                    eskiProje = proje;
                    eskiTertip = tertip;
                }
                int yer = OrtakFonksiyonlar.ConvertToInt(eko3, 0) - 1;
                if (yer >= 0)
                {
                    toplamAl.DegerAta(4, yer, odenek);
                    toplamAl.DegerArttir(3, yer, odenek);
                    toplamAl.DegerArttir(2, yer, odenek);
                    toplamAl.DegerArttir(1, yer, odenek);
                    toplamAl.DegerArttir(0, yer, odenek);
                }
            }

            if (eskiBirimSat > 0)
            {
                ToplamSatirYaz(XLS, eskiBirimSat, baslaSut + 3, toplamAl, 2);
            }
            if (eskiTertipSat > 0)
            {
                ToplamSatirYaz(XLS, eskiTertipSat, baslaSut + 3, toplamAl, 3);
            }

            satir++;
            XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut, "Merkez Merkez Ödenekleri Toplamı");
            ToplamSatirYaz(XLS, satir, baslaSut + 3, toplamAl, 0);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void ToplamSatirYaz(Tablo XLS, int sat, int sut, ToplamAl toplamAl, int toplamSat)
        {
            for (int i = 0; i < 9; i++)
            {
                XLS.HucreDegerYaz(sat, sut + i, toplamAl.toplam[toplamSat, i]);
            }
            XLS.HucreDegerYaz(sat, sut + 9, toplamAl.SatToplamAl(toplamSat, 0, 8));
            toplamAl.SatirSifirla(toplamSat);
        }

        private string AvansKapat()
        {
            bool islemYap = false;
            if (yil < DateTime.Now.Year)
            {
                islemYap = true;
            }
            string sql = "SELECT  F.MUHASEBEKOD, F.BIRIMKOD, D.HESAPKOD, D.KURKOD, D.FONKOD, D.FINKOD,D.EKOKOD,D.PEBKOD, SUM(BORC), SUM(ALACAK) ";
            sql += "FROM ButceIslemleri.dbo.MUHMIFFORM AS F INNER JOIN ";
            sql += "ButceIslemleri.dbo.MUHMIF AS D ON F.YIL = D.YIL AND F.BELGENO = D.BELGENO ";
            sql += "WHERE     (F.YIL = " + yil.ToString() + ") AND (F.FORMDURUM < 9) AND HESAPKOD LIKE '16%' AND NOT (HESAPKOD LIKE '162%') ";
            sql += "and D.KURKOD<>'' AND D.FONKOD<>'' AND  D.FINKOD<>'' AND D.EKOKOD<>'' AND D.PEBKOD<>'' ";
            sql += "group by F.MUHASEBEKOD, F.BIRIMKOD, D.HESAPKOD, D.KURKOD, D.FONKOD, D.FINKOD,D.EKOKOD,D.PEBKOD ";
            sql += "having SUM(BORC)<>SUM(ALACAK) ";
            sql += "order by 1,2 ";

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.BosDosyaAc(sonucDosyaAd);
            XLS.HucreAdBulYaz("Yil", yil);

            VTSorgula vts = new VTSorgula(sql, "BaglantiSatiriBUM", 0);
            string eskiBirim = "";
            int satir = 1;
            TNS.MUH.OdemeEmriMIFForm mifForm = null;
            double toplamBorcAlacak = 0;
            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            while (true)
            {
                System.Data.DataRow row = vts.satirOku();
                if (row == null) break;

                int saymanlikKod = vts.IntAl(0);
                string birim = vts.StringAl(1);
                string birimKodHepsi = saymanlikKod + "-" + birim;
                string hesapKod = vts.StringAl(2);
                string kurKod = vts.StringAl(3);
                string fonKod = vts.StringAl(4);
                string finKod = vts.StringAl(5);
                string ekoKod = vts.StringAl(6);
                string pebKod = vts.StringAl(7);
                double borc = vts.DoubleAl(8);
                double alacak = vts.DoubleAl(9);
                if (eskiBirim != birimKodHepsi)
                {
                    if (mifForm != null && islemYap)
                    {
                        if (toplamBorcAlacak != 0)
                        {
                            TNS.MUH.OdemeEmriMIFDetay dt = new OdemeEmriMIFDetay(yil, "103.02", toplamBorcAlacak > 0 ? toplamBorcAlacak : 0, toplamBorcAlacak < 0 ? 0 - toplamBorcAlacak : 0);
                            mifForm.detay.Ekle(dt);
                        }
                        Sonuc s = servisMUH.OdemeEmriMIFKaydet(kullanan, mifForm);
                        if (!s.islemSonuc)
                        {
                            throw new Exception(s.hataStr);
                        }
                    }
                    mifForm = new OdemeEmriMIFForm();
                    mifForm.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                    mifForm.ilgiliNo = mifForm.kurum;
                    mifForm.yil = yil;
                    mifForm.islemTarih = new TNSDateTime("31.12." + yil.ToString());
                    mifForm.muhasebe = saymanlikKod;
                    mifForm.birim = birim;
                    mifForm.kurKod = kurKod;
                    mifForm.fisTurYap(ENUMFISTUR.KAPANIS);
                    mifForm.odenekKontroluYapma = 9;
                    eskiBirim = birimKodHepsi;
                    toplamBorcAlacak = 0;
                }
                XLS.HucreDegerYaz(satir, 1, saymanlikKod);
                XLS.HucreDegerYaz(satir, 2, birim);
                XLS.HucreDegerYaz(satir, 3, hesapKod);
                XLS.HucreDegerYaz(satir, 4, kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod + "-" + pebKod);
                XLS.HucreDegerYaz(satir, 5, borc);
                XLS.HucreDegerYaz(satir, 6, alacak);
                if (borc != alacak)
                {
                    double fark = borc - alacak;
                    toplamBorcAlacak += fark;
                    TNS.MUH.OdemeEmriMIFDetay dt = new OdemeEmriMIFDetay(yil, hesapKod, fark < 0 ? 0 - fark : 0, fark > 0 ? fark : 0);
                    dt.kur = kurKod;
                    dt.fon = fonKod;
                    dt.fin = finKod;
                    dt.eko = ekoKod;
                    dt.peb = pebKod;
                    mifForm.detay.Ekle(dt);
                }
                satir++;
            }
            if (mifForm != null && islemYap)
            {
                if (toplamBorcAlacak != 0)
                {
                    TNS.MUH.OdemeEmriMIFDetay dt = new OdemeEmriMIFDetay(yil, "103.02", toplamBorcAlacak > 0 ? toplamBorcAlacak : 0, toplamBorcAlacak < 0 ? 0 - toplamBorcAlacak : 0);
                    mifForm.detay.Ekle(dt);
                }
                Sonuc s = servisMUH.OdemeEmriMIFKaydet(kullanan, mifForm);
                if (!s.islemSonuc)
                {
                    throw new Exception(s.hataStr);
                }

            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>
        /// Program ve EKonomik koda göre İcmal hazırlayan rapor
        /// </summary>
        /// <param name="IcmalTur">1 ise Program, 2 ise Ekonomik rapor hazırlar.</param>
        /// <returns></returns>
        private string OdenekIcmali(int IcmalTur)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0, fSat1 = 0, fSat2 = 0, fSut1 = 0, fSut2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);

            int baslaSat = 0, baslaSut = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);

            XLS.HucreAdBulYaz("Baslik", $"{yil} YILI {(IcmalTur == 1 ? "PROGRAM BÜTÇE" : "EKONOMİK I. DÜZEY")} ÖDENEK İCMALİ");

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPRGOdenek, "", "", "", "", "", TNS.DEG.Arac.VarsayilanKurumBul());
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 0, "0600");

            int okuAlan;
            if (IcmalTur == 1)
            {
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), 1));
                okuAlan = 1;
            }
            else
            {
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(7), 1));
                okuAlan = 7;
            }

            OdenekBilgi[] obler = servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true, false);
            int satir = baslaSat;
            double toplam = 0;
            int satYuk = XLS.SatirYukseklikAl(baslaSat);
            for (int i = 0; i < obler.Length; i++)
            {
                OdenekBilgi f = obler[i];
                string kod = f.tertip.kodlar[okuAlan];
                string ad = f.tertip.adlar[okuAlan];
                double odenek = f.EkRakam(0);

                int fark = (i) % 2;

                XLS.HucreKopyala(fSat1 + fark, fSut1, fSat2 + fark, fSut2, satir, baslaSut);
                XLS.HucreDegerYaz(satir, baslaSut, kod);
                XLS.HucreDegerYaz(satir, baslaSut + 1, ad);
                XLS.HucreDegerYaz(satir, baslaSut + 2, odenek);
                toplam += odenek;
                satir++;
            }

            XLS.HucreKopyala(fSat1 + 2, fSut1, fSat2 + 2, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut + 2, toplam);
            XLS.SatirYukseklikAyarla(baslaSat, satir, -1, 5, satYuk);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>
        /// Program ve EKonomik koda göre İcmal hazırlayan rapor
        /// </summary>
        /// <param name="IcmalTur">1 ise Program, 2 ise Ekonomik rapor hazırlar.</param>
        /// <returns></returns>
        private string OdenekIcmaliEko2()
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0, fSat1 = 0, fSat2 = 0, fSut1 = 0, fSut2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);

            int baslaSat = 0, baslaSut = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);

            XLS.HucreAdBulYaz("Baslik", $"{yil} YILI EKONOMİK II. DÜZEY ÖDENEK İCMALİ");

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPRGOdenek, "", "", "", "", "", TNS.DEG.Arac.VarsayilanKurumBul());
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 0, "0600");
            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(7), 2));

            OdenekBilgi[] obler = servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true, false);
            int satir = baslaSat;
            double toplam = 0;
            int satYuk = XLS.SatirYukseklikAl(baslaSat);
            string oncekiIlkKir = "";
            int eko1Say = -1;
            int oncekiBasla = -1;
            for (int i = 0; i < obler.Length; i++)
            {
                OdenekBilgi f = obler[i];
                string kod = f.tertip.kodlar[7];
                string ilkKir = kod.Substring(0, 2);
                if (oncekiIlkKir != ilkKir)
                {
                    eko1Say++;
                    if (oncekiBasla != -1)
                    {
                        XLS.HucreBirlestir(oncekiBasla, 0, satir - 1, 0);
                        XLS.HucreBirlestir(oncekiBasla, 1, satir - 1, 1);
                    }
                }

                int fark = (eko1Say) % 2;

                XLS.HucreKopyala(fSat1 + fark, fSut1, fSat2 + fark, fSut2, satir, baslaSut);

                string ad = f.tertip.adlar[7];
                double odenek = f.EkRakam(0);
                if (oncekiIlkKir != ilkKir)
                {
                    oncekiBasla = satir;
                    XLS.HucreDegerYaz(satir, baslaSut + 0, ilkKir);
                    XLS.HucreDegerYaz(satir, baslaSut + 1, servisUZY.UzayDegeriStr(kullanan, "ABSEKO", yil.ToString() + "-" + ilkKir, true));
                    oncekiIlkKir = ilkKir;
                }

                XLS.HucreDegerYaz(satir, baslaSut + 2, kod.Substring(3, 2));
                XLS.HucreDegerYaz(satir, baslaSut + 3, ad);
                XLS.HucreDegerYaz(satir, baslaSut + 4, odenek);
                toplam += odenek;
                satir++;
            }

            if (oncekiBasla != -1)
            {
                XLS.HucreBirlestir(oncekiBasla, 0, satir - 1, 0);
                XLS.HucreBirlestir(oncekiBasla, 1, satir - 1, 1);
            }

            XLS.HucreKopyala(fSat1 + 2, fSut1, fSat2 + 2, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, baslaSut + 4, toplam);
            XLS.SatirYukseklikAyarla(baslaSat, satir, -1, 5, satYuk);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private class OdenekIcmalCariBilgi : IComparable<OdenekIcmalCariBilgi>
        {
            internal string birimAd;
            internal string sektorAd;
            internal string projeKodu;
            internal string faalKod;
            internal string faalAd;
            internal string eko1;
            internal double[] ekoDegerDizi;
            internal double ekoToplam;

            public OdenekIcmalCariBilgi()
            {
                this.birimAd = "";
                this.sektorAd = "";
                this.projeKodu = "";
                this.faalKod = "";
                this.faalAd = "";
                this.eko1 = "";
                this.ekoDegerDizi = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                this.ekoToplam = 0;
            }

            public int CompareTo(OdenekIcmalCariBilgi other)
            {
                int tut = this.birimAd.CompareTo(other.birimAd);
                if (tut != 0) return tut;

                tut = this.sektorAd.CompareTo(other.sektorAd);
                if (tut != 0) return tut;

                tut = this.projeKodu.CompareTo(other.projeKodu);
                if (tut != 0) return tut;

                tut = this.faalAd.CompareTo(other.faalAd);
                if (tut != 0) return tut;

                tut = this.eko1.CompareTo(other.eko1);
                if (tut != 0) return tut;

                tut = other.ekoToplam.CompareTo(this.ekoToplam);
                return tut;
            }

            internal void EkoDegerAta(int indis, double odenek)
            {
                this.ekoDegerDizi[indis] = odenek;
            }
        }

        /// <summary>
        /// Program ve EKonomik koda göre İcmal hazırlayan rapor
        /// </summary>
        /// <param name="IcmalTur">1 ise Program, 2 ise Ekonomik rapor hazırlar.</param>
        /// <returns></returns>
        private string OdenekIcmaliCari(string ekoKodlar)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int sheetNo = 0, fSat1 = 0, fSat2 = 0, fSut1 = 0, fSut2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref fSat1, ref fSut1, ref fSat2, ref fSut2);

            int baslaSat = 0, baslaSut = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSat, ref baslaSut);

            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("Baslik", BaslikBul(yil, ekoKodlar));

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", ekoKodlar);
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, "0600");
            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
            IYATServis servisYAT = TNS.YAT.Arac.Tanimla();
            int faalSon = kistasTertip.KodNoKirilimSay(4);
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), faalSon));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2));

            UcgenSatir[] obler = servisBHMv2.UcgenTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, true);
            int satir = baslaSat;
            if (ekoKodlar.Length == 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    XLS.HucreDegerYaz(satir - 1, 5 + i, ekoKodlar + "." + (i + 1).ToString("00"));
                }
            }

            double toplam = 0;
            double[] toplamDizi = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            List<OdenekIcmalCariBilgi> bilgiler = new List<OdenekIcmalCariBilgi>();
            OdenekIcmalCariBilgi aktifBilgi = null;
            string sonEko1 = "";
            string birimAd = "";
            string faalAd = "";
            string[] faalDizi = { "", "", "", "" };
            string faalKod = "";
            string sektorAd = "";
            string projeKodu = "";
            for (int i = 0; i < obler.Length; i++)
            {
                UcgenSatir f = obler[i];
                double odenek = (double)f.ekDegerler[0];
                string kod = f.kodParca;

                if (f.yer == 0)
                {
                    birimAd = servisUZY.UzayDegeriStr(kullanan, "ABSPEBKOD", yil.ToString() + "-" + kod, true, "PEBAD2");
                    //birimAd = f.aciklama;
                    DiziDegerAta(faalDizi, f.yer, f.kodParca);
                }
                else if (f.yer > 0 && f.yer < faalSon - 1)
                {
                    DiziDegerAta(faalDizi, f.yer, f.kodParca);
                }
                else if (f.yer == faalSon - 1)
                {
                    DiziDegerAta(faalDizi, f.yer, f.kodParca);
                    faalKod = DiziDegerOl(faalDizi);
                    faalAd = f.aciklama;
                    if (ekoKodlar == "06" || ekoKodlar == "07")
                    {
                        ObjectArray oa = servisYAT.ProjeListeleFaal(kullanan, yil, "06.00", faalKod);
                        if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 1)
                        {
                            Proje gelen = (Proje)oa.objeler[0];
                            sektorAd = gelen.sektor.ad;
                            projeKodu = gelen.DPTProjeNo;
                        }
                        else
                        {
                            sektorAd = "Bulunamadı";
                            projeKodu = faalKod + " PEB";
                        }
                    }
                }
                else if (f.yer == faalSon)
                {
                    if (sonEko1 != "" || aktifBilgi == null)
                    {
                        aktifBilgi = new OdenekIcmalCariBilgi();
                        bilgiler.Add(aktifBilgi);
                    }
                    sonEko1 = kod;
                    aktifBilgi.birimAd = birimAd;
                    aktifBilgi.sektorAd = sektorAd;
                    aktifBilgi.projeKodu = projeKodu;
                    aktifBilgi.faalAd = faalAd;
                    aktifBilgi.faalKod = faalKod;
                    aktifBilgi.eko1 = kod;
                    aktifBilgi.ekoToplam = odenek;
                    toplam += odenek;
                }
                else if (f.yer > faalSon)
                {
                    int eko2No = OrtakFonksiyonlar.ConvertToInt(f.kodParca, 1);
                    toplamDizi[eko2No - 1] += odenek;
                    aktifBilgi.EkoDegerAta(eko2No - 1, odenek);
                }
            }
            bilgiler.Sort();
            int satYuk = XLS.SatirYukseklikAl(baslaSat);
            //string oncekiIlkKir = "";
            //int eko1Say = -1;
            //int oncekiBasla = -1;
            int oncekiBirimYer = -1;
            int oncekiSektorYer = -1;
            int oncekiProjeYer = -1;
            int oncekiFaaliyetYer = -1;
            OdenekIcmalCariBilgi onceki = null;
            foreach (OdenekIcmalCariBilgi bilgi in bilgiler)
            {
                XLS.HucreKopyala(fSat1, fSut1, fSat2, fSut2, satir, baslaSut);
                if (!BirimAdAyniMi(onceki, bilgi))
                {
                    if (oncekiBirimYer > -1)
                    {
                        XLS.HucreBirlestir(oncekiBirimYer, 0, satir - 1, 0);
                    }
                    oncekiBirimYer = satir;
                    XLS.HucreDegerYaz(satir, 0, bilgi.birimAd);
                }
                if (!SektorAyniMi(onceki, bilgi))
                {
                    if (oncekiSektorYer > -1)
                    {
                        XLS.HucreBirlestir(oncekiSektorYer, 1, satir - 1, 1);
                    }
                    oncekiSektorYer = satir;
                    XLS.HucreDegerYaz(satir, 1, bilgi.sektorAd);
                }
                if (!ProjeAyniMi(onceki, bilgi))
                {
                    if (oncekiProjeYer > -1)
                    {
                        XLS.HucreBirlestir(oncekiProjeYer, 2, satir - 1, 2);
                    }
                    oncekiProjeYer = satir;
                    XLS.HucreDegerYaz(satir, 2, bilgi.projeKodu);
                }
                if (!FaaliyetAyniMi(onceki, bilgi))
                {
                    if (oncekiFaaliyetYer > -1)
                    {
                        XLS.HucreBirlestir(oncekiFaaliyetYer, 3, satir - 1, 3);
                    }
                    oncekiFaaliyetYer = satir;
                    XLS.HucreDegerYaz(satir, 3, bilgi.faalAd);
                }
                XLS.HucreDegerYaz(satir, 4, bilgi.eko1);
                for (int i = 0; i < 10; i++)
                {
                    XLS.HucreDegerYaz(satir, 5 + i, bilgi.ekoDegerDizi[i]);
                }
                XLS.HucreDegerYaz(satir++, 15, bilgi.ekoToplam);
                onceki = bilgi;
            }
            if (oncekiFaaliyetYer > -1)
            {
                XLS.HucreBirlestir(oncekiFaaliyetYer, 3, satir - 1, 3);
            }
            if (oncekiProjeYer > -1)
            {
                XLS.HucreBirlestir(oncekiProjeYer, 2, satir - 1, 2);
            }
            if (oncekiSektorYer > -1)
            {
                XLS.HucreBirlestir(oncekiSektorYer, 1, satir - 1, 1);
            }
            if (oncekiBirimYer > -1)
            {
                XLS.HucreBirlestir(oncekiBirimYer, 0, satir - 1, 0);
            }
            XLS.HucreKopyala(fSat1 + 1, fSut1, fSat2 + 1, fSut2, satir, baslaSut);
            XLS.HucreDegerYaz(satir, 15, toplam);
            int silinen = 0;
            for (int i = toplamDizi.Length - 1; i >= 0; i--)
            {
                if (toplamDizi[i] > 0)
                    XLS.HucreDegerYaz(satir, 5 + i, toplamDizi[i]);
                else
                {
                    XLS.SutunSil(5 + i, 5 + i);
                    silinen++;
                }
            }
            if (silinen == 9)
            {
                XLS.SutunSil(5, 5);
            }
            if (ekoKodlar.Length == 2)
            {
                XLS.SutunSil(4, 4);
            }
            if (ekoKodlar != "06" && ekoKodlar != "07")
            {
                XLS.SutunSil(1, 2);
            }

            XLS.SatirYukseklikAyarla(baslaSat, satir, -1, 5, satYuk);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private bool BirimAdAyniMi(OdenekIcmalCariBilgi onceki, OdenekIcmalCariBilgi bilgi)
        {
            if (onceki == null || bilgi == null)
                return false;
            return onceki.birimAd.Equals(bilgi.birimAd);
        }

        private bool SektorAyniMi(OdenekIcmalCariBilgi onceki, OdenekIcmalCariBilgi bilgi)
        {
            if (!BirimAdAyniMi(onceki, bilgi)) return false;
            return onceki.sektorAd.Equals(bilgi.sektorAd);
        }

        private bool ProjeAyniMi(OdenekIcmalCariBilgi onceki, OdenekIcmalCariBilgi bilgi)
        {
            if (!SektorAyniMi(onceki, bilgi)) return false;
            return onceki.projeKodu.Equals(bilgi.projeKodu);
        }

        private bool FaaliyetAyniMi(OdenekIcmalCariBilgi onceki, OdenekIcmalCariBilgi bilgi)
        {
            if (!ProjeAyniMi(onceki, bilgi)) return false;
            return onceki.faalAd.Equals(bilgi.faalAd);
        }

        private string BaslikBul(int yil, string ekoKodlar)
        {
            if (ekoKodlar == "01,02,05,08")
                return $"{yil} YILI DİĞER CARİ ÖDENEKLER (EKONOMİK 01-02-05-08 KODLARI)";
            else if (ekoKodlar == "03")
                return $"{yil} YILI CARİ ÖDENEKLERİ (EKONOMİK 03 KODU)";
            else if (ekoKodlar == "06")
                return $"{yil} YILI YATIRIM (SERMAYE GİDERLERİ) ÖDENEKLERİ (EKONOMİK 06 KODU)";
            else if (ekoKodlar == "07")
                return $"{yil} YILI SERMAYE TRANSFERİ ÖDENEKLERİ (EKONOMİK 07 KODU)";
            else
                return "TANIMSIZ";
        }

        private string DiziDegerOl(string[] faalDizi)
        {
            string cevap = "";
            for (int i = 0; i < faalDizi.Length; i++)
            {
                if (faalDizi[i] == "") break;
                if (cevap != "") cevap += ".";
                cevap += faalDizi[i];
            }
            return cevap;
        }

        private void DiziDegerAta(string[] faalDizi, int yer, string kodParca)
        {
            faalDizi[yer] = kodParca;
            for (int i = yer + 1; i < faalDizi.Length; i++)
            {
                faalDizi[i] = "";
            }
        }

        public string RaporAl(object kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUz.KodUzunlukBul(3, 2, true) - tUz.KodUzunlukBul(3, 1, true);
            this.kullanan = (TNS.KYM.Kullanici)kullanan;
            this.sablonAd = sablonAd;
            this.sonucDosyaAd = sonucDosyaAd;
            this.raporParam = raporParam;
            this.servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            if (ekBilgi == "2")
                return TasraToplam();
            else if (ekBilgi == "1")
                return TasraDetay(eko2Uz);
            else if (ekBilgi == "3")
                return MerkezDetay(eko2Uz);
            else if (ekBilgi == "4")
                return MerkezToplam();
            else if (ekBilgi == "5")
                return MerkezMerkezDetay(eko2Uz);
            else if (ekBilgi == "6")
                return MerkezMerkezToplam();

            else if (ekBilgi == "7")
                return OdenekIcmali(1);
            else if (ekBilgi == "8")
                return OdenekIcmali(2);
            else if (ekBilgi == "9")
                return OdenekIcmaliEko2();
            else if (ekBilgi == "10")
                return OdenekIcmaliCari("01,02,05,08");
            else if (ekBilgi == "11")
                return OdenekIcmaliCari("03");
            else if (ekBilgi == "12")
                return OdenekIcmaliCari("06");
            else if (ekBilgi == "13")
                return OdenekIcmaliCari("07");

            else if (ekBilgi == "99")
                return AvansKapat();
            throw new Exception("Tanımsız ");
        }
    }
}

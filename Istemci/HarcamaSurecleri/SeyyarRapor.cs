using System;
using System.Collections.Generic;
using System.Linq;
using TNS.BHMv2;
using TNS.KYM;
using TNS;
using TNS.HRC;
using TNS.UZY;
using OrtakClass;
using System.Collections;

namespace HarcamaSurecleri
{
    /// <summary>
    /// Seyyar görev yolluğu (adam/gün) raporları (Rapor.xml NESNE "HarcamaSurecleri.SeyyarRapor"; EKBILGI: ILTOP, ILDET, BIRIM,
    /// DETAYREVIZE, DETAYDURUM, TOPLAMREVIZE, TOPLAMREVIZEIL, UNVANLISTE).
    /// Eski HarcamaSurecleri/SeyyarRapor.cs'ten alındı; değişenler: System.Web ve TNS.Raporlama.IRapor arayüzü kaldırıldı.
    /// </summary>
    public class SeyyarRapor
    {
        private string KurKod;
        private string Muhasebe;
        private string Birim;
        private IBHMv2Servis servisBHMv2;
        private IHRCServis servisHRC;
        private int yil;
        private int asama;
        private string asamaAd;
        private string Ilkod;
        private string kyKod;
        private int istihdamSekli;
        private Kullanici kullanan;
        private string sablonAd;
        private string sonucDosyaAd;

        public static KurumsalYapi KurumsalYapiGetir(IBHMv2Servis servis, string harcamaBirimi)
        {
            KurumsalYapi ky = servis.KurumsalYapiDondur(System.Threading.Thread.CurrentThread.CurrentCulture.Name, harcamaBirimi.Trim());
            return ky;
        }

        public static string Yazdir(IBHMv2Servis servis, IHRCServis servisHRC, IUZYServis servisUZY, Kullanici kullanan, int pYil, string pKYKod, string pKYAd, int pAsama, string pAsamaAdi, string sablonAd, string altBilgi, string sonucDosyaAd)
        {
            int yil = pYil;
            string harcamabirimi = pKYKod;
            string harcamabirimiAdi = pKYAd;
            int asama = pAsama;
            string asamaAdi = pAsamaAdi;
            ObjectArray donusDeger = servisHRC.SeyyarGorevAdamGunTespitListele(kullanan, yil, harcamabirimi, asama, true, true, false);

            if (!donusDeger.sonuc.islemSonuc)
            {
                throw new Exception(donusDeger.sonuc.hataStr);
            }
            if (donusDeger.objeler.Count > 0)
            {
                Tablo XLS = GenelIslemler.NewTablo();
                int satir = 0;
                int sutun = 0;
                int kaynakSatir = 0;

                sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
                XLS.DosyaAc(sablonAd, sonucDosyaAd);
                XLS.HucreAdAdresCoz("FormatSatir", ref kaynakSatir, ref sutun);
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

                XLS.HucreAdBulYaz("Yil", ":" + yil);
                XLS.HucreAdBulYaz("Asama", ":" + asamaAdi);
                XLS.HucreAdBulYaz("SimdikiYilKurumTeklifi", yil + " Yılı Kurum Teklifi");
                XLS.HucreAdBulYaz("SimdikiYilBirimTeklifi", yil + " Yılı Birim Teklifi");
                XLS.HucreAdBulYaz("OncekiYilVizeliAg", yil - 1 + " Yılı Vizeli A/G");
                XLS.HucreAdBulYaz("OncekiYilKullanilanAg", yil - 1 + " Yılı Kullanılan A/G");

                int yuks = XLS.SatirYukseklikAl(satir);
                for (int i = 0; i < donusDeger.objeler.Count; i++)
                {
                    SeyyarGorevI sg = (SeyyarGorevI)donusDeger.objeler[i];
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 23, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, i + 1);

                    XLS.HucreDegerYaz(satir, 1, sg.harcamaBirimiKodu.Substring(2, 2));
                    XLS.HucreDegerYaz(satir, 2, sg.harcamaBirimiKodu);
                    XLS.HucreDegerYaz(satir, 3, sg.harcamaBirimiAdi);
                    int sut = 4;
                    XLS.HucreDegerYaz(satir, sut++, sg.memur);
                    XLS.HucreDegerYaz(satir, sut++, sg.isci);
                    XLS.HucreDegerYaz(satir, sut++, sg.sozlesmeli);

                    XLS.HucreDegerYaz(satir, sut++, sg.oncAsamaMemur);
                    XLS.HucreDegerYaz(satir, sut++, sg.oncAsamaIsci);
                    XLS.HucreDegerYaz(satir, sut++, sg.oncAsamaSozlesmeli);

                    XLS.HucreDegerYaz(satir, sut++, sg.oncVizeliMemur);
                    XLS.HucreDegerYaz(satir, sut++, sg.oncVizeliIsci);
                    XLS.HucreDegerYaz(satir, sut++, sg.oncVizeliSozlesmeli);

                    XLS.HucreDegerYaz(satir, sut++, sg.oncKullanilanMemur);
                    XLS.HucreDegerYaz(satir, sut++, sg.oncKullanilanIsci);
                    XLS.HucreDegerYaz(satir, sut++, sg.oncKullanilanSozlesmeli);

                    XLS.HucreDegerYaz(satir, sut++, sg.oncekiYilKullanilanTutar);

                    XLS.HucreDegerYaz(satir, sut++, sg.personelSayisiMemur);
                    XLS.HucreDegerYaz(satir, sut++, sg.personelSayisiIsci);
                    XLS.HucreDegerYaz(satir, sut++, sg.personelSayisiSozlesmeli);
                    XLS.SatirYukseklikAyarla(satir, satir, -1, 7, yuks);
                    satir++;
                }
                XLS.HFDegerAta(1, 1, altBilgi);
                XLS.DosyaSaklaTamYol();
                return sonucDosyaAd;
            }
            else
                throw new Exception("Kayıt bulunamadı");
        }

        private static string HarcamaBirimiKodYapisi()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "KODYAPI/KOD17/KODYAPI");
        }

        public string RaporAlIlDetay()
        {
            string[] sorgular = new string[3];
            string[] baglantilar = new string[3];
            string ilKoduAlan = TNS.HRC.Arac.KYYapiIlKosul();
            string ilSorgu = "";
            if (!string.IsNullOrWhiteSpace(Ilkod))
                ilSorgu = " AND " + ilKoduAlan + "='" + Ilkod + "'";
            string belgeNoSorgu;
            if (asama == 4)
            {
                belgeNoSorgu = "";
                asama = 3;
            }
            else
            {
                belgeNoSorgu = "AND BELGENO = 0";
            }

            string odenekSorgu = "SELECT " + ilKoduAlan + ", IL.AD, SUBSTRING(HARCAMABIRIMI,1,1), MUH.MUHASEBEAD, HARCAMABIRIMI, BIR.BIRIMAD, GUNSAYISI " +
            "FROM HRCSEYYARGOREVPLANLANAN " +
            " left outer join BHMKURUMSALYAPI AS IL ON IL.KOD = '2' + " + ilKoduAlan +
            " left outer join BHMKURUMSALYAPI KY ON KY.KOD = HARCAMABIRIMI" +
            " left outer join HRCMUHASEBEVIEW MUH ON MUH.MUHASEBEKOD = KY.SAYMANLIKKOD" +
            " left outer join HRCMUHASEBEBIRIMVIEW BIR ON BIR.MUHASEBEKOD = KY.SAYMANLIKKOD AND BIR.BIRIMKOD = KY.BIRIMKOD AND BIR.KURKOD = KY.KURKOD " +
            "where YIL={0} AND ASAMA={1} AND ISTIHDAMSEKLI={2} {3} {4}" +
             " order by " + ilKoduAlan + ", 2, 3, 5";

            sorgular[0] = string.Format(odenekSorgu, yil, asama, "1", ilSorgu, belgeNoSorgu);
            baglantilar[0] = "BaglantiSatiriHRC";
            sorgular[1] = string.Format(odenekSorgu, yil, asama, "4", ilSorgu, belgeNoSorgu);
            baglantilar[1] = "BaglantiSatiriHRC";
            sorgular[2] = string.Format(odenekSorgu, yil, asama, "2", ilSorgu, belgeNoSorgu);
            baglantilar[2] = "BaglantiSatiriHRC";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0, 1, 2, 3, 4, 5 });

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = satir;
            int kaynakSatir2 = 0; int sutun2 = 0; int sheetNo = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref kaynakSatir, ref sutun, ref kaynakSatir2, ref sutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            satir = baslaSatir;
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("AsamaAd", asamaAd);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreAdBulYaz("ButceTur", "Genel Bütçe");

            int yukseklik = XLS.SatirYukseklikAl(baslaSatir - 2);
            ToplamAl toplam = new ToplamAl(3, 4);
            string eskiIlAd = null;
            int formulSat = 0;
            int ilIlkSatir = 0;
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string ilKod = alAnahtar.StrAl(0);
                string ilAd = alAnahtar.StrAl(1);
                string birimTur = alAnahtar.StrAl(2);
                string muhAd = alAnahtar.StrAl(3);
                string birimKod = alAnahtar.StrAl(4);
                string birimAd = alAnahtar.StrAl(5);

                if (ilAd == "") ilAd = "Merkez - Tutulu";

                if (eskiIlAd != ilAd)
                {
                    if (eskiIlAd != null)
                    {
                        ToplamYaz(XLS, ilIlkSatir, ref satir, yukseklik, sutun, sutun2, toplam, 1, eskiIlAd, formulSat, kaynakSatir);
                        toplam.SatirSifirla(1);
                        XLS.SayfaSonuKoy(satir);
                        if (formulSat == 0)
                            formulSat = satir - 5;
                    }
                    ilIlkSatir = satir;
                    eskiIlAd = ilAd;
                }
                toplam.toplam[2, 0] = alVeri.DblAl(0);
                toplam.toplam[2, 1] = alVeri.DblAl(1);
                toplam.toplam[2, 2] = alVeri.DblAl(2);
                toplam.toplam[2, 3] = toplam.toplam[2, 0] + toplam.toplam[2, 1] + toplam.toplam[2, 2];

                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun2, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, ilAd);
                XLS.HucreDegerYaz(satir, sutun + 1, TNS.BHMv2.Arac.BirimAdOl(muhAd, birimAd));
                RakamYaz(XLS, satir, sutun + 3, toplam, 2);
                satir++;
                toplam.ToplamEkle(1, 2);
                toplam.ToplamEkle(0, 2);
            }

            if (eskiIlAd != null)
            {
                ToplamYaz(XLS, ilIlkSatir, ref satir, yukseklik, sutun, sutun2, toplam, 1, eskiIlAd, formulSat, kaynakSatir);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void ToplamYaz(Tablo XLS, int ilIlkSatir, ref int satir, int yukseklik, int sutun, int sutun2, ToplamAl toplam, int toplamSatir, string ilAd, int formulSat, int kaynakSatir)
        {
            XLS.HucreKopyala(kaynakSatir + 1, sutun, kaynakSatir + 1, sutun2, satir, sutun);
            XLS.HucreDegerYaz(satir, sutun + 0, "Toplam " + ilAd);

            RakamYaz(XLS, satir, sutun + 3, toplam, toplamSatir);
            XLS.SatirYukseklikAyarla(ilIlkSatir, satir, yukseklik);
            satir += 2;

            XLS.HucreKopyala(kaynakSatir + 3, sutun, kaynakSatir + 8, sutun2, satir, sutun);
            if (formulSat > 0)
            {
                XLS.HucreFormulYaz(satir, sutun, "=A" + (formulSat).ToString());

                XLS.HucreFormulYaz(satir + 3, sutun, "=A" + (formulSat + 3).ToString());
                XLS.HucreFormulYaz(satir + 3, sutun + 1, "=B" + (formulSat + 3).ToString());
                XLS.HucreFormulYaz(satir + 3, sutun + 2, "=C" + (formulSat + 3).ToString());
                XLS.HucreFormulYaz(satir + 3, sutun + 3, "=D" + (formulSat + 3).ToString());

                XLS.HucreFormulYaz(satir + 4, sutun, "=A" + (formulSat + 4).ToString());
                XLS.HucreFormulYaz(satir + 4, sutun + 1, "=B" + (formulSat + 4).ToString());
                XLS.HucreFormulYaz(satir + 4, sutun + 2, "=C" + (formulSat + 4).ToString());
                XLS.HucreFormulYaz(satir + 4, sutun + 3, "=D" + (formulSat + 4).ToString());

                XLS.HucreFormulYaz(satir + 5, sutun, "=A" + (formulSat + 5).ToString());
                XLS.HucreFormulYaz(satir + 5, sutun + 1, "=B" + (formulSat + 5).ToString());
                XLS.HucreFormulYaz(satir + 5, sutun + 2, "=C" + (formulSat + 5).ToString());
                XLS.HucreFormulYaz(satir + 5, sutun + 3, "=D" + (formulSat + 5).ToString());
            }
            satir += 6;
        }

        private void RakamYaz(Tablo XLS, int satir, int sutun, ToplamAl toplam, int toplamSatir)
        {
            XLS.HucreDegerYaz(satir, sutun + 0, toplam.toplam[toplamSatir, 0]);
            XLS.HucreDegerYaz(satir, sutun + 1, toplam.toplam[toplamSatir, 1]);
            XLS.HucreDegerYaz(satir, sutun + 2, toplam.toplam[toplamSatir, 2]);
            XLS.HucreDegerYaz(satir, sutun + 3, toplam.toplam[toplamSatir, 3]);
        }

        public string RaporAlIlTop()
        {
            string[] sorgular = new string[4];
            string[] baglantilar = new string[4];
            string ilKoduAlan = TNS.HRC.Arac.KYYapiIlKosul();
            string belgeNoSorgu;
            if (asama == 4)
            {
                belgeNoSorgu = "";
                asama = 3;
            }
            else
            {
                belgeNoSorgu = "AND BELGENO = 0";
            }

            string odenekSorgu = "SELECT " + ilKoduAlan + " AS ILKOD, SUM(GUNSAYISI) FROM HRCSEYYARGOREVPLANLANAN"
                + " where YIL={0} AND ASAMA={1} AND ISTIHDAMSEKLI={2} {3} group by " + ilKoduAlan + " order by 1";

            sorgular[0] = string.Format(odenekSorgu, yil, asama, "1", belgeNoSorgu);
            baglantilar[0] = "BaglantiSatiriHRC";

            sorgular[1] = string.Format(odenekSorgu, yil, asama, "4", belgeNoSorgu);
            baglantilar[1] = "BaglantiSatiriHRC";

            sorgular[2] = string.Format(odenekSorgu, yil, asama, "2", belgeNoSorgu);
            baglantilar[2] = "BaglantiSatiriHRC";

            sorgular[3] = "SELECT KOD, AD FROM HRCIL order by 1";
            baglantilar[3] = "BaglantiSatiriHRC";

            TNS.UZY.IUZYServis servis = TNS.UZY.Arac.Tanimla();

            SortedList ht = servis.SorguBirlestir(sorgular, baglantilar, new int[] { 0 });

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = satir;
            int kaynakSatir2 = 0; int sutun2 = 0; int sheetNo = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref kaynakSatir, ref sutun, ref kaynakSatir2, ref sutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            satir = baslaSatir;
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("AsamaAd", asamaAd);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreAdBulYaz("ButceTur", "Genel Bütçe");


            int yukseklik = XLS.SatirYukseklikAl(baslaSatir - 2);
            double memurTop = 0;
            double isciTop = 0;
            double sozlesmeliTop = 0;
            foreach (DictionaryEntry de in ht)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string ilKod = alAnahtar.StrAl(0);
                double memur = alVeri.DblAl(0);
                double isci = alVeri.DblAl(1);
                double sozlesmeli = alVeri.DblAl(2);
                string ilAd = alVeri.StrAl(3);

                //if (string.IsNullOrWhiteSpace(ilAd)) ilAd = alVeri.StrAl(2);
                //if (string.IsNullOrWhiteSpace(ilAd)) ilAd = alVeri.StrAl(4);

                if (ilAd == "" || ilKod == "99") ilAd = "Merkez - Tutulu";

                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun2, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 0, ilAd);
                XLS.HucreDegerYaz(satir, sutun + 1, memur);
                XLS.HucreDegerYaz(satir, sutun + 2, isci);
                XLS.HucreDegerYaz(satir, sutun + 3, sozlesmeli);
                XLS.HucreDegerYaz(satir, sutun + 4, memur + isci + sozlesmeli);
                memurTop += memur;
                isciTop += isci;
                sozlesmeliTop += sozlesmeli;
                satir++;
            }
            XLS.HucreKopyala(kaynakSatir + 2, sutun, kaynakSatir + 2, sutun2, satir, sutun);
            XLS.HucreDegerYaz(satir, sutun + 0, "T O P L A M");
            XLS.HucreDegerYaz(satir, sutun + 1, memurTop);
            XLS.HucreDegerYaz(satir, sutun + 2, isciTop);
            XLS.HucreDegerYaz(satir, sutun + 3, sozlesmeliTop);
            XLS.HucreDegerYaz(satir, sutun + 4, memurTop + isciTop + sozlesmeliTop);
            XLS.SatirYukseklikAyarla(baslaSatir, satir, yukseklik);
            satir += 4;

            XLS.HucreKopyala(kaynakSatir + 4, sutun, kaynakSatir + 5, sutun2, satir, sutun);
            XLS.SatirYukseklikAyarla(satir, satir + 2, (int)(yukseklik * 1.5));

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string RaporAlIc()
        {
            string kodKy;
            if (!string.IsNullOrWhiteSpace(Muhasebe))
            {
                string[] kurYapilar = servisBHMv2.KurumsalYapiListele(Muhasebe, Birim, KurKod);
                if (kurYapilar != null && kurYapilar.Length > 0)
                    kodKy = kurYapilar[0];
                else
                    throw new Exception(string.Format("Saymanlık, birim ({0},{1},{2}); kurumsal yapıya çevrilemedi!", Muhasebe, Birim, KurKod));
            }
            else if (!string.IsNullOrWhiteSpace(kyKod))
                kodKy = kyKod;
            else
                throw new Exception("Saymanlık veya kurumsal yapı kod verilmelidir!");
            return SeyyarRapor.Yazdir(servisBHMv2, TNS.HRC.Arac.Tanimla(), TNS.UZY.Arac.Tanimla(), kullanan, yil, kodKy, "", asama, asamaAd, sablonAd, "", sonucDosyaAd);
        }

        ENUMRevizeFormBelgeTur belgeTur;
        int belgeNo;
        private Hashtable raporParam;

        //private string RaporAlIlRevize()
        //{
        //    Tablo XLS = GenelIslemler.NewTablo();
        //    sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
        //    XLS.DosyaAc(sablonAd, sonucDosyaAd);
        //    int satir = 0;
        //    int sutun = 0;
        //    int kaynakSatir = 0;
        //    int baslaSatir = satir;
        //    int kaynakSatir2 = 0; int sutun2 = 0; int sheetNo = 0;
        //    XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref kaynakSatir, ref sutun, ref kaynakSatir2, ref sutun2);
        //    XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
        //    int yukseklik = XLS.SatirYukseklikAl(baslaSatir - 1);
        //    satir = baslaSatir;
        //    XLS.HucreAdBulYaz("Yil", yil);
        //    XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
        //    XLS.HucreAdBulYaz("FormNo", belgeNo);

        //    servisHRC = TNS.HRC.Arac.Tanimla();
        //    SeyyarGorevRevize okunanForm = servisHRC.OkuSeyyarGorevRevize(kullanan, yil, ENUMRevizeFormBelgeTur.IcmalRevize, belgeNo, false, false);
        //    if (okunanForm != null && okunanForm.sonuc.islemSonuc)
        //    {
        //        for (int i = 0; i < 3; i++)
        //        {
        //            XLS.HucreKopyala(kaynakSatir + 5 + i, sutun, kaynakSatir + 5 + i, sutun + 3, satir, sutun);
        //            satir++;
        //            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 3, satir, sutun);
        //            satir++;
        //            int satSay = okunanForm.IlDegisimler.Count;
        //            int topMemur = 0;
        //            int topIsci = 0;
        //            int fark = 1;
        //            for (int j = 0; j < okunanForm.IlDegisimler.Count; j++)
        //            {
        //                TavanGenel tg = (TavanGenel)okunanForm.IlDegisimler[j];

        //                XLS.HucreKopyala(kaynakSatir + fark, sutun, kaynakSatir + fark, sutun + 3, satir, sutun);

        //                int memur = 0;
        //                int isci = 0;

        //                if (i == 0)
        //                {
        //                    memur = (int)(tg.tavanMemur);
        //                    isci = (int)(tg.tavanIsci);
        //                }
        //                else if (i == 1)
        //                {
        //                    memur = (int)(tg.yil0);
        //                    isci = (int)(tg.yil1);
        //                }
        //                else
        //                {
        //                    memur = (int)(tg.tavanMemur + tg.yil0);
        //                    isci = (int)(tg.tavanIsci + tg.yil1);
        //                }
        //                XLS.HucreDegerYaz(satir, sutun, tg.ad1);
        //                XLS.HucreDegerYaz(satir, sutun + 1, memur);
        //                XLS.HucreDegerYaz(satir, sutun + 2, isci);

        //                topMemur += memur;
        //                topIsci += isci;

        //                satir++;
        //                fark = 2;
        //            }

        //            XLS.HucreKopyala(kaynakSatir + 3, sutun, kaynakSatir + 3, sutun + 3, satir, sutun);
        //            XLS.HucreDegerYaz(satir, sutun, "T O P L A M");
        //            XLS.HucreDegerYaz(satir, sutun + 1, topMemur);
        //            XLS.HucreDegerYaz(satir, sutun + 2, topIsci);
        //            satir += 2;
        //        }
        //        satir += 2;
        //        XLS.HucreDegerYaz(satir, sutun, okunanForm.onayaSunanAdSoyad);
        //        XLS.HucreDegerYaz(satir + 1, sutun, okunanForm.onayaSunanUnvan);

        //        XLS.DuseyHizala(satir, sutun, satir + 1, sutun + 3, 2);
        //        XLS.KoyuYap(satir, sutun, satir + 1, sutun + 3, true);

        //        XLS.HucreBirlestir(satir, sutun + 2, satir, sutun + 3);
        //        XLS.HucreBirlestir(satir + 1, sutun + 2, satir + 1, sutun + 3);
        //        XLS.HucreDegerYaz(satir, sutun + 2, okunanForm.onaylayanAdSoyad);
        //        XLS.HucreDegerYaz(satir + 1, sutun + 2, okunanForm.onaylayanUnvan);
        //    }
        //    else if (okunanForm == null)
        //        XLS.HucreDegerYaz(satir, 1, "Form okunamadı");
        //    else
        //        XLS.HucreDegerYaz(satir, 1, okunanForm.sonuc.hataStr);

        //    XLS.SatirYukseklikAyarla(baslaSatir, satir + 1, yukseklik);
        //    XLS.DosyaSaklaTamYol();
        //    return sonucDosyaAd;
        //}

        private string RaporAlDetayRevize()
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = satir;
            int kaynakSatir2 = 0; int sutun2 = 0; int sheetNo = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref kaynakSatir, ref sutun, ref kaynakSatir2, ref sutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir - 2);
            satir = baslaSatir;
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreAdBulYaz("FormNo", belgeNo);

            servisHRC = TNS.HRC.Arac.Tanimla();
            SeyyarGorevRevize okunanForm = servisHRC.OkuSeyyarGorevRevize(kullanan, yil, ENUMRevizeFormBelgeTur.DetayRevize, belgeNo, false, true);
            if (okunanForm != null && okunanForm.sonuc.islemSonuc)
            {
                int t_memurO = 0;
                int t_isciO = 0;
                int t_sozO = 0;
                int t_memur = 0;
                int t_isci = 0;
                int t_soz = 0;
                string eskiIlKod = "";
                string ilAd = "";
                okunanForm.birimDegisimler.Sort(new Kiyasla());
                for (int j = 0; j < okunanForm.birimDegisimler.Count; j++)
                {
                    TNS.BKD.TavanGenel tg = (TNS.BKD.TavanGenel)okunanForm.birimDegisimler[j];
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 11, satir, sutun);

                    int memurO = (int)(tg.dagitilanMemur);
                    int isciO = (int)(tg.dagitilanIsci);
                    int sozO = (int)(tg.dagitilanSozlesmeli);
                    int memur = (int)(tg.yil0);
                    int isci = (int)(tg.yil1);
                    int soz = (int)(tg.yil2);

                    string ilKod = tg.kod1.Substring(2, 2);
                    if (ilAd == "" || ilKod != eskiIlKod)
                    {
                        ilAd = IlAdBul(ilKod);
                    }
                    XLS.HucreDegerYaz(satir, sutun, ilAd);
                    XLS.HucreDegerYaz(satir, sutun + 1, tg.ad1);
                    XLS.HucreDegerYaz(satir, sutun + 2, memurO);
                    XLS.HucreDegerYaz(satir, sutun + 3, memur);

                    XLS.HucreDegerYaz(satir, sutun + 5, isciO);
                    XLS.HucreDegerYaz(satir, sutun + 6, isci);

                    XLS.HucreDegerYaz(satir, sutun + 8, sozO);
                    XLS.HucreDegerYaz(satir, sutun + 9, soz);

                    t_memurO += memurO;
                    t_isciO += isciO;
                    t_sozO += sozO;
                    t_memur += memur;
                    t_isci += isci;
                    t_soz += soz;

                    satir++;
                }

                XLS.HucreKopyala(kaynakSatir + 1, sutun, kaynakSatir + 1, sutun + 11, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 2, t_memurO);
                XLS.HucreDegerYaz(satir, sutun + 3, t_memur);

                XLS.HucreDegerYaz(satir, sutun + 5, t_isciO);
                XLS.HucreDegerYaz(satir, sutun + 6, t_isci);

                XLS.HucreDegerYaz(satir, sutun + 8, t_sozO);
                XLS.HucreDegerYaz(satir, sutun + 9, t_soz);

                satir += 3;

                XLS.DuseyHizala(satir, sutun, satir + 1, sutun + 11, 2);
                XLS.KoyuYap(satir, sutun, satir + 1, sutun + 11, true);

                XLS.HucreBirlestir(satir, sutun + 7, satir, sutun + 11);
                XLS.HucreBirlestir(satir + 1, sutun + 7, satir + 1, sutun + 11);

                XLS.HucreDegerYaz(satir, sutun + 1, okunanForm.onayaSunanAdSoyad);
                XLS.HucreDegerYaz(satir + 1, sutun + 1, okunanForm.onayaSunanUnvan);
                XLS.HucreDegerYaz(satir, sutun + 7, okunanForm.onaylayanAdSoyad);
                XLS.HucreDegerYaz(satir + 1, sutun + 7, okunanForm.onaylayanUnvan);
            }
            else if (okunanForm == null)
                XLS.HucreDegerYaz(satir, 1, "Form okunamadı");
            else
                XLS.HucreDegerYaz(satir, 1, okunanForm.sonuc.hataStr);

            XLS.SatirYukseklikAyarla(baslaSatir, satir + 1, yukseklik);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public class Kiyasla : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x is TNS.BKD.TavanGenel && y is TNS.BKD.TavanGenel)
                {
                    return KoduIlYap((TNS.BKD.TavanGenel)x).CompareTo(KoduIlYap((TNS.BKD.TavanGenel)y));
                }
                return 0;
            }

            private string KoduIlYap(TNS.BKD.TavanGenel tavanGenel)
            {
                return TNS.HRC.Arac.KoduIlYap(tavanGenel.kod1) + "_" + tavanGenel.kod1;
            }
        }

        private string RaporAlIslemDurum(int sadeceIller)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = satir;
            int kaynakSatir2 = 0; int sutun2 = 0; int sheetNo = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref kaynakSatir, ref sutun, ref kaynakSatir2, ref sutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir - 2);
            satir = baslaSatir;

            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            //XLS.HucreAdBulYaz("FormNo", "");

            servisHRC = TNS.HRC.Arac.Tanimla();
            SeyyarGorevRevize kistasForm = (SeyyarGorevRevize)raporParam["Kistas"];
            kistasForm.yil = yil;
            ObjectArray liste = servisHRC.SeyyarGorevIslemListele(kullanan, kistasForm, true);
            liste.objeler.Sort(new Kiyasla());
            if (liste != null && liste.sonuc.islemSonuc)
            {
                int eskiIlKod = -1;
                string ilAd = "";
                List<int> iller = new List<int>();
                if (sadeceIller == 0)
                {
                    for (int j = 0; j < liste.ObjeSayisi; j++)
                    {
                        TNS.BKD.TavanGenel tg = (TNS.BKD.TavanGenel)liste.objeler[j];
                        int memur = (int)(tg.yil0);
                        int isci = (int)(tg.yil1);
                        int soz = (int)(tg.yil2);

                        if (memur != 0 || isci != 0 || soz != 0)
                        {
                            int ilKod = OrtakFonksiyonlar.ConvertToInt(TNS.HRC.Arac.KoduIlYap(tg.kod1), 0);

                            if (eskiIlKod != ilKod)
                            {
                                iller.Add(ilKod);
                                eskiIlKod = ilKod;
                            }
                        }
                    }
                }
                eskiIlKod = -1;
                ToplamAl toplamAl = new ToplamAl(3, 6);
                toplamAl.SatirSifirla(0);
                int formulSat = 0;
                for (int j = 0; j < liste.ObjeSayisi; j++)
                {
                    TNS.BKD.TavanGenel tg = (TNS.BKD.TavanGenel)liste.objeler[j];
                    int memur = (int)(tg.yil0);
                    int isci = (int)(tg.yil1);
                    int soz = (int)(tg.yil2);

                    int memurO = (int)(tg.dagitilanMemur);
                    int isciO = (int)(tg.dagitilanIsci);
                    int sozO = (int)(tg.dagitilanSozlesmeli);
                    int ilKod = OrtakFonksiyonlar.ConvertToInt(TNS.HRC.Arac.KoduIlYap(tg.kod1), 0);
                    if (sadeceIller == 0 && iller.IndexOf(ilKod) < 0) continue;
                    if (eskiIlKod != ilKod)
                    {
                        if (eskiIlKod != -1)
                        {
                            satir = ToplamTazRev(sadeceIller, XLS, satir, sutun, kaynakSatir, ilAd, toplamAl, formulSat);
                            if (formulSat == 0)
                                formulSat = satir - 5;
                            XLS.SayfaSonuKoy(satir);
                        }
                        toplamAl.SatirSifirla(1);
                        eskiIlKod = ilKod;
                        ilAd = IlAdBul(ilKod.ToString("00"));
                    }
                    if (kistasForm.durum == TNS.HRC.ENUMRevizeFormDurum.Onayli)
                    {
                        toplamAl.DegerAta(2, 0, memurO - memur);
                        toplamAl.DegerAta(2, 1, isciO - isci);
                        toplamAl.DegerAta(2, 2, sozO - soz);
                    }
                    else
                    {
                        toplamAl.DegerAta(2, 0, memurO);
                        toplamAl.DegerAta(2, 1, isciO);
                        toplamAl.DegerAta(2, 2, sozO);
                    }
                    toplamAl.DegerAta(2, 3, memur);
                    toplamAl.DegerAta(2, 4, isci);
                    toplamAl.DegerAta(2, 5, soz);
                    toplamAl.ToplamEkle(1, 2);
                    toplamAl.ToplamEkle(0, 2);
                    if (sadeceIller == 0)
                    {
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
                        YazdirToplamAl(XLS, satir, sutun, ilAd, tg.ad1, toplamAl, 2, sadeceIller);
                        satir++;
                    }

                }
                if (eskiIlKod != -1)
                {
                    satir = ToplamTazRev(sadeceIller, XLS, satir, sutun, kaynakSatir, ilAd, toplamAl, formulSat);
                    XLS.SayfaSonuKoy(satir);
                }
                XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
                YazdirToplamAl(XLS, satir, sutun, "", "GENEL TOPLAM", toplamAl, 0, sadeceIller);
            }
            else if (liste == null)
                XLS.HucreDegerYaz(satir, 1, "Form okunamadı");
            else
                XLS.HucreDegerYaz(satir, 1, liste.sonuc.hataStr);

            XLS.SatirYukseklikAyarla(baslaSatir, satir + 1, yukseklik);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private int ToplamTazRev(int sadeceIller, Tablo XLS, int satir, int sutun, int kaynakSatir, string ilAd, ToplamAl toplamAl, int formulSat)
        {
            XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 13, satir, sutun);
            YazdirToplamAl(XLS, satir, sutun, ilAd, "İL TOPLAMI", toplamAl, 1, sadeceIller);
            XLS.KoyuYap(satir, sutun, satir, sutun + 11, true);
            satir++;
            if (sadeceIller == 0)
            {
                satir++;
                XLS.HucreKopyala(kaynakSatir + 3, sutun, kaynakSatir + 8, sutun + 13, satir, sutun);
                if (formulSat > 0)
                {
                    XLS.HucreFormulYaz(satir, sutun, "=A" + (formulSat).ToString()); //Açıklama satırı

                    XLS.HucreFormulYaz(satir + 3, sutun, "=A" + (formulSat + 3).ToString());   //Hazırlayan
                    XLS.HucreFormulYaz(satir + 3, sutun + 6, "=G" + (formulSat + 3).ToString()); //Onaylayan

                    XLS.HucreFormulYaz(satir + 4, sutun, "=A" + (formulSat + 4).ToString());
                    XLS.HucreFormulYaz(satir + 4, sutun + 6, "=G" + (formulSat + 4).ToString());

                    XLS.HucreFormulYaz(satir + 5, sutun, "=A" + (formulSat + 5).ToString());
                    XLS.HucreFormulYaz(satir + 5, sutun + 6, "=G" + (formulSat + 5).ToString());
                }

                satir += 6;
            }
            return satir;
        }

        private string IlAdBul(string ilKod)
        {
            ObjectArray oa = servisHRC.IlListeleKY(kullanan, ilKod);
            if (oa.ObjeSayisi >= 1)
                return ((TNS.HRC.Il)oa[0]).ad;
            return "Merkez - Tutulu";
        }

        private void YazdirToplamAl(Tablo XLS, int satir, int sutun, string ilAd, string birimAd, ToplamAl toplamAl, int sat, int sadeceIller)
        {
            if (sadeceIller == 0)
            {
                XLS.HucreDegerYaz(satir, sutun, ilAd);
                XLS.HucreDegerYaz(satir, sutun + 1, birimAd);
                XLS.HucreDegerYaz(satir, sutun + 2, toplamAl.toplam[sat, 0]);
                XLS.HucreDegerYaz(satir, sutun + 3, toplamAl.toplam[sat, 3]);

                XLS.HucreDegerYaz(satir, sutun + 5, toplamAl.toplam[sat, 1]);
                XLS.HucreDegerYaz(satir, sutun + 6, toplamAl.toplam[sat, 4]);

                XLS.HucreDegerYaz(satir, sutun + 8, toplamAl.toplam[sat, 2]);
                XLS.HucreDegerYaz(satir, sutun + 9, toplamAl.toplam[sat, 5]);
            }
            else
            {
                XLS.HucreDegerYaz(satir, sutun, ilAd);
                XLS.HucreDegerYaz(satir, sutun + 1, toplamAl.toplam[sat, 0]);
                XLS.HucreDegerYaz(satir, sutun + 2, toplamAl.toplam[sat, 1]);
                XLS.HucreDegerYaz(satir, sutun + 3, toplamAl.toplam[sat, 2]);

                XLS.HucreDegerYaz(satir, sutun + 5, toplamAl.toplam[sat, 3]);
                XLS.HucreDegerYaz(satir, sutun + 6, toplamAl.toplam[sat, 4]);
                XLS.HucreDegerYaz(satir, sutun + 7, toplamAl.toplam[sat, 5]);
            }
        }

        private string RaporAlDurum()
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = satir;
            int kaynakSatir2 = 0; int sutun2 = 0; int sheetNo = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref kaynakSatir, ref sutun, ref kaynakSatir2, ref sutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir - 1);
            satir = baslaSatir;
            XLS.HucreAdBulYaz("Yil", yil);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            servisHRC = TNS.HRC.Arac.Tanimla();

            ToplamAl t = new ToplamAl(3, 6);
            t.SatirSifirla(0);

            ObjectArray oa = servisHRC.SeyyarGorevBilgiBulBirim(kullanan, yil, null, 0, true);
            if (oa != null && oa.sonuc.islemSonuc)
            {
                string eskiIlKod = "";
                int fark = 0;
                oa.objeler.Sort(new Kiyasla());
                for (int i = 0; i < oa.ObjeSayisi; i++)
                {
                    TNS.BKD.TavanGenel tg = (TNS.BKD.TavanGenel)oa.Al(i);
                    string gelenIlKod;
                    if (tg.kod1 != null && tg.kod1.Length >= 3)
                        gelenIlKod = TNS.HRC.Arac.KoduIlYap(tg.kod1);
                    else
                        gelenIlKod = "--";

                    if (gelenIlKod != eskiIlKod)
                    {
                        if (eskiIlKod != "")
                        {
                            fark = 0;
                            if (SariMi(t, 1))
                                fark = 1;
                            XLS.HucreKopyala(kaynakSatir + 3 + fark, sutun, kaynakSatir + 3 + fark, sutun + 13, satir, sutun);
                            SatirYaz(XLS, satir, sutun, eskiIlKod + " İl Toplam", t, 1);
                            satir++;
                        }
                        eskiIlKod = gelenIlKod;
                        t.SatirSifirla(1);
                    }

                    t.DegerAta(2, 0, tg.dagitilanMemur);
                    t.DegerAta(2, 1, tg.dagitilanIsci);
                    t.DegerAta(2, 2, tg.kullanilanMemur);
                    t.DegerAta(2, 3, tg.kullanilanIsci);
                    t.DegerAta(2, 4, tg.dagitilanSozlesmeli);
                    t.DegerAta(2, 5, tg.kullanilanSozlesmeli);
                    t.ToplamEkle(1, 2);
                    t.ToplamEkle(0, 2);

                    fark = 0;
                    if (SariMi(t, 2))
                        fark = 1;

                    XLS.HucreKopyala(kaynakSatir + fark, sutun, kaynakSatir + fark, sutun + 13, satir, sutun);
                    SatirYaz(XLS, satir, sutun, tg.kod1 + "-" + tg.ad1, t, 2);
                    satir++;
                }

                if (eskiIlKod != "")
                {
                    fark = 0;
                    if (SariMi(t, 1))
                        fark = 1;
                    XLS.HucreKopyala(kaynakSatir + 3 + fark, sutun, kaynakSatir + 3 + fark, sutun + 13, satir, sutun);
                    SatirYaz(XLS, satir, sutun, eskiIlKod + " İl Toplam", t, 1);
                    satir++;

                    fark = 0;
                    if (SariMi(t, 0))
                        fark = 1;
                    XLS.HucreKopyala(kaynakSatir + 3 + fark, sutun, kaynakSatir + 3 + fark, sutun + 13, satir, sutun);
                    SatirYaz(XLS, satir, sutun, "Genel Toplam", t, 0);
                }
            }
            else if (oa == null)
                XLS.HucreDegerYaz(satir, 1, "Form okunamadı");
            else
                XLS.HucreDegerYaz(satir, 1, oa.sonuc.hataStr);

            XLS.SatirYukseklikAyarla(baslaSatir, satir + 1, yukseklik);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private bool SariMi(ToplamAl t, int satir)
        {
            return (t.toplam[satir, 3] > t.toplam[satir, 1] || t.toplam[satir, 2] > t.toplam[satir, 0] || t.toplam[satir, 5] > t.toplam[satir, 4]);
        }

        private void SatirYaz(Tablo XLS, int satir, int sutun, string ad, ToplamAl t, int t_satir)
        {
            XLS.HucreDegerYaz(satir, sutun, ad);
            XLS.HucreDegerYaz(satir, sutun + 1, t.toplam[t_satir, 0]);
            XLS.HucreDegerYaz(satir, sutun + 2, t.toplam[t_satir, 1]);
            XLS.HucreDegerYaz(satir, sutun + 3, t.toplam[t_satir, 4]);

            XLS.HucreDegerYaz(satir, sutun + 5, t.toplam[t_satir, 2]);
            XLS.HucreDegerYaz(satir, sutun + 6, t.toplam[t_satir, 3]);
            XLS.HucreDegerYaz(satir, sutun + 7, t.toplam[t_satir, 5]);
        }

        private string RaporAlUnvanListe()
        {
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            int satir = 0;
            int sutun = 0;
            int kaynakSatir = 0;
            int baslaSatir = satir;
            int kaynakSatir2 = 0; int sutun2 = 0; int sheetNo = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref sheetNo, ref kaynakSatir, ref sutun, ref kaynakSatir2, ref sutun2);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
            int yukseklik = XLS.SatirYukseklikAl(kaynakSatir);
            satir = baslaSatir;
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("KurumAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreAdBulYaz("IstihdamSekli", IstihtamSekli.AdBul((ENUMPersonelIstihdamSekli)istihdamSekli));
            servisBHMv2 = TNS.BHMv2.Arac.Tanimla();

            double tyil0 = 0;
            double tyil1 = 0;
            ObjectArray oa = servisBHMv2.ListeleBHMTavanGenel(kullanan, yil, "040" + istihdamSekli.ToString(), TNS.BKD.ENUMTavanTur.tavanTurSeyyarIlTavan, new TNS.BKD.TavanGenel(), false);
            if (oa != null && oa.sonuc.islemSonuc)
            {
                int say = 0;
                for (int i = 0; i < oa.ObjeSayisi; i++)
                {
                    TNS.BKD.TavanGenel tg = (TNS.BKD.TavanGenel)oa.Al(i);

                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 6, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun + 0, ++say);
                    XLS.HucreDegerYaz(satir, sutun + 1, tg.kod1);
                    XLS.HucreDegerYaz(satir, sutun + 4, tg.yil0);
                    XLS.HucreDegerYaz(satir++, sutun + 5, tg.yil1);
                    tyil0 += tg.yil0;
                    tyil1 += tg.yil1;
                }

                XLS.HucreKopyala(kaynakSatir + 1, sutun, kaynakSatir + 1, sutun + 6, satir, sutun);
                XLS.HucreDegerYaz(satir, sutun + 4, tyil0);
                XLS.HucreDegerYaz(satir++, sutun + 5, tyil1);
                XLS.SatirYukseklikAyarla(baslaSatir, satir + 1, yukseklik, 0, yukseklik);
                XLS.SatirSil(0, 4);
            }
            else if (oa == null)
                XLS.HucreDegerYaz(satir, 1, "Form okunamadı");
            else
                XLS.HucreDegerYaz(satir, 1, oa.sonuc.hataStr);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string RaporAl(object o_kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            kullanan = (Kullanici)o_kullanan;
            KurKod = (string)raporParam["KurKod"];
            Muhasebe = (string)raporParam["Muhasebe"];
            Birim = (string)raporParam["Birim"];
            servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
            yil = OrtakFonksiyonlar.ConvertToInt(raporParam["Yil"], 0);
            asama = OrtakFonksiyonlar.ConvertToInt((string)raporParam["asama"], 3);
            Ilkod = (string)raporParam["Ilkod"];
            kyKod = (string)raporParam["kyKod"];
            istihdamSekli = OrtakFonksiyonlar.ConvertToInt((string)raporParam["istihdamSekli"], 1);

            belgeTur = ENUMRevizeFormBelgeTur.DetayRevize; //)OrtakFonksiyonlar.ConvertToInt(raporParam["BelgeTur"], 2);
            belgeNo = OrtakFonksiyonlar.ConvertToInt((string)raporParam["BelgeNo"], 0);
            this.raporParam = raporParam;

            this.sablonAd = sablonAd;
            this.sonucDosyaAd = sonucDosyaAd;

            asamaAd = "Tanımsız";
            if (asama == 1) asamaAd = "Birim Teklifi";
            else if (asama == 2) asamaAd = "Kurum Teklifi";
            else if (asama == 3) asamaAd = "Vize Edilen";

            if (ekBilgi == "ILTOP")
                return RaporAlIlTop();
            else if (ekBilgi == "ILDET")
                return RaporAlIlDetay();
            else if (ekBilgi == "BIRIM")
                return RaporAlIc();
            //else if (ekBilgi == "ILREVIZE")
            //    return RaporAlIlRevize();
            else if (ekBilgi == "DETAYREVIZE")
                return RaporAlDetayRevize();
            else if (ekBilgi == "DETAYDURUM")
                return RaporAlDurum();
            else if (ekBilgi == "TOPLAMREVIZE")
                return RaporAlIslemDurum(0);
            else if (ekBilgi == "TOPLAMREVIZEIL")
                return RaporAlIslemDurum(1);
            else if (ekBilgi == "UNVANLISTE")
                return RaporAlUnvanListe();
            else
                return RaporAlIc();
        }

    }
}
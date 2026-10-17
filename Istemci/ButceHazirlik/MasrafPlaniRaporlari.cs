using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    /// <summary>
    /// Masraf planı raporları (eski ButceHazirlikv2/MasrafPlaniRaporlar.cs içindeki MPRaporDetay sınıfı). Rapor kodu aynıdır; değişenler:
    /// TNS.Raporlama.IRapor ve System.Web kaldırıldı, şablon yolu SablonBul ile bulunur (önce kurumun RaporSablonYol\BHMv2 klasörü, yoksa
    /// uygulamayla gelen SGB.Net\RaporSablon\BHMv2). ekBilgi: HO, Tavan, TavanIcm, TavanIcmDetay, Hizli, GOND_FAZ/GOND_AZ/HEPSI(2), 36, 37,
    /// diğerleri AHP (MPRaporDetay.xlt, şablon adı verilir). Masraf Planı Tavan ekranı (MPTavanAna) Tavan, TavanIcmDetay ve HO'yu kullanır.
    /// </summary>
    public class MPRaporDetay
    {
        Tablo XLS;
        private readonly string uygulamaKlasoru;

        public MPRaporDetay() : this(null) { }

        /// <param name="uygulamaKlasoru">IWebHostEnvironment.ContentRootPath (uygulamayla gelen RaporSablon klasörü bunun altındadır).</param>
        public MPRaporDetay(string uygulamaKlasoru)
        {
            this.uygulamaKlasoru = string.IsNullOrEmpty(uygulamaKlasoru) ? AppContext.BaseDirectory : uygulamaKlasoru;
        }

        /// <summary>BHMv2 şablonu: önce kurumun RaporSablonYol\BHMv2 klasörü, yoksa uygulamayla gelen SGB.Net\RaporSablon\BHMv2.</summary>
        private string SablonBul(string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "BHMv2", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "BHMv2", sablonAd);
            if (File.Exists(u)) return u;
            throw new Exception("Rapor şablonu bulunamadı (BHMv2\\" + sablonAd + ").");
        }
        int baslaSutun = -1;
        int baslaSatir = -1;

        private string RaporAlAHP(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kurKod = (string)raporParam["KurKod"] + "";
            string kurAd = (string)raporParam["KurAd"] + "";
            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurKod, "", "", "", "");
            string birim = (string)raporParam["Birim"] + ""; ;
            TertipOdenek[] tos = servis.MPTertipListeleAy(kullanan, kistas, birim);

            XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            if (string.IsNullOrEmpty(sablonAd)) sablonAd = SablonBul("MPRaporDetay.xlt");
            XLS.DosyaAc(sablonAd, sonucDosyaAd);


            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("Birim", kurKod);
            XLS.HucreAdBulYaz("BirimAd", kurAd);
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir);

            int satir = baslaSatir;
            ToplamAl toplamAl = new ToplamAl(1, 14);
            int oncekiToplamSatir = -1;
            bool yeniBolum = true;
            for (int i = 0; i < tos.Length; i++)
            {
                if (tos[i].tertip.tabloKod == ENUMTabloKod.TABButce)
                {
                    XLS.HucreKopyala(baslaSatir - 4, baslaSutun, baslaSatir - 3, baslaSutun + 19, satir, baslaSutun);
                    //XLS.HucreBirlestir(satir, baslaSutun, satir + 1, baslaSutun);
                    yeniBolum = true;
                    if (oncekiToplamSatir != -1)
                    {
                        ToplamYaz(oncekiToplamSatir, toplamAl, baslaSutun);
                    }
                    toplamAl.HepsiniSifirla();
                    oncekiToplamSatir = satir;
                    XLS.SatirYukseklikAyarla(satir, oncekiToplamSatir, yukseklik);
                }
                else
                {
                    int kaynakSatir;
                    if (yeniBolum)
                    {
                        kaynakSatir = baslaSatir - 2;
                    }
                    else
                        kaynakSatir = baslaSatir - 1;
                    XLS.HucreKopyala(kaynakSatir, baslaSutun, kaynakSatir, baslaSutun + 19, satir, baslaSutun);
                    XLS.SatirYukseklikAyarla(satir, satir, yukseklik);
                    for (int ay = 0; ay < 12; ay++)
                    {
                        XLS.HucreDegerYaz(satir, baslaSutun + ay + 1, tos[i].odenek[ay]);
                    }
                    XLS.HucreDegerYaz(satir, baslaSutun + 13 + 1, tos[i].odenek[12]);
                    XLS.HucreDegerYaz(satir, baslaSutun + 14 + 1, tos[i].odenek[13]);
                    for (int ay = 0; ay < 14; ay++)
                    {
                        toplamAl.DegerArttir(0, ay, tos[i].odenek[ay]);
                    }
                    yeniBolum = false;
                }
                XLS.HucreDegerYaz(satir, baslaSutun, tos[i].tertip.TertipStrOl());
                satir++;
            }
            if (oncekiToplamSatir != -1)
            {
                ToplamYaz(oncekiToplamSatir, toplamAl, baslaSutun);
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;

        }

        private string RaporAlTavanIcm(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kur = (string)raporParam["KurKod"] + "";
            string fon = (string)raporParam["FonKod"] + "";
            string fin = (string)raporParam["FinKod"] + "";
            string eko = (string)raporParam["EkoKod"] + "";
            string peb = (string)raporParam["PebKod"] + "";
            string hrc = (string)raporParam["HrcKod"] + "";
            string sayBir = (string)raporParam["SayBir"] + "";

            //Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, "", "", "", "");
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABTasraOdenek, kur, fon, fin, eko, peb, hrc);
            TertipOdenek[] odnler = servis.MPTavanIlListele(kullanan, kistasTertip, sayBir);
            XLS = GenelIslemler.NewTablo();
            if (string.IsNullOrEmpty(sonucDosyaAd)) sonucDosyaAd = DosyaIslem.DosyaAdUret();
            if (string.IsNullOrEmpty(sablonAd)) sablonAd = SablonBul("MasrafPlaniTavanIcm.xltx");

            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int formatSatir = 0;
            int formatSutun = 0;
            XLS.HucreAdAdresCoz("FormatSatir", ref formatSatir, ref formatSutun);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("Birim", hrc);
            XLS.HucreAdBulYaz("BirimAd", GenelIslemler.KodAd(53, hrc, true));
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir - 2);

            int satir = baslaSatir;
            ToplamAl toplamAl = new ToplamAl(2, 2);
            toplamAl.SatirSifirla(1);

            for (int i = 0; i < odnler.Length; i++)
            {
                TertipOdenek to = odnler[i];

                toplamAl.DegerAta(1, 0, to.odenek[0]);
                toplamAl.DegerAta(1, 1, to.odenek[1]);
                toplamAl.ToplamEkle(0, 1);

                XLS.HucreKopyala(formatSatir, formatSutun, formatSatir, formatSutun + 5, satir, baslaSutun);

                XLS.HucreDegerYaz(satir, baslaSutun, to.tertip.TertipStrOl());
                XLS.HucreDegerYaz(satir, baslaSutun + 1, to.tertip.adlar[3] + "(" + to.tertip.adlar[4] + ")");
                XLS.HucreDegerYaz(satir, baslaSutun + 2, toplamAl.toplam[1, 0]);
                XLS.HucreDegerYaz(satir, baslaSutun + 3, toplamAl.toplam[1, 1]);

                satir++;
            }
            XLS.HucreKopyala(formatSatir + 1, formatSutun, formatSatir + 1, formatSutun + 5, satir, baslaSutun);
            XLS.HucreDegerYaz(satir, baslaSutun + 2, toplamAl.toplam[0, 0]);
            XLS.HucreDegerYaz(satir, baslaSutun + 3, toplamAl.toplam[0, 1]);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string RaporAlTavan(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kur = (string)raporParam["KurKod"] + "";
            string fon = (string)raporParam["FonKod"] + "";
            string fin = (string)raporParam["FinKod"] + "";
            string eko = (string)raporParam["EkoKod"] + "";
            string peb = (string)raporParam["PebKod"] + "";
            string hrc = (string)raporParam["HrcKod"] + "";
            string sayBir = (string)raporParam["SayBir"] + "";
            //Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kur, "", "", "", "");
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABTasraOdenek, kur, fon, fin, eko, peb, hrc);
            UcgenKodBilgileri ukb = new UcgenKodBilgileri();
            ukb.Add(new UcgenKodBilgi(17, OrtakFonksiyonlar.ConvertToInt((string)raporParam["HrcKodUz"], 0)));
            ukb.Add(new UcgenKodBilgi(1, OrtakFonksiyonlar.ConvertToInt((string)raporParam["KurKodUz"], 0)));
            ukb.Add(new UcgenKodBilgi(2, OrtakFonksiyonlar.ConvertToInt((string)raporParam["FonKodUz"], 0)));
            ukb.Add(new UcgenKodBilgi(3, OrtakFonksiyonlar.ConvertToInt((string)raporParam["FinKodUz"], 0)));
            ukb.Add(new UcgenKodBilgi(4, OrtakFonksiyonlar.ConvertToInt((string)raporParam["EkoKodUz"], 0)));
            ukb.Add(new UcgenKodBilgi(14, OrtakFonksiyonlar.ConvertToInt((string)raporParam["PebKodUz"], 0)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[4];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMPTavan, 0, 0, "1");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMPTavan, -1, 0, "1");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, -1, 0, "0/7");
            ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABMIFKY, -1, 0, "");

            TNS.MUH.IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
            string[] muhbir = sayBir.Split('-');
            bool yVar = servisMUH.MuhasebeBirimYetkiVarMi(kullanan, OrtakFonksiyonlar.DiziElemanAl(muhbir, 0, ""), OrtakFonksiyonlar.DiziElemanAl(muhbir, 1, ""), TNS.MUH.ENUMOGY.Gorme);
            if (!yVar)
                throw new Exception("Birimi görmeye yetkiniz yok!");

            OdenekBilgi[] tolar = servis.ListeTertipOl(kullanan, kistasTertip, ukb, ekBilgiler, true, false, true);
            //TertipOdenek[] tolar = servis.MPListeleRapor(kullanan, kistasTertip, ukb, true, sayBir);

            XLS = GenelIslemler.NewTablo();
            if (string.IsNullOrEmpty(sonucDosyaAd)) sonucDosyaAd = DosyaIslem.DosyaAdUret();
            if (string.IsNullOrEmpty(sablonAd)) sablonAd = SablonBul("MasrafPlaniTavan.xlt");

            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("Birim", hrc);
            XLS.HucreAdBulYaz("BirimAd", GenelIslemler.KodAd(53, hrc, true));
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir);

            int satir = baslaSatir;
            ToplamAl toplamAl = new ToplamAl(3, 5);
            toplamAl.DegerAta(2, 0, 0);
            XLS.HucreKopyala(baslaSatir - 4, baslaSutun, baslaSatir - 4, baslaSutun + 10, satir, baslaSutun);
            bool altiKoyu = true;
            XLS.HucreDegerYaz(satir, baslaSutun, "T O P L A M");
            satir++;
            string oncekiHBirim = "";
            int birimSatir = -1;
            for (int i = 0; i < tolar.Length; i++)
            {
                OdenekBilgi to = tolar[i];
                string simdikiBirim = to.tertip.kodlar[5];
                string simdikiBirimAd = BirimAdBul(uzyServis, kullanan, to.tertip.kodlar[5]);
                if (oncekiHBirim != simdikiBirim)
                {
                    if (birimSatir > -1)
                    {
                        ToplamlariYaz(XLS, birimSatir, toplamAl, 1);
                        toplamAl.SatirSifirla(1);
                    }
                    oncekiHBirim = simdikiBirim;
                    if (!string.IsNullOrEmpty(simdikiBirim))
                    {
                        birimSatir = satir;
                        XLS.HucreKopyala(baslaSatir - 4, baslaSutun, baslaSatir - 4, baslaSutun + 10, satir, baslaSutun);
                        XLS.HucreDegerYaz(satir, baslaSutun + 0, simdikiBirim);
                        XLS.HucreDegerYaz(satir, baslaSutun + 1, simdikiBirimAd);
                        altiKoyu = true;
                        satir++;
                    }
                }
                if (to.EkRakam(0) != 0)
                {
                    toplamAl.DegerAta(2, 1, to.EkRakam(0));
                    toplamAl.DegerAta(2, 2, to.EkRakam(1));
                    toplamAl.DegerAta(2, 3, to.EkRakam(2));
                    toplamAl.DegerAta(2, 4, to.EkRakam(3));
                    toplamAl.ToplamEkle(1, 2);
                    toplamAl.ToplamEkle(0, 2);
                    int kaynakSat = 0;
                    if (altiKoyu)
                    {
                        if (i == tolar.Length - 1)
                            kaynakSat = baslaSatir - 4;
                        else
                        {
                            kaynakSat = baslaSatir - 3;
                            altiKoyu = false;
                        }
                    }
                    else
                    {
                        if (i == tolar.Length - 1)
                            kaynakSat = baslaSatir - 1;
                        else
                        {
                            kaynakSat = baslaSatir - 2;
                            altiKoyu = false;
                        }
                    }
                    XLS.HucreKopyala(kaynakSat, baslaSutun, kaynakSat, baslaSutun + 10, satir, baslaSutun);

                    Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, to.tertip.kodlar[0], to.tertip.kodlar[1], to.tertip.kodlar[2], to.tertip.kodlar[3], to.tertip.kodlar[4]);
                    string tStr = t.TertipStrOl();
                    XLS.HucreDegerYaz(satir, baslaSutun, tStr.TrimStart('-'));
                    XLS.HucreDegerYaz(satir, baslaSutun - 1, to.tertip.adlar[5]);
                    XLS.HucreDegerYaz(satir, baslaSutun - 2, to.tertip.kodlar[5]);
                    XLS.HucreDegerYaz(satir, baslaSutun - 3, to.tertip.kodlar[4]);
                    XLS.HucreDegerYaz(satir, baslaSutun - 4, to.tertip.kodlar[3]);
                    XLS.HucreDegerYaz(satir, baslaSutun - 5, to.tertip.kodlar[2]);
                    XLS.HucreDegerYaz(satir, baslaSutun - 6, to.tertip.kodlar[1]);
                    XLS.HucreDegerYaz(satir, baslaSutun - 7, to.tertip.kodlar[0]);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, TertipAdiBul(to.tertip));

                    ToplamlariYaz(XLS, satir, toplamAl, 2);

                    satir++;
                }
            }
            if (birimSatir > -1)
            {
                ToplamlariYaz(XLS, birimSatir, toplamAl, 1);
            }

            ToplamlariYaz(XLS, baslaSatir, toplamAl, 0);
            XLS.HucreBirlestir(baslaSatir, baslaSutun + 0, baslaSatir, baslaSutun + 1);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string RaporAlTavanIcmDetay(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kur = ((string)raporParam["KurKod"] + "").Replace(".", "");
            string fon = ((string)raporParam["FonKod"] + "").Replace(".", "");
            string fin = ((string)raporParam["FinKod"] + "").Replace(".", "");
            string eko = ((string)raporParam["EkoKod"] + "").Replace(".", "");
            string peb = ((string)raporParam["PebKod"] + "").Replace(".", "");
            int tur = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["Tur"] + ""), 1);

            string[] sqller = new string[4];
            string[] baglar = new string[4];

            string sql = "SELECT LEFT(TVN.KYKOD,3), TVN.KURKOD, TVN.FONKOD, TVN.FINKOD, TVN.EKOKOD, TVN.PEBKOD, TER.AD, SUM(TVN.ODENEK) FROM MPTAVAN TVN LEFT OUTER JOIN ButceIslemleri.dbo.BHMKURUMSALYAPI TER ON LEFT(TVN.KYKOD,3)=TER.KOD WHERE TVN.YIL={0} and TVN.DETAYMI={1} AND TVN.KURKOD LIKE {2} AND TVN.FONKOD LIKE {3} AND TVN.FINKOD LIKE {4} AND TVN.EKOKOD LIKE {5} AND TVN.PEBKOD LIKE {6} group by LEFT(TVN.KYKOD,3), TVN.KURKOD, TVN.FONKOD, TVN.FINKOD, TVN.EKOKOD, TVN.PEBKOD, TER.AD order by LEFT(TVN.KYKOD,3), TVN.KURKOD, TVN.FONKOD, TVN.FINKOD, TVN.EKOKOD, TVN.PEBKOD";

            sqller[0] = string.Format(sql, yil, 0, Yuzdele(kur), Yuzdele(fon), Yuzdele(fin), Yuzdele(eko), Yuzdele(peb));
            sqller[1] = string.Format(sql, yil, 1, Yuzdele(kur), Yuzdele(fon), Yuzdele(fin), Yuzdele(eko), Yuzdele(peb));
            sqller[2] = string.Format(sql, yil - 1, 0, Yuzdele(kur), Yuzdele(fon), Yuzdele(fin), Yuzdele(eko), Yuzdele(peb));
            sqller[3] = string.Format(sql, yil - 1, 1, Yuzdele(kur), Yuzdele(fon), Yuzdele(fin), Yuzdele(eko), Yuzdele(peb));
            baglar[3] = baglar[2] = baglar[1] = baglar[0] = "BaglantiSatiriBHMv2";

            System.Collections.SortedList sl = uzyServis.SorguBirlestir(sqller, baglar, new int[] { 0, 1, 2, 3, 4, 5 });
            XLS = GenelIslemler.NewTablo();
            if (string.IsNullOrEmpty(sonucDosyaAd)) sonucDosyaAd = DosyaIslem.DosyaAdUret();
            if (string.IsNullOrEmpty(sablonAd)) sablonAd = SablonBul("MasrafPlaniTavanIcmDetay.xltx");

            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref baslaSutun);
            XLS.HucreAdBulYaz("ButceYil", yil);
            int yukseklik = XLS.SatirYukseklikAl(baslaSatir);
            int satir = baslaSatir;
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            foreach (System.Collections.DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string bir = alAnahtar.StrAl(0);
                kur = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), alAnahtar.StrAl(1));
                fon = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), alAnahtar.StrAl(2));
                fin = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(2), alAnahtar.StrAl(3));
                eko = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), alAnahtar.StrAl(4));
                peb = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(4), alAnahtar.StrAl(5));
                string ad1 = alVeri.StrAl(0);
                double od1 = alVeri.DblAl(1);

                if (string.IsNullOrEmpty(ad1))
                    ad1 = alVeri.StrAl(2);
                double od2 = alVeri.DblAl(3);

                if (string.IsNullOrEmpty(ad1))
                    ad1 = alVeri.StrAl(4);
                double od3 = alVeri.DblAl(5);

                if (string.IsNullOrEmpty(ad1))
                    ad1 = alVeri.StrAl(6);
                double od4 = alVeri.DblAl(7);
                if (tur == 1 || tur == 2 && od1 > od2 || tur == 3 && od1 < od2)
                {
                    XLS.HucreKopyala(baslaSatir - 8, baslaSutun, baslaSatir - 8, baslaSutun + 20, satir, baslaSutun);
                    XLS.HucreDegerYaz(satir, baslaSutun + 0, kur);
                    XLS.HucreDegerYaz(satir, baslaSutun + 1, fon);
                    XLS.HucreDegerYaz(satir, baslaSutun + 2, fin);
                    XLS.HucreDegerYaz(satir, baslaSutun + 3, eko);
                    XLS.HucreDegerYaz(satir, baslaSutun + 4, peb);
                    XLS.HucreDegerYaz(satir, baslaSutun + 5, bir);
                    XLS.HucreDegerYaz(satir, baslaSutun + 7, kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb);
                    XLS.HucreDegerYaz(satir, baslaSutun + 8, ad1);
                    XLS.HucreDegerYaz(satir, baslaSutun + 9, od3);
                    XLS.HucreDegerYaz(satir, baslaSutun + 10, od4);
                    XLS.HucreDegerYaz(satir, baslaSutun + 11, od1);
                    XLS.HucreDegerYaz(satir, baslaSutun + 12, od2);
                    satir++;
                    TertipDetayYaz(uzyServis, XLS, ref satir, baslaSutun, yil, kur, fon, fin, eko, peb, bir);
                }
            }
            XLS.SatirYukseklikAyarla(baslaSatir, satir, yukseklik);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void TertipDetayYaz(IUZYServis uzyServis, Tablo XLS, ref int satir, int baslaSutun, int yil, string kur, string fon, string fin, string eko, string peb, string bir)
        {
            string[] sqller = new string[2];
            string[] baglar = new string[2];

            string sql = "SELECT case BIR.MUHASEBEKOD%1000 when 100 then 0 when 150 then 0 else BIR.MUHASEBEKOD%1000 end, BIR.MUHASEBEKOD, BIR.BIRIMKOD, MUH.MUHASEBEAD, BIR.BIRIMAD, TVN.ODENEK FROM MPTAVAN TVN LEFT OUTER JOIN ButceIslemleri.dbo.BHMKURUMSALYAPI KY ON TVN.KYKOD=KY.KOD LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE MUH ON MUH.MUHASEBEKOD=KY.SAYMANLIKKOD LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM BIR ON BIR.BIRIMKOD=KY.BIRIMKOD AND BIR.MUHASEBEKOD=KY.SAYMANLIKKOD WHERE TVN.YIL={0} and TVN.DETAYMI=1 AND TVN.KYKOD LIKE {1} AND TVN.KURKOD = {2} AND TVN.FONKOD = {3} AND TVN.FINKOD = {4} AND TVN.EKOKOD = {5} AND TVN.PEBKOD = {6} order by 1,2,3";

            sqller[0] = string.Format(sql, yil, Yuzdele(bir), Yuzdele(kur, false), Yuzdele(fon, false), Yuzdele(fin, false), Yuzdele(eko, false), Yuzdele(peb, false));
            sqller[1] = string.Format(sql, yil - 1, Yuzdele(bir), Yuzdele(kur, false), Yuzdele(fon, false), Yuzdele(fin, false), Yuzdele(eko, false), Yuzdele(peb, false));
            baglar[1] = baglar[0] = "BaglantiSatiriBHMv2";

            System.Collections.SortedList sl = uzyServis.SorguBirlestir(sqller, baglar, new int[] { 0, 1, 2, 3, 4 });
            foreach (System.Collections.DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                int sil = alAnahtar.IntAl(0);
                int muh = alAnahtar.IntAl(1);
                string muhBir = alAnahtar.StrAl(2);
                string muhAd = alAnahtar.StrAl(3);
                string muhBirAd = alAnahtar.StrAl(4);

                double od1 = alVeri.DblAl(0);
                double od2 = alVeri.DblAl(1);

                XLS.HucreKopyala(baslaSatir - 9, baslaSutun, baslaSatir - 9, baslaSutun + 20, satir, baslaSutun);
                XLS.HucreDegerYaz(satir, baslaSutun + 0, kur);
                XLS.HucreDegerYaz(satir, baslaSutun + 1, fon);
                XLS.HucreDegerYaz(satir, baslaSutun + 2, fin);
                XLS.HucreDegerYaz(satir, baslaSutun + 3, eko);
                XLS.HucreDegerYaz(satir, baslaSutun + 4, peb);
                XLS.HucreDegerYaz(satir, baslaSutun + 5, muh);
                XLS.HucreDegerYaz(satir, baslaSutun + 6, muhBir);
                XLS.HucreDegerYaz(satir, baslaSutun + 7, kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb);
                XLS.HucreDegerYaz(satir, baslaSutun + 8, TNS.MUH.Arac.BirimAdOl(muhAd, muhBirAd));
                XLS.HucreDegerYaz(satir, baslaSutun + 10, od2);
                XLS.HucreDegerYaz(satir, baslaSutun + 12, od1);
                satir++;
            }
        }

        private string Yuzdele(string kod, bool yuzdeEkle = true)
        {
            return "'" + kod.Replace("'", "").Replace(".", "") + (yuzdeEkle ? "%" : "") + "'";
        }

        private string BirimAdBul(IUZYServis servisUZY, Kullanici kullanan, string birimKod)
        {
            string sql = "SELECT MUHMUHASEBE.MUHASEBEAD, MUHMUHASEBEBIRIM.BIRIMAD FROM BHMKURUMSALYAPI INNER JOIN MUHMUHASEBEBIRIM ON BHMKURUMSALYAPI.BIRIMKOD = MUHMUHASEBEBIRIM.BIRIMKOD AND BHMKURUMSALYAPI.SAYMANLIKKOD = MUHMUHASEBEBIRIM.MUHASEBEKOD INNER JOIN MUHMUHASEBE ON MUHMUHASEBEBIRIM.MUHASEBEKOD = MUHMUHASEBE.MUHASEBEKOD WHERE (BHMKURUMSALYAPI.KOD = '" + birimKod.Replace(".", "") + "')";
            TNS.UZY.VTSorgula vts = new TNS.UZY.VTSorgula(sql, "BaglantiSatiriBUM", 20000);

            System.Data.DataRow row = vts.satirOku();
            if (row == null) return "";

            return TNS.MUH.Arac.BirimAdOl(vts.StringAl(0), vts.StringAl(1));
        }

        private void ToplamlariYaz(Tablo XLS, int satir, ToplamAl toplamAl, int toplamAlSatir)
        {
            XLS.HucreDegerYaz(satir, baslaSutun + 2, toplamAl.toplam[toplamAlSatir, 1]);
            XLS.HucreDegerYaz(satir, baslaSutun + 3, toplamAl.toplam[toplamAlSatir, 2]);
            XLS.HucreDegerYaz(satir, baslaSutun + 4, GenelIslemler.DegisimOranBul(toplamAl.toplam[toplamAlSatir, 1], toplamAl.toplam[toplamAlSatir, 2]));
            XLS.HucreDegerYaz(satir, baslaSutun + 5, toplamAl.toplam[toplamAlSatir, 3]);
            XLS.HucreDegerYaz(satir, baslaSutun + 6, GenelIslemler.DegisimOranBul(toplamAl.toplam[toplamAlSatir, 1], toplamAl.toplam[toplamAlSatir, 3]));
            XLS.HucreDegerYaz(satir, baslaSutun + 7, toplamAl.toplam[toplamAlSatir, 4]);
            XLS.HucreDegerYaz(satir, baslaSutun + 8, GenelIslemler.DegisimOranBul(toplamAl.toplam[toplamAlSatir, 1], toplamAl.toplam[toplamAlSatir, 4]));
        }

        private string TertipAdiBul(Tertip tertip)
        {
            if (!string.IsNullOrWhiteSpace(tertip.tertipAdi)) return tertip.tertipAdi;

            if (!string.IsNullOrWhiteSpace(tertip.adlar[3]))
            {
                if (!string.IsNullOrWhiteSpace(tertip.adlar[4]))
                    return tertip.adlar[3] + " (" + tertip.adlar[4] + ")";
                else
                    return tertip.adlar[3];
            }
            for (int i = 0; i < tertip.adlar.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(tertip.adlar[i]))
                    return tertip.adlar[i];
            }

            return "";
        }

        private void ToplamYaz(int satir, ToplamAl toplam, int baslaSutun)
        {
            for (int ay = 0; ay < 12; ay++)
            {
                XLS.HucreDegerYaz(satir, baslaSutun + ay + 1, toplam.toplam[0, ay]);
            }
            XLS.HucreDegerYaz(satir, baslaSutun + 13 + 1, toplam.toplam[0, 12]);
            XLS.HucreDegerYaz(satir, baslaSutun + 14 + 1, toplam.toplam[0, 13]);
        }

        private string RaporAlGonderilenKiyas(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kurKod = (string)raporParam["KurKod"] + "";
            string kurAd = (string)raporParam["KurAd"] + "";
            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurKod, "", "", "", "");
            string birim = (string)raporParam["Birim"] + ""; ;

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
            //if (ekBilgi == "GOND_FAZ")
            //    kistasTertip.tabloKod = ENUMTabloKod.TABOdenekGondermeKY;

            kistasTertip.TertibeKodEkle(0, (string)raporParam["KurKod"], "");
            kistasTertip.TertibeKodEkle(1, (string)raporParam["FonKod"], "");
            kistasTertip.TertibeKodEkle(2, (string)raporParam["FinKod"], "");
            kistasTertip.TertibeKodEkle(3, (string)raporParam["EkoKod"], "");
            kistasTertip.TertibeKodEkle(4, (string)raporParam["PebKod"], "");
            kistasTertip.TertibeKodEkle(5, (string)raporParam["kurumsalyapi"], "");

            bool ekoIki = false;
            if (ekBilgi.EndsWith("2"))
            {
                ekBilgi = ekBilgi.Substring(0, ekBilgi.Length - 1);
                ekoIki = true;
            }

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));
            ENUMOdenekKontrolTur okt = TNS.MUH.Arac.OdenekKontrolTurBul(yil);
            if (ekoIki)
            {
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2));
                if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                    kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));
            }
            else
            {
                if (okt == ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okt == ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                {
                    kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
                }
                else
                {
                    kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));
                }
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));
            }
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(5), kistasTertip.KodNoKirilimSay(5)));
            EkDegerParam[] ekBilgiler = new EkDegerParam[4];
            int ay = TNS.BHMv2.Arac.SerbestAyBul(yil) + 1;

            string asama = (string)raporParam["Asama"];
            if (string.IsNullOrWhiteSpace(asama)) asama = "9";

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, asama + ";0/" + ay.ToString());
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "5");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, asama + ";0/7");
            ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, asama + ";");
            OdenekBilgi[] obler = servis.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, false);
            int sat = 0;
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.BosDosyaAc(sonucDosyaAd);
            XLS.HucreDegerYaz(sat, 0, "Tertip");
            XLS.HucreDegerYaz(sat, 1, "Birim");
            XLS.HucreDegerYaz(sat, 2, "Masraf Planı");
            XLS.HucreDegerYaz(sat, 3, "Gönderilen");
            XLS.HucreDegerYaz(sat, 4, "Fark");
            XLS.HucreDegerYaz(sat, 5, "6 Aylık MP");
            XLS.HucreDegerYaz(sat, 6, "Yıllık MP");
            XLS.SutunGenislikAyarla(0, 0, GenelIslemler.JexcelBirimtoExcelBirimCol(80));
            XLS.SutunGenislikAyarla(1, 1, GenelIslemler.JexcelBirimtoExcelBirimCol(25));
            XLS.SutunGenislikAyarla(2, 5, GenelIslemler.JexcelBirimtoExcelBirimCol(18));

            for (int i = 0; i < obler.Length; i++)
            {
                OdenekBilgi ob = obler[i];
                double mp = (double)ob.ekRakamlar[0];
                double gond = (double)ob.ekRakamlar[1];
                if ((ekBilgi == "HEPSI") || (mp > gond && ekBilgi == "GOND_AZ") || (mp < gond && ekBilgi == "GOND_FAZ") || ob.tertip.kodlar[5].StartsWith("9"))
                {
                    sat++;
                    string tut;
                    tut = ob.tertip.kodlar[5];
                    ob.tertip.kodlar[5] = "";
                    XLS.HucreDegerYaz(sat, 0, ob.tertip.TertipStrOl());
                    XLS.HucreDegerYaz(sat, 1, tut);
                    XLS.HucreDegerYaz(sat, 2, mp);
                    XLS.HucreDegerYaz(sat, 3, gond);
                    XLS.HucreDegerYaz(sat, 4, mp - gond);
                    XLS.HucreDegerYaz(sat, 5, (double)ob.ekRakamlar[2]);
                    XLS.HucreDegerYaz(sat, 6, (double)ob.ekRakamlar[3]);
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public class KontrolTertip
        {
            TNSCollection tc = new TNSCollection();
            public KontrolTertip(string tertipler)
            {
                if (!string.IsNullOrEmpty(tertipler))
                {
                    string[] aTertipler = tertipler.Split(',', ';');
                    foreach (string item in aTertipler)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            tc.Add(item.Split('-'));
                        }
                    }
                }
            }

            internal bool OdenekUygunMu(string[] tertipParcalar)
            {
                foreach (string[] item in tc)
                {
                    bool tertipUydu = true;
                    for (int i = 0; i < tertipParcalar.Length; i++)
                    {
                        string kont = OrtakFonksiyonlar.DiziElemanAl(item, i, "");
                        if (!string.IsNullOrWhiteSpace(kont))
                        {
                            bool uydu = tertipParcalar[i].Replace(".", "").StartsWith(kont.Replace(".", ""));
                            if (!uydu)
                            {
                                tertipUydu = false;
                                break;
                            }
                        }
                    }
                    if (tertipUydu)
                        return true;
                }
                return false;
            }
        }

        private string RaporAlHizliOdenek(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kurKod = (string)raporParam["KurKod"] + "";
            string kurAd = (string)raporParam["KurAd"] + "";
            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, kurKod, "", "", "", "");
            string birim = (string)raporParam["Birim"] + ""; ;

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
            kistasTertip.TertibeKodEkle(0, (string)raporParam["KurKod"], "");
            kistasTertip.TertibeKodEkle(1, (string)raporParam["FonKod"], "");
            kistasTertip.TertibeKodEkle(2, (string)raporParam["FinKod"], "");
            kistasTertip.TertibeKodEkle(3, (string)raporParam["EkoKod"], "");
            kistasTertip.TertibeKodEkle(4, (string)raporParam["PebKod"], "");
            kistasTertip.TertibeKodEkle(5, (string)raporParam["kurumsalyapi"], "");
            string cezali = (string)raporParam["cezali"];
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(5), kistasTertip.KodNoKirilimSay(5)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));

            TNS.MUH.ENUMOdenekKontrolTur okTur = TNS.MUH.Arac.OdenekKontrolTurBul(yil);
            if (okTur == TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okTur == TNS.MUH.ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, true));
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));

            if (okTur != TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));
            EkDegerParam[] ekBilgiler = new EkDegerParam[2];
            int ay = TNS.BHMv2.Arac.SerbestAyBul(yil) + 1;
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, ((int)ENUMMasrafPlanAsama.SONDURUMONAYLI).ToString() + ";0/" + ay.ToString());
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "0/7");
            OdenekBilgi[] obler = servis.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, false, false, false);
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.BosDosyaAc(sonucDosyaAd);
            XLS.SutunGenislikAyarla(0, 0, GenelIslemler.JexcelBirimtoExcelBirimCol(8));
            XLS.SutunGenislikAyarla(1, 1, GenelIslemler.JexcelBirimtoExcelBirimCol(5));
            XLS.SutunGenislikAyarla(2, 2, GenelIslemler.JexcelBirimtoExcelBirimCol(40));
            XLS.SutunGenislikAyarla(3, 5, GenelIslemler.JexcelBirimtoExcelBirimCol(13));

            int sat = 0;
            int saymanlik = 0;
            string eskiKyKod = "";
            bool yaz = true;

            int[] yilKontrolDizi = new int[2];
            int[] ayKontrolDizi = new int[2];
            TNS.KYM.IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

            TNS.BHMv2.Arac.OdenekKontrolDegiskenleri.Sifirla();
            TNS.BHMv2.Arac.OdenekKontrolDegiskenleri.Oku(yil);
            string tmpHataStr;
            KontrolTertip kontrolTertip = null;

            bool kontrolluOdenek = GenelIslemler.KullaniciDegiskenGetir("stratek", "OdenekKontrolYap") == "1";
            if (kontrolluOdenek)
            {
                kontrolTertip = new KontrolTertip(GenelIslemler.KullaniciDegiskenGetir("stratek", "OdenekKontrolTertipler"));
            }
            for (int i = 0; i < obler.Length; i++)
            {
                if (cezali == "1" && i == 0)
                {
                    XLS.HucreDegerYaz(sat, 7, string.Format("{0}-{1} ve {2}-{3} itibari ile Muhasebat harcaması SGB'den büyük olmayanlar ", yilKontrolDizi[0], ayKontrolDizi[0], yilKontrolDizi[1], ayKontrolDizi[1]));
                }
                string kyKod = obler[i].tertip.kodlar[5];
                if (eskiKyKod != kyKod || saymanlik == 0)
                {
                    KurumsalYapi ky = servis.KurumsalYapiDondur("tr", kyKod);
                    saymanlik = ky.saymanlikKod;
                    birim = ky.birimKod;
                    eskiKyKod = kyKod;
                    if (saymanlik > 0 && !string.IsNullOrWhiteSpace(birim))
                    {
                        XLS.HucreDegerYaz(sat, 6, GenelIslemler.KodAd(53, kyKod, true));
                        if (cezali == "1")
                        {
                            yaz = !TNS.BHMv2.Arac.SGBKBSFarkBul(servisMUH, yil, kullanan, OrtakFonksiyonlar.ConvertToInt(saymanlik, 0), birim, out tmpHataStr);
                        }
                        else
                            yaz = true;
                    }
                    else
                        yaz = false;
                }
                if (yaz)
                {
                    double mp = (double)obler[i].ekRakamlar[0];
                    double gond = (double)obler[i].ekRakamlar[1];
                    if (birim != "000" && mp > gond && !(kontrolluOdenek && kontrolTertip.OdenekUygunMu(obler[i].tertip.kodlar)))
                    {
                        XLS.HucreDegerYaz(sat, 0, saymanlik);
                        XLS.HucreDegerYaz(sat, 1, birim);
                        XLS.HucreDegerYaz(sat, 2, obler[i].tertip.kodlar[0] + "-" + obler[i].tertip.kodlar[1] + "-" + obler[i].tertip.kodlar[2] + "-" + obler[i].tertip.kodlar[3] + "-" + obler[i].tertip.kodlar[4]);
                        XLS.HucreDegerYaz(sat, 3, mp - gond);
                        XLS.HucreDegerYaz(sat, 4, mp);
                        XLS.HucreDegerYaz(sat, 5, gond);
                        sat++;
                    }
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string RaporAlHOTavan(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABMasrafPlani);
            kistasTertip.TertibeKodEkle(0, (string)raporParam["KurKod"], "");
            kistasTertip.TertibeKodEkle(1, (string)raporParam["FonKod"], "");
            kistasTertip.TertibeKodEkle(2, (string)raporParam["FinKod"], "");
            kistasTertip.TertibeKodEkle(3, (string)raporParam["EkoKod"], "");
            kistasTertip.TertibeKodEkle(4, (string)raporParam["PebKod"], "");
            kistasTertip.TertibeKodEkle(5, (string)raporParam["HrcKod"], "");
            string cezali = (string)raporParam["cezali"];
            string oranS = (string)raporParam["oran"];
            double oran = OrtakFonksiyonlar.ConvertToDouble(oranS);
            if (oran <= 0) oran = 8;
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(5), kistasTertip.KodNoKirilimSay(5)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));

            TNS.MUH.ENUMOdenekKontrolTur okTur = TNS.MUH.Arac.OdenekKontrolTurBul(yil);
            if (okTur == TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K || okTur == TNS.MUH.ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2));
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));

            if (okTur != TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K)
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));
            EkDegerParam[] ekBilgiler = new EkDegerParam[2];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMPTavan, 0, 0, "1");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "0/7");
            OdenekBilgi[] obler = servis.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, false, false, false);
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.BosDosyaAc(sonucDosyaAd);
            XLS.SutunGenislikAyarla(0, 0, GenelIslemler.JexcelBirimtoExcelBirimCol(8));
            XLS.SutunGenislikAyarla(1, 1, GenelIslemler.JexcelBirimtoExcelBirimCol(5));
            XLS.SutunGenislikAyarla(2, 2, GenelIslemler.JexcelBirimtoExcelBirimCol(40));
            XLS.SutunGenislikAyarla(3, 5, GenelIslemler.JexcelBirimtoExcelBirimCol(13));

            int sat = 0;
            int saymanlik = 0;
            string eskiKyKod = "";
            bool yaz = true;
            string birim = "";

            //int[] yilKontrolDizi = new int[2];
            //int[] ayKontrolDizi = new int[2];
            //int hizMalKontrolYil, temp;
            //TNS.KYM.IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();
            TNS.BHMv2.Arac.OdenekKontrolDegiskenleri.Sifirla();

            string tmpHataStr;

            for (int i = 0; i < obler.Length; i++)
            {
                string kyKod = obler[i].tertip.kodlar[5];
                if (eskiKyKod != kyKod || saymanlik == 0)
                {
                    KurumsalYapi ky = servis.KurumsalYapiDondur("tr", kyKod);
                    saymanlik = ky.saymanlikKod;
                    birim = ky.birimKod;
                    eskiKyKod = kyKod;
                    if (saymanlik > 0 && !string.IsNullOrWhiteSpace(birim))
                    {
                        XLS.HucreDegerYaz(sat, 6, GenelIslemler.KodAd(53, kyKod, true));
                        if (cezali == "1" || cezali == "2")
                        {
                            yaz = !TNS.BHMv2.Arac.SGBKBSFarkBul(servisMUH, yil, kullanan, OrtakFonksiyonlar.ConvertToInt(saymanlik, 0), birim, out tmpHataStr);
                        }
                        else
                            yaz = true;
                    }
                    else
                        yaz = false;
                }
                if (yaz)
                {
                    double mp = (double)obler[i].ekRakamlar[0];
                    double gonderilecek = Math.Floor(mp * oran / 100);
                    double gonderilen = (double)obler[i].ekRakamlar[1];
                    if (gonderilecek - gonderilen > 0)
                    {
                        XLS.HucreDegerYaz(sat, 0, saymanlik);
                        XLS.HucreDegerYaz(sat, 1, birim);
                        XLS.HucreDegerYaz(sat, 2, obler[i].tertip.kodlar[0] + "-" + obler[i].tertip.kodlar[1] + "-" + obler[i].tertip.kodlar[2] + "-" + obler[i].tertip.kodlar[3] + "-" + obler[i].tertip.kodlar[4]);
                        XLS.HucreDegerYaz(sat, 3, gonderilecek - gonderilen);
                        XLS.HucreDegerYaz(sat, 4, mp);
                        XLS.HucreDegerYaz(sat, 5, oran);
                        sat++;
                    }
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            if (ekBilgi == "HO")
                return RaporAlHOTavan(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "Tavan")
                return RaporAlTavan(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "TavanIcm")
                return RaporAlTavanIcm(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "TavanIcmDetay")
                return RaporAlTavanIcmDetay(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "Hizli")
                return RaporAlHizliOdenek(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "GOND_FAZ" || ekBilgi == "GOND_AZ" || ekBilgi == "GOND_FAZ2" || ekBilgi == "GOND_AZ2" || ekBilgi == "HEPSI" || ekBilgi == "HEPSI2")
                return RaporAlGonderilenKiyas(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "36")
                return RaporAlAHPMPFark(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            else if (ekBilgi == "37")
                return RaporAlAHPMPEksi(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            return RaporAlAHP(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);
            //throw new Exception("Hatalı rapor tanımı bilgisi (ekBilgi). Rapor bulunamadı!");
        }

        private string RaporAlAHPMPEksi(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            string kur = ((string)raporParam["KurKod"] + "").Replace(".", "");
            string fon = ((string)raporParam["FonKod"] + "").Replace(".", "");
            string fin = ((string)raporParam["FinKod"] + "").Replace(".", "");
            string eko = ((string)raporParam["EkoKod"] + "").Replace(".", "");
            string peb = ((string)raporParam["PebKod"] + "").Replace(".", "");
            int tur = OrtakFonksiyonlar.ConvertToInt(((string)raporParam["Tur"] + ""), 1);

            string[] sqller = new string[1];
            string[] baglar = new string[1];

            string sql = "WITH TERTIPLER AS (SELECT KURUMSALYAPIKOD+'-'+ KURKOD+'-'+ FONKOD+'-'+ FINKOD+'-'+ EKOKOD+'-'+ PEBKOD TERTIP FROM MPGMASRAFPLANITERTIP  WHERE YIL={0} AND ASAMA=5 AND DURUM=5 AND KURKOD LIKE {1} AND FONKOD LIKE {2} AND FINKOD LIKE {3} AND EKOKOD LIKE {4} AND PEBKOD LIKE {5} GROUP BY KURUMSALYAPIKOD, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD,AY HAVING SUM(ODENEK)<0) SELECT KURUMSALYAPIKOD, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY, SUM(ODENEK) FROM MPGMASRAFPLANITERTIP MP WHERE YIL={0} AND ASAMA=5 AND DURUM=5 AND KURKOD LIKE {1} AND FONKOD LIKE {2} AND FINKOD LIKE {3} AND EKOKOD LIKE {4} AND PEBKOD LIKE {5} AND 1 IN (SELECT 1 FROM TERTIPLER WHERE TERTIP=MP.KURUMSALYAPIKOD+'-'+ MP.KURKOD+'-'+ MP.FONKOD+'-'+ MP.FINKOD+'-'+ MP.EKOKOD+'-'+ MP.PEBKOD) group by KURUMSALYAPIKOD, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY order by KURUMSALYAPIKOD, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY";

            sqller[0] = string.Format(sql, yil, Yuzdele(kur), Yuzdele(fon), Yuzdele(fin), Yuzdele(eko), Yuzdele(peb));
            baglar[0] = "BaglantiSatiriBUM";

            System.Collections.SortedList sl = uzyServis.SorguBirlestir(sqller, baglar, new int[] { 0, 1, 2, 3, 4, 5, 6 });
            XLS = GenelIslemler.NewTablo();
            if (string.IsNullOrEmpty(sonucDosyaAd)) sonucDosyaAd = DosyaIslem.DosyaAdUret();

            sonucDosyaAd = sonucDosyaAd + "." + XLS.UzantiBul();
            XLS.BosDosyaAc(sonucDosyaAd);

            int satir = -1;
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            string eskiTertip = "";
            string eskiBir = "";
            foreach (System.Collections.DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                string bir = alAnahtar.StrAl(0);
                kur = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(0), alAnahtar.StrAl(1));
                fon = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(1), alAnahtar.StrAl(2));
                fin = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(2), alAnahtar.StrAl(3));
                eko = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(3), alAnahtar.StrAl(4));
                peb = OrtakFonksiyonlar.KodNoktaKoy(tUz.Kirilimlar(4), alAnahtar.StrAl(5));
                string tertip = kur + "-" + fon + "-" + fin + "-" + eko + "-" + peb;
                int ay = alAnahtar.IntAl(6);
                double od1 = alVeri.DblAl(0);

                if (eskiBir != bir || eskiTertip != tertip)
                {
                    satir++;
                    XLS.HucreDegerYaz(satir, 0, bir);
                    XLS.HucreDegerYaz(satir, 1, tertip);
                    eskiTertip = tertip;
                    eskiBir = bir;
                }
                XLS.HucreDegerYaz(satir, 1 + ay, od1);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string RaporAlAHPMPFark(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)o_kullanan;

            IBHMv2Servis servis;
            if (o_servis.GetType() == typeof(IBHMv2Servis))
                servis = (IBHMv2Servis)o_servis;
            else
                servis = TNS.BHMv2.Arac.Tanimla();

            IUZYServis uzyServis;
            if (o_servis.GetType() == typeof(IUZYServis))
                uzyServis = (IUZYServis)o_servis;
            else
                uzyServis = TNS.UZY.Arac.Tanimla();

            IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            TertipOdenek[] obler = servis.DagitilmayanlariHesapla(kullanan, yil);
            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd += "." + XLS.UzantiBul();
            XLS.BosDosyaAc(sonucDosyaAd);
            int sat = 0;
            if (obler != null)
            {
                for (int i = 0; i < obler.Length; i++)
                {
                    TertipOdenek to = obler[i];
                    XLS.HucreDegerYaz(sat, 0, to.tertip.kodlar[0]);
                    XLS.HucreDegerYaz(sat, 1, to.tertip.kodlar[1]);
                    XLS.HucreDegerYaz(sat, 2, to.tertip.kodlar[2]);
                    XLS.HucreDegerYaz(sat, 3, to.tertip.kodlar[3]);
                    XLS.HucreDegerYaz(sat, 4, to.tertip.kodlar[4]);
                    XLS.HucreDegerYaz(sat, 5, to.tertip.kodlar[5]);

                    for (int j = 0; j < 12; j++)
                    {
                        XLS.HucreDegerYaz(sat, 6 + j, to.odenek[j]);
                    }
                    sat++;
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }
}

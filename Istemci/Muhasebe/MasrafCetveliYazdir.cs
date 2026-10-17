using OrtakClass;
using System;
using System.Collections;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.MUH;
using TNS.UZY;
// Eski Istemci\ButceMuhasebe\MasrafCetveliYazdir.cs dosyasından alındı (ödenek durum / masraf cetveli raporları).
// Rapor.xml'deki NESNE adları korunsun diye isim alanı eskisi gibi ButceMuhasebe'dir.
// Değişenler: sınıflardan TNS.Raporlama.IRapor arayüzü kaldırıldı (Core'da yok; rapor motoru RaporAl'ı yansıma ile çağırır);
// ISLHarcamaAylik'ta GenelIslemler.MesajKutusu yerine "HATA: ..." metni döndürülür. Rapor kodu eskisinin aynısıdır.
namespace ButceMuhasebe
{
    public class PersonelTahminiHarcama
    {
        /*
         * 
         CREATE VIEW dbo.MUHMIFKY AS
            SELECT MUHASEBAT.YIL, MUHASEBAT.KURKOD, MUHASEBAT.FONKOD, MUHASEBAT.FINKOD, MUHASEBAT.EKOKOD, MUHASEBAT.PEBKOD,
            dbo.BHMKURUMSALYAPI.KOD AS KYKOD, MUHASEBAT.AY, 6 FORMDURUM, MUHASEBAT.BORC - MUHASEBAT.ALACAK AS HARCAMA
            FROM MUHASEBAT INNER JOIN BHMKURUMSALYAPI ON 
	            BHMKURUMSALYAPI.KURKOD = MUHASEBAT.KURKOD AND
	            BHMKURUMSALYAPI.SAYMANLIKKOD = MUHASEBAT.MUHASEBEKOD AND
	            BHMKURUMSALYAPI.BIRIMKOD = MUHASEBAT.BIRIMKOD 
            WHERE (MUHASEBAT.HESAPKOD LIKE '830%')
         * 
         * */

        Kullanici kullanan;
        int yil, ay;
        string kur;
        int onc1yil, onc1ay, onc2yil, onc2ay, onc3yil, onc3ay, son1yil, son1ay;

        public string RaporAl(object o_kullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, Hashtable raporParam, string ekBilgi)
        {
            kullanan = (Kullanici)o_kullanan;
            yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            Tertip tUz = new Tertip(onc1yil, ENUMTabloKod.TABPEBOdenek);
            ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            kur = (string)raporParam["Kur"] + "";
            AyIslemYap(yil, ay, out onc1yil, out onc1ay, -1);
            AyIslemYap(onc1yil, onc1ay, out onc2yil, out onc2ay, -1);
            AyIslemYap(onc2yil, onc2ay, out onc3yil, out onc3ay, -1);

            Tablo XLS = GenelIslemler.NewTablo();
            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int formatSat1 = 0, formatSut1 = 0, formatSat1s = 0, formatSut1s = 0, sh1 = 0, baslaSat1 = 0, baslaSut1 = 0;
            int formatSat2 = 0, formatSut2 = 0, formatSat2s = 0, formatSut2s = 0, sh2 = 0, baslaSat2 = 0, baslaSut2 = 0;
            XLS.HucreAdAdresCoz("FormatSatir1", ref sh1, ref formatSat1, ref formatSut1, ref formatSat1s, ref formatSut1s);
            XLS.HucreAdAdresCoz("BaslaSatir1", ref baslaSat1, ref baslaSut1);

            XLS.HucreAdAdresCoz("FormatSatir2", ref sh2, ref formatSat2, ref formatSut2, ref formatSat2s, ref formatSut2s);
            XLS.HucreAdAdresCoz("BaslaSatir2", ref baslaSat2, ref baslaSut2);

            XLS.HucreDegerYaz(baslaSat1 - 1, 5, $"{GenelIslemler.AyAdDondur(onc2ay)}\r\nHarcama");
            XLS.HucreDegerYaz(baslaSat1 - 1, 6, $"{GenelIslemler.AyAdDondur(onc1ay)}\r\nHarcama");
            XLS.HucreDegerYaz(baslaSat1 - 1, 7, $"{GenelIslemler.AyAdDondur(ay)}\r\nTahmin");
            XLS.HucreDegerYaz(baslaSat1 - 1, 8, $"{GenelIslemler.AyAdDondur(onc1ay)}\r\nTop. Harcama");
            OdenekBilgi[] oblerBirim;
            OdenekBilgi[] oblerTertip;
            try
            {
                oblerBirim = VeriHazirlaBirim();
                oblerTertip = VeriHazirlaTertip();
            }
            catch (Exception ex)
            {
                XLS.HucreDegerYaz(baslaSat1, baslaSut1, ex.Message);
                oblerBirim = null;
                oblerTertip = null;
            }
            if (oblerBirim != null)
            {
                int satir = baslaSat1;
                foreach (var item in oblerBirim)
                {
                    int icSut = baslaSut1;
                    XLS.HucreKopyala(formatSat1, formatSut1, formatSat1s, formatSut1s, satir, icSut);
                    string kyKod = item.tertip.kodlar[5];
                    int sayKod;
                    string birimKod;
                    SaymanlikBirimKodBul(kyKod.Replace(".", ""), out sayKod, out birimKod);
                    item.tertip.kodlar[5] = "";
                    XLS.HucreDegerYaz(satir, icSut++, sayKod);
                    XLS.HucreDegerYaz(satir, icSut++, birimKod);
                    XLS.HucreDegerYaz(satir, icSut++, item.tertip.kodlar[3]);
                    XLS.HucreDegerYaz(satir, icSut++, item.tertip.TertipStrOl());
                    XLS.HucreDegerYaz(satir, icSut++, TNS.BHMv2.Arac.ProgramTertipOl(item.tertip, false));
                    XLS.HucreDegerYaz(satir, icSut++, HarcamaBul(onc2ay, item.EkRakam(2), item.EkRakam(1)));
                    XLS.HucreDegerYaz(satir, icSut++, HarcamaBul(onc1ay, item.EkRakam(3), item.EkRakam(2)));
                    icSut++;
                    XLS.HucreDegerYaz(satir, icSut++, item.EkRakam(3));
                    XLS.HucreDegerYaz(satir, icSut++, item.EkRakam(0));
                    XLS.HucreDegerYaz(satir, icSut++, item.EkRakam(4));
                    satir++;
                }

                XLS.AktifSheetDegistir(sh2);
                satir = baslaSat2;
                foreach (var item in oblerTertip)
                {
                    int icSut = baslaSut2;
                    XLS.HucreKopyala(formatSat2, formatSut2, formatSat2s, formatSut2s, satir, icSut);
                    XLS.HucreDegerYaz(satir, icSut++, item.tertip.TertipStrOl());
                    XLS.HucreDegerYaz(satir, icSut++, TNS.BHMv2.Arac.ProgramTertipOl(item.tertip, false));
                    XLS.HucreDegerYaz(satir, icSut++, item.EkRakam(0));
                    XLS.HucreDegerYaz(satir, icSut++, item.EkRakam(1));
                    XLS.HucreDegerYaz(satir, icSut++, item.EkRakam(2));
                    XLS.HucreDegerYaz(satir, icSut++, item.EkRakam(3));
                    satir++;
                }
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private OdenekBilgi[] VeriHazirlaTertip()
        {
            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABMasrafPlani, kur, "", "", "01/02.99");

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            for (int i = 0; i < 5; i++)
            {
                int uzunluk;
                if (i == 3)
                    uzunluk = 2;
                else
                    uzunluk = kistas.KodNoKirilimSay(i);
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(i), uzunluk, true));
            }

            EkDegerParam[] ekBilgiler = new EkDegerParam[4];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, "9;0/");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMasrafPlani, 0, 0, "9;0/" + (ay + 1).ToString());
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "5");
            ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "96,99");
            return TNS.BHMv2.Arac.Tanimla().ListeTertipOl((Kullanici)kullanan, kistas, kodBilgileri, ekBilgiler, true, false, false, -1);
        }

        private OdenekBilgi[] VeriHazirlaBirim()
        {
            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABOdenekGondermeKY, kur, "", "", "01/02.99");

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            for (int i = 0; i < 6; i++)
            {
                int uzunluk;
                if (i == 3)
                    uzunluk = 2;
                else
                    uzunluk = kistas.KodNoKirilimSay(i);
                kodBilgileri.Add(new UcgenKodBilgi(kistas.KodNolar(i), uzunluk, true));
            }

            EkDegerParam[] ekBilgiler = new EkDegerParam[5];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "5");
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABMIFKY, yil - onc3yil, 0, (onc3ay).ToString());
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABMIFKY, yil - onc2yil, 0, (onc2ay).ToString());
            ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABMIFKY, yil - onc1yil, 0, onc1ay.ToString());
            ekBilgiler[4] = new EkDegerParam(ENUMTabloKod.TABOdenekGondermeKY, 0, 0, "99,96");
            return TNS.BHMv2.Arac.Tanimla().ListeTertipOl((Kullanici)kullanan, kistas, kodBilgileri, ekBilgiler, true, false, false, -1);
        }

        private double HarcamaBul(int ay, double ayHarcama, double oncAyHarcama)
        {
            if (ay == 1) return ayHarcama;
            else return ayHarcama - oncAyHarcama;
        }

        private void AyIslemYap(int yil, int ay, out int sonucYil, out int sonucAy, int ekle)
        {
            sonucYil = yil;
            sonucAy = ay + ekle;
            if (sonucAy < 1)
            {
                sonucAy = 12;
                sonucYil--;
            }
            else if (sonucAy > 12)
            {
                sonucAy = 1;
                sonucYil++;
            }
        }

        private void SaymanlikBirimKodBul(string kyKod, out int sayKod, out string birimKod)
        {
            if (string.IsNullOrEmpty(kyKod) || kyKod.Length < 11)
            {
                sayKod = 0;
                birimKod = "000";
            }
            else
            {
                sayKod = OrtakFonksiyonlar.ConvertToInt(kyKod.Substring(1, 5), 0);
                birimKod = kyKod.Substring(7, 3);
            }
        }
    }

    /// <summary>
    /// Masraf Cetveli Tertip işlemlerini düzenleyen sınıf
    /// </summary>
    public class MasrafCetveliTertip
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Masraf Cetveli Tertip raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int satirD = 0;
            int sutunD = 0;
            int prgColBasla = 26;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"] + "";
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());

            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            OdenekDurumKriter kr = new OdenekDurumKriter();
            TNS.MUH.Arac.EkoUzunlukOl(kr, (string)raporParam["EkoKirilim"]);
            kr.yeniMetot = true;
            kr.yil = yil;
            kr.ay = ay;
            kr.muhasebe = muhasebe;
            kr.birim = birim;
            kr.harcamaGoster = true;
            kr.avansGoster = true;
            kr.odenekGondermeGoster = true;
            kr.odemeEmriGoster = true;
            kr.harcamaBlokeGoster = true;
            kr.detay = ENUMOdenekDurumDetay.TERTIP;
            kr.faaliyetGoster = true;
            kr.harcamaPebli = kr.faaliyetGoster;

            kr.tertip.kodlar[0] = kur;
            kr.tertip.kodlar[1] = fon;
            kr.tertip.kodlar[2] = fin;
            kr.tertip.kodlar[3] = eko;
            kr.tertip.kodlar[4] = peb;
            kr.ekoAdBul = true;
            ObjectArray oVAN = servisMUH.OdenekDurumListesi(kr);
            string tButce = "";
            string egButce = "";
            double harcamaToplam = 0;
            double avansToplam = 0;
            double blokeEdilenToplam = 0;
            int toplamSatir = 0;
            int baslaSatir = satir;
            bool odenekGondermeEko2 = TNS.MUH.Arac.OdenekGondermeEko2liMi(kr.yil);
            bool ciftSatir = (kr.ekoKodKirilim == 4) && odenekGondermeEko2;
            bool ilkSatir = true;

            double onayliOdenekGondermeToplam = 0;
            double onayliTenkisToplam = 0;
            double onayliOdenekGondermeTenkisToplam = 0;
            double serbestKalanToplam = 0;
            double oeToplam = 0;

            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, false);
            int eko3Uz = tUzBul.KodUzunlukBul(3, 3, false);
            int eko4Uz = tUzBul.KodUzunlukBul(3, 4, true);
            foreach (TNS.MUH.OdenekDurum o in oVAN.objeler)
            {
                XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir, sutun);

                tButce = o.tertipButce.TertipStrOl();
                string gButce = o.tertipButce.kodlar[0] + "-" + o.tertipButce.kodlar[1] + "-" + o.tertipButce.kodlar[2] + "-" +
TNS.BKD.Arac.Eko23Dondur(yil, o.tertipButce.kodlar[3], eko2Uz, eko3Uz);
                if (kr.faaliyetGoster)
                {
                    gButce += "-" + o.tertipButce.kodlar[4];
                }
                if (ilkSatir)
                {
                    if (tButce.Length == gButce.Length && ciftSatir == false) ciftSatir = true;
                    ilkSatir = false;
                }

                XLS.HucreDegerYaz(satir, 0, tButce);
                XLS.HucreDegerYaz(satir, 1, o.tertipButce.adlar[3]);
                XLS_HucreDegerYazPRG(XLS, satir, prgColBasla, o.tertipButce);
                if (ciftSatir && egButce != gButce)
                {
                    if (toplamSatir > 0)
                    {
                        if (!odenekGondermeEko2)
                        {
                            XLS.HucreDegerYaz(toplamSatir, 8, onayliOdenekGondermeToplam);
                            XLS.HucreDegerYaz(toplamSatir, 9, onayliTenkisToplam);
                            XLS.HucreDegerYaz(toplamSatir, 10, onayliOdenekGondermeTenkisToplam);
                            XLS.HucreDegerYaz(toplamSatir, 11, serbestKalanToplam);
                        }
                        XLS.HucreDegerYaz(toplamSatir, 12, harcamaToplam);
                        XLS.HucreDegerYaz(toplamSatir, 13, avansToplam);
                        XLS.HucreDegerYaz(toplamSatir, 14, blokeEdilenToplam);
                        XLS.HucreDegerYaz(toplamSatir, 15, oeToplam);
                        toplamSatir = 0;
                    }

                    harcamaToplam = 0;
                    avansToplam = 0;
                    blokeEdilenToplam = 0;
                    oeToplam = 0;
                    onayliOdenekGondermeToplam = 0;
                    onayliTenkisToplam = 0;
                    onayliOdenekGondermeTenkisToplam = 0;
                    serbestKalanToplam = 0;
                    egButce = gButce;
                    toplamSatir = satir;
                }
                if (gButce == tButce || !ciftSatir)
                {
                    XLS.HucreDegerYaz(satir, 2, o.kbo);
                    XLS.HucreDegerYaz(satir, 3, o.eklenen);
                    XLS.HucreDegerYaz(satir, 4, o.dusulen);
                    XLS.HucreDegerYaz(satir, 5, o.toplamOdenek);
                    XLS.HucreDegerYaz(satir, 6, o.serbestBul);
                    XLS.HucreDegerYaz(satir, 7, o.blokeBul);
                    if (odenekGondermeEko2)
                    {
                        XLS.HucreDegerYaz(satir, 8, o.onayliOdenekGonderme);
                        XLS.HucreDegerYaz(satir, 9, o.onayliTenkis);
                        XLS.HucreDegerYaz(satir, 10, o.onayliOdenekGonderme + o.onayliTenkis);
                        XLS.HucreDegerYaz(satir, 11, o.serbestBul - (o.onayliOdenekGonderme + o.onayliTenkis));
                    }
                }
                if (!odenekGondermeEko2)
                {
                    XLS.HucreDegerYaz(satir, 8, o.onayliOdenekGonderme);
                    XLS.HucreDegerYaz(satir, 9, o.onayliTenkis);
                    XLS.HucreDegerYaz(satir, 10, o.onayliOdenekGonderme + o.onayliTenkis);
                    if (!ciftSatir)
                        XLS.HucreDegerYaz(satir, 11, o.serbestBul - (o.onayliOdenekGonderme + o.onayliTenkis));
                }
                XLS.HucreDegerYaz(satir, 12, o.harcama);
                XLS.HucreDegerYaz(satir, 13, o.avans);
                XLS.HucreDegerYaz(satir, 14, o.blokeEdilen);
                XLS.HucreDegerYaz(satir, 15, o.onayliOdemeEmri + o.onaysizOdemeEmri);

                onayliOdenekGondermeToplam += o.onayliOdenekGonderme;
                onayliTenkisToplam += o.onayliTenkis;
                onayliOdenekGondermeTenkisToplam += (o.onayliOdenekGonderme + o.onayliTenkis);
                serbestKalanToplam += (o.serbestBul - (o.onayliOdenekGonderme + o.onayliTenkis));
                harcamaToplam += o.harcama;
                avansToplam += o.avans;
                blokeEdilenToplam += o.blokeEdilen;
                oeToplam += (o.onayliOdemeEmri + o.onaysizOdemeEmri);
                satir++;
            }

            if (toplamSatir > 0)
            {
                if (!odenekGondermeEko2)
                {
                    XLS.HucreDegerYaz(toplamSatir, 8, onayliOdenekGondermeToplam);
                    XLS.HucreDegerYaz(toplamSatir, 9, onayliTenkisToplam);
                    XLS.HucreDegerYaz(toplamSatir, 10, onayliOdenekGondermeTenkisToplam);
                    XLS.HucreDegerYaz(toplamSatir, 11, serbestKalanToplam);
                }
                XLS.HucreDegerYaz(toplamSatir, 12, harcamaToplam);
                XLS.HucreDegerYaz(toplamSatir, 13, avansToplam);
                XLS.HucreDegerYaz(toplamSatir, 14, blokeEdilenToplam);
                XLS.HucreDegerYaz(toplamSatir, 15, oeToplam);
                XLS.SutunGizle(prgColBasla, prgColBasla + 20, true);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void XLS_HucreDegerYazPRG(Tablo XLS, int satir, int kolBasla, Tertip tButce)
        {
            string tut = TNS.BHMv2.Arac.ProgramTertipOl(tButce, false);
            string[] tutP = tut.Split('.', '-');
            XLS.HucreDegerYaz(satir, kolBasla++, tut);
            foreach (string item in tutP)
            {
                XLS.HucreDegerYaz(satir, kolBasla++, item);
            }
        }
    }

    /// <summary>
    /// Masraf Cetveli Tertip işlemlerini düzenleyen sınıf
    /// </summary>
    public class MasrafCetveliTertipKur
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        int satir = 0;
        int sutun = 0;
        int satirD = 0;
        int sutunD = 0;
        Hashtable htKur = new Hashtable();
        Hashtable htEko = new Hashtable();
        Kullanici kullanan;
        /// <summary>
        /// Masraf Cetveli Tertip raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";
            int kirilim = 2;
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());

            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            OdenekDurumKriter kr = new OdenekDurumKriter();
            kr.yeniMetot = true;
            kr.yil = yil;
            kr.ay = ay;
            kr.ekoKodKirilim = kirilim;
            kr.harcamaGoster = true;
            kr.avansGoster = true;
            kr.odenekGondermeGoster = false;
            kr.odemeEmriGoster = false;
            kr.harcamaBlokeGoster = true;
            kr.detay = ENUMOdenekDurumDetay.TERTIP;
            kr.tertip.kodlar[0] = kur;
            kr.tertip.kodlar[1] = fon;
            kr.tertip.kodlar[2] = fin;
            kr.tertip.kodlar[3] = eko;
            kr.tertip.kodlar[4] = peb;
            ObjectArray oVAN = servisMUH.OdenekDurumListesi(kr);
            string eskiKur = "";
            string eskiKurAd = "";
            int kurSatir = 0;
            ToplamAl toplamAl = new ToplamAl(3, 8);
            toplamAl.SatirSifirla(2);
            toplamAl.SatirSifirla(1);
            foreach (TNS.MUH.OdenekDurum o in oVAN.objeler)
            {
                string gelenKur = o.tertipButce.kodlar[0];
                if (gelenKur != eskiKur)
                {
                    if (kurSatir > 0)
                    {
                        SatirYaz(XLS, kurSatir, toplamAl, 1, eskiKur, eskiKurAd);
                        toplamAl.SatirSifirla(1);
                    }
                    eskiKur = gelenKur;
                    eskiKurAd = AdBul(1, htKur, yil, o.tertipButce.kodlar[0]);
                    kurSatir = satir;
                    satir++;
                }
                toplamAl.DegerAta(0, 0, o.kbo);
                toplamAl.DegerAta(0, 1, o.eklenen);
                toplamAl.DegerAta(0, 2, o.dusulen);
                toplamAl.DegerAta(0, 3, o.toplamOdenek);
                toplamAl.DegerAta(0, 4, o.harcama);
                toplamAl.DegerAta(0, 5, o.avans);
                toplamAl.DegerAta(0, 6, o.blokeEdilen);
                toplamAl.DegerAta(0, 7, o.toplamOdenek - (o.harcama + o.avans + o.blokeEdilen));
                toplamAl.ToplamEkle(1, 0);
                toplamAl.ToplamEkle(2, 0);
                SatirYaz(XLS, satir, toplamAl, 0, o.tertipButce.TertipStrOl(), AdBul(4, htEko, yil, o.tertipButce.kodlar[3]));
                satir++;
            }
            if (kurSatir > 0)
            {
                SatirYaz(XLS, kurSatir, toplamAl, 1, eskiKur, eskiKurAd);
                SatirYaz(XLS, satir, toplamAl, 2, "", "Kurum Toplamı");
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private string AdBul(int kodTur, Hashtable ht, int yil, string kod)
        {
            string ad = (string)ht[kod];
            if (string.IsNullOrEmpty(ad))
            {
                ad = servisUZY.UzayDegeriStr(kullanan, (kodTur == 1 ? "ABSKUR" : "ABSEKO"), yil.ToString() + "-" + kod, true);
                ht[kod] = ad;
            }
            return ad;
        }

        private void SatirYaz(Tablo XLS, int satir, ToplamAl toplamAl, int toplamSatir, string tertip, string tertipAd)
        {
            XLS.HucreKopyala(satirD + toplamSatir, sutunD, satirD + toplamSatir, sutunD + 10, satir, sutunD);
            XLS.HucreDegerYaz(satir, 0, tertip);
            XLS.HucreDegerYaz(satir, 1, tertipAd);

            XLS.HucreDegerYaz(satir, 2, toplamAl.toplam[toplamSatir, 0]);
            XLS.HucreDegerYaz(satir, 3, toplamAl.toplam[toplamSatir, 1]);
            XLS.HucreDegerYaz(satir, 4, toplamAl.toplam[toplamSatir, 2]);
            XLS.HucreDegerYaz(satir, 5, toplamAl.toplam[toplamSatir, 3]);
            XLS.HucreDegerYaz(satir, 6, toplamAl.toplam[toplamSatir, 4]);
            XLS.HucreDegerYaz(satir, 7, toplamAl.toplam[toplamSatir, 5]);
            XLS.HucreDegerYaz(satir, 8, toplamAl.toplam[toplamSatir, 6]);
            XLS.HucreDegerYaz(satir, 9, toplamAl.toplam[toplamSatir, 7]);
        }

        //private void KurSatirYaz()
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class MasrafCetveliMuhasebatSgb
    {
        public class Veriler : IComparable
        {
            public int saymanlik;
            public string birim;
            public string saymanlikad;
            public string birimad;
            public string kur;
            public string fon;
            public string fin;
            public string eko;
            public string peb;
            public double onaysizGond;
            public double onaysizTenk;
            public double onayliGond;
            public double onayliTenk;
            public double yevmHarc;
            public double topHarc;

            public int CompareTo(object obj)
            {
                if (!(obj is Veriler)) return -1;
                Veriler gelen = (Veriler)obj;
                int k;
                if ((k = this.saymanlik.CompareTo(gelen.saymanlik)) != 0) return k;
                if ((k = this.birim.CompareTo(gelen.birim)) != 0) return k;
                if ((k = this.kur.CompareTo(gelen.kur)) != 0) return k;
                if ((k = this.fon.CompareTo(gelen.fon)) != 0) return k;
                if ((k = this.fin.CompareTo(gelen.fin)) != 0) return k;
                if ((k = this.eko.CompareTo(gelen.eko)) != 0) return k;
                if ((k = this.peb.CompareTo(gelen.peb)) != 0) return k;
                return this.onayliGond.CompareTo(gelen.onayliGond);
            }
        }

        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        private string[] sqller;
        private string[] baglantilar;

        public string RaporAl(object okullanan, object servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Kullanici kullanan = (Kullanici)okullanan;
            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int saymanlik = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";
            int kirilim = OrtakFonksiyonlar.ConvertToInt((string)raporParam["EkoKirilim"], 0);
            if (kirilim == 23) kirilim = 2;

            sqller = new string[6];
            baglantilar = new string[6];
            baglantilar[5] = baglantilar[4] = baglantilar[3] = baglantilar[2] = baglantilar[1] = baglantilar[0] = "BaglantiSatiriBUM";
            sqller[0] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 1, ENUMBelgeTuru.ODENEKGONDERME, false, kur, fon, fin, eko); // Yeni, Ödenek gönderme, faaliyet yok
            sqller[1] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 1, ENUMBelgeTuru.TENKIS, false, kur, fon, fin, eko); // Yeni, Tenkis, faaliyet yok
            sqller[2] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 5, ENUMBelgeTuru.ODENEKGONDERME, false, kur, fon, fin, eko); // Onaylı, Ödenek gönderme, faaliyet yok
            sqller[3] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 5, ENUMBelgeTuru.TENKIS, false, kur, fon, fin, eko); // Onaylı, Tenkis, faaliyet yok
            sqller[4] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 1, ENUMBelgeTuru.ODEMEEMRI, false, kur, fon, fin, eko); // Yeni, Ödenek gönderme, faaliyet yok
            sqller[5] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 6, ENUMBelgeTuru.ODEMEEMRI, false, kur, fon, fin, eko); // Yeni, Tenkis, faaliyet yok

            SortedList sl = servisUZY.SorguBirlestir(sqller, baglantilar, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });

            ArrayList al = new ArrayList();
            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                Veriler ver = new Veriler();

                ver.saymanlik = alAnahtar.IntAl(0);
                ver.birim = alAnahtar.StrAl(1);

                ver.kur = alAnahtar.StrAl(2);
                ver.fon = alAnahtar.StrAl(3);
                ver.fin = alAnahtar.StrAl(4);
                ver.eko = alAnahtar.StrAl(5);
                ver.peb = "";
                ver.saymanlikad = alAnahtar.StrAl(6);
                ver.birimad = alAnahtar.StrAl(7);

                ver.onaysizGond = alVeri.DblAl(0);
                ver.onaysizTenk = alVeri.DblAl(1);
                ver.onayliGond = alVeri.DblAl(2);
                ver.onayliTenk = alVeri.DblAl(3);
                ver.topHarc = alVeri.DblAl(4);
                ver.yevmHarc = alVeri.DblAl(5);
                al.Add(ver);
            }

            sqller = new string[6];
            baglantilar = new string[6];
            baglantilar[5] = baglantilar[4] = baglantilar[3] = baglantilar[2] = baglantilar[1] = baglantilar[0] = "BaglantiSatiriBUM";
            sqller[0] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 1, ENUMBelgeTuru.ODENEKGONDERME, true, kur, fon, fin, eko); // Yeni, Ödenek gönderme, faaliyet yok
            sqller[1] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 1, ENUMBelgeTuru.TENKIS, true, kur, fon, fin, eko); // Yeni, Tenkis, faaliyet yok
            sqller[2] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 5, ENUMBelgeTuru.ODENEKGONDERME, true, kur, fon, fin, eko); // Onaylı, Ödenek gönderme, faaliyet yok
            sqller[3] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 5, ENUMBelgeTuru.TENKIS, true, kur, fon, fin, eko); // Onaylı, Tenkis, faaliyet yok
            sqller[4] = servisMUH.OdenekGondermeSorgu(yil, saymanlik, birim, 1, ENUMBelgeTuru.ODEMEEMRI, true, kur, fon, fin, eko); // Yeni, Ödenek gönderme, faaliyet yok
            sqller[5] = null;

            sl = servisUZY.SorguBirlestir(sqller, baglantilar, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });

            foreach (DictionaryEntry de in sl)
            {
                TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;

                Veriler ver = new Veriler();

                ver.saymanlik = alAnahtar.IntAl(0);
                ver.birim = alAnahtar.StrAl(1);

                ver.kur = alAnahtar.StrAl(2);
                ver.fon = alAnahtar.StrAl(3);
                ver.fin = alAnahtar.StrAl(4);
                ver.eko = alAnahtar.StrAl(5);
                ver.peb = alAnahtar.StrAl(6);
                ver.saymanlikad = alAnahtar.StrAl(7);
                ver.birimad = alAnahtar.StrAl(8);

                ver.onaysizGond = alVeri.DblAl(0);
                ver.onaysizTenk = alVeri.DblAl(1);
                ver.onayliGond = alVeri.DblAl(2);
                ver.onayliTenk = alVeri.DblAl(3);
                ver.topHarc = alVeri.DblAl(4);
                al.Add(ver);
            }
            al.Sort();

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int satirD = 0;
            int sutunD = 0;
            int satirT = 0;
            int sutunT = 0;
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());

            XLS.HucreAdAdresCoz("txtToplamSatir", ref satirT, ref sutunT);
            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);
            bool yaz = true;
            XLS.HucreDegerYaz(satir - 4, sutun + 11, "KALAN");
            foreach (Veriler item in al)
            {
                int kaynakSatir = 0;
                if (string.IsNullOrWhiteSpace(item.peb))
                {
                    yaz = true;
                    if (item.yevmHarc > item.topHarc)
                    {
                        kaynakSatir = satirT + 1;
                        if (kirilim == 2)
                            yaz = false;
                    }
                    else
                        kaynakSatir = satirT;
                }
                else
                    kaynakSatir = satirD;

                if (yaz)
                {
                    XLS.HucreKopyala(kaynakSatir, sutunD, kaynakSatir, sutunD + 20, satir, sutun);
                    XLS.HucreDegerYaz(satir, sutun, item.saymanlik);
                    XLS.HucreDegerYaz(satir, sutun + 1, item.birim);
                    XLS.HucreDegerYaz(satir, sutun + 2, item.kur + "-" + item.fon + "-" + item.fin + "-" + item.eko + "-" + item.peb);
                    XLS.HucreDegerYaz(satir, sutun + 3, item.onaysizGond);
                    XLS.HucreDegerYaz(satir, sutun + 4, item.onaysizTenk);
                    XLS.HucreDegerYaz(satir, sutun + 5, item.onayliGond);
                    XLS.HucreDegerYaz(satir, sutun + 6, item.onayliTenk);
                    double top = item.onayliGond + item.onayliTenk + item.onaysizGond + item.onaysizTenk;
                    XLS.HucreDegerYaz(satir, sutun + 7, top);
                    XLS.HucreDegerYaz(satir, sutun + 9, item.yevmHarc);
                    XLS.HucreDegerYaz(satir, sutun + 10, item.topHarc);
                    XLS.HucreDegerYaz(satir, sutun + 11, top - item.yevmHarc);
                    XLS.HucreDegerYaz(satir, sutun + 12, top - item.topHarc);
                    satir++;
                }
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Masraf Cetveli Muhasebe işlemlerini düzenleyen sınıf
    /// </summary>
    public class MasrafCetveliMuhasebe
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Masraf Cetveli Muhasebe raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int satirD = 0;
            int sutunD = 0;
            int satirT = 0;
            int sutunT = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"] + "";
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());

            XLS.HucreAdAdresCoz("txtToplamSatir", ref satirT, ref sutunT);
            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            OdenekDurumKriter kr = new OdenekDurumKriter();
            kr.yil = yil;
            kr.ay = ay;
            kr.muhasebe = muhasebe;
            kr.birim = birim;
            TNS.MUH.Arac.EkoUzunlukOl(kr, (string)raporParam["EkoKirilim"]);
            kr.harcamaGoster = true;
            kr.odenekGondermeGoster = true;
            kr.odenekGoster = false;
            kr.odemeEmriGoster = true;
            kr.mifGoster = true;
            kr.faaliyetGoster = !string.IsNullOrEmpty(ekBilgi) && (ekBilgi == "FAALIYETYOK" || ekBilgi == "FARKVAR") ? false : true;
            kr.harcamaPebli = kr.faaliyetGoster || ekBilgi == "PRGBUTCETERTIP";

            bool farkRapor = ekBilgi == "FARKVAR";
            if (true && farkRapor)
            {
                kr.faaliyetGoster = true;
                kr.harcamaPebli = true;
            }
            kr.yeniMetot = true;
            if (sablonAd.IndexOf("Muhasebe") > 0)
                kr.detay = ENUMOdenekDurumDetay.MUHASEBE;
            else
                kr.detay = ENUMOdenekDurumDetay.BIRIM;

            kr.tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            kr.tertip.kodlar[0] = kur;
            kr.tertip.kodlar[1] = fon;
            kr.tertip.kodlar[2] = fin;
            kr.tertip.kodlar[3] = eko;
            kr.tertip.kodlar[4] = peb;

            ObjectArray oVAN = servisMUH.HarcamaDurumListesi(kullanan, kr);

            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            int eko2Uz = tUzBul.KodUzunlukBul(3, 2, false);
            int eko3Uz = tUzBul.KodUzunlukBul(3, 3, false);

            if (!kr.eko23 && kr.ekoKodKirilim <= 2)
                eko3Uz = eko2Uz;

            TNS.MUH.OdenekDurum.eko2Uz = eko2Uz;
            TNS.MUH.OdenekDurum.eko3Uz = eko3Uz;

            string tButce;
            string egButce = "";
            double harcamaToplam = 0;
            double odemeEmriToplam = 0;
            double gonderilenToplam = 0;
            double odenekToplam = 0;
            double avansToplam = 0;
            int toplamSatir = 0;

            string bir;
            string eBir = "";
            int muh;
            int eMuh = 0;

            double birOdeOnayliToplam = 0;
            double birTenOnayliToplam = 0;
            double birOdeOnaysizToplam = 0;
            double birTenOnaysizToplam = 0;
            double birHarcamaToplam = 0;
            double birOdemeEmriToplam = 0;
            double birAvansToplam = 0;
            double birOdenekToplam = 0;
            double muhOdeOnayliToplam = 0;
            double muhTenOnayliToplam = 0;
            double muhOdeOnaysizToplam = 0;
            double muhTenOnaysizToplam = 0;
            double muhHarcamaToplam = 0;
            double muhOdemeEmriToplam = 0;
            double muhAvansToplam = 0;
            double muhOdenekToplam = 0;

            int muhToplamSatir = 0;
            int birToplamSatir = 0;
            bool ciftSatir = kr.ekoKodKirilim == 4 && TNS.MUH.Arac.OdenekGondermeEko2liMi(kr.yil);

            Hashtable htPrgIliski = TNS.BKD.Arac.ProgramButceIliskiHazirla(kullanan, kr.yil);
            string trp;
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
            foreach (TNS.MUH.OdenekDurum o in oVAN.objeler)
            {
                muh = o.muhasebe;
                bir = o.birim;
                tButce = o.tertipButce.TertipStrOl();
                string gButce = o.tertipButce.kodlar[0] + "-" + o.tertipButce.kodlar[1] + "-" + o.tertipButce.kodlar[2] + "-" + TNS.BKD.Arac.Eko23Dondur(yil, o.tertipButce.kodlar[3], eko2Uz, eko3Uz);
                if (kr.harcamaPebli)
                    gButce += "-" + o.tertipButce.kodlar[4];
                double fark = o.harcama - (o.onayliMIF + o.onayliOdemeEmri + o.onaysizMIF + o.onaysizOdemeEmri);
                int vurguluSatir = 0;
                if (farkRapor)
                {
                    if (fark > 0.001)
                        vurguluSatir = -1;
                }

                if (muh != eMuh)
                {
                    XLS.HucreKopyala(satirT, sutunT, satirT, sutunT + 20, satir, sutun);

                    if (muhToplamSatir > 0)
                    {
                        XLS.HucreDegerYaz(muhToplamSatir, 3, muhOdeOnaysizToplam);
                        XLS.HucreDegerYaz(muhToplamSatir, 4, muhTenOnaysizToplam);
                        XLS.HucreDegerYaz(muhToplamSatir, 5, muhOdeOnayliToplam);
                        XLS.HucreDegerYaz(muhToplamSatir, 6, muhTenOnayliToplam);
                        XLS.HucreDegerYaz(muhToplamSatir, 7, (muhOdeOnaysizToplam + muhOdeOnayliToplam) + (muhTenOnayliToplam + muhTenOnaysizToplam));
                        XLS.HucreDegerYaz(muhToplamSatir, 8, muhOdenekToplam);
                        XLS.HucreDegerYaz(muhToplamSatir, 9, muhHarcamaToplam);
                        XLS.HucreDegerYaz(muhToplamSatir, 10, muhOdemeEmriToplam);
                        if (!farkRapor)
                            XLS.HucreDegerYaz(muhToplamSatir, 11, muhAvansToplam);
                        else
                        {
                            XLS.HucreDegerYaz(muhToplamSatir, 11, muhOdenekToplam - muhOdemeEmriToplam);
                            XLS.HucreDegerYaz(muhToplamSatir, 12, muhHarcamaToplam - muhOdemeEmriToplam);
                        }
                    }
                    XLS.HucreDegerYaz(satir, 0, o.muhasebe);
                    XLS.HucreDegerYaz(satir, 2, o.MuhasebeAd(kullanan, uzyServis));
                    muhOdeOnaysizToplam = 0;
                    muhTenOnaysizToplam = 0;
                    muhOdeOnayliToplam = 0;
                    muhTenOnayliToplam = 0;
                    muhHarcamaToplam = 0;
                    muhOdemeEmriToplam = 0;
                    muhAvansToplam = 0;
                    muhOdenekToplam = 0;
                    muhToplamSatir = satir;
                    satir++;
                    eBir = "";
                    egButce = "";
                }

                if (bir != eBir)
                {
                    XLS.HucreKopyala(satirT, sutunT, satirT, sutunT + 20, satir, sutun);

                    if (birToplamSatir > 0)
                    {
                        XLS.HucreDegerYaz(birToplamSatir, 3, birOdeOnaysizToplam);
                        XLS.HucreDegerYaz(birToplamSatir, 4, birTenOnaysizToplam);
                        XLS.HucreDegerYaz(birToplamSatir, 5, birOdeOnayliToplam);
                        XLS.HucreDegerYaz(birToplamSatir, 6, birTenOnayliToplam);
                        XLS.HucreDegerYaz(birToplamSatir, 7, (birOdeOnaysizToplam + birOdeOnayliToplam) + (birTenOnayliToplam + birTenOnaysizToplam));
                        XLS.HucreDegerYaz(birToplamSatir, 8, birOdenekToplam);
                        XLS.HucreDegerYaz(birToplamSatir, 9, birHarcamaToplam);
                        XLS.HucreDegerYaz(birToplamSatir, 10, birOdemeEmriToplam);

                        if (!farkRapor)
                            XLS.HucreDegerYaz(birToplamSatir, 11, birAvansToplam);
                        else
                        {
                            XLS.HucreDegerYaz(birToplamSatir, 11, birOdenekToplam - birOdemeEmriToplam);
                            XLS.HucreDegerYaz(birToplamSatir, 12, birHarcamaToplam - birOdemeEmriToplam);
                        }
                    }

                    XLS.HucreDegerYaz(satir, 1, o.birim);
                    XLS.HucreDegerYaz(satir, 2, o.BirimAd(kullanan, uzyServis));
                    birOdeOnaysizToplam = 0;
                    birTenOnaysizToplam = 0;
                    birOdeOnayliToplam = 0;
                    birTenOnayliToplam = 0;
                    birHarcamaToplam = 0;
                    birOdemeEmriToplam = 0;
                    birAvansToplam = 0;
                    birOdenekToplam = 0;
                    birToplamSatir = satir;
                    satir++;
                    egButce = "";
                }
                if (ciftSatir && egButce != gButce)
                {
                    if (toplamSatir > 0)
                    {
                        XLS.HucreDegerYaz(toplamSatir, 9, harcamaToplam);
                        XLS.HucreDegerYaz(toplamSatir, 10, odemeEmriToplam);
                        if (!farkRapor)
                            XLS.HucreDegerYaz(toplamSatir, 11, avansToplam);
                        else
                        {
                            XLS.HucreDegerYaz(toplamSatir, 11, gonderilenToplam - (odemeEmriToplam));
                            XLS.HucreDegerYaz(toplamSatir, 12, harcamaToplam - odemeEmriToplam);
                        }
                    }
                    harcamaToplam = 0;
                    odemeEmriToplam = 0;
                    gonderilenToplam = 0;
                    avansToplam = 0;
                    egButce = gButce;
                    toplamSatir = satir;
                    if (gButce == tButce)
                    {
                        gonderilenToplam = (o.onaysizOdenekGonderme + o.onayliOdenekGonderme) + (o.onaysizTenkis + o.onayliTenkis);
                    }
                    else
                    {
                        gonderilenToplam = 0;
                        XLS.HucreKopyala(satirT, sutunT, satirT, sutunT + 20, satir, sutun);

                        trp = gButce;
                        if (ekBilgi == "PRGBUTCETERTIP")
                        {
                            trp = TNS.BHMv2.Arac.ProgramTertipOl(o.tertipButce, false);
                        }
                        else if (kr.harcamaPebli)
                            DetayBilgiYaz(XLS, satir, o);

                        XLS.HucreDegerYaz(satir, 2, trp);
                        satir++;
                    }
                }
                if (gButce == tButce)
                    XLS.HucreKopyala(satirT, sutunT, satirT, sutunT + 20, satir, sutun);
                else
                    XLS.HucreKopyala(satirD + vurguluSatir, sutunD, satirD + vurguluSatir, sutunD + 20, satir, sutun);

                trp = tButce;
                if (ekBilgi == "PRGBUTCETERTIP")
                {
                    trp = TNS.BHMv2.Arac.ProgramTertipOl(o.tertipButce, false);
                }
                else if (kr.harcamaPebli)
                    DetayBilgiYaz(XLS, satir, o);

                XLS.HucreDegerYaz(satir, 2, trp);
                if (gButce == tButce || !ciftSatir)
                {
                    XLS.HucreDegerYaz(satir, 3, o.onaysizOdenekGonderme);
                    XLS.HucreDegerYaz(satir, 4, o.onaysizTenkis);
                    XLS.HucreDegerYaz(satir, 5, o.onayliOdenekGonderme);
                    XLS.HucreDegerYaz(satir, 6, o.onayliTenkis);
                    XLS.HucreDegerYaz(satir, 7, (o.onaysizOdenekGonderme + o.onayliOdenekGonderme) + (o.onaysizTenkis + o.onayliTenkis));
                    XLS.HucreDegerYaz(satir, 11, (o.onaysizOdenekGonderme + o.onayliOdenekGonderme) + (o.onaysizTenkis + o.onayliTenkis) - (o.onayliMIF + o.onayliOdemeEmri + o.onaysizMIF + o.onaysizOdemeEmri));
                }
                XLS.HucreDegerYaz(satir, 8, o.toplamOdenek);
                XLS.HucreDegerYaz(satir, 9, o.harcama);
                XLS.HucreDegerYaz(satir, 10, o.onayliMIF + o.onayliOdemeEmri + o.onaysizMIF + o.onaysizOdemeEmri);

                if (!farkRapor)
                    XLS.HucreDegerYaz(satir, 11, o.avans);
                else
                {
                    XLS.HucreDegerYaz(satir, 12, fark);
                }

                birOdeOnaysizToplam += o.onaysizOdenekGonderme;
                birTenOnaysizToplam += o.onaysizTenkis;
                birOdeOnayliToplam += o.onayliOdenekGonderme;
                birTenOnayliToplam += o.onayliTenkis;
                birHarcamaToplam += o.harcama;
                birOdemeEmriToplam += o.onayliMIF + o.onayliOdemeEmri + o.onaysizMIF + o.onaysizOdemeEmri;
                birAvansToplam += o.avans;
                birOdenekToplam += o.toplamOdenek;

                muhOdeOnaysizToplam += o.onaysizOdenekGonderme;
                muhTenOnaysizToplam += o.onaysizTenkis;
                muhOdeOnayliToplam += o.onayliOdenekGonderme;
                muhTenOnayliToplam += o.onayliTenkis;
                muhHarcamaToplam += o.harcama;
                muhOdemeEmriToplam += o.onayliMIF + o.onayliOdemeEmri + o.onaysizMIF + o.onaysizOdemeEmri;
                muhAvansToplam += o.avans;
                muhOdenekToplam += o.toplamOdenek;

                harcamaToplam += o.harcama;
                odemeEmriToplam += o.onayliMIF + o.onayliOdemeEmri + o.onaysizMIF + o.onaysizOdemeEmri;
                avansToplam += o.avans;
                odenekToplam += o.toplamOdenek;

                eMuh = muh;
                eBir = bir;
                satir++;
            }

            if (toplamSatir > 0)
            {

                XLS.HucreDegerYaz(toplamSatir, 9, harcamaToplam);
                XLS.HucreDegerYaz(toplamSatir, 10, odemeEmriToplam);

                if (!farkRapor)
                    XLS.HucreDegerYaz(toplamSatir, 11, avansToplam);
                else
                {
                    XLS.HucreDegerYaz(toplamSatir, 11, gonderilenToplam - odemeEmriToplam);
                    XLS.HucreDegerYaz(toplamSatir, 12, harcamaToplam - odemeEmriToplam);
                }
            }

            if (muhToplamSatir > 0)
            {
                XLS.HucreDegerYaz(muhToplamSatir, 3, muhOdeOnaysizToplam);
                XLS.HucreDegerYaz(muhToplamSatir, 4, muhTenOnaysizToplam);
                XLS.HucreDegerYaz(muhToplamSatir, 5, muhOdeOnayliToplam);
                XLS.HucreDegerYaz(muhToplamSatir, 6, muhTenOnayliToplam);
                XLS.HucreDegerYaz(muhToplamSatir, 7, (muhOdeOnaysizToplam + muhOdeOnayliToplam) + (muhTenOnayliToplam + muhTenOnaysizToplam));
                XLS.HucreDegerYaz(muhToplamSatir, 8, muhOdenekToplam);
                XLS.HucreDegerYaz(muhToplamSatir, 9, muhHarcamaToplam);
                XLS.HucreDegerYaz(muhToplamSatir, 10, muhOdemeEmriToplam);

                if (!farkRapor)
                    XLS.HucreDegerYaz(muhToplamSatir, 11, muhAvansToplam);
                else
                {
                    XLS.HucreDegerYaz(muhToplamSatir, 11, muhOdenekToplam - muhOdemeEmriToplam);
                    XLS.HucreDegerYaz(muhToplamSatir, 12, muhHarcamaToplam - muhOdemeEmriToplam);
                }
            }

            if (birToplamSatir > 0)
            {
                XLS.HucreDegerYaz(birToplamSatir, 3, birOdeOnaysizToplam);
                XLS.HucreDegerYaz(birToplamSatir, 4, birTenOnaysizToplam);
                XLS.HucreDegerYaz(birToplamSatir, 5, birOdeOnayliToplam);
                XLS.HucreDegerYaz(birToplamSatir, 6, birTenOnayliToplam);
                XLS.HucreDegerYaz(birToplamSatir, 7, (birOdeOnaysizToplam + birOdeOnayliToplam) + (birTenOnayliToplam + birTenOnaysizToplam));
                XLS.HucreDegerYaz(birToplamSatir, 8, birOdenekToplam);
                XLS.HucreDegerYaz(birToplamSatir, 9, birHarcamaToplam);
                XLS.HucreDegerYaz(birToplamSatir, 10, birOdemeEmriToplam);

                if (!farkRapor)
                    XLS.HucreDegerYaz(birToplamSatir, 11, birAvansToplam);
                else
                {
                    XLS.HucreDegerYaz(birToplamSatir, 11, birOdenekToplam - birOdemeEmriToplam);
                    XLS.HucreDegerYaz(birToplamSatir, 12, birHarcamaToplam - birOdemeEmriToplam);
                }
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private void DetayBilgiYaz(Tablo XLS, int satir, OdenekDurum o)
        {
            int kol = 15;
            XLS.HucreDegerYaz(satir, kol++, o.muhasebe);
            XLS.HucreDegerYaz(satir, kol++, o.birim);
            XLS.HucreDegerYaz(satir, kol++, o.tertipButce.kodlar[0]);
            XLS.HucreDegerYaz(satir, kol++, o.tertipButce.kodlar[1]);
            XLS.HucreDegerYaz(satir, kol++, o.tertipButce.kodlar[2]);
            XLS.HucreDegerYaz(satir, kol++, o.tertipButce.kodlar[3]);
            XLS.HucreDegerYaz(satir, kol++, o.tertipButce.kodlar[4]);
            string prgTrtp = TNS.BHMv2.Arac.ProgramTertipOl(o.tertipButce, false);
            XLS.HucreDegerYaz(satir, kol++, prgTrtp);
            string[] prgTrtpParca = prgTrtp.Split('.', '-');
            if (prgTrtpParca.Length >= 4)
            {
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToInt(prgTrtpParca[0], 0));
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToInt(prgTrtpParca[1], 0));
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToInt(prgTrtpParca[2], 0));
                XLS.HucreDegerYaz(satir, kol++, OrtakFonksiyonlar.ConvertToInt(prgTrtpParca[3], 0));
            }
        }
    }

    /// <summary>
    /// Odenek Ustu Harcama işlemlerini düzenleyen sınıf
    /// </summary>
    public class OdenekUstuHarcama
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Odenek Ustu Harcama Muhasebe raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int satirD = 0;
            int sutunD = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());

            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            OdenekDurumKriter kr = new OdenekDurumKriter();
            kr.yil = yil;
            kr.ay = ay;
            kr.muhasebe = muhasebe;
            kr.birim = birim;
            kr.ekoKodKirilim = 2;
            kr.eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
            kr.yeniMetot = true;
            kr.faaliyetGoster = true;
            kr.harcamaPebli = true;
            kr.odenekGoster = false;
            kr.harcamaGoster = true;
            kr.odenekGondermeGoster = true;
            kr.avansGoster = false;
            kr.odemeEmriGoster = false;
            kr.harcamaBlokeGoster = false;
            kr.detay = ENUMOdenekDurumDetay.BIRIM;
            kr.tertip.kodlar[0] = kur;
            kr.tertip.kodlar[1] = fon;
            kr.tertip.kodlar[2] = fin;
            kr.tertip.kodlar[3] = eko;
            kr.tertip.kodlar[4] = peb;

            ObjectArray oVAN = servisMUH.HarcamaDurumListesi(kullanan, kr);
            foreach (TNS.MUH.OdenekDurum o in oVAN.objeler)
            {
                if (o.harcama > o.onayliOdenekGonderme)
                {
                    XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir, sutun);

                    XLS.HucreDegerYaz(satir, 0, o.muhasebe);
                    XLS.HucreDegerYaz(satir, 1, o.birim);
                    XLS.HucreDegerYaz(satir, 2, o.tertipButce.TertipStrOl());
                    XLS.HucreDegerYaz(satir, 3, o.onayliOdenekGonderme);
                    XLS.HucreDegerYaz(satir, 4, o.harcama);
                    XLS.HucreDegerYaz(satir, 5, o.harcama - o.onayliOdenekGonderme);

                    string tStr = TNS.BHMv2.Arac.ProgramTertipOl(o.tertipButce, false);
                    string[] tStrP = tStr.Split('.', '-');

                    for (int i = 0; i < tStrP.Length; i++)
                    {
                        XLS.HucreDegerYaz(satir, 7 + i, tStrP[i]);
                    }
                    satir++;
                }
            }
            XLS.SatirSil(6, 7);
            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Muhasebat Butce Gideri Fon Eko Liste işlemlerini düzenleyen sınıf
    /// </summary>
    public class MuhasebatButceGideriFonEkoListe
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Muhasebat Butce Gideri Fon Eko Liste raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int satirD = 0;
            int sutunD = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            if (yil == 0)
                yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Yil"], 0);

            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);

            TNS.TNSDateTime tarih1 = new TNS.TNSDateTime((string)raporParam["BasTarih"]);
            TNS.TNSDateTime tarih2 = new TNS.TNSDateTime((string)raporParam["BitTarih"]);

            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            if (muhasebe == 0)
                muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Saymanlik"], 0);

            string birim = (string)raporParam["Birim"] + "";

            string kur = (string)raporParam["Kur"] + "";
            if (kur == "") kur = (string)raporParam["KurKod"] + "";
            if (kur == "") kur = TNS.MUH.Arac.VarsayilanKurumKod();

            string fon = (string)raporParam["Fon"] + "";
            if (fon == "") fon = (string)raporParam["FonKod"] + "";

            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            int tur = TNS.OrtakFonksiyonlar.ConvertToInt((string)raporParam["Tur"], 0);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());
            XLS.HucreAdBulYaz("txtAy", ay.ToString());

            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            OdenekDurumKriter kr = new OdenekDurumKriter();
            kr.yil = yil;
            kr.ay = ay;
            kr.sorguTarih1 = tarih1;
            kr.sorguTarih2 = tarih2;
            kr.muhasebe = muhasebe;
            kr.birim = birim;

            TNS.MUH.Arac.EkoUzunlukOl(kr, (string)raporParam["EkoKirilim"]);

            kr.harcamaGoster = true;
            kr.odenekGondermeGoster = true;
            kr.durum = tur;
            kr.detay = ENUMOdenekDurumDetay.TERTIP;
            kr.tertip.kodlar[0] = kur;
            kr.tertip.kodlar[1] = fon;
            kr.tertip.kodlar[2] = fin;
            kr.tertip.kodlar[3] = eko;

            ObjectArray oVAN = servisMUH.MuhasebatButceGideriFonEkoListe(kr);

            foreach (KBSAktarimDeger o in oVAN.objeler)
            {
                if (o.borc == 0 && o.alacak == 0) continue;

                XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir, sutun);

                XLS.HucreDegerYaz(satir, 0, o.hesapKod);

                //Bütün kolonlara 0 yazılmalı
                for (int i = 1; i < 10; i++)
                {
                    XLS.HucreDegerYaz(satir, i, 0);
                }

                if (o.hesap.Length >= 5) XLS.HucreDegerYaz(satir, 1, OrtakFonksiyonlar.ConvertToInt(o.hesap.Substring(3, 2), 0));
                if (o.hesap.Length >= 7) XLS.HucreDegerYaz(satir, 2, OrtakFonksiyonlar.ConvertToInt(o.hesap.Substring(5, 2), 0));
                if (o.hesap.Length >= 9) XLS.HucreDegerYaz(satir, 3, OrtakFonksiyonlar.ConvertToInt(o.hesap.Substring(7, 2), 0));
                if (o.hesap.Length >= 11) XLS.HucreDegerYaz(satir, 4, OrtakFonksiyonlar.ConvertToInt(o.hesap.Substring(9, 2), 0));

                if (o.fon.Length >= 2) XLS.HucreDegerYaz(satir, 5, OrtakFonksiyonlar.ConvertToInt(o.fon.Substring(0, 2), 0));
                if (o.fon.Length >= 3) XLS.HucreDegerYaz(satir, 6, OrtakFonksiyonlar.ConvertToInt(o.fon.Substring(2, 1), 0));
                if (o.fon.Length >= 4) XLS.HucreDegerYaz(satir, 7, OrtakFonksiyonlar.ConvertToInt(o.fon.Substring(3, 1), 0));

                XLS.HucreDegerYaz(satir, 8, OrtakFonksiyonlar.ConvertToInt(o.fin, 0));

                XLS.HucreDegerYaz(satir, 9, o.borc);
                XLS.HucreDegerYaz(satir, 10, o.alacak);
                satir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Gelir Cetveli Muhasebe işlemlerini düzenleyen sınıf
    /// </summary>
    public class GelirCetveliMuhasebe
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Gelir Cetveli Muhasebe raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int satirD = 0;
            int sutunD = 0;
            int satirT = 0;
            int sutunT = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"];
            string kur = (string)raporParam["Kur"];
            string eko = (string)raporParam["Eko"];
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());

            XLS.HucreAdAdresCoz("txtToplamSatir", ref satirT, ref sutunT);
            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            OdenekDurumKriter kr = new OdenekDurumKriter();
            kr.yil = yil;
            kr.ay = ay;
            kr.muhasebe = muhasebe;
            kr.birim = birim;
            kr.tertip.kodlar[0] = kurum;
            kr.tertip.kodlar[1] = eko;

            ObjectArray oVAN = servisMUH.GelirDurumListesi(kr);
            int baslaSatir = satir;

            double tPlanlanan = 0;
            double tGerceklesen = 0;

            foreach (TNS.MUH.GelirDurumu o in oVAN.objeler)
            {
                XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir, sutun);

                XLS.HucreDegerYaz(satir, 0, o.ekoKod);
                XLS.HucreDegerYaz(satir, 1, o.planlanan);
                XLS.HucreDegerYaz(satir, 2, o.gerceklesen);
                XLS.HucreDegerYaz(satir, 3, o.gerceklesen - o.planlanan);

                tPlanlanan += o.planlanan;
                tGerceklesen += o.gerceklesen;

                satir++;
            }

            XLS.HucreKopyala(satirT, sutunT, satirT, sutunT + 20, satir, sutun);
            XLS.HucreDegerYaz(satir, 1, tPlanlanan);
            XLS.HucreDegerYaz(satir, 2, tGerceklesen);
            XLS.HucreDegerYaz(satir, 3, tGerceklesen - tPlanlanan);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Masraf Cetveli Mal Hizmet işlemlerini düzenleyen sınıf
    /// </summary>
    public class MasrafCetveliMalHizmet
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Masraf Cetveli Mal Hizmet raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            int sutun = 0;
            int satirD = 0;
            int sutunD = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";
            int kirilim = 4;// OrtakFonksiyonlar.ConvertToInt((string)raporParam["EkoKirilim"], 0);
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtKurum", kurum + " - " + kurumAd);
            XLS.HucreAdBulYaz("txtYil", yil.ToString());

            XLS.HucreAdAdresCoz("txtDetaySatir", ref satirD, ref sutunD);
            XLS.HucreAdAdresCoz("txtBaslaSatir", ref satir, ref sutun);

            OdenekDurumKriter kr = new OdenekDurumKriter();
            kr.yil = yil;
            kr.ay = ay;
            kr.muhasebe = muhasebe;
            kr.birim = birim;
            kr.ekoKodKirilim = kirilim;
            kr.harcamaGoster = true;
            kr.odenekGondermeGoster = true;
            kr.odemeEmriGoster = true;
            kr.detay = ENUMOdenekDurumDetay.TERTIP;
            kr.tertip.kodlar[0] = kur;
            kr.tertip.kodlar[1] = fon;
            kr.tertip.kodlar[2] = fin;
            kr.tertip.kodlar[3] = eko;//"03.1;03.2;03.4;03.5;03.6;03.7;03.8;05.3;06.1;06.3;06.5;06.7";
            kr.tertip.kodlar[4] = peb;

            ObjectArray oVAN = servisMUH.OdenekDurumListesi(kr);
            int baslaSatir = satir;

            string eKur = "";
            string eFon = "";
            string eFin = "";
            string eEko = "";

            int ekoSatir = 0;
            double ekoOdeme = 0;
            double ekoHarcanan = 0;
            double ekoOdemeEmri = 0;
            double ekoKbo = 0;
            double ekoEklenen = 0;
            double ekoDusulen = 0;
            double ekoToplamOdenek = 0;
            string aciklama = "";

            double tKBO = 0;
            double tEklenen = 0;
            double tDusulen = 0;
            double tOdenek = 0;
            double tOdemeEmri = 0;
            double tHarcanan = 0;

            foreach (TNS.MUH.OdenekDurum o in oVAN.objeler)
            {
                kur = o.tertipButce.kodlar[0];
                fon = o.tertipButce.kodlar[1];
                fin = o.tertipButce.kodlar[2];
                eko = o.tertipButce.kodlar[3];

                XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir, sutun);

                if (kur != eKur)
                {
                    XLS.HucreDegerYaz(satir, 0, kur);
                    eFon = "";
                }
                if (fon != eFon)
                {
                    XLS.HucreDegerYaz(satir, 1, fon);
                    eFin = "";
                }
                if (fin != eFin)
                {
                    XLS.HucreDegerYaz(satir, 2, fin);
                    eEko = "";
                }

                if (eko != eEko)
                {
                    XLS.HucreDegerYaz(satir, 3, eko);
                    aciklama = servisUZY.UzayDegeriStr(kullanan, "ABSEKO", yil + "-" + eko, true);
                    XLS.HucreDegerYaz(satir, 4, aciklama);

                    if (eko.Length < 6)
                    {

                        for (int bb = 0; bb < 13; bb++)
                            XLS.KoyuYap(satir, bb, true);

                        if (ekoSatir > 0)
                        {
                            XLS.HucreDegerYaz(ekoSatir, 9, ekoOdemeEmri);//ekoOdeme
                            XLS.HucreDegerYaz(ekoSatir, 10, ekoHarcanan);

                            if (o.kbo == 0)
                            {
                                XLS.HucreDegerYaz(ekoSatir, 5, ekoKbo);
                                XLS.HucreDegerYaz(ekoSatir, 6, ekoEklenen);
                                XLS.HucreDegerYaz(ekoSatir, 7, ekoDusulen);
                                XLS.HucreDegerYaz(ekoSatir, 8, ekoToplamOdenek);
                            }
                            else
                            {
                                XLS.HucreDegerYaz(satir, 5, o.kbo);
                                XLS.HucreDegerYaz(satir, 6, o.eklenen);
                                XLS.HucreDegerYaz(satir, 7, o.dusulen);
                                XLS.HucreDegerYaz(satir, 8, o.toplamOdenek);
                            }

                            ekoOdemeEmri = 0;
                            ekoHarcanan = 0;
                            ekoKbo = 0;
                            ekoEklenen = 0;
                            ekoDusulen = 0;
                            ekoToplamOdenek = 0;
                        }
                        else
                        {
                            //İlk girişte yazması için
                            XLS.HucreDegerYaz(satir, 5, o.kbo);
                            XLS.HucreDegerYaz(satir, 6, o.eklenen);
                            XLS.HucreDegerYaz(satir, 7, o.dusulen);
                            XLS.HucreDegerYaz(satir, 8, o.toplamOdenek);
                        }

                        ekoSatir = satir;
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, 9, o.onayliOdemeEmri + o.onaysizOdemeEmri);
                        XLS.HucreDegerYaz(satir, 10, o.harcama);
                    }

                    tKBO += o.kbo;
                    tEklenen += o.eklenen;
                    tDusulen += o.dusulen;
                    tOdenek += o.toplamOdenek;
                    tHarcanan += o.harcama;
                    tOdemeEmri += o.onayliOdemeEmri + o.onaysizOdemeEmri;

                    ekoKbo += o.kbo;
                    ekoEklenen += o.eklenen;
                    ekoDusulen += o.dusulen;
                    ekoToplamOdenek += o.toplamOdenek;
                    ekoHarcanan += o.harcama;
                    ekoOdemeEmri += o.onayliOdemeEmri + o.onaysizOdemeEmri;


                    satir++;
                }

                //if (o.tertipOdemeEmri.kodlar.Length > 1)
                //{
                //    XLS.HucreKopyala(satirD, sutunD, satirD, sutunD + 20, satir, sutun);
                //    ekoOdeme += o.onayliMIF + o.onayliOdemeEmri + o.onaysizOdemeEmri + o.onaysizMIF;

                //    string hEko = o.tertipOdemeEmri.kodlar[3];
                //    XLS.HucreDegerYaz(satir, 3, hEko);
                //    aciklama = servisUZY.UzayDegeriStr(kullanan, "ABSEKO", yil + "-" + hEko, true);
                //    XLS.HucreDegerYaz(satir, 4, aciklama);

                //    XLS.HucreDegerYaz(satir, 9, o.onayliMIF + o.onayliOdemeEmri + o.onaysizOdemeEmri + o.onaysizMIF);

                //    tOdemeEmri += o.onayliMIF + o.onayliOdemeEmri + o.onaysizOdemeEmri + o.onaysizMIF;

                //    satir++;
                //}

                eKur = kur;
                eFon = fon;
                eFin = fin;
                eEko = eko;
            }

            if (ekoKbo + ekoEklenen + ekoDusulen > 0)
            {
                XLS.HucreDegerYaz(ekoSatir, 5, ekoKbo);
                XLS.HucreDegerYaz(ekoSatir, 6, ekoEklenen);
                XLS.HucreDegerYaz(ekoSatir, 7, ekoDusulen);
                XLS.HucreDegerYaz(ekoSatir, 8, ekoToplamOdenek);
            }

            XLS.HucreDegerYaz(ekoSatir, 9, ekoOdemeEmri);
            XLS.HucreDegerYaz(ekoSatir, 10, ekoHarcanan);

            XLS.HucreKopyala(satirD + 1, sutunD, satirD + 1, sutunD + 20, satir, sutun);
            XLS.HucreDegerYaz(satir, 4, "TOPLAM");
            XLS.HucreDegerYaz(satir, 5, tKBO);
            XLS.HucreDegerYaz(satir, 6, tEklenen);
            XLS.HucreDegerYaz(satir, 7, tDusulen);
            XLS.HucreDegerYaz(satir, 8, tOdenek);
            XLS.HucreDegerYaz(satir, 9, tOdemeEmri);
            XLS.HucreDegerYaz(satir, 10, tHarcanan);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        void AltBolumYaz(Tablo XLS, int satir, TNS.MUH.OdenekDurum o)
        {
            XLS.HucreDegerYaz(satir, 5, o.toplamOdenek.ToString("#,###.00"));
            XLS.HucreDegerYaz(satir, 10, o.onayliOdemeEmri.ToString("#,###.00"));
            XLS.HucreDegerYaz(satir, 6, (o.toplamOdenek * 10 / 100).ToString("#,###.00"));
            XLS.HucreDegerYaz(satir, 11, ((o.toplamOdenek * 10 / 100) - o.onayliOdemeEmri).ToString("#,###.00"));
        }
    }

    /// <summary>
    /// Masraf Cetveli Yuzde On işlemlerini düzenleyen sınıf
    /// </summary>
    public class MasrafCetveliYuzdeOn
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Masraf Cetveli Yuzde On 2Duzey raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl_R(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int baslaSatir = 0;
            int satir = 0;
            int sutun = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;
            string[] kodlar = TNS.MUH.Arac.YuzdeOnAlimTurBul();
            int kodLen = kodlar.Length;
            int kurumSatir = 0, kurumSutun = 0;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            ObjectArray oa;
            if (ekBilgi == "R")
                oa = servisMUH.YuzdeOnListele(kullanan, yil);
            else
                oa = TNS.HRC.Arac.Tanimla().YuzdeOnListele(kullanan, yil);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);
            XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);

            XLS.HucreAdAdresCoz("KurumAd", ref kurumSatir, ref kurumSutun);
            XLS.HucreDegerYaz(kurumSatir, kurumSutun, GenelIslemlerIstemci.VarsayilanKurumBulAd());
            XLS.HucreBirlestir(kurumSatir, kurumSutun, kurumSatir, kurumSutun + 4 + kodLen);

            XLS.HucreAdAdresCoz("Yil", ref kurumSatir, ref kurumSutun);
            XLS.HucreDegerYaz(kurumSatir, kurumSutun, yil);
            XLS.HucreBirlestir(kurumSatir, kurumSutun, kurumSatir, kurumSutun + 4 + kodLen);
            satir = baslaSatir;
            if (oa.sonuc.islemSonuc)
            {
                if (ekBilgi != "R")
                {
                    XLS.HucreDegerYaz(baslaSatir - 6, sutun + 1, "Onaylanan");
                }

                for (int i = 0; i < kodLen + 1; i++)
                {
                    XLS.HucreKopyala(baslaSatir - 3, sutun + 2, baslaSatir - 2, sutun + 2, baslaSatir - 3, sutun + 3 + i);
                }

                for (int i = 0; i < kodLen; i++)
                {
                    XLS.HucreDegerYaz(baslaSatir - 3, sutun + 2 + i, kodlar[i]);
                }
                XLS.HucreDegerYaz(baslaSatir - 3, sutun + 2 + kodLen, "Toplam");
                XLS.HucreDegerYaz(baslaSatir - 3, sutun + 3 + kodLen, "Diğer");

                foreach (YuzdeOnBilgi bilgi in oa.objeler)
                {
                    int bolumSatir = satir;
                    XLS.HucreKopyala(baslaSatir - 8, sutun, baslaSatir - 3, sutun + kodLen + 4, satir, sutun);
                    XLS.HucreDegerYaz(bolumSatir, sutun + 2, bilgi.TurAd);
                    XLS.HucreDegerYaz(bolumSatir + 1, sutun + 2, bilgi.ustSinir);

                    satir += 6;
                    double bolumToplam = 0;
                    for (int i = 0; i < bilgi.liste.Length; i++)
                    {
                        XLS.HucreKopyala(baslaSatir - 2, sutun, baslaSatir - 2, sutun + kodLen + 4, satir, sutun);
                        double satToplam = 0;
                        string kod = "", ad = "";
                        OrtakFonksiyonlar.StringiIkiyeBol(bilgi.liste[i].tertip, "-", ref kod, ref ad);
                        XLS.HucreDegerYaz(satir, sutun + 0, kod);
                        XLS.HucreDegerYaz(satir, sutun + 1, ad);
                        for (int j = 0; j < bilgi.liste[i].odenekler.Length; j++)
                        {
                            int atla = 0;
                            if (j < kodLen)
                            {
                                atla = 0;
                                satToplam += bilgi.liste[i].odenekler[j];
                            }
                            else
                                atla = 1;
                            XLS.HucreDegerYaz(satir, sutun + 2 + j + atla, bilgi.liste[i].odenekler[j]);
                        }
                        XLS.HucreDegerYaz(satir, sutun + 2 + kodLen, satToplam);
                        bolumToplam += satToplam;
                        satir++;
                    }
                    XLS.HucreDegerYaz(bolumSatir + 2, sutun + 2, bolumToplam);
                    XLS.HucreDegerYaz(bolumSatir + 3, sutun + 2, bilgi.ustSinir - bolumToplam);

                    satir++;
                }
            }
            else
                XLS.HucreDegerYaz(satir, sutun, oa.sonuc.hataStr);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        /// <summary>
        /// Masraf Cetveli Yuzde On raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            if (ekBilgi == "R" || ekBilgi == "S")
                return RaporAl_R(o_kullanan, o_servis, sablonAd, sonucDosyaAd, ciktiYeri, raporParam, ekBilgi);

            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);


            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtYil", yil);
            XLS.HucreAdBulYaz("txtKur", kurum + " - " + kurumAd);

            string[] kodlar = { "21f", "22a", "22dMal", "22dHizmet", "22dYapım" };
            string deger = XLS.HucreAdDegerAl("KodBasla");
            if (!string.IsNullOrWhiteSpace(deger))
                kodlar = deger.Split(',');

            for (int i = 0; i < 300; i++)
            {
                if (MasrafCetveliYuzdeOn2Duzey.EkoMu(XLS.HucreDegerAl(satir, 0), out eko, true))
                {
                    double hKBO = servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", yil + "-0600-0-" + kur + "---" + eko, false);
                    double harcama = servisUZY.UzayDegeriDbl(kullanan, "ABSHARCAMA", yil + "-" + ay + "-" + kur + "---" + eko, false);
                    double odeme = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko, false);

                    double odeme21f = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---" + kodlar[0], false);
                    double odeme22a = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---" + kodlar[1], false);
                    double odeme22dMal = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---" + kodlar[2], false);
                    double odeme22dHizmet = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---" + kodlar[3], false);
                    double odeme22dYapim = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---" + kodlar[4], false);

                    XLS.HucreDegerYaz(satir, 2, hKBO);
                    XLS.HucreDegerYaz(satir, 5, harcama);
                    XLS.HucreDegerYaz(satir, 4, odeme);
                    XLS.HucreDegerYaz(satir, 6, odeme21f);
                    XLS.HucreDegerYaz(satir, 7, odeme22a);
                    XLS.HucreDegerYaz(satir, 8, odeme22dMal);
                    XLS.HucreDegerYaz(satir, 9, odeme22dHizmet);
                    XLS.HucreDegerYaz(satir, 10, odeme22dYapim);
                }
                satir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// Masraf Cetveli Yuzde On 2Duzey işlemlerini düzenleyen sınıf
    /// </summary>
    public class MasrafCetveliYuzdeOn2Duzey
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();


        public static bool EkoMu(string gelenEko, out string eko, bool sifirEkle)
        {
            eko = "";
            if (string.IsNullOrEmpty(gelenEko)) return false;

            string[] ekoParcalar = gelenEko.Split('.');
            if (ekoParcalar.Length == 2 || ekoParcalar.Length == 4)
            {
                for (int i = 0; i < ekoParcalar.Length; i++)
                {
                    if (i != 0) eko += ".";
                    string parca = ekoParcalar[i];
                    if (sifirEkle)
                    {
                        if (i == 1) parca = "0" + parca;
                        else if (i == 2) parca = parca + "0";
                    }
                    eko += parca;
                }
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Masraf Cetveli Yuzde On 2Duzey raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);


            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);


            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("txtYil", yil);
            XLS.HucreAdBulYaz("txtKur", kurum + " - " + kurumAd);


            for (int i = 0; i < 300; i++)
            {
                if (EkoMu(XLS.HucreDegerAl(satir, 0), out eko, true))
                {
                    double hKBO = servisUZY.UzayDegeriDbl(kullanan, "ABSBUTCESERBEST", yil + "-" + kur + "---" + eko, false);
                    double harcama = servisUZY.UzayDegeriDbl(kullanan, "ABSHARCAMA", yil + "-" + ay + "-" + kur + "---" + eko, false);
                    double odeme = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko, false);
                    double odeme21f = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---21f", false);
                    double odeme22a = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---22a", false);
                    double odeme22dMal = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---22dMal", false);
                    double odeme22dHizmet = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---22dHizmet", false);
                    double odeme22dYapim = servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", yil + "---830-" + kur + "---" + eko + "---22dYapım", false);

                    XLS.HucreDegerYaz(satir, 2, hKBO);
                    XLS.HucreDegerYaz(satir, 5, harcama);
                    XLS.HucreDegerYaz(satir, 4, odeme);
                    XLS.HucreDegerYaz(satir, 6, odeme21f);
                    XLS.HucreDegerYaz(satir, 7, odeme22a);
                    XLS.HucreDegerYaz(satir, 8, odeme22dMal);
                    XLS.HucreDegerYaz(satir, 9, odeme22dHizmet);
                    XLS.HucreDegerYaz(satir, 10, odeme22dYapim);
                }
                satir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ISLHarcamaAylik
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        /// <summary>
        /// Rapors the al.
        /// </summary>
        /// <param name="o_kullanan">The o_kullanan.</param>
        /// <param name="o_servis">The o_servis.</param>
        /// <param name="sablonAd">The sablon ad.</param>
        /// <param name="sonucDosyaAd">The sonuc dosya ad.</param>
        /// <param name="ciktiYeri">The cikti yeri.</param>
        /// <param name="raporParam">The rapor param.</param>
        /// <param name="ekBilgi">The ek bilgi.</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            int aktifSatir = 0;
            int basSatir = 0;
            int basSutun = 0;
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt((string)raporParam["Muhasebe"], 0);
            string birim = (string)raporParam["Birim"];
            string kur = (string)raporParam["Kur"] + "";
            string fon = (string)raporParam["Fon"] + "";
            string fin = (string)raporParam["Fin"] + "";
            string eko = (string)raporParam["Eko"] + "";
            string peb = (string)raporParam["Peb"] + "";
            Tertip tUz = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            string kurum = OrtakFonksiyonlar.KurumsaldanKuruma(kur, tUz.Kirilimlar(0));
            string kurumAd = servisUZY.UzayDegeriStr(kullanan, "ABSKUR", yil + "-" + kurum, true);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            XLS.HucreAdBulYaz("Baslik", yil + " YILI AYLIK HARCAMA TUTARLARI");
            XLS.HucreAdAdresCoz("BaslaSatir", ref basSatir, ref basSutun);
            aktifSatir = basSatir + 1;

            ISLHarcama hrc = new ISLHarcama();
            hrc.yil = yil;
            hrc.muhasebeKod = muhasebe;
            hrc.birimKod = birim;
            hrc.kurKod = kur;
            hrc.fonKod = fon;
            hrc.finKod = fin;
            hrc.ekoKod = eko;
            int ayid = 0;
            Ay oncekiAy = new Ay();
            ObjectArray harcamalar = servisMUH.ISLHarcamaListele(kullanan, hrc);
            string hata = null;
            if (harcamalar.sonuc.islemSonuc)
            {

                ISLHarcama[] harcamaListesi = HarcamaDuzenle(harcamalar, yil);

                foreach (ISLHarcama item in harcamaListesi)
                {
                    XLS.HucreKopyala(basSatir, 0, basSatir, 25, aktifSatir, 0);
                    XLS.HucreDegerYaz(aktifSatir, 0, item.muhasebeKod);//GenelIslemler.KodAd(11, item.muhasebeKod.ToString(), true));
                    XLS.HucreDegerYaz(aktifSatir, 1, item.birimKod);//GenelIslemler.KodAd(12, item.birimKod + "-" + item.muhasebeKod.ToString(), false));
                    XLS.HucreDegerYaz(aktifSatir, 2, item.kurKod);
                    XLS.HucreDegerYaz(aktifSatir, 3, item.fonKod);
                    XLS.HucreDegerYaz(aktifSatir, 4, item.finKod);
                    XLS.HucreDegerYaz(aktifSatir, 5, item.ekoKod);
                    ayid = 0;
                    oncekiAy.miktar = 0;
                    foreach (Ay ayy in item.aylar.objeler)
                    {
                        ayid++;
                        XLS.HucreDegerYaz(aktifSatir, 5 + ayid, ayy.miktar - oncekiAy.miktar);
                        if (ayid == 12)
                            XLS.HucreDegerYaz(aktifSatir, 5 + ayid + 1, ayy.miktar);

                        oncekiAy = ayy;
                    }
                    aktifSatir++;
                }
            }
            else
            {
                // eski: GenelIslemler.MesajKutusu("HATA", harcamalar.sonuc.hataStr);
                hata = harcamalar.sonuc.hataStr;
            }
            XLS.DosyaSaklaTamYol();
            if (hata != null)
                return "HATA: " + hata;
            return sonucDosyaAd;
        }

        private ISLHarcama[] HarcamaDuzenle(ObjectArray harcamalar, int yil)
        {
            Ay maxAy = null;
            //**yıl şimdiki yıl ise
            if (yil == DateTime.Now.Year)
            {
                foreach (ISLHarcama item in harcamalar.objeler)
                {
                    maxAy = GetMaxAy(item);
                    //**max ay aralık değil ise
                    //**kendisinden sonraki ayların tutarunı max ay tutarı olarak ata
                    if (maxAy.id < 12)
                    {
                        MaxAyTutariniSonrakiAylaraAta(item, maxAy);
                    }
                }
            }

            OncekiAyTutariSonrakiAyTutarindanBuyukOlamazKriteriUygula(harcamalar);

            return (ISLHarcama[])harcamalar.objeler.ToArray(typeof(ISLHarcama));
        }

        private void OncekiAyTutariSonrakiAyTutarindanBuyukOlamazKriteriUygula(ObjectArray harcamalar)
        {
            Ay kucukAy = null;
            Ay buyukAy = null;
            foreach (ISLHarcama item in harcamalar.objeler)
            {
                for (int i = 11; i > 0; i--)
                {
                    buyukAy = (Ay)item.aylar.objeler[i];
                    kucukAy = (Ay)item.aylar.objeler[i - 1];
                    if (kucukAy.miktar > buyukAy.miktar)
                        kucukAy.miktar = buyukAy.miktar;
                }
            }
        }

        private void MaxAyTutariniSonrakiAylaraAta(ISLHarcama item, Ay maxAy)
        {
            for (int i = maxAy.id; i <= 11; i++)
            {
                ((Ay)item.aylar.objeler[i]).miktar = maxAy.miktar;
            }
        }

        private Ay GetMaxAy(ISLHarcama item)
        {
            Ay maxAy = new Ay();
            for (int i = 11; i >= 0; i--)
            {
                maxAy = (Ay)item.aylar.objeler[i];
                if (maxAy.miktar > 0)
                    break;
            }
            return maxAy;
        }

    }

    public class NakitPlani
    {
        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        /// <summary>
        /// Masraf Cetveli Yuzde On raporu üreten fonksiyon
        /// </summary>
        /// <param name="o_kullanan">The kullanan.</param>
        /// <param name="o_servis">Rapor bilgilerinin çağıralacağı servis bilgisi</param>
        /// <param name="sablonAd">Şablon adı bilgisi</param>
        /// <param name="sonucDosyaAd">İşlem sonucu oluşacak dosya adı</param>
        /// <param name="ciktiYeri">Çıktı yeri bilgisi</param>
        /// <param name="raporParam">Rapor parametreleri</param>
        /// <param name="ekBilgi">Rapor için gerekli ek bilgi</param>
        /// <returns></returns>
        public string RaporAl(object o_kullanan, object o_servis, string sablonAd, string sonucDosyaAd, string ciktiYeri, System.Collections.Hashtable raporParam, string ekBilgi)
        {
            Tablo XLS = GenelIslemler.NewTablo();
            Kullanici kullanan = (Kullanici)o_kullanan;

            int yil = OrtakFonksiyonlar.ConvertToInt((string)raporParam["ButceYil"], 0);
            int ay = OrtakFonksiyonlar.ConvertToInt((string)raporParam["AyNo"], 0);

            sonucDosyaAd = sonucDosyaAd + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablonAd, sonucDosyaAd);

            int tur = OrtakFonksiyonlar.ConvertToInt((string)raporParam["odenekTur"], 1);
            double carpan;
            if (tur == 2)
                carpan = OrtakFonksiyonlar.ConvertToDbl((string)raporParam["katsayi"]);
            else
                carpan = 1;

            XLS.HucreAdBulYaz("butceYil", yil);
            XLS.HucreAdBulYaz("ayAd", GenelIslemler.AyAdDondur(ay));
            XLS.HucreAdBulYaz("odenekTur", tur == 1 ? "Masraf Pl." : "Harcama (Def.)");
            XLS.HucreAdBulYaz("odenekDonem", $"{(yil - tur + 1)} {GenelIslemler.AyAdDondur(ay)}" + (tur == 1 ? "" : $" ({carpan.ToString("0.00")})"));
            int satir = 0;
            int sutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);

            while (true)
            {
                string kurKod = XLS.HucreDegerAl(satir, sutun);
                string ekoKod = XLS.HucreDegerAl(satir, sutun + 1);
                if (!string.IsNullOrEmpty(kurKod))
                {
                    double miktar;
                    if (tur == 1)
                    {
                        miktar = servisUZY.UzayDegeriDbl(kullanan, "MPGMASRAFPLANITERTIPKSLZ", yil + "--" + kurKod + "---" + ekoKod + "---" + ay + "-5", false);
                    }
                    else
                    {
                        miktar = servisUZY.UzayDegeriDbl(kullanan, "ABSHARCAMA", (yil - 1) + "-" + ay + "-" + kurKod + "---" + ekoKod, false);
                        if (ay > 1)
                            miktar -= servisUZY.UzayDegeriDbl(kullanan, "ABSHARCAMA", (yil - 1) + "-" + (ay - 1) + "-" + kurKod + "---" + ekoKod, false);
                        miktar = miktar * carpan;
                    }

                    XLS.HucreDegerYaz(satir, sutun + 5, miktar);
                }
                else if (string.IsNullOrEmpty(ekoKod)) break;
                satir++;
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }
    }
}

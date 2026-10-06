using System;
using System.Collections.Generic;
using System.Text;
using TNS.BKD;
using TNS.UZY;
using TNS.KYM;

namespace TNS.BHMv2
{
    /// <summary>
    /// Tertip listelerinde tertiple birlikte istenen ek ödenek bilgilerinin özelliklerini tutan sınıf.
    /// </summary>
    [System.Serializable()]
    public class EkDegerParam
    {
        /// <summary>
        /// Ek değer bilgisinin bulunacağı tablo
        /// </summary>
        ENUMTabloKod tabloKod;

        /// <summary>
        /// Ek değer verileri hesaplanırken, bu ek değer için, işlem yapılan yıldan kaç çıkartılacağı
        /// </summary>
        public int butceYilFark;

        /// <summary>
        /// Ek değer verileri hesaplanırken, çok yıllı ödenek bilgilerinde hangi yılın kullanılacağı (0 işlem yılı, 1 sonraki, 2 iki sonraki)
        /// </summary>
        public int teklifYil;

        /// <summary>
        /// Ek değer verileri hesaplanırken, ödeneklerde senaryo kodu, gerçekleşmelerde ise ay bilgisi
        /// </summary>
        public string senaryoKodVeyaAy;

        /// <summary>
        /// Ek değer nesnesini verilen parametrelerle oluşturan oluşturucu yordam.
        /// </summary>
        /// <param name="tabloKod">Ek değer bilgisinin bulunacağı tablo</param>
        /// <param name="butceYilFark">Ek değer verileri hesaplanırken, bu ek değer için, işlem yapılan yıldan kaç çıkartılacağı</param>
        /// <param name="teklifYil">Ek değer verileri hesaplanırken, çok yıllı ödenek bilgilerinde hangi yılın kullanılacağı (0 işlem yılı, 1 sonraki, 2 iki sonraki)</param>
        /// <param name="senaryoKodVeyaAy">Ek değer verileri hesaplanırken, ödeneklerde senaryo kodu, gerçekleşmelerde ise ay bilgisi</param>
        public EkDegerParam(ENUMTabloKod tabloKod, int butceYilFark, int teklifYil, string senaryoKodVeyaAy)
        {
            this.tabloKod = tabloKod;
            this.butceYilFark = Math.Abs(butceYilFark);
            this.teklifYil = teklifYil;
            this.senaryoKodVeyaAy = senaryoKodVeyaAy;
        }

        /// <summary>
        /// Verilen tablo adı liste yapısı kullanarak, kullanılacak uzay adını döndüren yordam.
        /// </summary>
        /// <returns>Ek değerdeki tablo için kullanılacak uzay adı döndürülür.</returns>
        private string UzayAdBul()
        {
            if (tabloKod == ENUMTabloKod.TABPEBOdenek || tabloKod == ENUMTabloKod.TABOdenek) return "ABSODENEK";
            if (tabloKod == ENUMTabloKod.TABPEBGelir || tabloKod == ENUMTabloKod.TABGelir) return "ABSGELIR";
            if (tabloKod == ENUMTabloKod.TABFinansman) return "ABSFINANSMAN";

            if (tabloKod == ENUMTabloKod.TABHarcamaTahmin) return "ABSHARCAMATAHMIN";
            if (tabloKod == ENUMTabloKod.TABGelirTahmini) return "ABSGELIRTAHMIN";

            if (tabloKod == ENUMTabloKod.TABButce) return "ABSBUTCEGIDER";
            if (tabloKod == ENUMTabloKod.TABButceGelir) return "ABSBUTCEGELIR";

            if (tabloKod == ENUMTabloKod.TABPEBGelirGerceklesmesi || tabloKod == ENUMTabloKod.TABGelirGerceklesmesi) return "ABSGELIRGERCEKLESME";
            if (tabloKod == ENUMTabloKod.TABMPTavan) return "MPTAVAN";
            if (tabloKod == ENUMTabloKod.TABHAZHarcama || tabloKod == ENUMTabloKod.TABPEBHAZHarcama) return "ABSHAZMUHASEBAT";
            if (tabloKod == ENUMTabloKod.TABPEBHarcama || tabloKod == ENUMTabloKod.TABOdemeEmri) return "MUHODEMEEMRIMIF";
            if (tabloKod == ENUMTabloKod.TABMIFKY) return "MUHMIFKY";
            if (tabloKod == ENUMTabloKod.TABHarcama) return "MUHASEBAT";
            if (tabloKod == ENUMTabloKod.TABISLHarcama) return "ISLHARCAMAMUHASEBE";
            if (tabloKod == ENUMTabloKod.TABISLAvans) return "ISLAVANS";
            if (tabloKod == ENUMTabloKod.TABHYBloke) return "MUHBLOKEODENEK";
            if (tabloKod == ENUMTabloKod.TABOdenekGonderme) return "MUHGONDERMEBELGESITENKIS";
            if (tabloKod == ENUMTabloKod.TABOdenekGondermeKY) return "MUHODENEKGONDERMEKY";
            if (tabloKod == ENUMTabloKod.TABMasrafPlani)
            {
                if (senaryoKodVeyaAy.StartsWith("9")) return "MPGMASRAFPLANITERTIP";
                else return "MPGMASRAFPLANITERTIPKSLZ";
            }
            if (tabloKod == ENUMTabloKod.TABMasrafPlaniBirim)
            {
                if (senaryoKodVeyaAy.StartsWith("10") || senaryoKodVeyaAy.StartsWith("11")) return "MPGMASRAFPLANIBIRIMTUM";
                return "MPGMASRAFPLANIBIRIM";
            }
            if (tabloKod == ENUMTabloKod.TABMasrafPlaniForm) return "MPGMASRAFPLANIFORMTERTIP";
            if (tabloKod == ENUMTabloKod.TABPRGMasrafPlaniForm) return "MPGPRGMASRAFPLANIFORMTERTIP";
            if (tabloKod == ENUMTabloKod.TABTasraOdenek) return "BHMBIRIMBUTCEHAZIRLIK";
            if (tabloKod == ENUMTabloKod.TABPEBTavanUstu) return "BHMTAVANUSTU";
            if (tabloKod == ENUMTabloKod.TABPRGOdenek) return "PRGODENEK";

            if (tabloKod == ENUMTabloKod.TABPRGHarcamaTahmin) return "PRGHARCAMATAHMIN";
            if (tabloKod == ENUMTabloKod.TABPRGTavanUstu) return "PRGTAVANUSTU";
            if (tabloKod == ENUMTabloKod.TABPRGHarcama) return "PRGMUHASEBAT";
            if (tabloKod == ENUMTabloKod.TABPRGOdemeEmri || tabloKod == ENUMTabloKod.TABPRGOdemeEmriAvansiz) return "PRGODEMEEMRIMIF";

            if (tabloKod == ENUMTabloKod.TABGIDERAP) return "AHP";
            if (tabloKod == ENUMTabloKod.TABPRGGIDERAP) return "ISLAPGIDERPRG";
            return string.Empty;
        }

        public string ToplanacakAlan()
        {
            if (tabloKod == ENUMTabloKod.TABOdenekGonderme) return "MIKTAR";
            else if (tabloKod == ENUMTabloKod.TABOdenekGondermeKY) return "MIKTAR";
            else if (tabloKod == ENUMTabloKod.TABPEBTavanUstu || tabloKod == ENUMTabloKod.TABPRGTavanUstu)
            {
                if (teklifYil == 1) return "ODENEK1";
                else if (teklifYil == 2) return "ODENEK2";
            }
            return "ODENEK";
        }

        private string UzayAlanAdBul()
        {
            if (tabloKod == ENUMTabloKod.TABPEBTavanUstu || tabloKod == ENUMTabloKod.TABPRGTavanUstu)
            {
                if (teklifYil == 1) return "SUM(ODENEK1)";
                else if (teklifYil == 2) return "SUM(ODENEK2)";
            }
            return "";
        }

        public static string muhasebatTur = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebatTur") + "";

        /// <summary>
        /// Verilen bütçe yılı ve tertip şablonu kullanılarak, bu ek değerin hesaplanması için kullanılacak uzay koşulunu oluşturup döndüren yordam.
        /// Tertip şablonundan, tertip boyutları bulunur. Eğer tertip kurumsal yapı içeriyorsa ve uzay saymanlık kodu ve birim içeriyorsa gerekli dönüşüm işlemi yapılır.
        /// </summary>
        /// <param name="butceYil">Ek değerin hesaplanacağı ana yıl. Bütçe yıl fark değeri çıkartılarak döndürülecek yıl değeri hesaplanır.</param>
        /// <param name="tertipSablon">Ek değerin hesaplanacağı tertibe ait kodları içeren dizgi</param>
        /// <returns>Ek değer nesnesi için, verilen bütçe yılı ve tertip şablonu kullanılarak elde edilen uzay koşulu döndürülür.</returns>
        public string EkDegerKosul(int butceYil, string tertipSablon)
        {
            string uzayKosul = string.Empty;

            if (tabloKod == ENUMTabloKod.TABPEBOdenek || tabloKod == ENUMTabloKod.TABOdenek)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + SenaryoyaCevir(senaryoKodVeyaAy) + "-" + teklifYil.ToString() + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABPRGOdenek)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + SenaryoyaCevir(senaryoKodVeyaAy) + "-" + teklifYil.ToString() + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABPEBTavanUstu || tabloKod == ENUMTabloKod.TABPRGTavanUstu)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + SenaryoyaCevir(senaryoKodVeyaAy) + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABTasraOdenek)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + senaryoKodVeyaAy + "-" + BoyuttanBirimAl(tertipSablon, 6) + "-" + BoyuttanKucukleriAl(tertipSablon, 5);
            else if (tabloKod == ENUMTabloKod.TABMPTavan)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + senaryoKodVeyaAy + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABHAZHarcama || tabloKod == ENUMTabloKod.TABPEBHAZHarcama)
            {
                int ayNo = OrtakFonksiyonlar.ConvertToInt(senaryoKodVeyaAy, -1);
                if (ayNo < 1)
                {
                    ayNo = 12;
                }
                if (!string.IsNullOrEmpty(muhasebatTur))
                {
                    senaryoKodVeyaAy = "0/" + (ayNo + 1).ToString();
                }
                else
                    senaryoKodVeyaAy = ayNo.ToString();

                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + senaryoKodVeyaAy + "-" + SonBoyutBirim(tertipSablon, 6);
            }
            else if (tabloKod == ENUMTabloKod.TABMIFKY)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + BoyutaTamamla(tertipSablon, 6) + "-" + senaryoKodVeyaAy;
            else if (tabloKod == ENUMTabloKod.TABHarcama || tabloKod == ENUMTabloKod.TABISLHarcama || tabloKod == ENUMTabloKod.TABISLAvans || tabloKod == ENUMTabloKod.TABPRGHarcama)
            {
                string[] parcalar = senaryoKodVeyaAy.Split(';');

                int ayNo = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(parcalar, 0, ""), -1);
                if (ayNo < 1)
                {
                    ayNo = 12;
                }
                if (!string.IsNullOrEmpty(muhasebatTur))
                    senaryoKodVeyaAy = "0/" + (ayNo + 1).ToString();
                else
                    senaryoKodVeyaAy = ayNo.ToString();

                string hesapKod = OrtakFonksiyonlar.DiziElemanAl(parcalar, 1, "");
                if (string.IsNullOrWhiteSpace(hesapKod) && (tabloKod == ENUMTabloKod.TABHarcama || tabloKod == ENUMTabloKod.TABPRGHarcama)) hesapKod = "830,160,161,164,165,166,167";
                if (tabloKod == ENUMTabloKod.TABHarcama || tabloKod == ENUMTabloKod.TABPRGHarcama) hesapKod += "-";

                if (tabloKod == ENUMTabloKod.TABPRGHarcama)
                    uzayKosul = (butceYil - butceYilFark).ToString() + "-" + senaryoKodVeyaAy + "-" + BoyuttanBirimAl(tertipSablon, 11) + "-" + hesapKod + BoyuttanKucukleriAl(tertipSablon, 10);
                else
                    uzayKosul = (butceYil - butceYilFark).ToString() + "-" + senaryoKodVeyaAy + "-" + BoyuttanBirimAl(tertipSablon, 6) + "-" + hesapKod + BoyuttanKucukleriAl(tertipSablon, 5);
            }
            else if (tabloKod == ENUMTabloKod.TABOdemeEmri || tabloKod == ENUMTabloKod.TABPEBHarcama || tabloKod == ENUMTabloKod.TABPRGOdemeEmri || tabloKod == ENUMTabloKod.TABPRGOdemeEmriAvansiz)
            {
                string hesapKod = "";
                if (tabloKod == ENUMTabloKod.TABOdemeEmri || tabloKod == ENUMTabloKod.TABPRGOdemeEmri)
                    hesapKod = "830,160,161,164,165,166,167";
                else
                    hesapKod = "830";

                string ayKosul = "";
                string durumKosul = "";
                if (!string.IsNullOrWhiteSpace(senaryoKodVeyaAy))
                {
                    string[] durumParcalar = senaryoKodVeyaAy.Split(';');

                    int ayNo = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(durumParcalar, 0, ""), -1);
                    ayKosul = "0/" + (ayNo + 1).ToString();
                    durumKosul = OrtakFonksiyonlar.DiziElemanAl(durumParcalar, 1, "");
                }
                if (tabloKod == ENUMTabloKod.TABPRGOdemeEmri || tabloKod == ENUMTabloKod.TABPRGOdemeEmriAvansiz)
                    uzayKosul = (butceYil - butceYilFark).ToString() + "-" + ayKosul + "-" + BoyuttanBirimAl(tertipSablon, 11) + "-" + hesapKod + "-" + BoyuttanKucukleriAl(tertipSablon, 10) + durumKosul + "-" + BoyuttanVeriAl(tertipSablon, 12);
                else
                    uzayKosul = (butceYil - butceYilFark).ToString() + "-" + BoyuttanBirimAl(tertipSablon, 6) + "-" + hesapKod + "-" + BoyuttanKucukleriAl(tertipSablon, 4) + durumKosul + "-" + BoyuttanVeriAl(tertipSablon, 4) + "------" + ayKosul;
            }
            else if (tabloKod == ENUMTabloKod.TABHYBloke)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + BoyuttanBirimAl(tertipSablon, 6) + "-" + BoyuttanKucukleriAl(tertipSablon, 5);
            else if (tabloKod == ENUMTabloKod.TABHarcamaTahmin || tabloKod == ENUMTabloKod.TABPRGHarcamaTahmin)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + senaryoKodVeyaAy + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABOdenekGondermeKY)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + BoyutaTamamla(tertipSablon, 6) + "-" + senaryoKodVeyaAy;
            else if (tabloKod == ENUMTabloKod.TABOdenekGonderme)
            {
                string[] parcalar = senaryoKodVeyaAy.Split(';');
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + TNS.MUH.Arac.DiziElemanAl(parcalar, 2, "") + "---" + BoyuttanBirimAl(tertipSablon, 6) + "-" + TNS.MUH.Arac.DiziElemanAl(parcalar, 0, "") + "-" + TNS.MUH.Arac.DiziElemanAl(parcalar, 1, "") + "-" + BoyuttanKucukleriAl(tertipSablon, 5);
            }
            else if (tabloKod == ENUMTabloKod.TABButce || tabloKod == ENUMTabloKod.TABButceGelir)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABGelir || tabloKod == ENUMTabloKod.TABPEBGelir)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + SenaryoyaCevir(senaryoKodVeyaAy) + "-" + teklifYil.ToString() + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABFinansman)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + SenaryoyaCevir(senaryoKodVeyaAy) + "-" + teklifYil.ToString() + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABGelirGerceklesmesi || tabloKod == ENUMTabloKod.TABPEBGelirGerceklesmesi)
            {
                string ayKosul;
                string[] parcalar = senaryoKodVeyaAy.Split(';');
                int ayNo = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(parcalar, 0, ""), -1);

                if (ayNo < 1)
                {
                    ayNo = 12;
                }
                if (!string.IsNullOrEmpty(muhasebatTur))
                    ayKosul = "0/" + (ayNo + 1).ToString();
                else
                    ayKosul = ayNo.ToString();

                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + ayKosul + "-" + tertipSablon;
            }
            else if (tabloKod == ENUMTabloKod.TABGelirTahmini)
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + senaryoKodVeyaAy + "-" + tertipSablon;
            else if (tabloKod == ENUMTabloKod.TABMasrafPlani)
            {
                string durum = "";
                string asama = "";
                string ay = "";
                string revizeNo = "";
                if (!string.IsNullOrEmpty(senaryoKodVeyaAy))
                {
                    string[] bilgiler = senaryoKodVeyaAy.Split(';');
                    if (bilgiler.Length >= 1) asama = bilgiler[0];
                    if (asama == "9" || asama == "10")
                    {
                        asama = "5";
                        durum = "5";
                        if (asama == "10") durum = "0/6";
                    }
                    else
                        revizeNo = "0";
                    if (bilgiler.Length >= 2) ay = bilgiler[1];
                }
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + asama + "-" + BoyutaTamamla(tertipSablon, 6) + "-" + ay + "-" + durum + "-" + revizeNo;
            }
            else if (tabloKod == ENUMTabloKod.TABMasrafPlaniBirim)
            {
                string durum = "";
                string asama = "";
                string ay = "";
                string revizeNo = "";
                string belgeTur = "";
                string odenekKosul = "";
                if (!string.IsNullOrEmpty(senaryoKodVeyaAy))
                {
                    string[] bilgiler = senaryoKodVeyaAy.Split(';');
                    if (bilgiler.Length >= 1) asama = bilgiler[0];
                    if (asama == "9" || asama == "8" || asama == "10" || asama == "11")
                    {
                        asama = "5";
                        if (asama == "8" || asama == "10")
                            durum = "5";
                        else
                            durum = "1/6";
                    }
                    else
                        revizeNo = "0";
                    if (bilgiler.Length >= 2) ay = bilgiler[1];
                    if (bilgiler.Length >= 3) belgeTur = bilgiler[2];
                    if (bilgiler.Length >= 4) odenekKosul = bilgiler[3];
                }
                uzayKosul = (butceYil - butceYilFark).ToString() + "-" + asama + "-" + BoyuttanBirimAl(tertipSablon, 6) + "-" + BoyuttanKucukleriAl(tertipSablon, 5) + "-" + ay + "-" + durum + "-" + revizeNo + "-" + belgeTur + "-" + odenekKosul;
            }
            else if (tabloKod == ENUMTabloKod.TABMasrafPlaniForm || tabloKod == ENUMTabloKod.TABPRGMasrafPlaniForm)
            {
                string ay = "";
                string durum = "";
                string belgeTurler = "";
                string altIslemKod = "";
                string odenekKosul = "";
                string revizeNoKosul = "";
                if (!string.IsNullOrEmpty(senaryoKodVeyaAy))
                {
                    string[] bilgiler = senaryoKodVeyaAy.Split(';');
                    if (bilgiler.Length >= 1) durum = bilgiler[0];
                    if (bilgiler.Length >= 2) ay = bilgiler[1];
                    if (bilgiler.Length >= 3) belgeTurler = bilgiler[2];
                    if (bilgiler.Length >= 4) altIslemKod = bilgiler[3];
                    if (bilgiler.Length >= 5) odenekKosul = bilgiler[4];
                    if (bilgiler.Length >= 6) revizeNoKosul = bilgiler[5];
                }
                if (string.IsNullOrWhiteSpace(durum)) durum = "1/6";
                int tertipAlanSay;
                if (tabloKod == ENUMTabloKod.TABPRGMasrafPlaniForm)
                    tertipAlanSay = 11;
                else
                    tertipAlanSay = 6;
                uzayKosul = (butceYil - butceYilFark).ToString() + "--" + BoyutaTamamla(tertipSablon, tertipAlanSay) + "-" + ay + "-" + durum + "-" + altIslemKod + "-" + belgeTurler + "-" + odenekKosul + "-" + revizeNoKosul;
            }
            else if (tabloKod == ENUMTabloKod.TABGIDERAP || tabloKod == ENUMTabloKod.TABPRGGIDERAP)
            {
                string durum = "";
                string asama = "";
                string ISLEM = "0101";
                string ay = "1";
                if (!string.IsNullOrEmpty(senaryoKodVeyaAy))
                {
                    string[] bilgiler = senaryoKodVeyaAy.Split(';');
                    if (bilgiler.Length >= 1) asama = bilgiler[0];
                    if (asama == "4")
                    {
                        asama = "3";
                        durum = "5";
                        ISLEM = "";
                    }
                    if (bilgiler.Length >= 2) ay = bilgiler[1];
                }
                int tertipAlanSay;
                if (tabloKod == ENUMTabloKod.TABGIDERAP)
                {
                    tertipAlanSay = 5;
                }
                else
                    tertipAlanSay = 10;

                string tertipKosul = BoyutaTamamla(tertipSablon, tertipAlanSay);
                uzayKosul = TNS.UZY.Arac.UzayKosulOl((butceYil - butceYilFark).ToString(), "", ISLEM, asama, tertipKosul, ay, durum);
            }

            else
                uzayKosul = string.Empty;
            return uzayKosul;
        }

        private string BoyutaTamamla(string tertipSablon, int kirSay)
        {
            if (string.IsNullOrEmpty(tertipSablon)) return "".PadRight(kirSay - 1, '-');
            string[] kirilimlar = tertipSablon.Split('-');
            if (kirilimlar.Length > kirSay - 1) return tertipSablon;
            return tertipSablon + "".PadRight((kirSay) - kirilimlar.Length, '-');
        }

        private string BoyuttanVeriAl(string tertipSablon, int boyutYer)
        {
            string[] kirilimlar = tertipSablon.Split('-');
            if (kirilimlar.Length > boyutYer)
            {
                return kirilimlar[boyutYer];
            }
            return "";
        }

        /// <summary>
        /// Verilen bütçe yılı ve tertip şablonu kullanılarak, bu ek değerin hesaplanması için kullanılacak uzay koşulu oluşturulur.
        /// Tertip şablonundan, tertip boyutları bulunur. Eğer tertip kurumsal yapı içeriyorsa ve uzay saymanlık kodu ve birim içeriyorsa gerekli dönüşüm işlemi yapılır.
        /// Oluşturulan uzay koşulunun sql karşılığı bulunur ve döndürülür.
        /// </summary>
        /// <param name="uzyServis">Uzay koşulunu, sql cümlesine dönüştürecek uzay sunucu servisi nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="baglantiAd">Farklı veri tabanlarında, farklı kırılımlar kullanılabilir.
        /// Bu durumda veri tabanı bağlantısının son 4 hanesi yıl olmalıdır. Bu yıl bilgisi kullanılarak alanın kırılım bilgisi bulunur.</param>
        /// <param name="butceYil">Ek değerin hesaplanacağı ana yıl. Bütçe yıl fark değeri çıkartılarak döndürülecek yıl değeri hesaplanır.</param>
        /// <param name="tertipSablon">Ek değerin hesaplanacağı tertibe ait kodları içeren dizgi</param>
        /// <returns>Bütçe yılı ve tertip şablonu kullanılarak, bu ek değer için uzay koşulu elde edilir. Uzay koşulu uzay servisine verilir ve elde edilen sql sorgusu döndürülür.</returns>
        public string EkDegerSqlDondur(IUZYServis uzyServis, Kullanici kullanan, string baglantiAd, int butceYil, string tertipSablon)
        {
            string uzayKosul = EkDegerKosul(butceYil, tertipSablon);
            if (!string.IsNullOrEmpty(uzayKosul))
            {
                string uzayAd = UzayAdBul();
                return uzyServis.UzayKosulStr(kullanan, uzayAd + "@" + baglantiAd, uzayKosul, false, UzayAlanAdBul());
            }
            return string.Empty;
        }

        /// <summary>
        /// Verilen bütçe yılı ve tertip şablonu kullanılarak, bu ek değerin hesaplanması için kullanılacak uzay koşulu oluşturulur.
        /// Tertip şablonundan, tertip boyutları bulunur. Eğer tertip kurumsal yapı içeriyorsa ve uzay saymanlık kodu ve birim içeriyorsa gerekli dönüşüm işlemi yapılır.
        /// Oluşturulan uzay koşulu kullanılarak uzay değeri bulunur ve döndürülür.
        /// </summary>
        /// <param name="uzyServis">Uzay koşulunu, sql cümlesine dönüştürecek uzay sunucu servisi nesnesi</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYil">Ek değerin hesaplanacağı ana yıl. Bütçe yıl fark değeri çıkartılarak döndürülecek yıl değeri hesaplanır.</param>
        /// <param name="tertipSablon">Ek değerin hesaplanacağı tertibe ait kodları içeren dizgi</param>
        /// <returns>Bütçe yılı ve tertip şablonu kullanılarak, bu ek değer için uzay koşulu elde edilir. Uzay koşulu uzay servisine verilir ve bulunan değer döndürülür.</returns>
        public double EkDegerHesaplaDbl(IUZYServis uzyServis, Kullanici kullanan, int butceYil, string tertipSablon)
        {
            string uzayKosul = EkDegerKosul(butceYil, tertipSablon);
            if (!string.IsNullOrEmpty(uzayKosul))
            {
                string uzayAd = UzayAdBul();
                //string sql;
                //sql = uzyServis.UzayKosulStr(kullanan, uzayAd, uzayKosul, false, "");
                //sql += " ";
                return uzyServis.UzayDegeriDbl(kullanan, uzayAd, uzayKosul, false, UzayAlanAdBul());
            }
            return -1;
        }

        /// <summary>
        /// Tertip şablonu taşra tertibi ise, son kırılımında kurumsal yapı bilgisi bulunur.
        /// Bazı uzaylarda ise saymanlık ve birim kodu yer almaktadır.
        /// Bu yordam uzay sorgusundaki kurumsal yapı bilgisini saymanlık ve birim koduyla değiştirir ve döndürür.
        /// </summary>
        /// <param name="tertipSablon">Kurumsal yapı değeri değiştirilecek uzay sorgu dizgisi</param>
        /// <param name="sonBoyutYer">Kurumsal yapının, tertip şablonunda yer aldığı pozisyon</param>
        /// <returns>Tertip şablonunda, kurumsa yapı kodu yerine, saymanlık ve birim kodu gelmiş tertip şablonu döndürülür.</returns>
        private string SonBoyutBirim(string tertipSablon, int sonBoyutYer)
        {
            string[] kirilimlar = tertipSablon.Split('-');
            if (kirilimlar.Length == sonBoyutYer)
            {
                tertipSablon = string.Empty;
                for (int i = 0; i < sonBoyutYer - 1; i++)
                {
                    tertipSablon += (kirilimlar[i] + "-");
                }
                tertipSablon += SaymanlikBirimOl(kirilimlar[sonBoyutYer - 1]);
            }
            return tertipSablon;
        }

        private string BoyuttanKucukleriAl(string tertipSablon, int boyutYer)
        {
            string[] kirilimlar = tertipSablon.Split('-');
            if (kirilimlar.Length >= boyutYer)
            {
                tertipSablon = string.Empty;
                for (int i = 0; i < boyutYer; i++)
                {
                    tertipSablon += (kirilimlar[i] + "-");
                }
            }
            return tertipSablon;
        }

        private string BoyuttanBirimAl(string tertipSablon, int boyutYer)
        {
            string[] kirilimlar = tertipSablon.Split('-');
            if (kirilimlar.Length == boyutYer)
            {
                return SaymanlikBirimOl(kirilimlar[boyutYer - 1]);
            }
            return "-";
        }

        /// <summary>
        /// Verilen kurumsal yapının, saymanlık ve birim kodu karşılıkları bulunur ve uzay sorgusu formatında döndürülür.
        /// </summary>
        /// <param name="kurumsalYapi">Saymanlık ve birim kodu bulunacak kurumsal yapı değeri</param>
        /// <returns>Kurumsal yapının, saymanlık ve birim kodu karşılıkları uzay sorgusu formatında döndürülür.</returns>
        private string SaymanlikBirimOl(string kurumsalYapi)
        {
            if (!string.IsNullOrEmpty(kurumsalYapi))
            {
                if (kurumsalYapi.StartsWith("@"))
                {
                    kurumsalYapi = kurumsalYapi.Remove(0, 1);
                    string[] parcalar = kurumsalYapi.Split('.');
                    if (parcalar.Length == 1) return parcalar[0] + "-";
                    else if (parcalar.Length > 1) return parcalar[0] + "-" + parcalar[1];
                }
                else
                {
                    IBHMv2Servis s = TNS.BHMv2.Arac.Tanimla();
                    try
                    {
                        KurumsalYapi ky = s.KurumsalYapiDondur(null, kurumsalYapi);
                        return ky.saymanlikKod + "-" + ky.birimKod;
                    }
                    catch { }
                }
            }
            return "-";
        }

        /// <summary>
        /// Bu fonksiyon; aşama kodu ile senaryo kodu değişimi için yazılmıştır. 
        /// Aşama alanı eski kullanımda kalmış olup, tek basamaktan oluşmaktaydı. 
        /// Eğer tek basamaklı senaryo kodu değeri gelmişse, bu değerin aşama olduğu varsayılır ve 4 basamaklı senaryo koduna çevrilir.
        /// </summary>
        /// <param name="senaryoKod">Tek basamaklı ise 4 basamağa çevrilecek senaryo kodu değeri.</param>
        /// <returns>Tek basamaklı senaryo kodunun 4 basamağa çevrilmiş hali döndürülür.</returns>
        private string SenaryoyaCevir(string senaryoKod)
        {
            if (senaryoKod.Length == 1)
                return "0" + senaryoKod + ".00";
            else
                return senaryoKod;
        }

        /// <summary>
        /// Hazirlik veri tabanı çok yıllı olduğundan özel erişim gerekir.
        /// </summary>
        /// <returns>Hazirlik veri tabanı ise doğru döndürülür.</returns>
        public bool HazirlikMi()
        {
            if (tabloKod == ENUMTabloKod.TABOdenekGonderme || tabloKod == ENUMTabloKod.TABHYBloke || tabloKod == ENUMTabloKod.TABOdemeEmri || tabloKod == ENUMTabloKod.TABHarcama || tabloKod == ENUMTabloKod.TABISLAvans || tabloKod == ENUMTabloKod.TABISLHarcama || tabloKod == ENUMTabloKod.TABPEBHarcama || tabloKod == ENUMTabloKod.TABMIFKY || tabloKod == ENUMTabloKod.TABMasrafPlani || tabloKod == ENUMTabloKod.TABMasrafPlaniBirim || tabloKod == ENUMTabloKod.TABMasrafPlaniForm || tabloKod == ENUMTabloKod.TABPRGMasrafPlaniForm || tabloKod == ENUMTabloKod.TABHAZHarcama || tabloKod == ENUMTabloKod.TABPRGOdemeEmri || tabloKod == ENUMTabloKod.TABPRGOdemeEmriAvansiz || tabloKod == ENUMTabloKod.TABPRGGIDERAP || tabloKod == ENUMTabloKod.TABGIDERAP)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Ek değerin üretileceği tablonun kodunu döndüren yordam.
        /// </summary>
        /// <returns></returns>
        public ENUMTabloKod TabloKod()
        {
            return tabloKod;
        }

        public string VeriErisimAdBul()
        {
            if (tabloKod == ENUMTabloKod.TABHarcama || tabloKod == ENUMTabloKod.TABHYBloke || tabloKod == ENUMTabloKod.TABISLAvans || tabloKod == ENUMTabloKod.TABISLHarcama || tabloKod == ENUMTabloKod.TABButce || tabloKod == ENUMTabloKod.TABButceGelir || tabloKod == ENUMTabloKod.TABMasrafPlani || tabloKod == ENUMTabloKod.TABMasrafPlaniBirim || tabloKod == ENUMTabloKod.TABMasrafPlaniForm || tabloKod == ENUMTabloKod.TABPRGMasrafPlaniForm || tabloKod == ENUMTabloKod.TABOdenekGonderme || tabloKod == ENUMTabloKod.TABOdenekGondermeKY || tabloKod == ENUMTabloKod.TABOdemeEmri || tabloKod == ENUMTabloKod.TABHAZHarcama || tabloKod == ENUMTabloKod.TABPEBHarcama || tabloKod == ENUMTabloKod.TABMIFKY || tabloKod == ENUMTabloKod.TABPRGOdemeEmri || tabloKod == ENUMTabloKod.TABPRGOdemeEmriAvansiz || tabloKod == ENUMTabloKod.TABPRGHarcama || tabloKod == ENUMTabloKod.TABPRGGIDERAP || tabloKod == ENUMTabloKod.TABGIDERAP)
            {
                return "BaglantiSatiriBUM";
            }
            else
            {
                return "BaglantiSatiriBHMv2";
            }
        }

        public static string TabloAdiEkleBul(ENUMTabloKod tabloKod)
        {
            if (tabloKod == ENUMTabloKod.TABOdemeEmri || tabloKod == ENUMTabloKod.TABPEBHarcama) //|| tabloKod == ENUMTabloKod.TABPRGOdemeEmri || tabloKod == ENUMTabloKod.TABPRGOdemeEmriAvansiz
                return "H";
            else
                return "";
        }
    }
}

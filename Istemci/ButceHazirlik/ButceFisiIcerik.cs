using System;
using System.Collections.Generic;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.HRC;
using TNS.KYM;
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    /// <summary>
    /// Bütçe fişinin HTML içeriği (eski ButceHazirlikv2/ButceFisiDetay.aspx.cs içindeki ButceFisiIcerik ve ButceGelirFisiIcerik
    /// sınıfları; kaynak metinleri (Resources.ButceHazirlikv2) yerine yazıldı, kod eskisi gibi). Bütçe fişi sayfası ekranda bunu
    /// gösterir, Yazdır ile Word (.doc) dosyasına yazar.
    /// </summary>
    internal static class ButceFisiOrtak
    {
        private static readonly HashSet<string> SurecliEkonomikKodlar = new HashSet<string> {
            "032101", "032102", "032103", "032104", "032105", "032190", "032202", "032301", "032302", "032402",
            "032490", "032601", "032602", "032690", "032990", "035103", "035104", "035108", "035109", "035190",
            "035201", "035203", "035204", "035401", "035402", "035502", "035505", "035506", "036101", "037101",
            "037102", "037103", "037104", "037190", "037201", "037301", "037302", "037303", "037390", "038101",
            "038190", "038201", "038901", "061101", "061102", "061201", "061205", "061290", "061401", "063101",
            "065501", "066201", "066701", "067701", "067703", "069901"
        };

        /// <summary>Eski ButceFisiGiris.PersonelGizliMi: ButceFisiPersonelDahil değişkeni 1 değilse personel giderleri (01, 02) gösterilmez.</summary>
        public static bool PersonelGizliMi(string ekoKod, string personelDahilPar)
        {
            if (personelDahilPar == "1") return false;
            if (ekoKod.StartsWith("01")) return true;
            if (ekoKod.StartsWith("02")) return true;
            return false;
        }

        /// <summary>
        /// HTML ana başlıkları yazıldıktan sonra bütçe fişini dosyaya ekleyip HTML'i kapatan yordam (eski
        /// ButceHazirlikRaporlari.ButceFisiDosyayaYaz; Word bu dosyayı .doc olarak açar).
        /// </summary>
        public static void ButceFisiDosyayaYaz(string sayfa, System.IO.TextWriter dosyaGos)
        {
            dosyaGos.Write("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>\r\n");
            dosyaGos.Write("<head>\r\n");
            dosyaGos.Write("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>\r\n");
            dosyaGos.Write("<meta name='ProgId' content='Word.Document'/>\r\n");
            dosyaGos.Write("<meta name='Generator' content='Microsoft Word 9'/>\r\n");

            dosyaGos.Write("<style>\r\n");
            dosyaGos.Write("<!--\r\n");
            dosyaGos.Write(" p.MsoNormal, li.MsoNormal, div.MsoNormal\r\n");
            dosyaGos.Write("{mos-style-parent:'';\r\n");
            dosyaGos.Write("{margin:0cm;\r\n");
            dosyaGos.Write("margin-bottom:.0001pt;\r\n");
            dosyaGos.Write("font-size:10pt;\r\n");
            dosyaGos.Write("font-family:'Times New Roman';}\r\n");
            dosyaGos.Write("mso-fareast-font-family:'Times New Roman';}\r\n");
            dosyaGos.Write("@page Section1\r\n");
            dosyaGos.Write("{size:595.3pt 841.9pt;\r\n");
            dosyaGos.Write("margin:70.85pt 70.85pt 70.85pt 70.85pt;}\r\n");
            dosyaGos.Write("div.Section1\r\n");
            dosyaGos.Write("{page:Section1;}\r\n");
            dosyaGos.Write("-->\r\n");
            dosyaGos.Write("</style>\r\n");

            GenelIslemler.HTMLWordLandScapeSytle(dosyaGos);

            dosyaGos.Write("</head>\r\n");
            dosyaGos.Write("<body lang=TR>\r\n");
            dosyaGos.Write("<div class=Section1>\r\n");

            dosyaGos.Write(sayfa);

            dosyaGos.Write("</div>\r\n");
            dosyaGos.Write("</body>\r\n");
            dosyaGos.Write("</html>\r\n");
        }

        /// <summary>Eski BHMv2Ortak.EkonomikKodunSureciVarMi: ekonomik kodla ilişkili harcama süreci var mı?</summary>
        public static bool EkonomikKodunSureciVarMi(string ekokod)
        {
            return SurecliEkonomikKodlar.Contains(ekokod.Replace(".", ""));
        }
    }

    /// <summary>
    /// Bütçe fişinin html görüntüsünü oluşturan sınıf
    /// </summary>
    internal class ButceFisiIcerik
    {
        private static IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

        private static IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();

        /// <summary>
        /// Ekonomik kodu 01 ve 02 olmayan tertipler için bütçe fişini html olarak hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="trtp">Bütün kodları içeren tertip</param>
        /// <param name="senaryo">rakamların alınacağı senaryo</param>
        /// <param name="onceSayfaSonu">Bu tertipten önce sayfa sonu konulacaksa true</param>
        /// <returns>html olarak bütçe fişi döndürülür</returns>
        public static string SayfaOlustur(Kullanici kullanan, int yil, string trtp, string senaryo, bool onceSayfaSonu)
        {
            string personelDahilPar = TNS.DEG.Arac.DegiskenDegerBul(yil, "ButceFisiPersonelDahil");

            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            tertip.TertipAta(trtp);
            string ekoKod = tertip.kodlar[3];
            if (ButceFisiOrtak.PersonelGizliMi(ekoKod, personelDahilPar)) return "";

            string hesapPlanKod = TertipDetayUclari.ParametrikHesapPlanDon(ekoKod);

            string kesinHesap = KesinHesapTabloUret(kullanan, yil - 1, trtp);
            string harcamaTablo = HarcamaTabloUret(kullanan, yil - 1, trtp, ekoKod);
            string odenekUstuTalep = OdenekUstuTabloUret(kullanan, yil, senaryo, trtp);

            //string altFaaliyetTablo = AltFaaliyetTabloUret(kullanan, yil, tertip, senaryo);
            string malzemeTablo = string.Empty;

            if (!string.IsNullOrEmpty(hesapPlanKod))
            {
                malzemeTablo = MalzemeTabloUret(kullanan, tertip);
            }

            if (!string.IsNullOrEmpty(kesinHesap)) kesinHesap += "<br>";
            if (!string.IsNullOrEmpty(odenekUstuTalep)) odenekUstuTalep += "<br>";
            if (!string.IsNullOrEmpty(harcamaTablo)) harcamaTablo += "<br>";
            //if (!string.IsNullOrEmpty(altFaaliyetTablo)) altFaaliyetTablo += "<br>";
            if (!string.IsNullOrEmpty(malzemeTablo)) malzemeTablo += "<br>";

            string xx, yy;
            string gerekce = GerekceGetir(kullanan, yil, trtp, false, out xx, out yy);

            tertip.Noktala();
            tertip.AdlariBul(servisUZY);

            //string[] kurDizi = tertip.kodlar[0].Split('.');
            //string[] fonDizi = tertip.kodlar[1].Split('.');
            //string[] finDizi = tertip.kodlar[2].Split('.');
            //string[] ekoDizi = tertip.kodlar[3].Split('.');

            if (String.IsNullOrEmpty(senaryo))
                senaryo = "00.00";
            string kosul = yil + "-" + senaryo + "-{0}-" + trtp;

            double odenek1 = servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", String.Format(kosul, 0), false);
            double odenek2 = servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", String.Format(kosul, 1), false);
            double odenek3 = servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", String.Format(kosul, 2), false);

            string ekoAd = tertip.adlar[3];

            string bosluk = ""; // "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            string sayfa = string.Empty;
            if (onceSayfaSonu)
                sayfa = "<span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";mso-fareast-font-family:\"Times New Roman\";mso-ansi-language:TR;mso-fareast-language:TR;mso-bidi-language:AR-SA'><br clear=all style='mso-special-character:line-break;page-break-before:always'></span>";
            sayfa += "<p  class=MsoNormal align=right style='text-align:right'><b>" + "FORM:13" + "</b></p>" +
            "<p class=MsoNormal align='center'><b>" + "GİDER BÜTÇE FİŞİ" + "</b></p>" +
            "<p class=MsoNormal align='left'><b>" + "KURUM ADI :" + "</b>&nbsp;" + ButceHazirlikv2.BHGiderRaporlari.KurumAd() + "</p>" +
            "<p  class=MsoNormal align='right' style='text-align:right'>" + "(TL)" + "</p>" +
            "<table border='1' width='100%' id='table1' style='border-collapse: collapse' bordercolor='#000000'>" +
            "<colgroup span='4' style='width:7%'></colgroup>" +
            "<colgroup style='width:48%'></colgroup>" +
            "<colgroup span='3' style='8%'></colgroup>" +
            "<tr>" +
            "<td style='border-style: solid; border-width: 1px' rowspan='2' align='center'>" +
            "<p class=MsoNormal  style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "KURUMSAL" + "</font></b></p>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b>" +
            "<font size='2'>" + "SINIFLANDIRMA" + "</font></b></td>" +
            "<td style='border-style: solid; border-width: 1px' rowspan='2' align='center'>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "FİN." + "</font></b></p>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "TİPİ" + "</font></b></td>" +
            "<td style='border-style: solid; border-width: 1px' rowspan='2' align='center'>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "EKONOMİK" + "</font></b></p>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b>" +
            "<font size='2'>" + "SINIFLANDIRMA" + "</font></b></td>" +

            "<td style='border-style: solid; border-width: 1px' rowspan='2' align='center'>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "ALT&nbsp;FAALİYET" + "</font></b></p>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b>" +
            "<font size='2'>" + "SINIFLANDIRMASI" + "</font></b></td>" +

            "<td style='border-style: solid; border-width: 1px' align='center' rowspan='2'>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "AÇIKLAMA" + "</font></b></td>" +
            "<td style='border-style: solid; border-width: 1px' colspan='3' align='center'>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "BÜTÇE TEKLİFİ" + "</font></b></td>" +
            "</tr>" +

            "<tr>" +
            "<td  width='10%' style='border-style: solid; border-width: 1px' align='center'>" +
            "<b>" + yil.ToString() + "</b></td>" +
            "<td  width='10%' style='border-style: solid; border-width: 1px' align='center'>" +
            "<b>" + (yil + 1).ToString() + "</b></td>" +
            "<td  width='10%' style='border-style: solid; border-width: 1px' align='center'>" +
            "<b>" + (yil + 2).ToString() + "</b></td>" +
            "</tr>" +

            "<tr>" +
            "<td  style='border-style: solid; border-width: 1px' align='center'>" + tertip.kodlar[0] + "</td>" +
            "<td  style='border-style: solid; border-width: 1px' align='center'>" + tertip.kodlar[2] + "</td>" +
            "<td  style='border-style: solid; border-width: 1px' align='center'>" + tertip.kodlar[3] + "</td>" +
            "<td  style='border-style: solid; border-width: 1px' align='center'>" + tertip.kodlar[4] + "</td>" +
            "<td  style='border-style: solid; border-width: 1px' align='left'>" +
            ekoAd + "</td>" +
           "<td style='border-style: solid; border-width: 1px' align='right'>" +
            bosluk + odenek1.ToString("#,##0") + "</td>" +
            "<td style='border-style: solid; border-width: 1px' align='right'>" +
            bosluk + odenek2.ToString("#,##0") + "</td>" +
            "<td style='border-style: solid; border-width: 1px' align='right'>" +
            bosluk + odenek3.ToString("#,##0") + "</td>" +
            "</tr>" +
            "<tr>" +
            "<td  height=400 valign='top' style='padding:5px 5px 5px 5px; border-style: none; border-width: 1px' colspan='17'><b><u>" +
            "</u></b><p>" +
            "<br>" + kesinHesap + harcamaTablo + malzemeTablo + gerekce + odenekUstuTalep + "</td>" +
            "</tr>" +
            "</table>";

            return sayfa;
        }

        private static string OdenekUstuTabloUret(Kullanici kullanan, int yil, string senaryoKod, string trtp)
        {
            string[] senKod = { senaryoKod };

            KistasYapi tertip = new KistasYapi();
            tertip.ekleTertipler = new Tertip[1];
            tertip.ekleTertipler[0] = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, trtp);
            TavanUstuTertipOdenek[] tUstler = servisBHMv2.TavanUstuOdenekListele(kullanan, senKod, tertip, true);

            string fazla = "0";
            string gerekce = "-";
            if (tUstler != null && tUstler.Length > 0)
            {
                fazla = tUstler[0].ekOdenek0.ToString("#,##0");
                gerekce = tUstler[0].gerekce;
            }
            string tablo = "<table width=\"95%\" border=\"1\">" +
            "<tr>" +
            "<td style=\"width:250px; font-weight:bold;\">Ödenek Üstü Talep Miktarı</td>" +
            $"<td>{fazla}</td>" +
            "</tr>" +
            "<tr>" +
            $"<td style=\"width:250px; font-weight:bold;\">Gerekçesi</td>" +
            $"<td>{gerekce}</td>" +
            "</tr></table>";

            return tablo;
        }

        private static object DizidenAl(string[] dizi, int yer, string varsayilan)
        {
            if (yer >= dizi.Length)
                return varsayilan;
            return dizi[yer];
        }

        /// <summary>
        /// Ekonomik kodu 01 ve 02 olmayan tertipler için bütçe fişinin gerekçe kısmınıni html olarak hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="trtp">Bütün kodları içeren tertip</param>
        /// <param name="senaryo">rakamların alınacağı senaryo</param>
        /// <returns>html olarak bütçe fişi döndürülür</returns>        
        public static string AktarmaIcerikOlustur(Kullanici kullanan, int yil, string trtp, string senaryo, out string baslangicFark, out string sonrakiFark)
        {
            baslangicFark = "";
            sonrakiFark = "";
            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
            tertip.TertipAta(trtp);
            string ekoKod = tertip.kodlar[3];
            if (ekoKod.StartsWith("01") || ekoKod.StartsWith("02")) return "";

            string hesapPlanKod = TertipDetayUclari.ParametrikHesapPlanDon(ekoKod);

            string kesinHesap = KesinHesapTabloUret(kullanan, yil - 1, trtp);
            if (!string.IsNullOrEmpty(kesinHesap)) kesinHesap += "<br>";
            string harcamaTablo = HarcamaTabloUret(kullanan, yil - 1, trtp, ekoKod);
            if (!string.IsNullOrEmpty(harcamaTablo)) harcamaTablo += "<br>";
            string malzemeTablo = string.Empty;
            if (!string.IsNullOrEmpty(hesapPlanKod))
            {
                malzemeTablo = MalzemeTabloUret(kullanan, tertip);
            }
            if (!string.IsNullOrEmpty(malzemeTablo)) malzemeTablo += "<br>";

            string gerekce = GerekceGetir(kullanan, yil, trtp, false, out baslangicFark, out sonrakiFark);

            string icerik = kesinHesap + harcamaTablo + malzemeTablo + gerekce;

            return icerik;
        }

        /// <summary>
        /// Kullanıcının girdiği ve otomatik olarak üretilen kısma eklenecek kullanıcının girdiği gerekçeyi
        /// getiren yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Bütün kodları içeren tertip</param>
        /// <returns>Kullanıcının girdiği gerekçe döndürülür</returns>
        public static string GerekceGetir(Kullanici kullanan, int yil, string tertip, bool sebepEkle, out string baslangicFark, out string sonrakiFark)
        {
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            t.TertipAta(tertip);
            string[] gler = servisBHMv2.ButceFisiGerekceYaziGetir(kullanan, t);
            baslangicFark = OrtakFonksiyonlar.DiziElemanAl(gler, 1, "");
            sonrakiFark = OrtakFonksiyonlar.DiziElemanAl(gler, 2, "");
            string gerekce = OrtakFonksiyonlar.DiziElemanAl(gler, 0, "");
            if (sebepEkle)
                gerekce += (SebebYap("Başlangıç Ödeneğinden Farklılaşma Sebebi", baslangicFark) + SebebYap("Sonraki Yıl Ödeneklerinin Farklılaşma Sebebi", sonrakiFark));
            return gerekce;
        }

        private static string SebebYap(string baslik, string metin)
        {
            if (string.IsNullOrWhiteSpace(metin)) return "";
            return "<p /><b>" + baslik + ": </b>" + metin;
        }

        /// <summary>
        /// Bir tertip ile ilişkili malzeme listesi sunucudan alan ve html olarak döndüren yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="tertip">Malzemesi getirilecek tertip nesnesi</param>
        /// <returns>Kullanıcının girdiği tertip ile ilişkili malzeme listesi html olarak döndürülür</returns>
        private static string MalzemeTabloUret(Kullanici kullanan, Tertip tertip)
        {
            TNS.BHMv2.ButceFisiMalzeme[] liste = servisBHMv2.ButceFisiMalzemeListele(kullanan, tertip);

            string tablo = "<h3>" + "MALZEME LİSTESİ" + "</h3><br>" +
                           "<table width=\"95%\" border=\"1\">" +
                               "<tr style=\"text-align:center; font-weight:bold;\">" +
                                "<td>" + "Adı" + "</td>" +
                                "<td>" + "Miktar" + "</td>" +
                                "<td>" + "Ölçü Birimi" + "</td>" +
                                "<td>" + "Birim Fiyat" + "</td>" +
                                "<td>" + "Toplam Tutar" + "</td>" +
                               "</tr>{0}<tr style=\"text-align:center; font-weight:bold;\"><td colspan=\"4\"> " + "T O P L A M" + "</td><td>{1}</td></tr></table><br>";

            string satirlar = string.Empty;
            decimal toplam = 0;
            foreach (TNS.BHMv2.ButceFisiMalzeme m in liste)
            {
                satirlar += MalzemeSatirUret(m, ref toplam);
            }

            if (!string.IsNullOrEmpty(satirlar))
            {
                return String.Format(tablo, satirlar, toplam.ToString("#,##0.00"));
            }
            return string.Empty;
        }

        /// <summary>
        /// Malzeme tablosuna eklenecek bir satır html olarak üretilir.
        /// </summary>
        /// <param name="malzeme">HTML dizgiye çevrilecek malzeme nesnesi</param>
        /// <param name="toplam">Bu satıra ait miktar toplam'a eklenir</param>
        /// <returns>Malzemeye ait html dizgi</returns>
        private static string MalzemeSatirUret(TNS.BHMv2.ButceFisiMalzeme malzeme, ref decimal toplam)
        {
            string tabloSatir = "<tr>" +
                "<td style=\"text-align:left;\">{0}</td>" + //adı
                "<td style=\"text-align:right;\">{1}</td>" + //miktar
                "<td style=\"text-align:left;\">{2}</td>" + //olcubirim
                "<td style=\"text-align:right;\">{3}</td>" + //birimfiyat
                "<td style=\"text-align:right;\">{4}</td>" + //toplamtutar
                "</tr>";

            string satir = string.Format(tabloSatir, malzeme.hesapAd, malzeme.miktar.ToString("#,###.00"), malzeme.olcuBirimAd, malzeme.birimFiyat.ToString("#,###.00"), malzeme.toplamTutar.ToString("#,###.00"));
            toplam += malzeme.toplamTutar;
            return satir;
        }

        /// <summary>
        /// Eğer verilen tertibe ait ekonomik kodla ilişkili süreç var ise, 
        /// bu tertiple ilgili bütçe fişi gerekçesine konulmak üzere html harcama tablosu üretilir.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Harcama süreci tablosu getirilecek tertip dizgisi</param>
        /// <param name="ekokod">Tertibin ekonomik kodu</param>
        /// <returns>Kullanıcının girdiği tertip ile ilişkili harcama süreci tablosu html olarak döndürülür</returns>
        private static string HarcamaTabloUret(Kullanici kullanan, int yil, string tertip, string ekokod)
        {
            string deger = TNS.DEG.Arac.DegiskenDegerBul(yil + 1, "ButceFisiHarcama");
            if (string.IsNullOrEmpty(deger) || deger == "0") return "";

            if (ButceFisiOrtak.EkonomikKodunSureciVarMi(ekokod))
            {
                string tablo = "<h3>" + "HARCAMA" + "</h3><p>" +
                               "<table width=\"95%\" border=\"1\">" +
                                   "<tr style=\"text-align:center; font-weight:bold;\">" +
                                    "<td>" + "Yıl" + "</td>" +
                                    "<td>" + "KBÖ" + "</td>" +
                                    "<td>" + "Onay" + "</td>" +
                                    "<td>" + "Sözleşme" + "</td>" +
                                    "<td>" + "Tahakkuk*" + "</td>" +
                                    "<td>" + "Ödeme" + "</td>" +
                                    "<td>" + "Fark" + "</td>" +
                                   "</tr>{0}</table>" +
                                   "<br>" + "* Tahakkuk rakamlarına KDV dahildir." + "<br>";

                string satir = string.Empty;
                string[] dipNot = new string[3];
                for (int i = yil; i > yil - 3; i--)
                {
                    satir += HarcamaSatirUret(kullanan, i, tertip, out dipNot[yil - i]);
                }

                return String.Format(tablo, satir) + "<br>" + dipNot[0];
            }
            else return "";
        }

        /// <summary>
        /// Harcama tablosuna eklenecek her bir satır üretilir.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Harcama süreci tablosu getirilecek tertip dizgisi</param>
        /// <param name="dipNot">Tablonun altına dipnot eklenecekse bu değişkende döndürülür.</param>
        /// <returns>Harcama tablosuna eklenecek satır döndürülür.</returns>
        private static string HarcamaSatirUret(Kullanici kullanan, int yil, string tertip, out string dipNot)
        {
            string tabloSatir = "<tr>" +
                "<td style=\"text-align:center;\">{0}</td>" + //yil
                "<td style=\"text-align:right;\">{1}</td>" + //kbö
                "<td style=\"text-align:right;\">{2}</td>" + //onay
                "<td style=\"text-align:right;\">{3}</td>" + //sözleşme
                "<td style=\"text-align:right;\">{4}</td>" + //tahakkuk
                "<td style=\"text-align:right;\">{5}</td>" + //ödeme
                "<td style=\"text-align:right;\">{6}</td>" + //fark
                "</tr>";

            IHRCServis servisHRC = TNS.HRC.Arac.Tanimla();
            double kbo = KBOGetir(kullanan, yil, tertip);

            string[] tertipler = tertip.Split('-');

            string kosul = yil + "-------" + tertipler[0] + "-" + tertipler[1] + "-" + tertipler[2] + "-" + tertipler[3] + "-";
            double onay = servisUZY.UzayDegeriDbl(kullanan, "ABSHAZSUREC", kosul, false, "");
            double alim = servisUZY.UzayDegeriDbl(kullanan, "ABSHAZSUREC", kosul, false, "SUM(ALIM)");

            string sql = servisUZY.UzayKosulStr(kullanan, "ABSHAZSUREC", kosul, false, "SUM(ODEME)");

            double fatura = servisUZY.UzayDegeriDbl(kullanan, "ABSHAZSUREC", kosul, false, "SUM(FATURA)");
            double teslim = servisUZY.UzayDegeriDbl(kullanan, "ABSHAZSUREC", kosul, false, "SUM(TESLIM)");
            double odeme = servisUZY.UzayDegeriDbl(kullanan, "ABSHAZSUREC", kosul, false, "SUM(ODEME)");

            string satir = string.Format(tabloSatir, yil, kbo.ToString("#,##0"), onay.ToString("#,##0"), alim.ToString("#,##0"), fatura.ToString("#,##0"), odeme.ToString("#,##0"), (kbo - odeme).ToString("#,##0"));
            if (yil == DateTime.Now.Year)
            {
                dipNot = string.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + "Harcama Süreçlerine ilişkin veriler dikkate alınırsa; {0} yılında {1} TL başlangıç ödeneğinden, {2} TL harcama onayı alınmış, bu harcama onayına karşılık KDV dahil {3} TL tahakkuk gerçekleşmiş ve bu tahakkukun {4} TL’si ödenmiştir.", yil.ToString("###0"), kbo.ToString("#,##0"), onay.ToString("#,##0"), fatura.ToString("#,##0"), odeme.ToString("#,##0"));
            }
            else
            {
                dipNot = "";
            }
            return satir;
        }

        //private static string TasinirSatirUret(Kullanici kullanan, int yil, string tertip)
        //{
        //    string tabloSatir = "<tr>" +
        //        "<td style=\"text-align:center;\">{0}</td>" + //yil
        //        "<td style=\"text-align:right;\">{1}</td>" + //mevcut
        //        "<td style=\"text-align:right;\">{2}</td>" + //giren
        //        "<td style=\"text-align:right;\">{3}</td>" + //kullanılan
        //        "<td style=\"text-align:right;\">{4}</td>" + //kalan
        //        "</tr>";

        //    string satir = string.Format(tabloSatir, yil, "mevcut", "giren", "kullanılan", "kalan");
        //    return satir;
        //}

        /// <summary>
        /// Verilen tertibe ait kesin hesap tablosunu html olrak döndüren yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Kesin hesap tablosu getirilecek tertip dizgisi</param>
        /// <returns>Kesin hesap tablosu html olarak döndürülür.</returns>
        private static string KesinHesapTabloUret(Kullanici kullanan, int yil, string tertip)
        {
            string deger = TNS.DEG.Arac.DegiskenDegerBul(yil + 1, "ButceFisiKesinHesap");
            if (string.IsNullOrEmpty(deger) || deger == "0") return "";

            string tablo = "<h3>" + "KESİN HESAP" + "</h3><p>" +
                "<table width=\"95%\" border=\"1\">" +
                "<tr style=\"text-align:center; font-weight:bold;\">" +
                "<td rowspan=\"2\">" + "Yıl" + "</td>" +
                "<td rowspan=\"2\">" + "KBÖ" + "</td>" +
                "<td rowspan=\"2\">" + "Gerçekleşen" + "</td>" +
                "<td rowspan=\"2\">" + "Gerçekleşen%" + "</td>" +
                "<td colspan=\"2\">" + "Değişim%" + "</td>" +
                "</tr>" +
                "<tr style=\"text-align:center; font-weight:bold;\">" +
                "<td>" + "KBÖ" + "</td>" +
                "<td>" + "Gerçekleşen" + "</td>" +
                "</tr>{0}</table>";

            string satir = string.Empty;
            string[] yilDizi = new string[3];
            for (int i = yil; i > yil - 3; i--)
            {
                satir += KesinHesapSatirUret(kullanan, i, tertip, out yilDizi[yil - i]);
            }

            string kesinHesapTablo = String.Format(tablo, satir);
            kesinHesapTablo = kesinHesapTablo + "<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + String.Format("Bu tertibin, Kesin hesaba giren verilerine göre; {0} {1} {2}", yilDizi[2], yilDizi[1], yilDizi[0]);
            return kesinHesapTablo;
        }


        private static string AltFaaliyetSatirUret(Kullanici kullanan, OdenekBilgi odenekBilgi)
        {
            string tabloSatir = "<tr>" +
                "<td style=\"text-align:left;\">{0}</td>" + //Alt faal ad
                "<td style=\"text-align:right;\">{1}</td>" + //ödenek
                "<td style=\"text-align:right;\">{2}</td>" + //ödenek + 1
                "<td style=\"text-align:right;\">{3}</td>" + //ödenek + 2
                "</tr>";

            return string.Format(tabloSatir, odenekBilgi.tertip.adlar[4], ((double)odenekBilgi.ekRakamlar[0]).ToString("#,##0"), ((double)odenekBilgi.ekRakamlar[1]).ToString("#,##0"), ((double)odenekBilgi.ekRakamlar[2]).ToString("#,##0"));
        }

        private static string AltFaaliyetTabloUret(Kullanici kullanan, int yil, Tertip tertip, string senaryoKod)
        {
            string deger = TNS.DEG.Arac.DegiskenDegerBul(yil + 1, "ButceFisiAltFaaliyet");
            if (deger == "1") return "";

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(tertip.KodNolar(4), tertip.KodNoKirilimSay(4)));

            EkDegerParam[] ekBilgiler = new EkDegerParam[3];

            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 0, senaryoKod);
            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 1, senaryoKod);
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABPEBOdenek, 0, 2, senaryoKod);

            try
            {
                OdenekBilgi[] obler = servisBHMv2.ListeTertipOl(kullanan, tertip, kodBilgileri, ekBilgiler, true);

                if (obler == null || obler.Length <= 1) return "";


                string tablo = "<br><h3>" + "ÖDENEĞİN ALT FAALİYETLERE/BİRİMLERE DAĞILIMI" + "</h3><p>" +
                    "<table width=\"95%\" border=\"1\">{0}</table>";

                string satir = string.Empty;
                string[] yilDizi = new string[3];
                for (int i = 0; i < obler.Length; i++)
                {
                    satir += AltFaaliyetSatirUret(kullanan, obler[i]);
                }

                return String.Format(tablo, satir);
            }
            catch { return ""; }
        }


        /// <summary>
        /// Kesin hesap tablosuna eklenecek satır üretilir.
        /// İlgili satırla ilgili tablo altına eklenecek açıklama dipnot değişkeninde döndürülür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Kesin hesap tablosu getirilecek tertip dizgisi</param>
        /// <param name="dipNot">Üretilen yılla ilgili açıklama bilgisi bu alanda döndürülür.</param>
        /// <returns>Kesin hesap tablosuna eklenecek satır html olarak döndürülür.</returns>
        private static string KesinHesapSatirUret(Kullanici kullanan, int yil, string tertip, out string dipNot)
        {
            string tabloSatir = "<tr>" +
                "<td style=\"text-align:center;\">{0}</td>" + //yil
                "<td style=\"text-align:right;\">{1}</td>" + //kbö
                "<td style=\"text-align:right;\">{2}</td>" + //gerçekleşen
                "<td style=\"text-align:right;\">{3} %</td>" + //gerçekleşen%
                "<td style=\"text-align:right;\">{4} %</td>" + //degisim kbö %
                "<td style=\"text-align:right;\">{5} %</td>" + //degisim gerçekleşen%
                "</tr>";

            double ger = GerceklesmeGetir(kullanan, yil, tertip);
            double kbo = KBOGetir(kullanan, yil, tertip);

            double gerYuzde = Math.Round((ger / kbo) * 100, 2);
            double gerDegYuzde = GerceklesmeDegisimYuzdeGetir(kullanan, yil, tertip);
            double kboDegYuzde = KBODegisimYuzdeGetir(kullanan, yil, tertip);

            string gerYuzdeStr = gerYuzde.ToString("##0");
            string kboDegYuzdeStr = kboDegYuzde.ToString("##0");
            string gerDegYuzdeStr = gerDegYuzde.ToString("##0");

            if ((gerYuzde.ToString() == "NaN") || (gerYuzde.ToString() == "Infinity") || (gerYuzde.ToString() == "-Infinity"))
            {
                gerYuzdeStr = "-";
            }

            if ((gerDegYuzde.ToString() == "NaN") || (gerDegYuzde.ToString() == "Infinity") || (gerDegYuzde.ToString() == "-Infinity"))
            {
                gerDegYuzdeStr = "-";
            }

            if ((kboDegYuzde.ToString() == "NaN") || (kboDegYuzde.ToString() == "Infinity") || (kboDegYuzde.ToString() == "-Infinity"))
            {
                kboDegYuzdeStr = "-";
            }

            string satir = string.Format(tabloSatir, yil, kbo.ToString("#,##0"), ger.ToString("#,##0"), gerYuzdeStr, kboDegYuzdeStr, gerDegYuzdeStr);
            if (yil == DateTime.Now.Year)
                dipNot = "<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + string.Format("{0} yılında {1} TL ödenek mevcut olup haziran sonu itibarıyla {2} TL harcama gerçekleşmiş ve harcama oranı yaklaşık %{3} olmuştur.", yil.ToString("###0"), kbo.ToString("#,##0"), ger.ToString("#,##0"), gerYuzdeStr);
            else
                dipNot = string.Format("{0} yılında {1} TL başlangıç ödeneğine karşın {2} TL harcaması olduğundan gerçekleştirme oranı %{3} olmuştur.", yil.ToString(("###0")), kbo.ToString("#,##0"), ger.ToString("#,##0"), gerYuzdeStr);
            return satir;
        }

        /// <summary>
        /// Verilen tertibin bir önceki yıla göre gerçekleşme değişim oranı döndürülür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Gerçekleşme değişim yüzdesi getirilecek tertip dizgisi</param>
        /// <returns>Tertibin gerçekleşme değişim oranı</returns>
        private static double GerceklesmeDegisimYuzdeGetir(Kullanici kullanan, int yil, string tertip)
        {
            double ger = GerceklesmeGetir(kullanan, yil, tertip);
            double oncekiGer = GerceklesmeGetir(kullanan, yil - 1, tertip);
            return Math.Round((ger - oncekiGer) / ger * 100, 2);
        }

        /// <summary>
        /// Verilen tertibin, verilen yıldaki harcama miktarını bulan yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Gerçekleşme miktarı getirilecek tertip dizgisi</param>
        /// <returns>Tertibin gerçekleşme miktarı</returns>
        private static double GerceklesmeGetir(Kullanici kullanan, int yil, string tertip)
        {
            int ay = 12;
            if (yil == DateTime.Now.Year)
                ay = 6; //DateTime.Now.Month - 1;
            string senaryoKodVeyaAy = "";
            if (!string.IsNullOrEmpty(TNS.KYM.Arac.ServerWebConfigOku("MuhasebatTur", "")))
                senaryoKodVeyaAy = "0/" + (ay + 1).ToString();
            else
                senaryoKodVeyaAy = ay.ToString();

            string kosul = yil + "-" + senaryoKodVeyaAy + "-" + tertip;
            return servisUZY.UzayDegeriDbl(kullanan, "ABSHAZMUHASEBAT", kosul, false);
        }

        /// <summary>
        /// Verilen tertibin bir önceki yıla göre başlangıç ödeneği artış yüzdesi döndürülür.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Başlangıç ödeneği artış yüzdesi getirilecek tertip dizgisi</param>
        /// <returns>Tertibin başlangıç ödeneği artış yüzdesi </returns>
        private static double KBODegisimYuzdeGetir(Kullanici kullanan, int yil, string tertip)
        {
            double kbo = KBOGetir(kullanan, yil, tertip);
            double oncekikbo = KBOGetir(kullanan, yil - 1, tertip);
            return Math.Round((kbo - oncekikbo) / kbo * 100, 2);
        }

        /// <summary>
        /// Verilen tertibin, verilen yıldaki başlangıç ödeneğini bulan yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Başlangıç ödeneği getirilecek tertip dizgisi</param>
        /// <returns>Tertibin başlangıç ödeneği</returns>
        private static double KBOGetir(Kullanici kullanan, int yil, string tertip)
        {
            string kosul = yil + "-06.00-0-" + tertip;
            return servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", kosul, false);
        }
    }

    /// <summary>
    /// Bütçe fişinin html görüntüsünü oluşturan sınıf
    /// </summary>
    internal class ButceGelirFisiIcerik
    {
        private static IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        private static IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();

        /// <summary>
        /// Verilen tertip için bütçe gelir fişini html olarak hazırlayan yordam.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="trtp">Bütün kodları içeren tertip</param>
        /// <param name="senaryo">rakamların alınacağı senaryo</param>
        /// <param name="onceSayfaSonu">Bu tertipten önce sayfa sonu konulacaksa true</param>
        /// <returns>html olarak bütçe gelir fişi döndürülür</returns>
        public static string SayfaOlustur(Kullanici kullanan, int yil, string trtp, string senaryo, bool onceSayfaSonu)
        {
            Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBGelir);
            tertip.TertipAta(trtp);
            tertip.Noktala();
            tertip.AdlariBul(servisUZY);

            string ekoKod = tertip.kodlar[1];
            string[] ekoDizi = tertip.kodlar[1].Split('.');
            string gerekce = GerekceGetir(kullanan, yil, trtp);

            if (String.IsNullOrEmpty(senaryo))
                senaryo = "00.00";
            string kosul = yil + "-" + senaryo + "-{0}-" + trtp;

            double odenek1 = servisUZY.UzayDegeriDbl(kullanan, "ABSGELIR", String.Format(kosul, 0), false);
            double odenek2 = servisUZY.UzayDegeriDbl(kullanan, "ABSGELIR", String.Format(kosul, 1), false);
            double odenek3 = servisUZY.UzayDegeriDbl(kullanan, "ABSGELIR", String.Format(kosul, 2), false);

            string ekoAd = tertip.adlar[1];

            string sayfa = string.Empty;
            if (onceSayfaSonu)
                sayfa = "<span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";mso-fareast-font-family:\"Times New Roman\";mso-ansi-language:TR;mso-fareast-language:TR;mso-bidi-language:AR-SA'><br clear=all style='mso-special-character:line-break;page-break-before:always'></span>";
            sayfa += "<p  class=MsoNormal align=right style='text-align:right'><b>" + "FORM:15(1)" + "</b></p>" +
            "<p class=MsoNormal align='center'><b>" + "GELİR BÜTÇE FİŞİ" + "</b></p>" +
            "<p class=MsoNormal align='left'><b>" + "KURUM ADI :" + "</b>&nbsp;" + ButceHazirlikv2.BHGiderRaporlari.KurumAd() + "</p>" +
            "<p  class=MsoNormal align=right style='text-align:right'>" + "(TL)" + "</p>" +
            "<table border='1' width='100%' id='table1' style='border-collapse: collapse' bordercolor='#000000'>" +
            "<tr>" +
            "<td style='border-style: solid; border-width: 1px' colspan='4' align='center'>" +
            "<p class=MsoNormal  style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "EKONOMİK" + "</font></b></p>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b>" +
            "<font size='2'>" + "SINIFLANDIRMA" + "</font></b></td>" +
            "<td  width='45%' style='border-style: solid; border-width: 1px' align='center' rowspan='2'>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "AÇIKLAMA" + "</font></b></td>" +
            "<td style='border-style: solid; border-width: 1px' colspan='3' align='center'>" +
            "<p class=MsoNormal style='margin-top: 0; margin-bottom: 0'><b><font size='2'>" + "BÜTÇE TEKLİFİ" + "</font></b></td>" +
            "</tr>" +
            "<tr>" +
            "<td width='3.1%' style='border-style: solid; border-width: 1px' align='center'><b>" +
            "I</b></td>" +
            "<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'><b>" +
            "II</b></td>" +
            "<td  width='3.1%' style='border-style: solid; border-width: 1px' align='center'><b>" +
            "III</b></td>" +
            "<td width='3.1%' style='border-style: solid; border-width: 1px' align='center'><b>" +
            "IV</b></td>" +
            "<td width='15%' style='border-style: solid; border-width: 1px' align='center'>" +
            "<b>" + yil.ToString() + "</b></td>" +
            "<td width='15%' style='border-style: solid; border-width: 1px' align='center'>" +
            "<b>" + (yil + 1).ToString() + "</b></td>" +
            "<td width='15%' style='border-style: solid; border-width: 1px' align='center'>" +
            "<b>" + (yil + 2).ToString() + "</b></td>" +
            "</tr>" +

            "<tr>" +
            "<td  style='border-style: solid; border-width: 1px' align='center'>" + ekoDizi[0] + "</td>" +
            "<td  style='border-style: solid; border-width: 1px' align='center'>" + ekoDizi[1] + "</td>" +
            "<td  style='border-style: solid; border-width: 1px' align='center'>" + ekoDizi[2] + "</td>" +
            "<td  style='border-style: solid; border-width: 1px' align='center'>" + ekoDizi[3] + "</td>" +
            "<td  style='border-style: solid; border-width: 1px' align='left'>" +
            tertip.adlar[1] + "</td>" +
            "<td style='border-style: solid; border-width: 1px' align='right'>" +
            odenek1.ToString("#,##0") + "</td>" +
            "<td style='border-style: solid; border-width: 1px' align='right'>" +
            odenek2.ToString("#,##0") + "</td>" +
            "<td style='border-style: solid; border-width: 1px' align='right'>" +
            odenek3.ToString("#,##0") + "</td>" +
            "</tr>" +
            "<tr>" +
            "<td class=MsoNormal height=400 valign='top' style='border-style: solid; border-width: 1px' colspan='17'><b><u>" +
            "</u></b><p>" +
            "<br>" + gerekce.Replace("&nbsp;", " ") + "</td>" +
            "</tr>" +
            "</table>";

            return sayfa;
        }

        /// <summary>
        /// Sunucudan kullanıcının girdiği gelir fişi gerekçesi getirilir.
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü için kullanıcı nesnesi</param>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tertip">Bütün kodları içeren tertip</param>
        /// <returns>Kullanıcının girdiği gerekçe döndürülür.</returns>
        private static string GerekceGetir(Kullanici kullanan, int yil, string tertip)
        {
            Tertip t = new Tertip(yil, ENUMTabloKod.TABGelir);
            t.TertipAta(tertip);
            return servisBHMv2.ButceGelirFisiGerekceYaziGetir(kullanan, t);
        }
    }
}

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;
using TNS.UZY;
using TNS.TMM;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Sayıştay Verileri dosyaları (eski ButceMuhasebe/SayistayVerileri.aspx.cs btnListele_Click ve çağırdığı yordamlar, aynen).
    /// Eski sayfadaki kontrollerin (ddlYil, ddlAy, txtMuhasebe, hdnRaporTur, ddlMizanTur) yerine aynı adlı değer tutucular
    /// kullanılır; böylece yordamların gövdesi eskisiyle aynı kalır. Dosyalar CSV (| ayraçlı) ya da Excel (SAY-*.xltx) olarak
    /// üretilir ve Cevap'a indirilecek yanıt olarak yazılır.
    /// İstemci katmanında yapılan küçük düzeltmeler (iş kuralına dokunulmadı):
    ///  - API tipsiz listelerde tam sayı tutarları Int32 döndürdüğü için "foreach (double ...)" yerine ConvertToDbl kullanılır.
    ///  - Öz Kaynak Değişim ve Bütçeleşen/Gerçekleşen'de indirilen ada geçici dosyanın tam yolu yerine yalnızca adı konur.
    /// </summary>
    public class SayistayVerileriDosyalari
    {
        /// <summary>Eski Ext kontrollerinin yerine geçen değer tutucu (Value / Text).</summary>
        public class Alan
        {
            public Alan(string deger) { Text = deger ?? ""; }
            public string Text { get; set; }
            public object Value { get { return Text; } }
        }

        IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
        ITMMServis servisTMM = TNS.TMM.Arac.Tanimla();

        private readonly Kullanici kullanan;
        private readonly Alan ddlYil, ddlAy, txtMuhasebe, hdnRaporTur, ddlMizanTur;

        /// <summary>Üretilen dosya (indirilecek yanıt); rapor türü bilinmiyorsa null.</summary>
        public IResult Cevap { get; private set; }

        public SayistayVerileriDosyalari(Kullanici kullanan, string raporTur, int yil, int ay, int muhasebe, int mizanTur)
        {
            this.kullanan = kullanan;
            hdnRaporTur = new Alan(raporTur);
            ddlYil = new Alan(yil > 0 ? yil.ToString() : "");
            ddlAy = new Alan(ay > 0 ? ay.ToString() : "");
            txtMuhasebe = new Alan(muhasebe > 0 ? muhasebe.ToString() : "");
            ddlMizanTur = new Alan(mizanTur > 0 ? mizanTur.ToString() : "2");
        }

        /// <summary>
        /// Eski DosyaIslem.SablonAdOl(raporSablonYol, "MUH", ad, false): önce kurumun RaporSablonYol/MUH klasörü, yoksa uygulamanın
        /// RaporSablon/MUH klasörü (geliştirmede SGB.Net, yayında exe klasörü).
        /// </summary>
        private static string SablonYol(string sablonAd)
        {
            string kurum = Path.Combine(Ayar.Oku("RaporSablonYol") ?? "", "MUH", sablonAd);
            if (File.Exists(kurum)) return kurum;
            foreach (string kok in new string[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
            {
                string uygulama = Path.Combine(kok, "RaporSablon", "MUH", sablonAd);
                if (File.Exists(uygulama)) return uygulama;
            }
            return kurum;
        }

        private void GridIcerikDosyayaVer()
        {
            SayistayVeri veri = new SayistayVeri();
            veri.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            veri.ay = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
            veri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            veri.tur = OrtakFonksiyonlar.ConvertToStr(hdnRaporTur.Value);

            ObjectArray veriListe = servisMUH.SayistayVeriListele(kullanan, veri);
            string veriler = "";
            foreach (SayistayVeri sv in veriListe.objeler)
            {
                veriler += sv.veriler;
            }
            veriler = "<VeriListe>" + veriler + "</VeriListe>";

            string muhasebeAdi = servisUZY.UzayDegeriStr(null, "MUHMUHASEBE", veri.muhasebe.ToString(), true, "MUHASEBEAD");

            XmlDocument newXml = new XmlDocument();

            newXml.LoadXml(veriler);
            XmlNode root = newXml.DocumentElement;

            System.Xml.XmlNodeList nodeList = root.SelectNodes("/VeriListe/veri");

            string raporAdi = "";
            //CSV formatında dosya oluştur
            string sablon = "";
            if (veri.tur == "2.2")
            {
                sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{BANKAADI}|{SUBEADI}|{HESAPNO}";
                raporAdi = "BankaHesaplari";
            }
            if (veri.tur == "2.3")
            {
                sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{TARIH}|{BORCTOPLAM}|{ALACAKTOPLAM}|{OLMASIGEREKENTUTAR}|{OLANTUTAR}|{NOKSANTUTAR}|{FAZLATUTAR}|{BASKAN}|{UYE1}|{UYE2}";
                raporAdi = "KasaSayim";
            }
            if (veri.tur == "2.4")
            {
                sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{TARIH}|{BORCTOPLAM}|{ALACAKTOPLAM}|{OLMASIGEREKENTUTAR}|{BORCARTIGITUTAR}|{ALACAKARTIGITUTAR}|{NOKSANTUTAR}|{FAZLATUTAR}|{BASKAN}|{UYE1}|{UYE2}";
                raporAdi = "BankaMevcudu";
            }
            if (veri.tur == "2.5")
            {
                sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{TARIH}|{BORCTOPLAM}|{ALACAKTOPLAM}|{OLMASIGEREKENTUTAR}|{OLANTUTAR}|{NOKSANTUTAR}|{FAZLATUTAR}|{BASKAN}|{UYE1}|{UYE2}";
                raporAdi = "AlinanCekler";
            }
            if (veri.tur == "2.6")
            {
                sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{TARIH}|{DEGERKIMEAIT}|{DEGERCINSI}|{DEGERADET}|{DEGERTUTAR}|{MIKTARADET}|{MIKTARTUTAR}|{NOKSANMIKTARADET}|{NOKSANMIKTARTUTAR}|{FAZLAMIKTARADET}|{FAZLAMIKTARTUTAR}|{BASKAN}|{UYE1}|{UYE2}";
                raporAdi = "MenkulKiymet";
            }
            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            foreach (XmlNode xn in nodeList)
            {
                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(veri.yil.ToString("0000"), "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{MUHASEBEADI}", KontrolStr(muhasebeAdi, "STR", 80));
                satir = satir.Replace("{MUHASEBEKOD}", KontrolStr(veri.muhasebe.ToString(), "STR", 16));

                satir = satir.Replace("{ALACAKARTIGITUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "ALACAKARTIGITUTAR"), "DBL", 18));
                satir = satir.Replace("{ALACAKTOPLAM}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "ALACAKTOPLAM"), "DBL", 18));
                satir = satir.Replace("{BORCARTIGITUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "BORCARTIGITUTAR"), "DBL", 18));
                satir = satir.Replace("{BORCTOPLAM}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "BORCTOPLAM"), "DBL", 18));
                satir = satir.Replace("{DEGERADET}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "DEGERADET"), "DBL", 18));
                satir = satir.Replace("{DEGERTUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "DEGERTUTAR"), "DBL", 18));
                satir = satir.Replace("{FAZLAMIKTARADET}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "FAZLAMIKTARADET"), "DBL", 18));
                satir = satir.Replace("{FAZLAMIKTARTUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "FAZLAMIKTARTUTAR"), "DBL", 18));
                satir = satir.Replace("{FAZLATUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "FAZLATUTAR"), "DBL", 18));
                satir = satir.Replace("{MIKTARADET}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "MIKTARADET"), "DBL", 18));
                satir = satir.Replace("{MIKTARTUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "MIKTARTUTAR"), "DBL", 18));
                satir = satir.Replace("{NOKSANMIKTARADET}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "NOKSANMIKTARADET"), "DBL", 18));
                satir = satir.Replace("{NOKSANMIKTARTUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "NOKSANMIKTARTUTAR"), "DBL", 18));
                satir = satir.Replace("{NOKSANTUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "NOKSANTUTAR"), "DBL", 18));
                satir = satir.Replace("{OLANTUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "OLANTUTAR"), "DBL", 18));
                satir = satir.Replace("{OLMASIGEREKENTUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "OLMASIGEREKENTUTAR"), "DBL", 18));
                satir = satir.Replace("{ALACAKARTIGITUTAR}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "ALACAKARTIGITUTAR"), "DBL", 18));

                satir = satir.Replace("{TARIH}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "TARIH"), "TAR", 250));

                satir = satir.Replace("{DEGERKIMEAIT}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "DEGERKIMEAIT"), "STR", 250));
                satir = satir.Replace("{DEGERCINSI}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "DEGERCINSI"), "STR", 25));
                satir = satir.Replace("{BANKAADI}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "BANKAADI"), "STR", 100));
                satir = satir.Replace("{SUBEADI}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "SUBEADI"), "STR", 100));
                satir = satir.Replace("{HESAPNO}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "HESAPNO"), "STR", 30));
                satir = satir.Replace("{BASKAN}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "BASKAN"), "STR", 250));
                satir = satir.Replace("{UYE1}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "UYE1"), "STR", 250));
                satir = satir.Replace("{UYE2}", KontrolStr(TNS.MUH.Arac.OzellikVer(xn, "UYE2"), "STR", 250));

                donen.AppendLine(satir);
            }

            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);

            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder(raporAdi, sonucDosyaAd, gidenDosyaAdi);
        }

        public void Uret()
        {
            string raporTur = OrtakFonksiyonlar.ConvertToStr(hdnRaporTur.Value);
            // eski: listede olmayan rapor türünde hiçbir şey yapılmazdı; burada Sonuc boş kalır, uç hata döndürür
            if (raporTur == "2.1") MuhasebeBilgiFormu_21();

            else if (raporTur == "2.2") GridIcerikDosyayaVer();
            else if (raporTur == "2.3") GridIcerikDosyayaVer();
            else if (raporTur == "2.4") GridIcerikDosyayaVer();
            else if (raporTur == "2.5") GridIcerikDosyayaVer();
            else if (raporTur == "2.6") GridIcerikDosyayaVer();

            else if (raporTur == "2.7") TasinirKesinHesapCetveli_27();
            else if (raporTur == "2.8") TasinirKesinHesapIcmalCetveli_28();
            else if (raporTur == "2.9") ButceGiderleriTablosu_29();
            else if (raporTur == "2.10") ButceGelirlerininEkoSiniflandirmasi_210();
            else if (raporTur == "2.11") Bilanco_211();
            else if (raporTur == "2.12") FaaliyetSonuclari_212();
            else if (raporTur == "2.13") BirlestirilmisVeriler_213();
            else if (raporTur == "2.14") GiderButcesi_214();
            else if (raporTur == "2.15") GelirButcesi_215();
            else if (raporTur == "2.16") Mizan_216();
            else if (raporTur == "2.23") OzKaynakDegisim();
            else if (raporTur == "2.24") ButcelenenGerceklesenRapor();
        }

        private void OzKaynakDegisim()
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
            string kurumAd = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("OK", "") + ".xlsx";
            XLS.DosyaAc(SablonYol("SAY-OzkaynakDegisim.xltx"), sonucDosyaAd);

            XLS.HucreAdBulYaz("ButceYil", yil);
            XLS.HucreAdBulYaz("KurKod", kurumKod);
            XLS.HucreAdBulYaz("KurAd", kurumAd);
            int basSatir = 0, basSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref basSatir, ref basSutun);
            string[] hesKodlar = { "500.01", "500.02", "500.03", "500.07", "", "500.04/500.07,500.08/500.99", "51", "54", "570", "580", "590, 591" };
            for (int i = 0; i < 11; i++)
            {
                basSatir++;
                if (!string.IsNullOrWhiteSpace(hesKodlar[i]))
                {
                    XLS.HucreDegerYaz(basSatir, basSutun + 0, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIFTUR", TNS.UZY.Arac.UzayKosulOl((yil - 1).ToString(), muhasebe.ToString(), "", hesKodlar[i], "", "", "", "", "6", "", "", "", "", "", "", "", "1"), false, "SUM(H.ALACAK - H.BORC)"));
                    XLS.HucreDegerYaz(basSatir, basSutun + 1, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIFTUR", TNS.UZY.Arac.UzayKosulOl((yil - 1).ToString(), muhasebe.ToString(), "", hesKodlar[i], "", "", "", "", "6", "", "", "", "", "", "", "", "0"), false, "SUM(H.ALACAK - H.BORC)"));
                    XLS.HucreDegerYaz(basSatir, basSutun + 2, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIFTUR", TNS.UZY.Arac.UzayKosulOl((yil - 1).ToString(), muhasebe.ToString(), "", hesKodlar[i], "", "", "", "", "6", "", "", "", "", "", "", "", "9"), false, "SUM(H.BORC - H.ALACAK)"));
                    XLS.HucreDegerYaz(basSatir, basSutun + 3, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIFTUR", TNS.UZY.Arac.UzayKosulOl((yil - 0).ToString(), muhasebe.ToString(), "", hesKodlar[i], "", "", "", "", "6", "", "", "", "", "", "", "", "1"), false, "SUM(H.ALACAK - H.BORC)"));
                    XLS.HucreDegerYaz(basSatir, basSutun + 4, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIFTUR", TNS.UZY.Arac.UzayKosulOl((yil - 0).ToString(), muhasebe.ToString(), "", hesKodlar[i], "", "", "", "", "6", "", "", "", "", "", "", "", "0"), false, "SUM(H.ALACAK - H.BORC)"));
                    XLS.HucreDegerYaz(basSatir, basSutun + 5, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIFTUR", TNS.UZY.Arac.UzayKosulOl((yil - 0).ToString(), muhasebe.ToString(), "", hesKodlar[i], "", "", "", "", "6", "", "", "", "", "", "", "", "9"), false, "SUM(H.BORC - H.ALACAK)"));
                }
            }
            //    }
            //    sat = basSatir + 9 + i;

            //    XLS.HucreDegerYaz(sat, basSutun + 0, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), muhasebe.ToString(), "", "901.01", kurumKod, "", "", ekoKod, "6"), false, "SUM(H.ALACAK-H.BORC)"));
            //    XLS.HucreDegerYaz(sat, basSutun + 1, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), muhasebe.ToString(), "", "901.01/901.25", kurumKod, "", "", ekoKod, "6"), false, "SUM(H.ALACAK-H.BORC)"));

            //    if (i < 9)
            //        XLS.HucreDegerYaz(sat, basSutun + 2, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), muhasebe.ToString(), "", "830." + ekoKod, kurumKod, "", "", "", "6"), false));
            //}

            XLS.DosyaSaklaTamYol();
            // eski: hedef ad olarak geçici dosyanın tam yolu veriliyordu; indirilen ada yalnızca dosya adı konur
            DosyaGonder("OzkaynakDegisim", sonucDosyaAd, Path.GetFileNameWithoutExtension(sonucDosyaAd), "xlsx");
        }

        private void ButcelenenGerceklesenRapor()
        {
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            int muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();

            Tablo XLS = GenelIslemler.NewTablo();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("BG", "") + ".xlsx";
            XLS.DosyaAc(SablonYol("SAY-ButcelenenGerceklesen.xltx"), sonucDosyaAd);

            XLS.HucreAdBulYaz("ButceYil", yil);
            int basSatir = 0, basSutun = 0;
            XLS.HucreAdAdresCoz("BaslaSatir", ref basSatir, ref basSutun);

            for (int i = 1; i <= 9; i++)
            {
                string ekoKod = i.ToString("00");
                int sat = basSatir + i - 1;
                if (i > 6)
                    sat--;
                if (i != 7)
                {
                    string hesapKod;
                    int carpan;
                    if (i < 9)
                    {
                        hesapKod = "800." + ekoKod;
                        carpan = 1;
                    }
                    else
                    {
                        hesapKod = "810";
                        carpan = -1;
                    }

                    XLS.HucreDegerYaz(sat, basSutun + 0, servisUZY.UzayDegeriDbl(kullanan, "ABSGELIR", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), "06.00", "0", kurumKod, ekoKod), false) * carpan);
                    XLS.HucreDegerYaz(sat, basSutun + 2, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), muhasebe.ToString(), "", hesapKod, kurumKod, "", "", "", "6"), false, "SUM(H.ALACAK-H.BORC)") * carpan);
                }
                sat = basSatir + 9 + i;

                XLS.HucreDegerYaz(sat, basSutun + 0, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), muhasebe.ToString(), "", "901.01", kurumKod, "", "", ekoKod, "6"), false, "SUM(H.ALACAK-H.BORC)"));
                XLS.HucreDegerYaz(sat, basSutun + 1, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), muhasebe.ToString(), "", "901.01/901.25", kurumKod, "", "", ekoKod, "6"), false, "SUM(H.ALACAK-H.BORC)"));

                if (i < 9)
                    XLS.HucreDegerYaz(sat, basSutun + 2, servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(yil.ToString(), muhasebe.ToString(), "", "830." + ekoKod, kurumKod, "", "", "", "6"), false));
            }

            XLS.DosyaSaklaTamYol();
            DosyaGonder("ButcelenenGerceklesen", sonucDosyaAd, Path.GetFileNameWithoutExtension(sonucDosyaAd), "xlsx");
        }

        private void MuhasebeBilgiFormu_21()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{YETKILIADI}|{YETKILIUNVAN}|{YETKILIEPOSTA}";

            MuhasebeBilgisi kriter = new MuhasebeBilgisi();
            ObjectArray mler = servisMUH.MuhasebeListele(kriter);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            foreach (MuhasebeBilgisi m in mler.objeler)
            {
                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(ddlYil.Value + "", "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{MUHASEBEADI}", KontrolStr(m.ad, "STR", 80));
                satir = satir.Replace("{MUHASEBEKOD}", KontrolStr(m.kod.ToString(), "STR", 16));
                satir = satir.Replace("{YETKILIADI}", KontrolStr(m.ilgili1, "STR", 250));
                satir = satir.Replace("{YETKILIUNVAN}", KontrolStr(m.ilgili1Unvan, "STR", 100));
                satir = satir.Replace("{YETKILIEPOSTA}", KontrolStr(m.ePosta, "STR", 100));

                donen.AppendLine(satir);
            }

            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);

            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("MuhasebeBilgiFormu", sonucDosyaAd, gidenDosyaAdi);
        }

        private void TasinirKesinHesapCetveli_27()
        {
            //Tablo 1.3 İdare Taşınır Mal Yönetimi Ayrıntılı Hesap Cetveli
            //Tablo 5.7.A.1
            //string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{HESAPKOD1}|{YIL}|{HESAPKOD2}|{HESAPAD}|{OLCUBIRIMADI}|{GECENYILDEVRENMIKTAR}|{GECENYILDEVRENTUTAR}|{YILICINDEGIRENMIKTAR}|{YILICINDEGIRENTUTAR}|{TOPLAMMIKTAR}|{TOPLAMTUTAR}|{YILICINDECIKANMIKTAR}|{YILICINDECIKANTUTAR}|{GELECEKYILADEVREDENMIKTAR}|{GELECEKYILADEVREDENTUTAR}|{TASINIRKONSOLIDEGOREVLISIADI}|{TARIH}";
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{HESAPKOD1}|{HESAPKOD2}|{HESAPAD}|{OLCUBIRIMADI}|{GECENYILDEVRENMIKTAR}|{GECENYILDEVRENTUTAR}|{YILICINDEGIRENMIKTAR}|{YILICINDEGIRENTUTAR}|{TOPLAMMIKTAR}|{TOPLAMTUTAR}|{YILICINDECIKANMIKTAR}|{YILICINDECIKANTUTAR}|{GELECEKYILADEVREDENMIKTAR}|{GELECEKYILADEVREDENTUTAR}|{TASINIRKONSOLIDEGOREVLISIADI}|{TARIH}";

            Cetvel kriter = new Cetvel();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim().PadLeft(5, '0');
            //ObjectArray bilgi = servisTMM.TasinirKesinHesapIcmalCetveli(kullanan, kriter, true);
            ObjectArray bilgi = servisTMM.TasinirKesinHesapCetveli(kullanan, kriter, true);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            Cetvel cetvel = (Cetvel)bilgi.objeler[0];

            string muhasebeKod = txtMuhasebe.Text.Trim().PadLeft(5, '0');
            ObjectArray imza = servisTMM.ImzaListele(kullanan, muhasebeKod, "", "", (int)ENUMImzaYer.TASINIRKONSOLIDEGOREVLISI);
            TNS.TMM.ImzaBilgisi konsolideGorevlisi = new TNS.TMM.ImzaBilgisi();
            foreach (TNS.TMM.ImzaBilgisi ii in imza.objeler)
            {
                if (OrtakFonksiyonlar.ConvertToInt(ii.muhasebe, 0) == OrtakFonksiyonlar.ConvertToInt(muhasebeKod, 0))
                {
                    if (ii.imzaYer == (int)ENUMImzaYer.TASINIRKONSOLIDEGOREVLISI && ii.adSoyad != "" && konsolideGorevlisi.adSoyad == "")
                    {
                        konsolideGorevlisi = ii;
                    }
                }
            }

            for (int i = 0; i < cetvel.detay.Count; i++)
            {
                CetvelDetay detay = (CetvelDetay)cetvel.detay[i];
                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(kriter.yil.ToString() + "", "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{MUHASEBEADI}", KontrolStr(detay.muhasebeAd, "STR", 80));
                satir = satir.Replace("{MUHASEBEKOD}", KontrolStr(detay.muhasebeKod.ToString(), "STR", 16));
                satir = satir.Replace("{YIL}", KontrolStr(kriter.yil.ToString(), "STR", 5));
                satir = satir.Replace("{HESAPKOD1}", KontrolStr(detay.hesapPlanKod.Substring(0, 6), "STR", 8));
                satir = satir.Replace("{HESAPKOD2}", KontrolStr(detay.hesapPlanKod, "STR", 9));
                satir = satir.Replace("{HESAPAD}", KontrolStr(detay.hesapPlanAd, "STR", 80));
                satir = satir.Replace("{OLCUBIRIMADI}", KontrolStr(detay.olcuBirimAd, "STR", 10));
                satir = satir.Replace("{GECENYILDEVRENMIKTAR}", KontrolStr(detay.gecenYildanDevreden.miktar.ToString(), "DBL", 8));
                satir = satir.Replace("{GECENYILDEVRENTUTAR}", KontrolStr(detay.gecenYildanDevreden.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{YILICINDEGIRENMIKTAR}", KontrolStr(detay.yilIcindeGiren.miktar.ToString(), "DBL", 8));
                satir = satir.Replace("{YILICINDEGIRENTUTAR}", KontrolStr(detay.yilIcindeGiren.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{TOPLAMMIKTAR}", KontrolStr(detay.toplam.miktar.ToString(), "DBL", 8));
                satir = satir.Replace("{TOPLAMTUTAR}", KontrolStr(detay.toplam.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{YILICINDECIKANMIKTAR}", KontrolStr(detay.yilIcindeCikan.miktar.ToString(), "DBL", 8));
                satir = satir.Replace("{YILICINDECIKANTUTAR}", KontrolStr(detay.yilIcindeCikan.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{GELECEKYILADEVREDENMIKTAR}", KontrolStr(detay.gelecekYilaDevir.miktar.ToString(), "DBL", 8));
                satir = satir.Replace("{GELECEKYILADEVREDENTUTAR}", KontrolStr(detay.gelecekYilaDevir.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{TASINIRKONSOLIDEGOREVLISIADI}", KontrolStr(konsolideGorevlisi.adSoyad, "STR", 250));
                satir = satir.Replace("{TARIH}", KontrolStr(DateTime.Now.ToShortDateString(), "TAR", 0));

                donen.AppendLine(satir);
            }

            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);

            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("TasinirKesinHesapCetveli_5_7_A1", sonucDosyaAd, gidenDosyaAdi);
        }

        private void TasinirKesinHesapIcmalCetveli_28()
        {
            //Tablo 1.4 İdare Taşınır Mal Yönetim Hesabı İcmal Cetveli
            //string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{HESAPKOD1}|{YIL}|{HESAPKOD2}|{HESAPAD}|{GECENYILDEVRENTUTAR}|{YILICINDEGIRENTUTAR}|{TOPLAMTUTAR}|{YILICINDECIKANTUTAR}|{GELECEKYILADEVREDENTUTAR}|{TASINIRKONSOLIDEGOREVLISIADI}|{KAMUIDARESIUSTYONETICISIADI}|{BAKANADI}|{TARIH}";
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{HESAPKOD1}|{HESAPKOD2}|{HESAPAD}|{GECENYILDEVRENTUTAR}|{YILICINDEGIRENTUTAR}|{TOPLAMTUTAR}|{YILICINDECIKANTUTAR}|{GELECEKYILADEVREDENTUTAR}|{TASINIRKONSOLIDEGOREVLISIADI}|{KAMUIDARESIUSTYONETICISIADI}|{BAKANADI}|{TARIH}";

            Cetvel kriter = new Cetvel();
            kriter.yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            kriter.muhasebeKod = txtMuhasebe.Text.Trim().PadLeft(5, '0');
            ObjectArray bilgi = servisTMM.TasinirKesinHesapIcmalCetveli(kullanan, kriter, true);
            //ObjectArray bilgi = servisTMM.TasinirKesinHesapCetveli(kullanan, kriter, true);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            Cetvel cetvel = (Cetvel)bilgi.objeler[0];

            string muhasebeKod = txtMuhasebe.Text.Trim().PadLeft(5, '0');
            ObjectArray imza = servisTMM.ImzaListele(kullanan, muhasebeKod, "", "", 0);
            TNS.TMM.ImzaBilgisi konsolideGorevlisi = new TNS.TMM.ImzaBilgisi();
            TNS.TMM.ImzaBilgisi ustYonetici = new TNS.TMM.ImzaBilgisi();
            foreach (TNS.TMM.ImzaBilgisi ii in imza.objeler)
            {
                if (OrtakFonksiyonlar.ConvertToInt(ii.muhasebe, 0) == OrtakFonksiyonlar.ConvertToInt(muhasebeKod, 0))
                {
                    if (ii.imzaYer == (int)ENUMImzaYer.TASINIRKONSOLIDEGOREVLISI && ii.adSoyad != "" && konsolideGorevlisi.adSoyad == "")
                    {
                        konsolideGorevlisi = ii;
                    }
                    if (ii.imzaYer == (int)ENUMImzaYer.USTYONETICI && ii.adSoyad != "" && ustYonetici.adSoyad == "")
                    {
                        ustYonetici = ii;
                    }
                }
            }

            for (int i = 0; i < cetvel.detay.Count; i++)
            {
                CetvelDetay detay = (CetvelDetay)cetvel.detay[i];
                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(kriter.yil.ToString() + "", "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{MUHASEBEADI}", KontrolStr(detay.muhasebeAd, "STR", 80));
                satir = satir.Replace("{MUHASEBEKOD}", KontrolStr(detay.muhasebeKod.ToString(), "STR", 16));
                satir = satir.Replace("{YIL}", KontrolStr(kriter.yil.ToString(), "STR", 5));
                satir = satir.Replace("{HESAPKOD1}", KontrolStr(detay.hesapPlanKod.Substring(0, 3), "STR", 8));
                satir = satir.Replace("{HESAPKOD2}", KontrolStr(detay.hesapPlanKod, "STR", 8));
                satir = satir.Replace("{HESAPAD}", KontrolStr(detay.hesapPlanAd, "STR", 80));
                satir = satir.Replace("{GECENYILDEVRENTUTAR}", KontrolStr(detay.gecenYildanDevreden.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{YILICINDEGIRENTUTAR}", KontrolStr(detay.yilIcindeGiren.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{TOPLAMTUTAR}", KontrolStr(detay.toplam.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{YILICINDECIKANTUTAR}", KontrolStr(detay.yilIcindeCikan.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{GELECEKYILADEVREDENTUTAR}", KontrolStr(detay.gelecekYilaDevir.tutar.ToString(), "DBL", 18));
                satir = satir.Replace("{TASINIRKONSOLIDEGOREVLISIADI}", KontrolStr(konsolideGorevlisi.adSoyad, "STR", 250));
                satir = satir.Replace("{KAMUIDARESIUSTYONETICISIADI}", KontrolStr(ustYonetici.adSoyad, "STR", 250));
                satir = satir.Replace("{BAKANADI}", KontrolStr("", "STR", 250));
                satir = satir.Replace("{TARIH}", KontrolStr(DateTime.Now.ToShortDateString(), "TAR", 0));

                donen.AppendLine(satir);
            }

            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);

            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("TasinirKesinHesapCetveli_5_7_A2", sonucDosyaAd, gidenDosyaAdi);
        }

        private void ButceGiderleriTablosu_29()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{KURKOD1}|{KURKOD2}|{KURKOD3}|{KURKOD4}|{KURKOD5}|{YIL}|{FONKOD1}|{FONKOD2}|{FONKOD3}|{FONKOD4}|{FIN1}|{EKOKOD1}|{EKOKOD2}|{EKOADI}|{ODEMEEMRI}|{TENKIS}|{ODEMEEMRIKALAN}|{YILICINDEODEMELER}|{MAHSUPODEMELER}|{TOPLAMODEMELER}|{ODENEKUSTU}|{YETKILIADI}";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            string tarih1 = "01.01." + yil;
            string tarih2 = "31.12." + yil;
            //*******************************************************************
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            oEmri.sorguEkoSiraliMi = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.hesapKod = "901;830;165";
            oEmriDetay.kur = "";// OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);

            ObjectArray mb = servisMUH.KesinHesapGiderTertipListele(kullanan, oEmri);
            //*******************************************************************

            MuhasebeBilgisi kriter = new MuhasebeBilgisi();
            ObjectArray mler = servisMUH.MuhasebeListele(kriter);
            string yetkiliAdi = "";
            foreach (MuhasebeBilgisi m in mler.objeler)
            {
                yetkiliAdi = m.ilgili1;
                break;
            }
            //*******************************************************************

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            string[] kolonDeger = new string[5];
            kolonDeger[0] = "90101;90103;90104;90105;90106;90108";
            kolonDeger[1] = "90109;90111;90113;90115;90117";
            kolonDeger[2] = "90112;90114;90118;90110";
            kolonDeger[3] = "830";
            kolonDeger[4] = "165";

            foreach (string f in mb.objeler)
            {
                string[] kodlar = f.Split('-');

                OdemeEmriMIFForm form = new OdemeEmriMIFForm();
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(oEmri.yil);
                form.yil = oEmri.yil;
                //form.kurum = oEmri.kurum;
                form.muhasebe = oEmri.muhasebe;
                form.formDurum = oEmri.formDurum;
                form.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });

                detay.kur = kodlar[0];
                detay.fon = kodlar[1];
                detay.fin = kodlar[2];
                detay.eko = kodlar[3];
                form.detay.objeler.Add(detay);

                if (string.IsNullOrWhiteSpace(detay.kur) || string.IsNullOrWhiteSpace(detay.fon) || string.IsNullOrWhiteSpace(detay.fin) || string.IsNullOrWhiteSpace(detay.eko))
                    continue;

                ObjectArray tutarlar = servisMUH.KesinHesapGiderRakamVer(kullanan, form, kolonDeger);
                double odemeEmri = 0.00, eklenen = 0.00, tenkis = 0.00, odeme = 0.00, mahsup = 0.00, kolonSay = 0;
                //Gelen değerler alacak-borç olduğu için -1 ile çarpılıyor
                foreach (object degerNesne in tutarlar.objeler)
                {
                    double deger = OrtakFonksiyonlar.ConvertToDbl(degerNesne); // API tam sayı tutarları Int32 olarak döndürür
                    if (kolonSay == 0) odemeEmri = deger;
                    if (kolonSay == 1) eklenen = deger;
                    if (kolonSay == 2) tenkis = deger * -1;
                    if (kolonSay == 3) odeme = deger * -1;
                    if (kolonSay == 4) mahsup = deger * -1;

                    kolonSay++;
                }

                string ekoAdi = "";

                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(oEmri.yil.ToString() + "", "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{KURKOD1}", KontrolStr(detay.kur.Replace(".", "").Substring(0, 2), "STR", 8));
                satir = satir.Replace("{KURKOD2}", KontrolStr(detay.kur.Replace(".", "").Substring(2, 2), "STR", 8));
                satir = satir.Replace("{KURKOD3}", KontrolStr(detay.kur.Replace(".", "").Substring(4, 2), "STR", 8));
                satir = satir.Replace("{KURKOD4}", KontrolStr(detay.kur.Replace(".", "").Substring(6, 2), "STR", 8));
                satir = satir.Replace("{KURKOD5}", KontrolStr("", "STR", 8));
                satir = satir.Replace("{YIL}", KontrolStr(oEmri.yil.ToString(), "STR", 4));
                satir = satir.Replace("{FONKOD1}", KontrolStr(detay.fon.Replace(".", "").Substring(0, 2), "STR", 8));
                satir = satir.Replace("{FONKOD2}", KontrolStr(detay.fon.Replace(".", "").Substring(2, 1), "STR", 8));
                satir = satir.Replace("{FONKOD3}", KontrolStr(detay.fon.Replace(".", "").Substring(3, 1), "STR", 8));
                satir = satir.Replace("{FONKOD4}", KontrolStr(detay.fon.Replace(".", "").Substring(4, 2), "STR", 8));
                satir = satir.Replace("{FIN1}", KontrolStr(detay.fin, "STR", 8));
                satir = satir.Replace("{EKOKOD1}", KontrolStr(detay.eko.Replace(".", "").Substring(0, 2), "STR", 8));
                satir = satir.Replace("{EKOKOD2}", KontrolStr("0" + detay.eko.Replace(".", "").Substring(2, 1), "STR", 8));
                satir = satir.Replace("{EKOADI}", KontrolStr(ekoAdi, "STR", 100));

                satir = satir.Replace("{ODEMEEMRI}", KontrolStr((odemeEmri + eklenen).ToString(), "DBL", 18));
                satir = satir.Replace("{TENKIS}", KontrolStr(tenkis.ToString(), "DBL", 18));
                satir = satir.Replace("{ODEMEEMRIKALAN}", KontrolStr((odemeEmri + eklenen - tenkis).ToString(), "DBL", 18));
                satir = satir.Replace("{YILICINDEODEMELER}", KontrolStr(odeme.ToString(), "DBL", 18));
                satir = satir.Replace("{MAHSUPODEMELER}", KontrolStr(mahsup.ToString(), "DBL", 18));
                satir = satir.Replace("{TOPLAMODEMELER}", KontrolStr((odeme + mahsup).ToString(), "DBL", 18));

                double odenekUstu = (odemeEmri + eklenen - tenkis) - (odeme + mahsup);
                if (odenekUstu > 0) odenekUstu = 0;
                odenekUstu = Math.Abs(odenekUstu);
                satir = satir.Replace("{ODENEKUSTU}", KontrolStr(odenekUstu.ToString(), "DBL", 18));

                satir = satir.Replace("{YETKILIADI}", KontrolStr(yetkiliAdi, "STR", 250));

                donen.AppendLine(satir);
            }
            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);
            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("ButceGiderleriTablosu", sonucDosyaAd, gidenDosyaAdi);
        }

        private void ButceGelirlerininEkoSiniflandirmasi_210()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{HESAPKODU}|{EKOKOD1}|{EKOKOD2}|{EKOKOD3}|{EKOKOD4}|{GELIRTURU}|{RETTURU}|{TUTAR3}|{TUTAR2}|{TUTAR1}";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            string tarih1 = "01.01." + yil;
            string tarih2 = "31.12." + yil;
            //*******************************************************************
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            oEmri.sorguEkoSiraliMi = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.hesapKod = "800;810";
            oEmriDetay.kur = "";// OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);
            int kodTur = (int)ENUMKesinHesapButce.GELIREKONOMIK;

            ObjectArray mb = servisMUH.KesinHesapButce(kullanan, oEmri, kodTur);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                string[] kodlar = f.kod.Split('.');
                if (f.kod.Trim() == "")
                    continue;

                string eko1 = "", eko2 = "", eko3 = "", eko4 = "";
                if (kodlar.Length > 0) eko1 = kodlar[0];
                if (kodlar.Length > 1) eko2 = kodlar[1];
                if (kodlar.Length > 2) eko3 = kodlar[2];
                if (kodlar.Length > 3) eko4 = kodlar[3];

                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{HESAPKODU}", KontrolStr(f.hesapKod, "STR", 5));
                satir = satir.Replace("{EKOKOD1}", KontrolStr(eko1, "STR", 8));
                satir = satir.Replace("{EKOKOD2}", KontrolStr(eko2, "STR", 8));
                satir = satir.Replace("{EKOKOD3}", KontrolStr(eko3, "STR", 8));
                satir = satir.Replace("{EKOKOD4}", KontrolStr(eko4, "STR", 8));

                double carpan = 1.0;
                string gelirTuru = "";
                string retTuru = "";

                if (f.kodAd == "")
                    f.kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod + f.kod, true);

                if (f.hesapKod == "800")
                    gelirTuru = f.kodAd;
                else
                {
                    retTuru = f.kodAd;
                    carpan = -1.0;
                }

                satir = satir.Replace("{GELIRTURU}", KontrolStr(gelirTuru, "STR", 100));
                satir = satir.Replace("{RETTURU}", KontrolStr(retTuru, "STR", 50));

                satir = satir.Replace("{TUTAR3}", KontrolStr((f.deger3 * carpan).ToString(), "DBL", 18));
                satir = satir.Replace("{TUTAR2}", KontrolStr((f.deger2 * carpan).ToString(), "DBL", 18));
                satir = satir.Replace("{TUTAR1}", KontrolStr((f.deger1 * carpan).ToString(), "DBL", 18));

                donen.AppendLine(satir);
            }
            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);
            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("ButceGelirlerininEkoSiniflandirmasi", sonucDosyaAd, gidenDosyaAdi);
        }

        private void Bilanco_211()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{AKTIFPASIF}|{BOLUMKODU}|{GRUPKODU}|{HESAPKODU}|{HESAPADI}|{TUTAR3}|{TUTAR2}|{TUTAR1}";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            string tarih1 = "01.01." + yil;
            string tarih2 = "31.12." + yil;
            //*******************************************************************
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI, ENUMFISTUR.AMORTISMANKAPANISI, ENUMFISTUR.KAPANIS });
            oEmri.sorguEkoSiraliMi = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.hesapKod = "";
            oEmriDetay.kur = "";// OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);
            int kodTur = (int)ENUMKesinHesapButce.BILONCO;

            ObjectArray mb = servisMUH.KesinHesapButce(kullanan, oEmri, kodTur);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            //590 hesapla Eğer kapatma kaydı yapılmamış ise 59* lı hesaplar yoktur. O zaman 630-600 farkından hesaplanır. Olumlu (+) ise 590 olumsuz ise (-) 591
            //Kıbrıs için 630-600 farkı 590'a eklenmeli ki geçen yıl karı/zararı artmış olsun.
            //**************************************************
            double tutar1_59 = 0;
            double tutar2_59 = 0;
            double tutar3_59 = 0;
            double tutar1_590 = 0;
            double tutar2_590 = 0;
            double tutar3_590 = 0;
            double tutar1_591 = 0;
            double tutar2_591 = 0;
            double tutar3_591 = 0;

            //EGO 590 veya 591 fişi yaptıysa (yapmasına gerek yok) türü kapanış olmalı. O zaman hesaplamaya katılmıyor. Melih 01.03.2018

            foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
            {
                if (ff.kod.Replace(".", "").StartsWith("600"))
                {
                    tutar1_59 -= Math.Abs(ff.deger1);
                    tutar2_59 -= Math.Abs(ff.deger2);
                    tutar3_59 -= Math.Abs(ff.deger3);
                }
                if (ff.kod.Replace(".", "").StartsWith("630"))
                {
                    tutar1_59 += Math.Abs(ff.deger1);
                    tutar2_59 += Math.Abs(ff.deger2);
                    tutar3_59 += Math.Abs(ff.deger3);
                }
            }

            if (tutar1_59 < 0) tutar1_590 = tutar1_59;
            if (tutar2_59 < 0) tutar2_590 = tutar2_59;
            if (tutar3_59 < 0) tutar3_590 = tutar3_59;
            if (tutar1_59 > 0) tutar1_591 = tutar1_59;
            if (tutar2_59 > 0) tutar2_591 = tutar2_59;
            if (tutar3_59 > 0) tutar3_591 = tutar3_59;

            bool bulundu590 = false;
            bool bulundu591 = false;

            foreach (TNS.MUH.KesinHesapButce ff in mb.objeler)
            {
                if (ff.kod.Replace(".", "").StartsWith("590"))
                {
                    //Kıbrıs için 630-600 farkı 590'a eklenmeli ki geçen yıl karı artmış olsun.
                    ff.deger1 += tutar1_590;
                    ff.deger2 += tutar2_590;
                    ff.deger3 += tutar3_590;

                    tutar1_590 = ff.deger1;
                    tutar2_590 = ff.deger2;
                    tutar3_590 = ff.deger3;
                    bulundu590 = true;
                }
                if (ff.kod.Replace(".", "").StartsWith("591"))
                {
                    //Kıbrıs için 630-600 farkı 590'a eklenmeli ki geçen yıl zararı artmış olsun.
                    ff.deger1 += tutar1_591;
                    ff.deger2 += tutar2_591;
                    ff.deger3 += tutar3_591;

                    tutar1_591 = ff.deger1;
                    tutar2_591 = ff.deger2;
                    tutar3_591 = ff.deger3;
                    bulundu591 = true;
                }
            }

            if (!bulundu590)
            {
                TNS.MUH.KesinHesapButce ff1 = new TNS.MUH.KesinHesapButce();
                ff1.deger1 = tutar1_590;
                ff1.deger2 = tutar2_590;
                ff1.deger3 = tutar3_590;
                ff1.hesapKod = "590";
                ff1.kod = "590";
                mb.objeler.Add(ff1);
            }

            if (!bulundu591)
            {
                TNS.MUH.KesinHesapButce ff1 = new TNS.MUH.KesinHesapButce();
                ff1.deger1 = tutar1_591;
                ff1.deger2 = tutar2_591;
                ff1.deger3 = tutar3_591;
                ff1.hesapKod = "591";
                ff1.kod = "591";
                mb.objeler.Add(ff1);
            }
            //**************************************************

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                double carpan = 1;
                string aktifPasif = "";
                string bolumKodu = f.hesapKod.Substring(0, 1);
                string grupKodu = f.hesapKod.Substring(0, 2);

                //f.deger1 = Math.Abs(f.deger1);
                //f.deger2 = Math.Abs(f.deger2);
                //f.deger3 = Math.Abs(f.deger3);

                if (f.hesapKod.StartsWith("1") || f.hesapKod.StartsWith("2"))
                {
                    carpan = 1;
                    aktifPasif = "AKTİF";
                }
                else if (f.hesapKod.StartsWith("3") || f.hesapKod.StartsWith("4") || f.hesapKod.StartsWith("5"))
                {
                    carpan = -1;
                    aktifPasif = "PASİF";
                }
                else
                    continue;

                if (f.kodAd == "")
                    f.kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod, true);

                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{AKTIFPASIF}", KontrolStr(aktifPasif, "STR", 8));
                satir = satir.Replace("{BOLUMKODU}", KontrolStr(bolumKodu, "STR", 2));
                satir = satir.Replace("{GRUPKODU}", KontrolStr(grupKodu, "STR", 2));
                satir = satir.Replace("{HESAPKODU}", KontrolStr(f.hesapKod, "STR", 5));
                satir = satir.Replace("{HESAPADI}", KontrolStr(f.kodAd, "STR", 160));

                satir = satir.Replace("{TUTAR3}", KontrolStr((f.deger3 * carpan).ToString(), "DBL", 18));
                satir = satir.Replace("{TUTAR2}", KontrolStr((f.deger2 * carpan).ToString(), "DBL", 18));
                satir = satir.Replace("{TUTAR1}", KontrolStr((f.deger1 * carpan).ToString(), "DBL", 18));

                donen.AppendLine(satir);
            }

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                if (f.kod.StartsWith("9"))
                {
                    if (f.kodAd == "")
                        f.kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod, true);

                    string bolumKodu = f.hesapKod.Substring(0, 1);
                    string grupKodu = f.hesapKod.Substring(0, 2);

                    satir = sablon;
                    satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                    satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                    satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                    satir = satir.Replace("{AKTIFPASIF}", KontrolStr("", "STR", 8));
                    satir = satir.Replace("{BOLUMKODU}", KontrolStr(bolumKodu, "STR", 2));
                    satir = satir.Replace("{GRUPKODU}", KontrolStr(grupKodu, "STR", 2));
                    satir = satir.Replace("{HESAPKODU}", KontrolStr(f.hesapKod, "STR", 5));
                    satir = satir.Replace("{HESAPADI}", KontrolStr(f.kodAd, "STR", 160));

                    satir = satir.Replace("{TUTAR3}", KontrolStr((Math.Abs(f.deger3)).ToString(), "DBL", 18));
                    satir = satir.Replace("{TUTAR2}", KontrolStr((Math.Abs(f.deger2)).ToString(), "DBL", 18));
                    satir = satir.Replace("{TUTAR1}", KontrolStr((Math.Abs(f.deger1)).ToString(), "DBL", 18));

                    donen.AppendLine(satir);
                }
            }

            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);
            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("Bilanco", sonucDosyaAd, gidenDosyaAdi);
        }

        private void FaaliyetSonuclari_212()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{HESAPKODU}|{YARHESAPKODU1}|{YARHESAPKODU2}|{YARHESAPKODU3}|{YARHESAPKODU4}|{GIDERTURU}|{GELIRTURU}|{IADETURU}|{ENFLASYONTUTAR3}|{ENFLASYONTUTAR2}|{ENFLASYONTUTAR1}|{TUTAR3}|{TUTAR2}|{TUTAR1}";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            string tarih1 = "01.01." + yil;
            string tarih2 = "31.12." + yil;
            //*******************************************************************
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI, ENUMFISTUR.AMORTISMANKAPANISI });
            oEmri.sorguEkoSiraliMi = true;

            TNS.MUH.OdemeEmriMIFDetay oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.hesapKod = "600";
            oEmriDetay.kur = "";// OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay.objeler.Add(oEmriDetay);
            int kodTur = (int)ENUMKesinHesapButce.GELIREKONOMIK;

            ObjectArray mb1 = servisMUH.KesinHesapButce(kullanan, oEmri, kodTur);

            oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.hesapKod = "630";
            oEmriDetay.kur = "";// OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay = new ObjectArray();
            oEmri.detay.objeler.Add(oEmriDetay);
            kodTur = (int)ENUMKesinHesapButce.GIDEREKONOMIK;
            ObjectArray mb2 = servisMUH.KesinHesapButce(kullanan, oEmri, kodTur);

            oEmriDetay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmriDetay.hesapKod = "610";
            oEmriDetay.kur = "";// OrtakClass.GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmriDetay.fon = "";
            oEmriDetay.fin = "";
            oEmriDetay.eko = "";
            oEmri.detay = new ObjectArray();
            oEmri.detay.objeler.Add(oEmriDetay);
            kodTur = (int)ENUMKesinHesapButce.GELIREKONOMIK;
            ObjectArray mb3 = servisMUH.KesinHesapButce(kullanan, oEmri, kodTur);

            ObjectArray mb = new ObjectArray();
            foreach (TNS.MUH.KesinHesapButce f in mb1.objeler)
                mb.objeler.Add(f);

            foreach (TNS.MUH.KesinHesapButce f in mb2.objeler)
                mb.objeler.Add(f);

            foreach (TNS.MUH.KesinHesapButce f in mb3.objeler)
                mb.objeler.Add(f);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            //if (GenelIslemlerIstemci.VarsayilanKurumBul() == "cumhur" || GenelIslemlerIstemci.VarsayilanKurumBul().Replace(".", "") == "460604")
            donen = FaaliyetIcerikYazGenelButce(mb, oEmri, satir, sablon, yil, kurumAdi, kurumKod);
            //else
            //    donen = FaaliyetIcerikYazOzelButce(mb, oEmri, satir, sablon, yil, kurumAdi, kurumKod);

            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);
            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("FaaliyetSonuclari", sonucDosyaAd, gidenDosyaAdi);
        }

        private StringBuilder FaaliyetIcerikYazGenelButce(ObjectArray mb, OdemeEmriMIFForm oEmri, string satir, string sablon, int yil, string kurumAdi, string kurumKod)
        {
            StringBuilder donen = new StringBuilder();
            Hashtable islenen = new Hashtable();
            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                string[] kodlar = (f.hesapKod + "." + f.kod).Split('.');
                if (f.kod.Trim() == "")
                    continue;

                string kod = "";
                for (int i = 0; i < 5; i++)
                {
                    if (i + 1 > kodlar.Length) continue;

                    string eko1 = "00", eko2 = "00", eko3 = "00", eko4 = "00", eko5 = "00";
                    if (kodlar.Length > 0 && i >= 0) eko1 = kodlar[0];
                    if (kodlar.Length > 1 && i >= 1) eko2 = kodlar[1];
                    if (kodlar.Length > 2 && i >= 2) eko3 = kodlar[2];
                    if (kodlar.Length > 3 && i >= 3) eko4 = kodlar[3];
                    if (kodlar.Length > 4 && i >= 4) eko5 = kodlar[4];

                    double deger3 = 0;
                    double deger2 = 0;
                    double deger1 = 0;

                    string kodAd = "";
                    kod = kod + kodlar[i];
                    if (OrtakFonksiyonlar.ConvertToLong(islenen[f.hesapKod + kod]) > 0) continue;

                    foreach (TNS.MUH.KesinHesapButce f2 in mb.objeler)
                    {
                        if ((f2.hesapKod + f2.kod).Replace(".", "").StartsWith(kod) && f2.hesapKod == f.hesapKod)
                        {
                            deger3 = deger3 + f2.deger3;
                            deger2 = deger2 + f2.deger2;
                            deger1 = deger1 + f2.deger1;
                        }
                    }

                    if (deger1 + deger2 + deger3 == 0) continue;

                    if (kodAd == "")
                    {
                        kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + kod, true);
                        if (kodAd == "")
                        {
                            kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", (oEmri.yil - 1).ToString() + "-" + kod, true);
                            if (kodAd == "")
                                kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", (oEmri.yil - 2).ToString() + "-" + kod, true);
                        }
                    }

                    string giderAdi = "";
                    string gelirAdi = "";
                    string iadeAdi = "";
                    int carp = 1;
                    if (f.hesapKod == "600") gelirAdi = kodAd;
                    if (f.hesapKod == "630") giderAdi = kodAd;
                    if (f.hesapKod == "810") iadeAdi = kodAd;
                    if (f.hesapKod == "610") { iadeAdi = kodAd; carp = -1; }

                    islenen[f.hesapKod + kod] = 1;

                    satir = sablon;
                    satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                    satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                    satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                    satir = satir.Replace("{HESAPKODU}", KontrolStr(f.hesapKod, "STR", 5));
                    satir = satir.Replace("{YARHESAPKODU1}", KontrolStr(eko2, "STR", 8));
                    satir = satir.Replace("{YARHESAPKODU2}", KontrolStr(eko3, "STR", 8));
                    satir = satir.Replace("{YARHESAPKODU3}", KontrolStr(eko4, "STR", 8));
                    satir = satir.Replace("{YARHESAPKODU4}", KontrolStr(eko5, "STR", 8));

                    satir = satir.Replace("{GIDERTURU}", KontrolStr(giderAdi, "STR", 100));
                    satir = satir.Replace("{GELIRTURU}", KontrolStr(gelirAdi, "STR", 100));
                    satir = satir.Replace("{IADETURU}", KontrolStr(iadeAdi, "STR", 100));

                    satir = satir.Replace("{ENFLASYONTUTAR3}", KontrolStr("0", "DBL", 18));
                    satir = satir.Replace("{ENFLASYONTUTAR2}", KontrolStr("0", "DBL", 18));
                    satir = satir.Replace("{ENFLASYONTUTAR1}", KontrolStr("0", "DBL", 18));

                    satir = satir.Replace("{TUTAR3}", KontrolStr((deger3 * carp).ToString(), "DBL", 18));
                    satir = satir.Replace("{TUTAR2}", KontrolStr((deger2 * carp).ToString(), "DBL", 18));
                    satir = satir.Replace("{TUTAR1}", KontrolStr((deger1 * carp).ToString(), "DBL", 18));

                    satir = satir.Replace("\"", "");
                    donen.AppendLine(satir);
                }

            }

            return donen;
        }

        private StringBuilder FaaliyetIcerikYazOzelButce(ObjectArray mb, TNS.MUH.OdemeEmriMIFForm oEmri, string satir, string sablon, int yil, string kurumAdi, string kurumKod)
        {
            StringBuilder donen = new StringBuilder();

            foreach (TNS.MUH.KesinHesapButce f in mb.objeler)
            {
                string[] kodlar = f.kod.Split('.');
                if (f.kod.Trim() == "")
                    continue;

                string eko1 = "", eko2 = "", eko3 = "", eko4 = "";
                if (kodlar.Length > 0) eko1 = kodlar[0];
                if (kodlar.Length > 1) eko2 = kodlar[1];
                if (kodlar.Length > 2) eko3 = kodlar[2];
                if (kodlar.Length > 3) eko4 = kodlar[3];

                if (f.kodAd == "")
                {
                    f.kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-" + f.hesapKod + f.kod, true);
                    if (f.kodAd == "")
                    {
                        f.kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", (oEmri.yil - 1).ToString() + "-" + f.hesapKod + f.kod, true);
                        if (f.kodAd == "")
                            f.kodAd = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", (oEmri.yil - 2).ToString() + "-" + f.hesapKod + f.kod, true);
                    }
                }

                string giderAdi = "";
                string gelirAdi = "";
                string iadeAdi = "";
                if (f.hesapKod == "600") gelirAdi = f.kodAd;
                if (f.hesapKod == "630") giderAdi = f.kodAd;
                if (f.hesapKod == "810") iadeAdi = f.kodAd;

                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{HESAPKODU}", KontrolStr(f.hesapKod, "STR", 5));
                satir = satir.Replace("{YARHESAPKODU1}", KontrolStr(eko1, "STR", 8));
                satir = satir.Replace("{YARHESAPKODU2}", KontrolStr(eko2, "STR", 8));
                satir = satir.Replace("{YARHESAPKODU3}", KontrolStr(eko3, "STR", 8));
                satir = satir.Replace("{YARHESAPKODU4}", KontrolStr(eko4, "STR", 8));

                satir = satir.Replace("{GIDERTURU}", KontrolStr(giderAdi, "STR", 100));
                satir = satir.Replace("{GELIRTURU}", KontrolStr(gelirAdi, "STR", 100));
                satir = satir.Replace("{IADETURU}", KontrolStr(iadeAdi, "STR", 100));

                satir = satir.Replace("{ENFLASYONTUTAR3}", KontrolStr("0", "DBL", 18));
                satir = satir.Replace("{ENFLASYONTUTAR2}", KontrolStr("0", "DBL", 18));
                satir = satir.Replace("{ENFLASYONTUTAR1}", KontrolStr("0", "DBL", 18));

                satir = satir.Replace("{TUTAR3}", KontrolStr(f.deger3.ToString(), "DBL", 18));
                satir = satir.Replace("{TUTAR2}", KontrolStr(f.deger2.ToString(), "DBL", 18));
                satir = satir.Replace("{TUTAR1}", KontrolStr(f.deger1.ToString(), "DBL", 18));

                satir = satir.Replace("\"", "");

                donen.AppendLine(satir);
            }

            return donen;
        }

        private void BirlestirilmisVeriler_213()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{MUHASEBEADI}|{MUHASEBEKOD}|{YEVTARIH}|{YEVNO}|{HESAPADI}|{HESAPKODU}|{CEKNO}|{ACIKLAMA}|{KURKOD1}|{KURKOD2}|{KURKOD3}|{KURKOD4}|{KURKOD5}|{BIRIMADI}|{FONKOD1}|{FONKOD2}|{FONKOD3}|{FONKOD4}|{FINKOD}|{EKOKOD1}|{EKOKOD2}|{EKOKOD3}|{EKOKOD4}|{BORC}|{ALACAK}|{SATIRACIKLAMA}|{VEZNEALINDI}|{ILGILIADI}|{ILGILINO}|{ILGILIBANKA}|{ILGILIBANKAHESAPNO}|{ILGILIVD}|{IADE1}|{IADE2}|{IADE3}|{TAAHHUTNO}|{ISLEMTIP}";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            int ay = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
            string tarih1 = "01.01." + yil;
            string tarih2 = "01." + ay.ToString("00") + "." + yil;
            TNSDateTime ttarih = new TNSDateTime(tarih2);
            ttarih.AddMonths(1);
            ttarih.AddDays(-1);
            tarih2 = ttarih.ToString();
            //*******************************************************************
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI, ENUMFISTUR.KAPANIS, ENUMFISTUR.SONFIS });
            oEmri.sorguTarihSiraliMi = true;

            ObjectArray mb = servisMUH.OdemeEmriMIFListele(kullanan, oEmri, true, false);


            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(sonucDosyaAd, false, System.Text.Encoding.UTF8);

            foreach (OdemeEmriMIFForm f in mb.objeler)
            {
                foreach (OdemeEmriMIFDetay d in f.detay.objeler)
                {
                    if (d.borc == 0 && d.alacak == 0) continue;

                    string eko1 = BolumAl(d.hesapKod, 1);
                    string eko2 = BolumAl(d.hesapKod, 2);
                    string eko3 = BolumAl(d.hesapKod, 3);
                    string eko4 = BolumAl(d.hesapKod, 4);

                    if (f.ilgiliNo.Length < 10 || f.ilgiliNo.Length > 11)
                        f.ilgiliNo = "";

                    string ilgiliBankaNo = "";// f.ilgiliBankaNo;
                    //if (!OrtakFonksiyonlar.IBANKontrol(ilgiliBankaNo, true).islemSonuc)
                    //    ilgiliBankaNo = "";

                    satir = sablon;
                    satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                    satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                    satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                    satir = satir.Replace("{MUHASEBEADI}", KontrolStr(f.muhasebeAd, "STR", 80));
                    satir = satir.Replace("{MUHASEBEKOD}", KontrolStr(f.muhasebe.ToString(), "STR", 16));
                    satir = satir.Replace("{YEVTARIH}", KontrolStr(f.YEVYevTarih.ToString(), "TAR", 0));
                    satir = satir.Replace("{YEVNO}", KontrolStr(f.YEVYevNo.ToString(), "STR", 25));
                    satir = satir.Replace("{HESAPADI}", KontrolStr(d.hesapAd, "STR", 160));
                    satir = satir.Replace("{HESAPKODU}", KontrolStr(BolumAl(d.hesapKod, 0), "STR", 5));
                    satir = satir.Replace("{CEKNO}", KontrolStr(f.YEVCekNo, "STR", 160));
                    satir = satir.Replace("{ACIKLAMA}", KontrolStr(f.aciklama, "STR", 8000));
                    satir = satir.Replace("{KURKOD1}", KontrolStr(BolumAl(d.kur, 0), "STR", 8));
                    satir = satir.Replace("{KURKOD2}", KontrolStr(BolumAl(d.kur, 1), "STR", 8));
                    satir = satir.Replace("{KURKOD3}", KontrolStr(BolumAl(d.kur, 2), "STR", 8));
                    satir = satir.Replace("{KURKOD4}", KontrolStr(BolumAl(d.kur, 3), "STR", 8));
                    satir = satir.Replace("{KURKOD5}", KontrolStr(f.birimAd, "STR", 8));
                    satir = satir.Replace("{BIRIMADI}", KontrolStr(f.birim, "STR", 80));
                    satir = satir.Replace("{FONKOD1}", KontrolStr(BolumAl(d.fon, 0), "STR", 8));
                    satir = satir.Replace("{FONKOD2}", KontrolStr(BolumAl(d.fon, 1), "STR", 8));
                    satir = satir.Replace("{FONKOD3}", KontrolStr(BolumAl(d.fon, 2), "STR", 8));
                    satir = satir.Replace("{FONKOD4}", KontrolStr(BolumAl(d.fon, 3), "STR", 8));
                    satir = satir.Replace("{FINKOD}", KontrolStr(BolumAl(d.fin, 0), "STR", 8));
                    satir = satir.Replace("{EKOKOD1}", KontrolStr(eko1, "STR", 8));
                    satir = satir.Replace("{EKOKOD2}", KontrolStr(eko2, "STR", 8));
                    satir = satir.Replace("{EKOKOD3}", KontrolStr(eko3, "STR", 8));
                    satir = satir.Replace("{EKOKOD4}", KontrolStr(eko4, "STR", 8));
                    satir = satir.Replace("{BORC}", KontrolStr(d.borc.ToString(), "DBL", 18));
                    satir = satir.Replace("{ALACAK}", KontrolStr(d.alacak.ToString(), "DBL", 18));
                    satir = satir.Replace("{SATIRACIKLAMA}", KontrolStr(d.satirAciklama, "STR", 8000));
                    satir = satir.Replace("{VEZNEALINDI}", KontrolStr("", "STR", 16));
                    satir = satir.Replace("{ILGILIADI}", KontrolStr(f.ilgiliAd, "STR", 250));
                    satir = satir.Replace("{ILGILINO}", KontrolStr(f.ilgiliNo, "STR", 11));
                    satir = satir.Replace("{ILGILIBANKA}", KontrolStr(f.ilgiliBankaAd, "STR", 128));
                    satir = satir.Replace("{ILGILIBANKAHESAPNO}", KontrolStr(ilgiliBankaNo, "STR", 64));
                    satir = satir.Replace("{ILGILIVD}", KontrolStr(f.ilgiliVD, "STR", 128));
                    satir = satir.Replace("{IADE1}", KontrolStr("", "STR", 250));
                    satir = satir.Replace("{IADE2}", KontrolStr("", "STR", 10));
                    satir = satir.Replace("{IADE3}", KontrolStr("", "STR", 16));
                    satir = satir.Replace("{TAAHHUTNO}", KontrolStr(f.projeNo, "STR", 16));
                    satir = satir.Replace("{ISLEMTIP}", KontrolStr(BelgeTurdenSayistayBelgeTure(f.fisTur[0]), "STR", 1));

                    satir = satir.Replace("\"", "");
                    satir = satir.Replace("\t", " ");
                    satir = satir.Replace("\r", " ");
                    satir = satir.Replace("\n", " ");
                    satir = satir.Replace("'", " ");

                    sw.WriteLine(satir);
                }
            }

            sw.Flush();
            sw.Close();

            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value + "_" + ddlAy.Value;
            DosyaGonder("BirlestirilmisVeriler", sonucDosyaAd, gidenDosyaAdi);
        }

        private void GiderButcesi_214()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{KURKOD1}|{KURKOD2}|{KURKOD3}|{KURKOD4}|{KURKOD5}|{YIL}|{FONKOD1}|{FONKOD2}|{FONKOD3}|{FONKOD4}|{FINKOD}|{EKOKOD1}|{EKOKOD2}|{ODENEK}";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            string tarih1 = "01.01." + yil;
            string tarih2 = "31.12." + yil;
            //*******************************************************************
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            oEmri.sorguEkoSiraliMi = true;

            ObjectArray mb = servisMUH.KesinHesapGiderTertipListele(kullanan, oEmri);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            TNS.BKD.Tertip tertip = new TNS.BKD.Tertip(DateTime.Now.Year, TNS.BKD.ENUMTabloKod.TABButce);
            string[] kolonDeger = new string[6];
            kolonDeger[0] = "90103;90104;90105;90106;90108";
            kolonDeger[1] = "90101";
            kolonDeger[2] = "90115";
            kolonDeger[3] = "90109;90111;90113;90117";
            kolonDeger[4] = "90112;90114;90118";
            kolonDeger[5] = "";

            foreach (string f in mb.objeler)
            {
                tertip.TertipAta(f);
                tertip.Noktala();

                bool tamMi = tertip.KodTamMi(0);
                if (!tamMi) continue;
                tamMi = tertip.KodTamMi(1);
                if (!tamMi) continue;

                string tertipStr = tertip.TertipStrOl();
                string[] kirilimlar = tertipStr.Split('-');

                string ff = tertipStr.Replace("-", ".");
                //string[] kodlar = ff.Split('.');

                oEmri.detay = new ObjectArray();
                OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(oEmri.yil);
                detay.kur = kirilimlar[0];
                detay.fon = kirilimlar[1];
                detay.fin = kirilimlar[2];
                detay.eko = kirilimlar[3];
                oEmri.detay.objeler.Add(detay);
                ObjectArray rk = servisMUH.KesinHesapGiderRakamVer(kullanan, oEmri, kolonDeger);

                double netOdenek = 0;
                double dusulen = 0;
                int kolonSay = 0;
                foreach (object degerNesne in rk.objeler)
                {
                    double deger = OrtakFonksiyonlar.ConvertToDbl(degerNesne); // API tam sayı tutarları Int32 olarak döndürür
                    if (kolonSay == 0 || kolonSay == 1 || kolonSay == 2 || kolonSay == 3)
                        netOdenek += deger;
                    if (kolonSay == 4)
                        dusulen += OrtakFonksiyonlar.ConvertToDbl(Math.Abs(deger));
                    kolonSay++;
                }
                netOdenek = netOdenek - dusulen;

                satir = sablon;
                satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                satir = satir.Replace("{KURKOD1}", KontrolStr(BolumAl(ff, 0), "STR", 8));
                satir = satir.Replace("{KURKOD2}", KontrolStr(BolumAl(ff, 1), "STR", 8));
                satir = satir.Replace("{KURKOD3}", KontrolStr(BolumAl(ff, 2), "STR", 8));
                satir = satir.Replace("{KURKOD4}", KontrolStr(BolumAl(ff, 3), "STR", 8));
                satir = satir.Replace("{KURKOD5}", KontrolStr("", "STR", 8));
                satir = satir.Replace("{YIL}", KontrolStr(oEmri.yil.ToString(), "STR", 4));
                satir = satir.Replace("{FONKOD1}", KontrolStr(BolumAl(ff, 4), "STR", 8));
                satir = satir.Replace("{FONKOD2}", KontrolStr(BolumAl(ff, 5), "STR", 8));
                satir = satir.Replace("{FONKOD3}", KontrolStr(BolumAl(ff, 6), "STR", 8));
                satir = satir.Replace("{FONKOD4}", KontrolStr(BolumAl(ff, 7), "STR", 8));
                satir = satir.Replace("{FINKOD}", KontrolStr(BolumAl(ff, 8), "STR", 8));
                satir = satir.Replace("{EKOKOD1}", KontrolStr(BolumAl(ff, 9), "STR", 8));
                satir = satir.Replace("{EKOKOD2}", KontrolStr("0" + BolumAl(ff, 10), "STR", 8));
                satir = satir.Replace("{ODENEK}", KontrolStr(netOdenek.ToString(), "DBL", 18));

                donen.AppendLine(satir);
            }
            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);
            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("GiderButcesi", sonucDosyaAd, gidenDosyaAdi);
        }

        private void GelirButcesi_215()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{EKOKOD1}|{EKOKOD2}|{EKOKOD3}|{EKOKOD4}|{GELIRTURU}|{GELIRTAHMINI}|{IADETURU}|{IADETAHMINI}";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            string tarih1 = "01.01." + yil;
            string tarih2 = "31.12." + yil;
            //*******************************************************************
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            oEmri.sorguEkoSiraliMi = true;

            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(oEmri.yil);
            oEmri.detay = new ObjectArray();
            oEmri.detay.objeler.Add(detay);
            ObjectArray mb = servisMUH.KesinHesapGelirTertipListele(kullanan, oEmri);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            foreach (string f in mb.objeler)
            {
                string[] kodlar = f.Split('.');

                if (kodlar.Length != 4)
                    continue;

                if (kodlar[0] == "09")//Ret ve iadeler gösterilmeyecek
                    continue;

                string eko = kodlar[0] + kodlar[1] + kodlar[2] + kodlar[3];

                oEmri.detay = new ObjectArray();
                detay = new OdemeEmriMIFDetay(oEmri.yil);
                detay.kur = "";
                detay.fon = "";
                detay.fin = "";
                detay.eko = eko;
                oEmri.detay.objeler.Add(detay);
                ObjectArray rk = servisMUH.KesinHesapGelirRakamVer(kullanan, oEmri);
                double gelir = OrtakFonksiyonlar.ConvertToDbl(rk.objeler[3]);
                double iade = OrtakFonksiyonlar.ConvertToDbl(rk.objeler[5]);

                if (gelir > 0)
                {
                    satir = sablon;
                    string ekoAd800 = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-800" + eko, true);

                    satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                    satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                    satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                    satir = satir.Replace("{EKOKOD1}", KontrolStr(kodlar[0], "STR", 8));
                    satir = satir.Replace("{EKOKOD2}", KontrolStr(kodlar[1], "STR", 8));
                    satir = satir.Replace("{EKOKOD3}", KontrolStr(kodlar[2], "STR", 8));
                    satir = satir.Replace("{EKOKOD4}", KontrolStr(kodlar[3], "STR", 8));
                    satir = satir.Replace("{GELIRTURU}", KontrolStr(ekoAd800, "STR", 100));
                    satir = satir.Replace("{GELIRTAHMINI}", KontrolStr(gelir.ToString(), "DBL", 18));
                    satir = satir.Replace("{IADETURU}", KontrolStr("", "STR", 50));
                    satir = satir.Replace("{IADETAHMINI}", KontrolStr("0", "DBL", 18));

                    donen.AppendLine(satir);
                }
                if (iade > 0)
                {
                    satir = sablon;
                    string ekoAd810 = servisUZY.UzayDegeriStr(kullanan, "MUHHESAPPLANI", oEmri.yil.ToString() + "-810" + eko, true);

                    satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
                    satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
                    satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
                    satir = satir.Replace("{EKOKOD1}", KontrolStr(kodlar[0], "STR", 8));
                    satir = satir.Replace("{EKOKOD2}", KontrolStr(kodlar[1], "STR", 8));
                    satir = satir.Replace("{EKOKOD3}", KontrolStr(kodlar[2], "STR", 8));
                    satir = satir.Replace("{EKOKOD4}", KontrolStr(kodlar[3], "STR", 8));
                    satir = satir.Replace("{GELIRTURU}", KontrolStr("", "STR", 100));
                    satir = satir.Replace("{GELIRTAHMINI}", KontrolStr("0", "DBL", 18));
                    satir = satir.Replace("{IADETURU}", KontrolStr(ekoAd810, "STR", 50));
                    satir = satir.Replace("{IADETAHMINI}", KontrolStr(iade.ToString(), "DBL", 18));

                    donen.AppendLine(satir);
                }
            }
            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);
            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value;
            DosyaGonder("GelirButcesi", sonucDosyaAd, gidenDosyaAdi);
        }

        private void Mizan_216()
        {
            string sablon = "{MALIYIL}|{KURUMADI}|{KURUMKOD}|{TARIH}|{HESAPKODU}|{EKOKOD1}|{EKOKOD2}|{EKOKOD3}|{EKOKOD4}|{HESAPADI}|{BORC}|{ALACAK}|{BORCKALANI}|{ALACAKKALANI}|{DUZENLEYEN}|{YETKILIYRDADI}|{YETKILIADI}";
            int yil = OrtakFonksiyonlar.ConvertToInt(ddlYil.Value, 0);
            int ay = OrtakFonksiyonlar.ConvertToInt(ddlAy.Value, 0);
            string tarih1 = "01.01." + yil;
            string tarih2 = "01." + ay.ToString("00") + "." + yil;
            TNSDateTime ttarih = new TNSDateTime(tarih2);
            ttarih.AddMonths(1);
            ttarih.AddDays(-1);
            tarih2 = ttarih.ToString();
            int mizanTur = OrtakFonksiyonlar.ConvertToInt(ddlMizanTur.Value, 0);
            //*******************************************************************
            TNS.MUH.OdemeEmriMIFForm oEmri = new OdemeEmriMIFForm();
            oEmri.yil = yil;
            //oEmri.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            oEmri.muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            oEmri.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            oEmri.sorguYEVYevTarih = tarih1.ToString() + ";" + tarih2.ToString();
            if (mizanTur == 2)
                oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            if (mizanTur == 4)
                oEmri.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI, ENUMFISTUR.AMORTISMANKAPANISI, ENUMFISTUR.KAPANIS });
            oEmri.sorguEkoSiraliMi = true;

            MizanBilgi mb = servisMUH.Mizan(kullanan, oEmri.yil, oEmri.kurum, oEmri.muhasebe, "", new TNSDateTime(tarih1), new TNSDateTime(tarih2), 5, "100", "999z", 1, oEmri.fisTur, 0, "", true);

            StringBuilder donen = new StringBuilder();
            string satir = sablon;

            string kurumKod = KurumKodGetir();
            string kurumAdi = GenelIslemlerIstemci.VarsayilanKurumBulAd();

            foreach (AnaHesap ana in mb.anaHesaplar.objeler)
            {
                HesapBilgi hb = new HesapBilgi();
                hb.alacak = ana.alacakToplam;
                hb.borc = ana.borcToplam;
                hb.hesapAd = ana.hesapAd;
                hb.hesapKod = ana.hesapKod;
                hb.kalanAlacak = ana.kalanAlacakToplam;
                hb.kalanBorc = ana.kalanBorcToplam;

                string satirIc = "";

                //satirIc = MizanIcerik(sablon, yil, kurumAdi, kurumKod, hb);
                //donen.AppendLine(satirIc);

                foreach (HesapBilgi ah in ana.detayHesaplar.objeler)
                {
                    //if (ah.hesapKod == ana.hesapKod) continue;

                    satirIc = MizanIcerik(sablon, yil, kurumAdi, kurumKod, ah);
                    donen.AppendLine(satirIc);
                }
            }
            string sonucDosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
            File.WriteAllText(sonucDosyaAd, donen.ToString(), Encoding.UTF8);
            string gidenDosyaAdi = ("Tablo_" + hdnRaporTur.Value).Replace(".", "_") + "_" + ddlYil.Value + "_" + ddlAy.Value;
            DosyaGonder("Mizan", sonucDosyaAd, gidenDosyaAdi);
        }

        private string MizanIcerik(string sablon, int yil, string kurumAdi, string kurumKod, HesapBilgi hb)
        {
            string satir = sablon;
            satir = satir.Replace("{MALIYIL}", KontrolStr(yil.ToString() + "", "STR", 4));
            satir = satir.Replace("{KURUMADI}", KontrolStr(kurumAdi, "STR", 160));
            satir = satir.Replace("{KURUMKOD}", KontrolStr(kurumKod, "STR", 16));
            satir = satir.Replace("{TARIH}", KontrolStr(DateTime.Now.ToShortDateString(), "TAR", 10));
            satir = satir.Replace("{HESAPKODU}", KontrolStr(BolumAl(hb.hesapKod, 0), "STR", 5));
            satir = satir.Replace("{EKOKOD1}", KontrolStr(BolumAl(hb.hesapKod, 1), "STR", 8));
            satir = satir.Replace("{EKOKOD2}", KontrolStr(BolumAl(hb.hesapKod, 2), "STR", 8));
            satir = satir.Replace("{EKOKOD3}", KontrolStr(BolumAl(hb.hesapKod, 3), "STR", 8));
            satir = satir.Replace("{EKOKOD4}", KontrolStr(BolumAl(hb.hesapKod, 4), "STR", 8));
            satir = satir.Replace("{HESAPADI}", KontrolStr(hb.hesapAd, "STR", 160));
            satir = satir.Replace("{BORC}", KontrolStr(hb.borc.ToString(), "DBL", 18));
            satir = satir.Replace("{ALACAK}", KontrolStr(hb.alacak.ToString(), "DBL", 18));
            satir = satir.Replace("{BORCKALANI}", KontrolStr(hb.kalanBorc.ToString(), "DBL", 18));
            satir = satir.Replace("{ALACAKKALANI}", KontrolStr(hb.kalanAlacak.ToString(), "DBL", 18));
            satir = satir.Replace("{DUZENLEYEN}", KontrolStr("", "STR", 250));
            satir = satir.Replace("{YETKILIYRDADI}", KontrolStr("", "STR", 250));
            satir = satir.Replace("{YETKILIADI}", KontrolStr("", "STR", 250));

            return satir;
        }


        private string BolumAl(string kod, int bolNo)
        {
            return TNS.MUH.Arac.DiziElemanAl(kod.Split('.'), bolNo, "");
        }

        string KontrolStr(string icerik, string tip, int uzunluk)
        {
            if (tip == "STR")
            {
                icerik = icerik.Trim();
                icerik = icerik.Replace("\r", " ").Replace("\n", " ");

                if (!string.IsNullOrWhiteSpace(icerik) && icerik.Length > uzunluk) icerik = icerik.Substring(0, uzunluk);
            }
            else if (tip == "DBL")
            {
                icerik = icerik.Trim();
                double deger = OrtakFonksiyonlar.ConvertToDbl(icerik);
                icerik = Math.Round(deger, 2).ToString();
                icerik = icerik.Replace(".", ",");
            }
            else if (tip == "TAR")
            {
                icerik = icerik.Trim();
                TNSDateTime tarih = new TNSDateTime(icerik);
                if (!tarih.isNull)
                {
                    string gun = tarih.Gun.ToString("00");
                    string ay = tarih.Ay.ToString("00");
                    string yil = tarih.Yil.ToString("0000");
                    icerik = gun + "." + ay + "." + yil;
                    if (icerik.Length > 10) icerik = icerik.Substring(0, 10);
                }
            }

            return icerik;
        }

        string KurumKodGetir()
        {
            string icerik = GenelIslemlerIstemci.VarsayilanKurumBul();

            icerik = icerik.Replace(".", "");
            if (icerik.Length < 8)
                icerik = icerik.PadRight(8, '0');

            icerik = OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(DateTime.Now.Year, 1, false), icerik);

            if (icerik == "40.35.00.00") icerik = "40.35.00.01";//Özelleştirme Fon Muhasebesi için
            int muhasebe = OrtakFonksiyonlar.ConvertToInt(txtMuhasebe.Text.Trim(), 0);
            if (muhasebe == 220)
                icerik = "90.09.01.00";//Dijital dönüşüm ofisi
            else if (muhasebe == 240)
                icerik = "90.05.01.00";//Yatırım ofisi ofisi
            else if (icerik == "01.79.00.00") icerik = "40";//Milli Saraylar

            return icerik;
        }

        void DosyaGonder(string raporAdi, string kaynakDosya, string hedefDosya, string format = "csv")
        {
            hedefDosya = raporAdi + "_" + hedefDosya;

            Cevap = DosyaIslem.DosyaGonder(kaynakDosya, hedefDosya, true, format);
        }

        private string BelgeTurdenSayistayBelgeTure(ENUMFISTUR fisTur)
        {
            switch (fisTur)
            {
                case ENUMFISTUR.ACILIS: return "1";
                case ENUMFISTUR.DONEMICI: return "2";
                case ENUMFISTUR.KAPANIS: return "3";
                case ENUMFISTUR.AMORTISMANKAPANISI: return "3";
                case ENUMFISTUR.SONFIS: return "4";

            }
            return "2";
        }
    }
}

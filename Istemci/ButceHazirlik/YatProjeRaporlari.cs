using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words;
using OrtakClass;
using TNS;
using TNS.KYM;

namespace ButceHazirlikv2
{
    /// <summary>
    /// Yatırım Projeleri ekranlarının raporları (eski Rapor.xml 2002 YatirimRaporlari, 2004 ProjeDosyasiRaporlari /
    /// ProjeIzlemeDegerlendirme). Eski kod aynen kullanılır; burada yalnızca eski RaporlamaServis.TekRaporAl yerine doğrudan
    /// çağrılır ve üretilen dosyanın yolu döndürülür.
    /// </summary>
    public static class YatProjeRaporlari
    {
        private class RaporTanim
        {
            public int grup;
            public int no;
            public string sablon;
            public string sinif;
            public string ekBilgi;

            public RaporTanim(int grup, int no, string sablon, string sinif, string ekBilgi)
            {
                this.grup = grup;
                this.no = no;
                this.sablon = sablon;
                this.sinif = sinif;
                this.ekBilgi = ekBilgi;
            }
        }

        // Rapor.xml'deki tanımlar (GRUPNO, NO, SABLON, NESNE, EKBILGI). sinif: "YAT" = YatirimRaporlari,
        // "PD" = ProjeDosyasiRaporlari, "PID" = ProjeIzlemeDegerlendirme.
        private static readonly RaporTanim[] Tanimlar =
        {
            new RaporTanim(2002, 1, "YAT/YAT-Tablo1.xlt", "YAT", "1"),
            new RaporTanim(2002, 2, "YAT/YAT-Tablo2.xlt", "YAT", "2"),
            new RaporTanim(2002, 13, "YAT/YAT-Tablo2.xlt", "YAT", "13"),
            new RaporTanim(2002, 17, "YAT/YAT-Tablo2.xlt", "YAT", "2-o"),
            new RaporTanim(2002, 3, "YAT/YAT-Tablo2.xlt", "YAT", "2-1"),
            new RaporTanim(2002, 14, "YAT/YatStrPlan.xltx", "YAT", "14"),
            new RaporTanim(2002, 4, "YAT/YAT-Tablo3.xlt", "YAT", "3"),
            new RaporTanim(2002, 5, "YAT/YAT-Tablo4.xlt", "YAT", "4"),
            new RaporTanim(2002, 6, "YAT/YAT-Tablo5.xlt", "YAT", "5"),
            new RaporTanim(2002, 15, "YAT/Yat-Dis-Kredi.xltx", "YAT", "15"),
            new RaporTanim(2002, 8, "YAT/YAT-Tablo7.xlt", "YAT", "7"),
            new RaporTanim(2002, 9, "YAT/YAT-Tablo10.xlt", "YAT", "10"),
            new RaporTanim(2002, 10, "YAT/YAT-Tablo12-13.xlt", "YAT", "12_13-1"),
            new RaporTanim(2002, 11, "YAT/YAT-Tablo12-13.xlt", "YAT", "12_13-2"),
            new RaporTanim(2002, 12, "YAT/YAT-Tablo12-13.xlt", "YAT", "12_13-3"),
            new RaporTanim(2002, 16, "YAT/YatUygulamaRaporu.xltx", "YAT", "16"),
            new RaporTanim(2002, 18, "YAT/ProjeBilgiFormu.dotx", "YAT", "ProjeBilgiFormu"),
            new RaporTanim(2002, 19, "YAT/ProjeBilgiListe.xltx", "YAT", "ProjeBilgiListe"),
            new RaporTanim(2002, 20, "YAT/YAT-Tablo2 (Yeni).xltx", "YAT", "Yeni2"),

            new RaporTanim(2004, 1, "YAT/YATD-Kapak.dotx", "PD", "Kapak"),
            new RaporTanim(2004, 2, "YAT/YATD-ProjeOzeti.dotx", "PD", "Ozet"),
            new RaporTanim(2004, 3, "YAT/YATD-ProjeTutari.dotx", "PD", "Tutar"),
            new RaporTanim(2004, 4, "YAT/YATD-OdenekHarcama.dotx", "PD", "Harc"),
            new RaporTanim(2004, 5, "YAT/YATD-KrediHibeGiris.dotx", "PD", "KrediHibe1"),
            new RaporTanim(2004, 6, "YAT/YATD-KrediHibeGiris.dotx", "PD", "KrediHibe2"),
            new RaporTanim(2004, 7, "YAT/YATD-YatirimTeklif.dotx", "PD", "Teklif0"),
            new RaporTanim(2004, 8, "YAT/YATD-YatirimTeklif.dotx", "PD", "Teklif1"),
            new RaporTanim(2004, 9, "YAT/YATD-YatirimTeklif.dotx", "PD", "Teklif2"),
            new RaporTanim(2004, 10, "YAT/YATD-HizmetGerekcesi.dotx", "PD", "Bilgi"),
            new RaporTanim(2004, 11, "YAT/YATD-YatirimTeklifTam.dotx", "PD", "TeklifTam"),
            new RaporTanim(2004, 12, "YAT/YATD-KrediHibe.dotx", "PD", "KrediHibe"),
            new RaporTanim(2004, 13, "YAT/YATD-ProjeDetayProgram.dotx", "PD", "DetayP"),
            new RaporTanim(2004, 14, "YAT/YID-RaporBos.dotx", "PID", "ProjeDurum"),
            new RaporTanim(2004, 15, "YAT/YID-Rapor.dotx", "PID", "ProjeIzleme"),
            new RaporTanim(2004, 16, "YAT/YID-RaporBos.dotx", "PID", "ProjeDurumBirim"),
            new RaporTanim(2004, 17, "YAT/YID-SektorlereGore.dotx", "PID", "SektoreGore"),
            new RaporTanim(2004, 18, "YAT/YID-EkoKodaGore.dotx", "PID", "EkoKodaGore"),
            new RaporTanim(2004, 19, "YAT/YID-SektorEkoKodaGore.dotx", "PID", "SektorEkoKodaGore"),
        };

        // Rapor.xml KISTAS'taki ZORUNLU="1" alanlar (eski KistasEkran.ZorunlulukKontrol)
        private static readonly string[] Zorunlu2002 = { "ButceYil", "Senaryo", "KurKod" };
        private static readonly string[] Zorunlu2004 = { "ButceYil", "Senaryo", "ProjeNo" };

        // Eski AltProjeDetay.MenuEkle sırası ve OrtakClassRes REP_2002_n metinleri (kaynakta yoksa)
        private static readonly string[][] Menu2002 =
        {
            new[] { "2002-1", "Tablo 1 - Yatırım Teklifleri Özet Tablosu" },
            new[] { "2002-2", "Tablo 2 - Yatırım Projeleri Tablosu" },
            new[] { "2002-13", "Tablo 2 - Yatırım Projeleri Tablosu (Tam)" },
            new[] { "2002-14", "Tablo 4 - Yatırım Tekliflerinin St. Plan ve Per. Prog. ile İlişkisi" },
            new[] { "2002-4", "Tablo 5 - Yatırım Ödenek ve Harcamaları" },
            new[] { "2002-5", "Tablo 6 - İdame ve Yenileme Yatırımları" },
            new[] { "2002-6", "Tablo 7 - Proje İzleme Formu" },
            new[] { "2002-15", "Tablo 8 - Halen İşlemde Olan Dış Proje Kredileri" },
            new[] { "2002-8", "Tablo 9 - Dış Kredi Sağlanması İstenen Projeler" },
            new[] { "2002-9", "Tablo 12 - Kamulaştırma Harcamalarının Proje Bazında Dağılımı" },
        };

        /// <summary>"Yat. Raporları" menüsü (eski AltProjeDetay.MenuEkle): { kod = "2002-n", ad }.</summary>
        public static List<object> YatRaporListesi()
        {
            List<object> liste = new List<object>();
            foreach (string[] m in Menu2002)
                liste.Add(new { kod = m[0], ad = Istemci.Metin.Ortak("REP_" + m[0].Replace("-", "_"), m[1]) });
            return liste;
        }

        public static bool YatRaporTanimli(string kod)
        {
            foreach (string[] m in Menu2002)
                if (m[0] == kod) return true;
            return false;
        }

        /// <summary>Eski AltProjeDetay.RaporAl (raporKod "2002-n"; projeNo / altProjeNo yoksa -1).</summary>
        public static string YatRaporAl(Kullanici kullanan, string raporKod, int yil, string senaryo, string kurKod, int projeNo, int altProjeNo, string raporSablonYol)
        {
            // Eski kod "grup-no" biçimini çözüp bozuksa "Lütfen rapor seçiniz" diyordu (BHMBRD049).
            RaporTanim t = null;
            string[] parca = (raporKod ?? "").Split('-');
            if (parca.Length == 2)
                t = Bul(OrtakFonksiyonlar.ConvertToInt(parca[0], -1), OrtakFonksiyonlar.ConvertToInt(parca[1], -1));
            if (t == null || t.grup != 2002)
                throw new RaporHatasi("Lütfen rapor seçiniz");

            if (string.IsNullOrEmpty(kurKod))
                kurKod = YatRaporAraclari.VarsayilanKurumBul();

            Hashtable raporParam = new Hashtable();
            raporParam.Add("ButceYil", yil.ToString());
            raporParam.Add("Senaryo", senaryo);
            raporParam.Add("KurKod", kurKod);

            raporParam.Add("projeNo", projeNo.ToString());
            raporParam.Add("altProjeNo", altProjeNo.ToString());

            TNS.BHMv2.IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            raporParam.Add("SenaryoAd", servis.ButceSenaryoAdBul(kullanan.aktifKultur, senaryo));
            raporParam.Add("KurAd", GenelIslemler.KodAd(1, yil + "-" + kurKod, true));

            return Uret(t, kullanan, servis, raporParam, raporSablonYol);
        }

        /// <summary>Eski ProjeDetay.ProjeDosyasiYazdir: raporKod "ment" (bütün proje dosyası) ya da "men1".."men11"; uzanti docx / pdf.</summary>
        public static string ProjeDosyasiAl(Kullanici kullanan, string raporKod, int yil, string senaryo, int projeNo, int altProjeNo, string raporSablonYol, out string uzanti)
        {
            Hashtable ht = new Hashtable();
            ht.Add("ButceYil", yil.ToString());
            ht.Add("Senaryo", senaryo);
            ht.Add("SenaryoAd", "-");
            ht.Add("ProjeNo", projeNo.ToString());
            ht.Add("AltProjeNo", altProjeNo.ToString());
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI))
                uzanti = "docx";
            else
                uzanti = "pdf";
            if (raporKod == "ment")
            {
                // Bütün proje dosyası: 2004-1..11 Word olarak üretilip tek belgede birleştirilir.
                ht.Add("Format", "docx");
                Document doc = null;
                DocumentBuilder builder = null;
                for (int i = 1; i <= 11; i++)
                {
                    string dosyaAd = Rapor2004(kullanan, i, ht, raporSablonYol);
                    if (i == 1)
                    {
                        doc = new Document(dosyaAd);
                        builder = new DocumentBuilder(doc);
                    }
                    else
                    {
                        builder.MoveToDocumentEnd();
                        if (i == 2)
                        {
                            Document srcDoc = new Document(dosyaAd);
                            doc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);
                            doc.Sections[0].PageSetup.SectionStart = SectionStart.NewPage;
                            doc.Sections[1].PageSetup.RestartPageNumbering = true;
                            doc.Sections[1].PageSetup.PageStartingNumber = 1;
                        }
                        else
                            GenelIslemlerIstemci.AsposeBuilderaDocEkle(builder, dosyaAd);
                    }
                    try
                    {
                        System.IO.File.Delete(dosyaAd);
                    }
                    catch { }
                }

                string sonucDosyaAd = DosyaIslem.DosyaAdUret() + "." + uzanti;
                doc.Save(sonucDosyaAd);
                return sonucDosyaAd;
            }
            ht.Add("Format", uzanti);

            // "men1".."men11" -> 2004-n; tanınmayan kod eskisi gibi 2004-1 (kapak)
            int raporNo = 1;
            if (raporKod != null && raporKod.StartsWith("men"))
            {
                int n = OrtakFonksiyonlar.ConvertToInt(raporKod.Substring(3), 1);
                if (n >= 1 && n <= 11) raporNo = n;
            }
            return Rapor2004(kullanan, raporNo, ht, raporSablonYol);
        }

        /// <summary>Rapor.xml 2004-n (eski TekRaporAl(2004, n, ...)); dönen dosyanın uzantısı dosya adındadır.</summary>
        public static string Rapor2004(Kullanici kullanan, int raporNo, Hashtable raporParam, string raporSablonYol)
        {
            return Rapor(2004, raporNo, kullanan, null, raporParam, raporSablonYol);
        }

        /// <summary>Rapor.xml 2002-n (eski TekRaporAl(2002, n, ...)); dönen dosyanın uzantısı dosya adındadır.</summary>
        public static string Rapor2002(Kullanici kullanan, int raporNo, Hashtable raporParam, string raporSablonYol)
        {
            return Rapor(2002, raporNo, kullanan, null, raporParam, raporSablonYol);
        }

        /// <summary>
        /// Rapor sınıflarının kendi içinden çağırdığı eski RaporlamaServis.TekRaporUret (ör. proje dosyasında kredi / hibe ve
        /// detay programı, izleme raporunda proje sayfaları). Şablon kökü eskisi gibi ayarlardaki RaporSablonYol'dur.
        /// </summary>
        internal static string TekRaporUret(int grup, int raporNo, Kullanici kullanan, object servis, Hashtable raporParam)
        {
            return Rapor(grup, raporNo, kullanan, servis, raporParam, Istemci.Ayar.Oku("RaporSablonYol"));
        }

        private static string Rapor(int grup, int raporNo, Kullanici kullanan, object servis, Hashtable raporParam, string raporSablonYol)
        {
            RaporTanim t = Bul(grup, raporNo);
            if (t == null) throw new RaporHatasi(grup + " rapor bulunamadı");
            return Uret(t, kullanan, servis, raporParam ?? new Hashtable(), raporSablonYol);
        }

        private static RaporTanim Bul(int grup, int no)
        {
            foreach (RaporTanim t in Tanimlar)
                if (t.grup == grup && t.no == no) return t;
            return null;
        }

        /// <summary>
        /// Eski RaporGrup.RaporAl + Rapor.MetodRaporHazirla: zorunlu alanları denetler, raporu üretir ve dosya yolunu döndürür.
        /// (Eski kurum / aşama yetki denetimi bu gruplar için hep true dönüyordu; yetki sayfa ucunda denetlenir.)
        /// </summary>
        private static string Uret(RaporTanim t, Kullanici kullanan, object servis, Hashtable raporParam, string raporSablonYol)
        {
            string hata = "";
            foreach (string alan in t.grup == 2002 ? Zorunlu2002 : Zorunlu2004)
            {
                if (raporParam[alan] == null || raporParam[alan].ToString() == "")
                {
                    if (hata != "") hata += " ";
                    hata += string.Format("'{0}' alanını girmelisiniz!", Istemci.Metin.Ortak("REP_A_" + t.grup + "_" + alan, alan));
                }
            }
            if (hata != "") throw new RaporHatasi(hata);

            string sablonAd = (raporSablonYol ?? "").TrimEnd('/', '\\') + "/" + t.sablon;
            string sonucDosyaAd = DosyaIslem.DosyaAdUret();

            string sonuc;
            if (t.sinif == "YAT")
                sonuc = new YatirimRaporlari().RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, null, raporParam, t.ekBilgi);
            else if (t.sinif == "PD")
                sonuc = new ProjeDosyasiRaporlari().RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, null, raporParam, t.ekBilgi);
            else
                sonuc = new ProjeIzlemeDegerlendirme().RaporAl(kullanan, servis, sablonAd, sonucDosyaAd, null, raporParam, t.ekBilgi);

            // Eskisi "HATA..." / "BILGI..." dönen metni kullanıcıya gösteriyor, boş dönerse dosya göndermiyordu.
            if (string.IsNullOrEmpty(sonuc))
                throw new RaporHatasi("Rapor oluşturulamadı.");
            if (sonuc.StartsWith("HATA") || sonuc.StartsWith("BILGI") || sonuc.StartsWith("BİLGİ"))
                throw new RaporHatasi(sonuc);
            return sonuc;
        }
    }
}

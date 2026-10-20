using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Muavin Defteri kıstasları (eski MuavinDefteri.aspx PropertyGrid'i pgFiltre). Alan adları eski PropertyGridParameter
    /// adlarıdır; değerler sorgudan metin olarak okunur (tarihler yyyy-MM-dd). Eski sayfada muhasebede döviz yoksa "listeTur"
    /// alanı kaldırılırdı (pgFiltre.Source["listeTur"] == null); ListeTurVar bunu karşılar.
    /// </summary>
    public class MuavinDefteriKriter
    {
        public static readonly string[] Alanlar =
        {
            "yil", "durum", "belgeTur", "listeTur", "hesapNo", "althesapNo", "muhasebe", "birim", "yevmiyeNo1", "yevmiyeNo2",
            "belgeNo1", "belgeNo2", "belgeTarihi1", "belgeTarihi2", "ilgili", "ilgiliNo", "kur", "fon", "fin", "eko", "peb",
            "yevmiyeTarihi1", "yevmiyeTarihi2", "durumTarihi1", "durumTarihi2", "borc", "alacak", "fisTur", "islemYapan",
            "aciklama", "cekNoSorgu", "OdEsTur", "OdEsNo", "OdEsTarihi", "yatirimProjeNo"
        };

        private readonly Dictionary<string, string> degerler = new Dictionary<string, string>(StringComparer.Ordinal);

        /// <summary>Eski: muhasebede döviz yoksa Liste Tür alanı yoktu (TNS.MUH.Arac.MuhasebeDovizVarMi).</summary>
        public bool ListeTurVar { get; private set; }

        public string this[string ad]
        {
            get { return degerler.TryGetValue(ad, out string v) ? v : ""; }
        }

        public static MuavinDefteriKriter Oku(IQueryCollection q)
        {
            MuavinDefteriKriter k = new MuavinDefteriKriter();
            foreach (string ad in Alanlar)
                k.degerler[ad] = ((string)q[ad] ?? "").Trim();
            k.ListeTurVar = TNS.MUH.Arac.MuhasebeDovizVarMi();
            return k;
        }

        /// <summary>Eski listeTur okuması: boşsa (ya da alan yoksa) "1" Türk Lirası, "2" Döviz.</summary>
        public string ListeTur()
        {
            string listeTur = "";
            if (ListeTurVar)
                listeTur = OrtakFonksiyonlar.ConvertToStr(this["listeTur"] + "");
            if (listeTur == "") listeTur = "1";
            return listeTur;
        }
    }

    /// <summary>Sonuç tablosunun satırı (eski StoreListe alanları; adlar aynen). Tarihler yyyy-MM-dd, boşsa "".</summary>
    public class MuavinSatir
    {
        public string yil { get; set; } = "";
        public string belgeNo { get; set; } = "";
        public string yevmiyeNo { get; set; } = "";
        public string belgeTur { get; set; } = "";
        public string belgeTarih { get; set; } = "";
        public string muhasebe { get; set; } = "";
        public string birim { get; set; } = "";
        public double? borc { get; set; }
        public double? alacak { get; set; }
        public string bakiye { get; set; } = "";
        public string ilgili { get; set; } = "";
        public string yevmiyeTarih { get; set; } = "";
        public string durum { get; set; } = "";
        public string durumTarih { get; set; } = "";
        public string islemYapan { get; set; } = "";
        public string OEBNo { get; set; } = "";
        public string OEBTarih { get; set; } = "";
        public string cekNo { get; set; } = "";
        public string cekTarih { get; set; } = "";
        public string surecNo { get; set; } = "";
        public string aciklama { get; set; } = "";
        public string tertip { get; set; } = "";
        public string hesapKod { get; set; } = "";
        public string dovizKurTur { get; set; } = "";
        public decimal? dovizKur { get; set; }
        public double? dovizBorc { get; set; }
        public double? dovizAlacak { get; set; }
        public string dovizBakiye { get; set; } = "";
        public string siraNo { get; set; } = "";
        /// <summary>Tarih öncesi toplam (eski "DEVİR" satırı).</summary>
        public bool devir { get; set; }
    }

    /// <summary>
    /// Muavin Defteri iş kodu: eski ButceMuhasebe/MuavinDefteri.aspx.cs'nin Sorgula, btnListe_Click, btnListeYazdir_Click,
    /// btnListeYazdir2_Click, SayfaToplamYaz, UstToplamYaz, TarihOncesiToplamGetir, HTDoubleEkle, DurumDonustur ve
    /// HesapPlaniAdGetir rutinleri aynen. Farkları: kıstaslar pgFiltre yerine MuavinDefteriKriter'den okunur, kullanıcı
    /// parametreden gelir, eski MesajKutusu iletileri Uyarilar listesine yazılır (uç bunları istemciye döndürür), dosya burada
    /// gönderilmez (üretilen dosyanın yolu döner).
    /// </summary>
    public class MuavinDefteriIslem
    {
        public const string SablonTL = "MUH-MuavinDefteri.xlt";
        public const string SablonDoviz = "MUH-MuavinDefteriDoviz.xlt";
        public const string SablonOzet = "MUH-MuavinDefteri2.xlt";

        private readonly IMUHServis servisMUH = TNS.MUH.Arac.Tanimla();
        private readonly Kullanici kullanan;
        private readonly MuavinDefteriKriter pgFiltre;
        private readonly string uygulamaKlasoru;

        /// <summary>Eski GenelIslemler.MesajKutusu ile gösterilen iletiler (sırasıyla).</summary>
        public List<string> Uyarilar { get; } = new List<string>();

        public MuavinDefteriIslem(Kullanici kullanan, MuavinDefteriKriter kriter, string uygulamaKlasoru)
        {
            this.kullanan = kullanan;
            this.pgFiltre = kriter;
            this.uygulamaKlasoru = uygulamaKlasoru;
        }

        private void MesajKutusu(string metin)
        {
            if (!string.IsNullOrEmpty(metin) && !Uyarilar.Contains(metin)) Uyarilar.Add(metin);
        }

        /// <summary>Şablon: önce kurumun RaporSablonYol/MUH klasörü, yoksa uygulamanın RaporSablon/MUH klasörü.</summary>
        public static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol") ?? "";
            if (kurumYol != "")
            {
                string s = kurumYol + "/MUH/" + sablonAd;
                if (File.Exists(s)) return s;
            }
            if (!string.IsNullOrEmpty(uygulamaKlasoru))
            {
                string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "MUH", sablonAd);
                if (File.Exists(u)) return u;
            }
            return null;
        }

        private static string TarihYaz(TNSDateTime t)
        {
            return t == null || t.isNull ? "" : t.Oku().ToString("yyyy-MM-dd");
        }

        private static string BakiyeStr(double bakiye)
        {
            string bakiyeStr = Math.Abs(bakiye).ToString("#,###.00");
            if (bakiye < 0)
                bakiyeStr += "(A)";
            else
                bakiyeStr += "(B)";
            return bakiyeStr;
        }

        // ------------------------------------------------------------------ sorgu (eski Sorgula)

        public ObjectArray Sorgula(string hesapKod)
        {
            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
            f.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre["yil"], 0);
            f.formDurum = OrtakFonksiyonlar.ConvertToInt(pgFiltre["durum"], 0);

            string hesapNo = pgFiltre["hesapNo"];
            hesapNo = hesapNo.Split('-')[0].Trim();

            if (string.IsNullOrEmpty(hesapNo))
            {
                MesajKutusu("Hesap Kod alanı boş bırakılamaz.");
                return null;
            }

            f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            f.muhasebe = OrtakFonksiyonlar.ConvertToInt(pgFiltre["muhasebe"], 0);
            f.birim = pgFiltre["birim"];

            if (pgFiltre["belgeTur"] != "")
                f.sorguBelgeTur = OrtakFonksiyonlar.ConvertToInt(pgFiltre["belgeTur"], -1);

            string b1 = pgFiltre["belgeNo1"];
            string b2 = pgFiltre["belgeNo2"];
            if (b1 != "" || b2 != "")
            {
                if (b1 != "" && b1.Length < 9)
                    b1 = f.kurum.Replace(".", "").PadRight(4, '0') + b1.PadLeft(6, '0');
                if (b2 != "" && b2.Length < 9)
                    b2 = f.kurum.Replace(".", "").PadRight(4, '0') + b2.PadLeft(6, '0');

                f.belgeNo = b1 + ";" + b2;
            }

            if (pgFiltre["yevmiyeNo1"] != "" || pgFiltre["yevmiyeNo2"] != "")
                f.sorguYEVYevNo = pgFiltre["yevmiyeNo1"] + ";" + pgFiltre["yevmiyeNo2"];

            if (pgFiltre["yevmiyeTarihi1"] != "")
                f.sorguYEVYevTarih = new TNSDateTime(pgFiltre["yevmiyeTarihi1"]).ToString() + ";";
            if (pgFiltre["yevmiyeTarihi2"] != "")
                f.sorguYEVYevTarih += new TNSDateTime(pgFiltre["yevmiyeTarihi2"]).ToString();

            if (pgFiltre["belgeTarihi1"] != "")
                f.sorguIslemTarih = new TNSDateTime(pgFiltre["belgeTarihi1"]).ToString() + ";";
            if (pgFiltre["belgeTarihi2"] != "")
                f.sorguIslemTarih += new TNSDateTime(pgFiltre["belgeTarihi2"]).ToString();

            if (pgFiltre["durumTarihi1"] != "")
                f.sorguFormDurumTarih = new TNSDateTime(pgFiltre["durumTarihi1"]).ToString() + ";";
            if (pgFiltre["durumTarihi2"] != "")
                f.sorguFormDurumTarih += new TNSDateTime(pgFiltre["durumTarihi2"]).ToString();

            f.ilgiliAd = pgFiltre["ilgili"];
            f.ilgiliNo = pgFiltre["ilgiliNo"];
            f.islemYapan = pgFiltre["islemYapan"];
            f.projeNo = pgFiltre["yatirimProjeNo"];
            f.aciklama = pgFiltre["aciklama"];

            f.OEBTur = pgFiltre["OdEsTur"];
            f.OEBNo = pgFiltre["OdEsNo"];
            if (pgFiltre["OdEsTarihi"].Trim() != "")
                f.OEBTarih = new TNSDateTime(pgFiltre["OdEsTarihi"].Trim());

            f.YEVCekNo = pgFiltre["cekNoSorgu"];
            f.fisTurYap(IntDiziYap(pgFiltre["fisTur"]));
            if (f.fisTur == null)
            {
                f.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });
            }

            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(f.yil);
            detay.hesapKod = pgFiltre["hesapNo"];
            detay.hesapKod = detay.hesapKod.Split('-')[0].Trim();

            detay.altHesapKod = pgFiltre["althesapNo"];
            detay.kur = pgFiltre["kur"];
            detay.fon = pgFiltre["fon"];
            detay.fin = pgFiltre["fin"];
            detay.eko = pgFiltre["eko"];
            detay.peb = pgFiltre["peb"];

            detay.borc = OrtakFonksiyonlar.ConvertToDbl(pgFiltre["borc"]);
            detay.alacak = OrtakFonksiyonlar.ConvertToDbl(pgFiltre["alacak"]);
            f.detay.objeler.Add(detay);

            f.sorguHesapKodSiraliMi = true;
            return servisMUH.OdemeEmriMIFListele(kullanan, f, true);
        }

        /// <summary>Eski MizanTertip.IntDiziYap (ButceMuhasebe/MizanBilanco.cs).</summary>
        public static ENUMFISTUR[] IntDiziYap(string p)
        {
            string[] dizi = (p ?? "").Split(',');
            List<ENUMFISTUR> list = new List<ENUMFISTUR>();
            foreach (string item in dizi)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    try
                    {
                        ENUMFISTUR tut = (ENUMFISTUR)Convert.ToInt32(item);
                        list.Add(tut);
                    }
                    catch { }
                }
            }
            return list.ToArray();
        }

        // ------------------------------------------------------------------ liste (eski btnListe_Click)

        /// <summary>
        /// Satırlar (hesap koduna göre sıralı; her hesap kodunun başında, belge tarihi başlangıcı verilmişse tarih öncesi toplam
        /// "DEVİR" satırı). Hata olursa hata doldurulur, null döner.
        /// </summary>
        public List<MuavinSatir> Listele(out string hata)
        {
            hata = null;
            ObjectArray fler = Sorgula("");
            if (fler == null)
            {
                hata = Uyarilar.Count > 0 ? Uyarilar[0] : "Sorgu yapılamadı.";
                Uyarilar.Clear();
                return null;
            }

            string durum = "";
            string belgeTuru = "";
            List<MuavinSatir> dt = new List<MuavinSatir>();

            if (fler.sonuc.islemSonuc)
            {
                Hashtable bakiyeHash = new Hashtable();
                Hashtable dovizBakiyeHash = new Hashtable();
                string hesapKodTemp = string.Empty;

                double bakiye = 0.0;

                foreach (OdemeEmriMIFForm ff in fler.objeler)
                {
                    foreach (OdemeEmriMIFDetay detay in ff.detay.objeler)
                    {
                        string hesapKod = detay.hesapKod;
                        if (hesapKod != hesapKodTemp)
                        {
                            hesapKodTemp = detay.hesapKod;
                            OdemeEmriMIFDetay oncekiToplam = TarihOncesiToplamGetir(hesapKodTemp);

                            if (oncekiToplam != null)
                            {
                                MuavinSatir rowDevir = new MuavinSatir();
                                rowDevir.devir = true;
                                rowDevir.hesapKod = oncekiToplam.hesapKod;
                                rowDevir.aciklama = "DEVİR";
                                rowDevir.borc = oncekiToplam.borc;
                                rowDevir.alacak = oncekiToplam.alacak;

                                bakiye = HTDoubleEkle(bakiyeHash, oncekiToplam.hesapKod, (oncekiToplam.borc - oncekiToplam.alacak));
                                rowDevir.bakiye = BakiyeStr(bakiye);

                                rowDevir.dovizKurTur = oncekiToplam.dovizKurTur;
                                rowDevir.dovizKur = oncekiToplam.dovizKur;
                                rowDevir.dovizBorc = oncekiToplam.dovizBorc;
                                rowDevir.dovizAlacak = oncekiToplam.dovizAlacak;

                                bakiye = HTDoubleEkle(dovizBakiyeHash, oncekiToplam.hesapKod, (oncekiToplam.dovizBorc - oncekiToplam.dovizAlacak));
                                rowDevir.dovizBakiye = BakiyeStr(bakiye);
                                dt.Add(rowDevir);
                            }
                        }

                        MuavinSatir row = new MuavinSatir();

                        row.hesapKod = detay.hesapKod;

                        durum = DurumDonustur(ff.formDurum);
                        if (ff.belgeTur == ENUMBelgeTuru.ODEMEEMRI)
                            belgeTuru = "ÖDE";
                        else
                            belgeTuru = "MİF";
                        row.yil = ff.yil.ToString();
                        row.belgeNo = ff.belgeNo ?? "";

                        row.belgeTarih = TarihYaz(ff.islemTarih);

                        row.belgeTur = belgeTuru;
                        row.muhasebe = ff.muhasebe.ToString().PadLeft(5, '0') + " - " + ff.muhasebeAd;
                        row.birim = ff.birim + " - " + ff.birimAd;
                        row.ilgili = ff.ilgiliAd ?? "";

                        row.yevmiyeTarih = TarihYaz(ff.YEVYevTarih);
                        row.OEBTarih = TarihYaz(ff.OEBTarih);

                        row.durum = durum;

                        row.durumTarih = TarihYaz(ff.formDurumTarih);

                        row.islemYapan = ff.islemYapan ?? "";
                        row.OEBNo = ff.OEBNo ?? "";
                        row.surecNo = ff.surecYil + "/" + ff.surecNo;

                        row.cekNo = ff.YEVCekNo ?? "";
                        row.cekTarih = TarihYaz(ff.YEVCekTarih);

                        row.aciklama = detay.satirAciklama ?? "";
                        row.yevmiyeNo = ff.YEVYevNo.ToString();

                        row.tertip = TNS.MUH.Arac.TertipStrOl(detay);
                        row.borc = detay.borc;
                        row.alacak = detay.alacak;

                        bakiye = HTDoubleEkle(bakiyeHash, detay.hesapKod, (detay.borc - detay.alacak));
                        row.bakiye = BakiyeStr(bakiye);

                        row.dovizKurTur = detay.dovizKurTur ?? "";
                        row.dovizKur = detay.dovizKur;
                        row.dovizBorc = detay.dovizBorc;
                        row.dovizAlacak = detay.dovizAlacak;

                        bakiye = HTDoubleEkle(dovizBakiyeHash, detay.hesapKod, (detay.dovizBorc - detay.dovizAlacak));
                        row.dovizBakiye = BakiyeStr(bakiye);

                        row.siraNo = detay.satirNo.ToString();

                        dt.Add(row);
                    }
                }

                if (dt.Count == 0)
                    MesajKutusu("Verilen kriterlere uygun kayıt bulunamadı.");
                return dt;
            }
            else
            {
                hata = string.IsNullOrWhiteSpace(fler.sonuc.hataStr) ? "Sorgu yapılamadı." : fler.sonuc.hataStr;
                return null;
            }
        }

        private double HTDoubleEkle(Hashtable ht, string htKod, double ekleDeger)
        {
            object tut = ht[htKod];
            double tutD;
            if (tut != null)
                tutD = (double)tut;
            else
                tutD = 0;

            tutD += ekleDeger;
            ht[htKod] = tutD;

            return tutD;
        }

        // ------------------------------------------------------------------ Yazdır (eski btnListeYazdir_Click)

        /// <summary>Muavin defteri (Türk Lirası: MUH-MuavinDefteri.xlt, Döviz: MUH-MuavinDefteriDoviz.xlt). Üretilen dosyanın yolu; hata olursa null.</summary>
        public string Yazdir(out string hata)
        {
            hata = null;
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;

            int satirD = 0;
            int sutunD = 0;

            string sablonAd = string.Empty;

            string listeTur = pgFiltre.ListeTur();
            if (listeTur == "1")
                sablonAd = SablonTL;
            else if (listeTur == "2")
                sablonAd = SablonDoviz;

            string sablon = SablonBul(uygulamaKlasoru, sablonAd);
            if (sablon == null)
            {
                hata = "Rapor şablonu bulunamadı: MUH/" + sablonAd;
                return null;
            }

            // Kayıtlar önce okunur: hesap kodu boşsa ya da sorgu hata verirse dosya üretilmez (eski kod hata verip kalıyordu).
            ObjectArray fler = Sorgula("");
            if (fler == null)
            {
                hata = Uyarilar.Count > 0 ? Uyarilar[0] : "Sorgu yapılamadı.";
                return null;
            }
            if (!fler.sonuc.islemSonuc)
            {
                hata = string.IsNullOrWhiteSpace(fler.sonuc.hataStr) ? "Sorgu yapılamadı." : fler.sonuc.hataStr;
                return null;
            }

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("MuavinDefteri_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablon, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satirD, ref sutunD);
            satir = satirD + 1;

            string yil = pgFiltre["yil"];
            string hesapKod = pgFiltre["hesapNo"];

            string donem = string.Empty;
            if (pgFiltre["belgeTarihi1"] != "")
                donem += new TNSDateTime(pgFiltre["belgeTarihi1"]).ToString() + " - ";
            else
                donem += "01.01." + DateTime.Now.Year.ToString() + " - ";

            if (pgFiltre["belgeTarihi2"] != "")
                donem += new TNSDateTime(pgFiltre["belgeTarihi2"]).ToString();
            else
                donem += DateTime.Now.ToShortDateString();

            XLS.HucreAdBulYaz("Donem", donem);
            XLS.HucreAdBulYaz("HesapKod", hesapKod);

            string hesapAd = string.Empty;
            HesapPlaniSatir h = new HesapPlaniSatir();
            h.yil = OrtakFonksiyonlar.ConvertToInt(yil, DateTime.Now.Year);
            h.hesapKodHesapAd = hesapKod;
            h.detay = false;
            ObjectArray hesap = servisMUH.HesapPlaniListele(kullanan, h);
            if (hesap.ObjeSayisi > 0)
                hesapAd = ((HesapPlaniSatir)hesap.objeler[0]).aciklama;

            XLS.HucreAdBulYaz("HesapAd", hesapAd);

            double bakiye = 0.0;
            double borcToplam = 0.0;
            double alacakToplam = 0.0;

            UstToplamYaz(XLS, ref satir, ref borcToplam, ref alacakToplam);

            int toplamSayfaSatirSayisi = 0;
            if (listeTur == "1")
            {
                toplamSayfaSatirSayisi = 70;
            }
            else if (listeTur == "2")
            {
                toplamSayfaSatirSayisi = 85;
            }

            int sayfaNumarasi = 1;

            bakiye = borcToplam - alacakToplam;

            foreach (OdemeEmriMIFForm ff in fler.objeler)
            {
                foreach (OdemeEmriMIFDetay detay in ff.detay.objeler)
                {
                    XLS.HucreKopyala(satirD, 0, satirD, 10, satir, 0);

                    XLS.HucreDegerYaz(satir, 0, ff.islemTarih.ToString());
                    XLS.HucreDegerYaz(satir, 1, ff.belgeNo);
                    XLS.HucreDegerYaz(satir, 2, ff.YEVYevNo);
                    XLS.HucreDegerYaz(satir, 3, detay.hesapKod);
                    XLS.HucreBirlestir(satir, 4, satir, 5);
                    XLS.HucreDegerYaz(satir, 4, detay.satirAciklama);

                    if (listeTur == "1")
                    {
                        borcToplam += detay.borc;
                        XLS.HucreDegerYaz(satir, 6, detay.borc);

                        alacakToplam += detay.alacak;
                        XLS.HucreDegerYaz(satir, 7, detay.alacak);

                        bakiye = bakiye + detay.borc - detay.alacak;
                        XLS.HucreDegerYaz(satir, 8, bakiye);
                    }
                    else if (listeTur == "2")
                    {
                        XLS.HucreDegerYaz(satir, 6, detay.dovizKurTur);
                        XLS.HucreDegerYaz(satir, 7, detay.dovizKur);

                        borcToplam += detay.dovizBorc;
                        XLS.HucreDegerYaz(satir, 8, detay.dovizBorc);

                        alacakToplam += detay.dovizAlacak;
                        XLS.HucreDegerYaz(satir, 9, detay.dovizAlacak);

                        bakiye = bakiye + detay.dovizBorc - detay.dovizAlacak;
                        XLS.HucreDegerYaz(satir, 10, bakiye);
                    }

                    if (satir > toplamSayfaSatirSayisi * sayfaNumarasi)
                    {
                        satir++;
                        XLS.HucreKopyala(satirD, 0, satirD, 10, satir, 0);
                        bakiye = SayfaToplamYaz(XLS, satir, "SAYFA TOPLAMI", listeTur, borcToplam, alacakToplam);

                        satir++;
                        XLS.HucreKopyala(satirD, 0, satirD, 10, satir, 0);
                        bakiye = SayfaToplamYaz(XLS, satir, "ÖNCEKİ SAYFADAN DEVİR", listeTur, borcToplam, alacakToplam);

                        XLS.SayfaSonuKoy(satir);
                        sayfaNumarasi++;
                    }

                    satir++;
                }
            }

            XLS.HucreKopyala(satirD, 0, satirD, 10, satir, 0);
            SayfaToplamYaz(XLS, satir, "GENEL TOPLAM", listeTur, borcToplam, alacakToplam);

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        // ------------------------------------------------------------------ Yazdır2 (eski btnListeYazdir2_Click)

        /// <summary>Hesap kodu kırılımlarına göre borç / alacak / bakiye özeti (MUH-MuavinDefteri2.xlt). Üretilen dosyanın yolu; hata olursa null.</summary>
        public string Yazdir2(out string hata)
        {
            hata = null;
            Tablo XLS = GenelIslemler.NewTablo();
            int satir = 0;

            int satirD = 0;
            int sutunD = 0;

            string sablonAd = string.Empty;
            string listeTur = pgFiltre.ListeTur();
            sablonAd = SablonOzet;
            //else if (listeTur == "2")
            //    sablonAd = "MUH-MuavinDefteriDoviz.xlt";

            string sablon = SablonBul(uygulamaKlasoru, sablonAd);
            if (sablon == null)
            {
                hata = "Rapor şablonu bulunamadı: MUH/" + sablonAd;
                return null;
            }

            ObjectArray fler = Sorgula("");
            if (fler == null)
            {
                hata = Uyarilar.Count > 0 ? Uyarilar[0] : "Sorgu yapılamadı.";
                return null;
            }
            if (!fler.sonuc.islemSonuc)
            {
                hata = string.IsNullOrWhiteSpace(fler.sonuc.hataStr) ? "Sorgu yapılamadı." : fler.sonuc.hataStr;
                return null;
            }

            string sonucDosyaAd = DosyaIslem.DosyaAdUret("MuavinDefteri2_", "") + "." + GenelIslemler.ExcelTur();
            XLS.DosyaAc(sablon, sonucDosyaAd);

            XLS.HucreAdAdresCoz("BaslaSatir", ref satirD, ref sutunD);
            satir = satirD + 1;

            string yil = pgFiltre["yil"];
            string hesapKod = pgFiltre["hesapNo"];

            string donem = string.Empty;
            if (pgFiltre["belgeTarihi1"] != "")
                donem += new TNSDateTime(pgFiltre["belgeTarihi1"]).ToString() + " - ";
            else
                donem += "01.01." + DateTime.Now.Year.ToString() + " - ";

            if (pgFiltre["belgeTarihi2"] != "")
                donem += new TNSDateTime(pgFiltre["belgeTarihi2"]).ToString();
            else
                donem += DateTime.Now.ToShortDateString();

            XLS.HucreAdBulYaz("Donem", donem);
            XLS.HucreAdBulYaz("HesapKod", hesapKod);

            string hesapAd = string.Empty;
            HesapPlaniSatir h = new HesapPlaniSatir();
            h.yil = OrtakFonksiyonlar.ConvertToInt(yil, DateTime.Now.Year);
            h.hesapKodHesapAd = hesapKod;
            h.detay = false;
            ObjectArray hesap = servisMUH.HesapPlaniListele(kullanan, h);
            if (hesap.ObjeSayisi > 0)
                hesapAd = ((HesapPlaniSatir)hesap.objeler[0]).aciklama;

            XLS.HucreAdBulYaz("HesapAd", hesapAd);

            double bakiye = 0.0;
            double borcToplam = 0.0;
            double alacakToplam = 0.0;

            UstToplamYaz(XLS, ref satir, ref borcToplam, ref alacakToplam);

            bakiye = borcToplam - alacakToplam;

            Hashtable htBorc = new Hashtable();
            Hashtable htAlacak = new Hashtable();
            string hKod = string.Empty;
            foreach (OdemeEmriMIFForm ff in fler.objeler)
            {
                foreach (OdemeEmriMIFDetay detay in ff.detay.objeler)
                {
                    hKod = string.Empty;
                    var hKodList = detay.hesapKod.Split('.');
                    for (int i = 0; i < hKodList.Length; i++)
                    {
                        hKod += hKodList[i] + ".";
                        if (hKod.Replace(".", "") == detay.hesapKod.Replace(".", ""))
                            hKod = hKod + "@" + detay.hesapAd;
                        HTDoubleEkle(htBorc, hKod, detay.borc);
                        HTDoubleEkle(htAlacak, hKod, detay.alacak);
                    }
                }
            }

            List<string> list = new List<string>();
            foreach (DictionaryEntry dt in htBorc)
            {
                string anahtar = (string)dt.Key;
                double borc = (double)htBorc[anahtar];
                double alacak = (double)htAlacak[anahtar];
                list.Add(anahtar + ";" + borc.ToString("N2") + ";" + alacak.ToString("N2"));
            }
            list.Sort();

            foreach (string deger in list)
            {
                var degerList = deger.Split(';');
                string anahtar = degerList[0];
                double borc = OrtakFonksiyonlar.ConvertToDouble(degerList[1], 0.0);
                double alacak = OrtakFonksiyonlar.ConvertToDouble(degerList[2], 0.0);

                if (borc > 0.0 || alacak > 0.0)
                {
                    XLS.HucreKopyala(satirD, 0, satirD, 10, satir, 0);

                    var anahtarListe = anahtar.Split('@');
                    if (anahtarListe.Length > 1)
                    {
                        XLS.HucreDegerYaz(satir, 0, anahtarListe[0]);
                        XLS.HucreDegerYaz(satir, 1, anahtarListe[1]); //hesapAd
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, 0, anahtar);
                        XLS.HucreDegerYaz(satir, 1, HesapPlaniAdGetir(OrtakFonksiyonlar.ConvertToInt(yil, DateTime.Now.Year), anahtar.Replace(".", ""))); //hesapAd
                    }

                    XLS.HucreDegerYaz(satir, 2, borc);
                    XLS.HucreDegerYaz(satir, 3, alacak);
                    double b = borc - alacak;
                    if (b > 0.0)
                    {
                        XLS.HucreDegerYaz(satir, 4, b.ToString("N2") + "(B)");
                    }
                    else
                    {
                        XLS.HucreDegerYaz(satir, 4, Math.Abs(b).ToString("N2") + "(A)");
                    }

                    satir++;
                }
            }

            XLS.DosyaSaklaTamYol();
            return sonucDosyaAd;
        }

        private static double SayfaToplamYaz(Tablo XLS, int satir, string baslik, string listeTur, double borcToplam, double alacakToplam)
        {
            double bakiye = borcToplam - alacakToplam;
            if (listeTur == "1")
            {
                XLS.HucreBirlestir(satir, 0, satir, 5);
                XLS.HucreDegerYaz(satir, 0, baslik);

                XLS.HucreDegerYaz(satir, 6, borcToplam);
                XLS.HucreDegerYaz(satir, 7, alacakToplam);

                XLS.HucreDegerYaz(satir, 8, bakiye);
            }
            else if (listeTur == "2")
            {
                XLS.HucreBirlestir(satir, 0, satir, 7);
                XLS.HucreDegerYaz(satir, 0, baslik);

                XLS.HucreDegerYaz(satir, 8, borcToplam);
                XLS.HucreDegerYaz(satir, 9, alacakToplam);

                bakiye = borcToplam - alacakToplam;
                XLS.HucreDegerYaz(satir, 10, bakiye);
            }
            return bakiye;
        }

        private void UstToplamYaz(Tablo XLS, ref int satir, ref double borcToplam, ref double alacakToplam)
        {
            string hesapKod = pgFiltre["hesapNo"];
            hesapKod = hesapKod.Split('-')[0].Trim();

            OdemeEmriMIFDetay toplamDetay = TarihOncesiToplamGetir(hesapKod);

            if (toplamDetay != null)
            {
                string listeTur = pgFiltre.ListeTur();
                int satirD = 0;
                int sutunD = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref satirD, ref sutunD);
                XLS.HucreKopyala(satirD, 0, satirD, 10, satir, 0);
                if (listeTur == "1")
                {
                    XLS.HucreBirlestir(satir, 0, satir, 5);
                    XLS.HucreDegerYaz(satir, 0, "DEVİR");

                    XLS.HucreDegerYaz(satir, 6, toplamDetay.borc);
                    XLS.HucreDegerYaz(satir, 7, toplamDetay.alacak);

                    borcToplam = toplamDetay.borc;
                    alacakToplam = toplamDetay.alacak;
                    XLS.HucreDegerYaz(satir, 8, toplamDetay.borc - toplamDetay.alacak);
                }
                else if (listeTur == "2")
                {
                    XLS.HucreBirlestir(satir, 0, satir, 7);
                    XLS.HucreDegerYaz(satir, 0, "DEVİR");

                    XLS.HucreDegerYaz(satir, 8, toplamDetay.dovizBorc);
                    XLS.HucreDegerYaz(satir, 9, toplamDetay.dovizAlacak);

                    borcToplam = toplamDetay.dovizBorc;
                    alacakToplam = toplamDetay.dovizAlacak;
                    XLS.HucreDegerYaz(satir, 10, toplamDetay.dovizBorc - toplamDetay.dovizAlacak);
                }
                satir++;
            }
        }

        // ------------------------------------------------------------------ tarih öncesi toplam (eski TarihOncesiToplamGetir)

        private OdemeEmriMIFDetay TarihOncesiToplamGetir(string hesapKod)
        {
            OdemeEmriMIFForm f = new OdemeEmriMIFForm();
            f.yil = OrtakFonksiyonlar.ConvertToInt(pgFiltre["yil"], 0);
            f.formDurum = OrtakFonksiyonlar.ConvertToInt(pgFiltre["durum"], 0);

            if (string.IsNullOrEmpty(hesapKod))
            {
                MesajKutusu("Hesap Kod alanı boş bırakılamaz.");
                return null;
            }

            if (string.IsNullOrEmpty(pgFiltre["belgeTarihi1"]))
                return null;

            f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
            f.muhasebe = OrtakFonksiyonlar.ConvertToInt(pgFiltre["muhasebe"], 0);
            f.birim = pgFiltre["birim"];

            if (pgFiltre["belgeTur"] != "")
                f.sorguBelgeTur = OrtakFonksiyonlar.ConvertToInt(pgFiltre["belgeTur"], -1);

            string b1 = pgFiltre["belgeNo1"];
            string b2 = pgFiltre["belgeNo2"];
            if (b1.Trim() != "" || b2.Trim() != "")
            {
                if (b1 != "" && b1.Length < 9)
                    b1 = f.kurum.Replace(".", "").PadRight(4, '0') + b1.PadLeft(6, '0');
                if (b2 != "" && b2.Length < 9)
                    b2 = f.kurum.Replace(".", "").PadRight(4, '0') + b2.PadLeft(6, '0');

                f.belgeNo = b1 + ";" + b2;
            }

            if (pgFiltre["yevmiyeNo1"] != "" || pgFiltre["yevmiyeNo2"] != "")
                f.sorguYEVYevNo = pgFiltre["yevmiyeNo1"] + ";" + pgFiltre["yevmiyeNo2"];

            if (pgFiltre["yevmiyeTarihi1"] != "")
                f.sorguYEVYevTarih = new TNSDateTime(pgFiltre["yevmiyeTarihi1"]).ToString() + ";";
            if (pgFiltre["yevmiyeTarihi2"] != "")
                f.sorguYEVYevTarih += new TNSDateTime(pgFiltre["yevmiyeTarihi2"]).ToString();

            TNSDateTime ilkTarih = new TNSDateTime(pgFiltre["belgeTarihi1"]);
            TNSDateTime ikinciTarih = new TNSDateTime(pgFiltre["belgeTarihi2"]);
            if (ilkTarih.isNull) return null;

            string tarih = ilkTarih.ToString();
            string yil = ilkTarih.Yil.ToString();

            if (tarih != "")
            {
                var tarih1 = tarih;
                if (ilkTarih.Gun == 1 && ilkTarih.Ay == 1)
                    return null;

                f.sorguIslemTarih = new TNSDateTime("01.01." + yil).ToString() + ";";

                if (ikinciTarih.isNull)
                {
                    var tempDate = DateTime.Parse(tarih1).AddDays(-1.0);
                    f.sorguIslemTarih = new TNSDateTime("01.01." + yil).ToString() + ";" + tempDate.ToShortDateString();
                }
            }
            if (!ikinciTarih.isNull)
            {
                var tarih2 = new TNSDateTime(tarih);
                tarih2.AddDays(-1);
                f.sorguIslemTarih += tarih2.ToString();
            }

            if (pgFiltre["durumTarihi1"] != "")
                f.sorguFormDurumTarih = new TNSDateTime(pgFiltre["durumTarihi1"]).ToString() + ";";
            if (pgFiltre["durumTarihi2"] != "")
                f.sorguFormDurumTarih += new TNSDateTime(pgFiltre["durumTarihi2"]).ToString();

            f.ilgiliAd = pgFiltre["ilgili"];
            f.ilgiliNo = pgFiltre["ilgiliNo"];
            f.islemYapan = pgFiltre["islemYapan"];
            f.projeNo = pgFiltre["yatirimProjeNo"];
            f.aciklama = pgFiltre["aciklama"];

            f.OEBTur = pgFiltre["OdEsTur"];
            f.OEBNo = pgFiltre["OdEsNo"];
            if (pgFiltre["OdEsTarihi"].Trim() != "")
                f.OEBTarih = new TNSDateTime(pgFiltre["OdEsTarihi"].Trim());

            f.YEVCekNo = pgFiltre["cekNoSorgu"];
            f.fisTurYap(new ENUMFISTUR[] { ENUMFISTUR.ACILIS, ENUMFISTUR.DONEMICI });

            OdemeEmriMIFDetay detay = new OdemeEmriMIFDetay(f.yil);
            detay.hesapKod = hesapKod;

            detay.kur = pgFiltre["kur"];
            detay.fon = pgFiltre["fon"];
            detay.fin = pgFiltre["fin"];
            detay.eko = pgFiltre["eko"];
            detay.peb = pgFiltre["peb"];

            detay.borc = OrtakFonksiyonlar.ConvertToDbl(pgFiltre["borc"]);
            detay.alacak = OrtakFonksiyonlar.ConvertToDbl(pgFiltre["alacak"]);
            f.detay.objeler.Add(detay);

            f.sorguHesapKodSiraliMi = true;
            ObjectArray liste = servisMUH.OdemeEmriMIFTarihOncesiToplamGetir(kullanan, f);

            if (liste.sonuc.islemSonuc)
            {
                if (liste.ObjeSayisi > 0)
                {
                    return (OdemeEmriMIFDetay)liste.objeler[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                MesajKutusu(liste.sonuc.hataStr);
                return null;
            }
        }

        public static string DurumDonustur(int durum)
        {
            if (durum == (int)ENUMBelgeDurumu.YENI)
                return "Yeni";
            else if (durum == (int)ENUMBelgeDurumu.DEGISTIRILDI)
                return "Değiştirildi";
            else if (durum == (int)ENUMBelgeDurumu.ONAYLI)
                return "Onaylı";
            else if (durum == (int)ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                return "Yevmiyeli";
            else if (durum == (int)ENUMBelgeDurumu.IPTAL)
                return "İptal";
            else
                return "Belirsiz - " + durum;
        }

        private string HesapPlaniAdGetir(int yil, string hesapKod)
        {
            HesapPlaniSatir h = new HesapPlaniSatir();
            h.yil = yil;
            if (h.yil == 0) h.yil = DateTime.Now.Year;
            h.hesapKod = hesapKod;
            h.detay = false;
            ObjectArray hesap = servisMUH.HesapPlaniListele(kullanan, h);

            if (hesap.ObjeSayisi > 0)
            {
                return ((HesapPlaniSatir)hesap.objeler[0]).aciklama;
            }
            else
                return string.Empty;
        }
    }
}

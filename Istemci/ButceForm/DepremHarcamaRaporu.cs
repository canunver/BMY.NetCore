using System.Collections;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.KYM;
using TNS.MUH;

namespace ButceForm
{
    /// <summary>
    /// Deprem harcamaları raporları ve sorguları; eski ButceForm/DepremHarcamaForm.aspx.cs içindeki DepremRapor sınıfının aynısıdır.
    /// Yalnızca Yazdir... yordamları dosyayı göndermek yerine üretilen dosyanın yolunu döndürür (DosyaIslem.DosyaGonder ile gönderilir):
    ///   YazdirIlEko(1): İl-Eko Yazdır, YazdirIlEko(2): Eko Yazdır (şablon BHF\DepremIlEko.xltx)
    ///   YazdirBirim:    Muh. Birim Yazdır (şablon BHF\DepremIlEko.xltx)
    ///   YazdirListe:    Liste Yazdır (şablon BHF\DepremDetay.xltx)
    /// BirimSorgu (Onay İşlemleri listesi) ve GetirIlEko (e-bütçeye aktarım) de buradan kullanılır.
    /// </summary>
    public class DepremRapor
    {
        string sablonYol;
        Kullanici kullanan;
        DepremHarcamaAna form;
        IMUHServis muhServis;
        public DepremRapor(Kullanici kullanan, string sablonYol, DepremHarcamaAna form)
        {
            this.sablonYol = sablonYol;
            this.kullanan = kullanan;
            this.form = form;
            muhServis = TNS.MUH.Arac.Tanimla();
        }

        public SortedList GetirIlEko()
        {
            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, TNS.MUH.ENUMOGY.Gorme))
            {
                return null;
            }

            string[] sql = new string[1];
            string[] baglanti = new string[1];

            sql[0] = IlEkoSorguOl(form, 2, 1);

            baglanti[0] = "BaglantiSatiriBHM";
            TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            SortedList sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3 });
            return sl;
        }


        public static string BirimSorgu(DepremHarcamaAna form)
        {
            string sql = "SELECT MB.MUHASEBEKOD, MB.BIRIMKOD, M.MUHASEBEAD, MB.BIRIMAD, SUM(BUTCEICI), SUM(HT1), SUM(HT2), SUM(HT3), SUM(HT4), SUM(HT5), A.DURUM";
            sql += " FROM ButceIslemleri.dbo.MUHMUHASEBE M";
            sql += " INNER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM MB ON MB.MUHASEBEKOD = M.MUHASEBEKOD";
            sql += $" LEFT OUTER JOIN [ButceHazirlik].[dbo].[BHFDEPREMHARCAMAANA] A ON A.MUHASEBEKOD = MB.MUHASEBEKOD AND A.BIRIMKOD = MB.BIRIMKOD AND A.YIL={form.yil} AND A.AY={form.ay}";
            sql += $" LEFT OUTER JOIN [ButceHazirlik].[dbo].[BHFDEPREMHARCAMADETAY] D ON D.MUHASEBEKOD = MB.MUHASEBEKOD AND D.BIRIMKOD = MB.BIRIMKOD AND D.YIL={form.yil} AND D.AY={form.ay}";
            sql += " WHERE MB.BIRIMKOD!='000' AND MB.BIRIMKOD!='250'";
            if (form.muhasebeKod > 0)
                sql += $" AND D.MUHASEBEKOD = {form.muhasebeKod} ";
            if (!string.IsNullOrWhiteSpace(form.birimKod))
                sql += $" AND D.BIRIMKOD = {OrtakFonksiyonlar.Tirnakla(form.birimKod)} ";

            sql += $" AND 1 NOT IN (SELECT 1 FROM ButceIslemleri.dbo.MUHMUHASEBEBIRIMKAPALI K where K.BIRIMKOD = MB.BIRIMKOD AND  K.MUHASEBEKOD = MB.MUHASEBEKOD) ";

            sql += " group by MB.MUHASEBEKOD, MB.BIRIMKOD, M.MUHASEBEAD, MB.BIRIMAD, A.DURUM";
            sql += " order by 1, 2";
            return sql;
        }

        public string YazdirBirim()
        {
            string hata = "";
            string sonucDosya = DosyaIslem.DosyaAdUret("DepremIlEko_", $"{form.yil}_{form.ay}_{form.muhasebeKod}") + "." + GenelIslemler.ExcelTur();
            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonYol + "/BHF/DepremIlEko.xltx", sonucDosya);

            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, TNS.MUH.ENUMOGY.Gorme))
            {
                hata = ("Bu birimle (muhasebe:" + form.muhasebeKod + ", birim: " + form.birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.");
                XLS.HucreAdBulYaz("Baslik", hata);
            }
            else
            {
                //Bütçe İçi,  Özel Hesap,  Döner Sermaye,  Fon,  Sosyal Tesis,  Diğer
                string[] sql = new string[1];
                string[] baglanti = new string[1];
                sql[0] = BirimSorgu(form);

                baglanti[0] = "BaglantiSatiriBHM";
                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

                SortedList sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3 });

                XLS.HucreAdBulYaz("Baslik", $"{form.yil} yılı {form.ay}. ay deprem harcamaları");
                int fSat = 0, fSut = 0, baslaSatir = 0, sutun = 0;
                XLS.HucreAdAdresCoz("FormatSatir", ref fSat, ref fSut);
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
                int satir = baslaSatir;
                ToplamAl t = new ToplamAl(2, 6);
                foreach (DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    int muhKod = alAnahtar.IntAl(0);
                    string birimKod = alAnahtar.StrAl(1);
                    string muhAd = alAnahtar.StrAl(2);
                    string birimAd = alAnahtar.StrAl(3);

                    int fEkle = 4;
                    int basla = 0;
                    XLS.HucreKopyala(fSat + fEkle, fSut, fSat + fEkle, fSut + 7, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, TNS.MUH.Arac.BirimAdOl(muhAd, birimAd));

                    XLS.HucreDegerYaz(satir, 9, muhKod);
                    XLS.HucreDegerYaz(satir, 10, birimKod);

                    t.DegerAta(1, 0, alVeri.DblAl(basla + 0));
                    t.DegerAta(1, 1, alVeri.DblAl(basla + 1));
                    t.DegerAta(1, 2, alVeri.DblAl(basla + 2));
                    t.DegerAta(1, 3, alVeri.DblAl(basla + 3));
                    t.DegerAta(1, 4, alVeri.DblAl(basla + 4));
                    t.DegerAta(1, 5, alVeri.DblAl(basla + 5));

                    XLS.HucreDegerYaz(satir, 1, t.toplam[1, 0]);
                    XLS.HucreDegerYaz(satir, 2, t.toplam[1, 1]);
                    XLS.HucreDegerYaz(satir, 3, t.toplam[1, 2]);
                    XLS.HucreDegerYaz(satir, 4, t.toplam[1, 3]);
                    XLS.HucreDegerYaz(satir, 5, t.toplam[1, 4]);
                    XLS.HucreDegerYaz(satir++, 6, t.toplam[1, 5]);
                    t.ToplamEkle(0, 1);
                }

                XLS.HucreKopyala(fSat, fSut, fSat, fSut + 7, satir, sutun);
                XLS.HucreDegerYaz(satir, 1, t.toplam[0, 0]);
                XLS.HucreDegerYaz(satir, 2, t.toplam[0, 1]);
                XLS.HucreDegerYaz(satir, 3, t.toplam[0, 2]);
                XLS.HucreDegerYaz(satir, 4, t.toplam[0, 3]);
                XLS.HucreDegerYaz(satir, 5, t.toplam[0, 4]);
                XLS.HucreDegerYaz(satir++, 6, t.toplam[0, 5]);
                XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 5, 15);
                XLS.SatirSil(0, 5);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosya;
        }

        public string YazdirIlEko(int raporTur)
        {
            string hata = "";
            string sonucDosya = DosyaIslem.DosyaAdUret("DepremIlEko_", $"{form.yil}_{form.ay}_{form.muhasebeKod}") + "." + GenelIslemler.ExcelTur();
            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonYol + "/BHF/DepremIlEko.xltx", sonucDosya);

            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, TNS.MUH.ENUMOGY.Gorme))
            {
                hata = ("Bu birimle (muhasebe:" + form.muhasebeKod + ", birim: " + form.birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.");
                XLS.HucreAdBulYaz("Baslik", hata);
            }
            else
            {
                //Bütçe İçi,  Özel Hesap,  Döner Sermaye,  Fon,  Sosyal Tesis,  Diğer
                string[] sql = new string[3];
                string[] baglanti = new string[3];

                sql[0] = IlEkoSorguOl(form, 1, raporTur);
                sql[1] = IlEkoSorguOl(form, 2, raporTur);
                sql[2] = IlEkoSorguOl(form, 0, raporTur);

                baglanti[0] = "BaglantiSatiriBHM";
                baglanti[1] = "BaglantiSatiriBHM";
                baglanti[2] = "BaglantiSatiriBHM";
                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

                SortedList sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3 });

                XLS.HucreAdBulYaz("Baslik", $"{form.yil} yılı {form.ay}. ay deprem harcamaları");
                int fSat = 0, fSut = 0, baslaSatir = 0, sutun = 0;
                XLS.HucreAdAdresCoz("FormatSatir", ref fSat, ref fSut);
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
                int satir = baslaSatir;
                ToplamAl t = new ToplamAl(2, 6);
                foreach (DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    string ekoKod = alAnahtar.StrAl(2);

                    int basla, fEkle;
                    if (ekoKod.Length == 0)
                    {
                        fEkle = 1;
                        basla = 12;
                    }
                    else if (ekoKod.Length == 2)
                    {
                        if (raporTur == 1)
                            fEkle = 2;
                        else
                            fEkle = 1;
                        basla = 0;
                    }
                    else
                    {
                        if (raporTur == 1)
                            fEkle = 3;
                        else
                            fEkle = 2;
                        basla = 6;
                    }
                    XLS.HucreKopyala(fSat + fEkle, fSut, fSat + fEkle, fSut + 7, satir, sutun);

                    if (basla == 12)
                        XLS.HucreDegerYaz(satir, 0, alAnahtar.StrAl(1));
                    else
                        XLS.HucreDegerYaz(satir, 0, alAnahtar.StrAl(3));

                    XLS.HucreDegerYaz(satir, 9, alAnahtar.IntAl(0));
                    XLS.HucreDegerYaz(satir, 10, alAnahtar.StrAl(1));
                    XLS.HucreDegerYaz(satir, 11, ekoKod);
                    XLS.HucreDegerYaz(satir, 12, alAnahtar.StrAl(3));

                    t.DegerAta(1, 0, alVeri.DblAl(basla + 0));
                    t.DegerAta(1, 1, alVeri.DblAl(basla + 1));
                    t.DegerAta(1, 2, alVeri.DblAl(basla + 2));
                    t.DegerAta(1, 3, alVeri.DblAl(basla + 3));
                    t.DegerAta(1, 4, alVeri.DblAl(basla + 4));
                    t.DegerAta(1, 5, alVeri.DblAl(basla + 5));

                    XLS.HucreDegerYaz(satir, 1, t.toplam[1, 0]);
                    XLS.HucreDegerYaz(satir, 2, t.toplam[1, 1]);
                    XLS.HucreDegerYaz(satir, 3, t.toplam[1, 2]);
                    XLS.HucreDegerYaz(satir, 4, t.toplam[1, 3]);
                    XLS.HucreDegerYaz(satir, 5, t.toplam[1, 4]);
                    XLS.HucreDegerYaz(satir++, 6, t.toplam[1, 5]);
                    if (basla == 0)
                        t.ToplamEkle(0, 1);
                }

                XLS.HucreKopyala(fSat, fSut, fSat, fSut + 7, satir, sutun);
                XLS.HucreDegerYaz(satir, 1, t.toplam[0, 0]);
                XLS.HucreDegerYaz(satir, 2, t.toplam[0, 1]);
                XLS.HucreDegerYaz(satir, 3, t.toplam[0, 2]);
                XLS.HucreDegerYaz(satir, 4, t.toplam[0, 3]);
                XLS.HucreDegerYaz(satir, 5, t.toplam[0, 4]);
                XLS.HucreDegerYaz(satir++, 6, t.toplam[0, 5]);
                XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 5, 15);
                XLS.SatirSil(0, 5);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosya;
        }

        public string YazdirListe(string ekoKod)
        {
            string hata = "";
            string sonucDosya = DosyaIslem.DosyaAdUret("DepremIlEko_", $"{form.yil}_{form.ay}_{form.muhasebeKod}") + "." + GenelIslemler.ExcelTur();
            Tablo XLS = GenelIslemler.NewTablo();
            XLS.DosyaAc(sablonYol + "/BHF/DepremDetay.xltx", sonucDosya);

            if (!muhServis.MuhasebeBirimYetkiVarMi(kullanan, form.muhasebeKod.ToString(), form.birimKod, TNS.MUH.ENUMOGY.Gorme))
            {
                hata = ("Bu birimle (muhasebe:" + form.muhasebeKod + ", birim: " + form.birimKod + ") ilgili verileri görme yetkiniz bulunmamaktadır.");
                XLS.HucreAdBulYaz("Baslik", hata);
            }
            else
            {
                //Bütçe İçi,  Özel Hesap,  Döner Sermaye,  Fon,  Sosyal Tesis,  Diğer
                string[] sql = new string[1];
                string[] baglanti = new string[1];

                sql[0] = "SELECT  D.MUHASEBEKOD, D.BIRIMKOD, D.ILKOD, D.EKOKOD, M.MUHASEBEAD, MB.BIRIMAD, I.ILADI, S.AD, BUTCEICI, HT1, HT2, HT3, HT4, HT5, BUTCEICIACIKLAMA, BUTCEDISIACIKLAMA";
                sql[0] += $" FROM [ButceHazirlik].[dbo].[BHFDEPREMHARCAMADETAY] D";
                sql[0] += " LEFT OUTER JOIN  ButceIslemleri.dbo.MUHMUHASEBEBIRIM MB  ON D.MUHASEBEKOD = MB.MUHASEBEKOD AND D.BIRIMKOD = MB.BIRIMKOD";
                sql[0] += " LEFT OUTER JOIN  ButceIslemleri.dbo.MUHMUHASEBE M ON M.MUHASEBEKOD = D.MUHASEBEKOD";
                sql[0] += $" LEFT OUTER JOIN [ButceHazirlik].[dbo].BHFDEPREMSOZLUK S ON S.KOD = D.EKOKOD";
                sql[0] += " LEFT OUTER JOIN [Yatirim].[dbo].YATIL I ON I.ILKODU = D.ILKOD ";
                sql[0] += $" WHERE YIL = {form.yil} AND AY = {form.ay}";

                if (form.muhasebeKod > 0)
                    sql[0] += $" AND D.MUHASEBEKOD = {form.muhasebeKod} ";

                if (!string.IsNullOrWhiteSpace(form.birimKod))
                    sql[0] += $" AND D.BIRIMKOD = {OrtakFonksiyonlar.Tirnakla(form.birimKod)} ";

                if (!string.IsNullOrWhiteSpace(ekoKod))
                    sql[0] += $" AND D.EKOKOD LIKE {OrtakFonksiyonlar.Tirnakla(ekoKod.Replace(".", "") + "%")} ";

                sql[0] += " order by 1, 2, 3, 4";

                baglanti[0] = "BaglantiSatiriBHM";
                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

                SortedList sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0, 1, 2, 3 });

                XLS.HucreAdBulYaz("Baslik", $"{form.yil} yılı {form.ay}. ay deprem harcamaları");
                int fSat = 0, fSut = 0, baslaSatir = 0, sutun = 0;
                XLS.HucreAdAdresCoz("FormatSatir", ref fSat, ref fSut);
                XLS.HucreAdAdresCoz("BaslaSatir", ref baslaSatir, ref sutun);
                int satir = baslaSatir;
                ToplamAl t = new ToplamAl(2, 6);
                int fEkle = 1;
                int basla = 4;
                foreach (DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    int muhKod = alAnahtar.IntAl(0);
                    string birimKod = alAnahtar.StrAl(1);
                    string ilKod = alAnahtar.StrAl(2);
                    ekoKod = alAnahtar.StrAl(3);

                    string muhAd = alVeri.StrAl(0);
                    string birimAd = alVeri.StrAl(1);
                    string ilAd = alVeri.StrAl(2);
                    string ekoAd = alVeri.StrAl(3);

                    XLS.HucreKopyala(fSat + fEkle, fSut, fSat + fEkle, fSut + 10, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, TNS.MUH.Arac.BirimAdOl(muhAd, birimAd));
                    XLS.HucreDegerYaz(satir, 1, ilAd);
                    XLS.HucreDegerYaz(satir, 2, ekoAd);

                    t.DegerAta(1, 0, alVeri.DblAl(basla + 0));
                    t.DegerAta(1, 1, alVeri.DblAl(basla + 1));
                    t.DegerAta(1, 2, alVeri.DblAl(basla + 2));
                    t.DegerAta(1, 3, alVeri.DblAl(basla + 3));
                    t.DegerAta(1, 4, alVeri.DblAl(basla + 4));
                    t.DegerAta(1, 5, alVeri.DblAl(basla + 5));

                    XLS.HucreDegerYaz(satir, 3, t.toplam[1, 0]);
                    XLS.HucreDegerYaz(satir, 4, t.toplam[1, 1]);
                    XLS.HucreDegerYaz(satir, 5, t.toplam[1, 2]);
                    XLS.HucreDegerYaz(satir, 6, t.toplam[1, 3]);
                    XLS.HucreDegerYaz(satir, 7, t.toplam[1, 4]);
                    XLS.HucreDegerYaz(satir, 8, t.toplam[1, 5]);


                    XLS.HucreDegerYaz(satir, 11, muhKod);
                    XLS.HucreDegerYaz(satir++, 12, birimKod);

                    t.ToplamEkle(0, 1);
                }

                XLS.HucreKopyala(fSat, fSut, fSat, fSut + 10, satir, sutun);
                XLS.HucreDegerYaz(satir, 3, t.toplam[0, 0]);
                XLS.HucreDegerYaz(satir, 4, t.toplam[0, 1]);
                XLS.HucreDegerYaz(satir, 5, t.toplam[0, 2]);
                XLS.HucreDegerYaz(satir, 6, t.toplam[0, 3]);
                XLS.HucreDegerYaz(satir, 7, t.toplam[0, 4]);
                XLS.HucreDegerYaz(satir++, 8, t.toplam[0, 5]);
                XLS.SatirYukseklikAyarla(baslaSatir, satir, -1, 5, 15);
                XLS.SatirSil(0, 2);
            }
            XLS.DosyaSaklaTamYol();
            return sonucDosya;
        }

        private string IlEkoSorguOl(DepremHarcamaAna form, int komutTur, int raporTur)
        {
            string alanlar;
            string grpAlanlar;
            if (komutTur == 0)
            {
                if (raporTur == 2) return null;
                alanlar = "I.ILKODU, I.ILADI,'',''";
                grpAlanlar = "I.ILKODU, I.ILADI";
            }
            else
            {
                if (raporTur == 1)
                {
                    alanlar = "I.ILKODU, I.ILADI, S.KOD, S.AD";
                    grpAlanlar = alanlar;
                }
                else
                {
                    alanlar = "'', '', S.KOD, S.AD";
                    grpAlanlar = "S.KOD, S.AD";
                }
            }
            string sqlOrt = $"SELECT " + alanlar + ", SUM([BUTCEICI]), SUM([HT1]), SUM([HT2]), SUM([HT3]), SUM([HT4]), SUM([HT5]) ";
            sqlOrt += " FROM [ButceHazirlik].[dbo].[BHFDEPREMHARCAMADETAY] D ";
            if (komutTur != 0)
                sqlOrt += $" LEFT OUTER JOIN [ButceHazirlik].[dbo].BHFDEPREMSOZLUK S ON S.KOD = {(komutTur == 1 ? "LEFT(D.EKOKOD,2)" : "D.EKOKOD")}";
            if (raporTur == 1)
                sqlOrt += " LEFT OUTER JOIN [Yatirim].[dbo].YATIL I ON I.ILKODU = D.ILKOD ";
            sqlOrt += $" where YIL = {form.yil} AND AY = {form.ay} ";
            if (komutTur != 0)
                sqlOrt += $" AND LEN(S.KOD) = {(komutTur == 1 ? "2" : "4")}";
            if (form.muhasebeKod > 0)
                sqlOrt += $" AND D.MUHASEBEKOD = {form.muhasebeKod} ";
            if (!string.IsNullOrWhiteSpace(form.birimKod))
                sqlOrt += $" AND D.BIRIMKOD = {OrtakFonksiyonlar.Tirnakla(form.birimKod)} ";
            if (form.ilKod > 0)
                sqlOrt += $" AND D.ILKOD = {form.ilKod} ";

            sqlOrt += " group by " + grpAlanlar;
            sqlOrt += " order by 1,3 ";
            return sqlOrt;
        }
    }
}

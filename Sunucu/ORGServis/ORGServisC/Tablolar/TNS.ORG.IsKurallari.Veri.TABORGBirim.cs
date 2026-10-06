using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.ORG;
using TNS.ORG.IsKurallari.Veri;

namespace TNS.ORG.IsKurallari.Veri
{
    /// <summary>
    /// Organizasyon Birim veri tabanı tablosu ile ilgili olarak listeleme, kaydetme ve silme işlemini yapan sınıf
    /// </summary>
    public class TABORGBirim : TABOrtak
    {
        /// <summary>
        ///  Sınıfın oluşturucu(Constructer) fonksiyonu
        /// </summary>
        /// <param name="veriErisim">Veri Erişim tipinde sınıf</param>
        public TABORGBirim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adı
        /// </summary>
        public static string TABLOAD = "ORGBIRIM";

        /// <summary>
        /// Tablo alanları indisi
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Kod
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Adı
            /// </summary>
            ADI = 1,
            /// <summary>
            /// Kısa Adı
            /// </summary>
            KISAADI = 2,
            /// <summary>
            /// Ana birim kod
            /// </summary>
            ANABIRIMKOD = 3,
            /// <summary>
            /// Başlangıç tarihi
            /// </summary>
            BASLANGICTARIH = 4,
            /// <summary>
            /// Bitiş Tarihi
            /// </summary>
            BITISTARIH = 5,
            /// <summary>
            /// Muhasebe Kodu
            /// </summary>
            MUHASEBEKODU = 6,
            /// <summary>
            /// Muhasebe Birimi
            /// </summary>
            MUHASEBEBIRIM = 7,
            /// <summary>
            /// Evrak Birimi
            /// </summary>
            EVRAKBIRIM = 8,
            /// <summary>
            /// Ülke
            /// </summary>
            ULKE = 9,
            /// <summary>
            /// Bölge
            /// </summary>
            BOLGE = 10,
            /// <summary>
            /// İl
            /// </summary>
            IL = 11,
            /// <summary>
            /// İlçe
            /// </summary>
            ILCE = 12,
            /// <summary>
            /// Kurumsal Kodu
            /// </summary>
            KURUMSALKOD = 13,
            /// <summary>
            /// Vergi Numarası
            /// </summary>
            VERGINO = 14,
            /// <summary>
            /// Vergi Dairesi
            /// </summary>
            VERGIDAIRESI = 15,
            /// <summary>
            /// Adres
            /// </summary>
            ADRES = 16,
            /// <summary>
            /// Telefon
            /// </summary>
            TELEFON = 17,
            /// <summary>
            /// Faks
            /// </summary>
            FAKS = 18,
            /// <summary>
            /// E-Posta
            /// </summary>
            EPOSTA = 19,
            /// <summary>
            /// URL
            /// </summary>
            URL = 20,
            /// <summary>
            /// Antet
            /// </summary>
            ANTET = 21,
            /// <summary>
            /// Yer Bilgisi
            /// </summary>
            YERMI = 22,
            /// <summary>
            /// Harcama birimi mi?
            /// </summary>
            HARCAMAMI = 23,
            /// <summary>
            /// Bütçe birim mi?
            /// </summary>
            BUTCEMI = 24,
            /// <summary>
            /// Taşınır birim mi?
            /// </summary>
            TASINIRMI = 25,
            /// <summary>
            /// Personel birim mi?
            /// </summary>
            PERSONELMI = 26,
            /// <summary>
            /// Evrak birim mi?
            /// </summary>
            EVRAKMI = 27,
            /// <summary>
            /// Ülke mi?
            /// </summary>
            ULKEMI = 28,
            /// <summary>
            /// Bölge mi?
            /// </summary>
            BOLGEMI = 29,
            /// <summary>
            /// İl mi?
            /// </summary>
            ILMI = 30,
            /// <summary>
            /// İlçe mi?
            /// </summary>
            ILCEMI = 31,
            /// <summary>
            /// İletişim bilgisi mi?
            /// </summary>
            ILETISIMMI = 32,
            KOPRUKOD = 33,
            YONETICI = 34,
            SIRANO = 35,
            SABLONMU = 36,
            FONKOD = 37,
            MERKEZKOD = 38,
            USTYONETICI1 = 39,
            USTYONETICI2 = 40,
            TURKOD = 41,
            CIZIMCIKISNOKTASI = 42,
            CIZIMDEGORUNME = 43,
            KURUMMU = 44,
            KURUMKOD = 45,
            AMBARMI = 46,
            AMBARKOD = 47,
            YONETICITCKIMLIK = 48,
            USTYONETICITCKIMLIK1 = 49,
            USTYONETICITCKIMLIK2 = 50,
            ENTEGREKOD = 51,
            ALTFAALKODLAR = 52,
        }

        /// <summary>
        /// Tablo alanları
        /// </summary>
        static string[] ALANLAR = { "KOD", "ADI", "KISAADI", "ANABIRIMKOD", "BASLANGICTARIH", "BITISTARIH", "MUHASEBEKODU",
                                      "MUHASEBEBIRIM", "EVRAKBIRIM", "ULKE", "BOLGE", "IL", "ILCE", "KURUMSALKOD", "VERGINO",
                                      "VERGIDAIRESI", "ADRES", "TELEFON", "FAKS", "EPOSTA", "URL", "ANTET", "YERMI", "HARCAMAMI",
                                      "BUTCEMI", "TASINIRMI", "PERSONELMI", "EVRAKMI", "ULKEMI", "BOLGEMI", "ILMI", "ILCEMI",
                                      "ILETISIMMI", "KOPRUKOD", "YONETICI", "SIRANO", "SABLONMU", "FONKOD", "MERKEZKOD", "USTYONETICI1",
                                      "USTYONETICI2", "TURKOD", "CIZIMCIKISNOKTASI", "CIZIMDEGORUNME", "KURUMMU", "KURUMKOD", "AMBARMI",
                                      "AMBARKOD", "YONETICITCKIMLIK", "USTYONETICITCKIMLIK1", "USTYONETICITCKIMLIK2", "ENTEGREKOD", "ALTFAALKODLAR" };


        public IDataReader Il_IlceBul(string ilKod, string ilceKod, int aktifYil)
        {
            string sql = "SELECT DISTINCT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", ";
            sql += "(SELECT " + VeriErisim.IslecBul("COUNT") + "(1) FROM " + TABLOAD + " IC";
            sql += " WHERE IC." + ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            string kosul = "";
            if (string.IsNullOrEmpty(ilceKod))
            {
                VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "ILCE", "=", ilceKod));
                VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "ILCEMI", "=", 1));
            }
            else if (string.IsNullOrEmpty(ilKod))
            {
                VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "IL", "=", ilKod));
                VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "ILMI", "=", 1));
            }
            else
                return null;

            if (aktifYil > 0)
            {
                string icKosul = "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH] + " IS NULL";
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], "<=", new TNSDateTime("31.12." + aktifYil)));
                icKosul += ")";
                icKosul = VeriErisim.KosulEkle(icKosul, "AND (", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + " IS NULL");
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], ">=", new TNSDateTime("01.01." + aktifYil)));
                icKosul += ")";

                kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.SIRANO];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ADI];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Listeleme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="tcKimlikNo">Organizasyon birim tipinde sınıf</param>
        /// <param name="aktifYil">Organizasyon birim tipinde sınıf</param>
        /// <returns>IDataReader tipinde sınıf döndürür</returns>
        public IDataReader ListeleKullanici(string tcKimlikNo, int aktifYil)
        {
            string sql = "SELECT DISTINCT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", ";
            sql += "(SELECT " + VeriErisim.IslecBul("COUNT") + "(1) FROM " + TABLOAD + " IC";
            sql += " WHERE IC." + ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD + ", " + "GRVGENELGOREVPERSONEL";

            string kosul = TABLOAD + ".KOD=" + "GRVGENELGOREVPERSONEL.ORGKOD";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("GRVGENELGOREVPERSONEL", "TCKIMLIKNO", "=", tcKimlikNo));

            if (aktifYil > 0)
            {
                string icKosul = "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH] + " IS NULL";
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], "<=", new TNSDateTime("31.12." + aktifYil)));
                icKosul += ")";
                icKosul = VeriErisim.KosulEkle(icKosul, "AND (", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + " IS NULL");
                icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], ">=", new TNSDateTime("01.01." + aktifYil)));
                icKosul += ")";

                kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.SIRANO];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ADI];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Listeleme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Organizasyon birim tipinde sınıf</param>
        /// <returns>IDataReader tipinde sınıf döndürür</returns>
        public IDataReader Listele(OrganizasyonBirim p, bool esitle, int ustVeAyniSeviye)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", ";
            sql += "(SELECT " + VeriErisim.IslecBul("COUNT") + "(1) FROM " + TABLOAD + " IC";
            sql += " WHERE IC." + ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            else
            {
                if (!string.IsNullOrEmpty(p.adi))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADI], "LIKE", p.adi + "%"));
                else if (!string.IsNullOrEmpty(p.adlarSorgu))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.OR_KosulOl(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ADI], p.adlarSorgu, true, false, 3));
                }

                if (p.anaBirimKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD], "=", p.anaBirimKod));
                else if (p.anaBirimKod == -1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD], "=", 0));

                if (p.listeleAktifYil > 0)
                {
                    string icKosul = "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH] + " IS NULL";
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], "<=", new TNSDateTime("31.12." + p.listeleAktifYil)));
                    icKosul += ")";
                    icKosul = VeriErisim.KosulEkle(icKosul, "AND (", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + " IS NULL");
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], ">=", new TNSDateTime("01.01." + p.listeleAktifYil)));
                    icKosul += ")";

                    kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
                }
                else if (p.baslangicTarih != null && p.bitisTarih != null)
                {
                    if (p.baslangicTarih.isNull && p.bitisTarih.isNull)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + " IS NULL");
                    else
                    {
                        if (!p.baslangicTarih.isNull)
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], "<=", p.baslangicTarih));
                        if (!p.bitisTarih.isNull)
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], ">=", p.bitisTarih));
                    }
                }

                string islec = "LIKE";
                string ekLike = "%";
                if (esitle)
                {
                    islec = "=";
                    ekLike = string.Empty;
                }

                int i_muhasebeKodu = OrtakFonksiyonlar.ConvertToInt(p.muhasebekodu, 0);
                if (i_muhasebeKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", i_muhasebeKodu));

                if (OrtakFonksiyonlar.ConvertToInt(p.muhasebebirim, 0) > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEBIRIM], "=", p.muhasebebirim));

                if (!string.IsNullOrEmpty(p.yetkiliOlduguMuhasebeBirim))
                {
                    string icKosul = "";
                    string[] yler = p.yetkiliOlduguMuhasebeBirim.Split(';');
                    foreach (string y in yler)
                    {
                        if (!string.IsNullOrEmpty(y))
                        {
                            string[] mler = y.Split('-');
                            if (mler.Length == 2)
                            {
                                string icickosul = "";
                                if (OrtakFonksiyonlar.ConvertToInt(mler[0], 0) > 0)
                                    icickosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], "=", mler[0]);
                                if (OrtakFonksiyonlar.ConvertToInt(mler[1], 0) > 0)
                                    icickosul = VeriErisim.KosulEkle(icickosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEBIRIM], "=", mler[1]));
                                if (icickosul != "")
                                {
                                    if (icKosul != "") icKosul += " OR ";
                                    icKosul += "(" + icickosul + ")";
                                }
                            }
                        }
                    }
                    if (icKosul != "")
                    {
                        if (kosul != "") kosul += " AND ";
                        kosul += "(" + icKosul + ")";
                    }
                }

                if (!string.IsNullOrEmpty(p.evrakbirim))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIM], islec, p.evrakbirim + ekLike));

                if (!string.IsNullOrEmpty(p.ulke))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ULKE], "=", p.ulke.Replace(".", "")));

                if (!string.IsNullOrEmpty(p.bolge))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BOLGE], "=", p.bolge.Replace(".", "")));

                if (!string.IsNullOrEmpty(p.il))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IL], "=", p.il.Replace(".", "")));

                if (!string.IsNullOrEmpty(p.ilce))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCE], "=", p.ilce.Replace(".", "")));

                if (!string.IsNullOrEmpty(p.kurumsalkod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSALKOD], islec, p.kurumsalkod.Replace(".", "") + ekLike));

                if (!string.IsNullOrEmpty(p.fonKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], islec, p.fonKod.Replace(".", "") + ekLike));

                if (!string.IsNullOrEmpty(p.kisaadi))
                {
                    if (kosul != "") kosul += " AND ";

                    kosul += " (";
                    kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KISAADI], islec, p.kisaadi + ekLike));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADI], islec, p.kisaadi + ekLike));
                    kosul += ")";
                }

                string miKosul = string.Empty;
                if (p.yerMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YERMI], "=", p.yerMi));

                if (p.harcamaMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMAMI], "=", p.harcamaMi));

                if (p.butceMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BUTCEMI], "=", p.butceMi));

                if (p.tasinirMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASINIRMI], "=", p.tasinirMi));

                if (p.personelMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERSONELMI], "=", p.personelMi));

                if (p.evrakMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKMI], "=", p.evrakMi));

                if (p.ulkeMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ULKEMI], "=", p.ulkeMi));

                if (p.bolgeMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BOLGEMI], "=", p.bolgeMi));

                if (p.ilMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILMI], "=", p.ilMi));

                if (p.ilceMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILCEMI], "=", p.ilceMi));

                if (p.iletisimMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILETISIMMI], "=", p.iletisimMi));

                if (p.kurumMu > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMMU], "=", p.kurumMu));

                if (p.ambarMi > 0)
                    miKosul = VeriErisim.KosulEkle(miKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARMI], "=", p.ambarMi));

                if (!string.IsNullOrEmpty(miKosul))
                    kosul = VeriErisim.KosulEkle(kosul, "AND (", miKosul + ")");

                if (ustVeAyniSeviye > 0 && !(string.IsNullOrEmpty(p.kopruKod) || p.kopruKod.Length <= 3))
                {
                    string ekKopruKosul = string.Empty;
                    for (int i = 3; i <= p.kopruKod.Length; i += 3)
                    {
                        if (!string.IsNullOrEmpty(ekKopruKosul))
                            ekKopruKosul += ", ";
                        ekKopruKosul += "'" + p.kopruKod.Substring(0, i) + "'";
                    }

                    if (!string.IsNullOrEmpty(ekKopruKosul))
                    {
                        ekKopruKosul = "(" + ekKopruKosul + ")";
                        kosul = VeriErisim.KosulEkle(kosul, "AND (", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + " IN " + ekKopruKosul);
                    }

                    if (ustVeAyniSeviye == 2)
                    {
                        for (int i = 3; i < p.kopruKod.Length; i += 3)
                        {
                            kosul = VeriErisim.KosulEkle(kosul, "OR (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOPRUKOD], "LIKE", p.kopruKod.Substring(0, i) + "%"));
                            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.IslecBul("LEN") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + ") = " + (i + 3) + ")");
                        }
                    }

                    if (!string.IsNullOrEmpty(ekKopruKosul))
                        kosul += ")";
                }
                else if (!(string.IsNullOrEmpty(p.kopruKod) || p.kopruKod.Equals("999")))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOPRUKOD], islec, p.kopruKod + ekLike));
                else if (!string.IsNullOrEmpty(p.kopruKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.IslecBul("LEN") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + ") = " + p.kopruKod.Replace(".", "").Length);

                if (p.siraNo > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", p.siraNo));

                if (p.kod <= 0)
                {
                    if (p.sablonMu)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABLONMU], "=", 1));
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABLONMU], "=", 0));
                }

                if (!string.IsNullOrEmpty(p.turKod))
                {
                    string turKosul = "";
                    string[] turKodlar = p.turKod.Split(',');
                    for (int i = 0; i < turKodlar.Length; i++)
                    {
                        turKosul = VeriErisim.KosulEkle(turKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TURKOD], islec, turKodlar[i].Replace(".", "") + ekLike));
                    }
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + turKosul + ")");
                }

                if (!string.IsNullOrEmpty(p.entegreKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ENTEGREKOD], "=", p.entegreKod));
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.SIRANO];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ADI];
            //OrtakFonksiyonlar.HataStrYaz(sql);
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kaydetme işlemi yapan fonksiyon
        /// </summary>
        /// <param name="p">Organizasyon birim tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc Kaydet(OrganizasyonBirim p)
        {
            string sql = string.Empty;
            string p_turKod;
            if (string.IsNullOrEmpty(p.turKod))
                p_turKod = null;
            else
                p_turKod = p.turKod.Replace(".", "");

            if (p.kod == 0)
            {
                this.KomutCalistir("SET TRANSACTION ISOLATION LEVEL READ COMMITTED", 0);
                if (!string.IsNullOrWhiteSpace(veriErisim.hata)) return new Sonuc(false, veriErisim.hata, "");
                p.kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);
                p.siraNo = YeniSiraNoVer(p.anaBirimKod);

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                sql += VeriErisim.DegerYap(p.adi) + ",";
                sql += VeriErisim.DegerYap(p.kisaadi) + ",";
                sql += VeriErisim.DegerYap(p.anaBirimKod) + ",";
                sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ",";
                sql += VeriErisim.DegerYap(new TNSDateTime()) + ",";
                sql += VeriErisim.DegerYap(p.muhasebekodu) + ",";
                sql += VeriErisim.DegerYap(p.muhasebebirim) + ",";
                sql += VeriErisim.DegerYap(p.evrakbirim) + ",";
                sql += VeriErisim.DegerYap(p.ulke.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(p.bolge.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(p.il.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(p.ilce.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(p.kurumsalkod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(p.vergino) + ",";
                sql += VeriErisim.DegerYap(p.vergidairesi) + ",";
                sql += VeriErisim.DegerYap(p.adres) + ",";
                sql += VeriErisim.DegerYap(p.telefon) + ",";
                sql += VeriErisim.DegerYap(p.faks) + ",";
                sql += VeriErisim.DegerYap(p.eposta) + ",";
                sql += VeriErisim.DegerYap(p.url) + ",";
                sql += VeriErisim.DegerYap(p.antet) + ",";
                sql += VeriErisim.DegerYap(p.yerMi) + ",";
                sql += VeriErisim.DegerYap(p.harcamaMi) + ",";
                sql += VeriErisim.DegerYap(p.butceMi) + ",";
                sql += VeriErisim.DegerYap(p.tasinirMi) + ",";
                sql += VeriErisim.DegerYap(p.personelMi) + ",";
                sql += VeriErisim.DegerYap(p.evrakMi) + ",";
                sql += VeriErisim.DegerYap(p.ulkeMi) + ",";
                sql += VeriErisim.DegerYap(p.bolgeMi) + ",";
                sql += VeriErisim.DegerYap(p.ilMi) + ",";
                sql += VeriErisim.DegerYap(p.ilceMi) + ",";
                sql += VeriErisim.DegerYap(p.iletisimMi) + ",";
                sql += VeriErisim.DegerYap(p.kopruKod) + ",";
                sql += VeriErisim.DegerYap(p.yoneticiKod) + ",";
                sql += VeriErisim.DegerYap(p.siraNo) + ",";
                sql += VeriErisim.DegerYap(p.sablonMu ? 1 : 0) + ",";
                sql += VeriErisim.DegerYap(p.fonKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(p.merkezKod) + ",";
                sql += VeriErisim.DegerYap(p.ustYoneticiKod1) + ",";
                sql += VeriErisim.DegerYap(p.ustYoneticiKod2) + ",";
                sql += VeriErisim.DegerYap(p_turKod) + ",";
                sql += VeriErisim.DegerYap(p.cizimCikisNoktasi ? 1 : 0) + ",";
                sql += VeriErisim.DegerYap(p.cizimdeGorunme ? 1 : 0) + ",";
                sql += VeriErisim.DegerYap(p.kurumMu) + ",";
                sql += VeriErisim.DegerYap(p.kurumKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(p.ambarMi) + ",";
                sql += VeriErisim.DegerYap(p.ambarKod) + ",";
                sql += VeriErisim.DegerYap(p.yoneticiTCKimlik) + ",";
                sql += VeriErisim.DegerYap(p.ustYoneticiTCKimlik1) + ",";
                sql += VeriErisim.DegerYap(p.ustYoneticiTCKimlik2) + ",";
                sql += VeriErisim.DegerYap(p.entegreKod) + ",";
                sql += VeriErisim.DegerYap(p.altFaaliyetKodlar) + ")";
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], p.adi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISAADI], p.kisaadi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU], p.muhasebekodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEBIRIM], p.muhasebebirim) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIM], p.evrakbirim) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ULKE], p.ulke.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOLGE], p.bolge.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IL], p.il.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCE], p.ilce.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMSALKOD], p.kurumsalkod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGINO], p.vergino) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIDAIRESI], p.vergidairesi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], p.adres) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], p.telefon) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], p.faks) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], p.eposta) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.URL], p.url) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANTET], p.antet) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERMI], p.yerMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMAMI], p.harcamaMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCEMI], p.butceMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINIRMI], p.tasinirMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PERSONELMI], p.personelMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKMI], p.evrakMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ULKEMI], p.ulkeMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOLGEMI], p.bolgeMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILMI], p.ilMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEMI], p.ilceMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILETISIMMI], p.iletisimMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOPRUKOD], p.kopruKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YONETICI], p.yoneticiKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SABLONMU], p.sablonMu ? 1 : 0) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], p.fonKod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MERKEZKOD], p.merkezKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.USTYONETICI1], p.ustYoneticiKod1) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.USTYONETICI2], p.ustYoneticiKod2) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURKOD], p_turKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIZIMCIKISNOKTASI], p.cizimCikisNoktasi ? 1 : 0) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CIZIMDEGORUNME], p.cizimdeGorunme ? 1 : 0) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMMU], p.kurumMu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], p.kurumKod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARMI], p.ambarMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], p.ambarKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YONETICITCKIMLIK], p.yoneticiTCKimlik) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.USTYONETICITCKIMLIK1], p.ustYoneticiTCKimlik1) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.USTYONETICITCKIMLIK2], p.ustYoneticiTCKimlik2);
                if (!string.IsNullOrEmpty(p.entegreKod))
                    sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ENTEGREKOD], p.entegreKod);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTFAALKODLAR], p.altFaaliyetKodlar);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            }

            Sonuc sonuc = KomutCalistir(sql, 1);
            if (sonuc.islemSonuc)
                sonuc.anahtar = p.kod.ToString();

            return sonuc;
        }

        public bool KayitVarmi(int orgBirimKod, int anaBirimKod)
        {
            return TNS.Veri.Sayac.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0],
                new string[] { ALANLAR[(int)ENUMAlanlar.KOD], ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD] }, new int[] { orgBirimKod, anaBirimKod });
        }

        public bool KayitVarmi(ENUMORGBirimTur orgBirimTur, OrganizasyonBirim ob)
        {
            TNSCollection intAlan = new TNSCollection();
            TNSCollection intDeger = new TNSCollection();
            TNSCollection strAlan = new TNSCollection();
            TNSCollection strDeger = new TNSCollection();

            intAlan.Add(ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD]); intDeger.Add(ob.anaBirimKod);

            if (orgBirimTur == ENUMORGBirimTur.HARCAMA || orgBirimTur == ENUMORGBirimTur.TASINIR)
            {
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU]); strDeger.Add(ob.muhasebekodu);
                if (ob.orgHarcamaKod.Length <= 0 || (string.IsNullOrEmpty(ob.orgHarcamaKod[0].muhasebeBirimKod) && string.IsNullOrEmpty(ob.orgHarcamaKod[0].kurKod)))
                    return false;

                strAlan.Add(ALANLAR[(int)ENUMAlanlar.MUHASEBEBIRIM]); strDeger.Add(ob.orgHarcamaKod[0].muhasebeBirimKod);
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.KURUMSALKOD]); strDeger.Add(ob.orgHarcamaKod[0].kurKod.Replace(".", ""));

                if (orgBirimTur == ENUMORGBirimTur.HARCAMA)
                {
                    intAlan.Add(ALANLAR[(int)ENUMAlanlar.HARCAMAMI]); intDeger.Add(1);
                }
                else if (orgBirimTur == ENUMORGBirimTur.TASINIR)
                {
                    intAlan.Add(ALANLAR[(int)ENUMAlanlar.TASINIRMI]); intDeger.Add(1);
                }
            }
            else if (orgBirimTur == ENUMORGBirimTur.ULKE)
            {
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.ULKE]); strDeger.Add(ob.ulke.Replace(".", ""));
                intAlan.Add(ALANLAR[(int)ENUMAlanlar.ULKEMI]); intDeger.Add(1);
            }
            else if (orgBirimTur == ENUMORGBirimTur.BOLGE)
            {
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.BOLGE]); strDeger.Add(ob.bolge.Replace(".", ""));
                intAlan.Add(ALANLAR[(int)ENUMAlanlar.BOLGEMI]); intDeger.Add(1);
            }
            else if (orgBirimTur == ENUMORGBirimTur.IL)
            {
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.IL]); strDeger.Add(ob.il.Replace(".", ""));
                intAlan.Add(ALANLAR[(int)ENUMAlanlar.ILMI]); intDeger.Add(1);
            }
            else if (orgBirimTur == ENUMORGBirimTur.ILCE)
            {
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.ILCE]); strDeger.Add(ob.ilce.Replace(".", ""));
                intAlan.Add(ALANLAR[(int)ENUMAlanlar.ILCEMI]); intDeger.Add(1);
            }
            else if (orgBirimTur == ENUMORGBirimTur.KURUM)
            {
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.KURUMKOD]); strDeger.Add(ob.kurumKod.Replace(".", ""));
                intAlan.Add(ALANLAR[(int)ENUMAlanlar.KURUMMU]); intDeger.Add(1);
            }
            else if (orgBirimTur == ENUMORGBirimTur.AMBAR)
            {
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.MUHASEBEKODU]); strDeger.Add(ob.muhasebekodu);
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.MUHASEBEBIRIM]); strDeger.Add(ob.muhasebebirim);
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.KURUMSALKOD]); strDeger.Add(ob.kurumsalkod.Replace(".", ""));
                strAlan.Add(ALANLAR[(int)ENUMAlanlar.AMBARKOD]); strDeger.Add(ob.ambarKod);
                intAlan.Add(ALANLAR[(int)ENUMAlanlar.AMBARMI]); intDeger.Add(1);
            }

            int sonKayitKod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], (string[])strAlan.ToArray(typeof(string)), (string[])strDeger.ToArray(typeof(string)), (string[])intAlan.ToArray(typeof(string)), (int[])intDeger.ToArray(typeof(int))) - 1;
            if (ob.kod != sonKayitKod && sonKayitKod != 0)
                return true;

            return false;
        }

        public IDataReader GrafikBirimleriniBul(string kopruKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLAR) + " FROM " + TABLOAD + " A";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.KOPRUKOD], "LIKE", kopruKod.Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.CIZIMDEGORUNME], "=", 0));
            sql = VeriErisim.KosulEkle(sql, "AND", "A." + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + " IS NULL");//Kapalı birimler grafikte görünmesin diye eklendi.

            string icSQL = "SELECT 1 FROM " + TABLOAD + " B";
            icSQL = VeriErisim.KosulEkle(icSQL, "WHERE", "A." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + " LIKE B." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + " " + VeriErisim.IslecBul("+") + " '%'");
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.IslecBul("LEN") + "(A." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + ") > " + VeriErisim.IslecBul("LEN") + "(B." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + ")");
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", ALANLAR[(int)ENUMAlanlar.KOPRUKOD], "LIKE", kopruKod.Replace(".", "") + "%"));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", ALANLAR[(int)ENUMAlanlar.KOPRUKOD], "<>", kopruKod.Replace(".", "")));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", ALANLAR[(int)ENUMAlanlar.CIZIMCIKISNOKTASI], "=", 1));

            sql = VeriErisim.KosulEkle(sql, "AND", "NOT EXISTS (" + icSQL + ")");

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOPRUKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc BirimAc(int kod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.BITISTARIH] + " = NULL";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc BirimKapat(int kod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], new TNSDateTime(DateTime.Now));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc Sil(OrganizasyonBirim p)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            return KomutCalistir(sql, 1);
        }

        public string KopruKodBul(int kod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return veriErisim.TekKolonSQLStr(sql);
        }

        public string MaksKopruKodBulFromCocuk(int kod, int anaBirimKod)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("MAX") + "(" + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD], "=", anaBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "<>", kod));
            return veriErisim.TekKolonSQLStr(sql);
        }

        public Sonuc KopruKodGuncelle(int anaBirimKod, string anaKopruKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + " = '" + anaKopruKod + "' + "
                + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLAR[(int)ENUMAlanlar.KOPRUKOD]
                + ", " + VeriErisim.IslecBul("LEN") + "(" + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + ") - 2, 3)";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD], "=", anaBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOPRUKOD], "NOT LIKE", anaKopruKod + "%"));
            return KomutCalistir(sql, 0);
        }

        public Sonuc KopruKodGuncelleBirim(int birimKod, string kopruKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + " = " + VeriErisim.DegerYap(kopruKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "=", birimKod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc AnaBirimDegistir(int kod, int anaBirimKod, string kopruKod, int siraNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD], anaBirimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOPRUKOD], kopruKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], siraNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return KomutCalistir(sql, 1);
        }

        public int YeniSiraNoVer(int anaBirimKod)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("MAX") + "(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ")" + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD], "=", anaBirimKod));
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public Sonuc SiraNoBirDegistir(int kod, int anaBirimKod, int basSira, int bitSira, bool tasinan, bool arttir)
        {
            int degisecekKayitSayisi = 0;

            string degisim = " - 1";
            if (arttir)
                degisim = " + 1";

            string sql = "UPDATE " + TABLOAD + " SET ";
            if (tasinan)
            {
                degisecekKayitSayisi = 1;

                if (arttir)
                    sql += ALANLAR[(int)ENUMAlanlar.SIRANO] + " = " + basSira;
                else
                    sql += ALANLAR[(int)ENUMAlanlar.SIRANO] + " = " + bitSira;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            }
            else
            {
                sql += ALANLAR[(int)ENUMAlanlar.SIRANO] + " = " + ALANLAR[(int)ENUMAlanlar.SIRANO] + degisim;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANABIRIMKOD], "=", anaBirimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "<>", kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], ">=", basSira));
                if (bitSira > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "<=", bitSira));
            }

            return KomutCalistir(sql, degisecekKayitSayisi);
        }

        public IDataReader UlkedeBirimTurSayisiBul(string[] turKodlar)
        {
            string sql = "SELECT COUNT(*) SAY FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", BirimKosulOl("TURKOD", turKodlar));

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IllerdeBirimTurSayisiBul(string[] turKodlar, string[] ilKodlar)
        {
            string sql = "SELECT B.IL, B.KOD, B.ADI, COUNT(*) SAY FROM ORGBIRIM B, ORGBIRIM B2";
            sql = VeriErisim.KosulEkle(sql, "WHERE", "B.ILMI = 1");
            sql = VeriErisim.KosulEkle(sql, "AND", "B2.KOPRUKOD LIKE B.KOPRUKOD " + VeriErisim.IslecBul("+") + " '%' ");
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.IslecBul("LEN") + "(B2.KOPRUKOD) > " + VeriErisim.IslecBul("LEN") + "(B.KOPRUKOD)");
            sql = VeriErisim.KosulEkle(sql, "AND", BirimKosulOl("B2.TURKOD", turKodlar));
            sql = VeriErisim.KosulEkle(sql, "AND", BirimKosulOl("B.IL", ilKodlar));
            sql += " GROUP BY B.IL, B.KOD, B.ADI";
            sql += " ORDER BY SAY, B.ADI";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IlcelerdeBirimTurSayisiBul(string[] turKodlar, string[] ilKodlar)
        {
            string sql = "SELECT B.ILCE, B.KOD, B.ADI, COUNT(*) SAY FROM ORGBIRIM B, ORGBIRIM B2";
            sql = VeriErisim.KosulEkle(sql, "WHERE", "B.ILCEMI = 1");
            sql = VeriErisim.KosulEkle(sql, "AND", "B2.KOPRUKOD LIKE B.KOPRUKOD " + VeriErisim.IslecBul("+") + " '%' ");
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.IslecBul("LEN") + "(B2.KOPRUKOD) > " + VeriErisim.IslecBul("LEN") + "(B.KOPRUKOD)");
            sql = VeriErisim.KosulEkle(sql, "AND", BirimKosulOl("B2.TURKOD", turKodlar));
            sql = VeriErisim.KosulEkle(sql, "AND", BirimKosulOl("B.IL", ilKodlar));
            sql += " GROUP BY B.ILCE, B.KOD, B.ADI";
            sql += " ORDER BY SAY, B.ADI";

            return veriErisim.DataReaderDondur(sql);
        }

        private string BirimKosulOl(string alanAd, string[] turKodlar)
        {
            string donusDeger = "";
            if (turKodlar == null || turKodlar.Length == 0) return donusDeger;
            for (int i = 0; i < turKodlar.Length; i++)
            {
                if (!string.IsNullOrEmpty(donusDeger)) donusDeger += ", ";
                donusDeger += VeriErisim.DegerYap(turKodlar[i].Replace(".", ""));
            }
            donusDeger = alanAd + " IN (" + donusDeger + ")";
            return donusDeger;
        }
    }
}
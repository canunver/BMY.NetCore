using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHASEBAT, ABSGELIRGERCEKLESEN, ABSBUTCEGELIR, MUHODENEKUSTU, MUHMUHASEBATKONSOLIDE, ISLHARCAMA, ABSBUTCEGIDER veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOdenekHarcamaSorgu : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOdenekHarcamaSorgu(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOADBUTCE = "ABSBUTCEGIDER";
        static string TABLOADHARCAMA = "ISLHARCAMA";
        static string TABLOADMUHASEBE = "MUHMUHASEBATKONSOLIDE";
        static string TABLOADODENEKUSTU = "MUHODENEKUSTU";
        static string TABLOADGELIR = "ABSBUTCEGELIR";
        static string TABLOADGELIRGERCEKLESEN = "ABSGELIRGERCEKLESEN";
        //static string TABLOADAVANS = "ISLAVANS";
        static string TABLOADMUHASEBAT = "MUHASEBAT";

        public static string muhasebatTur = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebatTur") + "";

        enum ENUMAlanlarButce
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4,
            KANUN = 5,
            KBO = 6,
            EKLENEN = 7,
            DUSULEN = 8,
            SERBEST = 9,
            EKLENEN_TASLAK = 10,
            DUSULEN_TASLAK = 11,
            SERBEST_TASLAK = 12,
            PEBKOD = 13
        }
        static string[] ALANLARBUTCE = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "KANUN", "KBO", "EKLENEN", "DUSULEN", "SERBEST", "EKLENEN_TASLAK", "DUSULEN_TASLAK", "SERBEST_TASLAK", "PEBKOD" };

        enum ENUMAlanlarHarcama
        {
            YIL = 0,
            AY = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            KURKOD = 4,
            FONKOD = 5,
            FINKOD = 6,
            EKOKOD = 7,
            HARCAMA = 8,
            PEBKOD = 9,
            HESAPKOD = 10
        }
        static string[] ALANLARHARCAMA = { "YIL", "AY", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "HARCAMA", "PEBKOD", "HESAPKOD" };

        enum ENUMAlanlarGON
        {
            YIL = 0,
            BELGETUR = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            KURKOD = 4,
            FONKOD = 5,
            FINKOD = 6,
            EKOKOD = 7,
            MIKTAR = 8,
            FORMDURUM = 9
        }
        static string[] ALANLARGON = { "YIL", "BELGETUR", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "MIKTAR", "FORMDURUM" };

        enum ENUMAlanlarMuhasebe
        {
            YIL = 0,
            AY = 1,
            KURUMKOD = 2,
            HESAPKOD = 3,
            KURKOD = 4,
            FONKOD = 5,
            FINKOD = 6,
            EKOKOD = 7,
            BORC = 8,
            ALACAK = 9
        }
        static string[] ALANLARMUHASEBE = { "YIL", "AY", "KURUMKOD", "HESAPKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "BORC", "ALACAK" };

        enum ENUMAlanlarOdenekUstu
        {
            YIL = 0,
            AY = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            KURKOD = 4,
            FONKOD = 5,
            FINKOD = 6,
            EKOKOD = 7,
            ODENEK = 8,
            HARCAMA = 9,
            KALAN = 10,
        }
        static string[] ALANLARODENEKUSTU = { "YIL", "AY", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEK", "HARCAMA", "KALAN" };

        enum ENUMAlanlarGelir
        {
            YIL = 0,
            KURKOD = 1,
            EKOKOD = 2,
            KBO = 3
        }
        static string[] ALANLARGELIR = { "YIL", "KURKOD", "EKOKOD", "KBO" };

        enum ENUMAlanlarGelirGerceklesen
        {
            YIL = 0,
            AY = 1,
            MUHASEBEKOD = 2,
            BIRIM = 3,
            KURKOD = 4,
            EKOKOD = 5,
            GELIR = 6
        }
        static string[] ALANLARGELIRGERCEKLESEN = { "YIL", "AY", "MUHASEBEKOD", "BIRIM", "KURKOD", "EKOKOD", "GELIR" };

        enum ENUMAlanlarMuhasebat
        {
            YIL = 0,
            AY = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            HESAPKOD = 4,
            KURKOD = 5,
            FONKOD = 6,
            FINKOD = 7,
            EKOKOD = 8,
            BORC = 9,
            ALACAK = 10
        }
        static string[] ALANLARMUHASEBAT = { "YIL", "AY", "MUHASEBEKOD", "BIRIMKOD", "HESAPKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "BORC", "ALACAK" };

        /// <summary>
        /// Kullanıcıdan alınan değerleri kullanarak borç ve alacak miktarını hasaplar.
        /// </summary>
        /// <param name="bmd">İçersinde yil,ay,kurumKod,kurKod,fonKod,finKod,ekoKod,hesapKod,borc,alacak
        /// parametrelerini bulundurur. </param>
        /// <returns>Elde edilen değerler data reader olarak döndürülür.</returns>
        public IDataReader MuhasebeListe(ButceMuhasebe bmd)
        {
            string kosul = VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[0], "=", bmd.yil);

            if (bmd.ay == 0)
                bmd.ay = DateTime.Now.Month;

            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[1], "=", bmd.ay));

            if (bmd.kurumKod != "")
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[2], "=", bmd.kurumKod));

            if (bmd.hesapKod != "")
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[3], " LIKE ", bmd.hesapKod + "%"));

            string kur = bmd.kurKod;
            string kur2 = "";
            if (kur != "")
            {
                if (kur.Length == 2)
                    kur2 = kur + OrtakFonksiyonlar.BagimsizKurumBaslangic();

                string[] kurlar = kur.Split('/');
                if (kurlar.Length > 1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[4], ">= ", kurlar[0].Replace(".", "")));
                    kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[4], "< ", kurlar[1].Replace(".", "")));
                }
                else
                    kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[4], " LIKE ", kurlar[0].Replace(".", "")));

                if (kur2 != "")
                    kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[4], "<", kur2));

            }
            if (bmd.fonKod != "")
            {
                string ekKosul = "";
                string[] kodlar = bmd.fonKod.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[5], " LIKE ", kodlar[i].Replace(".", "") + "%"));
                }

                kosul += " AND (" + ekKosul + ")";
            }
            if (bmd.finKod != "")
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[6], " LIKE ", bmd.finKod.Replace(".", "") + "%"));
            if (bmd.ekoKod != "")
            {
                string ekKosul = "";
                string[] kodlar = bmd.ekoKod.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "", VeriErisim.KosulOl(TABLOADMUHASEBE, ALANLARMUHASEBE[7], " LIKE ", kodlar[i].Replace(".", "") + "%"));
                }

                kosul += " AND (" + ekKosul + ")";
            }

            string sql = "SELECT sum(BORC),sum(ALACAK) from " + TABLOADMUHASEBE + " where " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }


        public static string TertipKosulOl(string sql, BKD.Tertip tertip, string tabloAd, string kurAlanAd, string fonAlanAd, string finAlanAd, string ekoAlanAd, string pebAlanAd)
        {
            string kur = "";
            string kur2 = "";
            string fon = "";
            string fin = "";
            string eko = "";
            string peb = "";

            if (tertip.kodlar.Length > 0)
            {
                kur = ((string)tertip.kodlar[0]).Replace(".", "").Trim();
                if (kur.Length == 2)
                    kur2 = kur + OrtakFonksiyonlar.BagimsizKurumBaslangic();
            }
            if (tertip.kodlar.Length > 1)
                fon = ((string)tertip.kodlar[1]).Replace(".", "").Trim();
            if (tertip.kodlar.Length > 2)
                fin = ((string)tertip.kodlar[2]).Replace(".", "").Trim();
            if (tertip.kodlar.Length > 3)
                eko = ((string)tertip.kodlar[3]).Replace(".", "").Trim();
            if (tertip.kodlar.Length > 4)
                peb = ((string)tertip.kodlar[4]).Replace(".", "").Trim();

            if (kur != "")
            {
                string[] kurlar = kur.Split('/');
                if (kurlar.Length > 1)
                {
                    sql += " AND " + VeriErisim.KosulOl(tabloAd, kurAlanAd, ">=", kurlar[0].Replace(".", ""));
                    sql += " AND " + VeriErisim.KosulOl(tabloAd, kurAlanAd, "<", kurlar[1].Replace(".", ""));
                }
                else
                {
                    if (kur2 != "")
                        sql += " AND " + VeriErisim.KosulOl(tabloAd, kurAlanAd, " >= ", kur + "");
                    else
                        sql += " AND " + VeriErisim.KosulOl(tabloAd, kurAlanAd, " LIKE ", kur + "%");
                }

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(tabloAd, kurAlanAd, " < ", kur2);
            }

            if (fon != "")
            {
                string ekKosul = "";
                string[] kodlar = fon.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(tabloAd, fonAlanAd, " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }

            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl(tabloAd, finAlanAd, " LIKE ", fin + "%");

            if (eko != "")
            {
                string ekKosul = "";
                string[] kodlar = eko.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(kodlar[i])) continue;
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(tabloAd, ekoAlanAd, " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }

            if (peb != "")
            {
                string ekKosul = "";
                string[] kodlar = peb.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(tabloAd, pebAlanAd, " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }
            return sql;
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kr">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader OdenekListe(OdenekDurumKriter kr)
        {
            string sql = "SELECT ";
            string groupByAlanlar = "";
            if (!kr.konsolide)
            {
                groupByAlanlar = "YIL, KURKOD, FONKOD, FINKOD, ";
                groupByAlanlar += BKD.Arac.EkoAlanOl(kr.yil, "", ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD], kr.ekoKodKirilim, kr.eko23, VeriErisim.IslecBul("SUBSTRING"));

                if (kr.faaliyetGoster)
                    groupByAlanlar += ",PEBKOD";

                sql += groupByAlanlar + ", ";
            }
            sql += "SUM(" + ALANLARBUTCE[(int)ENUMAlanlarButce.KANUN] + "),";
            sql += "SUM(" + ALANLARBUTCE[(int)ENUMAlanlarButce.KBO] + "),";
            sql += "SUM(" + ALANLARBUTCE[(int)ENUMAlanlarButce.EKLENEN] + "),";
            sql += "SUM(" + ALANLARBUTCE[(int)ENUMAlanlarButce.DUSULEN] + "),";
            sql += "SUM(" + ALANLARBUTCE[(int)ENUMAlanlarButce.SERBEST] + "),";
            sql += "SUM(" + ALANLARBUTCE[(int)ENUMAlanlarButce.EKLENEN_TASLAK] + "),";
            sql += "SUM(" + ALANLARBUTCE[(int)ENUMAlanlarButce.DUSULEN_TASLAK] + "),";
            sql += "SUM(" + ALANLARBUTCE[(int)ENUMAlanlarButce.SERBEST_TASLAK] + ")";

            sql += " FROM " + TABLOADBUTCE + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADBUTCE, ALANLARBUTCE[(int)ENUMAlanlarButce.YIL], "=", kr.yil);

            sql = TertipKosulOl(sql, kr.tertip, TABLOADBUTCE, ALANLARBUTCE[(int)ENUMAlanlarButce.KURKOD], ALANLARBUTCE[(int)ENUMAlanlarButce.FONKOD], ALANLARBUTCE[(int)ENUMAlanlarButce.FINKOD], ALANLARBUTCE[(int)ENUMAlanlarButce.EKOKOD], ALANLARBUTCE[(int)ENUMAlanlarButce.PEBKOD]);

            if (!kr.konsolide)
            {
                sql += " GROUP BY " + groupByAlanlar;
                sql += " ORDER BY 1,2,3,4,5";
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kr">Kriter bilgisi.</param>
        /// <param name="avans">true ise sadece avanslar döndürülür.</param>
        /// <returns></returns>
        public IDataReader HarcamaListe(OdenekDurumKriter kr, bool avans)
        {
            if (kr.ay == 0)
            {
                if (kr.yil == DateTime.Now.Year)
                    kr.ay = DateTime.Now.Month;
                else
                    kr.ay = 12;
            }

            string sql = "SELECT ";
            string alanlar = "";

            if (kr.detay == ENUMOdenekDurumDetay.MUHASEBE)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD];
            }
            else if (kr.detay == ENUMOdenekDurumDetay.BIRIM)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD];
                alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.BIRIMKOD];
            }
            else if (kr.detay == ENUMOdenekDurumDetay.EKO1BASAMAK)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD];
                alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.BIRIMKOD];
                alanlar += "," + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD] + ",1,2)";
            }

            if (!kr.konsolide)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD];
                alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FONKOD];
                alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FINKOD];

                alanlar += "," + BKD.Arac.EkoAlanOl(kr.yil, "", ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD], kr.ekoKodKirilim, kr.eko23, VeriErisim.IslecBul("SUBSTRING"));

                if (kr.harcamaPebli)
                    alanlar += "," + "PEBKOD";
            }
            sql += alanlar;
            if (alanlar != "") sql += ",";

            sql += "SUM(" + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.HARCAMA] + ")";

            sql += " FROM " + TABLOADHARCAMA + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.YIL], "=", kr.yil);

            if (muhasebatTur == "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.AY], "=", kr.ay);
            else
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.AY], "<=", kr.ay);

            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD], "=", kr.muhasebe);

            if (kr.birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.BIRIMKOD], "=", kr.birim);

            //Sadece Gider Hesap Kodunu sorgula Ödenek Durum için sadece harcama kalemleri gerekli
            if (avans)
            {
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.HESAPKOD], "LIKE", "16%");
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.HESAPKOD], "NOT LIKE", "162%");
            }
            else
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.HESAPKOD], "LIKE", "830%");

            sql = TABOdenekHarcamaSorgu.TertipKosulOl(sql, kr.tertip, TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FONKOD], ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FINKOD], ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD], ALANLARHARCAMA[(int)ENUMAlanlarHarcama.PEBKOD]);

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kr">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader GelirGerceklesenListe(OdenekDurumKriter kr)
        {
            string sql = "";
            string kur = "";
            string kur2 = "";
            string eko = "";

            if (kr.tertip.kodlar.Length > 0)
            {
                kur = ((string)kr.tertip.kodlar[0]).Replace(".", "").Trim();
                if (kur.Length == 2)
                    kur2 = kur + OrtakFonksiyonlar.BagimsizKurumBaslangic();
            }
            if (kr.tertip.kodlar.Length > 1)
                eko = ((string)kr.tertip.kodlar[1]).Replace(".", "").Trim();

            if (kr.ay == 0)
            {
                if (kr.yil == DateTime.Now.Year)
                    kr.ay = DateTime.Now.Month;
                else
                    kr.ay = 12;
            }

            sql = "SELECT ";
            string alanlar = "";

            if (kr.detay == ENUMOdenekDurumDetay.MUHASEBE)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.MUHASEBEKOD];
            }
            else if (kr.detay == ENUMOdenekDurumDetay.BIRIM)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.MUHASEBEKOD];
                alanlar += "," + ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.BIRIM];
            }

            if (!kr.konsolide)
            {
                if (alanlar != "") alanlar += ",";
                alanlar += ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.KURKOD];
                alanlar += "," + ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.EKOKOD];
            }
            sql += alanlar;
            if (alanlar != "") sql += ",";


            sql += "SUM(" + ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.GELIR] + ")";

            sql += " FROM " + TABLOADGELIRGERCEKLESEN + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarHarcama.YIL], "=", kr.yil);

            if (muhasebatTur == "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarHarcama.AY], "=", kr.ay);
            else
                sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarHarcama.AY], "<=", kr.ay);

            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.MUHASEBEKOD], "=", kr.muhasebe);

            if (kr.birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.BIRIM], "=", kr.birim);

            if (kur != "")
            {
                string[] kurlar = kur.Split('/');
                if (kurlar.Length > 1)
                {
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.KURKOD], ">=", kurlar[0].Replace(".", ""));
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.KURKOD], "<", kurlar[1].Replace(".", ""));
                }
                else
                {
                    if (kur2 != "")
                        sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.KURKOD], " >= ", kurlar[0].Replace(".", ""));
                    else
                        sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.KURKOD], " LIKE ", kurlar[0].Replace(".", "") + "%");
                }

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.KURKOD], " < ", kur2);
            }
            if (eko != "")
            {
                string ekKosul = "";
                string[] kodlar = eko.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(TABLOADGELIRGERCEKLESEN, ALANLARGELIRGERCEKLESEN[(int)ENUMAlanlarGelirGerceklesen.EKOKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kr">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader GelirListe(OdenekDurumKriter kr)
        {
            string sql = "";
            string kur = "";
            string kur2 = "";
            string eko = "";

            if (kr.tertip.kodlar.Length > 0)
            {
                kur = ((string)kr.tertip.kodlar[0]).Replace(".", "").Trim();
                if (kur.Length == 2)
                    kur2 = kur + OrtakFonksiyonlar.BagimsizKurumBaslangic();
            }
            if (kr.tertip.kodlar.Length > 1)
                eko = ((string)kr.tertip.kodlar[1]).Replace(".", "").Trim();

            sql = "SELECT ";
            string alanlar = "";

            alanlar += ALANLARGELIR[(int)ENUMAlanlarGelir.KURKOD];
            alanlar += "," + ALANLARGELIR[(int)ENUMAlanlarGelir.EKOKOD];

            sql += alanlar;
            if (alanlar != "") sql += ",";

            sql += "SUM(" + ALANLARGELIR[(int)ENUMAlanlarGelir.KBO] + ")";

            sql += " FROM " + TABLOADGELIR + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADGELIR, ALANLARGELIR[(int)ENUMAlanlarGelir.YIL], "=", kr.yil);
            //sql += " AND " + VeriErisim.KosulOl(TABLOADGELIR, ALANLARGELIR[(int)ENUMAlanlarGelir.ASAMA], "=", "6");

            if (kur != "")
            {
                string[] kurlar = kur.Split('/');
                if (kurlar.Length > 1)
                {
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGELIR, ALANLARGELIR[(int)ENUMAlanlarGelir.KURKOD], ">=", kurlar[0].Replace(".", ""));
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGELIR, ALANLARGELIR[(int)ENUMAlanlarGelir.KURKOD], "<", kurlar[1].Replace(".", ""));
                }
                else
                {
                    if (kur2 != "")
                        sql += " AND " + VeriErisim.KosulOl(TABLOADGELIR, ALANLARGELIR[(int)ENUMAlanlarGelir.KURKOD], " >= ", kurlar[0].Replace(".", ""));
                    else
                        sql += " AND " + VeriErisim.KosulOl(TABLOADGELIR, ALANLARGELIR[(int)ENUMAlanlarGelir.KURKOD], " LIKE ", kurlar[0].Replace(".", "") + "%");
                }

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADGELIR, ALANLARGELIR[(int)ENUMAlanlarGelir.KURKOD], " < ", kur2);
            }
            if (eko != "")
            {
                string ekKosul = "";
                string[] kodlar = eko.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(TABLOADGELIR, ALANLARGELIR[(int)ENUMAlanlarGelir.EKOKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kr">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader OdenekUstuHarcama(OdenekDurumKriter kr)
        {
            string sql = "";
            string kur = "";
            string kur2 = "";
            string fon = "";
            string fin = "";
            string eko = "";

            if (kr.tertip.kodlar.Length > 0)
            {
                kur = ((string)kr.tertip.kodlar[0]).Replace(".", "").Trim();
                if (kur.Length == 2)
                    kur2 = kur + OrtakFonksiyonlar.BagimsizKurumBaslangic();
            }
            if (kr.tertip.kodlar.Length > 1)
                fon = ((string)kr.tertip.kodlar[1]).Replace(".", "").Trim();
            if (kr.tertip.kodlar.Length > 2)
                fin = ((string)kr.tertip.kodlar[2]).Replace(".", "").Trim();
            if (kr.tertip.kodlar.Length > 3)
                eko = ((string)kr.tertip.kodlar[3]).Replace(".", "").Trim();

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARODENEKUSTU);

            sql += " FROM " + TABLOADODENEKUSTU + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.YIL], "=", kr.yil);

            if (muhasebatTur == "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.AY], "=", kr.ay);
            else
                sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.AY], "<=", kr.ay);

            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.MUHASEBEKOD], "=", kr.muhasebe);

            if (kr.birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.BIRIMKOD], "=", kr.birim);

            if (kur != "")
            {
                string[] kurlar = kur.Split('/');
                if (kurlar.Length > 1)
                {
                    sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.KURKOD], ">=", kurlar[0].Replace(".", ""));
                    sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.KURKOD], "<", kurlar[1].Replace(".", ""));
                }
                else
                {
                    if (kur2 != "")
                        sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.KURKOD], " >= ", kurlar[0].Replace(".", ""));
                    else
                        sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.KURKOD], " LIKE ", kurlar[0].Replace(".", "") + "%");
                }

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.KURKOD], " < ", kur2);
            }
            if (fon != "")
            {
                string ekKosul = "";
                string[] kodlar = fon.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.FONKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }
            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.FINKOD], " LIKE ", fin + "%");
            if (eko != "")
            {
                string ekKosul = "";
                string[] kodlar = eko.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(TABLOADODENEKUSTU, ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.EKOKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }

            sql += " ORDER BY " + ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.YIL] + "," + ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.MUHASEBEKOD] + "," + ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.BIRIMKOD] + "," + ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.KURKOD] + "," + ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.FONKOD] + "," + ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.FINKOD] + "," + ALANLARODENEKUSTU[(int)ENUMAlanlarOdenekUstu.EKOKOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
        static string TABLOADMPGTERTIP = "MPGMASRAFPLANITERTIP";

        public IDataReader SerbestListe(OdenekDurumKriter kr)
        {
            string sql = "SELECT ";
            string groupByAlanlar = "";
            if (!kr.konsolide)
            {
                groupByAlanlar = "YIL, KURKOD, FONKOD, FINKOD, ";
                groupByAlanlar += BKD.Arac.EkoAlanOl(kr.yil, "", ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD], kr.ekoKodKirilim, kr.eko23, VeriErisim.IslecBul("SUBSTRING"));

                if (kr.faaliyetGoster)
                    groupByAlanlar += ", PEBKOD";

                sql += groupByAlanlar + ", ";
            }
            sql += "SUM(ODENEK)";

            sql += " FROM " + TABLOADMPGTERTIP + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADMPGTERTIP, "YIL", "=", kr.yil);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMPGTERTIP, "AY", "<=", kr.ay);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMPGTERTIP, "ASAMA", "=", 5);
            sql += " AND " + VeriErisim.KosulOl(TABLOADMPGTERTIP, "DURUM", "=", 5);

            sql = TertipKosulOl(sql, kr.tertip, TABLOADMPGTERTIP, ALANLARBUTCE[(int)ENUMAlanlarButce.KURKOD], ALANLARBUTCE[(int)ENUMAlanlarButce.FONKOD], ALANLARBUTCE[(int)ENUMAlanlarButce.FINKOD], ALANLARBUTCE[(int)ENUMAlanlarButce.EKOKOD], ALANLARBUTCE[(int)ENUMAlanlarButce.PEBKOD]);

            if (!kr.konsolide)
            {
                sql += " GROUP BY " + groupByAlanlar;
                sql += " ORDER BY 1,2,3,4,5";
            }

            return veriErisim.DataReaderDondur(sql);
        }

        //public IDataReader AvansListe(OdenekDurumKriter kr)
        //{
        //    string sql = "";
        //    string kur = "";
        //    string kur2 = "";
        //    string fon = "";
        //    string fin = "";
        //    string eko = "";
        //    int ekoUzunluk = 0;
        //    if (kr.ekoKodKirilim == 2)
        //        ekoUzunluk = 3;
        //    else
        //        ekoUzunluk = 6;

        //    if (kr.tertip.kodlar.Length > 0)
        //    {
        //        kur = ((string)kr.tertip.kodlar[0]).Replace(".", "").Trim();
        //        if (kur.Length == 2)
        //            kur2 = kur + Tertip.BagimsizKurumBaslangic();
        //    }
        //    if (kr.tertip.kodlar.Length > 1)
        //        fon = ((string)kr.tertip.kodlar[1]).Replace(".", "").Trim();
        //    if (kr.tertip.kodlar.Length > 2)
        //        fin = ((string)kr.tertip.kodlar[2]).Replace(".", "").Trim();
        //    if (kr.tertip.kodlar.Length > 3)
        //        eko = ((string)kr.tertip.kodlar[3]).Replace(".", "").Trim();

        //    if (kr.ay == 0)
        //    {
        //        if (kr.yil == DateTime.Now.Year)
        //            kr.ay = DateTime.Now.Month;
        //        else
        //            kr.ay = 12;
        //    }

        //    sql = "SELECT ";
        //    string alanlar = "";

        //    if (kr.detay == ENUMOdenekDurumDetay.MUHASEBE)
        //    {
        //        if (alanlar != "") alanlar += ",";
        //        alanlar += ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD];
        //    }
        //    else if (kr.detay == ENUMOdenekDurumDetay.BIRIM)
        //    {
        //        if (alanlar != "") alanlar += ",";
        //        alanlar += ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD];
        //        alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.BIRIMKOD];
        //    }

        //    if (!kr.konsolide)
        //    {
        //        if (alanlar != "") alanlar += ",";
        //        alanlar += ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD];
        //        alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FONKOD];
        //        alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FINKOD];
        //        alanlar += "," + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD] + ",1," + (ekoUzunluk) + ")";
        //    }
        //    sql += alanlar;
        //    if (alanlar != "") sql += ",";

        //    sql += "SUM(" + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.HARCAMA] + ")";

        //    sql += " FROM " + TABLOADAVANS + " WHERE ";
        //    sql += VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.YIL], "=", kr.yil);

        //    if (muhasebatTur == "")
        //        sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.AY], "=", kr.ay);
        //    else
        //        sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.AY], "<=", kr.ay);

        //    if (kr.muhasebe > 0)
        //        sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD], "=", kr.muhasebe);

        //    if (kr.birim != "")
        //        sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.BIRIMKOD], "=", kr.birim);

        //    if (kur != "")
        //    {
        //        string[] kurlar = kur.Split('/');
        //        if (kurlar.Length > 1)
        //        {
        //            sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], ">=", kurlar[0].Replace(".", ""));
        //            sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], "<", kurlar[1].Replace(".", ""));
        //        }
        //        else
        //        {
        //            if (kur2 != "")
        //                sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], " >= ", kurlar[0].Replace(".", ""));
        //            else
        //                sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], " LIKE ", kurlar[0].Replace(".", "") + "%");
        //        }

        //        if (kur2 != "")
        //            sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], " < ", kur2);
        //    }
        //    if (fon != "")
        //    {
        //        string ekKosul = "";
        //        string[] kodlar = fon.Split(';');
        //        for (int i = 0; i < kodlar.Length; i++)
        //        {
        //            if (ekKosul != "") ekKosul += " OR ";
        //            ekKosul += VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FONKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
        //        }

        //        sql += " AND (" + ekKosul + ")";
        //    }
        //    if (fin != "")
        //        sql += " AND " + VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FINKOD], " LIKE ", fin + "%");
        //    if (eko != "")
        //    {
        //        string ekKosul = "";
        //        string[] kodlar = eko.Split(';');
        //        for (int i = 0; i < kodlar.Length; i++)
        //        {
        //            if (ekKosul != "") ekKosul += " OR ";
        //            ekKosul += VeriErisim.KosulOl(TABLOADAVANS, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
        //        }

        //        sql += " AND (" + ekKosul + ")";
        //    }

        //    if (alanlar != "")
        //    {
        //        sql += " GROUP BY " + alanlar;
        //        sql += " ORDER BY " + alanlar;
        //    }

        //    IDataReader r = veriErisim.DataReaderDondur(sql);

        //    return r;
        //}

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="kr">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader MuhasebatButceGideriFonEkoListe(OdenekDurumKriter kr, bool hesap830Var)
        {
            string sql = "";
            string kur = "";
            string kur2 = "";
            string fon = "";
            string fin = "";
            string eko = "";

            if (kr.tertip.kodlar.Length > 0)
            {
                kur = ((string)kr.tertip.kodlar[0]).Replace(".", "").Trim();
                if (kur.Length == 2)
                    kur2 = kur + OrtakFonksiyonlar.BagimsizKurumBaslangic();
            }
            if (kr.tertip.kodlar.Length > 1)
                fon = ((string)kr.tertip.kodlar[1]).Replace(".", "").Trim();
            if (kr.tertip.kodlar.Length > 2)
                fin = ((string)kr.tertip.kodlar[2]).Replace(".", "").Trim();
            if (kr.tertip.kodlar.Length > 3)
                eko = ((string)kr.tertip.kodlar[3]).Replace(".", "").Trim();

            sql = "SELECT " + ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.YIL] + "," + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.HESAPKOD] + ",1,11)";

            if (hesap830Var)
                sql += "," + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.FONKOD] + ",1,4)";
            else
                sql += ",''";

            sql += ",sum(" + ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.BORC] + "),sum(" + ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.ALACAK] + ")";

            sql += " FROM " + TABLOADMUHASEBAT + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.YIL], "=", kr.yil);
            //sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.HESAPKOD], "LIKE ", "830%");

            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.MUHASEBEKOD], "=", kr.muhasebe);

            if (kr.birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.BIRIMKOD], "=", kr.birim);

            if (kur != "")
            {
                string[] kurlar = kur.Split('/');
                if (kurlar.Length > 1)
                {
                    sql += " AND ((" + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.KURKOD], ">=", kurlar[0].Replace(".", ""));
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.KURKOD], "<", kurlar[1].Replace(".", "")) + ")";
                    sql += " OR " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.KURKOD], "=", "") + ")";
                }
                else
                {
                    if (kur2 != "")
                        sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.KURKOD], " >= ", kurlar[0].Replace(".", ""));
                    else
                    {
                        sql += " AND (" + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.KURKOD], " LIKE ", kurlar[0].Replace(".", "") + "%");
                        sql += " OR " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.KURKOD], "=", "") + ")";
                    }
                }

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.KURKOD], " < ", kur2);
            }
            if (fon != "")
            {
                string ekKosul = "";
                string[] kodlar = fon.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.FONKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }
            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.FINKOD], " LIKE ", fin + "%");
            if (eko != "")
            {
                string ekKosul = "";
                string[] kodlar = eko.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.EKOKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }

            if (kr.ay > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.AY], " <=", kr.ay);

            if (hesap830Var)
                sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.HESAPKOD], " LIKE ", "830%");
            else
                sql += " AND " + VeriErisim.KosulOl(TABLOADMUHASEBAT, ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.HESAPKOD], " NOT LIKE ", "830%");

            sql += " GROUP BY " + ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.YIL] + "," + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.HESAPKOD] + ",1,11)";

            if (hesap830Var)
                sql += "," + VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.FONKOD] + ",1,4)";

            sql += " ORDER BY 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KBSAktarimDegerListele(KBSAktarim kriter, int yil, int ay, int muhasebe, string birim, string hesapKod, string kur, string fon, string fin, string eko, TNSDateTime sorguTarih1, TNSDateTime sorguTarih2, int formDurum)
        {
            string sql = "";
            string alanlar = "";

            int yer = 3;
            if (kriter.hesapUzunluk == 1) yer = 5;
            if (kriter.hesapUzunluk == 2) yer = 7;
            if (kriter.hesapUzunluk == 3) yer = 9;
            if (kriter.hesapUzunluk == 4) yer = 11;
            alanlar += "SUBSTRING(HESAPKOD,1," + yer + ")";

            if (kriter.fonUzunluk > 0)
            {
                yer = 2;
                if (kriter.fonUzunluk == 1) yer = 2;
                if (kriter.fonUzunluk == 2) yer = 3;
                if (kriter.fonUzunluk == 3) yer = 4;
                alanlar += ", SUBSTRING(FONKOD,1," + yer + ")";
            }

            if (kriter.finUzunluk > 0)
            {
                alanlar += ", FINKOD";
            }

            sql = "SELECT " + alanlar + ", SUM(BORC), SUM(ALACAK)";
            sql += " FROM MUHMIFFORM INNER JOIN MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO";

            sql += " WHERE ";
            sql += " (MUHMIFFORM.FISTUR < 2)";

            if (formDurum == 1)
                sql = VeriErisim.KosulEkle(sql, "AND", "MUHMIFFORM.FORMDURUM=6");
            else if (formDurum == 2)
                sql = VeriErisim.KosulEkle(sql, "AND", "MUHMIFFORM.FORMDURUM=5");
            else
                sql = VeriErisim.KosulEkle(sql, "AND", "MUHMIFFORM.FORMDURUM<>9");

            sql += " AND " + VeriErisim.KosulOl("MUHMIFFORM", "YIL", "=", yil);

            if (ay > 0)
            {
                //Sadece yevmiyeli belgeler olacağı için yevmiye tarihine göre sorgulanmalı Melih 27.04.2016 UA da sorun oluyor
                //sql += " AND " + VeriErisim.KosulOl("", "MONTH(MUHMIFFORM.ISLEMTARIH)", " <=", ay);

                //Muhasebat 3 aylık istediği için ay bilgisi kaldırıldı. Tarih eklendi Melih 07.05.2019 EGO
                //sql += " AND " + VeriErisim.KosulOl("", "MONTH(MUHMIFFORM.YEVMIYETARIH)", " <=", ay);
                if (formDurum == 1)
                {
                    sql += " AND " + VeriErisim.KosulOl("", "MUHMIFFORM.YEVMIYETARIH", ">=", sorguTarih1);
                    sql += " AND " + VeriErisim.KosulOl("", "MUHMIFFORM.YEVMIYETARIH", "<=", sorguTarih2);
                }
                else
                {
                    sql += " AND " + VeriErisim.KosulOl("", "MUHMIFFORM.ISLEMTARIH", ">=", sorguTarih1);
                    sql += " AND " + VeriErisim.KosulOl("", "MUHMIFFORM.ISLEMTARIH", "<=", sorguTarih2);
                }
            }


            if (muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl("MUHMIFFORM", ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.MUHASEBEKOD], "=", muhasebe);
            if (birim != "")
                sql += " AND " + VeriErisim.KosulOl("MUHMIFFORM", ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.BIRIMKOD], "=", birim);
            if (hesapKod != "")
                sql += " AND " + VeriErisim.KosulOl("MUHMIF", ALANLARMUHASEBAT[(int)ENUMAlanlarMuhasebat.HESAPKOD], "LIKE", hesapKod + "%");
            if (kur != "")
            {
                string ekKosul = "";
                foreach (string f in kur.Split(';'))
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl("MUHMIF", "KURKOD", " LIKE ", f.Replace(".", "") + "%");
                }
                sql += " AND (" + ekKosul + ")";
            }
            if (fon != "")
            {
                string ekKosul = "";
                foreach (string f in fon.Split(';'))
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl("MUHMIF", "FONKOD", " LIKE ", f.Replace(".", "") + "%");
                }
                sql += " AND (" + ekKosul + ")";
            }
            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl("MUHMIF", "FINKOD", " LIKE ", fin + "%");
            if (eko != "")
            {
                string ekKosul = "";
                foreach (string f in eko.Split(';'))
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl("MUHMIF", "EKOKOD", " LIKE ", f.Replace(".", "") + "%");
                }
                sql += " AND (" + ekKosul + ")";
            }

            sql += " GROUP BY " + alanlar;
            sql += " ORDER BY 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Gider eklenen düşülen var mı sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public double ButceGiderEklenenDusulenVarmi(int yil)
        {
            string sql = "";

            sql = "SELECT SUM(EKLENEN)+SUM(DUSULEN) FROM ABSBUTCEGIDER WHERE YIL=" + yil;
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Gider hazırlık aşama var mı sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="asama">Aşama.</param>
        /// <returns></returns>
        public double ButceGiderHazirlikAsamaVarmi(int yil, string asama)
        {
            string sql = "";

            sql = "SELECT SUM(ODENEK) FROM ABSBUTCEHAZIRLIKGIDER WHERE YIL=" + yil + " AND SENARYOKOD='" + asama + "'";
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Gider bilgisi sil sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public Sonuc ButceGiderBilgisiSil(int yil)
        {
            string sql = "DELETE FROM ABSBUTCEGIDER WHERE YIL=" + yil;
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Gider bilgisi olustur sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public IDataReader ButceGiderBilgisiOlustur(int yil)
        {
            string sql = "SELECT YIL,KURKOD,FONKOD,FINKOD," + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3),SUM(ODENEK) ";
            sql += " FROM ABSBUTCEHAZIRLIKGIDER WHERE YIL=" + yil + " AND SENARYOKOD='0600'";
            sql += " GROUP BY YIL,KURKOD,FONKOD,FINKOD," + VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3)";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Gider bilgisi kaydet sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kur">Kurumsal kod.</param>
        /// <param name="fon">Fonksiyonel kod.</param>
        /// <param name="fin">Finans kod.</param>
        /// <param name="eko">Ekonomik kod.</param>
        /// <param name="kanun">Kanun ödeneği.</param>
        /// <param name="kbo">Kesintili başlangıç Ödeneği.</param>
        /// <param name="serbest">Serbest bırakılan.</param>
        /// <returns></returns>
        public Sonuc ButceGiderBilgisiKaydet(int yil, string kur, string fon, string fin, string eko, double kanun, double kbo, double serbest)
        {
            string sql = "INSERT INTO ABSBUTCEGIDER (YIL,KURKOD,FONKOD,FINKOD,EKOKOD,KANUN,KBO,EKLENEN,DUSULEN,SERBEST,EKLENEN_TASLAK,DUSULEN_TASLAK,SERBEST_TASLAK) VALUES (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(kur) + ", ";
            sql += VeriErisim.DegerYap(fon) + ", ";
            sql += VeriErisim.DegerYap(fin) + ", ";
            sql += VeriErisim.DegerYap(eko) + ", ";
            sql += VeriErisim.DegerYap(kanun) + ", ";
            sql += VeriErisim.DegerYap(kbo) + ", ";
            sql += VeriErisim.DegerYap(0) + ", ";
            sql += VeriErisim.DegerYap(0) + ", ";
            sql += VeriErisim.DegerYap(serbest) + ", ";
            sql += VeriErisim.DegerYap(0) + ", ";
            sql += VeriErisim.DegerYap(0) + ", ";
            sql += VeriErisim.DegerYap(0) + ")";

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Gider hazirlik odenek rakamı sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="asama">Aşama.</param>
        /// <param name="kur">Kurumsal kod.</param>
        /// <param name="fon">Fonksiyonel kod.</param>
        /// <param name="fin">Finans kod.</param>
        /// <param name="eko">Ekonomik kod.</param>
        /// <returns></returns>
        public double ButceGiderHazirlikOdenekRakami(int yil, int asama, string kur, string fon, string fin, string eko)
        {
            string sql = "";

            sql = "SELECT SUM(ODENEK) FROM ABSBUTCEHAZIRLIKGIDER ";
            sql += " WHERE YIL=" + yil + " AND SENARYOKOD='" + asama + "'";
            sql += " AND " + VeriErisim.KosulOl("", "KURKOD", "=", kur.Replace(".", "").Trim());
            sql += " AND " + VeriErisim.KosulOl("", "FONKOD", "=", fon.Replace(".", "").Trim());
            sql += " AND " + VeriErisim.KosulOl("", "FINKOD", "=", fin.Replace(".", "").Trim());
            sql += " AND " + VeriErisim.KosulOl("", VeriErisim.IslecBul("SUBSTRING") + "(EKOKOD, 1, 3)", "=", eko.Replace(".", "").Trim());

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Gelir eklenen dusulen var mı sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public double ButceGelirEklenenDusulenVarmi(int yil)
        {
            string sql = "";

            sql = "SELECT SUM(EKLENEN)+SUM(DUSULEN) FROM ABSBUTCEGELIR WHERE YIL=" + yil;
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Gelir hazırlık aşama var mı sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="asama">Aşama.</param>
        /// <returns></returns>
        public double ButceGelirHazirlikAsamaVarmi(int yil, int asama)
        {
            string sql = "";

            sql = "SELECT SUM(GELIR) FROM ABSBUTCEHAZIRLIKGELIR WHERE YIL=" + yil + " AND ASAMA=" + asama;
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Gelir bilgisi sil sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public Sonuc ButceGelirBilgisiSil(int yil)
        {
            string sql = "DELETE FROM ABSBUTCEGELIR WHERE YIL=" + yil;
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Gelir bilgisi oluştur sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public IDataReader ButceGelirBilgisiOlustur(int yil)
        {
            string sql = "SELECT YIL,KURKOD,EKOKOD,SUM(GELIR) ";
            sql += " FROM ABSBUTCEHAZIRLIKGELIR WHERE YIL=" + yil + " AND ASAMA=6";
            sql += " GROUP BY YIL,KURKOD,EKOKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Gelir bilgisi kaydet sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon..
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kur">Kurumsal kod.</param>
        /// <param name="eko">Ekonomik kod.</param>
        /// <param name="kanun">Kanun ödeneği.</param>
        /// <param name="kbo">Kesintili başlangıç Ödeneği.</param>
        /// <param name="serbest">Serbest bırakılan.</param>
        /// <returns></returns>
        public Sonuc ButceGelirBilgisiKaydet(int yil, string kur, string eko, double kanun, double kbo, double serbest)
        {
            string sql = "INSERT INTO ABSBUTCEGELIR (YIL,KURKOD,EKOKOD,KANUN,KBO,EKLENEN,DUSULEN,SERBEST,EKLENEN_TASLAK,DUSULEN_TASLAK,SERBEST_TASLAK) VALUES (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(kur) + ", ";
            sql += VeriErisim.DegerYap(eko) + ", ";
            sql += VeriErisim.DegerYap(kanun) + ", ";
            sql += VeriErisim.DegerYap(kbo) + ", ";
            sql += VeriErisim.DegerYap(0) + ", ";
            sql += VeriErisim.DegerYap(0) + ", ";
            sql += VeriErisim.DegerYap(serbest) + ", ";
            sql += VeriErisim.DegerYap(0) + ", ";
            sql += VeriErisim.DegerYap(0) + ", ";
            sql += VeriErisim.DegerYap(0) + ")";

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Aylık harcama dağılımını listeyen sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon .
        /// </summary>
        /// <param name="kr">Kriter bilgisi.</param>
        /// <returns></returns>
        public IDataReader AylikDagilimHarcama(AylikDagilim kr)
        {
            string sql = "";
            string kur = "";
            string kur2 = "";
            string fon = "";
            string fin = "";
            string eko = "";

            if (kr.tertip.kodlar.Length > 0)
            {
                kur = ((string)kr.tertip.kodlar[0]).Replace(".", "").Trim();
                if (kur.Length == 2)
                    kur2 = kur + OrtakFonksiyonlar.BagimsizKurumBaslangic();
            }
            if (kr.tertip.kodlar.Length > 1)
                fon = ((string)kr.tertip.kodlar[1]).Replace(".", "").Trim();
            if (kr.tertip.kodlar.Length > 2)
                fin = ((string)kr.tertip.kodlar[2]).Replace(".", "").Trim();
            if (kr.tertip.kodlar.Length > 3)
                eko = ((string)kr.tertip.kodlar[3]).Replace(".", "").Trim();

            if (kr.ay == 0)
            {
                if (kr.yil == DateTime.Now.Year)
                    kr.ay = DateTime.Now.Month;
                else
                    kr.ay = 12;
            }

            string alanlar = "";
            alanlar += ALANLARHARCAMA[(int)ENUMAlanlarHarcama.AY];
            alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD];
            alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.BIRIMKOD];
            alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD];
            alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FONKOD];
            alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FINKOD];
            alanlar += "," + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD];

            sql = "SELECT " + alanlar;
            sql += ",SUM(" + ALANLARHARCAMA[(int)ENUMAlanlarHarcama.HARCAMA] + ")";
            sql += " FROM " + TABLOADHARCAMA + " WHERE ";
            sql += VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.YIL], "=", kr.yil);

            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.MUHASEBEKOD], "=", kr.muhasebe);

            if (kr.birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.BIRIMKOD], "=", kr.birim);

            if (kur != "")
            {
                string[] kurlar = kur.Split('/');
                if (kurlar.Length > 1)
                {
                    sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], ">=", kurlar[0].Replace(".", ""));
                    sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], "<", kurlar[1].Replace(".", ""));
                }
                else
                {
                    if (kur2 != "")
                        sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], " >= ", kurlar[0].Replace(".", ""));
                    else
                        sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], " LIKE ", kurlar[0].Replace(".", "") + "%");
                }

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.KURKOD], " < ", kur2);
            }
            if (fon != "")
            {
                string ekKosul = "";
                string[] kodlar = fon.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FONKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }
            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.FINKOD], " LIKE ", fin + "%");
            if (eko != "")
            {
                string ekKosul = "";
                string[] kodlar = eko.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl(TABLOADHARCAMA, ALANLARHARCAMA[(int)ENUMAlanlarHarcama.EKOKOD], " LIKE ", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND (" + ekKosul + ")";
            }

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Aylık ödenek gönderme rakamı dağılımını listeyen sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon .
        /// </summary>
        /// <param name="kr">Kriter bilgisi</param>
        /// <returns></returns>
        public IDataReader AylikDagilimOdenekGonderme(AylikDagilim kr)
        {
            string sql = "";
            string kur = "";
            string kur2 = "";
            string fon = "";
            string fin = "";
            string eko = "";

            if (kr.tertip.kodlar.Length > 0)
            {
                kur = ((string)kr.tertip.kodlar[0]).Replace(".", "").Trim();
                if (kur.Length == 2)
                    kur2 = kur + OrtakFonksiyonlar.BagimsizKurumBaslangic();
            }
            if (kr.tertip.kodlar.Length > 1)
                fon = ((string)kr.tertip.kodlar[1]).Replace(".", "").Trim();
            if (kr.tertip.kodlar.Length > 2)
                fin = ((string)kr.tertip.kodlar[2]).Replace(".", "").Trim();
            if (kr.tertip.kodlar.Length > 3)
                eko = ((string)kr.tertip.kodlar[3]).Replace(".", "").Trim();

            if (kr.ay == 0)
            {
                if (kr.yil == DateTime.Now.Year)
                    kr.ay = DateTime.Now.Month;
                else
                    kr.ay = 12;
            }

            string alanlar = "";
            alanlar += " MONTH(DURUMTARIH)";
            alanlar += "," + "MUHGONDERMETENKISFORM.MUHASEBEKOD";
            alanlar += "," + "MUHGONDERMETENKISFORM.BIRIMKOD";
            alanlar += "," + "KURKOD";
            alanlar += "," + "FONKOD";
            alanlar += "," + "FINKOD";
            alanlar += "," + "EKOKOD";

            sql = "SELECT " + alanlar;
            sql += ", SUM(MIKTAR)";
            sql += " FROM MUHGONDERMETENKISFORM, MUHGONDERMETENKIS";
            sql += " WHERE " + "MUHGONDERMETENKISFORM.YIL = MUHGONDERMETENKIS.YIL";
            sql += " AND " + "MUHGONDERMETENKISFORM.BELGENO = MUHGONDERMETENKIS.BELGENO";
            sql += " AND " + "MUHGONDERMETENKISFORM.BELGETUR = MUHGONDERMETENKIS.BELGETUR";
            sql += " AND " + VeriErisim.KosulOl("MUHGONDERMETENKISFORM", "YIL", "=", kr.yil);

            sql += " AND " + VeriErisim.KosulOl("MUHGONDERMETENKISFORM", "FORMDURUM", "=", 5);
            if (kr.muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl("MUHGONDERMETENKISFORM", "MUHASEBEKOD", "=", kr.muhasebe);

            if (kr.birim != "")
                sql += " AND " + VeriErisim.KosulOl("MUHGONDERMETENKISFORM", "BIRIMKOD", "=", kr.birim);

            if (kur != "")
            {
                string[] kurlar = kur.Split('/');
                if (kurlar.Length > 1)
                {
                    sql += " AND " + VeriErisim.KosulOl("", "KURKOD", ">=", kurlar[0].Replace(".", ""));
                    sql += " AND " + VeriErisim.KosulOl("", "KURKOD", "<", kurlar[1].Replace(".", ""));
                }
                else
                {
                    if (kur2 != "")
                        sql += " AND " + VeriErisim.KosulOl("", "KURKOD", ">=", kurlar[0].Replace(".", ""));
                    else
                        sql += " AND " + VeriErisim.KosulOl("", "KURKOD", "LIKE", kurlar[0].Replace(".", "") + "%");
                }

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl("", "KURKOD", "<", kur2);
            }
            if (fon != "")
            {
                string ekKosul = "";
                string[] kodlar = fon.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl("", "FONKOD", "LIKE", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND " + "(" + ekKosul + ")";
            }
            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl("", "FINKOD", "=", fin);
            if (eko != "")
            {
                string ekKosul = "";
                string[] kodlar = eko.Split(';');
                for (int i = 0; i < kodlar.Length; i++)
                {
                    if (ekKosul != "") ekKosul += " OR ";
                    ekKosul += VeriErisim.KosulOl("", "EKOKOD", "LIKE", kodlar[i].Replace(".", "") + "%");
                }

                sql += " AND " + "(" + ekKosul + ")";
            }

            if (alanlar != "")
            {
                sql += " GROUP BY " + alanlar;
                sql += " ORDER BY " + alanlar;
            }

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

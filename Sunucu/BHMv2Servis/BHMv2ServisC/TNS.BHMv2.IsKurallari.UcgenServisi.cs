using TNS.Veri;
using TNS.KYM;
using System.Data;
using System;
using TNS.UZY;
using TNS.BKD;
using System.Collections;
using System.Threading;

namespace TNS.BHMv2.IsKurallari
{
    /// <summary>
    /// Tertip listesi tabloları kullanılarak, tertip listesi veya üçgen listeyi verilen ek parametrelerle döndüren yordamları tutan sınıf.
    /// </summary>
    public class UcgenServisi
    {
        public static string UcgenKodBilgileriSorguAlanlarOl(int yil, UcgenKodBilgileri kodBilgileri, ref string[] degerMaske, string tabloAd, ENUMTabloKod tabloNo)
        {
            if (!string.IsNullOrEmpty(tabloAd)) tabloAd += ".";
            else tabloAd = "";
            string alanlar = string.Empty;
            IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();
            ButceKodYapi bky;
            for (int i = 0; i < kodBilgileri.BoyutSayisi; i++)
            {
                UcgenKodBilgi ukb = kodBilgileri[i];
                bky = bkdServis.ButceKodBilgiBul(yil, ukb.kodNo, string.Empty);
                degerMaske[i] = bky.kodYapi;
                if (alanlar != string.Empty) alanlar += ", ";
                string alanAd = bky.vtAlanKod;
                if (tabloNo == ENUMTabloKod.TABPRGMasrafPlaniForm)
                {
                    //if (bky.kodNo == 2) alanAd = "I.FONKOD";
                    //else if (alanAd == "AFAALKOD") alanAd = "ALTFAALKOD";
                }
                if (bky.kodNo == 17)
                {
                    if (tabloNo == ENUMTabloKod.TABMPTavan || tabloNo == ENUMTabloKod.TABMIFKY) alanAd = "KYKOD";
                    else alanAd = "KURUMSALYAPIKOD";
                }
                if (alanAd.Equals("PEBFONKOD")) alanAd = "PEBKOD";
                if (bky.kodYapi == "N")
                    alanlar += tabloAd + alanAd;
                else
                {
                    if (bky.kodNo == 4 && ukb.eko23)
                    {
                        alanlar += TNS.BKD.Arac.Eko23AlanOl(yil, bky, tabloAd, alanAd);
                    }
                    else
                        alanlar += VeriErisim.IslecBul("SUBSTRING") + "(" + tabloAd + alanAd + ", 1, " + bky.KirilimToplamUzunluk(ukb.kirilimSay).ToString() + ")";
                }
            }
            return alanlar;
        }

        /// <summary>
        /// Tertip listesi sonucunda, tertibin döndürülecek boyutları ve boyutların kırılımlarını oluşturacak şekilde 
        /// ve verilen kıstas tertiple uyumlu tertipleri seçen select cümlesinin oluşturan yordam.
        /// Bu yordam ayrıca üçgen tertiplerin oluşturulmasında kullanılacak bilgileri de ilgili parametrelerde döndürür.
        /// Hata olursa, döndürülen dizgi boşluk karakteri ile başlar ve hata mesajı ile devam eder.
        /// </summary>
        /// <param name="kullanan">Kullanıcının seçebileceği tertipleri belirlemek için, sql where kısmına eklemek için kullanılabilecek kullanıcı bilgisi.</param>
        /// <param name="kistas">Seçilecek tertiplerin kıstas bilgilerini tertip dizileri şeklinde tutan nesne</param>
        /// <param name="kodBilgileri">Seçilecek tertip boyutlarının hangileri olduğunu, sırasını ve kırılım sayısını tutan nesne.</param>
        /// <param name="ekDegerParam">Oluşturulacak sql'in hangi veri türü ve yılı için olacağını tutan değişken</param>
        /// <param name="degerMaske">Seçilen her bir boyut için, seçilen boyutun kırılım yapısını döndürecek dizi.</param>
        /// <returns>Liste yada üçgen liste oluşturacak sql döndürülür. Boşluk karekteri ile başlıyorsa hata döndürülmüş olur.</returns>
        public static string UcgenSQLOlYeni(Kullanici kullanan, KistasYapi kistas, UcgenKodBilgileri kodBilgileri, EkDegerParam ekDegerParam, ref string[] degerMaske, bool aciklama)
        {
            int yil = kistas.ekleTertipler[0].yil;
            ENUMTabloKod tabloNo = ekDegerParam.TabloKod();
            string tabloAdEkle = EkDegerParam.TabloAdiEkleBul(tabloNo);
            string alanlar = UcgenKodBilgileriSorguAlanlarOl(yil, kodBilgileri, ref degerMaske, tabloAdEkle, tabloNo);
            string sql = string.Empty;
            IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

            string erisimAd = ekDegerParam.VeriErisimAdBul() + yil.ToString();
            string toplamAlanAd;
            string ekleKosul = KosulOl(kistas.ekleTertipler, uzyServis, kullanan, erisimAd, yil, ekDegerParam, out toplamAlanAd, aciklama);
            if (string.IsNullOrEmpty(ekleKosul) || string.IsNullOrEmpty(toplamAlanAd))
                return " " + ButceHazirlikResI.BHMUS004 + " (" + tabloNo.ToString() + ", " + "Tertip Say:" + (kistas.ekleTertipler == null ? "null" : kistas.ekleTertipler.Length.ToString()) + ", " + erisimAd + ", " + ekDegerParam.senaryoKodVeyaAy + ", " + ekleKosul + ", " + toplamAlanAd + ")";
            sql = "SELECT " + alanlar + "," + toplamAlanAd;
            string cikartKosul = KosulOl(kistas.dusTertipler, uzyServis, kullanan, erisimAd, yil, ekDegerParam, out toplamAlanAd, aciklama);
            if (!string.IsNullOrEmpty(cikartKosul))
            {
                ekleKosul = "(" + ekleKosul + ") AND NOT (" + cikartKosul + ")";
            }
            sql += " WHERE " + ekleKosul;
            if (!aciklama)
                sql = sql + " GROUP BY " + alanlar;
            sql = sql + " ORDER BY ";
            for (int a = 1; a <= kodBilgileri.BoyutSayisi; a++)
            {
                sql += a.ToString();
                if (a != kodBilgileri.BoyutSayisi)
                    sql += ", ";
            }
            return sql;
        }

        private static string KosulOl(Tertip[] kistasTertipler, IUZYServis uzyServis, Kullanici kullanan, string erisimAd, int yil, EkDegerParam ekDegerParam, out string sumAlanAd, bool aciklama)
        {
            sumAlanAd = "";
            if (kistasTertipler == null) return "";
            string kosul = "";
            for (int i = 0; i < kistasTertipler.Length; i++)
            {
                string sorgu = ekDegerParam.EkDegerSqlDondur(uzyServis, kullanan, erisimAd, yil, kistasTertipler[i].TertipStrOl(false));
                int whereYer = sorgu.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase);
                //int fromYer = sorgu.IndexOf("FROM", StringComparison.CurrentCultureIgnoreCase);
                if (whereYer > 1)
                {
                    if (sumAlanAd == "")
                    {
                        sumAlanAd = sorgu.Substring(6, whereYer - 7);
                        if (aciklama)
                        {
                            string acAlan;
                            string acTabloEk;
                            if (kistasTertipler[i].tabloKod == ENUMTabloKod.TABPEBTavanUstu)
                            {
                                acAlan = "GEREKCE";
                                acTabloEk = "";
                            }
                            else
                            {
                                acAlan = "ACIKLAMA";
                                acTabloEk = "_ACIK";
                            }
                            int fromYer = sumAlanAd.IndexOf(" FROM ");
                            if (fromYer > 0)
                            {
                                sumAlanAd = acAlan + " " + sumAlanAd.Substring(fromYer).Trim() + acTabloEk;
                            }
                        }
                    }
                    sorgu = "(" + sorgu.Substring(whereYer + 5) + ")";
                    if (kosul != "") kosul += " OR ";
                    kosul += sorgu;
                }
            }
            return kosul;
        }

        ///// <summary>
        ///// Tertip listesi sonucunda, tertibin döndürülecek boyutları ve boyutların kırılımlarını oluşturacak şekilde 
        ///// ve verilen kıstas tertiple uyumlu tertipleri seçen select cümlesinin oluşturan yordam.
        ///// Bu yordam ayrıca üçgen tertiplerin oluşturulmasında kullanılacak bilgileri de ilgili parametrelerde döndürür.
        ///// Hata olursa, döndürülen dizgi boşluk karakteri ile başlar ve hata mesajı ile devam eder.
        ///// </summary>
        ///// <param name="kullanan">Kullanıcının seçebileceği tertipleri belirlemek için, sql where kısmına eklemek için kullanılabilecek kullanıcı bilgisi.</param>
        ///// <param name="yil">Seçilecek tertiplerin yılı</param>
        ///// <param name="tabloNo">Seçilecek tertiplerin bulunduğu tablo</param>
        ///// <param name="kistas">Seçilecek tertiplerin kıstas bilgisini tutan tertip yapısında nesne</param>
        ///// <param name="kodBilgileri">Seçilecek tertip boyutlarının hangileri olduğunu, sırasını ve kırılım sayısını tutan nesne.</param>
        ///// <param name="degerTablo">Seçilen her bir boyut için, boyutun kod açıklamasının bulunacağı tabloların isimlerini döndürecek dizi.</param>
        ///// <param name="degerTabloAlanAd">Seçilen her bir boyut için, boyutun kod açıklamasının bulunacağı tablodaki alanın isimlerini döndürecek dizi.</param>
        ///// <param name="degerSay">Seçilen her bir boyut için, seçilen boyutun kırılım sayısını döndürecek dizi.</param>
        ///// <param name="kirilimSay">Seçilen tüm alanların toplam kırılım sayısını döndüren değişken</param>
        ///// <param name="degerMaske">Seçilen her bir boyut için, seçilen boyutun kırılım yapısını döndürecek dizi.</param>
        ///// <param name="ekKosul">Yıl hariç, verilen kıstas tertip için oluşturulan koşulu döndüren değişken.</param>
        ///// <returns>Liste yada üçgen liste oluşturacak sql döndürülür. Boşluk karekteri ile başlıyorsa hata döndürülmüş olur.</returns>
        //public static string UcgenSQLOl(Kullanici kullanan, int yil, ENUMTabloKod tabloNo, Tertip kistas, UcgenKodBilgileri kodBilgileri, ref string[] degerTablo, ref string[] degerTabloAlanAd, ref int[] degerSay, ref int kirilimSay, ref string[] degerMaske, ref string ekKosul)
        //{
        //    string sql = string.Empty;
        //    string alanlar = string.Empty;
        //    string tabloAd = string.Empty;
        //    ButceKodYapi bky;
        //    IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();
        //    IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();

        //    if (tabloNo == ENUMTabloKod.TABPEBOdenek || tabloNo == ENUMTabloKod.TABPEBHAZHarcama || tabloNo == ENUMTabloKod.TABHAZHarcama || tabloNo == ENUMTabloKod.TABHarcamaTahmin || tabloNo == ENUMTabloKod.TABOdemeEmri || tabloNo == ENUMTabloKod.TABOdenek || tabloNo == ENUMTabloKod.TABPEBHAZHarcama || tabloNo == ENUMTabloKod.TABPEBOdenek || tabloNo == ENUMTabloKod.TABTasraOdenek)
        //        tabloAd = "BHMTERTIPLISTESIGIDER";
        //    else if (tabloNo == ENUMTabloKod.TABPEBGelir || tabloNo == ENUMTabloKod.TABPEBGelirGerceklesmesi || tabloNo == ENUMTabloKod.TABGelir || tabloNo == ENUMTabloKod.TABGelirGerceklesmesi || tabloNo == ENUMTabloKod.TABGelirTahmini)
        //        tabloAd = "BHMTERTIPLISTESIGELIR";
        //    else if (tabloNo == ENUMTabloKod.TABFinansman)
        //        tabloAd = "";
        //    else if (tabloNo == ENUMTabloKod.TABMasrafPlani)
        //        tabloAd = "MPGMASRAFPLANITERTIP";
        //    else if (tabloNo == ENUMTabloKod.TABOdenekGonderme)
        //        tabloAd = "MUHODENEKGONDERMEKY";
        //    else if (tabloNo == ENUMTabloKod.TABTJKBudget)
        //        tabloAd = "ButceHazirlik.dbo.TJKAPPROPRIATION";
        //    else
        //        return " " + ButceHazirlikResI.BHMUS001;

        //    for (int i = 0; i < kodBilgileri.BoyutSayisi; i++)
        //    {
        //        UcgenKodBilgi ukb = kodBilgileri[i];
        //        bky = bkdServis.ButceKodBilgiBul(yil, ukb.kodNo, string.Empty);
        //        degerTablo[i] = bky.tabloAd;
        //        degerTabloAlanAd[i] = bky.vtAlanAd + TNS.BHMv2.Arac.AlanEkBul();
        //        degerSay[i] = ukb.kirilimSay;
        //        kirilimSay += ukb.kirilimSay;
        //        degerMaske[i] = bky.kodYapi;
        //        if (alanlar != string.Empty) alanlar += ", ";
        //        string alanAd = bky.vtAlanKod;
        //        if (alanAd.Equals("PEBFONKOD")) alanAd = "PEBKOD";
        //        alanlar += VeriErisim.IslecBul("SUBSTRING") + "(" + alanAd + ", 1, " + bky.KirilimToplamUzunluk(ukb.kirilimSay).ToString() + ")";
        //    }

        //    string onKosul = uzyServis.KosulOlustur(string.Empty, tabloNo == ENUMTabloKod.TABTJKBudget ? "YEAR" : "YIL", "N", yil.ToString(), true, false);
        //    ekKosul = string.Empty;
        //    if (kistas != null)
        //    {
        //        for (int i = 0; i < kistas.kodlar.Length; i++)
        //        {
        //            if (!String.IsNullOrEmpty((string)kistas.kodlar[i]))
        //            {
        //                int kodNo = (int)kistas.KodNolar(i);
        //                bky = bkdServis.ButceKodBilgiBul(yil, kodNo, string.Empty);

        //                string tut = uzyServis.KosulOlustur(string.Empty, bky.vtAlanKod, bky.kodYapi, (string)kistas.kodlar[i], false, (int)kistas.KodNolar(i) == 1);

        //                ekKosul += " AND " + tut;
        //            }
        //        }
        //    }
        //    sql = sql + "SELECT DISTINCT " + alanlar + " FROM " + tabloAd + " WHERE " + onKosul + ekKosul;
        //    sql = sql + " ORDER BY ";
        //    for (int a = 1; a <= kodBilgileri.BoyutSayisi; a++)
        //    {
        //        sql += a.ToString();
        //        if (a != kodBilgileri.BoyutSayisi)
        //            sql += ", ";
        //    }
        //    return sql;
        //}

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public static OdenekBilgi[] ListeTertipOl(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul, int aciklamaKolon)
        {
            KistasYapi kY = new KistasYapi();
            kY.ekleTertipler = new Tertip[1];
            kY.ekleTertipler[0] = kistasTertip;
            return ListeTertipOlYeni(veriErisim, kullanan, kY, kodBilgileri, ekBilgiler, sifirlarHaric, tanimliTertipler, adlariBul, aciklamaKolon);
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasYapi">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public static OdenekBilgi[] ListeTertipOl(VeriErisim veriErisim, Kullanici kullanan, KistasYapi kistasYapi, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul, int aciklamaKolon)
        {
            return ListeTertipOlYeni(veriErisim, kullanan, kistasYapi, kodBilgileri, ekBilgiler, sifirlarHaric, tanimliTertipler, adlariBul, aciklamaKolon);
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        //private static OdenekBilgi[] ListeTertipOlEski(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric)
        //{
        //    int yil = kistasTertip.yil;
        //    ENUMTabloKod tabloNo = kistasTertip.tabloKod;

        //    int alanSay = kodBilgileri.BoyutSayisi;
        //    if (alanSay == 0) return null;
        //    string[] degerTablo = new string[alanSay];
        //    string[] degerTabloAlanAd = new string[alanSay];
        //    int[] degerSay = new int[alanSay];
        //    int[] capraz = new int[alanSay];
        //    int kirilimSay = 0;
        //    string[] degerMaske = new string[alanSay];
        //    string ekKosul = string.Empty;

        //    System.Collections.ArrayList donusDeger = new System.Collections.ArrayList();
        //    string sql = UcgenSQLOl(kullanan, yil, tabloNo, kistasTertip, kodBilgileri, ref degerTablo, ref degerTabloAlanAd, ref degerSay, ref kirilimSay, ref degerMaske, ref ekKosul);
        //    //ekKosul = string.Empty;
        //    if (sql == string.Empty)
        //    {
        //        throw new Exception(ButceHazirlikResI.BHMUS002);
        //    }
        //    else if (sql.StartsWith(" "))
        //    {
        //        throw new Exception(sql);
        //    }


        //    string[] kistaslar = new string[kistasTertip.KodNoSay()];
        //    for (int i = 0; i < kistasTertip.KodNoSay(); i++)
        //    {
        //        kistaslar[i] = string.Empty;
        //    }


        //    for (int i = 0; i < alanSay; i++)
        //    {
        //        int kistasYer = -1;
        //        for (int kod = 0; kod < kistasTertip.KodNoSay(); kod++)
        //            if (kodBilgileri[i].kodNo == (int)kistasTertip.KodNolar(kod))
        //            {
        //                kistasYer = kod;
        //                break;
        //            }
        //        if (kistasYer == -1) return null;
        //        capraz[i] = kistasYer;
        //    }

        //    UZY.IUZYServis uzS = TNS.UZY.Arac.Tanimla();

        //    VeriErisim ve = UcgenVeriErisimBul(veriErisim, yil, tabloNo);
        //    VeriErisim ve2 = UcgenVeriErisimBul(null, yil, ENUMTabloKod.TABPEBOdenek);

        //    System.Collections.Hashtable hashTable = new System.Collections.Hashtable();

        //    IDataReader r = ve.DataReaderDondur(sql);
        //    if (ve.hata == string.Empty)
        //    {
        //        while (r.Read())
        //        {
        //            OdenekBilgi o = new OdenekBilgi(yil, tabloNo);
        //            for (int i = 0; i < alanSay; i++)
        //            {
        //                string tut = VeriErisim.DegerAl(r, i, string.Empty);
        //                tut = OrtakFonksiyonlar.KodNoktaKoy(degerMaske[i], tut);
        //                kistaslar[capraz[i]] = tut.Replace(".", string.Empty);
        //                o.tertip.TertibeKodEkle(capraz[i], tut, TNS.BHMv2.Arac.UcgenAciklamaBul(uzS, hashTable, kullanan, yil, i, degerTablo[i], degerTabloAlanAd[i], tut));
        //            }
        //            string tertipStr = string.Empty;
        //            for (int i = 0; i < kistaslar.Length; i++)
        //            {
        //                tertipStr += kistaslar[i];
        //                if (i < kistaslar.Length - 1)
        //                    tertipStr += "-";
        //            }

        //            for (int e = 0; ekBilgiler != null && e < ekBilgiler.Length; e++)
        //            {
        //                if (ekBilgiler[e].HazirlikMi())
        //                {
        //                    string sqlTek = ekBilgiler[e].EkDegerSqlDondur(uzS, kullanan, ve2.baglantiAd, yil, tertipStr);
        //                    if (sqlTek != string.Empty)
        //                    {
        //                        o.ekRakamlar.Add(ve2.TekKolonSQLDbl(sqlTek + ekKosul));
        //                        ve2.hata = string.Empty;
        //                    }
        //                    else
        //                        o.ekRakamlar.Add((double)-1);
        //                }
        //                else
        //                    o.ekRakamlar.Add(ekBilgiler[e].EkDegerHesaplaDbl(uzS, kullanan, yil, tertipStr));
        //            }
        //            donusDeger.Add(o);
        //        }
        //        r.Close();
        //    }
        //    else
        //    {
        //        ve.VeriErisimKapat();
        //        ve2.VeriErisimKapat();
        //        throw new Exception(ve.hata);
        //    }
        //    ve.VeriErisimKapat();
        //    ve2.VeriErisimKapat();
        //    return (OdenekBilgi[])donusDeger.ToArray(typeof(OdenekBilgi));
        //}

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasYapi">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        private static OdenekBilgi[] ListeTertipOlYeni(VeriErisim veriErisim, Kullanici kullanan, KistasYapi kistasYapi, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul, int aciklamaKolon)
        {
            SortedList ht = ListeTertipOlHash(veriErisim, kullanan, kistasYapi, kodBilgileri, ekBilgiler, sifirlarHaric, tanimliTertipler, aciklamaKolon);
            OdenekBilgi[] donusDeger = new OdenekBilgi[ht.Count];
            int htSay = 0;
            UZY.IUZYServis uzS = TNS.UZY.Arac.Tanimla();
            System.Collections.Hashtable hashTable = new System.Collections.Hashtable();
            int alanSay = kodBilgileri.BoyutSayisi;
            if (alanSay == 0) return null;

            int[] capraz = new int[alanSay];
            string[] degerTablo = new string[alanSay];
            string[] degerTabloAlanAd = new string[alanSay];

            UcgenKodBilgileriCevrimBul(kodBilgileri, kistasYapi.ekleTertipler[0], capraz, degerTablo, degerTabloAlanAd);

            foreach (DictionaryEntry de in ht)
            {
                string tertipStr = (string)de.Key;
                HashOdenek degerler = (HashOdenek)de.Value;
                OdenekBilgi ob = new OdenekBilgi(kistasYapi.ekleTertipler[0].yil, kistasYapi.ekleTertipler[0].tabloKod);
                ob.aciklama = degerler.aciklama;
                if (degerler.tertip != null)
                {
                    ob.tertip = degerler.tertip;
                    if (adlariBul)
                    {
                        for (int i = 0; i < ob.tertip.kodlar.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(ob.tertip.adlar[i]) && !string.IsNullOrWhiteSpace(ob.tertip.kodlar[i]))
                            {
                                ob.tertip.adlar[i] = TNS.BHMv2.Arac.UcgenAciklamaBul(uzS, hashTable, kullanan, kistasYapi.ekleTertipler[0].yil, i, degerTablo[i], degerTabloAlanAd[i], ob.tertip.kodlar[i]);
                            }
                        }
                    }
                }
                else
                {
                    string[] kodlar = tertipStr.Split('-');
                    for (int i = 0; i < kodlar.Length; i++)
                    {
                        string tut = kodlar[i];
                        string ad = "";
                        if (adlariBul)
                            ad = TNS.BHMv2.Arac.UcgenAciklamaBul(uzS, hashTable, kullanan, kistasYapi.ekleTertipler[0].yil, i, degerTablo[i], degerTabloAlanAd[i], tut);
                        ob.tertip.TertibeKodEkle(capraz[i], tut, ad);
                    }
                }
                for (int i = 0; i < degerler.degerler.Length; i++)
                {
                    ob.ekRakamlar.Add(degerler.degerler[i]);
                }

                donusDeger[htSay++] = ob;
            }
            return donusDeger;
        }

        private static bool UcgenKodBilgileriCevrimBul(UcgenKodBilgileri kodBilgileri, Tertip kistasTertip, int[] capraz, string[] degerTablo, string[] degerTabloAlanAd)
        {
            int alanSay = kodBilgileri.BoyutSayisi;
            ButceKodYapi bky;
            IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();
            for (int i = 0; i < alanSay; i++)
            {
                int kistasYer = -1;
                for (int kod = 0; kod < kistasTertip.KodNoSay(); kod++)
                    if (kodBilgileri[i].kodNo == (int)kistasTertip.KodNolar(kod))
                    {
                        kistasYer = kod;
                        break;
                    }
                if (kistasYer == -1) return false;
                capraz[i] = kistasYer;
                UcgenKodBilgi ukb = kodBilgileri[i];
                bky = bkdServis.ButceKodBilgiBul(kistasTertip.yil, ukb.kodNo, string.Empty);
                degerTablo[i] = bky.tabloAd;
                degerTabloAlanAd[i] = bky.vtAlanAd + TNS.BHMv2.Arac.AlanEkBul();
            }
            return true;
        }

        private class HashOdenek
        {
            public double[] degerler = null;
            public Tertip tertip = null;
            public string aciklama = null;
            public HashOdenek(int degerSay, Tertip pTertip)
            {
                degerler = new double[degerSay];
                this.tertip = pTertip;
            }
        }

        private static SortedList ListeTertipOlHash(VeriErisim veriErisim, Kullanici kullanan, KistasYapi kistasYapi, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, int aciklamaKolon)
        {
            SortedList donusDeger = new SortedList();
            if (tanimliTertipler || aciklamaKolon > -1)
            {
                int icYil = kistasYapi.ekleTertipler[0].yil;
                VeriErisim veIc = VeriErisim.VeriErisimBul(null, BHMv2Genel.VeriErisimAdBul(icYil), kullanan == null ? null : kullanan.KimlikBul());
                if (tanimliTertipler)
                {
                    Tertip tTABPEBOdenek = new Tertip(icYil, ENUMTabloKod.TABPEBOdenek);
                    Tertip tTABPEBGelir = new Tertip(icYil, ENUMTabloKod.TABPEBGelir);

                    if (UcgenTamMi(tTABPEBOdenek, kodBilgileri) || UcgenTamMi(tTABPEBGelir, kodBilgileri))
                    {
                        Tertip[] tertipler = TertipTanim.Listele(veIc, kullanan, kistasYapi.ekleTertipler[0], 2, null);
                        if (tertipler != null)
                        {
                            for (int i = 0; i < tertipler.Length; i++)
                            {
                                donusDeger[tertipler[i].TertipStrOl()] = new HashOdenek(ekBilgiler == null ? 0 : ekBilgiler.Length, tertipler[i]);
                            }
                        }
                    }
                    else
                    {
                        string[] tertipler = TertipTanim.Listele(veIc, kullanan, kistasYapi.ekleTertipler[0], kodBilgileri);
                        if (tertipler != null)
                        {
                            for (int i = 0; i < tertipler.Length; i++)
                            {
                                donusDeger[tertipler[i]] = new HashOdenek(ekBilgiler == null ? 0 : ekBilgiler.Length, null);
                            }
                        }
                    }
                }
                if (aciklamaKolon > -1)
                {
                    ObjectArray oa = TertipTanim.ListeleAciklama(veIc, kullanan, kistasYapi, kodBilgileri, ekBilgiler[aciklamaKolon]);
                    if (oa != null && oa.objeler != null)
                    {
                        foreach (StrKodStrAd to in oa.objeler)
                        {
                            HashOdenek degerler = null;
                            try
                            {
                                degerler = (HashOdenek)donusDeger[to.kod];
                            }
                            catch { }
                            if (degerler == null)
                            {
                                degerler = new HashOdenek(ekBilgiler.Length, null);
                                donusDeger[to.kod] = degerler;
                            }
                            degerler.aciklama = to.ad;
                        }
                    }
                }
                veIc.VeriErisimKapat();
            }

            for (int ekSay = 0; ekSay < (ekBilgiler == null ? 0 : ekBilgiler.Length); ekSay++)
            {
                if (ekBilgiler[ekSay] == null) continue;
                int alanSay = kodBilgileri.BoyutSayisi;
                string[] degerMaske = new string[alanSay];
                string sql = UcgenSQLOlYeni(kullanan, kistasYapi, kodBilgileri, ekBilgiler[ekSay], ref degerMaske, false);
                //ekKosul = string.Empty;
                if (sql == string.Empty)
                {
                    throw new Exception(ButceHazirlikResI.BHMUS002);
                }
                else if (sql.StartsWith(" "))
                {
                    throw new Exception(sql);
                }

                VeriErisim ve = UcgenVeriErisimBul(veriErisim, kistasYapi.ekleTertipler[0].yil, ekBilgiler[ekSay]);
                ve.sureAsimi = 9000;
                IDataReader r = ve.DataReaderDondur(sql);
                if (r != null)
                {
                    while (r.Read())
                    {
                        string trtpStr = "";
                        for (int i = 0; i < alanSay; i++)
                        {
                            string tut;

                            if (degerMaske[i] == "N")
                            {
                                tut = VeriErisim.DegerAl(r, i, 0).ToString();
                            }
                            else
                            {
                                tut = VeriErisim.DegerAl(r, i, string.Empty);
                                tut = OrtakFonksiyonlar.KodNoktaKoy(degerMaske[i], tut);
                            }
                            if (i != 0) trtpStr += "-";
                            trtpStr += tut;
                        }
                        HashOdenek degerler = null;
                        try
                        {
                            degerler = (HashOdenek)donusDeger[trtpStr];
                        }
                        catch { }
                        if (degerler == null)
                        {
                            degerler = new HashOdenek(ekBilgiler.Length, null);
                            donusDeger[trtpStr] = degerler;
                        }
                        degerler.degerler[ekSay] = (double)VeriErisim.DegerAl(r, alanSay, (double)0);
                    }
                    r.Close();
                    ve.VeriErisimKapat();
                }
                else
                {
                    ve.VeriErisimKapat();
                    ve.hata = "";
                }
            }
            return donusDeger;
        }

        private static void DebugYaz(string metin, bool yeni = false)
        {
            OrtakFonksiyonlar.HataDosyaYaz("AAListeTertip.txt", metin, yeni);
        }

        private static bool UcgenTamMi(Tertip tertip, UcgenKodBilgileri kodBilgileri)
        {
            if (tertip.kodlar.Length != kodBilgileri.BoyutSayisi) return false;
            for (int i = 0; i < tertip.kodlar.Length; i++)
            {
                if (kodBilgileri[i].kodNo != tertip.KodNolar(i)) return false;
                if (kodBilgileri[i].kirilimSay != tertip.KodNoKirilimSay(i)) return false;
            }
            return true;
        }

        private static OdenekBilgi DonusDegerdeBul(ArrayList donusDeger, OdenekBilgi o)
        {
            foreach (OdenekBilgi item in donusDeger)
            {
                if (item.tertip.Kiyasla(o.tertip)) return item;
            }
            return null;
        }

        private static VeriErisim UcgenVeriErisimBul(VeriErisim veriErisim, int yil, EkDegerParam ekDegerParam)
        {
            string tut = ekDegerParam.VeriErisimAdBul();
            if (tut == "BaglantiSatiriBHMv2")
            {
                tut = BHMv2Genel.VeriErisimAdBul(yil);
            }
            return VeriErisim.VeriErisimBul(veriErisim, tut, null);
        }

        private static VeriErisim UcgenVeriErisimBul(VeriErisim veriErisim, int yil, ENUMTabloKod tabloNo)
        {
            if (tabloNo == ENUMTabloKod.TABMasrafPlani || tabloNo == ENUMTabloKod.TABOdenekGonderme || tabloNo == ENUMTabloKod.TABHarcama || tabloNo == ENUMTabloKod.TABPRGHarcama)
            {
                return VeriErisim.VeriErisimBul(veriErisim, "BaglantiSatiriBUM", null);
            }
            else
            {
                return VeriErisim.VeriErisimBul(veriErisim, BHMv2Genel.VeriErisimAdBul(yil), null);
            }
        }

        /// <summary>
        /// Veri tabanından yeni bir satır geldiğinde, satır kırılımlara ayrılır ve yeniDeğerler dizisine eklenir.
        /// Bu dizi yeniDegerler parametresi ile yordama geçirilir.
        /// Eğer kırılım yeni bir kodun başında ise, bu kodun tertip içindeki yeri bulunur ve uzay sorgusu boşaltılır ve bu kırılım ise yeni bir sorgu başlamış olur.
        /// Eğer kırılım önceki kodla aynı alanda ise, kırılım önceki uzay sorgusuna eklenir.
        /// </summary>
        /// <param name="kirilimYer">kırılımlar dizisindeki, hangi sıradaki kıırlımın işleme alınacaği</param>
        /// <param name="kirilimlarDizisi">Okunan satırın, sonuç kümesinde yer alan, her bir kırılımını tutan kırılımlar dizi.</param>
        /// <param name="kolonNo">Parametre olarak girişte işleme alınan bir önceki kırılımın, bulunduğu kolonda hangi sıradaki kırılım olduğunu gösteren
        /// çıkışta ise, kullanılan kırılımın bulunduğu kolonda hangi sırada olduğunu döndüren değişken</param>
        /// <param name="kirilimSay">Sonuç kümesinde bulunan her bir kolonun, sahip olduğu kırılım sayısı </param>
        /// <param name="aciklamaKod">Aktif kırılımın adını bulmak için kullanılacak uzay sorgusu döndürülür.</param>
        /// <param name="kodNo">Parametre olarak girişte işleme alınan bir önceki kırılımın seçilen hangi kolonda tutulduğunu gösteren
        /// çıkışta ise, kullanılan kırılımın seçilen hangi kolonda tutulduğu bilgisini döndüren değişken
        /// </param>
        /// <param name="kistas">Seçilen kolonların tertip içi sırasının bulunacağı tertip nesnesi. Tertip nesnesi yordama kıstas olarak geçirilen nesnedir.</param>
        /// <param name="kistasYer">İşleme alınan kırılımın bulunduğu kolonun, tertip içindeki sıra numarası.</param>
        /// <param name="kistaslar">Aktif kırılımın açıklamasını bulmak için kullanılacak uzay sorgusu dizisi. Bulunan uzay sorgusu, dizide, tertip sırasındaki yerine konulur.</param>
        /// <param name="kodBilgileri">Seçilen kolonun, tertip içindeki yerini bulmak için, seçilen kodların bilgisini tutan yapı.</param>
        private static void KodKirilimEkle(int kirilimYer, string[] kirilimlarDizisi, ref int kolonNo, int[] kirilimSay, ref string aciklamaKod, ref int kodNo, Tertip kistas, ref int kistasYer, string[] kistaslar, UcgenKodBilgileri kodBilgileri)
        {
            if (kodNo == -1 || kolonNo >= kirilimSay[kodNo])
            {
                aciklamaKod = string.Empty;
                kodNo++;
                UcgenKodBilgi ukb = kodBilgileri[kodNo];
                kolonNo = 0;
                if (kistas != null)
                {
                    for (int kod = 0; kod < kistas.KodNoSay(); kod++)
                        if (ukb.kodNo == kistas.KodNolar(kod))
                        {
                            kistasYer = kod;
                            break;
                        }
                }
                else
                    kistasYer = -1;
            }
            kolonNo++;

            if (aciklamaKod != string.Empty)
                aciklamaKod += ".";
            aciklamaKod += kirilimlarDizisi[kirilimYer];
            if (kistasYer >= 0)
                kistaslar[kistasYer] = aciklamaKod.Replace(".", string.Empty);
        }

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Daha sonra bu kısmın 1., 2. 3. ve n. kırılımları alınarak üçgen format elde edilir.
        /// Her bir üçgen satır için ek bilgiler hesapllanır ve sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Değişken değeri true ise ek bilgilerinin hepsi sıfır olan tertipler sonuç kümesine alınmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde üçgen olarak ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        //public static UcgenSatir[] UcgenTertipOlEski(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric)
        //{
        //    int yil = kistasTertip.yil;
        //    ENUMTabloKod tabloNo = kistasTertip.tabloKod;
        //    int alanSay = kodBilgileri.BoyutSayisi;
        //    if (alanSay == 0) return null;
        //    string[] degerTablo = new string[alanSay];
        //    string[] degerTabloAlanAd = new string[alanSay];
        //    int[] degerSay = new int[alanSay];
        //    int kirilimSay = 0;
        //    string[] degerMaske = new string[alanSay];
        //    string ekKosul = string.Empty;
        //    ArrayList donusDeger = new ArrayList();

        //    string sql = UcgenSQLOl(kullanan, yil, tabloNo, kistasTertip, kodBilgileri, ref degerTablo, ref degerTabloAlanAd, ref degerSay, ref kirilimSay, ref degerMaske, ref ekKosul);
        //    //ekKosul = string.Empty;
        //    if (sql == string.Empty)
        //    {
        //        throw new Exception(ButceHazirlikResI.BHMUS002);
        //    }
        //    else if (sql.StartsWith(" "))
        //    {
        //        throw new Exception(sql);
        //    }

        //    VeriErisim ve = UcgenVeriErisimBul(veriErisim, yil, tabloNo);
        //    VeriErisim ve2 = UcgenVeriErisimBul(null, yil, ENUMTabloKod.TABPEBOdenek);

        //    IUZYServis uzS = TNS.UZY.Arac.Tanimla();

        //    string[] kistaslar = new string[kistasTertip.KodNoSay()];
        //    System.Collections.Hashtable hashTable = new System.Collections.Hashtable();

        //    IDataReader r = ve.DataReaderDondur(sql);
        //    if (ve.hata == string.Empty)
        //    {
        //        string[] eskiDegerler = new string[kirilimSay];
        //        string[] yeniDegerler = new string[kirilimSay];
        //        string tut;

        //        for (int i = 0; i < kirilimSay; i++) eskiDegerler[i] = "x";

        //        while (r.Read())
        //        {
        //            int toplamKirilim = 0;
        //            for (int i = 0; i < alanSay; i++)
        //            {
        //                tut = VeriErisim.DegerAl(r, i, string.Empty);
        //                tut = OrtakFonksiyonlar.KodNoktaKoy(degerMaske[i], tut);
        //                string[] d = tut.Split('.');
        //                for (int j = 0; j < d.Length; j++)
        //                {
        //                    yeniDegerler[toplamKirilim++] = d[j];
        //                }
        //            }
        //            string aciklamaKod = string.Empty;
        //            int kodNo = -1;
        //            int kodYer = -1;
        //            int kistasYer = -1;
        //            UcgenKodBilgi ukb;
        //            if (kistasTertip != null)
        //            {
        //                kistasTertip.DiziKontrol();
        //                for (int i = 0; i < kistasTertip.kodlar.Length; i++)
        //                    kistaslar[i] = string.Empty;
        //            }
        //            for (int kirilimYer = 0; kirilimYer < toplamKirilim; kirilimYer++)
        //            {
        //                KodKirilimEkle(kirilimYer, yeniDegerler, ref kodYer, degerSay, ref aciklamaKod, ref kodNo, kistasTertip, ref kistasYer, kistaslar, kodBilgileri);
        //                for (int i = 0; i < kistaslar.Length; i++)
        //                {
        //                    if (string.IsNullOrWhiteSpace(kistaslar[i]) && !string.IsNullOrWhiteSpace(kistasTertip.kodlar[i])) kistaslar[i] = kistasTertip.kodlar[i];
        //                }
        //                if (eskiDegerler[kirilimYer] != yeniDegerler[kirilimYer])
        //                {
        //                    for (; kirilimYer < toplamKirilim; kirilimYer++)
        //                    {
        //                        string kistaslarStr = string.Empty;
        //                        for (int i = 0; i < kistaslar.Length; i++)
        //                        {
        //                            kistaslarStr += kistaslar[i] + "-";
        //                        }
        //                        kistaslarStr = kistaslarStr.TrimEnd('-');

        //                        ukb = kodBilgileri[kodNo];
        //                        UcgenSatir us = new UcgenSatir();
        //                        us.yer = kirilimYer;
        //                        us.kodParca = yeniDegerler[kirilimYer];
        //                        us.aciklama = TNS.BHMv2.Arac.UcgenAciklamaBul(uzS, hashTable, kullanan, yil, kodNo, degerTablo[kodNo], degerTabloAlanAd[kodNo], aciklamaKod);

        //                        for (int e = 0; ekBilgiler != null && e < ekBilgiler.Length; e++)
        //                        {
        //                            if (ekBilgiler[e].HazirlikMi())
        //                            {
        //                                string sqlTek = ekBilgiler[e].EkDegerSqlDondur(uzS, kullanan, ve2.baglantiAd, yil, kistaslarStr);
        //                                if (sqlTek != string.Empty)
        //                                    us.ekDegerler.Add(ve2.TekKolonSQLDbl(sqlTek + ekKosul));
        //                                else
        //                                    us.ekDegerler.Add((double)-1);
        //                            }
        //                            else
        //                                us.ekDegerler.Add(ekBilgiler[e].EkDegerHesaplaDbl(uzS, kullanan, yil, kistaslarStr));
        //                        }
        //                        donusDeger.Add(us);
        //                        eskiDegerler[kirilimYer] = yeniDegerler[kirilimYer];
        //                        if (kirilimYer < toplamKirilim - 1)
        //                        {
        //                            KodKirilimEkle(kirilimYer + 1, yeniDegerler, ref kodYer, degerSay, ref aciklamaKod, ref kodNo, kistasTertip, ref kistasYer, kistaslar, kodBilgileri);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        r.Close();
        //    }
        //    else
        //    {
        //        ve.VeriErisimKapat();
        //        ve2.VeriErisimKapat();
        //        throw new Exception(string.Format(ButceHazirlikResI.BHMUS003, sql, ve.hata));
        //    }
        //    ve.VeriErisimKapat();
        //    ve2.VeriErisimKapat();
        //    return (UcgenSatir[])donusDeger.ToArray(typeof(UcgenSatir));
        //}

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Daha sonra bu kısmın 1., 2. 3. ve n. kırılımları alınarak üçgen format elde edilir.
        /// Her bir üçgen satır için ek bilgiler hesapllanır ve sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı bağlantı nesnesi, fonksiyon başlarken veritabanı bağlantısı 'VeriErisim.VeriErisimBul' çağrısı yapılarak açılmalıdır.</param>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Değişken değeri true ise ek bilgilerinin hepsi sıfır olan tertipler sonuç kümesine alınmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde üçgen olarak ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        public static UcgenSatir[] UcgenTertipOl(VeriErisim veriErisim, Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, int aciklamaKolon)
        {
            KistasYapi kistasYapi = new KistasYapi();
            kistasYapi.ekleTertipler = new Tertip[1];
            kistasYapi.ekleTertipler[0] = kistasTertip;
            int yil = kistasTertip.yil;
            ENUMTabloKod tabloNo = kistasTertip.tabloKod;

            OdenekBilgi[] obler = ListeTertipOlYeni(veriErisim, kullanan, kistasYapi, kodBilgileri, ekBilgiler, sifirlarHaric, false, true, aciklamaKolon);
            if (obler == null) return null;
            return TNS.BHMv2.Arac.UcgenTertipOl(kullanan, kodBilgileri, sifirlarHaric, obler, yil);
        }
    }
}
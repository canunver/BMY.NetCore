using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;
using TNS.BHMv2;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Yatırım alt projelerinin bütçe kaynak bilgilerini tutan YATBUTCEKAYNAK veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATButceKaynak : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATButceKaynak(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATBUTCEKAYNAK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// KAYNAKNO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KAYNAKNO = 0,

            /// <summary>
            /// ALTPROJENO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 1,

            /// <summary>
            /// KREDIYIVEREN alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KREDIYIVEREN = 2,

            /// <summary>
            /// BASLAMATARIHI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BASLAMATARIHI = 3,

            /// <summary>
            /// YURURLUKTARIHI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YURURLUKTARIHI = 4,

            /// <summary>
            /// TURU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TURU = 5,

            /// <summary>
            /// TUTARI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TUTARI = 6,

            /// <summary>
            /// VADESI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            VADESI = 7,

            /// <summary>
            /// ODEMESIZDONEMI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ODEMESIZDONEMI = 8,

            /// <summary>
            /// FAIZORAN alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FAIZORAN = 9,

            /// <summary>
            /// ICPARAKREDIORAN alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ICPARAKREDIORAN = 10,

            /// <summary>
            /// KAYNAKTUR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KAYNAKTUR = 11,

            /// <summary>
            /// TOPLAMADAHIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMADAHIL = 12,

            /// <summary>
            /// KAYNAKAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KAYNAKAD = 13,
            DPTRAPORDAGOR = 14,
            YIL = 15,
            SENARYOKOD = 16,
            PBKOD = 17,
            DIPNOT = 18,
            YIKOBMU = 19
        }

        /// <summary>
        /// Bütçe kaynağın tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KAYNAKNO", "ALTPROJENO", "KREDIYIVEREN", "BASLAMATARIHI", "YURURLUKTARIHI", "TURU", "TUTARI",
                                     "VADESI", "ODEMESIZDONEMI", "FAIZORAN", "ICPARAKREDIORAN", "KAYNAKTUR", "TOPLAMADAHIL", "KAYNAKAD", "DPTRAPORDAGOR", "YIL", "SENARYOKOD", "PBKOD", "DIPNOT", "YIKOBMU" };

        /// <summary>
        /// Bütçe kaynak bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Bütçe kaynağın programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="kaynak">Bütçe kaynak bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string senaryoKod, ButceKaynak kaynak)
        {
            int kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYNAKNO]);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(kaynak.altProjeKodu) + ",";
            sql += VeriErisim.DegerYap(kaynak.krediyiVeren) + ",";
            sql += VeriErisim.DegerYap(kaynak.baslamaTarih) + ",";
            sql += VeriErisim.DegerYap(kaynak.yururlukTarih) + ",";
            sql += VeriErisim.DegerYap(kaynak.tur) + ",";
            sql += VeriErisim.DegerYap(kaynak.tutar) + ",";
            sql += VeriErisim.DegerYap(kaynak.vade) + ",";
            sql += VeriErisim.DegerYap(kaynak.odemesizDonem) + ",";
            sql += VeriErisim.DegerYap(kaynak.faizOran) + ",";
            sql += VeriErisim.DegerYap(kaynak.IcParaKrediOran) + ",";
            sql += VeriErisim.DegerYap((int)kaynak.kaynakTur) + ",";
            sql += VeriErisim.DegerYap((int)kaynak.toplamaDahil) + ",";
            sql += VeriErisim.DegerYap(kaynak.kaynakAd) + ",";
            sql += VeriErisim.DegerYap(kaynak.DPTRapordaGor ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(kaynak.pbKod) + ",";
            sql += VeriErisim.DegerYap(kaynak.dipnot) + ",";
            sql += VeriErisim.DegerYap(kaynak.yikobMu) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan bütçe kaynak üst bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="kaynak">Bütçe kaynak bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(int yil, string senaryoKod, ButceKaynak kaynak)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKTUR], (int)kaynak.kaynakTur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMADAHIL], (int)kaynak.toplamaDahil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKAD], kaynak.kaynakAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIKOBMU], kaynak.yikobMu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DPTRAPORDAGOR], kaynak.DPTRapordaGor ? 1 : 0);

            //sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KREDIYIVEREN], kaynak.krediyiVeren) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI], kaynak.baslamaTarih) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YURURLUKTARIHI], kaynak.yururlukTarih) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURU], kaynak.tur) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTARI], kaynak.tutar) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VADESI], kaynak.vade) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMESIZDONEMI], kaynak.odemesizDonem) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZORAN], kaynak.faizOran) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICPARAKREDIORAN], kaynak.IcParaKrediOran) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PBKOD], kaynak.pbKod) + ",";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIPNOT], kaynak.dipnot);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynak.kaynakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = kaynak.kaynakNo.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan bütçe kaynak detay kimlik bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="kaynak">Bütçe kaynak bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GuncelleTanim(int yil, string senaryoKod, ButceKaynak kaynak)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KREDIYIVEREN], kaynak.krediyiVeren) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI], kaynak.baslamaTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YURURLUKTARIHI], kaynak.yururlukTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURU], kaynak.tur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTARI], kaynak.tutar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VADESI], kaynak.vade) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMESIZDONEMI], kaynak.odemesizDonem) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAIZORAN], kaynak.faizOran) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PBKOD], kaynak.pbKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DIPNOT], kaynak.dipnot) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIKOBMU], kaynak.yikobMu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICPARAKREDIORAN], kaynak.IcParaKrediOran);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynak.kaynakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen değişkenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve bütçe kaynakları DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="altProjeKodu">Yatırım alt projesi programatik kodu</param>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="tur">Listelenmek istenen bütçe kaynak türleri dizisi</param>
        /// <returns>Verilen kriterlere uygun olan bütçe kaynakları tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string senaryoKod, int projeNo, int altProjeKodu, int kaynakNo, ENUMButceKaynakTur[] tur, string sektorKod, string projeAd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", YATALTPROJE.AD, YATPROJE.DPTPROJENO, YATPROJE.AD, YATALTPROJE.BIRIM, PB.ADI, PB.KISALTMA, YATPROJE.PROJENO, YATPROJE.SEKTORNO, YATSEKTOR.AD";
            sql += " FROM YATPROJE INNER JOIN YATALTPROJE ON " + VeriErisim.Join("YATPROJE", "YATALTPROJE", new string[] { "PROJENO", "YIL", "SENARYOKOD" }) + " INNER JOIN " + TABLOAD + " ON " + VeriErisim.Join("YATALTPROJE", TABLOAD, new string[] { "ALTPROJENO", "YIL", "SENARYOKOD" }) + " LEFT OUTER JOIN  HarcamaYonetimi.dbo.HRCPARABIRIMI PB ON " + VeriErisim.Join("PB", TABLOAD, new string[] { "KOD" }, new string[] { "PBKOD" });
            sql += " LEFT OUTER JOIN YATSEKTOR ON " + VeriErisim.Join("YATSEKTOR", "YATPROJE", new string[] { "YIL", "SEKTORNO" });

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, "SENARYOKOD", "=", senaryoKod.Replace(".", "")));

            if (!string.IsNullOrWhiteSpace(projeAd))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATPROJE", "AD", "LIKE", projeAd + "%"));
            if (projeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATPROJE", "PROJENO", "=", projeNo));

            if (altProjeKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeKodu));
            if (kaynakNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));

            if (!string.IsNullOrWhiteSpace(sektorKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATPROJE", "SEKTORNO", "LIKE", sektorKod.Replace(".", "") + "%"));

            if (tur != null)
            {
                string ekKosul = string.Empty;
                foreach (ENUMButceKaynakTur o in tur)
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYNAKTUR], "=", (int)o));
                if (!string.IsNullOrEmpty(ekKosul))
                    sql += " AND (" + ekKosul + ")";
            }

            sql += " ORDER BY YATPROJE.DPTPROJENO, YATPROJE.AD, YATALTPROJE.ALTPROJENO, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO];
            return veriErisim.DataReaderDondur(sql);
        }

        internal static string Listele(string yilUstAlanAd, string senaryoUstAlanAd, string altProjeUstAlanAd, int butceYil, string senaryoKod, bool tekYil, bool eskiYillarKBO, int ekoUz)
        {
            string pebKodSorgu = TABOrtak.IcSorguOl(ListelePebKod(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO]));
            string odenekIslemSorgu = TABOrtak.IcSorguOl(OdenekIslemListele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO], false, ekoUz));
            string harcamaSorgu = TABOrtak.IcSorguOl(HarcamaListele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO], ekoUz));
            string odenekSorgu = TABOrtak.IcSorguOl(OdenekListele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO], ekoUz));
            //pebKodSorgu = "NULL";
            string teklifSorgu = TABOrtak.IcSorguOl(TABYATButceKaynakTeklif.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO], butceYil, senaryoKod, tekYil, eskiYillarKBO));
            string yeniTeklifSorgu = TABOrtak.IcSorguOl(TABYATYENIPROJEBUTCE.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO]));

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", YATALTPROJEIC.AD, YATPROJEIC.DPTPROJENO, YATPROJEIC.AD, YATALTPROJEIC.BIRIM, PB.ADI, PB.KISALTMA, YATPROJEIC.PROJENO, YATPROJEIC.SEKTORNO, YATSEKTORIC.AD";
            sql += "," + pebKodSorgu + " XML_PEBKODLAR";
            sql += "," + teklifSorgu + " XML_TEKLIFLER";
            sql += "," + yeniTeklifSorgu + " XML_YENITEKLIFLER";
            sql += "," + odenekIslemSorgu + " XML_ODENEKDEGISIMLER";
            sql += "," + harcamaSorgu + " XML_HARCAMA";
            sql += "," + odenekSorgu + " XML_ODENEKLER";

            sql += " FROM YATPROJE YATPROJEIC INNER JOIN YATALTPROJE YATALTPROJEIC ON " + VeriErisim.Join("YATPROJEIC", "YATALTPROJEIC", new string[] { "PROJENO", "YIL", "SENARYOKOD" });
            sql += " INNER JOIN " + TABLOAD + " ON " + VeriErisim.Join("YATALTPROJEIC", TABLOAD, new string[] { "ALTPROJENO", "YIL", "SENARYOKOD" });
            sql += " LEFT OUTER JOIN  HarcamaYonetimi.dbo.HRCPARABIRIMI PB ON " + VeriErisim.Join("PB", TABLOAD, new string[] { "KOD" }, new string[] { "PBKOD" });
            sql += " LEFT OUTER JOIN YATSEKTOR YATSEKTORIC ON " + VeriErisim.Join("YATSEKTORIC", "YATPROJEIC", new string[] { "YIL", "SEKTORNO" });

            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yilUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO] + "=" + altProjeUstAlanAd);
            sql += " ORDER BY 1,2";

            return sql;
        }

        private static string HarcamaListele(string yilUstAlanAd, string senaryoUstAlanAd, string kaynakNoUstAlanAd, int ekoUz)
        {
            string ekoKod = "''";
            string grupEk = "";
            if (ekoUz > 0)
            {
                ekoKod = "LEFT(EKOKOD," + ekoUz.ToString() + ")";
                grupEk = "," + ekoKod;
            }
            string sql = "SELECT DATEPART(m, ISLEMTARIH) AY, FINKOD, " + ekoKod + " EKOKOD, SUM(BORC-ALACAK) ODENEK FROM YATBUTCEKAYNAKPEBKOD KAYPEB";
            sql += " INNER JOIN [ButceIslemleri].[dbo].[MUHMIF] MPT ON MPT.YIL=KAYPEB.YIL AND MPT.PEBKOD=KAYPEB.PEBKOD AND MPT.HESAPKOD LIKE '830%' ";
            sql += " INNER JOIN [ButceIslemleri].[dbo].[MUHMIFFORM] RF ON RF.YIL=MPT.YIL AND RF.BELGENO=MPT.BELGENO AND RF.FORMDURUM <= 6";
            sql = VeriErisim.KosulEkle(sql, "WHERE", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO] + "=" + kaynakNoUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yilUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstAlanAd);
            sql += " GROUP BY DATEPART(m, ISLEMTARIH), FINKOD" + grupEk;
            sql += " ORDER BY 1, 2";
            return sql;
        }

        private static string OdenekIslemListele(string yilUstAlanAd, string senaryoUstAlanAd, string kaynakNoUstAlanAd, bool ahpAy, int ekoUz)
        {
            string ayAd;
            if (ahpAy)
                ayAd = "AY";
            else
                ayAd = "DATEPART(m, KAYITTARIH)";

            string ekoKod = "''";
            string grupEk = "";
            if (ekoUz > 0)
            {
                ekoKod = "LEFT(EKOKOD," + ekoUz.ToString() + ")";
                grupEk = "," + ekoKod;
            }

            string sql = "SELECT " + ayAd + " AY, FINKOD, " + ekoKod + " EKOKOD, SUM(ODENEK) ODENEK FROM YATBUTCEKAYNAKPEBKOD KAYPEB";
            sql += " INNER JOIN [ButceIslemleri].[dbo].[MPGMASRAFPLANITERTIP] MPT ON MPT.YIL=KAYPEB.YIL AND MPT.PEBKOD=KAYPEB.PEBKOD AND MPT.ASAMA=5 AND MPT.DURUM=5 AND MPT.REVIZENO>0";
            sql += " INNER JOIN [ButceIslemleri].[dbo].[MPGMASRAFPLANIREVIZEFORM] RF ON RF.YIL=MPT.YIL AND RF.REVIZENO=MPT.REVIZENO AND BELGETUR IN (3,4,7,8)";
            sql = VeriErisim.KosulEkle(sql, "WHERE", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO] + "=" + kaynakNoUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yilUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstAlanAd);
            sql += " GROUP BY " + ayAd + ", FINKOD" + grupEk;
            sql += " ORDER BY 1,2,3";
            return sql;
        }

        private static string OdenekListele(string yilUstAlanAd, string senaryoUstAlanAd, string kaynakNoUstAlanAd, int ekoUz)
        {
            string ekoKod = "''";
            string grupEk = "";
            if (ekoUz > 0)
            {
                ekoKod = "LEFT(EKOKOD," + ekoUz.ToString() + ")";
                grupEk = "," + ekoKod;
            }

            string sql = "SELECT 0 AY, FINKOD, " + ekoKod + " EKOKOD, SUM(ODENEK) ODENEK FROM YATBUTCEKAYNAKPEBKOD KAYPEB";
            sql += " INNER JOIN [ButceHazirlikV2].[dbo].[BHMODENEK] O ON O.YIL=KAYPEB.YIL AND O.PEBKOD=KAYPEB.PEBKOD AND O.SENARYOKOD='0600' AND O.TEKLIFYIL=0";
            sql = VeriErisim.KosulEkle(sql, "WHERE", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO] + "=" + kaynakNoUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yilUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", "KAYPEB." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstAlanAd);
            sql += " GROUP BY FINKOD" + grupEk;
            sql += " ORDER BY 1,2,3";
            return sql;
        }

        /// <summary>
        /// Verilen bütçe kaynağın programatik kodunu kullanarak,
        /// ilgili bütçe kaynağı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string senaryoKod, int kaynakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilPebKod(int yil, string senaryoKod, int kaynakNo)
        {
            string sql = "DELETE FROM YATBUTCEKAYNAKPEBKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verile alt faaliyet nesnesini veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="pebKod">Alt faaliyet nesnesini </param>
        /// <returns>Sonuc nesnesinde kayıt sonucunu döndürür.</returns>
        public Sonuc KaydetPebKod(AltFaaliyet pebKod, string senaryoKod)
        {
            string sql = "INSERT INTO YATBUTCEKAYNAKPEBKOD (YIL, SENARYOKOD, PEBKOD, KAYNAKNO) VALUES (";
            sql += VeriErisim.DegerYap(pebKod.yil) + ",";
            sql += VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(pebKod.pebKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(pebKod.butceKaynakNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public static string ListelePebKod(string yilUstAlanAd, string senaryoUstAlanAd, string kaynakNoUstAlanAd)
        {
            string sql = "SELECT YATBUTCEKAYNAKPEBKOD.PEBKOD, YATBUTCEKAYNAKIC2.ALTPROJENO, ABSPEBKOD.PEBAD FROM YATBUTCEKAYNAKPEBKOD LEFT OUTER JOIN";
            sql += " YATBUTCEKAYNAK YATBUTCEKAYNAKIC2 ON YATBUTCEKAYNAKPEBKOD.YIL = YATBUTCEKAYNAKIC2.YIL AND YATBUTCEKAYNAKPEBKOD.SENARYOKOD = YATBUTCEKAYNAKIC2.SENARYOKOD AND";
            sql += " YATBUTCEKAYNAKPEBKOD.KAYNAKNO = YATBUTCEKAYNAKIC2.KAYNAKNO LEFT OUTER JOIN";
            sql += " ABSPEBKOD ON YATBUTCEKAYNAKPEBKOD.YIL = ABSPEBKOD.YIL AND YATBUTCEKAYNAKPEBKOD.PEBKOD = ABSPEBKOD.PEBKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", "YATBUTCEKAYNAKPEBKOD." + ALANLAR[(int)ENUMAlanlar.KAYNAKNO] + "=" + kaynakNoUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", "YATBUTCEKAYNAKPEBKOD." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yilUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", "YATBUTCEKAYNAKPEBKOD." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstAlanAd);

            return sql;
        }

        public IDataReader ListelePebKod(int kaynakNo, int yil, string senaryoKod)
        {
            string sql = "SELECT YATBUTCEKAYNAKPEBKOD.PEBKOD, YATBUTCEKAYNAK.ALTPROJENO, ABSPEBKOD.PEBAD FROM YATBUTCEKAYNAKPEBKOD LEFT OUTER JOIN";
            sql += " YATBUTCEKAYNAK ON YATBUTCEKAYNAKPEBKOD.YIL = YATBUTCEKAYNAK.YIL AND YATBUTCEKAYNAKPEBKOD.SENARYOKOD = YATBUTCEKAYNAK.SENARYOKOD AND";
            sql += " YATBUTCEKAYNAKPEBKOD.KAYNAKNO = YATBUTCEKAYNAK.KAYNAKNO LEFT OUTER JOIN";
            sql += " ABSPEBKOD ON YATBUTCEKAYNAKPEBKOD.YIL = ABSPEBKOD.YIL AND YATBUTCEKAYNAKPEBKOD.PEBKOD = ABSPEBKOD.PEBKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("YATBUTCEKAYNAKPEBKOD", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKPEBKOD", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKPEBKOD", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen bütçe kaynağın programatik kodunu kullanarak
        /// bütçe kaynağın veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool KaynakVarMi(int yil, string senaryoKod, int kaynakNo)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.KAYNAKNO] };
            int[] intDeger = { yil, kaynakNo };

            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SENARYOKOD] };
            string[] strDeger = { senaryoKod.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Verilen bütçe kaynağın, bütçe kaynak detay tablolarında kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="kaynakNo">Bütçe kaynağın programatik kodu</param>
        /// <returns>Bütçe kaynak; bütçe kaynak detay tablolarında kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool KaynakKullanilmisMi(int yil, string senaryoKod, int kaynakNo)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.KAYNAKNO] };
            int[] intDeger = { yil, kaynakNo };

            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SENARYOKOD] };
            string[] strDeger = { senaryoKod.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, "YATBUTCEKAYNAKPEBKOD", strAlan, strDeger, intAlan, intDeger)
                || TABGenel.KayitVarmi(veriErisim, "YATBUTCEKAYNAKTEKLIF", strAlan, strDeger, intAlan, intDeger)
                || TABGenel.KayitVarmi(veriErisim, "YATBUTCEKAYNAKGERIODEME", strAlan, strDeger, intAlan, intDeger);

        }

        public IDataReader PebKodProjeBilgiBul(int yil, string senaryoKod, string pebKod, string altProjeSahipKurKod)
        {
            string sql = "SELECT YATSEKTOR.SEKTORNO, YATSEKTOR.AD,  P.DPTPROJENO, P.AD, AP.ALTPROJENO, AP.AD, AP.PROJENO ";
            sql += " FROM YATBUTCEKAYNAKPEBKOD AS K INNER JOIN YATBUTCEKAYNAK AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " INNER JOIN YATALTPROJE AS AP ON " + VeriErisim.Join("BK", "AP", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " INNER JOIN YATPROJE AS P ON " + VeriErisim.Join("AP", "P", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            sql += " INNER JOIN YATSEKTOR ON " + VeriErisim.Join("P", "YATSEKTOR", new string[] { "YIL", "SEKTORNO" });
            sql += " WHERE (K.YIL = " + VeriErisim.DegerYap(yil) + ") ";
            sql += " AND (K.SENARYOKOD = " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ")";
            sql += " AND (K.PEBKOD = " + VeriErisim.DegerYap(pebKod.Replace(".", "")) + ")";
            sql += " AND (AP.BIRIM LIKE " + VeriErisim.DegerYap(altProjeSahipKurKod.Replace(".", "") + "%") + ")";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader MasrafPlaniTertipBilgi(MPRevizeFormSorgu form, string formSorgu, int eko2Uz)
        {
            string sql = $"SELECT D.KURKOD, D.FONKOD, D.FINKOD, left(D.EKOKOD, {eko2Uz}), D.PEBKOD, D.KURUMSALYAPIKOD, sum(D.ODENEK) as MIKTAR, MPGMASRAFPLANIREVIZEFORM.REVIZENO, MPGMASRAFPLANIREVIZEFORM.KAYITTARIH, MPGMASRAFPLANIREVIZEFORM.BELGETUR, MPGMASRAFPLANIREVIZEFORM.ACIKLAMA, MPGMASRAFPLANIREVIZEFORM.BUMKOISLEMNO, MPGMASRAFPLANIREVIZEFORM.ALTISLEMKOD, MPGMASRAFPLANIREVIZEFORM.FORMDURUM FROM ";
            sql += " MPGMASRAFPLANIREVIZEFORM AS MPGMASRAFPLANIREVIZEFORM INNER JOIN MPGMASRAFPLANITERTIP AS D ON " + "MPGMASRAFPLANIREVIZEFORM.YIL = D.YIL AND MPGMASRAFPLANIREVIZEFORM.REVIZENO = D.REVIZENO ";
            sql += " WHERE ";
            if (form.detayGoster)
            {
                sql += "(MPGMASRAFPLANIREVIZEFORM.YIL = " + VeriErisim.DegerYap(form.yil) + ") AND MPGMASRAFPLANIREVIZEFORM.REVIZENO IN (SELECT REVIZENO FROM MPGMASRAFPLANIREVIZEFORM IC WHERE " + formSorgu + ")";
            }
            else
            {
                sql += formSorgu;

                sql += TertipParcaKosulOl("D.KURKOD", form.tertip.kodlar[0]);
                sql += TertipParcaKosulOl("D.FONKOD", form.tertip.kodlar[1]);
                sql += TertipParcaKosulOl("D.FINKOD", form.tertip.kodlar[2]);
                sql += TertipParcaKosulOl("D.EKOKOD", form.tertip.kodlar[3]);
                sql += TertipParcaKosulOl("D.PEBKOD", form.tertip.kodlar[4]);
            }
            sql += $" GROUP BY D.KURKOD,D.FONKOD,D.FINKOD,left(D.EKOKOD, {eko2Uz}),D.PEBKOD,D.KURUMSALYAPIKOD, MPGMASRAFPLANIREVIZEFORM.REVIZENO, MPGMASRAFPLANIREVIZEFORM.KAYITTARIH, MPGMASRAFPLANIREVIZEFORM.BELGETUR, MPGMASRAFPLANIREVIZEFORM.ACIKLAMA, MPGMASRAFPLANIREVIZEFORM.BUMKOISLEMNO, MPGMASRAFPLANIREVIZEFORM.ALTISLEMKOD, MPGMASRAFPLANIREVIZEFORM.FORMDURUM ";
            sql += " HAVING SUM(D.ODENEK)<>0 ";
            sql += $" ORDER BY MPGMASRAFPLANIREVIZEFORM.REVIZENO,D.KURKOD,D.FONKOD,D.FINKOD,left(D.EKOKOD, {eko2Uz}),D.PEBKOD,D.KURUMSALYAPIKOD ";

            return veriErisim.DataReaderDondur(sql);
        }

        private string TertipParcaKosulOl(string alan, string deger)
        {
            if (string.IsNullOrEmpty(deger)) return "";
            deger = deger.Replace(".", "");

            string[] degerler = deger.Split(',');
            string icKosul = "";
            for (int i = 0; i < degerler.Length; i++)
            {
                string icDeger = degerler[i];
                if (!string.IsNullOrEmpty(icDeger))
                {
                    if (icKosul != "") icKosul += " OR ";
                    icKosul += alan + " LIKE " + VeriErisim.DegerYap(icDeger + "%");
                }
            }
            return " AND (" + icKosul + ")";
        }

        public Sonuc KaynakKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO " + TABLOAD + "(YIL, SENARYOKOD, KAYNAKNO, ALTPROJENO, KREDIYIVEREN, BASLAMATARIHI, YURURLUKTARIHI, TURU, TUTARI, VADESI, ODEMESIZDONEMI, FAIZORAN, ICPARAKREDIORAN, KAYNAKTUR, TOPLAMADAHIL, KAYNAKAD, DPTRAPORDAGOR, PBKOD, DIPNOT, YIKOBMU) ";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, KAYNAKNO, ALTPROJENO, KREDIYIVEREN, BASLAMATARIHI, YURURLUKTARIHI, TURU, TUTARI, VADESI, ODEMESIZDONEMI, FAIZORAN, ICPARAKREDIORAN, KAYNAKTUR, TOPLAMADAHIL, KAYNAKAD, DPTRAPORDAGOR, PBKOD, DIPNOT, YIKOBMU FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", "(1 NOT in (SELECT 1 FROM " + TABLOAD + $" IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.KAYNAKNO={TABLOAD}.KAYNAKNO AND IC.ALTPROJENO ={TABLOAD}.ALTPROJENO))");
            if (projeKodListe != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + AltProjeProjeyeBagla(projeKodListe, TABLOAD) + "))");
            }

            return KomutCalistir(sql, 0);
        }

        private string AltProjeProjeyeBagla(string projeKodListe, string tabloAd)
        {
            string sql = "SELECT 1 FROM YATALTPROJE WHERE YATALTPROJE.YIL = " + tabloAd + ".YIL AND YATALTPROJE.SENARYOKOD = " + tabloAd + ".SENARYOKOD AND YATALTPROJE.ALTPROJENO = " + tabloAd + ".ALTPROJENO AND YATALTPROJE.PROJENO IN (" + projeKodListe + ")";

            return sql;
        }

        public Sonuc PebKodKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO YATBUTCEKAYNAKPEBKOD(YIL, SENARYOKOD, PEBKOD, KAYNAKNO) ";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, PEBKOD, KAYNAKNO FROM YATBUTCEKAYNAKPEBKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", $"(1 NOT in (SELECT 1 FROM YATBUTCEKAYNAKPEBKOD IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.PEBKOD =YATBUTCEKAYNAKPEBKOD.PEBKOD AND IC.KAYNAKNO=YATBUTCEKAYNAKPEBKOD.KAYNAKNO))");
            if (projeKodListe != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + KaynakProjeyeBagla(projeKodListe, "YATBUTCEKAYNAKPEBKOD", kaynakYil, kaynakAsama) + "))");
            }
            return KomutCalistir(sql, 0);
        }

        public static string KaynakProjeyeBagla(string projeKodListe, string tabloAd, int bagYil, string sernaryoKod)
        {
            string sql = "SELECT 1 FROM YATBUTCEKAYNAK,YATALTPROJE WHERE YATBUTCEKAYNAK.YIL = " + VeriErisim.DegerYap(bagYil) + " AND YATBUTCEKAYNAK.SENARYOKOD = " + VeriErisim.DegerYap(sernaryoKod.Replace(".", "")) + " AND YATBUTCEKAYNAK.KAYNAKNO = " + tabloAd + ".KAYNAKNO AND YATALTPROJE.YIL = YATBUTCEKAYNAK.YIL AND YATALTPROJE.SENARYOKOD = YATBUTCEKAYNAK.SENARYOKOD AND YATALTPROJE.ALTPROJENO = YATBUTCEKAYNAK.ALTPROJENO AND YATALTPROJE.PROJENO IN (" + projeKodListe + ")";

            return sql;
        }

        public Sonuc Tasi(int yil, string senaryoKod, int kaynakNo, int apKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET ALTPROJENO = " + VeriErisim.DegerYap(apKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAYNAKNO], "=", kaynakNo));
            return KomutCalistir(sql, 0);
        }

        public Sonuc GeriOdemeKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO YATBUTCEKAYNAKGERIODEME(KAYNAKNO, YIL, SENARYOKOD, ODEMEYIL, ANAPARAGERIODEME, ODENECEKFAIZ, ALTPROJENINSONDURUMU, DARBOGAZCOZUM) ";
            sql += "SELECT KAYNAKNO, " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, ODEMEYIL, ANAPARAGERIODEME, ODENECEKFAIZ, ALTPROJENINSONDURUMU, DARBOGAZCOZUM FROM YATBUTCEKAYNAKGERIODEME";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql = VeriErisim.KosulEkle(sql, "AND", $"(1 NOT in (SELECT 1 FROM YATBUTCEKAYNAKGERIODEME IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.KAYNAKNO =YATBUTCEKAYNAKGERIODEME.KAYNAKNO))");

            if (projeKodListe != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + KaynakProjeyeBagla(projeKodListe, "YATBUTCEKAYNAKGERIODEME", kaynakYil, kaynakAsama) + "))");
            }
            return KomutCalistir(sql, 0);
        }

        public Sonuc KrediDetayKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO YATKREDIDETAY(BUTCEYIL, SENARYOKOD, KAYNAKNO, YIL, ANLASMATUTARI, KULLANILANTUTAR, KULLANILACAKTUTAR, ANLASMATUTARIIC, KULLANILANTUTARIC, KULLANILACAKTUTARIC) ";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " BUTCEYIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, KAYNAKNO, YIL, ANLASMATUTARI, KULLANILANTUTAR, KULLANILACAKTUTAR, ANLASMATUTARIIC, KULLANILANTUTARIC, KULLANILACAKTUTARIC FROM YATKREDIDETAY";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "BUTCEYIL", "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", $"(1 NOT in (SELECT 1 FROM YATKREDIDETAY IC WHERE IC.BUTCEYIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.KAYNAKNO =YATKREDIDETAY.KAYNAKNO))");
            if (projeKodListe != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + KaynakProjeyeBagla(projeKodListe, "YATKREDIDETAY", kaynakYil, kaynakAsama) + "))");
            }
            return KomutCalistir(sql, 0);
        }
    }
}
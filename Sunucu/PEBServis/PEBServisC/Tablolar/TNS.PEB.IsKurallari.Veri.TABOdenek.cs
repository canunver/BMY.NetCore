using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using TNS.PEB;
using TNS.BKD;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Faaliyet/projelerin ve sabit maliyetlerin ödenek bilgilerini tutan PEBODENEK
    /// veri tabanı tablosu ile ilgili listeleme ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABOdenek : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOdenek(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBODENEK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// MALIYETKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MALIYETKURKOD = 1,

            /// <summary>
            /// SENARYOKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 2,

            /// <summary>
            /// MALIYETKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MALIYETKOD = 3,

            /// <summary>
            /// SABITMI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SABITMI = 4,

            /// <summary>
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 5,

            /// <summary>
            /// FONKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 6,

            /// <summary>
            /// FINKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 7,

            /// <summary>
            /// EKOKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 8,

            /// <summary>
            /// ODENEK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ODENEK = 9,

            /// <summary>
            /// MALIYETFONKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MALIYETFONKOD = 10
        }

        /// <summary>
        /// Faaliyet/projelerin ve sabit maliyetlerin ödenek bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MALIYETKURKOD", "SENARYOKOD", "MALIYETKOD", "SABITMI", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEK", "MALIYETFONKOD" };

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve ilgili faaliyet/projeye veya sabit maliyete ait ödenek bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="maliyetKod">Faaliyet/proje veya sabit maliyet programatik kodu</param>
        /// <param name="maliyetTur">Faaliyet/proje mi (0 değeri) sabit maliyet mi (1 değeri) bilgisi</param>
        /// <returns>Verilen kriterlere uygun olan ödenek bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string kurKod, string senaryoKod, int maliyetKod, int maliyetTur)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD], "=", kurKod.Replace(".", "")));
            if (maliyetKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKOD], "=", maliyetKod));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "=", maliyetTur));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += "," + ALANLAR[(int)ENUMAlanlar.FONKOD];
            sql += "," + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            sql += "," + ALANLAR[(int)ENUMAlanlar.FINKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen sabit maliyeti kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve ilgili
        /// sabit maliyetin ödenek toplamını döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="smKod">Sabit maliyetin programatik kodu</param>
        /// <returns>Sabit maliyetin ödenek toplamı döndürülür.</returns>
        public double SabitMaliyetToplami(int yil, string kurKod, string senaryoKod, int smKod)
        {
            return ToplamAl(yil, kurKod, senaryoKod, smKod, (int)ENUMOdenekTur.SabitMaliyet);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve ilgili faaliyet/projeye veya sabit maliyete ait ödenek toplamını döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="maliyetKod">Faaliyet/proje veya sabit maliyet programatik kodu</param>
        /// <param name="maliyetTur">Faaliyet/proje mi (0 değeri) sabit maliyet mi (1 değeri) bilgisi</param>
        /// <returns>Faaliyet/projeye veya sabit maliyete ait ödenek toplamı döndürülür.</returns>
        private double ToplamAl(int yil, string kurKod, string senaryoKod, int maliyetKod, int maliyetTur)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";
            sql += " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD], "LIKE", kurKod.Replace(".", "") + "%"));
            if (maliyetKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKOD], "=", maliyetKod));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "=", maliyetTur));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve kriterlere uyan faaliyetlerin personel giderleri kaleminin ödenek toplamını döndüren yordam
        /// tur parametresine bakarak ENUMFaaliyetProjeTur listesindeki hangi değerlerin toplama dahil edileceğine karar verir.
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">ENUMSabitMaliyetTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>İlgili faaliyetlerin personel giderleri kaleminin ödenek toplamı döndürülür.</returns>
        public double YansitilacakFaaliyetlerToplami(int yil, string kurKod, string senaryoKod, int tur)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";
            sql += " FROM " + "PEBFAALIYETPROJE FP, " + TABLOAD;
            sql += " WHERE " + "FP.YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += " AND " + "FP.KURKOD" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD];
            sql += " AND " + "FP.FPKOD" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("FP", "PROJEMI", "=", (int)ENUMFaaliyetProje.Faaliyet));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "=", (int)ENUMOdenekTur.FaaliyetProje));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", personelGider + "%"));

            if (tur == (int)ENUMSabitMaliyetTur.Birim)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD], "=", kurKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("FP", "TUR", "<>", (int)ENUMFaaliyetProjeTur.DigerKurumIci));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("FP", "TUR", "<>", (int)ENUMFaaliyetProjeTur.DigerKurumDisi));
            }
            else if (tur == (int)ENUMSabitMaliyetTur.Merkez)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.Merkez));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.MerkezTasra));
                sql += ")";
            }
            else if (tur == (int)ENUMSabitMaliyetTur.Tasra)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.Tasra));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.MerkezTasra));
                sql += ")";
            }
            else if (tur == (int)ENUMSabitMaliyetTur.MerkezTasra)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.Merkez));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.Tasra));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.MerkezTasra));
                sql += ")";
            }
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        /// kriterlere uyan faaliyetleri ve personel giderleri kalemindeki ödenek toplamlarını DataReader yapısında döndüren yordam
        /// tur parametresine bakarak ENUMFaaliyetProjeTur listesindeki hangi değerlere sahip faaliyetleri döndüreceğine karar verir.
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">ENUMSabitMaliyetTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>İlgili faaliyetleri ve personel giderleri kalemindeki ödenek toplamlarını tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader YansitilacakFaaliyetler(int yil, string kurKod, string senaryoKod, int tur)
        {
            string sql = "SELECT FP.YIL, FP.KURKOD, FP.FPKOD, FP.FPAD";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";
            sql += " FROM " + "PEBFAALIYETPROJE FP LEFT JOIN " + TABLOAD;
            sql += " ON " + "FP.YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += " AND " + "FP.KURKOD" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD];
            sql += " AND " + "FP.FPKOD" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "=", (int)ENUMOdenekTur.FaaliyetProje));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", personelGider + "%"));

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("FP", "PROJEMI", "=", (int)ENUMFaaliyetProje.Faaliyet));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("FP", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (tur == (int)ENUMSabitMaliyetTur.Birim)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD], "=", kurKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("FP", "TUR", "<>", (int)ENUMFaaliyetProjeTur.DigerKurumIci));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("FP", "TUR", "<>", (int)ENUMFaaliyetProjeTur.DigerKurumDisi));
            }
            else if (tur == (int)ENUMSabitMaliyetTur.Merkez)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.Merkez));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.MerkezTasra));
                sql += ")";
            }
            else if (tur == (int)ENUMSabitMaliyetTur.Tasra)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.Tasra));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.MerkezTasra));
                sql += ")";
            }
            else if (tur == (int)ENUMSabitMaliyetTur.MerkezTasra)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.Merkez));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.Tasra));
                sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("FP", "TUR", "=", (int)ENUMFaaliyetProjeTur.MerkezTasra));
                sql += ")";
            }

            sql += "GROUP BY";
            sql += " FP.YIL";
            sql += ", FP.KURKOD";
            sql += ", FP.FPKOD";
            sql += ", FP.FPAD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        /// kriterlere uyan faaliyetleri ve ödenek toplamlarını DataReader yapısında döndüren yordam
        /// tur parametresine bakarak ENUMFaaliyetProjeTur listesindeki hangi değerlere sahip faaliyetleri döndüreceğine karar verir.
        /// Ödenekler e-butce sisteminin istediği yapıda yani ekonomik kodun ilk kırılımı bazında toplanıyor.
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <param name="tur">ENUMFaaliyetProjeTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>İlgili faaliyetleri ve ödenek toplamlarını tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader FaaliyetProjeOdenekleriEbutce(int yil, string senaryoKod, int tur)
        {
            string alanlar = string.Empty;
            if (tur <= (int)ENUMFaaliyetProjeTur.MerkezTasra)
            {
                alanlar = TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD];
                alanlar += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKOD];
                alanlar += ", PEBFAALIYETPROJE.EBUTCEKOD";
            }
            if (!string.IsNullOrEmpty(alanlar))
                alanlar += ", ";
            alanlar += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            alanlar += ", " + VeriErisim.IslecBul("LEFT") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", 2)";

            string sql = "SELECT " + alanlar + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";
            sql += " FROM PEBFAALIYETPROJE, " + TABLOAD;

            sql += " WHERE PEBFAALIYETPROJE.YIL = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += " AND PEBFAALIYETPROJE.KURKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD];
            sql += " AND PEBFAALIYETPROJE.FPKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKOD];

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "=", (int)ENUMOdenekTur.FaaliyetProje));

            if (tur <= (int)ENUMFaaliyetProjeTur.MerkezTasra)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFAALIYETPROJE", "TUR", "<=", tur));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFAALIYETPROJE", "TUR", "=", tur));

            sql += " GROUP BY " + alanlar;
            sql += " ORDER BY " + alanlar;
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve
        /// kriterlere uyan sabit maliyetlerin ve ENUMFaaliyetProjeTur.DigerKurumIci türündeki faaliyet/projelerin
        /// birim ve ekonomik kodun ilk kırılımı bazında ödenek toplamlarını DataReader yapısında döndüren yordam
        /// Ödenekler e-butce sisteminin istediği yapıda yani ekonomik kodun ilk kırılımı bazında toplanıyor.
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="senaryoKod">Senaryo kriteri</param>
        /// <returns>Birim ve ekonomik kodun ilk kırılımı bazında ödenek toplamlarını tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader SabitMaliyetOdenekleriEbutce(int yil, string senaryoKod)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += ", " + VeriErisim.IslecBul("LEFT") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", 2)";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";
            sql += " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "=", (int)ENUMOdenekTur.SabitMaliyet));
            sql = VeriErisim.KosulEkle(sql, "OR (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "=", (int)ENUMOdenekTur.FaaliyetProje));

            string icSql = "SELECT 1 FROM PEBFAALIYETPROJE";
            icSql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = PEBFAALIYETPROJE.YIL";
            icSql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD] + " = PEBFAALIYETPROJE.KURKOD";
            icSql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALIYETKOD] + " = PEBFAALIYETPROJE.FPKOD";
            icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("PEBFAALIYETPROJE", "TUR", "=", (int)ENUMFaaliyetProjeTur.DigerKurumIci));

            sql = VeriErisim.KosulEkle(sql, "AND EXISTS (", icSql + ")))");

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += ", " + VeriErisim.IslecBul("LEFT") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", 2)";

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += ", " + VeriErisim.IslecBul("LEFT") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", 2)";

            return veriErisim.DataReaderDondur(sql);
        }

        //public IDataReader FarkliTertipDüzenListele(int yil, string ekoKod)
        //{
        //    string sql = "SELECT DISTINCT " + ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.FONKOD] + "," + ALANLAR[(int)ENUMAlanlar.FINKOD] + "," + ALANLAR[(int)ENUMAlanlar.EKOKOD] + "," + ALANLAR[(int)ENUMAlanlar.MALIYETFONKOD] + " FROM " + TABLOAD;

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", ekoKod.Replace(".", "")));

        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETFONKOD], "<>", "NULL"));

        //    sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD];
        //    sql += "," + ALANLAR[(int)ENUMAlanlar.FONKOD];
        //    sql += "," + ALANLAR[(int)ENUMAlanlar.EKOKOD];
        //    sql += "," + ALANLAR[(int)ENUMAlanlar.FINKOD];
        //    sql += "," + ALANLAR[(int)ENUMAlanlar.MALIYETFONKOD];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public bool FonKodKullanilmisMi(FaaliyetProjeOdenek fpOdenek)
        //{
        //    string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", fpOdenek.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", fpOdenek.senaryoKod.Replace(".", "")));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETFONKOD], "=", fpOdenek.fpFonKod.Replace(".", "")));
        //    sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD], "<>", fpOdenek.kurKod.Replace(".", "")));
        //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKOD], "<>", fpOdenek.fpKod));
        //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "<>", (int)ENUMOdenekTur.FaaliyetProje));
        //    sql += ")";

        //    return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        //}

        //public bool FonKodKullanilmisMi(SabitMaliyetOdenek smOdenek)
        //{
        //    string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", smOdenek.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", smOdenek.senaryoKod.Replace(".", "")));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETFONKOD], "=", smOdenek.smFonKod.Replace(".", "")));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD], "=", smOdenek.kurKod.Replace(".", "")));
        //    sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKOD], "<>", smOdenek.smKod));
        //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABITMI], "<>", (int)ENUMOdenekTur.SabitMaliyet));
        //    sql += ")";

        //    return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        //}

        //public double FaaliyetProjeToplami(int yil, string kurKod, int asama, int fpKod)
        //{
        //    return ToplamAl(yil, kurKod, asama, fpKod, (int)ENUMOdenekTur.FaaliyetProje);
        //}

        //public IDataReader TertipToplamlari(int yil, string kurKod, string senaryoKod)
        //{
        //    string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KURKOD];
        //    sql += ", " + ALANLAR[(int)ENUMAlanlar.FONKOD];
        //    sql += ", " + ALANLAR[(int)ENUMAlanlar.FINKOD];
        //    sql += ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD];
        //    sql += ", " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";
        //    sql += " FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
        //    if (!string.IsNullOrEmpty(kurKod))
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETKURKOD], "=", kurKod.Replace(".", "")));

        //    sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.KURKOD];
        //    sql += ", " + ALANLAR[(int)ENUMAlanlar.FONKOD];
        //    sql += ", " + ALANLAR[(int)ENUMAlanlar.FINKOD];
        //    sql += ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        public IDataReader TertipToplamlari(int yil, string senaryoKod, Tertip tertip)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.FONKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.FINKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(tertip.kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(tertip.kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(tertip.kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", tertip.kodlar[2].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(tertip.kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));

            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.FONKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.FINKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.EKOKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(int yil, string senaryoKod, Tertip tertip)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tertip.kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(tertip.kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", tertip.kodlar[1].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(tertip.kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", tertip.kodlar[2].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(tertip.kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", tertip.kodlar[3].Replace(".", "") + "%"));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += "," + ALANLAR[(int)ENUMAlanlar.FONKOD];
            sql += "," + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            sql += "," + ALANLAR[(int)ENUMAlanlar.FINKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        //public double TertipToplami(int yil, string senaryoKod, string[] kodlar)
        //{
        //    string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";
        //    sql += " FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

        //    if (!string.IsNullOrEmpty(kodlar[0]))
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kodlar[0].Replace(".", "") + "%"));
        //    if (!string.IsNullOrEmpty(kodlar[1]))
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", kodlar[1].Replace(".", "") + "%"));
        //    if (!string.IsNullOrEmpty(kodlar[2]))
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", kodlar[2].Replace(".", "") + "%"));
        //    if (!string.IsNullOrEmpty(kodlar[3]))
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", kodlar[3].Replace(".", "") + "%"));

        //    return veriErisim.TekKolonSQLDbl(sql);
        //}
    }
}
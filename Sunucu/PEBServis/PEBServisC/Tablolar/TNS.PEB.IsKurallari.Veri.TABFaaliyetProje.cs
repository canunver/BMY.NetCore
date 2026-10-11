using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Faaliyet/proje bilgilerini tutan PEBFAALIYETPROJE veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// Faaliyet/projelerin bütçe alt faaliyetleri ile bağlarını tutan PEBFAALIYETPROJEPEBFON
    /// veri tabanı tablosu ile ilgili sql işlemleri de bu sınıfta yapılır.
    /// </summary>
    public class TABFaaliyetProje : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFaaliyetProje(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Faaliyet/projenin tutulduğu tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBFAALIYETPROJE";

        /// <summary>
        /// Faaliyet/projenin tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// FPKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FPKOD = 2,

            /// <summary>
            /// FPAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FPAD = 3,

            /// <summary>
            /// PROJEMI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PROJEMI = 4,

            /// <summary>
            /// SORUMLU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SORUMLU = 5,

            /// <summary>
            /// FAALIYETSEVIYESI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FAALIYETSEVIYESI = 6,

            /// <summary>
            /// TUR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TUR = 7,

            /// <summary>
            /// EBUTCEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            EBUTCEKOD = 8
        }

        /// <summary>
        /// Faaliyet/projenin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "FPKOD", "FPAD", "PROJEMI", "SORUMLU", "FAALIYETSEVIYESI", "TUR", "EBUTCEKOD" };

        /// <summary>
        /// Faaliyet/projelerin bütçe alt faaliyetleri ile bağlarının tutulduğu tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADPEBFON = "PEBFAALIYETPROJEPEBFON";

        /// <summary>
        /// Faaliyet/projelerin bütçe alt faaliyetleri ile bağlarının tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarPEBFON : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// PEBKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 1,

            /// <summary>
            /// MALIYETKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MALIYETKURKOD = 2,

            /// <summary>
            /// SABITMI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SABITMI = 3,

            /// <summary>
            /// MALIYETKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MALIYETKOD = 4
        }

        /// <summary>
        /// Faaliyet/projelerin bütçe alt faaliyetleri ile bağlarının tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARPEBFON = { "YIL", "PEBKOD", "MALIYETKURKOD", "SABITMI", "MALIYETKOD" };

        /// <summary>
        /// Faaliyet/proje bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Faaliyet/projenin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="fp">Faaliyet/proje bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(FaaliyetProje fp)
        {
            fp.fpKod = FPKodAl();
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(fp.yil) + ",";
            sql += VeriErisim.DegerYap(fp.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(fp.fpKod) + ",";
            sql += VeriErisim.DegerYap(fp.fpAd) + ",";
            sql += VeriErisim.DegerYap(fp.projeMi) + ",";
            sql += VeriErisim.DegerYap(fp.sorumlu) + ",";
            sql += VeriErisim.DegerYap(fp.seviye) + ",";
            sql += VeriErisim.DegerYap(fp.tur) + ",";
            sql += VeriErisim.DegerYap(fp.ebutceKod) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = fp.fpKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan faaliyet/proje bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="fp">Faaliyet/proje bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(FaaliyetProje fp)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FPAD], fp.fpAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJEMI], fp.projeMi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLU], fp.sorumlu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETSEVIYESI], fp.seviye) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], fp.tur) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EBUTCEKOD], fp.ebutceKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", fp.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", fp.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FPKOD], "=", fp.fpKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen faaliyet/projeyi kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve faaliyet/projeleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="fp">Faaliyet/proje bilgilerini tutan nesne</param>
        /// <param name="perHedefKod">Performans hedefi programatik kodu</param>
        /// <returns>Verilen kriterlere uygun olan faaliyet/projeleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(FaaliyetProje fp, int perHedefKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string baglac = "WHERE";
            if (perHedefKod > 0)
            {
                sql += ", PEBFAALIYETPROJEAGIRLIK";
                sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "PEBFAALIYETPROJEAGIRLIK.YIL";
                sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + "=" + "PEBFAALIYETPROJEAGIRLIK.FPKURKOD";
                sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FPKOD] + "=" + "PEBFAALIYETPROJEAGIRLIK.FPKOD";
                baglac = "AND";
            }

            sql = VeriErisim.KosulEkle(sql, baglac, VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", fp.yil));
            if (!string.IsNullOrEmpty(fp.kurKod))
            {
                if (perHedefKod > 0)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", fp.kurKod.Replace(".", "")));
                    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("PEBFAALIYETPROJEAGIRLIK", "PHKURKOD", "=", fp.kurKod.Replace(".", "")));
                    sql += ")";
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", fp.kurKod.Replace(".", "")));
            }
            if (fp.fpKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKOD], "=", fp.fpKod));
            if (!string.IsNullOrEmpty(fp.fpAd))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FPAD], "LIKE", "%" + fp.fpAd + "%"));
            if (fp.projeMi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEMI], "=", fp.projeMi));
            if (!string.IsNullOrEmpty(fp.sorumlu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLU], "=", fp.sorumlu));
            if (!string.IsNullOrEmpty(fp.seviye))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FAALIYETSEVIYESI], "=", fp.seviye));
            if (fp.tur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", fp.tur));
            if (perHedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBFAALIYETPROJEAGIRLIK", "PERHEDEFKOD", "=", perHedefKod));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen faaliyet/projeyi kullanarak,
        /// ilgili faaliyet/projeyi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="fp">Faaliyet/proje bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(FaaliyetProje fp)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", fp.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", fp.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FPKOD], "=", fp.fpKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen yılı, birim kodunu ve faaliyet/proje programatik kodunu kullanarak,
        /// ilgili faaliyet/projenin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool FPVarMi(int yil, int fpKod)
        {
            string[] strAlan = null; //{ ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = null; //{ kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.FPKOD] };
            int[] intDeger = { yil, fpKod };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen yıl, birim kodu ve faaliyet/proje programatik kodunun ait olduğu
        /// faaliyet/projenin, sabit maliyetlerden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="fpKod">Faaliyet/projenin programatik kodu</param>
        /// <returns>Faaliyet/proje; sabit maliyetlerde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool FPKullanilmisMi(int yil, string kurKod, int fpKod)
        {
            string[] strAlan = { "FPKURKOD" };
            string[] strDeger = { kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.FPKOD] };
            int[] intDeger = { yil, fpKod };
            return TABGenel.KayitVarmi(veriErisim, "PEBSABITMALIYETAGIRLIK", strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen yılı ve birim kodunu baz alarak
        /// yeni faaliyet/proje programatik kodu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Yeni faaliyet/proje programatik kodu döndürülür.</returns>
        public int FPKodAl() //int yil, string kurKod
        {
            string[] strAlan = null; //{ ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = null; //{ kurKod.Replace(".", "") };
            string[] intAlan = null; //{ ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = null; //{ yil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.FPKOD], strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kullanarak, ilgili faaliyet/projenin
        /// veya sabit maliyetin bütçe alt faaliyetleri ile bağlarını veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="maliyetKurKod">Faaliyet/projenin veya sabit maliyetin birim kodu</param>
        /// <param name="sabitMi">Faaliyet/proje mi (0 değeri) sabit maliyet mi (1 değeri) bilgisi</param>
        /// <param name="maliyetKod">Faaliyet/proje veya sabit maliyet programatik kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PEBFonKodlariSil(int yil, string maliyetKurKod, int sabitMi, int maliyetKod)
        {
            string sql = "DELETE FROM " + TABLOADPEBFON;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPEBFON[(int)ENUMAlanlarPEBFON.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPEBFON[(int)ENUMAlanlarPEBFON.MALIYETKURKOD], "=", maliyetKurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPEBFON[(int)ENUMAlanlarPEBFON.SABITMI], "=", sabitMi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPEBFON[(int)ENUMAlanlarPEBFON.MALIYETKOD], "=", maliyetKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin bütçe alt faaliyeti ile bağını veri tabanına
        /// ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="maliyetKurKod">Faaliyet/projenin veya sabit maliyetin birim kodu</param>
        /// <param name="sabitMi">Faaliyet/proje mi (0 değeri) sabit maliyet mi (1 değeri) bilgisi</param>
        /// <param name="maliyetKod">Faaliyet/proje veya sabit maliyet programatik kodu</param>
        /// <param name="pebFonKod">Bütçe alt faaliyeti kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PEBFonKodKaydet(int yil, string maliyetKurKod, int sabitMi, int maliyetKod, string pebFonKod)
        {
            string sql = "INSERT INTO " + TABLOADPEBFON + "(" + VeriErisim.TumAlanlar("", ALANLARPEBFON) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(pebFonKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(maliyetKurKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(sabitMi) + ", ";
            sql += VeriErisim.DegerYap(maliyetKod) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve faaliyet/projeye veya
        /// sabit maliyete ait bütçe alt faaliyetlerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="maliyetKurKod">Faaliyet/projenin veya sabit maliyetin birim kodu</param>
        /// <param name="sabitMi">Faaliyet/proje mi (0 değeri) sabit maliyet mi (1 değeri) bilgisi</param>
        /// <param name="maliyetKod">Faaliyet/proje veya sabit maliyet programatik kodu</param>
        /// <returns>Verilen kriterlere uygun olan bütçe alt faaliyetlerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader ListelePenFonlar(int yil, string maliyetKurKod, int sabitMi, int maliyetKod)
        {
            string sql = "SELECT " + ALANLARPEBFON[(int)ENUMAlanlarPEBFON.PEBKOD] + " FROM " + TABLOADPEBFON;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADPEBFON, ALANLARPEBFON[(int)ENUMAlanlarPEBFON.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPEBFON, ALANLARPEBFON[(int)ENUMAlanlarPEBFON.MALIYETKURKOD], "=", maliyetKurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPEBFON, ALANLARPEBFON[(int)ENUMAlanlarPEBFON.SABITMI], "=", sabitMi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPEBFON, ALANLARPEBFON[(int)ENUMAlanlarPEBFON.MALIYETKOD], "=", maliyetKod));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader AyniNumaraliBul()
        {
            string sql = @"select DISTINCT D.KURKOD,D.FPKOD from PEBFAALIYETPROJE D
                             where 1 IN (
                             select 1 from PEBFAALIYETPROJE I
                             WHERE I.YIL=D.YIL AND I.FPKOD=D.FPKOD
                             group by I.YIL,I.FPKOD
                             HAVING COUNT(*)>1)
                             order by 1,2";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
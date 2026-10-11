using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Yatırım alt projesi bilgilerini tutan YATALTPROJE veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATAltProje : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATAltProje(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATALTPROJE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// ALTPROJENO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 0,

            /// <summary>
            /// BIRIM alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BIRIM = 1,

            /// <summary>
            /// PROJENO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PROJENO = 2,

            /// <summary>
            /// TURKODU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TURKODU = 3,

            /// <summary>
            /// AD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AD = 4,

            /// <summary>
            /// KARAKTERISTIK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KARAKTERISTIK = 5,

            /// <summary>
            /// KARAKTERISTIKOZET alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KARAKTERISTIKOZET = 6,

            /// <summary>
            /// SORUMLUADSOYAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SORUMLUADSOYAD = 7,

            /// <summary>
            /// SORUMLUGOREV alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SORUMLUGOREV = 8,

            /// <summary>
            /// SORUMLUTELEFON alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SORUMLUTELEFON = 9,

            /// <summary>
            /// SORUMLUEPOSTA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SORUMLUEPOSTA = 10,

            /// <summary>
            /// ONERENAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ONERENAD = 11,

            /// <summary>
            /// ONERENADRES alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ONERENADRES = 12,

            /// <summary>
            /// ONERENTELEFON alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ONERENTELEFON = 13,

            /// <summary>
            /// ONERENFAKS alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ONERENFAKS = 14,

            /// <summary>
            /// GEREKCE alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 15,

            /// <summary>
            /// AMAC alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AMAC = 16,

            /// <summary>
            /// HEDEF alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            HEDEF = 17,

            /// <summary>
            /// MALIYET alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MALIYET = 18,

            /// <summary>
            /// FINANSMANKAYNAK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            FINANSMANKAYNAK = 19,

            /// <summary>
            /// BASLANGICYILI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BASLANGICYILI = 20,

            /// <summary>
            /// BITISYILI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BITISYILI = 21,

            /// <summary>
            /// MODEL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MODEL = 22,

            /// <summary>
            /// IHALETARIHI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            IHALETARIHI = 23,

            /// <summary>
            /// IHALEBEDELI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            IHALEBEDELI = 24,

            /// <summary>
            /// IHALEYASASI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            IHALEYASASI = 25,

            DPTDEVAR = 26,

            YIL = 27,

            SENARYOKOD = 28
        }

        /// <summary>
        /// Yatırım alt projesinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "ALTPROJENO", "BIRIM", "PROJENO", "TURKODU", "AD", "KARAKTERISTIK", "KARAKTERISTIKOZET",
                                   "SORUMLUADSOYAD", "SORUMLUGOREV", "SORUMLUTELEFON", "SORUMLUEPOSTA",
                                   "ONERENAD", "ONERENADRES", "ONERENTELEFON", "ONERENFAKS", "GEREKCE", "AMAC", "HEDEF",
                                   "MALIYET", "FINANSMANKAYNAK", "BASLANGICYILI", "BITISYILI", "MODEL",
                                   "IHALETARIHI", "IHALEBEDELI", "IHALEYASASI", "DPTDEVAR", "YIL", "SENARYOKOD"
        };

        /// <summary>
        /// Yatırım alt projesi bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Yatırım alt projesinin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="altProje">Yatırım alt projesi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string senaryoKod, AltProje altProje)
        {
            //int kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTPROJENO]);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(altProje.kod) + ",";
            sql += VeriErisim.DegerYap(altProje.birim.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(altProje.proje.kod) + ",";
            sql += VeriErisim.DegerYap(altProje.tur.kod) + ",";
            sql += VeriErisim.DegerYap(altProje.ad) + ",";
            sql += VeriErisim.DegerYap(altProje.karakteristik) + ",";
            sql += VeriErisim.DegerYap(altProje.karakteristikOzet) + ",";
            sql += VeriErisim.DegerYap(altProje.sorumluAd) + ",";
            sql += VeriErisim.DegerYap(altProje.sorumluGorev) + ",";
            sql += VeriErisim.DegerYap(altProje.sorumluTelefon) + ",";
            sql += VeriErisim.DegerYap(altProje.sorumluEPosta) + ",";
            sql += VeriErisim.DegerYap(altProje.onerenAd) + ",";
            sql += VeriErisim.DegerYap(altProje.onerenAdres) + ",";
            sql += VeriErisim.DegerYap(altProje.onerenTelefon) + ",";
            sql += VeriErisim.DegerYap(altProje.onerenFaks) + ",";
            sql += VeriErisim.DegerYap(altProje.gerekce) + ",";
            sql += VeriErisim.DegerYap(altProje.amac) + ",";
            sql += VeriErisim.DegerYap(altProje.hedef) + ",";
            sql += VeriErisim.DegerYap(altProje.maliyet) + ",";
            sql += VeriErisim.DegerYap(altProje.finansmanKaynak) + ",";
            sql += VeriErisim.DegerYap(altProje.baslangicYil) + ",";
            sql += VeriErisim.DegerYap(altProje.bitisYil) + ",";
            sql += VeriErisim.DegerYap(altProje.model) + ",";
            sql += VeriErisim.DegerYap(altProje.ihaleTarihi) + ",";
            sql += VeriErisim.DegerYap(altProje.ihaleBedeli) + ",";
            sql += VeriErisim.DegerYap(altProje.ihaleYasasi) + ",";
            sql += VeriErisim.DegerYap(altProje.DPTDeVar ? 1 : 0) + ",";

            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            //sonuc.anahtar = kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan yatırım alt projesi bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="altProje">Yatırım alt projesi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(int yil, string senaryoKod, AltProje altProje)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], altProje.birim.kod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], altProje.proje.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURKODU], altProje.tur.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], altProje.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARAKTERISTIK], altProje.karakteristik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARAKTERISTIKOZET], altProje.karakteristikOzet) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUADSOYAD], altProje.sorumluAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUGOREV], altProje.sorumluGorev) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUTELEFON], altProje.sorumluTelefon) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUEPOSTA], altProje.sorumluEPosta) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONERENAD], altProje.onerenAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONERENADRES], altProje.onerenAdres) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONERENTELEFON], altProje.onerenTelefon) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONERENFAKS], altProje.onerenFaks) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], altProje.gerekce) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMAC], altProje.amac) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDEF], altProje.hedef) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALIYET], altProje.maliyet) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FINANSMANKAYNAK], altProje.finansmanKaynak) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICYILI], altProje.baslangicYil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISYILI], altProje.bitisYil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODEL], altProje.model) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALETARIHI], altProje.ihaleTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEBEDELI], altProje.ihaleBedeli) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEYASASI], altProje.ihaleYasasi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DPTDEVAR], altProje.DPTDeVar ? 1 : 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProje.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = altProje.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Yıl, yatırım projesi programatik kodu ve degerler dizisindeki kullanıcının yetkili olduğu birimleri
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve yatırım alt projelerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="projeKodu">Yatırım projesi programatik kodu</param>
        /// <param name="kurkodlar">Kullanıcının yetkili olduğu birim listesi</param>
        /// <param name="sektorKod">Kullanıcının yetkili olduğu birim listesi</param>
        /// <returns>Verilen kriterlere uygun olan yatırım alt projelerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string senaryoKod, int projeKodu, string[] kurkodlar, string sektorKod, int altProjeNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD + " INNER JOIN YATPROJE ON YATALTPROJE.PROJENO = YATPROJE.PROJENO AND YATALTPROJE.YIL = YATPROJE.YIL AND YATALTPROJE.SENARYOKOD = YATPROJE.SENARYOKOD";

            string kosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            if (altProjeNo == 0)
            {
                if (projeKodu != 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeKodu));

                if (!string.IsNullOrWhiteSpace(sektorKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("YATPROJE", "SEKTORNO", "LIKE", sektorKod.Replace(".", "") + "%"));
            }
            else
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeNo));
            if (kosul != "") sql += " WHERE " + kosul;

            string orluBirimler = TABGenel.DegerleriOrla(kurkodlar, "YATALTPROJE.BIRIM", altProjeNo);
            if (orluBirimler != "")
                sql = VeriErisim.KosulEkle(sql, "AND", orluBirimler);

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ALTPROJENO];

            return veriErisim.DataReaderDondur(sql);
        }

        internal static string Listele(string UstAlanAd, string senaryoUstAlanAd, string projeUstAlanAd, int butceYil, string senaryoKod, bool tekYil, bool eskiYillarKBO, int ekoUz)
        {
            string kaynakSorgu = TABYATProje.IcSorguOl(TABYATButceKaynak.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO], butceYil, senaryoKod, tekYil, eskiYillarKBO, ekoUz));
            //kaynakSorgu = "NULL";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", ABSKUR.KURAD, YATPROJETUR.TURADI";
            sql += "," + TABYATProje.IcSorguOl(TABYATALTPROJEBILGI.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], null, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO])) + " XML_BILGILER";
            sql += "," + TABYATProje.IcSorguOl(TABYATIlIlce.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], null, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO])) + " XML_ILCELER";
            sql += "," + TABYATProje.IcSorguOl(TABYATPROJEKALKINMABOLGE.Listele(TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD], null, TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO])) + " XML_BOLGELER";
            sql += "," + kaynakSorgu + " XML_KAYNAKLAR";
            sql += " FROM " + TABLOAD;
            sql += " LEFT JOIN ButceHazirlik.dbo.ABSKUR ABSKUR ON " + VeriErisim.Join(TABLOAD, "ABSKUR", new string[] { "YIL", "BIRIM" }, new string[] { "YIL", "KURKOD" });
            sql += " LEFT JOIN YATPROJETUR ON " + VeriErisim.Join(TABLOAD, "YATPROJETUR", new string[] { "TURKODU" }); ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + UstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJENO] + "=" + projeUstAlanAd);
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ALTPROJENO];
            return sql;
        }

        ///// <summary>
        ///// Parametre olarak verilen yatırım alt projesi nesnesini kıstas olarak kullanarak,
        ///// select sql cümlesini oluşturup, çalıştıran
        ///// ve yatırım alt projelerini DataReader yapısında döndüren yordam
        ///// </summary>
        ///// <param name="altProje">Yatırım alt projesi bilgilerini tutan nesne</param>
        ///// <returns>Verilen kriterlere uygun olan yatırım alt projelerini tutan DataReader nesnesi döndürülür.</returns>
        //public IDataReader Listele(int yil, string senaryoKod, AltProje altProje)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

        //    if (altProje.kod != 0)
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProje.kod));

        //    sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ALTPROJENO];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        /// <summary>
        /// Verilen yatırım alt projesi programatik kodunu kullanarak,
        /// ilgili yatırım alt projesini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="altProjeKod">Yatırım alt projesi programatik kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string senaryoKod, int altProjeKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen yatırım alt projesi programatik kodunu kullanarak
        /// yatırım alt projesinin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="altProjeKodu">Yatırım alt projesi programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool AltProjeVarMi(int yil, string senaryoKod, int altProjeKodu)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.ALTPROJENO], ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { altProjeKodu, yil };

            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SENARYOKOD] };
            string[] strDeger = { senaryoKod.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Verilen proje türün, yatırım alt projelerinden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="projeTurKodu">Proje tür kodu</param>
        /// <returns>Proje tür; yatırım alt projelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool ProjeTurKullanilmisMi(int projeTurKodu)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.TURKODU] };
            int[] intDeger = { projeTurKodu };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Verilen yatırım projesinin, yatırım alt projelerinden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="projeKod">Yatırım projesi programatik kodu</param>
        /// <returns>Yatırım projesi; yatırım alt projelerinde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool ProjeKullanilmisMi(int yil, string senaryoKod, int projeKod)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.PROJENO], ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { projeKod, yil };
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SENARYOKOD] };
            string[] strDeger = { senaryoKod.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Verilen yatırım alt projesinin, yatırım alt projesi detay tablolarında kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="altProjeKod">Yatırım alt projesi programatik kodu</param>
        /// <returns>Yatırım alt projesi; yatırım alt projesi detay tablolarında kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool AltProjeKullanilmisMi(int yil, string senaryoKod, int altProjeKod)
        {
            string[] intAlan = { "YIL", "ALTPROJENO" };
            int[] intDeger = { yil, altProjeKod };

            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.SENARYOKOD] };
            string[] strDeger = { senaryoKod.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, "YATBUTCEKAYNAK", strAlan, strDeger, intAlan, intDeger);
            //|| TABGenel.KayitVarmi(veriErisim, "YATDOKUMANLAR", strAlan, strDeger, intAlan, intDeger)
            //|| TABGenel.KayitVarmi(veriErisim, "YATKREDITALEP", strAlan, strDeger, intAlan, intDeger);
        }

        //public Sonuc BitisYiliGuncelle(int altProjeKodu, int bitisYili)
        //{
        //    string sql = "UPDATE " + TABLOAD + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISYILI], bitisYili);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeKodu));

        //    return KomutCalistir(sql, 1);
        //}

        public Sonuc GerekceGuncelle(int yil, string senaryoKod, AltProje altProje)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], altProje.gerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProje.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        public Sonuc AltProjeKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, string projeKodListe)
        {
            string sql = "INSERT INTO " + TABLOAD + "(YIL, SENARYOKOD, ALTPROJENO, BIRIM, PROJENO, TURKODU, AD, KARAKTERISTIK, KARAKTERISTIKOZET, SORUMLUADSOYAD, SORUMLUGOREV, SORUMLUTELEFON, SORUMLUEPOSTA, ONERENAD, ONERENADRES, ONERENTELEFON, ONERENFAKS, GEREKCE, AMAC, HEDEF, MALIYET, FINANSMANKAYNAK, BASLANGICYILI, BITISYILI, MODEL, IHALETARIHI, IHALEBEDELI, IHALEYASASI, DPTDEVAR) ";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, ALTPROJENO, BIRIM, PROJENO, TURKODU, AD, KARAKTERISTIK, KARAKTERISTIKOZET, SORUMLUADSOYAD, SORUMLUGOREV, SORUMLUTELEFON, SORUMLUEPOSTA, ONERENAD, ONERENADRES, ONERENTELEFON, ONERENFAKS, GEREKCE, AMAC, HEDEF, MALIYET, FINANSMANKAYNAK, BASLANGICYILI, BITISYILI, MODEL, IHALETARIHI, IHALEBEDELI, IHALEYASASI, DPTDEVAR FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql = VeriErisim.KosulEkle(sql, "AND", "(1 NOT in (SELECT 1 FROM " + TABLOAD + $" IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.ALTPROJENO ={TABLOAD}.ALTPROJENO))");

            if (projeKodListe != null)
                sql = VeriErisim.KosulEkle(sql, "AND", "(PROJENO in (" + projeKodListe + "))");
            return KomutCalistir(sql, 0);
        }

        public Sonuc Tasi(int yil, string senaryoKod, int apKod, int pKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET PROJENO = " + VeriErisim.DegerYap(pKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", apKod));
            return KomutCalistir(sql, 0);
        }

        public static string AltProjeProjeyeBagla(string projeKodListe, string tabloAd)
        {
            string sql = "SELECT 1 FROM " + TABLOAD + " WHERE " + TABLOAD + ".YIL = " + tabloAd + ".YIL AND " + TABLOAD + ".SENARYOKOD = " + tabloAd + ".SENARYOKOD AND " + TABLOAD + ".ALTPROJENO = " + tabloAd + ".ALTPROJENO AND " + TABLOAD + ".PROJENO IN (" + projeKodListe + ")";
            return sql;
        }
    }
}
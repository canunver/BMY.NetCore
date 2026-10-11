using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Performans göstergesi bilgilerini tutan PEBGOSTERGE veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABGosterge : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGosterge(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBGOSTERGE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// GOSTERGEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GOSTERGEKOD = 0,

            /// <summary>
            /// GOSTERGEAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GOSTERGEAD = 1,

            /// <summary>
            /// OLCUBIRIMKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 2,

            /// <summary>
            /// TUR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TUR = 3,

            /// <summary>
            /// KULLANIMSEKLI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ONDALIKSAYISI = 4,

            /// <summary>
            /// SEVIYE alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SEVIYE = 5,

            /// <summary>
            /// ILERLEMEYONU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ILERLEMEYONU = 6,

            /// <summary>
            /// EBUTCEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            EBUTCEKOD = 7,

            /// <summary>
            /// KURUMKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 8
        }

        /// <summary>
        /// Performans göstergesinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "GOSTERGEKOD", "GOSTERGEAD", "OLCUBIRIMKOD", "TUR", "KULLANIMSEKLI", "SEVIYE", "ILERLEMEYONU", "EBUTCEKOD", "KURUMKOD" };

        /// <summary>
        /// Performans göstergesi bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Performans göstergesinin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="gosterge">Performans göstergesi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(PerformansGosterge gosterge)
        {
            gosterge.gostergeKod = GostergeKodAl();
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(gosterge.gostergeKod) + ",";
            sql += VeriErisim.DegerYap(gosterge.gostergeAd) + ",";
            sql += VeriErisim.DegerYap(gosterge.olcuBirim.kod) + ",";
            sql += VeriErisim.DegerYap((int)gosterge.tur) + ",";
            sql += VeriErisim.DegerYap(gosterge.ondalikSayisi) + ",";
            sql += VeriErisim.DegerYap(gosterge.seviye) + ",";
            sql += VeriErisim.DegerYap((int)gosterge.ilerlemeYonu) + ",";
            sql += VeriErisim.DegerYap(gosterge.ebutceKod) + ",";
            sql += VeriErisim.DegerYap(gosterge.kurumKod.Replace(".", "")) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = gosterge.gostergeKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan performans göstergesi bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="gosterge">Performans göstergesi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(PerformansGosterge gosterge)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GOSTERGEAD], gosterge.gostergeAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD], gosterge.olcuBirim.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], (int)gosterge.tur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONDALIKSAYISI], gosterge.ondalikSayisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEVIYE], gosterge.seviye) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILERLEMEYONU], (int)gosterge.ilerlemeYonu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EBUTCEKOD], gosterge.ebutceKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], "=", gosterge.gostergeKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen performans göstergesini
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve performans göstergelerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="gosterge">Performans göstergesi bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan performans göstergelerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(PerformansGosterge gosterge)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", PEBOLCUBIRIM.OLCUBIRIMAD, PEBOLCUBIRIM.EBUTCEKOD FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN PEBOLCUBIRIM ON PEBOLCUBIRIM.OLCUBIRIMKOD = " + TABLOAD + ".OLCUBIRIMKOD ";

            string kosul = string.Empty;
            if (gosterge.gostergeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], "=", gosterge.gostergeKod));
            else
            {
                if (!string.IsNullOrEmpty(gosterge.gostergeAd))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOSTERGEAD], "LIKE", "%" + gosterge.gostergeAd + "%"));
                if (gosterge.olcuBirim.kod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD], "=", gosterge.olcuBirim.kod));
                if ((int)gosterge.tur > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)gosterge.tur));
                if (gosterge.seviye > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEVIYE], "=", gosterge.seviye));
                if ((int)gosterge.ilerlemeYonu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILERLEMEYONU], "=", (int)gosterge.ilerlemeYonu));

                if (!string.IsNullOrWhiteSpace(gosterge.kurumKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", gosterge.kurumKod.Replace(".", "")));
            }
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;
            if (gosterge.adaGoreSirala)
                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.GOSTERGEAD];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen performans göstergesini kullanarak,
        /// ilgili performans göstergesini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="gosterge">Performans göstergesi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(PerformansGosterge gosterge)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], "=", gosterge.gostergeKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen performans göstergesi programatik kodunu ve kullanım şeklini kullanarak,
        /// performans göstergesinin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="gostergeKod">Performans göstergesi programatik kodu</param>
        /// <param name="kullanimSekli">ENUMGostergeKullanimSekli listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool GostergeVarMi(int gostergeKod, int kullanimSekli)
        {
            return KayitVarmi(TABLOAD, gostergeKod, kullanimSekli);
        }

        /// <summary>
        /// Parametre olarak verilen performans göstergesi programatik kodunu kullanarak,
        /// performans göstergesinin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="gostergeKod">Performans göstergesi programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool GostergeVarMi(int gostergeKod)
        {
            return KayitVarmi(TABLOAD, gostergeKod);
        }

        /// <summary>
        /// Parametre olarak verilen performans göstergesinin, hedeflenen veya gerçekleşen
        /// değerlerden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="gostergeKod">Performans göstergesi programatik kodu</param>
        /// <returns>Performans göstergesi; hedeflenen veya gerçekleşen değerlerde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool GostergeKullanilmisMi(int gostergeKod)
        {
            if (KayitVarmi("PEBGOSTERGEGERCEK", gostergeKod)
             || KayitVarmi("PEBGOSTERGEHEDEF", gostergeKod)
             || KayitVarmi("StratejikPlan.dbo.STRHEDEFGOSTERGE", gostergeKod)
             || KayitVarmi("StratejikPlan.dbo.STREYLEMPLANI", gostergeKod))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen kriterlere uygun performans göstergesi var mı kontrolü yapan yordam
        /// </summary>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="gostergeKod">Performans göstergesi programatik kodu</param>
        /// <param name="kullanimSekli">ENUMGostergeKullanimSekli listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, int gostergeKod, int kullanimSekli)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], ALANLAR[(int)ENUMAlanlar.ONDALIKSAYISI] };
            int[] intDeger = { gostergeKod, kullanimSekli };
            return TABGenel.KayitVarmi(veriErisim, tablo, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen performans göstergesi var mı kontrolü yapan yordam
        /// </summary>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="gostergeKod">Performans göstergesi programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, int gostergeKod)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD] };
            int[] intDeger = { gostergeKod };
            return TABGenel.KayitVarmi(veriErisim, tablo, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Yeni performans göstergesi programatik kodu döndüren yordam
        /// </summary>
        /// <returns>Yeni performans göstergesi programatik kodu döndürülür.</returns>
        private int GostergeKodAl()
        {
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], new string[0], new string[0], new string[0], new int[0]);
        }
    }
}
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Performans hedef bilgilerini tutan PEBPERHEDEF veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABPerHedef : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPerHedef(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBPERHEDEF";

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
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// HEDEFKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            HEDEFKOD = 2,

            /// <summary>
            /// HEDEF alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            HEDEF = 3,

            /// <summary>
            /// ACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 4,

            /// <summary>
            /// EBUTCEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            EBUTCEKOD = 5,

            /// <summary>
            /// EBUTCEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            HEDEFGORKOD = 6,
            KPAMAC = 7,
            KPHEDEF = 8,
            KPPOLITIKA = 9
        }

        /// <summary>
        /// Performans hedeflerin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "HEDEFKOD", "HEDEF", "ACIKLAMA", "EBUTCEKOD", "HEDEFGORKOD", "KPAMAC", "KPHEDEF", "KPPOLITIKA" };

        /// <summary>
        /// Performans hedef bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Performans hedefin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="hedef">Performans hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(PerformansHedef hedef)
        {
            hedef.perHedefKod = HedefKodAl(hedef.yil, hedef.kurKod);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hedef.yil) + ",";
            sql += VeriErisim.DegerYap(hedef.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(hedef.perHedefKod) + ",";
            sql += VeriErisim.DegerYap(hedef.perHedef) + ",";
            sql += VeriErisim.DegerYap(hedef.aciklama) + ",";
            sql += VeriErisim.DegerYap(hedef.ebutceKod) + ",";
            sql += VeriErisim.DegerYap(hedef.perHedefGorKod) + ",";

            sql += VeriErisim.DegerYap(hedef.kalkinmaAmac) + ",";
            sql += VeriErisim.DegerYap(hedef.kalkinmaHedef) + ",";
            sql += VeriErisim.DegerYap(hedef.kalkinmaPolitika) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = hedef.perHedefKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan performans hedef bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="hedef">Performans hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(PerformansHedef hedef)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDEF], hedef.perHedef) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], hedef.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EBUTCEKOD], hedef.ebutceKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDEFGORKOD], hedef.perHedefGorKod) + ", ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KPAMAC], hedef.kalkinmaAmac) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KPHEDEF], hedef.kalkinmaHedef) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KPPOLITIKA], hedef.kalkinmaPolitika);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hedef.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", hedef.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", hedef.perHedefKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen performans hedefi kullanarak,
        /// ilgili performans hedefi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="pHedef">Performans hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(PerformansHedef pHedef)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", pHedef.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", pHedef.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", pHedef.perHedefKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve performans hedefleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="hedef">Performans hedef bilgilerini tutan nesne</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedefin programatik kodu</param>
        /// <param name="kurumDahil">Kurum performans hedefleri dahil edilsin mi kriteri</param>
        /// <returns>Verilen kriterlere uygun olan performans hedefleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(PerformansHedef hedef, int donemBYil, int strHedefKod, bool kurumDahil, string kurYapi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (donemBYil > 0 || strHedefKod > 0)
            {
                sql += " LEFT OUTER JOIN PEBPERHEDEFAGIRLIK ";
                sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "PEBPERHEDEFAGIRLIK.YIL";
                sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + "=" + "PEBPERHEDEFAGIRLIK.KURKOD";
                sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HEDEFKOD] + "=" + "PEBPERHEDEFAGIRLIK.PERHEDEFKOD";
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", hedef.yil));

            if (!string.IsNullOrEmpty(hedef.kurKod))
            {
                string noktasizKurKod = hedef.kurKod.Replace(".", "");
                if (kurumDahil && noktasizKurKod.Length == 8)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", noktasizKurKod));
                    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", TNS.BKD.Arac.KurumsaldanKuruma(hedef.kurKod, kurYapi).Replace(".", "")));
                    sql += ")";
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", noktasizKurKod + "%"));
            }

            if (hedef.perHedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", hedef.perHedefKod));

            if (donemBYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "DONEMBYIL", "=", donemBYil));

            if (strHedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PEBPERHEDEFAGIRLIK", "STRHEDEFKOD", "=", strHedefKod));

            sql += " ORDER BY " + TABLOAD + ".KURKOD, HEDEFGORKOD";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen yılı, birim kodunu ve performans hedef programatik kodunu kullanarak,
        /// ilgili performans hedefin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="hedefKod">Performans hedefin programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool HedefVarMi(int yil, string kurKod, int hedefKod)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.HEDEFKOD] };
            int[] intDeger = { yil, hedefKod };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen yıl, birim kodu ve performans hedef programatik kodunun ait olduğu
        /// performans hedefin, faaliyet/projelerden, sabit maliyetlerden veya performans göstergesi hedeflenen
        /// değerlerinden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="hedefKod">Performans hedefin programatik kodu</param>
        /// <returns>Performans hedef; kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool HedefKullanilmisMi(int yil, string kurKod, int hedefKod)
        {
            if (KayitVarmi("PEBFAALIYETPROJEAGIRLIK", yil, kurKod, hedefKod)
             || KayitVarmi("PEBGOSTERGEHEDEF", yil, kurKod, hedefKod)
             || KayitVarmi("PEBSABITMALIYET", yil, kurKod, hedefKod))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen kriterlere uygun performans hedefi var mı kontrolü yapan yordam
        /// </summary>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="hedefKod">Performans hedefin programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, int yil, string kurKod, int hedefKod)
        {
            string[] strAlan = { "PHKURKOD" };
            string[] strDeger = { kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], "PERHEDEFKOD" };
            int[] intDeger = { yil, hedefKod };
            return TABGenel.KayitVarmi(veriErisim, tablo, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Parametre olarak verilen yılı ve birim kodunu baz alarak
        /// yeni performans hedef programatik kodu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Yeni performans hedef programatik kodu döndürülür.</returns>
        private int HedefKodAl(int yil, string kurKod)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURKOD] };
            string[] strDeger = { kurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { yil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], strAlan, strDeger, intAlan, intDeger);
        }
    }
}
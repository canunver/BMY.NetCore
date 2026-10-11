using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Yatırım alt projesinin kredi talep bilgilerini tutan YATKREDITALEP veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATKrediTalep : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATKrediTalep(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATKREDITALEP";

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
            /// KREDINO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KREDINO = 1,

            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 2,

            /// <summary>
            /// AMAC alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AMAC = 3,

            /// <summary>
            /// KREDIIHTIYACI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KREDIIHTIYACI = 4,

            /// <summary>
            /// TEMINEDILEN alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TEMINEDILEN = 5,

            /// <summary>
            /// TALEPEDILEN alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TALEPEDILEN = 6,

            /// <summary>
            /// ALTERNATIF alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTERNATIF = 7,

            /// <summary>
            /// DUSUNCELER alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DUSUNCELER = 8,

            /// <summary>
            /// TALEPAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TALEPAD = 9
        }

        /// <summary>
        /// Kredi taleplerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "ALTPROJENO", "KREDINO", "YIL", "AMAC", "KREDIIHTIYACI", "TEMINEDILEN",
                                   "TALEPEDILEN", "ALTERNATIF", "DUSUNCELER", "TALEPAD" };

        /// <summary>
        /// Kredi talep bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Kredi talebin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="krediTalep">Kredi talep bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(KrediTalep krediTalep)
        {
            int kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KREDINO]);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(krediTalep.altProjeKodu) + ",";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(krediTalep.yil) + ",";
            sql += VeriErisim.DegerYap(krediTalep.amac) + ",";
            sql += VeriErisim.DegerYap(krediTalep.krediIhtiyaci) + ",";
            sql += VeriErisim.DegerYap(krediTalep.teminEdilen) + ",";
            sql += VeriErisim.DegerYap(krediTalep.talepEdilen) + ",";
            sql += VeriErisim.DegerYap(krediTalep.alternatif) + ",";
            sql += VeriErisim.DegerYap(krediTalep.dusunceler) + ",";
            sql += VeriErisim.DegerYap(krediTalep.talepAd) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan kredi talep bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="krediTalep">Kredi talep bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(KrediTalep krediTalep)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMAC], krediTalep.amac) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KREDIIHTIYACI], krediTalep.krediIhtiyaci) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMINEDILEN], krediTalep.teminEdilen) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TALEPEDILEN], krediTalep.talepEdilen) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTERNATIF], krediTalep.alternatif) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DUSUNCELER], krediTalep.dusunceler) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TALEPAD], krediTalep.talepAd);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KREDINO], "=", krediTalep.krediNo));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = krediTalep.krediNo.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen değişkenleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve kredi taleplerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="altProjeKodu">Yatırım alt projesi programatik kodu</param>
        /// <param name="krediNo">Kredi talebin programatik kodu</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Verilen kriterlere uygun olan kredi taleplerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int altProjeKodu, int krediNo, int yil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (altProjeKodu > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeKodu);
            if (krediNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KREDINO], "=", krediNo));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KREDINO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen kredi talep programatik kodunu kullanarak
        /// kredi talebin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="krediNo">Kredi talebin programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool KrediTalepVarMi(int krediNo)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.KREDINO] };
            int[] intDeger ={ krediNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Verilen kredi talep programatik kodunu kullanarak kredi talebinin ait olduğu
        /// yatırım alt projesini bulan ve yatırım alt projesinin programatik kodunu döndüren yordam
        /// </summary>
        /// <param name="krediTalepNo">Kredi talebin programatik kodu</param>
        /// <returns>Yatırım alt projesinin programatik kodu döndürülür.</returns>
        public int AltProjeKoduBul(int krediTalepNo)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KREDINO], "=", krediTalepNo));

            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
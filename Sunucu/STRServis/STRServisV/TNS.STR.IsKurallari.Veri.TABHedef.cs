using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.STR.IsKurallari.Veri
{
    /// <summary>
    /// Stratejik hedef bilgilerini tutan STRHEDEF veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABHedef : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHedef(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRHEDEF";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// DONEMBYIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 0,

            /// <summary>
            /// HEDEFKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            HEDEFKOD = 1,

            /// <summary>
            /// AMACKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AMACKOD = 2,

            /// <summary>
            /// HEDEFKISA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            HEDEFKISA = 3
        }

        /// <summary>
        /// Stratejik hedefin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL", "HEDEFKOD", "AMACKOD", "HEDEFKISA" };

        /// <summary>
        /// Stratejik hedef bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Stratejik hedefin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(StratejikHedef hedef)
        {
            hedef.strHedefKod = HedefKodAl(hedef.donemBYil);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hedef.donemBYil) + ",";
            sql += VeriErisim.DegerYap(hedef.strHedefKod) + ",";
            sql += VeriErisim.DegerYap(hedef.strAmac.strAmacKod) + ",";
            sql += VeriErisim.DegerYap(hedef.strHedefKisa) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = hedef.strHedefKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan stratejik hedef bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(StratejikHedef hedef)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMACKOD], hedef.strAmac.strAmacKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKISA], hedef.strHedefKisa);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", hedef.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", hedef.strHedefKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen stratejik hedef nesnesini kullanarak,
        /// ilgili stratejik hedefi veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(StratejikHedef hedef)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", hedef.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", hedef.strHedefKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen stratejik hedef nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve stratejik hedefleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan stratejik hedefleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(StratejikHedef hedef)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABYazi.TABLOAD, TABYazi.ALANLAR) + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMACKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HEDEFKISA];
            sql += " FROM " + TABLOAD + ", " + TABYazi.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DONEMBYIL] + "=" + TABYazi.TABLOAD + "." + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HEDEFKOD] + "=" + TABYazi.TABLOAD + "." + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD];

            string kosul = string.Empty;
            if (hedef.donemBYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL], "=", hedef.donemBYil));
            if (hedef.dil.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DILKOD], "=", hedef.dil.kod));
            if (hedef.strHedefKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD], "=", hedef.strHedefKod));
            if (hedef.strAmac.strAmacKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMACKOD], "=", hedef.strAmac.strAmacKod));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KODTUR], "=", (int)ENUMStratejikPlan.StratejikHedef));

            if (!string.IsNullOrEmpty(kosul))
                sql += " AND " + kosul;

            //sql += " ORDER BY " + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KISAYAZI];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen dönem başlangıç yılını kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve stratejik hedef özet/kısa bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Verilen kriterlere uygun olan stratejik hedef özet/kısa bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader HedefKisaListele(int donemBYil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (donemBYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", donemBYil));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılını ve stratejik hedef programatik kodunu kullanarak
        /// stratejik hedefin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="hedefKod">Stratejik hedef programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool HedefVarMi(int donemBYil, int hedefKod)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.DONEMBYIL], ALANLAR[(int)ENUMAlanlar.HEDEFKOD] };
            int[] intDeger ={ donemBYil, hedefKod };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılı bazında yeni stratejik hedef programatik kodu döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Yeni stratejik hedef programatik kodu döndürülür.</returns>
        private int HedefKodAl(int donemBYil)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.DONEMBYIL] };
            int[] intDeger ={ donemBYil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], new string[0], new string[0], intAlan, intDeger);
        }
    }
}
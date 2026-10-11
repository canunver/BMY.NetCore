using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.STR.IsKurallari.Veri
{
    /// <summary>
    /// Tema bilgilerini tutan STRTEMA veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABTema : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTema(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRTEMA";

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
            /// TEMAKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TEMAKOD = 1,

            /// <summary>
            /// TEMAKISA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TEMAKISA = 2
        }

        /// <summary>
        /// Temanın tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL", "TEMAKOD", "TEMAKISA" };

        /// <summary>
        /// Tema bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Temanın programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Tema tema)
        {
            tema.temaKod = TemaKodAl(tema.donemBYil);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tema.donemBYil) + ",";
            sql += VeriErisim.DegerYap(tema.temaKod) + ",";
            sql += VeriErisim.DegerYap(tema.temaKisa) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = tema.temaKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan tema bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(Tema tema)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMAKISA], tema.temaKisa);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", tema.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEMAKOD], "=", tema.temaKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen tema nesnesini kullanarak, ilgili temayı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Tema tema)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", tema.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEMAKOD], "=", tema.temaKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen tema nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve temaları DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan temaları tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Tema tema)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABYazi.TABLOAD, TABYazi.ALANLAR) + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEMAKISA];
            sql += " FROM " + TABLOAD + ", " + TABYazi.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DONEMBYIL] + "=" + TABYazi.TABLOAD + "." + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEMAKOD] + "=" + TABYazi.TABLOAD + "." + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD];

            string kosul = string.Empty;
            if (tema.donemBYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL], "=", tema.donemBYil));
            if (tema.dil.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DILKOD], "=", tema.dil.kod));
            if (tema.temaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD], "=", tema.temaKod));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KODTUR], "=", (int)ENUMStratejikPlan.Tema));

            if (!string.IsNullOrEmpty(kosul))
                sql += " AND " + kosul;

            //sql += " ORDER BY " + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KISAYAZI];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen dönem başlangıç yılını kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve tema özet/kısa bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Verilen kriterlere uygun olan tema özet/kısa bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader TemaKisaListele(int donemBYil)
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
        /// Verilen dönem başlangıç yılını ve tema programatik kodunu kullanarak
        /// temanın veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="temaKod">Tema programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool TemaVarMi(int donemBYil, int temaKod)
        {
            return KayitVarmi(TABLOAD, donemBYil, temaKod);
        }

        /// <summary>
        /// Verilen temanın, stratejik amaçlardan herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="temaKod">Tema programatik kodu</param>
        /// <returns>Tema; stratejik amaçlarda kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool TemaKullanilmisMi(int donemBYil, int temaKod)
        {
            return KayitVarmi(TABAmac.TABLOAD, donemBYil, temaKod);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılını ve tema programatik kodunu kullanarak
        /// temanın verilen veri tabanı tablosunda kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="tablo">Veri tabanı tablo adı</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="temaKod">Tema programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, int donemBYil, int temaKod)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL], ALANLAR[(int)ENUMAlanlar.TEMAKOD] };
            int[] intDeger = { donemBYil, temaKod };
            return TABGenel.KayitVarmi(veriErisim, tablo, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılı bazında yeni tema programatik kodu döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Yeni tema programatik kodu döndürülür.</returns>
        private int TemaKodAl(int donemBYil)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL] };
            int[] intDeger = { donemBYil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.TEMAKOD], new string[0], new string[0], intAlan, intDeger);
        }
    }
}
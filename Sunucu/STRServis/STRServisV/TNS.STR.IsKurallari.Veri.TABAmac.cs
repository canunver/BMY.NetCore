using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.STR.IsKurallari.Veri
{
    /// <summary>
    /// Stratejik amaç bilgilerini tutan STRAMAC veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABAmac : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAmac(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRAMAC";

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
            /// AMACKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AMACKOD = 1,

            /// <summary>
            /// TEMAKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TEMAKOD = 2,

            /// <summary>
            /// AMACKISA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AMACKISA = 3
        }

        /// <summary>
        /// Stratejik amacın tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL", "AMACKOD", "TEMAKOD", "AMACKISA" };

        /// <summary>
        /// Stratejik amaç bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Stratejik amacın programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(StratejikAmac amac)
        {
            amac.strAmacKod = AmacKodAl(amac.donemBYil);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(amac.donemBYil) + ",";
            sql += VeriErisim.DegerYap(amac.strAmacKod) + ",";
            sql += VeriErisim.DegerYap(amac.tema.temaKod) + ",";
            sql += VeriErisim.DegerYap(amac.strAmacKisa) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = amac.strAmacKod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan stratejik amaç bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(StratejikAmac amac)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMAKOD], amac.tema.temaKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMACKISA], amac.strAmacKisa);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", amac.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMACKOD], "=", amac.strAmacKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen stratejik amaç nesnesini kullanarak,
        /// ilgili stratejik amacı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(StratejikAmac amac)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", amac.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMACKOD], "=", amac.strAmacKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen stratejik amaç nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve stratejik amaçları DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan stratejik amaçları tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(StratejikAmac amac)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABYazi.TABLOAD, TABYazi.ALANLAR) + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEMAKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMACKISA];
            sql += " FROM " + TABLOAD + ", " + TABYazi.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DONEMBYIL] + "=" + TABYazi.TABLOAD + "." + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMACKOD] + "=" + TABYazi.TABLOAD + "." + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD];

            string kosul = string.Empty;
            if (amac.donemBYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL], "=", amac.donemBYil));
            if (amac.dil.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DILKOD], "=", amac.dil.kod));
            if (amac.strAmacKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD], "=", amac.strAmacKod));
            if (amac.tema.temaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEMAKOD], "=", amac.tema.temaKod));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KODTUR], "=", (int)ENUMStratejikPlan.StratejikAmac));

            if (!string.IsNullOrEmpty(kosul))
                sql += " AND " + kosul;

            //sql += " ORDER BY " + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KISAYAZI];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen dönem başlangıç yılını kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve stratejik amaç özet/kısa bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Verilen kriterlere uygun olan stratejik amaç özet/kısa bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader AmacKisaListele(int donemBYil)
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
        /// Verilen dönem başlangıç yılını ve stratejik amaç programatik kodunu kullanarak
        /// stratejik amacın veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="amacKod">Stratejik amaç programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool AmacVarMi(int donemBYil, int amacKod)
        {
            return KayitVarmi(TABLOAD, donemBYil, amacKod);
        }

        /// <summary>
        /// Verilen stratejik amacın, stratejik hedeflerden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="amacKod">Stratejik amaç programatik kodu</param>
        /// <returns>Stratejik amaç; stratejik hedeflerde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool AmacKullanilmisMi(int donemBYil, int amacKod)
        {
            return KayitVarmi(TABHedef.TABLOAD, donemBYil, amacKod);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılını ve stratejik amaç programatik kodunu kullanarak
        /// stratejik amacın verilen veri tabanı tablosunda kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="tablo">Veri tabanı tablo adı</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="amacKod">Stratejik amaç programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, int donemBYil, int amacKod)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL], ALANLAR[(int)ENUMAlanlar.AMACKOD] };
            int[] intDeger = { donemBYil, amacKod };
            return TABGenel.KayitVarmi(veriErisim, tablo, new string[0], new string[0], intAlan, intDeger);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılı bazında yeni stratejik amaç programatik kodu döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Yeni stratejik amaç programatik kodu döndürülür.</returns>
        private int AmacKodAl(int donemBYil)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.DONEMBYIL] };
            int[] intDeger ={ donemBYil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.AMACKOD], new string[0], new string[0], intAlan, intDeger);
        }
    }
}
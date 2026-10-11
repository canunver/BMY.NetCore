using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
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
            AMACKISA = 3,

            /// <summary>
            /// KURUMKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 4
        }

        /// <summary>
        /// Stratejik amacın tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL", "AMACKOD", "TEMAKOD", "AMACKISA", "KURUMKOD" };

        /// <summary>
        /// Stratejik amaç bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Stratejik amacın programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="amac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(StratejikAmac amac)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(amac.donemBYil) + ",";
            sql += VeriErisim.DegerYap(amac.strAmacKod) + ",";
            sql += VeriErisim.DegerYap(amac.tema.temaKod) + ",";
            sql += VeriErisim.DegerYap(amac.strAmacKisa) + ",";
            sql += VeriErisim.DegerYap(amac.kurumKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
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
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", amac.kurumKod.Replace(".", "")));
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
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", amac.kurumKod.Replace(".", "")));
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
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMKOD] + "=" + TABYazi.TABLOAD + "." + TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KURUMKOD];

            string kosul = string.Empty;
            if (amac.donemBYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL], "=", amac.donemBYil));
            if (amac.dil.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DILKOD], "=", amac.dil.kod));
            if (amac.strAmacKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD], "=", amac.strAmacKod));
            if (amac.tema.temaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEMAKOD], "=", amac.tema.temaKod));
            if (!string.IsNullOrWhiteSpace(amac.kurumKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", amac.kurumKod.Replace(".", "")));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABYazi.TABLOAD, TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KODTUR], "=", (int)ENUMStratejikPlan.StratejikAmac));

            if (!string.IsNullOrEmpty(kosul))
                sql += " AND " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMACKISA];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen dönem başlangıç yılını kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve stratejik amaç özet/kısa bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Verilen kriterlere uygun olan stratejik amaç özet/kısa bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader AmacKisaListele(string kurumKod, int donemBYil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurumKod.Replace(".", "")));
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
        public bool AmacVarMi(string kurumKod, int donemBYil, int amacKod)
        {
            return KayitVarmi(TABLOAD, kurumKod, donemBYil, amacKod);
        }

        /// <summary>
        /// Verilen stratejik amacın, stratejik hedeflerden herhangi birinde kullanılıp kullanılmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="amacKod">Stratejik amaç programatik kodu</param>
        /// <returns>Stratejik amaç; stratejik hedeflerde kullanılmışsa true, kullanılmamışsa false döndürülür.</returns>
        public bool AmacKullanilmisMi(string kurumKod, int donemBYil, int amacKod)
        {
            return KayitVarmi(TABHedef.TABLOAD, kurumKod, donemBYil, amacKod);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılını ve stratejik amaç programatik kodunu kullanarak
        /// stratejik amacın verilen veri tabanı tablosunda kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="tablo">Veri tabanı tablo adı</param>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="amacKod">Stratejik amaç programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, string kurumKod, int donemBYil, int amacKod)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURUMKOD] };
            string[] strDeger = { kurumKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL], ALANLAR[(int)ENUMAlanlar.AMACKOD] };
            int[] intDeger = { donemBYil, amacKod };
            return TABGenel.KayitVarmi(veriErisim, tablo, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılı bazında yeni stratejik amaç programatik kodu döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Yeni stratejik amaç programatik kodu döndürülür.</returns>
        public int AmacKodAl(string kurumKod, int donemBYil)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURUMKOD] };
            string[] strDeger = { kurumKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL] };
            int[] intDeger = { donemBYil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.AMACKOD], strAlan, strDeger, intAlan, intDeger);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = $"INSERT INTO {TABLOAD}(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += $" SELECT {hedefYil} DONEMBYIL, " + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + " FROM " + TABLOAD;
            sql += $" WHERE DONEMBYIL={kaynakYil} AND 1 NOT IN (SELECT 1 FROM {TABLOAD} IC WHERE IC.DONEMBYIL={hedefYil} AND IC.AMACKOD = {TABLOAD}.AMACKOD)";

            return KomutCalistir(sql, 0);
        }
    }
}
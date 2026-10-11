using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
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
            HEDEFKISA = 3,

            /// <summary>
            /// KURUMKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 4

        }

        /// <summary>
        /// Stratejik hedefin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL", "HEDEFKOD", "AMACKOD", "HEDEFKISA", "KURUMKOD" };

        /// <summary>
        /// Stratejik hedef bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// Stratejik hedefin programatik kodu burada üretilir ve dönen yapının anahtar özelliğine eklenir.
        /// </summary>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(StratejikHedef hedef)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hedef.donemBYil) + ",";
            sql += VeriErisim.DegerYap(hedef.strHedefKod) + ",";
            sql += VeriErisim.DegerYap(hedef.strAmac.strAmacKod) + ",";
            sql += VeriErisim.DegerYap(hedef.strHedefKisa) + ",";
            sql += VeriErisim.DegerYap(hedef.kurumKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
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
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", hedef.kurumKod.Replace(".", "")));
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
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", hedef.kurumKod.Replace(".", "")));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen stratejik hedef nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve stratejik hedefleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan stratejik hedefleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(StratejikHedef hedef, bool hedefler)
        {
            if (hedef.dil.kod == 0) hedef.dil.kod = 1;
            string sql = "SELECT A.DONEMBYIL, A.KODTUR, H.HEDEFKOD, A.DILKOD, H.KISAYAZI, H.UZUNYAZI, H.DOSYA, H.EBUTCEKOD, A.KURUMKOD, A.AMACKOD, H.HEDEFKISA, A.DILAD, A.AMACKISA, A.KISAYAZI, A.EBUTCEKOD, A.ARALIKBASYIL, A.ARALIKSONYIL ";
            sql += " FROM (SELECT STRAMAC.KURUMKOD, STRAMAC.DONEMBYIL, STRAMAC.AMACKOD, STRAMAC.AMACKISA, STRYAZI.KODTUR, STRYAZI.KOD, STRYAZI.DILKOD, STRYAZI.KISAYAZI, STRYAZI.UZUNYAZI, STRYAZI.DOSYA, STRYAZI.EBUTCEKOD, STRDIL.DILAD, STRMISYON.ARALIKBASYIL, STRMISYON.ARALIKSONYIL FROM STRAMAC ";
            sql += " INNER JOIN STRYAZI ON STRAMAC.DONEMBYIL=STRYAZI.DONEMBYIL AND STRYAZI.KODTUR=4 AND STRAMAC.AMACKOD=STRYAZI.KOD AND STRYAZI.KURUMKOD=STRAMAC.KURUMKOD AND STRYAZI.DILKOD=" + hedef.dil.kod + " ";
            sql += " INNER JOIN STRMISYON ON STRMISYON.DONEMBYIL=STRAMAC.DONEMBYIL";
            sql += " INNER JOIN STRDIL ON STRDIL.DILKOD=STRYAZI.DILKOD) A";
            if(hedefler)
                sql += " INNER JOIN ";
            else
                sql += " LEFT OUTER JOIN ";
            sql += " (SELECT STRHEDEF.KURUMKOD, STRHEDEF.DONEMBYIL, STRHEDEF.HEDEFKOD, STRHEDEF.AMACKOD, STRHEDEF.HEDEFKISA, STRYAZI.KODTUR, STRYAZI.KOD, STRYAZI.DILKOD, STRYAZI.KISAYAZI, STRYAZI.UZUNYAZI, STRYAZI.DOSYA, STRYAZI.EBUTCEKOD, STRDIL.DILAD FROM STRHEDEF ";
            sql += " INNER JOIN STRYAZI ON STRHEDEF.DONEMBYIL=STRYAZI.DONEMBYIL AND STRYAZI.KODTUR=5 AND STRHEDEF.HEDEFKOD=STRYAZI.KOD AND STRYAZI.KURUMKOD=STRHEDEF.KURUMKOD AND STRYAZI.DILKOD=" + hedef.dil.kod + " ";
            sql += " INNER JOIN STRDIL ON STRDIL.DILKOD=STRYAZI.DILKOD) H ";
            sql += " ON H.DONEMBYIL= A.DONEMBYIL AND H.AMACKOD=A.AMACKOD AND H.KURUMKOD=A.KURUMKOD";

            string kosul = string.Empty;
            if (hedef.donemBYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", "DONEMBYIL", "=", hedef.donemBYil));
            if (hedef.strHedefKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("H", "HEDEFKOD", "=", hedef.strHedefKod));
            if (hedef.strAmac.strAmacKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", "AMACKOD", "=", hedef.strAmac.strAmacKod));
            if (!string.IsNullOrWhiteSpace(hedef.kurumKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", "KURUMKOD", "=", hedef.kurumKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY A.AMACKISA" + ", H.HEDEFKISA";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen dönem başlangıç yılını kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve stratejik hedef özet/kısa bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Verilen kriterlere uygun olan stratejik hedef özet/kısa bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader HedefKisaListele(string kurumKod, int donemBYil)
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
        /// Verilen dönem başlangıç yılını ve stratejik hedef programatik kodunu kullanarak
        /// stratejik hedefin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <param name="hedefKod">Stratejik hedef programatik kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool HedefVarMi(string kurumKod, int donemBYil, int hedefKod)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURUMKOD] };
            string[] strDeger = { kurumKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL], ALANLAR[(int)ENUMAlanlar.HEDEFKOD] };
            int[] intDeger = { donemBYil, hedefKod };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılı bazında yeni stratejik hedef programatik kodu döndüren yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Yeni stratejik hedef programatik kodu döndürülür.</returns>
        public int HedefKodAl(string kurumKod, int donemBYil)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURUMKOD] };
            string[] strDeger = { kurumKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL] };
            int[] intDeger = { donemBYil };
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], strAlan, strDeger, intAlan, intDeger);
        }

        public IDataReader HedefMaliyet(StratejikHedef strHedef)
        {
            string alanlar = "A.AMACKISA, A.AMACKISA + ' - ' + YA.KISAYAZI, H.HEDEFKISA, H.HEDEFKISA + ' - ' + Y.KISAYAZI";
            string sql = "SELECT " + alanlar + ", SUM(ODENEK0), SUM(ODENEK1), SUM(ODENEK2), SUM(ODENEK3), SUM(ODENEK4) ";
            sql += "FROM STRHedef H ";
            sql += "INNER JOIN STRAMAC A ON A.AMACKOD=H.AMACKOD AND A.DONEMBYIL=H.DONEMBYIL ";
            sql += "INNER JOIN STRYAZI Y ON Y.DONEMBYIL = H.DONEMBYIL AND Y.KOD=H.HEDEFKOD and Y.KODTUR=5 ";
            sql += "INNER JOIN STRYAZI YA ON YA.DONEMBYIL = A.DONEMBYIL AND YA.KOD=A.AMACKOD and YA.KODTUR=4 ";
            sql += "LEFT OUTER JOIN STRHEDEFODENEK O ON O.DONEMBYIL=H.DONEMBYIL AND O.HEDEFKOD=H.HEDEFKOD AND O.KURUMKOD=H.KURUMKOD ";
            sql += "where H.DONEMBYIL=" + VeriErisim.DegerYap(strHedef.donemBYil) + " AND H.KURUMKOD=" + VeriErisim.DegerYap(strHedef.kurumKod.Replace(".", ""));
            sql += " group by " + alanlar;
            sql += " order by 1,2,3,4 ";

            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = $"INSERT INTO {TABLOAD}(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += $" SELECT {hedefYil} DONEMBYIL, " + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + " FROM " + TABLOAD;
            sql += $" WHERE DONEMBYIL={kaynakYil} AND 1 NOT IN (SELECT 1 FROM {TABLOAD} IC WHERE IC.DONEMBYIL={hedefYil} AND IC.HEDEFKOD = {TABLOAD}.HEDEFKOD)";

            return KomutCalistir(sql, 0);
        }
    }
}
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Misyon bilgilerini tutan STRMISYON veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABMisyon : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMisyon(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRMISYON";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// DONEMBYIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 0,
            KURUMKOD = 1,
            ARALIKBASYIL = 2,
            ARALIKSONYIL = 3
        }

        /// <summary>
        /// Misyonun tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DONEMBYIL", "KURUMKOD", "ARALIKBASYIL", "ARALIKSONYIL" };

        /// <summary>
        /// Misyon bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Misyon misyon)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(misyon.donemBYil) + ",";
            sql += VeriErisim.DegerYap(misyon.kurumKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(misyon.aralikBasiYil) + ",";
            sql += VeriErisim.DegerYap(misyon.aralikSonuYil) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(Misyon misyon)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARALIKBASYIL], misyon.aralikBasiYil);
            sql += " , " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARALIKSONYIL], misyon.aralikSonuYil);
            sql += " WHERE " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], misyon.donemBYil);
            sql += " AND " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], misyon.kurumKod.Replace(".", ""));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen misyon nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve misyon bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="misyon">Misyon bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan misyon bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Misyon misyon)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("Y", TABYazi.ALANLAR) + ", M.ARALIKBASYIL, M.ARALIKSONYIL, K.KURAD FROM " + TABYazi.TABLOAD + " Y LEFT OUTER JOIN " + TABLOAD + " M ON Y.DONEMBYIL = M.DONEMBYIL AND Y.KURUMKOD = M.KURUMKOD";
            sql += " INNER JOIN ButceHazirlik.dbo.ABSKUR K ON K.YIL= M.DONEMBYIL AND K.KURKOD = M.KURUMKOD";
            string kosul = string.Empty;
            if (misyon.dil.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("Y", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DILKOD], "=", misyon.dil.kod));
            if (!string.IsNullOrWhiteSpace(misyon.kurumKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("Y", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KURUMKOD], "=", misyon.kurumKod.Replace(".", "")));

            if (misyon.izlemeYil > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", "M.DONEMBYIL = (SELECT MAX(DONEMBYIL) FROM STRMISYON where DONEMBYIL<=" + misyon.izlemeYil + " AND " + misyon.izlemeYil + "<=ARALIKSONYIL)");
            }
            else
            {
                if (misyon.donemBYil > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("Y", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.DONEMBYIL], "=", misyon.donemBYil));
            }
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("Y", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KODTUR], "=", (int)ENUMStratejikPlan.Misyon));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("Y", TABYazi.ALANLAR[(int)TABYazi.ENUMAlanlar.KOD], "=", 1));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1 DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen dönem başlangıç yılını kullanarak misyonun veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="donemBYil">Dönem başlangıç yılı</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool MisyonVarMi(string kurumKod, int donemBYil)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KURUMKOD] };
            string[] strDeger = { kurumKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMBYIL] };
            int[] intDeger = { donemBYil };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = $"INSERT INTO {TABLOAD}(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += $" SELECT {hedefYil} DONEMBYIL, " + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + " FROM " + TABLOAD;
            sql += $" WHERE DONEMBYIL={kaynakYil} AND 1 NOT IN (SELECT 1 FROM {TABLOAD} IC WHERE IC.DONEMBYIL={hedefYil})";

            return KomutCalistir(sql, 0);
        }
    }
}
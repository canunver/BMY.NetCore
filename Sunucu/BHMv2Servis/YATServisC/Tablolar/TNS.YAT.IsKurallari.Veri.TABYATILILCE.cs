using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Yatırım projelerinin ve/veya alt projelerinin il, ilçe bilgilerini tutan YATPROJEILILCE
    /// veri tabanı tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATIlIlce : TABOrtak
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATIlIlce(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATPROJEILILCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// PROJENO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PROJENO = 0,

            /// <summary>
            /// ALTPROJENO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 1,

            /// <summary>
            /// ILKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ILKOD = 2,

            /// <summary>
            /// ILCEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ILCEKOD = 3,

            YIL = 4, SENARYOKOD = 5
        }

        /// <summary>
        /// Proje il, ilçe bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "PROJENO", "ALTPROJENO", "ILKOD", "ILCEKOD", "YIL", "SENARYOKOD" };

        /// <summary>
        /// Proje il, ilçe bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="bilgi">Proje il, ilçe bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string senaryoKod, int projeNo, int altProjeNo, ProjeIlIlce bilgi)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(projeNo) + ",";
            sql += VeriErisim.DegerYap(altProjeNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilIlce.il.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilIlce.ilceKodu) + ",";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen proje il, ilçe nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve projenin il, ilçe bilgileri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="bilgi">Proje il, ilçe bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan proje il, ilçe bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string senaryoKod, int projeNo, int altProjeNo)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT YATPROJEILILCE.PROJENO, YATPROJEILILCE.ALTPROJENO, YATPROJEILILCE.ILKOD, YATPROJEILILCE.ILCEKOD, YATILCE.ILCEADI, YATIL.ILADI, YATIL.ACIKLAMA";
            sql += " FROM YATIL RIGHT OUTER JOIN ";
            sql += " YATPROJEILILCE ON YATIL.ILKODU = YATPROJEILILCE.ILKOD LEFT OUTER JOIN";
            sql += " YATILCE ON YATPROJEILILCE.ILKOD = YATILCE.ILKODU AND YATPROJEILILCE.ILCEKOD = YATILCE.ILCEKODU";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (altProjeNo == 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.PROJENO];

            return veriErisim.DataReaderDondur(sql);
        }

        internal static string Listele(string yilUstAlanAd, string senaryoUstAlanAd, string projeUstAlanAd, string altProjeUstAlanAd)
        {
            string sql = "SELECT YATPROJEILILCE.PROJENO, YATPROJEILILCE.ALTPROJENO, YATPROJEILILCE.ILKOD, YATPROJEILILCE.ILCEKOD, YATILCE.ILCEADI, YATIL.ILADI, YATIL.ACIKLAMA";
            sql += " FROM YATIL RIGHT OUTER JOIN ";
            sql += " YATPROJEILILCE ON YATIL.ILKODU = YATPROJEILILCE.ILKOD LEFT OUTER JOIN";
            sql += " YATILCE ON YATPROJEILILCE.ILKOD = YATILCE.ILKODU AND YATPROJEILILCE.ILCEKOD = YATILCE.ILCEKODU";

            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yilUstAlanAd);
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + "=" + senaryoUstAlanAd);

            if (!string.IsNullOrWhiteSpace(projeUstAlanAd))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJENO] + "=" + projeUstAlanAd);
                sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO] + "= 0");
            }

            if (!string.IsNullOrWhiteSpace(altProjeUstAlanAd))
                sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ALTPROJENO] + "=" + altProjeUstAlanAd);
            sql += " ORDER BY 1,2";

            return sql;
        }

        /// <summary>
        /// Verilen proje il, ilçe nesnesini kullanarak, ilgili il, ilçe bilgisini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="bilgi">Proje il, ilçe bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string senaryoKod, int projeNo, int altProjeNo)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (altProjeNo == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeNo));


            return KomutCalistir(sql, 0);
        }

        //public Sonuc Guncelle(ProjeIlIlce bilgi)
        //{
        //    string sql = "";
        //    sql = "UPDATE " + TABLOAD + " SET ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], bilgi.ilIlce.il.kod) + ", ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILAD], bilgi.ilIlce.il.ad) + ", ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEKOD], bilgi.ilIlce.ilceKodu) + ", ";
        //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEAD], bilgi.ilIlce.ilceAdi);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", bilgi.projeNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", bilgi.altProjeNo));

        //    return KomutCalistir(sql, 1);
        //}

        //public bool ProjeIlIlceVarMi(int ilKodu)
        //{
        //    string[] intAlan = { ALANLAR[(int)ENUMAlanlar.PROJEKOD] };
        //    int[] intDeger = { ilKodu };

        //    return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        //}

        public Sonuc IlIlceKopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, bool proje, string projeKodListe)
        {
            string sql = "INSERT INTO " + TABLOAD + "(YIL, SENARYOKOD, PROJENO, ALTPROJENO, ILKOD, ILCEKOD) ";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, PROJENO, ALTPROJENO, ILKOD, ILCEKOD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], proje ? "=" : "<>", 0));
            sql = VeriErisim.KosulEkle(sql, "AND", "(1 NOT in (SELECT 1 FROM " + TABLOAD + $" IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.PROJENO ={TABLOAD}.PROJENO))");
            if (projeKodListe != null)
            {
                if (proje)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", "(PROJENO in (" + projeKodListe + "))");
                }
                else
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", "(1 in (" + TABYATAltProje.AltProjeProjeyeBagla(projeKodListe, TABLOAD) + "))");
                }
            }
            return KomutCalistir(sql, 0);
        }
    }
}
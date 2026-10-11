using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// YATALTPROJEKALKINMABOLGE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYATPROJEKALKINMABOLGE : TABOrtak
    {
        /// <summary>
        /// TABYATALTPROJEKALKINMABOLGE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATPROJEKALKINMABOLGE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATPROJEKALKINMABOLGE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 1,

            /// <summary>
            /// PROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJENO = 2,

            /// <summary>
            /// ALTPROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 3,

            /// <summary>
            /// BOLGEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BOLGEKOD = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "PROJENO"
            , "ALTPROJENO"
            , "BOLGEKOD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, AltProjeKalkinmaBolge yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string senaryoKod, int projeNo, int altProjeNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (altProjeNo == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, AltProjeKalkinmaBolge yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, string senaryoKod, int projeNo, int altProjeNo, string bolgeKod)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(projeNo);
            sql += "," + VeriErisim.DegerYap(altProjeNo);
            sql += "," + VeriErisim.DegerYap(bolgeKod);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Kopyala(int kaynakYil, string kaynakAsama, int hedefYil, string hedefAsama, bool proje, string projeKodListe)
        {
            string sql = "INSERT INTO " + TABLOAD + "(YIL, SENARYOKOD, PROJENO, ALTPROJENO, BOLGEKOD) ";
            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " YIL, " + VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " SENARYOKOD, PROJENO, ALTPROJENO, BOLGEKOD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], proje ? "=" : "<>", 0));
            sql = VeriErisim.KosulEkle(sql, "AND", "(1 NOT in (SELECT 1 FROM " + TABLOAD + $" IC WHERE IC.YIL = {hedefYil}  AND IC.SENARYOKOD={VeriErisim.DegerYap(hedefAsama.Replace(".", ""))} AND IC.ALTPROJENO ={TABLOAD}.ALTPROJENO))");
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

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, AltProjeKalkinmaBolge yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(int yil, string senaryoKod, int projeNo, int altProjeNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            if (altProjeNo == 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJENO], "=", projeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTPROJENO], "=", altProjeNo));
            return veriErisim.DataReaderDondur(sql);
        }

        internal static string Listele(string yilUstAlanAd, string senaryoUstAlanAd, string projeUstAlanAd, string altProjeUstAlanAd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

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

    }
}

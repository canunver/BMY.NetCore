using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
namespace TNS.IKY.IsKurallari.Veri
{
    /// <summary>
    /// IKYGOREVLER veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIKYGOREVLER : TABOrtak
    {
        /// <summary>
        /// TABIKYGOREVLER sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIKYGOREVLER(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYGOREVLER";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// PKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PKOD = 0,

            /// <summary>
            /// GOREVNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVNO = 1,

            /// <summary>
            /// UNVANKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UNVANKOD = 2,

            /// <summary>
            /// GOREVDURUMU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVDURUMU = 3,

            /// <summary>
            /// BIRIMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMI = 4,

            /// <summary>
            /// BASLAMATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASLAMATARIHI = 5,

            /// <summary>
            /// BITISTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISTARIHI = 6,

            ACIKLAMA=7, ONAYNO=8, ONAYTARIHI=9
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "PKOD"
            , "GOREVNO"
            , "UNVANKOD"
            , "GOREVDURUMU"
            , "BIRIMI"
            , "BASLAMATARIHI"
            , "BITISTARIHI"
            , "ACIKLAMA"
            , "ONAYNO"
            , "ONAYTARIHI"
        };

        public int NoBul(Gorevler form)
        {
            string sql = "SELECT MAX(GOREVNO) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", form.pKod));
            int tut = (int)veriErisim.TekKolonSQLDbl(sql);

            return tut + 1;
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, Gorevlendirme yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Gorevler form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", form.pKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOREVNO], "=", form.gorevNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, gorevler dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, Gorevlendirme yapısında nesne</param>
        /// <param name="satirNo">gorevler dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(Gorevler form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.pKod);
            sql += "," + VeriErisim.DegerYap(form.gorevNo);
            sql += "," + VeriErisim.DegerYap(form.unvanKod);
            sql += "," + VeriErisim.DegerYap((int)form.gorevDurumu);
            sql += "," + VeriErisim.DegerYap(form.birimi);
            sql += "," + VeriErisim.DegerYap(form.baslamaTarihi);
            sql += "," + VeriErisim.DegerYap(form.bitisTarihi);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.onayNo);
            sql += "," + VeriErisim.DegerYap(form.onayTarihi);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        private static string sorguOn()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", O.ADI, U.AD, U.SIRANO FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN Kullanici.dbo.ORGBIRIM O ON O.KOD = " + TABLOAD + ".BIRIMI ";
            sql += " LEFT OUTER JOIN IKYUNVAN as U ON " + TABLOAD + ".UNVANKOD = U.KOD ";
            return sql;
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, Gorevlendirme yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(Gorevler form)
        {
            string sql = sorguOn();
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", form.pKod));
            sql += " ORDER BY U.SIRANO";
            return veriErisim.DataReaderDondur(sql);
        }

        internal static string ListeleUstTablo(int gorevYili, string ustTabloAlan, TNSDateTime gorevBitisBasla = null, TNSDateTime gorevBitisSon = null)
        {
            string sql = sorguOn();
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PKOD] + "=" + ustTabloAlan;
            if (gorevYili > 1000 || (!TNSDateTime.IsNull(gorevBitisBasla) && !TNSDateTime.IsNull(gorevBitisSon)))
            {
                string kesisimSorgu;
                if ((!TNSDateTime.IsNull(gorevBitisBasla) && !TNSDateTime.IsNull(gorevBitisSon)))
                {
                    kesisimSorgu = string.Format("dbo.GunTarihAraligindaMi({0},{1},{2}) = 1", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIHI], VeriErisim.DegerYap(gorevBitisBasla), VeriErisim.DegerYap(gorevBitisSon));
                }
                else
                {
                    TNSDateTime donemBas = new TNSDateTime(gorevYili.ToString() + "-01-01");
                    TNSDateTime donemSon = new TNSDateTime((gorevYili + 1).ToString() + "-01-01");
                    kesisimSorgu = string.Format("dbo.DonemlerKesisiyorMu({0},{1},{2},{3}) = 1", VeriErisim.DegerYap(donemBas), VeriErisim.DegerYap(donemSon), TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI], TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIHI]);
                }
                sql = VeriErisim.KosulEkle(sql, "AND", kesisimSorgu);
            }
            sql += " ORDER BY U.SIRANO";
            return sql;
        }
    }
}

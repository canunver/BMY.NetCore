using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// BHFTASITMARKAMODEL veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHFTASITMARKAMODEL : TABOrtak
    {
        /// <summary>
        /// TABBHFTASITMARKAMODEL sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHFTASITMARKAMODEL(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHFTASITMARKAMODEL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,

            /// <summary>
            /// MARKA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MARKA = 1,

            /// <summary>
            /// MODEL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODEL = 2,

            /// <summary>
            /// MODEL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIPKODU = 3,

            KESIN = 4
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "KOD"
            , "MARKA"
            , "MODEL"
            , "TIPKODU"
            , "KESIN"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, TasitMarkaModel yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TasitMarkaModel form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, TasitMarkaModel yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(TasitMarkaModel form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.marka);
            sql += "," + VeriErisim.DegerYap(form.model);
            sql += "," + VeriErisim.DegerYap(form.tipKodu);
            sql += "," + VeriErisim.DegerYap(form.kesin);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, TasitMarkaModel yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(TasitMarkaModel form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", BHFTASITTIP.TIP  FROM " + TABLOAD + " INNER JOIN BHFTASITTIP ON BHFTASITMARKAMODEL.TIPKODU=BHFTASITTIP.KOD";

            string kosul = "";
            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MARKA], "LIKE", "%" + form.marka + "%"));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MODEL], "LIKE", "%" + form.model + "%"));
                if (form.tipKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIPKODU], "=", form.tipKodu));
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }

        public int EnBuyukNo()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;

            return (int)this.veriErisim.TekKolonSQLDbl(sql) + 1;
        }

        public IDataReader ListeleMarkaModel(TasitMarkaModel form)
        {
            string sql = "";
            int siraKolNo;
            if (!string.IsNullOrWhiteSpace(form.marka))
            {
                sql = "SELECT " + ALANLAR[0] + ", " + ALANLAR[2] + ", KESIN FROM " + TABLOAD;
                siraKolNo = 2;
            }
            else
            {
                sql = "SELECT DISTINCT " + ALANLAR[1] + " FROM " + TABLOAD;
                siraKolNo = 1;
            }
            string kosul = "";
            if (!string.IsNullOrWhiteSpace(form.marka))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MARKA], "=", form.marka));
            if (!string.IsNullOrWhiteSpace(form.model))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MODEL], "LIKE", "%" + form.model + "%"));
            if (form.tipKodu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIPKODU], "=", form.tipKodu));

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY " + siraKolNo.ToString();
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc TipGuncelle(int tipKod, string marka, string modelBasla)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + ALANLAR[(int)ENUMAlanlar.TIPKODU] + " = " + VeriErisim.DegerYap(tipKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MARKA], "=", marka));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MODEL], "LIKE", modelBasla + "%"));
            return KomutCalistir(sql, 0);
        }
    }
}

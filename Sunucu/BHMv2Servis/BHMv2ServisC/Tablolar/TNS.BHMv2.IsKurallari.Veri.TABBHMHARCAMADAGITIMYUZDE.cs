using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Harcama rakamlarının alt faaliyetlere dağıtılabilmesi için, tertip içi altfaaliyetin dağıtım oranının tutulduğu BHMODENEKMUHASEBATYUZDE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMHARCAMADAGITIMORAN : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMHARCAMADAGITIMORAN(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Alt faaliyetin tertip için oranının tutulduğu tablonun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMODENEKMUHASEBATYUZDE";

        /// <summary>
        /// Alt faaliyetin tertip için oranının tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 1,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 3,

            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 4,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 6,

            /// <summary>
            /// ORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ORAN = 7,
        }

        /// <summary>
        /// Alt faaliyetin tertip için oranının tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public string[] ALANLAR = { "YIL", "AY", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "ORAN", };

        /// <summary>
        /// Alt faaliyetin verilen tertip içindeki oranını insert sql komutu ile
        /// veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="yil">Alt faaliyet oranının yıl parametresi</param>
        /// <param name="ay">Alt faaliyet oranının ay parametresi</param>
        /// <param name="odenekBilgi">Alt faaliyeti, ilgili tertibi ve faaliyetin tertip içi oranını tutan nesne</param>
        /// <returns>Alt faaliyet oranını kaydeden sql komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(int yil, int ay, OdenekBilgi odenekBilgi)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(ay) + ",";
            sql += VeriErisim.DegerYap(odenekBilgi.tertip.kodlar[0].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(odenekBilgi.tertip.kodlar[1].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(odenekBilgi.tertip.kodlar[2].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(odenekBilgi.tertip.kodlar[3].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(odenekBilgi.tertip.kodlar[4].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToDbl(odenekBilgi.ekRakamlar[0])) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Alt faaliyetin verilen tertip içindeki oranını veri tabanından silen yordam.
        /// Tertip verilmişse o tertibin, verilmemişse o yıl ve ayın tüm tertiplerinin oranı silinir.
        /// </summary>
        /// <param name="yil">Silinecek oranların yıl parametresi</param>
        /// <param name="ay">Silinecek oranların ay parametresi</param>
        /// <param name="tertip">Silinecek oranların tertip bilgisini tutan nesne</param>
        /// <returns>Alt faaliyet oranlarını silen, sql komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, int ay, Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", ay));

            if (tertip != null)
            {
                sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], tertip, 0);
                sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], tertip, 1);
                sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], tertip, 2);
                sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], tertip, 3);
                sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], tertip, 4);
            }
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verilen yıl, ay ve tertip kıstası kullanılarak, kayıtlı alt faaliyet oranlarının DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Listelenecek oranların yıl parametresi</param>
        /// <param name="ay">Listelenecek oranların ay parametresi</param>
        /// <param name="tertip">Listelenecek oranların tertip bilgisini tutan nesne</param>
        /// <returns>Oranları listeleyen sql komutunun çalıştırılmasının sonucunu, DataReader yapısında döndürür</returns>
        public IDataReader Listele(int yil, int ay, Tertip tertip)
        {
            string sql = string.Empty;

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.PEBKOD] + ", " + ALANLAR[(int)ENUMAlanlar.ORAN];
            sql = sql + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            sql = sql + " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

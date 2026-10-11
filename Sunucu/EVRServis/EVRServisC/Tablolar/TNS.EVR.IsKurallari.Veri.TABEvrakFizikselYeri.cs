using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// EVREVRAKFIZIKSELYERI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABEvrakFizikselYeri : TABOrtak
    {
        /// <summary>
        /// TABEVREVRAKFIZIKSELYERI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEvrakFizikselYeri(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "EVREVRAKFIZIKSELYERI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            PREVRAKNO = 0,
            BINA = 1,
            KAT = 2,
            ODA = 3,
            BOLUM = 4,
            RAF = 5,
            KLASOR = 6,
            RFID = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {"PREVRAKNO" , "BINA" , "KAT" , "ODA" , "BOLUM" , "RAF" , "KLASOR" , "RFID"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, EvrakFizikselYeri yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(EvrakFizikselYeri form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EvrakFizikselYeri yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(EvrakFizikselYeri form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.prEvrakNo);
            sql += "," + VeriErisim.DegerYap(form.bina);
            sql += "," + VeriErisim.DegerYap(form.kat);
            sql += "," + VeriErisim.DegerYap(form.oda);
            sql += "," + VeriErisim.DegerYap(form.bolum);
            sql += "," + VeriErisim.DegerYap(form.raf);
            sql += "," + VeriErisim.DegerYap(form.klasor);
            sql += "," + VeriErisim.DegerYap(form.RFID);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, EvrakFizikselYeri yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(EvrakFizikselYeri form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            return veriErisim.DataReaderDondur(sql);
        }

    }
}

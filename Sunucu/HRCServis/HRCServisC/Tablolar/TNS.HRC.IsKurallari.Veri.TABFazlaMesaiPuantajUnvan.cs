using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABFazlaMesaiPuantajUnvan : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFazlaMesaiPuantajUnvan(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Fazla Mesai Puantaj tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLFAZLAMESAIPUANTAJUNVAN";

        /// <summary>
        /// Fazla Mesai Puantaj tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// UNVANKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVANKOD = 0,
            /// <summary>
            /// TOPLAMSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMSAAT = 1,
            /// <summary>
            /// KESILENSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KESILENSAAT = 2,
        }

        /// <summary>
        /// Fazla Mesai Puantaj tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "UNVANKOD", "TOPLAMSAAT", "KESILENSAAT" };

        public static string TABLOADUNVAN = "HRCPERUNVANVIEW";

        public enum ENUMAlanlarUnvan : int
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1
        }

        public static string[] ALANLARUNVAN = { "KOD", "AD" };


        /// <summary>
        /// Fazla Mesai Puantaj unvan bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <returns>
        /// Fazla Mesai Puantaj unvan bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele()
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", HRCPERUNVANVIEW.AD " + " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABLOADUNVAN + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKOD] + "=" + TABLOADUNVAN + "." + ALANLARUNVAN[(int)ENUMAlanlarUnvan.KOD];
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.UNVANKOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public IDataReader ListeleUnvan()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADUNVAN, ALANLARUNVAN) + " FROM " + TABLOADUNVAN;
            sql += " ORDER BY " + ALANLARUNVAN[(int)ENUMAlanlarUnvan.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Fazla Mesai Puantaj unvan bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek fazla mesai puantaj unvan bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            foreach (FazlaMesaiPuantajUnvan item in liste.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(item.unvanKod) + ",";
                sql += VeriErisim.DegerYap(item.toplamSaat) + ",";
                sql += VeriErisim.DegerYap(item.kesilenSaat) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;

            }

            return sonuc;
        }

        /// <summary>
        /// Fazla mesai puantaj unvan bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek fazla mesai puantaj unvan bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil()
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;

            return KomutCalistir(sql, 0);
        }
    }
}

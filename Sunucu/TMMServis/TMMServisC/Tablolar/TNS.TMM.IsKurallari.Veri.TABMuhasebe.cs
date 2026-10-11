using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Muhasebe bilgilerini tutan TASMUHASEBE veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABMuhasebe : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMuhasebe(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASMUHASEBE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Muhasebe adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// İl kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILKOD = 2,
            /// <summary>
            /// İlçe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILCEKOD = 3,
            /// <summary>
            /// Kapalı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAPALI = 4,
            /// <summary>
            /// Bölge kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BOLGEKOD = 5,
            /// <summary>
            /// Kdv mükellefi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVMUKELLEFI = 6,
            /// <summary>
            /// İcmal Muaf alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ICMALMUAF = 7
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "KOD", "AD", "ILKOD", "ILCEKOD", "KAPALI", "BOLGEKOD", "KDVMUKELLEFI", "ICMALMUAF" };

        /// <summary>
        /// Muhasebe bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Muhasebe bilgilerini tutan, Muhasebe türünde nesne</param>
        /// <returns>Ekleme,Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(Muhasebe muhasebe)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(muhasebe.kod) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.ad) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.ilKod) + ", ";
            sql += VeriErisim.DegerYap(muhasebe.ilceKod) + ",";
            sql += VeriErisim.DegerYap(muhasebe.kapali) + ",";
            sql += VeriErisim.DegerYap(muhasebe.bolgeKod) + ",";
            sql += VeriErisim.DegerYap(muhasebe.kdvMukellefi) + ",";
            sql += VeriErisim.DegerYap(muhasebe.icmalMuaf) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], muhasebe.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], muhasebe.ilKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCEKOD], muhasebe.ilceKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAPALI], muhasebe.kapali) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOLGEKOD], muhasebe.bolgeKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVMUKELLEFI], muhasebe.kdvMukellefi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICMALMUAF], muhasebe.icmalMuaf);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", muhasebe.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Muhasebe bilgisini kullanarak
        /// Muhasebe bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Muhasebe bilgilerini tutan, Muhasebe türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(Muhasebe muhasebe)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", muhasebe.kod.Trim());

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Muhasebe bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Muhasebe bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Muhasebe bilgilerini tutan, Muhasebe türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Muhasebe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(Muhasebe muhasebe)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (muhasebe.kod != string.Empty)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", muhasebe.kod.Trim()));

            if (muhasebe.ad != string.Empty)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + muhasebe.ad + "%"));

            if (muhasebe.ilKod != string.Empty)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", muhasebe.ilKod));

            if (muhasebe.ilceKod != string.Empty)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILCEKOD], "=", muhasebe.ilceKod));

            if (muhasebe.kapali > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KAPALI], "=", muhasebe.kapali));

            if (muhasebe.kdvMukellefi > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KDVMUKELLEFI], "=", muhasebe.kdvMukellefi));

            if (muhasebe.icmalMuaf > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ICMALMUAF], "=", muhasebe.icmalMuaf));

            if (muhasebe.bolgeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BOLGEKOD], "=", muhasebe.bolgeKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

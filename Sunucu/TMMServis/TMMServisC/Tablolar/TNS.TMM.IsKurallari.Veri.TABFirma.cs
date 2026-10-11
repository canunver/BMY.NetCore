using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Firma bilgilerini tutan TASFIRMA veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABFirma : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFirma(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASFIRMA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Kullanıcı kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,
            /// <summary>
            /// Firma kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 1,
            /// <summary>
            /// Firma adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 2,
            /// <summary>
            /// Vergi Numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            VNO = 3,
            /// <summary>
            /// Vergi Dairesi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            VD = 4,
            /// <summary>
            /// Banka alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BANKA = 5,
            /// <summary>
            /// Hesap numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPNO = 6,
            /// <summary>
            /// Telefon numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TEL = 7,
            /// <summary>
            /// Faks numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAX = 8,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "KULLANICIKODU", "KOD", "AD", "VNO", "VD", "BANKA", "HESAPNO", "TEL", "FAX" };

        /// <summary>
        /// Firma bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="firma">Firma bilgilerini tutan, Firma türünde nesne</param>
        /// <returns>Ekleme,Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc FirmaKaydet(FirmaBilgisi firma)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(firma.kullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(firma.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(firma.ad) + ", ";
            sql = sql + VeriErisim.DegerYap(firma.vno) + ", ";
            sql = sql + VeriErisim.DegerYap(firma.vd) + ", ";
            sql = sql + VeriErisim.DegerYap(firma.banka) + ", ";
            sql = sql + VeriErisim.DegerYap(firma.hesapNo) + ",";
            sql = sql + VeriErisim.DegerYap(firma.tel) + ",";
            sql = sql + VeriErisim.DegerYap(firma.fax) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.AD] + "='" + firma.ad + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.BANKA] + "='" + firma.banka + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.HESAPNO] + "='" + firma.hesapNo + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.VD] + "='" + firma.vd + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.VNO] + "='" + firma.vno + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.TEL] + "='" + firma.tel + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.FAX] + "='" + firma.fax + "'";
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", firma.kullaniciKodu);
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", firma.kod);

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Firma bilgisini kullanarak
        /// ilgili Firma bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="firma">Firma bilgilerini tutan, FirmaBilgisi türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc FirmaSil(FirmaBilgisi firma)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", firma.kullaniciKodu));
            if (firma.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", firma.kod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Firma bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Firma bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="firma">Firma bilgilerini tutan, FirmaBilgisi türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Firma bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader FirmaListele(FirmaBilgisi firma)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (firma.kullaniciKodu != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", firma.kullaniciKodu));

            if (firma.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", firma.kod));

            if (firma.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + firma.ad + "%"));

            if (firma.vno != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VNO], "LIKE", "%" + firma.vno + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

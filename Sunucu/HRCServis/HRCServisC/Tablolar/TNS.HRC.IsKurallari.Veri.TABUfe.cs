using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Üfe bilgisini tutan HRCUFE veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABUfe : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABUfe(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Üfe bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCUFE";

        /// <summary>
        /// Üfe bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
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
            /// ORAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ORAN = 2,
        }

        /// <summary>
        /// Üfe tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "AY", "ORAN" };

        /// <summary>
        /// Üfe bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek üfe bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (Ufe f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.ay) + ",";
                sql += VeriErisim.DegerYap(f.oran) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORAN], f.oran);
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", f.ay));

                    sonuc = KomutCalistir(sql, 1);

                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Üfe bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak üfe bilgisi kıstas parametresi</param>
        /// <returns>
        /// Üfe bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(Ufe bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Üfe kümülatif toplam bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="ay">Listeleme için kullanılacak ay bilgisi kıstas parametresi</param>
        /// <returns>
        /// Üfe bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public double ToplamBul(int yil, int ay)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT SUM(" + ALANLAR[(int)ENUMAlanlar.ORAN] + ") FROM " + TABLOAD;
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], ">=", yil));
            if (ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], ">=", ay));

            if (yil > 0 && ay > 0)
            {
                if (kosul != "")
                    kosul = "(" + kosul + ") OR";

                kosul += "(";
                kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], ">", yil));
                kosul += ")";
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }
    }
}
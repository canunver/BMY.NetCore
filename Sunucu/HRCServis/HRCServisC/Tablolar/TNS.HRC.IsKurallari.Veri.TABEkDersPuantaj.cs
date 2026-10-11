using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Ek Ders Puantaj bilgisini tutan PSLEKDERSPUANTAJ veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABEkDersPuantaj : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEkDersPuantaj(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Ek Ders Puantaj tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLEKDERSPUANTAJ";

        /// <summary>
        /// Ek Ders Puantaj tablo alanlarının sırasını tutan enumeration
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
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 2,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3,
            /// <summary>
            /// YILLIKSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YILLIKSAAT = 4,
            /// <summary>
            /// HAFTALIKSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTALIKSAAT = 5,
            /// <summary>
            /// HAFTAICIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTAICIUCRET = 6,
            /// <summary>
            /// HAFTASONUUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTASONUUCRET = 7,
            /// <summary>
            /// VARSAYILAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VARSAYILAN = 8,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 9,
        }

        /// <summary>
        /// Ek Ders Puantaj tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "YIL", "AY", "KOD", "ACIKLAMA", "YILLIKSAAT", "HAFTALIKSAAT", "HAFTAICIUCRET", "HAFTASONUUCRET", "VARSAYILAN", "DURUM" };

        /// <summary>
        /// Ek Ders Puantaj bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listelenecek ek ders puantaj bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Ek ders puantaj bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(EkDersPuantaj f)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", f.durum));
            if (f.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", f.ay));

            if (f.kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", f.kod));

            if (f.aciklama != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + f.aciklama + "%"));

            sql += " WHERE " + kosul;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.VARSAYILAN] + " DESC, " + ALANLAR[(int)ENUMAlanlar.YIL] + ", " + ALANLAR[(int)ENUMAlanlar.ACIKLAMA];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Ek Ders Puantaj açıklama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listelenecek ek ders puantaj açıklama bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Ek ders puantaj açıklama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleAciklama(EkDersPuantaj f)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] +
                 " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", f.durum));
            if (f.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", f.ay));

            if (f.kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", f.kod));

            if (f.aciklama != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + f.aciklama + "%"));

            sql += " WHERE " + kosul;
            sql += " GROUP BY " + ALANLAR[(int)ENUMAlanlar.YIL] + ", "
                                + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + ", "
                                + ALANLAR[(int)ENUMAlanlar.KOD];
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + ", "
                                + ALANLAR[(int)ENUMAlanlar.ACIKLAMA];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Ek ders puantaj bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek ek ders puantaj bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(EkDersPuantaj f)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            if (f.kod == 0)
            {
                sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
                //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
                f.kod = OrtakFonksiyonlar.ConvertToInt(veriErisim.TekKolonSQLDbl(sql).ToString(), 0) + 1;

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.ay) + ",";
                sql += VeriErisim.DegerYap(f.kod) + ",";
                sql += VeriErisim.DegerYap(f.aciklama) + ",";
                sql += VeriErisim.DegerYap(f.yillikSaat) + ",";
                sql += VeriErisim.DegerYap(f.haftalikSaat) + ",";
                sql += VeriErisim.DegerYap(f.haftaiciUcret) + ",";
                sql += VeriErisim.DegerYap(f.haftasonuUcret) + ",";
                sql += VeriErisim.DegerYap(f.varsayilan) + ",";
                sql += VeriErisim.DegerYap(f.durum) + ")";

                sonuc = KomutCalistir(sql, 1);
                sonuc.anahtar = f.kod.ToString();
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + " = " + VeriErisim.DegerYap(f.aciklama) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.HAFTAICIUCRET] + " = " + VeriErisim.DegerYap(f.haftaiciUcret) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.HAFTALIKSAAT] + " = " + VeriErisim.DegerYap(f.haftalikSaat) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.HAFTASONUUCRET] + " = " + VeriErisim.DegerYap(f.haftasonuUcret) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.VARSAYILAN] + " = " + VeriErisim.DegerYap(f.varsayilan) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.YILLIKSAAT] + " = " + VeriErisim.DegerYap(f.yillikSaat) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.DURUM] + " = " + VeriErisim.DegerYap(f.durum);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", f.ay));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", f.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Ek ders puantaj bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek ek ders puantaj bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(EkDersPuantaj f)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", f.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", f.kod));

            return KomutCalistir(sql, 0);
        }
    }
}

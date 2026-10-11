using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Fazla Mesai Puantaj bilgisini tutan PSLFAZLAMESAIPUANTAJ veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABFazlaMesaiPuantaj : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFazlaMesaiPuantaj(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Fazla Mesai Puantaj tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLFAZLAMESAIPUANTAJ";

        /// <summary>
        /// Fazla Mesai Puantaj tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 1,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 2,
            /// <summary>
            /// AYLIKSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AYLIKSAAT = 3,
            /// <summary>
            /// HAFTAICISAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTAICISAAT = 4,
            /// <summary>
            /// HAFTAICIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HAFTAICIUCRET = 5,
            /// <summary>
            /// CUMARTESISAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CUMARTESISAAT = 6,
            /// <summary>
            /// CUMARTESIUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CUMARTESIUCRET = 7,
            /// <summary>
            /// PAZARSAAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PAZARSAAT = 8,
            /// <summary>
            /// PAZARUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PAZARUCRET = 9,
            /// <summary>
            /// VARSAYILAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VARSAYILAN = 10
        }

        /// <summary>
        /// Fazla Mesai Puantaj tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={"YIL", "KOD", "ACIKLAMA", "AYLIKSAAT", "HAFTAICISAAT", "HAFTAICIUCRET", "CUMARTESISAAT", "CUMARTESIUCRET", "PAZARSAAT", "PAZARUCRET", "VARSAYILAN" };

        /// <summary>
        /// Fazla Mesai Puantaj bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="f">Listeleme için kullanılacak fazla mesai puantaj kıstas parametresi</param>
        /// <returns>
        /// Fazla Mesai Puantaj bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(FazlaMesaiPuantaj f)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));

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
        /// Fazla Mesai Puantaj bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Eklenecek fazla mesai puantaj bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(FazlaMesaiPuantaj f)
        {
            string sql = "";
            Sonuc sonuc=new Sonuc();

            if (f.kod == 0)
            {
                sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
                //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
                f.kod = OrtakFonksiyonlar.ConvertToInt(veriErisim.TekKolonSQLDbl(sql).ToString(), 0) + 1;

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.kod) + ",";
                sql += VeriErisim.DegerYap(f.aciklama) + ",";
                sql += VeriErisim.DegerYap(f.aylikSaat) + ",";
                sql += VeriErisim.DegerYap(f.haftaIciSaat) + ",";
                sql += VeriErisim.DegerYap(f.haftaIciUcret) + ",";
                sql += VeriErisim.DegerYap(f.cumartesiSaat) + ",";
                sql += VeriErisim.DegerYap(f.cumartesiUcret) + ",";
                sql += VeriErisim.DegerYap(f.pazarSaat) + ",";
                sql += VeriErisim.DegerYap(f.pazarUcret) + ",";
                sql += VeriErisim.DegerYap(f.varsayilan) + ")";

                sonuc = KomutCalistir(sql, 1);
                sonuc.anahtar = f.kod.ToString();
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + " = " + VeriErisim.DegerYap(f.aciklama) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.AYLIKSAAT] + " = " + VeriErisim.DegerYap(f.aylikSaat) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.HAFTAICISAAT] + " = " + VeriErisim.DegerYap(f.haftaIciSaat) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.HAFTAICIUCRET] + " = " + VeriErisim.DegerYap(f.haftaIciUcret) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.CUMARTESISAAT] + " = " + VeriErisim.DegerYap(f.cumartesiSaat) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.CUMARTESIUCRET] + " = " + VeriErisim.DegerYap(f.cumartesiUcret) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.PAZARSAAT] + " = " + VeriErisim.DegerYap(f.pazarSaat) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.PAZARUCRET] + " = " + VeriErisim.DegerYap(f.pazarUcret) + ",";
                sql += ALANLAR[(int)ENUMAlanlar.VARSAYILAN] + " = " + VeriErisim.DegerYap(f.varsayilan);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", f.kod));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Fazla mesai puantaj bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="f">Silinecek fazla mesai puantaj bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(FazlaMesaiPuantaj f)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", f.kod));

            return KomutCalistir(sql, 0);
        }
    }
}

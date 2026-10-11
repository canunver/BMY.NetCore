using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABTRMHIBEMAKINAOZELLIGI : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTRMHIBEMAKINAOZELLIGI(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMHIBEMAKINAOZELLIGI";

        /// <summary>
        /// Malzeme grubu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 1,
            /// <summary>
            /// MALZEMEGRUBUKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMEGRUPKOD = 2,
            SIRANO = 3,
            COKLUSECIM = 4,
            COKLUSECIMICERIK = 5, 
        }

        /// <summary>
        /// Malzeme grubu tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "ADI", "MALZEMEGRUPKOD", "SIRANO", "COKLUSECIM", "COKLUSECIMICERIK" };

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Malzeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALZEMEGRUPKOD], bilgi.malzemeGrubu.kod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], bilgi.siraNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.COKLUSECIM], bilgi.cokluSecim) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.COKLUSECIMICERIK], bilgi.cokluSecimIcerik);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.malzemeGrubu.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.cokluSecim) + ",";
                sql += VeriErisim.DegerYap(bilgi.cokluSecimIcerik) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Malzeme bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Malzeme grubu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(Malzeme bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," +
                TABMalzemeGrubu.TABLOAD + "." + TABMalzemeGrubu.ALANLAR[(int)TABMalzemeGrubu.ENUMAlanlar.AD] + 
                " FROM " + TABLOAD + " LEFT OUTER JOIN " +
                      TABMalzemeGrubu.TABLOAD + " ON " +
                      TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALZEMEGRUPKOD] + " = " + TABMalzemeGrubu.TABLOAD + "." + TABMalzemeGrubu.ALANLAR[(int)TABMalzemeGrubu.ENUMAlanlar.KOD];

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADI], " LIKE ", "%" + bilgi.ad + "%"));

            if (bilgi.malzemeGrubu.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMEGRUPKOD], "=", bilgi.malzemeGrubu.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABMalzemeGrubu.TABLOAD + "." + TABMalzemeGrubu.ALANLAR[(int)TABMalzemeGrubu.ENUMAlanlar.AD] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tabloda kullanılan en büyük kod numarasını döndürür
        /// </summary>
        /// <returns>Verilen kriter bilgisine uygun kayıt bulunursa true, yoksa false döndürür</returns>
        public int EnBuyukKod()
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }
    }
}
using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABYatirimMakinaTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYatirimMakinaTanim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMYATIRIMMAKINE";

        /// <summary>
        /// Malzeme grubu özellik bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADOzellik = "TRMYATIRIMMAKINEOZELLIK";

        /// <summary>
        /// Malzeme grubu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            MALZEMEGRUBUKOD = 2,
            ANAKOD = 3,
            KULLANIMDA = 4,
        }

        /// <summary>
        /// Malzeme grubu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarOzellik
        {
            /// <summary>
            /// MALZEMEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMEKOD = 0,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,
            /// <summary>
            /// OZELLIKKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OZELLIKKOD = 2,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 3,
        }

        /// <summary>
        /// Malzeme grubu tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "AD", "MALZEMEGRUBUKOD", "ANAKOD", "KULLANIMDA" };

        /// <summary>
        /// Malzeme grubu özellik tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAROZELLIK = { "MALZEMEKOD", "SIRANO", "OZELLIKKOD", "AD" };

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
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBUKOD], bilgi.malzemeGrubu.kod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], bilgi.anaKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANIMDA], bilgi.kullanimda);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.malzemeGrubu.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.anaKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.kullanimda) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Malzeme ana kod bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc MalzemeAnaKodGuncelle(int malzemeKod, int anaMalzemeKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], anaMalzemeKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", malzemeKod));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetTanimOzellik(MakinaTanimOzellik bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADOzellik + " (" + VeriErisim.TumAlanlar("", ALANLAROZELLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.malzemeKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ozellikKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ")";

            sonuc = KomutCalistir(sql, 1);

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

        public Sonuc SilTanimOzellik(int malzemeKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADOzellik;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROZELLIK[(int)ENUMAlanlarOzellik.MALZEMEKOD], "=", malzemeKod));

            return KomutCalistir(sql, 0);
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

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", " +
                "TRMYATIRIMMALZEMEGRUP.ADI"  +
                " FROM " + TABLOAD + " LEFT OUTER JOIN TRMYATIRIMMALZEMEGRUP ON " +
                      TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBUKOD] + " = " + "TRMYATIRIMMALZEMEGRUP.KOD";

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.kullanimda > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANIMDA], "=", bilgi.kullanimda));

            if (bilgi.anaKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", bilgi.anaKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " <> " + ALANLAR[(int)ENUMAlanlar.ANAKOD]);
            }

            if (bilgi.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + bilgi.ad + "%"));

            if (bilgi.malzemeGrubu.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBUKOD], "=", bilgi.malzemeGrubu.kod));

            if (bilgi.malzemeBaglama)
                kosul = VeriErisim.KosulEkle(kosul, "AND", " ANAKOD = CASE WHEN ANAKOD <> 0 THEN TRMMALZEME.KOD ELSE ANAKOD END");

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + "TRMYATIRIMMALZEMEGRUP.ADI" + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Malzeme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Malzeme grubu bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader AnaMalzemeKodVarMi(int anaMalzemeKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT * FROM " + TABLOAD;

            if (anaMalzemeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", anaMalzemeKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
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
        public IDataReader ListeleMakinaTanimOzellik(int malzemeGrupKod, int malzemeKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT TRMYATIRIMMAKINAOZELLIGI.*";

            if (malzemeKod > 0)
                sql += "," + VeriErisim.TumAlanlar(TABLOADOzellik, ALANLAROZELLIK);

            sql += " FROM TRMYATIRIMMAKINAOZELLIGI";

            if (malzemeKod > 0)
            {
                sql += " LEFT OUTER JOIN ";
                sql += TABLOADOzellik + " ON ";
                sql += TABLOADOzellik + "." + ALANLAROZELLIK[(int)ENUMAlanlarOzellik.OZELLIKKOD] + " = " + "TRMYATIRIMMAKINAOZELLIGI.KOD";
            }
            if (malzemeGrupKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMYATIRIMMAKINAOZELLIGI", "MALZEMEGRUPKOD", "=", malzemeGrupKod));
            if (malzemeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADOzellik, ALANLAROZELLIK[(int)ENUMAlanlarOzellik.MALZEMEKOD], "=", malzemeKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + "TRMYATIRIMMAKINAOZELLIGI.SIRANO";

            return veriErisim.DataReaderDondur(sql);
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
        public IDataReader ListeleMakinaTanimVeOzellikleri(Malzeme m)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT TRMYATIRIMMAKINAOZELLIGI.*";
            sql += "," + VeriErisim.TumAlanlar(TABLOADOzellik, ALANLAROZELLIK);
            sql += "," + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += "," + "TRMYATIRIMMALZEMEGRUP.ADI";

            sql += " FROM " + "TRMYATIRIMMAKINAOZELLIGI";

            sql += " LEFT OUTER JOIN TRMYATIRIMMALZEMEGRUP ON " +
                      "TRMYATIRIMMAKINAOZELLIGI.MALZEMEGRUPKOD" + " = " + "TRMYATIRIMMALZEMEGRUP.KOD";

            sql += " LEFT OUTER JOIN ";
            sql += TABLOAD + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MALZEMEGRUBUKOD] + " = " + "TRMYATIRIMMAKINAOZELLIGI.MALZEMEGRUPKOD";

            sql += " LEFT OUTER JOIN ";
            sql += TABLOADOzellik + " ON ";
            sql += TABLOADOzellik + "." + ALANLAROZELLIK[(int)ENUMAlanlarOzellik.OZELLIKKOD] + " = " + "TRMYATIRIMMAKINAOZELLIGI.KOD";

            kosul = TABLOADOzellik + "." + ALANLAROZELLIK[(int)ENUMAlanlarOzellik.MALZEMEKOD] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];

            if (m.malzemeGrubu.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TRMYATIRIMMAKINAOZELLIGI", "MALZEMEGRUPKOD", "=", m.malzemeGrubu.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADOzellik + "." + ALANLAROZELLIK[(int)ENUMAlanlarOzellik.MALZEMEKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public bool MalzemeBasvurdaKullanilmisMi(int malzemeKod)
        {
            string sql = " SELECT COUNT(*) ";
            sql += " FROM TRMHIBEMAKINA INNER JOIN  TRMHIBEMAKINADETAY ON TRMHIBEMAKINA.KOD = TRMHIBEMAKINADETAY.HIBEKOD ";
            sql += " WHERE TRMHIBEMAKINA.DURUM < 99 ";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEMAKINADETAY", "MALZEMEKOD", "=", malzemeKod));

            return veriErisim.TekKolonSQLInt(sql) > 0;
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
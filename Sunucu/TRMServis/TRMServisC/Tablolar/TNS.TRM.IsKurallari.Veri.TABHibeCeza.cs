using System.Data;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeCeza : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeCeza(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Malzeme grubu bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TRMCEZA";

        /// <summary>
        /// Malzeme grubu özellik bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADKISILER = "TRMCEZALIKISILER";

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
            /// BASLANGICTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BASLANGICTARIHI = 1,
            /// <summary>
            /// BITISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BITISTARIHI = 2,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3,
            /// <summary>
            /// KAYITEDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYITEDEN = 4,
            /// <summary>
            /// KAYITTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYITTARIHI = 5,
        }

        /// <summary>
        /// Malzeme grubu bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarKisiler
        {
            /// <summary>
            /// CEZAKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CEZAKOD = 0,
            /// <summary>
            /// TCNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCNO = 1,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 2,
        }

        /// <summary>
        /// Malzeme grubu tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "BASLANGICTARIHI", "BITISTARIHI", "ACIKLAMA", "KAYITEDEN", "KAYITTARIHI" };

        /// <summary>
        /// Malzeme grubu özellik tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARKISILER = { "CEZAKOD", "TCNO", "ADSOYAD" };

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Ceza bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], bilgi.baslangicTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIHI], bilgi.bitisTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITEDEN], bilgi.kayitEden) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], bilgi.kayitTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.baslangicTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.bitisTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
                sql += VeriErisim.DegerYap(bilgi.kayitEden) + ",";
                sql += VeriErisim.DegerYap(bilgi.kayitTarihi) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetKisiler(CezaliKisiler bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOADKISILER + " (" + VeriErisim.TumAlanlar("", ALANLARKISILER) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.cezaKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.TCNO) + ",";
            sql += VeriErisim.DegerYap(bilgi.adSoyad) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Malzeme grubu bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Ceza bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilKisiler(Ceza bilgi)
        {
            string sql = "";
            int cnt = 0;

            sql = "DELETE FROM " + TABLOADKISILER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKISILER[(int)ENUMAlanlarKisiler.CEZAKOD], "=", bilgi.kod));

            //if (bilgi.cezaliKisiler != null  && !string.IsNullOrEmpty(bilgi.cezaliKisiler[0].TCNO) && bilgi.tip != "Kayıt" )
            //{
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKISILER[(int)ENUMAlanlarKisiler.TCNO], "=", bilgi.cezaliKisiler[0].TCNO));
            //    cnt = 1;
            //}

            return KomutCalistir(sql, cnt);
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
        public IDataReader Listele(Ceza bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI] + " DESC," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BITISTARIHI];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ceza bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Ceza bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleCezaVeCezaliKisiler(int cezaKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + ", " + VeriErisim.TumAlanlar(TABLOADKISILER, ALANLARKISILER) + " FROM " + TABLOAD;
            sql += " INNER JOIN " + TABLOADKISILER;
            sql += " ON " + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + ALANLARKISILER[(int)ENUMAlanlarKisiler.CEZAKOD];

            if (cezaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", cezaKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADKISILER + "." + ALANLARKISILER[(int)ENUMAlanlarKisiler.ADSOYAD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ceza Kişileri bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// Ceza Kişileri bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader CezaliKisileriListele(int cezaKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKISILER, ALANLARKISILER) + " FROM " + TABLOADKISILER;

            if (cezaKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKISILER, ALANLARKISILER[(int)ENUMAlanlarKisiler.CEZAKOD], "=", cezaKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOADKISILER + "." + ALANLARKISILER[(int)ENUMAlanlarKisiler.ADSOYAD];

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

        public bool CezaliMi(string TCNo)
        {
            if (string.IsNullOrEmpty(TCNo))
                return false;

            bool cezali = false;
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];
            sql += " FROM " + TABLOAD + "," + TABLOADKISILER;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADKISILER + "." + ALANLARKISILER[(int)ENUMAlanlarKisiler.CEZAKOD];
            sql += " AND " + TABLOADKISILER + "." + ALANLARKISILER[(int)ENUMAlanlarKisiler.TCNO] + "=" + "'" + TCNo + "'";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], "<=", new TNSDateTime(DateTime.Now)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIHI], ">=", new TNSDateTime(DateTime.Now)));
            IDataReader r = veriErisim.DataReaderDondur(sql);
            if (r.Read())
                cezali = true;

            r.Close();
            return cezali;
        }
    }
}
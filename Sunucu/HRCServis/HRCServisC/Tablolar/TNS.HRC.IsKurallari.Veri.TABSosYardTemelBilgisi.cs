using System;
using System.Data;
using TNS.Veri;
using TNS.HRC;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Sosyal Yardım Temel Tanım bilgisini tutan PSLSOSYARDTEMELBILGISI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSosYardTemelBilgi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSosYardTemelBilgi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Sosyal Yardım Temel Tanım bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLSOSYARDTEMELBILGISI";

        /// <summary>
        /// Sosyal Yardım Temel Tanım bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// AILEYARDIMGOST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AILEYARDIMGOST = 2,
            /// <summary>
            /// COCUKYARDIMGOST0_6 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            COCUKYARDIMGOST0_6 = 3,
            /// <summary>
            /// COCUKYARDIMGOST6_X alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            COCUKYARDIMGOST6_X = 4,
            /// <summary>
            /// DOGUMYARDIMGOST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOGUMYARDIMGOST = 5,
            /// <summary>
            /// MEMUROLUMKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MEMUROLUMKATSAYI = 6,
            /// <summary>
            /// ESCOCUKOLUMKATSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ESCOCUKOLUMKATSAYI = 7,
        }

        /// <summary>
        /// Sosyal Yardım Temel Tanım tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "AY", "AILEYARDIMGOST", "COCUKYARDIMGOST0_6", "COCUKYARDIMGOST6_X", "DOGUMYARDIMGOST", "MEMUROLUMKATSAYI", "ESCOCUKOLUMKATSAYI" };

        /// <summary>
        /// Sosyal Yardım Temel Tanım bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek sosyal yardım temel tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (TNS.HRC.PSLSosyalYardimTemelBilgisi f in bilgi.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(f.yil) + ",";
                sql += VeriErisim.DegerYap(f.ay) + ",";
                sql += VeriErisim.DegerYap(f.aileYardimGost) + ",";
                sql += VeriErisim.DegerYap(f.cocukYardimGost0_6) + ",";
                sql += VeriErisim.DegerYap(f.cocukYardimGost6_X) + ",";
                sql += VeriErisim.DegerYap(f.dogumYardimGost) + ",";
                sql += VeriErisim.DegerYap(f.memurOlumKatsayi) + ",";
                sql += VeriErisim.DegerYap(f.esCocukOlumKatsayi) + ")";


                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";
                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AILEYARDIMGOST], f.aileYardimGost) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.COCUKYARDIMGOST0_6], f.cocukYardimGost0_6) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.COCUKYARDIMGOST6_X], f.cocukYardimGost6_X) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOGUMYARDIMGOST], f.dogumYardimGost) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEMUROLUMKATSAYI], f.memurOlumKatsayi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ESCOCUKOLUMKATSAYI], f.esCocukOlumKatsayi);

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
        /// Sosyal Yardım Temel Tanım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak sosyal yardım temel tanım bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sosyal Yardım Temel Tanım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(PSLSosyalYardimTemelBilgisi bilgi)
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

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + "," + ALANLAR[(int)ENUMAlanlar.AY];

            return veriErisim.DataReaderDondur(sql);
        }


    }

}
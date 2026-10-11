using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Tanımlama veri tabanı işlemleri ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABTamimlama : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABTamimlama sınıfının yeni bir örneğini oluşturur.
        /// </summary>        
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        /// <param name="tabloAd">Veri tabanında işlem yapılacak tablo adını içeren nesne</param>
        public TABTamimlama(VeriErisim veriErisim, string tabloAd) : base(veriErisim) { TABLOAD = tabloAd; }

        /// <summary>
        /// TABTamimlama sınıfına ait, tablo adını tutan değişken
        /// </summary>
        private string TABLOAD = "";
        private string TABLOADGIZLILIK = "EVRGIZLILIK";

        /// <summary>
        /// TABTamimlama sınıfına ait, tanımlama işlem bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        private enum ENUMAlanlar { KOD = 0, AD = 1 }
        private enum ENUMAlanlarGizlilik { KOD = 0, AD = 1, DERECE=2 }

        /// <summary>
        /// TABTamimlama sınıfına ait, tanımlama işlem bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        private string[] ALANLAR = { "KOD", "AD" };
        private string[] ALANLARGIZLILIK = { "KOD", "AD", "DERECE" };

        /// <summary>
        /// TABTamimlama sınıfına ait, sql cümlesini tutan değişken
        /// </summary>
        private string sql = "";

        /// <summary>
        /// Parametre olarak verilen tanimlama nesne koleksiyonuna göre; tanımlama bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            foreach (Tanimlama form in liste.objeler)
            {
                if (form.kod == 0)
                {
                    form.kod = KodAl();

                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql = sql + VeriErisim.DegerYap(form.kod) + ", ";
                    sql = sql + VeriErisim.DegerYap(form.ad) + ")";
                }
                else
                {
                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod);
                }

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc) { break; }
            }
            return sonuc;
        }

        public Sonuc KaydetGizlilik(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            foreach (Tanimlama form in liste.objeler)
            {
                if (form.kod == 0)
                {
                    form.kod = KodAlGizlilik();

                    sql = "INSERT INTO " + TABLOADGIZLILIK + " (" + VeriErisim.TumAlanlar("", ALANLARGIZLILIK) + ") VALUES (";
                    sql = sql + VeriErisim.DegerYap(form.kod) + ", ";
                    sql = sql + VeriErisim.DegerYap(form.ad) + ",";
                    sql = sql + VeriErisim.DegerYap(form.derece) + ")";
                }
                else
                {
                    sql = "UPDATE " + TABLOADGIZLILIK + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLARGIZLILIK[(int)ENUMAlanlarGizlilik.AD], form.ad) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLARGIZLILIK[(int)ENUMAlanlarGizlilik.DERECE], form.derece);
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarGizlilik.KOD], "=", form.kod);
                }

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc) { break; }
            }
            return sonuc;
        }


        /// <summary>
        /// Parametre olarak verilen tanimlama nesne koleksiyonuna göre; tanımlama bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Tanimlama bilgi in liste.objeler)
            {
                sql = "DELETE FROM " + TABLOAD + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        public Sonuc SilGizlilik(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Tanimlama bilgi in liste.objeler)
            {
                sql = "DELETE FROM " + TABLOADGIZLILIK + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLARGIZLILIK[(int)ENUMAlanlarGizlilik.KOD], "=", bilgi.kod);

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen tanimlama nesnesine göre; tanımlama bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Tanımlama bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(Tanimlama bilgi)
        {
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.kod > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.ad != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + bilgi.ad + "%"));

            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleGizlilik(Tanimlama bilgi)
        {
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGIZLILIK, ALANLARGIZLILIK) + " FROM " + TABLOADGIZLILIK;
            if (bilgi.kod > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOADGIZLILIK, ALANLARGIZLILIK[(int)ENUMAlanlarGizlilik.KOD], "=", bilgi.kod));

            if (bilgi.ad != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOADGIZLILIK, ALANLARGIZLILIK[(int)ENUMAlanlarGizlilik.AD], " LIKE ", "%" + bilgi.ad + "%"));

            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Veritabanına yeni tanımlama bilgisi kaydetmek için, sql cümlesi hazırlayıp çalıştırarak yeni bir tanımlama kodu oluşturan yordam.
        /// </summary>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        private int KodAl()
        {
            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        private int KodAlGizlilik()
        {
            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlarGizlilik.KOD] + ") FROM " + TABLOADGIZLILIK;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}

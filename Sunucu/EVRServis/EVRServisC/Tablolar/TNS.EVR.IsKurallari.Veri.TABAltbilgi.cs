using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Alt bilgi bilgilerini içeren EVRALTBILGI veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABAltbilgi : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABAltbilgi sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABAltbilgi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Alt bilgi bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRALTBILGI";

        /// <summary>
        /// Alt bilgi bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            KURUMADRES = 2,
            AYRINTILIBILGI = 3,
            TELEFON = 4,
            FAKS = 5,
            EPOSTA = 6,
            ELEKTRONIKAG = 7
        }

        /// <summary>
        /// Alt bilgi bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KULLANICIKODU", "ALTBILGI", "KURUMADRES", "AYRINTILIBILGI", "TELEFON", "FAKS", "EPOSTA", "ELEKTRONIKAG" };

        /// <summary>
        /// Parametre olarak verilen Altbilgi nesne koleksiyonuna göre, kullanıcının evrak altbilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Altbilgi bilgi in liste.objeler)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.kullaniciKod) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.altbilgi) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kurumAdres) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.ayrintiliBilgi) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.telefon) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.faks) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.ePosta) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.elektronikAg) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.altbilgi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMADRES], bilgi.kurumAdres) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AYRINTILIBILGI], bilgi.ayrintiliBilgi) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], bilgi.telefon) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], bilgi.faks) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], bilgi.ePosta) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ELEKTRONIKAG], bilgi.elektronikAg);
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kullaniciKod);

                    sonuc = KomutCalistir(sql, 1);
                }

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Altbilgi nesne koleksiyonuna göre, kullanıcının evrak altbilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Altbilgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Altbilgi bilgi in liste.objeler)
            {
                string sql = "";

                sql = "DELETE FROM " + TABLOAD + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kullaniciKod);

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen Altbilgi nesnesine göre, kullanıcının evrak altbilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Altbilgi bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(Altbilgi bilgi)
        {
            string sql = "";
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.kullaniciKod != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kullaniciKod));

            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

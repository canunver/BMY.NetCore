using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Personel bilgilerini tutan TASPERSONEL veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABPersonel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPersonel(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASPERSONEL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 1,
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 2,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 3,
            /// <summary>
            /// Ünvan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UNVAN = 4,
            /// <summary>
            /// Görev alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREV = 5,
            /// <summary>
            /// Telefon alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TELEFON = 6,
            /// <summary>
            /// Resim alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            RESIM = 7,
            /// <summary>
            /// Oda alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODA = 8,
            /// <summary>
            /// ePosta alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EPOSTA = 9,
            /// <summary>
            /// ceptel alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEPTEL = 10,
            /// <summary>
            /// faks alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAKS = 11
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "KOD", "AD", "UNVAN", "GOREV", "TELEFON", "RESIM", "ODA", "EPOSTA", "CEPTEL", "FAKS" };

        /// <summary>
        /// Personel bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="personel">Personel bilgisini tutan, Personel türünde nesne</param>
        /// <returns>Ekleme, Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(Personel personel)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";

            //string sql = "SELECT MUHASEBEKOD, HARCAMABIRIMKOD, KOD FROM " + TABLOAD;
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", personel.harcamaBirimKod.Replace(".","")));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", personel.kod));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", personel.muhasebeKod));

            //IDataReader dr = veriErisim.DataReaderDondur(sql);

            //bool sonuc = dr.Read();
            //dr.Close();

            //if (sonuc) //Kayıt var, güncelle
            //{

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], personel.muhasebeKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], personel.harcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], personel.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVAN], personel.unvan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GOREV], personel.gorev) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], personel.telefon) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RESIM], personel.resim) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODA], personel.oda) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], personel.eposta) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEPTEL], personel.cepTel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], personel.faks);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", personel.kod));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", personel.harcamaBirimKod.Replace(".", "")));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", personel.muhasebeKod));

            sonuc = KomutCalistir(sql, 1);
            //}
            //else //Kayıt yok, ekle
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(personel.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(personel.harcamaBirimKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(personel.kod) + ",";
                sql += VeriErisim.DegerYap(personel.ad) + ",";
                sql += VeriErisim.DegerYap(personel.unvan) + ",";
                sql += VeriErisim.DegerYap(personel.gorev) + ",";
                sql += VeriErisim.DegerYap(personel.telefon) + ",";
                sql += VeriErisim.DegerYap(personel.resim) + ",";
                sql += VeriErisim.DegerYap(personel.oda) + ",";
                sql += VeriErisim.DegerYap(personel.eposta) + ",";
                sql += VeriErisim.DegerYap(personel.cepTel) + ",";
                sql += VeriErisim.DegerYap(personel.faks) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Personel bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Personel bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="personel">Personel bilgisini tutan, Personel türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Personel bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(Personel personel)
        {

            //string sql = "SELECT TASIL.KOD ILKOD, TASILCE.KOD ILCEKOD, TASPERSONEL.MUHASEBEKOD, TASPERSONEL.HARCAMABIRIMKOD, TASPERSONEL.KOD, TASPERSONEL.AD, TASPERSONEL.UNVAN, TASPERSONEL.GOREV, TASPERSONEL.TELEFON, TASPERSONEL.RESIM, TASPERSONEL.ODA,";
            //sql += " TASHARCAMABIRIM.AD HARCAMABIRIMAD, TASMUHASEBE.AD MUHASEBEAD, TASIL.AD ILAD, ";
            //sql += " TASILCE.AD ILCEAD FROM TASPERSONEL INNER JOIN";
            //sql += " TASHARCAMABIRIM ON TASPERSONEL.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD AND ";
            //sql += " TASPERSONEL.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD INNER JOIN ";
            //sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD INNER JOIN";
            //sql += " TASILCE ON TASMUHASEBE.ILKOD = TASILCE.ILKOD AND TASMUHASEBE.ILCEKOD = TASILCE.KOD INNER JOIN";
            //sql += " TASIL ON TASILCE.ILKOD = TASIL.KOD";

            string sql = "SELECT TASIL.KOD ILKOD, TASILCE.KOD ILCEKOD, TASPERSONEL.MUHASEBEKOD, TASPERSONEL.HARCAMABIRIMKOD, TASPERSONEL.KOD, TASPERSONEL.AD, TASPERSONEL.UNVAN, TASPERSONEL.GOREV, TASPERSONEL.TELEFON, TASPERSONEL.RESIM, TASPERSONEL.ODA, TASPERSONEL.EPOSTA, TASPERSONEL.CEPTEL, TASPERSONEL.FAKS, ";
            sql += " TASHARCAMABIRIM.AD, TASMUHASEBE.AD MUHASEBEAD, TASIL.AD ILAD, ";
            sql += " TASILCE.AD ILCEAD FROM TASILCE INNER JOIN ";
            sql += " TASMUHASEBE ON TASILCE.ILKOD = TASMUHASEBE.ILKOD AND TASILCE.KOD = TASMUHASEBE.ILCEKOD INNER JOIN ";
            sql += " TASIL ON TASILCE.ILKOD = TASIL.KOD INNER JOIN ";
            sql += " TASPERSONEL ON TASMUHASEBE.KOD = TASPERSONEL.MUHASEBEKOD LEFT OUTER JOIN ";
            sql += " TASHARCAMABIRIM ON TASPERSONEL.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD ";

            string kosul = "";

            if (personel != null)
            {
                if (personel.muhasebeKod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", personel.muhasebeKod));

                if (personel.harcamaBirimKod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "LIKE", personel.harcamaBirimKod.Replace(".", "") + "%"));

                if (personel.kod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", personel.kod));

                if (personel.ad != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + personel.ad + "%"));

                if (personel.oda != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODA], "LIKE", "%" + personel.oda + "%"));

                if (kosul != "")
                    sql += " WHERE " + kosul;
            }
            sql += " ORDER BY 6";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Depo Durum bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Personel Sayılarını döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="dp">Depo durum bilgisi, DepoDurum türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Personel Sayılarını tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader PersonelSayilari(DepoDurum dp)
        {
            string sql = "SELECT ";

            string sqlRaporTur = string.Empty;
            if (dp.raporTur == (int)ENUMDepoDurumRaporTur.MUHASEBE)
                sqlRaporTur += "TASPERSONEL.MUHASEBEKOD";
            else if (dp.raporTur == (int)ENUMDepoDurumRaporTur.HARCAMA)
            {
                sqlRaporTur += "TASPERSONEL.MUHASEBEKOD";
                sqlRaporTur += ", TASPERSONEL.HARCAMABIRIMKOD";
            }
            else if (dp.raporTur == (int)ENUMDepoDurumRaporTur.IL)
                sqlRaporTur += "TASMUHASEBE.ILKOD";

            sql += sqlRaporTur;
            sql += ", COUNT(TASPERSONEL.KOD)";
            sql += " FROM TASPERSONEL";

            if (dp.raporTur == (int)ENUMDepoDurumRaporTur.IL)
            {
                sql += " INNER JOIN TASHARCAMABIRIM ON TASPERSONEL.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD";
                sql += " AND TASPERSONEL.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD";
                sql += " INNER JOIN TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD";
            }

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(dp.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASPERSONEL", "MUHASEBEKOD", "=", dp.muhasebeKod));

            if (!string.IsNullOrEmpty(dp.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASPERSONEL", "HARCAMABIRIMKOD", "=", dp.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(dp.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASMUHASEBE", "ILKOD", "=", dp.ilKod));

            if (!string.IsNullOrEmpty(dp.ilceKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASMUHASEBE", "ILCEKOD", "=", dp.ilceKod));

            sql += " GROUP BY " + sqlRaporTur;
            sql += " ORDER BY " + sqlRaporTur;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Personel bilgisini kullanarak
        /// Personel bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="personel">Personel bilgisini tutan, Personel türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(Personel personel)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", personel.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", personel.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", personel.muhasebeKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen T.C. Kimlik numarası bilgisini kullanarak,
        /// Personel bilgisinin kayıtlı olup olmadığını,
        /// KayıtVarMı rutinini çalıştırarak bulan ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="tcKimlikNo">T.C. Kimlik numarası bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, kayıt var ise true yoksa false değerini veren Bool türünde nesne döndürür</returns>
        public bool PersonelVarMi(string tcKimlikNo)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.KOD] };
            string[] strDeger = { tcKimlikNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, new string[0], new int[0]);
        }
    }
}

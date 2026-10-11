using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Oda bilgilerini tutan TASODA veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABOda : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOda(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASODA";

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
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "KOD", "AD", "AMBARKOD" };

        /// <summary>
        /// Oda bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="oda">Oda bilgilerini tutan, Oda türünde nesne</param>
        /// <returns>Ekleme,Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(Oda oda)
        {
            string sql = "SELECT MUHASEBEKOD, HARCAMABIRIMKOD, KOD FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", oda.harcamaBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", oda.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", oda.muhasebeKod));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], oda.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], oda.ambarKod);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", oda.harcamaBirimKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", oda.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", oda.muhasebeKod));
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(oda.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(oda.harcamaBirimKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(oda.kod) + ",";
                sql += VeriErisim.DegerYap(oda.ad) + ",";
                sql += VeriErisim.DegerYap(oda.ambarKod) + ")";
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Oda bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Oda bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="oda">Oda bilgisini tutan, Oda türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Oda bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(Oda oda, bool merkezBankasi)
        {
            string sql = "SELECT TOP(500) TASIL.KOD ILKOD, TASILCE.KOD ILCEKOD, TASODA.MUHASEBEKOD, TASODA.HARCAMABIRIMKOD, TASODA.KOD, TASODA.AD, ";
            sql += " TASHARCAMABIRIM.AD HARCAMABIRIMAD, TASMUHASEBE.AD MUHASEBEAD, TASIL.AD ILAD, ";
            sql += " TASILCE.AD ILCEAD, TASODA.AMBARKOD AMBARKOD, TASODA.UKOD ";
            if (merkezBankasi)
                sql += ", TASODA.SIRANO, TASODA.KOD2 ";
            sql += " FROM TASODA LEFT OUTER JOIN";
            sql += " TASHARCAMABIRIM ON TASODA.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD AND ";
            sql += " TASODA.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD LEFT OUTER JOIN ";
            sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD LEFT OUTER JOIN";
            sql += " TASILCE ON TASMUHASEBE.ILKOD = TASILCE.ILKOD AND TASMUHASEBE.ILCEKOD = TASILCE.KOD LEFT OUTER JOIN";
            sql += " TASIL ON TASILCE.ILKOD = TASIL.KOD";
            string kosul = "";

            if (oda != null)
            {
                if (oda.muhasebeKod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", oda.muhasebeKod));

                if (oda.harcamaBirimKod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "LIKE", oda.harcamaBirimKod.Replace(".", "") + "%"));

                if (oda.ambarKod != string.Empty)
                {
                    string odaKosul = "";
                    odaKosul = VeriErisim.KosulEkle(odaKosul, "", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", oda.ambarKod));
                    odaKosul = VeriErisim.KosulEkle(odaKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", ""));
                    odaKosul += " OR " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMBARKOD] + " IS NULL";

                    kosul += " AND (" + odaKosul + ")";
                }

                if (oda.kod != string.Empty)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", oda.kod));

                if (oda.ad != string.Empty)
                {
                    string icKosul = "";
                    string[] bilgiler = oda.ad.Split(' ');
                    foreach (string oa in bilgiler)
                    {
                        if (oa.Trim() == "") continue;

                        string icKosul2 = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + oa + "%"));
                        icKosul2 = VeriErisim.KosulEkle(icKosul2, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", "%" + oa + "%"));
                        if (merkezBankasi)
                            icKosul2 = VeriErisim.KosulEkle(icKosul2, "OR", VeriErisim.KosulOl(TABLOAD, "KOD2", "LIKE", "%" + oa + "%"));
                        icKosul2 = "(" + icKosul2 + ")";

                        icKosul = VeriErisim.KosulEkle(icKosul, "AND", icKosul2);

                    }
                    if (icKosul != "") icKosul = "(" + icKosul + ")";
                    kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
                }


                string tip = veriErisim.TekKolonSQLStr("SELECT type_desc FROM sys.objects WHERE name = 'TASODA'");

                if (tip == "VIEW")
                {
                    if (!string.IsNullOrWhiteSpace(oda.kod2) && merkezBankasi)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "KOD2", "=", oda.kod2));

                    if (oda.seviye > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "SEVIYE", "=", oda.seviye));

                    if (oda.ustKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "USTKOD", "=", oda.ustKod));

                    if (oda.durum > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, "DURUM", "=", oda.durum));
                }

                if (kosul != "")
                    sql += " WHERE " + kosul;
            }

            if (merkezBankasi)
                sql += " ORDER BY TASODA.HARCAMABIRIMKOD, TASODA.SIRANO, TASODA.AD";
            else
                sql += " ORDER BY 1,2,3,4,5,6,7";

            //OrtakFonksiyonlar.HataDosyaYaz("Oda", "merkezBankasi:" + (merkezBankasi ? 1 : 0) + "-" + sql, false);

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Oda bilgisini kullanarak
        /// Oda bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="oda">Oda bilgisini tutan, Oda türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(Oda oda)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", oda.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", oda.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", oda.muhasebeKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Oda bilgilerini kullanarak,
        /// Oda bilgilerinin kayıtlı olup olmadığını dönen,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="oda">Oda bilgisini tutan, Oda türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Oda bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader VarMi(Oda oda)
        {
            string sql = "SELECT * FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", oda.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", oda.muhasebeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Oda bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Oda bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Oda bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ListeleSilinenler()
        {
            string sql = @"SELECT A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.NEREYEGITTI FROM TASSICILNOHAREKET AS A
            INNER JOIN TASSICILNOHAREKET AS H1 ON H1.PRSICILNO = A.PRSICILNO AND H1.SIRANO = (SELECT MIN(SIRANO) FROM TASSICILNOHAREKET B   WHERE A.PRSICILNO = B.PRSICILNO AND ISLEMTURUKOD < 50)
            LEFT OUTER JOIN TASODA ON A.NEREYEGITTI = dbo.TASODA.KOD AND A.HARCAMABIRIMKOD = dbo.TASODA.HARCAMABIRIMKOD AND A.MUHASEBEKOD = dbo.TASODA.MUHASEBEKOD
            WHERE(A.DURUM <> 2) AND(A.PRSICILNO NOT IN  (SELECT PRSICILNO FROM dbo.TASSICILNOHAREKET AS B WHERE(A.PRSICILNO = PRSICILNO) AND(A.SIRANO < SIRANO) AND(ISLEMTURUKOD = 46 OR ISLEMTURUKOD = 47))) AND(A.ISLEMTURUKOD = '90' OR A.ISLEMTURUKOD = '91')
            AND A.NEREYEGITTI <> ''
            AND TASODA.KOD IS NULL
            GROUP BY A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.NEREYEGITTI
            ORDER BY A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.NEREYEGITTI";

            return veriErisim.DataReaderDondur(sql);
        }

        public int KayitVarMi(Oda oda)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", oda.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", oda.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", oda.harcamaBirimKod.Replace(".", "")));

            return veriErisim.TekKolonSQLInt(sql);
        }


    }
}

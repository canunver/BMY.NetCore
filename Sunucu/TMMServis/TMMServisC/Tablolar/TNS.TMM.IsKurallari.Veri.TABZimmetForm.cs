using System;
using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Zimmet Form bilgilerini tutan TASZIMMETFORM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABZimmetForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABZimmetForm(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASZIMMETFORM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 4,
            /// <summary>
            /// Fiş tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISTARIH = 5,
            /// <summary>
            /// Tıp alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIP = 6,
            /// <summary>
            /// Kime gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KIMEGITTI = 7,
            /// <summary>
            /// Nereye gitti alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NEREYEGITTI = 8,
            /// <summary>
            /// Verme düşme alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            VERMEDUSME = 9,
            /// <summary>
            /// Düşme tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DUSMETARIH = 10,
            /// <summary>
            /// İşlem yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 11,
            /// <summary>
            /// İşlem tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 12,
            /// <summary>
            /// Belge türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 13,
            /// <summary>
            /// Durum alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 14
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "AMBARKOD", "FISTARIH", "TIP", "KIMEGITTI", "NEREYEGITTI", "VERMEDUSME", "DUSMETARIH", "ISLEMYAPAN", "ISLEMTARIH", "BELGETUR", "DURUM" };

        /// <summary>
        /// Zimmet Form bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="zimmet">Zimmet Form bilgilerini tutan, ZimmetForm türünde nesne</param>
        /// <returns>Ekleme, Ekleme komutunu çalıştıramazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(ZimmetForm zimmet)
        {
            string sql = "SELECT * FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmet.fisNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmet.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmet.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmet.belgeTur));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], zimmet.ambarKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISTARIH], zimmet.fisTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], zimmet.tip) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMEGITTI], zimmet.kimeGitti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREYEGITTI], zimmet.nereyeGitti) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERMEDUSME], zimmet.vermeDusme) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DUSMETARIH], zimmet.dusmeTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], zimmet.islemYapan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], zimmet.islemTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], zimmet.durum);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmet.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmet.fisNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmet.harcamaBirimKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmet.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmet.belgeTur));

                KaydetTarihce(zimmet.yil, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.fisNo, zimmet.belgeTur, zimmet.islemYapan, ZimmetForm.FormDurumAciklama((int)ENUMBelgeDurumu.DEGISTIRILDI), zimmet.ipAdresi);
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(zimmet.yil) + ",";
                sql += VeriErisim.DegerYap(zimmet.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(zimmet.harcamaBirimKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(zimmet.fisNo) + ",";
                sql += VeriErisim.DegerYap(zimmet.ambarKod) + ",";
                sql += VeriErisim.DegerYap(zimmet.fisTarih) + ",";
                sql += VeriErisim.DegerYap(zimmet.tip) + ",";
                sql += VeriErisim.DegerYap(zimmet.kimeGitti) + ",";
                sql += VeriErisim.DegerYap(zimmet.nereyeGitti) + ",";
                sql += VeriErisim.DegerYap(zimmet.vermeDusme) + ",";
                sql += VeriErisim.DegerYap(zimmet.dusmeTarih) + ",";
                sql += VeriErisim.DegerYap(zimmet.islemYapan) + ",";
                sql += VeriErisim.DegerYap(zimmet.islemTarih) + ",";
                sql += VeriErisim.DegerYap(zimmet.belgeTur) + ",";
                sql += VeriErisim.DegerYap(zimmet.durum) + ")";

                KaydetTarihce(zimmet.yil, zimmet.muhasebeKod, zimmet.harcamaBirimKod, zimmet.fisNo, zimmet.belgeTur, zimmet.islemYapan, ZimmetForm.FormDurumAciklama((int)ENUMBelgeDurumu.YENI), zimmet.ipAdresi);
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Form bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun bilgileri döndüren
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="zimmet">Zimmet Form bilgilerini tutan, ZimmetForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Zimmet bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader FisAc(ZimmetForm zimmet)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASIL.KOD, TASIL.AD, TASILCE.KOD, TASILCE.AD, TASHARCAMABIRIM.AD, TASPERSONEL.AD FROM " + TABLOAD;

            sql += " JOIN TASMUHASEBE ON TASMUHASEBE.KOD = TASZIMMETFORM.MUHASEBEKOD";
            sql += " JOIN TASHARCAMABIRIM ON TASHARCAMABIRIM.KOD = TASZIMMETFORM.HARCAMABIRIMKOD AND TASHARCAMABIRIM.MUHASEBEKOD = TASZIMMETFORM.MUHASEBEKOD";
            sql += " JOIN TASILCE ON TASILCE.ILKOD = TASMUHASEBE.ILKOD AND TASILCE.KOD = TASMUHASEBE.ILCEKOD";
            sql += " JOIN TASIL ON TASIL.KOD=TASILCE.ILKOD";
            sql += " LEFT OUTER JOIN TASPERSONEL ON TASZIMMETFORM.KIMEGITTI = TASPERSONEL.KOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmet.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmet.harcamaBirimKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmet.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmet.belgeTur));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Form ve Zimmet Form Kriter bilgisine göre,
        /// Zimmet Formları bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="zimmet">Zimmet Form bilgisini tutan, ZimmetForm türünde nesne</param>
        /// <param name="kriter">Zimmet Form Kriter bilgisini tutan, ZimmetFormKriter türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, Zimmet Form bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(ZimmetForm zimmet, ZimmetFormKriter kriter, string eOnayDurum)
        {
            string secAlanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            string sql = "SELECT " + secAlanlar + " FROM " + TABLOAD;
            if (kriter.sicilNo != string.Empty)
            {
                sql += " JOIN TASZIMMETDETAY ON TASZIMMETFORM.FISNO=TASZIMMETDETAY.FISNO AND TASZIMMETFORM.YIL=TASZIMMETDETAY.YIL AND TASZIMMETFORM.MUHASEBEKOD=TASZIMMETDETAY.MUHASEBEKOD AND TASZIMMETFORM.HARCAMABIRIMKOD=TASZIMMETDETAY.HARCAMABIRIMKOD AND TASZIMMETFORM.BELGETUR=TASZIMMETDETAY.BELGETUR";
            }

            if (!(string.IsNullOrEmpty(eOnayDurum)))
            {
                sql += " LEFT JOIN TASZIMMETFORMEONAY ON TASZIMMETFORM.FISNO=TASZIMMETFORMEONAY.FISNO AND TASZIMMETFORM.YIL=TASZIMMETFORMEONAY.YIL AND TASZIMMETFORM.MUHASEBEKOD=TASZIMMETFORMEONAY.MUHASEBEKOD AND TASZIMMETFORM.HARCAMABIRIMKOD=TASZIMMETFORMEONAY.HARCAMABIRIMKOD AND TASZIMMETFORM.BELGETUR=TASZIMMETFORMEONAY.BELGETUR";
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmet.yil));

            if (zimmet.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmet.muhasebeKod));

            if (zimmet.vermeDusme > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VERMEDUSME], "=", zimmet.vermeDusme));
            if (zimmet.tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", zimmet.tip));
            if (zimmet.harcamaBirimKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmet.harcamaBirimKod.Replace(".", "")));
            if (zimmet.ambarKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", zimmet.ambarKod));
            if (zimmet.belgeTur != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmet.belgeTur));
            if (!kriter.belgeTarihBasla.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISTARIH], ">=", kriter.belgeTarihBasla));
            if (!kriter.belgeTarihBit.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISTARIH], "<=", kriter.belgeTarihBit));
            if (kriter.belgeNoBasla != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], ">=", kriter.belgeNoBasla));
            if (kriter.belgeNoBit != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "<=", kriter.belgeNoBit));
            if (kriter.islemYapan != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", kriter.islemYapan));
            if (kriter.kime != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMEGITTI], "=", kriter.kime));
            if (kriter.nereye != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NEREYEGITTI], "=", kriter.nereye));
            if (kriter.durum > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", kriter.durum));
                if (kriter.durum == 1) //Yeni ve değişen belgeleri getir.
                    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI));
                sql += ")";
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));
            if (kriter.sicilNo != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASZIMMETDETAY", "HESAPPLANKOD", "LIKE", kriter.sicilNo.Replace(".", "") + "%"));

            if (!(string.IsNullOrEmpty(eOnayDurum)))
            {
                if (eOnayDurum.Equals("0"))
                    sql += " AND TASZIMMETFORMEONAY.DURUM IS NULL ";
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASZIMMETFORMEONAY", "DURUM", "=", eOnayDurum));
            }

            sql += " GROUP BY " + secAlanlar;

            sql += " ORDER BY 1,2,3,4";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Form ve Zimmet Form Kriter bilgisine göre,
        /// Zimmet Formları bilgisini EOnay bilgileri ile beraber döndüren fonksiyon
        /// </summary>
        /// <param name="zimmet">Zimmet Form bilgisini tutan, ZimmetForm türünde nesne</param>
        /// <param name="kriter">Zimmet Form Kriter bilgisini tutan, ZimmetFormKriter türünde nesne</param>
        /// <param name="eonaykriter"> Sorgulama için eonay kriter bilgilerini tutan nesne</param> 
        /// <returns>Select komutunun çalışması sonucunda, Zimmet Form bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ListeleEOnayli(ZimmetForm zimmet, ZimmetFormKriter kriter, ZimmetFormEOnayKriter eonaykriter)
        {
            string secAlanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR);

            string sql = "SELECT " + secAlanlar + ", TASZIMMETFORMEONAY.EMAILKIMDEN, TASZIMMETFORMEONAY.EMAILKIMDENAD, TASZIMMETFORMEONAY.EMAILKIME, TASZIMMETFORMEONAY.EMAILKIMEAD, TASZIMMETFORMEONAY.EMAILGONDERMETARIH, TASZIMMETFORMEONAY.EMAILCEVAPTARIH, TASZIMMETFORMEONAY.DURUM" + " FROM " + TABLOAD;
            if (kriter.sicilNo != string.Empty)
            {
                sql += " JOIN TASZIMMETDETAY ON TASZIMMETFORM.FISNO=TASZIMMETDETAY.FISNO AND TASZIMMETFORM.YIL=TASZIMMETDETAY.YIL AND TASZIMMETFORM.MUHASEBEKOD=TASZIMMETDETAY.MUHASEBEKOD AND TASZIMMETFORM.HARCAMABIRIMKOD=TASZIMMETDETAY.HARCAMABIRIMKOD AND TASZIMMETFORM.BELGETUR=TASZIMMETDETAY.BELGETUR";
            }
            sql += " LEFT JOIN TASZIMMETFORMEONAY ON TASZIMMETFORM.FISNO=TASZIMMETFORMEONAY.FISNO AND TASZIMMETFORM.YIL=TASZIMMETFORMEONAY.YIL AND TASZIMMETFORM.MUHASEBEKOD=TASZIMMETFORMEONAY.MUHASEBEKOD AND TASZIMMETFORM.HARCAMABIRIMKOD=TASZIMMETFORMEONAY.HARCAMABIRIMKOD AND TASZIMMETFORM.BELGETUR=TASZIMMETFORMEONAY.BELGETUR";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmet.yil));

            if (zimmet.muhasebeKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmet.muhasebeKod));

            if (zimmet.vermeDusme > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VERMEDUSME], "=", zimmet.vermeDusme));
            if (zimmet.tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", zimmet.tip));
            if (zimmet.harcamaBirimKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmet.harcamaBirimKod.Replace(".", "")));
            if (zimmet.ambarKod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", zimmet.ambarKod));
            if (zimmet.belgeTur != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmet.belgeTur));
            if (!kriter.belgeTarihBasla.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISTARIH], ">=", kriter.belgeTarihBasla));
            if (!kriter.belgeTarihBit.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISTARIH], "<=", kriter.belgeTarihBit));
            if (kriter.belgeNoBasla != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], ">=", kriter.belgeNoBasla));
            if (kriter.belgeNoBit != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "<=", kriter.belgeNoBit));
            if (kriter.islemYapan != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", kriter.islemYapan));
            if (kriter.kime != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KIMEGITTI], "=", kriter.kime));
            if (kriter.nereye != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NEREYEGITTI], "=", kriter.nereye));
            if (kriter.durum > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", kriter.durum));
                if (kriter.durum == 1) //Yeni ve değişen belgeleri getir.
                    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBelgeDurumu.DEGISTIRILDI));
                sql += ")";
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMBelgeDurumu.IPTAL));
            if (kriter.sicilNo != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASZIMMETDETAY", "HESAPPLANKOD", "LIKE", kriter.sicilNo.Replace(".", "") + "%"));


            if (eonaykriter.durum == 0)
                //sql += " AND TASZIMMETFORMEONAY.DURUM IS NULL ";
                sql += " ";
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASZIMMETFORMEONAY", "DURUM", "=", eonaykriter.durum));

            sql += " GROUP BY " + secAlanlar + ", TASZIMMETFORMEONAY.EMAILKIMDEN, TASZIMMETFORMEONAY.EMAILKIMDENAD, TASZIMMETFORMEONAY.EMAILKIME, TASZIMMETFORMEONAY.EMAILKIMEAD, TASZIMMETFORMEONAY.EMAILGONDERMETARIH, TASZIMMETFORMEONAY.EMAILCEVAPTARIH, TASZIMMETFORMEONAY.DURUM ";

            sql += " ORDER BY 1,2,3,4";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe Kodu, Harcama Birim Kodu, Fiş No, Belge Türü, İşlem Yapan ve İşlem bilgilerine göre,
        /// Zimmet Formunun tarihçesini kaydeden fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebeKod">Muhasebe kodunu tutan, String türünde nesne</param>
        /// <param name="harcamaBirimKod">Harcama Birim kodunu tutan, String türünde nesne</param>
        /// <param name="fisNo">Fiş no bilgisini tutan, String türünde nesne</param>
        /// <param name="belgeTur">Belge Tür bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemYapan">İşlem Yapan bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        public void KaydetTarihce(int yil, string muhasebeKod, string harcamaBirimKod, string fisNo, int belgeTur, string islemYapan, string islem, string ipAdresi)
        {
            string sql = "";

            sql = "INSERT INTO TASZIMMETFORMTARIHCE (YIL,MUHASEBEKOD,HARCAMABIRIMKOD,FISNO,BELGETUR,ISLEMYAPAN,ISLEM,ISLEMTARIH,IPADRESI) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(harcamaBirimKod.Trim().Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fisNo.Trim()) + ", ";
            sql = sql + VeriErisim.DegerYap(belgeTur) + ", ";
            sql = sql + VeriErisim.DegerYap(islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ", ";
            sql = sql + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ", ";
            sql = sql + VeriErisim.DegerYap(ipAdresi) + ") ";

            KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Form bilgisine göre,
        /// Belge durum bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="zimmet">Zimmet Form bilgisini tutan, ZimmetForm türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kıstaslara uygun zimmet durumları bilgisini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BelgeDurumGetir(ZimmetForm zimmet)
        {
            string sql = "SELECT TASZIMMETFORM.DURUM FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmet.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmet.harcamaBirimKod.Trim().Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmet.muhasebeKod.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmet.belgeTur));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Programatik Sicil Numarası bilgisine göre,
        /// Zimmet kaydının olup olmadığını dönen fonksiyon
        /// </summary>
        /// <param name="prSicilNo">Programatik Sicil Numarası bilgisini tutan, Int türünde nesne</param>
        /// <param name="gorSicilNo">Sicil Numarası bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterler uygun zimmet bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ZimmetVarMi(int prSicilNo, string gorSicilNo = "")
        {
            string sql = "SELECT TASAMBARZIMMETVIEW.PRSICILNO,TASAMBARZIMMETVIEW.YIL,TASAMBARZIMMETVIEW.MUHASEBEKOD,TASAMBARZIMMETVIEW.HARCAMABIRIMKOD,TASAMBARZIMMETVIEW.FISNO,TASAMBARZIMMETVIEW.ISLEMTURUKOD,TASAMBARZIMMETVIEW.KIMEGITTI,TASAMBARZIMMETVIEW.NEREYEGITTI,TASAMBARZIMMETVIEW.AMBARKOD,TASAMBARZIMMETVIEW.HESAPPLANKOD,TASAMBARZIMMETVIEW.KDVORAN,TASAMBARZIMMETVIEW.ZIMMETOZELLIK,TASAMBARZIMMETVIEW.FIYAT FROM TASAMBARZIMMETVIEW ";
            if (prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "PRSICILNO", "=", prSicilNo));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "SICILNO", "=", gorSicilNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Zimmet Form bilgisi ve Durum bilgisine göre,
        /// Zimmet formunun durumunu değiştiren fonksiyon
        /// </summary>
        /// <param name="zimmet">Zimmet bilgisini tutan, ZimmetForm türünde nesne</param>
        /// <param name="yeniDurum">Güncellenecek durum bilgisini tutan, String türünde nesne</param>
        /// <returns>Güncelleme komutunun çalışması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BelgeDurumDegistir(ZimmetForm zimmet, string yeniDurum)
        {
            int durum = TABGenel.FormDurumNoVer(yeniDurum);
            if (durum == (int)ENUMBelgeDurumu.ONAYKALDIR)
                durum = (int)ENUMBelgeDurumu.DEGISTIRILDI;

            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], zimmet.islemYapan);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", zimmet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", zimmet.fisNo.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", zimmet.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", zimmet.harcamaBirimKod.Trim().Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", zimmet.belgeTur));

            return KomutCalistir(sql, 1);
        }
    }
}
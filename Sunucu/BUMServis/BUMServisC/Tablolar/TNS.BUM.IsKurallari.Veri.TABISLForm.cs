using System.Data;
using TNS.Veri;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe işlem formu tablosuyla (Yapilan Butce islemleri ile ilgili bilgilerin tutuldugu tablo) 
    /// ilgili veri tabanı sorgularını gerçekleştiren sınıf.
    /// </summary>
    public class TABISLForm : TABOrtak
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABISLForm(VeriErisim veriErisim)
            : base(veriErisim)
        {

        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ISLFORM";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURUMKOD", "ISLEM", "SIRANO", "TARIH", "YAZINO", "ILGI", "NOTLAR", "FORMDURUM", "ILGILIISLEM", "ILGILISIRANO", "ISLEMYAPANKOD", "ISLEMYAPAN", "DEGISIKLIKTARIH", "ONAYTARIH", "BUMKOKURUMKOD", "BUMKOSIRANO" };

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
            /// Kurum kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 1,
            /// <summary>
            /// İşlem alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEM = 2,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,
            /// <summary>
            /// Tarih alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,
            /// <summary>
            /// Yazı no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAZINO = 5,
            /// <summary>
            /// İlgi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGI = 6,
            /// <summary>
            /// Notlar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            NOTLAR = 7,
            /// <summary>
            /// Form durumu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMDURUM = 8,
            /// <summary>
            /// İlgili işlem alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIISLEM = 9,
            /// <summary>
            /// İlgili sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILISIRANO = 10,
            /// <summary>
            /// İşlem yapan kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANKTKOD = 11,
            /// <summary>
            /// İşlem Yapan kullanıcı kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANKULKOD = 12,
            /// <summary>
            /// Değişiklik tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGISIKLIKTARIH = 13,
            /// <summary>
            /// Onay tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYTARIH = 14,
            /// <summary>
            /// BÜMKO kurum kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUMKOKURUMKOD = 15,
            /// <summary>
            /// BÜMKO sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUMKOSIRANO = 16
        }

        /// <summary>
        /// Parametre olarak verilen Bütçe İşlem Form bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Bütçe Form bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Bütçe İşlem Form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Sorgula(BIForm form)
        {
            string sql = "SELECT DISTINCT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "NOT LIKE", "0101%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "NOT LIKE", "0104%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "NOT LIKE", "0106%"));

            if (!string.IsNullOrEmpty(form.kurum))
                sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurum));

            if (!string.IsNullOrEmpty(form.islemKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", form.islemKod.Replace(".", "") + "%"));

            if (form.formDurumGrup != ENUMFormDurumGrup.HATALI)
                sql = GenelKosul.FormDurumKosulOl(sql, "AND", form.formDurumGrup, TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM]);

            if (!string.IsNullOrEmpty(form.bumkoKurum))
                sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BUMKOKURUMKOD], "=", form.bumkoKurum));

            if (form.bumkoSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BUMKOSIRANO], "=", form.bumkoSiraNo));

            if (form.giderler != null && form.giderler.Count >= 1 && form.giderler[0].GetType() == typeof(BISatir))
            {
                BISatir sat = (BISatir)form.giderler[0];
                if (sat.tertip.TertipStrOl() != "")
                {
                    string tertipKosul = "1 IN (SELECT 1 FROM " + TABISLGider.TABLOAD;
                    tertipKosul = VeriErisim.KosulEkle(tertipKosul, "WHERE", TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL]);
                    tertipKosul = VeriErisim.KosulEkle(tertipKosul, "AND", TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMKOD] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMKOD]);
                    tertipKosul = VeriErisim.KosulEkle(tertipKosul, "AND", TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM]);
                    tertipKosul = VeriErisim.KosulEkle(tertipKosul, "AND", TABISLGider.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO]);
                    tertipKosul = VeriErisim.KosulEkle(tertipKosul, "AND", GenelKosul.KistasKosulOl(-1, -1, "", sat.tertip.kodlar, false)) + ")";
                    sql = VeriErisim.KosulEkle(sql, "AND", tertipKosul);
                }
            }
            //if (form.gelirler != null && form.gelirler.Count >= 1 && form.gelirler[0].GetType() == typeof(BISatir))
            //{
            //    BISatir sat = (BISatir)form.gelirler[0];
            //    if (sat.tertip.TertipStrOl() != "")
            //    {
            //        string tertipKosul = "1 IN (SELECT 1 FROM " + TABISLGelir.TABLOAD;
            //        tertipKosul = VeriErisim.KosulEkle(tertipKosul, "WHERE", TABISLGelir.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL]);
            //        tertipKosul = VeriErisim.KosulEkle(tertipKosul, "AND", TABISLGelir.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMKOD] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMKOD]);
            //        tertipKosul = VeriErisim.KosulEkle(tertipKosul, "AND", TABISLGelir.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM]);
            //        tertipKosul = VeriErisim.KosulEkle(tertipKosul, "AND", TABISLGelir.TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO]);
            //        tertipKosul = VeriErisim.KosulEkle(tertipKosul, "AND", GenelKosul.KistasKosulOlFinansman(-1, "", sat.tertip.kodlar)) + ")";
            //        sql = VeriErisim.KosulEkle(sql, "AND", tertipKosul);
            //    }
            //}
            sql += " order by " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMKOD] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen bütçe işleminin, onay durumu döndürülür.
        /// </summary>
        /// <param name="yil">Form durumu değiştirilecek formun, yıl bilgisi.</param>
        /// <param name="kurum">Form durumu değiştirilecek formun, kurum bilgisi.</param>
        /// <param name="islemKod">Form durumu değiştirilecek formun, işlem kod bilgisi.</param>
        /// <param name="siraNo_Asama">Form durumu değiştirilecek formun, form bütçe işlemi ise sıra no bilgisi, işlem ahp veya ihp ise aşama bilgisi.</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Bütçe İşlem Form Durumu bilgisini tutan Int türünde nesne döndürür</returns>
        public int ButceIslemiOnayKoduBul(int yil, string kurum, string islemKod, int siraNo_Asama)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo_Asama));

            string sql = "SELECT FORMDURUM from " + TABLOAD + " where " + kosul;

            double tut = veriErisim.TekKolonSQLDbl(sql);
            return System.Convert.ToInt32(tut);
        }

        /// <summary>
        /// Verilen kayıt, insert SQL'ine çevrilir ve çalıştırılır.
        /// </summary>
        /// <param name="yil">Eklenecek işlemin yıl bilgisi.</param>
        /// <param name="kurum">Eklenecek işlemin kurum bilgisi.</param>
        /// <param name="islemKod">Eklenecek işlemin işlemkodu (serbest, ekleme) bilgisi.</param>
        /// <param name="siraNo">Eklenecek işlemin sıra no bilgisi.</param>
        /// <param name="yaziTarih">Eklenecek işlemin yazı tarihi.</param>
        /// <param name="yaziNo">Eklenecek işlemin yazı numarası.</param>
        /// <param name="ilgi">Eklenecek işlemin ilgisi.</param>
        /// <param name="dipNot">Eklenecek işlemin dip not bilgisi.</param>
        /// <param name="ilgiliIslemKod">İptal işlemi veya Doğru yanlış işlemi için; iptal edilen, yanlışlanan işlemin işlem kodu</param>
        /// <param name="ilgiliSiraNo">İptal işlemi veya doğru yanlış işlemi için; iptal edilen, yanlışlanan işlemin sıra numarası</param>
        /// <param name="islemYapanKT">İşlemi yapan kullanıcın kulllanıcı tipi</param>
        /// <param name="islemYapanKullaniciKod">İşlemi yapan kullanıcın kulllanıcı kodu</param>
        /// <param name="islemTarih">İşlemin yapıldığı tarih</param>
        /// <param name="bumkoKurum">BÜMKO Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="bumkoSiraNo">BÜMKO Sıra No bilgisini tutan, Int türünde nesne</param>
        /// <param name="formDurum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Yapılan işlemin sonucu, sonuç yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string kurum, string islemKod, int siraNo, TNSDateTime yaziTarih,
                           string yaziNo, string ilgi, string dipNot, int formDurum, string ilgiliIslemKod,
                           int ilgiliSiraNo, int islemYapanKT, string islemYapanKullaniciKod, TNSDateTime islemTarih, string bumkoKurum, int bumkoSiraNo)
        {
            if (islemKod.StartsWith("01.01"))
            {
                // ??? return new Sonuc(false, "AHP İşlemi bir süre için kapatılmıştır","");
            }
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(kurum.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(islemKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(siraNo) + ", ";
            sql += VeriErisim.DegerYap(yaziTarih) + ", ";
            sql += VeriErisim.DegerYap(yaziNo) + ", ";
            sql += VeriErisim.DegerYap(ilgi) + ", ";
            sql += VeriErisim.DegerYap(dipNot) + ", ";
            sql += VeriErisim.DegerYap(formDurum) + ", ";
            sql += VeriErisim.DegerYap(ilgiliIslemKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(ilgiliSiraNo) + ", ";
            sql += VeriErisim.DegerYap(islemYapanKT) + ", ";
            sql += VeriErisim.DegerYap(islemYapanKullaniciKod) + ", ";
            sql += VeriErisim.DegerYap(islemTarih) + ", ";
            sql += VeriErisim.DegerYap(new TNSDateTime()) + ", ";
            sql += VeriErisim.DegerYap(bumkoKurum.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(bumkoSiraNo) + ") ";

            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }

        /// <summary>
        /// Verilen kayıt, insert SQL'ine çevrilir ve çalıştırılır.
        /// </summary>
        /// <param name="yil">Eklenecek işlemin yıl bilgisi.</param>
        /// <param name="kurum">Eklenecek işlemin kurum bilgisi.</param>
        /// <param name="islemKod">Eklenecek işlemin işlemkodu (serbest, ekleme) bilgisi.</param>
        /// <param name="siraNo">Eklenecek işlemin sıra no bilgisi.</param>
        /// <param name="yaziTarih">Eklenecek işlemin yazı tarihi.</param>
        /// <param name="yaziNo">Eklenecek işlemin yazı numarası.</param>
        /// <param name="ilgi">Eklenecek işlemin ilgisi.</param>
        /// <param name="dipNot">Eklenecek işlemin dip not bilgisi.</param>
        /// <param name="ilgiliIslemKod">İptal işlemi veya Doğru yanlış işlemi için; iptal edilen, yanlışlanan işlemin işlem kodu</param>
        /// <param name="ilgiliSiraNo">İptal işlemi veya doğru yanlış işlemi için; iptal edilen, yanlışlanan işlemin sıra numarası</param>
        /// <param name="islemYapanKT">İşlemi yapan kullanıcın kulllanıcı tipi</param>
        /// <param name="islemYapanKullaniciKod">İşlemi yapan kullanıcın kulllanıcı kodu</param>
        /// <param name="islemTarih">İşlemin yapıldığı tarih</param>
        /// <param name="bumkoKurum">BÜMKO Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="bumkoSiraNo">BÜMKO Sıra No bilgisini tutan, Int türünde nesne</param>
        /// <param name="formDurum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Yapılan işlemin sonucu, sonuç yapısında döndürülür.</returns>
        public Sonuc Guncelle(int yil, string kurum, string islemKod, int siraNo, TNSDateTime yaziTarih,
                           string yaziNo, string ilgi, string dipNot, int formDurum, string ilgiliIslemKod,
                           int ilgiliSiraNo, int islemYapanKT, string islemYapanKullaniciKod, TNSDateTime islemTarih, string bumkoKurum, int bumkoSiraNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], yaziTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAZINO], yaziNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILGI], ilgi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NOTLAR], dipNot) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FORMDURUM], formDurum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILGILIISLEM], ilgiliIslemKod.Replace(".", "")) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILGILISIRANO], ilgiliSiraNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKTKOD], islemYapanKT) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKULKOD], islemYapanKullaniciKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISIKLIKTARIH], islemTarih) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYTARIH], new TNSDateTime()) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUMKOKURUMKOD], bumkoKurum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUMKOSIRANO], bumkoSiraNo) + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], " = ", siraNo));


            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }

        /// <summary>
        /// Parametre olarak verilen Bütçe İşlem Form bilgilerini kullanarak,
        /// BÜMKO ile ilgili bilgileri (Kurum ve Sıra No) veri tabanında güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BUMKOKodAta(BIForm islem)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUMKOKURUMKOD], islem.bumkoKurum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUMKOSIRANO], islem.bumkoSiraNo) + " ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", islem.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", islem.kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islem.islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], " = ", islem.siraNo));


            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }


        /// <summary>
        /// Verilen bütçe işlemi silinir.
        /// </summary>
        /// <param name="yil">Silinecek işlemin yıl bilgisi.</param>
        /// <param name="kurum">Silinecek işlemin kurum bilgisi.</param>
        /// <param name="islemKod">Silinecek işlemin işlemkodu (serbest, ekleme) bilgisi.</param>
        /// <param name="siraNo">Silinecek işlemin sıra no bilgisi.</param>
        /// <returns>Yapılan işlemin sonucu, sonuç yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            if (siraNo > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], " = ", siraNo));

            Sonuc s = KomutCalistir(sql, 0);
            if (s.islemSonuc && s.kayitSay > 1 && siraNo > -1)
            {
                s.islemSonuc = false;
                s.hataStr = ButceUygulamaResI.BUM334;
            }

            return s;
        }

        /// <summary>
        /// Verilen yıl ve kuruma ait, istenilen formdurumundaki kayıtlar listelenir.
        /// </summary>
        /// <param name="yil">Okunacak işlemin listeleneceği yıl.</param>
        /// <param name="kurum">Okunacak işlemin listeleneceği kurumun kodu.</param>
        /// <param name="islemKod">Okunacak işlemin işlem kodu.</param>
        /// <param name="siraNo">Okunacak işlemin sıra no bilgisi.</param>
        /// <returns>ISLForm'daki istenilen koşullara uygun tüm kayıtların, tüm alanları, IDataReader olarak döndürülür.</returns>
        public IDataReader Oku(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kaydedilecek bütçe işlemine verilecek sırano bulunur. 
        /// Bu amaçla  verile işlem kodunun ilk dört basamağına göre, 
        /// en  büyük sıranumarası bulunur ve bir fazlası döndürülür.
        /// </summary>
        /// <param name="yil">Yeni sıra numarası bulunacak yıl</param>
        /// <param name="kurum">Yeni sıra numarası bulunacak kurum</param>
        /// <param name="islemKod">Yeni sıra numarası bulunacak işlem</param>
        /// <returns>Sistemde kayıtlı sıra numarasının bir fazlası döndürülür.</returns>
        public int SiraNoBul(int yil, string kurum, string islemKod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));

            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            int siraNo = (int)veriErisim.TekKolonSQLDbl(sql);
            if (veriErisim.hata == "")
            {
                return siraNo + 1;
            }

            return -1;
        }

        /// <summary>
        /// ISLFORM tablosunun, formdurum bilgisi değiştirilir. bu islem ile ilgili bilgiler kaydedilir.
        /// </summary>
        /// <param name="yil">Form durumu değiştirilecek formun, yıl bilgisi.</param>
        /// <param name="kurum">Form durumu değiştirilecek formun, kurum bilgisi.</param>
        /// <param name="islemKod">Form durumu değiştirilecek formun, işlem kod bilgisi.</param>
        /// <param name="siraNo_Asama">Form durumu değiştirilecek formun, form bütçe işlemi ise sıra no bilgisi, işlem ahp veya ihp ise aşama bilgisi.</param>
        /// <param name="durum">Formun yeni durum kodu</param>
        /// <param name="islemYapanKT">Form durumunu değiştiren kullanıcının kullanıcı tip kodu.</param>
        /// <param name="islemYapanKulKod">Form durumunu değiştiren kullanıcının kullanıcı kodu</param>
        /// <param name="onayTarih">Form durumunun değiştirildiği tarih</param>
        /// <returns>Yapılan işlemin sonucu, sonuç yapısında döndürülür.</returns>
        public Sonuc OnayKaydet(int yil, string kurum, string islemKod, int siraNo_Asama, int durum, int islemYapanKT, string islemYapanKulKod, TNSDateTime onayTarih)
        {
            if (islemKod.StartsWith("01.01"))
            {
                // ??? return new Sonuc(false, "AHP İşlemi bir süre için kapatılmıştır","");
            }
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM], durum) + ", ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKTKOD], islemYapanKT) + ", ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKULKOD], islemYapanKulKod) + ", ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ONAYTARIH], onayTarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo_Asama));

            return KomutCalistir(sql, 1);
        }

        /*
		public Sonuc OnayKaldir(int yil, string kurum, string islemKod, int siraNo_Asama, int durum, int islemYapanKT, string islemYapanKulKod, TNSDateTime onayTarih)
		{	
            if(islemKod.Replace(".","").StartsWith("0101"))
            {
                // ??? return new Sonuc(false, "AHP İşlemi bir süre için kapatılmıştır","");
            }
	
			string sql="";

			sql = "UPDATE " + TABLOAD +" SET "; 
			sql += VeriErisim.AtamaOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.FORMDURUM], durum)+ ", " ;
			sql += VeriErisim.AtamaOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKTKOD], islemYapanKT)+ ", ";
			sql += VeriErisim.AtamaOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKULKOD], islemYapanKulKod)+ ", ";
			sql += VeriErisim.AtamaOl(TABLOAD,ALANLAR[(int)ENUMAlanlar.ONAYTARIH],onayTarih);
			sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
			sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".","")));
			sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
			sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo_Asama));

			return	KomutCalistir(sql, 1);					
		}


        /// <summary>
        /// Verilen yıl ve kuruma ait, istenilen formdurumundaki kayıtlar listelenir.
        /// </summary>
        /// <param name="yil">Bütçe işlemlerinin listeleneceği yıl.</param>
        /// <param name="kurum">Bütçe işlemlerinin listeleneceği kurumun kodu.</param>
        /// <param name="islemKod">Bütçe işlemlerinin listeleneceği, işlem türü.</param>
        /// <param name="formDurumGrup">Listelenecek formların durumu.</param>
        /// <returns>ISLForm'daki istenilen koşullara uygun tüm kayıtların, tüm alanları, IDataReader olarak döndürülür.</returns>
        public IDataReader Listele(int yil, string kurum, string islemKod, ENUMFormDurumGrup formDurumGrup)
        {
            string sql="SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql=VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL] ,"=",yil));
            sql=VeriErisim.KosulEkle(sql, "AND", GenelKosul.KurumKosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD] ,"=",kurum));
            sql=VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));

            sql=GenelKosul.FormDurumKosulOl(sql, "AND", formDurumGrup, "", ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            sql+=" order by 1,2,4 desc,3";
            return veriErisim.DataReaderDondur(sql);
        }

*/
    }
} 

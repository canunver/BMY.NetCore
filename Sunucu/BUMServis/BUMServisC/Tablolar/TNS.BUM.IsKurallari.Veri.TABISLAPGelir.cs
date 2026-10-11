using System.Data;
using TNS.Veri;
using TNS.BKD;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Ayrıntılı Program Gelir bilgilerini tutan ISLAPGELIR veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABISLAPGelir : TABAyrintiliProgram
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABISLAPGelir(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ISLAPGELIR";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURUMKOD", "ISLEM", "SIRANO", "KURKOD", "EKOKOD", "AY", "ODENEK", "FORMDURUM" };

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
            ISLEM = 2,// Ahp için 
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,
            /// <summary>
            /// Kurumsal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 4,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,
            /// <summary>
            /// Ay alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 6,
            /// <summary>
            /// Ödenek alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK = 7,
            /// <summary>
            /// Form durumu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMDURUM = 8
        }


        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No ve Durum bilgilerini kullanarak,
        /// Gelir bilgilerini veri tabanından güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc OnayKaydet(int yil, string kurum, string islemKod, int siraNo, int durum)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM], durum) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No ve Durum bilgilerini kullanarak,
        /// Gelir bilgilerini veri tabanından güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem Kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc EskiOnaylariDegistir(int yil, string kurum, string islemKod, int siraNo, int durum)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM], durum) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "<", siraNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Tertiplere ilişkin durum bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl  bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <param name="sadeceRevizeler">Revize bilgisine göre koşul eklenip eklenmeyeceği bilgisini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tertip Durum bilgilerini (Ay, Ödenek) tutan IDataReader türünde nesne döndürür</returns>
        public override IDataReader TertipDurum(int yil, string[] kodlar, ENUMFormDurumGrup onayDurum, bool sadeceRevizeler)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);
            kosul = GenelKosul.FormDurumKosulOl(kosul, "AND", onayDurum, "", ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            if (sadeceRevizeler)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", "01%"));
            }

            string sql = "SELECT AY, SUM(ODENEK) from " + TABLOAD + " where " + kosul + " Group By AY Order By 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisine göre,
        /// Revize olmayan Onaylı Tertip durumlarını TertipDurum rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurum, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Tertip Durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public override IDataReader TertipDurum(int yil, string[] kodlar, ENUMFormDurumGrup onayDurum)
        {
            return TertipDurum(yil, kodlar, ENUMFormDurumGrup.ONAYLI, false);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisine göre,
        /// Onaylı Tertip durumlarını TertipDurum rutinini çalıştırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurum, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Tertip Durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public override IDataReader TertipDurum(int yil, string[] kodlar)
        {
            return TertipDurum(yil, kodlar, ENUMFormDurumGrup.ONAYLI);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Sıra no bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Gelir bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Gelir bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public override IDataReader Listele(int yil, string kurum, string islem, int siraNo)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            string sql = "SELECT KURKOD, EKOKOD, AY, ODENEK from " + TABLOAD + " where " + kosul + " Order By 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Aşama ve Kodlar bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Aylık Ödenek bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal ve Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Aylık Ödenek bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public override IDataReader AylikOdenek(int yil, string kurum, string islem, int asama, string[] kodlar)
        {
            string kosul = "";

            if (kurum != "")
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", asama));
            kosul = GenelKosul.KistasKosulOlFinansman(yil, kosul, kodlar);

            string sql = "SELECT AY,SUM(ODENEK) from " + TABLOAD + " where " + kosul + " Group By AY Order By 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Aşama ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Kıstasa uygun Gelir kayıt sayısını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kodlar bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Gelir kayıt sayısını tutan Double türünde nesne döndürür</returns>
        public override double Kontrol(int yil, string kurum, string islem, int asama, string[] kodlar)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);

            if (kurum != "")
                kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.KurumKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", asama));

            string sql = "SELECT count(*) from " + TABLOAD + " where " + kosul;
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Gelir bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kodlar bilgisini tutan, String dizi türünde nesne</param>
        /// <param name="ay">Ay bilgisini tutan, Int türünde nesne</param>
        /// <param name="odenek">Ödenek bilgisini tutan, Double türünde nesne</param>
        /// <param name="durum">Durum bilgisini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc Kaydet(int yil, string kurum, string islem, int asama, string[] kodlar, int ay, double odenek, ENUMFormDurum durum)
        {
            if (kurum.Length < 2)
                return new Sonuc(false, ButceUygulamaResI.BUM337, "");
            if (islem.StartsWith(TNS.BUM.BISabitler.IslemKod(ENUMIslemKod.IHP)))
            {
                if (kodlar[1].StartsWith("09"))
                {
                    if (odenek > 0)
                        return new Sonuc(false, ButceUygulamaResI.BUM338, "");
                }
                else
                {
                    if (odenek < 0)
                        return new Sonuc(false, ButceUygulamaResI.BUM339, "");
                }
            }

            string sql = "insert into " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (" +
                VeriErisim.DegerYap(yil) + ", " +
                VeriErisim.DegerYap(kurum.Replace(".", "")) + ", " +
                VeriErisim.DegerYap(islem.Replace(".", "")) + ", " +
                VeriErisim.DegerYap(asama) + ", " +
                VeriErisim.DegerYap(((string)kodlar[0]).Replace(".", "")) + ", " +
                VeriErisim.DegerYap(((string)kodlar[1]).Replace(".", "")) + ", " +
                VeriErisim.DegerYap(ay) + ", " +
                VeriErisim.DegerYap(odenek) + ", " +
                VeriErisim.DegerYap((int)durum) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen bilgileri kullanarak,
        /// İlgili Gelir bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc Sil(int yil, string kurum, string islem, int asama, string[] kodlar)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);
            if (!string.IsNullOrWhiteSpace(kurum))
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[1], "=", kurum.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[2], "=", islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[3], "=", asama));
            //kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[5], "LIKE", (kodlar[1].Replace(".", "")) + "%"));
            string sql = "delete from " + TABLOAD;
            sql = sql + " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }


        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No bilgilerini kullanarak,
        /// İlgili Gelir bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public override Sonuc Sil(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            if (siraNo > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], " = ", siraNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, İşlem Kodu, Sıra No ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Tertip toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra No bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kodlar bilgisini tutan, String dizi türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Tertip Toplamını tutan Double türünde nesne döndürür</returns>
        public override double TertipToplamiBul(int yil, string islemKod, int siraNo, string[] kodlar)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);
            if (siraNo > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], " = ", siraNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            string sql = "SELECT SUM(ODENEK) from " + TABLOAD + " where " + kosul;
            return veriErisim.TekKolonSQLDbl(sql);

        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra No bilgisini kıstas olarak kullanarak,
        /// Gelir toplamını döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Gelir Toplamını tutan Double türünde nesne döndürür</returns>
        public override double ToplamBul(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql = "SELECT SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODENEK], ">", (int)0));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kodlar bilgisini kullanarak,
        /// Yeni Onaylı Revize olanların toplam değerini ToplamBulRevize rutinini çağırarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde nesne</param>
        /// <param name="hepsi">Hepsi parametresine göre Revize çeşitlerine göre (AHP,IHP) koşulun eklenip eklenmeyeceğini belirten, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Revize toplamını tutan Double türünde nesne döndürür</returns>
        public override double ToplamBulRevize(int yil, string[] kodlar, bool hepsi)
        {
            return ToplamBulRevize(yil, kodlar, hepsi, -1, ENUMFormDurumGrup.YENI_ONAYLI);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kodlar bilgisini kıstas olarak kullanarak,
        /// Kıstasa uygun Revize toplam değerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurumsal, Fonksiyonel, Finansal, Ekonomik kod bilgilerini tutan, String dizi türünde  nesne</param>
        /// <param name="hepsi">Hepsi parametresine göre Revize çeşitlerine göre (AHP,IHP) koşulun eklenip eklenmeyeceğini belirten, Bool türünde nesne</param>
        /// <param name="ay">Ay bilgisini tutan, Int türünde nesne</param>
        /// <param name="onayDurum">Durum bilgisini tutan, ENUMFormDurumGrup türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Revize Toplamı değerini tutan Double türünde nesne döndürür</returns>
        public override double ToplamBulRevize(int yil, string[] kodlar, bool hepsi, int ay, ENUMFormDurumGrup onayDurum)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);

            kosul = GenelKosul.FormDurumKosulOl(kosul, "AND", onayDurum, "", ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            if (ay >= 1 && ay <= 12)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "<=", ay));
            if (!hepsi)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", BISabitler.IslemKod(ENUMIslemKod.AHP).Replace(".", "") + "%"));
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE).Replace(".", "") + "%"));
                kosul = kosul + ")";
            }

            string sql = "SELECT SUM(ODENEK) from " + TABLOAD + " where " + kosul;
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem, Sıra no bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Gider bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="islem">İşlem bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Secilecek kayıtların kıstasının tutulduğu tertip nesnesi.</param>
        /// <param name="secAlan">Ay ve ödenek dışında sonuç kümesinde bulunacak alanlar.</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Gider bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public override IDataReader ListeleKodBilgi(string islem, int siraNo, Tertip kistasTertip, string secAlan)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kistasTertip.yil);
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));

            string sql = "SELECT " + secAlan + ", AY, SUM(ODENEK) from " + TABLOAD + " where " + kosul + " Group By " + secAlan + ", AY Order By " + secAlan;

            return veriErisim.DataReaderDondur(sql);
        }

        /*
        public IDataReader AGPAylikOdenek(int yil, string kurum, string islem, int asama, string[] kodlar)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);

            if (kurum != "")
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islem.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", asama));
            string sql = "SELECT AY,SUM(ODENEK) from " + TABLOAD + " where " + kosul + " Group By AY Order By 1";


            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen bütçe işlemine ait tüm detay gider kayıtları silinir.
        /// </summary>
        /// <param name="yil">Detay kayıtları silinecek bütçe işleminin yıl bilgisi.</param>
        /// <param name="kurum">Detay kayıtları silinecek bütçe işleminin kurum bilgisi.</param>
        /// <param name="islemKod">Detay kayıtları silinecek bütçe işleminin işlemkodu (serbest, ekleme) bilgisi.</param>
        /// <param name="siraNo">Detay kayıtları silinecek bütçe işleminin sıra no bilgisi.</param>
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
            return s;
        }

        public double ToplamBulRevize(int yil, string[] kodlar)
        {
            string kosul = GenelKosul.KistasKosulOlFinansman(yil, "", kodlar);

            kosul = GenelKosul.FormDurumKosulOl(kosul, "AND", ENUMFormDurumGrup.YENI_ONAYLI, "", ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", BISabitler.IslemKod[(int)ENUMIslemKod.AHP].Replace(".", "") + "%"));
            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", BISabitler.IslemKod[(int)ENUMIslemKod.IHPREVIZE].Replace(".", "") + "%"));
            kosul = kosul + ")";

            string sql = "SELECT SUM(ODENEK) from " + TABLOAD + " where " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }
*/

    }

}

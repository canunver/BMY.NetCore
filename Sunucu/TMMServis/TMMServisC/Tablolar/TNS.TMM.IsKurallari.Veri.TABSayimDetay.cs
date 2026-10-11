using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır Sayım detay bilgilerini tutan TASSAYIMDETAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSayimDetay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSayimDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSAYIMDETAY";

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
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 3,
            /// <summary>
            /// Sayım no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYIMNO = 4,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 5,
            /// <summary>
            /// Ambar miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARMIKTAR = 6,
            /// <summary>
            /// Ortak miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORTAKMIKTAR = 7,
            /// <summary>
            /// Açıklama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 8
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "SAYIMNO",
                                   "HESAPPLANKOD", "AMBARMIKTAR", "ORTAKMIKTAR", "ACIKLAMA" };

        /// <summary>
        /// Sayım Detay bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Sayım detay bilgisini tutan, SayimDetay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(SayimDetay sd)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sd.yil) + ",";
            sql += VeriErisim.DegerYap(sd.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(sd.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sd.ambarKod) + ",";
            sql += VeriErisim.DegerYap(sd.sayimNo) + ",";
            sql += VeriErisim.DegerYap(sd.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sd.ambarMiktar) + ",";
            sql += VeriErisim.DegerYap(sd.ortakMiktar) + ",";
            sql += VeriErisim.DegerYap(sd.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım Form bilgisini kullanarak
        /// Sayım Detay bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgilerini tutan, SayimForm türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(SayimForm sf)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sf.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", sf.sayimNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Sayım Detay bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgilerini tutan, SayimForm türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Sayım Detay bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(SayimForm sf)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASOLCUBIRIM.KOD, TASOLCUBIRIM.AD,TASHESAPPLAN.ACIKLAMA";
            sql += " FROM " + TABLOAD + ", TASHESAPPLAN, TASOLCUBIRIM";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD] + "=TASHESAPPLAN.KOD";
            sql += " AND TASHESAPPLAN.OLCUBIRIMKOD=TASOLCUBIRIM.KOD";

            if (sf.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sf.yil));

            if (!string.IsNullOrEmpty(sf.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sf.muhasebeKod));

            if (!string.IsNullOrEmpty(sf.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sf.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(sf.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sf.ambarKod));

            if (!string.IsNullOrEmpty(sf.sayimNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", sf.sayimNo));

            sql += " ORDER BY " + TABLOAD + ".HESAPPLANKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım Form bilgisine göre,
        /// Ambar stok durumu bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgilerini tutan, SayimForm türünde nesne</param>
        /// <param name="girisMi">Giriş parametresine göre koşul eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ambar stok durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader AmbardakileriGetirFromStok(SayimForm sf, bool girisMi)
        {
            string sql = "SELECT " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD, " + VeriErisim.IslecBul("SUM") + "(MIKTAR) FROM TASAMBARSTOKVIEW";

            string kosul = KosulOlustur(sf, "TASAMBARSTOKVIEW");

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "DURUM", "=", (int)ENUMBelgeDurumu.ONAYLI));

            if (girisMi)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", ">=", (int)ENUMIslemTipi.SATINALMAGIRIS));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", "<", (int)ENUMIslemTipi.TUKETIMCIKIS));
            }
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", "=", (int)ENUMIslemTipi.DEGERARTTIR));
                kosul += ")";
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", "<>", (int)ENUMIslemTipi.YILDEVIRCIKIS));
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD";
            sql += " ORDER BY " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım Form bilgisine göre,
        /// Ambar sicil durumu bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgilerini tutan, SayimForm türünde nesne</param>
        /// <param name="girisMi">Giriş parametresine göre koşul eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ambar sicil durum bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader AmbardakileriGetirFromSicil(SayimForm sf, bool girisMi)
        {
            string sql = "SELECT " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD, " + VeriErisim.IslecBul("COUNT") + "(*) FROM TASSICILHAREKETVIEW";

            string kosul = KosulOlustur(sf, "TASSICILHAREKETVIEW");

            //Erdal View iptalleri eliyor zaten
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "DURUM", "=", (int)ENUMSicilDurum.AKTIF));

            if (girisMi)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", ">=", (int)ENUMIslemTipi.SATINALMAGIRIS));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<", (int)ENUMIslemTipi.TUKETIMCIKIS));
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<>", (int)ENUMIslemTipi.ZFDUSME));
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<>", (int)ENUMIslemTipi.DTLDUSME));
            }
            else
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<>", (int)ENUMIslemTipi.ZFVERME));
                //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<>", (int)ENUMIslemTipi.DTLVERME));
            }

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "SICILNOHAREKETSICILNO", "NOT LIKE", "255070201??13%"));


            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD";
            sql += " ORDER BY " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Sayım Form bilgisine göre,
        /// Ambar Zimmet İşlem Form bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgilerini tutan, SayimForm türünde nesne</param>
        /// <param name="ortakKullanimMi">Ortak Kullanım parametresine göre koşul eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <param name="girisMi">Giriş parametresine göre koşul eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ambar Zimmet İşlem Form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader AmbardakileriGetirFromZIF(SayimForm sf, bool ortakKullanimMi, bool girisMi)
        {
            string sql = "SELECT " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD, " + VeriErisim.IslecBul("COUNT") + "(*) FROM TASSICILHAREKETVIEW";

            string kosul = KosulOlustur(sf, "TASSICILHAREKETVIEW");

            //Erdal View iptalleri eliyor zaten
            //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "DURUM", "=", (int)ENUMSicilDurum.AKTIF));

            //Erdal Oracle'da null, MS Sql'de boşluk oluyor değer
            //İşlem türü sorgusunu değiştirdim ve buna gerek kalmadı
            //if (ortakKullanimMi)
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "NEREYEGITTI", "<>", ""));
            //else
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "NEREYEGITTI", "=", ""));

            if (girisMi && ortakKullanimMi)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "=", (int)ENUMIslemTipi.DTLDUSME));
            else if (girisMi && !ortakKullanimMi)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "=", (int)ENUMIslemTipi.ZFDUSME));
            else if (!girisMi && ortakKullanimMi)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "=", (int)ENUMIslemTipi.DTLVERME));
            else if (!(girisMi || ortakKullanimMi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "=", (int)ENUMIslemTipi.ZFVERME));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD";
            sql += " ORDER BY " + "HESAPPLANKOD, HESAPPLANAD, OLCUBIRIMAD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilem Sayım Form bilgisi ve Tablo adına göre
        /// SQL cümlesine eklenecek koşul ifadesini oluşturan fonksiyon
        /// </summary>
        /// <param name="sf">Sayım Form bilgilerini tutan, SayimForm türünde nesne</param>
        /// <param name="tabloAdi">Tablo adı değerini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonunu çalışması sonucunda koşul ifadesini tutan String türünde nesne döndürür</returns>
        private string KosulOlustur(SayimForm sf, string tabloAdi)
        {
            string kosul = string.Empty;

            if (sf.harcamaKod.StartsWith("12") && (tabloAdi == "TASSICILHAREKETVIEW" || tabloAdi == "TASAMBARSTOKVIEW"))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, "ISLEMTIPIKOD", "<>", 26));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, "ISLEMTIPIKOD", "<>", 27));
            }

            if (sf.yil > 0 && tabloAdi == "TASAMBARSTOKVIEW")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.YIL], "=", sf.yil));
            else if (sf.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.YIL], "<=", sf.yil));

            if (!string.IsNullOrEmpty(sf.muhasebeKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sf.muhasebeKod));

            if (!string.IsNullOrEmpty(sf.harcamaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sf.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(sf.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sf.ambarKod));

            if (tabloAdi == "TASSICILHAREKETVIEW")
            {
                if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "SICILNOHAREKETSICILNO", "NOT LIKE", "255070201??13%"));
            }

            return kosul;
        }

        //private string KosulOlustur(SayimDetay sd, string tabloAdi)
        //{
        //    string kosul = string.Empty;

        //    if (sd.yil > 0 && tabloAdi == "TASAMBARSTOKVIEW")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.YIL], "=", sd.yil));
        //    else if (sd.yil > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.YIL], "<=", sd.yil));

        //    if (!string.IsNullOrEmpty(sd.muhasebeKod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sd.muhasebeKod));

        //    if (!string.IsNullOrEmpty(sd.harcamaKod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", sd.harcamaKod.Replace(".", "")));

        //    if (!string.IsNullOrEmpty(sd.ambarKod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", sd.ambarKod));

        //    if (!string.IsNullOrEmpty(sd.hesapPlanKod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAdi, ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "=", sd.hesapPlanKod.Replace(".", "")));

        //    return kosul;
        //}

        //public decimal MiktarGetirFromStok(SayimDetay sd, bool girisMi)
        //{
        //    string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(MIKTAR) FROM TASAMBARSTOKVIEW";

        //    string kosul = KosulOlustur(sd, "TASAMBARSTOKVIEW");

        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "DURUM", "=", (int)ENUMBelgeDurumu.ONAYLI));

        //    if (girisMi)
        //    {
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", ">=", (int)ENUMIslemTipi.SATINALMAGIRIS));
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", "<", (int)ENUMIslemTipi.TUKETIMCIKIS));
        //    }
        //    else
        //    {
        //        kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));
        //        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", "=", (int)ENUMIslemTipi.DEGERARTTIR));
        //        kosul += ")";
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARSTOKVIEW", "ISLEMTUR", "<>", (int)ENUMIslemTipi.YILDEVIRCIKIS));
        //    }

        //    if (!string.IsNullOrEmpty(kosul))
        //        sql += " WHERE " + kosul;

        //    return veriErisim.TekKolonSQLDecimal(sql);
        //}

        //public decimal MiktarGetirFromSicil(SayimDetay sd, bool girisMi)
        //{
        //    string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM TASSICILHAREKETVIEW";

        //    string kosul = KosulOlustur(sd, "TASSICILHAREKETVIEW");

        //    //Erdal View iptalleri eliyor zaten
        //    //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "DURUM", "=", (int)ENUMSicilDurum.AKTIF));

        //    if (girisMi)
        //    {
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", ">=", (int)ENUMIslemTipi.SATINALMAGIRIS));
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<", (int)ENUMIslemTipi.TUKETIMCIKIS));
        //        //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<>", (int)ENUMIslemTipi.ZFDUSME));
        //        //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<>", (int)ENUMIslemTipi.DTLDUSME));
        //    }
        //    else
        //    {
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", ">=", (int)ENUMIslemTipi.TUKETIMCIKIS));
        //        //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<>", (int)ENUMIslemTipi.ZFVERME));
        //        //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "<>", (int)ENUMIslemTipi.DTLVERME));
        //    }

        //    if (!string.IsNullOrEmpty(kosul))
        //        sql += " WHERE " + kosul;

        //    return veriErisim.TekKolonSQLDecimal(sql);
        //}

        //public decimal MiktarGetirFromZIF(SayimDetay sd, bool ortakKullanimMi, bool girisMi)
        //{
        //    string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM TASSICILHAREKETVIEW";

        //    string kosul = KosulOlustur(sd, "TASSICILHAREKETVIEW");

        //    //Erdal View iptalleri eliyor zaten
        //    //kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "DURUM", "=", (int)ENUMSicilDurum.AKTIF));

        //    if (ortakKullanimMi)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "NEREYEGITTI", "<>", ""));
        //    else
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "NEREYEGITTI", "=", ""));

        //    if (girisMi)
        //    {
        //        kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "=", (int)ENUMIslemTipi.ZFDUSME));
        //        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "=", (int)ENUMIslemTipi.DTLDUSME));
        //        kosul += ")";
        //    }
        //    else
        //    {
        //        kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "=", (int)ENUMIslemTipi.ZFVERME));
        //        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("TASSICILHAREKETVIEW", "ISLEMTIPTUR", "=", (int)ENUMIslemTipi.DTLVERME));
        //        kosul += ")";
        //    }

        //    if (!string.IsNullOrEmpty(kosul))
        //        sql += " WHERE " + kosul;

        //    return veriErisim.TekKolonSQLDecimal(sql);
        //}

    }
}
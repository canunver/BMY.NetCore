using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Alt faaliyet bilgilerini tutan ABSPEBKOD veri tabanı tablosu, yatırım bağını tutan YATBUTCEKAYNAKPEBKOD  ve performans bağını tutan PEBFAALIYETPROJEPEBFON tabloları ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABABSPEBKOD : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABABSPEBKOD(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ABSPEBKOD";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 1,

            /// <summary>
            /// PEBAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBAD = 2,

            /// <summary>
            /// PEBAD2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBAD2 = 3,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 4,

            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 5,

            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "PEBKOD"
            , "PEBAD"
            , "PEBAD2"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
        };

        /// <summary>
        /// AltFaaliyet yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="altFaaliyet">Anahtar değişkenleri tutan AltFaaliyet türünde nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(AltFaaliyet altFaaliyet)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", altFaaliyet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", altFaaliyet.pebKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// AltFaaliyet yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="altFaaliyet">Eklenecek kayıt bilgisini tutan, AltFaaliyet türünde nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(AltFaaliyet altFaaliyet)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(altFaaliyet.yil);
            sql += "," + VeriErisim.DegerYap(altFaaliyet.pebKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(altFaaliyet.pebAd);
            sql += "," + VeriErisim.DegerYap(altFaaliyet.pebAd2);
            sql += "," + VeriErisim.DegerYap(altFaaliyet.kurKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(altFaaliyet.fonKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(altFaaliyet.finKod.Replace(".", ""));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// AltFaaliyet yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="altFaaliyet">Select edilecek kayıtlara ait kıstas bilgilerini tutan, AltFaaliyet türünde nesne</param>
        /// <returns>Verilen kriterlere göre oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(AltFaaliyet altFaaliyet)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", altFaaliyet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", altFaaliyet.pebKod.Replace(".", "")));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// AltFaaliyet yapısında duran alanlar ile verilmiş ise kod uzunluğunu kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="altFaaliyet">Select edilecek kayıtlara ait kıstas bilgilerini tutan, AltFaaliyet türünde nesne</param>
        /// <param name="kodUzunluk">Alt faaliyet kodunun uzunluğu, sıfırdan farklı ise, bu değişkende verilen değer kadar olan kayıtlar döndürülür.</param>
        /// <returns>Verilen kriterlere göre oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleUzunluk(AltFaaliyet altFaaliyet)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", altFaaliyet.yil));
            if (!string.IsNullOrEmpty(altFaaliyet.pebKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "LIKE", altFaaliyet.pebKod.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(altFaaliyet.pebAd))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBAD], "LIKE", "%" + altFaaliyet.pebAd + "%"));
            if (altFaaliyet.kodUzunluk > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.IslecBul("LEN") + "(" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD] + ")", "=", altFaaliyet.kodUzunluk));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Alt faaliyetle, yatırım kaynağı arasında kurulmuş olan ve YATBUTCEKAYNAKPEBKOD tablosunda tutulan bağı silen yordam.
        /// </summary>
        /// <param name="altFaaliyet">Bağı silinecek alt faaliyet bilgilerini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc YatBagSil(AltFaaliyet altFaaliyet, string senaryoKod)
        {
            string sql = "DELETE FROM YATBUTCEKAYNAKPEBKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", altFaaliyet.yil));
            if (!string.IsNullOrWhiteSpace(senaryoKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "PEBKOD", "=", altFaaliyet.pebKod.Replace(".", "")));
            return this.KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Alt faaliyetle, yatırım kaynağı arasında bağ kurmak için YATBUTCEKAYNAKPEBKOD tablosuna kayıt ekleyen yordam.
        /// </summary>
        /// <param name="altFaaliyet">Bağ eklenecek alt faaliyet ve yatırım alt projesi kaynak bilgilerini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc YatBagEkle(AltFaaliyet altFaaliyet, string senaryoKod)
        {
            string sql = "INSERT INTO YATBUTCEKAYNAKPEBKOD(YIL, SENARYOKOD, PEBKOD, KAYNAKNO) VALUES (";
            sql += VeriErisim.DegerYap(altFaaliyet.yil) + ", ";
            sql += VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(altFaaliyet.pebKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(altFaaliyet.butceKaynakNo) + ") ";
            return this.KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen alt faaliyetin ilişkili olduğu alt proje kaynak verisini bulan yordam.
        /// </summary>
        /// <param name="altFaaliyet">İlişkili olduğu alt proje kaynak ilişkisi bulunacak, alt faaliyetin bilgisini tutan nesne</param>
        /// <returns>Alt faaliyetin, ilişkili olduğu alt proje kaynak dizgisi döndürülür.</returns>
        public void YatBagOku(AltFaaliyet altFaaliyet, string senaryoKod, AltFaaliyet okunan) // string senaryoKod, out int apNo, out string apAd, out int kaynakNo, out string kaynakAd)
        {
            string sql = "SELECT AP.ALTPROJENO, AP.AD, K.KAYNAKNO, K.KAYNAKAD FROM YATBUTCEKAYNAKPEBKOD KP INNER JOIN YATBUTCEKAYNAK K ON " + VeriErisim.Join("KP", "K", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += "  INNER JOIN YATALTPROJE AP ON " + VeriErisim.Join("K", "AP", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("KP", "YIL", "=", altFaaliyet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KP", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KP", "PEBKOD", "=", altFaaliyet.pebKod.Replace(".", "")));

            IDataReader dr = veriErisim.DataReaderDondur(sql);
            if (dr != null)
            {
                if (dr.Read())
                {
                    okunan.altProje = VeriErisim.DegerAl(dr, 0, 0);
                    okunan.butceKaynakNo = VeriErisim.DegerAl(dr, 2, 0);
                    okunan.altProjeKaynakAd = VeriErisim.DegerAl(dr, 1, "") + "-" + VeriErisim.DegerAl(dr, 3, "");
                }
                dr.Close();
            }
        }

        /// <summary>
        /// Kıstas olarak verilen alt faaliyet kıstas bilgilerinin ve/veya yatırım projesinin
        /// ilişkili olduğu tüm alt proje kaynaklarını ve ilişkili olduğu alt faaliyetleri döndüren yordam.
        /// </summary>
        /// <param name="altFaaliyet">Alt proje kaynakları listelenecek alt faaliyet kıstas bilgileri bulunacak, alt faaliyetin bilgisini tutan nesne</param>
        /// <param name="projeKod">Alt proje kaynakları listelenecek yatırım projesi kodu</param>
        /// <returns>Verilen kriterlere göre hazırlanmış select cümleciğinin çalıştırılması sonucu oluşturulan DataReader döndürülür.</returns>
        public IDataReader YatBagPebKodlariBul(AltFaaliyet altFaaliyet, int projeKod, string senaryoKod)
        {
            string sql = "SELECT YATBUTCEKAYNAKPEBKOD.YIL, YATBUTCEKAYNAKPEBKOD.PEBKOD";
            sql += ", YATALTPROJE.ALTPROJENO, YATBUTCEKAYNAKPEBKOD.KAYNAKNO";
            sql += " FROM YATBUTCEKAYNAKPEBKOD, YATBUTCEKAYNAK, YATALTPROJE, YATPROJE";  //ON " + VeriErisim.Join("", "", new string[] { });

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("YATBUTCEKAYNAKPEBKOD", "YIL", "=", altFaaliyet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKPEBKOD", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.Join("YATBUTCEKAYNAKPEBKOD", "YATBUTCEKAYNAK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" }));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.Join("YATBUTCEKAYNAK", "YATALTPROJE", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.Join("YATALTPROJE", "YATPROJE", new string[] { "YIL", "SENARYOKOD", "PROJENO" }));

            if (projeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATPROJE", "PROJENO", "=", projeKod));

            if (!string.IsNullOrEmpty(altFaaliyet.pebKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKPEBKOD", "PEBKOD", "LIKE", altFaaliyet.pebKod.Replace(".", "") + "%"));

            if (altFaaliyet.altProje > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATALTPROJE", "ALTPROJENO", "=", altFaaliyet.altProje));

            if (altFaaliyet.butceKaynakNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATBUTCEKAYNAKPEBKOD", "KAYNAKNO", "=", altFaaliyet.butceKaynakNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Alt faaliyetle, performans faaliyeti arasında kurulmuş olan ve PEBFAALIYETPROJEPEBFON tablosunda tutulan bağı silen yordam.
        /// </summary>
        /// <param name="altFaaliyet">Bağı silinecek alt faaliyet bilgilerini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc PerfBagSil(AltFaaliyet altFaaliyet)
        {
            string sql = "DELETE FROM PEBFAALIYETPROJEPEBFON";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", altFaaliyet.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "PEBKOD", "=", altFaaliyet.pebKod.Replace(".", "")));
            return this.KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Alt faaliyetle, performans faaliyeti arasında bağ kurmak için PEBFAALIYETPROJEPEBFON tablosuna kayıt ekleyen yordam.
        /// </summary>
        /// <param name="altFaaliyet">Bağ eklenecek alt faaliyet ve performans faaliyeti bilgilerini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc PerfBagEkle(AltFaaliyet altFaaliyet)
        {
            string sql = "INSERT INTO PEBFAALIYETPROJEPEBFON(YIL, PEBKOD, MALIYETKURKOD, SABITMI, MALIYETKOD) VALUES (";
            sql += VeriErisim.DegerYap(altFaaliyet.yil) + ", ";
            sql += VeriErisim.DegerYap(altFaaliyet.pebKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(altFaaliyet.faaliyetBag.kurKod.Replace(".", "")) + ", ";

            sql += VeriErisim.DegerYap((int)altFaaliyet.faaliyetBag.sabitFaaliyet) + ", ";
            sql += VeriErisim.DegerYap((int)altFaaliyet.faaliyetBag.sabitFaaliyetKod) + ") ";

            return this.KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen alt faaliyetin ilişkili olduğu performans faaliyeti kodunu bulan yordam.
        /// </summary>
        /// <param name="altFaaliyet">İlişkili olduğu performans faaliyeti ilişkisi bulunacak, alt faaliyetin bilgisini tutan nesne</param>
        /// <returns>Alt faaliyetin, ilişkili olduğu performans faaliyeti kodu dizgisi döndürülür.</returns>

        public string PerfBagOku(AltFaaliyet altFaaliyet)
        {
            string sql = "Select MALIYETKURKOD+'-'+ LTRIM(STR(SABITMI))+'-'+LTRIM(STR(MALIYETKOD)) FROM PEBFAALIYETPROJEPEBFON";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", altFaaliyet.yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SENARYOKOD", "=", "0600"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "PEBKOD", "=", altFaaliyet.pebKod.Replace(".", "")));

            return veriErisim.TekKolonSQLStr(sql);
        }

        public IDataReader SektordeKullanilanlariBul(int yil, string senaryoKod, string sektorKod)
        {
            string sql = @"SELECT PK.PEBKOD, ABSPEBKOD.PEBAD from YATBUTCEKAYNAKPEBKOD PK 
                            INNER JOIN ABSPEBKOD ON PK.YIL=ABSPEBKOD.YIL AND PK.PEBKOD=ABSPEBKOD.PEBKOD 
                            INNER JOIN YATBUTCEKAYNAK ON YATBUTCEKAYNAK.YIL=PK.YIL AND YATBUTCEKAYNAK.SENARYOKOD=PK.SENARYOKOD AND YATBUTCEKAYNAK.KAYNAKNO=PK.KAYNAKNO 
                            INNER JOIN YATALTPROJE ON YATBUTCEKAYNAK.YIL=YATALTPROJE.YIL AND YATBUTCEKAYNAK.SENARYOKOD=YATALTPROJE.SENARYOKOD AND YATBUTCEKAYNAK.ALTPROJENO=YATALTPROJE.ALTPROJENO
                            INNER JOIN YATPROJE ON YATPROJE.YIL=YATALTPROJE.YIL AND YATPROJE.SENARYOKOD=YATALTPROJE.SENARYOKOD AND YATPROJE.PROJENO=YATALTPROJE.PROJENO";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("PK", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("PK", "SENARYOKOD", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATPROJE", "SEKTORNO", "LIKE", sektorKod.Replace(".", "") + "%"));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
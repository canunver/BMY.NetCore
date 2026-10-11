using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BKD;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMTAVANUSTU veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMTAVANUSTU : TABBHMODENEK
    {
    /// <summary>
    ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
    /// </summary>
    /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMTAVANUSTU(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMTAVANUSTU";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 1,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// FONKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 3,

            /// <summary>
            /// FINKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 4,

            /// <summary>
            /// EKOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,

            /// <summary>
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 6,

            /// <summary>
            /// ODENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK0 = 7,

            /// <summary>
            /// ODENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK1 = 8,

            /// <summary>
            /// ODENEK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK2 = 9,

            /// <summary>
            /// GEREKCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 10,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "ODENEK"
            , "ODENEK1"
            , "ODENEK2"
            , "GEREKCE"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Tertip tertip, string senaryoKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[2], tertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[3], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[4], tertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[5], tertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[6], tertip, 4);
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, tavanUstuOdenek dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, yapısında nesne</param>
        /// <param name="satirNo">tavanUstuOdenek dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(Tertip tertip, string senaryoKod, double odenek0, double odenek1, double odenek2, string gerekce)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tertip.yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(odenek0);
            sql += "," + VeriErisim.DegerYap(odenek1);
            sql += "," + VeriErisim.DegerYap(odenek2);
            sql += "," + VeriErisim.DegerYap(gerekce);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Select edilecek kayıtlara ait kıstas bilgisini tutan, yapısında nesne</param>
        /// <param name="senaryoKod">Select edilecek kayıtlara ait kıstas bilgisini tutan, yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(Tertip tertip, string senaryoKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[2], tertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[3], tertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[4], tertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[5], tertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[6], tertip, 4);
            return veriErisim.DataReaderDondur(sql);
        }

        public static string ProjeTavanUstuBulEko2SektorPeb2Peb4Sql(int yil, string senaryoKod, string ekoKod, string sektorKod, string pebKod)
        {
            string sektorAlan = "case when SEKTORNO IS NULL then '00' else SEKTORNO end";

            string odenekSorgu = "SELECT LEFT(O.EKOKOD,2), " + sektorAlan + ", LEFT(O.PEBKOD,2), O.PEBKOD, SUM(O.ODENEK), SUM(ODENEK1), SUM(ODENEK2)";
            odenekSorgu += " FROM Yatirim.dbo.YATALTPROJE AS AP LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON " + VeriErisim.Join("P", "AP", new string[] { "YIL", "SENARYOKOD", "PROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAK AS K ON " + VeriErisim.Join("AP", "K", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" }); ;
            odenekSorgu += " RIGHT OUTER JOIN  Yatirim.dbo.YATBUTCEKAYNAKPEBKOD AS BK ON " + VeriErisim.Join("K", "BK", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            odenekSorgu += " RIGHT OUTER JOIN BHMTAVANUSTU AS O ON " + VeriErisim.Join("BK", "O", new string[] { "YIL", "SENARYOKOD", "PEBKOD" });
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "SENARYOKOD", "=", senaryoKod));
            if (ekoKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "EKOKOD", "LIKE", ekoKod.Replace(".", "") + "%"));

            if (sektorKod != null)
            {
                if (sektorKod == "00")
                    odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", "SEKTORNO IS NULL");
                else
                    odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("", "SEKTORNO", "=", sektorKod.Replace(".", "")));
            }

            if (pebKod != null)
                odenekSorgu = VeriErisim.KosulEkle(odenekSorgu, "AND", VeriErisim.KosulOl("O", "PEBKOD", "LIKE", pebKod.Replace(".", "") + "%"));

            odenekSorgu += " group by LEFT(O.EKOKOD,2), SEKTORNO, LEFT(O.PEBKOD,2), O.PEBKOD order by 1,2,3,4";

            return odenekSorgu;
        }

        internal Sonuc Kopyala(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") SELECT ";
            sql += VeriErisim.DegerYap(hedefTertip.yil) + " AS " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += VeriErisim.DegerYap(hedefAsama.Replace(".", "")) + " AS " + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FONKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FINKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PEBKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.ODENEK0] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.ODENEK1] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.ODENEK2] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.GEREKCE] + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", kaynakAsama.Replace(".", "")));

            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], hedefTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], hedefTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], hedefTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], hedefTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], hedefTertip, 4);

            return KomutCalistir(sql, 0);
        }

        public override Sonuc Ekle(int yil, string senaryoKod, int teklifYil, Tertip tertip, double odenek)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", ""));
            if (tertip.kodlar.Length > 4)
                sql += "," + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", ""));
            else
                sql += "," + VeriErisim.DegerYap("0");
            sql += "," + VeriErisim.DegerYap(odenek);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public override IDataReader TertipListesi(int yil, string senaryoKod, int teklifYil, string[] kodlar)
        {
            throw new NotImplementedException();
        }

        public override Sonuc CokYilliOdenekSil(int yil, string senaryoKod, Tertip tertip)
        {
            Sonuc sonuc;
            string sql = "DELETE FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", tertip.kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", tertip.kodlar[3].Replace(".", "")));
            if (tertip.kodlar.Length > 4)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", tertip.kodlar[4].Replace(".", "")));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", "0"));
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        public override Sonuc DegisimZamaniAyarla(TABDEGISIMZAMAN tabZaman, int yil, string senaryoKod, string kurKod, string kullaniciKod)
        {
            return tabZaman.OdenekZamanKaydet(new DegisimZaman(yil, senaryoKod, kurKod, ENUMZamanDamgaTur.TAVANUSTU, kullaniciKod));
        }

        public override Sonuc OdenekKopyala(Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama)
        {
            throw new NotImplementedException();
        }

        public override Sonuc CokYilliOdenekSilAciklama(int yil, string senaryoKod, Tertip tertip)
        {
            return new Sonuc();
        }

        public override Sonuc EkleAciklama(int yil, string senaryoKod, int i, Tertip tertip, string aciklama)
        {
            throw new NotImplementedException();
        }
    }
}

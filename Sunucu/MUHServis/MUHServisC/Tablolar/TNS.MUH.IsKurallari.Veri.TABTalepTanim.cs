using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHTalepTanimForm veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTalepTanimForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTalepTanimForm(VeriErisim veriErisim) : base(veriErisim) { }


        /// <summary>
        /// create table MUHTalepTanimForm
        /// (
        /// 	KOD INT, 
        /// 	JSON varchar(MAX),
        /// 	KAYITEDEN varchar(50),
        /// 	KAYITTARIHI DATETIME,
        /// 	primary key (KOD)
        /// )
        /// </summary>
        static string TABLOAD = "MUHTKDKTALEPTANIMFORM";

        enum ENUMAlanlar
        {
            KOD = 0,
            YIL = 1,
            AY = 2,
            TALEPTUR = 3,
            KAYITEDEN = 4,
            KAYITTARIHI = 5
        }
        static string[] ALANLAR = { "KOD", "YIL", "AY", "TALEPTUR", "KAYITEDEN", "KAYITTARIHI" };

        static string TABLOADDETAY = "MUHTKDKTALEPTANIMDETAY";

        enum ENUMAlanlarDetay
        {
            KOD = 0,
            TALEPKOD = 1,
            HESAPKOD = 2,
        }
        static string[] ALANLARDETAY = { "KOD", "TALEPKOD", "EKOKOD", "EKOADI" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(TalepTanimForm bilgi)
        {
            //bilgi.kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.yil) + ", ";
            sql += VeriErisim.DegerYap(bilgi.ay) + ", ";
            sql += VeriErisim.DegerYap(bilgi.talepTur) + ", ";
            sql += VeriErisim.DegerYap(bilgi.kayitEden) + ", ";
            sql += VeriErisim.DegerYap(bilgi.kayitTarihi) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanında güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Kayıt bilgisini tutan nesne</param>
        /// <returns>Komutun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(TalepTanimForm bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], bilgi.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AY], bilgi.ay) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TALEPTUR], bilgi.talepTur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TalepTanimForm bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(TalepTanimForm bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";
            if (bilgi.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", bilgi.ay));
            if (bilgi.talepTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPTUR], "=", bilgi.talepTur));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY YIL DESC, AY DESC, TALEPTUR";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(TalepTanimFormDetay bilgi)
        {
            //bilgi.kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);

            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.talepKod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.ekoKod) + ", ";
            sql += VeriErisim.DegerYap(bilgi.ekoAdi) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanında güncelleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Kayıt bilgisini tutan nesne</param>
        /// <returns>Komutun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleDetay(TalepTanimFormDetay bilgi)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HESAPKOD], bilgi.ekoKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KOD], "=", bilgi.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bilgi.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(TalepTanimForm bilgi)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPKOD], "=", bilgi.kod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleDetay(TalepTanimFormDetay bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDETAY) + " FROM " + TABLOADDETAY;
            string kosul = "";
            if (bilgi.kod != "")
                kosul = VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KOD], "=", bilgi.kod);
            if (bilgi.talepKod != "")
                kosul = VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TALEPKOD], "=", bilgi.talepKod);

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY EKOKOD";

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
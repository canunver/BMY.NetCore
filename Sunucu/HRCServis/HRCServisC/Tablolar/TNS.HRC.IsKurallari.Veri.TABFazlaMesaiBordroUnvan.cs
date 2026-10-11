using System;
using System.Data;
using TNS.HRC.IsKurallari;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Fazla Mesai Bordro bilgisini tutan PSLFAZLAMESAIBORDRO veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABFazlaMesaiBordroUnvan : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFazlaMesaiBordroUnvan(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Fazla Mesai İcmal tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "PSLFAZLAMESAIBORDROUNVAN";

        /// <summary>
        /// Fazla Mesai Bordro tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
        {
            YIL = 0,
            SURECNO = 1,
            ILGILIYIL = 2,
            ILGILIAY = 3,
            TCKIMLIKNO = 4,
            UNVANKOD = 5,
            TOPLAMSAAT = 6,
            KANUNIIZIN = 7,
            YURTDISIGOREV = 8,
            RESMIDINIGUN = 9,
            KESILENSAAT = 10,
            TOPLAMKESILENSAAT = 11,
            HESAPLANACAKSAAT = 12,
            SAATUCRETI = 13,
            TOPLAMTUTAR = 14,
            FAZLAMESAISAAT = 15,
            GENELTOPLAMSAAT = 16,
            AD = 17,
            SOYAD = 18,
            UNVANAD = 19,
            DAMGAVERGISI = 20,
            SGK195 = 21,
            SGK14 = 22,
            KESINTI = 23,
            KESINTITOPLAM = 24,
            NETODENECEK = 25,
        }

        static string[] ALANLAR = { "YIL", "SURECNO", "ILGILIYIL", "ILGILIAY", "TCKIMLIKNO", "UNVANKOD", "TOPLAMSAAT", "KANUNIIZIN", "YURTDISIGOREV", "RESMIDINIGUN", "KESILENSAAT", "TOPLAMKESILENSAAT", "HESAPLANACAKSAAT", "SAATUCRETI", "TOPLAMTUTAR", "FAZLAMESAISAAT", "GENELTOPLAMSAAT", "AD", "SOYAD", "UNVANAD", "DAMGAVERGISI", "SGK195", "SGK14", "KESINTI", "KESINTITOPLAM", "NETODENECEK" };


        public IDataReader Listele(FazlaMesaiBordroUnvan f, bool bankaListesi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));
            if (f.ilgiliYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIYIL], "=", f.ilgiliYil));
            if (f.ilgiliAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILGILIAY], "=", f.ilgiliAy));
            if (!string.IsNullOrEmpty(f.tcKimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", f.tcKimlikNo));


            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        public Sonuc Kaydet(FazlaMesaiBordroUnvan f)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(f.yil) + ",";
            sql += VeriErisim.DegerYap(f.surecNo) + ",";
            sql += VeriErisim.DegerYap(f.ilgiliYil) + ",";
            sql += VeriErisim.DegerYap(f.ilgiliAy) + ",";
            sql += VeriErisim.DegerYap(f.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(f.unvanKod) + ",";
            sql += VeriErisim.DegerYap(f.toplamSaat) + ",";
            sql += VeriErisim.DegerYap(f.kanuniIzin) + ",";
            sql += VeriErisim.DegerYap(f.yurtDisiGorev) + ",";
            sql += VeriErisim.DegerYap(f.resmiDiniGun) + ",";
            sql += VeriErisim.DegerYap(f.kesilenSaat) + ",";
            sql += VeriErisim.DegerYap(f.toplamKesilenSaat) + ",";
            sql += VeriErisim.DegerYap(f.hesaplanacakSaat) + ",";
            sql += VeriErisim.DegerYap(f.saatUcreti) + ",";
            sql += VeriErisim.DegerYap(f.toplamTutar) + ",";
            sql += VeriErisim.DegerYap(f.fazlaMesaiSaat) + ",";
            sql += VeriErisim.DegerYap(f.genelToplamSaat) + ",";
            sql += VeriErisim.DegerYap(f.ad) + ",";
            sql += VeriErisim.DegerYap(f.soyad) + ",";
            sql += VeriErisim.DegerYap(f.unvanAd) + ",";
            sql += VeriErisim.DegerYap(f.damgaVergisi) + ",";
            sql += VeriErisim.DegerYap(f.sgk195) + ",";
            sql += VeriErisim.DegerYap(f.sgk14) + ",";
            sql += VeriErisim.DegerYap(f.kesinti) + ",";
            sql += VeriErisim.DegerYap(f.kesintiToplam) + ",";
            sql += VeriErisim.DegerYap(f.netOdenecek) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(FazlaMesaiBordroUnvan f)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", f.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", f.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}

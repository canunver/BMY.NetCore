using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHTAAHHUTBORCLAR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhutBorclar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutBorclar(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUTBORCLAR";

        enum ENUMAlanlar
        {
            KOD=0,
            TAAHHUTKOD = 1,
            DOSYANO=2,
            ALACAKLIADSOYAD = 3,
            TEMLIKACANVERGINO = 4,
            TEMLIKVERENVERGINO = 5,
            TEMLIKTUTARI = 6,
            ISLEMYAPANAD = 7,
            ISLEMYAPANKOD = 8,
            ISLEMTARIHI =9,
        }

        static string[] ALANLAR ={  "KOD", "TAAHHUTKOD", "DOSYANO", "ALACAKLIADSOYAD", "TEMLIKACANVERGINO", "TEMLIKVERENVERGINO","TEMLIKTUTARI",
                                     "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI"};

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="teminatlar">The teminatlar.</param>
        /// <returns></returns>
        public Sonuc Kaydet(TaahhutBorclar bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.taahhutKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.dosyaNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.alacakliAdSoyad) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.temlikAcanVergiNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.temlikVerenVergiNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.temlikTutari) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemYapanAd) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemYapanKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemTarihi) + ") ";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Sil(TaahhutBorclar bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            string kosul = "";

            if (bilgi.kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod + " "));
            if (bilgi.taahhutKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod + " "));
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(TaahhutBorclar bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.taahhutKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        public int SiraNoBul(int kod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
           
               // sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
                return (int)veriErisim.TekKolonSQLDbl(sql) + 1;
        }

        /// <summary>
        /// Kayits the variable mi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public bool KayitVarMi(TaahhutBorclar bilgi)
        {
            string sql = "SELECT COUNT(*)  FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (veriErisim.TekKolonSQLInt(sql) > 0)
                return true;
            else 
                return false;
        }

    }
}

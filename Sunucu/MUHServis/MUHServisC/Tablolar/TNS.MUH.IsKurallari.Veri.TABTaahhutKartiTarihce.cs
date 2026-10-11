using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHTAAHHUTKARTITARIHCE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhutKartiTarihce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutKartiTarihce(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUTKARTITARIHCE";

        enum ENUMAlanlar
        {
            KOD = 0,
            TAAHHUTKOD = 1,
            ISLEMADI = 2,
            ISLEMKODU = 3,
            ISLEMYAPANAD = 4,
            ISLEMYAPANKOD = 5,
            ISLEMTARIHI = 6,
            ACIKLAMA = 7
        }
        static string[] ALANLAR ={ "KOD", "TAAHHUTKOD", "ISLEMADI", "ISLEMKODU", "ISLEMYAPANAD", "ISLEMYAPANKOD","ISLEMTARIHI",
                                     "ACIKLAMA" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="taahhutTarihce">The taahhutTarihce.</param>
        /// <returns></returns>
        public Sonuc Kaydet(TaahhutKartiTarihce taahhutTarihce)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(taahhutTarihce.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(taahhutTarihce.taahhutKod) + ", ";
            sql = sql + VeriErisim.DegerYap(taahhutTarihce.islemAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(taahhutTarihce.islemKod) + ", ";
            sql = sql + VeriErisim.DegerYap(taahhutTarihce.islemYapanAd) + ", ";
            sql = sql + VeriErisim.DegerYap(taahhutTarihce.islemYapanKod) + ", ";
            sql = sql + VeriErisim.DegerYap(taahhutTarihce.islemTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(taahhutTarihce.aciklama) + ")";

            sonuc = KomutCalistir(sql, 1);

            sonuc.anahtar = taahhutTarihce.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="taahhutTarihce">taahhutTarihce.</param>
        /// <returns></returns>
        public Sonuc Sil(TaahhutKartiTarihce taahhutTarihce)
        {
            string sql = "DELETE FROM " + TABLOAD;
            string kosul = "";

            if (taahhutTarihce.kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", taahhutTarihce.kod + " "));
            if (taahhutTarihce.taahhutKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", taahhutTarihce.taahhutKod + " "));
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(TaahhutKartiTarihce bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.taahhutKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        /// <summary>
        /// Siras the no bul.
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public int SiraNoBul()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return (int)veriErisim.TekKolonSQLDbl(sql) + 1;
        }

    }
}

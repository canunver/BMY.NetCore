using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHVERILENAVANSLARVEMAHSUPLAR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhutAvanslar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutAvanslar(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUTAVANSLAR";

        enum ENUMAlanlar
        {
            TAAHHUTKOD = 0,
            YIL = 1,
            BELGENO = 2,
            TARIH = 3,
            NUMARA = 4,
            VERILENAVANS = 5,
            MAHSUPEDILENAVANS = 6,
            ISLEMTARIHI = 7,
            ISLEMYAPANKOD = 8,
            ISLEMYAPANAD = 9,
        }

        static string[] ALANLAR ={  "TAAHHUTKOD", "YIL", "BELGENO", "TARIH", "NUMARA","VERILENAVANS",
                                     "MAHSUPEDILENAVANS", "ISLEMTARIHI", "ISLEMYAPANKOD", "ISLEMYAPANAD"};

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public Sonuc Kaydet(TaahhutAvanslar form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(form.taahhutKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(form.belgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(form.tarih) + ", ";
            sql = sql + VeriErisim.DegerYap(form.numara) + ", ";
            sql = sql + VeriErisim.DegerYap(form.verilenAvans) + ", ";
            sql = sql + VeriErisim.DegerYap(form.mahsupEdilenAvans) + ", ";
            sql = sql + VeriErisim.DegerYap(form.islemTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(form.islemYapanKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.islemYapanAd) + ") ";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(TaahhutAvanslar bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.taahhutKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGENO];

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        /// <summary>
        /// Kayits the variable mi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public bool KayitVarMi(TaahhutAvanslar bilgi)
        {
            string sql = "SELECT COUNT(*)  FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", bilgi.belgeNo));
            if (veriErisim.TekKolonSQLInt(sql) > 0)
                return true;
            else 
                return false;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Sil(TaahhutAvanslar bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            string kosul = "";

            if (bilgi.taahhutKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod + " "));
            if (bilgi.yil != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil + " "));
            if (!string.IsNullOrEmpty(bilgi.belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", bilgi.belgeNo + " "));
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

    }
}

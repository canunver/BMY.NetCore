using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHALINANTEMINATLAR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhutTeminatlar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutTeminatlar(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUTTEMINATLAR";

        enum ENUMAlanlar
        {
            TAAHHUTKOD = 0,
            YIL = 1,
            DOSYANO = 2,
            TARIH = 3,
            NUMARA = 4,
            ACIKLAMA = 5,
            TUTAR = 6,
            ISLEMTARIHI = 7,
            ISLEMYAPANKOD = 8,
            ISLEMYAPANAD = 9,
        }

        static string[] ALANLAR ={  "TAAHHUTKOD", "YIL", "DOSYANO", "TARIH", "NUMARA","ACIKLAMA",
                                     "TUTAR", "ISLEMTARIHI", "ISLEMYAPANKOD", "ISLEMYAPANAD"};

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="teminatlar">The teminatlar.</param>
        /// <returns></returns>
        public Sonuc Kaydet(TaahhutTeminatlar teminatlar)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(teminatlar.taahhutKod) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.dosyaNo) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.tarih) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.numara) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.tutar) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.islemTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.islemYapanKod) + ", ";
            sql = sql + VeriErisim.DegerYap(teminatlar.islemYapanAd) + ") ";

            sonuc = KomutCalistir(sql, 1);

            //if (!sonuc.islemSonuc)
            //{
            //    veriErisim.hata = "";

            //    sql = "UPDATE " + TABLOAD + " SET ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], teminatlar.taahhutKod) + ", ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], teminatlar.yil) + ", ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], teminatlar.dosyaNo) + ", ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], teminatlar.tarih) + ", ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], teminatlar.aciklama) + ", ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTAR], teminatlar.tutar) + ", ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], teminatlar.islemTarihi) + ", ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], teminatlar.islemYapanKod) + ", ";
            //    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], teminatlar.islemYapanAd);
            //    //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", teminatlar.kod));
            //   // sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", teminatlar.taahhutKod));
            //    sonuc = KomutCalistir(sql, 1);
            //}
            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Sil(TaahhutTeminatlar bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            string kosul = "";

            if (bilgi.taahhutKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod + " "));
            if (bilgi.yil != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil + " "));
            if (!string.IsNullOrEmpty(bilgi.dosyaNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", bilgi.dosyaNo + " "));
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(TaahhutTeminatlar bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.taahhutKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DOSYANO];

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        //public int SiraNoBul(int kod)
        //{
        //    string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
        //    if (kod != 0)
        //    {
        //        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
        //        return (int)veriErisim.TekKolonSQLDbl(sql);
        //    }
        //    else
        //        return (int)veriErisim.TekKolonSQLDbl(sql)+1;
        //}

        /// <summary>
        /// Kayits the variable mi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public bool KayitVarMi(TaahhutTeminatlar bilgi)
        {
            string sql = "SELECT COUNT(*)  FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTKOD], "=", bilgi.taahhutKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", bilgi.dosyaNo));
            if (veriErisim.TekKolonSQLInt(sql) > 0)
                return true;
            else
                return false;
        }

    }
}

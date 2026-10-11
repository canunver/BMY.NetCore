using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHYANSITMA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYansitma : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYansitma(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHYANSITMA";
        enum ENUMAlanlar
        {
            YIL = 0,
            HKOD = 1,
            YKOD = 2,
            SIRANO = 3,
        }
        static string[] ALANLAR = { "YIL", "HKOD", "YKOD", "SIRANO" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yansitma">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(HesapPlaniYansitma yansitma)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yansitma.yil) + ", ";
            sql += VeriErisim.DegerYap(yansitma.hesapKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(yansitma.yansitmaHesapKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(yansitma.satir) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yansitma.yil;
                sql += ", " + ALANLAR[(int)ENUMAlanlar.HKOD] + "='" + yansitma.hesapKod.Replace(".", "") + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.YKOD] + "='" + yansitma.yansitmaHesapKod.Replace(".", "") + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.SIRANO] + "=" + yansitma.satir;
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yansitma.yil);
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HKOD], "=", yansitma.hesapKod.Replace(".", ""));
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YKOD], "=", yansitma.yansitmaHesapKod.Replace(".", ""));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yansitma">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(HesapPlaniYansitma yansitma)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yansitma.yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HKOD], "=", yansitma.hesapKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YKOD], "=", yansitma.yansitmaHesapKod.Replace(".", ""));

            return KomutCalistir(sql, 0);
        }


        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilHepsini(int yil)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yansitma">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(HesapPlaniYansitma yansitma, bool sadeceEsitler)
        {
            string like = "LIKE";
            string yuzde = "%";

            if (sadeceEsitler)
            {
                like = "=";
                yuzde = "";
            }
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yansitma.yil));

            if (yansitma.hesapKod != "")
            {
                string[] yler = yansitma.hesapKod.Split(',');
                string kosul = "";
                for (int i = 0; i < yler.Length; i++)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HKOD], like, yler[i].Replace(".", "") + yuzde));
                }
                if (kosul != "")
                    sql += " AND (" + kosul + ")";
            }

            if (yansitma.yansitmaHesapKod != "")
            {
                string[] yler = yansitma.yansitmaHesapKod.Split(',');
                string kosul = "";
                for (int i = 0; i < yler.Length; i++)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YKOD], like, yler[i].Replace(".", "") + yuzde));
                }
                if (kosul != "")
                    sql += " AND (" + kosul + ")";
            }

            if (yansitma.satir > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", yansitma.satir.ToString()));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.HKOD] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO] + "," + ALANLAR[(int)ENUMAlanlar.YKOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }

        /// <summary>
        /// Yansitma verilerini parametre olarak verilen yıldan hedef yıla aktaran yordam.
        /// </summary>
        /// <param name="kaynakYil">Kaynak yıl.</param>
        /// <param name="hedefYil">Hedef yıl.</param>
        /// <returns></returns>
        public Sonuc YansitmaAktar(int kaynakYil, int hedefYil)
        {
            string sql = "";

            //Hedef yıldaki veri sil
            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hedefYil);
            KomutCalistir(sql, 0);


            sql = "INSERT INTO " + TABLOAD + " (YIL, HKOD, YKOD, SIRANO) ";
            sql += " SELECT " + hedefYil + ", HKOD ,YKOD, SIRANO FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yansitma">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleTasinirEkoKod(HesapPlaniYansitma yansitma)
        {
            string like = "LIKE";
            string yuzde = "%";

            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HKOD] + "," +
                              " MUHHESAPPLANI.ACIKLAMA FROM " + TABLOAD;
            sql += " INNER JOIN MUHHESAPPLANI ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + "MUHHESAPPLANI.YIL AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HKOD] +"="+ "MUHHESAPPLANI.HESAPKOD ";
    
            if (yansitma.hesapKod != "")
            {
                string[] yler = yansitma.hesapKod.Split(',');
                string kosul = "";
                for (int i = 0; i < yler.Length; i++)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HKOD], like, yler[i].Replace(".", "") + yuzde));
                }
                if (kosul != "")
                    sql += " AND (" + kosul + ")";

                sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] +","+ TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HKOD] +","+ "MUHHESAPPLANI.ACIKLAMA";
                sql += " HAVING " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + yansitma.yil;
            }

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}

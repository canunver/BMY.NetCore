using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHHESAPPLANIKISIT veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHesapPlaniKisit : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHesapPlaniKisit(VeriErisim veriErisim) : base(veriErisim) { }

        /*
         * 
create table MUHHESAPPLANIKISIT(
    KOD INT,
    YIL INT,
    MUHASEBEKOD INT,
    BIRIMKOD VARCHAR(20),
    HESAPKOD VARCHAR(200),
    HESAPADI VARCHAR(200),
    ISLEMYAPANAD VARCHAR(200),
    ISLEMYAPANKOD VARCHAR(200),
    ISLEMTARIHI DATETIME,
    PRIMARY KEY(KOD))
         * */

        static string TABLOAD = "MUHHESAPPLANIKISIT";

        enum ENUMAlanlar
        {
            KOD = 0,
            YIL = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            HESAPKOD = 4,
            HESAPADI = 5,
            ISLEMYAPANAD = 6,
            ISLEMYAPANKOD = 7,
            ISLEMTARIHI = 8,
        }

        static string[] ALANLAR = { "KOD", "YIL", "MUHASEBEKOD", "BIRIMKOD", "HESAPKOD", "HESAPADI", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kisit">The kisit.</param>
        /// <returns></returns>
        public Sonuc Kaydet(HesapPlaniKisit kisit)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(kisit.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(kisit.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kisit.muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(kisit.birimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(kisit.hesapKod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kisit.hesapAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(kisit.islemYapanAd) + ", ";
            sql = sql + VeriErisim.DegerYap(kisit.islemYapanKod) + ", ";
            sql = sql + VeriErisim.DegerYap(kisit.islemTarihi) + ") ";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], kisit.kod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], kisit.yil) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], kisit.muhasebeKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], kisit.birimKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], kisit.hesapKod.Replace(".", "")) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], kisit.islemYapanAd) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], kisit.islemYapanKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], kisit.islemTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPADI], kisit.hesapAdi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kisit.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            sonuc.anahtar = kisit.kod.ToString();
            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public Sonuc Sil(int kod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(HesapPlaniKisit bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.yil != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebeKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            if (!string.IsNullOrEmpty(bilgi.birimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birimKod));
            if (!string.IsNullOrEmpty(bilgi.hesapKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", bilgi.hesapKod));
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Muhasebes the belgesi kontrol.
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="hesapKodlari">The hesap kodlari.</param>
        /// <returns></returns>
        public IDataReader MuhasebeBelgesiKontrol(int yil, string hesapKodlari)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (!string.IsNullOrEmpty(hesapKodlari))
            {
                string[] hesapKodu = hesapKodlari.Split(';');
                foreach (string hesap in hesapKodu)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", hesap));
                }

                if (kosul != "")
                    sql += " AND (" + kosul + ")";
            }

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Siras the no bul.
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public int SiraNoBul(int kod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;

            if (kod != 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
                return (int)veriErisim.TekKolonSQLDbl(sql);
            }
            else
                return (int)veriErisim.TekKolonSQLDbl(sql) + 1;
        }

        /// <summary>
        /// Aktars the specified kaynak yil.
        /// </summary>
        /// <param name="kaynakYil">The kaynak yil.</param>
        /// <param name="hedefYil">The hedef yil.</param>
        /// <returns></returns>
        public Sonuc Aktar(int kaynakYil, int hedefYil)
        {
            //Hedef yıldaki veri sil
            string sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hedefYil);
            return KomutCalistir(sql, 0);
        }
    }
}

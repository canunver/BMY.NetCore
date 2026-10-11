using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMUTEMETTANIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMutemetTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMutemetTanim(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHMUTEMETTANIM";

        enum ENUMAlanlar
        {
            KOD = 0,
            YIL = 1,
            TCKIMLIKNO = 2,
            MUHASEBEKOD = 3,
            BIRIMKOD = 4,
            HESAPKOD = 5,
            TUTAR = 6,
            ISLEMYAPANAD = 7,
            ISLEMYAPANKOD = 8,
            ISLEMTARIHI = 9,
            KISIADI = 10,
            HESAPADI = 11,
        }

        static string[] ALANLAR = { "KOD", "YIL", "TCKIMLIKNO", "MUHASEBEKOD", "BIRIMKOD", "HESAPKOD", "TUTAR", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI", "KISIADI", "HESAPADI" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="mutemet">The mutemet.</param>
        /// <returns></returns>
        public Sonuc Kaydet(MutemetTanim mutemet)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(mutemet.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.yil) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.tcKimlikNo) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.birimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.hesapKod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.tutar) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.islemYapanAd) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.islemYapanKod) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.islemTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.kisiAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(mutemet.hesapAdi) + ") ";



            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], mutemet.kod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], mutemet.yil) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], mutemet.tcKimlikNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], mutemet.muhasebeKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], mutemet.birimKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], mutemet.hesapKod.Replace(".", "")) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTAR], mutemet.tutar) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], mutemet.islemYapanAd) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], mutemet.islemYapanKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], mutemet.islemTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISIADI], mutemet.kisiAdi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPADI], mutemet.hesapAdi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", mutemet.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            sonuc.anahtar = mutemet.kod.ToString();
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
        public IDataReader Listele(MutemetTanim bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.yil != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (!string.IsNullOrEmpty(bilgi.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", bilgi.tcKimlikNo));
            if (!string.IsNullOrEmpty(bilgi.kisiAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISIADI], "LIKE", "%" + bilgi.kisiAdi + "%"));
            if (bilgi.muhasebeKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            if (!string.IsNullOrEmpty(bilgi.birimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birimKod));
            if (!string.IsNullOrEmpty(bilgi.hesapKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", bilgi.hesapKod));
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
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

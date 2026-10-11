using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHBORCLARVETEMLIKLER veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTaahhutBorclarTanim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTaahhutBorclarTanim(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHTAAHHUTBORCLARTANIM";

        enum ENUMAlanlar
        {
            KOD = 0,
            KONU = 1,
            ALACAKLIADSOYAD=2,
            TEMLIKACANVERGINO = 3,
            TEMLIKVERENVERGINO = 4,
            TEMLIKTUTARI = 5,
            ACIKLAMA = 6,
            NOTERONAYTARIHI = 7,
            NOTERONAYNO = 8,
            ISLEMYAPANAD = 9,
            ISLEMYAPANKOD = 10,
            ISLEMTARIHI = 11
        }

        static string[] ALANLAR ={ "KOD", "KONU", "ALACAKLIADSOYAD", "TEMLIKACANVERGINO", "TEMLIKVERENVERGINO", "TEMLIKTUTARI", "ACIKLAMA",
                                     "NOTERONAYTARIHI", "NOTERONAYNO", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI"};

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="borclar">The borclar.</param>
        /// <returns></returns>
        public Sonuc Kaydet(TaahhutBorclarTanim borclar)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(borclar.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.konu) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.alacakliAdSoyad) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.temlikAcanVergiNo) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.temlikVerenVergiNo) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.temlikTutari) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.noterOnayTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.noterOnayNo) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.islemYapanAd) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.islemYapanKod) + ", ";
            sql = sql + VeriErisim.DegerYap(borclar.islemTarihi) + ") ";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONU], borclar.konu) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALACAKLIADSOYAD], borclar.alacakliAdSoyad) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMLIKACANVERGINO], borclar.temlikAcanVergiNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMLIKVERENVERGINO], borclar.temlikVerenVergiNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMLIKTUTARI], borclar.temlikTutari) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], borclar.aciklama) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NOTERONAYTARIHI], borclar.noterOnayTarihi) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NOTERONAYNO], borclar.noterOnayNo) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], borclar.islemYapanAd) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], borclar.islemYapanKod) + ", ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], borclar.islemTarihi);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", borclar.kod));
                sonuc = KomutCalistir(sql, 1);
                sonuc.anahtar = borclar.kod.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kod">kod.</param>
        /// <returns></returns>
        public Sonuc Sil(int kod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod + " ");

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(TaahhutBorclarTanim bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if(!string.IsNullOrEmpty(bilgi.konu))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KONU], "LIKE", "%" + bilgi.konu + "%"));
            if (!string.IsNullOrEmpty(bilgi.temlikAcanVergiNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEMLIKACANVERGINO], "=", bilgi.temlikAcanVergiNo ));
            if (!string.IsNullOrEmpty(bilgi.temlikVerenVergiNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEMLIKVERENVERGINO], "=", bilgi.temlikVerenVergiNo));
            if (!string.IsNullOrEmpty(bilgi.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + bilgi.aciklama + "%"));
          
            if (!bilgi.noterOnayTarihi.isNull)
            {
                if (bilgi.noterOnayTarihiSorgu.isNull)
                    bilgi.noterOnayTarihiSorgu = new TNSDateTime(bilgi.noterOnayTarihi.Oku());

                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.NOTERONAYTARIHI], ">=", bilgi.noterOnayTarihi));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.NOTERONAYTARIHI], "<=", new TNSDateTime( bilgi.noterOnayTarihiSorgu.Oku())));
            }

            if (!string.IsNullOrEmpty(bilgi.noterOnayNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.NOTERONAYNO], "=" , bilgi.noterOnayNo));

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

    }
}

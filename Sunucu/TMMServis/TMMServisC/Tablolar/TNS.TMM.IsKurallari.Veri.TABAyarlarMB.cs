using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Ambar bilgilerini tutan TASAYARLARMB veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAyarlarMB : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAyarlarMB(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASAYARLARMB";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// Harcama Birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 1,
            /// <summary>
            /// Ambar kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 2,
            ISLEMYAPAN = 3,
            ISLEMTARIH = 4,
            FISGIRISKAPANISTARIH = 5,
            MUTEMMIMBITISTARIH = 6,
            AONAYIMUHASEBEKONTROLU = 7
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "ISLEMYAPAN", "ISLEMTARIH", "FISGIRISKAPANISTARIH", "MUTEMMIMBITISTARIH", "AONAYIMUHASEBEKONTROLU" };

        public Sonuc Kaydet(AyarlarMB form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(form.harcamaKod) + ",";
            sql += VeriErisim.DegerYap(form.ambarKod) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemTarihi) + ",";
            sql += VeriErisim.DegerYap(form.fisGirisKapanisTarihi) + ",";
            sql += VeriErisim.DegerYap(form.mutemmimBitisTarihi) + ",";
            sql += VeriErisim.DegerYap(form.aOnayiMuhasebeKontrolu) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], form.islemYapan) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], form.islemTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FISGIRISKAPANISTARIH], form.fisGirisKapanisTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUTEMMIMBITISTARIH], form.mutemmimBitisTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AONAYIMUHASEBEKONTROLU], form.aOnayiMuhasebeKontrolu);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));

                veriErisim.hata = "";
                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public IDataReader Listele(AyarlarMB form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("AYARLAR", ALANLAR) + ", M.AD AS MUHASEBEAD, H.AD AS HARCAMABIRIMAD, A.AD AS AMBARAD FROM " + TABLOAD + " AYARLAR ";
            sql += "INNER JOIN TASMUHASEBE M ON M.KOD = AYARLAR.MUHASEBEKOD ";
            sql += "INNER JOIN TASHARCAMABIRIM H ON H.MUHASEBEKOD = AYARLAR.MUHASEBEKOD AND H.KOD = AYARLAR.HARCAMABIRIMKOD ";
            sql += "INNER JOIN TASAMBAR A ON A.MUHASEBEKOD = AYARLAR.MUHASEBEKOD AND A.HARCAMABIRIMKOD = AYARLAR.HARCAMABIRIMKOD AND A.KOD = AYARLAR.AMBARKOD ";

            string kosul = "";
            if (form.muhasebeKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("AYARLAR", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], " = ", form.muhasebeKod));
            if (form.harcamaKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("AYARLAR", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Replace(".", "")));
            if (form.ambarKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("AYARLAR", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(AyarlarMB form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", form.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", form.ambarKod));

            return KomutCalistir(sql, 1);
        }

    }
}

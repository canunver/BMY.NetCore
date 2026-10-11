using TNS.Veri;
using System;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHASEBAT veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMuhasebat : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMuhasebat(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHASEBAT";
        enum ENUMAlanlar
        {
            YIL = 0,
            AY = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            HESAPKOD = 4,
            KURKOD = 5,
            FONKOD = 6,
            FINKOD = 7,
            EKOKOD = 8,
            BORC = 9,
            ALACAK = 10,
            PEBKOD = 11,
        }

        static string[] ALANLAR = { "YIL", "AY", "MUHASEBEKOD", "BIRIMKOD", "HESAPKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "BORC", "ALACAK", "PEBKOD" };

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">Ay</param>
        /// <param name="hesapKod">Hesap Kodu</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, int ay, string hesapKod, int muhasebeKod, string birimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", ay));
            if(!string.IsNullOrEmpty(hesapKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "LIKE ", hesapKod + "%"));
            if (muhasebeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
            if (!string.IsNullOrWhiteSpace(birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="ay">Ay</param>
        /// <param name="muhasebe">Muhasebe Kodu</param>
        /// <param name="birim">Birim kodu</param>
        /// <param name="hesapKod">Hesap Kodu</param>
        /// <param name="kurKod">Kurumsal kod</param>
        /// <param name="fonKod">Fonksiyonel kod</param>
        /// <param name="finKod">Finans kod</param>
        /// <param name="ekoKod">Ekonomik kod</param>
        /// <param name="borc">Borç</param>
        /// <param name="alacak">Alacak</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(int yil, int ay, int muhasebe, string birim, string hesapKod, string kurKod, string fonKod, string finKod, string ekoKod, string pebKod, double borc, double alacak)
        {
            string sql;

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(ay) + ", ";
            sql = sql + VeriErisim.DegerYap(muhasebe) + ", ";
            sql = sql + VeriErisim.DegerYap(birim) + ", ";
            sql = sql + VeriErisim.DegerYap(hesapKod) + ", ";
            sql = sql + VeriErisim.DegerYap(kurKod) + ", ";
            sql = sql + VeriErisim.DegerYap(fonKod) + ", ";
            sql = sql + VeriErisim.DegerYap(finKod) + ", ";
            sql = sql + VeriErisim.DegerYap(ekoKod) + ", ";
            sql = sql + VeriErisim.DegerYap(borc) + ", ";
            sql = sql + VeriErisim.DegerYap(alacak) + ", ";
            sql = sql + VeriErisim.DegerYap(pebKod) + ")";

            return KomutCalistir(sql, 1);
        }

        public static string HarcamaSorgu(int yil, int muhasebe, string birim, int durumAralik, ENUMBelgeTuru belgeTur, bool faaliyetli, string kur, string fon, string fin, string eko)
        {
            kur = kur.Replace(".", "");
            fon = fon.Replace(".", "");
            fin = fin.Replace(".", "");
            eko = eko.Replace(".", "");
            string alanlar = TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            alanlar += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];
            alanlar += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            alanlar += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD];
            alanlar += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD];
            alanlar += ", " + VeriErisim.IslecBul("SUBSTRING") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ",1,3)";
            alanlar += ", MUHMUHASEBE.MUHASEBEAD, MUHMUHASEBEBIRIM.BIRIMAD";

            string kur2 = "";
            if (kur.Length == 2)
                kur2 = kur + "70";

            string sql = "SELECT " + alanlar + ", SUM(" + ALANLAR[(int)ENUMAlanlar.BORC] + "-" + ALANLAR[(int)ENUMAlanlar.ALACAK] + ")";

            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN MUHMUHASEBE ON MUHMUHASEBE.MUHASEBEKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            sql += " INNER JOIN MUHMUHASEBEBIRIM ON MUHMUHASEBEBIRIM.MUHASEBEKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", "MUHMUHASEBEBIRIM.BIRIMKOD =" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD]);
            sql = VeriErisim.KosulEkle(sql, "AND", "MUHMUHASEBEBIRIM.KURKOD =" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD]);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (TABOdenekHarcamaSorgu.muhasebatTur == "")
            {
                int ay = 12;
                DateTime dt = DateTime.Now;
                if (dt.Year == yil)
                    ay = dt.Month;
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "AY", "=", ay));
            }
            sql = VeriErisim.KosulEkle(sql, "AND", "(HESAPKOD LIKE '830%')");

            if (muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe);

            if (birim != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birim);

            if (kur != "")
            {
                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], " >= ", kur + "");
                else
                    sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], " LIKE ", kur + "%");

                if (kur2 != "")
                    sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], " < ", kur2);
            }

            if (fon != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], " LIKE ", fon + "%");
            if (fin != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], " LIKE ", fin + "%");
            if (eko != "")
                sql += " AND " + VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], " LIKE ", eko + "%");


            sql += " GROUP BY " + alanlar + " ORDER BY " + alanlar;
            return sql;
        }
    }
}

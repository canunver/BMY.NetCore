using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHIS veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIs : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIs(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHIS";

        public enum ENUMAlanlar
        {
            KOD = 0,
            ORGKOD = 1,
            ADI = 2,
            TIP = 3,
            BASLANGICTARIH = 4,
            BITISTARIH = 5,
            AMAC = 6,
            HEDEF = 7,
            SORUMLUKOD = 8,
            GOREVTANIMI = 9,
            DURUM = 10,
            ISLEMYAPAN = 11,
            ISLEMTARIH = 12
        }

        static string[] ALANLAR = { "KOD", "ORGKOD", "ADI", "TIP", "BASLANGICTARIH", "BITISTARIH", "AMAC", "HEDEF", "SORUMLUKOD", "GOREVTANIMI", "DURUM", "ISLEMYAPAN", "ISLEMTARIH" };

        public Sonuc Kaydet(IsTanim form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(form.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.orgBirimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.adi) + ", ";
            sql = sql + VeriErisim.DegerYap(form.tip) + ", ";
            sql = sql + VeriErisim.DegerYap(form.baslangicTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(form.bitisTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(form.amac) + ", ";
            sql = sql + VeriErisim.DegerYap(form.hedef) + ", ";
            sql = sql + VeriErisim.DegerYap(form.sorumluKod) + ", ";
            sql = sql + VeriErisim.DegerYap(form.gorevTanimi) + ", ";
            sql = sql + VeriErisim.DegerYap(form.durum) + ", ";
            sql = sql + VeriErisim.DegerYap(form.islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(form.islemTarihi) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(IsTanim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], form.orgBirimKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], form.adi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], form.tip) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], form.baslangicTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], form.bitisTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMAC], form.amac) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HEDEF], form.hedef) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUKOD], form.sorumluKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GOREVTANIMI], form.gorevTanimi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.durum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], form.islemYapan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], form.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(IsTanim form)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",bir.ADI, bir.MUHASEBEKODU, bir.MUHASEBEBIRIM FROM " + TABLOAD;
            sql += " INNER JOIN Kullanici.dbo.ORGBIRIM bir on bir.KOD = MUHIS.ORGKOD";

            if (!string.IsNullOrWhiteSpace(form.sorguEko))
            {
                sql += " INNER JOIN MUHISODENEK ON MUHISODENEK.ISKOD = MUHIS.KOD ";
                kosul = VeriErisim.KosulOl("MUHISODENEK", "EKOKOD", " LIKE ", form.sorguEko.Replace(".", "") + "%");
            }

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", form.tip));
            if (form.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", form.durum));

            if (form.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("bir", "MUHASEBEKODU", "=", form.muhasebeKod));
            if (!string.IsNullOrWhiteSpace(form.muhasebeBirimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("bir", "MUHASEBEBIRIM", "=", form.muhasebeBirimKod));

            if (form.kod == 0 && (form.bitisTarihi == null || form.bitisTarihi.isNull))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], ">=", new TNSDateTime(System.DateTime.Now.Date)));

            if (form.sorguYil > 0)
            {
                string ekKosul1 = "";
                ekKosul1 = VeriErisim.KosulEkle(ekKosul1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], ">=", new TNSDateTime(new System.DateTime(form.sorguYil, 1, 1))));
                ekKosul1 = VeriErisim.KosulEkle(ekKosul1, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], "<", new TNSDateTime(new System.DateTime(form.sorguYil + 1, 1, 1))));

                string ekKosul2 = "";
                ekKosul2 = VeriErisim.KosulEkle(ekKosul2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], ">=", new TNSDateTime(new System.DateTime(form.sorguYil, 1, 1))));
                ekKosul2 = VeriErisim.KosulEkle(ekKosul2, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], "<", new TNSDateTime(new System.DateTime(form.sorguYil + 1, 1, 1))));

                string ekKosul = "(" + ekKosul1 + ") OR (" + ekKosul2 + ")";

                if (ekKosul != "")
                    sql = sql + " AND (" + ekKosul + ")";
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ADI];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BlokeOdenekListele(IsTanim form)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT MUHIS.ADI, bir.ADI, K.AD + ' ' + K.SOYAD";
            sql += " , (SELECT SUM(IC.BLOKETUTAR) FROM HRCBLOKEODENEK IC WHERE IC.ISKOD = MUHIS.KOD AND IC.MIFNO <> '' [KRITER_HARCAMALAR]) AS HARCAMALAR ";
            sql += " , (SELECT SUM(IC.BLOKETUTAR) FROM HRCBLOKEODENEK IC WHERE IC.ISKOD = MUHIS.KOD AND IC.MIFNO = '' [KRITER_BUTCEODENEKLER]) AS BUTCEODENEKLER";
            sql += " FROM MUHIS ";
            sql += " INNER JOIN Kullanici.dbo.ORGBIRIM bir on bir.KOD = MUHIS.ORGKOD ";
            sql += " LEFT OUTER JOIN Kullanici.dbo.KULLANICI K on K.MERNIS = MUHIS.SORUMLUKOD ";

            string icKriter = "";
            if (!string.IsNullOrWhiteSpace(form.sorguEko))
                icKriter = VeriErisim.KosulEkle(icKriter, "AND", VeriErisim.KosulOl("IC", "EKOKOD", " LIKE ", form.sorguEko.Replace(".", "") + "%"));
            if (form.sorguYil > 0)
                icKriter = VeriErisim.KosulEkle(icKriter, "AND", VeriErisim.KosulOl("IC", "YIL", "=", form.sorguYil));

            if (icKriter != "")
                icKriter = "AND " + icKriter;

            sql = sql.Replace("[KRITER_HARCAMALAR]", icKriter);
            sql = sql.Replace("[KRITER_BUTCEODENEKLER]", icKriter);

            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (form.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", form.tip));
            if (form.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", form.durum));

            if (form.sorguYil > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", "YEAR(MUHIS.BASLANGICTARIH)>=" + form.sorguYil);
                kosul = VeriErisim.KosulEkle(kosul, "AND", "YEAR(MUHIS.BITISTARIH)<=" + form.sorguYil);
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            //sql += " GROUP BY MUHIS.ADI, bir.ADI, K.AD + ' ' + K.SOYAD ";
            sql += " ORDER BY 1, 2, 3";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

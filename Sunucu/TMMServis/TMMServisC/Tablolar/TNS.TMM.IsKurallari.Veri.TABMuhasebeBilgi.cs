using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Muhasebe Bilgi kayıtlarını tutan TASMUHASEBEBILGI veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABMuhasebeBilgi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMuhasebeBilgi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASMUHASEBEBILGI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            ISLEMREFNO = 0,
            ISLEMCINSI = 1,
            SERVIS = 2,
            DURUM = 3,
            ISLEMYAPAN = 4,
            ISLEMTARIH = 5,
            JSON = 6
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "ISLEMREFNO", "ISLEMCINSI", "SERVIS", "DURUM", "ISLEMYAPAN", "ISLEMTARIH", "JSON" };


        static string TABLOADDETAY = "TASMUHASEBEBILGIDETAY";

        public enum ENUMAlanlarDetay
        {
            KOD = 0,
            PRSICILNO = 1,
            ISLEMREFNO = 2,
        }

        static string[] ALANLARDETAY = { "KOD", "PRSICILNO", "ISLEMREFNO" };


        public IDataReader Listele(MuhasebeIslemi form)
        {
            string sql = "";

            string secAlanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR);

            sql += "SELECT " + secAlanlar + " FROM TASSICILNOHAREKET AS TDV ";
            sql += "LEFT OUTER JOIN TASISLEMTIP ON TDV.ISLEMTIPIKOD = TASISLEMTIP.KOD ";
            sql += "LEFT OUTER JOIN TASSICILNODURUM ON TDV.PRSICILNO = TASSICILNODURUM.PRSICILNO ";
            sql += "INNER JOIN TASMUHASEBEBILGIDETAY ON TDV.PRSICILNO = TASMUHASEBEBILGIDETAY.PRSICILNO ";
            sql += "INNER JOIN TASMUHASEBEBILGI ON TASMUHASEBEBILGIDETAY.ISLEMREFNO = TASMUHASEBEBILGI.ISLEMREFNO ";

            sql += "WHERE TDV.SIRANO = (SELECT MAX(SIRANO) AS Expr1 FROM dbo.TASSICILNOHAREKET AS B WHERE(TDV.PRSICILNO = B.PRSICILNO)) ";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TDV", "MUHASEBEKOD", " = ", form.kriter.muhasebeKod));

            if (form.kriter.harcamaKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TDV", "HARCAMABIRIMKOD", " = ", form.kriter.harcamaKod.Replace(".", "")));
            if (form.kriter.fisNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TDV", "FISNO", " = ", form.kriter.fisNo));
            if (form.islemRefNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMREFNO], " = ", form.islemRefNo));
            if (form.islemCinsi != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMCINSI], " = ", form.islemCinsi));
            if (form.durum > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], " = ", form.durum));
            if (!form.sorguTarih1.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", form.sorguTarih1));
            if (!form.sorguTarih2.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<=", form.sorguTarih2));

            sql += " GROUP BY " + secAlanlar;
            sql += " ORDER BY ISLEMREFNO";

            return veriErisim.DataReaderDondur(sql);
        }


        public IDataReader ListeleGonderilecek(MuhasebeIslemiKriter form)
        {
            string sql = "";

            if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.TASINIRISLEMFISI)
            {
                if (form.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM || //ihraç
                    form.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS ||
                    form.islemTipTur == (int)ENUMIslemTipi.HURDACIKIS ||
                    form.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKIS || form.islemTipTur == (int)ENUMIslemTipi.DEVIRGIRIS)
                {
                    sql = "SELECT A.PRSICILNO, A.HESAPPLANKOD, TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD, A.MUHASEBEKOD,A.HARCAMABIRIMKOD, A.AMBARKOD, A.KDVORAN, A.KDVLIBIRIMFIYAT, H.KDVLIBIRIMFIYAT, TASMUHASEBE.AD AS MUHASEBEAD, TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, TASAMBAR.AD AS AMBARAD, TASISLEMTIP.TUR ";
                    sql += ", CASE WHEN TASSICILNOOZELLIK.BULUNDUGUYER != '' THEN TASSICILNOOZELLIK.BULUNDUGUYER ELSE A.NEREYEGITTI END AS NEREYEGITTI";
                    sql += ",TASMUHASEBEISLEMTIPI.TIP, (SELECT SATISBEDELI FROM TASISLEMDETAY IC WHERE IC.YIL = A.YIL AND IC.MUHASEBEKOD = A.MUHASEBEKOD AND IC.HARCAMABIRIMKOD = A.HARCAMABIRIMKOD AND IC.FISNO = A.FISNO  AND IC.SIRANO=A.FISSIRANO), TIF.ACIKLAMA ";
                    sql += ", TIF.MUTEMMIM AS MUTEMMIM ";
                    sql += ",(SELECT * FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE)  ";
                    sql += ",(SELECT * FROM TASAMORTISMAN IC WHERE IC.PRSICILNO = A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE)  ";
                    sql += ",(SELECT COUNT(*) FROM TASGECICIIHRAC IC WHERE IC.PRSICILNO = A.PRSICILNO) AS GECICIIHRAC ";
                    sql += ", 0 AS TASITAMORTISMANSINIR ";

                    sql += "FROM TASSICILNOOZELLIK ";
                    sql += "RIGHT OUTER JOIN TASAMBAR ";
                    sql += "INNER JOIN TASSICILNOHAREKET AS A ";
                    sql += "INNER JOIN TASHESAPPLAN ON A.HESAPPLANKOD = TASHESAPPLAN.KOD ";
                    sql += "INNER JOIN TASMUHASEBE ON A.MUHASEBEKOD = TASMUHASEBE.KOD ";
                    sql += "INNER JOIN TASHARCAMABIRIM ON A.HARCAMABIRIMKOD = dbo.TASHARCAMABIRIM.KOD AND A.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD ON ";
                    sql += "TASAMBAR.MUHASEBEKOD = A.MUHASEBEKOD AND dbo.TASAMBAR.HARCAMABIRIMKOD = A.HARCAMABIRIMKOD AND TASAMBAR.KOD = A.AMBARKOD ON ";
                    sql += "TASSICILNOOZELLIK.PRSICILNO = A.PRSICILNO ";
                    sql += "LEFT OUTER JOIN TASISLEMTIP ON A.ISLEMTIPIKOD = TASISLEMTIP.KOD ";
                    sql += "LEFT OUTER JOIN TASMUHASEBEISLEMTIPI ON TASMUHASEBEISLEMTIPI.KOD = (SELECT BLOKAJKODU FROM TASISLEMOZELLIK IC WHERE IC.YIL = A.YIL AND IC.MUHASEBEKOD = A.MUHASEBEKOD AND IC.HARCAMABIRIMKOD = A.HARCAMABIRIMKOD AND IC.FISNO = A.FISNO AND IC.SIRANO = 1) ";
                    sql += "INNER JOIN TASSICILNOHAREKET H ON H.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO and H.SIRANO = (SELECT MIN(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO AND IC.ISLEMTURUKOD < 50 AND DURUM = 1) ";

                    if (form.islemTipKod == (int)ENUMIslemTipi.GECICIIHRAC)
                    {
                        sql += "INNER JOIN TASISLEMDETAY D ON D.SICILNO = A.PRSICILNO ";
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "YIL", " = ", form.yil));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "MUHASEBEKOD", " = ", form.muhasebeKod));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "HARCAMABIRIMKOD", " = ", form.harcamaKod.Replace(".", "")));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", "FISNO", " = ", form.fisNo));

                        sql += "INNER JOIN TASISLEMFORM AS TIF ON TIF.YIL = D.YIL AND TIF.MUHASEBEKOD = D.MUHASEBEKOD AND TIF.HARCAMABIRIMKOD = D.HARCAMABIRIMKOD AND TIF.ISLEMTIPIKOD = " + (int)ENUMIslemTipi.GECICIIHRAC + " AND TIF.FISNO = D.FISNO ";

                        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "DURUM", " <> ", 2));
                        sql += " AND A.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO AND IC.ISLEMTURUKOD < 50 AND DURUM = 1)";
                    }
                    else
                    {
                        sql += "INNER JOIN TASISLEMFORM AS TIF ON TIF.YIL = A.YIL AND TIF.MUHASEBEKOD = A.MUHASEBEKOD AND TIF.HARCAMABIRIMKOD = A.HARCAMABIRIMKOD AND TIF.AMBARKOD = A.AMBARKOD AND TIF.FISNO = A.FISNO ";

                        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "YIL", " = ", form.yil));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "MUHASEBEKOD", " = ", form.muhasebeKod));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", " = ", form.harcamaKod.Replace(".", "")));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "FISNO", " = ", form.fisNo));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "DURUM", " <> ", 2));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "TUR", " = ", form.islemTipTur));
                    }


                    //Onaylanmamış İharçların muhasebe ön izlemesinde gösterilmesi için (TASSICILNOHAREKET tablosuna henüz kayıt eklenmediği için)
                    //Durum : 100 - Ön izlememde onaysız kayıtların listelenmesi için 
                    if (form.durum == 100 && (form.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM || form.islemTipTur == (int)ENUMIslemTipi.SATISCIKIS || form.islemTipTur == (int)ENUMIslemTipi.HURDACIKIS))
                    {
                        string sql2 = @"
                        UNION ALL
                        SELECT TIFDETAY.SICILNO PRSICILNO, TIFDETAY.HESAPPLANKOD, TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD, TIF.MUHASEBEKOD,TIF.HARCAMABIRIMKOD, TIF.AMBARKOD, TIFDETAY.KDVORAN
                        , TIFDETAY.BIRIMFIYATKDVLI KDVLIBIRIMFIYAT, H.KDVLIBIRIMFIYAT, TASMUHASEBE.AD AS MUHASEBEAD, TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, TASAMBAR.AD AS AMBARAD
                        , TASISLEMTIP.TUR 
                        , CASE WHEN TASSICILNOOZELLIK.BULUNDUGUYER != '' THEN TASSICILNOOZELLIK.BULUNDUGUYER ELSE TIF.NEREYEGITTI END AS NEREYEGITTI,TASMUHASEBEISLEMTIPI.TIP
                        , (SELECT SATISBEDELI FROM TASISLEMDETAY IC WHERE IC.YIL = TIF.YIL AND IC.MUHASEBEKOD = TIF.MUHASEBEKOD AND IC.HARCAMABIRIMKOD = TIF.HARCAMABIRIMKOD AND IC.FISNO = TIF.FISNO  AND IC.SIRANO=TIFDETAY.SIRANO)
                        , TIF.ACIKLAMA , TIF.MUTEMMIM AS MUTEMMIM ,(SELECT * FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = TIFDETAY.SICILNO FOR XML RAW, ELEMENTS xsinil, TYPE)  
                        ,(SELECT * FROM TASAMORTISMAN IC WHERE IC.PRSICILNO = TIFDETAY.SICILNO FOR XML RAW, ELEMENTS xsinil, TYPE)  
                        ,(SELECT COUNT(*) FROM TASGECICIIHRAC IC WHERE IC.PRSICILNO = TIFDETAY.SICILNO) AS GECICIIHRAC 
                        , 0 AS TASITAMORTISMANSINIR

                        FROM TASISLEMFORM TIF
                        INNER JOIN TASISLEMDETAY AS TIFDETAY ON TIFDETAY.YIL = TIF.YIL AND TIFDETAY.MUHASEBEKOD = TIF.MUHASEBEKOD AND TIFDETAY.HARCAMABIRIMKOD = TIF.HARCAMABIRIMKOD AND TIFDETAY.FISNO = TIF.FISNO
                        INNER JOIN TASISLEMTIP AS TIFTIP ON TIF.ISLEMTIPIKOD = TIFTIP.KOD 
                        INNER JOIN TASHESAPPLAN ON TIFDETAY.HESAPPLANKOD = TASHESAPPLAN.KOD 
                        INNER JOIN TASMUHASEBE ON TIF.MUHASEBEKOD = TASMUHASEBE.KOD 
                        INNER JOIN TASAMBAR ON TIF.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND TIF.HARCAMABIRIMKOD = TASAMBAR.HARCAMABIRIMKOD AND TIF.AMBARKOD = dbo.TASAMBAR.KOD 
                        INNER JOIN TASHARCAMABIRIM ON TIF.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD AND TIF.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD 
                        INNER JOIN TASISLEMTIP ON TIF.ISLEMTIPIKOD = TASISLEMTIP.KOD 
                        LEFT OUTER JOIN TASMUHASEBEISLEMTIPI ON TASMUHASEBEISLEMTIPI.KOD = (SELECT BLOKAJKODU FROM TASISLEMOZELLIK IC WHERE IC.YIL = TIF.YIL AND IC.MUHASEBEKOD =TIF.MUHASEBEKOD AND IC.HARCAMABIRIMKOD = TIF.HARCAMABIRIMKOD AND IC.FISNO = TIF.FISNO AND IC.SIRANO = 1) 
                        INNER JOIN TASSICILNOOZELLIK TASSICILNOOZELLIK ON TASSICILNOOZELLIK.PRSICILNO = TIFDETAY.SICILNO
                        INNER JOIN TASSICILNOHAREKET H ON H.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO AND H.SIRANO = (SELECT MIN(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO AND IC.ISLEMTURUKOD < 50 AND DURUM = 1) 
                        ";

                        if (form.islemTipTur == (int)ENUMIslemTipi.DEVIRCIKISKURUM) //İhraç
                            sql2 = VeriErisim.KosulEkle(sql2, "WHERE", VeriErisim.KosulOl("TIF", "ISLEMTIPIKOD", " = ", 23));
                        else
                            sql2 = VeriErisim.KosulEkle(sql2, "WHERE", VeriErisim.KosulOl("TIF", "ISLEMTIPIKOD", " = ", 10));

                        sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("TIF", "DURUM", " = ", 1));
                        sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("TIF", "YIL", " = ", form.yil));
                        sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("TIF", "MUHASEBEKOD", " = ", form.muhasebeKod));
                        sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("TIF", "HARCAMABIRIMKOD", " = ", form.harcamaKod.Replace(".", "")));
                        sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("TIF", "FISNO", " = ", form.fisNo));

                        sql += sql2;
                    }
                    //--------------------------------------

                    sql += " ORDER BY HESAPPLANKOD, PRSICILNO";
                }
                else if (form.islemTipTur == (int)ENUMIslemTipi.SATINALMAGIRIS || form.islemTipTur == (int)ENUMIslemTipi.BAGISGIRIS || form.islemTipTur == (int)ENUMIslemTipi.ACILIS)
                {
                    sql = "SELECT H.PRSICILNO, H.HESAPPLANKOD, TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD, H.MUHASEBEKOD, H.HARCAMABIRIMKOD, H.AMBARKOD, H.KDVORAN, H.KDVLIBIRIMFIYAT, H.FIYAT ";
                    sql += ", TASMUHASEBE.AD AS MUHASEBEAD, TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, TASAMBAR.AD AS AMBARAD, H.ISLEMTURUKOD, H2.NEREYEGITTI, TASMUHASEBEISLEMTIPI.TIP ";
                    sql += ", 0 AS SATISBEDELI, OZ.YAZARADI ";
                    sql += ", TIF.MUTEMMIM AS MUTEMMIM ";
                    sql += ", NULL AS DEGERARTIS ";
                    sql += ", NULL AS AMORTISMAN ";
                    sql += ", 0 AS GECICIIHRAC ";
                    sql += ", 0 AS TASITAMORTISMANSINIR ";

                    sql += "FROM TASSICILNOHAREKET H ";
                    sql += "INNER JOIN TASISLEMFORM AS TIF ON TIF.YIL = H.YIL AND TIF.MUHASEBEKOD = H.MUHASEBEKOD AND TIF.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD AND TIF.AMBARKOD = H.AMBARKOD AND TIF.FISNO = H.FISNO ";
                    sql += "INNER JOIN TASSICILNOOZELLIK OZ ON H.PRSICILNO = OZ.PRSICILNO ";
                    sql += "INNER JOIN TASHESAPPLAN ON H.HESAPPLANKOD = TASHESAPPLAN.KOD ";
                    sql += "INNER JOIN TASMUHASEBE ON H.MUHASEBEKOD = TASMUHASEBE.KOD ";
                    sql += "INNER JOIN TASHARCAMABIRIM ON H.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD AND H.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD ";
                    sql += "LEFT OUTER JOIN TASAMBAR ON TASAMBAR.MUHASEBEKOD = H.MUHASEBEKOD AND dbo.TASAMBAR.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD AND TASAMBAR.KOD = H.AMBARKOD ";
                    sql += "LEFT OUTER JOIN TASMUHASEBEISLEMTIPI ON TASMUHASEBEISLEMTIPI.KOD = OZ.BLOKAJKODU ";
                    sql += "LEFT OUTER JOIN TASSICILNOHAREKET H2 ON H2.PRSICILNO = H.PRSICILNO ";
                    sql += "AND H2.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = H2.PRSICILNO AND(IC.ISLEMTURUKOD < 50 OR IC.ISLEMTURUKOD = 90)) ";

                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "YIL", " = ", form.yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "MUHASEBEKOD", " = ", form.muhasebeKod));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", " = ", form.harcamaKod.Replace(".", "")));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISNO", " = ", form.fisNo));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "ISLEMTIPIKOD", " < ", 1000));

                    if (form.durum != 100)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "DURUM", " <> ", 2));

                    sql += " ORDER BY H.HESAPPLANKOD, H.PRSICILNO";
                }
                else
                {
                    sql = "SELECT TDV.PRSICILNO, HESAPPLANKOD, HESAPPLANAD, MUHASEBEKOD, HARCAMABIRIMKOD, AMBARKOD, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, TASISLEMTIP.TUR ";
                    sql += ",OZ.BULUNDUGUYER, TASMUHASEBEISLEMTIPI.TIP, 0 AS SATISBEDELI, OZ.YAZARADI ";
                    sql += ", 0 AS MUTEMMIM ";
                    sql += ",(SELECT * FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = TDV.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) ";
                    sql += ",(SELECT * FROM TASAMORTISMAN IC WHERE IC.PRSICILNO = TDV.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) ";
                    sql += ", 0 AS GECICIIHRAC ";
                    sql += ", 0 AS TASITAMORTISMANSINIR ";

                    sql += "FROM TASDEMIRBASVIEW AS TDV ";
                    sql += "LEFT OUTER JOIN TASISLEMTIP ON TDV.ISLEMTIPIKOD = TASISLEMTIP.KOD ";
                    sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = TDV.PRSICILNO ";
                    sql += "LEFT OUTER JOIN TASMUHASEBEISLEMTIPI ON TASMUHASEBEISLEMTIPI.KOD = OZ.BLOKAJKODU ";

                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TDV", "YIL", " = ", form.yil));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TDV", "MUHASEBEKOD", " = ", form.muhasebeKod));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TDV", "HARCAMABIRIMKOD", " = ", form.harcamaKod.Replace(".", "")));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TDV", "FISNO", " = ", form.fisNo));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TDV", "ISLEMTIPIKOD", " < ", 1000));

                    sql += " ORDER BY HESAPPLANKOD, TDV.PRSICILNO";
                }
            }
            else if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.DEGERARTIS)
            {
                sql = "SELECT TASSICILNODEGERARTIS.PRSICILNO,TASSICILNOHAREKET.HESAPPLANKOD,TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD,TASSICILNOHAREKET.MUHASEBEKOD,TASSICILNOHAREKET.HARCAMABIRIMKOD,TASSICILNOHAREKET.AMBARKOD,TASSICILNOHAREKET.KDVORAN,TASSICILNOHAREKET.KDVLIBIRIMFIYAT,TASSICILNOHAREKET.FIYAT,TASMUHASEBE.AD AS MUHASEBEAD, TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, '' AS AMBARAD, TASSICILNOHAREKET.ISLEMTURUKOD,'' AS NEREYEGITTI, TASMUHASEBEISLEMTIPI.TIP,0 AS SATISBEDELI, TASSICILNODEGERARTIS.GEREKCE";
                sql += ", 0 AS MUTEMMIM ";
                sql += ",(SELECT * FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = TASSICILNODEGERARTIS.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE)";
                sql += ",(SELECT * FROM TASAMORTISMAN IC WHERE IC.PRSICILNO = TASSICILNODEGERARTIS.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE)";
                sql += ", 0 AS GECICIIHRAC ";
                sql += ", 0 AS TASITAMORTISMANSINIR ";

                //,TASSICILNODEGERARTIS.KOD, TASSICILNODEGERARTIS.TUTAR, TASSICILNODEGERARTIS.TUR, TASAMORTISMAN.AMORTISMANMIKTAR

                sql += " FROM TASSICILNODEGERARTIS";
                sql += " INNER JOIN TASMUHASEBE ON TASSICILNODEGERARTIS.MUHASEBEKOD = TASMUHASEBE.KOD";
                sql += " INNER JOIN TASHARCAMABIRIM ON TASSICILNODEGERARTIS.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD";
                sql += " INNER JOIN TASSICILNOHAREKET ON TASSICILNODEGERARTIS.PRSICILNO = TASSICILNOHAREKET.PRSICILNO AND TASSICILNOHAREKET.SIRANO = 1 ";
                sql += " INNER JOIN TASHESAPPLAN ON TASSICILNOHAREKET.HESAPPLANKOD = TASHESAPPLAN.KOD";
                sql += " INNER JOIN TASSICILNOOZELLIK ON TASSICILNODEGERARTIS.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO";
                //sql += " LEFT OUTER JOIN TASAMORTISMAN ON TASSICILNODEGERARTIS.KOD = TASAMORTISMAN.DEGERARTISKODU";
                sql += " LEFT OUTER JOIN TASMUHASEBEISLEMTIPI ON TASMUHASEBEISLEMTIPI.KOD = TASSICILNOOZELLIK.BLOKAJKODU";

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "MUHASEBEKOD", " = ", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "HARCAMABIRIMKOD", " = ", form.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "BELGENO", " = ", form.donem.ToString("000000")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "DURUM", " = ", 5));
            }
            else if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.ENFLASYONDUZELTMESI)
            {
                sql = "SELECT TASSICILNODEGERARTIS.PRSICILNO,TASSICILNOHAREKET.HESAPPLANKOD,TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD,TASSICILNOHAREKET.MUHASEBEKOD,TASSICILNOHAREKET.HARCAMABIRIMKOD,TASSICILNOHAREKET.AMBARKOD,TASSICILNOHAREKET.KDVORAN,TASSICILNOHAREKET.KDVLIBIRIMFIYAT,TASSICILNOHAREKET.FIYAT,TASMUHASEBE.AD AS MUHASEBEAD, TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, '' AS AMBARAD, TASSICILNOHAREKET.ISLEMTURUKOD,'' AS NEREYEGITTI, TASMUHASEBEISLEMTIPI.TIP,0 AS SATISBEDELI, TASSICILNODEGERARTIS.GEREKCE";
                sql += ", 0 AS MUTEMMIM ";
                sql += ",(SELECT * FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = TASSICILNODEGERARTIS.PRSICILNO AND IC.KOD = TASSICILNODEGERARTIS.KOD FOR XML RAW, ELEMENTS xsinil, TYPE)";
                sql += ",(SELECT * FROM TASAMORTISMAN IC WHERE IC.PRSICILNO = TASSICILNODEGERARTIS.PRSICILNO AND IC.DEGERARTISKODU = TASSICILNODEGERARTIS.KOD FOR XML RAW, ELEMENTS xsinil, TYPE)";
                sql += ", 0 AS GECICIIHRAC ";
                sql += ", 0 AS TASITAMORTISMANSINIR ";

                //,TASSICILNODEGERARTIS.KOD, TASSICILNODEGERARTIS.TUTAR, TASSICILNODEGERARTIS.TUR, TASAMORTISMAN.AMORTISMANMIKTAR

                sql += " FROM TASSICILNODEGERARTIS";
                sql += " INNER JOIN TASMUHASEBE ON TASSICILNODEGERARTIS.MUHASEBEKOD = TASMUHASEBE.KOD";
                sql += " INNER JOIN TASHARCAMABIRIM ON TASSICILNODEGERARTIS.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD";
                sql += " INNER JOIN TASSICILNOHAREKET ON TASSICILNODEGERARTIS.PRSICILNO = TASSICILNOHAREKET.PRSICILNO AND TASSICILNOHAREKET.SIRANO = 1 ";
                sql += " INNER JOIN TASHESAPPLAN ON TASSICILNOHAREKET.HESAPPLANKOD = TASHESAPPLAN.KOD";
                sql += " INNER JOIN TASSICILNOOZELLIK ON TASSICILNODEGERARTIS.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO";
                //sql += " LEFT OUTER JOIN TASAMORTISMAN ON TASSICILNODEGERARTIS.KOD = TASAMORTISMAN.DEGERARTISKODU";
                sql += " LEFT OUTER JOIN TASMUHASEBEISLEMTIPI ON TASMUHASEBEISLEMTIPI.KOD = TASSICILNOOZELLIK.BLOKAJKODU";

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "MUHASEBEKOD", " = ", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "HARCAMABIRIMKOD", " = ", form.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "AMBARKOD", " = ", form.ambarKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "TUR", " = ", 1)); //Enflasyon Düzeltmesi
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "TIFNO", " = ", form.fisNo));

                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "PRSICILNO", "=", 116380));

                //Muhasebe Önizleme
                if (form.durum != 100)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNODEGERARTIS", "DURUM", " = ", 5));
            }
            else if (form.muhasebeIslemTur == ENUMMuhasebeIslemTur.AMORTISMAN)
            {
                sql = "SELECT DISTINCT H.PRSICILNO,H.HESAPPLANKOD, HP.ACIKLAMA, A.MUHASEBEKOD,  A.HARCAMABIRIMKOD, A.AMBARKOD, H.KDVORAN, H.KDVLIBIRIMFIYAT, H.FIYAT, M.AD, HB.AD, AMB.AD ";
                sql += ", (SELECT H2.ISLEMTURUKOD FROM TASSICILNOHAREKET H2 WHERE H2.PRSICILNO = A.PRSICILNO AND H2.SIRANO = 1) AS ILKISLEMTURKOD "; //H.ISLEMTURUKOD
                //sql += ",CASE WHEN TASSICILNOOZELLIK.BULUNDUGUYER != '' THEN TASSICILNOOZELLIK.BULUNDUGUYER ELSE ";
                //sql += "(SELECT TASODA.KOD FROM TASSICILNOHAREKET AS H2 INNER JOIN TASODA ON TASODA.KOD = H2.NEREYEGITTI  WHERE H2.PRSICILNO = H.PRSICILNO AND H2.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET AS H3 WHERE H3.PRSICILNO = H2.PRSICILNO AND H3.ISLEMTURUKOD = 90 AND H3.DURUM <> 2)) END AS NEREYEGITTI ";
                //sql += ", (SELECT HN.NEREYEGITTI FROM TASSICILNOHAREKET HN WHERE HN.PRSICILNO = A.PRSICILNO AND HN.SIRANO = (SELECT MAX(HN2.SIRANO) FROM TASSICILNOHAREKET HN2 WHERE HN2.PRSICILNO = HN.PRSICILNO AND HN2.ISLEMTURUKOD IN(90,46,91,47) AND HN2.DURUM <> 2)) AS NEREYEGITTI ";
                sql += ", TASSICILNOOZELLIK.BULUNDUGUYER AS NEREYEGITTI "; //Gayrimenkulde 
                sql += ", TASMUHASEBEISLEMTIPI.TIP, 0 AS SATISBEDELI, '' AS ACIKLAMA ";
                sql += ", 0 AS MUTEMMIM ";
                sql += ", CAST ((SELECT * FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) AS NVARCHAR(MAX)) ";
                sql += ", CAST ((SELECT * FROM TASAMORTISMAN IC WHERE IC.PRSICILNO = A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) AS NVARCHAR(MAX)) ";
                sql += ", (SELECT COUNT(*) FROM TASISLEMFORM INNER JOIN TASISLEMDETAY ON  TASISLEMDETAY.YIL = TASISLEMFORM.YIL  AND TASISLEMDETAY.MUHASEBEKOD = TASISLEMFORM.MUHASEBEKOD  AND TASISLEMDETAY.HARCAMABIRIMKOD = TASISLEMFORM.HARCAMABIRIMKOD AND TASISLEMDETAY.FISNO = TASISLEMFORM.FISNO WHERE TASISLEMDETAY.YIL = H.YIL  AND TASISLEMDETAY.MUHASEBEKOD = H.MUHASEBEKOD  AND TASISLEMDETAY.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD  AND TASISLEMDETAY.SICILNO = CAST(H.PRSICILNO AS nvarchar)  AND TASISLEMFORM.ISLEMTIPIKOD = 24 AND TASISLEMFORM.DURUM = 5 AND TASISLEMFORM.ONAYDURUM = 3) AS GECICIIHRAC  ";
                sql += ", (SELECT AMORTISMANSINIRTASIT FROM TASAMORTISMANSINIR WHERE TASAMORTISMANSINIR.YIL = (SELECT B.YIL FROM TASSICILNOHAREKET B WHERE B.PRSICILNO = H.PRSICILNO AND B.SIRANO = 1)) AS TASITAMORTISMANSINIR ";
                
                sql += "FROM TASAMORTISMANFORM AS AF ";
                sql += "INNER JOIN TASAMORTISMAN AS A ON A.YIL = AF.YIL AND A.DONEM = AF.DONEM AND A.MUHASEBEKOD = AF.MUHASEBEKOD AND A.HARCAMABIRIMKOD = AF.HARCAMABIRIMKOD AND A.AMBARKOD = AF.AMBARKOD "; //AND (A.AMORTISMANTUR=1 OR A.AMORTISMANTUR=3) 
                sql += "INNER JOIN TASSICILNOHAREKET AS H ON H.PRSICILNO = A.PRSICILNO ";
                sql += "INNER JOIN TASHARCAMABIRIM AS HB ON A.MUHASEBEKOD = HB.MUHASEBEKOD AND A.HARCAMABIRIMKOD = HB.KOD ";
                sql += "INNER JOIN TASMUHASEBE AS M ON A.MUHASEBEKOD = M.KOD ";
                sql += "INNER JOIN TASAMBAR AS AMB ON A.MUHASEBEKOD = AMB.MUHASEBEKOD AND A.HARCAMABIRIMKOD = AMB.HARCAMABIRIMKOD AND A.AMBARKOD = AMB.KOD ";
                sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD = HP.KOD ";
                sql += "INNER JOIN TASSICILNOOZELLIK ON TASSICILNOOZELLIK.PRSICILNO = A.PRSICILNO ";
                sql += "LEFT OUTER JOIN TASMUHASEBEISLEMTIPI ON TASMUHASEBEISLEMTIPI.KOD = TASSICILNOOZELLIK.BLOKAJKODU ";

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "DURUM", " <> ", 2));

                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "ISLEMTURUKOD", " < ", 50));
                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "ISLEMTIPIKOD", " < ", 1000));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "YIL", " <= ", form.yil));
                sql += " AND 1 IN(SELECT 1 FROM TASSICILNOHAREKET B WHERE B.PRSICILNO = H.PRSICILNO AND B.MUHASEBEKOD = AF.MUHASEBEKOD AND B.HARCAMABIRIMKOD = AF.HARCAMABIRIMKOD AND B.AMBARKOD = AF.AMBARKOD AND B.DURUM <> 2 AND B.YIL <= AF.YIL) ";
                sql += " AND H.SIRANO = (SELECT MAX(B.SIRANO) FROM TASSICILNOHAREKET B WHERE B.PRSICILNO = H.PRSICILNO AND B.YIL <= AF.YIL AND B.DURUM <> 2 AND B.ISLEMTURUKOD>0) ";

                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AF", "YIL", " = ", form.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AF", "DONEM", " = ", form.donem));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AF", "MUHASEBEKOD", " = ", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AF", "HARCAMABIRIMKOD", " = ", form.harcamaKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AF", "AMBARKOD", " = ", form.ambarKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AF", "FISNO", " = ", form.fisNo));

                if (form.durum != 100)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("AF", "DURUM", " = ", 5));

                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "PRSICILNO", " = ", 203582));

                sql += " ORDER BY H.HESAPPLANKOD, H.PRSICILNO";
            }
            else
            {
                veriErisim.hata = "muhasebeIslemTur bulunamadı.";
                return null;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleTarihce(int yil, string muhasebe, string harcamaBirimi, string fisNo)
        {
            string sql = "";

            sql = "SELECT TARIHCE.YIL, TARIHCE.MUHASEBEKOD, TARIHCE.HARCAMABIRIMKOD, TARIHCE.FISNO, TARIHCE.ISLEMYAPAN, TARIHCE.ISLEMTARIH, TARIHCE.ISLEM, TARIHCE.IPADRESI, TARIHCE.TASISLEMFORMKOD, KULLANICI.SICILNO, KULLANICI.AD + ' ' + KULLANICI.SOYAD ";
            sql += "FROM TASISLEMFORMTARIHCE AS TARIHCE INNER JOIN KULLANICI ON KULLANICI.KULLANICIKODU = TARIHCE.ISLEMYAPAN ";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TARIHCE", "YIL", " = ", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TARIHCE", "MUHASEBEKOD", " = ", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TARIHCE", "HARCAMABIRIMKOD", " = ", harcamaBirimi.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TARIHCE", "FISNO", " = ", fisNo));

            sql += " ORDER BY 1,2,3,4,6";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(MuhasebeIslemi form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.islemRefNo) + ",";
            sql += VeriErisim.DegerYap(form.islemCinsi) + ",";
            sql += VeriErisim.DegerYap(form.servis) + ",";
            sql += VeriErisim.DegerYap(form.durum) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.json) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Kaydet(MuhasebeIslemiDetay form)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.refKod) + ",";
            sql += VeriErisim.DegerYap(form.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(form.islemRefNo) + ")";

            return KomutCalistir(sql, 1);
        }

        public int DurumGetirTekKolon(MuhasebeIslemi form)
        {
            string sql = "SELECT DURUM FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMREFNO], "=", form.islemRefNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc DurumDegistir(MuhasebeIslemi form, ENUMMuhasebeIslemiOnayDurumu yeniDurum)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)yeniDurum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMREFNO], "=", form.islemRefNo));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleDurumuIslenmeyenReferanslar()
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.ISLEMREFNO] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], " <> ", 1));

            return veriErisim.DataReaderDondur(sql);
        }

    }
}

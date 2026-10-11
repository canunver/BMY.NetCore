using System.Data;
using TNS.DEG;
using TNS.EVR.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using EVRServisI;
using System.Collections;
using System;

namespace TNS.EVR.IsKurallari
{
    public class EVRVeritabani
    {
        public static Sonuc Kaydet(VeriErisim veriErisim, Kullanici kullanan, string kod, string sql)
        {
            Sonuc sonuc = new Sonuc(false, EvrakI.EVRI110, "");

            string hata = string.Empty;
            if (string.IsNullOrEmpty(kod))
                hata += "kod boş bırakılamaz." + "<br>";
            if (string.IsNullOrEmpty(kod))
                hata += "sql boş bırakılamaz." + "<br>";


            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Kaydet(kod, new TNSDateTime(DateTime.Now));

                if (sonuc.islemSonuc)
                {
                    //sql query execute
                    try
                    {
                        sql += "\r\n";
                        int i = ve.ExecuteNonquery(sql.Replace("GO\r\n", ""));

                        if (ve.hata != "")
                        {
                            sonuc.islemSonuc = false;
                            sonuc.hataStr = ve.hata;
                        }
                    }
                    catch (Exception ex)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ex.Message;
                    }
                }

                if (sonuc.islemSonuc)
                {
                    sonuc.bilgiStr = EvrakI.EVRI094;
                    ve.Commit();
                }
                else
                {
                    sonuc.hataStr = EvrakI.EVRI095 + "<br>" + sonuc.hataStr;
                    ve.Rollback();
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{65577E1A-4201-43D0-BD2F-64817F38062F} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            ve.VeriErisimKapat();
            return sonuc;
        }
        public static Sonuc Sil(VeriErisim veriErisim, Kullanici kullanan, string kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string hata = string.Empty;
            if (kod != "")
                hata = "Silinecek veri bulunamadı." + "<br>";

            if (!string.IsNullOrEmpty(hata))
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = hata;
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);

            ve.BeginTransaction();
            try
            {
                sonuc = tab.Sil(kod);
                if (sonuc.islemSonuc)
                {
                    ve.Commit();
                    sonuc.bilgiStr = EvrakI.EVRI097;
                }
                else
                {
                    ve.Rollback();
                    sonuc.hataStr = EvrakI.EVRI098 + "<br>" + sonuc.hataStr;
                }
            }
            catch (Exception ex)
            {
                ve.Rollback();
                OrtakFonksiyonlar.HataStrYaz("{E6A65AA3-9697-4074-B016-2EEFB08DC264} BeginTransaction - Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }

            ve.VeriErisimKapat();
            return sonuc;
        }
        public static GuncellemeBilgi[] Listele(VeriErisim veriErisim, Kullanici kullanan, string kod)
        {
            ObjectArray liste = new ObjectArray();
            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);

            IDataReader dr = tab.Listele(kod);
            if (dr != null)
            {
                while (dr.Read())
                {
                    GuncellemeBilgi form = new GuncellemeBilgi();
                    form.kod = VeriErisim.DegerAl(dr, (int)TABVeritabani.ENUMAlanlar.KOD, "");
                    form.islemTarihi = VeriErisim.DegerAl(dr, (int)TABVeritabani.ENUMAlanlar.ISLEMTARIH);

                    liste.objeler.Add(form);
                }
                dr.Close();
            }
            else if (ve.hata != "")
            {
                //tablo oluştur
                try
                {
                    ve.hata = "";
                    int i = ve.ExecuteNonquery(getCreateTableSql());
                }
                catch (Exception ex)
                {
                    ve.hata = ex.Message;
                }
            }

            ve.VeriErisimKapat();

            GuncellemeBilgi[] arr = (GuncellemeBilgi[])liste.objeler.ToArray(typeof(GuncellemeBilgi));

            return arr;
        }
        private static string getCreateTableSql()
        {
            return @"
                    SET ANSI_NULLS ON

                    SET QUOTED_IDENTIFIER ON

                    SET ANSI_PADDING ON

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GUNCELLEMEBILGI]') AND type in (N'U'))
                    BEGIN
                    CREATE TABLE [dbo].[GUNCELLEMEBILGI](
	                    [KOD] [varchar](50) NOT NULL,
	                    [ISLEMTARIH] [datetime] NULL,
                     CONSTRAINT [PK_GUNCELLEMEBILGI] PRIMARY KEY CLUSTERED 
                    (
	                    [KOD] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                    ) ON [PRIMARY]
                    END

                    SET ANSI_PADDING OFF
                    ";
        }

        public static Sonuc EvrakHareketSonDurumTabloMu(VeriErisim veriErisim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);

            try
            {
                string sql = "IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EVREVRAKHAREKETSONDURUM]') AND type in (N'U')) SELECT '1'";
                string deger = ve.TekKolonSQLStr(sql);

                if (ve.hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                else if (deger == "1")
                    sonuc.islemSonuc = true;
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        public static Sonuc EvrakHareketSonDurumTabloYap(VeriErisim veriErisim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);

            try
            {
                ve.sureAsimi = 180;
                int deger = ve.TekKolonSQLInt(getEvrakHareketSonDurumSql());

                if (ve.hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                else if (deger == 2)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "EVREVRAKHAREKETSONDURUM isimli View bulunamıyor. Lütfen veritabanı üzerinden kontrol ediniz.";
                }
                else if (deger > -1)
                {
                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr = "EVREVRAKHAREKETSONDURUM view <font style='color:#D50200;'><b>" + deger + "</b></font> satır bilgi ile tabloya dönüştürüldü.";
                }

            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }
        private static string getEvrakHareketSonDurumSql()
        {
            return @"
                    BEGIN TRANSACTION
                    SET QUOTED_IDENTIFIER ON
                    SET ARITHABORT ON
                    SET NUMERIC_ROUNDABORT OFF
                    SET CONCAT_NULL_YIELDS_NULL ON
                    SET ANSI_NULLS ON
                    SET ANSI_PADDING ON
                    SET ANSI_WARNINGS ON
                    COMMIT
                    BEGIN TRANSACTION

                    IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[EVREVRAKHAREKETSONDURUM]') AND type in (N'V'))
                    BEGIN

	                    EXEC dbo.sp_rename N'[dbo].[EVREVRAKHAREKETSONDURUM]', N'EVREVRAKHAREKETSONDURUMV', 'OBJECT'

	                    SELECT * INTO dbo.EVREVRAKHAREKETSONDURUM FROM dbo.EVREVRAKHAREKETSONDURUMV

	                    ALTER TABLE dbo.EVREVRAKHAREKETSONDURUM ADD CONSTRAINT
	                    PK_EVREVRAKHAREKETSONDURUM PRIMARY KEY CLUSTERED 
	                    (
	                    PREVRAKNO
	                    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	
	                    ALTER TABLE dbo.EVREVRAKHAREKETSONDURUM SET (LOCK_ESCALATION = TABLE)
                        
                        SELECT COUNT(*) FROM dbo.EVREVRAKHAREKETSONDURUM
                    END
                    ELSE
	                    SELECT 2

                    COMMIT
            ";
        }

        /// <summary>
        /// Dağıtım ile kaydedilen evrakların evrak hareketleri Havale edildi olarak mı görünüyor.
        /// </summary>
        public static Sonuc DagitimHareketleriHavaleMi(VeriErisim veriErisim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);

            try
            {
                string sql = @"SELECT COUNT(*)
                                FROM EVREVRAKHAREKET INNER JOIN
                                EVREVRAKSICIL ON EVREVRAKHAREKET.PREVRAKNO = EVREVRAKSICIL.PREVRAKNO INNER JOIN
                                EVREVRAK ON EVREVRAKSICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO
                                WHERE (EVREVRAKHAREKET.SIRANO = 1) AND (EVREVRAKHAREKET.ISLEM = 51) AND (EVREVRAK.KAYITISLEM = 1)";
                int deger = ve.TekKolonSQLInt(sql);

                if (ve.hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                else if (deger > 0)
                {
                    sonuc.islemSonuc = true;
                    sonuc.anahtar = deger.ToString();
                }
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        /// <summary>
        /// Dağıtım ile kaydedilen evrakların evrak hareketleri Havale edildi ise, Dağıtım Yapıldı olarak değiştir.
        /// </summary>
        public static Sonuc DagitimHavaleHarekeleriniDegistir(VeriErisim veriErisim)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, TABGenel.BaglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);
            ve.sureAsimi = 360;

            try
            {
                string sql = @"UPDATE EVREVRAKHAREKET
                                SET ISLEM = 52
                                FROM EVREVRAKHAREKET INNER JOIN
                                EVREVRAKSICIL ON EVREVRAKHAREKET.PREVRAKNO = EVREVRAKSICIL.PREVRAKNO INNER JOIN
                                EVREVRAK ON EVREVRAKSICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO
                                WHERE (EVREVRAKHAREKET.SIRANO = 1) AND (EVREVRAKHAREKET.ISLEM = 51) AND (EVREVRAK.KAYITISLEM = 1)";
                int deger = ve.ExecuteNonquery(sql);


                if (ve.hata == "" && deger > 0)
                {
                    Sonuc sonucSondurum = EvrakHareketSonDurumTabloMu(ve);
                    if (sonucSondurum.islemSonuc)
                    {
                        sql = @"UPDATE EVREVRAKHAREKETSONDURUM
                                SET ISLEM = 52
                                FROM EVREVRAKHAREKETSONDURUM INNER JOIN
                                EVREVRAKSICIL ON EVREVRAKHAREKETSONDURUM.PREVRAKNO = EVREVRAKSICIL.PREVRAKNO INNER JOIN
                                EVREVRAK ON EVREVRAKSICIL.EVREVRAKNO = EVREVRAK.EVREVRAKNO
                                WHERE (EVREVRAKHAREKETSONDURUM.SIRANO = 1) AND (EVREVRAKHAREKETSONDURUM.ISLEM = 51) AND (EVREVRAK.KAYITISLEM = 1)";
                        int deger2 = ve.ExecuteNonquery(sql);
                    }
                }


                if (ve.hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                else if (deger > 0)
                {
                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr = "Havale edildi olarak görünen <font style='color:#D50200;'><b>" + deger + "</b></font> kayıt dağıtım yapıldı olarak değiştirildi.";
                }

            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }


        #region Index İşlemleri


        public static VeritabaniIndex[] IndexListele(VeriErisim veriErisim, VeritabaniIndex form, string baglantiSatiri)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            ObjectArray liste = new ObjectArray();

            if (string.IsNullOrEmpty(baglantiSatiri))
                baglantiSatiri = TABGenel.BaglantiSatiri;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, baglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);

            try
            {
                string sql = @"SELECT
                              [sObj].[object_id] AS [ObjectID],
                              [sObj].[name] AS [ObjectName]
                              , ISNULL([sIdx].[name], '-') AS [IndexName]
                              , CASE
                                     WHEN [sIdx].[type] = 0 THEN 'Heap'
                                     WHEN [sIdx].[type] = 1 THEN 'Clustered'
                                     WHEN [sIdx].[type] = 2 THEN 'Nonclustered'
                                     WHEN [sIdx].[type] = 3 THEN 'XML'
                                     WHEN [sIdx].[type] = 4 THEN 'Spatial'
                                     WHEN [sIdx].[type] = 5 THEN 'Reserved for future use'
                                     WHEN [sIdx].[type] = 6 THEN 'Nonclustered columnstore index'
                                 END AS [IndexType]
                               , [IdxSizeDetails].[IndexSizeMB]
                               , CAST([sdmfIPS].[avg_fragmentation_in_percent] AS NUMERIC(5,2)) AS [AvgPctFrag]
                               , [sIdx].[fill_factor] AS [FillFactor]
                            FROM 
                               [sys].[indexes] AS [sIdx]
                               INNER JOIN [sys].[objects] AS [sObj]
                                  ON [sIdx].[object_id] = [sObj].[object_id]
                               LEFT JOIN  [sys].[partitions] AS [sPtn]
                                  ON [sIdx].[object_id] = [sPtn].[object_id]
                                  AND [sIdx].[index_id] = [sPtn].[index_id]
                               LEFT JOIN (
                                        SELECT
                                           [sIdx].[object_id]
                                           , [sIdx].[index_id]
                                           --, SUM([sAU].[used_pages]) * 8 / 1024.0 AS [IndexSizeMB]
			                               ,Convert(numeric(18,3),Convert(numeric,8 * SUM([sAU].[used_pages] - CASE WHEN [sAU].[type] <> 1 THEN [sAU].[used_pages] WHEN [sIdx].[index_id] < 2 THEN [sAU].data_pages ELSE 0 END)) / 1024) As [IndexSizeMB]
                                        FROM 
                                           [sys].[indexes] AS [sIdx]
                                           INNER JOIN [sys].[partitions] AS [sPtn]
                                              ON [sIdx].[object_id] = [sPtn].[object_id]
                                              AND [sIdx].[index_id] = [sPtn].[index_id]
                                           INNER JOIN [sys].[allocation_units] AS [sAU]
                                              ON [sPtn].[partition_id] = [sAU].[container_id]
                                        GROUP BY [sIdx].[object_id], [sIdx].[index_id]
                               ) [IdxSizeDetails]
                                  ON [sIdx].[object_id] = [IdxSizeDetails].[object_id]
                                  AND [sIdx].[index_id] = [IdxSizeDetails].[index_id]
                               LEFT JOIN [sys].[dm_db_index_physical_stats] (DB_ID(),NULL,NULL,NULL,'LIMITED') [sdmfIPS]
                                  ON [sIdx].[object_id] = [sdmfIPS].[object_id]
                                  AND [sIdx].[index_id] = [sdmfIPS].[index_id]
                                  AND [sdmfIPS].[database_id] = DB_ID()
                            WHERE
                               [sObj].[type] IN ('U','V')         -- Look in Tables & Views
                               AND [sObj].[is_ms_shipped] = 0x0   -- Exclude System Generated Objects
                               AND [sIdx].[is_disabled] = 0x0     -- Exclude Disabled Indexes
                               AND [sdmfIPS].[alloc_unit_type_desc] = 'IN_ROW_DATA' AND ";
                if (form.indexTip == "Nonclustered")
                    sql += "[sIdx].[type] = 2 ";
                else if (form.indexTip == "Clustered")
                    sql += "[sIdx].[type] = 1";
                else
                    sql += "([sIdx].[type] = 2 OR [sIdx].[type] = 1)";
                sql += " ORDER BY [sIdx].[type], [IdxSizeDetails].[IndexSizeMB] DESC";

                IDataReader dr = ve.DataReaderDondur(sql);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        form = new VeritabaniIndex();
                        form.objectId = VeriErisim.DegerAl(dr, 0, 0);
                        form.tabloAd = VeriErisim.DegerAl(dr, 1, "");
                        form.indexAd = VeriErisim.DegerAl(dr, 2, "");
                        form.indexTip = VeriErisim.DegerAl(dr, 3, "");
                        form.indexSize = VeriErisim.DegerAl(dr, 4, 0.0);
                        form.fragmentation = VeriErisim.DegerAl(dr, 5, 0.0);
                        form.fillFactor = VeriErisim.DegerAl(dr, 6, 0);

                        liste.objeler.Add(form);
                    }
                    dr.Close();
                }

                if (ve.hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            ve.VeriErisimKapat();

            VeritabaniIndex[] arr = (VeritabaniIndex[])liste.objeler.ToArray(typeof(VeritabaniIndex));

            return arr;
        }

        public static Sonuc IndexIslem(VeriErisim veriErisim, VeritabaniIndex[] liste, ENUMVeritabaniIndexIslem islem, string baglantiSatiri)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (liste == null || liste.Length == 0)
            {
                sonuc.DegerAta("Liste boş olamaz.");
                return sonuc;
            }

            if (string.IsNullOrEmpty(baglantiSatiri))
                baglantiSatiri = TABGenel.BaglantiSatiri;

            VeriErisim ve = VeriErisim.VeriErisimBul(veriErisim, baglantiSatiri);
            TABVeritabani tab = new TABVeritabani(ve);

            try
            {
                ve.sureAsimi = 360;

                string sql = "";
                int deger = 0;
                string bilgi = "";
                if (islem == ENUMVeritabaniIndexIslem.REBUILD)
                {
                    bilgi = "Rebuild Index";
                    sql = "";
                    foreach (var form in liste)
                        sql += string.Format("ALTER INDEX [{0}] ON [dbo].[{1}] REBUILD PARTITION = ALL WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON{2});\r\n", form.indexAd, form.tabloAd, ((form.fillFactor > 0 && form.fillFactor < 100) ? string.Format(", FILLFACTOR = {0}", form.fillFactor) : ""));

                    deger = ve.ExecuteNonquery(sql);

                }
                else if (islem == ENUMVeritabaniIndexIslem.REORGANIZE)
                {
                    bilgi = "Reorganize Index";
                    foreach (var form in liste)
                        sql += string.Format("ALTER INDEX [{0}] ON [dbo].[{1}] REORGANIZE WITH ( LOB_COMPACTION = ON );\r\n", form.indexAd, form.tabloAd);

                    deger = ve.ExecuteNonquery(sql);

                }
                else if (islem == ENUMVeritabaniIndexIslem.DROP)
                {
                    bilgi = "Drop Index";
                    foreach (var form in liste)
                        if (form.indexTip == "Nonclustered")
                            sql += string.Format("DROP INDEX [{0}] ON [dbo].[{1}];\r\n", form.indexAd, form.tabloAd);

                    deger = ve.ExecuteNonquery(sql);
                }
                else if (islem == ENUMVeritabaniIndexIslem.CREATE)
                {
                    bilgi = "Create Index";
                    foreach (var form in liste)
                        if (form.indexTip == "Nonclustered")
                            sql += string.Format("IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[{1}]') AND name = N'{0}')\r\nCREATE NONCLUSTERED INDEX [{0}] ON [dbo].[{1}] ({2}){3}\r\n", form.indexAd, form.tabloAd, form.kolon, (!string.IsNullOrEmpty(form.include) ? string.Format(" INCLUDE ({0})", form.include) : ""));

                    deger = ve.ExecuteNonquery(sql);
                }


                if (ve.hata != "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                else
                {
                    sonuc.islemSonuc = true;
                    sonuc.bilgiStr = bilgi + " işlemi başarıyla gerçekleştirildi.";
                }
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }

            ve.VeriErisimKapat();

            return sonuc;
        }

        #endregion

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TNS.KYM;
using TNS.Veri;

namespace TNS.UZY
{
    /// <summary>
    /// Veri tabanı index işlemleri (listele, rebuild, reorganize, drop, create) ve uygulamanın bağlantı satırları.
    /// Eski EVRServis'teki EVRVeritabani.IndexListele / IndexIslem ve EVRServis.VeritabaniBaglantiSatiriListele'nin aynısıdır;
    /// Evrak servisi henüz taşınmadığından ve işlemler evrağa özgü olmadığından UZY servisine alındı.
    /// Değişenler: yalnızca sistem yöneticisi çağırabilir; bağlantı satırının kendisi değil appSettings'teki adı kullanılır
    /// (eski sayfa adı gönderiyordu, VeriErisimBul da adla çalışır).
    /// </summary>
    public static class UZYVeritabani
    {
        private static bool Yonetici(Kullanici kullanan)
        {
            return kullanan != null && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        /// <summary>
        /// appSettings'teki BaglantiSatiri... anahtarları (adında x ve . geçmeyenler). Anahtar: veri tabanı adı (Catalog), değer: appSettings adı.
        /// </summary>
        public static Hashtable BaglantiSatiriListele(Kullanici kullanan)
        {
            Hashtable list = new Hashtable();
            if (!Yonetici(kullanan)) return list;
            try
            {
                string p_sifrele = System.Configuration.ConfigurationManager.AppSettings.Get("BaglantiSatiri.Sifrele") + "";

                foreach (string item in System.Configuration.ConfigurationManager.AppSettings.AllKeys)
                {
                    if (item.ToLower().IndexOf("x") == -1 && item.IndexOf(".") == -1 && item.StartsWith("BaglantiSatiri"))
                    {
                        string value = System.Configuration.ConfigurationManager.AppSettings.Get(item) + "";

                        if (OrtakFonksiyonlar.ConvertToInt(p_sifrele, 0) > 0 && !string.IsNullOrEmpty(value))
                        {
                            if (p_sifrele == "2")
                                value = OrtakFonksiyonlar.SifreliCozOrtakAnahtar(value);
                            else
                                value = OrtakFonksiyonlar.SifreCoz(value);
                        }

                        int yer = value.IndexOf("Catalog=");
                        if (yer < 0) continue;
                        string key = value.Substring(yer + 8).Split(';')[0];

                        if (!list.ContainsKey(key))
                            list.Add(key, item);
                    }
                }
            }
            catch (Exception ex)
            {
                list.Add("*", ex.Message);
            }
            return list;
        }

        /// <summary>Verilen tipteki (Nonclustered, Clustered, Hepsi) indexler. Hata olursa tek elemanlı dizi: objectId = -2, tabloAd = hata.</summary>
        public static VeritabaniIndex[] IndexListele(Kullanici kullanan, VeritabaniIndex form, string baglantiAd)
        {
            if (!Yonetici(kullanan))
                return new VeritabaniIndex[1] { new VeritabaniIndex() { objectId = -2, tabloAd = "Bu işlem için yetkiniz yok." } };

            List<VeritabaniIndex> liste = new List<VeritabaniIndex>();
            string hata = "";

            VeriErisim ve = VeriErisim.VeriErisimBul(null, baglantiAd);
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
                if (form != null && form.indexTip == "Nonclustered")
                    sql += "[sIdx].[type] = 2 ";
                else if (form != null && form.indexTip == "Clustered")
                    sql += "[sIdx].[type] = 1";
                else
                    sql += "([sIdx].[type] = 2 OR [sIdx].[type] = 1)";
                sql += " ORDER BY [sIdx].[type], [IdxSizeDetails].[IndexSizeMB] DESC";

                IDataReader dr = ve.DataReaderDondur(sql);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        VeritabaniIndex satir = new VeritabaniIndex();
                        satir.objectId = VeriErisim.DegerAl(dr, 0, 0);
                        satir.tabloAd = VeriErisim.DegerAl(dr, 1, "");
                        satir.indexAd = VeriErisim.DegerAl(dr, 2, "");
                        satir.indexTip = VeriErisim.DegerAl(dr, 3, "");
                        satir.indexSize = VeriErisim.DegerAl(dr, 4, 0.0);
                        satir.fragmentation = VeriErisim.DegerAl(dr, 5, 0.0);
                        satir.fillFactor = VeriErisim.DegerAl(dr, 6, 0);
                        liste.Add(satir);
                    }
                    dr.Close();
                }
                hata = ve.hata;
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            ve.VeriErisimKapat();

            // Eski kod hatayı yutuyordu; sayfa hatayı objectId = -2 satırıyla gösteriyor.
            if (!string.IsNullOrEmpty(hata) && liste.Count == 0)
                return new VeritabaniIndex[1] { new VeritabaniIndex() { objectId = -2, tabloAd = hata } };

            return liste.ToArray();
        }

        public static Sonuc IndexIslem(Kullanici kullanan, VeritabaniIndex[] liste, ENUMVeritabaniIndexIslem islem, string baglantiAd)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            if (!Yonetici(kullanan))
            {
                sonuc.DegerAta("Bu işlem için yetkiniz yok.");
                return sonuc;
            }

            if (liste == null || liste.Length == 0)
            {
                sonuc.DegerAta("Liste boş olamaz.");
                return sonuc;
            }

            VeriErisim ve = VeriErisim.VeriErisimBul(null, baglantiAd);

            try
            {
                ve.sureAsimi = 360;

                string sql = "";
                string bilgi = "";
                if (islem == ENUMVeritabaniIndexIslem.REBUILD)
                {
                    bilgi = "Rebuild Index";
                    foreach (var form in liste)
                        sql += string.Format("ALTER INDEX [{0}] ON [dbo].[{1}] REBUILD PARTITION = ALL WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON{2});\r\n", Ad(form.indexAd), Ad(form.tabloAd), ((form.fillFactor > 0 && form.fillFactor < 100) ? string.Format(", FILLFACTOR = {0}", form.fillFactor) : ""));
                }
                else if (islem == ENUMVeritabaniIndexIslem.REORGANIZE)
                {
                    bilgi = "Reorganize Index";
                    foreach (var form in liste)
                        sql += string.Format("ALTER INDEX [{0}] ON [dbo].[{1}] REORGANIZE WITH ( LOB_COMPACTION = ON );\r\n", Ad(form.indexAd), Ad(form.tabloAd));
                }
                else if (islem == ENUMVeritabaniIndexIslem.DROP)
                {
                    bilgi = "Drop Index";
                    foreach (var form in liste)
                        if (form.indexTip == "Nonclustered")
                            sql += string.Format("DROP INDEX [{0}] ON [dbo].[{1}];\r\n", Ad(form.indexAd), Ad(form.tabloAd));
                }
                else if (islem == ENUMVeritabaniIndexIslem.CREATE)
                {
                    bilgi = "Create Index";
                    foreach (var form in liste)
                        if (form.indexTip == "Nonclustered")
                            sql += string.Format("IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[{1}]') AND name = N'{4}')\r\nCREATE NONCLUSTERED INDEX [{0}] ON [dbo].[{1}] ({2}){3}\r\n", Ad(form.indexAd), Ad(form.tabloAd).Replace("'", "''"), form.kolon, (!string.IsNullOrEmpty(form.include) ? string.Format(" INCLUDE ({0})", form.include) : ""), (form.indexAd ?? "").Replace("'", "''"));
                }

                if (sql != "")
                    ve.ExecuteNonquery(sql);

                if (!string.IsNullOrEmpty(ve.hata))
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ve.hata;
                }
                else if (sql == "")
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = "Seçilenler arasında bu işlemin yapılabileceği index yok.";
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

        /// <summary>Köşeli parantez içine yazılan tablo / index adı: ] karakteri SQL kuralına göre ikilenir.</summary>
        private static string Ad(string ad)
        {
            return (ad ?? "").Replace("]", "]]");
        }
    }
}

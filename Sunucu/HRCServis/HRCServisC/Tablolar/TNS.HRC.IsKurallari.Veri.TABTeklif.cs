using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Teklif bilgisini tutan HRCTEKLIF veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTeklif : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTeklif(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Teklif bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCTEKLIF";

        /// <summary>
        /// Teklif bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi) 
            /// </summary>
            BIRIMKOD = 2,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 3,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 4,
            /// <summary>
            /// MALZEMENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMENO = 5,
            /// <summary>
            /// FIRMANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMANO = 6,
            /// <summary>
            /// DONEMNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DONEMNO = 7,
            /// <summary>
            /// BIRIMFIYAT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 8,
            /// <summary>
            /// KAZANAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAZANAN = 9,
            /// <summary>
            /// MALZEMEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMEKOD = 10,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 11,
            PARABIRIMI = 12
        }

        /// <summary>
        /// Teklif tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TIP", "MALZEMENO", "FIRMANO", "DONEMNO", "BIRIMFIYAT", "KAZANAN", "MALZEMEKOD", "ACIKLAMA", "PARABIRIMI" };

        /// <summary>
        /// Teklif bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek teklif bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).yil) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).muhasebe) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).birim) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).surecNo) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).tip) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).malzeme.malzemeNo) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).firma.firmaNo) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).donem.donemNo) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).birimFiyat) + ",";
                sql += VeriErisim.DegerYap(OrtakFonksiyonlar.ConvertToInt(((Teklif)bilgi.objeler[i]).kazanan, 0)) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).malzeme.malzemeKod) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).aciklama) + ",";
                sql += VeriErisim.DegerYap(((Teklif)bilgi.objeler[i]).yMParaBirimi) + ")";
                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Teklif bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek teklif bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(Teklif bilgi, bool ekleFirma)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (bilgi.tip)));
            if (bilgi.firma.firmaNo > 0 && ekleFirma)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMANO], "=", (bilgi.firma.firmaNo)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MALZEMENO], "=", bilgi.malzeme.malzemeNo));

            }
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Teklif bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif bilgisi kıstas parametresi</param>
        /// <returns>
        /// Teklif bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(Teklif bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT HRCTEKLIF.*, HRCTEKLIFDONEM.TARIH, HRCTEKLIFMALZEME.ADI, HRCTEKLIFMALZEME.MIKTAR, HRCTEKLIFMALZEME.OLCUBIRIMKOD,HRCOLCUBIRIM.AD,HRCTEKLIFMALZEME.MALZEMEKOD";

            if ((bilgi.tip == (int)ENUMTeklifTuru.YAKLASIKMALIYETFIRMA) || (bilgi.tip == (int)ENUMTeklifTuru.PIYASAFIYATARASTIRMASI)
                || (bilgi.tip == (int)ENUMTeklifTuru.KISMITEKLIF))
            {
                sql += ", HRCFIRMABILGISI.AD, HRCFIRMABILGISI.ADRES, HRCFIRMABILGISI.KOD ";
                sql += " FROM HRCFIRMABILGISI INNER JOIN";
                sql += " HRCTEKLIF ON HRCFIRMABILGISI.KOD = HRCTEKLIF.FIRMANO AND HRCFIRMABILGISI.MUHASEBEKOD = HRCTEKLIF.MUHASEBEKOD LEFT OUTER JOIN";
                sql += " HRCOLCUBIRIM INNER JOIN";
                sql += " HRCTEKLIFMALZEME ON HRCOLCUBIRIM.KOD = HRCTEKLIFMALZEME.OLCUBIRIMKOD ON HRCTEKLIF.YIL = HRCTEKLIFMALZEME.YIL AND ";
                sql += " HRCTEKLIF.SURECNO = HRCTEKLIFMALZEME.SURECNO AND HRCTEKLIF.MALZEMENO = HRCTEKLIFMALZEME.MALZEMENO AND ";
                sql += " HRCTEKLIF.TIP = HRCTEKLIFMALZEME.TIP LEFT OUTER JOIN";
                sql += " HRCTEKLIFDONEM ON HRCTEKLIF.YIL = HRCTEKLIFDONEM.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFDONEM.SURECNO AND ";
                sql += " HRCTEKLIF.DONEMNO = HRCTEKLIFDONEM.DONEMNO LEFT OUTER JOIN";
                sql += " HRCTEKLIFFIRMA ON HRCTEKLIF.YIL = HRCTEKLIFFIRMA.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFFIRMA.SURECNO AND ";
                sql += " HRCTEKLIF.TIP = HRCTEKLIFFIRMA.TIP AND HRCTEKLIF.FIRMANO = HRCTEKLIFFIRMA.FIRMANO";
            }
            else if ((bilgi.tip == (int)ENUMTeklifTuru.YAKLASIKMALIYETDONEM))
            {
                sql += " FROM HRCTEKLIF LEFT OUTER JOIN";
                sql += " HRCOLCUBIRIM INNER JOIN";
                sql += " HRCTEKLIFMALZEME ON HRCOLCUBIRIM.KOD = HRCTEKLIFMALZEME.OLCUBIRIMKOD ON HRCTEKLIF.YIL = HRCTEKLIFMALZEME.YIL AND ";
                sql += " HRCTEKLIF.SURECNO = HRCTEKLIFMALZEME.SURECNO AND HRCTEKLIF.MALZEMENO = HRCTEKLIFMALZEME.MALZEMENO AND ";
                sql += " HRCTEKLIF.TIP = HRCTEKLIFMALZEME.TIP LEFT OUTER JOIN";
                sql += " HRCTEKLIFDONEM ON HRCTEKLIF.YIL = HRCTEKLIFDONEM.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFDONEM.SURECNO AND ";
                sql += " HRCTEKLIF.DONEMNO = HRCTEKLIFDONEM.DONEMNO LEFT OUTER JOIN";
                sql += " HRCTEKLIFFIRMA ON HRCTEKLIF.YIL = HRCTEKLIFFIRMA.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFFIRMA.SURECNO AND ";
                sql += " HRCTEKLIF.TIP = HRCTEKLIFFIRMA.TIP AND HRCTEKLIF.FIRMANO = HRCTEKLIFFIRMA.FIRMANO";
            }

            int yilSurecSay = 0;
            if (bilgi.yil > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                yilSurecSay++;
            }
            if (bilgi.surecNo != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
                yilSurecSay++;
            }
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

            if (yilSurecSay < 2 && bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

            if (yilSurecSay < 2 && bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));

            if (bilgi.kazanan > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAZANAN], "=", bilgi.kazanan));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1,2,3,4,5,6,7,8";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KazananGetir(int yil, string surecNo, int malzemeNo)
        {
            string sql = "SELECT FIRMANO, TIP ,HRCFIRMABILGISI.MUHASEBEKOD ,HRCFIRMABILGISI.VERGINO FROM HRCTEKLIF INNER JOIN HRCFIRMABILGISI ON HRCTEKLIF.MUHASEBEKOD = HRCFIRMABILGISI.MUHASEBEKOD AND HRCTEKLIF.FIRMANO = HRCFIRMABILGISI.KOD ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", surecNo));
            if (malzemeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "MALZEMENO", "=", malzemeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", "(KAZANAN = 1 OR KAZANAN = 2)");

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kısmi teklif kazanan bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif bilgisi kıstas parametresi</param>
        /// <returns>
        /// Kısmi teklif kazanan bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleKismiTeklifKazanan(Teklif bilgi)
        {
            string sql = "";
            string kosul = "";

            //sql = "SELECT HRCTEKLIF.*, HRCTEKLIFDONEM.TARIH, HRCTEKLIFMALZEME.ADI, HRCTEKLIFMALZEME.MIKTAR, HRCTEKLIFMALZEME.OLCUBIRIMKOD,HRCOLCUBIRIM.AD,HRCTEKLIFMALZEME.MALZEMEKOD";


            //sql += ", HRCFIRMABILGISI.AD, HRCFIRMABILGISI.ADRES, HRCFIRMABILGISI.KOD ";
            //sql += " FROM HRCFIRMABILGISI INNER JOIN";
            //sql += " HRCTEKLIF ON HRCFIRMABILGISI.KOD = HRCTEKLIF.FIRMANO AND HRCFIRMABILGISI.MUHASEBEKOD = HRCTEKLIF.MUHASEBEKOD LEFT OUTER JOIN";
            //sql += " HRCOLCUBIRIM INNER JOIN";
            //sql += " HRCTEKLIFMALZEME ON HRCOLCUBIRIM.KOD = HRCTEKLIFMALZEME.OLCUBIRIMKOD ON HRCTEKLIF.YIL = HRCTEKLIFMALZEME.YIL AND ";
            //sql += " HRCTEKLIF.SURECNO = HRCTEKLIFMALZEME.SURECNO AND HRCTEKLIF.MALZEMENO = HRCTEKLIFMALZEME.MALZEMENO AND ";
            //sql += " HRCTEKLIF.TIP = HRCTEKLIFMALZEME.TIP LEFT OUTER JOIN";
            //sql += " HRCTEKLIFDONEM ON HRCTEKLIF.YIL = HRCTEKLIFDONEM.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFDONEM.SURECNO AND ";
            //sql += " HRCTEKLIF.DONEMNO = HRCTEKLIFDONEM.DONEMNO LEFT OUTER JOIN";
            //sql += " HRCTEKLIFFIRMA ON HRCTEKLIF.YIL = HRCTEKLIFFIRMA.YIL AND HRCTEKLIF.SURECNO = HRCTEKLIFFIRMA.SURECNO AND ";
            //sql += " HRCTEKLIF.TIP = HRCTEKLIFFIRMA.TIP AND HRCTEKLIF.FIRMANO = HRCTEKLIFFIRMA.FIRMANO";

            sql = "SELECT DISTINCT HRCTEKLIF.FIRMANO, HRCFIRMABILGISI.AD, HRCFIRMABILGISI.VERGINO " +
                    " FROM HRCTEKLIF LEFT OUTER JOIN " +
                    " HRCFIRMABILGISI ON HRCTEKLIF.MUHASEBEKOD = HRCFIRMABILGISI.MUHASEBEKOD AND " +
                    " HRCTEKLIF.FIRMANO = HRCFIRMABILGISI.KOD ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", "(HRCTEKLIF.KAZANAN = 1 OR HRCTEKLIF.KAZANAN = 2)");

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1,2";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yaklaşık maliyet ortalama birim fiyat bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif bilgisi kıstas parametresi</param>
        /// <returns>
        /// Yaklaşık maliyet ortalama birim fiyat bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleYMOrtBirimFiyat(Teklif bilgi)
        {
            string sql = "";


            sql = "SELECT   HRCTEKLIF.MALZEMENO, " +
                    "SUM(HRCTEKLIF.BIRIMFIYAT) AS TOPLAM , " +
                    "COUNT(HRCTEKLIF.FIRMANO) AS FIRMASAYISI, " +
                    "SUM(HRCTEKLIF.BIRIMFIYAT)/COUNT(HRCTEKLIF.FIRMANO) AS ORTALAMA " +
                "FROM    HRCTEKLIF INNER JOIN " +
                      "HRCTEKLIFMALZEME ON HRCTEKLIF.YIL = HRCTEKLIFMALZEME.YIL AND " +
                      "HRCTEKLIF.MUHASEBEKOD = HRCTEKLIFMALZEME.MUHASEBEKOD AND " +
                      "HRCTEKLIF.BIRIMKOD = HRCTEKLIFMALZEME.BIRIMKOD AND " +
                      "HRCTEKLIF.SURECNO = HRCTEKLIFMALZEME.SURECNO AND " +
                      "HRCTEKLIF.MALZEMENO = HRCTEKLIFMALZEME.MALZEMENO " +
                "WHERE   (HRCTEKLIF.YIL = " + bilgi.yil + ") AND " +
                        "(HRCTEKLIF.SURECNO = " + bilgi.surecNo + ") AND " +
                        "(HRCTEKLIF.TIP = 1)" +
                "GROUP BY HRCTEKLIF.MALZEMENO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Teklif firma bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek teklif firma bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc FirmaSil(FirmaAnahtari bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMANO], "=", bilgi.firmaKodu));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Alım harcama bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="ay">Listeleme için kullanılacak ay kıstas parametresi</param>
        /// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        /// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        /// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        /// <param name="peb">Listeleme için kullanılacak alt faaliyet kod kıstas parametresi</param>
        /// <returns>
        /// Alım harcama bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleButceHazirlikAlimHarcama(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        {
            string sql = string.Empty;

            sql = "SELECT     YEAR(HRCIHALEONAYFORM.BELGETARIH) AS ISLEMYIL, MONTH(HRCIHALEONAYFORM.BELGETARIH) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL, " +
                      " HRCIHALEONAYFORM.SURECNO, HRCTEKLIF.MUHASEBEKOD, HRCTEKLIF.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD,  " +
                      " HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, SUM(HRCTEKLIF.BIRIMFIYAT * HRCTEKLIFMALZEME.MIKTAR)  " +
                      " AS TOPLAM, HRCSUREC.YILLARASARIISLEMMI " +
                    " FROM         HRCTEKLIF LEFT OUTER JOIN " +
                      " HRCIHALEONAYFORM ON HRCTEKLIF.YIL = HRCIHALEONAYFORM.YIL AND HRCTEKLIF.SURECNO = HRCIHALEONAYFORM.SURECNO LEFT OUTER JOIN " +
                      " HRCTEKLIFMALZEME ON HRCTEKLIF.MALZEMENO = HRCTEKLIFMALZEME.MALZEMENO AND HRCTEKLIF.YIL = HRCTEKLIFMALZEME.YIL AND  " +
                      " HRCTEKLIF.SURECNO = HRCTEKLIFMALZEME.SURECNO LEFT OUTER JOIN " +
                      " HRCIHALEONAYTERTIP ON HRCTEKLIF.YIL = HRCIHALEONAYTERTIP.YIL AND HRCTEKLIF.SURECNO = HRCIHALEONAYTERTIP.SURECNO LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCTEKLIF.YIL = HRCSUREC.YIL AND HRCTEKLIF.SURECNO = HRCSUREC.SURECNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (HRCTEKLIF.TIP = 3) AND (HRCTEKLIF.KAZANAN = 1) AND (HRCTEKLIFMALZEME.TIP = 3) ";

            if (yil > 0)
                sql += " AND (YEAR(HRCIHALEONAYFORM.BELGETARIH)  = " + yil + ") ";
            if (ay > 0)
                sql += " AND (MONTH(HRCIHALEONAYFORM.BELGETARIH)  = " + ay + ") ";
            if (muhasebe > 0)
                sql += " AND (HRCTEKLIF.MUHASEBEKOD = " + muhasebe + ") ";
            if (!string.IsNullOrEmpty(birim))
                sql += " AND (HRCTEKLIF.BIRIMKOD = '" + birim + "') ";
            if (!string.IsNullOrEmpty(kur))
                sql += " AND (HRCIHALEONAYTERTIP.KURKOD = '" + kur + "') ";
            if (!string.IsNullOrEmpty(fon))
                sql += " AND (HRCIHALEONAYTERTIP.FONKOD = '" + fon + "') ";
            if (!string.IsNullOrEmpty(fin))
                sql += " AND (HRCIHALEONAYTERTIP.FINKOD = '" + fin + "') ";
            if (!string.IsNullOrEmpty(eko))
                sql += " AND (HRCIHALEONAYTERTIP.EKOKOD = '" + eko + "') ";
            if (!string.IsNullOrEmpty(peb))
                sql += " AND (HRCIHALEONAYTERTIP.PEBKOD = '" + peb + "') ";

            sql += " GROUP BY YEAR(HRCIHALEONAYFORM.BELGETARIH), MONTH(HRCIHALEONAYFORM.BELGETARIH), HRCIHALEONAYFORM.SURECNO, HRCTEKLIF.MUHASEBEKOD, " +
                      " HRCTEKLIF.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, " +
                      " HRCIHALEONAYTERTIP.PEBKOD, HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL";

            sql += " UNION ";

            sql += "SELECT     YEAR(HRCHAKEDIS.SOZLESMETARIHI) AS ISLEMYIL, MONTH(HRCHAKEDIS.SOZLESMETARIHI) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL, " +
                      " HRCHAKEDIS.SURECNO, HRCHAKEDIS.MUHASEBEKOD, HRCHAKEDIS.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD,  " +
                      " HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD, SUM(HRCHAKEDIS.SOZLESMEBEDEL) AS TOPLAM,  " +
                      " HRCSUREC.YILLARASARIISLEMMI " +
                    " FROM         HRCHAKEDIS LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCHAKEDIS.YIL = HRCSUREC.YIL AND HRCHAKEDIS.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN " +
                      " HRCIHALEONAYTERTIP ON HRCHAKEDIS.YIL = HRCIHALEONAYTERTIP.YIL AND HRCHAKEDIS.SURECNO = HRCIHALEONAYTERTIP.SURECNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) ";

            if (yil > 0)
                sql += " AND (YEAR(HRCHAKEDIS.SOZLESMETARIHI)  = " + yil + ") ";
            if (ay > 0)
                sql += " AND (MONTH(HRCHAKEDIS.SOZLESMETARIHI)  = " + ay + ") ";
            if (muhasebe > 0)
                sql += " AND (HRCHAKEDIS.MUHASEBEKOD = " + muhasebe + ") ";
            if (!string.IsNullOrEmpty(birim))
                sql += " AND (HRCHAKEDIS.BIRIMKOD = '" + birim + "') ";
            if (!string.IsNullOrEmpty(kur))
                sql += " AND (HRCIHALEONAYTERTIP.KURKOD = '" + kur + "') ";
            if (!string.IsNullOrEmpty(fon))
                sql += " AND (HRCIHALEONAYTERTIP.FONKOD = '" + fon + "') ";
            if (!string.IsNullOrEmpty(fin))
                sql += " AND (HRCIHALEONAYTERTIP.FINKOD = '" + fin + "') ";
            if (!string.IsNullOrEmpty(eko))
                sql += " AND (HRCIHALEONAYTERTIP.EKOKOD = '" + eko + "') ";
            if (!string.IsNullOrEmpty(peb))
                sql += " AND (HRCIHALEONAYTERTIP.PEBKOD = '" + peb + "') ";

            sql += " GROUP BY YEAR(HRCHAKEDIS.SOZLESMETARIHI), MONTH(HRCHAKEDIS.SOZLESMETARIHI), HRCHAKEDIS.SURECNO, HRCHAKEDIS.MUHASEBEKOD, " +
                      " HRCHAKEDIS.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD, HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, " +
                      " HRCIHALEONAYTERTIP.PEBKOD, HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL";

            sql += " UNION ";

            sql += "SELECT     YEAR(HRCIHALEONAYFORM.BELGETARIH) AS ISLEMYIL, MONTH(HRCIHALEONAYFORM.BELGETARIH) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL,  " +
                      " HRCIHALEONAYFORM.SURECNO, HRCIHALEONAYFORM.MUHASEBEKOD, HRCIHALEONAYFORM.BIRIMKOD, HRCIHALEONAYTERTIP.KURKOD,  " +
                      " HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD,  " +
                      " SUM(HRCIHALEONAYTERTIP.KULLANILACAKTUTAR) AS TOPLAM, HRCSUREC.YILLARASARIISLEMMI " +
                    " FROM         HRCIHALEONAYFORM LEFT OUTER JOIN " +
                      " HRCIHALEONAYTERTIP ON HRCIHALEONAYFORM.YIL = HRCIHALEONAYTERTIP.YIL AND  " +
                      " HRCIHALEONAYFORM.SURECNO = HRCIHALEONAYTERTIP.SURECNO LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCIHALEONAYFORM.YIL = HRCSUREC.YIL AND HRCIHALEONAYFORM.SURECNO = HRCSUREC.SURECNO " +
                    " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (HRCSUREC.ISLEMKOD = 18) ";

            if (yil > 0)
                sql += " AND (YEAR(HRCIHALEONAYFORM.BELGETARIH) = " + yil + ") ";
            if (ay > 0)
            {
                sql += " AND (MONTH(HRCIHALEONAYFORM.BELGETARIH) = " + ay + ") ";
            }
            if (muhasebe > 0)
            {
                sql += " AND (HRCIHALEONAYFORM.MUHASEBEKOD = " + muhasebe + ") ";
            }
            if (!string.IsNullOrEmpty(birim))
            {
                sql += " AND (HRCIHALEONAYFORM.BIRIMKOD = '" + birim + "') ";
            }
            if (!string.IsNullOrEmpty(kur))
            {
                sql += " AND (HRCIHALEONAYTERTIP.KURKOD = '" + kur + "') ";
            }
            if (!string.IsNullOrEmpty(fon))
            {
                sql += " AND (HRCIHALEONAYTERTIP.FONKOD = '" + fon + "') ";
            }
            if (!string.IsNullOrEmpty(fin))
            {
                sql += " AND (HRCIHALEONAYTERTIP.FINKOD = '" + fin + "') ";
            }
            if (!string.IsNullOrEmpty(eko))
            {
                sql += " AND (HRCIHALEONAYTERTIP.EKOKOD = '" + eko + "') ";
            }
            if (!string.IsNullOrEmpty(peb))
            {
                sql += " AND (HRCIHALEONAYTERTIP.PEBKOD = '" + peb + "') ";
            }

            sql += " GROUP BY HRCIHALEONAYFORM.MUHASEBEKOD, HRCIHALEONAYFORM.BIRIMKOD, HRCIHALEONAYFORM.SURECNO, HRCIHALEONAYTERTIP.KURKOD,  " +
                      " HRCIHALEONAYTERTIP.FONKOD, HRCIHALEONAYTERTIP.FINKOD, HRCIHALEONAYTERTIP.EKOKOD, HRCIHALEONAYTERTIP.PEBKOD,  " +
                      " YEAR(HRCIHALEONAYFORM.BELGETARIH), MONTH(HRCIHALEONAYFORM.BELGETARIH), HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL ";


            return veriErisim.DataReaderDondur(sql);
        }
    }
}
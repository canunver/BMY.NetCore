using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Teklif malzeme bilgisini tutan HRCTEKLIFMALZEME veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABTeklifMalzeme : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTeklifMalzeme(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Teklif malzeme bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCTEKLIFMALZEME";

        /// <summary>
        /// Teklif malzeme bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// MALZEMENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMENO = 4,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 5,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 6,
            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 7,
            /// <summary>
            /// OLCUBIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            OLCUBIRIMKOD = 8,
            /// <summary>
            /// MALZEMEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMEKOD = 9
        }

        /// <summary>
        /// Teklif malzeme tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "MALZEMENO", "TIP", "ADI", "MIKTAR", "OLCUBIRIMKOD", "MALZEMEKOD" };

        /// <summary>
        /// Teklif malzeme bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADTEKLIF = "HRCTEKLIF";

        /// <summary>
        /// Teklif malzeme bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarTeklif
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
            /// MALZEMENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MALZEMENO = 4,
            /// <summary>
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 5
        }

        /// <summary>
        /// Teklif malzeme tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTEKLIF = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "MALZEMENO", "TIP" };

        /// <summary>
        /// Teklif malzeme bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADOLCUBIRIM = "HRCOLCUBIRIM";

        /// <summary>
        /// Teklif malzeme bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarOlcuBirim
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1
        }

        /// <summary>
        /// Teklif malzeme tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAROLCUBIRIM = { "KOD", "AD" };

        /// <summary>
        /// Teklif malzeme bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek teklif malzeme bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            int no = 0;
            foreach (TeklifMalzeme m in bilgi.objeler)
            {
                if (m.malzemeNo > no)
                    no = m.malzemeNo;
            }

            foreach (TeklifMalzeme m in bilgi.objeler)
            {
                if (!m.sil)
                {
                    if (m.malzemeNo == 0)
                        m.malzemeNo = ++no;//primary key sıra no olarak kullanılamaz. araya malzeme no eklendiğinde en sona taşınmak zorunda. Çünkü daha sonra aradan malzeme çıkartıldığında pk ya yeni numara olarak sıradan no verilmesi çift kayıt sorununa neden oluyor. Ayırca teklif kaydı yapılmışsa onu da etkiliyor. Cumhurbaşkanlığı 13.04.2023 Melih ---Seçip kaydet ilerle deyince en alta bir ya da daha fazla kopyasını atıyor.

                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(m.yil) + ",";
                    sql += VeriErisim.DegerYap(m.muhasebe) + ",";
                    sql += VeriErisim.DegerYap(m.birim) + ",";
                    sql += VeriErisim.DegerYap(m.surecNo) + ",";
                    sql += VeriErisim.DegerYap(m.malzemeNo) + ",";
                    sql += VeriErisim.DegerYap(m.tip) + ",";
                    sql += VeriErisim.DegerYap(m.adi) + ",";
                    sql += VeriErisim.DegerYap(m.miktar) + ",";
                    sql += VeriErisim.DegerYap(m.olcuBirim.kod) + ",";
                    sql += VeriErisim.DegerYap(m.malzemeKod) + ")";

                    sonuc = KomutCalistir(sql, 1);
                    if (!sonuc.islemSonuc)
                    {
                        veriErisim.hata = "";
                        sql = "UPDATE " + TABLOAD + " SET ";
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], m.adi) + ",";
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], m.miktar) + ",";
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD], m.olcuBirim.kod) + ",";
                        sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MALZEMEKOD], m.malzemeKod);
                        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", m.yil));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", m.muhasebe));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", m.birim));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", m.surecNo));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MALZEMENO], "=", m.malzemeNo));
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", m.tip));

                        sonuc = KomutCalistir(sql, 1);
                    }
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Teklif malzeme bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek teklif malzeme bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ObjectArray bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (TeklifMalzeme m in bilgi.objeler)
            {
                if (m.sil)
                {
                   
                    sql = "DELETE FROM " + TABLOADTEKLIF;
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.YIL], "=", (m.yil)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.MUHASEBEKOD], "=", (m.muhasebe)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.BIRIMKOD], "=", (m.birim)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.SURECNO], "=", (m.surecNo)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.MALZEMENO], "=", (m.malzemeNo)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTEKLIF[(int)ENUMAlanlarTeklif.TIP], "=", (m.tip)));

                    sonuc = KomutCalistir(sql, 0);
                    if (!sonuc.islemSonuc)
                        break;

                    sql = "DELETE FROM " + TABLOAD;
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (m.yil)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", (m.muhasebe)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", (m.birim)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (m.surecNo)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MALZEMENO], "=", (m.malzemeNo)));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (m.tip)));

                    sonuc = KomutCalistir(sql, 1);
                    if (!sonuc.islemSonuc)
                        break;

                }
            }

            return sonuc;
        }

        /// <summary>
        /// Teklif malzeme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak teklif malzeme bilgisi kıstas parametresi</param>
        /// <returns>
        /// Teklif malzeme bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(TeklifMalzeme bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABLOADOLCUBIRIM + "." + ALANLAROLCUBIRIM[(int)ENUMAlanlarOlcuBirim.AD];
            sql += " FROM " + TABLOAD + "," + TABLOADOLCUBIRIM;

            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.OLCUBIRIMKOD] + "=" + TABLOADOLCUBIRIM + "." + ALANLAROLCUBIRIM[(int)ENUMAlanlarOlcuBirim.KOD]);

            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + ", " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " + ALANLAR[(int)ENUMAlanlar.MALZEMENO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kazanan firma teklif malzeme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası bilgisi kıstas parametresi</param>
        /// <param name="firmaNo">Listeleme için kullanılacak firma numarası bilgisi kıstas parametresi</param>
        /// <returns>
        /// Kazanan firma teklif malzeme bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleMalzemeKazananFirma(int yil, string surecNo, int firmaNo)
        {
            string sql = "";
            sql = " SELECT     HRCTEKLIFMALZEME.YIL, HRCTEKLIFMALZEME.MUHASEBEKOD, HRCTEKLIFMALZEME.BIRIMKOD, HRCTEKLIFMALZEME.SURECNO,  " +
                      " HRCTEKLIFMALZEME.MALZEMENO, HRCTEKLIFMALZEME.TIP, HRCTEKLIFMALZEME.ADI, HRCTEKLIFMALZEME.MIKTAR, HRCTEKLIFMALZEME.OLCUBIRIMKOD,  " +
                      " HRCTEKLIFMALZEME.MALZEMEKOD, HRCOLCUBIRIM.AD, HRCTEKLIF.BIRIMFIYAT " +
                    " FROM         HRCTEKLIFMALZEME INNER JOIN " +
                      " HRCOLCUBIRIM ON HRCTEKLIFMALZEME.OLCUBIRIMKOD = HRCOLCUBIRIM.KOD LEFT OUTER JOIN " +
                      " HRCTEKLIF ON HRCTEKLIFMALZEME.YIL = HRCTEKLIF.YIL AND HRCTEKLIFMALZEME.SURECNO = HRCTEKLIF.SURECNO AND  " +
                      " HRCTEKLIFMALZEME.MALZEMENO = HRCTEKLIF.MALZEMENO " +
                    " WHERE (HRCTEKLIFMALZEME.YIL = " + yil + ") AND " +
                          " (HRCTEKLIFMALZEME.SURECNO = '" + surecNo + "') AND " +
                          " (HRCTEKLIF.KAZANAN = 1) AND (HRCTEKLIF.FIRMANO = " + firmaNo + ") AND  " +
                          " (HRCTEKLIFMALZEME.TIP = 3)";

            return veriErisim.DataReaderDondur(sql);
        }


    }
}
using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Su fatura bilgisini tutan HRCABONEFATURASU veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABAboneFaturaSu : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAboneFaturaSu(VeriErisim veriErisim) : base(veriErisim)
        {
        }

        /// <summary>
        /// Abone su fatura tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCABONEFATURASU";

        /// <summary>
        /// Abone su fatura tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar : int
        {
            /// <summary>
            /// FATURAYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURAYIL = 0,
            /// <summary>
            /// FATURAAY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURAAY = 1,
            /// <summary>
            /// ISLETMEKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLETMEKODU = 2,
            /// <summary>
            /// HIZMETNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HIZMETNO = 3,
            /// <summary>
            /// ADSOYADUNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYADUNVAN = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// FATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURANO = 6,
            /// <summary>
            /// ADRES alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADRES = 7,
            /// <summary>
            /// TEBLIGTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEBLIGTARIH = 8,
            /// <summary>
            /// SONODEMETARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SONODEMETARIH = 9,
            /// <summary>
            /// SAYACNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SAYACNO = 10,
            /// <summary>
            /// ILKOKUMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILKOKUMA = 11,
            /// <summary>
            /// SONOKUMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SONOKUMA = 12,
            /// <summary>
            /// ILKENDEX alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILKENDEX = 13,
            /// <summary>
            /// SONENDEX alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SONENDEX = 14,
            /// <summary>
            /// TUKETIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUKETIM = 15,
            /// <summary>
            /// KIYAS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KIYAS = 16,
            /// <summary>
            /// CTV alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CTV = 17,
            /// <summary>
            /// KDV alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KDV = 18,
            /// <summary>
            /// SUBEDELI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SUBEDELI = 19,
            /// <summary>
            /// ATIKSU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ATIKSU = 20,
            /// <summary>
            /// SUBEYOLU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SUBEYOLU = 21,
            /// <summary>
            /// FATTOPTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATTOPTUTAR = 22,
            /// <summary>
            /// TAHSIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TAHSIL = 23,
            /// <summary>
            /// TENZIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TENZIL = 24,
            /// <summary>
            /// CEZA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CEZA = 25,
            /// <summary>
            /// KALAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KALAN = 26,
        }

        /// <summary>
        /// Abone su fatura tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "FATURAYIL", "FATURAAY", "ISLETMEKODU", "HIZMETNO", "ADSOYADUNVAN", "SIRANO", "FATURANO", "ADRES", "TEBLIGTARIH", "SONODEMETARIH", "SAYACNO", "ILKOKUMA", "SONOKUMA", "ILKENDEX", "SONENDEX", "TUKETIM", "KIYAS", "CTV", "KDV", "SUBEDELI", "ATIKSU", "SUBEYOLU", "FATTOPTUTAR", "TAHSIL", "TENZIL", "CEZA", "KALAN " };

        /// <summary>
        /// Abone su fatura bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="fatura">Eklenecek abone su fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(AboneFaturaSu fatura)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc(false, "Beklenmeyen bir hata oluştu", "x");

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql += VeriErisim.DegerYap(fatura.faturaYil) + ",";
            sql += VeriErisim.DegerYap(fatura.faturaAy) + ",";
            sql += VeriErisim.DegerYap(fatura.isletmeKodu.Trim()) + ",";
            sql += VeriErisim.DegerYap(fatura.hizmetNo.Trim()) + ",";
            sql += VeriErisim.DegerYap(fatura.aboneAd) + ",";
            sql += VeriErisim.DegerYap(fatura.siraNo) + ",";
            sql += VeriErisim.DegerYap(fatura.faturaNo) + ",";
            sql += VeriErisim.DegerYap(fatura.adresi) + ",";
            sql += VeriErisim.DegerYap(fatura.tebligTarih) + ",";
            sql += VeriErisim.DegerYap(fatura.sonOdemeTarih) + ",";
            sql += VeriErisim.DegerYap(fatura.sayacNo) + ",";
            sql += VeriErisim.DegerYap(fatura.ilkOkuma) + ",";
            sql += VeriErisim.DegerYap(fatura.sonOkuma) + ",";
            sql += VeriErisim.DegerYap(fatura.ilkEndex) + ",";
            sql += VeriErisim.DegerYap(fatura.sonEndex) + ",";
            sql += VeriErisim.DegerYap(fatura.tuketim) + ",";
            sql += VeriErisim.DegerYap(fatura.kiyas) + ",";
            sql += VeriErisim.DegerYap(fatura.ctvTutari) + ",";
            sql += VeriErisim.DegerYap(fatura.kdv) + ",";
            sql += VeriErisim.DegerYap(fatura.suBedeli) + ",";
            sql += VeriErisim.DegerYap(fatura.atikSu) + ",";
            sql += VeriErisim.DegerYap(fatura.subeYolu) + ",";
            sql += VeriErisim.DegerYap(fatura.fatTopTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.tahsil) + ",";
            sql += VeriErisim.DegerYap(fatura.tenzil) + ",";
            sql += VeriErisim.DegerYap(fatura.ceza) + ",";
            sql += VeriErisim.DegerYap(fatura.kalan) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// Abone su fatura bilgisini veri tabanından çeken sql cümleciğini verilen abone su fatura kıstasına göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek abone su fatura için kıstas parametresi</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> verilirse süreç oluşturulmamış faturaları listeler</param>
        /// <returns>
        /// Abone su fatura bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(AboneFaturaSu bilgi, bool sadeceYeniler)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLAR) +
                "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.MUHASEBEKOD] +
                "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.BIRIMKOD] +
                "," + TABIdareBilgi.TABLOAD + "." + TABIdareBilgi.ALANLAR[(int)TABIdareBilgi.ENUMAlanlar.EPOSTA];
            sql += " FROM HRCIDAREBILGI RIGHT OUTER JOIN";
            sql += " HRCABONENO ON HRCIDAREBILGI.MUHASEBEKOD = HRCABONENO.MUHASEBEKOD AND ";
            sql += " HRCIDAREBILGI.BIRIMKOD = HRCABONENO.BIRIMKOD LEFT OUTER JOIN";
            sql += " HRCABONEFATURASU A ON HRCABONENO.ABONENO = A.HIZMETNO AND HRCABONENO.ISLETMEKODU = A.ISLETMEKODU";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", bilgi.faturaYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", bilgi.faturaAy));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABAboneNo.TABLOAD, TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.ABONETIP], "=", (int)ENUMAboneTip.SU));
            if (bilgi.hizmetNo != "")
            {
                string kosul = "";
                string[] h = bilgi.hizmetNo.Split(';');
                for (int i = 0; i < h.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIZMETNO], "=", h[i]));

                if (kosul != "")
                    sql += " AND (" + kosul + ")";
            }
            if (sadeceYeniler)//Süreç oluşturulmamışlar listelemek için
                sql += " AND A.HIZMETNO NOT IN (SELECT ABONENO FROM HRCABONEFATURA B,HRCSUREC WHERE HRCSUREC.YIL=B.YIL AND HRCSUREC.SURECNO=B.SURECNO AND A.FATURAYIL = B.FATURAYIL AND A.FATURAAY = B.FATURAAY AND A.HIZMETNO = B.ABONENO AND HRCABONENO.MUHASEBEKOD=B.MUHASEBEKOD)";//AND HRCSUREC.SURECDURUM<9 //daha önce kayıt ettiğimiz fakat kullanıcının ödeme yapmış olmasından dolayı iptal ettiği süreçleride dikkate almalıyız

            sql += " ORDER BY " + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.MUHASEBEKOD] + "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.BIRIMKOD] + ",A.SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone su fatura bilgisinin kayıtlı olup olmadığını veri tabanından çeken sql cümleciğini verilen abone su fatura kıstasına göre 
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="abone">Listelenecek abone su fatura için kıstas parametresi</param>
        /// <returns>Abone elektrik fatura bilgisinin kayıtlı olup olmadığı bilgisini bulmak için oluşturulan 
        /// select komutunun çalıştırılması sonucu elde edilen string değeri döndürür</returns>
        public string KayitVarMi(AboneFaturaSu abone)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HIZMETNO] + " FROM " + TABLOAD;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLETMEKODU], "=", abone.isletmeKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", abone.faturaYil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", abone.faturaAy));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIZMETNO], "=", abone.hizmetNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Su faturası abonelerine ait mail bilgisini veri tabanından çeken sql cümleciğini verilen abone su fatura kıstasına göre 
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelemede kıstas olarak kullanılacak nesne</param>
        /// <returns>Su faturası abonelerine ait mail bilgisini bulmak için oluşturulan select 
        /// komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader MailIcinListe(AboneFaturaSu bilgi)
        {
            string sql = "";
            sql = "SELECT HRCABONENO.MUHASEBEKOD, HRCABONENO.BIRIMKOD, HRCIDAREBILGI.EPOSTA, HRCABONEFATURA.ODEMEEMRINO, HRCABONEFATURA.SURECNO, A.HIZMETNO, A.SONODEMETARIH, A.FATTOPTUTAR, A.ISLETMEKODU,HRCSUREC.SURECTARIH";
            sql += " FROM HRCSUREC INNER JOIN HRCABONEFATURA ON HRCSUREC.YIL = HRCABONEFATURA.YIL AND HRCSUREC.SURECNO = HRCABONEFATURA.SURECNO INNER JOIN";
            sql += " HRCABONENO ON HRCABONEFATURA.MUHASEBEKOD = HRCABONENO.MUHASEBEKOD AND ";
            sql += " HRCABONEFATURA.BIRIMKOD = HRCABONENO.BIRIMKOD LEFT OUTER JOIN";
            sql += " HRCABONEFATURASU A ON HRCABONEFATURA.ABONENO = A.HIZMETNO AND HRCABONEFATURA.FATURAAY = A.FATURAAY AND ";
            sql += " HRCABONEFATURA.FATURAYIL = A.FATURAYIL AND HRCABONENO.ABONENO = A.HIZMETNO LEFT OUTER JOIN";
            sql += " HRCIDAREBILGI ON HRCABONENO.MUHASEBEKOD = HRCIDAREBILGI.MUHASEBEKOD AND ";
            sql += " HRCABONENO.BIRIMKOD = HRCIDAREBILGI.BIRIMKOD";
            sql += " WHERE A.FATURAYIL = " + bilgi.faturaYil;
            sql += " AND A.FATURAAY = " + bilgi.faturaAy;
            sql += " AND HRCABONENO.ABONETIP =" + (int)ENUMAboneTip.SU;
            sql += " AND EPOSTA IS NOT NULL AND EPOSTA<>''";
            sql += " ORDER BY HRCABONENO.MUHASEBEKOD, HRCABONENO.BIRIMKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone su fatura istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstaslara göre 
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listelemede kıstas olarak kullanılacak yıl bilgisi</param>
        /// <param name="muhasebe">Listelemede kıstas olarak kullanılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">Listelemede kıstas olarak kullanılacak birim kod bilgis.</param>
        /// <returns>Abone su fatura istatistik bilgisini bulmak için oluşturulan select komutunun 
        /// çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleSuIstatistik(int yil, int muhasebe, string birim)
        {
            string sql = "";
            sql = "SELECT     HRCABONEFATURA.FATURAYIL, HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD, SUM(HRCABONEFATURA.FATURATUTAR) AS TOPLAM, "+
                      " SUM(HRCABONEFATURASU.TUKETIM) AS TUKETIM "+ 
                    " FROM         HRCABONEFATURA LEFT OUTER JOIN "+
                      " HRCABONEFATURASU ON HRCABONEFATURA.ABONENO = HRCABONEFATURASU.HIZMETNO AND "+
                      " HRCABONEFATURA.FATURAAY = HRCABONEFATURASU.FATURAAY AND HRCABONEFATURA.FATURAYIL = HRCABONEFATURASU.FATURAYIL LEFT OUTER JOIN "+
                      " HRCSUREC ON HRCABONEFATURA.YIL = HRCSUREC.YIL AND HRCABONEFATURA.SURECNO = HRCSUREC.SURECNO "+
                    " WHERE     (HRCABONEFATURA.ABONETIP = 2) AND (HRCABONEFATURA.MUHASEBEKOD = " + muhasebe + ") AND (HRCABONEFATURA.BIRIMKOD = '" + birim + "') AND (HRCABONEFATURA.FATURAYIL = " + yil + ") AND (HRCABONEFATURA.ODEMEEMRINO <> '') " +
                    " GROUP BY HRCABONEFATURA.FATURAYIL, HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

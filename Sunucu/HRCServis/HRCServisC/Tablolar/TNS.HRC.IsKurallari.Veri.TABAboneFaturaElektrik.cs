using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Elektrik fatura bilgisini tutan HRCABONEFATURAELEKTRIK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABAboneFaturaElektrik : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAboneFaturaElektrik(VeriErisim veriErisim) : base(veriErisim)
        {
        }

        /// <summary>
        /// Abone Fatura Elektrik tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCABONEFATURAELEKTRIK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
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
            /// DOSYANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOSYANO = 5,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 6,
            /// <summary>
            /// ADRES alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADRES = 7,
            /// <summary>
            /// DOCNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOCNO = 8,
            /// <summary>
            /// TARIFE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TARIFE = 9,
            /// <summary>
            /// FATTOPTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATTOPTUTAR = 10,
            /// <summary>
            /// ODEMEDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEDURUM = 11,
            /// <summary>
            /// TUKETIMADKTIFKWH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUKETIMAKTIFKWH = 12,
            /// <summary>
            /// TUKETIMREAKTIFKWH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUKETIMREAKTIFKWH = 13,
            /// <summary>
            /// SONODEMETARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SONODEMETARIH = 14,
        }

        /// <summary>
        /// Abone Elektrik Fatura tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "FATURAYIL", "FATURAAY", "ISLETMEKODU", "HIZMETNO", "ADSOYADUNVAN", "DOSYANO", "SIRANO", "ADRES", "DOCNO", "TARIFE", "FATTOPTUTAR", "ODEMEDURUM", "TUKETIMAKTIFKWH ", "TUKETIMREAKTIFKWH", "SONODEMETARIH" };

        /// <summary>
        /// Abone Elektrik Fatura Sayaç tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSAYAC = "HRCABONEFATURAELEKTRIKSAYAC";

        /// <summary>
        /// Abone Elektrik Fatura Sayaç tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarSayac : int
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
            /// SERINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SERINO = 4,
            /// <summary>
            /// MARKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MARKA = 5,
            /// <summary>
            /// ENERJIOLCUMCINS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ENERJIOLCUMCINS = 6,
            /// <summary>
            /// HANETAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HANETAM = 7,
            /// <summary>
            /// HANEKESIR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HANEKESIR = 8,
            /// <summary>
            /// SONOKUMATARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SONOKUMATARIH = 9
        }

        /// <summary>
        /// Abone Elektrik Fatura Sayaç tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSAYAC = { "FATURAYIL", "FATURAAY", "ISLETMEKODU", "HIZMETNO", "SERINO", "MARKA", "ENERJIOLCUMCINS", "HANETAM", "HANEKESIR", "SONOKUMATARIH" };


        /// <summary>
        /// Abone elektrik fatura bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="fatura">Eklenecek abone elektrik fatura bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(AboneFaturaElektrik fatura)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc(false, "Beklenmeyen bir hata oluştu", "x");

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql += VeriErisim.DegerYap(fatura.faturaYil) + ",";
            sql += VeriErisim.DegerYap(fatura.faturaAy) + ",";
            sql += VeriErisim.DegerYap(fatura.isletmeKodu.Trim()) + ",";
            sql += VeriErisim.DegerYap(fatura.hizmetNo.Trim()) + ",";
            sql += VeriErisim.DegerYap(fatura.adSoyadUnvan) + ",";
            sql += VeriErisim.DegerYap(fatura.dosyaNo) + ",";
            sql += VeriErisim.DegerYap(fatura.siraNo) + ",";
            sql += VeriErisim.DegerYap(fatura.adres) + ",";
            sql += VeriErisim.DegerYap(fatura.docNo) + ",";
            sql += VeriErisim.DegerYap(fatura.tarife) + ",";
            sql += VeriErisim.DegerYap(fatura.fatTopTutar) + ",";
            sql += VeriErisim.DegerYap(fatura.odemeDurum) + ",";
            sql += VeriErisim.DegerYap(fatura.tuketimAktifKwh) + ",";
            sql += VeriErisim.DegerYap(fatura.tuketimReaktifKwh) + ",";
            sql += VeriErisim.DegerYap(fatura.sonOdemeTarih) + ")";

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Abone elektrik fatura sayaç bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sayac">Eklenecek abone elektrik fatura sayaç bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetSayacBilgi(AboneFaturaElektrikSayac sayac)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc(false, "Beklenmeyen bir hata oluştu", "x");

            sql = "INSERT INTO " + TABLOADSAYAC + " (" + VeriErisim.TumAlanlar("", ALANLARSAYAC) + ") VALUES (";

            sql += VeriErisim.DegerYap(sayac.faturaYil) + ",";
            sql += VeriErisim.DegerYap(sayac.faturaAy) + ",";
            sql += VeriErisim.DegerYap(sayac.isletmeKodu.Trim()) + ",";
            sql += VeriErisim.DegerYap(sayac.hizmetNo.Trim()) + ",";
            sql += VeriErisim.DegerYap(sayac.seriNo) + ",";
            sql += VeriErisim.DegerYap(sayac.marka) + ",";
            sql += VeriErisim.DegerYap(sayac.enerjiOlcumCins) + ",";
            sql += VeriErisim.DegerYap(sayac.haneTam) + ",";
            sql += VeriErisim.DegerYap(sayac.haneKesir) + ",";
            sql += VeriErisim.DegerYap(sayac.sonOkumaTarih) + ")";

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Abone elektrik fatura bilgisini veri tabanından çeken sql cümleciğini verilen abone elektrik fatura kıstasına göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelenecek abone elektrik fatura için kıstas parametresi</param>
        /// <param name="sadeceYeniler">Eğer <c>true</c> verilirse süreç oluşturulmamış faturaları listeler</param>
        /// <returns>
        /// Abone elektrik fatura bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(AboneFaturaElektrik bilgi, bool sadeceYeniler)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLAR) +
                "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.MUHASEBEKOD] +
                "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.BIRIMKOD] +
                "," + TABIdareBilgi.TABLOAD + "." + TABIdareBilgi.ALANLAR[(int)TABIdareBilgi.ENUMAlanlar.EPOSTA];
            sql += " FROM HRCIDAREBILGI RIGHT OUTER JOIN";
            sql += " HRCABONENO ON HRCIDAREBILGI.MUHASEBEKOD = HRCABONENO.MUHASEBEKOD AND ";
            sql += " HRCIDAREBILGI.BIRIMKOD = HRCABONENO.BIRIMKOD LEFT OUTER JOIN";
            sql += " HRCABONEFATURAELEKTRIK A ON HRCABONENO.ABONENO = A.HIZMETNO AND HRCABONENO.ISLETMEKODU = A.ISLETMEKODU";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", bilgi.faturaYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", bilgi.faturaAy));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABAboneNo.TABLOAD, TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.ABONETIP], "=", (int)ENUMAboneTip.ELEKTRIK));
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
                sql += " AND A.HIZMETNO NOT IN (SELECT ABONENO FROM HRCABONEFATURA B,HRCSUREC WHERE HRCSUREC.YIL=B.YIL AND HRCSUREC.SURECNO=B.SURECNO AND A.FATURAYIL = B.FATURAYIL AND A.FATURAAY = B.FATURAAY AND A.HIZMETNO = B.ABONENO AND HRCABONENO.MUHASEBEKOD=B.MUHASEBEKOD)";// AND HRCSUREC.SURECDURUM<9//daha önce kayıt ettiğimiz fakat kullanıcının ödeme yapmış olmasından dolayı iptal ettiği süreçleride dikkate almalıyız

            sql += " ORDER BY " + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.MUHASEBEKOD] + "," + TABAboneNo.TABLOAD + "." + TABAboneNo.ALANLAR[(int)TABAboneNo.ENUMAlanlar.BIRIMKOD] + ",A.SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone elektrik fatura bilgisinin kayıtlı olup olmadığını veri tabanından çeken sql cümleciğini verilen abone elektrik fatura kıstasına göre 
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="abone">Listelenecek abone elektrik fatura için kıstas parametresi</param>
        /// <returns>Abone elektrik fatura bilgisinin kayıtlı olup olmadığı bilgisini bulmak için oluşturulan 
        /// select komutunun çalıştırılması sonucu elde edilen string değeri döndürür</returns>
        public string KayitVarMi(AboneFaturaElektrik abone)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HIZMETNO] + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HIZMETNO], "=", abone.hizmetNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLETMEKODU], "=", abone.isletmeKodu));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAYIL], "=", abone.faturaYil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FATURAAY], "=", abone.faturaAy));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Elektrik faturası abonelerine ait mail bilgisini veri tabanından çeken sql cümleciğini verilen abone elektrik fatura kıstasına göre 
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listelemede kıstas olarak kullanılacak nesne</param>
        /// <returns>
        /// Elektrik faturası abonelerine ait mail bilgisini bulmak için oluşturulan select 
        /// komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür
        /// </returns>
        public IDataReader MailIcinListe(AboneFaturaElektrik bilgi)
        {
            string sql = "";
            sql = "SELECT HRCABONENO.MUHASEBEKOD, HRCABONENO.BIRIMKOD, HRCIDAREBILGI.EPOSTA, HRCABONEFATURA.ODEMEEMRINO, HRCABONEFATURA.SURECNO, A.HIZMETNO, A.SONODEMETARIH, A.FATTOPTUTAR, A.ISLETMEKODU,HRCSUREC.SURECTARIH";
            sql += " FROM HRCSUREC INNER JOIN HRCABONEFATURA ON HRCSUREC.YIL = HRCABONEFATURA.YIL AND HRCSUREC.SURECNO = HRCABONEFATURA.SURECNO INNER JOIN";
            sql += " HRCABONENO ON HRCABONEFATURA.MUHASEBEKOD = HRCABONENO.MUHASEBEKOD AND ";
            sql += " HRCABONEFATURA.BIRIMKOD = HRCABONENO.BIRIMKOD LEFT OUTER JOIN";
            sql += " HRCABONEFATURAELEKTRIK A ON HRCABONEFATURA.ABONENO = A.HIZMETNO AND HRCABONEFATURA.FATURAAY = A.FATURAAY AND ";
            sql += " HRCABONEFATURA.FATURAYIL = A.FATURAYIL AND HRCABONENO.ABONENO = A.HIZMETNO LEFT OUTER JOIN";
            sql += " HRCIDAREBILGI ON HRCABONENO.MUHASEBEKOD = HRCIDAREBILGI.MUHASEBEKOD AND ";
            sql += " HRCABONENO.BIRIMKOD = HRCIDAREBILGI.BIRIMKOD";
            sql += " WHERE A.FATURAYIL = " + bilgi.faturaYil;
            sql += " AND A.FATURAAY = " + bilgi.faturaAy;
            sql += " AND HRCABONENO.ABONETIP =" + (int)ENUMAboneTip.ELEKTRIK;
            sql += " AND EPOSTA IS NOT NULL AND EPOSTA<>''";
            sql += " ORDER BY HRCABONENO.MUHASEBEKOD, HRCABONENO.BIRIMKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Abone elektrik fatura istatistik bilgisini veri tabanından çeken sql cümleciğini verilen kıstaslara göre 
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listelemede kıstas olarak kullanılacak yıl bilgisi</param>
        /// <param name="muhasebe">Listelemede kıstas olarak kullanılacak muhasebe kod bilgisi.</param>
        /// <param name="birim">Listelemede kıstas olarak kullanılacak birim kod bilgis.</param>
        /// <returns>
        /// Abone elektrik fatura istatistik bilgisini bulmak için oluşturulan select komutunun 
        /// çalıştırılması sonucu elde edilen DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleElektrikIstatistik(int yil, int muhasebe, string birim)
        {
            string sql = "";
            sql = "SELECT     HRCABONEFATURA.FATURAYIL, HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD, " +
                      " SUM(HRCABONEFATURA.FATURATUTAR )AS TOPLAM, SUM(HRCABONEFATURAELEKTRIK.TUKETIMAKTIFKWH) AS TUKETIM " +
                    " FROM         HRCABONEFATURA LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCABONEFATURA.YIL = HRCSUREC.YIL AND HRCABONEFATURA.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN " +
                      " HRCABONEFATURAELEKTRIK ON HRCABONEFATURA.FATURAYIL = HRCABONEFATURAELEKTRIK.FATURAYIL AND " +
                      " HRCABONEFATURA.FATURAAY = HRCABONEFATURAELEKTRIK.FATURAAY AND HRCABONEFATURA.ABONENO = HRCABONEFATURAELEKTRIK.HIZMETNO AND " +
                      " HRCABONEFATURA.ISLETMENO = HRCABONEFATURAELEKTRIK.ISLETMEKODU " +
                    " WHERE     (HRCABONEFATURA.ABONETIP = 1) AND (HRCABONEFATURA.MUHASEBEKOD = " + muhasebe + ") AND (HRCABONEFATURA.BIRIMKOD = '" + birim + "') AND " +
                      " (HRCABONEFATURA.ODEMEEMRINO <> '') AND (HRCSUREC.SURECDURUM <> 9) AND (HRCABONEFATURA.FATURAYIL = " + yil + ") " +
                    " GROUP BY HRCABONEFATURA.FATURAYIL, HRCABONEFATURA.MUHASEBEKOD, HRCABONEFATURA.BIRIMKOD";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Firma bilgisini tutan HRCFIRMABILGISI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABFirmaBilgisi : TABOrtak
    {

        /*
         * 
         * 
         ALTER TABLE dbo.HRCFIRMABILGISI ADD
            BICSWIFT varchar(20) NULL,
            KISIUNVAN varchar(20) NULL,
            DERECE int NULL,
            EKGOSTERGE int NULL
         * 
         * */

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABFirmaBilgisi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Firma bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCFIRMABILGISI";

        /// <summary>
        /// Firma bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 2,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,
            /// <summary>
            /// VERGINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGINO = 4,
            /// <summary>
            /// VERGIDAIRESI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGIDAIRESI = 5,
            /// <summary>
            /// BANKA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BANKA = 6,
            /// <summary>
            /// HESAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPNO = 7,
            /// <summary>
            /// EMAIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EMAIL = 8,
            /// <summary>
            /// FIRMAYETKILISI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FIRMAYETKILISI = 9,
            /// <summary>
            /// TELEFON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TELEFON = 10,
            /// <summary>
            /// FAKS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FAKS = 11,
            /// <summary>
            /// ADRES alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADRES = 12,
            /// <summary>
            /// UYRUK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UYRUK = 13,
            /// <summary>
            /// GORULMESIN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GORULMESIN = 14,
            /// <summary>
            /// YASAKLI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YASAKLI = 15,
            /// <summary>
            /// CEPTEL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CEPTEL = 16,
            /// <summary>
            /// WEB alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            WEB = 17,
            /// <summary>
            /// FATURA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FATURA = 18,

            BICSWIFT = 19,
            KISIUNVAN = 20,
            DERECE = 21,
            EKGOSTERGE = 22,
            ACIKLAMA = 23,
            KARALISTE = 24,
            ADRESNO = 25
        }

        /// <summary>
        /// Firma bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "AD", "MUHASEBEKOD", "BIRIMKOD", "VERGINO", "VERGIDAIRESI", "BANKA", "HESAPNO", "EMAIL", "FIRMAYETKILISI", "TELEFON", "FAKS", "ADRES", "UYRUK", "GORULMESIN", "YASAKLI", "CEPTEL", "WEB", "FATURA", "BICSWIFT", "KISIUNVAN", "DERECE", "EKGOSTERGE", "ACIKLAMA", "KARALISTE", "ADRESNO" };

        /// <summary>
        /// Firma giriş tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADGIRIS = "HRCFIRMAGIRIS";

        /// <summary>
        /// Firma giriş tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarGiris
        {
            /// <summary>
            /// VERGINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGINO = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// PAROLA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PAROLA = 2,
            /// <summary>
            /// SONGIRISTARIHI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SONGIRISTARIHI = 3,
        }

        /// <summary>
        /// Firma giriş tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARGIRIS = { "VERGINO", "AD", "PAROLA", "SONGIRISTARIHI" };

        /// <summary>
        /// Firma bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek firma bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(FirmaBilgisi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.vergiNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.vergiDairesi) + ",";
            sql += VeriErisim.DegerYap(bilgi.banka) + ",";
            sql += VeriErisim.DegerYap(bilgi.hesapNo.Replace(" ", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.eMail) + ",";
            sql += VeriErisim.DegerYap(bilgi.firmaYetkilisi) + ",";
            sql += VeriErisim.DegerYap(bilgi.telefon) + ",";
            sql += VeriErisim.DegerYap(bilgi.faks) + ",";
            sql += VeriErisim.DegerYap(bilgi.adres) + ",";
            sql += VeriErisim.DegerYap(bilgi.uyruk) + ",";
            sql += VeriErisim.DegerYap(bilgi.gorulmesin) + ",";
            sql += VeriErisim.DegerYap(bilgi.yasakli) + ",";
            sql += VeriErisim.DegerYap(bilgi.cepTel) + ",";
            sql += VeriErisim.DegerYap(bilgi.web) + ",";
            sql += VeriErisim.DegerYap(bilgi.fatura);
            sql += "," + VeriErisim.DegerYap(bilgi.bicSwift);
            sql += "," + VeriErisim.DegerYap(bilgi.kisiUnvan);
            sql += "," + VeriErisim.DegerYap(bilgi.derece);
            sql += "," + VeriErisim.DegerYap(bilgi.ekGosterge);
            sql += "," + VeriErisim.DegerYap(bilgi.aciklama);
            sql += "," + VeriErisim.DegerYap(bilgi.karaListe);
            sql += "," + VeriErisim.DegerYap(bilgi.adresNo);

            sql += ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGINO], bilgi.vergiNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIDAIRESI], bilgi.vergiDairesi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKA], bilgi.banka) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPNO], bilgi.hesapNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMAIL], bilgi.eMail) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAYETKILISI], bilgi.firmaYetkilisi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], bilgi.telefon) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], bilgi.faks) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], bilgi.adres) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UYRUK], bilgi.uyruk) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GORULMESIN], bilgi.gorulmesin) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YASAKLI], bilgi.yasakli) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEPTEL], bilgi.cepTel) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.WEB], bilgi.web) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURA], bilgi.fatura);

                sql += "," + VeriErisim.AtamaOl("", "BICSWIFT", bilgi.bicSwift);
                sql += "," + VeriErisim.AtamaOl("", "KISIUNVAN", bilgi.kisiUnvan);
                sql += "," + VeriErisim.AtamaOl("", "DERECE", bilgi.derece);
                sql += "," + VeriErisim.AtamaOl("", "EKGOSTERGE", bilgi.ekGosterge);
                sql += "," + VeriErisim.AtamaOl("", "ACIKLAMA", bilgi.aciklama);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARALISTE], bilgi.karaListe);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRESNO], bilgi.adresNo);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VERGINO], "=", bilgi.vergiNo));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Firma bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek firma bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(FirmaBilgisi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VERGINO], "=", bilgi.vergiNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Firma bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak firma bilgisi kıstas parametresi</param>
        /// <returns>
        /// Firma bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(FirmaBilgisi bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            else
            {
                //  if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
                //else
                //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">", 0));

                if (bilgi.ad != "")
                {
                    string[] parcalar = bilgi.ad.Split(',', ';');
                    string icKosul = "";
                    foreach (string parca in parcalar)
                    {
                        if (parca.Trim() == "") continue;
                        if (icKosul != "") icKosul += " OR ";
                        icKosul = VeriErisim.KosulEkle(icKosul, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + parca + "%"));
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VERGINO], "LIKE", parca + "%"));
                        icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BANKA], "LIKE", "%" + parca + "%"));
                    }
                    if (icKosul != "")
                        kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
                }

                if (bilgi.vergiNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VERGINO], "=", bilgi.vergiNo));

                if (bilgi.vergiDairesi != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VERGIDAIRESI], "LIKE", "%" + bilgi.vergiDairesi + "%"));

                if (bilgi.banka != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BANKA], "LIKE", "%" + bilgi.banka + "%"));

                if (bilgi.hesapNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPNO], "LIKE", "%" + bilgi.hesapNo + "%"));

                if (bilgi.firmaYetkilisi != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAYETKILISI], "LIKE", "%" + bilgi.firmaYetkilisi + "%"));

                if (bilgi.karaListe == 1)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KARALISTE], "=", 1));

                if (bilgi.aktifDurum == 1)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GORULMESIN], "=", 0));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YASAKLI], "=", 0));
                }
                else if (bilgi.aktifDurum == 2)
                {
                    kosul = kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GORULMESIN], "<>", 0) + "OR" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YASAKLI], "<>", 0) + ")");
                }
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Firma bilgisinin olup olmadığı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, string döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Kayıtlı olup olmadığına bakılacak firma bilgisi için kıstas parametresi</param>
        /// <returns>
        /// Firma bilgisinin olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// string nesnesini döndürür
        /// </returns>
        public string KayitVarMi(FirmaBilgisi bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.VERGINO] + " FROM " + TABLOAD;

            if (bilgi.vergiNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VERGINO], "=", bilgi.vergiNo));

            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Firmanın devam eden işleri bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, string döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="vNo">Listeleme için kullanılacak firma vergi numarası bilgisi kıstas parametresi</param>
        /// <param name="sadeceDevamEdenler">Eğer <c>true</c> verilirse sadece firmanın devam eden işleri listelenir</param>
        /// <returns>
        /// Firmanın devam eden işleri bilgisinin olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// string nesnesini döndürür
        /// </returns>
        public IDataReader FirmaninIsleri(int yil, string vNo, bool sadeceDevamEdenler)
        {
            string sql = "SELECT HRCTEKLIFGRUP.YIL, HRCTEKLIFGRUP.MUHASEBEKOD, HRCTEKLIFGRUP.BIRIMKOD, HRCTEKLIFGRUP.SURECNO, HRCTEKLIFGRUP.FIRMANO, HRCTEKLIFGRUP.BIRIMFIYAT, HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD, HRCSUREC.ISINADI";
            sql += " FROM HRCTEKLIFGRUP INNER JOIN";
            sql += " HRCFIRMABILGISI ON HRCTEKLIFGRUP.FIRMANO = HRCFIRMABILGISI.KOD INNER JOIN";
            sql += " HRCSUREC ON HRCTEKLIFGRUP.YIL = HRCSUREC.YIL AND HRCTEKLIFGRUP.SURECNO = HRCSUREC.SURECNO";
            sql += " WHERE";
            sql += " HRCTEKLIFGRUP.YIL = " + yil;
            sql += " AND HRCFIRMABILGISI.VERGINO = '" + vNo + "'";
            if (sadeceDevamEdenler)
                sql += " AND HRCSUREC.SURECDURUM < 5";
            else
                sql += " AND HRCSUREC.SURECDURUM < 9";
            sql += " UNION ";
            sql += " SELECT HRCKAZANANFIRMA.YIL, HRCKAZANANFIRMA.MUHASEBEKOD, HRCKAZANANFIRMA.BIRIMKOD, HRCKAZANANFIRMA.SURECNO, ";
            sql += " HRCKAZANANFIRMA.FIRMAKOD1 FIRMAKOD, HRCTEKLIFEDILENFIYAT.TEKLIFTUTAR, HRCFIRMABILGISI.VERGINO, HRCFIRMABILGISI.AD, HRCSUREC.ISINADI";
            sql += " FROM HRCKAZANANFIRMA INNER JOIN";
            sql += " HRCTEKLIFEDILENFIYAT ON HRCKAZANANFIRMA.YIL = HRCTEKLIFEDILENFIYAT.YIL AND ";
            sql += " HRCKAZANANFIRMA.SURECNO = HRCTEKLIFEDILENFIYAT.SURECNO AND ";
            sql += " HRCKAZANANFIRMA.FIRMAKOD1 = HRCTEKLIFEDILENFIYAT.FIRMAKODU INNER JOIN";
            sql += " HRCSUREC AS HRCSUREC ON HRCKAZANANFIRMA.YIL = HRCSUREC.YIL AND ";
            sql += " HRCKAZANANFIRMA.SURECNO = HRCSUREC.SURECNO INNER JOIN";
            sql += " HRCFIRMABILGISI AS HRCFIRMABILGISI ON HRCKAZANANFIRMA.FIRMAKOD1 = HRCFIRMABILGISI.KOD";
            sql += " WHERE HRCKAZANANFIRMA.YIL = " + yil;
            sql += " AND HRCFIRMABILGISI.VERGINO = '" + vNo + "'";
            if (sadeceDevamEdenler)
                sql += " AND HRCSUREC.SURECDURUM < 5";
            else
                sql += " AND HRCSUREC.SURECDURUM < 9";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Firmanın sisteme giriş bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek firma bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc FirmaGirisKaydet(FirmaBilgisi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "UPDATE " + TABLOADGIRIS + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.PAROLA], bilgi.parola);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.VERGINO], "=", bilgi.vergiNo));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOADGIRIS + " (" + VeriErisim.TumAlanlar("", ALANLARGIRIS) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.vergiNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.parola) + ",";
                sql += VeriErisim.DegerYap(bilgi.sonGirisTarih) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Sisteme giriş yapan firmanın parola bilgisinin olup olmadığı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, double döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak firma bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sisteme giriş yapan firmanın parola bilgisinin olup olmadığı bilgisinin olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// string nesnesini döndürür
        /// </returns>
        public double FirmaGirisKayitlimi(FirmaBilgisi bilgi)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADGIRIS;
            sql += " WHERE VERGINO= '" + bilgi.vergiNo + "'";

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Firmanın sisteme giriş bilgisini veri tabanından silmek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek firma bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc FirmaGirisSil(FirmaBilgisi bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADGIRIS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.VERGINO], "=", bilgi.vergiNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Firmanın sisteme giriş bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak firma bilgisi kıstas parametresi</param>
        /// <returns>
        /// Firmanın sisteme giriş bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader FirmaGirisListele(FirmaBilgisi bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARGIRIS) + " FROM " + TABLOADGIRIS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.VERGINO], "=", bilgi.vergiNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Firmanın sisteme giriş tarih bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullaniciKodu">Güncellenecek bilgiye sahip firmanın vergi numarası</param>
        public void FirmaGirisTarihKaydet(string kullaniciKodu)
        {
            string sql = "UPDATE " + TABLOADGIRIS + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.SONGIRISTARIHI], new TNSDateTime(DateTime.Now.ToString()));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.VERGINO], "=", kullaniciKodu));

            KomutCalistir(sql, 1);
        }
    }
}
using System.Data;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeMakineOdeme : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeMakineOdeme(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// hibe makine Ödeme bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TRMHIBEMAKINAODEME";

        /// <summary>
        /// hibe makine Ödeme bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// ETAPNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ETAPNO = 1,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TURKOD = 2,
            TIPI = 3,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 4,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KONU = 5,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GERSURE = 6,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GERUSUL = 7,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERTIP = 8,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEHESAP = 9,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 10,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKBEDEL = 11,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEK = 12,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GERGOREVLI = 13,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 14,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFEDENAD = 15,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFEDENUNVAN = 16,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HARYETKILIAD = 17,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HARYETKILIUNVAN = 18,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRIYIL = 19,
            /// <summary>
            /// alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 20,
            MUHASEBE = 21,
            BIRIM = 22,
            VERGINO = 23,
            ODEMEYAPILDI = 24,
            ODEMEYAPILMATARIHI = 25,
            KOORDINATORAD = 26,
            KOORDINATORUNVAN = 27,
            HIBE = 28,
            KOMISYON = 29,
            HAZIRLAYAN1AD = 30,
            HAZIRLAYAN1UNVAN = 31,
            HAZIRLAYAN2AD = 32,
            HAZIRLAYAN2UNVAN = 33
        }

        /// <summary>
        /// hibe makine Ödeme tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "ETAPNO", "TURKOD", "TIPI", "GEREKCE", "KONU", "GERSURE", "GERUSUL", "TERTIP", "ODEMEHESAP", "MIKTAR", "YAKLASIKBEDEL", "ODENEK", "GERGOREVLI", "ACIKLAMA", "TEKLIFEDENAD", "TEKLIFEDENUNVAN", "HARYETKILIAD", "HARYETKILIUNVAN", "ODEMEEMRIYIL", "ODEMEEMRINO", "MUHASEBE", "BIRIM", "VERGINO", "ODEMEYAPILDI", "ODEMEYAPILMATARIHI", "KOORDINATORAD", "KOORDINATORUNVAN", "HIBE", "KOMISYON", "HAZIRLAYAN1AD", "HAZIRLAYAN1UNVAN", "HAZIRLAYAN2AD", "HAZIRLAYAN2UNVAN" };

        /// <summary>
        /// hibe makine Ödeme bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(HibeMakineSulamaOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURKOD], bilgi.turKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], bilgi.gerekce) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONU], bilgi.konu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERSURE], bilgi.gerceklestirmeSure) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERUSUL], bilgi.gerceklestirmeUsulu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERTIP], bilgi.tertip) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEHESAP], bilgi.odemeHesap) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], bilgi.miktar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKBEDEL], bilgi.yaklasikBedel) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENEK], bilgi.odenek) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERGOREVLI], bilgi.gerceklestirmeGorevlileri) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFEDENAD], bilgi.teklifEdenAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFEDENUNVAN], bilgi.teklifEdenUnvan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARYETKILIAD], bilgi.harcamaYetkilisiAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HARYETKILIUNVAN], bilgi.harcamaYetkilisiUnvan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], bilgi.muhasebe) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], bilgi.birim) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGINO], bilgi.vergiNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOORDINATORAD], bilgi.koordinatorAd) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOORDINATORUNVAN], bilgi.koordinatorUnvan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HIBE], bilgi.hibe) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOMISYON], bilgi.komisyon) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAZIRLAYAN1AD], bilgi.hazirlayan1Ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAZIRLAYAN1UNVAN], bilgi.hazirlayan1Unvan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAZIRLAYAN2AD], bilgi.hazirlayan2Ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAZIRLAYAN2UNVAN], bilgi.hazirlayan2Unvan);
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRIYIL], bilgi.odemeEmriYil) + ", ";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.kod) + ",";
                sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.turKod) + ",";
                sql += VeriErisim.DegerYap(bilgi.tipi) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerekce) + ",";
                sql += VeriErisim.DegerYap(bilgi.konu) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerceklestirmeSure) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerceklestirmeUsulu) + ",";
                sql += VeriErisim.DegerYap(bilgi.tertip) + ",";
                sql += VeriErisim.DegerYap(bilgi.odemeHesap) + ",";
                sql += VeriErisim.DegerYap(bilgi.miktar) + ",";
                sql += VeriErisim.DegerYap(bilgi.yaklasikBedel) + ",";
                sql += VeriErisim.DegerYap(bilgi.odenek) + ",";
                sql += VeriErisim.DegerYap(bilgi.gerceklestirmeGorevlileri) + ",";
                sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifEdenAd) + ",";
                sql += VeriErisim.DegerYap(bilgi.teklifEdenUnvan) + ",";
                sql += VeriErisim.DegerYap(bilgi.harcamaYetkilisiAd) + ",";
                sql += VeriErisim.DegerYap(bilgi.harcamaYetkilisiUnvan) + ",";
                sql += VeriErisim.DegerYap(bilgi.odemeEmriYil) + ",";
                sql += VeriErisim.DegerYap(bilgi.odemeEmriNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.vergiNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.odemeYapildi) + ",";
                sql += VeriErisim.DegerYap(bilgi.odemeYapilmaTarihi) + ",";
                sql += VeriErisim.DegerYap(bilgi.koordinatorAd) + ",";
                sql += VeriErisim.DegerYap(bilgi.koordinatorUnvan) + ",";
                sql += VeriErisim.DegerYap(bilgi.hibe) + ",";
                sql += VeriErisim.DegerYap(bilgi.komisyon) + ",";
                sql += VeriErisim.DegerYap(bilgi.hazirlayan1Ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.hazirlayan1Unvan) + ",";
                sql += VeriErisim.DegerYap(bilgi.hazirlayan2Ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.hazirlayan2Unvan) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// hibe makine Ödeme Emri No bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek hibe makine bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetOdemeEmriNo(HibeMakineSulamaOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRIYIL], bilgi.odemeEmriYil) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc KaydetOdendi(int kod, int tur)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEYAPILDI], tur) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEYAPILMATARIHI], new TNSDateTime(DateTime.Now));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ODEMEYAPILDI], "=", 0));

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        /// <summary>
        /// hibe makine Ödeme bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(HibeMakineSulamaOdeme bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SulamaveGrubunuTemizle(HibeMakineSulamaOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE TRMHIBESULAMA SET DURUM=16";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBESULAMA", "DURUM", "=", 18));
            sql += " AND GRUPKOD IN (SELECT KOD FROM TRMHIBEMAKINAGRUP WHERE ODEMEKOD=" + bilgi.kod + " AND DURUM=18)";
            sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "UPDATE TRMHIBEMAKINAGRUP SET ";
                sql += "DURUM=16, ODEMEKOD=0, KONTROLTARIHI=NULL  ";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TRMHIBEMAKINAGRUP", "ODEMEKOD", "=", bilgi.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBEMAKINAGRUP", "DURUM", "=", 18));

                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        public Sonuc SulamaDurumDegistir(HibeMakineSulamaOdeme bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";


            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        /// <summary>
        /// hibe makine Ödeme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak para birimi bilgisi kıstas parametresi</param>
        /// <returns>
        /// hibe makine Ödeme bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(HibeMakineSulamaOdeme bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", bilgi.etapNo));
            if (bilgi.tipi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIPI], "=", bilgi.tipi));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ETAPNO] + " DESC, " + ALANLAR[(int)ENUMAlanlar.KOD]+ " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tabloda kullanılan en büyük kod numarasını döndürür
        /// </summary>
        /// <returns>Verilen kriter bilgisine uygun kayıt bulunursa true, yoksa false döndürür</returns>
        public int EnBuyukKod()
        {
            string sql = "";

            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }

    }
}
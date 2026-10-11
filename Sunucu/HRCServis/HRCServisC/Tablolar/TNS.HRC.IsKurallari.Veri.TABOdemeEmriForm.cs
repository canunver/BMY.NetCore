using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Ödeme Emri Belge bilgisini tutan HRCODEMEEMRIFORM ve HRCODEMEEMRIDETAY veri tabanı tabloları ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOdemeEmriForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOdemeEmriForm(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Ödeme Emri Belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCODEMEEMRIFORM";

        /// <summary>
        /// Ödeme Emri Belge bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// PRODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRODEMEEMRINO = 4,
            /// <summary>
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRFATURANO = 5,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 6,
            /// <summary>
            /// ODEMEEMRITARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRITARIH = 7,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 8,
            /// <summary>
            /// BELGETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 9,
            PRFATURANOEK = 10,
        }

        /// <summary>
        /// Ödeme Emri Belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRODEMEEMRINO", "PRFATURANO", "ODEMEEMRINO", "ODEMEEMRITARIH", "ACIKLAMA", "BELGETUR", "PRFATURANOEK" };

        /// <summary>
        /// Ödeme Emri Belge Detay bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "HRCODEMEEMRIDETAY";

        /// <summary>
        /// Ödeme Emri Belge Detay bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDetay
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
            /// PRODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PRODEMEEMRINO = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// HESAPPLANKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 6,
            /// <summary>
            /// TERTIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TERTIP = 7,
            /// <summary>
            /// YANSITMAHESAPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YANSITMAHESAPKOD = 8,
            /// <summary>
            /// BORC alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BORC = 9,
            /// <summary>
            /// ALACAK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ALACAK = 10
        }

        /// <summary>
        /// Ödeme Emri Belge Detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "PRODEMEEMRINO", "SIRANO", "HESAPPLANKOD", "TERTIP", "YANSITMAHESAPKOD", "BORC", "ALACAK" };

        /// <summary>
        /// Sürece ait ödeme emri belge belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADSURECMUHMIFBAG = "HRCSURECMUHMIFBAG";

        /// <summary>
        /// Sürece ait ödeme emri belge belge bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSurecMuhMifBag
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
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 4
        }

        /// <summary>
        /// Sürece ait ödeme emri belge belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARSURECMUHMIFBAG = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "ODEMEEMRINO" };

        /// <summary>
        /// Ödeme emri belge bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ödeme emri belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(OdemeEmriMIFForm bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.prOdemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.prFaturaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.belgeTur) + ",";
            sql += VeriErisim.DegerYap(bilgi.prFaturaNoEk) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PRFATURANO], bilgi.prFaturaNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PRFATURANOEK], bilgi.prFaturaNoEk) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRITARIH], bilgi.odemeEmriTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], bilgi.belgeTur);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRODEMEEMRINO], "=", bilgi.prOdemeEmriNo));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Ödeme emri belge detay bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek ödeme emri belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(OdemeEmriMIFForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.detay.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.prOdemeEmriNo) + ",";
                sql += VeriErisim.DegerYap(i + 1) + ",";
                sql += VeriErisim.DegerYap(((OdemeEmriMIFDetay)bilgi.detay.objeler[i]).hesapPlanKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(((OdemeEmriMIFDetay)bilgi.detay.objeler[i]).tertipVeMahsupBelgeNo.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(((OdemeEmriMIFDetay)bilgi.detay.objeler[i]).yansitma.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(((OdemeEmriMIFDetay)bilgi.detay.objeler[i]).borc) + ",";
                sql += VeriErisim.DegerYap(((OdemeEmriMIFDetay)bilgi.detay.objeler[i]).alacak) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        /// <summary>
        /// Ödeme emri belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ödeme emri belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Ödeme emri belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(OdemeEmriMIFForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.prOdemeEmriNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRODEMEEMRINO], "=", bilgi.prOdemeEmriNo));
            if (bilgi.prFaturaNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRFATURANO], "=", bilgi.prFaturaNo));
            if (bilgi.odemeEmriNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "=", bilgi.odemeEmriNo));
            if (bilgi.belgeTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", bilgi.belgeTur));

            if (bilgi.prFaturaNoEk != "")
            {
                string icKosul = "";
                string[] fler = bilgi.prFaturaNoEk.Replace("|", "").Split(';');
                foreach (string fn in fler)
                {
                    if (fn.Trim() == "") continue;

                    string fn2 = "|" + fn.Trim() + "|";
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRFATURANOEK], "LIKE", "%" + fn2 + "%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRFATURANO], "=", OrtakFonksiyonlar.ConvertToInt(fn, 0)));
                }

                if (icKosul != "")
                {
                    icKosul = "(" + icKosul + ")";
                    kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
                }
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ODEMEEMRITARIH] + " DESC, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ödeme emri belge süreç bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ödeme emri belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Ödeme emri belge süreç bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOdemeEmriMuhMif(OdemeEmriMIFForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG) + " FROM " + TABLOADSURECMUHMIFBAG;

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.SURECNO], "=", bilgi.surecNo));
            if (bilgi.odemeEmriNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSURECMUHMIFBAG, ALANLARSURECMUHMIFBAG[(int)ENUMAlanlarSurecMuhMifBag.ODEMEEMRINO], "=", bilgi.odemeEmriNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ödeme emri belge detay bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak ödeme emri belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Ödeme emri belge detay bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleDetay(OdemeEmriMIFForm bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDETAY) + " FROM " + TABLOADDETAY;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.SURECNO], "=", bilgi.surecNo));
            if (bilgi.prOdemeEmriNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.PRODEMEEMRINO], "=", bilgi.prOdemeEmriNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD] + ", " + ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD] + "," + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + "," + ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        ///// <summary>
        ///// Ödeme emri belge miktar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        ///// üretip, çalıştırıp, datareader döndüren yordam.
        ///// </summary>
        ///// <param name="bilgi">Listeleme için kullanılacak ödeme emri belge bilgisi kıstas parametresi</param>
        ///// <param name="hesapKod">Listeleme için kullanılacak hesap kod kıstas parametresi</param>
        ///// <returns>
        ///// Ödeme emri belge miktar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        ///// DataReader nesnesini döndürür
        ///// </returns>
        //public IDataReader ListeleMiktar(OdemeEmriMIFForm bilgi, string hesapKod)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    sql = "SELECT HESAPPLANKOD,SUM(BORC),SUM(ALACAK),TERTIP FROM " + TABLOADDETAY;
        //    if (bilgi.yil > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", bilgi.yil));
        //    if (bilgi.muhasebe > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", bilgi.muhasebe));
        //    if (bilgi.birim != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.BIRIMKOD], "=", bilgi.birim));
        //    if (bilgi.surecNo != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.SURECNO], "=", bilgi.surecNo));
        //    if (bilgi.prOdemeEmriNo > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.PRODEMEEMRINO], "=", bilgi.prOdemeEmriNo));
        //    if (hesapKod != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HESAPPLANKOD], "LIKE", hesapKod + "%"));

        //    if (kosul != "")
        //        sql += " WHERE " + kosul;

        //    sql += " GROUP BY HESAPPLANKOD , TERTIP";
        //    sql += " ORDER BY HESAPPLANKOD";

        //    return veriErisim.DataReaderDondur(sql);
        //}

        ///// <summary>
        ///// Ödeme emri belge miktar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        ///// üretip, çalıştırıp, datareader döndüren yordam.
        ///// </summary>
        ///// <param name="bilgi">Listeleme için kullanılacak ödeme emri belge bilgisi kıstas parametresi</param>
        ///// <param name="hesapKod">Listeleme için kullanılacak hesap kod kıstas parametresi</param>
        ///// <returns>
        ///// Ödeme emri belge miktar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        ///// DataReader nesnesini döndürür
        ///// </returns>
        //public IDataReader ListeleMiktar(OdemeEmriMIFForm bilgi, string hesapKod, string tertip)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    sql = "SELECT " +
        //        TABLOADDETAY + ".YIL," +
        //        TABLOADDETAY + ".SURECNO," +
        //        TABLOADDETAY + ".HESAPPLANKOD," +
        //        "SUM(" + TABLOADDETAY + ".BORC)," +
        //        "SUM(" + TABLOADDETAY + ".ALACAK)," +
        //        TABLOADDETAY + ".TERTIP " +
        //        " FROM " + TABLOADDETAY +
        //        " INNER JOIN  " + TABLOAD + " ON " +
        //        TABLOADDETAY + ".YIL= " + TABLOAD + ".YIL AND " + TABLOADDETAY + ".SURECNO = " + TABLOAD + ".SURECNO";

        //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "<>", ""));

        //    if (bilgi.yil > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", bilgi.yil));
        //    if (bilgi.muhasebe > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", bilgi.muhasebe));
        //    if (bilgi.birim != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.BIRIMKOD], "=", bilgi.birim));
        //    if (bilgi.surecNo != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.SURECNO], "=", bilgi.surecNo));
        //    if (bilgi.prOdemeEmriNo > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.PRODEMEEMRINO], "=", bilgi.prOdemeEmriNo));
        //    if (hesapKod != "")
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.HESAPPLANKOD], "LIKE", hesapKod + "%"));
        //    if (tertip != "")
        //    {
        //        string[] t = tertip.Replace(".", "").Split('-');
        //        if (t.Length > 0)
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", "SUBSTRING(" + VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TERTIP] + ",0,9)", "LIKE", t[0] + "%"));
        //        if (t.Length > 1)
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", "SUBSTRING(" + VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TERTIP] + ",10,6)", "LIKE", t[1] + "%"));
        //        if (t.Length > 2)
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", "SUBSTRING(" + VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TERTIP] + ",17,1)", "LIKE", t[2] + "%"));
        //        if (t.Length > 3)
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", "SUBSTRING(" + VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TERTIP] + ",19,6)", "LIKE", t[3] + "%"));
        //        if (t.Length > 4)
        //            kosul = VeriErisim.KosulEkle(kosul, "AND", "SUBSTRING(" + VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TERTIP] + ",26,8)", "LIKE", t[4] + "%"));
        //    }


        //    if (kosul != "")
        //        sql += " WHERE " + kosul;

        //    sql += " GROUP BY " + TABLOADDETAY + ".YIL, " + TABLOADDETAY + ".SURECNO, " + TABLOADDETAY + ".HESAPPLANKOD , " + TABLOADDETAY + ".TERTIP";
        //    sql += " ORDER BY " + TABLOADDETAY + ".YIL, " + TABLOADDETAY + ".SURECNO, " + TABLOADDETAY + ".HESAPPLANKOD , " + TABLOADDETAY + ".TERTIP";

        //    return veriErisim.DataReaderDondur(sql);
        //}

        /// <summary>
        /// Ödeme emri belge aylık dağılım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kurKod">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="islemKodListesi">Listeleme için kullanılacak işlem kod kıstas parametresi</param>
        /// <returns>
        /// Ödeme emri belge aylık dağılım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// double döndürür
        /// </returns>
        public IDataReader ListeleAylikDagilim(int yil, int muhasebeKod, string birimKod, string kurKod, int[] islemKodListesi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT  MONTH(HRCODEMEEMRIFORM.ODEMEEMRITARIH), HRCODEMEEMRIFORM.MUHASEBEKOD, HRCODEMEEMRIFORM.BIRIMKOD, MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, MUHMIF.EKOKOD, SUM(MUHMIF.BORC) ";
            sql += " FROM         HRCODEMEEMRIFORM LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCODEMEEMRIFORM.YIL = HRCSUREC.YIL AND HRCODEMEEMRIFORM.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN " +
                      " MUHMIF AS MUHMIF ON HRCODEMEEMRIFORM.YIL = MUHMIF.YIL AND HRCODEMEEMRIFORM.ODEMEEMRINO = MUHMIF.BELGENO ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "KURKOD", "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "FONKOD", "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "FINKOD", "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "EKOKOD", "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCSUREC", "SURECDURUM", "<>", 9));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "HESAPKOD", "LIKE", "830%"));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCODEMEEMRIFORM", ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", yil));
            if (muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCODEMEEMRIFORM", ALANLAR[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
            if (birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCODEMEEMRIFORM", ALANLAR[(int)ENUMAlanlarDetay.BIRIMKOD], "=", birimKod));
            if (kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "KURKOD", "=", kurKod));
            if (islemKodListesi.Length > 0)
            {
                string inDeger = "(";
                for (int i = 0; i < islemKodListesi.Length; i++)
                {
                    inDeger += islemKodListesi[i].ToString();
                    if (i != islemKodListesi.Length - 1)
                        inDeger += ",";
                }
                inDeger += ")";
                kosul = VeriErisim.KosulEkle(kosul, "AND", "HRCSUREC.ISLEMKOD IN " + inDeger); //VeriErisim.KosulOl("HRCSUREC", "ISLEMKOD", "IN", inDeger));
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY MONTH(HRCODEMEEMRIFORM.ODEMEEMRITARIH),HRCODEMEEMRIFORM.MUHASEBEKOD, HRCODEMEEMRIFORM.BIRIMKOD, MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, MUHMIF.EKOKOD ";
            sql += " ORDER BY HRCODEMEEMRIFORM.MUHASEBEKOD, HRCODEMEEMRIFORM.BIRIMKOD,MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, MUHMIF.EKOKOD, MONTH(HRCODEMEEMRIFORM.ODEMEEMRITARIH) ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Ödeme emri belge toplam aylık dağılım bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        /// <param name="kurKod">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        /// <param name="islemKodListesi">Listeleme için kullanılacak işlem kod kıstas parametresi</param>
        /// <returns>
        /// Ödeme emri belge toplam aylık dağılım bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// double döndürür
        /// </returns>
        public double ToplamAylikDagilim(int yil, int muhasebeKod, string birimKod, string kurKod, int[] islemKodListesi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT   SUM(MUHMIF.BORC) ";
            sql += " FROM         HRCODEMEEMRIFORM LEFT OUTER JOIN " +
                      " HRCSUREC ON HRCODEMEEMRIFORM.YIL = HRCSUREC.YIL AND HRCODEMEEMRIFORM.SURECNO = HRCSUREC.SURECNO LEFT OUTER JOIN " +
                      " MUHMIF AS MUHMIF ON HRCODEMEEMRIFORM.YIL = MUHMIF.YIL AND HRCODEMEEMRIFORM.ODEMEEMRINO = MUHMIF.BELGENO ";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "KURKOD", "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "FONKOD", "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "FINKOD", "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "EKOKOD", "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCSUREC", "SURECDURUM", "<>", 9));

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "HESAPKOD", "LIKE", "830%"));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCODEMEEMRIFORM", ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", yil));
            if (muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCODEMEEMRIFORM", ALANLAR[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
            if (birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("HRCODEMEEMRIFORM", ALANLAR[(int)ENUMAlanlarDetay.BIRIMKOD], "=", birimKod));
            if (kurKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("MUHMIF", "KURKOD", "=", kurKod));
            if (islemKodListesi.Length > 0)
            {
                string inDeger = "(";
                for (int i = 0; i < islemKodListesi.Length; i++)
                {
                    inDeger += islemKodListesi[i].ToString();
                    if (i != islemKodListesi.Length - 1)
                        inDeger += ",";
                }
                inDeger += ")";
                kosul = VeriErisim.KosulEkle(kosul, "AND", "HRCSUREC.ISLEMKOD IN " + inDeger); //VeriErisim.KosulOl("HRCSUREC", "ISLEMKOD", "IN", inDeger));
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            //sql += " GROUP BY MONTH(HRCODEMEEMRIFORM.ODEMEEMRITARIH),HRCODEMEEMRIFORM.MUHASEBEKOD, HRCODEMEEMRIFORM.BIRIMKOD, MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, MUHMIF.EKOKOD ";
            //sql += " ORDER BY HRCODEMEEMRIFORM.MUHASEBEKOD, HRCODEMEEMRIFORM.BIRIMKOD,MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, MUHMIF.EKOKOD, MONTH(HRCODEMEEMRIFORM.ODEMEEMRITARIH) ";

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Ödeme emri belge programatik belge numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası kıstas parametresi</param>
        /// <param name="odemeEmriNo">Listeleme için kullanılacak ödeme emri belge numarası kıstas parametresi</param>
        /// <returns>
        /// Ödeme emri belge programatik belge numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public int PrOdemeEmriNoBul(int yil, string surecNo, string odemeEmriNo)
        {
            string sql = "";

            sql = "SELECT PRODEMEEMRINO FROM HRCODEMEEMRIFORM";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "=", odemeEmriNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Ödeme emri belge bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek ödeme emri belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(OdemeEmriMIFForm bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.PRODEMEEMRINO], "=", (bilgi.prOdemeEmriNo)));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Ödeme emri belge detay bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek ödeme emri belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(OdemeEmriMIFForm bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", (bilgi.yil)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", (bilgi.muhasebe)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", (bilgi.birim)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", (bilgi.surecNo)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PRODEMEEMRINO], "=", (bilgi.prOdemeEmriNo)));

            return KomutCalistir(sql, 0);
        }

        public Sonuc OdemeEmriNoTemizle(int yil, string surecNo, string odemeEmriNo)
        {
            string sql = "";

            sql = "UPDATE HRCFATURAFORM SET ";
            sql += VeriErisim.AtamaOl("", "ODEMEEMRINO", "") + ",";
            sql += VeriErisim.AtamaOl("", "ODEMEEMRIYIL", 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ODEMEEMRINO", "=", odemeEmriNo));

            return KomutCalistir(sql, 0);
        }


        ///// <summary>
        ///// Ödeme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        ///// üretip, çalıştırıp, datareader döndüren yordam.
        ///// </summary>
        ///// <param name="yil">Listeleme için kullanılacak yıl kıstas parametresi</param>
        ///// <param name="ay">Listeleme için kullanılacak ay kıstas parametresi</param>
        ///// <param name="muhasebe">Listeleme için kullanılacak muhasebe kod kıstas parametresi</param>
        ///// <param name="birim">Listeleme için kullanılacak birim kod kıstas parametresi</param>
        ///// <param name="kur">Listeleme için kullanılacak kurumsal kod kıstas parametresi</param>
        ///// <param name="fon">Listeleme için kullanılacak fonksiyonel kod kıstas parametresi</param>
        ///// <param name="fin">Listeleme için kullanılacak finansman kod kıstas parametresi</param>
        ///// <param name="eko">Listeleme için kullanılacak ekonomik kod kıstas parametresi</param>
        ///// <param name="peb">Listeleme için kullanılacak alt faaliyet kod kıstas parametresi</param>
        ///// <returns>
        ///// Ödeme bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        ///// DataReader nesnesini döndürür
        ///// </returns>
        //public IDataReader ListeleButceHazirlikOdemeHarcama(int yil, int ay, int muhasebe, string birim, string kur, string fon, string fin, string eko, string peb)
        //{
        //    string sql = string.Empty;

        //    sql = "SELECT     YEAR(HRCODEMEEMRIFORM.ODEMEEMRITARIH) AS ISLEMYIL, MONTH(HRCODEMEEMRIFORM.ODEMEEMRITARIH) AS ISLEMAY, HRCSUREC.YIL AS SURECYIL,  " +
        //              " HRCODEMEEMRIFORM.SURECNO, HRCODEMEEMRIDETAY.MUHASEBEKOD, HRCODEMEEMRIDETAY.BIRIMKOD, SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 0, 9) " +
        //               " AS KURKOD, SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 10, 6) AS FONKOD, SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 17, 1) AS FINKOD,  " +
        //              " SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 19, 6) AS EKOKOD, SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 26, 6) AS PEBKOD,  " +
        //              " SUM(HRCODEMEEMRIDETAY.BORC) AS TOPLAM, HRCSUREC.YILLARASARIISLEMMI " +
        //            " FROM         HRCODEMEEMRIFORM LEFT OUTER JOIN " +
        //              " HRCODEMEEMRIDETAY ON HRCODEMEEMRIFORM.PRODEMEEMRINO = HRCODEMEEMRIDETAY.PRODEMEEMRINO AND  " +
        //              " HRCODEMEEMRIFORM.YIL = HRCODEMEEMRIDETAY.YIL AND HRCODEMEEMRIFORM.SURECNO = HRCODEMEEMRIDETAY.SURECNO LEFT OUTER JOIN " +
        //              " HRCSUREC ON HRCODEMEEMRIFORM.YIL = HRCSUREC.YIL AND HRCODEMEEMRIFORM.SURECNO = HRCSUREC.SURECNO " +
        //            " WHERE     (HRCSUREC.SURECDURUM <> 9) AND (HRCODEMEEMRIDETAY.YANSITMAHESAPKOD LIKE '830%') AND (HRCODEMEEMRIFORM.ODEMEEMRINO <> '') ";

        //    if (yil > 0)
        //        sql += " AND (YEAR(HRCODEMEEMRIFORM.ODEMEEMRITARIH) = " + yil + ") ";
        //    if (ay > 0)
        //        sql += " AND (MONTH(HRCODEMEEMRIFORM.ODEMEEMRITARIH) = " + ay + ") ";
        //    if (muhasebe > 0)
        //        sql += " AND (HRCODEMEEMRIDETAY.MUHASEBEKOD = " + muhasebe + ") ";
        //    if (!string.IsNullOrEmpty(birim))
        //        sql += " AND (HRCODEMEEMRIDETAY.BIRIMKOD = '" + birim + "') ";
        //    if (!string.IsNullOrEmpty(kur))
        //        sql += " AND (SUBSTRING(HRCODEMEEMRIDETAY.TERTIP,0, 9) = '" + kur + "') ";
        //    if (!string.IsNullOrEmpty(fon))
        //        sql += " AND (SUBSTRING(HRCODEMEEMRIDETAY.TERTIP,10, 6) = '" + fon + "') ";
        //    if (!string.IsNullOrEmpty(fin))
        //        sql += " AND (SUBSTRING(HRCODEMEEMRIDETAY.TERTIP,17, 1) = '" + fin + "') ";
        //    if (!string.IsNullOrEmpty(eko))
        //        sql += " AND (SUBSTRING(HRCODEMEEMRIDETAY.TERTIP,19, 6) = '" + eko + "') ";
        //    if (!string.IsNullOrEmpty(peb))
        //        sql += " AND (SUBSTRING(HRCODEMEEMRIDETAY.TERTIP,26, 6) = '" + peb + "') ";

        //    sql += " GROUP BY YEAR(HRCODEMEEMRIFORM.ODEMEEMRITARIH), MONTH(HRCODEMEEMRIFORM.ODEMEEMRITARIH), HRCODEMEEMRIFORM.SURECNO,  " +
        //              " HRCODEMEEMRIDETAY.MUHASEBEKOD, HRCODEMEEMRIDETAY.BIRIMKOD, SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 0, 9),  " +
        //              " SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 10, 6), SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 17, 1), SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 19, 6),  " +
        //              " SUBSTRING(HRCODEMEEMRIDETAY.TERTIP, 26, 6), HRCSUREC.YILLARASARIISLEMMI, HRCSUREC.YIL ";

        //    sql += " UNION ";

        //    sql += "SELECT     YEAR(MUHMIFFORM.ISLEMTARIH) AS ISLEMYIL, MONTH(MUHMIFFORM.ISLEMTARIH) AS ISLEMAY, MUHMIFFORM.SURECYIL, " +
        //          " MUHMIFFORM.SURECNO, MUHMIFFORM.MUHASEBEKOD, MUHMIFFORM.BIRIMKOD, MUHMIF.KURKOD, MUHMIF.FONKOD," +
        //          " MUHMIF.FINKOD, MUHMIF.EKOKOD, MUHMIF.PEBKOD, SUM(MUHMIF.BORC) AS TOPLAM, 0 AS YILLARASARIISLEMMI " +
        //          " FROM MUHMIFFORM LEFT OUTER JOIN " +
        //          " MUHMIF ON MUHMIFFORM.YIL = MUHMIF.YIL AND MUHMIFFORM.BELGENO = MUHMIF.BELGENO " +
        //          " WHERE     (MUHMIFFORM.BELGETUR = 1) AND (MUHMIFFORM.BELGENO NOT IN " +
        //                        " (SELECT HRCODEMEEMRIFORM.ODEMEEMRINO " +
        //                          " FROM HRCODEMEEMRIFORM LEFT OUTER JOIN " +
        //                            " HRCSUREC ON HRCODEMEEMRIFORM.YIL = HRCSUREC.YIL AND " +
        //                            " HRCODEMEEMRIFORM.SURECNO = HRCSUREC.SURECNO " +
        //                          " WHERE (HRCODEMEEMRIFORM.BELGETUR = 1) ";
        //    if (yil > 0)
        //        sql += " AND (HRCODEMEEMRIFORM.YIL = " + yil + ") ";

        //    sql += "  AND (HRCODEMEEMRIFORM.ODEMEEMRINO <> '') AND (HRCSUREC.SURECDURUM <> 9))) AND " +
        //            " (MUHMIFFORM.FORMDURUM <> 9) AND (MUHMIF.HESAPKOD LIKE '830%') AND " +
        //            " (MUHMIFFORM.SURECYIL > 0) AND (MUHMIFFORM.SURECNO <> '') ";

        //    if (yil > 0)
        //        sql += " AND (YEAR(MUHMIFFORM.ISLEMTARIH)  = " + yil + ") ";
        //    if (ay > 0)
        //        sql += " AND (MONTH(MUHMIFFORM.ISLEMTARIH)  = " + ay + ") ";
        //    if (muhasebe > 0)
        //        sql += " AND (MUHMIFFORM.MUHASEBEKOD = " + muhasebe + ") ";
        //    if (!string.IsNullOrEmpty(birim))
        //        sql += " AND (MUHMIFFORM.BIRIMKOD = '" + birim + "') ";
        //    if (!string.IsNullOrEmpty(kur))
        //        sql += " AND (MUHMIF.KURKOD = '" + kur + "') ";
        //    if (!string.IsNullOrEmpty(fon))
        //        sql += " AND (MUHMIF.FONKOD = '" + fon + "') ";
        //    if (!string.IsNullOrEmpty(fin))
        //        sql += " AND (MUHMIF.FINKOD = '" + fin + "') ";
        //    if (!string.IsNullOrEmpty(eko))
        //        sql += " AND (MUHMIF.EKOKOD = '" + eko + "') ";
        //    if (!string.IsNullOrEmpty(peb))
        //        sql += " AND (MUHMIF.PEBKOD = '" + peb + "') ";

        //    sql += " GROUP BY YEAR(MUHMIFFORM.ISLEMTARIH), MONTH(MUHMIFFORM.ISLEMTARIH), MUHMIFFORM.SURECYIL, MUHMIFFORM.SURECNO, MUHMIFFORM.MUHASEBEKOD, " +
        //             " MUHMIFFORM.BIRIMKOD, MUHMIF.HESAPKOD, MUHMIF.KURKOD, MUHMIF.FONKOD, MUHMIF.FINKOD, MUHMIF.EKOKOD, MUHMIF.PEBKOD";

        //    return veriErisim.DataReaderDondur(sql);
        //}
    }
}

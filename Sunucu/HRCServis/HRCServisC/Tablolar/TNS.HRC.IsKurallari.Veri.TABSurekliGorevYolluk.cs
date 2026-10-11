using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Sürekli görev yolluk belge bilgisini tutan PSLSUREKLIGOREVYOLLUKBELGE ve PSLSUREKLIGOREVYOLLUKDETAY veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSurekliGorevYolluk : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSurekliGorevYolluk(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Sürekli görev yolluk belge bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLSUREKLIGOREVYOLLUKBELGE";

        /// <summary>
        /// Sürekli görev yolluk belge detay bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "PSLSUREKLIGOREVYOLLUKDETAY";

        /// <summary>
        /// Süreç bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADSUREC = "HRCSUREC";

        /// <summary>
        /// Sürekli görev yolluk belge bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// BELGETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 4,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 5,
            /// <summary>
            /// KADROEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KADROEK = 6,
            /// <summary>
            /// GUNDELIKKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNDELIKKOD = 7,
            /// <summary>
            /// NERENDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NEREDEN = 8,
            /// <summary>
            /// NEREYE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NEREYE = 9,
            /// <summary>
            /// ATAMATARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ATAMATARIH = 10,
            /// <summary>
            /// TASITCESIT alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASITCESIT = 11,
            /// <summary>
            /// BILETUCRET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILETUCRET = 12,
            /// <summary>
            /// GUNSAYI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNSAYI = 13,
            /// <summary>
            /// GUNDELIKTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNDELIKTUTAR = 14,
            /// <summary>
            /// YERDEGISMESAFE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YERDEGISMESAFE = 15,
            /// <summary>
            /// YERDEGISTUTARI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YERDEGISTUTARI = 16,
            /// <summary>
            /// DOVIZKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOVIZKODU = 17,
            /// <summary>
            /// DOVIZKURU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DOVIZKURU = 18,
            /// <summary>
            /// DEGISKENUNSUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGISKENUNSUR = 19,
            /// <summary>
            /// YERDEGISTIRMEGIDERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YERDEGISTIRMEGIDERI = 20,
            /// <summary>
            /// UNVANKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVANKODU = 21,
            /// <summary>
            /// BILDIRIMTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILDIRIMTARIH = 22,
            /// <summary>
            /// ODEMEEMRINO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 23,
            /// <summary>
            /// EMEKLILIKMAASMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EMEKLILIKMAASIMI = 24,
            /// <summary>
            /// EMEKLILIKMAASI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EMEKLILIKMAASI = 25,
            /// <summary>
            /// GUNDELIKYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GUNDELIKYIL = 26,
        }

        /// <summary>
        /// Sürekli görev yolluk belge detay bilgisi tablo alanlarının sırasını tutan enumeration
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
            /// BELGETUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 4,
            /// <summary>
            /// TCKIMLIKNOBELGE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNOBELGE = 5,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 6,
            /// <summary>
            /// TCKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 7,
            /// <summary>
            /// ADSOYAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADSOYAD = 8,
            /// <summary>
            /// AKRABALIKKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AKRABALIKKODU = 9,
            /// <summary>
            /// SABITUNSUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SABITUNSUR = 10,
            /// <summary>
            /// TOPLAMTUTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTAR = 11,
            /// <summary>
            /// TOPLAMTUTARDOVIZ alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TOPLAMTUTARDOVIZ = 12,
        }

        /// <summary>
        /// Süreç bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarSurec
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,
            /// <summary>
            /// SURECDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECDURUM = 2,
        }

        /// <summary>
        /// Sürekli görev yolluk belge tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "BELGETUR", "TCKIMLIKNO", "KADROEK", "GUNDELIKKOD", "NEREDEN", "NEREYE", "ATAMATARIH", "TASITCESIT", "BILETUCRET", "GUNSAYI", "GUNDELIKTUTAR", "YERDEGISMESAFE", "YERDEGISTUTARI", "DOVIZKODU", "DOVIZKURU", "DEGISKENUNSUR", "YERDEGISTIRMEGIDERI", "UNVANKODU", "BILDIRIMTARIH", "ODEMEEMRINO", "EMEKLILIKMAASIMI", "EMEKLILIKMAASI", "GUNDELIKYIL" };

        /// <summary>
        /// Sürekli görev yolluk belge detay tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "BELGETUR", "TCKIMLIKNOBELGE", "SIRANO", "TCKIMLIKNO", "ADSOYAD", "AKRABALIKKODU", "SABITUNSUR", "TOPLAMTUTAR", "TOPLAMTUTARDOVIZ" };

        /// <summary>
        /// Süreç tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARSUREC = { "YIL", "SURECNO", "SURECDURUM" };

        /// <summary>
        /// Sürekli görev yolluk belge bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sb">Eklenecek sürekli görev yolluk belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(SurekliGorevYollukBelge sb)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sb.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sb.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sb.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", sb.belgeTur));
            if (sb.tcKimlikNo != "" && sb.surecNo == "-1")//Dışardan şifresiz giriş 
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sb.tcKimlikNo));

            IDataReader dr = veriErisim.DataReaderDondur(sql);

            bool sonuc = dr.Read();
            dr.Close();

            if (sonuc) //Kayıt var, güncelle
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], sb.belgeTur) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], sb.tcKimlikNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KADROEK], sb.kadroEk) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNDELIKKOD], sb.gundelikKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREDEN], sb.nereden) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.NEREYE], sb.nereye) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ATAMATARIH], sb.atamaTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASITCESIT], sb.tasitCesit) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BILETUCRET], sb.biletUcret) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNSAYI], sb.gunSayi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNDELIKTUTAR], sb.gundelikTutar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERDEGISMESAFE], sb.yerDegisMesafe) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERDEGISTUTARI], sb.yerDegisTutari) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZKODU], sb.dovizKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOVIZKURU], sb.dovizKuru) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISKENUNSUR], sb.degiskenUnsur) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YERDEGISTIRMEGIDERI], sb.yerDegistirmeGideri) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNVANKODU], sb.unvanKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BILDIRIMTARIH], sb.bildirimTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], sb.odemeEmriNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLILIKMAASIMI], sb.emeklilikMaasiMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EMEKLILIKMAASI], sb.emeklilikMaasi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GUNDELIKYIL], sb.gundelikYil);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sb.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sb.birimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sb.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", sb.belgeTur));
                if (sb.tcKimlikNo != "" && sb.surecNo == "-1")//Dışardan şifresiz giriş 
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sb.tcKimlikNo));
            }
            else //Kayıt yok, ekle
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(sb.yil) + ",";
                sql += VeriErisim.DegerYap(sb.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(sb.birimKod) + ",";
                sql += VeriErisim.DegerYap(sb.surecNo) + ",";
                sql += VeriErisim.DegerYap(sb.belgeTur) + ",";
                sql += VeriErisim.DegerYap(sb.tcKimlikNo) + ",";
                sql += VeriErisim.DegerYap(sb.kadroEk) + ",";
                sql += VeriErisim.DegerYap(sb.gundelikKod) + ",";
                sql += VeriErisim.DegerYap(sb.nereden) + ",";
                sql += VeriErisim.DegerYap(sb.nereye) + ",";
                sql += VeriErisim.DegerYap(sb.atamaTarih) + ",";
                sql += VeriErisim.DegerYap(sb.tasitCesit) + ",";
                sql += VeriErisim.DegerYap(sb.biletUcret) + ",";
                sql += VeriErisim.DegerYap(sb.gunSayi) + ",";
                sql += VeriErisim.DegerYap(sb.gundelikTutar) + ",";
                sql += VeriErisim.DegerYap(sb.yerDegisMesafe) + ",";
                sql += VeriErisim.DegerYap(sb.yerDegisTutari) + ",";
                sql += VeriErisim.DegerYap(sb.dovizKodu) + ",";
                sql += VeriErisim.DegerYap(sb.dovizKuru) + ",";
                sql += VeriErisim.DegerYap(sb.degiskenUnsur) + ",";
                sql += VeriErisim.DegerYap(sb.yerDegistirmeGideri) + ",";
                sql += VeriErisim.DegerYap(sb.unvanKodu) + ",";
                sql += VeriErisim.DegerYap(sb.bildirimTarihi) + ",";
                sql += VeriErisim.DegerYap(sb.odemeEmriNo) + ",";
                sql += VeriErisim.DegerYap(sb.emeklilikMaasiMi) + ",";
                sql += VeriErisim.DegerYap(sb.emeklilikMaasi) + ",";
                sql += VeriErisim.DegerYap(sb.gundelikYil) + ")";
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Sürekli görev yolluk belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sb">Listeleme için kullanılacak sürekli görev yolluk belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürekli görev yolluk belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(SurekliGorevYollukBelge sb)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + DBDegiskeni.TABLOADPERSONEL + ".AD" + "," + DBDegiskeni.TABLOADPERSONEL + ".SOYAD" + "," +
                DBDegiskeni.TABLOADUNVAN + ".AD" + "," + TABSurekliGorevGundelikTanim.TABLOAD + "." + TABSurekliGorevGundelikTanim.ALANLAR[(int)TABSurekliGorevGundelikTanim.ENUMAlanlar.DEGER];
            sql += " FROM " + TABLOAD + " LEFT OUTER JOIN " + DBDegiskeni.TABLOADPERSONEL + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = " + DBDegiskeni.TABLOADPERSONEL + ".TCKIMLIKNO ";
            sql += " LEFT OUTER JOIN " + TABSurekliGorevGundelikTanim.TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GUNDELIKKOD] + " = " + TABSurekliGorevGundelikTanim.TABLOAD + "." + TABSurekliGorevGundelikTanim.ALANLAR[(int)TABSurekliGorevGundelikTanim.ENUMAlanlar.KOD] +
            " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GUNDELIKYIL] + " = " + TABSurekliGorevGundelikTanim.TABLOAD + "." + TABSurekliGorevGundelikTanim.ALANLAR[(int)TABSurekliGorevGundelikTanim.ENUMAlanlar.YIL];
            sql += " LEFT OUTER JOIN " + DBDegiskeni.TABLOADUNVAN + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKODU] + " = " + DBDegiskeni.TABLOADUNVAN + ".KOD ";

            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));
            if (sb.muhasebeKod != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sb.muhasebeKod));
            if (sb.birimKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sb.birimKod));
            if (sb.surecNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sb.surecNo));
            if (sb.belgeTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", sb.belgeTur));
            if (sb.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sb.tcKimlikNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk belge bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Eklenecek sürekli görev yolluk belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetDetay(SurekliGorevYollukDetay sd)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(sd.yil) + ",";
            sql += VeriErisim.DegerYap(sd.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(sd.birimKod) + ",";
            sql += VeriErisim.DegerYap(sd.surecNo) + ",";
            sql += VeriErisim.DegerYap(sd.belgeTur) + ",";
            sql += VeriErisim.DegerYap(sd.tcKimlikNoBelge) + ",";
            sql += VeriErisim.DegerYap(sd.siraNo) + ",";
            sql += VeriErisim.DegerYap(sd.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(sd.adSoyad) + ",";
            sql += VeriErisim.DegerYap(sd.akrabalikKodu) + ",";
            sql += VeriErisim.DegerYap(sd.sabitUnsur) + ",";
            sql += VeriErisim.DegerYap(sd.toplamTutar) + ",";
            sql += VeriErisim.DegerYap(sd.toplamTutarDoviz) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Sürekli görev yolluk belge detay bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Silinecek sürekli görev yolluk belge detay bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc SilDetay(SurekliGorevYollukDetay sd)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "DELETE FROM " + TABLOADDETAY;

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.BELGETUR], "=", sd.belgeTur));
            if (sd.tcKimlikNoBelge != "" && sd.surecNo == "-1")//Dışardan şifresiz giriş 
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE], "=", sd.tcKimlikNoBelge));

            //Tek satır
            if (sd.siraNo != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlarDetay.SIRANO], "=", sd.siraNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Sürekli görev yolluk belge detay bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sd">Listeleme için kullanılacak sürekli görev yolluk belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürekli görev yolluk belge detay bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleDetay(SurekliGorevYollukBelge sd)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLAR[(int)ENUMAlanlarDetay.BELGETUR], "=", sd.belgeTur));
            if (sd.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE], "=", sd.tcKimlikNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk belgesine ait odeme emri belge numarası bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek sürekli görev yolluk belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc OdemeEmriNoKaydet(SurekliGorevYollukBelge bilgi)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", bilgi.belgeTur));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Sürekli görev yolluk belge havuzundaki sürekli görev yolluk belge bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sd">Silinecek sürekli görev yolluk belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc HavuzSil(SurekliGorevYollukBelge sd)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sd.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sd.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sd.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", sd.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", sd.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sd.tcKimlikNo));

            sonuc = KomutCalistir(sql, 0);

            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOADDETAY;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", sd.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", sd.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", sd.birimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO], "=", sd.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", sd.belgeTur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE], "=", sd.tcKimlikNo));

                sonuc = KomutCalistir(sql, 0);
            }

            return sonuc;
        }

        /// <summary>
        /// Sürekli görev yolluk belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak sürekli görev yolluk belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürekli görev yolluk belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleIstatistikKurlu(PSLYollukIstatistikSinif kriter)
        {
            string sql = "";
            string kosul = "";
            string groupBy = "";
            string orderBy = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "/1000"
                + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR]
                + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI"
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + "*" + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.KUR] + ")"
                + ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + ") "
                + ", " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + "(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNOBELGE] + "))"
                 + ", " + VeriErisim.IslecBul("COUNT") + "(*)"
                 + " FROM " + TABLOAD;

            sql += " INNER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + " = " + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR];

            sql += " LEFT OUTER JOIN " + TABKurBilgisi.TABLOAD + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.SURECNO] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + " = " + TABKurBilgisi.TABLOAD + "." + TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.PARABIRIMIKOD];

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABKurBilgisi.TABLOAD, TABKurBilgisi.ALANLAR[(int)TABKurBilgisi.ENUMAlanlar.TIP], "=", 1));

            sql += " INNER JOIN " + TABSurec.TABLOAD + " ON ";
            sql += TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECNO] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO];
            sql += " AND " + TABSurec.TABLOAD + "." + TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.YIL] + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABSurec.TABLOAD, TABSurec.ALANLAR[(int)TABSurec.ENUMAlanlar.SURECDURUM], "<", (int)ENUMSurecDurum.IPTAL));

            sql += " INNER JOIN " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + " ON ";
            sql += "  " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".ILKODU" + "=" + TABLOAD + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD] + "/1000";

            sql += " INNER JOIN " + DBDegiskeni.MUHMIF + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO];
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIF, "HESAPKOD", "LIKE", "830%"));

            sql += " INNER JOIN " + DBDegiskeni.MUHMIFFORM + " ON ";
            sql += "  " + DBDegiskeni.MUHMIF + ".BELGENO" + "=" + DBDegiskeni.MUHMIFFORM + ".BELGENO";
            sql += " AND " + DBDegiskeni.MUHMIF + ".YIL" + "=" + DBDegiskeni.MUHMIFFORM + ".YIL";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(DBDegiskeni.MUHMIFFORM, "FORMDURUM", "<", 9));


            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], "<>", ""));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<>", -1));

            groupBy = " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000" + ", " + DBDegiskeni.TABLOADPERILDADIGILIMIVIEW + ".SAYI";
            orderBy = " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGETUR] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD]
                + "/1000";
            if (kosul != "")
                sql += " WHERE " + kosul;

            if (groupBy != "")
                sql += groupBy + orderBy;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk belgesine ait ödeme emri belge numarası bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="surecNo">Listeleme için kullanılacak süreç numarası bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürekli görev yolluk belgesine ait ödeme emri belge numarası bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ListeleOdemeEmri(int yil, string surecNo)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO] + "," +
                              TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL]
                + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));

            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Sürekli görev yolluk belgesinin kayıtlı olup olmadığı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="sb">Listeleme için kullanılacak sürekli görev yolluk belge bilgisi kıstas parametresi</param>
        /// <returns>
        /// Sürekli görev yolluk belgesinin kayıtlı olup olmadığı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader OncedenKayitliMi(SurekliGorevYollukBelge sb)
        {
            string sql = "SELECT PSLSUREKLIGOREVYOLLUKBELGE.YIL, PSLSUREKLIGOREVYOLLUKBELGE.MUHASEBEKOD, PSLSUREKLIGOREVYOLLUKBELGE.BIRIMKOD, " +
                      " PSLSUREKLIGOREVYOLLUKBELGE.SURECNO, PSLSUREKLIGOREVYOLLUKBELGE.BELGETUR, PSLSUREKLIGOREVYOLLUKBELGE.TCKIMLIKNO, " +
                      " PSLSUREKLIGOREVYOLLUKBELGE.ATAMATARIH " +
                      " FROM         PSLSUREKLIGOREVYOLLUKBELGE " +
                      " INNER JOIN HRCSUREC ON " +
                      " PSLSUREKLIGOREVYOLLUKBELGE.YIL = HRCSUREC.YIL " +
                      " AND PSLSUREKLIGOREVYOLLUKBELGE.SURECNO = HRCSUREC.SURECNO";

            sql = VeriErisim.KosulEkle(sql, " WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSUREC", "SURECDURUM", "<>", 9));
            if (sb.muhasebeKod != 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", sb.muhasebeKod));
            if (sb.birimKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", sb.birimKod));
            if (sb.tcKimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", sb.tcKimlikNo));
            if (!sb.atamaTarih.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ATAMATARIH], "=", sb.atamaTarih));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk toplam adam sayısı bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, double döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür bilgisi kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse il bazında toplam verir</param>
        /// <returns>
        /// Sürekli görev yolluk toplam adam sayısı bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// double nesnesini döndürür
        /// </returns>
        public double ToplamAdamSayisi(int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT COUNT(*) AS ADAMSAYI ";
            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                }
            } if (belgeTur != ENUMSurekliGorevTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk ortalama mesafe bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, double döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür bilgisi kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse il bazında toplam verir</param>
        /// <returns>
        /// Sürekli görev yolluk ortalama mesafe bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// double nesnesini döndürür
        /// </returns>
        public double OrtalamaMesafe(int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT SUM(" + ALANLAR[(int)ENUMAlanlar.YERDEGISMESAFE] + ") / COUNT(*) AS ORTMESAFE ";
            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                }
            } if (belgeTur != ENUMSurekliGorevTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk toplam yevmiye tutar bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, double döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür bilgisi kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse il bazında toplam verir</param>
        /// <returns>
        /// Sürekli görev yolluk toplam yevmiye tutar bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// double nesnesini döndürür
        /// </returns>
        public double ToplamYevmiyeTutar(int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT SUM(" + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + ") AS ODENEN ";
            sql += " FROM " + TABLOADDETAY + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.MUHASEBEKOD], "=", muhasebeKod));
                }
            } if (belgeTur != ENUMSurekliGorevTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk belge toplam tayin bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür bilgisi kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse il bazında toplam verir</param>
        /// <returns>
        /// Sürekli görev yolluk belge toplam tayin bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ToplamTayinBilgileriListele(int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT COUNT(DISTINCT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "), SUM(" + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + ") AS ODENEN ";

            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            sql += " LEFT OUTER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMEKLILIKMAASIMI], "=", 0));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                }
            } if (belgeTur != ENUMSurekliGorevTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk belge toplam emeklilik bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür bilgisi kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse il bazında toplam verir</param>
        /// <returns>
        /// Sürekli görev yolluk belge toplam emeklilik bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ToplamEmekliBilgileriListele(int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT COUNT(DISTINCT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + ") AS KISISAYI, SUM(" + ALANLAR[(int)ENUMAlanlar.EMEKLILIKMAASI] + ") AS ODENEN ";

            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMEKLILIKMAASIMI], "=", 1));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                }
            } if (belgeTur != ENUMSurekliGorevTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sürekli görev yolluk belge toplam gerçekleşme bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="yil">Listeleme için kullanılacak yıl bilgisi kıstas parametresi</param>
        /// <param name="muhasebeKod">Listeleme için kullanılacak muhasebe kod bilgisi kıstas parametresi</param>
        /// <param name="birimKod">Listeleme için kullanılacak birim kod bilgisi kıstas parametresi</param>
        /// <param name="belgeTur">Listeleme için kullanılacak belge tür bilgisi kıstas parametresi</param>
        /// <param name="sadeceIl">Eğer <c>true</c> verilirse il bazında toplam verir</param>
        /// <returns>
        /// Sürekli görev yolluk belge toplam gerçekleşme bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader ToplamGerceklesmeListele(int yil, int muhasebeKod, string birimKod, ENUMSurekliGorevTuru belgeTur, bool sadeceIl)
        {
            string sql = "";

            sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.NEREYE] + ", COUNT(DISTINCT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + ") AS KISISAYI, SUM(" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR] + ") AS TUTAR";

            sql += " FROM " + TABLOAD + " INNER JOIN " + TABLOADSUREC + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADSUREC + "." + ALANLARSUREC[(int)ENUMAlanlarSurec.SURECNO];

            sql += " LEFT OUTER JOIN " + TABLOADDETAY + " ON ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.YIL] + " AND ";
            sql += TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.SURECNO];

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSUREC, ALANLARSUREC[(int)ENUMAlanlarSurec.SURECDURUM], "<>", 9));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TOPLAMTUTAR], ">", 0));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EMEKLILIKMAASIMI], "=", 0));
            if (yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (birimKod != "")
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }
            else
            {
                if (sadeceIl)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ">=", muhasebeKod * 1000));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "<", (muhasebeKod + 1) * 1000));
                }
                else
                {
                    if (muhasebeKod > 0)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                }
            }
            if (belgeTur != ENUMSurekliGorevTuru.TANIMSIZ)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "," + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.NEREYE];
            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.NEREYE];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

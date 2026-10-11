using System;
using System.Data;
using TNS.Veri;
using TNS.BKD;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Para birimi bilgisini tutan HRCPARABIRIMI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGeciciGorevYollukTalep : TABOrtak
    {

        /*
         * 
         * 
CREATE TABLE HRCGGYTALEPBERABER(
YIL INT NOT NULL
, FORMKOD VARCHAR(50) NOT NULL
, SIRANO INT NOT NULL
, TCKIMLIK VARCHAR(50) NOT NULL
, PRIMARY KEY(YIL, FORMKOD, SIRANO)
);
         * 
         * */


        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGeciciGorevYollukTalep(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Görev yolluğu bilgisini tutan tablonun adını tutan sabit değişken
        /// </summary>
        static string TABLOADBASLIK = "HRCGGYTALEPBASLIK";

        /// <summary>
        /// Yolculuk günlerini tutan tablonun adını tutan sabit değişken
        /// </summary>
        static string TABLOADDETAY = "HRCGGYTALEPDETAY";

        /// <summary>
        /// Yolculukta beraber seyahat edilecek kişileri tutan tablonun adını tutan sabit değişken
        /// </summary>
        static string TABLOADBERABER = "HRCGGYTALEPBERABER";

        /// <summary>
        /// Görev yolluğu bilgisini tutan tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarBaslik
        {
            YIL = 0,
            FORMKOD = 1,
            MUHASEBE = 2,
            BIRIM = 3,
            KURKOD = 4,
            DURUM = 5,
            TARIH = 6,
            TCKIMLIKNO = 7,
            AD = 8,
            SOYAD = 9,
            UNVAN = 10,
            GUNDELIKYIL = 11,
            GUNDELIKKOD = 12,
            GUNDELIKTUTAR = 13,
            GUNDELIKDOVIZKOD = 14,
            NEREDEN = 15,
            NEREYE = 16,
            BASLANGICTARIH = 17,
            BITISTARIH = 18,
            SURECYIL = 19,
            SURECNO = 20,
            TUR = 21,
            GOREVSURESI = 22,
            GEREKCE = 23,
            ACIKLAMALAR = 24,
            TERTIP = 25,
            AVANS = 26,
            SUNANAD = 27,
            SUNANUNVAN = 28,
            HAZIRAD = 29,
            HAZIRUNVAN = 30,
            ONAYAD = 31,
            ONAYUNVAN = 32,
            ONAYMAKAM = 33,
            AVANSODEMEEMRI = 34
        }

        /// <summary>
        ///  Yolculuk günlerini tutan tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarDetay
        {
            YIL = 0,
            FORMKOD = 1,
            SIRANO = 2,
            TUR = 3,
            TARIH = 4,
            GUN = 5
        }

        /// <summary>
        /// Yolculukta beraber seyahat edilecek kişileri tutan tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlarBeraber
        {
            YIL = 0,
            FORMKOD = 1,
            SIRANO = 2,
            TCKIMLIK = 3
        }

        /// <summary>
        /// Görev yolluğu bilgisini tutan tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBASLIK = { "YIL", "FORMKOD", "MUHASEBE", "BIRIM", "KURKOD", "DURUM", "TARIH", "TCKIMLIKNO", "AD", "SOYAD", "UNVAN", "GUNDELIKYIL",
                                        "GUNDELIKKOD","GUNDELIKTUTAR","GUNDELIKDOVIZKOD","NEREDEN","NEREYE","BASLANGICTARIH","BITISTARIH","SURECYIL","SURECNO","TUR",
                                        "GOREVSURESI", "GEREKCE", "ACIKLAMALAR", "TERTIP", "AVANS", "SUNANAD","SUNANUNVAN","HAZIRAD","HAZIRUNVAN","ONAYAD","ONAYUNVAN", "ONAYMAKAM", "AVANSODEMEEMRI"};

        /// <summary>
        ///  Yolculuk günlerini tutan tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = { "YIL", "FORMKOD", "SIRANO", "TUR", "TARIH", "GUN" };

        /// <summary>
        ///  Yolculukta beraber seyahat edilecek kişileri tutan tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBERABER = { "YIL", "FORMKOD", "SIRANO", "TCKIMLIK" };

        //public Sonuc Guncelle(GeciciGorevYollukTalepBaslik bilgi)
        //{
        //    string sql = "UPDATE " + TABLOADBASLIK + " SET ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.MUHASEBE], bilgi.muhasebe) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.BIRIM], bilgi.birim) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.KURKOD], bilgi.kurKod) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.TCKIMLIKNO], bilgi.kimlikNo) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.GEREKCE], bilgi.gerekce) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.NEREDEN], bilgi.nereden) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.NEREYE], bilgi.nereye) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.GOREVSURESI], 0) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.BASLANGICTARIH], bilgi.baslangicTarih) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.BITISTARIH], bilgi.bitisTarih) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.GUNDELIKDOVIZKOD], bilgi.gundelikDovizKod) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.GUNDELIKKOD], bilgi.gundelikKod) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.GUNDELIKTUTAR], bilgi.gundelikTutar) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.GUNDELIKYIL], bilgi.gundelikYil) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.SURECYIL], bilgi.surecYil) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.SURECNO], bilgi.surecNo) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.ACIKLAMALAR], bilgi.aciklamalar) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.TERTIP], bilgi.butceTertibi) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.AVANS], bilgi.avansTutari) + ", ";

        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.SUNANAD], bilgi.sunanAd) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.SUNANUNVAN], bilgi.sunanUnvan) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.HAZIRAD], bilgi.hazirAd) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.HAZIRUNVAN], bilgi.hazirUnvan) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.ONAYAD], bilgi.onayAd) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.ONAYUNVAN], bilgi.onayUnvan) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.ONAYMAKAM], bilgi.onayMakam) + ", ";
        //    sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.AVANSODEMEEMRI], bilgi.avansOdemeEmri) + " ";

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", bilgi.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", bilgi.formKod));

        //    return KomutCalistir(sql, 1);
        //}

        /// <summary>
        /// Para birimi bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek para birimi bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(GeciciGorevYollukTalepBaslik bilgi)
        {
            string sql = "INSERT INTO " + TABLOADBASLIK + " (" + VeriErisim.TumAlanlar("", ALANLARBASLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.formKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.kurKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.durum) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.kimlikNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.soyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.unvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.gundelikYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.gundelikKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.gundelikTutar) + ",";
            sql += VeriErisim.DegerYap(bilgi.gundelikDovizKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.nereden) + ",";
            sql += VeriErisim.DegerYap(bilgi.nereye) + ",";
            sql += VeriErisim.DegerYap(bilgi.baslangicTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.bitisTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ",";
            sql += VeriErisim.DegerYap((int)0) + ",";
            sql += VeriErisim.DegerYap(bilgi.gerekce) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklamalar) + ",";
            sql += VeriErisim.DegerYap(bilgi.butceTertibi) + ",";
            sql += VeriErisim.DegerYap(bilgi.avansTutari) + ",";

            sql += VeriErisim.DegerYap(bilgi.sunanAd) + ",";
            sql += VeriErisim.DegerYap(bilgi.sunanUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.hazirAd) + ",";
            sql += VeriErisim.DegerYap(bilgi.hazirUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onayAd) + ",";
            sql += VeriErisim.DegerYap(bilgi.onayUnvan) + ",";
            sql += VeriErisim.DegerYap(bilgi.onayMakam) + ",";
            sql += VeriErisim.DegerYap(bilgi.avansOdemeEmri) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumGuncelle(GeciciGorevYollukTalepBaslik form, ENUMGeciciGorevYollukTalepDurum durum)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADBASLIK + " SET ";
            sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.DURUM], (int)durum);
            sql += ", " + VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.AVANSODEMEEMRI], form.avansOdemeEmri);
            sql += ", " + VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.AVANS], form.avansTutari);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", form.formKod));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc SurecBilgiGuncelle(int yil, string formKod, int surecYil, string surecNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADBASLIK + " SET ";
            sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.SURECYIL], surecYil) + ",";
            sql += VeriErisim.AtamaOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.SURECNO], surecNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", formKod));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc SilAna(GeciciGorevYollukTalepBaslik bilgi)
        {
            string sql = "DELETE FROM " + TABLOADBASLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADBASLIK, ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", bilgi.formKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Oku(int yil, string formKod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARBASLIK);
            sql += " FROM " + TABLOADBASLIK;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBASLIK[(int)ENUMAlanlarBaslik.FORMKOD], "=", formKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetDetay(int yil, string formKod, int i, GeciciGorevYollukTalepDetay bilgi)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(formKod) + ",";
            sql += VeriErisim.DegerYap(i) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.gun) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDetay(int yil, string formKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.FORMKOD], "=", formKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleDetay(int yil, string formKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FORMKOD], "=", formKod));

            sql += " ORDER BY 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilBeraberindekiler(int yil, string formKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADBERABER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBERABER[(int)ENUMAlanlarBeraber.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBERABER[(int)ENUMAlanlarBeraber.FORMKOD], "=", formKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KaydetBeraber(int yil, string formKod, int siraNo, string tcKimlik)
        {
            string sql = "INSERT INTO " + TABLOADBERABER + " (" + VeriErisim.TumAlanlar("", ALANLARBERABER) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(formKod) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(tcKimlik) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleBeraber(int yil, string formKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARBERABER) + " FROM " + TABLOADBERABER;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBERABER[(int)ENUMAlanlarBeraber.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBERABER[(int)ENUMAlanlarBeraber.FORMKOD], "=", formKod));
            sql += " ORDER BY 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

//public IDataReader Sorgula(SatinAlmaTalepFormSorgu bilgi)
//{
//    string sql = "";
//    string select = "";
//    string from = "";
//    string innerJoin = "";
//    string where = "";
//    string kosul = "";
//    string orderBy = "";

//    //SELECT//
//    select += "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR)
//                  + "," + VeriErisim.TumAlanlar(TABLOADIS, ALANLARIS);
//    //+ "," + TABLOADMUHASEBEVIEW + "." + ALANLARMUHASEBEVIEW[(int)ENUMAlanlarMUHASEBEVIEW.MUHASEBEAD]
//    //+ "," + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.PARCAKOD]
//    //+ "," + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.TARIH]
//    //+ "," + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.KULLANICIAD];
//    //--------------------->

//    //FROM//
//    from += " FROM " + TABLOAD;

//    //innerJoin += " INNER JOIN " + TABLOADMUHASEBEVIEW + " ON " +
//    //            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "=" + TABLOADMUHASEBEVIEW + "." + ALANLARMUHASEBEVIEW[(int)ENUMAlanlarMUHASEBEVIEW.MUHASEBEKOD];

//    innerJoin += " LEFT OUTER JOIN " + TABLOADIS + " ON " +
//                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADIS + "." + ALANLARIS[(int)ENUMAlanlarIS.KOD];

//    //innerJoin += " LEFT OUTER JOIN " + TABLOADDURUM + " ON " +
//    //            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.YIL]
//    //            + " AND " +
//    //            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.SURECNO]
//    //            + " AND " +
//    //            TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEMKOD] + "=" + TABLOADDURUM + "." + ALANLARDURUM[(int)ENUMAlanlarDurum.ISLEMKOD];
//    //--------------------->

//    //WHERE//
//    kosul = "";
//    if (bilgi.yil > 0)
//        kosul = VeriErisim.KosulEkle(kosul, "AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
//    if (bilgi.muhasebe > 0)
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", bilgi.muhasebe));
//    if (bilgi.birim != "")
//    {
//        string[] birimler = bilgi.birim.Split(';');
//        string birimKosul = "";
//        for (int i = 0; i < birimler.Length; i++)
//        {
//            if (birimler[i].Trim() != "")
//                birimKosul = VeriErisim.KosulEkle(birimKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birimler[i].Trim()));
//        }
//        if (birimKosul != "")
//            kosul += " AND (" + birimKosul + ")";
//    }

//    if (bilgi.durum > 0)
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));
//    else
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<>", (int)ENUMSatinalmaTalepFormDurum.IPTAL));

//    if (bilgi.islemKod > 0)
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", bilgi.islemKod));

//    //if (bilgi.muhasebe > 0)
//    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", bilgi.muhasebe));
//    //if (bilgi.birim != "")
//    //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", bilgi.birim));

//    if (bilgi.formKodBasla != "")
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMKOD], ">=", bilgi.formKodBasla));
//    if (bilgi.formKodBitis != "")
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMKOD], "<=", bilgi.formKodBitis));
//    if (!bilgi.iseBaslamaTarihiBasla.isNull)
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], ">=", bilgi.iseBaslamaTarihiBasla));
//    if (!bilgi.iseBaslamaTarihiBitis.isNull)
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], "<=", bilgi.iseBaslamaTarihiBitis));
//    if (bilgi.yaklasikMaliyetBasla > 0)
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYET], ">=", bilgi.yaklasikMaliyetBasla));
//    if (bilgi.yaklasikMaliyetBitis > 0)
//        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISEBASLAMATARIHI], "<=", bilgi.yaklasikMaliyetBitis));

//    where += " WHERE " + kosul;
//    //---------------------->

//    //ORDERBY//
//    orderBy += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + "," + ALANLAR[(int)ENUMAlanlar.FORMKOD];
//    //----------------------->

//    sql = select + from + innerJoin + where + orderBy;
//    //OrtakFonksiyonlar.HataStrYaz(sql);
//    return veriErisim.DataReaderDondur(sql);
//}

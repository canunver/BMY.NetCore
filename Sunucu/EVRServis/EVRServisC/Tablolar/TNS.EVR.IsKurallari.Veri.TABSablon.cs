using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Şablon bilgilerini içeren EVRSABLON veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABSablon : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABSablon sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABSablon(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Şablon bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRSABLON";

        /// <summary>
        /// Şablon bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0, AD = 1, KULLANICIKODU = 2, EVRAKBIRIMKODU = 3, KAYITTARIHI = 4, EVRAKTURU = 5, GIZLILIK = 6, ANTET = 7, YAZIDURUM = 8, DOSYAPLANKODU = 9, KONU = 10, GIDECEGIYER = 11, ICERIK = 12, EK = 13, PARAF = 14, ALTBILGI = 15, DETAY = 16,
        }

        /// <summary>
        /// Şablon bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KOD", "AD", "KULLANICIKODU", "EVRAKBIRIMKODU", "KAYITTARIHI", "EVRAKTURU", "GIZLILIK", "ANTET", "YAZIDURUM", "DOSYAPLANKODU", "KONU", "GIDECEGIYER", "ICERIK", "EK", "PARAF", "ALTBILGI", "DETAY" };

        /// <summary>
        /// Parametre olarak verilen sablon nesnesine göre; şablon bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Şablon bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(Sablon form)
        {
            if (form.kod == 0) { form.kod = SonKod(); }
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kod) + ",";
            sql += VeriErisim.DegerYap(form.ad) + ",";
            sql += VeriErisim.DegerYap(form.kullaniciKodu) + ",";
            sql += VeriErisim.DegerYap(form.evrakBrimKodu) + ",";
            sql += VeriErisim.DegerYap(form.kayitTarihi) + ",";
            sql += VeriErisim.DegerYap(form.evrakTuru) + ",";
            sql += VeriErisim.DegerYap(form.gizlilik) + ",";
            sql += VeriErisim.DegerYap(form.antet) + ",";
            sql += VeriErisim.DegerYap(form.yaziDurum) + ",";
            sql += VeriErisim.DegerYap(form.dosyaPlanKodu) + ",";
            sql += VeriErisim.DegerYap(form.konu) + ",";
            sql += VeriErisim.DegerYap(form.gidecegiYer) + ",";
            sql += VeriErisim.DegerYap(form.icerik) + ",";
            sql += VeriErisim.DegerYap(form.ek) + ",";
            //sql += VeriErisim.DegerYap(form.dagitimGeregi) + ",";
            //sql += VeriErisim.DegerYap(form.dagitimBilgi) + ",";
            sql += VeriErisim.DegerYap(form.paraf) + ",";
            sql += VeriErisim.DegerYap(form.altBilgi) + ",";
            sql += VeriErisim.DegerYap(form.detay) + ")";
            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DETAY], form.detay) + ",";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKTURU], form.evrakTuru) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIZLILIK], form.gizlilik) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANTET], form.antet) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAZIDURUM], form.yaziDurum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYAPLANKODU], form.dosyaPlanKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONU], form.konu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICERIK], form.icerik) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIDECEGIYER], form.gidecegiYer) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EK], form.ek) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARAF], form.paraf) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTBILGI], form.altBilgi);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", (form.kod)));
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen sablon nesnesine göre; şablon bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="form">Şablon bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(Sablon form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = "";
            if (form.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            if (!string.IsNullOrEmpty(form.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "=", form.ad));

            //if (form.detay == 1 && !string.IsNullOrEmpty(form.evrakBrimKodu)) //Liste 
            //{
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DETAY], "=", form.detay));
            //    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "like", form.evrakBrimKodu + "%"));
            //    if (!string.IsNullOrEmpty(form.kullaniciKodu))
            //    {
            //        string altKosul = "";
            //        altKosul = VeriErisim.KosulEkle(altKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "like", form.evrakBrimKodu + "%"));
            //        altKosul = VeriErisim.KosulEkle(altKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", form.kullaniciKodu));
            //        kosul = "(" + kosul + ") OR (" + altKosul + ")";
            //    }
            //}
            //if (string.IsNullOrEmpty(form.evrakBrimKodu))
            //{
            //    if (!string.IsNullOrEmpty(form.kullaniciKodu))
            //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", form.kullaniciKodu));

            //    if (form.detay != -1)
            //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DETAY], "=", form.detay));
            //}

             if (!string.IsNullOrEmpty(form.evrakBrimKodu) && !string.IsNullOrEmpty(form.kullaniciKodu))
            { //benim şablonlarım 
                if (!string.IsNullOrEmpty(form.kullaniciKodu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", form.kullaniciKodu));

                if(!string.IsNullOrEmpty(form.evrakBrimKodu))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "like", form.evrakBrimKodu + "%"));

                if (form.detay != -1) //birim ile paylaşılıp paylaşılmayacağı kontrol ediliyor
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DETAY], "=", form.detay));
            }

             else  if (form.detay == 1 && string.IsNullOrEmpty(form.kullaniciKodu))
             { //hepsibirim  (bazlı)
                 if (!string.IsNullOrEmpty(form.evrakBrimKodu))
                     kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "like", form.evrakBrimKodu + "%"));
                     kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DETAY], "=", form.detay));
             }

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen şablon kod bilgisine göre; şablon bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="sablonKod">The sablon kod.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc Sil(int sablonKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", sablonKod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Veritabanına yeni şablon bilgisi kaydetmek için, sql cümlesi hazırlayıp çalıştırarak yeni bir şablon kodu oluşturan yordam.
        /// </summary>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public int SonKod()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}

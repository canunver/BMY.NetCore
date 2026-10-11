using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// BHFTASITBILGI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHFTASITBILGI : TABOrtak
    {
        /// <summary>
        /// TABBHFTASITBILGI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHFTASITBILGI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHFTASITBILGI";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADDETAY = "BHFTASITBILGIDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SASENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SASENO = 0,

            /// <summary>
            /// MODELKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODELKODU = 1,

            /// <summary>
            /// MODELYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODELYILI = 2,

            /// <summary>
            /// CINSKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CINSKODU = 3,

            /// <summary>
            /// MOTORNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MOTORNO = 4,

            /// <summary>
            /// KOLTUKSAYISI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOLTUKSAYISI = 5,

            /// <summary>
            /// YAKITTURU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YAKITTURU = 6,

            /// <summary>
            /// AITOLDUGUBUTCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AITOLDUGUBUTCE = 7,

            /// <summary>
            /// KULLANIMDURUMU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANIMDURUMU = 8,

            /// <summary>
            /// ONAYKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYKODU = 9,

            ARACSINIFI = 10,

            RUHSATRESIMAD = 11,

            RUHSATRESIM = 12,

            MODELGIRILEN = 13,

        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDetay : int
        {

            /// <summary>
            /// SASENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SASENO = 0,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// BULUNDUGUSAYMANLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULUNDUGUSAYMANLIK = 2,

            /// <summary>
            /// BULUNDUGUBIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BULUNDUGUBIRIM = 3,

            /// <summary>
            /// AITOLDUGUSAYMANLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AITOLDUGUSAYMANLIK = 4,

            /// <summary>
            /// AITOLDUGUBIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AITOLDUGUBIRIM = 5,

            /// <summary>
            /// PLAKANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PLAKANO = 6,

            /// <summary>
            /// GIRISTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GIRISTARIH = 7,

            /// <summary>
            /// CIKISTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CIKISTARIH = 8,


            CIKISEVRAKSAYISI = 9,

            /// <summary>
            /// CIKISSEBEBI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CIKISSEBEBI = 10,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SASENO"
            , "MODELKODU"
            , "MODELYILI"
            , "CINSKODU"
            , "MOTORNO"
            , "KOLTUKSAYISI"
            , "YAKITTURU"
            , "AITOLDUGUBUTCE"
            , "KULLANIMDURUMU"
            , "ONAYKODU"
            , "ARACSINIFI"
            , "RUHSATRESIMAD"
            , "RUHSATRESIM"
            , "MODELGIRILEN"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = {
            "SASENO",
            "SIRANO",
            "BULUNDUGUSAYMANLIK",
            "BULUNDUGUBIRIM",
            "AITOLDUGUSAYMANLIK",
            "AITOLDUGUBIRIM",
            "PLAKANO",
            "GIRISTARIH",
            "CIKISTARIH",
            "CIKISEVRAKSAYISI",
            "CIKISSEBEBI" };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, TasitBilgi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc DetaySil(TasitBilgi form)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SASENO], "=", form.saseNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, TasitBilgi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(TasitBilgi form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SASENO], "=", form.saseNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, TasitBilgi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc DetayGuncelle(TasitBilgi form)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BULUNDUGUSAYMANLIK], form.bulunduguSaymanlik);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.BULUNDUGUBIRIM], form.bulunduguBirim);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AITOLDUGUSAYMANLIK], form.aitOlduguSaymanlik);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.AITOLDUGUBIRIM], form.aitOlduguBirim);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PLAKANO], form.plakaNo.Replace(" ", ""));
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.GIRISTARIH], form.girisTarih);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.CIKISTARIH], form.cikisTarih);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.CIKISEVRAKSAYISI], form.cikisEvrakSayisi);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.CIKISSEBEBI], (int)form.cikisSebebi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SASENO], "=", form.saseNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", 0));
            return KomutCalistir(sql, 1);
        }

        public Sonuc DetayGuncelle(string saseNo, int siraNo, TNSDateTime cikisTarih, string cikisEvrakSayisi, ENUMAracCikisSebebi cikisSebebi)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.CIKISTARIH], cikisTarih);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.CIKISEVRAKSAYISI], cikisEvrakSayisi);
            sql += "," + VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.CIKISSEBEBI], (int)cikisSebebi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SASENO], "=", saseNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO], "=", siraNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, TasitBilgi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(TasitBilgi form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODELKODU], form.modelKodu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODELYILI], form.modelYili);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ARACSINIFI], form.aracSinifi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CINSKODU], form.cinsKodu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MOTORNO], form.motorNo);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOLTUKSAYISI], form.koltukSayisi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKITTURU], (int)form.yakitTuru);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AITOLDUGUBUTCE], (int)form.aitOlduguButce);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANIMDURUMU], (int)form.kullanimDurumu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYKODU], form.onayKodu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MODELGIRILEN], form.modelGirilen);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SASENO], "=", form.saseNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, TasitBilgi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(TasitBilgi form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.saseNo);
            sql += "," + VeriErisim.DegerYap(form.modelKodu);
            sql += "," + VeriErisim.DegerYap(form.modelYili);
            sql += "," + VeriErisim.DegerYap(form.cinsKodu);
            sql += "," + VeriErisim.DegerYap(form.motorNo);
            sql += "," + VeriErisim.DegerYap(form.koltukSayisi);
            sql += "," + VeriErisim.DegerYap((int)form.yakitTuru);
            sql += "," + VeriErisim.DegerYap((int)form.aitOlduguButce);
            sql += "," + VeriErisim.DegerYap((int)form.kullanimDurumu);
            sql += "," + VeriErisim.DegerYap(form.onayKodu);
            sql += "," + VeriErisim.DegerYap(form.aracSinifi);
            sql += ", null, null";
            sql += "," + VeriErisim.DegerYap(form.modelGirilen);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, TasitBilgi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc DetayEkle(TasitBilgi form)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.saseNo);
            sql += "," + VeriErisim.DegerYap(0);
            sql += "," + VeriErisim.DegerYap(form.bulunduguSaymanlik);
            sql += "," + VeriErisim.DegerYap(form.bulunduguBirim);
            sql += "," + VeriErisim.DegerYap(form.aitOlduguSaymanlik);
            sql += "," + VeriErisim.DegerYap(form.aitOlduguBirim);
            sql += "," + VeriErisim.DegerYap(form.plakaNo.Replace(" ", ""));
            sql += "," + VeriErisim.DegerYap(form.girisTarih);
            sql += "," + VeriErisim.DegerYap(form.cikisTarih);
            sql += "," + VeriErisim.DegerYap(form.cikisEvrakSayisi);
            sql += "," + VeriErisim.DegerYap((int)form.cikisSebebi);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, TasitBilgi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(TasitBilgi form, bool ayniKayitlar, TNSCollection formlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", " + TABBHFTASITMARKAMODEL.TABLOAD + "." + TABBHFTASITMARKAMODEL.ALANLAR[(int)TABBHFTASITMARKAMODEL.ENUMAlanlar.MARKA] + ", " + TABBHFTASITMARKAMODEL.TABLOAD + "." + TABBHFTASITMARKAMODEL.ALANLAR[(int)TABBHFTASITMARKAMODEL.ENUMAlanlar.MODEL];
            sql += ", " + TABBHFTASITCINS.TABLOAD + "." + TABBHFTASITCINS.ALANLAR[(int)TABBHFTASITCINS.ENUMAlanlar.CINS];
            sql += ", " + TABBHFTASITCINS.TABLOAD + "." + TABBHFTASITCINS.ALANLAR[(int)TABBHFTASITCINS.ENUMAlanlar.TIPKODU];
            sql += ", " + TABBHFTASITCINS.TABLOAD + "." + TABBHFTASITCINS.ALANLAR[(int)TABBHFTASITCINS.ENUMAlanlar.TKODU];
            sql += ", " + TABBHFTASITTIP.TABLOAD + "." + TABBHFTASITTIP.ALANLAR[(int)TABBHFTASITTIP.ENUMAlanlar.TIP];
            sql += ", " + TABBHFTASITMARKAMODEL.TABLOAD + "." + TABBHFTASITMARKAMODEL.ALANLAR[(int)TABBHFTASITMARKAMODEL.ENUMAlanlar.KESIN];
            sql += ", " + TABBHFTASITCINS.TABLOAD + ".MOTORSUZ";
            sql += ", " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            sql += ", MUH1.MUHASEBEAD, BIR1.BIRIMAD, MUH2.MUHASEBEAD, BIR2.BIRIMAD";
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN  " + TABLOADDETAY + " ON " + VeriErisim.Join(TABLOAD, TABLOADDETAY, new string[] { ALANLAR[(int)ENUMAlanlar.SASENO] });
            sql += " LEFT OUTER JOIN  " + TABBHFTASITMARKAMODEL.TABLOAD + " ON " + VeriErisim.Join(TABLOAD, TABBHFTASITMARKAMODEL.TABLOAD, new string[] { ALANLAR[(int)ENUMAlanlar.MODELKODU] }, new string[] { "KOD" });
            sql += " LEFT OUTER JOIN " + TABBHFTASITCINS.TABLOAD + "  ON " + VeriErisim.Join(TABLOAD, TABBHFTASITCINS.TABLOAD, new string[] { ALANLAR[(int)ENUMAlanlar.CINSKODU] }, new string[] { "KOD" });
            sql += " LEFT OUTER JOIN " + TABBHFTASITTIP.TABLOAD + " ON " + VeriErisim.Join(TABBHFTASITCINS.TABLOAD, TABBHFTASITTIP.TABLOAD, new string[] { TABBHFTASITCINS.ALANLAR[(int)TABBHFTASITCINS.ENUMAlanlar.TIPKODU] }, new string[] { "KOD" });
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE MUH1 ON " + VeriErisim.Join(TABLOADDETAY, "MUH1", new string[] { ALANLARDETAY[(int)ENUMAlanlarDetay.AITOLDUGUSAYMANLIK] }, new string[] { "MUHASEBEKOD" });
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM BIR1 ON " + VeriErisim.Join(TABLOADDETAY, "BIR1", new string[] { ALANLARDETAY[(int)ENUMAlanlarDetay.AITOLDUGUSAYMANLIK], ALANLARDETAY[(int)ENUMAlanlarDetay.AITOLDUGUBIRIM] }, new string[] { "MUHASEBEKOD", "BIRIMKOD" });
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE MUH2 ON " + VeriErisim.Join(TABLOADDETAY, "MUH2", new string[] { ALANLARDETAY[(int)ENUMAlanlarDetay.BULUNDUGUSAYMANLIK] }, new string[] { "MUHASEBEKOD" });
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM BIR2 ON " + VeriErisim.Join(TABLOADDETAY, "BIR2", new string[] { ALANLARDETAY[(int)ENUMAlanlarDetay.BULUNDUGUSAYMANLIK], ALANLARDETAY[(int)ENUMAlanlarDetay.BULUNDUGUBIRIM] }, new string[] { "MUHASEBEKOD", "BIRIMKOD" });
            string kosul = "";
            if (formlar != null)
                kosul = ListeKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SASENO], formlar);
            else if (ayniKayitlar)
            {
                if (!string.IsNullOrWhiteSpace(form.motorNo))
                    kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MOTORNO], "=", form.motorNo);
                if (!string.IsNullOrWhiteSpace(form.plakaNo))
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.PLAKANO], "=", form.plakaNo.Replace(" ", "")));
                kosul = VeriErisim.KosulEkle(VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SASENO], "<>", form.saseNo), "AND", "(" + kosul + ")");
            }
            else if (!string.IsNullOrWhiteSpace(form.saseNo))
            {
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SASENO], "=", form.saseNo);
            }
            else
            {
                if (form.bulunduguSaymanlik > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BULUNDUGUSAYMANLIK], "=", form.bulunduguSaymanlik));
                if (!string.IsNullOrWhiteSpace(form.bulunduguBirim))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BULUNDUGUBIRIM], "=", form.bulunduguBirim));
                if (form.aitOlduguSaymanlik > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.AITOLDUGUSAYMANLIK], "=", form.aitOlduguSaymanlik));
                if (!string.IsNullOrWhiteSpace(form.aitOlduguBirim))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.AITOLDUGUBIRIM], "=", form.aitOlduguBirim));
                if (!string.IsNullOrWhiteSpace(form.plakaNo))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.PLAKANO], "LIKE", form.plakaNo.Replace(" ", "") + "%"));

                if (!string.IsNullOrWhiteSpace(form.markasi))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABBHFTASITMARKAMODEL.TABLOAD, TABBHFTASITMARKAMODEL.ALANLAR[(int)TABBHFTASITMARKAMODEL.ENUMAlanlar.MARKA], "=", form.markasi));
                if (!string.IsNullOrWhiteSpace(form.modeli))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABBHFTASITMARKAMODEL.TABLOAD, TABBHFTASITMARKAMODEL.ALANLAR[(int)TABBHFTASITMARKAMODEL.ENUMAlanlar.MODEL], "LIKE", "%" + form.modeli + "%"));
                if (form.modelYili > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MODELYILI], "=", form.modelYili));
                if (form.cinsKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CINSKODU], "=", form.cinsKodu));
                if (form.tipKodu > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABBHFTASITCINS.TABLOAD, TABBHFTASITCINS.ALANLAR[(int)TABBHFTASITCINS.ENUMAlanlar.TIPKODU], "=", form.tipKodu));
                if (form.yakitTuru != ENUMYakitTuru.Bos)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YAKITTURU], "=", (int)form.yakitTuru));
                if (form.aitOlduguButce != ENUMAitOlduguButce.Bos)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AITOLDUGUBUTCE], "=", (int)form.aitOlduguButce));
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SASENO] + ", " + ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO] + " ASC";
            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeKosulOl(string tabloAd, string alanAd, TNSCollection formlar)
        {
            if (!string.IsNullOrWhiteSpace(tabloAd)) alanAd = tabloAd + "." + alanAd;

            string kosul = "";
            foreach (TasitBilgi item in formlar)
            {
                if (kosul != "") kosul += ", ";
                kosul += VeriErisim.DegerYap(item.saseNo);
            }
            return alanAd + " IN (" + kosul + ")";
        }

        public Sonuc OnayKodAta(TNSCollection formlar, int onayKod)
        {
            string sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.ONAYKODU] + " = " + VeriErisim.DegerYap(onayKod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", ListeKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SASENO], formlar));
            return this.KomutCalistir(sql, 0);
        }

        public Sonuc DetayArttir(string saseNo)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET SIRANO = SIRANO + 1";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SASENO], "=", saseNo));
            return KomutCalistir(sql, 0);
        }

        public Sonuc ResimGuncelle(string saseNo, string resimAd, byte[] resim)
        {
            string imgStr = "0x" + System.BitConverter.ToString(resim).Replace("-", string.Empty);
            string sql = "UPDATE " + TABLOAD + " SET RUHSATRESIM = " + imgStr + ", RUHSATRESIMAD = " + VeriErisim.DegerYap(resimAd);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SASENO], "=", saseNo));

            return KomutCalistir(sql, 0);
        }
    }
}

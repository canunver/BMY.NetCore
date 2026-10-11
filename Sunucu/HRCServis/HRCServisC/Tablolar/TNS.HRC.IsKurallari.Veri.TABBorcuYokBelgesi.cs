using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Borcu Yok Belge bilgisini tutan HRCBORCUYOKTUR veri tabanı ve 
    /// belge sorgulayan kullanıcı log bilgsinini tutan HRCGIBLOG veri tabanı 
    /// tabloları ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBorcuYokBelgesi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBorcuYokBelgesi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Borcu Yok Belgesi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCBORCUYOKTUR";

        /// <summary>
        /// Borcu Yok Belgesi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "CEVAPKOD", "VERGIKIMLIKNO", "NEDEN", "DURUM", "BASLANGICTARIH", "BITISTARIH", "SISTEMKAYITTARIH", "KAYDEDEN", "DEGISTIRMETARIH", "DEGISTIREN", "HATAMESAJI", "VERGIDAIRE", "UNVAN", "ADRES", "ILGILIMAKAM", "FORMDURUM", "BIRIM", "ILGILIBELGEYIL", "ILGILIBELGENO" };

        /// <summary>
        /// Borcu Yok Belgesi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// CEVAPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CEVAPKOD = 2,
            /// <summary>
            /// VERGIKIMLIKNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGIKIMLIKNO = 3,
            /// <summary>
            /// NEDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            NEDEN = 4,
            /// <summary>
            /// DURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DURUM = 5,
            /// <summary>
            /// BASLANGICTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BASLANGICTARIH = 6,
            /// <summary>
            /// BITISTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BITISTARIH = 7,
            /// <summary>
            /// SISTEMKAYITTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SISTEMKAYITTARIH = 8,
            /// <summary>
            /// KAYDEDEN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KAYDEDEN = 9,
            /// <summary>
            /// DEGISTIRMETARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGISTIRMETARIH = 10,
            /// <summary>
            /// DEGISTIREN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DEGISTIREN = 11,
            /// <summary>
            /// HATAMESAJI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            HATAMESAJI = 12,
            /// <summary>
            /// VERGIDAIRE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VERGIDAIRE = 13,
            /// <summary>
            /// UNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVAN = 14,
            /// <summary>
            /// ADRES alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADRES = 15,
            /// <summary>
            /// ILGILIMAKAM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ILGILIMAKAM = 16,
            /// <summary>
            /// FORMDURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FORMDURUM = 17,

            BIRIM = 18,
            ILGILIBELGEYIL = 19,
            ILGILIBELGENO = 20
        }

        /// <summary>
        /// Borcu Yok Belgesi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADLOG = "HRCGIBLOG";

        /// <summary>
        /// Borcu Yok Belgesi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARLOG = { "SORGULANAN", "ISLEMTARIH", "ISLEMYAPAN", "ISLEMACIKLAMA", "ISLEMYAPANKURUM" };

        /// <summary>
        /// Borcu Yok Belge Log tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarLOG
        {
            /// <summary>
            /// SORGULANAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SORGULANAN = 0,
            /// <summary>
            /// ISLEMTARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 1,
            /// <summary>
            /// ISLEMYAPAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 2,
            /// <summary>
            /// ISLEMACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMACIKLAMA = 3,
            /// <summary>
            /// ISLEMYAPANKURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPANKURUM = 4,
        }

        /// <summary>
        /// Borcu Yok Belge bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="belge">Listelenecek Borcu Yok Belge bilgisi için kıstas parametresi</param>
        /// <param name="yakinTarih">Eğer <c>true</c> verilirse tarihe göre tersten sıralı olarak listeleme yapar</param>
        /// <returns>
        /// Borcu Yok Belge bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(BorcuYokBelgesi belge, bool yakinTarih)
        {
            string kosul = string.Empty;
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            if (belge.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", belge.kod));
            else
            {
                if (belge.muhasebe > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", belge.muhasebe));
                if (belge.baslangicTarihi != null && !belge.baslangicTarihi.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SISTEMKAYITTARIH], ">=", belge.baslangicTarihi));
                if (belge.bitisTarihi != null && !belge.bitisTarihi.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SISTEMKAYITTARIH], "<=", belge.bitisTarihi));
                if (belge.cevapId != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.CEVAPKOD], "=", belge.cevapId));
                if (belge.talepNedeni > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.NEDEN], "=", belge.talepNedeni));
                if (belge.vergiKimlikNo != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VERGIKIMLIKNO], "=", belge.vergiKimlikNo));
                if (belge.durum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", belge.durum));
                if (belge.formDurum > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM], "=", belge.formDurum));
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            if (yakinTarih)
                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SISTEMKAYITTARIH] + " DESC";
            else
                sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SISTEMKAYITTARIH] + " ASC";

            //OrtakFonksiyonlar.HataStrYaz(sql);
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Borcu Yok Belgesi dilekçesinin verilip verilmediği bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="belge">Listeleme için kullanılacak kıstas parametresi</param>
        /// <returns>Borcu Yok Belgesi dilekçesi verilmişse 1 döner, verilmemiş ise 0 döner</returns>
        public int DilekceVerilmisMi(BorcuYokBelgesi belge)
        {
            int retValue = 0;
            string kosul = string.Empty;
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KOD];
            sql += " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", belge.muhasebe));
            // kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISTARIH], "=", belge.bitisTarihi));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VERGIKIMLIKNO], "=", belge.vergiKimlikNo));
            kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBorcuYokDurum.CEVAPLANMADI));
            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBorcuYokDurum.DILEKCESORGUHATASI));
            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBorcuYokDurum.GONDERILDI));
            kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)ENUMBorcuYokDurum.SIRADA));
            kosul = kosul + ")";

            sql += " WHERE " + kosul;
            try
            {
                retValue = veriErisim.TekKolonSQLInt(sql);
            }
            catch (Exception)
            {
                retValue = 1;
            }
            return retValue;
        }

        /// <summary>
        /// Borcu Yok Belge bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="belge">Eklenecek Borcu Yok Belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(BorcuYokBelgesi belge)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(belge.kod) + ",";
            sql += VeriErisim.DegerYap(belge.muhasebe) + ",";
            sql += VeriErisim.DegerYap(belge.cevapId) + ",";
            sql += VeriErisim.DegerYap(belge.vergiKimlikNo) + ",";
            sql += VeriErisim.DegerYap(belge.talepNedeni) + ",";
            sql += VeriErisim.DegerYap(belge.durum) + ",";
            sql += VeriErisim.DegerYap(belge.baslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(belge.bitisTarihi) + ",";
            sql += VeriErisim.DegerYap(belge.kayitTarihi) + ",";
            sql += VeriErisim.DegerYap(belge.kaydedenTcKimlik) + ",";
            sql += VeriErisim.DegerYap(belge.sonIslemTarihi) + ",";
            sql += VeriErisim.DegerYap(belge.islemYapan) + ",";
            sql += VeriErisim.DegerYap(belge.hataMesaji) + ",";
            sql += VeriErisim.DegerYap(belge.vergiDaire) + ",";
            sql += VeriErisim.DegerYap(belge.unvan) + ",";
            sql += VeriErisim.DegerYap(belge.adres) + ",";
            sql += VeriErisim.DegerYap(belge.ilgiliMakam) + ",";
            sql += VeriErisim.DegerYap(belge.formDurum) + ",";
            sql += VeriErisim.DegerYap(belge.birim) + ",";
            sql += VeriErisim.DegerYap(belge.ilgiliBelgeYil) + ",";
            sql += VeriErisim.DegerYap(belge.ilgiliBelgeNo) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Borcu Yok Belge bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="belge">Güncellenecek Borcu Yok Belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(BorcuYokBelgesi belge)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            if (!string.IsNullOrEmpty(belge.cevapId))
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEVAPKOD], belge.cevapId) + ",";
            if (belge.baslangicTarihi != null && !belge.baslangicTarihi.isNull)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], belge.baslangicTarihi) + ",";
            if (belge.bitisTarihi != null && !belge.bitisTarihi.isNull)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], belge.bitisTarihi) + ",";
            if (belge.durum > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], belge.durum) + ",";
            if (!string.IsNullOrEmpty(belge.hataMesaji))
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HATAMESAJI], belge.hataMesaji) + ",";
            if (!string.IsNullOrEmpty(belge.vergiKimlikNo))
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERGIKIMLIKNO], belge.vergiKimlikNo) + ",";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIREN], belge.islemYapan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIRMETARIH], belge.sonIslemTarihi);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", belge.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Borcu Yok Belgesine ait durum bilgisini veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="belge">Güncellenecek Borcu Yok Belge bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc BelgeDurumDegistir(BorcuYokBelgesi belge)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FORMDURUM], belge.formDurum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIREN], belge.islemYapan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGISTIRMETARIH], belge.sonIslemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", belge.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Borcu Yok Belge bilgisini sorgusuna ait log bilgisini veri tabanına eklemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sorgulanan">Sorgulama yapılan kişi/kurum TC Kimlik/Vergi numarası bilgisi</param>
        /// <param name="islemYapan">Sorgulama yapan kullanıcı kodu bilgisi</param>
        /// <param name="aciklama">Sorgulamaya ait açıklama</param>
        /// <param name="kurum">Sorgulamayı yapan kurum bilgisi</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc GIBLogKaydet(string sorgulanan, string islemYapan, string aciklama, string kurum)
        {
            string sql = "INSERT INTO " + TABLOADLOG + " (" + VeriErisim.TumAlanlar("", ALANLARLOG) + ") VALUES (";
            sql += VeriErisim.DegerYap(sorgulanan) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now.ToString())) + ",";
            sql += VeriErisim.DegerYap(islemYapan) + ",";
            sql += VeriErisim.DegerYap(aciklama) + ",";
            sql += VeriErisim.DegerYap(kurum.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BorcuYokBelgesi belge)
        {
            string sql = "";
            sql = " DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", belge.kod));
            return KomutCalistir(sql, 1);
        }
    }
}
using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Komisyon Üye bilgisini tutan HRCKOMISYONUYE veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKomisyonUye : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKomisyonUye(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Komisyon Üye bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCKOMISYONUYE";

        /// <summary>
        /// Komisyon Üye bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
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
            /// TIP alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TIP = 4,
            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 5,
            /// <summary>
            /// KOMISYONGOREV alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOMISYONGOREV = 6,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 7,
            /// <summary>
            /// UNVAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            UNVAN = 8,
            /// <summary>
            /// GOREV alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GOREV = 9,
            /// <summary>
            /// SICILNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SICILNO = 10,
            /// <summary>
            /// TELEFON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TELEFON = 11,
            /// <summary>
            /// CEPTELEFON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            CEPTELEFON = 12,
            /// <summary>
            /// GOREVLENDIRMETARIH alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GOREVLENDIRMETARIH = 13,
            /// <summary>
            /// GOREVLENDIRMENO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            GOREVLENDIRMENO = 14,

            TESLIMALAN = 15,
            TESLIMTARIH = 16,
        }

        /// <summary>
        /// Komisyon Üye tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TIP", "SIRANO", "KOMISYONGOREV", "AD", "UNVAN", "GOREV", "SICILNO", "TELEFON", "CEPTELEFON", "GOREVLENDIRMETARIH", "GOREVLENDIRMENO", "TESLIMALAN", "TESLIMTARIH" };

        /// <summary>
        /// Komisyon Üye bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek komisyon üye bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(KomisyonUye bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            int satir = 0;
            foreach (Uye f in bilgi.uyeler.objeler)
            {
                satir++;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
                sql += VeriErisim.DegerYap(bilgi.birim) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.tip) + ",";
                sql += VeriErisim.DegerYap(satir) + ",";
                sql += VeriErisim.DegerYap(f.komisyonGorev) + ",";
                sql += VeriErisim.DegerYap(f.ad) + ",";
                sql += VeriErisim.DegerYap(f.unvan) + ",";
                sql += VeriErisim.DegerYap(f.gorev) + ",";
                sql += VeriErisim.DegerYap(f.sicilNo) + ",";
                sql += VeriErisim.DegerYap(f.telefon) + ",";
                sql += VeriErisim.DegerYap(f.cepTelefon) + ",";
                sql += VeriErisim.DegerYap(bilgi.gorevlendirmeTarih) + ",";
                sql += VeriErisim.DegerYap(bilgi.gorevlendirmeNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimAlan) + ",";
                sql += VeriErisim.DegerYap(bilgi.teslimTarih) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Komisyon Üye bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="bilgi">Listeleme için kullanılacak komisyon üye bilgisi kıstas parametresi</param>
        /// <returns>
        /// Komisyon Üye bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(KomisyonUye bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));

            if (bilgi.uyeler.objeler.Count > 0)
            {
                Uye uye = new Uye();
                uye = (Uye)bilgi.uyeler.objeler[0];

                if (uye.ad != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + uye.ad + "%"));
                if (uye.komisyonGorev > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOMISYONGOREV], "=", uye.komisyonGorev));
            }
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO] + "," + ALANLAR[(int)ENUMAlanlar.YIL] + "," + ALANLAR[(int)ENUMAlanlar.SURECNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Komisyon Üye bilgisini veri tabanından silmek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Silinecek komisyon üye bilgisini tutan nesne</param>
        /// <returns>
        /// Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür
        /// </returns>
        public Sonuc Sil(KomisyonUye bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));

            return KomutCalistir(sql, 0);
        }
    }
}
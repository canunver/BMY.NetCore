using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// İdare bilgisini tutan HRCIDAREBILGI veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABIdareBilgi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIdareBilgi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// İdare bilgisi tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCIDAREBILGI";

        /// <summary>
        /// İdare bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,
            /// <summary>
            /// ADI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADI = 2,
            /// <summary>
            /// ADRES alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ADRES = 3,
            /// <summary>
            /// TELEFON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TELEFON = 4,
            /// <summary>
            /// FAKS alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FAKS = 5,
            /// <summary>
            /// EPOSTA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EPOSTA = 6,
            /// <summary>
            /// ANTET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ANTET = 7,
            /// <summary>
            /// EVRAKSERI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EVRAKSERI = 8,
            /// <summary>
            /// BUYUKSEHIRMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BUYUKSEHIRMI = 9,
            /// <summary>
            /// SICILNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SICILNO = 10,
            /// <summary>
            /// VNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VNO = 11,
            /// <summary>
            /// VD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            VD = 12,

            /// <summary>
            /// IGNACE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IGNACE = 13,
            /// <summary>
            /// IBAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IBAN = 14,
            VADI = 15,
        }

        /// <summary>
        /// İdare bilgisi tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "MUHASEBEKOD", "BIRIMKOD", "ADI", "ADRES", "TELEFON", "FAKS", "EPOSTA", "ANTET", "EVRAKSERI", "BUYUKSEHIRMI", "SICILNO", "VNO", "VD", "IGNACE", "IBAN", "VADI" };

        /// <summary>
        /// İdare bilgisini veri tabanına eklemek için
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek idare bilgisini tutan nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(IdareBilgi bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.adi) + ",";
            sql += VeriErisim.DegerYap(bilgi.adres) + ",";
            sql += VeriErisim.DegerYap(bilgi.telefon) + ",";
            sql += VeriErisim.DegerYap(bilgi.faks) + ",";
            sql += VeriErisim.DegerYap(bilgi.ePosta) + ",";
            sql += VeriErisim.DegerYap(bilgi.antet) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrakSeri) + ",";
            sql += VeriErisim.DegerYap(bilgi.argeMi*10 + bilgi.buyuksehirMi) + ",";
            sql += VeriErisim.DegerYap(bilgi.sicilNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.vNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.vd) + ",";
            sql += VeriErisim.DegerYap(bilgi.IGNaceKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bilgi.iban) + ",";
            sql += VeriErisim.DegerYap(bilgi.vergiAdi) + ")";

            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], bilgi.muhasebe) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], bilgi.birim) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.adi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], bilgi.adres) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TELEFON], bilgi.telefon) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKS], bilgi.faks) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EPOSTA], bilgi.ePosta) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANTET], bilgi.antet) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKSERI], bilgi.evrakSeri) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUYUKSEHIRMI], bilgi.argeMi * 10 + bilgi.buyuksehirMi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SICILNO], bilgi.sicilNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VNO], bilgi.vNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VD], bilgi.vd) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VADI], bilgi.vergiAdi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IGNACE], bilgi.IGNaceKod.Replace(".", "")) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IBAN], bilgi.iban);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// İdare bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <param name="kriter">Listeleme için kullanılacak idare bilgisi kıstas parametresi</param>
        /// <returns>
        /// İdare bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele(IdareBilgi kriter)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (kriter.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe));

            if (!string.IsNullOrWhiteSpace(kriter.birim))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kriter.birim));

            if (!string.IsNullOrWhiteSpace(kriter.vNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VNO], "=", kriter.vNo));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
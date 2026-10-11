using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Parametrik Yolluk Ortalama Fiyat bilgilerini tutan PARYOLLUKLARORTALAMAFIYAT veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABPARYOLLUKLARORTALAMAFIYAT : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPARYOLLUKLARORTALAMAFIYAT(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PARYOLLUKLARORTALAMAFIYAT";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar { 
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0, 
            
            /// <summary>
            /// Tip alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIP = 1, 
            
            /// <summary>
            /// Alt Tip alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTTIP = 2, 
            
            /// <summary>
            /// Alt Tip ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTTIPAD = 3, 
            
            /// <summary>
            /// Ortalama Fiyat Görev alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORTFIYATGOREV = 4, 
            
            /// <summary>
            /// Ortalama Fiyat Atama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORTFIYATATAMA = 5,
            
            /// <summary>
            /// Ortalama Fiyat Dosya alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORTFIYATDOSYA = 6,
            
            /// <summary>
            /// Ortalama Fiyat Gün alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORTFIYATTOPLAMGUN = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "TIP", "ALTTIP", "ALTTIPAD", "ORTFIYATGOREV", "ORTFIYATATAMA", "ORTFIYATDOSYA", "ORTFIYATTOPLAMGUN" };

        /// <summary>
        /// ParYollukOrtalamaFiyat yapısında duran bilgileri  veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parYollukOrtalamaFiyat">Bilgileri tutan ParYollukOrtalamaFiyat türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında nesne döndürür</returns>
        public Sonuc Kaydet(ParYollukOrtalamaFiyat parYollukOrtalamaFiyat)
        {
            if (parYollukOrtalamaFiyat.altTip == 0) { parYollukOrtalamaFiyat.altTip = SonAltTip(parYollukOrtalamaFiyat); }
            Sonuc sonuc = new Sonuc();
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(parYollukOrtalamaFiyat.yil) + ",";
            sql += VeriErisim.DegerYap((int)parYollukOrtalamaFiyat.tip) + ",";
            sql += VeriErisim.DegerYap(parYollukOrtalamaFiyat.altTip) + ",";
            sql += VeriErisim.DegerYap(parYollukOrtalamaFiyat.altTipAd) + ",";
            sql += VeriErisim.DegerYap(parYollukOrtalamaFiyat.ortFiyatGorev) + ",";
            sql += VeriErisim.DegerYap(parYollukOrtalamaFiyat.ortFiyatAtama) + ",";
            sql += VeriErisim.DegerYap(parYollukOrtalamaFiyat.ortFiyatDosya) + ",";
            sql += VeriErisim.DegerYap(parYollukOrtalamaFiyat.ortFiyatToplamGun) + ")";
            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], parYollukOrtalamaFiyat.yil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], (int)parYollukOrtalamaFiyat.tip) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTTIP], parYollukOrtalamaFiyat.altTip) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTTIPAD], parYollukOrtalamaFiyat.altTipAd) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORTFIYATGOREV], parYollukOrtalamaFiyat.ortFiyatGorev) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORTFIYATATAMA], parYollukOrtalamaFiyat.ortFiyatAtama) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORTFIYATDOSYA], parYollukOrtalamaFiyat.ortFiyatDosya) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORTFIYATTOPLAMGUN], parYollukOrtalamaFiyat.ortFiyatToplamGun);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (parYollukOrtalamaFiyat.yil)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)parYollukOrtalamaFiyat.tip));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTTIP], "=", parYollukOrtalamaFiyat.altTip));
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// ParYollukOrtalamaFiyat yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parYollukOrtalamaFiyat">Anahtar değişkenleri tutan ParYollukOrtalamaFiyat türünde nesne</param>
        /// <returns>Oluşturulan select komutunun çalıştırılması sonucunu, IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(ParYollukOrtalamaFiyat parYollukOrtalamaFiyat)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";
            if (parYollukOrtalamaFiyat.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", parYollukOrtalamaFiyat.yil));
            if (parYollukOrtalamaFiyat.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)parYollukOrtalamaFiyat.tip));

            if (parYollukOrtalamaFiyat.altTip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTTIP], "=", parYollukOrtalamaFiyat.altTip));
            if (parYollukOrtalamaFiyat.altTipAd != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ALTTIPAD], "LIKE", parYollukOrtalamaFiyat.altTipAd + "%"));
            if (parYollukOrtalamaFiyat.ortFiyatGorev > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORTFIYATGOREV], "=", parYollukOrtalamaFiyat.ortFiyatGorev));
            if (parYollukOrtalamaFiyat.ortFiyatAtama > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORTFIYATATAMA], "=", parYollukOrtalamaFiyat.ortFiyatAtama));
            if (parYollukOrtalamaFiyat.ortFiyatDosya > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORTFIYATDOSYA], "=", parYollukOrtalamaFiyat.ortFiyatDosya));
            if (parYollukOrtalamaFiyat.ortFiyatToplamGun > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORTFIYATTOPLAMGUN], "=", parYollukOrtalamaFiyat.ortFiyatToplamGun));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// ParYollukOrtalamaFiyat yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parYollukOrtalamaFiyat">Anahtar değişkenleri tutan ParYollukOrtalamaFiyat türünde nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ParYollukOrtalamaFiyat parYollukOrtalamaFiyat)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parYollukOrtalamaFiyat.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)parYollukOrtalamaFiyat.tip));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALTTIP], "=", parYollukOrtalamaFiyat.altTip));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// ParYollukOrtalamaFiyat yapısında duran anahtar alanları kullanarak, en yüksek alt tip kaydını veri tabanından getiren
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parYollukOrtalamaFiyat">Anahtar değişkenleri tutan ParYollukOrtalamaFiyat türünde nesne</param>
        /// <returns>Oluşturulan select komutunun çalıştırılması sonucunu, Int yapısında nesne döndürür</returns>
        private int SonAltTip(ParYollukOrtalamaFiyat parYollukOrtalamaFiyat)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.ALTTIP] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parYollukOrtalamaFiyat.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)parYollukOrtalamaFiyat.tip));

            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}

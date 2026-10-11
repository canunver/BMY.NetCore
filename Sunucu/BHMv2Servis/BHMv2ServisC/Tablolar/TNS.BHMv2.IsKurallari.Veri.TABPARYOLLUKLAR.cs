using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Parametrik Yolluk bilgilerini tutan PARYOLLUKLAR veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABPARYOLLUKLAR : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPARYOLLUKLAR(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PARYOLLUKLAR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// Tip alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIP = 1,
            
            /// <summary>
            /// Aşama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ASAMA = 2,
            
            /// <summary>
            /// Kurumsal Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 3,
            
            /// <summary>
            /// Fonksiyonel Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 4,
            
            /// <summary>
            /// Finansal Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 5,
            
            /// <summary>
            /// Ekonomik Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 6,
            
            /// <summary>
            /// Alt Faaliyet alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 7,
            
            /// <summary>
            /// Kurumsal Yapı Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMSALYAPIKOD = 8,
            
            /// <summary>
            /// Alt Tip alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTTIP = 9,
            
            /// <summary>
            /// Görev Sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVSAYISI = 10,
            
            /// <summary>
            /// Atama Sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ATAMASAYISI = 11,
            
            /// <summary>
            /// Dosya Sayısı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYASAYISI = 12,
            
            /// <summary>
            /// Toplam Gün Sayısı  alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TOPLAMGUNSAYISI = 13,
            
            /// <summary>
            /// Tutar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTAR = 14
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "TIP", "ASAMA", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "KURUMSALYAPIKOD", "ALTTIP", "GOREVSAYISI", "ATAMASAYISI", "DOSYASAYISI", "TOPLAMGUNSAYISI", "TUTAR" };

        /// <summary>
        /// ParYolluk dizi yapısında duran bilgileri  veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parYollukDizi">Bilgileri dizi şeklinde tutan ParYolluk türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında nesne döndürür</returns>
        public Sonuc Kaydet(ParYolluk[] parYollukDizi)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sonuc = Sil(parYollukDizi[0]);
            if (sonuc.islemSonuc)
            {
                foreach (ParYolluk parYolluk in parYollukDizi)
                {
                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(parYolluk.yil) + ",";
                    sql += VeriErisim.DegerYap((int)parYolluk.tip) + ",";
                    sql += VeriErisim.DegerYap((int)parYolluk.asama) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.kur.Replace(".", "")) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.fon.Replace(".", "")) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.fin.Replace(".", "")) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.eko.Replace(".", "")) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.peb.Replace(".", "")) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.kurumsalYapiKod.Replace(".", "")) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.altTip) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.gorevSayisi) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.atamaSayisi) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.dosyaSayisi) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.toplamGunSayisi) + ",";
                    sql += VeriErisim.DegerYap(parYolluk.tutar) + ")";

                    sonuc = KomutCalistir(sql, 1);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }

        /// <summary>
        /// ParYolluk yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parYolluk">Anahtar değişkenleri tutan ParYolluk türünde nesne</param>
        /// <returns>Oluşturulan select komutunun çalıştırılması sonucunu, IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(ParYolluk parYolluk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", parYolluk.yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)parYolluk.tip));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", (int)parYolluk.asama));
            if (parYolluk.kur != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", parYolluk.kur.Replace(".", "")));
            if (parYolluk.fon != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "=", parYolluk.fon.Replace(".", "")));
            if (parYolluk.fin != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "=", parYolluk.fin.Replace(".", "")));
            if (parYolluk.eko != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", parYolluk.eko.Replace(".", "")));
            if (parYolluk.peb != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", parYolluk.peb.Replace(".", "")));
            if (parYolluk.kurumsalYapiKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSALYAPIKOD], "=", parYolluk.kurumsalYapiKod.Replace(".", "")));

            sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// ParYolluk yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parYolluk">Anahtar değişkenleri tutan ParYolluk türünde nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ParYolluk parYolluk)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parYolluk.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)parYolluk.tip));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", (int)parYolluk.asama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", parYolluk.kur.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", parYolluk.fon.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", parYolluk.fin.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", parYolluk.eko.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", parYolluk.peb.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSALYAPIKOD], "=", parYolluk.kurumsalYapiKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }
    }
}

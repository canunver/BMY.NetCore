using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Parametrik Talep Taşınır bilgilerini tutan PARTALEPTASINIR veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABPARTALEPTASINIR : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPARTALEPTASINIR(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PARTALEPTASINIR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// Aşama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ASAMA = 1,

            /// <summary>
            /// Kurumsal Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,


            /// <summary>
            /// Fonksiyonel Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 3,


            /// <summary>
            /// Finansal Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 4,


            /// <summary>
            /// Ekonomik Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,

            /// <summary>
            /// Alt Faaliyet Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 6,

            /// <summary>
            /// Kurumsal Yapı Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMSALYAPIKOD = 7,


            /// <summary>
            /// Hesap Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPKOD = 8,


            /// <summary>
            /// Miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 9,


        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "ASAMA"
            , "KURKOD"
            , "FONKOD"
            , "FINKOD"
            , "EKOKOD"
            , "PEBKOD"
            , "KURUMSALYAPIKOD"
            , "HESAPKOD"
            , "MIKTAR"
        };

        /// <summary>
        /// ParTalepTasinir yapısında duran anahtar alanları kullanarak, ilgili kayıtları veri tabanından listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTalepTasinir">Anahtar değişkenleri tutan ParTalepTasinir türünde nesne</param>
        /// <returns>Oluşturulan select komutunun çalıştırılması sonucunu, IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(ParTalepTasinir parTalepTasinir)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (parTalepTasinir.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", parTalepTasinir.yil));


            if (parTalepTasinir.asama > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ASAMA], "=", (int)parTalepTasinir.asama));

            if (parTalepTasinir.kur != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", "%" + parTalepTasinir.kur.Replace(".", "") + "%"));

            if (parTalepTasinir.fon != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", "%" + parTalepTasinir.fon.Replace(".", "") + "%"));


            if (parTalepTasinir.fin != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", "%" + parTalepTasinir.fin.Replace(".", "") + "%"));


            if (parTalepTasinir.eko != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", "%" + parTalepTasinir.eko.Replace(".", "") + "%"));


            if (parTalepTasinir.peb != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PEBKOD], "LIKE", "%" + parTalepTasinir.peb.Replace(".", "") + "%"));


            if (parTalepTasinir.kurumsalYapiKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMSALYAPIKOD], "LIKE", "%" + parTalepTasinir.kurumsalYapiKod.Replace(".", "") + "%"));


            if (parTalepTasinir.tasHesapKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPKOD], "LIKE", "%" + parTalepTasinir.tasHesapKod.Replace(".", "") + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + " ," + ALANLAR[(int)ENUMAlanlar.ASAMA] + " ," + ALANLAR[(int)ENUMAlanlar.HESAPKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// ParTalepTasinir yapısında duran bilgileri  veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTalepTasinir">Bilgileri tutan ParTalepTasinir türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında nesne döndürür</returns>
        public Sonuc Kaydet(ParTalepTasinir parTalepTasinir)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(parTalepTasinir.yil) + ",";
            sql += VeriErisim.DegerYap((int)parTalepTasinir.asama) + ",";
            sql += VeriErisim.DegerYap(parTalepTasinir.kur.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTalepTasinir.fon.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTalepTasinir.fin.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTalepTasinir.eko.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTalepTasinir.peb.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTalepTasinir.kurumsalYapiKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTalepTasinir.tasHesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parTalepTasinir.miktar) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], parTalepTasinir.miktar);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parTalepTasinir.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", (int)parTalepTasinir.asama));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", parTalepTasinir.kur.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", parTalepTasinir.fon.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", parTalepTasinir.fin.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", parTalepTasinir.eko.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", parTalepTasinir.peb.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSALYAPIKOD], "=", parTalepTasinir.kurumsalYapiKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", parTalepTasinir.tasHesapKod.Replace(".", "")));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// ParTalepTasinir yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="parTalepTasinir">Anahtar değişkenleri tutan ParTalepTasinir türünde nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ParTalepTasinir parTalepTasinir)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", parTalepTasinir.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", (int)parTalepTasinir.asama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", parTalepTasinir.kur.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", parTalepTasinir.fon.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", parTalepTasinir.fin.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", parTalepTasinir.eko.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", parTalepTasinir.peb.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSALYAPIKOD], "=", parTalepTasinir.kurumsalYapiKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", parTalepTasinir.tasHesapKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }
    }
}

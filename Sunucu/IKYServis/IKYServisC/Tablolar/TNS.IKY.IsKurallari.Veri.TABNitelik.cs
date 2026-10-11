using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    /// <summary>
    /// Personel nitelik bilgilerini tutan PEBPERSONELNITELIK veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABNitelik : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABNitelik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "IKYPERSONELNITELIK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// TCKIMLIKNO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 0,

            /// <summary>
            /// GOREV alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GOREV = 1,

            /// <summary>
            /// BILGI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BILGI = 2,

            /// <summary>
            /// BECERI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BECERI = 3,
        }

        /// <summary>
        /// Personel nitelik bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "TCKIMLIKNO", "GOREV", "BILGI", "BECERI"};

        /// <summary>
        /// Personel nitelik bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(PersonelBilgiBirikimi personel)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(personel.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(personel.gorev) + ",";
            sql += VeriErisim.DegerYap(personel.bilgi) + ",";
            sql += VeriErisim.DegerYap(personel.beceri) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan personel nitelik bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(PersonelBilgiBirikimi personel)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GOREV], personel.gorev) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BILGI], personel.bilgi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BECERI], personel.beceri) + "";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", personel.tcKimlikNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen personel nitelik bilgilerini kıstas
        /// olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve personel nitelik bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan personel nitelik bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(PersonelBilgiBirikimi personel)
        {
            string sql = "SELECT IKYPERSONEL.TCKIMLIKNO, IKYPERSONEL.AD, IKYPERSONEL.SOYAD, ";
            sql += VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM IKYPERSONEL LEFT OUTER JOIN " + TABLOAD;
            sql += " ON IKYPERSONEL.TCKIMLIKNO = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO];

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(personel.tcKimlikNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("IKYPERSONEL", "TCKIMLIKNO", "=", personel.tcKimlikNo));

            if (!string.IsNullOrEmpty(personel.gorev))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREV], "LIKE", "%" + personel.gorev + "%"));
            if (!string.IsNullOrEmpty(personel.bilgi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BILGI], "LIKE", "%" + personel.bilgi + "%"));
            if (!string.IsNullOrEmpty(personel.beceri))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BECERI], "LIKE", "%" + personel.beceri + "%"));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen personel nitelik bilgilerini kullanarak,
        /// ilgili personel nitelik bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(PersonelBilgiBirikimi personel)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", personel.tcKimlikNo));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen personel T.C. kimlik numarasını kullanarak, ilgili personelin
        /// nitelik bilgilerinin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="tcKimlikNo">Personel T.C. kimlik numarası</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool PersonelVarMi(string tcKimlikNo)
        {
            return KayitVarmi(TABLOAD, tcKimlikNo);
        }

        /// <summary>
        /// Parametre olarak verilen personel T.C. kimlik numarasını kullanarak,
        /// ilgili personelin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="tcKimlikNo">Personel T.C. kimlik numarası</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool PersonelVarMiView(string tcKimlikNo)
        {
            return KayitVarmi("IKYPERSONEL", tcKimlikNo);
        }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen T.C. kimlik numaralı personel var mı kontrolü yapan yordam
        /// </summary>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="tcKimlikNo">Personel T.C. kimlik numarası</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        private bool KayitVarmi(string tablo, string tcKimlikNo)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] };
            string[] strDeger = { tcKimlikNo };
            return TNS.Veri.Sayac.KayitVarmi(veriErisim, tablo, strAlan, strDeger, new string[0], new int[0]);

        }

        //public bool PersonelKullanilmisMi(string tcKimlikNo)
        //{
        //    return KayitVarmi("PEBISGOREVSORUMLU", tcKimlikNo);
        //}
    }
}
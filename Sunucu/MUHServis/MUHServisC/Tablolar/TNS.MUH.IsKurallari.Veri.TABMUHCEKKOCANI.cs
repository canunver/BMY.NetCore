using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHCEKKOCANI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMUHCEKKOCANI : TABOrtak
    {
        /// <summary>
        /// TABMUHCEKKOCANI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMUHCEKKOCANI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "MUHCEKKOCANI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// HESAPKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPKOD = 0,

            /// <summary>
            /// CEKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEKNO = 1,

            /// <summary>
            /// KAYITTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYITTARIHI = 2,

            /// <summary>
            /// IPTALTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IPTALTARIHI = 3,

            /// <summary>
            /// IPTALTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IPTALNEDENI = 4
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "HESAPKOD"
            , "CEKNO"
            , "KAYITTARIHI"
            , "IPTALTARIHI"
            , "IPTALNEDENI"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, CekKocani yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(CekKocani form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], "=", form.hesapKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.CEKNO], "=", form.cekNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, CekKocani yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(CekKocani form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.hesapKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.cekNo);
            sql += "," + VeriErisim.DegerYap(form.kayitTarihi);
            sql += "," + VeriErisim.DegerYap(form.iptalTarihi);
            sql += "," + VeriErisim.DegerYap(form.iptalNedeni);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, CekKocani yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(CekKocani form, TNSDateTime baslaTarih, TNSDateTime bitisTarih)
        {
            string alan1 = "CASE WHEN O.HESAPKOD IS NULL THEN C.HESAPKOD ELSE O.HESAPKOD END";
            string alan2 = "CASE WHEN O.CEKNO IS NULL THEN C.CEKNO ELSE O.CEKNO END";
            string kosul = "";
            if (!string.IsNullOrWhiteSpace(form.hesapKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", alan1, "LIKE", form.hesapKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(form.cekNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", alan2, "LIKE", form.cekNo + "%"));
            if (!TNSDateTime.IsNull(baslaTarih))
                kosul = VeriErisim.KosulEkle(kosul, "AND", $"(O.GUN>={VeriErisim.DegerYap(baslaTarih)} OR C.IPTALTARIHI>={VeriErisim.DegerYap(baslaTarih)})");
            if (!TNSDateTime.IsNull(bitisTarih))
                kosul = VeriErisim.KosulEkle(kosul, "AND", $"(O.GUN<={VeriErisim.DegerYap(bitisTarih)} OR C.IPTALTARIHI<={VeriErisim.DegerYap(bitisTarih)})");

            string sql = "select " + alan1 + " HESAPKOD, " + alan2 + " CEKNO, ";
            sql += "O.GUN, O.MIFBELGENO, O.TC_VNO, O.ILGILI, O.TUTAR, O.ODEMEEMRINO, ISLEMTARIH, C.IPTALTARIHI, C.IPTALNEDENI ";
            sql += "FROM (SELECT D.MUHHESAPKOD HESAPKOD, D.CEKNO, D.GUN, D.MIFBELGENO, D.TC_VNO, D.ILGILI, D.TUTAR, D.ODEMEEMRINO, FO.ISLEMTARIH ";
            sql += "FROM MUHBANKAODEMEDETAY D ";
            sql += "LEFT OUTER JOIN MUHMIFFORM FA ON FA.YIL=D.MIFYIL AND FA.BELGENO=D.MIFBELGENO ";
            sql += "LEFT OUTER JOIN MUHMIF FAD ON FAD.YIL=D.MIFYIL AND FAD.BELGENO=D.MIFBELGENO AND FAD.SIRA=D.MIFSIRA ";
            sql += "LEFT OUTER JOIN MUHMIFFORM FO ON FO.YIL=D.MIFYIL AND FO.BELGENO=D.ODEMEEMRINO ";
            sql += "WHERE D.CEKNO IS NOT NULL) O ";
            sql += "FULL OUTER JOIN MUHCEKKOCANI C ON ";
            sql += "O.HESAPKOD=C.HESAPKOD AND C.CEKNO=O.CEKNO ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY 1,2 desc";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}

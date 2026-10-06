using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// KYMEKLIDOSYALAR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKYMEKLIDOSYALAR : TABOrtak
    {
        /// <summary>
        /// TABKYMEKLIDOSYALAR sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKYMEKLIDOSYALAR(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "KYMEKLIDOSYA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// MODUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODUL = 0,

            /// <summary>
            /// FORM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORM = 1,

            /// <summary>
            /// DOSYANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYANO = 2,

            /// <summary>
            /// DOSYAADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYAADI = 3,

            /// <summary>
            /// BUYUKLUK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUYUKLUK = 4,

            /// <summary>
            /// YUKLEMETARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YUKLEMETARIHI = 5,

            /// <summary>
            /// YUKLEYEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YUKLEYEN = 6,
            ONAYLAYAN = 7,
            ONAYZAMAN = 8,
            ACIKLAMA = 9,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "MODUL"
            , "FORM"
            , "DOSYANO"
            , "DOSYAADI"
            , "BUYUKLUK"
            , "YUKLEMETARIHI"
            , "YUKLEYEN"
            , "ONAYLAYAN"
            , "ONAYZAMAN"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADPARCA = "KYMEKLIDOSYAPARCALARI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarPARCA : int
        {
            /// <summary>
            /// MODUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODUL = 0,

            /// <summary>
            /// FORM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORM = 1,

            /// <summary>
            /// DOSYANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYANO = 2,

            /// <summary>
            /// PARCANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PARCANO = 3,

            /// <summary>
            /// PARCA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PARCA = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARPARCA = {
            "MODUL"
            , "FORM"
            , "DOSYANO"
            , "PARCANO"
            , "PARCA"
        };

        public int DosyaNoBul(EkliDosyalar form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.DOSYANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MODUL], "=", form.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FORM], "=", form.form));
            int d = (int)this.veriErisim.TekKolonSQLDbl(sql);
            return d + 1;
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, EkliDosyalar yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(EkliDosyalar form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MODUL], "=", form.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FORM], "=", form.form));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", form.dosyaNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EkliDosyalar yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(EkliDosyalar form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.modul);
            sql += "," + VeriErisim.DegerYap(form.form);
            sql += "," + VeriErisim.DegerYap(form.dosyaNo);
            sql += "," + VeriErisim.DegerYap(form.dosyaAdi);
            sql += "," + VeriErisim.DegerYap(form.buyukluk);
            sql += "," + VeriErisim.DegerYap(form.yuklemeTarihi);
            sql += "," + VeriErisim.DegerYap(form.yukleyen);
            sql += ",NULL,NULL";
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, EkliDosyalar yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(EkliDosyalar form, bool like)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MODUL], "=", form.modul));

            string[] formlar = form.form.Split(';');
            string kosul = "";
            foreach (string formNoDis in formlar)
            {
                string formNo = formNoDis;
                if (like) formNo = formNo.Replace("_", "[_]");
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FORM], (like ? "LIKE" : "="), formNo + (like ? "%" : "")));
            }

            sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");

            if (form.dosyaNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", form.dosyaNo));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, EkliDosyalar yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc SilParca(EkliDosyalar form)
        {
            string sql = "DELETE FROM " + TABLOADPARCA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPARCA[(int)ENUMAlanlarPARCA.MODUL], "=", form.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPARCA[(int)ENUMAlanlarPARCA.FORM], "=", form.form));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPARCA[(int)ENUMAlanlarPARCA.DOSYANO], "=", form.dosyaNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, ekliDosyalarParcalari dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EkliDosyalar yapısında nesne</param>
        /// <param name="satirNo">ekliDosyalarParcalari dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleParca(EkliDosyalar form, int parcaNo, byte[] parca)
        {
            string sql = "INSERT INTO " + TABLOADPARCA + "(" + VeriErisim.TumAlanlar("", ALANLARPARCA) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.modul);
            sql += "," + VeriErisim.DegerYap(form.form);
            sql += "," + VeriErisim.DegerYap(form.dosyaNo);
            sql += "," + VeriErisim.DegerYap(parcaNo);
            sql += "," + ParcaAl(parca, -1, -1);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        //public Sonuc EkleParca(EkliDosyalar form, int parcaNo, byte[] parca, int basla, int uzunluk)
        //{
        //    string sql = "INSERT INTO " + TABLOADPARCA + "(" + VeriErisim.TumAlanlar("", ALANLARPARCA) + ") VALUES";
        //    sql += "(" + VeriErisim.DegerYap(form.modul);
        //    sql += "," + VeriErisim.DegerYap(form.form);
        //    sql += "," + VeriErisim.DegerYap(form.dosyaNo);
        //    sql += "," + VeriErisim.DegerYap(parcaNo);
        //    sql += "," + VeriErisim.DegerYap(ParcaAl(parca, basla, uzunluk));
        //    sql += ")";
        //    return KomutCalistir(sql, 1);
        //}

        private string ParcaAl(byte[] parca, int basla, int uzunluk)
        {
            if (basla == -1)
                return "0x" + System.BitConverter.ToString(parca).Replace("-", string.Empty);
            else
                return "0x" + System.BitConverter.ToString(parca, basla, uzunluk).Replace("-", string.Empty);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, EkliDosyalar yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleParca(EkliDosyalar form)
        {
            string sql = "SELECT " + ALANLARPARCA[(int)ENUMAlanlarPARCA.PARCA] + ", D.DOSYAADI, D.BUYUKLUK FROM " + TABLOADPARCA + " P INNER JOIN " + TABLOAD + " D ON D.MODUL=P.MODUL AND D.FORM=P.FORM AND D.DOSYANO=P.DOSYANO";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("D", ALANLARPARCA[(int)ENUMAlanlarPARCA.MODUL], "=", form.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARPARCA[(int)ENUMAlanlarPARCA.FORM], "=", form.form));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARPARCA[(int)ENUMAlanlarPARCA.DOSYANO], "=", form.dosyaNo));

            sql += " ORDER BY " + ALANLARPARCA[(int)ENUMAlanlarPARCA.PARCANO];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc OnayIslem(EkliDosyalar form, string onaylayan, TNSDateTime islemZaman)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYLAYAN], onaylayan);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYZAMAN], islemZaman);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MODUL], "=", form.modul));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FORM], "=", form.form));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYANO], "=", form.dosyaNo));
            return KomutCalistir(sql, 0);
        }
    }
}

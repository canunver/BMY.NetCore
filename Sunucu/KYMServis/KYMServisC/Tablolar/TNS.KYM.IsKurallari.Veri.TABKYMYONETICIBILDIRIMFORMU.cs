using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// KYMYONETICIBILDIRIMFORMU veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKYMYONETICIBILDIRIMFORMU : TABOrtak
    {
        /// <summary>
        /// TABKYMYONETICIBILDIRIMFORMU sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKYMYONETICIBILDIRIMFORMU(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "KYMYONETICIBILDIRIMFORMU";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SAYMANLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIK = 0,

            /// <summary>
            /// BIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIM = 1,

            /// <summary>
            /// TALEPNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TALEPNO = 2,

            /// <summary>
            /// TALEPTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TALEPTARIHI = 3,

            /// <summary>
            /// YONETICITCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YONETICITCKIMLIKNO = 4,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 5,

            /// <summary>
            /// ISLEMYAPAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 6,

            /// <summary>
            /// ISLEMONAYLAYAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMONAYLAYAN = 7,

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SAYMANLIK"
            , "BIRIM"
            , "TALEPNO"
            , "TALEPTARIHI"
            , "YONETICITCKIMLIKNO"
            , "DURUM"
            , "ISLEMYAPAN"
            , "ISLEMONAYLAYAN"
                                  };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, YoneticiBildirimFormu yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Delete(YoneticiBildirimFormu form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPNO], "=", form.talepNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, YoneticiBildirimFormu yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Insert(YoneticiBildirimFormu form, string kullaniciKodu)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.saymanlik);
            sql += "," + VeriErisim.DegerYap(form.birim);
            sql += "," + VeriErisim.DegerYap(form.talepNo);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap(form.yoneticiTCKimlikNo);
            sql += "," + VeriErisim.DegerYap(1);
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, YoneticiBildirimFormu yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader List(YoneticiBildirimFormu form)
        {
            string kosul = string.Empty;
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", PERPERSONEL.AD, PERPERSONEL.SOYAD FROM " + TABLOAD + " LEFT OUTER JOIN PERPERSONEL ON PERPERSONEL.TCKIMLIKNO = KYMYONETICIBILDIRIMFORMU.YONETICITCKIMLIKNO";
            int anahtarSay = 0;
            if (form.saymanlik > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
                anahtarSay++;
            }
            if (!string.IsNullOrWhiteSpace(form.birim))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
                anahtarSay++;
            }
            if (form.talepNo > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TALEPNO], "=", form.talepNo));
                anahtarSay++;
            }
            else if (!string.IsNullOrWhiteSpace(form.talepNolar))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TALEPNO] + " IN  (" + form.talepNolar + ")");
                anahtarSay++;
            }
            if (anahtarSay < 3)
            {
                if ((int)form.durum > 0)
                {
                    if (form.durum == ENUMYoneticiBildirimFormuDurum.YeniGonderildi)
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<=", (int)ENUMYoneticiBildirimFormuDurum.Gonderildi));
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)form.durum));
                }
            }
            if (kosul != "")
                sql += " WHERE " + kosul;
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DurumGuncelle(YoneticiBildirimFormu form, string kullaniciKodu)
        {
            string kosul = string.Empty;
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)form.durum);
            if (form.durum == ENUMYoneticiBildirimFormuDurum.IptalEdildi || form.durum == ENUMYoneticiBildirimFormuDurum.Reddedildi || form.durum == ENUMYoneticiBildirimFormuDurum.Onaylandi)
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMONAYLAYAN], kullaniciKodu);
            else
            {
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], kullaniciKodu);
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TALEPTARIHI], new TNSDateTime(DateTime.Now));
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPNO], "=", form.talepNo));

            return KomutCalistir(sql, 1);
        }

        public int MaxTalepNoBul(YoneticiBildirimFormu form)
        {
            string kosul = string.Empty;
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.TALEPNO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));

            return (int)this.veriErisim.TekKolonSQLDbl(sql);
        }
    }
}

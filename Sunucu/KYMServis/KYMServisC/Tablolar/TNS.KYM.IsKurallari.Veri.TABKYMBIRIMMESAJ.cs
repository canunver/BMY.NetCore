using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// KYMBIRIMMESAJ veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKYMBIRIMMESAJ : TABOrtak
    {
        /// <summary>
        /// TABKYMBIRIMMESAJ sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKYMBIRIMMESAJ(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// 
        /// 
        /// CREATE TABLE KYMBIRIMMESAJ
        /// (SAYMANLIKKOD INT
        /// , BIRIMKOD INT
        /// , KURUMKOD VARCHAR(20)
        /// , SIRANO INT
        /// , MESAJ VARCHAR(2000)
        /// , KAYDEDEN VARCHAR(200)
        /// , KAYITTARIHI DATETIME
        /// , KARSILAYAN VARCHAR(200)
        /// , KARSILAMATARIHI VARCHAR(200)
        /// , SAYFA VARCHAR(200)
        /// , DURUM INT
        /// , PRIMARY KEY (SAYMANLIKKOD, BIRIMKOD, KURUMKOD, SIRANO))
        /// 
        /// </summary>
        public static string TABLOAD = "KYMBIRIMMESAJ";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SAYMANLIKKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIKKOD = 0,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,

            /// <summary>
            /// KURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 2,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,

            /// <summary>
            /// MESAJ alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MESAJ = 4,

            /// <summary>
            /// KAYDEDEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYDEDEN = 5,

            /// <summary>
            /// KAYITTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYITTARIHI = 6,

            /// <summary>
            /// KARSILAYAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KARSILAYAN = 7,

            /// <summary>
            /// KARSILAMATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KARSILAMATARIHI = 8,

            /// <summary>
            /// SAYFA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYFA = 9,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 10,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SAYMANLIKKOD"
            , "BIRIMKOD"
            , "KURUMKOD"
            , "SIRANO"
            , "MESAJ"
            , "KAYDEDEN"
            , "KAYITTARIHI"
            , "KARSILAYAN"
            , "KARSILAMATARIHI"
            , "SAYFA"
            , "DURUM"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BirimMesajAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(BirimMesaj form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], "=", form.saymanlikKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 0);
        }

        public int SiraNoBul(BirimMesaj form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], "=", form.saymanlikKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurKod.Replace(".", "")));
            int sonuc = (int)veriErisim.TekKolonSQLDbl(sql);
            return sonuc + 1;
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BirimMesajAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(BirimMesaj form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MESAJ], form.mesaj);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYDEDEN], form.kaydeden);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], form.kayitTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)form.durum);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYFA], form.sayfa);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], "=", form.saymanlikKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, birimMesaj dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BirimMesajAna yapısında nesne</param>
        /// <param name="satirNo">birimMesaj dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(BirimMesaj form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.saymanlikKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.kurKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.mesaj);
            sql += "," + VeriErisim.DegerYap(form.kaydeden);
            sql += "," + VeriErisim.DegerYap(form.kayitTarihi);
            sql += "," + VeriErisim.DegerYap(form.karsilayan);
            sql += "," + VeriErisim.DegerYap(form.karsilamaTarihi);
            sql += "," + VeriErisim.DegerYap(form.sayfa);
            sql += "," + VeriErisim.DegerYap((int)form.durum);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        private string SorguOl(BirimMesaj form, bool say)
        {
            string alanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", MUHASEBEAD, BIR.BIRIMAD";
            if (say)
                alanlar = "COUNT(*)";
            string sql = "SELECT " + alanlar + "  FROM ";
            sql += TABLOAD + " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE MUH ON MUH.MUHASEBEKOD=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD];
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM BIR ON BIR.MUHASEBEKOD=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD] + " AND BIR.BIRIMKOD=" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "LIKE", form.kurKod.Replace(".", "") + "%"));

            if (form.saymanlikKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], "=", form.saymanlikKod));
            if (!string.IsNullOrWhiteSpace(form.birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            if (form.kayitTarihKucuk != null && !form.kayitTarihKucuk.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], ">=", form.kayitTarihKucuk));
            if (form.kayitTarihBuyuk != null && !form.kayitTarihBuyuk.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], "<=", form.kayitTarihBuyuk));
            if (!string.IsNullOrWhiteSpace(form.kaydeden))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KAYDEDEN], "LIKE", form.kaydeden + "%"));
            if (!string.IsNullOrWhiteSpace(form.sayfa))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYFA], "=", form.sayfa));
            if (!string.IsNullOrWhiteSpace(form.durumlar))
                sql += VeriErisim.IN_KosulOl("AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DURUM], form.durumlar, false, false);
            return sql;
        }
        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BirimMesajAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(BirimMesaj form)
        {
            return veriErisim.DataReaderDondur(SorguOl(form, false));
        }

        public Sonuc DurumAta(BirimMesaj form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)form.durum);
            if (form.durum == BirimMesaj.MesajDurum.Yapildi)
            {
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARSILAYAN], form.karsilayan);
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARSILAMATARIHI], form.karsilamaTarihi);
            }
            else
            {
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYDEDEN], form.karsilayan);
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITTARIHI], form.karsilamaTarihi);
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], "=", form.saymanlikKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 0);
        }

        public int MesajSayisi(BirimMesaj form)
        {
            return (int)veriErisim.TekKolonSQLDbl(SorguOl(form, true));
        }
    }
}

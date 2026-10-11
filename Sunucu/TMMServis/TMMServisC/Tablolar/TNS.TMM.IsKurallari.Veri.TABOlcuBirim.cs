using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Ölçü birimi bilgilerini tutan TASOLCUBIRIM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABOlcuBirim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOlcuBirim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASOLCUBIRIM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Ölçü birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Ölçü birim adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "AD" };

        /// <summary>
        /// Ölçü birimi bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="olcuBirim">Ölçü birimi bilgilerini tutan, Ölçü birim türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(OlcuBirim olcuBirim)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(olcuBirim.kod) + ",";
            sql += VeriErisim.DegerYap(olcuBirim.ad) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Ölçü birimi bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="olcuBirim">Ölçü birimi bilgilerini tutan, Ölçü birim türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(OlcuBirim olcuBirim)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], olcuBirim.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", olcuBirim.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Ölçü Birim bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Ölçü birim bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="olcuBirim">Ölçü birimi bilgilerini tutan, Ölçü birim türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ölçü birim bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(OlcuBirim olcuBirim)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (olcuBirim.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", olcuBirim.kod));

            if (olcuBirim.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + olcuBirim.ad + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            //veriErisim.sureAsimi = 60000;

            //sql = "alter table TASODAt drop constraint FK_TASODA_TASHARCAMABIRIM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASGECICIDETAY drop constraint FK_TASGECICIDETAY_TASGECICIFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASGECICIFORM drop constraint FK_TASGECICIDETAY_TASGECICIFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASIMZA drop constraint FK_TASIMZA_TASAMBAR";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASISLEMDETAY drop constraint FK_TASISLEMDETAY_TASISLEMFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASISLEMFORM drop constraint FK_TASISLEMDETAY_TASISLEMFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASZIMMETDETAY drop constraint FK_TASZIMMETDETAY_TASZIMMETFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASZIMMETFORM drop constraint FK_TASZIMMETDETAY_TASZIMMETFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASISLEMFORM drop constraint FK_TASISLEMOZELLIK_TASISLEMFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASISLEMOZELLIK drop constraint FK_TASISLEMOZELLIK_TASISLEMFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASISTEKDETAY drop constraint FK_TASISTEKDETAY_TASISTEKFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASISTEKFORM drop constraint FK_TASISTEKDETAY_TASISTEKFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASKAYITTANDUSMEFORM drop constraint FK__TASKAYITTANDUSME__7CA47C3F";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASKAYITTANDUSMEDETAY drop constraint FK__TASKAYITTANDUSME__7CA47C3F";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASSAYIMFORM drop constraint FK_SAYIMDETAY_SAYIMFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASSAYIMDETAY drop constraint FK_SAYIMDETAY_SAYIMFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASTALEPKARSILAMADETAY drop constraint FK_TASTALEPKARSILAMADETAY_TASTALEPKARSILAMA";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASTALEPKARSILAMA drop constraint FK_TASTALEPKARSILAMADETAY_TASTALEPKARSILAMA";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASZIMMETFORM drop constraint FK_TASZIMMETDETAY_TASZIMMETFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASZIMMETDETAY drop constraint FK_TASZIMMETDETAY_TASZIMMETFORM";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASAMBAR drop constraint FK_TASISLEMFORM_TASAMBAR";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "alter table TASAMBAR drop constraint FK_KARSILAYANTASISTEKFORM_TASAMBAR";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);



            //sql = "UPDATE TASACIKPAZARDETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASACIKPAZARFORM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASACIKPAZARISTEK SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASAKTARIM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            ////sql = "UPDATE TASAMBAR SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            ////veriErisim.hata = "";
            ////KomutCalistir(sql, 0);
            //sql = "UPDATE TASAMORTISMAN SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASAMORTISMANDEGERLEME SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASAMORTISMANFORM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASGECICIDETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASGECICIFORM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASGECICIIHRAC SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASGIYECEKTECHIZAT SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASIMZA SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMDETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMDOSYA SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMFORM SET GHARCAMAKOD = '02020000965' WHERE GHARCAMAKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMFORM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMFORMTARIHCE SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMMIF SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMONAYYETKI SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMOZELLIK SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISLEMSUREC SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISTEKDETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASISTEKFORM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASKAYITTANDUSMEDETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASKAYITTANDUSMEFORM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            ////xxsql = "UPDATE TASODA SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            ////xxveriErisim.hata = "";
            ////xxKomutCalistir(sql, 0);
            ////xxsql = "UPDATE TASPERSONEL SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            ////xxveriErisim.hata = "";
            ////xxKomutCalistir(sql, 0);
            //sql = "UPDATE TASSAYIMDETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASSAYIMFORM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASSAYIMRFID SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASSICILNODEGERARTIS SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASSICILNOHAREKET SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASSICILNONUMARA SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASTALEPKARSILAMA SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASTALEPKARSILAMADETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASTUKETIMURETIM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASTUKETIMURETIMDETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASZIMMETDETAY SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASZIMMETFORM SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASZIMMETFORMTARIHCE SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);
            //sql = "UPDATE TASPERSONEL SET HARCAMABIRIMKOD = '02020000965' WHERE HARCAMABIRIMKOD = '01010000965'";
            //veriErisim.hata = "";
            //KomutCalistir(sql, 0);

            //veriErisim.sureAsimi = 60000;
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Ölçü Birim bilgisini kullanarak
        /// Ölçü birim bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="olcuBirim">Ölçü birimi bilgilerini tutan, Ölçü birim türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(OlcuBirim olcuBirim)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", olcuBirim.kod));

            return KomutCalistir(sql, 1);
        }
    }
}
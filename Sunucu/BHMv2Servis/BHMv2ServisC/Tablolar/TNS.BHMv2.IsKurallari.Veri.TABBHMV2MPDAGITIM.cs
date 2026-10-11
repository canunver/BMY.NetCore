using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMV2MPDAGITIM veri tabaný tablosu ile ilgili, listeleme ve yazma iþlemleri tutan nesne
    /// </summary>
    public class TABBHMV2MPDAGITIM : TABOrtak
    {
        /// <summary>
        /// TABBHMV2MPDAGITIM sýnýf için oluþturucu (constructer) fonksiyonu, veri eriþiminde kullanacaðý nesneyi parametre olarak alýr
        /// </summary>
        /// <param name="veriErisim">Veri tabaný eriþiminde kullanýlacak nesne</param>
        public TABBHMV2MPDAGITIM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adýný tutan sabit deðiþken
        /// </summary>
        public static string TABLOAD = "BHMV2MPDAGITIM";

        /// <summary>
        /// Tablo alanlarýnýn sýrasýný tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// ID alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            ID = 0,

            /// <summary>
            /// YIL alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            YIL = 1,

            /// <summary>
            /// KUR alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            KUR = 2,

            /// <summary>
            /// FON alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            FON = 3,

            /// <summary>
            /// FIN alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            FIN = 4,

            /// <summary>
            /// EKO alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            EKO = 5,

            /// <summary>
            /// PEB alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            PEB = 6,

            /// <summary>
            /// ACIKLAMA alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            ACIKLAMA = 7,

            /// <summary>
            /// MIKTAR alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            MIKTAR = 8,
        }

        /// <summary>
        /// Tablo alanlarýnýn isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "ID"
            , "YIL"
            , "KUR"
            , "FON"
            , "FIN"
            , "EKO"
            , "PEB"
            , "ACIKLAMA"
            , "MIKTAR"
        };

        /// <summary>
        /// Tablo adýný tutan sabit deðiþken
        /// </summary>
        public static string TABLOADDETAY = "BHMV2MPDAGITIMDETAY";

        /// <summary>
        /// Tablo alanlarýnýn sýrasýný tutan enumeration
        /// </summary>
        public enum ENUMAlanlarDETAY : int
        {

            /// <summary>
            /// ID alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            ID = 0,

            /// <summary>
            /// BIRIMKOD alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            BIRIMKOD = 1,

            /// <summary>
            /// MIKTAR alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            MIKTAR = 2,

            /// <summary>
            /// BUYILDEGER alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            BUYILDEGER = 3,

            /// <summary>
            /// ONCYILDEGER alanýnýn select cümleceði sonrasýnda, DataReaderdaki sýra numarasý (indisi)
            /// </summary>
            ONCYILDEGER = 4,
        }

        /// <summary>
        /// Tablo alanlarýnýn isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARDETAY = {
            "ID"
            , "BIRIMKOD"
            , "MIKTAR"
            , "BUYILDEGER"
            , "ONCYILDEGER"
        };


        /// <summary>
        /// Form yapýsýnda duran anahtar alanlarý kullanarak, ilgili kaydý veri tabanýndan silen
        /// sql cümlesini oluþturup, çalýþtýran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar deðiþkenleri tutan, MPDagitim yapýsýnda nesne</param>
        /// <returns>Sil komutunun çalýþtýrýlmasý sonucu Sonuc yapýsýnda döndürür</returns>
        public Sonuc Sil(MPDagitimAna form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ID], "=", form.id));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapýsýnda duran alanlarý veri tabanýna ekleyen
        /// sql cümlesini oluþturup, çalýþtýran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayýt bilgisini tutan, MPDagitim yapýsýnda nesne</param>
        /// <returns>Ekleme komutunun çalýþtýrýlmasý sonucu Sonuc yapýsýnda döndürür</returns>
        public Sonuc Ekle(MPDagitimAna form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + ")  OUTPUT Inserted.ID VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.kur.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.fon.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.fin.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.eko.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.peb.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.miktar);
            sql += ")";

            int eklenenDeger = veriErisim.TekKolonSQLInt(sql);
            Sonuc s = new Sonuc();
            s.anahtar = eklenenDeger.ToString();

            if (!string.IsNullOrWhiteSpace(veriErisim.hata))
                s.DegerAta(veriErisim.hata);

            return s;
        }

        /// <summary>
        /// Form yapýsýnda duran alanlarý veri tabanýna ekleyen
        /// sql cümlesini oluþturup, çalýþtýran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayýt bilgisini tutan, MPDagitim yapýsýnda nesne</param>
        /// <returns>Ekleme komutunun çalýþtýrýlmasý sonucu Sonuc yapýsýnda döndürür</returns>
        public Sonuc Guncelle(MPDagitimAna form)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KUR], form.kur.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FON], form.fon.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIN], form.fin.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKO], form.eko.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PEB], form.peb.Replace(".", ""));
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], form.aciklama);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], form.miktar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ID], "=", form.id));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapýsýnda duran alanlarý kýstas olarak kullanarak,
        /// select sql cümlesini oluþturup, çalýþtýran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayýtlara ait kýstas bilgisini tutan, MPDagitim yapýsýnda nesne</param>
        /// <returns>Select komutunun çalýþtýrýlmasý sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(MPDagitimAna form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("D", ALANLAR) + ", K.KURAD, F.FONAD, I.FINAD, E.EKOAD, P.PEBAD FROM " + TABLOAD + " D";

            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSKUR    K ON K.YIL= D.YIL AND K.KURKOD = D.KUR ";
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSFON    F ON F.YIL= D.YIL AND F.FONKOD = D.FON ";
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSFIN    I ON I.YIL= D.YIL AND I.FINKOD = D.FIN ";
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSEKO    E ON E.YIL= D.YIL AND E.EKOKOD = D.EKO ";
            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD P ON P.YIL= D.YIL AND P.PEBKOD = D.PEB ";
            if (form.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("D", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("D", ALANLAR[(int)ENUMAlanlar.ID], "=", form.id));
            return veriErisim.DataReaderDondur(sql);
        }



        /// <summary>
        /// Form yapýsýnda duran anahtar alanlarý kullanarak, ilgili kaydý veri tabanýndan silen
        /// sql cümlesini oluþturup, çalýþtýran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar deðiþkenleri tutan, MPDagitim yapýsýnda nesne</param>
        /// <returns>Sil komutunun çalýþtýrýlmasý sonucu Sonuc yapýsýnda döndürür</returns>
        public Sonuc SilDetay(int id, string birimKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.ID], "=", id));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDETAY.BIRIMKOD], "=", birimKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapýsýnda duran alanlarý veri tabanýna ekleyen
        /// sql cümlesini oluþturup, çalýþtýran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayýt bilgisini tutan, MPDagitim yapýsýnda nesne</param>
        /// <returns>Ekleme komutunun çalýþtýrýlmasý sonucu Sonuc yapýsýnda döndürür</returns>
        public Sonuc EkleDetay(int id, MPDagitimDetay form)
        {
            string birimKod = form.birimKod.Replace(".", "");

            string sql = @" IF EXISTS ( SELECT 1 FROM " + TABLOADDETAY + @" WHERE ID = " + VeriErisim.DegerYap(id) + @" AND BIRIMKOD = " + VeriErisim.DegerYap(birimKod) + @")
BEGIN
    UPDATE " + TABLOADDETAY + @"
    SET
        MIKTAR = " + VeriErisim.DegerYap(form.miktar) + @",
        BUYILDEGER = " + VeriErisim.DegerYap(form.buYilDeger) + @",
        ONCYILDEGER = " + VeriErisim.DegerYap(form.oncYilDeger) + @"
    WHERE ID = " + VeriErisim.DegerYap(id) + @"
      AND BIRIMKOD = " + VeriErisim.DegerYap(birimKod) + @"
END
ELSE
BEGIN
    INSERT INTO " + TABLOADDETAY + @" (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + @")
    VALUES (
        " + VeriErisim.DegerYap(id) + @",
        " + VeriErisim.DegerYap(birimKod) + @",
        " + VeriErisim.DegerYap(form.miktar) + @",
        " + VeriErisim.DegerYap(form.buYilDeger) + @",
        " + VeriErisim.DegerYap(form.oncYilDeger) + @"
    )
END";

            return KomutCalistir(sql, 1);
        }


        /// <summary>
        /// Form yapýsýnda duran alanlarý kýstas olarak kullanarak,
        /// select sql cümlesini oluþturup, çalýþtýran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayýtlara ait kýstas bilgisini tutan, MPDagitim yapýsýnda nesne</param>
        /// <returns>Select komutunun çalýþtýrýlmasý sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleDetay(int id)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("D", ALANLARDETAY) + ", K.AD FROM " + TABLOADDETAY + " D";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.BHMKURUMSALYAPI K ON K.KOD = D.BIRIMKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("D", ALANLARDETAY[(int)ENUMAlanlarDETAY.ID], "=", id));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

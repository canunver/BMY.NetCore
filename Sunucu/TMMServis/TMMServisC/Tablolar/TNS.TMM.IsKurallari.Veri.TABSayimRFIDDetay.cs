using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır Sayım RFID Detay bilgilerini tutan TASSAYIMRFIDDETAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSayimRFIDDetay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSayimRFIDDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSAYIMRFIDDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            YIL = 0,
            SAYIMNO = 1,
            PRSICILNO = 2,
            DURUM = 3,
            TERMINALKOD = 4,
            BULUNANODA = 5,
            BEKLENENODA = 6,
            BULUNANTCKIMLIKNO = 7,
            BEKLENENTCKIMLIKNO = 8,
            BULUNANTARIH = 9,
            BEKLENENMUHASEBE = 10,
            BEKLENENHARCAMABIRIM = 11,
            BEKLENENAMBAR = 12,
            RFID = 13,
            TCKIMLIKNO = 14,
            ODA = 15
        }

        public enum ENUMAlanlar2 : int
        {
            YIL = 0,
            SAYIMNO = 1,
            RFID = 2,
            PRSICILNO = 3,
            TCKIMLIKNO = 4,
            ODA = 5,
            DURUM = 6
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
      
        
        static string[] ALANLAR = { "YIL", "SAYIMNO", "PRSICILNO", "DURUM", "TERMINALKOD", "BULUNANODA", "BEKLENENODA",
                                    "BULUNANTCKIMLIKNO", "BEKLENENTCKIMLIKNO", "BULUNANTARIH",
                                    "BEKLENENMUHASEBE", "BEKLENENHARCAMABIRIM", "BEKLENENAMBAR",
                                    "RFID","TCKIMLIKNO","ODA"};

        static string[] ALANLAR2 = { "YIL", "SAYIMNO", "RFID", "PRSICILNO", "TCKIMLIKNO","ODA", "DURUM"};  
        /// <summary>
        /// Sayım RFID Detay bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sdrfid">Sayım RFID detay bilgilerini tutan, SayimRFIDDetay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(SayimRFIDDetay sdrfid)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sdrfid.yil) + ",";
            sql += VeriErisim.DegerYap(sdrfid.sayimNo) + ",";
            sql += VeriErisim.DegerYap(sdrfid.prSicilNo) + ",";
            sql += VeriErisim.DegerYap((int)sdrfid.durum) + ",";
            sql += VeriErisim.DegerYap(sdrfid.terminalKod) + ",";
            sql += VeriErisim.DegerYap(sdrfid.bulunanOdaKod) + ",";
            sql += VeriErisim.DegerYap(sdrfid.beklenenOdaKod) + ",";
            sql += VeriErisim.DegerYap(sdrfid.bulunanKisiKod) + ",";
            sql += VeriErisim.DegerYap(sdrfid.beklenenKisiKod) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(sdrfid.bulunanTarih)) + ",";
            sql += VeriErisim.DegerYap(sdrfid.beklenenMuhasebeKod) + ",";
            sql += VeriErisim.DegerYap(sdrfid.beklenenHarcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sdrfid.beklenenAmbarKod) + ",";
            sql += VeriErisim.DegerYap(sdrfid.rfid) + ",";
            sql += VeriErisim.DegerYap(sdrfid.tckimlikno) + ",";
            sql += VeriErisim.DegerYap(sdrfid.oda) + ")";
            Sonuc sonuc = KomutCalistir(sql, 1);
            if (sonuc.islemSonuc)
                return sonuc;

            veriErisim.hata = string.Empty;

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)sdrfid.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERMINALKOD], sdrfid.terminalKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULUNANODA], sdrfid.bulunanOdaKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BEKLENENODA], sdrfid.beklenenOdaKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULUNANTCKIMLIKNO], sdrfid.bulunanKisiKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BEKLENENTCKIMLIKNO], sdrfid.beklenenKisiKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BULUNANTARIH], new TNSDateTime(sdrfid.bulunanTarih)) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BEKLENENMUHASEBE], sdrfid.beklenenMuhasebeKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BEKLENENHARCAMABIRIM], sdrfid.beklenenHarcamaBirimKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BEKLENENAMBAR], sdrfid.beklenenAmbarKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RFID], sdrfid.rfid) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], sdrfid.tckimlikno) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODA], sdrfid.oda);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sdrfid.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", sdrfid.sayimNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", sdrfid.prSicilNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Sayım No bilgisini kullanarak
        /// Sayım RFID Detay bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="sayimNo">Sayım No bilgisini tutan, String türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(int yil, string sayimNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYIMNO], "=", sayimNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Sayım No bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Sayım RFID Detay bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="sayimNo">Sayım No bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Sayım RFID Detay bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(int yil, string sayimNo)
        {
            //string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR); //+ ", TASPERSONEL.AD, TASODA.AD";
            //sql += " FROM " + TABLOAD; //+ " LEFT OUTER JOIN TASPERSONEL ";
            ////sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = TASPERSONEL.KOD";
            ////sql += " LEFT OUTER JOIN TASODA ";
            ////sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODA] + " = TASODA.KOD";

            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR2[(int)ENUMAlanlar.YIL], "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR2[(int)ENUMAlanlar.SAYIMNO], "=", sayimNo));


            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR2); //+ ", TASPERSONEL.AD, TASODA.AD";
            sql += " FROM " + TABLOAD; //+ " LEFT OUTER JOIN TASPERSONEL ";
            //sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = TASPERSONEL.KOD";
            //sql += " LEFT OUTER JOIN TASODA ";
            //sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ODA] + " = TASODA.KOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR2[(int)ENUMAlanlar2.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR2[(int)ENUMAlanlar2.SAYIMNO], "=", sayimNo));


            return veriErisim.DataReaderDondur(sql);
        }
    }
}
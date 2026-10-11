using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMAKBUZHESAP veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMakbuzHesap : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMakbuzHesap(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// create table MUHMAKBUZHESAP
        /// (
        /// 	YIL INT, 
        /// 	MUHASEBEKOD INT, 
        /// 	YEVMIYENO VARCHAR(200),
        /// 	HESAPKOD VARCHAR(200),
        /// 	TUTAR MONEY,
        /// 	SATIRNO INT, 
        /// 	primary key (YIL, MUHASEBEKOD, YEVMIYENO, SATIRNO)
        /// )
        /// </summary>
        static string TABLOAD = "MUHMAKBUZHESAP";

        enum ENUMAlanlar
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            YEVMIYENO = 2,
            HESAPKOD = 3,
            TUTAR = 4,
            SATIRNO = 5,
            HESAPLANANALAN = 6,
            ALTHESAP = 7,
            TERTIP = 8
        }
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "YEVMIYENO", "HESAPKOD", "TUTAR", "SATIRNO", "HESAPLANANALAN", "ALTHESAP", "TERTIP" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="m">Eklenecek kayıt bilgisinin ana kaydı</param>
        /// <param name="satirNo">Eklenecek kayıt bilgisinin sıra numarası</param>
        /// <param name="mh">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(Makbuz m, int satirNo, MakbuzHesap mh)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(m.yil) + ", ";
            sql += VeriErisim.DegerYap(m.muhasebe) + ", ";
            sql += VeriErisim.DegerYap(m.yevmiyeNo) + ", ";
            sql += VeriErisim.DegerYap(mh.hesapKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(mh.tutar) + ",";
            sql += VeriErisim.DegerYap(satirNo) + ",";
            sql += VeriErisim.DegerYap(mh.hesaplananAlan) + ",";
            sql += VeriErisim.DegerYap(mh.altHesap) + ",";
            sql += VeriErisim.DegerYap(mh.tertip) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="muhasebe">Muhasebe Kodu</param>
        /// <param name="yevmiyeNo">Yevmiye Numarası</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, int muhasebe, string yevmiyeNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YEVMIYENO], "=", yevmiyeNo));

            return KomutCalistir(sql, 0);
        }
    }
}
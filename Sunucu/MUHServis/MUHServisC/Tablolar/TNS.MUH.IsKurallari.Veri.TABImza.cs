using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHIMZA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABImza : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABImza(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHIMZA";

        enum ENUMAlanlar
        {
            KULLANICIKODU = 0,
            BELGETUR = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            IMZAYER = 4,
            AD = 5,
            UNVAN = 6,
            GOREVUNVAN = 7,
            TARIH = 8,
            KULLANILMIYOR = 9,
        }
        static string[] ALANLAR = { "KULLANICIKODU", "BELGETUR", "MUHASEBEKOD", "BIRIMKOD", "IMZAYER", "AD", "UNVAN", "GOREVUNVAN", "TARIH", "KULLANILMIYOR" };

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="imza">Kayıt edilecek bilgiler.</param>
        /// <returns></returns>
        public Sonuc ImzaKaydet(ImzaBilgisi imza)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(imza.kullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap((int)imza.belgeTur) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.muhasebe) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.birim.Trim() + " ") + ", ";
            sql = sql + VeriErisim.DegerYap(imza.imzaYer) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.adSoyad) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.unvan) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.gorevUnvan) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.tarih) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.kullanilmiyor) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre bilgilerine uygun Tablodaki kayıtları silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kullaniciKodu">Kullanıcı kodu.</param>
        /// <param name="belgeTur">Belge tür.</param>
        /// <param name="muhasebe">Muhasebe kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <returns></returns>
        public Sonuc ImzaSil(string kullaniciKodu, ENUMBelgeTuru belgeTur, int muhasebe, string birim)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur);

            if (muhasebe > 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe);
            if (birim != "999")//Bütün birimdeki silmesi için Melih 09.04.2020 Manas Tek imza kullanacak (ImzaTanım)
                sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birim.Trim() + " ");

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="kullaniciKodu">Kullanıcı kodu.</param>
        /// <param name="belgeTur">Belge tür.</param>
        /// <param name="muhasebe">Muhasebe kodu.</param>
        /// <param name="birim">Harcama birim kodu.</param>
        /// <returns></returns>
        public IDataReader ImzaListele(string kullaniciKodu, ENUMBelgeTuru belgeTur, int muhasebe, string birim)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birim.Trim() + " ") + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)belgeTur);

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}

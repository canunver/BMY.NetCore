using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMUDURLUK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMudurluk : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMudurluk(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHMUDURLUK";
        enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            AKTIF = 2,
        }
        static string[] ALANLAR = { "KOD", "AD", "AKTIF" };

        public Sonuc Kaydet(Mudurluk mudurluk)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(mudurluk.kod) + ", ";
            sql += VeriErisim.DegerYap(mudurluk.ad) + ", ";
            sql += VeriErisim.DegerYap(mudurluk.aktif) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += ALANLAR[(int)ENUMAlanlar.AD] + "='" + mudurluk.ad + "'";
                sql += ", " + ALANLAR[(int)ENUMAlanlar.AKTIF] + "=" + mudurluk.aktif;
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", mudurluk.kod);

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc Sil(Mudurluk mudurluk)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", mudurluk.kod);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="Mudurluk">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(Mudurluk mudurluk)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (mudurluk.kod != "")
            {
                if (kosul != "") kosul += " AND ";
                kosul += VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", mudurluk.kod);
            }

            if (mudurluk.aktif > -1)
            {
                if (kosul != "") kosul += " AND ";
                kosul += VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIF], "=", mudurluk.aktif);
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD];

            IDataReader r = veriErisim.DataReaderDondur(sql);

            return r;
        }
    }
}

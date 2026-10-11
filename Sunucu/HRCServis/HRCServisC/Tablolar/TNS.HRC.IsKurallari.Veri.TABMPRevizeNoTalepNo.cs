using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABMPRevizeNoTalepNo : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMPRevizeNoTalepNo(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Hesap Cetveli bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCMPREVIZENOTALEPNO";

        /// <summary>
        /// Hesap Cetveli bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// SURECYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 1,
            /// <summary>
            /// SURECNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SURECNO = 2,
            /// <summary>
            /// PRFATURANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            REVIZENO = 3,
            /// <summary>
            /// HPDETAYSIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TALEPNO = 4,
        }

        static string[] ALANLAR = { "YIL", "SURECYIL", "SURECNO", "REVIZENO", "TALEPNO" };

        public Sonuc Kaydet(MPRevizeNoTalepNo bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            string kosul = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.REVIZENO], bilgi.revizeNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TALEPNO], bilgi.talepNo);

            kosul = VeriErisim.KosulEkle(" WHERE", "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", (bilgi.yil)));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", (bilgi.surecYil)));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", (bilgi.surecNo)));

            sql += kosul;
            sonuc = KomutCalistir(sql, 1);


            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

                sql += VeriErisim.DegerYap(bilgi.yil) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecYil) + ",";
                sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.revizeNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.talepNo) + ")";

                sonuc = KomutCalistir(sql, 1);
            }


            return sonuc;
        }

        public IDataReader Listele(MPRevizeNoTalepNo bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.surecYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", bilgi.surecYil));
            if (bilgi.surecNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(MPRevizeNoTalepNo bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}
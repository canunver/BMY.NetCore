using System.Data;
using TNS.Veri;
using System;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeSulamaBankaKredi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHibeSulamaBankaKredi(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "TRMHIBESULAMABANKAKREDI";
        enum ENUMAlanlar
        {
            KOD = 0,
            ETAPNO = 1,
            ILKOD = 2,
            TARIH = 3,
            ALIMYERI = 4,
            KIMLIKNO = 5,
            ADI = 6,
            SOYADI = 7,
            PARSELNO = 8,
            KAYITEDEN = 9
        }
        static string[] ALANLAR = { "KOD", "ETAPNO", "ILKOD", "TARIH", "ALIMYERI", "KIMLIKNO", "ADI", "SOYADI", "PARSELNO", "KAYITEDEN" };

        public Sonuc Kaydet(HibeSulamaBankaKredi bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.etapNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.alimyeri) + ",";
            sql += VeriErisim.DegerYap(bilgi.kimlikNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.adi) + ",";
            sql += VeriErisim.DegerYap(bilgi.soyadi) + ",";
            sql += VeriErisim.DegerYap(bilgi.parselNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.kayitEden) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(HibeSulamaBankaKredi bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], bilgi.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALIMYERI], bilgi.alimyeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], bilgi.kimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.adi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOYADI], bilgi.soyadi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PARSELNO], bilgi.parselNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KAYITEDEN], bilgi.kayitEden);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(HibeSulamaBankaKredi kriter)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", ORGCOGRAFYA.AD";
            sql += " FROM TRMHIBESULAMABANKAKREDI, ORGCOGRAFYA ";
            sql += " WHERE TRMHIBESULAMABANKAKREDI.ILKOD=ORGCOGRAFYA.KOD";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", kriter.etapNo));

            if (kriter.kod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMABANKAKREDI", ALANLAR[(int)ENUMAlanlar.KOD], "=", kriter.kod));

            if (kriter.ilKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMABANKAKREDI", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", kriter.ilKod));

            if (kriter.kimlikNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMABANKAKREDI", ALANLAR[(int)ENUMAlanlar.KIMLIKNO], "=", kriter.kimlikNo));

            if (kriter.parselNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMABANKAKREDI", ALANLAR[(int)ENUMAlanlar.PARSELNO], "=", kriter.parselNo));

            if (kriter.sorguParsel != "")
                sql += " AND TRMHIBESULAMABANKAKREDI.PARSELNO IN (" + kriter.sorguParsel + ")";

            sql += " ORDER BY TRMHIBESULAMABANKAKREDI.ADI, TRMHIBESULAMABANKAKREDI.SOYADI";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(string kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }
    }
}

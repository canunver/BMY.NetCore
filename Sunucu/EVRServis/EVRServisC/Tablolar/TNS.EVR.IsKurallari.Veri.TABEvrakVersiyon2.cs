using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakVersiyon2 : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakVersiyon2 sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakVersiyon2(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKVERSIYON2";

        public enum ENUMAlanlar : int
        {
            EVREVRAKNO = 0,
            SIRANO = 1,
            GUID = 2,
            ISLEMTARIH = 3,
            ISLEMYAPAN = 4,
            ISLEMYAPANAD = 5
        }

        /// <summary>
        /// Evrak versiyon bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "EVREVRAKNO", "SIRANO", "GUID", "ISLEMTARIH", "ISLEMYAPAN", "ISLEMYAPANAD" };


        public Sonuc Kaydet(EvrakVersiyon2 form)
        {
            form.siraNo = this.SiraNoAl(form.evrEvrakNo);
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.guid) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islemYapanAd) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EvrakVersiyon2 form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));
            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        private int SiraNoAl(string evrEvrakNo)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", evrEvrakNo));

            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}

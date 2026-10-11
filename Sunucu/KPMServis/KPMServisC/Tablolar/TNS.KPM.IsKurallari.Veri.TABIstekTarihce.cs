//using System;
//using System.Data;
//using System.Collections.Generic;
//using System.Text;
//using TNS.Veri;

//namespace TNS.KPM.IsKurallari.Veri
//{
//    public class TABIstekTarihce : TABOrtak
//    {
//        public TABIstekTarihce(VeriErisim veriErisim) : base(veriErisim) { }

//        static string TABLOAD = "KPMISTEKTARIHCE";
//        enum ENUMAlanlar : int
//        {
//            YIL = 0,
//            MUHASEBEKOD = 1,
//            BIRIMKOD = 2,
//            ISTEKNO = 3,
//            ISLEMYAPAN = 4,
//            ISLEMTARIH = 5,
//            DURUM = 6
//        }
//        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "BIRIMKOD", "ISTEKNO",
//                                   "ISLEMYAPAN", "ISLEMTARIH", "DURUM" };

//        public Sonuc Kaydet(IstekTarihce it)
//        {
//            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
//            sql += VeriErisim.DegerYap(it.yil) + ",";
//            sql += VeriErisim.DegerYap(it.muhasebeKod) + ",";
//            sql += VeriErisim.DegerYap(it.birimKod) + ",";
//            sql += VeriErisim.DegerYap(it.istekNo) + ",";
//            sql += VeriErisim.DegerYap(it.islemYapan) + ",";
//            sql += VeriErisim.DegerYap(it.islemTarih) + ",";
//            sql += VeriErisim.DegerYap(it.durum) + ")";
            
//            return KomutCalistir(sql, 1);
//        }

//        public IDataReader Listele(IstekTarihce it)
//        {
//            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
//            sql += " FROM " + TABLOAD;
//            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", it.yil));
//            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", it.muhasebeKod));
//            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", it.birimKod));
//            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISTEKNO], "=", it.istekNo));

//            return veriErisim.DataReaderDondur(sql);
//        }
//    }
//}
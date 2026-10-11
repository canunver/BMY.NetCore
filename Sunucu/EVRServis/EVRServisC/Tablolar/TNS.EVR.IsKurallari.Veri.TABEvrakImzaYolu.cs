using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakImzaYolu : TABOrtak
    {
        public TABEvrakImzaYolu(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKIMZAYOLU";
        public static string TABLOADHAREKET = "EVREVRAKIMZAYOLUHAREKET";

        public enum ENUMAlanlar : int
        {
            PREVRAKNO = 0,
            SIRANO = 1,
            IMZAYOLU = 2,
            IMZAYOLUAD = 3,
            ISLEMTIP = 4,
            TCKIMLIKNO = 5,
            AD = 6,
            SOYAD = 7,
            EVRAKBIRIMKOD = 8,
            UNVAN = 9,
            AKADEMIKUNVAN = 10
        }
        public enum ENUMAlanlarHareket : int
        {
            PREVRAKNO = 0,
            SIRANO = 1,
            KISI1 = 2,
            KISI2 = 3,
            ISLEMTIP = 4,
            ISLEMTARIH = 5
        }

        public static string[] ALANLAR = { "PREVRAKNO", "SIRANO", "IMZAYOLU", "IMZAYOLUAD", "ISLEMTIP", "TCKIMLIKNO", "AD", "SOYAD", "EVRAKBIRIMKOD", "UNVAN", "AKADEMIKUNVAN" };
        public static string[] ALANLARHAREKET = { "PREVRAKNO", "SIRANO", "KISI1", "KISI2", "ISLEMTIP", "ISLEMTARIH" };

        public Sonuc Kaydet(EvrakImzaYolu form)
        {
            Sonuc sonuc = this.Sil(form);
            if (sonuc.islemSonuc)
            {
                int siraNo = 0;
                foreach (ImzaYoluDetay detay in form.imzaYolu.detaylar)
                {
                    siraNo++;
                    string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
                    sql += VeriErisim.DegerYap(siraNo) + ",";
                    sql += VeriErisim.DegerYap(form.imzaYolu.kod) + ",";
                    sql += VeriErisim.DegerYap(form.imzaYolu.ad) + ",";
                    sql += VeriErisim.DegerYap(detay.islemTip) + ",";
                    sql += VeriErisim.DegerYap(detay.tcKimlikNo) + ",";
                    sql += VeriErisim.DegerYap(detay.ad) + ",";
                    sql += VeriErisim.DegerYap(detay.soyad) + ",";
                    sql += VeriErisim.DegerYap(detay.evrakBirimKod) + ",";
                    sql += VeriErisim.DegerYap(detay.unvan) + ",";
                    sql += VeriErisim.DegerYap(detay.akademikUnvan) + ")";

                    sonuc = KomutCalistir(sql, 1);
                    if (!sonuc.islemSonuc)
                        break;
                }
            }
            return sonuc;
        }
        public Sonuc Kaydet(EvrakImzaYoluHareket hareket)
        {
            hareket.siraNo = this.HareketSiraNoAl(hareket.prEvrakNo);

            string sql = "INSERT INTO " + TABLOADHAREKET + " (" + VeriErisim.TumAlanlar("", ALANLARHAREKET) + ") VALUES (";
            sql += VeriErisim.DegerYap(hareket.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(hareket.siraNo) + ",";
            sql += VeriErisim.DegerYap(hareket.kisi1) + ",";
            sql += VeriErisim.DegerYap(hareket.kisi2) + ",";
            sql += VeriErisim.DegerYap(hareket.islemTip) + ",";
            sql += VeriErisim.DegerYap(hareket.islemTarih) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EvrakImzaYolu form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            if (form.imzaYolu != null && form.imzaYolu.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IMZAYOLU], "=", form.imzaYolu.kod));

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleHareket(string prEvrakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARHAREKET) + " FROM " + TABLOADHAREKET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAREKET[(int)ENUMAlanlarHareket.PREVRAKNO], "=", prEvrakNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(EvrakImzaYolu form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc Sil(EvrakImzaYoluHareket form)
        {
            string sql = "DELETE FROM " + TABLOADHAREKET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));

            return KomutCalistir(sql, 0);
        }


        private int HareketSiraNoAl(string prEvrakNo)
        {
            string sql = "SELECT MAX(" + ALANLARHAREKET[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOADHAREKET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARHAREKET[(int)ENUMAlanlarHareket.PREVRAKNO], "=", prEvrakNo));

            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}

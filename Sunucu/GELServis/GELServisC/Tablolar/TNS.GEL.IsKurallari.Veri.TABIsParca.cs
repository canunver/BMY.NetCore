using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABIsParca : TABOrtak
    {
        public TABIsParca(VeriErisim veriErisim) : base(veriErisim) { }

        protected static string TABLOAD = "GELISPARCA";
        protected enum ENUMAlanlar
        {
            ISKOD = 0,
            PARCAKOD = 1,
            SIRANO = 2,
            DUZEY = 3,
            ZORUNLU = 4
        }

        protected static string[] ALANLAR = { "ISKOD", "PARCAKOD", "SIRANO", "DUZEY", "ZORUNLU" };


        public Sonuc Kaydet(IsParca bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            for (int i = 0; i < bilgi.parcalar.objeler.Count; i++)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.isAd.kod) + ",";
                sql += VeriErisim.DegerYap(((Parca)bilgi.parcalar.objeler[i]).kod) + ",";
                sql += VeriErisim.DegerYap(i + 1) + ",";
                sql += VeriErisim.DegerYap(((Parca)bilgi.parcalar.objeler[i]).duzey) + ",";
                sql += VeriErisim.DegerYap(((Parca)bilgi.parcalar.objeler[i]).zorunlu) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }

            return sonuc;
        }

        public Sonuc Sil(int isKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISKOD], "=", isKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(int isKod)
        {
            string sql = "";

            //sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + "," + VeriErisim.TumAlanlar(TABParca.TABLOAD, TABParca.ALANLAR) + " FROM " + TABLOAD + "," + TABParca.TABLOAD;
            //sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARCAKOD] + "=" + TABParca.TABLOAD + "." + TABParca.ALANLAR[(int)TABParca.ENUMAlanlar.KOD]);
            //if (isKod > 0)
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISKOD], "=", isKod));
            //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ISKOD] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO];

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + ",";
            sql += TABParca.ENUMAlanlar.AD + ",";
            sql += TABParca.ENUMAlanlar.SAYFA + ",";
            sql += TABParca.ENUMAlanlar.YUKSEKLIK + ",";
            sql += TABParca.ENUMAlanlar.GENISLIK;
            sql += " FROM " + TABLOAD + "," + TABParca.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PARCAKOD] + "=" + TABParca.TABLOAD + "." + TABParca.ALANLAR[(int)TABParca.ENUMAlanlar.KOD]);
            if (isKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISKOD], "=", isKod));
            return veriErisim.DataReaderDondur(sql);
        }


    }
}

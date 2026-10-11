using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABAskerlik : TABOrtak
    {
        public TABAskerlik(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELASKERLIK";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            ASKERLIKDURUMU = 1,
            TECILTARIHI = 2,
            DUHULTARIHI = 3,
            TERHISTARIHI = 4,
            MUAFIYETNEDENI = 5,
            SAYILMAYANGUNSAYISI = 6,
            ASKERLIKCUZDANNO = 7,
            RUTBESI = 8,
            SINIFI = 9,
            BORCLANMAVARMI = 10,
            TERHISBELGENO = 11,
            TERHISBELGETARIHI = 12,
            ASKERLIKSUBESI = 13,
            YEDEKSUBAYOKULUGIRISTARIHI = 14,
            ASTEGMENNASPTARIHI = 15,
            TEGMENNASPTARIHI = 16,
            SINIFDONEMSICILNO = 17,
            ERYEDEKSUBAYGOREVYERI = 18,
            ERYEDEKSUBAYGECISTARIHI = 19,
            ERYEDEKSUBAYKITAHIZMETIBASLAMATARIHI = 20,
            ERYEDEKSUBAYKITAHIZMETIBITISTARIHI = 21,
            IBRAZINTIKALTARIHI = 22,
            ACIKLAMA = 23
        }
        public static string[] ALANLAR = { "KOD", "ASKERLIKDURUMU", "TECILTARIHI", "DUHULTARIHI", "TERHISTARIHI", "MUAFIYETNEDENI", "SAYILMAYANGUNSAYISI", "ASKERLIKCUZDANNO", "RUTBESI", 
                                    "SINIFI", "BORCLANMAVARMI", "TERHISBELGENO", "TERHISBELGETARIHI", "ASKERLIKSUBESI", "YEDEKSUBAYOKULUGIRISTARIHI", "ASTEGMENNASPTARIHI", "TEGMENNASPTARIHI", 
                                    "SINIFDONEMSICILNO", "ERYEDEKSUBAYGOREVYERI", "ERYEDEKSUBAYGECISTARIHI", "ERYEDEKSUBAYKITAHIZMETIBASLAMATARIHI", "ERYEDEKSUBAYKITAHIZMETIBITISTARIHI", 
                                    "IBRAZINTIKALTARIHI", "ACIKLAMA" };

        public IDataReader Listele(int kod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " , ";
            sql += TABSozluk.TABLOAD + "." + TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KAYITKOD] + " , ";
            sql += TABSozluk.TABLOAD + "." + TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.HITAPID] + " , ";
            sql += TABSozluk.TABLOAD + "." + TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KAYITAD] + ", s2.KAYITAD";
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABSozluk.TABLOAD + " ON " + ALANLAR[(int)ENUMAlanlar.ASKERLIKDURUMU] + "=" + TABSozluk.TABLOAD + "." + TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KOD];
            sql += " LEFT OUTER JOIN IKYSOZLUK as s2 ON " + ALANLAR[(int)ENUMAlanlar.SINIFI] + "= s2.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(PAskerlik p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOAD + " SET ";
            if (p.askerlikDurumu > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASKERLIKDURUMU], p.askerlikDurumu) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASKERLIKDURUMU], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TECILTARIHI], p.tecilTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DUHULTARIHI], p.askerlikBaslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERHISTARIHI], p.terhisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUAFIYETNEDENI], p.askerlikMuafNeden) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYILMAYANGUNSAYISI], p.sayilmayanGunSayisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASKERLIKCUZDANNO], p.askerlikCuzdanNo) + ",";
            if (p.rutbesi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RUTBESI], p.rutbesi) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RUTBESI], (string)null) + ",";
            if (p.sinifi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIFI], p.sinifi) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIFI], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCLANMAVARMI], p.borclanmaVarMi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERHISBELGENO], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TERHISBELGETARIHI], p.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASKERLIKSUBESI], p.askerlikSubesi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YEDEKSUBAYOKULUGIRISTARIHI], p.yedekSubayOkuluGirisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ASTEGMENNASPTARIHI], p.astegmenNaspTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEGMENNASPTARIHI], p.tegmenNaspTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIFDONEMSICILNO], p.sinifDonemSicilNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ERYEDEKSUBAYGOREVYERI], p.erYedekSubayGorevYeri) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ERYEDEKSUBAYGECISTARIHI], p.erYedekSubayGecisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ERYEDEKSUBAYKITAHIZMETIBASLAMATARIHI], p.erYedekSubayKitaHizmetiBaslamaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ERYEDEKSUBAYKITAHIZMETIBITISTARIHI], p.erYedekSubayKitaHizmetiBitisTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IBRAZINTIKALTARIHI], p.ibrazIntikalTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], p.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                if (p.askerlikDurumu > 0)
                    sql += VeriErisim.DegerYap(p.askerlikDurumu) + ",";
                else
                    sql += VeriErisim.DegerYap((string)null) + ",";
                sql += VeriErisim.DegerYap(p.tecilTarihi) + ",";
                sql += VeriErisim.DegerYap(p.askerlikBaslamaTarihi) + ",";
                sql += VeriErisim.DegerYap(p.terhisTarihi) + ",";
                sql += VeriErisim.DegerYap(p.askerlikMuafNeden) + ",";
                sql += VeriErisim.DegerYap(p.sayilmayanGunSayisi) + ",";
                sql += VeriErisim.DegerYap(p.askerlikCuzdanNo) + ",";
                if (p.rutbesi > 0)
                    sql += VeriErisim.DegerYap(p.rutbesi) + ",";
                else
                    sql += VeriErisim.DegerYap((string)null) + ",";
                if (p.sinifi > 0)
                    sql += VeriErisim.DegerYap(p.sinifi) + ",";
                else
                    sql += VeriErisim.DegerYap((string)null) + ",";
                sql += VeriErisim.DegerYap(p.borclanmaVarMi) + ",";
                sql += VeriErisim.DegerYap(p.belgeNo) + ",";
                sql += VeriErisim.DegerYap(p.belgeTarihi) + ",";
                sql += VeriErisim.DegerYap(p.askerlikSubesi) + ",";
                sql += VeriErisim.DegerYap(p.yedekSubayOkuluGirisTarihi) + ",";
                sql += VeriErisim.DegerYap(p.astegmenNaspTarihi) + ",";
                sql += VeriErisim.DegerYap(p.tegmenNaspTarihi) + ",";
                sql += VeriErisim.DegerYap(p.sinifDonemSicilNo) + ",";
                sql += VeriErisim.DegerYap(p.erYedekSubayGorevYeri) + ",";
                sql += VeriErisim.DegerYap(p.erYedekSubayGecisTarihi) + ",";
                sql += VeriErisim.DegerYap(p.erYedekSubayKitaHizmetiBaslamaTarihi) + ",";
                sql += VeriErisim.DegerYap(p.erYedekSubayKitaHizmetiBitisTarihi) + ",";
                sql += VeriErisim.DegerYap(p.ibrazIntikalTarihi) + ",";
                sql += VeriErisim.DegerYap(p.aciklama) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc Sil(PAskerlik p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }
    }
}

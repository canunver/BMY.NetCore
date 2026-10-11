using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABGorevPersonel : TABOrtak
    {
        public TABGorevPersonel(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "PERGOREVPERSONEL";

        public enum ENUMAlanlar : int
        {
            GOREVKOD = 0,
            TCKIMLIKNO = 1,
            AD = 2,
            SOYAD = 3,
            UNVANAD = 4,
            UNVANKOD = 5,
            KADRODERECE = 6,
            EKGOSTERGE = 7,
            GOREVYIL = 8,
        }
        public static string[] ALANLAR = { "GOREVKOD", "TCKIMLIKNO", "AD", "SOYAD", "UNVANAD", "UNVANKOD", "KADRODERECE", "EKGOSTERGE", "GOREVYIL" };

        public IDataReader Listele(GorevPersonel p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.gorevYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVYIL], "=", p.gorevYil));

            if (p.gorevKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVKOD], "=", p.gorevKod));

            if (p.tckimlikno != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", p.tckimlikno));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.GOREVKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(GorevPersonel p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.gorevKod) + ",";
            sql += VeriErisim.DegerYap(p.tckimlikno) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.soyad) + ",";
            sql += VeriErisim.DegerYap(p.unvan) + ",";
            sql += VeriErisim.DegerYap(p.unvanKod) + ",";
            sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
            sql += VeriErisim.DegerYap(p.ekGosterge) + ",";
            sql += VeriErisim.DegerYap(p.gorevYil) + ")";

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public Sonuc Kaydet(ObjectArray liste, int gorevYil, int gorevKod)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            foreach (GorevPersonel p in liste.objeler)
            {
                sql = string.Empty;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(gorevKod) + ",";
                sql += VeriErisim.DegerYap(p.tckimlikno) + ",";
                sql += VeriErisim.DegerYap(p.ad) + ",";
                sql += VeriErisim.DegerYap(p.soyad) + ",";
                sql += VeriErisim.DegerYap(p.unvan) + ",";
                sql += VeriErisim.DegerYap(p.unvanKod) + ",";
                sql += VeriErisim.DegerYap(p.kadroDerece) + ",";
                sql += VeriErisim.DegerYap(p.ekGosterge) + ",";
                sql += VeriErisim.DegerYap(p.gorevYil) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    break;
                }
            }
            return sonuc;
        }

        public Sonuc Sil(int gorevYil, int gorevKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOREVYIL], "=", gorevYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOREVKOD], "=", gorevKod));

            return KomutCalistir(sql, 0);
        }
    }
}

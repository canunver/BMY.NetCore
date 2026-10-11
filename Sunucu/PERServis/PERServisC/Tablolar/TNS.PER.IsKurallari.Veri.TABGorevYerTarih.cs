using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABGorevYerTarih : TABOrtak
    {
        public TABGorevYerTarih(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "PERGOREVYERTARIH";

        public enum ENUMAlanlar : int
        {
            GOREVKOD = 0,
            SIRANO = 1,
            BULUNDUGUILKOD = 2,
            BULUNDUGUILCEKOD = 3,
            GIDECEGIILKOD = 4,
            GIDECEGIILCEKOD = 5,
            BASLANGICTARIH = 6,
            BITISTARIH = 7,
            GOREVYIL = 8,
        }

        public static string[] ALANLAR = { "GOREVKOD", "SIRANO", "BULUNDUGUILKOD", "BULUNDUGUILCEKOD", "GIDECEGIILKOD", "GIDECEGIILCEKOD", "BASLANGICTARIH", "BITISTARIH", "GOREVYIL" };

        public IDataReader Listele(GorevYerTarih p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.gorevYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVYIL], "=", p.gorevYil));

            if (p.gorevKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOREVKOD], "=", p.gorevKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(ObjectArray liste, int gorevYil, int gorevKod)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            int siraNo = 1;
            foreach (GorevYerTarih p in liste.objeler)
            {
                sql = string.Empty;
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(gorevKod) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(p.bulunduguIlKod) + ",";
                sql += VeriErisim.DegerYap(p.bulunduguIlceKod) + ",";
                sql += VeriErisim.DegerYap(p.gidecegiIlKod) + ",";
                sql += VeriErisim.DegerYap(p.gidecegiIlceKod) + ",";
                sql += VeriErisim.DegerYap(p.baslangicTarihi) + ",";
                sql += VeriErisim.DegerYap(p.bitisTarihi) + ",";
                sql += VeriErisim.DegerYap(p.gorevYil) + ")";

                sonuc = KomutCalistir(sql, 1);
                siraNo++;
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

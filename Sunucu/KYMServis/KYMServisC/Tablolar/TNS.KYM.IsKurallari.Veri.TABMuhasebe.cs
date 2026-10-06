using System;
using TNS;
using TNS.Veri;

namespace TNS.KYM.IsKurallari.Veri
{
    public class TABMuhasebe : TABOrtak
    {
        string TABLOAD = "KULLANICI_MUHASEBE";

        public TABMuhasebe(VeriErisim pVeriErisim)
            : base(pVeriErisim)
        {
        }

        public System.Data.IDataReader Listele()
        {
            string sql = "SELECT KOD, AD FROM " + TABLOAD;
            System.Data.IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        public System.Data.IDataReader Oku(string kod)
        {
            string sql = "SELECT KOD, AD FROM " + TABLOAD + " WHERE ";
            sql = sql + VeriErisim.KosulOl("", "KOD", "=", kod);
            System.Data.IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }

        public TNS.KYM.Muhasebe OkuAd(string ad)
        {
            Muhasebe y = new Muhasebe();

            string sql = "SELECT KOD, AD FROM " + TABLOAD + " WHERE ";
            sql = sql + VeriErisim.KosulOl("", "AD", "=", ad);
            System.Data.IDataReader r = veriErisim.DataReaderDondur(sql);
            if (r.Read())
            {
                y.kod = VeriErisim.DegerAl(r, 0, "");
                y.ad = VeriErisim.DegerAl(r, 1, "");
            }
            r.Close();
            return y;
        }

        public Sonuc Ekle(Muhasebe muhasebe)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "INSERT INTO " + TABLOAD + "(KOD, AD) VALUES (";
            sql = sql + VeriErisim.DegerYap(muhasebe.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(muhasebe.ad) + ") ";
            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql = sql + VeriErisim.AtamaOl("", "AD", muhasebe.ad);
                sql = sql + "WHERE " + VeriErisim.KosulOl("", "KOD", "=", muhasebe.kod);
                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public int KullaniciSayiBul(string kod)
        {
            int say;

            string sql = "SELECT COUNT(*) FROM KULLANICI ";
            sql = sql + "WHERE CALISTIGIKURUM='" + kod + "'";
            System.Data.IDataReader r = veriErisim.DataReaderDondur(sql);
            if (veriErisim.hata == "")
            {
                if (r.Read())
                {
                    if (r.IsDBNull(0))
                        say = 0;
                    else
                        say = VeriErisim.DegerAl(r, 0, 0);
                }
                else
                    say = 0;
                r.Close();
            }
            else
                say = -1;
            return say;
        }

        public Sonuc Sil(string kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = sql + " WHERE KOD=" + kod;

            return KomutCalistir(sql, 1);
        }
    }
}

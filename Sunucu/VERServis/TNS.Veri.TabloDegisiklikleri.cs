using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using Oracle.ManagedDataAccess.Client;

namespace TNS.Veri
{
    public class TabloDegisiklikleri
    {
        public static Sonuc Kaydet(string p_baglantiAd, SQLKomut[] komutlar)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            string tur = OrtakFonksiyonlar.WebConfigOku("VTTip", "");

            VeriErisim ve = VeriErisim.VeriErisimBul(null, p_baglantiAd, null);
            try
            {
                int versiyon = ve.TekKolonSQLInt("SELECT MAX(KOD) FROM VERSIYON");

                if (versiyon == 0)
                {
                    string sql = @"CREATE TABLE [dbo].[VERSIYON](
                             [KOD] [int] NOT NULL,
                             [TARIH] [datetime] NULL,
                             [ACIKLAMA] [varchar](500) NULL,
                             CONSTRAINT [PK_VERSIYON] PRIMARY KEY CLUSTERED 
                            ([KOD] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
                    ve.hata = "";
                    ve.ExecuteNonquery(sql);
                }

                TNSDateTime tarih = new TNSDateTime(DateTime.Now);

                //VERSIYON tablosunu listele ve daha önce çalıştırılmış SQL komutlarını bul
                string sqlIc = "SELECT KOD FROM VERSIYON";
                ve.hata = "";
                IDataReader dr = ve.DataReaderDondur(sqlIc);
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        int vkod = VeriErisim.DegerAl(dr, 0, 0);
                        foreach (SQLKomut komut in komutlar)
                        {
                            if (vkod == komut.versiyon)
                            {
                                komut.sql = "";
                                break;
                            }
                        }
                    }

                    dr.Close();
                }

                foreach (SQLKomut komut in komutlar)
                {
                    if (komut.sql == "") continue;

                    if (string.IsNullOrWhiteSpace(komut.tur) || komut.tur == tur)
                    {
                        ve.hata = "";
                        ve.ExecuteNonquery(komut.sql);

                        if (string.IsNullOrEmpty(ve.hata))
                            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + komut.versiyon + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(komut.aciklama) + ")");
                        else
                            continue;

                    }
                }
            }
            catch (Exception ex)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = ex.Message;
            }
            ve.hata = "";
            ve.VeriErisimKapat();

            return sonuc;
        }

    }

    public class SQLKomut
    {
        public int versiyon = 0;
        public string tur = "";
        public string sql = "";
        public string aciklama = "";

        public SQLKomut(int versiyon, string tur, string sql, string aciklama)
        {
            this.versiyon = versiyon;
            this.tur = tur;
            this.sql = sql;
            this.aciklama = aciklama;
        }

    }

}

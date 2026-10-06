using TNS.Veri;

namespace TNS.KYM.IsKurallari.Veri
{
    public class TABLogoIslem : TABOrtak
    {
        public TABLogoIslem(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }
        static string TABLOAD = "LOGO";
        public enum ENUMAlanlar : int
        {

            TUR = 0,
            DATA = 1

        }
        static string[] ALANLAR = { "TUR", "DATA" };

        public System.Data.IDataReader Listele(Logo bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM  " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (bilgi.turkod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.turkod));
            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc Kaydet(Logo bilgi)
        {
            string sql = "";
            string imgStr = "0x" + System.BitConverter.ToString(bilgi.data).Replace("-", string.Empty);

            Sonuc sonuc = new Sonuc(false, "", "");
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.turkod) + ",";
            sql += imgStr + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(int turkod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", turkod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc TabloOlustur()
        {
            string sql = "";
            sql = "IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LOGO') ";
            sql += " BEGIN ";
            sql += " 	CREATE TABLE [dbo].[LOGO]( ";
            sql += " 	[TUR] [int] NOT NULL, ";
            sql += " 	[DATA] [image] NULL, ";
            sql += " 	CONSTRAINT [PK_LOGO] PRIMARY KEY CLUSTERED  ";
            sql += " 	( ";
            sql += " 	[TUR] ASC ";
            sql += " 	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ";
            sql += " 	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";
            sql += " END ";
            return KomutCalistir(sql, 0);
        }
    }
}

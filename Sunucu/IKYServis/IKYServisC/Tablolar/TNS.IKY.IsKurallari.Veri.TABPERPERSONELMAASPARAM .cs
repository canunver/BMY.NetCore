using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.IKY;
using System.Text;
namespace TNS.IKY.IsKurallari.Veri
{
    /// <summary>
    /// PERBORDROTUR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class PERPERSONELMAASPARAM : TABOrtak
    {
        /// <summary>
        /// TABPERBORDROTUR sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public PERPERSONELMAASPARAM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
            CREATE TABLE PERPERSONELMAASPARAM(TCKIMLIKNO VARCHAR(11), DEGISKEN VARCHAR(50), TIP INTEGER,  DEGER MONEY, PRIMARY KEY (TCKIMLIKNO, DEGISKEN))
            CREATE TABLE PERPERSONELMAASMATRAH(TCKIMLIKNO VARCHAR(11), YIL INT, SURECNO VARCHAR(50), MATRAHTUR INT, TARIH DATETIME, DEGER MONEY, TABLOAD VARCHAR(80) NOT NULL DEFAULT '', SATIRNO int NOT NULL DEFAULT 0, PRIMARY KEY (TCKIMLIKNO, YIL, SURECNO, MATRAHTUR, TARIH, TABLOAD, SATIRNO)) 
         * * 
         * */

        public Sonuc SilParam(string personelTCKimlikNo, TNSCollection degiskenler)
        {
            string sql = "DELETE FROM PERPERSONELMAASPARAM WHERE TCKIMLIKNO=" + VeriErisim.DegerYap(personelTCKimlikNo);
            if (degiskenler != null && degiskenler.Count > 0)
            {
                string ekle = "";
                foreach (string item in degiskenler)
                {
                    if (ekle != "") ekle += ",";
                    ekle += VeriErisim.DegerYap(item);
                }
                sql = VeriErisim.KosulEkle(sql, "AND", "DEGISKEN IN (" + ekle + ")");
            }
            return this.KomutCalistir(sql, 0);
        }

        public Sonuc EkleParam(string personelTCKimlikNo, MaasParam item)
        {
            string sql = "INSERT INTO PERPERSONELMAASPARAM (TCKIMLIKNO, DEGISKEN, DEGER, DEGERSTR ) VALUES (" + VeriErisim.DegerYap(personelTCKimlikNo) + "," + VeriErisim.DegerYap(item.degiskenAd) + "," + VeriErisim.DegerYap(item.degerDbl) + "," + VeriErisim.DegerYap(item.degerStr) + ")";
            return this.KomutCalistir(sql, 0);
        }

        public IDataReader ListeleParam(string personelTCKimlikNo)
        {
            string sql = "SELECT DEGISKEN, DEGER, DEGERSTR FROM PERPERSONELMAASPARAM WHERE TCKIMLIKNO=" + VeriErisim.DegerYap(personelTCKimlikNo);
            return this.veriErisim.DataReaderDondur(sql);
        }

        public static string ListeleParamUstTablo(string ustTabloAlan)
        {
            return "SELECT DEGISKEN, DEGER, DEGERSTR FROM PERPERSONELMAASPARAM WHERE PERPERSONELMAASPARAM.TCKIMLIKNO=" + ustTabloAlan;
        }

        public Sonuc SilMatrah(string personelTCKimlikNo, int yil, string surecNo, string tabloAd, int satirNo)
        {
            string sql = "DELETE FROM PERPERSONELMAASMATRAH WHERE YIL=" + VeriErisim.DegerYap(yil) + " AND SURECNO=" + VeriErisim.DegerYap(surecNo);
            sql += " AND (TABLOAD = '' OR TABLOAD IS NULL OR TABLOAD=" + VeriErisim.DegerYap(tabloAd) + ")";
            if (!string.IsNullOrWhiteSpace(personelTCKimlikNo))
                sql += " AND TCKIMLIKNO=" + VeriErisim.DegerYap(personelTCKimlikNo);
            if (satirNo > 0)
                sql += " AND SATIRNO=" + VeriErisim.DegerYap(satirNo);
            return this.KomutCalistir(sql, 0);
        }

        public Sonuc EkleMatrah(PersonelMaasMatrah item)
        {
            string sql = "INSERT INTO PERPERSONELMAASMATRAH(TCKIMLIKNO, YIL, SURECNO, MATRAHTUR, TARIH, DEGER, TABLOAD, SATIRNO) VALUES (" + VeriErisim.DegerYap(item.tcKimlikNo) + "," + VeriErisim.DegerYap(item.yil) + "," + VeriErisim.DegerYap(item.surecNo) + "," + VeriErisim.DegerYap(item.matrahTur) + "," + VeriErisim.DegerYap(item.tarih) + "," + VeriErisim.DegerYap(item.deger) + "," + VeriErisim.DegerYap(item.tabloAd) + "," + VeriErisim.DegerYap(item.satirNo) + ")";
            return this.KomutCalistir(sql, 0);
        }

        public IDataReader ListeleMatrah(string personelTCKimlikNo, int yil)
        {
            string sql = "SELECT SURECNO, MATRAHTUR, TARIH, DEGER, TABLOAD, SATIRNO FROM PERPERSONELMAASMATRAH WHERE TCKIMLIKNO=" + VeriErisim.DegerYap(personelTCKimlikNo) + " AND YIL=" + VeriErisim.DegerYap(yil) + " ORDER BY TARIH";
            return this.veriErisim.DataReaderDondur(sql);
        }

        internal static string ListeleMatrahUstTablo(string ustTabloAlan, int yil)
        {
            return "SELECT SURECNO, MATRAHTUR, TARIH, DEGER, TABLOAD, SATIRNO FROM PERPERSONELMAASMATRAH WHERE TCKIMLIKNO=" + ustTabloAlan + " AND YIL >= " + VeriErisim.DegerYap(yil - 1) + " AND YIL <= " + VeriErisim.DegerYap(yil + 1) + " ORDER BY TARIH";
        }

        public IDataReader MatrahListele(TNSDateTime matrahTarih, System.Text.StringBuilder listeStr, int yontem, int matrahTur)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT IKYPERSONEL.TCKIMLIKNO, GELDIGIKURUMSICILI, SUM(PERPERSONELMAASMATRAH.DEGER)");
            sb.Append(" FROM IKYPERSONEL LEFT OUTER JOIN PERPERSONELMAASMATRAH ON ");
            sb.Append(" IKYPERSONEL.TCKIMLIKNO=PERPERSONELMAASMATRAH.TCKIMLIKNO AND PERPERSONELMAASMATRAH.MATRAHTUR=" + matrahTur);
            //sb.Append(" AND PERPERSONELMAASMATRAH.YIL = " + VeriErisim.DegerYap(matrahTarih.Yil));
            sb.Append(" AND PERPERSONELMAASMATRAH.TARIH < " + VeriErisim.DegerYap(matrahTarih));
            sb.Append(" AND PERPERSONELMAASMATRAH.TARIH >= " + VeriErisim.DegerYap(new TNSDateTime(new DateTime(matrahTarih.Yil, 1, 1))));
            sb.Append(" where " + (yontem == 1 ? "IKYPERSONEL.GELDIGIKURUMSICILI" : "IKYPERSONEL.TCKIMLIKNO") + " IN (");
            sb.Append(listeStr);
            sb.Append(")");
            sb.Append(" group by IKYPERSONEL.TCKIMLIKNO, GELDIGIKURUMSICILI");
            sb.Append(" order by " + (yontem == 1 ? "GELDIGIKURUMSICILI" : "IKYPERSONEL.TCKIMLIKNO"));

            return this.veriErisim.DataReaderDondur(sb.ToString());
        }

        public Sonuc MatrahSil(TNSDateTime matrahTarih, int yontem, int matrahTur)
        {
            return this.KomutCalistir("DELETE from PERPERSONELMAASMATRAH where PERPERSONELMAASMATRAH.TARIH >= " + VeriErisim.DegerYap(matrahTarih) + " AND PERPERSONELMAASMATRAH.YIL=" + matrahTarih.Yil + " AND PERPERSONELMAASMATRAH.MATRAHTUR=" + matrahTur, 0);
        }
    }
}
